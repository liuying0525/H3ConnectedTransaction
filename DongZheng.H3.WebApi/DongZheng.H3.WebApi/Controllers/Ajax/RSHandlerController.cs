using DongZheng.H3.WebApi.Common.Portal;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class RSHandlerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public string Index(string CommandName)
        {
            string result = "";
            var context = HttpContext;
            switch (CommandName)
            {
                case "workitemSubmit": result = RSRiskManagement(context); break;
                case "getRSResult": result = getRSResult(context); break;
                case "postRongshu": result = postRongshuBymanual(context); break;
                case "postRongshuNew": result = postRongshuBymanualNew(context); break;
            }
            return result;
        }

        private string RSRiskManagement(HttpContextBase context)
        {
            bool rtn = false;
            string instanceid = context.Request["instanceid"];
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.SubmitWorkItemByInstanceID(instanceid, "");
            object obj = new
            {
                Result = rtn
            };
            return JSSerializer.Serialize(obj);
        }

        public string workitemSubmit()
        {
            return RSRiskManagement(HttpContext);
        }

        private string getRSResult(HttpContextBase context)
        {
            string rtn = "";
            string url = context.Request["address"];
            string param = context.Request["param"];
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.PostMoths(url, param);
            return rtn;
        }

        public string getRSResult()
        {
            return getRSResult(HttpContext);
        }

        private string postRongshuBymanual(HttpContextBase context)
        {
            string rtn = "", url = "";
            string SchemaCode = context.Request["SchemaCode"];
            string instanceid = context.Request["instanceid"];
            string manual = context.Request["manual"];
            if (manual == "1")
            {
                url = ConfigurationManager.AppSettings["rsurlRenGong"] + string.Empty;
            }
            else if (manual == "2")
            {
                url = ConfigurationManager.AppSettings["rsurl"] + string.Empty;
            }
            WorkFlowFunction wf = new WorkFlowFunction();
            rtn = wf.postHttpByManual(SchemaCode, instanceid, manual, url);
            return rtn;
        }

        public string postRongshu()
        {
            return postRongshuBymanual(HttpContext);
        }

        private string postRongshuBymanualNew(HttpContextBase context)
        {
            string rtn = "", url = "";
            string SchemaCode = context.Request["SchemaCode"];
            string instanceid = context.Request["instanceid"];
            string manual = context.Request["manual"];
            if (manual == "1")
            {
                url = ConfigurationManager.AppSettings["rsurlRenGong"] + string.Empty;
            }
            else if (manual == "2")
            {
                url = ConfigurationManager.AppSettings["rsurl"] + string.Empty;
            }
            WorkFlowFunctionNew wf = new WorkFlowFunctionNew();
            rtn = wf.postHttpByManual(SchemaCode, instanceid, manual, url);
            return rtn;
        }

        public string postRongshuNew()
        {
            return postRongshuBymanualNew(HttpContext);
        }
        #region 序列化器

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        private JavaScriptSerializer _JsSerializer = null;
        /// <summary>
        /// 获取JS序列化对象
        /// </summary>
        private JavaScriptSerializer JSSerializer
        {
            get
            {
                if (_JsSerializer == null)
                {
                    _JsSerializer = new JavaScriptSerializer();
                }
                return _JsSerializer;
            }
        }

        #endregion
    }
}