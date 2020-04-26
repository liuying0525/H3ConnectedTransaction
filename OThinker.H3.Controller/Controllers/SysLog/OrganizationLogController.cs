using OThinker.Data;
using OThinker.Data.Database;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysLog
{
    /// <summary>
    /// 组织日志控制器
    /// </summary>
    [Authorize]
    public class OrganizationLogController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.SysLog_OrganizationLog_Code;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public OrganizationLogController() { }

        /// <summary>
        /// 获取组织日志分页数据集
        /// </summary>
        /// <param name="pagerInfo">页码信息</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="unitID">组织ID</param>
        /// <returns>组织日志Json对象</returns>
        [HttpPost]
        public JsonResult GetOrganizationLogList(PagerInfo pagerInfo, string startTime, string endTime, string unitID)
        {
            return ExecuteFunctionRun(() =>
              {
                  int count = 0;
                  //查询结果集(数据库返回对象)
                  System.Data.DataTable dt = QueryOrganizationLog(startTime, endTime, unitID, pagerInfo.PageIndex, pagerInfo.PageSize, out count);
                  List<OrganizationLogViewModel> list = new List<OrganizationLogViewModel>();

                  //Dictionary<string, string> fullNames = this.GetUnitNamesFromTable(dt, new string[] { OrganizationLog.PropertyName_UnitID });
                  Dictionary<string, string> modifierNames = this.GetUnitNamesFromTable(dt, new string[] { OrganizationLog.PropertyName_Modifier });
                  //构造前端返回对象
                  //TODO 显示组织机构变更日志
                  foreach (DataRow r in dt.Rows)
                  {
                      string modifier = r[OrganizationLog.PropertyName_Modifier].ToString();
                      string unitId = r[OrganizationLog.PropertyName_UnitID].ToString();
                      list.Add(new OrganizationLogViewModel()
                      {
                          Time = r[OrganizationLog.PropertyName_Time] + string.Empty,
                          Operation = ((OThinker.Organization.OrganizationLog.OperationType)(int.Parse(r[OrganizationLog.PropertyName_Operation] + string.Empty))).ToString(),
                          Content = r[OrganizationLog.PropertyName_Content].ToString().Replace(OrganizationLog.PostPropertyValue.ToString(), " ").Replace("<", "(").Replace(">", ")"),
                          Modifier = modifierNames.ContainsKey(modifier) ? modifierNames[modifier] : "QueryOrgLog.System",
                          UnitID = r[OrganizationLog.PropertyName_Name]+string.Empty
                      });

                  }
                  var griddata = new { Rows = list, Total = count };
                  return Json(griddata, JsonRequestBehavior.AllowGet);
              });
        }

        /// <summary>
        /// 查询组织架构日志
        /// </summary>
        /// <param name="StartTime">开始时间</param>
        /// <param name="EndTime">结束时间</param>
        /// <param name="UnitID">组织ID</param>
        /// <param name="PageIndex">页号</param>
        /// <param name="PageSize">页大小</param>
        /// <param name="RecordCount">总记录数</param>
        /// <returns>组织架构数据</returns>
        private DataTable QueryOrganizationLog(string startTime, string endTime, string unitID, int pageIndex, int pageSize, out int total)
        {
            #region 构造查询参数
            QueryConditonCollection conditions = new QueryConditonCollection();

            DateTime startTimeValue = DateTime.MinValue;
            DateTime endTimeValue = DateTime.MinValue;
            if (DateTime.TryParse(startTime, out startTimeValue))
            {
                conditions.Add(ComparisonOperatorType.NotBelow,
                    OrganizationLog.PropertyName_Time,
                    startTimeValue,
                   this.Engine.EngineConfig.LogDBType);
            }

            if (DateTime.TryParse(endTime, out endTimeValue))
            {
                endTimeValue = endTimeValue.AddDays(1);
                conditions.Add(ComparisonOperatorType.NotAbove,
                  OrganizationLog.PropertyName_Time,
                  endTimeValue,
                  this.Engine.EngineConfig.LogDBType);
            }
            if (!string.IsNullOrEmpty(unitID))
            {
                conditions.Add(ComparisonOperatorType.Equal, OrganizationLog.PropertyName_UnitID, unitID);
            }
            #endregion
            return this.Engine.Query.QueryOrganizationLog(conditions.ToArray(), pageIndex, pageSize, out total);
        }

        // End Class
    }
}
