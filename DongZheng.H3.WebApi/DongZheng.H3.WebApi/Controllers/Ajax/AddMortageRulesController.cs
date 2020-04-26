using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class AddMortageRulesController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";
        
        public string Index()
        {
            var context = HttpContext;
            System.IO.Stream s = context.Request.InputStream;
            using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
            {
                string sql = "";
                Newtonsoft.Json.Linq.JObject ret = Newtonsoft.Json.Linq.JObject.Parse(sr.ReadToEnd());
                try
                {
                    if (!string.IsNullOrEmpty(ret["ObjectID"] + ""))
                    {
                        sql = @"UPDATE C_MORTGAGERULES SET Shop='" + ret["Products"][0]["Shop"] + "',Province='" + ret["Products"][0]["Province"] + "' ,City='" + ret["Products"][0]["City"] + "',SpName='" + ret["Products"][0]["SpName"] + "',DyName='" + ret["Products"][0]["DyName"] + "' WHERE ObjectID='" + ret["ObjectID"] + "'";
                        AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                    }
                    else
                    {
                        foreach (Newtonsoft.Json.Linq.JObject token in ret["Products"])
                        {
                            sql = "SELECT * FROM C_MORTGAGERULES WHERE SHOP = '" + token["Shop"] + "' AND PROVINCE = '" + token["Province"] + "' AND CITY = '" + token["City"] + "' AND SPNAME = '" + token["SpName"] + "' AND DYNAME = '" + token["DyName"] + "'";
                            DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                            if (dt == null || dt.Rows.Count < 1)
                            {
                                sql = @"INSERT INTO C_MORTGAGERULES (ObjectID,Shop,Province,City,SpName,DyName) VALUES('" + Guid.NewGuid().ToString() + "','" + token["Shop"] + "','" + token["Province"] + "','" + token["City"] + "','" + token["SpName"] + "','" + token["DyName"] + "')";
                                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                            }
                        }
                    }
                    //context.Response.Write("Success");
                    return "Success";
                }
                catch (Exception e)
                {
                    //context.Response.Write("Failed");
                    return "Failed";
                }
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