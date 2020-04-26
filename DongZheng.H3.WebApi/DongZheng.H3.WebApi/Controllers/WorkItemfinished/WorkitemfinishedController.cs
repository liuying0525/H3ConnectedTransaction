using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.WorkItemfinished
{
    [ValidateInput(false)]
    [Xss]
    public class WorkItemfinishedController : OThinker.H3.Controllers.ControllerBase
    {
        // GET: Workitem
        public string Index()
        {
            return "Workitem API";
        }

        public override string FunctionCode
        {
            get { return ""; }
        }


        #region 
        /// <summary>
        /// 获取开票时间
        /// </summary>
        public JsonResult GetInvoiceTimeResult(string instanceID)
        {
            object obj = new object();
            string sql = "select max(RECEIVETIME) as RECEIVETIME from  (select RECEIVETIME from OT_Workitem where DISPLAYNAME='运营人员（初审）' and INSTANCEID='" + instanceID + "' union select RECEIVETIME from OT_workitemfinished where DISPLAYNAME = '运营人员（初审）' and INSTANCEID = '" + instanceID + "')";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                obj = new
                {
                    receiveTime = dt.Rows[0]["RECEIVETIME"].ToString()
                };
            }
            return Json(new { Success = true, Data = obj }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}