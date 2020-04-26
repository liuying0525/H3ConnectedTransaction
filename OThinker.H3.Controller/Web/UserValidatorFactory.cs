using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Configuration;
using System.Web;
using System.Web.UI;
using System.Web.Security;
using OThinker.H3.Configs;
using System.IO;
using System.Net;
using System.DirectoryServices;
using OThinker.H3.WeChat;
using OThinker.Organization;
using OThinker.H3.Site;
using System.Configuration;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 身分验证类工厂
    /// </summary>
    public class UserValidatorFactory
    {
        /// <summary>
        /// 验证用户名或密码是否正确
        /// </summary>
        /// <param name="userAlias">用户名</param>
        /// <param name="password">密码</param>
        /// <returns>获取或设置登录是否正确</returns>
        public static UserValidator Validate(string userAlias, string password)
        {
            if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
            {
                bool enableAD = false;
                bool success = false;
                string adPath = string.Empty;

                Dictionary<string, object> settings = AppUtility.Engine.SettingManager.GetSheetSettings();

                enableAD = (bool)settings[OThinker.H3.Settings.CustomSetting.Setting_EnableADValidation];

                adPath = settings[OThinker.H3.Settings.CustomSetting.Setting_ADPathes] + string.Empty;
                success = Validate(AppConfig.MonoEngine.Organization, userAlias, password, enableAD, adPath);
                if (!success) return null;

                UserValidator user = GetUserValidator(AppConfig.MonoEngine, userAlias, settings);
                if (user.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType] + string.Empty != OThinker.H3.Configs.LicenseType.Develop.ToString())
                {
                    if (!bool.Parse(user.PortalSettings[OThinker.H3.Configs.License.PropertyName_Mobile] + string.Empty))
                    {// 移动办公使用权限验证
                        if (HttpContext.Current.Request != null)
                        {
                            string url = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
                            if (url.IndexOf("mobile") > -1 || url.IndexOf("ismobile") > -1)
                            {
                                return null;
                            }
                        }
                    }
                }
                return user;
            }
            else
            {

                if (!Login(Clusterware.AuthenticationType.Forms, null, userAlias, password, PortalType.Portal))
                {
                    return null;
                }
                Dictionary<string, object> settings = CurrentUser.Engine.SettingManager.GetSheetSettings();

                if (!bool.Parse(settings[OThinker.H3.Configs.License.PropertyName_Mobile] + string.Empty))
                {// 移动办公使用权限验证
                    if (HttpContext.Current.Request != null)
                    {
                        string url = HttpContext.Current.Request.UrlReferrer.ToString().ToLower();
                        if (url.IndexOf("mobile") > -1 || url.IndexOf("ismobile") > -1)
                        {
                            return null;
                        }
                    }
                }

                return CurrentUser;
            }
        }

        /// <summary>
        /// 通过用户别名获得身份
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="UserLoginName"></param>
        /// <returns></returns>
        public static UserValidator GetUserValidator(IEngine Engine, string UserLoginName)
        {
            return GetUserValidator(Engine, UserLoginName, AppConfig.MonoPortalSettings);
        }

        /// <summary>
        /// 通过用户别名获得身份
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="UserLoginName"></param>
        /// <param name="Settings"></param>
        /// <returns></returns>
        public static UserValidator GetUserValidator(IEngine Engine, string UserLoginName, Dictionary<string, object> Settings)
        {
            if (string.IsNullOrEmpty(UserLoginName)) return null;
            string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
            Organization.User unit = Engine.Organization.GetUserByCode(UserLoginName);
            if (unit == null)
            {
                return null;
            }
            return new UserValidator(Engine, unit.ObjectID, tempImagesPath, Settings);
        }

        /// <summary>
        /// 通过用户别名获得身份
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="TempImagesPath"></param>
        /// <param name="UserLoginName"></param>
        /// <returns></returns>
        private static UserValidator GetUserValidator(IEngine Engine, string TempImagesPath, string UserLoginName, Dictionary<string, object> PortalSettings)
        {
            if (string.IsNullOrEmpty(UserLoginName)) return null;
            Organization.User unit = Engine.Organization.GetUserByCode(UserLoginName);
            if (unit == null)
            {
                return null;
            }
            return new UserValidator(Engine, unit.ObjectID, TempImagesPath, PortalSettings);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="TempImagesPath"></param>
        /// <param name="UserId"></param>
        /// <returns></returns>
        public static UserValidator GetUserValidatorById(IEngine Engine, string TempImagesPath, string UserId, Dictionary<string, object> PortalSettings)
        {
            if (UserId == null || UserId == "")
            {
                return null;
            }
            else
            {
                return new UserValidator(Engine, UserId, TempImagesPath, PortalSettings);
            }
        }

        /// <summary>
        /// 获取当前用户认证的信息
        /// </summary>
        /// <param name="Page">页面对象，用来获得页面的参数配置信息</param>
        /// <param name="PortalRoot">格式是http://H3Site/Portal，最后不带'/'。用于获得登录页面，登录页面是PortalRoot + "/Login.aspx"</param>
        /// <param name="Message">如果出现错误，则在这里标记错误信息</param>
        /// <returns>用户登录信息</returns>
        public static UserValidator GetUserValidator(System.Web.UI.Page Page, string PortalRoot, out string Message)
        {
            Message = null;
            string windowsAlias = string.Empty;       // Windows 集成中获取到的帐号
            string urlAlias = string.Empty;           // URL 中获取到的帐号
            UserValidator userInSession = null;   // Session 中的对象

            // 1.取 Session 帐号
            userInSession = Page.Session[Sessions.GetUserValidator()] as UserValidator;
            try
            {
                urlAlias = Page.Request.QueryString[SheetEnviroment.Param_LoginName];
            }
            catch { }
            //异步话，这里已经登录成功了
            if (userInSession != null)
            {
                if (string.IsNullOrEmpty(urlAlias) || (!string.IsNullOrEmpty(urlAlias) && userInSession.UserCode == urlAlias))
                {
                    return userInSession;
                }
            }

            string tempImagesPath = AppConfig.TempImagesPath;

            // 2.如果 Windows 集成，并且当前 Windows 帐号和 Session 不一致，那么切换用户
            windowsAlias = Page.User.Identity.Name;
            if (!string.IsNullOrEmpty(windowsAlias))
            {
                if (windowsAlias.IndexOf("|") > -1) windowsAlias = windowsAlias.Substring(windowsAlias.IndexOf("|") + 1);
                string windowsAliasNoDomain = windowsAlias;
                if (windowsAlias.IndexOf("\\") > -1)
                {
                    windowsAliasNoDomain = windowsAlias.Substring(windowsAlias.IndexOf("\\") + 1);
                }
                if (userInSession == null || (userInSession.UserCode != windowsAlias && userInSession.UserCode != windowsAliasNoDomain))
                {
                    Dictionary<string, object> settings = AppUtility.Engine.SettingManager.GetSheetSettings();
                    // 切换Windows帐号登录
                    if ((bool)settings[H3.Settings.CustomSetting.Setting_ADLoginNameIncludeDomain])
                    {
                        // 带域帐号的登录
                        userInSession = GetUserValidator(AppUtility.Engine, tempImagesPath, windowsAlias, settings);
                    }
                    else
                    {
                        // 不带域帐号的登录
                        userInSession = GetUserValidator(AppUtility.Engine, tempImagesPath, windowsAliasNoDomain, settings);
                    }
                    if (userInSession == null || userInSession.User.State == Organization.State.Inactive)
                    {
                        Message = Configs.Global.ResourceManager.GetString("UserValidatorFactory_User") + windowsAlias + " " + Configs.Global.ResourceManager.GetString("UserValidatorFactory_Mssg");
                        return null;
                    }
                    Page.Session[Sessions.GetUserValidator()] = userInSession;
                    FormsAuthentication.SetAuthCookie(userInSession.UserCode, false);
                }

                return userInSession;
            }

            // 4.如果非Windows认证，那么检测URL认证帐号信息(此方式得去除)
            urlAlias = Page.Request.QueryString[SheetEnviroment.Param_LoginName];
            urlAlias = System.Web.HttpUtility.UrlDecode(urlAlias);


            // 5.移动端单点登录 SID、Token、UserCode 进行唯一登录
            string sid = Page.Request.QueryString[OThinker.H3.Controllers.SheetEnviroment.Param_LoginSID];
            string token = Page.Request.QueryString[OThinker.H3.Controllers.SheetEnviroment.Param_MobileToken];
            if (!string.IsNullOrEmpty(urlAlias)
                && !string.IsNullOrEmpty(sid)
                && !string.IsNullOrEmpty(token)
                && AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
            {
                OThinker.Organization.User user = AppUtility.Engine.Organization.GetUserByCode(urlAlias);
                if (user != null)
                {
                    if (sid == user.SID && OThinker.Security.MD5Encryptor.GetMD5(token) == user.MobileToken)
                    {
                        userInSession = GetUserValidator(OThinker.H3.Controllers.AppUtility.Engine, urlAlias);
                        Page.Session[Sessions.GetUserValidator()] = userInSession;
                        FormsAuthentication.SetAuthCookie(userInSession.UserCode, false);
                    }
                }
            }

            // 未通过认证转向登录页面
            if (userInSession == null)
            {
                RedirectLogin(Page.Request, Page.Response, PortalRoot);
            }

            return userInSession;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Controller"></param>
        /// <param name="PortalRoot"></param>
        /// <param name="Message"></param>
        /// <returns></returns>
        public static UserValidator GetUserValidator(System.Web.Mvc.Controller Controller, string PortalRoot, out string Message)
        {
            Message = null;
            string windowsAlias = string.Empty;       // Windows 集成中获取到的帐号
            string urlAlias = string.Empty;           // URL 中获取到的帐号
            UserValidator userInSession = null;   // Session 中的对象

            // 1.取 Session 帐号
            userInSession = Controller.Session[Sessions.GetUserValidator()] as UserValidator;
            try
            {
                urlAlias = Controller.Request.QueryString[SheetEnviroment.Param_LoginName];
            }
            catch { }
            //异步话，这里已经登录成功了
            if (userInSession != null)
            {
                if (string.IsNullOrEmpty(urlAlias) || (!string.IsNullOrEmpty(urlAlias) && userInSession.UserCode == urlAlias))
                {
                    return userInSession;
                }
            }

            string tempImagesPath = AppConfig.TempImagesPath;

            // 2.如果 Windows 集成，并且当前 Windows 帐号和 Session 不一致，那么切换用户
            windowsAlias = Controller.User.Identity.Name;
            if (!string.IsNullOrEmpty(windowsAlias))
            {
                if (windowsAlias.IndexOf("|") > -1) windowsAlias = windowsAlias.Substring(windowsAlias.LastIndexOf("|") + 1);
                string windowsAliasNoDomain = windowsAlias;
                if (windowsAlias.IndexOf("\\") > -1)
                {
                    windowsAliasNoDomain = windowsAlias.Substring(windowsAlias.IndexOf("\\") + 1);
                }
                if (userInSession == null || (userInSession.UserCode != windowsAlias && userInSession.UserCode != windowsAliasNoDomain))
                {
                    Dictionary<string, object> settings = AppUtility.Engine.SettingManager.GetSheetSettings();
                    // 切换Windows帐号登录
                    if ((bool)settings[H3.Settings.CustomSetting.Setting_ADLoginNameIncludeDomain])
                    {
                        // 带域帐号的登录
                        userInSession = GetUserValidator(AppUtility.Engine, tempImagesPath, windowsAlias, settings);
                    }
                    if (userInSession == null)
                    {
                        // 不带域帐号的登录
                        userInSession = GetUserValidator(AppUtility.Engine, tempImagesPath, windowsAliasNoDomain, settings);
                    }
                    if (userInSession == null || userInSession.User.State == Organization.State.Inactive)
                    {
                        Message = Configs.Global.ResourceManager.GetString("UserValidatorFactory_User") + windowsAlias + " " + Configs.Global.ResourceManager.GetString("UserValidatorFactory_Mssg");
                        return null;
                    }
                    Controller.Session[Sessions.GetUserValidator()] = userInSession;
                    FormsAuthentication.SetAuthCookie(userInSession.UserCode, false);
                }

                return userInSession;
            }

            // 4.如果非Windows认证，那么检测URL认证帐号信息(此方式得去除)
            urlAlias = Controller.Request.QueryString[SheetEnviroment.Param_LoginName];
            urlAlias = System.Web.HttpUtility.UrlDecode(urlAlias);


            // 5.移动端单点登录 SID、Token、UserCode 进行唯一登录
            string sid = Controller.Request.QueryString[OThinker.H3.Controllers.SheetEnviroment.Param_LoginSID];
            string token = Controller.Request.QueryString[OThinker.H3.Controllers.SheetEnviroment.Param_MobileToken];
            if (!string.IsNullOrEmpty(urlAlias)
                && !string.IsNullOrEmpty(sid)
                && !string.IsNullOrEmpty(token)
                && AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
            {
                OThinker.Organization.User user = AppUtility.Engine.Organization.GetUserByCode(urlAlias);
                if (user != null)
                {
                    if (sid == user.SID && OThinker.Security.MD5Encryptor.GetMD5(token) == user.MobileToken)
                    {
                        userInSession = GetUserValidator(OThinker.H3.Controllers.AppUtility.Engine, urlAlias);
                        Controller.Session[Sessions.GetUserValidator()] = userInSession;
                        FormsAuthentication.SetAuthCookie(userInSession.UserCode, false);
                    }
                }
            }

            // 6.支持H3的SSO,URL中传入 Token
            token = Controller.Request.QueryString["Token"] + string.Empty;
            string secret = ConfigurationManager.AppSettings["Secret"] + string.Empty;
            if (token!=string.Empty&&secret != string.Empty)
            {
                string userCode = AppUtility.Engine.SSOManager.GetUserCode(OThinker.H3.Configs.ProductInfo.ProductName, secret, token);
                userInSession = GetUserValidator(OThinker.H3.Controllers.AppUtility.Engine, userCode);
                if (userInSession != null)
                {
                    Controller.Session[Sessions.GetUserValidator()] = userInSession;
                    FormsAuthentication.SetAuthCookie(userInSession.UserCode, false);
                }
            }
            return userInSession;
        }

        /// <summary>
        /// 转向登录页面
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Response"></param>
        /// <param name="PortalRoot"></param>
        public static void RedirectLogin(System.Web.HttpRequest Request, System.Web.HttpResponse Response, string PortalRoot)
        {
            // 添加判断：管理后台，不跳转到根目录的登录界面
            if (Request.Url.AbsoluteUri.ToLowerInvariant().IndexOf((PortalRoot + "/Admin").ToLowerInvariant()) > -1)
            {
                return;
            }
            if (Request.Url.ToString().ToLower().Contains("login.aspx"))
            {
                return;
            }
            // 获得登陆页面
            string loginUrl;
            string loginParam = "q=" + System.Web.HttpUtility.UrlEncode(Request.Url.ToString());
            loginUrl = System.IO.Path.Combine(OThinker.H3.Controllers.AppConfig.PortalRootForUrl, "Login.aspx").Replace('\\', '/') + "?" + loginParam;
            // 转向登录界面
            Response.Redirect(loginUrl);
        }

        /// <summary>
        /// 用于退出，退出将会清空与用户相关的Session，最后跳转到登录页面
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="PortalRoot">格式是http://H3Site/Portal，最后不带'/'。用于获得登录页面，登录页面是PortalRoot + "/Login.aspx"</param>
        public static void Exit(System.Web.UI.Page Page, string PortalRoot)
        {
            // 退出
            OThinker.H3.Controllers.AppUtility.OnUserLogout(
                (UserValidator)Page.Session[Sessions.GetUserValidator()],
                Page.Request);
            // 清空缓存
            Page.Session[Sessions.GetUserValidator()] = null;
            // 检查缓存中的UserAlias是否存在
            Page.Session[Sessions.GetUserAlias()] = null;

            // 获得登陆页面
            string loginPage = System.IO.Path.Combine(OThinker.H3.Controllers.AppConfig.PortalRootForUrl, "Login.aspx").Replace('\\', '/');
            Page.Response.Redirect(loginPage);
        }
        /// <summary>
        /// 用于退出，退出将会清空与用户相关的Session，最后跳转到登录页面
        /// </summary>
        /// <param name="controller">当前控制器</param>
        public static void Exit(ControllerBase controller)
        {
            // 退出
            OThinker.H3.Controllers.AppUtility.OnUserLogout(
                (UserValidator)controller.Session[Sessions.GetUserValidator()],
                HttpContext.Current.Request);
            // 清空缓存
            controller.Session[Sessions.GetUserValidator()] = null;
            // 检查缓存中的UserAlias是否存在
            controller.Session[Sessions.GetUserAlias()] = null;
        }

        /// <summary>
        /// 微信登录，返回当前用户信息
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="WeChatCode"></param>
        /// <returns></returns>
        public static UserValidator LoginAsWeChatReturnUserValidator(string EngineCode, string WeChatCode)
        {
            UserValidator user = null;
            string userCode = string.Empty;
            string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
            switch (AppConfig.ConnectionMode)
            {
                case ConnectionStringParser.ConnectionMode.Mono:
                    IEngine engine = AppUtility.Engine;
                    userCode = engine.WeChatAdapter.GetUserCode(WeChatCode);
                    OThinker.Organization.User unit = engine.Organization.GetUserByCode(userCode);
                    if (unit == null) return null;
                    //离职、禁用用户以及虚拟用户不能登录
                    if (unit == null
                        || unit.ServiceState == UserServiceState.Dismissed
                        || unit.IsVirtualUser
                        || unit.State == State.Inactive)
                        return null;
                    user = new UserValidator(engine, unit.ObjectID, tempImagesPath, null);
                    break;
                case ConnectionStringParser.ConnectionMode.Shared:
                    OThinker.H3.Connection conn = new Connection();
                    string userId = string.Empty;
                    if (conn.Open(AppConfig.ConnectionStringParser.Servers[0],
                         Clusterware.AuthenticationType.WeChat,
                         EngineCode, WeChatCode, "WeChat", out userId) != Clusterware.ConnectionResult.Success)
                    {
                        return null;
                    }
                    user = new UserValidator(conn.Engine, userId, tempImagesPath, null);
                    break;
                default:
                    throw new NotImplementedException();
            }

            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, Site.PortalType.WeChat);
            return user;
        }

        /// <summary>
        /// 钉钉登陆，返回当前用户信息
        /// </summary>
        /// <param name="DingTalkCode">临时授权码，JSAPI获取</param>
        /// <returns></returns>
        public static UserValidator LoginAsDingTalkReturnUserValidator(string EngineCode, string DingTalkCode)
        {
            UserValidator user = null;
            string userCode = string.Empty;
            string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
            switch (AppConfig.ConnectionMode)
            {
                case ConnectionStringParser.ConnectionMode.Mono:
                    IEngine engine = AppUtility.Engine;

                    OThinker.H3.DingTalk.DingTalkUserInfo dingUser = engine.DingTalkAdapter.GetUserInfo(DingTalkCode);
                    if (dingUser == null) return null;
                    userCode = dingUser.userid;
                    engine.LogWriter.Write("DingTalk---deviceid:" + dingUser.deviceId + ",userid:" + dingUser.userid);

                    OThinker.Organization.User unit = engine.Organization.GetUserByCode(userCode);
                    if (unit == null)
                    {
                        //通过手机号匹配
                        OThinker.H3.DingTalk.DingTalkUserByUserid DingTalkUserInfo = engine.DingTalkAdapter.GetUserInfoByUserid(userCode);
                        if (DingTalkUserInfo != null)
                        {
                            string uCode = engine.Organization.GetUserCodeByMobile(DingTalkUserInfo.mobile);
                            unit = engine.Organization.GetUserByCode(uCode);
                        }
                    }

                    //离职、禁用用户以及虚拟用户不能登录
                    if (unit == null
                        || unit.ServiceState == UserServiceState.Dismissed
                        || unit.IsVirtualUser
                        || unit.State == State.Inactive)
                        return null;

                    //DingTalkAccount为空时做一次更新
                    if (string.IsNullOrEmpty(unit.DingTalkAccount))
                    {
                        unit.DingTalkAccount = dingUser.userid;
                        engine.Organization.UpdateUnit(User.AdministratorID, unit);
                    }
                    user = new UserValidator(engine, unit.ObjectID, tempImagesPath, null);
                    break;
                case ConnectionStringParser.ConnectionMode.Shared:
                    OThinker.H3.Connection conn = new Connection();
                    string userId = string.Empty;
                    if (conn.Open(AppConfig.ConnectionStringParser.Servers[0],
                         Clusterware.AuthenticationType.WeChat,
                         EngineCode, DingTalkCode, "DingTalk", out userId) != Clusterware.ConnectionResult.Success)
                    {
                        return null;
                    }
                    user = new UserValidator(conn.Engine, userId, tempImagesPath, null);
                    break;
                default:
                    throw new NotImplementedException();
            }

            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, Site.PortalType.DingTalk);
            return user;
        }


        /// <summary>
        /// 钉钉登录认证
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="WeChatCode"></param>
        /// <returns></returns>
        public static bool LoginAsDingTalk(string EngineCode, string DingTalkCode)
        {
            UserValidator user = LoginAsDingTalkReturnUserValidator(EngineCode, DingTalkCode);
            if (user == null) return false;

            HttpContext.Current.Session[Sessions.GetDingTalkLogin()] = true;
            HttpContext.Current.Session[Sessions.GetUserValidator()] = user;
            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, Site.PortalType.DingTalk);
            return true;
        }



        /// <summary>
        /// 钉钉PC弹出页面登陆，返回当前用户信息,使用UserIDea验证
        /// </summary>
        /// <param name="UserID">用户H3中的Object ID</param>
        /// <returns></returns>
        public static UserValidator LoginAsDingTalkPCAndReturnUserValidator(string EngineCode, string UserID)
        {
            UserValidator user = null;
            string userCode = string.Empty;
            string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
            switch (AppConfig.ConnectionMode)
            {
                case ConnectionStringParser.ConnectionMode.Mono:
                    IEngine engine = AppUtility.Engine;

                    OThinker.Organization.User unit = engine.Organization.GetUnit(UserID) as OThinker.Organization.User;
                    if (unit == null
                        || unit.ServiceState == UserServiceState.Dismissed
                        || unit.IsVirtualUser) return null;
                    user = new UserValidator(engine, unit.ObjectID, tempImagesPath, null);
                    break;
                case ConnectionStringParser.ConnectionMode.Shared:
                    OThinker.H3.Connection conn = new Connection();
                    string userId = string.Empty;
                    if (conn.Open(AppConfig.ConnectionStringParser.Servers[0],
                         Clusterware.AuthenticationType.WeChat,
                         EngineCode, UserID, "DingTalk", out userId) != Clusterware.ConnectionResult.Success)
                    {
                        return null;
                    }
                    user = new UserValidator(conn.Engine, userId, tempImagesPath, null);
                    break;
                default:
                    throw new NotImplementedException();
            }

            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, Site.PortalType.DingTalk);
            return user;
        }


        /// <summary>
        /// 微信登录认证
        /// </summary>
        /// <param name="EngineCode"></param>
        /// <param name="WeChatCode"></param>
        /// <returns></returns>
        public static bool LoginAsWeChat(string EngineCode, string WeChatCode)
        {
            UserValidator user = LoginAsWeChatReturnUserValidator(EngineCode, WeChatCode);
            if (user == null) return false;

            HttpContext.Current.Session[Sessions.GetWeChatLogin()] = true;
            HttpContext.Current.Session[Sessions.GetUserValidator()] = user;
            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, Site.PortalType.WeChat);
            return true;
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        /// <param name="AuthenticationType">验证方式</param>
        /// <param name="EngineCode">引擎编码</param>
        /// <param name="UserName">用户名</param>
        /// <param name="Password">密码</param>
        /// <param name="LoginType">登录类型</param>
        /// <param name="isAdminLogin">是否后台登录</param>
        /// <returns>返回登录是否成功</returns>
        public static bool Login(Clusterware.AuthenticationType AuthenticationType,
            string EngineCode,
            string UserName,
            string Password,
            PortalType LoginType,
            bool isAdminLogin = false)
        {
            UserValidator user = null;
            string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
            switch (AppConfig.ConnectionMode)
            {
                case ConnectionStringParser.ConnectionMode.Mono:
                    IEngine engine = AppUtility.Engine;
                    Dictionary<string, object> settings = engine.SettingManager.GetSheetSettings();
                    bool enableAD = false;
                    string adPath = string.Empty;
                    bool.TryParse(settings[OThinker.H3.Settings.CustomSetting.Setting_EnableADValidation] + string.Empty, out enableAD);
                    adPath = settings[OThinker.H3.Settings.CustomSetting.Setting_ADPathes] + string.Empty;

                    if (!Validate(engine.Organization, UserName, Password, enableAD, adPath))
                    {
                        return false;
                    }

                    user = GetUserValidator(AppUtility.Engine,
                        tempImagesPath,
                        UserName,
                        settings);
                    break;
                case ConnectionStringParser.ConnectionMode.Shared:
                    OThinker.H3.Connection conn = new Connection();
                    if (conn.Open(AppConfig.ConnectionStringParser.Servers[0],
                        AuthenticationType,
                        EngineCode, UserName, Password) != Clusterware.ConnectionResult.Success)
                    {
                        return false;
                    }
                    OThinker.Organization.User unit = conn.Engine.Organization.GetUserByCode(UserName);
                    if (unit == null) return false;
                    user = new UserValidator(conn.Engine,
                        unit.UnitID,
                        tempImagesPath,
                        null);
                    break;
                default:
                    throw new NotImplementedException();
            }

            //如果不是后台登陆，登陆成功即可；如是后台登陆，则判断是否有后台登陆权限
            if (isAdminLogin && !(user.User.IsConsoleUser || user.ValidateAdministrator()))
            {
                return false;
            }

            HttpContext.Current.Session[Sessions.GetUserValidator()] = user;
            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, LoginType);

            return true;
        }

        /// <summary>
        /// 当前用户对象
        /// </summary>
        public static UserValidator CurrentUser
        {
            get
            {
                if (HttpContext.Current.Session[Sessions.GetUserValidator()] == null)
                {
                    switch (AppConfig.ConnectionMode)
                    {
                        case ConnectionStringParser.ConnectionMode.Mono:
                            string code = null;
                            if (HttpContext.Current.Request.IsAuthenticated)
                            {
                                code = HttpContext.Current.User.Identity.Name;

                                string tempImagesPath = Path.Combine(HttpContext.Current.Server.MapPath("."), "TempImages");
                                UserValidator user = GetUserValidator(AppConfig.MonoEngine, tempImagesPath, code, AppConfig.MonoPortalSettings);
                                HttpContext.Current.Session[Sessions.GetUserValidator()] = user;
                            }
                            else
                            {
                                string urlAlias = HttpContext.Current.Request.QueryString[SheetEnviroment.Param_LoginName];
                                urlAlias = System.Web.HttpUtility.UrlDecode(urlAlias);

                                if (!string.IsNullOrEmpty(urlAlias))
                                {
                                    // URL 帐号不为空
                                    string loginPassword = HttpContext.Current.Request.QueryString[OThinker.H3.Controllers.SheetEnviroment.Param_LoginPassword];
                                    loginPassword = System.Web.HttpUtility.UrlDecode(loginPassword);
                                    if (!Login(Clusterware.AuthenticationType.Forms, null, urlAlias, loginPassword, PortalType.Portal))
                                    {
                                        RedirectLogin(HttpContext.Current.Request, HttpContext.Current.Response, AppConfig.PortalRoot);
                                    }
                                    HttpContext.Current.Session[Sessions.GetUserValidator()] = CurrentUser;
                                }
                                else
                                {
                                    RedirectLogin(HttpContext.Current.Request, HttpContext.Current.Response, AppConfig.PortalRoot);
                                }
                            }
                            break;
                        case ConnectionStringParser.ConnectionMode.Shared:
                            RedirectLogin(HttpContext.Current.Request, HttpContext.Current.Response, AppConfig.PortalRoot);
                            break;
                    }
                }
                return (UserValidator)HttpContext.Current.Session[Sessions.GetUserValidator()];
            }
        }

        /// <summary>
        /// 登录后的跳转
        /// </summary>
        /// <param name="userName">登录名</param>
        /// <param name="isPersist">是否记录登录信息[forms方式有效]</param>
        public static void Redirect(string userName)
        {
            // Get the section.
            AuthenticationMode authMode = AppConfig.AuthenticationMode;
            //Forms
            if (authMode == AuthenticationMode.Forms)
            {
                if (!string.IsNullOrEmpty(FormsAuthentication.CookieDomain))
                {
                    HttpCookie UserCookie = HttpContext.Current.Response.Cookies[FormsAuthentication.FormsCookieName];
                    if (UserCookie != null)
                    {
                        UserCookie.Domain = FormsAuthentication.CookieDomain;
                    }
                }
                HttpContext.Current.Response.Redirect(FormsAuthentication.GetRedirectUrl(userName, false));
            }
            //Windows, None
            else
            {
                string goUrl = HttpContext.Current.Request.QueryString["ReturnUrl"] ?? HttpContext.Current.Request.QueryString["q"];
                if (string.IsNullOrEmpty(goUrl))
                {
                    goUrl = HttpContext.Current.Request.ApplicationPath;
                    goUrl = goUrl.TrimEnd('/') + "/";

                    //if (HttpContext.Current.Request.UserAgent.ToLower().IndexOf("mobile") >= 0)
                    //{
                    //    goUrl += "Mobile/index.html";
                    //}
                }
                HttpContext.Current.Response.Redirect(goUrl);
            }
        }

        #region 登录验证

        /// <summary>
        /// 验证用户的身份和密码
        /// </summary>
        /// <param name="Organization">组织结构管理器</param>
        /// <param name="UserAlias">用户登录名</param>
        /// <param name="Password">用户登录密码</param>
        /// <param name="EnableADValidation">是否使用AD认证</param>
        /// <param name="ADPathes">AD地址</param>
        /// <returns>如果用户验证成功，则返回true，否则返回false</returns>
        public static bool Validate(
            OThinker.Organization.IOrganization Organization,
            string UserAlias,
            string Password,
            bool EnableADValidation,
            string ADPathes)
        {
            return ValidateUser(Organization, UserAlias, Password, EnableADValidation, ADPathes) != null;
        }

        /// <summary>
        /// 验证用户的身份和密码
        /// </summary>
        /// <param name="Organization">组织结构管理器</param>
        /// <param name="UserAlias">用户登录名</param>
        /// <param name="Password">用户登录密码</param>
        /// <param name="EnableADValidation">是否使用AD认证</param>
        /// <param name="ADPathes">AD地址</param>
        /// <returns>如果用户验证成功，则返回User，否则返回null</returns>
        public static OThinker.Organization.User ValidateUser(
            OThinker.Organization.IOrganization Organization,
            string UserAlias,
            string Password,
            bool EnableADValidation,
            string ADPathes)
        {
            if (UserAlias == null)
            {
                return null;
            }
            OThinker.Organization.User user = Organization.GetUserByCode(UserAlias);
            if (user == null || user.State == State.Inactive
                || user.ServiceState == UserServiceState.Dismissed || user.IsVirtualUser)
            {// 虚拟用户、离职、禁用用户不允许登录
                return null;
            }
            if (EnableADValidation && (!user.IsSystemUser || !user.IsAdministrator))
            {
                // 使用AD认证
                bool success = false;
                if (!string.IsNullOrEmpty(ADPathes))
                {
                    string[] ads = ADPathes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string ad in ads)
                    {
                        success = IsAuthenticated(ad, UserAlias, Password);
                        if (success) return user;
                    }
                }
            }
            else if (user.ValidatePassword(Password))
            {
                // 密码正确
                return user;
            }
            return null;
        }

        /// <summary>
        /// 验证AD用户帐号密码是否正确
        /// </summary>
        /// <param name="AdServer"></param>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        private static bool IsAuthenticated(string AdServer, string UserName, string Password)
        {
            try
            {
                DirectoryEntry entry = new DirectoryEntry(AdServer, UserName, Password);
                // 绑定到本机 AdsObject 以强制身份验证。 
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + UserName + ")";
                search.PropertiesToLoad.Add("cn");
                SearchResult result = search.FindOne();
                if (null == result)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        #endregion
    }
}