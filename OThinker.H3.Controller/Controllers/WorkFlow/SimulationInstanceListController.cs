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
    public class SimulationInstanceListController : ControllerBase
    {

        public override string FunctionCode
        {
            get { return ""; }
        }

        public JsonResult Load(string SimulationID, string SimulationToken, string SchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                var data = LoadInstances(SimulationID, SimulationToken, SchemaCode);
                return Json(data,JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="SimulationID"></param>
        /// <param name="SimulationToken"></param>
        /// <param name="SchemaCode">流程编码</param>
        /// <returns></returns>
        private object LoadInstances(string SimulationID, string SimulationToken, string SchemaCode)
        {
            if (string.IsNullOrEmpty(SimulationID))
            {
                return null;
            }
            InstanceSimulation Simulation = this.Engine.SimulationManager.GetSimulation(SimulationID);
            if (Simulation == null || Simulation.NextBatchNo <= InstanceSimulationLog.InitialBatchNo)
                return null;

            //根据流程编码查找SchemaCode
           string bizChemaCode =  this.Engine.WorkflowManager.GetClause(SchemaCode).BizSchemaCode;
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
     }
}
