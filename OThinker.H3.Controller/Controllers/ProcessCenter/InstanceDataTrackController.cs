using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessCenter
{
    public class InstanceDataTrackController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        #region 参数

        private string InstanceId;
        private OThinker.H3.Instance.InstanceContext InstanceContext = null;
        private string WorkItemID;
        private OThinker.H3.WorkItem.WorkItem WorkItem = null;
        private OThinker.H3.WorkItem.CirculateItem CirculateItem = null;
        private string ItemName;
        private WorkflowTemplate.PublishedWorkflowTemplate Workflow;
        private DataModel.PropertySchema[] WorkflowDataItems;
        private DataModel.PropertySchema WorkflowDataItem;

        private void ParseParams(string instanceId, string itemName, string workItemID)
        {
            // 基本参数
            this.InstanceId = instanceId;
            this.ItemName = System.Web.HttpUtility.UrlDecode(itemName);
            this.WorkItemID = workItemID;

            // 验证参数
            if (string.IsNullOrEmpty(this.InstanceId) && string.IsNullOrEmpty(this.WorkItemID))
            {
                throw new ArgumentNullException("Param InstanceId and WorkItemID can NOT be null at the same time.");
            }


            // 获得流程或者任务对象
            if (!string.IsNullOrEmpty(this.InstanceId))
            {
                this.InstanceContext = this.Engine.InstanceManager.GetInstanceContext(this.InstanceId);
                if (this.InstanceContext == null)
                {
                    throw new ArgumentNullException("InstanceID invalid.");
                }
            }
            if (!string.IsNullOrEmpty(this.WorkItemID))
            {
                this.WorkItem = this.Engine.WorkItemManager.GetWorkItem(this.WorkItemID);
                this.CirculateItem = this.Engine.WorkItemManager.GetCirculateItem(this.WorkItemID);

                if (this.WorkItem == null && this.CirculateItem == null)
                {
                    throw new ArgumentNullException("WorkItemID invalid.");
                }
            }

            // 获得流程模板
            this.Workflow = this.Engine.WorkflowManager.GetPublishedTemplate(this.InstanceContext.WorkflowCode, this.InstanceContext.WorkflowVersion);
            //            this.Workflow = this.Engine.WorkflowManager.GetWorkflow(this.InstanceContext.WorkflowPackage, this.InstanceContext.WorkflowName, this.InstanceContext.WorkflowVersion);
            if (this.Workflow == null)
            {
                throw new ArgumentNullException("Workflow Template invalid.");
            }

            // 获得数据定义
            this.WorkflowDataItems = this.Engine.BizObjectManager.GetPublishedSchema(this.Workflow.BizObjectSchemaCode).Properties;
            //this.Workflow.GetDataItem(this.ItemName);

            foreach (DataModel.PropertySchema p in this.WorkflowDataItems)
            {
                if (this.ItemName == p.Name)
                {
                    this.WorkflowDataItem = p;
                    break;
                }
            }

            if (this.WorkflowDataItem == null)
            {
                throw new ArgumentNullException("Workflow Data Item invalid.");
            }
        }

        #endregion

        private System.Data.DataTable TrackTable;
        private Dictionary<string, string> NameTable = new Dictionary<string, string>();
        Dictionary<string, string> participantDic = new Dictionary<string, string>();

        public JsonResult GetTrackTable(string instanceId, string itemName, string workItemID)
        {
            return ExecuteFunctionRun(() =>
            {

                ActionResult result = new ActionResult();
                // 解析参数
                this.ParseParams(instanceId, itemName, workItemID);
                object resultItem;
                string ActivityCode;
                // 需验证数据库是否有拆表
                if (this.WorkItem == null)
                {
                    resultItem = this.CirculateItem;
                    ActivityCode = this.CirculateItem.ActivityCode;
                }
                else
                {
                    resultItem = this.WorkItem;
                    ActivityCode = this.WorkItem.ActivityCode;
                }

                // 验证权限
                if (resultItem != null && ((WorkflowTemplate.ParticipativeActivity)this.Workflow.GetActivityByCode(ActivityCode)).GetItemTrackVisible(this.ItemName))
                {
                    // 有权限查看数据痕迹
                }
                else if (!this.UserValidator.ValidateWFInsAdmin(this.InstanceContext.WorkflowCode, this.InstanceContext.Originator))
                {
                    // 没有权限
                    //this.NotifyMessage(PortalPage.LackOfAuth);
                    result.Success = false;
                    result.Message = "没有权限";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                var ItemDisplayName = string.IsNullOrEmpty(this.WorkflowDataItem.DisplayName) ? this.WorkflowDataItem.Name : this.WorkflowDataItem.DisplayName;
                if (!OThinker.H3.Data.DataLogicTypeConvertor.IsAttachmentType(this.WorkflowDataItem.LogicType))
                {
                    this.TrackTable = this.Engine.BizObjectTrackManager.QueryTable(this.InstanceContext.BizObjectId, this.ItemName);

                    ProcessParticipant();

                    if (this.TrackTable != null)
                    {
                        List<string> list = new List<string>();
                        foreach (DataRow row in this.TrackTable.Rows)
                        {
                            string modifyBy = row[OThinker.H3.DataModel.BizObjectPropertyTrack.PropertyName_ModifiedBy] + string.Empty;
                            if (!list.Contains(modifyBy)) list.Add(modifyBy);
                        }
                        List<OThinker.Organization.Unit> units = this.Engine.Organization.GetUnits(list.ToArray()).ToList();
                        if (units != null)
                        {
                            foreach (OThinker.Organization.Unit unit in units)
                            {
                                if (!this.NameTable.ContainsKey(unit.ObjectID))
                                    this.NameTable.Add(unit.ObjectID, unit.Name);
                            }
                        }

                        List<object> lstObjs = GetListObjectFromTrackTable(TrackTable);
                        var gridData = CreateLigerUIGridData(lstObjs.ToArray());
                        return Json(gridData, JsonRequestBehavior.AllowGet);
                    }

                    return null;
                }
                else
                {

                    Data.AttachmentHeader[] headers = this.Engine.BizObjectManager.QueryAttachment(this.InstanceContext.BizObjectSchemaCode, this.InstanceContext.BizObjectId, this.ItemName, OThinker.Data.BoolMatchValue.Unspecified, string.Empty);

                    List<Object> lstObjs = new List<object>();

                    if (headers != null && headers.Length > 0)
                    {
                        List<string> list = new List<string>();
                        foreach (Data.AttachmentHeader header in headers)
                        {
                            //ObjectID BizObjectId PropertyName ModifiedBy ModifiedValue ModifiedTime
                            var obj = new
                            {
                                ObjectID = header.ObjectID,
                                //BizObjectId =header.BizObjectId,
                                //BizObjectSchemaCode =header.BizObjectSchemaCode,
                                ModifiedTime = header.ModifiedTime,
                                ModifiedBy = header.CreatedBy,
                                PropertyName = header.DataField,
                                Value = header.FileName
                            };
                            lstObjs.Add(obj);
                            if (!list.Contains(header.CreatedBy)) list.Add(header.CreatedBy);
                        }
                        List<OThinker.Organization.Unit> units = this.Engine.Organization.GetUnits(list.ToArray());
                        foreach (OThinker.Organization.Unit unit in units)
                        {
                            if (!this.NameTable.ContainsKey(unit.ObjectID))
                                this.NameTable.Add(unit.ObjectID, unit.Name);
                        }
                    }

                    var gridData = CreateLigerUIGridData(lstObjs.ToArray());
                    return Json(gridData);
                }
            });
        }

        /// <summary>
        /// 将参与者类型的痕迹数据批量查询出来，减少与Engine的连接次数
        /// </summary>
        private void ProcessParticipant()
        {
            List<string> participantIds = new List<string>();
            if (this.WorkflowDataItem.LogicType == Data.DataLogicType.SingleParticipant)
            {
                foreach (DataRow row in this.TrackTable.Rows)
                {
                    string participantId = OThinker.Data.Convertor.XmlToObject(typeof(string),
                        row[DataModel.BizObjectPropertyTrack.PropertyName_ModifiedValue] + string.Empty) + string.Empty;
                    if (!string.IsNullOrEmpty(participantId) && !participantIds.Contains(participantId))
                    {
                        participantIds.Add(participantId);
                    }
                }
            }
            else if (this.WorkflowDataItem.LogicType == Data.DataLogicType.MultiParticipant)
            {
                foreach (DataRow row in this.TrackTable.Rows)
                {
                    string[] ids = OThinker.Data.Convertor.XmlToObject(typeof(string[]),
                        row[DataModel.BizObjectPropertyTrack.PropertyName_ModifiedValue] + string.Empty) as string[];
                    if (ids != null)
                    {
                        foreach (string id in ids)
                        {
                            if (!participantIds.Contains(id))
                            {
                                participantIds.Add(id);
                            }
                        }
                    }
                }
            }
            List<OThinker.Organization.Unit> participants = this.Engine.Organization.GetUnits(participantIds.ToArray());
            if (participants != null)
            {
                foreach (OThinker.Organization.Unit participant in participants)
                {
                    if (!participantDic.ContainsKey(participant.ObjectID))
                    {
                        participantDic.Add(participant.ObjectID, participant.Name);
                    }
                }
            }
        }

        private string ParseItemValue(string itemValue, string attachmentId)
        {
            var updatedItemValue = "";
            if (OThinker.H3.Data.DataLogicTypeConvertor.IsAttachmentType(this.WorkflowDataItem.LogicType))
            {
                // 修改值
                updatedItemValue =
                    "<a href=\"" +
                        this.GetReadAttachmentUrl(this.Workflow.BizObjectSchemaCode, attachmentId) +
                        "\">" + itemValue + "</a>";
            }
            else
            {
                object dbOriginalValue = itemValue;
                if (this.WorkflowDataItem.LogicType == Data.DataLogicType.SingleParticipant)
                {
                    string originalValue = OThinker.Data.Convertor.XmlToObject(typeof(string), dbOriginalValue + string.Empty) + string.Empty;
                    string name = string.Empty;
                    if (participantDic.ContainsKey(originalValue))
                    {
                        name = participantDic[originalValue];
                    }
                    updatedItemValue = name;
                }
                else if (this.WorkflowDataItem.LogicType == Data.DataLogicType.MultiParticipant)
                {
                    string[] originalValue = OThinker.Data.Convertor.XmlToObject(typeof(string[]), dbOriginalValue + string.Empty) as string[];
                    string names = string.Empty;
                    if (originalValue != null)
                    {
                        foreach (string id in originalValue)
                        {
                            if (participantDic.ContainsKey(id))
                            {
                                names += participantDic[id] + ",";
                            }
                        }
                    }
                    updatedItemValue = string.IsNullOrEmpty(names) ? "" : names.Substring(0, names.Length - 1);
                }
                else
                {
                    object originalValue = OThinker.Data.Convertor.XmlToObject(typeof(string), dbOriginalValue + string.Empty);
                    updatedItemValue = originalValue + string.Empty;
                }
            }
            return "";
        }

        private string PareseModify(string modifyBy)
        {
            // 修改人
            if (this.NameTable != null && this.NameTable.ContainsKey(modifyBy))
            {
                return this.NameTable[modifyBy];
            }

            return modifyBy;
        }

        private List<object> GetListObjectFromTrackTable(DataTable trackTable)
        {
            List<Object> lstObjs = new List<object>();

            if (trackTable != null && trackTable.Rows.Count > 0)
            {
                List<string> list = new List<string>();
                foreach (DataRow dr in trackTable.Rows)
                {
                    //ObjectID BizObjectId PropertyName ModifiedBy ModifiedValue ModifiedTime
                    var obj = new
                    {
                        ObjectID = dr["ObjectID"] + string.Empty,
                        ModifiedTime = dr["ModifiedTime"] + string.Empty,
                        //BizObjectId = dr["BizObjectId"] + string.Empty,
                        //BizObjectSchemaCode = dr["BizObjectSchemaCode"] + string.Empty,
                        PropertyName = dr["PropertyName"] + string.Empty,
                        ModifiedBy = PareseModify(dr["ModifiedBy"] + string.Empty),
                        Value = dr["ModifiedValue"] + string.Empty,
                    };

                    lstObjs.Add(obj);
                }
            }

            return lstObjs;
        }
    }
}
