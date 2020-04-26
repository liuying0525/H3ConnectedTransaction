using OThinker.H3.BizBus.Filter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers
{
    public class BizQueryHandlerController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public BizQueryHandlerController()
        {
        }

        #region
        private const string GetQuerySetting = "GetQuerySetting";
        private const string GetQueryData = "GetQueryData";
        private const string GetQuerySettingAndData = "GetQuerySettingAndData";

        private string _QueryCode;
        private string QueryCode
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_QueryCode))
                {
                    _QueryCode = Request["QueryCode"] ?? "";
                }
                return _QueryCode;
            }
        }
        private OThinker.H3.DataModel.BizObjectSchema _Schema = null;
        /// <summary>
        /// 获取当前对象的Schema
        /// </summary>
        private OThinker.H3.DataModel.BizObjectSchema Schema
        {
            get
            {
                if (_Schema == null)
                {
                    this._Schema = this.Engine.BizObjectManager.GetPublishedSchema(Request["SchemaCode"]);
                }
                return _Schema;
            }
        }
        private DataModel.BizQuery _BizQuery;
        private DataModel.BizQuery BizQuery
        {
            get
            {
                if (_BizQuery == null)
                {
                    _BizQuery = this.Engine.BizObjectManager.GetBizQuery(QueryCode);
                }
                return _BizQuery;
            }
        }
        private Dictionary<string, string> _InputMapping;
        private Dictionary<string, string> InputMapping
        {
            get
            {
                if (_InputMapping == null)
                {
                    string InputMappingStr = Request["InputMapping"] ?? "";
                    if (!string.IsNullOrWhiteSpace(InputMappingStr))
                    {
                        _InputMapping = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(InputMappingStr);
                    }
                }
                return _InputMapping;
            }
        }
        #endregion

        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult BizQueryHandler()
        {
            string Action = this.Request["Action"];
            object json = "";
            bool LoadFinished = false;
            Dictionary<string, string> columns = new Dictionary<string, string>();
            switch (Action)
            {
                case GetQuerySetting:
                    DataModel.BizQuery q = this.GetBizQuery();
                    json = new { QuerySetting = q };
                    break;
                case GetQueryData:
                    List<object> d = GetBizQueryData(out LoadFinished);

                    if (this.Schema != null)
                    {
                        foreach (DataModel.PropertySchema property in this.Schema.Properties)
                        {
                            columns.Add(property.Name, property.DisplayName);
                        }
                    }
                    json = new { QueryData = d, LoadFinished = LoadFinished, Columns = columns };
                    break;
                case GetQuerySettingAndData:
                    DataModel.BizQuery q1 = this.GetBizQuery();
                    List<object> d1 = GetBizQueryData(out LoadFinished);
                    if (this.Schema != null)
                    {
                        foreach (DataModel.PropertySchema property in this.Schema.Properties)
                        {
                            columns.Add(property.Name, property.DisplayName);
                        }
                    }

                    json = new
                    {
                        QuerySetting = q1,
                        QueryData = d1,
                        LoadFinished = LoadFinished,
                        Columns = columns
                    };
                    break;
            }
            return Json(json, JsonRequestBehavior.AllowGet);
        }

        private DataModel.BizQuery GetBizQuery()
        {
            return this.BizQuery;
        }

        private List<object> GetBizQueryData(out bool LoadFinished)
        {
            BizBus.Filter.Filter filter = GetFilter();
            LoadFinished = false;
            //分页
            int totalCount = 0;
            int pageSize = Convert.ToInt32(Request.Params["PageSize"]);
            int currentPageIndex = Convert.ToInt32(Request.Params["NextPageIndex"]);
            if (pageSize > 0)
            {
                filter.FromRowNum = currentPageIndex * pageSize < 1 ? 1 : currentPageIndex * pageSize + 1;
                filter.ToRowNum = filter.FromRowNum + pageSize - 1;
            }
            filter.RequireCount = true;
            // 调用查询
            DataModel.BizObject[] objs = this.Schema.GetList(
                this.Engine.Organization,
                this.Engine.MetadataRepository,
                this.Engine.BizObjectManager,
                this.UserValidator.UserID,
                "GetList",
                filter,
                out totalCount);

            if (totalCount <= filter.ToRowNum)
            {
                LoadFinished = true;
            }
            List<object> dataList = new List<object>();
            if (objs != null)
            {
                //业务对象数组类型的属性名称
                List<string> VisiblePropertyNames = new List<string>();
                if (this.Schema.Properties != null)
                {
                    foreach (DataModel.PropertySchema p in this.Schema.Properties)
                    {
                        if (p.TypeSearchable)
                        {
                            VisiblePropertyNames.Add(p.Name);
                        }
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
                        Organization.Unit[] Units = this.Engine.Organization.GetUnits(UnitIDList.ToArray()).ToArray();
                        // <UnitID,Unit>
                        Dictionary<string, OThinker.Organization.Unit> UnitDictionary = new Dictionary<string, Organization.Unit>();
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
                                        dicValueTable.Add(pName, UnitDictionary[obj.ValueTable[pName] + string.Empty].Name);
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
                                                multiParticipantString += UnitDictionary[participant].Name + ";";
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

        private BizBus.Filter.Filter GetFilter()
        {
            BizBus.Filter.Filter filter = new BizBus.Filter.Filter();
            And and = new And();
            filter.Matcher = and;

            if (this.BizQuery == null) return filter;
            //读取默认值和传入的映射
            Dictionary<string, object> items = new Dictionary<string, object>();
            Dictionary<string, DataModel.BizQueryItem> QueryItems = new Dictionary<string, DataModel.BizQueryItem>();
            if (this.BizQuery.QueryItems == null) this.BizQuery.QueryItems = new DataModel.BizQueryItem[]{};
                foreach (DataModel.BizQueryItem QueryItem in this.BizQuery.QueryItems)
                {
                    QueryItems.Add(QueryItem.PropertyName, QueryItem);
                    if (QueryItem.FilterType == OThinker.H3.DataModel.FilterType.SystemParam)
                    {
                        items.Add(QueryItem.PropertyName, SheetUtility.GetSystemParamValue(this.UserValidator, QueryItem.DefaultValue));
                    }
                    else
                    {
                        //设置默认值
                        if (!string.IsNullOrWhiteSpace(QueryItem.DefaultValue))
                        {
                            items.Add(QueryItem.PropertyName, SheetUtility.GetSystemParamValue(this.UserValidator, QueryItem.DefaultValue));
                        }
                        //设置映射传入的值
                        if (this.InputMapping != null)
                        {
                            if (this.InputMapping.ContainsKey(QueryItem.PropertyName))
                            {
                                if (items.ContainsKey(QueryItem.PropertyName))
                                {
                                    items[QueryItem.PropertyName] = this.InputMapping[QueryItem.PropertyName];
                                }
                                else
                                {
                                    items.Add(QueryItem.PropertyName, this.InputMapping[QueryItem.PropertyName]);
                                }
                            }
                        }
                    }
                }
            //页面传过来的值
            string FilterStr = Request["Filters"] ?? "";
            if (!string.IsNullOrWhiteSpace(FilterStr))
            {
                Dictionary<string, string> Filters = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(FilterStr);
                foreach (string key in Filters.Keys)
                {
                    if (!QueryItems.ContainsKey(key)) continue;
                    DataModel.BizQueryItem QueryItem = QueryItems[key];
                    if (QueryItem.FilterType == OThinker.H3.DataModel.FilterType.SystemParam) continue;
                    if (QueryItem.Visible == OThinker.Data.BoolMatchValue.False) continue;

                    if (items.ContainsKey(key))
                    {
                        items[key] = Filters[key];
                    }
                    else
                    {
                        items.Add(key, Filters[key]);
                    }
                }
            }
            //添加到过滤集合
            foreach (string key in items.Keys)
            {
                DataModel.BizQueryItem QueryItem = QueryItems[key];
                if (QueryItem.FilterType == OThinker.H3.DataModel.FilterType.Equals
                    || QueryItem.FilterType == OThinker.H3.DataModel.FilterType.SystemParam)
                {
                    and.Add(new ItemMatcher(key, OThinker.Data.ComparisonOperatorType.Equal, items[key]));
                }
                else if (QueryItem.FilterType == OThinker.H3.DataModel.FilterType.Contains)
                {
                    and.Add(new ItemMatcher(key, OThinker.Data.ComparisonOperatorType.Contain, items[key]));
                }
                else if (QueryItem.FilterType == OThinker.H3.DataModel.FilterType.Scope)
                {
                    string[] vals = items[key].ToString().Split(';');
                    if (vals.Length > 1)
                    {
                        and.Add(new ItemMatcher(key, OThinker.Data.ComparisonOperatorType.NotBelow, vals[0]));
                    }
                    if (vals.Length > 2)
                    {
                        and.Add(new ItemMatcher(key, OThinker.Data.ComparisonOperatorType.NotAbove, vals[1]));
                    }
                }
            }
            //设置排序
            filter.AddSortBy(new SortBy(this.BizQuery.Columns.FirstOrDefault().PropertyName, SortDirection.Ascending));
            return filter;
        }
    }
}
