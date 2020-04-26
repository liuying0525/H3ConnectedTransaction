using OThinker.H3;
using OThinker.H3.Acl;
using OThinker.H3.Controllers;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.WorkItem;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class WorkItemHandlerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";
        
        public string Index(string CommandName)
        {
            HttpContextBase context = HttpContext;
            string result = "";
            switch (CommandName)
            {
                case "getLastWorkItem": result = getLastWorkItem(context); break;//获取上一条任务的详细信息，第一条不返回
                case "getOverTimeInfo": result = getOverTimeInfo(context); break;//获取加班的详细信息
                case "getLeaveOverTimeInfo": result = getLeaveOverTimeInfo(context); break;//获取调休加班的详细信息
            }
            return result;
        }

        //public JsonResult GetUnfinishWorkItemsNew(PagerInfo pagerInfo, string keyWord, string startDate, string endDate, string userType, string itemPriority)
        //{
        //    return this.ExecuteFunctionRun(() =>
        //    {
        //        //string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, keyWord, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.WorkItem.TableName);
        //        //string OrderBy = "ORDER BY " +
        //        //     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," +
        //        //     WorkItem.WorkItem.TableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC";
        //        //DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, WorkItem.WorkItem.TableName);
        //        //int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItem.TableName); // 记录总数

        //        int total = 0;
        //        DataTable dtWorkitem = QueryWorkItemCustom(pagerInfo, keyWord, startDate, endDate, userType, itemPriority, ref total);

        //        string[] columns = new string[] { WorkItem.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };
        //        List<WorkItemViewModel> griddata = Getgriddata(dtWorkitem, columns);
        //        GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);
        //        return Json(result, JsonRequestBehavior.AllowGet);
        //    }, FunctionNode.Workspace_MyUnfinishedWorkItem_Code);
        //}

        /// <summary>
        /// 获取返回到前端的WorkItemViewModel
        /// </summary>
        /// <param name="dtWorkItem"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<WorkItemViewModel> Getgriddata(DataTable dtWorkItem, string[] columns)
        {
            //
            string InstanceName = string.Empty;
            //批量获取组织架构显示名称
            Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtWorkItem, columns);
            //批量获取流程发起人的OU显示名称
            Dictionary<string, string> orgOUNames = this.GetParentUnitNamesFromTable(dtWorkItem,
                new string[] { WorkItem.PropertyName_Originator });
            //批量获取流程模板名称WorkflowName
            Dictionary<string, string> workflowNames = this.GetWorkflowNamesFromTable(dtWorkItem);
            List<WorkItemViewModel> griddata = new List<WorkItemViewModel>();
            foreach (DataRow row in dtWorkItem.Rows)
            {
                if (string.IsNullOrEmpty(row[InstanceContext.PropertyName_InstanceName] + string.Empty))
                {
                    var workItem = this.Engine.WorkItemManager.GetWorkItem(this.GetColumnsValue(row, WorkItem.PropertyName_ObjectID));
                    InstanceName = this.Engine.WorkflowManager.GetClauseDisplayName(workItem.WorkflowCode);
                }
                else
                {
                    InstanceName = row[InstanceContext.PropertyName_InstanceName] + string.Empty;
                }
                griddata.Add(new WorkItemViewModel()
                {
                    Priority = this.GetColumnsValue(row, InstanceContext.PropertyName_Priority),
                    Urged = this.GetColumnsValue(row, WorkItem.PropertyName_Urged).ToString() == "1" ? true : false,
                    CirculateCreator = this.GetColumnsValue(row, CirculateItem.PropertyName_Creator),
                    CirculateCreatorName = this.GetColumnsValue(row, CirculateItem.PropertyName_CreatorName),
                    ObjectID = this.GetColumnsValue(row, WorkItem.PropertyName_ObjectID),
                    InstanceId = row[WorkItem.PropertyName_InstanceId] + string.Empty,
                    InstanceName = InstanceName,
                    WorkflowCode = row[WorkItem.PropertyName_WorkflowCode] + string.Empty,
                    WorkflowName = this.GetValueFromDictionary(workflowNames,
                        row[InstanceContext.PropertyName_WorkflowCode] + string.Empty),
                    ActivityCode = row[WorkItem.PropertyName_ActivityCode] + string.Empty,
                    DisplayName = row[WorkItem.PropertyName_DisplayName] + string.Empty,
                    ReceiveTime = this.GetValueFromDate(row[WorkItem.PropertyName_ReceiveTime], WorkItemTimeFormat),
                    PlanFinishTime = this.GetValueFromDate(row[WorkItem.PropertyName_PlanFinishTime], WorkItemTimeFormat),
                    FinishTime = this.GetValueFromDate(row[WorkItem.PropertyName_FinishTime], WorkItemTimeFormat),
                    StayTime = System.DateTime.Now.Subtract(this.GetTime(row[InstanceContext.PropertyName_PlanFinishTime] + string.Empty)),
                    Participant = row[WorkItem.PropertyName_Participant] + string.Empty,
                    ParticipantName = row[WorkItem.PropertyName_ParticipantName] + string.Empty,
                    Originator = row[WorkItem.PropertyName_Originator] + string.Empty,
                    OriginatorName = row[InstanceContext.PropertyName_OriginatorName] + string.Empty,
                    OriginatorOUName = this.GetValueFromDictionary(orgOUNames,
                        row[WorkItem.PropertyName_Originator] + string.Empty),
                    ItemSummary = row[WorkItem.PropertyName_ItemSummary] + string.Empty
                });
            }
            return griddata;
        }

        public string GetColumnsValue(DataRow row, string columns)
        {
            return row.Table.Columns.Contains(columns) ? row[columns] + string.Empty : "";
        }

        public DataTable QueryWorkItemCustom(PagerInfo pagerInfo, string keyWord, string startDate, string endDate, string userType, string ItemPriority, ref int totalNumber)
        {
            //userType:0表示内网用户,1表示外网用户
            string OrderBy = "ORDER BY ";
            string Conditions = "";//其它查询条件
            string LeftUserTable = "";//是否需要关联用户表
            #region 排序条件
            if (pagerInfo.iSortCol_0 == 1)
            {
                OrderBy += InstanceContext.TableName + "." + InstanceContext.PropertyName_InstanceName + " " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 2)
            {
                OrderBy += WorkItem.TableName + "." + WorkItem.PropertyName_DisplayName + " " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 3)
            {
                OrderBy += WorkItem.TableName + "." + WorkItem.PropertyName_ReceiveTime + " " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 4)
            {
                OrderBy += WorkItem.TableName + "." + WorkItem.PropertyName_Originator + " " + pagerInfo.sSortDir_0.ToUpper();
            }
            else if (pagerInfo.iSortCol_0 == 5)
            {
                OrderBy += WorkItem.TableName + "." + WorkItem.PropertyName_OrgUnit + " " + pagerInfo.sSortDir_0.ToUpper();
            }
            else //任务优先级反过来排序（默认是顺序）
            {
                if (pagerInfo.sSortDir_0 == "asc")
                {
                    OrderBy += WorkItem.TableName + "." + WorkItem.PropertyName_Priority + " DESC," +
                    WorkItem.TableName + "." + WorkItem.PropertyName_ReceiveTime + " DESC";
                }
                else
                {
                    OrderBy += WorkItem.TableName + "." + WorkItem.PropertyName_Priority + " ASC," +
                    WorkItem.TableName + "." + WorkItem.PropertyName_ReceiveTime + " ASC";
                }
            }
            #endregion

            #region 其它查询条件
            if (!string.IsNullOrEmpty(startDate))//接收时间的开始时间范围
            {
                Conditions += string.Format("AND OT_WorkItem.ReceiveTime>to_date('{0}','yyyy-mm-dd')\r\n", startDate);
            }
            if (!string.IsNullOrEmpty(endDate))//接收时间的结束时间范围
            {
                Conditions += string.Format("AND OT_WorkItem.ReceiveTime<to_date('{0}','yyyy-mm-dd')\r\n", endDate);
            }
            if (!string.IsNullOrEmpty(userType))//内网（98开始的用户以及80000开始的用户），其它为外网用户
            {
                LeftUserTable = "LEFT JOIN OT_User ON OT_InstanceContext.Originator=OT_User.ObjectID\r\n";
                if (userType == "0")
                    Conditions += "AND (OT_User.Code LIKE '98%' or OT_User.Code LIKE '80000%')";
                else if (userType == "1")
                    Conditions += "AND OT_User.Code NOT IN(SELECT Code FROM OT_User WHERE Code LIKE '98%' or Code LIKE '80000%')\r\n";
            }
            if (!string.IsNullOrEmpty(ItemPriority))//任务优先级
            {
                Conditions += string.Format("AND OT_WorkItem.Priority='{0}'\r\n", ItemPriority);
            }
            if (!string.IsNullOrEmpty(keyWord))
            {
                Conditions += string.Format("AND ( OT_InstanceContext.InstanceName LIKE '%{0}%' OR OT_WorkItem.DisplayName LIKE '%{0}%')\r\n", keyWord);
            }
            #endregion

            #region 获取待办任务列表
            string sql = @"
SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( {1} ) AS RowNumber_ ,
                    OT_WorkItem.* ,
                    OT_InstanceContext.InstanceName AS InstanceName ,
                    OT_InstanceContext.OriginatorName AS OriginatorName ,
                    OT_InstanceContext.LastActiveTime AS LastActiveTime ,
                    OT_InstanceContext.WorkflowSummary AS WorkflowSummary ,
                    OT_InstanceContext.OrgUnit AS OriginateUnit ,
                    OT_InstanceContext.BizObjectId AS BizObjectId
          FROM      OT_WorkItem
                    LEFT JOIN OT_InstanceContext ON OT_WorkItem.InstanceId = OT_InstanceContext.ObjectID
                    {5}
          WHERE     ( OT_WorkItem.Participant = '{0}'
                      OR OT_WorkItem.Delegant = '{0}'
                    )
                    AND OT_WorkItem.State in (0,1)
                    {4}
        ) T
