<%@ WebHandler Language="C#" Class="CRM_To_H3_EarlyWarning" %>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using OThinker.H3;
using OThinker.H3.Controllers;
    /// <summary>
    /// 预警接口
    /// </summary>
public class CRM_To_H3_EarlyWarning : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        context.Response.ContentType = "text/plain";
        string data = "";
        using (StreamReader sr = new StreamReader(context.Request.InputStream))
        {
            data = sr.ReadToEnd();
            JObject jtoken = JObject.Parse(data);
            data = jtoken["data"].ToString();
        }
        AppUtility.Engine.LogWriter.Write("CRM_To_H3接口预警流程数据：" + data);
        JavaScriptSerializer json = new JavaScriptSerializer();
        WorkFlowMethod.Return result = new WorkFlowMethod.Return();
        try
        {
            data = DESDecrypt(data, "B^@s(d)+");
            AppUtility.Engine.LogWriter.Write("CRM_To_H3接口预警解密数据：" + data);
            JObject jsonData = JObject.Parse(data);
            List<DataItemParam> dataList = new List<DataItemParam>();
            dataList.Add(new DataItemParam { ItemName = "CrmDealerId", ItemValue = jsonData["third_system_no"].ToString() });
            dataList.Add(new DataItemParam { ItemName = "WarningInfo", ItemValue = (LetterEnum)Convert.ToInt32(jsonData["warningtype"].ToString()) });
            dataList.Add(new DataItemParam { ItemName = "Reason", ItemValue = jsonData["warningcontent"].ToString() });
            DataTable dealer = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM I_AllowIn WHERE CRMDEALERID='" + jsonData["third_system_no"] + "'");
            if (dealer != null && dealer.Rows.Count > 0)
            {
                dataList.Add(new DataItemParam { ItemName = "Dealer", ItemValue = dealer.Rows[0]["Distributor"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Grade", ItemValue = dealer.Rows[0]["ZScore"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Channels", ItemValue = dealer.Rows[0]["Type"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "DealerClass", ItemValue = dealer.Rows[0]["DistributorType"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Province", ItemValue = dealer.Rows[0]["Province"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "City", ItemValue = dealer.Rows[0]["City"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Address", ItemValue = dealer.Rows[0]["CompanyAddr"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Company", ItemValue = dealer.Rows[0]["BelongTo"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Brand", ItemValue = dealer.Rows[0]["Brand"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "NetSilver", ItemValue = dealer.Rows[0]["QYWYKT"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Loan", ItemValue = dealer.Rows[0]["LoanType"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "OpenDate", ItemValue = dealer.Rows[0]["ZHTime"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Memo", ItemValue = dealer.Rows[0]["Memo"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "License", ItemValue = dealer.Rows[0]["License"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Register", ItemValue = dealer.Rows[0]["EnterpriseRegistration"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "RegisterDate", ItemValue = dealer.Rows[0]["RegistrationDate"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "OriginateDate", ItemValue = dealer.Rows[0]["CreatDate"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Representative", ItemValue = dealer.Rows[0]["LegalRepresentative"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Card", ItemValue = dealer.Rows[0]["CorporateIdentityCard"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Capital", ItemValue = dealer.Rows[0]["RegisteredCapital"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "OpenBank", ItemValue = dealer.Rows[0]["BankBranch"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "AccountType", ItemValue = dealer.Rows[0]["AccountType"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "AccountName", ItemValue = dealer.Rows[0]["BankName"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Account", ItemValue = dealer.Rows[0]["BankAccount"].ToString() });
                dataList.Add(new DataItemParam { ItemName = "Couplet", ItemValue = dealer.Rows[0]["CoupletNumber"].ToString() });
                BPMServiceResult resul =  StartWorkflow("Prewaring", jsonData["originator"].ToString(), false, dataList);
                if (resul.Success)
                {
                    result.Result = "0";
                    result.Message = resul.Message;
                }
                else
                {
                    result.Result = "-1";
                    result.Message = resul.Message;
                }
            }
            else
            {
                result.Result = "-1";
                result.Message = "未找到经销商信息";
            }
        }
        catch (Exception e)
        {
            result.Result = "-1";
            result.Message = e.Message;
        }
        context.Response.Write(json.Serialize(result));
    }
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    public enum LetterEnum
    {
        FalseInfo=1,
        PlacesMortgage=2,
        Overdue=3
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
        var CurrentUserValidator = OThinker.H3.Controllers.UserValidatorFactory.GetUserValidator(Engine, UserCode);
        return CurrentUserValidator.UserID;
    }

    public string getBizobjectByInstanceid(string Instanceid)
    {
        string sql = "select bizobjectid from H3.OT_instancecontext where objectid='" + Instanceid + "'";
        return Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
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
        /// 启动H3流程实例
        /// </summary>
        /// <param name="workflowCode">流程模板编码</param>
        /// <param name="userCode">启动流程的用户编码</param>
        /// <param name="finishStart">是否结束第一个活动</param>
        /// <param name="paramValues">流程实例启动初始化数据项集合</param>
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
                        var t =  new List<OThinker.H3.DataModel.BizObject>();
                        foreach (List<DataItemParam> list in (IEnumerable)param.ItemValue)
                        {
                            var m = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, schema.Fields.FirstOrDefault(x => x.ChildSchemaCode==param.ItemName).Schema, user);
                            foreach (DataItemParam item in list)
                            {
                                if(m.Schema.ContainsField(item.ItemName)) m[item.ItemName] = item.ItemValue;
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
            string InstanceId = this.Engine.InstanceManager.CreateInstanceByDefault(bo.ObjectID,workflowTemplate.WorkflowCode,null,user);
            // 设置紧急程度为普通
            OThinker.H3.Messages.MessageEmergencyType emergency = OThinker.H3.Messages.MessageEmergencyType.Normal;
            // 这里也可以在启动流程的时候赋值
            Dictionary<string, object> paramTables = new Dictionary<string, object>();
            // 启动流程的消息
            OThinker.H3.Messages.StartInstanceMessage startInstanceMessage =
                new OThinker.H3.Messages.StartInstanceMessage(emergency, InstanceId, null,paramTables,
                    OThinker.H3.Instance.PriorityType.Normal, true, null, false, OThinker.H3.Instance.Token.UnspecifiedID, null);
            Engine.InstanceManager.SendMessage(startInstanceMessage);
            result = new BPMServiceResult(true, InstanceId, null, "流程实例启动成功！", "");
        }
        catch (Exception ex)
        {
            result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.Message);
        }
        return result;
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
        return Engine.WorkflowManager.GetPublishedTemplateHeader(workflowCode, workflowVersion);;
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
/// <summary>
/// 解密
/// </summary>
/// <param name="pToDecrypt">解密字符串</param>
/// <param name="sKey">密钥</param>
/// <returns></returns>
    public static string DESDecrypt(string pToDecrypt, string sKey = "test")
    {
        DESCryptoServiceProvider des = new DESCryptoServiceProvider();
        byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
        for (int x = 0; x < pToDecrypt.Length / 2; x++)
        {
            int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
            inputByteArray[x] = (byte)i;
        }
        des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
        des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
        MemoryStream ms = new MemoryStream();
        CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
        cs.Write(inputByteArray, 0, inputByteArray.Length);
        cs.FlushFinalBlock();
        //StringBuilder ret = new StringBuilder();
        return System.Web.HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
    }
}
