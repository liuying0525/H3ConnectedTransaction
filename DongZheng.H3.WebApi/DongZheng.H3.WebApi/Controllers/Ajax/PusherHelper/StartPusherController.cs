using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax.PusherHelper
{
    [ValidateInput(false)]
    [Xss]
    public class StartPusherController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            JObject ret = new JObject();
            try
            {

                ServiceController[] serviceControllers = ServiceController.GetServices();
                ServiceController service = serviceControllers.FirstOrDefault(x => x.ServiceName.ToUpper() == System.Configuration.ConfigurationManager.AppSettings["PusherServiceName"].ToUpper());
                if (service == null)
                {
                    ret.Add("Result", 1);
                    ret.Add("Message", "未安装服务。");
                }
                else
                {
                    if (service.Status == ServiceControllerStatus.Running)
                    {
                        ret.Add("Result", 1);
                        ret.Add("Message", "服务已经启动。");
                    }
                    else
                    {
                        service.Start();
                        service.WaitForStatus(ServiceControllerStatus.Running);
                        service.Close();
                        ret.Add("Result", 0);
                        ret.Add("Message", "服务启动成功。");
                    }
                }
            }
            catch (Exception ex)
            {
                ret.Add("Result", -1);
                ret.Add("Message", ex.ToString());
            }

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