<%@ WebHandler Language="C#" Class="VerificationCode" %>

using System;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using OThinker.H3;

public class VerificationCode : IHttpHandler, IRequiresSessionState
{
    private static object ___Lock = new object();
    private static OThinker.H3.Connection Connection;
    private static IEngine _engine = null;
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string result = "", sql = "";
        string enableCheckCode = System.Configuration.ConfigurationManager.AppSettings["EnableCheckCode"] + string.Empty;
        if (enableCheckCode == "0")
        {
            result = "0|0";
        }
        else
        {
            string action = context.Request["action"] ?? "";
            string code = context.Request["code"] ?? "";
            string user = context.Request["user"] ?? "";
            string model = "INSERT INTO OT_CHECKCODE(OBJECTID,USERCODE,IP,SYSTEMINFO,BROWSER,CODE,STATE)VALUES('[OBJECTID]','" + dealString(user) + "','" + dealString(context.Request.UserHostAddress) + "','" + dealString(context.Request.UserAgent) + "','" + dealString(context.Request.Browser.Browser) + "/" + dealString(context.Request.Browser.Version) + "','" + dealString(code) + "',[STATE])";

            string[] wait = System.Configuration.ConfigurationManager.AppSettings["WaitTime"].Split(',');
            try
            {
                switch (action)
                {
                    case "Error":
                        sql = model.Replace("[OBJECTID]", Guid.NewGuid().ToString()).Replace("[STATE]", "0");
                        int e = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                        break;
                    case "Submit":
                        sql = "SELECT COUNT(1) FROM OT_CHECKCODE WHERE STATE = 0 AND USERCODE = N'" + dealString(user) + "'";
                        int c = Convert.ToInt32(Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty);

                        string ret = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT Max(CREATETIME) FROM OT_CHECKCODE WHERE STATE = 0 AND USERCODE = N'" + user.Replace("'", "''") + "'") + string.Empty;
                        DateTime dt = !string.IsNullOrWhiteSpace(ret) ? Convert.ToDateTime(ret) : DateTime.Now;

                        int waits = 0;
                        int.TryParse(wait[c >= wait.Length ? wait.Length - 1 : c], out waits);
                        var left = (waits - (DateTime.Now - dt).TotalSeconds);

                        result = c + "|" + (waits > 0 ? (left < 0 ? 0 : left) : 0);
                        break;
                    case "Success":
                        sql = "UPDATE OT_CHECKCODE SET STATE = 1 WHERE USERCODE = N'" + dealString(user) + "'";
                        int g = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                        break;
                }
            }
            catch (Exception e)
            {
                Engine.LogWriter.Write("VerificationCode.ashx action-->" + action + ",code-->" + code + ",user-->" + user + ",Exception-->" + e.ToString());
            }
        }
        context.Response.Write(result);
    }

    private string dealString(string src)
    {
        return src.Replace("'", "''");
    }

    private static void SetEngin()
    {
        lock (___Lock)
        {
            if (Connection == null || Connection.Engine == null)
            {
                OThinker.H3.Connection c = new OThinker.H3.Connection();
                string connection = System.Configuration.ConfigurationManager.AppSettings["BPMEngine"];
                try
                {
                    OThinker.Clusterware.ConnectionResult result = c.Open(connection);
                    if (result != OThinker.Clusterware.ConnectionResult.Success)
                    {
                        throw new Exception("引擎服务连接错误->" + result.ToString());
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                Connection = c;
            }
        }
        _engine = Connection.Engine;
    }
    private IEngine Engine
    {
        get
        {
            if (_engine == null) SetEngin();
            return _engine;
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