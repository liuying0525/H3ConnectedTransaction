using Newtonsoft.Json;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.WorkFlow
{
    public class EditSimulationDataController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        /// <summary>
        /// 加载数据，返回类型及显示名称
        /// </summary>
        /// <param name="WorkflowCode"></param>
        /// <param name="ActivityCode"></param>
        /// <param name="ItemName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public JsonResult Load(string WorkflowCode, string ActivityCode,string ItemName,string Value)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                BizObjectSchema Schema=null;
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

                //string[] values = null;
                //try
                //{
                //    values = (string[])JsonConvert.DeserializeObject(Value, typeof(string[]));
                //}
                //catch { }
                var DisplayName="";
                var LogicTypeDisplay="";
                var LogicType = "";

                if (PropertySchema != null)
                {
                     DisplayName = PropertySchema.DisplayName;
                     LogicTypeDisplay = OThinker.H3.Data.DataLogicTypeConvertor.ToLogicTypeName(PropertySchema.LogicType);
                     LogicType = PropertySchema.LogicType.ToString();
                    //前端处理
                    ////选择参与者
                    //if (PropertySchema.LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant || this.PropertySchema.LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                    //{
                    //    usParticipants.Visible = true;
                    //    usParticipants.SelectedUsers = values;
                    //}
                    //逻辑型
                    //else if (this.PropertySchema.LogicType == OThinker.H3.Data.DataLogicType.Bool)
                    //{
                    //    chk.Visible = true;
                    //    if (values != null && values.Length > 0 && (values[0] + string.Empty).ToLower() == "true")
                    //    {
                    //        chk.Checked = true;
                    //    }
                    //}
                    //逻辑型
                    //else if (this.PropertySchema.LogicType == OThinker.H3.Data.DataLogicType.DateTime)
                    //{
                    //    st.Visible = true;
                    //    DateTime time = DateTime.Now;
                    //    if (values != null && values.Length > 0 && DateTime.TryParse(values[0], out time))
                    //    {
                    //        st.TimeValue = time;
                    //    }
                    //}
                    //else
                    //{
                    //    txtValues.Visible = true;
                    //    if (values != null && values.Length > 0)
                    //    {
                    //        txtValues.Text = string.Join(";", values);
                    //    }
                    //}
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
                    result.Message="Simulation.EditSimulationData_Mssg1";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                result.Success = true;
                result.Extend = new { DisplayName = DisplayName,LogicTypeDisplay=LogicTypeDisplay,LogicType = LogicType };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

       
    }
}
