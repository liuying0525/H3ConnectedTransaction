using Newtonsoft.Json;
using OThinker.Data;
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
    /// 查询列表控制器
    /// </summary>
    [Authorize]
    public class BizQueryController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }
        #region 参数
        /// <summary>
        /// SchemaObject编码
        /// </summary>
        public string SchemaCode
        {
            get;
            set;
        }

        private bool _SheetsLoaded = false;
        private Sheet.BizSheet[] _Sheets;
        public Sheet.BizSheet[] Sheets
        {
            get
            {
                if (!_SheetsLoaded)
                {
                    _SheetsLoaded = true;
                    this._Sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(this.SchemaCode);
                }
                return this._Sheets;
            }
        }

        /// <summary>
        /// 查询对象的编码
        /// </summary>
        public string QueryCode
        {
            get;
            set;
        }

        private BizQuery currentQuery;
        /// <summary>
        /// 获取当前的对象
        /// </summary>
        public BizQuery CurrentQuery
        {
            get
            {
                if (currentQuery == null)
                {
                    List<BizQueryAction> lstBizQueryActions = new List<BizQueryAction>();
                    if (Sheets != null && Sheets.Length > 0)
                    {
                        lstBizQueryActions.Add(new BizQueryAction()
                        {
                            ActionCode = "AddNew",
                            DisplayName = "GlobalButton.Add",
                            ActionType = OThinker.H3.DataModel.ActionType.OpenBizSheet,
                            BizSheetCode = Sheets[0].SheetCode,
                            WithID = OThinker.Data.BoolMatchValue.False,
                            AfterSave = SheetAfterSaveAction.CloseAndRefreshParent,
                            Icon = "fa fa-plus",
                            IsDefault = BoolMatchValue.True
                        });
                        lstBizQueryActions.Add(new BizQueryAction()
                        {
                            ActionCode = "Edit",
                            DisplayName = "GlobalButton.Edit",
                            ActionType = OThinker.H3.DataModel.ActionType.OpenBizSheet,
                            BizSheetCode = Sheets[0].SheetCode,
                            WithID = OThinker.Data.BoolMatchValue.True,
                            AfterSave = SheetAfterSaveAction.CloseAndRefreshParent,
                            Icon = "fa fa-edit",
                            IsDefault = BoolMatchValue.True
                        });
                    }

                    lstBizQueryActions.Add(new BizQueryAction()
                    {
                        ActionCode = "Delete",
                        DisplayName = "GlobalButton.Remove",
                        ActionType = OThinker.H3.DataModel.ActionType.BizMethod,
                        BizMethodName = DataModel.BizObjectSchema.MethodName_Remove,
                        WithID = OThinker.Data.BoolMatchValue.True,
                        Confirm = OThinker.Data.BoolMatchValue.True,
                        Icon = "fa fa-minus",
                        IsDefault = BoolMatchValue.True
                    });
                    currentQuery = new BizQuery()
                    {
                        BizActions = lstBizQueryActions.ToArray()
                    };
                }
                return currentQuery;
            }
            set
            {
                currentQuery = value;
            }
        }

        private string _selectMethod;
        /// <summary>
        /// 选中方法
        /// </summary>
        public string SelectMethod
        {
            get
            {
                if (!string.IsNullOrEmpty(_selectMethod))
                    return _selectMethod;
                return "";
            }
        }
        /// <summary>
        /// 业务对象
        /// </summary>
        private OThinker.H3.DataModel.BizObjectSchema Schema
        {
            get
            {
                if (null == this.Engine.BizObjectManager.GetPublishedSchema(this.SchemaCode))
                {
                    return new BizObjectSchema();
                }
                return this.Engine.BizObjectManager.GetPublishedSchema(this.SchemaCode);
            }
        }

        List<string> _VisiblePropertyNames = null;
        /// <summary>
        /// 可见的属性名称,(Name,DisplayName)
        /// </summary>
        List<string> VisiblePropertyNames
        {
            get
            {
                if (_VisiblePropertyNames == null)
                {
                    _VisiblePropertyNames = new List<string>();

                    if (this.Schema != null && this.Schema.Properties != null)
                    {
                        //映射的业务服务方法
                        OThinker.H3.DataModel.ServiceMethodMap methodMap = null;
                        //如果GetList方法源于DataTableAdapter业务服务方法,则只显示映射的业务服务中存在的字段
                        if (!string.IsNullOrEmpty(SelectMethod))
                        {
                            OThinker.H3.DataModel.MethodGroupSchema method = this.Schema.GetMethod(SelectMethod);
                            if (method != null && method.MethodMaps != null && method.MethodMaps.Count() > 0)
                            {
                                //因为GetList类型方法只允许绑定一个业务服务方法,所以取第一个就OK了
                                methodMap = method.MethodMaps[0];
                            }
                        }

                        if (methodMap != null)
                        {
                            if (methodMap.ReturnMaps != null)
                            {
                                string[] returns = methodMap.ReturnMaps.GetReferencedPropertyNames();
                                if (returns != null)
                                {
                                    foreach (string r in returns)
                                    {
                                        _VisiblePropertyNames.Add(r);
                                    }
                                }
                            }
                        }
                        else
                        {
                            foreach (PropertySchema property in this.Schema.Properties)
                            {
                                //附件、审批意见、子表对象不支持查询显示
                                if (property.LogicType == OThinker.H3.Data.DataLogicType.Comment||
                                    property.LogicType==OThinker.H3.Data.DataLogicType.Attachment||
                                    property.LogicType==OThinker.H3.Data.DataLogicType.BizObject||
                                    property.LogicType==OThinker.H3.Data.DataLogicType.BizObjectArray||
                                    property.LogicType==OThinker.H3.Data.DataLogicType.BizStructure||
                                    property.LogicType==OThinker.H3.Data.DataLogicType.BizStructureArray)
                                    continue;
                                if (property.Name == OThinker.H3.DataModel.BizObjectSchema.PropertyName_ModifiedBy||
                                    property.Name == OThinker.H3.DataModel.BizObjectSchema.PropertyName_ObjectID||
                                    property.Name == OThinker.H3.DataModel.BizObjectSchema.PropertyName_RunningInstanceId)
                                    continue;
                                _VisiblePropertyNames.Add(property.Name);
                            }
                        }
                    }
                }
                return _VisiblePropertyNames;
            }
        }

        private List<BizQueryItem> queryItemsTable;
        /// <summary>
        /// 查询条件的表格
        /// </summary>
        protected List<BizQueryItem> QueryItemsTable
        {
            get
            {
                if (queryItemsTable == null)
                {
                    queryItemsTable = new List<BizQueryItem>();
                    if (CurrentQuery.QueryItems != null)
                    {
                        foreach (BizQueryItem item in CurrentQuery.QueryItems)
                        {
                            if (this.Schema != null && this.Schema.GetProperty(item.PropertyName) != null)
                                queryItemsTable.Add(item);
                        }
                    }
                }

                return queryItemsTable;
            }
            set
            {
                queryItemsTable = value;
            }
        }

        private List<BizQueryAction> queryActionsTable;
        /// <summary>
        /// 功能编码的表格
        /// </summary>
        protected List<BizQueryAction> QueryActionsTable
        {
            get
            {
                if (queryActionsTable == null)
                {
                    queryActionsTable = new List<BizQueryAction>();
                    if (CurrentQuery.BizActions != null)
                    {
                        foreach (BizQueryAction Action in CurrentQuery.BizActions)
                        {
                            queryActionsTable.Add(Action);
                        }
                    }
                }

                return queryActionsTable;
            }
            set
            {
                queryActionsTable = value;
            }

        }

        #endregion
        /// <summary>
        /// 检验数据模型是否已经发布
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult IsSchemaPublished(string schemaCode, string ownSchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result.Success = null != this.Engine.BizObjectManager.GetPublishedSchema(schemaCode);
                result.Message = result.Success ? "" : "BizQuery.SchemaNotExistOrNotPublished";
                result.Extend = BizWorkflowPackageLockByID(schemaCode, ownSchemaCode);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取查询列表
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>查询列表数据</returns>
        [HttpPost]
        public JsonResult GetBizQueryList(PagerInfo pagerInfo, string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                BizQuery[] queries = this.Engine.BizObjectManager.GetBizQueries(schemaCode);
                int total = queries == null ? 0 : queries.Length;
                List<BizQueryViewMode> list = new List<BizQueryViewMode>();
                if (queries != null)
                {
                    queries = queries.Skip((pagerInfo.PageIndex - 1) * pagerInfo.PageSize).Take(pagerInfo.PageSize).ToArray();
                    list = queries.Select(s => new BizQueryViewMode()
                    {
                        DisplayName = s.DisplayName,
                        QueryCode = s.QueryCode,
                        ObjectID = s.ObjectID,
                        SchemaCode = s.SchemaCode,
                        ListMethod = s.ListMethod,
                        QueryWhenLoad = s.ListDefault == BoolMatchValue.True,
                        Columns = s.Columns

                    }).ToList();
                }
                result.Extend = new { Rows = list, Total = total };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除查询列表
        /// </summary>
        /// <param name="codes">查询实例编码</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelBizQuery(string codes)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();

                string queryCodes = codes;
                if (!string.IsNullOrWhiteSpace(queryCodes))
                {
                    string[] codeArray = queryCodes.Split(';');
                    foreach (string code in codeArray)
                    {
                        if (string.IsNullOrEmpty(code))
                        {
                            continue;
                        }
                        this.Engine.BizObjectManager.RemoveBizQuery(this.Engine.BizObjectManager.GetBizQuery(code));
                    }
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取查询列表页面基础信息
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="queryCode">查询列表编码</param>
        /// <returns>查询列表信息</returns>
        [HttpGet]
        public JsonResult GetBizQuery(string schemaCode, string queryCode)
        {
            return ExecuteFunctionRun(() =>
            {
                this.SchemaCode = schemaCode;
                this.QueryCode = queryCode;
                currentQuery = this.Engine.BizObjectManager.GetBizQuery(this.QueryCode);
                ActionResult result = new ActionResult();
                BizQueryViewMode model = GetBizQueryViewMode(schemaCode, queryCode);
                List<Item> listMethods = GetListMethods();
                if (listMethods.FirstOrDefault() != null)
                {
                    model.ListMethod = string.IsNullOrEmpty(model.ListMethod) ? listMethods.FirstOrDefault().Value : model.ListMethod;
                }
                _selectMethod = model.ListMethod;
                //查询方法列表
                //属性列表表格
                List<BizQueryPropertyViewModel> propertys = GetPropertys();
                //条件列表表格
                List<BizQueryConditionViewModel> conditions = GetConditions();
                List<Item> conditionColumns = GetConditionColumns();
                List<Item> filterTypes = GetFilterTypes();
                List<Item> defaultValues = GetDefaultValues();
                List<Item> displayTypes = GetDisplayTypes();
                //功能编码表格
                List<BizQueryActionViewModel> actions = GetActions();
                List<Item> actionSheets = GetActionSheets();
                List<Item> actionTypes = GetActionTypes(actionSheets.Count);
                List<Item> actionMethods = GetActionMethods();
                List<Item> saveCompletes = GetSaveCompletes();
                result.Extend = new
                {
                    BizQuery = model,
                    ListMethods = listMethods,
                    Propertys = propertys,
                    ConditionColumns = conditionColumns,
                    FilterTypes = filterTypes,
                    DefaultValues = defaultValues,
                    Conditions = conditions,
                    DisplayTypes = displayTypes,
                    Actions = actions,
                    ActionTypes = actionTypes,
                    ActionMethods = actionMethods,
                    ActionSheets = actionSheets,
                    SaveCompletes = saveCompletes,
                    IsLocked = BizWorkflowPackageLockByID(this.SchemaCode)
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 检查条件是否合法
        /// </summary>
        /// <param name="coditions">查询条件列表</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="queryCode">查询列表编码</param>
        /// <returns>是否合法</returns>
        [HttpPost]
        public JsonResult CheckBizQueryCondition(string conditions, string schemaCode, string queryCode)
        {
            return ExecuteFunctionRun(() =>
            {
                this.SchemaCode = schemaCode;
                this.QueryCode = queryCode;
                currentQuery = this.Engine.BizObjectManager.GetBizQuery(this.QueryCode);
                List<BizQueryConditionViewModel> list = JsonConvert.DeserializeObject<BizQueryConditionViewModel[]>(conditions).ToList();
                ActionResult result = SaveQueryItem(list);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存查询列表信息
        /// </summary>
        /// <param name="model">查询列表详情</param>
        /// <param name="propertys">属性设置</param>
        /// <param name="conditions">条件设置</param>
        /// <param name="actions">动作设置</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        [ValidateInput(false)]
        public JsonResult SaveBizQuery(BizQueryViewMode model, string propertys, string conditions, string actions)
        {
            return ExecuteFunctionRun(() =>
            {
                this.SchemaCode = model.SchemaCode;
                this.QueryCode = model.QueryCode;
                currentQuery = this.Engine.BizObjectManager.GetBizQuery(this.QueryCode);
                ActionResult result = new ActionResult();
                //属性列表
                List<BizQueryPropertyViewModel> propertysList = new List<BizQueryPropertyViewModel>();
                //条件列表表格
                List<BizQueryConditionViewModel> conditionsList = new List<BizQueryConditionViewModel>();
                //功能编码表格
                List<BizQueryActionViewModel> actionsList = new List<BizQueryActionViewModel>();
                if (!string.IsNullOrEmpty(propertys))
                {
                    propertysList = JsonConvert.DeserializeObject<List<BizQueryPropertyViewModel>>(propertys);
                }
                if (!string.IsNullOrEmpty(conditions))
                {
                    conditionsList = JsonConvert.DeserializeObject<List<BizQueryConditionViewModel>>(conditions);
                } if (!string.IsNullOrEmpty(actions))
                {
                    actionsList = JsonConvert.DeserializeObject<List<BizQueryActionViewModel>>(actions);
                }
                CurrentQuery.QueryCode = model.QueryCode;
                if (string.IsNullOrEmpty(CurrentQuery.QueryCode))
                {
                    result.Message = "BizQuery.CodeNotNull";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //新增时需判断code全局唯一
                if (string.IsNullOrEmpty(model.OrginQueryCode) && this.Engine.BizObjectManager.GetBizQuery(CurrentQuery.QueryCode) != null)
                {
                    result.Message = "BizQuery.CodeExist";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                BizQuery q = this.Engine.BizObjectManager.GetBizQuery(CurrentQuery.QueryCode);
                bool isNew = true;
                if (q != null)
                {
                    isNew = false;
                    CurrentQuery = q;
                }
                CurrentQuery.DisplayName = model.DisplayName;
                CurrentQuery.SchemaCode = this.SchemaCode;

                CurrentQuery.ListDefault = model.QueryWhenLoad ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;

                if (string.IsNullOrEmpty(model.ListMethod))
                {
                    result.Message = "BizQuery.Mssg1";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                CurrentQuery.ListMethod = model.ListMethod;

                // 保存查询条件
                result = SaveQueryItem(conditionsList);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);

                //保存属性设置
                result = SaveColumnSets(propertysList);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);

                //保存功能动作
                result = SaveQueryAction(actionsList);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);


                CurrentQuery.QueryItems = this.QueryItemsTable.ToArray();
                CurrentQuery.BizActions = this.QueryActionsTable.ToArray();
                if ((isNew && this.Engine.BizObjectManager.AddBizQuery(CurrentQuery)) || (!isNew && this.Engine.BizObjectManager.UpdateBizQuery(CurrentQuery)))
                {
                    result.Message = "msgGlobalString.SaveSucced";
                    result.Success = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取生成页面基本信息
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>生成页面基本信息</returns>
        [HttpGet]
        public JsonResult GetGenerateBizQuery(string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = schemaCode;
                BizQueryPageViewModel model = new BizQueryPageViewModel();
                model.SchemaCode = this.SchemaCode;
                List<Item> querys = GetBizQueryList();
                if (querys.Any()) model.Query = querys.FirstOrDefault().Value;
                result.Extend = new
                {
                    BizQueryPage = model,
                    Querys = querys
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 生成页面
        /// </summary>
        /// <param name="model">页面信息</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult GenerateBizQuery(BizQueryPageViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                this.SchemaCode = model.SchemaCode;

                string listPageName = model.ListClassName;
                string queryCode = model.Query;
                if (string.IsNullOrEmpty(listPageName))
                {
                    result.Message = "BizQuery.ClassEmpty";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (string.IsNullOrEmpty(queryCode))
                {
                    result.Message = "BizQuery.CheckCode";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                model.ListASPXPage = this.GenerateListASPXPage(listPageName);
                model.ListCSPage = this.GenerateListCSPage(listPageName, queryCode);

                result.Success = true;
                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取查询列表
        /// </summary>
        /// <returns>查询列表</returns>
        private List<Item> GetBizQueryList()
        {
            List<Item> list = new List<Item>();
            BizQuery[] queries = this.Engine.BizObjectManager.GetBizQueries(this.SchemaCode);
            if (queries != null)
            {
                foreach (BizQuery query in queries)
                {
                    list.Add(new Item(query.DisplayName, query.QueryCode));
                }
            }
            return list;
        }

        private ActionResult SaveQueryAction(List<BizQueryActionViewModel> actionsList)
        {
            ActionResult result = new ActionResult();
            if (actionsList == null || actionsList.Count == 0)
            {
                QueryActionsTable = new List<BizQueryAction>();
                result.Success = true;
                return result;
            }
            List<BizQueryAction> bizQueryActions = new List<BizQueryAction>();
            foreach (BizQueryActionViewModel m in actionsList)
            {
                string objectId = m.ObjectID;
                BizQueryAction query = QueryActionsTable.Find((action) => { return action.ObjectID == objectId; });
                if (query == null)
                {
                    query = new BizQueryAction();
                    query.ObjectID = Guid.NewGuid().ToString();
                    query.ActionCode = m.ActionCode;
                    query.DisplayName = m.DisplayName;
                    query.ActionType = (ActionType)m.ActionType;
                    query.BizMethodName = m.ActionMethod;
                    query.BizSheetCode = m.ActionSheet;
                    query.Confirm = m.Confirm ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    query.WithID = m.WithID ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    query.Url = m.Url;
                    query.AfterSave = (SheetAfterSaveAction)m.SaveCompleted;
                    query.SortKey = m.SortKey;
                    query.Visible = m.Visible ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    query.IsDefault = m.IsDefault ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                }
                else
                {
                    query.ActionCode = m.ActionCode;
                    query.DisplayName = m.DisplayName;
                    query.ActionType = (ActionType)m.ActionType;
                    query.BizMethodName = m.ActionMethod;
                    query.BizSheetCode = m.ActionSheet;
                    query.Confirm = m.Confirm ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    query.WithID = m.WithID ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                    query.Url = m.Url;
                    query.AfterSave = (SheetAfterSaveAction)m.SaveCompleted;
                    query.SortKey = m.SortKey;
                    query.Visible = m.Visible ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False;
                }
                bizQueryActions.Add(query);

            }
            QueryActionsTable = bizQueryActions;
            result.Success = true;

            return result;
        }

        /// <summary>
        /// 保存属性列表
        /// </summary>
        /// <param name="propertysList">属性列表</param>
        /// <returns>保存结果</returns>
        private ActionResult SaveColumnSets(List<BizQueryPropertyViewModel> propertysList)
        {
            ActionResult result = new ActionResult();
            //是否存在可见列
            bool existVisibleColumn = false;
            List<BizQueryColumn> lstColumns = new List<BizQueryColumn>();
            foreach (BizQueryPropertyViewModel m in propertysList)
            {
                int width = m.Width;
                existVisibleColumn = propertysList.Any(s => s.Visible == true);
                lstColumns.Add(new BizQueryColumn()
                {
                    PropertyName = m.NameValue,
                    Visible = m.Visible ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False,
                    //Sortable = ((CheckBox)Row.FindControl("chkSortable")).Checked ? OThinker.Data.BoolMatchValue.True : OThinker.Data.BoolMatchValue.False,
                    Width = width,
                    DisplayFormat = m.Foramt,
                    Zindex=m.Zindex
                });
            }
            if (existVisibleColumn == false)
            {
                result.Success = false;
                result.Message = "BizQuery.Mssg2";
                return result;
            }
            this.CurrentQuery.Columns = lstColumns.ToArray();
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 保存查询条件
        /// </summary>
        /// <param name="list">查询条件列表</param>
        /// <returns>保存结果</returns>
        private ActionResult SaveQueryItem(List<BizQueryConditionViewModel> list)
        {
            ActionResult result = new ActionResult();
            if (list == null || list.Count == 0)
            {
                QueryItemsTable = new List<BizQueryItem>();
                result.Success = true;
                return result;
            }
            List<BizQueryItem> bizQueryItems = new List<BizQueryItem>();
            foreach (var r in list)
            {
                string objectId = r.ObjectID;
                BizQueryItem item = QueryItemsTable.Find(queryItem => queryItem.ObjectID == objectId);
                if (item == null)
                {

                    item = new BizQueryItem();
                    item.PropertyName = r.ConditionColumn;
                    item.DisplayType = (ControlType)r.DisplayType;
                    item.FilterType = (FilterType)r.FilterType;
                    item.Visible = r.Visible ? BoolMatchValue.True : BoolMatchValue.False;
                    item.DefaultValue = r.DefaultValue;
                    item.SelectedValues = r.ConditionValue;
                    item.ObjectID = Guid.NewGuid().ToString();
                }
                else
                {
                    item.PropertyName = r.ConditionColumn;
                    item.DisplayType = (ControlType)r.DisplayType;
                    item.FilterType = (FilterType)r.FilterType;
                    item.Visible = r.Visible ? BoolMatchValue.True : BoolMatchValue.False;
                    item.DefaultValue = r.DefaultValue;
                    item.SelectedValues = r.ConditionValue;
                }
                bizQueryItems.Add(item);
                Data.DataLogicType propertyLogicType = this.Schema.GetProperty(item.PropertyName).LogicType;

                if (propertyLogicType == Data.DataLogicType.Bool)
                {
                    if (item.FilterType != FilterType.Equals)
                    {
                        result.Success = false;
                        result.Message = "BizQuery.BoolError";
                        result.Extend = r.ConditionValue;
                        return result;
                    }
                    int defaultInt;
                    if (!string.IsNullOrEmpty(item.DefaultValue)&&!int.TryParse(item.DefaultValue, out defaultInt))
                    {
                        result.Success = false;
                        result.Message = "BizQuery.BoolDefault";
                        result.Extend = r.ConditionValue;
                        return result;
                    }
                }
                if (item.FilterType == FilterType.Scope)
                { // 范围参数，只允许数字和日期类型
                    if (!OThinker.H3.Data.DataLogicTypeConvertor.IsDateTimeType(propertyLogicType) && !OThinker.H3.Data.DataLogicTypeConvertor.IsNumericType(propertyLogicType))
                    {
                        result.Success = false;
                        result.Message = "BizQuery.ScopeError";
                        result.Extend = r.ConditionValue;
                        return result;
                    }
                }
                else if (item.FilterType == FilterType.SystemParam)
                { // 系统参数，不允许数字和日期类型
                    if (OThinker.H3.Data.DataLogicTypeConvertor.IsDateTimeType(propertyLogicType) || OThinker.H3.Data.DataLogicTypeConvertor.IsNumericType(propertyLogicType))
                    {
                        result.Success = false;
                        result.Message = "BizQuery.SystemParamError";
                        result.Extend = r.ConditionValue;
                        return result;
                    }
                    if (string.IsNullOrEmpty(item.DefaultValue))
                    {
                        result.Success = false;
                        result.Message = "BizQuery.SystemParamEmpty";
                        result.Extend = r.ConditionValue;
                        return result;
                    }
                }
                else if (item.FilterType == FilterType.Contains)
                {// 日期类型不允许模糊匹配
                    if (OThinker.H3.Data.DataLogicTypeConvertor.IsDateTimeType(propertyLogicType))
                    {
                        result.Success = false;
                        result.Message = "BizQuery.ContainsError";
                        result.Extend = r.ConditionValue;
                        return result;
                    }
                }
                // 检测是否存在重复的属性
                List<string> queryProperties = new List<string>();
                foreach (var m in list)
                {
                    if (queryProperties.Contains(m.ConditionColumn))
                    {// 存在属性查询条件重复，那么给出提示
                        result.Success = false;
                        result.Message = "BizQuery.PropertyRepeat";
                        result.Extend = m.ConditionColumn;
                        return result;
                    }
                    queryProperties.Add(m.ConditionColumn);
                }
            }
            QueryItemsTable = bizQueryItems;
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 获取查询列表基础信息
        /// </summary>
        /// <returns>查询列表基础信息</returns>
        private BizQueryViewMode GetBizQueryViewMode(string schemaCode, string queryCode)
        {
            BizQueryViewMode model = new BizQueryViewMode()
            {
                SchemaCode = schemaCode,
                DisplayName = CurrentQuery.DisplayName,
                ListMethod = CurrentQuery.ListMethod,
                QueryCode = queryCode,
                OrginQueryCode = queryCode,
                QueryWhenLoad = CurrentQuery.ListDefault == OThinker.Data.BoolMatchValue.True
            };
            return model;
        }

        /// <summary>
        /// 获取表单保存操作列表
        /// </summary>
        /// <returns>表单保存操作列表</returns>
        private List<Item> GetSaveCompletes()
        {
            List<Item> list = new List<Item>();
            foreach (var s in Enum.GetNames(typeof(SheetAfterSaveAction)))
            {
                int value = (int)Enum.Parse(typeof(SheetAfterSaveAction), s);
                list.Add(new Item("BizQuery.SheetAfterSaveAction." + s, value.ToString()));
            };
            return list;
        }

        /// <summary>
        /// 获取功能表单列表
        /// </summary>
        /// <returns>功能表单列表</returns>
        private List<Item> GetActionSheets()
        {
            List<Item> list = new List<Item>();

            Sheet.BizSheet[] Sheets = this.Engine.BizSheetManager.GetBizSheetBySchemaCode(this.SchemaCode);
            if (Sheets != null)
            {
                foreach (Sheet.BizSheet sheet in Sheets)
                {
                    list.Add(new Item(sheet.DisplayName + "[" + sheet.SheetCode + "]", sheet.SheetCode));
                }
            }
            return list;
        }


        /// <summary>
        /// 获取方法列表
        /// </summary>
        /// <returns>方法列表</returns>
        private List<Item> GetActionMethods()
        {
            List<Item> list = new List<Item>();
            if (this.Schema.Methods != null)
            {
                foreach (MethodGroupSchema method in this.Schema.Methods)
                {
                    if (method.MethodName == DataModel.BizObjectSchema.MethodName_Create
                        || method.MethodName == DataModel.BizObjectSchema.MethodName_Update
                        || method.MethodName == DataModel.BizObjectSchema.MethodName_GetList
                        || method.MethodName == DataModel.BizObjectSchema.MethodName_Load)
                        continue;
                    list.Add(new Item(method.DisplayName + "[" + method.MethodName + "]", method.MethodName));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取功能类型
        /// </summary>
        /// <returns>功能类型</returns>
        private List<Item> GetActionTypes(int sheetCount)
        {
            List<Item> list = new List<Item>();
            foreach (var s in Enum.GetNames(typeof(ActionType)))
            {
                int value = (int)Enum.Parse(typeof(ActionType), s);
                if (s != ActionType.OpenBizSheet.ToString())
                    list.Add(new Item("BizQuery.ActionTypes." + s, value.ToString()));
                else if (sheetCount != 0)
                    list.Add(new Item("BizQuery.ActionTypes." + s, value.ToString()));

            };
            return list;
        }

        /// <summary>
        /// 获取功能列表
        /// </summary>
        /// <returns>功能列表</returns>
        private List<BizQueryActionViewModel> GetActions()
        {
            List<BizQueryActionViewModel> list = new List<BizQueryActionViewModel>();
            list = QueryActionsTable.Select(s => new BizQueryActionViewModel()
            {
                ObjectID = s.ObjectID,
                ActionCode = s.ActionCode,
                ActionMethod = s.BizMethodName,
                DisplayName = s.DisplayName,
                IsDefault = s.IsDefault == BoolMatchValue.True,
                ActionType = (int)s.ActionType,
                Confirm = s.Confirm == BoolMatchValue.True,
                SortKey = s.SortKey,
                WithID = s.WithID == BoolMatchValue.True,
                SaveCompleted = (int)s.AfterSave,
                Url = s.Url,
                ActionSheet = s.BizSheetCode,
                Visible = s.Visible == BoolMatchValue.True
            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取默认值列表
        /// </summary>
        /// <returns>默认值列表</returns>
        private List<Item> GetDefaultValues()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item("BizQuery.CurrentID", "{UserID}"));
            list.Add(new Item("BizQuery.CurrentAccount", "{UserAlias}"));
            list.Add(new Item("BizQuery.DepartID", "{Department}"));
            return list;
        }

        /// <summary>
        /// 获取显示类型列表
        /// </summary>
        /// <returns>显示类型列表</returns>
        private List<Item> GetDisplayTypes()
        {
            List<Item> list = new List<Item>();
            foreach (var s in Enum.GetNames(typeof(ControlType)))
            {
                int value = (int)Enum.Parse(typeof(ControlType), s);
                if (value != 4)
                    list.Add(new Item("BizQuery.DisplayTypes." + s, value.ToString()));
            };
            return list;
        }

        /// <summary>
        /// 获取匹配方式列表
        /// </summary>
        /// <returns></returns>
        private List<Item> GetFilterTypes()
        {
            List<Item> list = new List<Item>();
            foreach (var s in Enum.GetNames(typeof(FilterType)))
            {
                int value = (int)Enum.Parse(typeof(FilterType), s);
                list.Add(new Item("BizQuery." + s, value.ToString()));
            };
            return list;
        }

        /// <summary>
        /// 获取查询条件属性名称下拉框
        /// </summary>
        /// <returns>查询条件属性名称下拉框 </returns>
        private List<Item> GetConditionColumns()
        {
            List<Item> list = new List<Item>();
            ServiceMethodMap[] methodMaps = this.Schema.GetMethod("GetList").MethodMaps;
            if (methodMaps != null && methodMaps.Length > 0)
            {
                string[] paramNames = methodMaps[0].ParamMaps.GetReferencedPropertyNames();
                foreach (string propertyName in paramNames)
                {
                    PropertySchema property = this.Schema.GetProperty(propertyName);
                    if (property != null && !(property.LogicType == Data.DataLogicType.Html || property.LogicType == Data.DataLogicType.HyperLink))
                    {
                        if (OThinker.H3.Data.DataLogicTypeConvertor.IsSearchable(property.LogicType))
                        {
                            Item item = new Item(property.DisplayName, property.Name);
                            item.Extend = new { LogicType = property.LogicType + string.Empty };
                            list.Add(item);
                        }
                    }
                }
            }
            else
            {
                foreach (string propertyName in this.VisiblePropertyNames)
                {
                    PropertySchema property = this.Schema.GetProperty(propertyName);
                    if (property != null && !(property.LogicType == Data.DataLogicType.Html || property.LogicType == Data.DataLogicType.HyperLink))
                    {
                        if (OThinker.H3.Data.DataLogicTypeConvertor.IsSearchable(property.LogicType))
                        {
                            Item item = new Item(property.DisplayName, property.Name);
                            item.Extend = new { LogicType = property.LogicType + string.Empty };
                            list.Add(item);
                        }
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 获取查询条件列表
        /// </summary>
        /// <returns></returns>
        private List<BizQueryConditionViewModel> GetConditions()
        {
            List<BizQueryConditionViewModel> list = new List<BizQueryConditionViewModel>();

            list = this.QueryItemsTable.Select(s => new BizQueryConditionViewModel()
            {
                ObjectID = s.ObjectID,
                ConditionColumn = s.PropertyName,
                FilterType = (int)s.FilterType,
                DefaultValue = s.DefaultValue,
                Visible = s.Visible == BoolMatchValue.True,
                DisplayType = (int)s.DisplayType,
                ConditionValue = s.SelectedValues,
                DisplayTypes = GetDisplayTypes()

            }).ToList();
            return list;
        }

        /// <summary>
        /// 获取属性显示列表
        /// </summary>
        /// <returns>查询条件列表</returns>
        private List<BizQueryPropertyViewModel> GetPropertys()
        {
            //已存在的列
            Dictionary<string, BizQueryColumn> ExistedVisibleColumns = new Dictionary<string, BizQueryColumn>();
            if (CurrentQuery.Columns != null)
            {
                foreach (BizQueryColumn column in CurrentQuery.Columns)
                {
                    ExistedVisibleColumns.Add(column.PropertyName, column);
                }
            }

            List<BizQueryPropertyViewModel> list = new List<BizQueryPropertyViewModel>();
            BizQueryColumn bizQueryColumn = new BizQueryColumn();
            ServiceMethodMap[] methodMaps = this.Schema.GetMethod("GetList").MethodMaps;
            if (methodMaps != null && methodMaps.Length > 0)
            {
                DataMapCollection returnMaps = this.Schema.GetMethod("GetList").MethodMaps[0].ReturnMaps;
                string[] returnNames = returnMaps.GetParamNames();

                foreach (string propertyName in returnNames)
                {
                    string mapProperty = returnMaps.GetMap(propertyName).MapTo;
                    if (string.IsNullOrEmpty(mapProperty)) continue;
                    if (ExistedVisibleColumns.ContainsKey(mapProperty))
                    {
                        bizQueryColumn = ExistedVisibleColumns[mapProperty];
                        list.Add(new BizQueryPropertyViewModel()
                        {
                            Foramt = bizQueryColumn.DisplayFormat,
                            Name = bizQueryColumn.PropertyName,
                            NameValue = bizQueryColumn.PropertyName,
                            Sortable = bizQueryColumn.Sortable == BoolMatchValue.True,
                            Visible = bizQueryColumn.Visible == BoolMatchValue.True,
                            Width = bizQueryColumn.Width,
                            Zindex=bizQueryColumn.Zindex
                        });
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(mapProperty))
                            list.Add(new BizQueryPropertyViewModel()
                            {
                                Foramt = string.Empty,
                                Name = mapProperty,
                                NameValue = mapProperty,
                                Sortable = BoolMatchValue.False == BoolMatchValue.True,
                                Visible = BoolMatchValue.False == BoolMatchValue.True,
                                Width = bizQueryColumn.Width,
                                Zindex=bizQueryColumn.Zindex
                            });
                    }
                }
            }
            else if (this.Schema.Properties != null)
            {
                {
                    foreach (string propertyName in this.VisiblePropertyNames)
                    {
                        //var fullName = GetPropertyFullName(propertyName);
                        if (ExistedVisibleColumns.ContainsKey(propertyName))
                        {
                            bizQueryColumn = ExistedVisibleColumns[propertyName];
                            list.Add(new BizQueryPropertyViewModel()
                            {
                                Foramt = bizQueryColumn.DisplayFormat,
                                Name = bizQueryColumn.PropertyName,
                                NameValue = propertyName,
                                Sortable = bizQueryColumn.Sortable == BoolMatchValue.True,
                                Visible = bizQueryColumn.Visible == BoolMatchValue.True,
                                Width = bizQueryColumn.Width,
                                Zindex=bizQueryColumn.Zindex
                            });
                        }
                        else
                        {
                            list.Add(new BizQueryPropertyViewModel()
                            {
                                Foramt = string.Empty,
                                Name = propertyName,
                                NameValue = propertyName,
                                Sortable = BoolMatchValue.False == BoolMatchValue.True,
                                Visible = BoolMatchValue.False == BoolMatchValue.True,
                                Width = 0,
                                Zindex = bizQueryColumn.Zindex

                            });
                        }
                    }
                }
            }
            foreach (var s in list)
            {
                s.Name = GetPropertyFullName(s.Name);
            }
            return list;
        }
        /// <summary>
        /// 获取查询方法列表
        /// </summary>
        /// <returns>查询方法列表</returns>
        private List<Item> GetListMethods()
        {
            List<Item> list = new List<Item>();
            if (this.Schema.Methods != null)
            {
                foreach (MethodGroupSchema method in this.Schema.Methods)
                {
                    if (method.MethodType == MethodType.Filter)
                        list.Add(new Item(method.DisplayName + "[" + method.MethodName + "]", method.MethodName));
                }
            }
            return list;
        }

        /// <summary>
        /// 获取属性的全名称
        /// </summary>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        string GetPropertyFullName(string PropertyName)
        {
            if (this.Schema == null)
            {
                return string.Empty;
            }
            PropertySchema p = this.Schema.GetProperty(PropertyName);
            if (p != null)
            {
                return p.FullName;
            }
            return string.Empty;
        }

        #region 生成列表页面的ASPX文件 -------------------
        /// <summary>
        /// 生成列表页面的ASPX文件
        /// </summary>
        /// <param name="pageClassName"></param>
        /// <param name="editPageName"></param>
        /// <param name="schema"></param>
        public string GenerateListASPXPage(string pageClassName)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<%@ Page Language=\"C#\" AutoEventWireup=\"true\" CodeFile=\"" + pageClassName + ".aspx.cs\" Inherits=\"OThinker.H3.Portal." + pageClassName + "\" %>");
            sb.AppendLine("<!DOCTYPE html>");
            sb.AppendLine("<%@ Register Assembly=\"OThinker.H3.WorkSheet\" Namespace=\"OThinker.H3.WorkSheet\" TagPrefix=\"SheetControls\" %>");
            sb.AppendLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            sb.AppendLine("<head>");
            sb.AppendLine("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
            sb.AppendLine("<title></title>");
            sb.AppendLine("<link href=\"<%=this.PortalResRoot %>/_Content/themes/ligerUI/Aqua/css/ligerui-all.css\" rel=\"stylesheet\" type=\"text/css\" />");
            sb.AppendLine("<link href=\"<%=this.PortalResRoot %>/_Content/themes/H3Default/H3-All.css\" rel=\"stylesheet\" type=\"text/css\" />");
            sb.AppendLine("<link href=\"<%=this.PortalResRoot %>/_Content/themes/H3Default/BizQueryView.css\" rel=\"stylesheet\" type=\"text/css\" />");
            sb.AppendLine("<script src=\"<%=this.PortalResRoot %>/_Scripts/jquery/jquery.js\"></script>");
            sb.AppendLine("<script src=\"<%=this.PortalResRoot %>/_Scripts/ligerUI/ligerui.all.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"<%=this.PortalResRoot %>/_Scripts/ligerUI/plugins/ligerDrag.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"<%=this.PortalResRoot %>/_Scripts/H3AdminPlugins.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script src=\"<%=this.PortalResRoot %>/_Scripts/bizquery.js\" type=\"text/javascript\"></script>");
            sb.AppendLine("<script type=\"text/javascript\">");
            sb.AppendLine("var _PORTALROOT_GLOBAL = \"" + this.PortalRoot + "\";");
            sb.AppendLine("var CustomColumns = [];");
            sb.AppendLine("// 若要添加自定义的列,请参考以下示例");
            sb.AppendLine("// CustomColumns.push({");
            sb.AppendLine("//    display: \"自定义列标题1\",//列标题");
            sb.AppendLine("//    width: \"360px\",//宽度");
            sb.AppendLine("//    //显示逻辑");
            sb.AppendLine("//    render: function (rowData, rowIndex) {");
            sb.AppendLine("//        return \"<div>行号:\" + rowIndex + \"; 字段[ObjectID]值为:\" + rowData.ObjectID + \"</div>\";");
            sb.AppendLine("//    }");
            sb.AppendLine("// });");
            sb.AppendLine("</script>");
            sb.AppendLine("</head>");
            sb.AppendLine("<body>");
            sb.AppendLine("<form id=\"form1\" runat=\"server\">");

            sb.AppendLine("<div class=\"panel\">");
            sb.AppendLine("<div id=\"custem-toolbar\" class=\"panel-heading\">");
            sb.AppendLine("<div class=\"form-inline\" role=\"form\">");
            sb.AppendLine("<asp:LinkButton ID=\"btnSearch\" runat=\"server\" OnClick=\"btnSearch_Click\" CssClass=\"btn btn-default\"><i class=\"panel-title-icon fa fa-search\"></i><%=this.PortalResource.GetString(\"RunBizQuery_Query\")%></asp:LinkButton>");
            sb.AppendLine("<div class=\"btn-group btn-default\" id=\"query-btn-group\">");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"panel-body\" style=\"min-height: 300px\">");
            sb.AppendLine("<div class=\"row\">");
            sb.AppendLine("<asp:Panel ID=\"panelSearch\" runat=\"server\">");
            sb.AppendLine("<div class=\"H3Panel\">");
            sb.AppendLine("<asp:Table ID=\"tableSearch\" runat=\"server\">");
            sb.AppendLine("</asp:Table>");
            sb.AppendLine("</div>");
            sb.AppendLine("</asp:Panel>");
            sb.AppendLine("</div>");
            sb.AppendLine("<div class=\"row\">");
            sb.AppendLine("<div id=\"masterDataGrid\"></div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");
            sb.AppendLine("</div>");

            sb.AppendLine("</form>");
            sb.AppendLine("</body>");
            sb.AppendLine("</html>");
            return sb.ToString();
        }
        #endregion

        #region 生成列表页面的CS文件 ---------------------
        /// <summary>
        /// 生成列表页面的ASPX文件
        /// </summary>
        /// <param name="pageClassName"></param>
        public string GenerateListCSPage(string pageClassName, string queryCode)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("using System;");
            sb.AppendLine("using System.Web.UI.WebControls;");
            sb.AppendLine("using OThinker.H3.BizBus.Filter;");
            sb.AppendLine("namespace OThinker.H3.Portal");
            sb.AppendLine("{");
            sb.AppendLine("public partial class " + pageClassName + " : BizQueryPage");
            sb.AppendLine("{");
            sb.AppendLine("#region 页面控件");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 查询条件所在容器");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("protected override Panel SearchPanel");
            sb.AppendLine("{");
            sb.AppendLine("get");
            sb.AppendLine("{");
            sb.AppendLine("return this.panelSearch;");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 查询条件表格");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("protected override Table SearchTable");
            sb.AppendLine("{");
            sb.AppendLine("get");
            sb.AppendLine("{");
            sb.AppendLine("// 若不启用系统默认的查询条件,请取消注释以下行");
            sb.AppendLine("// return null;");
            sb.AppendLine("return this.tableSearch;");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("#endregion");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 获取或设置业务对象编码");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("public override string SchemaCode");
            sb.AppendLine("{");
            sb.AppendLine("get");
            sb.AppendLine("{");
            sb.AppendLine("return \"" + this.SchemaCode + "\";");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 获取或设置查询编码");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("public override string QueryCode");
            sb.AppendLine("{");
            sb.AppendLine("get");
            sb.AppendLine("{");
            sb.AppendLine("return \"" + queryCode + "\";");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 功能按钮是否显示");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("protected override bool ActionVisible");
            sb.AppendLine("{");
            sb.AppendLine("get");
            sb.AppendLine("{");
            sb.AppendLine("return true;");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 页面加载事件");
            sb.AppendLine("/// </summary> ");
            sb.AppendLine("/// <param name=\"sender\"></param>");
            sb.AppendLine("/// <param name=\"e\"></param>");
            sb.AppendLine("protected void Page_Load(object sender, EventArgs e)");
            sb.AppendLine("{");
            sb.AppendLine("base.Page_Load(sender, e);");
            sb.AppendLine("}");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 查询");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("/// <param name=\"sender\"></param>");
            sb.AppendLine("/// <param name=\"e\"></param>");
            sb.AppendLine("protected void btnSearch_Click(object sender, EventArgs e)");
            sb.AppendLine("{");
            sb.AppendLine("this.BindGridView();");
            sb.AppendLine("}");
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// 获取查询条件");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("/// <returns></returns>");
            sb.AppendLine("protected virtual Filter GetFilter()");
            sb.AppendLine("{");
            sb.AppendLine("// 如果使用自定义的查询条件，请取消注释以下行");
            sb.AppendLine("// Filter filter = new Filter();");
            sb.AppendLine("// 构建查询条件并返回");
            sb.AppendLine("// And and = new And();");
            sb.AppendLine("// filter.Matcher = and;");
            sb.AppendLine("// 示例: 添加查询条件,CreatedBy为当前用户");
            sb.AppendLine("// and.Add(new ItemMatcher(");
            sb.AppendLine("//    \"CreatedBy\",");
            sb.AppendLine("//    OThinker.Data.ComparisonOperatorType.Equal,");
            sb.AppendLine("//    this.UserValidator.UserID));");
            sb.AppendLine("// return filter;");
            sb.AppendLine("return base.GetFilter();");
            sb.AppendLine("}");
            sb.AppendLine("}");
            sb.AppendLine("}");
            return sb.ToString();
        }
        #endregion
    }
}
