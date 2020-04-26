using OThinker.Data.Database.Serialization;
using OThinker.H3.Acl;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
using OThinker.H3.Instance;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// MvcController 类
    /// </summary>
    public class MvcController
    {
        #region 页面参数 --------------------

        /// <summary>
        /// 提交
        /// </summary>
        public const string Button_Submit = "submit";
        /// <summary>
        /// 驳回
        /// </summary>
        public const string Button_Reject = "reject";
        /// <summary>
        /// 加载表单数据
        /// </summary>
        public const string Button_Load = "load";
        /// <summary>
        /// 保存
        /// </summary>
        public const string Button_Save = "save";
        /// <summary>
        /// 结束流程
        /// </summary>
        public const string Button_FinishInstance = "finishinstance";
        /// <summary>
        /// 取消流程
        /// </summary>
        public const string Button_CancelInstance = "cancelinstance";
        /// <summary>
        /// 取回流程
        /// </summary>
        public const string Button_RetrieveInstance = "retrieveinstance";
        /// <summary>
        /// 锁定
        /// </summary>
        public const string Button_LockInstance = "lockinstance";
        /// <summary>
        /// 解锁
        /// </summary>
        public const string Button_UnLockInstance = "unlockinstance";
        /// <summary>
        /// 驳回到开始
        /// </summary>
        public const string RejectTo_Start = "__RejectTo_Start";
        /// <summary>
        /// 驳回到前一步
        /// </summary>
        public const string RejectTo_Previous = "__RejectTo_Previous";
        /// <summary>
        /// 页面传递过来的值
        /// </summary>
        public const string Param_MvcPostValue = "MvcPostValue";
        /// <summary>
        /// 增加表单名称数据项
        /// </summary>
        public const string SheetDisplayName = "Sheet__DisplayName";
        /// <summary>
        /// 增加征询意见数据项
        /// </summary>
        public const string SheetConsultComment = "Sheet__ConsultComment";

        /// <summary>
        /// 增加协办意见数据项
        /// </summary>
        public const string SheetAssistComment = "Sheet__Assist";

        /// <summary>
        /// 增加转发意见数据项
        /// </summary>
        public const string SheetForwardComment = "Sheet__Forward";

        #endregion

        #region 属性信息 --------------------
        private string _DisplayName = string.Empty;
        /// <summary>
        /// 获取表单的显示名称
        /// </summary>
        public string DisplayName
        {
            get
            {
                if (_DisplayName == string.Empty)
                {
                    return this.ActionContext.SheetDisplayName;
                }
                return _DisplayName;
            }
        }

        /// <summary>
        /// 征询意见序号
        /// </summary>
        private int _consltNumber = 0;
        private int consltNumber
        {
            get
            {
                return this._consltNumber;
            }
            set
            {
                this._consltNumber = value;
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

        private bool _SheetLocked = false;
        /// <summary>
        /// 获取或设置任务是否被锁定
        /// </summary>
        public bool SheetLocked
        {
            get
            {
                return this._SheetLocked;
            }
            set
            {
                this._SheetLocked = value;
            }
        }
        #endregion

        #region 构造函数 --------------------
        public MvcPage Page = null;

        /// <summary>
        /// MvcController构造函数
        /// </summary>
        /// <param name="Page">MvcPage类</param>
        /// <param name="Enviroment">环境对象</param>
        public MvcController(MvcPage Page, ActionContext Enviroment)
        {
            this.Page = Page;
            this._ActionContext = Enviroment;
            this._Request = Enviroment.Request;
        }
        #endregion

        #region 加载表单数据 ----------------
        /// <summary>
        /// 检查用户是否具有操作权限
        /// </summary>
        /// <returns></returns>
        public virtual OThinker.Data.BoolMatchValue ValidateAuthorization()
        {
            if (SheetUtility.ValidateAuthorization(this.ActionContext.User,
                this.ActionContext.SheetDataType,
                this.ActionContext.IsOriginateMode,
                this.ActionContext.SchemaCode,
                this.ActionContext.BizObject,
                this.ActionContext.SheetMode,
                this.ActionContext.WorkflowCode,
                this.ActionContext.WorkItem,
                this.ActionContext.CirculateItem,
                this.ActionContext.InstanceContext))
            {
                return OThinker.Data.BoolMatchValue.True;
            }

            // 验证关联关系流程权限
            if (IsRelationInstance()) return OThinker.Data.BoolMatchValue.True;

            return OThinker.Data.BoolMatchValue.False;
        }

        private string _EncryptKey = string.Empty;
        /// <summary>
        /// 获取加密URL的Key
        /// </summary>
        public string EncryptKey
        {
            get
            {
                if (_EncryptKey == string.Empty)
                {
                    _EncryptKey = ConfigurationManager.AppSettings["EncryptKey"] + string.Empty;
                }
                if (_EncryptKey == string.Empty)
                {
                    _EncryptKey = this.ActionContext.Engine.EngineConfig.Code;
                }
                return this._EncryptKey;
            }
        }

        /// <summary>
        /// 是否关联关系流程
        /// </summary>
        /// <returns></returns>
        private bool IsRelationInstance()
        {
            string relation = OThinker.Data.Convertor.SqlInjectionPrev(Request.QueryString["RelationID"] + string.Empty);
            if (!string.IsNullOrEmpty(relation))
            { // 严重关联的流程是否有权限
                relation = relation.Replace(" ", "+");
                relation = OThinker.Security.DESEncryptor.DecryptDES(relation, EncryptKey);
                if (relation.IndexOf(".") > -1)
                {
                    string instanceId = relation.Substring(0, relation.IndexOf("."));
                    InstanceContext context = this.ActionContext.Engine.InstanceManager.GetInstanceContext(instanceId);
                    if (context != null)
                    {// 只支持对关联流程的权限验证
                        if (SheetUtility.ValidateAuthorization(this.ActionContext.User,
                               SheetDataType.Workflow,
                               false,
                               context.BizObjectSchemaCode,
                               null,
                               SheetMode.View,
                               context.WorkflowCode,
                               null,
                               null,
                               context))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 加载表单数据
        /// </summary>
        /// <returns></returns>
        public MvcViewContext DoPageLoad()
        {
            //当前活动
            WorkflowTemplate.ParticipativeActivity activity = this.ActionContext.ActivityTemplate as WorkflowTemplate.ParticipativeActivity;

            #region 构造Mvc后台返回前端的数据
            Dictionary<string, string> priorities = new Dictionary<string, string>();
            PriorityType[] priorityTypes = PriorityTypeConvertor.GetSelectableTypes();
            foreach (PriorityType v in priorityTypes)
            {
                string name = PriorityTypeConvertor.ToString(this.ActionContext.User.PortalSettings, v);
                priorities.Add(v.ToString(), name);
            }

            //设置是否显示同意/不同意
            bool ApprovalListVisible = false;
            if (this.ActionContext.ActivityTemplate != null)
            {
                if (this.ActionContext.ActivityTemplate.IsApproveActivity//审批节点
                    && this.ActionContext.IsWorkMode //工作模式
                    && this.ActionContext.ActivityTemplate.ParticipateType == ActivityParticipateType.MultiParticipants//多人参与者
                    && ((ApproveActivity)this.ActionContext.ActivityTemplate).DisapproveExit != "1"//否决出口不等于1
                    && !this.ActionContext.WorkItem.IsAssistive//非协办
                    && !this.ActionContext.WorkItem.IsConsultive//非征询
                    )
                {
                    ApprovalListVisible = true;
                }
            }

            MvcViewContext sheet = new MvcViewContext()
            {
                //StartActivityCode = this.ActionContext.Workflow.StartActivity.ActivityCode,
                Consulted = this.ActionContext.WorkItem != null ? this.ActionContext.WorkItem.Consulted : false,
                ConsultantFinished = this.ActionContext.WorkItem != null ? this.ActionContext.WorkItem.ConsultantFinished : false,
                StartActivityCode = this.ActionContext.Workflow == null ? "" : this.ActionContext.Workflow.StartActivity.ActivityCode,//发起环节编码
                ActivityCode = this.ActionContext.ActivityCode,
                Originator = this.ActionContext.InstanceContext == null ? this.ActionContext.User.UserID : this.ActionContext.InstanceContext.Originator,
                OriginatorOU = this.ActionContext.InstanceContext == null ? this.ActionContext.User.Department : this.ActionContext.InstanceContext.OrgUnit,
                UserID = this.ActionContext.User.UserID,
                UserCode = this.ActionContext.User.UserCode,
                UserName = this.ActionContext.User.UserName,
                UserImage = this.GetUserImage(),
                //Participants = Participants,
                Language = (this.Page.Session[Sessions.GetLang()] + string.Empty).Replace("-", "_").ToLower(),
                SchemaCode = this.ActionContext.SchemaCode,
                InstanceId = this.ActionContext.InstanceId,
                BizObjectID = this.ActionContext.BizObjectID,
                WorkItemId = this.ActionContext.WorkItemID,
                DisplayName = this.DisplayName,
                // OriginatorCode = this.ActionContext.User.UserCode,
                WorkItemType = this.ActionContext.WorkItem != null ? this.ActionContext.WorkItem.ItemType : (this.ActionContext.CirculateItem != null ? WorkItem.WorkItemType.Circulate : OThinker.H3.WorkItem.WorkItemType.Unspecified),
                SheetMode = this.ActionContext.SheetMode,
                SheetDataType = this.ActionContext.SheetDataType,
                IsOriginateMode = this.ActionContext.IsOriginateMode,
                IsMobile = this.ActionContext.IsMobile,
                MobileReturnUrl = this.GetMobileReturnUrl(),
                PrintUrl = this.ActionContext.IsOriginateMode ? string.Empty : AppConfig.GetPrintSheetUrl(this.ActionContext.Sheet, this.ActionContext.WorkItemID, this.ActionContext.InstanceId),
                ApprovalListVisible = ApprovalListVisible,
                Priorities = priorities,
                HiddenFields = HeapDataItem.GetHiddenFields(this.ActionContext.Engine.HeapDataManager, this.ActionContext.InstanceId),
                OptionalRecipients = GetOptionalRecipients(),
                PageScript = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.PageScript,
                // 增加流程模板和节点的扩展属性，提供前端使用
                Activity = new ExtendProperty()
                {
                    ShortText1 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.ShortText1,
                    ShortText2 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.ShortText2,
                    ShortText3 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.ShortText3,
                    ShortText4 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.ShortText4,
                    ShortText5 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.ShortText5,
                    LongText6 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.LongText6,
                    LongText7 = this.ActionContext.ActivityTemplate == null ? string.Empty : this.ActionContext.ActivityTemplate.LongText7
                },
                WorkflowTemplate = new ExtendProperty()
                {
                    ShortText1 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.ShortText1,
                    ShortText2 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.ShortText2,
                    ShortText3 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.ShortText3,
                    ShortText4 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.ShortText4,
                    ShortText5 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.ShortText5,
                    LongText6 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.LongText6,
                    LongText7 = this.ActionContext.Workflow == null ? string.Empty : this.ActionContext.Workflow.LongText7
                }
            };
            if (this.ActionContext.SheetDataType == SheetDataType.Workflow)
            {
                sheet.WorkflowCode = this.ActionContext.WorkflowCode;
                sheet.WorkflowVersion = this.ActionContext.WorkflowVersion;

                sheet.Posts = this.LoadPosts(this.ActionContext.Workflow); // TODO:这里要修改；
                if (!ApprovalListVisible && this.ActionContext.CirculateItem == null)
                {
                    sheet.SubmitActivities = GetSubmitActivies();
                    sheet.RejectActivities = GetRejectActivies();
                }
                sheet.PermittedActions = GetPermittedActions(activity, ApprovalListVisible);

                //传阅活动永远允许继续传阅
                if (this.ActionContext.ActivityTemplate != null &&
                    this.ActionContext.ActivityTemplate.ActivityType == ActivityType.Circulate)
                {
                    sheet.PermittedActions.Circulate = sheet.PermittedActions.Recirculate = true;
                }
            }
            else
            {
                sheet.PermittedActions = new PermittedActions()
                {
                    Save = (this.ActionContext.SheetMode == SheetMode.Work || this.ActionContext.SheetMode == SheetMode.Originate),
                    Close = true
                };

            }
            sheet.Company = this.ActionContext.Engine.Organization.Company;
            #endregion

            // 处理锁策略后的权限
            if (this.ActionContext.SheetDataType == SheetDataType.Workflow
                && this.ActionContext.IsWorkMode)
            {
                // 先请求锁定级别
                string message = string.Empty;
                if (!this.ValidateLockLevel(ref message))
                {
                    // 禁止多人同时编辑表单
                    sheet.Message = message;
                    //sheet.Close = true;
                    //return sheet;
                }
                sheet.Message = message;

                if (activity != null && (activity.LockPolicy == OThinker.H3.WorkflowTemplate.LockPolicy.Request
                    || activity.LockPolicy == WorkflowTemplate.LockPolicy.Open))
                {
                    if (this.ActionContext.WorkItem.IsLocked)
                    {
                        //判断表单锁类型，可多人同时编辑，或不能
                        WorkflowTemplate.ParticipativeActivity task = this.ActionContext.ActivityTemplate as WorkflowTemplate.ParticipativeActivity;

                        if (task.LockLevel == LockLevel.Mono)
                        {
                            SheetLocked = true; //只允许一个人编辑时，其他人数据项不可编辑

                            // 别人锁定了，禁止提交、转发、保存等
                            sheet.PermittedActions.Save = false;
                            sheet.PermittedActions.Submit = false;
                            sheet.PermittedActions.Forward = false;
                            sheet.PermittedActions.AdjustParticipant = false;
                            sheet.PermittedActions.Reject = false;
                            sheet.PermittedActions.Consult = false;
                            sheet.PermittedActions.Assist = false;
                            sheet.PermittedActions.Circulate = false;
                            sheet.PermittedActions.FinishInstance = false;
                            sheet.PermittedActions.CancelInstance = false;
                        }
                    }
                    else if (!this.ActionContext.WorkItem.IsLocking)
                    {
                        sheet.PermittedActions.LockInstance = true;
                    }
                    else if (this.ActionContext.WorkItem.IsLocking)
                    {
                        sheet.PermittedActions.UnLockInstance = true;
                    }
                }
            }
            if (this.ActionContext.WorkItem != null
                && this.ActionContext.WorkItem.State == WorkItem.WorkItemState.Waiting)
            {
                this.ActionContext.Engine.WorkItemManager.DoWorkItem(this.ActionContext.WorkItemID);
            }

            if (this.ActionContext.InstanceContext != null)
            {
                // 流程已经结束或已经取消，禁止显示取消流程按钮
                if (this.ActionContext.InstanceContext.State == Instance.InstanceState.Finished || this.ActionContext.InstanceContext.State == Instance.InstanceState.Canceled)
                {
                    sheet.PermittedActions.CancelInstance = false;
                }
                else
                {
                   
                    if (this.ActionContext.IsMobile && this.ActionContext.User.ValidateWFInsAdmin(this.ActionContext.WorkflowCode, this.ActionContext.InstanceContext.Originator))
                    {
                        sheet.PermittedActions.CancelInstance = true;
                    }
                }
            }

            //协办征询无需锁策略
            if (this.ActionContext.WorkItem != null)
            {
                if (this.ActionContext.WorkItem.IsAssistive || this.ActionContext.WorkItem.IsConsultive)
                {
                    sheet.PermittedActions.LockInstance = false;
                }
            }


            // 构造表单名称数据项
            MvcBizObject bizObject = new MvcBizObject();
            bizObject.DataItems.Add(SheetDisplayName, new MvcDataItem()
            {
                L = DataLogicType.String,
                N = this.DisplayName,
                O = "V",
                V = this.DisplayName
            });

            string consultOption = string.Empty;
            // 构造征询意见数据项
            if (this.ActionContext.WorkItem != null)
            {
                if (this.ActionContext.WorkItem.IsConsultive)
                {
                    consultOption = this.ActionContext.IsMobile ? "M" : "V";
                    if (this.ActionContext.IsWorkMode) consultOption += "E";
                }
                else if (this.ActionContext.WorkItem.ActivityConsulted || this.ActionContext.WorkItem.Consulted)
                {
                    consultOption = this.ActionContext.IsMobile ? "M" : "V";
                }
            }
            bizObject.DataItems.Add(SheetConsultComment, new MvcDataItem()
            {
                L = DataLogicType.Comment,
                N = Configs.Global.ResourceManager.GetString("SheetActionPane_Consult"),
                O = consultOption,
                V = consultOption == string.Empty ? null : this.LoadCommentDataNew(SheetConsultComment)
            });
            bizObject.DataItems.Add(SheetAssistComment, new MvcDataItem()
            {
                L = DataLogicType.Comment,
                N = "沟通意见",
                O = this.ActionContext.IsMobile ? "M" : "V",
                V = this.LoadCommentData(SheetAssistComment)
            });
            bizObject.DataItems.Add(SheetForwardComment, new MvcDataItem()
            {
                L = DataLogicType.Comment,
                N = "转办意见",
                O = this.ActionContext.IsMobile ? "M" : "V",
                V = this.LoadCommentData(SheetForwardComment)
            });
            #region 添加系统数据项
            foreach (string keyword in OThinker.H3.Instance.Keywords.ParserFactory.GetSheetSystemData())
            {
                if (!bizObject.DataItems.ContainsKey(keyword))
                {
                    bizObject.DataItems.Add(keyword,
                        new MvcDataItem(this.ActionContext.InstanceData[keyword],
                            SheetLocked));
                }
            }
            #endregion

            #region  添加数据项

            //AttachmentHeader[] headers = this.ActionContext.Engine.BizObjectManager.QueryAttachment(
            //     this.ActionContext.SchemaCode,
            //     this.ActionContext.BizObjectID,
            //     field.Name,
            //     OThinker.Data.BoolMatchValue.True,
            //     null);

            //List<MvcListItem> listItems = null;
            //string[] ids = this.ActionContext.InstanceData[field.Name].Value as string[];
            //if (ids != null)
            //{
            //    listItems = new List<MvcListItem>();
            //    Organization.Unit[] units = this.ActionContext.Engine.Organization.GetUnits(ids);

            foreach (FieldSchema field in this.ActionContext.Schema.Fields)
            {
                if (bizObject.DataItems.ContainsKey(field.Name))
                {
                    continue;
                }
                MvcDataItem dataItem = new MvcDataItem(this.ActionContext.InstanceData[field.Name], SheetLocked);
                if (this.ActionContext.IsViewMode)
                {
                    //只读
                    dataItem.O = dataItem.O.Replace("E", "");
                }
                switch (field.LogicType)
                {
                    case DataLogicType.BizObject: // 业务对象
                        HttpContext.Current.Session.Remove(this.ActionContext.BizObjectID + field.Name);
                        dataItem.V = this.LoadBizObjectData(field);
                        break;
                    case DataLogicType.BizObjectArray://业务对象数组
                        HttpContext.Current.Session.Remove(this.ActionContext.BizObjectID + field.Name);
                        dataItem.V = this.LoadBizObjectArrayData(field);
                        break;
                    case DataLogicType.Comment://处理审批意见逻辑
                        dataItem.V = this.LoadCommentData(field.Name);
                        break;
                    case DataLogicType.Attachment://处理附件逻辑
                        dataItem.V = this.LoadAttachmentData(field);
                        break;
                    case DataLogicType.MultiParticipant://处理多人参与者逻辑
                        dataItem.V = this.LoadMultiParticipantData(field);
                        break;
                    case DataLogicType.SingleParticipant://处理单人参与者逻辑
                        string v = this.ActionContext.InstanceData[field.Name].Value + string.Empty;
                        if (!string.IsNullOrEmpty(v))
                        {
                            Organization.Unit unit = this.ActionContext.Engine.Organization.GetUnit(v);
                            if (unit != null)
                            {
                                string type = string.Empty;
                                if (unit.UnitType == Organization.UnitType.OrganizationUnit)
                                    type = "O";
                                else if (unit.UnitType == Organization.UnitType.Group)
                                    type = "G";
                                else if (unit.UnitType == Organization.UnitType.User)
                                    type = "U";
                                // dataItem.V = new MvcListItem(v, this.ActionContext.Engine.Organization.GetName(v));
                                dataItem.V = new MvcListItem(v, unit.Name, null, 0, type);
                            }
                        }
                        break;
                    case DataLogicType.Association:
                        dataItem.V = this.LoadAssociationData(field, dataItem.V + string.Empty);
                        break;
                }

                //征询和传阅，把可写去掉
                if ((this.ActionContext.WorkItem != null
                    && (this.ActionContext.WorkItem.IsConsultive
                    || this.ActionContext.WorkItem.ItemType == WorkItemType.Circulate)) || this.ActionContext.CirculateItem != null)
                {
                    dataItem.O = dataItem.O.Replace("E", "");
                }

                bizObject.DataItems.Add(field.Name, dataItem);
            }
            #endregion

            sheet.BizObject = bizObject;
            sheet.DirectParentUnits = this.GetDirectoryUnits(sheet.Originator);

            #region 添加当前处理人的签章
            List<Signature> mySignatures = new List<Signature>();
            Signature[] signatures = this.ActionContext.Engine.Organization.GetSignaturesByUnit(this.ActionContext.User.User.UnitID);
            if (signatures != null)
            {
                for (int i = 0; i < signatures.Length; i++)
                {
                    Signature signature = signatures[i];
                    if (signature.State == State.Active)
                    {
                        //TempImages文件夹中的内容会定期清空，图片不存在则重新创建
                        string fileName = this.TempImagesPath + "\\" + signature.ObjectID + ".jpg";
                        if (!File.Exists(fileName))
                        {
                            try
                            {
                                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                                {
                                    fs.Write(signature.Content, 0, signature.Content.Length);
                                    fs.Close();
                                }
                            }
                            catch
                            {
                                continue;
                            }
                        }
                        mySignatures.Add(new Signature()
                        {
                            ObjectID = signature.ObjectID,
                            IsDefault = signature.IsDefault,
                            Name = signature.Name,
                            SortKey = signature.SortKey
                        });
                        // mySignatures.Add(signature);
                    }
                }
            }
            sheet.MySignatures = mySignatures;
            #endregion
            this.AddUserLog(Tracking.UserLogType.OpenWorkSheet);
            return sheet;
        }

        /// <summary>
        /// 通过发起人获取到发起人的上级部门
        /// </summary>
        /// <param name="Originator">发起人</param>
        /// <returns>发起人的上级部门,包含自己的上级部门以及关联的部门</returns>
        private Dictionary<string, string> GetDirectoryUnits(string Originator)
        {
            // 添加直接所属OU
            User OriginatorUser = this.ActionContext.Engine.Organization.GetUnit(Originator) as User;

            //发起环节如果已经选择了虚拟用户部门，需要先找到实体用户
            if (OriginatorUser.IsVirtualUser)
            {
                OriginatorUser = this.ActionContext.Engine.Organization.GetUnit(OriginatorUser.RelationUserID) as User;
            }

            //读取用户关联OU和当前部门OU
            Dictionary<string, string> dicUserOUs = new Dictionary<string, string>();
            //添加本部
            dicUserOUs.Add(OriginatorUser.ParentID, this.ActionContext.Engine.Organization.GetUnit(OriginatorUser.ParentID).Name);

            //添加关联部门
            if (this.ActionContext.User.RelationUsers.Count > 0)
            {
                foreach (OThinker.Organization.User usere in this.ActionContext.User.RelationUsers)
                {
                    OThinker.Organization.Unit unit = this.ActionContext.Engine.Organization.GetUnit(usere.ParentID);
                    if (!dicUserOUs.ContainsKey(unit.ObjectID))
                    {
                        dicUserOUs.Add(unit.ObjectID, unit.Name);
                    }
                }
            }

            return dicUserOUs;
        }

        /// <summary>
        /// 检查签章是否存在
        /// </summary>
        /// <param name="SignatureId"></param>
        void CheckSignatureImage(string SignatureId)
        {

            //TempImages文件夹中的内容会定期清空，图片不存在则重新创建
            string fileName = this.TempImagesPath + "\\" + SignatureId + ".jpg";
            if (!File.Exists(fileName))
            {
                try
                {
                    Signature signature = this.ActionContext.Engine.Organization.GetSignature(SignatureId);
                    if (signature != null)
                    {
                        using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                        {
                            fs.Write(signature.Content, 0, signature.Content.Length);
                            fs.Close();
                        }
                    }
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 获取当前用户图像地址
        /// </summary>
        /// <returns></returns>
        public string GetUserImage()
        {
            OThinker.Organization.User user = this.ActionContext.User.User;
            // 获取用户图片
            string imagePath = UserValidator.GetUserImagePath(this.ActionContext.Engine, user, TempImagesPath);
            string avatar = "";
            if (user.Gender == Organization.UserGender.Male)
            {
                avatar = AppUtility.PortalRoot + "/img/TempImages/usermale.jpg";
            }
            else if (user.Gender == Organization.UserGender.Female)
            {
                avatar = AppUtility.PortalRoot + "/img/TempImages/userfemale.jpg";
            }
            else
            {
                avatar = AppUtility.PortalRoot + "/img/user.jpg";
            }

            if (!string.IsNullOrEmpty(imagePath))
            {
                avatar = this.GetImageUrlPath(imagePath) + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            return avatar;
        }

        string GetMobileReturnUrl()
        {
            string MobileReturnUrl = string.Empty;
            if (this.ActionContext.IsOriginateMode)
            {
                MobileReturnUrl = AppConfig.GetMobileOriginateInstancesUrl();
            }
            else if (this.ActionContext.IsWorkMode)
            {
                MobileReturnUrl = AppConfig.GetMobileUnfinishedWorkItemUrl();
            }
            else
            {
                MobileReturnUrl = AppConfig.GetMobileFinishedWorkItemUrl();
            }
            return MobileReturnUrl;
        }

        #endregion

        #region 保存表单数据 ----------------
        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="MvcPost"></param>
        /// <param name="MvcResult"></param>
        public void DoSaveAtion(MvcPostValue MvcPost, MvcResult MvcResult)
        {
            if (!this.ValidateData(MvcPost, ref MvcResult))
            {
                MvcResult.Successful = false;
                return;
            }

            this.SaveData(MvcPost, MvcResult);

            if (MvcResult.IsSaveAction)
            {// 保存操作，发起流程
                if (MvcResult.Successful && this.ActionContext.IsOriginateMode)
                {
                    // 创建流程实例
                    string url = string.Empty;
                    if (this.ActionContext.SheetDataType == SheetDataType.Workflow)
                    {
                        this.OriginateInstance(MvcPost, MvcResult, false, true);
                        url = AppConfig.GetWorkItemUrl(MvcResult.WorkItemId, this.ActionContext.IsMobile);
                    }
                    else
                    {
                        url = AppConfig.GetBizObjectUrl(this.ActionContext.SchemaCode,
                            this.ActionContext.BizObjectID,
                            this.ActionContext.Sheet.SheetCode,
                            this.ActionContext.IsMobile);
                    }
                    MvcResult.Url = url;
                }
                if (MvcResult.Successful)
                {
                    //如果是发起环节，可以选择兼职部门，更新一次InstanceContext对象
                    if (this.ActionContext.Workflow != null && this.ActionContext.Workflow.StartActivity.ActivityCode == this.ActionContext.ActivityCode && !this.ActionContext.IsOriginateMode)
                    {
                        Instance.InstanceContext context = this.ActionContext.Engine.InstanceManager.GetInstanceContext(this.ActionContext.InstanceId);

                        this.ActionContext.Engine.InstanceManager.SetOrgUnit(this.ActionContext.InstanceId, MvcPost.ParentUnitID);
                        this.ActionContext.Engine.InstanceManager.SetOriginator(this.ActionContext.InstanceId, this.GetOriginator(MvcPost));
                    }

                    // 保存HiddenFields
                    if (MvcPost.HiddenFields != null && MvcPost.HiddenFields.Count() > 0)
                    {
                        HeapDataItem.SetHiddenFields(this.ActionContext.Engine.HeapDataManager,
                            this.ActionContext.InstanceId, MvcPost.HiddenFields);
                    }
                }
            }
        }
        #endregion

        #region 提交表单数据
        /// <summary>
        /// 提交表单数据
        /// </summary>
        /// <param name="MvcPost"></param>
        /// <param name="MvcResult"></param>
        public void DoSubmitAction(MvcPostValue MvcPost, MvcResult MvcResult)
        {
            this.Approval = OThinker.Data.BoolMatchValue.True;
            // 保存数据
            // this.SaveData(MvcPost, MvcResult);
            if (MvcResult.Successful)
            {
                string message = string.Empty;
                if (!ValidateSubmitting(MvcPost, MvcResult.Errors))
                {// 提交验证不通过
                    MvcResult.Successful = false;
                    MvcResult.ClosePage = false;
                    return;
                }

                //提交任务时解锁锁定的表单
                UnLockActivity();

                // 提交任务
                if (this.ActionContext.IsOriginateMode)
                {
                    this.OriginateInstance(MvcPost, MvcResult, true, false);
                }
                else
                {
                    //设置是否显示同意/不同意
                    if (this.ActionContext.ActivityTemplate.IsApproveActivity//审批节点
                        && this.ActionContext.ActivityTemplate.ParticipateType == ActivityParticipateType.MultiParticipants//多人参与者
                        && ((ApproveActivity)this.ActionContext.ActivityTemplate).DisapproveExit != "1")//否决出口不等于1
                    {
                        MvcPost.Command = Configs.Global.ResourceManager.GetString("SheetComment_Approval");
                        this.ForwardWorkItem(MvcPost, this.Approval, WorkItem.ActionEventType.None, (int)SheetButtonType.Submit);
                    }
                    else
                    {
                        ForwardWorkItem(MvcPost, this.Approval, WorkItem.ActionEventType.Forward, (int)SheetButtonType.Submit);
                    }
                }

                // 保存HiddenFields
                if (MvcPost.HiddenFields != null && MvcPost.HiddenFields.Count() > 0)
                {
                    HeapDataItem.SetHiddenFields(this.ActionContext.Engine.HeapDataManager, this.ActionContext.InstanceId, MvcPost.HiddenFields);
                }
                if (this.ActionContext.IsMobile)
                {
                    MvcResult.Url = this.ActionContext.IsOriginateMode ?
                        AppConfig.GetMobileOriginateInstancesUrl() : AppConfig.GetMobileUnfinishedWorkItemUrl();
                }
                else if (string.IsNullOrWhiteSpace(MvcResult.Url))
                {
                    MvcResult.ClosePage = true;
                }
            }
        }

        /// <summary>
        /// 验证是否可以提交
        /// </summary>
        /// <param name="MvcPost"></param>
        /// <param name="Errors"></param>
        /// <returns></returns>
        protected bool ValidateSubmitting(MvcPostValue MvcPost, List<string> Errors)
        {
            // 验证提交
            bool submitLegal = true;
            if (this.ActionContext.WorkItem != null &&
                (this.ActionContext.WorkItem.ItemType == WorkItem.WorkItemType.Circulate
                || this.ActionContext.WorkItem.ItemType == WorkItem.WorkItemType.Read
                    || this.ActionContext.WorkItem.ItemType == WorkItem.WorkItemType.WorkItemAssist
                    || this.ActionContext.WorkItem.ItemType == WorkItem.WorkItemType.ActivityAssist
                    || this.ActionContext.WorkItem.ItemType == WorkItem.WorkItemType.ActivityConsult
                    || this.ActionContext.WorkItem.ItemType == WorkItem.WorkItemType.WorkItemConsult))
            {
                // 传阅类型，不需要判断
                return true;
            }
            WorkflowTemplate.ParticipativeActivity task = this.ActionContext.ActivityTemplate as WorkflowTemplate.ParticipativeActivity;
            if (task != null && task.SubmittingValidation == SubmittingValidationType.CheckNextParIsNull)
            {
                // 检查后续的活动是否有参与者
                if (!string.IsNullOrEmpty(MvcPost.DestActivityCode))
                {
                    Activity destActivity = this.ActionContext.Workflow.GetActivityByCode(MvcPost.DestActivityCode);
                    if (destActivity.IsClient)
                    {
                        // 人工参与活动检测下一个活动节点参与者是否为空
                        string[] participants = ((ClientActivity)destActivity).ParseParticipants(this.ActionContext.InstanceData, this.ActionContext.Engine.Organization);
                        if (participants == null || participants.Length == 0)
                        {
                            submitLegal = false;
                            Errors.Add(string.Format(Configs.Global.ResourceManager.GetString("SheetUtility_ParticipantInvalid"), destActivity.DisplayName));
                        }
                    }
                }
                else
                {
                    // 获得后续的活动
                    OThinker.H3.WorkflowTemplate.Activity[] activities = this.ActionContext.Workflow.GetActivityPostActivities(this.ActionContext.ActivityCode);
                    if (activities != null)
                    {
                        foreach (OThinker.H3.WorkflowTemplate.Activity activity in activities)
                        {
                            // 检查每一个活动
                            if (activity.IsClient)
                            {
                                string[] participants = ((ClientActivity)activity).ParseParticipants(this.ActionContext.InstanceData, this.ActionContext.Engine.Organization);
                                if (participants == null || participants.Length == 0)
                                {
                                    submitLegal = false;
                                    Errors.Add(string.Format(Configs.Global.ResourceManager.GetString("SheetUtility_ParticipantInvalid"), activity.DisplayName));
                                }
                            }
                        }
                    }
                }
            }
            return submitLegal;
        }
        #endregion

        #region 驳回操作
        /// <summary>
        /// 驳回操作
        /// </summary>
        /// <param name="MvcPost"></param>
        /// <param name="MvcResult"></param>
        public void DoReject(MvcPostValue MvcPost, MvcResult MvcResult)
        {
            this.Approval = OThinker.Data.BoolMatchValue.False;
            // 保存数据
            // this.SaveData(MvcPost, MvcResult);
            if (MvcResult.Successful)
            {
                // 保存HiddenFields
                if (MvcPost.HiddenFields != null && MvcPost.HiddenFields.Count() > 0)
                {
                    HeapDataItem.SetHiddenFields(this.ActionContext.Engine.HeapDataManager, this.ActionContext.InstanceId
                        , MvcPost.HiddenFields);
                }

                // 提交任务
                //设置是否显示同意/不同意
                if (this.ActionContext.ActivityTemplate.IsApproveActivity//审批节点
                    && this.ActionContext.ActivityTemplate.ParticipateType == ActivityParticipateType.MultiParticipants//多人参与者
                    && ((ApproveActivity)this.ActionContext.ActivityTemplate).DisapproveExit != "1")//否决出口不等于1
                {
                    MvcPost.Command = Configs.Global.ResourceManager.GetString("SheetComment_Disapproval");
                    MvcPost.DestActivityCode = "";
                    this.ForwardWorkItem(MvcPost, this.Approval, WorkItem.ActionEventType.None, (int)SheetButtonType.Submit);
                }
                else
                {
                    this.ForwardWorkItem(MvcPost, this.Approval, WorkItem.ActionEventType.Backward, (int)SheetButtonType.Return);
                }
            }
            MvcResult.ClosePage = true;
        }
        #endregion

        #region 取消流程事件
        /// <summary>
        /// 取消流程事件
        /// </summary>
        public MvcResult DoCancelInstance()
        {
            MvcResult MvcResult = new MvcResult();
            // 取消该流程
            OThinker.H3.Messages.CancelInstanceMessage cancelMessage =
                    new OThinker.H3.Messages.CancelInstanceMessage(
                        OThinker.H3.Messages.MessageEmergencyType.Normal,
                        this.ActionContext.InstanceId,
                        true);
            OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.SendMessage(cancelMessage);

            this.AddUserLog(Tracking.UserLogType.CancelInstance);

            MvcResult.Successful = true;
            MvcResult.ClosePage = true;
            this.AddUserLog(Tracking.UserLogType.CancelInstance);
            return MvcResult;
        }
        #endregion

        #region 取回流程操作
        /// <summary>
        /// 取回流程操作
        /// </summary>
        /// <returns></returns>
        public MvcResult DoRetrieveInstance()
        {
            MvcResult MvcResult = new MvcResult();
            int postTokenId = 0;
            string postActivityName = "";
            if (!this.GetPostTokenId(MvcResult, this.ActionContext.WorkItem.TokenId, ref postTokenId, ref postActivityName))
            {
                return MvcResult;
            }

            // 记录流程ID，TokenID和参与者信息
            string instanceId = this.ActionContext.WorkItem.InstanceId;
            int tokenId = this.ActionContext.WorkItem.TokenId;
            string participant = this.ActionContext.WorkItem.Participant;

            OThinker.H3.Messages.CancelActivityMessage rollback
                = new OThinker.H3.Messages.CancelActivityMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    this.ActionContext.WorkItem.InstanceId,
                    postActivityName,
                    true);
            this.ActionContext.Engine.InstanceManager.SendMessage(rollback);

            // 记录操作日志
            this.AddUserLog(Tracking.UserLogType.Retrieve);

            // 读取新的工作项
            // 获得该实例的任务
            string[] items = null;
            for (int triedTimes = 0; triedTimes < 10; triedTimes++)
            {
                System.Threading.Thread.Sleep(1500);

                items = this.ActionContext.Engine.Query.QueryWorkItems(
                    new string[] { instanceId },
                    new string[] { participant },
                    System.DateTime.MinValue,
                    System.DateTime.MaxValue,
                    OThinker.H3.WorkItem.WorkItemState.Unfinished,
                    OThinker.H3.WorkItem.WorkItem.NullWorkItemID);
                if (items != null && items.Length != 0)
                {
                    break;
                }
            }
            if (items == null || items.Length == 0)
            {
                MvcResult.ClosePage = true;
            }
            else
            {
                MvcResult.ClosePage = false;
                MvcResult.WorkItemId = items[0];
                MvcResult.Url = AppConfig.GetWorkItemUrl(MvcResult.WorkItemId, this.ActionContext.IsMobile);
            }
            return MvcResult;
        }

        /// <summary>
        /// 获取后置的路由TOKEN
        /// </summary>
        /// <param name="MvcResult"></param>
        /// <param name="curTokenId"></param>
        /// <param name="postTokenId"></param>
        /// <param name="activity"></param>
        /// <returns></returns>
        private bool GetPostTokenId(MvcResult MvcResult, int curTokenId, ref int postTokenId, ref string activity)
        {
            // 获得后继的Token
            int[] postTokenIds = this.ActionContext.InstanceContext.GetPostTokens(curTokenId, TokenState.Undropped);
            //判断后继任务是否可以取回
            if (postTokenIds == null || postTokenIds.Length == 0)
            {// 已经取回过了
                MvcResult.Successful = false;
                MvcResult.Errors.Add(Configs.Global.ResourceManager.GetString("MvcController_Retrieve"));
                return false;
            }

            if (postTokenIds.Length > 1)
            {// 后继活动是发散活动
                MvcResult.Successful = false;
                MvcResult.Errors.Add(Configs.Global.ResourceManager.GetString("MvcController_Divergent"));
            }
            else
            {
                postTokenId = postTokenIds[0];
                OThinker.H3.Instance.IToken postToken = this.ActionContext.InstanceContext.GetToken(postTokenId);
                if (!this.ActionContext.Workflow.GetActivityByCode(postToken.Activity).IsClient)
                {
                    // 具备取回的条件
                    return GetPostTokenId(MvcResult, postTokenId, ref postTokenId, ref activity);
                }
                else if (!postToken.Retrievable)
                {
                    // 无法取回
                    MvcResult.Successful = false;
                    MvcResult.Errors.Add(Configs.Global.ResourceManager.GetString("MvcController_CannotRetrieve"));
                    return false;
                }
                activity = postToken.Activity;
                MvcResult.Successful = true;
            }
            return MvcResult.Successful;
        }
        #endregion

        #region 结束流程
        /// <summary>
        /// 结束流程
        /// </summary>
        public void DoFinishInstance(MvcPostValue MvcPost, MvcResult MvcResult)
        {
            MvcPost.DestActivityCode = this.ActionContext.Workflow.EndActivity.ActivityCode;
            // 结束掉流程
            OThinker.H3.WorkflowTemplate.Activity end = this.ActionContext.Workflow.EndActivity;
            // 提交流程至结束
            ForwardWorkItem(MvcPost, OThinker.Data.BoolMatchValue.Unspecified, WorkItem.ActionEventType.Adjust, (int)SheetButtonType.FinishInstance);

            //在非发起模式下，更新流程的优先级
            if (this.ActionContext.InstanceContext != null &&
                MvcPost.Priority != PriorityType.Unspecified &&
                this.ActionContext.InstanceContext.Priority != MvcPost.Priority)
            {
                this.ActionContext.Engine.InstanceManager.SetInstancePriority(
                    this.ActionContext.InstanceId,
                    MvcPost.Priority);
            }

            MvcResult.ClosePage = true;

            this.AddUserLog(Tracking.UserLogType.FinishInstance);
        }
        #endregion

        #region 验证数据

        /// <summary>
        /// 验证数据
        /// </summary>
        /// <param name="MvcPost"></param>
        /// <param name="MvcResult"></param>
        /// <returns></returns>
        public bool ValidateData(MvcPostValue MvcPost, ref MvcResult MvcResult)
        {
            // 数据项存在2个情况,1.数据项名称  2.数据项.子数据项
            foreach (FieldSchema field in this.ActionContext.Schema.Fields)
            {
                // 如果数据项设置不允许保存，那么不会被保存进去
                if (this.ActionContext.SheetDataType == SheetDataType.Workflow && !this.GetItemEditable(field.Name)) continue;
                if (field.LogicType == DataLogicType.BizObject)
                {
                    // 业务对象可以使用普通控件显示，例如：业务对象A.Code,可以使用文本框绑定数据项显示 A.Code
                    ValidateBizObject(MvcPost.BizObject, field, ref MvcResult);
                }
                else
                {
                    if (!MvcPost.BizObject.DataItems.ContainsKey(field.Name)) continue;
                    if (field.LogicType == DataLogicType.BizObjectArray)
                    {
                        // 处理业务对象数字逻辑
                        ValidateBizObjectArray(MvcPost.BizObject, field, ref MvcResult);
                    }
                    else if (field.LogicType == DataLogicType.Comment)
                    {

                    }
                    else if (field.LogicType == DataLogicType.Attachment)
                    {

                    }
                    else if (field.LogicType == DataLogicType.MultiParticipant)
                    {
                        if (!ValidateMultiParticipant(MvcPost.BizObject.DataItems[field.Name].V))
                        {
                            MvcResult.Errors.Add(field.DisplayName + ":输入格式不正确");
                        }
                    }
                    else
                    {

                        if (!ValidateRealType(MvcPost.BizObject.DataItems[field.Name].V, field.RealType))
                        {
                            MvcResult.Errors.Add(field.DisplayName + ":输入格式不正确");
                        }
                    }
                }
            }

            return MvcResult.Errors.Count == 0;
        }

        /// <summary>
        /// 验证业务对象
        /// </summary>
        /// <param name="BizObject"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        bool ValidateBizObject(MvcBizObject BizObject, FieldSchema field, ref MvcResult MvcResult)
        {
            BizObjectSchema childSchema = field.Schema;
            if (childSchema == null)
            {
                childSchema = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
            }

            string datafield = string.Empty;

            if (BizObject.DataItems.ContainsKey(field.Name))
            {
                List<Dictionary<string, object>> result = null;
                try
                {
                    result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(BizObject.DataItems[field.Name].V.ToString());
                }
                catch
                {
                    MvcResult.Errors.Add(field.DisplayName + ":保存失败");
                    return false;
                }

                if (result == null || result.Count == 0)
                {
                    return true;
                }

                int errorCount = 0;
                // BizObject
                foreach (FieldSchema f in childSchema.Fields)
                {
                    datafield = field.Name + "." + f.Name;
                    //业务对象至多一行,取result[0]即可
                    if (!result[0].ContainsKey(f.Name)) continue;
                    if (f.LogicType == DataLogicType.MultiParticipant)
                    {
                        if (!ValidateMultiParticipant(result[0][f.Name]))
                        {
                            MvcResult.Errors.Add(field.DisplayName + "." + f.DisplayName + ":输入格式不正确");
                            errorCount++;
                        }
                    }
                    //
                    else if (f.LogicType == DataLogicType.Attachment)
                    {

                    }
                    else
                    {
                        if (!ValidateRealType(result[0][f.Name], f.RealType))
                        {
                            MvcResult.Errors.Add(field.DisplayName + "." + f.DisplayName + ":输入格式不正确");
                            errorCount++;
                        }
                    }
                }
                return errorCount == 0;
            }
            {
                return true;
            }
        }

        /// <summary>
        /// 验证业务对象数组
        /// </summary>
        /// <param name="BizObject"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        bool ValidateBizObjectArray(MvcBizObject BizObject, FieldSchema field, ref MvcResult MvcResult)
        {
            List<Dictionary<string, object>> result = null;
            try
            {
                result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(BizObject.DataItems[field.Name].V.ToString());
            }
            catch
            {
                MvcResult.Errors.Add(field.DisplayName + ":保存失败");
                return false;
            }

            if (result == null || result.Count == 0)
            {
                return true;
            }

            DataModel.BizObjectSchema childSchema = null;
            if (this.ActionContext.Schema.GetProperty(field.Name) != null)
            {
                childSchema = this.ActionContext.Schema.GetProperty(field.Name).ChildSchema;
            }
            else
            {
                childSchema = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
            }
            if (childSchema != null)
            {
                int errorCount = 0;
                int rowIndex = 0;
                foreach (Dictionary<string, object> NameAndValues in result)
                {
                    //行数从1开始
                    rowIndex++;

                    foreach (string ColumnName in NameAndValues.Keys)
                    {
                        if (string.Compare(ColumnName, BizObjectSchema.PropertyName_ObjectID, true) == 0) continue;
                        if (!string.IsNullOrEmpty(childSchema.GetField(ColumnName).Formula)) continue;
                        if (!this.GetColumnVisible(ColumnName, field) || !this.GetColumnEditable(ColumnName, field))
                        {
                            continue;
                        }

                        // 附件
                        if (childSchema.GetProperty(ColumnName).LogicType == DataLogicType.Attachment)
                        {

                        }
                        else if (childSchema.GetProperty(ColumnName).LogicType == DataLogicType.MultiParticipant)
                        {
                            if (!ValidateMultiParticipant(NameAndValues[ColumnName]))
                            {
                                MvcResult.Errors.Add(field.DisplayName + ",第" + rowIndex + "条," + "字段\"" + childSchema.GetProperty(ColumnName).DisplayName + "\":" + "输入格式不正确");
                                errorCount++;
                            }
                        }
                        else
                        {
                            if (!ValidateRealType(
                                NameAndValues[ColumnName],
                                childSchema.GetProperty(ColumnName).RealType))
                            {
                                MvcResult.Errors.Add(field.DisplayName + ",第" + rowIndex + "条," + "字段\"" + childSchema.GetProperty(ColumnName).DisplayName + "\":" + "输入格式不正确");
                                errorCount++;
                            }
                        }
                    }
                }
                if (errorCount > 0)
                {
                    return false;
                }
            }

            // 更新到字段上
            if (this.ActionContext.InstanceData[field.Name].LogicType == Data.DataLogicType.BizObject
                || this.ActionContext.InstanceData[field.Name].LogicType == Data.DataLogicType.BizObjectArray)
            {

            }
            else
            {
                // 不支持的类型
                MvcResult.Errors.Add("This control can not be used for data type \"" + this.ActionContext.InstanceData[field.Name].LogicType + "\".");
                return false;
            }

            return true;
        }

        /// <summary>
        /// 验证是否可以转换为多人参与者
        /// </summary>
        /// <param name="SerializedValue">JSON序列化的值</param>
        /// <returns></returns>
        bool ValidateMultiParticipant(object SerializedValue)
        {
            try
            {
                object convertedObj = null;
                return OThinker.Data.Convertor.Convert(
                    Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(SerializedValue + string.Empty),
                    typeof(string[]),
                    ref convertedObj);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 验证是否可以转换为真实数据类型
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="RealType"></param>
        /// <returns></returns>
        bool ValidateRealType(object Value, Type RealType)
        {
            object convertedObj = null;
            return OThinker.Data.Convertor.Convert(Value, RealType, ref convertedObj);
        }

        #endregion

        #region 保存数据

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <returns></returns>
        public void SaveData(MvcPostValue MvcPost, MvcResult MvcResult)
        {
            // ERROR, 这个应该挪到统一的提交里面去，因为提交也要放到Interactor里面去
            //在非发起模式下，更新流程的优先级
            if (this.ActionContext.InstanceContext != null &&
                MvcPost.Priority != PriorityType.Unspecified &&
                this.ActionContext.InstanceContext.Priority != MvcPost.Priority)
            {
                this.ActionContext.Engine.InstanceManager.SetInstancePriority(
                    this.ActionContext.InstanceId,
                    MvcPost.Priority);
            }

            // 保存征询意见
            if (MvcPost.BizObject.DataItems.ContainsKey(SheetConsultComment))
            {
                SaveCommentData(MvcPost.BizObject, SheetConsultComment, MvcResult, null);
            }

            // 数据项存在2个情况,1.数据项名称  2.数据项.子数据项
            foreach (FieldSchema field in this.ActionContext.Schema.Fields)
            {
                // 如果数据项设置不允许保存，那么不会被保存进去
                if (this.ActionContext.SheetDataType == SheetDataType.Workflow && !this.GetItemEditable(field.Name)) continue;
                if (!MvcPost.BizObject.DataItems.ContainsKey(field.Name)) continue;
                if (field.LogicType == DataLogicType.BizObject || field.LogicType == DataLogicType.BizObjectArray) continue;

                if (field.LogicType == DataLogicType.Comment && int.Equals(MvcPost.WorkItemType, (int)WorkItemType.WorkItemAssist))
                {
                    // 协办处理审核意见和审批结果逻辑
                    SaveCommentData(MvcPost.BizObject, field.Name, MvcResult, MvcPost);
                    HttpContext.Current.Session.Remove(this.ActionContext.BizObjectID + field.Name);
                }
                else if (field.LogicType == DataLogicType.Comment)
                {
                    // 处理审核意见和审批结果逻辑
                    SaveCommentData(MvcPost.BizObject, field.Name, MvcResult, null);
                    HttpContext.Current.Session.Remove(this.ActionContext.BizObjectID + field.Name);
                }
                else if (field.LogicType == DataLogicType.Attachment)
                {
                    // 处理附件逻辑
                    SaveAttachmentData(MvcPost.BizObject, field);
                }
                else if (field.LogicType == DataLogicType.MultiParticipant)
                {
                    // 处理多人参与者逻辑
                    this.ActionContext.InstanceData[field.Name].Value = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(MvcPost.BizObject.DataItems[field.Name].V.ToString());
                }
                else
                {   // 处理普通数据项
                    this.ActionContext.InstanceData[field.Name].Value = MvcPost.BizObject.DataItems[field.Name].V;
                }
            }

            // 最后保存子表，因为关联关系的子表主键有可能在主表中，所以先保存主表
            foreach (FieldSchema field in this.ActionContext.Schema.Fields)
            {
                // 如果数据项设置不允许保存，那么不会被保存进去
                if (this.ActionContext.SheetDataType == SheetDataType.Workflow && !this.GetItemEditable(field.Name)) continue;
                if (!MvcPost.BizObject.DataItems.ContainsKey(field.Name)) continue;
                if (field.LogicType != DataLogicType.BizObject && field.LogicType != DataLogicType.BizObjectArray) continue;

                if (field.LogicType == DataLogicType.BizObject)
                {
                    // 业务对象可以使用普通控件显示，例如：业务对象A.Code,可以使用文本框绑定数据项显示 A.Code
                    SaveBizObject(MvcPost.BizObject, field);
                }
                else if (field.LogicType == DataLogicType.BizObjectArray)
                {
                    // 处理子表逻辑
                    SaveBizObjectArray(MvcPost.BizObject, field);
                    HttpContext.Current.Session.Remove(this.ActionContext.BizObjectID + field.Name);
                }
            }

            // 提交数据项的变更
            if (this.ActionContext.SheetMode == SheetMode.Originate && this.ActionContext.SheetDataType == SheetDataType.Workflow)
            {
                this.ActionContext.InstanceData.BizObject.RunningInstanceId = this.ActionContext.InstanceId;
            } 
                
            if (!this.ActionContext.InstanceData.Submit())
            {
                MvcResult.Successful = false;
                MvcResult.Errors.Add(Configs.Global.ResourceManager.GetString("MvcController_SaveFailed"));
            }
            this.AddUserLog(Tracking.UserLogType.SaveWorkSheet);
        }

        #endregion

        #region 启动流程 -------------------------------
        /// <summary>
        /// 获取所选择的发起人身份
        /// </summary>
        /// <param name="PostValue"></param>
        /// <returns></returns>
        private string GetOriginator(MvcPostValue PostValue)
        {
            if (string.IsNullOrEmpty(PostValue.ParentUnitID))
            {
                return this.ActionContext.Participant.UserID;
            }
            else
            {
                foreach (User user in this.ActionContext.User.RelationUsers)
                {
                    if (user.ParentID == PostValue.ParentUnitID) return user.ObjectID;
                }
            }
            //默认使用当前用户为发起人
            return this.ActionContext.Participant.UserID;
        }

        /// <summary>
        /// 启动流程
        /// </summary>
        /// <param name="PostValue">页面回发数据</param>
        /// <param name="MvcResult">返回执行结果</param>
        /// <param name="FinishStartActivity">是否结束开始活动</param>
        /// <param name="ReturnWorkItem">是否返回工作任务</param>
        public void OriginateInstance(MvcPostValue PostValue, MvcResult MvcResult, bool FinishStartActivity, bool ReturnWorkItem)
        {
            string instanceId = this.ActionContext.InstanceId;
            string workItemId = string.Empty;
            string error = string.Empty;

            // 创建流程并启动
            bool success = false;
            if (ReturnWorkItem)
            {
                success = SheetUtility.OriginateInstance(
                    this.ActionContext.Engine,
                    this.ActionContext.InstanceData.BizObject.ObjectID,
                    this.ActionContext.Workflow,
                    this.ActionContext.Schema,
                    ref instanceId,
                    this.GetOriginator(PostValue),
                    PostValue.PostValue,
                    PostValue.InstanceName,
                    PostValue.Priority == Instance.PriorityType.Unspecified ? PriorityType.Normal : PostValue.Priority,
                    null,
                    null,
                    this.Request,
                    ref workItemId,
                    ref error,
                    FinishStartActivity);
            }
            else
            {
                OThinker.H3.Messages.OriginateInstanceMessage instMessage = new Messages.OriginateInstanceMessage(
                   Messages.MessageEmergencyType.High,
                   this.ActionContext.BizObjectID,
                   this.ActionContext.WorkflowCode,
                   this.ActionContext.WorkflowVersion,
                   instanceId,
                   PostValue.InstanceName,
                   this.GetOriginator(PostValue),
                   PostValue.PostValue,
                   null,
                   PostValue.Priority == Instance.PriorityType.Unspecified ? PriorityType.Normal : PostValue.Priority,
                   false,
                   null,
                   null,
                   false,
                   H3.Instance.Token.UnspecifiedID,
                   null,
                   true,
                   PostValue.DestActivityCode,
                   null);
                this.ActionContext.Engine.InstanceManager.SendMessage(instMessage);
                success = true;
            }

            // 保存隐藏字段
            //this.ActionContext.SaveHiddenField();
            MvcResult.Successful = success;
            MvcResult.WorkItemId = workItemId;
            if (!string.IsNullOrWhiteSpace(error))
            {
                MvcResult.Errors.Add(error);
            }
        }

        #endregion

        #region 结束工作任务

        /// <summary>
        /// 结束工作任务
        /// </summary>
        /// <param name="MvcPost">MVC表单传递过来的值</param>
        /// <param name="Approval">审核结果</param>
        /// <param name="ActionEventType">事件类型</param>
        /// <param name="ActionButtonType">按钮类型</param>
        public void ForwardWorkItem(
            MvcPostValue MvcPost,
            OThinker.Data.BoolMatchValue Approval,
            WorkItem.ActionEventType ActionEventType,
            int ActionButtonType)
        {
            string activityCode = MvcPost.DestActivityCode;
            if (MvcPost.DestActivityCode == RejectTo_Start)
            {
                activityCode = this.ActionContext.Workflow.StartActivityCode;
            }
            else if (MvcPost.DestActivityCode == RejectTo_Previous)
            {
                // 获取上一个提交过来的活动节点
                IToken preToken = this.GetPreToken(this.ActionContext.WorkItem.TokenId);
                activityCode = preToken.Activity;
            }

            if (this.ActionContext.WorkItem != null)
            {
                this.ActionContext.Engine.Interactor.FinishWorkItem(
                this.ActionContext.WorkItemID,
                this.ActionContext.User.UserID,
                (this.ActionContext.IsMobile || SheetUtility.IsMobileDevice(this.Request)) ? H3.WorkItem.AccessPoint.Mobile : H3.WorkItem.AccessPoint.Web,
                MvcPost.PostValue,
                this.Approval,
                this.Comment,
                MvcPost.Command,
                ActionEventType,
                ActionButtonType,
                activityCode,
                SheetUtility.GetClientIP(this.Request),
                SheetUtility.GetClientPlatform(this.Request),
                SheetUtility.GetClientBrowser(this.Request));
                this.ActionContext.WorkItem.State = OThinker.H3.WorkItem.WorkItemState.Finished;
            }
            else
            {
                this.ActionContext.Engine.WorkItemManager.FinishCirculateItem(this.ActionContext.CirculateItem.ObjectID, this.ActionContext.User.UserID,
                    (this.ActionContext.IsMobile || SheetUtility.IsMobileDevice(this.Request)) ? H3.WorkItem.AccessPoint.Mobile : H3.WorkItem.AccessPoint.Web);
                this.ActionContext.CirculateItem.State = OThinker.H3.WorkItem.WorkItemState.Finished;
            }
        }

        #endregion

        #region 获取前一个活动
        /// <summary>
        /// 获取前一个活动
        /// </summary>
        /// <param name="TokenId"></param>
        /// <returns></returns>
        public OThinker.H3.Instance.IToken GetPreToken(int TokenId)
        {
            // 通过日志获得当前步骤令牌
            H3.Instance.IToken token = this.ActionContext.InstanceContext.GetToken(TokenId);
            IToken preToken = this.GetPreToken(token);
            if (preToken != null)
            {
                Activity preActivity = this.ActionContext.Workflow.GetActivityByCode(preToken.Activity);
                if (preActivity.ActivityType == ActivityType.FillSheet
                    || preActivity.ActivityType == ActivityType.Approve
                    || preActivity.ActivityType == ActivityType.SubInstance)
                {
                    return preToken;
                }
                else
                {
                    return GetPreToken(preToken.TokenId);
                }
            }

            // 继续检测该活动的历史活动，直到找到最近的一次提交源为止
            IToken[] historyTokens = this.ActionContext.InstanceContext.GetTokens(this.ActionContext.ActivityCode, TokenState.Finished);
            if (historyTokens != null)
            {
                foreach (IToken historyToken in historyTokens)
                {
                    preToken = this.GetPreToken(historyToken);
                    if (preToken != null) return preToken;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取某个活动的前驱活动
        /// </summary>
        /// <param name="Token"></param>
        /// <returns></returns>
        public OThinker.H3.Instance.IToken GetPreToken(IToken Token)
        {
            if (Token == null || Token.PreTokens == null || Token.PreTokens.Length != 1) return null;
            int preTokenId = Token.PreTokens[0];
            H3.Instance.IToken preToken = this.ActionContext.InstanceContext.GetToken(preTokenId);
            if (preToken.Activity == this.ActionContext.ActivityCode)
            {// 前一个活动的节点就是本节点，禁止打回
                return null;
            }
            if (preToken.Approval != OThinker.Data.BoolMatchValue.False)
            {// 前一个活动是提交，则返回当前活动
                return preToken;
            }
            // 以下条件注释，因为存在2个情况
            // 情况1：A->B->A->C，A是跳过，C驳回上一步应该仍然是A比较合理
            // 情况2: A->业务动作->B，B驳回上一步，应该是驳回到业务动作节点比较合理，业务流程系统、人工参与都是相同级别活动
            //if (preToken.SkippedExecution)//  || !this.Enviroment.Workflow.GetActivityByCode(preToken.Activity).IsClient)
            //{// 前一个步骤是跳过的
            //    return this.GetPreToken(preToken.TokenId);
            //}
            return null;
        }
        #endregion

        #region 记录用户日志
        /// <summary>
        /// 添加用户日志
        /// </summary>
        /// <param name="UserLogType">日志类型</param>
        public void AddUserLog(Tracking.UserLogType UserLogType)
        {
            this.ActionContext.WriteLog(UserLogType);
        }
        #endregion

        #region 如果要征询意见/协办/传阅，那么可选的征询意/协办/传阅的人员由这里获得
        /// <summary>
        /// 如果要征询意见/协办/传阅，那么可选的征询意/协办/传阅的人员由这里获得
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, UserOptions> GetOptionalRecipients()
        {
            //Dictionary<string, SheetUserOptions> OptionalRecipients = new Dictionary<string, SheetUserOptions>();
            //OptionalRecipients.Add(
            //    OThinker.H3.Controllers.SelectRecipientType.Forward.ToString(),
            //    new SheetUserOptions()
            //    {
            //        GroupVisible = true,
            //        PlaceHolder = "转发人",
            //        UserVisible = false
            //    });

            //return OptionalRecipients;
            return null;
        }
        #endregion

        #region 数据项权限控制接口 ---------------
        /// <summary>
        /// 获取或设置数据项是否可编辑
        /// </summary>
        /// <param name="ItemName">数据项名称</param>
        /// <returns></returns>
        public virtual bool GetItemEditable(string ItemName)
        {
            return this.ClientActivity.GetItemEditable(ItemName);
        }

        /// <summary>
        /// 获取或设置数据项是否可见
        /// </summary>
        /// <param name="ItemName">数据项名称</param>
        /// <returns></returns>
        public virtual bool GetItemVisible(string ItemName)
        {
            return this.ClientActivity.GetItemVisible(ItemName);
        }

        /// <summary>
        /// 获取或设置数据项痕迹是否可见
        /// </summary>
        /// <param name="ItemName">数据项名称</param>
        /// <returns></returns>
        public virtual bool GetItemTrackVisible(string ItemName)
        {
            return this.ClientActivity.GetItemTrackVisible(ItemName);
        }

        /// <summary>
        /// 获取或设置数据项是否必填
        /// </summary>
        /// <param name="ItemName">数据项名称</param>
        /// <returns></returns>
        public virtual bool GetItemRequired(string ItemName)
        {
            return this.ClientActivity.GetItemRequired(ItemName);
        }

        private OThinker.Data.BoolMatchValue _Approval = OThinker.Data.BoolMatchValue.True;
        /// <summary>
        /// 获取或设置当前任务审批结果
        /// </summary>
        public virtual OThinker.Data.BoolMatchValue Approval
        {
            get
            {
                return this._Approval;
            }
            set
            {
                this._Approval = value;
            }
        }

        private string _Comment = null;
        /// <summary>
        /// 获取或设置当前任务审批意见
        /// </summary>
        public virtual string Comment
        {
            get
            {
                return this._Comment;
            }
            set
            {
                this._Comment = value;
            }
        }

        ///// <summary>
        ///// 流程名称编辑控件
        ///// </summary>
        //public virtual SheetInstanceNameEditor InstanceNameEditor { get; set; }

        ///// <summary>
        ///// 设置数据项的值
        ///// </summary>
        ///// <param name="ItemName"></param>
        ///// <param name="Value"></param>
        ///// <returns></returns>
        //public virtual bool SetItemValue(string ItemName, object Value)
        //{
        //    return SheetUtility.SetItemValue(this.Enviroment, ItemName, Value, ref this._Approval, ref this._Comment);
        //}

        ///// <summary>
        ///// 获取数据项的值
        ///// </summary>
        ///// <param name="ItemName"></param>
        ///// <param name="Value"></param>
        ///// <returns></returns>
        //public virtual bool GetItemValue(string ItemName, ref object Value)
        //{
        //    return false;
        //}
        #endregion

        #region 加载数据相关接口

        #region 加载操作权限
        private PermittedActions GetPermittedActions(WorkflowTemplate.ParticipativeActivity activity,
            bool ApprovalListVisible)
        {
            if ((activity == null
                || (this.ActionContext.ActivityTemplate != null && this.ActionContext.ActivityTemplate.ActivityType == ActivityType.Circulate)
                || (this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.ItemType == WorkItemType.Circulate)) || this.ActionContext.CirculateItem != null)
            {//传阅节点，没有操作权限，默认添加已阅、传阅功能
                return new PermittedActions()
                {
                    ViewInstance = true,
                    Print = !this.ActionContext.IsMobile,
                    Viewed = (this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.IsUnfinished) || (this.ActionContext.CirculateItem != null && this.ActionContext.CirculateItem.IsUnfinished),
                    Circulate = (this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.PermittedActions.Recirculate) || this.ActionContext.CirculateItem != null,
                    Close = !this.ActionContext.IsMobile
                };
            }

            if (this.ActionContext.WorkItem != null
                && (this.ActionContext.WorkItem.IsAssistive//协办
                || this.ActionContext.WorkItem.IsConsultive//征询
                ))
            {
                return new PermittedActions()
                {
                    ViewInstance = true,
                    Print = !this.ActionContext.IsMobile,
                    Assist = this.ActionContext.IsWorkMode && this.ActionContext.WorkItem.IsAssistive,
                    Consult = this.ActionContext.IsWorkMode && this.ActionContext.WorkItem.IsConsultive,
                    Submit = this.ActionContext.IsWorkMode,
                    Close = !this.ActionContext.IsMobile//false//!this.ActionContext.IsMobile
                };
            }

            return new PermittedActions()
                {
                    AdjustParticipant = activity == null || this.ActionContext.IsOriginateMode ? false : this.ActionContext.IsWorkMode && activity.PermittedActions.AdjustParticipant && this.ActionContext.ActivityTemplate.ParticipateType == ActivityParticipateType.MultiParticipants && ((ParticipativeActivity)this.ActionContext.ActivityTemplate).ParticipateMethod == ParticipateMethod.Parallel,//改为加签
                    Assist = activity == null || this.ActionContext.IsOriginateMode ? false : this.ActionContext.IsWorkMode && activity.PermittedActions.Assist,
                    Circulate = activity == null || this.ActionContext.IsOriginateMode ? false : activity.PermittedActions.Circulate,
                    Recirculate = activity == null || this.ActionContext.IsOriginateMode ? false : activity.PermittedActions.Recirculate,
                    Consult = activity == null || this.ActionContext.IsOriginateMode ? false : this.ActionContext.IsWorkMode && activity.PermittedActions.Consult && !this.ActionContext.WorkItem.IsConsultive,
                    FinishInstance = activity == null || this.ActionContext.IsOriginateMode ? false : this.ActionContext.IsWorkMode && activity.PermittedActions.FinishInstance,
                    Forward = activity == null || this.ActionContext.IsOriginateMode ? false : this.ActionContext.IsWorkMode && activity.PermittedActions.Forward,
                    PreviewParticipant = activity == null ? false : activity.PermittedActions.PreviewParticipant,
                    Reject = activity == null || this.ActionContext.IsOriginateMode ? false : this.ActionContext.IsWorkMode
                            && this.ActionContext.Workflow.StartActivity.ActivityCode != this.ActionContext.ActivityCode
                            && (ApprovalListVisible || activity.PermittedActions.Reject || activity.PermittedActions.RejectToFixed || activity.PermittedActions.RejectToStart),
                    Close = this.ActionContext.SheetDataType == SheetDataType.BizObject || !this.ActionContext.IsMobile,
                    Print = !this.ActionContext.IsOriginateMode && !this.ActionContext.IsMobile,
                    Save = this.ActionContext.IsOriginateMode || this.ActionContext.IsWorkMode,//发起模式或工作模式
                    Submit = ApprovalListVisible || this.ActionContext.IsOriginateMode
                        || (this.ActionContext.IsWorkMode && this.ActionContext.SheetDataType == SheetDataType.Workflow),
                    ViewInstance = this.ActionContext.SheetDataType == SheetDataType.Workflow,
                    RetrieveInstance = IsRetrieveable(activity),
                    Choose = activity == null ? false : activity.PermittedActions.Choose,
                    CancelInstance = this.ActionContext.WorkItem == null || this.ActionContext.InstanceContext.State == InstanceState.Canceled ? false :
                        ((this.ActionContext.WorkItem.State == WorkItemState.Waiting || this.ActionContext.WorkItem.State == WorkItemState.Working) && activity.PermittedActions.CancelIfUnfinished)
                            || (this.ActionContext.WorkItem.State == WorkItemState.Finished && activity.PermittedActions.CancelIfFinished)
                };
        }

        /// <summary>
        /// 获取某个活动节点是否允许被取回
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private bool IsRetrieveable(ParticipativeActivity activity)
        {
            // 任务为空，节点不允许取回或者流程不处于运行状态，则不可见取回操作
            if (this.ActionContext.WorkItem == null) return false;
            if (!activity.PermittedActions.Retrieve) return false;
            if (this.ActionContext.InstanceContext.State != InstanceState.Running) return false;
            if (this.ActionContext.WorkItem.State != WorkItemState.Finished) return false;
            int[] tokens = this.ActionContext.InstanceContext.GetPostTokens(this.ActionContext.WorkItem.TokenId);
            foreach (int tokenId in tokens)
            {
                IToken token = this.ActionContext.InstanceContext.GetToken(tokenId);
                if (token.State == TokenState.Dropped) continue;
                if (!token.Retrievable) return false;
            }
            return true;
        }
        #endregion

        #region 加载群组 ---------------

        /// <summary>
        /// 加载群组
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private List<MvcListItem> LoadPosts(PublishedWorkflowTemplate workflowTemplate)
        {
            if (workflowTemplate == null) return new List<MvcListItem>();
            List<MvcListItem> MvcListItem = null;
            if (this.ActionContext.IsWorkMode || this.ActionContext.IsOriginateMode)
            {
                if (workflowTemplate.RequirePost)
                {   //添加角色数据
                    //Organization.Unit[] units = this.ActionContext.PostUnits;
                    List<string> lstPosts = this.ActionContext.Engine.Organization.GetParents(this.ActionContext.User.UserID, UnitType.Post, true, State.Active);

                    MvcListItem = new List<MvcListItem>();
                    if (lstPosts != null && lstPosts.Count > 0)
                    {
                        foreach (string unitid in lstPosts)
                        {
                            Organization.Unit unit = this.ActionContext.Engine.Organization.GetUnit(unitid);
                            MvcListItem.Add(new MvcListItem(unit.ObjectID, unit.Name));
                        }
                    }
                }
            }

            return MvcListItem;
        }
        #endregion

        #region 获取当前任务可以提交的活动节点
        /// <summary>
        /// 获取当前任务可以提交的活动节点
        /// </summary>
        /// <returns></returns>
        private List<MvcListItem> GetSubmitActivies()
        {
            // TODO:增加过滤节点重写方法
            List<MvcListItem> activities = new List<MvcListItem>();

            WorkflowTemplate.ParticipativeActivity activity = this.ActionContext.ActivityTemplate as WorkflowTemplate.ParticipativeActivity;
            if (activity == null)
            {
                return activities;
            }

            OThinker.H3.WorkflowTemplate.Activity[] postActivitys = this.ActionContext.Workflow.GetActivityPostActivities(this.ActionContext.ActivityCode);
            bool allowSubmitRejectActivity = this.ActionContext.WorkItem != null
                && this.ActionContext.WorkItem.PreActionEventType == H3.WorkItem.ActionEventType.Backward
                && this.ActionContext.WorkItem.PermittedActions.SubmitToRejectedActivity;

            if (//this.ActionContext.WorkItem != null && 
                postActivitys != null &&
                (
                    (this.ActionContext.SheetMode == SheetMode.Originate && activity.PermittedActions.Choose)
                    || (this.ActionContext.SheetMode == SheetMode.Work && this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.PermittedActions.Choose)
                    || allowSubmitRejectActivity
                ))
            {// 活动允许手工选择，添加后续活动
                foreach (OThinker.H3.WorkflowTemplate.Activity postActivity in postActivitys)
                {
                    activities.Add(new MvcListItem(postActivity.ActivityCode, postActivity.DisplayName));
                }
            }

            // 允许直线提交
            if (allowSubmitRejectActivity)
            {
                int[] preTokens = this.ActionContext.InstanceContext.GetPreTokens(this.ActionContext.WorkItem.TokenId);
                if (preTokens != null && preTokens.Length > 0)
                {
                    string code = this.ActionContext.InstanceContext.GetToken(preTokens[preTokens.Length - 1]).Activity;
                    if (code != this.ActionContext.ActivityCode &&
                        activities.Find((item) => { return item.Code == code; }) == null)
                    {
                        activities.Add(new MvcListItem(code, this.ActionContext.Workflow.GetActivityByCode(code).DisplayName));
                    }
                }
            }

            // 活动允许跳转
            if ((this.ActionContext.SheetMode == SheetMode.Originate && activity.PermittedActions.Jump)
                || (this.ActionContext.WorkItem != null && this.ActionContext.SheetMode == SheetMode.Work && this.ActionContext.WorkItem.PermittedActions.Jump))
            {
                foreach (OThinker.H3.WorkflowTemplate.Activity processActivity in this.ActionContext.Workflow.Activities)
                {
                    if (processActivity.ActivityType == WorkflowTemplate.ActivityType.Start
                        || processActivity.ActivityType == ActivityType.End) continue;


                    if (processActivity.ActivityCode != this.ActionContext.ActivityCode &&
                        activities.Find((item) => { return item.Code == processActivity.ActivityCode; }) == null)
                    {
                        activities.Add(new MvcListItem(processActivity.ActivityCode, processActivity.DisplayName));
                    }
                }
            }
            return activities;
        }
        #endregion

        #region 获取当前任务可以驳回的活动节点
        /// <summary>
        /// 获取当前任务可以驳回的活动节点
        /// </summary>
        /// <returns></returns>
        private List<MvcListItem> GetRejectActivies()
        {
            // TODO:增加过滤节点重写方法
            List<MvcListItem> activities = new List<MvcListItem>();
            if (this.ActionContext.SheetMode == SheetMode.View
                || this.ActionContext.SheetMode == SheetMode.Print
                || this.ActionContext.SheetMode == SheetMode.Originate
                || this.ActionContext.SheetMode == SheetMode.Unspecified
                || this.ActionContext.WorkItem == null
                || !this.ActionContext.WorkItem.IsParticipative)
            {
                return activities;
            }

            // 允许驳回到开始
            if (this.ActionContext.WorkItem.PermittedActions.RejectToStart)
            {
                string text = Configs.Global.ResourceManager.GetString("SheetActionPane_ReturnToStart");
                activities.Add(new MvcListItem(RejectTo_Start, text));
            }
            // 允许驳回到上一步
            if (this.ActionContext.WorkItem.PermittedActions.Reject)
            {
                string text = Configs.Global.ResourceManager.GetString("SheetActionPane_ReturnToPrevious");

                activities.Add(new MvcListItem(RejectTo_Previous, text));
            }
            // 允许驳回到指定的活动环节
            if (this.ActionContext.WorkItem.PermittedActions.RejectToFixed)
            {
                OThinker.H3.WorkflowTemplate.ParticipativeActivity activity = (OThinker.H3.WorkflowTemplate.ParticipativeActivity)this.ActionContext.Workflow.GetActivityByCode(this.ActionContext.WorkItem.ActivityCode);
                string[] RejectToActivityCodes = activity.PermittedActions.RejectToActivityCodes;
                if (RejectToActivityCodes != null)
                {
                    // 获得所有已经执行过的活动
                    Instance.IToken[] tokens = this.ActionContext.InstanceContext.Tokens;
                    string activityText;
                    foreach (Instance.IToken token in tokens)
                    {
                        if (
                            // 已经完成了的
                            token.State == Instance.TokenState.Finished &&
                            // 活动未被包含
                            activities.Find((item) => { return item.Code == token.Activity; }) == null)
                        // 人参与的活动，允许驳回到连接点
                        // && this.ActionContext.Workflow.GetActivityByCode(token.Activity).IsClient)
                        {
                            if (Array.IndexOf<string>(RejectToActivityCodes, token.Activity) > -1)
                            {
                                activityText = "驳回到" + this.ActionContext.Workflow.GetActivityByCode(token.Activity).DisplayName;
                                activities.Add(new MvcListItem(token.Activity, activityText));
                            }
                        }
                    }
                }
            }
            return activities;
        }
        #endregion

        #region 获取可以调整的活动节点集合
        /// <summary>
        /// 获取可以调整的活动节点集合
        /// </summary>
        /// <returns></returns>
        private List<MvcListItem> GetAdjustActivities()
        {
            List<MvcListItem> items = new List<MvcListItem>();
            if (this.ActionContext.WorkItem == null)
            {
                foreach (Activity Activity in this.ActionContext.Workflow.Activities)
                {
                    if (Activity.ActivityType == H3.WorkflowTemplate.ActivityType.Start) continue;
                    items.Add(new MvcListItem(Activity.ActivityCode, Activity.DisplayName));
                }
            }
            else
            {
                items.Add(new MvcListItem(this.ActionContext.WorkItem.ActivityCode, this.ActionContext.WorkItem.DisplayName));
            }
            return items;
        }
        #endregion

        #region 加载业务对象数据

        /// <summary>
        /// 加载业务对象数据
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private MvcBizObject LoadBizObjectData(FieldSchema field)
        {
            BizObject bo = this.ActionContext.InstanceData[field.Name].Value as BizObject;
            MvcBizObject mbo = new MvcBizObject();
            if (bo == null)
            {
                BizObjectSchema schema = field.Schema;
                if (schema == null)
                {
                    schema = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                }
                bo = new BizObject(AppUtility.Engine, schema, this.ActionContext.User.UserID);
            }
            foreach (PropertySchema p in bo.Schema.Properties)
            {
                mbo.DataItems.Add(field.Name + "." + p.Name, GetMvcDataFromProperty(field, p, bo));
            }
            return mbo;
        }

        #endregion
        
        #region 加载业务对象数组数据
        /// <summary>
        /// 加载业务对象数组数据
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        private object LoadBizObjectArrayData(FieldSchema field)
        {
            BizObject[] boValues = this.GetBOsValue(this.ActionContext.InstanceData.BizObject, field);// this.ActionContext.InstanceData[field.Name].Value as BizObject[];
            MvcDataItem dataItem = new MvcDataItem(this.ActionContext.InstanceData[field.Name], SheetLocked);
            MvcBizObjectList mboList = new MvcBizObjectList();

            //添加一行默认行
            DataModel.BizObjectSchema childSchema = null;
            if (this.ActionContext.Schema.GetProperty((field.Name)) != null)
            {
                childSchema = this.ActionContext.Schema.GetProperty(field.Name).ChildSchema;
            }
            else
            {
                childSchema = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
            }

            BizObject newbo = null;
            if (childSchema != null)
            {
                newbo = new BizObject(this.ActionContext.Engine, childSchema, this.ActionContext.User.UserID)
                {
                    ObjectID = Guid.NewGuid().ToString()
                };
            }
            MvcBizObject mnewbo = new MvcBizObject();
            foreach (PropertySchema p in newbo.Schema.Properties)
            {
                mnewbo.DataItems.Add(field.Name + "." + p.Name, GetMvcDataFromProperty(field, p, newbo));
            }
            if (boValues == null && this.ActionContext.IsOriginateMode)
            {
                boValues = new BizObject[1] { newbo };
            }

            if (boValues != null)
            {
                foreach (BizObject bo in boValues)
                {
                    MvcBizObject mbo = new MvcBizObject();
                    foreach (PropertySchema p in bo.Schema.Properties)
                    {
                        MvcDataItem item = GetMvcDataFromProperty(field, p, bo);
                        mbo.DataItems.Add(field.Name + "." + p.Name, item);
                    }
                    mboList.Add(mbo);
                }
            }
            object V = new { R = mboList, T = mnewbo };
            return V;
        }
        #endregion

        #region 加载审批控件数据
        private string _TempImagesPath = string.Empty;
        /// <summary>
        /// 获取临时文件存储目录
        /// </summary>
        public string TempImagesPath
        {
            get
            {
                if (this._TempImagesPath == string.Empty)
                {
                    _TempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("~/"), "TempImages");
                }
                return _TempImagesPath;
            }
        }

        /// <summary>
        /// 获取图片URL路径
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private string GetImageUrlPath(string imagePath)
        {
            int length = this.Request.Path.Split('/').Length;
            string path = string.Empty;
            if (length > 3)
            {
                for (int i = 3; i < length; i++)
                {
                    path += "../";
                }
            }
            return path + "TempImages/" + imagePath;
        }

        /// <summary>
        /// 加载征询意见数据
        /// </summary>
        /// <param name="ObjectID"></param>
        /// <param name="customName"></param>
        /// <param name="datafield"></param>
        /// <param name="historyComments"></param>
        /// <param name="dt"></param>
        /// <param name="consultState"></param>
        /// <returns></returns>
        private List<object> ConsultComments(string ObjectID, string customName, string datafield, List<object> historyComments, DataTable dt, bool consultState)
        {
            WorkItem.WorkItem workitem = null;
            Organization.User user = null;
            Organization.User delegant = null;
            if (string.Equals(customName, WorkItem.WorkItem.PropertyName_ObjectID) && !this.ActionContext.WorkItem.Consulted && !this.ActionContext.WorkItem.ConsultantFinished)
            {
                if (!string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.SourceWorkItemID))
                {
                    workitem = this.ActionContext.Engine.WorkItemManager.GetWorkItem(this.ActionContext.WorkItem.SourceWorkItemID); //this.ActionContext.WorkItemID
                    user = this.ActionContext.Engine.Organization.GetUnit(workitem.Participant) as OThinker.Organization.User; //this.ActionContext.WorkItem.Participant 
                }
            }
            else
            {
                workitem = this.ActionContext.Engine.WorkItemManager.GetWorkItem(ObjectID); //this.ActionContext.WorkItemID
                user = this.ActionContext.Engine.Organization.GetUnit(workitem.Participant) as OThinker.Organization.User; //this.ActionContext.WorkItem.Participant 
            }

            // 获取用户图片
            string imagePath = UserValidator.GetUserImagePath(this.ActionContext.Engine, delegant == null ? user : delegant, TempImagesPath);
            string avatar = "";
            if (user.Gender == Organization.UserGender.Male)
            {
                avatar = AppUtility.PortalRoot + "/img/TempImages/usermale.jpg";
            }
            else if (user.Gender == Organization.UserGender.Female)
            {
                avatar = AppUtility.PortalRoot + "/img/TempImages/userfemale.jpg";
            }
            else
            {
                avatar = AppUtility.PortalRoot + "/img/user.jpg";
            }

            if (!string.IsNullOrEmpty(imagePath))
            {
                avatar = this.GetImageUrlPath(imagePath) + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
            }
            string _itemComment = string.Empty;
            string _itemActionName = string.Empty;
            string _participant = string.Empty;
            if (!consultState)
            {
                if ((this.ActionContext.WorkItem.Consulted
                    && string.Equals(this.ActionContext.WorkItem.ObjectID, this.ActionContext.WorkItem.SourceWorkItemID))
                    || string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.SourceWorkItemID))
                {
                    DataTable itemComment = this.ActionContext.Engine.Query.GetWorkItemItemCommentData(ObjectID);
                    foreach (DataRow item in itemComment.Rows)
                    {
                        if (!string.Equals(_itemActionName, item["ItemActionName"].ToString().Trim())
                             && !string.Equals(_itemComment, item["ItemComment"].ToString().Trim()))
                        {
                            consltNumber = 0;
                            historyComments.Add(new
                            {
                                CommentID = ObjectID,//this.ActionContext.BizObjectID,
                                Avatar = Page.ResolveUrl(avatar),                           // 用户图片
                                OUName = user.Name,                                       // 意见审批人所属OU
                                UserName = workitem.ParticipantName,                    // 意见审批人
                                //DelegantName = ,                                         // 代理人
                                Activity = "征询",
                                DateStr = Convert.ToDateTime(item["ReceiveTime"]).ToShortDateString() + " " + Convert.ToDateTime(item["ReceiveTime"]).ToShortTimeString(),
                                Text = item["ItemComment"],
                                IsMyComment = false,
                                ParentPropertyName = "征询",
                                ItemActionName = item["ItemActionName"],
                                SignatureId = "",
                                Approval = 1,
                                itemAction = "",
                                OriGinatorName = "",
                                consltNumber = consltNumber
                            });
                        }

                        _itemComment = item["ItemComment"].ToString().Trim();
                        _itemActionName = item["ItemActionName"].ToString().Trim();
                        _participant = item["Participant"].ToString().Trim();
                        consltNumber++;
                        LoadFeedBackCommentData(item["WorkflowCode"].ToString().Trim(), ObjectID, item["InstanceId"].ToString().Trim()
                            , ObjectID, _participant, datafield, true, historyComments);

                    }
                }
                else
                {
                    //审批意见
                    Comment[] comments;
                    Unit[] units;
                    //常用意见
                    List<string> frequentlyUsedComments;
                    //一次从Engine获取方法需要的数据
                    this.ActionContext.Engine.Interactor.LoadCommentData(
                        this.ActionContext.SchemaCode,
                        this.ActionContext.BizObjectID,
                        this.ActionContext.InstanceId,
                        this.ActionContext.WorkItem == null ? null : this.ActionContext.WorkItem.WorkItemID,
                        datafield,
                        datafield == SheetConsultComment ? this.ActionContext.User.UserID : null,
                        null,
                        this.ActionContext.User.UserID,
                        out comments,
                        out units,
                        out frequentlyUsedComments);

                    if (comments != null || !string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.ItemComment))
                    {
                        string itemComment = this.ActionContext.WorkItem.ItemComment;
                        string ItemActionName = this.ActionContext.WorkItem.ItemActionName;
                        historyComments.Add(new
                        {
                            CommentID = ObjectID,//this.ActionContext.BizObjectID,
                            Avatar = Page.ResolveUrl(avatar),                           // 用户图片
                            OUName = user.Name,                                       // 意见审批人所属OU
                            UserName = workitem.ParticipantName,                    // 意见审批人
                            //DelegantName = ,                                         // 代理人
                            Activity = "征询",
                            DateStr = workitem.ReceiveTime.ToShortDateString() + " " + workitem.ReceiveTime.ToShortTimeString(),
                            Text = itemComment,
                            IsMyComment = false,
                            ParentPropertyName = "征询",
                            ItemActionName = ItemActionName,
                            SignatureId = "",
                            Approval = 1,
                            itemAction = "",
                            OriGinatorName = "",
                            consltNumber = consltNumber
                        });
                        consltNumber++;
                        if (this.ActionContext.WorkItem.ConsultantFinished && !this.ActionContext.WorkItem.Consulted)
                        {
                            LoadFeedBackCommentData(this.ActionContext.SchemaCode, this.ActionContext.BizObjectID, this.ActionContext.InstanceId
                                    , this.ActionContext.WorkItem == null ? null : this.ActionContext.WorkItem.WorkItemID,
                                    datafield == SheetConsultComment ? this.ActionContext.User.UserID : null, datafield, false, historyComments);
                        }
                    }
                }
            }
            else if (consultState)
            {
                if ((this.ActionContext.WorkItem.Consulted
                    && string.Equals(dt.Rows[0]["SourceWorkItemID"].ToString().Trim(), this.ActionContext.WorkItem.SourceWorkItemID))
                    || string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.SourceWorkItemID))
                {
                    DataTable itemComment = this.ActionContext.Engine.Query.GetWorkItemItemCommentData(ObjectID);
                    foreach (DataRow item in itemComment.Rows)
                    {

                        if (!string.Equals(_itemActionName, item["ItemActionName"].ToString().Trim())
                             && !string.Equals(_itemComment, item["ItemComment"].ToString().Trim()))
                        {
                            historyComments.Add(new
                            {
                                CommentID = ObjectID,//this.ActionContext.BizObjectID,
                                Avatar = Page.ResolveUrl(avatar),                           // 用户图片
                                OUName = user.Name,                                       // 意见审批人所属OU
                                UserName = workitem.ParticipantName,                    // 意见审批人
                                //DelegantName = ,                                         // 代理人
                                Activity = "征询",
                                DateStr = Convert.ToDateTime(item["ReceiveTime"]).ToShortDateString() + " " + Convert.ToDateTime(item["ReceiveTime"]).ToShortTimeString(),
                                Text = item["ItemComment"],
                                IsMyComment = false,
                                ParentPropertyName = "征询",
                                ItemActionName = item["ItemActionName"],
                                SignatureId = "",
                                Approval = 1,
                                itemAction = "",
                                OriGinatorName = "",
                                consltNumber = consltNumber
                            });
                        }

                        _itemComment = item["ItemComment"].ToString().Trim();
                        _itemActionName = item["ItemActionName"].ToString().Trim();
                        _participant = item["Participant"].ToString().Trim();
                        consltNumber++;
                    }
                    LoadFeedBackCommentData(this.ActionContext.SchemaCode, this.ActionContext.BizObjectID, this.ActionContext.InstanceId
                            , this.ActionContext.WorkItem == null ? null : this.ActionContext.WorkItem.WorkItemID,
                            datafield == SheetConsultComment ? this.ActionContext.User.UserID : null, datafield, false, historyComments);
                }
            }

            return historyComments;
        }

        /// <summary>
        /// 加载审批控件数据
        /// </summary>
        /// <param name="datafield"></param>
        /// <returns></returns>
        private object LoadCommentData(string datafield)
        {
            //审批意见
            Comment[] comments;
            Unit[] units;
            //常用意见
            List<string> frequentlyUsedComments;
            //一次从Engine获取方法需要的数据
            this.ActionContext.Engine.Interactor.LoadCommentData(
                this.ActionContext.SchemaCode,
                this.ActionContext.BizObjectID,
                this.ActionContext.InstanceId,
                this.ActionContext.WorkItem == null ? null : this.ActionContext.WorkItem.WorkItemID,
                datafield,
                datafield == SheetConsultComment ? this.ActionContext.User.UserID : null,
                null,
                this.ActionContext.User.UserID,
                out comments,
                out units,
                out frequentlyUsedComments);

            List<object> historyComments = null;
            if (comments != null)
            {
                historyComments = new List<object>();
                foreach (Comment comment in comments)
                {
                    OThinker.Organization.User user = null;
                    OThinker.Organization.User delegant = null;
                    foreach (OThinker.Organization.Unit unit in units)
                    {
                        if (unit.ObjectID == comment.UserID)
                        {
                            user = unit as OThinker.Organization.User;
                        }
                        if (unit.ObjectID == comment.Delegant)
                        {
                            delegant = unit as OThinker.Organization.User;
                        }
                    }

                    // 获取用户图片
                    string imagePath = UserValidator.GetUserImagePath(this.ActionContext.Engine, delegant == null ? user : delegant, TempImagesPath);
                    string avatar = "";
                    if (user.Gender == Organization.UserGender.Male)
                    {
                        avatar = AppUtility.PortalRoot + "/img/TempImages/usermale.jpg";
                    }
                    else if (user.Gender == Organization.UserGender.Female)
                    {
                        avatar = AppUtility.PortalRoot + "/img/TempImages/userfemale.jpg";
                    }
                    else
                    {
                        avatar = AppUtility.PortalRoot + "/img/user.jpg";
                    }

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        avatar = this.GetImageUrlPath(imagePath) + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    }

                    WorkflowTemplate.Activity Activity = this.ActionContext.Workflow.GetActivityByCode(comment.Activity);
                    bool isMyComment = false;
                    if (this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.TokenId == comment.TokenId)
                    {// A委托给B，那么A填的意见B能改，B填的意见A也能改
                        isMyComment = comment.UserID == this.ActionContext.WorkItem.Participant;
                    }
                    string itemAction = string.Empty;
                    string itemAction2 = string.Empty;
                    string OriGinatorName = string.Empty;
                    InstanceDetailController instanceDetail = new InstanceDetailController();
                    OThinker.H3.WorkItem.WorkItem workitem = this.ActionContext.Engine.WorkItemManager.GetWorkItem(comment.WorkItemId);
                    if (workitem != null)
                    {
                        // 类型
                        WorkItem.WorkItemType itemType = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(workitem.ItemType);
                        itemAction = instanceDetail.GetItemAction(itemType);
                        WorkItem.WorkItemType consulted = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(workitem.Consulted);
                        itemAction2 = instanceDetail.GetItemAction(consulted);
                        if (!string.IsNullOrWhiteSpace(comment.WorkItemId) && string.Equals(itemAction, Configs.Global.ResourceManager.GetString("SheetActionPane_Assist")))
                        {
                            DataTable dt = this.ActionContext.Engine.Query.GetWorkItemCreator(comment.WorkItemId);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                Unit unit = this.ActionContext.Engine.Organization.GetUnit(dt.Rows[0]["Creator"].ToString().Trim());
                                if (unit != null) OriGinatorName = unit.Name + "的" + itemAction;
                            }
                        }
                        if (string.Equals(itemAction2, Configs.Global.ResourceManager.GetString("WorkItemType_Consult")))
                        {
                            Unit _unit = this.ActionContext.Engine.Organization.GetUnit(comment.ConsultInitiator);
                            if (_unit != null) OriGinatorName = _unit.Name + "的" + Configs.Global.ResourceManager.GetString("WorkItemType_Consult");
                        }
                    }
                    historyComments.Add(new
                    {
                        CommentID = comment.CommentID,
                        Avatar = Page.ResolveUrl(avatar),                                                        // 用户图片
                        OUName = comment.OUName,                                                                 // 意见审批人所属OU
                        UserName = comment.UserName,                                                             // 意见审批人
                        DelegantName = comment.Delegant == comment.UserID ? string.Empty : comment.DelegantName, // 代理人
                        Activity = Activity == null ? comment.Activity : Activity.DisplayName,
                        DateStr = comment.ModifiedTime.ToShortDateString() + " " + comment.ModifiedTime.ToShortTimeString(),
                        Text = comment.Text,
                        IsMyComment = isMyComment,
                        SignatureId = comment.SignatureId,
                        Approval = comment.Approval,
                        ParentPropertyName = comment.ParentPropertyText,
                        itemAction = itemAction,
                        OriGinatorName = OriGinatorName.Trim()
                    });

                    CheckSignatureImage(comment.SignatureId);
                }
            }

            return new { Comments = historyComments, FrequentlyUsedComments = frequentlyUsedComments };
        }
        /// <summary>
        /// 加载征询审批控件数据
        /// </summary>
        /// <param name="datafield"></param>
        /// <returns></returns>
        private object LoadCommentDataNew(string datafield)
        {
            //审批意见
            Comment[] comments;
            //常用意见
            List<string> frequentlyUsedComments = null;
            List<object> historyComments = null;
            // 征询时为true,不是发起环节
            if (this.ActionContext.WorkItem != null && (!string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.SourceWorkItemID) || this.ActionContext.WorkItem.Consulted))//!string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.ItemComment) ||
            {
                historyComments = new List<object>();

                // 上
                if (!string.IsNullOrEmpty(this.ActionContext.WorkItem.SourceWorkItemID) && (this.ActionContext.WorkItem.ConsultantFinished || this.ActionContext.WorkItem.Consulted))
                    ConsultComments(this.ActionContext.WorkItem.SourceWorkItemID, WorkItem.WorkItem.PropertyName_SourceWorkItemID, datafield, historyComments, null, false);
                else
                    ConsultComments(this.ActionContext.WorkItem.ObjectID, WorkItem.WorkItem.PropertyName_ObjectID, datafield, historyComments, null, false);
                // 下   
                if (this.ActionContext.Engine.Query.GetWorkItemItemCommentData(this.ActionContext.WorkItem.SourceWorkItemID).Rows.Count > 0
                     && this.ActionContext.WorkItem.Consulted && !string.IsNullOrWhiteSpace(this.ActionContext.WorkItem.SourceWorkItemID))
                {
                    ConsultComments(this.ActionContext.WorkItem.ObjectID, WorkItem.WorkItem.PropertyName_ObjectID, datafield, historyComments, this.ActionContext.Engine.Query.GetWorkItemItemCommentData(this.ActionContext.WorkItem.SourceWorkItemID), true);

                }
                frequentlyUsedComments = this.ActionContext.Engine.Organization.GetFrequentlyUsedCommentTextsByUser(this.ActionContext.User.UserID);
            }

            return new { Comments = historyComments, FrequentlyUsedComments = frequentlyUsedComments };
        }

        /// <summary>
        /// 加载反馈的审批意见数据
        /// </summary>
        /// <param name="SchemaCode">数据模型编码</param>
        /// <param name="BizObjectID">数据模型ID</param>
        /// <param name="InstanceId">流程实例ID</param>
        /// <param name="WorkItem">数据项名称</param>
        /// <param name="datafield">意见填写人ID</param> 
        /// <param name="UserID">用户ID</param> 
        private void LoadFeedBackCommentData(string SchemaCode,
            string BizObjectID, string InstanceId, string WorkItemID,
            string UserID, string datafield, bool ConsultType,
            List<object> historyComments)
        {
            //审批意见
            Comment[] comments;
            Unit[] units;
            //常用意见
            List<string> frequentlyUsedConsultComments;
            //一次从Engine获取方法需要的数据 
            if (ConsultType)
            {
                this.ActionContext.Engine.Interactor.LoadConsultCommentData(
                   SchemaCode,
                   BizObjectID,
                   InstanceId,
                   WorkItemID,
                   datafield,
                   UserID,
                   null,
                   UserID,
                   out comments,
                   out units,
                   out frequentlyUsedConsultComments);
            }
            else
            {
                this.ActionContext.Engine.Interactor.LoadCommentData(
                   SchemaCode,
                   BizObjectID,
                   InstanceId,
                   WorkItemID,
                   datafield,
                   UserID,
                   null,
                   UserID,
                   out comments,
                   out units,
                   out frequentlyUsedConsultComments);
            }
            if (comments != null)
            {
                if (historyComments == null) historyComments = new List<object>();
                foreach (Comment comment in comments)
                {
                    OThinker.Organization.User user = null;
                    OThinker.Organization.User delegant = null;
                    foreach (OThinker.Organization.Unit unit in units)
                    {
                        if (unit.ObjectID == comment.UserID)
                        {
                            user = unit as OThinker.Organization.User;
                        }
                        if (unit.ObjectID == comment.Delegant)
                        {
                            delegant = unit as OThinker.Organization.User;
                        }
                    }

                    // 获取用户图片
                    string imagePath = UserValidator.GetUserImagePath(this.ActionContext.Engine, delegant == null ? user : delegant, TempImagesPath);
                    string avatar = "";
                    if (user.Gender == Organization.UserGender.Male)
                    {
                        avatar = AppUtility.PortalRoot + "/img/TempImages/usermale.jpg";
                    }
                    else if (user.Gender == Organization.UserGender.Female)
                    {
                        avatar = AppUtility.PortalRoot + "/img/TempImages/userfemale.jpg";
                    }
                    else
                    {
                        avatar = AppUtility.PortalRoot + "/img/user.jpg";
                    }

                    if (!string.IsNullOrEmpty(imagePath))
                    {
                        avatar = this.GetImageUrlPath(imagePath) + "?" + DateTime.Now.ToString("yyyyMMddHHmmss");
                    }

                    WorkflowTemplate.Activity Activity = this.ActionContext.Workflow.GetActivityByCode(comment.Activity);
                    bool isMyComment = false;
                    if (this.ActionContext.WorkItem != null && this.ActionContext.WorkItem.TokenId == comment.TokenId)
                    {// A委托给B，那么A填的意见B能改，B填的意见A也能改
                        isMyComment = comment.UserID == this.ActionContext.WorkItem.Participant;
                    }
                    string itemAction = string.Empty;
                    string itemAction2 = string.Empty;
                    string OriGinatorName = string.Empty;
                    InstanceDetailController instanceDetail = new InstanceDetailController();
                    OThinker.H3.WorkItem.WorkItem workitem = this.ActionContext.Engine.WorkItemManager.GetWorkItem(comment.WorkItemId);
                    if (workitem != null)
                    {
                        // 类型
                        WorkItem.WorkItemType itemType = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(workitem.ItemType);
                        itemAction = instanceDetail.GetItemAction(itemType);
                        WorkItem.WorkItemType consulted = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(workitem.Consulted);
                        itemAction2 = instanceDetail.GetItemAction(consulted);
                        if (!string.IsNullOrWhiteSpace(comment.WorkItemId) && string.Equals(itemAction, Configs.Global.ResourceManager.GetString("SheetActionPane_Assist")))
                        {
                            DataTable dt = this.ActionContext.Engine.Query.GetWorkItemCreator(comment.WorkItemId);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                Unit unit = this.ActionContext.Engine.Organization.GetUnit(dt.Rows[0]["Creator"].ToString().Trim());
                                if (unit != null) OriGinatorName = unit.Name + "的" + itemAction;
                            }
                        }
                        if (string.Equals(itemAction2, Configs.Global.ResourceManager.GetString("WorkItemType_Consult")))
                        {
                            Unit _unit = this.ActionContext.Engine.Organization.GetUnit(comment.ConsultInitiator);
                            if (_unit != null) OriGinatorName = _unit.Name + "的" + Configs.Global.ResourceManager.GetString("WorkItemType_Consult");
                        }
                    }
                    historyComments.Add(new
                    {
                        CommentID = comment.CommentID,
                        Avatar = Page.ResolveUrl(avatar),                                                        // 用户图片
                        OUName = comment.OUName,                                                                 // 意见审批人所属OU
                        UserName = comment.UserName,                                                             // 意见审批人
                        DelegantName = comment.Delegant == comment.UserID ? string.Empty : comment.DelegantName, // 代理人
                        Activity = Activity == null ? comment.Activity : Activity.DisplayName,
                        DateStr = comment.ModifiedTime.ToShortDateString() + " " + comment.ModifiedTime.ToShortTimeString(),
                        Text = comment.Text,
                        IsMyComment = isMyComment,
                        ParentPropertyName = "征询",
                        SignatureId = comment.SignatureId,
                        Approval = comment.Approval,
                        itemAction = itemAction,
                        OriGinatorName = OriGinatorName.Trim(),
                        consltNumber = consltNumber
                    });
                    CheckSignatureImage(comment.SignatureId);
                }
            }
        }

        #endregion

        #region 加载附件数据项
        private List<MvcListItem> LoadAttachmentData(FieldSchema field)
        {
            List<MvcListItem> listItems = new List<MvcListItem>();
            AttachmentHeader[] headers = this.ActionContext.Engine.BizObjectManager.QueryAttachment(
                 this.ActionContext.SchemaCode,
                 this.ActionContext.BizObjectID,
                 field.Name,
                 OThinker.Data.BoolMatchValue.True,
                 null);
            if (headers != null)
            {
                foreach (AttachmentHeader header in headers)
                {
                    string url = string.IsNullOrEmpty(header.DownloadUrl) ? AppConfig.GetReadAttachmentUrl(
                        this.ActionContext.IsMobile,
                        this.ActionContext.Schema.SchemaCode,
                        this.ActionContext.BizObjectID,
                        header.ObjectID,
                        AttachmentOpenMethod.Download) : header.DownloadUrl;

                    listItems.Add(new MvcListItem(header.ObjectID, header.FileName, url, header.ContentLength, header.ContentType));
                }
            }
            return listItems;
        }
        #endregion

        #region 加载多人参与者数据
        private List<MvcListItem> LoadMultiParticipantData(FieldSchema field)
        {
            List<MvcListItem> listItems = null;
            string[] ids = this.ActionContext.InstanceData[field.Name].Value as string[];
            if (ids != null)
            {
                listItems = new List<MvcListItem>();
                List<Organization.Unit> units = this.ActionContext.Engine.Organization.GetUnits(ids);
                if (units != null)
                {
                    foreach (string user in ids)
                    {
                        string name = string.Empty;
                        string type = string.Empty;
                        foreach (Organization.Unit u in units)
                        {
                            if (u.ObjectID == user)
                            {
                                name = u.Name;
                                if (u.UnitType == Organization.UnitType.OrganizationUnit)
                                {
                                    type = "O";
                                }
                                else if (u.UnitType == Organization.UnitType.Group)
                                {
                                    type = "G";
                                }
                                else if (u.UnitType == Organization.UnitType.User)
                                {
                                    type = "U";
                                }
                                break;
                            }
                        }
                        //listItems.Add(new MvcListItem(user, name));
                        listItems.Add(new MvcListItem(user, name, null, 0, type));
                    }
                }
            }
            return listItems;
        }
        #endregion

        #region 加载关联表单数据
        private object LoadAssociationData(FieldSchema field, string schemaObjectId)
        {
            BizObjectSchema schema = null;
            Sheet.BizSheet sheet = null;
            this.ActionContext.Engine.Interactor.GetDataForBizObject(field.DefaultValue + string.Empty, "S" + field.DefaultValue, out schema, out sheet);
            BizObject bizObject = new BizObject(
             this.ActionContext.Engine,
             schema,
             this.ActionContext.User.UserID);
            bizObject.ObjectID = schemaObjectId + string.Empty;

            //this.ActionContext.Engine.Interactor.
            if (string.IsNullOrEmpty(bizObject.ObjectID))
            {
                object obj = new string[] { schemaObjectId, "" };
                return obj;
            }
            else
            {
                bizObject.Load();
                //BizObjectSchema quoteSchema = this.ActionContext.Engine.biz
                //BizObject bizObject = this.ActionContext.BizObject
                object obj = new string[] { schemaObjectId, bizObject.Name, bizObject.RunningInstanceId };
                return obj;
            }
        }
        #endregion
        #region 从控件属性构造MVC数据项
        /// <summary>
        /// 从控件属性构造MVC数据项
        /// </summary>
        /// <param name="field">业务对象数组(父对象)属性</param>
        /// <param name="Property">字段属性</param>
        /// <param name="bo">业务对象实例</param>
        /// <returns></returns>
        private MvcDataItem GetMvcDataFromProperty(FieldSchema field, PropertySchema Property, BizObject bo)
        {
            object Value = bo[Property.Name];
            MvcDataItem dataItem = new MvcDataItem()
            {
                N = Property.DisplayName,
                L = Property.LogicType,
                V = Value
            };
            ClientActivity activity = this.ActionContext.ActivityTemplate as ClientActivity;
            // TODO:业务表单的权限控制待设计
            if (this.ActionContext.IsWorkMode || this.ActionContext.IsOriginateMode)
            {
                if (this.ActionContext.WorkItem != null
                    && this.ActionContext.WorkItem.ItemType == WorkItemType.Circulate)
                {
                }
                else
                {
                    if (this.ActionContext.SheetDataType == SheetDataType.BizObject || activity.GetSubColumnEditable(field.Name,
                        Property.Name))
                    {
                        dataItem.O += "E";
                    }
                    if (activity != null && activity.GetSubColumnRequired(field.Name, Property.Name))
                    {// 工作模式子表数据项必填
                        dataItem.O += "R";
                    }
                }
            }
            if (this.ActionContext.IsMobile && activity != null && activity.GetSubColumnMobileVisible(field.Name, Property.Name))
            {// 移动办公模式并且移动办公可见
                dataItem.O += "M";
            }
            if (activity != null && activity.GetSubColumnTrackVisible(field.Name, Property.Name))
            {// 当前节点可以查看痕迹
                dataItem.O += "T";
            }
            if (this.ActionContext.SheetDataType == SheetDataType.BizObject || activity == null || activity.GetSubColumnVisible(field.Name, Property.Name))
            {// 业务表单模式或者当前节点可见
                dataItem.O += "V";
            }

            if (Property.LogicType == DataLogicType.SingleParticipant)
            {
                if (!string.IsNullOrEmpty(Value + string.Empty))
                {
                    dataItem.V = new MvcListItem(Value + string.Empty, this.ActionContext.Engine.Organization.GetName(Value + string.Empty));
                }
            }
            else if (Property.LogicType == DataLogicType.MultiParticipant)
            {
                List<MvcListItem> listItems = null;
                string[] ids = Value as string[];
                if (ids != null)
                {
                    listItems = new List<MvcListItem>();
                    List<Organization.Unit> units = this.ActionContext.Engine.Organization.GetUnits(ids);
                    if (units != null)
                    {
                        foreach (string user in ids)
                        {
                            string name = string.Empty;
                            foreach (Organization.Unit u in units)
                            {
                                if (u.ObjectID == user)
                                {
                                    name = u.Name;
                                    break;
                                }
                            }
                            listItems.Add(new MvcListItem(user, name));
                        }
                    }
                }
                dataItem.V = listItems;
            }
            else if (Property.LogicType == DataLogicType.Attachment)
            {
                List<MvcListItem> listItems = new List<MvcListItem>();
                AttachmentHeader[] headers = this.ActionContext.Engine.BizObjectManager.QueryAttachment(
                     this.ActionContext.Schema.SchemaCode,
                     bo.ObjectID,
                     field.Name + "." + Property.Name,
                     OThinker.Data.BoolMatchValue.True,
                     null);
                if (headers != null)
                {
                    foreach (AttachmentHeader header in headers)
                    {
                        string url = AppConfig.GetReadAttachmentUrl(
                            this.ActionContext.IsMobile,
                            this.ActionContext.Schema.SchemaCode,
                            bo.ObjectID,
                            header.ObjectID,
                            AttachmentOpenMethod.Download);
                        listItems.Add(new MvcListItem(header.ObjectID, header.FileName, url, header.ContentLength, header.ContentType));
                    }
                }
                dataItem.V = listItems;
            }

            return dataItem;
        }
        #endregion
        #endregion

        #region 保存数据项数据相关接口 -----------

        #region 保存审批意见 -------
        /// <summary>
        /// 保存审批意见
        /// </summary>
        /// <param name="BizObject"></param>
        /// <param name="DataField"></param>
        /// <param name="MvcResult"></param>
        private void SaveCommentData(MvcBizObject BizObject, string DataField, MvcResult MvcResult, MvcPostValue MvcPost)
        {
            Dictionary<string, string> result = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(BizObject.DataItems[DataField].V.ToString());
            string CommentID = result["CommentID"];
            string CommandText = result["Text"];
            bool IsNewComment = Convert.ToBoolean(result["IsNewComment"]);
            bool SetFrequent = Convert.ToBoolean(result["SetFrequent"]);
            string signatureId = result["SignatureId"];

            if (this.ActionContext.ActivityTemplate == null) return;

            if (this.ActionContext.ActivityTemplate.ActivityType == ActivityType.Approve)
            {  // 设置审核意见
                string commentDataItem = ((ApproveActivity)this.ActionContext.ActivityTemplate).CommentDataItem;
                if (!string.IsNullOrEmpty(commentDataItem))
                {
                    this.ActionContext.InstanceData[commentDataItem].Value = CommandText;
                }
            }

            #region 设置为常用意见
            if (SetFrequent)
            {
                Organization.FrequentlyUsedComment frequentComment = new Organization.FrequentlyUsedComment()
                {
                    CommentText = CommandText.Trim(),
                    SortKey = 100,
                    UserID = this.ActionContext.User.UserID
                };
                this.ActionContext.Engine.Organization.AddFrequentlyUsedComment(frequentComment);
            }
            #endregion


            string _parentPropertyText = this.ActionContext.Engine.Query.GetFrequentlyCommentParentPropertyText(CommentID);
            if (!string.IsNullOrWhiteSpace(_parentPropertyText)) CommentID = Guid.NewGuid().ToString();
            if (IsNewComment || !string.IsNullOrWhiteSpace(_parentPropertyText))
            {
                #region 新增审批意见 --------------
                OThinker.H3.Data.Comment comment = new OThinker.H3.Data.Comment();
                //写回数据
                if (!string.IsNullOrWhiteSpace(CommentID))
                {
                    comment.ObjectID = CommentID;
                }
                comment.Activity = this.ActionContext.ActivityCode;

                //if (MvcResult.IsSaveAction)
                //    comment.Approval = OThinker.Data.BoolMatchValue.Unspecified;
                //else
                //    comment.Approval = this.Approval;

                if (MvcResult.IsSaveAction)
                    comment.Approval = OThinker.Data.BoolMatchValue.Unspecified;
                else if (MvcResult.IsRejectAction)
                    comment.Approval = OThinker.Data.BoolMatchValue.False;
                else
                    comment.Approval = this.Approval;



                comment.CreatedTime = System.DateTime.Now;
                #region 在审批意见释放时，系统报错
                bool ifSheetAssistComment = false;
                if (this.ActionContext.WorkItem != null)
                {
                    if (this.ActionContext.WorkItem.ItemType != null)
                    {
                        ifSheetAssistComment = (this.ActionContext.WorkItem.ItemType == WorkItemType.ActivityAssist || this.ActionContext.WorkItem.ItemType == WorkItemType.WorkItemAssist);
                    }
                }
                #endregion
                if (ifSheetAssistComment)
                {
                    comment.DataField = SheetAssistComment;
                }
                else
                {
                    comment.DataField = DataField;
                }
                comment.DataField = DataField;
                comment.InstanceId = this.ActionContext.InstanceId;
                comment.ModifiedTime = System.DateTime.Now;
                comment.Text = CommandText;

                #region 记录意见归属人 -----------
                comment.UserID = this.ActionContext.WorkItem == null ? this.ActionContext.User.UserID : this.ActionContext.WorkItem.Participant;

                if (comment.UserID == this.ActionContext.User.UserID)
                {
                    comment.UserName = this.ActionContext.User.UserName;
                    comment.OUName = this.ActionContext.User.DepartmentName;
                }
                else
                {
                    comment.UserName = this.ActionContext.Engine.Organization.GetName(comment.UserID);
                    Unit parentUnit = this.ActionContext.Engine.Organization.GetParentUnit(comment.UserID);
                    comment.OUName = parentUnit.Name;
                }

                if (comment.UserID != this.ActionContext.User.UserID)
                {// 当前任务处理人不是原任务归属人
                    comment.Delegant = this.ActionContext.User.UserID;
                    comment.DelegantName = this.ActionContext.User.UserName;
                }
                #endregion

                comment.BizObjectSchemaCode = this.ActionContext.Schema.SchemaCode;
                comment.InstanceId = this.ActionContext.InstanceId;
                // 协办完成获取当前WorkItem表单ObjectID，已做区分
                if (MvcPost != null && int.Equals(MvcPost.WorkItemType, (int)WorkItemType.WorkItemAssist))
                {
                    comment.WorkItemId = MvcPost.WorkItemId;
                }
                else if (this.ActionContext.WorkItem != null)
                {
                    comment.WorkItemId = this.ActionContext.WorkItem.SourceWorkItemID;
                }
                comment.BizObjectId = this.ActionContext.InstanceData.BizObject.ObjectID;

                if (this.ActionContext.WorkItem != null)
                {
                    comment.TokenId = this.ActionContext.WorkItem.TokenId;
                }
                else if (this.ActionContext.IsOriginateMode)
                {
                    comment.TokenId = OThinker.H3.Instance.Token.InitialTokenId;
                }
                // 征询确认后状态应改变
                if (string.Equals(DataField, SheetConsultComment))
                {
                    comment.ItemType = (int)WorkItemType.WorkItemConsult;
                    //comment.ModifiedBy = this.ActionContext.User.UserID;
                    WorkItem.WorkItem _workitem = this.ActionContext.Engine.WorkItemManager.GetWorkItem(this.ActionContext.WorkItem.SourceWorkItemID);
                    if (_workitem != null) comment.ConsultInitiator = _workitem.Participant;
                }
                comment.SignatureId = signatureId;
                this.ActionContext.Engine.BizObjectManager.AddComment(comment);

                #endregion

                string message = string.Format("新增审批意见,InstanceID={0},UserID={1},Activity={2}",
                    this.ActionContext.InstanceId, this.ActionContext.User.UserID, this.ActionContext.ActivityTemplate.DisplayName);
                this.ActionContext.Engine.LogWriter.Write(message);
            }
            else
            {
                OThinker.Data.BoolMatchValue approval = this.Approval;
                if (MvcResult.IsSaveAction)
                    approval = OThinker.Data.BoolMatchValue.Unspecified;
                //更新审批意见
                this.ActionContext.Engine.BizObjectManager.UpdateComment(
                    this.ActionContext.Schema.SchemaCode,
                    this.ActionContext.InstanceId,
                    DataField,
                    CommentID,
                    this.ActionContext.User.UserID,
                    CommandText,
                    approval,
                    signatureId);

                string message = string.Format("更新审批意见,InstanceID={0},UserID={1},CommentID={2}",
                    this.ActionContext.InstanceId, this.ActionContext.User.UserID, CommentID);
                this.ActionContext.Engine.LogWriter.Write(message);
            }
        }

        /// <summary>
        /// 获取节点的审批意见
        /// </summary>
        /// <param name="defaultDataField"></param>
        /// <returns></returns>
        private string GetActivityCommentDataField(string defaultDataField)
        {
            if (defaultDataField == SheetAssistComment || defaultDataField == SheetForwardComment)
            {
                string datafield = this.ActionContext.Workflow.GetDefaultCommentDataItem(this.ActionContext.Schema, this.ActionContext.ActivityCode);
                if (string.IsNullOrEmpty(datafield)) return defaultDataField;
                return datafield;
            }
            return defaultDataField;
        }
        #endregion

        #region 保存附件数据
        /// <summary>
        /// 保存附件数据
        /// </summary>
        /// <param name="BizObject"></param>
        /// <param name="field"></param>
        private void SaveAttachmentData(MvcBizObject BizObject, FieldSchema field)
        {
            Dictionary<string, string> result = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(BizObject.DataItems[field.Name].V.ToString());
            string Attachments = result["AttachmentIds"];
            string DelAttachmentIds = result["DelAttachmentIds"];
            //添加的附件
            if (!string.IsNullOrWhiteSpace(Attachments))
            {
                this.ActionContext.Engine.BizObjectManager.AttachBizObject(
                 this.ActionContext.Participant.UserID,
                 Attachments.Split(';'),
                 this.ActionContext.Schema.SchemaCode,
                 this.ActionContext.BizObjectID,
                 field.Name);
                this.AddUserLog(Tracking.UserLogType.AddAttachment);
            }
            //删除附件
            if (!string.IsNullOrWhiteSpace(DelAttachmentIds))
            {
                string[] RemovingAttachments = DelAttachmentIds.Split(';');
                foreach (string id in RemovingAttachments)
                {
                    if (id != null && id != "")
                    {
                        this.ActionContext.Engine.BizObjectManager.RemoveAttachment(
                            this.ActionContext.Participant.UserID,
                            this.ActionContext.Schema.SchemaCode,
                            this.ActionContext.BizObjectID,
                            id);
                    }
                }
                this.AddUserLog(Tracking.UserLogType.RemoveAttachment);
            }
        }
        #endregion

        #region 保存业务对象
        /// <summary>
        /// 保存业务对象
        /// </summary>
        /// <param name="BizObject"></param>
        /// <param name="field"></param>
        private void SaveBizObject(MvcBizObject BizObject, FieldSchema field)
        {
            BizObject bo = this.ActionContext.InstanceData.BizObject[field.Name] as BizObject;
            bool isNew = false;
            if (bo == null)
            {
                if (field.Schema != null)
                {
                    bo = new BizObject(this.ActionContext.Engine, field.Schema, this.ActionContext.User.UserID);
                }
                else
                {
                    BizObjectSchema childSchema = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                    bo = new BizObject(this.ActionContext.Engine, childSchema, this.ActionContext.User.UserID);
                }
                isNew = true;
            }
            string datafield = string.Empty;

            if (BizObject.DataItems.ContainsKey(field.Name))
            {
                List<Dictionary<string, object>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(BizObject.DataItems[field.Name].V.ToString());
                // 赋值给 BizObject
                foreach (FieldSchema f in bo.Schema.Fields)
                {
                    datafield = field.Name + "." + f.Name;
                    if (result.Count == 0 || !result[0].ContainsKey(f.Name)) continue;
                    if (f.LogicType == DataLogicType.MultiParticipant)
                    {
                        bo[f.Name] = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(result[0][f.Name] + string.Empty);
                    }
                    else if (f.LogicType == DataLogicType.Attachment)
                    {
                        Dictionary<string, string> attachments = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(result[0][f.Name] + string.Empty);
                        string Attachments = attachments["AttachmentIds"];
                        string DelAttachmentIds = attachments["DelAttachmentIds"];
                        string bizobjectid = bo.ObjectID;

                        //添加的附件
                        if (!string.IsNullOrWhiteSpace(Attachments))
                        {
                            this.ActionContext.Engine.BizObjectManager.AttachBizObject(
                             this.ActionContext.Participant.UserID,
                             Attachments.Split(';'),
                             this.ActionContext.Schema.SchemaCode,
                             bizobjectid,
                             datafield);
                        }
                        //删除附件
                        if (!string.IsNullOrWhiteSpace(DelAttachmentIds))
                        {
                            string[] RemovingAttachments = DelAttachmentIds.Split(';');
                            foreach (string id in RemovingAttachments)
                            {
                                if (id != null && id != "")
                                {
                                    this.ActionContext.Engine.BizObjectManager.RemoveAttachment(
                                        this.ActionContext.Participant.UserID,
                                        this.ActionContext.Schema.SchemaCode,
                                        bizobjectid,
                                        id);
                                }
                            }
                        }
                    }
                    else
                    {
                        bo[f.Name] = result[0][f.Name];
                    }
                }
            }
            if (IsAssociationSchema(field))
            {
                if (isNew) bo.Create();
                else bo.Update();
            }
            else
            {
                this.ActionContext.InstanceData.BizObject[field.Name] = bo;
            }
        }
        #endregion

        #region 保存业务对象数组
        /// <summary>
        /// 保存业务对象数组
        /// </summary>
        /// <param name="BizObject"></param>
        /// <param name="field"></param>
        private void SaveBizObjectArray(MvcBizObject BizObject, FieldSchema field)
        {
            try
            {
                BizObjectAssociation asso = null;
                List<Dictionary<string, object>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(BizObject.DataItems[field.Name].V.ToString());
                if (result == null || result.Count == 0)
                {
                    //删除所有数据
                }
                //将BO从数组拷贝到List中
                List<BizObject> oldBoList = this.GetBOList(field);
                // 新的BO列表
                List<DataModel.BizObject> newBoList = new List<DataModel.BizObject>();

                int rowIndex = 0;
                bool needSort = this.ActionContext.Schema.GetProperty(SerializableObject.PropertyName_ParentIndex) != null;
                if (result == null)
                {
                    result = new List<Dictionary<string, object>>();
                }
                DataModel.BizObjectSchema childSchema = null;
                if (this.ActionContext.Schema.GetProperty((field.Name)) != null)
                {
                    childSchema = this.ActionContext.Schema.GetProperty(field.Name).ChildSchema;
                }
                else
                {
                    childSchema = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(field.ChildSchemaCode);
                }

                // 先执行删除不存在的数据，否则相同编码不能加入
                if (IsAssociationSchema(field))
                {
                    asso = this.ActionContext.Schema.GetAssociation(field.Name);
                    //删除不再存在的行
                    for (int index = 0; index < oldBoList.Count; index++)
                    {
                        bool isExists = false;
                        foreach (Dictionary<string, object> NameAndValues in result)
                        {
                            if (NameAndValues.ContainsKey(OThinker.H3.Data.Attachment.PropertyName_ObjectID) &&
                                NameAndValues[OThinker.H3.Data.Attachment.PropertyName_ObjectID] + string.Empty == oldBoList[index].ObjectID)
                            {
                                isExists = true; break;
                            }
                        }
                        if (!isExists)
                        {
                            // 删除
                            DataModel.BizObject bo = oldBoList[index];
                            bo.Remove();
                        }
                    }
                }

                foreach (Dictionary<string, object> NameAndValues in result)
                {
                    if (NameAndValues == null) continue;
                    rowIndex++;
                    bool Found = false;
                    if (NameAndValues != null && NameAndValues.ContainsKey(SerializableObject.C_ObjectID))
                    {
                        for (int oldIndex = oldBoList.Count - 1; oldIndex >= 0; oldIndex--)
                        {
                            //若找到则更新
                            if (NameAndValues[SerializableObject.C_ObjectID] + string.Empty == oldBoList[oldIndex].ObjectID)
                            {
                                Found = true;

                                this.UpdateBO(childSchema, NameAndValues, oldBoList[oldIndex], field);
                                //更新行号字段
                                if (needSort)
                                {
                                    oldBoList[oldIndex][SerializableObject.PropertyName_ParentIndex] = rowIndex;
                                }

                                if (IsAssociationSchema(field))
                                {
                                    oldBoList[oldIndex].Update();
                                }

                                //添加到新列表
                                newBoList.Add(oldBoList[oldIndex]);

                                //从旧列表中移除
                                oldBoList.RemoveAt(oldIndex);
                                break;
                            }
                        }
                    }

                    if (!Found)
                    {
                        if (childSchema != null)
                        {
                            // 新增
                            DataModel.BizObject bo = new DataModel.BizObject(
                                this.ActionContext.Engine,
                                childSchema,
                                this.ActionContext.User.UserID,
                                this.ActionContext.User.User.ParentID);
                            this.UpdateBO(childSchema, NameAndValues, bo, field);
                            //更新行号字段
                            if (needSort)
                            {
                                bo[SerializableObject.PropertyName_ParentIndex] = rowIndex;
                            }
                            newBoList.Add(bo);
                            if (IsAssociationSchema(field))
                            {
                                if (asso.Maps != null)
                                {
                                    foreach (DataMap map in asso.Maps.ToArray())
                                    {
                                        if (map.MapType == DataMapType.None) continue;
                                        if (this.ActionContext.Schema.GetProperty(map.MapTo) != null)
                                        {
                                            bo[map.ItemName] = this.ActionContext.InstanceData[map.MapTo].Value;
                                        }
                                        else
                                        {
                                            bo[map.ItemName] = map.MapTo;
                                        }
                                    }
                                }
                                bo.Create();
                            }
                        }
                    }
                    rowIndex++;
                }

                // 更新到字段上
                if (this.ActionContext.InstanceData[field.Name].LogicType == Data.DataLogicType.BizObject)
                {
                    this.ActionContext.InstanceData[field.Name].Value = newBoList.Count == 0 ? null : newBoList[0];
                }
                else if (this.ActionContext.InstanceData[field.Name].LogicType == Data.DataLogicType.BizObjectArray)
                {
                    this.ActionContext.InstanceData[field.Name].Value = newBoList.Count == 0 ? null : newBoList.ToArray();
                }
                else
                {
                    // 不支持的类型
                    throw new ArgumentOutOfRangeException("This control can not be used for data type \"" + this.ActionContext.InstanceData[field.Name].LogicType + "\".");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取指定的字段的视图状态
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        private OThinker.H3.DataModel.FieldState GetFieldState(FieldSchema field)
        {
            // 获得数据项的视图选项
            if (this.ActionContext.ActivityTemplate == null)
            {
                return null;
            }

            if (this.ClientActivity != null)
            {
                string viewName = this.ClientActivity.GetItemViewName(field.Name);
                if (!string.IsNullOrEmpty(viewName))
                {
                    OThinker.H3.DataModel.View view = this.ActionContext.Schema.Views[viewName];
                    return view.GetFieldState(field.Name);
                }
            }
            return null;
        }

        #region 当前活动

        H3.WorkflowTemplate.ClientActivity _ClientActivity;
        /// <summary>
        /// 当前活动
        /// </summary>
        H3.WorkflowTemplate.ClientActivity ClientActivity
        {
            get
            {
                if (this._ClientActivity == null)
                {
                    this._ClientActivity = (H3.WorkflowTemplate.ClientActivity)this.ActionContext.ActivityTemplate;
                }
                return this._ClientActivity;
            }
            set
            {
                this._ClientActivity = value;
            }
        }

        #endregion

        /// <summary>
        /// 获取指定的字段是否可见
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        private bool GetColumnVisible(string ColumnName, FieldSchema field)
        {
            if (this.ActionContext.SheetDataType == SheetDataType.BizObject
                && (this.ActionContext.SheetMode == SheetMode.Work || this.ActionContext.SheetMode == SheetMode.Originate))
            {
                return true;
            }

            OThinker.H3.DataModel.FieldState state = this.GetFieldState(field);
            if (state != null)
                return state.Visible;
            if (this.ClientActivity != null)
                return this.ClientActivity.GetSubColumnVisible(field.Name, ColumnName);
            return true;
        }

        /// <summary>
        /// 获取指定字段是否可编辑
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <returns></returns>
        private bool GetColumnEditable(string ColumnName, FieldSchema field)
        {
            if (this.ActionContext.SheetDataType == SheetDataType.BizObject
                && (this.ActionContext.SheetMode == SheetMode.Work || this.ActionContext.SheetMode == SheetMode.Originate))
            {
                return true;
            }

            OThinker.H3.DataModel.FieldState state = this.GetFieldState(field);
            if (state != null)
                return state.Editable;
            if (this.ClientActivity != null)
                return this.ClientActivity.GetSubColumnEditable(field.Name, ColumnName);
            return false;
        }

        /// <summary>
        /// 更新子BO对象
        /// </summary>
        /// <param name="Schema"></param>
        /// <param name="NameValues"></param>
        /// <param name="bo"></param>
        /// <param name="field"></param>
        public void UpdateBO(BizObjectSchema Schema, Dictionary<string, object> NameValues, DataModel.BizObject bo, FieldSchema field)
        {
            foreach (string ColumnName in NameValues.Keys)
            {
                if (DataModel.BizObjectSchema.IsReservedProperty(ColumnName))
                {
                    if (ColumnName.ToLower() == OThinker.H3.Data.Attachment.PropertyName_ObjectID.ToLower())
                    {
                        bo[ColumnName] = NameValues[ColumnName];
                    }
                    continue;
                }
                if (ColumnName.ToLower() == BizObjectSchema.PropertyName_ObjectID.ToLower()) continue;
                if (!string.IsNullOrEmpty(bo.Schema.GetField(ColumnName).Formula)) continue;
                if (!this.GetColumnVisible(ColumnName, field) || !this.GetColumnEditable(ColumnName, field))
                {
                    continue;
                }
                // 设置值
                // 附件
                PropertySchema property = Schema.GetProperty(ColumnName);
                if (property.LogicType == DataLogicType.Attachment)
                {
                    Dictionary<string, string> attachments = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(NameValues[ColumnName] + string.Empty);
                    string Attachments = attachments["AttachmentIds"];
                    string DelAttachmentIds = attachments["DelAttachmentIds"];
                    string bizobjectid = NameValues.ContainsKey(SerializableObject.C_ObjectID) ? NameValues["ObjectID"] + string.Empty : bo.ObjectID;

                    //添加的附件
                    if (!string.IsNullOrWhiteSpace(Attachments))
                    {
                        this.ActionContext.Engine.BizObjectManager.AttachBizObject(
                         this.ActionContext.Participant.UserID,
                         Attachments.Split(';'),
                         this.ActionContext.Schema.SchemaCode,
                         bizobjectid,
                         field.Name + "." + ColumnName);
                    }
                    //删除附件
                    if (!string.IsNullOrWhiteSpace(DelAttachmentIds))
                    {
                        string[] RemovingAttachments = DelAttachmentIds.Split(';');
                        foreach (string id in RemovingAttachments)
                        {
                            if (id != null && id != "")
                            {
                                this.ActionContext.Engine.BizObjectManager.RemoveAttachment(
                                    this.ActionContext.Participant.UserID,
                                    this.ActionContext.Schema.SchemaCode,
                                    bizobjectid,
                                    id);
                            }
                        }
                    }
                }
                else if (property.LogicType == DataLogicType.MultiParticipant)
                { //多人参与者
                    bo.SetValue(ColumnName, Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(NameValues[ColumnName] + string.Empty));
                }
                else
                    bo.SetValue(ColumnName, NameValues[ColumnName]);
            }
            // 更新关联字段的值
            if (this.IsAssociationSchema(field))
            {
                BizObjectAssociation asso = this.ActionContext.Schema.GetAssociation(field.Name);
                if (asso != null)
                {
                    DataMap[] maps = asso.Maps.ToArray();
                    if (maps != null)
                    {
                        foreach (DataMap map in maps)
                        {
                            if (map.MapType == DataMapType.None) continue;
                            if (map.MapType == DataMapType.Const)
                            {
                                bo.SetValue(
                                    map.ItemName,
                                    map.MapTo);
                            }
                            else
                            {
                                string v = this.ActionContext.InstanceData[map.MapTo].Value + string.Empty;
                                bo.SetValue(
                                   map.ItemName,
                                   v);
                            }
                        }
                    }
                }
            }
            // End
        }

        protected bool IsAssociationSchema(FieldSchema field)
        {
            return this.ActionContext.Schema.GetAssociation(field.Name) != null;
        }

        /// <summary>
        /// 获取已有BO列表
        /// </summary>
        /// <returns></returns>
        List<BizObject> GetBOList(FieldSchema field)
        {
            List<BizObject> oldBoList = new List<BizObject>();

            // 获得原来的值
            DataModel.BizObject[] oldBos = this.GetBOsValue(this.ActionContext.InstanceData.BizObject, field);
            if (oldBos != null && oldBos.Length > 0)
            {
                for (int i = 0; i < oldBos.Length; i++)
                {
                    oldBoList.Add(oldBos[i]);
                }
            }
            return oldBoList;
        }

        /// <summary>
        /// 获取业务对象数据
        /// </summary>
        /// <param name="SheetPage"></param>
        /// <returns></returns>
        private DataModel.BizObject[] GetBOsValue(BizObject BizObject, FieldSchema field)
        {
            if (HttpContext.Current.Session[BizObject.ObjectID + field.Name] != null)
                return HttpContext.Current.Session[BizObject.ObjectID + field.Name] as BizObject[];

            DataModel.BizObject[] objs = null;
            object v = BizObject[field.Name];
            if (v == null)
            {
                objs = null;
            }
            else if (v is DataModel.BizObject)
            {
                objs = new DataModel.BizObject[] { (DataModel.BizObject)v };
            }
            else if (v is DataModel.BizObject[])
            {
                objs = (DataModel.BizObject[])v;
            }
            if (!field.IsProperty)
            {// 关联对象，存储到 Session 中，因为 ObjectID 不是固定的
                // 可能需要设计将业务字段赋值给 ObjectID
                HttpContext.Current.Session[BizObject.ObjectID + field.Name] = objs;
            }
            return objs;
        }
        #endregion

        #endregion

        #region 验证表单锁 -----------------
        /// <summary>
        /// 检查是否满足锁定的要求
        /// </summary>
        /// <returns>可以编辑返回true,否则返回false</returns>
        public bool ValidateLockLevel(ref string Message)
        {
            if (this.ActionContext.WorkItem != null && this.ActionContext.IsWorkMode)
            {
                return SheetUtility.ValidateLockLevel(this.ActionContext.Engine,
                    this.ActionContext.User,
                    this.ActionContext.ActivityTemplate,
                    this.ActionContext.WorkItem,
                    ref Message);
            }
            return true;
        }

        /// <summary>
        /// 检查是否满足锁定的要求
        /// </summary>
        /// <returns></returns>
        public MvcResult LockActivity()
        {
            MvcResult result = new MvcResult();
            if (this.ActionContext.WorkItem != null && this.ActionContext.IsWorkMode)
            {
                string message = string.Empty;
                if (SheetUtility.TryLock(this.ActionContext.Engine,
                    this.ActionContext.User,
                     this.ActionContext.ActivityTemplate,
                     this.ActionContext.WorkItem,
                     ref message))
                {// 锁定成功，需要刷新页面
                    result.Message = Configs.Global.ResourceManager.GetString("MvcController_LockSuccess");
                    result.Refresh = true;
                    result.Successful = true;
                }
                else
                {
                    result.Successful = false;
                    result.Message = message;
                }
            }
            // 记录操作日志
            this.AddUserLog(Tracking.UserLogType.LockInstance);
            return result;
        }

        /// <summary>
        /// 检查是否满足锁定的要求
        /// </summary>
        /// <returns></returns>
        public MvcResult UnLockActivity()
        {
            MvcResult result = new MvcResult();
            if (this.ActionContext.WorkItem != null && this.ActionContext.IsWorkMode)
            {
                WorkflowTemplate.ParticipativeActivity task = (WorkflowTemplate.ParticipativeActivity)this.ActionContext.ActivityTemplate;
                // 解锁
                WorkItem.LockResult lockResult = this.ActionContext.Engine.WorkItemManager.DropLock(
                    this.ActionContext.WorkItemID,
                    task.LockLevel == OThinker.H3.WorkflowTemplate.LockLevel.CancelOthers);
                if (lockResult == LockResult.Success)
                {
                    result.Message = Configs.Global.ResourceManager.GetString("MvcController_UnlockSuccess");
                    result.Successful = true;
                    result.ClosePage = true;
                }
                else
                {
                    result.Successful = false;
                    result.Message = Configs.Global.ResourceManager.GetString("MvcController_UnlockFailed");
                }
            }
            // 记录操作日志
            this.AddUserLog(Tracking.UserLogType.UnlockInstance);
            return result;
        }
        #endregion
    }
}
