using Newtonsoft.Json;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common
{
    public class BPM
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

        public static void SubmitItem(string WorkitemId, OThinker.Data.BoolMatchValue approval, string commentText, string userId)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(WorkitemId);
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
        }

        public RestfulResult StartWorkflow_Base(string USER_CODE, string WORKFLOW_CODE, bool FINISH_START, string INSTANCE_ID, string PARAM_VALUES)
        {
            Engine.LogWriter.Write("Restful 服务，方法：StartWorkflow，参数：USER_CODE-->" + USER_CODE + ",WORKFLOW_CODE-->" + WORKFLOW_CODE + ",FINISH_START-->" + FINISH_START + ",INSTANCE_ID-->" + INSTANCE_ID + ",PARAM_VALUES-->" + JsonConvert.SerializeObject(PARAM_VALUES));
            RestfulResult result = new RestfulResult();
            Dictionary<string, object> listParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(PARAM_VALUES);
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;
            try
            {
                #region 参数校验
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflowTemplate = this.Engine.WorkflowManager.GetDefaultWorkflow(WORKFLOW_CODE);
                if (workflowTemplate == null)
                {
                    result.INSTANCE_ID = "";
                    result.MESSAGE = "流程模板不存在，模板编码:" + WORKFLOW_CODE + "。";
                    result.STATUS = "0";
                    result.WORKFLOW_CODE = WORKFLOW_CODE;
                    result.USER_CODE = USER_CODE;
                    return result;
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(USER_CODE);
                if (user == null)
                {
                    result.INSTANCE_ID = "";
                    result.MESSAGE = "用户{" + USER_CODE + "}不存在。";
                    result.STATUS = "0";
                    result.WORKFLOW_CODE = WORKFLOW_CODE;
                    result.USER_CODE = USER_CODE;
                    return result;
                }
                #endregion

                #region 流程实例ID为空：发起新流程
                if (string.IsNullOrEmpty(INSTANCE_ID))
                {
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
                        foreach (KeyValuePair<string, object> param in listParams)
                        {
                            OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.Key);
                            if (property == null) continue;
                            SetItemValue(bo, property, param.Value);
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
                        foreach (KeyValuePair<string, object> param in listParams)
                        {
                            OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.Key);
                            if (property == null) continue;
                            if (property.LogicType == DataLogicType.Comment)
                            {// 审核意见
                                CommentParam comment = JsonConvert.DeserializeObject<CommentParam>(JsonConvert.SerializeObject(param.Value));
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
                                    UserName = user.Name,
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
                        FINISH_START,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                    Engine.InstanceManager.SendMessage(startInstanceMessage);

                    result.INSTANCE_ID = InstanceId;
                    result.MESSAGE = "流程实例启动成功！";
                    result.BIZOBJECTID = bo.ObjectID;
                    result.STATUS = "2";
                    result.WORKFLOW_CODE = WORKFLOW_CODE;
                    result.USER_CODE = USER_CODE;
                }
                #endregion

                #region 流程实例ID不为空：更新流程数据；
                else
                {
                    InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(INSTANCE_ID);
                    if (ic == null)
                    {
                        result.INSTANCE_ID = INSTANCE_ID;
                        result.MESSAGE = "InstanceID错误，此ID在H3系统中不存在，请检查";
                        result.STATUS = "0";
                        result.WORKFLOW_CODE = WORKFLOW_CODE;
                        result.USER_CODE = USER_CODE;
                        return result;
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

                    bo.ObjectID = ic.BizObjectId;
                    bo.Load();//装载流程数据；

                    if (listParams != null)
                    {
                        // 这里可以在创建流程的时候赋值
                        foreach (KeyValuePair<string, object> param in listParams)
                        {
                            OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.Key);
                            if (property == null) continue;
                            SetItemValue(bo, property, param.Value);
                        }
                    }

                    bo.Update();
                    #region 提交当前任务，往下流转
                    if (FINISH_START)
                    {
                        string sql = "SELECT ObjectID FROM OT_WorkItem WHERE InstanceId='{0}' ORDER BY TokenId desc";
                        sql = string.Format(sql, INSTANCE_ID);
                        string workItemId = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                        if (workItemId != "")
                        {
                            // 获取工作项
                            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);

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
                        }
                    }
                    #endregion

                    result.INSTANCE_ID = INSTANCE_ID;
                    result.MESSAGE = "流程实例启动成功！（更新数据项的值）";
                    result.BIZOBJECTID = ic.BizObjectId;
                    result.STATUS = "2";
                    result.WORKFLOW_CODE = WORKFLOW_CODE;
                    result.USER_CODE = USER_CODE;
                }
                #endregion
            }
            catch (Exception ex)
            {
                result = new RestfulResult();
                result.INSTANCE_ID = "";
                result.BIZOBJECTID = "";
                result.MESSAGE = "接口异常：" + ex.ToString();
                result.STATUS = "0";
                result.WORKFLOW_CODE = WORKFLOW_CODE;
                result.USER_CODE = USER_CODE;
            }
            return result;
        }

        /// <summary>
        /// 设置数据项的值
        /// </summary>
        /// <param name="bo"></param>
        /// <param name="property"></param>
        /// <param name="itemValue"></param>
        public void SetItemValue(OThinker.H3.DataModel.BizObject bo, OThinker.H3.DataModel.PropertySchema property, object itemValue)
        {
            if (property.LogicType == DataLogicType.DateTime)
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
            else if (property.LogicType == DataLogicType.SingleParticipant)
            {// 参与者单人:传入用户登录名
                if (itemValue != null)
                {
                    try
                    {
                        var user = this.Engine.Organization.GetUserByCode(itemValue + string.Empty);
                        bo[property.Name] = user == null ? "" : user.ObjectID;
                    }
                    catch (Exception ex)
                    {
                        bo[property.Name] = null;
                        this.Engine.LogWriter.Write("字段：" + property.Name + "赋值异常-->" + ex.ToString());
                    }
                }
            }
            else if (property.LogicType == DataLogicType.MultiParticipant)
            {// 参与者多人:传入用户登录名
                if (itemValue != null)
                {
                    try
                    {
                        List<string> retUserIDs = new List<string>();
                        List<string> UserCodes = JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(itemValue));
                        if (UserCodes == null)
                        {
                            bo[property.Name] = null;
                        }
                        else
                        {
                            foreach (String code in UserCodes)
                            {
                                var user = this.Engine.Organization.GetUserByCode(code + string.Empty);
                                if (user != null)
                                    retUserIDs.Add(user.ObjectID);
                            }
                            bo[property.Name] = retUserIDs.ToArray();
                        }

                    }
                    catch (Exception ex)
                    {
                        bo[property.Name] = null;
                        this.Engine.LogWriter.Write("字段：" + property.Name + "赋值异常-->" + ex.ToString());
                    }
                }
            }
            else if (property.LogicType == DataLogicType.BizObjectArray)
            {// 子表
                if (property.ChildSchema == null) return;

                List<OThinker.H3.DataModel.BizObject> details = new List<OThinker.H3.DataModel.BizObject>();
                List<Dictionary<string, object>> gridView = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(JsonConvert.SerializeObject(itemValue));
                if (gridView == null) return;

                foreach (Dictionary<string, object> row in gridView)
                {
                    OThinker.H3.DataModel.BizObject subBo = new OThinker.H3.DataModel.BizObject(this.Engine, property.ChildSchema, bo.OwnerId);
                    foreach (KeyValuePair<string, object> item in row)
                    {
                        OThinker.H3.DataModel.PropertySchema childProperty = property.ChildSchema.GetProperty(item.Key);
                        if (childProperty == null) continue;
                        this.SetItemValue(subBo, childProperty, item.Value);
                    }
                    details.Add(subBo);
                }
                bo[property.Name] = details.ToArray();
            }
            else if (property.LogicType == DataLogicType.Attachment)
            {// 附件
                //List<List<DataItemParam>> attachments = JSSerializer.Deserialize<List<List<DataItemParam>>>(JSSerializer.Serialize(itemValue));
                List<AttachmentParamNew> attachments = JsonConvert.DeserializeObject<List<AttachmentParamNew>>(JsonConvert.SerializeObject(itemValue));
                //先清理掉当前流程实例的所有附件（在更新的情况下）
                this.Engine.BizObjectManager.RemoveAttachments(bo.Schema.SchemaCode, bo.ObjectID, property.Name);
                if (attachments != null)
                {
                    foreach (AttachmentParamNew attachment in attachments)
                    {
                        OThinker.H3.Data.Attachment attach = new Attachment()
                        {
                            FileName = attachment.FileName,
                            BizObjectId = bo.ObjectID,
                            DataField = property.Name,
                            Downloadable = true,
                            DownloadUrl = attachment.DownloadUrl,
                            Content = attachment.Content,
                            BizObjectSchemaCode = bo.Schema.SchemaCode,
                            ContentLength = attachment.Content == null ? attachment.ContentLength : attachment.Content.Length,
                            ContentType = attachment.ContentType
                        };
                        this.Engine.BizObjectManager.AddAttachment(attach);
                    }
                }
            }
            //if (property.LogicType == DataLogicType.String
            //                   || property.LogicType == DataLogicType.ShortString
            //                   || property.LogicType == DataLogicType.Int
            //                   || property.LogicType == DataLogicType.Long
            //                   || property.LogicType == DataLogicType.Decimal
            //                   || property.LogicType == DataLogicType.Html
            //    || property.LogicType == DataLogicType.Double)
            else
            {// 短文本、长文本、整数、数值、HTML
                bo[property.Name] = itemValue;
            }
        }
        
        #region Class
        public class RestfulResultCancleInstance
        {
            /// <summary>
            /// BPM返回的状态：0代表取消流程失败，1代表取消流程成功
            /// </summary>
            public string STATUS { get; set; }
            /// <summary>
            /// BPM返回的状态说明或者异常信息
            /// </summary>
            public string MESSAGE { get; set; }
            /// <summary>
            /// 预留
            /// </summary>
            public string DATA { get; set; }
        }
        public class RestfulResult
        {
            /// <summary>
            /// BPM返回的状态：0代表启动失败，1代表审批通过，2审批中，3驳回,-1代表 Token状态失效
            /// </summary>
            public string STATUS { get; set; }
            /// <summary>
            /// BPM中对应的流程编码
            /// </summary>
            public string WORKFLOW_CODE { get; set; }
            /// <summary>
            /// BPM系统中的流程实例ID
            /// </summary>
            public string INSTANCE_ID { get; set; }
            /// <summary>
            /// BPM系统中的流程实例ID
            /// </summary>
            public string BIZOBJECTID { get; set; }
            /// <summary>
            /// H3系统中返回的系统提示
            /// </summary>
            public string MESSAGE { get; set; }
            /// <summary>
            /// 用户登录名；
            /// </summary>
            public string USER_CODE { get; set; }
        }
        public class AttachmentParamNew
        {
            /// <summary>
            /// 获取或设置文件名称
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// 获取或设置文件内容
            /// </summary>
            public byte[] Content { get; set; }
            /// <summary>
            /// 获取或设置文件类型
            /// </summary>
            public string ContentType { get; set; }
            /// <summary>
            /// 获取或设置文件大小
            /// </summary>
            public int ContentLength { get; set; }
            /// <summary>
            /// 获取或设置文件下载地址
            /// </summary>
            public string DownloadUrl { get; set; }
        }
        public class ImgFile
        {
            public string id { get; set; }
        }
        #endregion
    }
}