using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.CustomSetting
{
    [ValidateInput(false)]
    [Xss]
    public class CustomSettingController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public string Index()
        {
            return "Proposal service";
        }

        public JsonResult GetAllCustomSetting()
        {
            var obj = new CustomSettingParas();

            obj.SysStopFromTime = AppUtility.Engine.SettingManager.GetCustomSetting("SysStopFromTime");
            obj.SysStopToTime = AppUtility.Engine.SettingManager.GetCustomSetting("SysStopToTime");
            obj.SysStopMessage = AppUtility.Engine.SettingManager.GetCustomSetting("SysStopMessage");

            obj.HttpPostCreatedURL = AppUtility.Engine.SettingManager.GetCustomSetting("HttpPostCreatedURL");
            obj.HttpPostUpdatedURL = AppUtility.Engine.SettingManager.GetCustomSetting("HttpPostUpdatedURL");
            obj.HttpPostRemovedURL = AppUtility.Engine.SettingManager.GetCustomSetting("HttpPostRemovedURL");

            return Json(new { Success = true, CustomParam = obj }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult SaveAllCustomSetting(CustomSettingParas para)
        {
            bool result = false;
            if (AppUtility.Engine.SettingManager.SetCustomSetting("HttpPostCreatedURL", para.HttpPostCreatedURL) &&
            AppUtility.Engine.SettingManager.SetCustomSetting("HttpPostUpdatedURL", para.HttpPostUpdatedURL) &&
            AppUtility.Engine.SettingManager.SetCustomSetting("HttpPostRemovedURL", para.HttpPostRemovedURL) &&
            AppUtility.Engine.SettingManager.SetCustomSetting("SysStopFromTime", para.SysStopFromTime) &&
            AppUtility.Engine.SettingManager.SetCustomSetting("SysStopToTime", para.SysStopToTime) &&
            AppUtility.Engine.SettingManager.SetCustomSetting("SysStopMessage", para.SysStopMessage))
            {
                result = true;
            }
            return Json(new { Success = result }, JsonRequestBehavior.AllowGet);
        }
    }
}