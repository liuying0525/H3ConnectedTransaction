<%@ WebService Language="C#" Class="OThinker.H3.Portal.ToolsWebService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Collections.Generic;
using System.Data;
using OThinker.H3.Controllers;
using OThinker.H3.DataModel;

namespace OThinker.H3.Portal
{
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class ToolsWebService : System.Web.Services.WebService
    {

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        //[WebMethod]
        public bool FixHolidayData()
        {
            string sql = @"
 select * from c_Emp_Holidays_Detail where instanceid in
 (
 select con.objectid from I_restflow a
left join Ot_Instancecontext con on a.objectid=con.bizobjectid
 where category<>'调休' and con.state=4
 )
";
            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);


            foreach (DataRow row in dt.Rows)
            {
                string sqlUpdate = "  update c_emp_holidays set usedhours=usedhours-{0} where id='{1}'";
                string sqlDelete = "delete from c_Emp_Holidays_Detail where id='{0}'";

                sqlUpdate = string.Format(sqlUpdate, row["Hours"], row["Pid"]);
                sqlDelete = string.Format(sqlDelete, row["id"]);

                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlUpdate);
                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sqlDelete);
            }
            return true;
        }

        //[WebMethod]
        private bool ImportFinishedItemsToDistinctTable(string startdate,string enddate)
        {
            int n = 0;
            try
            {
                string sqlGetAllWorkItem = "SELECT * FROM OT_WorkItemFinished where ReceiveTime >to_date('{0}','yyyy-mm-dd') and ReceiveTime<to_date('{1}','yyyy-mm-dd') ORDER BY ReceiveTime";
                sqlGetAllWorkItem = string.Format(sqlGetAllWorkItem, startdate, enddate);
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlGetAllWorkItem);
                foreach (DataRow row in dt.Rows)
                {
                    n++;
                    //1.去重
                    string str_SQL = "SELECT objectid FROM OT_WorkItemFinishedDistinct WHERE InstanceId='{0}' AND ActivityCode='{1}' AND (Participant='{2}' OR Finisher='{2}')";
                    str_SQL = string.Format(str_SQL, row["InstanceId"], row["ActivityCode"], row["Finisher"]);
                    string obj = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(str_SQL) + string.Empty;
                    if (!string.IsNullOrEmpty(obj))//有重复的，需要进行删除；
                    {
                        int n_Delete = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM OT_WorkItemFinishedDistinct WHERE objectid='" + obj + "'");
                        AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct 重复数据删除 ObjectID-->" + obj + "，受影响行数：" + n_Delete);
                    }
                    //2.插入到Distinct表
                    int n_Insert = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("INSERT INTO OT_WorkItemFinishedDistinct SELECT * FROM OT_WorkItemFinished WHERE ObjectID='" + row["ObjectID"] + "'");
                    AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct Insert ObjectID-->" + row["ObjectID"]);
                }
                AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct Import Finished 行数：" + n);
                return true;
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("OT_WorkItemFinishedDistinct Import Exception，当前处理行:" + n + "，Message" + ex.ToString());
                return false;
            }
        }

        [WebMethod(Description = "设置流程优先级：高中低")]
        public bool SetInstancePriority(string InsID, string Level)
        {
            Instance.PriorityType pt = new Instance.PriorityType();
            switch (Level)
            {
                case "高": pt = Instance.PriorityType.High; break;
                case "中": pt = Instance.PriorityType.Normal; break;
                case "低": pt = Instance.PriorityType.Low; break;
                default: return false;
            }

            return AppUtility.Engine.InstanceManager.SetInstancePriority(InsID, pt);
        }
        [WebMethod(Description = "设置自定义设置的参数值")]
        public bool SetCustomSetting(string settingName, string settingValue)
        {
            return AppUtility.Engine.SettingManager.SetCustomSetting(settingName, settingValue);
        }

        [WebMethod(Description = "设置数据项的值")]
        public bool SetPropertyValue(string schemaCode, string bizObjectID, string itemName, string itemValue)
        {
            return AppUtility.Engine.BizObjectManager.SetPropertyValue(schemaCode, bizObjectID, "", itemName, itemValue);
        }
        
        [WebMethod(Description = "驳回到FI")]
        public string SendBackFI(string applicationNo, string comment)
        {
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
            value.applicationNo = applicationNo;
            value.Comment = comment;
            AppUtility.Engine.LogWriter.Write(applicationNo + "打回CAP草稿状态意见：" + value.Comment);
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.SendBackFI(value);
            AppUtility.Engine.LogWriter.Write(applicationNo + "：打回CAP草稿状态结束：'" + transResult.message + "'");
            return transResult.message;
        }

        [WebMethod(Description = "设置流程名称")]
        public bool SetInstanceName(string InstanceID, string InstanceName)
        {
            return AppUtility.Engine.InstanceManager.SetInstanceName(InstanceID, InstanceName);
        }

        [WebMethod(Description = "设置子表数据项的值")]
        public bool SetDetailValue(string workflowCode, string detail_Obj, string itemName, string itemValue)
        {
            BizObjectSchema schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema(workflowCode);
            BizObject bo = new BizObject(AppUtility.Engine.Organization, AppUtility.Engine.BizObjectManager, schema, "");
            bo.ObjectID = detail_Obj;
            bo.Load();
            bo[itemName] = itemValue;
            return bo.Update();
        }

        [WebMethod]
        public void SubmitItem(string workItemId, string commentText, string userId)
        {
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = AppUtility.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext instance = AppUtility.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // 添加意见
            this.AppendComment(instance, item, OThinker.Data.BoolMatchValue.Unspecified, commentText);

            // 结束工作项
            AppUtility.Engine.WorkItemManager.FinishWorkItem(
                item.ObjectID,
                userId,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                null,
                OThinker.Data.BoolMatchValue.True,
                commentText,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                (int)OThinker.H3.Controllers.SheetButtonType.Submit);
            // 需要通知实例事件管理器结束事件
            Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    OThinker.Data.BoolMatchValue.True,
                    false,
                    OThinker.Data.BoolMatchValue.True,
                    true,
                    null);
            AppUtility.Engine.InstanceManager.SendMessage(endMessage);
        }
        private void AppendComment(OThinker.H3.Instance.InstanceContext Instance, OThinker.H3.WorkItem.WorkItem item, OThinker.Data.BoolMatchValue approval, string commentText)
        {
            // 添加一个审批意见
            WorkflowTemplate.PublishedWorkflowTemplate workflow = AppUtility.Engine.WorkflowManager.GetPublishedTemplate(
                item.WorkflowCode,
                item.WorkflowVersion);
            // 审批字段
            string approvalDataItem = null;
            if (workflow != null)
            {
                OThinker.H3.DataModel.BizObjectSchema schema = AppUtility.Engine.BizObjectManager.GetPublishedSchema(item.WorkflowCode);
                approvalDataItem = workflow.GetDefaultCommentDataItem(schema, item.ActivityCode);
            }
            if (approvalDataItem != null)
            {
                // 创建审批
                OThinker.H3.Data.Comment comment = new Data.Comment();
                comment.Activity = item.ActivityCode;
                comment.Approval = approval;
                comment.CreatedTime = System.DateTime.Now;
                comment.DataField = approvalDataItem;
                comment.InstanceId = item.InstanceId;
                comment.BizObjectId = Instance.BizObjectId;
                comment.BizObjectSchemaCode = Instance.BizObjectSchemaCode;
                comment.OUName = AppUtility.Engine.Organization.GetName(AppUtility.Engine.Organization.GetParent(item.Participant));
                comment.Text = commentText;
                comment.TokenId = item.TokenId;
                comment.UserID = item.Participant;

                // 设置用户的默认签章
                Organization.Signature[] signs = AppUtility.Engine.Organization.GetSignaturesByUnit(item.Participant);
                if (signs != null && signs.Length > 0)
                {
                    foreach (Organization.Signature sign in signs)
                    {
                        if (sign.IsDefault)
                        {
                            comment.SignatureId = sign.ObjectID;
                            break;
                        }
                    }
                }
                AppUtility.Engine.BizObjectManager.AddComment(comment);
            }
        }
    }
}

