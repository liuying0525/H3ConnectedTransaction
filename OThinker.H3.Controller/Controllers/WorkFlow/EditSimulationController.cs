using Newtonsoft.Json;
using OThinker.H3.Controllers.ViewModels;
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
   public class EditSimulationController:ControllerBase
    {
       public override string FunctionCode
       {
           get { return ""; }
       }

       public JsonResult LoadSimulation(string simulationID, string workflowCode)
       {
           return ExecuteFunctionRun(() => {
               EditSimulationViewModel model = new EditSimulationViewModel();
               ActionResult result = new ActionResult(true);

               DataModel.BizObjectSchema Schema = null;
               OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;
               InstanceSimulation Simulation =null;

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

                   ShowDataItems(workflowCode, Schema, null,null,model);
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

                       model.UseCaseName= Simulation.InstanceName;
                       model.Creator = JsonConvert.SerializeObject(Simulation.Originators);

                       //DataItems
                       ShowDataItems(workflowCode,Schema,Simulation,null,model);
                   }
                   
               }
               result.Extend = model;
               return Json(result, JsonRequestBehavior.AllowGet);
           });
       }

       public JsonResult SaveSimulation(EditSimulationViewModel model)
       {
           return ExecuteFunctionRun(() => {
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
                   result.Message="Simulation.SimulationDetail_Mssg2";
                   return Json(result, JsonRequestBehavior.AllowGet);
               }

               //数据项预设值
               Dictionary<string, string[]> DataItems = (Dictionary<string, string[]>) JsonConvert.DeserializeObject(model.DataItemsString + string.Empty, typeof(Dictionary<string, string[]>));
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
                   result.Success =this.Engine.SimulationManager.UpdateSimulation(Simulation);
               }
               else
               {
                   result.Success =this.Engine.SimulationManager.AddSimulation(Simulation);
               }

               if (result.Success) { result.Message = Simulation.ObjectID; }
               else { result.Message = "msgGlobalString.SaveFailed"; }

               return Json(result, JsonRequestBehavior.AllowGet);
           });
          
       }

       public JsonResult ImportSimulation(string SquenceNo, string WorkflowCode)
       {
           return ExecuteFunctionRun(() => {
               ActionResult result = new ActionResult(false,"");

               DataModel.BizObjectSchema Schema = null;
               OThinker.H3.WorkflowTemplate.WorkflowClause Clause = null;

               EditSimulationViewModel model =new EditSimulationViewModel();
               model.WorkflowCode=WorkflowCode;
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
                               model.Originator = new { ObjectID=Originator.ObjectID, Name =Originator.Name };

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

                           model.WorkflowName =context.InstanceName;
                           ShowDataItems(WorkflowCode,Schema,null, valuetable,model);
                       }
                       else
                       {
                           result.Message ="Simulation.EditSimulation_Mssg1";
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
    }
}
