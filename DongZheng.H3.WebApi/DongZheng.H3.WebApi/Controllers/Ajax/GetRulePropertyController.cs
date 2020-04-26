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
    public class GetRulePropertyController : CustomController
    {
        public override string FunctionCode => "";

        System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        public void Index()
        {
            var context = HttpContext;
            string InternetType = context.Request["InternetType"];
            string ClassName = context.Request["ClassName"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string method = context.Request["method"];
            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

            int total = 0;

            if (method == "AddRuleProperty")
            {
                string sql = "";
                if (string.IsNullOrEmpty(ObjectID))
                    sql = @"
INSERT into C_RULEPROPERTY (ObjectID,InternetType,ClassName) 
VALUES('" + Guid.NewGuid().ToString() + "','" + InternetType + "','" + ClassName + "')";
                else
                    sql = @"update C_RULEPROPERTY set InternetType='" + InternetType + "',ClassName='" + ClassName + "' where objectID='" + ObjectID + "'";
                int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                if (i > 0)
                    context.Response.Write(1);
                else
                    context.Response.Write(0);
            }
            else if (method == "GetPerproty")
            {
                var dt = GetPerproty("性质");
                var dt0 = GetPerproty("渠道分类");
                ret.Add("property", dt);
                ret.Add("type", dt0);
                context.Response.Write(ret);
            }
            else
            {
                ret.Add("Data", new ReturnData() { InternetType = InternetType, ClassName = ClassName, ObjectID = ObjectID }.GetJson(Page, Size, out total));
                ret.Add("Total", total);
                context.Response.Write(ret);
            }
        }
        public class ReturnData
        {
            public class MortgageRules : IComparable
            {

                public string InternetType = "";
                public string ClassName = "";
                public string ObjectID = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("InternetType", InternetType);
                    ret.Add("ClassName", ClassName);
                    ret.Add("ObjectID", ObjectID);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageRules o = obj as MortgageRules;
                    int ret = this.InternetType.CompareTo(o.InternetType);
                    if (ret == 0)
                    {
                        ret = this.InternetType.CompareTo(o.InternetType);
                    }
                    return ret;
                }
            }

            private List<MortgageRules> products = new List<MortgageRules>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string InternetType;
            public string ClassName;
            public string ObjectID;

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "WHERE STATE=1";
                    if (!string.IsNullOrWhiteSpace(InternetType))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " InternetType LIKE '%" + InternetType + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(ClassName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ClassName LIKE '%" + ClassName + "%'";
                    }



                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select A.* from C_RULEPROPERTY A " + query + " ORDER BY CLASSNAME ASC");

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;


                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new MortgageRules()
                            {
                                InternetType = ReadCell(row, "InternetType"),
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
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_RULEPROPERTY WHERE ObjectID LIKE '%" + ObjectID + "%'");
                    total = 0;
                    foreach (DataRow row in products.Rows)
                    {

                        this.products.Add(new MortgageRules()
                        {
                            InternetType = ReadCell(row, "InternetType"),
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
        public Newtonsoft.Json.Linq.JArray GetPerproty(string id)
        {
            Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
            DataTable dt = new DataTable();
            string sql = "SELECT ENUMVALUE   from OT_EnumerableMetadata t where CATEGORY='" + id + "'";
            dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                ret.Add("ENUMVALUE", dt.Rows[i]["ENUMVALUE"].ToString());
                data.Add(ret);
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
        public struct Enumer
        {
            public string ENUMVALUE;
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