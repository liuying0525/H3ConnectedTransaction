using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace OThinker.H3.Controllers
{
    public class ImportDZController : Controller
    {
       


        public DataTable ImportFile()
        {
            System.Web.HttpFileCollectionBase files = HttpContext.Request.Files; files = Request.Files;
            if (files == null || files.Count == 0) return null;
            string attachmentId = Guid.NewGuid().ToString();
            DataTable data = new DataTable();
            List<string> participantColunms = new List<string>();
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
                                if (firstRow.GetCell(j).StringCellValue.Contains("名称$"))
                                {
                                    participantColunms.Add(firstRow.GetCell(j).StringCellValue);
                                }
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
                {
                }
                finally
                {
                    // stream.Close();
                }
            }

            return data;
        }
    }
}
