using Newtonsoft.Json;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.MySetting;
using DongZheng.H3.WebApi.DTO;
using DongZheng.H3.WebApi.Models.ApiResponse;
using DongZheng.H3.WebApi.Models.Enum;
using DongZheng.H3.WebApi.Models.DAL;
using System.Linq;
using DongZheng.H3.WebApi.Models;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Data.OracleClient;
using System.Configuration;
using DongZheng.H3.WebApi.Controllers.ProcessCenter;
using DongZheng.H3.WebApi.Models.FaceSign;
using DongZheng.H3.WebApi.Common.PreApproval;
using OThinker.H3.WorkItem;
using System.Text;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    public class ApiController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        #region String
        /// <summary>
        /// Cap业务数据库的连接的编码
        /// </summary>
        public string Cap_DB = System.Configuration.ConfigurationManager.AppSettings["CAP_Retail"] + string.Empty;
        /// <summary>
        /// 预审批系统接口的URL
        /// </summary>
        public string ysp_url = System.Configuration.ConfigurationManager.AppSettings["YSP_URL"] + string.Empty;
        /// <summary>
        /// 是否启用预审批系统接口
        /// </summary>
        public bool ysp_enable = Convert.ToBoolean(System.Configuration.ConfigurationManager.AppSettings["YSP_ENABLE"] + string.Empty == "" ? "false" : System.Configuration.ConfigurationManager.AppSettings["YSP_ENABLE"] + string.Empty);
        /// <summary>
        /// 预审批与进件流程的控制关系：strong  weak   none
        /// </summary>
        public string ysp_controltype = System.Configuration.ConfigurationManager.AppSettings["YSP_CONTROLTYPE"] + string.Empty;

        /// <summary>
        /// 风控分单开关
        /// </summary>
        public const string FKFenDanStatus = "FKFenDanStatus";

        /// <summary>
        /// 面签推送数据的URL
        /// </summary>
        public string faceSignRequestUrl = System.Configuration.ConfigurationManager.AppSettings["FaceSignRequestUrl"] + string.Empty;
        #endregion

        // GET: Api
        public string Index()
        {
            return "api 接口，不校验登录；";
        }

        //获取资产信息
        [Xss]
        public JsonResult Get_VEHICLE(string asset_make_cde, string asset_brand_cde, string asset_model_cde, string asset_model_name)
        {
            AppUtility.Engine.LogWriter.Write("Get_VEHICLE:asset_make_cde-->" + asset_make_cde + ",asset_brand_cde-->" + asset_brand_cde + ",asset_model_cde-->" + asset_model_cde + ",asset_model_name-->" + asset_model_name);
            string conditions = "";
            string sql = @"
select distinct amc.asset_make_dsc,amc.asset_make_cde,abc.asset_brand_dsc,abc.asset_brand_cde,mod.asset_model_dsc,mod.asset_model_cde
,mcd.miocn_id,mcd.miocn_nbr,mcd.miocn_dsc,vi.list_price_amt retail_price_amt,vtype.vehicle_type_cde,vtype.vehicle_type_dsc,vsubtype.vehicle_subtyp_cde,vsubtype.vehicle_subtyp_dsc
,info.transmission_type_cde from asset_model_code@to_cms mod
join asset_make_code@to_cms amc on mod.asset_make_cde=amc.asset_make_cde
join asset_brand_code@to_cms abc on mod.asset_brand_cde=abc.asset_brand_cde and mod.asset_make_cde=abc.asset_make_cde
join miocn_config_detail@to_cms mcd on mod.asset_model_cde=mcd.asset_model_cde  and mod.asset_make_cde=mcd.asset_make_cde
join ASSET_MODEL_GROUP@to_cms vi on vi.asset_make_cde = amc.asset_make_cde and vi.asset_model_cde=mod.asset_model_cde and vi.asset_brand_cde=abc.asset_brand_cde
left join vehicle_type_code@to_cms vtype on vi.vehicle_type_cde=vtype.vehicle_type_cde
left join vehicle_subtype_code@to_cms vsubtype on vi.vehicle_subtyp_cde=vsubtype.vehicle_subtyp_cde
left join vehicle_information@to_cms info on info.asset_make_cde = amc.asset_make_cde and info.asset_model_cde=mod.asset_model_cde and info.asset_brand_cde=abc.asset_brand_cde
where 1 = 1
and mod.activate_ind = 'T'--T生效车型
and amc.activate_ind = 'T'--T生效品牌
and abc.activate_ind = 'T'--T生效车系
{0}
order by amc.asset_make_dsc asc,abc.asset_brand_dsc asc,mod.asset_model_dsc desc";
            if (string.IsNullOrEmpty(asset_make_cde) && string.IsNullOrEmpty(asset_brand_cde)
                && string.IsNullOrEmpty(asset_model_cde))
                return Json(new { data = new List<VEHICLE>() }, JsonRequestBehavior.AllowGet);
            if (!string.IsNullOrEmpty(asset_make_cde))
                conditions += "and amc.asset_make_cde ='" + asset_make_cde + "'\r\n";
            if (!string.IsNullOrEmpty(asset_brand_cde))
                conditions += "and abc.asset_brand_cde ='" + asset_brand_cde + "'\r\n";
            if (!string.IsNullOrEmpty(asset_model_cde))
                conditions += "and mod.asset_model_cde ='" + asset_model_cde + "'\r\n";
            if (!string.IsNullOrEmpty(asset_model_name))
                conditions += "and mod.asset_model_dsc like '%" + asset_model_name + "%'\r\n";
            sql = string.Format(sql, conditions);
            List<VEHICLE> result = new List<VEHICLE>();
            DataTable dt = DBHelper.ExecuteDataTableSql(Cap_DB, sql);
            foreach (DataRow row in dt.Rows)
            {
                VEHICLE v = new VEHICLE();
                v.asset_make_cde = row["asset_make_cde"] + string.Empty;
                v.asset_make_dsc = row["asset_make_dsc"] + string.Empty;
                v.asset_brand_cde = row["asset_brand_cde"] + string.Empty;
                v.asset_brand_dsc = row["asset_brand_dsc"] + string.Empty;
                v.asset_model_cde = row["asset_model_cde"] + string.Empty;
                v.asset_model_dsc = row["asset_model_dsc"] + string.Empty;
                v.miocn_id = row["miocn_id"] + string.Empty;
                v.miocn_nbr = row["miocn_nbr"] + string.Empty;
                v.miocn_dsc = row["miocn_dsc"] + string.Empty;
                v.retail_price_amt = row["retail_price_amt"] + string.Empty;
                v.vehicle_type_cde = row["vehicle_type_cde"] + string.Empty;
                v.vehicle_type_dsc = row["vehicle_type_dsc"] + string.Empty;
                v.vehicle_subtyp_cde = row["vehicle_subtyp_cde"] + string.Empty;
                v.vehicle_subtyp_dsc = row["vehicle_subtyp_dsc"] + string.Empty;
                //除了等于00001的，其它都认为是00002自动
                v.transmission_type_cde = row["transmission_type_cde"] + string.Empty == "00001" ? "N" : "Y";
                result.Add(v);
            }
            return Json(new { data = result }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 逻辑删除合同利率
        /// </summary>
        [Xss]
        public JsonResult RemoveContractRate(string removeRateId, string Originator)
        {
            if (string.IsNullOrEmpty(removeRateId))
            {
                return Json(new
                {
                    removeResult = "objectId不能为空"
                }, JsonRequestBehavior.AllowGet);
            }

            AppUtility.Engine.LogWriter.Write($"RemoveContractRate:objectId--> {removeRateId};RemoveBy-->{Originator}");

            string sql = $"UPDATE i_htll2 SET ISDELETE = 1, MODIFIEDBY = '{Originator}', MODIFIEDTIME = TO_DATE('{DateTime.Now.ToString()}', 'yyyy-MM-dd hh24:mi:ss') WHERE OBJECTID = '{removeRateId}'";
            bool flag = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql) > 0;

            return Json(new
            {
                removeResult = flag ? "删除成功" : "删除失败"
            }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取合同利率列表
        /// </summary>
        [Xss]
        public JsonResult GetContractRate(string duration)
        {
            AppUtility.Engine.LogWriter.Write("GetContractRate:start-->");

            IList<ContractRate> result = new List<ContractRate>();
            string rateBase;
            StringBuilder sql = new StringBuilder($@"SELECT OBJECTID, DURATION, RATEBASE, ENABLEDTIME FROM i_htll2 WHERE ISDELETE = 0 ");

            if (!string.IsNullOrEmpty(duration))
            {
                sql.Append($" AND DURATION = '{duration}' ");
            }

            sql.Append(" ORDER BY ENABLEDTIME DESC ");

            DataTable rateTable = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataSet(sql.ToString()).Tables[0];

            foreach (DataRow row in rateTable.Rows)
            {
                rateBase = row.IsNull("RATEBASE") ? "0" : row["RATEBASE"] + string.Empty;
                result.Add(new ContractRate
                {
                    RateId = row["OBJECTID"].ToString(),
                    Duration = row["DURATION"] + string.Empty,
                    RateBase = Convert.ToDecimal(rateBase),
                    EnabledTime = row["ENABLEDTIME"] + string.Empty,
                });
            }

            return Json(new
            {
                data = result
            }, JsonRequestBehavior.AllowGet);
        }

        //资产Make下拉框数据源
        [Xss]
        public JsonResult Get_Asset_Make(string bp_id, string asset_condition)
        {
            try
            {
                string sql = "";
                if (asset_condition == "N")//新车
                {
                    //有维护经销商与品牌的关系，则关联这张表
                    string sql_getnum = @"select count(1) from zpartner_make_init where business_partner_id=" + bp_id;
                    if (DBHelper.ExecuteDataTableSql(Cap_DB, sql_getnum).Rows[0][0].ToString() != "0")
                    {
                        sql = @"
select amc.asset_make_cde,amc.asset_make_dsc from asset_make_code@to_cms  amc
join zpartner_make_init zm ON  zm.asset_make_cde = amc.asset_make_cde and zm.business_partner_id={0}
where amc.Activate_Ind='T' and amc.asset_make_dsc is not null 
order by amc.asset_make_dsc ";
                        sql = string.Format(sql, bp_id);
                    }
                }
                //其它情况显示所有的品牌
                if (sql == "")
                {
                    sql = @"
select asset_make_cde,asset_make_dsc from asset_make_code@to_cms where Activate_Ind='T' and asset_make_dsc is not null
order by asset_make_dsc";
                }
                DataTable dt = DBHelper.ExecuteDataTableSql(Cap_DB, sql);
                List<object> ret = new List<object>();
                foreach (DataRow row in dt.Rows)
                {
                    ret.Add(new
                    {
                        asset_make_cde = row["asset_make_cde"],
                        asset_make_dsc = row["asset_make_dsc"]
                    });
                }
                return Json(new { data = ret }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("Get_Asset_Make Exception-->" + ex.ToString());
                return Json(new { data = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        //预审批系统查询方法
        [HttpPost]
        [Xss]
        public JsonResult YSP_Query(string instanceid, string name, string idCardNo, string phone, string bankcardnumber, string bankcardnumber2)
        {
            AppUtility.Engine.LogWriter.Write("调用预审批接口(" + (ysp_enable ? "已" : "未") + "启用预审批系统)：instanceid-->"
                + instanceid + "，name-->" + name + "，idCardNo-->" + idCardNo + "，phone-->" + phone);
            try
            {
                if (ysp_enable)
                {
                    NameValueCollection body = new NameValueCollection();
                    body.Add("name", name);
                    body.Add("phone", phone);
                    body.Add("idCardNo", idCardNo);
                    body.Add("bankCard", bankcardnumber);
                    body.Add("bankCard2", bankcardnumber2);
                    body.Add("instanceid", instanceid);
                    body.Add("requestSource", "h3");

                    var instance = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);
                    User us = (User)AppUtility.Engine.Organization.GetUnit(instance.Originator);

                    body.Add("dealerNumber", us.Code);
                    body.Add("dealerName", us.Appellation);
                    body.Add("customerSource", getFIType(us.Code));
                    AppUtility.Engine.LogWriter.Write(HttpHelper.postFormData(ysp_url, new NameValueCollection(), body));
                }
                else
                {
                    var instance = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);
                    var keyvalues = new Dictionary<string, object>();
                    keyvalues.Add("SYS_PASS", false);
                    keyvalues.Add("SYS_ENABLE", false);
                    keyvalues.Add("SYS_MESSAGE", "未启用预审批系统");
                    keyvalues.Add("FinishedTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                    AppUtility.Engine.BizObjectManager.SetPropertyValues(instance.BizObjectSchemaCode, instance.BizObjectId, "", keyvalues);
                    //Sleep 1s
                    System.Threading.Thread.Sleep(1000);
                    #region 提交任务，到下一个节点；
                    SubmitItem(instanceid, OThinker.Data.BoolMatchValue.True, "", "");
                    #endregion
                    AppUtility.Engine.LogWriter.Write("未启用预审批系统，自动提交到下一个节点");
                }
            }
            catch (Exception e)
            {
                AppUtility.Engine.LogWriter.Write("请求预审批系统异常-->" + e.ToString());
            }
            return Json(new { Success = true });
        }

        //预审批系统回调方法
        [HttpPost]
        public JsonResult YSP_Result(string instanceid, string result, string message)
        {
            rtn_data rtn = new rtn_data();
            AppUtility.Engine.LogWriter.Write("预审批回调方法：InstanceID-->" + instanceid + "，Result-->" + result + "，message-->" + message);
            try
            {
                var instance = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);
                if (instance == null)
                {
                    rtn.code = -10000;
                    rtn.message = "InstanceID错误，或者此ID在H3中不存在；";
                    AppUtility.Engine.LogWriter.Write(rtn.message);
                    return Json(rtn);
                }

                //01通过 02人工 03拒绝 05信息错误  06风险用户  07驳回
                if (result != "01" && result != "02" && result != "03" && result != "05" && result != "06" && result != "07")
                {
                    rtn.code = -10001;
                    rtn.message = "状态错误，不存在状态-->" + result;
                    AppUtility.Engine.LogWriter.Write(rtn.message);
                    return Json(rtn);
                }

                //单独流程预审批回调
                if (instance.WorkflowCode == "YSP")
                {
                    YspResultCommon(rtn, instanceid, result, message);
                }

                //正源预审批回调
                if (instance.WorkflowCode == "APPLICATION_ZY")
                {
                    YspResultZy(rtn, instanceid, result, message);
                }

                AppUtility.Engine.LogWriter.Write(instance.WorkflowCode + "预审批" + Newtonsoft.Json.JsonConvert.SerializeObject(rtn));
                return Json(rtn);
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("预审批回调方法处理异常：" + ex.ToString());
                rtn.code = -1;
                rtn.message = "异常-->" + ex.Message;
                return Json(rtn);
            }
        }

        /// <summary>
        /// 单独流程预审批回调
        /// </summary>
        private void YspResultCommon(rtn_data rtn, string instanceid, string result, string message)
        {
            bool pass = result != "03" && result != "05" && result != "07";//是否通过，除了03拒绝之外，其它都算通过（06走人工）

            var keyvalues = new Dictionary<string, object>();
            keyvalues.Add("SYS_PASS", pass);
            keyvalues.Add("SYS_ENABLE", true);//启用了预审批
            keyvalues.Add("SYS_MESSAGE", message);
            keyvalues.Add("FinishedTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            keyvalues.Add("SYS_CODE", result);

            var instance = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);

            if (AppUtility.Engine.BizObjectManager.SetPropertyValues(instance.BizObjectSchemaCode, instance.BizObjectId, "", keyvalues))
            {
                rtn.code = 1;
                rtn.message = "success";


                InstanceData data = new InstanceData(AppUtility.Engine, instance.InstanceId, "");
                #region 推送接口中心
                string uuid = data["uuid"].Value + string.Empty;
                if (!string.IsNullOrEmpty(uuid))
                {
                    string ispass = "1";
                    if (result == "03")
                        ispass = "0";
                    else if (result == "07")
                        ispass = "2";
                    string paraCallback = JsonConvert.SerializeObject(new
                    {
                        Uuid = uuid,
                        CallbackType = "0",
                        CallbackParameters = JsonConvert.SerializeObject(new
                        {
                            uuid = uuid,
                            ispass = ispass
                        })
                    });
                    AppUtility.Engine.LogWriter.Write("推送消息给接口中心结果：" + HttpHelper.PostWebRequest(ConfigurationManager.AppSettings["apiAddMQMsg"] + string.Empty, "application/json", paraCallback));
                }
                #endregion
                //07驳回，其他流转到下一步
                if (result == "07")
                {
                    //驳回流程，返回到上一步
                    WorkFlowFunction.ReturnWorkItemByInstanceid(instanceid, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID, "");
                }
                else
                {
                    #region 提交任务，到下一个节点；
                    SubmitItem(instanceid, OThinker.Data.BoolMatchValue.True, "", "");
                    #endregion
                }

                //如果通过，且勾选自动发起进件选项，自动发起进件流程
                if (pass)
                {
                    if ((data["AUTO_INITIATE"].Value + string.Empty).ToLower() == "true")
                    {
                        User us = (User)AppUtility.Engine.Organization.GetUnit(instance.Originator);
                        DongZheng.H3.WebApi.Proposal.GenerateProposalTask(us.Code, data["SYS_XM"].Value + string.Empty, data["SYS_IDCARD"].Value + string.Empty, data["SYS_PHONE"].Value + string.Empty, string.Empty);
                    }
                }



            }
            else
            {
                rtn.code = -10002;
                rtn.message = "H3中数据项赋值失败";
            }
        }


        /// <summary>
        /// 正源预审批回调
        /// </summary>
        /// <param name="rtn"></param>
        /// <param name="instanceid"></param>
        /// <param name="result"></param>
        /// <param name="message"></param>
        private void YspResultZy(rtn_data rtn, string instanceid, string result, string message)
        {
            var keyvalues = new Dictionary<string, object>();
            keyvalues.Add("zy_SYS_MESSAGE", message);
            keyvalues.Add("zy_SYS_CODE", result);
            keyvalues.Add("zy_transactionNo", "");

            var instance = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);

            if (AppUtility.Engine.BizObjectManager.SetPropertyValues(instance.BizObjectSchemaCode, instance.BizObjectId, "", keyvalues))
            {
                rtn.code = 1;
                rtn.message = "success";

                //07驳回，其他流转到下一步
                if (result == "07" || result == "03")
                {
                    //驳回流程，返回到上一步
                    WorkFlowFunction.ReturnWorkItemByInstanceid(instanceid, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID, "");
                }
                else
                {
                    #region 提交任务，到下一个节点；
                    SubmitItem(instanceid, OThinker.Data.BoolMatchValue.True, "", "");
                    #endregion
                }
            }
            else
            {
                rtn.code = -10002;
                rtn.message = "正源H3中数据项赋值失败";
            }

        }

        /// <summary>
        /// 检查是否通过预审批
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idCardNo"></param>
        /// <param name="phone"></param>
        /// <returns></returns>
        [Xss]
        public JsonResult YSP_Check(string name, string idCardNo, string phone)
        {
            //控制类型：强、弱、无
            string controlType = ysp_controltype;
            //是否查询过
            bool hasQueried = false;
            //是否已拒绝
            bool isRefused = false;

            bool isfinish = false;

            if (ysp_enable)//启用预审批接口
            {
                //string sql = string.Format("select ysp.SYS_PASS,con.state from I_YSP ysp left join OT_InstanceContext con on ysp.objectid=con.bizobjectid where ysp.SYS_XM='{0}' and ysp.SYS_IDCARD='{1}' and ysp.SYS_ENABLE=1 order by ysp.FinishedTime desc", name, idCardNo);
                //DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                //if (dt.Rows.Count == 0)
                //    hasQueried = false;
                //else
                //{
                //    hasQueried = true;
                //    if (dt.Rows[0]["state"] + string.Empty == "4")
                //    {
                //        isfinish = true;
                //        if (dt.Rows[0]["SYS_PASS"] + string.Empty == "0")
                //            isRefused = true;
                //    }
                //}
                PreApproval preApproval = new PreApproval();
                var result = preApproval.QueryPreApprovalResult(name, idCardNo);
                if (result != null)
                {
                    hasQueried = true;
                    //通过的
                    //01通过，02转人工，06风险用户
                    if (result.preResult == "01" || result.preResult == "02" || result.preResult == "06")
                    {
                        isfinish = true;
                        isRefused = false;
                    }
                    //拒绝的
                    else if (result.preResult == "03")
                    {
                        isfinish = true;
                        isRefused = true;
                    }
                    //未完成的
                    //05信息错误，07驳回
                    else if (result.preResult == "05" || result.preResult == "07")
                    {
                        isfinish = false;
                        isRefused = false;
                    }
                    //其它：未完成，拒绝
                    else
                    {
                        isfinish = false;
                        isRefused = true;
                    }
                }
            }

            return Json(new { YSP_Enable = ysp_enable, ControlType = controlType, Queried = hasQueried, IsFinish = isfinish, Refused = isRefused }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取自动审批的银行卡
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IdCardNo"></param>
        /// <returns></returns>
        [Xss]
        public JsonResult YSP_BankCards(string Name, string IdCardNo)
        {
            IdCardNo = IdCardNo.ToUpper();

            var bankCardDic = YSP_BankCard(Name, IdCardNo);

            return Json(bankCardDic, JsonRequestBehavior.AllowGet);
        }

        [Xss]
        public bool SubmitItem(string instanceid, OThinker.Data.BoolMatchValue approval, string commentText, string userId)
        {
            string sql_getid = string.Format("select objectid from Ot_Workitem where instanceid='{0}'", instanceid);
            string workItemId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_getid) + string.Empty;
            if (string.IsNullOrEmpty(workItemId))
            {
                AppUtility.Engine.LogWriter.Write("InstanceID-->" + instanceid + "不存在代办任务,提交任务失败");
                return false;
            }


            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext instance = AppUtility.Engine.InstanceManager.GetInstanceContext(item.InstanceId);

            // 结束工作项
            AppUtility.Engine.WorkItemManager.FinishWorkItem(
                item.ObjectID,
                userId,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                null,
                approval,
                commentText,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                (int)OThinker.H3.Controllers.SheetButtonType.Submit);
            // 需要通知实例事件管理器结束事件
            AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    approval,
                    false,
                    approval,
                    true,
                    null);
            AppUtility.Engine.InstanceManager.SendMessage(endMessage);
            return true;
        }

        [Xss]
        public JsonResult RepairYSPInstance(string min)
        {
            if (string.IsNullOrEmpty(min))
                min = "10";
            string sql = @"select con.objectid Instanceid,ysp.sys_xm,ysp.sys_idcard,ysp.sys_phone,round((sysdate -wi.receivetime)*24*60,2) min_diff from OT_INSTANCECONTEXT con 
left join Ot_Workitem wi on con.objectid=wi.instanceid
left join I_YSP ysp on con.bizobjectid=ysp.objectid
where con.workflowcode='YSP' and con.state=2 
and wi.activitycode='Activity3'  and (sysdate -wi.receivetime)*24*60>" + min;
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            AppUtility.Engine.LogWriter.Write("修复预审批的数据源：\r\n" + Newtonsoft.Json.JsonConvert.SerializeObject(dt));
            foreach (DataRow row in dt.Rows)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("instanceid", row["Instanceid"] + string.Empty);
                dic.Add("name", row["sys_xm"] + string.Empty);
                dic.Add("idCardNo", row["sys_idcard"] + string.Empty);
                dic.Add("phone", row["sys_phone"] + string.Empty);
                BizService.ExecuteBizNonQuery("YSP_Query", "YSP_Query", dic);
            }
            return Json("ok", JsonRequestBehavior.AllowGet);
        }

        //接口URL
        /// <summary>
        /// 查询NCIIC的接口地址
        /// </summary>
        public static string url_NCIIC = System.Configuration.ConfigurationManager.AppSettings["queryNCIIC"] + string.Empty;
        /// <summary>
        /// 风控评估接口地址
        /// </summary>
        public static string url_RiskEvaluation = System.Configuration.ConfigurationManager.AppSettings["riskEvaluation"] + string.Empty;
        /// <summary>
        /// 主动查询风控评估结果的接口地址
        /// </summary>
        public static string url_RiskEvaluationResult = System.Configuration.ConfigurationManager.AppSettings["queryRiskEvaluationResult"] + string.Empty;
        /// <summary>
        /// 查询PBOC的接口地址
        /// </summary>
        public static string url_PBOC = System.Configuration.ConfigurationManager.AppSettings["queryPBOCReport"] + string.Empty;
        /// <summary>
        /// 风控系统回调的接口地址
        /// </summary>
        public static string url_RiskResult = System.Configuration.ConfigurationManager.AppSettings["riskResult"] + string.Empty;

        /// <summary>
        /// 银行卡报告接口地址
        /// </summary>
        public static string url_BankCardResult = System.Configuration.ConfigurationManager.AppSettings["queryBankCardReport"] + string.Empty;


        /// <summary>
        /// 增加需要调用东正风控的用户接口
        /// </summary>
        /// <param name="LoginName"></param>
        /// <returns></returns>
        [HttpGet]
        [Xss]
        public JsonResult SaveFkFenDanUser(string LoginName)
        {
            LoginName = LoginName.Trim();

            var fenDanUsers = GetAllFkFenDanUser();

            var userCodes = fenDanUsers.Where(p => p.UserCode == LoginName).ToList();

            if (userCodes != null && userCodes.Any())
            {
                return Json(new { Code = -1, Message = "已存在相应数据" }, JsonRequestBehavior.AllowGet);
            }

            var result = InsertFkFenDanUser(LoginName);

            return Json(new { Code = result > 0 ? 0 : -1 }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除需要调用东正风控的用户接口
        /// </summary>
        /// <param name="LoginName"></param>
        /// <returns></returns>
        [HttpGet]
        [Xss]
        public JsonResult DelFkFenDanUser(string LoginName)
        {
            var result = DeleteFkFenDanUser(LoginName);

            return Json(new { Code = result > 0 ? 0 : -1 }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 自动分单接口
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        [Xss]
        public JsonResult FKFenDan(string InstanceId, string LoginName, string SchemaCode)
        {
            AppUtility.Engine.LogWriter.Write(string.Format("自动分单接口:InstanceId={0}，SchemaCode={1}，LoginName={2}", InstanceId, SchemaCode, LoginName));

            var fkFenDanStatus = AppUtility.Engine.SettingManager.GetCustomSetting(FKFenDanStatus);

            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);

            var recordNumber = string.Empty;

            if (instanceContext == null)
            {
                AppUtility.Engine.LogWriter.Write(string.Format("InstanceId={0},上下文不存在", InstanceId));

                return Json(new { reqID = InstanceId, recordId = "" }, JsonRequestBehavior.AllowGet);
            }

            var fenDanProcessDic = GetFenDanProcess(instanceContext.BizObjectId);

            //只有fkresult!=null  fkSysType=null  的情况走融数,其他走新自动审批
            if (!string.IsNullOrEmpty(fenDanProcessDic["fkresult"]) && string.IsNullOrEmpty(fenDanProcessDic["fkSysType"]))
            {
                AppUtility.Engine.LogWriter.Write(string.Format("自动分单接口:融数fkresult!=null  fkSysType=null，InstanceId={0}", InstanceId));

                //需要走数融风控接口
                Dictionary<string, object> srdic = new Dictionary<string, object>();
                srdic.Add("SchemaCode", SchemaCode);
                srdic.Add("instanceid", InstanceId);
                BizService.ExecuteBizNonQuery("DZBIZServiceNew", "postHttp", srdic);
            }
            else if (!string.IsNullOrEmpty(fenDanProcessDic["fkSysType"]))
            {
                AppUtility.Engine.LogWriter.Write(string.Format("自动分单接口:自动审批 fkSysType!=null，InstanceId={0}", InstanceId));

                //只要fkSysType!=null ，走自动审批
                recordNumber = RiskEvaluation(InstanceId);
            }
            else
            {
                //fkresult =null  fkSysType=null走自动分单
                AppUtility.Engine.LogWriter.Write(string.Format("自动分单接口:自动分单 fkresult =null  fkSysType=null，InstanceId={0}", InstanceId));

                var fkFenDanStatusEnum = (E_FkFenDanStaus)Enum.Parse(typeof(E_FkFenDanStaus), fkFenDanStatus);

                switch (fkFenDanStatusEnum)
                {
                    case E_FkFenDanStaus.AllNew:

                        //调用东正风控评估
                        recordNumber = RiskEvaluation(InstanceId);

                        break;

                    case E_FkFenDanStaus.AllOld:

                        //调用数融接口
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        dic.Add("SchemaCode", SchemaCode);
                        dic.Add("instanceid", InstanceId);
                        BizService.ExecuteBizNonQuery("DZBIZServiceNew", "postHttp", dic);

                        break;

                    case E_FkFenDanStaus.PartIn:

                        //获取所有需要调用东正风控评估的 账户
                        //后期 看需求决定是否  存入redis
                        var fenDanUsers = GetAllFkFenDanUser();

                        var userCodes = fenDanUsers.Select(p => p.UserCode).ToList();

                        //需要走东正风控
                        if (userCodes.Contains(LoginName))
                        {
                            //调用东正风控评估
                            recordNumber = RiskEvaluation(InstanceId);
                        }
                        else
                        {
                            //需要走数融风控接口
                            Dictionary<string, object> srdic = new Dictionary<string, object>();
                            srdic.Add("SchemaCode", SchemaCode);
                            srdic.Add("instanceid", InstanceId);
                            BizService.ExecuteBizNonQuery("DZBIZServiceNew", "postHttp", srdic);
                        }

                        break;
                }

                AppUtility.Engine.LogWriter.Write(string.Format("风控决定流程fkFenDanStatus={0},InstanceId={1},LoginName={2},返回结果={3}", fkFenDanStatus, InstanceId, LoginName, JsonConvert.SerializeObject(recordNumber)));
            }

            if (!string.IsNullOrEmpty(recordNumber))
            {
                AppUtility.Engine.BizObjectManager.SetPropertyValue(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, string.Empty, "FK_SYS_TYPE", "1");

                AppUtility.Engine.BizObjectManager.SetPropertyValue(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, string.Empty, "FK_RECORDID", recordNumber);

            }

            return Json(new { reqID = InstanceId, recordId = recordNumber }, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 风控系统返回结果
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RiskSysResult(Risk.Parameters_Result para)
        {
            AppUtility.Engine.LogWriter.Write("风控系统回调参数：" + Newtonsoft.Json.JsonConvert.SerializeObject(para));

            rtn_data rtn = new rtn_data { code = 1, message = "接收成功" };

            //校验参数
            if (para.IsEmpty())
            {
                rtn.code = -10001;
                rtn.message = "一个或多个参数为空";

                return Json(rtn);
            }

            //校验instanceId准确性
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(para.reqId);

            if (instanceContext == null)
            {
                rtn.code = -10001;
                rtn.message = "reqId不合法";
                return Json(rtn);
            }

            //检查流程是否处于东正风控节点下
            var workFlowId = WorkFlowFunction.getRSWorkItemIDByInstanceid(para.reqId, "风控");
            if (string.IsNullOrEmpty(workFlowId))
            {
                AppUtility.Engine.LogWriter.Write("当前流程不处于东正风控：" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                rtn.code = 1;//返回成功，或者自动审批会挂单；
                rtn.message = "当前流程不处于东正风控（不做处理）";
                return Json(rtn);
            }


            var fkresult = "转人工";

            //流程
            List<DataItemParam> keyValues = new List<DataItemParam>();

            //更新结果
            var updateResultFlag = false;

            //审核结果
            var resultValue = new Dictionary<string, object>();
            resultValue.Add("FK_SHOWRESULT", para.showResult == null ? "null" : para.showResult);

            switch (para.finalResult)
            {
                case "00":
                    //初始化 记录日志，不处理
                    AppUtility.Engine.LogWriter.Write("风控系统返回00-初始化：" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                    fkresult = "初始化";

                    resultValue.Add("FK_RESULT", "初始化");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);

                    break;

                case "01":
                    //通过
                    resultValue.Add("FK_RESULT", "通过");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);


                    //1.Automatic_approval = 1 
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 1
                    });

                    var userId = WorkFlowFunction.GetUserIDByCode("rsfk");

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, userId);


                    //2.提交当前任务
                    var result = WorkFlowFunction.SubmitWorkItemByInstanceID(para.reqId, "");
                    if (!result)
                    {
                        AppUtility.Engine.LogWriter.Write("风控系统返回01-通过，提交当前任务失败" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                    }

                    fkresult = "通过";

                    break;

                case "02":
                    //拒绝
                    resultValue.Add("FK_RESULT", "拒绝");
                    updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);


                    //1.Automatic_approval = -1 
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = -1
                    });

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, WorkFlowFunction.GetUserIDByCode("rsfk"));

                    string Application_Number = "";
                    string UserCode = "";
                    string sql = @" select a.user_name,a.application_number 
                                from I_application a join OT_instancecontext b on a.objectid = b.bizobjectid 
                                where b.objectid = '{0}'";
                    sql = string.Format(sql, para.reqId);
                    DataTable dt = CommonFunction.ExecuteDataTableSql(sql);
                    if (CommonFunction.hasData(dt))
                    {
                        Application_Number = dt.Rows[0]["application_number"].ToString();
                        UserCode = dt.Rows[0]["user_name"].ToString();
                    }

                    Dictionary<string, object> dic = new Dictionary<string, object>();
                    dic.Add("InstanceID", para.reqId + string.Empty);
                    dic.Add("Application_Number", Application_Number + string.Empty);
                    dic.Add("StatusCode", "拒绝" + string.Empty);
                    dic.Add("Approval_UserCode", UserCode + string.Empty);
                    dic.Add("Approval_Comment", "自动审批系统拒绝" + string.Empty);
                    CommonFunction.ExecuteBizBus("CAPServiceNew", "ProposalApproval", dic);

                    AppUtility.Engine.LogWriter.Write("风控系统拒绝，回写cap状态：" + Newtonsoft.Json.JsonConvert.SerializeObject(dic));

                    //结束当前任务
                    string workitemid = WorkFlowFunction.getRSWorkItemIDByInstanceid(para.reqId, "东正风控评估");
                    var item = AppUtility.Engine.WorkItemManager.GetWorkItem(workitemid);
                    if (item != null)
                    {
                        AppUtility.Engine.WorkItemManager.FinishWorkItem(workitemid, "", AccessPoint.InternalSystem, "", OThinker.Data.BoolMatchValue.False, "", "Submit", ActionEventType.Forward, 0);

                        // 需要通知实例事件管理器结束事件
                        OThinker.H3.Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                                OThinker.H3.Messages.MessageEmergencyType.Normal,
                                item.InstanceId,
                                item.ActivityCode,
                                item.TokenId,
                                 OThinker.Data.BoolMatchValue.True,
                                false,
                                 OThinker.Data.BoolMatchValue.True,
                                false,
                                null);
                        AppUtility.Engine.InstanceManager.SendMessage(endMessage);
                    }
                    //结束流程
                    WorkFlowFunction.FinishedInstance(para.reqId);

                    fkresult = "拒绝";

                    break;
                case "03":
                    //转人工
                    keyValues.Add(new DataItemParam()
                    {
                        ItemName = "Automatic_approval",
                        ItemValue = 0
                    });

                    WorkFlowFunction.SetItemValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, keyValues, WorkFlowFunction.GetUserIDByCode("rsfk"));

                    AllocationOrder allocationOrder = new AllocationOrder();
                    //不需要分单
                    if (!allocationOrder.NeedAllocationOrder(instanceContext))
                    {
                        //2.提交当前任务
                        var success = WorkFlowFunction.SubmitWorkItemByInstanceID(para.reqId, "");
                        if (!success)
                        {
                            AppUtility.Engine.LogWriter.Write("风控系统返回03-转人工，提交当前任务失败" + Newtonsoft.Json.JsonConvert.SerializeObject(para));
                        }
                        //必须要放在提交任务之后赋值，否则可能会有并发的风险；
                        resultValue.Add("FK_RESULT", "转人工");
                        updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);
                    }
                    else//需要分单
                    {
                        resultValue.Add("FK_RESULT", "转人工");
                        resultValue.Add("allocation_available", 1);
                        updateResultFlag = AppUtility.Engine.BizObjectManager.SetPropertyValues(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, "", resultValue);
                    }
                    fkresult = "转人工";

                    break;
                default:
                    rtn.code = -10001;
                    rtn.message = "finalResult不合法";
                    break;
            }

            //更新风控返回结果
            //AppUtility.Engine.BizObjectManager.SetPropertyValue(instanceContext.BizObjectSchemaCode, instanceContext.BizObjectId, string.Empty, "fkResult", fkresult);

            AppUtility.Engine.LogWriter.Write("风控系统回调结果fkResult：" + fkresult);


            AppUtility.Engine.LogWriter.Write("风控系统回调结果更新fkResult：" + updateResultFlag.ToString());

            return Json(rtn);
        }

        [Xss]
        public JsonResult GetPersons(string InstanceId, string ApplicationNumber)
        {
            InstanceContext ic = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
            var r = GetPersonsInfo(ic.BizObjectId);
            var obj_paras = new { reqId = InstanceId, requestId = ApplicationNumber };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询NCIIC参数：" + str_paras);
            var nciicResponse = HttpHelper.PostWebRequest<QueryNciicResultResponse>(url_NCIIC, "application/json", str_paras);

            if (nciicResponse == null)
            {
                return Json(new { Person = r, NCIIC = new List<NciicResultDto>() }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Person = r, NCIIC = nciicResponse.nciicResultList }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Xss]
        public JsonResult Test2(string status, string instanceid, string recordId)
        {
            if (status == "AllNew" || status == "AllOld" || status == "PartIn")
            {
                AppUtility.Engine.SettingManager.SetCustomSetting(FKFenDanStatus, status);
            }

            //if (Cache.GetCache("RELATIONSHIP") == null)
            //{
            //    Dictionary<string, string> dic = new Dictionary<string, string>();
            //    var r = BizService.ExecuteBizNonQuery("DropdownListDataSource", "get_relationship", null);
            //    OThinker.H3.BizBus.BizService.BizStructure[] result = (OThinker.H3.BizBus.BizService.BizStructure[])r["List"];
            //    foreach (var biz in result)
            //    {
            //        dic.Add(biz["RELATIONSHIP_CDE"] + string.Empty, biz["RELATIONSHIP_DSC"] + string.Empty);
            //    }
            //    Cache.SetCache("RELATIONSHIP", dic);
            //    return Json(dic, JsonRequestBehavior.AllowGet);
            //}
            //else
            //{
            //    var r = (Dictionary<string, string>)Cache.GetCache("RELATIONSHIP");
            //    return Json(r, JsonRequestBehavior.AllowGet);
            //}

            if (status == "1234")
            {

                string sql = @"select * from ot_instancecontext where instancename like 'Br-%' and bizobjectschemacode='APPLICATION' and createdtime >= to_date('2019-03-01','yyyy-MM-dd') and  createdtime < to_date('2019-04-01','yyyy-MM-dd') order by createdtime desc ";


                var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

                //var index = 39;

                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    var instanceId = dt.Rows[i]["objectid"].ToString();

                    try
                    {
                        recordId = RiskEvaluation(instanceId);

                        AppUtility.Engine.LogWriter.Write(string.Format("自动提交风控Api，第{0}次，InstanceId={1},recordId={2}", i, instanceId, recordId));
                    }
                    catch (Exception ex)
                    {
                        AppUtility.Engine.LogWriter.Write(string.Format("自动提交风控Api，第{0}次，InstanceId={1},recordId={2},错误={3}", i, instanceId, recordId, ex.ToString()));
                    }
                }
            }

            if (status == "delfkreport")
            {
                DelFkReport(instanceid);
            }

            if (status == "addfkreport")
            {
                var result = "[{\"FielaName\":\"APPLICATION\",\"FieldItem\":[{\"ItemFieldName\":\"BP_SALESPERSON_ID\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select bm.business_partner_nme from bp_main@to_cms bm where business_partner_id='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"BUSINESS_PARTNER_ID\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select business_partner_nme from bp_main@to_cms where business_partner_id='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"STATE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select code.state_nme from  state_code@to_cms code where code.state_cde ='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CITY_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select code.city_nme from  city_code@to_cms code where code.city_cde = '{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"VEHICLE_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select vehicle_type_dsc from vehicle_type_code@to_cms where activate_ind='T' and Vehicle_Type_Cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"BP_DEALER_ID\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select bm.business_partner_nme from bp_main@to_cms bm where bm.business_partner_id='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"BP_SHOWROOM_ID\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select bm.business_partner_nme from bp_main@to_cms bm where business_partner_id='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"dysf\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select codename from area where codeid ='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"dycs\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select codename from area where codeid ='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"dyx\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select codename from area where codeid ='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"BP_COMPANY_ID\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"申请贷款公司\"},{\"ItemFieldName\":\"APPLICATION_TYPE_CODE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"申请类型\"}]},{\"FielaName\":\"APPLICANT_DETAIL\",\"FieldItem\":[{\"ItemFieldName\":\"GUARANTOR_RELATIONSHIP_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select relationship_dsc from relationship_code@to_cms where activate_ind='T' and relationship_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"INDUSTRY_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select industry_type_dsc from industry_type_code@to_cms where activate_ind='T' and industry_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"INDUSTRY_SUBTYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select industry_subtype_dsc from industry_subtype_code@to_cms where  activate_ind='T' and industry_subtype_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"OCCUPATION_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select OCCUPATION_DSC from occupation_code@to_cms where activate_ind='T' and OCCUPATION_CDE='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"SUB_OCCUPATION_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select sub_occupation_dsc from sub_occupation@to_cms where activate_ind='T' and sub_Occupation_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"JOB_GROUP_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select job_group_dsc from job_group@to_cms where  activate_ind='T' and job_group_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"ID_CARD_TYP\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"证件类型\"},{\"ItemFieldName\":\"SEX\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"性别\"},{\"ItemFieldName\":\"HUKOU_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"户口所在地\"},{\"ItemFieldName\":\"NATION_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"民族\"},{\"ItemFieldName\":\"EDUCATION_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"教育程度\"},{\"ItemFieldName\":\"DRIVING_LICENSE_CODE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"驾照状态\"},{\"ItemFieldName\":\"EMPLOYMENT_TYPE_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"雇员类型\"},{\"ItemFieldName\":\"DESIGNATION_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"职位\"},{\"ItemFieldName\":\"MARITAL_STATUS_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"婚姻状况\"},{\"ItemFieldName\":\"RACE_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"国籍\"}]},{\"FielaName\":\"COMPANY_DETAIL\",\"FieldItem\":[{\"ItemFieldName\":\"BUSINESS_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"SELECT business_type_dsc FROM BUSINESS_TYPE_CODE@to_cms where activate_ind='T' and business_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"INDUSTRY_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select industry_type_dsc from industry_type_code@to_cms where activate_ind='T' and industry_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"INDUSTRY_SUBTYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select industry_subtype_dsc from industry_subtype_code@to_cms where  activate_ind='T' and industry_subtype_cde='{0}'\",\"FindNameByCode\":\"\"}]},{\"FielaName\":\"ADDRESS\",\"FieldItem\":[{\"ItemFieldName\":\"STATE_CDE4\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select state_nme from state_code@to_cms where Activate_Ind='T' and state_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CITY_CDE4\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select city_nme from city_code@to_cms where  Activate_Ind='T' and city_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"ADDRESS_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select address_type_dsc from address_type_code@to_cms where activate_ind='T' and address_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CurrentState\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select state_nme from state_code@to_cms where Activate_Ind='T' and state_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CurrentCity\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select city_nme from city_code@to_cms where  Activate_Ind='T' and city_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"PROPERTY_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select property_type_dsc from property_type_code@to_cms where activate_ind='T'  and property_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"RESIDENCE_TYPE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select residence_dsc from RESIDENCE_TYPE_Code@To_Cms where activate_ind='T' and residence_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"COUNTRY_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"国家\"},{\"ItemFieldName\":\"ADDRESS_STATUS\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"地址状态\"}]},{\"FielaName\":\"EMPLOYER\",\"FieldItem\":[{\"ItemFieldName\":\"BUSINESS_NATURE_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"SELECT business_type_dsc,activate_ind FROM BUSINESS_TYPE_CODE@to_cms where activate_ind='T' and business_type_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"STATE_CDE2\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select state_nme from state_code@to_cms where Activate_Ind='T' and state_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CITY_CDE6\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select city_nme from city_code@to_cms where  Activate_Ind='T' and city_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"DESIGNATION_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"职位\"}]},{\"FielaName\":\"PERSONNAL_REFERENCE\",\"FieldItem\":[{\"ItemFieldName\":\"RELATIONSHIP_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select relationship_dsc from relationship_code@to_cms where activate_ind='T' and relationship_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"STATE_CDE10\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select state_nme from state_code@to_cms where Activate_Ind='T' and state_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CITY_CDE10\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select city_nme from city_code@to_cms where  Activate_Ind='T' and city_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"HOKOU_TYPE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"户口所在地\"}]},{\"FielaName\":\"VEHICLE_DETAIL\",\"FieldItem\":[{\"ItemFieldName\":\"COLOR\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"select color_dsc from asset_color_code@to_cms where activate_ind='T' and color_cde='{0}'\",\"FindNameByCode\":\"\"},{\"ItemFieldName\":\"CONDITION\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"资产状况\"},{\"ItemFieldName\":\"USAGE7\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"购车目的\"},{\"ItemFieldName\":\"TRANSMISSION\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"变速器\"},{\"ItemFieldName\":\"GURANTEE_OPTION_H3\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"担保方式\"}]},{\"FielaName\":\"ASSET_ACCESSORY\",\"FieldItem\":[{\"ItemFieldName\":\"ACCESSORY_CDE\",\"FindNameMethod\":\"schemacode\",\"FindNameBySQL\":\"SELECT accessory_dsc FROM accessory_code@to_cms where activate_ind='T' and accessory_cde='{0}'\",\"FindNameByCode\":\"\"}]},{\"FielaName\":\"CONTRACT_DETAIL\",\"FieldItem\":[{\"ItemFieldName\":\"FREQUENCY_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"付款频率\"}]},{\"FielaName\":\"APPLICANT_PHONE_FAX\",\"FieldItem\":[{\"ItemFieldName\":\"PHONE_TYPE_CDE\",\"FindNameMethod\":\"masterdatacategory\",\"FindNameBySQL\":\"\",\"FindNameByCode\":\"电话类型\"}]}]";

                SaveFkReport(result, instanceid, "fkreport", recordId);
            }
            if (status == "addbankcardinfo")
            {
                var result = "{\"code\":\"00\",\"message\":\"调用成功\",\"data\":[{\"data\":[{\"code\":\"auth_result\",\"value\":\"验证成功\",\"key\":\"银联三要素验证结果\"},{\"code\":\"comcredit\",\"value\":[{\"code\":\"zh2\",\"value\":\"-\",\"key\":\"常驻城市\"},{\"code\":\"zh3\",\"value\":\"1\",\"key\":\"关联银行卡数\"},{\"code\":\"zh4\",\"value\":\"0\",\"key\":\"信用卡数量\"},{\"code\":\"zh5\",\"value\":\"1\",\"key\":\"借记卡数量\"},{\"code\":\"zh6\",\"value\":\"0\",\"key\":\"非普卡数量\"},{\"code\":\"zh8\",\"value\":\"2\",\"key\":\"历史关联卡交易总天数\"},{\"code\":\"xf3\",\"value\":\"低\",\"key\":\"近12月关联卡交易活跃度\"},{\"code\":\"zh34\",\"value\":\"41\",\"key\":\"近12月关联卡最近一笔交易距今月数\"},{\"code\":\"zh33\",\"value\":\"-\",\"key\":\"近12月关联卡夜交易总金额\"},{\"code\":\"zh9\",\"value\":\"1\",\"key\":\"关联卡近12月有交易月数\"},{\"code\":\"zh17\",\"value\":\"(0,1000]\",\"key\":\"关联卡近12月交易金额\"}],\"key\":\"综合评估\"},{\"code\":\"cusability\",\"value\":[{\"code\":\"xf2\",\"value\":\"一档\",\"key\":\"消费档次\"},{\"code\":\"xf7\",\"value\":\"(0,1000]\",\"key\":\"历史关联卡消费总金额\"},{\"code\":\"xf28\",\"value\":\"-\",\"key\":\"近6月关联卡餐饮消费总金额\"},{\"code\":\"xf29\",\"value\":\"-\",\"key\":\"近6月关联卡出行消费总金额\"},{\"code\":\"xf30\",\"value\":\"-\",\"key\":\"近6月关联卡博彩消费总金额\"},{\"code\":\"xf33\",\"value\":\"-\",\"key\":\"近6月关联卡整额消费总金额\"},{\"code\":\"xf49\",\"value\":\"-\",\"key\":\"近6月关联卡夜消费总金额\\n\"},{\"code\":\"xf53\",\"value\":\"-\",\"key\":\"近6月最大单日同商户多笔消费笔数\"},{\"code\":\"xf26\",\"value\":\"(0,1000]\",\"key\":\"近6月最大单日消费金额\"},{\"code\":\"xf58\",\"value\":\"-\",\"key\":\"近12月双休日消费总金额\"},{\"code\":\"xf11\",\"value\":\"-\",\"key\":\"近12月关联卡月均消费金额\"},{\"code\":\"xf7\",\"value\":\"(0,1000]\",\"key\":\"近12月消费总金额\"},{\"code\":\"xf15\",\"value\":\"1\",\"key\":\"近12月消费总笔数\"},{\"code\":\"xf62\",\"value\":\"-\",\"key\":\"关联卡近12月工作日工作时间消费金额\"}],\"key\":\"消费能力评估\"},{\"code\":\"asset\",\"value\":[{\"code\":\"zc2\",\"value\":\"Unknown\",\"key\":\"是否有房\"},{\"code\":\"zc8\",\"value\":\"Unknown\",\"key\":\"是否理财\"},{\"code\":\"zc9\",\"value\":\"-\",\"key\":\"近6月关联卡理财总金额\"},{\"code\":\"zc10\",\"value\":\"-\",\"key\":\"近6月关联卡理财总笔数\"},{\"code\":\"zc11\",\"value\":\"-\",\"key\":\"近6月关联卡保险交易金额\"},{\"code\":\"zc12\",\"value\":\"-\",\"key\":\"近6月关联卡保险交易笔数\"},{\"code\":\"zc35\",\"value\":\"-\",\"key\":\"近6月关联卡最大单日取现金总金额\"},{\"code\":\"zc31\",\"value\":\"-\",\"key\":\"近6月关联卡大额单笔[10k，+) 入账总金额\"},{\"code\":\"zc32\",\"value\":\"-\",\"key\":\"近12月关联卡大额单笔[10k，+）入账总金额\"},{\"code\":\"zc16\",\"value\":\"-\",\"key\":\"近12月关联卡银行卡入账总金额\"},{\"code\":\"zc20\",\"value\":\"-\",\"key\":\"近12月关联卡银行卡入账总笔数\"},{\"code\":\"zc36\",\"value\":\"-\",\"key\":\"近12月关联卡最大单日取现总金额\"},{\"code\":\"zh25\",\"value\":\"1\",\"key\":\"近12月关联卡银行交易总金额\"}],\"key\":\"资产状况评估\"},{\"code\":\"netloan\",\"value\":[{\"code\":\"xd16\",\"value\":\"-\",\"key\":\"近5次关联卡借贷最小还款金额\"},{\"code\":\"xd17\",\"value\":\"-\",\"key\":\"近5次关联卡借贷最大还款金额\"},{\"code\":\"xd18\",\"value\":\"-\",\"key\":\"近5次关联卡借贷平均还款金额\"},{\"code\":\"xd19\",\"value\":\"-\",\"key\":\"近5次关联卡借贷平均逾期天数\"},{\"code\":\"xf6\",\"value\":\"(0,1000]\",\"key\":\"近6月关联卡信用卡还款总金额\"},{\"code\":\"xf14\",\"value\":\"1\",\"key\":\"近6月关联卡信用卡还款总笔数\"},{\"code\":\"xd5\",\"value\":\"-\",\"key\":\"近6月关联卡金额不足交易总笔数\"},{\"code\":\"xd13\",\"value\":\"-\",\"key\":\"近6月关联卡最大单日交易失败总笔数\"},{\"code\":\"xd20\",\"value\":\"-\",\"key\":\"近6月关联卡现金借贷最小借贷费率\"},{\"code\":\"xd21\",\"value\":\"-\",\"key\":\"近6月关联卡借贷最小还款金额\"},{\"code\":\"xd29\",\"value\":\"-\",\"key\":\"近12月关联卡最大单月借贷总金额\"},{\"code\":\"xd30\",\"value\":\"-\",\"key\":\"近12月关联卡单月借贷总金额最大月距今月数\\n\"},{\"code\":\"xd14\",\"value\":\"-\",\"key\":\"近12月关联卡交易失败总笔数\"},{\"code\":\"xd15\",\"value\":\"-\",\"key\":\"近12月关联卡最近一笔失败交易距今天数\"},{\"code\":\"xd22\",\"value\":\"-\",\"key\":\"近12月关联卡新增借贷平台最大借贷费率\"},{\"code\":\"xd23\",\"value\":\"-\",\"key\":\"近12月关联卡消费金融借贷月数占比\"},{\"code\":\"xd24\",\"value\":\"-\",\"key\":\"近12月关联卡消费金融还款总月数\"}],\"key\":\"网贷/消金行为评估\"},{\"code\":\"other\",\"value\":[{\"code\":\"xd4\",\"value\":\"-\",\"key\":\"近3个月余额不足次数\"},{\"code\":\"xd6\",\"value\":\"-\",\"key\":\"近12个月余额不足次数\"},{\"code\":\"bp1014\",\"value\":null,\"key\":\"推测年龄\"},{\"code\":\"cp5028\",\"key\":\"近12月入账金额\"},{\"code\":\"cp5033\",\"key\":\"近12月出账金额\"},{\"code\":\"cp5096\",\"key\":\"近12月交易代付笔数\"},{\"code\":\"cp0003\",\"key\":\"交易地域（N线城市）\"},{\"code\":\"cp0002\",\"key\":\"具体城市（按笔数）\"},{\"code\":\"cp0001\",\"key\":\"具体城市（按天数）\"},{\"code\":\"bp1005\",\"key\":\"卡等级\"},{\"code\":\"cp0228\",\"key\":\"消费水平在当地城市排名(近12月消费金额同城排名)\"}],\"key\":\"其他指标\"}],\"idCard\":\"142601196508111940\",\"card\":\"6230940060000169718\"},{\"data\":[{\"code\":\"auth_result\",\"value\":\"验证成功\",\"key\":\"银联三要素验证结果\"},{\"code\":\"comcredit\",\"value\":[{\"code\":\"zh2\",\"value\":\"天津市\",\"key\":\"常驻城市\"},{\"code\":\"zh3\",\"value\":\"1\",\"key\":\"关联银行卡数\"},{\"code\":\"zh4\",\"value\":\"1\",\"key\":\"信用卡数量\"},{\"code\":\"zh5\",\"value\":\"0\",\"key\":\"借记卡数量\"},{\"code\":\"zh6\",\"value\":\"0\",\"key\":\"非普卡数量\"},{\"code\":\"zh8\",\"value\":\"52\",\"key\":\"历史关联卡交易总天数\"},{\"code\":\"xf3\",\"value\":\"低\",\"key\":\"近12月关联卡交易活跃度\"},{\"code\":\"zh34\",\"value\":\"284\",\"key\":\"近12月关联卡最近一笔交易距今月数\"},{\"code\":\"zh33\",\"value\":\"-\",\"key\":\"近12月关联卡夜交易总金额\"},{\"code\":\"zh9\",\"value\":\"1\",\"key\":\"关联卡近12月有交易月数\"},{\"code\":\"zh17\",\"value\":\"(0,1000]\",\"key\":\"关联卡近12月交易金额\"}],\"key\":\"综合评估\"},{\"code\":\"cusability\",\"value\":[{\"code\":\"xf2\",\"value\":\"三档\",\"key\":\"消费档次\"},{\"code\":\"xf7\",\"value\":\"(0,1000]\",\"key\":\"历史关联卡消费总金额\"},{\"code\":\"xf28\",\"value\":\"-\",\"key\":\"近6月关联卡餐饮消费总金额\"},{\"code\":\"xf29\",\"value\":\"-\",\"key\":\"近6月关联卡出行消费总金额\"},{\"code\":\"xf30\",\"value\":\"-\",\"key\":\"近6月关联卡博彩消费总金额\"},{\"code\":\"xf33\",\"value\":\"-\",\"key\":\"近6月关联卡整额消费总金额\"},{\"code\":\"xf49\",\"value\":\"-\",\"key\":\"近6月关联卡夜消费总金额\\n\"},{\"code\":\"xf53\",\"value\":\"-\",\"key\":\"近6月最大单日同商户多笔消费笔数\"},{\"code\":\"xf26\",\"value\":\"-\",\"key\":\"近6月最大单日消费金额\"},{\"code\":\"xf58\",\"value\":\"-\",\"key\":\"近12月双休日消费总金额\"},{\"code\":\"xf11\",\"value\":\"(0,1000]\",\"key\":\"近12月关联卡月均消费金额\"},{\"code\":\"xf7\",\"value\":\"(0,1000]\",\"key\":\"近12月消费总金额\"},{\"code\":\"xf15\",\"value\":\"1\",\"key\":\"近12月消费总笔数\"},{\"code\":\"xf62\",\"value\":\"-\",\"key\":\"关联卡近12月工作日工作时间消费金额\"}],\"key\":\"消费能力评估\"},{\"code\":\"asset\",\"value\":[{\"code\":\"zc2\",\"value\":\"Unknown\",\"key\":\"是否有房\"},{\"code\":\"zc8\",\"value\":\"Unknown\",\"key\":\"是否理财\"},{\"code\":\"zc9\",\"value\":\"-\",\"key\":\"近6月关联卡理财总金额\"},{\"code\":\"zc10\",\"value\":\"-\",\"key\":\"近6月关联卡理财总笔数\"},{\"code\":\"zc11\",\"value\":\"-\",\"key\":\"近6月关联卡保险交易金额\"},{\"code\":\"zc12\",\"value\":\"-\",\"key\":\"近6月关联卡保险交易笔数\"},{\"code\":\"zc35\",\"value\":\"-\",\"key\":\"近6月关联卡最大单日取现金总金额\"},{\"code\":\"zc31\",\"value\":\"-\",\"key\":\"近6月关联卡大额单笔[10k，+) 入账总金额\"},{\"code\":\"zc32\",\"value\":\"-\",\"key\":\"近12月关联卡大额单笔[10k，+）入账总金额\"},{\"code\":\"zc16\",\"value\":\"-\",\"key\":\"近12月关联卡银行卡入账总金额\"},{\"code\":\"zc20\",\"value\":\"-\",\"key\":\"近12月关联卡银行卡入账总笔数\"},{\"code\":\"zc36\",\"value\":\"-\",\"key\":\"近12月关联卡最大单日取现总金额\"},{\"code\":\"zh25\",\"value\":\"-\",\"key\":\"近12月关联卡银行交易总金额\"}],\"key\":\"资产状况评估\"},{\"code\":\"netloan\",\"value\":[{\"code\":\"xd16\",\"value\":\"-\",\"key\":\"近5次关联卡借贷最小还款金额\"},{\"code\":\"xd17\",\"value\":\"-\",\"key\":\"近5次关联卡借贷最大还款金额\"},{\"code\":\"xd18\",\"value\":\"-\",\"key\":\"近5次关联卡借贷平均还款金额\"},{\"code\":\"xd19\",\"value\":\"-\",\"key\":\"近5次关联卡借贷平均逾期天数\"},{\"code\":\"xf6\",\"value\":\"-\",\"key\":\"近6月关联卡信用卡还款总金额\"},{\"code\":\"xf14\",\"value\":\"-\",\"key\":\"近6月关联卡信用卡还款总笔数\"},{\"code\":\"xd5\",\"value\":\"-\",\"key\":\"近6月关联卡金额不足交易总笔数\"},{\"code\":\"xd13\",\"value\":\"-\",\"key\":\"近6月关联卡最大单日交易失败总笔数\"},{\"code\":\"xd20\",\"value\":\"-\",\"key\":\"近6月关联卡现金借贷最小借贷费率\"},{\"code\":\"xd21\",\"value\":\"-\",\"key\":\"近6月关联卡借贷最小还款金额\"},{\"code\":\"xd29\",\"value\":\"-\",\"key\":\"近12月关联卡最大单月借贷总金额\"},{\"code\":\"xd30\",\"value\":\"-\",\"key\":\"近12月关联卡单月借贷总金额最大月距今月数\"},{\"code\":\"xd14\",\"value\":\"-\",\"key\":\"近12月关联卡交易失败总笔数\"},{\"code\":\"xd15\",\"value\":\"-\",\"key\":\"近12月关联卡最近一笔失败交易距今天数\"},{\"code\":\"xd22\",\"value\":\"-\",\"key\":\"近12月关联卡新增借贷平台最大借贷费率\"},{\"code\":\"xd23\",\"value\":\"-\",\"key\":\"近12月关联卡消费金融借贷月数占比\"},{\"code\":\"xd24\",\"value\":\"-\",\"key\":\"近12月关联卡消费金融还款总月数\"}],\"key\":\"网贷/消金行为评估\"},{\"code\":\"other\",\"value\":[{\"code\":\"xd4\",\"value\":\"-\",\"key\":\"近3个月余额不足次数\"},{\"code\":\"xd6\",\"value\":\"-\",\"key\":\"近12个月余额不足次数\"},{\"code\":\"bp1014\",\"value\":\"top80\",\"key\":\"推测年龄\"},{\"code\":\"cp5028\",\"key\":\"近12月入账金额\"},{\"code\":\"cp5033\",\"key\":\"近12月出账金额\"},{\"code\":\"cp5096\",\"key\":\"近12月交易代付笔数\"},{\"code\":\"cp0003\",\"key\":\"交易地域（N线城市）\"},{\"code\":\"cp0002\",\"key\":\"具体城市（按笔数）\"},{\"code\":\"cp0001\",\"key\":\"具体城市（按天数）\"},{\"code\":\"bp1005\",\"key\":\"卡等级\"},{\"code\":\"cp0228\",\"key\":\"消费水平在当地城市排名(近12月消费金额同城排名)\"}],\"key\":\"其他指标\"}],\"idCard\":\"142601196508111940\",\"card\":\"3568950028734550\"}]}";
                SaveFkReport(result, instanceid, "bankcardinfo", string.Empty);
            }
            if (status == "departleader")
            {
                AppUtility.Engine.SettingManager.SetCustomSetting("UnionOrderDepartLearder", recordId);
            }

            if (status == "c_fidatatracksetting")
            {

            }

            //var str = GetFkReport("4408bd11-d46d-45af-945f-af903c04182a", "fkreport");

            //var tracks = JsonConvert.DeserializeObject<List<FIDataTrack>>(str);

            //var schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema("APPLICATION");

            //var fields = schema.Fields;

            var testJson = "{\"BP_COMPANY_ID\":\"1\",\"APPLICATION_NUMBER\":\"Br-A157572000\",\"VEHICLE_TYPE_CDE\":\"00001\",\"BP_DEALER_ID\":null,\"USER_NAME\":\"62000201\",\"BP_SALESPERSON_ID\":\"120504\",\"CONTACT_PERSON\":\"13162711019\",\"BUSINESS_PARTNER_ID\":\"120462\",\"APPLICATION_DATE\":\"2019-05-07 00:00:00\",\"REFERENCE_NBR\":\"111111111\",\"APPLICATION_TYPE_CODE\":\"00001\",\"BP_SHOWROOM_ID\":\"120473\",\"STATE_CDE\":\"00004\",\"CITY_CDE\":\"00026\",\"IDENTIFICATION_ID\":\"1\",\"bp_id\":\"120462\",\"financedamount\":\"136790.4\",\"PLAN_ID\":\"162048\",\"Cbankname\":\"包商银行\",\"Caccountnum\":\"jdskjafsalalmddsd\",\"dysf\":\"150000\",\"dycs\":\"150100\",\"dyx\":\"150102\",\"dydz\":\"内蒙古自治区呼和浩特市新城区\",\"APPLICATION_TYPE_NAME\":\"个人Individual\",\"APPLICATION_NAME\":\"剑四十八1\",\"engine_number\":\"65434532423447\",\"vin_number\":\"JE434235432546479\",\"isgpd\":\"0\",\"APPLICANT_DETAIL\":[{\"FIRST_THI_NME\":\"剑四十八1\",\"ID_CARD_TYP\":\"00001\",\"ID_CARD_NBR\":\"610111195306274161\",\"DATE_OF_BIRTH\":\"1953-06-27\",\"SEX\":\"F\",\"HUKOU_CDE\":\"00004\",\"NATION_CDE\":\"0001\",\"ISSUING_AUTHORITY\":\"呼和浩特公安局\",\"EDUCATION_CDE\":\"00002\",\"GUARANTOR_RELATIONSHIP_CDE\":\"\",\"SPOUSE_IND\":false,\"DRIVING_LICENSE_CODE\":\"00001\",\"EMPLOYMENT_TYPE_CDE\":\"00007\",\"INDUSTRY_TYPE_CDE\":\"00011\",\"INDUSTRY_SUBTYPE_CDE\":\"00002\",\"OCCUPATION_CDE\":\"00003\",\"SUB_OCCUPATION_CDE\":\"00044\",\"DESIGNATION_CDE\":\"00001\",\"JOB_GROUP_CDE\":\"00002\",\"ACTUAL_SALARY\":\"20000\",\"MARITAL_STATUS_CDE\":\"00001\",\"LAST_NAME\":\"fan\",\"FIRST_NAME\":\"pin\",\"MIDDLE_NAME\":\"一\",\"FORMER_NAME\":\"暧昧\",\"ID_CARDISSUE_DTE\":\"2019-08-21\",\"ID_CARDEXPIRY_DTE\":\"2021-12-24\",\"IDENTIFICATION_CODE2\":\"1\",\"TITLE_CDE\":\"00002\",\"THAI_TITLE_CDE\":\"00002\",\"RACE_CDE\":\"00001\",\"AGE_IN_YEAR\":\"66\",\"AGE_IN_MONTH\":\"5\",\"LIENEE\":false},{\"FIRST_THI_NME\":\"米九五\",\"ID_CARD_TYP\":\"00001\",\"ID_CARD_NBR\":\"621228196608241031\",\"DATE_OF_BIRTH\":\"1966-08-24\",\"SEX\":\"M\",\"HUKOU_CDE\":\"00005\",\"NATION_CDE\":\"0001\",\"ISSUING_AUTHORITY\":\"666\",\"EDUCATION_CDE\":\"00002\",\"GUARANTOR_RELATIONSHIP_CDE\":\"00006\",\"SPOUSE_IND\":true,\"DRIVING_LICENSE_CODE\":\"00001\",\"EMPLOYMENT_TYPE_CDE\":\"00005\",\"INDUSTRY_TYPE_CDE\":\"00007\",\"INDUSTRY_SUBTYPE_CDE\":\"00001\",\"OCCUPATION_CDE\":\"00001\",\"SUB_OCCUPATION_CDE\":\"00042\",\"DESIGNATION_CDE\":\"00002\",\"JOB_GROUP_CDE\":\"00008\",\"ACTUAL_SALARY\":\"66666\",\"MARITAL_STATUS_CDE\":\"00001\",\"LAST_NAME\":\"666\",\"FIRST_NAME\":\"666\",\"MIDDLE_NAME\":\"6666\",\"FORMER_NAME\":\"6666\",\"ID_CARDISSUE_DTE\":\"2019-08-13\",\"ID_CARDEXPIRY_DTE\":\"2019-08-30\",\"IDENTIFICATION_CODE2\":\"2\",\"TITLE_CDE\":\"00001\",\"THAI_TITLE_CDE\":\"00001\",\"RACE_CDE\":\"00001\",\"AGE_IN_YEAR\":\"53\",\"AGE_IN_MONTH\":\"3\",\"LIENEE\":true},{\"FIRST_THI_NME\":\"林九四\",\"ID_CARD_TYP\":\"00001\",\"ID_CARD_NBR\":\"621122198111109604\",\"DATE_OF_BIRTH\":\"1981-11-10\",\"SEX\":\"F\",\"HUKOU_CDE\":\"00004\",\"NATION_CDE\":\"0001\",\"ISSUING_AUTHORITY\":\"和价格浮动\",\"EDUCATION_CDE\":\"00002\",\"GUARANTOR_RELATIONSHIP_CDE\":\"00002\",\"SPOUSE_IND\":false,\"DRIVING_LICENSE_CODE\":\"00001\",\"EMPLOYMENT_TYPE_CDE\":\"00006\",\"INDUSTRY_TYPE_CDE\":\"00001\",\"INDUSTRY_SUBTYPE_CDE\":\"00001\",\"OCCUPATION_CDE\":\"00004\",\"SUB_OCCUPATION_CDE\":\"00045\",\"DESIGNATION_CDE\":\"00001\",\"JOB_GROUP_CDE\":\"00004\",\"ACTUAL_SALARY\":\"54335\",\"MARITAL_STATUS_CDE\":\"00002\",\"LAST_NAME\":\"34发给\",\"FIRST_NAME\":\"vfg\",\"MIDDLE_NAME\":\"ds\",\"FORMER_NAME\":\"与太热\",\"ID_CARDISSUE_DTE\":\"2019-08-06\",\"ID_CARDEXPIRY_DTE\":\"2020-08-20\",\"IDENTIFICATION_CODE2\":\"3\",\"TITLE_CDE\":\"00002\",\"THAI_TITLE_CDE\":\"00002\",\"RACE_CDE\":\"00001\",\"AGE_IN_YEAR\":\"38\",\"AGE_IN_MONTH\":\"0\",\"LIENEE\":false}],\"ADDRESS\":[{\"COUNTRY_CDE\":\"00086\",\"STATE_CDE4\":\"00001\",\"CITY_CDE4\":\"00002\",\"ADDRESS_TYPE_CDE\":\"00002\",\"ADDRESS_ID\":\"浦东新区老垭口11\",\"REGISTERED_ADDRESS\":\"\",\"NATIVE_DISTRICT\":\"上海上海\",\"BIRTHPLEASE_PROVINCE\":\"上海上海\",\"CurrentState\":\"00005\",\"CurrentCity\":\"大同\",\"UNIT_NO\":\"呼啦呼啦县\",\"POST_CODE_2\":\"010010\",\"ADDRESS_STATUS\":\"C\",\"PROPERTY_TYPE_CDE\":\"00008\",\"RESIDENCE_TYPE_CDE\":\"00004\",\"LIVING_FROM_DTE\":\"2019-08-05\",\"IDENTIFICATION_CODE4\":\"1\",\"ADDRESS_CODE\":\"1\",\"TIME_IN_YEAR\":\"0\",\"TIME_IN_MONTH\":\"0\"},{\"COUNTRY_CDE\":\"00086\",\"STATE_CDE4\":\"00001\",\"CITY_CDE4\":\"00002\",\"ADDRESS_TYPE_CDE\":\"00002\",\"ADDRESS_ID\":\"和发给第三方是发\",\"REGISTERED_ADDRESS\":\"\",\"NATIVE_DISTRICT\":\"上海上海\",\"BIRTHPLEASE_PROVINCE\":\"上海上海\",\"CurrentState\":\"00005\",\"CurrentCity\":\"晋城\",\"UNIT_NO\":\"二温柔温热\",\"POST_CODE_2\":\"543242\",\"ADDRESS_STATUS\":\"C\",\"PROPERTY_TYPE_CDE\":\"00001\",\"RESIDENCE_TYPE_CDE\":\"00001\",\"LIVING_FROM_DTE\":\"2019-08-06\",\"IDENTIFICATION_CODE4\":\"2\",\"ADDRESS_CODE\":\"1\",\"TIME_IN_YEAR\":\"0\",\"TIME_IN_MONTH\":\"0\"},{\"COUNTRY_CDE\":\"00086\",\"STATE_CDE4\":\"00008\",\"CITY_CDE4\":\"00059\",\"ADDRESS_TYPE_CDE\":\"00005\",\"ADDRESS_ID\":\"锋倒萨\",\"REGISTERED_ADDRESS\":\"\",\"NATIVE_DISTRICT\":\"吉林白城\",\"BIRTHPLEASE_PROVINCE\":\"吉林白城\",\"CurrentState\":\"00005\",\"CurrentCity\":\"晋城\",\"UNIT_NO\":\"发热撒旦士大夫\",\"POST_CODE_2\":\"654324\",\"ADDRESS_STATUS\":\"C\",\"PROPERTY_TYPE_CDE\":\"00001\",\"RESIDENCE_TYPE_CDE\":\"00002\",\"LIVING_FROM_DTE\":\"2019-08-02\",\"IDENTIFICATION_CODE4\":\"3\",\"ADDRESS_CODE\":\"1\",\"TIME_IN_YEAR\":\"0\",\"TIME_IN_MONTH\":\"0\"}],\"APPLICANT_PHONE_FAX\":[{\"PHONE_SEQ_ID\":\"1\",\"IDENTIFICATION_CODE5\":\"1\",\"COUNTRY_CODE\":\"86\",\"AREA_CODE\":\"\",\"PHONE_NUMBER\":\"13248459708\",\"EXTENTION_NBR\":\"153355\",\"PHONE_TYPE_CDE\":\"00003\",\"ADDRESS_CODE5\":\"1\"},{\"PHONE_SEQ_ID\":\"1\",\"IDENTIFICATION_CODE5\":\"2\",\"COUNTRY_CODE\":\"86\",\"AREA_CODE\":\"\",\"PHONE_NUMBER\":\"13162711019\",\"EXTENTION_NBR\":\"\",\"PHONE_TYPE_CDE\":\"00003\",\"ADDRESS_CODE5\":\"1\"},{\"PHONE_SEQ_ID\":\"1\",\"IDENTIFICATION_CODE5\":\"3\",\"COUNTRY_CODE\":\"86\",\"AREA_CODE\":\"\",\"PHONE_NUMBER\":\"13162766555\",\"EXTENTION_NBR\":\"\",\"PHONE_TYPE_CDE\":\"00003\",\"ADDRESS_CODE5\":\"1\"}],\"EMPLOYER\":[{\"NAME_2\":\"内蒙古吉瑞捷矿业建设有限公司11\",\"BUSINESS_NATURE_CDE\":\"00010\",\"STATE_CDE2\":\"00004\",\"CITY_CDE6\":\"00026\",\"TIME_IN_YEAR_2\":\"5\",\"TIME_IN_MONTH_2\":\"\",\"ADDRESS_ONE_2\":\"一个头两大1010室\",\"DESIGNATION_CDE\":\"00002\",\"PHONE\":\"13525555557\",\"IDENTIFICATION_CODE6\":\"1\",\"EMPLOYEE_LINE_ID\":\"1\"},{\"NAME_2\":\"官方的说法的\",\"BUSINESS_NATURE_CDE\":\"00006\",\"STATE_CDE2\":\"00006\",\"CITY_CDE6\":\"00009\",\"TIME_IN_YEAR_2\":\"5\",\"TIME_IN_MONTH_2\":\"\",\"ADDRESS_ONE_2\":\"和广泛的撒的是\",\"DESIGNATION_CDE\":\"00002\",\"PHONE\":\"13525732097\",\"IDENTIFICATION_CODE6\":\"2\",\"EMPLOYEE_LINE_ID\":\"1\"},{\"NAME_2\":\"给天热为\",\"BUSINESS_NATURE_CDE\":\"00006\",\"STATE_CDE2\":\"00005\",\"CITY_CDE6\":\"00021\",\"TIME_IN_YEAR_2\":\"5\",\"TIME_IN_MONTH_2\":\"\",\"ADDRESS_ONE_2\":\"ui有天赋的v被册封\",\"DESIGNATION_CDE\":\"00002\",\"PHONE\":\"13156545454\",\"IDENTIFICATION_CODE6\":\"3\",\"EMPLOYEE_LINE_ID\":\"1\"}],\"VEHICLE_DETAIL\":[{\"CONDITION\":\"N\",\"USAGE7\":\"1\",\"ASSET_MAKE_DSC\":\"进口马自达\",\"ASSET_BRAND_DSC\":\"马自达5(进口)\",\"POWER_PARAMETER\":\"2008款 马自达5(进口) 2.0L 自动舒适型\",\"MIOCN_NBR\":\"DZ-180503-V034580\",\"TRANSMISSION\":\"Y\",\"COLOR\":\"\",\"NEW_PRICE\":\"169800.00\",\"RELEASE_DTE\":\"2019-07-29\",\"GURANTEE_OPTION_H3\":\"00001;\",\"COMMENTS7\":\"进口马自达 2008款 马自达5(进口) 2.0L 自动舒适型\",\"VEHICLE_AGE\":\"0\",\"ODOMETER_READING\":\"0\",\"RELEASE_YEAR\":\"2019\",\"RELEASE_MONTH\":\"7\",\"IDENTIFICATION_CODE7\":\"1\",\"ASSET_MAKE_CDE\":\"00590\",\"ASSET_BRAND_CDE\":\"03309\",\"VEHICLE_TYPE_CDE\":\"10008\",\"VEHICLE_SUBTYPE_CDE\":\"00164\",\"ASSET_MODEL_CDE\":\"43797\",\"MIOCN_ID\":\"46314\",\"MIOCN_DSC\":\"DZ-180503-V034580\",\"GURANTEE_OPTION\":\"00001\",\"VEHICLE_COMMENT\":\"进口马自达 2008款 马自达5(进口) 2.0L 自动舒适型\"}],\"CONTRACT_DETAIL\":[{\"FP_GROUP_ID\":\"8\",\"FREQUENCY_CDE\":\"00001\",\"FINANCIAL_PRODUCT_ID\":\"694\",\"TOTAL_ASSET_COST\":\"169800.00\",\"LEASE_TERM_IN_MONTH\":\"12\",\"SALE_PRICE\":\"170988.00\",\"ASSET_COST\":\"170988.00\",\"SECURITY_DEPOSIT_PCT\":\"20\",\"FINANCED_AMT_PCT\":\"80\",\"BALLOON_PERCENTAGE\":\"0\",\"CASH_DEPOSIT\":\"34197.60\",\"AMOUNT_FINANCED\":\"136790.40\",\"BALLOON_AMOUNT\":\"0.00\",\"BASE_CUSTOMER_RATE\":\"10.9800\",\"ACTUAL_RTE\":\"10.98\",\"CALC_SUBSIDY_RTE\":\"0\",\"YFJE\":\"179259.48\",\"CALC_SUBSIDY_AMT\":\"0.00\",\"ASSETITC\":\"8271.48\",\"IDENTIFICATION_CODE8\":\"1\",\"TOTAL_ACCESSORY_AMT\":\"1188.00\",\"FINANCIAL_PRODUCT_NAME\":\"车秒贷外-首20-1098\",\"FP_GROUP_NAME\":\"等额本息-车秒贷\",\"WCYE\":\"145061.88\",\"ACCESSORY_AMT\":\"1188.00\"}],\"PMS_RENTAL_DETAIL\":[{\"RENTAL_DETAIL_SEQ\":\"1\",\"START_TRM\":\"1\",\"END_TRM\":\"12\",\"RENTAL_AMT\":\"12088.49\",\"EQUAL_RENTAL_AMT\":\"12088.49\",\"INTEREST_RTE\":\"10.98\"}],\"ASSET_ACCESSORY\":[{\"ACCESSORY_CDE\":\"00004\",\"PRICE\":\"1188.00\"}],\"PERSONNAL_REFERENCE\":[{\"IDENTIFICATION_CODE10\":\"2\",\"LINE_ID10\":\"1\",\"NAME10\":\"广泛的撒\",\"RELATIONSHIP_CDE\":\"00004\",\"MOBILE\":\"13162711987\",\"ADDRESS_ONE\":\"广泛的撒是否\",\"PHONE\":\"13162711980\",\"HOKOU_TYPE\":\"00004\",\"STATE_CDE10\":\"00006\",\"CITY_CDE10\":\"00010\",\"POST_CODE\":\"543231\"},{\"IDENTIFICATION_CODE10\":\"3\",\"LINE_ID10\":\"1\",\"NAME10\":\"点三十分\",\"RELATIONSHIP_CDE\":\"00001\",\"MOBILE\":\"13187678765\",\"ADDRESS_ONE\":\"应天府广场v额外豆腐干反对下\",\"PHONE\":\"13187678760\",\"HOKOU_TYPE\":\"00004\",\"STATE_CDE10\":\"00005\",\"CITY_CDE10\":\"00020\",\"POST_CODE\":\"654322\"},{\"IDENTIFICATION_CODE10\":\"1\",\"LINE_ID10\":\"1\",\"NAME10\":\"他认为二\",\"RELATIONSHIP_CDE\":\"00004\",\"MOBILE\":\"13525732098\",\"ADDRESS_ONE\":\"山豆根发射点风格\",\"PHONE\":\"13525732098\",\"HOKOU_TYPE\":\"00005\",\"STATE_CDE10\":\"00006\",\"CITY_CDE10\":\"00009\",\"POST_CODE\":\"675432\"}]}";

            var jObject = JObject.Parse(testJson);

            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(testJson);

            foreach (var key in dic.Keys)
            {
                var x = jObject[key] + string.Empty;
                JArray.Parse(x);
            }


            return Json(new { reqID = "123", recordId = "1234" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 风控系统评估Api
        /// </summary>
        /// <param name="para"></param>
        /// <returns></returns>
        [Xss]
        public string RiskEvaluation(string InstanceId)
        {
            Risk.Parameters_Evaluation para = GetRiskEvaluationParameters(InstanceId);
            string str_paras = JsonConvert.SerializeObject(para);
            AppUtility.Engine.LogWriter.Write("调用风控评估参数：" + str_paras);

            var result = HttpHelper.PostWebRequest(url_RiskEvaluation, "application/json", str_paras);
            AppUtility.Engine.LogWriter.Write("风控评估结果:" + result);
            if (string.IsNullOrEmpty(result))
            {
                return string.Empty;
            }
            //调试；
            DZCommonApiResult<object> rtn = JsonConvert.DeserializeObject<DZCommonApiResult<object>>(result);

            AppUtility.Engine.LogWriter.Write(string.Format("调用风控风控评估结果：{0}", JsonConvert.SerializeObject(rtn)));

            if (rtn.code == "00" || rtn.code == "04")
            {

                var data = JsonConvert.SerializeObject(rtn.data);

                var jobj = JObject.Parse(data);

                return jobj["recordId"].ToString();
            }
            else
            {
                return string.Empty; ;
            }
        }

        /// <summary>
        /// 查询NCIIC
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <param name="ApplicationNumber"></param>
        /// <returns></returns>
        [Xss]
        public JsonResult QueryNCIIC(string InstanceId, string ApplicationNumber)
        {
            var obj_paras = new { reqId = InstanceId, requestId = ApplicationNumber };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询NCIIC参数：" + str_paras);
            var result = HttpHelper.PostWebRequest(url_NCIIC, "application/json", str_paras);

            var returnData = JsonConvert.DeserializeObject<DZCommonApiResult<QueryNciicResultResponse>>(result);

            return Json(returnData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询风控评估结果
        /// </summary>
        /// <param name="recordId">风控系统ID</param>
        /// <param name="InstanceId">流程实例ID</param>
        /// <returns></returns>
        [Xss]
        public JsonResult QueryRiskEvaluationResult(string InstanceId, string idCard)
        {
            var dicResult = GetFkReportId(InstanceId);

            if (string.IsNullOrEmpty(dicResult["recordId"]))
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }

            var recordId = dicResult["recordId"];

            var getReport = GetFkReport(InstanceId, "fkreport", recordId);

            if (!string.IsNullOrEmpty(getReport))
            {
                return Json(getReport, JsonRequestBehavior.AllowGet);
            }

            var obj_paras = new { recordId = recordId, reqId = InstanceId, idCard = idCard.ToUpper() };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询风控评估结果：" + str_paras);
            var result = HttpHelper.PostWebRequest(url_RiskEvaluationResult, "application/json", str_paras);

            if (result == null || result == "")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var jObject = JObject.Parse(result);

            if (jObject["code"].ToString() != "00")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //保存入数据库
            SaveFkReport(result, InstanceId, "fkreport", recordId);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 查询PBOC报告
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <param name="ApplicationNumber"></param>
        /// <returns></returns>
        [Xss]
        public JsonResult QueryPBOCReport(string InstanceId, string ApplicationNumber, string recordId)
        {
            var obj_paras = new { reqId = InstanceId, requestId = ApplicationNumber, recordId };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询PBOC结果：" + str_paras);
            var result = HttpHelper.PostWebRequest(url_PBOC, "application/json", str_paras);

            var returnData = JsonConvert.DeserializeObject<DZCommonApiResult<QueryPbocResultResponse>>(result);

            return Json(returnData, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询银行卡信息报告
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <param name="ApplicationNumber"></param>
        /// <returns></returns>
        [Xss]
        public JsonResult QueryBankCardReport(string InstanceId, string IdCard, string Name)
        {
            var getReport = GetFkReport(InstanceId, "bankcardinfo");

            if (!string.IsNullOrEmpty(getReport))
            {
                return Json(getReport, JsonRequestBehavior.AllowGet);
            }

            IdCard = IdCard.ToUpper();

            var bankCardDic = YSP_BankCard(Name, IdCard);

            var obj_paras = new { idCard = IdCard, card1 = bankCardDic["bankcard1"], card2 = bankCardDic["bankcard2"] };
            string str_paras = JsonConvert.SerializeObject(obj_paras);
            AppUtility.Engine.LogWriter.Write("查询银行卡信息报告：" + str_paras);
            var result = HttpHelper.PostWebRequest(url_BankCardResult, "application/json", str_paras);
            AppUtility.Engine.LogWriter.Write("查询银行卡信息报告结果：" + result);


            if (result == null || result == "")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            var jObject = JObject.Parse(result);

            if (jObject["code"].ToString() != "00")
            {
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            //保存入数据库
            SaveFkReport(result, InstanceId, "bankcardinfo", string.Empty);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 风控评估参数
        /// </summary>
        /// <param name="InstanceId"></param>
        /// <returns></returns>
        [Xss]
        public Risk.Parameters_Evaluation GetRiskEvaluationParameters(string InstanceId)
        {
            InstanceContext ic = AppUtility.Engine.InstanceManager.GetInstanceContext(InstanceId);
            InstanceData data = new InstanceData(AppUtility.Engine, InstanceId, "");
            #region 判断是第一次请求还是多次请求
            string requestFlags = "";
            var rtn = WorkflowSetting.GetCustomSettingValue(InstanceId, "requestFlags");
            if (string.IsNullOrEmpty(rtn))
            {
                requestFlags = "00";
                WorkflowSetting.SetCustomSettingValue(InstanceId, ic.BizObjectSchemaCode, "requestFlags", "1");
            }
            else
            {
                requestFlags = "01";
                WorkflowSetting.SetCustomSettingValue(InstanceId, ic.BizObjectSchemaCode, "requestFlags", (Convert.ToInt32(rtn) + 1) + string.Empty);
            }
            #endregion
            Risk.Parameters_Evaluation para = new Risk.Parameters_Evaluation();
            para.reqId = InstanceId;
            para.cbUrl = url_RiskResult;
            para.time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            para.requestId = data["APPLICATION_NUMBER"].Value + string.Empty;//申请单号
            para.requestFlags = requestFlags;//00新增请求;01变更请求，重新跑规则


            Risk.Parameters_Evaluation.Order order = new Risk.Parameters_Evaluation.Order();

            var personsInfo = GetPersonsInfo(ic.BizObjectId);
            //省份
            var dic_Province = GetDropDownListSource("PROVINCE", "get_state_code", "STATE_CDE", "STATE_NME");
            ///获取Mapping的城市；
            var mapping_Cities = GetMappingCities();

            #region 条件判断
            string[] cyzdArray = new string[3];
            if (data["APPLICATION_TYPE_CODE"].Value + string.Empty == "00001")
            {
                string sql_I = @"
select us.code,app_det.id_card_nbr,con_det.fp_group_name from OT_INSTANCECONTEXT con 
left join I_APPLICANT_DETAIL app_det on app_det.parentobjectid=con.bizobjectid
left join I_CONTRACT_DETAIL con_det on con_det.parentobjectid=con.bizobjectid
left join OT_User us on con.originator=us.objectid
where con.objectid='{0}'
and app_det.identification_code2=1
";
                sql_I = string.Format(sql_I, InstanceId);

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
                sql_C = string.Format(sql_C, InstanceId);

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
            string lx = cyzdArray[1];
            #endregion

            #region 订单信息
            string sql_order = @"
select asset.condition,app.USER_Name USER_CODE,us.name USER_NAME,app.dycs,a.codename MortgageCity,app.CITY_CDE,app.state_cde,
asset.ASSET_BRAND_DSC,asset.ASSET_MAKE_DSC,asset.USAGE7,asset.RELEASE_DTE,asset.ODOMETER_READING,
contract.SALE_PRICE,contract.AMOUNT_FINANCED,contract.LEASE_TERM_IN_MONTH,contract.fp_group_name,contract.financial_product_name,contract.Security_Deposit_Pct,
contract.CASH_DEPOSIT,asset.NEW_PRICE,contract.ACCESSORY_AMT,contract.CALC_SUBSIDY_RTE,app.isgpd
from i_application app 
left join i_vehicle_detail asset on app.objectid=asset.parentobjectid
left join i_contract_detail contract on asset.parentobjectid=contract.parentobjectid
left join ot_user us on app.user_name=us.code
left join area a on app.dycs=a.codeid
where app.objectid='{0}'
";
            sql_order = string.Format(sql_order, ic.BizObjectId);
            DataTable dt_order = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_order);
            DataRow row_Order = dt_order.Rows[0];

            Risk.Parameters_Evaluation.Order o = new Risk.Parameters_Evaluation.Order();
            o.businessType = row_Order["condition"] + string.Empty == "N" ? "新车 New" : "二手车 Uesd";
            o.foursNumber = row_Order["USER_CODE"] + string.Empty;
            o.dealerNam = row_Order["USER_NAME"] + string.Empty;
            o.vehicleModel = row_Order["ASSET_BRAND_DSC"] + string.Empty;
            o.vehicleBrand = row_Order["ASSET_MAKE_DSC"] + string.Empty;
            o.vehiclePurpose = row_Order["USAGE7"] + string.Empty == "1" ? "自用 Self-Use" : "商用 Corporate-Use";
            o.vehiclePrice = row_Order["SALE_PRICE"] + string.Empty;
            o.applicationAmount = row_Order["AMOUNT_FINANCED"] + string.Empty;
            o.applicationTerm = row_Order["LEASE_TERM_IN_MONTH"] + string.Empty;
            o.productGroup = row_Order["FP_GROUP_NAME"] + string.Empty;
            o.productType = row_Order["FINANCIAL_PRODUCT_NAME"] + string.Empty;
            o.firstPaymentsRatio = row_Order["SECURITY_DEPOSIT_PCT"] + string.Empty;
            o.firstPaymentsAmount = row_Order["CASH_DEPOSIT"] + string.Empty;
            o.vehicleIdentificationValue = row_Order["NEW_PRICE"] + string.Empty;
            o.extraChargeLoan = row_Order["ACCESSORY_AMT"] + string.Empty;
            o.ifRereqComplete = "是";
            o.ifMortgage = personsInfo.Ifmortgage;
            o.vehicleManufactureDate = GetDate(row_Order["RELEASE_DTE"] + string.Empty);
            o.vehicleKilometers = row_Order["ODOMETER_READING"] + string.Empty;
            o.ifDataComplete = "是";
            //o.ifStockSimplifiedLoan = row_Order[""] + string.Empty;
            //o.exposureValue = row_Order[""] + string.Empty;
            //o.bankNo = row_Order[""] + string.Empty;
            //o.ifQualityDealerCode = row_Order[""] + string.Empty;
            o.plegeAddressCity = row_Order["MortgageCity"] + string.Empty;
            string dealer_code = row_Order["STATE_CDE"] + string.Empty;
            o.resideAddressProvince = dic_Province[dealer_code];
            Dictionary<string, object> dealer_Para = new Dictionary<string, object>();
            dealer_Para.Add("state_cde", dealer_code);
            var dic_Dealer_City = GetDropDownListSource("PROVINCE_" + dealer_code, "get_city_all", dealer_Para, "CITY_CDE", "CITY_NME");
            o.dealerAddressCity = mapping_Cities[dic_Dealer_City[row_Order["CITY_CDE"] + string.Empty]];
            o.customerSource = getFIType(row_Order["USER_CODE"] + string.Empty);
            if (lx != "3")
            {
                ////判断申请人是否有存量的简化贷---待确认
                o.ifStockSimplifiedLoan = GetJHDBySQR(cyzdArray[2], data["APPLICATION_NUMBER"].Value + string.Empty);
                string ck = "0";
                string lastapplication = getLastApplicationNumByIdCard(cyzdArray[2]);
                if (lastapplication != "")
                {
                    ck = getckAll(cyzdArray[2], lastapplication);
                }
                o.exposureValue = ck;
                o.bankNo = data["Caccountnum"].Value + string.Empty;
                //是否属于优质经销商
                o.ifQualityDealerCode = yzjxs(cyzdArray[0]);
            }
            o.interestRates = Convert.ToDecimal(row_Order["CALC_SUBSIDY_RTE"].ToString());
            o.dealerType = row_Order["ASSET_MAKE_DSC"] + string.Empty;
            var maleCardLoans = row_Order["isgpd"] + string.Empty;

            //是否公牌贷标识
            if (string.IsNullOrEmpty(maleCardLoans))
            {
                o.maleCardLoans = 0;
            }
            else
            {
                o.maleCardLoans = Convert.ToInt32(maleCardLoans);
            }

            //判断贷款类型
            //机构贷   ：申请类型：公司
            //公牌贷   ：申请类型：个人，共同借款人为公司
            //借牌贷   ：申请类型：个人， 主贷人非抵押人
            //个人贷   ：非上述三种
            var loanTypeName = data["APPLICATION_TYPE_CODE"].Value + string.Empty == "00001" ? "个人" : "机构";

            var loanType = "个人贷";

            if (loanTypeName == "机构")
            {
                loanType = "机构贷";
            }
            else
            {
                if (o.maleCardLoans > 0)
                {
                    loanType = "公牌贷";
                }
                else if (o.ifMortgage == "否")
                {
                    var person = personsInfo.Persons.Where(p => p.personnelCategory == "B" && p.personnelRelationship != "Husband-Wife配偶关系" && p.lienee == "1").ToList();
                    if (person != null && person.Any())
                    {
                        loanType = "借牌贷";
                    }
                }
            }
            o.loanType = loanType;

            #endregion

            para.personnelList = personsInfo.Persons;
            para.order = o;

            return para;
        }

        [HttpPost]
        [Xss]
        public JsonResult UploadFile(string uuid, string usercode, string schemacode, string datafield)
        {
            rtn_data rtn = new rtn_data();
            if (Request.Files.Count == 0)
            {
                rtn.code = -1;
                rtn.message = "无文件内容";
                return Json(rtn);
            }
            var user = AppUtility.Engine.Organization.GetUserByCode(usercode);
            if (user == null)
            {
                rtn.code = -1;
                rtn.message = "usercode[" + usercode + "]不存在";
                return Json(rtn);
            }
            HttpPostedFileBase file = Request.Files[0];
            byte[] contents = new byte[file.ContentLength];
            file.InputStream.Read(contents, 0, file.ContentLength);
            // 添加这个附件
            OThinker.H3.Data.Attachment attachment = new OThinker.H3.Data.Attachment();
            attachment.ObjectID = Guid.NewGuid().ToString();
            attachment.Content = contents;
            attachment.ContentType = file.ContentType;
            attachment.CreatedBy = user.ObjectID;
            attachment.CreatedTime = DateTime.Now;
            attachment.FileName = Path.GetFileName(file.FileName);
            attachment.LastVersion = true;
            attachment.ModifiedBy = null;
            attachment.ModifiedTime = System.DateTime.Now;
            attachment.BizObjectSchemaCode = schemacode;
            attachment.ContentLength = contents.Length;
            attachment.BizObjectId = uuid;
            attachment.DataField = datafield;
            string obj = AppUtility.Engine.BizObjectManager.AddAttachment(attachment);
            rtn.code = 1;
            rtn.message = "上传成功";
            rtn.data = obj;
            return Json(rtn);
        }

        [HttpPost]
        [Xss]
        public JsonResult SubmitWorkItem(string instanceid, string userCode)
        {
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);
            var user = AppUtility.Engine.Organization.GetUserByCode(userCode);
            rtn_data rtn = new rtn_data();
            if (SubmitItem(instanceid, OThinker.Data.BoolMatchValue.True, "", user.ObjectID))
            {
                rtn.code = 1;
                rtn.message = "提交成功";
            }
            else
            {
                rtn.code = -1;
                rtn.message = "提交任务到下一步失败";
            }
            return Json(rtn);
        }

        [HttpPost]
        [Xss]
        public JsonResult SaveItemValue(string instanceid, string userCode, string para)
        {
            rtn_data rtn = new rtn_data();
            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);
            var user = AppUtility.Engine.Organization.GetUserByCode(userCode);
            if (context == null)
            {
                rtn.code = -1;
                rtn.message = string.Format("instanceid-->{0}错误", instanceid);
                return Json(rtn);
            }
            if (user == null)
            {
                rtn.code = -1;
                rtn.message = string.Format("userCode-->{0}错误", userCode);
                return Json(rtn);
            }

            Dictionary<string, object> namevalue = JsonConvert.DeserializeObject<Dictionary<string, object>>(para);
            if (AppUtility.Engine.BizObjectManager.SetPropertyValues(context.BizObjectSchemaCode, context.BizObjectId, user.ObjectID, namevalue))
            {
                rtn.code = 1;
                rtn.message = "提交成功";
            }
            else
            {
                rtn.code = -1;
                rtn.message = "保存数据失败";
            }
            return Json(rtn);
        }

        /// <summary>
        /// 自动生成零售的待办
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="CustomerName"></param>
        /// <param name="IdCardNumber"></param>
        /// <param name="Mobile"></param>
        /// <returns></returns>
        [HttpPost]
        [Xss]
        public JsonResult GenerateProposalTask(string UserCode, string CustomerName, string IdCardNumber, string Mobile, string AppOrderId)
        {
            rtn_data rtn = new rtn_data();
            if (string.IsNullOrEmpty(UserCode) || string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(IdCardNumber) || string.IsNullOrEmpty(Mobile))
            {
                rtn.code = -1;
                rtn.message = "参数不允许为空";
                return Json(rtn);
            }
            var r = DongZheng.H3.WebApi.Proposal.GenerateProposalTask(UserCode, CustomerName, IdCardNumber, Mobile, AppOrderId);
            return Json(r);
        }

        /// <summary>
        /// 推送数据至泛钛客-电子面签
        /// </summary>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpGet]
        [Xss]
        public JsonResult PushOrderToFTK(string instanceid)
        {

            rtn_data rtn = new rtn_data() { code = 1 };

            InstanceContext ic = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);

            var tokens = ic.GetPreTokens(ic.NextTokenId - 1);

            AppUtility.Engine.LogWriter.Write("面签：-->instanceid=" + instanceid + "next-->" + ic.NextTokenId + " --->" + JsonConvert.SerializeObject(tokens));

            var preTokenId = tokens[0];

            var token = ic.GetToken(preTokenId);

            AppUtility.Engine.LogWriter.Write("面签：-->instanceid=" + instanceid + "--->preTokenId=" + preTokenId + "-->token=" + token.Activity);

            InstanceData data = new InstanceData(AppUtility.Engine, instanceid, "");

            //申请号
            var applicationNumber = data["APPLICATION_NUMBER"].Value.ToString();

            var needFaceSign = data["NeedFaceSign"].Value + string.Empty;

            var faceSignResult = data["FaceSignResult"].Value + string.Empty;

            //1.信审未选择需要面签  2.机构贷不需要面签
            if (needFaceSign != "1")
            {
                AppUtility.Engine.LogWriter.Write("不需要面签，applicationNumber=：" + applicationNumber + "-->instanceid=" + instanceid);

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            if (faceSignResult == "通过")
            {
                AppUtility.Engine.LogWriter.Write("面签通过无需再次面签，applicationNumber=：" + applicationNumber + "-->instanceid=" + instanceid);

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            if (token.Activity == "Activity17")
            {
                AppUtility.Engine.LogWriter.Write("运营初审驳回不需要面签，applicationNumber=：" + applicationNumber + "-->instanceid=" + instanceid);

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }


            //经销商号
            var accountNo = data["USER_NAME"].Value.ToString();

            var personsInfo = GetPersonsInfo(ic.BizObjectId);

            //AppUtility.Engine.LogWriter.Write("0personsInfo.Persons：" + JsonConvert.SerializeObject(personsInfo.Persons));

            var faceSignPersons = personsInfo.Persons.Where(p => p.personnelCategory == "A").Select(p =>
                {
                    var facePerson = new PersonsItemInfo();
                    switch (p.personnelCategory)
                    {
                        case "A":
                            facePerson.PersonCategory = "主借人";
                            break;
                        case "B":
                            facePerson.PersonCategory = "共借人";
                            break;
                        case "C":
                            facePerson.PersonCategory = "担保人";
                            break;
                    }
                    facePerson.Name = p.custName;
                    facePerson.Sex = p.gender;
                    facePerson.IdNumber = p.certNo;
                    facePerson.Birthday = p.custBirthday;
                    facePerson.Mobile = p.mobile;
                    facePerson.MaritalStatus = p.maritalStatus;
                    facePerson.PropertyType = p.houseStatus;
                    facePerson.LivingAddress = p.currentAddressProvince + p.currentAddressCity + p.currentLivingAddress;
                    facePerson.CompanyName = p.employer;
                    facePerson.CompanyProvince = p.employerAddressProvince;
                    facePerson.CompanyAddress = p.employerAddressCity + p.employerAddress;

                    if (p.personnelCategory == "A")
                    {
                        //AppUtility.Engine.LogWriter.Write("3personsInfo.Persons：" + JsonConvert.SerializeObject(personsInfo.Persons));
                        var coBorrower = personsInfo.Persons.Where(item => item.personnelCategory == "B").Select(item => new Tuple<string, string>(item.custName, item.mobile)).FirstOrDefault();

                        //AppUtility.Engine.LogWriter.Write("4personsInfo.Persons：" + JsonConvert.SerializeObject(coBorrower));

                        facePerson.CoBorrowerName = coBorrower == null ? "" : coBorrower.Item1;

                        //AppUtility.Engine.LogWriter.Write("5personsInfo.Persons：" + JsonConvert.SerializeObject(personsInfo.Persons));
                        var mortgagor = personsInfo.Persons.Where(item => item.lienee == "1").Select(item => new Tuple<string, string>(item.custName, item.mobile)).FirstOrDefault();

                        //AppUtility.Engine.LogWriter.Write("6personsInfo.Persons：" + JsonConvert.SerializeObject(mortgagor));
                        facePerson.MortgagorName = mortgagor == null ? "" : mortgagor.Item1;
                    }
                    if (p.maritalStatus.Contains("已婚"))
                    {
                        if (p.personnelCategory == "A")
                        {
                            //AppUtility.Engine.LogWriter.Write("1personsInfo.Persons：" + JsonConvert.SerializeObject(personsInfo.Persons));

                            var maritalInfo = personsInfo.Persons.Where(item => item.personnelRelationship.Contains("Husband-Wife配偶关系")).Select(item => new Tuple<string, string>(item.custName, item.mobile)).FirstOrDefault();

                            //AppUtility.Engine.LogWriter.Write("2personsInfo.Persons：" + JsonConvert.SerializeObject(maritalInfo));

                            facePerson.MaritalName = maritalInfo == null ? "" : maritalInfo.Item1;
                            facePerson.MaritalMobile = maritalInfo == null ? "" : maritalInfo.Item2;
                        }
                        else if (p.personnelRelationship.Contains("Husband-Wife配偶关系"))
                        {
                            var maritalInfo = personsInfo.Persons.Where(item => item.personnelCategory == "A").Select(item => new Tuple<string, string>(item.custName, item.mobile)).FirstOrDefault();

                            facePerson.MaritalName = maritalInfo == null ? "" : maritalInfo.Item1;
                            facePerson.MaritalMobile = maritalInfo == null ? "" : maritalInfo.Item2;
                        }
                    }

                    return facePerson;
                }).ToList();

            //获取Order信息
            string sql_order = @"select app.dydz,asset.condition,app.USER_Name USER_CODE,us.name USER_NAME,app.dycs,a.codename MortgageCity,app.CITY_CDE,app.state_cde,asset.POWER_PARAMETER,asset.ASSET_BRAND_DSC,asset.ASSET_MAKE_DSC,asset.USAGE7,asset.RELEASE_DTE,asset.ODOMETER_READING, contract.SALE_PRICE,contract.AMOUNT_FINANCED, contract.ASSET_COST,contract.TOTAL_ASSET_COST,contract.LEASE_TERM_IN_MONTH,contract.fp_group_name,contract.financial_product_name,contract.Security_Deposit_Pct,
                                        contract.CASH_DEPOSIT,asset.NEW_PRICE,contract.ACCESSORY_AMT,contract.CALC_SUBSIDY_RTE,app.isgpd
                                        from i_application app 
                                        left join i_vehicle_detail asset on app.objectid=asset.parentobjectid
                                        left join i_contract_detail contract on asset.parentobjectid=contract.parentobjectid
                                        left join ot_user us on app.user_name=us.code
                                        left join area a on app.dycs=a.codeid
                                        where app.objectid='{0}'
                                        ";
            sql_order = string.Format(sql_order, ic.BizObjectId);
            DataTable dt_order = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql_order);
            DataRow row_Order = dt_order.Rows[0];

            var contractInfo = new ContractInfo();
            //contractInfo.ProductGroupName = row_Order["fp_group_name"].ToString();
            //contractInfo.ProductType = row_Order["financial_product_name"].ToString();
            contractInfo.DealerName = row_Order["USER_NAME"] + string.Empty;
            contractInfo.AssetMake = row_Order["ASSET_MAKE_DSC"] + string.Empty;
            contractInfo.AssetBrand = row_Order["ASSET_BRAND_DSC"] + string.Empty;
            contractInfo.VehicleComment = row_Order["POWER_PARAMETER"] + string.Empty;
            var lease = row_Order["LEASE_TERM_IN_MONTH"] + string.Empty;
            contractInfo.LeaseTermMonth = Convert.ToInt32(string.IsNullOrEmpty(lease) ? "0" : lease);

            var cash = row_Order["CASH_DEPOSIT"] + string.Empty;
            contractInfo.CashDeposit = Convert.ToDecimal(string.IsNullOrEmpty(cash) ? "0" : cash);

            var security = row_Order["SECURITY_DEPOSIT_PCT"] + string.Empty;
            contractInfo.SecurityDepositPCT = string.Format("{0}%", string.IsNullOrEmpty(security) ? "0" : security);

            //contractInfo.NewPrice = Convert.ToDecimal(row_Order["NEW_PRICE"].ToString());
            var amount = row_Order["AMOUNT_FINANCED"] + string.Empty;
            contractInfo.AmountFinanced = Convert.ToDecimal(string.IsNullOrEmpty(amount) ? "0" : amount);

            var asset = row_Order["ASSET_COST"] + string.Empty;
            contractInfo.ContractPrice = Convert.ToDecimal(string.IsNullOrEmpty(asset) ? "0" : asset);

            var totalAsset = row_Order["TOTAL_ASSET_COST"] + string.Empty;
            contractInfo.CarPrice = Convert.ToDecimal(string.IsNullOrEmpty(totalAsset) ? "0" : totalAsset);

            var access = row_Order["ACCESSORY_AMT"] + string.Empty;
            contractInfo.ExtraChargePrice = Convert.ToDecimal(string.IsNullOrEmpty(access) ? "0" : access);
            contractInfo.MortgageCity = row_Order["dydz"] + string.Empty;

            var faceSinRequest = new FaceSignRequest();
            faceSinRequest.AccountNo = accountNo;
            faceSinRequest.ApplicationNumber = applicationNumber;
            faceSinRequest.Contract = contractInfo;
            faceSinRequest.Persons = faceSignPersons;

            var str_paras = JsonConvert.SerializeObject(faceSinRequest);
            AppUtility.Engine.LogWriter.Write("面签推送数据：" + str_paras);

            var result = HttpHelper.PostWebRequest(faceSignRequestUrl, "application/json", str_paras);
            AppUtility.Engine.LogWriter.Write("面签推送数据结果:" + result);

            if (string.IsNullOrEmpty(result))
            {
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var jObject = JObject.Parse(result);

            if (jObject["code"].ToString() == "000000")
            {

                var keyvalues = new Dictionary<string, object>();

                keyvalues.Add("FaceSignResult", "");

                AppUtility.Engine.BizObjectManager.SetPropertyValues(ic.BizObjectSchemaCode, ic.BizObjectId, "", keyvalues);
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 电字面签返回结果
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult FTKFaceSignResult(FaceSignCallBackResponse response)
        {
            AppUtility.Engine.LogWriter.Write("电字面签回调结果:" + JsonConvert.SerializeObject(response));

            rtn_data rtn = new rtn_data() { code = 1 };
            try
            {
                var dataTable = GetApplicationByAppNumber(response.applicationNumber);

                if (dataTable == null)
                {
                    AppUtility.Engine.LogWriter.Write("无此申请号:" + JsonConvert.SerializeObject(response));
                    rtn.code = 0;
                    rtn.message = "无此申请号";
                    return Json(rtn, JsonRequestBehavior.AllowGet);
                }


                InstanceContext instance = AppUtility.Engine.InstanceManager.GetInstanceContextsByBizObject("APPLICATION", dataTable.Rows[0]["ObjectId"] + string.Empty).First();


                var keyvalues = new Dictionary<string, object>();

                var retryNo = dataTable.Rows[0]["FaceSignRetryNo"] + string.Empty == "" ? 0 : Convert.ToInt32(dataTable.Rows[0]["FaceSignRetryNo"]) + 1;

                keyvalues.Add("FaceSignResult", response.bizInfoData.bizSts);
                keyvalues.Add("FaceSignRetryNo", retryNo);
                keyvalues.Add("FaceSignLastAppNo", response.appNo);

                AppUtility.Engine.BizObjectManager.SetPropertyValues(instance.BizObjectSchemaCode, instance.BizObjectId, "", keyvalues);

                InsertFaceSignResult(response.bizRelDetails, dataTable.Rows[0]["ObjectId"] + string.Empty, dataTable.Rows[0]["APPLICATION_NUMBER"] + string.Empty, retryNo, response.appNo, response.bizInfoData.bizSts);

                AppUtility.Engine.LogWriter.Write("电字面签回调结果Success:" + JsonConvert.SerializeObject(response));

                //获取当前流程
                var workItemId = WorkFlowFunction.getRSWorkItemIDByInstanceid(instance.ObjectID, "等待面核结果");

                if (string.IsNullOrEmpty(workItemId))
                {
                    AppUtility.Engine.LogWriter.Write("电字面签回调结果当前流程不处于等待面核结果：" + Newtonsoft.Json.JsonConvert.SerializeObject(response));
                    rtn.code = 1;
                    return Json(rtn, JsonRequestBehavior.AllowGet);
                }

                //等待1s，提交
                System.Threading.Thread.Sleep(1000);

                var success = WorkFlowFunction.SubmitWorkItemByInstanceID(instance.ObjectID, "");
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("电字面签回调错误:" + JsonConvert.SerializeObject(response) + "，error：" + ex.ToString());
            }
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 获取面签信息
        /// </summary>
        /// <param name="instanceid"></param>
        /// <returns></returns>
        [HttpGet]
        [Xss]
        public JsonResult QueryFaceSignResult(string instanceid)
        {
            rtn_data rtn = new rtn_data() { code = 1 };

            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);

            var results = GetFaceSignResult(context.BizObjectId);
            rtn.data = results;
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 等待面签结果提交
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        [HttpGet]
        [Xss]
        public JsonResult SubmitFaceSignResult(string instanceId)
        {
            rtn_data rtn = new rtn_data() { code = 1 };

            InstanceData data = new InstanceData(AppUtility.Engine, instanceId, "");

            var faceSignResult = data["FaceSignResult"].Value + string.Empty;

            if (!string.IsNullOrEmpty(faceSignResult))
            {
                AppUtility.Engine.LogWriter.Write("等待面核结果自动提交:" + faceSignResult);
                var success = WorkFlowFunction.SubmitWorkItemByInstanceID(instanceId, "");
            }

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// FI进件传阅
        /// </summary>
        /// <param name="userCode"></param>
        /// <returns></returns>
        [HttpGet]
        [Xss]
        public JsonResult FITransfer(string instanceId, string userCode, string activityCode)
        {
            AppUtility.Engine.LogWriter.Write($"FITransfer：userCode={userCode}-->instanceId={instanceId}-->activityCode={activityCode}");

            rtn_data rtn = new rtn_data();

            if (getFIType(userCode) == "内网")
            {
                AppUtility.Engine.LogWriter.Write("FITransfer内网经销商：" + userCode + "无需传阅" + "-->instanceId=" + instanceId);
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            //查询经销商编码
            var dealerCodeSql = @"select * from in_cms.V_BUSINESS_PARTENTER_INFO where user_name='{0}'";

            dealerCodeSql = string.Format(dealerCodeSql, userCode);

            var dealerDt = CommonFunction.ExecuteDataTableSql("CAPDB", dealerCodeSql);

            if (!CommonFunction.hasData(dealerDt))
            {
                AppUtility.Engine.LogWriter.Write("FITransfer无此经销商数据：" + userCode + "-->instanceId=" + instanceId);
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            AppUtility.Engine.LogWriter.Write("FITransfer经销商" + userCode + "：" + JsonConvert.SerializeObject(dealerDt) + "-->instanceId=" + instanceId);

            var dealercode = dealerDt.Rows[0]["DEALER_CODE"] + string.Empty;


            //查询各个级别经理账号
            var leaderSql = @"select * from interface.v_crm_staff_level where extend84='{0}'";

            var leaderDt = CommonFunction.ExecuteDataTableSql("crm_interface", string.Format(leaderSql, dealercode));

            if (!CommonFunction.hasData(leaderDt))
            {
                AppUtility.Engine.LogWriter.Write("FITransfer无此经销商经理数据：" + userCode + "-->instanceId=" + instanceId);
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }
            //一级经理
            var level1Manager = leaderDt.Rows[0]["account1"] + string.Empty;

            //二级经理
            var level2Manager = leaderDt.Rows[0]["account2"] + string.Empty;

            //三级经理
            var level3Manager = leaderDt.Rows[0]["account3"] + string.Empty;

            //四级
            var level4Manager = leaderDt.Rows[0]["account4"] + string.Empty;

            var userSql = @"select * from h3.ot_user {0}";

            var whereSql = @" where code in({0}) ";

            var conditionSql = string.Empty;

            if (!string.IsNullOrEmpty(level1Manager) && !level1Manager.Contains("liujun@"))
            {
                conditionSql += "'" + level1Manager + "',";
            }
            if (!string.IsNullOrEmpty(level2Manager) && !level2Manager.Contains("liujun@") && !level2Manager.Contains("admin"))
            {
                conditionSql += "'" + level2Manager + "',";
            }
            if (!string.IsNullOrEmpty(level3Manager) && !level3Manager.Contains("liujun@") && !level3Manager.Contains("admin"))
            {
                conditionSql += "'" + level3Manager + "',";
            }
            if (!string.IsNullOrEmpty(level4Manager) && !level4Manager.Contains("liujun@") && !level4Manager.Contains("admin"))
            {
                conditionSql += "'" + level4Manager + "',";
            }
            if (string.IsNullOrEmpty(conditionSql))
            {
                AppUtility.Engine.LogWriter.Write("FITransfer无此经销商经理数据：" + userCode + "-->instanceId=" + instanceId);
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            conditionSql = conditionSql.Substring(0, conditionSql.Length - 1);
            whereSql = string.Format(whereSql, conditionSql);
            userSql = string.Format(userSql, whereSql);

            DataTable userDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(userSql);

            var userTransfers = new List<UserTransfer>();

            //获取ot_user objectid
            if (CommonFunction.hasData(userDt))
            {
                foreach (DataRow row in userDt.Rows)
                {
                    userTransfers.Add(new UserTransfer { UserId = row["ObjectId"] + string.Empty, UserName = row["Name"] + string.Empty, UserOrgUnit = row["ParentId"] + string.Empty });
                }
            }

            if (!userTransfers.Any())
            {
                AppUtility.Engine.LogWriter.Write("FITransfer无传阅数据：" + userCode + "-->instanceId=" + instanceId);
                return Json(rtn, JsonRequestBehavior.AllowGet);
            }

            var user = AppUtility.Engine.Organization.GetUserByCode(userCode);

            //if (activityCode != "Activity2")
            //{
            //    #region FI上传附件，FI输入放款信息传阅

            //    string sql_getid = string.Format("select objectid from Ot_Workitem where instanceid='{0}'", instanceId);
            //    string workItemId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_getid) + string.Empty;

            //    AppUtility.Engine.LogWriter.Write("FITransfer传阅workItemId：" + workItemId + "-->instanceid=" + instanceId);

            //    if (string.IsNullOrEmpty(workItemId))
            //    {
            //        return Json(rtn, JsonRequestBehavior.AllowGet);
            //    }
            //    OThinker.H3.WorkItem.WorkItem workItem = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);

            //    var circulates = AppUtility.Engine.WorkItemManager.Circulate(user.ObjectID, workItem.ObjectID, userTransfers.Select(p => p.UserId).ToArray(), false, workItem.DisplayName);

            //    AppUtility.Engine.LogWriter.Write("FITransfer传阅：" + JsonConvert.SerializeObject(circulates) + "-->instanceId=" + instanceId);

            //    #endregion

            //}


            var displayName = string.Empty;
            var sheetCode = string.Empty;

            switch (activityCode)
            {
                case "Activity2":
                    displayName = "传阅:FI进件";
                    sheetCode = "APPLICATION_001_02";
                    break;
                case "Activity8":
                    displayName = "传阅:FI上传附件";
                    sheetCode = "APPLICATION_002_02";
                    break;
                case "Activity48":
                    displayName = "传阅:FI输入放款信息";
                    sheetCode = "APPLICATION_006_02";
                    break;
                default:
                    activityCode = "Activity2";
                    displayName = "传阅:FI进件";
                    sheetCode = "APPLICATION_001_02";
                    break;
            }


            #region FI进件传阅

            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            var token = instanceContext.GetTokens(activityCode, TokenState.Unspecified).OrderByDescending(p => p.CreatedTime).First();

            var circulateItems = new List<CirculateItem>();

            foreach (var userTransfer in userTransfers)
            {
                circulateItems.Add(new CirculateItem(userTransfer.UserId, userTransfer.UserName, userTransfer.UserOrgUnit, instanceContext.WorkflowCode, instanceContext.WorkflowVersion, activityCode, displayName, sheetCode, instanceId, token.TokenId, string.Empty, string.Empty, user.ObjectID, string.Empty, false, true, ActionEventType.None));
            }

            var circulateItemIds = AppUtility.Engine.WorkItemManager.AddCirculateItems(circulateItems.ToArray());

            AppUtility.Engine.LogWriter.Write("FITransfer传阅" + displayName + "circulateItemIds：" + JsonConvert.SerializeObject(circulateItemIds) + "-->instanceid=" + instanceId);

            foreach (var itemId in circulateItemIds)
            {
                var result = AppUtility.Engine.WorkItemManager.DoCirculateItem(itemId) > 0 ? true : false;

                AppUtility.Engine.LogWriter.Write("FITransfer传阅结果：" + itemId + "" + result + "-->instanceid=" + instanceId);
            }


            #endregion


            rtn.code = 1;
            rtn.data = userTransfers;

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 历史版本数据
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FIDataTrackLog(string instanceId)
        {
            rtn_data rtn = new rtn_data() { code = 1 };

            var settingValue = GetFIDataTrackSetting("APPLICATION");

            var dataTrackSettings = JsonConvert.DeserializeObject<List<FIDataTrack>>(settingValue);

            var returnResult = new List<DataTrackViewModel>();

            var trackLogDt = GetFIDataTrackLog(instanceId, "Activity2");

            if (trackLogDt == null)
            {
                rtn.data = returnResult;

                return Json(rtn, JsonRequestBehavior.AllowGet);
            }
            var schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema("APPLICATION");

            var fields = schema.Fields;

            foreach (DataRow row in trackLogDt.Rows)
            {
                var resultDic = new Dictionary<string, object>();

                var trackJson = row[0] + string.Empty; ;

                var jObject = JObject.Parse(trackJson);

                var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(trackJson);

                var mainTrackSetting = dataTrackSettings.Where(p => p.FielaName == "APPLICATION").FirstOrDefault();

                foreach (var key in dic.Keys)
                {
                    var value = jObject[key] + string.Empty;
                    var field = fields.Where(p => p.Name == key).FirstOrDefault();
                    if (jObject[key].Type == JTokenType.Array)
                    {
                        var childTrackSetting = dataTrackSettings.Where(p => p.FielaName == key).FirstOrDefault();

                        var array = JArray.Parse(value);

                        var resultChildDic = new List<Dictionary<string, object>>();

                        //每个子项List
                        for (var i = 0; i < array.Count; i++)
                        {
                            var childToken = array[i].ToString();

                            var childDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(childToken);

                            var childItemBuildDic = new Dictionary<string, object>();

                            foreach (var childDicKey in childDic.Keys)
                            {
                                var childField = field.Schema.Fields.Where(p => p.Name == childDicKey).FirstOrDefault();
                                var childItemValue = childDic[childDicKey] + string.Empty;

                                if (childTrackSetting != null)
                                {
                                    var childItemTrackSetting = childTrackSetting.FieldItem.Where(p => p.ItemFieldName == childDicKey).FirstOrDefault();

                                    if (childItemTrackSetting != null && childItemTrackSetting.ViewDisable) continue;

                                    if (childItemTrackSetting != null)
                                    {

                                        if (childItemTrackSetting.FindNameMethod == "schemacode")
                                        {
                                            var parmasValues = new List<string>();
                                            parmasValues.Add(childItemValue);
                                            if (!string.IsNullOrEmpty(childItemTrackSetting.ParentFieldName))
                                            {
                                                //parentValues.Add(childDic[childDicKey] + string.Empty;);

                                                var parentKeys = childItemTrackSetting.ParentFieldName.Split(',').ToList();

                                                foreach (var parentKey in parentKeys)
                                                {
                                                    parmasValues.Add(childDic[parentKey] + string.Empty);
                                                }

                                            }
                                            //执行sql
                                            childItemValue = GetFIDataTrackValueBySchemaCode(childItemTrackSetting.FindNameBySQL, parmasValues);
                                        }
                                        else
                                        {
                                            //执行sql
                                            childItemValue = GetFIDataTrackValueByDataCategory(childItemTrackSetting.FindNameByCode, childItemValue);
                                        }
                                    }
                                }
                                childItemBuildDic.Add(childField.DisplayName, childItemValue);
                            }
                            resultChildDic.Add(childItemBuildDic);
                        }
                        resultDic.Add(field.DisplayName, resultChildDic);
                    }
                    else
                    {
                        var mainItemTrackSetting = mainTrackSetting.FieldItem.Where(p => p.ItemFieldName == key).FirstOrDefault();
                        if (mainItemTrackSetting != null && mainItemTrackSetting.ViewDisable) continue;
                        if (mainItemTrackSetting != null)
                        {
                            if (mainItemTrackSetting.FindNameMethod == "schemacode")
                            {
                                var parmasValues = new List<string>();
                                parmasValues.Add(value);
                                if (!string.IsNullOrEmpty(mainItemTrackSetting.ParentFieldName))
                                {
                                    //parentValues.Add(childDic[childDicKey] + string.Empty;);

                                    var parentKeys = mainItemTrackSetting.ParentFieldName.Split(',').ToList();

                                    foreach (var parentKey in parentKeys)
                                    {
                                        parmasValues.Add(jObject[parentKey] + string.Empty);
                                    }

                                }
                                //执行sql
                                value = GetFIDataTrackValueBySchemaCode(mainItemTrackSetting.FindNameBySQL, parmasValues); ;
                            }
                            else
                            {
                                //执行sql
                                value = GetFIDataTrackValueByDataCategory(mainItemTrackSetting.FindNameByCode, value);
                            }
                        }
                        resultDic.Add(field.DisplayName, value);
                    }
                }

                var version = row["verson"] + string.Empty;
                var tokenId = row["tokenid"] + string.Empty;
                var createtime = row["CREATEDTIME"] + string.Empty;
                returnResult.Add(new DataTrackViewModel { DataTrack = resultDic, TokenId = tokenId, Version = version, CreateTime = createtime });
            }

            rtn.data = returnResult;

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 工联单设置参与者
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult UnionOrderOperator(string userId, string schemaCode)
        {
            AppUtility.Engine.LogWriter.Write($"工联单设置参与者：userId={userId}-->schemaCode={schemaCode}");

            rtn_data rtn = new rtn_data();

            var jobCode = "UnionOrderDepartment";
            if (schemaCode != "UnionOrder")
            {
                jobCode = "bumen";
            }

            //是否有特殊领导
            var leaderStr = AppUtility.Engine.SettingManager.GetCustomSetting("UnionOrderDepartLearder");
            if (!string.IsNullOrEmpty(leaderStr))
            {
                var leaders = JsonConvert.DeserializeObject<List<UnionOrderDepartLeaders>>(leaderStr);

                var departleader = leaders.Where(p => p.numbers.Contains(userId)).FirstOrDefault();

                if (departleader != null)
                {
                    rtn.code = 1;
                    rtn.data = new List<string>() { departleader.leader };
                    return Json(rtn, JsonRequestBehavior.AllowGet);
                }
            }

            var operators = AppUtility.Engine.Organization.GetUsersByJobCode(userId, jobCode, "");

            rtn.code = 1;
            rtn.data = operators;

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// FI输入放款信息驳回
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FKSQBack(string SchemaCode, string applicationNo, string InstanceID, string ActivityCode)
        {
            AppUtility.Engine.LogWriter.Write($"FI输入放款信息驳回：SchemaCode={SchemaCode}-->applicationNo={applicationNo}-->InstanceID={InstanceID}-->ActivityCode={ActivityCode}");

            rtn_data rtn = new rtn_data();

            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("SchemaCode", SchemaCode);
            dic.Add("applicationNo", applicationNo);
            dic.Add("InstanceID", InstanceID);
            dic.Add("ActivityCode", ActivityCode);
            var result = CommonFunction.ExecuteBizBus("CAPServiceNew", "FKSQBack", dic);

            AppUtility.Engine.LogWriter.Write($"FI输入放款信息驳回CAP转97：{JsonConvert.SerializeObject(result)}");

            if (result != null && result.Count > 0)
            {
                var issuccess = (result["Return"] + string.Empty).ToLower() == "success" ? true : false;
                if (issuccess)
                {
                    Dictionary<string, object> dic2 = new Dictionary<string, object>();
                    dic2.Add("applicationNo", applicationNo);
                    var toNewResult = CommonFunction.ExecuteBizBus("CAPServiceNew", "FKSQBackToNew", dic);
                    AppUtility.Engine.LogWriter.Write($"FI输入放款信息驳回CAP转06：{JsonConvert.SerializeObject(toNewResult)}");
                }
            }
            return Json(rtn, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 历史记录数
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult FIDataTrackLogCount(string instanceId)
        {
            rtn_data rtn = new rtn_data() { code = 1 };

            var trackLogDt = GetFIDataTrackLog(instanceId, "Activity2");

            var count = trackLogDt == null ? 0 : trackLogDt.Rows.Count;

            rtn.data = count;

            return Json(rtn, JsonRequestBehavior.AllowGet);
        }


        #region Method
        [Xss]
        public string GetPersonCategory(string app_type, string gur_type, string main_type)
        {
            if (main_type == "Y")
                return "A";
            else if (app_type == "I")
                return "B";
            else return "C";
        }
        [Xss]
        public string GetCardType(string type)
        {
            switch (type)
            {
                case "00001": return "0";
                case "00002": return "1";
                case "00003": return "2";
                case "00004": return "3";
                case "00005": return "5";
                case "00006": return "0";
                case "00007": return "6";
            }
            return "";
        }
        [Xss]
        public string GetDate(string Date)
        {
            if (Date == "")
                return "";
            string d = Convert.ToDateTime(Date).ToString("yyyy-MM-dd");
            if (d == "1753-01-01")
                return "";
            return d;
        }
        /// <summary>
        /// 是否有存量的简化贷
        /// </summary>
        /// <param name="IdCardNo"></param>
        /// <returns></returns>
        [Xss]
        public string GetJHDBySQR(string IdCardNo, string applicationno)
        {
            string sql1 = "";
            //CAP_EXPOSURE根据身份证号和申请号查敞口信息 简化贷
            sql1 += "  select sum(CT) total ,exp_applicant_card_id from ( ";
            //sql1 += " select round(ce.net_financed_amt) ct,to_char(ce.exp_application_number) exp_application_number,to_char(ce.application_status_cde) application_status_cde,to_char(ce.exp_applicant_card_id) exp_applicant_card_id,";
            //sql1 += " to_char(ce.exp_applicant_name) exp_applicant_name,to_char(fpg.fp_group_nme) fp_group_nme";
            //sql1 += " from CAP_EXPOSURE ce";
            //sql1 += " join contract_detail cd on cd.application_number=ce.application_number";
            //sql1 += " join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id";
            //sql1 += " where 1=1 ";
            //sql1 += " and ce.is_exposed_ind='T'";
            //sql1 += " and ce.exp_applicant_card_id='" + IdCardNo + "'";//'44520219800129241X'";
            //sql1 += " and ce.application_number='" + applicationno + "'";
            //sql1 += " and fpg.fp_group_id in (20,7,8,10,11,12,13,14,15,16,17,18)";
            //sql1 += " union all";
            //CMS_EXPOSURE根据身份证号和申请号查敞口信息 简化贷
            sql1 += " select  round(cme.principle_outstanding_amt) ct,to_char(cme.application_number) exp_application_number ,to_char(cme.identification_code) application_status_cde,to_char(cme.exp_applicant_card_id) exp_applicant_card_id,";
            sql1 += " to_char(cme.exp_applicant_name) exp_applicant_name,to_char(fpg.fp_group_nme) fp_group_nme";
            sql1 += " from CMS_EXPOSURE cme ";
            sql1 += " JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number";
            sql1 += " join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id ";
            sql1 += " where 1=1";
            sql1 += " and cme.application_number='" + applicationno + "'";
            sql1 += " and IS_EXPOSED_IND = 'T'";
            sql1 += " and cme.exp_applicant_card_id='" + IdCardNo + "'";//'620102196707275317'";
            sql1 += " and fpg.fp_group_id in (20,7,8,10,11,12,13,14,15,16,17,18)";
            sql1 += " ) group by exp_applicant_card_id";
            DataTable dt = DBHelper.ExecuteDataTableSql("cap", sql1);
            string rtn = "";
            if (dt != null && dt.Rows.Count > 0 && dt.Rows[0][0].ToString() != "0")
            {
                rtn = "是";
            }
            else
            {
                rtn = "否";
            }
            return rtn;
        }
        /// <summary>
        /// 根据证件号获取申请人的最新的申请单号
        /// </summary>
        /// <param name="IdCardNum"></param>
        /// <returns></returns>
        [Xss]
        public string getLastApplicationNumByIdCard(string IdCardNum)
        {
            string rtn = "";
            string sql = "select  a.application_number, a.application_date, ad.ID_CARD_NBR";
            sql += " from APPLICATION a";
            sql += " JOIN APPLICANT_DETAIL AD ON AD.APPLICATION_NUMBER = a.APPLICATION_NUMBER AND AD.IDENTIFICATION_CODE = '1'";
            sql += " Join (";
            sql += "  select count(*) cc ,ad.id_card_nbr id_card_nbr ,max( to_number(substr(a.application_number,5))) max_number ";
            sql += "  from application a";
            sql += "  JOIN APPLICANT_DETAIL AD ON AD.APPLICATION_NUMBER = a.APPLICATION_NUMBER AND AD.IDENTIFICATION_CODE = '1'";
            sql += "   where a.status_code in ('01','55','05','06') ";
            sql += "  group by ad.id_card_nbr";
            sql += " ) aa on aa.id_card_nbr = ad.id_card_nbr and aa.max_number = to_number(substr(a.application_number,5))";
            sql += " where ad.ID_CARD_NBR ='" + IdCardNum + "' order by a.submission_date desc";
            DataTable dt = DBHelper.ExecuteDataTableSql("cap", sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rtn = dt.Rows[0][0].ToString();
            }
            else
            {
                AppUtility.Engine.LogWriter.Write("敞口查询错误：通过身份证号-" + IdCardNum + "未找到最新的申请单！");
            }
            return rtn;
        }
        /// <summary>
        /// 获取所有敞口信息
        /// </summary>
        /// <param name="idcard"></param>
        /// <param name="applicationnum"></param>
        /// <returns></returns>
        [Xss]
        public string getckAll(string idcard, string applicationnum)
        {
            string rtn = "";
            string sql = "";
            sql += " select sum(cnt) cnt from (";
            sql += " select round(ce.net_financed_amt) cnt";
            sql += " from CAP_EXPOSURE ce";
            sql += " join contract_detail cd on cd.application_number=ce.application_number";
            sql += " join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id    ";
            sql += " join asset_make_code am  on am.asset_make_cde = ce.asset_make_cde     ";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = ce.asset_brand_cde   ";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde ";
            sql += " where 1=1 ";
            sql += " and ce.exp_applicant_card_id='" + idcard + "'";
            //sql+=" --and ce.is_exposed_ind='T'";
            sql += " and ce.application_number='" + applicationnum + "'";
            sql += " ";
            sql += " union all ";
            sql += " select  round(cme.principle_outstanding_amt) cnt";
            sql += " from CMS_EXPOSURE cme ";
            sql += " join request_status_code rs on rs.request_status_cde=cme.contract_status_cde";
            sql += " JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number     ";
            sql += " JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = app.BUSINESS_PARTNER_ID         ";
            sql += " join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id         ";
            sql += " join asset_make_code am  on am.asset_make_cde = cme.asset_make_cde          ";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = cme.asset_brand_cde        ";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde      ";
            sql += " where 1=1";
            sql += " and cme.application_number='" + applicationnum + "'";
            //sql+=" --and IS_EXPOSED_IND = 'T'";
            sql += " and cme.exp_applicant_card_id='" + idcard + "'";
            sql += " ";
            sql += " union all";
            sql += " ";
            sql += " select round(ce.net_financed_amt) cnt";
            sql += " from CAP_EXPOSURE ce";
            sql += " join contract_detail cd on cd.application_number=ce.application_number";
            sql += " join financial_product_group fpg on ce.fp_group_id=fpg.fp_group_id    ";
            sql += " join asset_make_code am  on am.asset_make_cde = ce.asset_make_cde     ";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = ce.asset_brand_cde   ";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = ce.asset_model_cde ";
            sql += " where 1=1";
            sql += " and ce.application_number='" + applicationnum + "'";
            sql += " and ce.identification_code in (2,3)";
            sql += " ";
            sql += " union all";
            sql += " ";
            sql += " select  round(cme.principle_outstanding_amt) cnt";
            sql += " from CMS_EXPOSURE cme";
            sql += " join request_status_code rs on rs.request_status_cde=cme.contract_status_cde";
            sql += " JOIN APPLICATION app ON app.APPLICATION_NUMBER = cme.application_number     ";
            sql += " JOIN BP_MAIN BM ON BM.BUSINESS_PARTNER_ID = app.BUSINESS_PARTNER_ID         ";
            sql += " join financial_product_group fpg on cme.fp_group_id=fpg.fp_group_id         ";
            sql += " join asset_make_code am  on am.asset_make_cde = cme.asset_make_cde          ";
            sql += " join asset_brand_code ab on ab.asset_brand_cde = cme.asset_brand_cde        ";
            sql += " join ASSET_MODEL_CODE AMC ON AMC.ASSET_MODEL_CDE = cme.asset_model_cde      ";
            sql += " where 1=1";
            sql += " and cme.application_number='" + applicationnum + "'";
            sql += " and cme.identification_code in (2,3)";
            sql += " ) a ";
            DataTable dt = DBHelper.ExecuteDataTableSql("cap", sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                rtn = dt.Rows[0][0].ToString();
            }
            return rtn;
        }
        /// <summary>
        /// 判断是否是优质经销商
        /// </summary>
        /// <param name="jxscode"></param>
        /// <returns></returns>
        [Xss]
        public string yzjxs(string jxscode)
        {
            int rtn = 0;
            string sql = string.Format("select count(1) from I_YZJXS jxs left join ot_user us on jxs.jxsname = us.appellation where us.code = '{0}'", jxscode);
            rtn = Convert.ToInt32(AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql));
            return rtn == 0 ? "否" : "是";
        }

        /// <summary>
        /// 获取经销商类型：内网还是外网
        /// </summary>
        /// <param name="serCode"></param>
        /// <returns></returns>
        [Xss]
        public string getFIType(string UserCode)
        {
            if (UserCode.IndexOf("98") == 0 || UserCode.IndexOf("80000") == 0)
                return "内网";
            else
                return "外网";
        }

        [Xss]
        public Dictionary<string, string> GetDropDownListSource(string DicKey, string ServerFunction, string DDLCode, string DDLText)
        {
            if (Cache.GetCache(DicKey) == null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var r = BizService.ExecuteBizNonQuery("DropdownListDataSource", ServerFunction, null);
                OThinker.H3.BizBus.BizService.BizStructure[] result = (OThinker.H3.BizBus.BizService.BizStructure[])r["List"];
                foreach (var biz in result)
                {
                    dic.Add(biz[DDLCode] + string.Empty, biz[DDLText] + string.Empty);
                }
                Cache.SetCache(DicKey, dic);
                return dic;
            }
            else
            {
                var r = (Dictionary<string, string>)Cache.GetCache(DicKey);
                return r;
            }
        }

        [Xss]
        public Dictionary<string, string> GetDropDownListSource(string DicKey, string ServerFunction, Dictionary<string, object> Parameters, string DDLCode, string DDLText)
        {
            if (Cache.GetCache(DicKey) == null)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                var r = BizService.ExecuteBizNonQuery("DropdownListDataSource", ServerFunction, Parameters);
                OThinker.H3.BizBus.BizService.BizStructure[] result = (OThinker.H3.BizBus.BizService.BizStructure[])r["List"];
                foreach (var biz in result)
                {
                    dic.Add(biz[DDLCode] + string.Empty, biz[DDLText] + string.Empty);
                }
                Cache.SetCache(DicKey, dic);
                return dic;
            }
            else
            {
                var r = (Dictionary<string, string>)Cache.GetCache(DicKey);
                return r;
            }
        }

        [Xss]
        public Dictionary<string, string> GetMappingCities()
        {
            if (Cache.GetCache("MAPPINGCITIES") == null)
            {
                Proposal.ProposalController p = new Proposal.ProposalController();
                var r = p.GetAllMappingCities();

                var result = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(r.Data));
                var rtn = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(result["Data"]));
                Cache.SetCache("MAPPINGCITIES", rtn);
                return rtn;
            }
            else
            {
                return (Dictionary<string, string>)Cache.GetCache("MAPPINGCITIES");
            }
        }

        [Xss]
        public Risk.Parameters_Evaluation.PersonsInfo GetPersonsInfo(string BizObjectId)
        {
            ///关系
            var dic_RelationShip = GetDropDownListSource("RELATIONSHIP", "get_relationship", "RELATIONSHIP_CDE", "RELATIONSHIP_DSC");
            if (!dic_RelationShip.ContainsKey(""))
            {
                dic_RelationShip.Add("", "OneSelf本人");
            }
            //房产类型
            var dic_Property = GetDropDownListSource("PROPERTY", "get_property_type", "PROPERTY_TYPE_CDE", "PROPERTY_TYPE_DSC");
            ///省份
            var dic_Province = GetDropDownListSource("PROVINCE", "get_state_code", "STATE_CDE", "STATE_NME");


            List<Risk.Parameters_Evaluation.Person> persons = new List<Risk.Parameters_Evaluation.Person>();
            string ifmortgage = "";

            #region 人员信息
            #region sql
            string sql = @"
select det.parentobjectid,type.identification_code1,type.applicant_type,type.guarantor_type,type.main_applicant,type.is_inactive_ind,
det.GUARANTOR_RELATIONSHIP_CDE,det.FORMER_NAME,det.FIRST_THI_NME,det.ID_CARD_TYP,det.ID_CARD_NBR,det.ID_CARDEXPIRY_DTE,det.SEX,det.AGE_IN_YEAR,det.AGE_IN_MONTH,det.DATE_OF_BIRTH,
det.RACE_CDE,det.NATION_CDE,det.EDUCATION_CDE,edu.enumvalue EDUCATION_NME,det.MARITAL_STATUS_CDE,marital.enumvalue MARITAL_STATUS_NME,det.ACTUAL_SALARY,det.LIENEE 
from i_Applicant_Detail det 
left join i_applicant_type type on det.parentobjectid=type.parentobjectid and det.identification_code2=type.identification_code1
left join ot_enumerablemetadata edu on det.education_cde=edu.code and edu.category='教育程度' 
left join ot_enumerablemetadata marital on det.MARITAL_STATUS_CDE=marital.code and marital.category='婚姻状况' 
where det.parentobjectid='{0}' 
order by type.identification_code1
";
            #endregion
            sql = string.Format(sql, BizObjectId);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


            foreach (DataRow row in dt.Rows)
            {
                if (row["main_applicant"] + string.Empty == "Y")
                {
                    if (row["LIENEE"] + string.Empty == "1")
                    {
                        ifmortgage = "是";
                    }
                    else
                    {
                        ifmortgage = "否";
                    }
                }

                var phoneNumber = GetPersonPhone(BizObjectId, row["identification_code1"] + string.Empty);

                var addressInfo = GetPersonAddress(BizObjectId, row["identification_code1"] + string.Empty);

                var employeeInfo = GetPersonEmployer(BizObjectId, row["identification_code1"] + string.Empty);

                Risk.Parameters_Evaluation.Person person = new Risk.Parameters_Evaluation.Person();
                person.personnelCategory = GetPersonCategory(row["applicant_type"] + string.Empty, row["guarantor_type"] + string.Empty, row["main_applicant"] + string.Empty);
                person.personnelRelationship = dic_RelationShip[row["GUARANTOR_RELATIONSHIP_CDE"] + string.Empty];
                person.formerName = row["FORMER_NAME"] + string.Empty;
                person.custName = row["FIRST_THI_NME"] + string.Empty;
                person.certType = GetCardType(row["ID_CARD_TYP"] + string.Empty);
                person.certNo = row["ID_CARD_NBR"] + string.Empty;
                person.certEndDate = GetDate(row["ID_CARDEXPIRY_DTE"] + string.Empty);
                person.mobile = phoneNumber + string.Empty;
                person.gender = row["SEX"] + string.Empty == "F" ? "女" : "男";
                person.birthdayYear = row["AGE_IN_YEAR"] + string.Empty;
                person.birthdayMonth = row["AGE_IN_MONTH"] + string.Empty;
                person.custBirthday = GetDate(row["DATE_OF_BIRTH"] + string.Empty);
                person.nationality = row["RACE_CDE"] + string.Empty == "00001" ? "中国 Chinese" : "其他 Other";
                person.nation = row["NATION_CDE"] + string.Empty == "0001" ? "汉族" : "其他";
                person.educationLevel = row["EDUCATION_NME"] + string.Empty;
                person.maritalStatus = row["MARITAL_STATUS_NME"] + string.Empty;
                person.houseStatus = dic_Property[addressInfo["PROPERTY_TYPE_CDE"] + string.Empty];
                person.employer = employeeInfo["NAME_2"] + string.Empty;
                person.employerTelphone = employeeInfo["PHONE"] + string.Empty;
                //检查到此
                //公司省份
                string emp_province_code = employeeInfo["STATE_CDE2"] + string.Empty;
                ///城市
                Dictionary<string, object> emp_Para = new Dictionary<string, object>();
                emp_Para.Add("state_cde", emp_province_code);
                var dic_Emp_City = GetDropDownListSource("PROVINCE_" + emp_province_code, "get_city_all", emp_Para, "CITY_CDE", "CITY_NME");
                //地址省份
                string ads_province_code = addressInfo["state_cde4"] + string.Empty;
                Dictionary<string, object> ads_Para = new Dictionary<string, object>();
                ads_Para.Add("state_cde", ads_province_code);
                var dic_Ads_City = GetDropDownListSource("PROVINCE_" + ads_province_code, "get_city_all", ads_Para, "CITY_CDE", "CITY_NME");
                person.employerAddressProvince = dic_Province[emp_province_code];
                person.employerAddressCity = GetMappingCity(dic_Emp_City[employeeInfo["CITY_CDE6"] + string.Empty]);
                person.employerAddress = employeeInfo["ADDRESS_ONE_2"] + string.Empty;
                person.occupation = employeeInfo["DESIGNATION_NME"] + string.Empty;
                person.workingYears = employeeInfo["TIME_IN_YEAR_2"] + string.Empty;
                person.incomeMonth = row["ACTUAL_SALARY"] + string.Empty;
                person.birthPalaceProvince = addressInfo["BIRTHPLEASE_PROVINCE"] + string.Empty;
                person.nativeDistrict = addressInfo["NATIVE_DISTRICT"] + string.Empty;
                person.domicilePlace = dic_Province[addressInfo["state_cde4"] + string.Empty] + GetMappingCity(dic_Ads_City[addressInfo["city_cde4"] + string.Empty]);//省+市
                person.currentAddressProvince = dic_Province[addressInfo["currentstate"] + string.Empty];
                person.currentAddressCity = GetMappingCity(addressInfo["currentcity"] + string.Empty);
                person.currentLivingAddress = addressInfo["UNIT_NO"] + string.Empty;
                person.isInvokePboc = row["IS_INACTIVE_IND"] + string.Empty == "T" ? "0" : "1";
                person.lienee = row["LIENEE"] + string.Empty;
                persons.Add(person);
            }
            #endregion
            return new Risk.Parameters_Evaluation.PersonsInfo() { Persons = persons, Ifmortgage = ifmortgage };
        }

        /// <summary>
        /// 增加需要调用东正风控的用户
        /// </summary>
        /// <returns></returns>
        [Xss]
        public int InsertFkFenDanUser(string loginName)
        {
            string sql = "insert into H3.C_FKFENDANUSER(objectid,usercode,appellation) values ('" + Guid.NewGuid() + "',    '" + loginName + "',    '')";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }

        /// <summary>
        /// 删除需要调用东正风控的用户
        /// </summary>
        /// <returns></returns>
        [Xss]
        public int DeleteFkFenDanUser(string loginName)
        {
            string sql = "delete from H3.C_FKFENDANUSER where usercode='" + loginName + "'";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
        }


        /// <summary>
        /// 查询需要调用东正风控的用户
        /// </summary>
        /// <returns></returns>
        [Xss]
        public List<C_FkFenDanUser> GetAllFkFenDanUser()
        {
            string sql = "select objectid,usercode,appellation from H3.C_FKFENDANUSER";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            List<C_FkFenDanUser> fenDanUsers = new List<C_FkFenDanUser>();

            foreach (DataRow row in dt.Rows)
            {
                C_FkFenDanUser fenDanUser = new C_FkFenDanUser();
                fenDanUser.ObjectId = row[0].ToString();
                fenDanUser.UserCode = row[1].ToString();
                fenDanUser.Appellation = row[2].ToString();
                fenDanUsers.Add(fenDanUser);
            }
            return fenDanUsers;
        }

        /// <summary>
        /// 查询风控报告Id
        /// </summary>
        /// <returns></returns>
        [Xss]
        public Dictionary<string, string> GetFkReportId(string instanceId)
        {
            //var sql = "select ap.FK_Report,ap.FK_RECORDID from h3.ot_instancecontext ot join h3.i_application ap on ot.bizobjectid = ap.objectid where ot.objectid = '"+ instanceId + "' ";
            string sql = "select ap.FK_Report,ap.FK_RECORDID from h3.ot_instancecontext ot join h3.i_application ap on ot.bizobjectid = ap.objectid where ot.objectid ='" + instanceId + "'";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var report = string.Empty;
            var recordId = string.Empty;

            var dic = new Dictionary<string, string>();

            if (dt.Rows.Count > 0)
            {
                report = dt.Rows[0][0].ToString();
                recordId = dt.Rows[0][1].ToString();
            }
            dic.Add("report", report);
            dic.Add("recordId", recordId);
            return dic;
        }

        /// <summary>
        /// 获取风控报告
        /// </summary>
        /// <returns></returns>
        [Xss]
        public string GetFkReport(string instanceId, string sourceType, string recordId = "")
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            string sql = "select fk_report from h3.C_APPLICATION_CLOB where sourcetype='" + sourceType + "' and  objectid ='" + instanceContext.BizObjectId + "'";

            if (!string.IsNullOrEmpty(recordId))
            {
                sql += " and recordid='" + recordId + "'";
            }

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var report = string.Empty;

            if (dt.Rows.Count > 0)
            {
                report = dt.Rows[0][0].ToString();
            }
            return report;
        }

        /// <summary>
        /// 查询风控报告
        /// </summary>
        /// <returns></returns>
        [Xss]
        public int SaveFkReport(string fkReport, string instanceId, string sourceType, string recordId)
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            //string sql = "update H3.i_application set FK_REPORT='" + fkReport + "' where objectid ='" + instanceContext.BizObjectId + "'";

            var i = 0;

            string sql = "insert into H3.C_APPLICATION_CLOB(fk_report,objectid,sourcetype,recordid) values(:content,'" + instanceContext.BizObjectId + "','" + sourceType + "','" + recordId + "')";
            try
            {
                string connectionCode = "Engine";
                var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                OracleConnection connection = new OracleConnection(dbObject.DbConnectionString);
                connection.Open();
                OracleCommand Cmd = new OracleCommand(sql, connection);
                OracleParameter Temp = new OracleParameter("content", OracleType.NClob);
                Temp.Direction = ParameterDirection.Input;
                Temp.Value = fkReport;
                Cmd.Parameters.Add(Temp);
                i = Cmd.ExecuteNonQuery();
                connection.Close();
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("保存风控报告数据异常：" + ex.ToString());
            }
            return i;

        }

        /// <summary>
        /// 删除查询风控报告
        /// </summary>
        /// <returns></returns>
        [Xss]
        public int DelFkReport(string instanceId)
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            string sql = "delete from H3.C_APPLICATION_CLOB where objectid='" + instanceContext.BizObjectId + "'";
            return OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);

        }

        /// <summary>
        /// 获取分单流程走向
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        [Xss]
        public Dictionary<string, string> GetFenDanProcess(string objectId)
        {
            var sql = "select FK_SYS_TYPE, fkresult from i_application where objectid = '" + objectId + "'";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var fkSysType = string.Empty;

            var fkresult = string.Empty;

            var dic = new Dictionary<string, string>();


            if (dt.Rows.Count > 0)
            {
                fkSysType = dt.Rows[0][0].ToString();
                fkresult = dt.Rows[0][1].ToString();
            }

            dic.Add("fkSysType", fkSysType);
            dic.Add("fkresult", fkresult);
            return dic;
        }

        /// <summary>
        /// 获取预审批银行卡号
        /// </summary>
        /// <param name="name"></param>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        [Xss]
        public Dictionary<string, string> YSP_BankCard(string name, string idCardNo)
        {

            var bankcard1 = string.Empty;
            var bankcard2 = string.Empty;
            var phone = string.Empty;

            var dic = new Dictionary<string, string>();

            #region sql获取预审批数据

            //var sql = "select * from ( select sys_phone,sys_bankcardnumber,sys_bankcardnumber2 from i_ysp where sys_pass='1' and sys_enable='1' and sys_xm='{0}' and sys_idcard='{1}'  order by createdtime desc) where rownum=1";


            //DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format(sql, name, idCardNo));

            //if (dt.Rows.Count > 0)
            //{
            //    bankcard1 = dt.Rows[0]["sys_bankcardnumber"].ToString();
            //    bankcard2 = dt.Rows[0]["sys_bankcardnumber2"].ToString();
            //    phone = dt.Rows[0]["sys_phone"].ToString();
            //}

            #endregion

            PreApproval preApproval = new PreApproval();
            var result = preApproval.QueryPreApprovalResult(name, idCardNo);

            if (result != null)
            {
                bankcard1 = result.firstBankCard;
                bankcard2 = result.secondBankCard;
                phone = result.mobiePhone;
            }

            dic.Add("bankcard1", bankcard1);
            dic.Add("bankcard2", bankcard2);
            dic.Add("phone", phone);

            return dic;
        }


        /// <summary>
        /// 获取映射城市
        /// </summary>
        /// <returns></returns>
        [Xss]
        public string GetMappingCity(string dicName)
        {
            try
            {
                ///获取Mapping的城市；
                var mapping_Cities = GetMappingCities();

                if (mapping_Cities.ContainsKey(dicName))
                {
                    return mapping_Cities[dicName].ToString();
                }
                else
                {
                    return dicName;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("获取CMS映射城市错误,dicName=" + dicName + "-->" + ex.ToString());

                return string.Empty;
            }
        }

        /// <summary>
        /// 获取人员手机号
        /// </summary>
        /// <param name="bizObjectId"></param>
        /// <param name="identificationCode"></param>
        /// <returns></returns>
        [Xss]
        public string GetPersonPhone(string bizObjectId, string identificationCode)
        {

            var phoneSql = @"select tel.PHONE_NUMBER from i_Applicant_Detail det 
            left join i_applicant_phone_fax tel on det.parentobjectid=tel.parentobjectid  and tel.phone_type_cde='00003' and tel.identification_code5 = det.identification_code2 where det.parentobjectid='{0}' and det.identification_code2='{1}'  ";
            phoneSql = string.Format(phoneSql, bizObjectId, identificationCode);
            DataTable phoneDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(phoneSql);

            var phoneNumber = string.Empty;
            if (CommonFunction.hasData(phoneDt))
            {
                phoneNumber = phoneDt.Rows[0][0].ToString();
            }

            return phoneNumber;
        }

        /// <summary>
        /// 获取人员地址
        /// </summary>
        /// <returns></returns>
        [Xss]
        public Dictionary<string, string> GetPersonAddress(string bizObjectId, string identificationCode)
        {
            var addressSql = @"select  ads.PROPERTY_TYPE_CDE,ads.BIRTHPLEASE_PROVINCE,ads.NATIVE_DISTRICT,ads.UNIT_NO,ads.state_cde4,ads.city_cde4,ads.currentstate,ads.currentcity from i_Applicant_Detail det 
              left join i_address ads on ads.parentobjectid=det.parentobjectid and ads.identification_code4=det.identification_code2 where  det.parentobjectid='{0}' and det.identification_code2='{1}' ";

            addressSql = string.Format(addressSql, bizObjectId, identificationCode);
            DataTable addressDt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(addressSql);

            var addressInfo = new Dictionary<string, string>();

            if (CommonFunction.hasData(addressDt))
            {
                addressInfo.Add("PROPERTY_TYPE_CDE", addressDt.Rows[0][0].ToString());
                addressInfo.Add("BIRTHPLEASE_PROVINCE", addressDt.Rows[0][1].ToString());
                addressInfo.Add("NATIVE_DISTRICT", addressDt.Rows[0][2].ToString());
                addressInfo.Add("UNIT_NO", addressDt.Rows[0][3].ToString());
                addressInfo.Add("state_cde4", addressDt.Rows[0][4].ToString());
                addressInfo.Add("city_cde4", addressDt.Rows[0][5].ToString());
                addressInfo.Add("currentstate", addressDt.Rows[0][6].ToString());
                addressInfo.Add("currentcity", addressDt.Rows[0][7].ToString());
            }

            return addressInfo;
        }

        /// <summary>
        /// 获取工作信息
        /// </summary>
        /// <param name="bizObjectId"></param>
        /// <param name="identificationCode"></param>
        /// <returns></returns>
        [Xss]
        public Dictionary<string, string> GetPersonEmployer(string bizObjectId, string identificationCode)
        {
            var sql = @"select emp.NAME_2,emp.ADDRESS_ONE_2,emp.PHONE,emp.STATE_CDE2,emp.CITY_CDE6,emp.DESIGNATION_CDE,jobs.enumvalue  DESIGNATION_NME,emp.TIME_IN_YEAR_2 from i_Applicant_Detail det 
              left join i_employer emp on emp.parentobjectid=det.parentobjectid and emp.identification_code6=det.identification_code2 
              left join ot_enumerablemetadata jobs on emp.DESIGNATION_CDE=jobs.code and jobs.category='职位' where  det.parentobjectid='{0}' and det.identification_code2='{1}' ";

            sql = string.Format(sql, bizObjectId, identificationCode);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var employeeInfo = new Dictionary<string, string>();

            if (CommonFunction.hasData(dt))
            {
                employeeInfo.Add("NAME_2", dt.Rows[0][0].ToString());
                employeeInfo.Add("ADDRESS_ONE_2", dt.Rows[0][1].ToString());
                employeeInfo.Add("PHONE", dt.Rows[0][2].ToString());
                employeeInfo.Add("STATE_CDE2", dt.Rows[0][3].ToString());
                employeeInfo.Add("CITY_CDE6", dt.Rows[0][4].ToString());
                employeeInfo.Add("DESIGNATION_CDE", dt.Rows[0][5].ToString());
                employeeInfo.Add("DESIGNATION_NME", dt.Rows[0][6].ToString());
                employeeInfo.Add("TIME_IN_YEAR_2", dt.Rows[0][7].ToString());
            }

            return employeeInfo;
        }

        [Xss]
        public DataTable GetApplicationByAppNumber(string appNumber)
        {
            var sql = @"select * from i_Application where application_number='{0}' ";

            sql = string.Format(sql, appNumber);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            if (CommonFunction.hasData(dt))
            {
                return dt;
            }

            return null;
        }


        /// <summary>
        /// 保存面签结果
        /// </summary>
        /// <param name="detail"></param>
        [Xss]
        public void InsertFaceSignResult(List<FaceSignBizRelDetails> details, string bizObjectId, string applicationNumber, int retryNo, string faceSignNo, string commonResult)
        {

            foreach (var detail in details)
            {
                var sql = @"insert into C_FACESIGNRECORD   (objectid,bizobjectid,applicationnumber,applicanttype,name,idnumber,facesignresult,resons,facesignno,retrynoindex,mobile,liveaddr,result) values('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}','{11}','{12}')";

                sql = string.Format(sql, Guid.NewGuid().ToString(), bizObjectId, applicationNumber, detail.relTyp, detail.name, detail.idNo, commonResult, detail.reasons, faceSignNo, retryNo, detail.mobile, detail.liveAddr, detail.bizSts);

                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql.ToString());
            }
        }

        /// <summary>
        /// 获取面签信息
        /// </summary>
        /// <param name="bizObjectId"></param>
        /// <returns></returns>
        [Xss]
        public List<FaceSignResultResponse> GetFaceSignResult(string bizObjectId)
        {
            var sql = @"select * from  h3.C_FACESIGNRECORD where bizobjectid='{0}' ";

            sql = string.Format(sql, bizObjectId);
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var results = new List<FaceSignResultResponse>();

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    var resultResponse = new FaceSignResultResponse();
                    resultResponse.Objectid = row["objectid"] + string.Empty;
                    resultResponse.BizObjectId = row["bizobjectid"] + string.Empty;
                    resultResponse.ApplicationNumber = row["applicationnumber"] + string.Empty;
                    resultResponse.ApplicantType = row["applicanttype"] + string.Empty;
                    resultResponse.Name = row["name"] + string.Empty;
                    resultResponse.IdNumber = row["idnumber"] + string.Empty;
                    resultResponse.FaceSignResult = row["facesignresult"] + string.Empty;
                    resultResponse.Resons = row["resons"] + string.Empty;
                    resultResponse.CreatedTime = Convert.ToDateTime(row["createdtime"] + string.Empty);
                    resultResponse.FaceSignNo = row["facesignno"] + string.Empty;
                    resultResponse.RetryNoIndex = Convert.ToInt32(row["retrynoindex"] + string.Empty);
                    resultResponse.Mobile = row["mobile"] + string.Empty;
                    resultResponse.LiveAddress = row["liveaddr"] + string.Empty;
                    results.Add(resultResponse);
                }
            }

            return results;
        }

        /// <summary>
        /// 获取历史留痕数据
        /// </summary>
        /// <returns></returns>
        public DataTable GetFIDataTrackLog(string instanceId, string activityCode)
        {
            var instanceContext = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);

            string sql = "select datatrack,verson,tokenid,createdtime from h3.c_fidatatrack where instanceid='" + instanceId + "' and  activitycode ='" + activityCode + "'";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var report = string.Empty;

            if (CommonFunction.hasData(dt))
            {
                return dt;
            }

            return null;
        }

        /// <summary>
        /// 获取数据留痕配置
        /// </summary>
        /// <returns></returns>
        public string GetFIDataTrackSetting(string fieldName)
        {
            string sql = "select settingvalue from h3.c_fidatatracksetting where fieldname='" + fieldName + "'";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            var report = string.Empty;

            if (CommonFunction.hasData(dt))
            {
                return dt.Rows[0][0] + string.Empty;
            }

            return null;
        }

        /// <summary>
        /// 根据SchemaCode查询数据值
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public string GetFIDataTrackValueBySchemaCode(string sql, List<string> parentValues)
        {
            var querySql = string.Format(sql, parentValues.ToArray());

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(querySql);

            if (CommonFunction.hasData(dt))
            {
                return dt.Rows[0][0] + string.Empty;
            }

            return string.Empty;
        }

        /// <summary>
        /// 根据masterdatacategory查询数据值
        /// </summary>
        /// <returns></returns>
        public string GetFIDataTrackValueByDataCategory(string category, string code)
        {
            List<OT_EnumerableMetaData> enumDatas = new List<OT_EnumerableMetaData>();
            if (Cache.GetCache("masterdatacategory") == null)
            {
                var meatDatas = GetAllDataCategory();
                if (meatDatas.Any())
                {
                    enumDatas = meatDatas;
                    Cache.SetCache("masterdatacategory", JsonConvert.SerializeObject(meatDatas), TimeSpan.FromDays(1));
                }

            }
            else
            {
                var cacheValue = Cache.GetCache("masterdatacategory").ToString();

                enumDatas = JsonConvert.DeserializeObject<List<OT_EnumerableMetaData>>(cacheValue);
            }

            if (!enumDatas.Any())
            {
                return string.Empty;
            }
            var enumData = enumDatas.Where(p => p.Category == category && p.Code == code).FirstOrDefault();

            return enumData == null ? string.Empty : enumData.EnumValue;

        }

        /// <summary>
        /// 获取所有字典表数据
        /// </summary>
        public List<OT_EnumerableMetaData> GetAllDataCategory()
        {
            List<OT_EnumerableMetaData> datas = new List<OT_EnumerableMetaData>();

            var querySql = "select * from ot_enumerablemetadata";

            var dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(querySql);

            if (CommonFunction.hasData(dt))
            {
                foreach (DataRow row in dt.Rows)
                {
                    var metaData = new OT_EnumerableMetaData();
                    metaData.Category = row["Category"] + string.Empty;
                    metaData.Code = row["Code"] + string.Empty;
                    metaData.EnumValue = row["EnumValue"] + string.Empty;
                    datas.Add(metaData);
                }
            }
            return datas;
        }

        #endregion

        public class UserTransfer
        {
            public string UserName { get; set; }

            public string UserId { get; set; }

            public string UserOrgUnit { get; set; }
        }

        public class FIDataTrack
        {
            public string FielaName { get; set; }

            public List<FIDataTrackItem> FieldItem { get; set; }
        }

        public class FIDataTrackItem
        {
            public string ItemFieldName { get; set; }
            public string FindNameMethod { get; set; }
            public string FindNameBySQL { get; set; }
            public string FindNameByCode { get; set; }

            /// <summary>
            /// 能否展示 0展示  1不展示
            /// </summary>
            public bool ViewDisable { get; set; }

            public string ParentFieldName { get; set; }
        }

        public class DataTrackViewModel
        {
            public string Version { get; set; }

            public string TokenId { get; set; }

            public string CreateTime { get; set; }

            public Dictionary<string, object> DataTrack { get; set; }
        }


        /// <summary>
        /// 工联单特殊人员领导
        /// </summary>
        public class UnionOrderDepartLeaders
        {
            public string leader { get; set; }

            public List<string> numbers { get; set; }
        }

        public class ContractRate
        {
            public string RateId { get; set; }

            public string Duration { get; set; }

            public decimal RateBase { get; set; }

            public string EnabledTime { get; set; }
        }

    }
}