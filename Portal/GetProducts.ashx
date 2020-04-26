<%@ WebHandler Language="C#" Class="GetProducts" %>

using System;
using System.Web;
using System.IO;
using System.Data;
using System.Collections.Generic;
using OThinker.H3;
using System.Text;
using System.Web.Mvc;
using OThinker.H3.Controllers;
using System.Web.SessionState;
using Newtonsoft.Json;
using System.Configuration;
using System.Linq;

public class GetProducts : MvcPage, IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    public void ProcessRequest(HttpContext context)
    {
        string ProductName = context.Request["ProductName"];
        string ProductAlias = context.Request["ProductAlias"];
        string Description = context.Request["Description"];
        string Dealer = context.Request["Dealer"];
        string State = context.Request["State"];
        string Page = context.Request["Page"];
        string Size = context.Request["Size"];
        string isBase = context.Request["IsBase"];

        Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();

        int total = 0;
        ret.Add("Data", new ReturnData() { State = State, Dealer = Dealer, Description = Description, ProductAlias = ProductAlias, ProductName = ProductName }.GetJson(Page, Size, isBase, out total));
        ret.Add("Total", total);

        context.Response.Write(ret);
    }

    public class ReturnData
    {
        public class Product : IComparable
        {
            public int ProductId = 0;
            public string ProductName = "";
            public string ProductAlias = "";
            public string Dealer = "";
            public string State = "";
            public string Description = "";

            public Newtonsoft.Json.Linq.JObject GetJson(int index)
            {
                Newtonsoft.Json.Linq.JObject ret = new Newtonsoft.Json.Linq.JObject();
                ret.Add("No", index);
                ret.Add("Dealer", Dealer);
                ret.Add("ProductName", ProductName);
                ret.Add("State", State);
                ret.Add("ProductAlias", ProductAlias);
                ret.Add("Description", Description);
                ret.Add("ProductId", ProductId);
                return ret;
            }

            public int CompareTo(object obj)
            {
                Product o = obj as Product;
                int ret = this.Dealer.CompareTo(o.Dealer);
                if (ret == 0)
                {
                    ret = this.ProductId.CompareTo(o.ProductId);
                }
                return ret;
            }
        }

        private List<Product> products = new List<Product>();

        private Dictionary<int, string> baseProducts = new Dictionary<int, string>();

        public string ProductName;
        public string ProductAlias;
        public string Dealer;
        public string State;
        public string Description;

        public Newtonsoft.Json.Linq.JArray GetJson(string page, string size, string isBase, out int total)
        {
            Newtonsoft.Json.Linq.JArray data = new Newtonsoft.Json.Linq.JArray();

            #region 查询列表页面
            if (!string.IsNullOrWhiteSpace(page) && !string.IsNullOrWhiteSpace(size))
            {

                #region 查询所有，但不包括没有别名的
                if (!string.IsNullOrWhiteSpace(isBase))
                {
                    string DealerCode = "";
                    string DealerName = "";

                    if (!string.IsNullOrWhiteSpace(Dealer))
                    {
                        System.Data.DataTable Users = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT Code,Appellation FROM OT_USER WHERE Appellation LIKE '%" + Dealer + "%'");
                        foreach (DataRow row in Users.Rows)
                        {
                            DealerCode += "'" + ReadCell(row, "Code") + "',";
                            DealerName += "'" + ReadCell(row, "APPELLATION") + "',";
                        }
                        if (string.IsNullOrWhiteSpace(DealerCode) || string.IsNullOrWhiteSpace(DealerName))
                        {
                            total = 0;
                            return data;
                        }
                    }
                    DealerCode = DealerCode.TrimEnd(',');
                    DealerName = DealerName.TrimEnd(',');
                    System.Data.DataTable allProducts = this.ExecuteDataTableSql("CAPDB", @"SELECT distinct 
FD.FINANCIAL_PRODUCT_ID ProductID,FP.FINANCIAL_PRODUCT_NME ProductName
FROM 
FP_DEALER@to_cms FD 
JOIN FINANCIAL_PRODUCT@to_cms FP ON FD.FINANCIAL_PRODUCT_ID=FP.FINANCIAL_PRODUCT_ID
JOIN 
(SELECT BSU.WORKFLOW_LOGIN_NME , BM2.BUSINESS_PARTNER_ID ,BM2.BUSINESS_PARTNER_NME 
FROM BP_SYS_USER@to_cms BSU 
JOIN BP_MAIN@to_cms BM ON BSU.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID 
JOIN BP_RELATIONSHIP@to_cms BRE ON BRE.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID AND BRE.RELATIONSHIP_CDE='00112'
JOIN BP_MAIN@to_cms BM2 ON BM2.BUSINESS_PARTNER_ID=BRE.BP_PRIMARY_ID
WHERE  BSU.WORKFLOWLOGIN_ACTIVE_IND='T' ) 
WORKFLOW_LOGIN ON 
WORKFLOW_LOGIN.BUSINESS_PARTNER_ID=FD.BUSINESS_PARTNER_ID
WHERE to_char(FP.VALID_FROM_DTE,'yyyymmdd')< to_char(sysdate,'yyyymmdd') and to_char(FP.VALID_TO_DTE,'yyyymmdd')>to_char(sysdate,'yyyymmdd')"
        + (string.IsNullOrWhiteSpace(Dealer) ? "" : " and WORKFLOW_LOGIN.WORKFLOW_LOGIN_NME IN (" + DealerCode + ")")
        + (string.IsNullOrWhiteSpace(ProductName) ? "" : " AND FP.FINANCIAL_PRODUCT_NME LIKE '%" + ProductName + "%'")
        + " ORDER BY FD.FINANCIAL_PRODUCT_ID ASC");
                    if (allProducts != null)
                        foreach (DataRow row in allProducts.Rows)
                        {
                            baseProducts.Add(int.Parse(ReadCell(row, "ProductID")), ReadCell(row, "ProductName"));
                        }
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(ProductName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ProductName LIKE '%" + ProductName + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(ProductAlias))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ProductAlias LIKE '%" + ProductAlias + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(Description))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ProductDescription LIKE '%" + Description + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(Dealer))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " Dealer IN (" + DealerName + ")";
                    }

                    string sql = "";
                    string countSql = "";
                    total = 0;
                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    if (string.IsNullOrWhiteSpace(State))
                    {
                        sql = "SELECT * FROM(SELECT row_number()over(order by Dealer ASC,Productid ASC) i,C_PRODUCTS.* FROM C_PRODUCTS " + query + " ORDER BY Dealer ASC, ProductId ASC) tmp WHERE i between " + (start + 1) + " AND " + (start + _size);
                        countSql = "SELECT COUNT(1) FROM C_PRODUCTS";
                    }
                    else
                    {
                        List<int> selfProductIds = new List<int>();
                        System.Data.DataTable pro = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT DISTINCT ProductID FROM C_PRODUCTS" + query);
                        foreach (DataRow row in pro.Rows)
                        {
                            selfProductIds.Add(int.Parse(ReadCell(row, "ProductID")));
                        }

                        string productIds = "";
                        foreach (int id in selfProductIds)
                        {
                            if (!baseProducts.ContainsKey(id))
                            {
                                productIds += ("'" + id + "',");
                            }
                        }
                        if (string.IsNullOrWhiteSpace(productIds))
                        {
                            productIds = "'-1'";
                        }
                        else
                        {
                            productIds = productIds.TrimEnd(',');
                        }
                        if ("1".Equals(State))
                        {
                            sql = "SELECT * FROM(SELECT row_number()over(order by Dealer ASC,Productid ASC) i,C_PRODUCTS.* FROM C_PRODUCTS WHERE ProductID IN(" + productIds + ")" + query.Replace("WHERE", "AND") + " ORDER BY Dealer ASC, ProductId ASC) tmp WHERE i between " + (start + 1) + " AND " + (start + _size);
                            countSql = "SELECT COUNT(1) FROM C_PRODUCTS WHERE ProductID IN(" + productIds + ")";
                        }
                        else
                        {
                            sql = "SELECT * FROM(SELECT row_number()over(order by Dealer ASC,Productid ASC) i,C_PRODUCTS.* FROM C_PRODUCTS WHERE ProductID NOT IN(" + productIds + ")" + query.Replace("WHERE", "AND") + " ORDER BY Dealer ASC, ProductId ASC) tmp WHERE i between " + (start + 1) + " AND " + (start + _size);
                            countSql = "SELECT COUNT(1) FROM C_PRODUCTS WHERE ProductID NOT IN(" + productIds + ")";
                        }

                    }

                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);

                    foreach (DataRow row in products.Rows)
                    {
                        if (string.IsNullOrWhiteSpace(State))
                        {
                            this.products.Add(new Product()
                            {
                                Dealer = ReadCell(row, "Dealer"),
                                ProductAlias = ReadCell(row, "ProductAlias"),
                                Description = ReadCell(row, "ProductDescription"),
                                ProductId = int.Parse(ReadCell(row, "ProductId")),
                                ProductName = ReadCell(row, "ProductName"),
                                State = baseProducts.Keys.Contains(int.Parse(ReadCell(row, "ProductId"))) ? "0" : "1"
                            });
                        }
                        else
                        {
                            this.products.Add(new Product()
                            {
                                Dealer = ReadCell(row, "Dealer"),
                                ProductAlias = ReadCell(row, "ProductAlias"),
                                Description = ReadCell(row, "ProductDescription"),
                                ProductId = int.Parse(ReadCell(row, "ProductId")),
                                ProductName = ReadCell(row, "ProductName"),
                                State = State
                            });
                        }
                    }

                    object count = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(countSql);
                    if (count == null || count == DBNull.Value || !int.TryParse(count.ToString(), out total))
                    {
                        total = 0;
                    }

                    foreach (Product p in this.products)
                    {
                        data.Add(p.GetJson(++start));
                    }
                }
                #endregion
                #region 查询单个供应商的产品，包括没有别名的
                else
                {
                    string DealerCode = "";

                    System.Data.DataTable Users = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT Code,APPELLATION FROM OT_USER WHERE APPELLATION='" + Dealer + "'");
                    foreach (DataRow row in Users.Rows)
                    {
                        DealerCode += "'" + ReadCell(row, "Code") + "',";
                    }
                    DealerCode = DealerCode.TrimEnd(',');
                    System.Data.DataTable allProducts = this.ExecuteDataTableSql("CAPDB", @"SELECT distinct " +
                        (!string.IsNullOrWhiteSpace(DealerCode) ? "WORKFLOW_LOGIN.BUSINESS_PARTNER_NME Dealer," : "'' Dealer,") + @"
FD.FINANCIAL_PRODUCT_ID ProductID,FP.FINANCIAL_PRODUCT_NME ProductName
FROM 
FP_DEALER@to_cms FD 
JOIN FINANCIAL_PRODUCT@to_cms FP ON FD.FINANCIAL_PRODUCT_ID=FP.FINANCIAL_PRODUCT_ID
JOIN 
(SELECT BSU.WORKFLOW_LOGIN_NME , BM2.BUSINESS_PARTNER_ID ,BM2.BUSINESS_PARTNER_NME 
FROM BP_SYS_USER@to_cms BSU 
JOIN BP_MAIN@to_cms BM ON BSU.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID 
JOIN BP_RELATIONSHIP@to_cms BRE ON BRE.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID AND BRE.RELATIONSHIP_CDE='00112'
JOIN BP_MAIN@to_cms BM2 ON BM2.BUSINESS_PARTNER_ID=BRE.BP_PRIMARY_ID
WHERE  BSU.WORKFLOWLOGIN_ACTIVE_IND='T' ) 
WORKFLOW_LOGIN ON 
WORKFLOW_LOGIN.BUSINESS_PARTNER_ID=FD.BUSINESS_PARTNER_ID
WHERE to_char(FP.VALID_FROM_DTE,'yyyymmdd')< to_char(sysdate,'yyyymmdd') and to_char(FP.VALID_TO_DTE,'yyyymmdd')>to_char(sysdate,'yyyymmdd')"
        + (string.IsNullOrWhiteSpace(DealerCode) ? "" : " and WORKFLOW_LOGIN.WORKFLOW_LOGIN_NME IN(" + DealerCode + ")")
        + (string.IsNullOrWhiteSpace(ProductName) ? "" : " AND FP.FINANCIAL_PRODUCT_NME LIKE '%" + ProductName + "%'")
        + " ORDER BY FD.FINANCIAL_PRODUCT_ID ASC");
                    Dictionary<int, string> Dealers = new Dictionary<int, string>();
                    if (allProducts != null)
                        foreach (DataRow row in allProducts.Rows)
                        {
                            baseProducts.Add(int.Parse(ReadCell(row, "ProductID")), ReadCell(row, "ProductName"));
                            Dealers.Add(int.Parse(ReadCell(row, "ProductID")), ReadCell(row, "Dealer"));
                        }
                    string query = "";
                    if (!string.IsNullOrWhiteSpace(ProductName))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ProductName LIKE '%" + ProductName + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(ProductAlias))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ProductAlias LIKE '%" + ProductAlias + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(Description))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " ProductDescription LIKE '%" + Description + "%'";
                    }
                    if (!string.IsNullOrWhiteSpace(Dealer))
                    {
                        query += (query == "" ? " WHERE" : " AND") + " Dealer='" + Dealer + "'";
                    }

                    List<int> selfProduct = new List<int>();

                    System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_PRODUCTS" + query + " ORDER BY ProductId ASC");

                    Dictionary<int, int> totalProducts = new Dictionary<int, int>();

                    for (int i = 0; i < products.Rows.Count; i++)
                    {
                        totalProducts.Add(int.Parse(ReadCell(products.Rows[i], "ProductId")), i);
                    }

                    if (string.IsNullOrWhiteSpace(ProductAlias) && string.IsNullOrWhiteSpace(Description) && !"1".Equals(State))
                    {
                        for (int i = 0; i < baseProducts.Count; i++)
                        {
                            if (!totalProducts.ContainsKey(baseProducts.ElementAt(i).Key))
                            {
                                totalProducts.Add(baseProducts.ElementAt(i).Key, -1);
                            }
                        }
                    }

                    int _size = int.Parse(size);
                    int start = int.Parse(page) * _size;
                    total = 0;

                    var dicSort = from objDic in totalProducts orderby objDic.Key ascending select objDic;
                    totalProducts = new Dictionary<int, int>();
                    switch (State)
                    {
                        case "1":
                            {
                                foreach (KeyValuePair<int, int> item in dicSort)
                                {
                                    if (!baseProducts.ContainsKey(item.Key))
                                    {
                                        totalProducts.Add(item.Key, item.Value);
                                    }
                                }
                            }
                            break;
                        case "0":
                            {
                                foreach (KeyValuePair<int, int> item in dicSort)
                                {
                                    if (baseProducts.ContainsKey(item.Key))
                                    {
                                        totalProducts.Add(item.Key, item.Value);
                                    }
                                }
                            }
                            break;
                        default:
                            {
                                foreach (KeyValuePair<int, int> item in dicSort)
                                {
                                    totalProducts.Add(item.Key, item.Value);
                                }
                            }
                            break;
                    }

                    for (int i = start; i < start + _size && i < totalProducts.Count; i++)
                    {
                        if (totalProducts.ElementAt(i).Value >= 0)
                        {
                            this.products.Add(new Product()
                            {
                                Dealer = ReadCell(products.Rows[totalProducts.ElementAt(i).Value], "Dealer"),
                                ProductAlias = ReadCell(products.Rows[totalProducts.ElementAt(i).Value], "ProductAlias"),
                                Description = ReadCell(products.Rows[totalProducts.ElementAt(i).Value], "ProductDescription"),
                                ProductId = int.Parse(ReadCell(products.Rows[totalProducts.ElementAt(i).Value], "ProductId")),
                                ProductName = ReadCell(products.Rows[totalProducts.ElementAt(i).Value], "ProductName"),
                                State = baseProducts.Keys.Contains(int.Parse(ReadCell(products.Rows[totalProducts.ElementAt(i).Value], "ProductId"))) ? "0" : "1"
                            });
                        }
                        else
                        {
                            this.products.Add(new Product()
                            {
                                Dealer = Dealer,
                                ProductAlias = "",
                                Description = "",
                                ProductId = totalProducts.ElementAt(i).Key,
                                ProductName = baseProducts[totalProducts.ElementAt(i).Key],
                                State = ""
                            });
                        }
                    }

                    total = totalProducts.Count;
                    foreach (Product p in this.products)
                    {
                        data.Add(p.GetJson(++start));
                    }
                }
                #endregion
            }
            #endregion
            #region 新增页面查询
            else
            {
                    string DealerCode = "";

                    System.Data.DataTable Users = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT Code,APPELLATION FROM OT_USER WHERE APPELLATION='" + Dealer + "'");
                    foreach (DataRow row in Users.Rows)
                    {
                        DealerCode += "'" + ReadCell(row, "Code") + "',";
                    }
                    DealerCode = DealerCode.TrimEnd(',');
                    System.Data.DataTable allProducts = this.ExecuteDataTableSql("CAPDB", @"SELECT distinct " +
                        (!string.IsNullOrWhiteSpace(DealerCode) ? "WORKFLOW_LOGIN.BUSINESS_PARTNER_NME Dealer," : "'' Dealer,") + @"
FD.FINANCIAL_PRODUCT_ID ProductID,FP.FINANCIAL_PRODUCT_NME ProductName
FROM 
FP_DEALER@to_cms FD 
JOIN FINANCIAL_PRODUCT@to_cms FP ON FD.FINANCIAL_PRODUCT_ID=FP.FINANCIAL_PRODUCT_ID
JOIN 
(SELECT BSU.WORKFLOW_LOGIN_NME , BM2.BUSINESS_PARTNER_ID ,BM2.BUSINESS_PARTNER_NME 
FROM BP_SYS_USER@to_cms BSU 
JOIN BP_MAIN@to_cms BM ON BSU.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID 
JOIN BP_RELATIONSHIP@to_cms BRE ON BRE.BP_SECONDARY_ID=BM.BUSINESS_PARTNER_ID AND BRE.RELATIONSHIP_CDE='00112'
JOIN BP_MAIN@to_cms BM2 ON BM2.BUSINESS_PARTNER_ID=BRE.BP_PRIMARY_ID
WHERE  BSU.WORKFLOWLOGIN_ACTIVE_IND='T' ) 
WORKFLOW_LOGIN ON 
WORKFLOW_LOGIN.BUSINESS_PARTNER_ID=FD.BUSINESS_PARTNER_ID
WHERE to_char(FP.VALID_FROM_DTE,'yyyymmdd')< to_char(sysdate,'yyyymmdd') and to_char(FP.VALID_TO_DTE,'yyyymmdd')>to_char(sysdate,'yyyymmdd')"
        + (string.IsNullOrWhiteSpace(DealerCode) ? "" : " and WORKFLOW_LOGIN.WORKFLOW_LOGIN_NME IN(" + DealerCode + ")")
        + (string.IsNullOrWhiteSpace(ProductName) ? "" : " AND FP.FINANCIAL_PRODUCT_NME LIKE '%" + ProductName + "%'")
        + " ORDER BY FD.FINANCIAL_PRODUCT_ID ASC");
                Dictionary<int, string> Dealers = new Dictionary<int, string>();
                if (allProducts != null)
                    foreach (DataRow row in allProducts.Rows)
                    {
                        baseProducts.Add(int.Parse(ReadCell(row, "ProductID")), ReadCell(row, "ProductName"));
                        Dealers.Add(int.Parse(ReadCell(row, "ProductID")), ReadCell(row, "Dealer"));
                    }

                System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT * FROM C_PRODUCTS WHERE Dealer='" + Dealer + "'");
                total = 0;
                foreach (DataRow row in products.Rows)
                {
                    if (baseProducts.Keys.Contains(int.Parse(ReadCell(row, "ProductId"))))
                    {
                        this.products.Add(new Product()
                        {
                            Description = ReadCell(row, "ProductDescription"),
                            ProductId = int.Parse(ReadCell(row, "ProductId")),
                            ProductName = ReadCell(row, "ProductName"),
                            ProductAlias = ReadCell(row, "ProductAlias")
                        });
                    }
                }
                foreach (KeyValuePair<int, string> tp in baseProducts)
                {
                    bool has = false;
                    foreach (Product p in this.products)
                    {
                        if (tp.Key == p.ProductId)
                        {
                            has = true;
                            break;
                        }
                    }
                    if (!has)
                    {
                        this.products.Add(new Product()
                        {
                            ProductAlias = "",
                            Description = "",
                            ProductId = tp.Key,
                            ProductName = tp.Value,
                        });
                    }
                }
                this.products.Sort();
                int index = 0;
                foreach (Product p in this.products)
                {
                    data.Add(p.GetJson(++index));
                }
            }
            #endregion
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