using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class FinancialCodeSearchController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string code = context.Request["code"] ?? "";
            string objectid = context.Request["objectid"] ?? "";
            string sql = "SELECT OBJECTID FROM I_ALLOWIN WHERE FINANCIALCODE = '" + code + "'";
            string result = "";
            try
            {
                result = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                result = string.IsNullOrEmpty(result) || objectid == result ? "0" : "1";
            }
            catch (Exception e)
            {
                AppUtility.Engine.LogWriter.Write("入网流程财务编码检索：" + e.Message);
            }
            context.Response.Write(result);
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