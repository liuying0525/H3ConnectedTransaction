using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class MortgageSearchController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string cityCode = context.Request["codeid"] ?? "";
            string provinceCode = context.Request["provinceid"] ?? "";
            string shop = context.Request["shop"] ?? "";
            string city = context.Request["city"] ?? "";
            string province = context.Request["province"] ?? "";
            string sql = "SELECT DISTINCT A.SHOP,A.PROVINCE,A.CITY,A.SPNAME,A.DYNAME,B.NAME AS SNAME,C.NAME AS DNAME FROM C_MORTGAGERULES A LEFT JOIN OT_USER B ON A.SPNAME = B.OBJECTID LEFT JOIN OT_USER C ON A.DYNAME = C.OBJECTID WHERE A.CITY = '" + cityCode + "' AND A.PROVINCE = '" + provinceCode + "'";
            string result = "";
            try
            {
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        result += (string.IsNullOrEmpty(result) ? "" : "|") + item["SHOP"] + (string.IsNullOrEmpty(item["SNAME"].ToString()) ? "" : "-") + item["SNAME"] + (string.IsNullOrEmpty(item["DNAME"].ToString()) ? "" : ",") + item["DNAME"] + ":" + item["SPNAME"] + "," + item["DYNAME"];
                    }
                }
            }
            catch (Exception e)
            {
                AppUtility.Engine.LogWriter.Write("抵押流程抵押员上牌员检索：" + e.Message);
            }
            context.Response.Write(result);
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