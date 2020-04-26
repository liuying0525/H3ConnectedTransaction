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
    public class MortgageListController : CustomController
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            string ApplyNo = context.Request["ApplyNo"];
            string CustomerName = context.Request["CustomerName"];
            string Saletor = context.Request["Saletor"];
            string CarType = context.Request["CarType"];
            string FkState = context.Request["FkState"];
            string DyState = context.Request["DyState"];
            string GdTime = context.Request["GdTime"];
            string StartTime = context.Request["StartTime"];
            string EndTime = context.Request["EndTime"];
            string Page = context.Request["Page"];
            string Size = context.Request["Size"];
            OThinker.H3.Controllers.UserValidator User = context.Session[OThinker.H3.Controllers.Sessions.GetUserValidator()] as OThinker.H3.Controllers.UserValidator;
            string userCode = User.UserCode;
            string ObjectID = context.Request["ObjectID"] + string.Empty;

            Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
            int total = 0;
            ret.Add("Data", new ReturnData() { ApplyNo = ApplyNo, GdTime = GdTime, FkTime = StartTime, DyTime = EndTime, CustomerName = CustomerName, Saletor = Saletor, CarType = CarType, FkState = FkState, DyState = DyState, userCode = userCode }.GetJson(Page, Size, out total));
            ret.Add("Total", total);

            context.Response.Write(ret);

        }
        public class ReturnData
        {
            public class MortgageList : IComparable
            {
                public string ApplyNo = "";
                public string ContractNo = "";
                public string SaleNo = "";
                public string producer = "";
                public string CarType = "";
                public string CustomerName = "";
                public string DqTime = "";
                public string FkTime = "";
                public string DyTime = "";
                public string GdTime = "";
                public string Saletor = "";
                public string UserCode = "";

                public Newtonsoft.Json.Linq.JObject GetJson(int index)
                {
                    Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                    ret.Add("No", index);
                    ret.Add("ApplyNo", ApplyNo);
                    ret.Add("SaleNo", SaleNo);
                    ret.Add("ContractNo", ContractNo);
                    ret.Add("producer", producer);
                    ret.Add("CarType", CarType);
                    ret.Add("CustomerName", CustomerName);
                    ret.Add("DqTime", DqTime);
                    ret.Add("FkTime", FkTime);
                    ret.Add("DyTime", DyTime);
                    ret.Add("GdTime", GdTime);
                    ret.Add("Saletor", Saletor);
                    ret.Add("UserCode", UserCode);
                    return ret;
                }

                public int CompareTo(object obj)
                {
                    MortgageList o = obj as MortgageList;
                    int ret = this.ApplyNo.CompareTo(o.ApplyNo);
                    if (ret == 0)
                    {
                        ret = this.ContractNo.CompareTo(o.ContractNo);
                    }
                    return ret;
                }
            }

            private List<MortgageList> products = new List<MortgageList>();

            private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

            public string ApplyNo = "";
            public string ContractNo = "";
            public string SaleNo = "";
            public string producer = "";
            public string CarType = "";
            public string FkState = "";
            public string DyState = "";
            public string CustomerName = "";
            public string DqTime = "";
            public string FkTime = "";
            public string DyTime = "";
            public string GdTime = "";
            public string Saletor = "";
            public string userCode = "";

            public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, out int total)
            {
                total = 0;
                Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();
                if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
                {
                    if (string.IsNullOrEmpty(DyState)) DyState = "未抵押";
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(ApplyNo))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " 申请号 LIKE '%" + ApplyNo + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(GdTime))
                    {
                        if (GdTime == "未归档")
                            query += (query == "" ? " WHERE" : " AND") + " 归档时间 is null";
                        else
                            query += (query == "" ? " WHERE" : " AND") + " 归档时间 is not null";
                    }
                    if (!string.IsNullOrWhiteSpace(FkTime))//放款时间
                    {
                        query += (query == "" ? " WHERE" : " AND") + " to_date(case when trim(合同激活日期) is null then '2000-10-10' else trim(合同激活日期) end, 'yyyy/MM/dd') >=to_date('" + FkTime + "','yyyy/MM/dd') ";
                    }
                    if (!string.IsNullOrWhiteSpace(DyTime))//抵押时间
                    {
                        query += (query == "" ? " WHERE" : " AND") + " to_date(case when trim(合同激活日期) is null then '2000-10-10' else trim(合同激活日期) end, 'yyyy/MM/dd') <=to_date('" + DyTime + "','yyyy/MM/dd') ";
                    }
                    if (!string.IsNullOrWhiteSpace(CustomerName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " 客户名称 LIKE '%" + CustomerName + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(Saletor))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " 经销商名称 LIKE '%" + Saletor + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(CarType))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " 贷款车型 LIKE '%" + CarType + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(userCode) && userCode != "Administrator")
                    {
                        query += (query == " " ? " WHERE" : " AND") + " 经营店帐号 LIKE '%" + userCode + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(FkState))
                    {
                        if (FkState == "已放款")
                            query += (query == "" ? " WHERE" : " AND") + " 合同号 IS NOT NULL ";
                        else
                            query += (query == "" ? " WHERE" : " AND") + " 合同号 IS  NULL ";
                    }
                    if (!string.IsNullOrWhiteSpace(DyState))
                    {
                        //string applylist = "";
                        //if (!string.IsNullOrEmpty(ApplyNo))
                        //{
                        //    applylist = ApplyNo;
                        //}
                        //else
                        //{
                        //    applylist = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT WM_CONCAT(DISTINCT ApplyNo) FROM I_Mortgage") + string.Empty;
                        //}
                        //string[] tempapply = applylist.Split(',');
                        //string strapplyno = "";
                        //for (int i = 0; i < tempapply.Length; i++)
                        //{
                        //    if (string.IsNullOrEmpty(tempapply[i])) continue;
                        //    strapplyno += "'" + tempapply[i] + "',";
                        //}
                        //strapplyno = strapplyno.TrimEnd(',');
                        //strapplyno = "'" + applylist.Replace(",","','") + "'";
                        //AppUtility.Engine.LogWriter.Write("DyState-->" + DyState);
                        //if (DyState == "已抵押")
                        //{
                        //    if (string.IsNullOrEmpty(ApplyNo))
                        //        query += (query == "" ? " WHERE " : " AND") + (string.IsNullOrEmpty(strapplyno) ? " 1=2 AND" : " 申请号 IN (" + strapplyno + ") AND") + " 抵押登记日期 IS NOT NULL";  //AND (抵押登记日期 is not null and 抵押登记日期 <> '')
                        //                                                                                                                                                                     //else if (!string.IsNullOrEmpty(ApplyNo))
                        //                                                                                                                                                                     //    query += (query == "" ? " WHERE " : " AND") + " 抵押登记日期 IS NOT NULL";
                        //    AppUtility.Engine.LogWriter.Write("query -->" + query );
                        //}
                        //if (DyState == "未抵押")
                        //{
                        //    if (!string.IsNullOrEmpty(ApplyNo))
                        //        query += (query == "" ? " WHERE" : " AND") + " 抵押登记日期 IS NULL OR 抵押登记日期 = ''";
                        //    if (!string.IsNullOrEmpty(strapplyno))
                        //        query += (query == "" ? " WHERE" : " AND") + (string.IsNullOrEmpty(strapplyno) ? "" : " 申请号 NOT IN (" + strapplyno + ") AND") + " 抵押登记日期 IS NULL ";    // AND (抵押登记日期 is null and 抵押登记日期 = '')
                        //    //else
                        //    //    query += (query == "" ? " WHERE" : " AND") + " 抵押登记日期 IS NULL OR 抵押登记日期 = ''";
                        //}
                        //string strapplyno = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT WM_CONCAT(DISTINCT ApplyNo) FROM I_Mortgage") + string.Empty;
                        if (DyState == "已抵押")
                        {
                            query += (query == "" ? " WHERE " : " AND ") + " 抵押登记日期 IS NOT NULL";
                        }
                        if (DyState == "未抵押")
                        {
                            //query += (query == "" ? " WHERE " : " AND ") + " 抵押登记日期 IS NULL AND 申请号 NOT IN ('1','" + strapplyno.Replace(",", "','") + "')";
                            query += (query == "" ? " WHERE " : " AND ") + " 抵押登记日期 IS NULL AND 申请号 NOT IN (SELECT DISTINCT ApplyNo FROM I_Mortgage WHERE APPLYNO IS NOT NULL)";
                        }
                        if (DyState == "抵押中")
                        {
                            //query += (query == "" ? " WHERE " : " AND ") + " 抵押登记日期 IS NULL AND 申请号 IN ('1','" + strapplyno.Replace(",", "','") + "') ";
                            query += (query == "" ? " WHERE " : " AND ") + " 抵押登记日期 IS NULL AND 申请号 IN (SELECT DISTINCT ApplyNo FROM I_Mortgage WHERE APPLYNO IS NOT NULL)";
                        }
                        AppUtility.Engine.LogWriter.Write("query -->" + query);
                    }
                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    //System.Data.DataTable products = this.ExecuteDataTableSql("CAPDB", "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY \"申请号\" ASC) i,COUNT(1) OVER() t,s.* FROM in_cms.mv_dy_application_date_info@to_dy_cms s " + query + ") tmp  WHERE i BETWEEN " + start + " AND " + (start + _size) + " ORDER BY \"申请号\" ASC");
                    string sql = "SELECT * FROM (SELECT ROW_NUMBER() OVER(ORDER BY \"申请号\" ASC) i,COUNT(1) OVER() t,s.* FROM in_cms.mv_dy_application_date_info@to_dy_cms s " +
                                 query + ") tmp  WHERE i BETWEEN " + start + " AND " + (start + _size) + " ORDER BY \"申请号\" ASC";
                    DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                    if (products != null && products.Rows.Count > 0)
                    {
                        foreach (DataRow row in products.Rows)
                        {
                            if (this.products.Count >= _size) continue;
                            this.products.Add(new MortgageList()
                            {
                                ApplyNo = ReadCell(row, "申请号"),
                                ContractNo = ReadCell(row, "合同号"),
                                SaleNo = ReadCell(row, "经营店帐号"),
                                producer = ReadCell(row, "制造商"),
                                CarType = ReadCell(row, "贷款车型"),
                                CustomerName = ReadCell(row, "客户名称"),
                                DqTime = ReadCell(row, "合同到期日"),
                                FkTime = ReadCell(row, "合同激活日期"),
                                DyTime = ReadCell(row, "抵押登记日期"),
                                GdTime = ReadCell(row, "归档时间"),
                                Saletor = ReadCell(row, "经销商名称"),
                            });
                        }
                    }
                    if (products != null && products.Rows.Count > 0) total = Convert.ToInt32(products.Rows[0]["t"]) + 1;
                    this.products.Sort();
                    foreach (MortgageList p in this.products)
                    {
                        data.Add(p.GetJson(++start));
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