using System;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using System.Data;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 组织机构服务
    /// </summary>
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public OrganizationController()
        {
        }

        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="userCode">用户账号</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult LoginIn(string userCode, string password)
        {
            bool loginResult = false;
            object result;
            string enableCheckCode = System.Configuration.ConfigurationManager.AppSettings["EnableCheckCode"] + string.Empty;
            try
            {
                if (enableCheckCode != "0")
                {
                    string[] wait = System.Configuration.ConfigurationManager.AppSettings["WaitTime"].Split(',');
                    var sql = "SELECT COUNT(1) FROM OT_CHECKCODE WHERE STATE = 0 AND USERCODE = N'" + userCode.Replace("'", "''") + "'";
                    int c = Convert.ToInt32(Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty);
                    int waits = 0;
                    int.TryParse(wait[c >= wait.Length ? wait.Length - 1 : c], out waits);
                    string ret = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT Max(CREATETIME) FROM OT_CHECKCODE WHERE STATE = 0 AND USERCODE = N'" + userCode.Replace("'", "''") + "'") + string.Empty;
                    DateTime dt = Convert.ToDateTime(string.IsNullOrWhiteSpace(ret) ? "2019-01-01 00:00:00" : ret);
                    if (waits > 0 && (DateTime.Now - dt).TotalSeconds < waits)
                    {
                        return Json(new { Success = false, Message = "NeedCheckCode" }, JsonRequestBehavior.AllowGet);
                    }
                }
                loginResult = UserValidatorFactory.Login(
                                OThinker.Clusterware.AuthenticationType.Forms,
                                string.Empty,
                                userCode,
                                password,
                                Site.PortalType.Portal);
            }
            catch (Exception ex)
            {
                //ConnectionFailed
                if (ex.Message.Contains("ConnectionFailed"))
                {
                    return Json(new { Success = false, Message = "ConnectionFailed" }, JsonRequestBehavior.AllowGet);
                }
                else if (ex.Message.Contains("PasswordInvalid"))
                {
                    return Json(new { Success = false, Message = "EnginePasswordInvalid" }, JsonRequestBehavior.AllowGet);
                }
            }
            if (loginResult)
            {
                if (enableCheckCode != "0")
                {
                    string sql = "UPDATE OT_CHECKCODE SET STATE = 1 WHERE USERCODE = N'" + userCode.Replace("'", "''") + "'";
                    Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                }
                result = getCurrentUser();
                FormsAuthentication.SetAuthCookie(this.UserValidator.User.Code, false);
            }
            else
            {
                result = new
                {
                    Success = false,
                    Message = "用户名或密码错误"

                };
                if (enableCheckCode != "0")
                {
                    string model = "INSERT INTO OT_CHECKCODE(OBJECTID,USERCODE,IP,SYSTEMINFO,BROWSER,CODE,STATE,LOGINTIME, CREATETIME)VALUES('[OBJECTID]','" + userCode.Replace("'", "''") + "','" + Request.UserHostAddress.Replace("'", "''") + "','" + Request.UserAgent.Replace("'", "''") + "','" + Request.Browser.Browser.Replace("'", "''") + "/" + Request.Browser.Version.Replace("'", "''") + "','',[STATE],to_date('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-MM-dd HH24:mi:ss'),to_date('" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "','yyyy-MM-dd HH24:mi:ss'))";
                    string sql = model.Replace("[OBJECTID]", Guid.NewGuid().ToString()).Replace("[STATE]", "0");
                    Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                }

                UserValidatorFactory.Exit(this);
                Session.Clear();
                Session.Abandon();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 钉钉集成登录
        /// </summary>
        /// <param name="state">引擎编码</param>
        /// <param name="code">用户 H3ObjectID</param>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult ValidateLoginForDingTalkPC(string state, string code)
        {
            this.Engine.LogWriter.Write("DingTalkPC：进入登录方法-------------------------");
            OThinker.Organization.User currentUser = null;
            UserValidator userValidator = null;
            string userImage = string.Empty;
            // 钉钉登录
            IEngine engine = AppUtility.Engine;
            userValidator = UserValidatorFactory.LoginAsDingTalkPCAndReturnUserValidator(state, code);
            object result = null;
            if (userValidator == null)
            {
                result = new
                {
                    Success = false
                };
            }
            else
            {
                currentUser = userValidator.User;
                userImage = userValidator.ImagePath;
                MobileAccess mobile = new MobileAccess();
                MobileAccess.MobileUser mobileUser = mobile.GetMobileUser(userValidator, currentUser, userImage, string.Empty, string.Empty);
                FormsAuthentication.SetAuthCookie(currentUser.Code, false);
                // 当前用户登录
                Session[Sessions.GetUserValidator()] = userValidator;
                result = getCurrentUser();
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 移动登陆
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <param name="uuid"></param>
        /// <param name="jpushId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="mobileType"></param>
        /// <param name="isAppLogin"></param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public object LoginInMobile(string userCode,
            string password,
            string uuid,
            string jpushId,
            string mobileToken,
            string mobileType,
            bool isAppLogin)
        {
            OThinker.Organization.User currentUser = null;
            string userImage = string.Empty;

            OThinker.H3.Controllers.UserValidator userValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, userCode);
            password = HttpUtility.UrlDecode(password).Replace("_38;_", "&");

            if (userValidator != null)
            {
                /*
                 * 移动端登录：1.比较账号密码匹配;2.比较uuid和Token是否匹配
                 */

                OThinker.Organization.User user = Engine.Organization.GetUserByCode(userCode);
                if (user == null || user.State == OThinker.Organization.State.Inactive
                || user.ServiceState == OThinker.Organization.UserServiceState.Dismissed || user.IsVirtualUser)
                {// 虚拟用户、离职、禁用用户不允许登录
                    return new { Success = false };
                }

                if (
                    (!string.IsNullOrEmpty(uuid)
                        && userValidator.User.SID == uuid
                        && userValidator.User.ValidateMobileToken(mobileToken))
                    || userValidator.User.ValidatePassword(password)
                   )
                {
                    currentUser = userValidator.User;
                    userImage = userValidator.ImagePath;
                }
            }

            if (currentUser == null)
            {// 登录失败
                return new { Success = false };
            }
            FormsAuthentication.SetAuthCookie(currentUser.Code, false);
            // App 登录则更新 MobileToken 等信息
            if (isAppLogin)
            {
                mobileToken = this.GetMobileToken(currentUser, mobileType, uuid, jpushId);
            }

            OThinker.H3.Controllers.MobileAccess mobile = new OThinker.H3.Controllers.MobileAccess();
            MobileAccess.MobileUser mobileUser = mobile.GetMobileUser(userValidator, currentUser, userImage, userValidator.DepartmentName, mobileToken);

            var result = new
            {
                Success = true,
                PortalRoot = this.PortalRoot,
                MobileUser = mobileUser,
                User = mobileUser
            };

            System.Web.HttpContext.Current.Session[Sessions.GetUserValidator()] = userValidator;

            // 记录登录日志
            this.Engine.UserLogWriter.Write(new Tracking.UserLog()
            {
                LogType = Tracking.UserLogType.Login,
                SiteType = Site.PortalType.Mobile,
                ClientPlatform = mobileType,
                ClientAddress = jpushId,
                DeviceToken = mobileToken,
                UserCode = mobileUser.Code,
                MobileSID = uuid,
                UserID = mobileUser.ObjectID
            });
            return result;
        }

        /// <summary>
        /// 获取移动办公单点登录编码
        /// </summary>
        /// <param name="user"></param>
        /// <param name="mobileType"></param>
        /// <param name="uuid"></param>
        /// <param name="jpushId"></param>
        /// <returns></returns>
        private string GetMobileToken(OThinker.Organization.User user, string mobileType, string uuid, string jpushId)
        {
            user.SID = uuid;
            if (!string.IsNullOrEmpty(jpushId))
            {
                //保证用户JPushID的唯一性
                List<string> usersId = Engine.PortalQuery.GetUsersIdByjpushId(jpushId);
                List<Organization.Unit> units = Engine.Organization.GetUnits(usersId.ToArray());
                foreach (var unit in units)
                {
                    if (unit.ObjectID != user.ObjectID)
                    {
                        Organization.User Updateuser = unit as Organization.User;
                        Updateuser.JPushID = null;
                        this.Engine.LogWriter.Write("删除" + Updateuser.Name + "[" + Updateuser.Code + "] jpushId");
                        this.Engine.Organization.UpdateUnit(user.ObjectID, Updateuser);
                    }
                }
                // JPushId 只会在第一次登录的时候取值
                user.JPushID = jpushId;
            }

            // 生成一个Token值
            string mobileToken = Guid.NewGuid().ToString().Replace("-", string.Empty);
            user.MobileToken = mobileToken;// OThinker.Security.MD5Encryptor.GetMD5(mobileToken);
            user.MobileType = mobileType.ToLower().IndexOf("android") > -1 ? OThinker.Organization.MobileSystem.Android : OThinker.Organization.MobileSystem.iOS;
            // 更新用户的信息
            if (user.DirtyProperties.Length > 0)
            {
                this.Engine.LogWriter.Write("更新" + user.Name + "[" + user.Code + "] jpushId：" + jpushId);
                this.Engine.Organization.UpdateUnit(user.ObjectID, user);
            }
            return mobileToken;
        }

        public JsonResult DoLock()
        {
            if (this.UserValidator != null)
            {
                this.UserValidator.User.DoLock = true;
                this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, this.UserValidator.User);
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            return Json(false, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DoUnlock(string userCode,
            string password)
        {
            if (this.UserValidator != null)
            {
                if (this.UserValidator.User.ValidatePassword(password))
                {
                    this.UserValidator.User.DoLock = false;
                    this.Engine.Organization.UpdateUnit(this.UserValidator.UserID, this.UserValidator.User);
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(false, JsonRequestBehavior.AllowGet);
            //OThinker.H3.Controllers.UserValidator userValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, userCode);
            //if (userValidator != null)
            //{
            //    if (userValidator.User.ValidatePassword(password))
            //    {
            //        userValidator.User.DoLock = false;
            //        this.Engine.Organization.UpdateUnit(OThinker.Organization.User.AdministratorID, userValidator.User);
            //        return Json(true, JsonRequestBehavior.AllowGet);
            //    }
            //}
            //return Json(false, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前用户
        /// </summary>
        /// <returns>返回当前已登录的用户信息</returns>
        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetCurrentUser()
        {
            var result = this.getCurrentUser();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取当前已登录的用户信息
        /// </summary>
        /// <returns></returns>
        private object getCurrentUser()
        {
            OThinker.Organization.User user = null;
            List<FunctionViewModel> functions = null;

            if (this.UserValidator != null)
            {
                user = this.UserValidator.User;
                user.Password = string.Empty;
                functions = this.getFunctionApps();
                user.ImageUrl = GetUserImageUrl(user);
            }
            var result = new
            {
                Success = user != null,
                Functions = functions,
                User = user,
                PortalRoot = this.PortalRoot,
                ManagerName = GetUserManagerName(user),
                OUDepartName = GetUserParentName(user),
                chkEmail = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.Email),
                chkMobileMessage = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.MobileMessage),
                chkWeChat = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.WeChat),
                chkApp = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.App),
                chkDingTalk = user == null ? false : user.CheckNotifyType(OThinker.Organization.NotifyType.DingTalk),
                DirectoryUnits = user == null ? null : GetDirectoryUnits(user.ObjectID),
                UserRole = user == null ?"": GetUserRole(user.ObjectID)

            };
            return result;
        }

        /// <summary>
        /// 获取应用App的集合
        /// </summary>
        /// <returns></returns>
        private List<FunctionViewModel> getFunctionApps()
        {
            //门户可见、固定到首页
            List<FunctionViewModel> functions = new List<FunctionViewModel>();
            if (this.UserValidator != null)
            {
                OThinker.H3.Apps.AppNavigation[] AllApps = this.Engine.AppNavigationManager.GetAllApps();
                Dictionary<string, OThinker.H3.Apps.AppNavigation> appNavigation = new Dictionary<string, Apps.AppNavigation>();
                foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
                {
                    foreach (OThinker.H3.Apps.AppNavigation app in AllApps)
                    {
                        if (functionNode.Code == app.AppCode)
                        {
                            if (!appNavigation.ContainsKey(functionNode.Code))
                            {
                                appNavigation.Add(functionNode.Code, app);
                                break;
                            }
                        }
                    }
                }

            
                foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
                {
                    if (functionNode.NodeType == OThinker.H3.Acl.FunctionNodeType.AppNavigation)
                    {
                        if (!appNavigation.ContainsKey(functionNode.Code)) continue;
                        OThinker.H3.Apps.AppNavigation appNav = appNavigation[functionNode.Code];
                        if (appNav.VisibleOnPortal == OThinker.Data.BoolMatchValue.True)
                        {
                            this.TopAppCode = functionNode.Code;
                            FunctionViewModel parentViewModel = new FunctionViewModel()
                            {
                                Code = functionNode.Code,
                                TopAppCode = this.TopAppCode,
                                DisplayName = functionNode.DisplayName,
                                IconUrl = functionNode.IconUrl,
                                IconCss = functionNode.IconCss,
                                SortKey = functionNode.SortKey,
                                Url = functionNode.Url,
                                Children = new List<FunctionViewModel>(),
                                DockOnHomePage = (appNav != null && appNav.DockOnHomePage == OThinker.Data.BoolMatchValue.True)
                            };
                            functions.Add(parentViewModel);
                            // 递归获取子节点
                            this.getChildFunctions(functionNode, parentViewModel);
                        }
                    }
                }

                //foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
                //{
                //    if (functionNode.NodeType == OThinker.H3.Acl.FunctionNodeType.AppNavigation)
                //    {
                //        FunctionViewModel parentViewModel = new FunctionViewModel()
                //        {
                //            Code = functionNode.Code,
                //            DisplayName = functionNode.DisplayName,
                //            IconCss = functionNode.IconCss,
                //            SortKey = functionNode.SortKey,
                //            Url = functionNode.Url,
                //            Children = new List<FunctionViewModel>()
                //        };
                //        functions.Add(parentViewModel);
                //        // 递归获取子节点
                //        this.getChildFunctions(functionNode, parentViewModel);
                //    }
                //}
            }
            return functions;
        }
        
      

        /// <summary>
        /// 递归获取子节点
        /// </summary>
        /// <param name="parentNode"></param>
        /// <param name="parentViewModel"></param>
        private void getChildFunctions(OThinker.H3.Acl.FunctionNode parentNode,
            FunctionViewModel parentViewModel)
        {
            foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
            {
                if (functionNode.ParentCode == parentNode.Code)
                {
                    FunctionViewModel viewModel = new FunctionViewModel()
                    {
                        Code = functionNode.Code,
                        DisplayName = functionNode.DisplayName,
                        TopAppCode = this.TopAppCode,
                        IconCss = functionNode.IconCss,
                        SortKey = functionNode.SortKey,
                        Url = functionNode.Url,
                        Children = new List<FunctionViewModel>()
                    };
                    parentViewModel.Children.Add(viewModel);

                    // 递归获取子节点
                    this.getChildFunctions(functionNode, viewModel);
                }
            }
        }

        /// <summary>
        /// 系统退出登录操作
        /// </summary>
        [HttpGet]
        public void LoginOut()
        {
            UserValidatorFactory.Exit(this);
            Session.Clear();
            FormsAuthentication.SignOut();
            Session.Abandon();
        }

        [HttpGet]
        public string LogoutMobile(string callback)
        {
            this.LoginOut();
            ActionResult result = new ActionResult(true);
            this.UserValidator.User.MobileToken = "";
            result.Success = this.Engine.Organization.UpdateUnit(this.UserValidator.User.ObjectID, this.UserValidator.User) == OThinker.Organization.HandleResult.SUCCESS;
            string jsonp = string.Format("{0}({1})", callback, Newtonsoft.Json.JsonConvert.SerializeObject(result));
            return jsonp;

        }
        /// <summary>
        /// 获取当前用户可见的菜单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadFunctions()
        {
            OThinker.Organization.User user = null;

            var result = new
            {
                Success = true,
                User = user
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 组织机构权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// 菜单所属顶层node code
        /// </summary>
        private string _TopAppCode;
        public string TopAppCode
        {
            get
            {
                if (string.IsNullOrEmpty(_TopAppCode)) return "";
                return _TopAppCode;
            }
            set
            {
                _TopAppCode = value;
            }
        }

        public int ExecuteCountSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                var count = command.ExecuteScalar(sql);
                return Convert.ToInt32(count);
            }
            return 0;
        }
    }
}