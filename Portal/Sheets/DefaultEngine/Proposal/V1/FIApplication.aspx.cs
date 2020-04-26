using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OThinker.H3.Controllers;
using System.Collections.Generic;
using System.Data.OracleClient;
using OThinker.H3.DataModel;
using System.Linq;
using Newtonsoft.Json;
using System.Threading.Tasks;
using OThinker.H3.Controllers.MvcSheet;

namespace OThinker.H3.Portal.Sheets.DefaultEngine
{
    public partial class FIApplication : OThinker.H3.Controllers.MvcPage
    {
        public static string connecting_code = System.Configuration.ConfigurationManager.AppSettings["CAP_Retail"] + string.Empty;
        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        /// <summary>
        /// 保存表单数据到引擎中
        /// </summary>
        /// <param name="Args"></param>
        public override void SaveDataFields(MvcPostValue MvcPost, MvcResult result)
        {
            try
            {
                MvcDataItem type = new MvcDataItem();
                MvcPost.BizObject.DataItems.TryGetValue("APPLICANT_TYPE", out type);
                var dataJson = JsonConvert.SerializeObject(MvcPost.BizObject.DataItems);
                var r = JsonConvert.DeserializeObject<List<System.Collections.Generic.Dictionary<object, object>>>(JsonConvert.SerializeObject(type.V));
                if (r.Count() > 0)
                {
                    var name = r[0]["NAME1"] + string.Empty;
                    string msg = "";
                    bool isInject = new DongZheng.H3.WebApi.Controllers.XssAttribute().IsContainXSSCharacter(name, out msg);
                    if (isInject)
                    {
                        result.Successful = false;
                        result.Errors.Add("检测到SQL敏感字符");
                        return;
                    }
                    isInject = new DongZheng.H3.WebApi.Controllers.SqlInjectAttribute().IsSqlInjectCharacter(name, out msg);
                    if (isInject)
                    {
                        result.Successful = false;
                        result.Errors.Add("检测到XSS敏感字符");
                        return;
                    }
                }

            }
            catch (Exception ex)
            {

            }
            
            // 保存后，后台执行事件
            base.SaveDataFields(MvcPost, result);

            string Command = Request["Command"] + string.Empty;

            //1.判断是否成功保存
            if (result.Successful && Command.ToLower() == MvcController.Button_Submit)
            {
                var version = 1;
                var tokenId = 1;
                var fields = this.ActionContext.Schema.Fields;
                var sheetDataType = this.ActionContext.SheetDataType;
                var clientActivity = (H3.WorkflowTemplate.ClientActivity)this.ActionContext.ActivityTemplate;
                var context = this.ActionContext.Engine.InstanceManager.GetInstanceContext(this.ActionContext.InstanceId);
                if (context != null)
                {
                    var tokens = context.GetTokens("Activity2", Instance.TokenState.Unspecified).OrderByDescending(p => p.CreatedTime);
                    version = tokens.Count() == 0 ? 1 : tokens.Count();
                    tokenId = tokens.Count() == 0 ? 1 : tokens.FirstOrDefault().TokenId;
                }
                var instanceId = this.ActionContext.InstanceId;
                //2.记录数据变动日志
                Task.Run(() =>
                {
                    var trackResult = new DataLogger().DataTrack(MvcPost, fields, sheetDataType, clientActivity);
                    string sql = "insert into H3.c_fidatatrack(objectid,instanceid,verson,activitycode,datatrack,tokenid,createdtime) values('" + Guid.NewGuid().ToString() + "','" + instanceId + "','" + version + "','Activity2',:content,'" + tokenId + "',to_date('" + DateTime.Now + "','yyyy/mm/dd HH24:MI:SS'))";
                    try
                    {
                        var i = 0;
                        string connectionCode = "Engine";
                        var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                        OracleConnection connection = new OracleConnection(dbObject.DbConnectionString);
                        connection.Open();
                        OracleCommand Cmd = new OracleCommand(sql, connection);
                        OracleParameter Temp = new OracleParameter("content", OracleType.NClob);
                        Temp.Direction = ParameterDirection.Input;
                        Temp.Value = trackResult;
                        Cmd.Parameters.Add(Temp);
                        i = Cmd.ExecuteNonQuery();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        AppUtility.Engine.LogWriter.Write("保存风控报告数据异常：" + ex.ToString());
                    }
                }).GetAwaiter();

            }
        }
        public object get_product_parameter(string product_id, string qs)
        {
            if (!string.IsNullOrEmpty(qs))
            {
                string sql_qs = "select distinct MINIMUM_LEASE_TRM,MAXIMUN_LEASE_TRM from V_FP_PARAMETER_MAIN_ALL  where financial_product_id=" + product_id;
                DataTable dt_qs = ExecuteDataTable(sql_qs);
                if (string.IsNullOrEmpty(qs))
                    return new { Success = false, Data = "", Message = "产品ID：" + product_id + "未查询到数据" };
                //判断qs是否在dt_qs的期数范围内
                if (Convert.ToDouble(dt_qs.Rows[0]["MINIMUM_LEASE_TRM"]) <= Convert.ToDouble(qs) &&
                     Convert.ToDouble(dt_qs.Rows[0]["MAXIMUN_LEASE_TRM"]) >= Convert.ToDouble(qs))
                {
                    //qs合法，使用此数值进行计算
                }
                else    //期数不符合设定，使用默认值
                {
                    qs = dt_qs.Rows[0]["MINIMUM_LEASE_TRM"] + string.Empty;//使用最小值作为最小期数
                }
            }
            else
            {
                string sql_qs = "select distinct MINIMUM_LEASE_TRM from V_FP_PARAMETER_MAIN_ALL  where financial_product_id=" + product_id;
                qs = ExecuteScalar(sql_qs) + string.Empty;
                if (string.IsNullOrEmpty(qs))
                    return new { Success = false, Data = "", Message = "产品ID：" + product_id + "未查询到数据" };
            }

