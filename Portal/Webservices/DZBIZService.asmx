<%@ WebService Language="C#" Class="OThinker.H3.Portal.DZBIZService" %>
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using OThinker.H3.Acl;
using OThinker.H3.WorkflowTemplate;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using OThinker.H3.DataModel;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
//using OThinker.H3.Controllers.ViewModels;// 19.7.2 wangxg
using OThinker.H3.Data;
using OThinker.H3.Instance;
using OThinker.H3.Messages;
using OThinker.Organization;
using System.Configuration;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 
namespace OThinker.H3.Portal
{
    /// <summary>
    /// 流程实例操作相关接口
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.None)]
    //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释。 
    // [System.Web.Script.Services.ScriptService]
    public class DZBIZService : System.Web.Services.WebService
    {
        /// <summary>
        /// 
        /// </summary>
        public DZBIZService()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }
        /// <summary>
        /// 零售信审审批和放款审批分单逻辑
        /// </summary>
        /// <param name="dep">部门</param>
        /// <param name="post">岗位</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        [WebMethod(Description = "零售信审审批和放款审批分单逻辑")]
        public string BizAutoAssign(string instanceID, string activeCode, string cyz, string amount)
        {
            string userID = "";
            WorkFlowFunction wf = new WorkFlowFunction();
            userID = wf.BizAutoAssign(instanceID, activeCode, cyz, amount);
            return userID;
        }
        /// <summary>
        /// 零售信审审批和放款审批分单逻辑
        /// </summary>
        /// <param name="dep">部门</param>
        /// <param name="post">岗位</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        [WebMethod(Description = "零售信审审批和放款审批分单逻辑")]
        public string BizAutoAssignHistory(string instanceID, string activeCode, string cyz, string amount)
        {
            if (!(cyz == null || cyz == ""))
            {
                return cyz;
            }
            string userID = "";
            string dep = "", post = "";
            string sql = " SELECT * FROM (select b.OBJECTID,a.CODE from DZ_AUTOASSIGN a,OT_User b ";
            string sqlCondition = " WHERE A.CODE=B.CODE";
            switch (activeCode)
            {
                //信贷初审 信贷终审  运营初审 运营终审
                case "信审初审": dep = "信贷审批部"; post = "初审"; break;
                case "信审终审": dep = "信贷审批部"; post = "终审"; sql += ",OT_WORKITEMFINISHED c "; sqlCondition += " and C.PARTICIPANT!=B.OBJECTID and C.INSTANCEID='" + instanceID + "'"; break;
                case "运营初审": dep = "运管部"; post = "初审"; sql += ",OT_WORKITEMFINISHED c "; sqlCondition += " and C.PARTICIPANT!=B.OBJECTID and C.INSTANCEID='" + instanceID + "'"; break;
                case "运营终审": dep = "运管部"; post = "终审"; sql += ",OT_WORKITEMFINISHED c "; sqlCondition += " and C.PARTICIPANT!=B.OBJECTID and C.INSTANCEID='" + instanceID + "'"; break;
            }
            sql = sql + sqlCondition + " and a.DEP='" + dep + "'";
            if (dep.Contains("信贷审批部") && post.Contains("初审"))
            {
                sql += " AND a.LIMIT >=" + amount;
            }
            sql += "  AND a.POST='" + post + "'  AND a.STATE=1 ORDER BY a.CODE DESC) WHERE ROWNUM=1 ";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt != null && dt.Rows.Count > 0)
            {
                userID = dt.Rows[0][0].ToString();
                //更新统计
                if (dep.Contains("信贷审批部") && post.Contains("初审"))
                {
                    this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("update DZ_AUTOASSIGN set COUNTER=RATIO + COUNTER where CODE='" + dt.Rows[0][1].ToString() + "'");
                }
            }
            return userID;
        }
        [WebMethod(Description = "取消流程实例")]
        public string cancelInstanceData(string instanceID, string workflowCode)
        {
            return new WorkFlowFunction().cancelInstanceData(instanceID);
        }
        [WebMethod(Description = "流程实例数据更新")]
        public string updateInstanceData(string instanceID, string workflowCode)
        {
            return new WorkFlowFunction().updateInstanceData(instanceID, workflowCode);
        }
        [WebMethod(Description = "流程实例结束")]
        public string finishedInstance(string instanceID)
        {
            return new WorkFlowFunction().finishedInstance(instanceID).ToString();
        }
