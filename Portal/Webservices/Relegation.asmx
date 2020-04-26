<%@ WebService Language="C#" Class="Relegation" %>


using System;
using System.Collections.Generic;
using System.Web.Services;
using OThinker.H3.Controllers;

/// <summary>
/// Warning 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class Relegation : System.Web.Services.WebService
{
    public Relegation()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public List<Relegate> GetRelegation(string search)
    {
        return GetRelegationData(search);
    }

    public List<Relegate> GetRelegationData(string search)
    {
        string sql = "SELECT Distributor as Dealer,SystemScore as Grade,Type as Channels,DistributorType as DealerClass,Province,City,CompanyAddr as Address,BelongTo as Company,Brand," +
                     " QYWYKT as NetSilver,LoanType as Loan,ZHTime as OpenDate,'' as CloseDate,Memo,License,EnterpriseRegistration as Register," +
                     " RegistrationDate as RegisterDate,CreatDate as OriginateDate,LegalRepresentative as Representative,CorporateIdentityCard as Card,RegisteredCapital as Capital, " + 
                     " BankBranch as OpenBank,AccountType,BankName as AccountName,BankAccount as Account,CoupletNumber as Couplet,CrmDealerId FROM I_ALLOWIN" +
                    (string.IsNullOrEmpty(search) ? "" : " WHERE Distributor LIKE '%" + search + "%'");
        System.Data.DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        List<Relegate> resultlist = new List<Relegate>();
        if (table.Rows.Count > 0)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Relegate rel = new Relegate();
                rel.Dealer = table.Rows[i]["Dealer"].ToString();
                rel.Grade = table.Rows[i]["Grade"].ToString();
                rel.Channels = table.Rows[i]["Channels"].ToString();
                rel.DealerClass = table.Rows[i]["DealerClass"].ToString();
                rel.Province = table.Rows[i]["Province"].ToString();
                rel.City = table.Rows[i]["City"].ToString();
                rel.Address = table.Rows[i]["Address"].ToString();
                rel.Company = table.Rows[i]["Company"].ToString();
                rel.Brand = table.Rows[i]["Brand"].ToString();
                rel.NetSilver = table.Rows[i]["NetSilver"].ToString();
                rel.Loan = table.Rows[i]["Loan"].ToString();
                rel.OpenDate = table.Rows[i]["OpenDate"].ToString();
                rel.CloseDate = table.Rows[i]["CloseDate"].ToString();
                rel.Memo = table.Rows[i]["Memo"].ToString();
                rel.License = table.Rows[i]["License"].ToString();
                rel.Register = table.Rows[i]["Register"].ToString();
                rel.RegisterDate = table.Rows[i]["RegisterDate"].ToString();
                rel.OriginateDate = table.Rows[i]["OriginateDate"].ToString();
                rel.Representative = table.Rows[i]["Representative"].ToString();
                rel.Card = table.Rows[i]["Card"].ToString();
                rel.Capital = table.Rows[i]["Capital"].ToString();
                rel.OpenBank = table.Rows[i]["OpenBank"].ToString();
                rel.AccountType = table.Rows[i]["AccountType"].ToString();
                rel.AccountName = table.Rows[i]["AccountName"].ToString();
                rel.Account = table.Rows[i]["Account"].ToString();
                rel.Couplet = table.Rows[i]["Couplet"].ToString();
                rel.CrmDealerId = table.Rows[i]["CrmDealerId"].ToString();
                resultlist.Add(rel);
            }
        }
        return resultlist;
    }

    public struct Relegate
    {
        public string Dealer;
        public string Grade;
        public string Channels;
        public string DealerClass;
        public string Province;
        public string City;
        public string Address;
        public string Company;
        public string Brand;
        public string NetSilver;
        public string Loan;
        public string OpenDate;
        public string CloseDate;
        public string Memo;
        public string License;
        public string Register;
        public string RegisterDate;
        public string OriginateDate;
        public string Representative;
        public string Card;
        public string Capital;
        public string OpenBank;
        public string AccountType;
        public string AccountName;
        public string Account;
        public string Couplet;
        public string Relegation;
        public string TargetMargin;
        public string TargetLine;
        public string Remark;
        public string SalesApproval;
        public string CreditApproval;
        public string FinalMargin;
        public string FinalLine;
        public string PneumaticDecision;
        public string CrmDealerId;
    }
}