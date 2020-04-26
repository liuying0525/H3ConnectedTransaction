using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.ViewModels;
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
    /// 主数据运行方法控制器
    /// </summary>
    [Authorize]
    public class TestBizObjectController : ControllerBase
    {

        #region 参数
        protected string SchemaCode
        {
            get;
            set;
        }
        protected string SelectedMethod
        {
            get;
            set;
        }
        protected OThinker.H3.DataModel.BizObjectSchema Schema = null;
        /// <summary>
        /// 操作模式
        ///     0：用于测试该BizObjectSchema，不带任何其他参数，默认值
        ///     1：加载模式，用于根据主键加载某个BizObject
        ///     2：创建模式，传入一组参数用于创建某个BizObject
        /// </summary>
        protected int Mode;

        private OThinker.H3.DataModel.BizObject _BizObject = null;
        private OThinker.H3.DataModel.BizObject BizObject
        {
            get
            {
                if (_BizObject == null)
                {
                    _BizObject = new H3.DataModel.BizObject(
                        this.Engine.Organization,
                        this.Engine.MetadataRepository,
                        this.Engine.BizObjectManager,
                        this.Engine.BizBus,
                        this.Schema,
                        this.UserValidator.UserID,
                        this.UserValidator.User.ParentID);
                }
                return this._BizObject;
            }
        }

        private BizObjectSchema _AssociatedSchema = null;
        private BizObjectSchema AssociatedSchema
        {
            get
            {
                if (_AssociatedSchema == null)
                {

                }
                return _AssociatedSchema;
            }
            set
            {
                _AssociatedSchema = value;
            }
        }

        private ActionResult ParseParam(string schemaCode, string mode)
        {
            ActionResult result = new ActionResult();
            this.SchemaCode = schemaCode;
            if (string.IsNullOrEmpty(SchemaCode))
            {
                this.Schema = null;
            }
            else
            {
                this.Schema = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
            }

            if (this.Schema == null)
            {
                result.Success = false;
                result.Message = "TestBizObject.Msg0";
                return result;
            }

            string s_Mode = mode;
            int.TryParse(s_Mode, out this.Mode);
            result.Success = true;
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
        /// 获取运行方法相关信息
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="method">方法名称</param>
        /// <returns>运行方法相关信息</returns>
        [HttpGet]
        public JsonResult GetTestBizObject(string schemaCode, string method)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, "");
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                TestObjectViewModel model = new TestObjectViewModel();
                model.SchemaCode = schemaCode;
                List<Item> methods = GetMethods();
                List<Item> propertyGrid = GetPropertyGrid();
                bool isVisible;//是否显示子表
                List<Item> associationNames = GetAssociationNames(out isVisible);
                if (string.IsNullOrEmpty(model.AssociationName) && null != associationNames.FirstOrDefault()) model.AssociationName = associationNames.FirstOrDefault().Text;
                List<Item> associatedColunms = GetAssociatedColunms();
                List<AssociatedObjectRowViewModel> AssociatedObjectGrid = GetAssociatedObjectGrid(model.AssociationName);
                if (string.IsNullOrEmpty(model.Method)) model.Method = method;
                result.Extend = new
                {
                    TestBizObject = model,
                    Methods = methods,
                    PropertyGrid = propertyGrid,
                    AssociationNames = associationNames,
                    PanelAssociation = isVisible,
                    AssociatedColunms = associatedColunms,
                    Available = false
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 运行方法
        /// </summary>
        /// <param name="propertyGrid">主表参数列表</param>
        /// <param name="model">运行方法模型</param>
        /// <returns>运行结果</returns>
        [HttpPost]
        public JsonResult RunTestBizObject(string propertyGrid, TestObjectViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, "");
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                // 删除与组织结构对应的系统权限
                Dictionary<string, object> valueTableFromUI = new Dictionary<string, object>();
                List<Item> propertyGridList = JsonConvert.DeserializeObject<List<Item>>(propertyGrid);
                foreach (Item item in propertyGridList)
                {
                    string itemName = item.Text;

                    OThinker.H3.DataModel.FieldSchema field = this.Schema.GetField(itemName);

                    if (!string.IsNullOrEmpty(field.Formula))
                    {
                        continue;
                    }

                    string str = item.Value;
                    // 转换成相应的类型
                    object obj = null;
                    if (string.IsNullOrEmpty(str))
                    {
                        obj = OThinker.Data.Convertor.GetDefaultValue(this.Schema.GetField(itemName).RealType);
                    }
                    else if (field.LogicType == Data.DataLogicType.BizObject ||
                        field.LogicType == Data.DataLogicType.BizObjectArray ||
                        field.RealType.IsSubclassOf(typeof(System.Array)))
                    {
                        obj = OThinker.Data.Convertor.XmlToObject(this.Schema.GetField(itemName).RealType, str);
                    }
                    else if (!OThinker.Data.Convertor.Convert(
                         str,
                         field.RealType,
                         ref obj))
                    {
                        //无法转换数据
                        result.Message = "TestBizObject.UnableConvert";
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    valueTableFromUI.Add(itemName, obj);
                }

                // 初始化业务对象
                foreach (string itemName in valueTableFromUI.Keys)
                {
                    //if (string.IsNullOrEmpty(this.Schema.GetField(itemName).OnChange))
                    //{
                    this.BizObject.SetValue(itemName, valueTableFromUI[itemName]);
                    //}
                }
                // 只设置哪些可以设置的属性
                // 初始化业务对象
                foreach (string itemName in valueTableFromUI.Keys)
                {
                    if (this.BizObject.GetPropertyEditable(itemName))//&& !string.IsNullOrEmpty(this.Schema.GetField(itemName).OnChange))
                    {
                        this.BizObject.SetValue(itemName, valueTableFromUI[itemName]);
                    }
                }

                // 检查是不是必填
                string[] itemNames = this.Schema.GetPropertyNames();
                if (itemNames != null)
                {
                    foreach (string itemName in itemNames)
                    {
                        if (this.BizObject.GetPropertyRequired(itemName) &&
                            (this.BizObject[itemName] == null || this.BizObject[itemName].ToString() == ""))
                        {
                            //数据{0}是必填字段 
                            result.Message = "TestBizObject.FieldRequired";
                            result.Extend = itemName;
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }

                // 调用方法
                this.BizObject.Invoke(model.Method);
                bool editable = false;
                bool visiable = false;
                // 回写值
                // 删除与组织结构对应的系统权限
                foreach (Item item in propertyGridList)
                {
                    string itemName = item.Text;

                    // 转换成相应的类型
                    object obj = this.BizObject.GetValue(itemName);
                    OThinker.H3.DataModel.FieldSchema field = this.BizObject.Schema.GetField(itemName);
                    if (obj == null)
                    {
                        item.Value = null;
                    }
                    else if (field.LogicType == Data.DataLogicType.BizObject)
                    {
                        // text.Text = OThinker.Data.Convertor.ObjectToXml(obj);
                    }
                    else if (field.LogicType == Data.DataLogicType.BizObjectArray)
                    {
                        // text.Text = OThinker.Data.Convertor.ObjectToXml(obj);
                    }
                    else if (field.RealType.IsSubclassOf(typeof(System.Array)))
                    {
                        item.Value = OThinker.Data.Convertor.ObjectToXml(obj);
                    }
                    else
                    {
                        item.Value = obj.ToString();
                    }

                    // 加载状态
                    editable = this.BizObject.GetPropertyEditable(itemName);
                    visiable = this.BizObject.GetPropertyVisible(item.Text);
                    if (!this.BizObject.GetPropertyVisible(itemName))
                    {
                        visiable = false;
                    }
                    item.Extend = new
                    {
                        Edit = editable,
                        Visible = visiable
                    };
                }
                var associatedColunms = new List<AssociatedObjectRowViewModel>();
                if (this.Mode == 0)
                {
                    associatedColunms = GetAssociatedObjectGrid(model.AssociationName);
                }
                result.Extend = new
                {
                    PropertyGrid = propertyGridList,
                    AssociatedColunms = associatedColunms,
                    Mode = this.Mode
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取关联表表格数据
        /// </summary>
        /// <param name="associationName"关联的业务对象</param>
        /// <returns>关联表表格数据</returns>
        private List<AssociatedObjectRowViewModel> GetAssociatedObjectGrid(string associationName)
        {
            List<AssociatedObjectRowViewModel> list = new List<AssociatedObjectRowViewModel>();

            if (string.IsNullOrEmpty(associationName))
            {
                return list;
            }
            OThinker.H3.DataModel.BizObjectAssociation asso = this.Schema.GetAssociation(associationName);
            // 获得关联对象的模式
            this.AssociatedSchema = this.Engine.BizObjectManager.GetPublishedSchema(asso.AssociatedSchemaCode);
            if (this.BizObject != null)
            {
                BizObject[] objs = null;
                if (asso.AssociationType == H3.DataModel.AssociationType.OneOne)
                {
                    BizObject obj = (BizObject)this.BizObject.GetValue(asso.Name);
                    if (obj != null)
                    {
                        objs = new BizObject[] { obj };
                    }
                }
                else
                {
                    objs = (BizObject[])this.BizObject.GetValue(asso.Name);
                }

                if (null != objs)
                {
                    PropertySchema[] names = Schema.Properties;
                    foreach (BizObject obj in objs)
                    {
                        if (names != null)
                        {
                            AssociatedObjectRowViewModel model = new AssociatedObjectRowViewModel();
                            model.Values = new List<Item>();
                            foreach (PropertySchema name in names)
                            {
                                if (Data.DataLogicTypeConvertor.IsSubDataTableSupported(name.RealType))
                                {
                                    //值不为Null时直接赋值,值为Null时可转为DBNull或不赋值
                                    if (obj[name.Name] != null)
                                    {
                                        model.Values.Add(new Item(obj[name.Name].ToString(), obj[name.Name].ToString()));
                                    }
                                }
                            }
                            list.Add(model);
                        }
                    }
                }

            }
            foreach (var item in list)
            {
                // 组合主键参数

                Dictionary<string, string> vt = new Dictionary<string, string>();
                PropertySchema[] names = this.AssociatedSchema.Properties;
                for (int count = 0; count < item.Values.Count - 1; count++)
                {
                    string name = names[count].Name;
                    string v = item.Values[count].Value;
                    vt.Add(name, v);
                }
                string url = GetTestBizObjectUrl(this.AssociatedSchema.SchemaCode, 1, vt);
                //lnkEdit.Attributes["onclick"] = WorkSheet.SheetUtility.GetOpenWindowScript(url, false);
                //lnkEdit.Attributes["style"] = "cursor: hand";
                //e.Item.Cells[e.Item.Cells.Count - 1].Controls.Add(lnkEdit);
            }

            return list;
        }

        private string GetTestBizObjectUrl(string SchemaCode, int Mode, Dictionary<string, string> Params)
        {
            System.Text.StringBuilder paramUrl = new System.Text.StringBuilder();
            if (Params != null)
            {
                foreach (string name in Params.Keys)
                {
                    if (paramUrl.Length > 0)
                    {
                        paramUrl.Append("&");
                    }
                    paramUrl.Append(name + "=" + HttpUtility.UrlEncode(Params[name]));
                }
            }

            return System.IO.Path.Combine(this.Request.Url.AbsolutePath, "?" +
                "__" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(SchemaCode) + "&" +
                "__" + ConstantString.Param_Mode + "=" + Mode + "&" +
                paramUrl.ToString());
        }
        /// <summary>
        /// 获取子表表头信息
        /// </summary>
        /// <param name="associationName">关联对象</param>
        /// <returns></returns>
        private List<Item> GetAssociatedColunms()
        {
            List<Item> list = new List<Item>();


            PropertySchema[] names = Schema.Properties;
            if (names != null)
            {
                foreach (PropertySchema name in names)
                {
                    if (Data.DataLogicTypeConvertor.IsSubDataTableSupported(name.RealType))
                    {
                        list.Add(new Item(name.Name, name.RealType.ToString()));
                    }
                }
            }

            return list;
        }
        /// <summary>
        /// 获取关联业务对象下拉框
        /// </summary>
        /// <returns>关联业务对象下拉框</returns>
        private List<Item> GetAssociationNames(out bool isVisible)
        {
            List<Item> list = new List<Item>();
            // 关联关系
            OThinker.H3.DataModel.BizObjectAssociation[] assos = this.Schema.Associations;
            if (assos == null || assos.Length == 0)
            {
                isVisible = false;
            }
            else
            {
                isVisible = true;
                foreach (OThinker.H3.DataModel.BizObjectAssociation asso in assos)
                {
                    list.Add(new Item(asso.Name, asso.Name));
                }
            }
            //BizWorkflowPackageLockByID(this.SchemaCode);
            return list;
        }

        /// <summary>
        /// 获取方法列表
        /// </summary>
        /// <returns>方法列表</returns>
        private List<Item> GetMethods()
        {
            List<Item> list = new List<Item>();

            if (this.Schema.Methods != null)
            {
                foreach (OThinker.H3.DataModel.MethodGroupSchema method in this.Schema.Methods)
                {
                    if (method.MethodType != H3.DataModel.MethodType.Normal)
                    {
                        continue;
                    }
                    else if (this.Mode == 1 &&
                        (method.MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Create ||
                        method.MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Load))
                    {
                        continue;
                    }
                    else if (this.Mode == 2 &&
                        (method.MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Load ||
                        method.MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Remove ||
                        method.MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Update))
                    {
                        continue;
                    }

                    // 检查方法在当前状态下是否可以运行
                    if (!this.BizObject.GetMethodRunnable(method.MethodName))
                    {
                        continue;
                    }
                    list.Add(new Item(method.FullName, method.MethodName));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取PropertyGrid显示数据
        /// </summary>
        /// <returns>PropertyGrid显示数据</returns>
        private List<Item> GetPropertyGrid()
        {
            List<Item> list = new List<Item>();
            // 属性名称列表
            if (this.Schema.Properties != null)
            {
                bool editable = false;
                bool visiable = false;
                foreach (OThinker.H3.DataModel.PropertySchema item in this.Schema.Properties)
                {
                    if (this.Schema.GetAssociation(item.Name) == null)
                    {
                        Item m = new Item();
                        m.Text = item.Name;
                        object v = this.BizObject.GetValue(item.Name);
                        if (this.Schema.PrimaryKey == item.Name && item.SourceType != SourceType.Metadata)//主键不赋值
                        {
                            v = string.Empty;
                        }
                        OThinker.H3.DataModel.FieldSchema field = this.BizObject.Schema.GetField(item.Name);
                        if (v == null)
                        {
                            m.Value = null;
                        }
                        else if (field.LogicType == Data.DataLogicType.BizObject ||
                            field.LogicType == Data.DataLogicType.BizObjectArray ||
                            field.RealType.IsSubclassOf(typeof(System.Array)))
                        {
                            m.Value = OThinker.Data.Convertor.ObjectToXml(v);
                        }
                        else
                        {
                            m.Value = v.ToString();
                        }
                        // 加载状态
                        editable = this.BizObject.GetPropertyEditable(item.Name);
                        visiable = this.BizObject.GetPropertyVisible(item.Name);
                        // 如果是加载模式的话，那么主键禁止修改
                        if (this.Mode == 1)
                        {
                            if (string.Compare(item.Name, BizObjectSchema.PropertyName_ObjectID, true) == 0)
                            {
                                editable = false;
                            }
                        }
                        m.Extend = new
                        {
                            Edit = editable,
                            Visible = visiable
                        };
                        list.Add(m);
                    }
                }
            }
            return list;
        }
    }
}
