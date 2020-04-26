using OThinker.H3.Controllers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;

namespace H3_Scheduled
{
    public class UpdateMortgage
    {
        private static UpdateMortgage _mort = new UpdateMortgage();
        private Thread _thread;
        public static UpdateMortgage GetInstance()
        {
            return _mort;
        }

        public bool StartThread()
        {
            try
            {
                _thread = new Thread(new ThreadStart(UpdateMortgageData));
                _thread.Start();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void UpdateMortgageData()
        {
            while (true)
            {
                DateTime start = DateTime.Now;
                //List<DataItemParam> dataList1 = new List<DataItemParam>();
                //string sqljy1 = "SELECT * FROM I_MORTGAGE WHERE OBJECTID = '9ff09aa7-3c54-4431-b76e-ae2abb838855'";
                //DataTable dtjy1 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqljy1);
                //string sqlzd1 = "SELECT * FROM I_GJPEOPLE WHERE PARENTOBJECTID = '9ff09aa7-3c54-4431-b76e-ae2abb838855'";
                //DataTable dtzd1 = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlzd1);
                //dataList1 = GetFlowData(dtjy1,dtzd1);
                //BPMServiceResult result1 = StartWorkflow("Solutionmortgage", "Administrator", false, dataList1);
                //return;
                try
                {                    
                    string sqlworkitem = "SELECT * FROM OT_WORKITEM WHERE WORKFLOWCODE = 'Mortgage' AND SHEETCODE = 'MortgageSelf' AND displayName='等待'";
                    DataTable dtlis = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlworkitem);
                    Console.Write("等待数据：" + dtlis.Rows.Count);
                    string workItemId = "";
                    if (dtlis != null && dtlis.Rows.Count > 0)
                    {
                        for (int i = 0; i < dtlis.Rows.Count; i++)
                        {
                            string InstanceID = dtlis.Rows[i]["INSTANCEID"].ToString();
                            Console.Write("InstanceID：" + InstanceID);
                            string BIZOBJECTID = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT BIZOBJECTID FROM OT_INSTANCECONTEXT WHERE OBJECTID = '" + InstanceID + "'") + string.Empty;
                            Console.Write("BIZOBJECTID：" + BIZOBJECTID);
                            workItemId = dtlis.Rows[i]["OBJECTID"].ToString();
                            Console.Write("workItemId：" + workItemId);
                            string sql = "SELECT OBJECTID,APPLYNO,RCHIVINGRESULTS FROM I_MORTGAGE WHERE OBJECTID = '" + BIZOBJECTID + "'";
                            Console.Write("sql：" + sql);
                            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                            Console.Write("查询数据1：" + dt.Rows.Count);
                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int k = 0; k < dt.Rows.Count; k++)
                                {
                                    string userId = "Administrator", commentText = "";
                                    if (DateTime.Now.Day - Convert.ToDateTime(dtlis.Rows[i]["STARTTIME"]).Day >= 10 && string.IsNullOrEmpty(dt.Rows[k]["RCHIVINGRESULTS"].ToString()))
                                    {                                        
                                        ReturnItem(userId, workItemId, commentText);//日期超过10天，自动驳回
                                    }
                                    else
                                    {
                                        DataTable dtproducts = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM in_cms.mv_dy_application_date_info@to_dy_cms WHERE 申请号 = '" + dt.Rows[k]["APPLYNO"].ToString() + "'");
                                        //DataTable dtproducts = ExecuteDataTableSql("CAPDB", "SELECT * FROM in_cms.mv_dy_application_date_info WHERE 申请号 = '" + dt.Rows[k]["APPLYNO"].ToString() + "'");
                                        Console.Write("查询数据2：" + dtproducts.Rows.Count);
                                        if (dtproducts != null && dtproducts.Rows.Count > 0)
                                        {
                                            for (int d = 0; d < dtproducts.Rows.Count; d++)
                                            {
                                                if (dtproducts.Rows[d]["归档时间"] != null && dtproducts.Rows[d]["归档时间"] != DBNull.Value && !string.IsNullOrEmpty(dtproducts.Rows[d]["归档时间"].ToString()))
                                                {
                                                    string updatesql = "UPDATE I_MORTGAGE SET RCHIVINGRESULTS = '" + dtproducts.Rows[d]["归档时间"] + "' WHERE OBJECTID = '" + BIZOBJECTID + "'";
                                                    int returnresult = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(updatesql);
                                                    SubmitItem(workItemId, OThinker.Data.BoolMatchValue.True, commentText, userId);
                                                    List<DataItemParam> dataList = new List<DataItemParam>();
                                                    string sqljy = "SELECT * FROM I_MORTGAGE WHERE OBJECTID = '" + BIZOBJECTID + "'";
                                                    DataTable dtjy = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqljy);
                                                    string sqlzd = "SELECT * FROM I_GJPEOPLE WHERE PARENTOBJECTID = '" + BIZOBJECTID + "'";
                                                    DataTable dtzd = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqlzd);
                                                    dataList = GetFlowData(dtjy,dtzd);
                                                    BPMServiceResult result = StartWorkflow("Solutionmortgage", userId, false, dataList);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    Log.WriteLog(true, "归档成功。",(DateTime.Now - start).TotalSeconds, start);
                }
                catch (Exception ex)
                {
                    Log.WriteLog(true, "归档异常:异常信息：\r\n" + ex.ToString(), (DateTime.Now - start).TotalSeconds, start);
                }
                Thread.Sleep(2 * 60 * 60 * 1000);
            }
        }

        public List<DataItemParam> GetFlowData(DataTable dt, DataTable dtzd)
        {
            List<DataItemParam> dataList = new List<DataItemParam>();
            if (dt != null && dt.Rows.Count > 0)
            {
                dataList.Add(new DataItemParam { ItemName = "PROVINCE", ItemValue = dt.Rows[0]["PROVINCE"] });
                dataList.Add(new DataItemParam { ItemName = "CITY", ItemValue = dt.Rows[0]["CITY"] });
                dataList.Add(new DataItemParam { ItemName = "APPLYTYPE", ItemValue = dt.Rows[0]["APPLYTYPE"] });
                dataList.Add(new DataItemParam { ItemName = "JKNAME", ItemValue = dt.Rows[0]["JKNAME"] });
                dataList.Add(new DataItemParam { ItemName = "GJNAME", ItemValue = dt.Rows[0]["GJNAME"] });
                dataList.Add(new DataItemParam { ItemName = "DBRNAME", ItemValue = dt.Rows[0]["DBRNAME"] });
                dataList.Add(new DataItemParam { ItemName = "JKCARDID", ItemValue = dt.Rows[0]["JKCARDID"] });
                dataList.Add(new DataItemParam { ItemName = "GJCARDID", ItemValue = dt.Rows[0]["GJCARDID"] });
                dataList.Add(new DataItemParam { ItemName = "DBRCARDID", ItemValue = dt.Rows[0]["DBRCARDID"] });
                dataList.Add(new DataItemParam { ItemName = "ASSETS", ItemValue = dt.Rows[0]["ASSETS"] });
                dataList.Add(new DataItemParam { ItemName = "FACTORY", ItemValue = dt.Rows[0]["FACTORY"] });
                dataList.Add(new DataItemParam { ItemName = "CARTYPE", ItemValue = dt.Rows[0]["CARTYPE"] });
                dataList.Add(new DataItemParam { ItemName = "NEWCARPRICE", ItemValue = dt.Rows[0]["NEWCARPRICE"] });
                dataList.Add(new DataItemParam { ItemName = "ASSETSPRICE", ItemValue = dt.Rows[0]["ASSETSPRICE"] });
                dataList.Add(new DataItemParam { ItemName = "MOTORTYPE", ItemValue = dt.Rows[0]["MOTORTYPE"] });
                dataList.Add(new DataItemParam { ItemName = "FRAMENUMBER", ItemValue = dt.Rows[0]["FRAMENUMBER"] });
                dataList.Add(new DataItemParam { ItemName = "PRODUCTCLASS", ItemValue = dt.Rows[0]["PRODUCTCLASS"] });
                dataList.Add(new DataItemParam { ItemName = "PRODUCTTYPE", ItemValue = dt.Rows[0]["PRODUCTTYPE"] });
                dataList.Add(new DataItemParam { ItemName = "DKMONEY", ItemValue = dt.Rows[0]["DKMONEY"] });
                dataList.Add(new DataItemParam { ItemName = "FJMONEY", ItemValue = dt.Rows[0]["FJMONEY"] });
                dataList.Add(new DataItemParam { ItemName = "SFMONEY", ItemValue = dt.Rows[0]["SFMONEY"] });
                dataList.Add(new DataItemParam { ItemName = "ISSELF", ItemValue = dt.Rows[0]["ISSELF"] });
                dataList.Add(new DataItemParam { ItemName = "LOCALDO", ItemValue = dt.Rows[0]["LOCALDO"] });
                dataList.Add(new DataItemParam { ItemName = "SELECTSHOP", ItemValue = dt.Rows[0]["SELECTSHOP"] });
                dataList.Add(new DataItemParam { ItemName = "SPPROVINCE", ItemValue = dt.Rows[0]["SPPROVINCE"] });
                dataList.Add(new DataItemParam { ItemName = "SPCITY", ItemValue = dt.Rows[0]["SPCITY"] });
                //dataList.Add(new DataItemParam { ItemName = "CERTIFICATE", ItemValue = dt.Rows[0]["CERTIFICATE"] });
                dataList.Add(new DataItemParam { ItemName = "LICENCENO", ItemValue = dt.Rows[0]["LICENCENO"] });
                //dataList.Add(new DataItemParam { ItemName = "OTHERFILE", ItemValue = dt.Rows[0]["OTHERFILE"] });
                //dataList.Add(new DataItemParam { ItemName = "MORTGAGEFILE", ItemValue = dt.Rows[0]["MORTGAGEFILE"] });
                //dataList.Add(new DataItemParam { ItemName = "SPTYPE", ItemValue = dt.Rows[0]["SPTYPE"] });
                //dataList.Add(new DataItemParam { ItemName = "SPADVISE", ItemValue = dt.Rows[0]["SPADVISE"] });
                dataList.Add(new DataItemParam { ItemName = "EXPRESSNUMBER", ItemValue = dt.Rows[0]["EXPRESSNUMBER"] });
                dataList.Add(new DataItemParam { ItemName = "RCHIVINGRESULTS", ItemValue = dt.Rows[0]["RCHIVINGRESULTS"] });
                dataList.Add(new DataItemParam { ItemName = "APPLYNO", ItemValue = dt.Rows[0]["APPLYNO"] });
                List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();
                if(dtzd != null && dtzd.Rows.Count > 0)
                {
                    for (int i = 0; i < dtzd.Rows.Count; i++)
                    {
                        chileList.Add(new List<DataItemParam> {
                            new DataItemParam { ItemName = "PEOPLE", ItemValue = dtzd.Rows[i]["PEOPLE"] },
                            new DataItemParam { ItemName = "ID", ItemValue = dtzd.Rows[i]["ID"] },
                            new DataItemParam { ItemName = "IDTYPE", ItemValue = dtzd.Rows[i]["IDTYPE"] },
                            new DataItemParam { ItemName = "PEOPLETYPE", ItemValue = dtzd.Rows[i]["PEOPLETYPE"] } });
                    }
                }
                dataList.Add(new DataItemParam { ItemName = "GJPeoples", ItemValue = chileList });
            }
            return dataList;
        }
        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            try
            {
                var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteDataTable(sql);
                }
                return null;
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write("ExecuteDataTableSql Exception:connectionCode-->" + connectionCode + ",sql-->" + sql + "," + ex);
                return null;
            }
        }
        /// <summary>
        /// 提交工作项
        /// </summary>
        /// <param name="workItemId">工作项ID</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        /// <param name="userId">处理人</param>
        public void SubmitItem(string workItemId, OThinker.Data.BoolMatchValue approval, string commentText, string userId)
        {
            //Console.Write("开始提交工作项");
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext instance = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // 添加意见
            this.AppendComment(instance, item, OThinker.Data.BoolMatchValue.Unspecified, commentText);

            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                item.ObjectID,
                userId,
                OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                null,
                approval,
                commentText,
                null,
                OThinker.H3.WorkItem.ActionEventType.Forward,
                (int)OThinker.H3.Controllers.SheetButtonType.Submit);
            // 需要通知实例事件管理器结束事件
            OThinker.H3.Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    approval,
                    false,
                    approval,
                    true,
                    null);
            this.Engine.InstanceManager.SendMessage(endMessage);
        }
        /// <summary>
        /// 驳回工作任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="workItemId"></param>
        /// <param name="commentText"></param>
        /// <returns></returns>
        public bool ReturnItem(string userId, string workItemId, string commentText)
        {
            //Console.Write("不回");
            OThinker.Organization.User user = this.Engine.Organization.GetUnit(userId) as OThinker.Organization.User;
            if (user == null) return false;
            // 获取工作项
            OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);
            OThinker.H3.Instance.InstanceContext context = this.Engine.InstanceManager.GetInstanceContext(item.InstanceId);
            // ToKen
            OThinker.H3.Instance.IToken Token = context.GetToken(item.TokenId);
            if (Token.PreTokens == null) return false;
            int PreToken = int.Parse(Token.PreTokens[0].ToString());
            OThinker.H3.Instance.IToken PreToken1 = context.GetToken(PreToken);
            string activityName = PreToken1.Activity;
            // 添加意见
            this.AppendComment(context, item, OThinker.Data.BoolMatchValue.False, commentText);
            // 结束工作项
            this.Engine.WorkItemManager.FinishWorkItem(
                  item.ObjectID,
                  user.ObjectID,
                  OThinker.H3.WorkItem.AccessPoint.ExternalSystem,

                  null,
                  OThinker.Data.BoolMatchValue.False,
                  commentText,
                  null,
                  OThinker.H3.WorkItem.ActionEventType.Backward,
                  (int)OThinker.H3.Controllers.SheetButtonType.Return);
            // 准备触发后面Activity的消息
            OThinker.H3.Messages.ActivateActivityMessage activateMessage
                = new OThinker.H3.Messages.ActivateActivityMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,
                activityName,
                OThinker.H3.Instance.Token.UnspecifiedID,
                null,
                new int[] { item.TokenId },
                false,
                OThinker.H3.WorkItem.ActionEventType.Backward);

