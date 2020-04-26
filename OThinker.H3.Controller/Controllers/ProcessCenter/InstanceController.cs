using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using OThinker.H3.Data;
using OThinker.Data.Database;
using OThinker.H3.Instance;
using OThinker.H3.WorkflowTemplate;
using System.Data;
using System.Web.UI.WebControls;
using System.Text;

using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.IO;
using System.Drawing.Design;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 工作任务服务类
    /// </summary>
    [Authorize]
    //[ActionXSSFillters]
    public class InstanceController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public InstanceController()
        {
        }
        public string Test()
        {
            return "SUCCESS";
        }

        #region 服务

        /// <summary>
        /// 组织结构对象
        /// </summary>
        protected OThinker.Organization.IOrganization Organization;
        /// <summary>
        /// 系统权限管理器
        /// </summary>
        protected ISystemAclManager SystemAclManager;
        /// <summary>
        /// 系统权限管理器
        /// </summary>
        protected ISystemOrgAclManager SystemOrgAclManager;
        /// <summary>
        /// 流程模板权限管理器
        /// </summary>
        protected IWorkflowAclManager WorkflowAclManager;
        protected DataModel.IBizObjectManager BizObjectManager;
        protected WorkflowTemplate.IWorkflowManager WorkflowManager;

        #endregion

        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }

        //  所有返回失败信息
        //  ***为多语言编码
        //  result: {Success:false,Message:"***"}


        /// <summary>
        /// 我的流程
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="instanceName"></param>
        /// <param name="workflowCode"></param>
        /// <param name="state"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        public JsonResult QueryMyInstance(PagerInfo pagerInfo, string instanceName, string workflowCode, int state, DateTime? startTime, DateTime? endTime)
        {
            return this.ExecuteFunctionRun(() =>
            {
                InstanceState instanceState = GetInstanceState(state);
                // 查询条件
                string[] conditions = null;

                workflowCode = string.IsNullOrWhiteSpace(workflowCode) ? null : workflowCode;
                // 查找我发起的流程
                conditions = this.Engine.PortalQuery.GetInstanceConditions(
                    new string[] { this.UserValidator.UserID },
                    null,
                    Instance.InstanceContext.UnspecifiedID,
                    instanceState,
                    workflowCode,
                    WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    instanceName,
                    OThinker.H3.Instance.PriorityType.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified);

                DataTable dtInstance = Engine.PortalQuery.QueryInstance(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex);
                int total = Engine.PortalQuery.CountInstance(conditions); // 记录总数
                dtInstance = this.GetParticipant(dtInstance);

                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                List<InstanceViewModel> griddata = this.Getgriddata(dtInstance, columns);
                GridViewModel<InstanceViewModel> result = new GridViewModel<InstanceViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 查询流程实例
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="searchKey">流程名称</param>
        /// <param name="workflowCode"></param>
        /// <param name="unitID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="instaceState"></param>
        /// <returns></returns>
        public JsonResult QueryInstance(PagerInfo pagerInfo, string searchKey, string workflowCode, string unitID, DateTime? startTime, DateTime? endTime, int instaceState)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<InstanceViewModel> griddata = new List<InstanceViewModel>();
                GridViewModel<InstanceViewModel> result = new GridViewModel<InstanceViewModel>(0, griddata, pagerInfo.sEcho);
                // 验证权限
                string[] workflowCodeArray = string.IsNullOrEmpty(workflowCode) ? null : workflowCode.Split(',');
                string[] unitIDArray = string.IsNullOrEmpty(unitID) ? null : unitID.Split(',');
                var queryAcl = this.GetQueryAcl(ref workflowCodeArray, ref unitIDArray);
                if (workflowCodeArray!=null)
                {
                    workflowCode = string.Join(",",workflowCodeArray);
                }
                if (unitIDArray != null)
                {
                    unitID = string.Join(",", unitIDArray);
                }

                if (!queryAcl.Success && (workflowCodeArray == null || workflowCodeArray.Length == 0) && (unitIDArray == null || unitIDArray.Length == 0))
                {
                    result.ExtendProperty = queryAcl;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                InstanceState state = this.GetInstanceState(instaceState);
                string[] conditions = Engine.PortalQuery.GetQueryInstanceConditionsForMonitor(
                    unitID, workflowCode, state,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1)
                    );

                if (!string.IsNullOrEmpty(searchKey))
                {
                    searchKey = searchKey.Replace("'", "").Replace("--", "").Trim();
                    // 添加关键词搜索条件
                    string searchKeyCondition = InstanceContext.TableName + "." + InstanceContext.PropertyName_InstanceName + " LIKE '%" + searchKey + "%'  ";
                    conditions = OThinker.Data.ArrayConvertor<string>.AddToArray(conditions, searchKeyCondition);
                }
                DataTable dtInstance = Engine.PortalQuery.QueryInstance(conditions, pagerInfo.StartIndex, pagerInfo
                    .EndIndex);
                dtInstance = this.GetParticipant(dtInstance);
                //查询流程
                int total = Engine.PortalQuery.CountInstance(conditions); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                griddata = this.Getgriddata(dtInstance, columns);
                result = new GridViewModel<InstanceViewModel>(total, griddata, pagerInfo.sEcho);
                 
                if (!queryAcl.Success)
                {
                    result.ExtendProperty = queryAcl; 
                } 
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 查询超时的流程
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="instanceName">流程名称，为Null时忽略该条件</param>
        /// <param name="workflowCode">流程模板编码，为Null时忽略该条件</param>
        /// <param name="startTime">开始时间，为MinValue时忽略该条件</param>
        /// <param name="endTime">结束时间，为MaxValue时忽略该条件</param>
        /// <param name="instanceState">流程实例状态条件，为Unspecified时忽略该条件</param>
        /// <returns></returns>
        public JsonResult QueryElapsedInstance(PagerInfo pagerInfo, string instanceName, string workflowCode, string participant, DateTime? startTime, DateTime? endTime)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<InstanceViewModel> griddata = new List<InstanceViewModel>();
                GridViewModel<InstanceViewModel> result = new GridViewModel<InstanceViewModel>(0, griddata, pagerInfo.sEcho);
                // 验证权限
                string[] workflowCodeArray = string.IsNullOrEmpty(workflowCode) ? null : workflowCode.Split(',');
                string[] participantArray = string.IsNullOrEmpty(participant) ? null : participant.Split(',');
                var queryAcl = this.GetQueryAcl(ref workflowCodeArray, ref participantArray);
                if (workflowCodeArray != null)
                {
                    workflowCode = string.Join(",", workflowCodeArray);
                }
                if (participantArray != null)
                {
                    participant = string.Join(",", participantArray);
                }

                if (!queryAcl.Success && (workflowCodeArray == null || workflowCodeArray.Length == 0) && (participantArray == null || participantArray.Length == 0))
                {
                    result.ExtendProperty = queryAcl;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string[] conditions = Engine.PortalQuery.GetQueryInstanceConditionsForMonitor(
                    participant,
                    instanceName,
                    workflowCode,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    InstanceState.Running,
                    PriorityType.Unspecified,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    OThinker.Data.BoolMatchValue.True);
                DataTable dtInstance = Engine.PortalQuery.QueryInstance(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex);
                dtInstance = this.GetParticipant(dtInstance);
                int total = Engine.PortalQuery.CountInstance(conditions); // 记录总数
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_OrgUnit };
                griddata = this.Getgriddata(dtInstance, columns);
                result = new GridViewModel<InstanceViewModel>(total, griddata, pagerInfo.sEcho);
                 
                if (!queryAcl.Success)
                {
                    result.ExtendProperty = queryAcl; 
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        #region 导出流程数据
        /// <summary>
        /// 流程模板改变,获得可做查询条件、可导出的数据项
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <returns></returns>
        public JsonResult ChangeWorkflowCode(string WorkflowCode)
        {
            return this.ExecuteFunctionRun(() =>
            {
                List<ListItem> SearcgDataItems = this.GetDataItem(WorkflowCode);
                List<ListItem> ExportDataItems = this.ExportDataItems(WorkflowCode);
                List<ListItem>[] result = new List<ListItem>[] { SearcgDataItems, ExportDataItems };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取流程模板可做查询条件的数据项
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <returns></returns>
        public List<ListItem> GetDataItem(string WorkflowCode)
        {
            this.WorkflowCode = WorkflowCode;
            List<ListItem> DataItem = new List<ListItem>();
            if (this.PropertySchemas != null)
            {
                // 获得所有数据项					
                foreach (PropertySchema item in this.PropertySchemas)
                {
                    if (item != null && !string.IsNullOrEmpty(item.Name))
                    {
                        if (item.Searchable &&
                            item.SerializeMethod == OThinker.Data.Database.Serialization.PropertySerializeMethod.Auto)
                        {
                            string itemTyep = string.Empty;
                            if (OThinker.H3.Data.DataLogicTypeConvertor.IsDateTimeType(item.LogicType))
                            {
                                itemTyep = "DateTime";
                            }
                            else if (OThinker.H3.Data.DataLogicTypeConvertor.IsNumericType(item.LogicType))
                            {
                                itemTyep = "Numeric";
                            }
                            else if (OThinker.H3.Data.DataLogicTypeConvertor.IsStringType(item.LogicType))
                            {
                                itemTyep = "String";
                            }
                            else if (item.LogicType == Data.DataLogicType.SingleParticipant)
                            {
                                itemTyep = "Participant";
                            }
                            else
                            {
                                itemTyep = "Bool";
                            }
                            //只添加标识为可用于搜索的数据项
                            if (item.Searchable)
                            {
                                DataItem.Add(new ListItem(item.DisplayName + "(" + item.LogicType.ToString() + ")", item.Name + "|" + itemTyep));
                            }
                        }
                    }
                }
            }
            return DataItem;
        }

        /// <summary>
        /// 获取流程模板可导出的数据项
        /// </summary>
        /// <returns></returns>
        public List<ListItem> ExportDataItems(string WorkflowCode)
        {
            this.WorkflowCode = WorkflowCode;
            List<ListItem> result = new List<ListItem>();
            if (this.PropertySchemas == null)
            {
                return new List<ListItem>();
            }
            // 将其中的列显示在可选的列中
            foreach (DataModel.PropertySchema item in this.PropertySchemas)
            {
                if (item.Name == DataModel.BizObjectSchema.PropertyName_CreatedBy ||
                    (item.Name != OThinker.Data.Database.Serialization.SerializableObject.C_ObjectID
                        && item.SerializeMethod == OThinker.Data.Database.Serialization.PropertySerializeMethod.Auto))
                {
                    // 暂不支持显示业务对象数组和审批类型的数据项
                    if (item.LogicType != Data.DataLogicType.BizObjectArray &&
                        item.LogicType != Data.DataLogicType.Comment)
                    {
                        string text = item.DisplayName;
                        if (DataModel.BizObjectSchema.IsReservedProperty(item.Name))
                        {
                            switch (item.Name)
                            {
                                case "Name": text = "名称"; break;
                                case "ModifiedTime": text = "修改时间"; break;
                                case "ModifiedBy": text = "修改人"; break;
                                case "CreatedTime": text = "创建时间"; break;
                                case "CreatedBy": text = "创建人"; break;
                                default: break;
                            }
                        }
                        result.Add(new ListItem(text, item.Name.ToLower()));
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 查询导出的流程数据
        /// </summary>
        /// <param name="Originator">发起人</param>
        /// <param name="WorkflowCode">流程模板编码</param>
        /// <param name="Columns">获取显示的字段集合</param>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="SQLString">流程数据项查询条件</param>
        /// <returns></returns>
        public JsonResult QueryInstanceData(PagerInfo PagerInfo,
            string Originator,
            string WorkflowCode,
            string Columns,
            DateTime? StartTime, DateTime? EndTime,
            string Conditions)
        {
            return this.ExecuteFunctionRun(() =>
            {
                //定义返回的数据
                int recordCount = 0;//查询数据总数
                List<Dictionary<string, object>> griddata = new List<Dictionary<string, object>>();//返回查询数据
                GridViewModel<Dictionary<string, object>> result = new GridViewModel<Dictionary<string, object>>(recordCount, griddata, PagerInfo.sEcho);
                // 必须选择流程模板
                if (string.IsNullOrEmpty(WorkflowCode))
                {
                    var extend = new
                    {
                        Success = false,
                        Message = "DataFilter_WorkflowRequired"
                    };
                    result.ExtendProperty = extend;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (!this.UserValidator.ValidateWFInsAdmin(WorkflowCode, Originator))
                {
                    var extend = new
                    {
                        Success = false,
                        Message = "DataFilter_NotEnoughAuth"
                    };
                    result.ExtendProperty = extend;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (Columns == null || Columns == "" || WorkflowCode == null)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                this.WorkflowCode = WorkflowCode;
                string[] chkColumns = Columns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

                //条件数组
                List<string> sqlConditions = this.GetsqlConditions(Originator, WorkflowCode, Conditions, StartTime == null ? DateTime.MinValue : StartTime.Value, EndTime == null ? DateTime.MaxValue : EndTime.Value.AddDays(1));
                WorkflowClause Clause = Engine.WorkflowManager.GetClause(WorkflowCode);
                BizObjectSchema BizObject = Engine.BizObjectManager.GetPublishedSchema(Clause.BizSchemaCode);
                Dictionary<string, string> items = new Dictionary<string, string>();
                //获取要查询的项
                List<string> columns = new List<string>();
                foreach (string column in chkColumns)
                {
                    columns.Add(string.Format("{0}.{1}", BizObject.TableName, column));
                }
                DataTable ReportTable = Engine.PortalQuery.QueryInstanceData(WorkflowCode, BizObject.TableName, columns.ToArray(), sqlConditions.ToArray(), PagerInfo.StartIndex, PagerInfo.EndIndex, out recordCount);
                //获取参与者类型的字段
                List<string> colNames = new List<string>();
                foreach (System.Data.DataColumn col in ReportTable.Columns)
                {
                    if (col.ColumnName.ToLower() == "rownumber_") continue;
                    var ColumnType = BizObject.GetProperty(col.ColumnName).LogicType;//获取列数据类型
                    if (ColumnType == OThinker.H3.Data.DataLogicType.SingleParticipant || ColumnType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                    {
                        colNames.Add(col.ColumnName);
                    }
                }
                //批量获取组织对象的显示名称集合
                Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(ReportTable, colNames.ToArray());

                foreach (DataRow row in ReportTable.Rows)
                {
                    Dictionary<string, object> rowdata = new Dictionary<string, object>();
                    foreach (System.Data.DataColumn col in ReportTable.Columns)
                    {
                        if (col.ColumnName.ToLower() == "rownumber_") continue;
                        //时间，选人
                        var ColumnType = BizObject.GetProperty(col.ColumnName).LogicType;//获取列数据类型
                        var value = "";
                        if (ColumnType == OThinker.H3.Data.DataLogicType.DateTime)
                        {
                            value = row[col] + string.Empty;
                            if (value.IndexOf("1753") > -1)
                            {
                                value = "";
                            }
                        }
                        else if (ColumnType == DataLogicType.SingleParticipant || ColumnType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                        {
                            value = this.GetValueFromDictionary(unitNames, row[col] + string.Empty);
                        }
                        else if (ColumnType == DataLogicType.TimeSpan)
                        {
                            string formatTimeSpan = string.Empty;
                            long itemValue;
                            if (long.TryParse(row[col] + string.Empty, out itemValue))
                            {
                                TimeSpan ts = new TimeSpan(itemValue);
                                formatTimeSpan += ts.Days == 0 ? "" : ts.Days + "天";
                                formatTimeSpan += ts.Hours == 0 ? "" : ts.Hours + "小时";
                                formatTimeSpan += ts.Minutes == 0 ? "" : ts.Minutes + "分钟";
                                formatTimeSpan += ts.Seconds == 0 ? "" : ts.Seconds + "秒";
                            }
                            value = formatTimeSpan;
                        }
                        else
                        {
                            value = row[col] + string.Empty;
                        }
                        var a = col.DataType;
                        rowdata.Add(col.ColumnName.ToLower(), value);
                    }
                    griddata.Add(rowdata);
                }
                result = new GridViewModel<Dictionary<string, object>>(recordCount, griddata, PagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        //导出权限验证
        public JsonResult ExportDataValidateWFInsAdmin(string Originator,
            string WorkflowCode, string Columns)
        {
            ActionResult result = new ActionResult(true);
            // 必须选择流程模板
            if (string.IsNullOrEmpty(WorkflowCode))
            {
                result.Success = false;
                result.Message = "DataFilter_WorkflowRequired";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (string.IsNullOrEmpty(Columns))
            {
                result.Success = false;
                result.Message = "DataFilter_ColumnsRequired";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (!this.UserValidator.ValidateWFInsAdmin(WorkflowCode, Originator))
            {
                result.Success = false;
                result.Message = "DataFilter_NotEnoughAuth";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public Object ExportIntanceData(PagerInfo PagerInfo,
            string Originator,
            string WorkflowCode,
            string Columns,
            DateTime? StartTime, DateTime? EndTime,
            string Conditions)
        {
            return this.ExecuteFileResultFunctionRun(() =>
            {
                //缺少必要条件，不导出
                if (WorkflowCode == null || Columns == null || Columns == "")
                {
                    return null;
                }

                if (!this.UserValidator.ValidateWFInsAdmin(WorkflowCode, Originator))
                {
                    return null;
                }

                this.WorkflowCode = WorkflowCode;
                WorkflowClause Clause = Engine.WorkflowManager.GetClause(WorkflowCode);
                BizObjectSchema BizObject = Engine.BizObjectManager.GetPublishedSchema(Clause.BizSchemaCode);
                string[] chkColumns = Columns.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                //要查询的列
                List<string> columns = new List<string>();
                foreach (string column in chkColumns)
                {
                    columns.Add(string.Format("{0}.{1}", BizObject.TableName, column));
                }

                //搜索条件
                List<string> sqlConditions = this.GetsqlConditions(Originator, WorkflowCode, Conditions, StartTime == null ? DateTime.MinValue : StartTime.Value, EndTime == null ? DateTime.MaxValue : EndTime.Value.AddDays(1));
                //导出数据
                DataTable ReportTable = Engine.PortalQuery.QueryInstanceData(WorkflowCode, BizObject.TableName, columns.ToArray(), sqlConditions.ToArray());

                //要导出的列
                List<DataModel.PropertySchema> ExportColumns = new List<DataModel.PropertySchema>();
                foreach (string columnValue in columns)
                {
                    var value = columnValue.IndexOf(".") > -1 ? columnValue.Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries)[1] : columnValue;
                    DataModel.PropertySchema propertySchema = this.GetClauseDataItem(value);
                    if (propertySchema != null)
                    {
                        ExportColumns.Add(propertySchema);
                    }
                }
                return this.Export(ExportColumns, ReportTable);
            });
        }



        /// <summary>
        /// 返回条件查询语句
        /// </summary>
        /// <param name="Originator"></param>
        /// <param name="WorkflowCode"></param>
        /// <param name="Conditions"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        public List<string> GetsqlConditions(string Originator, string WorkflowCode, string Conditions, DateTime StartTime, DateTime EndTime)
        {
            this.WorkflowCode = WorkflowCode;
            List<string> SqlConditions = new List<string>();
            string tableName = string.Empty;
            BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(this.Workflow.BizObjectSchemaCode);
            if (schema != null)
            {
                tableName = schema.TableName + ".";
            }

            // 校验流程实例的管理权限
            if (!this.UserValidator.ValidateWFInsAdmin(this.WorkflowCode, Originator))
            {
                return SqlConditions;
            }

            // 搜索数据用的条件
            string sqlCondition = null;

            //保存搜索条件
            if (Conditions != null && Conditions.Length > 0)
            {
                string[] conditionList = Conditions.Split(new char[] { ';' });
                for (int count = 0; count < conditionList.Length - 1; count += 4)
                {
                    // 条件显示名称
                    string cellName = conditionList[count] + conditionList[count + 1] + conditionList[count + 2] + conditionList[count + 3];
                    // 条件内容
                    string cellValue = conditionList[count] + ";" + conditionList[count + 1] + ";" + conditionList[count + 2] + ";" + conditionList[count + 3] + ";";
                    //this.lstConditions.Items.Add(new ListItem(cellName, cellValue));
                    // SQL条件
                    string cell = this.GetCondition(conditionList[count + 1], conditionList[count + 2], conditionList[count + 3], tableName);
                    if (sqlCondition != null)
                    {
                        // Connection
                        string connection = conditionList[count];
                        if (connection == null || connection == "" || connection.Contains("OR"))
                        {
                            connection = " OR ";
                        }
                        else
                        {
                            connection = " AND ";
                        }
                        sqlCondition = sqlCondition + connection;
                    }
                    sqlCondition = sqlCondition + cell;
                }
            }
            if (!string.IsNullOrEmpty(sqlCondition))
            {
                SqlConditions.Add(string.Format("({0})", sqlCondition));
            }
            string propertySql = Engine.PortalQuery.GetQueryInstanceSqlForMonitor(
                Originator,
                WorkflowCode,
                OThinker.H3.Instance.InstanceState.Unspecified,
                StartTime,
                EndTime);
            if (!string.IsNullOrEmpty(propertySql))
            {
                SqlConditions.Add(string.Format("{0} IN ({1})", BizObjectSchema.PropertyName_ObjectID, propertySql));
            }
            return SqlConditions;
        }

        /// <summary>
        /// 将匹配条件转换为SQL的条件语句
        /// </summary>
        /// <param name="ColumnName"></param>
        /// <param name="Operator"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        private string GetCondition(string ColumnName, string Operator, string Value, string tableName)
        {
            string comparison = Operator;
            string dataComparison = null;
            if (Operator == "IsNull")
            {
                dataComparison = "is null";
            }
            else if (Operator == "IsNotNull")
            {
                dataComparison = "is not null";
            }
            else if (Operator == "Contain")
            {
                dataComparison = "LIKE '%" + Value.Replace("'", "''") + "%'";
            }
            else if (Operator == "NotContain")
            {
                dataComparison = "NOT LIKE '%" + Value.Replace("'", "''") + "%'";
            }
            else if (Operator == "==")
            {
                Value = Value == "Yes" ? "1" : "0";
                dataComparison = " = " + Value;
            }
            else
            {
                dataComparison = comparison + "'" + Value.Replace("'", "''") + "'";
            }
            return tableName + Database.GetSqlName(this.Engine.EngineConfig.DBType, ColumnName) + " " + dataComparison;
        }

        private string WorkflowCode = "";
        private WorkflowTemplate.PublishedWorkflowTemplate _Workflow = null;
        /// <summary>
        /// 获取流程模板对象
        /// </summary>
        public WorkflowTemplate.PublishedWorkflowTemplate Workflow
        {
            get
            {
                if (_Workflow == null)
                {
                    this._Workflow = this.Engine.WorkflowManager.GetDefaultWorkflow(this.WorkflowCode);
                }
                return this._Workflow;
            }
        }
        public System.Collections.Generic.Dictionary<string, DataModel.PropertySchema> PropertySchemaTable = new System.Collections.Generic.Dictionary<string, DataModel.PropertySchema>();
        private DataModel.PropertySchema[] _PropertySchemas = null;
        private string lastWorkflowCode = null;
        public DataModel.PropertySchema[] PropertySchemas
        {
            get
            {
                if (string.IsNullOrEmpty(this.WorkflowCode))
                {
                    return new PropertySchema[] { };
                }
                if (this._PropertySchemas == null || lastWorkflowCode != this.WorkflowCode)
                {
                    List<DataModel.PropertySchema> listProperty = new List<PropertySchema>();
                    if (this.Workflow != null)
                    {
                        BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(this.Workflow.BizObjectSchemaCode);
                        DataModel.PropertySchema[] properties = schema.Properties;
                        this.PropertySchemaTable.Clear();
                        if (properties != null)
                        {
                            foreach (DataModel.PropertySchema item in properties)
                            {
                                //可查询的不添加进来，界面的可查询就看不到了
                                //if (!OThinker.H3.Data.DataLogicTypeConvertor.IsSearchable(item.LogicType)) continue;
                                listProperty.Add(item);
                                this.PropertySchemaTable.Add(item.Name.ToLower(), item);
                            }
                        }
                        this.lastWorkflowCode = this.WorkflowCode;
                        this._PropertySchemas = listProperty.ToArray();
                    }
                }
                return this._PropertySchemas;
            }
        }

        public WorkflowTemplate.PublishedWorkflowTemplate GetWorkflow(string WorkflowCode)
        {
            return Engine.WorkflowManager.GetDefaultWorkflow(WorkflowCode);
        }

        public DataModel.PropertySchema[] GetPropertySchemas(string WorkflowCode, PublishedWorkflowTemplate Workflow)
        {
            System.Collections.Generic.Dictionary<string, DataModel.PropertySchema> PropertySchemaTable = new System.Collections.Generic.Dictionary<string, DataModel.PropertySchema>();
            if (string.IsNullOrEmpty(this.WorkflowCode))
            {
                return new PropertySchema[] { };
            }

            List<DataModel.PropertySchema> listProperty = new List<PropertySchema>();
            if (Workflow != null)
            {
                BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(Workflow.BizObjectSchemaCode);
                DataModel.PropertySchema[] properties = schema.Properties;
                if (properties != null)
                {
                    foreach (DataModel.PropertySchema item in properties)
                    {
                        listProperty.Add(item);
                        PropertySchemaTable.Add(item.Name, item);
                    }
                }
            }
            return listProperty.ToArray();
        }


        public DataModel.PropertySchema GetClauseDataItem(string ColumnValue)
        {
            if (ColumnValue == null ||
                // 调用这句话将初始化this.ClauseDataItemTable
                this.PropertySchemas == null ||
                this.PropertySchemaTable == null ||
                !this.PropertySchemaTable.ContainsKey(ColumnValue))
            {
                return null;
            }
            else
            {
                return this.PropertySchemaTable[ColumnValue];
            }
        }
        #endregion

        #region 将datatable 导出Excel
        protected FileResult Export(List<PropertySchema> Columns, DataTable ReportTable)
        {
            //导出用的DataTable
            DataTable dtTemp = new DataTable();
            if (ReportTable != null)
            {
                foreach (System.Data.DataColumn column in ReportTable.Columns)
                {
                    if (Columns.Exists(i => i.Name.ToUpper() == column.ColumnName.ToUpper()))
                    {
                        dtTemp.Columns.Add(column.ColumnName);
                    }
                }
                //出于性能考虑，找出dtTemp中类型为SingleParticipant的列，根据该列的id值找出对应的name
                List<string> participants = new List<string>();
                foreach (System.Data.DataColumn col in dtTemp.Columns)
                {
                    DataModel.PropertySchema propertySchema = Columns.First(i => i.Name.ToUpper() == col.ColumnName.ToUpper());
                    if (propertySchema != null && propertySchema.LogicType == DataLogicType.SingleParticipant)
                    {
                        foreach (DataRow row in ReportTable.Rows)
                        {
                            string participant = row[col.ColumnName].ToString();
                            if (!string.IsNullOrEmpty(participant) && !participants.Contains(participant))
                            {
                                participants.Add(participant);
                            }
                        }
                    }
                }

                Dictionary<string, string> participantDic = Engine.Organization.GetNames(participants.ToArray());
                for (int i = 0; i < ReportTable.Rows.Count; i++)
                {
                    DataRow dr = dtTemp.NewRow();
                    foreach (System.Data.DataColumn col in dtTemp.Columns)
                    {
                        string colName = col.ColumnName;
                        string colValue = ReportTable.Rows[i][colName].ToString();
                        DataLogicType logicType = Columns.First(c => c.Name.ToUpper() == colName.ToUpper()).LogicType;
                        if (logicType == DataLogicType.SingleParticipant)
                        {
                            if (participantDic.ContainsKey(colValue))
                            {
                                dr[colName] = participantDic[colValue];
                            }
                        }
                        else if (logicType == DataLogicType.DateTime)
                        {
                            if (colValue.IndexOf("1753") > -1)
                            {
                                colValue = "";
                            }
                            dr[colName] = colValue;
                        }
                        else if (logicType == DataLogicType.TimeSpan)
                        {
                            string formatTimeSpan = string.Empty;
                            long itemValue;
                            if (long.TryParse(colValue, out itemValue))
                            {
                                TimeSpan ts = new TimeSpan(itemValue);
                                formatTimeSpan += ts.Days == 0 ? "" : ts.Days + "天";
                                formatTimeSpan += ts.Hours == 0 ? "" : ts.Hours + "小时";
                                formatTimeSpan += ts.Minutes == 0 ? "" : ts.Minutes + "分钟";
                                formatTimeSpan += ts.Seconds == 0 ? "" : ts.Seconds + "秒";
                            }
                            dr[colName] = formatTimeSpan;
                        }
                        else
                        {
                            dr[colName] = colValue;
                        }
                    }
                    dtTemp.Rows.Add(dr);
                }
                return DataTableToExcelWithNPOI(dtTemp, Columns);
            }
            else
            {
                return null;
            }
        }

        public FileResult DataTableToExcelWithNPOI(DataTable dtData, List<DataModel.PropertySchema> Columns)
        {
            IWorkbook workbook = null;
            ISheet sheet = null;
            try
            {
                workbook = new HSSFWorkbook();
                sheet = workbook.CreateSheet();
                // 生成标题行
                IRow row = sheet.CreateRow(0);
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    string text = Columns.First(c => c.Name.ToUpper() == dtData.Columns[i].ColumnName.ToUpper()).DisplayName;
                    if (DataModel.BizObjectSchema.IsReservedProperty(text))
                    {
                        switch (text)
                        {
                            case "Name": text = "名称"; break;
                            case "ModifiedTime": text = "修改时间"; break;
                            case "ModifiedBy": text = "修改人"; break;
                            case "CreatedTime": text = "创建时间"; break;
                            case "CreatedBy": text = "创建人"; break;
                            default: break;
                        }
                    }
                    row.CreateCell(i).SetCellValue(text);
                }
                // 生成数据行
                for (int i = 0; i < dtData.Rows.Count; i++)
                {
                    row = sheet.CreateRow(i + 1);
                    for (int j = 0; j < dtData.Columns.Count; j++)
                    {
                        row.CreateCell(j).SetCellValue(dtData.Rows[i][j].ToString());
                    }
                }
                //自动调整列宽
                for (int i = 0; i < dtData.Columns.Count; i++)
                {
                    sheet.AutoSizeColumn(i);
                }
                string FileName = DateTime.Now.ToString("yyyyMMddHHmmss");
                //导出excel
                //Response.ContentType = "application/ms-excel";
                //Response.Charset = "";
                //Response.AppendHeader("content-disposition", "attachment;filename=\"" + System.Web.HttpUtility.UrlEncode(FileName, System.Text.Encoding.UTF8) + ".xls");
                //using (MemoryStream ms = new MemoryStream())
                //{
                MemoryStream ms = new MemoryStream();
                workbook.Write(ms);
                ms.Position = 0;
                Response.BinaryWrite(ms.ToArray());
                FileStreamResult result = new FileStreamResult(ms, "application/octect-stream");
                result.FileDownloadName = FileName + ".xls";
                return result;
                // }
            }
            catch
            {
                return null;
            }
        }
        #endregion

        /// <summary>
        /// 根据查询流程实例表，获取审批环节，审批人，审批人所属组织
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public DataTable GetParticipant(DataTable dtInstance)
        {
            if (dtInstance.Rows.Count == 0) return dtInstance;
            List<string> instanceID = new List<string>();
            foreach (DataRow row in dtInstance.Rows)
            {
                instanceID.Add(row[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty);
            }
            DataTable dtWorkItem = Engine.PortalQuery.GetActiveNamebyInstanceId(instanceID);
            //获取审批人所属组织的集合
            Dictionary<string, string> ParticipantOuDept = this.GetUnitNamesFromTable(
                dtWorkItem,
                new string[] { WorkItem.WorkItem.PropertyName_OrgUnit });

            //获取流程实例的当前审批环节,审批人,审批人所属组织
            Dictionary<string, string> ApproverLink = new Dictionary<string, string>();
            Dictionary<string, string> Approver = new Dictionary<string, string>();
            Dictionary<string, string> ApproverOuDept = new Dictionary<string, string>();
            foreach (DataRow row in dtWorkItem.Rows)
            {
                string instanceid = (row[WorkItem.WorkItem.PropertyName_InstanceId] + string.Empty).Trim();
                string approverlink = ((row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty) != "" ? (row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty) : (row[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty)).Trim();
                string participantname = (row[WorkItem.WorkItem.PropertyName_ParticipantName] + string.Empty).Trim();
                string participantOUname = this.GetValueFromDictionary(ParticipantOuDept,
                        row[WorkItem.WorkItem.PropertyName_OrgUnit] + string.Empty);

                //审批人
                if (Approver.ContainsKey(instanceid))
                {
                    if (Approver[instanceid].IndexOf(participantname) == -1)
                        Approver[instanceid] = Approver[instanceid] + "," + participantname;
                }
                else
                {
                    Approver[instanceid] = participantname;
                }
                //审批环节
                if (ApproverLink.ContainsKey(instanceid))
                {
                    if (ApproverLink[instanceid].IndexOf(approverlink) == -1)
                        ApproverLink[instanceid] = ApproverLink[instanceid] + "," + approverlink;
                }
                else
                {
                    ApproverLink[instanceid] = approverlink;
                }
                //审批人所属组织
                if (ApproverOuDept.ContainsKey(instanceid))
                {
                    if (ApproverOuDept[instanceid].IndexOf(participantOUname) == -1)
                        ApproverOuDept[instanceid] = ApproverOuDept[instanceid] + "," + participantOUname;
                }
                else
                {
                    ApproverOuDept[instanceid] = participantOUname;
                }
            }

            if (dtWorkItem.Rows.Count > 0)
            {
                for (int i = 0; i < dtInstance.Rows.Count; i++)
                {
                    try
                    {
                        var instanceid = dtInstance.Rows[i][InstanceContext.PropertyName_ObjectID] + string.Empty;
                        dtInstance.Rows[i]["Approver"] = Approver[instanceid];
                        dtInstance.Rows[i]["ApproverLink"] = ApproverLink[instanceid];
                        dtInstance.Rows[i]["ApproverOuDept"] = ApproverOuDept[instanceid];
                    }
                    catch { }
                }
            }
            return dtInstance;

        }

        /// <summary>
        /// 获取返回到前端的InstanceViewModel
        /// </summary>
        /// <param name="dtInstance"></param>
        /// <returns></returns>
        protected List<InstanceViewModel> Getgriddata(DataTable dtInstance, string[] columns)
        {
            Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtInstance, columns);
            List<InstanceViewModel> griddata = new List<InstanceViewModel>();
            foreach (DataRow row in dtInstance.Rows)
            {
                griddata.Add(new InstanceViewModel()
                {
                    Priority = row[InstanceContext.PropertyName_Priority] + string.Empty,
                    InstanceID = row[InstanceContext.PropertyName_InstanceId] + string.Empty,
                    InstanceName = string.IsNullOrEmpty((row[InstanceContext.PropertyName_InstanceName] +string.Empty)) ? (row[WorkflowClause.PropertyName_WorkflowName] + string.Empty) : (row[InstanceContext.PropertyName_InstanceName].ToString()),

                    WorkflowName = row[WorkflowClause.PropertyName_WorkflowName] + string.Empty,
                    WorkflowCode = row[InstanceContext.PropertyName_WorkflowCode] + string.Empty,
                    Originator = row[InstanceContext.PropertyName_Originator] + string.Empty,
                    OriginatorName = row[InstanceContext.PropertyName_OriginatorName] + string.Empty,
                    OriginatorOUName = this.GetValueFromDictionary(unitNames,
                            row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty),
                    InstanceState = row[InstanceContext.PropertyName_State] + string.Empty,
                    CreatedTime = this.GetValueFromDate(row[InstanceContext.PropertyName_CreatedTime] + string.Empty, WorkItemTimeFormat),
                    PlanFinishTime = this.GetValueFromDate(row[InstanceContext.PropertyName_PlanFinishTime] + string.Empty, WorkItemTimeFormat),
                    FinishedTime = this.GetValueFromDate(row[InstanceContext.PropertyName_FinishTime] + string.Empty, WorkItemTimeFormat),
                    stayTime = System.DateTime.Now.Subtract(this.GetTime(row[InstanceContext.PropertyName_PlanFinishTime] + string.Empty)),
                    ApproverLink = row["ApproverLink"] + string.Empty,
                    Approver = row["Approver"] + string.Empty,
                    ApproverOuDept = row["ApproverOuDept"] + string.Empty,
                    Exceptional = row[InstanceContext.PropertyName_Exceptional] + string.Empty == "1" ? true : false
                });
            }
            return griddata;
        }

        /// <summary>
        /// 获取查询流程的状态
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public InstanceState GetInstanceState(int value)
        {
            InstanceState state = new InstanceState();
            switch (value)
            {
                case 0: state = InstanceState.Unfinished; break;
                case 1: state = InstanceState.Finished; break;
                case 2: state = InstanceState.Canceled; break;
                default: state = InstanceState.Unspecified; break;
            }
            return state;
        }


        #region 已弃用
        ///// <summary>
        ///// 查询进行中的流程
        ///// </summary>
        ///// <param name="pagerInfo"></param>
        ///// <param name="instanceName"></param>
        ///// <param name="workflowCode"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public JsonResult QueryUnfinishedInstance(PagerInfo pagerInfo, string instanceName, string workflowCode, DateTime? startTime, DateTime? endTime)
        //{
        //    return this.ExecuteFunctionRun(() =>
        //    {
        //        return this.QueryInstance(pagerInfo, instanceName, workflowCode,
        //            startTime == null ? DateTime.MinValue : startTime.Value,
        //            endTime == null ? DateTime.MaxValue : endTime.Value,
        //        InstanceState.Unspecified, PriorityType.Unspecified, OThinker.Data.BoolMatchValue.Unspecified, OThinker.Data.BoolMatchValue.Unspecified);
        //    });
        //}

        ///// <summary>
        ///// 查询异常的流程
        ///// </summary>
        ///// <param name="pagerInfo"></param>
        ///// <param name="instanceName"></param>
        ///// <param name="workflowCode"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <returns></returns>
        //public JsonResult QueryExceptionInstance(PagerInfo pagerInfo, string instanceName, string workflowCode, DateTime? startTime, DateTime? endTime)
        //{
        //    return this.ExecuteFunctionRun(() =>
        //    {
        //        return this.QueryInstance(pagerInfo, instanceName, workflowCode,
        //            startTime == null ? DateTime.MinValue : startTime.Value,
        //            endTime == null ? DateTime.MaxValue : endTime.Value,
        //            InstanceState.Exceptional, PriorityType.Unspecified, OThinker.Data.BoolMatchValue.Unspecified, OThinker.Data.BoolMatchValue.Unspecified);
        //    });
        //}


        #endregion
        // End Class
    }
}