using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using OThinker.H3.Sheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 工作表单控制器
    /// </summary>
    public class WorkSheetController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 获取工作表信息
        /// </summary>
        /// <param name="id">工作表ID</param>
        /// <param name="parentId">父节点ID</param>
        /// <param name="parentCode">父节点编码</param>
        /// <returns>工作表信息</returns>
        [HttpGet]
        public JsonResult GetWorkSheet(string id, string parentId, string parentCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                string root = this.Engine.FunctionAclManager.GetFunctionNode(parentId).DisplayName;
                if (string.IsNullOrEmpty(id))
                {
                    WorkSheetViewModel model = new WorkSheetViewModel()
                    {
                        SortKey = "1",
                        Type = "true",
                        Root = root,
                        isMobileAdd = true,
                        isPrintAdd = true,
                        ParentCode = parentCode,
                        ParentId = parentId
                    };
                    result.Extend = model;
                }
                else
                {
                    BizSheet workflowSheet = this.Engine.BizSheetManager.GetBizSheetByID(id);
                    WorkSheetViewModel model = new WorkSheetViewModel()
                    {
                        SortKey = "1",
                        Type = (workflowSheet.SheetType == SheetType.DefaultSheet).ToString().ToLower(),
                        Root = root,
                        isMobileAdd = string.IsNullOrEmpty(workflowSheet.MobileSheetAddress),
                        isPrintAdd = string.IsNullOrEmpty(workflowSheet.PrintSheetAddress),
                        ParentCode = parentCode,
                        ParentId = parentId,
                        ObjectID = workflowSheet.ObjectID,
                        Code = workflowSheet.SheetCode,
                        MobileAdd = workflowSheet.MobileSheetAddress,
                        PrintAdd = workflowSheet.PrintSheetAddress,
                        Name = workflowSheet.DisplayName,
                        PCAdd = workflowSheet.SheetAddress
                    };
                    result.Extend = model;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存工作表信息
        /// </summary>
        /// <param name="model">工作表信息</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveWorkSheet(WorkSheetViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                if (!Boolean.Parse(model.Type) && string.IsNullOrWhiteSpace(model.PCAdd.Trim()))
                {
                    result.Success = false;
                    result.Message = "WorkSheetEdit.Required";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //校验编码，不能以数字开始
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                if (!regex.Match(model.Code).Success)
                {
                    result.Success = false;
                    result.Message = "WorkSheetEdit.Msg";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                int sortKey;
                Int32.TryParse(model.SortKey, out sortKey);
                result.Success = false;
                SheetType sheetType = Boolean.Parse(model.Type) ? SheetType.DefaultSheet : SheetType.CustomSheet;

                BizObjectSchema schema = this.Engine.BizObjectManager.GetDraftSchema(model.ParentCode);
                string bizObjectSchemaCode = model.ParentCode;
                if (schema != null && schema.IsQuotePacket)
                {
                    bizObjectSchemaCode = schema.BindPacket;
                }
                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    BizSheet workflowSheet = new BizSheet(
                        model.ParentId,
                        model.Code.Trim(),
                        model.Name.Trim(),
                        sheetType,
                        Boolean.Parse(model.Type) ? "" : model.PCAdd.Trim(),
                      !model.isMobileAdd ? model.MobileAdd : "");
                    workflowSheet.PrintSheetAddress = !model.isPrintAdd ? model.PrintAdd : "";
                    workflowSheet.BizObjectSchemaCode = bizObjectSchemaCode;
                    if (bizObjectSchemaCode != model.ParentCode)
                    {
                        workflowSheet.OwnSchemaCode = model.ParentCode;
                    }
                    //流程共享包,共享表单
                    if (schema.IsShared)
                    {
                        workflowSheet.IsShared = true;
                    }
                    result.Success = this.Engine.BizSheetManager.AddBizSheet(workflowSheet);
                }
                else
                {
                    BizSheet workflowSheet = this.Engine.BizSheetManager.GetBizSheetByID(model.ObjectID);
                    workflowSheet.DisplayName = model.Name.Trim();
                    workflowSheet.SheetAddress = Boolean.Parse(model.Type) ? "" : model.PCAdd.Trim();
                    workflowSheet.SheetType = sheetType;
                    workflowSheet.MobileSheetAddress = !model.isMobileAdd ? model.MobileAdd : "";
                    workflowSheet.PrintSheetAddress = !model.isPrintAdd ? model.PrintAdd : "";
                    result.Success = this.Engine.BizSheetManager.UpdateBizSheet(workflowSheet);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
