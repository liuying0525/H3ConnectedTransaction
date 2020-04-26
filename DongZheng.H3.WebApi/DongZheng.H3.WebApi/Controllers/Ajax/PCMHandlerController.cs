using DongZheng.H3.WebApi.Common.Portal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class PCMHandlerController : OThinker.H3.Controllers.ControllerBase
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
                case "GetConnect": result = GetCustomerInfo(context); break;


            }
            context.Response.Write(result);

        }

        public string GetCustomerInfo(HttpContextBase context)
        {
            try
            {
                string rtn = "";
                string strCertno = context.Request["certno"];
                string appno = context.Request["appno"] == null ? "" : context.Request["appno"];
                string strDzcarloan = context.Request["dzcarloan"];
                WorkPcmFunction pcm = new WorkPcmFunction();
                rtn = pcm.IsExistHKNLInfo(appno, strCertno);
                if (rtn == "")
                {
                    rtn = pcm.GetCustomerInfo(strCertno, strDzcarloan);
                }
                return rtn;
            }
            catch (Exception ex)
            {
                CommonFunction.AddLog("PCM_LOG", DateTime.Now.ToString("yyyyMMdd"), ex.ToString());
                return "";
            }
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