using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.BPA
{
    /// <summary>
    /// 报表数据源控制器
    /// </summary>
    [Authorize]
    public class ReportSourceController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码:报表数据源
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.BPA_ReportSource_Code; }
        }

        /// <summary>
        /// 更改数据来源类型
        /// </summary>
        /// <param name="sorceType">来源类型</param>
        /// <returns>数据库列表项 或 业务系统列表</returns>
        [HttpPost]
        public JsonResult SorceTypeChange(string sorceType)
        {
            return ExecuteFunctionRun(() =>
            {
                List<ListItem> items = new List<ListItem>();
                ReportSourceType SorceType = (ReportSourceType)Enum.Parse(typeof(ReportSourceType), sorceType ?? ReportSourceType.H3System.ToString());
                switch (SorceType)
                {
                    case ReportSourceType.H3System:
                        DataModel.BizObjectSchema[] schemas = this.Engine.BizObjectManager.GetPublishedSchemas();
                        FunctionNode[] packages = this.Engine.FunctionAclManager.GetFunctionNodesByNodeType(FunctionNodeType.BizWorkflowPackage);
                        foreach (DataModel.BizObjectSchema schema in schemas)
                        {
                            for (int i = 0, j = packages.Length; i < j; i++)
                            {
                                if (packages[i].Code.Equals(schema.SchemaCode, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    items.Add(new ListItem(packages[i].DisplayName, schema.SchemaCode + "@" + schema.TableName));
                                }
                            }
                        }
                        break;
                    case ReportSourceType.DbConnection:
                        BizDbConnectionConfig[] configs = this.Engine.SettingManager.GetBizDbConnectionConfigList();
                        foreach (BizDbConnectionConfig config in configs)
                        {
                            items.Add(new ListItem(config.DisplayName, config.DbCode));
                        }
                        break;
                }
                return Json(items, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取数据表数组
        /// </summary>
        /// <param name="dbCode">业务数据库编号</param>
        /// <returns>所有表的名称</returns>
        [HttpPost]
        public JsonResult LoadTables(string dbCode)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] tableNames = this.Engine.SettingManager.GetBizDbTableNames(dbCode);
                return Json(tableNames, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获得业务数据库里的所有视图名称
        /// </summary>
        /// <param name="dbCode">业务数据库编号</param>
        /// <returns>所有视图的名称</returns>
        [HttpPost]
        public JsonResult LoadViews(string dbCode)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] viewNames = this.Engine.SettingManager.GetBizDbViewNames(dbCode);
                return Json(viewNames, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取业务数据库指定表名的所有列
        /// </summary>
        /// <param name="reportSourceSetting">报表数据源信息</param>
        /// <returns>指定表的所有列的信息</returns>
        [HttpPost]
        public JsonResult LoadCloumnData(string reportSourceSetting)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                try
                {
                    ReportSource ReportSource = JsonConvert.DeserializeObject<ReportSource>(reportSourceSetting);
                    result.Success = true;
                    result.Extend = GetOriginalColumnDataBySetting(ReportSource); ;
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取报表数据源数据
        /// </summary>
        /// <param name="sourceCode">数据源编码</param>
        /// <returns>报表数据源数据</returns>
        [HttpPost]
        public JsonResult LoadSourceSetting(string sourceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (string.IsNullOrEmpty(sourceCode))
                {
                    result.Success = false;
                    result.Extend = "ReportSource.CodeNotExist";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                ReportSource reportSource = this.Engine.Analyzer.GetReportSourceByCode(sourceCode);
                List<ReportSourceColumn> OriginalColumnData = GetOriginalColumnDataBySetting(reportSource);
                var sourceObj = new { ReportSource = reportSource, OriginalColumnData = OriginalColumnData };
                result.Success = true;
                result.Extend = sourceObj;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 添加报表数据源
        /// </summary>
        /// <param name="reportSourceSetting">报表数据源</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult AddReportSourceSetting(string reportSourceSetting)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportSource ReportSource = JsonConvert.DeserializeObject<ReportSource>(reportSourceSetting);
                //校验
                result = ValidateSaveData(ReportSource);
                if (result.Success)
                {
                    result.Success = this.Engine.Analyzer.AddReportSource(ReportSource);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 更新报表数据源
        /// </summary>
        /// <param name="reportSourceSetting">报表数据源</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult UpdateReportSourceSetting(string reportSourceSetting)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ReportSource ReportSource = JsonConvert.DeserializeObject<ReportSource>(reportSourceSetting);
                result = CheckReportSource(ReportSource.Code);
                if (result.Success)
                {
                    foreach (ReportSourceColumn col in ReportSource.Columns)
                    {
                        col.SetPropertyDirty(ReportSourceColumn.PropertyName_DataRule);
                        col.SetPropertyDirty(ReportSourceColumn.PropertyName_DisplayName);
                        col.SetPropertyDirty(ReportSourceColumn.PropertyName_IsOrderColumn);
                        col.SetPropertyDirty(ReportSourceColumn.PropertyName_Ascending);
                        col.SetPropertyDirty(ReportSourceColumn.PropertyName_ColumnName);
                    }
                    result.Success = this.Engine.Analyzer.UpdateReportSource(ReportSource);
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 检查报表数据源是否被引用
        /// </summary>
        /// <param name="ReportSource">报表数据源</param>
        /// <returns>验证结果</returns>
        private ActionResult CheckReportSource(string code)
        {
            ActionResult result = new ActionResult(true);
            ReportTemplate[] reportTemplates = this.Engine.Analyzer.GetAllReportTemplates();
            foreach (var s in reportTemplates)
            {
                if (s.SourceCode == code)
                {
                    result.Extend += s.DisplayName + ";";
                }
            }
            if (result.Extend != null)
            {
                result.Success = false;
                result.Message = "ReportSource.ReportTemplateExits";
            }
            return result;
        }

        /// <summary>
        /// 校验Sql语句
        /// </summary>
        /// <param name="sqlData">sql命令</param>
        /// <param name="dbCode">数据库配置编码</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult SqlValidation(string sqlData, string dbCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                Regex reg = new Regex("/order\\s+by/");
                if (string.IsNullOrWhiteSpace(sqlData))
                {
                    result.Success = false;
                }
                else if (reg.IsMatch(sqlData.ToLower()))
                {
                    result.Success = false;
                }
                else
                {
                    try
                    {
                        BizDbConnectionConfig dbConfig = this.Engine.SettingManager.GetBizDbConnectionConfig(dbCode);
                        Analytics.AnalyticalQuery Query = new Analytics.AnalyticalQuery(dbConfig.DbType, dbConfig.DbConnectionString);
                        Query.CommandFactory.CreateCommand().ExecuteDataTable(sqlData);
                        result.Success = true;
                    }
                    catch
                    {
                        result.Success = false;
                    }
                }

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除报表数据源
        /// </summary>
        /// <param name="sourceCode">数据源编码</param>
        /// <returns>ActionResult</returns>
        [HttpPost]
        public JsonResult DelReportSource(string sourceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = CheckReportSource(sourceCode);
                if (result.Success)
                {
                    ReportSource ReportSource = this.Engine.Analyzer.GetReportSourceByCode(sourceCode);
                    result.Success = this.Engine.Analyzer.RemoveReportSource(ReportSource.ObjectID);
                }
                return Json(result, JsonRequestBehavior.AllowGet);

            });

        }

        /// <summary>
        /// 校验保存数据
        /// </summary>
        /// <param name="ReportSource"></param>
        /// <returns></returns>
        private ActionResult ValidateSaveData(ReportSource ReportSource)
        {
            ActionResult actionResult = new ActionResult();
            actionResult.Success = true;

            if (string.IsNullOrWhiteSpace(ReportSource.Code))
            {
                actionResult.Message = "ReportSource.Msg0";
                actionResult.Success = false;
            }
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(ReportCodeReg);
            if (!regex.Match(ReportSource.Code).Success)
            {
                actionResult.Success = false;
                actionResult.Message = "msgGlobalString.ReportCodeInvalid";

            }
            if (string.IsNullOrEmpty(ReportSource.DisplayName))
            {
                actionResult.Success = false;
                actionResult.Message = "msgGlobalString.DisplayNameNotNull";
            }
            if (ReportSource.ReportSourceType == ReportSourceType.H3System)
            {
                if (string.IsNullOrWhiteSpace(ReportSource.SchemaCode))
                {
                    actionResult.Message = "ReportSource.Msg1";
                    actionResult.Success = false;
                }
            }
            else if (string.IsNullOrWhiteSpace(ReportSource.DbCode))
            {
                actionResult.Message = "ReportSource.Msg2";
                actionResult.Success = false;
            }

            if (string.IsNullOrWhiteSpace(ReportSource.TableNameOrCommandText))
            {
                actionResult.Message = "ReportSource.Msg3";
                actionResult.Success = false;
            }

            //判断字段信息的编码是否重复
            List<string> ColumnCodes = new List<string>();
            List<string[]> errorInfo = new List<string[]>();
            foreach (ReportSourceColumn col in ReportSource.Columns)
            {
                if (ColumnCodes.Contains(col.ColumnCode))
                {
                    actionResult.Message = "ReportSource.Msg4";
                    string[] errorArr = { col.ColumnCode, col.DisplayName };
                    errorInfo.Add(errorArr);
                }
                else
                {
                    ColumnCodes.Add(col.ColumnCode);
                }
                if (errorInfo.Count != 0)
                {
                    actionResult.Success = false;
                    actionResult.Extend = errorInfo;
                }
            }

            //新增校验编码是否重复
            if (this.Engine.Analyzer.GetReportSourceByCode(ReportSource.Code) != null)
            {
                actionResult.Message = "ReportSource.Msg5";
                actionResult.Success = false;
            }

            if (this.Engine.FunctionAclManager.GetFunctionNodeByCode(ReportSource.Code) != null)
            {
                actionResult.Message = "ReportSource.Msg6";
                actionResult.Success = false;
            }
            return actionResult;
        }

        /// <summary>
        /// 获取数据模型的列
        /// </summary>
        /// <param name="Schema"></param>
        /// <returns></returns>
        private List<ReportSourceColumn> CreateColumnsBySchema(DataModel.BizObjectSchema Schema, string parentName = "")
        {
            List<ReportSourceColumn> ReportSourceColumns = new List<ReportSourceColumn>();
            if (Schema.Properties != null)
            {
                for (int i = 0, j = Schema.Properties.Length; i < j; i++)
                {
                    DataModel.PropertySchema p = Schema.Properties[i];

                    if (p.LogicType == Data.DataLogicType.Comment
                        || p.LogicType == Data.DataLogicType.Attachment
                        || p.LogicType == Data.DataLogicType.BizObject
                        || p.LogicType == Data.DataLogicType.BoolArray
                        || p.LogicType == Data.DataLogicType.ByteArray
                        || p.LogicType == Data.DataLogicType.BizObjectArray
                        )
                    {
                        continue;
                    }
                    DataType DataType = DataType.String;
                    switch (p.LogicType)
                    {
                        case DataLogicType.Decimal:
                        case DataLogicType.Int:
                        case DataLogicType.Double:
                        case DataLogicType.Long:
                            DataType = DataType.Numeric;
                            break;
                        case DataLogicType.DateTime:
                            DataType = DataType.DateTime;
                            break;
                        default:
                            DataType = DataType.String;
                            break;
                    }

                    ReportSourceColumns.Add(new ReportSourceColumn()
                    {
                        ColumnCode = p.Name,
                        ColumnName = p.Name,
                        DisplayName = p.DisplayName,
                        ReportSourceColumnType = ReportSourceColumnType.TableColumn,
                        //DataRuleText = GetPropertySchemaRule(p),
                        DataType = DataType,
                        FunctionType = Function.Sum
                    });
                }
            }
            return ReportSourceColumns;
        }

        /// <summary>
        /// 读取配置最原始的的列
        /// </summary>
        /// <param name="ReportSource"></param>
        /// <returns></returns>
        private List<ReportSourceColumn> GetOriginalColumnDataBySetting(ReportSource ReportSource)
        {
            List<ReportSourceColumn> ReportSourceColumns = new List<ReportSourceColumn>();
            if (ReportSource.ReportSourceType == ReportSourceType.H3System && !string.IsNullOrWhiteSpace(ReportSource.SchemaCode))
            {//数据模型
                DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(ReportSource.SchemaCode);
                if (schema != null) ReportSourceColumns.AddRange(CreateColumnsBySchema(schema));
            }
            else
            {//业务数据配置
                //BizDbConnectionConfig DBConfig = this.Engine.SettingManager.GetBizDbConnectionConfig(ReportSource.DbCode);
                Analytics.AnalyticalQuery Query = this.Engine.BPAQuery.GetAnalyticalQuery(this.Engine, ReportSource);
                if (null != Query) ReportSourceColumns = Query.GetTableColumns(ReportSource.TableNameOrCommandText, ReportSource.DataSourceType);
            }

            return ReportSourceColumns;
        }

    }
}
