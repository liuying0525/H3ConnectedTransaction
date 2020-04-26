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
    public class GetRuleNameController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string BClassName = context.Request["BClassName"];
            string RATE = context.Request["RATE"];
            string ClassName = context.Request["ClassName"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string method = context.Request["method"];
            string Pid = context.Request["Pid"];
            string GetState = context.Request["GetState"];
            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

            int total = 0;
            //增加删除
            if (method == "AddRuleName")
            {
                string sql = "";
                if (string.IsNullOrEmpty(ObjectID))
                    sql = @"
INSERT into C_RULENAME (ObjectID,BClassName,ClassName,RATE,pid) 
VALUES('" + Guid.NewGuid().ToString() + "','" + BClassName + "','" + ClassName + "','" + RATE + "','" + Pid + "')";
                else
                    sql = @"update C_RULENAME set BClassName='" + BClassName + "',ClassName='" + ClassName + "',RATE='" + RATE + "' where objectID='" + ObjectID + "'";
                int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                if (i > 0)
                    context.Response.Write(1);
                else
                    context.Response.Write(0);
            }
            //增加的时候加载性质
            else if (!string.IsNullOrEmpty(GetState))
            {
                string sql = @"select classname from C_RULEPROPERTY  where objectid='" + Pid + "'";
                string name = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                context.Response.Write(name);
            }
            //加载
            else
            {
                ret.Add("Data", new ReturnData() { ClassName = ClassName, Rate = RATE, ObjectID = ObjectID }.GetJson(Page, Size, Pid, out total));
                ret.Add("Total", total);
                context.Response.Write(ret);
            }
        }
        public class ReturnData
        {
            public class MortgageRules : IComparable
            {

                public string Rate = "";
                public string BClassName = "";
                public string ClassName = "";
                public string ObjectID = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("Rate", Rate);
                    ret.Add("BClassName", BClassName);
                    ret.Add("ClassName", ClassName);
                    ret.Add("ObjectID", ObjectID);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageRules o = obj as MortgageRules;
                    int ret = this.BClassName.CompareTo(o.BClassName);
                    if (ret == 0)
                    {
                        ret = this.BClassName.CompareTo(o.BClassName);
                    }
                    return ret;
                }
            }

            private List<MortgageRules> products = new List<MortgageRules>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string Rate;
            public string BClassName;
            public string ClassName;
            public string ObjectID;

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, string pid, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "WHERE STATE=1 and  PID='" + pid + "'";
                    if (!string.IsNullOrWhiteSpace(ClassName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ClassName LIKE '%" + ClassName + "%'";
                    }



                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select A.* from C_RULENAME A " + query + " ORDER BY CLASSNAME ASC");

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;


                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new MortgageRules()
                            {
                                Rate = ReadCell(row, "Rate"),
                                BClassName = ReadCell(row, "BClassName"),
                                ClassName = ReadCell(row, "ClassName"),
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
                            BClassName = ReadCell(row, "BClassName"),
                            ClassName = ReadCell(row, "ClassName"),
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
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}