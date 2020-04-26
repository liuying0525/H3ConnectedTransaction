using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Web;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.DataModel;
using System.Web.UI;
using System.IO;
using OThinker.H3.Sheet;
using OThinker.H3.Configs;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 表单环境，用于获得各项工作流服务，当前表单的工作模式和流程数据
    /// </summary>
    public class ActionContext
    {
        private HttpRequest _Request;
        /// <summary>
        /// 页面 HttpRequest 对象
        /// </summary>
        public HttpRequest Request
        {
            get
            {
                return this._Request;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Request">Request对象</param>
        public ActionContext(HttpRequest Request)
        {
            this._Request = Request;
        }

        private OThinker.Data.BoolMatchValue _Authorized = OThinker.Data.BoolMatchValue.Unspecified;
        public OThinker.Data.BoolMatchValue Authorized
        {
            get
            {
                return this._Authorized;
            }
        }

        private bool _LockValidation = true;
        public bool LockValidation
        {
            get
            {
                return this._LockValidation;
            }
        }

        private string _Message;
        public string Message
        {
            get
            {
                return this._Message;
            }
        }

        #region URL参数 --------------------
        /// <summary>
        /// 表单URL上的参数，用于指定打开表单所使用的模式，比如：只读模式、打印模式、工作模式等
        /// </summary>
        public const string Param_Mode = "Mode";
        /// <summary>
        /// 表单上模式编码参数
        /// </summary>
        public const string Param_SchemaCode = "SchemaCode";
        /// <summary>
        /// 表单上业务对象ID参数
        /// </summary>
        public const string Param_BizObjectID = "BizObjectID";
        /// <summary>
        /// 表单URL上的参数，如果是以打开一个工作项的角度来打开这个表单的话，那么需要指定该参数
        /// </summary>
        public const string Param_WorkItemID = "WorkItemID";
        /// <summary>
        /// 表单URL上的参数，如果是以打开一个流程的表单的角度来打开这个表单的话，那么需要指定该参数
        /// </summary>
        public const string Param_InstanceId = "InstanceId";
        /// <summary>
        /// 表单URL上的参数，流程发起时有效，用于指定当前表单对应的流程模板。实际上，一个表单可以被N个流程模板共用。
        /// </summary>
        public const string Param_WorkflowCode = "WorkflowCode";
        /// <summary>
        /// 表单编码
        /// </summary>
        public const string Param_SheetCode = "SheetCode";
        /// <summary>
        /// 表单URL上的参数，流程发起时有效，用于指定当前表单对应的流程模板。实际上，一个表单可以被N个流程模板/版本共用。
        /// </summary>
        public const string Param_WorkflowVersion = "WorkflowVersion";
        /// <summary>
        /// 表单URL上的参数，用于指定当前用户名，这样用户不用登陆即可直接打开表单
        /// </summary>
        public const string Param_LoginName = "LoginName";
        /// <summary>
        /// 表单URL上的参数，用于指定当前用户的密码，这样用户不用登陆即可直接打开表单
        /// </summary>
        public const string Param_LoginPassword = "LoginPassword";
        /// <summary>
        /// 表单URL上的参数，用于传递业务对象的 Method Name
        /// </summary>
        public const string Param_Method = "Method";
        /// <summary>
        /// 是否通过手机访问的
        /// </summary>
        public const string Param_IsMobile = "IsMobile";
        /// <summary>
        /// 保存后
        /// </summary>
        public const string Param_AfterSave = "AfterSave";

        /// <summary>
        /// 是否通过手机访问的
        /// </summary>
        public bool IsMobile
        {
            get
            {
                return SheetUtility.IsMobile;
            }
        }

        private SheetMode _SheetMode = SheetMode.Unspecified;
        /// <summary>
        /// 打开表单所使用的模式，比如：只读模式、打印模式、工作模式等，如果URL参数中未指定，则默认为Unspecified，由表单程序自行指定
        /// </summary>
        public SheetMode SheetMode
        {
            get
            {
                if (_SheetMode == SheetMode.Unspecified)
                {
                    string strMode = this.Request.QueryString[Param_Mode];
                    if (string.IsNullOrEmpty(strMode))
                    {
                        this._SheetMode = SheetMode.View;
                    }
                    else
                    {
                        this._SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), strMode);
                    }
                }

                return this._SheetMode;
            }
        }

        /// <summary>
        /// 平台模式
        /// </summary>
        private SheetDataType _SheetDataType = SheetDataType.Unspecified;
        /// <summary>
        /// 获取平台模式(流程/开发平台)
        /// </summary>
        public SheetDataType SheetDataType
        {
            get
            {
                if (this._SheetDataType == SheetDataType.Unspecified) this.GetDataFromServer();
                return this._SheetDataType;
            }
        }

        /// <summary>
        /// 获取当前表单对应的工作任务ID
        /// </summary>
        public string WorkItemID
        {
            get
            {
                if (this.WorkItem == null)
                {
                    if (this.CirculateItem != null)
                    {
                        return this.CirculateItem.ObjectID;
                    }
                    return null;
                }
                return this.WorkItem.WorkItemID;
            }
        }

        private OThinker.H3.WorkItem.WorkItem _WorkItem = null;
        /// <summary>
        /// 获取当前任务
        /// </summary>
        public OThinker.H3.WorkItem.WorkItem WorkItem
        {
            get
            {
                if (this._WorkItem == null) this.GetDataFromServer();
                return this._WorkItem;
            }
        }

        private OThinker.H3.WorkItem.CirculateItem _CirculateItem = null;
        public OThinker.H3.WorkItem.CirculateItem CirculateItem
        {
            get
            {
                if (this._WorkItem == null) this.GetDataFromServer();
                return this._CirculateItem;
            }
        }

        private string _InstanceId = null;
        /// <summary>
        /// 当前表单对应的流程实例ID
        /// </summary>
        public string InstanceId
        {
            get
            {
                if (this.InstanceContext == null)
                {
                    if (this._InstanceId == null)
                    {
                        this._InstanceId = Guid.NewGuid().ToString();
                    }
                    return this._InstanceId;
                }
                else
                {
                    return this.InstanceContext.InstanceId;
                }
            }
            set
            {
                this._InstanceId = value;
            }
        }

        private Instance.InstanceContext _InstanceContext = null;
        /// <summary>
        /// 当前表单对应的流程实例的上下文
        /// </summary>
        public Instance.InstanceContext InstanceContext
        {
            get
            {
                if (this._InstanceContext == null) this.GetDataFromServer();
                return this._InstanceContext;
            }
        }

        /// <summary>
        /// 当前表单对应的流程模板编码
        /// </summary>
        public string WorkflowCode
        {
            get
            {
                return this.Workflow == null ? string.Empty : this.Workflow.WorkflowCode;
            }
        }

        /// <summary>
        /// 当前表单对应的流程模板版本
        /// </summary>
        public int WorkflowVersion
        {
            get
            {
                return this.Workflow.WorkflowVersion;
            }
        }

        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate _Workflow = null;
        /// <summary>
        /// 当前表单对应的流程模板
        /// </summary>
        public OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate Workflow
        {
            get
            {
                if (this._Workflow == null) this.GetDataFromServer();
                return this._Workflow;
            }
        }

        private DataModel.BizObjectSchema _Schema = null;
        /// <summary>
        /// 获取或设置数据模型结构
        /// </summary>
        public DataModel.BizObjectSchema Schema
        {
            get
            {
                if (this._Schema == null) this.GetDataFromServer();
                return this._Schema;
            }
        }

        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode
        {
            get
            {
                return this.Schema.SchemaCode;
            }
        }

        private BizSheet _Sheet = null;
        /// <summary>
        /// 获取当前显示的表单
        /// </summary>
        public BizSheet Sheet
        {
            get
            {
                if (this._Sheet == null) this.GetDataFromServer();
                return _Sheet;
            }
        }

        private string _SheetDisplayName = null;
        /// <summary>
        /// 获取或设置当前表单的显示名称
        /// </summary>
        public string SheetDisplayName
        {
            get
            {
                if (this._SheetDisplayName == null)
                {
                    this.GetDataFromServer();
                    if (this._SheetDisplayName == null)
                        this._SheetDisplayName = this.BizObject.Name;
                }
                return _SheetDisplayName;
            }
        }

        /// <summary>
        /// 获取或设置数据实例ID
        /// </summary>
        public string BizObjectID
        {
            get
            {
                return this.InstanceData.BizObject.ObjectID;
            }
        }

        private string _ActivityCode = null;
        /// <summary>
        /// 当前活动的名称。如果是发起模式，那么返回第一个活动的名称；如果是工作项相关的模式，那么返回工作项对应的活动名称；否则从URL里面读取。
        /// </summary>
        public string ActivityCode
        {
            get
            {
                if (this._ActivityCode == null) this.GetDataFromServer();
                return this._ActivityCode;
            }
        }

        /// <summary>
        /// 获取当前活动对应的活动模板
        /// </summary>
        public ClientActivity ActivityTemplate
        {
            get
            {
                if (this.Workflow == null)
                {
                    return null;
                }
                return this.Workflow.GetActivityByCode(this.ActivityCode) as ClientActivity;
            }
        }

        /// <summary>
        /// 解析活动上定义的参与者
        /// </summary>
        /// <param name="ActivityCode">活动编码</param>
        /// <returns>实际的处理人，这个会具体落实到组织结构里的某个/某些人</returns>
        public string[] ParseDefinedParticipants(string ActivityCode)
        {
            // 获得活动模板
            OThinker.H3.WorkflowTemplate.ClientActivity activity = this.Workflow.GetActivityByCode(ActivityCode) as H3.WorkflowTemplate.ClientActivity;
            if (activity == null)
            {
                return null;
            }
            // 返回活动上定义的人员
            return activity.ParseParticipants(this.InstanceData, this.Engine.Organization);
        }

        /// <summary>
        /// 获得流程上所有活动上定义的参与者
        /// </summary>
        /// <returns>实际的处理人，这个会具体落实到组织结构里的某个/某些人</returns>
        public string[] ParseDefinedWorkflowParticipants()
        {
            if (this.Workflow == null || this.Workflow.Activities == null)
            {
                return null;
            }
            // 遍历所有活动，获得所有定义的参与者
            string participants = string.Empty;

            foreach (H3.WorkflowTemplate.Activity activity in this.Workflow.Activities)
            {
                if (activity is ClientActivity)
                {
                    participants += ((ClientActivity)activity).Participants + ";";
                }
            }
            return H3.WorkflowTemplate.ClientActivity.ParseParticipants(participants, this.InstanceData, this.Engine.Organization);
        }

        #endregion

        #region 表单数据

        private bool _GetDataFromServer = false;
        /// <summary>
        /// 从服务器读取数据
        /// </summary>
        private void GetDataFromServer()
        {
            if (!_GetDataFromServer)
            {
                _GetDataFromServer = true;
                string strWorkItemId = OThinker.Data.Convertor.SqlInjectionPrev(this.Request.QueryString[Param_WorkItemID]);
                string strInstanceId = OThinker.Data.Convertor.SqlInjectionPrev(this.Request.QueryString[Param_InstanceId]);
                string strWorkflowCode = OThinker.Data.Convertor.SqlInjectionPrev(this.Request.QueryString[Param_WorkflowCode]);
                string strSchemaCode = OThinker.Data.Convertor.SqlInjectionPrev(this.Request.QueryString[Param_SchemaCode]);
                int workflowVersion = WorkflowTemplate.PublishedWorkflowTemplate.NullWorkflowVersion;
                int.TryParse(this.Request.QueryString[Param_WorkflowVersion], out workflowVersion);
                string strSheetCode = OThinker.Data.Convertor.SqlInjectionPrev(this.Request.QueryString[OThinker.H3.Controllers.SheetEnviroment.Param_SheetCode] + string.Empty);

                if (!string.IsNullOrEmpty(strWorkItemId)
                    || !string.IsNullOrEmpty(strInstanceId)
                    || (this.IsOriginateMode && !string.IsNullOrEmpty(strWorkflowCode)))
                {
                    // 流程模式
                    this._SheetDataType = SheetDataType.Workflow;
                }
                else if (!string.IsNullOrEmpty(strSchemaCode))
                {
                    // 开发平台模式
                    this._SheetDataType = SheetDataType.BizObject;
                }

                if (this.SheetDataType == SheetDataType.Workflow)
                {
                    this.Engine.Interactor.GetDataForWorkflow(
                        (InteractiveType)this.SheetMode,
                        strWorkflowCode,
                        workflowVersion,
                        strInstanceId,
                        strWorkItemId,
                        strSheetCode,
                        this.User.UserID,
                        SheetUtility.GetClientIP(this.Request),
                        SheetUtility.GetClientPlatform(this.Request),
                        SheetUtility.GetClientBrowser(this.Request),
                        out this._Authorized,
                        out this._InstanceContext,
                        out this._WorkItem,
                        out this._CirculateItem,
                        out this._Workflow,
                        out this._Schema,
                        out this._ActivityCode,
                        out this._Sheet,
                        out this._PostUnits,
                        out this._SheetDisplayName,
                        out this._LockValidation,
                        out this._Message);
                }
                else
                {
                    this.Engine.Interactor.GetDataForBizObject(
                        strSchemaCode,
                        strSheetCode,
                        out this._Schema,
                        out this._Sheet);
                }
            }
        }

        /// <summary>
        /// 获取当前业务实例对象
        /// </summary>
        public BizObject BizObject
        {
            get
            {
                return this.InstanceData.BizObject;
            }
        }

        private OThinker.H3.Instance.InstanceData _InstanceData;
        /// <summary>
        /// 用于获得/设置流程数据的属性，包括值。该对象缓存在this._InstanceData中，当页面PostBack的时候需要重新创建。
        /// InstanceData启用缓存机制，也就是说，对于任意一个数据项的修改，并不会直接回写到服务器一，只有等到调用InstanceData.Submit方法之后，才会更新服务器；同时，读取一个只得时候也只会从缓存中读取，只有无法读到，才从服务器上请求。
        /// </summary>
        public OThinker.H3.Instance.InstanceData InstanceData
        {
            get
            {
                if (this._InstanceData == null)
                {
                    this._InstanceData = new OThinker.H3.Instance.InstanceData(
                        this.Engine,
                        this.InstanceId,
                        this.InstanceContext,
                        this.WorkItem == null ? Instance.Token.UnspecifiedID : this.WorkItem.TokenId,
                        this.Workflow,
                        this.ActivityCode,
                        this.ParticipantId,
                        this.ParticipantParentId,
                        null,
                        this._Schema,
                        this.Request.QueryString,
                        this.IsOriginateMode ? string.Empty : OThinker.Data.Convertor.SqlInjectionPrev(this.Request.QueryString[Param_BizObjectID]));
                }
                return this._InstanceData;
            }
        }

        /// <summary>
        /// 堆数据访问对象。堆数据是一种没有事先在流程里定义的数据项，只要是可以序列化的对象都可以使用堆数据来存储。
        /// 值得注意的是，如果要存储的数据太大了，那么可能会造成性能问题，实际上每次访问一个流程实例的数据项的时候，都会锁定某个流程实例。如果数据太大，则可能会造成该流程被阻塞。所以，请不要使用该对象来存储文件之类的内容。
        /// </summary>
        public HeapDataItemCollection HeapData
        {
            get
            {
                return new HeapDataItemCollection(this.Engine, this.InstanceId);
            }
        }

        #endregion

        #region 表单模式 -------------------

        /// <summary>
        /// 当前表单模式是否是工作模式
        /// </summary>
        public bool IsWorkMode
        {
            get
            {
                return
                    this.SheetMode == SheetMode.Work &&
                    ((this.SheetDataType == SheetDataType.Workflow && this.WorkItem != null && this.WorkItem.IsUnfinished)
                        || this.SheetDataType == SheetDataType.BizObject);
            }
        }

        /// <summary>
        /// 当前表单模式是否是发起模式
        /// </summary>
        public bool IsOriginateMode
        {
            get
            {
                return this.SheetMode == SheetMode.Originate;
            }
        }

        /// <summary>
        /// 当前表单模式是否是查看模式(查看模式表单只允许只读)
        /// </summary>
        public bool IsViewMode
        {
            get
            {
                if (this.SheetMode == SheetMode.View)
                {
                    return true;
                }
                if (this.WorkItem != null
                    && (this.WorkItem.State == H3.WorkItem.WorkItemState.Finished || this.WorkItem.State == H3.WorkItem.WorkItemState.Canceled))
                {
                    return true;
                }
                return false;
            }
        }

        #endregion

        #region 当前用户 -------------------
        /// <summary>
        /// 获得当前登陆用户
        /// </summary>
        public UserValidator User
        {
            get
            {
                return UserValidatorFactory.CurrentUser;
            }
        }

        /// <summary>
        /// 获取当前引擎实例对象
        /// </summary>
        public IEngine Engine
        {
            get
            {
                return this.User.Engine;
            }
        }

        private Organization.Unit[] _PostUnits = null;
        /// <summary>
        /// 获取用户所属角色信息
        /// </summary>
        public Organization.Unit[] PostUnits
        {
            get
            {
                if (this._PostUnits == null) this.GetDataFromServer();
                return this._PostUnits;
            }
        }

        /// <summary>
        /// 获取当前登录用户的ID
        /// </summary>
        public string ParticipantId
        {
            get
            {
                if (this.WorkItem == null)
                {
                    // 当前用户
                    return this.User.UserID;
                }

                OThinker.H3.Settings.AgencyIdentityType identityType = (OThinker.H3.Settings.AgencyIdentityType)this.User.PortalSettings[Settings.CustomSetting.Setting_AgencyIdentityType];
                switch (identityType)
                {
                    case Settings.AgencyIdentityType.Source:
                        if (!string.IsNullOrEmpty(this.WorkItem.Delegant))
                        {
                            // 最初委托人
                            return this.WorkItem.Delegant;
                        }
                        else
                        {
                            // 参与者
                            return this.WorkItem.Participant;
                        }
                    case Settings.AgencyIdentityType.Agent:
                        return this.User.UserID;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 获取当前任务处理用户所在的组织ID
        /// </summary>
        public string ParticipantParentId
        {
            get
            {
                return this.Participant.User.ParentID;
            }
        }

        private UserValidator _Participant = null;
        /// <summary>
        /// 获取当前用户的访问对象，通过该对象可以获得当前用户的所有信息，包括：所属的组织结构、权限等。
        /// </summary>
        public UserValidator Participant
        {
            get
            {
                if (this._Participant == null)
                {
                    if (this.ParticipantId == this.User.UserID)
                    {
                        this._Participant = this.User;
                    }
                    else
                    {
                        string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
                        this._Participant = UserValidatorFactory.GetUserValidatorById(this.User.Engine, tempImagesPath, this.ParticipantId, this.User.PortalSettings);
                    }
                }
                return this._Participant;
            }
        }

        #endregion

        #region 所有的隐藏字段 -------------

        private bool GotHiddenFields = false;
        private bool IsHiddenFieldsDirty = false;
        private Dictionary<string, string> _HiddenFields = null;
        /// <summary>
        /// 表单上的所有HiddenFields
        /// </summary>
        private Dictionary<string, string> HiddenFields
        {
            get
            {
                if (!this.GotHiddenFields)
                {
                    if (this.SheetMode == SheetMode.Originate)
                    {
                        this._HiddenFields = new Dictionary<string, string>();
                    }
                    else
                    {
                        this._HiddenFields = Instance.HeapDataItem.GetHiddenFields(this.Engine.HeapDataManager, this.InstanceId);
                    }
                    this.GotHiddenFields = true;
                }
                return this._HiddenFields;
            }
        }

        /// <summary>
        /// 提供给SheetHiddenField控件使用，用于记录HiddenField的值。如果该字段存在，则更新；否则新建一个
        /// </summary>
        /// <param name="FieldID">字段ID，通常是SheetHiddenField控件的ID</param>
        /// <param name="FieldValue">字段的值</param>
        public void SetHiddenField(string FieldID, string FieldValue)
        {
            if (string.IsNullOrEmpty(FieldID))
            {
                return;
            }
            this.IsHiddenFieldsDirty = true;
            if (this.HiddenFields == null)
            {
                this._HiddenFields = new Dictionary<string, string>();
            }
            if (this.HiddenFields.ContainsKey(FieldID))
            {
                this.HiddenFields[FieldID] = FieldValue;
            }
            else
            {
                this.HiddenFields.Add(FieldID, FieldValue);
            }
        }

        /// <summary>
        /// 获得一个字段的值
        /// </summary>
        /// <param name="FieldID">字段ID，通常是SheetHiddenField控件的ID</param>
        /// <returns>值。如果字段存在，则返回值，否则返回null</returns>
        public string GetHiddenField(string FieldID)
        {
            if (FieldID == null ||
                this.HiddenFields == null ||
                !this.HiddenFields.ContainsKey(FieldID))
            {
                return null;
            }
            else
            {
                return this.HiddenFields[FieldID];
            }
        }

        /// <summary>
        /// 保存HiddenField，该方法会调用服务器的方法，将值保存到服务器上
        /// </summary>
        public void SaveHiddenField()
        {
            if (this.IsHiddenFieldsDirty)
            {
                // 只有修改过才进行保存
                Instance.HeapDataItem.SetHiddenFields(this.Engine.HeapDataManager, this.InstanceId, this.HiddenFields);
            }
        }

        #endregion

        #region 日志 -----------------------
        /// <summary>
        /// 输出用户操作日志
        /// </summary>
        /// <param name="LogType">日志的类型</param>
        public void WriteLog(OThinker.H3.Tracking.UserLogType LogType)
        {
            OThinker.H3.Tracking.UserLog log = new OThinker.H3.Tracking.UserLog(
                LogType,
                this.User.UserID,
                this.SchemaCode,
                this.BizObjectID,
                this.InstanceId,
                this.WorkItemID,
                this.WorkItem == null ? null : this.WorkItem.DisplayName,
                null,
                SheetUtility.GetClientIP(this.Request),
                SheetUtility.GetClientPlatform(this.Request),
                SheetUtility.GetClientBrowser(this.Request));
            this.Engine.UserLogWriter.Write(log);
        }
        #endregion
    }
}
