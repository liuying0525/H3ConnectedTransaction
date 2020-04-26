<%@ WebHandler Language="C#" Class="CheckFunction" %>

using System;
using System.Web;
using System.Web.SessionState;

public class CheckFunction :IRequiresSessionState, IHttpHandler {
    
    public void ProcessRequest (HttpContext context) {
        context.Response.ContentType = "text/html";
        if (OThinker.H3.Controllers.UserValidatorFactory.CurrentUser.ValidateFunctionRun(context.Request["Code"]))
        {
            Script.Console("OK");
        }
        else
        {
            Script.Alert("您无权访问此页面。");
        }
    }
 
    public bool IsReusable {
        get {
            return false;
        }
    }

}

public class Script
{
    public static void Alert(string message)
    {
        ResponseScript("alert('" + message + "');window.location = '/Portal/NoFunctionPage.html';");
    }
    public static void Console(string message)
    {
        ResponseScript("console.log('" + message + "');");
    }
    public static void ResponseScript(string script)
    {
        HttpContext.Current.Response.Write(script);
    }
}