[WebMethod(Description = "获取经销商放款统计信息")]
        public string postDZJR(string type,string url)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "----------------------经销商放款信息统计开始------------------");
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "经销商放款信息统计传入type参数：" + type + "，url地址：" + url);
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"DZJR\":[");
            DataTable dt = new DataTable();
            if (type == "INKPIDATA")
            {
                dt = new WorkFlowFunction().getJXSFQBL();
            }
            else if (type == "REFUSEDATA")
            {
                //首先判断是否为星期一，是才开始发送上一周的数据
                int xqj = new WorkFlowFunction().CaculateWeekDay(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                if (xqj != 1)
                {
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + "经销商放款信息统计回传结果：非星期一不会调用拒批接口" );
                }
                dt = new WorkFlowFunction().getDZJR_REFUSEDATA();
            }
            else if (type == "INSUBDATA")
            {
                dt = new WorkFlowFunction().getJXSFQBLMX();
            }
            if (CommonFunction.hasData(dt))
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    jsonBuilder.Append("{");
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        jsonBuilder.Append("\"");
                        jsonBuilder.Append(dt.Columns[j].ColumnName);
                        jsonBuilder.Append("\":\"");
                        jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\"")); //对于特殊字符，还应该进行特别的处理。
                        jsonBuilder.Append("\",");
                    }
                    jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                    jsonBuilder.Append("},");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            }
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            try
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "经销商放款信息统计传出参数：" + jsonBuilder.ToString());
                //集团地址
                string result=new WorkFlowFunction().PostMoths(url, jsonBuilder.ToString());
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "经销商放款信息统计回传结果：" + result.ToString());
            }
            catch(Exception ex)
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "经销商放款信息统计报错：" + ex.Message);
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "----------------------经销商放款信息统计结束------------------");
            return jsonBuilder.ToString();
        }
        /// <summary>
        /// 不考虑附件
        /// </summary>
        /// <param name="WorkFlowCode">流程编码</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="JsonStr">表单数据json格式字符串</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("authentication")]
        [WebMethod(Description = "发起流程自带子表数据")]
        public string StartWorkFlowWidthValues(string WorkFlowCode, string UserCode, string JsonStr)
        {
            //ValidateSoapHeader();
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;
            BPMServiceResult result;
            try
            {
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(WorkFlowCode);
                if (workflowTemplate == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,流程模板不存在，模板编码:" + WorkFlowCode + "。");
                    return JsonConvert.SerializeObject(result);
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(UserCode) as Organization.User;
                if (user == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,用户{" + UserCode + "}不存在。");
                    return JsonConvert.SerializeObject(result);
                }
                //将json格式字符串转化为键值对
                var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonStr);
                // 这里演示了构造 BizObject 对象，以及调用 BizObject 对象的 Create 方法
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(WorkFlowCode);
                BizObject bo = new BizObject(this.Engine, schema, user.ObjectID);
                foreach (var key in dic.Keys)
                {
                    var value = dic[key];
                    //字段类型
                    var logicType = schema.GetLogicType(key);
                    //多人参与者
                    if (logicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                    {
                        //如果值为空继续循环
                        if (string.IsNullOrEmpty(value.ToString()))
                        {
                            continue;
                        }
                        bo[key] = JsonConvert.DeserializeObject<string[]>(value.ToString());
                    }
                    //子表
                    else if (logicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                    {
                        //如果值为空继续循环
                        if (string.IsNullOrEmpty(value.ToString()))
                        {
                            continue;
                        }
                        var childList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(value.ToString());

                        // 子表数据项赋值，适用于子表类型数据项（不适用于关联关系）
                        BizObjectSchema childSchema = schema.GetProperty(key).ChildSchema; // 获取子业务对象编码
                        var lstChildBO = new List<BizObject>();
                        /*
                        这里类似以上BizObject的方法给 childBO 赋值
                        例如：childBO["短文本数据项"] = "值";
                        */
                        foreach (var child in childList)
                        {
                            BizObject childBO = new BizObject(this.Engine, childSchema, user.ObjectID);
                            foreach (var childKey in child.Keys)
                            {
                                var childValue = child[childKey];
                                //字段类型
                                var childLogicType = childSchema.GetLogicType(childKey);
                                //多人参与者
                                if (childLogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                                {
                                    //如果值为空继续循环
                                    if (string.IsNullOrEmpty(childValue.ToString()))
                                    {
                                        continue;
                                    }
                                    childBO[childKey] = JsonConvert.DeserializeObject<string[]>(childValue.ToString());
                                }
                                else
                                {
                                    childBO[childKey] = childValue;
                                }
                            }
                            lstChildBO.Add(childBO);
                        }
                        //bo["子表类型数据项(业务对象类型)"] = childBO;  // 针对业务对象类型数据项，赋值是 BizObject
                        // 针对是业务对象数组类型数据项，赋值是 BizObject[];
                        //bo["子表类型数据项(业务对象数组/子表类型)"] = new BizObject[] { childBO };
                        bo[key] = lstChildBO.ToArray();
                    }
                    else
                    {
                        bo[key] = value;
                    }
                }
                bo.Create();
                // 创建流程实例
                string InstanceId = this.Engine.InstanceManager.CreateInstance(
                     bo.ObjectID,
                     workflowTemplate.WorkflowCode,
                     workflowTemplate.WorkflowVersion,
                     null,
                     null,
                     user.UnitID,
                     null,
                     false,
                     Instance.InstanceContext.UnspecifiedID,
                     null,
                     Instance.Token.UnspecifiedID);

                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();

                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                    = new OThinker.H3.Messages.StartInstanceMessage(
                        emergency,
                        InstanceId,
                        null,
                        paramTables,
                        Instance.PriorityType.Normal,
                        true,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                Engine.InstanceManager.SendMessage(startInstanceMessage);
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", "");
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.ToString());
            }
            return JsonConvert.SerializeObject(result);
        }
        /// <summary>
        /// 不考虑附件
        /// </summary>
        /// <param name="WorkFlowCode">流程编码</param>
        /// <param name="UserID">用户ID</param>
        /// <param name="JsonStr">表单数据json格式字符串</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("authentication")]
        [WebMethod(Description = "发起流程自带子表数据")]
        public string updateWorkFlowData(string WorkFlowCode, string UserCode, string JsonStr, string instanceid)
        {
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;

            string rtn = "";
            var user = this.Engine.Organization.GetUserByCode("administrator" + string.Empty);
            InstanceContext ic = this.Engine.InstanceManager.GetInstanceContext(instanceid);
            if (ic == null)
            {
                return "";
            }
            //OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(WorkFlowCode);
            //OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(
            //    this.Engine.Organization,
            //    this.Engine.MetadataRepository,
            //    this.Engine.BizObjectManager,
            //    null,
            //    schema,
            //    user.ObjectID,
            //    user.ParentID);
            BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(WorkFlowCode);
            BizObject bo = new BizObject(this.Engine, schema, user.ObjectID);
            bo.ObjectID = ic.BizObjectId;
            bo.Load();//装载流程数据；
                      //将json格式字符串转化为键值对
            var dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonStr);
            // 这里演示了构造 BizObject 对象，以及调用 BizObject 对象的 Create 方法
            foreach (var key in dic.Keys)
            {
                var value = dic[key];
                //字段类型
                var logicType = schema.GetLogicType(key);
                //多人参与者
                if (logicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                {
                    //如果值为空继续循环
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        continue;
                    }
                    bo[key] = JsonConvert.DeserializeObject<string[]>(value.ToString());
                }
                //子表
                else if (logicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                {
                    //如果值为空继续循环
                    if (string.IsNullOrEmpty(value.ToString()))
                    {
                        continue;
                    }
                    var childList = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(value.ToString());

                    // 子表数据项赋值，适用于子表类型数据项（不适用于关联关系）
                    BizObjectSchema childSchema = schema.GetProperty(key).ChildSchema; // 获取子业务对象编码
                    var lstChildBO = new List<BizObject>();
                    /*
                    这里类似以上BizObject的方法给 childBO 赋值
                    例如：childBO["短文本数据项"] = "值";
                    */
                    foreach (var child in childList)
                    {
                        BizObject childBO = new BizObject(this.Engine, childSchema, user.ObjectID);
                        foreach (var childKey in child.Keys)
                        {
                            var childValue = child[childKey];
                            //字段类型
                            var childLogicType = childSchema.GetLogicType(childKey);
                            //多人参与者
                            if (childLogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
                            {
                                //如果值为空继续循环
                                if (string.IsNullOrEmpty(childValue.ToString()))
                                {
                                    continue;
                                }
                                childBO[childKey] = JsonConvert.DeserializeObject<string[]>(childValue.ToString());
                            }
                            else
                            {
                                childBO[childKey] = childValue;
                            }
                        }
                        lstChildBO.Add(childBO);
                    }
                    //bo["子表类型数据项(业务对象类型)"] = childBO;  // 针对业务对象类型数据项，赋值是 BizObject
                    // 针对是业务对象数组类型数据项，赋值是 BizObject[];
                    //bo["子表类型数据项(业务对象数组/子表类型)"] = new BizObject[] { childBO };
                    bo[key] = lstChildBO.ToArray();
                }
                else
                {
                    bo[key] = value;
                }
            }
            bo.Update();
            #region 结束当前任务
            string sql = "SELECT ObjectID FROM OT_WorkItem WHERE InstanceId='{0}' ORDER BY TokenId desc";
            sql = string.Format(sql, instanceid);
            string workItemId = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            if (workItemId != "")
            {
                // 获取工作项
                OThinker.H3.WorkItem.WorkItem item = this.Engine.WorkItemManager.GetWorkItem(workItemId);

                // 结束工作项
                this.Engine.WorkItemManager.FinishWorkItem(
                    item.ObjectID,
                    user.UnitID,
                    OThinker.H3.WorkItem.AccessPoint.ExternalSystem,
                    null,
                    OThinker.Data.BoolMatchValue.True,
                    string.Empty,
                    null,
                    OThinker.H3.WorkItem.ActionEventType.Forward,
                    11);

                // 需要通知实例事件管理器结束事件
                AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(
                        MessageEmergencyType.Normal,
                        item.InstanceId,
                        item.ActivityCode,
                        item.TokenId,
                        OThinker.Data.BoolMatchValue.True,
                        false,
                        OThinker.Data.BoolMatchValue.True,
                        true,
                        null);
                this.Engine.InstanceManager.SendMessage(endMessage);
            }
            #endregion
            return rtn;

        }
        /// <summary>
        /// 启动H3流程实例
        /// </summary>
        /// <param name="xmlData">CAP Data</param>
        /// <returns></returns>
        //[WebMethod(Description = "Start H3 workflow")]
        //public string StartWorkFlow(string workflowCode, string userCode, bool finishStart, List<DataItemParam> paramValues)
        //{
        //    string result = "";
        //    BPMServiceResult bpmresult;
        //    bpmresult = startWorkflow(workflowCode, userCode, finishStart, paramValues);
        //    result = result = "{ \"Success\": \"" + bpmresult.Success + "\", \"Message\":\"" + bpmresult.Message + "\"}";
        //    return result;
        //}
        [WebMethod(Description = "设置流程数据项的值")]
        public bool SetSigleItemValues(string bizObjectSchemaCode, string bizObjectId, string ItemName, string ItemValue)
        {
            if (ItemName == "" || ItemValue == "") return false;
            Dictionary<string, object> values = new Dictionary<string, object>();
            values.Add(ItemName, ItemValue);
            return this.Engine.BizObjectManager.SetPropertyValues(bizObjectSchemaCode, bizObjectId, "18f923a7-5a5e-426d-94ae-a55ad1a4b239", values);
        }
        /// <summary>
        /// 设置批量流程数据项的值
        /// </summary>
        /// <param name="bizObjectSchemaCode">流程模板编码</param>
        /// <param name="bizObjectId">流程实例ID</param>
        /// <param name="keyValues"></param>
        /// <returns></returns>
        [WebMethod(Description = "设置批量流程数据项的值")]
        [System.Web.Services.Protocols.SoapHeader("authentication")]
        public bool SetItemValues(string bizObjectSchemaCode, string bizObjectId, List<DataItemParam> keyValues)
        {
            ValidateSoapHeader();
            // 获取操作的用户
            if (keyValues == null || keyValues.Count == 0) return false;
            Dictionary<string, object> values = new Dictionary<string, object>();
            foreach (DataItemParam param in keyValues)
            {
                values.Add(param.ItemName, param.ItemValue);
            }
            return this.Engine.BizObjectManager.SetPropertyValues(bizObjectSchemaCode, bizObjectId, OThinker.H3.Controllers.UserValidatorFactory.CurrentUser.UserID, values);
        }
        private IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
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
            set
            {
                _Engine = value;
            }
        }
        /// <summary>
        /// 启动H3流程实例
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="userCode">启动流程的用户编码</param>
        /// <param name="finishStart">是否结束第一个活动</param>
        /// <param name="paramValues">流程实例启动初始化数据项集合</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("authentication")]
        [WebMethod(Description = "启动H3流程实例")]
        public BPMServiceResult StartWorkflow(
            string workflowCode,
            string userCode,
            bool finishStart,
            List<DataItemParam> paramValues)
        {
            ValidateSoapHeader();
            return startWorkflow(workflowCode, userCode, finishStart, paramValues);
        }
        public Authentication authentication;
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
            this.Engine = UserValidator.Engine;
            // this.Engine = OThinker.H3.WorkSheet.AppUtility.Engine;
        }
        private BPMServiceResult startWorkflow(
        string workflowCode,
        string userCode,
        bool finishStart,
        List<DataItemParam> paramValues)
        {
            //ValidateSoapHeader();
            string workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;
            BPMServiceResult result;

            try
            {
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(workflowCode);
                if (workflowTemplate == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,流程模板不存在，模板编码:" + workflowCode + "。");
                    return result;
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(userCode) as Organization.User;
                if (user == null)
                {
                    result = new BPMServiceResult(false, "流程启动失败,用户{" + userCode + "}不存在。");
                    return result;
                }

                OThinker.H3.DataModel.BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                OThinker.H3.DataModel.BizObject bo = new DataModel.BizObject(
                    this.Engine.Organization,
                    this.Engine.MetadataRepository,
                    this.Engine.BizObjectManager,
                    this.Engine.BizBus,
                    schema,
                    OThinker.Organization.User.AdministratorID,
                    OThinker.Organization.OrganizationUnit.DefaultRootID);

                if (paramValues != null)
                {
                    // 这里可以在创建流程的时候赋值
                    foreach (DataItemParam param in paramValues)
                    {

                        if (bo.Schema.ContainsField(param.ItemName))
                        {
                            bo[param.ItemName] = param.ItemValue;
                        }
                    }
                }

                bo.Create();

                // 创建流程实例
                string InstanceId = this.Engine.InstanceManager.CreateInstance(
                     bo.ObjectID,
                     workflowTemplate.WorkflowCode,
                     workflowTemplate.WorkflowVersion,
                     null,
                     null,
                     user.UnitID,
                     null,
                     false,
                     Instance.InstanceContext.UnspecifiedID,
                     null,
                     Instance.Token.UnspecifiedID);

                // 设置紧急程度为普通
                OThinker.H3.Messages.MessageEmergencyType emergency = Messages.MessageEmergencyType.Normal;
                // 这里也可以在启动流程的时候赋值
                Dictionary<string, object> paramTables = new Dictionary<string, object>();

                // 启动流程的消息
                OThinker.H3.Messages.StartInstanceMessage startInstanceMessage
                    = new OThinker.H3.Messages.StartInstanceMessage(
                        emergency,
                        InstanceId,
                        null,
                        paramTables,
                        Instance.PriorityType.Normal,
                        true,
                        null,
                        false,
                        OThinker.H3.Instance.Token.UnspecifiedID,
                        null);
                Engine.InstanceManager.SendMessage(startInstanceMessage);
                result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", "");
            }
            catch (Exception ex)
            {
                result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.ToString());
            }
            return result;
        }
        /// <summary>
        /// 获取最新的流程模板
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode)
        {
            // 获取最新版本号
            int workflowVersion = this.Engine.WorkflowManager.GetWorkflowDefaultVersion(workflowCode);
            return GetWorkflowTemplate(workflowCode, workflowVersion);
        }
        /// <summary>
        /// 获取指定版本号的流程模板对象
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="workflowVersion">流程模板版本号</param>
        /// <returns></returns>
        private OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode, int workflowVersion)
        {
            // 获取模板
            OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplateHeader(
                    workflowCode,
                    workflowVersion);
            return workflowTemplate;
        }
        /// <summary>
        /// 调用融数接口
        /// </summary>
        /// <param name="instanceid">流程实例ID</param>
        /// <returns></returns>
        [System.Web.Services.Protocols.SoapHeader("authentication")]
        [WebMethod(Description = "调用融数接口")]
        public string postHttp(string SchemaCode,string instanceid)
        {
            WorkFlowFunction wf = new WorkFlowFunction();
            return wf.postHttp(SchemaCode,instanceid);
        }
        /// <summary>
        /// 获取内外网经销商
        /// </summary>
        /// <param name="nww"></param>
        /// <returns></returns>
        [WebMethod(Description = "获取内外网经销商")]
        public List<jxs_model> getjxs(string nww)
        {
            List<jxs_model> lst_item = new List<jxs_model>();
            DataTable dt = new WorkFlowFunction().getjxs(nww);
            foreach (DataRow row in dt.Rows)
            {
                lst_item.Add(new jxs_model() { jxsname = row[0].ToString(), nww = row[1].ToString() });
            }
            return lst_item;
        }
        /// <summary>
        /// 经销商额度管理名单
        /// </summary>
        [WebMethod(Description = "获取经销商额度管理明细")]
        public List<jxs_model> getJxsEdglList(string nww, string jxs)
        {
            DataTable dt = new WorkFlowFunction().getJxsEdglList(nww, jxs);
            List<jxs_model> lst = new List<jxs_model>();
            lst = CommonFunction.DataTableToList<jxs_model>(dt);
            return lst;
        }
