using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessCenter
{
    public class ImportController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }
        public JsonResult ImportFile()
        {
            System.Web.HttpFileCollectionBase files = HttpContext.Request.Files; files = Request.Files;
            if (files == null || files.Count == 0) return null;
            string attachmentId = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            for (int i = 0; i < files.Count; i++)
            {
                HttpPostedFileBase file = files[i] as System.Web.HttpPostedFileBase;
                if (file.ContentLength == 0) continue;
                string sheetName = "sheet1";
                bool isFirstRowColumn = true;
                IWorkbook workbook = null;
                ISheet sheet = null;
                int startRow = 0;
                try
                {
                    workbook = WorkbookFactory.Create(file.InputStream);

                    if (sheetName != null)
                    {
                        sheet = workbook.GetSheet(sheetName);
                    }
                    else
                    {
                        sheet = workbook.GetSheetAt(0);
                    }
                    if (sheet != null)
                    {
                        IRow firstRow = sheet.GetRow(0);
                        int cellCount = firstRow.LastCellNum; //一行最后一个cell的编号 即总的列数
                        if (isFirstRowColumn)
                        {
                            for (int j = firstRow.FirstCellNum; j < cellCount; ++j)
                            {
                                DataColumn column = new DataColumn(firstRow.GetCell(j).StringCellValue);
                                data.Columns.Add(column);
                            }
                            startRow = sheet.FirstRowNum + 1;
                        }
                        else
                        {
                            startRow = sheet.FirstRowNum;
                        }
                        //最后一列的标号
                        int rowCount = sheet.LastRowNum;
                        for (int j = startRow; j <= rowCount; ++j)
                        {
                            IRow row = sheet.GetRow(j);
                            if (row == null) continue; //没有数据的行默认是null　　　　　　　

                            DataRow dataRow = data.NewRow();
                            for (int k = row.FirstCellNum; k < cellCount; ++k)
                            {
                                if (row.GetCell(k) != null) //同理，没有数据的单元格都默认是null
                                    dataRow[k] = row.GetCell(k).ToString();
                            }
                            data.Rows.Add(dataRow);
                        }
                    }
                }
                catch
                { }
                finally
                {
                    // stream.Close();
                }
            }
            if (data.Rows.Count > 0)
            {
                string[] list = new string[data.Rows.Count];
                for (int i = 0; i < data.Rows.Count; i++)
                {
                    string row = string.Empty;
                    for (int y = 0; y < data.Rows[i].ItemArray.Length; y++)
                    {
                        row += data.Rows[i].ItemArray[y] + ",";
                    }
                    row = row.TrimEnd(',');
                    list[i] = row;
                }
                return Json(list,"text/html" ,JsonRequestBehavior.AllowGet);
            }
            return Json(new string[0], JsonRequestBehavior.AllowGet);
        }
    }
}
