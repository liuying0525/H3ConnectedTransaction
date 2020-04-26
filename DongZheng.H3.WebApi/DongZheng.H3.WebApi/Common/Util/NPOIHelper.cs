using NPOI.SS.UserModel;
using NPOI.SS.Util;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Util
{
    public static class NPOIHelper
    {
        /// <summary>  
        /// DataTable导出到Excel的MemoryStream  
        /// </summary>  
        /// <param name="dtSource">源DataTable</param>  
        /// <param name="strHeaderText">表头文本</param>  
        public static XSSFWorkbook Export(DataTable dtSource, string strHeaderText,List<string> headers)
        {
            XSSFWorkbook workbook = new XSSFWorkbook();
            ISheet sheet = workbook.CreateSheet();

            ICellStyle dateStyle = workbook.CreateCellStyle();
            IDataFormat format = workbook.CreateDataFormat();
            dateStyle.DataFormat = format.GetFormat("yyyy-MM-dd");

            #region 取得每列的列宽（最大宽度）
            int[] arrColWidth = new int[dtSource.Columns.Count];
            foreach (DataColumn item in dtSource.Columns)
            {
                //GBK对应的code page是CP936
                arrColWidth[item.Ordinal] = Encoding.GetEncoding(936).GetBytes(item.ColumnName.ToString()).Length;
            }
            for (int i = 0; i < dtSource.Rows.Count; i++)
            {
                for (int j = 0; j < dtSource.Columns.Count; j++)
                {
                    int intTemp = Encoding.GetEncoding(936).GetBytes(dtSource.Rows[i][j].ToString()).Length;
                    if (intTemp > arrColWidth[j])
                    {
                        arrColWidth[j] = intTemp;
                    }
                }
            }
            #endregion

            int rowIndex = 0;



            #region 表头及样式
            //{
            //    IRow headerRow = sheet.CreateRow(0);
            //    headerRow.HeightInPoints = 25;
            //    headerRow.CreateCell(0).SetCellValue(strHeaderText);

            //    ICellStyle headStyle = workbook.CreateCellStyle();
            //    headStyle.Alignment = HorizontalAlignment.Center;
            //    IFont font = workbook.CreateFont();
            //    font.FontHeightInPoints = 20;
            //    font.Boldweight = 700;
            //    headStyle.SetFont(font);

            //    headerRow.GetCell(0).CellStyle = headStyle;

            //    sheet.AddMergedRegion(new CellRangeAddress(0, 0, 0, dtSource.Columns.Count - 1));
            //}
            #endregion


            #region 列头及样式
            {
                IRow headerRow = sheet.CreateRow(0);
                ICellStyle headStyle = workbook.CreateCellStyle();
                headStyle.Alignment = HorizontalAlignment.Center;
                IFont font = workbook.CreateFont();
                font.FontHeightInPoints = 10;
                font.Boldweight = 700;
                headStyle.SetFont(font);

                var headerIndex = 0;
                foreach (string headerName in headers)
                {
                    headerRow.CreateCell(headerIndex).SetCellValue(headerName);
                    headerRow.GetCell(headerIndex).CellStyle = headStyle;
                    headerIndex++;
                }
            }
            #endregion

            rowIndex = 1;
            
            foreach (DataRow row in dtSource.Rows)
            {
                #region 填充内容
                ICellStyle contentStyle = workbook.CreateCellStyle();
                contentStyle.Alignment = HorizontalAlignment.Left;
                IRow dataRow = sheet.CreateRow(rowIndex);
                foreach (DataColumn column in dtSource.Columns)
                {
                    ICell newCell = dataRow.CreateCell(column.Ordinal);
                    newCell.CellStyle = contentStyle;

                    string drValue = row[column].ToString();

                    switch (column.DataType.ToString())
                    {
                        case "System.String"://字符串类型  
                            newCell.SetCellValue(drValue);
                            break;
                        case "System.DateTime"://日期类型  
                            DateTime dateV;
                            DateTime.TryParse(drValue, out dateV);
                            newCell.SetCellValue(dateV);

                            newCell.CellStyle = dateStyle;//格式化显示  
                            break;
                        case "System.Boolean"://布尔型  
                            bool boolV = false;
                            bool.TryParse(drValue, out boolV);
                            newCell.SetCellValue(boolV);
                            break;
                        case "System.Int16"://整型  
                        case "System.Int32":
                        case "System.Int64":
                        case "System.Byte":
                            int intV = 0;
                            int.TryParse(drValue, out intV);
                            newCell.SetCellValue(intV);
                            break;
                        case "System.Decimal"://浮点型  
                        case "System.Double":
                            double doubV = 0;
                            double.TryParse(drValue, out doubV);
                            newCell.SetCellValue(doubV);
                            break;
                        case "System.DBNull"://空值处理  
                            newCell.SetCellValue("");
                            break;
                        default:
                            newCell.SetCellValue(drValue);
                            break;
                    }

                }
                #endregion

                rowIndex++;
            }

            return workbook;
            //using (MemoryStream ms = new MemoryStream())
            //{
            //    workbook.Write(ms);
            //    ms.Flush();
            //    //sheet.Dispose();
            //    //workbook.Dispose();//一般只用写这一个就OK了，他会遍历并释放所有资源，但当前版本有问题所以只释放sheet  
            //    return ms;
            //}

        }
    }

    public class NpoiMemoryStream : MemoryStream
    {
        public NpoiMemoryStream()
        {
            AllowClose = true;
        }

        public bool AllowClose { get; set; }

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}