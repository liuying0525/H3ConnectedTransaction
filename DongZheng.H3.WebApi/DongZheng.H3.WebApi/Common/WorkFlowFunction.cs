using DongZheng.H3.WebApi.Models;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common
{
    public static class WorkFlowFunction
    {
        /// <summary>
        /// 设置批量流程数据项的值
        /// </summary>
        /// <param name="bizObjectSchemaCode"></param>
        /// <param name="bizObjectId"></param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        public static bool SetItemValues(string bizObjectSchemaCode, string bizObjectId, List<DataItemParam> keyValues, string userid)
        {
            if (keyValues == null || keyValues.Count == 0) return false;
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (DataItemParam param in keyValues)
            {
                values.Add(param.ItemName, param.ItemValue);
            }
            return AppUtility.Engine.BizObjectManager.SetPropertyValues(bizObjectSchemaCode, bizObjectId, userid, values);
        }

        public static string GetUserIDByCode(string UserCode)
        {
            var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(AppUtility.Engine, UserCode);
            return CurrentUserValidator.UserID;
        }

        /// <summary>
        /// 通过实例ID提交工作任务
        /// </summary>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public static bool SubmitWorkItemByInstanceID(string instanceid, string commentText)
        {
            string workItemId = getWorkItemIDByInstanceid(instanceid);
            if (!CommonFunction.hasData(workItemId))
            {
                //日志记录
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid提交任务失败，未找到任务ID，流程实例ID：" + instanceid);
                return false;
            }
            OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);
            SubmitItem(workItemId, OThinker.Data.BoolMatchValue.True, commentText, AppUtility.Engine.Organization.GetUserByCode("administrator").ObjectID);
            return true;
        }

        /// <summary>
        /// 结束流程
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        public static bool FinishedInstance(string instanceId)
        {
            try
            {
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid强制结束流程开始，流程实例ID：" + instanceId);
                OThinker.H3.Instance.InstanceContext context = AppUtility.Engine.InstanceManager.GetInstanceContext(instanceId);
                if (context == null) return false;

                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflow = AppUtility.Engine.WorkflowManager.GetDefaultWorkflow(context.WorkflowCode);

                OThinker.H3.Messages.ActivateActivityMessage activateMessage = new OThinker.H3.Messages.ActivateActivityMessage(
                        OThinker.H3.Messages.MessageEmergencyType.High,
                        instanceId,
                        workflow.EndActivityCode,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null,
                        null,
                        false,
                        OThinker.H3.WorkItem.ActionEventType.Adjust
                    );
                AppUtility.Engine.InstanceManager.SendMessage(activateMessage);
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid强制结束流程成功，流程实例ID：" + instanceId);
                return true;
            }
            catch
            {
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 根据instanceid强制结束流程失败，流程实例ID：" + instanceId);
                return false;
            }
        }

        public static string getWorkItemIDByInstanceid(string instanceid)
        {
            string rtn = "";
            string sql = "select OBJECTID from H3.OT_WORKITEM  where INSTANCEID='" + instanceid + "' ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.Rows[0][0].ToString();
            }
            return rtn;
        }

        /// <summary>
        /// 提交工作项
        /// </summary>
        /// <param name="workItemId">工作项ID</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        /// <param name="userId">处理人</param>
        private static void SubmitItem(string workItemId, OThinker.Data.BoolMatchValue approval, string commentText, string userId)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext instance = AppUtility.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // 添加意见
            AppendComment(instance, item, OThinker.Data.BoolMatchValue.Unspecified, commentText);

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
            OThinker.H3.Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    approval,
                    false,
                    approval,
                    true,
                    null);
            AppUtility.Engine.InstanceManager.SendMessage(endMessage);
        }



        /// <summary>
        /// 给工作项添加审批意见
        /// </summary>
        /// <param name="item">工作项</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        private static void AppendComment(OThinker.H3.Instance.InstanceContext Instance, OThinker.H3.WorkItem.WorkItem item, OThinker.Data.BoolMatchValue approval, string commentText)
        {
            // 添加一个审批意见
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflow = AppUtility.Engine.WorkflowManager.GetPublishedTemplate(
                item.WorkflowCode,
                item.WorkflowVersion);
            // 审批字段
            string approvalDataItem = null;
            if (workflow != null)
            {
                OThinker.H3.DataModel.BizObjectSchema schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema(item.WorkflowCode);
                approvalDataItem = workflow.GetDefaultCommentDataItem(schema, item.ActivityCode);
            }
            if (approvalDataItem != null)
            {
                // 创建审批
                OThinker.H3.Data.Comment comment = new OThinker.H3.Data.Comment();
                comment.Activity = item.ActivityCode;
                comment.Approval = approval;
                comment.CreatedTime = System.DateTime.Now;
                comment.DataField = approvalDataItem;
                comment.InstanceId = item.InstanceId;
                comment.BizObjectId = Instance.BizObjectId;
                comment.BizObjectSchemaCode = Instance.BizObjectSchemaCode;
                comment.OUName = AppUtility.Engine.Organization.GetName(AppUtility.Engine.Organization.GetParent(item.Participant));
                comment.Text = commentText;
                comment.TokenId = item.TokenId;
                comment.UserID = item.Participant;

                // 设置用户的默认签章
                OThinker.Organization.Signature[] signs = AppUtility.Engine.Organization.GetSignaturesByUnit(item.Participant);
                if (signs != null && signs.Length > 0)
                {
                    foreach (OThinker.Organization.Signature sign in signs)
                    {
                        if (sign.IsDefault)
                        {
                            comment.SignatureId = sign.ObjectID;
                            break;
                        }
                    }
                }
                AppUtility.Engine.BizObjectManager.AddComment(comment);
            }
        }



        public static string ReturnWorkItemByInstanceid(string instanceid, string userId, string commentText)
        {
            bool rtn = false;
            try
            {
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 驳回流程节点开始，流程实例ID：" + instanceid);
                //string userId = "ac845887-3bde-4580-ae0e-fe74149465e2";//融数风控
                string workItemId = getWorkItemIDByInstanceid(instanceid);
                OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);
                //手工节点不允许驳回
                if (item != null && item.ItemType == OThinker.H3.WorkItem.WorkItemType.Fill) return rtn.ToString();
                rtn = ReturnItem(userId, workItemId, commentText);
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 驳回流程节点结束，流程实例ID：" + instanceid);
                return rtn.ToString();

            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write(DateTime.Now.ToString() + " 驳回流程节点异常，流程实例ID：" + instanceid + "，异常信息：" + ex.Message);
                return rtn.ToString();
            }
        }
        private static bool ReturnItem(string userId, string workItemId, string commentText)
        {
            OThinker.Organization.User user = AppUtility.Engine.Organization.GetUnit(userId) as OThinker.Organization.User;
            if (user == null) return false;
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext context = AppUtility.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // ToKen
            OThinker.H3.Instance.IToken Token = context.GetToken(item.TokenId);
            if (Token.PreTokens == null) return false;
            int PreToken = int.Parse(Token.PreTokens[0].ToString());
            OThinker.H3.Instance.IToken PreToken1 = context.GetToken(PreToken);
            string activityName = PreToken1.Activity;
            // 添加意见
            AppendComment(context, item, OThinker.Data.BoolMatchValue.False, commentText);
            // 结束工作项
            AppUtility.Engine.WorkItemManager.FinishWorkItem(
                  item.ObjectID,
                  user.ObjectID,
                  OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                  null,
                  OThinker.Data.BoolMatchValue.False,
                  commentText,
                  null,
                  OThinker.H3.WorkItem.ActionEventType.Backward,
                  (int)OThinker.H3.Controllers.SheetButtonType.Return);
            // 准备触发后面Activity的消息
            OThinker.H3.Messages.ActivateActivityMessage activateMessage
                = new OThinker.H3.Messages.ActivateActivityMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,
                activityName,
                OThinker.H3.Instance.Token.UnspecifiedID,
                null,
                new int[] { item.TokenId },
                false,
                OThinker.H3.WorkItem.ActionEventType.Backward);

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
            AppUtility.Engine.InstanceManager.SendMessage(endMessage);
            return true;
        }


        public static string getRSWorkItemIDByInstanceid(string instanceid, string displayname)
        {
            string rtn = string.Empty;
            string sql = "select objectid from H3.OT_WORKITEM where displayname like '%" + displayname + "%' and instanceid='" + instanceid + "' ";
            DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (CommonFunction.hasData(dt))
            {
                rtn = dt.Rows[0][0].ToString();
            }
            return rtn;
        }
    }
}