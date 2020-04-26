using OThinker.H3.Acl;
using OThinker.Reporting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using OThinker.Data.Database.Serialization;
using OThinker.H3.Data;

namespace OThinker.H3.Controllers.Reporting
{
    [SessionState(System.Web.SessionState.SessionStateBehavior.ReadOnly)]
    public class ReportingController : ReportBase
    {
        //站点的根目录
        public string PortalRoot = System.Configuration.ConfigurationManager.AppSettings["PortalRoot"];

        private string DetailRowNumber = "DetailRowNumber";
        private int MaxRowMutiplyColumn = 40000;//默认最大的统计报表行数为40000行，这样做的目的是为了防止.0 响应时长过长的问题
        /// <summary>
        /// 授权条件
        /// </summary>
        private OThinker.H3.Acl.SystemAcl DataAclScope = new OThinker.H3.Acl.SystemAcl();



        /// <summary>
        /// 导出报表准备
        /// </summary>
        /// <returns></returns>
        // [UserAuthorizeFilter]
        public JsonResult PreExportTable()
        {
            string WidgetID = this.Request["WidgetID"];
            string Code = this.Request["Code"];
            string FilterDataJson = this.Request["FilterDataJson"];
            if (FilterDataJson == null)
                FilterDataJson = "";
            //if (!this.Engine.ReportManager.CanExport(Code, WidgetID))
            //{
            //    return Json(new { Result = false, Msg = "不能导出" }, JsonRequestBehavior.AllowGet);
            //}
            string ExportGuid = System.Guid.NewGuid().ToString().Replace("-", "");
            string KeyFilter = this.UserValidator.User.ObjectID + "_" + ExportGuid + "_" + "ExportReport" + "_" + "Filter";
            System.Web.Caching.Cache myCache = HttpRuntime.Cache;//构建Web服务的缓存
            string orderby = this.Request["orderby"];
            string dir = this.Request["dir"];
            myCache.Add(KeyFilter, FilterDataJson, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(2, 0, 0), System.Web.Caching.CacheItemPriority.High, null);
            return Json(new { Result = true, url = PortalRoot + "/Reporting/ExportTable?WidgetID=" + WidgetID + "&Code=" + Code + "&ExportGuid=" + ExportGuid + "&orderby=" + orderby + "&dir=" + dir });

        }
        /// <summary>
        /// 导出报表
        /// </summary>
        /// <returns></returns>
        public JsonResult ExportTable()
        {
            string WidgetID = this.Request["WidgetID"];
            string Code = this.Request["Code"];
            string ExportGuid = this.Request["ExportGuid"];
            System.Web.Caching.Cache myCache = HttpRuntime.Cache;
            string KeyFilter = this.UserValidator.User.ObjectID + "_" + ExportGuid + "_" + "ExportReport" + "_" + "Filter";
            string FilterDataJson = (string)myCache.Get(KeyFilter);
            myCache.Remove(KeyFilter);
            ReportPage reportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(Code));
            if (reportPage == null)
            {
                return Json(new { State = false, Msg = "当前用户的报表页对象未获取到，请重新登陆在尝试导出" }, JsonRequestBehavior.AllowGet);
            }
            ReportWidget Setting = reportPage.ReportWidgets.FirstOrDefault(p => p.ObjectID == WidgetID);
            ReportSource ReportSource = null;
            if (reportPage != null && reportPage.ReportSources != null && reportPage.ReportSources.Length > 0)
            {
                foreach (ReportSource q in reportPage.ReportSources)
                {
                    if (q.ObjectID == Setting.ReportSourceId)
                        ReportSource = q;
                }
            }
            Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
            List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage.Filters, Setting, ReportSource, out ChildCodes);
            Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();

            //重新检查列
            List<string> Msgs = new List<string>();
            if (!this.ReCheckColumns(Setting, SourceColumns, out Msgs))
            {
                return Json(new { State = false, Msg = string.Join(";", Msgs) }, JsonRequestBehavior.AllowGet);
            }

            //处理过滤条件
            if (!string.IsNullOrEmpty(FilterDataJson))
            {
                SetFilterValue(reportPage, FilterDataJson);
            }
            List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
            List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
            if (Setting.WidgetType == WidgetType.Combined)
            {
                List<string> Conditions = new List<string>();
                List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
                string tablename = Setting.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportPage.Code, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);

