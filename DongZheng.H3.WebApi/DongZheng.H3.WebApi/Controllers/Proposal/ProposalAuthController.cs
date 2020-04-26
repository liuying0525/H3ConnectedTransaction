using DongZheng.H3.WebApi.Common;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Proposal
{
    [ValidateInput(false)]
    [Xss]
    public class ProposalAuthController : CustomController
    {
        public string Cap_DB = System.Configuration.ConfigurationManager.AppSettings["CAP_Retail"] + string.Empty;
        //系统日结开始时间
        string dayendStartTime = AppUtility.Engine.SettingManager.GetCustomSetting("SysStopFromTime");
        //系统日结结束时间
        string dayendEndTime = AppUtility.Engine.SettingManager.GetCustomSetting("SysStopToTime");
        //系统日结提示信息
        string dayendMessage = AppUtility.Engine.SettingManager.GetCustomSetting("SysStopMessage");

        string queryTdReportUrl = System.Configuration.ConfigurationManager.AppSettings["queryTdReportUrl"] + string.Empty;

        public override string FunctionCode
        {
            get { return ""; }
        }

        public string Index()
        {
            return "Proposal service";
        }

        //贷款列表，获取数据
        public JsonResult GetProposalDetail(PagerInfo pagerInfo, string app_no, string app_name, string from_dte, string to_dte)
        {
            #region order by 条件
            string OrderBy = "ORDER BY ";
            if (pagerInfo.iSortCol_0 == 4)
            {
                OrderBy += "app.createdtime " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 10)
            {
                OrderBy += "con_det.AMOUNT_FINANCED " + pagerInfo.sSortDir_0.ToUpper();
            }
            else
            {
                OrderBy += "app.createdtime " + pagerInfo.sSortDir_0.ToUpper();
            }
            #endregion
            #region 过滤条件
            string condition = "";
            if (!string.IsNullOrEmpty(app_no))
            {
                condition += string.Format(" and app.application_number like '%{0}%'", app_no);
            }
            if (!string.IsNullOrEmpty(app_name))
            {
                condition += string.Format(" and app.application_name like '%{0}%'", app_name);
            }
            if (!string.IsNullOrEmpty(from_dte))
            {
                condition += string.Format(" and app.createdtime > to_date('{0}','yyyy-mm-dd')", from_dte);
            }
            if (!string.IsNullOrEmpty(to_dte))
            {
                condition += string.Format(" and app.createdtime < to_date('{0}','yyyy-mm-dd')", to_dte);
            }
            #endregion

            #region 
            string sql_get_dtl = @"
select * from (
select ROW_NUMBER() OVER ( {0} ) AS ROWNUMBER_ , con.objectid INSTANCEID,app.APPLICATION_NUMBER,
app.APPLICATION_NAME,app.CREATEDTIME,con.STATE,app.APPLICATION_TYPE_CODE,'' CAP_STATE,asset.ASSET_MAKE_DSC,
asset.POWER_PARAMETER,con_det.FP_GROUP_NAME,con_det.FINANCIAL_PRODUCT_NAME,con_det.AMOUNT_FINANCED from i_Application app
left join ot_instancecontext con on app.objectid=con.bizobjectid
left join i_vehicle_detail asset on app.objectid=asset.parentobjectid
left join i_contract_detail con_det on app.objectid=con_det.parentobjectid
where con.state is not null and con.originator='{4}' {3}
) T
WHERE   RowNumber_ >= {1}
        AND RowNumber_ <= {2}
";
            string sql_get_num = @"
select count(1) from i_Application app
left join ot_instancecontext con on app.objectid=con.bizobjectid
where con.state is not null and con.originator='{1}' {0}
";
            #endregion

            sql_get_dtl = string.Format(sql_get_dtl, OrderBy, pagerInfo.StartIndex, pagerInfo.EndIndex, condition, this.UserValidator.UserID);
            sql_get_num = string.Format(sql_get_num, condition, this.UserValidator.UserID);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_get_dtl);
            string ids = "";
            DataTable dtCapNo = new DataTable();
            foreach (DataRow row in dt.Rows)
            {
                ids += "'" + row["APPLICATION_NUMBER"] + "',";
            }
            if (ids != "")
            {
                ids = ids.Substring(0, ids.Length - 1);
                string sql_get_cap_no = "select * from V_APPLICATION_STATUS where application_number in ({0})";
                dtCapNo = DBHelper.ExecuteDataTableSql(Cap_DB, string.Format(sql_get_cap_no, ids));
            }

            List<object> rows = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                var state_rows = dtCapNo.Select("APPLICATION_NUMBER='" + row["APPLICATION_NUMBER"] + "'");
                string cap_state = "";
                if (state_rows.Count() > 0)
                {
                    cap_state = state_rows[0]["STATUS_CODE"] + string.Empty;
                }
                rows.Add(new
                {
                    ROWNUMBER_ = row["ROWNUMBER_"],
                    INSTANCEID = row["INSTANCEID"],
                    APPLICATION_NUMBER = row["APPLICATION_NUMBER"],
                    APPLICATION_NAME = row["APPLICATION_NAME"],
                    CREATEDTIME = Convert.ToDateTime(row["CREATEDTIME"]).ToString("yyyy-MM-dd"),
                    STATE = row["STATE"],
                    APPLICATION_TYPE_CODE = row["APPLICATION_TYPE_CODE"],
                    CAP_STATE = cap_state,
                    ASSET_MAKE_DSC = row["ASSET_MAKE_DSC"],
                    POWER_PARAMETER = row["POWER_PARAMETER"],
                    FP_GROUP_NAME = row["FP_GROUP_NAME"],
                    FINANCIAL_PRODUCT_NAME = row["FINANCIAL_PRODUCT_NAME"],
                    AMOUNT_FINANCED = row["AMOUNT_FINANCED"]
                });
            }
            string num = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_get_num) + string.Empty;
            return Json(new { Rows = rows, Total = num, iTotalDisplayRecords = num, iTotalRecords = num, sEcho = pagerInfo.sEcho }, JsonRequestBehavior.AllowGet);
        }
        //获取还款计划明细列表数据
