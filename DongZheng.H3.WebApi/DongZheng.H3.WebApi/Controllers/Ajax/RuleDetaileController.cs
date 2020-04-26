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
    public class RuleDetaileController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        public void Index()
        {
            var context = HttpContext;
            string DetaileName = context.Request["DetaileName"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string method = context.Request["method"];
            string Pid = context.Request["Pid"];
            string Cid = context.Request["Cid"];
            string GetState = context.Request["GetState"];
            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

            int total = 0;
            //增加删除
            if (!string.IsNullOrEmpty(Cid))
            {
                string sql = @"select a.rulename,a.detailename,a.rate,c.internettype,c.classname from C_RULEDETAILS a left join C_RULENAME B ON  a.pid=B.OBJECTID LEFT JOIN C_RULEPROPERTY C 
ON B.PID=C.OBJECTID  where a.objectid='" + Cid + "'";
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    string state = "0";
                    if (Pid == "check")
                    {
                        state = "1";
                    }
                    ret.Add("RuleName", dt.Rows[0]["RULENAME"].ToString());
                    ret.Add("DetaileName", dt.Rows[0]["DETAILENAME"].ToString());
                    ret.Add("Rate", dt.Rows[0]["RATE"].ToString());
                    ret.Add("Internettype", dt.Rows[0]["INTERNETTYPE"].ToString());
                    ret.Add("Classname", dt.Rows[0]["CLASSNAME"].ToString());
                    ret.Add("State", state);
                    var result = GetScore(Cid);
                    ret.Add("datas", result);
                }
                context.Response.Write(ret);
            }
            //增加的时候加载性质
            else if (!string.IsNullOrEmpty(GetState))
            {
                string sql = @"select classname from C_RULEName  where objectid='" + Pid + "'";
                string name = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                context.Response.Write(name);
            }
            //加载
            else
            {
                ret.Add("Data", new ReturnData() { DetaileName = DetaileName, ObjectID = ObjectID }.GetJson(Page, Size, Pid, out total));
                ret.Add("Total", total);
                context.Response.Write(ret);
            }
        }
        public class ReturnData
        {
            public class MortgageRules : IComparable
            {

                public string Rate = "";
                public string RuleName = "";
                public string DetaileName = "";
                public string ObjectID = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("Rate", Rate);
                    ret.Add("RuleName", RuleName);
                    ret.Add("DetaileName", DetaileName);
                    ret.Add("ObjectID", ObjectID);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageRules o = obj as MortgageRules;
                    int ret = this.RuleName.CompareTo(o.RuleName);
                    if (ret == 0)
                    {
                        ret = this.RuleName.CompareTo(o.RuleName);
                    }
                    return ret;
                }
            }

            private List<MortgageRules> products = new List<MortgageRules>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string Rate;
            public string DetaileName;
            public string RuleName;
            public string ObjectID;

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, string pid, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "WHERE STATE=1 and  PID='" + pid + "'";
                    // string query = "";
                    if (!string.IsNullOrWhiteSpace(DetaileName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " DetaileName LIKE '%" + DetaileName + "%'";
                    }



                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select A.* from C_RULEDETAILS A " + query + " ORDER BY DetaileName ASC");

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;


                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new MortgageRules()
                            {
                                Rate = ReadCell(row, "Rate"),
                                RuleName = ReadCell(row, "RuleName"),
                                DetaileName = ReadCell(row, "DetaileName"),
                                ObjectID = ReadCell(row, "ObjectID"),
                            });
                        }
                        total++;



                    }
                    this.products.Sort();
                    foreach (MortgageRules p in this.products)
                    {
                        data.Add(p.GetJson(++start));
                    }
                }

                else
                {
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_RULENAME WHERE ObjectID LIKE '%" + ObjectID + "%'");
                    total = 0;
                    foreach (DataRow row in products.Rows)
                    {

                        this.products.Add(new MortgageRules()
                        {
                            Rate = ReadCell(row, "Rate"),
                            RuleName = ReadCell(row, "RuleName"),
                            DetaileName = ReadCell(row, "DetaileName"),
                            ObjectID = ReadCell(row, "ObjectID"),
                        });

                        total++;

                    }
                    this.products.Sort();
                    int index = 0;
                    foreach (MortgageRules p in this.products)
                    {
                        data.Add(p.GetJson(++index));
                    }


                }



                return data;
            }

            public string ReadCell(DataRow row, string column)
            {
                object value = row[column];
                if (value == null || value == DBNull.Value)
                {
                    return "";
                }
                else
                {
                    return value.ToString();
                }
            }

            public DataTable ExecuteDataTableSql(string connectionCode, string sql)
            {
                try
                {
                    var dbObject = AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
                    if (dbObject != null)
                    {
                        OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                        var command = factory.CreateCommand();
                        return command.ExecuteDataTable(sql);
                    }
                    return null;
                }
                catch (Exception ex)
                {
                    AppUtility.Engine.LogWriter.Write("ExecuteDataTableSql Exception:connectionCode-->" + connectionCode + ",sql-->" + sql + "," + ex);
                    return null;
                }
            }

        }
        public struct Score
        {
            public string score;
            public string memo;
        }
        public Newtonsoft.Json.Linq.JObject GetJson(Score score)
        {
            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
            ret.Add("Score", score.score);
            ret.Add("Memo", score.memo);
            return ret;
        }
        public Newtonsoft.Json.Linq.JArray GetScore(string id)
        {
            Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
            string sql = "select distinct SCORE,MEMO   from C_SCORE t where PID='" + id + "' ORDER BY SCORE DESC";


            System.Data.DataTable mytable = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            List<Score> list = new List<Score>();

            if (mytable.Rows.Count > 0)
            {

                for (int i = 0; i < mytable.Rows.Count; i++)
                {
                    Score score = new Score();
                    score.score = mytable.Rows[i]["SCORE"].ToString();
                    score.memo = mytable.Rows[i]["MEMO"].ToString();
                    data.Add(GetJson(score));
                }
            }
            return data;


        }
        private System.Web.Script.Serialization.JavaScriptSerializer JSSerializer
        {
            get
            {
                if (jsSerializer == null)
                {
                    jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                }
                return jsSerializer;
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