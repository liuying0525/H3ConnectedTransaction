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
    public class DelMortageRulesController : CustomController
    {
        public override string FunctionCode => "";


        public void Index()
        {
            var context = HttpContext;
            System.IO.Stream s = context.Request.InputStream;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
            {
                string sql = "";
                Newtonsoft.Json.Linq.JArray ret = Newtonsoft.Json.Linq.JArray.Parse(sr.ReadToEnd());
                foreach (Newtonsoft.Json.Linq.JObject item in ret)
                {
                    sql += (sql == "" ? " WHERE " : " OR ") + "(ObjectID='" + item["ObjectID"] + "' )";
                }
                if (!string.IsNullOrWhiteSpace(sql))
                    AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM C_MORTGAGERULES" + sql);
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