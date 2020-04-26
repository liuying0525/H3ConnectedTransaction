using OThinker.H3.Acl;
using OThinker.H3.BizBus.BizRule;
using OThinker.H3.BizBus.BizService;
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
    /// <summary>
    /// 业务方法绑定控制器
    /// </summary>
    public class ServiceMethodMapController : ControllerBase
    {

        #region 参数

        /// <summary>
        /// 方法类型，服务方法：ServiceMethod；规则方法：RuleMethod
        /// </summary>
        protected string MethodType
        {
            get;
            set;
        }

        /// <summary>
        /// 数据模型编码,必须有值
        /// </summary>
        protected string SchemaCode
        {
            get;
            set;
        }
        /// <summary>
        /// 当前数据模型
        /// </summary>
        protected OThinker.H3.DataModel.BizObjectSchema Schema;
        /// <summary>
        /// 选中的业务方法，必须有值
        /// </summary>
        protected string SelectedMethod
        {
            get;
            set;
        }
        /// <summary>
        /// 当前业务方法
        /// </summary>
        protected OThinker.H3.DataModel.MethodGroupSchema Method;
        /// <summary>
        /// 选中的映射关系,新增时，为空
        /// </summary>
        protected int SelectedMapIndex = -1;
        /// <summary>
        /// 编辑映射关系
        /// </summary>
        protected OThinker.H3.DataModel.ServiceMethodMap SelectedMap;
        #endregion

        #region 初始化参数
        /// <summary>
        /// 初始化参数
        /// </summary>
        /// <returns>初始化结果</returns>
        private ActionResult ParseParam(string schemaCode, string selectedMethod, string methodType)
        {
            this.MethodType = methodType;
            this.SchemaCode = schemaCode;
            this.SelectedMethod = selectedMethod;
            ActionResult result = new ActionResult();
            //数据模型和业务方法不能为空
            if (string.IsNullOrEmpty(SchemaCode) || string.IsNullOrWhiteSpace(this.SelectedMethod))
            {
                this.Schema = null;
                result.Success = false;
            }
            else
            {
                this.Schema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
                result.Success = true;
            }

            if (this.Schema == null)
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaMethod_Msg1";
            }
            //业务方法
            this.Method = this.Schema.GetMethod(this.SelectedMethod);
            return result;
        }
        #endregion

        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        /// <summary>
        /// 获取业务方法绑定映射信息
        /// </summary>
        /// <param name="mapIndex">参数行号</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="method">方法名称</param>
        /// <param name="methodType">方法类型</param>
        /// <returns>业务方法绑定映射信息</returns>
        [HttpGet]
        public JsonResult GetServiceMethodMap(string mapIndex, string schemaCode, string method, string methodType)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, method, methodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);

                int isLocked = BizWorkflowPackageLockByID(this.SchemaCode);

                ServiceMethodMapViewModel model = GetServiceMethodMapViewModel(mapIndex);
                List<Item> serviceCodes = GetServiceCodesList();
                List<Item> methodNames = null;
                if (methodType.Equals("RuleMethod", StringComparison.OrdinalIgnoreCase))
                {// 业务规则
                    methodNames = GetBizRuleMethodNamesList(model.ServiceCode);
                }
                else
                {
                    methodNames = GetMethodNamesList(model.ServiceCode);
                }
                DataModel.ServiceMethodMap buldMap = BuldMap(model.ServiceCode, model.MethodName);
                result.Extend = new
                {
                    ServiceMethodMap = model,
                    ServiceCodes = serviceCodes,
                    MethodNames = methodNames,
                    ParamGridData = GetGridData(buldMap, "param"),
                    ReturnGridData = GetGridData(buldMap, "return"),
                    IsLocked = isLocked
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取方法列表
        /// </summary>
        /// <param name="serviceCode">服务编码</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="method">方法名称</param>
        /// <param name="methodType">方法类型</param>
        /// <returns>方法列表</returns>
        [HttpGet]
        public JsonResult GetServiceMethodNamesList(string serviceCode, string schemaCode, string method, string methodType)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, method, methodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                List<Item> methodNames = null;
                if (methodType.Equals("RuleMethod", StringComparison.OrdinalIgnoreCase))
                {// 业务规则
                    methodNames = GetBizRuleMethodNamesList(serviceCode);
                }
                else
                {
                    methodNames = GetMethodNamesList(serviceCode);
                }
                string methodName = methodNames.Count > 0 ? methodNames.FirstOrDefault().Value : "";
                DataModel.ServiceMethodMap buldMap = BuldMap(serviceCode, methodName);
                result.Extend = new
                {
                    MethodNames = methodNames,
                    ParamGridData = GetGridData(buldMap, "param"),
                    ReturnGridData = GetGridData(buldMap, "return"),
                    MethodName = methodName
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存映射信息
        /// </summary>
        /// <param name="model">映射模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveServiceMethodMap(ServiceMethodMapViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, model.Method, model.MethodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrWhiteSpace(model.ServiceCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethodMap.SelectBiz";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrWhiteSpace(model.MethodName))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethodMap.SelectMethodName";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                GetServiceMethodMapViewModel(model.MapIndex);//初始化this.SelectedMap
                if (this.SelectedMap == null)
                {
                    this.Schema.GetMethod(this.SelectedMethod).AddMethodMap(BuldMap(model.ServiceCode, model.MethodName));
                }
                this.SelectedMap = this.Schema.GetMethod(this.SelectedMethod).MethodMaps.FirstOrDefault(p => p.ServiceCode == model.ServiceCode && p.MethodName == model.MethodName);
                this.SelectedMap.ExeCondition = model.ExeCondition;
                // 保存
                if (!this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Success = true;
                result.Message = "msgGlobalString.SaveSuccess";
                this.SelectedMapIndex = this.Schema.GetMethod(this.SelectedMethod).MethodMaps.Length - 1;
                model = GetServiceMethodMapViewModel(this.SelectedMapIndex.ToString());
                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除绑定的业务规则或业务方法
        /// </summary>
        /// <param name="schemaCode">业务模型编码</param>
        /// <param name="method">方法名称</param>
        /// <param name="methodType">方法类型</param>
        /// <param name="mapIndex">列号</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelServiceMethodMap(string schemaCode, string method, string methodType, string mapIndex)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, method, methodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                ServiceMethodMapViewModel model = GetServiceMethodMapViewModel(mapIndex);
                this.Schema.GetMethod(this.SelectedMethod).RemoveMethodMap(this.SelectedMapIndex);
                result.Success = this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                result.Extend = GetServiceMethodMapList();
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 方法名称选择时间
        /// </summary>
        /// <param name="model">业务服务与业务方法映射模型</param>
        /// <returns>表格数据</returns>
        [HttpGet]
        public JsonResult MethodNameChange(ServiceMethodMapViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, model.Method, model.MethodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                DataModel.ServiceMethodMap buldMap = BuldMap(model.ServiceCode, model.MethodName);
                if (buldMap == null)
                {
                    result.Success = false;
                }
                else
                {
                    result.Extend = new
                    {
                        ParamGridData = GetGridData(buldMap, "param"),
                        ReturnGridData = GetGridData(buldMap, "return"),
                    };
                    result.Success = true;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 升级到最新的方法
        /// </summary>
        /// <param name="model">业务服务与业务方法映射模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult UpgradeServiceMethodMap(ServiceMethodMapViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, model.Method, model.MethodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrWhiteSpace(model.ServiceCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethodMap.SelectBiz";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                GetServiceMethodMapViewModel(model.MapIndex);

                H3.DataModel.MethodGroupSchema methodGroup = this.Schema.GetMethod(this.SelectedMethod);

                H3.DataModel.ServiceMethodMap map = null;
                if (methodGroup.MethodType == H3.DataModel.MethodType.Normal || methodGroup.MethodType == H3.DataModel.MethodType.Static)
                {
                    // 获得最新的方法
                    H3.BizBus.BizService.MethodSchema methodSchema = this.Engine.BizBus.GetMethod(this.SelectedMap.ServiceCode, this.SelectedMap.MethodName);
                    if (methodSchema == null)
                    {
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    map = new DataModel.ServiceMethodMap(
                        this.SelectedMap.ServiceCode,
                        methodSchema);
                }
                else if (methodGroup.MethodType == H3.DataModel.MethodType.Filter)
                {
                    // 获得最新的搜索方法
                    H3.BizBus.Filter.FilterSchema filterSchema = this.Engine.BizBus.GetFilterSchema(this.SelectedMap.ServiceCode, this.SelectedMap.MethodName);
                    if (filterSchema == null)
                    {
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    map = new DataModel.ServiceMethodMap(
                        this.SelectedMap.ServiceCode,
                        filterSchema);
                }
                else
                {
                    throw new NotImplementedException();
                }
                map.ExeCondition = model.ExeCondition;
                // 更新方法的参数映射
                if (map.ParamMaps != null)
                {
                    H3.DataModel.DataMap[] dataMaps = map.ParamMaps.ToArray();
                    if (dataMaps != null)
                    {
                        foreach (H3.DataModel.DataMap dataMap in dataMaps)
                        {
                            dataMap.MapTo = this.SelectedMap.ParamMaps.GetMappingPropertyName(dataMap.ItemName);
                        }
                    }
                }
                // 更新方法的返回值映射
                if (map.ReturnMaps != null)
                {
                    H3.DataModel.DataMap[] dataMaps = map.ReturnMaps.ToArray();
                    if (dataMaps != null)
                    {
                        foreach (H3.DataModel.DataMap dataMap in dataMaps)
                        {
                            dataMap.MapTo = this.SelectedMap.ReturnMaps.GetMappingPropertyName(dataMap.ItemName);
                        }
                    }
                }
                // 更新的方法集合中
                this.Schema.GetMethod(this.SelectedMethod).RemoveMethodMap(this.SelectedMapIndex);
                this.Schema.GetMethod(this.SelectedMethod).InsertMethodMap(this.SelectedMapIndex, map);
                result.Success = this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                if (result.Success)
                {
                    result.Extend = new
                    {
                        ParamGridData = GetGridData(map, "param"),
                        ReturnGridData = GetGridData(map, "return")
                    };

                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 自动创建属性并绑定
        /// </summary>
        /// <param name="model">业务服务与业务方法映射模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult CreateProperty(ServiceMethodMapViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, model.Method, model.MethodType);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrWhiteSpace(model.ServiceCode))
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethodMap.SelectBiz";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                GetServiceMethodMapViewModel(model.MapIndex);
                H3.DataModel.BizObjectSchemaUtility.CreateProperty(this.Engine.BizBus, this.Schema, this.SelectedMethod, this.SelectedMap.ServiceCode, this.SelectedMap.MethodName, this.SelectedMap);
                result.Success = this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                if (result.Success)
                {
                    result.Extend = GetBuldMapGridData(model.ServiceCode, model.MethodName);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="serviceCode">业务模型编码</param>
        /// <param name="methodName">方法名称</param>
        /// <returns>数据</returns>
        private object GetBuldMapGridData(string serviceCode, string methodName)
        {
            DataModel.ServiceMethodMap buldMap = BuldMap(serviceCode, methodName);
            if (buldMap == null) return null;
            return new
            {
                ParamGridData = GetGridData(buldMap, "param"),
                ReturnGridData = GetGridData(buldMap, "return")
            };
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
            ServiceMethodMapViewModel model = new ServiceMethodMapViewModel();
            foreach (DataModel.ServiceMethodMap map in Method.MethodMaps)
            {
                model.ServiceCode = map.ServiceCode;
                model.MethodName = map.MethodName;
                model.MethodType = this.Engine.BizBus.GetBizRule(map.ServiceCode) == null ? "ServiceMethod" : "RuleMethod";
                list.Add(model);
            }
            griddata = new { Rows = list, total = list.Count };
            return griddata;
        }

        /// <summary>
        /// 获取列表展示内容
        /// </summary>
        /// <param name="buldMap">复合类</param>
        /// <param name="type">表格名称</param>
        /// <returns>表格内容</returns>
        private object GetGridData(DataModel.ServiceMethodMap buldMap, string type)
        {
            List<ServiceMethodMapDetailViewModel> list = new List<ServiceMethodMapDetailViewModel>();
            int totalCount = 0;
            if (null == buldMap)
            {
                return new { Rows = list, Total = totalCount };
            }
            else
            {
                DataMapCollection map;
                if ("param".Equals(type)) map = buldMap.ParamMaps;
                else map = buldMap.ReturnMaps;

                string[] Names = map.GetParamNames();
                DataModel.DataMap[] mapArray = map.ToArray();
                foreach (OThinker.H3.DataModel.DataMap m in mapArray)
                {
                    ServiceMethodMapDetailViewModel temMap = new ServiceMethodMapDetailViewModel();
                    string ItemName = m.ItemName;
                    foreach (string Name in Names)
                    {
                        if (Name.Contains("[" + m.ItemName + "]"))
                        {
                            ItemName = Name;
                            break;
                        }
                    }
                    temMap.ItemName = ItemName;
                    temMap.MapTo = m.MapTo;
                    temMap.MapType = m.MapType.ToString();
                    list.Add(temMap);
                }
                return new { Rows = list, Total = list.Count };
            }
        }

        /// <summary>
        /// 获取ServiceMethodMap对象
        /// </summary>
        /// <param name="serviceCode">业务编码</param>
        /// <param name="methodName">方法名称</param>
        /// <returns>ServiceMethodMap对象</returns>
        private DataModel.ServiceMethodMap BuldMap(string serviceCode, string methodName)
        {
            if (this.SelectedMap != null)
            {
                return this.SelectedMap;
            }
            string ServiceCode = serviceCode;
            string MethodName = methodName;
            if (string.IsNullOrWhiteSpace(ServiceCode) || string.IsNullOrWhiteSpace(MethodName))
                return null;
            switch (this.Schema.GetMethod(this.SelectedMethod).MethodType)
            {
                case H3.DataModel.MethodType.Filter:
                    OThinker.H3.BizBus.Filter.FilterSchema mapSchema = this.Engine.BizBus.GetFilterSchema(
                        ServiceCode,
                        MethodName);
                    return new DataModel.ServiceMethodMap(
                         ServiceCode,
                         mapSchema);
                case H3.DataModel.MethodType.Normal:
                case H3.DataModel.MethodType.Static:
                    OThinker.H3.BizBus.BizService.MethodSchema mapSchema2 = this.Engine.BizBus.GetMethod(
                        ServiceCode,
                        MethodName);
                    return new DataModel.ServiceMethodMap(
                         ServiceCode,
                         mapSchema2);
                default:
                    throw new NotImplementedException();
            }
        }


        private List<Item> GetBizRuleMethodNamesList(string serviceCode)
        {
            List<Item> list = new List<Item>();
            if (!string.IsNullOrEmpty(serviceCode))
            {
                switch (this.Method.MethodType)
                {
                    case H3.DataModel.MethodType.Filter:
                        // 是搜索
                        OThinker.H3.BizBus.Filter.FilterSchema[] filters = this.Engine.BizBus.GetFilterSchemas(serviceCode);
                        if (filters != null)
                        {
                            foreach (OThinker.H3.BizBus.Filter.FilterSchema f in filters)
                            {
                                list.Add(new Item(f.SchemaCode, f.SchemaCode));
                            }
                        }
                        break;
                    case H3.DataModel.MethodType.Normal:
                    case H3.DataModel.MethodType.Static:
                        BizRuleTable ruleTable = this.Engine.BizBus.GetBizRule(serviceCode);
                        if (ruleTable != null)
                        {
                            foreach (BizRuleDecisionMatrix matrix in ruleTable.DecisionMatrixes)
                            {
                                list.Add(new Item(matrix.DisplayName, matrix.Code));
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取方法列表
        /// </summary>
        /// <param name="serviceCode">业务服务编码</param>
        /// <returns>方法列表</returns>
        private List<Item> GetMethodNamesList(string serviceCode)
        {
            List<Item> list = new List<Item>();
            if (!string.IsNullOrEmpty(serviceCode))
            {
                switch (this.Method.MethodType)
                {
                    case H3.DataModel.MethodType.Filter:
                        // 是搜索
                        OThinker.H3.BizBus.Filter.FilterSchema[] filters = this.Engine.BizBus.GetFilterSchemas(serviceCode);
                        if (filters != null)
                        {
                            foreach (OThinker.H3.BizBus.Filter.FilterSchema f in filters)
                            {
                                list.Add(new Item(f.SchemaCode, f.SchemaCode));
                            }
                        }
                        break;
                    case H3.DataModel.MethodType.Normal:
                    case H3.DataModel.MethodType.Static:
                        OThinker.H3.BizBus.BizService.MethodSchema[] methods = this.Engine.BizBus.GetMethods(serviceCode);
                        if (methods != null)
                        {
                            foreach (OThinker.H3.BizBus.BizService.MethodSchema m in methods)
                            {
                                list.Add(new Item(m.FullName, m.MethodName));
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }
            return list;
        }

        /// <summary>
        /// 获取业务规则或业和业务方法列表
        /// </summary>
        /// <returns>业务规则或业和业务方法列表</returns>
        private List<Item> GetServiceCodesList()
        {
            List<Item> list = new List<Item>();

            BizService[] services = this.Engine.BizBus.GetBizServices();
            //services = this.Engine.WorkflowPackageManager.GetBizServiceBySchemaCode(this.SchemaCode, MethodType);
            if (services != null)
            {
                list.Add(new Item("EditBizObjectSchemaMethodMap.SelectItem", ""));
                foreach (BizService service in services)
                {
                    if (
                        (MethodType.Equals("ServiceMethod") && !service.BizAdapterCode.Equals(DataModel.Declaration.RuleAdapter_Code))
                        ||
                        (MethodType.Equals("RuleMethod") && service.BizAdapterCode.Equals(DataModel.Declaration.RuleAdapter_Code))
                        )
                        list.Add(new Item(service.FullName, service.Code));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取业务服务与业务方法映射模型
        /// </summary>
        /// <param name="mapIndex">输入参数</param>
        /// <returns>业务服务与业务方法映射模型</returns>
        private ServiceMethodMapViewModel GetServiceMethodMapViewModel(string mapIndex)
        {
            ServiceMethodMapViewModel model = new ServiceMethodMapViewModel();

            if (string.IsNullOrEmpty(mapIndex)) mapIndex = "-1";
            string strIndex = mapIndex;
            if (string.IsNullOrEmpty(strIndex) || strIndex == "-1")
            {
                this.SelectedMapIndex = FindSelectedMapIndex("", "");
            }
            else
            {
                this.SelectedMapIndex = int.Parse(strIndex);
            }
            if (this.SelectedMapIndex > -1 && this.Method.MethodMaps.Length > 0)
                this.SelectedMap = this.Method.MethodMaps[this.SelectedMapIndex];
            else
                this.SelectedMap = null;

            //根据当前业务服务是否绑定映射，控制界面按钮显示
            if (this.SelectedMap == null)//新增
            {
                model.IsSelectMap = false;
                model.ExeCondition = string.Empty;
                model.ServiceCode = "";
            }
            else//编辑
            {
                model.ServiceCode = this.SelectedMap.ServiceCode;
                model.MethodName = this.SelectedMap.MethodName;
                model.ExeCondition = this.SelectedMap.ExeCondition;
                model.IsSelectMap = true;
                model.MapIndex = mapIndex;
            }
            model.MethodType = this.MethodType;
            model.SchemaCode = this.SchemaCode;
            model.Method = this.SelectedMethod;
            return model;
        }

        /// <summary>
        /// 查找当前选中的服务，与业务方法里已经做了映射的映射序号
        /// </summary>
        /// <returns></returns>
        private int FindSelectedMapIndex(string serviceCode, string methodName)
        {
            for (int i = 0; i < this.Method.MethodMaps.Length; i++)
            {
                //当前选中的业务服务，且业务服务名称
                if (this.Method.MethodMaps[i].ServiceCode == serviceCode && this.Method.MethodMaps[i].MethodName == methodName)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
