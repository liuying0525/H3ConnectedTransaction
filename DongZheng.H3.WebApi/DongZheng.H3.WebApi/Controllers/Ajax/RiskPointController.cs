using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class RiskPointController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            JavaScriptSerializer json = new JavaScriptSerializer();
            string search = context.Request["search"] ?? "";
            context.Response.Write(json.Serialize(GetRiskPoint(search)));
        }
        public List<Risk> GetRiskPoint(string search)
        {
            string sql = "SELECT DISTINCT IRP.Dealer,IA.ZScore,IA.TYPE,IA.DISTRIBUTORTYPE,IA.PROVINCE,IA.CITY,IA.CrmDealerId FROM I_RiskPointForm IRP LEFT JOIN I_ALLOWIN IA ON IA.DISTRIBUTOR = IRP.Dealer" +
                         (string.IsNullOrEmpty(search) ? "" : " WHERE IRP.ParentObjectID like '%" + search + "%'");
            System.Data.DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            List<Risk> list = new List<Risk>();
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Risk Area = new Risk();
                    Area.经销商名称 = table.Rows[i]["Dealer"] + string.Empty;
                    Area.渠道分类 = table.Rows[i]["Type"] + string.Empty;
                    Area.经销商分类 = table.Rows[i]["DistributorType"] + string.Empty;
                    Area.省份 = table.Rows[i]["Province"] + string.Empty;
                    Area.城市 = table.Rows[i]["City"] + string.Empty;
                    Area.评分 = table.Rows[i]["ZSCORE"] == null || table.Rows[i]["ZSCORE"].ToString() == "" ? "0" : table.Rows[i]["ZSCORE"].ToString();
                    Area.CrmDealerId = table.Rows[i]["CrmDealerId"].ToString();
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
            public string 评分;
            public string CrmDealerId;
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}