            // 通知该Activity已经完成
            OThinker.H3.Messages.AsyncEndMessage endMessage =
                new OThinker.H3.Messages.AsyncEndMessage(
                    OThinker.H3.Messages.MessageEmergencyType.Normal,
                    item.InstanceId,
                    item.ActivityCode,
                    item.TokenId,
                    OThinker.Data.BoolMatchValue.False,
                    true,
                    OThinker.Data.BoolMatchValue.False,
                    false,
                    activateMessage);
            this.Engine.InstanceManager.SendMessage(endMessage);
            return true;
        }
        /// <summary>
        /// 给工作项添加审批意见
        /// </summary>
        /// <param name="item">工作项</param>
        /// <param name="approval">审批结果</param>
        /// <param name="commentText">审批意见</param>
        private void AppendComment(OThinker.H3.Instance.InstanceContext Instance, OThinker.H3.WorkItem.WorkItem item, OThinker.Data.BoolMatchValue approval, string commentText)
        {
            // 添加一个审批意见
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflow = this.Engine.WorkflowManager.GetPublishedTemplate(
                 item.WorkflowCode,
                 item.WorkflowVersion);
            // 审批字段
            string approvalDataItem = null;
            if (workflow != null)
            {
                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(item.WorkflowCode);
                approvalDataItem = workflow.GetDefaultCommentDataItem(schema, item.ActivityCode);
            }
            if (approvalDataItem != null)
            {
                // 创建审批
                OThinker.H3.Data.Comment comment = new OThinker.H3.Data.Comment();
                comment.Activity = item.ActivityCode;
                comment.Approval = approval;
                comment.CreatedTime = System.DateTime.Now;
                comment.DataField = approvalDataItem;
                comment.InstanceId = item.InstanceId;
                comment.BizObjectId = Instance.BizObjectId;
                comment.BizObjectSchemaCode = Instance.BizObjectSchemaCode;
                comment.OUName = this.Engine.Organization.GetName(this.Engine.Organization.GetParent(item.Participant));
                comment.Text = commentText;
                comment.TokenId = item.TokenId;
                comment.UserID = item.Participant;

                // 设置用户的默认签章
                OThinker.Organization.Signature[] signs = this.Engine.Organization.GetSignaturesByUnit(item.Participant);
                if (signs != null && signs.Length > 0)
                {
                    foreach (OThinker.Organization.Signature sign in signs)
                    {
                        if (sign.IsDefault)
                        {
                            comment.SignatureId = sign.ObjectID;
                            break;
                        }
                    }
                }
                this.Engine.BizObjectManager.AddComment(comment);
            }
        }
        private OThinker.H3.IEngine _Engine = null;
        public OThinker.H3.IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == OThinker.H3.ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set { _Engine = value; }
        }
        /// <summary>
        /// 身份验证类
        /// </summary>
        public class Authentication : System.Web.Services.Protocols.SoapHeader
        {
            public Authentication()
            {
            }
            public Authentication(string UserCode, string Password)
            {
                this.UserCode = UserCode;
                this.Password = Password;
            }
            public string UserCode { get; set; }
            public string Password { get; set; }
        }
        public Authentication authentication = new Authentication("administrator", "shdzretail1");
        public OThinker.H3.Controllers.UserValidator UserValidator = null;
        /// <summary>
        /// 验证当前用户是否正确
        /// </summary>
        /// <returns></returns>
        public void ValidateSoapHeader()
        {
            if (authentication == null)
            {
                throw new Exception("请输入身份认证信息!");
            }
            UserValidator = OThinker.H3.Controllers.UserValidatorFactory.Validate(authentication.UserCode, authentication.Password);
            if (UserValidator == null)
            {
                throw new Exception("帐号或密码不正确!");
            }
            Engine = UserValidator.Engine;
        }
        /// <summary>
        /// 获取最新版本号
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode)
        {
            return GetWorkflowTemplate(workflowCode, Engine.WorkflowManager.GetWorkflowDefaultVersion(workflowCode));
        }
        /// <summary>
        /// 获取模板
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="workflowVersion"></param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode, int workflowVersion)
        {
            return Engine.WorkflowManager.GetPublishedTemplateHeader(workflowCode, workflowVersion); ;
        }
        public string GetUserIDByCode(string UserCode)
        {
            //var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, UserCode);
            //return CurrentUserValidator.UserID;
            string sql = "SELECT OBJECTID FROM OT_USER WHERE CODE = '" + UserCode + "'";
            string userId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql).ToString();
            return userId;
        }
        /// <summary>
        /// 启动流程实例
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="userCode"></param>
        /// <param name="finishStart"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public BPMServiceResult StartWorkflow(string workflowCode, string userCode, bool finishStart, List<DataItemParam> paramValues)
        {
            //ValidateSoapHeader();
            BPMServiceResult result = new BPMServiceResult();
            try
            {
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(workflowCode);
                if (workflowTemplate == null)
                {
                    return new BPMServiceResult(false, "流程启动失败,流程模板不存在，模板编码:" + workflowCode + "。");
                }
                // 查找流程发起人
                //OThinker.Organization.User user = Engine.Organization.GetUnitByCode(userCode) as Organization.User;
                string user = GetUserIDByCode(userCode);
                if (user == null)
                {
                    return new BPMServiceResult(false, "流程启动失败,用户{" + userCode + "}不存在。");
                }
                OThinker.H3.DataModel.BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema, user);
                if (paramValues != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in paramValues)
                    {
                        if (bo.Schema.GetProperty(param.ItemName).LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                        {
                            var t = new List<OThinker.H3.DataModel.BizObject>();
                            foreach (List<DataItemParam> list in (IEnumerable)param.ItemValue)
                            {
                                var m = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema.Fields.FirstOrDefault(x => x.ChildSchemaCode == param.ItemName).Schema, user);
                                foreach (DataItemParam item in list)
                                {
                                    if (m.Schema.ContainsField(item.ItemName)) m.SetValue(item.ItemName, item.ItemValue);
                                }
                                t.Add(m);
                            }
                            bo[param.ItemName] = t.ToArray();
                        }
                        else if (bo.Schema.ContainsField(param.ItemName))
                        {
                            bo[param.ItemName] = param.ItemValue;
                        }
                    }
                }
                bo.Create();
                // 创建流程实例
                //string InstanceId = this.Engine.InstanceManager.CreateInstance(bo.ObjectID,workflowTemplate.WorkflowCode,workflowTemplate.WorkflowVersion,
                //    null,null,user,null, null, false, OThinker.H3.Instance.InstanceContext.UnspecifiedID,null,OThinker.H3.Instance.Token.UnspecifiedID);
                string InstanceId = this.Engine.InstanceManager.CreateInstanceByDefault(bo.ObjectID, workflowTemplate.WorkflowCode, null, user);
                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();
                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage = new OThinker.H3.Messages.StartInstanceMessage(emergency,
                    InstanceId, null, paramTables, OThinker.H3.Instance.PriorityType.Normal, true, null, false,
                    OThinker.H3.Instance.Token.UnspecifiedID, null);
                Engine.InstanceManager.SendMessage(startInstanceMessage);
                result = new BPMServiceResult(true, InstanceId, null, "流程实例启动成功！", "");
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.Message);
            }
            return result;
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
    }
}
