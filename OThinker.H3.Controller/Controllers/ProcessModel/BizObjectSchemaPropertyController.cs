using OThinker.Data.Database.Serialization;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.DataModel;
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
    /// 数据模型属性控制器
    /// </summary>
    [Authorize]
    public class BizObjectSchemaPropertyController : ControllerBase
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        #region 变量
        /// <summary>
        /// 数据母性编码
        /// </summary>
        private string SchemaCode;
        /// <summary>
        /// 根业务对象
        /// </summary>
        private OThinker.H3.DataModel.BizObjectSchema RootSchema;
        /// <summary>
        /// 当前业务对象
        /// </summary>
        private OThinker.H3.DataModel.BizObjectSchema CurrentSchema;
        /// <summary>
        /// 当前发布属性
        /// </summary>
        private OThinker.H3.DataModel.PropertySchema CurrentPublishedPropertySchema;
        /// <summary>
        ///  当前属性值
        /// </summary>
        protected string SelectedProperty;

        #endregion

        #region 初始化参数
        private OThinker.H3.DataModel.BizObjectSchema _PublishedSchema = null;
        /// <summary>
        /// 获取已发布的数据模型
        /// </summary>
        private OThinker.H3.DataModel.BizObjectSchema PublishedSchema
        {
            get
            {
                if (this._PublishedSchema == null)
                {
                    this._PublishedSchema = this.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
                }
                return this._PublishedSchema;
            }
            set
            {
                this._PublishedSchema = value;
            }
        }

        /// <summary>
        /// 对参数进行解析
        /// </summary>
        /// <param name="property">属性编码</param>
        /// <param name="code">属性模板编码</param>
        /// <param name="parentProperty">父属性</param>
        private void OnParseParam(string property, string code, string parentProperty)
        {
            this.SelectedProperty = HttpUtility.UrlDecode(property) ?? "";
            this.SchemaCode = code;
            if (!string.IsNullOrEmpty(SchemaCode))
            {
                this.RootSchema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
                // 获得之前已经发布的
                string parentPropertyPath = HttpUtility.UrlDecode(parentProperty);//属性路径
                if (!string.IsNullOrEmpty(parentPropertyPath))
                {
                    //因为只有两级结构，如果路径的第一个节点不等于当前编辑的数据项,说明是编辑子对象里的数据项，当前模型是子对象的模型
                    string[] properties = parentPropertyPath.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries);
                    if (!this.SelectedProperty.Equals(properties[0], StringComparison.InvariantCultureIgnoreCase))
                    {
                        this.CurrentSchema = this.RootSchema.GetProperty(properties[0]).ChildSchema;
                        PublishedSchema = (PublishedSchema != null && PublishedSchema.GetProperty(properties[0]) != null) ? PublishedSchema.GetProperty(properties[0]).ChildSchema : null;
                    }
                }

                if (this.CurrentSchema == null)//没有子对象，那么当前就是主对象
                {
                    this.CurrentSchema = this.RootSchema;
                }
                if (PublishedSchema == null)//还没发布
                {
                    this.CurrentPublishedPropertySchema = null;
                }
                else
                {
                    this.CurrentPublishedPropertySchema = PublishedSchema.GetProperty(this.SelectedProperty);
                }
            }
        }
        #endregion
        /// <summary>
        /// 获取模板属性
        /// </summary>
        /// <param name="schemaCode">模型编码</param>
        /// <param name="property">属性编码</param>
        /// <param name="parentProperty">父属性编码</param>
        /// <param name="isPublished">是否发布</param>
        /// <returns>模板信息</returns>
        [HttpGet]
        public JsonResult GetBizObjectSchemaProperty(string schemaCode, string property, string parentProperty, string isPublished)
        {
            return ExecuteFunctionRun(() =>
            {
                //解析参数
                OnParseParam(property, schemaCode, parentProperty);
                ActionResult result = new ActionResult();
                if (this.CurrentSchema == null)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethod.Msg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                BizObjectSchemaPropertyViewModel model;
                if (string.IsNullOrEmpty(SelectedProperty))//新增
                {
                    var logicTypes = GetLogicTypes();
                    var globals = GetGlobals();
                    model = new BizObjectSchemaPropertyViewModel()
                    {
                        SchemaCode = schemaCode,
                        Property = property,
                        ParentProperty = parentProperty,
                        LogicType = logicTypes.FirstOrDefault() != null ? logicTypes.FirstOrDefault().Value.ToString() : "",
                        Global = globals.FirstOrDefault() != null ? globals.FirstOrDefault().Value.ToString() : "",
                        VirtualField = false
                    };
                    result.Extend = new
                    {
                        LogicTypes = logicTypes,
                        Globals = globals,
                        SchemProperty = model,
                    };
                }
                else
                {//编辑
                    OThinker.H3.DataModel.PropertySchema item = this.CurrentSchema.GetProperty(this.SelectedProperty);
                    //全局变量
                    var logicType = item.SourceType == SourceType.Metadata ? OThinker.H3.Data.DataLogicType.GlobalData : item.LogicType;
                    model = new BizObjectSchemaPropertyViewModel()
                    {
                        SchemaCode = schemaCode,
                        Property = property,
                        ParentProperty = parentProperty,
                        PropertyName = item.Name,
                        DisplayName = item.DisplayName,
                        DefaultValue = item.DefaultValue != null ? item.DefaultValue.ToString() : "",
                        Formula = item.Formula,
                        LogicType = logicType.ToString(),
                        Indexed = item.Indexed,
                        VirtualField = item.SerializeMethod == OThinker.Data.Database.Serialization.PropertySerializeMethod.None,
                        RecordTrail = item.Trackable,
                        Searchable = item.Searchable,
                        Global = item.MetadataItemName,
                        IsPublished = Convert.ToBoolean(isPublished)
                    };
                    result.Extend = new
                    {
                        LogicTypes = GetLogicTypes(),
                        Globals = GetGlobals(),
                        SchemProperty = model,
                        IsCodeEdit = this.PublishedSchema != null && this.PublishedSchema.ContainsField(item.Name)
                    };
                }
                result.Success = true;
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存模板属性信息
        /// </summary>
        /// <param name="model">模板熟悉模型</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult SaveBizObjectSchemaProperty(BizObjectSchemaPropertyViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                OnParseParam(model.Property, model.SchemaCode, model.ParentProperty);
                ActionResult result = new ActionResult();
                if (this.CurrentSchema == null)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethod.Msg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //调用保存方法
                bool saveSucces = SaveBizObjectSchemaProperty(model, out result);
                if (!saveSucces) return Json(result, JsonRequestBehavior.AllowGet);

                result.Success = saveSucces;
                var logicTypes = GetLogicTypes();
                var globals = GetGlobals();

                model = new BizObjectSchemaPropertyViewModel()
                {
                    SchemaCode = this.SchemaCode,
                    ParentProperty = model.ParentProperty,
                    LogicType = logicTypes.FirstOrDefault() != null ? logicTypes.FirstOrDefault().Value.ToString() : "",
                    Global = globals.FirstOrDefault() != null ? globals.FirstOrDefault().Value.ToString() : "",
                    VirtualField = false
                };
                result.Extend = new
                {
                    SchemaCode = model.SchemaCode,
                    SchemProperty = model
                };
                return Json(result, JsonRequestBehavior.AllowGet);


            });
        }

        /// <summary>
        /// 删除属性
        /// </summary>
        /// <param name="schemaCode">属性模板编码</param>
        /// <param name="property">属性编码</param>
        /// <param name="parentProperty">父属性编码</param>
        /// <returns>是否能成功</returns>
        [HttpPost]
        public JsonResult DeleteBizObjectSchemaProperty(string schemaCode, string property, string parentProperty)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                OnParseParam(property, schemaCode, parentProperty);
                if (this.CurrentSchema == null)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaMethod.Msg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                this.CurrentSchema.RemoveProperty(this.SelectedProperty);
                // 保存
                if (!this.Engine.BizObjectManager.UpdateDraftSchema(this.RootSchema))
                {
                    result.Success = false;
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        #region 执行保存
        /// <summary>
        /// 执行保存
        /// </summary>
        /// <returns>是否保存成功</returns>
        private bool SaveBizObjectSchemaProperty(BizObjectSchemaPropertyViewModel model, out ActionResult actionResult)
        {
            actionResult = new ActionResult();
            string propertyName = model.PropertyName;
            string displayName = string.IsNullOrWhiteSpace(model.DisplayName) ? propertyName : model.DisplayName;
            // 检查选中的参数
            Data.DataLogicType logicType = (Data.DataLogicType)Enum.Parse(typeof(Data.DataLogicType), model.LogicType);
            object defaultValue;//默认值

            if (model.DefaultValue == string.Empty)
            {
                defaultValue = null;
            }
            else if (!DataValidator(model, propertyName, logicType, out defaultValue, out actionResult))
            {//数据校验
                return false;
            }

            // 校验编码规范
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(BizObjectSchema.CodeRegex);
            if (!regex.IsMatch(propertyName))
            {
                actionResult.Success = false;
                actionResult.Message = "EditBizObjectSchemaProperty.Msg2";
                return false;
            }

            DataModel.PropertySchema newItem = null;
            if (logicType == Data.DataLogicType.BizObject || logicType == Data.DataLogicType.BizObjectArray)
            {
                //业务对象或业务对象数组
                H3.DataModel.BizObjectSchema childSchema = CreateChildSchema(model.PropertyName);
                newItem = new DataModel.PropertySchema(
                         propertyName,
                         displayName,
                         logicType,
                         childSchema);
            }
            else
            {
                PropertySerializeMethod method = PropertySerializeMethod.None;
                int maxLength = 0;
                if (!model.VirtualField)
                {
                    DataLogicTypeConvertor.GetSerializeMethod(logicType, ref method, ref maxLength);
                    if (logicType == DataLogicType.Attachment && this.RootSchema.SchemaCode != this.CurrentSchema.SchemaCode)
                        method = PropertySerializeMethod.Xml;
                }
                SourceType sourceType = logicType == DataLogicType.GlobalData ? SourceType.Metadata : SourceType.Normal;
                newItem = new DataModel.PropertySchema(
                         propertyName,
                         sourceType,
                         method,
                         model.Global,
                        model.Indexed,
                         displayName,
                         logicType == DataLogicType.GlobalData ? DataLogicType.ShortString : logicType,
                         maxLength,
                         defaultValue,
                         model.Formula,
                         string.Empty,//this.txtDisplayValueFormula.Text, // 显示值公式没有实际作用，已注释
                        model.RecordTrail,
                        model.Searchable,
                         false,// this.chkAbstract.Checked   // 摘要没有实际作用，已注释
                         false
                         );
            }

            bool result = true;
            if (!string.IsNullOrEmpty(this.SelectedProperty))
            {//更新
                DataModel.PropertySchema oldPropertySchema = this.CurrentSchema.GetProperty(this.SelectedProperty);
                DataModel.BizObjectSchema published = this.Engine.BizObjectManager.GetPublishedSchema(this.RootSchema.SchemaCode);
                if (published != null
                    && published.GetProperty(propertyName) != null
                    && !Data.DataLogicTypeConvertor.CanConvert(oldPropertySchema.LogicType, newItem.LogicType))
                {
                    actionResult.Success = false;
                    actionResult.Message = "EditBizObjectSchemaProperty.Msg1";
                    return false;
                }
                result = this.CurrentSchema.UpdateProperty(this.SelectedProperty, newItem);
            }
            else
            {
                result = this.CurrentSchema.AddProperty(newItem);
            }
            if (!result)
            {
                // 保存失败
                actionResult.Success = false;
                actionResult.Message = "EditBizObjectSchemaProperty.Msg9";
                return false;
            }

            // 保存
            if (!this.Engine.BizObjectManager.UpdateDraftSchema(this.RootSchema))
            {
                // 保存失败
                actionResult.Success = false;
                actionResult.Message = "msgGlobalString.SaveFailed";
                return false;
            }
            return true;
        }
        #endregion

        #region 数据校验
        /// <summary>
        /// 数据校验
        /// </summary>
        /// <param name="model">数据项</param>
        /// <param name="propertyName">属性名称</param>
        /// <param name="logicType">属性类型</param>
        /// <param name="defaultValue">输出:默认值</param>
        /// <param name="result">输出:检验结果</param>
        /// <returns是否成功></returns>
        private bool DataValidator(BizObjectSchemaPropertyViewModel model, string propertyName, Data.DataLogicType logicType, out object defaultValue, out ActionResult result)
        {
            Type realType = Data.DataLogicTypeConvertor.ToRealType(logicType);
            if (!(logicType == Data.DataLogicType.Double ||
                logicType == Data.DataLogicType.Int ||
                logicType == Data.DataLogicType.Long
                ))
            {
                defaultValue = OThinker.Data.Convertor.GetDefaultValue(realType);//默认值
            }
            else
            {
                defaultValue = null;
            }
            result = new ActionResult();
            if (string.IsNullOrEmpty(propertyName))
            {
                //属性名称不能为空
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.Msg10";
                return false;
            }

            if (propertyName.Length > BizObjectSchema.MaxPropertyNameLength)
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.Msg3";
                result.Extend = BizObjectSchema.MaxPropertyNameLength;
                return false;
            }

            // 数据项必须以字母开始，不让创建到数据库表字段时报错
            // System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[A-Za-z][A-Za-z0-9_]*$");
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
            if (!regex.Match(propertyName).Success)
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.Msg4";
                return false;
            }

            if (OThinker.Data.Database.Database.IsDBKeyWord(propertyName))
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.Msg5";
                return false;
            }

            if (DataModel.BizObjectSchema.IsReservedProperty(propertyName) || propertyName.ToUpper() == "T")
            {
                //不能添加保留属性
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.Msg6";
                result.Extend = propertyName;
                return false;
            }
            else if (!string.IsNullOrEmpty(model.DefaultValue))
            {
                if (logicType != DataLogicType.GlobalData && !OThinker.Data.Convertor.Convert(model.DefaultValue, realType, ref defaultValue))
                {   // 转换失败,无法将默认值转换成选中的类型
                    result.Success = false;
                    result.Message = "EditBizObjectSchemaProperty.Msg7";
                    return false;
                }
            }
            if (PublishedSchema != null && this.SelectedProperty != propertyName && this.PublishedSchema.GetProperty(propertyName) != null)
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.Msg9";
                return false;
            }

            if (logicType == Data.DataLogicType.Association && string.IsNullOrEmpty(model.DefaultValue))
            {
                result.Success = false;
                result.Message = "EditBizObjectSchemaProperty.AssociationCodeRequired";
                return false;
            }

            // 业务对象数组时，判断子业务对象编码是否已经被其他的数据模型占用
            if (logicType == Data.DataLogicType.BizObject || logicType == Data.DataLogicType.BizObjectArray)
            {
                string childSchemaCode = model.PropertyName; // this.txtChildSchemaCode.Text.Trim();
                // 修改时，子业务对象编码不能修改，所以只在新增时进行验证
                if (string.IsNullOrEmpty(this.SelectedProperty))
                {
                    if (this.CurrentSchema.Properties.Any(p => p.LogicType == logicType && p.ChildSchema.SchemaCode == childSchemaCode))
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchemaProperty.Msg8";
                        return false;
                    }
                    string msg = "";
                    if (!this.Engine.BizObjectManager.CheckSchemaCodeDuplicated(childSchemaCode, out msg))
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchemaProperty.Msg8";
                        return false;
                    }
                }
            }
            return true;
        }
        #endregion

        #region 创建子对象
        /// <summary>
        /// 创建子对象
        /// </summary>
        /// <param name="propertyName">属性名称</param>
        /// <returns>子对象</returns>
        private DataModel.BizObjectSchema CreateChildSchema(string propertyName)
        {
            DataModel.PropertySchema oldPropertySchema = this.CurrentSchema.GetProperty(this.SelectedProperty);
            H3.DataModel.BizObjectSchema childSchema = null;
            if (oldPropertySchema == null || (oldPropertySchema != null && oldPropertySchema.ChildSchema == null))
            {
                childSchema = new DataModel.BizObjectSchema(
                    this.CurrentSchema.StorageType == DataModel.StorageType.PureServiceBased ? DataModel.StorageType.PureServiceBased : DataModel.StorageType.DataList,
                  propertyName, //this.txtChildSchemaCode.Text,
                    false);
            }
            else
            {
                childSchema = oldPropertySchema.ChildSchema;
                childSchema.SchemaCode = propertyName;
            }
            return childSchema;
        }
        #endregion

        /// <summary>
        /// 获取全局变量
        /// </summary>
        /// <returns></returns>
        private List<Item> GetGlobals()
        {
            Data.PrimitiveMetadata[] itemNames = this.Engine.MetadataRepository.GetAllPrimitiveItems();
            List<Item> list = new List<Item>();
            foreach (var s in itemNames)
            {
                list.Add(new Item(s.ItemName, s.ItemName));
            }
            return list;
        }
        /// <summary>
        /// 属性类型
        /// </summary>
        /// <returns></returns>
        private List<Item> GetLogicTypes()
        {
            List<Item> list = new List<Item>();
            OThinker.H3.Data.DataLogicType[] types = OThinker.H3.Data.DataLogicTypeConvertor.GetBizObjectSupportLogicTypes();
            string[] normal = { "ShortString", "String", "Bool", "Int", "Long", "Double", "DateTime", "Attachment", "Comment", "BizObjectArray", "SingleParticipant", "MultiParticipant", "TimeSpan", "Html", "HyperLink", "GlobalData", "Association" };
            foreach (OThinker.H3.Data.DataLogicType logicType in types)
            {
                //子数据模型(子业务对象)的数据项，只能选择非业务对象的属性类型（注：这个改动，就变成只有两级结构）
                if (this.CurrentSchema != null && this.CurrentSchema != this.RootSchema)
                {
                    if (logicType == OThinker.H3.Data.DataLogicType.BizObject
                        || logicType == OThinker.H3.Data.DataLogicType.BizObjectArray
                        || logicType == OThinker.H3.Data.DataLogicType.Comment)
                    {
                        continue;
                    }
                }
                string typeName = Data.DataLogicTypeConvertor.ToLogicTypeName(logicType);
                if (normal.Contains(logicType + string.Empty))
                    list.Add(new Item(typeName, logicType.ToString()));
                //if (this.CurrentSchema.GetProperty(this.SelectedProperty) != null && this.CurrentSchema.GetProperty(this.SelectedProperty).LogicType != logicType)
                //    list.Add(new Item(typeName, logicType.ToString()));
            }
            return list;
        }
    }
}
