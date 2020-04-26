using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    public class WorkingCalendar : ControllerBase
    {
        /// <summary>
        /// 获取当前用户权限码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.SysParam_WorkingCalendar_Code; }
        }

        public JsonResult GetWorkingCalendar()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
