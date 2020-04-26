using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.Admin.PropertyTreeHandler
{
    public class PropertyTreeHandlerController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult GetPropertyTree(string schemaCode)
        {
            return ExecuteFunctionRun(() => {

                Handler.PropertyTreeHandler handler = new Handler.PropertyTreeHandler(this);
                List<OThinker.H3.Controllers.Controllers.Admin.Handler.PropertyTreeHandler.ComboxTree> listTree = handler.GetPropertyTree(schemaCode);

                return Json(listTree,JsonRequestBehavior.AllowGet);
            });
        }
    }

   
}
