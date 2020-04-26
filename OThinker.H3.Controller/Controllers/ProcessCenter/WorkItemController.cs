
using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Data;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    //[ActionXSSFillters]
    public class WorkItemController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WorkItemController()
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

        #region  待办、待阅、我的流程数量  —————————————
        /// <summary>
        /// 获取待办、待阅、我的流程数量
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkCount()
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                Dictionary<string, int> Extend = new Dictionary<string, int>();
                //待办
                string[] conditions1 = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, string.Empty, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.WorkItem.TableName);
                //待阅
                string[] conditions2 = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, string.Empty, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.CirculateItem.TableName);
                // 我的流程（未完成）
                string[] conditions3 = this.Engine.PortalQuery.GetInstanceConditions(
                    new string[] { this.UserValidator.UserID },
                    null,
                    Instance.InstanceContext.UnspecifiedID,
                    InstanceState.Unfinished,
                    null,
                    WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    string.Empty,
                    OThinker.H3.Instance.PriorityType.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified);

                int total1 = Engine.PortalQuery.CountWorkItem(conditions1, WorkItem.WorkItem.TableName); // 待办总数
                int total2 = Engine.PortalQuery.CountWorkItem(conditions2, WorkItem.CirculateItem.TableName); // 待阅总数
                int total3 = Engine.PortalQuery.CountInstance(conditions3); // 我的流程总数

                Extend["UnfinishedWorkItemCount"] = total1;
                Extend["UnreadWorkItemCount"] = total2;
                Extend["MyWorkflowCount"] = total3;
                result.Extend = Extend;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        #endregion

        #region  待办任务，已办任务  ————————————————
        /// <summary>
        /// 查询用户的待办
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public JsonResult GetUnfinishWorkItems(PagerInfo pagerInfo, string keyWord)
        {

            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, keyWord, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.WorkItem.TableName);
                string OrderBy = "ORDER BY " +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, WorkItem.WorkItem.TableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItem.TableName); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyUnfinishedWorkItem_Code);
        }


        /// <summary>
        /// 获取首页的待办
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="startIndex"></param>
        /// <param name="endIndex"></param>
        /// <returns></returns>
        public JsonResult GetDefaultUnfinishWorkItems(PagerInfo pagerInfo, int startIndex, int endIndex)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, string.Empty, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.WorkItem.TableName);
                string OrderBy = "ORDER BY " +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";

                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, startIndex, endIndex, OrderBy, WorkItem.WorkItem.TableName);

                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(endIndex + 1, griddata, 0);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyUnfinishedWorkItem_Code);
        }

        /// <summary>
        /// 分组模式-待办
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="keyWord">流程模板名称</param>
        /// <returns></returns>
        public JsonResult MyUnfinishedWorkItemByGroup()
        {
            return this.ExecuteFunctionRun(() =>
            {
                string orderBy = "ORDER BY " +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, string.Empty, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.WorkItem.TableName);
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, -1, -1, orderBy, WorkItem.WorkItem.TableName);
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtWorkitem, columns);
                Dictionary<string, string> workflowNames = this.GetWorkflowNamesFromTable(dtWorkitem);
                Dictionary<string, WorkItemGroup> workflowGroups = new Dictionary<string, WorkItemGroup>();
                // 获取分组集合
                foreach (DataRow row in dtWorkitem.Rows)
                {
                    string workflowCode = row[WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty;
                    if (!workflowGroups.ContainsKey(workflowCode))
                    {
                        string workflowName = workflowNames[workflowCode] + string.Empty;
                        workflowGroups.Add(workflowCode, new WorkItemGroup(workflowCode, workflowName));
                    }

                    var workItem = new WorkItemViewModel()
                    {
                        Priority = row[WorkItem.WorkItem.PropertyName_Priority] + string.Empty,
                        Urged = this.GetColumnsValue(row, WorkItem.WorkItem.PropertyName_Urged).ToString() == "1" ? true : false,
                        ObjectID = row[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty,
                        InstanceId = row[WorkItem.WorkItem.PropertyName_InstanceId] + string.Empty,
                        WorkflowCode = workflowCode,
                        InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty,
                        ActivityCode = row[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty,
                        DisplayName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                        ReceiveTime = this.GetValueFromDate(row[WorkItem.WorkItem.PropertyName_ReceiveTime], WorkItemTimeFormat),
                        Originator = row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty,
                        OriginatorName = row[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty,
                        OriginatorOUName = this.GetValueFromDictionary(unitNames, row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty)
                    };
                    workflowGroups[workflowCode].WorkItems.Add(workItem);
                }

                List<WorkItemGroup> gridDatas = new List<WorkItemGroup>();
                foreach (string key in workflowGroups.Keys)
                {
                    gridDatas.Add(workflowGroups[key]);
                }

                GridViewModel<WorkItemGroup> result = new GridViewModel<WorkItemGroup>(workflowGroups.Count, gridDatas);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyUnfinishedWorkItem_Code);
        }

        /// <summary>
        /// 批量审批-待办
        /// </summary>
        /// <returns></returns>
        public JsonResult MyUnfinishedWorkItemByBatch(PagerInfo pagerInfo, string keyWord)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetBatchUnfinishedWorkItemConditions(this.UserValidator.UserID, keyWord);
                string orderBy = "ORDER BY " +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";
                DataTable dtWorkflowItem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, orderBy, WorkItem.WorkItem.TableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItem.TableName); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkflowItem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyUnfinishedWorkItem_Code);
        }

        /// <summary>
        /// 查询用户的已办
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="workflowCode"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public JsonResult GetFinishWorkItems(PagerInfo pagerInfo, DateTime? startTime, DateTime? endTime, string workflowCode, string instanceName)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    WorkItem.WorkItemState.Finished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    true,
                    WorkItem.WorkItemFinished.TableName);
                string OrderBy = "ORDER BY " +
                     WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC";
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, WorkItem.WorkItemFinished.TableName);
                foreach (DataRow item in dtWorkitem.Rows)
                {
                    if (int.Equals(item["ItemType"], (int)(WorkItem.WorkItemType.ActivityForward)))
                    {
                        item["DisplayName"] = "转办";
                    }
                }
                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItemFinished.TableName); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItemFinished.PropertyName_OrgUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyFinishedWorkItem_Code);
        }

        #endregion

        #region 流程查询——————————————————————

        /// <summary>
        /// 查询任务，不同状态的任务处于不同的表
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="instanceName"></param>
        /// <param name="workflowCode"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="userName"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public JsonResult QueryParticipantWorkItems(PagerInfo pagerInfo, string instanceName, string workflowCode, string participant, DateTime? startTime, DateTime? endTime, int state)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<WorkItemViewModel> griddata = new List<WorkItemViewModel>();
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(0, griddata, pagerInfo.sEcho);
                // 验证权限
                string[] workflowCodeArray = string.IsNullOrEmpty(workflowCode) ? null : workflowCode.Split(',');
                string[] participantArray = string.IsNullOrEmpty(participant) ? null : participant.Split(',');
                var queryAcl = this.GetQueryAcl(ref workflowCodeArray, ref participantArray);
                if (workflowCodeArray != null)
                {
                    workflowCode = string.Join(",", workflowCodeArray);
                }
                if (participantArray != null)
                {
                    participant = string.Join(",", participantArray);
                }

                if (!queryAcl.Success && (workflowCodeArray == null || workflowCodeArray.Length == 0) && (participantArray == null || participantArray.Length == 0))
                {
                    result.ExtendProperty = queryAcl;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //if (string.IsNullOrEmpty(participant))
                //{
                //    var extend = new
                //    {
                //        Success = false,
                //        Message = "QueryParticipantWorkItem_NoParticipant"
                //    };
                //    result.ExtendProperty = extend;
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}
                //if (participant == this.UserValidator.User.ObjectID)
                //{ }
                //else if (this.UserValidator.ValidateAdministrator())
                //{ }
                //else if (this.UserValidator.ValidateOrgView(participant))
                //{ }
                //else if (!string.IsNullOrEmpty(workflowCode) &&
                //   this.UserValidator.ValidateWFInsView(workflowCode,
                //   participant))
                //{ }
                //else
                //{
                //    var extend = new
                //    {
                //        Success = false,
                //        Message = "DataFilter_NotEnoughAuth"
                //    };
                //    result.ExtendProperty = extend;
                //    return Json(result, JsonRequestBehavior.AllowGet);
                //}

                WorkItem.WorkItemState workitemState = GetWorkItemState(state);
                string tableName = "";
                switch (workitemState)
                {
                    //此处取消 的表取确认
                    case WorkItem.WorkItemState.Unfinished: tableName = WorkItem.WorkItem.TableName; break;
                    case WorkItem.WorkItemState.Finished: tableName = WorkItem.WorkItemFinished.TableName; break;
                    default: tableName = WorkItem.WorkItemFinished.TableName; break;
                }

                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(participant,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    workitemState, instanceName, OThinker.Data.BoolMatchValue.Unspecified, workflowCode, true, tableName);

                string orderBy = "";
                switch (workitemState)
                {
                    //case 0: state = WorkItem.WorkItemState.Unfinished; break;
                    case WorkItem.WorkItemState.Unfinished: orderBy = " ORDER BY " + tableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC "; break;
                    case WorkItem.WorkItemState.Finished: orderBy = " ORDER BY " + tableName + "." + WorkItem.WorkItem.PropertyName_FinishTime + " DESC "; break;
                    case WorkItem.WorkItemState.Canceled: orderBy = " ORDER BY " + tableName + "." + WorkItem.WorkItem.PropertyName_FinishTime + " DESC "; break;
                }
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, orderBy, tableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, tableName); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit, WorkItem.WorkItem.PropertyName_WorkflowCode };
                griddata = this.Getgriddata(dtWorkitem, columns);
                result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);

                if (!queryAcl.Success)
                {
                    result.ExtendProperty = queryAcl;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 查询超时任务
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="instanceName"></param>
        /// <param name="workflowCode"></param>
        /// <param name="participant"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public JsonResult QueryElapsedWorkItem(PagerInfo pagerInfo, string instanceName, string workflowCode, string participant, DateTime? startTime, DateTime? endTime)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<WorkItemViewModel> griddata = new List<WorkItemViewModel>();
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(0, griddata, pagerInfo.sEcho);


                string[] workflowCodeArray = string.IsNullOrEmpty(workflowCode) ? null : workflowCode.Split(',');
                string[] participantArray = string.IsNullOrEmpty(participant) ? null : participant.Split(',');
                var queryAcl = this.GetQueryAcl(ref workflowCodeArray, ref participantArray);
                if (workflowCodeArray != null)
                {
                    workflowCode = string.Join(",", workflowCodeArray);
                }
                if (participantArray != null)
                {
                    participant = string.Join(",", participantArray);
                }

                if (!queryAcl.Success && (workflowCodeArray == null || workflowCodeArray.Length == 0) && (participantArray == null || participantArray.Length == 0))
                {
                    result.ExtendProperty = queryAcl;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string[] orgs = string.IsNullOrWhiteSpace(participant) ? this.UserValidator.ViewableOrgs : participant.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                instanceName = instanceName == null ? null : instanceName.Trim().Replace("'", "").Replace("--", "");
                string[] conditions = Engine.PortalQuery.GetQueryWorkItemConditionsForMonitor(
                    orgs,
                    workflowCode,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    instanceName,
                    WorkItem.WorkItemState.Unfinished,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    OThinker.Data.BoolMatchValue.True);
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, string.Empty, WorkItem.WorkItem.TableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItem.TableName); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                griddata = this.Getgriddata(dtWorkitem, columns);
                result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);

                if (!queryAcl.Success)
                {
                    result.ExtendProperty = queryAcl;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        #endregion

        #region 批量审批——————————————————————

        public JsonResult HandleWorkItemByBatch(bool Approval, string[] WorkItemIDs, string CommentText)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string result = "";

                if (Approval)
                {
                    result = SubmitWorkItems(WorkItemIDs, CommentText);
                }
                else
                {
                    result = ReturnWorkItems(WorkItemIDs, CommentText);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 批量通过
        /// </summary>
        /// <param name="WorkItemIDs"></param>
        /// <param name="CommentText"></param>
        /// <returns></returns>
        private string SubmitWorkItems(string[] WorkItemIDs, string CommentText)
        {
            string errors = null;
            foreach (string itemId in WorkItemIDs)
            {
                OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(itemId);
                if (item == null || !item.IsUnfinished)
                {
                    continue;
                }
                if (item.BatchProcessing == false)
                {
                    errors += item.DisplayName + ";";
                    continue;
                }

                // 添加意见
                this.AppendComment(item, OThinker.Data.BoolMatchValue.True, CommentText);

                // 结束工作项
                this.Engine.WorkItemManager.FinishWorkItem(
                    itemId,
                    this.UserValidator.UserID,
                    WorkItem.AccessPoint.Batch,
                    //null,
                    null,
                    OThinker.Data.BoolMatchValue.True,
                    CommentText,
                    null,
                    WorkItem.ActionEventType.Forward,
                    (int)SheetButtonType.Submit);
                // 需要通知实例事件管理器结束事件
                Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                        Messages.MessageEmergencyType.Normal,
                        item.InstanceId,
                        item.ActivityCode,
                        item.TokenId,
                        OThinker.Data.BoolMatchValue.True,
                        false,
                        OThinker.Data.BoolMatchValue.True,
                        true,
                        null);
                this.Engine.InstanceManager.SendMessage(endMessage);
            }
            return errors;
        }

        /// <summary>
        /// 批量拒绝
        /// </summary>
        /// <param name="WorkItemIDs"></param>
        /// <param name="CommentText"></param>
        /// <returns></returns>
        private string ReturnWorkItems(string[] WorkItemIDs, string CommentText)
        {
            string errors = null;
            foreach (string itemId in WorkItemIDs)
            {
                WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(itemId);

                if (item == null || !item.IsUnfinished)
                {
                    continue;
                }
                if (item.BatchProcessing == false)
                {
                    errors += item.DisplayName + ";";
                    continue;
                }
                WorkflowTemplate.PublishedWorkflowTemplate workflow = GetWorkflowTemplate(item.WorkflowCode, item.WorkflowVersion);
                // 添加意见
                this.AppendComment(item, OThinker.Data.BoolMatchValue.True, CommentText);

                // 结束工作项
                this.Engine.WorkItemManager.FinishWorkItem(
                      item.ObjectID,
                      this.UserValidator.UserID,
                      H3.WorkItem.AccessPoint.ExternalSystem,
                    //null,
                      null,
                      OThinker.Data.BoolMatchValue.False,
                      CommentText,
                      null,
                      H3.WorkItem.ActionEventType.Backward,
                      (int)SheetButtonType.Return);
                // 准备触发后面Activity的消息
                OThinker.H3.Messages.ActivateActivityMessage activateMessage
                    = new OThinker.H3.Messages.ActivateActivityMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    workflow.StartActivityCode,
                    OThinker.H3.Instance.Token.UnspecifiedID,
                    null,
                    new int[] { item.TokenId },
                    false,
                    H3.WorkItem.ActionEventType.Backward);

                // 通知该Activity已经完成
                OThinker.H3.Messages.AsyncEndMessage endMessage =
                    new OThinker.H3.Messages.AsyncEndMessage(
                        OThinker.H3.Messages.MessageEmergencyType.Normal,
                        item.InstanceId,
                        item.ActivityCode,
                        item.TokenId,
                        OThinker.Data.BoolMatchValue.False,
                        true,
                        OThinker.Data.BoolMatchValue.False,
                        false,
                        activateMessage);
                this.Engine.InstanceManager.SendMessage(endMessage);
            }
            return errors;
        }


        private void AppendComment(WorkItem.WorkItem Item, OThinker.Data.BoolMatchValue Approval, string Comment)
        {
            WorkflowTemplate.PublishedWorkflowTemplate workflow = GetWorkflowTemplate(Item.WorkflowCode, Item.WorkflowVersion);
            // 审批字段
            string approvalDataItem = null;
            if (workflow != null)
            {
                PublishedWorkflowTemplate workflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplate(Item.WorkflowCode, Item.WorkflowVersion);
                OThinker.H3.DataModel.BizObjectSchema schema = GetSchema(workflowTemplate.BizObjectSchemaCode);
                approvalDataItem = workflow.GetDefaultCommentDataItem(schema, Item.ActivityCode);
            }
            if (approvalDataItem != null)
            {
                // 创建审批
                //TODO:此处待修改，Item.BizObjectId不存在,通过 OT_InstanceContext获取
                Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(Item.InstanceId);
                string BizObjectId = InstanceContext == null ? string.Empty : InstanceContext.BizObjectId;

                Data.Comment comment = new Data.Comment();
                comment.Activity = Item.ActivityCode;
                comment.Approval = Approval;
                comment.CreatedTime = System.DateTime.Now;
                comment.DataField = approvalDataItem;
                comment.InstanceId = Item.InstanceId;
                comment.OUName = this.Engine.Organization.GetName(this.Engine.Organization.GetParent(Item.Participant));
                comment.Text = Comment;
                comment.TokenId = Item.TokenId;
                comment.UserID = Item.Participant;
                comment.UserName = Item.ParticipantName;
                //comment.BizObjectId = Item.BizObjectId;
                comment.BizObjectId = BizObjectId;
                comment.BizObjectSchemaCode = Item.WorkflowCode;
                this.Engine.BizObjectManager.AddComment(comment);
            }
        }

        protected Dictionary<string, WorkflowTemplate.PublishedWorkflowTemplate> WorkflowTemplates = new Dictionary<string, WorkflowTemplate.PublishedWorkflowTemplate>();
        protected Dictionary<string, OThinker.H3.DataModel.BizObjectSchema> Schemas = new Dictionary<string, DataModel.BizObjectSchema>();
        private WorkflowTemplate.PublishedWorkflowTemplate GetWorkflowTemplate(string workflowCode, int workflowVersion)
        {
            WorkflowTemplate.PublishedWorkflowTemplate workflow = null;
            if (!WorkflowTemplates.ContainsKey(workflowCode + "." + workflowVersion))
            {
                // 添加一个审批意见
                workflow = this.Engine.WorkflowManager.GetPublishedTemplate(
                    workflowCode,
                    workflowVersion);
                WorkflowTemplates.Add(workflowCode + "." + workflowVersion, workflow);
            }
            else
            {
                workflow = WorkflowTemplates[workflowCode + "." + workflowVersion];
            }
            return workflow;
        }
        private OThinker.H3.DataModel.BizObjectSchema GetSchema(string schemaCode)
        {
            OThinker.H3.DataModel.BizObjectSchema schema = null;
            if (!Schemas.ContainsKey(schemaCode))
            {
                schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                Schemas.Add(schemaCode, schema);
            }
            else
            {
                schema = Schemas[schemaCode];
            }
            return schema;
        }

        #endregion


        #region 应用中心——任务列表————————————————
        public JsonResult GetMyWorkItem(PagerInfo pagerInfo, string FunctionCode, int State, string WorkflowCode, DateTime? StartTime, DateTime? EndTime)
        {
            return this.ExecuteFunctionRun(() =>
            {
                WorkItem.WorkItemState workitemState = GetWorkItemState(State);
                total_Workitem total_workitem = this.GetWorkItemTable(pagerInfo, workitemState, WorkflowCode,
                    StartTime, EndTime);
                DataTable dtWorkitem = total_workitem.dt;
                int total = total_workitem.total;
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        public class total_Workitem
        {
            public int total { get; set; }
            public DataTable dt { get; set; }
        }
        private total_Workitem GetWorkItemTable(PagerInfo pagerInfo, WorkItem.WorkItemState State, string WorkflowCode, DateTime? StartTime, DateTime? EndTime)
        {
            total_Workitem total_workitem = new total_Workitem();
            if (State == WorkItem.WorkItemState.Unspecified)
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID,
                    StartTime == null ? DateTime.MinValue : StartTime.Value,
                    EndTime == null ? DateTime.MaxValue : EndTime.Value,
                    State,
                    string.Empty,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    WorkflowCode,
                    true,
                    WorkItem.WorkItem.TableName);
                string OrderBy = "ORDER BY " +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, WorkItem.WorkItem.TableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItem.TableName); // 记录总数

                string[] conditions1 = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID,
                   StartTime == null ? DateTime.MinValue : StartTime.Value,
                   EndTime == null ? DateTime.MaxValue : EndTime.Value,
                   State,
                   string.Empty,
                   OThinker.Data.BoolMatchValue.Unspecified,
                   WorkflowCode,
                   true,
                   WorkItem.WorkItemFinished.TableName);
                string OrderBy1 = "ORDER BY " +
                     WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_ReceiveTime + " DESC";
                DataTable dtWorkitem1 = Engine.PortalQuery.QueryWorkItem(conditions1, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy1, WorkItem.WorkItemFinished.TableName);
                total = total + Engine.PortalQuery.CountWorkItem(conditions1, WorkItem.WorkItemFinished.TableName); // 记录总数
                if (dtWorkitem != null)
                {
                    dtWorkitem.Merge(dtWorkitem1);
                }
                else
                {
                    dtWorkitem = dtWorkitem1;
                }

                total_workitem.total = total;
                total_workitem.dt = dtWorkitem;
                return total_workitem;
            }
            else
            {
                string TableName = State == WorkItem.WorkItemState.Finished ? WorkItem.WorkItemFinished.TableName : WorkItem.WorkItem.TableName;
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID,
                    StartTime == null ? DateTime.MinValue : StartTime.Value,
                    EndTime == null ? DateTime.MaxValue : EndTime.Value,
                    State,
                    string.Empty,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    WorkflowCode,
                    true,
                    TableName);
                string OrderBy = "ORDER BY " +
                     WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_Priority + " DESC," +
                     WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_ReceiveTime + " DESC";
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, TableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, TableName); // 记录总数
                var result = new
                {
                    total = total,
                    dtWorkitem = dtWorkitem
                };
                total_workitem.total = total;
                total_workitem.dt = dtWorkitem;
                return total_workitem;
            }
        }
        #endregion

        #region 其他————————————————————————

        /// <summary>
        /// 获取返回到前端的WorkItemViewModel
        /// </summary>
        /// <param name="dtWorkItem"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<WorkItemViewModel> Getgriddata(DataTable dtWorkItem, string[] columns)
        {
            //
            string InstanceName = string.Empty;
            //批量获取组织架构显示名称
            Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtWorkItem, columns);
            //批量获取流程发起人的OU显示名称
            Dictionary<string, string> orgOUNames = this.GetParentUnitNamesFromTable(dtWorkItem,
                new string[] { WorkItem.WorkItem.PropertyName_Originator });
            //批量获取流程模板名称WorkflowName
            Dictionary<string, string> workflowNames = this.GetWorkflowNamesFromTable(dtWorkItem);
            List<WorkItemViewModel> griddata = new List<WorkItemViewModel>();
            foreach (DataRow row in dtWorkItem.Rows)
            {
                if (string.IsNullOrEmpty(row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty))
                {
                    var workItem = this.Engine.WorkItemManager.GetWorkItem(this.GetColumnsValue(row, WorkItem.WorkItem.PropertyName_ObjectID));
                    InstanceName = this.Engine.WorkflowManager.GetClauseDisplayName(workItem.WorkflowCode);
                }
                else
                {
                    InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty;
                }
                griddata.Add(new WorkItemViewModel()
                {
                    Priority = this.GetColumnsValue(row, InstanceContext.PropertyName_Priority),
                    Urged = this.GetColumnsValue(row, WorkItem.WorkItem.PropertyName_Urged).ToString() == "1" ? true : false,
                    CirculateCreator = this.GetColumnsValue(row, WorkItem.CirculateItem.PropertyName_Creator),
                    CirculateCreatorName = this.GetColumnsValue(row, WorkItem.CirculateItem.PropertyName_CreatorName),
                    ObjectID = this.GetColumnsValue(row, WorkItem.WorkItem.PropertyName_ObjectID),
                    InstanceId = row[WorkItem.WorkItem.PropertyName_InstanceId] + string.Empty,
                    InstanceName = InstanceName,
                    WorkflowCode = row[WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty,
                    WorkflowName = this.GetValueFromDictionary(workflowNames,
                        row[Instance.InstanceContext.PropertyName_WorkflowCode] + string.Empty),
                    ActivityCode = row[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty,
                    DisplayName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                    ReceiveTime = this.GetValueFromDate(row[WorkItem.WorkItem.PropertyName_ReceiveTime], WorkItemTimeFormat),
                    PlanFinishTime = this.GetValueFromDate(row[WorkItem.WorkItem.PropertyName_PlanFinishTime], WorkItemTimeFormat),
                    FinishTime = this.GetValueFromDate(row[WorkItem.WorkItem.PropertyName_FinishTime], WorkItemTimeFormat),
                    StayTime = System.DateTime.Now.Subtract(this.GetTime(row[InstanceContext.PropertyName_PlanFinishTime] + string.Empty)),
                    Participant = row[WorkItem.WorkItem.PropertyName_Participant] + string.Empty,
                    ParticipantName = row[WorkItem.WorkItem.PropertyName_ParticipantName] + string.Empty,
                    Originator = row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty,
                    OriginatorName = row[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty,
                    OriginatorOUName = this.GetValueFromDictionary(orgOUNames,
                        row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty),
                    ItemSummary = row[WorkItem.WorkItem.PropertyName_ItemSummary] + string.Empty
                });
            }
            return griddata;
        }

        public string GetColumnsValue(DataRow row, string columns)
        {
            return row.Table.Columns.Contains(columns) ? row[columns] + string.Empty : "";
        }

        /// <summary>
        /// 获取查询任务的状态
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public WorkItem.WorkItemState GetWorkItemState(int value)
        {
            WorkItem.WorkItemState state = new WorkItem.WorkItemState();
            switch (value)
            {
                case 0: state = WorkItem.WorkItemState.Unfinished; break;
                case 1: state = WorkItem.WorkItemState.Finished; break;
                case 2: state = WorkItem.WorkItemState.Canceled; break;
                default: state = WorkItem.WorkItemState.Unspecified; break;
            }
            return state;
        }

        #endregion
        // End Class

    }
}