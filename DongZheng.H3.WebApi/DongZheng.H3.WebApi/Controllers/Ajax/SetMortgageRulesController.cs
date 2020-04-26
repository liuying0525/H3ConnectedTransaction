using DongZheng.H3.WebApi.Common.Portal;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class SetMortgageRulesController : CustomController
    {
        public override string FunctionCode => "";


        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            formDataUpload(context);
        }

        public static string InsertMortgageRule = "INSERT INTO C_MORTGAGERULES(\"OBJECTID\",\"SHOP\",\"PROVINCE\",\"CITY\",\"SPNAME\",\"DYNAME\") VALUES([VALUES])";
        private static void formDataUpload(HttpContextBase context)
        {
            HttpFileCollectionBase files = context.Request.Files;
            JavaScriptSerializer json = new JavaScriptSerializer();
            WorkFlowMethod.Return result = new WorkFlowMethod.Return();
            string msg = string.Empty;
            string error = string.Empty;
            if (files.Count > 0)
            {
                string filepath = @"D:\\H3 BPM\\Portal\\TemplateFile\\MortgageRules";
                try
                {
                    string paths = filepath + "\\" + System.IO.Path.GetFileName(files[0].FileName);
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    files[0].SaveAs(filepath + "\\" + System.IO.Path.GetFileName(files[0].FileName));
                    DataTable dt = ExcelToDataTable(paths);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        List<string> sqlList = new List<string>();
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string provinceCode = "", CityCode = "", SPCode = "", DYCode = "";
                            string shop = dt.Rows[i]["办理店"].ToString();
                            //if (!string.IsNullOrEmpty(dt.Rows[i]["省份"].ToString()))
                            provinceCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT CODEID FROM AREA WHERE CODENAME LIKE '%" + dt.Rows[i]["省份"] + "%'") + string.Empty;
                            //if (!string.IsNullOrEmpty(dt.Rows[i]["城市"].ToString()))
                            CityCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT CODEID FROM AREA WHERE CODENAME LIKE '%" + dt.Rows[i]["城市"] + "%'") + string.Empty;
                            //if (!string.IsNullOrEmpty(dt.Rows[i]["上牌员"].ToString()))
                            SPCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_USER WHERE NAME LIKE '%" + dt.Rows[i]["上牌员"] + "%'") + string.Empty;
                            //if (!string.IsNullOrEmpty(dt.Rows[i]["抵押员"].ToString()))
                            DYCode = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar("SELECT OBJECTID FROM OT_USER WHERE NAME LIKE '%" + dt.Rows[i]["抵押员"] + "%'") + string.Empty;

                            //if (!string.IsNullOrEmpty(provinceCode) && !string.IsNullOrEmpty(CityCode) && !string.IsNullOrEmpty(SPCode) && !string.IsNullOrEmpty(DYCode))
                            //{
                            string sql = InsertMortgageRule.Replace("[VALUES]", "'" + Guid.NewGuid() + "','" + shop + "','" + provinceCode + "','" + CityCode + "','" + SPCode + "','" + DYCode + "'");
                            sqlList.Add(sql);
                            //}
                            //else
                            //{
                            //    if (string.IsNullOrEmpty(provinceCode))
                            //        msg = "省份不能为空!";
                            //    if (string.IsNullOrEmpty(CityCode))
                            //        msg += " 城市不能为空!";
                            //    if (string.IsNullOrEmpty(SPCode))
                            //        msg += " 上牌员信息不能为空!";
                            //    if (string.IsNullOrEmpty(DYCode))
                            //        msg += " 抵押员信息不能为空!";

                            //    result.Result = "-1";
                            //    result.Message = msg;
                            //}
                        }

                        //if (string.IsNullOrEmpty(result.Message))
                        //{
                        foreach (string sql in sqlList)
                        {
                            int k = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteNonQuery(sql);
                        }
                        result.Result = "0";
                        result.Message = msg;
                        //}
                    }

                    if (System.IO.File.Exists(paths))
                    {
                        FileInfo file = new FileInfo(paths);
                        file.Attributes = FileAttributes.Normal;
                        file.Delete();
                    }
                }
                catch (Exception e)
                {
                    result.Result = "-1";
                    result.Message = e.Message;
                }

                context.Response.Write(json.Serialize(result));
            }
        }


        /// <summary>读取excel 到datatable    
        /// 默认第一行为表头，导入第一个工作表   
        /// </summary>      
        /// <param name="strFileName">excel文档路径</param>      
        /// <returns></returns>      
        public static DataTable ExcelToDataTable(string strFileName)
        {
            DataTable dt = new DataTable();
            FileStream file = null;
            IWorkbook Workbook = null;
            try
            {
                using (file = new FileStream(strFileName, FileMode.Open, FileAccess.Read))//C#文件流读取文件
                {
                    if (strFileName.IndexOf(".xlsx") > 0)
                        //把xlsx文件中的数据写入Workbook中
                        Workbook = new XSSFWorkbook(file);

                    else if (strFileName.IndexOf(".xls") > 0)
                        //把xls文件中的数据写入Workbook中
                        Workbook = new HSSFWorkbook(file);

                    if (Workbook != null)
                    {
                        ISheet sheet = Workbook.GetSheetAt(0);//读取第一个sheet
                        System.Collections.IEnumerator rows = sheet.GetRowEnumerator();
                        //得到Excel工作表的行 
                        IRow headerRow = sheet.GetRow(0);
                        //得到Excel工作表的总列数  
                        int cellCount = headerRow.LastCellNum;

                        for (int j = 0; j < cellCount; j++)
                        {
                            //得到Excel工作表指定行的单元格  
                            ICell cell = headerRow.GetCell(j);
                            dt.Columns.Add(cell.ToString());
                        }

                        for (int i = (sheet.FirstRowNum + 1); i <= sheet.LastRowNum; i++)
                        {
                            IRow row = sheet.GetRow(i);
                            DataRow dataRow = dt.NewRow();

                            for (int j = row.FirstCellNum; j < cellCount; j++)
                            {
                                if (row.GetCell(j) != null)
                                    dataRow[j] = row.GetCell(j).ToString();
                            }
                            dt.Rows.Add(dataRow);
                        }
                    }
                    return dt;
                }
            }

            catch (Exception)
            {
                if (file != null)
                {
                    file.Close();//关闭当前流并释放资源
                }
                return null;
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