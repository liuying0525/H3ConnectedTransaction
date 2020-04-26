using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Data;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 委托服务类  TODO:这个类去除，和后台的服务类合并
    /// </summary>
    [Authorize]
    public class AgentsController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public AgentsController()
        {
        }

        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                // TODO:需要使用OThinker.H3.Acl.FunctionNode中定义的常量编码
                return string.Empty;
            }
        }

        /// <summary>
        /// 获取用户委托详情
        /// </summary>
        /// <returns></returns>
        public JsonResult GeyAgencyTable()
        {
            return this.ExecuteFunctionRun(() =>
            {
                DataTable dtAgencyTable = Engine.Query.QueryAgent(this.UserValidator.UserID);
                int total = dtAgencyTable.Rows.Count;
                Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(dtAgencyTable, new string[] { WorkItem.Agency.PropertyName_UserID, WorkItem.Agency.PropertyName_AgentID });
                List<AgencyViewModel> griddata = new List<AgencyViewModel>();
                foreach (DataRow row in dtAgencyTable.Rows)
                {
                    griddata.Add(new AgencyViewModel()
                    {
                        AgencyID = row[WorkItem.Agency.PropertyName_ObjectID] + string.Empty,
                        UserID = this.GetValueFromDictionary(unitNames,
                            row[WorkItem.Agency.PropertyName_UserID] + string.Empty),
                        WasAgentName = this.GetValueFromDictionary(unitNames,//被委托人
                            row[WorkItem.Agency.PropertyName_AgentID] + string.Empty),
                        WorkflowCode = row[WorkItem.Agency.PropertyName_WorkflowCode] + string.Empty,
                        WorkflowName = row[WorkflowClause.PropertyName_WorkflowName] + string.Empty,
                        AgencyType = row[WorkItem.Agency.PropertyName_AgencyType] + string.Empty,
                        StartTime = Convert.ToDateTime(row[WorkItem.Agency.PropertyName_StartTime]).ToString("yyyy-MM-dd"),
                        EndTime = Convert.ToDateTime(row[WorkItem.Agency.PropertyName_EndTime]).ToString("yyyy-MM-dd")
                    });
                }
                GridViewModel<AgencyViewModel> result = new GridViewModel<AgencyViewModel>(total, griddata);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取一个委托详情
        /// </summary>
        /// <param name="AgencyID">委托ID</param>
        /// <returns></returns>
        public JsonResult GetAgency(string AgencyID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                WorkItem.Agency agency = Engine.AgencyManager.GetAgency(AgencyID);
                List<AgencyViewModel> griddata = new List<AgencyViewModel>();
                if (agency != null)
                {
                    griddata.Add(new AgencyViewModel()
                    {
                        AgencyID = agency.AgencyID + string.Empty,//委托id
                        UserID = agency.UserID + string.Empty,
                        WasAgentID = agency.AgentID + string.Empty,//被委托人id
                        WorkflowCode = agency.WorkflowCode + string.Empty,
                        OriginatorRange = agency.Originator + string.Empty,//发起人范围
                        StartTime = Convert.ToDateTime(agency.StartTime + string.Empty).ToString("yyyy-MM-dd"),
                        EndTime = Convert.ToDateTime(agency.EndTime + string.Empty).ToString("yyyy-MM-dd")
                    });
                }
                GridViewModel<AgencyViewModel> result = new GridViewModel<AgencyViewModel>(1, griddata);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
       

       
    }
}
