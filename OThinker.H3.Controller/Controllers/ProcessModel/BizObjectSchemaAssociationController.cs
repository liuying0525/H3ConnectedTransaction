using Newtonsoft.Json;
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
    /// <summary>
    /// 关联关系控制器
    /// </summary>
   [Authorize]
    public class BizObjectSchemaAssociationController : ControllerBase
    {
        /// <summary>
        /// 获取节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }
        #region 参数
        protected string SchemaCode
        {
            get;
            set;
        }

        protected string AssociationName
        {
            get;
            set;
        }

        protected OThinker.H3.DataModel.BizObjectSchema Schema;
        protected OThinker.H3.DataModel.BizObjectAssociation Association;

        private ActionResult ParseParam(string schemaCode, string associationName)
        {

            this.SchemaCode = schemaCode;
            this.AssociationName = associationName;
            ActionResult result = new ActionResult();
            if (string.IsNullOrEmpty(this.SchemaCode))
            {
                this.Schema = null;
            }
            else
            {
                this.Schema = this.Engine.BizObjectManager.GetDraftSchema(this.SchemaCode);
            }
            if (this.Schema == null)
            {
                //业务对象模式不存在，或者已经被删除
                result.Message = "BizObjectSchemaAssociation.Msg0";
                result.Success = false;
                return result;
            }
            if (!string.IsNullOrWhiteSpace(AssociationName))
                this.Association = this.Schema.GetAssociation(AssociationName);

            result.Success = true;
            return result;
        }
        #endregion
        /// <summary>
        /// 删除关联关系
        /// </summary>
        /// <param name="names">所选关联关系名称</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>删除结果</returns>
        [HttpPost]
        public JsonResult DelAssociation(string names,string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, "");
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                if (string.IsNullOrEmpty(names)) {
                    result.Success = false;
                    result.Message = "msgGlobalString.SelectItem";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                names = names.TrimEnd(',');
                string[] associationNames = names.Split(',');
                foreach (string name in associationNames)
                {
                    if (!string.IsNullOrWhiteSpace(name))
                        this.Schema.RemoveAssociation(name);
                }
                // 保存
                result.Success= this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 保存关联对象信息
        /// </summary>
        /// <param name="model">关联对象信息</param>
        /// <returns>是否成功</returns>
        public JsonResult SaveAssociation(ObjectSchemaAssociationViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, model.RelationName);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);

                OThinker.H3.DataModel.BizObjectSchema schema = null;
                if (string.IsNullOrEmpty(model.WorkflowID) ||
                    string.IsNullOrEmpty(model.RelationName) ||
                    (schema = this.Engine.BizObjectManager.GetDraftSchema(model.WorkflowID)) == null)
                {
                    //输入的名称或者要关联的业务对象模式不正确
                    result.Message = "BizObjectSchemaAssociation.Msg1";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //if(){}
                if (this.Association == null)
                {
                    this.Association = new DataModel.BizObjectAssociation(
                        model.RelationName,
                        model.DisplayName,
                        (H3.DataModel.AssociationType)Enum.Parse(typeof(AssociationType), model.Type),
                        schema,
                        model.FilterMethod);
                }
                OThinker.H3.DataModel.DataMap[] maps = this.Association.Maps.ToArray();
                if (maps != null)
                {
                    List<AssociationDataMap> propertys = null;
                    if (!string.IsNullOrEmpty(model.PropertyMap))
                    {
                        propertys = JsonConvert.DeserializeObject<AssociationDataMap[]>((model.PropertyMap)).ToList();
                    }
                    if (propertys == null)
                        propertys = new List<AssociationDataMap>();
                    if (propertys.Count == 0)
                    {
                        propertys.Add(new AssociationDataMap()
                        {
                            ItemName = model.ItemName,
                            MapTo = model.MapTo,
                            MapType = model.MapType
                        });
                    }
                    foreach (OThinker.H3.DataModel.DataMap map in maps)
                    {
                        // 影射关系
                        AssociationDataMap property = propertys == null ? null : propertys.Where(p => p.ItemName == map.ItemName).FirstOrDefault();
                        if (property == null)
                            map.MapType = H3.DataModel.DataMapType.None;
                        else
                        {
                            map.MapTo = property.MapTo;
                            map.MapType = (OThinker.H3.DataModel.DataMapType)Enum.Parse(typeof(OThinker.H3.DataModel.DataMapType), property.MapType);
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(model.ObjectID))
                {
                    //添加
                    if (!this.Schema.AddAssociation(this.Association))
                    {
                        //添加失败
                        result.Message = "BizObjectSchemaAssociation.Msg2";
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                //更新
                if (!this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema))
                {
                    //保存业务对象模式失败
                    result.Message = "BizObjectSchemaAssociation.Msg3";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取关联信息
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="associationName">关联对象名称</param>
        /// <returns>关联信息</returns>
        [HttpGet]
        public JsonResult GetAssociation(string schemaCode, string associationName)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, associationName);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                List<AssociationDataMap> map;
                ObjectSchemaAssociationViewModel model = GetAssociationViewModel(out map);
                List<Item> associationTypes = GetAssociationTypes();
                List<Item> filterMethods = GetFilterMethods(model.WorkflowID);
                List<Item> itemNames = GetItemNames(model);
                List<Item> mapTypes = GetMapTypes();
                List<Item> mapToes = GetMapToes();
                model.ItemName = itemNames.FirstOrDefault()==null? "" : itemNames.FirstOrDefault().Value;
                model.MapType = model.MapType ?? mapTypes.FirstOrDefault().Value;
                model.MapTo = model.MapTo ?? mapToes.FirstOrDefault().Value;
                result.Success = true;
                result.Extend = new
                {
                    Association = model,
                    AssociationTypes = associationTypes,
                    ItemNames = itemNames,
                    MapTypes = mapTypes,
                    MapToes = mapToes,
                    FilterMethods = filterMethods,
                    PropertyMap = map
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取关联对象的信息
        /// </summary>
        /// <param name="mapArray">out:被关联对象列表</param>
        /// <returns>关联对象的信息</returns>
        private ObjectSchemaAssociationViewModel GetAssociationViewModel(out  List<AssociationDataMap> mapArray)
        {
            ObjectSchemaAssociationViewModel model = new ObjectSchemaAssociationViewModel();
            mapArray = new List<AssociationDataMap>();
            model.Type = AssociationType.OneOne.ToString();
            model.SchemaCode = SchemaCode;
            if (this.Association == null)
            {
                return model;
            }
            model.ObjectID = this.Association.AssociatedSchemaCode;
            model.RelationName = this.Association.Name;
            model.DisplayName = this.Association.DisplayName;
            model.Type = this.Association.AssociationType.ToString();
            model.FilterMethod = this.Association.FilterMethod;
            OThinker.H3.DataModel.DataMap[] maps = this.Association.Maps.ToArray();
            foreach (OThinker.H3.DataModel.DataMap map in maps)
            {
                if (map.MapType == DataModel.DataMapType.None)
                {
                    continue;
                }
                mapArray.Add(new AssociationDataMap()
                {
                    ItemName = map.ItemName,
                    MapType = map.MapType.ToString(),
                    MapTo = map.MapTo
                });
            }
            model.WorkflowID = this.Association.AssociatedSchemaCode;
            return model;
        }

        private List<Item> GetMapTypes()
        {
            List<Item> list = new List<Item>();
            // 初始化所有影射方式
            foreach (string name in Enum.GetNames(typeof(OThinker.H3.DataModel.DataMapType)))
            {
                if (name != OThinker.H3.DataModel.DataMapType.None.ToString())
                    list.Add(new Item(name, name));
            }
            return list;
        }

        private List<Item> GetMapToes()
        {
            List<Item> list = new List<Item>();
            foreach (string name in this.Schema.GetPropertyNames())
            {
                list.Add(new Item(this.Schema.GetProperty(name).FullName, name));
            }
            return list;
        }
        private List<Item> GetItemNames(ObjectSchemaAssociationViewModel model)
        {
            List<Item> list = new List<Item>();
            OThinker.H3.DataModel.BizObjectSchema associatedSchema = this.Engine.BizObjectManager.GetDraftSchema(model.WorkflowID);
            if (associatedSchema == null) return list;
            OThinker.H3.DataModel.BizObjectAssociation asso = new DataModel.BizObjectAssociation(
            model.RelationName ?? "",
             model.DisplayName ?? "",
             (H3.DataModel.AssociationType)Enum.Parse(typeof(AssociationType), model.Type ?? ""),
             associatedSchema,
             model.FilterMethod ?? "");
            OThinker.H3.DataModel.DataMap[] maps = asso.Maps.ToArray();
            ServiceMethodMap[] methodMaps = associatedSchema.GetMethod("GetList").MethodMaps;
            if (methodMaps != null && methodMaps.Length > 0)
            {
                DataMapCollection paramMaps = associatedSchema.GetMethod("GetList").MethodMaps[0].ParamMaps;
                string[] paramNames = paramMaps.GetParamNames();

                foreach (string propertyName in paramNames)
                {
                    DataMap map = paramMaps.GetMap(propertyName);
                    string mapProperty = map.MapTo;
                    if (!string.IsNullOrEmpty(mapProperty))
                    {
                        DataModel.PropertySchema item = associatedSchema.GetProperty(map.ItemName);
                        if (item != null)
                            list.Add(new Item(item.FullName, item.Name));
                    }
                }
            }
            else
            {
                foreach (OThinker.H3.DataModel.DataMap map in maps)
                {
                    DataModel.PropertySchema item = associatedSchema.GetProperty(map.ItemName);
                    list.Add(new Item(item.FullName, item.Name));
                }
            }
            return list;
        }

        private List<Item> GetFilterMethods(string workflowID)
        {
            List<Item> list = new List<Item>();
            OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetDraftSchema(workflowID);
            if (schema != null)
            {
                foreach (OThinker.H3.DataModel.MethodGroupSchema method in schema.Methods)
                {
                    if (method.MethodType == H3.DataModel.MethodType.Filter)
                    {
                        list.Add(new Item(method.FullName, method.MethodName));
                    }
                }
            }
            return list;
        }

        private List<Item> GetAssociationTypes()
        {
            List<Item> list = new List<Item>();
            // 关联关系
            string[] names = Enum.GetNames(typeof(H3.DataModel.AssociationType));
            foreach (string name in names)
            {
                if (name == "OneOne" || name == "OneMulti")
                {
                    list.Add(new Item(name, name));
                }
                
            }
            return list;
        }

        /// <summary>
        /// 获取关联关系列表
        /// </summary>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="parentId">父ID</param>
        /// <returns>关联关系列表</returns>
        [HttpPost]
        public JsonResult GetAssociationList(PagerInfo pageInfo, string schemaCode, string parentId)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, "");
                List<ObjectSchemaAssociationViewModel> list = new List<ObjectSchemaAssociationViewModel>();
                if (!result.Success)
                {
                    result.Extend = new { Rows = list, Total = list.Count };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                BizObjectAssociation[] asso = this.Schema.Associations;
                if (asso.Length != 0)
                {

                    list = asso.Skip(pageInfo.PageSize * (pageInfo.PageIndex - 1)).Take(pageInfo.PageSize).Select(s => new ObjectSchemaAssociationViewModel()
                    {
                        RelationName = s.Name,
                        Type=s.AssociationType.ToString(),
                        WorkflowID = s.AssociatedSchemaCode

                    }).ToList();
                }
                result.Extend = new { Rows = list, Total = list.Count };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 数据模型选择事件
        /// </summary>
        /// <param name="model">关联模型</param>
        /// <returns>1,过滤方法列表 2,被关联对象列表 </returns>
        [HttpPost]
        public JsonResult WorkflowChange(ObjectSchemaAssociationViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                List<Item> filterMethods = GetFilterMethods(model.WorkflowID);
                string method = filterMethods.FirstOrDefault() == null ? "" : filterMethods.FirstOrDefault().Value;
                model.FilterMethod = method;
                List<Item> itemNames = GetItemNames(model);
                result.Extend = new
                {
                    FilterMethods = filterMethods,
                    ItemNames = itemNames,
                    FilterMethod = method,
                    ItemName = itemNames.FirstOrDefault() == null ? "" : itemNames.FirstOrDefault().Value.ToString()
                };
                result.Success = true;

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 过滤方法选择事件
        /// </summary>
        /// <param name="model">关联关系模型</param>
        /// <returns>被关联对象列表</returns>
        [HttpPost]

        public JsonResult FilterMethodChange(ObjectSchemaAssociationViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                List<Item> itemNames = GetItemNames(model);
                result.Extend = new
                {
                    ItemNames = itemNames,
                };
                result.Success = true;

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
