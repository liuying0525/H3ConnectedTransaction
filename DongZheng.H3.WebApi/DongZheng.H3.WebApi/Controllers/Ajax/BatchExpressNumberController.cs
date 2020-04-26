using DongZheng.H3.WebApi.Common;
using DongZheng.H3.WebApi.Common.Portal;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class BatchExpressNumberController : CustomController
    {
        public override string FunctionCode => "BatchExpress";

        public void Index()
        {
            var context = HttpContext;
            string action = Request.Params["Action"] ?? "";
            string express = Request.Params["ExpressNumber"] ?? "";
            string mortgageId = Request.Params["MortgageId"] ?? "";
            string model = "<option value='[Value]'>[Name]</option>";
            string result = "";
            if (action == "Load")
            {
                string sql = "SELECT A.OBJECTID,A.NAME FROM I_MORTGAGE A LEFT JOIN OT_INSTANCECONTEXT B ON A.OBJECTID = B.BIZOBJECTID LEFT JOIN OT_WORKITEM C ON B.OBJECTID = C.INSTANCEID " +
                             "WHERE C.ACTIVITYCODE = 'Activity13'";
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result += model
                            .Replace("[Name]", dr["NAME"] == null || dr["NAME"] == DBNull.Value ? "" : dr["NAME"].ToString())
                            .Replace("[Value]", dr["OBJECTID"] == null || dr["OBJECTID"] == DBNull.Value ? "" : dr["OBJECTID"].ToString());
                    }
                }
            }
            else
            {
                if (!string.IsNullOrEmpty(mortgageId) || mortgageId != "|")
                {
                    var listId = mortgageId.Split('|');
                    try
                    {
                        foreach (var t in listId)
                        {
                            if (!string.IsNullOrEmpty(t))
                            {
                                UserValidator userInSession = Session[Sessions.GetUserValidator()] as UserValidator;
                                string instanceId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID = '" + t + "'").ToString();
                                string workItemId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_WORKITEM WHERE INSTANCEID = '" + instanceId + "'").ToString();
                                List<DataItemParam> dataList = new List<DataItemParam> { new DataItemParam { ItemName = "expressNumber", ItemValue = express } };
                                BPMServiceResult re = SubmitItem("Mortgage", instanceId, workItemId, (OThinker.Data.BoolMatchValue)1, "", userInSession.UserCode, dataList);
                                result = re.Success ? "快递单号提交成功" : "快递单号提交失败";
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result = ex.Message;
                    }
                }
                else
                {
                    result = "请选择抵押流程";
                }
            }
            context.Response.Write(result);
        }

        

        /// <summary>
        /// 提交工作项
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="instanceId"></param>
        /// <param name="workItemId"></param>
        /// <param name="approval"></param>
        /// <param name="commentText"></param>
        /// <param name="userId"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        private BPMServiceResult SubmitItem(string workflowCode, string instanceId, string workItemId, OThinker.Data.BoolMatchValue approval, string commentText, string userId, List<DataItemParam> values)
        {
            BPMServiceResult result = new BPMServiceResult();
            try
            {
                string user = GetUserIDByCode(userId);
                if (user == null)
                {
                    return new BPMServiceResult(false, "流程启动失败,用户{" + userId + "}不存在。");
                }
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflowTemplate = Engine.WorkflowManager.GetDefaultWorkflow(workflowCode);
                InstanceContext ic = Engine.InstanceManager.GetInstanceContext(instanceId);
                if (ic == null)
                {
                    return new BPMServiceResult(false, "InstanceID错误，此ID在H3系统中不存在，请检查");
                }
                OThinker.H3.DataModel.BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema, user);
                bo.ObjectID = ic.BizObjectId;
                bo.Load();
                foreach (DataItemParam value in values)
                {
                    OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(value.ItemName);
                    if (property.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                    {
                        var t = new List<OThinker.H3.DataModel.BizObject>();
                        foreach (List<DataItemParam> list in (IEnumerable)value.ItemValue)
                        {
                            var m = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, property.ChildSchema, bo.OwnerId);
                            foreach (DataItemParam dataItem in list)
                            {
                                if (m.Schema.ContainsField(dataItem.ItemName)) m.SetValue(dataItem.ItemName, dataItem.ItemValue);
                            }
                            t.Add(m);
                        }
                        bo[value.ItemName] = t.ToArray();
                    }
                    else if (bo.Schema.ContainsField(value.ItemName))
                    {
                        bo[value.ItemName] = value.ItemValue;
                    }
                }
                bo.Update();
                // 获取工作项
                OThinker.H3.WorkItem.WorkItem item = Engine.WorkItemManager.GetWorkItem(workItemId);
                OThinker.H3.Instance.InstanceContext instance = Engine.InstanceManager.GetInstanceContext(item.InstanceId);
                // 结束工作项
                Engine.WorkItemManager.FinishWorkItem(item.ObjectID, userId, OThinker.H3.WorkItem.AccessPoint.ExternalSystem, null, approval,
                    commentText, null, OThinker.H3.WorkItem.ActionEventType.Forward, (int)OThinker.H3.Controllers.SheetButtonType.Submit);
                // 需要通知实例事件管理器结束事件
                OThinker.H3.Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId, item.ActivityCode, item.TokenId, approval, false, approval, true, null);
                Engine.InstanceManager.SendMessage(endMessage);
                result = new BPMServiceResult(true, "", null, "流程实例启动成功！", "");
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.Message);
            }
            return result;
        }
        public string GetUserIDByCode(string UserCode)
        {
            var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, UserCode);
            return CurrentUserValidator.UserID;
        }
        private IEngine _Engine = null;
        public IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set { _Engine = value; }
        }
        [Serializable]
        public class DataItemParam
        {
            private string itemName = string.Empty;
            public string ItemName
            {
                get { return itemName; }
                set { this.itemName = value; }
            }
            private object itemValue = string.Empty;
            public object ItemValue
            {
                get { return itemValue; }
                set { this.itemValue = value; }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}