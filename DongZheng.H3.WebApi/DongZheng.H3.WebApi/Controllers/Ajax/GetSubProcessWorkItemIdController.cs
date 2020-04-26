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
    public class GetSubProcessWorkItemIdController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string instanceId = context.Request["instanceId"] ?? "";
            string type = context.Request["workItem"] ?? "";
            string sql = "", objectId = "";
            if (type == "关闭账户")
            {
                sql = "SELECT OBJECTID FROM I_EXITNET WHERE PARENTINSTANCEID = '" + instanceId + "'";
                objectId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            }
            if (type == "调整额度")
            {
                sql = "SELECT OBJECTID FROM I_UPDATELINE WHERE PARENTINSTANCEID = '" + instanceId + "'";
                objectId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            }
            sql = "SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID = '" + objectId + "'";
            string subInstanceId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            sql = "SELECT OBJECTID FROM OT_WORKITEM WHERE INSTANCEID = '" + subInstanceId + "'";
            string workItem = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            sql = "SELECT OBJECTID FROM OT_WORKITEMFINISHED WHERE INSTANCEID = '" + subInstanceId + "'";
            if (string.IsNullOrEmpty(workItem)) workItem = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            context.Response.Write(workItem);
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