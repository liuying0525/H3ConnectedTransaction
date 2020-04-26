<%@ WebService Language="C#" Class="RiskPoint" %>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
//using ScoreCore;// 19.7.2 wangxg
using System.Web.Services;
using System.Web.Script.Services;
using OThinker.H3.Controllers;

/// <summary>
/// RiskPoint 的摘要说明
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
[System.Web.Script.Services.ScriptService]
public class RiskPoint : System.Web.Services.WebService
{
    public RiskPoint()
    {
        //如果使用设计的组件，请取消注释以下行 
        //InitializeComponent(); 
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public List<Risk> GetRiskPointData(string search)
    {
        return GetRiskPoint(search);
    }

    public List<Risk> GetRiskPoint(string search)
    {
        //string sql = "SELECT DISTINCT IRP.Dealer,IA.SystemScore,IA.TYPE,IA.DISTRIBUTORTYPE,IA.PROVINCE,IA.CITY,IA.CrmDealerId FROM I_RiskPointForm IRP LEFT JOIN I_ALLOWIN IA ON IA.DISTRIBUTOR = IRP.Dealer " +
        //        (string.IsNullOrEmpty(search) ? "" : " WHERE IRP.ParentObjectID like '%" + search + "%'");
        string sql =
            "SELECT DEALER,TYPE,DISTRIBUTORTYPE,PROVINCE,CITY,CRMDEALERID,TOTAL,ICOUNT,CASE WHEN TOTAL IS NULL OR TOTAL = 0 THEN 0 ELSE ICOUNT*100/TOTAL END RATE FROM (" +
            "SELECT DISTINCT IRP.Dealer,IA.SystemScore,IA.TYPE,IA.DISTRIBUTORTYPE,IA.PROVINCE,IA.CITY,IA.CrmDealerId,(SELECT COUNT(1) FROM in_cms.mv_dy_application_date_info@to_dy_cms cms WHERE cms.经销商名称 = IRP.Dealer) TOTAL" +
            ",(SELECT COUNT(1) FROM in_cms.mv_dy_application_date_info@to_dy_cms cms WHERE cms.经销商名称 = IRP.Dealer AND TO_DATE(cms.合同到期日,'yyyy-mm-dd')< SYSDATE) ICOUNT FROM I_RiskPointForm IRP LEFT JOIN I_ALLOWIN IA ON IA.DISTRIBUTOR = IRP.Dealer" +
            (string.IsNullOrEmpty(search) ? "" : " WHERE IRP.ParentObjectID like '%" + search + "%'") + ")";
        System.Data.DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        List<Risk> list = new List<Risk>();
        if (table.Rows.Count > 0)
        {
            for (int i = 0; i < table.Rows.Count; i++)
            {
                Risk Area = new Risk();
                Area.经销商名称 = table.Rows[i]["DEALER"] + string.Empty;
                //Area.Grade = table.Rows[i]["SystemScore"] + string.Empty;
                Area.渠道分类 = table.Rows[i]["TYPE"] + string.Empty;
                Area.经销商分类 = table.Rows[i]["DISTRIBUTORTYPE"] + string.Empty;
                Area.省份 = table.Rows[i]["PROVINCE"] + string.Empty;
                Area.城市 = table.Rows[i]["CITY"].ToString();
                Area.CrmDealerId = table.Rows[i]["CRMDEALERID"] + string.Empty;
                Area.Rate = table.Rows[i]["RATE"] + string.Empty + "%";
                list.Add(Area);
            }
        }
        return list;
    }
        
    public struct Risk
    {
        public string 经销商名称;
        public string Grade;
        public string 渠道分类;
        public string 经销商分类;
        public string 省份;
        public string 城市;
        public string CrmDealerId;
        public string Rate;
    }
}
