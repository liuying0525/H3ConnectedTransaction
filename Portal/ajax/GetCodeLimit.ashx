<%@ WebHandler Language="C#" Class="GetCodeLimit" %>

using System;
using System.Web;
using Newtonsoft.Json.Linq;

public class GetCodeLimit : IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        JArray limit = JArray.Parse(System.Configuration.ConfigurationManager.AppSettings["loginWaitLine"]);
        JObject ret = new JObject();
        string ip = context.Request.UserHostAddress;
        ip = ip.Replace("'","''");

        string o = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT COUNT(1) FROM C_VisitedHistory WHERE IP='" + ip + "';") + "";

        int times = 0;
        if (int.TryParse(o, out times))
        {
            int wait = (int)limit[times];
            ret.Add("CodeStatus", wait);
            if (wait <= 0)
            {
                ret.Add("WaitTime", 0);
            }
            else
            {
                o = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT MAX(VisitedTime) FROM C_VisitedHistory WHERE  IP='" + ip + "';") + "";
                DateTime dt = DateTime.Now;
                DateTime.TryParse(o, out dt);
                double timewait = (DateTime.Now - dt).TotalMilliseconds;
                ret.Add("WaitTime", timewait);
            }
        }
        else
        {
            ret.Add("CodeStatus", limit[0]);
            ret.Add("WaitTime", 0);
        }
        
        context.Response.ContentType = "text/plain";
        context.Response.Write("Hello World");
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}