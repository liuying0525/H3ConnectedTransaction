<%@ WebHandler Language="C#" Class="OThinker.H3.Portal.ProposalHandler" %>
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
    public class ProposalHandler : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
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

        public string Get_CAP_EXPOSURE(HttpContext context)
        {
            string identifycode = context.Request["identifycode"];
            string name = context.Request["name"];
            string application_no = context.Request["application_no"];
            string app_type = context.Request["app_type"];
            DataTable dt = new CAP_Fuc().Get_CAP_EXPOSURE(identifycode, name, application_no, app_type);
            return JsonConvert.SerializeObject(dt);
        }

        public string Get_CMS_EXPOSURE(HttpContext context)
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