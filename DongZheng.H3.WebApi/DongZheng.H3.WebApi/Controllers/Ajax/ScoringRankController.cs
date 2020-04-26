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
    public class ScoringRankController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;

            string PropertyName = context.Request["PropertyName"];        //规则名称
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string method = context.Request["method"];
            string Grade = context.Request["Grade"];
            string RangeTo = context.Request["RangeTo"];
            string RangeFrom = context.Request["RangeFrom"];
            string Pid = context.Request["Pid"];
            string GetState = context.Request["GetState"];
            string ObjectID = context.Request["ObjectID"] + string.Empty;
            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
            int total = 0;
            if (method == "AddScoringRank")
            {
                string sql = "";
                if (string.IsNullOrEmpty(ObjectID))
                    sql = @"
INSERT into C_GRADE (ObjectID,GRADE,RANGEFROM,RANGETO,pid,STATES) 
VALUES('" + Guid.NewGuid().ToString() + "','" + Grade + "','" + RangeFrom + "','" + RangeTo + "','" + Pid + "','1')";
                else
                    sql = @"update C_GRADE set GRADE='" + Grade + "',RANGEFROM='" + RangeFrom + "',RANGETO='" + RangeTo + "' where objectID='" + ObjectID + "'";
                int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                if (i > 0)
                    context.Response.Write(1);
                else
                    context.Response.Write(0);
            }
            else if (!string.IsNullOrEmpty(GetState))
            {
                string sql = @"select concat(concat(classname,'-'),internettype) from C_RULEPROPERTY  where objectid='" + Pid + "'";
                string name = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                context.Response.Write(name);
            }
            //加载
            else
            {
                ret.Add("Data", new ReturnData() { PropertyName = PropertyName, ObjectID = ObjectID }.GetJson(Page, Size, Pid, out total));
                ret.Add("Total", total);
                context.Response.Write(ret);
            }

        }
        public class ReturnData
        {
            public class ScoringRank : IComparable
            {
                public string PropertyName = "";
                public string Grade = "";
                public string RangeFrom = "";
                public string RangeTo = "";
                public string ObjectID = "";
                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("PropertyName", PropertyName);
                    ret.Add("Grade", Grade);
                    ret.Add("RangeFrom", RangeFrom);
                    ret.Add("RangeTo", RangeTo);
                    ret.Add("ObjectID", ObjectID);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    ScoringRank o = obj as ScoringRank;
                    int ret = this.Grade.CompareTo(o.Grade);
                    if (ret == 0)
                    {
                        ret = this.RangeFrom.CompareTo(o.RangeFrom);
                    }
                    return ret;
                }
            }

            private List<ScoringRank> products = new List<ScoringRank>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string PropertyName = "";
            public string Grade = "";
            public string RangeFrom = "";
            public string RangeTo = "";
            public string ObjectID = "";

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, string pid, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "WHERE A.STATES=1 and  A.PID='" + pid + "'";
                    if (!string.IsNullOrWhiteSpace(PropertyName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ClassName LIKE '%" + PropertyName + "%'";
                    }

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    string sql = "select A.*,c.ClassName from C_GRADE A left join C_RULENAME c on A.pid=c.objectid " + query + " ORDER BY grade ASC";
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new ScoringRank()
                            {
                                PropertyName = ReadCell(row, "ClassName"),
                                Grade = ReadCell(row, "Grade"),
                                RangeFrom = ReadCell(row, "RangeFrom"),
                                RangeTo = ReadCell(row, "RangeTo"),
                                ObjectID = ReadCell(row, "ObjectID"),
                            });
                        }
                        total++;



                    }
                    this.products.Sort();
                    foreach (ScoringRank p in this.products)
                    {
                        data.Add(p.GetJson(++start));
                    }
                }
                else
                {
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select A.*,c.ClassName from C_GRADE A left join C_RULENAME c on A.pid=c.objectid  WHERE A.ObjectID LIKE '%" + ObjectID + "%'");
                    total = 0;
                    foreach (DataRow row in products.Rows)
                    {

                        this.products.Add(new ScoringRank()
                        {
                            PropertyName = ReadCell(row, "ClassName"),
                            Grade = ReadCell(row, "Grade"),
                            RangeFrom = ReadCell(row, "RangeFrom"),
                            RangeTo = ReadCell(row, "RangeTo"),
                            ObjectID = ReadCell(row, "ObjectID"),
                        });

                        total++;

                    }
                    this.products.Sort();
                    int index = 0;
                    foreach (ScoringRank p in this.products)
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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}