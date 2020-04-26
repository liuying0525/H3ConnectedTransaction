using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using OThinker.H3.Data;
using System.Data;

namespace DongZheng.H3.WebApi.Controllers.Workflow
{
    [ValidateInput(false)]
    [Xss]
    public class WorkflowApiController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }
        // GET: WorkflowSetting
        public string Index()
        {
            return "Workflow api";
        }

        public JsonResult GetDataFieldList(string SchemaCode, int FieldType)
        {
            var data = new List<object>();
            try
            {
                var type = (DataLogicType)FieldType;
                var schema = AppUtility.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
                foreach (var field in schema.Fields)
                {
                    if (field.LogicType == type)
                    {
                        data.Add(new { Code = field.Name, Name = field.DisplayName });
                    }
                }
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Data = "", Msg = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }

            return Json(new { Success = true, Data = data, Msg = "" }, JsonRequestBehavior.AllowGet);
        }
    }
}