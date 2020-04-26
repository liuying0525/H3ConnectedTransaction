<%@ WebHandler Language="C#" Class="ProductInit" %>

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

public class ProductInit : MvcPage, IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    Dictionary<string, string> role = new Dictionary<string, string>() {
        { "结构性-常规贷", "又名奖金贷，结构贷，年供产品。首付x%起，客户利率z%，贷款年限1-n年，每个贷款年度的第12个月偿还20%的本金，其他月份偿还本金及利息，且月供相等。车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "百禄贷-常规贷", "又名气球贷、百禄贷、尾款贷、弹性尾款贷。首付x%起，尾款随贷款年限不同而不同，最高y%，客户利率z%，贷款年限1-n年，每月月供相等（最后一期除外）而最后一期偿还金额为尾款部分；车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "等额本息-车秒贷", "首付x%起，无尾款，客户利率y%，贷款年限1-Z年，每月月供相等，月供为贷款金额本息和除以贷款期限；车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "等额本息-常规贷", "首付x%起，无尾款，客户利率y%，贷款年限1-Z年，每月月供相等，月供为贷款金额本息和除以贷款期限；车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "百禄贷-车秒贷", "又名气球贷、百禄贷、尾款贷、弹性尾款贷。首付x%起，尾款随贷款年限不同而不同，最高y%，客户利率z%，贷款年限1-n年，每月月供相等（最后一期除外）而最后一期偿还金额为尾款部分；车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "机构贷款", "专门为公司、组织等法人单位设计满足其金融需求，首付x%起，尾款a%，贷款利率y%，贷款年限1-Z年，还款方式一般有等额本息、百禄和结构性贷款，车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "等额本金-常规贷", "首付x%起，无尾款，客户利率y%，贷款年限1-Z年，每月偿还本金相等，利息随未偿还本金逐月递减，月供随未偿还本金逐月递减，车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "结构性-车秒贷", "又名奖金贷，结构贷，年供产品。首付x%起，客户利率z%，贷款年限1-n年，每个贷款年度的第12个月偿还20%的本金，其他月份偿还本金及利息，且月供相等。车秒贷限额3万-70万，常规贷款限额3万-500万" },
        { "等额本金-车秒贷", "首付x%起，无尾款，客户利率y%，贷款年限1-Z年，每月偿还本金相等，利息随未偿还本金逐月递减，月供随未偿还本金逐月递减，车秒贷限额3万-70万，常规贷款限额3万-500万" }
    };
    public void ProcessRequest(HttpContext context)
    {
        object tableCount = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT COUNT(1) FROM user_tables t where t.table_name = upper('C_Products')");
        int t = 0;
        if(tableCount == null || tableCount == DBNull.Value || string.IsNullOrWhiteSpace(tableCount.ToString()) || !int.TryParse(tableCount.ToString(), out t) || t <= 0)
        {
            AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(@"
                create table C_PRODUCTS
                (
                    productname        NVARCHAR2(2000),
                    productalias       NVARCHAR2(2000),
                    dealer             NVARCHAR2(2000),
                    productdescription NVARCHAR2(2000),
                    objectid           NVARCHAR2(2000),
                    productid          INTEGER
                )
                tablespace TBS_H3_DATA
                    pctfree 10
                    pctused 40
                    initrans 1
                    maxtrans 255
                    storage
                    (
                    initial 64K
                    next 1M
                    minextents 1
                    maxextents unlimited
                )");
        }
        else
        {
            AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery("DELETE FROM C_PRODUCTS");
        }

        System.Data.DataTable allProducts = this.ExecuteDataTableSql("CAPDB", @"
SELECT DISTINCT FD.FINANCIAL_PRODUCT_ID ProductID,
       FP.FINANCIAL_PRODUCT_NME ProductName,
			 FPG.FP_GROUP_NME ProductGroup,
       WORKFLOW_LOGIN.BUSINESS_PARTNER_NME Dealer
  FROM FP_DEALER@to_cms FD
       JOIN FINANCIAL_PRODUCT@to_cms FP
          ON FD.FINANCIAL_PRODUCT_ID = FP.FINANCIAL_PRODUCT_ID
       JOIN FINANCIAL_PRODUCT_GROUP@to_cms FPG ON FPG.FP_GROUP_ID = FP.FP_GROUP_ID
       JOIN
       (SELECT BSU.WORKFLOW_LOGIN_NME,
               BM2.BUSINESS_PARTNER_ID,
               BM2.BUSINESS_PARTNER_NME
          FROM BP_SYS_USER@to_cms BSU
               JOIN BP_MAIN@to_cms BM
                  ON BSU.BP_SECONDARY_ID = BM.BUSINESS_PARTNER_ID
               JOIN BP_RELATIONSHIP@to_cms BRE
                  ON     BRE.BP_SECONDARY_ID = BM.BUSINESS_PARTNER_ID
                     AND BRE.RELATIONSHIP_CDE = '00112'
               JOIN BP_MAIN@to_cms BM2
                  ON BM2.BUSINESS_PARTNER_ID = BRE.BP_PRIMARY_ID
         WHERE BSU.WORKFLOWLOGIN_ACTIVE_IND = 'T') WORKFLOW_LOGIN
          ON WORKFLOW_LOGIN.BUSINESS_PARTNER_ID = FD.BUSINESS_PARTNER_ID
 WHERE     TO_CHAR (FP.VALID_FROM_DTE, 'yyyymmdd') <=
              TO_CHAR (SYSDATE, 'yyyymmdd')
       AND TO_CHAR (FP.VALID_TO_DTE, 'yyyymmdd') >=
              TO_CHAR (SYSDATE, 'yyyymmdd')");
        string sql = "";
        foreach (DataRow row in allProducts.Rows)
        {
            if (((string)row["ProductGroup"]).IndexOf("作废") >= 0) continue;
            foreach (string key in role.Keys)
            {
                if (((string)row["ProductGroup"]).IndexOf(key) >= 0)
                {
                    sql += @"
INSERT INTO C_PRODUCTS (ObjectID,Dealer,ProductId,ProductName,ProductAlias,ProductDescription) VALUES('" + Guid.NewGuid().ToString() + "','" + row["Dealer"] + "', '" + row["ProductID"] + "','" + row["ProductName"] + "','" + row["ProductName"] + "','" + role[key] + "');";
                    break;
                }
            }
            if (sql.Length >= 1)
            {
                AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql.TrimEnd(';'));
                sql = "";
            }
        }
        if (!string.IsNullOrWhiteSpace(sql))
            AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql.TrimEnd(';'));
        context.Response.Write("OK");
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
            return new DataTable();
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