/// <summary>
        /// 客户收入负债比测算
        /// </summary>
        [WebMethod(Description = "客户收入负债比测算")]
        public int GetGuessCustomer(string instanceid)
        {
            return new WorkFlowFunction().AutomaticApprovalByRongShu(instanceid);
        }
    }

    public class Employee
    {
        public Employee() { }
        public Employee(string UserCode, string Password)
        {
            this.UserCode = UserCode;
            this.Password = Password;
        }

        public string UserCode { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 身份验证类
    /// </summary>
    public class Authentication : System.Web.Services.Protocols.SoapHeader
    {
        public Authentication() { }
        public Authentication(string UserCode, string Password)
        {
            this.UserCode = UserCode;
            this.Password = Password;
        }

        public string UserCode { get; set; }
        public string Password { get; set; }
    }

    ///// <summary>
    ///// 流程服务返回消息类
    ///// </summary>
    //public class BPMServiceResult
    //{
    //    /// <summary>
    //    /// 消息类构造函数
    //    /// </summary>
    //    /// <param name="success"></param>
    //    /// <param name="instanceId"></param>
    //    /// <param name="workItemId"></param>
    //    /// <param name="message"></param>
    //    public BPMServiceResult(bool success, string instanceId, string workItemId, string message, string WorkItemUrl)
    //    {
    //        this.Success = success;
    //        this.InstanceID = instanceId;
    //        this.Message = message;
    //        this.WorkItemID = workItemId;
    //        this.WorkItemUrl = WorkItemUrl;
    //    }

    //    /// <summary>
    //    /// 消息类构造函数
    //    /// </summary>
    //    /// <param name="success"></param>
    //    /// <param name="message"></param>
    //    public BPMServiceResult(bool success, string message)
    //        : this(success, string.Empty, string.Empty, message, string.Empty)
    //    {

    //    }

    //    public BPMServiceResult() { }

    //    private bool success = false;
    //    /// <summary>
    //    /// 获取或设置流程启动是否成功
    //    /// </summary>
    //    public bool Success
    //    {
    //        get { return success; }
    //        set { success = value; }
    //    }
    //    private string instanceId = string.Empty;
    //    /// <summary>
    //    /// 获取或设置启动的流程实例ID
    //    /// </summary>
    //    public string InstanceID
    //    {
    //        get { return instanceId; }
    //        set { this.instanceId = value; }
    //    }
    //    private string message = string.Empty;
    //    /// <summary>
    //    /// 获取或设置系统返回消息
    //    /// </summary>
    //    public string Message
    //    {
    //        get { return message; }
    //        set { this.message = value; }
    //    }
    //    private string workItemId = string.Empty;
    //    /// <summary>
    //    /// 获取或设置第一个节点的ItemID
    //    /// </summary>
    //    public string WorkItemID
    //    {
    //        get { return workItemId; }
    //        set { this.workItemId = value; }
    //    }
    //    private string workItemUrl = string.Empty;
    //    /// <summary>
    //    /// 获取或设置第一个节点的url
    //    /// </summary>
    //    public string WorkItemUrl
    //    {
    //        get { return workItemUrl; }
    //        set { this.workItemUrl = value; }
    //    }
    //}

    /// <summary>
    /// 提交任务后返回对象
    /// </summary>
    [Serializable]
    public class ReturnWorkItemInfo
    {
        public ReturnWorkItemInfo() { }
        private bool isSuccess = false;
        /// <summary>
        /// 是否提交成功
        /// </summary>
        public bool IsSuccess
        {
            get { return isSuccess; }
            set { this.isSuccess = value; }
        }
        private string workItemUrl = string.Empty;
        /// <summary>
        /// 当前表单地址
        /// </summary>
        public string WorkItemUrl
        {
            get { return workItemUrl; }
            set { this.workItemUrl = value; }
        }
    }
}