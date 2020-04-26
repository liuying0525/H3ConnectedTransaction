using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax.PusherHelper
{
    [ValidateInput(false)]
    [Xss]
    public class GetNewLogFileController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            DateTime more = new DateTime(1970, 1, 1);
            FileInfo file = new FileInfo("NoLog.txt");
            string dir = System.Configuration.ConfigurationManager.AppSettings["PusherConfigFile"];
            dir = dir.Substring(0, dir.LastIndexOf("\\")) + "\\Logs";
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            DirectoryInfo logs = new DirectoryInfo(dir);
            foreach (FileInfo item in logs.GetFiles())
            {
                if (item.Extension.ToLower() == ".txt")
                {
                    if (item.CreationTime > more)
                    {
                        more = item.CreationTime;
                        file = item;
                    }
                }
            }

            context.Response.Buffer = true;
            context.Response.Clear();
            context.Response.ClearContent();
            context.Response.ClearHeaders();
            context.Response.AddHeader("Content-Disposition", "attachment;filename=" + file.Name);
            context.Response.AddHeader("Content-Type", "text/plain");
            context.Response.TransmitFile(file.FullName);
            context.Response.End();
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