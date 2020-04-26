using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OracleClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Threading;


namespace DongZheng.H3.WebApi.Common.Portal
{
    public class CAP_Fuc
    {
        public static string cap_db_connectcode = System.Configuration.ConfigurationManager.AppSettings["CAP_Retail"] + string.Empty;

        /// <summary>
        /// 获取CAP敞口信息
        /// </summary>
        /// <param name="identifycode">个人：身份证号，机构：组织机构代码</param>
        /// <param name="name">个人：姓名，机构：公司名称</param>
        /// <param name="application_no">申请号:Br-XXXX</param>
        /// <param name="app_type">个人：I，机构：C</param>
        /// <returns></returns>
        public DataTable Get_CAP_EXPOSURE(string identifycode, string name, string application_no, string app_type)
        {
            try
            {
                AppUtility.Engine.LogWriter.Write("获取CAP敞口信息：identifycode->" + identifycode + ",name->" + name +
                    ",application_no->" + application_no + ",app_type->" + app_type);
                OracleParameter[] paras = new[]
                {
                new OracleParameter("CAPCUR",OracleType.Cursor),
                new OracleParameter("P_OLD_ID",OracleType.VarChar,15),
                new OracleParameter("P_NEW_ID",OracleType.VarChar,18),
                new OracleParameter("P_NAME",OracleType.VarChar,50),
                new OracleParameter("P_APPLICANT_TYPE_IND",OracleType.VarChar,1),
                new OracleParameter("P_APPLICATION_NUMBER",OracleType.VarChar,50),
                new OracleParameter("P_ID_Type",OracleType.VarChar,50)
            };
                paras[0].Direction = ParameterDirection.Output;
                paras[1].Direction = ParameterDirection.Input;
                paras[2].Direction = ParameterDirection.Input;
                paras[3].Direction = ParameterDirection.Input;
                paras[4].Direction = ParameterDirection.Input;
                paras[5].Direction = ParameterDirection.Input;
                paras[6].Direction = ParameterDirection.Input;

                if (app_type == "I")//个人 是身份证：有15位，18位的区别
                {
                    if (identifycode.Length == 15)
                    {
                        paras[1].Value = identifycode;
                        paras[2].Value = "";
                    }

                    else if (identifycode.Length == 18)
                    {
                        paras[1].Value = "";
                        paras[2].Value = identifycode;
                    }
                }
                else
                {
                    paras[1].Value = "";
                    paras[2].Value = identifycode;
                }
                paras[3].Value = name;
                paras[4].Value = app_type;
                paras[5].Value = application_no;
                paras[6].Value = "00001";
                var ds = RunProcedureReturnDataSet(cap_db_connectcode, "ZCAPSEARCHCAP_EXPOSURE_REF", paras);
                if (ds != null)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("获取CAP敞口信息异常：" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 获取CMS敞口信息
        /// </summary>
        /// <param name="application_no">申请号:Br-XXXX</param>
        /// <returns></returns>
        public DataTable Get_CMS_EXPOSURE(string application_no)
        {
            try
            {
                AppUtility.Engine.LogWriter.Write("获取CMSP敞口信息：application_no->" + application_no);
                OracleParameter[] paras = new[]
                {
                new OracleParameter("CapRefCursor",OracleType.Cursor),
                new OracleParameter("P_APPLICATION_NUMBER",OracleType.VarChar,50),
                new OracleParameter("P_EXP_TYPE_IND",OracleType.VarChar,18)
            };
                paras[0].Direction = ParameterDirection.Output;
                paras[1].Direction = ParameterDirection.Input;
                paras[2].Direction = ParameterDirection.Input;


                paras[1].Value = application_no;
                paras[2].Value = "N";
                var ds = RunProcedureReturnDataSet(cap_db_connectcode, "ZCAPCMS_EXPOSURESELECT1_REF", paras);
                if (ds != null)
                {
                    return ds.Tables[0];
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("获取CMS敞口信息异常：" + ex.ToString());
                return null;
            }
        }

        /// <summary>
        /// 数据写回到CAP系统
        /// </summary>
        /// <param name="biz_objectid">BizObjectID</param>
        /// <param name="plan_id">还款计划ID</param>
        /// <param name="application_no">申请号</param>
        /// <returns></returns>
        public static ToCapResult WriteDataToCAP(string biz_objectid, string plan_id, string application_no)
        {
            AppUtility.Engine.LogWriter.Write("WriteToCAP参数：biz_objectid->" + biz_objectid + ",plan_id->" + plan_id + ",application_no->" + application_no);
            //没有申请号：发起，给个默认值-1；
            if (string.IsNullOrEmpty(application_no))
                application_no = "-1";
            OracleParameter[] paras = new[]
            {
                new OracleParameter("p_object_id",OracleType.VarChar,100),
                new OracleParameter("p_plan_id",OracleType.Number),
                new OracleParameter("o_application_number",OracleType.VarChar,20),
                new OracleParameter("o_result_code",OracleType.Number),
                new OracleParameter("o_debug_id",OracleType.Number),
                new OracleParameter("o_error_info",OracleType.VarChar,2000)
            };
            paras[0].Direction = ParameterDirection.Input;
            paras[1].Direction = ParameterDirection.Input;
            paras[2].Direction = ParameterDirection.InputOutput;
            paras[3].Direction = ParameterDirection.Output;
            paras[4].Direction = ParameterDirection.Output;
            paras[5].Direction = ParameterDirection.Output;

            paras[0].Value = biz_objectid;
            paras[1].Value = plan_id;
            paras[2].Value = application_no;
            ToCapResult ret = new ToCapResult();
            var result = RunProcedure(cap_db_connectcode, "ZSAVEAPP_REQ", paras);
            if (result != null)
            {
                AppUtility.Engine.LogWriter.Write("写回到CAP系统，BizObjectID-->" + biz_objectid +
                    "：ZSAVEAPP_REQ返回值Application_Number->" + result[2].Value + "，Result_Code-->" + result[3].Value +
                    ",Debug ID-->" + result[4].Value + ",Error Info-->" + result[5].Value);
                
                ret.Result_Code = result[3].Value + string.Empty;
                if (ret.Result_Code == "1")//写入到CAP成功，使用返回的单号
                {
                    ret.Application_Number = result[2].Value + string.Empty;
                    #region 推送订单给CRM
                    try
                    {
                        CRM_Order cRM_Order = new CRM_Order(biz_objectid, ret.Application_Number);
                        cRM_Order.PostOrder();
                        //Thread thread = new Thread(new ThreadStart(cRM_Order.PostOrder));
                        //thread.Start();
                    }
                    catch (Exception ex)
                    {
                        AppUtility.Engine.LogWriter.Write("CAP_Fuc，CRM订单推送异常，BizObjectID-->" + biz_objectid+"，"+ex.Message);
                    }
                    #endregion

                    #region 推送给接口中心
                    try
                    {
                        string sqlGetUuid = "select uuid from i_application where objectid='{0}'";
                        sqlGetUuid = string.Format(sqlGetUuid, biz_objectid);
                        string uuid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetUuid) + string.Empty;
                        if (!string.IsNullOrEmpty(uuid))
                        {
                            string para = Newtonsoft.Json.JsonConvert.SerializeObject(new
                            {
                                Uuid = uuid,
                                ApplicationNumber = ret.Application_Number
                            });
                            string toapiresult = DongZheng.H3.WebApi.HttpHelper.PostWebRequest(ConfigurationManager.AppSettings["apiUpdateApplicationNum"], "application/json", para);
                            AppUtility.Engine.LogWriter.Write("推送到接口中心结果-->" + toapiresult + ",参数-->" + para);
                            ExternalSysIgnore(biz_objectid);
                        }
                    }
                    catch (Exception ex)
                    {
                        AppUtility.Engine.LogWriter.Write("CAP_Fuc，推送接口中心异常，BizObjectID-->" + biz_objectid + "，" + ex.ToString());
                    }
                    #endregion
                }
                else//失败还是用入参的单号作为返回；
                {
                    ret.Application_Number = application_no;
                }
            }
            else
            {
                ret.Application_Number = application_no;
                ret.Result_Code = "-10000";//执行存储过程异常
            }
            return ret;
        }

        /// <summary>
        /// 外部系统发起的跳过上传附件节点
        /// </summary>
        /// <param name="biz_objectid"></param>
        public static void ExternalSysIgnore(string biz_objectid)
        {
            string sql = "select wi.objectid from ot_workitem wi left join ot_instancecontext con on wi.instanceid = con.objectid where con.bizobjectid = '" + biz_objectid + "' and wi.activitycode='Activity8'";
            string taskid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            if (string.IsNullOrEmpty(taskid))
            {
                AppUtility.Engine.LogWriter.Write("任务id为空，bizobjectid:" + biz_objectid);
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("自动提交到下一环节，任务：" + taskid);
                WorkFlowFunction wf = new WorkFlowFunction();
                wf.SubmitWorkItem(taskid, "");
            }
        }

        //获取产品类型
        public static List<FinancialType> GetFinancialType(string asset_condition, string bp_id, string group_id, string model_cde, string app_type)
        {
            var result = new List<FinancialType>();
            if (string.IsNullOrEmpty(bp_id) || string.IsNullOrEmpty(group_id) || string.IsNullOrEmpty(model_cde))
                return result;
            string asset_code = "";
            string type = "";
            if (app_type == "00001")
                type = "I";
            else if (app_type == "00002")
                type = "C";
            else
                return result;

            if (asset_condition == "N")
                asset_code = "00001";
            else if (asset_condition == "U")
                asset_code = "00002";
            else
                return result;

            string sql = @"
select distinct financial_product_id,financial_product_nme financial_product_dsc from V_FP_ASSET_DEALER_LOCAL v 
where v.business_partner_id={0} 
and v.asset_model_cde='{2}'
and v.fp_group_id={1}
and v.FLEET_COMP_INDIV_IND='{3}'
and v.asset_condition_cde='{4}'
order by financial_product_nme 
";
            sql = string.Format(sql, bp_id, group_id, model_cde, type, asset_code);
            AppUtility.Engine.LogWriter.Write("获取产品sql-->" + sql);
            DataTable dt = ExecuteDataTableSql(cap_db_connectcode, sql);
            foreach (DataRow dr in dt.Rows)
            {
                var v = new FinancialType();
                v.financial_product_id = dr["financial_product_id"] + string.Empty;
                v.financial_product_dsc = dr["financial_product_dsc"] + string.Empty;
                result.Add(v);
            }
            return result;
        }

        public string GetCAPStateCode(string app_no)
        {
            string sql = "select STATUS_CODE from Application@To_Cms where application_number='" + app_no + "'";
            string code = ExecuteScalar(cap_db_connectcode, sql) + string.Empty;
            AppUtility.Engine.LogWriter.Write("GetCAPStateCode sql-->" + sql + ",value-->" + code);
            return code;
        }

        public bool SetStateTo06(string app_num, string reason)
        {
            OracleParameter[] paras = new[]
                {
                new OracleParameter("p_application_number",OracleType.VarChar,30),
                new OracleParameter("p_change_reason",OracleType.VarChar,200),
                new OracleParameter("o_result_code",OracleType.Number)
                };
            paras[0].Direction = ParameterDirection.Input;
            paras[1].Direction = ParameterDirection.Input;
            paras[2].Direction = ParameterDirection.Output;

            paras[0].Value = app_num;
            paras[1].Value = reason;
            RunProcedure(cap_db_connectcode, "ZCHANGE_APP_STATUS_97_06", paras);
            AppUtility.Engine.LogWriter.Write("ZCHANGE_APP_STATUS_97_06 Result-->" + paras[2].Value);
            return paras[2].Value + string.Empty == "1";
        }

        #region Class
        /// <summary>
        /// CAP敞口结果类
        /// </summary>
        public class CAP_EXPOSURE
        {
            public string APPLICATION_STATUS_CDE { get; set; }
            public string APPLICATION_STATUS_DTE { get; set; }
            public string ASSET_BRAND_DSC { get; set; }
            public string ASSET_MAKE_DSC { get; set; }
            public string ASSET_MODEL_DSC { get; set; }
            public string EXP_APPLICANT_CARD_ID { get; set; }
            public string EXP_APPLICANT_NAME { get; set; }
            public string EXP_APPLICATION_NUMBER { get; set; }
            public string FP_GROUP_DSC { get; set; }
            public string NO_OF_TERMS { get; set; }
            public string NET_FINANCED_AMT { get; set; }
        }
        /// <summary>
        /// CMS敞口结果类
        /// </summary>
        public class CMS_EXPOSURE
        {
            public string PRINCIPLE_OUTSTANDING_AMT { get; set; }
            public string EXP_APPLICATION_NUMBER { get; set; }
            public string CONTRACT_NUMBER { get; set; }
            public string APPLICATION_NUMBER { get; set; }
            public string IDENTIFICATION_CODE { get; set; }
            public string LINE_NBR { get; set; }
            public string CONTRACT_ID { get; set; }
            public string CONTRACT_STATUS_CDE { get; set; }
            public string CONTRACT_STATUS_DSC { get; set; }
            public string CONTRACT_DTE { get; set; }
            public string CONTRACT_TYPE_CDE { get; set; }
            public string APPLICANT_ID { get; set; }

            //, , , , , , , , , , , , APPLICANT_ROLE_IND, EXP_APPLICATION_NUMBER, EXP_APPLICANT_CARD_ID, EXP_APPLICANT_NAME, EXP_APPLICANT_TYPE_IND, EXP_APPLICANT_ROLE_IND, EXP_INACTIVE_IND, IS_EXPOSED_IND, IS_CONTINGENT_IND, EXP_TYPE_IND, LIVE_STS, SEARCH_TYPE_IND, SEARCH_VALUE, ASSET_MAKE_CDE, ASSET_MAKE_DSC, ASSET_BRAND_CDE, ASSET_BRAND_DSC, ASSET_MODEL_CDE, ASSET_MODEL_DSC, FP_GROUP_ID, FP_GROUP_DSC, FINANCIAL_PRODUCT_ID, FINANCIAL_PRODUCT_DSC, NET_FINANCED_AMT, PRINCIPLE_OUTSTANDING_AMT, INTEREST_RATE, NO_OF_TERMS, NO_OF_TERMS_PAID, OVERDUE_30_DAYS, OVERDUE_60_DAYS, OVERDUE_90_DAYS, OVERDUE_ABOVE_90_DAYS, OVERDUE_ABOVE_120_DAYS, FACILITY_TYPE_IND, SYS_IDENTIFIED_IND, BP_DEALER_ID, BP_DEALER_NAME
        }

        /// <summary>
        /// 产品类型
        /// </summary>
        public class FinancialType
        {
            public string financial_product_id { get; set; }
            public string financial_product_dsc { get; set; }
        }
        

        public class ToCapResult
        {
            public string Application_Number { get; set; }
            public string Result_Code { get; set; }
            public string Result_Msg
            {
                get
                {
                    switch (Result_Code)
                    {
                        case "1": return "写入成功";
                        case "-10000": return "调用存储过程异常，请参照系统日志";
                        //...
                        default: return "写入失败";
                    }
                }
            }
        }
        #endregion

        #region Basic Method
        public static OracleParameterCollection RunProcedure(string connectionCode,string StoredProcedureName, OracleParameter[] Parameters)
        {
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

        public static DataSet RunProcedureReturnDataSet(string connectionCode,string StoredProcedureName, OracleParameter[] Parameters)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            OracleConnection conn = new OracleConnection(dbObject.DbConnectionString);
            try
            {
                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = StoredProcedureName;
                cmd.Parameters.AddRange(Parameters);
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                DataSet ds = new DataSet();
                da.Fill(ds);
                conn.Close();
                return ds;
            }
            catch (Exception ex)
            {
                conn.Close();
                AppUtility.Engine.LogWriter.Write("RunProcedure执行存储过程方法异常：" + ex.ToString());
                return null;
            }
        }

        public static DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }

        public static object ExecuteScalar(string connectionCode, string sql)
        {
            var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteScalar(sql);
            }
            return null;
        }
        #endregion
    }


}