            string sql_get_data = "select * from V_FP_PARAMETER_MAIN_ALL where financial_product_id={0} and minimum_term_mm<={1}  and maximum_term_mm>={1}";
            sql_get_data = string.Format(sql_get_data, product_id, qs);
            DataTable dt = ExecuteDataTable(sql_get_data);
            if (dt == null || dt.Rows.Count == 0)
                return new { Success = false, Data = "", Message = "无产品参数，请联系系统管理" };
            DataRow row = dt.Rows[0];
            var data = new
            {
                FINANCIAL_PRODUCT_ID = product_id,
                FINANCIAL_PRODUCT_NME = row["FINANCIAL_PRODUCT_NME"] + string.Empty,
                MINIMUM_FINANCING_AMT = Convert.ToDecimal(row["MINIMUM_FINANCING_AMT"] + string.Empty),
                MAXIMUM_FINANCING_AMT = Convert.ToDecimal(row["MAXIMUM_FINANCING_AMT"] + string.Empty),
                MINIMUM_LEASE_TRM = Convert.ToInt32(row["MINIMUM_LEASE_TRM"] + string.Empty),
                MAXIMUN_LEASE_TRM = Convert.ToInt32(row["MAXIMUN_LEASE_TRM"] + string.Empty),
                AVAILABLE_TRM = qs,//
                MAXIMUM_FINANCING_PCT = Convert.ToDouble(row["MAXIMUM_FINANCING_PCT"] + string.Empty),
                FUTURE_VALUE_TYP = row["FUTURE_VALUE_TYP"] + string.Empty,
                MINIMUM_RV_PCT = (row["FUTURE_VALUE_TYP"] + string.Empty) == "N" ? 0 : Convert.ToDouble(row["MINIMUM_RV_PCT"] + string.Empty),
                MAXIMUM_RV_PCT = (row["FUTURE_VALUE_TYP"] + string.Empty) == "N" ? 0 : Convert.ToDouble(row["MAXIMUM_RV_PCT"] + string.Empty),
                RV_EDITABLE_IND = row["RV_EDITABLE_IND"] + string.Empty,
                RV_PCT = (row["FUTURE_VALUE_TYP"] + string.Empty) == "N" ? 0 : Convert.ToDouble(row["RV_PCT"] + string.Empty)
            };
            return new
            {
                Success = true,
                Data = data,
                Message = ""
            };
        }

