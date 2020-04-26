using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkItem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OThinker.H3.Controllers.AppCode
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

        private System.Web.Script.Serialization.JavaScriptSerializer _JsSerializer = null;
        /// <summary>
        /// 获取JS序列化对象
        /// </summary>
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return _JsSerializer;
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
                    //foreach (DataItemParam param in paramValues)
                    //{

                    //    if (bo.Schema.ContainsField(param.ItemName))
                    //    {
                    //        bo[param.ItemName] = param.ItemValue;
                    //    }
                    //}
                    List<OThinker.H3.DataModel.BizObject> details = null;
                    foreach (DataItemParam param in paramValues)
                    {

                        if (!(details != null && details.Count > 0 && details[0].Schema.SchemaCode == param.ItemName))
                            details = new List<OThinker.H3.DataModel.BizObject>();
                        OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(param.ItemName);
                        if (property == null) continue;
                        SetItemValue(bo, property, param.ItemValue, details, param.ItemName);
                        
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
        /// <param name="MvcPost">MVC表单传递过来的值</param>
        /// <param name="Approval">审核结果</param>
        /// <param name="ActionEventType">事件类型</param>
        /// <param name="ActionButtonType">按钮类型</param>
        public void ForwardWorkItem(
           List<DataItemParam> paramList, string code, string schemaCode, string instanceId, string workItemCode)
        {


            OThinker.H3.Instance.InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(instanceId);
            OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
            OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine, schema, code);
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemCode);


            bo.ObjectID = ic.BizObjectId;
            bo.Load();//装载流程数据；

            if (paramList != null)
            {
                // 这里可以在创建流程的时候赋值
                foreach (DataItemParam param in paramList)
                {

                    if (bo.Schema.ContainsField(param.ItemName))
                    {
                        bo[param.ItemName] = param.ItemValue;
                    }
                }
            }

            bo.Update();

            if (!string.IsNullOrEmpty(workItemCode))
            {
            
            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                 workItemCode,
                 code,
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
                    instanceId,
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

        private void SetItemValue(OThinker.H3.DataModel.BizObject bo, OThinker.H3.DataModel.PropertySchema property, object itemValue, List<OThinker.H3.DataModel.BizObject> details,string paramName)
        {

            if (property.LogicType == OThinker.H3.Data.DataLogicType.DateTime)
            {
                DateTime d = DateTime.Now;
                DateTime.TryParse(itemValue + string.Empty, out d);
                bo[property.Name] = d;
            }
            else if (property.LogicType == OThinker.H3.Data.DataLogicType.Bool)
            {
                bool b = false;
                bool.TryParse(itemValue + string.Empty, out b);
                bo[property.Name] = b;
            }
            else if (property.LogicType == OThinker.H3.Data.DataLogicType.TimeSpan)
            {// 时间段
                TimeSpan t = new TimeSpan(0, 0, 0);
                TimeSpan.TryParse(itemValue + string.Empty, out t);
                bo[property.Name] = t;
            }
            else if (property.LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant)
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
            else if (property.LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
            {// 参与者多人:传入用户登录名
                if (itemValue != null)
                {
                    try
                    {
                        List<string> retUserIDs = new List<string>();
                        List<string> userCodes = JSSerializer.Deserialize<List<string>>(JSSerializer.Serialize(itemValue));
                        if (userCodes == null)
                            bo[property.Name] = null;
                        else
                        {
                            foreach (string code in userCodes)
                            {
                                var user = this.Engine.Organization.GetUserByCode(code + string.Empty);
                                //this.Engine.Organization.getuser
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
            else if (property.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
            {// 子表
                if (property.ChildSchema == null) return;
                if (itemValue == null) return;


                List<DataItemParam> listParams = JSSerializer.Deserialize<List<DataItemParam>>(itemValue as string);
                if (listParams == null) return;
                OThinker.H3.DataModel.BizObject subBo = new OThinker.H3.DataModel.BizObject(this.Engine, property.ChildSchema, bo.OwnerId);
                foreach (DataItemParam item in listParams)
                {
                    OThinker.H3.DataModel.PropertySchema childProperty = property.ChildSchema.GetProperty(item.ItemName);
                    if (childProperty == null) continue;
                    this.SetItemValue(subBo, childProperty, item.ItemValue, details, paramName);
                   
                }
                
                details.Add(subBo);

                bo[property.Name] = details.ToArray();
            }
            
            else if (property.LogicType == OThinker.H3.Data.DataLogicType.Comment)
            {
                //OThinker.H3.DataModel.IBizObjectManager BizObjectManager;

            }
            //else if (property.LogicType == OThinker.H3.Data.DataLogicType.Attachment)
            //{// 附件
            //    //List<List<DataItemParam>> attachments = JSSerializer.Deserialize<List<List<DataItemParam>>>(JSSerializer.Serialize(itemValue));
            //    List<AttachmentParamNew> attachments = JSSerializer.Deserialize<List<AttachmentParamNew>>(JSSerializer.Serialize(itemValue));
            //    //先清理掉当前流程实例的所有附件（在更新的情况下）
            //    this.Engine.BizObjectManager.RemoveAttachments(bo.Schema.SchemaCode, bo.ObjectID, property.Name);
            //    if (attachments != null)
            //    {
            //        foreach (AttachmentParamNew attachment in attachments)
            //        {
            //            OThinker.H3.Data.Attachment attach = new Attachment()
            //            {
            //                FileName = attachment.FileName,
            //                BizObjectId = bo.ObjectID,
            //                DataField = property.Name,
            //                Downloadable = true,
            //                DownloadUrl = attachment.DownloadUrl,
            //                Content = attachment.Content,
            //                BizObjectSchemaCode = bo.Schema.SchemaCode,
            //                ContentLength = attachment.Content == null ? attachment.ContentLength : attachment.Content.Length,
            //                ContentType = attachment.ContentType
            //            };
            //            this.Engine.BizObjectManager.AddAttachment(attach);
            //        }
            //    }
            //}
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
    }
}
