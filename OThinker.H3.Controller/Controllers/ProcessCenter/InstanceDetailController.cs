using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Collections;

namespace OThinker.H3.Controllers
{
    public class InstanceDetailController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InstanceDetailController()
        {
        }

        public string Test()
        {
            return "SUCCESS";
        }
        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }



        /// <summary>
        /// 获取流程状态信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult GetInstanceStateInfo(string InstanceID, string WorkItemID = "")
        {
            InstanceID = OThinker.Data.Convertor.SqlInjectionPrev(InstanceID);
            WorkItemID = OThinker.Data.Convertor.SqlInjectionPrev(WorkItemID);

            //获取ToolBar权限
            //获取基本信息
            //流程运行跟踪信息
            //日志信息
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                if (InstanceContext != null)
                {
                    if (!GetInstanceVal(InstanceID, WorkItemID))
                    {
                        result.Success = false;
                        result.Message = "InstanceDetail_LackOfAuth";
                    }
                    var ToolBarAuthority = this.GetToolBarAuthority(InstanceContext);
                    var BaseInfo = this.GetInstanceBaseInfo(InstanceContext);
                    var CurrentWorkInfo = this.GetCurrentWorkInfo(InstanceID);
                    var InstanceLogInfo = this.GetInstanceLogInfo(InstanceID);
                    var ExceptionActivities = this.ExceptionActivities(InstanceID);
                    var Extend = new
                    {
                        Exceptional = InstanceContext.Exceptional,
                        ToolBarAuthority = ToolBarAuthority,
                        BaseInfo = BaseInfo,
                        CurrentWorkInfo = CurrentWorkInfo,
                        InstanceLogInfo = InstanceLogInfo
                    };
                    result.Extend = Extend;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.Success = false;
                    result.Message = "InstanceDetail_InstanceNotExist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            });
        }

        public bool GetInstanceVal(string InstanceID, string WorkItemID)
        {
            var validate = false;
            Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
            // 验证是否具有权限
            if (!string.IsNullOrEmpty(InstanceContext.ParentInstanceID))
            {
                // 验证该用户对父流程是否有权限
                Instance.InstanceContext parentInstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceContext.ParentInstanceID);
                validate = this.UserValidator.ValidateWFInsView(parentInstanceContext.WorkflowCode, parentInstanceContext.Originator);
                if (!validate)
                {
                    //
                }
            }
            if (!validate)
            {
                // 验证对流程的权限
                validate = this.UserValidator.ValidateWFInsView(InstanceContext.WorkflowCode, InstanceContext.Originator);
                if (!validate && WorkItemID != "")
                {// 验证对某个工作任务的权限
                    OThinker.H3.WorkItem.WorkItem WorkItem = this.Engine.WorkItemManager.GetWorkItem(WorkItemID);
                    if (WorkItem != null)
                    {
                        validate = SheetUtility.ValidateWorkItemAuth(WorkItem, this.UserValidator, this.Engine.AgencyManager);
                    }
                    else
                    {
                        OThinker.H3.WorkItem.CirculateItem CirculateItem = this.Engine.WorkItemManager.GetCirculateItem(WorkItemID);
                        if (CirculateItem != null)
                        {
                            validate = SheetUtility.ValidateCirculateItemAuth(CirculateItem, this.UserValidator);
                        }
                    }
                }
            }
            return validate;
        }

        /// <summary>
        /// 添加一条催办信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="UrgeContent"></param>
        /// <returns></returns>
        public JsonResult AddUrgeInstance(string InstanceID, string UrgeContent)
        {
            return this.ExecuteFunctionRun(() =>
            {
                System.Collections.Generic.List<string> instances = new System.Collections.Generic.List<string>();
                instances.Add(InstanceID);
                instances = this.AddChildren(InstanceID, instances);
                // 催办
                foreach (string instance in instances)
                {
                    this.Engine.UrgencyManager.Urge(this.UserValidator.UserID, instance, UrgeContent);
                }
                return Json("SUCCESS", JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 获取任务的催办信息
        /// </summary>
        /// <param name="WorkItemID"></param>
        /// <returns></returns>
        public JsonResult GetUrgeWorkItemInfo(string WorkItemID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<UrgeInstance> UrgeWorkItem = new List<UrgeInstance>();
                DataTable urgeTable = this.Engine.PortalQuery.QueryUrgency(WorkItemID);
                if (urgeTable != null && urgeTable.Rows.Count > 0)
                {
                    foreach (DataRow row in urgeTable.Rows)
                    {
                        UrgeWorkItem.Add(new UrgeInstance
                        {
                            UrgeUserName = this.Engine.Organization.GetName(row[WorkItem.Urgency.PropertyName_Urger] + string.Empty),
                            UrgeTime = row[WorkItem.Urgency.PropertyName_UrgeTime] + string.Empty,
                            UrgeContent = row[WorkItem.Urgency.PropertyName_Content] + string.Empty
                        });
                    }
                }
                return Json(UrgeWorkItem, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取流程的催办信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult GetUrgeInstanceInfo(string InstanceID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<UrgeInstance> UrgeInstance = new List<UrgeInstance>();
                DataTable urgeTable = this.Engine.PortalQuery.QueryUrgency(this.UserValidator.UserID, InstanceID);
                if (urgeTable != null && urgeTable.Rows.Count > 0)
                {
                    foreach (DataRow row in urgeTable.Rows)
                    {
                        UrgeInstance.Add(new UrgeInstance
                        {
                            UrgeTime = row[WorkItem.Urgency.PropertyName_UrgeTime] + string.Empty,
                            UrgeContent = row[WorkItem.Urgency.PropertyName_Content] + string.Empty
                        });
                    }
                }
                return Json(UrgeInstance, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取流程实例的用户日志
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult GetInstanceUserLog(string InstanceID)
        {
            InstanceID = OThinker.Data.Convertor.SqlInjectionPrev(InstanceID);
            return this.ExecuteFunctionRun(() =>
            {
                bool SUCCESS = true;
                string MSG = "";
                List<UserLogInfo> UserLogInfo = new List<UserLogInfo>();
                if (InstanceID != null && InstanceID != "")
                {
                    Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                    // 检查是否具有管理员权限
                    if (!this.UserValidator.ValidateWFInsView(
                        context.WorkflowCode,
                        context.Originator))
                    {
                        // 不具备权限
                        SUCCESS = false;
                        MSG = "不具备查看权限";
                    }
                    else
                    {
                        DataTable table = this.Engine.PortalQuery.QueryUserLog(InstanceID);
                        DataTable table1 = this.Engine.PortalQuery.QueryUserLog(context.BizObjectId);
                        if (table1 != null)
                        {
                            table.Merge(table1);
                        }
                        table.DefaultView.Sort = Tracking.UserLog.PropertyName_CreatedTime;
                        if (table != null && table.Rows.Count > 0)
                        {
                            Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(table,
                                new string[] { Tracking.UserLog.PropertyName_UserID });
                            for (int i = 0; i < table.Rows.Count; i++)
                            {
                                DataRow row = table.Rows[i];
                                // 日志的类型
                                int nLogType = int.Parse(row[Tracking.UserLog.PropertyName_LogType] + string.Empty);
                                Tracking.UserLogType logType = (Tracking.UserLogType)nLogType;
                                UserLogInfo.Add(new UserLogInfo
                                {
                                    No = (i + 1) + string.Empty,
                                    Time = row[Tracking.UserLog.PropertyName_CreatedTime] + string.Empty,
                                    Type = logType.ToString(),
                                    User = this.GetValueFromDictionary(unitNames, row[Tracking.UserLog.PropertyName_UserID] + string.Empty),
                                    TargetName = row[Tracking.UserLog.PropertyName_TargetName] + string.Empty,
                                    ClientPlatform = row[Tracking.UserLog.PropertyName_ClientPlatform] + string.Empty,
                                    ClientAddress = row[Tracking.UserLog.PropertyName_ClientAddress] + string.Empty
                                });
                            }
                        }
                    }
                }
                var result = new
                {
                    SUCCESS = SUCCESS,
                    MSG = MSG,
                    UserLogInfo = UserLogInfo
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取调整活动的节点信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult GetAdjustActivityInfo(string InstanceID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                bool SUCCESS = true;
                string MSG = "";
                List<InstanceActivity> InstanceActivity = new List<InstanceActivity>();


                Instance.InstanceData InstanceData = null;

                string WorkItemId = string.Empty;
                if (InstanceID == null || InstanceID == "")
                {
                    SUCCESS = false;
                    MSG = "流程InstanceID不能为空";
                }
                else
                {
                    // 获得流程上下文
                    InstanceData = new Instance.InstanceData(this.Engine, InstanceID, this.UserValidator.UserID);
                    // 检查是否具有管理员权限
                    if (!this.UserValidator.ValidateWFInsAdmin(InstanceData.InstanceContext.WorkflowCode, InstanceData.InstanceContext.Originator))
                    {
                        // 不具备权限
                        SUCCESS = false;
                        MSG = "不具备查看权限";
                    }
                    else
                    {
                        // 获得流程模板
                        if (InstanceData.Workflow.Activities != null)
                        {
                            foreach (WorkflowTemplate.Activity activity in InstanceData.Workflow.Activities)
                            {
                                if (activity.ActivityType == WorkflowTemplate.ActivityType.Start) continue;
                                if (activity != null)
                                {
                                    InstanceActivity.Add(new InstanceActivity
                                    {
                                        ActivityCode = activity.ActivityCode,
                                        ActivityName = activity.DisplayName
                                    });
                                }
                            }
                        }
                    }
                }
                var result = new
                {
                    SUCCESS = SUCCESS,
                    MSG = MSG,
                    InstanceActivity = InstanceActivity
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 调整活动 活动节点改变
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="ActivityCode"></param>
        /// <returns></returns>
        public JsonResult GetActivityChangeInfo(string InstanceID, string ActivityCode)
        {
            // 第一种情况：进行中 InstanceData.InstanceContext.IsActivityRunning(ActivityCode)
            // TODO:从 WorkItem表里面找进行中的参与者，只允许调整活动参与者（单人、多人）
            // 第二种情况：已结束或未激活
            // TODO: 如果是 ClientActivity
            return this.ExecuteFunctionRun(() =>
            {
                //Enabled 是否启用
                bool UserEnabled = false;//选人控件是否启用
                bool btnActivateEnabled = false; //激活活动是否启用
                bool btnCancelEnabled = false; //取消活动是否启用
                bool btnAdjustEnabled = false; //调整活动参与者是否启用
                string[] UserValue = null;//选人控件的默认值
                Instance.InstanceData InstanceData = new Instance.InstanceData(this.Engine, InstanceID, this.UserValidator.UserID);
                WorkflowTemplate.Activity DestActivity = null;//目标任务
                if (InstanceData.Workflow != null)
                {
                    DestActivity = InstanceData.Workflow.GetActivityByCode(ActivityCode);
                }

                if (InstanceData.Workflow == null || InstanceData.Workflow.GetActivityByCode(ActivityCode) == null)
                {
                    // 没有选中的要操作的节点
                    //btnActivate = false;btnCancel = false;lnkAdjust = false;
                }
                else if (InstanceData.InstanceContext.IsActivityRunning(ActivityCode))
                {
                    // 处于运行状态
                    UserEnabled = true;
                    btnCancelEnabled = true;
                    btnAdjustEnabled = true;
                    // 查找处于运行状态的用户
                    Instance.IToken[] tokens = InstanceData.InstanceContext.GetTokens(ActivityCode, Instance.TokenState.Running);
                    if (tokens != null && tokens.Length > 0)
                    {
                        UserValue = this.Engine.Query.QueryParticipants(
                            InstanceID,
                            tokens[0].TokenId,
                            OThinker.H3.WorkItem.WorkItemType.Participative,
                            OThinker.H3.WorkItem.WorkItemState.NotCanceled);
                    }
                }
                else
                {
                    // 处于非运行状态，只能激活
                    btnActivateEnabled = true;
                    if (!(DestActivity is WorkflowTemplate.ParticipativeActivity)) { }
                    else
                    {// 初始化选人的对话框
                        if (((WorkflowTemplate.ParticipativeActivity)DestActivity).ParticipateType == OThinker.H3.WorkflowTemplate.ActivityParticipateType.NoneParticipant)
                        {
                        }
                        else if (((WorkflowTemplate.ParticipativeActivity)DestActivity).ParticipateType == OThinker.H3.WorkflowTemplate.ActivityParticipateType.SingleParticipant)
                        {
                            UserEnabled = true;
                            string[] participants = this.GetOptionalParticipants(DestActivity, InstanceData);
                            if (participants != null && participants.Length > 0)
                            {
                                UserValue = new string[1] { participants[0] };
                            }
                        }
                        else if (((WorkflowTemplate.ParticipativeActivity)DestActivity).ParticipateType == OThinker.H3.WorkflowTemplate.ActivityParticipateType.MultiParticipants)
                        {
                            UserEnabled = true;
                            string[] participants = this.GetOptionalParticipants(DestActivity, InstanceData);
                            if (participants != null && participants.Length > 0)
                            {
                                UserValue = participants;
                            }
                        }
                        else if (((WorkflowTemplate.ParticipativeActivity)DestActivity).ParticipateType == OThinker.H3.WorkflowTemplate.ActivityParticipateType.AllParticipants)
                        {

                        }
                    }
                }
                var result = new
                {
                    btnActivateEnabled = btnActivateEnabled,
                    btnCancelEnabled = btnCancelEnabled,
                    btnAdjustEnabled = btnAdjustEnabled,
                    UserEnabled = UserEnabled,
                    UserValue = UserValue,
                    ActivityType = DestActivity.ActivityType
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取可以调整活动参与者的范围
        /// </summary>
        /// <returns></returns>
        private string[] GetOptionalParticipants(WorkflowTemplate.Activity DestActivity, Instance.InstanceData InstanceData)
        {
            return ((WorkflowTemplate.ParticipativeActivity)DestActivity).ParseParticipants(InstanceData, this.Engine.Organization);
        }

        /// <summary>
        /// 取消流程实例
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult CancelInstance(string InstanceID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                // 取消该流程
                Messages.CancelInstanceMessage cancelMessage =
                        new Messages.CancelInstanceMessage(
                            Messages.MessageEmergencyType.Normal,
                            InstanceID,
                            true);
                this.Engine.InstanceManager.SendMessage(cancelMessage);
                // 记录日志
                Tracking.UserLog log = new Tracking.UserLog(
                                    Tracking.UserLogType.CancelInstance,
                                    this.UserValidator.UserID,
                                    InstanceContext.BizObjectSchemaCode,
                                    InstanceContext.BizObjectId,
                                    InstanceID,
                                    InstanceID,
                                    null,
                                    null,
                                    SheetUtility.GetClientIP(Request),
                                    SheetUtility.GetClientPlatform(Request),
                                    SheetUtility.GetClientBrowser(Request));
                this.Engine.UserLogWriter.Write(log);
                var result = new
                {
                    SUCCESS = true,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 激活流程实例
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult ActivateInstance(string InstanceID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Messages.ActivateInstanceMessage activateMessage = new Messages.ActivateInstanceMessage(InstanceID);
                this.Engine.InstanceManager.SendMessage(activateMessage);
                var result = new
                {
                    SUCCESS = true,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除流程实例
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public JsonResult RemoveInstance(string InstanceID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                this.Engine.InstanceManager.RemoveInstance(InstanceID, true);
                var result = new
                {
                    SUCCESS = true,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 激活活动
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="ActivityCode"></param>
        /// <param name="participants"></param>
        /// <returns></returns>
        public JsonResult ActivateActivity(string InstanceID, string ActivityCode, string[] Participants)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Instance.InstanceData InstanceData = new Instance.InstanceData(this.Engine, InstanceID, this.UserValidator.UserID);
                // 准备触发后面Activity的消息
                OThinker.H3.Messages.ActivateActivityMessage activateMessage
                    = new OThinker.H3.Messages.ActivateActivityMessage(
                        OThinker.H3.Messages.MessageEmergencyType.Normal,
                        InstanceID,
                        ActivityCode,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        Participants,
                        null,
                        false,
                        H3.WorkItem.ActionEventType.Adjust);
                this.Engine.InstanceManager.SendMessage(activateMessage);
                // 记录操作日志
                OThinker.H3.Tracking.UserLog log = new Tracking.UserLog(
                    Tracking.UserLogType.ActivateActivity,
                    this.UserValidator.UserID,
                    InstanceData.InstanceContext.BizObjectSchemaCode,
                    InstanceData.InstanceContext.BizObjectId,
                    InstanceData.InstanceContext.InstanceId,
                    ActivityCode,
                    ActivityCode,
                    null,
                    SheetUtility.GetClientIP(this.HttpContext.Request),
                    SheetUtility.GetClientPlatform(this.HttpContext.Request),
                    SheetUtility.GetClientBrowser(this.HttpContext.Request)
                    );
                this.Engine.UserLogWriter.Write(log);
                var result = new
                {
                    SUCCESS = true,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 取消活动
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="ActivityCode"></param>
        /// <returns></returns>
        public JsonResult CancelActivity(string InstanceID, string ActivityCode)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Instance.InstanceData InstanceData = new Instance.InstanceData(this.Engine, InstanceID, this.UserValidator.UserID);
                // 准备触发后面Activity的消息
                OThinker.H3.Messages.CancelActivityMessage cancelMessage
                    = new OThinker.H3.Messages.CancelActivityMessage(
                        OThinker.H3.Messages.MessageEmergencyType.Normal,
                        InstanceID,
                        ActivityCode,
                        false);
                this.Engine.InstanceManager.SendMessage(cancelMessage);
                // 记录操作日志
                OThinker.H3.Tracking.UserLog log = new Tracking.UserLog(
                    Tracking.UserLogType.CancelActivity,
                    this.UserValidator.UserID,
                    InstanceData.InstanceContext.BizObjectSchemaCode,
                    InstanceData.InstanceContext.BizObjectId,
                    InstanceData.InstanceContext.InstanceId,
                    ActivityCode,
                    ActivityCode,
                    null,
                    SheetUtility.GetClientIP(Request),
                    SheetUtility.GetClientPlatform(Request),
                    SheetUtility.GetClientBrowser(Request));
                this.Engine.UserLogWriter.Write(log);
                var result = new
                {
                    SUCCESS = true,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 调整活动参与者
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="ActivityCode"></param>
        /// <param name="Participants"></param>
        /// <returns></returns>
        public JsonResult AdjustActivity(string InstanceID, string ActivityCode, string[] Participants)
        {
            return this.ExecuteFunctionRun(() =>
            {
                Instance.InstanceData InstanceData = new Instance.InstanceData(this.Engine, InstanceID, this.UserValidator.UserID);
                int tokenId = InstanceData.InstanceContext.GetLastToken(ActivityCode).TokenId;
                OThinker.H3.Messages.AdjustParticipantMessage message = new OThinker.H3.Messages.AdjustParticipantMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    InstanceID,
                    ActivityCode,
                    Participants,
                    tokenId);
                this.Engine.InstanceManager.SendMessage(message);
                // 记录操作日志
                OThinker.H3.Tracking.UserLog log = new Tracking.UserLog(
                    Tracking.UserLogType.AdjustParticipant,
                    this.UserValidator.UserID,
                    InstanceData.InstanceContext.BizObjectSchemaCode,
                    InstanceData.InstanceContext.BizObjectId,
                    InstanceData.InstanceContext.InstanceId,
                    ActivityCode,
                    ActivityCode,
                    null,
                    SheetUtility.GetClientIP(Request),
                    SheetUtility.GetClientPlatform(Request),
                    SheetUtility.GetClientBrowser(Request));
                this.Engine.UserLogWriter.Write(log);
                var result = new
                {
                    SUCCESS = true
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取子流程ID
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="Instances"></param>
        /// <returns></returns>
        private System.Collections.Generic.List<string> AddChildren(string InstanceID, System.Collections.Generic.List<string> Instances)
        {
            if (InstanceID != null && InstanceID != "" && InstanceID != Instance.InstanceContext.UnspecifiedID)
            {
                string[] children = this.Engine.PortalQuery.QueryChildInstances(
                        InstanceID,
                        H3.Instance.Token.UnspecifiedID,
                        OThinker.H3.Instance.InstanceState.Unfinished,
                        OThinker.Data.BoolMatchValue.Unspecified);

                if (children != null && children.Length != 0)
                {
                    foreach (string child in children)
                    {
                        if (child != null && child != "" && child != Instance.InstanceContext.UnspecifiedID)
                        {
                            Instances.Add(child);
                            this.AddChildren(child, Instances);
                        }
                    }
                }
            }
            return Instances;
        }

        /// <summary>
        /// 获取流程状态页面ToolBar权限
        /// </summary>
        /// <param name="InstanceContext"></param>
        /// <returns></returns>
        private object GetToolBarAuthority(Instance.InstanceContext InstanceContext)
        {
            bool validate = this.UserValidator.ValidateWFInsAdmin(InstanceContext.WorkflowCode, InstanceContext.Originator);
            //是否显示
            var UrgeActivity_Show = true;
            var ViewInstanceData_Show = true;
            var ViewInstanceSheets_Show = true;
            var ViewUserLog_Show = true;
            var AdjustActivity_Show = true;
            var ReactActivity_Show = true;
            var CancelActivity_Show = true;
            var Remove_Show = true;
            //是否启用
            var AdjustActivity_Enabled = true;
            var Cancel_Enabled = true;
            var Urge_Enabled = true;
            var Activate_Enabled = true;
            if (!validate)
            {
                // 检查是否能显示调整活动的按钮
                AdjustActivity_Show = false;
                ReactActivity_Show = false;
                // 检查是否能显示取消的按钮
                CancelActivity_Show = false;
                // 检查是否能显示删除的按钮
                Remove_Show = false;
                // 检查是否能显示查看的按钮
                ViewInstanceData_Show = false;
                ViewInstanceSheets_Show = false;
                ViewUserLog_Show = false;
            }

            // 如果该流程已经完成则隐藏某些按钮
            if (InstanceContext.State == Instance.InstanceState.Finished ||
                InstanceContext.State == Instance.InstanceState.Canceled)
            {
                AdjustActivity_Enabled = false;//调整活动
                Cancel_Enabled = false;//取消
                Urge_Enabled = false;//催办
                Activate_Enabled = true;//重新激活
            }
            else
            {
                // 只有取消和完成状态才允许被重新激活
                Activate_Enabled = false;//重新激活
            }

            var result = new
            {
                UrgeActivity_Show = UrgeActivity_Show,
                ViewInstanceData_Show = ViewInstanceData_Show,
                ViewInstanceSheets_Show = ViewInstanceSheets_Show,
                ViewUserLog_Show = ViewUserLog_Show,
                AdjustActivity_Show = AdjustActivity_Show,
                ReactActivity_Show = ReactActivity_Show,
                CancelActivity_Show = CancelActivity_Show,
                Remove_Show = Remove_Show,

                Urge_Enabled = Urge_Enabled,
                AdjustActivity_Enabled = AdjustActivity_Enabled,
                Activate_Enabled = Activate_Enabled,
                Cancel_Enabled = Cancel_Enabled
            };
            return result;
        }

        /// <summary>
        /// 获取流程实例的基本信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        public object GetInstanceBaseInfo(Instance.InstanceContext InstanceContext)
        {
            string PlanFinishDate = "";
            if (InstanceContext.PlanFinishTime.Year == 9999)
            {
                PlanFinishDate = "";
            }
            else
            {
                PlanFinishDate = InstanceContext.PlanFinishTime.ToShortDateString();
            }

            var result = new
            {
                Exceptional = InstanceContext.Exceptional ? true : false,
                SchemaCode = InstanceContext.BizObjectSchemaCode,
                BizObjectId = InstanceContext.BizObjectId,
                InstanceId = InstanceContext.InstanceId,
                Name = InstanceContext.InstanceName,
                SequenceNo = InstanceContext.SequenceNo,
                Originator = InstanceContext.OriginatorName,
                //父流程链接
                ParentflowLink = InstanceContext.ParentInstanceID,
                WorkflowFullName = this.Engine.WorkflowManager.GetTemplateDisplayName(
                    InstanceContext.WorkflowCode,
                    InstanceContext.WorkflowVersion),
                Priority = Instance.PriorityTypeConvertor.ToString(this.UserValidator.PortalSettings,
                    InstanceContext.Priority),
                State = OThinker.H3.Settings.CustomSetting.GetStateName(this.UserValidator.PortalSettings,
                        InstanceContext.State, InstanceContext.Approval),
                StartDate = InstanceContext.StartTime.ToShortDateString(),
                FinishDate = (InstanceContext.State == Instance.InstanceState.Finished) ? InstanceContext.FinishTime.ToShortDateString() : string.Empty,
                PlanFinishDate = PlanFinishDate
            };
            return result;
        }




        /// <summary>
        /// 获取流程实例的当前任务信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        private List<WorkItemInfo> GetCurrentWorkInfo(string InstanceID)
        {
            List<WorkItemInfo> result = new List<WorkItemInfo>();
            Instance.InstanceContext entity = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
            if (entity != null)
            {
                // 查找流程所有任务（传阅）               
                DataTable table = this.GetItemTable(InstanceID, true);

                // 按照活动名称排序人员
                Dictionary<string, List<string>> actParticipants = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> actAssistants = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> actConsultants = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> actCicrculators = new Dictionary<string, List<string>>();
                // 所有活动
                List<string> activities = new List<string>();

                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit, WorkItem.WorkItem.PropertyName_Participant, WorkItem.WorkItem.PropertyName_Creator };
                Dictionary<string, string> userNameTable = this.GetUnitNamesFromTable(table, columns);

                Dictionary<string, string> userCodeTable = this.GetUnitCodesFromTable(table, new string[] { WorkItem.WorkItem.PropertyName_Participant });


                // 排序
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        string state = row[WorkItem.WorkItem.PropertyName_State] + string.Empty;
                        if (state == "2" || state == "3") continue;

                        // 活动名称
                        string actName = (string)row[WorkItem.WorkItem.PropertyName_DisplayName];
                        // 任务类型
                        WorkItem.WorkItemType itemType = (WorkItem.WorkItemType)int.Parse(row[WorkItem.WorkItem.PropertyName_ItemType].ToString());
                        // 人员
                        string participant = row[WorkItem.WorkItem.PropertyName_Participant] + string.Empty;
                        string parName = (string.IsNullOrEmpty(participant) || !userNameTable.ContainsKey(participant)) ? null : userNameTable[participant] + "[" + userCodeTable[participant] + "]";

                        // 添加到记录中
                        if (!activities.Contains(actName))
                        {
                            activities.Add(actName);
                            actParticipants.Add(actName, new List<string>());
                            actAssistants.Add(actName, new List<string>());
                            actConsultants.Add(actName, new List<string>());
                            actCicrculators.Add(actName, new List<string>());
                        }

                        // 列表名称
                        switch (itemType)
                        {
                            case OThinker.H3.WorkItem.WorkItemType.ActivityConsult:
                            case OThinker.H3.WorkItem.WorkItemType.WorkItemConsult:
                                // 征询意见模式
                                actConsultants[actName].Add(parName);
                                break;
                            case OThinker.H3.WorkItem.WorkItemType.Approve:
                            case OThinker.H3.WorkItem.WorkItemType.Fill:
                            case OThinker.H3.WorkItem.WorkItemType.Read:
                                // 参与者
                                actParticipants[actName].Add(parName);
                                break;
                            case WorkItem.WorkItemType.ActivityAssist:
                            case OThinker.H3.WorkItem.WorkItemType.WorkItemAssist:
                                // 征询意见
                                actAssistants[actName].Add(parName);
                                break;
                            case OThinker.H3.WorkItem.WorkItemType.Circulate:
                                // 传阅
                                actCicrculators[actName].Add(parName);
                                break;
                            default:
                                throw new NotImplementedException();
                        }
                    }
                }

                foreach (string activity in activities)
                {
                    // 查找该任务对应的接收人
                    result.Add(new WorkItemInfo
                    {
                        Activity = activity,
                        Participants = actParticipants[activity],
                        Assistants = actAssistants[activity],
                        Consultants = actConsultants[activity],
                        Circulates = actCicrculators[activity]
                    });
                }
            }
            return result;
        }


        /// <summary>
        /// 获取流程的日志信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        private List<InstanceLogInfo> GetInstanceLogInfo(string InstanceID)
        {
            Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
            DataTable TokenTable = InstanceContext.GetTokenTable();
            DataTable ItemTable = this.GetItemTable(InstanceID, false);
            // 查询子流程
            DataTable ChildInstanceTable = this.Engine.Query.QueryChildInstance(InstanceID,
                Instance.Token.UnspecifiedID,
                Instance.InstanceState.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified);
            //获取所有的日志信息
            IEnumerable<LogInfo> Items = this.GetItemTable(TokenTable, ItemTable, ChildInstanceTable);

            #region //获取活动节点及字段编码：2018-01-19 chs
            List<Dictionary<string, string>> listActivityAndField = new List<Dictionary<string, string>>();
            string activityAndField = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT  ActivityAndField FROM I_Para_Operation WHERE FlowCode='" + InstanceContext.WorkflowCode + "'") + string.Empty;
            if (!string.IsNullOrEmpty(activityAndField.Trim()))
            {
                try
                {
                    var pair = activityAndField.Split('|');
                    foreach (string kv in pair)
                    {
                        Dictionary<string, string> d = new Dictionary<string, string>();
                        var v = kv.Split(';');
                        d.Add(v[0], v[1]);
                        listActivityAndField.Add(d);
                    }
                }
                catch (Exception ex)
                {
                    this.Engine.LogWriter.Write("解析流程操作设置失败：" + ex.ToString());
                }
            }

            string sqlLogInfo = "SELECT WorkItemID, OpeName FROM C_InstanceApprovelLog WHERE InstanceID='" + InstanceContext.ObjectID + "'";
            DataTable dtLogInfo = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlLogInfo);

            #endregion

            //数据处理返回到前端
            List<InstanceLogInfo> result = new List<InstanceLogInfo>();

            WorkflowTemplate.PublishedWorkflowTemplate WorkflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplate(InstanceContext.WorkflowCode, InstanceContext.WorkflowVersion);
            foreach (var item in Items)
            {
                //活动显示名称
                WorkflowTemplate.Activity Activity = WorkflowTemplate.GetActivityByCode(item.ActivityCode);
                string ActivityName = string.Empty;
                if (Activity != null)
                {
                    ActivityName = Activity.DisplayName + string.Empty;
                }



                // 设置完成时间是否显示
                string finishedTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.FinishedTime);
                // item.FinishedTime + string.Empty;
                if (finishedTime.StartsWith("1753") || finishedTime.StartsWith("9999") || finishedTime.StartsWith("000"))
                    finishedTime = string.Empty;
                // 计算使用时间
                string usedTimeText = item.UsedTime + string.Empty;
                System.TimeSpan usedTime = new TimeSpan(0, 0, 0);
                if (usedTimeText != string.Empty)
                {
                    long v = 0;
                    if (long.TryParse(usedTimeText, out v))
                    {
                        usedTime = new TimeSpan(v);
                    }
                    else
                    {
                        TimeSpan.TryParse(usedTimeText, out usedTime);
                    }
                }
                if (usedTime.TotalSeconds > 1)
                {
                    string usedTimeStr = usedTime.Days == 0 ? "" : usedTime.Days + "天";
                    usedTimeStr += string.IsNullOrWhiteSpace(usedTimeStr) ? (usedTime.Hours == 0 ? "" : usedTime.Hours + "小时") : (usedTime.Hours + "小时");
                    usedTimeStr += string.IsNullOrWhiteSpace(usedTimeStr) ? (usedTime.Minutes == 0 ? "" : usedTime.Minutes + "分钟") : (usedTime.Minutes + "分钟");
                    usedTimeStr += string.IsNullOrWhiteSpace(usedTimeStr) ? (usedTime.Seconds == 0 ? "" : usedTime.Seconds + "秒") : (usedTime.Seconds + "秒");
                    usedTimeText = usedTimeStr;
                }
                else
                {
                    usedTimeText = "-";
                }
                //获取组织对象的显示名称集合
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_Delegant, WorkItem.WorkItem.PropertyName_Creator };
                Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(ItemTable, columns);

                // 设置处理人，四部分（参与者，委托人，处理人/提交人，其他（征询，协办。。。））
                //                  participant-delegant-finisher-otherUser

                // 提交人
                string finisher = string.Empty;
                string finisherName = string.Empty;
                //参与者
                string participant = string.Empty;
                string participantName = string.Empty;
                //委托人
                string delegant = string.Empty;
                string delegantName = string.Empty;

                //其他（征询，协办。。。）
                string creator = string.Empty;
                string creatorName = string.Empty;
                string itemTypeValue = item.ItemType + string.Empty;


                if (!string.IsNullOrEmpty(item.Participant))
                {
                    participant = item.Participant + string.Empty;
                    participantName = item.ParticipantName + string.Empty;
                }

                if (!string.IsNullOrEmpty(item.Delegant) && item.Participant == item.Finisher)
                {
                    //如果代理人自己处理,就不关被代理人什么事了，直接显示代理人信息
                    participant = "";
                    participantName = "";
                    finisher = item.Finisher + string.Empty;
                    finisherName = item.FinisherName + string.Empty;
                }
                else
                {
                    if (string.IsNullOrEmpty(item.Delegant) || item.Delegant == item.Participant)
                    {

                    }
                    else
                    {
                        delegant = item.Delegant + string.Empty;
                        delegantName = this.GetValueFromDictionary(unitNames, delegant) + string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(finisher) || finisher == delegant || finisher == participant)
                {

                }
                else
                {
                    finisher = item.Finisher + string.Empty;
                    finisherName = item.FinisherName + string.Empty;
                }
                string ParentPropertyText = string.Empty;
                string strState = string.Empty;
                string Forwarder = string.Empty;
                string userName = string.Empty;
                if (itemTypeValue != string.Empty)
                {
                    // 类型
                    WorkItem.WorkItemType itemType = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(itemTypeValue);
                    string action = this.GetItemAction(itemType);
                    WorkItem.WorkItem _workitem = this.Engine.WorkItemManager.GetWorkItem(item.WorkItemID);
                    if (_workitem != null)
                    {
                        Forwarder = _workitem.Forwarder;
                        if (!string.IsNullOrWhiteSpace(Forwarder))
                        {
                            OThinker.Organization.Unit unit = this.Engine.Organization.GetUnit(Forwarder);
                            if (unit != null) userName = unit.Name;
                        }
                    }
                    // 创建人
                    if (!string.IsNullOrEmpty(Forwarder) && !string.Equals(Forwarder, item.Participant))
                    {
                        creator = Forwarder.Trim() + string.Empty;
                        creatorName = userName == null ? "(" + this.GetValueFromDictionary(unitNames, creator) : "(" + userName + "的" + Configs.Global.ResourceManager.GetString("SheetActionPane_Forward") + ")";
                        ParentPropertyText = this.GetItemAction(itemType);
                    }
                    else if (string.IsNullOrEmpty(item.Creator) || item.Creator == item.Delegant || item.Creator == item.Participant)
                    {
                        if (!string.IsNullOrEmpty(action))
                        {

                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(action))
                        {
                            creator = item.Creator.Trim() + string.Empty;
                            creatorName = "(" + this.GetValueFromDictionary(unitNames, creator) + "的" + this.GetItemAction(itemType) + ")";
                            ParentPropertyText = this.GetItemAction(itemType);
                        }
                    }
                }

                // 设置任务状态名称
                string stateValue = item.State + string.Empty;
                string approval = item.Approval + string.Empty;
                string instanceStateValue = item.InstanceState + string.Empty;
                string childInstanceId = string.Empty;
                if (instanceStateValue != string.Empty)
                {// 子流程
                    childInstanceId = item.ChildInstanceID + string.Empty;
                    OThinker.Data.BoolMatchValue childApproval = (OThinker.Data.BoolMatchValue)int.Parse(approval);
                    Instance.InstanceState childState = (Instance.InstanceState)int.Parse(instanceStateValue);
                    strState = OThinker.H3.Settings.CustomSetting.GetStateName(this.UserValidator.PortalSettings, childState, childApproval);
                }
                else if (stateValue != string.Empty && !string.Equals(item.ItemType, ((int)(WorkItem.WorkItemType.ActivityForward)).ToString()))
                {// 非子流程
                    WorkItem.WorkItemState state = (WorkItem.WorkItemState)OThinker.Data.Convertor.Convert<int>(stateValue);
                    //strState = H3.Settings.CustomSetting.GetStateName(
                    //            this.UserValidator.PortalSettings,
                    //            state,
                    //            (OThinker.Data.BoolMatchValue)OThinker.Data.Convertor.Convert<int>(approval));

                    #region 修改流程日志的显示名称
                    if (listActivityAndField.Count > 0)
                    {
                        foreach (var v in listActivityAndField)
                        {
                            if (v.Keys.Contains(item.ActivityCode))
                            {
                                DataRow[] rows = dtLogInfo.Select("WorkItemID='" + item.WorkItemID + "'");
                                if (rows.Length > 0)
                                {
                                    strState = rows[0]["OpeName"] + string.Empty;
                                    break;
                                }
                            }
                        }
                    }
                    if (strState == "")
                    {
                        strState = H3.Settings.CustomSetting.GetStateName(
                                    this.UserValidator.PortalSettings,
                                    state,
                                    (OThinker.Data.BoolMatchValue)OThinker.Data.Convertor.Convert<int>(approval));
                    }
                    #endregion
                    
                }
                else if (string.Equals(item.ItemType, ((int)(WorkItem.WorkItemType.ActivityForward)).ToString()))
                {
                    WorkItem.WorkItemType _itemType = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(item.ItemType);
                    //strState = this.GetItemAction(_itemType);

                    #region 修改流程日志的显示名称
                    if (listActivityAndField.Count > 0)
                    {
                        foreach (var v in listActivityAndField)
                        {
                            if (v.Keys.Contains(item.ActivityCode))
                            {
                                DataRow[] rows = dtLogInfo.Select("WorkItemID='" + item.WorkItemID + "'");
                                if (rows.Length > 0)
                                {
                                    strState = rows[0]["OpeName"] + string.Empty;
                                    break;
                                }
                            }
                        }
                    }
                    if (strState == "")
                    {
                        strState = this.GetItemAction(_itemType);
                    }
                    #endregion
                }

                if (item.WorkflowCode == "APPLICATION" && string.Equals(item.ItemType, ((int)(WorkItem.WorkItemType.Circulate)).ToString()))
                {
                    strState = "已传阅";
                }

                //string text = string.Empty;

                result.Add(new InstanceLogInfo
               {
                   No = item.TokenID + string.Empty,
                   ActivityCode = item.ActivityCode,
                   ActivityName = ActivityName,//item.ActivityName,
                   CreatedTime = string.Format("{0:yyyy-MM-dd HH:mm:ss}", item.CreatedTime), //item.CreatedTime,
                   FinishedTime = finishedTime,
                   UsedTime = usedTimeText,
                   Participant = participant,
                   ParticipantName = participantName.Trim(),
                   Finisher = finisher,
                   FinisherName = finisherName,
                   Delegant = delegant,
                   DelegantName = delegantName,
                   Creator = creator,
                   CreatorName = creatorName,
                   ApprovalName = strState,
                   ChildInstanceId = childInstanceId,
                   ParentPropertyText = ParentPropertyText
               });
            }
            return result;
        }


        /// <summary>
        /// 生成流程状态图片
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <param name="base64"></param>
        /// <returns></returns>
        public object Base64ToImg(string InstanceID, string base64)
        {
            //保存文件到服务器
            string fileName = InstanceID + ".jpg";
            string savePath = AppDomain.CurrentDomain.BaseDirectory + "/TempImages/" + fileName;

            try
            {
                string[] img = base64.Split(',');
                byte[] content = Convert.FromBase64String(img[1]);
                using (FileStream fs = new FileStream(savePath, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                {
                    using (BinaryWriter bw = new BinaryWriter(fs))
                    {
                        bw.Write(content);
                        bw.Close();
                    }
                }
            }
            catch { }

            savePath = System.Configuration.ConfigurationManager.AppSettings["PortalRoot"] + savePath.Replace(AppDomain.CurrentDomain.BaseDirectory, "").Replace(@"\", "/");
            return savePath;
        }


        private string[] ExceptionActivities(string InstanceId)
        {
            //异常活动
            List<string> lstExceptionActivities = new List<string>();
            Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceId);
            Exceptions.ExceptionLog[] ExceptionLogs = this.Engine.ExceptionManager.GetExceptionsByInstance(InstanceId);
            if (ExceptionLogs != null)
            {

                foreach (Exceptions.ExceptionLog ExceptionLog in ExceptionLogs)
                {
                    if (ExceptionLog.State == Exceptions.ExceptionState.Unfixed)
                    {
                        InstanceContext.State = Instance.InstanceState.Exceptional;
                        if (ExceptionLog.SourceType == Instance.RuntimeObjectType.Activity)
                        {
                            lstExceptionActivities.Add(ExceptionLog.SourceName);
                        }
                    }
                }
            }
            return lstExceptionActivities.ToArray();
        }

        /// <summary>
        /// 获取流程日志的Item表
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        private DataTable GetItemTable(string InstanceID, bool customState)
        {
            //获取流程实例所有ItemTable
            DataTable WorkItemTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.WorkItem.TableName);
            DataTable WorkItemFinishedTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.WorkItemFinished.TableName);
            DataTable CirculateItemTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.CirculateItem.TableName);
            DataTable CirculateItemFinishedTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.CirculateItemFinished.TableName);

            //合并Table   
            string[] columns = new string[] { Instance.Token.PropertyName_TokenId, WorkItem.WorkItem.PropertyName_ObjectID, 
                WorkItem.WorkItem.PropertyName_Participant, WorkItem.WorkItem.PropertyName_ParticipantName, WorkItem.WorkItem.PropertyName_ActivityCode,
                WorkItem.WorkItem.PropertyName_DisplayName, WorkItem.WorkItem.PropertyName_ReceiveTime, WorkItem.WorkItem.PropertyName_FinishTime, 
                WorkItem.WorkItem.PropertyName_UsedTime, WorkItem.WorkItem.PropertyName_OrgUnit, WorkItem.WorkItem.PropertyName_Approval, 
                WorkItem.WorkItem.PropertyName_Delegant, WorkItem.WorkItem.PropertyName_Finisher, WorkItem.WorkItem.PropertyName_FinisherName, 
                WorkItem.WorkItem.PropertyName_ItemType, WorkItem.WorkItem.PropertyName_Creator, WorkItem.WorkItem.PropertyName_State,WorkItem.WorkItem.PropertyName_WorkflowCode,
                WorkItem.WorkItem.PropertyName_InstanceId
               // ,WorkItem.WorkItem.PropertyName_Forwarder
                 };

            //if (customState)
            //{
            //    ArrayList al = new ArrayList(columns);
            //    al.RemoveAt(columns.Length - 1);
            //    columns = (string[])al.ToArray(typeof(string));
            //}

            DataTable dt = new DataTable();
            foreach (string col in columns)
            {
                dt.Columns.Add(col);
            }
            foreach (DataRow row in WorkItemTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    dr[col] = row[col];
                }
                dt.Rows.Add(dr);
            }
            foreach (DataRow row in WorkItemFinishedTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    dr[col] = row[col];
                }
                dt.Rows.Add(dr);
            }
            foreach (DataRow row in CirculateItemTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    if (col == WorkItem.WorkItem.PropertyName_Approval)
                    {
                        dr[col] = 1;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_UsedTime)
                    {
                        dr[col] = 0;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_ItemType)
                    {
                        dr[col] = 4;
                    }
                    else
                    {
                        dr[col] = row[col];
                    }
                }
                dt.Rows.Add(dr);
            }
            foreach (DataRow row in CirculateItemFinishedTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    if (col == WorkItem.WorkItem.PropertyName_Approval)
                    {
                        dr[col] = 1;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_UsedTime)
                    {
                        dr[col] = 0;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_ItemType)
                    {
                        dr[col] = 4;
                    }
                    else
                    {
                        dr[col] = row[col];
                    }
                }
                dt.Rows.Add(dr);
            }
            //排序
            dt.DefaultView.Sort = Instance.Token.PropertyName_TokenId + " asc";
            dt = dt.DefaultView.ToTable();
            return dt;
        }

        private IEnumerable<LogInfo> GetItemTable(
            DataTable TokenTable,
            DataTable ItemTable,
            DataTable ChildInstanceTable)
        {
            var query = from t1 in TokenTable.AsEnumerable()
                        join t2 in ItemTable.AsEnumerable()
                        on t1[Instance.Token.PropertyName_TokenId].ToString()
                            equals t2[WorkItem.WorkItem.PropertyName_TokenId].ToString()
                            into tmp
                        from workItem in tmp.DefaultIfEmpty()
                        // orderby workItem[WorkItem.WorkItem.PropertyName_TokenId], workItem[WorkItem.WorkItem.PropertyName_FinishTime]
                        select new
                        {
                            TokenID = int.Parse(t1[Instance.Token.PropertyName_TokenId] + string.Empty),
                            WorkItemID = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty,
                            Participant = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Participant] + string.Empty,
                            ParticipantName = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_ParticipantName] + string.Empty,
                            ActivityCode = workItem == null ? t1[Instance.Token.PropertyName_Activity] + string.Empty : workItem[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty,
                            ActivityName = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                            CreatedTime = workItem == null ? DateTime.Parse(t1[Instance.Token.PropertyName_CreatedTime] + string.Empty)
                                    : DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty),
                            FinishedTime = workItem == null ? DateTime.Parse(t1[Instance.Token.PropertyName_FinishedTime] + string.Empty)
                                    : DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty),
                            SortTime = workItem == null ? DateTime.Parse(t1[Instance.Token.PropertyName_FinishedTime] + string.Empty)
                                : (DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty).Year < 2000 ? DateTime.Now : DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty)),
                            Approval = workItem == null ? string.Empty : (workItem[WorkItem.WorkItem.PropertyName_Approval] + string.Empty),
                            Retrievable = bool.Parse(t1[Instance.Token.PropertyName_Retrievable] + string.Empty),
                            Exceptional = bool.Parse(t1[Instance.Token.PropertyName_Exceptional] + string.Empty),
                            UsedTime = workItem == null ? t1[Instance.Token.PropertyName_UsedTime] + string.Empty : workItem[WorkItem.WorkItem.PropertyName_UsedTime] + string.Empty,
                            OrgUnit = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_OrgUnit] + string.Empty,
                            Delegant = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Delegant] + string.Empty,
                            Finisher = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Finisher] + string.Empty,
                            FinisherName = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_FinisherName] + string.Empty,
                            ItemType = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_ItemType] + string.Empty,
                            Creator = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Creator] + string.Empty,
                            //Forwarder = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Forwarder] + string.Empty,
                            State = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_State] + string.Empty,
                            WorkflowCode = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty,
                            InstanceId = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_InstanceId] + string.Empty
                        };
            var result = from q in query
                         join t3 in ChildInstanceTable.AsEnumerable()
                            on q.TokenID.ToString()
                            equals t3[Instance.InstanceContext.PropertyName_ParentActivityTokenId].ToString()
                            into tmp1
                         from instance in tmp1.DefaultIfEmpty()
                         orderby q.TokenID, q.SortTime
                         select new LogInfo
                         {
                             TokenID = q.TokenID,
                             WorkItemID = q.WorkItemID.Trim(),
                             Participant = instance != null ? instance[Instance.InstanceContext.PropertyName_Originator] + string.Empty : q.Participant,
                             ParticipantName = instance != null ? instance[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty : q.ParticipantName,
                             ActivityCode = q.ActivityCode,
                             ActivityName = q.ActivityName,
                             CreatedTime = q.CreatedTime,
                             FinishedTime = q.FinishedTime,
                             Approval = instance != null ? instance[Instance.InstanceContext.PropertyName_Approval] + string.Empty : q.Approval,
                             Retrievable = q.Retrievable,
                             Exceptional = q.Exceptional,
                             UsedTime = q.UsedTime,
                             OrgUnit = instance != null ? instance[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty : q.OrgUnit,
                             Delegant = q.Delegant.Trim(),
                             Finisher = q.Finisher.Trim(),
                             FinisherName = q.FinisherName,
                             ItemType = q.ItemType,
                             Creator = q.Creator.Trim(),
                             //Forwarder = q.Forwarder.Trim(),
                             State = q.State,
                             WorkflowCode = q.WorkflowCode,
                             InstanceId = q.InstanceId,
                             InstanceState = instance != null ? instance[Instance.InstanceContext.PropertyName_State] + string.Empty : string.Empty,
                             ChildInstanceID = instance != null ? instance[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty : string.Empty
                         };
            return result;
        }

        /// <summary>
        /// 获取操作描述名称
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        public string GetItemAction(WorkItem.WorkItemType itemType)
        {
            string itemTypeAction = null;
            switch (itemType)
            {
                case WorkItem.WorkItemType.Circulate:
                    itemTypeAction = Configs.Global.ResourceManager.GetString("WorkItemType_Circulate");//传阅;
                    break;
                case WorkItem.WorkItemType.WorkItemAssist:
                case WorkItem.WorkItemType.ActivityAssist:
                    itemTypeAction = Configs.Global.ResourceManager.GetString("SheetActionPane_Assist");//协办;
                    break;
                case WorkItem.WorkItemType.WorkItemConsult:
                    itemTypeAction = Configs.Global.ResourceManager.GetString("WorkItemType_Consult"); //征询
                    break;
                case WorkItem.WorkItemType.ActivityConsult:
                    itemTypeAction = Configs.Global.ResourceManager.GetString("WorkItemType_Consult"); //征询
                    break;
                case WorkItem.WorkItemType.ActivityForward:
                    itemTypeAction = Configs.Global.ResourceManager.GetString("SheetActionPane_Forward");//转发;
                    break;
                case WorkItem.WorkItemType.Approve:
                case WorkItem.WorkItemType.Assistive:
                case WorkItem.WorkItemType.Consult:
                case WorkItem.WorkItemType.Fill:
                case WorkItem.WorkItemType.Read:
                    break;
                default:
                    throw new NotImplementedException();
            }
            return itemTypeAction;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class LogInfo
    {
        public int TokenID { get; set; }
        public string WorkItemID { get; set; }
        public string Participant { get; set; }
        public string ParticipantName { get; set; }
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime FinishedTime { get; set; }
        public string Approval { get; set; }
        public bool Retrievable { get; set; }
        public bool Exceptional { get; set; }
        public string UsedTime { get; set; }
        public string OrgUnit { get; set; }
        public string Delegant { get; set; }
        public string Finisher { get; set; }
        public string FinisherName { get; set; }
        public string ItemType { get; set; }
        public string Creator { get; set; }
        public string State { get; set; }
        public string InstanceState { get; set; }
        public string ChildInstanceID { get; set; }
        public string ParentPropertyText { get; set; }
        public string WorkflowCode { get; set; }
        public string InstanceId { get; set; }
        //public string Forwarder { get; set; }

    }
    public class WorkItemInfo
    {
        public string Activity { get; set; }
        public List<string> Participants { get; set; }
        public List<string> Assistants { get; set; }
        public List<string> Consultants { get; set; }
        public List<string> Circulates { get; set; }
    }

    /// <summary>
    /// 流程活动日志类
    /// </summary>
    public class InstanceLogInfo
    {
        /// <summary>
        /// 获取或设置流程实例日志的编号
        /// </summary>
        public string No { get; set; }
        /// <summary>
        /// 获取或设置流程实例日志的活动编码
        /// </summary>
        public string ActivityCode { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的活动名称
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的接收时间
        /// </summary>
        public string CreatedTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的完成时间
        /// </summary>
        public string FinishedTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的使用时间
        /// </summary>
        public string UsedTime { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的参与者ID
        /// </summary>
        public string Participant { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的参与者Name
        /// </summary>
        public string ParticipantName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ParticipantOuName { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的完成者ID
        /// </summary>
        public string Finisher { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的完成者Name
        /// </summary>
        public string FinisherName { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的委托人ID
        /// </summary>
        public string Delegant { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的委托人ID
        /// </summary>
        public string DelegantName { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的其他人ID（协办，征询，传阅）
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的其他人ID（转发）
        /// </summary>
        //public string Forwarder { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的其他人Name（协办，征询，传阅）
        /// </summary>
        public string CreatorName { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的操作
        /// </summary>
        public string Approval { get; set; }
        /// <summary>
        /// 获取或设置流程实例活动的操作名称
        /// </summary>
        public string ApprovalName { get; set; }
        /// <summary>
        /// 获取或设置流程实例子流程ID
        /// </summary>
        public string ChildInstanceId { get; set; }
        /// <summary>
        /// 获取或设置流程实例日志的操作
        /// </summary>
        public string Retrievable { get; set; }
        /// <summary>
        /// 获取或设置流程实例日志的异常
        /// </summary>
        public string Exceptional { get; set; }

        /// <summary>
        /// 获取或设置日志头像图片
        /// </summary>
        public string ParticipantImageURL { get; set; }
        /// <summary>
        /// 审批意见
        /// </summary>

        public string Comments { get; set; }
        /// <summary>
        /// 签章ID
        /// </summary>
        public string SignatureId { get; set; }
        /// <summary>
        /// 流程状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 转办审批意见
        /// </summary>
        public string ParentPropertyText { get; set; }
    }

    /// <summary>
    /// 流程实例用户操作日志类
    /// </summary>
    public class UserLogInfo
    {
        public string No { get; set; }
        public string Time { get; set; }
        public string Type { get; set; }
        public string User { get; set; }
        public string TargetName { get; set; }
        public string ClientPlatform { get; set; }
        public string ClientAddress { get; set; }

    }
    /// <summary>
    /// 催办类
    /// </summary>
    public class UrgeInstance
    {
        public string UrgeUserName { get; set; }
        public string UrgeTime { get; set; }
        public string UrgeContent { get; set; }
    }

    /// <summary>
    /// 获取流程实例的活动节点信息类
    /// </summary>
    public class InstanceActivity
    {
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
    }
}
