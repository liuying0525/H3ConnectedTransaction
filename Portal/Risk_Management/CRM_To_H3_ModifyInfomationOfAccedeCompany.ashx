<%@ WebHandler Language="C#" Class="CRM_To_H3_ModifyInfomationOfAccedeCompany" %>

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
/// 入网修改接口
/// </summary>
public class CRM_To_H3_ModifyInfomationOfAccedeCompany : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        string data = "";
        using (StreamReader sr = new StreamReader(context.Request.InputStream))
        {
            data = sr.ReadToEnd();
            JObject jtoken = JObject.Parse(data);
            data = jtoken["data"].ToString();
        }
        AppUtility.Engine.LogWriter.Write("CRM_To_H3接口修改入网流程数据：" + data);
        JavaScriptSerializer json = new JavaScriptSerializer();
        WorkFlowMethod.Return result = new WorkFlowMethod.Return();
        try
        {
            List<DataItemParam> dataList = new List<DataItemParam>();
            data = DESDecrypt(data, "B^@s(d)+");
            AppUtility.Engine.LogWriter.Write("CRM_To_H3接口修改入网解密数据：" + data);
            JObject jsonData = JObject.Parse(data);
            string crmid = "";
            dataList = GetDataItemParam(jsonData, out crmid);//2019-04-15
            //2019-04-18int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("UPDATE I_ALLOWIN SET FINANCIALCODE = '" + crmid + "' WHERE CRMDEALERID = '" + jsonData["third_system_no"].ToString() + "'");
            string objectid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM I_ALLOWIN WHERE CRMDEALERID='" + jsonData["third_system_no"] + "'")+string.Empty;
            string instanceid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_INSTANCECONTEXT WHERE BIZOBJECTID='" + objectid + "'") + string.Empty;
            OThinker.H3.Instance.InstanceData instanceData = new OThinker.H3.Instance.InstanceData(OThinker.H3.Controllers.AppUtility.Engine, instanceid, null);
            if (instanceData != null && instanceData.BizObject.ValueTable.Count > 0)
            {
                if(jsonData["accedecontent"].ToString().IndexOf("Manager".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Manager", ItemValue = instanceData.BizObject.ValueTable["Manager"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Distributor".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Distributor", ItemValue = instanceData.BizObject.ValueTable["Distributor"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Type".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Type", ItemValue = instanceData.BizObject.ValueTable["Type"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("DistributorType".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "DistributorType", ItemValue = instanceData.BizObject.ValueTable["DistributorType"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Province".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Province", ItemValue = instanceData.BizObject.ValueTable["Province"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("City".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "City", ItemValue = instanceData.BizObject.ValueTable["City"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CompanyAddr".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CompanyAddr", ItemValue = instanceData.BizObject.ValueTable["CompanyAddr"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BelongTo".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BelongTo", ItemValue = instanceData.BizObject.ValueTable["BelongTo"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Brand".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Brand", ItemValue = instanceData.BizObject.ValueTable["Brand"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("QYWYKT".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "QYWYKT", ItemValue = instanceData.BizObject.ValueTable["QYWYKT"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("LoanType".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "LoanType", ItemValue = instanceData.BizObject.ValueTable["LoanType"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Memo".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Memo", ItemValue = instanceData.BizObject.ValueTable["Memo"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("License".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "License", ItemValue = instanceData.BizObject.ValueTable["License"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("EnterpriseRegistration".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "EnterpriseRegistration", ItemValue = instanceData.BizObject.ValueTable["EnterpriseRegistration"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("RegistrationDate".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "RegistrationDate", ItemValue = instanceData.BizObject.ValueTable["RegistrationDate"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CreatDate".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CreatDate", ItemValue = instanceData.BizObject.ValueTable["CreatDate"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("LegalRepresentative".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "LegalRepresentative", ItemValue = instanceData.BizObject.ValueTable["LegalRepresentative"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CorporateIdentityCard".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CorporateIdentityCard", ItemValue = instanceData.BizObject.ValueTable["CorporateIdentityCard"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("RegisteredCapital".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "RegisteredCapital", ItemValue = instanceData.BizObject.ValueTable["RegisteredCapital"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BankBranch".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BankBranch", ItemValue = instanceData.BizObject.ValueTable["BankBranch"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("AccountType".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "AccountType", ItemValue = instanceData.BizObject.ValueTable["AccountType"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BankName".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BankName", ItemValue = instanceData.BizObject.ValueTable["BankName"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BankAccount".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BankAccount", ItemValue = instanceData.BizObject.ValueTable["BankAccount"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CoupletNumber".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CoupletNumber", ItemValue = instanceData.BizObject.ValueTable["CoupletNumber"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("GPSInstallation".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "GPSInstallation", ItemValue = instanceData.BizObject.ValueTable["GPSInstallation"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("SixBankFlow".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "SixBankFlow", ItemValue = instanceData.BizObject.ValueTable["SixBankFlow"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("Profits".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Profits", ItemValue = instanceData.BizObject.ValueTable["Profits"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("IsBank".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "IsBank", ItemValue = instanceData.BizObject.ValueTable["IsBank"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("SystemScore".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "SystemScore", ItemValue = instanceData.BizObject.ValueTable["SystemScore"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("GPSAddr".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "GPSAddr", ItemValue = instanceData.BizObject.ValueTable["GPSAddr"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("GPSAccount".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "GPSAccount", ItemValue = instanceData.BizObject.ValueTable["GPSAccount"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("GPSPassword".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "GPSPassword", ItemValue = instanceData.BizObject.ValueTable["GPSPassword"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("ApprovalForm".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "ApprovalForm", ItemValue = instanceData.BizObject.ValueTable["ApprovalForm"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CollectionTable".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CollectionTable", ItemValue = instanceData.BizObject.ValueTable["CollectionTable"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BusinessLicense".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BusinessLicense", ItemValue = instanceData.BizObject.ValueTable["BusinessLicense"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CreditDocuments".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CreditDocuments", ItemValue = instanceData.BizObject.ValueTable["CreditDocuments"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("AuthorizationDocument".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "AuthorizationDocument", ItemValue = instanceData.BizObject.ValueTable["AuthorizationDocument"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Photo".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Photo", ItemValue = instanceData.BizObject.ValueTable["Photo"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("DiscountAgreement".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "DiscountAgreement", ItemValue = instanceData.BizObject.ValueTable["DiscountAgreement"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CargoAgreement".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CargoAgreement", ItemValue = instanceData.BizObject.ValueTable["CargoAgreement"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("HallPhoto".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "HallPhoto", ItemValue = instanceData.BizObject.ValueTable["HallPhoto"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("OfficePhoto".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "OfficePhoto", ItemValue = instanceData.BizObject.ValueTable["OfficePhoto"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BusinessInformation".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BusinessInformation", ItemValue = instanceData.BizObject.ValueTable["BusinessInformation"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("FinancialInformation".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "FinancialInformation", ItemValue = instanceData.BizObject.ValueTable["FinancialInformation"] + string.Empty });
                //if(jsonData["accedecontent"].ToString().IndexOf("".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "FinancialId", ItemValue = instanceData.BizObject.ValueTable["FinancialId"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CooperationAgreement".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CooperationAgreement", ItemValue = instanceData.BizObject.ValueTable["CooperationAgreement"] + string.Empty });
                //if(jsonData["accedecontent"].ToString().IndexOf("".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "RepresentativeId", ItemValue = instanceData.BizObject.ValueTable["RepresentativeId"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("PersonnelPhoto".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "PersonnelPhoto", ItemValue = instanceData.BizObject.ValueTable["PersonnelPhoto"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("Report".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Report", ItemValue = instanceData.BizObject.ValueTable["Report"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("SiteContract".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "SiteContract", ItemValue = instanceData.BizObject.ValueTable["SiteContract"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("XFHDSPB".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "XFHDSPB", ItemValue = instanceData.BizObject.ValueTable["XFHDSPB"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("QuotaForm".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "QuotaForm", ItemValue = instanceData.BizObject.ValueTable["QuotaForm"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("SalesData".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "SalesData", ItemValue = instanceData.BizObject.ValueTable["SalesData"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("FinancialInstitutions".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "FinancialInstitutions", ItemValue = instanceData.BizObject.ValueTable["FinancialInstitutions"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("BankFlow".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BankFlow", ItemValue = instanceData.BizObject.ValueTable["BankFlow"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("PersonalInformation".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "PersonalInformation", ItemValue = instanceData.BizObject.ValueTable["PersonalInformation"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("SecurityAgreement".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "SecurityAgreement", ItemValue = instanceData.BizObject.ValueTable["SecurityAgreement"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("CreditReport".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "CreditReport", ItemValue = instanceData.BizObject.ValueTable["CreditReport"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("QYCreditReport".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "QYCreditReport", ItemValue = instanceData.BizObject.ValueTable["QYCreditReport"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("QualificationInformation".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "QualificationInformation", ItemValue = instanceData.BizObject.ValueTable["QualificationInformation"] + string.Empty });

                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("Total".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Total", ItemValue = instanceData.BizObject.ValueTable["Total"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("ShareRatio".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "ShareRatio", ItemValue = instanceData.BizObject.ValueTable["ShareRatio"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("Area".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Area", ItemValue = instanceData.BizObject.ValueTable["Area"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("FinancialDepartment".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "FinancialDepartment", ItemValue = instanceData.BizObject.ValueTable["FinancialDepartment"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("MortgageConfirmation".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "MortgageConfirmation", ItemValue = instanceData.BizObject.ValueTable["MortgageConfirmation"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("XSQK".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "XSQK", ItemValue = instanceData.BizObject.ValueTable["XSQK"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("ZXQK".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "ZXQK", ItemValue = instanceData.BizObject.ValueTable["ZXQK"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("SXQK".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "SXQK", ItemValue = instanceData.BizObject.ValueTable["SXQK"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("BusinessLife".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "BusinessLife", ItemValue = instanceData.BizObject.ValueTable["BusinessLife"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("Seniority".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Seniority", ItemValue = instanceData.BizObject.ValueTable["Seniority"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("Administrative".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Administrative", ItemValue = instanceData.BizObject.ValueTable["Administrative"] + string.Empty });
                //2019-04-15if(jsonData["accedecontent"].ToString().IndexOf("Lease".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "Lease", ItemValue = instanceData.BizObject.ValueTable["Lease"] + string.Empty });

                if (jsonData["accedecontent"].ToString().IndexOf("QueryRecord".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "QueryRecord", ItemValue = instanceData.BizObject.ValueTable["QueryRecord"] + string.Empty });
                if(jsonData["accedecontent"].ToString().IndexOf("LegalPerson".ToLower()) < 0) dataList.Add(new DataItemParam { ItemName = "LegalPerson", ItemValue = instanceData.BizObject.ValueTable["LegalPerson"] + string.Empty });
                BPMServiceResult resul = StartWorkflow("UpdateEnterNet", jsonData["originator"].ToString(), false, dataList);
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
    /// json数据项转List
    /// </summary>
    /// <param name="jsonData">json数据</param>
    /// <returns></returns>
    public List<DataItemParam> GetDataItemParam(JObject jsonData, out string crmid)
    {
        List<DataItemParam> dataList = new List<DataItemParam>();
        crmid = "";
        string changeField = "";
        dataList.Add(new DataItemParam { ItemName = "CrmDealerId", ItemValue = jsonData["third_system_no"].ToString() });
        foreach (var item in jsonData["accedecontent"])
        {
            switch (item["code"].ToString())
            {
                case "manager": dataList.Add(new DataItemParam { ItemName = "Manager", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "Manager" : ",Manager") + ":" + item["name"];break;
                case "distributor": dataList.Add(new DataItemParam { ItemName = "Distributor", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "Distributor" : ",Distributor") + ":" + item["name"];break;
                case "type":dataList.Add(new DataItemParam { ItemName = "Type", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Type" : ",Type") + ":" + item["name"];break;
                case "distributortype": dataList.Add(new DataItemParam { ItemName = "DistributorType", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "DistributorType" : ",DistributorType") + ":" + item["name"];break;
                case "province": dataList.Add(new DataItemParam { ItemName = "Province", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Province" : ",Province") + ":" + item["name"];break;
                case "city": dataList.Add(new DataItemParam { ItemName = "City", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "City" : "City,") + ":" + item["name"];break;
                case "companyaddr": dataList.Add(new DataItemParam { ItemName = "CompanyAddr", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CompanyAddr" : ",CompanyAddr") + ":" + item["name"];break;
                case "belongto": dataList.Add(new DataItemParam { ItemName = "BelongTo", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "BelongTo" : ",BelongTo") + ":" + item["name"];break;
                case "brand": dataList.Add(new DataItemParam { ItemName = "Brand", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Brand" : ",Brand") + ":" + item["name"];break;
                case "qywykt": dataList.Add(new DataItemParam { ItemName = "QYWYKT", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "QYWYKT" : ",QYWYKT") + ":" + item["name"];break;
                case "loantype": dataList.Add(new DataItemParam { ItemName = "LoanType", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "LoanType" : ",LoanType") + ":" + item["name"];break;
                case "memo": dataList.Add(new DataItemParam { ItemName = "Memo", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Memo" : ",Memo") + ":" + item["name"];break;
                case "license": dataList.Add(new DataItemParam { ItemName = "License", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "License" : ",License") + ":" + item["name"];break;
                case "enterpriseregistration": dataList.Add(new DataItemParam { ItemName = "EnterpriseRegistration", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "EnterpriseRegistration" : ",EnterpriseRegistration") + ":" + item["name"];break;
                case "registrationdate": dataList.Add(new DataItemParam { ItemName = "RegistrationDate", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "RegistrationDate" : ",RegistrationDate") + ":" + item["name"];break;
                case "creatdate": dataList.Add(new DataItemParam { ItemName = "CreatDate", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "CreatDate" : ",CreatDate") + ":" + item["name"];break;
                case "legalrepresentative": dataList.Add(new DataItemParam { ItemName = "LegalRepresentative", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "LegalRepresentative" : ",LegalRepresentative") + ":" + item["name"];break;
                case "corporateidentitycard": dataList.Add(new DataItemParam { ItemName = "CorporateIdentityCard", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CorporateIdentityCard" : ",CorporateIdentityCard") + ":" + item["name"];break;
                case "registeredcapital": dataList.Add(new DataItemParam { ItemName = "RegisteredCapital", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "RegisteredCapital" : ",RegisteredCapital") + ":" + item["name"];break;
                case "bankbranch": dataList.Add(new DataItemParam { ItemName = "BankBranch", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "BankBranch" : ",BankBranch") + ":" + item["name"];break;
                case "accounttype": dataList.Add(new DataItemParam { ItemName = "AccountType", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "AccountType" : ",AccountType") + ":" + item["name"];break;
                case "bankname": dataList.Add(new DataItemParam { ItemName = "BankName", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "BankName" : ",BankName") + ":" + item["name"];break;
                case "bankaccount": dataList.Add(new DataItemParam { ItemName = "BankAccount", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "BankAccount" : ",BankAccount") + ":" + item["name"];break;
                case "coupletnumber": dataList.Add(new DataItemParam { ItemName = "CoupletNumber", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CoupletNumber" : ",CoupletNumber") + ":" + item["name"];break;
                case "contactinformation":
                    List<List<DataItemParam>> chileList = new List<List<DataItemParam>>();
                    foreach (var contact in item["value"])
                    {
                        chileList.Add(new List<DataItemParam>()
                        {
                            new DataItemParam{ ItemName = "Dept", ItemValue = contact["position"].ToString() },
                            new DataItemParam{ ItemName = "Contact", ItemValue = contact["name"].ToString() },
                            new DataItemParam{ ItemName = "Phone", ItemValue = contact["code"].ToString() },
                            new DataItemParam{ ItemName = "Email", ItemValue = contact["value"].ToString() }
                        });
                    }
                    dataList.Add(new DataItemParam { ItemName = "UpdateContactInformation", ItemValue = chileList });changeField += (changeField == "" ? "UpdateContactInformation" : ",UpdateContactInformation") + ":" + item["name"];
                    break;
                case "gpsinstallation": dataList.Add(new DataItemParam { ItemName = "GPSInstallation", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "GPSInstallation" : ",GPSInstallation") + ":" + item["name"];break;
                case "系统评分": dataList.Add(new DataItemParam { ItemName = "SystemScore", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "SystemScore" : ",SystemScore") + ":" + item["name"];break;
                case "gpsaddr": dataList.Add(new DataItemParam { ItemName = "GPSAddr", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "GPSAddr" : ",GPSAddr") + ":" + item["name"];break;
                case "gpsaccount": dataList.Add(new DataItemParam { ItemName = "GPSAccount", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "GPSAccount" : ",GPSAccount") + ":" + item["name"];break;
                case "gpspassword": dataList.Add(new DataItemParam { ItemName = "GPSPassword", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "GPSPassword" : ",GPSPassword") + ":" + item["name"];break;
                case "approvalform":dataList.Add(new DataItemParam { ItemName = "ApprovalForm", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "ApprovalForm" : ",ApprovalForm") + ":" + item["name"];break;
                case "collectiontable":dataList.Add(new DataItemParam { ItemName = "CollectionTable", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CollectionTable" : ",CollectionTable") + ":" + item["name"];break;
                case "businesslicense": dataList.Add(new DataItemParam { ItemName = "BusinessLicense", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "BusinessLicense" : ",BusinessLicense") + ":" + item["name"];break;
                case "creditdocuments": dataList.Add(new DataItemParam { ItemName = "CreditDocuments", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CreditDocuments" : ",CreditDocuments") + ":" + item["name"];break;
                case "authorizationdocument": dataList.Add(new DataItemParam { ItemName = "AuthorizationDocument", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "AuthorizationDocument" : ",AuthorizationDocument") + ":" + item["name"]; break;
                case "photo":dataList.Add(new DataItemParam { ItemName = "Photo", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "Photo" : ",Photo") + ":" + item["name"];break;
                case "discountagreement": dataList.Add(new DataItemParam { ItemName = "DiscountAgreement", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "DiscountAgreement" : ",DiscountAgreement") + ":" + item["name"]; break;
                case "cargoagreement": dataList.Add(new DataItemParam { ItemName = "CargoAgreement", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CargoAgreement" : ",CargoAgreement") + ":" + item["name"];break;
                case "hallphoto": dataList.Add(new DataItemParam { ItemName = "HallPhoto", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "HallPhoto" : ",HallPhoto") + ":" + item["name"];break;
                case "officephoto": dataList.Add(new DataItemParam { ItemName = "OfficePhoto", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "OfficePhoto" : ",OfficePhoto") + ":" + item["name"];break;
                case "businessinformation": dataList.Add(new DataItemParam { ItemName = "BusinessInformation", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "BusinessInformation" : ",BusinessInformation") + ":" + item["name"];break;
                case "financialinformation": dataList.Add(new DataItemParam { ItemName = "FinancialInformation", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "FinancialInformation" : ",FinancialInformation") + ":" + item["name"];break;
                case "金融专员身份证复印件": dataList.Add(new DataItemParam { ItemName = "FinancialId", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "FinancialId" : ",FinancialId") + ":" + item["name"];break;
                case "cooperationagreement": dataList.Add(new DataItemParam { ItemName = "CooperationAgreement", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "CooperationAgreement" : ",CooperationAgreement") + ":" + item["name"];break;
                case "代表/实控人(身份证复印件)": dataList.Add(new DataItemParam { ItemName = "RepresentativeId", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "RepresentativeId" : ",RepresentativeId") + ":" + item["name"];break;
                case "personnelphoto": dataList.Add(new DataItemParam { ItemName = "PersonnelPhoto", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "PersonnelPhoto" : ",PersonnelPhoto") + ":" + item["name"]; break;
                case "report": dataList.Add(new DataItemParam { ItemName = "Report", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Report" : ",Report") + ":" + item["name"];break;
                case "sitecontract": dataList.Add(new DataItemParam { ItemName = "SiteContract", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "SiteContract" : ",SiteContract") + ":" + item["name"];break;
                case "xfhdspb": dataList.Add(new DataItemParam { ItemName = "XFHDSPB", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "XFHDSPB" : ",XFHDSPB") + ":" + item["name"];break;
                case "quotaform": dataList.Add(new DataItemParam { ItemName = "QuotaForm", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "QuotaForm" : ",QuotaForm") + ":" + item["name"];break;
                case "salesdata":dataList.Add(new DataItemParam { ItemName = "SalesData", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "SalesData" : ",SalesData") + ":" + item["name"];break;
                case "financialinstitutions": dataList.Add(new DataItemParam { ItemName = "FinancialInstitutions", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "FinancialInstitutions" : ",FinancialInstitutions") + ":" + item["name"]; break;
                case "bankflow":  dataList.Add(new DataItemParam { ItemName = "BankFlow", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "BankFlow" : ",BankFlow") + ":" + item["name"];break;
                case "personalinformation":  dataList.Add(new DataItemParam { ItemName = "PersonalInformation", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "PersonalInformation" : ",PersonalInformation") + ":" + item["name"]; break;
                case "securityagreement":  dataList.Add(new DataItemParam { ItemName = "SecurityAgreement", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "SecurityAgreement" : ",SecurityAgreement") + ":" + item["name"]; break;
                case "creditreport":  dataList.Add(new DataItemParam { ItemName = "CreditReport", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "CreditReport" : ",CreditReport") + ":" + item["name"]; break;
                case "qycreditreport":  dataList.Add(new DataItemParam { ItemName = "QYCreditReport", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "QYCreditReport" : ",QYCreditReport") + ":" + item["name"]; break;
                case "qualificationinformation":  dataList.Add(new DataItemParam { ItemName = "QualificationInformation", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "QualificationInformation" : ",QualificationInformation") + ":" + item["name"]; break;

                //2019-04-15case "total": dataList.Add(new DataItemParam { ItemName = "Total", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Total" : ",Total") + ":" + item["name"];break;
                //2019-04-15case "shareratio": dataList.Add(new DataItemParam { ItemName = "ShareRatio", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "ShareRatio" : ",ShareRatio") + ":" + item["name"];break;
                //2019-04-15case "area": dataList.Add(new DataItemParam { ItemName = "Area", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Area" : ",Area") + ":" + item["name"];break;
                //2019-04-15case "financialdepartment": dataList.Add(new DataItemParam { ItemName = "FinancialDepartment", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "FinancialDepartment" : ",FinancialDepartment") + ":" + item["name"];break;
                //2019-04-15case "mortgageconfirmation": dataList.Add(new DataItemParam { ItemName = "MortgageConfirmation", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "MortgageConfirmation" : ",MortgageConfirmation") + ":" + item["name"];break;
                //2019-04-15case "xsqk": dataList.Add(new DataItemParam { ItemName = "XSQK", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "XSQK" : ",XSQK") + ":" + item["name"];break;
                //2019-04-15case "zxqk": dataList.Add(new DataItemParam { ItemName = "ZXQK", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "ZXQK" : ",ZXQK") + ":" + item["name"];break;
                //2019-04-15case "sxqk": dataList.Add(new DataItemParam { ItemName = "SXQK", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "SXQK" : ",SXQK") + ":" + item["name"];break;
                //2019-04-15case "businesslife": dataList.Add(new DataItemParam { ItemName = "BusinessLife", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "BusinessLife" : ",BusinessLife") + ":" + item["name"];break;
                //2019-04-15case "seniority": dataList.Add(new DataItemParam { ItemName = "Seniority", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Seniority" : ",Seniority") + ":" + item["name"];break;
                //2019-04-15case "administrative": dataList.Add(new DataItemParam { ItemName = "Administrative", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Administrative" : ",Administrative") + ":" + item["name"];break;
                //2019-04-15case "lease": dataList.Add(new DataItemParam { ItemName = "Lease", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Lease" : ",Lease") + ":" + item["name"];break;
                //2019-04-15case "sixbankflow": dataList.Add(new DataItemParam { ItemName = "SixBankFlow", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "SixBankFlow" : ",SixBankFlow") + ":" + item["name"];break;
                //2019-04-15case "profits": dataList.Add(new DataItemParam { ItemName = "Profits", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "Profits" : ",Profits") + ":" + item["name"];break;
                //2019-04-15case "isbank": dataList.Add(new DataItemParam { ItemName = "IsBank", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "IsBank" : ",IsBank") + ":" + item["name"];break;

                //2019-04-15case "total": changeField += (changeField == "" ? "Total" : ",Total") + ":" + item["name"];break;
                //case "shareratio": changeField += (changeField == "" ? "ShareRatio" : ",ShareRatio") + ":" + item["name"];break;
                //case "area": changeField += (changeField == "" ? "Area" : ",Area") + ":" + item["name"];break;
                //case "financialdepartment": changeField += (changeField == "" ? "FinancialDepartment" : ",FinancialDepartment") + ":" + item["name"];break;
                //case "mortgageconfirmation": changeField += (changeField == "" ? "MortgageConfirmation" : ",MortgageConfirmation") + ":" + item["name"];break;
                //case "xsqk": changeField += (changeField == "" ? "XSQK" : ",XSQK") + ":" + item["name"];break;
                //case "zxqk": changeField += (changeField == "" ? "ZXQK" : ",ZXQK") + ":" + item["name"];break;
                //case "sxqk": changeField += (changeField == "" ? "SXQK" : ",SXQK") + ":" + item["name"];break;
                //case "businesslife": changeField += (changeField == "" ? "BusinessLife" : ",BusinessLife") + ":" + item["name"];break;
                //case "seniority": changeField += (changeField == "" ? "Seniority" : ",Seniority") + ":" + item["name"];break;
                //case "administrative": changeField += (changeField == "" ? "Administrative" : ",Administrative") + ":" + item["name"];break;
                //case "lease": changeField += (changeField == "" ? "Lease" : ",Lease") + ":" + item["name"];break;
                //case "sixbankflow": changeField += (changeField == "" ? "SixBankFlow" : ",SixBankFlow") + ":" + item["name"];break;
                //case "profits": changeField += (changeField == "" ? "Profits" : ",Profits") + ":" + item["name"];break;
                //case "isbank": changeField += (changeField == "" ? "IsBank" : ",IsBank") + ":" + item["name"];break;

                case "queryrecord":  dataList.Add(new DataItemParam { ItemName = "QueryRecord", ItemValue = item["value"].ToString() }); changeField += (changeField == "" ? "QueryRecord" : ",QueryRecord") + ":" + item["name"];break;
                case "legalperson":  dataList.Add(new DataItemParam { ItemName = "LegalPerson", ItemValue = item["value"].ToString() });changeField += (changeField == "" ? "LegalPerson" : ",LegalPerson") + ":" + item["name"]; break;
                case "financialcode": /*dataList.Add(new DataItemParam { ItemName = "FinancialCode", ItemValue = item["value"].ToString() }); */crmid = item["value"].ToString(); break;//2019-04-15
            }
        }
        dataList.Add(new DataItemParam { ItemName = "ChangeField", ItemValue = changeField });
        return dataList;
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
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
}