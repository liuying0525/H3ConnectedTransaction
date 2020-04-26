using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkItem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OThinker.H3.Portal
{
    public class WorkItemServer
    {
        #region 构造函数
        public WorkItemServer()
        {

        }
        #endregion

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

        private ActionContext _ActionContext;
        /// <summary>
        /// 获取环境对象
        /// </summary>
        public ActionContext ActionContext
        {
            get
            {
                return this._ActionContext;
            }
        }
        private HttpRequest _Request;
        /// <summary>
        /// 页面客户端请求
        /// </summary>
        public HttpRequest Request
        {
            get
            {
                return this._Request;
            }
        }

        public bool startWorkflow(
           string workflowCode,
           string userCode,
           bool finishStart,
           List<DataItemParam> paramValues)
        {
            //ValidateSoapHeader();
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;


            try
            {
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(workflowCode);
                if (workflowTemplate == null)
                {

                    return false;
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as Organization.User;
                if (user == null)
                {

                    return false;
                }

                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new DataModel.BizObject(
                    this.Engine.Organization,
                    this.Engine.MetadataRepository,
                    this.Engine.BizObjectManager,
                    this.Engine.BizBus,
                    schema,
                    OThinker.Organization.User.AdministratorID,
                    OThinker.Organization.OrganizationUnit.DefaultRootID);

                if (paramValues != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in paramValues)
                    {

                        if (bo.Schema.ContainsField(param.ItemName))
                        {
                            bo[param.ItemName] = param.ItemValue;
                        }
                    }
                }

                bo.Create();

                // 创建流程实例
                string InstanceId = this.Engine.InstanceManager.CreateInstance(
                     bo.ObjectID,
                     workflowTemplate.WorkflowCode,
                     workflowTemplate.WorkflowVersion,
                     null,
                     null,
                     user.UnitID,
                     null,
                     false,
                     Instance.InstanceContext.UnspecifiedID,
                     null,
                     Instance.Token.UnspecifiedID);

                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();

                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                    = new OThinker.H3.Messages.StartInstanceMessage(
                        emergency,
                        InstanceId,
                        null,
                        paramTables,
                        Instance.PriorityType.Normal,
                        true,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                Engine.InstanceManager.SendMessage(startInstanceMessage);

            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }


        #region 结束工作任务

        /// <summary>
        /// 结束工作任务
        /// </summary>
        public void ENDWorkItem(string UpdateUserId, string schemaCode, string workItemID, string STATE)
        {
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemID);

            if (!string.IsNullOrEmpty(schemaCode))
            {
                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine, schema, UpdateUserId);

                OThinker.H3.Instance.InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);

                if (!string.IsNullOrEmpty(workItemID))
                {
                    bo.ObjectID = ic.BizObjectId;
                    bo.Load();//装载流程数据；

                    if (!string.IsNullOrEmpty(STATE))
                    {
                        if (bo.Schema.ContainsField("LCZT"))
                        {
                            bo["LCZT"] = STATE;
                        }
                        if (bo.Schema.ContainsField("OperationStates"))
                        {
                            bo["OperationStates"] = STATE;
                        }
                        if (bo.Schema.ContainsField("states"))
                        {
                            bo["states"] = STATE;
                        }

                    }

                    bo.Update();
                }


            }

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                 workItemID,
                 UpdateUserId,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,
                null,
                OThinker.Data.BoolMatchValue.True,
                string.Empty,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                (int)OThinker.H3.Controllers.SheetButtonType.Submit);

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

        #endregion


        #region 取消当前节点激活到指定节点--财务部通过和驳回
        public bool ReturnItem(string userId, string workItemId, string activityCode, string SchemaCode, double xbzjye, string state, DateTime bzjyedysj, string userName, string PassState, double yzzfbzj)
        { 


            Organization.User user = this.Engine.Organization.GetUnit(userId) as Organization.User;
            if (user == null) return false;
            // 获取工作项

            OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine, schema, userId);
            bo.ObjectID = context.BizObjectId;
            bo.Load();//装载流程数据；

            if (bo.Schema.ContainsField("BZJYE"))//保证金余额
            {
                bo["BZJYE"] = xbzjye;
            }
            if (bo.Schema.ContainsField("LCZT"))
            {
                bo["LCZT"] = state;
            }
            if (bo.Schema.ContainsField("BZJYEDYSJ"))
            {
                bo["BZJYEDYSJ"] = bzjyedysj;
            }
            if (bo.Schema.ContainsField("CWCLR"))
            {
                bo["CWCLR"] = userName;
            }
            if (bo.Schema.ContainsField("CWSPRID"))
            {
                bo["CWSPRID"] = userId;
            }
            //this.AppendComment(item.InstanceId, item, OThinker.Data.BoolMatchValue.Unspecified, "现保证金余额x钱，应再支付X保证金");
            
           
           

            // 结束工作项
            if (PassState == "TG")
            {//财务通过
                this.Engine.WorkItemManager.FinishWorkItem(
                      item.ObjectID,
                      user.ObjectID,
                      H3.WorkItem.AccessPoint.ExternalSystem,
                      null,
                      OThinker.Data.BoolMatchValue.True,
                      string.Empty,
                      null,
                      H3.WorkItem.ActionEventType.Backward,
                      (int)OThinker.H3.Controllers.SheetButtonType.Return);

                if (bo.Schema.ContainsField("CWSPYJDX"))
                {
                    bo["CWSPYJDX"] = "通过";
                }

                this.AppendComment(context, item, OThinker.Data.BoolMatchValue.True, "通过");//财务部通过后，显示通过

            }
            else
            {//财务驳回
                this.Engine.WorkItemManager.FinishWorkItem(
                      item.ObjectID,
                      user.ObjectID,
                      H3.WorkItem.AccessPoint.ExternalSystem,
                      null,
                      OThinker.Data.BoolMatchValue.False,
                      string.Empty,
                      null,
                      H3.WorkItem.ActionEventType.Backward,
                      (int)OThinker.H3.Controllers.SheetButtonType.Return);
                var info = "现保证金余额" + string.Format("{0:N2}", xbzjye) + "元，应再支付" + string.Format("{0:N2}", yzzfbzj) + "保证金";
                this.AppendComment(context, item, OThinker.Data.BoolMatchValue.False, info);

                if (bo.Schema.ContainsField("CWSPYJDX"))
                {
                    bo["CWSPYJDX"] = "驳回";
                }

                try
                {
                    MessageClass ms = new MessageClass();

                    string sql = @"select distinct a.PARTICIPANT,b.code, d.objectid  , d.code jxsuserCode, e.JXS,e.JXSCODE from Ot_Workitemfinished  a
join Ot_User b on a.PARTICIPANT = b.objectid 
join Ot_Instancecontext c on c.objectid = a.instanceid 
join OT_User d on d.objectid = c.ORIGINATOR 
join I_DealerLoan e on e.objectid = c.bizobjectid
where   ACTIVITYCODE='Activity3' and c.workFlowCode = 'DealerLoan' and instanceid='" + item.InstanceId + "'";
                    DataTable dt = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

                    string msstr1 = dt.Rows[0]["JXS"].ToString() + "(" + dt.Rows[0]["JXSCODE"].ToString() + ") 还需要支付<font color=\"red\">" + string.Format("{0:N2}",yzzfbzj) + "</font> 元 保证金金额才可以进行本次贷款申请，现保证金账户余额 " + string.Format("{0:N2}",xbzjye) + " 元";

                    string msstr2 = "您的账户还需要支付<font color=\"red\">" + string.Format("{0:N2}",yzzfbzj) + "</font> 元 保证金金额才可以进行本次贷款申请，现保证金账户余额 " + string.Format("{0:N2}",xbzjye) + "元";

                    if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
                    {
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            ms.InsertMSG(dt.Rows[i]["PARTICIPANT"].ToString(), dt.Rows[i]["code"].ToString(), msstr1, true, 0, "");
                        }

                    }

                    ms.InsertMSG(dt.Rows[0]["objectid"].ToString(), dt.Rows[0]["jxsCode"].ToString(), msstr2, false, 0, "");

                }
                catch (Exception e)
                {
                    
                   
                }
               

            }

            bo.Update();
            // 准备触发后面Activity的消息
            OThinker.H3.Messages.ActivateActivityMessage activateMessage
                = new OThinker.H3.Messages.ActivateActivityMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,
                activityCode,
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


           


            return true;
        }
        #endregion

        #region 激活指定节点

        public bool ActiveToken(string instanceId, string activityCode, string[] participants)
        {
            try
            {
                // 准备触发后面Activity的消息
                OThinker.H3.Messages.ActivateActivityMessage activateMessage
                    = new OThinker.H3.Messages.ActivateActivityMessage(
                        OThinker.H3.Messages.MessageEmergencyType.Normal,
                        instanceId,
                        activityCode,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        participants,
                        null,
                        false,
                        H3.WorkItem.ActionEventType.Adjust);
                this.Engine.InstanceManager.SendMessage(activateMessage);
                return true;
            }
            catch (Exception)
            {
                throw;
            }

        }
        #endregion


        /// <summary>
        /// 给工作项添加审批意见
        /// </summary>
        /// <param name="item">工作项</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        private void AppendComment(OThinker.H3.Instance.InstanceContext Instance, OThinker.H3.WorkItem.WorkItem item, OThinker.Data.BoolMatchValue approval, string commentText)
        {
            // 添加一个审批意见
            WorkflowTemplate.PublishedWorkflowTemplate workflow = this.Engine.WorkflowManager.GetPublishedTemplate(
                item.WorkflowCode,
                item.WorkflowVersion);
            // 审批字段
            string approvalDataItem = null;
            if (workflow != null)
            {
                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(item.WorkflowCode);
                approvalDataItem = workflow.GetDefaultCommentDataItem(schema, item.ActivityCode);
            }
            if (approvalDataItem != null)
            {
                // 创建审批
                OThinker.H3.Data.Comment comment = new Data.Comment();
                comment.Activity = item.ActivityCode;
                comment.Approval = approval;
                comment.CreatedTime = System.DateTime.Now;
                comment.DataField = approvalDataItem;
                comment.InstanceId = item.InstanceId;
                comment.BizObjectId = Instance.BizObjectId;
                comment.BizObjectSchemaCode = Instance.BizObjectSchemaCode;
                comment.OUName = this.Engine.Organization.GetName(this.Engine.Organization.GetParent(item.Participant));
                comment.Text = commentText;
                comment.TokenId = item.TokenId;
                comment.UserID = item.Participant;
                comment.UserName = item.ParticipantName;

                // 设置用户的默认签章
                Organization.Signature[] signs = this.Engine.Organization.GetSignaturesByUnit(item.Participant);
                if (signs != null && signs.Length > 0)
                {
                    foreach (Organization.Signature sign in signs)
                    {
                        if (sign.IsDefault)
                        {
                            comment.SignatureId = sign.ObjectID;
                            break;
                        }
                    }
                }
                this.Engine.BizObjectManager.AddComment(comment);
            }
        }



        /// <summary>
        /// 获取最新的流程模板
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode)
        {
            // 获取最新版本号
            int workflowVersion = this.Engine.WorkflowManager.GetWorkflowDefaultVersion(workflowCode);
            return GetWorkflowTemplate(workflowCode, workflowVersion);
        }

        /// <summary>
        /// 获取指定版本号的流程模板对象
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="workflowVersion">流程模板版本号</param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode, int workflowVersion)
        {
            // 获取模板
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplateHeader(
                    workflowCode,
                    workflowVersion);
            return workflowTemplate;  
        }
        /// <summary>
        /// 提交工作项--授信管理
        /// </summary>
        /// <param name="workItemId">工作项ID</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        /// <param name="userId">处理人</param>
        public void SubmitItem(string workItemId, string userId)
        {
            // 获取工作项
            var item = this.Engine.WorkItemManager.GetCirculateItem(workItemId);// GetWorkItem(workItemId);
            //OThinker.H3.Instance.InstanceContext instance = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            //// 添加意见
            //this.AppendComment(instance, item, OThinker.Data.BoolMatchValue.Unspecified, commentText);

            // 结束工作项
            this.Engine.WorkItemManager.FinishCirculateItem(
                item.ObjectID,
                userId,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem);

            //// 需要通知实例事件管理器结束事件
            //Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
            //        Messages.MessageEmergencyType.Normal,
            //        item.InstanceId,
            //        item.ActivityCode,
            //        item.TokenId,
            //        approval,
            //        false,
            //        approval,
            //        true,
            //        null);
            //this.Engine.InstanceManager.SendMessage(endMessage);
        }

    }
}
