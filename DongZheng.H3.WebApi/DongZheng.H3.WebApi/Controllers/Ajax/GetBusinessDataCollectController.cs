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
    public class GetBusinessDataCollectController : CustomController
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string DEALERNAME = context.Request["DEALERNAME"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

            int total = 0;
            ret.Add("Data", new ReturnData() { DEALERNAME = DEALERNAME }.GetJson(Page, Size, out total));
            ret.Add("Total", total);

            context.Response.Write(ret);

        }

        public class ReturnData
        {
            public class MortgageList : IComparable
            {
                public string DEALERNAME = "";
                public string RISK = "";
                public string TOW = "";
                public string PUNISH = "";
                public string ARCHIVING = "";
                public string COMPLAIN = "";
                public string UPDATETIME = "";
                public string ObjectID = "";
                public string DealerId = "";
                public string CRMDEALERID = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("DEALERNAME", DEALERNAME);
                    ret.Add("RISK", RISK);
                    ret.Add("TOW", TOW);
                    ret.Add("PUNISH", PUNISH);
                    ret.Add("ARCHIVING", ARCHIVING);
                    ret.Add("COMPLAIN", COMPLAIN);
                    ret.Add("UPDATETIME", UPDATETIME);
                    ret.Add("ObjectID", ObjectID);
                    ret.Add("DealerId", DealerId);
                    ret.Add("CRMDEALERID", CRMDEALERID);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageList o = obj as MortgageList;
                    int ret = this.DEALERNAME.CompareTo(o.DEALERNAME);
                    if (ret == 0)
                    {
                        //ret = this.ZRSCORE.CompareTo(o.ZRSCORE);
                    }
                    return ret;
                }
            }

            private List<MortgageList> products = new List<MortgageList>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string DEALERNAME = "";
            public string RISK = "";
            public string TOW = "";
            public string PUNISH = "";
            public string ARCHIVING = "";
            public string COMPLAIN = "";
            public string UPDATETIME = "";
            public string ObjectID = "";
            public string DealerId = "";
            public string CRMDEALERID = "";

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(DEALERNAME))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " DEALERNAME LIKE '%" + DEALERNAME + "%'";
                    }
                    //if (!string.IsNullOrWhiteSpace(NWWW))
                    //{
                    //    query += (query == "" ? " WHERE" : " AND") + " NWWW LIKE '%" + NWWW + "%'";
                    //}
                    //if (!string.IsNullOrWhiteSpace(KIND))
                    //{
                    //    query += (query == "" ? " WHERE" : " AND") + " KIND LIKE '%" + KIND + "%'";
                    //}
                    //if (!string.IsNullOrWhiteSpace(JXS_LEVEL))
                    //{
                    //    query += (query == "" ? " WHERE" : " AND") + " JXS_LEVEL LIKE '%" + JXS_LEVEL + "%'";
                    //}
                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select * from DZ_V_H3_BUSINESSDATACOLLECT " + query + " ORDER BY DEALERNAME ASC");

                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new MortgageList()
                            {
                                DEALERNAME = ReadCell(row, "DEALERNAME"),
                                RISK = ReadCell(row, "RISK"),
                                TOW = ReadCell(row, "TOW"),
                                PUNISH = ReadCell(row, "PUNISH"),
                                ARCHIVING = ReadCell(row, "ARCHIVING"),
                                COMPLAIN = ReadCell(row, "COMPLAIN"),
                                UPDATETIME = ReadCell(row, "UPDATETIME"),
                                ObjectID = ReadCell(row, "ObjectID"),
                                DealerId = ReadCell(row, "DealerId"),
                                CRMDEALERID = ReadCell(row, "CRMDEALERID")
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