using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class OrganizationSyncController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        #region const 常量
        public const string LastWechatSyncTime = "LastWechatSyncTime";
        public const string LastWechatSyncResult = "LastWechatSyncResult";

        public const string Last263SyncTime = "Last263SyncTime";
        public const string Last263SyncResult = "Last263SyncResult";

        public const string LastCAPSyncTime = "LastCAPSyncTime";
        public const string LastCAPSyncResult = "LastCAPSyncResult";

        #endregion

        // GET: OrganizationSync
        public string Index()
        {
            return "H3组织架构同步服务(客制)";
        }

        #region 同步结果
        [HttpGet]
        public JsonResult GetSyncInfo()
        {
            string lastWechatSyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(LastWechatSyncTime);
            string lastWechatSyncResult = AppUtility.Engine.SettingManager.GetCustomSetting(LastWechatSyncResult);

            string last263SyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(Last263SyncTime);
            string last263SyncResult = AppUtility.Engine.SettingManager.GetCustomSetting(Last263SyncResult);

            string lastCAPSyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(LastCAPSyncTime);
            string lastCAPSyncResult = AppUtility.Engine.SettingManager.GetCustomSetting(LastCAPSyncResult);

            return Json(new
            {
                LastWechatSyncTime = lastWechatSyncTime,
                LastWechatSyncResult = lastWechatSyncResult,
                Last263SyncTime = last263SyncTime,
                Last263SyncResult = last263SyncResult,
                LastCAPSyncTime = lastCAPSyncTime,
                LastCAPSyncResult = lastCAPSyncResult
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region 263同步
        [HttpPost]
        public JsonResult SyncFrom263()
        {
            //获取上次的同步时间
            var last263SyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(Last263SyncTime);
            last263SyncTime = string.IsNullOrEmpty(last263SyncTime) ? DateTime.MinValue.ToString() : last263SyncTime;
            //执行时间
            DateTime dt = DateTime.Now;
            //执行
            string Msg = DongZheng.H3.WebApi.Common._263.YXTClass.SetYXTUser(false, dt);
            //设置本次的同步时间
            AppUtility.Engine.SettingManager.SetCustomSetting(Last263SyncTime, DateTime.Now.ToString());
            AppUtility.Engine.SettingManager.SetCustomSetting(Last263SyncResult, Msg);

            AppUtility.Engine.LogWriter.Write("263同步结束：" + Msg);
            return Json(new { Success = true, Message = Msg });
        }

        [HttpPost]
        public JsonResult SyncAllFrom263()
        {
            //获取上次的同步时间
            var last263SyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(Last263SyncTime);
            last263SyncTime = string.IsNullOrEmpty(last263SyncTime) ? DateTime.MinValue.ToString() : last263SyncTime;
            //执行时间
            DateTime dt = DateTime.Now;
            //执行
            string Msg = DongZheng.H3.WebApi.Common._263.YXTClass.SetYXTUser(true, dt);
            //设置本次的同步时间
            AppUtility.Engine.SettingManager.SetCustomSetting(Last263SyncTime, DateTime.Now.ToString());
            AppUtility.Engine.SettingManager.SetCustomSetting(Last263SyncResult, Msg);

            AppUtility.Engine.LogWriter.Write("263同步结束：" + Msg);
            return Json(new { Success = true, Message = Msg });
        }
        #endregion

        #region Wechat 微信同步
        [HttpPost]
        public JsonResult SyncToWechatFromH3()
        {
            //获取上次的同步时间
            var lastWechatSyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(LastWechatSyncTime);
            lastWechatSyncTime = string.IsNullOrEmpty(lastWechatSyncTime) ? DateTime.MinValue.ToString() : lastWechatSyncTime;

            //设置本次的同步时间
            AppUtility.Engine.SettingManager.SetCustomSetting(LastWechatSyncTime, DateTime.Now.ToString());
            //开始同步
            var wechat = new WeChatAdapter(OThinker.H3.Controllers.AppUtility.Engine);
            SyncResult result = wechat.SyncOrgToWeChat(Convert.ToDateTime(lastWechatSyncTime));
            //保存本次的同步结果            
            AppUtility.Engine.SettingManager.SetCustomSetting(LastWechatSyncResult, result.ToString());
            AppUtility.Engine.LogWriter.Write("微信同步结束：" + result.ToString());
            return Json(new { Success = result.Success, Message = result.ToString() });
        }

        [HttpPost]
        public JsonResult SyncAllToWechatFromH3()
        {
            //设置本次的同步时间
            AppUtility.Engine.SettingManager.SetCustomSetting(LastWechatSyncTime, DateTime.Now.ToString());
            //开始同步
            var wechat = new WeChatAdapter(OThinker.H3.Controllers.AppUtility.Engine);
            SyncResult result = wechat.SyncOrgToWeChat(DateTime.MinValue);
            //保存本次的同步结果  
            AppUtility.Engine.SettingManager.SetCustomSetting(LastWechatSyncResult, result.ToString());
            AppUtility.Engine.LogWriter.Write("微信同步结束：" + result.ToString());
            return Json(new { Success = result.Success, Message = result.ToString() });
        }
        #endregion

        #region CAP 同步
        [HttpPost]
        public JsonResult SyncFromCAP()
        {
            //获取上次的同步时间
            var lastCAPSyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(LastCAPSyncTime);
            lastCAPSyncTime = string.IsNullOrEmpty(lastCAPSyncTime) ? DateTime.MinValue.ToString() : lastCAPSyncTime;
            //执行
            string Msg = CAP.CAPClass.SetCAPUser(false);
            //设置本次的同步时间
            AppUtility.Engine.SettingManager.SetCustomSetting(LastCAPSyncTime, DateTime.Now.ToString());
            AppUtility.Engine.SettingManager.SetCustomSetting(LastCAPSyncResult, Msg);

            AppUtility.Engine.LogWriter.Write("CAP同步结束：" + Msg);
            return Json(new { Success = true, Message = Msg });
        }

        [HttpPost]
        public JsonResult SyncAllFromCAP()
        {

            //获取上次的同步时间
            var lastCAPSyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(LastCAPSyncTime);
            lastCAPSyncTime = string.IsNullOrEmpty(lastCAPSyncTime) ? DateTime.MinValue.ToString() : lastCAPSyncTime;
            //执行
            string Msg = CAP.CAPClass.SetCAPUser(true);
            //设置本次的同步时间
            AppUtility.Engine.SettingManager.SetCustomSetting(LastCAPSyncTime, DateTime.Now.ToString());
            AppUtility.Engine.SettingManager.SetCustomSetting(LastCAPSyncResult, Msg);
            AppUtility.Engine.LogWriter.Write("CAP同步结束：" + Msg);
            return Json(new { Success = true, Message = Msg });
        }
        #endregion

        [HttpPost]
        public JsonResult AllSync()
        {
            try
            {
                #region CAP
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("CAP开始");
                //获取上次的同步时间
                var lastCAPSyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(LastCAPSyncTime);
                lastCAPSyncTime = string.IsNullOrEmpty(lastCAPSyncTime) ? DateTime.MinValue.ToString() : lastCAPSyncTime;
                //执行
                string CAPMsg = CAP.CAPClass.SetCAPUser(true);
                //设置本次的同步时间
                AppUtility.Engine.SettingManager.SetCustomSetting(LastCAPSyncTime, DateTime.Now.ToString());
                AppUtility.Engine.SettingManager.SetCustomSetting(LastCAPSyncResult, CAPMsg);
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(CAPMsg);
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("CAP结束");
                #endregion

                #region 263
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("263开始");
                //获取上次的同步时间
                var last263SyncTime = AppUtility.Engine.SettingManager.GetCustomSetting(Last263SyncTime);
                last263SyncTime = string.IsNullOrEmpty(last263SyncTime) ? DateTime.MinValue.ToString() : last263SyncTime;
                //执行时间
                DateTime dt = DateTime.Now;
                //执行
                string Msg = DongZheng.H3.WebApi.Common._263.YXTClass.SetYXTUser(true,dt);
                //设置本次的同步时间
                AppUtility.Engine.SettingManager.SetCustomSetting(Last263SyncTime, DateTime.Now.ToString());
                AppUtility.Engine.SettingManager.SetCustomSetting(Last263SyncResult, Msg);
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(Msg);
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("263结束");
                #endregion

                #region 微信
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("WX开始");
                //设置本次的同步时间
                AppUtility.Engine.SettingManager.SetCustomSetting(LastWechatSyncTime, DateTime.Now.ToString());
                //开始同步
                var wechat = new WeChatAdapter(OThinker.H3.Controllers.AppUtility.Engine);
                SyncResult result = wechat.SyncOrgToWeChat(DateTime.MinValue);
                //保存本次的同步结果  
                AppUtility.Engine.SettingManager.SetCustomSetting(LastWechatSyncResult, result.ToString());
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write(result.ToString());
                OThinker.H3.Controllers.AppUtility.Engine.LogWriter.Write("WX结束");
                #endregion
            }
            catch (Exception E)
            {
                return Json(new { Success = false, Message = E.ToString() });
            }

            return Json(new { Success = true, Message = "成功" });

        }

        #region Test chenghs
        public JsonResult GetWechatUser(string id)
        {
            WeChatAdapter wechat = new WeChatAdapter(AppUtility.Engine);
            wechat.LoadOrgToken();
            var ret = wechat.GetWeChatUser(id);
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAllUsers()
        {
            var msg = "";
            string domain = "dongzhengafc.com";
            string account = "dongzhengafc.com";
            string sign1 = "cf6436ec710286f08292d48e36034012";
            string sign2 = "d7e7089f86df015c74fdadcc244ce4fb";

            xmapi.XmapiImplProxyService api = new xmapi.XmapiImplProxyService();
            var ou = api.getDepartment("chenghuashan", domain, account, sign2); //全部ou
            var user = api.getDomainUserlist_New(domain, account, sign1); //全部用户

            return Json(user, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}