//        public JsonResult GetPMTDetail(string plan_id)
//        {
//            string sql = "select * from V_PLAN_DT v where v.plan_id=" + plan_id + " order by TERM_ID";
//            DataTable dt = DBHelper.ExecuteDataTableSql(Cap_DB, sql);
//            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
//        }

//        //获取还款计划明细列表总数
//        public JsonResult GetPMTSum(string plan_id)
//        {
//            string sql = "select max(TERM_ID) TERM_ID,sum(principal) principal,sum(assetitc) assetitc,sum(pmt) pmt from V_PLAN_DT v where v.plan_id=" + plan_id;
//            DataTable dt = DBHelper.ExecuteDataTableSql(Cap_DB, sql);
//            return Json(Newtonsoft.Json.JsonConvert.SerializeObject(dt), JsonRequestBehavior.AllowGet);
//        }

//        //获取敞口数据
//        public JsonResult Get_EXPOSURE(string application_number)
//        {
//            string sql_get_userType = "select * from APPLICANT_TYPE@to_cms where application_number='" + application_number + "' order by identification_code";
//            DataTable dt_userType = DBHelper.ExecuteDataTableSql(Cap_DB, sql_get_userType);
//            List<object> result = new List<object>();
//            decimal total_FZ = 0;//负债

//            foreach (DataRow user in dt_userType.Rows)
//            {
//                #region CAP
//                #region sql
//                string sql = @"
//select round(ce.net_financed_amt,2) net_financed_amt,ce.exp_application_number,ce.application_status_cde,ce.exp_applicant_role_ind
//,ce.exp_applicant_card_id,ce.exp_applicant_name,fpg.fp_group_nme,ce.application_status_dte,ce.no_of_terms
//,am.asset_make_dsc,amc.asset_model_dsc,ab.asset_brand_dsc,ce.applicant_role_ind,ce.sys_identified_ind,ce.is_exposed_ind,ce.is_contingent_ind
//from CAP_EXPOSURE@to_cms ce
//join contract_detail@to_cms cd on cd.application_number=ce.application_number
//join financial_product_group@to_cms fpg on ce.fp_group_id=fpg.fp_group_id
//join asset_make_code@to_cms am  on am.asset_make_cde = ce.asset_make_cde
//join asset_brand_code@to_cms ab on ab.asset_brand_cde = ce.asset_brand_cde
//join ASSET_MODEL_CODE@to_cms AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde
//where 1=1 
//and ce.application_number='{0}' and ce.identification_code='{1}'
//order by 2 desc
//";
//                #endregion
//                DataTable dt = DBHelper.ExecuteDataTableSql(Cap_DB, string.Format(sql, application_number, user["IDENTIFICATION_CODE"]));
//                string type = "";
//                if (user["MAIN_APPLICANT"] + string.Empty == "Y")
//                {
//                    type = "B";
//                }
//                else if (user["APPLICANT_TYPE"] + string.Empty != "")
//                {
//                    type = "C";
//                }
//                else if (user["guarantor_type"] + string.Empty != "")
//                {
//                    type = "G";
//                }
//                List<CAP_EXPOSURE> objs = new List<CAP_EXPOSURE>();
//                foreach (DataRow row in dt.Rows)
//                {
//                    var obj = new CAP_EXPOSURE
//                    {
//                        NET_FINANCED_AMT = Convert.ToDecimal(row["NET_FINANCED_AMT"]).ToString("N2"),//融资额
//                        EXP_APPLICATION_NUMBER = row["EXP_APPLICATION_NUMBER"] + string.Empty,//申请号
//                        APPLICATION_STATUS_CDE = row["APPLICATION_STATUS_CDE"] + string.Empty,//状态编码
//                        EXP_APPLICANT_ROLE_IND = row["EXP_APPLICANT_ROLE_IND"] + string.Empty,//角色编码

