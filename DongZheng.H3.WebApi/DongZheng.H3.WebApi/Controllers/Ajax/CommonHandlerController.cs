using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class CommonHandlerController : OThinker.H3.Controllers.ControllerBase
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
                case "calculate": result = calculate(context); break;
                case "loadmvcdata": result = LoadMvcData(context); break;
            }
            context.Response.Write(result);
        }

        private string LoadMvcData(HttpContextBase context)
        {
            context.Response.ContentType = "text/plain";
            string id = context.Request["id"];
            string schemacode = context.Request["schemacode"];
            var result = Common.Portal.Common.LoadMvcInstanceData(id, schemacode);
            return JsonConvert.SerializeObject(result);
        }

        public string LoadMvcData()
        {
            return LoadMvcData(HttpContext);
        }

        private string calculate(HttpContextBase context)
        {
            context.Response.ContentType = "text/plain";
            string result = "";
            string type = context.Request["type"];
            string arg1 = (context.Request["arg1"] + string.Empty).Replace(",", "");
            string arg2 = (context.Request["arg2"] + string.Empty).Replace(",", "");
            string precision = context.Request["precision"];
            double p1 = Convert.ToDouble(arg1 == "" ? "0" : arg1);
            double p2 = Convert.ToDouble(arg2 == "" ? "0" : arg2);
            //判断
            switch (type)
            {
                case "add": result = (p1 + p2).ToString(precision); break;
                case "min": result = (p1 - p2).ToString(precision); break;
                case "mul": result = (p1 * p2).ToString(precision); break;
                case "div": result = (p1 / p2).ToString(precision); break;
            }


            return JsonConvert.SerializeObject(result);
        }

        public string calculate()
        {
            return calculate(HttpContext);
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