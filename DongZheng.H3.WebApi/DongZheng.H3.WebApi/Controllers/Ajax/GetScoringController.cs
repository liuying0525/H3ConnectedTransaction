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
    public class GetScoringController :CustomController
    {
        public override string FunctionCode => "";


        public void Index()
        {
            var context = HttpContext;
            string JXSMC = context.Request["JXSMC"];
            string NWWW = context.Request["NWWW"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string kind = context.Request["KIND"];
            string level = context.Request["JXS_LEVEL"];
            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

            int total = 0;
            ret.Add("Data", new ReturnData() { JXSMC = JXSMC, NWWW = NWWW, KIND = kind, JXS_LEVEL = level }.GetJson(Page, Size, out total));
            ret.Add("Total", total);

            context.Response.Write(ret);

        }

        public class ReturnData
        {
            public class MortgageList : IComparable
            {
                public string JXSMC = "";
                public string NWWW = "";
                public string ZRSCORE = "";
                public string DHSCORE = "";
                public string ZHSCORE = "";
                public string JXS_LEVEL = "";
                public string KIND = "";
                public string Year = "";
                public string Month = "";
                public string Day = "";
                public string Link = "";
                public string DealerId = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("JXSMC", JXSMC);
                    ret.Add("NWWW", NWWW);
                    ret.Add("ZRSCORE", ZRSCORE);
                    ret.Add("DHSCORE", DHSCORE);
                    ret.Add("ZHSCORE", ZHSCORE);
                    ret.Add("JXS_LEVEL", JXS_LEVEL);
                    ret.Add("KIND", KIND);
                    ret.Add("Year", Year);
                    ret.Add("Month", Month);
                    ret.Add("Day", Day);
                    ret.Add("Link", Link);
                    ret.Add("DealerId", DealerId);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageList o = obj as MortgageList;
                    int ret = this.JXSMC.CompareTo(o.JXSMC);
                    if (ret == 0)
                    {
                        ret = this.ZRSCORE.CompareTo(o.ZRSCORE);
                    }
                    return ret;
                }
            }

            private List<MortgageList> products = new List<MortgageList>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string JXSMC = "";
            public string NWWW = "";
            public string ZRSCORE = "";
            public string DHSCORE = "";
            public string ZHSCORE = "";
            public string JXS_LEVEL = "";
            public string KIND = "";
            public string Year = "";
            public string Month = "";
            public string Day = "";
            public string Link = "";

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(JXSMC))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " JXSMC LIKE '%" + JXSMC + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(NWWW))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " NWWW LIKE '%" + NWWW + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(KIND))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " KIND LIKE '%" + KIND + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(JXS_LEVEL))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " JXS_LEVEL LIKE '%" + JXS_LEVEL + "%'";
                    }

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("select A.* from C_JXSMC A " + query + " ORDER BY JXSMC ASC");

                    string where = "";
                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            where += string.Format("'{0}',", ReadCell(row, "CrmDealerId"));
                            this.products.Add(new MortgageList()
                            {
                                JXSMC = ReadCell(row, "JXSMC"),
                                NWWW = ReadCell(row, "NWWW"),
                                ZRSCORE = ReadCell(row, "ZRSCORE"),
                                DHSCORE = ReadCell(row, "DHSCORE"),
                                ZHSCORE = ReadCell(row, "ZHSCORE"),
                                JXS_LEVEL = ReadCell(row, "JXS_LEVEL"),
                                KIND = ReadCell(row, "KIND"),
                                Year = ReadCell(row, "Year"),
                                Month = ReadCell(row, "Month"),
                                Day = ReadCell(row, "Day"),
                                DealerId = ReadCell(row, "CrmDealerId"),
                                Link = "window.open('InstanceSheets.html?InstanceId={link}')",
                            });
                        }
                        total++;
                    }
                    if (!string.IsNullOrWhiteSpace(where))
                    {
                        DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT A.OBJECTID,B.FinancialCode FROM OT_INSTANCECONTEXT A INNER JOIN I_ALLOWIN B ON A.BIZOBJECTID=B.OBJECTID AND B.FinancialCode IN (" + where.TrimEnd(',') + ")");
                        foreach (var item in this.products)
                        {
                            string objectId = null;
                            foreach (DataRow row in dt.Rows)
                            {
                                if (ReadCell(row, "FinancialCode") == item.DealerId)
                                {
                                    objectId = ReadCell(row, "OBJECTID");
                                    break;
                                }
                            }
                            if (string.IsNullOrWhiteSpace(objectId))
                            {
                                item.Link = "alert('未找到来源流程.')";
                            }
                            else
                            {
                                item.Link = item.Link.Replace("{link}", objectId);
                            }
                        }
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