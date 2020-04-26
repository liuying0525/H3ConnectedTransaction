using OThinker.H3.Sheet;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web.Configuration;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 站点全局变量
    /// </summary>
    public class AppConfig
    {
        #region Web路径
        /// <summary>
        /// 获取 web.config 中配置的 Portal 根目录路径
        /// </summary>
        public const string AppSettingName_PortalRoot = "PortalRoot";
        /// <summary>
        /// 获取Portal站点的跟目录路径
        /// </summary>
        public static string PortalRoot
        {
            get
            {
                string root = System.Configuration.ConfigurationManager.AppSettings[AppSettingName_PortalRoot];
                if (string.IsNullOrEmpty(root))
                {
                    return "/Portal";
                }
                else
                {
                    //if (root.Length > 1 && (root.EndsWith("/") || root.EndsWith("\\")))
                    //{
                    //    root = root.Substring(0, root.Length - 1);
                    //}
                    if (root.EndsWith("/") || root.EndsWith("\\"))
                    {
                        root = root.Substring(0, root.Length - 1);
                    }
                    return root;
                }
            }
        }

        /// <summary>
        /// 提供给构造Url使用的链接
        /// </summary>
        public static string PortalRootForUrl
        {
            get
            {
                if (PortalRoot == string.Empty) return "/";
                return PortalRoot;
            }
        }

        /// <summary>
        /// 站点的根目录
        /// </summary>
        public static string PortalResRoot
        {
            get
            {
                return System.IO.Path.Combine(PortalRootForUrl, "WFRes").Replace('\\', '/');
            }
        }

        /// <summary>
        /// 不以'/'结尾的形式，比如"/Portal"，或者""表示"/"
        /// </summary>
        public static string PortalFCKEditorRoot
        {
            get
            {
                return PortalResRoot;
            }
        }

        /// <summary>
        /// 不以'/'结尾的形式，比如"/Portal"，或者""表示"/"
        /// </summary>
        public static string PortalImageRoot
        {
            get
            {
                return System.IO.Path.Combine(PortalResRoot, "Images").Replace('\\', '/');
            }
        }

        /// <summary>
        /// 不以'/'结尾的形式，比如"/Portal"，或者""表示"/"
        /// </summary>
        public static string PortalScriptRoot
        {
            get
            {
                return System.IO.Path.Combine(PortalResRoot, "Scripts").Replace('\\', '/');
            }
        }

        /// <summary>
        /// 不以'/'结尾的形式，比如"/Portal"，或者""表示"/"
        /// </summary>
        public static string PortalCssRoot
        {
            get
            {
                return System.IO.Path.Combine(PortalResRoot, "Css").Replace('\\', '/');
            }
        }

        /// <summary>
        /// 获取附件上传URL
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="DivID"></param>
        /// <param name="RemoveTextBoxID"></param>
        /// <param name="AddTextBoxID"></param>
        /// <param name="OpenType"></param>
        /// <returns></returns>
        public static string GetUploadAttachmentUrl(
            string SchemaCode,
            string DivID,
            string RemoveTextBoxID,
            string AddTextBoxID,
            AttachmentOpenMethod OpenType)
        {
            string basePage = "UploadAttachment.aspx";
            return System.IO.Path.Combine(PortalRootForUrl, basePage).Replace('\\', '/') + "?" +
                        "SchemaCode=" + System.Web.HttpUtility.UrlEncode(SchemaCode) + "&" +
                        "Div=" + DivID + "&" +
                        "RemoveTextBoxID=" + RemoveTextBoxID + "&" +
                        "AddTextBoxID=" + AddTextBoxID + "&" +
                        "OpenMethod=" + ((int)OpenType).ToString(); ;
        }

        /// <summary>
        /// 获得选择用户的URL
        /// </summary>
        /// <returns></returns>
        public static string GetSelectUserUrl()
        {
            return System.IO.Path.Combine(PortalRootForUrl, "SelectUser.aspx").Replace('\\', '/');
        }

        /// <summary>
        /// 获得选择用户的URL
        /// </summary>
        /// <returns></returns>
        public static string GetSelectUserListUrl()
        {
            return System.IO.Path.Combine(PortalRootForUrl, "SelectUserList.aspx").Replace('\\', '/');
        }

        /// <summary>
        /// 获得选择用户的图片的URL
        /// </summary>
        /// <returns></returns>
        public static string GetSelectUserImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "p_add.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 获得删除用户的图片的URL
        /// </summary>
        /// <returns></returns>
        public static string GetRemoveUserImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "p_del.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 获得删除用户的图片的URL
        /// </summary>
        /// <returns></returns>
        public static string GetClearUserImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "p_Clear.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 获得选中的图标
        /// </summary>
        /// <returns></returns>
        public static string GetCheckedImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "checked.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 或者非选中的图标
        /// </summary>
        /// <returns></returns>
        public static string GetUnCheckedImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "unchecked.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 获取历史痕迹查看页面URL
        /// </summary>
        /// <param name="WorkItemId"></param>
        /// <param name="ItemName"></param>
        /// <returns></returns>
        public static string GetDataTrackUrl(string WorkItemId, string ItemName)
        {
            return System.IO.Path.Combine(PortalRootForUrl, "InstanceDataTrack.aspx").Replace('\\', '/') + "?" +
                    "WorkItemID=" + WorkItemId + "&" +
                    "ItemName=" + System.Web.HttpUtility.UrlEncode(ItemName);
        }

        /// <summary>
        /// 获取历史记录痕迹查看图标
        /// </summary>
        /// <returns></returns>
        public static string GetDataTrackImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "DataTrack.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 获取待办任务的URL
        /// </summary>
        /// <param name="WorkItemID"></param>
        /// <param name="IsMobile"></param>
        /// <returns></returns>
        public static string GetWorkItemUrl(string WorkItemID, bool IsMobile)
        {
            //WorkItemSheets.html?WorkItemID=04419900-18e2-427c-9b0b-90b3cb0781a8
            return System.IO.Path.Combine(PortalRootForUrl, "WorkItemSheets.html").Replace('\\', '/') + "?" +
                "WorkItemID=" + WorkItemID + "&" +
                "Mode=Work" + "&" +
                "IsMobile=" + IsMobile;
        }

        /// <summary>
        /// 获取BizObject显示页面
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="BizObjectID"></param>
        /// <param name="SheetCode"></param>
        /// <param name="IsMobile"></param>
        /// <returns></returns>
        public static string GetBizObjectUrl(string SchemaCode, string BizObjectID, string SheetCode, bool IsMobile)
        {
            return System.IO.Path.Combine(PortalRootForUrl, "RunBizQuery/EditBizObject").Replace('\\', '/') + "?" +
                "SchemaCode=" + SchemaCode + "&" +
                "Mode=Work" + "&" +
                "BizObjectID=" + BizObjectID + "&" +
                "SheetCode=" + SheetCode + "&" +
                "IsMobile=" + IsMobile;
        }

        /// <summary>
        /// 获取BizObject显示页面
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="BizObjectID"></param>
        /// <param name="SheetCode"></param>
        /// <param name="IsMobile"></param>
        /// <returns></returns>
        public static string GetBizObjectUrl(SheetMode Mode, string SchemaCode, string BizObjectID, string SheetCode, bool IsMobile)
        {
            return System.IO.Path.Combine(PortalRootForUrl, "EditBizObject.aspx").Replace('\\', '/') + "?" +
                "SchemaCode=" + SchemaCode + "&" +
                "Mode=" + Mode.ToString() + "&" +
                "BizObjectID=" + BizObjectID + "&" +
                "SheetCode=" + SheetCode + "&" +
                "IsMobile=" + IsMobile;
        }

        /// <summary>
        /// 获取打印表单的URL地址
        /// </summary>
        /// <param name="Sheet"></param>
        /// <param name="WorkItemID"></param>
        /// <returns></returns>
        public static string GetPrintSheetUrl(BizSheet Sheet, string WorkItemID, string InstanceId)
        {
            if (!string.IsNullOrEmpty(Sheet.PrintModel))
            {
                return System.IO.Path.Combine(PortalRootForUrl, "SheetPrint.aspx").Replace('\\', '/') + "?" +
                    "InstanceId=" + InstanceId + "&" +
                    "WorkItemID=" + WorkItemID + "&" +
                    "Mode=Print" + "&" +
                    "SheetCode=" + Sheet.SheetCode;
            }
            else if (!string.IsNullOrEmpty(Sheet.PrintSheetAddress))
            {
                string url = string.Empty;
                if (Sheet.PrintSheetAddress.StartsWith("~/"))
                    url = Sheet.PrintSheetAddress;
                else
                {
                    url = System.IO.Path.Combine(PortalRootForUrl, Sheet.PrintSheetAddress).Replace('\\', '/');
                }
                return url + "?" +
                    "WorkItemID=" + WorkItemID + "&" +
                    "Mode=Print" + "&" +
                    "SheetCode=" + Sheet.SheetCode;
            }
            return string.Empty;
        }

        /// <summary>
        /// 获取流程信息的URL
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="WorkflowVersion"></param>
        /// <returns></returns>
        public static string GetWorkflowInfoUrl(string WorkflowCode, int WorkflowVersion)
        {
            return System.IO.Path.Combine(PortalRootForUrl, "WorkflowInfo.aspx").Replace('\\', '/') + "?" +
                "WorkflowCode=" + System.Web.HttpUtility.UrlEncode(WorkflowCode) + "&" +
                "WorkflowVersion=" + WorkflowVersion;
        }

        /// <summary>
        /// 获取查看附件的URL
        /// </summary>
        /// <param name="IsMobile"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="BizObjectId"></param>
        /// <param name="AttachmentID"></param>
        /// <param name="Method"></param>
        /// <returns></returns>
        public static string GetReadAttachmentUrl(bool IsMobile, string SchemaCode, string BizObjectId, string AttachmentID, AttachmentOpenMethod Method)
        {
            string url = System.IO.Path.Combine(PortalRootForUrl, "ReadAttachment/Read").Replace('\\', '/') + "?" +
                "BizObjectSchemaCode=" + System.Web.HttpUtility.UrlEncode(SchemaCode) +
                "&BizObjectID=" + BizObjectId +
                "&AttachmentID=" + AttachmentID +
                "&OpenMethod=" + (int)Method;
            if (IsMobile)
            {
                url += "&" + SheetEnviroment.Param_IsMobile + "=True";
            }
            return url;
        }


        /// <summary>
        /// 获取查看附件的URL
        /// </summary>
        /// <param name="IsMobile"></param>
        /// <param name="WorkflowCode"></param>
        /// <param name="BizObjectId"></param>
        /// <param name="AttachmentID"></param>
        /// <returns></returns>
        public static string GetReadAttachmentUrl(bool IsMobile, string WorkflowCode, string BizObjectId, string AttachmentID)
        {
            return GetReadAttachmentUrl(IsMobile, WorkflowCode, BizObjectId, AttachmentID, AttachmentOpenMethod.Download);
        }

        /// <summary>
        /// 获得删除附件的图片的URL
        /// </summary>
        /// <returns></returns>
        public static string GetDeleteAttachmentImageUrl()
        {
            return System.IO.Path.Combine(PortalImageRoot, "Remove.gif").Replace('\\', '/');
        }

        /// <summary>
        /// 插件文件夹目录
        /// </summary>
        public static string WebPartFolderPath
        {
            get
            {
                string webpartFolder = System.Configuration.ConfigurationManager.AppSettings["OT_Portal_WebPartFolder"];
                if (string.IsNullOrEmpty(webpartFolder))
                    webpartFolder = "~/WebParts";
                return webpartFolder.TrimEnd('/');
            }
        }


        /// <summary>
        /// 获取临时图片文件夹目录
        /// </summary>
        public static string TempImagesPath
        {
            get
            {
                return System.IO.Path.Combine(PortalRootForUrl, "TempImages");
            }
        }

        /// <summary>
        /// 获得日期控件脚本的地址
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        public static string GetCalendarScriptPath(System.Web.UI.Page Page)
        {
            string path = "~/WFRes/_Scripts/Calendar/WdatePicker.js";
            return Page.ResolveUrl(path);
        }

        /// <summary>
        /// 获得流程编码选择控件脚本地址
        /// </summary>
        /// <param name="Page"></param>
        /// <returns></returns>
        public static string GetWorkflowCodeScriptPath(System.Web.UI.Page Page)
        {
            string path = "~/WFRes/Scripts/WorkflowSelector.js";
            return Page.ResolveUrl(path);
            // return System.IO.Path.Combine(PortalRoot, path).Replace('\\', '/');
        }

        #region 移动办公方法

        /// <summary>
        /// 获得移动办公待办列表页面地址
        /// </summary>
        public static string GetMobileUnfinishedWorkItemUrl()
        {
            return PortalRoot + "/Mobile/index.html?reloadData=true#/tab/home";
        }

        /// <summary>
        /// 获得移动办公已办列表页面地址
        /// </summary>
        public static string GetMobileFinishedWorkItemUrl()
        {
            return PortalRoot + "/Mobile/index.html?reloadData=true#/tab/home";
        }

        /// <summary>
        /// 获得移动办公发起流程列表页面地址
        /// </summary>
        public static string GetMobileOriginateInstancesUrl()
        {
            return PortalRoot + "/Mobile/index.html?reloadData=true#/tab/startworkflow";
        }

        /// <summary>
        /// 移动办公提交任务
        /// </summary>
        /// <param name="WorkItemID"></param>
        /// <param name="Approval"></param>
        /// <param name="Comment"></param>
        /// <param name="ActionName"></param>
        /// <param name="ActionEventType"></param>
        /// <param name="ActionButtonType"></param>
        /// <param name="Originate"></param>
        /// <param name="DestActivityCode"></param>
        /// <returns></returns>
        public static string GetFinishMobileWorkItemUrl(
            string WorkItemID,
            OThinker.Data.BoolMatchValue Approval,
            string Comment,
            string ActionName,
            int ActionEventType,
            int ActionButtonType,
            OThinker.Data.BoolMatchValue Originate,
            string DestActivityCode)
        {
            string formattedComment = null;
            if (Comment == null)
            {
                formattedComment = null;
            }
            else if (Comment.Length <= 150)
            {
                formattedComment = Comment;
            }
            else
            {
                formattedComment = Comment.Substring(0, 150);
            }
            return PortalRoot + "/Mobile/FinishMobileWorkItem.aspx?" +
                    "WorkItemID=" + WorkItemID + "&" +
                    "Approval=" + Approval.ToString() + "&" +
                    "Comment=" + System.Web.HttpUtility.UrlEncode(formattedComment) + "&" +
                    "ActionName=" + System.Web.HttpUtility.UrlEncode(ActionName) + "&" +
                    "ActionEventType=" + ActionEventType + "&" +
                    "ActionButtonType=" + ActionButtonType + "&" +
                    "Originate=" + Originate.ToString() + "&" +
                    "DestActivityCode=" + DestActivityCode;
            // REVIEW, 本来要返回的URL中，需要包含FromDest的，但是，我把下面的代码注释掉了，需要确认是否需要这个FromDesk参数
            //if (HttpContext.Current.Request["FromDesk"] != null)
            //    url += "&FromDesk=true";
        }

        /// <summary>
        /// 移动办公任务驳回
        /// </summary>
        /// <param name="WorkItemID"></param>
        /// <param name="Approval"></param>
        /// <param name="Comment"></param>
        /// <param name="RejectTo"></param>
        public static string GetReturnMobileWorkItemUrl(
            string WorkItemID,
            OThinker.Data.BoolMatchValue Approval,
            string Comment,
            string RejectTo)
        {
            string formattedComment = null;
            if (Comment == null)
            {
                formattedComment = null;
            }
            else if (Comment.Length <= 150)
            {
                formattedComment = Comment;
            }
            else
            {
                formattedComment = Comment.Substring(0, 150);
            }
            return PortalRoot + "/Mobile/ReturnMobileWorkItem.aspx?" +
                    "WorkItemID=" + WorkItemID + "&" +
                    "Approval=" + Approval.ToString() + "&" +
                    "Comment=" + System.Web.HttpUtility.UrlEncode(formattedComment) + "&" +
                    "RejectTo=" + System.Web.HttpUtility.UrlEncode(RejectTo);
        }

        #endregion

        #endregion

        /// <summary>
        /// 取得当前配置文件上设置的登录验证模式
        /// </summary>
        /// <returns></returns>
        public static AuthenticationMode AuthenticationMode
        {
            get
            {
                try
                {
                    AuthenticationSection authenticationSection = (AuthenticationSection)System.Configuration.ConfigurationManager.GetSection("system.web/authentication");
                    return authenticationSection.Mode;
                }
                catch
                {
                    return AuthenticationMode.None;
                }
            }
        }

        private static object MonoConnectionLockObject = new object();
        private static OThinker.H3.Connection MonoConnection;
        /// <summary>
        /// 引擎服务
        /// </summary>
        internal static OThinker.H3.IEngine MonoEngine
        {
            get
            {
                lock (MonoConnectionLockObject)
                {
                    if (MonoConnection == null || MonoConnection.Engine == null)
                    {
                        OThinker.H3.Connection c = new Connection();
                        string connectionString = System.Configuration.ConfigurationManager.AppSettings[OThinker.H3.Connection.ConnectionString_KeyName];
                        try
                        {
                            Clusterware.ConnectionResult result = c.Open(connectionString);
                            if (result != Clusterware.ConnectionResult.Success)
                            {
                                throw new Exception("引擎服务连接错误->" + result.ToString());
                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                        MonoConnection = c;
                    }
                }
                return MonoConnection.Engine;
            }
        }


        private static string _ConnectionString = string.Empty;
        /// <summary>
        /// 获取引擎连接字符串
        /// </summary>
        private static string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_ConnectionString))
                {
                    _ConnectionString = System.Configuration.ConfigurationManager.AppSettings[OThinker.H3.Connection.ConnectionString_KeyName];
                }
                return _ConnectionString;
            }
            set
            {
                _ConnectionString = value;
            }
        }

        /// <summary>
        /// 重置引擎连接
        /// </summary>
        /// <param name="connectionString"></param>
        public static void ResetEngine(string connectionString)
        {
            lock (MonoConnectionLockObject)
            {
                ConnectionString = connectionString;
                MonoConnection = null;
            }
        }

        private static Dictionary<string, object> _MonoPortalSettings;
        /// <summary>
        /// 独占式Portal，默认会设置好引擎连接，这样就能一次性获得Portal设置，然后提供给所有的用户连接使用
        /// </summary>
        internal static Dictionary<string, object> MonoPortalSettings
        {
            get
            {
                if (_MonoPortalSettings == null)
                {
                    _MonoPortalSettings = AppUtility.Engine.SettingManager.GetSheetSettings();
                }
                return _MonoPortalSettings;
            }
        }

        private static OThinker.H3.ConnectionStringParser _ConnectionStringParser;
        public static OThinker.H3.ConnectionStringParser ConnectionStringParser
        {
            get
            {
                if (_ConnectionStringParser == null)
                {
                    string connectionString = System.Configuration.ConfigurationManager.AppSettings[OThinker.H3.Connection.ConnectionString_KeyName];
                    _ConnectionStringParser = new ConnectionStringParser(connectionString);
                }
                return _ConnectionStringParser;
            }
        }

        public static OThinker.H3.ConnectionStringParser.ConnectionMode ConnectionMode
        {
            get
            {
                return ConnectionStringParser.Mode;
            }
        }
    }
}
