using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class AddRuleDetaileController : OThinker.H3.Controllers.ControllerBase
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
                    if (string.IsNullOrEmpty(ret["Cid"] + ""))
                    {
                        string id = Guid.NewGuid().ToString();
                        sql = @"
INSERT into C_RULEDETAILS (ObjectID,RULENAME,DETAILENAME,RATE,PID) 
VALUES('" + id + "','" + ret["RuleName"] + "','" + ret["DetaileName"] + "','" + ret["Rate"] + "','" + ret["ObjectID"] + "')";
                        int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                        if (i > 0)
                        {
                            foreach (Newtonsoft.Json.Linq.JObject token in ret["Products"])
                            {
                                sql = @"
INSERT into C_SCORE (ObjectID,SCORE,memo,pid) 
VALUES('" + Guid.NewGuid().ToString() + "','" + token["Score"] + "','" + token["Memo"] + "','" + id + "')";
                                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                            }
                        }
                    }
                    else
                    {
                        sql = @"UPDATE C_RULEDETAILS SET RULENAME='" + ret["RuleName"] + "',DETAILENAME='" + ret["DetaileName"] + "',RATE='" + ret["Rate"] + "' WHERE OBJECTID='" + ret["Cid"] + "'";
                        int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                        if (i > 0)
                        {
                            sql = @"DELETE FROM C_SCORE WHERE PID='" + ret["Cid"] + "'";
                            AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                            foreach (Newtonsoft.Json.Linq.JObject token in ret["Products"])
                            {
                                sql = @"
INSERT into C_SCORE (ObjectID,SCORE,memo,pid) 
VALUES('" + Guid.NewGuid().ToString() + "','" + token["Score"] + "','" + token["Memo"] + "','" + ret["Cid"] + "')";
                                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                            }
                        }

                    }
                    //context.Response.Write("Success");
                    return "Success";
                }
                catch (Exception ex)
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