//                        EXP_APPLICANT_CARD_ID = row["EXP_APPLICANT_CARD_ID"] + string.Empty,//ID/注册号
//                        EXP_APPLICANT_NAME = row["EXP_APPLICANT_NAME"] + string.Empty,//申请人姓名
//                        FP_GROUP_NME = row["FP_GROUP_NME"] + string.Empty,//融资类型
//                        TYPE = (row["EXP_APPLICATION_NUMBER"] + string.Empty) == application_number ? "This Request" : "Old Request",//类型
//                        APPLICATION_STATUS_DTE = (row["APPLICATION_STATUS_DTE"] + string.Empty) == "" ? "" : Convert.ToDateTime(row["APPLICATION_STATUS_DTE"]).ToString("yyyy-MM-dd"),//状态日期

//                        NO_OF_TERMS = row["NO_OF_TERMS"] + string.Empty,//期数
//                        ASSET_MAKE_DSC = row["ASSET_MAKE_DSC"] + string.Empty,//生产商
//                        ASSET_MODEL_DSC = row["ASSET_MODEL_DSC"] + string.Empty,//动力参数
//                        ASSET_BRAND_DSC = row["ASSET_BRAND_DSC"] + string.Empty,//车型

//                        APPLICANT_ROLE_IND = row["APPLICANT_ROLE_IND"] + string.Empty,
//                        SYS_IDENTIFIED_IND = row["SYS_IDENTIFIED_IND"] + string.Empty,
//                        IS_EXPOSED_IND = row["IS_EXPOSED_IND"] + string.Empty,
//                        IS_CONTINGENT_IND = row["IS_CONTINGENT_IND"] + string.Empty
//                    };
//                    objs.Add(obj);
//                }
//                #endregion