WHERE   RowNumber_ >= {2}
        AND RowNumber_ <= {3}
";
            #endregion

            #region 获取待办任务总数
            string sqlGetNumber = @"
		SELECT    COUNT(1)
          FROM      OT_WorkItem
                    LEFT JOIN OT_InstanceContext ON OT_WorkItem.InstanceId = OT_InstanceContext.ObjectID
					{2}
          WHERE     ( OT_WorkItem.Participant = '{0}'
                      OR OT_WorkItem.Delegant = '{0}'
                    )
                    AND OT_WorkItem.State != 2
                    AND OT_WorkItem.State != 3 and OT_WorkItem.WORKFLOWCODE not in ('Bond','Repayment','DealerLoan') -- 不查询批售的流程待办
					{1}
";
            #endregion

            sql = string.Format(sql, this.UserValidator.UserID, OrderBy, pagerInfo.StartIndex, pagerInfo.EndIndex, Conditions, LeftUserTable);
            sqlGetNumber = string.Format(sqlGetNumber, this.UserValidator.UserID, Conditions, LeftUserTable);
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            var num = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetNumber);
            totalNumber = Convert.ToInt32(num);
            return dt;
        }

        private string getOverTimeInfo(HttpContextBase context)
        {
            string usercode = context.Request["code"];
            var user = AppUtility.Engine.Organization.GetUserByCode(usercode);
            if (user == null)
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "Code错误-->" + usercode + "在H3系统中不存在" });
            string sql = "select * from c_emp_holidays where usercode='{0}' and Holidaytype='加班' and state=1";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format(sql, usercode));
            string msg = "";
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                double total = Convert.ToDouble(row["totalhours"] + string.Empty);
                double used = Convert.ToDouble(row["usedhours"] + string.Empty);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = true, Data = "", Msg = "加班总工时：" + total + ",已用工时：" + used });
            }
            else
            {
                msg = "无加班工时";
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = true, Data = "", Msg = msg });
            }
        }

        public string getOverTimeInfo()
        {
            return getOverTimeInfo(HttpContext);
        }

        private string getLeaveOverTimeInfo(HttpContextBase context)
        {
            string usercode = context.Request["code"];
            string hours = context.Request["hours"];
            double applyHours = 0;
            if (!string.IsNullOrEmpty(hours))
            {
                if (!double.TryParse(hours, out applyHours))
                {
                    return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "hours错误，必须为数值" });
                }
            }
            var user = AppUtility.Engine.Organization.GetUserByCode(usercode);
            if (user == null)
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "Code错误-->" + usercode + "在H3系统中不存在" });
            string sql = "select * from c_emp_holidays where usercode='{0}' and Holidaytype='加班' and state=1";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(string.Format(sql, usercode));
            string msg = "";
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                double total = Convert.ToDouble(row["totalhours"] + string.Empty);
                double used = Convert.ToDouble(row["usedhours"] + string.Empty);
                if (!string.IsNullOrEmpty(hours))
                {
                    double remain = total - used;
                    if (remain < applyHours)
                    {
                        return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "申请工时（" + applyHours + "）大于剩余工时（" + remain + "）" });
                    }
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = true, Data = "", Msg = "加班总工时：" + total + ",已用工时：" + used });
            }
            else
            {
                msg = "无加班工时，不能进行调休";
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = msg });
            }
        }

        public string getLeaveOverTimeInfo()
        {
            return getLeaveOverTimeInfo(HttpContext);
        }

        private string getLastWorkItem(HttpContextBase context)
        {
            string id = context.Request["id"];
            var workitem = AppUtility.Engine.WorkItemManager.GetWorkItem(id);
            if (workitem == null)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "id错误或不存在" });
            }
            else if (workitem.TokenId == 1)
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "此任务为第一条任务，无上一条任务" });
            }
            string sql = "select objectid from Ot_Workitemfinished where instanceid='{0}' and tokenid={1}";
            sql = string.Format(sql, workitem.InstanceId, workitem.TokenId - 1);
            string obj = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            if (string.IsNullOrEmpty(obj))
            {
                return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = false, Data = "", Msg = "获取上一条任务的ID为空" });
            }
            var lastWorkitem = AppUtility.Engine.WorkItemManager.GetWorkItem(obj);
            return Newtonsoft.Json.JsonConvert.SerializeObject(new { Success = true, Data = lastWorkitem, Msg = "" });

        }

        public string getLastWorkItem()
        {
            return getLastWorkItem(HttpContext);
        }

        /// <summary>
        /// 查询用户的已办
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="workflowCode"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public JsonResult GetFinishWorkItems(PagerInfo pagerInfo, DateTime? startTime, DateTime? endTime, string workflowCode, string instanceName)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    OThinker.H3.WorkItem.WorkItemState.Finished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    true,
                    OThinker.H3.WorkItem.WorkItemFinished.TableName);
                string OrderBy = "ORDER BY " +
                     OThinker.H3.WorkItem.WorkItemFinished.TableName + "." + OThinker.H3.WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC";
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, OrderBy, OThinker.H3.WorkItem.WorkItemFinished.TableName);
                foreach (DataRow item in dtWorkitem.Rows)
                {
                    if (int.Equals(item["ItemType"], (int)(OThinker.H3.WorkItem.WorkItemType.ActivityForward)))
                    {
                        item["DisplayName"] = "转办";
                    }
                }
                int total = Engine.PortalQuery.CountWorkItem(conditions, OThinker.H3.WorkItem.WorkItemFinished.TableName); // 记录总数
                string[] columns = new string[] { OThinker.H3.WorkItem.WorkItemFinished.PropertyName_OrgUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyFinishedWorkItem_Code);
        }

        /// <summary>
        /// 已办任务去重
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="workflowCode"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public JsonResult GetFinishWorkItemsDistinct(PagerInfo pagerInfo, DateTime? startTime, DateTime? endTime, string workflowCode, string instanceName, bool distinct)
        {
            if (!distinct)
                return GetFinishWorkItems(pagerInfo, startTime, endTime, workflowCode, instanceName);
            return this.ExecuteFunctionRun(() =>
            {
                #region Conditions
                string Conditions = "";
                DateTime sTime = startTime == null ? DateTime.MinValue : startTime.Value;
                DateTime eTime = endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1);
                string OrderBy = "ORDER BY " +
                     OThinker.H3.WorkItem.WorkItemFinished.TableName + "Distinct." + OThinker.H3.WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC";
                if (!string.IsNullOrEmpty(workflowCode))
                {
                    Conditions += "\r\n AND OT_InstanceContext.WorkflowCode = '" + workflowCode + "' ";
                }
                if (!string.IsNullOrEmpty(instanceName))
                {
                    Conditions += "\r\n AND OT_InstanceContext.InstanceName LIKE '%" + instanceName + "%'";
                }
                #endregion

                #region SQL
                string sqlGetData = @"
SELECT  *
FROM    ( SELECT    ROW_NUMBER() OVER ( {0} ) AS RowNumber_ ,
                    OT_WorkItemFinishedDistinct.* ,
                    OT_InstanceContext.InstanceName AS InstanceName ,
                    OT_InstanceContext.OriginatorName AS OriginatorName ,
                    OT_InstanceContext.LastActiveTime AS LastActiveTime ,
                    OT_InstanceContext.WorkflowSummary AS WorkflowSummary ,
                    OT_InstanceContext.OrgUnit AS OriginateUnit ,
                    OT_InstanceContext.BizObjectId AS BizObjectId
          FROM      OT_WorkItemFinishedDistinct
                    LEFT JOIN OT_InstanceContext ON OT_WorkItemFinishedDistinct.InstanceId = OT_InstanceContext.ObjectID
          WHERE     OT_WorkItemFinishedDistinct.ObjectID IN (
                    SELECT  OT_WorkItemFinishedDistinct.ObjectID
                    FROM    OT_WorkItemFinishedDistinct
                            LEFT JOIN OT_InstanceContext ON OT_WorkItemFinishedDistinct.InstanceId = OT_InstanceContext.ObjectID
                    WHERE   ( OT_WorkItemFinishedDistinct.Participant = '{1}'
                              OR OT_WorkItemFinishedDistinct.Finisher = '{1}'
                            )
                            AND OT_WorkItemFinishedDistinct.FinishTime > to_date('{2}','yyyy-mm-dd')
                            AND OT_WorkItemFinishedDistinct.FinishTime <= to_date('{3}','yyyy-mm-dd')
                            AND ( OT_WorkItemFinishedDistinct.State = 2 ) {4})
                    AND OT_InstanceContext.Objectid is not null
        ) T
WHERE   RowNumber_ >= {5}
        AND RowNumber_ <= {6}
";
                string sqlGetNum = @"
SELECT  COUNT(1)
FROM    OT_WorkItemFinishedDistinct
        LEFT JOIN OT_InstanceContext ON OT_WorkItemFinishedDistinct.InstanceId = OT_InstanceContext.ObjectID
WHERE   OT_WorkItemFinishedDistinct.ObjectID IN (
        SELECT  OT_WorkItemFinishedDistinct.ObjectID
        FROM    OT_WorkItemFinishedDistinct
                LEFT JOIN OT_InstanceContext ON OT_WorkItemFinishedDistinct.InstanceId = OT_InstanceContext.ObjectID
        WHERE   ( OT_WorkItemFinishedDistinct.Participant = '{0}'
                  OR OT_WorkItemFinishedDistinct.Finisher = '{0}'
                )
                AND OT_WorkItemFinishedDistinct.FinishTime > to_date('{1}','yyyy-mm-dd')
                AND OT_WorkItemFinishedDistinct.FinishTime <= to_date('{2}','yyyy-mm-dd')
                AND ( OT_WorkItemFinishedDistinct.State = 2 ) {3})
        AND OT_InstanceContext.Objectid is not null
";
                #endregion
                sqlGetData = string.Format(sqlGetData, OrderBy, this.UserValidator.UserID, sTime.ToString("yyyy-MM-dd"), eTime.ToString("yyyy-MM-dd"), Conditions, pagerInfo.StartIndex, pagerInfo.EndIndex);
                sqlGetNum = string.Format(sqlGetNum, this.UserValidator.UserID, sTime.ToString("yyyy-MM-dd"), eTime.ToString("yyyy-MM-dd"), Conditions);

                DataTable dtWorkitem = Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlGetData);
                foreach (DataRow item in dtWorkitem.Rows)
                {
                    if (int.Equals(item["ItemType"], (int)(OThinker.H3.WorkItem.WorkItemType.ActivityForward)))
                    {
                        item["DisplayName"] = "转办";
                    }
                }
                //int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.WorkItemFinished.TableName); // 记录总数
                int total = Convert.ToInt32(this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sqlGetNum) + string.Empty);

                string[] columns = new string[] { OThinker.H3.WorkItem.WorkItemFinished.PropertyName_OrgUnit };
                List<WorkItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns);
                GridViewModel<WorkItemViewModel> result = new GridViewModel<WorkItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            }, FunctionNode.Workspace_MyFinishedWorkItem_Code);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}