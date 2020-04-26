using DongZheng.H3.WebApi.Common.Portal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class ProposalHandlerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string result = "";
            string commandName = context.Request["CommandName"];
            switch (commandName)
            {
                case "Get_CAP_EXPOSURE": result = Get_CAP_EXPOSURE(context); break;
                case "Get_CMS_EXPOSURE": result = Get_CMS_EXPOSURE(context); break;
            }
            context.Response.Write(result);
        }

        private string Get_CAP_EXPOSURE(HttpContextBase context)
        {
            string identifycode = context.Request["identifycode"];
            string name = context.Request["name"];
            string application_no = context.Request["application_no"];
            string app_type = context.Request["app_type"];
            DataTable dt = new CAP_Fuc().Get_CAP_EXPOSURE(identifycode, name, application_no, app_type);
            return JsonConvert.SerializeObject(dt);
        }

        private string Get_CMS_EXPOSURE(HttpContextBase context)
        {
            string application_no = context.Request["application_no"];
            DataTable dt = new CAP_Fuc().Get_CMS_EXPOSURE(application_no);
            return JsonConvert.SerializeObject(dt);
        }



        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}