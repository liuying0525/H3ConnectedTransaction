<%@ WebService Language="C#" Class="OThinker.H3.Portal.m" %>

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Data;
using OThinker.Organization;
using System.Web.Script.Services;
using OThinker.H3.Controllers;
//using System.DirectoryServices;

namespace OThinker.H3.Portal
{
    /// <summary>
    /// 流程实例操作相关接口
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    [System.Web.Script.Services.ScriptService]
    //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
    // [System.Web.Script.Services.ScriptService]
    public class m : System.Web.Services.WebService
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public m()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }

        public IEngine _Engine = null;
        /// <summary>
        /// 获取引擎实例对象
        /// </summary>
        public IEngine Engine
        {
            get
            {
                if (_Engine == null)
                {
                    _Engine = OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
        }

        private System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        /// <summary>
        /// 获取JOSN序列化对象
        /// </summary>
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (jsSerializer == null)
                {
                    jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return jsSerializer;
            }
        }

        /// <summary>
        /// 返回前端调用方法
        /// </summary>
        private string CallbackMethod
        {
            get
            {
                return HttpContext.Current.Request["callback"] + string.Empty;
            }
        }

        /// <summary>
        /// 输出JSON格式到前端
        /// </summary>
        /// <param name="json"></param>
        private void ResponseJSON(string json)
        {
            HttpContext.Current.Response.ContentType = "application/json;charset=UTF-8";
            HttpContext.Current.Response.Write(string.Format("{0}({1})", CallbackMethod, json));
            HttpContext.Current.Response.End();
        }

        /// <summary>
        /// 检查是否有新版本需要更新
        /// </summary>
        /// <param name="platform">终端类型</param>
        /// <param name="version">终端版本号</param>
        /// <returns></returns>
        [WebMethod(Description = "加载用户的待办任务,上次加载时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void CheckVersion(string platform, string version)
        {
            object result = null;
            // 客户端会与 Version 进行比较，如果版本号不对，那么会提示或强制客户端进行升级
            if (platform.ToLower().IndexOf("android") > -1)
            {// Android
                result = new
                {
                    Version = "1.0.0",             // 最新版本号
                    Confirm = true.ToString(),     // 终端是否提示
                    Description = "新版本：1.0.0<br>更新特性：<br>1.发起流程更新",               // 更新说明
                    Url = "http://121.40.136.138:8010/Portal/H3.apk"   // APK下载地址
                };
            }
            else
            {// iOS 版本
                result = new
                {
                    Version = "1.0.0",             // 最新版本号
                    Confirm = true.ToString(),     // 终端是否提示
                    Description = "新版本：1.0.0<br>更新特性：<br>1.发起流程更新",               // 更新说明
                    Url = ""   // APK下载地址
                };
            }

            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 移动办公用户登录
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="password"></param>
        /// <param name="uuid"></param>
        /// <param name="jpushId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="mobileType"></param>
        /// <param name="isAppLogin">是否App登录</param>
        [WebMethod(EnableSession = true, Description = "用户登录")]
        public void ValidateLogin(string userCode,
            string password,
            string uuid,
            string jpushId,
            string mobileToken,
            string mobileType,
            bool isAppLogin)
        {
            OThinker.H3.Controllers.OrganizationController login = new OThinker.H3.Controllers.OrganizationController();
            var result = login.LoginInMobile(userCode, password, uuid, jpushId, mobileToken, mobileType, isAppLogin);
            ResponseJSON(JSSerializer.Serialize(result));
        }

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

            //if (userValidator.User.MobileToken == OThinker.Security.MD5Encryptor.GetMD5(mobileToken))
            if (userValidator.User.MobileToken == mobileToken)
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

        //[WebMethod(Description = "获取任务移动端Badge")]
        //[ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        //public void GetBadgeNum(string userId, string mobileToken)
        //{
        //    if (!ValidateMobileToken(userId, mobileToken)) return;
        //    MobileAccess mobile = new MobileAccess();
        //    var result = mobile.GetBadgeNum(userId, mobileToken);
        //    ResponseJSON(JSSerializer.Serialize(result));
        //}

        #region 待办任务 -------------------------
        /// <summary>
        /// 获取任务是否已经结束
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="workItemId"></param>
        [WebMethod(Description = "获取任务是否已经结束")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void IsWorkItemFinished(string userId, string mobileToken, string workItemId)
        {
            if (!ValidateMobileToken(userId, mobileToken)) return;

            bool isFinished = true;
            OThinker.H3.WorkItem.WorkItem workitem = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            if (workitem != null &&
                (workitem.State == WorkItem.WorkItemState.Waiting || workitem.State == WorkItem.WorkItemState.Working))
            {
                isFinished = false;
            }

            var result = new
            {
                Finished = isFinished
            };

            ResponseJSON(JSSerializer.Serialize(result));
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
        [WebMethod(Description = "加载用户的待办任务,上次加载时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoadWorkItems(string userId, string mobileToken, string keyWord,
            DateTime lastTime,
            string sortKey,
            string sortDirection,
            bool finishedWorkItem)
        {
            userId = AppUtility.RegParam(userId);
            keyWord = AppUtility.RegParam(keyWord);
            if (!ValidateMobileToken(userId, mobileToken)) return;

            MobileAccess mobile = new MobileAccess();
            var result = mobile.LoadWorkItems(userId, mobileToken, keyWord, lastTime, sortKey, sortDirection, finishedWorkItem);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));
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
        /// <returns></returns>
        [WebMethod(Description = "获取待办，已办任务列表，不区分优先级")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetWorkItems(MobileAccess.WorkItemQueryParams Params)
        {
            int returnCount = 10;//返回数量，默认为10；
            if (!ValidateMobileToken(Params.userId, Params.mobileToken)) return;
            MobileAccess mobile = new MobileAccess();
            var Items = mobile.GetLoadWorkItems(Params);
            ResponseJSON(JSSerializer.Serialize(Items));
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
        [WebMethod(Description = "获取待办，已办任务列表，区分优先级")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetWorkItemsByPriority(MobileAccess.WorkItemQueryParams Params)
        {
            if (!ValidateMobileToken(Params.userId, Params.mobileToken)) return;

            //Params.returnCount = 10;//返回数量，默认为10；
            //Params.lastTime = DateTime.Now;
            MobileAccess mobile = new MobileAccess();
            bool showNormal = true;//是否需要查询普通优先级
            OThinker.H3.Controllers.MobileAccess.LoadWorkItemsClass NormalPriorityItems = new H3.Controllers.MobileAccess.LoadWorkItemsClass();
            var HighPriorityItems = mobile.GetLoadWorkItems(Params);

            if (HighPriorityItems != null && HighPriorityItems.WorkItems.Count == 10)
            {
                showNormal = false;
            }
            else if (HighPriorityItems != null && HighPriorityItems.WorkItems.Count < 10)
            {
                //Params. = 10 - HighPriorityItems.WorkItems.Count;
            }

            if (showNormal)
            {
                NormalPriorityItems = mobile.GetLoadWorkItems(Params);
            }

            var result = new
            {
                HighPriorityItems = HighPriorityItems,
                NormalPriorityItems = NormalPriorityItems
            };

            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 获取用户的在办流程实例
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="lastTime">最后时间</param>
        /// <param name="sortKey">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        [WebMethod(Description = "获取用户的在办流程实例,上次加载时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        //public void LoadInstances(string userId,
        //    string mobileToken,
        //    string keyWord,
        //    DateTime lastTime,
        //    string sortKey,
        //    string sortDirection)
        //{
        //    userId = AppUtility.RegParam(userId);
        //    keyWord = AppUtility.RegParam(keyWord);
        //    sortKey = AppUtility.RegParam(sortKey);
        //    sortDirection = AppUtility.RegParam(sortDirection);
        //    if (!ValidateMobileToken(userId, mobileToken)) return;

        //    MobileAccess mobile = new MobileAccess();
        //    var result = mobile.LoadInstances(userId, mobileToken, keyWord, lastTime, sortKey, sortDirection);
        //    if (result == null) return;
        //    ResponseJSON(JSSerializer.Serialize(result));
        //}
        public void LoadInstances(MobileAccess.InstanceQueryParams Params)
        {
            Params.sortKey = "ReceiveTime";
            Params.sortDirection = "Desc";
            //Params.lastTime = DateTime.Now;
            Params.userId = AppUtility.RegParam(Params.userId);
            Params.keyWord = AppUtility.RegParam(Params.keyWord);
            if (!ValidateMobileToken(Params.userId, Params.mobileToken)) return;
            MobileAccess mobile = new MobileAccess();
            var result = new object() ;
            if (Params.status == 5)
            {
                result = mobile.LoadCanceledInstance(Params);
            }
            else if (Params.status == 4)
            {
                result = mobile.LoadFinishedInstance(Params);
            }
            else
            {
                result = mobile.LoadUnFinishedInstance(Params);
            }
            ResponseJSON(JSSerializer.Serialize(result));
        }
        /// <summary>
        /// 加载流程状态
        /// </summary>
        /// <param name="mobileToken"></param>
        /// <param name="instanceId"></param>
        /// <param name="workflowCode"></param>
        [WebMethod(Description = "加载流程状态")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoadInstanceState(string userId, string userCode, string mobileToken, string instanceId)
        {
            if (!ValidateMobileToken(userId, mobileToken)) return;
            UserValidator userValidate = ValidateUserMobileToken(userCode, mobileToken);
            MobileAccess mobile = new MobileAccess();
            var result = mobile.LoadInstanceState(userId, mobileToken, instanceId, userValidate);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 刷新用户的待办任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        [WebMethod(Description = "加载用户的待办任务,上次刷新时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RefreshInstances(string userId, string mobileToken, string keyWord, DateTime lastTime)
        {
            userId = AppUtility.RegParam(userId);
            if (!ValidateMobileToken(userId, mobileToken)) return;

            MobileAccess mobile = new MobileAccess();
            var result = mobile.RefreshInstances(userId, mobileToken, keyWord, lastTime);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));
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
        [WebMethod(Description = "加载用户的待办任务,上次刷新时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RefreshWorkItem(string userId,
            string mobileToken,
            string keyWord,
            DateTime lastTime,
            bool finishedWorkItem,
            int existsLength)
        {
            userId = AppUtility.RegParam(userId);
            if (!ValidateMobileToken(userId, mobileToken)) return;

            MobileAccess mobile = new MobileAccess();
            var result = mobile.RefreshWorkItem(userId, mobileToken, keyWord, lastTime, finishedWorkItem, existsLength);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));
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
        [WebMethod(Description = "刷新任务列表,不区分优先级")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetRefreshWorkItems(string userId, string mobileToken, string keyWord, DateTime LastTime,
            bool finishedWorkItem, int existsLength)
        {
            int returnCount = 10;//返回数量，默认为10；

            MobileAccess mobile = new MobileAccess();
            var Items = mobile.GetRefreshWorkItemsFn(userId, mobileToken, keyWord, LastTime,
                finishedWorkItem, existsLength, Instance.PriorityType.Unspecified, null, null, returnCount);
            ResponseJSON(JSSerializer.Serialize(Items));

        }

        /// <summary>
        /// 刷新任务列表
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
        [WebMethod(Description = "刷新任务列表,区分优先级")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetRefreshWorkItemsByPriority(string userId, string mobileToken, string keyword, DateTime highLastTime,
            DateTime normalLastTime, bool finishedWorkItem, int existsLengthHigh, int existsLenthNormal)
        {
            int returnCount = 10;//返回数量，默认为10；
            bool showNormal = true;//是否需要查询普通优先级
            MobileAccess mobile = new MobileAccess();
            H3.Controllers.MobileAccess.RefreshWorkItemsClass NormalRefreshWrokitem = new H3.Controllers.MobileAccess.RefreshWorkItemsClass();
            var HighRefreshWrokitem = mobile.GetRefreshWorkItemsFn(userId, mobileToken, keyword, highLastTime, finishedWorkItem, existsLengthHigh, Instance.PriorityType.High, DateTime.Parse("2004-01-01"), DateTime.MaxValue, returnCount);

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
                NormalRefreshWrokitem = mobile.GetRefreshWorkItemsFn(userId, mobileToken, keyword, highLastTime, finishedWorkItem, existsLenthNormal, Instance.PriorityType.Normal, DateTime.Parse("2004-01-01"), DateTime.MaxValue, returnCount);
            }

            var result = new
            {
                HighRefreshWrokitem = HighRefreshWrokitem,
                NormalRefreshWrokitem = NormalRefreshWrokitem
            };

            ResponseJSON(JSSerializer.Serialize(result));
        }




        /// <summary>
        /// 刷新用户的待办任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        /// <param name="finishedWorkItem">是否加载已办任务</param>
        [WebMethod(Description = "加载用户的待办任务,上次刷新时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RefreshWorkItems(string userId, string mobileToken, string keyWord, DateTime lastTime, bool finishedWorkItem)
        {
            RefreshWorkItem(userId, mobileToken, keyWord, lastTime, finishedWorkItem, 0);
        }

        #region 待阅、已阅
        /// <summary>
        /// 加载用户的待阅任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="lastTime">最后时间</param>
        /// <param name="sortKey">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        /// <param name="readWorkItem">是否已阅任务</param>
        [WebMethod(Description = "加载用户的待办任务,上次加载时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoadCirculateItems(MobileAccess.CirculateItemQueryParams Params)
        {
            Params.userId = AppUtility.RegParam(Params.userId);
            Params.keyWord = AppUtility.RegParam(Params.keyWord);
            if (!ValidateMobileToken(Params.userId, Params.mobileToken)) return;
            //if (!Params.lastTime.HasValue)
            //    Params.lastTime = DateTime.Now;
            MobileAccess mobile = new MobileAccess();
            var result = mobile.LoadCirculateItemsFn(Params);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));

        }

        /// <summary>
        /// 刷新用户的待阅任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="readWorkItem"></param>
        /// <param name="existsLength"></param>
        [WebMethod(Description = "加载用户的待办任务,上次加载时间")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RefreshCirculateItems(string userId,
           string mobileToken,
           string keyWord,
           DateTime lastTime,
           bool readWorkItem,
           int existsLength)
        {
            userId = AppUtility.RegParam(userId);
            keyWord = AppUtility.RegParam(keyWord);
            if (!ValidateMobileToken(userId, mobileToken)) return;
            MobileAccess mobile = new MobileAccess();
            var result = mobile.RefreshCirculateItemFn(userId, mobileToken, keyWord, lastTime, readWorkItem, existsLength);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 执行已阅
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="CirculateItemIDs"></param>
        [WebMethod(Description = "执行已阅")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void ReadCirculateItems(string userId,
           string mobileToken,
           string CirculateItemIDs,
            bool ReadAll)
        {
            userId = AppUtility.RegParam(userId);
            CirculateItemIDs = AppUtility.RegParam(CirculateItemIDs);
            if (!ValidateMobileToken(userId, mobileToken)) return;
            MobileAccess CirculateItem = new MobileAccess();
            var result = CirculateItem.ReadCirculateItemsByBatchFn(userId, mobileToken, CirculateItemIDs, ReadAll);
            ResponseJSON(JSSerializer.Serialize(result));
        }
        #endregion

        #region 我的实例
        [WebMethod(Description = "获取全部实例")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoadAllInstances(OThinker.H3.Controllers.MobileAccess.InstanceQueryParams Params)
        {
            //userId = AppUtility.RegParam(userId);
            if (!ValidateMobileToken(Params.userId, Params.mobileToken)) return;
            MobileAccess mobileAccess = new MobileAccess();
            //DateTime lastTime = DateTime.Now;
            //if (Params.lastTime.Equals(default(DateTime)))
            //    Params.lastTime = DateTime.Now;
            var result = mobileAccess.LoadAllInstanceList(Params);
            ResponseJSON(JSSerializer.Serialize(result));
        }

        [WebMethod(Description = "获取指定状态的流程实例")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoadInstancesByState(string userId, string mobileToken, string keyWord, int instanceState, int loadStart)
        {
            userId = AppUtility.RegParam(userId);
            if (!ValidateMobileToken(userId, mobileToken)) return;
            MobileAccess mobileAccess = new MobileAccess();
            var result = mobileAccess.LoadInstances(userId, Server.UrlDecode(keyWord), instanceState, loadStart);
            ResponseJSON(JSSerializer.Serialize(result));
        }


        [WebMethod(Description = "获取指定状态的流程实例")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void RefreshInstancesByState(string userId, string mobileToken, string keyWord, int instanceState, DateTime lastTime)
        {
            userId = AppUtility.RegParam(userId);
            if (!ValidateMobileToken(userId, mobileToken)) return;
            MobileAccess mobileAccess = new MobileAccess();
            var result = mobileAccess.RefreshInstancesByState(userId, keyWord, lastTime, instanceState);
            ResponseJSON(JSSerializer.Serialize(result));
        }
        #endregion

        /// <summary>
        /// 获取组织的名称
        /// </summary>
        /// <param name="units"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetUnitName(Unit[] units, string userId)
        {
            if (units == null) return string.Empty;
            foreach (Unit unit in units)
            {
                if (userId == unit.ObjectID)
                {
                    return unit.Name;
                }
            }
            return string.Empty;
        }
        #endregion

        /// <summary>
        /// 获取指定父ID的直属子组织和用户
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <param name="parentId"></param>
        [WebMethod(Description = "加载直接下级组织")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void SearchUser(string userCode, string mobileToken, string parentId, string searchKey)
        {
            UserValidator userValidator = ValidateUserMobileToken(userCode, mobileToken);
            if (userValidator == null) return;

            MobileAccess mobile = new MobileAccess();
            var result = mobile.SearchUserFn(userCode, mobileToken, parentId, searchKey);
            if (result == null) return;

            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 获取指定父ID的直属子组织和用户
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <param name="parentId"></param>
        [WebMethod(Description = "加载直接下级组织")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetOrganizationByParent(string userCode, string mobileToken, string parentId)
        {
            UserValidator userValidator = ValidateUserMobileToken(userCode, mobileToken);
            if (userValidator == null) return;

            MobileAccess mobile = new MobileAccess();
            var result = mobile.GetOrganizationByParentFn(userValidator, userCode, mobileToken, parentId);
            if (result == null) return;

            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <param name="targetUserId"></param>
        [WebMethod(Description = "获取用户信息")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetUserByObjectID(string userCode, string mobileToken, string targetUserId)
        {
            UserValidator sourceUser = ValidateUserMobileToken(userCode, mobileToken);
            if (sourceUser == null) return;

            OThinker.Organization.User user = this.Engine.Organization.GetUnit(targetUserId) as OThinker.Organization.User;
            if (user == null) return;

            UserValidator userValidator = UserValidatorFactory.GetUserValidator(this.Engine, user.Code);
            MobileAccess mobile = new MobileAccess();
            MobileAccess.MobileUser mobileUser = mobile.GetMobileUser(sourceUser, user, user.ImageUrl, userValidator.DepartmentName, string.Empty);
            var result = new
            {
                MobileUser = mobileUser
            };
            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 设置流程是否常用
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="workflowCode"></param>
        /// <param name="isFavorite"></param>
        [WebMethod(Description = "设置流程是否常用")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void SetFavorite(string userId, string mobileToken, string workflowCode, bool isFavorite)
        {
            if (!ValidateMobileToken(userId, mobileToken)) return;

            if (isFavorite)
            {
                this.Engine.WorkflowConfigManager.AddFavoriteWorkflow(userId, workflowCode);
            }
            else
            {
                this.Engine.WorkflowConfigManager.DeleteFavoriteWorkflow(userId, workflowCode);
            }
        }

        /// <summary>
        /// 加载流程模板信息
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        [WebMethod(Description = "获取用户可以发起的流程")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void LoadWorkflows(string userCode, string mobileToken)
        {
            UserValidator userValidate = ValidateUserMobileToken(userCode, mobileToken);
            if (userValidate == null)
            {
                throw new Exception("用户不存在");
            }

            MobileAccess mobile = new MobileAccess();
            var result = mobile.LoadWorkflowsFn(userValidate);
            if (result == null) return;
            ResponseJSON(JSSerializer.Serialize(result));
        }


        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        [WebMethod(Description = "修改密码")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void SetPassword(string userCode, string mobileToken, string oldPassword, string newPassword)
        {
            oldPassword = HttpUtility.UrlDecode(oldPassword).Replace("_38;_", "&");
            newPassword = HttpUtility.UrlDecode(newPassword).Replace("_38;_", "&");
            ActionResult result = new ActionResult(true);
            UserValidator userValidate = ValidateUserMobileToken(userCode, mobileToken);
            if (userValidate == null)
            {
                result.Success = false;
                result.Message = "用户不存在";
            }
            else
            {
                MobileAccess mobile = new MobileAccess();
                result = mobile.SetPasswordFn(userCode, oldPassword, newPassword);
            }
            ResponseJSON(JSSerializer.Serialize(result));
        }

        #region 门户首页------------------------------------------------

        /// <summary>
        /// 底部菜单待办待阅数量
        /// </summary>
        /// <returns></returns>
        [WebMethod(Description = "底部菜单待办待阅数量")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetWorkItemCount(string userId, string mobileToken)
        {
            if (!ValidateMobileToken(userId, mobileToken)) { return; }

            ActionResult result = new ActionResult(true);
            MobileAccess mobile = new MobileAccess();
            result.Extend = mobile.GetWorkItemCountCommon(userId);
            ResponseJSON(JSSerializer.Serialize(result));
        }

        /// <summary>
        /// 门户首页轮播图，显示菜单
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="appCode"></param>
        [WebMethod(Description = "门户首页轮播图，显示菜单")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetHybridApp(string userId, string mobileToken, string appCode)
        {
            if (!ValidateMobileToken(userId, mobileToken)) { return; }

            ActionResult result = new ActionResult(false);
            //是否显示
            bool slideShowDisplay = false;
            Boolean.TryParse(this.Engine.SettingManager.GetCustomSetting(OThinker.H3.Settings.CustomSetting.Setting_SlideShowDisplay), out slideShowDisplay);

            result.Success = true;
            OThinker.H3.Controllers.Controllers.HybridApp.HybridAppController hybridApp = new Controllers.Controllers.HybridApp.HybridAppController();

            result.Extend = new
            {
                SlideShows = hybridApp.GetDisplaySlideShows(userId),
                AppFunctionNodes = hybridApp.GetDisplayFunctionNodes(userId),
                SlideShowDisplay = slideShowDisplay,
                Badge = new
                {
                    unfinishedworkitem = hybridApp.GetUserUnfinishedWorkItemCount(userId) > 99
                    ? "99+" : hybridApp.GetUserUnfinishedWorkItemCount(userId).ToString(),
                    unreadworkitem = hybridApp.GetUserUnReadWorkItemCount(userId) > 99
                    ? "99+" : hybridApp.GetUserUnReadWorkItemCount(userId).ToString()
                }
            };
            ResponseJSON(JSSerializer.Serialize(result));
        }


        [WebMethod(Description = "获取数据模型数据")]
        [ScriptMethod(ResponseFormat = ResponseFormat.Json, UseHttpGet = true)]
        public void GetDataModelData(string userId, string DataModelCode, string QueryCode, string SortBy, int FromRowNum, int ShowCount)
        {
            ActionResult result = new ActionResult(true);
            MobileAccess mobile = new MobileAccess();
            result.Extend = mobile.GetDataModelDataFn(userId, DataModelCode, QueryCode, SortBy, FromRowNum, ShowCount);
            ResponseJSON(JSSerializer.Serialize(result));
        }
        #endregion
    }
}