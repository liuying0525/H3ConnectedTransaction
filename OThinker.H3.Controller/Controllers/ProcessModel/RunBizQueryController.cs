using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Acl;
using OThinker.H3.BizBus.Filter;
using OThinker.Organization;
using OThinker.H3.Controllers.ViewModels;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 查询列表运行方法
    /// </summary>
    [Authorize]
    public class RunBizQueryController : ControllerBase
    {
        /// <summary>
        /// 节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return ""; }
        }
        #region 属性名称 -------------
        /// <summary>
        /// 获取或设置业务对象编码
        /// </summary>
        public virtual string SchemaCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置查询编码
        /// </summary>
        public virtual string QueryCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置显示主数据的宿主控件ID
        /// </summary>
        public string CtrlID
        {
            get
            {
                return Request["CtrlID"];
            }
        }

        /// <summary>
        /// 获取或设置窗体的打开方式，默认是0：以window.open方式打开的，1：以iframe方式打开的
        /// </summary>
        protected string OpenType
        {
            get
            {
                string openType = HttpUtility.UrlDecode(Request.QueryString["OpenType"] + string.Empty);
                openType = string.IsNullOrEmpty(openType) ? "0" : openType;
                return openType;
            }
        }

        /// <summary>
        /// 是否为开窗查询模式
        /// </summary>
        protected bool IsPopup
        {
            get
            {
                return !string.IsNullOrEmpty(Request.QueryString["IsPopup"]);
            }
        }

        string _ListMethod = "GetList";
        /// <summary>
        /// 获取或设置查询编码
        /// </summary>
        public virtual string ListMethod
        {
            get
            {
                if (this.Query != null)
                {
                    return this._ListMethod;
                }
                return this._ListMethod;
            }
            set { _ListMethod = value; }
        }

        private DataModel.BizQuery _Query;
        /// <summary>
        /// 获取或设置业务对象的查询方法
        /// </summary>
        protected DataModel.BizQuery Query
        {
            get
            {
                if (_Query == null)
                {
                    _Query = this.Engine.BizObjectManager.GetBizQuery(QueryCode);
                    if (_Query == null)
                    {
                        throw new Exception(string.Format("主数据[{0}]不存在查询编码[{1}]。", SchemaCode, QueryCode));
                    }
                }
                return _Query;
            }
        }
        List<DataModel.BizQueryItem> _lstVisibleQueryItems;
        List<DataModel.BizQueryItem> lstVisibleQueryItems
        {
            get
            {
                if (_lstVisibleQueryItems == null)
                {
                    _lstVisibleQueryItems = new List<DataModel.BizQueryItem>();
                    if (Query != null && Query.QueryItems != null)
                        foreach (DataModel.BizQueryItem item in Query.QueryItems)
                        {
                            if (item.Visible == OThinker.Data.BoolMatchValue.True)
                                _lstVisibleQueryItems.Add(item);
                        }
                }
                return _lstVisibleQueryItems;
            }
        }

        OThinker.H3.DataModel.BizObjectSchema _Schema = null;
        /// <summary>
        /// 获取当前对象的Schema
        /// </summary>
        protected OThinker.H3.DataModel.BizObjectSchema Schema
        {
            get
            {
                if (_Schema == null)
                {
                    this._Schema = this.Engine.BizObjectManager.GetPublishedSchema(this.SchemaCode);
                }
                return _Schema;
            }
        }

        /// <summary>
        /// 是否显示功能按钮
        /// </summary>
        protected virtual bool ActionVisible
        {
            get
            {
                return true;
            }
        }

        private string _OutputPropertyMappings;
        // <summary>
        /// 获取或设置业务对象/属性的输出参数
        /// </summary>
        public string OutputPropertyMappings
        {
            get
            {
                if (string.IsNullOrEmpty(_OutputPropertyMappings))
                {
                    _OutputPropertyMappings = HttpUtility.UrlDecode(Request.QueryString["OutputParams"] + string.Empty);
                }
                return _OutputPropertyMappings;
            }
            set
            {
                _OutputPropertyMappings = value;
            }
        }

        private string _InputPropertyMappings;
        /// <summary>
        /// 获取或设置业务对象/属性的输入参数
        /// </summary>
        string InputPropertyMappings
        {
            get
            {
                if (_InputPropertyMappings == null)
                {
                    _InputPropertyMappings = Request["InputParam"] == null ? string.Empty : HttpUtility.UrlDecode(Request["InputParam"]);
                }
                return _InputPropertyMappings;
            }
        }

        /// <summary>
        /// 表头
        /// </summary>
        protected List<Dictionary<string, string>> GirdColumns = new List<Dictionary<string, string>>();

        private DataTable _TableSource;
        /// <summary>
        /// 数据源
        /// </summary>
        DataTable TableSource
        {
            get
            {
                if (_TableSource == null)
                {
                    _TableSource = new DataTable();
                }
                return _TableSource;
            }
            set
            {
                _TableSource = value;
            }
        }

        #endregion

        #region 页面控件

        ///// <summary>
        ///// 查询条件所在容器
        ///// </summary>
        //protected virtual Panel SearchPanel
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}

        ///// <summary>
        ///// 查询条件表格
        ///// </summary>
        //protected virtual Table SearchTable
        //{
        //    get
        //    {
        //        return null;
        //    }
        //}
        #endregion

        /// <summary>
        /// 查询列表数据查询
        /// </summary>
        /// <param name="schemaCode"></param>
        /// <param name="queryCode"></param>
        /// <returns></returns>
        public JsonResult GetBizQueryViewData(string schemaCode, string queryCode, string filterStr, PagerInfo pageInfo)
        {
            string st = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT CODE FROM ot_functionnode WHERE REPLACE(URL, ' ', '') LIKE '%SchemaCode:\"" + schemaCode.Replace("'", "''") + "\"%' AND REPLACE(URL, ' ', '') LIKE '%QueryCode:\"" + queryCode.Replace("'", "''") + "\"%'") + "";            if (!this.UserValidator.ValidateFunctionRun(st)) { return Json("无权限", JsonRequestBehavior.AllowGet); }
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                Dictionary<string, object> param = new Dictionary<string, object>();
                param["ActionFilter"] = GetRunBizQueryData(schemaCode, queryCode);
                param["DisplayFormats"] = GetDisplayFormats();
                result.Success = true;
                result.Extend = param;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        public JsonResult GetGridDataForPortal(string schemaCode, string queryCode, string filterStr, PagerInfo pageInfo)
        {
            string st = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT CODE FROM ot_functionnode WHERE REPLACE(URL, ' ', '') LIKE '%SchemaCode:\"" + schemaCode.Replace("'", "''") + "\"%' AND REPLACE(URL, ' ', '') LIKE '%QueryCode:\"" + queryCode.Replace("'", "''") + "\"%'") + "";            if (!this.UserValidator.ValidateFunctionRun(st)) { return Json("无权限", JsonRequestBehavior.AllowGet); }
            return ExecuteFunctionRun(() =>
            {
                this.SchemaCode = schemaCode;
                this.QueryCode = queryCode;
                int totalCount;
                if (pageInfo.iSortCol_0 != 0)
                {
                    CreateColumns();
                    var columns = GirdColumns;
                    foreach (var dic in columns[pageInfo.iSortCol_0 - 1])
                    {
                        pageInfo.sortname = dic.Value;
                    }
                    pageInfo.sortorder = pageInfo.sSortDir_0;
                } if (filterStr == null) { filterStr = "{}"; }

                if (pageInfo.iSortCol_0 !=0)
                {
                    CreateColumns();
                    var columns = GirdColumns;
                    foreach (var dic in columns[pageInfo.iSortCol_0 - 1])
                    {
                        pageInfo.sortname = dic.Value;
                    }
                    pageInfo.sortorder = pageInfo.sSortDir_0;
                }


                List<object> dataList = GetGridDataList(filterStr, pageInfo, out totalCount);
                Session[this.Request.Url.ToString()] = null;
                GridViewModel<object> result = new GridViewModel<object>(totalCount, dataList, pageInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取页面所需数据
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="queryCode">查询编码</param>
        /// <returns>页面数据</returns>
        [HttpGet]
        public JsonResult GetRunBizQueryData(string schemaCode, string queryCode)
        {
            this.SchemaCode = schemaCode;
            this.QueryCode = queryCode;
            ActionResult result = new ActionResult();
            Dictionary<string, object> param = new Dictionary<string, object>();
            CreateColumns();
            param["isFrame"] = this.OpenType == "1";
            param["isReturn"] = this.OutputPropertyMappings.Split(new string[] { " | " }, StringSplitOptions.RemoveEmptyEntries).Length > 0;
            param["outputPropertyMappings"] = this.OutputPropertyMappings;
            param["CtrlID"] = this.CtrlID;
            param["SchemaCode"] = this.SchemaCode;
            param["ActionVisible"] = this.ActionVisible;
            param["Param_BizObjectID"] = SheetEnviroment.Param_BizObjectID;
            param["Param_SchemaCode"] = SheetEnviroment.Param_SchemaCode;
            param["GirdColumns"] = GirdColumns;
            param["BizQueryActions"] = GetActions();
            param["DisplayFormats"] = GetDisplayFormats();
            param["FilterData"] = GetVisibleFilterData();
            param["ListDefault"] = Query.ListDefault;
            result.Success = true;
            result.Extend = param;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="queryCode">查询编码</param>
        /// <param name="filterStr">过滤条件</param>
        /// <param name="pageInfo">分页信息</param>
        /// <returns>列表数据</returns>
        [HttpPost]
        public JsonResult GetGridData(string schemaCode, string queryCode, string filterStr, PagerInfo pageInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                if (!string.IsNullOrEmpty(filterStr))
                {
                    filterStr = filterStr.Replace(@"%", @"[%]");
                }
                this.SchemaCode = schemaCode;
                this.QueryCode = queryCode;
                ActionResult result = new ActionResult();
                int totalCount;
                List<object> dataList = GetGridDataList(filterStr, pageInfo, out totalCount);
                Session[this.Request.Url.ToString()] = null;
                result.Extend = new { Rows = dataList.ToArray(), Total = totalCount };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取查询出的数据列以及总大小
        /// </summary>
        /// <param name="filterStr">过滤参数</param>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="totalCount">返回的总数</param>
        /// <returns>表格数据</returns>
        private List<object> GetGridDataList(string filterStr, PagerInfo pageInfo, out int totalCount)
        {
            OThinker.H3.BizBus.Filter.Filter filter = GetFilter(filterStr);
            totalCount = 0;
            int pageSize = pageInfo.PageSize;
            int currentPageIndex = pageInfo.PageIndex;

            //点击排序支持
            string SortName = Request.Params["sortname"] ;
            string SortOrder = Request.Params["sortorder"] ;
            if (!string.IsNullOrEmpty(pageInfo.sortname) && !string.IsNullOrEmpty(pageInfo.sortorder))
            {
                SortName = pageInfo.sortname;
                SortOrder = pageInfo.sortorder;
            }
            if (!string.IsNullOrEmpty(pageInfo.sortname)&&!string.IsNullOrEmpty(pageInfo.sortorder))
            {
                  SortName =pageInfo.sortname ;
                 SortOrder = pageInfo.sortorder ;
            }

            OThinker.H3.BizBus.Filter.SortDirection SortDirection = OThinker.H3.BizBus.Filter.SortDirection.Ascending;
            if (!string.IsNullOrEmpty(SortName))
            {
                if (!string.IsNullOrEmpty(SortOrder) && SortOrder.ToLower() == "desc")
                {
                    SortDirection = OThinker.H3.BizBus.Filter.SortDirection.Descending;
                }
            }
            //如果没有指定排序,默认取第一列
            else
            {
                for (int i = 0; i < Query.Columns.Length; i++)
                {
                    if (Query.Columns[i].Visible == OThinker.Data.BoolMatchValue.True)
                    {
                        filter.AddSortBy(new SortBy(Query.Columns[i].PropertyName, OThinker.H3.BizBus.Filter.SortDirection.Ascending));
                        break;
                    }
                }
            }
            filter.AddSortBy(new SortBy(SortName, SortDirection));

            if (filter.SortByCollection == null || filter.SortByCollection.Length == 0)
            {
                foreach (DataModel.BizQueryColumn column in Query.Columns)
                {
                    if (column.Visible == OThinker.Data.BoolMatchValue.True)
                    {
                        filter.AddSortBy(new SortBy(column.PropertyName, OThinker.H3.BizBus.Filter.SortDirection.Ascending));
                        break;
                    }
                }
            }

            if (pageSize > 0)
            {
                filter.FromRowNum = (currentPageIndex - 1) * pageSize < 1 ? 1 : (currentPageIndex - 1) * pageSize + 1;
                filter.ToRowNum = filter.FromRowNum + pageSize - 1;
            }
            filter.RequireCount = true;

            // 调用查询
            DataModel.BizObject[] objs = this.Schema.GetList(
                this.Engine.Organization,
                this.Engine.MetadataRepository,
                this.Engine.BizObjectManager,
                this.UserValidator.UserID,
                ListMethod,
                filter,
                out totalCount);
            List<object> dataList = new List<object>();
            if (objs != null)
            {
                //业务对象数组类型的属性名称
                List<string> VisiblePropertyNames = new List<string>();
                if (this.Schema.Properties != null)
                {
                    foreach (DataModel.PropertySchema p in this.Schema.Properties)
                    {
                        //if (p.TypeSearchable)
                        //{
                        VisiblePropertyNames.Add(p.Name);
                        //}
                    }
                }

                List<string> UnitIDList = new List<string>();
                // 业务对象
                foreach (DataModel.BizObject obj in objs)
                {
                    Dictionary<string, object> dicValueTable = new Dictionary<string, object>();
                    if (obj.ValueTable != null)
                    {
                        foreach (string pName in VisiblePropertyNames)
                        {
                            if (obj.ValueTable.ContainsKey(pName))
                            {
                                //单人参与者
                                if (this.Schema.GetProperty(pName).LogicType == Data.DataLogicType.SingleParticipant
                                    || pName == DataModel.BizObjectSchema.PropertyName_CreatedBy
                                    || pName == DataModel.BizObjectSchema.PropertyName_CreatedByParentId)
                                {
                                    UnitIDList.Add(obj.ValueTable[pName] + string.Empty);
                                }
                                //多人参与者
                                else if (this.Schema.GetProperty(pName).LogicType == Data.DataLogicType.MultiParticipant)
                                {
                                    string[] participants = (string[])obj.ValueTable[pName];
                                    if (participants != null && participants.Length > 0)
                                    {
                                        string multiParticipantString = string.Empty;
                                        foreach (string participant in participants)
                                        {
                                            UnitIDList.Add(participant);
                                        }
                                    }
                                }
                            }
                        }

                        Unit[] Units = this.Engine.Organization.GetUnits(UnitIDList.ToArray()).ToArray();
                        // <UnitID,Unit>
                        Dictionary<string, OThinker.Organization.Unit> UnitDictionary = new Dictionary<string, OThinker.Organization.Unit>();
                        if (Units != null && Units.Length > 0)
                        {
                            foreach (OThinker.Organization.Unit u in Units)
                            {
                                if (!UnitDictionary.ContainsKey(u.ObjectID))
                                {
                                    UnitDictionary.Add(u.ObjectID, u);
                                }
                            }
                        }

                        foreach (string pName in VisiblePropertyNames)
                        {
                            if (obj.ValueTable.ContainsKey(pName))
                            {
                                //单人参与者
                                if (this.Schema.GetProperty(pName).LogicType == Data.DataLogicType.SingleParticipant
                                    || pName == DataModel.BizObjectSchema.PropertyName_CreatedBy
                                    || pName == DataModel.BizObjectSchema.PropertyName_CreatedByParentId)
                                {
                                    if (UnitDictionary.ContainsKey(obj.ValueTable[pName] + string.Empty))
                                    {
                                        dicValueTable.Add(pName, GetUnitDisplayHtml(UnitDictionary[obj.ValueTable[pName] + string.Empty]));
                                    }
                                    else
                                    {
                                        dicValueTable.Add(pName, obj.ValueTable[pName] + string.Empty);
                                    }
                                }
                                //多人参与者
                                else if (this.Schema.GetProperty(pName).LogicType == Data.DataLogicType.MultiParticipant)
                                {
                                    string[] participants = (string[])obj.ValueTable[pName];
                                    if (participants != null && participants.Length > 0)
                                    {
                                        string multiParticipantString = string.Empty;
                                        foreach (string participant in participants)
                                        {
                                            if (UnitDictionary.ContainsKey(participant))
                                            {
                                                multiParticipantString += GetUnitDisplayHtml(UnitDictionary[participant]) + ";";
                                            }
                                            else
                                            {
                                                multiParticipantString += participant + ";";
                                            }
                                        }
                                        dicValueTable.Add(pName, multiParticipantString);
                                    }
                                    else
                                    {
                                        dicValueTable.Add(pName, obj.ValueTable[pName]);
                                    }
                                }
                                //时间段
                                else if (this.Schema.GetProperty(pName).LogicType == Data.DataLogicType.TimeSpan)
                                {
                                    if (null != obj.ValueTable[pName])
                                    {
                                        string timeSpanStr = string.Empty;
                                        TimeSpan timeSpan = (TimeSpan)obj.ValueTable[pName];
                                        timeSpanStr = timeSpan.ToString();
                                        dicValueTable.Add(pName, timeSpanStr);
                                    }
                                }
                                else
                                {
                                    dicValueTable.Add(pName, obj.ValueTable[pName]);
                                }
                            }
                        }
                    }
                    dataList.Add(dicValueTable);
                }
            }
            return dataList;
        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="bizSchemaCode">业务模型编码</param>
        /// <param name="bizObjectID">所选行ID</param>
        /// <param name="methodName">执行方法名称</param>
        /// <returns>执行结果</returns>
        [HttpPost]
        public JsonResult InvokeMethod(string bizSchemaCode, string bizObjectID, string methodName)
        {
            ActionResult result = new ActionResult();
            string BizSchemaCode = bizSchemaCode;
            string BizObjectID = bizObjectID;
            string MethodName = methodName;

            OThinker.H3.DataModel.BizObject BizObject = new OThinker.H3.DataModel.BizObject(
                this.Engine.Organization,
                this.Engine.MetadataRepository,
                this.Engine.BizObjectManager,
                this.Engine.BizBus,
                this.Engine.BizObjectManager.GetPublishedSchema(BizSchemaCode),
                this.UserValidator.UserID,
                this.UserValidator.User.ParentID);
            BizObject.ObjectID = BizObjectID;
            BizObject.Load();

            if (BizObject != null)
            {
                //校验权限
                if (//MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Update|| 
                    MethodName == OThinker.H3.DataModel.BizObjectSchema.MethodName_Remove)
                {

                    if (!this.UserValidator.ValidateBizObjectAdmin(BizSchemaCode, "", BizObject.OwnerId))
                    {
                        string[] Errors = new string[] { "无管理权限" };
                        result.Extend = new { Result = false, Errors = Errors };
                        return Json(result.Extend, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        BizObject.Invoke(MethodName);
                        result.Extend = new { Result = true, NeedRefresh = true };
                        return Json(result.Extend, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    BizObject.Invoke(MethodName);
                    result.Extend = new { Result = true };
                    return Json(result.Extend, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                result.Extend = new { Result = false };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            }
        }

        /// <summary>
        /// 编辑或添加流程数据
        /// </summary>
        /// <param name="BizObjectID">对象ID</param>
        /// <param name="SchemaCode">数据模型编码</param>
        /// <param name="SheetCode">表单编码</param>
        /// <param name="Mode">打开表单的模式</param>
        /// <param name="AfterSave">保存后的动作</param>
        /// <param name="IsEditInstanceData">是否修改数据项模式</param>
        /// <param name="isMobile">是否是手机访问</param>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public void EditBizObject(string BizObjectID, string SchemaCode, string SheetCode, string Mode, string AfterSave, string IsEditInstanceData, string isMobile)
        {
            ExecuteFunctionRun(() =>
            {

                string url = string.Empty;
                bool mobile = !string.IsNullOrEmpty(isMobile);
                if (string.IsNullOrEmpty(isMobile))
                {
                    mobile = false;
                }
                else
                {
                    bool.TryParse(isMobile, out mobile);
                }
                SheetMode sheetmode = (SheetMode)Enum.Parse(typeof(SheetMode), Mode);
                if (!string.IsNullOrEmpty(IsEditInstanceData))
                {
                    url = GetWorkSheetUrl(
                            SchemaCode,
                            BizObjectID,
                            sheetmode,
                            mobile
                            );
                    url += "&EditInstanceData=true";
                }
                else
                {
                    OThinker.H3.Sheet.BizSheet sheet = this.Engine.BizSheetManager.GetBizSheetByCode(SheetCode);
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
                           sheetmode,
                           mobile
                           );
                }
                Response.Redirect(url);
                return null;
            });
        }



        /// <summary>
        /// 检查用户是否具有操作权限
        /// </summary>
        /// <returns></returns>
        public virtual OThinker.Data.BoolMatchValue ValidateAuthorization(string BizObjectID, string SchemaCode, SheetMode SheetMode, string WorkflowCode)
        {
            //验证权限
            var instanceId = "";
            Instance.InstanceData InstanceData = null;
            Instance.InstanceContext[] context = this.Engine.InstanceManager.GetInstanceContextsByBizObject(SchemaCode, BizObjectID);
            if (context != null && context.Length > 0)
            {
                instanceId = context[0].InstanceId;
                InstanceData = new Instance.InstanceData(this.Engine, instanceId, this.UserValidator.UserID);
            }

            if (SheetMode == SheetMode.Originate)
            {
                if (UserValidator.ValidateBizObjectAdd(SchemaCode, null, UserValidator.UserID))
                    return OThinker.Data.BoolMatchValue.True;
            }
            else
            {
                if (UserValidator.ValidateOrgAdmin(InstanceData.BizObject.OwnerId) || UserValidator.ValidateBizObjectAdmin(SchemaCode, null, InstanceData.BizObject.OwnerId))
                    return OThinker.Data.BoolMatchValue.True;
            }

            //if (SheetUtility.ValidateAuthorization(this.UserValidator,
            //    SheetDataType.BizObject,
            //    true,//this.ActionContext.IsOriginateMode,
            //    SchemaCode,//this.ActionContext.SchemaCode,
            //    null,//this.ActionContext.BizObject,
            //    SheetMode,//this.ActionContext.SheetMode,
            //    WorkflowCode,//this.ActionContext.WorkflowCode,
            //    null,//this.ActionContext.WorkItem,
            //    null,//this.ActionContext.CirculateItem,
            //    null//this.ActionContext.InstanceContext
            //    ))
            //{
            //    return OThinker.Data.BoolMatchValue.True;
            //}

            // 验证关联关系流程权限
            //if (IsRelationInstance()) return OThinker.Data.BoolMatchValue.True;
            return OThinker.Data.BoolMatchValue.False;
        }



        /// <summary>
        /// 应用中心-流程列表（前端）
        /// </summary>
        /// <param name="BizObjectID"></param>
        /// <param name="SchemaCode"></param>
        /// <param name="SheetCode"></param>
        /// <param name="Mode"></param>
        /// <param name="IsMobile"></param>
        /// <param name="EditInstanceData"></param>
        /// <returns></returns>
        public JsonResult EditBizObjectSheet(string BizObjectID, string SchemaCode, string SheetCode, string Mode, bool IsMobile, string EditInstanceData)
        {
            ActionResult result = new ActionResult(false, "");

            string url = string.Empty;
            SheetMode SheetMode = SheetMode.Work;

            SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), Mode);

            if (this.ValidateAuthorization(BizObjectID, SchemaCode, SheetMode, SchemaCode) == OThinker.Data.BoolMatchValue.False)
            {
                result.Success = false;
                //result.Message = "MvcController_Perission";
                result.Message = Configs.Global.ResourceManager.GetString("MvcController_Perission");
                return Json(result, JsonRequestBehavior.AllowGet);
            }
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
        }


        /// <summary>
        /// 获取过滤条件数据
        /// </summary>
        /// <returns>过滤条件数据</returns>
        public List<FilterDataViewModel> GetVisibleFilterData()
        {
            List<FilterDataViewModel> list = new List<FilterDataViewModel>();
            if (null != lstVisibleQueryItems)
            {
                list = lstVisibleQueryItems.Select(s => new FilterDataViewModel
                {
                    DefaultValue = s.DefaultValue ?? "",
                    DisplayType = (int)(s.FilterType != OThinker.H3.DataModel.FilterType.Scope ? s.DisplayType : OThinker.H3.DataModel.ControlType.TextBox),//过滤类型为范围查询,强制变更为文本控件
                    FilterType = (int)s.FilterType,
                    LogicType = GetPropertyLogicTypeByName(s.PropertyName),
                    ParentIndex = s.ParentIndex,
                    PropertyCode = s.PropertyName,
                    PropertyName = GetPropertyDisplayNameByName(s.PropertyName),
                    PropertyType = (int)s.PropertyType,
                    SelectedValues = GetFilterSelectedValues(s.SelectedValues, s.DisplayType.ToString(), GetPropertyLogicTypeByName(s.PropertyName)),
                    Visible = (int)s.Visible
                }).ToList();
            }
            return list;
        }

        /// <summary>
        /// 获取下拉列表的值
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <returns></returns>
        private List<Item> GetFilterSelectedValues(string selectedValue, string displayType, string logicType)
        {
            List<Item> list = new List<Item>();
            Data.EnumerableMetadata[] enumDatas = null;
            if (!string.IsNullOrEmpty(selectedValue)) enumDatas = this.Engine.MetadataRepository.GetByCategory(selectedValue);

            if (displayType == OThinker.H3.DataModel.ControlType.DropdownList.ToString())
            {
                list.Add(new Item("全部", ""));
            }
            if (enumDatas != null && enumDatas.Length != 0)
            {
                list.AddRange(enumDatas.Select(s => new Item()
                {
                    Value = s.Code,
                    Text = s.EnumValue
                }));
            }
            else
            {
                if (!string.IsNullOrEmpty(selectedValue))
                {
                    TestServiceViewModel model = null;
                    OThinker.H3.BizBus.BizService.MethodSchema method = null;
                    try
                    {
                        model = JsonConvert.DeserializeObject<TestServiceViewModel>(selectedValue);
                        if (model != null)
                        {
                            method = this.Engine.BizBus.GetMethod(model.ServiceCode, model.RunMethod);
                        }
                    }
                    catch { }
                    if (model != null && method != null)
                    {
                        //查找方法的返回类型，
                        OThinker.H3.BizBus.BizService.BizService Service = Engine.BizBus.GetBizService(model.ServiceCode);
                        OThinker.H3.BizBus.BizService.BizServiceMethod bMethod = Service.GetMethod(model.RunMethod);
                        //删除与组织结构对应的系统权限
                        OThinker.H3.BizBus.BizService.BizStructure param = null;
                        if (method.ParamSchema != null)
                        {
                            param = H3.BizBus.BizService.BizStructureUtility.Create(method.ParamSchema);
                        }
                        //调用方法
                        OThinker.H3.BizBus.BizService.BizStructure ret = null;
                        try
                        {
                            ret = this.Engine.BizBus.Invoke(
                                 new OThinker.H3.BizBus.BizService.BizServiceInvokingContext(
                                     this.UserValidator.UserID,
                                     model.ServiceCode,
                                     method.ServiceVersion,
                                     method.MethodName,
                                     param));
                        }
                        catch (Exception ex)
                        {
                        }
                        //返回结果
                        if (ret != null && ret.Schema != null)
                        {
                            foreach (OThinker.H3.BizBus.BizService.ItemSchema item in ret.Schema.Items)
                            {
                                OThinker.H3.Data.DataLogicType type = item.LogicType;
                                //转换成相应的类型
                                object obj = ret[item.Name];
                                if (type == Data.DataLogicType.BizStructureArray)
                                {
                                    System.Data.DataTable table = null;
                                    OThinker.H3.BizBus.BizService.BizStructure[] bizStructs = (OThinker.H3.BizBus.BizService.BizStructure[])obj;

                                    foreach (OThinker.H3.BizBus.BizService.BizStructure st in bizStructs)
                                    {
                                        if (st != null)
                                        {
                                            table = OThinker.H3.BizBus.BizService.BizStructureUtility.ToTable(
                                                st.Schema,
                                                bizStructs);
                                            break;
                                        }
                                    }
                                    if (table != null && table.Rows.Count > 0)
                                    {
                                        foreach (DataRow row in table.Rows)
                                        {
                                            try
                                            {
                                                string Value = "";
                                                string Text = "";
                                                if (!string.IsNullOrEmpty(model.Text)
                                                    && !string.IsNullOrEmpty(model.Value)
                                                    && row.Table.Columns.Contains(model.Text)
                                                    && row.Table.Columns.Contains(model.Value))
                                                {
                                                    Value = row[model.Value] + string.Empty;
                                                    Text = row[model.Text] + string.Empty;
                                                }
                                                else if (row.ItemArray.Length >= 2)
                                                {
                                                    Value = row[0] + string.Empty;
                                                    Text = row[1] + string.Empty;
                                                }
                                                else if (row.ItemArray.Length == 1)
                                                {
                                                    Value = row[0] + string.Empty;
                                                    Text = row[0] + string.Empty;
                                                }
                                                list.Add(new Item() { Value = Value, Text = Text });
                                            }
                                            catch { }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        bool isBool = false;
                        if (logicType == Data.DataLogicType.Bool.ToString()) isBool = true;
                        string[] selectedValueArr = selectedValue.TrimEnd(';').Split(';');
                        list.AddRange(selectedValueArr.Select(s => new Item()
                        {
                            Value = s,
                            Text = isBool ? (s == "1" ? "True" : "False") : s
                        }));
                    }
                }
            }
            return list;
        }
        /// <summary>
        /// 根据名称获取业务对象的显示名称
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetPropertyDisplayNameByName(string propertyName)
        {
            var propertySchema = GetPropertySchemaByName(propertyName);
            if (null == propertySchema)
                return string.Empty;
            else return propertySchema.DisplayName.ToString();
        }

        /// <summary>
        /// 根据名称获取业务对象的逻辑类型
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public string GetPropertyLogicTypeByName(string propertyName)
        {
            var propertySchema = GetPropertySchemaByName(propertyName);
            if (null == propertySchema)
                return string.Empty;
            else return propertySchema.LogicType.ToString();
        }
        /// <summary>
        /// 根据名称获取业务对象属性
        /// </summary>
        /// <param name="properties"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        DataModel.PropertySchema GetPropertySchemaByName(string propertyName)
        {
            if (this.Schema != null)
                return this.Schema.GetProperty(propertyName);
            return null;
        }
        /// <summary>
        /// 获取显示格式
        /// </summary>
        /// <returns>显示格式</returns>
        private Dictionary<string, string> GetDisplayFormats()
        {
            if (this.Query != null && this.Query.Columns != null)
            {
                //ColumnName, DisplayFormat
                Dictionary<string, string> dicDisplayFormats = new Dictionary<string, string>();
                foreach (DataModel.BizQueryColumn column in this.Query.Columns)
                {
                    if (column.Visible == OThinker.Data.BoolMatchValue.True && !string.IsNullOrEmpty(column.DisplayFormat))
                    {
                        dicDisplayFormats.Add(column.PropertyName, column.DisplayFormat);
                    }
                }
                return dicDisplayFormats;

            }
            else return null;
        }

        /// <summary>
        /// 获取组织显示的HTML
        /// </summary>
        /// <param name="UnitID"></param>
        /// <returns></returns>
        private string GetUnitDisplayHtml(OThinker.Organization.Unit Unit)
        {
            if (Unit != null)
            {
                string url = null;
                switch (Unit.UnitType)
                {
                    //case OThinker.Organization.UnitType.Company:
                    //    url = System.IO.Path.Combine(GetPortalRoot(Page), "Admin/Organization/EditCompany.aspx?Mode=View&UnitID=") + Unit.ObjectID;
                    //    break;
                    case OThinker.Organization.UnitType.Group:
                        url = System.IO.Path.Combine(this.PortalRoot, "Admin/TabMaster.html?url=Organization/EditGroup.html&Mode=View&ID=") + Unit.ObjectID;
                        break;
                    case OThinker.Organization.UnitType.OrganizationUnit:
                        url = System.IO.Path.Combine(this.PortalRoot, "Admin/TabMaster.html?url=Organization/EditOrgUnit.html&Mode=View&ID=") + Unit.ObjectID;
                        break;
                    case OThinker.Organization.UnitType.Post:
                        url = System.IO.Path.Combine(this.PortalRoot, "Admin/TabMaster.html?url=Organization/EditPost.html&Mode=View&ID=") + Unit.ObjectID;
                        break;
                    //case OThinker.Organization.UnitType.Segment:
                    //    url = System.IO.Path.Combine(GetPortalRoot(Page), "Admin/Organization/EditSegement.aspx?Mode=View&ID=") + Unit.ObjectID;
                    //    break;
                    case OThinker.Organization.UnitType.User:
                        url = System.IO.Path.Combine(this.PortalRoot, "Admin/TabMaster.html?url=Organization/EditUser.html&Mode=View&ID=") + Unit.ObjectID;
                        break;
                    //case OThinker.Organization.UnitType.Staff:
                    case OThinker.Organization.UnitType.Unspecified:
                    default:
                        break;
                }

                if (!string.IsNullOrEmpty(url))
                {
                    return "<a href='" + url + "' target='_blank'>" + Unit.Name + "</a>";
                }
                else
                {
                    return Unit.Name;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        protected OThinker.H3.BizBus.Filter.Filter GetFilter(string filterStr)
        {
            // 构造查询条件
            OThinker.H3.BizBus.Filter.Filter filter = new OThinker.H3.BizBus.Filter.Filter();
            string paramName, paramValue;
            And and = new And();
            //Or or = new Or();
            filter.Matcher = and;
            // 获得需要的列
            List<string> columns = new List<string>();
            DataModel.PropertySchema[] properties = this.Schema.Properties;
            foreach (DataModel.PropertySchema property in properties)
            {
                if (property.TypeSearchable || property.LogicType == Data.DataLogicType.MultiParticipant || property.LogicType == Data.DataLogicType.TimeSpan)
                {
                    columns.Add(property.Name);
                }
            }
            filter.ReturnItems = columns.ToArray();

            if (Query.Columns == null)
            {
                throw new Exception(string.Format("没有给主数据：{0}的查询：{1}添加可以显示的字段", this.SchemaCode, this.QueryCode));
            }
            List<string> filterItems = new List<string>();
            if (lstVisibleQueryItems != null && Query.QueryItems != null)
            {
                string[] paramValues = InputPropertyMappings.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (DataModel.BizQueryItem queryItem in Query.QueryItems)
                {
                    // 增加系统参数条件
                    if (queryItem.FilterType == DataModel.FilterType.SystemParam
                        //确认属性存在
                        && this.Schema.GetProperty(queryItem.PropertyName) != null)
                    {
                        and.Add(new ItemMatcher(queryItem.PropertyName,
                             OThinker.Data.ComparisonOperatorType.Equal,
                             SheetUtility.GetSystemParamValue(this.UserValidator, queryItem.DefaultValue)));
                    }
                    else if (queryItem.Visible == OThinker.Data.BoolMatchValue.False)
                    {// 隐藏的查询条件
                        foreach (string v in paramValues)
                        {
                            if (v.IndexOf(",") > -1)
                            {
                                string key = v.Substring(0, v.IndexOf(","));
                                string value = v.Substring(v.IndexOf(",") + 1);
                                if (key == queryItem.PropertyName)
                                {
                                    filterItems.Add(queryItem.PropertyName);
                                    and.Add(new ItemMatcher(queryItem.PropertyName,
                                        OThinker.Data.ComparisonOperatorType.Equal, value));
                                }
                            }
                        }
                    }
                }
            }
            Dictionary<string, object> filterDic = null;
            if (string.IsNullOrEmpty(filterStr))
            {
                filterDic = new Dictionary<string, object>();
            }
            else
            {
                filterDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(filterStr);
            }
            List<FilterDataViewModel> filterList = GetVisibleFilterData();
            foreach (var filterItem in filterList)
            {
                paramName = paramValue = string.Empty;
                DataModel.BizQueryItem query = GetQueryItemByName(lstVisibleQueryItems.ToArray(), filterItem.PropertyCode);
                paramName = filterItem.PropertyCode;

                if (filterDic.ContainsKey(filterItem.PropertyCode))
                    paramValue = filterDic[filterItem.PropertyCode].ToString().TrimEnd(';');
                if (string.IsNullOrEmpty(paramValue)) continue;
                if (filterItem.FilterType == 2)//范围查询
                {
                    Dictionary<string, string> child = JsonConvert.DeserializeObject<Dictionary<string, string>>(paramValue);
                    string fromValue = child.ContainsKey("start") ? child["start"] : string.Empty;
                    string toValue = child.ContainsKey("start") ? child["end"] : string.Empty;
                    if (child.Count != 0)
                    {
                        if (filterItem.LogicType == Data.DataLogicType.DateTime.ToString())//日期类型
                        {
                            DateTime t = DateTime.Now;
                            DateTime.TryParse(fromValue, out t);
                            if (t.Year == 1 || t.Year == 1753 || t.Year == 9999) fromValue = string.Empty;
                            if (DateTime.TryParse(toValue, out t))
                            {
                                toValue = t.AddDays(1).AddSeconds(-1).ToString();
                            };
                            if (t.Year == 1 || t.Year == 1753 || t.Year == 9999) toValue = string.Empty;
                        }
                        // 做范围查询
                        if (fromValue != string.Empty)
                        {
                            and.Add(new ItemMatcher(paramName,
                               OThinker.Data.ComparisonOperatorType.NotBelow,
                               fromValue));
                        }
                        if (toValue != string.Empty)
                        {
                            and.Add(new ItemMatcher(paramName,
                              OThinker.Data.ComparisonOperatorType.NotAbove,
                              toValue));
                        }
                    }

                }
                else
                {
                    // 模糊匹配，并且为空值
                    if (paramValue == string.Empty && query.FilterType == DataModel.FilterType.Contains)
                        continue;
                    if (query != null && query.FilterType == DataModel.FilterType.Contains)
                    {// 模糊匹配
                        and.Add(new ItemMatcher(paramName,
                         OThinker.Data.ComparisonOperatorType.Contain,
                         paramValue));
                    }
                    else
                    {//完全匹配
                        and.Add(new ItemMatcher(paramName,
                         OThinker.Data.ComparisonOperatorType.Equal,
                         paramValue));
                    }
                }
            }
            //if (Query.QueryItems != null)
            //{
            //    //添加隐藏条件
            //    foreach (DataModel.BizQueryItem item in Query.QueryItems)
            //    {
            //        if (item.Visible == OThinker.Data.BoolMatchValue.False)
            //        {
            //            if (item.FilterType != DataModel.FilterType.SystemParam
            //                && !filterItems.Contains(item.PropertyName))
            //            {
            //                and.Add(new ItemMatcher(item.PropertyName,
            //                     item.FilterType == DataModel.FilterType.Contains ? OThinker.Data.ComparisonOperatorType.Contain
            //                        : OThinker.Data.ComparisonOperatorType.Equal,
            //                     item.DefaultValue));
            //            }
            //        }
            //    }
            //}
            return filter;
        }

        /// <summary>
        /// 根据名称获取查询条件
        /// </summary>
        /// <param name="queryItems"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        DataModel.BizQueryItem GetQueryItemByName(DataModel.BizQueryItem[] queryItems, string name)
        {
            foreach (DataModel.BizQueryItem queryItem in queryItems)
            {
                if (queryItem.PropertyName == name) return queryItem;
            }
            return null;
        }

        /// <summary>
        /// 获取查询动作
        /// </summary>
        /// <returns>查询动作</returns>
        private DataModel.BizQueryAction[] GetActions()
        {

            if (this.ActionVisible)
            {
                DataModel.BizQueryAction[] BizQueryActions = this.Query.BizActions;
                if (BizQueryActions != null && BizQueryActions.Length > 0)
                {
                    foreach (DataModel.BizQueryAction action in BizQueryActions)
                    {
                        if (string.IsNullOrEmpty(action.Icon))
                        {
                            if (action.ActionCode == "AddNew")
                            {
                                action.Icon = "fa fa-plus";
                            }
                            if (action.ActionCode == "Edit")
                            {
                                action.Icon = "fa fa-edit";
                            }
                            if (action.ActionCode == "Delete")
                            {
                                action.Icon = "fa fa-minus";
                            }
                        }
                        if (action.ActionType == DataModel.ActionType.OpenUrl
                            && !string.IsNullOrEmpty(action.Url)
                            && !action.Url.ToLower().StartsWith("http:")
                            && !action.Url.ToLower().StartsWith("https:"))
                        {
                            if (action.Url.ToLower().StartsWith("www."))
                            {
                                action.Url = "http://" + action.Url;
                            }
                            else
                            {
                                action.Url = GetUriPath(action.Url);
                            }
                        }

                    }
                }
                return BizQueryActions;
            }
            else return null; ;
        }

        /// <summary>
        /// 获取解析后的URI路径，对于存在 Portal
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string GetUriPath(string url)
        {
            if (this.PortalRoot == string.Empty)
            {
                return url;
            }
            if (string.IsNullOrEmpty(url))
            {
                return null;
            }
            return System.IO.Path.Combine(this.PortalRoot, url).Replace("\\", "/");
        }

        #region 获取表头 -------------------------

        /// <summary>
        /// 获取表头
        /// </summary>
        private void CreateColumns()
        {
            Dictionary<string, string> dicCloumns = null;
            List<string> lstDisplayColumns = new List<string>();
            if (this.Query.Columns != null)
            {
                foreach (DataModel.BizQueryColumn column in this.Query.Columns)
                {
                    if (column.Visible == OThinker.Data.BoolMatchValue.True)
                    {
                        DataModel.PropertySchema p = this.Schema.GetProperty(column.PropertyName);
                        if (p == null) continue;
                        dicCloumns = new Dictionary<string, string>();
                        dicCloumns.Add("display", p == null ? p.Name : p.DisplayName);
                        dicCloumns.Add("name", p.Name);
                        if (column.Width > 0)
                        {
                            dicCloumns.Add("width", column.Width.ToString());
                        }
                        else if (column.Width < 0)
                        {
                            dicCloumns.Add("hide", "true");
                        }
                        GirdColumns.Add(dicCloumns);
                    }
                }
            }
        }

        #endregion
    }
}
