using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.BPA
{
    /// <summary>
    /// 报表模板控制器
    /// </summary>
    [Authorize]
    public class ReportTemplateController : ControllerBase
    {
        /// <summary>
        /// 模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.BPA_ReportTemplate_Code; }
        }

        /// <summary>
        /// 获取报表模板
        /// </summary>
        /// <param name="templateCode">报表模板编码</param>
        /// <returns>报表模板</returns>
        [HttpPost]
        public JsonResult LoadReportTemplateSetting(string templateCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportTemplate ReportTemplate = this.Engine.Analyzer.GetReportTemplateByCode(templateCode);
                if (null == ReportTemplate)
                {
                    result.Success = false;
                    result.Message = "ReportTemplate.ReportTemplateRequired";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(ReportTemplate.SourceCode);
                if (null == ReportSource)
                {
                    result.Success = false;
                    result.Message = "ReportTemplate.ReportSourceRequired";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                ClearColumnData(ReportSource);
                ImproveColumnData(ReportSource);
                var SourceData = LoadSourceData(ReportSource);
                result.Success = true;
                result.Extend = new { ReportTemplate = ReportTemplate, ReportSource = ReportSource, SourceData = SourceData };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取所有报表数据源
        /// </summary>
        /// <returns>返回报表数据源集合</returns>
        [HttpPost]
        public JsonResult LoadReportSource()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportSource[] ReportSources = this.Engine.Analyzer.GetReportSources();
                if (ReportSources == null || ReportSources.Length == 0)
                {
                    result.Message = "ReportTemplate.ReportSourceRequired";
                    result.Success = false;
                }
                else
                {
                    foreach (ReportSource ReportSource in ReportSources)
                    {
                        //这个处理很重要，新增的时候，得把所有列的序列化标志全部清空,因为这个表继承了父类，从客户端传过来时，把父类的标示页传过来
                        ClearColumnData(ReportSource);
                    }
                    result.Success = true;
                    result.Extend = ReportSources;
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 加载报表
        /// </summary>
        /// <param name="sourceCode">数据源编码</param>
        /// <returns>指定编码报表数据</returns>
        [HttpPost]
        public JsonResult LoadSourceData(string sourceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(sourceCode);
                var jsonObj = LoadSourceData(ReportSource);
                return Json(jsonObj, JsonRequestBehavior.AllowGet);

            });
        }

        /// <summary>
        /// 添加报表模板
        /// </summary>
        /// <param name="reportTemplate">报表模板数据</param>
        /// <param name="childColumns">报表模板显示列的子列</param>
        /// <returns>ActionResult:是否成功</returns>
        [HttpPost]
        public JsonResult AddTemplate(string reportTemplate,string childColumns)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportTemplate ReportTemplate = JsonConvert.DeserializeObject<ReportTemplate>(reportTemplate);
                ReportTemplateColumn[] ChildColumns = JsonConvert.DeserializeObject<ReportTemplateColumn[]>(childColumns);
                ReportTemplate.Columns = RefactorColumn(ReportTemplate.Columns, ChildColumns);
                ReportTemplate.Creator = this.UserValidator.UserID;
                result = ValidateSaveData(ReportTemplate, true);
                if (!result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                long Result = this.Engine.Analyzer.AddReportTemplate(ReportTemplate);
                if (Result == ErrorCode.SUCCESS)
                {
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.Message = "ReportTemplate.Msg0";
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 更新报表模板
        /// </summary>
        /// <param name="reportTemplate">报表模板数据</param>
        /// <param name="childColumns">报表模板显示列的子列</param>
        /// <returns>ActionResult:是否成功</returns>
        [HttpPost]
        public JsonResult UpdateTemplate(string reportTemplate, string childColumns)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportTemplate ReportTemplate = JsonConvert.DeserializeObject<ReportTemplate>(reportTemplate);
                ReportTemplateColumn[] ChildColumns = JsonConvert.DeserializeObject<ReportTemplateColumn[]>(childColumns);
                ReportTemplate.Columns = RefactorColumn(ReportTemplate.Columns, ChildColumns);
                result = ValidateSaveData(ReportTemplate, false);
                if (!result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                result.Success = this.Engine.Analyzer.UpdateReportTemplate(ReportTemplate);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 将子列添加到列中
        /// </summary>
        /// <param name="reportTemplateColumn">原始列</param>
        /// <param name="childColumns">子列</param>
        /// <returns>增加子列后的列</returns>
        private ReportTemplateColumn[] RefactorColumn(ReportTemplateColumn[] reportTemplateColumn, ReportTemplateColumn[] childColumns)
        {
            if (reportTemplateColumn == null || reportTemplateColumn.Length == 0) return reportTemplateColumn;
            if (null != childColumns && childColumns.Length != 0)
            {
                List<ReportTemplateColumn> list = reportTemplateColumn.ToList();
                list.AddRange(childColumns.ToList());
                reportTemplateColumn = list.ToArray();
            }
            return reportTemplateColumn;
        }

        /// <summary>
        /// 加载已经保存的报表模板
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult LoadReportCodes()
        {
            return ExecuteFunctionRun(() =>
            {
                List<ListItem> ReportCodes = new List<ListItem>();
                ReportTemplate[] ReportTemplate = this.Engine.Analyzer.GetReportTemplatesByType(ReportType.Cross);
                if (ReportTemplate != null)
                {
                    foreach (ReportTemplate tmp in ReportTemplate)
                    {
                        ReportCodes.Add(new ListItem(tmp.DisplayName, tmp.Code));
                    }
                }
                return Json(ReportCodes, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除报表模板
        /// </summary>
        /// <param name="templateCode">模板编码</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult DelReportTemplate(string templateCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportTemplate ReportTemplate = this.Engine.Analyzer.GetReportTemplateByCode(templateCode);
                this.Engine.Analyzer.RemoveReportTemplate(ReportTemplate.ObjectID);
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);

            });

        }
        /// <summary>
        /// 加载报表模板数据
        /// </summary>
        /// <param name="ReportSource">报表数据源</param>
        /// <returns>报表模板数据</returns>
        private object LoadSourceData(ReportSource ReportSource)
        {
            Analytics.AnalyticalQuery Query = this.Engine.BPAQuery.GetAnalyticalQuery(this.Engine, ReportSource);
            List<Dictionary<string, object>> objList = new List<Dictionary<string, object>>();
            Dictionary<string, object> obj;
            DataTable dataTable = Query.QueryTable(ReportSource.ExecuteTableName, ReportSource.Columns, null, 1, 5);
            foreach (DataRow row in dataTable.Rows)
            {
                obj = new Dictionary<string, object>();
                foreach (ReportSourceColumn col in ReportSource.Columns)
                {
                    if (col.DataType == DataType.DateTime)//时间类型做转换
                    {
                        obj.Add(col.ColumnCode, row[col.ColumnCode].ToString());
                    }
                    else if (col.ReportSourceColumnType != ReportSourceColumnType.CustomColumn)
                    {
                        obj.Add(col.ColumnCode, row[col.ColumnCode]);
                    }

                }
                objList.Add(obj);
            }
            return new { Rows = objList, Total = objList.Count };
        }

        /// <summary>
        /// 清理列数据
        /// 这个处理很重要，新增的时候，得把所有列的序列化标志全部清空,因为这个表继承了父类，从客户端传过来时，把父类的标示页传过来
        /// </summary>
        private void ClearColumnData(ReportSource ReportSource)
        {
            foreach (ReportSourceColumn Column in ReportSource.Columns)
            {
                Column.ObjectID = Guid.NewGuid().ToString();
                Column.Serialized = false;
            }
        }

        /// <summary>
        /// 优化报表数据源数据项
        /// </summary>
        /// <param name="reportSource">报表数据源</param>
        private void ImproveColumnData(ReportSource reportSource)
        {

            //优化数据源列显示
            foreach (ReportSourceColumn col in reportSource.Columns)
            {
                if (string.IsNullOrEmpty(col.DisplayName))
                    col.DisplayName = col.ColumnCode;

            }
        }
        /// <summary>
        /// 校验保存数据
        /// </summary>
        /// <param name="ReportTemplate"></param>
        /// <returns></returns>
        private ActionResult ValidateSaveData(ReportTemplate ReportTemplate, bool isAdd)
        {
            ActionResult result = new ActionResult();
            result.Success = true;
            List<string> errorMsg = new List<string>();
            if (string.IsNullOrWhiteSpace(ReportTemplate.Code))
            {
                result.Success = false;
                errorMsg.Add("ReportTemplate.Msg1");
            }
            ReportTemplate template = this.Engine.Analyzer.GetReportTemplateByCode(ReportTemplate.Code);//校验编码是否重复
            if (null != template && isAdd)
            {
                result.Success = false;
                errorMsg.Add("msgGlobalString.CodeDuplicate");
            }
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(ReportCodeReg);
            if (!regex.Match(ReportTemplate.Code).Success)
            {
                result.Success = false;
                errorMsg.Add("msgGlobalString.ReportCodeInvalid");

            }
            if (string.IsNullOrWhiteSpace(ReportTemplate.SourceCode))
            {
                result.Success = false;
                errorMsg.Add("ReportTemplate.Msg2");
            }

            if (ReportTemplate.ReportType == ReportType.Summary && (ReportTemplate.Columns == null || ReportTemplate.Columns.Length == 0))
            {
                result.Success = false;
                errorMsg.Add("ReportTemplate.Msg3");
            }

            //交叉分析表校验
            if (ReportTemplate.ReportType == ReportType.Cross)
            {
                if (string.IsNullOrWhiteSpace(ReportTemplate.ColumnTitle) && string.IsNullOrWhiteSpace(ReportTemplate.RowTitle))
                {
                    result.Success = false;
                    errorMsg.Add("ReportTemplate.Msg4");
                }

                if (string.IsNullOrWhiteSpace(ReportTemplate.RowTitle)
                    || string.IsNullOrWhiteSpace(ReportTemplate.ColumnTitle))
                {
                    if (ReportTemplate.Columns == null && ReportTemplate.Columns.Length == 0)
                    {
                        result.Success = false;
                        errorMsg.Add("ReportTemplate.Msg5");
                    }
                }

                if (!string.IsNullOrWhiteSpace(ReportTemplate.RowTitle)
                    && !string.IsNullOrWhiteSpace(ReportTemplate.ColumnTitle))
                {
                    if (ReportTemplate.Columns != null && ReportTemplate.Columns.Length > 1)
                    {
                        result.Success = false;
                        errorMsg.Add("ReportTemplate.Msg6");
                    }
                }

                //判断钻取维度
                if (!string.IsNullOrWhiteSpace(ReportTemplate.DrillCode))
                {
                    if (!string.IsNullOrWhiteSpace(ReportTemplate.RowTitle))
                    {
                        string[] RowTitles = ReportTemplate.RowTitle.Split(';');
                        for (int i = 0; i < RowTitles.Length; i++)
                        {
                            if (RowTitles[i] != ReportTemplate.RowDrillParam[i].Title)
                            {
                                result.Success = false;
                                errorMsg.Add("ReportTemplate.Msg7");
                            }
                        }
                    }

                    if (!string.IsNullOrWhiteSpace(ReportTemplate.ColumnTitle))
                    {
                        if (ReportTemplate.ColumnTitle != ReportTemplate.ColumnDrillParam.Title)
                        {
                            result.Success = false;
                            errorMsg.Add("ReportTemplate.Msg8");
                        }
                    }
                }
            }

            ReportTemplate.SetPropertyDirty(ReportTemplate.PropertyName_AxisUnit);
            ReportTemplate.SetPropertyDirty(ReportTemplate.PropertyName_XAxisUnit);
            if (ReportTemplate.Parameters != null)
            {//这里可以做一些过来参数的校验

            }
            if (!result.Success) result.Extend = errorMsg;
            return result;
        }
    }
}
