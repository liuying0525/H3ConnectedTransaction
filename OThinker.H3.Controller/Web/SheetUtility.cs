using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Configuration;
using System.Web.UI;
using OThinker.H3.DataModel;
using System.Reflection;
using System.IO;
using System.Threading;
using OThinker.H3.Data;
using System.Web.UI.HtmlControls;
using System.Text.RegularExpressions;
using System.Web;
using OThinker.H3.WorkflowTemplate;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 表单相关功能的辅助类
    /// </summary>
    public class SheetUtility
    {
        /// <summary>
        /// DataField 的属性名称
        /// </summary>
        public const string Attr_DataField = "DataField";
        /// <summary>
        /// DisplayRule 的属性名称
        /// </summary>
        public const string Attr_DisplayRule = "DisplayRule";
        /// <summary>
        /// ComputeRule 的属性名称
        /// </summary>
        public const string Attr_ComputationRule = "ComputationRule";
        /// <summary>
        /// FormatRule 的属性名称
        /// </summary>
        public const string Attr_FormatRule = "FormatRule";
        /// <summary>
        /// VaildRule 的属性名称
        /// </summary>
        public const string Attr_VaildationRule = "VaildationRule";

        public static string ChineseString = "zh-CN";
        public static string EnglishString = "en-US";

        /// <summary>
        /// 获得打开窗口的脚本
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="IsUrlVar">Url参数是否是变量，如果是变量的话，则打开页面的脚本中不需要加''</param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GetOpenWindowScript(string Url, bool IsUrlVar, int width, int height)
        {
            return
               "childWindowProperties='toolbar=no,location=no,directories=no,status=no,menubar=no,scrollbars=yes,resizable=yes,width=" + width + ",height=" + height + ",left=' + (window.screen.width - " + width + ")/2 +',top='+ (window.screen.height - " + height + ")/2;" +
               "window.open(" + (IsUrlVar ? Url : ("'" + Url + "'")) + ", '', childWindowProperties);";
        }

        /// <summary>
        /// 获得打开窗口的脚本
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="IsUrlVar">Url参数是否是变量，如果是变量的话，则打开页面的脚本中不需要加''</param>
        /// <returns></returns>
        public static string GetOpenWindowScript(string Url, bool IsUrlVar)
        {
            return GetOpenWindowScript(Url, IsUrlVar, 600, 510);
        }

        /// <summary>
        /// 创建一个HTML，该HTML是一个链接，点击链接的时候会弹出一个页面
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="Url"></param>
        /// <param name="IsUrlVar"></param>
        /// <returns></returns>
        public static string GetLinkHtml(string Text, string Url, bool IsUrlVar)
        {
            return "<a onclick=\"" + GetOpenWindowScript(Url, false) + "\"" + "style=\"cursor: pointer;\">" + Text + "</a>";
        }

        /// <summary>
        /// 创建bootstrap modal窗口
        /// </summary>
        /// <param name="text">窗口Title</param>
        /// <param name="saveText">按钮文字</param>
        /// <param name="url">iframeurl</param>
        /// <returns></returns>
        public static string GetPopupLink(string text, string saveText, string url)
        {
            string tick = DateTime.Now.Ticks.ToString();
            string popupDivId = "div_" + tick;
            string iframeName = "iframe_" + tick;
            string popupDiv = "<div id='" + popupDivId + "' class='modal fade' style='z-index:10000; display:none;' tabindex='-1' role='dialog' aria-hidden='true'><div class='modal-dialog'><div class='modal-content'><div class='modal-header'><button type='button' class='close' data-dismiss='modal'><span aria-hidden='true'>&times;</span></button><h4 class='modal-title'>" + text + "</h4></div>";
            popupDiv += "<div class='modal-body' style='padding:5px;'>";
            popupDiv += "<iframe src='" + url + "' scrolling='no' frameborder='0' width='100%' height='470px' id='" + iframeName + "' name='" + iframeName + "'></iframe></div><div class='modal-footer'><button type='button' class='btn btn-primary' onclick='(function(){$(\"#" + iframeName + "\").contents().find(\"#inputBtnSave\").click();window.location.reload();})()'>" + saveText + "</button></div></div></div></div>";
            //弹窗按钮
            var popupLink = "<a href='#' data-toggle='modal' data-target='#" + popupDivId + "'>"
                + text + "</a>";
            return popupLink + popupDiv;
        }

        /// <summary>
        /// 如果textObj设置为{zh:xx,en:xx}，则根据语言设置返回对应text
        /// 否则原样返回
        /// </summary>
        /// <param name="textObj"></param>
        /// <returns></returns>
        public static object GetGlobalString(object textObj)
        {
            try
            {
                if (textObj == null)
                {
                    return null;
                }
                Regex regex = new Regex(@"\{.+\}");
                if (regex.IsMatch(textObj.ToString()))
                {
                    string text = regex.Match(textObj.ToString()).Value;
                    if (!string.IsNullOrEmpty(text))
                    {
                        text = text.Replace("{", "").Replace("}", "");
                        object lang = HttpContext.Current.Session[Sessions.GetLang()];
                        string[] textArray = text.Split(',');
                        foreach (string item in textArray)
                        {
                            if (lang.ToString().ToLower().Contains(item.Split(':')[0]))
                            {
                                string temp = regex.Replace(textObj.ToString(), "{0}");
                                textObj = string.Format(temp, item.Split(':')[1]);
                                break;
                            }
                        }
                    }
                }
            }
            catch
            {
            }
            return textObj;
        }

        /// <summary>
        /// 获取客户端的IP地址
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string GetClientIP(HttpRequest Request)
        {
            string loginip = string.Empty;

            if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
            {
                loginip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"] + string.Empty;
            }
            else
            {
                loginip = Request.ServerVariables["REMOTE_ADDR"] + string.Empty;
            }
            if (loginip == string.Empty) loginip = Request.UserHostAddress;

            return loginip;
        }

        public static string GetClientIP(HttpRequestBase Request)
        {
            string loginip = string.Empty;
            if (Request.ServerVariables["REMOTE_ADDR"] != null)
            {
                loginip = Request.ServerVariables["REMOTE_ADDR"].ToString();
            }
            else if (Request.ServerVariables["HTTP_VIA"] != null)
            {
                if (Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null)
                {
                    loginip = Request.ServerVariables["HTTP_X_FORWARDED_FOR"].ToString();
                }
                else
                {
                    loginip = Request.UserHostAddress;
                }
            }
            else
            {
                loginip = Request.UserHostAddress;
            }
            return loginip;
        }


        /// <summary>
        /// 获取是否是移动端浏览器访问
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static bool IsMobileDevice(HttpRequest Request)
        {
            bool isMobile = Request.Browser.IsMobileDevice;
            if (!isMobile)
            {
                string agent = HttpContext.Current.Request.UserAgent;
                // 排除Window 桌面系统 和 苹果桌面系统 
                if (agent != null && !agent.Contains("Windows NT") && !agent.Contains("Macintosh"))
                {
                    if (agent.ToLower().Contains("iphone") || agent.ToLower().Contains("android"))
                    {
                        isMobile = true;
                    }
                }
            }
            return isMobile;
        }
        public static bool IsMobileDevice(HttpRequestBase Request)
        {
            bool isMobile = Request.Browser.IsMobileDevice;
            if (!isMobile)
            {
                string agent = HttpContext.Current.Request.UserAgent;
                // 排除Window 桌面系统 和 苹果桌面系统 
                if (agent != null && !agent.Contains("Windows NT") && !agent.Contains("Macintosh"))
                {
                    if (agent.ToLower().Contains("iphone") || agent.ToLower().Contains("android"))
                    {
                        isMobile = true;
                    }
                }
            }
            return isMobile;
        }

        /// <summary>
        /// 获取是否移动端访问方式
        /// </summary>
        public static bool IsMobile
        {
            get
            {
                string strIsMobile = HttpContext.Current.Request.QueryString[SheetEnviroment.Param_IsMobile] + string.Empty;
                bool result = false;
                if (string.IsNullOrEmpty(strIsMobile) || !bool.TryParse(strIsMobile.ToLower(), out result))
                {
                    // 微信消息通知打开表单，获取 state 字段如果和 EngineCode 相同，表示是移动方式
                    string state = System.Web.HttpContext.Current.Request.QueryString["state"] + string.Empty;
                    if (state.ToLower() == UserValidatorFactory.CurrentUser.Engine.EngineConfig.Code.ToLower())
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
                return result;
            }
        }

        /// <summary>
        /// 获取是否是移动端浏览器访问
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string GetClientPlatform(HttpRequest Request)
        {
            string agent = HttpContext.Current.Request.UserAgent;
            //if (agent.Contains("Macintosh"))
            //{
            //    return "Macintosh";
            //}
            //else if (agent.ToLower().Contains("iphone"))
            //{
            //    return "iPhone";
            //}
            //else if (agent.ToLower().Contains("android"))
            //{
            //    return "Android";
            //}

            //return Request.Browser.Platform;
            if (agent != null && agent.Length > 200)
            {
                agent = agent.Substring(0, 199);
            }
            return agent;
        }
        public static string GetClientPlatform(HttpRequestBase Request)
        {
            string agent = HttpContext.Current.Request.UserAgent;
            //if (agent.Contains("Macintosh"))
            //{
            //    return "Macintosh";
            //}
            //else if (agent.ToLower().Contains("iphone"))
            //{
            //    return "iPhone";
            //}
            //else if (agent.ToLower().Contains("android"))
            //{
            //    return "Android";
            //}

            //return Request.Browser.Platform;
            if (agent != null && agent.Length > 200)
            {
                agent = agent.Substring(0, 199);
            }
            return agent;
        }

        /// <summary>
        /// 获取客户端浏览器类型和版本
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public static string GetClientBrowser(HttpRequest Request)
        {
            return Request.Browser.Browser + "/" + Request.Browser.Version;
        }
        public static string GetClientBrowser(HttpRequestBase Request)
        {
            return Request.Browser.Browser + "/" + Request.Browser.Version;
        }
        /// <summary>
        /// 根据参数的名称解析参数的值，如果是用户ID/登录名/所在部门，则返回当前用户的ID/登录名/所在部门；否则检查全局变量，如果是全局变量的话，则返回全局变量；否则返回参数值。
        /// 值得注意的是：该方法的性能并不好，有可能会需要访问引擎的接口。
        /// </summary>
        /// <param name="UserValidator"></param>
        /// <param name="ParamName"></param>
        /// <returns></returns>
        public static string GetSystemParamValue(UserValidator UserValidator, string ParamName)
        {
            if (ParamName == "{UserID}")
            {
                return UserValidator.UserID;
            }
            else if (ParamName == "{UserAlias}")
            {
                return UserValidator.UserCode;
            }
            else if (ParamName == "{Department}")
            {
                return UserValidator.Department;
            }
            else if (AppUtility.Engine.MetadataRepository.ExistPrimitiveItem(ParamName))
            {
                return AppUtility.Engine.MetadataRepository.GetPrimitiveItemValue(ParamName);
            }
            return ParamName;
        }

        #region 验证权限 --------------------------
        /// <summary>
        /// 验证权限
        /// </summary>
        /// <param name="UserValidator"></param>
        /// <param name="SheetDataType"></param>
        /// <param name="IsOriginateMode"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="BizObject"></param>
        /// <param name="SheetMode"></param>
        /// <param name="WorkflowCode"></param>
        /// <param name="WorkItem"></param>
        /// <param name="InstanceContext"></param>
        /// <returns></returns>
        public static bool ValidateAuthorization(
            UserValidator UserValidator,
            SheetDataType SheetDataType,
            bool IsOriginateMode,
            string SchemaCode,
            DataModel.BizObject BizObject,
            SheetMode SheetMode,
            string WorkflowCode,
            WorkItem.WorkItem WorkItem,
            WorkItem.CirculateItem CirculateItem,
            Instance.InstanceContext InstanceContext)
        {
            if (UserValidator.ValidateAdministrator())
            {
                return true;
            }
            // 业务对象表单
            else if (SheetDataType == SheetDataType.BizObject)
            {
                if (IsOriginateMode)
                {
                    return UserValidator.ValidateBizObjectAdd(SchemaCode, null, UserValidator.UserID);
                }
                else
                {
                    if (UserValidator.ValidateOrgAdmin(BizObject.OwnerId))
                        return true;
                    return UserValidator.ValidateBizObjectAdmin(SchemaCode, null, BizObject.OwnerId);
                }
            }
            else
            {
                switch (SheetMode)
                {
                    case SheetMode.Originate:
                        return UserValidator.ValidateCreateInstance(WorkflowCode);
                    case SheetMode.View:
                    case SheetMode.Print:
                        if (WorkItem != null &&
                            ValidateWorkItemAuth(WorkItem, UserValidator, UserValidator.Engine.AgencyManager))
                        {
                            return true;
                        }
                        else if (CirculateItem != null && ValidateCirculateItemAuth(CirculateItem, UserValidator))
                        {
                            return true;
                        }
                        else if (UserValidator.ValidateWFInsView(WorkflowCode, InstanceContext.Originator))
                        {
                            // 是否允许查看这个流程模板的所有实例
                            return true;
                        }
                        else
                        {
                            // 判定用户是否参与过流程实例
                            string[] workItems = UserValidator.Engine.Query.QueryWorkItems(
                                new string[] { InstanceContext.InstanceId },
                                new string[] { UserValidator.UserID },
                                DateTime.Now.AddYears(-100),
                                DateTime.Now.AddDays(1),
                                 H3.WorkItem.WorkItemState.Unspecified,
                                 OThinker.H3.WorkItem.WorkItem.NullWorkItemID);
                            if (workItems != null && workItems.Length > 0) return true;
                            return false;
                        }
                    case SheetMode.Work:
                        if (WorkItem != null)
                        {
                            return ValidateWorkItemAuth(WorkItem, UserValidator, UserValidator.Engine.AgencyManager);
                        }
                        else
                        {
                            return ValidateCirculateItemAuth(CirculateItem, UserValidator);
                        }

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 验证某个用户是否有访问WorkItem的权限
        /// </summary>
        /// <param name="WorkItem">工作项</param>
        /// <param name="UserValidator">用户ID</param>
        /// <param name="AgencyManager">委托关系</param>
        /// <returns>如果具备权限，那么返回true；否则返回false。</returns>
        public static bool ValidateWorkItemAuth(WorkItem.WorkItem WorkItem, UserValidator UserValidator, WorkItem.IAgencyManager AgencyManager)
        {
            if (UserValidator.ValidateAdministrator())
            {// 判定当前用户是否超级管理员
                return true;
            }
            else if (WorkItem.Participant == UserValidator.UserID ||
                WorkItem.Forwarder == UserValidator.UserID ||
                WorkItem.Delegant == UserValidator.UserID ||
                WorkItem.Finisher == UserValidator.UserID)
            {// 判定当前用户是否和当前工作任务相关
                return true;
            }
            else if (UserValidator.ValidateWFInsView(WorkItem.WorkflowCode, WorkItem.Participant))
            {// 判定当前用户是否可以查看指定流程模板的组织
                return true;
            }
            // 已经不允许设置委托发起
            //else if (
            //    WorkItem.Participant == WorkItem.Originator &&
            //    AgencyManager.CheckOriginateAgency(WorkItem.Participant, UserValidator.UserID, WorkItem.WorkflowCode))
            //{
            //    // 如果当前用户跟工作项的参与者是委托关系，也有权限
            //    return true;
            //}

            return false;
        }
        /// <summary>
        /// 验证某个用户是否有访问CirculateItem的权限
        /// </summary>
        /// <param name="CirculateItem"></param>
        /// <param name="UserValidator"></param>
        /// <returns></returns>
        public static bool ValidateCirculateItemAuth(WorkItem.CirculateItem CirculateItem, UserValidator UserValidator)
        {
            if (UserValidator.ValidateAdministrator())
            {// 判定当前用户是否超级管理员
                return true;
            }
            else if (CirculateItem.Participant == UserValidator.UserID ||
                CirculateItem.Delegant == UserValidator.UserID ||
                CirculateItem.Finisher == UserValidator.UserID)
            {// 判定当前用户是否和当前工作任务相关
                return true;
            }
            else if (UserValidator.ValidateWFInsView(CirculateItem.WorkflowCode, CirculateItem.Participant))
            {// 判定当前用户是否可以查看指定流程模板的组织
                return true;
            }
            return false;
        }


        #endregion

        #region 锁定操作 --------------------------
        ///// <summary>
        ///// 验证锁定。如果打开即需要锁定，那么尝试锁定，并返回尝试结果；否则，检查是否有被其他工作项锁定，如果有则返回false，否则返回true。
        ///// </summary>
        ///// <param name="Enviroment"></param>
        ///// <param name="Message"></param>
        ///// <returns></returns>
        //public static bool ValidateLockLevel(SheetEnviroment Enviroment, ref string Message)
        //{
        //    return ValidateLockLevel(Enviroment.Engine,
        //        Enviroment.UserValidator,
        //        Enviroment.WorkflowActivity,
        //        Enviroment.WorkItem,
        //        ref Message);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="UserValidator"></param>
        /// <param name="WorkflowActivity"></param>
        /// <param name="WorkItem"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool ValidateLockLevel(IEngine Engine, UserValidator UserValidator,
            Activity WorkflowActivity, WorkItem.WorkItem WorkItem, ref string Message)
        {
            WorkflowTemplate.ParticipativeActivity task = WorkflowActivity as WorkflowTemplate.ParticipativeActivity;
            if (task != null && task.LockPolicy == OThinker.H3.WorkflowTemplate.LockPolicy.Open)
            {
                return TryLock(Engine, UserValidator, WorkflowActivity, WorkItem, ref Message);
            }
            else if (!WorkItem.IsLocked)
            {
                // 没有被其他任何人锁定
                return true;
            }
            else
            {
                string locker = Engine.WorkItemManager.GetLocker(WorkItem.InstanceId, WorkItem.TokenId);
                string lockerFullName = Engine.Organization.GetName(locker);
                Message = string.Format(
                    Configs.Global.ResourceManager.GetString("SheetUtility_LockedBy"),
                    lockerFullName);
                if (task.LockLevel == WorkflowTemplate.LockLevel.Mono) // 禁止多人编辑表单
                    return false;
                else // 当多人编辑表单时，只是给出提示
                    return true;
            }
        }

        ///// <summary>
        ///// 验证锁定级别
        ///// </summary>
        ///// <param name="Enviroment"></param>
        ///// <param name="Message"></param>
        ///// <returns></returns>
        //public static bool TryLock(SheetEnviroment Enviroment, ref string Message)
        //{
        //    return TryLock(Enviroment.Engine, Enviroment.UserValidator, Enviroment.WorkflowActivity, Enviroment.WorkItem, ref Message);
        //}

        /// <summary>
        /// 尝试锁定表单
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="UserValidator"></param>
        /// <param name="WorkflowActivity"></param>
        /// <param name="WorkItem"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static bool TryLock(IEngine Engine,
            UserValidator UserValidator,
            Activity WorkflowActivity,
            WorkItem.WorkItem WorkItem,
            ref string Message)
        {
            WorkItem.LockResult result = OThinker.H3.WorkItem.LockResult.Success;

            // 解析锁定的策略
            bool cancelOther = false;
            bool onlyWarning = false;
            WorkflowTemplate.ParticipativeActivity task = (WorkflowTemplate.ParticipativeActivity)WorkflowActivity;
            if (task != null)
                switch (task.LockLevel)
                {
                    case WorkflowTemplate.LockLevel.Warning:
                        onlyWarning = true;
                        cancelOther = false;
                        break;
                    case WorkflowTemplate.LockLevel.Mono:
                        onlyWarning = false;
                        cancelOther = false;
                        break;
                    case OThinker.H3.WorkflowTemplate.LockLevel.CancelOthers:
                        // 需要单独锁定
                        onlyWarning = false;
                        cancelOther = true;
                        break;
                    default:
                        throw new NotImplementedException();
                }

            if (WorkItem.IsLocking)
            {
                //  已经执行过了锁定
                result = H3.WorkItem.LockResult.LockedAlready;
            }
            else if (WorkItem.IsLocked)
            {
                // 已经被锁定了
                result = H3.WorkItem.LockResult.LockedByOther;
            }
            else
            {
                // 需要单独锁定
                result = Engine.WorkItemManager.LockActivity(WorkItem.WorkItemID, UserValidator.UserID, cancelOther);
            }

            switch (result)
            {
                case OThinker.H3.WorkItem.LockResult.NoNeed:
                case OThinker.H3.WorkItem.LockResult.LockedAlready:
                    return true;
                case OThinker.H3.WorkItem.LockResult.Success:
                    WorkItem.IsLocking = true;
                    Message = Configs.Global.ResourceManager.GetString("SheetUtility_LockSucceed");
                    return true;
                case OThinker.H3.WorkItem.LockResult.LockedByOther:
                    // 找到当前的所有正在进行操作的用户
                    string locker = Engine.WorkItemManager.GetLocker(WorkItem.InstanceId, WorkItem.TokenId);
                    // 生成提示
                    Message = string.Format(
                        Configs.Global.ResourceManager.GetString("SheetUtility_LockedBy"),
                        Engine.Organization.GetName(locker)
                        //Engine.Organization.GetFullName(locker)
                        );

                    // 如果锁定级别是提示，那么仍然返回true，表示验证成功；否则返回false，表示验证失败
                    if (onlyWarning)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        #endregion

        #region 创建一个新的流程实例 --------------
        /// <summary>
        /// 解析HttpRequest中的参数，如果InstanceParams为null，则直接返回，如果流程参数在InstanceParams已经存在该参数则不添加，否则将QueryString中的参数转化为流程参数，如果转换失败，则抛出异常
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Workflow"></param>
        /// <returns></returns>
        public static void ParseRequestParams(
            System.Web.HttpRequest Request,
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate Workflow,
            OThinker.H3.DataModel.BizObjectSchema Schema,
            System.Collections.Generic.Dictionary<string, object> instanceParams)
        {
            if (Request == null || Workflow == null || instanceParams == null)
            {
                return;
            }

            DataModel.PropertySchema[] items = Schema.Properties;
            if (items != null)
            {
                foreach (DataModel.PropertySchema item in items)
                {
                    object v = null;
                    string urlV = Request.QueryString[item.Name];
                    if (!string.IsNullOrEmpty(urlV) &&
                        !instanceParams.ContainsKey(item.Name) &&
                        OThinker.Data.Convertor.Convert(urlV, item.RealType, ref v))
                    {
                        instanceParams.Add(item.Name, v);
                    }
                }
            }
        }

        /// <summary>
        /// 创建一个新的流程实例
        /// </summary>
        /// <param name="Engine">流程引擎</param>
        /// <param name="BizObjectId">业务对象ID</param>
        /// <param name="Workflow">流程模板</param>
        /// <param name="Schema">数据模型结构</param>
        /// <param name="InstanceId">流程实例ID</param>
        /// <param name="Originator">发起人</param>
        /// <param name="OriginatedGroup">发起人选择的身份，可以为空，如果为空，则表示不以任何组成员的身份发起流程</param>
        /// <param name="OriginatedPost">发起人所在的岗位，可以为空，如果为空，则表示不以任何岗位的身份发起流程</param>
        /// <param name="InstanceName">流程实例名称</param>
        /// <param name="OriginatingInstance">发起流程的事件接口</param>
        /// <param name="ParameterTable">发起流程的参数表</param>
        /// <param name="Request">HttpRequest</param>
        /// <param name="WorkItemId">工作任务ID</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <returns>返回创建流程是否成功</returns>
        public static bool OriginateInstance(
            IEngine Engine,
            string BizObjectId,
            WorkflowTemplate.PublishedWorkflowTemplate Workflow,
            DataModel.BizObjectSchema Schema,
            ref string InstanceId,
            string Originator,
            string OriginatedJob,
            string InstanceName,
            EventHandler<OriginateInstanceEventArgs> OriginatingInstance,
            Dictionary<string, object> ParameterTable,
            System.Web.HttpRequest Request,
            ref string WorkItemId,
            ref string ErrorMessage)
        {
            return OriginateInstance(
                Engine,
                BizObjectId,
                Workflow,
                Schema,
                ref InstanceId,
                Originator,
                OriginatedJob,
                InstanceName,
                Instance.PriorityType.Normal,
                OriginatingInstance,
                ParameterTable,
                Request,
                ref WorkItemId,
                ref ErrorMessage,
                false);
        }

        /// <summary>
        /// 创建一个新的流程实例
        /// </summary>
        /// <param name="Engine">引擎实例对象</param>
        /// <param name="BizObjectId">业务对象ID</param>
        /// <param name="Workflow">流程模板</param>
        /// <param name="Schema">数据模型结构</param>
        /// <param name="InstanceId">流程实例ID</param>
        /// <param name="Originator">发起人</param>
        /// <param name="OriginatedJob">发起人使用的角色</param>
        /// <param name="InstanceName">流程实例名称</param>
        /// <param name="Priority">紧急程度</param>
        /// <param name="OriginatingInstance">发起流程的事件接口</param>
        /// <param name="ParameterTable">发起流程的参数表</param>
        /// <param name="Request">HttpRequest</param>
        /// <param name="WorkItemId">返回工作任务ID</param>
        /// <param name="ErrorMessage">错误消息</param>
        /// <param name="FinishStartActivity">是否结束第一个活动</param>
        /// <returns>返回创建流程是否成功</returns>
        public static bool OriginateInstance(
            IEngine Engine,
            string BizObjectId,
            WorkflowTemplate.PublishedWorkflowTemplate Workflow,
            DataModel.BizObjectSchema Schema,
            ref string InstanceId,
            string Originator,
            string OriginatedJob,
            string InstanceName,
            Instance.PriorityType Priority,
            EventHandler<OriginateInstanceEventArgs> OriginatingInstance,
            Dictionary<string, object> ParameterTable,
            System.Web.HttpRequest Request,
            ref string WorkItemId,
            ref string ErrorMessage,
            bool FinishStartActivity)
        {
            if (Workflow == null)
            {
                ErrorMessage = Configs.Global.ResourceManager.GetString("SheetUtility_WorkflowNotExist");
                return false;
            }

            // 创建流程实例
            InstanceId = AppUtility.Engine.InstanceManager.CreateInstance(
                BizObjectId,
                Workflow.WorkflowCode,
                Workflow.WorkflowVersion,
                InstanceId,
                InstanceName,
                Originator,
                OriginatedJob,
                false,
                Instance.InstanceContext.UnspecifiedID,
                null,
                Instance.Token.UnspecifiedID);

            // 设置紧急程度为普通
            OThinker.H3.Messages.MessageEmergencyType emergency = Messages.MessageEmergencyType.Normal;
            // 如果是发起后需要用户填写表单的模式，则紧急程度为高
            if (Workflow.StartWithSheet)
            {
                emergency = OThinker.H3.Messages.MessageEmergencyType.High;
            }

            // 解析流程参数
            System.Collections.Generic.Dictionary<string, object> instanceParams = ParameterTable;
            if (instanceParams == null)
            {
                instanceParams = new Dictionary<string, object>();
            }

            // Http Request Parameters
            ParseRequestParams(Request, Workflow, Schema, instanceParams);

            // 调用发起事件
            OriginateInstanceEventArgs originateArgs = new OriginateInstanceEventArgs(InstanceId, instanceParams);
            if (OriginatingInstance != null)
            {
                OriginatingInstance(OriginatingInstance, originateArgs);
            }

            WorkItemId = Guid.NewGuid().ToString().ToLower();
            // 启动流程的消息
            OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                = new OThinker.H3.Messages.StartInstanceMessage(
                emergency,
                InstanceId,
                WorkItemId,
                originateArgs == null ? null : originateArgs.InstanceParameterTable.Count == 0 ? null : originateArgs.InstanceParameterTable,
                Priority,
                false,
                OThinker.H3.Instance.Token.UnspecifiedID,
                null);
            Engine.InstanceManager.SendMessage(startInstanceMessage);

            if (!Workflow.StartWithSheet)
            {
                // 返回工作项为空
                WorkItemId = H3.WorkItem.WorkItem.NullWorkItemID;
                return true;
            }

            // 查找新创建的工作项
            string[] jobs = null;
            for (int triedTimes = 0; triedTimes < 30; triedTimes++)
            {
                System.Threading.Thread.Sleep(500);
                if (AppUtility.Engine.WorkItemManager.GetWorkItem(WorkItemId) != null)
                {
                    WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(WorkItemId);
                    jobs = new string[] { item.WorkItemID };
                    break;
                }
            }

            if (jobs == null || jobs.Length == 0)
            {
                ErrorMessage = Configs.Global.ResourceManager.GetString("SheetUtility_OriginateFailed");
                WorkItemId = OThinker.H3.WorkItem.WorkItem.NullWorkItemID;
                return false;
            }
            else
            {
                // 返回新创建的工作项
                WorkItemId = jobs[0];

                if (FinishStartActivity)
                {
                    OThinker.H3.WorkItem.WorkItem item = Engine.WorkItemManager.GetWorkItem(WorkItemId);
                    // 结束掉第一个活动
                    Engine.WorkItemManager.FinishWorkItem(
                        WorkItemId,
                        Originator,
                        Request.Browser.IsMobileDevice ? WorkItem.AccessPoint.Mobile : WorkItem.AccessPoint.Web,
                        OriginatedJob,
                        OThinker.Data.BoolMatchValue.Unspecified,
                        null,
                        null,
                        WorkItem.ActionEventType.Forward,
                        WorkItem.WorkItem.UnspecifiedActionButtonType);
                    OThinker.H3.Messages.AsyncEndMessage endMessage = new Messages.AsyncEndMessage(
                        Messages.MessageEmergencyType.Normal,
                        InstanceId,
                        item.ActivityCode,
                        item.TokenId,
                        OThinker.Data.BoolMatchValue.Unspecified,
                        false,
                        OThinker.Data.BoolMatchValue.Unspecified,
                        true,
                        null);
                    Engine.InstanceManager.SendMessage(endMessage);
                }
                return true;
            }
        }

        #endregion

    }
}