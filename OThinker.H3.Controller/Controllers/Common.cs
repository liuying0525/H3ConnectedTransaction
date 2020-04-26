using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers
{
    /// <summary>
    /// 公共方法类
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// 将Excel转化为数据表
        /// </summary>
        /// <param name="fileStream">文件流</param>
        /// <returns>数据表</returns>
        public static DataTable ExcelToDataTable(Stream fileStream)
        {
            IWorkbook workbook = new HSSFWorkbook(fileStream);
            ISheet sheet = workbook.GetSheetAt(0);
            DataTable dt = new DataTable();

            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;
            int rowCount = sheet.LastRowNum;

            for (int i = headerRow.FirstCellNum; i < cellCount; i++)
            {
                DataColumn column = new DataColumn(headerRow.GetCell(i).StringCellValue);
                dt.Columns.Add(column);
            }

            for (int i = (sheet.FirstRowNum + 1); i <= rowCount; i++)
            {
                IRow row = sheet.GetRow(i);
                DataRow dataRow = dt.NewRow();
                if (row != null)
                {
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            dataRow[j] = row.GetCell(j).ToString();
                        }
                    }
                }
                dt.Rows.Add(dataRow);
            }

            workbook = null;
            sheet = null;

            return dt;
        }
        /// <summary>
        /// 替换字符串特殊符号
        /// </summary>
        /// <param name="str">目标字符串</param>
        /// <returns>替换后的字符串</returns>
        public static string TrimHtml(string str)
        {
            return str.Replace("&", string.Empty)
                .Replace("<", string.Empty)
                .Replace("'", string.Empty)
                .Replace("\"", string.Empty)
                .Replace("\\", string.Empty);
        }

    }
}
