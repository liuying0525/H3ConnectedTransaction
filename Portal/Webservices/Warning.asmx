<%@ WebService Language="C#" Class="Warning" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Services;
using OThinker.H3.Controllers;

/// <summary>
/// Warning 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
// [System.Web.Script.Services.ScriptService]
public class Warning : System.Web.Services.WebService
{
    public Warning()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    public List<Warn> GetWarning(string search)
    {
        return GetWarn(search);
    }

    public List<Warn> GetWarn(string search)
    {
        string sql =
            "select Distributor,ZScore,Type,DistributorType,Province,City,CompanyAddr,BelongTo,Brand,QYWYKT,LoanType,ZHTime,Memo,License,EnterpriseRegistration,RegistrationDate,CreatDate,LegalRepresentative,CorporateIdentityCard,RegisteredCapital,BankBranch,AccountType,BankName,BankAccount,CoupletNumber,SystemScore,CrmDealerId from i_allowin" +
            (string.IsNullOrEmpty(search) ? "" : " where DISTRIBUTOR like '%" + search + "%'");
        System.Data.DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        List<Warn> list = new List<Warn>();
        if (table.Rows.Count > 0)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Warn Area = new Warn();
                Area.经销商名称 = table.Rows[i]["Distributor"].ToString();
                Area.Grade = table.Rows[i]["ZScore"].ToString();
                Area.渠道分类 = table.Rows[i]["Type"].ToString();
                Area.经销商分类 = table.Rows[i]["DistributorType"].ToString();
                Area.省份 = table.Rows[i]["Province"].ToString();
                Area.城市 = table.Rows[i]["City"].ToString();
                Area.Address = table.Rows[i]["CompanyAddr"].ToString();
                Area.Company = table.Rows[i]["BelongTo"].ToString();
                Area.Brand = table.Rows[i]["Brand"].ToString();
                Area.NetSilver = table.Rows[i]["QYWYKT"].ToString();
                Area.Loan = table.Rows[i]["LoanType"].ToString();
                Area.OpenDate = table.Rows[i]["ZHTime"].ToString();
                Area.Memo = table.Rows[i]["Memo"].ToString();
                Area.License = table.Rows[i]["License"].ToString();
                Area.Register = table.Rows[i]["EnterpriseRegistration"].ToString();
                Area.RegisterDate = table.Rows[i]["RegistrationDate"].ToString();
                Area.OriginateDate = table.Rows[i]["CreatDate"].ToString();
                Area.Representative = table.Rows[i]["LegalRepresentative"].ToString();
                Area.Card = table.Rows[i]["CorporateIdentityCard"].ToString();
                Area.Capital = table.Rows[i]["RegisteredCapital"].ToString();
                Area.OpenBank = table.Rows[i]["BankBranch"].ToString();
                Area.AccountType = table.Rows[i]["AccountType"].ToString();
                Area.AccountName = table.Rows[i]["BankName"].ToString();
                Area.Account = table.Rows[i]["BankAccount"].ToString();
                Area.Couplet = table.Rows[i]["CoupletNumber"].ToString();
                Area.SystemScore = table.Rows[i]["SystemScore"].ToString();
                Area.CrmDealerId = table.Rows[i]["CrmDealerId"].ToString();
                list.Add(Area);
            }
        }
        return list;
    }
    public struct Warn
    {
        public string 经销商名称;
        public string Grade;
        public string 渠道分类;
        public string 经销商分类;
        public string 省份;
        public string 城市;
        public string Address;
        public string Company;
        public string Brand;
        public string NetSilver;
        public string Loan;
        public string OpenDate;
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
        public string SystemScore;
        public string CrmDealerId;
    }
}
