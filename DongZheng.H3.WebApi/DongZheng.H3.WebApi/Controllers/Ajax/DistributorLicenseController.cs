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
    public class DistributorLicenseController : CustomController
    {
        public override string FunctionCode => "";

        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string action = context.Request["Action"] ?? "";
            string search = context.Request["search"] ?? "";
            string express = context.Request["License"] ?? "";
            string objectid = context.Request["Objectid"] ?? "";
            string distributor = context.Request["Distributor"] ?? "";
            string crmDealedId = context.Request["crmDealerID"] ?? "";
            string model = "<li onclick=\"var $t = $(this); var $p = $t.parent().parent(); $p.prev().val($t.text()); $(\'[data-yyzz]\').val($t.attr(\'data-license\')).attr(\'data-objectid\', $t.attr(\'data-objectid\')).attr(\'data-crmdealerid\', $t.attr(\'data-crmdealerid\')); $p.hide();\" data-crmdealerid=\"[CrmId]\" data-objectid=\"[ObjectId]\" data-license=\"[License]\">[Name]</li>";
            string result = "";
            if (action == "Load" && !string.IsNullOrEmpty(search))
            {
                string sql = "SELECT OBJECTID,CRMDEALERID,DISTRIBUTOR,LICENSE FROM DISTRIBUTORLICENSE " + (string.IsNullOrEmpty(search) ? "" : "WHERE DISTRIBUTOR LIKE '%" + search + "%'") +
                    " UNION ALL SELECT CAST('' AS NVARCHAR2(200)),CRMDEALERID,DISTRIBUTOR,CAST('' AS NVARCHAR2(200)) FROM I_ALLOWIN WHERE DISTRIBUTOR NOT IN (SELECT DISTRIBUTOR FROM DISTRIBUTORLICENSE) " +
                    (string.IsNullOrEmpty(search) ? "" : "AND DISTRIBUTOR LIKE '%" + search + "%'");
                DataTable dt = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        result += model
                            .Replace("[License]", dr["LICENSE"] == null || dr["LICENSE"] == DBNull.Value ? "" : dr["LICENSE"].ToString())
                            .Replace("[ObjectId]", dr["OBJECTID"] == null || dr["OBJECTID"] == DBNull.Value ? "" : dr["OBJECTID"].ToString())
                            .Replace("[Name]", dr["DISTRIBUTOR"] == null || dr["DISTRIBUTOR"] == DBNull.Value ? "" : dr["DISTRIBUTOR"].ToString())
                            .Replace("[CrmId]", dr["CRMDEALERID"] == null || dr["CRMDEALERID"] == DBNull.Value ? "" : dr["CRMDEALERID"].ToString());
                    }
                }
            }
            if (action == "Save")
            {
                string sql = string.IsNullOrEmpty(objectid) ? "INSERT INTO DISTRIBUTORLICENSE(OBJECTID,CRMDEALERID,DISTRIBUTOR,LICENSE)VALUES('" + Guid.NewGuid() + "','" + crmDealedId + "','" + distributor + "','" + express + "')" : "UPDATE DISTRIBUTORLICENSE SET LICENSE = '" + express + "' WHERE OBJECTID = '" + objectid + "'";
                int i = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                result = "保存成功";
            }
            if (action == "Get")
            {
                UserValidator userInSession = context.Session[Sessions.GetUserValidator()] as UserValidator;
                string sql = "SELECT LICENSE FROM DISTRIBUTORLICENSE WHERE DISTRIBUTOR LIKE '" + userInSession.UserName + "'";//山东旭东汽车销售有限公司    02'
                string license = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
                result = license;
            }
            context.Response.Write(result);
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