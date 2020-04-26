using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using OThinker.H3.Controllers;

namespace OThinker.H3.Cluster
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //输入Portal转向到Index.html页面
            if (Context.Request.FilePath == "/Cluster"|| Context.Request.FilePath ==  "/Cluster/")
            {
                Context.Response.Redirect("/Cluster/login.html");
            }
            HttpRequestBase request = new HttpRequestWrapper(this.Context.Request);
            //ajax请求,取消FormsAuthenticationRedirect.
            if (request.IsAjaxRequest())
            {
                Context.Response.SuppressFormsAuthenticationRedirect = true;
            }
        }
    }
}
