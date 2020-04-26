<%@ WebHandler Language="C#" Class="OThinker.H3.Portal.CommonHandler" %>
using System;
using System.Text;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace OThinker.H3.Portal
{
    public class CommonHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
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

        public string LoadMvcData(HttpContext context)
        {
            string id = context.Request["id"];
            string schemacode = context.Request["schemacode"];
            var result = Common.LoadMvcInstanceData(id, schemacode);
            return JsonConvert.SerializeObject(result);
        }


        public string calculate(HttpContext context)
        {
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




        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}