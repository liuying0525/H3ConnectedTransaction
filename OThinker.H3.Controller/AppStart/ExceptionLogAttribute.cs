using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.AppStart
{
    public class ExceptionLogAttribute : HandleErrorAttribute
    {
        /// <summary>
        /// 异常消息处理类
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnException(ExceptionContext filterContext)
        {
            string connectionString = System.Configuration.ConfigurationManager.AppSettings[OThinker.H3.Connection.ConnectionString_KeyName];
            if (string.IsNullOrEmpty(connectionString))
            {
                return;
            }
            string message = string.Format("Controller.Exception->Type={0}\r\n消息内容：{1}\r\n引发异常的方法：{2}\r\n引发异常的对象：{3}\r\n异常目录：{4}\r\n异常文件：{5}"
                , filterContext.Exception.GetType().Name
                , filterContext.Exception.Message
                , filterContext.Exception.TargetSite
                , filterContext.Exception.Source
                , filterContext.RouteData.GetRequiredString("controller")
                , filterContext.RouteData.GetRequiredString("action"));

            AppUtility.Engine.LogWriter.Write(message);

            base.OnException(filterContext);
        }
    }

}