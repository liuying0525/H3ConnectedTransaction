using OThinker.H3.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class SimulationInstanceInforController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult Load(string SimulationID, string InstanceId)
        {
            return ExecuteFunctionRun(() => {

                return Json(LoadInstances(SimulationID, InstanceId),JsonRequestBehavior.AllowGet);
            });
        }

        private object LoadInstances(string SimulationID, string InstanceId)
        {
            if (string.IsNullOrEmpty(SimulationID))
            {
                return null;
            }
            InstanceSimulation Simulation = this.Engine.SimulationManager.GetSimulation(SimulationID);
            if (Simulation == null || Simulation.NextBatchNo <= InstanceSimulationLog.InitialBatchNo)
                return null;
            InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(InstanceId);
            if (context == null)
                return null;

            var InstanceName = Simulation.InstanceName;
            var Originator = this.Engine.Organization.GetName(Simulation.Originators[0]);
            var State = "InstanceState.InstanceState_" + Enum.Parse(typeof(InstanceState), (int)context.State + string.Empty).ToString();

            Dictionary<string, bool> ActivityIgnore = new Dictionary<string, bool>();
            Dictionary<string, string[]> ExistItems = new Dictionary<string, string[]>();
            Dictionary<string, string> DicUnits = new Dictionary<string, string>();
            if (Simulation.DataItems != null && Simulation.DataItems.Length > 0)
            {
                foreach (InstanceSimulationDataItem item in Simulation.DataItems)
                {
                    if (!ExistItems.ContainsKey(item.ItemName))
                    {
                        ExistItems.Add(item.ItemName, item.ItemValues);
                    }
                    if (!ActivityIgnore.ContainsKey(item.ItemName))
                    {
                        ActivityIgnore.Add(item.ItemName, item.Ignore);
                    }
                }
            }

            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate tempalte = this.Engine.WorkflowManager.GetDefaultWorkflow(Simulation.WorkflowCode);
            foreach (OThinker.H3.WorkflowTemplate.Activity activity in tempalte.Activities)
            {
                if (ExistItems.ContainsKey(activity.ActivityCode))
                {
                    OThinker.Organization.Unit[] Units = this.Engine.Organization.GetUnits(ExistItems[activity.ActivityCode]).ToArray();
                    if (Units != null)
                    {
                        foreach (OThinker.Organization.Unit u in Units)
                        {
                            if (!DicUnits.ContainsKey(u.ObjectID))
                            {
                                DicUnits.Add(u.ObjectID, u.Name);
                            }
                        }
                    }
                }
            }

            int failedCount = 0;
            int IgnoreCount = 0;

            //输出到前台的表格数据
            List<object> InstanceObjs = new List<object>();

            string[] workitemIds = this.Engine.Query.QueryWorkItems(InstanceId, -1, OThinker.H3.WorkItem.WorkItemType.Unspecified, OThinker.H3.WorkItem.WorkItemState.Finished, OThinker.Data.BoolMatchValue.Unspecified);
            string[] workitemIdsUnfinished = this.Engine.Query.QueryWorkItems(InstanceId, -1, OThinker.H3.WorkItem.WorkItemType.Unspecified, OThinker.H3.WorkItem.WorkItemState.Unfinished, OThinker.Data.BoolMatchValue.Unspecified);
            workitemIds = OThinker.Data.ArrayConvertor<string>.Add(workitemIds, workitemIdsUnfinished);

            if (workitemIds != null || workitemIds.Length > 0)
            {
                foreach (string workitemId in workitemIds)
                {
                    OThinker.H3.WorkItem.WorkItem workItem = this.Engine.WorkItemManager.GetWorkItem(workitemId);
                    if (ActivityIgnore.ContainsKey(workItem.ActivityCode))
                    {
                        if (!ActivityIgnore[workItem.ActivityCode])
                        {
                            if (ExistItems.ContainsKey(workItem.ActivityCode))
                            {
                                if (ExistItems[workItem.ActivityCode] == null || ExistItems[workItem.ActivityCode].Length == 0)
                                {
                                    IgnoreCount++;
                                }
                                else if (!ExistItems[workItem.ActivityCode].Contains(workItem.Participant))
                                    failedCount++;
                            }
                        }
                        else
                        {
                            IgnoreCount++;
                        }
                        string UnitNames = string.Empty;
                        foreach (string id in ExistItems[workItem.ActivityCode])
                        {
                            if (DicUnits.ContainsKey(id))
                            {
                                UnitNames += DicUnits[id] + ";";
                            }
                        }

                        bool result = true;
                        if (!ActivityIgnore[workItem.ActivityCode] && ExistItems[workItem.ActivityCode].Length > 0 && !ExistItems[workItem.ActivityCode].Contains(workItem.Participant))
                        {
                            result = false;
                        }

                        InstanceObjs.Add(new
                        {
                            WorkItemName = workItem.DisplayName,
                            Participant = this.Engine.Organization.GetName(workItem.Participant),
                            ExisItem = UnitNames.TrimEnd(';'),
                            Ignore = ActivityIgnore[workItem.ActivityCode] ? "Simulation.SheetPrint_Yes" : "Simulation.SheetPrint_No",
                            Result = result
                        });
                    }
                }
            }

            var WorkItemCount = workitemIds.Length + string.Empty;
            var SuccessCount = (workitemIds.Length - IgnoreCount - failedCount) + string.Empty;
            var FailedCount = failedCount + string.Empty;

            double Percent = 0;
            if (workitemIds.Length > 0)
                Percent = (workitemIds.Length - IgnoreCount - failedCount) * 100 / (workitemIds.Length - IgnoreCount);
            var PercentString = Percent + "%";

            var gridData =CreateLigerUIGridData(InstanceObjs.ToArray());

            var obj = new
            {
                InstanceName = InstanceName,
                Originator = Originator,
                State = State,
                WorkItemCount = WorkItemCount,
                SuccessCount = SuccessCount,
                FailedCount = FailedCount,
                IgnoreCount=IgnoreCount.ToString(),
                Percent = PercentString,
                Instances = gridData
            };

            return obj;
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "Instances", "var Instances=" + CreateLigerUIGridData(InstanceObjs.ToArray()) + ";", true);
        }
    }
}
