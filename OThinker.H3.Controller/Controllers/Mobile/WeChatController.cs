using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace OThinker.H3.Controllers
{
    public class WeChatController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public WeChatController()
        {
        }
        public override string FunctionCode
        {
            get { return ""; }
        }
        /// <summary>
        /// 微信集成登录
        /// </summary>
        /// <param name="context"></param>
        public JsonResult ValidateLoginForWeChat(string state, string code)
        {
            OThinker.Organization.User currentUser = null;
            UserValidator userValidator = null;
            string userImage = string.Empty;
            // 微信登录
            if (this.UserValidator != null)
            {
                userValidator = this.UserValidator;
            }
            else
            {
                IEngine engine = AppUtility.Engine;
                userValidator = UserValidatorFactory.LoginAsWeChatReturnUserValidator(state, code);
            }

            object result = null;
            if (userValidator == null)
            {
                result = new
                {
                    Success = false,
                    Messages = "UserNotExist"
                };
            }
            else
            {
                currentUser = userValidator.User;
                if (currentUser == null
                    || currentUser.State == State.Inactive
                    || currentUser.ServiceState == UserServiceState.Dismissed
                    || currentUser.IsVirtualUser)
                {
                    result = new
                    {
                        Success = false,
                        Messages = "InvalidUser"
                    };
                }

                userImage = userValidator.ImagePath;
                MobileAccess mobile = new MobileAccess();
                MobileAccess.MobileUser mobileUser = mobile.GetMobileUser(userValidator, currentUser, userImage, string.Empty, string.Empty);
                result = new
                {
                    Success = true,
                    PortalRoot = this.PortalRoot,
                    MobileUser = mobileUser,
                    DirectoryUnits = GetDirectoryUnits(currentUser.ObjectID, userValidator)
                };
                FormsAuthentication.SetAuthCookie(currentUser.Code, false);
                // 当前用户登录
                Session[Sessions.GetUserValidator()] = userValidator;
                Session[Sessions.GetWeChatLogin()] = true;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 加载用户的待办任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="lastTime">最后时间</param>
        /// <param name="sortKey">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="finishedWorkItem">是否已办任务</param>
        public JsonResult LoadWorkItems(string userId, string mobileToken, string keyWord, DateTime? lastTime, string sortKey, string sortDirection, bool finishedWorkItem)
        {
            return ExecuteFunctionRun(() =>
            {
                if (userId.IndexOf("'") > -1 || userId.IndexOf("--") > -1)
                    return Json("", JsonRequestBehavior.AllowGet); ;
                if (keyWord.IndexOf("'") > -1 || keyWord.IndexOf("--") > -1)
                    return Json("", JsonRequestBehavior.AllowGet); ;

                MobileAccess mobile = new MobileAccess();
                var result = mobile.LoadWorkItems(userId, mobileToken, keyWord,
                    lastTime == null ? DateTime.MinValue : lastTime.Value,
                    sortKey, sortDirection, finishedWorkItem);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 刷新用户的待办任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        /// <param name="finishedWorkItem">是否加载已办任务</param>
        /// <param name="existsLength">返回待办任务ID的数量</param>
        public JsonResult RefreshWorkItem(string userId, string mobileToken, string keyWord, DateTime? lastTime, bool finishedWorkItem, int existsLength)
        {
            return ExecuteFunctionRun(() =>
            {
                existsLength = int.Parse(existsLength + string.Empty == "" ? "0" : existsLength + string.Empty);

                if (userId.IndexOf("'") > -1 || userId.IndexOf("--") > -1) return Json("", JsonRequestBehavior.AllowGet); ;

                MobileAccess mobile = new MobileAccess();
                var result = mobile.RefreshWorkItem(userId, mobileToken, keyWord,
                    lastTime == null ? DateTime.MinValue : lastTime.Value,
                    finishedWorkItem, existsLength);
                if (result == null) return Json("", JsonRequestBehavior.AllowGet); ;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult RefreshWorkItems(string userId, string mobileToken, string keyWord, DateTime? lastTime, bool finishedWorkItem)
        {
            return RefreshWorkItem(userId, mobileToken, keyWord, lastTime, finishedWorkItem, 0);
        }


        public JsonResult RefreshInstances(string userId, string mobileToken, string keyWord, DateTime? lastTime)
        {
            return ExecuteFunctionRun(() =>
             {
                 if (userId.IndexOf("'") > -1 || userId.IndexOf("--") > -1) return Json("", JsonRequestBehavior.AllowGet);
                 MobileAccess mobile = new MobileAccess();
                 var result = mobile.RefreshInstances(userId, mobileToken, keyWord,
                     lastTime == null ? DateTime.MinValue : lastTime.Value);
                 if (result == null) return Json("", JsonRequestBehavior.AllowGet);
                 return Json(result, JsonRequestBehavior.AllowGet);
             });
        }

        public JsonResult LoadInstances(string userId, string mobileToken, string keyWord, DateTime? lastTime, string sortKey, string sortDirection)
        {
            return ExecuteFunctionRun(() =>
            {
                if (userId.IndexOf("'") > -1 || userId.IndexOf("--") > -1) Json("", JsonRequestBehavior.AllowGet); ;
                if (keyWord.IndexOf("'") > -1 || keyWord.IndexOf("--") > -1) Json("", JsonRequestBehavior.AllowGet); ;

                MobileAccess mobile = new MobileAccess();
                var result = mobile.LoadInstances(userId, mobileToken, keyWord,
                    lastTime == null ? DateTime.MinValue : lastTime.Value,
                    sortKey, sortDirection);
                if (result == null) Json("", JsonRequestBehavior.AllowGet); ;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult GetOrganizationByParent(string userCode, string mobileToken, string parentId)
        {
            return ExecuteFunctionRun(() =>
            {
                MobileAccess mobile = new MobileAccess();
                var result = mobile.GetOrganizationByParentFn(this.UserValidator, userCode, mobileToken, parentId);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="context"></param>
        public JsonResult SearchUser(string userCode, string mobileToken, string searchKey, string parentId)
        {
            return ExecuteFunctionRun(() =>
            {
                MobileAccess mobile = new MobileAccess();
                var result = mobile.SearchUserFn(userCode, "", parentId, searchKey);
                if (result == null) return Json("", JsonRequestBehavior.AllowGet); ;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <param name="targetUserId"></param>
        public JsonResult GetUserByObjectID(string userCode, string targetUserId)
        {
            return ExecuteFunctionRun(() =>
            {
                OThinker.Organization.User user = this.Engine.Organization.GetUnit(targetUserId) as OThinker.Organization.User;
                if (user == null) Json("", JsonRequestBehavior.AllowGet); ;

                UserValidator userValidator = UserValidatorFactory.GetUserValidator(this.Engine, user.Code);
                MobileAccess mobile = new MobileAccess();
                MobileAccess.MobileUser mobileUser = mobile.GetMobileUser(this.UserValidator, user,
                    userValidator.ImagePath,
                    userValidator.DepartmentName,
                    string.Empty);

                var result = new
                {
                    MobileUser = mobileUser
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult LoadWorkflows()
        {
            return ExecuteFunctionRun(() =>
            {
                MobileAccess mobile = new MobileAccess();
                var result = mobile.LoadWorkflowsFn(this.UserValidator);
                if (result == null) return Json("", JsonRequestBehavior.AllowGet);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public void SetFavorite(string userId, string workflowCode, bool isFavorite)
        {

            if (isFavorite)
            {
                this.Engine.WorkflowConfigManager.AddFavoriteWorkflow(userId, workflowCode);
            }
            else
            {
                this.Engine.WorkflowConfigManager.DeleteFavoriteWorkflow(userId, workflowCode);
            }

        }

        public JsonResult LoadInstanceState(string userId, string userCode, string mobileToken, string instanceId, string workItemId)
        {
            userId = this.UserValidator.UserID;
            return ExecuteFunctionRun(() =>
            {
                MobileAccess mobile = new MobileAccess();
                var result = mobile.LoadInstanceState(userId, mobileToken, instanceId, this.UserValidator, workItemId);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
