using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
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
    public class SimulationController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        #region 测试用例

        /// <summary>
        /// 获取测试用例列表
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public JsonResult GetDataList(string workflowCode)
        {
            return ExecuteFunctionRun(() =>
            {

                InstanceSimulation[] Simulations = this.Engine.SimulationManager.GetSimulationByWorkflow(workflowCode);
                if (Simulations != null)
                {
                    List<string> lstSimulationNames = new List<string>();

                    List<string> lstUserIDs = new List<string>();
                    foreach (InstanceSimulation simulation in Simulations)
                    {
                        if (simulation.Originators != null)
                        {
                            foreach (string originator in simulation.Originators)
                            {
                                lstUserIDs.Add(originator);
                            }
                        }

                        lstSimulationNames.Add(simulation.InstanceName);
                    }
                    Simulations = OThinker.Data.Sorter<string, InstanceSimulation>.Sort(lstSimulationNames.ToArray(), Simulations);

                    OThinker.Organization.Unit[] Units = this.Engine.Organization.GetUnits(lstUserIDs.ToArray()).ToArray();
                    // User <ID,Name>
                    Dictionary<string, string> dicUserNames = new Dictionary<string, string>();
                    if (Units != null && Units.Length > 0)
                    {
                        foreach (OThinker.Organization.Unit u in Units)
                        {
                            if (u.UnitType == OThinker.Organization.UnitType.User && !dicUserNames.ContainsKey(u.ObjectID))
                            {
                                dicUserNames.Add(u.ObjectID, u.Name);
                            }
                        }
                    }

                    List<object> lstSimulationObjs = new List<object>();
                    foreach (InstanceSimulation simulation in Simulations)
                    {
                        string OriginatorString = string.Empty;
                        if (simulation.Originators != null)
                        {
                            foreach (string originator in simulation.Originators)
                            {
                                if (dicUserNames.ContainsKey(originator))
                                {
                                    OriginatorString += dicUserNames[originator] + ";";
                                }
                            }
                            OriginatorString = OriginatorString.TrimEnd(';');
                        }
                        lstSimulationObjs.Add(new
                        {
                            ObjectID = simulation.ObjectID,
                            InstanceName = simulation.InstanceName,
                            IsRunning = simulation.IsRunning.ToString(),
                            Originator = OriginatorString,
                            NextBatchNo = simulation.NextBatchNo,
                            LastRunTime = simulation.LastRunTime
                        });
                    }

                    var gridData = CreateLigerUIGridData(lstSimulationObjs.ToArray());
                    return Json(gridData);
                }

                var gridData2 = CreateLigerUIGridData(new object[] { });
                return Json(gridData2, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult LoadSimulation(string simulationID, string workflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                EditSimulationViewModel model = new EditSimulationViewModel();
                ActionResult result = new ActionResult(true);

                DataModel.BizObjectSchema Schema = null;
                OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;
                InstanceSimulation Simulation = null;

                if (!string.IsNullOrEmpty(workflowCode))
                {
                    Clause = this.Engine.WorkflowManager.GetClause(workflowCode);
                }
                if (Clause != null)
                {
                    Schema = this.Engine.BizObjectManager.GetPublishedSchema(Clause.BizSchemaCode);
                }

                if (Schema == null)
                {
                    result.Success = false;
                    result.Message = "EditBizObjectSchema.Msg0";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                WorkflowTemplate.PublishedWorkflowTemplate tempalte = this.Engine.WorkflowManager.GetDefaultWorkflow(workflowCode);
                if (tempalte == null)
                {
                    result.Success = false;
                    result.Message = "StartInstance.StartInstance_Msg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                if (string.IsNullOrEmpty(simulationID))
                {

                    model.WorkflowCode = workflowCode;
                    model.WorkflowName = this.Engine.WorkflowManager.GetClauseDisplayName(workflowCode);

                    ShowDataItems(workflowCode, Schema, null, null, model);
                }
                else
                {
                    Simulation = this.Engine.SimulationManager.GetSimulation(simulationID);
                    if (Simulation == null)
                    {
                        result.Success = false;
                        result.Message = "Simulation.SimulationDetail_Mssg1";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        if (Clause != null)
                        {
                            model.WorkflowName = Clause.WorkflowName + "[" + workflowCode + "]";
                        }

                        model.UseCaseName = Simulation.InstanceName;
                        model.Creator = JsonConvert.SerializeObject(Simulation.Originators);

                        //DataItems
                        ShowDataItems(workflowCode, Schema, Simulation, null, model);
                    }

                }
                result.Extend = model;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        public JsonResult SaveSimulation(EditSimulationViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                InstanceSimulation Simulation = null;

                ActionResult result = new ActionResult(false, "");

                if (string.IsNullOrEmpty(model.ObjectID))
                {
                    Simulation = new InstanceSimulation()
                    {
                        WorkflowCode = model.WorkflowCode
                    };
                }
                else
                {
                    Simulation = this.Engine.SimulationManager.GetSimulation(model.ObjectID);
                }
                if (Simulation == null)
                {
                    //编辑的模拟不存在
                    result.Message = "Simulation.SimulationDetail_Mssg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                Simulation.Originators = (string[])JsonConvert.DeserializeObject(model.Creator, typeof(string[])); //(model.Creator + string.Empty).Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                if (Simulation.Originators == null || Simulation.Originators.Length == 0)
                {
                    Simulation.Originators = new string[] { this.UserValidator.User.ObjectID };
                }
                Simulation.InstanceName = model.UseCaseName;

                if (string.IsNullOrEmpty(Simulation.InstanceName))
                {
                    //编辑的模拟不存在
                    result.Message = "Simulation.SimulationDetail_Mssg2";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                //数据项预设值
                Dictionary<string, string[]> DataItems = (Dictionary<string, string[]>)JsonConvert.DeserializeObject(model.DataItemsString + string.Empty, typeof(Dictionary<string, string[]>));
                Dictionary<string, bool> Ignores = (Dictionary<string, bool>)JsonConvert.DeserializeObject(model.IgnoresString + string.Empty, typeof(Dictionary<string, bool>));
                List<InstanceSimulationDataItem> Items = new List<InstanceSimulationDataItem>();
                foreach (string itemName in DataItems.Keys)
                {
                    Items.Add(new InstanceSimulationDataItem()
                    {
                        ItemName = itemName,
                        ItemValues = DataItems[itemName]
                    });
                }
                if (Ignores != null && Ignores.Count > 0)
                {
                    foreach (InstanceSimulationDataItem item in Items)
                    {
                        if (Ignores.ContainsKey(item.ItemName) && Ignores[item.ItemName] != null && Ignores[item.ItemName])
                        {
                            item.Ignore = true;
                        }
                    }
                }
                Simulation.DataItems = Items.ToArray();

                if (!string.IsNullOrEmpty(model.ObjectID))
                {
                    result.Success = this.Engine.SimulationManager.UpdateSimulation(Simulation);
                }
                else
                {
                    result.Success = this.Engine.SimulationManager.AddSimulation(Simulation);
                }

                if (result.Success) { result.Message = Simulation.ObjectID; }
                else { result.Message = "msgGlobalString.SaveFailed"; }

                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        public JsonResult ImportSimulation(string SquenceNo, string WorkflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");

                DataModel.BizObjectSchema Schema = null;
                OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;

                EditSimulationViewModel model = new EditSimulationViewModel();
                model.WorkflowCode = WorkflowCode;
                if (!string.IsNullOrEmpty(WorkflowCode))
                {
                    Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                }
                if (Clause != null)
                {
                    Schema = this.Engine.BizObjectManager.GetPublishedSchema(Clause.BizSchemaCode);
                }

                string[] conditions = null;
                string seqCondition = OThinker.H3.Instance.InstanceContext.TableName + "." + OThinker.H3.Instance.InstanceContext.PropertyName_SequenceNo + " ='" + SquenceNo + "'";
                conditions = OThinker.Data.ArrayConvertor<string>.AddToArray(conditions, seqCondition);
                DataTable dt = this.Engine.Query.QueryInstance(conditions, 0, 1);
                if (dt != null && dt.Rows.Count > 0)
                {
                    string InstanceId = dt.Rows[0][1] + string.Empty;
                    InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(InstanceId);
                    if (context != null)
                    {
                        if (context.WorkflowCode == WorkflowCode)
                        {
                            InstanceData dataitems = new InstanceData(this.Engine, InstanceId, this.UserValidator.UserID);
                            Dictionary<string, object> valuetable = dataitems.BizObject.ValueTable;

                            OThinker.Organization.User Originator = this.Engine.Organization.GetUnit(context.Originator) as OThinker.Organization.User;
                            if (Originator != null)
                            {
                                model.Originator = new { ObjectID = Originator.ObjectID, Name = Originator.Name };

                            }


                            string[] workitemIds = this.Engine.Query.QueryWorkItems(InstanceId, -1, WorkItem.WorkItemType.Unspecified, WorkItem.WorkItemState.Unspecified, OThinker.Data.BoolMatchValue.Unspecified);
                            if (workitemIds != null || workitemIds.Length > 0)
                            {
                                foreach (string workitemId in workitemIds)
                                {
                                    WorkItem.WorkItem workItem = this.Engine.WorkItemManager.GetWorkItem(workitemId);
                                    if (!valuetable.ContainsKey(workItem.ActivityCode))
                                        valuetable.Add(workItem.ActivityCode, workItem.Participant);
                                }
                            }

                            model.WorkflowName = context.InstanceName;
                            ShowDataItems(WorkflowCode, Schema, null, valuetable, model);
                        }
                        else
                        {
                            result.Message = "Simulation.EditSimulation_Mssg1";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else
                {
                    result.Message = "Simulation.EditSimulation_Mssg2";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.Success = true;
                result.Extend = model;
                result.Message = "msgGlobalString.ImportSucceed";
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        EditSimulationViewModel ShowDataItems(string workflowCode, DataModel.BizObjectSchema Schema, InstanceSimulation simulation, Dictionary<string, object> valueTable, EditSimulationViewModel model)
        {

            if (Schema == null || Schema.Properties == null)
                return model;

            //<ItemName,Values>
            WorkflowTemplate.PublishedWorkflowTemplate tempalte = this.Engine.WorkflowManager.GetDefaultWorkflow(workflowCode);
            Dictionary<string, string[]> ExistItems = new Dictionary<string, string[]>();
            Dictionary<string, bool> ActivityIgnore = new Dictionary<string, bool>();
            if (valueTable != null)
            {
                foreach (string key in valueTable.Keys)
                {
                    string[] values = new string[1];
                    values[0] = valueTable[key] + string.Empty;
                    ExistItems.Add(key, values);
                }
            }
            else if (simulation != null && simulation.DataItems != null && simulation.DataItems.Length > 0)
            {
                foreach (InstanceSimulationDataItem item in simulation.DataItems)
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
            //可设置的类型
            Data.DataLogicType[] SetableLogicTypes = new Data.DataLogicType[]{
                Data.DataLogicType.Bool,
                //Data.DataLogicType.Comment,
                Data.DataLogicType.DateTime,
                Data.DataLogicType.Decimal,
                Data.DataLogicType.Double,
                //Data.DataLogicType.Html,
                Data.DataLogicType.Int,
                Data.DataLogicType.Long,
                Data.DataLogicType.MultiParticipant, 
                Data.DataLogicType.ShortString,
                Data.DataLogicType.SingleParticipant,
                Data.DataLogicType.String,
                Data.DataLogicType.TimeSpan//,
                //Data.DataLogicType.Xml
            };
            List<string> UserIDs = new List<string>();
            foreach (DataModel.PropertySchema p in Schema.Properties)
            {
                if (!DataModel.BizObjectSchema.IsReservedProperty(p.Name)
                    && (p.LogicType == Data.DataLogicType.SingleParticipant || p.LogicType == Data.DataLogicType.MultiParticipant)
                    && ExistItems.ContainsKey(p.Name)
                    && ExistItems[p.Name] != null)
                {
                    foreach (string id in ExistItems[p.Name])
                    {
                        UserIDs.Add(id);
                    }
                }
            }
            foreach (WorkflowTemplate.Activity activity in tempalte.Activities)
            {
                if (activity.ActivityType == WorkflowTemplate.ActivityType.Start || activity.ActivityType == WorkflowTemplate.ActivityType.End)
                {
                    continue;
                }
                if (activity.ActivityCode == tempalte.StartActivityCode)
                {
                    continue;
                }

                if (ExistItems.ContainsKey(activity.ActivityCode))
                {
                    foreach (string id in ExistItems[activity.ActivityCode])
                    {
                        UserIDs.Add(id);
                    }
                }
            }
            OThinker.Organization.Unit[] Units = this.Engine.Organization.GetUnits(UserIDs.ToArray()).ToArray();
            //<UserID,UserName>
            Dictionary<string, string> DicUnits = new Dictionary<string, string>();
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

            List<object> lstDataItems = new List<object>();
            foreach (DataModel.PropertySchema p in Schema.Properties)
            {
                if (DataModel.BizObjectSchema.IsReservedProperty(p.Name))
                    continue;
                if (!SetableLogicTypes.Contains(p.LogicType))
                {
                    continue;
                }

                string DisplayValueString = string.Empty;
                if (ExistItems.ContainsKey(p.Name) && ExistItems[p.Name] != null && ExistItems[p.Name].Length > 0)
                {
                    if (p.LogicType == Data.DataLogicType.SingleParticipant || p.LogicType == Data.DataLogicType.MultiParticipant)
                    {
                        foreach (string _Value in ExistItems[p.Name])
                        {
                            if (DicUnits.ContainsKey(_Value))
                            {
                                DisplayValueString += DicUnits[_Value] + ";";
                            }
                            else
                            {
                                DisplayValueString += ";";
                            }
                        }
                    }
                    else
                    {
                        DisplayValueString = string.Join(";", ExistItems[p.Name]);
                    }
                }
                lstDataItems.Add(new
                {
                    ItemName = p.Name,
                    ItemValues = ExistItems.ContainsKey(p.Name) ? ExistItems[p.Name] : null,
                    DisplayValueString = DisplayValueString,
                    LogicType = OThinker.H3.Data.DataLogicTypeConvertor.ToLogicTypeName(p.LogicType),
                    Editable = SetableLogicTypes.Contains(p.LogicType)
                });
            }

            List<object> lstActivitys = new List<object>();
            foreach (WorkflowTemplate.Activity activity in tempalte.Activities)
            {
                if (activity.ActivityType == WorkflowTemplate.ActivityType.Start || activity.ActivityType == WorkflowTemplate.ActivityType.End)
                {
                    continue;
                }
                if (activity.ActivityCode == tempalte.StartActivityCode)
                {
                    continue;
                }
                string DisplayValueString = string.Empty;
                if (ExistItems.ContainsKey(activity.ActivityCode) && ExistItems[activity.ActivityCode] != null && ExistItems[activity.ActivityCode].Length > 0)
                {

                    foreach (string _Value in ExistItems[activity.ActivityCode])
                    {
                        if (DicUnits.ContainsKey(_Value))
                        {
                            DisplayValueString += DicUnits[_Value] + ";";
                        }
                        else
                        {
                            DisplayValueString += ";";
                        }
                    }

                }
                lstActivitys.Add(new
                {
                    WorkflowTemplate = workflowCode,
                    ActivityCode = activity.ActivityCode,
                    ActivityName = activity.DisplayName,
                    Participants = ExistItems.ContainsKey(activity.ActivityCode) ? ExistItems[activity.ActivityCode] : null,
                    DisplayValueString = DisplayValueString,
                    Ignore = ActivityIgnore.ContainsKey(activity.ActivityCode) ? ActivityIgnore[activity.ActivityCode] : false,
                    Editable = true
                });
            }

            model.DataItems = CreateLigerUIGridData(lstDataItems.ToArray());
            model.Activitys = CreateLigerUIGridData(lstActivitys.ToArray());

            return model;
        }
      
        /// <summary>
        /// 加载数据，返回类型及显示名称
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="ActivityCode"></param>
        /// <param name="ItemName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public JsonResult LoadEditSimulationData(string WorkflowCode, string ActivityCode, string ItemName, string Value)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                BizObjectSchema Schema = null;
                OThinker.H3.WorkflowTemplate.WorkflowClause Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                if (Clause != null)
                {
                    Schema = this.Engine.BizObjectManager.GetPublishedSchema(Clause.BizSchemaCode);
                }
                else
                {
                    result.Message = "Simulation.EditSimulation_Mssg3";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                PropertySchema PropertySchema = null;
                if (Schema != null)
                {
                    PropertySchema = Schema.GetProperty(ItemName);
                }
                else
                {
                    result.Message = "Simulation.EditSimulation_Mssg3";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                var DisplayName = "";
                var LogicTypeDisplay = "";
                var LogicType = "";

                if (PropertySchema != null)
                {
                    DisplayName = PropertySchema.DisplayName;
                    LogicTypeDisplay = OThinker.H3.Data.DataLogicTypeConvertor.ToLogicTypeName(PropertySchema.LogicType);
                    LogicType = PropertySchema.LogicType.ToString();
                }
                else if (!string.IsNullOrEmpty(ActivityCode))
                {
                    OThinker.H3.WorkflowTemplate.WorkflowDocument document = this.Engine.WorkflowManager.GetPublishedTemplate(WorkflowCode, this.Engine.WorkflowManager.GetWorkflowDefaultVersion(WorkflowCode));
                    foreach (OThinker.H3.WorkflowTemplate.Activity activity in document.Activities)
                    {
                        if (activity.ActivityCode == ActivityCode)
                        {
                            DisplayName = activity.DisplayName;
                            LogicType = string.Empty;
                        }
                    }
                }
                else
                {
                    result.Message = "Simulation.EditSimulationData_Mssg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.Success = true;
                result.Extend = new { DisplayName = DisplayName, LogicTypeDisplay = LogicTypeDisplay, LogicType = LogicType };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 运行测试用例
        /// </summary>
        /// <param name="ObjectIDString"></param>
        /// <returns></returns>
        public JsonResult RunSimulation(string ObjectIDString)
        {
            return ExecuteFunctionRun(() =>
            {

                ActionResult result = new ActionResult(true, "");
                string[] ObjectIDs = (ObjectIDString ?? "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (ObjectIDs.Length > 0)
                {
                    this.Engine.SimulationManager.RunSimulation(ObjectIDs);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        #endregion

        #region 测试用例集
        /// <summary>
        /// 测试集列表
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="page"></param>
        /// <param name="pagesize"></param>
        /// <returns></returns>
        public JsonResult GetSimulationList(string workflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                InstanceSimulationList[] SimulationLists = this.Engine.SimulationManager.GetSimulationListByWorkflow(workflowCode);
                if (SimulationLists != null)
                {
                    List<string> simulationIDs = new List<string>();
                    foreach (InstanceSimulationList simulationList in SimulationLists)
                    {
                        if (simulationList.Simulations != null)
                        {
                            foreach (string simulationID in simulationList.Simulations)
                            {
                                simulationIDs.Add(simulationID);
                            }
                        }
                    }

                    Dictionary<string, string> simulationNames = new Dictionary<string, string>();
                    InstanceSimulation[] simulations = this.Engine.SimulationManager.GetSimulations(simulationIDs.ToArray());
                    if (simulations != null || simulations.Length > 0)
                    {

                        foreach (InstanceSimulation simulation in simulations)
                        {
                            if (simulation != null)
                                if (!simulationNames.ContainsKey(simulation.ObjectID))
                                {
                                    simulationNames.Add(simulation.ObjectID, simulation.InstanceName);
                                }
                        }
                    }

                    List<object> lstSimulationObjs = new List<object>();
                    foreach (InstanceSimulationList simulationList in SimulationLists)
                    {
                        string simulationName = string.Empty;
                        foreach (string simulation in simulationList.Simulations)
                        {
                            if (simulationNames.ContainsKey(simulation))
                                simulationName += simulationNames[simulation] + ";";
                        }

                        lstSimulationObjs.Add(new
                        {
                            ObjectID = simulationList.ObjectID,
                            SimulationCode = simulationList.SimulationListCode,
                            Simulations = simulationName,
                            LastRunTime = simulationList.LastRunTime
                        });
                    }

                    var gridData = CreateLigerUIGridData(lstSimulationObjs.ToArray());
                    return Json(gridData);
                }
                var gridData2 = CreateLigerUIGridData(new object[] { });
                return Json(gridData2, JsonRequestBehavior.AllowGet);

            });
        }

        public JsonResult LoadSimulationList(string SimulationListID, string WorkflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");

                InstanceSimulationList SimulationList = null;
                OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;

                Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                SimulationList = this.Engine.SimulationManager.GetSimulationList(SimulationListID);

                var WorkflowName = "";
                var SimulationListCode = "";
                string simulation = string.Empty;//当前测试集的测试用例ID
                var Simulations = new List<object>();//当前流程测试用例集合

                //ID非空为编辑模式
                if (!string.IsNullOrEmpty(SimulationListID))
                {
                    if (SimulationList != null)
                    {
                        if (Clause != null)
                        {
                            WorkflowName = Clause.WorkflowName + "[" + WorkflowCode + "]";
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

        public JsonResult SaveSimulationList(string SimulationListID, string SimulationListCode, string WorkflowCode, string SimulationIDS)
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
                }
                else
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
                    result.Success = this.Engine.SimulationManager.AddSimulationList(SimulationList);
                }
                if (result.Success) { result.Message = "msgGlobalString.SaveSucced"; }
                else { result.Message = "msgGlobalString.SaveFailed"; }
                return Json(result);
            });
        }
   

        /// <summary>
        /// 运行测试用例集
        /// </summary>
        /// <param name="ObjectIDString"></param>
        /// <returns></returns>
        public JsonResult RunSimulationList(string ObjectIDString)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] ObjectIDs = (ObjectIDString ?? "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (ObjectIDs.Length > 0)
                {
                    this.Engine.SimulationManager.RunSimulationList(ObjectIDs);
                }
                return null;
            });
        }

        /// <summary>
        /// 删除测试用例
        /// </summary>
        /// <param name="ObjectIDString"></param>
        /// <returns></returns>
        public JsonResult DeleteSimulation(string ObjectIDString)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] ObjectIDs = (ObjectIDString ?? "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                this.Engine.SimulationManager.RemoveSimulation(ObjectIDs);
                return null;
            });
        }
        /// <summary>
        /// 删除测试用例集
        /// </summary>
        /// <param name="ObjectIDString"></param>
        /// <returns></returns>
        public JsonResult DeleteSimulationList(string ObjectIDString)
        {
            return ExecuteFunctionRun(() =>
            {
                string[] ObjectIDs = (ObjectIDString ?? "").Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                this.Engine.SimulationManager.RemoveSimulationList(ObjectIDs);
                return null;
            });
        }
        #endregion

        #region  SimulationInstanceInfo
        public JsonResult LoadSimulationInstanceInfo(string SimulationID, string InstanceId)
        {
            return ExecuteFunctionRun(() =>
            {

                return Json(LoadInstances(SimulationID, InstanceId), JsonRequestBehavior.AllowGet);
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

            var gridData = CreateLigerUIGridData(InstanceObjs.ToArray());

            var obj = new
            {
                InstanceName = InstanceName,
                Originator = Originator,
                State = State,
                WorkItemCount = WorkItemCount,
                SuccessCount = SuccessCount,
                FailedCount = FailedCount,
                IgnoreCount = IgnoreCount.ToString(),
                Percent = PercentString,
                Instances = gridData
            };

            return obj;
            //ScriptManager.RegisterClientScriptBlock(this.Page, this.Page.GetType(), "Instances", "var Instances=" + CreateLigerUIGridData(InstanceObjs.ToArray()) + ";", true);
        }
        #endregion 

        #region SimulationInstanceList
        public JsonResult LoadSimulationInstanceList(string SimulationID, string SimulationToken, string SchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                var data = LoadInstanceList(SimulationID, SimulationToken, SchemaCode);
                return Json(data, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SimulationID"></param>
        /// <param name="SimulationToken"></param>
        /// <param name="SchemaCode">流程编码</param>
        /// <returns></returns>
        private object LoadInstanceList(string SimulationID, string SimulationToken, string SchemaCode)
        {
            if (string.IsNullOrEmpty(SimulationID))
            {
                return null;
            }
            InstanceSimulation Simulation = this.Engine.SimulationManager.GetSimulation(SimulationID);
            if (Simulation == null || Simulation.NextBatchNo <= InstanceSimulationLog.InitialBatchNo)
                return null;

            //根据流程编码查找SchemaCode
            string bizChemaCode = this.Engine.WorkflowManager.GetClause(SchemaCode).BizSchemaCode;
            //查看批次号
            int SimulationTokenID = 0;
            int.TryParse(SimulationToken + string.Empty, out SimulationTokenID);
            if (SimulationTokenID <= InstanceSimulationLog.InitialBatchNo || SimulationTokenID >= Simulation.NextBatchNo)
            {
                SimulationTokenID = Simulation.NextBatchNo - 1;
            }
            object gridData = null;
            DataTable InstanceTable = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(
                " SELECT "
                + "A." + InstanceSimulationLog.PropertyName_DataItemsSummary
                + ",B." + InstanceContext.C_ObjectID
                + ",B." + InstanceContext.PropertyName_Originator
                + ",B." + InstanceContext.PropertyName_InstanceName
                + ",B." + InstanceContext.PropertyName_State
                + " FROM " + InstanceSimulationLog.TableName + " A JOIN " + InstanceContext.TableName + " B "
                + " ON A." + InstanceSimulationLog.PropertyName_InstanceId + "=B." + InstanceContext.C_ObjectID
                + " WHERE A." + InstanceSimulationLog.PropertyName_SimulationID + "='" + SimulationID + "'"
                + " AND A." + InstanceSimulationLog.PropertyName_BatchNo + "=" + SimulationTokenID + " ");

            if (InstanceTable != null && InstanceTable.Rows.Count > 0)
            {
                //一次获取所有用户
                List<string> lstUserIDs = new List<string>();
                foreach (DataRow row in InstanceTable.Rows)
                {
                    lstUserIDs.Add(row[InstanceContext.PropertyName_Originator] + string.Empty);
                }
                OThinker.Organization.Unit[] Units = this.Engine.Organization.GetUnits(lstUserIDs.ToArray()).ToArray();
                //<UserID,UserName>
                Dictionary<string, string> DicUnits = new Dictionary<string, string>();
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

                //输出到前台的表格数据
                List<object> InstanceObjs = new List<object>();
                foreach (DataRow row in InstanceTable.Rows)
                {
                    string DataItemsSummary = row[InstanceSimulationLog.PropertyName_DataItemsSummary] + string.Empty;

                    string instanceId = row[InstanceContext.C_ObjectID] + string.Empty;
                    Dictionary<string, string> NameValues = InstanceSimulationLog.DeserializeItemSummary(DataItemsSummary);
                    OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(bizChemaCode);
                    Dictionary<string, string> ItemValues = new Dictionary<string, string>();
                    List<string> participants = new List<string>();
                    foreach (string key in NameValues.Keys)
                    {
                        OThinker.H3.DataModel.FieldSchema field = schema.GetField(key);
                        if (field != null && (field.LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant || field.LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant))
                        {
                            participants.Add(NameValues[key]);
                        }
                    }
                    OThinker.Organization.Unit[] units = this.Engine.Organization.GetUnits(participants.ToArray()).ToArray();
                    Dictionary<string, string> names = new Dictionary<string, string>();
                    foreach (OThinker.Organization.Unit unit in units)
                    {
                        if (!names.ContainsKey(unit.ObjectID))
                            names.Add(unit.ObjectID, unit.Name);
                    }
                    foreach (string key in NameValues.Keys)
                    {
                        OThinker.H3.DataModel.FieldSchema field = schema.GetField(key);
                        if (field != null && (field.LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant || field.LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant))
                        {
                            if (names.ContainsKey(NameValues[key]))
                                ItemValues.Add(key, names[NameValues[key]]);
                        }
                        else
                        {
                            ItemValues.Add(key, NameValues[key]);
                        }
                    }
                    InstanceObjs.Add(new
                    {
                        ObjectID = instanceId,
                        Originator = DicUnits.ContainsKey(row[InstanceContext.PropertyName_Originator] + string.Empty) ? DicUnits[row[InstanceContext.PropertyName_Originator] + string.Empty] : "",
                        InstanceName = row[InstanceContext.PropertyName_InstanceName] + string.Empty,
                        State = "InstanceState.InstanceState_" + Enum.Parse(typeof(InstanceState), row[InstanceContext.PropertyName_State] + string.Empty).ToString(),
                        Url = this.GetInstanceUrl(instanceId, "", DateTime.Now.ToString("yyyyMMddHHmmss")),
                        DataItems = ItemValues
                    });
                }
                gridData = CreateLigerUIGridData(InstanceObjs.ToArray());
            }

            if (gridData == null)
            {
                gridData = new { Rows = new object[] { }, Total = 0 };
            }
            var data = new
            {
                SimulationName = Simulation.InstanceName,
                Instances = gridData
            };

            return data;
        }
        #endregion

        #region SimulationListReport

        public JsonResult LoadSimulationListReport(string SimulationListID)
        {
            return ExecuteFunctionRun(() =>
            {
                return Json(LoadInstanceReport(SimulationListID), JsonRequestBehavior.AllowGet);
            });
        }

        object LoadInstanceReport(string SimulationListID)
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
                    if (lstInstanceIds != null && lstInstanceIds.Count > 0)
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
                            Url = this.GetInstanceUrl(InstanceId, "", DateTime.Now.ToString("yyyyMMddHHmmss")),
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
            var RunTime = DateTime.MinValue.ToLongDateString();
            if (SimulationLists.LastRunTime > DateTime.Parse("1753-1-1") && SimulationLists.LastRunTime < DateTime.MaxValue)
                RunTime = SimulationLists.LastRunTime.ToLongDateString();
            var SimulationNum = total + string.Empty;

            var FailedNum = failed + string.Empty;
            var PercentString = percent + "%";

            var gridData = CreateLigerUIGridData(InstanceObjs.ToArray());

            results.Success = true;
            results.Extend = new
            {
                GridData = gridData,
                SimulationListCode = SimulationListCode,
                RunTime = RunTime,
                SimulationNum = SimulationNum,
                FailedNum = FailedNum,
                SuccessNum = SuccessNum.ToString(),
                Percent = PercentString
            };

            return results;
        }

        #endregion

    }
}
