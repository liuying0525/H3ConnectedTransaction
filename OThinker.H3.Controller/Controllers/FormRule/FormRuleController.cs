using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OThinker.H3.Data;
using OThinker.H3.Instance.Keywords;
using System.Web.Mvc;
using OThinker.H3.DataModel;
using System.Resources;
using System.Data;
using System.Text.RegularExpressions;

namespace OThinker.H3.Controllers.Controllers.Reporting
{
    public class FormRuleController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }




        /// <summary>
        /// 参与者
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <returns></returns>

        //public ActionResult Participant()
        //{
        //    string workflowCode = this.Request["SchemaCode"];
        //    //获取业务数据线
        //    List<DataLogicType> bizDataTypes = new List<DataLogicType>();
        //    bizDataTypes.Add(DataLogicType.SingleParticipant);
        //    bizDataTypes.Add(DataLogicType.MultiParticipant);
        //    this.ViewBag.BizPropertys = GetBizProperties(workflowCode, bizDataTypes);

        //    //组织机构第一级公司
        //    this.ViewBag.Company = new Organization.Company()
        //    {
        //        UnitId = this.GetOrgnizationFormat(this.Engine.Organization.Company.CompanyId),
        //        Name = this.Engine.Organization.Company.Name
        //    };
        //    //标签

        //    List<Organization.RoleSummary> roleSummaries = this.Engine.Organization.GetAllRolesSummary();
        //    foreach (Organization.RoleSummary item in roleSummaries)
        //    {
        //        item.ObjectId = this.GetOrgnizationFormat(item.ObjectId);
        //    }
        //    this.ViewBag.Roles = roleSummaries.ToArray();
        //    //设置参与者函数
        //    this.ViewBag.ParticipantFunctions = this.SetParticipantFunctions();
        //    this.ViewBag.ParticipantFunctionsJson = this.JSSerializer.Serialize(this.ViewBag.ParticipantFunctions).Replace("\\", "\\\\");
        //    return View();
        //}

        /// <summary>
        /// 节点实例名称
        /// </summary>
        /// <returns></returns>
        //public ActionResult ActivityName()
        //{
        //    return View();
        //}
        /// <summary>
        /// 设置参与者函数
        /// </summary>
        //private List<ParticipantFunction> SetParticipantFunctions()
        //{
        //    List<ParticipantFunction> participantFunctions = new List<ParticipantFunction>();

        //    FunctionHelper[] functionHelpers = Parser.CreateHelper(this.Engine.Organization, this.Engine.BizObjectManager, this.UserContext.IsDeveloper);
        //    foreach (FunctionHelper f in functionHelpers)
        //    {
        //        //只显示经理，逐层经理，角色查找
        //        if (//f.Category != FunctionCategory.Organization
        //            !f.FunctionCategories.Contains(FunctionCategory.Organization)
        //            || f.Name == "Contains"
        //            || f.Name == "GetMembers"
        //            || f.Name == "GetRoles"
        //            || f.Name == "ManagerByLevel"
        //            )
        //        {
        //            continue;
        //        }
        //        if (f.Name == "Contains") continue;
        //        //构造脚本
        //        participantFunctions.Add(new ParticipantFunction()
        //        {
        //            FunctionName = f.Name,
        //            FunctionDisplayName = f.DisplayName,
        //            Helper = f
        //        });
        //    }
        //    return participantFunctions;
        //}
        /// <summary>
        /// 读取业务属性
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="dataType"></param>
        /// <returns></returns>
        private List<FormulaDataItem> GetBizProperties(string schemaCode, List<DataLogicType> dataTypes = null)
        {
            List<FormulaDataItem> bizPrpertys = new List<FormulaDataItem>();
            //发起人 
            bizPrpertys.Add(new FormulaDataItem()
            {
                Name = this.GetPropertyFormat("Originator"),
                DisplayName = "发起人",
                BizDataType = DataLogicType.SingleParticipant
            });
            if (!string.IsNullOrWhiteSpace(schemaCode))
            {
                H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetDraftSchema(schemaCode);
                H3.DataModel.PropertySchema[] Properties = schema.Properties;

                if (Properties != null && Properties.Length != 0)
                {
                    foreach (DataModel.PropertySchema p in Properties)
                    {
                        if (H3.DataModel.BizObjectSchema.IsBOReservedPropertiesOnly(p.Name))
                        {
                            switch (p.Name)
                            {//拥有者
                                case DataModel.BizObjectSchema.PropertyName_OwnerId:
                                    string displayName = p.DisplayName;
                                    if (p.DisplayName == p.Name)
                                    {
                                        displayName = "拥有者";
                                    }
                                    bizPrpertys.Add(new FormulaDataItem()
                                    {
                                        Name = this.GetPropertyFormat(p.Name),
                                        DisplayName = displayName,
                                        BizDataType = DataLogicType.SingleParticipant
                                    });
                                    break;
                            }
                            continue;
                        }
                        if (dataTypes == null || dataTypes.Contains(p.LogicType))
                        {
                            bizPrpertys.Add(new FormulaDataItem()
                            {
                                Name = this.GetPropertyFormat(p.Name),
                                DisplayName = p.DisplayName,
                                BizDataType = p.LogicType
                            });
                        }


                        if (p.LogicType == DataLogicType.BizObjectArray)
                        {
                            foreach (DataModel.PropertySchema cp in p.ChildSchema.Properties)
                            {
                                if (H3.DataModel.BizObjectSchema.IsBOReservedPropertiesOnly(cp.Name)) continue;
                                if (cp.LogicType == DataLogicType.SingleParticipant || cp.LogicType == DataLogicType.MultiParticipant)
                                {
                                    bizPrpertys.Add(new FormulaDataItem()
                                    {
                                        Name = this.GetPropertyFormat(p.Name + "." + cp.Name),
                                        DisplayName = p.DisplayName + "." + cp.DisplayName,
                                        BizDataType = cp.LogicType
                                    });
                                }
                            }
                        }
                    }
                }
            }
            return bizPrpertys.OrderBy(p => p.DisplayName).ToList();
        }
        /// <summary>
        /// 获取数据项的名称格式，{Name}
        /// </summary>
        /// <param name="name"></param>
        private string GetPropertyFormat(string name)
        {
            return "{" + name + "}";
        }
        /// <summary>
        /// 获取组织机构在公式里的ID格式,U(unitid)
        /// </summary>
        /// <param name="unitID"></param>
        /// <returns></returns>
        private string GetOrgnizationFormat(string unitID)
        {
            return "u(" + unitID + ")";
        }

        //设定用于智能感知的函数
        private void setIntelligentFunctions(FormulaModel model, string formulaType)
        {
            FunctionScenarioType functionType = getScenarioType(formulaType);
            List<IntelligentFunction> lstIntelligentFunctions = new List<IntelligentFunction>();
            Dictionary<FunctionCategory, List<FunctionHelper>> functions = new Dictionary<FunctionCategory, List<FunctionHelper>>();
            List<FunctionHelper> li = new List<FunctionHelper>();
            FunctionHelper fun1 = new FunctionHelper();
            fun1.Description = "返回当前时间，精确到时分秒，格式为:yyyy-MM-dd hh:mm:ss";
            fun1.Example = "NOW()";
            fun1.DisplayName = "NOW";
            fun1.Name = "NOW";
            fun1.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Time };
            fun1.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            li.Add(fun1);

            FunctionHelper fun2 = new FunctionHelper();
            fun2.Description = "返回今天的日期，格式为:yyyy-MM-dd";
            fun2.Example = "TODAY()";
            fun2.DisplayName = "TODAY";
            fun2.Name = "TODAY";
            fun2.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Time };
            fun2.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            li.Add(fun2);
            functions.Add(FunctionCategory.Time, li);


            List<FunctionHelper> lis = new List<FunctionHelper>();
            FunctionHelper fun3 = new FunctionHelper();
            fun3.Description = "根据不同分支获取执行结果";
            fun3.Example = "CASE(条件语句,执行表达式,[条件语句,执行表达式])";
            fun3.DisplayName = "CASE";
            fun3.Name = "CASE";
            fun3.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Logic };
            fun3.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            lis.Add(fun3);

            FunctionHelper fun4 = new FunctionHelper();
            fun4.Description = "如果满足条件A，则返回B，否则返回C";
            fun4.Example = "IF(A,B,C)";
            fun4.DisplayName = "IF";
            fun4.Name = "IF";
            fun1.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Logic };
            fun1.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            lis.Add(fun4);
            //functions.Add(FunctionCategory.Logic, lis);
            foreach (var category in functions.Keys)
            {
                foreach (var item in functions[category])
                {
                    if (lstIntelligentFunctions.Find(m => m.FunctionName == item.Name) != null)
                        continue;
                    lstIntelligentFunctions.Add(new IntelligentFunction()
                    {
                        FunctionName = item.Name,
                        //Helper = item
                        Description = item.Description,
                        Example = item.Example
                    });
                }
            }
            model.IntelligentFunctions = lstIntelligentFunctions;
        }

        public string ToBizDataTypeName(string key)
        {
            Dictionary<string, string> lst = new Dictionary<string, string>();
            lst.Add("Unspecified", "空值");
            lst.Add("BoolArray", "逻辑数组型");
            lst.Add("Bool", "逻辑型");
            lst.Add("DateTimeArray", "时间数组型");
            lst.Add("DateTime", "日期型");
            lst.Add("DoubleArray", "双精度数组型");
            lst.Add("Double", "双精度数值型");
            lst.Add("IntArray", "整数数组型");
            lst.Add("Int", "整数");
            lst.Add("LongArray", "长整型数组型");
            lst.Add("Long", "长整数");
            lst.Add("StringArray", "字符串数组型");
            lst.Add("String", "长文本");
            lst.Add("ShortString", "短文本");
            lst.Add("HyperLink", "链接");
            lst.Add("Comment", "审批");
            lst.Add("ByteArray", "二进制流");
            lst.Add("Attachment", "未指定类型的附件");
            lst.Add("TimeSpan", "时间段型");
            lst.Add("SingleParticipant", "参与者（单人）");
            lst.Add("MultiParticipant", "参与者（多人）");
            lst.Add("Html", "Html");
            lst.Add("Object", "对象类型");
            lst.Add("Xml", "Xml");
            lst.Add("Guid", "Guid");
            lst.Add("GuidArray", "Guid数组");
            lst.Add("Decimal", "Decimal");
            lst.Add("DecimalArray", "Decimal数组");
            lst.Add("BizObject", "业务对象");
            lst.Add("BizObjectArray", "业务对象数组");
            lst.Add("BizStructure", "业务结构");
            lst.Add("BizStructureArray", "业务结构数组");
            lst.Add("GlobalData", "全局变量");

            return lst.Where(e => e.Key == key).ToString();

        }

        // 设定函数返回数据类型
        private void setBizDataTypes(FormulaModel model)
        {

            Dictionary<string, object> bizDataTypeDictionary = new Dictionary<string, object>();
            foreach (DataLogicType bizDataType in Enum.GetValues(typeof(DataLogicType)))
            {
                bizDataTypeDictionary.Add(((int)bizDataType).ToString(), new { Name = bizDataType.ToString(), DisplayName = ToBizDataTypeName(bizDataType.ToString()) });
            }
            model.BizDataTypes = bizDataTypeDictionary;
        }

        private FunctionScenarioType getScenarioType(string formulaType)
        {
            FunctionScenarioType scenarioType = FunctionScenarioType.Computation;
            switch (formulaType.ToUpper())
            {
                case "SCHEMASUBMIT"://表单提交规则
                    scenarioType = FunctionScenarioType.Submit;
                    break;
                case "BOOL"://流程路由
                    scenarioType = FunctionScenarioType.Router;
                    break;
                case "DATASOURCE"://数据源
                    scenarioType = FunctionScenarioType.ReportSource;
                    break;
                case "BUSINESS"://业务规则
                    scenarioType = FunctionScenarioType.Business;
                    break;
                case "SCHEMACOMPUTATION"://字段计算规则
                    scenarioType = FunctionScenarioType.Computation;
                    break;
                case "SCHEMADISPLAY"://字段显示规则
                    scenarioType = FunctionScenarioType.Display;
                    break;
                case "ASSOCIATIONFILTER"://关联查询过滤
                    scenarioType = FunctionScenarioType.AssociationFilter;
                    break;
                default:
                    break;
            }
            return scenarioType;
        }







        //private const string ActionName_LoadUnit = "LoadUnit";
        private const string ActionName_InitAppTree = "InitAppTree";
        private const string ActionName_LoadParameter = "LoadParameter";
        private const string ActionName_LoadOrganization = "LoadOrganization";
        private const string ActionName_LoadNamesByUnitIds = "LoadNamesByUnitIds";
        private const string ActionName_InitFunction = "InitFunctionList";
        private const string ActionName_Validate = "Validate";
        private const string ActionName_ParseFormulaText = "ParseFormulaText";
        private const string ActionName_InitDataItems = "InitDataItems";

        public System.Web.Mvc.JsonResult DoAction()
        {
            string actionName = this.Request["ActionName"] ?? "";
            switch (actionName)
            {
                //case ActionName_LoadUnit://根据传入UnitId获取组织机构
                //return this.LoadUnit();
                case ActionName_LoadParameter://加载参数
                    return this.LoadParameter();
                // case ActionName_LoadOrganization:
                // return this.LoadOrganization();
                // case ActionName_LoadNamesByUnitIds:
                // return this.LoadNamesByUnitIds();
                case ActionName_InitAppTree://初始模态树
                    return this.InitModuleTree();
                case ActionName_InitDataItems:
                    return this.InitModuleTree();
                case ActionName_Validate:
                    return this.Validate();
                case ActionName_InitFunction:
                    return this.InitFunctionList();
                case ActionName_ParseFormulaText:
                    return this.ParseFormulaText();
            }
            return Json(new AjaxContext() { ErrorMessage = "参数不对!" });
        }

        /// <summary>
        /// 将Formula解析成FormulaText
        /// </summary>
        /// <returns></returns>
        private JsonResult ParseFormulaText()
        {
            AjaxContext ajaxContext = new AjaxContext();
            string formula = this.Request["Formula"];
            string schemaCode = this.Request["SchemaCode"];
            string schemaCodes = this.Request["SchemaCodes"] ?? "";
            string formulaText = this.ConvertFormulaToText(formula, schemaCode, schemaCodes);
            //  string formulaText = formula;
            ajaxContext.Result.Add("FormulaText", formulaText);
            return Json(ajaxContext);
        }

        //  初始化函数列表
        private JsonResult InitFunctionList()
        {
            string formulaType = this.Request["FormulaType"];
            AjaxContext ajaxContext = new AjaxContext();

            FunctionScenarioType functionType = getScenarioType(formulaType);
            Dictionary<FunctionCategory, List<FunctionHelper>> functions = new Dictionary<FunctionCategory, List<FunctionHelper>>();
            List<FunctionHelper> li = new List<FunctionHelper>();
            FunctionHelper fun1 = new FunctionHelper();
            fun1.Description = "返回当前时间，精确到时分秒，格式为:yyyy-MM-dd hh:mm:ss";
            fun1.Example = "NOW()";
            fun1.DisplayName = "NOW";
            fun1.Name = "NOW";
            fun1.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Time };
            fun1.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            li.Add(fun1);

            FunctionHelper fun2 = new FunctionHelper();
            fun2.Description = "返回今天的日期，格式为:yyyy-MM-dd";
            fun2.Example = "TODAY()";
            fun2.DisplayName = "TODAY";
            fun2.Name = "TODAY";
            fun2.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Time };
            fun2.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            li.Add(fun2);
            functions.Add(FunctionCategory.Time, li);


            List<FunctionHelper> lis = new List<FunctionHelper>();
            FunctionHelper fun3 = new FunctionHelper();
            fun3.Description = "根据不同分支获取执行结果";
            fun3.Example = "CASE(条件语句,执行表达式,[条件语句,执行表达式])";
            fun3.DisplayName = "CASE";
            fun3.Name = "CASE";
            fun3.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Logic };
            fun3.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            lis.Add(fun3);

            FunctionHelper fun4 = new FunctionHelper();
            fun4.Description = "如果满足条件A，则返回B，否则返回C";
            fun4.Example = "IF(A,B,C)";
            fun4.DisplayName = "IF";
            fun4.Name = "IF";
            fun1.FunctionCategories = new FunctionCategory[1] { FunctionCategory.Logic };
            fun1.FunctionScenarioTypes = new FunctionScenarioType[3] { FunctionScenarioType.ReportSource, FunctionScenarioType.Computation, FunctionScenarioType.Display };
            lis.Add(fun4);
            //functions.Add(FunctionCategory.Logic, lis);

            Dictionary<string, List<FunctionHelper>> myFunctions = new Dictionary<string, List<FunctionHelper>>();
            foreach (var key in functions.Keys)
            {
                string newKey = string.Empty;
                switch (key)
                {
                    case FunctionCategory.Math:
                        newKey = "数学函数";
                        break;
                    case FunctionCategory.Logic:
                        newKey = "逻辑函数";
                        break;
                    case FunctionCategory.Organization:
                        newKey = "组织机构函数";
                        break;
                    case FunctionCategory.BackMethod:
                        newKey = "高级函数";
                        break;
                    case FunctionCategory.FrontMethod:
                        newKey = "前端函数";
                        break;
                    case FunctionCategory.Text:
                        newKey = "文本函数";
                        break;
                    case FunctionCategory.Time:
                        newKey = "时间函数";
                        break;
                    case FunctionCategory.Other:
                        newKey = "其他函数";
                        break;
                    default:
                        break;
                }
                myFunctions.Add(newKey, functions[key]);
            }
            ajaxContext.Result.Add("Functions", myFunctions);
            return Json(ajaxContext);
        }
        private JsonResult InitModuleTree()
        {
            string formulaType = this.Request["FormulaType"];
            string schemaCode = this.Request["SchemaCode"];
            string formulaField = this.Request["FormulaField"];
            AjaxContext ajaxContext = new AjaxContext();
            ModelTree tree = new ModelTree();
            if (string.IsNullOrEmpty(formulaType))
            {
                formulaType = "BOOL";
            }
            switch (formulaType.ToUpper())
            {
                case "BUSINESS"://业务规则
                    tree.ShowSystem = true;
                    tree.ShowSystemEN = true;
                    tree.ShowSubSheet = true;
                    tree.ShowAssociation = true;
                    tree.ShowField = true;
                    tree.ShowOrganization = true;
                    tree.ShowPost = true;
                    tree.ShowRules = true;
                    tree.ShowSubSheetField = true;
                    tree.ShowAssociationField = true;
                    tree.ShowSheetAssociation = true;
                    tree.CurSheetCode = schemaCode;
                    tree.AppCode = "root";
                    tree.Display = "应用对象";
                    break;
                case "ACTIVITYNAME":
                    tree.ShowSystem = false;
                    tree.ShowSystemEN = false;
                    tree.ShowSubSheet = false;
                    tree.ShowAssociation = false;
                    tree.ShowField = true;
                    tree.ShowOrganization = false;
                    tree.ShowPost = false;
                    tree.ShowRules = false;
                    tree.ShowSubSheetField = false;
                    tree.ShowAssociationField = false;
                    tree.ShowSheetAssociation = false;
                    tree.AppCode = schemaCode;

                    tree.Display = "业务对象";
                    break;
                case "SCHEMASUBMIT"://表单提交
                    tree.ShowSystem = true;
                    tree.ShowField = true;
                    tree.ShowSubSheet = true;
                    tree.ShowSubSheetField = true;
                    tree.ShowOrganization = true;
                    tree.ShowPost = true;
                    tree.ShowAssociation = true;
                    //tree.ShowAssociationField = true;
                    tree.AppCode = schemaCode;
                    tree.Display = "当前表单";
                    break;
                case "SCHEMACOMPUTATION"://表单控件计算规则
                case "SCHEMADISPLAY"://表单控件显示规则
                    tree.ShowSystem = true;
                    tree.ShowField = true;
                    tree.ShowSubSheet = true;
                    tree.ShowSubSheetField = true;
                    tree.ShowOrganization = true;
                    tree.ShowAssociation = true;
                    tree.ShowPost = true;
                    tree.AppCode = schemaCode;
                    tree.Display = "当前表单";
                    tree.ExcludeFields = formulaField.Split(',');
                    break;
                case "BOOL"://路由条件
                    tree.ShowSystem = true;
                    tree.ShowField = true;
                    tree.ShowSubSheet = true;
                    tree.ShowSubSheetField = true;
                    //tree.ShowAssociation = true;
                    //tree.ShowAssociationField = true;
                    tree.ShowAssociation = true;
                    tree.ShowBizProperties = true;
                    tree.ShowOrganization = true;
                    tree.ShowPost = true;
                    tree.AppCode = schemaCode;
                    tree.Display = "当前表单";
                    break;
                case "DATASOURCE"://报表数据源
                    tree.ShowField = true;
                    tree.CustomCodes = GetCustomCodes();
                    break;
                case "ASSOCIATIONFILTER"://关联查询过滤
                    BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                    //string associationSchemaCode = GetAssociationSchemaCode(schema, formulaField);
                    ////如果关联的是子表，则需要在左侧树中显示其parentSchema
                    //schema = this.Engine.BizObjectManager.GetPublishedSchemaSummary(associationSchemaCode);
                    //if (schema != null && schema.ParentSchema != null)
                    //{
                    //    associationSchemaCode = schema.ParentSchema.SchemaCode;
                    //}
                    tree.ShowSystem = true;
                    tree.ShowField = true;
                    tree.ShowAssociation = true;
                    tree.ShowOrganization = true;
                    tree.ShowPost = true;
                    //tree.ShowAssociationField = true;
                    tree.ShowSubSheet = true;
                    tree.ShowSubSheetField = true;
                    // tree.AppCode = associationSchemaCode;
                    //tree.Display = "当前对象";

                    tree.CurSheetCode = schemaCode;
                    //tree.AppCode = "root";
                    tree.Display = "关联表单";
                    break;
                default:
                    break;
            }
            ajaxContext.Result.Add("AppTree", tree);
            return Json(ajaxContext);
        }
        //private string GetAssociationSchemaCode(BizObjectSchema schema, string propertyName)
        //{
        //    if (propertyName.IndexOf('.') > -1)
        //    {
        //        //子表
        //        string[] names = propertyName.Split(new char[] { '.' });
        //        string childShemaCode = names[0];
        //        propertyName = names[1];
        //        BizObjectSchema childSchema = schema.(childShemaCode);
        //        return childSchema.GetProperty(propertyName).;
        //    }
        //    else
        //    {
        //        return schema.GetProperty(propertyName).AssociationSchemaCode;
        //    }
        //}
        //根据传入的schemacode构造返回的node
        private List<string> GetCustomCodes()
        {
            string codeStr = this.Request["FormulaParameter"];
            string[] codes = codeStr.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            return codes.ToList();
        }
        /// <summary>
        /// 表达式校验
        /// </summary>
        /// <returns></returns>
        private JsonResult Validate()
        {
            string formula = this.Request["Formula"];
            //string TableName = formula.Substring(1, formula.IndexOf('.')-1);
            string formulaType = this.Request["FormulaType"];
            string schemaCode = this.Request["SchemaCode"];
            string dataField = this.Request["DataField"];
            AjaxContext ajaxContext = new AjaxContext();
            string errorMessage = null;
            if (this.Request.ContentLength > 2000)
            {
                ajaxContext.Result.Add("Validate", false);
                ajaxContext.Result.Add("Msg", "表达式太长");
                return Json(ajaxContext);
            }
            // H3.DataModel.Script.Parser parser = new Parser(this.Engine.Organization, this.Engine.BizObjectManager, formula, out errorMessage);

            if (!string.IsNullOrEmpty(errorMessage))
            {
                ajaxContext.Result.Add("Validate", false);
                ajaxContext.Result.Add("Msg", errorMessage);
                return Json(ajaxContext);
            } 
            //DataModel.BizObjectSchemaSummary schema = this.Engine.BizObjectManager.GetDraftSchemaSummary(schemaCode);
            //error 不更新引擎的临时写法
            DataModel.BizObjectSchema schema = null;
            string[] schemacodes = schemaCode.Split(',');
            List<string> fieldValues = new List<string>();
            List<string> TableName = new List<string>();
            //因为有可能会存在多个子表，所以默认第一个元素是主表，后面的元素分别是光临表单的表名
            TableName.Add(schemacodes[0]);
            DataModel.BizObjectSchema fullschema = this.Engine.BizObjectManager.GetPublishedSchema(schemacodes[0]);// this.Engine.BizObjectManager.GetDraftSchemaSummary(schemaCode);
            if (fullschema != null)
            { 
                foreach (var pro in fullschema.Properties)
                {
                    if (pro.LogicType != OThinker.H3.Data.DataLogicType.BizObjectArray)
                    {
                        if (schemacodes.Length>1)
                        {
                            fieldValues.Add(schemacodes[0]+"."+pro.Name);
                        }
                        else
                        {
                            fieldValues.Add(pro.Name);
                        }
                      
                    }
                    if (pro.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                    {

                        foreach (var item in pro.ChildSchema.Fields)
                        {
                            fieldValues.Add(pro.Name + "." + item.Name);
                        }
                        if (schemaCode.IndexOf(pro.Name) != -1)
                        {
                            TableName.Add(pro.Name);
                        }
                    }
                }
            }


            //end不更新引擎的临时写法
            string js = null;
            bool ret = true;
            if (!string.IsNullOrEmpty(formulaType))
            {
                if (formulaType.ToUpper() == FormulaType.SCHEMACOMPUTATION ||
                    formulaType.ToUpper() == FormulaType.SCHEMADISPLAY ||
                    //formulaType.ToUpper() == FormulaType.SCHEMASUBMIT ||
                    formulaType.ToUpper() == FormulaType.BOOL)
                {
                    //js验证
                    //    ret = parser.ParseJs(schema, dataField, out js, out errorMessage, new Dictionary<string, string>(), false);
                }
                else if (formulaType.ToUpper() == FormulaType.SCHEMASUBMIT)
                {
                    // ret = parser.ParseJs(schema, dataField, out js, out errorMessage, new Dictionary<string, string>(), false);
                    //  if (ret)
                    //  {
                    //      string[] variables = parser.GetVariables();

                    //  }
                }
                else if (formulaType.ToUpper() == FormulaType.BUSINESS)
                {
                    //sql验证
                    //H3.DataModel.Script.FileScript fileEvent = null;
                    //ret = parser.ParseBackScript(this.UserContext.Engine.EngineConfig.DbType, schema, false, false, null, out js, out fileEvent, out errorMessage);
                }
                else if (formulaType.ToUpper() == FormulaType.DATASOURCE)
                {

                    ret = VisibleData(formula, TableName, fieldValues, out errorMessage);
                    //报表数据源配置验证
                    // ret = parser.ParseReportSourceSql(this.UserContext.Engine.EngineConfig.DbType, out js, out errorMessage);
                }
                else if (formulaType.ToUpper() == FormulaType.ASSOCIATIONFILTER)
                {
                    ////关联查询过滤条件验证
                    ////关联查询配置的规则必需包含关联表单中字段
                    //H3.DataModel.PropertySchemaSummary property = schema.GetProperty(dataField);
                    //if (dataField.IndexOf(".") > -1)
                    //{
                    //    //子表
                    //    string[] fields = dataField.Split(new char[] { '.' });
                    //    H3.DataModel.BizObjectSchemaSummary childSchema = schema.GetChildSchema(fields[0]);
                    //    property = childSchema.GetProperty(fields[1]);
                    //}
                    //if (property.DataType == Data.BizDataType.Association && !string.IsNullOrEmpty(formula))
                    //{
                    //    bool isContain = false;//标示关联查询的过滤规则中是否包含关联表单中字段
                    //    H3.DataModel.BizObjectSchemaSummary associationSchema = this.Engine.BizObjectManager.GetPublishedSchemaSummary(property.AssociationSchemaCode);
                    //    if (associationSchema == null)
                    //    {
                    //        ajaxContext.Result.Add("Validate", false);
                    //        ajaxContext.Result.Add("Msg", "关联查询字段没有配置关联表单或未保存");
                    //        return Json(ajaxContext);
                    //    }
                    //    foreach (H3.DataModel.PropertySchemaSummary item in associationSchema.Properties)
                    //    {
                    //        if (formula.Contains(item.Name))
                    //        {
                    //            isContain = true;
                    //            break;
                    //        }
                    //    }
                    //    if (!isContain && associationSchema.ParentSchema != null)
                    //    {
                    //        foreach (H3.DataModel.PropertySchemaSummary item in associationSchema.ParentSchema.Properties)
                    //        {
                    //            if (formula.Contains(item.Name))
                    //            {
                    //                isContain = true;
                    //                break;
                    //            }
                    //        }
                    //    }
                    //    if (!isContain)
                    //    {
                    //        ajaxContext.Result.Add("Validate", false);
                    //        ajaxContext.Result.Add("Msg", "表达式未包含关联表单中字段");
                    //        return Json(ajaxContext);
                    //    }
                    //}

                    //H3.Data.Filter.Filter filter = null;
                    //ret = parser.ParseFilter(schema, null, out filter, out errorMessage);
                }
            }
            ajaxContext.Result.Add("Validate", ret);
            ajaxContext.Result.Add("Msg", errorMessage);
            return Json(ajaxContext);
        }

        /// <summary>
        /// 验证的时候 直接拼接一个执行sql 如果执行正确不抛异常 则验证通过
        /// </summary>
        /// <param name="formrule"></param>
        /// <returns></returns>
        public bool VisibleData(string formrule, List<string> tableName, List<string> fieldValues, out string Messages)
        {
            string sqlstart = "";
            string sqlend = "";
            if (tableName.Count > 1)
            {
                //说明是主表和子表
                sqlstart = "SELECT * FROM (SELECT ROW_NUMBER() OVER ( ORDER BY I_" + tableName[0] + ".ObjectId ) AS RowNumber_, ";
                sqlend = "I_" + tableName[0] + ".ObjectId as I_" + tableName[0] + "_ObjectId_Name  FROM I_" + tableName[0];
                string ss = "";
                for (var i = 1; i < tableName.Count; i++)
                {
                    ss = ss + " left join I_" + tableName[i] + " on " + " I_" + tableName[0] + ".ObjectId = I_" + tableName[i] + ".ParentObjectID ";
                }
                sqlend = sqlend + ss + ") T WHERE RowNumber_>=0 AND RowNumber_<=2";



            }
            else
            {
                sqlstart = "SELECT * FROM (SELECT ROW_NUMBER() OVER ( ORDER BY I_" + tableName[0] + ".ObjectId ) AS RowNumber_, ";
                sqlend = "I_" + tableName[0] + ".ObjectId as I_" + tableName[0] + "_ObjectId_Name  FROM I_" + tableName[0] + ") T WHERE RowNumber_>=0 AND RowNumber_<=2";
            }


            string conditions = "";

            if (formrule.IndexOf("{") != -1 || formrule.IndexOf("}") != -1)
            {
                conditions = formrule.Replace("{", " ").Replace("}", " ");

                foreach (var items in fieldValues)
                {
                    if (tableName.Count > 1)
                    {
                        conditions = conditions.Replace(items,  items);
                    }
                    else
                    {
                        conditions = conditions.Replace(items,  "I_"+tableName[0] + "." + items);
                    }


                }
            }
            else
            {
                conditions = formrule;
            }

            //该方法目前只写了两个函数，还有其他函数有待后续开发
            bool result = false;
            if (formrule != null)
            {

                //如果是mysql
                if (this.Engine.EngineConfig.DBType == OThinker.Data.Database.DatabaseType.MySql)
                {
                    //获取当前日期格式：yyyymmdd hh:mm:ss
                    if (formrule.IndexOf("NOW()") > -1)
                    {

                        conditions = formrule.Replace("NOW()", "now()");

                    }
                    //获取当前日期格式：yyyymmdd
                    else if (formrule.IndexOf("TODAY()") > -1)
                    {

                        conditions = formrule.Replace("TODAY()", "date()");

                    }
                }
                //如果是sqlservers
                else if (this.Engine.EngineConfig.DBType == OThinker.Data.Database.DatabaseType.SqlServer)
                {
                    //获取当前日期格式：yyyymmdd hh:mm:ss
                    if (formrule.IndexOf("NOW()") > -1)
                    {

                        conditions = formrule.Replace("NOW()", "getdate()");

                    }
                    //获取当前日期格式：yyyymmdd
                    else if (formrule.IndexOf("TODAY()") > -1)
                    {

                        conditions = formrule.Replace("TODAY()", "CONVERT(varchar(100),getdate(),23)");

                    }
                }   //如果是Oracle
                else if (this.Engine.EngineConfig.DBType == OThinker.Data.Database.DatabaseType.Oracle)
                {

                }



                sqlstart = sqlstart + conditions + " as dada1ada1ad1231a12aa2d1 , " + sqlend;
                try
                {
                    DataTable dt = this.Engine.Query.QueryTable(sqlstart);

                    result = true;
                    Messages = "";

                }
                catch (Exception ex)
                {
                    result = false;
                    Messages = ex.Message;
                }



            }
            else
            {
                Messages = "函数建议不通过";
            }
            return result;
        }
        /// <summary>
        /// 加载组织机构名称
        /// </summary>
        /// <returns></returns>
        //private JsonResult LoadNamesByUnitIds()
        //{
        //    string UnitIdsString = this.Request["UnitIds"] ?? "";
        //    if (string.IsNullOrWhiteSpace(UnitIdsString)) return null;
        //    string[] unitIds = this.JSSerializer.Deserialize<string[]>(UnitIdsString);//.Replace("(", "").Replace(")", "");
        //    if (unitIds == null) return null;
        //    for (int i = 0; i < unitIds.Length; i++)
        //    {
        //        unitIds[i] = unitIds[i].Substring(2, unitIds[i].Length - 3);
        //    }
        //    Dictionary<string, Organization.Unit> unitDictionary = new Dictionary<string, Organization.Unit>();

        //    Organization.Unit[] units = this.Engine.Organization.GetUnits(unitIds);
        //    if (units != null)
        //    {
        //        foreach (Organization.Unit unit in units)
        //        {
        //            if (!unitDictionary.ContainsKey(this.GetOrgnizationFormat(unit.UnitId)))
        //            {
        //                unitDictionary.Add(this.GetOrgnizationFormat(unit.UnitId), unit);
        //            }
        //        }
        //    }

        //    return Json(unitDictionary);
        //}
        ///// <summary>
        ///// 加载组织机构
        ///// </summary>
        ///// <returns></returns>
        //private JsonResult LoadOrganization()
        //{
        //    string parentId = this.Request["ParentId"] ?? "";
        //    parentId = parentId.Substring(2, parentId.Length - 3);//u(xxxx)->xxxx
        //    Dictionary<string, Organization.Unit> returnUnits = new Dictionary<string, Organization.Unit>();

        //    if (string.IsNullOrWhiteSpace(parentId)) return Json(returnUnits);
        //    Organization.Unit[] units = this.Engine.Organization.GetChildUnits(new string[] { parentId }, Organization.UnitType.Unspecified, false, Organization.State.Active);
        //    if (units != null)
        //    {
        //        foreach (Organization.Unit u in units)
        //        {
        //            if (u.UnitType == Organization.UnitType.User || u.UnitType == Organization.UnitType.OrganizationUnit)
        //            {
        //                u.UnitId = this.GetOrgnizationFormat(u.UnitId);
        //                returnUnits.Add(u.UnitId, u);
        //            }
        //        }
        //    }

        //    return Json(returnUnits);
        //}
        private JsonResult LoadParameter()
        {
            string formulaType = this.Request["FormulaType"];
            string formula = this.Request["Formula"];
            string schemaCode = this.Request["SchemaCode"];//当前表单的schemaCode
            string schemaCodes = this.Request["SchemaCodes"] ?? "";//传入的schemaCodes数组
            AjaxContext context = new AjaxContext();
            FormulaModel model = new FormulaModel();
            setIntelligentFunctions(model, formulaType);
            setBizDataTypes(model);//方法返回类型
            var res = new JsonResult();
            res.Data = model;
            context.Result.Add("Parameter", model);
            if (!string.IsNullOrEmpty(formula))
            {
                string formulaText = this.ConvertFormulaToText(formula, schemaCode, schemaCodes);

                context.Result.Add("FormulaText", formulaText);
            }
            return Json(context);
        }
        /// <summary>
        /// 将表达式转成文本
        /// </summary>
        /// <param name="formula">表达式</param>
        /// <param name="schemaCode">当前表单code</param>
        /// <param name="schemaCodes">表达式中字段用到的schemacode，可以不传</param>
        /// <returns></returns>
        private string ConvertFormulaToText(string formula, string schemaCode, string schemaCodes)
        {
            string formulaText = "";
            if (!string.IsNullOrEmpty(formula))
            {
                string[] codes = null;
                if (!string.IsNullOrEmpty(schemaCodes))
                {
                    codes = schemaCodes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    codes = new string[] { schemaCode };
                }
                H3.DataModel.BizObjectSchema currentSchema = this.Engine.BizObjectManager.GetPublishedSchema(schemaCode.Split(',')[0]);
                var node = this.Engine.FunctionAclManager.GetFunctionNodeByCode(schemaCode.Split(',')[0]);
                string FormulaString = formula;
                if (currentSchema != null)
                {
                    FormulaString = formula.Replace(schemaCode.Split(',')[0], node.DisplayName.ToString()).Replace("{I_","{");
                   
                    foreach (var pro in currentSchema.Properties)
                    {
                        if (pro.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                        {
                            FormulaString = FormulaString.Replace(pro.Name.ToString(), pro.DisplayName.ToString());
                            foreach (var item in pro.ChildSchema.Fields)
                            {
                                FormulaString = FormulaString.Replace(item.Name.ToString(), item.DisplayName.ToString());
                            } 
                        }
                        if (pro.LogicType != OThinker.H3.Data.DataLogicType.BizObjectArray)
                        {
                            FormulaString = FormulaString.Replace(pro.Name.ToString(), pro.DisplayName.ToString());
                           
                        }
                    }
                   
                    
                    //PropertySchema[] properties = currentSchema.Properties;
                     
                    //for (int i = 0; i < properties.Length; i++)
                    //{
                    //    FormulaString = FormulaString.Replace(properties[i].Name.ToString(), properties[i].DisplayName.ToString());
                    //}
                }


                formulaText = FormulaString;
                if (formulaText.Contains("{Originator}"))
                {
                    formulaText = formulaText.Replace("{Originator}", "{发起人}");
                }
            }
            return formulaText;
        }
        ///// <summary>
        ///// 加载组织机构
        ///// </summary>
        ///// <returns></returns>
        //private JsonResult LoadUnit()
        //{
        //    string[] unitKeys = (string[])JSSerializer.Deserialize(this.Request["UnitKeys"], typeof(string[]));
        //    AjaxContext context = new AjaxContext();
        //    Dictionary<string, string> organizations = new Dictionary<string, string>();
        //    Organization.Company company = this.Engine.Organization.Company;
        //    List<Organization.RoleSummary> roleSummaries = this.Engine.Organization.GetAllRolesSummary();
        //    Dictionary<string, string> units = new Dictionary<string, string>();
        //    Organization.Unit[] u = this.Engine.Organization.GetUnits(unitKeys);
        //    foreach (Organization.Unit item in u)
        //    {
        //        units.Add(item.UnitId, item.Name);
        //    }
        //    if (!organizations.ContainsKey(company.UnitId) && unitKeys.Contains(company.UnitId))
        //    {
        //        organizations.Add(company.UnitId, company.Name);
        //    }
        //    foreach (var role in roleSummaries)
        //    {
        //        if (!organizations.ContainsKey(role.ObjectId) && unitKeys.Contains(role.ObjectId))
        //        {
        //            organizations.Add(role.ObjectId, role.Name);
        //        }
        //    }
        //    foreach (var key in units.Keys)
        //    {
        //        if (!organizations.ContainsKey(key) && unitKeys.Contains(key))
        //        {
        //            organizations.Add(key, units[key]);
        //        }
        //    }
        //    context.Result.Add("Organization", organizations);
        //    return Json(context);
        //}
    }


    //模型树
    public class ModelTree
    {
        public bool ShowSystem = false;  //是否显示系统字段
        public bool ShowSystemEN = false;  //是否显示系统英文字段
        public bool ShowSubSheet = false; //是否显示子表
        public bool ShowAssociation = false; //是否显示关联查询对象
        public bool ShowField = false; //是否显示字段
        public bool ShowOrganization = false; //是否显示组织机构
        public bool ShowPost = false; //是否显示角色
        public bool ShowRules = false;//是否显示规则
        public bool ShowFunction = false; //是否显示函数公式  
        public bool ShowConst = false;  //是否显示常量
        public bool ShowSubSheetField = false;//是否显示子表字段
        public bool ShowBizProperties = false;//是否显示业务属性字段
        public bool ShowAssociationField = false;//是否显示关联对象字段
        public bool ShowOnlyParticipant = false;//是否只显示参与者类型
        public bool ShowSheetAssociation = false;//是否显示当前表单的关联对象
        public string CurSheetCode = "";//当前表单Code
        //root
        public string AppCode = "";
        public string Display = "";
        public string Icon = "";
        public bool Expand = true;
        public string[] ExcludeFields = new string[] { };//不显示的字段

        //customCode
        public List<string> CustomCodes = new List<string>();
    }
    public class FormulaModel
    {
        public string WorkflowCode { get; set; }
        public string SchemaCode { get; set; }
        public string RuleCode { get; set; }
        public Dictionary<string, string> FormulaGlobalString { get; set; }
        public List<IntelligentFunction> IntelligentFunctions { get; set; }
        public Dictionary<string, object> BizDataTypes { get; set; }

        public ModelTree Tree = new ModelTree();
    }
    //用于智能感知的函数
    public class IntelligentFunction
    {
        public string FunctionName { get; set; }
        public string Description { get; set; }
        public string Example { get; set; }
    }

    // 公式编辑器数据模型对象
    public class FormulaDataItem
    {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public DataLogicType BizDataType { get; set; }
    }

    /// <summary>
    /// 参与者函数模型
    /// </summary>
    public class ParticipantFunction
    {
        public string FunctionName { get; set; }
        public string FunctionDisplayName { get; set; }
        public FunctionHelper Helper { get; set; }
    }
    class FormulaType
    {
        public const string SCHEMADISPLAY = "SCHEMADISPLAY";//显示规则
        public const string SCHEMACOMPUTATION = "SCHEMACOMPUTATION";//计算规则
        public const string BOOL = "BOOL";//路由跳转
        public const string SCHEMASUBMIT = "SCHEMASUBMIT";//表单提交
        public const string BUSINESS = "BUSINESS";//业务逻辑
        public const string DATASOURCE = "DATASOURCE";//报表数据源
        public const string ASSOCIATIONFILTER = "ASSOCIATIONFILTER";//关联查询过滤

    }
    public enum FunctionCategory
    {
        /// <summary>
        /// 数学公式
        /// </summary>
        Math,
        /// <summary>
        /// 短文本
        /// </summary>
        Text,
        /// <summary>
        /// 时间公式
        /// </summary>
        Time,
        /// <summary>
        /// 逻辑表达式
        /// </summary>
        Logic,
        /// <summary>
        /// 组织机构函数
        /// </summary>
        Organization,
        /// <summary>
        /// 前端脚本
        /// </summary>
        FrontMethod,
        /// <summary>
        /// 后台脚本
        /// </summary>
        BackMethod,
        /// <summary>
        /// 其他公式
        /// </summary>
        Other
    }

    public enum FunctionScenarioType
    {
        Unspecified = -1,
        /// <summary>
        /// 流程路由
        /// </summary>
        Router,
        /// <summary>
        /// 计算规则
        /// </summary>
        Computation,
        /// <summary>
        /// 显示规则
        /// </summary>
        Display,
        /// <summary>
        /// 提交规则
        /// </summary>
        Submit,
        /// <summary>
        /// 业务规则
        /// </summary>
        Business,
        /// <summary>
        /// 关联查询过滤
        /// </summary>
        AssociationFilter,
        /// <summary>
        /// 报表数据源
        /// </summary>
        ReportSource
    }


}
