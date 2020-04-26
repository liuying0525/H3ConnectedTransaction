using OThinker.H3.Controllers;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Configuration;
using System.Data.OracleClient;
using OThinker.H3;
/// <summary>
/// WorkFlowFunction 的摘要说明
/// </summary>

namespace DongZheng.H3.WebApi.Common.Portal
{
    public class WorkFlowFunctionNew
    {
        public static string Str_TDScores = "TDScores";
        public static int TdAutoApprovelScore = 30;
        public static string batchQueryCustomerPrecredit = System.Configuration.ConfigurationManager.AppSettings["batchQueryCustomerPrecredit"] + string.Empty;
        public static string queryTdScoreUrl = System.Configuration.ConfigurationManager.AppSettings["queryTdScoreUrl"] + string.Empty;
        WorkFlowFunction wf = new WorkFlowFunction();
        DongZheng.H3.WebApi.Controllers.Workflow.WorkflowSettingController wsc = new DongZheng.H3.WebApi.Controllers.Workflow.WorkflowSettingController();
        public WorkFlowFunctionNew()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set
            {
                _Engine = value;
            }
        }

        /// <summary>
        /// 融数自动审批通过判断
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public int AutomaticApprovalByRongShu(string instanceid)
        {
            //0不走自动审批，1走自动审批
            int rtn = 1;
            InstanceContext instanceContext = this.Engine.InstanceManager.GetInstanceContext(instanceid);
            if (instanceContext == null)
            {
                rtn = 0;
                return rtn;
            }
            //1.二手车不走自动审批  //condition--资产状况
            string sql = "select condition from I_VEHICLE_DETAIL where parentobjectid='" + instanceContext.BizObjectId + "'";
            DataTable dt_asset = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt_asset) && dt_asset.Rows[0][0].ToString().Contains("U"))
            {
                rtn = 0;
                return rtn;
            }
            //2.公牌贷的时候不走自动审批
            sql = " select a.objectid  from I_APPLICANT_TYPE a inner join I_APPLICATION b on a.parentobjectid=b.objectid ";
            sql += " join OT_Instancecontext c on c.bizobjectid = b.objectid ";
            sql += " where a.applicant_type='C' and b.APPLICATION_TYPE_CODE='00001' and  c.objectid='" + instanceid + "'";
            DataTable dt_appType = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt_appType))
            {
                rtn = 0;
                return rtn;
            }
            //3.只要驳回过的流程，不走自动审批
            sql = string.Format("select count(1) from Ot_Workitemfinished where instanceid='{0}' and actioneventtype=3", instanceid);
            int num = Convert.ToInt32(this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql));
            if (num > 0)
            {
                rtn = 0;
                return rtn;
            }
            //外网经销商
            var user = (OThinker.Organization.User)this.Engine.Organization.GetUnit(instanceContext.Originator);
            var orgName = this.Engine.Organization.GetName(user.ParentID);
            this.Engine.LogWriter.Write("orgName：" + orgName);
            if (orgName != null && orgName.Contains("外网"))
            {
                sql = @"
 select app.application_type_code,app.application_number,det.LEASE_TERM_IN_MONTH,det.AMOUNT_FINANCED,appDet.Id_Card_Typ,appDet.Id_Card_Nbr, 
(select count(1) from i_Applicant_Type t where t.parentobjectid=app.objectid and t.applicant_type='I' and t.main_applicant is null) co_borrower_num from I_application app
left join i_Contract_Detail det on app.objectid=det.parentobjectid
left join i_Applicant_Detail appDet on app.objectid=appDet.parentobjectid and appDet.Identification_Code2='1' and appDet.Id_Card_Typ='00001'
 where app.objectid='{0}' and app.application_type_code='00001'
";
                sql = string.Format(sql, instanceContext.BizObjectId);
                DataTable dt_Contract = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                //个人类型
                if (dt_Contract.Rows.Count > 0)
                {
                    this.Engine.LogWriter.Write("dt_Contract：" + dt_Contract.Rows.Count);
                    DataRow row_Contract = dt_Contract.Rows[0];
                    //4.同盾分数
                    var td_result = wsc.GetCustomSetting(instanceid, Str_TDScores);
                    if (td_result.Data + string.Empty == "")
                    {
                        object obj_paras = new { idCardNo = row_Contract["ID_CARD_NBR"] + string.Empty };
                        var result = DongZheng.H3.WebApi.HttpHelper.PostWebRequest(queryTdScoreUrl, "application/json", JsonConvert.SerializeObject(obj_paras));
                        this.Engine.LogWriter.Write("queryTdScoreResult：" + result);
                        QueryTDScoreResult tdScore = JsonConvert.DeserializeObject<QueryTDScoreResult>(result);
                        var td_write_result = wsc.SetCustomSetting(instanceContext.InstanceId, instanceContext.BizObjectSchemaCode, Str_TDScores, tdScore.data.final_score);
                        this.Engine.LogWriter.Write("保存同盾分数：" + tdScore.data.final_score + ",InstanceID:" + instanceContext.InstanceId + ",Result:" + JsonConvert.SerializeObject(td_write_result.Data));
                        if (tdScore.data.final_score == "" || Convert.ToInt32(tdScore.data.final_score) > TdAutoApprovelScore)
                        {
                            rtn = 0;
                            return rtn;
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(td_result.Data + string.Empty) > TdAutoApprovelScore)
                        {
                            rtn = 0;
                            return rtn;
                        }
                    }

                    #region 5.个人收入负债测算
                    try
                    {
                        this.Engine.LogWriter.Write("客户收入负债测算开始");
                        double dzcarloan = 0;
                        double qs = double.Parse(row_Contract["LEASE_TERM_IN_MONTH"] + string.Empty);//期数
                        double financedamount = double.Parse(row_Contract["AMOUNT_FINANCED"] + string.Empty);//贷款金额
                        double myhk = financedamount / qs;
                        WorkPcmFunction wpf = new WorkPcmFunction();
                        //有共借人
                        if (Convert.ToInt32(row_Contract["CO_BORROWER_NUM"] + string.Empty) > 0)
                        {
                            dzcarloan = myhk * 0.7;
                        }
                        else
                        {
                            dzcarloan = myhk;
                        }
                        //身份证号
                        string stream = wpf.GetCustomerInfo(row_Contract["ID_CARD_NBR"] + string.Empty, dzcarloan.ToString());
                        this.Engine.LogWriter.Write("客户收入负债测算回传值：" + stream);
                        JavaScriptSerializer js = new JavaScriptSerializer();
                        var gcresult = js.Deserialize<hknl_result>(stream);
                        if (gcresult != null && gcresult.Result == "Success")
                        {
                            this.Engine.LogWriter.Write("客户收入负债测算：每期还款额度为" + myhk + "，客户还款能力为" + gcresult.C_RepayLoan);
                            //数据保存
                            double srfzc = double.Parse(gcresult.C_RepayLoan);
                            hknl_model md = new hknl_model();
                            md.APPNO = row_Contract["APPLICATION_NUMBER"] + string.Empty;//单号
                            md.ZJR = row_Contract["ID_CARD_NBR"] + string.Empty;//身份证号
                            md.ZJR_KHYHKNL = gcresult.C_RepayLoan;
                            md.ZJR_KHZCGZ = gcresult.C_AssetValuation;
                            md.ZJR_YSRGZ = gcresult.C_IncomOfMonth;
                            md.ZJR_YYHZW = gcresult.C_DabtsOfMonth;
                            md.QS = qs.ToString();
                            md.DKJE = financedamount.ToString();
                            md.SRFZC = srfzc.ToString("f2");
                            wf.inserthknl(md);
                            if (srfzc < 0)
                            {
                                //客户收入负债差小于0
                                rtn = 0;
                                return rtn;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Engine.LogWriter.Write("客户收入负债测算发生异常：" + ex.Message);
                    }
                    #endregion
                }
            }
            return rtn;
        }

        /// <summary>
        /// 路由字段设值
        /// </summary>
        /// <param name="instanceID"></param>
        /// <returns></returns>
        public RouteResult ApplicationRouter(string instanceID)
        {
            this.Engine.LogWriter.Write("ApplicationRouter：In：instanceID=" + instanceID);

            RouteResult rr = new RouteResult();
            List<DataItemParam> keyValues = new List<DataItemParam>();
            string sql = "  select  a.APPLICATION_TYPE_CODE,b.applicant_type,b.guarantor_type,c.AMOUNT_FINANCED,d.FIRST_THI_NME from Ot_Instancecontext ins inner join I_APPLICATION a on ins.bizobjectid=a.objectid ";
            sql += " inner join I_APPLICANT_TYPE b on b.parentobjectid=a.objectid ";
            sql += " inner join I_CONTRACT_DETAIL c on  c.parentobjectid=a.objectid ";
            sql += " inner join I_APPLICANT_DETAIL d on  d.parentobjectid=a.objectid ";
            sql += " where ins.objectid='" + instanceID + "'";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            WorkFlowFunction wf = new WorkFlowFunction();
            rr.flag_NeedFK = false;//默认不要走风控
            if (CommonFunction.hasData(dt))
            {
                #region 机构贷款的担保人不是个人的时候不需要走风控
                if (dt.Rows[0]["APPLICATION_TYPE_CODE"].ToString() == "00001")//个人走风控
                {
                    rr.flag_NeedFK = true;
                }
                else//机构   机构贷款的担保人是个人的时候需要走风控
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["guarantor_type"] + string.Empty == "I" || row["applicant_type"] + string.Empty == "I")
                        {
                            rr.flag_NeedFK = true;
                        }
                    }
                }
                #endregion

                #region 判断是否要走信贷会  flag_NeedXDH
                if (float.Parse(dt.Rows[0]["AMOUNT_FINANCED"].ToString()) > 2000000)
                {
                    rr.flag_NeedXDH = true;
                }
                else
                {
                    rr.flag_NeedXDH = false;
                }
                #endregion
            }

            this.Engine.LogWriter.Write("ApplicationRouter：Out=" + JsonConvert.SerializeObject(rr));

            return rr;
        }

        /// <summary>
        /// 批量查询同盾报告
        /// </summary>
        /// <param name="instanceID"></param>
        public void BatchQueryTdReport(string instanceID)
        {
            this.Engine.LogWriter.Write("BatchQueryTdReport：instanceID" + instanceID);

            string sql_fkrs = @"select ap.fk_sys_type from ot_instancecontext ot join i_application ap on ot.bizobjectid = ap.objectid where ot.objectid ='" + instanceID + "'";

            DataTable dt_fktype = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_fkrs);

            var fkType = string.Empty;

            try
            {
                fkType = dt_fktype.Rows[0][0].ToString();
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write("BatchQueryTdReport：ex=" + ex.ToString());
            }
            this.Engine.LogWriter.Write("BatchQueryTdReport：fkType=" + fkType);

            //旧融数流程
            if (fkType == "0" || string.IsNullOrEmpty(fkType))
            {
                //增加批量查询
                string sql_get_people = @"
select b.main_applicant,d.FIRST_THI_NME,d.ID_CARD_NBR,d.id_card_typ,d.identification_code2 from Ot_Instancecontext ins 
left join I_APPLICANT_TYPE b on b.parentobjectid=ins.bizobjectid 
left join I_APPLICANT_DETAIL d on  d.parentobjectid=b.parentobjectid and b.identification_code1=d.identification_code2 
where ins.objectid='" + instanceID + "' and (b.applicant_type='I' or b.guarantor_type='I')";
                DataTable dt_all_people = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_get_people);


                List<object> body = new List<object>();
                foreach (DataRow row in dt_all_people.Rows)
                {
                    //主贷人不查询；
                    if (row["MAIN_APPLICANT"] + string.Empty == "Y")
                        continue;
                    //非居民身份证号的不查询；
                    if (row["ID_CARD_TYP"] + string.Empty != "00001")
                        continue;

                    var phoneNumber = getPersonPhone(instanceID, row["identification_code2"] + string.Empty);

                    body.Add(new
                    {
                        name = row["FIRST_THI_NME"] + string.Empty,
                        idCardNo = row["ID_CARD_NBR"] + string.Empty,
                        phone = phoneNumber + string.Empty
                    });
                }
                //需要进行批量查询的；
                if (body.Count > 0)
                {
                    var paras = new
                    {
                        requestSource = "h3",
                        instanceId = instanceID,
                        clientQueryList = body
                    };
                    this.Engine.LogWriter.Write("batchQueryParameters：" + JsonConvert.SerializeObject(paras));
                    var batchQueryResult = DongZheng.H3.WebApi.HttpHelper.PostWebRequest(
                        batchQueryCustomerPrecredit,
                        "application/json",
                        JsonConvert.SerializeObject(paras)
                        );
                    this.Engine.LogWriter.Write("batchQueryResult：" + batchQueryResult);
                }
            }
        }
        /// <summary>
        /// 调用融数接口
        /// </summary>
        /// <param name="instanceid">流程实例id</param>
        /// <returns></returns>
        public string postHttp(string SchemaCode, string instanceid)
        {
            if (instanceid == null || instanceid == "")
            {
                this.Engine.LogWriter.Write("instanceid：" + instanceid);
                return "";
            }
            //融数地址
            String url = ConfigurationManager.AppSettings["rsurl"] + string.Empty;
            //回调Url
            string cbURL = ConfigurationManager.AppSettings["cbURLNew"] + string.Empty;
            if (url == null || url == "" || cbURL == null || cbURL == "")
            {
                this.Engine.LogWriter.Write("调用失败：调用融数url或融数回调url不能为空！");
                return "调用失败：调用url不能为空！";
            }
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt = wf.getInstanceData(SchemaCode, instanceid);
            //通信识别ID
            string reqID = instanceid;
            string rsjson = readMessageFromAttachment(SchemaCode, instanceid, "rsjson");
            //创建时间
            string time = DateTime.Now.ToString();
            if (rsjson == "")
            {
                this.Engine.LogWriter.Write("error：提交融数的数据源为空！" + instanceid + "未走融数风控");
            }
            string[] cyzdArray = new string[3];
            if (dt.Rows[0]["APPLICATION_TYPE_CODE"] + string.Empty == "00001")
            {
                string sql_I = @"
select us.code,app_det.id_card_nbr,con_det.fp_group_name from OT_INSTANCECONTEXT con 
left join I_APPLICANT_DETAIL app_det on app_det.parentobjectid=con.bizobjectid
left join I_CONTRACT_DETAIL con_det on con_det.parentobjectid=con.bizobjectid
left join OT_User us on con.originator=us.objectid
where con.objectid='{0}'
and app_det.identification_code2=1
";
                sql_I = string.Format(sql_I, instanceid);
                
                DataTable dt_I = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_I);
                string I_type = "";
                string ft_group = dt_I.Rows[0]["fp_group_name"] + string.Empty;
                cyzdArray[0] = dt_I.Rows[0]["code"] + string.Empty;
                
                cyzdArray[2] = dt_I.Rows[0]["id_card_nbr"] + string.Empty;

                if (ft_group.Contains("简化") || ft_group.Contains("车秒") || ft_group.Contains("高首付"))
                {
                    I_type = "1";
                }
                else if (ft_group.Contains("机构贷"))
                {
                    I_type = "3";
                }
                else
                {
                    I_type = "2";
                }
                cyzdArray[1] = I_type;
            }
            else
            {
                string sql_C = @"
select us.code,con_det.fp_group_name from OT_INSTANCECONTEXT con 
left join I_CONTRACT_DETAIL con_det on con_det.parentobjectid=con.bizobjectid
left join OT_User us on con.originator=us.objectid
where con.objectid='{0}'
";
                sql_C = string.Format(sql_C, instanceid);

                DataTable dt_C = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_C);
                string C_type = "";
                string ft_group = dt_C.Rows[0]["fp_group_name"] + string.Empty;
                cyzdArray[0] = dt_C.Rows[0]["code"] + string.Empty;

                cyzdArray[2] = "";

                if (ft_group.Contains("简化") || ft_group.Contains("车秒") || ft_group.Contains("高首付"))
                {
                    C_type = "1";
                }
                else if (ft_group.Contains("机构贷"))
                {
                    C_type = "3";
                }
                else
                {
                    C_type = "2";
                }
                cyzdArray[1] = C_type;
            }
            AppUtility.Engine.LogWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(cyzdArray));

            //string[] cyzdArray = dt.Rows[0]["cyzd"].ToString().Split(',');//0-金融专员账号，1-贷款类型，2-申请人证件号码
            string lx = cyzdArray[1];
            //string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"requestId\":\"" + dt.Rows[0]["APPLICATION_NUMBER"].ToString() + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            param += rsjson;
            if (lx != "3")
            {
                ////判断申请人是否有存量的简化贷---待确认
                param += ",\"if_stock_simplified_loan\":\"" + wf.GetJHDBySQR(cyzdArray[2], dt.Rows[0]["APPLICATION_NUMBER"].ToString().Trim()) + "\",";// wf.GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString().Trim(), dt.Rows[0]["applicationNo"].ToString().Trim()) + "\",";
                //获取敞口值：申请人+配偶+本次贷款金额---待确认
                string ck = "0";
                string lastapplication = getLastApplicationNumByIdCard(cyzdArray[2]);
                if (lastapplication != "")
                {
                    ck = getckAll(cyzdArray[2], lastapplication);
                    //ck = wf.indvidualHistoryFinancedAmount(cyzdArray[2], dt.Rows[0]["APPLICATION_NUMBER"].ToString().Trim(), dt.Rows[0]["financedamount"].ToString().Trim()); 
                }
                param += "\"exposure_value\":\"" + ck + "\",";
                //客户现有银行号 [Caccountnum]--待确认
                param += "\"bank_no\":\"" + dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";// dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";
                //是否属于优质经销商
                param += "\"if_quality_dealer_code\":\"" + wf.yzjxs(cyzdArray[0]).Trim()+ "\"";
            }
            param += "}}}";
            this.Engine.LogWriter.Write("调用融数风控系统开始！");
            this.Engine.LogWriter.Write("param参数值：" + param);
            //推送数据给融数
            var result = "";
            try
            {
                result = wf.PostMoths(url, param);
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write("融数风控调用出错" + ex.Message.ToString());
            }
            this.Engine.LogWriter.Write(result.ToString());
            return result.ToString();

        }
        public string postHttpByManual(string SchemaCode, string instanceid, string manual, string url)
        {
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt = wf.getInstanceData(SchemaCode, instanceid);
            //通信识别ID
            string reqID = instanceid;
            string rsjson = readMessageFromAttachment(SchemaCode, instanceid, "rsjson");
            //创建时间
            string time = DateTime.Now.ToString();
            if (rsjson == "")
            {
                this.Engine.LogWriter.Write("error：提交融数的数据源为空！" + instanceid + "未走融数风控");
            }
            string[] cyzdArray = dt.Rows[0]["cyzd"].ToString().Split(',');//0-金融专员账号，1-贷款类型，2-申请人证件号码
            string lx = cyzdArray[1];
            //string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"requestId\":\"" + dt.Rows[0]["APPLICATION_NUMBER"].ToString() + "\",\"cbURL\":\"\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"manual\":\"" + manual + "\",\"param\":{";
            param += rsjson;
            if (lx != "3")
            {
                ////判断申请人是否有存量的简化贷---待确认
                param += ",\"if_stock_simplified_loan\":\"" + wf.GetJHDBySQR(cyzdArray[2], dt.Rows[0]["APPLICATION_NUMBER"].ToString().Trim()) + "\",";// wf.GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString().Trim(), dt.Rows[0]["applicationNo"].ToString().Trim()) + "\",";
                //获取敞口值：申请人+配偶+本次贷款金额---待确认
                string ck = "0";
                string lastapplication = getLastApplicationNumByIdCard(cyzdArray[2]);
                if (lastapplication != "")
                {
                    ck = getckAll(cyzdArray[2], lastapplication);
                    //ck = wf.indvidualHistoryFinancedAmount(cyzdArray[2], dt.Rows[0]["APPLICATION_NUMBER"].ToString().Trim(), dt.Rows[0]["financedamount"].ToString().Trim()); 
                }
                param += "\"exposure_value\":\"" + ck + "\",";
                //客户现有银行号 [Caccountnum]--待确认
                param += "\"bank_no\":\"" + dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";// dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";
                //是否属于优质经销商
                param += "\"if_quality_dealer_code\":\"" + wf.yzjxs(cyzdArray[0]).Trim() + "\""; 
            }
            param += "}}}";
            this.Engine.LogWriter.Write("调用融数风控系统开始！");
            this.Engine.LogWriter.Write("param参数值：" + param);
            //推送数据给融数
            var result = "";
            try
            {
                result = wf.PostMoths(url, param);
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write("融数风控调用出错" + ex.Message.ToString());
            }
            this.Engine.LogWriter.Write(result.ToString());
            return result.ToString();

        }
        /// <summary>
        /// 自动分单
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="activeCode"></param>
        /// <param name="cyz"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string BizAutoAssign(string instanceID, string activeCode, string cyz, string amount)
        {
            if (!(cyz == null || cyz == ""))
            {
                string lzsql = " select a.state,a.servicestate,b.name from OT_User a inner join Ot_Organizationunit b";
                lzsql += " on a.parentid=b.objectid  where (a.state=0 or a.servicestate=2) and  a.objectid='" + cyz + "'";
                DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(lzsql);
                if (dt == null || dt.Rows.Count < 1)
                {
                    return cyz;
                }
            }
            if (instanceID == null || instanceID == "")
            {
                this.Engine.LogWriter.Write("instanceid为空！");
                return "";
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "instanceID:" + instanceID + "amount:" + amount);
            string userID = "", dep = "", post = "";
            string sql = " ";
            switch (activeCode)
            {
                //信贷初审 信贷终审  运营初审 运营终审
                case "信审初审": dep = "信贷审批部"; post = "初审"; break;
                case "信审终审": dep = "信贷审批部"; post = "终审"; break;
                case "运营初审": dep = "运营部"; post = "初审"; break;
                case "运营终审": dep = "运营部"; post = "终审"; break;
                case "运营预审": dep = "运营部"; post = "预审"; break;
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + " instanceID：" + instanceID);
            if (dep.Contains("信贷"))
            {
                WorkFlowFunction wf = new WorkFlowFunction();
                if (decimal.Parse(amount) > 1200000 && post == "终审")//信审终审大于120万
                {
                    string[] arrayUser = wf.getGroupUserByCondition("信审终审", "自动分单");
                    if (arrayUser != null && arrayUser.Length > 0)
                    {
                        userID = arrayUser[0];
                    }
                    else
                    {
                        userID = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";//发给管理员
                        this.Engine.LogWriter.Write("自动分单结果，instanceid：" + instanceID + "金额大于90万，未找到信审终审的人员");
                    }

                    return userID;
                }
                string type = "";
                DataTable dtInstanceData = wf.getInstanceData(instanceID);
                if (dtInstanceData.Rows[0]["USER_NAME"].ToString().StartsWith("98") || dtInstanceData.Rows[0]["USER_NAME"].ToString().StartsWith("80000"))
                {
                    type = "内";
                }
                else
                {
                    type = "外";
                }
                sql += " select d.yh,d.xm,d.gw,  nvl(sum(d.shxs),0) counter from (";
                sql += " select  d.yh,d.xm,d.gw,d.shxs from ( ";
                sql += " SELECT ow1.PARTICIPANT,TO_CHAR (TRIM (OW1.PARTICIPANTNAME)) username,      case when  OW1.ACTIVITYCODE='Activity13' then '终审'         when  OW1.ACTIVITYCODE='Activity14' then '初审' end gw,           OW1.INSTANCEID                             FROM                               (SELECT OW.instanceid,ow.PARTICIPANT,                                        OW.PARTICIPANTNAME,                                       OW.ACTIVITYCODE,                                          OW.DISPLAYNAME,                                          OW.RECEIVETIME,                                          OW.FINISHTIME,                                           OW.ACTIONNAME,                              ROW_NUMBER ()                                           OVER (PARTITION BY OW.ACTIVITYCODE,OW.instanceid                                               ORDER BY OW.RECEIVETIME DESC) RN                             FROM                           ( (SELECT instanceid,PARTICIPANT,                                         PARTICIPANTNAME,                                           ACTIVITYCODE,                                          DISPLAYNAME,                                           RECEIVETIME,                                           FINISHTIME,                                          ACTIONNAME                                      FROM OT_Workitemfinished                                    WHERE     workflowCode IN ('RetailApp','CompanyApp','APPLICATION')                                         AND PARTICIPANTNAME <> '系统管理员'                                        AND TO_CHAR (RECEIVETIME, 'YYYYMMDD') >                                               TO_CHAR (SYSDATE-1, 'YYYYMMDD')                                        AND ACTIVITYCODE in ( 'Activity14'，'Activity13')                                        )                         UNION ALL                      (SELECT instanceid,PARTICIPANT,                                          PARTICIPANTNAME,                                         ACTIVITYCODE,                                            DISPLAYNAME,                                           RECEIVETIME,                                           FINISHTIME,                                            ACTIONNAME                                      FROM OT_Workitem                                   WHERE     workflowCode IN ('RetailApp','CompanyApp','APPLICATION')                                        AND PARTICIPANTNAME <> '系统管理员'                                        AND TO_CHAR (RECEIVETIME, 'YYYYMMDD') >                                                TO_CHAR (SYSDATE-1, 'YYYYMMDD')                                         AND ACTIVITYCODE in ( 'Activity14'，'Activity13')                                         ) ) OW) OW1";
                sql += "  WHERE OW1.RN = 1 ";
                sql += "  ) a right join I_xsyyshryxx d  on   d.yh=a.participant ";
                sql += " inner join  Ot_User e on e.objectid=d.yh";
                sql += "  where 1=1  ";
                sql += " and e.state=1 and e.servicestate=0  ";
                sql += " and d.nww like '%" + type + "%'";
                sql += " and d.zt='上班'";
                sql += " and d.gw='" + post + "'";
                sql += " and d.bm='" + dep + "'";
                sql += " and d.shedend >= " + amount + "";
                sql += " and d.shedstart <  " + amount + "";
                sql += " ) d ";
                sql += " group by d.yh,d.xm,d.gw ";
                sql += " order by counter asc";
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "信贷 sql：" + sql);
            }
            else if (dep.Contains("运营"))
            {
                sql += " select OBJECTID,NAME,sum(COUNTER) countor from (";
                sql += " select * from (";
                sql += " select c.name,c.objectid,count(d.PARTICIPANT) COUNTER from ";
                sql += "  (select B.NAME,B.OBJECTID from I_XSYYSHRYXX a ";
                sql += "  inner join OT_User b on A.zh=B.CODE ";
                if (post == "预审")
                {
                    sql += "  where  a.gw='" + post + "'  and a.zt='上班' and b.state=1 and b.servicestate=0  and b.objectid not in (";
                }
                else
                {
                    sql += "  where  a.bm='" + dep + "' and a.gw='" + post + "'  and a.zt='上班' and b.state=1 and b.servicestate=0  and b.objectid not in (";
                }
                sql += "  select PARTICIPANT from  OT_WORKITEMFINISHED where instanceid='" + instanceID + "')";
                sql += "  ) c left join OT_WORKITEM d on C.OBJECTID =D.PARTICIPANT";
                sql += "  and d.workflowcode in ('RetailApp','CompanyApp','APPLICATION') and d.receivetime>to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00','yyyy-mm-dd hh24:mi:ss'";
                sql += "  )";
                sql += " group by c.NAME,C.OBJECTID,D.PARTICIPANT ORDER BY COUNTER ASC )";
                sql += " union all";
                sql += " select * from (";
                sql += " select c.name,c.objectid,count(d.PARTICIPANT) COUNTER from ";
                sql += "  (select B.NAME,B.OBJECTID from I_XSYYSHRYXX a ";
                sql += "  inner join OT_User b on A.zh=B.CODE ";
                if (post == "预审")
                {
                    sql += "  where  a.gw='" + post + "' and a.zt='上班' and b.state=1 and b.servicestate=0  and b.objectid not in (";
                }
                else
                {
                    sql += "  where  a.bm='" + dep + "' and a.gw='" + post + "' and a.zt='上班' and b.state=1 and b.servicestate=0  and b.objectid not in (";
                }
                
                sql += "  select PARTICIPANT from  OT_WORKITEMFINISHED where instanceid='" + instanceID + "')";
                sql += "  ) c left join OT_WORKITEMFINISHED d on C.OBJECTID =D.PARTICIPANT";
                sql += "  and d.workflowcode in ('RetailApp','CompanyApp','APPLICATION') and d.receivetime>to_date('" + DateTime.Now.ToString("yyyy-MM-dd") + " 00:00:00','yyyy-mm-dd hh24:mi:ss'";
                sql += "  )";
                sql += " group by c.NAME,C.OBJECTID,D.PARTICIPANT ORDER BY COUNTER ASC )";
                sql += " ) group by NAME,OBJECTID ORDER BY countor ASC ";
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 运管sql：" + sql);
            }
            try
            {
                DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    userID = dt.Rows[0][0].ToString();
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 自动分单结果：" + userID);
                }
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + " 自动分单sql报错：" + ex.ToString());
            }
            if (userID.Trim() == "")
            {
                string gw="";
                if (dep.Contains("信贷"))
                {
                    gw="信审管理员";
                }
                else if(dep.Contains("运营"))
                {
                    gw="运营管理员";
                }
                string[] defualtUser = wf.getGroupUserByCondition(gw, "自动分单");
                if (defualtUser != null && defualtUser.Length > 0)
                {
                    userID = defualtUser[0];
                }
                else
                {
                    userID = "18f923a7-5a5e-426d-94ae-a55ad1a4b239";//发给管理员
                    this.Engine.LogWriter.Write("自动分单结果，instanceid：" + instanceID + "未找到"+gw);
                }
            }
            return userID;
        }
        /// <summary>
        /// 根据证件号获取申请人的最新的申请单号
        /// </summary>
        /// <param name="IdCardNum"></param>
        /// <returns></returns>
        public string getLastApplicationNumByIdCard(string IdCardNum)
        {
            string rtn = "";
            string sql = "select  a.application_number, a.application_date, ad.ID_CARD_NBR";
            sql+=" from APPLICATION a";
            sql += " JOIN APPLICANT_DETAIL AD ON AD.APPLICATION_NUMBER = a.APPLICATION_NUMBER AND AD.IDENTIFICATION_CODE = '1'";
            sql+=" Join (";
            sql+="  select count(*) cc ,ad.id_card_nbr id_card_nbr ,max( to_number(substr(a.application_number,5))) max_number ";
            sql+="  from application a";
            sql+="  JOIN APPLICANT_DETAIL AD ON AD.APPLICATION_NUMBER = a.APPLICATION_NUMBER AND AD.IDENTIFICATION_CODE = '1'";
            sql+="   where a.status_code in ('05','06') ";
            sql+="  group by ad.id_card_nbr";
            sql+=" ) aa on aa.id_card_nbr = ad.id_card_nbr and aa.max_number = to_number(substr(a.application_number,5))";
            sql += " where ad.ID_CARD_NBR ='" + IdCardNum + "'";
            DataTable dt = new WorkFlowFunction().ExecuteDataTableSql("cap", sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.Rows[0][0].ToString();
            }
            else
            {
                this.Engine.LogWriter.Write("敞口查询错误：通过身份证号-" + IdCardNum + "未找到最新的申请单！");
            }
            return rtn;
        }
        /// <summary>
        /// 获取所有敞口信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="applicationnum"></param>
        /// <returns></returns>
        public string getckAll(string idcard,string applicationnum)
        {
            string rtn = "";
            string sql = "";
            sql+=" select sum(cnt) cnt from (";
            sql+=" select round(ce.net_financed_amt) cnt";
            sql+=" from CAP_EXPOSURE ce";
            sql+=" join contract_detail cd on cd.application_number=ce.application_number";
            sql+=" join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id    ";
            sql+=" join asset_make_code am  on am.asset_make_cde = ce.asset_make_cde     ";
            sql+=" join asset_brand_code ab on ab.asset_brand_cde = ce.asset_brand_cde   ";
            sql+=" join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde ";
            sql+=" where 1=1 ";
            sql+=" and ce.exp_applicant_card_id='"+idcard+"'";
            //sql+=" --and ce.is_exposed_ind='T'";
            sql+=" and ce.application_number='"+applicationnum+"'";
            sql+=" ";
            sql+=" union all ";
            sql+=" select  round(cme.principle_outstanding_amt) cnt";
            sql+=" from CMS_EXPOSURE cme ";
            sql+=" join request_status_code rs on rs.request_status_cde=cme.contract_status_cde";
            sql+=" JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number     ";
            sql+=" JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = app.BUSINESS_PARTNER_ID         ";
            sql+=" join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id         ";
            sql+=" join asset_make_code am  on am.asset_make_cde = cme.asset_make_cde          ";
            sql+=" join asset_brand_code ab on ab.asset_brand_cde = cme.asset_brand_cde        ";
            sql+=" join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde      ";
            sql+=" where 1=1";
            sql+=" and cme.application_number='"+applicationnum+"'";
            //sql+=" --and IS_EXPOSED_IND = 'T'";
            sql+=" and cme.exp_applicant_card_id='"+idcard+"'";
            sql+=" ";
            sql+=" union all";
            sql+=" ";
            sql+=" select round(ce.net_financed_amt) cnt";
            sql+=" from CAP_EXPOSURE ce";
            sql+=" join contract_detail cd on cd.application_number=ce.application_number";
            sql+=" join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id    ";
            sql+=" join asset_make_code am  on am.asset_make_cde = ce.asset_make_cde     ";
            sql+=" join asset_brand_code ab on ab.asset_brand_cde = ce.asset_brand_cde   ";
            sql+=" join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde ";
            sql+=" where 1=1";
            sql+=" and ce.application_number='"+applicationnum+"'";
            sql+=" and ce.identification_code in (2,3)";
            sql+=" ";
            sql+=" union all";
            sql+=" ";
            sql+=" select  round(cme.principle_outstanding_amt) cnt";
            sql+=" from CMS_EXPOSURE cme";
            sql+=" join request_status_code rs on rs.request_status_cde=cme.contract_status_cde";
            sql+=" JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number     ";
            sql+=" JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = app.BUSINESS_PARTNER_ID         ";
            sql+=" join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id         ";
            sql+=" join asset_make_code am  on am.asset_make_cde = cme.asset_make_cde          ";
            sql+=" join asset_brand_code ab on ab.asset_brand_cde = cme.asset_brand_cde        ";
            sql+=" join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde      ";
            sql+=" where 1=1";
            sql+=" and cme.application_number='"+applicationnum+"'";
            sql+=" and cme.identification_code in (2,3)";
            sql += " ) a ";
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt= wf.ExecuteDataTableSql("cap", sql);
            if (CommonFunction.hasData(dt))
            {
                rtn=dt.Rows[0][0].ToString();
            }
            return rtn;
        }
        //附件保存
        public string saveMessageToAttachment(string BizObjectSchemaCode, string BizObjectId, string DataField, string MessageInfo)
        {
            string rtn = "";
            byte[] Content = System.Text.Encoding.Default.GetBytes(MessageInfo);// Encoding.Unicode.GetBytes(MessageInfo);
            Attachment attachment = new Attachment();
            attachment.BizObjectSchemaCode = BizObjectSchemaCode;
            attachment.BizObjectId = BizObjectId;
            attachment.DataField = DataField;
            attachment.Content = Content;
            attachment.CreatedBy = "";
            attachment.CreatedTime = DateTime.Now;
            attachment.ContentLength = Content.Length;
            attachment.ContentType = "text/plain ";
            attachment.LastVersion = true;
            attachment.FileName=BizObjectId+".txt";
            attachment.FileFlag = 0;
            rtn = this.Engine.BizObjectManager.AddAttachment(attachment);
            return rtn;
        }

        public string readMessageFromAttachment(string BizObjectSchemaCode,string BizObjectId, string DataField)
        {
            string rtn = "";
            string attachmentId="";
            string sql="select objectid from Ot_Attachment where Datafield='"+DataField+"' and bizobjectid='"+BizObjectId+"' order by createdtime desc";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if(CommonFunction.hasData(dt))
            {
                attachmentId=dt.Rows[0][0].ToString();
            }
            Attachment attachment = new Attachment();
            attachment = this.Engine.BizObjectManager.GetAttachment("", "", BizObjectId, attachmentId);
            byte[] Content = attachment.Content;
            rtn = System.Text.Encoding.Default.GetString(Content);
            return rtn;
        }

        /// <summary>
        /// 生成调用融数接口的参数（测试用）
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        public string getRSParameters(string SchemaCode, string instanceid, string cburl)
        {
            if (instanceid == null || instanceid == "")
            {
                this.Engine.LogWriter.Write("instanceid：" + instanceid);
                return "";
            }
            WorkFlowFunction wf = new WorkFlowFunction();
            DataTable dt = wf.getInstanceData(SchemaCode, instanceid);
            //通信识别ID
            string reqID = instanceid;
            string rsjson = readMessageFromAttachment(SchemaCode, instanceid, "rsjson");
            //创建时间
            string time = DateTime.Now.ToString();
            if (rsjson == "")
            {
                this.Engine.LogWriter.Write("error：提交融数的数据源为空！" + instanceid + "未走融数风控");
            }
            string[] cyzdArray = new string[3];
            if (dt.Rows[0]["APPLICATION_TYPE_CODE"] + string.Empty == "00001")
            {
                string sql_I = @"
select us.code,app_det.id_card_nbr,con_det.fp_group_name from OT_INSTANCECONTEXT con 
left join I_APPLICANT_DETAIL app_det on app_det.parentobjectid=con.bizobjectid
left join I_CONTRACT_DETAIL con_det on con_det.parentobjectid=con.bizobjectid
left join OT_User us on con.originator=us.objectid
where con.objectid='{0}'
and app_det.identification_code2=1
";
                sql_I = string.Format(sql_I, instanceid);

                DataTable dt_I = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_I);
                string I_type = "";
                string ft_group = dt_I.Rows[0]["fp_group_name"] + string.Empty;
                cyzdArray[0] = dt_I.Rows[0]["code"] + string.Empty;

                cyzdArray[2] = dt_I.Rows[0]["id_card_nbr"] + string.Empty;

                if (ft_group.Contains("简化") || ft_group.Contains("车秒") || ft_group.Contains("高首付"))
                {
                    I_type = "1";
                }
                else if (ft_group.Contains("机构贷"))
                {
                    I_type = "3";
                }
                else
                {
                    I_type = "2";
                }
                cyzdArray[1] = I_type;
            }
            else
            {
                string sql_C = @"
select us.code,con_det.fp_group_name from OT_INSTANCECONTEXT con 
left join I_CONTRACT_DETAIL con_det on con_det.parentobjectid=con.bizobjectid
left join OT_User us on con.originator=us.objectid
where con.objectid='{0}'
";
                sql_C = string.Format(sql_C, instanceid);

                DataTable dt_C = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_C);
                string C_type = "";
                string ft_group = dt_C.Rows[0]["fp_group_name"] + string.Empty;
                cyzdArray[0] = dt_C.Rows[0]["code"] + string.Empty;

                cyzdArray[2] = "";

                if (ft_group.Contains("简化") || ft_group.Contains("车秒") || ft_group.Contains("高首付"))
                {
                    C_type = "1";
                }
                else if (ft_group.Contains("机构贷"))
                {
                    C_type = "3";
                }
                else
                {
                    C_type = "2";
                }
                cyzdArray[1] = C_type;
            }
            AppUtility.Engine.LogWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(cyzdArray));

            //string[] cyzdArray = dt.Rows[0]["cyzd"].ToString().Split(',');//0-金融专员账号，1-贷款类型，2-申请人证件号码
            string lx = cyzdArray[1];
            //string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"cbURL\":\"" + cbURL + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            string param = "{\"request\":{\"repID\":\"" + instanceid + "\",\"requestId\":\"" + dt.Rows[0]["APPLICATION_NUMBER"].ToString() + "\",\"cbURL\":\"" + cburl + "\",\"time\":\"" + time + "\",\"type\":\"" + lx + "\",\"param\":{";
            param += rsjson;
            if (lx != "3")
            {
                ////判断申请人是否有存量的简化贷---待确认
                param += ",\"if_stock_simplified_loan\":\"" + wf.GetJHDBySQR(cyzdArray[2], dt.Rows[0]["APPLICATION_NUMBER"].ToString().Trim()) + "\",";// wf.GetJHDBySQR(dt.Rows[0]["IdCardNo"].ToString().Trim(), dt.Rows[0]["applicationNo"].ToString().Trim()) + "\",";
                //获取敞口值：申请人+配偶+本次贷款金额---待确认
                string ck = "0";
                string lastapplication = getLastApplicationNumByIdCard(cyzdArray[2]);
                if (lastapplication != "")
                {
                    ck = getckAll(cyzdArray[2], lastapplication);
                    //ck = wf.indvidualHistoryFinancedAmount(cyzdArray[2], dt.Rows[0]["APPLICATION_NUMBER"].ToString().Trim(), dt.Rows[0]["financedamount"].ToString().Trim()); 
                }
                param += "\"exposure_value\":\"" + ck + "\",";
                //客户现有银行号 [Caccountnum]--待确认
                param += "\"bank_no\":\"" + dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";// dt.Rows[0]["Caccountnum"].ToString().Trim() + "\",";
                //是否属于优质经销商
                param += "\"if_quality_dealer_code\":\"" + wf.yzjxs(cyzdArray[0]).Trim() + "\"";
            }
            param += "}}}";
            return param;
        }

        /// <summary>
        /// 获取人员手机号
        /// </summary>
        /// <param name="instanceID"></param>
        /// <param name="identificationCode"></param>
        /// <returns></returns>
        public string getPersonPhone(string instanceID,string identificationCode)
        {
            var phoneSql = @"select tel.PHONE_NUMBER from i_Applicant_Detail d 
 left join I_APPLICANT_PHONE_FAX tel on d.parentobjectid=tel.parentobjectid and d.identification_code2=tel.identification_code5 and tel.phone_type_cde='00003'  where d.parentobjectid='{0}' and d.identification_code2='{1}' ";

            InstanceContext ic = OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.GetInstanceContext(instanceID);

            phoneSql = string.Format(phoneSql, ic.BizObjectId, identificationCode);
            DataTable phoneDt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(phoneSql);

            var phoneNumber = string.Empty;
            if (CommonFunction.hasData(phoneDt))
            {
                phoneNumber = phoneDt.Rows[0][0].ToString();
            }

            
            return phoneNumber;
        }

        #region Class
        public class RouteResult
        {
            public bool flag_NeedFK { get; set; }
            public bool flag_NeedXDH { get; set; }
        }

        public class QueryTDScoreResult
        {
            public string code { get; set; }
            public string message { get; set; }
            public string sessionId { get; set; }
            public bool success { get; set; }
            public Score data { get; set; }

            public class Score
            {
                public string final_score { get; set; }
            }
        }
        #endregion

    }
}