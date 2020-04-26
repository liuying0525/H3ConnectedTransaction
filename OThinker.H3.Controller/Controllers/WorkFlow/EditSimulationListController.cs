using OThinker.H3.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class EditSimulationListController:ControllerBase
    {

        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult Load(string SimulationListID,string WorkflowCode)
        {
            return ExecuteFunctionRun(() => {
                ActionResult result = new ActionResult(false, "");

                InstanceSimulationList SimulationList = null;
                OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;

                Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                SimulationList = this.Engine.SimulationManager.GetSimulationList(SimulationListID);

                var WorkflowName = "";
                var SimulationListCode = "";
                string simulation = string.Empty;//当前测试集的测试用例ID
                var Simulations=new List<object>();//当前流程测试用例集合

                //ID非空为编辑模式
                if (!string.IsNullOrEmpty(SimulationListID))
                {
                    if (SimulationList != null)
                    {
                        if (Clause != null)
                        {
                           WorkflowName= Clause.WorkflowName + "[" + WorkflowCode + "]";
                        }
                       
                       SimulationListCode = SimulationList.SimulationListCode;
                       
                        if (SimulationList.Simulations != null && SimulationList.Simulations.Length > 0)
                        {
                            foreach (string id in SimulationList.Simulations)
                            {
                                simulation += id + ";";
                            }
                            simulation = simulation.TrimEnd(';');
                        }
                       // ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "simulation", "var simulation=\"" + simulation + "\";", true);
                    }
                    else
                    {
                        result.Message = "Simulation.SimulationDetail_Mssg1";
                        //打开失败
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }
                //ID为空,为新增模式
                else
                {
                    WorkflowName = this.Engine.WorkflowManager.GetClauseDisplayName(WorkflowCode);
                }

                InstanceSimulation[] SimulationsObject = this.Engine.SimulationManager.GetSimulationByWorkflow(WorkflowCode);
                if (SimulationsObject != null && SimulationsObject.Length > 0)
                {
                    List<object> lstSimulations = new List<object>();
                    foreach (InstanceSimulation sim in SimulationsObject)
                    {
                        lstSimulations.Add(new
                        {
                            ObjectID = sim.ObjectID,
                            InstanceName = sim.InstanceName
                        });
                    }
                    Simulations = lstSimulations;
                 }

                var returnObject = new
                {
                    WorkflowName = WorkflowName,
                    SimulationListCode = SimulationListCode,
                    SimulationIDS = simulation,
                    Simulations = CreateLigerUIGridData(Simulations.ToArray())
                };

                result.Success = true;
                result.Message = "";
                result.Extend = returnObject;

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult Save(string SimulationListID,string SimulationListCode,string WorkflowCode,string SimulationIDS)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");
                InstanceSimulationList SimulationList = null;
                OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;

                Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                
                if (string.IsNullOrEmpty(SimulationListID))
                {
                    SimulationList = new InstanceSimulationList()
                    {
                        WorkflowCode = WorkflowCode
                    };
                }else
                {
                    SimulationList = this.Engine.SimulationManager.GetSimulationList(SimulationListID);
                }
                
                if (SimulationList == null)
                {
                    //编辑的模拟不存在
                    result.Message = "Simulation.SimulationDetail_Mssg4";

                    return Json(result);
                }
                //编码必须以字母开始
                //System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[A-Za-z][A-Za-z0-9_]*$");
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                if (!regex.Match(SimulationListCode).Success)
                {
                    result.Message = "Simulation.ObjectSchemaProperty_Mssg4";
                    return Json(result);
                }


                SimulationList.Simulations = (SimulationIDS + string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                SimulationList.SimulationListCode = SimulationListCode;
                SimulationList.WorkflowCode = WorkflowCode;

                if (string.IsNullOrEmpty(SimulationList.SimulationListCode))
                {
                    //编辑的模拟不存在
                   // WriteResponse(false, this.PortalResource.GetString("SimulationDetail_Mssg3"));
                    result.Message = "Simulation.SimulationDetail_Mssg3";
                    return Json(result);
                    
                }
                if (SimulationList.Simulations == null || SimulationList.Simulations.Length < 1)
                {
                    //编辑的模拟不存在
                   
                    result.Message = "Simulation.SimulationDetail_Mssg5";
                    return Json(result);
                }

                if (!string.IsNullOrEmpty(SimulationListID))
                {
                   result.Success = this.Engine.SimulationManager.UpdateSimulationList(SimulationList);
                }
                else
                {
                    result.Success =this.Engine.SimulationManager.AddSimulationList(SimulationList);
                }
                if (result.Success) { result.Message = "msgGlobalString.SaveSucced"; }
                else { result.Message = "msgGlobalString.SaveFailed"; }
                return Json(result);
            });
        }
    }
}
