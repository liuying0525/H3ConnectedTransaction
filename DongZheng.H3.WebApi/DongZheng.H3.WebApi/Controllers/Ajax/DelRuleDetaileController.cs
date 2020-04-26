using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class DelRuleDetaileController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";


        public void Index()
        {
            var context = HttpContext;
            System.IO.Stream s = context.Request.InputStream;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
            {
                string sql = "";
                string sql0 = "";
                Newtonsoft.Json.Linq.JArray ret = Newtonsoft.Json.Linq.JArray.Parse(sr.ReadToEnd());
                foreach (Newtonsoft.Json.Linq.JObject item in ret)
                {
                    sql += (sql == "" ? " WHERE " : " OR ") + "(ObjectID='" + item["ObjectID"] + "' )";
                    sql0 += (sql0 == "" ? " WHERE " : " OR ") + "(PID='" + item["ObjectID"] + "' )";
                }
                if (!string.IsNullOrWhiteSpace(sql))
                    AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("UPDATE  C_RULEDETAILS SET STATE=0" + sql);
                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("UPDATE  C_SCORE SET STATE=0" + sql0);
            }
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