        public object get_HK_plan(string total_price, string dkje, string wkje, string dkqs, string product_id)
        {
            this.ActionContext.Engine.LogWriter.Write("总价：" + total_price + "贷款金额：" + dkje + ",尾款金额：" + wkje + ",贷款期数：" + dkqs + ",产品编号：" + product_id);
            string sql_get_type = "select zgettype(" + product_id + ") from dual";
            DataTable dt_type = ExecuteDataTable(sql_get_type);
            double para_t = 0;
            decimal para_r = 0;//客户利率
            decimal para_actual_rte = 0;//实际利率
            if (dt_type.Rows.Count > 0)
                para_t = Convert.ToDouble(dt_type.Rows[0][0] + string.Empty);
            string sql_get_r = "select distinct customer_rte/100 customer_rte,actual_rte from V_FP_PARAMETER_MAIN_ALL v where v.financial_product_id=" + product_id + " and minimum_term_mm<= " + dkqs + " and maximum_term_mm>= " + dkqs;
            DataTable dt_r = ExecuteDataTable(sql_get_r);
            if (dt_r.Rows.Count > 0)
            {
                para_r = Convert.ToDecimal(dt_r.Rows[0]["customer_rte"]);
                para_actual_rte = Convert.ToDecimal(dt_r.Rows[0]["actual_rte"]);
            }
            this.ActionContext.Engine.LogWriter.Write("贷款金额：" + dkje + ",尾款金额：" + wkje + ",贷款期数：" + dkqs + ",产品编号：" + product_id + ",T：" + para_t + "，客户利率：" + para_r + "，实际利率：" + para_actual_rte);
            #region 生成还款计划
            OracleParameter[] paras = new[] {
                new OracleParameter("D",OracleType.Number),
                new OracleParameter("W",OracleType.Number),
                new OracleParameter("M",OracleType.Number),
                new OracleParameter("R",OracleType.Number),
                new OracleParameter("T",OracleType.Number),
                new OracleParameter("FP",OracleType.Number),
                new OracleParameter("o_plan_id",OracleType.Number)
            };
            paras[0].Direction = ParameterDirection.Input;
            paras[1].Direction = ParameterDirection.Input;
            paras[2].Direction = ParameterDirection.Input;
            paras[3].Direction = ParameterDirection.Input;
            paras[4].Direction = ParameterDirection.Input;
            paras[5].Direction = ParameterDirection.Input;
            paras[6].Direction = ParameterDirection.Output;

            paras[0].Value = Convert.ToDouble(dkje);
            paras[1].Value = Convert.ToDouble(wkje);
            paras[2].Value = Convert.ToDouble(dkqs);
            paras[3].Value = para_r;
            paras[4].Value = para_t;
            paras[5].Value = Convert.ToDouble(product_id);
            string message = "";
            var result = RunProcedure("zgeneral_plan", paras);
            string plan_id = result[6].Value + string.Empty;
            this.ActionContext.Engine.LogWriter.Write("还款计划ID：" + plan_id);
            #endregion

            #region 生成应付、未偿、利息总额等数值；
            OracleParameter[] paras_fin_data = new[] {
                new OracleParameter("p_plan_id",OracleType.Number),
                new OracleParameter("p_sale_price",OracleType.Number),
                new OracleParameter("p_actual_rte",OracleType.Number),
                new OracleParameter("o_result_code",OracleType.Number)
            };
            paras_fin_data[0].Direction = ParameterDirection.Input;
            paras_fin_data[1].Direction = ParameterDirection.Input;
            paras_fin_data[2].Direction = ParameterDirection.Input;
            paras_fin_data[3].Direction = ParameterDirection.Output;
            paras_fin_data[0].Value = plan_id;
            paras_fin_data[1].Value = total_price;
            paras_fin_data[2].Value = para_actual_rte;
            var result_fin_data = RunProcedure("CALC_FIN_DATA", paras_fin_data);
            this.ActionContext.Engine.LogWriter.Write("CALC_FIN_DATA存储过程执行结果：" + result_fin_data[3].Value);
            #endregion


