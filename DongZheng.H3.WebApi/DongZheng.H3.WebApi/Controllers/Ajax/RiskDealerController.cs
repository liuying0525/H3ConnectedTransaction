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
    public class RiskDealerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string search = context.Request["search"];
            JavaScriptSerializer json = new JavaScriptSerializer();
            var result = GetRiskPoint(search);
            context.Response.Write("{\"List\":" + json.Serialize(result) + "}");
        }

        public List<Risk> GetRiskPoint(string search)
        {
            string sql = "SELECT DISTINCT IRP.Dealer " +
                         "FROM I_RiskPointForm IRP " +
                         "LEFT JOIN I_ALLOWIN IA ON IA.DISTRIBUTOR = IRP.Dealer " +
                    (string.IsNullOrEmpty(search) ? "" : " WHERE IRP.ParentObjectID like '%" + search + "%'");
            System.Data.DataTable table = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            List<Risk> list = new List<Risk>();
            if (table.Rows.Count > 0)
            {
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    Risk Area = new Risk();
                    Area.经销商名称 = table.Rows[i]["Dealer"].ToString();
                    //Area.Grade = table.Rows[i]["ZScore"].ToString();
                    //Area.渠道分类 = table.Rows[i]["Type"].ToString();
                    //Area.经销商分类 = table.Rows[i]["DistributorType"].ToString();
                    //Area.省份 = table.Rows[i]["Province"].ToString();
                    //Area.城市 = table.Rows[i]["City"].ToString();
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
        }

        public void Log(string msg)
        {
            System.IO.File.AppendAllText(System.Web.HttpContext.Current.Server.MapPath("~/Log/log.txt"), msg);
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