//                #region CMS
//                #region sql
//                string sql_cms = @"
//select distinct round(cme.principle_outstanding_amt,2) principle_outstanding_amt, cme.exp_application_number,cme.contract_number,rs.request_status_dsc
//,cme.contract_status_cde,cme.exp_inactive_ind,cme.exp_applicant_role_ind,cme.exp_applicant_card_id,cme.exp_applicant_name
//,BM.BUSINESS_PARTNER_NME,fpg.fp_group_nme,cme.contract_dte,round(cme.interest_rate,2) interest_rate,round(cme.net_financed_amt,2) net_financed_amt
//,am.asset_make_dsc,amc.asset_model_dsc,ab.asset_brand_dsc,cme.overdue_30_days,cme.overdue_60_days,cme.overdue_90_days
//,cme.overdue_above_90_days,cme.overdue_above_120_days,cme.no_of_terms,cme.no_of_terms_paid,cme.applicant_role_ind,cme.sys_identified_ind,cme.is_exposed_ind,cme.is_contingent_ind
// from CMS_EXPOSURE@To_Cms cme 
// join request_status_code@to_cms rs on rs.request_status_cde=cme.contract_status_cde
// JOIN APPLICATION@To_Cms app ON app.APPLICATION_NUMBER = cme.application_number
// JOIN BP_MAIN@To_Cms BM ON BM.BUSINESS_PARTNER_ID = cme.BP_DEALER_ID
// join financial_product_group@To_Cms fpg on cme.fp_group_id=fpg.fp_group_id 
// join asset_make_code@To_Cms am  on am.asset_make_cde = cme.asset_make_cde
// join asset_brand_code@To_Cms ab on ab.asset_brand_cde = cme.asset_brand_cde
// join ASSET_MODEL_CODE@To_Cms AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde
//where 1=1
//and cme.application_number='{0}' and cme.identification_code='{1}'
//";
//                #endregion
//                DataTable dt_cms = DBHelper.ExecuteDataTableSql(Cap_DB, string.Format(sql_cms, application_number, user["IDENTIFICATION_CODE"]));

//                List<CMS_EXPOSURE> objs_cms = new List<CMS_EXPOSURE>();
//                foreach (DataRow row in dt_cms.Rows)
//                {
//                    var obj_cms = new CMS_EXPOSURE
//                    {
//                        PRINCIPLE_OUTSTANDING_AMT = Convert.ToDecimal(row["PRINCIPLE_OUTSTANDING_AMT"]).ToString("N2"),//本金余额
//                        EXP_APPLICATION_NUMBER = row["EXP_APPLICATION_NUMBER"] + string.Empty,//申请号
//                        CONTRACT_NUMBER = row["CONTRACT_NUMBER"] + string.Empty,//合同号
//                        REQUEST_STATUS_DSC = row["REQUEST_STATUS_DSC"] + string.Empty,//状态
//                        EXP_INACTIVE_IND = row["EXP_INACTIVE_IND"] + string.Empty,//申请人状态

//                        EXP_APPLICANT_ROLE_IND = row["EXP_APPLICANT_ROLE_IND"] + string.Empty,//角色
//                        EXP_APPLICANT_NAME = row["EXP_APPLICANT_NAME"] + string.Empty,//申请人姓名
//                        FP_GROUP_NME = row["FP_GROUP_NME"] + string.Empty,//合同类型
//                        EXP_APPLICANT_CARD_ID = row["EXP_APPLICANT_CARD_ID"] + string.Empty,//ID/注册号
//                        BUSINESS_PARTNER_NME = row["BUSINESS_PARTNER_NME"] + string.Empty,//经销商

//                        CONTRACT_DTE = (row["CONTRACT_DTE"] + string.Empty) == "" ? "" : Convert.ToDateTime(row["CONTRACT_DTE"]).ToString("yyyy-MM-dd"),//合同日期
//                        INTEREST_RATE = row["INTEREST_RATE"] + string.Empty,//利率
//                        NET_FINANCED_AMT = Convert.ToDecimal(row["NET_FINANCED_AMT"]).ToString("N2"),//融资额
//                        ASSET_MAKE_DSC = row["ASSET_MAKE_DSC"] + string.Empty,//生产商
//                        ASSET_MODEL_DSC = row["ASSET_MODEL_DSC"] + string.Empty,//动力参数
//                        ASSET_BRAND_DSC = row["ASSET_BRAND_DSC"] + string.Empty,//车型

