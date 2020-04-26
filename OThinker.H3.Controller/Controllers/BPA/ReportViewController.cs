using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using OThinker.H3.Data;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.BPA
{
    /// <summary>
    /// 报表显示控制器
    /// </summary>
    [Authorize]
    public class ReportViewController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 加载报表配置
        /// </summary>
        /// <param name="sourceCode">数据源编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadSourceSetting(string sourceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportTemplate ReportTemplate = this.Engine.Analyzer.GetReportTemplateByCode(sourceCode);
                return Json(ReportTemplate, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 加载报表数据源
        /// </summary>
        /// <param name="reportSourceSetting">报表数据源</param>
        /// <param name="filterDatas">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>数据源</returns>
        [HttpPost]
        public JsonResult LoadSourceData(string reportSourceSetting, string filterDatas, int page, int pageSize)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportSource ReportSource = JsonConvert.DeserializeObject<ReportSource>(reportSourceSetting);
                SummaryReportResult SummaryReportResult = QueryTable(ReportSource, page, pageSize);
                return Json(SummaryReportResult, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 加载明细汇总表数据
        /// </summary>
        /// <param name="reportSourceSetting">报表数据源</param>
        /// <param name="filterDatas">过滤条件</param>
        /// <param name="page">页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <returns>数据源</returns>
        [HttpPost]
        public JsonResult LoadSummaryData(string reportTemplateSetting, string childColumns, string filterDatas, int page, int pageSize)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportTemplate ReportTemplate = JsonConvert.DeserializeObject<ReportTemplate>(reportTemplateSetting);
                ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(ReportTemplate.SourceCode);
                ReportTemplateParameter[] FilterDatas = JsonConvert.DeserializeObject<ReportTemplateParameter[]>(filterDatas) ?? ReportTemplate.Parameters;
                //合并显示的列
                ReportTemplateColumn[] children = JsonConvert.DeserializeObject<ReportTemplateColumn[]>(childColumns);
                ReportTemplateColumn[] columns = new ReportTemplateColumn[ReportTemplate.Columns.Length + children.Length];
                ReportTemplate.Columns.CopyTo(columns, 0);
                children.CopyTo(columns, ReportTemplate.Columns.Length);

                SummaryReportResult SummaryReportResult = QueryTable(ReportSource, page, pageSize, columns, FilterDatas);
                return Json(SummaryReportResult, JsonRequestBehavior.AllowGet); ;
            });
        }

        /// <summary>
        /// 加载交叉分析表数据
        /// </summary>
        /// <param name="reportTemplateSetting">报表数据源</param>
        /// <param name="filterDatas">过滤条件</param>
        /// <returns>数据源</returns>
        [HttpPost]
        public JsonResult LoadChartsData(string reportTemplateSetting, string filterDatas)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportTemplate ReportTemplate = JsonConvert.DeserializeObject<ReportTemplate>(reportTemplateSetting);
                ReportTemplateParameter[] FilterDatas = JsonConvert.DeserializeObject<ReportTemplateParameter[]>(filterDatas) ?? ReportTemplate.Parameters;
                ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(ReportTemplate.SourceCode);
                return QueryGroupTable(ReportSource, ReportTemplate, FilterDatas);
            });
        }

        /// <summary>
        /// 导出交叉分析报表
        /// </summary>
        /// <param name="filterDatas">过滤条件</param>
        /// <param name="reportTemplateSetting">报表源配置</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult ExportEchartsReport(string filterDatas, string reportTemplateSetting)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportTemplate ReportTemplate = JsonConvert.DeserializeObject<ReportTemplate>(reportTemplateSetting);
                ReportTemplateParameter[] FilterDatas = JsonConvert.DeserializeObject<ReportTemplateParameter[]>(filterDatas);
                ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(ReportTemplate.SourceCode);

                Analytics.AnalyticalQuery Query = this.Engine.BPAQuery.GetAnalyticalQuery(this.Engine, ReportSource);
                DataTable dataTable = Query.QueryGroupTable(ReportSource.ExecuteTableName, ReportSource.Columns, ReportTemplate, FilterDatas);

                string FileUrl = string.Empty;
                if (string.IsNullOrWhiteSpace(ReportTemplate.RowTitle))
                {//没有配置行标题的
                    FileUrl = ExportEchartsReportWithoutRowTitle(ReportTemplate, dataTable, ReportSource.Columns);
                }
                else
                {//有配置行标题
                    if (string.IsNullOrWhiteSpace(ReportTemplate.ColumnTitle))
                    {
                        FileUrl = ExportEchartsReportWithoutColumnTitle(ReportTemplate, dataTable, ReportSource.Columns);
                    }
                    else
                    {
                        FileUrl = ExportEchartsReportQueryExistRowTitle(ReportTemplate, dataTable, ReportSource.Columns);
                    }
                }
                var result = new { FileUrl = FileUrl };
                return Json(result, JsonRequestBehavior.AllowGet); ;
            });
        }
        /// <summary>
        /// 校验过滤数据的权限
        /// </summary>
        /// <param name="filterDatas">过滤条件</param>
        /// <returns>是否验证通过</returns>
        [HttpPost]
        public JsonResult ValiteFilterData(string filterDatas)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result.Success = true;
                ReportTemplateParameter[] FilterDatas = JsonConvert.DeserializeObject<ReportTemplateParameter[]>(filterDatas);
                //查找出所有需要验权的组织编码
                List<string> OrgCodes = new List<string>();
                List<string> WorkflowCodes = new List<string>();
                foreach (ReportTemplateParameter Parameter in FilterDatas)
                {
                    if (string.IsNullOrWhiteSpace(Parameter.DefaultValue)) continue;
                    if (Parameter.ParameterType == Analytics.ParameterType.Orgnization
                        && Parameter.ParameterValue == "1")
                    {
                        OrgCodes.Add(Parameter.DefaultValue);
                    }

                    if (Parameter.ParameterType == Analytics.ParameterType.WorkflowCode)
                    {
                        WorkflowCodes.Add(Parameter.DefaultValue);
                    }
                }

                //没有组织的过滤条件时，返回有权，因为只能根据组织来控制权限
                if (OrgCodes.Count > 0)
                {
                    Unit[] Units = this.Engine.Organization.GetUnits(OrgCodes.ToArray()).ToArray();
                    foreach (Unit Unit in Units)
                    {
                        //验证查看权限
                        if (!this.UserValidator.ValidateOrgView(Unit.UnitID))
                        {
                            //验证管理权限 
                            if (!this.UserValidator.ValidateOrgAdmin(Unit.UnitID))
                            {
                                if (WorkflowCodes.Count > 0)
                                {
                                    //没有组织权限，看看BO有无权限
                                    foreach (string WorkflowCode in WorkflowCodes)
                                    {
                                        if (!this.UserValidator.ValidateWFInsAdmin(WorkflowCode, Unit.UnitID))
                                        {
                                            result.Success = false;
                                            result.Message = "ReportTemplate.Msg0";
                                            result.Extend = Unit.Name;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    result.Success = false;
                                    result.Message = "ReportTemplate.Msg0";
                                    result.Extend = Unit.Name;
                                    break;
                                }
                            }
                        }
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet); ;
            });
        }

        /// <summary>
        /// 导出报表
        /// </summary>
        /// <param name="filterDatas">过滤条件</param>
        /// <param name="reportTemplateSetting">数据源配置</param>
        /// <returns>导出结果</returns>
        [HttpPost]
        public JsonResult ExportligerReport(string filterDatas, string reportTemplateSetting)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportTemplateParameter[] FilterDatas = JsonConvert.DeserializeObject<ReportTemplateParameter[]>(filterDatas);
                ReportTemplate ReportTemplate = JsonConvert.DeserializeObject<ReportTemplate>(reportTemplateSetting);
                ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(ReportTemplate.SourceCode);
                SummaryReportResult SummaryReportResult = QueryTable(ReportSource, 0, 0, ReportTemplate.Columns, FilterDatas);
                Dictionary<string, string> heads = new Dictionary<string, string>();
                foreach (ReportTemplateColumn Column in ReportTemplate.Columns)
                {
                    if (Column.ReportSourceColumnType != ReportSourceColumnType.CustomColumn)
                    {
                        heads.Add(Column.ColumnCode, Column.DisplayName);
                    }
                    //heads.Add(Column.ColumnCode, Column.DisplayName);
                }

                List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
                Dictionary<string, object> data = new Dictionary<string, object>();
                for (int i = 0; i < SummaryReportResult.Total; i++)
                {
                    data = new Dictionary<string, object>();
                    foreach (string key in heads.Keys)
                    {
                        data.Add(key, SummaryReportResult.Rows[i][key]);
                    }
                    datas.Add(data);
                }
                var result = new { FileUrl = ExportExcel.ExportExecl(ReportTemplate.DisplayName + "." + ReportTemplate.Code, heads, datas) };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 导出没有行标题的交叉分析报表
        /// </summary>
        /// <param name="ReportTemplate"></param>
        /// <param name="DataTable"></param>
        /// <returns>导出文件的下载地址</returns>
        private string ExportEchartsReportWithoutRowTitle(ReportTemplate ReportTemplate, DataTable DataTable, ReportSourceColumn[] ReportSourceColumns)
        {
            WithoutRowChartsReportResult WithoutRowChartsReportResult = QueryNoneRowTitle(ReportTemplate, DataTable, ReportSourceColumns);
            Dictionary<string, string> heads = new Dictionary<string, string>();
            string RowTitle = "RowTitle";
            heads.Add(RowTitle, "");
            foreach (string Column in WithoutRowChartsReportResult.ColumnData)
            {
                heads.Add(Column, Column);
            }

            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string groupKey in WithoutRowChartsReportResult.GroupData.Keys)
            {
                data = new Dictionary<string, object>();
                data.Add(RowTitle, WithoutRowChartsReportResult.GroupData[groupKey]);
                for (int i = 0, len = WithoutRowChartsReportResult.ReportData.Count; i < len; i++)
                {
                    Dictionary<string, object> row = WithoutRowChartsReportResult.ReportData[i];
                    if (row.ContainsKey(groupKey))
                    {
                        data.Add(WithoutRowChartsReportResult.ColumnData[i], row[groupKey]);
                    }
                }
                datas.Add(data);
            }

            return ExportExcel.ExportExecl(ReportTemplate.DisplayName + "." + ReportTemplate.Code, heads, datas);
        }

        private string ExportEchartsReportWithoutColumnTitle(ReportTemplate ReportTemplate, DataTable DataTable, ReportSourceColumn[] ReportSourceColumns)
        {
            ExistRowChartsReportResult ExistRowChartsReportResult = QueryNoneColumnTitle(ReportTemplate, DataTable, ReportSourceColumns);
            string[] RowTitles = ReportTemplate.RowTitle.Split(';');
            string GroupTitle = RowTitles[0];
            string SubGroupTitle = RowTitles.Length > 1 ? RowTitles[1] : string.Empty;

            Dictionary<string, string> heads = new Dictionary<string, string>();

            foreach (string RowTitle in RowTitles)
            {
                heads.Add(RowTitle, GetColumnDisplayName(RowTitle, ExistRowChartsReportResult.ReportSourceColumns));
            }

            foreach (string Column in ExistRowChartsReportResult.ColumnData)
            {
                heads.Add(Column, Column);
            }

            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string groupKey in ExistRowChartsReportResult.GroupData)
            {
                if (string.IsNullOrWhiteSpace(SubGroupTitle))
                {//第二个行标题为空
                    data = new Dictionary<string, object>();
                    data.Add(GroupTitle, groupKey);
                    for (var j = 0; j < ExistRowChartsReportResult.ColumnData.Count; j++)
                    {
                        string columnValue = ExistRowChartsReportResult.ColumnData[j];
                        if (data.ContainsKey(columnValue)) continue;
                        Dictionary<string, object> row = ExistRowChartsReportResult.ReportData.Find((o) =>
                         {
                             return string.Equals(o[GroupTitle], groupKey);
                         });
                        data.Add(columnValue, row == null ? 0 : row[ReportTemplate.Columns[j].ColumnCode]);
                    }
                    datas.Add(data);
                }
                else
                {//有第二个行标题
                    foreach (string subGroup in ExistRowChartsReportResult.SubGroupData)
                    {
                        data = new Dictionary<string, object>();
                        data.Add(GroupTitle, groupKey);
                        data.Add(SubGroupTitle, subGroup);
                        for (var j = 0; j < ExistRowChartsReportResult.ColumnData.Count; j++)
                        {
                            string columnValue = ExistRowChartsReportResult.ColumnData[j];
                            if (data.ContainsKey(columnValue)) continue;
                            Dictionary<string, object> row = ExistRowChartsReportResult.ReportData.Find((o) =>
                            {
                                return string.Equals(o[GroupTitle], groupKey)
                                    && string.Equals(o[SubGroupTitle], subGroup);
                            });
                            data.Add(columnValue, row == null ? 0 : row[ReportTemplate.Columns[j].ColumnCode]);
                        }
                        datas.Add(data);
                    }
                }
            }

            return ExportExcel.ExportExecl(ReportTemplate.DisplayName + "." + ReportTemplate.Code, heads, datas);
        }



        /// <summary>
        /// 导出有行标题的交叉分析报表
        /// </summary>
        /// <param name="ReportTemplate"></param>
        /// <param name="DataTable"></param>
        /// <returns></returns>
        private string ExportEchartsReportQueryExistRowTitle(ReportTemplate ReportTemplate, DataTable DataTable, ReportSourceColumn[] ReportSourceColumns)
        {
            ExistRowChartsReportResult ExistRowChartsReportResult = QueryExistRowTitle(ReportTemplate, DataTable, ReportSourceColumns);
            Dictionary<string, string> heads = new Dictionary<string, string>();

            string[] RowTitles = ReportTemplate.RowTitle.Split(';');
            string GroupTitle = RowTitles[0];
            string SubGroupTitle = RowTitles.Length > 1 ? RowTitles[1] : string.Empty;

            var columnCode = "CountNum";//有行标题时，最多只能有一个统计字段
            if (ReportTemplate.Columns != null && ReportTemplate.Columns.Length > 0)
            {
                columnCode = ReportTemplate.Columns[0].ColumnCode;
            }

            foreach (string RowTitle in RowTitles)
            {
                heads.Add(RowTitle, GetColumnDisplayName(RowTitle, ExistRowChartsReportResult.ReportSourceColumns));
            }

            foreach (string Column in ExistRowChartsReportResult.ColumnData)
            {
                heads.Add(Column, Column);
            }

            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            foreach (string groupKey in ExistRowChartsReportResult.GroupData)
            {
                if (string.IsNullOrWhiteSpace(SubGroupTitle))
                {//第二个行标题为空
                    data = new Dictionary<string, object>();
                    data.Add(GroupTitle, groupKey);
                    for (var j = 0; j < ExistRowChartsReportResult.ColumnData.Count; j++)
                    {
                        string columnValue = ExistRowChartsReportResult.ColumnData[j];
                        if (data.ContainsKey(columnValue)) continue;
                        Dictionary<string, object> row = ExistRowChartsReportResult.ReportData.Find((o) =>
                         {
                             return string.Equals(o[GroupTitle], groupKey)
                                 && string.Equals(o[ReportTemplate.ColumnTitle], columnValue);
                         });
                        data.Add(columnValue, row == null ? 0 : row[columnCode]);
                    }
                    datas.Add(data);
                }
                else
                {//有第二个行标题
                    foreach (string subGroup in ExistRowChartsReportResult.SubGroupData)
                    {
                        data = new Dictionary<string, object>();
                        data.Add(GroupTitle, groupKey);
                        data.Add(SubGroupTitle, subGroup);
                        for (var j = 0; j < ExistRowChartsReportResult.ColumnData.Count; j++)
                        {
                            string columnValue = ExistRowChartsReportResult.ColumnData[j];
                            if (data.ContainsKey(columnValue)) continue;
                            Dictionary<string, object> row = ExistRowChartsReportResult.ReportData.Find((o) =>
                            {
                                return string.Equals(o[GroupTitle], groupKey)
                                    && string.Equals(o[SubGroupTitle], subGroup)
                                    && string.Equals(o[ReportTemplate.ColumnTitle], columnValue);
                            });
                            data.Add(columnValue, row == null ? 0 : row[columnCode]);
                        }
                        datas.Add(data);
                    }
                }
            }

            return ExportExcel.ExportExecl(ReportTemplate.DisplayName + "." + ReportTemplate.Code, heads, datas);
        }

        /// <summary>
        /// 获取原始列名
        /// </summary>
        /// <param name="ColumnCode"></param>
        /// <param name="ReportSourceColumns"></param>
        /// <returns></returns>
        private string GetColumnDisplayName(string ColumnCode, ReportSourceColumn[] ReportSourceColumns)
        {
            foreach (ReportSourceColumn column in ReportSourceColumns)
            {
                if (column.ColumnCode == ColumnCode)
                {
                    return column.DisplayName;
                }
            }
            return "";
        }

        /// <summary>
        /// 查询表
        /// </summary>;
        /// <param name="ReportSource"></param>
        /// <param name="TemplateColumns"></param>
        private SummaryReportResult QueryTable(ReportSource ReportSource, int page, int pageSize, ReportTemplateColumn[] TemplateColumns = null, ReportTemplateParameter[] Parameters = null)
        {
            Analytics.AnalyticalQuery Query = this.Engine.BPAQuery.GetAnalyticalQuery(this.Engine, ReportSource);
            ReportSourceColumn[] Columns = ReportSource.Columns;

            int dataCount = Query.QueryTableCount(ReportSource.ExecuteTableName, Parameters);
            int startIndex = 0;
            int endIndex = dataCount;
            if (pageSize > 0)
            {
                startIndex = ((page - 1) * pageSize + 1) < 1 ? 1 : ((page - 1) * pageSize) + 1;
                endIndex = page * pageSize >= dataCount ? dataCount : page * pageSize;
            }

            //报表数据
            List<Dictionary<string, object>> ReportDatas = new List<Dictionary<string, object>>();
            //汇总列数据
            Dictionary<string, double> SummaryColumns = new Dictionary<string, double>();
            Dictionary<string, object> ReportData;
            DataTable dataTable = Query.QueryTable(ReportSource.ExecuteTableName, Columns, Parameters, startIndex, endIndex);
            //提取需要被装换的列名
            List<string> convertCodes = new List<string>();
            //批量查询组织
            Unit[] units = this.GetConvertCodesUnits(out convertCodes, TemplateColumns, dataTable.Rows, Columns);
            //填充返回单元格
            foreach (DataRow row in dataTable.Rows)
            {
                ReportData = new Dictionary<string, object>();
                foreach (ReportSourceColumn col in Columns)
                {
                    if (col.ReportSourceColumnType != ReportSourceColumnType.CustomColumn)
                    {
                        //从Units中找到对应的Unit
                        if (convertCodes.Contains(col.ColumnCode))
                        {
                            Unit user = null;
                            foreach (var u in units)
                            {
                                if (u.ObjectID == row[col.ColumnCode].ToString())
                                {
                                    user = u;
                                    break;
                                }
                            }
                            if (user != null && user.Name != null)
                                row[col.ColumnCode] = user.Name;
                        }
                        ReportData.Add(col.ColumnCode, row[col.ColumnCode]);
                    }
                    //汇总列结算
                    if (col.ReportSourceColumnType != ReportSourceColumnType.CustomColumn && col.DataType == DataType.Numeric)
                    {
                        double count = 0;
                        if (double.TryParse(row[col.ColumnCode].ToString(), out count))
                        {
                            if (SummaryColumns.ContainsKey(col.ColumnCode))
                            {

                                SummaryColumns[col.ColumnCode] += count;
                            }
                            else
                            {
                                SummaryColumns.Add(col.ColumnCode, count);
                            }
                        }

                    }
                }
                ReportDatas.Add(ReportData);
            }
            SummaryReportResult SummaryReportResult = new SummaryReportResult(ReportDatas, dataCount, SummaryColumns);
            return SummaryReportResult;
        }

        /// <summary>
        /// 将列中的数据装换为units
        /// </summary>
        /// <param name="convertCodes">列名</param>
        /// <param name="templateColumns">显示的列</param>
        /// <param name="rows">数据行</param>
        /// <param name="Columns">源列</param>
        /// <returns>组织机构基础单元</returns>
        private Unit[] GetConvertCodesUnits(out List<string> convertCodes, ReportTemplateColumn[] templateColumns, DataRowCollection rows, ReportSourceColumn[] Columns)
        {
            convertCodes = new List<string>();
            if (templateColumns != null)
            {
                foreach (var TemplateColumn in templateColumns)
                {
                    //需要装换的列
                    if (TemplateColumn.DisplayType == DataLogicType.SingleParticipant || TemplateColumn.DisplayType == DataLogicType.MultiParticipant)
                        convertCodes.Add(TemplateColumn.ColumnCode);
                }
            }
            List<string> users = new List<string>();
            foreach (DataRow row in rows)
            {
                foreach (ReportSourceColumn col in Columns)
                {
                    if (col.ReportSourceColumnType != ReportSourceColumnType.CustomColumn)
                    {
                        //从Units中找到对应的Unit
                        if (convertCodes.Contains(col.ColumnCode))
                        {
                            users.Add(row[col.ColumnCode].ToString());
                        }
                    }
                }
            }
            return this.Engine.Organization.GetUnits(users.ToArray()).ToArray();
        }

        /// <summary>
        /// 查询分组表
        /// </summary>
        /// <param name="ReportSource"></param>
        /// <param name="ReportTemplate"></param>
        /// <param name="Parameters"></param>
        private JsonResult QueryGroupTable(ReportSource ReportSource, ReportTemplate ReportTemplate, ReportTemplateParameter[] Parameters)
        {
            Analytics.AnalyticalQuery Query = this.Engine.BPAQuery.GetAnalyticalQuery(this.Engine, ReportSource);
            DataTable dataTable = Query.QueryGroupTable(ReportSource.ExecuteTableName, ReportSource.Columns, ReportTemplate, Parameters);

            object QueryResult;
            if (string.IsNullOrWhiteSpace(ReportTemplate.ColumnTitle))
            {//没有列标题
                QueryResult = QueryNoneColumnTitle(ReportTemplate, dataTable, ReportSource.Columns);
            }
            else
            {//有行标题
                if (string.IsNullOrWhiteSpace(ReportTemplate.RowTitle))
                {//没有配置行标题的
                    QueryResult = QueryNoneRowTitle(ReportTemplate, dataTable, ReportSource.Columns);
                }
                else
                {//有配置行标题
                    QueryResult = QueryExistRowTitle(ReportTemplate, dataTable, ReportSource.Columns);
                }
            }

            return Json(QueryResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 没有列标题时构建数据
        /// </summary>
        /// <param name="ReportSource"></param>
        /// <param name="ReportTemplate"></param>
        /// <param name="DataTable"></param>
        private ExistRowChartsReportResult QueryNoneColumnTitle(
            ReportTemplate ReportTemplate,
            DataTable DataTable,
            ReportSourceColumn[] ReportSourceColumns)
        {
            //列标题数据
            List<string> ColumnData = new List<string>();
            //行数据
            List<string> GroupData = new List<string>();
            //第二个行标题数据，可为空,且最多两个
            List<string> SubGroupData = new List<string>();

            // 行数据钻取的维度值
            List<string> GroupDrillData = new List<string>();
            //第二个行标题的钻取维度
            List<string> SubGroupDrillData = new List<string>();

            //报表数据
            List<Dictionary<string, object>> ReportData = new List<Dictionary<string, object>>();

            //添加列标题数据
            foreach (ReportTemplateColumn column in ReportTemplate.Columns)
            {
                ColumnData.Add(column.DisplayName);
            }

            //行标题标示
            string[] rowTitles = ReportTemplate.RowTitle.Split(new string[] { ReportTemplateParameter.Delimiter }
                , StringSplitOptions.RemoveEmptyEntries);
            string groupTitle = rowTitles[0];
            string subGroupTitle = rowTitles.Length > 1 ? rowTitles[1] : "";

            DrillParam[] RowDrillParam = ReportTemplate.RowDrillParam;


            //循环每一个数据
            Dictionary<string, object> result;
            string groupValue;
            string subGroupValue;
            foreach (DataRow row in DataTable.Rows)
            {
                result = new Dictionary<string, object>();

                //添加行标题数据
                groupValue = row[groupTitle].ToString();
                if (!GroupData.Contains(groupValue))
                {
                    GroupData.Add(groupValue);
                    GroupDrillData.Add(row[RowDrillParam[0].Code].ToString());
                }

                //添加第二个行标题数据
                if (!string.IsNullOrWhiteSpace(subGroupTitle))
                {
                    subGroupValue = row[subGroupTitle].ToString();
                    if (!SubGroupData.Contains(subGroupValue))
                    {
                        SubGroupData.Add(subGroupValue);
                        SubGroupDrillData.Add(row[RowDrillParam[1].Code].ToString());
                    }
                    result.Add(subGroupTitle, subGroupValue);
                    if (!result.ContainsKey(RowDrillParam[1].Code))
                    {
                        result.Add(RowDrillParam[1].Code, row[RowDrillParam[1].Code].ToString());
                    }
                }

                //添加报表数据
                result.Add(groupTitle, groupValue);
                if (!result.ContainsKey(RowDrillParam[0].Code))
                {
                    result.Add(RowDrillParam[0].Code, row[RowDrillParam[0].Code].ToString());
                }

                foreach (ReportTemplateColumn column in ReportTemplate.Columns)
                {
                    if (!result.ContainsKey(column.ColumnCode))
                        result.Add(column.ColumnCode, row[column.ColumnCode]);
                }
                ReportData.Add(result);
            }

            return new ExistRowChartsReportResult(ColumnData, GroupData, SubGroupData, null, GroupDrillData, SubGroupDrillData, ReportData, ReportSourceColumns);
        }
        /// <summary>
        /// 存在行标题时构建数据
        /// </summary>
        /// <param name="ReportSource"></param>
        /// <param name="ReportTemplate"></param>
        /// <param name="DataTable"></param>
        private ExistRowChartsReportResult QueryExistRowTitle(
            ReportTemplate ReportTemplate,
            DataTable DataTable,
            ReportSourceColumn[] ReportSourceColumns)
        {
            //列标题数据
            List<string> ColumnData = new List<string>();
            //行数据
            List<string> GroupData = new List<string>();
            //第二个行标题数据，可为空,且最多两个
            List<string> SubGroupData = new List<string>();
            //钻取数据
            List<string> ColumnDrillData = new List<string>();
            // 行数据钻取的维度值
            List<string> GroupDrillData = new List<string>();
            //第二个行标题的钻取维度
            List<string> SubGroupDrillData = new List<string>();

            //报表数据
            List<Dictionary<string, object>> ReportData = new List<Dictionary<string, object>>();

            //行标题标示
            string[] rowTitles = ReportTemplate.RowTitle.Split(new string[] { ReportTemplateParameter.Delimiter }
                , StringSplitOptions.RemoveEmptyEntries);
            string groupTitle = rowTitles[0];
            string subGroupTitle = rowTitles.Length > 1 ? rowTitles[1] : "";
            string ColumnTitle = ReportTemplate.ColumnTitle;

            //循环每一个数据
            Dictionary<string, object> result;
            string groupValue;
            string subGroupValue;
            foreach (DataRow row in DataTable.Rows)
            {
                result = new Dictionary<string, object>();
                string columnValue = row[ColumnTitle].ToString();
                //添加列标题数据
                if (!ColumnData.Contains(columnValue))
                {
                    ColumnData.Add(columnValue);
                    ColumnDrillData.Add(row[ReportTemplate.ColumnDrillParam.Code].ToString());
                }

                //添加行标题数据
                groupValue = row[groupTitle].ToString();
                if (!GroupData.Contains(groupValue))
                {
                    GroupData.Add(groupValue);
                    GroupDrillData.Add(row[ReportTemplate.RowDrillParam[0].Code].ToString());
                }

                //添加第二个行标题数据
                if (!string.IsNullOrWhiteSpace(subGroupTitle))
                {
                    subGroupValue = row[subGroupTitle].ToString();
                    if (!SubGroupData.Contains(subGroupValue))
                    {
                        SubGroupData.Add(subGroupValue);
                        SubGroupDrillData.Add(row[ReportTemplate.RowDrillParam[1].Code].ToString());
                    }
                    if (!result.ContainsKey(subGroupTitle))
                        result.Add(subGroupTitle, subGroupValue);
                    if (!result.ContainsKey(ReportTemplate.RowDrillParam[1].Code))
                        result.Add(ReportTemplate.RowDrillParam[1].Code, row[ReportTemplate.RowDrillParam[1].Code].ToString());
                }

                //添加报表数据
                if (!result.ContainsKey(ColumnTitle))
                    result.Add(ColumnTitle, columnValue);
                if (!result.ContainsKey(ReportTemplate.ColumnDrillParam.Code))
                    result.Add(ReportTemplate.ColumnDrillParam.Code, row[ReportTemplate.ColumnDrillParam.Code].ToString());
                if (!result.ContainsKey(groupTitle))
                    result.Add(groupTitle, groupValue);
                if (!result.ContainsKey(ReportTemplate.RowDrillParam[0].Code))
                    result.Add(ReportTemplate.RowDrillParam[0].Code, row[ReportTemplate.ColumnDrillParam.Code].ToString());
                if (ReportTemplate.Columns == null)
                {
                    result.Add("CountNum", row["CountNum"]);
                }
                else
                {
                    foreach (ReportTemplateColumn column in ReportTemplate.Columns)
                    {
                        result.Add(column.ColumnCode, row[column.ColumnCode]);
                    }
                }
                ReportData.Add(result);
            }

            return new ExistRowChartsReportResult(ColumnData, GroupData, SubGroupData, ColumnDrillData, GroupDrillData, SubGroupDrillData, ReportData, ReportSourceColumns);
        }

        /// <summary>
        /// 没有行标题的时候构建数据
        /// </summary>
        /// <param name="ReportSource"></param>
        /// <param name="ReportTemplate"></param>
        /// <param name="DataTable"></param>
        private WithoutRowChartsReportResult QueryNoneRowTitle(
            ReportTemplate ReportTemplate,
            DataTable DataTable,
            ReportSourceColumn[] ReportSourceColumns)
        {
            //列标题数据
            List<string> ColumnData = new List<string>();
            //钻取数据
            List<string> ColumnDrillData = new List<string>();
            //行数据
            Dictionary<string, string> GroupData = new Dictionary<string, string>();
            //报表数据
            List<Dictionary<string, object>> ReportData = new List<Dictionary<string, object>>();
            //添加行标题数据
            foreach (ReportTemplateColumn column in ReportTemplate.Columns)
            {
                GroupData.Add(column.ColumnCode, column.DisplayName);
            }

            //循环每一个数据
            Dictionary<string, object> result;
            string ColumnTitle = ReportTemplate.ColumnTitle;
            foreach (DataRow row in DataTable.Rows)
            {
                string columnValue = row[ColumnTitle].ToString();
                //添加列标题数据
                if (!ColumnData.Contains(columnValue))
                {
                    ColumnData.Add(columnValue);
                }

                if (!ColumnDrillData.Contains(row[ReportTemplate.ColumnDrillParam.Code].ToString()))
                {
                    ColumnDrillData.Add(row[ReportTemplate.ColumnDrillParam.Code].ToString());
                }

                //添加报表数据
                result = new Dictionary<string, object>();
                result.Add(ColumnTitle, columnValue);
                if (!result.ContainsKey(ReportTemplate.ColumnDrillParam.Code))
                    result.Add(ReportTemplate.ColumnDrillParam.Code, row[ReportTemplate.ColumnDrillParam.Code].ToString());
                foreach (ReportTemplateColumn column in ReportTemplate.Columns)
                {
                    result.Add(column.ColumnCode, row[column.ColumnCode]);
                }
                ReportData.Add(result);
            }

            return new WithoutRowChartsReportResult(ColumnData, ColumnDrillData, GroupData, ReportData, ReportSourceColumns);
        }


        private class ExistRowChartsReportResult
        {
            /// <summary>
            /// 列数据
            /// </summary>
            public List<string> ColumnData;

            /// <summary>
            /// 列钻取的维度数据
            /// </summary>
            public List<string> ColumnDrillData;

            /// <summary>
            /// 分组数据
            /// </summary>
            public List<string> GroupData;

            /// <summary>
            /// 分组钻取的维度值
            /// </summary>
            public List<string> GroupDrillData;

            /// <summary>
            /// 二级分组数据
            /// </summary>
            public List<string> SubGroupData;

            /// <summary>
            /// 分组钻取的维度值
            /// </summary>
            public List<string> SubGroupDrillData;

            /// <summary>
            /// 报表数据
            /// </summary>
            public List<Dictionary<string, object>> ReportData;

            /// <summary>
            /// 报表列
            /// </summary>
            public ReportSourceColumn[] ReportSourceColumns;

            /// <summary>
            /// 存在行标题
            /// </summary>
            public bool ExistRowTitle = true;

            public ExistRowChartsReportResult(
                List<string> ColumnData,
                List<string> GroupData,
                List<string> SubGroupData,
                List<string> ColumnDrillData,
                List<string> GroupDrillData,
                List<string> SubGroupDrillData,
                List<Dictionary<string, object>> ReportData,
                ReportSourceColumn[] ReportSourceColumns)
            {
                this.ColumnData = ColumnData;
                this.GroupData = GroupData;
                this.SubGroupData = SubGroupData;
                this.ReportData = ReportData;

                this.ColumnDrillData = ColumnDrillData;
                this.GroupDrillData = GroupDrillData;
                this.SubGroupDrillData = SubGroupDrillData;
                this.ReportSourceColumns = ReportSourceColumns;

                this.ExistRowTitle = true;

            }
        }

        private class WithoutRowChartsReportResult
        {
            /// <summary>
            /// 列数据
            /// </summary>
            public List<string> ColumnData;

            /// <summary>
            /// 列钻取的维度数据
            /// </summary>
            public List<string> ColumnDrillData;

            /// <summary>
            /// 分组数据
            /// </summary>
            public Dictionary<string, string> GroupData;

            /// <summary>
            /// 报表数据
            /// </summary>
            public List<Dictionary<string, object>> ReportData;

            /// <summary>
            /// 存在行标题
            /// </summary>
            public bool ExistRowTitle = false;

            /// <summary>
            /// 报表列
            /// </summary>
            public ReportSourceColumn[] ReportSourceColumns;

            public WithoutRowChartsReportResult(
                List<string> ColumnData,
                 List<string> ColumnDrillData,
                Dictionary<string, string> GroupData,
                List<Dictionary<string, object>> ReportData,
               ReportSourceColumn[] ReportSourceColumns)
            {
                this.ColumnData = ColumnData;
                this.GroupData = GroupData;
                this.ColumnDrillData = ColumnDrillData;
                this.ReportData = ReportData;
                this.ExistRowTitle = false;
                this.ReportSourceColumns = ReportSourceColumns;
            }
        }

        private class SummaryReportResult
        {
            /// <summary>
            /// 汉数据
            /// </summary>
            public List<Dictionary<string, object>> Rows;

            /// <summary>
            /// 总数
            /// </summary>
            public int Total;

            /// <summary>
            /// 汇总列数据
            /// </summary>
            public Dictionary<string, double> SummaryColumns;
            public SummaryReportResult(List<Dictionary<string, object>> Rows, int Total, Dictionary<string, double> SummaryColumns)
            {
                this.Rows = Rows;
                this.Total = Total;
                this.SummaryColumns = SummaryColumns;
            }
        }
    }
}
