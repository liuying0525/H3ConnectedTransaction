using OThinker.H3.Notification;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace OThinker.H3.Controllers
{
    public class DingTalkController : ControllerBase
    {

        public override string FunctionCode
        {
            get { return ""; }
        }


        public IDingTalkAdapter _Adapter;
        public IDingTalkAdapter DingAdapter
        {
            get
            {
                if (_Adapter != null) { return _Adapter; }
                else
                {
                    _Adapter = this.Engine.DingTalkAdapter;
                    return _Adapter;
                }
            }
        }

        private string _ranNoce;
        public string RanNoce
        {
            get
            {
                if (string.IsNullOrEmpty(_ranNoce))
                {
                    _ranNoce = this.randNonce();
                    return _ranNoce;
                }
                return _ranNoce;
            }
        }

        private string _timeStamp;
        public string TimeStamp
        {
            get
            {
                if (string.IsNullOrEmpty(_timeStamp))
                {
                    _timeStamp = this.timeStamp();
                    return _timeStamp;
                }
                return _timeStamp;
            }
        }

        /// <summary>
        /// 返回一个八位的随机号，用于签名
        /// </summary>
        /// <returns></returns>
        private string randNonce()
        {
            var result = "";
            var random = new Random((int)DateTime.Now.Ticks);
            const string textArray = "123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            for (var i = 0; i < 8; i++)
            {
                result = result + textArray.Substring(random.Next() % textArray.Length, 1);
            }
            return result;
        }

        /// <summary>
        /// 时间戳的随机数,用户签名
        /// </summary>
        /// <returns></returns>
        private string timeStamp()
        {
            DateTime dt1 = Convert.ToDateTime("1970-01-01 00:00:00");
            TimeSpan ts = DateTime.Now - dt1;
            return System.Math.Ceiling(ts.TotalSeconds).ToString();
        }


        /// <summary>
        /// 获取签名字符串hash值
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string GetSignaure(string url)
        {
            string string1 = "jsapi_ticket={0}&noncestr={1}&timestamp={2}&url={3}";
            string nonce = RanNoce;
            string timestamp = TimeStamp;
            string ticket = DingAdapter.GetJSAPITicket();
            string1 = string.Format(string1, ticket, nonce, timestamp,System.Web.HttpUtility.UrlDecode(url));
            string signature = FormsAuthentication.HashPasswordForStoringInConfigFile(string1, "SHA1").ToLower();
            this.Engine.LogWriter.Write("DingTalk Signaure:" + signature);
            return signature;
        }

        /// <summary>
        /// 获取根虚拟目录
        /// </summary>
        /// <returns></returns>
        public JsonResult GetPortalRoot()
        {
            var pr = new { PortalRoot = this.PortalRoot };
            return Json(pr, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 签名信息,供前端做签名校验
        /// </summary>
        /// <returns></returns>
        public JsonResult GetSignConfig(string url)
        {
            return ExecutionActionWithLog("DingTalkController.GetSignConfig", () =>
            {
                string agentID = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDAgentId);
                string corpId = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DDCorpID);
                var config = new
                {
                    agentId = agentID,
                    corpId = corpId,
                    timeStamp = TimeStamp,
                    nonce = RanNoce,
                    signature = GetSignaure(url)
                };
                return Json(config, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 钉钉集成登录
        /// </summary>
        /// <param name="state">引擎编码</param>
        /// <param name="code">临时授权码</param>
        public JsonResult ValidateLoginForDingTalk(string state, string code)
        {
            return ExecutionActionWithLog("DingTalkController.ValidateLoginForDingTalk", () =>
            {
                OThinker.Organization.User currentUser = null;
                UserValidator userValidator = null;
                string userImage = string.Empty;
                // 钉钉登录
                IEngine engine = AppUtility.Engine;
                userValidator = UserValidatorFactory.LoginAsDingTalkReturnUserValidator(state, code);
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
                    result = new
                    {
                        Success = true,
                        PortalRoot = this.PortalRoot,
                        MobileUser = mobileUser,
                        DirectoryUnits = GetDirectoryUnits(mobileUser.ObjectID,userValidator)
                    };
                    FormsAuthentication.SetAuthCookie(currentUser.Code, false);
                    // 当前用户登录
                    Session[Sessions.GetUserValidator()] = userValidator;
                    Session[Sessions.GetDingTalkLogin()] = true;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 钉钉集成登录,使用用户ObjectID登录
        /// </summary>
        /// <param name="state">引擎编码</param>
        /// <param name="code">用户 H3 ObjectID</param>
        public JsonResult ValidateLoginForDingTalkMobile(string state, string code)
        {
            this.Engine.LogWriter.Write("DingTalkMobile：进入登录方法-------------------------");
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
                result = new
                {
                    Success = true,
                    MobileUser = mobileUser
                };
                FormsAuthentication.SetAuthCookie(currentUser.Code, false);
                // 当前用户登录
                Session[Sessions.GetUserValidator()] = userValidator;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

    }
}