//                        OVERDUE_30_DAYS = row["OVERDUE_30_DAYS"] + string.Empty,//30天
//                        OVERDUE_60_DAYS = row["OVERDUE_60_DAYS"] + string.Empty,//60天
//                        OVERDUE_90_DAYS = row["OVERDUE_90_DAYS"] + string.Empty,//90天
//                        OVERDUE_ABOVE_90_DAYS = row["OVERDUE_ABOVE_90_DAYS"] + string.Empty,//90天以上
//                        OVERDUE_ABOVE_120_DAYS = row["OVERDUE_ABOVE_120_DAYS"] + string.Empty,//120天以上
//                        NO_OF_TERMS = row["NO_OF_TERMS"] + string.Empty,//期数
//                        NO_OF_TERMS_PAID = row["NO_OF_TERMS_PAID"] + string.Empty,//已付期数

//                        APPLICANT_ROLE_IND = row["APPLICANT_ROLE_IND"] + string.Empty,
//                        SYS_IDENTIFIED_IND = row["SYS_IDENTIFIED_IND"] + string.Empty,
//                        IS_EXPOSED_IND = row["IS_EXPOSED_IND"] + string.Empty,
//                        IS_CONTINGENT_IND = row["IS_CONTINGENT_IND"] + string.Empty
//                    };
//                    objs_cms.Add(obj_cms);
//                }
//                #endregion

//                decimal amt_cap = objs.Where(cap => cap.IS_CONTINGENT_IND == "T").Sum(cap => Convert.ToDecimal(cap.NET_FINANCED_AMT));
//                decimal amt_cms = objs_cms.Where(cms => cms.IS_CONTINGENT_IND == "T").Sum(cms => Convert.ToDecimal(cms.PRINCIPLE_OUTSTANDING_AMT));
//                total_FZ += (amt_cap + amt_cms);
//                result.Add(new { Type = type, Name = user["NAME"], No = user["IDENTIFICATION_CODE"], CAP = objs, CMS = objs_cms });
//            }
//            return Json(new { Data = result, Total_FZ = total_FZ.ToString("N2") }, JsonRequestBehavior.AllowGet);
//        }

//        /// <summary>
//        /// 根据错误编码获取错误信息
//        /// </summary>
//        /// <param name="error_code"></param>
//        /// <returns></returns>
//        public JsonResult Get_WriteToCAP_Msg(string error_code)
//        {
//            string errmsg = "";
//            switch (error_code)
//            {
//                case "-10000": errmsg = "-10000:调用存储过程异常，请联系管理员！"; break;
//                //存储过程中定义的错误编码
//                case "-20010": errmsg = "-20010:查无此金融产品或产品已过期，请联系管理员！"; break;
//                case "-20011": errmsg = "-20011:查无此车辆信息，请联系管理员！"; break;
//                case "-20012": errmsg = "-20012:此申请号已存在，请联系管理员！"; break;
//                case "-20013": errmsg = "-20013:根据objectid在H3没查到数据，请联系管理员！"; break;
//                case "-20014": errmsg = "-20014:申请号不在CMS，请联系管理员！"; break;
//                case "-20015": errmsg = "-20015:根据objectid在H3没查到user_name，请联系管理员！"; break;
//                case "-20016": errmsg = "-20016:根据objectid在H3没查到sale_price，请联系管理员！"; break;
//                case "-20017": errmsg = "-20017:未查出相关金融参数，请联系管理员！"; break;
//                case "-20018": errmsg = "-20018:尾款超范围，请联系管理员！"; break;
//                case "-20019": errmsg = "-20019:未找到临时还款计划，请联系管理员！"; break;
//                case "-20020": errmsg = "-20020:Load数据异常，请联系管理员！"; break;
//                case "-20021": errmsg = "-20021:未找到临时还款计划，请联系管理员！"; break;
//                case "-20022": errmsg = "-20022:未查到月供数据，请联系管理员！"; break;
//                case "-20023": errmsg = "-20023:计算金融数据异常，请联系管理员！"; break;
//                default: errmsg = error_code + ":未定义的编码，请联系管理员！"; break;
//            }
//            return Json(new { Code = error_code, Msg = errmsg }, JsonRequestBehavior.AllowGet);
//        }

