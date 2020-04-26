using OThinker.H3.Instance;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class SimulationListReportController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult Load(string SimulationListID)
        {
            return ExecuteFunctionRun(() => {

                return Json(LoadInstances(SimulationListID), JsonRequestBehavior.AllowGet);
                
            });
        }

       object LoadInstances(string SimulationListID)
        {
            ActionResult results = new ActionResult(false, "");

            if (string.IsNullOrEmpty(SimulationListID))
            {
                return null;
            }
            InstanceSimulationList SimulationLists = this.Engine.SimulationManager.GetSimulationList(SimulationListID);
            if (SimulationLists == null || SimulationLists.Simulations == null)
                return null;

            Dictionary<string, string[]> ExistItems = new Dictionary<string, string[]>();
            Dictionary<string, bool> ActivityIgnore = new Dictionary<string, bool>();
            List<object> InstanceObjs = new List<object>();

            InstanceSimulation[] simulations = this.Engine.SimulationManager.GetSimulations(SimulationLists.Simulations);
            Dictionary<string, string> DicUnits = new Dictionary<string, string>();
            Dictionary<string, string> lstInstanceIds = new Dictionary<string, string>();
            Dictionary<string, string> lstState = new Dictionary<string, string>();
            List<string> lstUserIDs = new List<string>();
            Dictionary<string, int> result = new Dictionary<string, int>();

            if (simulations != null || simulations.Length > 0)
            {
                foreach (InstanceSimulation Simulation in simulations)
                {
                    if (Simulation.NextBatchNo < 2)
                    {
                        results.Message = "Simulation.SimulationReport_Mssg";
                        return results;
                    }

                    if (Simulation.DataItems != null && Simulation.DataItems.Length > 0)
                    {
                        foreach (InstanceSimulationDataItem item in Simulation.DataItems)
                        {
                            if (!ExistItems.ContainsKey(Simulation.ObjectID + item.ItemName))
                            {
                                ExistItems.Add(Simulation.ObjectID + item.ItemName, item.ItemValues);
                            }
                            if (!ActivityIgnore.ContainsKey(Simulation.ObjectID + item.ItemName))
                            {
                                ActivityIgnore.Add(Simulation.ObjectID + item.ItemName, item.Ignore);
                            }
                        }
                    }

                    int SimulationTokenID = 0;
                    if (SimulationTokenID <= InstanceSimulationLog.InitialBatchNo || SimulationTokenID >= Simulation.NextBatchNo)
                    {
                        SimulationTokenID = Simulation.NextBatchNo - 1;
                    }

                    DataTable InstanceTable = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(
                    " SELECT "
                    + "B." + InstanceContext.C_ObjectID
                    + ",B." + InstanceContext.PropertyName_Originator
                    + ",B." + InstanceContext.PropertyName_InstanceName
                    + ",B." + InstanceContext.PropertyName_State
                    + " FROM " + InstanceSimulationLog.TableName + " A JOIN " + InstanceContext.TableName + " B "
                    + " ON A." + InstanceSimulationLog.PropertyName_InstanceId + "=B." + InstanceContext.C_ObjectID
                    + " WHERE A." + InstanceSimulationLog.PropertyName_SimulationID + "='" + Simulation.ObjectID + "'"
                    + " AND A." + InstanceSimulationLog.PropertyName_BatchNo + "=" + SimulationTokenID + " ");

                    if (InstanceTable != null && InstanceTable.Rows.Count > 0)
                    {
                        //一次获取所有用户
                        foreach (DataRow row in InstanceTable.Rows)
                        {
                            lstUserIDs.Add(row[InstanceContext.PropertyName_Originator] + string.Empty);
                            if (!lstInstanceIds.ContainsKey(Simulation.ObjectID))
                            {
                                lstInstanceIds.Add(Simulation.ObjectID, row[InstanceContext.C_ObjectID] + string.Empty);
                            }
                            if (!lstState.ContainsKey(Simulation.ObjectID))
                            {
                                lstState.Add(Simulation.ObjectID, row[InstanceContext.PropertyName_State] + string.Empty);
                            }
                        }
                        OThinker.Organization.Unit[] Units = this.Engine.Organization.GetUnits(lstUserIDs.ToArray()).ToArray();
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
                foreach (InstanceSimulation Simulation in simulations)
                {
                    List<string> message = new List<string>();

                    int workItemCount = 0;
                    int failedCount = 0;
                    int IgnoreCount = 0;
                    bool exception = false;
                    if (lstInstanceIds != null && lstInstanceIds.Count>0)
                    {
                        string InstanceId = lstInstanceIds[Simulation.ObjectID];
                        InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(InstanceId);
                        if (!context.Exceptional)
                        {
                            string[] workitemIds = this.Engine.Query.QueryWorkItems(InstanceId, -1, OThinker.H3.WorkItem.WorkItemType.Unspecified, OThinker.H3.WorkItem.WorkItemState.Finished, OThinker.Data.BoolMatchValue.Unspecified);
                            if (workitemIds != null || workitemIds.Length > 0)
                            {
                                foreach (string workitemId in workitemIds)
                                {
                                    OThinker.H3.WorkItem.WorkItem workItem = this.Engine.WorkItemManager.GetWorkItem(workitemId);
                                    if (ActivityIgnore.ContainsKey(Simulation.ObjectID + workItem.ActivityCode))
                                    {
                                        if (!ActivityIgnore[Simulation.ObjectID + workItem.ActivityCode])
                                        {
                                            if (ExistItems.ContainsKey(Simulation.ObjectID + workItem.ActivityCode))
                                            {
                                                if (ExistItems[Simulation.ObjectID + workItem.ActivityCode] == null || ExistItems[Simulation.ObjectID + workItem.ActivityCode].Length == 0)
                                                {
                                                    IgnoreCount++;
                                                }
                                                else if (!ExistItems[Simulation.ObjectID + workItem.ActivityCode].Contains(workItem.Participant))
                                                    failedCount++;

                                            }
                                        }
                                        else
                                        {
                                            IgnoreCount++;
                                        }
                                    }
                                }
                                workItemCount = workitemIds.Length - IgnoreCount;
                            }
                        }
                        else
                        {
                            exception = true;
                            if (!result.ContainsKey(Simulation.ObjectID))
                            {
                                result.Add(Simulation.ObjectID, 1);
                            }
                        }
                        if (failedCount > 0 && !result.ContainsKey(Simulation.ObjectID))
                            result.Add(Simulation.ObjectID, 1);

                        double Percent = 0;
                        if (workItemCount > 0)
                            Percent = (workItemCount - failedCount) * 100 / workItemCount;
                        InstanceObjs.Add(new
                        {
                            ObjectID = InstanceId,
                            Originator = DicUnits.ContainsKey(Simulation.Originators[0]) ? DicUnits[Simulation.Originators[0]] : "",
                            InstanceName = Simulation.InstanceName,
                            State = (exception ? "Simulation.InstanceGrid_Exceptional" : "InstanceState.InstanceState_" + Enum.Parse(typeof(InstanceState), lstState[Simulation.ObjectID] + string.Empty).ToString()),
                            Url = this.GetInstanceUrl(InstanceId,"", DateTime.Now.ToString("yyyyMMddHHmmss")),
                            workItemCount = workItemCount + IgnoreCount,
                            SuccessCount = workItemCount - failedCount,
                            failedCount = failedCount,
                            IgnoreCount = IgnoreCount,
                            Percent = Percent + "%",
                            SimulationID = Simulation.ObjectID
                        });
                    }
                }
            }

            List<object> lstSimulationObjs = new List<object>();

            int failed = 0;
            foreach (string simulationId in SimulationLists.Simulations)
            {
                if (result.ContainsKey(simulationId))
                    failed += result[simulationId];
            }

            int total = SimulationLists.Simulations.Length;
            int SuccessNum = SimulationLists.Simulations.Length - failed;
            double percent = SuccessNum * 100 / total;

            var SimulationListCode = SimulationLists.SimulationListCode;
            var RunTime=DateTime.MinValue.ToLongDateString();
            if (SimulationLists.LastRunTime > DateTime.Parse("1753-1-1") && SimulationLists.LastRunTime < DateTime.MaxValue)
                RunTime = SimulationLists.LastRunTime.ToLongDateString();
            var SimulationNum = total + string.Empty;
            
            var FailedNum = failed + string.Empty;
            var PercentString = percent + "%";

            var gridData =CreateLigerUIGridData(InstanceObjs.ToArray());

           results.Success=true;
           results.Extend = new{
                      GridData=gridData,
                      SimulationListCode=SimulationListCode,
                      RunTime=RunTime,
                      SimulationNum =SimulationNum,
                      FailedNum=FailedNum,
                      SuccessNum=SuccessNum.ToString(),
                      Percent=PercentString
                   };

          return results;
        }


    }
}
