using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    public class MasterPackageController : ControllerBase
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        /// <summary>
        /// 获取主数据信息
        /// </summary>
        /// <param name="objectType">对象类型</param>
        /// <param name="parentId">父节点ID</param>
        /// <param name="parentCode">父节点对象编码</param>
        /// <returns>主数据信息</returns>
        [HttpGet]
        public JsonResult GetMasterPackage(string objectType, string parentId, string parentCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                MasterPackageViewModel model = new MasterPackageViewModel()
                {
                    ParentId = parentId,
                    ObjectType = objectType,
                    ParentCode = parentCode,
                    StorageType = "1",
                    IsImport = false,
                };
                result.Extend = new
                {
                    MasterPackage = model,
                    BizServices = GetBizServices()
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        [HttpPost]
        public JsonResult SaveMasterPackage(MasterPackageViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (string.IsNullOrWhiteSpace(model.Code))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.CodeNotNull";
                    return Json(result, JsonRequestBehavior.AllowGet);
                    //this.ShowErrorMessage(this.PortalResource.GetString("BizOperation_CodeNotNull"));
                }
                H3.DataModel.BizObjectSchema schema = null;
                if (model.IsImport)
                {
                    if (!string.IsNullOrEmpty(model.BizService))
                    {
                        schema = this.FromDbTableAdapter(model.Code, model.BizService);
                        schema.DisplayName =model.DisplayName??schema.SchemaCode;
                    }
                }
                else
                {
                    ////手工新建
                    //if (string.IsNullOrWhiteSpace(model.DisplayName))
                    //{
                    //    result.Success = false;
                    //    result.Message = "EditBizObjectSchema.DisplayNameNotNull";
                    //    return Json(result, JsonRequestBehavior.AllowGet);
                    //}
                    StorageType storageType = (StorageType)int.Parse(model.StorageType);
                    schema = new DataModel.BizObjectSchema(storageType,
                        model.Code.Trim(),
                        storageType != StorageType.PureServiceBased);
                    schema.DisplayName = model.DisplayName ?? schema.SchemaCode;
                }

                if (schema == null)
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.CreateFailed";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //添加校验
                if (schema.SchemaCode.Length > BizObjectSchema.MaxCodeLength)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg1";
                    result.Extend = BizObjectSchema.MaxCodeLength;
                    return Json(result, JsonRequestBehavior.AllowGet);
                    //this.ShowWarningMessage(this.PortalResource.GetString("BizObjectSchema_Mssg1") + BizObjectSchema.MaxCodeLength);
                }

                //校验编码规范
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(BizObjectSchema.CodeRegex);
                if (!regex.IsMatch(schema.SchemaCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaProperty.Msg2";
                    return Json(result, JsonRequestBehavior.AllowGet);
                    //this.ShowWarningMessage(this.PortalResource.GetString("BizObjectSchema_Mssg2"));
                }

                if (!this.Engine.BizObjectManager.AddDraftSchema(schema))
                {
                    //保存失败，可能是由于编码重复
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg3";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    // 添加成功，这里还是需要写入FuntionNode节点的，为了跟流程包里的数据模型节点区别
                    Acl.FunctionNode node = new Acl.FunctionNode
                        (schema.SchemaCode,
                        schema.DisplayName,
                        "",
                        "",
                        FunctionNodeType.BizObject,
                        model.ParentCode,
                        0
                        );

                    this.Engine.FunctionAclManager.AddFunctionNode(node);
                }

                result.Success = true;
                result.Message = "msgGlobalString.SaveSuccess";
                result.Extend = model.ParentId;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 从服务创建
        /// </summary>
        /// <returns></returns>
        private H3.DataModel.BizObjectSchema FromDbTableAdapter(string code, string service)
        {
            H3.DataModel.BizObjectSchema schema = new DataModel.BizObjectSchema(DataModel.StorageType.PureServiceBased, code, false);

            // 获得服务定义
            H3.DataModel.BizObjectSchemaUtility.CreateMethodMap(this.Engine.BizBus, schema, H3.DataModel.BizObjectSchema.MethodName_Create, H3.DataModel.MethodType.Normal, service, H3.BizBus.Declaration.DbTableAdapter_Method_Insert);
            H3.DataModel.BizObjectSchemaUtility.CreateMethodMap(this.Engine.BizBus, schema, H3.DataModel.BizObjectSchema.MethodName_Load, H3.DataModel.MethodType.Normal, service, H3.BizBus.Declaration.DbTableAdapter_Method_Load);
            H3.DataModel.BizObjectSchemaUtility.CreateMethodMap(this.Engine.BizBus, schema, H3.DataModel.BizObjectSchema.MethodName_Update, H3.DataModel.MethodType.Normal, service, H3.BizBus.Declaration.DbTableAdapter_Method_Update);
            H3.DataModel.BizObjectSchemaUtility.CreateMethodMap(this.Engine.BizBus, schema, H3.DataModel.BizObjectSchema.MethodName_Remove, H3.DataModel.MethodType.Normal, service, H3.BizBus.Declaration.DbTableAdapter_Method_Delete);
            H3.DataModel.BizObjectSchemaUtility.CreateMethodMap(this.Engine.BizBus, schema, H3.DataModel.BizObjectSchema.MethodName_GetList, H3.DataModel.MethodType.Filter, service, H3.BizBus.Declaration.DbTableAdapter_DefaultGetListName);

            return schema;
        }
        /// <summary>
        /// 获取业务服务下拉数据
        /// </summary>
        /// <returns>业务服务数据</returns>
        private List<Item> GetBizServices()
        {
            List<Item> itemList = new List<Item>();
            itemList.Add(new Item("", ""));
            H3.BizBus.BizService.BizService[] services = this.Engine.BizBus.GetBizServicesByAdapter(H3.BizBus.Declaration.DbTableAdapter_Code);
            foreach (H3.BizBus.BizService.BizService service in services)
            {
                itemList.Add(new Item(service.FullName, service.Code));
            }
            return itemList;
        }

    }
}
