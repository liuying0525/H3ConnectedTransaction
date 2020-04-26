using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class MortgagePrintController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string BIZObjectID = context.Request["BIZOBJECTID"];
            GetPrintContent(BIZObjectID);
        }


        public void GetPrintContent(string objectId)
        {
            string sql = "SELECT * FROM I_MORTGAGE WHERE OBJECTID = '" + objectId + "'";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

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