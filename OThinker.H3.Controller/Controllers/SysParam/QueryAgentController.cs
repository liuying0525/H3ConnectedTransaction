using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 委托设置控制器
    /// </summary>
    [Authorize]
    public class QueryAgentController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.SysParam_QueryAgent_Code; }
        }

        #region 后台管理员 ————————————————————
        /// <summary>
        /// 获取代理数据集
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>用于LigerUi的代理数据json对象</returns>
        [HttpPost]
        public JsonResult GetAgentList(PagerInfo pagerInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                int total = 0;
                //查询代理数据
                DataTable dt = QueryAgencyList(pagerInfo, out total);
                List<AgentsViewModel> list = new List<AgentsViewModel>();
                //查询代理人名称,用户名称
                Dictionary<string, string> names = this.GetUnitNamesFromTable(dt, new string[] { Agency.PropertyName_UserID, Agency.PropertyName_AgentID });
                foreach (DataRow r in dt.Rows)
                {
                    DateTime startTime, endTime;
                    string start = "", end = "";
                    if (DateTime.TryParse(r[Agency.PropertyName_StartTime].ToString(), out startTime))
                    {
                        start = startTime.ToShortDateString();
                    };
                    if (DateTime.TryParse(r[Agency.PropertyName_EndTime].ToString(), out endTime))
                    {
                        end = endTime.ToShortDateString();
                    }
                    list.Add(new AgentsViewModel
                    {

                        ObjectID = r[Agency.PropertyName_ObjectID] + string.Empty,
                        UserName = names[r[Agency.PropertyName_UserID] + string.Empty],
                        WorkflowCode = r[Agency.PropertyName_WorkflowCode] + string.Empty,
                        WorkflowName = r[WorkflowClause.PropertyName_WorkflowName] + string.Empty,
                        AgentName = names[r[Agency.PropertyName_AgentID] + string.Empty],
                        StartTime = start,
                        EndTime = end
                    });
                }
                switch (pagerInfo.sortname)
                {
                    case "UserName":
                        if (pagerInfo.sortorder == "asc")
                            list = list.OrderBy(a => a.UserName).ToList();
                        else
                        {
                            list = list.OrderByDescending(a => a.UserName).ToList();
                        }
                        break;
                    case "WorkflowCode":
                        if (pagerInfo.sortorder == "asc")
                            list = list.OrderBy(a => a.WorkflowCode).ToList();
                        else
                        {
                            list = list.OrderByDescending(a => a.WorkflowCode).ToList();
                        }
                        break;
                    case "WorkflowName":
                        if (pagerInfo.sortorder == "asc")
                            list = list.OrderBy(a => a.WorkflowName).ToList();
                        else
                        {
                            list = list.OrderByDescending(a => a.WorkflowName).ToList();
                        }
                        break;
                    case "AgentName":
                        if (pagerInfo.sortorder == "asc")
                            list = list.OrderBy(a => a.AgentName).ToList();
                        else
                        {
                            list = list.OrderByDescending(a => a.AgentName).ToList();
                        }
                        break;
                    case "StartTime":
                        if (pagerInfo.sortorder == "asc")
                            list = list.OrderBy(a => a.StartTime).ToList();
                        else
                        {
                            list = list.OrderByDescending(a => a.StartTime).ToList();
                        }
                        break;
                    case "EndTime":
                        if (pagerInfo.sortorder == "asc")
                            list = list.OrderBy(a => a.EndTime).ToList();
                        else
                        {
                            list = list.OrderByDescending(a => a.EndTime).ToList();
                        }
                        break;
                }

                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取代理数据集
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <returns>用于LigerUi的代理数据json对象</returns>
        [HttpPost]
        public JsonResult GetAgentListParams(PagerInfo pagerInfo, QueryAgentListParam param)
        {
            return ExecuteFunctionRun(() =>
            {
                int total = 0;
                //查询代理数据
                DataTable dt = QueryAgencyListParams(pagerInfo, param, out total);
                //DataTable dt = dtQuery.Clone();


                List<AgentsViewModel> list = new List<AgentsViewModel>();
                //查询代理人名称,用户名称
                Dictionary<string, string> names = this.GetUnitNamesFromTable(dt, new string[] { Agency.PropertyName_UserID, Agency.PropertyName_AgentID });
                foreach (DataRow r in dt.Rows)
                {
                    DateTime startTime, endTime;
                    string start = "", end = "";
                    if (DateTime.TryParse(r[Agency.PropertyName_StartTime].ToString(), out startTime))
                    {
                        start = startTime.ToShortDateString();
                    };
                    if (DateTime.TryParse(r[Agency.PropertyName_EndTime].ToString(), out endTime))
                    {
                        end = endTime.ToShortDateString();
                    }
                    list.Add(new AgentsViewModel
                    {

                        ObjectID = r[Agency.PropertyName_ObjectID] + string.Empty,
                        UserName = names[r[Agency.PropertyName_UserID] + string.Empty],
                        WorkflowCode = r[Agency.PropertyName_WorkflowCode] + string.Empty,
                        WorkflowName = r[WorkflowClause.PropertyName_WorkflowName] + string.Empty,
                        AgentName = names[r[Agency.PropertyName_AgentID] + string.Empty],
                        StartTime = start,
                        EndTime = end
                    });
                }

                var griddata = new { Rows = list, Total = total };
                return Json(griddata, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取委托信息
        /// </summary>
        /// <param name="agentID"委托ID></param>
        /// <returns>委托信息</returns>
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public JsonResult GetAgent(string agentID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                AgentsViewModel model = new AgentsViewModel();
                if (string.IsNullOrEmpty(agentID))
                {
                    model.StartTime = DateTime.Now.ToString("yyy-MM-dd");
                    model.EndTime = DateTime.Now.AddDays(1).ToString("yyy-MM-dd");
                    model.Default = false;
                    result.Success = true;
                    result.Extend = model;
                }
                else
                {
                    var item = this.Engine.AgencyManager.GetAgency(agentID);
                    if (item != null)
                    {
                        model = new AgentsViewModel()
                        {
                            ObjectID = item.ObjectID,
                            UserID = item.UserID,
                            Default = item.Default,
                            WorkflowCode = item.WorkflowCode,
                            StartTime = item.StartTime.ToString("yyy-MM-dd"),
                            EndTime = item.EndTime.ToString("yyy-MM-dd"),
                            Originator = item.Originator,
                            Agent = item.AgentID
                        };
                        result.Success = true;
                        result.Extend = model;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = "msgGlobalString.NullObjectException";
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存委托设置
        /// </summary>
        /// <param name="model">委托设置信息</param>
        /// <returns>ActionResult,是否成功</returns>
        [HttpPost]
        public JsonResult SaveAgent(AgentsViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                List<string> workflowCodes = new List<string>();
                if (!string.IsNullOrEmpty(model.WorkflowCode))
                {
                    model.WorkflowCode = model.WorkflowCode.TrimEnd(',');
                    workflowCodes = model.WorkflowCode.Split(',').ToList();
                }
                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    return SaveAgent(model, workflowCodes);
                }
                else
                {
                    return UpdateAgent(model);
                }
            });
        }

        /// <summary>
        /// 删除委托设置
        /// </summary>
        /// <param name="ids">委托ID</param>
        /// <returns>删除结果</returns>
        [HttpPost]
        public JsonResult DelAgent(string ids)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ids = ids.TrimEnd(',');
                string[] idList = ids.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                try
                {
                    foreach (string list in idList)
                    {
                        this.Engine.AgencyManager.Remove(list);
                    }
                    result.Success = true;
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
        /// 查询代理数据
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <param name="total">总行数</param>
        /// <returns>带分页数据的代理数据</returns>
        private DataTable QueryAgencyListParams(PagerInfo pagerInfo, QueryAgentListParam param, out int total)
        {
            string Params = string.Empty;
            switch (pagerInfo.sortname)
            {
                case "UserName":
                    Params = "UserID";

                    break;
                case "WorkflowCode":
                    Params = "WorkflowCode";

                    break;
                case "WorkflowName":
                    Params = "WorkflowName";
                    break;
                case "AgentName":
                    Params = "AgentID";
                    break;
                case "StartTime":
                    Params = "StartTime";

                    break;
                case "EndTime":
                    Params = "EndTime";

                    break;
            }
            int startIndex = pagerInfo.StartIndex;
            int endIndex = pagerInfo.EndIndex;
            DataTable dt = this.Engine.Query.QueryAgent(pagerInfo.PageIndex, pagerInfo.PageSize, Params, pagerInfo.sortorder); // TODO:修改这个方法,带分页查询

            string UserID = string.Empty;
            string WorkflowCode = string.Empty;
            string Mandatary = string.Empty;

            if (param.Agent != null)
            {
                UserID = param.Agent[0] + string.Empty;
            }
            if (param.WorkFlowCode != null)
            {
                WorkflowCode = param.WorkFlowCode[0] + string.Empty;
            }
            if (param.Mandatary != null)
            {
                Mandatary = param.Mandatary[0] + string.Empty;
            }
            if (!(string.IsNullOrEmpty(UserID) && string.IsNullOrEmpty(WorkflowCode) && string.IsNullOrEmpty(Mandatary)))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string User = dt.Rows[i]["UserID"] + string.Empty;
                    string workflow = dt.Rows[i]["WorkflowCode"] + string.Empty;
                    string Agent = dt.Rows[i]["AgentID"] + string.Empty;
                    if (!(
                        (string.IsNullOrEmpty(UserID) || UserID.IndexOf(User) != -1) &&
                        (string.IsNullOrEmpty(WorkflowCode) || (WorkflowCode.IndexOf(workflow) != -1 && workflow != "")) &&
                        (string.IsNullOrEmpty(Mandatary) || Mandatary.IndexOf(Agent) != -1)))
                    {
                        dt.Rows[i].Delete();
                    }
                }
                dt.AcceptChanges();
            }
            DataTable dtt = this.Engine.Query.QueryAgent();
            total = dtt.Rows.Count;
            return dt;
        }
        /// <summary>
        /// 查询代理数据
        /// </summary>
        /// <param name="pagerInfo">分页信息</param>
        /// <param name="total">总行数</param>
        /// <returns>带分页数据的代理数据</returns>
        private DataTable QueryAgencyList(PagerInfo pagerInfo, out int total)
        {
            string Params = string.Empty;
            switch (pagerInfo.sortname)
            {
                case "UserName":
                    Params = "UserID";

                    break;
                case "WorkflowCode":
                    Params = "B.WorkflowCode";

                    break;
                case "WorkflowName":
                    Params = "WorkflowName";
                    break;
                case "AgentName":
                    Params = "AgentID";
                    break;
                case "StartTime":
                    Params = "StartTime";

                    break;
                case "EndTime":
                    Params = "EndTime";

                    break;
            }
            int startIndex = pagerInfo.StartIndex;
            int endIndex = pagerInfo.EndIndex;
            DataTable dt = this.Engine.Query.QueryAgent(pagerInfo.PageIndex, pagerInfo.PageSize, Params, pagerInfo.sortorder); // TODO:修改这个方法,带分页查询
            DataTable dtt = this.Engine.Query.QueryAgent();
            total = dtt.Rows.Count;
            return dt;
        }

        /// <summary>
        /// 更新委托设置
        /// </summary>
        /// <param name="model">委托模型</param>
        /// <returns>ActionResult</returns>
        private JsonResult UpdateAgent(AgentsViewModel model)
        {
            ActionResult result = new ActionResult();
            // 编辑模式
            Agency agency = this.Engine.AgencyManager.GetAgency(model.ObjectID);
            agency.AgentID = model.Agent;
            // 时间设置
            agency.StartTime = DateTime.Parse(model.StartTime);
            agency.EndTime = DateTime.Parse(model.EndTime);
            agency.AgencyType = WorkItem.AgencyType.WorkItem;
            agency.Originator = model.Originator;
            agency.UserID = model.UserID;
            agency.WorkflowCode = model.WorkflowCode;

            if (!agency.Validate())
            {
                result.Success = false;
                result.Message = "Agent.InvalidAgency";
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            if (this.Engine.AgencyManager.Update(agency) == false)
            {
                result.Success = false;
                result.Message = "Agent.EditError";
                result.Extend = new
                {
                    WorkflowErrorflowCodes = new string[] { agency.WorkflowCode }
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 保存委托
        /// </summary>
        /// <param name="model">委托模型</param>
        /// <param name="workflowCodes">流程编码集</param>
        /// <returns>ActionResult</returns>
        private JsonResult SaveAgent(AgentsViewModel model, List<string> workflowCodes)
        {
            ActionResult result = new ActionResult();
            // 添加模式
            // 检查流程模板是否设置
            if (!model.Default && workflowCodes.Count == 0)
            {
                result.Success = false;
                result.Message = "Agent.WorkflowCodeNull";
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            List<OThinker.H3.WorkItem.Agency> agencys = new List<WorkItem.Agency>();
            WorkItem.AgencyType agencyType = WorkItem.AgencyType.WorkItem;
            if (model.Default)
            {
                //勾选所有其他模板
                agencys.Add(new WorkItem.Agency(model.UserID,
                        agencyType,
                        model.Agent,
                        "",
                        model.Originator,
                        DateTime.Parse(model.StartTime),
                        DateTime.Parse(model.EndTime)));
            }
            else
            {
                foreach (string workflowCode in workflowCodes)
                {

                    if (this.Engine.AgencyManager.Exists(model.UserID, workflowCode, model.Originator, DateTime.Parse(model.StartTime), DateTime.Parse(model.EndTime)))
                    {
                        result.Success = false;
                        result.Message = "Agent.WorkflowExists";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    // 创建委托关系
                    agencys.Add(new WorkItem.Agency(model.UserID,
                        agencyType,
                        model.Agent,
                        workflowCode,
                        model.Originator,
                        DateTime.Parse(model.StartTime),
                        DateTime.Parse(model.EndTime)));

                }
            }
            List<string> errorCodes = new List<string>();
            foreach (WorkItem.Agency agency in agencys)
            {
                // 检查合法性
                if (!agency.Validate())
                {
                    result.Success = false;
                    result.Message = "Agent.InvalidAgency";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                // 保存
                if (this.Engine.AgencyManager.Add(agency) == false)
                {
                    errorCodes.Add(agency.WorkflowCode);
                    result.Success = false;
                    result.Message = "Agent.AddError";
                    result.Extend = new
                    {
                        WorkflowErrorCodes = errorCodes
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            result.Success = true;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion


        #region 前台用户  —————————————————————

        /// <summary>
        /// 获取用户委托详情
        /// </summary>
        /// <returns></returns>
        public JsonResult GeyAgencyTable(PagerInfo pagerInfo)
        {
            return this.ExecuteFunctionRun(() =>
            {
                string[] conditions = new string[]{
                (this.UserValidator.UserID == null)?null:(WorkItem.Agency.PropertyName_UserID + "='" + 
                    this.UserValidator.UserID + "'")};
                DataTable dtAgencyTable = Engine.PortalQuery.QueryAgent(conditions, pagerInfo.StartIndex, pagerInfo.EndIndex);
                int total = Engine.PortalQuery.Count(WorkItem.Agency.TableName, conditions);
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
            }, string.Empty);
        }

        /// <summary>
        /// 获取一个委托详情
        /// </summary>
        /// <param name="AgencyID">委托ID</param>
        /// <returns></returns>
        public JsonResult GetAgency(string agentID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                WorkItem.Agency agency = Engine.AgencyManager.GetAgency(agentID);
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
            }, string.Empty);
        }

        /// <summary>
        /// 添加、编辑委托
        /// </summary>
        /// <param name="AgencyID"></param>
        /// <param name="IsAllWorkflow"></param>
        /// <param name="WorkflowCodes"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <param name="OriginatorRange"></param>
        /// <param name="WasAgent"></param>
        /// <returns></returns>
        public JsonResult AddAgency(string AgencyID, bool IsAllWorkflow, string[] WorkflowCodes, DateTime StartTime, DateTime EndTime, string OriginatorRange, string WasAgent)
        {
            return this.ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                if (AgencyID == string.Empty)     //添加模式
                {
                    if (IsAllWorkflow)
                    {
                        WorkflowCodes = new string[] { null };
                    }
                    // 检查是否已经存在
                    for (int count = 0; count < WorkflowCodes.Length; count++)
                    {
                        string code = WorkflowCodes[count];
                        if (this.Engine.AgencyManager.Exists(this.UserValidator.UserID, code, OriginatorRange, StartTime, EndTime))
                        {
                            result.Success = false;
                            result.Message = "Agent.AddError";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                    for (int count = 0; count < WorkflowCodes.Length; count++)
                    {
                        string code = WorkflowCodes[count];
                        // 创建委托关系
                        WorkItem.Agency agency = new WorkItem.Agency(
                            this.UserValidator.UserID,
                            WorkItem.AgencyType.WorkItem,
                            WasAgent,
                            code,
                            OriginatorRange,
                            StartTime,
                            EndTime.AddDays(1).AddSeconds(-1));
                        // 检查合法性
                        if (!agency.Validate())
                        {
                            result.Success = false;
                            result.Message = "Agent.AddError";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            // 保存
                            Engine.AgencyManager.Add(agency);
                        }
                    }
                }
                else    //编辑模式
                {
                    WorkItem.Agency thisAgency = Engine.AgencyManager.GetAgency(AgencyID);
                    thisAgency.StartTime = StartTime;
                    thisAgency.EndTime = EndTime;
                    thisAgency.Originator = OriginatorRange;
                    thisAgency.AgentID = WasAgent;
                    thisAgency.AgencyType = WorkItem.AgencyType.WorkItem;
                    // 检查是否已经存在
                    if (this.Engine.AgencyManager.Update(thisAgency) == false)
                    {
                        result.Success = false;
                        result.Message = "Agent.EditError";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        //删除委托
        public JsonResult RemoveAgency(string[] AgencysID)
        {
            return this.ExecuteFunctionRun(() =>
            {
                var result = true;
                foreach (string agencyID in AgencysID)
                {
                    Engine.AgencyManager.Remove(agencyID);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }
        #endregion
    }
}
