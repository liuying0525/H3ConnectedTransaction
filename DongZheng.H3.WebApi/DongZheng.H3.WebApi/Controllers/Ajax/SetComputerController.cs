using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class SetComputerController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string id = context.Request["id"] + string.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                string sql = string.Format("select OBJECTID,DETAILENAME from C_RULEDETAILS where PID='{0}'and state=1 ", id);
                var data = GetJson(sql);
                sql = string.Format("select EXPRESSION from C_RuleExpression where RULENAMEID='{0}'", id);
                var dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                string expStr = "";
                if (dt.Rows.Count > 0)
                {
                    expStr = dt.Rows[0]["EXPRESSION"] + string.Empty;
                }
                expStr = expStr.Replace('*', '×');
                expStr = expStr.Replace('/', '÷');

                //var exp = GetEXP(sql, out expStr);
                ret.Add("data", data);
                ret.Add("expression", expStr);
                //ret.Add("exp", exp);
                context.Response.Write(ret);
            }
            else
            {
                System.IO.Stream s = context.Request.InputStream;
                int i = 0;
                using (System.IO.StreamReader sr = new System.IO.StreamReader(s))
                {
                    var t = sr.ReadToEnd();
                    //Expersion exp = JsonConvert.DeserializeObject<Expersion>(t);
                    Newtonsoft.Json.Linq.JObject ret = Newtonsoft.Json.Linq.JObject.Parse(t);
                    string expe = ret["exp"] + string.Empty;
                    id = ret["id"] + string.Empty;

                    expe = expe.Replace('×', '*');
                    expe = expe.Replace('÷', '/');

                    string sql = string.Format("select count(1) from C_RuleExpression where RULENAMEID='{0}'", id);
                    object sqlResult = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql);
                    if (sqlResult != null && sqlResult != DBNull.Value && int.TryParse(sqlResult.ToString(), out i))
                    {
                        if (i > 0)
                            sql = string.Format("update C_RuleExpression set EXPRESSION='{0}' where RULENAMEID='{1}'", expe, id);
                        else
                            sql = string.Format("insert into C_RuleExpression(objectid,RULENAMEID,EXPRESSION) values('{0}','{1}','{2}')", Guid.NewGuid().ToString(), id, expe);
                    }
                    i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);

                }
                if (i > 0)
                    context.Response.Write("success");
                else
                    context.Response.Write("failed");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        public Newtonsoft.Json.Linq.JArray GetJson(string sql)
        {
            Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
            var dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("id", dt.Rows[i]["OBJECTID"] + string.Empty);
                    ret.Add("name", dt.Rows[i]["DETAILENAME"] + string.Empty);

                    data.Add(ret);
                }
            }
            return data;
        }
        public Newtonsoft.Json.Linq.JArray GetEXP(string sql, out string outStr)
        {
            outStr = "";
            Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
            var dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            if (dt.Rows.Count > 0)
            {
                //提取公式内{}内容获取规则明细名称
                string exp = dt.Rows[0]["EXPRESSION"] + string.Empty;
                exp.Replace('×', '*');
                exp.Replace('÷', '/');
                outStr = exp;
                MatchCollection mc = Regex.Matches(exp, @"{([^}]+)}");
                foreach (var item in mc)
                {
                    string s = item.ToString();
                    string s0 = s.Substring(1, s.Length - 2);
                    string Namesql = string.Format("select DETAILENAME from C_RULEDETAILS where objectid='{0}' ", s0);
                    var name = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(Namesql) + string.Empty;
                    string s1 = "<button style='margin:1px' value='" + s0 + "'>" + name + "</button>";
                    exp = exp.Replace(s, s1);

                }
                Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                ret.Add("exp", exp);
                data.Add(ret);
            }
            return data;
        }
        public struct Expersion
        {
            public string id;
            public string exp;
        }
    }
}