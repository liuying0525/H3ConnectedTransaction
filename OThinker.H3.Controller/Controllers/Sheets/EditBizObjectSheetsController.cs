using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Sheets
{
    public class EditBizObjectSheetsController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public string Test()
        {
            return "SUCCESS";
        }

        public JsonResult EditBizObjectSheet(string BizObjectID, string SchemaCode, string SheetCode, string Mode, bool IsMobile, string EditInstanceData)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");

                string url = string.Empty;
                SheetMode SheetMode = SheetMode.Work;

                SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), Mode);
                if (!string.IsNullOrEmpty(EditInstanceData))
                {
                    url = GetWorkSheetUrl(
                            SchemaCode,
                            BizObjectID,
                            SheetMode,
                            IsMobile);
                    url += "&EditInstanceData=true";
                }
                else
                {
                    Sheet.BizSheet sheet = this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);
                    if (sheet == null)
                    {
                        // 兼容旧版本
                        OThinker.H3.Sheet.BizSheet[] sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(SchemaCode);
                        if (sheets == null || sheets.Length == 0)
                        {
                            throw new Exception("流程包{" + SchemaCode + "}表单不存在，请检查。");
                        }
                        sheet = sheets[0];
                    }
                    url = GetWorkSheetUrl(
                            SchemaCode,
                            BizObjectID,
                            sheet,
                            SheetMode,
                            IsMobile);
                }
                result.Message = url;
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
