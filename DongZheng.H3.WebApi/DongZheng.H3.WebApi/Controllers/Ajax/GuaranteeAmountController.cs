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
    public class GuaranteeAmountController : CustomController
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string DISTRIBUTOR = context.Request["DISTRIBUTOR"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            //string ObjectID = context.Request["ObjectID"] + string.Empty;
            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
            int total = 0;
            ret.Add("Data", new ReturnData() { DISTRIBUTOR = DISTRIBUTOR }.GetJson(Page, Size, out total));
            ret.Add("Total", total);
            context.Response.Write(ret);
        }

        public class ReturnData
        {
            public class MortgageList : IComparable
            {
                public string DISTRIBUTOR = "";
                public string VERIFYLOAN = "";
                public string QUOTA = "";
                public string USEQUOTA = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("DISTRIBUTOR", DISTRIBUTOR);
                    ret.Add("VERIFYLOAN", VERIFYLOAN);
                    ret.Add("QUOTA", QUOTA);
                    ret.Add("USEQUOTA", USEQUOTA);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageList o = obj as MortgageList;
                    int ret = this.DISTRIBUTOR.CompareTo(o.DISTRIBUTOR);
                    if (ret == 0)
                    {
                        //ret = this.ZRSCORE.CompareTo(o.ZRSCORE);
                    }
                    return ret;
                }
            }

            private List<MortgageList> products = new List<MortgageList>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string DISTRIBUTOR = "";
            public string VERIFYLOAN = "";
            public string QUOTA = "";
            public string USEQUOTA = "";

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(DISTRIBUTOR))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " DISTRIBUTOR LIKE '%" + DISTRIBUTOR + "%'";
                    }
                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM DZ_V_H3_GUARANTEEAMOUNT " + query);

                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new MortgageList()
                            {
                                DISTRIBUTOR = ReadCell(row, "DISTRIBUTOR"),
                                VERIFYLOAN = ReadCell(row, "VERIFYLOAN"),
                                QUOTA = ReadCell(row, "QUOTA"),
                                USEQUOTA = ReadCell(row, "USEQUOTA")
                            });
                        }
                        total++;
                    }
                    this.products.Sort();
                    foreach (MortgageList p in this.products)
                    {
                        data.Add(p.GetJson(++start));
                    }
                }
                else
                {
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