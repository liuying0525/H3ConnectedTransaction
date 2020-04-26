using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 业务方法控制器
    /// </summary>
    [Authorize]
    public class BizObjectSchemaMethodController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }
        /// <summary>
        /// 业务模型编码
        /// </summary>
        protected string SchemaCode
        {
            get;
            set;
        }
        private OThinker.H3.DataModel.BizObjectSchema Schema;

        /// <summary>
        /// 方法Code
        /// </summary>
        protected string SelectedMethod
        {
            get;
            set;
        }

        /// <summary>
        /// 所属主数据
        /// </summary>
        private string ParentID
        {
            get;
            set;
        }

        protected OThinker.H3.DataModel.MethodGroupSchema Method;

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="parentId">所属数据模型ID</param>
        /// <param name="schemaCode">主数据编码</param>
        /// <param name="method">方法名称</param>
        /// <returns></returns>
        private bool ParseParam(string parentId, string schemaCode, string method)
        {
            this.ParentID = parentId;
            this.SchemaCode = schemaCode;
            this.SelectedMethod = method;
            if (!string.IsNullOrEmpty(SchemaCode))
            {
                this.Schema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
            }

            if (this.Schema == null)
            {
                return false;
            }
            if (!string.IsNullOrWhiteSpace(this.SelectedMethod))
            {
                this.Method = this.Schema.GetMethod(SelectedMethod);
            }
            return true;
        }

        /// <summary>
        /// 获取业务方法基础信息
        /// </summary>
        /// <param name="parentId">所属主数据ID</param>
        /// <param name="schemaCode">业务模型编码</param>
        /// <param name="method">方法Code</param>
        /// <returns>业务方法信息</returns>
        [HttpGet]
        public JsonResult GetBizObjectSchemaMethod(string parentId, string schemaCode, string method, string ownSchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (!ParseParam(parentId, schemaCode, method))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethod.Msg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                BizObjectSchemaMethod model = new BizObjectSchemaMethod();
                var methodTypes = GetMethodTypes();
                if (this.Method != null)
                {
                    model.MethodName = this.Method.MethodName;
                    model.DisplayName = this.Method.DisplayName;
                    model.Transaction = this.Method.TransactionalAsDefault;
                    model.UpdateAfterInvoking = this.Method.UpdateAfterInvoking;
                    model.ParentId = parentId;
                    model.MethodType = this.Method.MethodType.ToString();
                    model.IsDefaultMethod = DataModel.BizObjectSchema.GetDefaultMethods().Contains(this.Method.MethodName);
                    model.SchemaCode = schemaCode;
                    model.MethodCode = method;
                }
                else
                {
                    model.IsDefaultMethod = true;
                    model.ParentId = parentId;
                    model.SchemaCode = schemaCode;
                    model.MethodType = methodTypes.FirstOrDefault().Value; 
                }
                result.Success = true;
                result.Extend = new
                {
                    MethodTypes = methodTypes,
                    BizObjectSchemaMethod = model,
                    ServiceMethodMapList = GetServiceMethodMapList(),
                    IsLocked = BizWorkflowPackageLockByID(this.SchemaCode, ownSchemaCode),
                    StorageType = this.Schema.StorageType
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除业务方法
        /// </summary>
        /// <param name="schemaCode">业务模型编码</param>
        /// <param name="methodCode">方法Code</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DeleteBizObjectSchemaMethod(string schemaCode, string methodCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                methodCode = HttpUtility.UrlDecode(methodCode);
                if (!ParseParam("", schemaCode, methodCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Success = this.Schema.RemoveMethod(this.SelectedMethod);
                if (result.Success)
                {
                    result.Success = this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                }
                else {
                    result.Message = "msgGlobalString.DeleteFailed";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存业务方法
        /// </summary>
        /// <param name="model">业务方法模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveBizObjectSchemaMethod(BizObjectSchemaMethod model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (!ParseParam(model.ParentId, model.SchemaCode, model.MethodCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (this.Method == null)
                {
                    string methodName = model.MethodName;
                    if (string.IsNullOrWhiteSpace(methodName))
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchema.Msg1";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    this.Method = this.Schema.CreateMethod(methodName, (H3.DataModel.MethodType)Enum.Parse(typeof(H3.DataModel.MethodType), model.MethodType));
                }
                if (this.Method == null)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg2";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                this.Method.DisplayName = model.DisplayName;
                this.Method.TransactionalAsDefault = model.Transaction;
                this.Method.UpdateAfterInvoking = model.UpdateAfterInvoking;
                if (!this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Success = true;
                result.Message = "msgGlobalString.SaveFailed";
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 调整业务方法排序
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="method"></param>
        /// <param name="currentIndex"></param>
        /// <param name="targetIndex"></param>
        /// <param name="changeType"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DragMethodMap(string schemaCode,string method,string currentIndex,string targetIndex,string changeType) {
            return ExecuteFunctionRun(() => {
                ActionResult result = new ActionResult();
                return Json(result,JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取业务服务与业务方法映射模型列表
        /// </summary>
        /// <returns>业务服务与业务方法映射模型列表</returns>
        private object GetServiceMethodMapList()
        {
            var griddata = new object { };
            if (string.IsNullOrWhiteSpace(this.SelectedMethod))
            {
                return griddata;
            }
            List<ServiceMethodMapViewModel> list = new List<ServiceMethodMapViewModel>();
            foreach (DataModel.ServiceMethodMap map in Method.MethodMaps)
            {
                ServiceMethodMapViewModel model = new ServiceMethodMapViewModel();
                model.ServiceCode = map.ServiceCode;
                model.MethodName = map.MethodName;
                model.MethodType = this.Engine.BizBus.GetBizRule(map.ServiceCode) == null ? "ServiceMethod" : "RuleMethod";
                list.Add(model);
            }
            griddata = new { Rows = list, total = list.Count };
            return griddata;
        }

        /// <summary>
        /// 获取方法类型下拉框数据
        /// </summary>
        /// <returns>方法类型下拉框数据</returns>
        public List<Item> GetMethodTypes()
        {
            List<Item> list = new List<Item>();
            // 方法类型选项
            string[] names = Enum.GetNames(typeof(OThinker.H3.DataModel.MethodType));
            foreach (string name in names)
            {
                list.Add(new Item("EditBizObjectSchemaMethod.MethodType." + name, name));
            }
            return list;
        }

    }
}
