using DZ.External.WebApi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DZ.External.WebApi.Controllers
{
    public class TestController : OThinker.H3.Controllers.ControllerBase
    {
        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        public string Index()
        {
            return "TestController 服务";
        }
        
        public JsonResult GetValue(string access_token,string id)
        {
            Validate v = new Validate();

            return Json(new { ID = id, Text = "ValidateToken结果：" + v.ValidateToken(access_token) }, JsonRequestBehavior.AllowGet);
        }
    }
}