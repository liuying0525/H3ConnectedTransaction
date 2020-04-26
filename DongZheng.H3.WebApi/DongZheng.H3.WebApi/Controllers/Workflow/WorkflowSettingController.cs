using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using System.Data;
using DongZheng.H3.WebApi.Common.MySetting;

namespace DongZheng.H3.WebApi.Controllers.Workflow
{
    [ValidateInput(false)]
    [Xss]
    public class WorkflowSettingController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }
        // GET: WorkflowSetting
        public string Index()
        {
            return "WorkflowSetting";
        }

        public JsonResult SetCustomSetting(string InstanceID, string SchemaCode, string SettingName, string SettingValue)
        {
            int n = WorkflowSetting.SetCustomSettingValue(InstanceID, SchemaCode, SettingName, SettingValue);
            if (n == 0)
            {
                AppUtility.Engine.LogWriter.Write("SetCustomSetting失败,0行受影响");
                return Json(new { Success = false, EffectNumber = n }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { Success = true, EffectNumber = n }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCustomSetting(string InstanceID, string SettingName)
        {
            return Json(WorkflowSetting.GetCustomSettingValue(InstanceID, SettingName), JsonRequestBehavior.AllowGet);
        }
    }
}