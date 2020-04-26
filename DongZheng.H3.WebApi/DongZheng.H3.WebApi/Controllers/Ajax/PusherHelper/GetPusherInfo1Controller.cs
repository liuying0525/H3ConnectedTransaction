using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace DongZheng.H3.WebApi.Controllers.Ajax.PusherHelper
{
    [ValidateInput(false)]
    [Xss]
    public class GetPusherInfo1Controller : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        enum ServiceState
        {
            NotExists = 0,
            Running = 1,
            Stoped = 2
        }

        public void Index()
        {
            var context = HttpContext;
            JObject ret = new JObject();

            try
            {
                ret.Add("Result", 0);
                ret.Add("ServiceState", 0);

                XmlDocument doc = new XmlDocument();
                doc.Load(System.Configuration.ConfigurationManager.AppSettings["PusherConfigFile"]);
                JObject data = new JObject();
                data.Add("PushInterval", doc.GetElementsByTagName("PushInterval")[0].InnerText);
                data.Add("Engine", doc.GetElementsByTagName("Engine")[0].InnerText);
                data.Add("PushPointTime", doc.GetElementsByTagName("PushPointTime")[0].InnerText);
                data.Add("PushUrl", doc.GetElementsByTagName("PushUrl")[0].InnerText);
                JObject sName = new JObject();
                foreach (XmlNode group in doc.GetElementsByTagName("StructNames")[0].ChildNodes)
                {
                    JObject g = new JObject();
                    foreach (XmlNode token in group.ChildNodes)
                    {
                        g.Add(token.Name, token.InnerText);
                    }
                    sName.Add(group.Name, g);
                }
                JArray views = new JArray();
                foreach (XmlNode view in doc.GetElementsByTagName("DataViews"))
                {
                    JObject token = new JObject();
                    foreach (XmlNode node in view.ChildNodes)
                    {
                        token.Add(node.Name, node.InnerText);
                    }
                    views.Add(token);
                }
                data.Add("StructNames", sName);
                data.Add("Views", views);
                ret.Add("Data", data);
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
                if (file.Name == "NoLog.txt")
                    ret.Add("Log", "无日志");
                else
                    ret.Add("Log", more.ToString("yyyy年MM月dd日 HH:mm:ss"));
            }
            catch (Exception ex) { }

            context.Response.Write(ret);
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