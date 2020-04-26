using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Controllers.Controllers;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Data;
using OThinker.H3.DataModel;

namespace DongZheng.H3.WebApi.Controllers
{
    [ValidateInput(false)]
    [Xss]
    public class UploadController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";

        // GET: Upload
        public string Index()
        {
            return "UploadControl";
        }
        
        [HttpPost]
        public JsonResult ImportBizObjectSchemaProperty(string schemaCode)
        {
            var files = Request.Files;
            var length = files.Count;
            if (length == 0)
                return Json(new { Success = false, Data = "", Msg = "文件为空" });
            string fileName = Request.Files[0].FileName;
            string fileType = Path.GetExtension(OThinker.H3.Controllers.Controllers.Common.TrimHtml(Path.GetFileName(fileName))).ToLowerInvariant();
            //文件格式错误
            if (!fileType.Equals(".xls"))
            {
                return Json(new { Success = false, Data = "", Msg = "文件格式错误，请根据模板进行编辑" });
            }
            List<object> d;
            try
            {
                Stream s = Request.Files[0].InputStream;
                d = SaveUplodStream(s, schemaCode);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Data = "", Msg = "Exception-->" + e.Message });
            }
            return Json(new { Success = true, Msg = "", Data = d }, "text/html");
        }

        [HttpPost]
        public JsonResult ImportFinancialProductCorrelation()
        {
            var files = Request.Files;
            var length = files.Count;
            if (length == 0 || length == 1)
                return Json(new { Success = false, Data = "", Msg = "文件缺少，请上传二个文件" });
            string fileName1 = Request.Files[0].FileName;
            string fileType1 = Path.GetExtension(OThinker.H3.Controllers.Controllers.Common.TrimHtml(Path.GetFileName(fileName1))).ToLowerInvariant();
            string fileName2 = Request.Files[1].FileName;
            string fileType2 = Path.GetExtension(OThinker.H3.Controllers.Controllers.Common.TrimHtml(Path.GetFileName(fileName2))).ToLowerInvariant();
            //文件格式错误
            if (!fileType1.Equals(".xls") || !fileType2.Equals(".xls"))
            {
                return Json(new { Success = false, Data = "", Msg = "文件格式错误，请上传xls格式文件" });
            }
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            try
            {
                //经销商
                DataTable dt_FI = OThinker.H3.Controllers.Controllers.Common.ExcelToDataTable(Request.Files[0].InputStream);
                //产品
                DataTable dt_Product = OThinker.H3.Controllers.Controllers.Common.ExcelToDataTable(Request.Files[1].InputStream);
                string condition_Product = "";
                foreach (DataRow rowProduct in dt_Product.Rows)
                {
                    condition_Product += "'" + rowProduct[0] + "',";
                }
                if (condition_Product != "")
                    condition_Product = condition_Product.Substring(0, condition_Product.Length - 1);
                //生产SQL
                foreach (DataRow row_FI in dt_FI.Rows)
                {
                    string sql = @"
DECLARE
BEGIN
FOR DEALERCOMMISION IN (SELECT FINANCIAL_PRODUCT_ID FROM FINANCIAL_PRODUCT WHERE FINANCIAL_PRODUCT_NME IN (
{0}
) ) LOOP

---填写金融产品名称 
    INSERT INTO FP_DEALER
      (FINANCIAL_PRODUCT_ID,
       BUSINESS_PARTNER_ID,
       ROLE_CDE,
       EXECUTION_DTE,
       INSERTED_BY)
      SELECT DEALERCOMMISION.FINANCIAL_PRODUCT_ID, -- FP ID
             BP_MAIN.BUSINESS_PARTNER_ID,
             '00001',
             SYSDATE,
             'BACKEND'
        FROM BP_MAIN, BP_ROLE
       WHERE BP_ROLE.BUSINESS_PARTNER_ID = BP_MAIN.BUSINESS_PARTNER_ID
         AND BP_ROLE.ROLE_CDE = '00001'
         AND BP_MAIN.BUSINESS_PARTNER_NME IN ({1})  

---填写经销商名称
         AND NOT EXISTS
       (SELECT 1 FROM FP_DEALER FD
               WHERE FD.FINANCIAL_PRODUCT_ID =
                     DEALERCOMMISION.FINANCIAL_PRODUCT_ID
                 AND FD.BUSINESS_PARTNER_ID = BP_MAIN.BUSINESS_PARTNER_ID
                 AND FD.ROLE_CDE = '00001');
    INSERT INTO FP_DEALER_COMMISSION
      (FINANCIAL_PRODUCT_ID, BUSINESS_PARTNER_ID, ROLE_CDE, CHART_ID)
      SELECT DEALERCOMMISION.FINANCIAL_PRODUCT_ID,
             BP_MAIN.BUSINESS_PARTNER_ID,
             '00001',
             '1'
        FROM BP_MAIN, BP_ROLE
       WHERE BP_ROLE.BUSINESS_PARTNER_ID = BP_MAIN.BUSINESS_PARTNER_ID
         AND BP_ROLE.ROLE_CDE = '00001'
          AND BP_MAIN.BUSINESS_PARTNER_NME IN ({1}) 

-------填写经销商名称
         AND NOT EXISTS
       (SELECT 1 FROM FP_DEALER_COMMISSION FD
               WHERE FD.FINANCIAL_PRODUCT_ID =
                     DEALERCOMMISION.FINANCIAL_PRODUCT_ID
                 AND FD.BUSINESS_PARTNER_ID = BP_MAIN.BUSINESS_PARTNER_ID
                 AND FD.ROLE_CDE = '00001'
                 AND FD.CHART_ID = '1');
  END LOOP;
END;
";

                    sql = string.Format(sql, condition_Product, row_FI[0]);
                    var v = new Dictionary<string, string>();
                    v.Add(row_FI[0] + string.Empty, sql);
                    result.Add(v);
                }
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Data = "", Msg = "Exception-->" + e.Message });
            }
            return Json(new { Success = true, Data = result, Msg = "" }, "text/html");
        }

        private List<object> SaveUplodStream(Stream inputStream, string schemaCode)
        {
            OThinker.H3.Controllers.Controllers.ProcessModel.BizObjectSchemaPropertyController property = new OThinker.H3.Controllers.Controllers.ProcessModel.BizObjectSchemaPropertyController();
            HttpHelper http = new HttpHelper();

            DataTable dt = OThinker.H3.Controllers.Controllers.Common.ExcelToDataTable(inputStream);
            List<object> ret = new List<object>();
            foreach (DataRow row in dt.Rows)
            {
                var paraData = new
                {
                    SchemaCode = schemaCode,
                    PropertyName = GetFieldCode(row[0] + string.Empty, row[1] + string.Empty),
                    DisplayName = row[2] + string.Empty,
                    LogicType = GetFieldType(row[3] + string.Empty),
                    VirtualField = (row[5] + string.Empty) == "true" ? true : false,
                    RecordTrail = (row[6] + string.Empty) == "true" ? true : false,
                    Indexed = (row[7] + string.Empty) == "true" ? true : false,
                    Searchable = (row[8] + string.Empty) == "true" ? true : false,
                    DefaultValue = row[4] + string.Empty,
                    Formula = "",
                    Global = "",
                    IsPublished = false,//发布了的字段这里是true
                    Property = "",//发布了的字段这里对应自身字段编码
                    ParentProperty = GetParentFieldCode(row[0] + string.Empty, row[1] + string.Empty),//主表：创建为空，修改为\自已的编辑；子表中的字段：为子表的编码
                    ObjectID = ""
                };
                ret.Add(paraData);
                //var result = property.SaveBizObjectSchemaProperty(paraData);
                //var result = http.PostWebRequest("http://" + Request.Url.Authority + "/Portal/BizObjectSchemaProperty/SaveBizObjectSchemaProperty", Newtonsoft.Json.JsonConvert.SerializeObject(paraData));
            }
            //msg.Add(new ImportError { Seq = 1, Error = dt.Rows.Count.ToString() });
            return ret;
        }

        public string GetFieldCode(string code1, string code2)
        {
            if (string.IsNullOrEmpty(code1) && string.IsNullOrEmpty(code2))
            {
                return "";
            }
            if (!string.IsNullOrEmpty(code2)&&!string.IsNullOrEmpty(code1))
                return code2.ToUpper();
            return code1.ToUpper();
        }

        public string GetParentFieldCode(string code1, string code2)
        {
            if (string.IsNullOrEmpty(code1) && string.IsNullOrEmpty(code2))
            {
                return "";
            }
            if (!string.IsNullOrEmpty(code2) && !string.IsNullOrEmpty(code1))
                return @"\" + code1;
            return "";
        }

        public string GetFieldType(string TypeName)
        {
            switch (TypeName)
            {
                case "短文本": return "ShortString";
                case "长文本": return "String";
                case "逻辑型": return "Bool";
                case "整数": return "Int";
                case "长整数": return "Long";
                case "数值": return "Double";
                case "日期": return "DateTime";
                case "附件": return "Attachment";
                case "审批意见": return "Comment";
                case "子表": return "BizObjectArray";
                case "参与者（单人）": return "SingleParticipant";
                case "参与者（多人）": return "MultiParticipant";
                case "时间段": return "TimeSpan";
                case "HTML": return "Html";
                case "链接": return "HyperLink";

                case "全局变量": return "GlobalData";
                case "关联表单": return "Association";
                default:return "";
            }
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