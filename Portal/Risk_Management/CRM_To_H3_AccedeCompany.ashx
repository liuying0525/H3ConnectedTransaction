<%@ WebHandler Language="C#" Class="CRM_To_H3_AccedeCompany" %>

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Antlr.Runtime.Misc;
using Aspose.Pdf;
using Newtonsoft.Json.Linq;
using OThinker.H3;
using OThinker.H3.Controllers;
using OThinker.H3.Instance;
/// <summary>
/// 入网接口
/// </summary>
public class CRM_To_H3_AccedeCompany : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public string DealerData = "INSERT INTO DEALERDATA(\"ObjectID\",\"CrmDealerId\",\"Name\",\"Type\",\"Value\",\"DealerType\",\"DealerKind\",\"DealerId\")VALUES([VALUES],[TYPEKIND])";
    public string DealerDataArray = "INSERT INTO DEALERDATAARRAY(\"ObjectID\",\"DealerDataObjectID\",\"Value\")VALUES([VALUES])";
    public void ProcessRequest(HttpContext context)
    {
        string data = "";
        using (StreamReader sr = new StreamReader(context.Request.InputStream))
        {
            data = sr.ReadToEnd();
            JObject jtoken = JObject.Parse(data);
            data = jtoken["data"].ToString();
        }
        AppUtility.Engine.LogWriter.Write("CRM_To_H3接口入网流程数据：" + data);
        JavaScriptSerializer json = new JavaScriptSerializer();
        WorkFlowMethod.Return result = new WorkFlowMethod.Return();
        try
        {
            List<DataItemParam> dataList = new List<DataItemParam>();
            List<string> sqlList = new List<string>();
            string objectid = "";
            data = DESDecrypt(data, "B^@s(d)+");
            AppUtility.Engine.LogWriter.Write("CRM_To_H3接口入网解密数据：" + data);
            JObject jsonData = JObject.Parse(data);
            string dealerid = jsonData["third_system_no"].ToString();
            objectid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM I_ALLOWIN WHERE CRMDEALERID='" + jsonData["third_system_no"] + "'") + string.Empty;
            if (string.IsNullOrEmpty(objectid))
            {
                string dealerType = "", dealerKind = "", crmid = "";
                dataList = GetDataItemParam(jsonData, out sqlList, out dealerType, out dealerKind, out crmid, dealerid, objectid);
                BPMServiceResult resul =  StartWorkflow("AllowIn", jsonData["originator"].ToString(), false, dataList);
                if (resul.Success)
                {
                    result.Result = "0";
                    result.Message = resul.Message;
                    foreach (string text in sqlList)
                    {
                        int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(text.Replace("[TYPEKIND]", "'" + dealerKind + "','" + dealerType + "','" + crmid + "'"));
                    }
                }
                else
                {
                    result.Result = "-1";
                    result.Message = resul.Message;
                }
            }
            else
            {
                string instanceId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID = '" + objectid + "'") + string.Empty;
                string workItemId = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_WORKITEM WHERE INSTANCEID = '" + instanceId + "'") + string.Empty;
                string dealerType = "", dealerKind = "", crmid = "";
                dataList = GetDataItemParam(jsonData, out sqlList, out dealerType, out dealerKind, out crmid, dealerid, objectid);
                BPMServiceResult re = SubmitItem("AllowIn", instanceId,workItemId, (OThinker.Data.BoolMatchValue)1, "", jsonData["originator"].ToString(),dataList);
                if (re.Success)
                {
                    int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM DEALERDATAARRAY WHERE \"DealerDataObjectID\" IN (SELECT \"ObjectID\" FROM DEALERDATA WHERE \"CrmDealerId\" = '" + dealerid + "')");
                    int j = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM DEALERDATA WHERE \"CrmDealerId\" = '" + dealerid + "'");
                    foreach (string text in sqlList)
                    {
                        int k = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(text.Replace("[TYPEKIND]", "'" + dealerKind + "','" + dealerType + "','" + crmid + "'"));
                    }
                    result.Result = "0";
                    result.Message = re.Message;
                }
                else
                {
                    result.Result = "-1";
                    result.Message = re.Message;
                }
            }
        }
        catch (Exception e)
        {
            result.Result = "-1";
            result.Message = e.Message;
        }
        context.Response.Write(json.Serialize(result));
    }
    /// <summary>
    /// 提交工作项
    /// </summary>
    /// <param name="workItemId">工作项ID</param>
    /// <param name="approval">审批结果</param>
    /// <param name="commentText">审批意见</param>
    /// <param name="userId">处理人</param>
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
                    var t =  new List<OThinker.H3.DataModel.BizObject>();
                    foreach (List<DataItemParam> list in (IEnumerable)value.ItemValue)
                    {
                        var m = new OThinker.H3.DataModel.BizObject(Engine.Organization, Engine.MetadataRepository, Engine.BizObjectManager, null, property.ChildSchema, bo.OwnerId);
                        foreach (DataItemParam dataItem in list)
                        {
                            if(m.Schema.ContainsField(dataItem.ItemName)) m.SetValue(dataItem.ItemName,dataItem.ItemValue);
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
            Engine.WorkItemManager.FinishWorkItem(item.ObjectID,userId,OThinker.H3.WorkItem.AccessPoint.ExternalSystem,null,approval,
                commentText,null,OThinker.H3.WorkItem.ActionEventType.Forward,(int)OThinker.H3.Controllers.SheetButtonType.Submit);
            // 需要通知实例事件管理器结束事件
            OThinker.H3.Messages.AsyncEndMessage endMessage = new OThinker.H3.Messages.AsyncEndMessage(OThinker.H3.Messages.MessageEmergencyType.Normal,
                item.InstanceId,item.ActivityCode,item.TokenId,approval,false,approval,true,null);
            Engine.InstanceManager.SendMessage(endMessage);
            result = new BPMServiceResult(true,"", null, "流程实例启动成功！", "");
        }
        catch (Exception ex)
        {
            result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.Message);
        }
        return result;
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
                                if(m.Schema.ContainsField(item.ItemName)) m.SetValue(item.ItemName,item.ItemValue);
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
            OThinker.H3.Messages.StartInstanceMessage startInstanceMessage = new OThinker.H3.Messages.StartInstanceMessage(emergency,
                InstanceId, null,paramTables,OThinker.H3.Instance.PriorityType.Normal, true, null, false,
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
    /// <summary>
    /// json数据项转List
    /// </summary>
    /// <param name="jsonData">json数据</param>
    /// <param name="sqlList">list数据</param>
    /// <param name="dealertype">经销商分类</param>
    /// <param name="dealerkind">渠道分类</param>
    /// <param name="dealerid">经销商ID</param>
    /// <param name="objectid">流程ID</param>
    /// <returns></returns>
    public List<DataItemParam> GetDataItemParam(JObject jsonData, out List<string> sqlList, out string dealertype, out string dealerkind, out string crmid, string dealerid, string objectid)
    {
        dealerkind = "";
        dealertype = "";
        crmid = "";
        string city = "";
        sqlList = new List<string>();
        List<DataItemParam> dataList = new List<DataItemParam>();
        dataList.Add(new DataItemParam { ItemName = "CrmDealerId", ItemValue = jsonData["third_system_no"].ToString() });
        foreach (var item in jsonData["accedecontent"])
        {
            switch (item["code"].ToString())
            {
                case "manager": dataList.Add(new DataItemParam { ItemName = "Manager", ItemValue = item["value"].ToString() });break;
                case "distributor": dataList.Add(new DataItemParam { ItemName = "Distributor", ItemValue = item["value"].ToString() });break;
                case "type":dealerkind = item["value"].ToString(); dataList.Add(new DataItemParam { ItemName = "Type", ItemValue = item["value"].ToString() }); break;
                case "distributortype":dealertype = item["value"].ToString(); dataList.Add(new DataItemParam { ItemName = "DistributorType", ItemValue = item["value"].ToString() }); break;
                case "province": dataList.Add(new DataItemParam { ItemName = "Province", ItemValue = item["value"].ToString() }); break;
                case "city": city = item["value"].ToString(); dataList.Add(new DataItemParam { ItemName = "City", ItemValue = item["value"].ToString() }); break;
                case "companyaddr": dataList.Add(new DataItemParam { ItemName = "CompanyAddr", ItemValue = item["value"].ToString() }); break;
                case "belongto": dataList.Add(new DataItemParam { ItemName = "BelongTo", ItemValue = item["value"].ToString() }); break;
                case "brand": dataList.Add(new DataItemParam { ItemName = "Brand", ItemValue = item["value"].ToString() }); break;
                case "qywykt": dataList.Add(new DataItemParam { ItemName = "QYWYKT", ItemValue = item["value"].ToString() }); break;
                case "loantype": dataList.Add(new DataItemParam { ItemName = "LoanType", ItemValue = item["value"].ToString() }); break;
                case "memo": dataList.Add(new DataItemParam { ItemName = "Memo", ItemValue = item["value"].ToString() }); break;
                case "license": dataList.Add(new DataItemParam { ItemName = "License", ItemValue = item["value"].ToString() }); break;
                case "enterpriseregistration": dataList.Add(new DataItemParam { ItemName = "EnterpriseRegistration", ItemValue = item["value"].ToString() }); break;
                case "registrationdate": dataList.Add(new DataItemParam { ItemName = "RegistrationDate", ItemValue = item["value"].ToString() }); break;
                case "creatdate": dataList.Add(new DataItemParam { ItemName = "CreatDate", ItemValue = item["value"].ToString() });break;
                case "legalrepresentative": dataList.Add(new DataItemParam { ItemName = "LegalRepresentative", ItemValue = item["value"].ToString() }); break;
                case "corporateidentitycard": dataList.Add(new DataItemParam { ItemName = "CorporateIdentityCard", ItemValue = item["value"].ToString() }); break;
                case "registeredcapital": dataList.Add(new DataItemParam { ItemName = "RegisteredCapital", ItemValue = item["value"].ToString() }); break;
                case "bankbranch": dataList.Add(new DataItemParam { ItemName = "BankBranch", ItemValue = item["value"].ToString() });break;
                case "accounttype": dataList.Add(new DataItemParam { ItemName = "AccountType", ItemValue = item["value"].ToString() }); break;
                case "bankname": dataList.Add(new DataItemParam { ItemName = "BankName", ItemValue = item["value"].ToString() }); break;
                case "bankaccount": dataList.Add(new DataItemParam { ItemName = "BankAccount", ItemValue = item["value"].ToString() }); break;
                case "coupletnumber": dataList.Add(new DataItemParam { ItemName = "CoupletNumber", ItemValue = item["value"].ToString() }); break;
                case "contactinformation":
                    List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();
                    foreach (var contact in item["value"])
                    {
                        chileList.Add(new List<DataItemParam>()
                        {
                            new DataItemParam{ ItemName = "Department", ItemValue = contact["position"].ToString() },
                            new DataItemParam{ ItemName = "People", ItemValue = contact["name"].ToString() },
                            new DataItemParam{ ItemName = "Tel", ItemValue = contact["code"].ToString() },
                            new DataItemParam{ ItemName = "Maile", ItemValue = contact["value"].ToString() }
                        });
                    }
                    dataList.Add(new DataItemParam { ItemName = "ContactInformation", ItemValue = chileList });
                    break;
                case "gpsinstallation": dataList.Add(new DataItemParam { ItemName = "GPSInstallation", ItemValue = item["value"].ToString() }); break;
                case "系统评分": dataList.Add(new DataItemParam { ItemName = "SystemScore", ItemValue = item["value"].ToString() }); break;
                case "gpsaddr": dataList.Add(new DataItemParam { ItemName = "GPSAddr", ItemValue = item["value"].ToString() });break;
                case "gpsaccount": dataList.Add(new DataItemParam { ItemName = "GPSAccount", ItemValue = item["value"].ToString() });break;
                case "gpspassword": dataList.Add(new DataItemParam { ItemName = "GPSPassword", ItemValue = item["value"].ToString() }); break;
                case "approvalform":dataList.Add(new DataItemParam { ItemName = "ApprovalForm", ItemValue = item["value"].ToString() });break;
                case "collectiontable":dataList.Add(new DataItemParam { ItemName = "CollectionTable", ItemValue = item["value"].ToString() }); break;
                case "businesslicense": dataList.Add(new DataItemParam { ItemName = "BusinessLicense", ItemValue = item["value"].ToString() }); break;
                case "creditdocuments": dataList.Add(new DataItemParam { ItemName = "CreditDocuments", ItemValue = item["value"].ToString() }); break;
                case "authorizationdocument": dataList.Add(new DataItemParam { ItemName = "AuthorizationDocument", ItemValue = item["value"].ToString() }); break;
                case "photo":dataList.Add(new DataItemParam { ItemName = "Photo", ItemValue = item["value"].ToString() });break;
                case "discountagreement": dataList.Add(new DataItemParam { ItemName = "DiscountAgreement", ItemValue = item["value"].ToString() }); break;
                case "cargoagreement": dataList.Add(new DataItemParam { ItemName = "CargoAgreement", ItemValue = item["value"].ToString() }); break;
                case "hallphoto": dataList.Add(new DataItemParam { ItemName = "HallPhoto", ItemValue = item["value"].ToString() });break;
                case "officephoto": dataList.Add(new DataItemParam { ItemName = "OfficePhoto", ItemValue = item["value"].ToString() }); break;
                case "businessinformation": dataList.Add(new DataItemParam { ItemName = "BusinessInformation", ItemValue = item["value"].ToString() });break;
                case "financialinformation": dataList.Add(new DataItemParam { ItemName = "FinancialInformation", ItemValue = item["value"].ToString() }); break;
                case "金融专员身份证复印件": dataList.Add(new DataItemParam { ItemName = "FinancialId", ItemValue = item["value"].ToString() }); break;
                case "cooperationagreement": dataList.Add(new DataItemParam { ItemName = "CooperationAgreement", ItemValue = item["value"].ToString() }); break;
                /*** mod start by luoji 2018/12/27 16:18
                 * source
                case "代表/实控人(身份证复印件)": dataList.Add(new DataItemParam { ItemName = "RepresentativeId", ItemValue = item["value"].ToString() }); break;
                 * mod ***/
                case "files11": dataList.Add(new DataItemParam { ItemName = "RepresentativeId", ItemValue = item["value"].ToString() }); break;
                /*** mod end ***/
                case "personnelphoto": dataList.Add(new DataItemParam { ItemName = "PersonnelPhoto", ItemValue = item["value"].ToString() }); break;
                case "report": dataList.Add(new DataItemParam { ItemName = "Report", ItemValue = item["value"].ToString() }); break;
                case "sitecontract": dataList.Add(new DataItemParam { ItemName = "SiteContract", ItemValue = item["value"].ToString() }); break;
                case "xfhdspb": dataList.Add(new DataItemParam { ItemName = "XFHDSPB", ItemValue = item["value"].ToString() }); break;
                case "quotaform": dataList.Add(new DataItemParam { ItemName = "QuotaForm", ItemValue = item["value"].ToString() }); break;
                case "salesdata":dataList.Add(new DataItemParam { ItemName = "SalesData", ItemValue = item["value"].ToString() }); break;
                case "financialinstitutions": dataList.Add(new DataItemParam { ItemName = "FinancialInstitutions", ItemValue = item["value"].ToString() }); break;
                case "bankflow":  dataList.Add(new DataItemParam { ItemName = "BankFlow", ItemValue = item["value"].ToString() }); break;
                case "personalinformation":  dataList.Add(new DataItemParam { ItemName = "PersonalInformation", ItemValue = item["value"].ToString() }); break;
                case "securityagreement":  dataList.Add(new DataItemParam { ItemName = "SecurityAgreement", ItemValue = item["value"].ToString() }); break;
                case "creditreport":  dataList.Add(new DataItemParam { ItemName = "CreditReport", ItemValue = item["value"].ToString() }); break;
                case "qycreditreport":  dataList.Add(new DataItemParam { ItemName = "QYCreditReport", ItemValue = item["value"].ToString() }); break;
                case "qualificationinformation":  dataList.Add(new DataItemParam { ItemName = "QualificationInformation", ItemValue = item["value"].ToString() }); break;
                case "total":   dataList.Add(new DataItemParam { ItemName = "Total", ItemValue = item["value"].ToString() }); break;
                case "shareratio":  dataList.Add(new DataItemParam { ItemName = "ShareRatio", ItemValue = item["value"].ToString() }); break;
                case "area":  dataList.Add(new DataItemParam { ItemName = "Area", ItemValue = item["value"].ToString() }); break;
                case "financialdepartment":  dataList.Add(new DataItemParam { ItemName = "FinancialDepartment", ItemValue = item["value"].ToString() }); break;
                case "mortgageconfirmation":  dataList.Add(new DataItemParam { ItemName = "MortgageConfirmation", ItemValue = item["value"].ToString() }); break;
                case "xsqk":  dataList.Add(new DataItemParam { ItemName = "XSQK", ItemValue = item["value"].ToString() }); break;
                case "zxqk": dataList.Add(new DataItemParam { ItemName = "ZXQK", ItemValue = item["value"].ToString() }); break;
                case "sxqk": dataList.Add(new DataItemParam { ItemName = "SXQK", ItemValue = item["value"].ToString() }); break;
                case "businesslife": dataList.Add(new DataItemParam { ItemName = "BusinessLife", ItemValue = item["value"].ToString() }); break;
                case "seniority": dataList.Add(new DataItemParam { ItemName = "Seniority", ItemValue = item["value"].ToString() }); break;
                case "administrative": dataList.Add(new DataItemParam { ItemName = "Administrative", ItemValue = item["value"].ToString() }); break;
                case "lease":  dataList.Add(new DataItemParam { ItemName = "Lease", ItemValue = item["value"].ToString() }); break;
                case "sixbankflow":  dataList.Add(new DataItemParam { ItemName = "SixBankFlow", ItemValue = item["value"].ToString() }); break;
                case "profits":  dataList.Add(new DataItemParam { ItemName = "Profits", ItemValue = item["value"].ToString() }); break;
                case "isbank":  dataList.Add(new DataItemParam { ItemName = "IsBank", ItemValue = item["value"].ToString() }); break;
                case "queryrecord":  dataList.Add(new DataItemParam { ItemName = "QueryRecord", ItemValue = item["value"].ToString() }); break;
                case "legalperson":  dataList.Add(new DataItemParam { ItemName = "LegalPerson", ItemValue = item["value"].ToString() }); break;
                case "financialcount":  dataList.Add(new DataItemParam { ItemName = "FinancialCount", ItemValue = item["value"].ToString() }); break;
                case "salescount":  dataList.Add(new DataItemParam { ItemName = "SalesCount", ItemValue = item["value"].ToString() }); break;
                case "advanceloan":  dataList.Add(new DataItemParam { ItemName = "AdvanceLoan", ItemValue = item["value"].ToString() }); break;
                case "financialcode": dataList.Add(new DataItemParam { ItemName = "FinancialCode", ItemValue = item["value"].ToString() }); crmid = item["value"].ToString(); break;
            }
            switch (item["type"].ToString())
            {
                case "文本":
                    string text1 = DealerData.Replace("[VALUES]","'" + Guid.NewGuid() + "','" + dealerid + "','" + item["name"] + "','Single','" +item["value"] + "'");
                    sqlList.Add(text1);
                    break;
                case "文件":
                    string dealerobjectid1 = Guid.NewGuid().ToString();
                    string text2 = DealerData.Replace("[VALUES]","'" + dealerobjectid1 + "','" + dealerid + "','" + item["name"] +"','Array','" + item["value"] + "'");
                    sqlList.Add(text2);
                    foreach (var s in item["value"].ToString().Split('<'))
                    {
                        if (!string.IsNullOrEmpty(s))
                        {
                            string text3 = DealerDataArray.Replace("[VALUES]","'" + Guid.NewGuid() + "','" + dealerobjectid1 + "','" + s + "'");
                            sqlList.Add(text3);
                        }
                    }
                    break;
                case "文本数组":
                    string dealerobjectid2 = Guid.NewGuid().ToString();
                    string text4 = DealerData.Replace("[VALUES]","'" + dealerobjectid2 + "','" + dealerid + "','" + item["name"] +"','Array','" + "'");
                    sqlList.Add(text4);
                    string text5 = "";
                    foreach (var contact in item["value"])
                    {
                        text5 = contact["name"] + ">" + contact["value"];
                        text5 = DealerDataArray.Replace("[VALUES]","'" + Guid.NewGuid() + "','" + dealerobjectid2 + "','" +text5+ "'");
                        sqlList.Add(text5);
                    }
                    break;
            }
            if(!string.IsNullOrWhiteSpace(city))
            {
                city = city.Replace("市", "");
                string citylevel = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT Category FROM OT_EnumerableMetadata WHERE CODE LIKE '%" + city +"%'") + string.Empty;
                if (string.IsNullOrWhiteSpace(citylevel)) citylevel = "六级";
                string text = DealerData.Replace("[VALUES]","'" + Guid.NewGuid() + "','" + dealerid + "','城市等级','Single','" + citylevel + "'");
                sqlList.Add(text);
                city = "";
            }
        }
        return dataList;
    }
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
    public Authentication authentication = new Authentication("administrator", "shdzretail1");
    public OThinker.H3.Controllers.UserValidator UserValidator = null;
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
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}