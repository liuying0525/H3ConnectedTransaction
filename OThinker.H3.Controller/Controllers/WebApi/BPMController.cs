using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace OThinker.H3.Controllers.Controllers.WebApi
{
    /// <summary>
    /// BPM服务接口
    /// </summary>
    [AllowAnonymous]
    public class BPMController : Controller
    {
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (this._Engine == null)
                {
                    OThinker.H3.Connection conn = new Connection();
                    conn.Open(ConfigurationManager.AppSettings["BPMEngine"]);
                    _Engine = conn.Engine;
                }
                return _Engine;
            }
        }

        private System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        /// <summary>
        /// 获取JOSN序列化对象
        /// </summary>
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (jsSerializer == null)
                {
                    jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return jsSerializer;
            }
        }

        public System.Web.Mvc.ActionResult Index()
        {
            return Json("BPM接口服务.", JsonRequestBehavior.AllowGet);
        }

        #region 测试用服务

        public JsonResult CreateBO(string id, string name, int age)
        {
            System.Threading.Thread.Sleep(20000);
            Console.WriteLine("BO Created : id=" + id);
            ActionResult result = new ActionResult();
            result.Success = true;
            result.Message = "成功";
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetBOS(string id, string name)
        {
            ActionResult result = new ActionResult();
            result.Success = true;
            result.Message = "成功";
            List<ActionResult> list = new List<ActionResult>();
            for (int i = 0; i < 10; i++)
            {
                ActionResult r = new ActionResult(false, "我们");
                if (i % 2 == 0)
                {
                    r.Success = true;
                    r.Message = "你们" + i;
                }

                list.Add(r);
            }
            if (!string.IsNullOrEmpty(name))
                list = list.Where(s => s.Message.Contains(name)).ToList();
            if (!string.IsNullOrEmpty(id))
                list = list.Where(s => s.Message.Contains(id)).ToList();

            result.Extend = list;
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult GetBOS()
        {
            ActionResult result = new ActionResult();
            result.Success = true;
            result.Message = "成功";
            List<ActionResult> list = new List<ActionResult>();
            for (int i = 0; i < 10; i++)
            {
                ActionResult r = new ActionResult(false, "我们");
                if (i % 2 == 0)
                {
                    r.Success = true;
                    r.Message = "你们" + i;
                }

                list.Add(r);
            }

            result.Extend = list;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region 新的服务 ----------------------------------
        /// <summary>
        /// 启动流程实例
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode"></param>
        /// <param name="workflowCode"></param>
        /// <param name="finishStart"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        [HttpPost]
        public System.Web.Mvc.ActionResult StartWorkflow(string appId, string pwd, string userCode, string workflowCode,
            bool finishStart, string paramValues)
        {
            BPMServiceResult result = new BPMServiceResult();

            List<DataItemParam> listParams = JSSerializer.Deserialize<List<DataItemParam>>(paramValues);
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;

            try
            {
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflowTemplate = this.Engine.WorkflowManager.GetDefaultWorkflow(workflowCode);
                if (workflowTemplate == null)
                {
                    result = new BPMServiceResult(false, "流程模板不存在，模板编码:" + workflowCode + "。");
                    return Json(result);
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
                if (user == null)
                {
                    result = new BPMServiceResult(false, "用户{" + userCode + "}不存在。");
                    return Json(result);
                }

                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(
                    this.Engine.Organization,
                    this.Engine.MetadataRepository,
                    this.Engine.BizObjectManager,
                    null,
                    schema,
                    user.ObjectID,
                    user.ParentID);

                if (listParams != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in listParams)
                    {
                        OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.ItemName);
                        if (property == null) continue;
                        SetItemValue(bo, property, param.ItemValue);
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
                     InstanceContext.UnspecifiedID,
                     null,
                     Token.UnspecifiedID);

                if (listParams != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in listParams)
                    {
                        OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.ItemName);
                        if (property == null) continue;
                        if (property.LogicType == DataLogicType.Comment)
                        {// 审核意见
                            CommentParam comment = JSSerializer.Deserialize<CommentParam>(param.ItemValue + string.Empty);
                            this.Engine.BizObjectManager.AddComment(new Comment()
                            {
                                BizObjectId = bo.ObjectID,
                                BizObjectSchemaCode = schema.SchemaCode,
                                InstanceId = InstanceId,
                                Activity = workflowTemplate.StartActivityCode,
                                TokenId = 1,
                                Approval = OThinker.Data.BoolMatchValue.True,
                                DataField = property.Name,
                                UserID = user.ObjectID,
                                SignatureId = comment.SignatureId,
                                Text = comment.Text
                            });
                        }
                    }
                }

                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                    = new OThinker.H3.Messages.StartInstanceMessage(
                        MessageEmergencyType.Normal,
                        InstanceId,
                        workItemID,
                        null,
                        PriorityType.Normal,
                        true,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                Engine.InstanceManager.SendMessage(startInstanceMessage);
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", string.Empty);
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, ex.ToString());
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 获取流程相关信息
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode"></param>
        /// <param name="workItemId"></param>
        /// <returns></returns>
        [HttpPost]
        public System.Web.Mvc.ActionResult GetInstanceInfo(string appId, string pwd, string userCode, string workItemId)
        {
            OThinker.H3.WorkItem.WorkItem workItem = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (workItem == null) throw new Exception("工作任务不存在");
            OThinker.H3.Instance.InstanceContext instance = this.Engine.InstanceManager.GetInstanceContext(workItem.InstanceId);

            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate template = this.Engine.WorkflowManager.GetPublishedTemplate(instance.WorkflowCode, instance.WorkflowVersion);
            OThinker.H3.WorkflowTemplate.Activity activity = template.GetActivityByCode(workItem.ActivityCode);

            InstanceInfo result = new InstanceInfo()
            {
                ActivityCode = activity.ActivityCode,
                ActivityName = activity.DisplayName,
                DataPermissions = (activity is OThinker.H3.WorkflowTemplate.ParticipativeActivity) ? ((OThinker.H3.WorkflowTemplate.ParticipativeActivity)activity).DataPermissions : null,
                InstanceId = instance.InstanceId,
                InstanceState = instance.State,
                Originator = instance.Originator,
                TokenId = workItem.TokenId,
                WorkflowCode = template.WorkflowCode,
                WorkflowName = template.WorkflowFullName,
                SequenceNo = instance.SequenceNo,
                WorkItemState = workItem.State
            };

            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
            if (user != null)
            {
                OThinker.Organization.Signature[] signatures = this.Engine.Organization.GetSignaturesByUnit(user.ObjectID);
                result.MySignatures = new List<SignatureParam>();
                if (signatures != null)
                {
                    foreach (OThinker.Organization.Signature signature in signatures)
                    {
                        result.MySignatures.Add(new SignatureParam()
                        {
                            IsDefault = signature.IsDefault,
                            SignatureName = signature.Name,
                            SignautreId = signature.ObjectID,
                            SortKey = signature.SortKey
                        });
                    }
                }

                result.FrequentlyComment = this.Engine.Organization.GetFrequentlyUsedCommentTextsByUser(user.ObjectID);

                if (activity is OThinker.H3.WorkflowTemplate.ParticipativeActivity)
                {
                    // PermittedActions actions = new PermittedActions();
                    OThinker.H3.WorkflowTemplate.ParticipativeActivity participative = activity as OThinker.H3.WorkflowTemplate.ParticipativeActivity;
                    //actions.AdjustParticipant = participative.PermittedActions.AdjustParticipant;
                    //actions.Assist = participative.PermittedActions.Assist;
                    //actions.CancelIfUnfinished = participative.PermittedActions.CancelIfUnfinished;
                    //actions.Choose = participative.PermittedActions.Choose;
                    //actions.Circulate = participative.PermittedActions.Circulate;
                    //actions.Consult = participative.PermittedActions.Consult;
                    //actions.Forward = participative.PermittedActions.Forward;
                    //actions.Reject = participative.PermittedActions.Reject || participative.PermittedActions.RejectToAny || participative.PermittedActions.RejectToFixed;

                    if (participative.PermittedActions.RejectToAny)
                    {// 获取允许驳回的节点
                        List<ActivityParam> rejectActivies = new List<ActivityParam>();
                        foreach (OThinker.H3.Instance.Token token in instance.Tokens)
                        {
                            if (token.Activity == activity.ActivityCode) continue;
                            ParticipativeActivity act = template.GetActivityByCode(token.Activity) as ParticipativeActivity;
                            if (act == null) continue;
                            rejectActivies.Add(new ActivityParam()
                            {
                                ActivityCode = act.ActivityCode,
                                DisplayName = act.DisplayName
                            });
                        }
                        result.RejectActivies = rejectActivies;
                    }

                    result.PermittedActions = participative.PermittedActions;
                }
            }

            return Json(result);
        }

        /// <summary>
        /// 提交工作任务
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode"></param>
        /// <param name="workItemId"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        [HttpPost]
        public System.Web.Mvc.ActionResult SubmitWorkItem(string appId, string pwd, string userCode, string workItemId, string paramValues)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (item == null) throw new Exception("工作任务不存在");
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
            if (user == null) throw new Exception("用户不存在");
            List<DataItemParam> listParamValues = JSSerializer.Deserialize<List<DataItemParam>>(paramValues);
            // 保存数据项
            SaveBizObject(item, user, OThinker.Data.BoolMatchValue.True, listParamValues);

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                item.ObjectID,
                user.UnitID,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,
                null,
                OThinker.Data.BoolMatchValue.True,
                string.Empty,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                11);

            // 需要通知实例事件管理器结束事件
            AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    OThinker.Data.BoolMatchValue.True,
                    false,
                    OThinker.Data.BoolMatchValue.True,
                    true,
                    null);
            this.Engine.InstanceManager.SendMessage(endMessage);
            return Json(new
            {
                Result = true,
                Message = string.Empty
            });
        }

        /// <summary>
        /// 驳回工作任务
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="pwd"></param>
        /// <param name="userCode">用户账号</param>
        /// <param name="workItemId">工作任务ID</param>
        /// <param name="rejectToActivity">要驳回的指定节点编码，如果为空则由系统按照流程模板设置处理</param>
        /// <param name="paramValues">驳回时的表单参数</param>
        /// <returns></returns>
        [HttpPost]
        public System.Web.Mvc.ActionResult RejectWorkItem(string appId, string pwd, string userCode, string workItemId, string rejectToActivity, string paramValues)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (item == null) throw new Exception("工作任务不存在");
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as OThinker.Organization.User;
            if (user == null) throw new Exception("用户不存在");
            List<DataItemParam> listParamValues = JSSerializer.Deserialize<List<DataItemParam>>(paramValues);
            // 保存数据项
            SaveBizObject(item, user, OThinker.Data.BoolMatchValue.True, listParamValues);

            // 获取驳回节点
            if (string.IsNullOrEmpty(rejectToActivity))
            {// 目标节点为空，则判断当前节点允许驳回的环节 
                PublishedWorkflowTemplate template = this.Engine.WorkflowManager.GetPublishedTemplate(item.WorkflowCode, item.WorkflowVersion);
                ParticipativeActivity activity = template.GetActivityByCode(item.ActivityCode) as ParticipativeActivity;
                if (activity == null) throw new Exception("当前环节不允许驳回");
                if (activity.PermittedActions.RejectToStart) rejectToActivity = template.StartActivityCode; // 驳回到开始
                else if (activity.PermittedActions.RejectToFixed || activity.PermittedActions.RejectToAny)
                {
                    if (activity.PermittedActions.RejectToActivityCodes != null && activity.PermittedActions.RejectToActivityCodes.Length > 0)
                    {
                        rejectToActivity = activity.PermittedActions.RejectToActivityCodes[0];
                    }
                }
                else if (activity.PermittedActions.Reject)
                {// 驳回上一步 
                    InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
                    int[] tokens = context.GetToken(item.TokenId).PreTokens;
                    if (tokens != null && tokens.Length > 0)
                    {
                        rejectToActivity = context.GetToken(tokens[0]).Activity;
                    }
                }

                if (string.IsNullOrEmpty(rejectToActivity))
                {
                    rejectToActivity = template.StartActivityCode;
                }
            }

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                  item.ObjectID,
                  user.ObjectID,
                  H3.WorkItem.AccessPoint.ExternalSystem,
                  null,
                  OThinker.Data.BoolMatchValue.False,
                  string.Empty,
                  null,
                  H3.WorkItem.ActionEventType.Backward,
                  10);
            // 准备触发后面Activity的消息
            OThinker.H3.Messages.ActivateActivityMessage activateMessage
                = new OThinker.H3.Messages.ActivateActivityMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,
                rejectToActivity,
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
            return Json(new
            {
                Result = true,
                Message = string.Empty
            });
        }
        #endregion

        #region 私有方法 ----------------------------------
        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="workItem"></param>
        /// <param name="user"></param>
        /// <param name="boolMatchValue"></param>
        /// <param name="paramValues"></param>
        private void SaveBizObject(OThinker.H3.WorkItem.WorkItem workItem, OThinker.Organization.User user, OThinker.Data.BoolMatchValue boolMatchValue, List<DataItemParam> paramValues)
        {
            Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(workItem.InstanceId);
            string bizObjectId = InstanceContext == null ? string.Empty : InstanceContext.BizObjectId;
            OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(InstanceContext.BizObjectSchemaCode);
            OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(this.Engine, schema, workItem.Participant);
            bo.ObjectID = bizObjectId;
            bo.Load();

            // 设置数据项的值
            foreach (DataItemParam param in paramValues)
            {
                OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.ItemName);
                if (property.LogicType == DataLogicType.Comment)
                {// 审核意见
                    CommentParam comment = JSSerializer.Deserialize<CommentParam>(param.ItemValue + string.Empty);
                    this.Engine.BizObjectManager.AddComment(new Comment()
                    {
                        BizObjectId = bo.ObjectID,
                        BizObjectSchemaCode = schema.SchemaCode,
                        InstanceId = workItem.InstanceId,
                        TokenId = workItem.TokenId,
                        Approval = boolMatchValue,
                        Activity = workItem.ActivityCode,
                        DataField = property.Name,
                        UserID = user.ObjectID,
                        SignatureId = comment.SignatureId,
                        Text = comment.Text
                    });
                }
                else
                {
                    this.SetItemValue(bo, property, param.ItemValue);
                }
            }

            bo.Update();
        }

        /// <summary>
        /// 设置数据项的值
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="property"></param>
        /// <param name="itemValue"></param>
        private void SetItemValue(OThinker.H3.DataModel.BizObject bo, OThinker.H3.DataModel.PropertySchema property, object itemValue)
        {
            if (property.LogicType == DataLogicType.String
                               || property.LogicType == DataLogicType.ShortString
                               || property.LogicType == DataLogicType.Int
                               || property.LogicType == DataLogicType.Long
                               || property.LogicType == DataLogicType.Decimal
                               || property.LogicType == DataLogicType.Html
                               || property.LogicType == DataLogicType.SingleParticipant)
            {// 短文本、长文本、整数、数值、HTML、参与者单人
                bo[property.Name] = itemValue;
            }
            else if (property.LogicType == DataLogicType.DateTime)
            {
                DateTime d = DateTime.Now;
                DateTime.TryParse(itemValue + string.Empty, out d);
                bo[property.Name] = d;
            }
            else if (property.LogicType == DataLogicType.Bool)
            {
                bool b = false;
                bool.TryParse(itemValue + string.Empty, out b);
                bo[property.Name] = b;
            }
            else if (property.LogicType == DataLogicType.TimeSpan)
            {// 时间段
                TimeSpan t = new TimeSpan(0, 0, 0);
                TimeSpan.TryParse(itemValue + string.Empty, out t);
                bo[property.Name] = t;
            }
            else if (property.LogicType == DataLogicType.MultiParticipant)
            {// 参与者多人
                if (itemValue != null)
                    bo[property.Name] = itemValue;
            }
            else if (property.LogicType == DataLogicType.BizObjectArray)
            {// 子表
                if (property.ChildSchema == null) return;

                List<OThinker.H3.DataModel.BizObject> details = new List<OThinker.H3.DataModel.BizObject>();
                List<List<DataItemParam>> gridView = JSSerializer.Deserialize<List<List<DataItemParam>>>(itemValue + string.Empty);
                if (gridView == null) return;

                foreach (List<DataItemParam> row in gridView)
                {
                    OThinker.H3.DataModel.BizObject subBo = new OThinker.H3.DataModel.BizObject(this.Engine, property.ChildSchema, bo.OwnerId);
                    foreach (DataItemParam item in row)
                    {
                        OThinker.H3.DataModel.PropertySchema childProperty = property.ChildSchema.GetProperty(item.ItemName);
                        if (childProperty == null) continue;
                        this.SetItemValue(subBo, childProperty, item.ItemValue);
                    }
                    details.Add(subBo);
                }
                bo[property.Name] = details.ToArray();
            }
            else if (property.LogicType == DataLogicType.Attachment)
            {// 附件
                List<AttachmentParam> attachments = JSSerializer.Deserialize<List<AttachmentParam>>(itemValue + string.Empty);
                if (attachments != null)
                {
                    foreach (AttachmentParam attachment in attachments)
                    {
                        OThinker.H3.Data.Attachment attach = new Attachment()
                        {
                            FileName = attachment.FileName,
                            BizObjectId = bo.ObjectID,
                            DataField = property.Name,
                            Content = attachment.Content,
                            BizObjectSchemaCode = bo.Schema.SchemaCode,
                            ContentLength = attachment.Content.Length,
                            ContentType = attachment.ContentType,
                            DownloadUrl = attachment.DownloadUrl
                        };
                        this.Engine.BizObjectManager.AddAttachment(attach);
                    }
                }
            }
        }
        #endregion
    }

}
