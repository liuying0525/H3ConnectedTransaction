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
    /// �����֤�๤��
    /// </summary>
    public class UserValidatorFactory
    {
        /// <summary>
        /// ��֤�û����������Ƿ���ȷ
        /// </summary>
        /// <param name="userAlias">�û���</param>
        /// <param name="password">����</param>
        /// <returns>��ȡ�����õ�¼�Ƿ���ȷ</returns>
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
                    {// �ƶ��칫ʹ��Ȩ����֤
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
                {// �ƶ��칫ʹ��Ȩ����֤
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
        /// ͨ���û�����������
        /// </summary>
        /// <param name="Engine"></param>
        /// <param name="UserLoginName"></param>
        /// <returns></returns>
        public static UserValidator GetUserValidator(IEngine Engine, string UserLoginName)
        {
            return GetUserValidator(Engine, UserLoginName, AppConfig.MonoPortalSettings);
        }

        /// <summary>
        /// ͨ���û�����������
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
        /// ͨ���û�����������
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
        /// ��ȡ��ǰ�û���֤����Ϣ
        /// </summary>
        /// <param name="Page">ҳ������������ҳ��Ĳ���������Ϣ</param>
        /// <param name="PortalRoot">��ʽ��http://H3Site/Portal����󲻴�'/'�����ڻ�õ�¼ҳ�棬��¼ҳ����PortalRoot + "/Login.aspx"</param>
        /// <param name="Message">������ִ������������Ǵ�����Ϣ</param>
        /// <returns>�û���¼��Ϣ</returns>
        public static UserValidator GetUserValidator(System.Web.UI.Page Page, string PortalRoot, out string Message)
        {
            Message = null;
            string windowsAlias = string.Empty;       // Windows �����л�ȡ�����ʺ�
            string urlAlias = string.Empty;           // URL �л�ȡ�����ʺ�
            UserValidator userInSession = null;   // Session �еĶ���

            // 1.ȡ Session �ʺ�
            userInSession = Page.Session[Sessions.GetUserValidator()] as UserValidator;
            try
            {
                urlAlias = Page.Request.QueryString[SheetEnviroment.Param_LoginName];
            }
            catch { }
            //�첽���������Ѿ���¼�ɹ���
            if (userInSession != null)
            {
                if (string.IsNullOrEmpty(urlAlias) || (!string.IsNullOrEmpty(urlAlias) && userInSession.UserCode == urlAlias))
                {
                    return userInSession;
                }
            }

            string tempImagesPath = AppConfig.TempImagesPath;

            // 2.��� Windows ���ɣ����ҵ�ǰ Windows �ʺź� Session ��һ�£���ô�л��û�
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
                    // �л�Windows�ʺŵ�¼
                    if ((bool)settings[H3.Settings.CustomSetting.Setting_ADLoginNameIncludeDomain])
                    {
                        // �����ʺŵĵ�¼
                        userInSession = GetUserValidator(AppUtility.Engine, tempImagesPath, windowsAlias, settings);
                    }
                    else
                    {
                        // �������ʺŵĵ�¼
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

            // 4.�����Windows��֤����ô���URL��֤�ʺ���Ϣ(�˷�ʽ��ȥ��)
            urlAlias = Page.Request.QueryString[SheetEnviroment.Param_LoginName];
            urlAlias = System.Web.HttpUtility.UrlDecode(urlAlias);


            // 5.�ƶ��˵����¼ SID��Token��UserCode ����Ψһ��¼
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

            // δͨ����֤ת���¼ҳ��
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
            string windowsAlias = string.Empty;       // Windows �����л�ȡ�����ʺ�
            string urlAlias = string.Empty;           // URL �л�ȡ�����ʺ�
            UserValidator userInSession = null;   // Session �еĶ���

            // 1.ȡ Session �ʺ�
            userInSession = Controller.Session[Sessions.GetUserValidator()] as UserValidator;
            try
            {
                urlAlias = Controller.Request.QueryString[SheetEnviroment.Param_LoginName];
            }
            catch { }
            //�첽���������Ѿ���¼�ɹ���
            if (userInSession != null)
            {
                if (string.IsNullOrEmpty(urlAlias) || (!string.IsNullOrEmpty(urlAlias) && userInSession.UserCode == urlAlias))
                {
                    return userInSession;
                }
            }

            string tempImagesPath = AppConfig.TempImagesPath;

            // 2.��� Windows ���ɣ����ҵ�ǰ Windows �ʺź� Session ��һ�£���ô�л��û�
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
                    // �л�Windows�ʺŵ�¼
                    if ((bool)settings[H3.Settings.CustomSetting.Setting_ADLoginNameIncludeDomain])
                    {
                        // �����ʺŵĵ�¼
                        userInSession = GetUserValidator(AppUtility.Engine, tempImagesPath, windowsAlias, settings);
                    }
                    if (userInSession == null)
                    {
                        // �������ʺŵĵ�¼
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

            // 4.�����Windows��֤����ô���URL��֤�ʺ���Ϣ(�˷�ʽ��ȥ��)
            urlAlias = Controller.Request.QueryString[SheetEnviroment.Param_LoginName];
            urlAlias = System.Web.HttpUtility.UrlDecode(urlAlias);


            // 5.�ƶ��˵����¼ SID��Token��UserCode ����Ψһ��¼
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

            // 6.֧��H3��SSO,URL�д��� Token
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
        /// ת���¼ҳ��
        /// </summary>
        /// <param name="Request"></param>
        /// <param name="Response"></param>
        /// <param name="PortalRoot"></param>
        public static void RedirectLogin(System.Web.HttpRequest Request, System.Web.HttpResponse Response, string PortalRoot)
        {
            // ����жϣ������̨������ת����Ŀ¼�ĵ�¼����
            if (Request.Url.AbsoluteUri.ToLowerInvariant().IndexOf((PortalRoot + "/Admin").ToLowerInvariant()) > -1)
            {
                return;
            }
            if (Request.Url.ToString().ToLower().Contains("login.aspx"))
            {
                return;
            }
            // ��õ�½ҳ��
            string loginUrl;
            string loginParam = "q=" + System.Web.HttpUtility.UrlEncode(Request.Url.ToString());
            loginUrl = System.IO.Path.Combine(OThinker.H3.Controllers.AppConfig.PortalRootForUrl, "Login.aspx").Replace('\\', '/') + "?" + loginParam;
            // ת���¼����
            Response.Redirect(loginUrl);
        }

        /// <summary>
        /// �����˳����˳�����������û���ص�Session�������ת����¼ҳ��
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="PortalRoot">��ʽ��http://H3Site/Portal����󲻴�'/'�����ڻ�õ�¼ҳ�棬��¼ҳ����PortalRoot + "/Login.aspx"</param>
        public static void Exit(System.Web.UI.Page Page, string PortalRoot)
        {
            // �˳�
            OThinker.H3.Controllers.AppUtility.OnUserLogout(
                (UserValidator)Page.Session[Sessions.GetUserValidator()],
                Page.Request);
            // ��ջ���
            Page.Session[Sessions.GetUserValidator()] = null;
            // ��黺���е�UserAlias�Ƿ����
            Page.Session[Sessions.GetUserAlias()] = null;

            // ��õ�½ҳ��
            string loginPage = System.IO.Path.Combine(OThinker.H3.Controllers.AppConfig.PortalRootForUrl, "Login.aspx").Replace('\\', '/');
            Page.Response.Redirect(loginPage);
        }
        /// <summary>
        /// �����˳����˳�����������û���ص�Session�������ת����¼ҳ��
        /// </summary>
        /// <param name="controller">��ǰ������</param>
        public static void Exit(ControllerBase controller)
        {
            // �˳�
            OThinker.H3.Controllers.AppUtility.OnUserLogout(
                (UserValidator)controller.Session[Sessions.GetUserValidator()],
                HttpContext.Current.Request);
            // ��ջ���
            controller.Session[Sessions.GetUserValidator()] = null;
            // ��黺���е�UserAlias�Ƿ����
            controller.Session[Sessions.GetUserAlias()] = null;
        }

        /// <summary>
        /// ΢�ŵ�¼�����ص�ǰ�û���Ϣ
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
                    //��ְ�������û��Լ������û����ܵ�¼
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
        /// ������½�����ص�ǰ�û���Ϣ
        /// </summary>
        /// <param name="DingTalkCode">��ʱ��Ȩ�룬JSAPI��ȡ</param>
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
                        //ͨ���ֻ���ƥ��
                        OThinker.H3.DingTalk.DingTalkUserByUserid DingTalkUserInfo = engine.DingTalkAdapter.GetUserInfoByUserid(userCode);
                        if (DingTalkUserInfo != null)
                        {
                            string uCode = engine.Organization.GetUserCodeByMobile(DingTalkUserInfo.mobile);
                            unit = engine.Organization.GetUserByCode(uCode);
                        }
                    }

                    //��ְ�������û��Լ������û����ܵ�¼
                    if (unit == null
                        || unit.ServiceState == UserServiceState.Dismissed
                        || unit.IsVirtualUser
                        || unit.State == State.Inactive)
                        return null;

                    //DingTalkAccountΪ��ʱ��һ�θ���
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
        /// ������¼��֤
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
        /// ����PC����ҳ���½�����ص�ǰ�û���Ϣ,ʹ��UserIDea��֤
        /// </summary>
        /// <param name="UserID">�û�H3�е�Object ID</param>
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
        /// ΢�ŵ�¼��֤
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
        /// ��¼��֤
        /// </summary>
        /// <param name="AuthenticationType">��֤��ʽ</param>
        /// <param name="EngineCode">�������</param>
        /// <param name="UserName">�û���</param>
        /// <param name="Password">����</param>
        /// <param name="LoginType">��¼����</param>
        /// <param name="isAdminLogin">�Ƿ��̨��¼</param>
        /// <returns>���ص�¼�Ƿ�ɹ�</returns>
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

            //������Ǻ�̨��½����½�ɹ����ɣ����Ǻ�̨��½�����ж��Ƿ��к�̨��½Ȩ��
            if (isAdminLogin && !(user.User.IsConsoleUser || user.ValidateAdministrator()))
            {
                return false;
            }

            HttpContext.Current.Session[Sessions.GetUserValidator()] = user;
            OThinker.H3.Controllers.AppUtility.OnUserLogin(user, HttpContext.Current.Request, LoginType);

            return true;
        }

        /// <summary>
        /// ��ǰ�û�����
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
                                    // URL �ʺŲ�Ϊ��
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
        /// ��¼�����ת
        /// </summary>
        /// <param name="userName">��¼��</param>
        /// <param name="isPersist">�Ƿ��¼��¼��Ϣ[forms��ʽ��Ч]</param>
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

        #region ��¼��֤

        /// <summary>
        /// ��֤�û�����ݺ�����
        /// </summary>
        /// <param name="Organization">��֯�ṹ������</param>
        /// <param name="UserAlias">�û���¼��</param>
        /// <param name="Password">�û���¼����</param>
        /// <param name="EnableADValidation">�Ƿ�ʹ��AD��֤</param>
        /// <param name="ADPathes">AD��ַ</param>
        /// <returns>����û���֤�ɹ����򷵻�true�����򷵻�false</returns>
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
        /// ��֤�û�����ݺ�����
        /// </summary>
        /// <param name="Organization">��֯�ṹ������</param>
        /// <param name="UserAlias">�û���¼��</param>
        /// <param name="Password">�û���¼����</param>
        /// <param name="EnableADValidation">�Ƿ�ʹ��AD��֤</param>
        /// <param name="ADPathes">AD��ַ</param>
        /// <returns>����û���֤�ɹ����򷵻�User�����򷵻�null</returns>
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
            {// �����û�����ְ�������û��������¼
                return null;
            }
            if (EnableADValidation && (!user.IsSystemUser || !user.IsAdministrator))
            {
                // ʹ��AD��֤
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
                // ������ȷ
                return user;
            }
            return null;
        }

        /// <summary>
        /// ��֤AD�û��ʺ������Ƿ���ȷ
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
                // �󶨵����� AdsObject ��ǿ�������֤�� 
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