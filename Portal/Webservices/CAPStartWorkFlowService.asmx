<%@ WebService Language="C#" Class="OThinker.H3.Portal.CAPStartWorkFlowService" %>
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
    //若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消对下行的注释 。 
    // [System.Web.Script.Services.ScriptService]
    public class CAPStartWorkFlowService : System.Web.Services.WebService
    {
        public string cap_startflow_enabled = ConfigurationManager.AppSettings["Cap_Startflow_Enabled"] + string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public CAPStartWorkFlowService()
        {
            //如果使用设计的组件，请取消注释以下行 
            //InitializeComponent(); 
        }
        #region CAP
        [WebMethod(Description = "ProposalApproval")]
        public string ProposalApproval(string SchemaCode,string applicationNumber, string StatusCode, string ApprovalUser)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：回写CAP开始");
            CAP_Fuc cap = new CAP_Fuc();
            string cap_state = cap.GetCAPStateCode(applicationNumber);
            if (cap_state == "01")
            {
                ReturnToCAP.TransResult transResult_ = new ReturnToCAP.TransResult();
                ReturnToCAP.SendBackFIReq value_ = new ReturnToCAP.SendBackFIReq();
                value_.applicationNo = applicationNumber;
                value_.Comment = "驳回";
                ReturnToCAP.WFServiceImpl service_ = new ReturnToCAP.WFServiceImpl();
                transResult_ = service_.SendBackFI(value_);
                this.Engine.LogWriter.Write(applicationNumber + "：打回CAP草稿状态结束(01)：'" + transResult_.message + "'");

                cap.SetStateTo06(applicationNumber, "核准后需要取消");
            }

            string state = "";
            switch (StatusCode)
            {
                case "核准": state = "APP"; break;
                case "拒绝": state = "DEC"; break;
                case "有条件核准": state = "APC"; break;
                case "取消": state = "CAN"; break;
            }
            if (state == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态为空，未回写CAP");
                return "";
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "回写CAP参数：" + SchemaCode + "," + applicationNumber + ","+state+"," + ApprovalUser);
            string ApprovalComments = "";
            //string sql = "select * from (select  text from OT_Comment where instanceid='" + instanceid + "'  order by CreatedTime desc) where rownum=1";
            string sql = "select c.text,a.objectid from OT_instancecontext a inner join I_" + SchemaCode + " b on A.BIZOBJECTID=B.OBJECTID inner join OT_Comment c on c.instanceid=a.objectid where a.state=2 and C.DATAFIELD='ZSYJj' and b.applicationno='" + applicationNumber + "' order by a.createdtime desc";
            DataTable dt = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            ApprovalComments = dt.Rows[0][0].ToString();
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.ApprovalReq value = new ReturnToCAP.ApprovalReq();
            value.StatusCode = state;
            value.applicationNumber = applicationNumber;
            value.ApprovalComments = ApprovalComments;
            value.ApprovalUser = ApprovalUser;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.ProposalApproval(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态回写CAP结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝" || StatusCode == "取消")
            {
                //结束当前流程
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：审核拒绝，自动结束流程！");
                new WorkFlowFunction().finishedInstance(dt.Rows[0][1].ToString()); 
            }
            return transResult.message;
        }
        [WebMethod(Description = "ProposalApprovalByFK")]
        public string ProposalApprovalByFK(string SchemaCode, string applicationNumber, string ApprovalUser)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态'APP'回写CAP开始：" + SchemaCode + "," + applicationNumber + "," + ApprovalUser);
            string state = "APP";

            string ApprovalComments = "风控自动通过";
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.ApprovalReq value = new ReturnToCAP.ApprovalReq();
            value.StatusCode = state;
            value.applicationNumber = applicationNumber;
            value.ApprovalComments = ApprovalComments;
            value.ApprovalUser = ApprovalUser;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.ProposalApproval(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：风控自动通过状态回写CAP结束：'" + transResult.message + "'");
            return transResult.message;
        }
        [WebMethod(Description = "SendBackFI")]
         public string SendBackFI(string SchemaCode,string applicationNo, string InstanceID, string ActivityCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：驳回BackFI：" + SchemaCode + "," + applicationNo + "," + InstanceID + "," + ActivityCode);
            //取当前节点的所有任务
            string tokenid = "";
            string sqltoken = "select InstanceId,ActivityCode,TokenId from OT_WorkItem where InstanceId='" + InstanceID + "' and ActivityCode='" + ActivityCode + "'";
            DataTable dtworkitem = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sqltoken);
            if (dtworkitem != null && dtworkitem.Rows.Count > 0)
            {
                tokenid = dtworkitem.Rows[0]["TokenId"].ToString();
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：tokenid为"+tokenid);
            if (tokenid=="" || tokenid == "1")
            {
                return ""; 
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "打回CAP草稿状态开始");
            string ApprovalComments = "";
            string sql = "select c.text from OT_instancecontext a inner join I_" + SchemaCode + " b on A.BIZOBJECTID=B.OBJECTID inner join OT_Comment c on c.instanceid=a.objectid where a.state=2 and C.DATAFIELD='CSYJ' and b.applicationno='" + applicationNo + "' order by a.createdtime desc";
            ApprovalComments = this.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.SendBackFIReq value = new ReturnToCAP.SendBackFIReq();
            value.applicationNo = applicationNo;
            value.Comment = ApprovalComments!=""?ApprovalComments:"驳回";
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "打回CAP草稿状态意见：" + value.Comment);
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.SendBackFI(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：打回CAP草稿状态结束：'" + transResult.message + "'");
            return transResult.message;
        }
        [WebMethod(Description = "UpdateDDBank")]
        public string UpdateDDBank(string applicationNumber, string AccountName, string AccountNumber, string BankName, string BranchName, string EngineNo, string VinNo)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "更新银行信息开始：" + applicationNumber +"," +  AccountName +"," +  AccountNumber +"," +  BankName +"," +  BranchName +"," +  EngineNo + "," + VinNo);
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.DDBankReq value = new ReturnToCAP.DDBankReq();
            value.AccountName = AccountName;
            value.AccountNumber = AccountNumber;
            value.BankName = BankName;
            value.BranchName = BranchName;
            value.EngineNo = EngineNo;
            value.VinNo = VinNo;
            value.applicationNumber = applicationNumber;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.UpdateDDBank(value);
            this.Engine.LogWriter.Write("更新银行信息结束：returnCode-->" + transResult.returnCode + ",message->" + transResult.message);
            if (transResult.message == "Invalid / Null Application Number")
                return "无效申请号，请检查cap";
            else if (transResult.message == "Invalid Approval Status")
                return "cap中状态非核准状态";
            else if (transResult.message == "Provided VIN Number length less than 17 characters")
                return "车驾号小于17位";
            else if (transResult.message == "Provided VIN Number is already exist")
            {
                string sql = "SELECT APPLICATION_NUMBER FROM VEHICLE_DETAIL WHERE VIN_NUMBER = '{0}' AND APPLICATION_NUMBER !=  '{1}'";
                sql = string.Format(sql, VinNo, applicationNumber);
                DataTable dt = ExecuteDataTable("cap", sql);
                string numbers = "";
                foreach (DataRow row in dt.Rows)
                {
                    numbers += row[0] + ",";
                }
                if (numbers != "")
                    numbers = numbers.Substring(0, numbers.Length - 1);
                return "VIN重复，请检查（" + numbers + "）";
            }
            else if (transResult.message == "Provided Engine Number is already exist")
            {
                string sql = "SELECT APPLICATION_NUMBER FROM VEHICLE_DETAIL WHERE engine_number = '{0}' AND APPLICATION_NUMBER !=  '{1}'";
                sql = string.Format(sql, EngineNo, applicationNumber);
                DataTable dt = ExecuteDataTable("cap", sql);
                string numbers = "";
                foreach (DataRow row in dt.Rows)
                {
                    numbers += row[0] + ",";
                }
                if (numbers != "")
                    numbers = numbers.Substring(0, numbers.Length - 1);
                return "发动机号重复，请检查（" + numbers + "）";
            }
            else if (transResult.message == "Bank information is not provided yet")
                return "请提供银行账户信息";
            else if (transResult.message == "Applicant Role - Bank information is not provided yet")
                return "请提供银行账户信息";
            else if (transResult.message == "Provided Engine Number is already exist")
                return "请提供银行账户信息";

            return transResult.message;
        }

        public DataTable ExecuteDataTable(string connectionCode,string sql)
        {
            try
            {
                var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                if (dbObject != null)
                {
                    OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                    var command = factory.CreateCommand();
                    return command.ExecuteDataTable(sql);
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        [WebMethod(Description = "CMSComplied")]
        public string CMSComplied(string instanceid, string applicationNumber, string StatusCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSComplied开始");
            string state = "";
            switch (StatusCode)
            {
                case "核准": state = "12"; break;
                case "拒绝": state = "14"; break;
            }
            if (state == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态为空，CMSComplied回写失败");
                return "";
            }
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSComplied 参数：" + applicationNumber + "," + StatusCode+","+state);
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.CMSCompliedReq value = new ReturnToCAP.CMSCompliedReq();
            value.applicationNumber = applicationNumber;
            value.StatusCode = state;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.CMSComplied(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：CMSComplied结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝")
            {
                //结束当前流程
                new WorkFlowFunction().finishedInstance(instanceid);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：审核拒绝，自动结束流程！");
            }
            return transResult.message;
        }
        [WebMethod(Description = "CMSSettled")]
        public string CMSSettled(string instanceid, string applicationNumber, string StatusCode)
        {
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSSettled开始");
            string state = "";
            switch (StatusCode)
            {
                case "核准": state = "17"; break;
                case "拒绝": state = "05"; break;
            }
            if (state == "")
            {
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：状态为空，CMSSettled回写失败");
                return "";
            }
            ReturnToCAP.TransResult transResult = new ReturnToCAP.TransResult();
            ReturnToCAP.CMSSettledReq value = new ReturnToCAP.CMSSettledReq();
            value.applicationNumber = applicationNumber;
            value.StatusCode = state;
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            transResult = service.CMSSettled(value);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：CMSSettled结束：'" + transResult.message + "'");
            if (StatusCode == "拒绝")
            {
                //结束当前流程
                new WorkFlowFunction().finishedInstance(instanceid);
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：审核拒绝，自动结束流程！");
            }
            return transResult.message;
        }
        [WebMethod(Description = "CMSCollectionData")]
        public string CMSCollectionData(string contractNbr)
        {
            string value = "";
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CMSCollectionData开始");
            ReturnToCAP.WFServiceImpl service = new ReturnToCAP.WFServiceImpl();
            value = service.CMSCollectionData(contractNbr);
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "：CMSCollectionData结束：'" + value + "'");
            return value;
        }
        /// <summary>
        /// CAP Start H3 workflow
        /// </summary>
        /// <param name="capinfo">CAP Data</param>
        /// <returns></returns>
        [WebMethod(Description = "CAP Start H3 workflow")]
        public string CAPStartWorkFlow(string capinfo)
        {
            //    string rtnValue = "";
            //    rtnValue += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            //    rtnValue+=" <CAPStartWorkFlowResponse>";
            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "CAPStartWorkFlow开始");
            string rtnValue = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            string WorkFlowCode = "", UserCode = "administrator", ApplyNO = "", productGroupCode = "", workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;
            //BPMServiceResult result;
            try
            {
                //capinfo = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <Application> <appAsset> <data> <applicationNo>Br-A043972000</applicationNo> <assetConditionCode>N</assetConditionCode> <assetConditionName>新车 New</assetConditionName> <assetMakeCode>00016</assetMakeCode> <assetMakeName>别克</assetMakeName> <assetPrice>209000</assetPrice> <brandCode>00079</brandCode> <brandName>GL8</brandName> <comments>别克 201 2.4 经典版</comments> <deliveryDate /> <engineNo /> <miocn>BUICK GL8-A24LV01</miocn> <modelCode>00429</modelCode> <modelName>201 2.4 经典版</modelName> <purposeCode>1</purposeCode> <purposeName>自用 Self-Use</purposeName> <releaseDate /> <releaseMonth>0</releaseMonth> <releaseYear>0</releaseYear> <series /> <transmissionCode>Y</transmissionCode> <transmissionName>自动 Automatic</transmissionName> <vehicleSubtypeCode>00152</vehicleSubtypeCode> <vehicleSubtypeName>N/A</vehicleSubtypeName> <vehiclecolorCode /> <vehiclecolorName /> <vinNo /> <manufactureYear /> <engineCC /> <vehicleAge>0</vehicleAge> <registrationNo /> <vehicleBody /> <style /> <cylinder /> <odometer>0</odometer> <wheelWidth /> <vineditind>F</vineditind> </data> </appAsset> <appFinanTerms> <data> <actualRate>9.98</actualRate> <applicationNo>Br-A043972000</applicationNo> <balloonAmount>0</balloonAmount> <balloonRate>0</balloonRate> <deferredTerm>0</deferredTerm> <downpaymentamount>41800</downpaymentamount> <downpaymentrate>20</downpaymentrate> <financedamount>167200</financedamount> <financedamountrate>80</financedamountrate> <interestRate>9.98</interestRate> <otherfees>0</otherfees> <paymentFrequencyCode>00001</paymentFrequencyCode> <paymentFrequencyName>Monthly</paymentFrequencyName> <productGroupCode>6</productGroupCode> <productGroupName>等额本息-常规贷</productGroupName> <productTypeCode>224</productTypeCode> <productTypeName>BDI-A20N-等额本息01</productTypeName> <rentalmodeCode>00003</rentalmodeCode> <rentalmodeName>银行代扣 Direct Debit</rentalmodeName> <salesprice>209000</salesprice> <subsidyAmount>0</subsidyAmount> <subsidyRate>0</subsidyRate> <termMonth>12</termMonth> <totalintamount>9175.76</totalintamount> <vehicleprice>209000</vehicleprice> </data> </appFinanTerms> <appInfo> <data> <APPCREATETIME>10/18/2017 12:00:00 AM</APPCREATETIME> <appStatus>01</appStatus> <appTypeCode>00001</appTypeCode> <appTypeName>个人Individual</appTypeName> <applicationNo>Br-A043972000</applicationNo> <comments /> <financialadvisorId /> <financialadvisorName /> <provinceCode>00022</provinceCode> <provinceName>贵州</provinceName> <vehicleTypeCde>00001</vehicleTypeCde> <vehicleTypeName>标准型</vehicleTypeName> <salesPersonCode>7299</salesPersonCode> <showRoomCode /> <distributorCode /> <userName>yanama</userName> </data> </appInfo> <appRental> <data> <applicationNo>Br-A043972000</applicationNo> <rental_seq>1</rental_seq> <startTerm>1</startTerm> <endTerm>12</endTerm> <rentalAmount>14697.98</rentalAmount> <grossRentalAmount>14697.98</grossRentalAmount> </data> </appRental> <applicantType> <data> <APPLICATION_NUMBER>Br-A043972000</APPLICATION_NUMBER> <APPLICANT_TYPE>I</APPLICANT_TYPE> <GUARANTOR_TYPE /> <FIRST_NAME /> <LAST_NAME /> <COMPANY_NAME /> <REGESTRATION_NO /> <ID_CARD_NBR>440301198110012725</ID_CARD_NBR> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <NAME>陈佳</NAME> <MAIN_APPLICANT>Y</MAIN_APPLICANT> </data> <data> <APPLICATION_NUMBER>Br-A043972000</APPLICATION_NUMBER> <APPLICANT_TYPE /> <GUARANTOR_TYPE>C</GUARANTOR_TYPE> <FIRST_NAME /> <LAST_NAME /> <COMPANY_NAME>adf</COMPANY_NAME> <REGESTRATION_NO>32132132132132132132</REGESTRATION_NO> <ID_CARD_NBR /> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <NAME>zaheer company</NAME> <MAIN_APPLICANT /> </data> </applicantType> <assetLibilities> <data> <Code>BS120</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>14697.98</Value> <FinancialDetailType>L</FinancialDetailType> <IdentifierCode>BS120</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>本次车贷月供金额</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS112</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS112</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>代发工资</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS118</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS118</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>定期存款</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>B0011</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BA002</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>股票</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS113</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS113</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>活期结息</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>B0012</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BA004</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>基金</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BA001</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BA005</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>理财</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>B3005</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>E</FinancialDetailType> <IdentifierCode>BE005</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>利息支出</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS119</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>L</FinancialDetailType> <IdentifierCode>BS119</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>其他负债月供</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS111</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS111</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>收入/所得税</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> </assetLibilities> <financialRatios> <data> <Financialrationcode>HQ001</Financialrationcode> <Financialratiodes>活期日均Current Interest Average Daily</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ001</Financialrationcode> <Financialratiodes>借款人月收入Borrower Monthly Income</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ002</Financialrationcode> <Financialratiodes>共借人月收入Co-borrower Monthly Income</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ003</Financialrationcode> <Financialratiodes>担保人月收入Guarantor Monthly Income</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ004</Financialrationcode> <Financialratiodes>收入负债比</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ005</Financialrationcode> <Financialratiodes>金融资产Financing Asset</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> </financialRatios> <assetAccessories>123</assetAccessories> <companyInfo> <data> <COMPANYROLE>G</COMPANYROLE> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <annualRevenue>5454</annualRevenue> <applicationNo>Br-A043972000</applicationNo> <businesstypeCode>00008</businesstypeCode> <businesstypeName>合资企业 Joint Venture</businesstypeName> <capitalReamount>212121</capitalReamount> <companyName>zaheer company</companyName> <emailAddress>adfasd</emailAddress> <taxid>566541f</taxid> <compaynameeng>adf</compaynameeng> <establishedin>10/15/2003 12:00:00 AM</establishedin> <industrysubtypeCode>00003</industrysubtypeCode> <industrysubtypeName>房地产中介服务 Agency Services for Real Estate</industrysubtypeName> <industrytypeCode>00011</industrytypeCode> <industrytypeName>房地产业</industrytypeName> <loanCard /> <loancardPassword /> <networthamt>0</networthamt> <organizationCode>132132132132132132</organizationCode> <parentCompany>parent </parentCompany> <registrationNo>32132132132132132132</registrationNo> <representativeName>adf</representativeName> <representativeidNo /> <staffNumber>asf</staffNumber> <subsidaryCompany>asdsf</subsidaryCompany> <trustName>4654</trustName> <years>14</years> <representativeDesignation>adf</representativeDesignation> <comments>adfads</comments> </data> </companyInfo> <applicantInfo> <data> <APPLICATION_NUMBER>Br-A043972000</APPLICATION_NUMBER> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <APPLICANTROLE>B</APPLICANTROLE> <idcardtypeCode>00001</idcardtypeCode> <idcardtypeName>居民身份证</idcardtypeName> <idcardnumber>440301198110012725</idcardnumber> <customerid>0</customerid> <idcardissuedate /> <idcardexpirydate /> <titlecode>00001</titlecode> <titlename>Mr.</titlename> <thaititlecode>00001</thaititlecode> <thaititlename>先生</thaititlename> <customerName>陈佳</customerName> <thaimiddlename /> <thailastname /> <engfirstname /> <engmiddlename /> <englastname /> <maritalstatuscode>00002</maritalstatuscode> <maritalstatusname>未婚</maritalstatusname> <childrenflag>N</childrenflag> <numberofdependents /> <occupationCode>00003</occupationCode> <occupationName>办事人员和有关人员</occupationName> <occupationType>O</occupationType> <citizenshipCode>00001</citizenshipCode> <citizenshipName>中国 Chinese</citizenshipName> <licenseNumber /> <licenseexpirydate /> <dateofbirth>10/1/1981 12:00:00 AM</dateofbirth> <ageInYear>35</ageInYear> <ageInMonth>11</ageInMonth> <ageInYYMM>3511</ageInYYMM> <employeetypeCode>00006</employeetypeCode> <employeetypeName>自雇人士</employeetypeName> <timeasCustomer /> <genderCode>F</genderCode> <genderName>女</genderName> <industrytypeCode>00010</industrytypeCode> <industrytypeName>金融业</industrytypeName> <IndustrySubTypeCode /> <IndustrySubTypeName /> <emailaddress>chenjia2010@yeah.net</emailaddress> <tradingas /> <educationCode>00002</educationCode> <educationName>大学本科</educationName> <relationShipCode /> <relationShipName /> <bankstatementind /> <lettercertifyind /> <salayslipind /> <consent>N</consent> <staffind>F</staffind> <vipind>F</vipind> <blacklistdurationmonthNo /> <blacklistind>F</blacklistind> <blacklistnorecordind>T</blacklistnorecordind> <blacklistcontractind /> <hukouCode>00004</hukouCode> <hukouName>本地户口</hukouName> <nooffamilymembers /> <familyavgincome /> <houseownercode /> <houseownername /> <subOccupationCode>00044</subOccupationCode> <subOccupationName>默认值</subOccupationName> <designationCode>00001</designationCode> <desginationName>高级领导</desginationName> <jobgroupCode>00002</jobgroupCode> <jobgroupName>高级</jobgroupName> <salaryrangeCode /> <salaryrangeName /> <actualsalary>10000</actualsalary> <drivinglicensestatusCode>00001</drivinglicensestatusCode> <drivinglicensestatusName>本人或直系亲属有驾照</drivinglicensestatusName> <formername /> <nationCode>0001</nationCode> <nationName>汉族</nationName> <issuingAuthority>深圳公安局罗湖分局</issuingAuthority> <lienee>T</lienee> </data> </applicantInfo> <employment> <data> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <companyAddress>深圳市罗湖区笋岗中民时代广场A座20楼</companyAddress> <companyName>中信银行</companyName> <lineId>1</lineId> <phonenumber /> <Fax /> <provinceCode>00018</provinceCode> <provinceName>广东</provinceName> <cityCode>00199</cityCode> <cityName>深圳</cityName> <positionCode /> <positionName /> <postCode /> <timeinmonth /> <timeinyear>9</timeinyear> <comments /> <jobdescription /> <employertype /> <noofemployees /> <businesstypeCode>00010</businesstypeCode> <businesstypeName>私营有限责任公司 Private limited company</businesstypeName> </data> </employment> <referenceList> <data> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <address /> <cityCode /> <cityName /> <lineId>1</lineId> <mobile></mobile> <name /> <phoneNo /> <postCode /> <provinceCode /> <provinceName /> <relationshipCode /> <relationshipName /> <hokouCode /> <hokouName /> </data> </referenceList> <addressList> <data> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <addressCode>1</addressCode> <AddressId>罗湖区人民北路3110号大院12栋601</AddressId> <addressstatusCode>C</addressstatusCode> <addressstatusName>Current</addressstatusName> <addresstypeCode>00002</addresstypeCode> <addresstypeName>家庭地址 Home</addresstypeName> <cityCode>00199</cityCode> <cityName>深圳</cityName> <currentLivingAddress>罗湖区人民北路3110号大院12栋601</currentLivingAddress> <countryCode>00086</countryCode> <countryName>中国</countryName> <defaultmailingaddress>N</defaultmailingaddress> <hukouaddress>N</hukouaddress> <postcode>518000</postcode> <propertytypeCode>00001</propertytypeCode> <propertytypeName>自置</propertytypeName> <residencetypeCode /> <residencetypeName /> <provinceCode>00018</provinceCode> <provinceName>广东</provinceName> <homevalue /> <livingsince /> <stayinyear /> <stayinmonth /> <nativedistrict>广东</nativedistrict> <birthpalaceprovince>深圳</birthpalaceprovince> </data> <data> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <addressCode>1</addressCode> <AddressId /> <addressstatusCode>C</addressstatusCode> <addressstatusName>Current</addressstatusName> <addresstypeCode>00002</addresstypeCode> <addresstypeName>家庭地址 Home</addresstypeName> <cityCode>00199</cityCode> <cityName>深圳</cityName> <currentLivingAddress>罗湖区人民北路3110号大院12栋601</currentLivingAddress> <countryCode>00086</countryCode> <countryName>中国</countryName> <defaultmailingaddress>N</defaultmailingaddress> <hukouaddress>N</hukouaddress> <postcode>518000</postcode> <propertytypeCode>00001</propertytypeCode> <propertytypeName>自置</propertytypeName> <residencetypeCode /> <residencetypeName /> <provinceCode>00018</provinceCode> <provinceName>广东</provinceName> <homevalue /> <livingsince /> <stayinyear /> <stayinmonth /> <nativedistrict /> <birthpalaceprovince /> </data> </addressList> <phoneList> <data> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <addressCode>1</addressCode> <areaCode>4000</areaCode> <countrycodeCode>86</countrycodeCode> <countrycodeName /> <extension /> <phoneNo>15016701908</phoneNo> <phoneSeqId>1</phoneSeqId> <phonetypeCode>00003</phonetypeCode> <phonetypeName>手机 Mobile Phone</phonetypeName> </data> <data> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <addressCode>1</addressCode> <areaCode>4000</areaCode> <countrycodeCode>86</countrycodeCode> <countrycodeName /> <extension /> <phoneNo>15016701908</phoneNo> <phoneSeqId>1</phoneSeqId> <phonetypeCode>00003</phonetypeCode> <phonetypeName>手机 Mobile Phone</phonetypeName> </data> </phoneList> </Application>";
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(capinfo);
                XmlNode xn = xd.SelectSingleNode("//appInfo//data");
                appInfo appInfo = XmlUtil.Deserialize(typeof(appInfo), "<appInfo>" + xn.InnerXml + "</appInfo>") as appInfo;
                ApplyNO = appInfo.applicationNo;
                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "applicationNo：" + ApplyNO);
                if (appInfo.userName == "")
                {
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + "appInfo.userName为空");
                    //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程启动失败,账号:" + appInfo.userName + "不存在。\"}";
                    //return JsonConvert.SerializeObject(rtnValue);
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>流程启动失败,账号:" + appInfo.userName + "不存在。</messageInfo></CAPStartWorkFlowResponse>";
                }
				//增加判断，是否是从二期流程发起的
                var app_num = Convert.ToInt32(Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("select count(1) from I_APPlication app left join OT_InstanceContext con on app.objectid=con.bizobjectid where Application_number='" + ApplyNO + "' and (con.state=2 or con.state=4)") + string.Empty);
                if (app_num > 0)
                {
                    this.Engine.LogWriter.Write("CAP发起H3流程失败，此单号-->" + ApplyNO + "是从零售二期中发起的流程，不能从CAP提交");
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>CAP发起H3流程失败，此单号-->" + ApplyNO + "是从零售二期中发起的流程，不能从CAP提交</messageInfo></CAPStartWorkFlowResponse>";
                }
                //增加判断，是否是从cap发起的,2018-11-17开始不能从CAP发起流程
                if (!string.IsNullOrEmpty(cap_startflow_enabled) && !Convert.ToBoolean(cap_startflow_enabled))
                {
                    string sql_cap = @"
select count(1) from(
select app.objectid from i_Retailapp app left join OT_InstanceContext con on app.objectid=con.bizobjectid where app.applicationno='{0}' and con.objectid is not null
union all
select company.objectid from i_companyapp company left join OT_InstanceContext con on company.objectid=con.bizobjectid where company.applicationno='{0}' and con.objectid is not null
) aa
";
                    sql_cap = string.Format(sql_cap, ApplyNO);
                    this.Engine.LogWriter.Write("sql-->" + sql_cap);
                    var cap_n = Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql_cap) + string.Empty;

                    this.Engine.LogWriter.Write("cap_n-->" + cap_n);
                    if (cap_n == "0")//新发起的单据，不允许通过
                    {
                        this.Engine.LogWriter.Write("CAP发起H3流程失败，不能从CAP提交");
                        return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>CAP发起H3流程失败，不能从CAP提交</messageInfo></CAPStartWorkFlowResponse>";
                    }
                }
                UserCode = appInfo.userName;
                //判断是标贷还是简化贷
                productGroupCode = xd.SelectSingleNode("//appFinanTerms//data//productGroupCode").InnerXml;
                //if(productGroupCode=="6")//判断标贷，简化贷款
                WorkFlowCode = appInfo.appTypeCode == "00001" ? "RetailApp" : "CompanyApp";
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(WorkFlowCode);
                if (workflowTemplate == null)
                {
                    //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程启动失败,流程模板不存在，模板编码:" + WorkFlowCode + "。\"}";
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>workflow failed to start, workflow template does not exist, template coding:" + WorkFlowCode + "</messageInfo></CAPStartWorkFlowResponse>";
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(UserCode) as Organization.User;
                if (user == null)
                {
                    this.Engine.LogWriter.Write(DateTime.Now.ToString() + "在H3中找不到此账户" + UserCode);
                    //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程启动失败,用户{" + UserCode + "}不存在。\"}";
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>workflow failed to start,user{" + UserCode + "}not exsit。</messageInfo></CAPStartWorkFlowResponse>";
                    //return JsonConvert.SerializeObject(rtnValue);
                }
                // 这里演示了构造 BizObject 对象，以及调用 BizObject 对象的 Create 方法
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(WorkFlowCode);
                BizObject bo = new BizObject(this.Engine, schema, user.ObjectID);
                if (WorkFlowCode == "RetailApp")
                {
                    //个人贷款
                    DataTable dt = WorkFlowFunction.getdkInstanceData(WorkFlowCode, ApplyNO);
                    //已经存在applicationNo
                    if (CommonFunction.hasData(dt))
                    {
                        if (dt.Rows[0]["applicationNo"].ToString() == ApplyNO)
                        {
                            //数据更新
                            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "cap草稿状态提交开始");
                            string wid = WorkFlowFunction.getWorkItemIDByInstanceidAndActivityCode(dt.Rows[0]["instanceid"].ToString(), "Activity2");
                            if (wid == "")
                            {
                                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "instanceid：" + dt.Rows[0]["instanceid"].ToString() + "，cap草稿状态提交异常，当前流程不在初始节点");
                                rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>failed to start workflow! error： cap草稿状态提交异常，当前流程不在初始节点</messageInfo></CAPStartWorkFlowResponse>";
                                return rtnValue;
                            }
                            bo.ObjectID = dt.Rows[0]["objectid"].ToString();
                            bo.Load();
                            bo = new WorkFlowFunction().setIndvidualBizObject(bo, xd, WorkFlowCode, user.ObjectID, "RentalDetailtable");
                            bo.Update();
                            new WorkFlowFunction().SubmitWorkItemByInstanceID( dt.Rows[0]["instanceid"].ToString(),"");
                            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "cap草稿状态提交结束");
                            rtnValue += "<CAPStartWorkFlowResponse><messageField>Success</messageField> <returnCodeField>00001</returnCodeField><messageInfo>workflow instance started successfully</messageInfo></CAPStartWorkFlowResponse>";
                            return rtnValue;
                        }
                        else
                        {
                            //取消流程
                            OThinker.H3.Controllers.InstanceDetailController controller = new Controllers.InstanceDetailController();
                            controller.CancelInstance(dt.Rows[0]["objectid"].ToString());
                            bo = new WorkFlowFunction().setIndvidualBizObject(bo, xd, WorkFlowCode, user.ObjectID, "RentalDetailtable");
                            bo.Create();
                        }
                    }
                    else
                    {
                        bo = new WorkFlowFunction().setIndvidualBizObject(bo, xd, WorkFlowCode, user.ObjectID, "RentalDetailtable");
                        bo.Create();
                    }
                }
                else
                {
                    //机构贷款
                    DataTable dt = WorkFlowFunction.getdkInstanceData(WorkFlowCode, ApplyNO);
                    //已经存在applicationNo
                    if (CommonFunction.hasData(dt))
                    {
                        if (dt.Rows[0]["applicationNo"].ToString() == ApplyNO)
                        {
                            //数据更新
                            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "cap草稿状态提交开始");
                            string wid = WorkFlowFunction.getWorkItemIDByInstanceidAndActivityCode(dt.Rows[0]["instanceid"].ToString(), "Activity2");
                            if (wid == "")
                            {
                                this.Engine.LogWriter.Write(DateTime.Now.ToString() + "instanceid：" + dt.Rows[0]["instanceid"].ToString() + "，cap草稿状态提交异常，当前流程不在初始节点");
                                rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>failed to start workflow! error： cap草稿状态提交异常，当前流程不在初始节点</messageInfo></CAPStartWorkFlowResponse>";
                                return rtnValue;
                            }
                            bo.ObjectID = dt.Rows[0]["objectid"].ToString();
                            bo.Load();
                            bo = new WorkFlowFunction().setCompanyBizObject(bo, xd, WorkFlowCode, user.ObjectID, "CompanyDetailtable");
                            bo.Update();
                            new WorkFlowFunction().SubmitWorkItemByInstanceID(dt.Rows[0]["instanceid"].ToString(), "");
                            this.Engine.LogWriter.Write(DateTime.Now.ToString() + "cap草稿状态提交结束");
                            rtnValue += "<CAPStartWorkFlowResponse><messageField>Success</messageField> <returnCodeField>00001</returnCodeField><messageInfo>workflow instance started successfully</messageInfo></CAPStartWorkFlowResponse>";
                            return rtnValue;
                        }
                        else
                        {
                            //取消流程
                            OThinker.H3.Controllers.InstanceDetailController controller = new Controllers.InstanceDetailController();
                            controller.CancelInstance(dt.Rows[0]["objectid"].ToString());
                            bo = new WorkFlowFunction().setCompanyBizObject(bo, xd, WorkFlowCode, user.ObjectID, "CompanyDetailtable");
                            bo.Create();
                        }
                    }
                    else
                    {
                        bo = new WorkFlowFunction().setCompanyBizObject(bo, xd, WorkFlowCode, user.ObjectID, "CompanyDetailtable");
                        bo.Create();
                    }
                }




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
                //result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", "");
                //rtnValue = "{ \"Success\": \"true\", \"Message\":\"流程实例启动成功\"}";
                rtnValue += "<CAPStartWorkFlowResponse><messageField>Success</messageField> <returnCodeField>00001</returnCodeField><messageInfo>workflow instance started successfully</messageInfo></CAPStartWorkFlowResponse>";


            }
            catch (Exception ex)
            {
                //result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.ToString());
                //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程实例启动失败！错误：" + ex.ToString() + "\"}";
                rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>failed to start workflow! error：" + ex.ToString() + "</messageInfo></CAPStartWorkFlowResponse>";
            }

            return rtnValue;

        }
        
        /// <summary>
        /// CAP Start H3 workflow
        /// </summary>
        /// <param name="capinfo">CAP Data</param>
        /// <returns></returns>
        [WebMethod(Description = "HistoryCAP Start H3 workflow")]
        public string HistoryCAPStartWorkFlow(string capinfo)
        {
            //    string rtnValue = "";
            //    rtnValue += "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";
            //    rtnValue+=" <CAPStartWorkFlowResponse>";

            string rtnValue = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>";

            string WorkFlowCode = "", UserCode = "administrator", ApplyNO = "", productGroupCode = "", workItemID, keyItem, errorMessage;
            workItemID = keyItem = errorMessage = string.Empty;
            //BPMServiceResult result;
            try
            {
                //capinfo = "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <Application> <appAsset> <data> <applicationNo>Br-A043972000</applicationNo> <assetConditionCode>N</assetConditionCode> <assetConditionName>新车 New</assetConditionName> <assetMakeCode>00016</assetMakeCode> <assetMakeName>别克</assetMakeName> <assetPrice>209000</assetPrice> <brandCode>00079</brandCode> <brandName>GL8</brandName> <comments>别克 201 2.4 经典版</comments> <deliveryDate /> <engineNo /> <miocn>BUICK GL8-A24LV01</miocn> <modelCode>00429</modelCode> <modelName>201 2.4 经典版</modelName> <purposeCode>1</purposeCode> <purposeName>自用 Self-Use</purposeName> <releaseDate /> <releaseMonth>0</releaseMonth> <releaseYear>0</releaseYear> <series /> <transmissionCode>Y</transmissionCode> <transmissionName>自动 Automatic</transmissionName> <vehicleSubtypeCode>00152</vehicleSubtypeCode> <vehicleSubtypeName>N/A</vehicleSubtypeName> <vehiclecolorCode /> <vehiclecolorName /> <vinNo /> <manufactureYear /> <engineCC /> <vehicleAge>0</vehicleAge> <registrationNo /> <vehicleBody /> <style /> <cylinder /> <odometer>0</odometer> <wheelWidth /> <vineditind>F</vineditind> </data> </appAsset> <appFinanTerms> <data> <actualRate>9.98</actualRate> <applicationNo>Br-A043972000</applicationNo> <balloonAmount>0</balloonAmount> <balloonRate>0</balloonRate> <deferredTerm>0</deferredTerm> <downpaymentamount>41800</downpaymentamount> <downpaymentrate>20</downpaymentrate> <financedamount>167200</financedamount> <financedamountrate>80</financedamountrate> <interestRate>9.98</interestRate> <otherfees>0</otherfees> <paymentFrequencyCode>00001</paymentFrequencyCode> <paymentFrequencyName>Monthly</paymentFrequencyName> <productGroupCode>6</productGroupCode> <productGroupName>等额本息-常规贷</productGroupName> <productTypeCode>224</productTypeCode> <productTypeName>BDI-A20N-等额本息01</productTypeName> <rentalmodeCode>00003</rentalmodeCode> <rentalmodeName>银行代扣 Direct Debit</rentalmodeName> <salesprice>209000</salesprice> <subsidyAmount>0</subsidyAmount> <subsidyRate>0</subsidyRate> <termMonth>12</termMonth> <totalintamount>9175.76</totalintamount> <vehicleprice>209000</vehicleprice> </data> </appFinanTerms> <appInfo> <data> <APPCREATETIME>10/18/2017 12:00:00 AM</APPCREATETIME> <appStatus>01</appStatus> <appTypeCode>00001</appTypeCode> <appTypeName>个人Individual</appTypeName> <applicationNo>Br-A043972000</applicationNo> <comments /> <financialadvisorId /> <financialadvisorName /> <provinceCode>00022</provinceCode> <provinceName>贵州</provinceName> <vehicleTypeCde>00001</vehicleTypeCde> <vehicleTypeName>标准型</vehicleTypeName> <salesPersonCode>7299</salesPersonCode> <showRoomCode /> <distributorCode /> <userName>yanama</userName> </data> </appInfo> <appRental> <data> <applicationNo>Br-A043972000</applicationNo> <rental_seq>1</rental_seq> <startTerm>1</startTerm> <endTerm>12</endTerm> <rentalAmount>14697.98</rentalAmount> <grossRentalAmount>14697.98</grossRentalAmount> </data> </appRental> <applicantType> <data> <APPLICATION_NUMBER>Br-A043972000</APPLICATION_NUMBER> <APPLICANT_TYPE>I</APPLICANT_TYPE> <GUARANTOR_TYPE /> <FIRST_NAME /> <LAST_NAME /> <COMPANY_NAME /> <REGESTRATION_NO /> <ID_CARD_NBR>440301198110012725</ID_CARD_NBR> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <NAME>陈佳</NAME> <MAIN_APPLICANT>Y</MAIN_APPLICANT> </data> <data> <APPLICATION_NUMBER>Br-A043972000</APPLICATION_NUMBER> <APPLICANT_TYPE /> <GUARANTOR_TYPE>C</GUARANTOR_TYPE> <FIRST_NAME /> <LAST_NAME /> <COMPANY_NAME>adf</COMPANY_NAME> <REGESTRATION_NO>32132132132132132132</REGESTRATION_NO> <ID_CARD_NBR /> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <NAME>zaheer company</NAME> <MAIN_APPLICANT /> </data> </applicantType> <assetLibilities> <data> <Code>BS120</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>14697.98</Value> <FinancialDetailType>L</FinancialDetailType> <IdentifierCode>BS120</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>本次车贷月供金额</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS112</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS112</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>代发工资</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS118</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS118</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>定期存款</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>B0011</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BA002</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>股票</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS113</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS113</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>活期结息</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>B0012</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BA004</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>基金</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BA001</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BA005</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>理财</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>B3005</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>E</FinancialDetailType> <IdentifierCode>BE005</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>利息支出</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS119</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>L</FinancialDetailType> <IdentifierCode>BS119</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>其他负债月供</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> <data> <Code>BS111</Code> <ApplicationNo>Br-A043972000</ApplicationNo> <IdentificationCode>1</IdentificationCode> <Value>0</Value> <FinancialDetailType>A</FinancialDetailType> <IdentifierCode>BS111</IdentifierCode> <ApplicantRole>B</ApplicantRole> <Descritption>收入/所得税</Descritption> <Ismandatory>F</Ismandatory> <Insertedby /> <Insertedon /> <Updatedby /> <Updatedon /> <Pcmind>F</Pcmind> </data> </assetLibilities> <financialRatios> <data> <Financialrationcode>HQ001</Financialrationcode> <Financialratiodes>活期日均Current Interest Average Daily</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ001</Financialrationcode> <Financialratiodes>借款人月收入Borrower Monthly Income</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ002</Financialrationcode> <Financialratiodes>共借人月收入Co-borrower Monthly Income</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ003</Financialrationcode> <Financialratiodes>担保人月收入Guarantor Monthly Income</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ004</Financialrationcode> <Financialratiodes>收入负债比</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> <data> <Financialrationcode>QQ005</Financialrationcode> <Financialratiodes>金融资产Financing Asset</Financialratiodes> <Applicationnumber>Br-A043972000</Applicationnumber> <Evalresult>0</Evalresult> <Activeind>T</Activeind> <Visibleind>T</Visibleind> </data> </financialRatios> <assetAccessories>123</assetAccessories> <companyInfo> <data> <COMPANYROLE>G</COMPANYROLE> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <annualRevenue>5454</annualRevenue> <applicationNo>Br-A043972000</applicationNo> <businesstypeCode>00008</businesstypeCode> <businesstypeName>合资企业 Joint Venture</businesstypeName> <capitalReamount>212121</capitalReamount> <companyName>zaheer company</companyName> <emailAddress>adfasd</emailAddress> <taxid>566541f</taxid> <compaynameeng>adf</compaynameeng> <establishedin>10/15/2003 12:00:00 AM</establishedin> <industrysubtypeCode>00003</industrysubtypeCode> <industrysubtypeName>房地产中介服务 Agency Services for Real Estate</industrysubtypeName> <industrytypeCode>00011</industrytypeCode> <industrytypeName>房地产业</industrytypeName> <loanCard /> <loancardPassword /> <networthamt>0</networthamt> <organizationCode>132132132132132132</organizationCode> <parentCompany>parent </parentCompany> <registrationNo>32132132132132132132</registrationNo> <representativeName>adf</representativeName> <representativeidNo /> <staffNumber>asf</staffNumber> <subsidaryCompany>asdsf</subsidaryCompany> <trustName>4654</trustName> <years>14</years> <representativeDesignation>adf</representativeDesignation> <comments>adfads</comments> </data> </companyInfo> <applicantInfo> <data> <APPLICATION_NUMBER>Br-A043972000</APPLICATION_NUMBER> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <APPLICANTROLE>B</APPLICANTROLE> <idcardtypeCode>00001</idcardtypeCode> <idcardtypeName>居民身份证</idcardtypeName> <idcardnumber>440301198110012725</idcardnumber> <customerid>0</customerid> <idcardissuedate /> <idcardexpirydate /> <titlecode>00001</titlecode> <titlename>Mr.</titlename> <thaititlecode>00001</thaititlecode> <thaititlename>先生</thaititlename> <customerName>陈佳</customerName> <thaimiddlename /> <thailastname /> <engfirstname /> <engmiddlename /> <englastname /> <maritalstatuscode>00002</maritalstatuscode> <maritalstatusname>未婚</maritalstatusname> <childrenflag>N</childrenflag> <numberofdependents /> <occupationCode>00003</occupationCode> <occupationName>办事人员和有关人员</occupationName> <occupationType>O</occupationType> <citizenshipCode>00001</citizenshipCode> <citizenshipName>中国 Chinese</citizenshipName> <licenseNumber /> <licenseexpirydate /> <dateofbirth>10/1/1981 12:00:00 AM</dateofbirth> <ageInYear>35</ageInYear> <ageInMonth>11</ageInMonth> <ageInYYMM>3511</ageInYYMM> <employeetypeCode>00006</employeetypeCode> <employeetypeName>自雇人士</employeetypeName> <timeasCustomer /> <genderCode>F</genderCode> <genderName>女</genderName> <industrytypeCode>00010</industrytypeCode> <industrytypeName>金融业</industrytypeName> <IndustrySubTypeCode /> <IndustrySubTypeName /> <emailaddress>chenjia2010@yeah.net</emailaddress> <tradingas /> <educationCode>00002</educationCode> <educationName>大学本科</educationName> <relationShipCode /> <relationShipName /> <bankstatementind /> <lettercertifyind /> <salayslipind /> <consent>N</consent> <staffind>F</staffind> <vipind>F</vipind> <blacklistdurationmonthNo /> <blacklistind>F</blacklistind> <blacklistnorecordind>T</blacklistnorecordind> <blacklistcontractind /> <hukouCode>00004</hukouCode> <hukouName>本地户口</hukouName> <nooffamilymembers /> <familyavgincome /> <houseownercode /> <houseownername /> <subOccupationCode>00044</subOccupationCode> <subOccupationName>默认值</subOccupationName> <designationCode>00001</designationCode> <desginationName>高级领导</desginationName> <jobgroupCode>00002</jobgroupCode> <jobgroupName>高级</jobgroupName> <salaryrangeCode /> <salaryrangeName /> <actualsalary>10000</actualsalary> <drivinglicensestatusCode>00001</drivinglicensestatusCode> <drivinglicensestatusName>本人或直系亲属有驾照</drivinglicensestatusName> <formername /> <nationCode>0001</nationCode> <nationName>汉族</nationName> <issuingAuthority>深圳公安局罗湖分局</issuingAuthority> <lienee>T</lienee> </data> </applicantInfo> <employment> <data> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <companyAddress>深圳市罗湖区笋岗中民时代广场A座20楼</companyAddress> <companyName>中信银行</companyName> <lineId>1</lineId> <phonenumber /> <Fax /> <provinceCode>00018</provinceCode> <provinceName>广东</provinceName> <cityCode>00199</cityCode> <cityName>深圳</cityName> <positionCode /> <positionName /> <postCode /> <timeinmonth /> <timeinyear>9</timeinyear> <comments /> <jobdescription /> <employertype /> <noofemployees /> <businesstypeCode>00010</businesstypeCode> <businesstypeName>私营有限责任公司 Private limited company</businesstypeName> </data> </employment> <referenceList> <data> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <address /> <cityCode /> <cityName /> <lineId>1</lineId> <mobile></mobile> <name /> <phoneNo /> <postCode /> <provinceCode /> <provinceName /> <relationshipCode /> <relationshipName /> <hokouCode /> <hokouName /> </data> </referenceList> <addressList> <data> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <addressCode>1</addressCode> <AddressId>罗湖区人民北路3110号大院12栋601</AddressId> <addressstatusCode>C</addressstatusCode> <addressstatusName>Current</addressstatusName> <addresstypeCode>00002</addresstypeCode> <addresstypeName>家庭地址 Home</addresstypeName> <cityCode>00199</cityCode> <cityName>深圳</cityName> <currentLivingAddress>罗湖区人民北路3110号大院12栋601</currentLivingAddress> <countryCode>00086</countryCode> <countryName>中国</countryName> <defaultmailingaddress>N</defaultmailingaddress> <hukouaddress>N</hukouaddress> <postcode>518000</postcode> <propertytypeCode>00001</propertytypeCode> <propertytypeName>自置</propertytypeName> <residencetypeCode /> <residencetypeName /> <provinceCode>00018</provinceCode> <provinceName>广东</provinceName> <homevalue /> <livingsince /> <stayinyear /> <stayinmonth /> <nativedistrict>广东</nativedistrict> <birthpalaceprovince>深圳</birthpalaceprovince> </data> <data> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <addressCode>1</addressCode> <AddressId /> <addressstatusCode>C</addressstatusCode> <addressstatusName>Current</addressstatusName> <addresstypeCode>00002</addresstypeCode> <addresstypeName>家庭地址 Home</addresstypeName> <cityCode>00199</cityCode> <cityName>深圳</cityName> <currentLivingAddress>罗湖区人民北路3110号大院12栋601</currentLivingAddress> <countryCode>00086</countryCode> <countryName>中国</countryName> <defaultmailingaddress>N</defaultmailingaddress> <hukouaddress>N</hukouaddress> <postcode>518000</postcode> <propertytypeCode>00001</propertytypeCode> <propertytypeName>自置</propertytypeName> <residencetypeCode /> <residencetypeName /> <provinceCode>00018</provinceCode> <provinceName>广东</provinceName> <homevalue /> <livingsince /> <stayinyear /> <stayinmonth /> <nativedistrict /> <birthpalaceprovince /> </data> </addressList> <phoneList> <data> <IDENTIFICATION_CODE>1</IDENTIFICATION_CODE> <addressCode>1</addressCode> <areaCode>4000</areaCode> <countrycodeCode>86</countrycodeCode> <countrycodeName /> <extension /> <phoneNo>15016701908</phoneNo> <phoneSeqId>1</phoneSeqId> <phonetypeCode>00003</phonetypeCode> <phonetypeName>手机 Mobile Phone</phonetypeName> </data> <data> <IDENTIFICATION_CODE>2</IDENTIFICATION_CODE> <addressCode>1</addressCode> <areaCode>4000</areaCode> <countrycodeCode>86</countrycodeCode> <countrycodeName /> <extension /> <phoneNo>15016701908</phoneNo> <phoneSeqId>1</phoneSeqId> <phonetypeCode>00003</phonetypeCode> <phonetypeName>手机 Mobile Phone</phonetypeName> </data> </phoneList> </Application>";
                XmlDocument xd = new XmlDocument();
                xd.LoadXml(capinfo);
                XmlNode xn = xd.SelectSingleNode("//appInfo//data");
                appInfo appInfo = XmlUtil.Deserialize(typeof(appInfo), "<appInfo>" + xn.InnerXml + "</appInfo>") as appInfo;
                ApplyNO = appInfo.applicationNo;
                if (appInfo.userName == "")
                {
                    //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程启动失败,账号:" + appInfo.userName + "不存在。\"}";
                    //return JsonConvert.SerializeObject(rtnValue);
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>流程启动失败,账号:" + appInfo.userName + "不存在。</messageInfo></CAPStartWorkFlowResponse>";
                }
                UserCode = appInfo.userName;
                //判断是标贷还是简化贷
                productGroupCode = xd.SelectSingleNode("//appFinanTerms//data//productGroupCode").InnerXml;
                //if(productGroupCode=="6")//判断标贷，简化贷款
                WorkFlowCode = appInfo.appTypeCode == "00001" ? "HRetailApp" : "HCompanyApp";
                // 获取模板
                OThinker.H3.WorkflowTemplate.PublishedWorkflowTemplateHeader workflowTemplate = GetWorkflowTemplate(WorkFlowCode);
                if (workflowTemplate == null)
                {
                    //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程启动失败,流程模板不存在，模板编码:" + WorkFlowCode + "。\"}";
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>workflow failed to start, workflow template does not exist, template coding:" + WorkFlowCode + "</messageInfo></CAPStartWorkFlowResponse>";
                }
                // 查找流程发起人
                OThinker.Organization.User user = this.Engine.Organization.GetUserByCode(UserCode) as Organization.User;
                if (user == null)
                {
                    //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程启动失败,用户{" + UserCode + "}不存在。\"}";
                    return rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>workflow failed to start,user{" + UserCode + "}not exsit。</messageInfo></CAPStartWorkFlowResponse>";
                    //return JsonConvert.SerializeObject(rtnValue);
                }
                // 这里演示了构造 BizObject 对象，以及调用 BizObject 对象的 Create 方法
                BizObjectSchema schema = this.Engine.BizObjectManager.GetPublishedSchema(WorkFlowCode);
                BizObject bo = new BizObject(this.Engine, schema, user.ObjectID);
                if (WorkFlowCode == "HRetailApp")
                {
                    //个人贷款
                    DataTable dt = WorkFlowFunction.getdkInstanceData(WorkFlowCode, ApplyNO);
                    //已经存在applicationNo
                    if (CommonFunction.hasData(dt))
                    {
                        if (dt.Rows[0]["applicationNo"].ToString() == ApplyNO)
                        {
                            return "";
                            //数据更新
                            bo.ObjectID = dt.Rows[0]["objectid"].ToString();
                            bo.Load();
                            bo = new WorkFlowFunction().setIndvidualBizObject(bo, xd, WorkFlowCode, user.ObjectID, "hRentalDetailtable");
                            bo.Update();
                        }
                        else
                        {
                            //取消流程
                            OThinker.H3.Controllers.InstanceDetailController controller = new Controllers.InstanceDetailController();
                            controller.CancelInstance(dt.Rows[0]["objectid"].ToString());
                            bo = new WorkFlowFunction().setIndvidualBizObject(bo, xd, WorkFlowCode, user.ObjectID, "hRentalDetailtable");
                            bo.Create();
                        }
                    }
                    else
                    {
                        bo = new WorkFlowFunction().setIndvidualBizObject(bo, xd, WorkFlowCode, user.ObjectID, "hRentalDetailtable");
                        bo.Create();
                    }
                }
                else
                {
                    //机构贷款
                    DataTable dt = WorkFlowFunction.getdkInstanceData(WorkFlowCode, ApplyNO);
                    //已经存在applicationNo
                    if (CommonFunction.hasData(dt))
                    {
                        if (dt.Rows[0]["applicationNo"].ToString() == ApplyNO)
                        {
                            return "";
                            //数据更新
                            bo.ObjectID = dt.Rows[0]["objectid"].ToString();
                            bo.Load();
                            bo = new WorkFlowFunction().setCompanyBizObject(bo, xd, WorkFlowCode, user.ObjectID, "hCompanyDetailtable");
                            bo.Update();
                        }
                        else
                        {
                            //取消流程
                            OThinker.H3.Controllers.InstanceDetailController controller = new Controllers.InstanceDetailController();
                            controller.CancelInstance(dt.Rows[0]["objectid"].ToString());
                            bo = new WorkFlowFunction().setCompanyBizObject(bo, xd, WorkFlowCode, user.ObjectID, "hCompanyDetailtable");
                            bo.Create();
                        }
                    }
                    else
                    {
                        bo = new WorkFlowFunction().setCompanyBizObject(bo, xd, WorkFlowCode, user.ObjectID, "hCompanyDetailtable");
                        bo.Create();
                    }
                }




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
                //result = new BPMServiceResult(true, InstanceId, workItemID, "流程实例启动成功！", "");
                //rtnValue = "{ \"Success\": \"true\", \"Message\":\"流程实例启动成功\"}";
                rtnValue += "<CAPStartWorkFlowResponse><messageField>Success</messageField> <returnCodeField>00001</returnCodeField><messageInfo>workflow instance started successfully</messageInfo></CAPStartWorkFlowResponse>";


            }
            catch (Exception ex)
            {
                //result = new BPMServiceResult(false, "流程实例启动失败！错误：" + ex.ToString());
                //rtnValue = "{ \"Success\": \"false\", \"Message\":\"流程实例启动失败！错误：" + ex.ToString() + "\"}";
                rtnValue += "<CAPStartWorkFlowResponse><messageField>Fail</messageField> <returnCodeField>00002</returnCodeField><messageInfo>failed to start workflow! error：" + ex.ToString() + "</messageInfo></CAPStartWorkFlowResponse>";
            }

            return rtnValue;

        }
        #endregion
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