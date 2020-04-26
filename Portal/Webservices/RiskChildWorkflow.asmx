<%@ WebService Language="C#" Class="RiskChildWorkflow" %>


using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using System.Text;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;
using stream = System.IO.Stream;
using DongZheng.H3.WebApi.Common.Portal;// 19.7.2 wangxg 

/// <summary>
/// RiskChildWorkflow 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class RiskChildWorkflow : System.Web.Services.WebService
{

    public RiskChildWorkflow()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public void GetRiskDecisionData(string instanceid, string objectid, string auditoremail, string auditor)
    {
        if (string.IsNullOrEmpty(instanceid))
        {
            instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'") + string.Empty;
        }
        string sql = "SELECT * FROM I_RiskPointDecision WHERE PARENTOBJECTID = '" + objectid + "'";
        DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

        OThinker.H3.Instance.InstanceContext instanceContext = OThinker.H3.Controllers.AppUtility.Engine.InstanceManager.GetInstanceContext(instanceid);

        if (dt != null && dt.Rows.Count > 0)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["RISKDECISION"].ToString() == "调整额度")
                {
                    List<DataItemParam> datalist = new List<DataItemParam>();
                    datalist.Add(new DataItemParam { ItemName = "FinalAmount", ItemValue = dt.Rows[i]["TargetLimit"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "parentInstanceId", ItemValue = instanceid });
                    datalist.Add(new DataItemParam { ItemName = "childObjectId", ItemValue = dt.Rows[i]["ObjectID"].ToString() });

                    BPMServiceResult result = StartWorkFlow("Updateline", auditoremail, false, datalist);   //"liuwanying@dongzhengafc.com"

                    #region
                    //if (result.Success)
                    //{

                    //string SQL = string.Format(@" update I_RiskPointDecision  SET childInstanceId='{0}'  where  objectid='{1}'",result.InstanceID,dt.Rows[0]["ObjectID"].ToString());

                    //AppUtility.Engine.Query.CommandFactory.CreateCommand().ExecuteNonQuery(SQL);
                    //   AppUtility.Engine.BizObjectManager.ReloadBizObject("RiskPoint", instanceid);
                    //string user = GetUserIDByCode(auditoremail);
                    //OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplate workflowTemplate = Engine.WorkflowManager.GetDefaultWorkflow("RiskPoint");
                    //OThinker.H3.DataModel.BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(workflowTemplate.BizObjectSchemaCode);
                    //OThinker.H3.DataModel.BizObject bo = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema, user);

                    //OThinker.H3.Instance.InstanceData instanceData = new OThinker.H3.Instance.InstanceData(OThinker.H3.Controllers.AppUtility.Engine, instanceid, null);




                    //bo.ObjectID = instanceContext.BizObjectId;
                    //bo.Load();
                    //var childPoints = instanceData.BizObject.GetValue("RiskPointDecision") as List<OThinker.H3.DataModel.BizObject>;
                    //OThinker.H3.DataModel.BizObject childPoint = childPoints.FirstOrDefault(x => x.ObjectID == dt.Rows[i]["OBJECTID"].ToString());
                    //childPoint.SetValue("ChildInstanceId", result.InstanceID);
                    //instanceData.BizObject.Update();


                    //List<DataItemParam> datalistyy = new List<DataItemParam>();
                    //List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();

                    //chileList.Add(new List<DataItemParam>()
                    //{
                    //    new DataItemParam{ ItemName = "ChildInstanceId", ItemValue = instanceid+ string.Empty
                    //    },
                    //});

                    //datalistyy.Add(new DataItemParam { ItemName = "RiskPointDecision", ItemValue = chileList });
                    //bo.ObjectID = objectid;
                    //bo.Load();
                    //foreach (DataItemParam value in datalistyy)
                    //{
                    //    OThinker.H3.DataModel.PropertySchema property = schema.GetProperty(value.ItemName);
                    //    if (property.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray)
                    //    {
                    //        var t = new List<OThinker.H3.DataModel.BizObject>();
                    //        foreach (List<DataItemParam> list in (IEnumerable)value.ItemValue)
                    //        {
                    //            var m = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, property.ChildSchema, bo.OwnerId);
                    //            foreach (DataItemParam dataItem in list)
                    //            {
                    //                if (m.Schema.ContainsField(dataItem.ItemName)) m.SetValue(dataItem.ItemName, dataItem.ItemValue);
                    //            }
                    //            t.Add(m);
                    //        }
                    //        bo[value.ItemName] = t.ToArray();
                    //    }
                    //    else if (bo.Schema.ContainsField(value.ItemName))
                    //    {
                    //        bo[value.ItemName] = value.ItemValue;
                    //    }
                    //}
                    //bo.Update();
                    //}
                    #endregion


                }
                else if (dt.Rows[i]["RISKDECISION"].ToString() == "关闭账户")
                {
                    List<DataItemParam> datalist = new List<DataItemParam>();
                    datalist.Add(new DataItemParam { ItemName = "FTUFile", ItemValue = dt.Rows[i]["AttachFile"].ToString() });
                    datalist.Add(new DataItemParam { ItemName = "parentInstanceId", ItemValue = instanceid });
                    datalist.Add(new DataItemParam { ItemName = "childObjectId", ItemValue = dt.Rows[i]["ObjectID"].ToString() });
                    BPMServiceResult result = StartWorkFlow("Exitnet", auditoremail, false, datalist);
                }
            }
        }
    }

    public BPMServiceResult StartWorkFlow(string workflowCode, string userCode, bool finishStart, List<DataItemParam> paramValues)
    {
        //ValidateSoapHeader();//wangxg 19.8
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
                                if (m.Schema.ContainsField(item.ItemName)) m[item.ItemName] = item.ItemValue;
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
            //string InstanceId = this.Engine.InstanceManager.CreateInstance(
            //    bo.ObjectID,
            //    workflowTemplate.WorkflowCode,
            //    workflowTemplate.WorkflowVersion,
            //    null,
            //    null,
            //    user,
            //    null, // 以组的身份发起
            //    null, // 以岗位的身份发起
            //    false, //
            //    OThinker.H3.Instance.InstanceContext.UnspecifiedID,
            //    null,
            //    OThinker.H3.Instance.Token.UnspecifiedID);
            string InstanceId = this.Engine.InstanceManager.CreateInstanceByDefault(
                bo.ObjectID, workflowTemplate.WorkflowCode, null, user);
            // 设置紧急程度为普通
            OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
            // 这里也可以在启动流程的时候赋值
            Dictionary<string, object> paramTables = new Dictionary<string, object>();
            // 启动流程的消息
            OThinker.H3.Messages.StartInstanceMessage startInstanceMessage =
                new OThinker.H3.Messages.StartInstanceMessage(emergency, InstanceId, null, paramTables,
                    OThinker.H3.Instance.PriorityType.Normal, finishStart, null, false, OThinker.H3.Instance.Token.UnspecifiedID, null);
            Engine.InstanceManager.SendMessage(startInstanceMessage);
            result = new BPMServiceResult(true, InstanceId, null, "流程实例启动成功！", "");



        }
        catch (Exception ex)
        {
            result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.Message);
        }
        return result;
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

    public string GetUserIDByCode(string UserCode)
    {
        var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(this.Engine, UserCode);
        return CurrentUserValidator.UserID;
    }

    public string getBizobjectByInstanceid(string Instanceid)
    {
        string sql = "select bizobjectid from H3.OT_instancecontext where objectid='" + Instanceid + "'";
        return this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
    }

    public Authentication authentication = new Authentication("administrator", "000000");
    public OThinker.H3.Controllers.UserValidator UserValidator = null;

    /// <summary>
    /// 验证当前用户是否正确
    /// </summary>
    public void ValidateSoapHeader()
    {
        if (authentication == null)
        {
            throw new Exception("请输入身份认证信息!");
        }
        UserValidator = OThinker.H3.Controllers.UserValidatorFactory.Validate(authentication.UserCode, authentication.Password);
        if (UserValidator == null)
        {
            throw new Exception("账号或密码不正确!");
        }
        Engine = UserValidator.Engine;
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

    /// <summary>
    /// 获取最新版本号
    /// </summary>
    /// <param name="workflowCode"></param>
    /// <returns></returns>
    public OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode)
    {
        return GetWorkflowTemplate(workflowCode, Engine.WorkflowManager.GetWorkflowDefaultVersion(workflowCode));
    }

    /// <summary>
    /// 获取模板
    /// </summary>
    /// <param name="workflowCode"></param>
    /// <param name="workflowVersion"></param>
    /// <returns></returns>
    public OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader GetWorkflowTemplate(string workflowCode, int workflowVersion)
    {
        return Engine.WorkflowManager.GetPublishedTemplateHeader(workflowCode, workflowVersion);
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
            set { itemValue = value; }
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