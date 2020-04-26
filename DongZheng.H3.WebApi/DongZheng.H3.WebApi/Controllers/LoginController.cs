using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;

namespace DongZheng.H3.WebApi.Controllers
{
    public class LoginController : OThinker.H3.Controllers.ControllerBase
    {
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
        public override string FunctionCode => throw new NotImplementedException();

        public string Test()
        {

            return "hello world";
        }

        /// GET: Login
        /// <summary>
        /// 系统登录
        /// </summary>
        /// <param name="userCode">用户账号</param>
        /// <param name="password">用户密码</param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        public JsonResult SecureLogin(string userCode, string password)
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
                                OThinker.H3.Site.PortalType.Portal);
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


                #region 校验密码复杂度

                //Regex reg = new Regex(@"^(((?=.*[0-9])(?=.*[a-zA-Z]))|((?=.*[0-9])(?=.*[!@#$%\^&*\(\)]))|((?=.*[a-zA-Z])(?=.*[!@#$%\^&*\(\)]))).{6,16}$", RegexOptions.None);

                if (!RegValidate(password))
                {
                    UserValidatorFactory.Exit(this);
                    Session.Clear();
                    Session.Abandon();

                    result = new
                    {
                        Success = false,
                        ErrorCode = 1,
                        Message = "密码复杂度不符合要求"
                    };
                }
                else
                {
                    result = getCurrentUser();
                    FormsAuthentication.SetAuthCookie(this.UserValidator.User.Code, false);
                }

                #endregion
            }
            else
            {
                result = new
                {
                    Success = false,
                    ErrorCode = 2,
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
                UserRole = user == null ? "" : GetUserRole(user.ObjectID)

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
                Dictionary<string, OThinker.H3.Apps.AppNavigation> appNavigation = new Dictionary<string, OThinker.H3.Apps.AppNavigation>();
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
        /// 修改密码
        /// </summary>
        /// <param name="old_pwd"></param>
        /// <param name="NewPassword"></param>
        /// <returns></returns>
        public JsonResult SetPassword(string user_code, string old_pwd, string new_pwd)
        {
            var result = false;
            //验证
            old_pwd = old_pwd.Trim();
            bool success = UserValidatorFactory.Login(OThinker.Clusterware.AuthenticationType.Forms, null, user_code, old_pwd, OThinker.H3.Site.PortalType.Portal);
            if (success)
            {
                //this.UserValidator.User.Password = new_pwd;
                var u = Engine.Organization.GetUserByCode(user_code);
                u.Password = new_pwd;
                //var user_updated = Engine.Organization.GetUnit(u.ObjectID);
                Engine.Organization.UpdateUnit(user_code, u);
                result = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);

        }

        bool RegValidate(string v)
        {
            //数字 (?=.*[0-9])
            //字母 (?=.*[a-zA-Z])
            //特殊符号 ((?=[\x21-\x7e]+)[^A-Za-z0-9])
            //@"^(((?=.*[0-9])(?=.*[a-zA-Z]))|((?=.*[0-9])((?=[\x21-\x7e]+)[^A-Za-z0-9]))|((?=.*[a-zA-Z])((?=[\x21-\x7e]+)[^A-Za-z0-9]))).{6,16}$";

            Regex reg_num = new Regex(@"(?=.*[0-9])", RegexOptions.None);
            Regex reg_en = new Regex(@"(?=.*[a-zA-Z])", RegexOptions.None);
            Regex reg_sign = new Regex(@"((?=[\x21-\x7e]+)[^A-Za-z0-9])", RegexOptions.None);

            //长度
            var len = (v.Length >= 6 && v.Length <= 16) ? true : false;
            //数字
            var num = reg_num.Match(v).Success ? true : false;
            //字母
            var en = reg_en.Match(v).Success ? true : false;
            //特殊符号
            var sign = reg_sign.Match(v).Success ? true : false;
            //true 通过

            var ret_result = false;
            if (len)
            {
                if (num)
                {
                    if (en || sign)
                    {
                        ret_result = true;
                    }
                    else
                    {
                        ret_result = false;
                    }
                }
                else if (en && sign)
                {
                    ret_result = true;
                }
                else
                {
                    ret_result = false;
                }
            }
            else
            {
                ret_result = false;
            }

            return ret_result;
        }
    }
}