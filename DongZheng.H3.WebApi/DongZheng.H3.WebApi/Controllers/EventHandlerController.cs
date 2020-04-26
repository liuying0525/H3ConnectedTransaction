using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using OThinker.Organization;
using OThinker.H3.WorkItem;
using OThinker.H3.Instance;
using System.Data;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections.Specialized;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class EventHandlerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        //
        // GET: /EventHandler/

        public string Index()
        {
            return "EventHandler 服务";
        }

        [HttpGet]
        public JsonResult OnCreated(string id)
        {
            AppUtility.Engine.LogWriter.Write("WebApi OnCreated...workitemID-->" + id);
            WorkItem WorkItem = AppUtility.Engine.WorkItemManager.GetWorkItem(id);
            if (WorkItem == null)
                return Json(new { Success = false, Message = "workitemID错误" }, JsonRequestBehavior.AllowGet);
            if (WorkItem.ItemType == WorkItemType.Approve)
            {
                string conditions = "";
                if (WorkItem.WorkflowCode == "RetailApp" || WorkItem.WorkflowCode == "CompanyApp" ||
                    WorkItem.WorkflowCode == "APPLICATION")
                {
                    //#region 待办任务推送CRM
                    ////if (WorkItem.ActivityCode != "Activity2")
                    ////{  
                    ////}
                    //AppUtility.Engine.LogWriter.Write("待办任务推送开始：WorkItemID：" + id);
                    //Dictionary<string, object> dic = new Dictionary<string, object>();
                    //dic.Add("SchemaCode", WorkItem.WorkflowCode + string.Empty);
                    //dic.Add("workitemID", id + string.Empty);
                    //try
                    //{
                    //    BizService.ExecuteBizNonQuery("CRMService", "postUnFinishedWorkItemInfo", dic);
                    //}
                    //catch (Exception ex)
                    //{
                    //    AppUtility.Engine.LogWriter.Write("待办任务推送异常：WorkItemID：" + id+"，message："+ex.Message);
                    //}
                    //AppUtility.Engine.LogWriter.Write("待办任务推送结束：WorkItemID：" + id);
                    //#endregion
                    //运营二个节点
                    if (WorkItem.ActivityCode == "Activity17" || WorkItem.ActivityCode == "Activity18")
                    {
                        conditions += " and (activitycode='Activity17' or activitycode='Activity18')";
                    }
                    //信审二个节点
                    else if (WorkItem.ActivityCode == "Activity14" || WorkItem.ActivityCode == "Activity13")
                    {
                        conditions += " and (activitycode='Activity14' or activitycode='Activity13')";
                    }
                }
                #region 判断当前任务是否是有驳回过
                if (WorkItem.DisplayName.Contains("信审") || WorkItem.DisplayName.Contains("运营人员"))
                {
                    string sql = "select count(1) from Ot_Workitemfinished where instanceid='{0}' {1} and actioneventtype=3";
                    sql = string.Format(sql, WorkItem.InstanceId, conditions);
                    AppUtility.Engine.LogWriter.Write("判断是否有驳回,SQL-->" + sql);
                    int num = Convert.ToInt32(AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql));
                    AppUtility.Engine.LogWriter.Write("当前任务-->" + WorkItem.WorkItemID + "对应的单据之前有驳回过" + num + "次");
                    if (num > 0)
                    {
                        long ret = AppUtility.Engine.WorkItemManager.UpdatePriority(WorkItem.WorkItemID, PriorityType.High);
                        AppUtility.Engine.LogWriter.Write("已驳回的单据，设置任务优先级为最高：WorkItemID-->" + WorkItem.WorkItemID + ",Result-->" + ret);
                    }
                }
                #endregion

                #region 推送接口中心，回调信息
                if (WorkItem.WorkflowCode == "APPLICATION")
                {
                    #region FI进件节点
                    if (WorkItem.ItemType == WorkItemType.Fill)
                    {
                        if (WorkItem.PreActionEventType == ActionEventType.Backward)
                        {
                            var context = AppUtility.Engine.InstanceManager.GetInstanceContext(WorkItem.InstanceId);
                            var activity = context.GetToken(WorkItem.TokenId - 1);
                            if (activity.Activity == "Activity13")//信审终审驳回；
                            {
                                InstanceData data = new InstanceData(AppUtility.Engine, WorkItem.InstanceId, "");
                                string uuid = data["uuid"].Value + string.Empty;
                                if (!string.IsNullOrEmpty(uuid))
                                {
                                    ApprovalResult para = new ApprovalResult();
                                    para.uuid = uuid;
                                    para.type = "01";
                                    para.status = "005";
                                    para.messages = GetAllMsg(WorkItem.InstanceId);

                                    NameValueCollection nvBody = new NameValueCollection();
                                    nvBody.Add("Uuid", para.uuid);
                                    nvBody.Add("CallbackType", "2");
                                    nvBody.Add("CallbackParameters", JsonConvert.SerializeObject(para));

                                    AppUtility.Engine.LogWriter.Write("推送消息给接口中心结果[Created FI Formdata]：" + HttpHelper.postFormData(ConfigurationManager.AppSettings["apiAddMQMsg"] + string.Empty, new NameValueCollection(), nvBody));
                                }
                            }
                        }
                    }
                    #endregion

                    #region FI输入放款信息
                    else if (WorkItem.ActivityCode == "Activity48")
                    {
                        InstanceData data = new InstanceData(AppUtility.Engine, WorkItem.InstanceId, "");
                        //FI输入放款信息且是自动审批的，需要回调；
                        if (data["Automatic_approval"].Value + string.Empty == "1")
                        {
                            string uuid = data["uuid"].Value + string.Empty;
                            if (!string.IsNullOrEmpty(uuid))
                            {
                                ApprovalResult para = new ApprovalResult();
                                para.uuid = uuid;
                                para.type = "01";
                                para.status = "001";
                                para.messages = GetAllMsg(WorkItem.InstanceId);

                                NameValueCollection nvBody = new NameValueCollection();
                                nvBody.Add("Uuid", para.uuid);
                                nvBody.Add("CallbackType", "2");
                                nvBody.Add("CallbackParameters", JsonConvert.SerializeObject(para));

                                AppUtility.Engine.LogWriter.Write("推送消息给接口中心结果[Created 放款 Formdata]：" + HttpHelper.postFormData(ConfigurationManager.AppSettings["apiAddMQMsg"] + string.Empty, new NameValueCollection(), nvBody));
                            }
                        }
                    }
                    #endregion
                }
                #endregion
            }

            return Json(new { Success = true, Message = "" }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult OnRemoved(string id)
        {
            try
            {
                AppUtility.Engine.LogWriter.Write("OnRemoved-->" + id);
                WorkItem WorkItem = AppUtility.Engine.WorkItemManager.GetWorkItem(id);
                if (WorkItem == null)
                {
                    //流程实例删除，会执行到这里
                    int n_del = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM OT_WorkItemFinishedDistinct WHERE instanceid='" + id + "'");
                    AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct 数据删除(原任务被删除) ObjectID-->" + id + "，受影响行数：" + n_del);
                    return Json(new { Success = true, Message = "workitemID错误" }, JsonRequestBehavior.AllowGet);
                }

                AppUtility.Engine.LogWriter.Write("EventHandler Remove " + WorkItem.ObjectID + ",State:" + WorkItem.State.ToString() + ",ItemType:" + WorkItem.ItemType.ToString());

                InstanceData data = new InstanceData(AppUtility.Engine, WorkItem.InstanceId, "");

                #region 插入到新的已办任务表中，需要去重：同一个InstanceID，同一个ActivityCode，同一个人只能有一条最新的单据；
                //1.去重
                string str_SQL = "SELECT objectid FROM OT_WorkItemFinishedDistinct WHERE InstanceId='{0}' AND ActivityCode='{1}' AND (Participant='{2}' OR Finisher='{2}')";
                str_SQL = string.Format(str_SQL, WorkItem.InstanceId, WorkItem.ActivityCode, WorkItem.Finisher);
                string obj = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(str_SQL) + string.Empty;
                if (!string.IsNullOrEmpty(obj))//有重复的，需要进行删除；
                {
                    int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM OT_WorkItemFinishedDistinct WHERE objectid='" + obj + "'");
                    AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct 重复数据删除 ObjectID-->" + obj + "，受影响行数：" + n);
                }
                //2.插入到Distinct表
                int n_Insert = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("INSERT INTO OT_WorkItemFinishedDistinct SELECT * FROM OT_WorkItemFinished WHERE ObjectID='" + WorkItem.ObjectID + "'");
                AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct Insert ObjectID-->" + WorkItem.ObjectID);
                #endregion

                #region 特定流程插入到流程日志操作表中，记录操作情况；
                if (WorkItem.State == WorkItemState.Finished)
                {
                    try
                    {
                        #region //获取活动节点及字段编码：2018-01-19
                        List<InstanceLogRule> listActivityAndField = new List<InstanceLogRule>();
                        string sql = "SELECT ActivityAndField FROM I_Para_Operation WHERE FlowCode='" + WorkItem.WorkflowCode + "'";
                        string activityAndField = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                        if (!string.IsNullOrEmpty(activityAndField.Trim()))
                        {
                            try
                            {
                                var pair = activityAndField.Split('|');
                                foreach (string kv in pair)
                                {
                                    InstanceLogRule rule = new InstanceLogRule();
                                    var v = kv.Split(';');
                                    rule.ActivityCode = v[0];
                                    rule.DataField = v[1];
                                    rule.ValJson = JsonConvert.DeserializeObject<Dictionary<string, string>>(v[2]);
                                    listActivityAndField.Add(rule);
                                }
                            }
                            catch (Exception ex)
                            {
                                AppUtility.Engine.LogWriter.Write("解析流程操作设置失败：" + ex.ToString());
                            }
                        }
                        #endregion
                        if (listActivityAndField.Count > 0)
                        {
                            #region 插入流程日志的操作名称到数据表
                            foreach (var rule in listActivityAndField)
                            {
                                if (rule.ActivityCode == WorkItem.ActivityCode.ToString())
                                {
                                    AppUtility.Engine.LogWriter.Write("Insert。。。");
                                    InsertToTable(data, WorkItem, rule);
                                }
                            }
                            #endregion
                        }
                    }
                    catch (Exception ex)
                    {
                        AppUtility.Engine.LogWriter.Write("插入到流程日志表异常：" + ex.ToString());
                    }
                }
                #endregion

                if (WorkItem.WorkflowCode == "RetailApp" || WorkItem.WorkflowCode == "CompanyApp" || WorkItem.WorkflowCode == "APPLICATION")
                {
                    //#region 已办任务推送CRM
                    ////if (WorkItem.ActivityCode != "Activity2")
                    ////{
                    ////}
                    //AppUtility.Engine.LogWriter.Write("已办任务推送开始：WorkItemID：" + id);
                    //Dictionary<string, object> dic = new Dictionary<string, object>();
                    //dic.Add("SchemaCode", WorkItem.WorkflowCode + string.Empty);
                    //dic.Add("activity", WorkItem.ActivityCode + string.Empty);
                    //dic.Add("workitemID", id + string.Empty);
                    //BizService.ExecuteBizNonQuery("CRMService", "postFinishedWorkItemInfo", dic);
                    //AppUtility.Engine.LogWriter.Write("已办任务推送结束：WorkItemID：" + id);
                    //#endregion
                }

                #region 推送接口中心，回调信息
                if (WorkItem.WorkflowCode == "APPLICATION")
                {
                    string uuid = data["uuid"].Value + string.Empty;
                    if (!string.IsNullOrEmpty(uuid))
                    {
                        var para = GetApprovalResult(data, WorkItem);
                        if (para != null)
                        {
                            NameValueCollection nvBody = new NameValueCollection();
                            nvBody.Add("Uuid", para.uuid);
                            nvBody.Add("CallbackType", "2");
                            nvBody.Add("CallbackParameters", JsonConvert.SerializeObject(para));

                            AppUtility.Engine.LogWriter.Write("推送消息给接口中心结果[Removed Formdata]：" + HttpHelper.postFormData(ConfigurationManager.AppSettings["apiAddMQMsg"] + string.Empty, new NameValueCollection(), nvBody));
                        }
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("EventHandler Remove Exception-->" + ex.ToString());
            }
            return Json(new { Success = true, Message = "Success" }, JsonRequestBehavior.AllowGet);
        }

        #region Method
        private string GetOpeName(InstanceData data, WorkItem wi, InstanceLogRule rule)
        {
            string opeName = "";
            try
            {
                string value = data[rule.DataField].Value + string.Empty;
                if (wi.ActionEventType == ActionEventType.Backward)
                {
                    opeName = "驳回";
                }
                else if (!string.IsNullOrEmpty(value))
                {
                    var v = value.ToLower();
                    if (rule.ValJson != null)
                    {
                        opeName = rule.ValJson[v];//"通过";
                    }
                    else
                    {
                        opeName = value;
                    }
                }
                else
                {
                    switch (wi.ActionEventType)
                    {
                        case ActionEventType.Backward: opeName = "驳回"; break;
                        case ActionEventType.Forward: opeName = "通过"; break;
                        default: opeName = "已完成"; break;
                    }
                }
            }
            catch (Exception ex)
            {

            }
            if (opeName == "")
                return "通过";
            else
                return opeName;
        }

        private int InsertToTable(InstanceData data, WorkItem wi, InstanceLogRule rule)
        {
            try
            {
                AppUtility.Engine.LogWriter.Write("InsertToTable。。。");
                string opename = GetOpeName(data, wi, rule);
                string sql = "INSERT INTO C_InstanceApprovelLog( WorkItemID ,InstanceID,OpeName ,Activity ,DataField ,CreateTime)VALUES('{0}','{1}','{2}','{3}','{4}','{5}')";
                sql = string.Format(sql, wi.WorkItemID, wi.InstanceId, opename, wi.ActivityCode, rule.DataField, DateTime.Now.ToString());
                int n = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                return n;
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("Insert To C_InstanceApprovelLog Exception:" + ex.ToString());
                return 0;
            }
        }

        private ApprovalResult GetApprovalResult(InstanceData data, WorkItem wi)
        {
            try
            {
                var result = new ApprovalResult();
                if (wi.ActivityCode == "Activity14")//信审初审
                {
                    #region
                    if (wi.ActionEventType == ActionEventType.Backward)//驳回
                    {
                        result.uuid = data["uuid"].Value + string.Empty;
                        result.type = "01";
                        result.status = "005";
                        result.messages = GetAllMsg(wi.InstanceId);
                        return result;
                    }
                    else//其它情况不需要推送
                    {
                        return null;
                    }
                    #endregion
                }
                else if (wi.ActivityCode == "Activity13")//信审终审
                {
                    #region
                    if (wi.ActionEventType == ActionEventType.Backward)//驳回      有问题（驳回到初审还是FI？到FI需要通知）
                    {
                        //返回null，不处理，在待办任务中判断；
                        return null;
                    }
                    else
                    {
                        string zsshzt = data["zsshzt"].Value + string.Empty;
                        result.uuid = data["uuid"].Value + string.Empty;
                        result.type = "01";
                        if (zsshzt == "核准")
                            result.status = "001";
                        else if (zsshzt == "拒绝")
                            result.status = "002";
                        else
                            result.status = "004";
                        result.messages = GetAllMsg(wi.InstanceId);
                        return result;
                    }
                    #endregion
                }
                else if (wi.ActivityCode == "Activity17")//运营初审
                {
                    #region
                    if (wi.ActionEventType == ActionEventType.Backward)//驳回
                    {
                        result.uuid = data["uuid"].Value + string.Empty;
                        result.type = "02";
                        result.status = "005";
                        result.messages = GetAllMsg(wi.InstanceId);
                        return result;
                    }
                    else
                    {
                        //运营初审审核状态：拒绝需要回调；
                        string yycsshzt = data["yycsshzt"].Value + string.Empty;
                        if (yycsshzt == "拒绝")
                        {
                            result.uuid = data["uuid"].Value + string.Empty;
                            result.type = "02";
                            result.status = "002";
                            result.messages = GetAllMsg(wi.InstanceId);
                            return result;
                        }
                        //其它情况不需要推送
                        return null;
                    }
                    #endregion
                }
                else if (wi.ActivityCode == "Activity18")//运营终审
                {
                    #region
                    if (wi.ActionEventType == ActionEventType.Backward)//驳回（只能驳回到初审，不需要消息推送）
                    {
                        return null;
                    }
                    else if (wi.ActionEventType == ActionEventType.Forward)//提交
                    {
                        //运营终审的状态:只有核准与拒绝
                        string yyzsshzt = data["yyzsshzt"].Value + string.Empty;
                        result.uuid = data["uuid"].Value + string.Empty;
                        result.type = "02";
                        if (yyzsshzt == "核准")
                            result.status = "001";
                        else
                            result.status = "002";
                        result.messages = GetAllMsg(wi.InstanceId);
                        return result;
                    }
                    #endregion
                }
                else if (wi.ActivityCode == "Activity12")//风控评估
                {
                    #region
                    string fkresult = data["FK_RESULT"].Value + string.Empty;
                    if (fkresult == "拒绝")
                    {
                        result.uuid = data["uuid"].Value + string.Empty;
                        result.type = "01";
                        result.status = "002";
                        result.messages = GetAllMsg(wi.InstanceId);
                        return result;
                    }
                    #endregion
                }
                else
                {
                    return null;
                }
                return null;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public List<Msg> GetAllMsg(string InstanceId)
        {
            var result = new List<Msg>();
            string sql = "select a.*,b.name as username from I_LY a,OT_User b where A.LYRY=B.OBJECTID and ";
            sql += "a.Parentobjectid = '" + InstanceId + "' order by to_date(a.lysj, 'yyyy-mm-dd hh24:mi:ss') desc";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            foreach (DataRow row in dt.Rows)
            {
                result.Add(
                    new Msg()
                    {
                        text = row["LYXX"] + string.Empty,
                        username = row["USERNAME"] + string.Empty,
                        time = row["LYSJ"] + string.Empty
                    }
                );
            }
            return result;
        }
        #endregion

        #region Class
        public class ApprovalResult
        {
            public string uuid { get; set; }
            public string type { get; set; }
            public string status { get; set; }
            public List<Msg> messages { get; set; }
        }

        public class Msg
        {
            public string text { get; set; }
            public string username { get; set; }
            public string time { get; set; }
        }

        public class InstanceLogRule
        {
            public string ActivityCode { get; set; }
            public string DataField { get; set; }
            public Dictionary<string, string> ValJson { get; set; }
        }
        #endregion

    }
}
