using Newtonsoft.Json.Linq;
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
    public class GetDetailVarListController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            JArray ret = new JArray();

            DataTable table = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT DISTINCT \"Name\" FROM DEALERDATA");
            foreach (DataRow row in table.Rows)
            {
                JObject token = new JObject();
                token.Add("a", row[0].ToString());
                ret.Add(token);
            }

            table = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM in_cms.MV_H3_DEALER_OD_INFO_CN@to_dy_cms WHERE ROWNUM<=1");
            foreach (DataColumn column in table.Columns)
            {
                if ("经销商名称，经销商编号，渠道分类，渠道性质".IndexOf(column.ColumnName) >= 0) continue;
                JObject token = new JObject();
                token.Add("a", column.ColumnName);
                ret.Add(token);
            }

            table = OThinker.H3.Controllers.AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DZ_V_H3_GETSCOREGRADE WHERE ROWNUM<=1");
            foreach (DataColumn column in table.Columns)
            {
                if ("DEALERID，OBJECTID，DealerName, DISTRIBUTOR, DISTRIBUTORTYPE， TYPE".ToLower().IndexOf(column.ColumnName.ToLower()) >= 0) continue;
                JObject token = new JObject();
                token.Add("a", column.ColumnName);
                ret.Add(token);
            }

            context.Response.Write(ret);
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