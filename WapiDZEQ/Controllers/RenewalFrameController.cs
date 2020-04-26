using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Data;
using System.IO;
namespace OThinker.H3.Controllers
{
    public class RenewalFrameController : Controller
    {
        public string Index()
        {
            return "UploadControl";
        }
        public const string dataCode = "Wholesale";
        public JsonResult ImportBizObjectSchemaProperty(string schemaCode)
        {
            var files = Request.Files;
            var length = files.Count;
            if (length == 0)
                return Json(new { Success = false, Data = "", Msg = "文件为空" });
            string fileName = Request.Files[0].FileName;
            string fileType = Path.GetExtension(OThinker.H3.Controllers.Controllers.Common.TrimHtml(Path.GetFileName(fileName))).ToLowerInvariant();
            //文件格式错误

            string d;
            try
            {
                Stream s = Request.Files[0].InputStream;
                d = SaveUplodStream(s);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Data = "文件格式错误;", Msg = "Exception-->" + e.Message }, "text/html");
            }
            return Json(new { Success = true, Msg = "", Data = d }, "text/html");
        }

        private string SaveUplodStream(Stream inputStream)
        {
            RepaymentController rc = new RepaymentController();
            JsonResult jr = new JsonResult();

            OThinker.H3.Controllers.Controllers.ProcessModel.BizObjectSchemaPropertyController property = new OThinker.H3.Controllers.Controllers.ProcessModel.BizObjectSchemaPropertyController();
            DataTable dt = OThinker.H3.Controllers.Controllers.Common.ExcelToDataTable(inputStream);

            List<string> ret = new List<string>();
            List<string> cfcjh = new List<string>();

          
            string cf = string.Empty;
            for (int i = dt.Rows.Count - 1; i >= 0; i--)
            {
                string lscjh = dt.Rows[i]["临时车架号"].ToString().Trim();
                string yjcjh = dt.Rows[i]["永久车架号"].ToString().Trim();

                int k = 0;
                // 文档本身车架号是否重复
                for (int j = dt.Rows.Count - 1; j >= 0; j--)
                {
                   
                    if (yjcjh == dt.Rows[j]["永久车架号"].ToString().Trim())
                    {
                        k = k + 1;
                    }
                        
                }
                if (k == 1)

                    ret.Add(lscjh + ";" + yjcjh);
                else
                    cf = cf + yjcjh + ";"; //如果for循环存在重复，就把新旧车架号添加起来
                   
              
            }
            List<string> cjh = new List<string>();
            foreach (string item in ret)
            {
                
                string yj = item.Split(';')[1];
                
                // 验证车架号是否重复 通过数据库
                //rc.CfuCJH();
                string sql = string.Format(@"select count(1) count from IN_WFS.V_LOAN_STOCK_LIST where VIN_NO = '{0}' ", yj);
                int count = ExecuteCountSql(dataCode, sql);
                if (count > 0 || string.IsNullOrEmpty(yj) || yj.Length!=17)
                {
                    cf = cf + yj + ";";
                }
                else
                {
                    cjh.Add(item);
                }

            }
            rc.UpdateCJH(cjh.ToArray());//修改车架号成功，车架号添加到新的数组

            return cf;

        }

         public int ExecuteCountSql(string connectionCode, string sql)
    {
        var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
        if (dbObject != null)
        {
            OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
            var command = factory.CreateCommand();
            var count = command.ExecuteScalar(sql); 
            return Convert.ToInt32(count);
        }
        return 0;

        
    }


    }


    public class ImportError
    {
        /// <summary>
        /// 获取或设置行数
        /// </summary>
        public int Seq { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; set; }
    }
}