                List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
                tablename,
                SourceColumns.ToArray(),
                Setting.SortColumns,
                reportPage.Filters,
                out statisticalColumn,
                out UnitTableAndAssociationTable,
                out TypeChangedColumns,
                Conditions,
                Parameters,
                ReportSource);
                CombineReport MyCombineReport = new CombineReport();
                List<List<string>> ValueTable = new List<List<string>>();//这里要先横后竖，所以要先循环行表头；
                Dictionary<string, List<MyTd>> MyColumnTread = new Dictionary<string, List<MyTd>>();
                Dictionary<string, List<MyTd>> MyColumnTreadOnlyHeader = new Dictionary<string, List<MyTd>>();
                List<SimpleHeaderNode> RowSimpleHeaderNodes = new List<SimpleHeaderNode>();
                List<Dictionary<string, string>> ColumnRoad = new List<Dictionary<string, string>>();
                List<Dictionary<string, string>> RowRoad = new List<Dictionary<string, string>>();
                Dictionary<string, ReportWidgetColumn> allReportWidgetColumn = new Dictionary<string, ReportWidgetColumn>();
                string errorMessage = "";
                this.GetCombineTable(true, SourceData, Setting, out ColumnRoad, out RowRoad, UnitTableAndAssociationTable, out ValueTable, out MyColumnTread, out RowSimpleHeaderNodes, out MyCombineReport, out MyColumnTreadOnlyHeader, out allReportWidgetColumn, out errorMessage);
                CombineExportToExcel(ValueTable, MyColumnTread, RowSimpleHeaderNodes, MyCombineReport, Setting.DisplayName, allReportWidgetColumn, UnitTableAndAssociationTable, Setting.Columns);

            }
            else if (Setting.WidgetType == WidgetType.Detail)
            {
                string orderby = this.Request["orderby"];
                bool Ascending = !string.IsNullOrEmpty(this.Request["dir"]) && this.Request["dir"] != "asc" ? false : true;
                if (!string.IsNullOrEmpty(orderby))
                {
                    ReportWidgetColumn OrderbyColumn = new ReportWidgetColumn();
                    OrderbyColumn.ColumnCode = orderby;
                    OrderbyColumn.Ascending = Ascending;
                    Setting.SortColumns = new ReportWidgetColumn[1] { OrderbyColumn };
                }
                List<string> Conditions = new List<string>();
                int startnum = OThinker.Data.Database.Database.UnspecificRowNum;
                int endnum = OThinker.Data.Database.Database.UnspecificRowNum;
                List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
                string tablename = Setting.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportPage.Code, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);

                List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(
                    this.Engine,
                tablename,
                SourceColumns.ToArray(),
                Setting.SortColumns,
                reportPage.Filters,
                out statisticalColumn,
                out UnitTableAndAssociationTable,
                out TypeChangedColumns,
                Conditions,
                Parameters,
                ReportSource,
                OThinker.Data.Database.Database.UnspecificRowNum,
                OThinker.Data.Database.Database.UnspecificRowNum,
                false, WidgetType.Detail);
                HashSet<string> Columns = new HashSet<string>();
                List<ReportWidgetColumn> ResultSourceColumns = new List<ReportWidgetColumn>();
                foreach (ReportWidgetColumn q in SourceColumns)
                {
                    if (!Columns.Contains(q.ColumnCode))
                    {
                        Columns.Add(q.ColumnCode);
                    }
                }
                foreach (ReportWidgetColumn q in Setting.Columns)
                {
                    if (Columns.Contains(q.ColumnCode))
                        ResultSourceColumns.Add(q);
                }
                Dictionary<string, Dictionary<string, object>> dicSourceData = new Dictionary<string, Dictionary<string, object>>();
                string MainObjectIdCode = this.GetMainObjectId_Name(ReportSource.SchemaCode);
                //string MainObjectIdCode = this.GetMainObjectId(ReportSource.SchemaCode);
                if (ChildCodes != null && ChildCodes.Count > 0)
                {
                    //处理主表和子表的关系;
                    foreach (Dictionary<string, object> q in SourceData)
                    {
                        string ObjectId = q[MainObjectIdCode].ToString();
                        if (!dicSourceData.ContainsKey(ObjectId))
                        {
                            dicSourceData.Add(ObjectId, new Dictionary<string, object>());
                        }
                        foreach (string ColumnCode in ChildCodes.Keys)
                        {
                            ColumnSummary ColumnSummary = ChildCodes[ColumnCode];
                            if (!dicSourceData[ObjectId].ContainsKey(ColumnCode) && q.ContainsKey(ColumnCode))
                            {
                                dicSourceData[ObjectId].Add(ColumnCode, q[ColumnCode]);
                            }
                            if (ColumnSummary.ChildeColumnSummary != null && ColumnSummary.ChildeColumnSummary.Count > 0)
                            {
                                foreach (string ChildColumnCode in ColumnSummary.ChildeColumnSummary.Keys)
                                {
                                    ColumnSummary ChildColumnSummary = ColumnSummary.ChildeColumnSummary[ChildColumnCode];
                                    if (dicSourceData[ObjectId].ContainsKey(ChildColumnCode))
                                    {
                                        List<string> ChildResult = (List<string>)dicSourceData[ObjectId][ChildColumnCode];
                                        ChildResult.Add(q[ChildColumnCode].ToString());
                                        dicSourceData[ObjectId][ChildColumnCode] = ChildResult;
                                    }
                                    else
                                    {
                                        dicSourceData[ObjectId].Add(ChildColumnCode, new List<string>());
                                        List<string> ChildResult = new List<string>();
                                        ChildResult.Add(q[ChildColumnCode].ToString());
                                        dicSourceData[ObjectId][ChildColumnCode] = ChildResult;
                                    }
                                }
                            }
                        }
                    }
                    SourceData = dicSourceData.Values.ToList<Dictionary<string, object>>();
                }
                DetailExportToExcel(SourceData, ResultSourceColumns, Setting.DisplayName, ChildCodes);
            }
            return Json(new { }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 加载报表配置 Error:这个不需要这里加载的，再打开界面View的时候，就可以一次性传给前端
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadReportPage()
        {
            string ReportCode = this.Request["Code"];
            Dictionary<string, string> boolDic = new Dictionary<string, string>();
            ReportPage Setting = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(ReportCode));
            if (Setting == null)
            {
                return Json(new { State = false, Message = "报表配置不存在!" });
            }
            Setting.Filters = this.Engine.Interactor.GetReportFilters(ReportCode, Setting.ObjectID, out boolDic);
            if (Setting.Filters != null)
            {
                for (int i = 0; i < Setting.Filters.Length; i++)
                {
                    //如果是数据字典类型
                    if (Setting.Filters[i].FilterType == OThinker.H3.Reporting.FilterType.MasterData)
                    {
                        if (Setting.Filters[i].FilterValue != "")
                        {
                            EnumerableMetadata[] masterdatas = this.Engine.MetadataRepository.GetByCategory(Setting.Filters[i].FilterValue);
                            string master = "";
                            for (int j = 0; j < masterdatas.Length; j++)
                            {


                                if (j == masterdatas.Length - 1)
                                {
                                    master = master + masterdatas[j].Code + "::" + masterdatas[j].EnumValue;
                                }
                                else
                                {
                                    master = master + masterdatas[j].Code + "::" + masterdatas[j].EnumValue + ";;";
                                }

                            }
                            Setting.Filters[i].FilterValue = master;
                        }
                    }
                }
            }

            FunctionNode node = this.UserValidator.GetFunctionNode(ReportCode);
            string displayName = node == null ? "" : node.DisplayName;
            UserInformation UserInformation = new UserInformation();
            UserInformation.UserID = this.UserValidator.UserID;
            UserInformation.UserCode = this.UserValidator.User.Code;
            UserInformation.UserDisplayName = this.UserValidator.User.Name;
            Organization.Unit Company = Engine.Organization.GetUnit(this.UserValidator.CompanyId);
            UserInformation.UserCompanyID = this.UserValidator.CompanyId;
            UserInformation.UserCompanyCode = Company.UnitID;
            UserInformation.UserCompanyDisplayName = Company.Name;
            UserInformation.UserParentOUCode = Engine.Organization.GetUnit(this.UserValidator.User.ParentID).UnitID;
            UserInformation.UserParentOUID = this.UserValidator.User.ParentID;
            UserInformation.UserParentOUDisplayName = this.UserValidator.DepartmentName;
            return Json(new { State = true, ReportPage = Setting, CurrentUser = UserInformation, boolDic = boolDic, DisplayName = displayName });
        }
        /// <summary>
        /// 加载汇总表，图表
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadChartsData()
        {
            string widgetJson = this.Request["Widget"];
            string reportPageJson = this.Request["ReportPage"];
            string cha = "\";\"";//剔除选人控件初始值为空时，查询条件应该默认为空
            string FilterDataJson = null;
            string DataJson = this.Request["FilterData"];
            if (DataJson != null)
            {
                FilterDataJson = DataJson.Replace(cha, "");
            }
            string ObjectID = this.Request["ObjectID"];
            string Code = this.Request["Code"];
            string UnitFilterDataJson = this.Request["UnitFilterDataJson"];
            string reportSourceJson = this.Request["ReportSource"];
            ReportWidget Setting = null;
            ReportPage reportPage = null;
            ReportSource ReportSource = null;
            string msg = this.GetReportWidgetReportPageReportSource(widgetJson, reportPageJson, ObjectID, Code, reportSourceJson, out Setting, out reportPage, out ReportSource);
            if (!string.IsNullOrEmpty(msg))
            {
                return Json(new { State = false, Msg = msg });
            }
            if (Setting.WidgetType == WidgetType.Detail)
            {
                return Json(new { State = false, Msg = "参数错误" });
            }
            if ((Setting.Series == null || Setting.Series.Length == 0) && (Setting.Categories == null || Setting.Categories.Length == 0))
            {
                return Json(new { State = false });
            }
            Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
            //加设计时参数；
            bool IsDesign = string.IsNullOrEmpty(this.Request["isDesign"]) ? false : bool.Parse(this.Request["isDesign"]);
            int startnum = OThinker.Data.Database.Database.UnspecificRowNum;
            int endnum = OThinker.Data.Database.Database.UnspecificRowNum;
            Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
            List<string> reCheckColumnsMsgs = new List<string>();
            List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
            List<string> Conditions = new List<string>();
            if (IsDesign)
            {
                startnum = 0;
                endnum = 5;
            }
            //排除系列和分类里的defautcode
            this.RemoveDefautcodeFromSC(Setting);

            List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage.Filters, Setting, ReportSource, out ChildCodes);
            //联动
            if (!string.IsNullOrEmpty(UnitFilterDataJson))
            {
                this.DoUnit(UnitFilterDataJson, SourceColumns, reportPage, Setting, ReportSource);
            }
            //end联动
            //重新检查列
            if (!this.ReCheckColumns(Setting, SourceColumns, out reCheckColumnsMsgs))
            {
                return Json(new { State = false, Msg = string.Join(";", reCheckColumnsMsgs) });
            }
            //处理过滤条件
            if (!string.IsNullOrEmpty(FilterDataJson))
            {
                SetFilterValue(reportPage, FilterDataJson);
            }



            List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
            string tablename = Setting.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportPage.Code, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
            if (Setting.SortColumns != null)
            {
                //ReportWidgetColumn[] rep = null;
                if (Setting.SortColumns.Length > 0)
                {
                    foreach (var set in Setting.SortColumns)
                    {
                        foreach (var pro in SourceColumns.ToArray())
                        {
                            if (set.ColumnName.IndexOf(pro.ColumnName) != -1)
                            {
                                set.ColumnName = tablename.Trim() + "." + pro.ColumnName.Trim();
                            }
                        }
                    }
                }
            }
            List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();

            List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
                tablename,
                SourceColumns.ToArray(),
                Setting.SortColumns,
                reportPage.Filters,
                out statisticalColumn,
                out UnitTableAndAssociationTable,
                out TypeChangedColumns,
                Conditions,
                Parameters,
                ReportSource,
                startnum,
                endnum
                );
            //检查columntype变动
            if (TypeChangedColumns != null && TypeChangedColumns.Count > 0)
            {
                foreach (ReportWidgetColumn q in TypeChangedColumns)
                {
                    for (int i = 0; Setting.Columns != null && i < Setting.Columns.Length; i++)
                    {
                        if (Setting.Columns[i].ColumnCode == q.ColumnCode)
                        {
                            Setting.Columns[i].ColumnType = q.ColumnType;
                        }
                    }
                    for (int i = 0; Setting.Categories != null && i < Setting.Categories.Length; i++)
                    {
                        if (Setting.Categories[i].ColumnCode == q.ColumnCode)
                        {
                            Setting.Categories[i].ColumnType = q.ColumnType;
                        }
                    }
                    for (int i = 0; Setting.Series != null && i < Setting.Series.Length; i++)
                    {
                        if (Setting.Series[i].ColumnCode == q.ColumnCode)
                        {
                            Setting.Series[i].ColumnType = q.ColumnType;
                        }
                    }
                }
            }
            if (Setting.WidgetType == WidgetType.Combined)
            {
                #region 汇总表
                CombineReport MyCombineReport = new CombineReport();
                List<List<string>> ValueTable = new List<List<string>>();//这里要先横后竖，所以要先循环行表头；
                Dictionary<string, List<MyTd>> MyColumnTread = new Dictionary<string, List<MyTd>>();
                Dictionary<string, List<MyTd>> MyColumnTreadOnlyHeader = new Dictionary<string, List<MyTd>>();
                List<List<MyTd>> RowListTable = new List<List<MyTd>>();
                List<SimpleHeaderNode> RowSimpleHeaderNodes = new List<SimpleHeaderNode>();
                List<Dictionary<string, string>> ColumnRoad = new List<Dictionary<string, string>>();//列路径
                List<Dictionary<string, string>> RowRoad = new List<Dictionary<string, string>>();//行路径
                Dictionary<string, ReportWidgetColumn> allReportWidgetColumn = new Dictionary<string, ReportWidgetColumn>();
                string errorMessage = "";
                this.GetCombineTable(false, SourceData, Setting, out ColumnRoad, out RowRoad, UnitTableAndAssociationTable, out ValueTable, out MyColumnTread, out RowSimpleHeaderNodes, out MyCombineReport, out MyColumnTreadOnlyHeader, out allReportWidgetColumn, out errorMessage);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return Json(new { State = "false", errorMessage = errorMessage });
                }
                Dictionary<string, string>[] ArrayColumnRoad = ColumnRoad.ToArray();
                Dictionary<string, string>[] ArrayRowRoad = RowRoad.ToArray();
                //行表头table转换成list<list<>>;
                foreach (SimpleHeaderNode Node in RowSimpleHeaderNodes)
                {
                    List<MyTd> NewMyRowTd = new List<MyTd>();
                    RowListTable.Add(NewMyRowTd);
                    GetRowListTable(RowListTable, Node, NewMyRowTd, UnitTableAndAssociationTable, allReportWidgetColumn);
                }
                //END行表头table转换成list<list<>>;
                return Json(new { State = true, ValueTable = ValueTable, ColumnTable = MyColumnTread, RowTable = RowListTable, OnlyHeader = MyCombineReport.OnlyHeader, ColumnRoad = ArrayColumnRoad, RowRoad = ArrayRowRoad, OnlyHeaderTable = MyColumnTreadOnlyHeader });
                #endregion
            }
            else if (Setting.WidgetType == WidgetType.Bar || Setting.WidgetType == WidgetType.Line || Setting.WidgetType == WidgetType.Area || Setting.WidgetType == WidgetType.Pie || Setting.WidgetType == WidgetType.Radar || Setting.WidgetType == WidgetType.Funnel)
            {
                #region 图表

                #region 准备数据;
                ChartsDataResult Result = new ChartsDataResult();
                Dictionary<string, Dictionary<string, object>> AllBos = new Dictionary<string, Dictionary<string, object>>();
                Dictionary<string, HashSet<string>> SeriesBoIDs = new Dictionary<string, HashSet<string>>();
                Dictionary<string, HashSet<string>> CategorisBoIDs = new Dictionary<string, HashSet<string>>();
                foreach (Dictionary<string, object> Bo in SourceData)
                {
                    string BoObjectID = System.Guid.NewGuid().ToString();
                    while (true)
                    {
                        if (!AllBos.ContainsKey(BoObjectID))
                        {
                            AllBos.Add(BoObjectID, Bo);
                            break;
                        }
                        else
                        {
                            BoObjectID = System.Guid.NewGuid().ToString();
                        }
                    }
                    //系列
                    if (Setting.Series != null && Setting.Series.Length > 0)
                    {
                        ReportWidgetColumn Serie = Setting.Series[0];

                        string SeriesValue = Bo[Serie.ColumnCode] + string.Empty; 
                        if (!string.IsNullOrWhiteSpace(SeriesValue))
                        {
                            //年月日处理
                            if (Serie.FunctionType == Function.Year)
                            {
                                SeriesValue = DateTime.Parse(SeriesValue).ToString("yyyy");
                            }
                            else if (Serie.FunctionType == Function.YearAndMonth)
                            {
                                SeriesValue = DateTime.Parse(SeriesValue).ToString("yyyy-MM");// + " " + this.GetQuarter(DateTime.Parse(SeriesValue));
                            }
                            else if (Serie.FunctionType == Function.YearAndMonthAndDay)
                            {
                                SeriesValue = DateTime.Parse(SeriesValue).ToString("yyyy-MM-dd");// + " " + this.GetQuarter(DateTime.Parse(SeriesValue));
                            }
                            else if (Serie.FunctionType == Function.YearAndQuarter)
                            {
                                SeriesValue = DateTime.Parse(SeriesValue).ToString("yyyy") + " " + this.GetQuarter(DateTime.Parse(SeriesValue));
                            }
                            //年月日处理
                        }
                        if (SeriesBoIDs.ContainsKey(SeriesValue))
                        {
                            SeriesBoIDs[SeriesValue].Add(BoObjectID);
                        }
                        else
                        {
                            SeriesBoIDs.Add(SeriesValue, new HashSet<string>());
                            SeriesBoIDs[SeriesValue].Add(BoObjectID);
                        }
                    }
                    //end系列
                    //分类
                    if (Setting.Categories != null && Setting.Categories.Length > 0)
                    {
                        ReportWidgetColumn Categoty = Setting.Categories[0];
                        string CategorisValue = Bo[Categoty.ColumnCode] + string.Empty; 
                        if (!string.IsNullOrWhiteSpace(CategorisValue))
                        {
                            //年月日处理
                            if (Categoty.FunctionType == Function.Year)
                            {
                                CategorisValue = DateTime.Parse(CategorisValue).ToString("yyyy");
                            }
                            else if (Categoty.FunctionType == Function.YearAndMonth)
                            {
                                CategorisValue = DateTime.Parse(CategorisValue).ToString("yyyy-MM");// + " " + this.GetQuarter(DateTime.Parse(CategorisValue));
                            }
                            else if (Categoty.FunctionType == Function.YearAndMonthAndDay)
                            {
                                CategorisValue = DateTime.Parse(CategorisValue).ToString("yyyy-MM-dd");// + " " + this.GetQuarter(DateTime.Parse(CategorisValue));
                            }
                            else if (Categoty.FunctionType == Function.YearAndQuarter)
                            {
                                CategorisValue = DateTime.Parse(CategorisValue).ToString("yyyy") + " " + this.GetQuarter(DateTime.Parse(CategorisValue));
                            }
                            //年月日处理
                        }
                        if (CategorisBoIDs.ContainsKey(CategorisValue))
                        {
                            CategorisBoIDs[CategorisValue].Add(BoObjectID);
                        }
                        else
                        {
                            CategorisBoIDs.Add(CategorisValue, new HashSet<string>());
                            CategorisBoIDs[CategorisValue].Add(BoObjectID);
                        }
                    }
                    //end分类
                }
                //end准备数据
                #endregion
                //一个值，一个分类，一个系列
                if ((Setting.Series != null && Setting.Series.Length > 0) && (Setting.Categories != null && Setting.Categories.Length > 0) && (Setting.Columns != null && Setting.Columns.Length == 1))
                {
                    Result.CategoryCode = Setting.Categories[0].ColumnCode;
                    Result.CategoryDisplayName = Setting.Categories[0].DisplayName;
                    Result.CategoryType = Setting.Categories[0].ColumnType;
                    Result.SerieCode = Setting.Series[0].ColumnCode;
                    Result.SerieDisplayName = Setting.Series[0].DisplayName;
                    Result.SerieType = Setting.Series[0].ColumnType;
                    int Counter = 0;
                    foreach (string SeriesValue in SeriesBoIDs.Keys)
                    {
                        HashSet<string> SeriesBoIDsItem = SeriesBoIDs[SeriesValue];
                        SeriesForCharts SeriesItem = new SeriesForCharts();
                        SeriesItem.Code = SeriesValue;
                        SeriesItem.Name = SeriesItem.Code;
                        if ((Result.SerieType == ColumnType.Association || Result.SerieType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(SeriesItem.Code))
                        {
                            SeriesItem.Name = UnitTableAndAssociationTable[SeriesItem.Code];
                        }
                        Dictionary<CategoriesForCharts, double> sortDic = new Dictionary<CategoriesForCharts, double>();
                        foreach (string CategorisValue in CategorisBoIDs.Keys)
                        {
                            HashSet<string> CategorisBoIDsItem = CategorisBoIDs[CategorisValue];
                            double result = ChartsGetComputedValue(SeriesBoIDsItem, CategorisBoIDsItem, AllBos, Setting.Columns[0]);
                            //保留所设置的小数位数
                            if (!string.IsNullOrEmpty(Setting.Columns[0].Format) && Setting.Columns[0].Format.Contains("."))
                            {
                                string[] TempStrs = Setting.Columns[0].Format.Split('.');
                                result = System.Math.Round(result, TempStrs[1].Length);
                                SeriesItem.Precision = TempStrs[1].Length;
                            }
                            SeriesItem.Data.Add(result);
                            if (Counter == 0)
                            {
                                if ((Result.CategoryType == ColumnType.Association || Result.CategoryType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(CategorisValue))
                                {
                                    CategoriesForCharts MyCategories = new CategoriesForCharts() { key = CategorisValue, value = UnitTableAndAssociationTable[CategorisValue] };
                                    Result.Categories.Add(MyCategories);
                                    //这里记录一个排序，在系列与，分类中的data有一个对应关系；
                                    if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && SeriesBoIDs.Keys.Count == 1)
                                        sortDic.Add(MyCategories, result);
                                    //记录end
                                }
                                else
                                {
                                    CategoriesForCharts MyCategories = new CategoriesForCharts() { key = CategorisValue, value = CategorisValue };
                                    Result.Categories.Add(MyCategories);
                                    //这里记录一个排序，在系列与，分类中的data有一个对应关系；
                                    if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && SeriesBoIDs.Keys.Count == 1)
                                        sortDic.Add(MyCategories, result);
                                    //记录end
                                }
                            }
                        }
                        if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && SeriesBoIDs.Keys.Count == 1)
                        {
                            if (Setting.SortColumns[0].Ascending)
                            {
                                sortDic = sortDic.OrderBy(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                            }
                            else
                            {
                                sortDic = sortDic.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                            }
                            Result.Categories = sortDic.Keys.ToList();
                            SeriesItem.Data = sortDic.Values.ToList();
                        }
                        Counter++;
                        Result.Series.Add(SeriesItem);
                    }
                    return Json(new { State = true, MyChartsDataResult = Result });
                }
                ///有分类，并且值为1个或多个
                else if ((Setting.Categories != null && Setting.Categories.Length > 0) && (Setting.Columns != null && Setting.Columns.Length > 0))
                {
                    int Counter = 0;
                    Result.CategoryCode = Setting.Categories[0].ColumnCode;
                    Result.CategoryDisplayName = Setting.Categories[0].DisplayName;
                    Result.CategoryType = Setting.Categories[0].ColumnType;
                    Result.SerieCode = null;
                    Result.SerieDisplayName = null;

                    foreach (ReportWidgetColumn Column in Setting.Columns)
                    {
                        SeriesForCharts SeriesItem = new SeriesForCharts();
                        SeriesItem.Code = Column.DisplayName;
                        SeriesItem.Name = SeriesItem.Code;
                        if ((Result.SerieType == ColumnType.Association || Result.SerieType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(SeriesItem.Code))
                        {
                            SeriesItem.Name = UnitTableAndAssociationTable[SeriesItem.Code];
                        }
                        Dictionary<CategoriesForCharts, double> sortDic = new Dictionary<CategoriesForCharts, double>();
                        foreach (string CategorisValue in CategorisBoIDs.Keys)
                        {
                            HashSet<string> CategorisBoIDsItem = CategorisBoIDs[CategorisValue];
                            double result = ChartsGetComputedValue(null, CategorisBoIDsItem, AllBos, Column);
                            //保留所设置的小数位数
                            if (!string.IsNullOrEmpty(Column.Format)&&Column.Format.Contains("."))
                            {
                                string[] TempStrs=Column.Format.Split('.');
                                result = System.Math.Round(result, TempStrs[1].Length);
                                SeriesItem.Precision = TempStrs[1].Length;
                            }


                            SeriesItem.Data.Add(result);
                            if (Counter == 0)
                            {
                                if ((Result.CategoryType == ColumnType.Association || Result.CategoryType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(CategorisValue))
                                {
                                    CategoriesForCharts MyCategories = new CategoriesForCharts() { key = CategorisValue, value = UnitTableAndAssociationTable[CategorisValue] };
                                    Result.Categories.Add(MyCategories);
                                    //这里记录一个排序，在系列与，分类中的data有一个对应关系；
                                    if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && Setting.Columns.Length == 1)
                                        sortDic.Add(MyCategories, result);
                                    //记录end
                                }
                                else
                                {
                                    CategoriesForCharts MyCategories = new CategoriesForCharts() { key = CategorisValue, value = CategorisValue };
                                    Result.Categories.Add(MyCategories);
                                    //这里记录一个排序，在系列与，分类中的data有一个对应关系；
                                    if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && Setting.Columns.Length == 1)
                                        sortDic.Add(MyCategories, result);
                                    //记录end
                                }
                            }
                        }
                        if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && Setting.Columns.Length == 1)
                        {
                            if (Setting.SortColumns[0].Ascending)
                            {
                                sortDic = sortDic.OrderBy(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                            }
                            else
                            {
                                sortDic = sortDic.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                            }
                            Result.Categories = sortDic.Keys.ToList();
                            SeriesItem.Data = sortDic.Values.ToList();
                        }
                        Result.Series.Add(SeriesItem);
                        Counter++;
                    }
                    return Json(new { State = true, MyChartsDataResult = Result });
                }
                //一个值，一个系列
                else if ((Setting.Series != null && Setting.Series.Length == 1) && (Setting.Categories == null || Setting.Categories.Length == 0) && (Setting.Columns != null && Setting.Columns.Length == 1))
                {
                    Result.CategoryCode = null;
                    Result.CategoryDisplayName = null;
                    Result.SerieCode = Setting.Series[0].ColumnCode;
                    Result.SerieDisplayName = Setting.Series[0].DisplayName;
                    Result.SerieType = Setting.Series[0].ColumnType;
                    int Counter = 0;
                    Dictionary<SeriesForCharts, double> sortDic = new Dictionary<SeriesForCharts, double>();
                    foreach (string SeriesValue in SeriesBoIDs.Keys)
                    {
                        HashSet<string> SeriesBoIDsItem = SeriesBoIDs[SeriesValue];
                        SeriesForCharts SeriesItem = new SeriesForCharts();
                        SeriesItem.Code = SeriesValue;
                        SeriesItem.Name = SeriesItem.Code;
                        if ((Result.SerieType == ColumnType.Association || Result.SerieType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(SeriesItem.Code))
                        {
                            SeriesItem.Name = UnitTableAndAssociationTable[SeriesItem.Code];
                        }
                        double result = ChartsGetComputedValue(SeriesBoIDsItem, null, AllBos, Setting.Columns[0]);
                        //保留所设置的小数位数
                        if (!string.IsNullOrEmpty(Setting.Columns[0].Format) && Setting.Columns[0].Format.Contains("."))
                        {
                            string[] TempStrs = Setting.Columns[0].Format.Split('.');
                            result = System.Math.Round(result, TempStrs[1].Length);
                            SeriesItem.Precision = TempStrs[1].Length;
                        }
                        SeriesItem.Data.Add(result);
                        //记录一个排序
                        sortDic.Add(SeriesItem, result);
                        if (Counter == 0)
                        {
                            Result.Categories.Add(new CategoriesForCharts() { key = Setting.Columns[0].DisplayName, value = Setting.Columns[0].DisplayName });
                        }
                        Result.Series.Add(SeriesItem);
                        Counter++;
                    }
                    //记录一个排序
                    if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric)
                    {
                        if (Setting.SortColumns[0].Ascending)
                        {
                            sortDic = sortDic.OrderBy(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                        }
                        else
                        {
                            sortDic = sortDic.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                        }
                        Result.Series = sortDic.Keys.ToList();
                    }
                    return Json(new { State = true, MyChartsDataResult = Result });
                }
                //多个值，一个系列
                else if ((Setting.Series != null && Setting.Series.Length == 1) && (Setting.Categories == null || Setting.Categories.Length == 0) && (Setting.Columns != null && Setting.Columns.Length > 1))
                {
                    Result.CategoryCode = null;
                    Result.CategoryDisplayName = null;
                    Result.SerieCode = Setting.Series[0].ColumnCode;
                    Result.SerieDisplayName = Setting.Series[0].DisplayName;
                    Result.SerieType = Setting.Series[0].ColumnType;
                    //值作为系列，值是系列的汇总
                    GetSimpleCollectValue(SourceData, Result, Setting.Columns, Setting);
                    return Json(new { State = true, MyChartsDataResult = Result });
                }
                //单个分类或单个系列
                else
                {
                    Result.CategoryCode = null;
                    Result.CategoryDisplayName = null;
                    Result.SerieCode = null;
                    Result.SerieDisplayName = null;
                    Result.SerieType = ColumnType.String;
                    Result.CategoryType = ColumnType.String;
                    if (Setting.Categories != null && Setting.Categories.Length > 0)
                    {
                        Result.CategoryType = Setting.Categories[0].ColumnType;
                        Result.CategoryCode = Setting.Categories[0].ColumnCode;
                        Result.CategoryDisplayName = Setting.Categories[0].DisplayName;
                        foreach (string CategorisValue in CategorisBoIDs.Keys)
                        {
                            if (Result.CategoryType == ColumnType.Association || Result.CategoryType == ColumnType.Unit)
                            {
                                if (UnitTableAndAssociationTable.ContainsKey(CategorisValue))
                                    Result.Categories.Add(new CategoriesForCharts() { key = CategorisValue, value = UnitTableAndAssociationTable[CategorisValue] });
                                else
                                {
                                    Result.Categories.Add(new CategoriesForCharts() { key = CategorisValue, value = CategorisValue });
                                }
                            }
                            else
                            {
                                Result.Categories.Add(new CategoriesForCharts() { key = CategorisValue, value = CategorisValue });
                            }
                        }
                    }
                    if (Setting.Series != null && Setting.Series.Length > 0)
                    {
                        Result.SerieCode = Setting.Series[0].ColumnCode;
                        Result.SerieDisplayName = Setting.Series[0].DisplayName;
                        Result.SerieType = Setting.Series[0].ColumnType;
                        foreach (string SeriesValue in SeriesBoIDs.Keys)
                        {
                            HashSet<string> SeriesBoIDsItem = SeriesBoIDs[SeriesValue];
                            SeriesForCharts SeriesItem = new SeriesForCharts();
                            SeriesItem.Code = SeriesValue;
                            SeriesItem.Name = SeriesItem.Code;
                            if ((Result.SerieType == ColumnType.Association || Result.SerieType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(SeriesItem.Code))
                            {
                                SeriesItem.Name = UnitTableAndAssociationTable[SeriesItem.Code];
                            }
                            Result.Series.Add(SeriesItem);
                        }
                    }

                    return Json(new { State = true, MyChartsDataResult = Result });
                }
                #endregion
            }
            //modify by chenghs on 2018-02-01 add AllowGet
            return Json(new { State = true, SourceData = SourceData, SourceColumns = SourceColumns, Msg = "" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 处理汇总表联动
        /// </summary>
        /// <param name="UnitFilterDataJson"></param>
        /// <param name="SourceColumns"></param>
        /// <param name="reportPage"></param>
        /// <param name="Setting"></param>
        /// <param name="ReportSource"></param>
        private void DoUnit(string UnitFilterDataJson, List<ReportWidgetColumn> SourceColumns, ReportPage reportPage, ReportWidget Setting, ReportSource ReportSource)
        {
            ReportFilter[] UnitFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportFilter[]>(UnitFilterDataJson);
            if (UnitFilters != null)
            {
                Dictionary<string, ReportFilter> listFilters = new Dictionary<string, ReportFilter>();
                HashSet<string> hashSource = new HashSet<string>();
                foreach (ReportWidgetColumn q in SourceColumns)
                {
                    if (!hashSource.Contains(q.DisplayName))
                        hashSource.Add(q.DisplayName);
                }
                foreach (ReportFilter q in UnitFilters)
                {
                    if (hashSource.Contains(q.DisplayName))
                    {
                        if (!listFilters.ContainsKey(q.DisplayName))
                            listFilters.Add(q.DisplayName, q);
                    }
                }
                if (reportPage.Filters != null && reportPage.Filters.Length > 0)
                {
                    foreach (ReportFilter q in reportPage.Filters)
                    {
                        if (!listFilters.ContainsKey(q.DisplayName))
                            listFilters.Add(q.DisplayName, q);
                    }
                }

                if (listFilters != null && listFilters.Count > 0)
                    reportPage.Filters = listFilters.Values.ToArray();
                Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
                SourceColumns = this.GetSourceColumns(reportPage.Filters, Setting, ReportSource, out ChildCodes);
            }
        }
        /// <summary>
        /// 处理汇总表联动,简易看板
        /// </summary>
        /// <param name="UnitFilterDataJson"></param>
        /// <param name="SourceColumns"></param>
        /// <param name="reportPage"></param>
        /// <param name="Setting"></param>
        /// <param name="ReportSource"></param>
        private void DoUnit(string UnitFilterDataJson, List<ReportWidgetColumn> SourceColumns, ReportPage reportPage, ReportWidgetSimpleBoard Setting, ReportSource ReportSource, ref List<ReportWidgetColumn> SourceColumn)
        {
            ReportFilter[] UnitFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportFilter[]>(UnitFilterDataJson);
            if (UnitFilters != null)
            {
                Dictionary<string, ReportFilter> listFilters = new Dictionary<string, ReportFilter>();
                HashSet<string> hashSource = new HashSet<string>();
                foreach (ReportWidgetColumn q in SourceColumns)
                {
                    hashSource.Add(q.ColumnCode);
                }
                foreach (ReportFilter q in UnitFilters)
                {
                    // if (hashSource.Contains(q.ColumnCode))
                    listFilters.Add(q.ColumnCode, q);
                }
                if (reportPage.Filters != null)
                {
                    foreach (ReportFilter q in reportPage.Filters)
                    {
                        if (!listFilters.ContainsKey(q.ColumnCode))
                            listFilters.Add(q.ColumnCode, q);
                    }
                }
                if (listFilters != null && listFilters.Count > 0)
                    reportPage.Filters = listFilters.Values.ToArray();
                Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
                SourceColumn = this.GetSourceColumns(reportPage.Filters, Setting, ReportSource, this.Engine.EngineConfig.DBType);
                foreach (var fil in reportPage.Filters)
                {
                    foreach (var so in SourceColumn)
                    {
                        if (fil.ColumnCode == so.ColumnCode)
                        {
                            fil.ColumnName = so.ColumnName;
                        }
                    }

                }

            }
        }
        /// <summary>
        /// 获取报表相关数据
        /// </summary>
        /// <param name="widgetJson"></param>
        /// <param name="reportPageJson"></param>
        /// <param name="ObjectID"></param>
        /// <param name="Code"></param>
        /// <param name="reportSourceJson"></param>
        /// <param name="Setting"></param>
        /// <param name="reportPage"></param>
        /// <param name="ReportSource"></param>
        /// <returns></returns>
        private string GetReportWidgetReportPageReportSource(string widgetJson, string reportPageJson, string ObjectID, string Code, string reportSourceJson, out ReportWidget Setting, out ReportPage reportPage, out ReportSource ReportSource)
        {
            Setting = null;
            reportPage = null;
            ReportSource = null;
            if (!string.IsNullOrEmpty(reportPageJson))
                reportPage = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportPage>(reportPageJson);
            else
            {
                reportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(Code));
            }
            if (!string.IsNullOrEmpty(widgetJson))
                Setting = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportWidget>(widgetJson);
            else
            {
                if (!string.IsNullOrEmpty(ObjectID))
                {
                    foreach (ReportWidget q in reportPage.ReportWidgets)
                    {
                        if (q.ObjectID == ObjectID)
                        {
                            Setting = q;
                            break;
                        }
                    }
                }
                else
                {
                    return "参数错误";
                }
            }
            if (!string.IsNullOrEmpty(reportSourceJson))
                ReportSource = this.JsSerializer.Deserialize<ReportSource>(reportSourceJson);
            else
            {
                for (int i = 0, len = reportPage.ReportSources.Length; i < len; i++)
                {
                    if (reportPage.ReportSources[i].ObjectID == Setting.ReportSourceId)
                    {
                        ReportSource = reportPage.ReportSources[i];
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 根据时间获取第几季
        /// </summary>
        /// <param name="Time"></param>
        /// <returns></returns>
        private string GetQuarter(DateTime Time)
        {
            string result = "";
            int Month = Time.Month;
            switch (Month)
            {
                case 1:
                case 2:
                case 3:
                    return "第一季度";
                case 4:
                case 5:
                case 6:
                    return "第二季度";
                case 7:
                case 8:
                case 9:
                    return "第三季度";
                case 10:
                case 11:
                case 12:
                    return "第四季度";
            }
            return result;
        }
        /// <summary>
        /// 获取汇总表格式化数据
        /// </summary>
        /// <param name="IsExport">导入</param>
        /// <param name="SourceData">数据源</param>
        /// <param name="Setting">报表配置</param>
        /// <param name="ColumnRoad">列路径</param>
        /// <param name="RowRoad">行路径</param>
        /// <param name="UnitTableAndAssociationTable">组织机构和关联查询对应表</param>
        /// <param name="ValueTable">表体</param>
        /// <param name="MyColumnTread">列表头</param>
        /// <param name="RowSimpleHeaderNodes">行表头</param>
        /// <param name="MyCombineReport">表头</param>
        /// <param name="MyColumnTreadOnlyHeader">仅行表头和列表头</param>
        /// <param name="allReportWidgetColumn">所有ReportWidgetColumn</param>
        private void GetCombineTable(bool IsExport,
            List<Dictionary<string, object>> SourceData,
            ReportWidget Setting,
            out List<Dictionary<string, string>> ColumnRoad,
            out List<Dictionary<string, string>> RowRoad,
            Dictionary<string, string> UnitTableAndAssociationTable,
            out List<List<string>> ValueTable,
            out Dictionary<string,
            List<MyTd>> MyColumnTread,
            out List<SimpleHeaderNode> RowSimpleHeaderNodes,
            out CombineReport MyCombineReport,
            out Dictionary<string, List<MyTd>> MyColumnTreadOnlyHeader,
            out Dictionary<string, ReportWidgetColumn> allReportWidgetColumn,
            out string errorMessage)
        {
            #region 定义变量
            MyCombineReport = new CombineReport();
            Dictionary<string, HeaderNode> AllColumnHeadNode = new Dictionary<string, HeaderNode>();
            Dictionary<string, HeaderNode> AllRowHeadNode = new Dictionary<string, HeaderNode>();
            ValueTable = new List<List<string>>();//这里要先横后竖，所以要先循环行表头；
            MyColumnTread = new Dictionary<string, List<MyTd>>();
            RowSimpleHeaderNodes = new List<SimpleHeaderNode>();
            MyColumnTreadOnlyHeader = new Dictionary<string, List<MyTd>>();
            errorMessage = "";
            //行表头table转换成list<list<>>;
            string ColumnHeaderTableLastLine = "ColumnHeaderTableLastLine";
            string ColumnHeaderTableLastSecondLine = "ColumnHeaderTableLastSecondLine";
            string CollectHeaderValue = "汇总";
            bool RowHasYearFunction = false;
            bool ColumnHasYearFunction = false;
            Dictionary<string, Dictionary<string, object>> AllBos = new Dictionary<string, Dictionary<string, object>>();
            allReportWidgetColumn = new Dictionary<string, ReportWidgetColumn>();
            Dictionary<string, SorkeyForCombine> DicSorkey = new Dictionary<string, SorkeyForCombine>();
            int ColumnLeafNumber = 0;
            int RowLeafNumber = 0;
            ColumnRoad = new List<Dictionary<string, string>>();
            RowRoad = new List<Dictionary<string, string>>();
            #endregion

            #region 行表头，列表头初步处理
            if (Setting.Series != null && Setting.Series.Length > 0)
            {
                foreach (ReportWidgetColumn q in Setting.Series)
                {
                    if (!allReportWidgetColumn.ContainsKey(q.ColumnCode))
                        allReportWidgetColumn.Add(q.ColumnCode, q);
                    OnlyHeaderNode QNode = new OnlyHeaderNode();
                    QNode.DisplayName = q.DisplayName;
                    QNode.Code = q.ColumnCode;
                    QNode.FunctionType = q.FunctionType;
                    if (q.FunctionType == Function.Year)
                    {
                        ColumnHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                    }
                    else if (q.FunctionType == Function.YearAndMonth)
                    {
                        ColumnHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "季度", Code = q.ColumnCode + "Quarter", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "月", Code = q.ColumnCode + "Month", FunctionType = q.FunctionType });
                    }
                    else if (q.FunctionType == Function.YearAndMonthAndDay)
                    {
                        ColumnHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "季度", Code = q.ColumnCode + "Quarter", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "月", Code = q.ColumnCode + "Month", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "日", Code = q.ColumnCode + "Day", FunctionType = q.FunctionType });
                    }
                    else if (q.FunctionType == Function.YearAndQuarter)
                    {
                        ColumnHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "季度", Code = q.ColumnCode + "Quarter", FunctionType = q.FunctionType });
                    }
                    MyCombineReport.OnlyHeader.ColumnHeader.Add(q.ColumnCode, QNode);
                }
            }
            if (Setting.Categories != null && Setting.Categories.Length > 0)
            {
                foreach (ReportWidgetColumn q in Setting.Categories)
                {
                    if (!allReportWidgetColumn.ContainsKey(q.ColumnCode))
                        allReportWidgetColumn.Add(q.ColumnCode, q);
                    OnlyHeaderNode QNode = new OnlyHeaderNode();
                    QNode.DisplayName = q.DisplayName;
                    QNode.Code = q.ColumnCode;
                    QNode.FunctionType = q.FunctionType;
                    if (q.FunctionType == Function.Year)
                    {
                        RowHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                    }
                    else if (q.FunctionType == Function.YearAndMonth)
                    {
                        RowHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "季度", Code = q.ColumnCode + "Quarter", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "月", Code = q.ColumnCode + "Month", FunctionType = q.FunctionType });
                    }
                    else if (q.FunctionType == Function.YearAndMonthAndDay)
                    {
                        RowHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "季度", Code = q.ColumnCode + "Quarter", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "月", Code = q.ColumnCode + "Month", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "日", Code = q.ColumnCode + "Day", FunctionType = q.FunctionType });
                    }
                    else if (q.FunctionType == Function.YearAndQuarter)
                    {
                        RowHasYearFunction = true;
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "年", Code = q.ColumnCode + "Year", FunctionType = q.FunctionType });
                        QNode.ChildNodes.Add(new OnlyHeaderNode() { DisplayName = "季度", Code = q.ColumnCode + "Quarter", FunctionType = q.FunctionType });
                    }
                    MyCombineReport.OnlyHeader.RowHeader.Add(q.ColumnCode, QNode);
                }
            }
            int ColumnHeaderLength = this.GetHeaderLength(MyCombineReport.OnlyHeader.ColumnHeader);
            int RowHeaderLength = this.GetHeaderLength(MyCombineReport.OnlyHeader.RowHeader);
            #endregion

            #region 行表头树与列表头树
            foreach (Dictionary<string, object> bo in SourceData)
            {
                bool hasDbNull = false;
                foreach (string ColumnCode in MyCombineReport.OnlyHeader.ColumnHeader.Keys)
                {
                    if (bo[ColumnCode] is DBNull)
                    {
                        hasDbNull = true;
                        break;
                    }
                }
                if (hasDbNull)
                    continue;
                foreach (string ColumnCode in MyCombineReport.OnlyHeader.RowHeader.Keys)
                {
                    if (bo[ColumnCode] is DBNull)
                    {
                        hasDbNull = true;
                        break;
                    }
                }
                if (hasDbNull)
                    continue;
                string BoObjectID = System.Guid.NewGuid().ToString();
                string HeaderNodeParentObjectID = "";
                int ColumnCounter = 1;
                int RowCounter = 1;
                //为每个bo编号
                while (true)
                {
                    if (!AllBos.ContainsKey(BoObjectID))
                    {
                        AllBos.Add(BoObjectID, bo);
                        break;
                    }
                    else
                    {
                        BoObjectID = System.Guid.NewGuid().ToString();
                    }
                }
                //end为每个bo编号
                #region 列表头树
                foreach (string ColumnCode in MyCombineReport.OnlyHeader.ColumnHeader.Keys)
                {

                    string BoValue = bo[ColumnCode] + string.Empty; 
                    ReportWidgetColumn columncolumn = allReportWidgetColumn[ColumnCode];
                    if (columncolumn.ColumnType == ColumnType.Numeric)
                    {
                        BoValue = this.GetFormatValue(double.Parse(BoValue), columncolumn.Format);
                    }
                    Function Type = MyCombineReport.OnlyHeader.ColumnHeader[ColumnCode].FunctionType;
                    bool leafNodePlusOne = false;
                    //年月日处理
                    if (Type == Function.Year || Type == Function.YearAndMonth || Type == Function.YearAndMonthAndDay || Type == Function.YearAndQuarter)
                    {
                        foreach (OnlyHeaderNode node in MyCombineReport.OnlyHeader.ColumnHeader[ColumnCode].ChildNodes)
                        {
                            string tempbovalue = "";
                            if (node.Code == ColumnCode + "Year")
                                tempbovalue = DateTime.Parse(BoValue).ToString("yyyy");
                            else if (node.Code == ColumnCode + "Month")
                                tempbovalue = DateTime.Parse(BoValue).ToString("MM");
                            if (node.Code == ColumnCode + "Day")
                                tempbovalue = DateTime.Parse(BoValue).ToString("dd");
                            if (node.Code == ColumnCode + "Quarter")
                            {
                                tempbovalue = this.GetQuarter(DateTime.Parse(BoValue)); ;
                            }
                            string tempcode = node.Code;

                            HeaderNodeParentObjectID = this.BuildColumnTree(ColumnCounter, ColumnHeaderLength, AllColumnHeadNode, HeaderNodeParentObjectID, MyCombineReport, tempbovalue, tempcode, IsExport, BoObjectID, out leafNodePlusOne);

                            ColumnCounter++;
                        }
                    }
                    else
                    {
                        HeaderNodeParentObjectID = this.BuildColumnTree(ColumnCounter, ColumnHeaderLength, AllColumnHeadNode, HeaderNodeParentObjectID, MyCombineReport, BoValue, ColumnCode, IsExport, BoObjectID, out leafNodePlusOne);
                        ColumnCounter++;
                    }
                    if (leafNodePlusOne)
                        ColumnLeafNumber++;
                    //end年月日处理
                }
                #endregion

                #region 行表头树
                HeaderNodeParentObjectID = "";
                foreach (string ColumnCode in MyCombineReport.OnlyHeader.RowHeader.Keys)
                {
                    string BoValue = bo[ColumnCode] + string.Empty;
                    ReportWidgetColumn rowcolumn = allReportWidgetColumn[ColumnCode];
                    if (rowcolumn.ColumnType == ColumnType.Numeric)
                    {
                        BoValue = this.GetFormatValue(double.Parse(BoValue), rowcolumn.Format);
                    }
                    // if (string.IsNullOrEmpty(BoValue)) continue;
                    Function Type = MyCombineReport.OnlyHeader.RowHeader[ColumnCode].FunctionType;
                    bool leafNodePlusOne = false;
                    //年月日处理
                    if (Type == Function.Year || Type == Function.YearAndMonth || Type == Function.YearAndMonthAndDay || Type == Function.YearAndQuarter)
                    {
                        foreach (OnlyHeaderNode node in MyCombineReport.OnlyHeader.RowHeader[ColumnCode].ChildNodes)
                        {
                            string tempbovalue = "";
                            if (node.Code == ColumnCode + "Year")
                                tempbovalue = DateTime.Parse(BoValue).ToString("yyyy");
                            else if (node.Code == ColumnCode + "Month")
                                tempbovalue = DateTime.Parse(BoValue).ToString("MM");
                            if (node.Code == ColumnCode + "Day")
                                tempbovalue = DateTime.Parse(BoValue).ToString("dd");
                            if (node.Code == ColumnCode + "Quarter")
                            {
                                tempbovalue = this.GetQuarter(DateTime.Parse(BoValue)); ;
                            }
                            string tempcode = node.Code;
                            HeaderNodeParentObjectID = this.BuildRowTree(RowCounter, RowHeaderLength, AllRowHeadNode, HeaderNodeParentObjectID, MyCombineReport, tempbovalue, tempcode, IsExport, BoObjectID, out leafNodePlusOne);
                            RowCounter++;
                        }
                    }
                    else
                    {
                        HeaderNodeParentObjectID = this.BuildRowTree(RowCounter, RowHeaderLength, AllRowHeadNode, HeaderNodeParentObjectID, MyCombineReport, BoValue, ColumnCode, IsExport, BoObjectID, out leafNodePlusOne);
                        RowCounter++;
                    }
                    if (leafNodePlusOne)
                        RowLeafNumber++;
                    //end年月日处理
                }
                #endregion

            }
            #endregion

            #region 如果超过40000行,返回
            if (!IsExport)
            {
                if (Setting.Columns != null)
                {
                    int num = (RowLeafNumber * ColumnLeafNumber * Setting.Columns.Length);
                    if (num > MaxRowMutiplyColumn)
                    {
                        errorMessage = "数量超过显示限制";
                        return;
                    }
                }
            }

            #endregion

            #region 表体
            List<HeaderNode> AllColumnLeafNode = new List<HeaderNode>();//之后用于生成"列Table"
            List<HeaderNode> AllRowLeafNode = new List<HeaderNode>();//之后用于生成"行Table"
            bool OneForeachOver = false;

            ReportWidgetColumn[] ComputeColumns = Setting.Columns;
            Dictionary<string, Dictionary<string, myCollect>> ColumnCollect = new Dictionary<string, Dictionary<string, myCollect>>();//列汇总
            Dictionary<string, myCollect> RowCollectAndColumnCollect = new Dictionary<string, myCollect>();
            if (MyCombineReport.OnlyHeader.ColumnHeader != null && MyCombineReport.OnlyHeader.ColumnHeader.Count > 0 && MyCombineReport.OnlyHeader.RowHeader != null && MyCombineReport.OnlyHeader.RowHeader.Count > 0)
            {
                foreach (string RowNodeKey in MyCombineReport.RowHeaderNodes.Keys)
                {
                    HeaderNode RowNode = MyCombineReport.RowHeaderNodes[RowNodeKey];
                    List<HeaderNode> PartRowLeafNodes = GetLeafNode(RowNode, AllRowHeadNode);
                    List<string> RowValues = new List<string>();
                    foreach (HeaderNode RowLeafNode in PartRowLeafNodes)
                    {
                        //  AllRowLeafNode.Add(RowLeafNode);
                        if (OneForeachOver)
                        {
                            Dictionary<string, myCollect> RowCollect = new Dictionary<string, myCollect>();//行汇总
                            RowValues = new List<string>();
                            foreach (HeaderNode ColumnLeafNode in AllColumnLeafNode)
                            {
                                //得出交叉点的boIds，根据计算字段计算出值，填入存数据表的数据结构；
                                if (ComputeColumns != null && ComputeColumns.Length > 0)
                                {
                                    foreach (ReportWidgetColumn ComputeColumn in ComputeColumns)
                                    {
                                        RowValues.Add(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable));

                                    }
                                }
                            }
                            //行汇总值
                            foreach (string ComputeColumnCode in RowCollect.Keys)
                            {
                                RowValues.Add(GetCollectValue(RowCollect[ComputeColumnCode]));
                            }
                            //end行汇总值
                            //记录一个排序
                            if (Setting.Categories != null && Setting.Categories.Length == 1 && Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && RowCollect.ContainsKey(Setting.SortColumns[0].ColumnCode))
                            {
                                double tempvalue = 0;
                                double.TryParse(GetCollectValue(RowCollect[Setting.SortColumns[0].ColumnCode]), out tempvalue);
                                DicSorkey.Add(RowLeafNode.HeaderValue, new SorkeyForCombine() { CollectValue = tempvalue, RowNumber = ValueTable.Count });
                            }
                            //end记录一个排序
                            ValueTable.Add(RowValues);
                        }
                        else
                        {
                            Dictionary<string, myCollect> RowCollect = new Dictionary<string, myCollect>();//行汇总
                            RowValues = new List<string>();
                            foreach (string ColumnNodeKey in MyCombineReport.ColumnHeaderNodes.Keys)
                            {
                                HeaderNode ColumnNode = MyCombineReport.ColumnHeaderNodes[ColumnNodeKey];
                                List<HeaderNode> PartColumnLeafNodes = GetLeafNode(ColumnNode, AllColumnHeadNode);
                                foreach (HeaderNode ColumnLeafNode in PartColumnLeafNodes)
                                {
                                    AllColumnLeafNode.Add(ColumnLeafNode);
                                    //得出交叉点的boIds，根据计算字段计算出值，填入存数据表的数据结构；
                                    if (ComputeColumns != null && ComputeColumns.Length > 0)
                                    {
                                        foreach (ReportWidgetColumn ComputeColumn in ComputeColumns)
                                        {
                                            string Daystring = GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable);
                                            //列增加一个DataTime时间类型的年月日 格式
                                            if (!String.IsNullOrEmpty(Daystring) && !Daystring.Equals("-", StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                switch (ComputeColumn.FunctionType)
                                                {
                                                    case Function.Year:
                                                        Daystring = DateTime.Parse(Daystring).ToString("yyyy") + "年";
                                                        break;
                                                    case Function.YearAndMonth:
                                                        Daystring = String.Format("{0:Y}", DateTime.Parse(Daystring));
                                                        break;
                                                    case Function.YearAndMonthAndDay:
                                                        Daystring = string.Format("{0:D}", DateTime.Parse(Daystring));
                                                        break;
                                                    case Function.YearAndQuarter:
                                                        Daystring = this.GetQuarter(DateTime.Parse(Daystring));
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            //string Daystring = "";
                                            //string Time=GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable);
                                            //if (ComputeColumn.FunctionType == Function.Year)
                                            //{

                                            //    if (Time != "" && Time != "-")
                                            //    {
                                            //        Daystring = DateTime.Parse(Time).ToString("yyyy") + "年";
                                            //    }
                                            //    else 
                                            //    {
                                            //        Daystring = "";
                                            //    }

                                            //}
                                            //else if (ComputeColumn.FunctionType == Function.YearAndMonth)
                                            //{

                                            //    if (Time != "" && Time != "-")
                                            //    {
                                            //        Daystring = DateTime.Parse(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable)).ToString("yyyy") + "年" + DateTime.Parse(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable)).ToString("MM") + "月";
                                            //    }
                                            //    else 
                                            //    {
                                            //        Daystring = "";
                                            //    }

                                            //}
                                            //else if (ComputeColumn.FunctionType == Function.YearAndMonthAndDay)
                                            //{
                                            //    if (Time != "" && Time != "-")
                                            //    {
                                            //        Daystring = DateTime.Parse(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable)).ToString("yyyy") + "年" + DateTime.Parse(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable)).ToString("MM") + "月" + DateTime.Parse(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable)).ToString("dd") + "日";
                                            //    }
                                            //    else {
                                            //        Daystring = "";
                                            //    }
                                            //}
                                            //else if (ComputeColumn.FunctionType == Function.YearAndQuarter)
                                            //{
                                            //    if (Time != "" && Time != "-")
                                            //    {
                                            //    Daystring = this.GetQuarter(DateTime.Parse(GetComputedValue(ComputeColumn, ColumnLeafNode, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable)));
                                            //}else{
                                            // Daystring = "";
                                            //}
                                            //}


                                            //加一个结构，包含bo的值
                                            RowValues.Add(Daystring);
                                            //导出时不做此操作
                                            if (!IsExport)
                                            {
                                                //列路径
                                                ColumnRoad.Add(ColumnLeafNode.Road);
                                            }

                                        }
                                    }
                                }
                            }
                            //行汇总值
                            foreach (string ColumnCode in RowCollect.Keys)
                            {
                                RowValues.Add(GetCollectValue(RowCollect[ColumnCode]));
                            }
                            //end行汇总值
                            //记录一个排序
                            if (Setting.Categories != null && Setting.Categories.Length == 1 && Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && RowCollect.ContainsKey(Setting.SortColumns[0].ColumnCode))
                            {
                                double tempvalue = 0;
                                double.TryParse(GetCollectValue(RowCollect[Setting.SortColumns[0].ColumnCode]), out tempvalue);
                                DicSorkey.Add(RowLeafNode.HeaderValue, new SorkeyForCombine() { CollectValue = tempvalue, RowNumber = ValueTable.Count });
                            }
                            //end记录一个排序
                            ValueTable.Add(RowValues);
                            OneForeachOver = true;
                        }
                        //行路径 //导出时不做此操作
                        if (!IsExport)
                            RowRoad.Add(RowLeafNode.Road);
                    }
                }
            }
            else if (MyCombineReport.OnlyHeader.ColumnHeader != null && MyCombineReport.OnlyHeader.ColumnHeader.Count > 0 && (MyCombineReport.OnlyHeader.RowHeader == null || MyCombineReport.OnlyHeader.RowHeader.Count == 0))
            {
                Dictionary<string, myCollect> RowCollect = new Dictionary<string, myCollect>();//行汇总
                List<string> RowValues = new List<string>();
                foreach (string ColumnNodeKey in MyCombineReport.ColumnHeaderNodes.Keys)
                {
                    HeaderNode ColumnNode = MyCombineReport.ColumnHeaderNodes[ColumnNodeKey];
                    List<HeaderNode> PartColumnLeafNodes = GetLeafNode(ColumnNode, AllColumnHeadNode);
                    foreach (HeaderNode ColumnLeafNode in PartColumnLeafNodes)
                    {
                        AllColumnLeafNode.Add(ColumnLeafNode);
                        //得出交叉点的boIds，根据计算字段计算出值，填入存数据表的数据结构；
                        if (ComputeColumns != null && ComputeColumns.Length > 0)
                        {
                            foreach (ReportWidgetColumn ComputeColumn in ComputeColumns)
                            {
                                //加一个结构，包含bo的值
                                GetComputedValue(ComputeColumn, ColumnLeafNode, null, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable);
                            }
                        }
                    }
                }
            }
            else if ((MyCombineReport.OnlyHeader.ColumnHeader == null || MyCombineReport.OnlyHeader.ColumnHeader.Count == 0) && (MyCombineReport.OnlyHeader.RowHeader != null && MyCombineReport.OnlyHeader.RowHeader.Count > 0))
            {
                foreach (string RowNodeKey in MyCombineReport.RowHeaderNodes.Keys)
                {
                    HeaderNode RowNode = MyCombineReport.RowHeaderNodes[RowNodeKey];
                    List<HeaderNode> PartRowLeafNodes = GetLeafNode(RowNode, AllRowHeadNode);
                    List<string> RowValues = new List<string>();
                    foreach (HeaderNode RowLeafNode in PartRowLeafNodes)
                    {
                        Dictionary<string, myCollect> RowCollect = new Dictionary<string, myCollect>();//行汇总
                        RowValues = new List<string>();
                        //得出交叉点的boIds，根据计算字段计算出值，填入存数据表的数据结构；
                        if (ComputeColumns != null && ComputeColumns.Length > 0)
                        {
                            foreach (ReportWidgetColumn ComputeColumn in ComputeColumns)
                            {
                                GetComputedValue(ComputeColumn, null, RowLeafNode, AllBos, RowCollect, ColumnCollect, RowCollectAndColumnCollect, UnitTableAndAssociationTable);
                            }
                        }
                        //行汇总值
                        foreach (string ColumnCode in RowCollect.Keys)
                        {
                            RowValues.Add(GetCollectValue(RowCollect[ColumnCode]));
                        }
                        //end行汇总值
                        //记录一个排序
                        if (Setting.Categories != null && Setting.Categories.Length == 1 && Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric && RowCollect.ContainsKey(Setting.SortColumns[0].ColumnCode))
                        {
                            double tempvalue = 0;
                            double.TryParse(GetCollectValue(RowCollect[Setting.SortColumns[0].ColumnCode]), out tempvalue);
                            DicSorkey.Add(RowLeafNode.HeaderValue, new SorkeyForCombine() { CollectValue = tempvalue, RowNumber = ValueTable.Count });
                        }
                        //end记录一个排序
                        ValueTable.Add(RowValues);
                    }
                }
            }
            //列汇总
            List<string> ColumnCollectList = new List<string>();
            foreach (string ColumnObjectID in ColumnCollect.Keys)
            {
                foreach (string ColumnCode in ColumnCollect[ColumnObjectID].Keys)
                {
                    ColumnCollectList.Add(GetCollectValue(ColumnCollect[ColumnObjectID][ColumnCode]));
                }
            }
            foreach (string ComputeColumnCode in RowCollectAndColumnCollect.Keys)
            {
                ColumnCollectList.Add(GetCollectValue(RowCollectAndColumnCollect[ComputeColumnCode]));
            }
            if (RowCollectAndColumnCollect.Count == 0)
            {
                if (Setting.Columns != null)
                {
                    foreach (ReportWidgetColumn column in Setting.Columns)
                    {
                        if (column.ColumnType == ColumnType.Numeric)
                        {
                            ColumnCollectList.Add("0");
                        }
                        else
                        {
                            ColumnCollectList.Add("-");
                        }
                    }
                }
            }
            ValueTable.Add(ColumnCollectList);
            //end列汇总
            #endregion

            #region 当行表头一维，并且仅按值排序时，重新生成表格;生成表体时记录一个排序，这里按照记录重新排序行表头
            if (Setting.Categories != null && Setting.Categories.Length == 1 && Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == ColumnType.Numeric)
            {
                if (Setting.Categories[0].ColumnType != ColumnType.DateTime || (Setting.Categories[0].ColumnType == ColumnType.DateTime && Setting.Categories[0].FunctionType == Function.Year))
                {
                    if (Setting.SortColumns[0].Ascending)
                    {
                        DicSorkey = DicSorkey.OrderBy(p => p.Value.CollectValue).ToDictionary(p => p.Key, o => o.Value);
                    }
                    else
                    {
                        DicSorkey = DicSorkey.OrderByDescending(p => p.Value.CollectValue).ToDictionary(p => p.Key, o => o.Value);
                    }
                    Dictionary<string, HeaderNode> newRowSimpleHeaderNodes = new Dictionary<string, HeaderNode>();
                    List<List<string>> newValueTable = new List<List<string>>();
                    List<Dictionary<string, string>> newRowRoad = new List<Dictionary<string, string>>();
                    if (DicSorkey.Count > 0)
                    {
                        foreach (string q in DicSorkey.Keys)
                        {
                            newRowSimpleHeaderNodes.Add(q, MyCombineReport.RowHeaderNodes[q]);
                            newValueTable.Add(ValueTable[DicSorkey[q].RowNumber]);
                            //记录的顺序也要变
                            if (DicSorkey[q].RowNumber < RowRoad.Count)
                                newRowRoad.Add(RowRoad[DicSorkey[q].RowNumber]);
                        }
                        newValueTable.Add(ValueTable[ValueTable.Count - 1]);
                        ValueTable = newValueTable;
                        RowRoad = newRowRoad;
                        MyCombineReport.RowHeaderNodes = newRowSimpleHeaderNodes;
                    }
                }
            }
            #endregion

            #region 列表头table，用于前台展现
            if (true || (MyCombineReport.OnlyHeader.ColumnHeader != null && MyCombineReport.OnlyHeader.ColumnHeader.Count > 0 && MyCombineReport.OnlyHeader.RowHeader != null && MyCombineReport.OnlyHeader.RowHeader.Count > 0))
            {
                string FirstLineCode = "";
                int TempCount = 0;
                //仅表头table
                foreach (string key in MyCombineReport.OnlyHeader.ColumnHeader.Keys)
                {
                    Function Type = MyCombineReport.OnlyHeader.ColumnHeader[key].FunctionType;
                    OnlyHeaderNode ItemNode = MyCombineReport.OnlyHeader.ColumnHeader[key];
                    //error类似这种写法，扩展的时候需要重写；
                    if (Type == Function.Year)
                    {
                        foreach (OnlyHeaderNode node in MyCombineReport.OnlyHeader.ColumnHeader[key].ChildNodes)
                        {
                            int YearCount = 0;
                            if (TempCount == 0)
                            {
                                FirstLineCode = node.Code;
                            }
                            if (YearCount == 0)
                            {
                                this.BuildTd(MyColumnTread, node.Code, ItemNode.DisplayName, ItemNode.ChildNodes, RowHeaderLength - 1);
                            }
                            this.BuildTd(MyColumnTread, node.Code, node.DisplayName, node.ChildNodes, 1);
                            YearCount++;
                            TempCount++;
                        }
                    }
                    else if (Type == Function.YearAndMonth || Type == Function.YearAndMonthAndDay || Type == Function.YearAndQuarter)
                    {
                        int YearCount = 0;
                        foreach (OnlyHeaderNode node in MyCombineReport.OnlyHeader.ColumnHeader[key].ChildNodes)
                        {
                            if (TempCount == 0)
                            {
                                FirstLineCode = node.Code;
                            }
                            if (YearCount == 0)
                            {
                                this.BuildTd(MyColumnTread, node.Code, ItemNode.DisplayName, ItemNode.ChildNodes, RowHeaderLength - 1);
                            }
                            this.BuildTd(MyColumnTread, node.Code, node.DisplayName, node.ChildNodes, 1);
                            TempCount++;
                            YearCount++;
                        }
                    }
                    else
                    {
                        if (ColumnHasYearFunction)
                        {
                            if (TempCount == 0)
                            {
                                FirstLineCode = key;
                            }
                            //现在只有两层父子关系，所以比较2与RowHeaderlength取大的
                            if (RowHeaderLength < 2)
                                this.BuildTd(MyColumnTread, key, ItemNode.DisplayName, ItemNode.ChildNodes, 2);
                            else
                                this.BuildTd(MyColumnTread, key, ItemNode.DisplayName, ItemNode.ChildNodes, RowHeaderLength);
                            TempCount++;
                        }
                        else
                        {
                            if (TempCount == 0)
                            {
                                FirstLineCode = key;
                            }
                            this.BuildTd(MyColumnTread, key, ItemNode.DisplayName, ItemNode.ChildNodes, RowHeaderLength);
                            TempCount++;
                        }
                    }
                }
                //列表头的最后一行或两行
                if (RowHasYearFunction)
                {
                    MyColumnTread.Add(ColumnHeaderTableLastSecondLine, new List<MyTd>());
                }

                if (MyCombineReport.OnlyHeader.RowHeader != null && MyCombineReport.OnlyHeader.RowHeader.Count > 0)
                {
                    MyColumnTread.Add(ColumnHeaderTableLastLine, new List<MyTd>());
                    foreach (string key in MyCombineReport.OnlyHeader.RowHeader.Keys)
                    {
                        OnlyHeaderNode rownode = MyCombineReport.OnlyHeader.RowHeader[key];
                        if (RowHasYearFunction)
                        {
                            bool isYearFunction = false;
                            if (rownode.FunctionType == Function.Year || rownode.FunctionType == Function.YearAndMonth || rownode.FunctionType == Function.YearAndMonthAndDay || rownode.FunctionType == Function.YearAndQuarter)
                            {
                                isYearFunction = true;
                                foreach (OnlyHeaderNode RowOnlyHeaderNode in rownode.ChildNodes)
                                {
                                    MyTd tempTd = new MyTd();
                                    tempTd.RowSpan = 1;
                                    if (RowHeaderLength < 2 && ColumnHasYearFunction)
                                        tempTd.ColSpan = 2;
                                    else
                                        tempTd.ColSpan = 1;
                                    tempTd.Value = RowOnlyHeaderNode.DisplayName;
                                    MyColumnTread[ColumnHeaderTableLastLine].Add(tempTd);
                                }
                            }
                            int colspan = 1;
                            int rowspan = 1;
                            if (isYearFunction)
                            {
                                colspan = rownode.ChildNodes == null || rownode.ChildNodes.Count == 0 ? 1 : rownode.ChildNodes.Count;
                                if (RowHeaderLength < 2 && ColumnHasYearFunction)
                                    colspan = 2;
                            }
                            else
                            {
                                rowspan = 2;
                            }
                            MyTd Td = new MyTd();
                            Td.RowSpan = rowspan;
                            Td.ColSpan = colspan;
                            Td.Value = MyCombineReport.OnlyHeader.RowHeader[key].DisplayName;
                            MyColumnTread[ColumnHeaderTableLastSecondLine].Add(Td);
                        }
                        else
                        {
                            MyTd Td = new MyTd();
                            Td.RowSpan = 1;
                            if (MyCombineReport.OnlyHeader.RowHeader.Keys.Count == 1 && ColumnHasYearFunction)
                                Td.ColSpan = 2;
                            else
                                Td.ColSpan = 1;
                            Td.Value = MyCombineReport.OnlyHeader.RowHeader[key].DisplayName;
                            MyColumnTread[ColumnHeaderTableLastLine].Add(Td);
                        }
                    }
                }
                //end列表头的最后一行或两行 
                string tempSer = this.JsSerializer.Serialize(MyColumnTread).Replace("\\", "\\\\");
                MyColumnTreadOnlyHeader = this.JsSerializer.Deserialize<Dictionary<string, List<MyTd>>>(tempSer);
                //end 仅表头table
                foreach (string Key in MyCombineReport.ColumnHeaderNodes.Keys)
                {
                    int ColumnLength = Setting.Columns == null || Setting.Columns.Length == 0 ? 1 : Setting.Columns.Length;
                    string ColumnCode = MyCombineReport.ColumnHeaderNodes[Key].ColumnCode;
                    if (allReportWidgetColumn.ContainsKey(ColumnCode))
                    {
                        GetColSpan(MyCombineReport.ColumnHeaderNodes[Key], AllColumnHeadNode, ColumnLength, MyColumnTread, UnitTableAndAssociationTable, allReportWidgetColumn);
                    }
                    else
                    {
                        GetColSpan(MyCombineReport.ColumnHeaderNodes[Key], AllColumnHeadNode, ColumnLength, MyColumnTread, UnitTableAndAssociationTable, allReportWidgetColumn);
                    }
                }
                #region //汇总行
                if (Setting.Columns != null && Setting.Columns.Length > 0)
                {
                    MyTd CollectTd = new MyTd();
                    CollectTd.ColSpan = Setting.Columns.Length;
                    CollectTd.RowSpan = ColumnHeaderLength;
                    CollectTd.Value = CollectHeaderValue;
                    if (MyCombineReport.OnlyHeader.ColumnHeader != null && MyCombineReport.OnlyHeader.ColumnHeader.Count > 0)
                    {
                        if (MyColumnTread.ContainsKey(FirstLineCode))
                            MyColumnTread[FirstLineCode].Add(CollectTd);
                    }
                }
                #endregion  //end汇总行
            }

            if (AllColumnLeafNode != null && AllColumnLeafNode.Count > 0)
            {
                foreach (HeaderNode Node in AllColumnLeafNode)
                {
                    if (Setting.Columns != null && Setting.Columns.Length > 0)
                    {
                        foreach (ReportWidgetColumn Column in Setting.Columns)
                        {
                            MyTd Td = new MyTd();
                            Td.ColSpan = 1;
                            Td.Value = Column.DisplayName;
                            if (RowHasYearFunction)
                            {
                                Td.RowSpan = 2;
                                if (!MyColumnTread.ContainsKey(ColumnHeaderTableLastSecondLine))
                                {
                                    MyColumnTread.Add(ColumnHeaderTableLastSecondLine, new List<MyTd>());
                                    MyTd tempTd = new MyTd();
                                    tempTd.ColSpan = 1;
                                    tempTd.Value = "";
                                    tempTd.RowSpan = 2;
                                    if (ColumnHasYearFunction)
                                        tempTd.ColSpan = 2;
                                    MyColumnTread[ColumnHeaderTableLastSecondLine].Add(tempTd);
                                }
                                MyColumnTread[ColumnHeaderTableLastSecondLine].Add(Td);
                            }
                            else
                            {
                                Td.RowSpan = 1;
                                if (!MyColumnTread.ContainsKey(ColumnHeaderTableLastLine))
                                {
                                    MyColumnTread.Add(ColumnHeaderTableLastLine, new List<MyTd>());
                                    MyTd tempTd = new MyTd();
                                    tempTd.ColSpan = 1;
                                    tempTd.Value = "";
                                    tempTd.RowSpan = 1;
                                    if (ColumnHasYearFunction)
                                        tempTd.ColSpan = 2;
                                    MyColumnTread[ColumnHeaderTableLastLine].Add(tempTd);
                                }
                                MyColumnTread[ColumnHeaderTableLastLine].Add(Td);
                            }
                        }
                    }
                }
            }
            //汇总
            if (Setting.Columns != null && Setting.Columns.Length > 0)
            {
                foreach (ReportWidgetColumn Column in Setting.Columns)
                {
                    MyTd Td = new MyTd();
                    Td.ColSpan = 1;
                    Td.Value = Column.DisplayName;
                    if (RowHasYearFunction)
                    {
                        Td.RowSpan = 2;
                        MyColumnTread[ColumnHeaderTableLastSecondLine].Add(Td);
                    }
                    else if (MyColumnTread.ContainsKey(ColumnHeaderTableLastLine))
                    {
                        Td.RowSpan = 1;
                        MyColumnTread[ColumnHeaderTableLastLine].Add(Td);
                    }
                }
            }
            //汇总
            //end列表头的最后一行
            #endregion

            #region 行表头table，用于前台展现
            //行表头在前端，以最左一列为准，一大格，一大格展现；
            foreach (string key in MyCombineReport.RowHeaderNodes.Keys)
            {
                if (RowHeaderLength < 2 && ColumnHasYearFunction)
                {
                    RowSimpleHeaderNodes.Add(GetSimpleHeaderNode(MyCombineReport.RowHeaderNodes[key], AllRowHeadNode, true, 2));
                }
                else
                {
                    RowSimpleHeaderNodes.Add(GetSimpleHeaderNode(MyCombineReport.RowHeaderNodes[key], AllRowHeadNode, false, 0));
                }

            }
            //最后一行汇总
            if (Setting.Columns != null && Setting.Columns.Length > 0)
            {
                SimpleHeaderNode LastRowHeaderSimpleHeaderNode = new SimpleHeaderNode();
                LastRowHeaderSimpleHeaderNode.Value = CollectHeaderValue;
                if (RowHeaderLength < 2 && ColumnHasYearFunction)
                    LastRowHeaderSimpleHeaderNode.ColSpan = 2;
                else
                    LastRowHeaderSimpleHeaderNode.ColSpan = RowHeaderLength;
                LastRowHeaderSimpleHeaderNode.RowSpan = 1;
                RowSimpleHeaderNodes.Add(LastRowHeaderSimpleHeaderNode);
            }
            //end最后行汇总
            #endregion


        }
        /// <summary>
        /// 生成td
        /// </summary>
        /// <param name="MyColumnTread"></param>
        /// <param name="key"></param>
        /// <param name="Code"></param>
        /// <param name="Nodes"></param>
        /// <param name="length"></param>
        private void BuildTd(Dictionary<string, List<MyTd>> MyColumnTread, string key, string Code, List<OnlyHeaderNode> Nodes, int length)
        {
            if (!MyColumnTread.ContainsKey(key))
                MyColumnTread.Add(key, new List<MyTd>());
            MyTd Td = new MyTd();
            Td.Value = Code;
            Td.Code = Code;
            int ColummnColSpan = length;
            Td.ColSpan = ColummnColSpan <= 0 ? 1 : ColummnColSpan;
            int rowspan = this.GetHeaderLength(Nodes);
            Td.RowSpan = rowspan <= 0 ? 1 : rowspan;
            MyColumnTread[key].Add(Td);
        }
        /// <summary>
        /// 生成列表头树
        /// </summary>
        /// <param name="ColumnCounter"></param>
        /// <param name="ColumnHeaderLength"></param>
        /// <param name="AllColumnHeadNode"></param>
        /// <param name="HeaderNodeParentObjectID"></param>
        /// <param name="MyCombineReport"></param>
        /// <param name="BoValue"></param>
        /// <param name="ColumnCode"></param>
        /// <param name="IsExport"></param>
        /// <param name="BoObjectID"></param>
        /// <returns></returns>
        private string BuildColumnTree(int ColumnCounter, int ColumnHeaderLength, Dictionary<string, HeaderNode> AllColumnHeadNode, string HeaderNodeParentObjectID, CombineReport MyCombineReport, string BoValue, string ColumnCode, bool IsExport, string BoObjectID, out bool leafNodePlusOne)
        {
            string NodeObjectID = Guid.NewGuid().ToString();
            bool IsLeafNode = ColumnCounter == ColumnHeaderLength ? true : false;
            leafNodePlusOne = false;
            while (true)
            {
                if (AllColumnHeadNode.ContainsKey(NodeObjectID))
                    NodeObjectID = Guid.NewGuid().ToString();
                else
                    break;
            }
            if (HeaderNodeParentObjectID == "")
            {
                HeaderNodeParentObjectID = NodeObjectID;
            }
            if (NodeObjectID == HeaderNodeParentObjectID)
            {
                if (!MyCombineReport.ColumnHeaderNodes.ContainsKey(BoValue))
                {

                    HeaderNode TempHeaderNode = new HeaderNode();
                    TempHeaderNode.IsLeaf = IsLeafNode;
                    TempHeaderNode.ColumnCode = ColumnCode;
                    TempHeaderNode.ObjectID = NodeObjectID;
                    TempHeaderNode.HeaderValue = BoValue;
                    if (!IsExport)
                    {
                        if (!TempHeaderNode.Road.ContainsKey(ColumnCode))
                            TempHeaderNode.Road.Add(ColumnCode, BoValue);
                    }
                    MyCombineReport.ColumnHeaderNodes.Add(BoValue, TempHeaderNode);
                    AllColumnHeadNode.Add(NodeObjectID, TempHeaderNode);
                    if (!IsExport && IsLeafNode)
                        leafNodePlusOne = true;
                }
                HeaderNodeParentObjectID = MyCombineReport.ColumnHeaderNodes[BoValue].ObjectID;
                if (IsLeafNode)
                    MyCombineReport.ColumnHeaderNodes[BoValue].BoObjectIDs.Add(BoObjectID);
            }
            else
            {
                HeaderNode ParentNode = AllColumnHeadNode[HeaderNodeParentObjectID];
                if (!ParentNode.ChildrenValueAndObjectID.ContainsKey(BoValue))
                {

                    HeaderNode TempHeaderNode = new HeaderNode();
                    TempHeaderNode.IsLeaf = IsLeafNode;
                    TempHeaderNode.ColumnCode = ColumnCode;
                    TempHeaderNode.ObjectID = NodeObjectID;
                    TempHeaderNode.HeaderValue = BoValue;
                    if (!IsExport)
                    {
                        TempHeaderNode.Road = (Dictionary<string, string>)OThinker.Data.Utility.Clone(ParentNode.Road);
                        if (!TempHeaderNode.Road.ContainsKey(ColumnCode))
                            TempHeaderNode.Road.Add(ColumnCode, BoValue);
                        else
                        {
                            TempHeaderNode.Road[ColumnCode] = BoValue;
                        }
                    }
                    AllColumnHeadNode.Add(NodeObjectID, TempHeaderNode);
                    ParentNode.ChildrenValueAndObjectID.Add(BoValue, NodeObjectID);
                    if (!IsExport && IsLeafNode)
                        leafNodePlusOne = true;
                }
                HeaderNodeParentObjectID = ParentNode.ChildrenValueAndObjectID[BoValue];
                if (IsLeafNode)
                    AllColumnHeadNode[HeaderNodeParentObjectID].BoObjectIDs.Add(BoObjectID);
            }
            return HeaderNodeParentObjectID;
        }
        /// <summary>
        /// 生成行表头树
        /// </summary>
        /// <param name="RowCounter">行号，</param>
        /// <param name="RowHeaderLength"></param>
        /// <param name="AllRowHeadNode"></param>
        /// <param name="HeaderNodeParentObjectID"></param>
        /// <param name="MyCombineReport"></param>
        /// <param name="BoValue"></param>
        /// <param name="ColumnCode"></param>
        /// <param name="IsExport"></param>
        /// <param name="BoObjectID"></param>
        /// <returns></returns>
        private string BuildRowTree(int RowCounter, int RowHeaderLength, Dictionary<string, HeaderNode> AllRowHeadNode, string HeaderNodeParentObjectID, CombineReport MyCombineReport, string BoValue, string ColumnCode, bool IsExport, string BoObjectID, out bool leafNodePlusOne)
        {
            string NodeObjectID = Guid.NewGuid().ToString();
            bool IsLeafNode = RowCounter == RowHeaderLength ? true : false;
            leafNodePlusOne = false;
            while (true)
            {
                if (AllRowHeadNode.ContainsKey(NodeObjectID))
                    NodeObjectID = Guid.NewGuid().ToString();
                else
                    break;
            }
            if (HeaderNodeParentObjectID == "")
            {
                HeaderNodeParentObjectID = NodeObjectID;
            }
            if (NodeObjectID == HeaderNodeParentObjectID)
            {
                if (!MyCombineReport.RowHeaderNodes.ContainsKey(BoValue))
                {

                    HeaderNode TempHeaderNode = new HeaderNode();
                    TempHeaderNode.IsLeaf = IsLeafNode;
                    TempHeaderNode.ColumnCode = ColumnCode;
                    TempHeaderNode.ObjectID = NodeObjectID;
                    TempHeaderNode.HeaderValue = BoValue;
                    if (!IsExport)
                    {
                        if (!TempHeaderNode.Road.ContainsKey(ColumnCode))
                            TempHeaderNode.Road.Add(ColumnCode, BoValue);
                    }
                    MyCombineReport.RowHeaderNodes.Add(BoValue, TempHeaderNode);
                    AllRowHeadNode.Add(NodeObjectID, TempHeaderNode);
                    if (!IsExport && IsLeafNode)
                        leafNodePlusOne = true;
                }
                HeaderNodeParentObjectID = MyCombineReport.RowHeaderNodes[BoValue].ObjectID;
                if (IsLeafNode)
                    MyCombineReport.RowHeaderNodes[BoValue].BoObjectIDs.Add(BoObjectID);
            }
            else
            {
                HeaderNode ParentNode = AllRowHeadNode[HeaderNodeParentObjectID];
                if (!ParentNode.ChildrenValueAndObjectID.ContainsKey(BoValue))
                {
                    HeaderNode TempHeaderNode = new HeaderNode();
                    TempHeaderNode.IsLeaf = IsLeafNode;
                    TempHeaderNode.ColumnCode = ColumnCode;
                    TempHeaderNode.ObjectID = NodeObjectID;
                    TempHeaderNode.HeaderValue = BoValue;
                    if (!IsExport)
                    {
                        TempHeaderNode.Road = (Dictionary<string, string>)OThinker.Data.Utility.Clone(ParentNode.Road);
                        if (!TempHeaderNode.Road.ContainsKey(ColumnCode))
                            TempHeaderNode.Road.Add(ColumnCode, BoValue);
                        else
                        {
                            TempHeaderNode.Road[ColumnCode] = BoValue;
                        }
                    }
                    AllRowHeadNode.Add(NodeObjectID, TempHeaderNode);
                    ParentNode.ChildrenValueAndObjectID.Add(BoValue, NodeObjectID);
                    if (!IsExport && IsLeafNode)
                        leafNodePlusOne = true;
                }
                HeaderNodeParentObjectID = ParentNode.ChildrenValueAndObjectID[BoValue];
                if (IsLeafNode)
                    AllRowHeadNode[HeaderNodeParentObjectID].BoObjectIDs.Add(BoObjectID);
            }
            return HeaderNodeParentObjectID;
        }
        /// <summary>
        /// 获取表头长度
        /// </summary>
        /// <param name="RowHeaderNodes"></param>
        /// <returns></returns>
        private int GetHeaderLength(Dictionary<string, OnlyHeaderNode> RowHeaderNodes)
        {
            int Result = 0;
            if (RowHeaderNodes != null && RowHeaderNodes.Count > 0)
            {
                foreach (string RowColumnCode in RowHeaderNodes.Keys)
                {
                    if (RowHeaderNodes[RowColumnCode].IsLeafNode)
                        Result++;
                    else
                        Result = Result + this.GetHeaderLength(RowHeaderNodes[RowColumnCode].ChildNodes);
                }
            }
            return Result;
        }
        /// <summary>
        /// 获取表头长度
        /// </summary>
        /// <param name="RowHeaderNodes"></param>
        /// <returns></returns>
        private int GetHeaderLength(List<OnlyHeaderNode> RowHeaderNodes)
        {
            int Result = 0;
            if (RowHeaderNodes != null && RowHeaderNodes.Count > 0)
            {
                foreach (OnlyHeaderNode Item in RowHeaderNodes)
                {
                    if (Item.IsLeafNode)
                        Result++;
                    else
                        Result = Result + this.GetHeaderLength(Item.ChildNodes);
                }
            }
            return Result;
        }
        /// <summary>
        /// 行表头table转换成list<list<>>;
        /// </summary>
        /// <param name="RowListTable"></param>
        /// <param name="Node"></param>
        /// <param name="ListRowTd"></param>
        private void GetRowListTable(List<List<MyTd>> RowListTable, SimpleHeaderNode Node, List<MyTd> ListRowTd, Dictionary<string, string> UnitTableAndAssociationTable, Dictionary<string, ReportWidgetColumn> allReportWidgetColumn)
        {
            MyTd TempTd = new MyTd();
            TempTd.ColumnCode = Node.ColumnCode;
            TempTd.RowSpan = Node.RowSpan;
            TempTd.ColSpan = Node.ColSpan;
            TempTd.Value = Node.Value;
            TempTd.Code = Node.Value;
            ColumnType ColumnType = ColumnType.String;
            if (!string.IsNullOrEmpty(Node.ColumnCode) && allReportWidgetColumn.ContainsKey(Node.ColumnCode))
                ColumnType = allReportWidgetColumn[Node.ColumnCode].ColumnType;
            if ((ColumnType == ColumnType.Association || ColumnType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(TempTd.Code))
            {
                TempTd.Value = UnitTableAndAssociationTable[TempTd.Code];
            }
            ListRowTd.Add(TempTd);
            if (Node.ChildNode == null || Node.ChildNode.Count == 0)
            {
                return;
            }
            else
            {
                int i = 0;
                foreach (SimpleHeaderNode ChildNode in Node.ChildNode)
                {
                    if (i == 0)
                    {
                        GetRowListTable(RowListTable, ChildNode, ListRowTd, UnitTableAndAssociationTable, allReportWidgetColumn);
                    }
                    else
                    {
                        List<MyTd> NewMyRowTd = new List<MyTd>();
                        RowListTable.Add(NewMyRowTd);
                        GetRowListTable(RowListTable, ChildNode, NewMyRowTd, UnitTableAndAssociationTable, allReportWidgetColumn);
                    }
                    i++;
                }
            }
        }
        /// <summary>
        /// 获取叶子节点
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="AllHeadNode"></param>
        /// <returns></returns>
        private List<HeaderNode> GetLeafNode(HeaderNode ParentNode, Dictionary<string, HeaderNode> AllHeadNode)
        {
            List<HeaderNode> LeafNode = new List<HeaderNode>();

            HeaderNode Node = ParentNode;
            if (Node.IsLeaf)
            {
                LeafNode.Add(Node);
            }
            else
            {
                Dictionary<string, string> ChildrenObjectID = Node.ChildrenValueAndObjectID;
                //在这里可以根据值排序；根据配置的排序字段
                foreach (string key in ChildrenObjectID.Keys)
                {
                    LeafNode.AddRange(GetLeafNode(AllHeadNode[ChildrenObjectID[key]], AllHeadNode));
                }
            }
            return LeafNode;
        }
        /// <summary>
        /// 汇总表获取值
        /// </summary>
        /// <param name="ComputeColumn"></param>
        /// <param name="ColumnNode"></param>
        /// <param name="RowNode"></param>
        /// <param name="AllBos"></param>
        /// <param name="RowCollect"></param>
        /// <param name="ColumnCollect"></param>
        /// <param name="RowCollectAndColumnCollect"></param>
        /// <returns></returns>
        private string GetComputedValue(
            ReportWidgetColumn ComputeColumn,
            HeaderNode ColumnNode,
            HeaderNode RowNode,
            Dictionary<string, Dictionary<string, object>> AllBos,
            Dictionary<string, myCollect> RowCollect,
            Dictionary<string, Dictionary<string, myCollect>> ColumnCollect,
            Dictionary<string, myCollect> RowCollectAndColumnCollect,
            Dictionary<string, string> UnitTableAndAssociationTable)
        {
            HashSet<string> ColumnBOIDs = new HashSet<string>();
            if (ColumnNode != null && ColumnNode.BoObjectIDs != null && ColumnNode.BoObjectIDs.Count > 0)
            {
                ColumnBOIDs = ColumnNode.BoObjectIDs;
            }

            HashSet<string> RowBOIDs = new HashSet<string>();
            if (RowNode != null && RowNode.BoObjectIDs != null && RowNode.BoObjectIDs.Count > 0)
            {
                RowBOIDs = RowNode.BoObjectIDs;
            }
            string Result = "-";
            string ComputeColumnCode = ComputeColumn.ColumnCode;
            string ColumnFormat = ComputeColumn.Format;
            OThinker.Reporting.Function FunctionType = ComputeColumn.FunctionType;
            ColumnType ColumnType = ComputeColumn.ColumnType;
            string ColumnObjectID = ColumnNode == null ? null : ColumnNode.ObjectID;
            HashSet<string> ListResult = new HashSet<string>();

            //加仅有key的dic项，保证汇总的顺序
            #region 列汇总
            if (ColumnNode != null)
            {
                if (!ColumnCollect.ContainsKey(ColumnObjectID))
                {
                    ColumnCollect.Add(ColumnObjectID, new Dictionary<string, myCollect>());
                }
                if (ComputeColumn != null && !ColumnCollect[ColumnObjectID].ContainsKey(ComputeColumnCode))
                {
                    ColumnCollect[ColumnObjectID].Add(ComputeColumnCode, new myCollect());
                    ColumnCollect[ColumnObjectID][ComputeColumnCode].FunctionType = FunctionType;
                    ColumnCollect[ColumnObjectID][ComputeColumnCode].Format = ColumnFormat;
                    ColumnCollect[ColumnObjectID][ComputeColumnCode].ColumnType = ColumnType;
                }
            }
            #endregion

            #region 行汇总
            if (RowNode != null)
            {
                if (!RowCollect.ContainsKey(ComputeColumnCode))
                {
                    RowCollect.Add(ComputeColumnCode, new myCollect());
                    RowCollect[ComputeColumnCode].FunctionType = FunctionType;
                    RowCollect[ComputeColumnCode].Format = ColumnFormat;
                    RowCollect[ComputeColumnCode].ColumnType = ColumnType;
                }
            }
            #endregion

            #region 行汇总和列汇总
            if (ComputeColumn != null && !RowCollectAndColumnCollect.ContainsKey(ComputeColumnCode))
            {
                RowCollectAndColumnCollect.Add(ComputeColumnCode, new myCollect());
                RowCollectAndColumnCollect[ComputeColumnCode].FunctionType = FunctionType;
                RowCollectAndColumnCollect[ComputeColumnCode].Format = ColumnFormat;
            }

            #endregion
            //end加仅有key的dic项，保证汇总的顺序
            //处理非数值类型的结果
            if (ColumnType == ColumnType.Numeric)
            {
                #region 数值类,分3种情况
                double Value = 0;
                int BoNum = 0;

                if (ColumnBOIDs != null && ColumnBOIDs.Count > 0 && RowBOIDs != null && RowBOIDs.Count > 0)
                {
                    foreach (string BoID in ColumnBOIDs)
                    {
                        if (RowBOIDs.Contains(BoID))
                        {
                            BoNum++;
                            string ComputeColumnValue = "";

                            #region 行汇总
                            double tempValue = 0;
                            if (ComputeColumnCode != OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                            {
                                tempValue = 0;
                                ComputeColumnValue = AllBos[BoID][ComputeColumnCode].ToString();
                                double.TryParse(ComputeColumnValue, out tempValue);
                            }
                            else
                            {
                                tempValue = 1;
                            }
                            RowCollect[ComputeColumnCode].Values.Add(tempValue);
                            #endregion

                            #region 列汇总 和 列汇总行汇总
                            if (ComputeColumnCode != OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                            {
                                ComputeColumnValue = AllBos[BoID][ComputeColumnCode].ToString();
                                tempValue = 0;
                                double.TryParse(ComputeColumnValue, out tempValue);
                            }
                            else
                            {
                                tempValue = 1;
                            }
                            ColumnCollect[ColumnObjectID][ComputeColumnCode].Values.Add(tempValue);
                            RowCollectAndColumnCollect[ComputeColumnCode].Values.Add(tempValue);
                            #endregion

                            switch (FunctionType)
                            {
                                case OThinker.Reporting.Function.Sum:
                                case OThinker.Reporting.Function.Avg:
                                    {
                                        tempValue = 0;
                                        double.TryParse(ComputeColumnValue, out tempValue);
                                        Value += tempValue;
                                    }; break;

                                case OThinker.Reporting.Function.Count:
                                    {
                                        Value++;
                                    }; break;
                                case OThinker.Reporting.Function.Max:
                                    {
                                        tempValue = 0;
                                        double.TryParse(ComputeColumnValue, out tempValue);
                                        if (BoNum == 1)
                                            Value = tempValue;
                                        if (Value < tempValue)
                                            Value = tempValue;
                                    }; break;
                                case OThinker.Reporting.Function.Min:
                                    {
                                        tempValue = 0;
                                        double.TryParse(ComputeColumnValue, out tempValue);
                                        if (BoNum == 1)
                                            Value = tempValue;
                                        if (Value > tempValue)
                                            Value = tempValue;
                                    }; break;
                                default: ; break;
                            }
                        }
                    }
                }
                else if ((ColumnBOIDs != null || ColumnBOIDs.Count > 0) && (RowBOIDs == null || RowBOIDs.Count == 0))
                {
                    foreach (string BoID in ColumnBOIDs)
                    {
                        string ComputeColumnValue = "";
                        double tempValue = 0;
                        #region 列汇总
                        if (ComputeColumnCode != OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                        {
                            ComputeColumnValue = AllBos[BoID][ComputeColumnCode].ToString();
                            tempValue = 0;
                            double.TryParse(ComputeColumnValue, out tempValue);
                        }
                        else
                        {
                            tempValue = 1;
                        }
                        ColumnCollect[ColumnObjectID][ComputeColumnCode].Values.Add(tempValue);
                        RowCollectAndColumnCollect[ComputeColumnCode].Values.Add(tempValue);
                        #endregion
                    }
                }
                if ((ColumnBOIDs == null || ColumnBOIDs.Count == 0) && (RowBOIDs != null && RowBOIDs.Count > 0))
                {
                    foreach (string BoID in RowBOIDs)
                    {
                        string ComputeColumnValue = "";

                        double tempValue = 0;
                        #region 行汇总
                        if (ComputeColumnCode != OThinker.Reporting.ReportWidgetColumn.DefaultCountCode)
                        {
                            ComputeColumnValue = AllBos[BoID][ComputeColumnCode].ToString();
                            tempValue = 0;
                            double.TryParse(ComputeColumnValue, out tempValue);
                        }
                        else
                        {
                            tempValue = 1;
                        }
                        RowCollect[ComputeColumnCode].Values.Add(tempValue);
                        RowCollectAndColumnCollect[ComputeColumnCode].Values.Add(tempValue);
                        #endregion
                    }
                }
                if (BoNum == 0)
                {
                    return Result;
                }
                if (ComputeColumn.FunctionType == OThinker.Reporting.Function.Avg && BoNum != 0)
                    Value = Value / double.Parse(BoNum.ToString());
                //处理数字格式
                Result = GetFormatValue(Value, ColumnFormat);
                #endregion
            }
            else
            {
                #region 非数值类,非数值类不计算汇总；
                if (ColumnBOIDs != null && ColumnBOIDs.Count > 0 && RowBOIDs != null && RowBOIDs.Count > 0)
                {
                    foreach (string BoID in ColumnBOIDs)
                    {
                        if (RowBOIDs.Contains(BoID))
                        {
                            string tempresult = AllBos[BoID][ComputeColumnCode].ToString();
                            if (ColumnType == ColumnType.Unit || ColumnType == ColumnType.Association)
                            {
                                if (UnitTableAndAssociationTable.ContainsKey(tempresult))
                                    tempresult = UnitTableAndAssociationTable[tempresult];
                            }
                            ListResult.Add(tempresult);
                        }
                    }
                }
                if (ListResult.Count > 0)
                {
                    Result = string.Join(";", ListResult);
                }
                else
                {
                    Result = "-";
                }
                #endregion
            }
            return Result;
        }
        /// <summary>
        /// 解析显示规则
        /// </summary>
        /// <param name="OrgValue"></param>
        /// <param name="Format"></param>
        /// <returns></returns>
        private string GetFormatValue(double OrgValue, string Format)
        {
            string Result = "";
            Result = OrgValue.ToString();
            if (string.IsNullOrEmpty(Format))
            {
                return Result;
            }
            string FormatDotPart = "";
            //小数位
            if (Format.IndexOf("#") > -1)
            {

                int start = Format.IndexOf("#");
                int end = Format.LastIndexOf("#");
                FormatDotPart = Format.Substring(start, end - start + 1);
                if (FormatDotPart.IndexOf('.') > -1)
                {
                    FormatDotPart = "F" + FormatDotPart.Split('.')[1].Length.ToString();
                }
                Result = double.Parse(Result).ToString(FormatDotPart);
                if (Result == "")
                    Result = "0";
            }
            //百分号
            if (Format.IndexOf('%') > -1)
            {

                Result = (double.Parse(Result) * 100).ToString(FormatDotPart) + "%";
            }
            //千分符
            if (Format.IndexOf(',') > -1)
            {
                string Dotlength = "0";
                if (Result.IndexOf('.') > -1)
                    Dotlength = Result.Split('.')[1].Length.ToString();
                Result = string.Format("{0:N" + Dotlength + "}", double.Parse(Result));
            }
            return Result;
        }
        /// <summary>
        ///多个值，一个系列汇总
        /// </summary>
        /// <param name="SeriesBoIDsItem"></param>
        /// <param name="CategorisBoIDsItem"></param>
        /// <param name="AllBos"></param>
        /// <param name="ComputeColumn"></param>
        /// <returns></returns>
        private double ChartsGetComputedValue(HashSet<string> SeriesBoIDsItem, HashSet<string> CategorisBoIDsItem, Dictionary<string, Dictionary<string, object>> AllBos, ReportWidgetColumn ComputeColumn)
        {
            Function FunctionType = ComputeColumn.FunctionType;
            double Value = 0;
            string ComputeColumnCode = ComputeColumn.ColumnCode;
            int BoNum = 0;
            string ColumnFormat = ComputeColumn.Format;
            ColumnType ColumnType = ComputeColumn.ColumnType;
            if (ColumnType == ColumnType.Numeric)
            {
                #region 数值类
                if (SeriesBoIDsItem != null && SeriesBoIDsItem.Count > 0 && CategorisBoIDsItem != null && CategorisBoIDsItem.Count > 0)
                {
                    foreach (string SeriesBoID in SeriesBoIDsItem)
                    {
                        if (CategorisBoIDsItem.Contains(SeriesBoID))
                        {
                            BoNum++;
                            string ComputeColumnValue = "";// AllBos[SeriesBoID][ComputeColumnCode].ToString();
                            if (ComputeColumnCode != ReportWidgetColumn.DefaultCountCode)
                            {
                                ComputeColumnValue = AllBos[SeriesBoID][ComputeColumnCode].ToString();
                            }
                            switch (FunctionType)
                            {
                                case OThinker.Reporting.Function.Sum:
                                case OThinker.Reporting.Function.Avg:
                                    {
                                        double tempValue = 0;
                                        double.TryParse(ComputeColumnValue, out tempValue);
                                        Value += tempValue;
                                    }; break;
                                case OThinker.Reporting.Function.Count:
                                    {
                                        Value++;
                                    }; break;
                                case OThinker.Reporting.Function.Max:
                                    {
                                        double tempValue = 0;
                                        double.TryParse(ComputeColumnValue, out tempValue);
                                        if (BoNum == 1)
                                            Value = tempValue;
                                        if (Value < tempValue)
                                            Value = tempValue;
                                    }; break;
                                case OThinker.Reporting.Function.Min:
                                    {
                                        double tempValue = 0;
                                        double.TryParse(ComputeColumnValue, out tempValue);
                                        if (BoNum == 1)
                                            Value = tempValue;
                                        if (Value > tempValue)
                                        {
                                            Value = tempValue;
                                        }
                                    }; break;
                                default: ; break;
                            }
                        }
                    }
                }
                else if ((SeriesBoIDsItem == null || SeriesBoIDsItem.Count == 0) && CategorisBoIDsItem != null && CategorisBoIDsItem.Count > 0)
                {
                    foreach (string CategorisBoID in CategorisBoIDsItem)
                    {
                        BoNum++;
                        string ComputeColumnValue = "";// AllBos[SeriesBoID][ComputeColumnCode].ToString();
                        if (ComputeColumnCode != ReportWidgetColumn.DefaultCountCode)
                        {
                            ComputeColumnValue = AllBos[CategorisBoID][ComputeColumnCode].ToString();
                        }
                        switch (FunctionType)
                        {
                            case OThinker.Reporting.Function.Sum:
                            case OThinker.Reporting.Function.Avg:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    Value += tempValue;
                                }; break;
                            case OThinker.Reporting.Function.Count:
                                {
                                    Value++;
                                }; break;
                            case OThinker.Reporting.Function.Max:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    if (BoNum == 1)
                                        Value = tempValue;
                                    if (Value < tempValue)
                                        Value = tempValue;
                                }; break;
                            case OThinker.Reporting.Function.Min:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    if (BoNum == 1)
                                        Value = tempValue;
                                    if (Value > tempValue)
                                    {
                                        Value = tempValue;
                                    }
                                }; break;
                            default: ; break;
                        }
                    }
                }
                else if (SeriesBoIDsItem != null && SeriesBoIDsItem.Count > 0 && (CategorisBoIDsItem == null || CategorisBoIDsItem.Count == 0))
                {
                    foreach (string SeriesBoID in SeriesBoIDsItem)
                    {
                        BoNum++;
                        string ComputeColumnValue = "";// AllBos[SeriesBoID][ComputeColumnCode].ToString();
                        if (ComputeColumnCode != ReportWidgetColumn.DefaultCountCode)
                        {
                            ComputeColumnValue = AllBos[SeriesBoID][ComputeColumnCode].ToString();
                        }
                        switch (FunctionType)
                        {
                            case OThinker.Reporting.Function.Sum:
                            case OThinker.Reporting.Function.Avg:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    Value += tempValue;
                                }; break;
                            case OThinker.Reporting.Function.Count:
                                {
                                    Value++;
                                }; break;
                            case OThinker.Reporting.Function.Max:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    if (BoNum == 1)
                                        Value = tempValue;
                                    if (Value < tempValue)
                                        Value = tempValue;
                                }; break;
                            case OThinker.Reporting.Function.Min:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    if (BoNum == 1)
                                        Value = tempValue;
                                    if (Value > tempValue)
                                    {
                                        Value = tempValue;
                                    }
                                }; break;
                            default: ; break;
                        }
                    }
                }
                if (ComputeColumn.FunctionType == OThinker.Reporting.Function.Avg && BoNum != 0)
                    Value = Value / double.Parse(BoNum.ToString());
                //Result = GetFormatValue(Value, ColumnFormat);
                #endregion
            }
            else
            {
                //图标类报表，非数值类，默认为0
                if (SeriesBoIDsItem != null && SeriesBoIDsItem.Count > 0 && CategorisBoIDsItem != null && CategorisBoIDsItem.Count > 0)
                {
                    foreach (string SeriesBoID in SeriesBoIDsItem)
                    {
                        if (CategorisBoIDsItem.Contains(SeriesBoID))
                        {
                            BoNum++;
                        }
                    }
                }
                else if ((SeriesBoIDsItem == null || SeriesBoIDsItem.Count == 0) && CategorisBoIDsItem != null && CategorisBoIDsItem.Count > 0)
                {
                    foreach (string CategorisBoID in CategorisBoIDsItem)
                    {
                        BoNum++;
                    }
                }
                else if (SeriesBoIDsItem != null && SeriesBoIDsItem.Count > 0 && (CategorisBoIDsItem == null || CategorisBoIDsItem.Count == 0))
                {
                    foreach (string SeriesBoID in SeriesBoIDsItem)
                    {
                        BoNum++;
                    }
                }
                //Result = "0";
                Value = BoNum;
            }

            return Value;
        }
        /// <summary>
        /// 仅用于多个值，一个系列，无分类的情况
        /// </summary>
        /// <param name="SourceData"></param>
        /// <param name="Result"></param>
        /// <param name="Columns"></param>
        private void GetSimpleCollectValue(List<Dictionary<string, object>> SourceData, ChartsDataResult Result, ReportWidgetColumn[] Columns, ReportWidget Setting)
        {
            Result.Categories = new List<CategoriesForCharts>();
            Result.Categories.Add(new CategoriesForCharts() { key = Columns[0].DisplayName, value = Columns[0].DisplayName });
            Dictionary<SeriesForCharts, double> sortDic = new Dictionary<SeriesForCharts, double>();
            foreach (ReportWidgetColumn Column in Columns)
            {
                SeriesForCharts Series = new SeriesForCharts();
                Series.Name = Column.DisplayName;
                Series.Code = Column.DisplayName;
                Function FunctionType = Column.FunctionType;
                string ColumnFormat = Column.Format;
                ColumnType ColumnType = Column.ColumnType;
                double ResultDouble = 0;
                if (ColumnType == ColumnType.Numeric)
                {
                    double Value = 0;
                    int BoNum = 0;
                    foreach (Dictionary<string, object> Bo in SourceData)
                    {
                        string ComputeColumnValue = "";// AllBos[SeriesBoID][ComputeColumnCode].ToString();
                        if (Column.ColumnCode != ReportWidgetColumn.DefaultCountCode)
                        {
                            ComputeColumnValue = Bo[Column.ColumnCode] + string.Empty;
                        }
                        switch (FunctionType)
                        {
                            case OThinker.Reporting.Function.Sum:
                            case OThinker.Reporting.Function.Avg:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    Value += tempValue;
                                }; break;

                            case OThinker.Reporting.Function.Count:
                                {
                                    Value++;
                                }; break;
                            case OThinker.Reporting.Function.Max:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    if (BoNum == 1)
                                        Value = tempValue;
                                    if (Value < tempValue)
                                        Value = tempValue;
                                }; break;
                            case OThinker.Reporting.Function.Min:
                                {
                                    double tempValue = 0;
                                    double.TryParse(ComputeColumnValue, out tempValue);
                                    if (BoNum == 1)
                                        Value = tempValue;
                                    if (Value > tempValue)
                                        Value = tempValue;
                                }; break;
                            default: ; break;
                        }
                    }
                    if (BoNum == 0)
                    {
                        Series.Data.Add(0);
                        Result.Series.Add(Series);
                    }
                    if (FunctionType == OThinker.Reporting.Function.Avg)
                        Value = Value / double.Parse(BoNum.ToString());
                    // ResultString = GetFormatValue(Value, ColumnFormat);
                    ResultDouble = Value;
                }
                else
                {
                    double Value = 0;
                    foreach (Dictionary<string, object> Bo in SourceData)
                    {
                        Value++;
                    }
                    //  ResultString = "0";
                    ResultDouble = Value;
                }
                Series.Data.Add(ResultDouble);
                sortDic.Add(Series, ResultDouble);
                Result.Series.Add(Series);
            }
            //记录一个排序
            if (Setting.SortColumns != null && Setting.SortColumns.Length == 1 && Setting.SortColumns[0].ColumnType == OThinker.Reporting.ColumnType.Numeric)
            {
                if (Setting.SortColumns[0].Ascending)
                {
                    sortDic = sortDic.OrderBy(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                }
                else
                {
                    sortDic = sortDic.OrderByDescending(p => p.Value).ToDictionary(p => p.Key, o => o.Value);
                }
                Result.Series = sortDic.Keys.ToList();
            }
        }
        /// <summary>
        /// 获取汇总值
        /// </summary>
        /// <param name="MyCollect"></param>
        /// <returns></returns>
        private string GetCollectValue(myCollect MyCollect)
        {
            string ColumnFormat = MyCollect.Format;
            OThinker.Reporting.Function FunctionType = MyCollect.FunctionType;
            double Value = 0;
            string Result = "-";
            int BoNum = 0;
            ColumnType ColumnType = MyCollect.ColumnType;
            if (ColumnType == ColumnType.Numeric)
            {
                foreach (double Num in MyCollect.Values)
                {
                    BoNum++;
                    switch (FunctionType)
                    {
                        case OThinker.Reporting.Function.Sum:
                        case OThinker.Reporting.Function.Avg:
                            {
                                Value += Num;
                            }; break;

                        case OThinker.Reporting.Function.Count:
                            {
                                Value++;
                            }; break;
                        case OThinker.Reporting.Function.Max:
                            {
                                if (BoNum == 1)
                                    Value = Num;
                                if (Value < Num)
                                    Value = Num;
                            }; break;
                        case OThinker.Reporting.Function.Min:
                            {
                                if (BoNum == 1)
                                    Value = Num;
                                if (Value > Num)
                                    Value = Num;
                            }; break;
                        default: ; break;
                    }
                }
                if (FunctionType == OThinker.Reporting.Function.Avg && BoNum != 0)
                    Value = Value / double.Parse(BoNum.ToString());
                Result = GetFormatValue(Value, ColumnFormat);
            }
            else
            {
                Result = "-";
            }
            return Result;
        }
        /// <summary>
        /// 获取colspan
        /// </summary>
        /// <param name="ParentNode"></param>
        /// <param name="AllHeaderNodes"></param>
        /// <param name="ColumnLength"></param>
        /// <param name="MyTread"></param>
        /// <returns></returns>
        private MyTd GetColSpan(HeaderNode ParentNode, Dictionary<string, HeaderNode> AllHeaderNodes, int ColumnLength, Dictionary<string, List<MyTd>> MyTread, Dictionary<string, string> UnitTableAndAssociationTable, Dictionary<string, ReportWidgetColumn> allReportWidgetColumn)
        {
            if (ColumnLength == 0)
                return null;
            HeaderNode Node = ParentNode;
            int SpanLength = 0;
            MyTd Result = new MyTd();
            Result.RowSpan = 1;
            Result.Value = Node.HeaderValue;
            Result.Code = Node.HeaderValue;
            if (allReportWidgetColumn.ContainsKey(Node.ColumnCode) && (allReportWidgetColumn[Node.ColumnCode].ColumnType == ColumnType.Association || allReportWidgetColumn[Node.ColumnCode].ColumnType == ColumnType.Unit) && UnitTableAndAssociationTable.Count > 0)
            {
                Result.Value = UnitTableAndAssociationTable[Node.HeaderValue];
            }
            if (Node.IsLeaf)
            {
                SpanLength = ColumnLength;
            }
            else
            {
                if (Node.ChildrenValueAndObjectID != null && Node.ChildrenValueAndObjectID.Count > 0)
                {
                    foreach (string ChildrenValue in Node.ChildrenValueAndObjectID.Keys)
                    {
                        string ChildrenObjectID = Node.ChildrenValueAndObjectID[ChildrenValue];
                        SpanLength += GetColSpan(AllHeaderNodes[ChildrenObjectID], AllHeaderNodes, ColumnLength, MyTread, UnitTableAndAssociationTable, allReportWidgetColumn).ColSpan;
                    }
                }
            }
            Result.ColSpan = SpanLength;
            MyTread[Node.ColumnCode].Add(Result);
            return Result;
        }
        /// <summary>
        /// 获取整理后的行表头
        /// </summary>
        /// <param name="Node"></param>
        /// <param name="AllRowHeaderNode"></param>
        /// <returns></returns>
        private SimpleHeaderNode GetSimpleHeaderNode(HeaderNode Node, Dictionary<string, HeaderNode> AllRowHeaderNode, bool SpecialAction, int length)
        {
            SimpleHeaderNode Result = new SimpleHeaderNode();
            Result.ChildNode = new List<SimpleHeaderNode>();
            if (SpecialAction)
                Result.ColSpan = length;
            else
                Result.ColSpan = 1;
            Result.ColumnCode = Node.ColumnCode;
            Result.Value = Node.HeaderValue;
            int RowSpan = 0;
            if (Node.IsLeaf)
            {
                RowSpan = 1;
            }
            else
            {
                foreach (string ChildValue in Node.ChildrenValueAndObjectID.Keys)
                {
                    SimpleHeaderNode ChildNode = GetSimpleHeaderNode(AllRowHeaderNode[Node.ChildrenValueAndObjectID[ChildValue]], AllRowHeaderNode, false, length);
                    RowSpan += ChildNode.RowSpan;
                    Result.ChildNode.Add(ChildNode);
                }
            }
            Result.RowSpan = RowSpan;
            return Result;
        }
        /// <summary>
        /// 明细表
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadGridData()
        {
            string WidgetID = this.Request["WidgetID"];
            string Code = this.Request["Code"];
            string cha = "\";\"";//剔除选人控件初始值为空时，查询条件应该默认为空
            string FilterDataJson = null;
            string DataJson = this.Request["FilterData"];
            if (DataJson != null)
            {
                FilterDataJson = DataJson.Replace(cha, "");
            }
            ReportPage reportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(Code));
            if (reportPage.ReportWidgets == null) { return Json(new { State = false, Msg = "报表部件不存在" }); }
            ReportWidget Setting = reportPage.ReportWidgets.FirstOrDefault(p => p.ObjectID == WidgetID);
            FunctionNode reportNode = this.UserValidator.GetFunctionNode(reportPage.Code);
            if (Setting == null)
            {
                return Json(new { State = false, Msg = "报表页与数据源不匹配" });
            }

            ReportSource ReportSource = null;
            if (reportPage != null && reportPage.ReportSources != null && reportPage.ReportSources.Length > 0)
            {
                foreach (ReportSource q in reportPage.ReportSources)
                {
                    if (q.ObjectID == Setting.ReportSourceId)
                        ReportSource = q;
                }
            }
            if (Setting.WidgetType != WidgetType.Detail)
            {
                return Json(new { State = false, Msg = "参数错误" });
            }

            int PageSize = 10;
            int BeginNum = 1;
            if (!int.TryParse(this.Request.Form["length"] ?? "", out PageSize))
            {
                PageSize = 50;
            }
            int Count;
            if (!int.TryParse(this.Request.Form["start"] ?? "", out BeginNum))
            {
                BeginNum = 1;
            }
            if (BeginNum > 1)
            {
                PageSize = PageSize - 1;
            }
            string orderby = this.Request["orderby"];
            bool Ascending = !string.IsNullOrEmpty(this.Request["dir"]) && this.Request["dir"] != "asc" ? false : true;
            if (!string.IsNullOrEmpty(orderby))
            {
                //if (orderby.Split('_').Length > 1) {
                //    orderby = orderby.Split('_')[1];
                //}
                ReportWidgetColumn OrderbyColumn = new ReportWidgetColumn();
                OrderbyColumn.ColumnCode = orderby;
                OrderbyColumn.ColumnName = orderby;
                OrderbyColumn.Ascending = Ascending;
                Setting.SortColumns = new ReportWidgetColumn[1] { OrderbyColumn };
            }
            //排除系列和分类里的defautcode
            this.RemoveDefautcodeFromSC(Setting);
            Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
            //只要是关联查询，不管报表页中是否选择了子表的ObjectID ，都应该加入该列，这样做是为了解决明细报表总行数和事件数据不同步的问题

            List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage.Filters, Setting, ReportSource, out ChildCodes);
            //根据数据库类型，给函数重新赋值 
            for (int i = 0; i < SourceColumns.Count; i++)
            {
                if (SourceColumns[i].Formula != null)
                {
                    //如果是mysql
                    if (this.Engine.EngineConfig.DBType == OThinker.Data.Database.DatabaseType.MySql)
                    {
                        //获取当前日期格式：yyyymmdd hh:mm:ss
                        if (SourceColumns[i].Formula.IndexOf("NOW()") > -1)
                        {
                            string formula = "";
                            formula = SourceColumns[i].Formula.Replace("NOW()", "now()");
                            SourceColumns[i].Formula = formula;
                        }
                        //获取当前日期格式：yyyymmdd
                        else if (SourceColumns[i].Formula.IndexOf("TODAY()") > -1)
                        {
                            string formula = "";
                            formula = SourceColumns[i].Formula.Replace("TODAY()", "date()");
                            SourceColumns[i].Formula = formula;
                        }
                    }
                    //如果是sqlservers
                    else if (this.Engine.EngineConfig.DBType == OThinker.Data.Database.DatabaseType.SqlServer)
                    {
                        //获取当前日期格式：yyyymmdd hh:mm:ss
                        if (SourceColumns[i].Formula.IndexOf("NOW()") > -1)
                        {
                            string formula = "";
                            formula = SourceColumns[i].Formula.Replace("NOW()", "getdate()");
                            SourceColumns[i].Formula = formula;


                        }
                        //获取当前日期格式：yyyymmdd
                        else if (SourceColumns[i].Formula.IndexOf("TODAY()") > -1)
                        {
                            string formula = "";
                            formula = SourceColumns[i].Formula.Replace("TODAY()", "CONVERT(varchar(100),getdate(),23)");
                            SourceColumns[i].Formula = formula;

                        }
                    }   //如果是Oracle
                    else if (this.Engine.EngineConfig.DBType == OThinker.Data.Database.DatabaseType.Oracle)
                    {

                    }
                }
            }

            //联动
            string UnitFilterDataJson = this.Request["UnitFilterDataJson"];
            bool IsUnitFilter = false;
            if (!string.IsNullOrEmpty(UnitFilterDataJson))
            {
                IsUnitFilter = true;
                this.DoUnit(UnitFilterDataJson, SourceColumns, reportPage, Setting, ReportSource);
            }
            //end联动


            //重新检查列
            List<string> Msgs = new List<string>();
            if (!this.ReCheckColumns(Setting, SourceColumns, out Msgs))
            {
                return Json(new { State = false, Msg = string.Join(";", Msgs) });
            }

            //处理过滤条件
            if (!string.IsNullOrEmpty(FilterDataJson))
            {
                SetFilterValue(reportPage, FilterDataJson);
                ////动态赋值过滤条件，已拼接查询函数作为条件。
                //foreach (var p in reportPage.Filters) { 

                //}
            }


            List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
            List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
            Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
            List<string> Conditions = new List<string>();
            List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
            string reportNodeCode = reportNode == null ? "" : reportNode.Code;
            string tablename = Setting.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportNodeCode, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
            List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
                tablename,
                SourceColumns.ToArray(),
                Setting.SortColumns,
                reportPage.Filters,
                out statisticalColumn,
                out UnitTableAndAssociationTable,
                out TypeChangedColumns,
                BeginNum,
                BeginNum + PageSize,
                out Count,
                Conditions,
                Parameters,
                ReportSource,
                true,
                WidgetType.Detail,
                ChildCodes.Count > 0,
                this.GetMainObjectId(ReportSource.SchemaCode),
                IsUnitFilter); // IsUnitFilter 判断过滤是否来自联动报表
            HashSet<string> Columns = new HashSet<string>();
            List<ReportWidgetColumn> ResultSourceColumns = new List<ReportWidgetColumn>();
            bool Numeric = false;
            Dictionary<string, ReportWidgetColumn> NumericCode = new Dictionary<string, ReportWidgetColumn>();
            foreach (ReportWidgetColumn q in SourceColumns)
            {
                if (!Columns.Contains(q.ColumnCode))
                {
                    Columns.Add(q.ColumnCode);
                }
                if (q.ColumnType == ColumnType.Numeric && string.IsNullOrEmpty(q.Formula))
                {
                    NumericCode.Add(q.ColumnCode, q);
                    Numeric = true;
                }
            }
            if (TypeChangedColumns != null && TypeChangedColumns.Count > 0)
            {
                foreach (ReportWidgetColumn q in TypeChangedColumns)
                {
                    if (q.ColumnType == ColumnType.Numeric)
                    {
                        NumericCode.Add(q.ColumnCode, q);
                        Numeric = true;
                    }
                }
            }
            foreach (ReportWidgetColumn q in Setting.Columns)
            {
                if (Columns.Contains(q.ColumnCode))
                    ResultSourceColumns.Add(q);
            }
            Dictionary<string, Dictionary<string, object>> dicSourceData = new Dictionary<string, Dictionary<string, object>>();
            string MainObjectIdCode = "";
            if (ReportSource.ReportSourceAssociations == null)
            {
                MainObjectIdCode = this.GetMainObjectId_Name(ReportSource.SchemaCode);
            }
            else
            {
                //目前表单中选择关联的子表只能选择一个关联子表，所以循环也没有作用
                if (ReportSource.ReportSourceAssociations.Length > 0)
                {
                    //foreach (var q in ReportSource.ReportSourceAssociations) {

                    //}
                    string code = ReportSource.ReportSourceAssociations[0].SchemaCode.Substring(ReportSource.ReportSourceAssociations[0].SchemaCode.IndexOf("___") + 3);
                    MainObjectIdCode = GetSonObjectID_Name(code);
                }

            }


            if (ChildCodes != null && ChildCodes.Count > 0)
            {
                //处理主表和子表的关系;
                int counter = 1;
                string lastobjectid = "";
                Dictionary<string, List<string>> collecion = new Dictionary<string, List<string>>();
                foreach (Dictionary<string, object> q in SourceData)
                {
                    string ObjectId = "";
                    //如果明细表中拖拽了子表的ObjectID查询数据,怎关联字段没有子表的ＩＤ否则以主表的ID为查询主ID
                    if (q.ContainsKey(MainObjectIdCode))
                    {
                        ObjectId = q[MainObjectIdCode].ToString();
                    }
                    else
                    {
                        ObjectId = q[this.GetMainObjectId_Name(ReportSource.SchemaCode)].ToString();
                    }//如果不是明细表关联查询,则更加objectid去重复
                    if (ReportSource.ReportSourceAssociations == null && ReportSource.ReportSourceAssociations.Length == 0)
                    {
                        if (!dicSourceData.ContainsKey(ObjectId))
                        {
                            dicSourceData.Add(ObjectId, new Dictionary<string, object>());

                            dicSourceData[ObjectId].Add(DetailRowNumber, counter + BeginNum);
                            lastobjectid = ObjectId;
                            counter++;
                        }
                    }
                    else
                    {
                        if (ObjectId == "")
                        {
                            ObjectId = Guid.NewGuid() + string.Empty;
                        }
                        if (!dicSourceData.ContainsKey(ObjectId))
                        {
                            dicSourceData.Add(ObjectId, new Dictionary<string, object>());

                            dicSourceData[ObjectId].Add(DetailRowNumber, counter + BeginNum);
                            lastobjectid = ObjectId;
                            counter++;
                        }
                    }
                    foreach (string ColumnCode in ChildCodes.Keys)
                    {
                        ColumnSummary ColumnSummary = ChildCodes[ColumnCode];
                        if (!dicSourceData[ObjectId].ContainsKey(ColumnCode) && q.ContainsKey(ColumnCode))
                        {
                            dicSourceData[ObjectId].Add(ColumnCode, q[ColumnCode]);


                        }
                        if (ColumnSummary.ChildeColumnSummary != null && ColumnSummary.ChildeColumnSummary.Count > 0)
                        {
                            foreach (string ChildColumnCode in ColumnSummary.ChildeColumnSummary.Keys)
                            {
                                ColumnSummary ChildColumnSummary = ColumnSummary.ChildeColumnSummary[ChildColumnCode];
                                if (dicSourceData[ObjectId].ContainsKey(ChildColumnCode))
                                {
                                    List<string> ChildResult = (List<string>)dicSourceData[ObjectId][ChildColumnCode];
                                    ChildResult.Add(q[ChildColumnCode].ToString());
                                    dicSourceData[ObjectId][ChildColumnCode] = ChildResult;
                                }
                                else
                                {
                                    dicSourceData[ObjectId].Add(ChildColumnCode, new List<string>());
                                    List<string> ChildResult = new List<string>();
                                    ChildResult.Add(q[ChildColumnCode].ToString());
                                    dicSourceData[ObjectId][ChildColumnCode] = ChildResult;
                                }

                            }
                        }

                    }
                    ////汇总

                    if (!dicSourceData[ObjectId].ContainsKey(DataModel.BizObjectSchema.PropertyName_ObjectID))
                    {
                        dicSourceData[ObjectId].Add(DataModel.BizObjectSchema.PropertyName_ObjectID, new { ObjectId = ObjectId, SchemaCode = ReportSource.SchemaCode });
                    }

                }

                if (Numeric && dicSourceData.ContainsKey(lastobjectid) && statisticalColumn.Count > 0)
                    dicSourceData[lastobjectid][DetailRowNumber] = "汇总";
                SourceData = dicSourceData.Values.ToList<Dictionary<string, object>>();
            }
            else
            {
                int counter = 1;
                foreach (Dictionary<string, object> q in SourceData)
                {
                    if (q.ContainsKey(MainObjectIdCode) && !string.IsNullOrEmpty(q[MainObjectIdCode].ToString()))
                        q.Add(DataModel.BizObjectSchema.PropertyName_ObjectID, new { ObjectId = q[MainObjectIdCode], SchemaCode = ReportSource.SchemaCode });

                    if (counter == SourceData.Count && Numeric)
                        q.Add(DetailRowNumber, "汇总");
                    else
                    {
                        q.Add(DetailRowNumber, counter + BeginNum);
                    }
                    counter++;
                }
            }
            return Json(new { aaData = SourceData, SourceColumns = ResultSourceColumns, iTotalRecords = Count, iTotalDisplayRecords = Count, ChildCodes = ChildCodes, Numeric = Numeric }, JsonRequestBehavior.AllowGet);
        }
        private string GetDetailCollect(List<string> values, ReportWidgetColumn Column)
        {
            OThinker.Reporting.Function FunctionType = Column.FunctionType;
            int count = 0;
            string ColumnFormat = Column.Format;
            double Value = 0;
            string Result = "-";
            foreach (string q in values)
            {
                count++;
                double Num = 0;
                double.TryParse(q, out Num);
                switch (FunctionType)
                {
                    case OThinker.Reporting.Function.Sum:
                    case OThinker.Reporting.Function.Avg:
                        {
                            Value += Num;
                        }; break;

                    case OThinker.Reporting.Function.Count:
                        {
                            Value++;
                        }; break;
                    case OThinker.Reporting.Function.Max:
                        {
                            if (Value < Num)
                                Value = Num;
                        }; break;
                    case OThinker.Reporting.Function.Min:
                        {
                            if (Value > Num)
                            {
                                Value = Num;
                            }
                            else
                            {
                                if (count == 1)
                                {
                                    Value = Num;
                                }
                            }
                        }; break;
                    default: ; break;
                }
            }
            if (FunctionType == OThinker.Reporting.Function.Avg)
            {
                if (count > 0)
                {
                    Value = Value / count;
                }
            }
            Result = GetFormatValue(Value, ColumnFormat);
            return Result;
        }
        /// <summary>
        /// 获取设计时明细表
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadDesignGridData()
        {
            try
            {
                string ReportPageJson = this.Request["ReportPage"];
                string widgetId = this.Request["ObjectId"];
                string cha = "\";\"";//剔除选人控件初始值为空时，查询条件应该默认为空
                string FilterDataJson = null;
                string DataJson = this.Request["FilterData"];
                if (DataJson != null)
                {
                    FilterDataJson = DataJson.Replace(cha, "");
                }
                ReportPage reportPage = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportPage>(ReportPageJson);
                if (reportPage == null || reportPage.ReportWidgets.Length == 0)
                {
                    return Json(new { State = false, Msg = "参数错误" });
                }
                ReportWidget reportWidget = null;
                for (int i = 0, len = reportPage.ReportWidgets.Length; i < len; i++)
                {
                    if (reportPage.ReportWidgets[i].ObjectID == widgetId)
                    {
                        reportWidget = reportPage.ReportWidgets[i];
                        break;
                    }
                }
                ReportSource ReportSource = null;
                for (int i = 0, len = reportPage.ReportSources.Length; i < len; i++)
                {
                    if (reportPage.ReportSources[i].ObjectID == reportWidget.ReportSourceId)
                    {
                        ReportSource = reportPage.ReportSources[i];
                    }
                }
                int PageSize = 10;
                int BeginNum = 1;
                if (!int.TryParse(this.Request["limit"] ?? "", out PageSize))
                {
                    PageSize = 10;
                }
                int Count;
                if (!int.TryParse(this.Request["offset"] ?? "", out BeginNum))
                {
                    BeginNum = 1;
                }
                if (BeginNum > 1)
                {

                    //PageSize = PageSize - 1;
                }
                //设计报表状态下为0
                BeginNum = 0;
                //排除系列和分类里的defautcode
                this.RemoveDefautcodeFromSC(reportWidget);
                //联动
                string UnitFilterDataJson = this.Request["UnitFilterDataJson"];
                if (!string.IsNullOrEmpty(UnitFilterDataJson))
                {
                    ReportFilter[] UnitFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportFilter[]>(UnitFilterDataJson);
                    if (reportPage.Filters != null && UnitFilters != null)
                    {
                        List<ReportFilter> listFilters = reportPage.Filters.ToList();
                        listFilters.AddRange(UnitFilters);
                        reportPage.Filters = listFilters.ToArray();
                    }
                    else if (UnitFilters != null)
                    {
                        List<ReportFilter> listFilters = new List<ReportFilter>();
                        listFilters.AddRange(UnitFilters);
                        reportPage.Filters = listFilters.ToArray();
                    }
                }
                //end联动
                FunctionNode reportNode = this.UserValidator.GetFunctionNode(reportPage.Code);

                Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
                List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage.Filters, reportWidget, ReportSource, out ChildCodes);
                //重新检查列
                List<string> Msgs = new List<string>();
                if (!this.ReCheckColumns(reportWidget, SourceColumns, out Msgs))
                {
                    return Json(new { State = false, Msg = string.Join(";", Msgs) });
                }
                Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
                List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
                List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
                List<string> Conditions = new List<string>();
                List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
                string tablename = reportWidget.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportPage.Code, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
                List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
                    tablename,
                    SourceColumns.ToArray(),
                    reportWidget.SortColumns,
                    reportPage.Filters,
                    out statisticalColumn,
                    out UnitTableAndAssociationTable,
                    out TypeChangedColumns,
                    BeginNum,
                    BeginNum + PageSize,
                    out Count,
                    Conditions,
                    Parameters,
                    ReportSource,
                    false,
                    WidgetType.Detail);
                return Json(new { State = true, rows = SourceData, total = Count }, JsonRequestBehavior.AllowGet);

            }
            catch (Exception e)
            {
                return Json(new { State = false, Msg = "参数错误" });
            }
        }
        /// <summary>
        /// 自定义报表
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadCustomGridData()
        {
            try
            {
                string ReportWidgetJson = this.Request["ReportWidgetStr"];
                ReportWidget reportWidget = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportWidget>(ReportWidgetJson);
                ReportSource ReportSource = this.Engine.ReportManager.GetReportSource(reportWidget.ReportSourceId, reportWidget.ParentObjectID);
                if (reportWidget == null)
                {
                    return Json(new { State = false, Msg = "参数错误" });
                }
                int PageSize = 10;
                int BeginNum = 1;
                if (!int.TryParse(this.Request["limit"] ?? "", out PageSize))
                {
                    PageSize = 10;
                }
                int Count;
                if (!int.TryParse(this.Request["offset"] ?? "", out BeginNum))
                {
                    BeginNum = 1;
                }
                if (BeginNum > 1)
                {

                    PageSize = PageSize - 1;
                }
                string orderby = this.Request["orderby"];
                bool Ascending = !string.IsNullOrEmpty(this.Request["dir"]) && this.Request["dir"] != "asc" ? false : true;

                if (!string.IsNullOrEmpty(orderby))
                {
                    ReportWidgetColumn OrderbyColumn = new ReportWidgetColumn();
                    OrderbyColumn.ColumnCode = orderby;
                    OrderbyColumn.Ascending = Ascending;
                    if (reportWidget.SortColumns != null)
                    {
                        List<ReportWidgetColumn> templist = reportWidget.SortColumns.ToList();
                        templist.Add(OrderbyColumn);
                        reportWidget.SortColumns = templist.ToArray();
                    }
                    else
                    {
                        reportWidget.SortColumns = new ReportWidgetColumn[1] { OrderbyColumn };
                    }
                }

                ReportPage reportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(reportWidget.ParentObjectID));
                //排除系列和分类里的defautcode
                this.RemoveDefautcodeFromSC(reportWidget);
                if (reportPage == null)
                {
                    return Json(new { State = false, Msg = "参数错误" });
                }
                //联动
                string UnitFilterDataJson = this.Request["UnitFilterDataJson"];
                if (!string.IsNullOrEmpty(UnitFilterDataJson))
                {
                    ReportFilter[] UnitFilters = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportFilter[]>(UnitFilterDataJson);
                    if (reportPage.Filters != null && UnitFilters != null)
                    {
                        List<ReportFilter> listFilters = reportPage.Filters.ToList();
                        listFilters.AddRange(UnitFilters);
                        reportPage.Filters = listFilters.ToArray();
                    }
                    else if (UnitFilters != null)
                    {
                        List<ReportFilter> listFilters = new List<ReportFilter>();
                        listFilters.AddRange(UnitFilters);
                        reportPage.Filters = listFilters.ToArray();
                    }
                }
                //end联动
                FunctionNode reportNode = this.UserValidator.GetFunctionNode(reportPage.Code);
                List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
                List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
                Dictionary<string, ColumnSummary> ChildCodes = new Dictionary<string, ColumnSummary>();
                List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage.Filters, reportWidget, ReportSource, out ChildCodes);
                Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
                List<string> Conditions = new List<string>();
                List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
                string tablename = reportWidget.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, reportPage.Code, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
                List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
                    tablename,
                    reportWidget.Columns.ToArray(),
                    reportWidget.SortColumns,
                    reportPage.Filters,
                    out statisticalColumn,
                    out UnitTableAndAssociationTable,
                    out TypeChangedColumns,
                    BeginNum,
                    BeginNum + PageSize,
                    out Count,
                    Conditions,
                    Parameters,
                    ReportSource,
                     true);

                return Json(new { State = true, rows = SourceData, total = Count }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {
                return Json(new { State = false, Msg = "参数错误" });
            }

        }
        /// <summary>
        /// 简易看板获取加载数据
        /// </summary>
        /// <returns></returns>
        public JsonResult LoadSimpleBoard()
        {
            string widgetsimpleboardJson = this.Request["WidgetSimpleBoard"];
            string reportPageJson = this.Request["ReportPage"];
            string cha = "\";\"";//剔除选人控件初始值为空时，查询条件应该默认为空
            string FilterDataJson = null;
            string DataJson = this.Request["FilterData"];
            if (DataJson != null)
            {
                FilterDataJson = DataJson.Replace(cha, "");
            }
            string ReportWidgetSimpleBoardJson = this.Request["ReportWidgetSimpleBoard"];
            string WidgetObjectID = this.Request["WidgetObjectID"];
            string ReportPageObjectID = this.Request["ReportPageObjectID"];
            string ReportWidgetSimpleBoardObjectID = this.Request["ReportWidgetSimpleBoardObjectId"];
            string reportSourceJson = this.Request["ReportSource"];
            ReportWidget Setting = null;
            ReportPage reportPage = null;
            ReportSource ReportSource = null;
            Dictionary<string, string> UnitTableAndAssociationTable = new Dictionary<string, string>();
            if (!string.IsNullOrEmpty(reportPageJson))
                reportPage = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportPage>(reportPageJson);
            else
            {
                reportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(ReportPageObjectID));
            }
            if (!string.IsNullOrEmpty(widgetsimpleboardJson))
                Setting = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportWidget>(widgetsimpleboardJson);
            else
            {
                if (reportPage != null && reportPage.ReportWidgets.Length > 0)
                {
                    foreach (ReportWidget q in reportPage.ReportWidgets)
                    {
                        if (q.ObjectID == WidgetObjectID)
                        {
                            Setting = q;
                            break;
                        }
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(WidgetObjectID) && !string.IsNullOrEmpty(ReportPageObjectID))
                        Setting = (ReportWidget)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportWidget(ReportPageObjectID, WidgetObjectID));
                }
            }
            ReportWidgetSimpleBoard SimpleBoardSetting = null;
            if (!string.IsNullOrEmpty(ReportWidgetSimpleBoardJson))
            {
                SimpleBoardSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportWidgetSimpleBoard>(reportPageJson);
            }
            else
            {
                if (Setting != null && Setting.ReportWidgetSimpleBoard.Length > 0)
                {
                    foreach (ReportWidgetSimpleBoard q in Setting.ReportWidgetSimpleBoard)
                    {
                        if (q.ObjectID == ReportWidgetSimpleBoardObjectID)
                        {
                            SimpleBoardSetting = q;
                            break;
                        }
                    }
                }
                else
                {
                    //简易看板
                    SimpleBoardSetting = this.Engine.ReportManager.GetReportWidgetSimpleBoard(ReportWidgetSimpleBoardObjectID, WidgetObjectID, ReportPageObjectID);
                }

            }
            if (!string.IsNullOrEmpty(reportSourceJson))
                ReportSource = this.JsSerializer.Deserialize<ReportSource>(reportSourceJson);
            else
            {
                if (reportPage == null)
                {
                    reportPage = (ReportPage)OThinker.Data.Utility.Clone(this.Engine.ReportManager.GetReportPage(Setting.ParentObjectID));

                }
                for (int i = 0, len = reportPage.ReportSources.Length; i < len; i++)
                {
                    if (reportPage.ReportSources[i].ObjectID == SimpleBoardSetting.ReportSourceId)
                    {
                        ReportSource = reportPage.ReportSources[i];
                        break;
                    }
                }
            }
            if (Setting.WidgetType != WidgetType.SimpleBoard)
            {
                return Json(new { State = false, Msg = "参数错误" });
            }
            if (ReportSource == null)
            {
                return Json(new { State = false, Msg = "无数据源" });
            }
            //ReportSource = this.Engine.ReportManager.GetReportSource(SimpleBoardSetting.ReportSourceId, Setting.ParentObjectId);
            List<ReportWidgetColumn> SourceColumns = this.GetSourceColumns(reportPage.Filters, SimpleBoardSetting, ReportSource, this.Engine.EngineConfig.DBType);

            //联动
            string UnitFilterDataJson = this.Request["UnitFilterDataJson"];
            if (!string.IsNullOrEmpty(UnitFilterDataJson))
            {
                this.DoUnit(UnitFilterDataJson, SourceColumns, reportPage, SimpleBoardSetting, ReportSource, ref SourceColumns);
            }
            //end联动


            //重新检查列
            List<string> Msgs = new List<string>();
            if (!this.ReCheckColumns(SimpleBoardSetting, SourceColumns, out Msgs))
            {
                return Json(new { State = false, Msg = string.Join(";", Msgs) });
            }
            //处理过滤条件
            if (!string.IsNullOrEmpty(FilterDataJson))
            {
                SetFilterValue(reportPage, FilterDataJson);
            }
            List<ReportWidgetColumn> statisticalColumn = new List<ReportWidgetColumn>();
            List<ReportWidgetColumn> TypeChangedColumns = new List<ReportWidgetColumn>();
            FunctionNode reportNode = this.UserValidator.GetFunctionNode(reportPage.Code);
            string NodeCode = "";
            if (reportNode != null)
            {
                NodeCode = reportNode.Code;
            }
            List<string> Conditions = new List<string>();
            List<OThinker.Data.Database.Parameter> Parameters = new List<OThinker.Data.Database.Parameter>();
            string tablename = SimpleBoardSetting.ExecuteTableName(this.Engine.BizObjectManager, this.Engine.Organization, reportPage.Filters, DataAclScope, SourceColumns, NodeCode, ReportSource, this.Engine.EngineConfig.DBType, out Conditions, out Parameters);
            List<Dictionary<string, object>> SourceData = this.Engine.ReportQuery.QueryTable(this.Engine,
               tablename,
                SourceColumns.ToArray(),
                Setting.SortColumns,
                reportPage.Filters,
                out statisticalColumn,
                out UnitTableAndAssociationTable,
                out TypeChangedColumns,
                Conditions,
                Parameters,
                ReportSource);
            if (TypeChangedColumns != null && TypeChangedColumns.Count > 0)
            {
                foreach (ReportWidgetColumn q in TypeChangedColumns)
                {
                    for (int i = 0; SimpleBoardSetting.Columns != null && i < SimpleBoardSetting.Columns.Length; i++)
                    {
                        if (SimpleBoardSetting.Columns[i].ColumnCode == q.ColumnCode)
                        {
                            SimpleBoardSetting.Columns[i].ColumnType = q.ColumnType;
                        }
                    }

                }
            }
            if (SimpleBoardSetting.Columns == null)
                return Json(new { State = false });
            string Resultvalue = this.GetSimpleBoardCollectValue(SourceData, SimpleBoardSetting.Columns[0]);
            return Json(new { State = true, Text = SimpleBoardSetting.Columns[0].DisplayName, Value = Resultvalue });
        }
        /// <summary>
        /// 简易看板数据汇总
        /// </summary>
        /// <param name="SourceData"></param>
        /// <param name="Column"></param>
        /// <returns></returns>
        private string GetSimpleBoardCollectValue(List<Dictionary<string, object>> SourceData, ReportWidgetColumn Column)
        {
            string ColumnFormat = Column.Format;
            OThinker.Reporting.Function FunctionType = Column.FunctionType;
            double Value = 0;
            string Result = "-";
            int count = 0;
            ColumnType ColumnType = Column.ColumnType;
            foreach (Dictionary<string, object> Bo in SourceData)
            {
                count++;
                if (Column.ColumnCode == "DefaultCountCode" || ColumnType != ColumnType.Numeric)
                {
                    continue;
                }
                double Num = 0;
                double.TryParse(Bo[Column.ColumnCode].ToString(), out Num);
                switch (FunctionType)
                {
                    case OThinker.Reporting.Function.Sum:
                    case OThinker.Reporting.Function.Avg:
                        {
                            Value += Num;
                        }; break;

                    case OThinker.Reporting.Function.Count:
                        {
                            Value++;
                        }; break;
                    case OThinker.Reporting.Function.Max:
                        {
                            if (Value < Num)
                                Value = Num;
                        }; break;
                    case OThinker.Reporting.Function.Min:
                        {
                            if (Value > Num)
                            {
                                Value = Num;
                            }
                            else
                            {
                                if (count == 1)
                                {
                                    Value = Num;
                                }
                            }
                        }; break;
                    default: ; break;
                }
            }

            if (Column.ColumnCode == "DefaultCountCode" || ColumnType != ColumnType.Numeric)
            {
                Value = count;
            }
            else
            {
                if (FunctionType == OThinker.Reporting.Function.Avg)
                {
                    if (count > 0)
                    {
                        Value = Value / count;
                    }
                }
            }
            Result = GetFormatValue(Value, ColumnFormat);
            return Result;
        }
        /// <summary>
        /// 设置过滤条件的值
        /// </summary>
        private void SetFilterValue(ReportPage ReportSetting, string FilterDataJson)
        {
            if (ReportSetting.Filters == null || ReportSetting.Filters.Length == 0) return;
            List<ReportFilter> Filters = new List<ReportFilter>();
            Dictionary<string, string[]> FilterData = this.JsSerializer.Deserialize<Dictionary<string, string[]>>(FilterDataJson);
            if (FilterData == null || FilterData.Count == 0) return;
            Dictionary<string, string[]> LowerFilterData = new Dictionary<string, string[]>();
            foreach (string key in FilterData.Keys)
            {
                LowerFilterData.Add(key.ToLowerInvariant(), FilterData[key]);
            }



            foreach (ReportFilter p in ReportSetting.Filters)
            {
                if (LowerFilterData.ContainsKey(p.ColumnCode.ToLowerInvariant()))
                {
                    string[] FilterValues = LowerFilterData[p.ColumnCode.ToLowerInvariant()];
                    if (FilterValues != null && FilterValues.Length > 0)
                    {
                        foreach (string q in FilterValues)
                        {
                            if (!string.IsNullOrEmpty(q))
                            {
                                p.DefaultValue = string.Join(";", FilterValues);
                                break;
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 重新检查列 Error:未完成,//不确定这个方法是否有意义
        /// </summary>
        private bool ReCheckColumns(ReportWidget ReportSetting, List<ReportWidgetColumn> SourceColumns, out List<string> Msgs)
        {
            Msgs = new List<string>();
            if (ReportSetting.Columns != null)
            {
                ReportWidgetColumn[] ReportColumns = ReportSetting.Columns;
                foreach (ReportWidgetColumn column in ReportColumns)
                {
                    bool isExist = false;
                    foreach (ReportWidgetColumn SourceColumn in SourceColumns)
                    {
                        if (SourceColumn.ColumnCode == column.ColumnCode
                            && SourceColumn.ColumnType == column.ColumnType)
                        {
                            isExist = true;
                            SourceColumn.DisplayName = column.DisplayName;
                            SourceColumn.FunctionType = column.FunctionType;
                            SourceColumn.ColumnType = column.ColumnType;
                            SourceColumn.Format = column.Format;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        Msgs.Add(string.Format("字段[{0}]不存在", column.DisplayName));
                    }
                }
            }

            return true;
        }
        /// <summary>
        /// 重新检查列，简易看板 Error:未完成
        /// </summary>
        private bool ReCheckColumns(ReportWidgetSimpleBoard ReportWidgetSimpleBoard, List<ReportWidgetColumn> SourceColumns, out List<string> Msgs)
        {
            Msgs = new List<string>();
            if (ReportWidgetSimpleBoard.Columns != null)
            {
                ReportWidgetColumn[] ReportColumns = ReportWidgetSimpleBoard.Columns;
                foreach (ReportWidgetColumn column in ReportColumns)
                {
                    bool isExist = false;
                    foreach (ReportWidgetColumn SourceColumn in SourceColumns)
                    {
                        if (SourceColumn.ColumnCode == column.ColumnCode
                            && SourceColumn.ColumnType == column.ColumnType)
                        {
                            isExist = true;
                            SourceColumn.DisplayName = column.DisplayName;
                            SourceColumn.FunctionType = column.FunctionType;
                            SourceColumn.ColumnType = column.ColumnType;
                            break;
                        }
                    }
                    if (!isExist)
                    {
                        Msgs.Add(string.Format("字段[{0}]不存在", column.DisplayName));
                    }
                }
            }

            return true;
        }
    }
}