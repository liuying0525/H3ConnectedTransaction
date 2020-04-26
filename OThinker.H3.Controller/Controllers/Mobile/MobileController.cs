using OThinker.H3.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Mobile
{
    public class MobileController : MobileAccess
    {
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// 底部菜单 待办待阅数量
        /// </summary>
        /// <returns></returns>
        public JsonResult GetWorkItemCount()
        {
            return ExecuteFunctionRun(() =>
            {
                string UserID = this.UserValidator.UserID;

                ActionResult result = new ActionResult(true);
                result.Extend = this.GetWorkItemCountCommon(UserID);

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取待办，已办任务列表，不区分优先级
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="sortKey"></param>
        /// <param name="sortDirection"></param>
        /// <param name="finishedWorkItem"></param>
        /// <param name="finishedWorkItem"></param>
        /// <param name="finishedWorkItem"></param>
        /// <returns></returns>
        public JsonResult GetWorkItems(WorkItemQueryParams Params)
        {
            return ExecuteFunctionRun(() =>
                {
                    //int returnCount = 10;//返回数量，默认为10；
                    //Params.returnCount = 10;
                    var Items = this.GetLoadWorkItems(Params);
                    return Json(Items, JsonRequestBehavior.AllowGet);
                });
        }


        /// <summary>
        /// 获取待办，已办任务列表，按高优先级，普通区分
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="sortKey"></param>
        /// <param name="sortDirection"></param>
        /// <param name="finishedWorkItem"></param>
        /// <returns></returns>
        public JsonResult GetWorkItemsByPriority(WorkItemQueryParams Params)
        {
            //int returnCount = 10;
            //Params.returnCount = 10;//返回数量，默认为10；
            bool showNormal = true;//是否需要查询普通优先级
            LoadWorkItemsClass NormalPriorityItems = new H3.Controllers.MobileAccess.LoadWorkItemsClass();
            var HighPriorityItems = this.GetLoadWorkItems(Params);

            if (HighPriorityItems != null && HighPriorityItems.WorkItems.Count == 10)
            {
                showNormal = false;

            }
            else if (HighPriorityItems != null && HighPriorityItems.WorkItems.Count < 10)
            {
                //Params.returnCount = 10 - HighPriorityItems.WorkItems.Count;
            }

            if (showNormal)
            {
                NormalPriorityItems = this.GetLoadWorkItems(Params);
            }

            var result = new
            {
                HighPriorityItems = HighPriorityItems,
                NormalPriorityItems = NormalPriorityItems
            };

            return Json(result, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 刷新待办任务，不区分优先级
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyword"></param>
        /// <param name="highLastTime"></param>
        /// <param name="normalLastTime"></param>
        /// <param name="finishedWrokItem"></param>
        /// <param name="existsLengthHigh"></param>
        /// <param name="existsLenthNormal"></param>
        /// <returns></returns>
        public JsonResult GetRefreshWorkItems(string userId, string mobileToken, string keyword, DateTime LastTime,
            bool finishedWorkItem, int existsLength)
        {
            int returnCount = 10;//返回数量，默认为10；
            if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet);
            var FinishedItems = this.GetRefreshWorkItemsFn(userId, mobileToken, keyword, LastTime, finishedWorkItem, existsLength, Instance.PriorityType.Unspecified, null, null, returnCount);
            return Json(FinishedItems, JsonRequestBehavior.AllowGet);


        }

        /// <summary>
        /// 刷新待办任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyword"></param>
        /// <param name="highLastTime"></param>
        /// <param name="normalLastTime"></param>
        /// <param name="finishedWrokItem"></param>
        /// <param name="existsLengthHigh"></param>
        /// <param name="existsLenthNormal"></param>
        /// <returns></returns>
        public JsonResult GetRefreshWorkItemsByPriority(string userId, string mobileToken, string keyword, DateTime highLastTime,
            DateTime normalLastTime, bool finishedWorkItem, int existsLengthHigh, int existsLenthNormal)
        {
            int returnCount = 10;//返回数量，默认为10；
            bool showNormal = true;//是否需要查询普通优先级

            RefreshWorkItemsClass NormalRefreshWrokitem = new H3.Controllers.MobileAccess.RefreshWorkItemsClass();
            var HighRefreshWrokitem = this.GetRefreshWorkItemsFn(userId, mobileToken, keyword, highLastTime, finishedWorkItem, existsLengthHigh, Instance.PriorityType.High, DateTime.Parse("2004-01-01"), DateTime.MaxValue, returnCount);

            if (HighRefreshWrokitem != null && HighRefreshWrokitem.WorkItems.Count == 10)
            {
                showNormal = false;
            }
            else if (HighRefreshWrokitem != null && HighRefreshWrokitem.WorkItems.Count < 10)
            {
                returnCount = 10 - HighRefreshWrokitem.WorkItems.Count;
            }

            if (showNormal)
            {
                NormalRefreshWrokitem = this.GetRefreshWorkItemsFn(userId, mobileToken, keyword, highLastTime, finishedWorkItem, existsLengthHigh, Instance.PriorityType.Normal, DateTime.Parse("2004-01-01"), DateTime.MaxValue, returnCount);
            }

            var result = new
            {
                HighRefreshWrokitem = HighRefreshWrokitem,
                NormalRefreshWrokitem = NormalRefreshWrokitem
            };

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #region 我的流程

        /// <summary>
        /// 加载所有的流程
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult LoadAllInstances(InstanceQueryParams Params)
        {
            return ExecuteFunctionRun(() =>
                {
                    //if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;
                    var result = this.LoadAllInstanceList(Params);
                    return Json(result, JsonRequestBehavior.AllowGet);
                });
        }
        [HttpGet]
        public JsonResult LoadInstances(InstanceQueryParams Params)
        {
            return ExecuteFunctionRun(() =>
            {
                LoadWorkItemsClass result = new LoadWorkItemsClass();
                if (Params.status == (int)InstanceState.Canceled)
                {
                    result = LoadCanceledInstance(Params);
                }
                else if (Params.status == (int)InstanceState.Finished)
                {
                    result = LoadFinishedInstance(Params);
                }
                else
                {
                    result = LoadUnFinishedInstance(Params);
                }
                //lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差
                //var result = LoadFinishedInstance(userID, lastTime, mobileToken, sortKey, sortDirection, isPriority, startDate, endDate, keyWord);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 加载更多数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord">关键字</param>
        /// <param name="instanceState">流程状态</param>
        /// <param name="lastTime">上次加载时间</param>
        /// <returns>流程数据</returns>
        [HttpGet]
        public JsonResult LoadInstancesByState(string userId, string mobileToken, string keyWord, int instanceState, int loadStart)
        {
            userId = AppUtility.RegParam(userId);
            if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;
            var result = this.LoadInstances(userId, Server.UrlDecode(keyWord), instanceState, loadStart);
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 刷新流程数据
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord">关键字</param>
        /// <param name="instanceState">流程状态</param>
        /// <param name="lastTime">上次刷新时间</param>
        /// <returns>流程数据</returns>
        [HttpGet]
        public JsonResult RefreshInstancesByState(string userId, string mobileToken, string keyWord, int instanceState, DateTime lastTime)
        {
            userId = AppUtility.RegParam(userId);
            if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;
            var result = this.RefreshInstancesByState(userId, keyWord, lastTime, instanceState);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 待阅、已阅
        public JsonResult LoadCirculateItems(CirculateItemQueryParams Params)
        {
            return ExecuteFunctionRun(() =>
            {
                //userId = AppUtility.RegParam(userId);
                //keyWord = AppUtility.RegParam(keyWord);
                //if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;
                //if (!Params.lastTime.HasValue)
                //    Params.lastTime = DateTime.Now;
                //Params.returnCount = 10;
                var result = this.LoadCirculateItemsFn(Params);
                if (result == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        public JsonResult RefreshCirculateItems(string userId,
        string mobileToken,
        string keyWord,
        DateTime lastTime,
        bool readWorkItem,
        int existsLength)
        {
            userId = AppUtility.RegParam(userId);
            keyWord = AppUtility.RegParam(keyWord);
            if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;

            var result = this.RefreshCirculateItemFn(userId, mobileToken, keyWord, lastTime, readWorkItem, existsLength);
            if (result == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ReadCirculateItems(string userId,
           string mobileToken,
           string CirculateItemIDs,
           bool ReadAll)
        {
            userId = AppUtility.RegParam(userId);
            CirculateItemIDs = AppUtility.RegParam(CirculateItemIDs);
            if (this.UserValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;

            var result = this.ReadCirculateItemsByBatchFn(userId, mobileToken, CirculateItemIDs, ReadAll);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 发起流程
        public JsonResult LoadWorkflows(string userCode, string mobileToken)
        {
            UserValidator userValidate = this.UserValidator;
            if (userValidate == null)
            {
                return Json(new { }, JsonRequestBehavior.AllowGet);
            }
            var result = this.LoadWorkflowsFn(userValidate);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void SetFavorite(string userId, string mobileToken, string workflowCode, bool isFavorite)
        {
            //if (!ValidateMobileToken(userId, mobileToken)) return;
            if (isFavorite)
            {
                this.Engine.WorkflowConfigManager.AddFavoriteWorkflow(userId, workflowCode);
            }
            else
            {
                this.Engine.WorkflowConfigManager.DeleteFavoriteWorkflow(userId, workflowCode);
            }
        }
        #endregion

        #region 通讯录
        public JsonResult GetOrganizationByParent(string userId, string userCode, string mobileToken, string parentId)
        {
            UserValidator userValidator = this.UserValidator; ;
            if (userValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet); ;

            var result = this.GetOrganizationByParentFn(userValidator, userCode, mobileToken, parentId);
            if (result == null) return Json(new { }, JsonRequestBehavior.AllowGet);

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUserByObjectID(string userCode, string mobileToken, string targetUserId)
        {
            UserValidator sourceUser = this.UserValidator;
            if (sourceUser == null) return Json(new { }, JsonRequestBehavior.AllowGet);

            OThinker.Organization.User user = this.Engine.Organization.GetUnit(targetUserId) as OThinker.Organization.User;
            if (user == null) return Json(new { }, JsonRequestBehavior.AllowGet);

            UserValidator userValidator = UserValidatorFactory.GetUserValidator(this.Engine, user.Code);
            MobileAccess mobile = new MobileAccess();
            MobileAccess.MobileUser mobileUser = mobile.GetMobileUser(sourceUser, user, user.ImageUrl, userValidator.DepartmentName, string.Empty);
            var result = new
            {
                MobileUser = mobileUser
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SearchUser(string userCode, string mobileToken, string parentId, string searchKey)
        {
            UserValidator userValidator = this.UserValidator;
            if (userValidator == null) return Json(new { }, JsonRequestBehavior.AllowGet);
            var result = this.SearchUserFn(userCode, mobileToken, parentId, searchKey);
            if (result == null) return Json(new { }, JsonRequestBehavior.AllowGet);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        #endregion

        #region 数据模型
        public JsonResult GetDataModelData(string userId, string DataModelCode, string QueryCode, string SortBy, int FromRowNum, int ShowCount)
        {
            ActionResult result = new ActionResult(true);
            result.Extend = this.GetDataModelDataFn(userId, DataModelCode, QueryCode, SortBy, FromRowNum, ShowCount);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 密码设置
        public JsonResult SetPassword(string userCode, string mobileToken, string oldPassword, string newPassword)
        {
            oldPassword = System.Web.HttpUtility.UrlDecode(oldPassword).Replace("_38;_", "&");
            newPassword = System.Web.HttpUtility.UrlDecode(newPassword).Replace("_38;_", "&");
            ActionResult result = new ActionResult(true);
            UserValidator userValidate = this.UserValidator;//ValidateUserMobileToken(userCode, mobileToken);
            if (userValidate == null)
            {
                result.Success = false;
                result.Message = "用户不存在";
            }
            else
            {
                result = this.SetPasswordFn(userCode, oldPassword, newPassword);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion



        /// <summary>
        /// 验证获取信息的用户身份
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <returns></returns>
        private UserValidator ValidateUserMobileToken(string userCode, string mobileToken)
        {
            UserValidator userValidator = UserValidatorFactory.GetUserValidator(this.Engine, userCode);

            if (userValidator == null) return null;

            if (userValidator.User.MobileToken == OThinker.Security.MD5Encryptor.GetMD5(mobileToken))
            {
                return userValidator;
            }
            return null;
        }
        /// <summary>
        /// 验证用户身份
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <returns></returns>
        private bool ValidateMobileToken(string userId, string mobileToken)
        {
            OThinker.Organization.User user = this.Engine.Organization.GetUnit(userId) as OThinker.Organization.User;
            return user == null ? false : user.ValidateMobileToken(mobileToken);
        }
        [HttpGet]
        public JsonResult TestMsg()
        {
            try
            {
                List<Notification.Notification> notifications = new List<Notification.Notification>();
                Notification.Notification n = new Notification.Notification()
                {
                    SenderID = this.UserValidator.UserID,
                    ReceiverID = this.UserValidator.UserID,
                    SendTime = System.DateTime.Now,
                    State = Notification.NotificationState.Unread,
                    Title = "回复了您: ",
                    SchemaCode = "",
                    BizObjectId = "",
                    Type = Notification.NotifyType.Internal
                };
                notifications.Add(n);
                this.Engine.Notifier.Send(notifications.ToArray());
                return Json("success");
            }
            catch (Exception ex)
            {
                return Json("false");
            }
        }
        [HttpGet]
        public JsonResult AddUrgeInstance(string InstanceID, string UrgeContent)
        {
            return this.ExecuteFunctionRun(() =>
            {
                var result = base.AddUrgeInstance(InstanceID, UrgeContent);
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }
        [HttpGet]
        public JsonResult GetAppList(string AppCode)
        {
            return this.ExecuteFunctionRun(() =>
            {
                var result = string.IsNullOrEmpty(AppCode)?base.FetchAllApps(this.UserValidator.UserID):base.GetApps(AppCode);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        [HttpGet]
        public JsonResult GetFunctions(String AppCode)
        {
            return this.ExecuteFunctionRun(() => {
                var result = base.FetchFunction(AppCode);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult CancelInstance(string InstanceID)
        {
            return this.ExecuteFunctionRun(() =>
            {

                var result = base.CancelInstance(InstanceID);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        [HttpGet]
        public JsonResult SetFavoriteApp(bool isFavorite,string appCode)
        {
            return this.ExecuteFunctionRun(() => {
                var result = base.SetFavoriteApp(this.UserValidator.UserID, appCode, isFavorite);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
