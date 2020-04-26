using System;
using System.Collections.Generic;
using System.Linq;   
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Threading;
using System.Globalization;
using System.Web.Script.Serialization;
using OThinker.H3.Sheet;
using OThinker.H3.Instance.Keywords;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.IO;
using System.Text;
using System.Configuration;
using System.Text.RegularExpressions;
using System.Data;
using OThinker.Organization;
using NPOI.HSSF.UserModel;
using System.Data.OracleClient;

namespace WapiDZEQ
{
    public class CurrencyClass : OThinker.H3.Controllers.MvcPage
    {
        public CurrencyClass()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }


        public void ExportReportCurrency(DataTable dt, string SheetName)
        {
            var ms = new MemoryStream();
            IWorkbook wb = new XSSFWorkbook();
            var sheet = wb.CreateSheet(SheetName);
            var rowHeader = sheet.CreateRow(0);

            for (var j = 0; j < dt.Columns.Count; j++)
            {
                rowHeader.CreateCell(j).SetCellValue(dt.Columns[j].ColumnName);
            }
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            //wb.Write(ms);
            //HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename="
            //    + HttpUtility.UrlEncode(SheetName, Encoding.UTF8) + ".xlsx");
            //HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
            //ms.Close();
            //ms.Dispose();

            wb.Write(ms);

            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Charset = "UTF-8";
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel;charset=UFT-8";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", HttpUtility.UrlEncode(SheetName, Encoding.UTF8)));
            System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            // Response.OutputStream.ToString();
            System.Web.HttpContext.Current.Response.Flush();
            // Response.Close();
            System.Web.HttpContext.Current.Response.End();
            wb = null;
            ms.Close();
            ms.Dispose();

        }


        public void ExportExcel(DataTable dt,string SheetName )
        {
            var ms = new MemoryStream();
            IWorkbook wb = new HSSFWorkbook();
            var sheet = wb.CreateSheet(SheetName);
            var rowHeader = sheet.CreateRow(0);

            for (var j = 0; j < dt.Columns.Count; j++)
            {
                rowHeader.CreateCell(j).SetCellValue(dt.Columns[j].ColumnName);
            }
            for (var i = 0; i < dt.Rows.Count; i++)
            {
                var row = sheet.CreateRow(i + 1);
                for (var j = 0; j < dt.Columns.Count; j++)
                {
                    row.CreateCell(j).SetCellValue(dt.Rows[i][j].ToString());
                }
            }
            //wb.Write(ms);
            //HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("utf-8");
            //HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename="
            //    + HttpUtility.UrlEncode(SheetName, Encoding.UTF8) + ".xlsx");
            //HttpContext.Current.Response.ContentType = "application/ms-excel";
            //HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            //HttpContext.Current.Response.Flush();
            //HttpContext.Current.Response.End();
            //HttpContext.Current.ApplicationInstance.CompleteRequest();
            //ms.Close();
            //ms.Dispose();

            wb.Write(ms);

            System.Web.HttpContext.Current.Response.Clear();
            System.Web.HttpContext.Current.Response.Buffer = true;
            System.Web.HttpContext.Current.Response.Charset = "UTF-8";
            System.Web.HttpContext.Current.Response.ContentType = "application/ms-excel;charset=UFT-8";
            System.Web.HttpContext.Current.Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xls", HttpUtility.UrlEncode(SheetName, Encoding.UTF8)));
            System.Web.HttpContext.Current.Response.BinaryWrite(ms.ToArray());
            // Response.OutputStream.ToString();
            System.Web.HttpContext.Current.Response.Flush();
            // Response.Close();
            System.Web.HttpContext.Current.Response.End();
            wb = null;
            ms.Close();
            ms.Dispose();
        }


        /// <summary> 
        /// DataTable转为json 
        /// </summary> 
        /// <param name="dt">DataTable</param> 
        /// <returns>json数据</returns> 
        public static string ToJson(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc]);
                }
                list.Add(result);
            }

            return SerializeToJson(list);
        }

        /// <summary>
        /// 序列化对象为Json字符串
        /// </summary>
        /// <param name="obj">要序列化的对象</param>
        /// <param name="recursionLimit">序列化对象的深度，默认为100</param>
        /// <returns>Json字符串</returns>
        public static string SerializeToJson(object obj)
        {
            JavaScriptSerializer serialize = new JavaScriptSerializer();

            return serialize.Serialize(obj);
        }
        /// <summary>
        /// 汇总数据接口 
        /// </summary>
        public void SUMMARY(string UserCode)
        {

            //此连接在web.config里
            string connectionString = System.Configuration.ConfigurationManager.AppSettings["jxsDZPORT"];
            //执行存储过程

            OracleConnection conn = new OracleConnection(connectionString);
            string storedProcedureName = "IN_WFS.ZSUMMARY_DLR";
            OracleParameter[] parameters = { new OracleParameter("p_company_code", OracleType.VarChar, 80),
                new OracleParameter("p_dealer_code", OracleType.VarChar, 80)};
            parameters[0].Value = "08615";
            parameters[1].Value = UserCode;
            try
            {
                conn.Open();//打开连接
                OracleCommand cmd = conn.CreateCommand();
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = storedProcedureName;
                cmd.Parameters.Add(parameters[0]);
                cmd.Parameters.Add(parameters[1]);
                cmd.ExecuteNonQuery();
                //string ret = cmd.Parameters["o_trans_no"].Value + string.Empty;
                //result = cmd.Parameters["o_result"].Value + string.Empty;
                //err_code = cmd.Parameters["o_err_code"].Value + string.Empty;

                conn.Close();
                //return ret;

            }
            catch (Exception ex)
            {
                //result = string.Empty;
                //err_code = string.Empty;
                conn.Close();
                //return "";

            }

        }

        /// <summary>
        /// 阻止频繁的调用接口
        /// </summary>
        /// <param name="UserCode"></param>
        /// <returns></returns>
        public string CheckTime(string UserCode)
        {
            string sql = "select  case when UPD_TIME+ 2/(24*60)<=sysdate then '1' else '0' end flag   from IN_WFS.V_SUMMARY_UPD_TIME v where v.dealer_code=" + UserCode;

            DataTable dt = ExecuteDataTableSql("Wholesale", sql);

            if (dt != null && dt.Rows != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0][0].ToString();
            }

            return "1";
        }
        /// <summary>
        /// dataTable
        /// </summary>
        /// <param name="connectionCode"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }

    }
}