            //查询还款计划
            string sql_get_plan = "select * from V_PLAN_DT v where v.plan_id=" + plan_id + " order by TERM_ID";
            DataTable dt_plan = ExecuteDataTable(sql_get_plan);
            //查询利息总额
            //string sql_total_assetitc = "select sum(assetitc) total_assetitc  from V_PLAN_DT where plan_id=" + plan_id;
            //DataTable dt_total_assetitc = ExecuteDataTable(sql_total_assetitc);
            string sql_zgen_plan = "select * from zgen_plan where plan_id=" + plan_id;
            DataTable dt_zgen_plan = ExecuteDataTable(sql_zgen_plan);
            List<object> plan_detail = new List<object>();
            List<object> plan_simple = new List<object>();
            List<object> plan_info = new List<object>();
            //double r = Convert.ToDouble(dt_plan.Rows[0]["R"] + string.Empty) * 100;
            //double total_assetitc = (dt_total_assetitc.Rows[0][0] + string.Empty) == "" ? 0 : Convert.ToDouble(dt_total_assetitc.Rows[0][0] + string.Empty);
            int start_term = 1;
            int end_term = 1;
            int seq_id = 1;
            double pmt = Convert.ToDouble(dt_plan.Rows[0]["PMT"]);
            foreach (DataRow row in dt_plan.Rows)
            {
                plan_detail.Add(new
                {
                    Plan_ID = row["PLAN_ID"] + string.Empty,
                    D = Convert.ToDouble(row["D"] + string.Empty),
                    W = row["W"] + string.Empty,
                    M = row["M"] + string.Empty,
                    R = Convert.ToDouble(row["R"] + string.Empty) * 100,
                    T = row["T"] + string.Empty,
                    TERM_ID = row["TERM_ID"] + string.Empty,
                    TOTAL_RECEIVE = Convert.ToDouble(row["TOTAL_RECEIVE"] + string.Empty),
                    PRINCIPAL = Convert.ToDouble(row["PRINCIPAL"] + string.Empty),
                    ASSETITC = Convert.ToDouble(row["ASSETITC"] + string.Empty),
                    PMT = Convert.ToDouble(row["PMT"] + string.Empty)
                });
                if (pmt == Convert.ToDouble(row["PMT"]))
                {
                    end_term = Convert.ToInt32(row["TERM_ID"]);
                }
                else
                {
                    //如果不一样,把记录写到plan_simple
                    plan_simple.Add(new
                    {
                        RENTAL_DETAIL_SEQ = seq_id,
                        START_TRM = start_term,
                        END_TRM = end_term,
                        RENTAL_AMT = pmt.ToString("0.00"),
                        EQUAL_RENTAL_AMT = pmt.ToString("0.00"),
                        INTEREST_RTE = (para_r * 100).ToString("0.0000")
                    });
                    //重启记录一条新的数据
                    start_term = Convert.ToInt32(row["TERM_ID"]);
                    end_term = start_term;
                    pmt = Convert.ToDouble(row["PMT"]);
                    seq_id++;
                }
            }

            plan_simple.Add(new
            {
                RENTAL_DETAIL_SEQ = seq_id,
                START_TRM = start_term,
                END_TRM = end_term,
                RENTAL_AMT = pmt.ToString("0.00"),
                EQUAL_RENTAL_AMT = pmt.ToString("0.00"),
                INTEREST_RTE = (para_r * 100).ToString("0.0000")
            });
            
            DataRow zgen_row = dt_zgen_plan.Rows[0];
            return new
            {
                Success = true,
                Plan_ID = plan_id,
                RTE = (para_r * 100).ToString("0.0000"),
                ACTUAL_RTE = para_actual_rte.ToString("0.0000"),
                TX_RTE = (para_actual_rte - para_r * 100).ToString("0.0000"),
                PlanInfo = plan_info,
                Plan_Detail = plan_detail,
                Plan_Simple = plan_simple,
                SUBSIDY_AMT = Convert.ToDecimal((zgen_row["SUBSIDY_AMT"] + string.Empty) == "" ? 0 : zgen_row["SUBSIDY_AMT"]),
                TOTAL_PAY_AMT = Convert.ToDecimal((zgen_row["TOTAL_PAY_AMT"] + string.Empty) == "" ? 0 : zgen_row["TOTAL_PAY_AMT"]),
                TOTAL_NO_PAY_AMT = Convert.ToDecimal((zgen_row["TOTAL_NO_PAY_AMT"] + string.Empty) == "" ? 0 : zgen_row["TOTAL_NO_PAY_AMT"]),
                TOTAL_ASSETITC = Convert.ToDecimal((zgen_row["TOTAL_ASSETITC"] + string.Empty) == "" ? 0 : zgen_row["TOTAL_ASSETITC"]),
                Message = message
            };
        }

        #region Basic Method
        public DataTable ExecuteDataTable(string sql)
        {
            try
            {
                string connectionCode = connecting_code;
                var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteDataTable(sql);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public object ExecuteScalar(string sql)
        {
            try
            {
                string connectionCode = connecting_code;
                var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteScalar(sql);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public OracleParameterCollection RunProcedure(string StoredProcedureName, OracleParameter[] Parameters)
        {
            string connectionCode = "CAPDB";
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            OracleConnection conn = new OracleConnection(dbObject.DbConnectionString);
            try
            {
                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedureName;
                cmd.Parameters.AddRange(Parameters);
                cmd.ExecuteNonQuery();
                conn.Close();
                return cmd.Parameters;
            }
            catch (Exception ex)
            {
                conn.Close();
                AppUtility.Engine.LogWriter.Write("RunProcedure执行存储过程方法异常：" + ex.ToString());
                return null;
            }

        }
        #endregion
    }
}
