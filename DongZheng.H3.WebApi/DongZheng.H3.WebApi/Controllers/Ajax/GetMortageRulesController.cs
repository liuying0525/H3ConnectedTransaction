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
    public class GetMortageRulesController : CustomController
    {
        public override string FunctionCode => "";
        System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = null;
        public void Index()
        {
            var context = HttpContext;
            string Shop = context.Request["Shop"];
            string Province = context.Request["Province"];
            string City = context.Request["City"];
            string SpName = context.Request["SpName"];
            string DyName = context.Request["DyName"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            string method = context.Request["method"];
            string pid = context.Request["id"];

            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

            int total = 0;
            ret.Add("Data", new ReturnData() { Shop = Shop, Province = Province, City = City, SpName = SpName, DyName = DyName, ObjectID = ObjectID }.GetJson(Page, Size, out total));
            ret.Add("Total", total);
            if (method == "GetProvince")
            {
                var result = GetArea(string.IsNullOrEmpty(pid) ? "" : pid);
                context.Response.Write(JSSerializer.Serialize(result));
            }
            if (method == "GetCity" && !string.IsNullOrEmpty(pid))
            {
                var result = GetArea(pid);
                context.Response.Write(JSSerializer.Serialize(result));
            }
            if (method == "GetPeople")
            {
                var result = GetPeople();
                context.Response.Write(JSSerializer.Serialize(result));
            }
            if (method == "Data")
            {
                context.Response.Write(ret);
            }
        }


        public class ReturnData
        {
            public class MortgageRules : IComparable
            {
                public int ProductId = 0;
                public string Shop = "";
                public string Province = "";
                public string Sheng = "";
                public string City = "";
                public string Shi = "";
                public string SpName = "";
                public string DyName = "";
                public string DyRealName = "";
                public string SpRealName = "";
                public string ObjectID = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("Shop", Shop);
                    ret.Add("Province", Province);
                    ret.Add("Sheng", Sheng);
                    ret.Add("City", City);
                    ret.Add("Shi", Shi);
                    ret.Add("SpName", SpName);
                    ret.Add("DyName", DyName);
                    ret.Add("DyRealName", DyRealName);
                    ret.Add("SpRealName", SpRealName);
                    ret.Add("ObjectID", ObjectID);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageRules o = obj as MortgageRules;
                    int ret = this.Shop.CompareTo(o.Shop);
                    if (ret == 0)
                    {
                        ret = this.ProductId.CompareTo(o.ProductId);
                    }
                    return ret;
                }
            }

            private List<MortgageRules> products = new List<MortgageRules>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string Shop;
            public string Province;
            public string Sheng = "";
            public string City;
            public string Shi = "";
            public string SpName;
            public string DyName;
            public string DyRealName;
            public string SpRealName;
            public string ObjectID;

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(Shop))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " Shop LIKE '%" + Shop + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(Province))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " Province LIKE '%" + Province + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(City))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " City LIKE '%" + City + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(SpName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " e.name LIKE '%" + SpName + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(DyName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " d.name LIKE '%" + DyName + "%'";
                    }


                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT A.*,B.CODENAME AS SHENG,C.CODENAME AS SHI,D.NAME AS DYREALNAME,e.name AS SPREALNAME FROM C_MORTGAGERULES A left join AREA B ON A.PROVINCE=B.CODEID LEFT JOIN AREA C ON A.CITY=C.CODEID left join ot_user d on A.DYNAME=D.OBJECTID LEFT JOIN OT_USER E ON A.SPNAME=E.OBJECTID " + query + " ORDER BY Shop ASC");

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;


                    foreach (DataRow row in products.Rows)
                    {
                        if (total >= start && this.products.Count < _size)
                        {
                            this.products.Add(new MortgageRules()
                            {
                                Shop = ReadCell(row, "Shop"),
                                Province = ReadCell(row, "Province"),
                                City = ReadCell(row, "City"),
                                SpName = ReadCell(row, "SpName"),
                                DyName = ReadCell(row, "DyName"),
                                ObjectID = ReadCell(row, "ObjectID"),
                                Sheng = ReadCell(row, "Sheng"),
                                Shi = ReadCell(row, "Shi"),
                                DyRealName = ReadCell(row, "DyRealName"),
                                SpRealName = ReadCell(row, "SpRealName"),

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
                    if (!string.IsNullOrEmpty(ObjectID))
                    {
                        System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_MORTGAGERULES WHERE ObjectID LIKE '%" + ObjectID + "%'");
                        total = 0;
                        foreach (DataRow row in products.Rows)
                        {
                            this.products.Add(new MortgageRules()
                            {
                                Shop = ReadCell(row, "Shop"),
                                Province = ReadCell(row, "Province"),
                                City = ReadCell(row, "City"),
                                SpName = ReadCell(row, "SpName"),
                                DyName = ReadCell(row, "DyName"),
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
        public List<area> GetArea(string id)
        {
            string sql = "";
            if (string.IsNullOrEmpty(id))
                sql = "SELECT DISTINCT CODEID,CODENAME FROM AREA WHERE PARENTID=100000 AND SFDY = '1'";
            else
                sql = "SELECT DISTINCT CODEID,CODENAME FROM AREA WHERE SFDY = '1' AND PARENTID = " + id + "";
            System.Data.DataTable mytable = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
            List<area> list = new List<area>();

            if (mytable.Rows.Count > 0)
            {

                for (int i = 0; i < mytable.Rows.Count; i++)
                {
                    area Area = new area();
                    Area.code = mytable.Rows[i]["CodeID"].ToString();
                    Area.codeName = mytable.Rows[i]["CODENAME"].ToString();
                    list.Add(Area);
                }
            }
            return list;


        }
        public List<people>[] GetPeople()
        {

            //抵押员
            var names0 = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetChildren("59a20a1d-823d-44d8-995f-9ae9b53a0a8e", OThinker.Organization.UnitType.User, false, OThinker.Organization.State.Active);
            //var names1 = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetChildren("f5d6e797-fe79-46ae-aaef-088e34769d07", OThinker.Organization.UnitType.User,false,OThinker.Organization.State.Active);
            var names1 = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetChildren("59a20a1d-823d-44d8-995f-9ae9b53a0a8e", OThinker.Organization.UnitType.User, false, OThinker.Organization.State.Active);


            Dictionary<int, List<people>> dc = new Dictionary<int, List<people>>();

            List<people>[] list = new List<people>[2];
            people[] array = new people[2];

            if (names0.Count > 0)
            {
                list[0] = new List<people>();
                for (int i = 0; i < names0.Count; i++)
                {
                    people people = new people();
                    people.id = names0[i];
                    people.Name = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetName(names0[i]);
                    list[0].Add(people);
                }
            }


            if (names1.Count > 0)
            {
                list[1] = new List<people>();
                for (int i = 0; i < names1.Count; i++)
                {
                    people people = new people();
                    people.id = names1[i];
                    people.Name = OThinker.H3.Controllers.AppUtility.Engine.Organization.GetName(names1[i]);
                    list[1].Add(people);
                }
            }
            return list;
        }
        public struct area
        {
            public string code;
            public string codeName;
        }
        public struct people
        {
            public string id;
            public string Name;
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