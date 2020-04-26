<%@ WebHandler Language="C#" Class="GetProducts_Select" %>

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

public class GetProducts_Select : MvcPage, IHttpHandler, System.Web.SessionState.IRequiresSessionState {

    public void ProcessRequest (HttpContext context) {
        Dictionary<int, int> baseProducts = new Dictionary<int, int>();
        string Dealer = UserValidatorFactory.CurrentUser.UserCode;
        System.Data.DataTable allProducts = this.ExecuteDataTableSql("CAPDB", @"SELECT distinct FD.FINANCIAL_PRODUCT_ID ProductID,FP.FINANCIAL_PRODUCT_NME ProductName
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
+ " and WORKFLOW_LOGIN.WORKFLOW_LOGIN_NME='" + Dealer + "'"
+ " ORDER BY FD.FINANCIAL_PRODUCT_ID ASC");
        Newtonsoft.Json.Linq.JArray ret = new Newtonsoft.Json.Linq.JArray();
        if(allProducts != null)
            for (int i = 0; i < allProducts.Rows.Count; i++)
            {
                Newtonsoft.Json.Linq.JObject token = new Newtonsoft.Json.Linq.JObject();
                token.Add("ProductID", int.Parse(ReadCell(allProducts.Rows[i], "ProductID")));
                token.Add("ProductName", ReadCell(allProducts.Rows[i], "ProductName"));
                token.Add("ProductDescription", "暂无描述");
                ret.Add(token);
                baseProducts.Add(int.Parse(ReadCell(allProducts.Rows[i], "ProductID")), i);
            }

        System.Data.DataTable products = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable("SELECT p.* FROM C_PRODUCTS p LEFT JOIN OT_USER u ON p.dealer=u.appellation WHERE u.Code='" + UserValidatorFactory.CurrentUser.UserCode + "' ORDER BY ProductId ASC");
        foreach (DataRow row in products.Rows)
        {
            if(baseProducts.ContainsKey(int.Parse(ReadCell(row, "ProductID"))))
            {
                ret[baseProducts[int.Parse(ReadCell(row, "ProductID"))]]["ProductName"] = ReadCell(row, "ProductAlias");
                ret[baseProducts[int.Parse(ReadCell(row, "ProductID"))]]["ProductDescription"] = ReadCell(row, "ProductDescription");
            }
        }
        context.Response.Write(ret);
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

    public bool IsReusable {
        get {
            return false;
        }
    }

}