using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Data.Database;
using OThinker.Data;
using System.Data;
using System.IO;
using OThinker.H3.Acl;
using System.Text;
using OThinker.H3.Sheet;
using System.Web.SessionState;
using System.Diagnostics;
using System.Web.Security;
using System.Web.Script.Serialization;
using OThinker.H3.DataModel;
using OThinker.Organization;
using System.Text.RegularExpressions;
using System.Web.Routing;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 所有 Controller 的基类
    /// </summary>
    /// 
    public abstract class ControllerBase : Controller
    {
        /// <summary>
        /// 工作任务显示的时间格式
        /// </summary>
        public const string WorkItemTimeFormat = "yyyy-MM-dd HH:mm";

        #region PortalPage 常量
        public const string Param_Keyword = "Keyword";
        public const string PageName_CopyMember = "CopyMember.aspx";
        public const string Param_HighlightDirItemId = "HighlightDirItemId";
        public const string Param_AttachmentID = "AttachmentID";
        public const string Param_UnitType = "UnitType";
        public const string Param_StartSelectableLevel = "StartSelectableLevel";
        public const string Param_EndSelectableLevel = "EndSelectableLevel";
        public const string Param_ExpandToLevel = "ExpandToLevel";
        public const string Param_VisibleUnitType = "VisibleUnitType";
        public const string Param_VisibleType = "VisibleType";
        public const string Param_VisibleUnits = "VisibleUnits";
        public const string Param_BaseUnits = "BaseUnits";
        public const string Param_RoleName = "RoleName";
        public const string Param_VisibleCategories = "VisibleCategories";
        public const string Param_CategoryCodes = "CategoryCodes";
        public const string Param_ActionName = "ActionName";
        public const string Param_ActionEventType = "ActionEventType";
        public const string Param_ActionButtonType = "ActionButtonType";
        public const string Param_Priority = "Priority";
        public const string Param_LeastRecurrences = "LeastRecurrences";
        public const string Param_OptionalRecipients = "OptionalRecipients";
        public const string Param_TrackID = "TrackID";
        public const string Param_TextBoxID = "TextBoxID";
        public const string Param_ControlID = "ControlID";
        public const string Param_SelectMode = "Mode";
        // 委托人
        public const string Param_Delegant = "Delegant";
        public const string Param_AgencyID = "AgencyID";
        // 被委托人
        public const string Param_Delegatee = "Delegatee";
        public const string Param_Message = "Message";
        public const string Param_Mode = "Mode";
        public const string Param_State = "State";
        public const string Param_ItemName = "ItemName";
        public const string Param_InstanceId = "InstanceId";
        public const string Param_InstanceName = "InstanceName";
        public const string Param_DataField = "DataField";
        public const string Param_Scope = "Scope";

        public const string Param_EditBizObject_AfterSave = "AfterSave";
        /// <summary>
        /// 参数：数据模型编码
        /// </summary>
        public const string Param_SchemaCode = "SchemaCode";
        /// <summary>
        /// 参数：数据模型ID
        /// </summary>
        public const string Param_BizObjectID = "BizObjectID";
        /// <summary>
        /// 参数：流程编码
        /// </summary>
        public const string Param_WorkflowCode = "WorkflowCode";
        /// <summary>
        /// 参数：流程版本号
        /// </summary>
        public const string Param_WorkflowVersion = "WorkflowVersion";
        public const string Param_ExceptionID = "ExceptionID";
        public const string Param_AclID = "AclID";
        public const string Param_FunctionCode = "FunctionCode";
        public const string Param_Alias = "Alias";
        public const string Param_From = "From";
        public const string Param_To = "To";
        public const string Param_Elapsed = "Elapsed";
        public const string Param_EstimatedElapsed = "EstimatedElapsed";
        // 工作项ID
        public const string Param_WorkItemID = "WorkItemID";
        public const string Param_WorkItemType = "WorkItemType";
        // 目的活动
        public const string Param_DestActivityName = "DestActivityName";
        // 是否同意
        public const string Param_Approval = "Approval";
        public const string Param_Comment = "Comment";
        public const string Param_ParticipateGroup = "ParticipateGroup";
        public const string Param_ParticipatePost = "ParticipatePost";
        public const string Param_DestParticipantType = "DestParticipantType";

        // 是否是移动模式
        public const string Param_IsMobile = "IsMobile";

        //应用程序
        public const string Param_AppCode = "AppCode";

        // 是否是创建
        public const string Param_EditMode = "EditMode";
        /// <summary>
        /// 编辑中的表单编码
        /// </summary>
        public const string Param_SheetCode = "SheetCode";
        public const string Param_FileName = "FileName";
        // 用户名
        public const string Param_UserID = "UserID";
        public const string Param_ID = "ID";
        public const string Param_ADUser = "ADUser";
        public const string Param_ADPath = "ADPath";
        public const string Param_Parent = "Parent";
        public const string Param_ParentGroup = "ParentGroup";
        public const string Param_OriginateRole = "OriginateRole";

        #endregion
        #region 正则表达式
        public const string ReportCodeReg = "^([a-zA-Z0-9]|[_])+$";
        #endregion
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public abstract string FunctionCode { get; }

        private OThinker.H3.IEngine _Engine = null;
        /// <summary>
        /// 获取引擎对象
        /// </summary>
        public OThinker.H3.IEngine Engine
        {
            get
            {
                if (_Engine == null)
                {
                    _Engine = AppUtility.Engine;
                }
                return _Engine;
            }
        }

        private string _PortalRoot = null;
        /// <summary>
        /// 获取站点名称
        /// </summary>
        public string PortalRoot
        {
            get
            {
                if (_PortalRoot == null)
                {
                    _PortalRoot = ConfigurationManager.AppSettings["PortalRoot"] + string.Empty;
                    if (this._PortalRoot == string.Empty) _PortalRoot = "/Portal";
                    if (this._PortalRoot == "/") _PortalRoot = "";//为“/”时会把localhost和端口号丢失
                }
                return this._PortalRoot;
            }
        }

        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    base.OnActionExecuting(filterContext);
        //}

        protected override IAsyncResult BeginExecute(RequestContext requestContext, AsyncCallback callback, object state)
        {

            System.Web.Mvc.MvcHandler.DisableMvcResponseHeader = true;
            return base.BeginExecute(requestContext, callback, state);
        }

        public UserValidator _UserValidator = null;
        /// <summary>
        /// 获取当前登录用户
        /// </summary>
        public UserValidator UserValidator
        {
            get
            {
                if (this._UserValidator == null)
                {
                    string message = string.Empty;
                    this._UserValidator = UserValidatorFactory.GetUserValidator(this, this.PortalRoot, out message);// Session[Sessions.GetUserValidator()] as UserValidator;
                    // 备注：临时使用
                    if (this._UserValidator == null)
                    {
                        if (ConfigurationManager.AppSettings["Test"] + string.Empty != string.Empty)
                        {
                            this._UserValidator = UserValidatorFactory.GetUserValidator(this.Engine, ConfigurationManager.AppSettings["Test"] + string.Empty);
                            this.Session[Sessions.GetUserValidator()] = this._UserValidator;
                            FormsAuthentication.SetAuthCookie(this._UserValidator.UserCode, false);
                        }
                    }
                }
                return this._UserValidator;
            }
        }

        /// <summary>
        /// 执行带有权限认证的方法
        /// </summary>
        /// <param name="func">功能函数，返回JsonResult</param>
        /// <returns></returns>
        public JsonResult ExecuteFunctionRun(Func<JsonResult> func)
        {
            return this.ExecuteFunctionRun(func, this.FunctionCode);
        }


        /// <summary>
        /// 执行带有权限的功能代码，func函数返回FileResult类，下载附件使用
        /// </summary>
        /// <param name="func">功能函数，返回FileResult</param>
        public Object ExecuteFileResultFunctionRun(Func<FileResult> func)
        {
            return this.ExecuteFileResultFunctionRun(func, this.FunctionCode);
        }

        /// <summary>
        /// 执行带有权限的功能代码，func函数返回FileResult类，下载附件使用
        /// </summary>
        /// <param name="func">功能函数，返回FileResult</param>
        /// <param name="functionCode">功能代码</param>
        /// <returns></returns>
        public object ExecuteFileResultFunctionRun(Func<FileResult> func, string functionCode)
        {
            if (!string.IsNullOrEmpty(functionCode) && this.UserValidator == null)
            {
                ActionResult result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
                return result;
            }
            else
            {
                if (string.IsNullOrEmpty(functionCode) ||
                    this.UserValidator.ValidateFunctionRun(functionCode))
                {
                    return func();
                }
                ActionResult result = new ActionResult(false, "您没有访问当前功能的权限！");
                return result;
            }
        }

        /// <summary>
        /// 执行带有权限的功能代码
        /// </summary>
        /// <param name="func">功能函数，返回JsonResult</param>
        /// <param name="functionCode">功能代码</param>
        /// <returns></returns>
        public JsonResult ExecuteFunctionRun(Func<JsonResult> func, string functionCode)
        {
            //if (!string.IsNullOrEmpty(functionCode) && this.UserValidator == null)
            if (this.UserValidator == null)
            {
                ActionResult result = new ActionResult(false, "登录超时！", null, ExceptionCode.NoAuthorize);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            else
            {
                if (string.IsNullOrEmpty(functionCode) ||
                    this.UserValidator.ValidateFunctionRun(functionCode))
                {
                    return func();
                }
                ActionResult result = new ActionResult(false, "您没有访问当前功能的权限！");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 判断流程包ID锁定判断和处理
        /// </summary>
        /// <param name="BizWorkflowPackageID">BizObjectSchemaCode</param>
        /// <returns>0，锁定，1未锁定</returns>
        public int BizWorkflowPackageLockByID(string BizWorkflowPackageID, string OwnSchemaCode = "")
        {
            int IsControlUsable = 1;
            FunctionNode functionNode = this.Engine.FunctionAclManager.GetFunctionNodeByCode(BizWorkflowPackageID);
            if (functionNode != null)
            {
                if (functionNode.IsLocked == true && !this.UserValidator.UserID.Equals(functionNode.LockedBy))
                {

                    IsControlUsable = 0;
                }
                else
                {
                    OThinker.H3.DataModel.BizObjectSchema bizObjectSchema = this.Engine.BizObjectManager.GetDraftSchema(OwnSchemaCode);
                    if (bizObjectSchema != null && bizObjectSchema.IsQuotePacket) IsControlUsable = 0;
                }
            }
            return IsControlUsable;
        }

        #region 创建LigerUI表格的数据，已序列化

        #region 每页数量
        /// <summary>
        /// 每页数量
        /// </summary>
        public int Pagesize
        {
            get { return Convert.ToInt32(Request.Params["PageSize"] ?? "0"); }
        }
        #endregion

        #region 页码
        /// <summary>
        /// 页码
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                //分页时有时候传递的是page,有时候是pageindex,找到原因后使用统一参数
                return (Convert.ToInt32(Request.Params["page"] ?? "0")) + (Convert.ToInt32(Request.Params["pageIndex"] ?? "0"));
            }
        }
        #endregion

        /// <summary>
        /// 创建LigerUI表格的数据，已序列化
        /// </summary>
        /// <param name="rowOjb"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public object CreateLigerUIGridData(object[] rowOjb)
        {
            int totalCount = rowOjb == null ? 0 : rowOjb.Length;
            int startIndex = 0;
            int endIndex = totalCount;

            List<Object> objList = new List<object>();
            if (Pagesize > 0 && CurrentPageIndex > 0)
            {
                startIndex = (CurrentPageIndex - 1) * Pagesize < 0 ? 0 : (CurrentPageIndex - 1) * Pagesize;
                endIndex = CurrentPageIndex * Pagesize >= totalCount ? totalCount : CurrentPageIndex * Pagesize;
            }

            for (int i = startIndex; i < endIndex; i++)
            {
                objList.Add((rowOjb as object[])[i]);
            }

            var jsonObj = new { Rows = objList, Total = totalCount };
            return jsonObj;
        }

        #endregion

        #region 格式转换 --------------------
        /// <summary>
        /// 批量获取组织对象的显示名称集合
        /// </summary>
        /// <param name="dtSource">数据表</param>
        /// <param name="columns">组织对象存储的字段名称</param>
        /// <returns>返回组织对象的显示名称集合</returns>
        public Dictionary<string, string> GetUnitNamesFromTable(DataTable dtSource, string[] columns)
        {
            int rowCount = dtSource.Rows.Count;
            List<string> userIds = new List<string>();

            for (int i = 0; i < rowCount; i++)
            {
                foreach (string column in columns)
                {
                    string userId = dtSource.Rows[i][column] + string.Empty;
                    if (!string.IsNullOrEmpty(userId) && !userIds.Contains(userId))
                    {
                        userIds.Add(userId);
                    }
                }
            }
            return this.Engine.Organization.GetNames(userIds.ToArray());
        }

        public Dictionary<string, string> GetUnitCodesFromTable(DataTable dtSource, string[] columns)
        {
            int rowCount = dtSource.Rows.Count;
            Dictionary<string, string> DicUnitCodes = new Dictionary<string, string>();
            List<string> userIds = new List<string>();

            for (int i = 0; i < rowCount; i++)
            {
                foreach (string column in columns)
                {
                    string userId = dtSource.Rows[i][column] + string.Empty;
                    if (!string.IsNullOrEmpty(userId) && !userIds.Contains(userId))
                    {
                        userIds.Add(userId);
                    }
                }
            }
            List<OThinker.Organization.Unit> units = this.Engine.Organization.GetUnits(userIds.ToArray());
            foreach (var unit in units)
            {
                OThinker.Organization.User user = unit as OThinker.Organization.User;
                if (user == null)
                {
                    DicUnitCodes[unit.ObjectID] = unit.Name;
                }
                else
                {
                    DicUnitCodes[user.ObjectID] = user.Code;
                }
            }
            return DicUnitCodes;
        }

        protected Dictionary<string, string> GetParentUnitNamesFromTable(DataTable dtSource, string[] columns)
        {
            int rowCount = dtSource.Rows.Count;
            List<string> userIds = new List<string>();
            List<string> ouIds = new List<string>();
            Dictionary<string, string> userId_ouId = new Dictionary<string, string>();
            Dictionary<string, string> ParentUnitNames = new Dictionary<string, string>();
            for (int i = 0; i < rowCount; i++)
            {
                foreach (string column in columns)
                {
                    string userId = dtSource.Rows[i][column] + string.Empty;
                    if (!userIds.Contains(userId) && userId != "")
                    {
                        userIds.Add(userId);
                        string ouId = this.Engine.Organization.GetParent(userId);
                        userId_ouId[userId] = ouId;
                        if (!string.IsNullOrEmpty(ouId) && !ouIds.Contains(ouId))
                        {
                            ouIds.Add(ouId);
                        }
                    }
                }
            }
            Dictionary<string, string> ouIdsName = this.Engine.Organization.GetNames(ouIds.ToArray());

            foreach (var userId in userIds)
            {
                string ouId = userId_ouId[userId];
                ParentUnitNames[userId] = string.IsNullOrEmpty(ouId) ? "" : ouIdsName[ouId];
            }
            return ParentUnitNames;
        }

        /// <summary>
        /// 批量获取流程模板的显示名称集合
        /// </summary>
        /// <param name="dtSource">数据表</param>
        /// <returns>返回流程模板的显示名称集合</returns>
        protected Dictionary<string, string> GetWorkflowNamesFromTable(DataTable dtSource)
        {
            int rowCount = dtSource.Rows.Count;
            List<string> workflowCodes = new List<string>();

            for (int i = 0; i < rowCount; i++)
            {
                string workflowCode = dtSource.Rows[i][WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty;
                if (!workflowCodes.Contains(workflowCode) && workflowCode != "")
                {
                    workflowCodes.Add(workflowCode);
                }
            }
            var WorkflowNameTable = Engine.PortalQuery.GetWorkflowNameByWorkflowCodes(workflowCodes);
            Dictionary<string, string> WorkflowNames = new Dictionary<string, string>();
            foreach (DataRow row in WorkflowNameTable.Rows)
            {
                string code = row[WorkflowTemplate.WorkflowClause.PropertyName_WorkflowCode] + string.Empty;
                string name = row[WorkflowTemplate.WorkflowClause.PropertyName_WorkflowName] + string.Empty;
                WorkflowNames[code] = name;
            }
            //var WorkflowClause = this.Engine.WorkflowManager.GetClausesBySchemaCodes(workflowCodes.ToArray());
            //foreach (string workflowCode in workflowCodes)
            //{
            //    if (WorkflowClause[workflowCode] != null && WorkflowClause[workflowCode].Length > 0) {
            //        string workflowName = WorkflowClause[workflowCode][0].WorkflowName + string.Empty;
            //        WorkflowNames[workflowCode] = workflowName;
            //    }                
            //}
            return WorkflowNames;
        }

        /// <summary>
        /// 批量获取组织机构完整显示名称
        /// </summary>
        /// <param name="dtSource">数据表</param>
        /// <param name="columns">字段名称</param>
        /// <returns></returns>
        protected Dictionary<string, string> GetUnitFullNamesFromTable(DataTable dtSource, string[] columns)
        {
            int rowCount = dtSource.Rows.Count;
            List<string> userIds = new List<string>();
            for (int i = 0; i < rowCount; i++)
            {
                foreach (string column in columns)
                {
                    string userId = dtSource.Rows[i][column] + string.Empty;
                    if (!userIds.Contains(userId))
                    {
                        userIds.Add(userId);
                    }
                }
            }
            return this.Engine.Organization.GetFullNames(userIds.ToArray());
        }
        /// <summary>
        /// 格式化日期为字符串
        /// </summary>
        /// <param name="dateValue"></param>
        /// <param name="format"></param>
        /// <returns></returns>
        protected string GetValueFromDate(object dateValue, string format)
        {
            DateTime date = DateTime.MinValue;
            if (DateTime.TryParse(dateValue + string.Empty, out date))
            {
                if (date.ToString(format).IndexOf("1753") == -1)
                {
                    return date.ToString(format);
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// 从数据字典中获取KEY的值
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetValueFromDictionary(Dictionary<string, string> dictionary, string key)
        {
            if (key != null && dictionary.ContainsKey(key)) return
                dictionary[key] == null ? string.Empty : dictionary[key];
            return string.Empty;
        }

        /// <summary>
        /// 格式化字符串为日期
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        protected System.DateTime GetTime(string Time)
        {
            try
            {
                return System.DateTime.Parse(Time);
            }
            catch (Exception ex)
            {
                return System.DateTime.MaxValue;
            }
        }

        protected string TrimHtml(string str)
        {
            return str.Replace("&", "").Replace("<", "").Replace("'", "").Replace("\"", "").Replace("\\", "");
        }

        /// <summary>
        /// 流转二进制
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected byte[] GetBytesFromStream(Stream stream)
        {
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, SeekOrigin.Begin);
            return bytes;
        }
        #endregion

        #region 获取流程页面URL
        /// <summary>
        /// 获取待办任务的链接地址
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="WorkItem"></param>
        /// <param name="Sheet"></param>
        /// <param name="Action"></param>
        /// <param name="ActionParam"></param>
        /// <param name="IsMobile"></param>
        /// <param name="LoginName"></param>
        /// <param name="LoginPassword"></param>
        /// <returns></returns>
        public string GetWorkSheetUrl(
            WorkItem.WorkItem WorkItem,
            BizSheet Sheet,
            bool IsMobile)
        {
            if (WorkItem == null)
            {
                return null;
            }
            string baseUrl = string.Empty;
            baseUrl = GetSheetBaseUrl(SheetMode.Work, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode.Work + "&";
            if (WorkItem != null)
            {
                baseUrl += SheetEnviroment.Param_WorkItemID + "=" + WorkItem.WorkItemID + "&";
            }
            return baseUrl;
        }

        /// <summary>
        /// 以默认表单展现,修改流程数据项模式使用
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="BizObjectID"></param>
        /// <param name="Action"></param>
        /// <param name="ActionParam"></param>
        /// <param name="IsMobile"></param>
        /// <param name="LoginName"></param>
        /// <param name="LoginPassword"></param>
        /// <returns></returns>
        public string GetWorkSheetUrl(
          string SchemaCode,
          string BizObjectID,
          SheetMode SheetMode,
          bool IsMobile)
        {
            string baseUrl = string.Empty;
            //baseUrl = System.IO.Path.Combine(this.PortalRoot, "/MvcDefaultSheet.aspx").Replace('\\', '/');
            baseUrl = this.PortalRoot + "/MvcDefaultSheet.aspx";
            baseUrl += IsMobile ? "?IsMobile=true" : "?";

            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode.ToString() + "&";
            baseUrl += SheetEnviroment.Param_SchemaCode + "=" + SchemaCode + "&";
            if (!string.IsNullOrEmpty(BizObjectID))
            {
                baseUrl += SheetEnviroment.Param_BizObjectID + "=" + BizObjectID + "&";
            }
            return baseUrl;
        }

        public string GetWorkSheetUrl(
           string SchemaCode,
           string BizObjectID,
           BizSheet Sheet,
           SheetMode SheetMode,
           bool IsMobile)
        {
            string baseUrl = string.Empty;
            baseUrl = GetSheetBaseUrl(SheetMode.Work, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode.ToString() + "&";
            baseUrl += SheetEnviroment.Param_SchemaCode + "=" + SchemaCode + "&";
            if (!string.IsNullOrEmpty(BizObjectID))
            {
                baseUrl += SheetEnviroment.Param_BizObjectID + "=" + BizObjectID + "&";
            }
            return baseUrl;
        }

        /// <summary>
        /// 获取流程实例打开页面
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="InstanceId"></param>
        /// <param name="WorkItemID"></param>
        /// <returns></returns>
        public string GetInstanceUrl(string InstanceId, string WorkItemID)
        {
            return this.GetInstanceUrl(InstanceId, WorkItemID, DateTime.Now.ToString("yyyyMMddHHmmss"));
        }

        /// <summary>
        /// 获取流程实例打开页面
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="InstanceId"></param>
        /// <param name="WorkItemID"></param>
        /// <param name="Param"></param>
        /// <returns></returns>
        public string GetInstanceUrl(string InstanceId, string WorkItemID, string Param)
        {
            string RootString = PortalRoot;
            //this.Engine.LogWriter.Write("---------Portroot:" + RootString);
            string url = RootString + "/index.html#/InstanceDetail/" + InstanceId + "///";
            //this.Engine.LogWriter.Write("---------URL:" + RootString);
            return url;
        }

        /// <summary>
        /// 获得某一个表单的某一种操作对应的URL
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="InstanceId"></param>
        /// <param name="SheetMode"></param>
        /// <param name="IsMobile"></param>
        /// <returns></returns>
        public string GetViewSheetUrl(BizSheet Sheet, string InstanceId, SheetMode SheetMode, bool IsMobile)
        {
            if (Sheet == null)
            {
                return null;
            }
            else
            {
                string baseUrl = GetSheetBaseUrl(SheetMode.View, IsMobile, Sheet);
                string url = baseUrl +
                     SheetEnviroment.Param_Mode + "=" + SheetMode.ToString() + "&" +
                     SheetEnviroment.Param_InstanceId + "=" + InstanceId + "&" +
                     SheetEnviroment.Param_SheetCode + "=" + Sheet.SheetCode;
                // 检查Url是否是绝对路径
                //if (url.IndexOf("/") == 0)
                //{
                //    // 是绝对路径
                //}
                //else
                //{
                //    // 获得绝对路径
                //    string localPath = this.Request.Url.LocalPath;
                //    int lastIndex = localPath.LastIndexOf("/");
                //    string folder = null;
                //    if (lastIndex == -1)
                //    {
                //        folder = null;
                //    }
                //    else if (lastIndex == 0)
                //    {
                //        folder = "/";
                //    }
                //    else
                //    {
                //        folder = localPath.Substring(0, lastIndex) + "/";
                //    }
                //    url = folder + url;
                //}
                return url;
            }
        }

        /// <summary>
        /// 获取查看工作任务的表单URL
        /// </summary>
        /// <param name="WorkItem"></param>
        /// <param name="Sheet"></param>
        /// <param name="SheetMode"></param>
        /// <param name="IsMobile"></param>
        /// <returns></returns>
        public string GetViewSheetUrl(
            WorkItem.WorkItem WorkItem,
            BizSheet Sheet,
            SheetMode SheetMode,
            bool IsMobile)
        {
            string baseUrl = GetSheetBaseUrl(SheetMode.View, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode + "&";
            baseUrl += SheetEnviroment.Param_WorkItemID + "=" + WorkItem.WorkItemID + "&";
            return baseUrl;
        }

        public string GetViewSheetUrl(
            WorkItem.CirculateItem CirculateItem,
            BizSheet Sheet,
            SheetMode SheetMode,
            bool IsMobile)
        {
            string baseUrl = GetSheetBaseUrl(SheetMode.View, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode + "&";
            baseUrl += SheetEnviroment.Param_WorkItemID + "=" + CirculateItem.ObjectID + "&";
            return baseUrl;
        }

        public string GetViewCirculateItemSheetUrl(
            WorkItem.CirculateItem CirculateItem,
            BizSheet Sheet,
            SheetMode SheetMode,
            bool IsMobile)
        {
            string baseUrl = GetSheetBaseUrl(SheetMode.View, IsMobile, Sheet);
            baseUrl += SheetEnviroment.Param_Mode + "=" + SheetMode + "&";
            baseUrl += SheetEnviroment.Param_WorkItemID + "=" + CirculateItem.ObjectID + "&";
            return baseUrl;
        }


        /// <summary>
        /// 获取打开表单的URL地址
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="SheetMode"></param>
        /// <param name="IsMobile"></param>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        public string GetSheetBaseUrl(SheetMode SheetMode, bool IsMobile, BizSheet Sheet)
        {
            string baseUrl = null;

            switch (Sheet.SheetType)
            {
                case SheetType.None:
                    baseUrl = null;
                    break;
                case SheetType.DefaultSheet:
                    baseUrl = GetDefaultSheetUrl(Sheet, IsMobile);
                    break;
                case SheetType.CustomSheet:
                    string sheetUrl = GetWorkItemSheet(IsMobile, SheetMode, Sheet.MobileSheetAddress, Sheet.SheetAddress, Sheet.PrintSheetAddress);
                    if (sheetUrl.LastIndexOf("?") == -1)
                    {
                        // url的形式应该是http://.../page1.aspx
                        baseUrl = sheetUrl + "?";
                    }
                    else if (sheetUrl.LastIndexOf("?") == sheetUrl.Length - 1
                        || sheetUrl.LastIndexOf("&") == sheetUrl.Length - 1)
                    {
                        // url的形式应该是http://.../page1.aspx?
                        // url的形式应该是http://.../page1.aspx?param1=value1&
                        baseUrl = sheetUrl;
                    }
                    else
                    {
                        // url的形式应该是http://.../page1.aspx?param1=value1
                        baseUrl = sheetUrl + "&";
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
            return baseUrl;
        }

        /// <summary>
        /// 获取任务打开的表单URL
        /// </summary>
        /// <param name="IsMobile"></param>
        /// <param name="SheetMode"></param>
        /// <param name="MobileSheetUrl"></param>
        /// <param name="PCSheetUrl"></param>
        /// <param name="PrintSheetUrl"></param>
        /// <returns></returns>
        public string GetWorkItemSheet(bool IsMobile, SheetMode SheetMode, string MobileSheetUrl, string PCSheetUrl, string PrintSheetUrl)
        {
            if (SheetMode == SheetMode.Print) return PrintSheetUrl;
            if (IsMobile)
            {
                if (!string.IsNullOrEmpty(MobileSheetUrl))
                {
                    return MobileSheetUrl + "?" + Param_IsMobile + "=" + true;
                }
                else
                {
                    return PCSheetUrl + "?" + Param_IsMobile + "=" + true;
                }
            }
            else
            {
                return PCSheetUrl + "?";
            }
        }

        /// <summary>
        /// 获取默认表单URL
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        private string GetDefaultSheetUrl(BizSheet Sheet, bool isMobile)
        {
            return GetDefaultSheetUrl(Sheet) + (isMobile ? "IsMobile=" + isMobile + "&" : "");
        }

        /// <summary>
        /// 获取默认表单URL
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        private string GetDefaultSheetUrl(BizSheet Sheet)
        {
            string SheetUrl = string.Empty;
            if (Sheet.EnabledCode)
            {
                SheetUrl = System.IO.Path.Combine(this.PortalRoot, this.GetDesignerSheetAddress(Sheet));
            }
            else
            {
                if (Sheet.IsMVC)
                {
                    SheetUrl = System.IO.Path.Combine(this.PortalRoot, "MvcDefaultSheet.aspx").Replace('\\', '/');
                }
                else
                {
                    SheetUrl = System.IO.Path.Combine(this.PortalRoot, "DefaultSheet.aspx").Replace('\\', '/');
                }
            }
            SheetUrl += "?" + Param_SheetCode + "=" + Sheet.SheetCode + "&";
            return SheetUrl;
        }

        /// <summary>
        /// 获取设计后的表单URL地址
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="Sheet"></param>
        /// <returns></returns>
        public string GetDesignerSheetAddress(BizSheet Sheet)
        {
            // 表单文件名称
            string sheetAddress = Sheet.SheetAddress == string.Empty ? Sheet.SheetCode + ".aspx" : Sheet.SheetAddress;
            string sheetDirectory = "Sheets/" + AppUtility.Engine.EngineConfig.Code + "/" + Sheet.BizObjectSchemaCode;
            // 表单存储路径
            sheetAddress = Path.Combine(sheetDirectory, sheetAddress);
            // 获取完整的表单路径
            string sheetAddressFullPath = Path.Combine(Server.MapPath("../"), sheetAddress);

            bool created = false;

            if (!System.IO.File.Exists(sheetAddressFullPath))
            {
                if (!Directory.Exists(Path.Combine(Server.MapPath("../"), sheetDirectory)))
                {
                    Directory.CreateDirectory(Path.Combine(Server.MapPath("../"), sheetDirectory));
                }
                created = true;
            }
            else
            {
                FileInfo f = new FileInfo(sheetAddressFullPath);
                if (f.LastWriteTime < Sheet.LastModifiedTime)
                {
                    created = true;
                }
            }

            if (created)
            {
                // 生成文件
                string cs = Sheet.CodeContent;
                string aspx = string.Empty;
                if (cs == string.Empty) cs = GetCSharpCode(AppUtility.Engine.EngineConfig.Code, Sheet.SheetCode);
                if (Sheet.IsMVC)
                {
                    aspx = GetMvcSheetAspx(AppUtility.Engine.EngineConfig.Code, Sheet.SheetCode, Sheet.RuntimeContent, Sheet.Javascript);
                }
                //else
                //{
                //    aspx = GetSheetAspx(AppUtility.Engine.EngineConfig.Code, Sheet.SheetCode, Sheet.RuntimeContent, Sheet.Javascript);
                //}

                using (StreamWriter sw = new StreamWriter(sheetAddressFullPath, false, Encoding.UTF8))
                {
                    sw.Write(aspx);
                }
                using (StreamWriter sw = new StreamWriter(sheetAddressFullPath + ".cs", false, Encoding.UTF8))
                {
                    sw.Write(cs);
                }
            }

            return sheetAddress;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="SheetCode"></param>
        /// <param name="SheetAspx"></param>
        /// <param name="Script"></param>
        /// <returns></returns>
        public string GetMvcSheetAspx(string EngineCode, string SheetCode, string SheetAspx, string Script)
        {
            string aspx = "<%@ Page Language=\"C#\" AutoEventWireup=\"true\" CodeFile=\"" + SheetCode + ".aspx.cs\"";
            aspx += " Inherits=\"OThinker.H3.Portal.Sheets." + EngineCode + "." + SheetCode + "\" EnableEventValidation=\"false\" MasterPageFile=\"~/MvcSheet.master\" %>\n";
            aspx += "<%@ OutputCache Duration=\"999999\" VaryByParam=\"T\" VaryByCustom=\"browser\" %>\n";
            aspx += "<asp:Content ID=\"head\" ContentPlaceHolderID=\"headContent\" runat=\"Server\">\n";
            //aspx += "\t<script type=\"text/javascript\">\n";
            //aspx += Script;
            //aspx += "\n</script>\n";
            aspx += "</asp:Content>\n";
            aspx += "<asp:Content ID=\"menu\" ContentPlaceHolderID=\"cphMenu\" runat=\"Server\">\n";
            aspx += "</asp:Content>\n";
            aspx += "<asp:Content ID=\"master\" ContentPlaceHolderID=\"masterContent\" runat=\"Server\">\n";
            aspx += SheetAspx;
            aspx += "</asp:Content>";
            // aspx = string.Format(aspx, SheetCode, EngineCode, SheetAspx);
            return aspx;
        }

        /// <summary>
        /// 获取C#代码
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="SheetCode"></param>
        /// <returns></returns>
        public string GetCSharpCode(string EngineCode, string SheetCode)
        {
            // 保存为 cs 文件
            string cs = "using System;\r\n";
            cs += "using System.Collections;\r\n";
            cs += "using System.Configuration;\r\n";
            cs += "using System.Data;\r\n";
            cs += "using System.Web;\r\n";
            cs += "using System.Web.Security;\r\n";
            cs += "using System.Web.UI;\r\n";
            cs += "using System.Web.UI.HtmlControls;\r\n";
            cs += "using System.Web.UI.WebControls;\r\n";
            cs += "using System.Web.UI.WebControls.WebParts;\r\n";
            cs += "\r\n";
            cs += "namespace OThinker.H3.Portal.Sheets." + EngineCode + "\r\n";
            cs += "{\r\n";
            cs += "    public partial class " + SheetCode + " : OThinker.H3.Controllers.MvcPage\r\n";
            cs += "    {\r\n";
            cs += "        protected void Page_Load(object sender, EventArgs e)\r\n";
            cs += "        {\r\n";
            cs += "        }\r\n";
            cs += "\r\n";
            cs += "        /// <summary>\r\n";
            cs += "        /// 加载引擎数据到表单\r\n";
            cs += "        /// </summary>\r\n";
            cs += "        public override void LoadDataFields()\r\n";
            cs += "        {\r\n";
            cs += "            base.LoadDataFields();\r\n";
            cs += "        }\r\n";
            cs += "\r\n";
            cs += "        /// <summary>\r\n";
            cs += "        /// 保存表单数据到引擎中\r\n";
            cs += "        /// </summary>\r\n";
            cs += "        /// <param name=\"Args\"></param>\r\n";
            cs += "        public override void SaveDataFields(OThinker.H3.WorkSheet.SheetSubmitEventArgs Args)\r\n";
            cs += "        {\r\n";
            cs += "            base.SaveDataFields(Args);\r\n";
            cs += "        }\r\n";
            cs += "    }\r\n";
            cs += "}\r\n";

            return cs;
        }

        #endregion

        #region  获取附件打开的链接地址
        /// <summary>
        /// 获取附件打开的链接地址
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="AttachmentID"></param>
        /// <param name="openType"></param>
        /// <returns></returns>
        public string GetReadAttachmentUrl(string SchemaCode, string AttachmentID, string openType)
        {
            return System.IO.Path.Combine(PortalRoot, "/ReadAttachment/Read").Replace('\\', '/') + "?" + Param_AttachmentID + "=" + AttachmentID + "&" + Param_SchemaCode + "=" + HttpUtility.UrlEncode(SchemaCode) + "&OpenMethod=" + openType;
        }

        /// <summary>
        /// 获取附件打开的链接地址
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="AttachmentID"></param>
        /// <returns></returns>
        public string GetReadAttachmentUrl(string SchemaCode, string AttachmentID)
        {
            return GetReadAttachmentUrl(SchemaCode, AttachmentID, "1");
        }


        #endregion

        #region 用户信息处理-------------
        /// <summary>
        /// 默认图像
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public Organization.User UpdateUserImageUrl(Organization.User user)
        {
            if (user != null)
            {
                user.ImageUrl = GetUserImageUrl(user);
            }
            return user;
        }

        /// <summary>
        /// 上级主管名称
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetUserManagerName(Organization.User user)
        {
            string ManagerName = "";
            if (user != null && !string.IsNullOrWhiteSpace(user.ManagerID))
            {
                Organization.Unit unit = this.Engine.Organization.GetUnit(user.ManagerID);
                if (unit != null)
                    ManagerName = unit.Name;
            }

            return ManagerName;
        }
        /// <summary>
        /// 所属组织
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public string GetUserParentName(Organization.User user)
        {
            string ParentName = "";
            if (user != null && !string.IsNullOrWhiteSpace(user.ParentID))
            {
                Organization.Unit unit = this.Engine.Organization.GetUnit(user.ParentID);
                if (unit != null)
                    ParentName = unit.Name;
            }
            return ParentName;
        }

        /// <summary>
        /// 获取用户对应的所有角色   add zhangamei
        /// </summary>
        /// <param name="userID"></param>
        /// <returns></returns>
        public string GetUserRole(string userID)
        {

            List<string> lstPosts = this.Engine.Organization.GetParents(userID, UnitType.Post, true, State.Active);
            string roleCodes = "";
            if (lstPosts != null && lstPosts.Count > 0)
            {
                foreach (string postid in lstPosts)
                {
                    OThinker.Organization.OrgPost post = this.Engine.Organization.GetUnit(postid) as OThinker.Organization.OrgPost;
                    foreach (OrgStaff staff in post.ChildList)
                    {
                        if (staff.UserID == userID)
                        {
                            roleCodes += post.Code + ";";

                        }
                    }
                }
            }
            return roleCodes;
        }

        /// <summary>
        /// 获取用户头像图片
        /// </summary>
        /// <param name="unit"></param>
        /// <returns></returns>
        public string GetUserImageUrl(Organization.User User)
        {
            string imagePath = "";
            if (User != null)
            {
                //imagePath = OThinker.H3.Controllers.UserValidator.GetUserImagePath(this.Engine,
                //          User, Server.MapPath(this.PortalRoot + @"/TempImages/")).Replace("//", "/");
                imagePath = OThinker.H3.Controllers.UserValidator.GetUserImagePath(this.Engine, User,
                    System.Web.HttpContext.Current.Server.MapPath(this.PortalRoot + @"/TempImages/")).Replace("//", "/");
                if (imagePath != "")
                {
                    imagePath = this.PortalRoot + "/TempImages/" + imagePath + "?T=" + DateTime.Now.ToString("yyyyMMddhhmmss");
                }
                else
                {
                    if (User.Gender == Organization.UserGender.Male)
                    {
                        imagePath = this.PortalRoot + "/img/TempImages/usermale.jpg";
                    }
                    else if (User.Gender == Organization.UserGender.Female)
                    {
                        imagePath = this.PortalRoot + "/img/TempImages/userfemale.jpg";
                    }
                    else
                    {
                        imagePath = this.PortalRoot + "/img/user.jpg";
                    }
                }
            }
            return imagePath;
        }

        public string GetUserImageUrlAllowEmpty(Organization.User User)
        {
            string imagePath = "";
            if (User != null)
            {
                //imagePath = OThinker.H3.Controllers.UserValidator.GetUserImagePath(this.Engine,
                //          User, Server.MapPath(this.PortalRoot + @"/TempImages/")).Replace("//", "/");
                imagePath = OThinker.H3.Controllers.UserValidator.GetUserImagePath(this.Engine, User,
                    System.Web.HttpContext.Current.Server.MapPath(this.PortalRoot + @"/TempImages/")).Replace("//", "/");
                if (imagePath != "")
                {
                    imagePath = this.PortalRoot + "/TempImages/" + imagePath + "?T=" + DateTime.Now.ToString("yyyyMMddhhmmss");
                }
            }
            return imagePath;
        }
        #endregion

        #region 组织权限验证

        /// <summary>
        /// 获取是否有组织编辑，查看权限
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        public OrgAclViewModel GetUnitAcl(string UnitID)
        {
            OrgAclViewModel model = new OrgAclViewModel();

            model.View = this.UserValidator.ValidateOrgView(UnitID);
            model.Edit = this.UserValidator.ValidateOrgEdit(UnitID);

            return model;
        }
        #endregion

        #region 编码检测,只能以字母开头

        public ActionResult VlidateCode(string Code)
        {
            ActionResult result = new ActionResult(true, "");

            // 数据项必须以字母开始，不让创建到数据库表字段时报错
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
            if (!regex.Match(Code).Success)
            {
                result.Success = false;
                result.Message = "msgGlobalString.InvalidCode";
            }

            return result;
        }

        #endregion

        #region 流程文件夹权限设置
        /// <summary>
        /// 添加、保存流程文件夹权限设置
        /// </summary>
        /// <param name="model"></param>
        public void SaveWorkflowFloderAclBase(WorkflowFolderAclViewModel model)
        {
            Acl.FunctionAcl FunctionAcl = new Acl.FunctionAcl();
            if (string.IsNullOrEmpty(model.ObjectID))
            {
                FunctionAcl = new Acl.FunctionAcl();
                FunctionAcl.FunctionCode = model.WorkflowFolderCode;
                FunctionAcl.UserID = model.UserID;
                FunctionAcl.CreatedTime = System.DateTime.Now;
                FunctionAcl.ParentObjectID = "";
            }
            else
            {
                FunctionAcl = this.Engine.FunctionAclManager.GetAcl(model.ObjectID);
            }
            FunctionAcl.Run = model.Run;

            if (string.IsNullOrEmpty(model.ObjectID))
                this.Engine.FunctionAclManager.Add(FunctionAcl);
            else
                this.Engine.FunctionAclManager.Update(FunctionAcl);
        }

        #endregion

        #region 移动端打开表单验证
        /// <summary>
        /// 移动端打开表单验证
        /// </summary>
        /// <param name="code"></param>
        /// <param name="mobileToken"></param>
        /// <returns></returns>
        public bool SSOopenSheet(string LoginName, string MobileToken)
        {
            OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(LoginName) as OThinker.Organization.User;
            bool result = user == null ? false : user.ValidateMobileToken(MobileToken);
            if (result)
            {
                UserValidator userValidator = null;
                var tempImages = Path.Combine(System.Web.HttpContext.Current.Server.MapPath("."), "TempImages");
                userValidator = new UserValidator(this.Engine, user.ObjectID, tempImages, null);
                Session[Sessions.GetUserValidator()] = userValidator;
            }
            return result;
        }
        #endregion

        #region 用户查询流程、任务权限验证
        /// <summary>
        /// 用户查询流程、任务权限验证
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="Participant"></param>
        /// <returns></returns>
        public ActionResult GetQueryAcl(ref string[] WorkflowCodeArray, ref string[] ParticipantArray)
        {
            ActionResult QueryAclResult = new ActionResult(true, "");
            if (this.UserValidator.ValidateAdministrator())
            {
                return QueryAclResult;
            }

            if (WorkflowCodeArray == null)
            {
                if (ParticipantArray != null)
                {
                    List<string> ParticipantList = ParticipantArray.ToList();

                    for (int i = 0; i < ParticipantArray.Length; i++)
                    {
                        if (ParticipantArray[i] == this.UserValidator.User.ObjectID)
                        { }
                        else if (this.UserValidator.ValidateOrgView(ParticipantArray[i]))
                        { }
                        else
                        {
                            ParticipantList.Remove(ParticipantArray[i]);
                            QueryAclResult.Success = false;
                            QueryAclResult.Message = "QueryInstanceByProperty_NotEnoughAuth1";
                        }
                    }
                    ParticipantArray = ParticipantList.ToArray();
                }
                else
                {
                    if (!this.UserValidator.ValidateOrgView(null))
                    {
                        QueryAclResult.Success = false;
                        QueryAclResult.Message = "QueryInstanceByProperty_NotEnoughAuth3";
                    }
                }
            }
            else
            {
                List<string> ParticipantList = ParticipantArray == null ? new List<string>() : ParticipantArray.ToList();
                List<string> WorkflowCodeList = WorkflowCodeArray.ToList();
                for (int j = 0; j < WorkflowCodeArray.Length; j++)
                {
                    if (ParticipantArray != null)
                    {
                        for (int i = 0; i < ParticipantArray.Length; i++)
                        {
                            if (!this.UserValidator.ValidateWFInsView(WorkflowCodeArray[j], ParticipantArray[i]))
                            {
                                ParticipantList.Remove(ParticipantArray[i]);
                                WorkflowCodeList.Remove(WorkflowCodeArray[j]);
                                QueryAclResult.Success = false;
                                QueryAclResult.Message = "QueryInstanceByProperty_NotEnoughAuth2";

                            }
                        }
                    }
                    else
                    {
                        if (!this.UserValidator.ValidateWFInsView(WorkflowCodeArray[j], null))
                        {
                            WorkflowCodeList.Remove(WorkflowCodeArray[j]);
                            QueryAclResult.Success = false;
                            QueryAclResult.Message = "QueryInstanceByProperty_NotEnoughAuth3";
                        }
                    }
                }
                ParticipantArray = ParticipantList.ToArray();
                WorkflowCodeArray = WorkflowCodeList.ToArray();
            }
            return QueryAclResult;
        }

        /// <summary>
        /// 监听方法执行时间
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public JsonResult ExecutionActionWithLog(string methodName, Func<JsonResult> fun)
        {
            Stopwatch w = new Stopwatch();
            w.Start();
            JsonResult res = fun();
            w.Stop();
            this.Engine.LogWriter.Write(string.Format("执行Controller方法{0}耗时{1}毫秒", methodName, w.ElapsedMilliseconds));
            return res;
        }

        #endregion
        // End Class
        //qiancheng
        //批量获取字段的显示名称
        public Dictionary<string, string> GetPropertiesName(string[] schemaCodes, string[] propertiesCode)
        {
            if (schemaCodes == null || propertiesCode == null || schemaCodes.Length != propertiesCode.Length) return null;
            Dictionary<string, BizObjectSchema> schemas = new Dictionary<string, BizObjectSchema>();
            Dictionary<string, string> result = new Dictionary<string, string>();
            for (int i = 0; i < schemaCodes.Length; i++)
            {
                string schemaCode = schemaCodes[i];
                string propertyCode = propertiesCode[i];
                BizObjectSchema schema = null;
                if (schemas.ContainsKey(schemaCode))
                {
                    schema = schemas[schemaCode];
                }
                else
                {
                    schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                    schemas.Add(schemaCode, schema);
                }
                if (schema == null)
                {
                    continue;
                }
                if (propertyCode.IndexOf("___") != -1)
                {
                    propertyCode = propertyCode.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                PropertySchema property = schema.GetProperty(propertyCode);
                if (property != null)
                {
                    result.Add(schemaCode + property, property.DisplayName);
                }
            }
            return result;
        }


        /// <summary>
        /// 获取指定用户当前父部门
        /// </summary>
        /// <param name="userId">用户</param>
        /// <returns>父部门</returns>
        public Dictionary<string, string> GetDirectoryUnits(string userId, UserValidator userValidator = null)
        {

            // 添加直接所属OU
            OThinker.Organization.User OriginatorUser = this.Engine.Organization.GetUnit(userId) as OThinker.Organization.User;

            //发起环节如果已经选择了虚拟用户部门，需要先找到实体用户
            if (OriginatorUser.IsVirtualUser)
            {
                OriginatorUser = this.Engine.Organization.GetUnit(OriginatorUser.RelationUserID) as OThinker.Organization.User;
            }

            //读取用户关联OU和当前部门OU
            Dictionary<string, string> dicUserOUs = new Dictionary<string, string>();
            //添加本部
            dicUserOUs.Add(OriginatorUser.ParentID, this.Engine.Organization.GetUnit(OriginatorUser.ParentID).Name);

            var UserValidator = userValidator ?? this.UserValidator;
            //添加关联部门
            if (UserValidator.RelationUsers.Count > 0)
            {
                foreach (OThinker.Organization.User usere in UserValidator.RelationUsers)
                {
                    OThinker.Organization.Unit unit = this.Engine.Organization.GetUnit(usere.ParentID);
                    if (!dicUserOUs.ContainsKey(unit.ObjectID))
                    {
                        dicUserOUs.Add(unit.ObjectID, unit.Name);
                    }
                }
            }

            return dicUserOUs;
        }

        //qiancheng
        //获取schema显示名称
        public Dictionary<string, string> GetSchemasDisplayName(string[] schemaCodes)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            if (schemaCodes == null)
                return null;
            foreach (string q in schemaCodes)
            {
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(q);
                if (schema != null)
                {
                    result.Add(q, schema.DisplayName);
                }
                else
                {
                    if (q.IndexOf("___") != -1)
                    {
                        string[] sch = q.Split(new string[] { "___" }, StringSplitOptions.RemoveEmptyEntries);
                        BizObjectSchema schemas = this.Engine.BizObjectManager.GetPublishedSchema(sch[0]);
                        if (schemas != null)
                        {
                            foreach (var sh in schemas.Fields)
                            {
                                if (sh.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray && sh.ChildSchemaCode == sch[1])
                                {
                                    result.Add(q, sh.DisplayName);
                                }
                            }
                        }

                    }
                }
            }
            return result;
        }
        //qiancheng 构造一个用户信息，用于报表显示数据
        public class UserInformation
        {

            public string UserID { get; set; }
            public string UserCode { get; set; }
            public string UserDisplayName { get; set; }
            public string UserParentOUID { get; set; }
            public string UserParentOUCode { get; set; }
            public string UserParentOUDisplayName { get; set; }
            public string UserCompanyID { get; set; }
            public string UserCompanyCode { get; set; }
            public string UserCompanyDisplayName { get; set; }
        }

        //qiancheng json序列化对象

        private JavaScriptSerializer _JsSerializer = null;
        protected JavaScriptSerializer JsSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new JavaScriptSerializer();
                }
                return _JsSerializer;
            }
        }
    }

    public class ActionXSSFillters : ActionFilterAttribute, IActionFilter
    {

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //执行action后执行这个方法 比如做操作日志
        }
        /// <summary>
        /// OnActionExecuting
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            //获取参数集合
            var ps = context.ActionDescriptor.GetParameters();
            //遍历参数集合
            foreach (var p in ps)
            {
                //当参数是str
                if (p.ParameterType.Equals(typeof(string)))
                {
                    context.ActionParameters[p.ParameterName] = XSSHelper.XssFilter(context.ActionParameters[p.ParameterName].ToString());
                }
                else if (p.ParameterType.IsClass)//当参数是一个实体
                {
                    PostModelFieldFilter(p.ParameterType, context.ActionParameters[p.ParameterName]);
                }
            }
            
        }

        /// <summary>
        /// 遍历实体的字符串属性
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        private object PostModelFieldFilter(Type type, object obj)
        {
            if (obj != null)
            {
                foreach (var item in type.GetProperties())
                {
                    if (item.GetValue(obj) != null)
                    {
                        //当参数是str
                        if (item.PropertyType.Equals(typeof(string)))
                        {
                            string value = item.GetValue(obj).ToString();
                            item.SetValue(obj, XSSHelper.XssFilter(value));
                        }
                        else if (item.PropertyType.IsClass)//当参数是一个实体
                        {
                            item.SetValue(obj, PostModelFieldFilter(item.PropertyType, item.GetValue(obj)));
                        }
                    }

                }
            }
            return obj;
        }
    }
    public static class XSSHelper
    {
        /// <summary>
        /// XSS过滤
        /// </summary>
        /// <param name="html">html代码</param>
        /// <returns>过滤结果</returns>
        public static string XssFilter(string html)
        {
            string str = HtmlFilter(html);
            return str;
        }

        /// <summary>
        /// 过滤HTML标记
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string HtmlFilter(string Htmlstring)
        {

            

            //<script>(.*?)</script>
            //Regex.Replace(Htmlstring, "<script>(.*?)</script>", String.Empty);
                
            //return result;
            // 
            // var pattern = new Regex("[%--`~!@#$^&*()=|{}':;',\\[\\].<>/?~！@#￥……&*（）——|{}【】‘；：”“'。，、？]");
            string result = Regex.Replace(Htmlstring,"[%`~!#$^*()|{}':;',\\[\\].<>~！@#￥……*（)|{}【】‘；：”“'。，、？]", String.Empty);
            return result;
        }
    }

}