//        /// <summary>
//        /// 根据申请号获取H3中的流程详情(URL)
//        /// 1.个人及历史个人
//        /// 2.机构及历史机构
//        /// 3.零售二期
//        /// </summary>
//        /// <param name="application_number"></param>
//        /// <returns></returns>
//        public JsonResult Get_H3_Detail(string application_number)
//        {
//            string sql = @"
//select con.objectid from Ot_Instancecontext con join (
//select objectid from I_RETAILAPP where applicationno='{0}'
//union 
//select objectid from I_HRETAILAPP where applicationno='{0}'
//union
//select objectid from i_Companyapp where applicationno='{0}'
//union 
//select objectid from i_HCompanyapp where applicationno='{0}'
//union
//select objectid from I_APPLICATION where application_number='{0}'
//) d on con.bizobjectid=d.objectid
//order by createdtime desc
//";
//            sql = string.Format(sql, application_number);
//            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
//            if (dt.Rows.Count == 0)
//            {
//                return Json(new { URL = "", JsonRequestBehavior.AllowGet });
//            }
//            else
//            {
//                return Json(new { URL = "/Portal/InstanceSheets.html?InstanceId=" + dt.Rows[0][0], JsonRequestBehavior.AllowGet });
//            }
//        }

//        public JsonResult CheckIsDayEnd()
//        {
//            bool success = true;
//            var message = "";
//            #region 已设置好系统日结开始、结束时间
//            if (!string.IsNullOrEmpty(dayendStartTime) && !string.IsNullOrEmpty(dayendEndTime))
//            {
//                try
//                {
//                    //转化成日期格式，非日期格式转化则会异常
//                    var d_start = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + dayendStartTime);
//                    var d_end = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd") + " " + dayendEndTime);
//                    #region 判断登录时间是否在日结时间范围内，范围内不允许登录；
//                    if (d_end < d_start)
//                    {
//                        if (DateTime.Now > d_start || DateTime.Now < d_end)
//                        {
//                            success = false;
//                            message = string.Format(dayendMessage, "（从" + dayendStartTime + "到次日" + dayendEndTime + "）");
//                        }
//                    }
//                    else
//                    {
//                        if (DateTime.Now > d_start && DateTime.Now < d_end)
//                        {
//                            success = false;
//                            message = string.Format(dayendMessage, "（从" + dayendStartTime + "到" + dayendEndTime + "）");
//                        }
//                    }
//                    #endregion
//                }
//                catch (Exception ex)
//                {
//                    success = false;
//                    message = "系统日志时间段设置错误，开始时间-->" + dayendStartTime + "，结束时间-->" + dayendEndTime;
//                    AppUtility.Engine.LogWriter.Write(message);
//                }
//            }
//            #endregion
//            return Json(new
//            {
//                Success = success,
//                Message = message
//            }, JsonRequestBehavior.AllowGet);
//        }

//        //是否是高风险
//        public JsonResult IsHighRisk(string FI_Code)
//        {
//            string sql = "select  a.BJNR  from I_gfxjxs a join OT_User b  on a.jxs = b.APPELLATION where b.code = '{0}'";
//            //string sql = "select count(1) from C_HighRiskFI where usercode='{0}'";
//            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format(sql, FI_Code));
//            string highrisk = "";
//            if (dt != null && dt.Rows.Count > 0)
//            {
//                highrisk = dt.Rows[0]["BJNR"].ToString();
//            }
//            return Json(new { HighRisk = highrisk }, JsonRequestBehavior.AllowGet);
//        }
//        //获取同盾报告的数据
//        public JsonResult GetTDData(string idcardno)
//        {
//            string url = queryTdReportUrl + "?id_card_no=" + idcardno;
//            //string para = Newtonsoft.Json.JsonConvert.SerializeObject(new { id_card_no = idcardno });
//            string result = HttpHelper.PostWebRequest(url, "");

//            return Json(result, JsonRequestBehavior.AllowGet);
//        }

//        /// <summary>
//        /// 单位敞口
//        /// </summary>
//        /// <param name="companyName">公司名称</param>
//        /// <returns></returns>

