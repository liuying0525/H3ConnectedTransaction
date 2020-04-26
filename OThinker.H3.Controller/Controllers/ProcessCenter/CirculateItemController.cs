using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Collections.Generic;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;

using System.Data;

namespace OThinker.H3.Controllers
{
    [Authorize]
    //[ActionXSSFillters]
    public class CirculateItemController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public CirculateItemController()
        {
        }

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

        /// <summary>
        /// 查询用户的待阅任务
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="keyWord"></param>
        /// <returns></returns>
        public JsonResult GetUnReadWorkItems(PagerInfo pagerInfo, string keyWord)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID, DateTime.MinValue, DateTime.MaxValue, WorkItem.WorkItemState.Unfinished, keyWord, OThinker.Data.BoolMatchValue.Unspecified, string.Empty, false, WorkItem.CirculateItem.TableName);
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, string.Empty, WorkItem.CirculateItem.TableName);
                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.CirculateItem.TableName); // 记录总数
                string[] columns = new string[] { WorkItem.CirculateItem.PropertyName_OrgUnit };
                List<CirculateItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns, false);
                GridViewModel<CirculateItemViewModel> result = new GridViewModel<CirculateItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 查询用户的已阅任务
        /// </summary>
        /// <param name="pagerInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="workflowCode"></param>
        /// <param name="instanceName"></param>
        /// <returns></returns>
        public JsonResult GetReadWorkItems(PagerInfo pagerInfo, DateTime? startTime, DateTime? endTime, string workflowCode, string instanceName)
        {
            // TODO:传阅类任务，接口名称统一改为 CirculateItem 相关的，例如： GetCirculateItems 
            // TODO:查询待阅、已阅，是从待阅和已阅表中查询，所以应该不是 QueryWorkItem 这个接口，应该有一个 QueryCirculateItem 接口
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = Engine.PortalQuery.GetWorkItemConditions(this.UserValidator.UserID,
                    startTime == null ? DateTime.MinValue : startTime.Value,
                    endTime == null ? DateTime.MaxValue : endTime.Value.AddDays(1),
                    WorkItem.WorkItemState.Finished,
                    instanceName,
                    OThinker.Data.BoolMatchValue.Unspecified,
                    workflowCode,
                    false,
                    WorkItem.CirculateItemFinished.TableName);
                DataTable dtWorkitem = Engine.PortalQuery.QueryWorkItem(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex, string.Empty, WorkItem.CirculateItemFinished.TableName);

                int total = Engine.PortalQuery.CountWorkItem(conditions, WorkItem.CirculateItemFinished.TableName); // 记录总数
                string[] columns = new string[] { WorkItem.CirculateItemFinished.PropertyName_OrgUnit, Query.ColumnName_OriginateUnit };
                List<CirculateItemViewModel> griddata = this.Getgriddata(dtWorkitem, columns, false);
                GridViewModel<CirculateItemViewModel> result = new GridViewModel<CirculateItemViewModel>(total, griddata, pagerInfo.sEcho);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        public JsonResult ReadCirculateItemsByBatch(string[] CirculateItemIDs)
        {
            return this.ExecuteFunctionRun(() =>
            {

                string ids = "";
                foreach (string id in CirculateItemIDs)
                {
                    long result = this.Engine.WorkItemManager.FinishCirculateItem(id, this.UserValidator.UserID, WorkItem.AccessPoint.Batch);
                    ids = result + "||" + ids;
                }
                return Json(ids, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取返回到前端的CirculateItemViewModel
        /// </summary>
        /// <param name="dtWorkItem"></param>
        /// <param name="columns"></param>
        /// <returns></returns>
        public List<CirculateItemViewModel> Getgriddata(DataTable dtWorkItem, string[] columns, bool unfinishedWorkitem = true)
        {
            string InstanceName = string.Empty;
            //批量获取组织架构显示名称
            Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtWorkItem, columns);
            //批量获取流程发起人的OU显示名称
            Dictionary<string, string> orgOUNames = this.GetParentUnitNamesFromTable(dtWorkItem,
                new string[] { WorkItem.WorkItem.PropertyName_Originator });
            List<CirculateItemViewModel> griddata = new List<CirculateItemViewModel>();
            foreach (DataRow row in dtWorkItem.Rows)
            {
                if (string.IsNullOrEmpty(row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty))
                {
                    var workItem = this.Engine.WorkItemManager.GetCirculateItem(this.GetColumnsValue(row, WorkItem.WorkItem.PropertyName_ObjectID));
                    InstanceName = this.Engine.WorkflowManager.GetClauseDisplayName(workItem.WorkflowCode);
                }
                else
                {
                    InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty;
                }
                griddata.Add(new CirculateItemViewModel()
                {
                    CirculateCreator = this.GetColumnsValue(row, WorkItem.CirculateItem.PropertyName_Creator),
                    CirculateCreatorName = row.Table.Columns.Contains(WorkItem.CirculateItem.PropertyName_CreatorName) ? row[WorkItem.CirculateItem.PropertyName_CreatorName] + string.Empty : null,
                    ObjectID = this.GetColumnsValue(row, WorkItem.WorkItem.PropertyName_ObjectID),
                    InstanceId = row[WorkItem.WorkItem.PropertyName_InstanceId] + string.Empty,
                    InstanceName = InstanceName,
                    WorkflowCode = row[WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty,
                    WorkflowName = row.Table.Columns.Contains(WorkflowClause.PropertyName_WorkflowName) ? row[WorkflowClause.PropertyName_WorkflowName] + string.Empty : null,
                    DisplayName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                    ReceiveTime = this.GetValueFromDate(row[WorkItem.WorkItem.PropertyName_ReceiveTime], WorkItemTimeFormat),
                    FinishTime = this.GetValueFromDate(row[WorkItem.WorkItem.PropertyName_FinishTime], WorkItemTimeFormat),
                    Originator = row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty,
                    OriginatorName = row[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty,
                    OriginatorOUName = this.GetValueFromDictionary(orgOUNames,
                        row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty)
                });
            }
            return griddata;
        }

        public string GetColumnsValue(DataRow row, string columns)
        {
            return row.Table.Columns.Contains(columns) ? row[columns] + string.Empty : "";
        }
    }
}
