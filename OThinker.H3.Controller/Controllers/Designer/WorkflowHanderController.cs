using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Designer
{
    public class WorkflowHanderController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }

        #region 流程模板设计管理器

        IWorkflowConfigManager _WorkflowDraftManager;
        IWorkflowConfigManager WorkflowDraftManager
        {
            get
            {
                if (this._WorkflowDraftManager == null)
                    this._WorkflowDraftManager = this.Engine.WorkflowConfigManager;
                return this._WorkflowDraftManager;
            }
        }

        #endregion

        #region 保存流程模板

        /// <summary>
        /// 保存流程模板
        /// </summary>
        public JsonResult SaveWorkflow(WorkflowTemplateViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {

                ActionResult result = new ActionResult();
                try
                {
                    // 读取活动模板定义信息
                    DraftWorkflowTemplate DraftWorkflowTemplate = ReadRequestWorkflowTemplate(model);
                    result.Success = this.Engine.WorkflowManager.SaveDraftTemplate(this.UserValidator.UserID, DraftWorkflowTemplate);
                    result.Message = string.Empty;
                    result.Success = true;
                }
                catch (Exception ex)
                {
                    result.Message = ex.Message;
                    result.Success = false;
                }

                return Json(result);
            });
        }

        #endregion

        #region 从Context中获取流程模板内容

        DraftWorkflowTemplate ReadRequestWorkflowTemplate(WorkflowTemplateViewModel model)
        {
            string WorkflowString = Server.HtmlDecode(model.WorkflowTemplate);
            //读取活动模板定义信息
            DraftWorkflowTemplate DraftWorkflowTemplate = (DraftWorkflowTemplate)JsonConvert.DeserializeObject(WorkflowString, typeof(DraftWorkflowTemplate));
            if (DraftWorkflowTemplate == null)
                return null;

            List<Activity> lstActivities = new List<Activity>();
            #region 读取活动

            //Start
            string StartActivityString = Server.HtmlDecode(model.StartActivities);
            StartActivity[] StartActivities = (StartActivity[])JsonConvert.DeserializeObject(StartActivityString, typeof(StartActivity[]));
            if (StartActivities != null)
                foreach (StartActivity activity in StartActivities)
                {
                    lstActivities.Add(activity);
                }

            //End
            string EndActivityString = Server.HtmlDecode(model.EndActivities);
            EndActivity[] EndActivities = (EndActivity[])JsonConvert.DeserializeObject(EndActivityString, typeof(EndActivity[]));
            if (EndActivities != null)
                foreach (EndActivity activity in EndActivities)
                {
                    lstActivities.Add(activity);
                }


            //FillSheet
            string FillSheetActivityString = Server.HtmlDecode(model.FillSheetActivities);
            FillSheetActivity[] FillSheetActivities = (FillSheetActivity[])JsonConvert.DeserializeObject(FillSheetActivityString, typeof(FillSheetActivity[]));
            if (FillSheetActivities != null)
                foreach (FillSheetActivity activity in FillSheetActivities)
                {
                    lstActivities.Add(activity);
                }

            //Approve
            string ApproveActivityString = Server.HtmlDecode(model.ApproveActivities);
            ApproveActivity[] ApproveActivities = (ApproveActivity[])JsonConvert.DeserializeObject(ApproveActivityString, typeof(ApproveActivity[]));
            if (ApproveActivities != null)
                foreach (ApproveActivity activity in ApproveActivities)
                {
                    lstActivities.Add(activity);
                }

            //Circulate
            string CirculateActivityString = Server.HtmlDecode(model.CirculateActivities);
            CirculateActivity[] CirculateActivities = (CirculateActivity[])JsonConvert.DeserializeObject(CirculateActivityString, typeof(CirculateActivity[]));
            if (CirculateActivities != null)
                foreach (CirculateActivity activity in CirculateActivities)
                {
                    //ERROR:
                    activity.ParticipateType = ActivityParticipateType.MultiParticipants;
                    lstActivities.Add(activity);
                }

            //Notify
            string NotifyActivityString = Server.HtmlDecode(model.NotifyActivities);
            NotifyActivity[] NotifyActivities = (NotifyActivity[])JsonConvert.DeserializeObject(NotifyActivityString, typeof(NotifyActivity[]));
            if (NotifyActivities != null)
                foreach (NotifyActivity activity in NotifyActivities)
                {
                    lstActivities.Add(activity);
                }

            //Wait
            string WaitActivityString = Server.HtmlDecode(model.WaitActivities);
            WaitActivity[] WaitActivities = (WaitActivity[])JsonConvert.DeserializeObject(WaitActivityString, typeof(WaitActivity[]));
            if (WaitActivities != null)
                foreach (WaitActivity activity in WaitActivities)
                {
                    lstActivities.Add(activity);
                }

            //Connection
            string ConnectionActivityString = Server.HtmlDecode(model.ConnectionActivities);
            ConnectionActivity[] ConnectionActivities = (ConnectionActivity[])JsonConvert.DeserializeObject(ConnectionActivityString, typeof(ConnectionActivity[]));
            if (ConnectionActivities != null)
                foreach (ConnectionActivity activity in ConnectionActivities)
                {
                    lstActivities.Add(activity);
                }

            //BizAction
            string BizActionActivityString = Server.HtmlDecode(model.BizActionActivities);
            BizActionActivity[] BizActionActivities = (BizActionActivity[])JsonConvert.DeserializeObject(BizActionActivityString, typeof(BizActionActivity[]));
            if (BizActionActivities != null)
                foreach (BizActionActivity activity in BizActionActivities)
                {
                    lstActivities.Add(activity);
                }

            //SubInstance
            string SubInstanceActivityString = Server.HtmlDecode(model.SubInstanceActivities);
            SubInstanceActivity[] SubInstanceActivities = (SubInstanceActivity[])JsonConvert.DeserializeObject(SubInstanceActivityString, typeof(SubInstanceActivity[]));
            if (SubInstanceActivities != null)
                foreach (SubInstanceActivity activity in SubInstanceActivities)
                {
                    lstActivities.Add(activity);
                }

            #endregion

            DraftWorkflowTemplate.Activities = lstActivities.ToArray();

            return DraftWorkflowTemplate;
        }

        #endregion

        #region 更新流程名称
        public JsonResult UpdateClauseName(string WorkflowCode, string ClauseName)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false, "");
                WorkflowClause Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                if (Clause != null)
                {
                    Clause.WorkflowName = ClauseName;
                    if (this.Engine.WorkflowManager.UpdateClause(Clause) == (long)ErrorCode.SUCCESS)
                    {
                        result.Success = true;
                    }
                }
                return Json(result);
            });

        }
        #endregion

        #region 验证流程模板
        /// <summary>
        /// 验证流程模板是否合法
        /// </summary>
        /// <param name="DraftWorkflowTemplate"></param>
        /// <returns></returns>
        public JsonResult ValidateWorkflow(WorkflowTemplateViewModel model)
        {
            return ExecuteFunctionRun(() =>
             {
                 DraftWorkflowTemplate DraftWorkflowTemplate = ReadRequestWorkflowTemplate(model);
                 if (DraftWorkflowTemplate != null)
                 {
                     OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(DraftWorkflowTemplate.BizObjectSchemaCode);
                     OThinker.H3.ValidationResult ValidationResult = DraftWorkflowTemplate.Validate(schema);

                     return Json(new { Result = ValidationResult.Valid, Errors = ValidationResult.Errors, Warnings = ValidationResult.Warnings });
                 }
                 else
                 {
                     var result = new { Result = false, Errors = new List<string>() { "Designer.WorkflowHandler_Msg0" } };
                     return Json(result);
                 }
             });
        }
        #endregion

        #region 发布流程模板

        /// <summary>
        /// 发布流程模板
        /// </summary>
        public JsonResult PublishWorkflow(WorkflowTemplateViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                DraftWorkflowTemplate DraftWorkflowTemplate = ReadRequestWorkflowTemplate(model);
                if (DraftWorkflowTemplate == null)
                {
                    return Json(new { Result = false, Errors = new List<string>() { "Designer.WorkflowHandler_Msg1" } });

                }

                string WorkflowCode = DraftWorkflowTemplate.WorkflowCode;
                WorkflowClause Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                if (Clause == null)
                {
                    return Json(new { Result = false, Errors = new List<string>() { "Designer.WorkflowHandler_Msg2" } });
                }
                DraftWorkflowTemplate.BizObjectSchemaCode = Clause.BizSchemaCode;

                // 先保存流程模板
                if (!this.Engine.WorkflowManager.SaveDraftTemplate(this.UserValidator.UserID, DraftWorkflowTemplate))
                {
                    return Json(new { Result = false, Errors = new List<string>() { "Designer.WorkflowHandler_Msg3" } });
                }

                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(Clause.BizSchemaCode);
                OThinker.H3.ValidationResult ValidationResult = DraftWorkflowTemplate.Validate(schema);

                if (!ValidationResult.Valid)
                {

                    return Json(ValidationResult);
                }

                PublishResult PublishResult = this.Engine.WorkflowManager.RegisterWorkflow(this.UserValidator.UserID, DraftWorkflowTemplate.WorkflowCode, true);

                if (PublishResult.Result == (long)OThinker.H3.ErrorCode.SUCCESS)
                {
                    return Json(new { Result = true, Message = new List<string>() { "流程发布成功:当前版本号: " + PublishResult.RegisteredVersion.ToString() } });
                }
                else
                {
                    return Json(new { Result = false, Errors = PublishResult.Errors });
                }
            });

        }

        #endregion

        #region 获取流程模板

        /// <summary>
        /// 获取流程模板
        /// </summary>
        public JsonResult GetWorkflow(string WorkflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                DraftWorkflowTemplate DraftWorkflowTemplate = this.Engine.WorkflowManager.GetDraftTemplate(WorkflowCode + string.Empty);

                return Json(DraftWorkflowTemplate, JsonRequestBehavior.AllowGet);
            });

        }

        #endregion

        #region 导出流程模板

        /// <summary>
        /// 导出流程模板
        /// </summary>
        public JsonResult ExportWorkflow(string WorkflowCode)
        {
            return ExecuteFunctionRun(() =>
            {

                return Json(this.Engine.WorkflowManager.GetDraftTemplate(WorkflowCode));
            });

        }

        #endregion

        #region 保存活动模板

        /// <summary>
        /// 保存活动模板
        /// </summary>
        public JsonResult SaveActivityTemplate(string ActivityType, string Activity)
        {
            return ExecuteFunctionRun(() =>
            {
                int ActivityTypeValue = 0;
                int.TryParse(ActivityType, out ActivityTypeValue);
                ActivityType ActivityTypeObject = (ActivityType)ActivityTypeValue;
                Activity ActivityObject = null;

                string ActivityString = Server.HtmlDecode(Activity);
                switch (ActivityTypeObject)
                {
                    case OThinker.H3.WorkflowTemplate.ActivityType.Start:
                        ActivityObject = (StartActivity)JsonConvert.DeserializeObject(ActivityString, typeof(StartActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.End:
                        ActivityObject = (EndActivity)JsonConvert.DeserializeObject(ActivityString, typeof(EndActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.FillSheet:
                        ActivityObject = (FillSheetActivity)JsonConvert.DeserializeObject(ActivityString, typeof(FillSheetActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.Approve:
                        ActivityObject = (ApproveActivity)JsonConvert.DeserializeObject(ActivityString, typeof(ApproveActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.Circulate:
                        ActivityObject = (CirculateActivity)JsonConvert.DeserializeObject(ActivityString, typeof(CirculateActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.Connection:
                        ActivityObject = (ConnectionActivity)JsonConvert.DeserializeObject(ActivityString, typeof(ConnectionActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.Notify:
                        ActivityObject = (NotifyActivity)JsonConvert.DeserializeObject(ActivityString, typeof(NotifyActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.Wait:
                        ActivityObject = (WaitActivity)JsonConvert.DeserializeObject(ActivityString, typeof(WaitActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.BizAction:
                        ActivityObject = (BizActionActivity)JsonConvert.DeserializeObject(ActivityString, typeof(BizActionActivity));
                        break;
                    case OThinker.H3.WorkflowTemplate.ActivityType.SubInstance:
                        ActivityObject = (SubInstanceActivity)JsonConvert.DeserializeObject(ActivityString, typeof(SubInstanceActivity));
                        break;
                }

                if (Activity != null)
                {
                    return Json(new { Success = WorkflowDraftManager.SaveActivityTemplate(ActivityObject) });
                }
                else
                {
                    return Json(new { Success = false, Errors = new List<string>() { "Designer.WorkflowHandler_Msg5" } });
                }
            });
        }

        #endregion

        /// <summary>
        /// 获取数据项
        /// </summary>
        public JsonResult GetDataItems(string SchameCode)
        {
            return ExecuteFunctionRun(() =>
            {
                return WriteSchema(SchameCode);
            });

        }

        /// <summary>
        /// 根据流程编码获取可映射的数据项
        /// </summary>
        public JsonResult GetDataItemsByWorkflowCode(string WorkflowCode)
        {
            return ExecuteFunctionRun(() =>
            {
                string SchameCode = string.Empty;
                if (!string.IsNullOrEmpty(WorkflowCode))
                {
                    WorkflowClause Clause = this.Engine.WorkflowManager.GetClause(WorkflowCode);
                    if (Clause != null)
                    {
                        SchameCode = Clause.BizSchemaCode;
                    }
                }
                return WriteSchema(SchameCode);
            });

        }

        public JsonResult GetParticipants(string ActivityCode, string InstanceID)
        {
            return ExecuteFunctionRun(() =>
            {
                if (!string.IsNullOrEmpty(ActivityCode) && !string.IsNullOrEmpty(InstanceID))
                {
                    //流程
                    Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                    if (InstanceContext != null)
                    {
                        //流程模板
                        WorkflowTemplate.PublishedWorkflowTemplate Template = this.Engine.WorkflowManager.GetPublishedTemplate(
                            InstanceContext.WorkflowCode,
                            InstanceContext.WorkflowVersion);
                        if (Template != null)
                        {
                            //活动信息
                            ClientActivityBase Activity = Template.GetActivityByCode(ActivityCode) as ClientActivityBase;

                            OThinker.H3.Instance.InstanceData InstanceData = new Instance.InstanceData(this.Engine, InstanceID, null);

                            string[] ParticipantIDs = Activity.ParseParticipants(InstanceData, this.Engine.Organization);
                            OThinker.Organization.Unit[] ParticipantUsers = this.Engine.Organization.GetUnits(ParticipantIDs).ToArray();

                            if (ParticipantUsers != null)
                            {
                                List<object> ParticipantUserNames = new List<object>();
                                foreach (OThinker.Organization.Unit u in ParticipantUsers)
                                {
                                    if (u != null && u.UnitType == OThinker.Organization.UnitType.User)
                                    {
                                        OThinker.Organization.User user = (OThinker.Organization.User)u;
                                        ParticipantUserNames.Add(new
                                        {
                                            Name = user.Name,
                                            Code = user.Code,
                                            ObjectID = user.ObjectID
                                        });
                                        //ParticipantUserNames.Add(u.Name + "[" + u.Code + "]");
                                    }
                                }
                                return Json(ParticipantUserNames, JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }

                return Json(new string[] { }, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 输出数据模型
        /// </summary>
        /// <param name="SchemaCode"></param>
        public JsonResult WriteSchema(string SchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                DataModel.BizObjectSchema BizObject = this.Engine.BizObjectManager.GetDraftSchema(SchemaCode);
                if (BizObject != null && BizObject.Properties != null)
                {
                    List<object> lstDataItems = new List<object>();
                    foreach (DataModel.PropertySchema Property in BizObject.Properties)
                    {
                        //不显示保留数据项
                        if (!DataModel.BizObjectSchema.IsReservedProperty(Property.Name))
                        {
                            lstDataItems.Add(new
                            {
                                Text = Property.DisplayName,
                                Value = Property.Name
                            });
                        }
                    }
                    return Json(new
                    {
                        SchemaCode = SchemaCode,
                        DataItems = lstDataItems.ToArray()
                    });
                }
                return Json(new { SchemaCode = "", DataItems =new {}});
            });
        }
    }
}