//        public JsonResult GetCompanyOrderList(string companyName, string application_number)
//        {
//            bool success = false;
//            List<object> objs = new List<object>();
//            string message = string.Empty;
//            try
//            {
//                string sql = "select * from unit_application_list where 公司名称='" + companyName + "' order by 申请日期 desc";
//                DataTable dt = DBHelper.ExecuteDataTableSql(Cap_DB, sql);
//                foreach (DataRow row in dt.Rows)
//                {
//                    if (row["申请号"].ToString().Trim() != application_number)
//                    {
//                        objs.Add(new
//                        {
//                            OBJECTID = row["OBJECTID"].ToString().Trim(),
//                            APPLICATION_NUMBER = row["申请号"].ToString().Trim(),
//                            BUSINESS_PARTNER_NME = row["公司名称"].ToString().Trim(),
//                            APPLICATION_DATE = row["申请日期"].ToString().Trim(),
//                            Trial_STATUS = row["初审审核状态"].ToString().Trim(),
//                            FINALLY_STATUS = row["终审审核状态"].ToString().Trim(),
//                            STATUS = row["申请状态"].ToString().Trim(),
//                            WORKFLOWCODE = row["WORKFLOWCODE"].ToString().Trim(),
//                        });
//                    }
//                }
//                success = true;
//            }
//            catch (Exception ex)
//            {
//                message = "公司名称为：" + companyName + "查询出错了，错误原因：" + ex.ToString();
//            }
//            return Json(new { Success = success, Data = objs, Message = message }, JsonRequestBehavior.AllowGet);
//        }

//        public JsonResult GetAllMappingCities()
//        {
//            string sql = "select * from C_CityMapping";
//            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
//            Dictionary<string, string> dic = new Dictionary<string, string>();
//            try
//            {
//                foreach (DataRow row in dt.Rows)
//                {
//                    if (row["CmsCityName"] + string.Empty == "")
//                        continue;
//                    if (dic.ContainsKey(row["CmsCityName"] + string.Empty))
//                        continue;
//                    dic.Add(row["CmsCityName"] + string.Empty, row["H3CityName"] + string.Empty);
//                }
//            }
//            catch (Exception ex)
//            {
//                return Json(new { Success = false, Data = "", Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
//            }
//            return Json(new { Success = true, Data = dic, Msg = "OK" }, JsonRequestBehavior.AllowGet);
//        }
//        /// <summary>
//        /// 获取经销商开户行信息
//        /// </summary>
//        /// <param name="FI_Code"></param>
//        /// <returns></returns>
//        public JsonResult getjxskhxx(string FI_Code)
//        {
//            string sql = @"select distinct A.User_Name,bb.business_partner_id,bc1.company_nme jxs
//                           ,bb.bp_primary_id,bc2.company_nme zh
//                           ,bb.bp_secondary_id,bc3.company_nme jxskhh
//                           ,bb.account_nme zhm
//                           ,bb.account_nbr jxskhhkh
//                           ,bb.default_ind
//                           --,bb.*
//                           from BP_BANK BB 
//                           JOIN BP_COMPANY BC1 ON BC1.BUSINESS_PARTNER_ID=BB.Business_Partner_Id
//                           JOIN BP_COMPANY BC2 ON BC2.BUSINESS_PARTNER_ID=BB.Bp_Primary_Id
//                           JOIN BP_COMPANY BC3 ON BC3.BUSINESS_PARTNER_ID=BB.Bp_Secondary_Id
//                           JOIN Application A ON A.Business_Partner_Id = BB.Business_Partner_Id
//                           where bb.default_ind='T' --默认选选中的银行
//                           and A.User_Name = '{0}'
//                           --and bb.business_partner_id=1288843、
//                           order by 3 asc";
//            sql = string.Format(sql, FI_Code);
//            DataTable dt = DBHelper.ExecuteDataTableSql("cap", sql);
//            string jxskhh = "", jxskhhkh = "";
//            if (dt != null && dt.Rows.Count > 0)
//            {
//                jxskhh = dt.Rows[0]["jxskhh"].ToString();
//                jxskhhkh = dt.Rows[0]["jxskhhkh"].ToString();
//            }
//            return Json(new
//            {
//                jxskhh = jxskhh,
//                jxskhhkh = jxskhhkh
//            }, JsonRequestBehavior.AllowGet);
//        }

    }
}