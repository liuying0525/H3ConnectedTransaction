using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.HSSF.UserModel;
using System.Data;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// ExportExcel 的摘要说明
    /// </summary>
    public static class ExportExcel
    {
        /// <summary>
        /// 导出入口，返回下载路径
        /// </summary>
        /// <param name="Page"></param>
        /// <param name="ReportName"></param>
        /// <param name="TableHeads"></param>
        /// <param name="TableDatas"></param>
        /// <returns></returns>
        public static string ExportExecl(
            string ReportName,
            Dictionary<string, string> TableHeads,
            List<Dictionary<string, object>> TableDatas)
        {
            IWorkbook workbook = new HSSFWorkbook();
            ISheet sheet = workbook.CreateSheet("sheet1");

            int count = 0;
            // 生成标题行
            IRow row = sheet.CreateRow(count++);
            int headerIndex = 0;
            foreach (string columnName in TableHeads.Keys)
            {
                row.CreateCell(headerIndex++).SetCellValue(TableHeads[columnName]);
            }
            //生成数据
            foreach (Dictionary<string, object> data in TableDatas)
            {
                row = sheet.CreateRow(count++);
                int bodyIndex = 0;
                foreach (string key in TableHeads.Keys)
                {
                    row.CreateCell(bodyIndex++).SetCellValue(data[key]!=null?data[key].ToString():"");
                }
            }

            //StringBuilder OutFileContent = new StringBuilder();
            //OutFileContent.AppendLine(AddHeadFile());
            //OutFileContent.AppendLine(AddContentFile(TableHeads, TableDatas));
            //OutFileContent.AppendLine(AddEndFile());

            MemoryStream ms = new MemoryStream();
            workbook.Write(ms);
            ms.Position = 0;

            string strPath = System.Web.HttpContext.Current.Server.MapPath("~/TempImages/");
            string strExcelFile = strPath + ReportName + ".xls";
            FileStream OutFile = new FileStream(strExcelFile, FileMode.Create, FileAccess.Write);
            byte[] btArray = ms.ToArray();
            OutFile.Write(btArray, 0, btArray.Length);
            OutFile.Flush();
            OutFile.Close();

            return AppConfig.PortalRoot + "/TempImages/" + ReportName + ".xls";
        }

        #region 文件开头
        /// <summary>
        /// 文件开头
        /// </summary>
        /// <param name="OutFileContent"></param>
        /// <returns></returns>
        private static string AddHeadFile()
        {
            StringBuilder OutFileContent = new StringBuilder();

            OutFileContent.Append("<?xml version=\"1.0\"?>\r\n");
            OutFileContent.Append("<?mso-application progid=\"Excel.Sheet\"?>\r\n");
            OutFileContent.Append("<Workbook xmlns=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n");
            OutFileContent.Append(" xmlns:o=\"urn:schemas-microsoft-com:office:office\"\r\n");
            OutFileContent.Append(" xmlns:x=\"urn:schemas-microsoft-com:office:excel\"\r\n");
            OutFileContent.Append(" xmlns:ss=\"urn:schemas-microsoft-com:office:spreadsheet\"\r\n");
            OutFileContent.Append(" xmlns:html=\"http://www.w3.org/TR/REC-html40\">\r\n");
            OutFileContent.Append(" <DocumentProperties xmlns=\"urn:schemas-microsoft-com:office:office\">\r\n");
            OutFileContent.Append("  <Author>panss</Author>\r\n");
            OutFileContent.Append("  <LastAuthor></LastAuthor>\r\n");
            OutFileContent.Append("  <Created>" + DateTime.Now.ToShortDateString() + "</Created>\r\n");
            OutFileContent.Append("  <Company>Prcedu</Company>\r\n");
            OutFileContent.Append("  <Version>12.00</Version>\r\n");
            OutFileContent.Append(" </DocumentProperties>\r\n");
            OutFileContent.Append(" <OfficeDocumentSettings xmlns=\"urn:schemas-microsoft-com:office:office\">\r\n");
            OutFileContent.Append("  <DownloadComponents/>\r\n");

            //OutFileContent.Append("  <LocationOfComponents HRef=\"file:///F:\\Tools\\OfficeXP\\OfficeXP\\\"/>\r\n");

            OutFileContent.Append(" </OfficeDocumentSettings>\r\n");
            OutFileContent.Append(" <ExcelWorkbook xmlns=\"urn:schemas-microsoft-com:office:excel\">\r\n");
            OutFileContent.Append("  <WindowHeight>9000</WindowHeight>\r\n");
            OutFileContent.Append("  <WindowWidth>10620</WindowWidth>\r\n");
            OutFileContent.Append("  <WindowTopX>480</WindowTopX>\r\n");
            OutFileContent.Append("  <WindowTopY>45</WindowTopY>\r\n");
            OutFileContent.Append("  <ProtectStructure>False</ProtectStructure>\r\n");
            OutFileContent.Append("  <ProtectWindows>False</ProtectWindows>\r\n");
            OutFileContent.Append(" </ExcelWorkbook>\r\n");
            OutFileContent.Append(" <Styles>\r\n");
            OutFileContent.Append("  <Style ss:ID=\"Default\" ss:Name=\"Normal\">\r\n");
            OutFileContent.Append("   <Alignment ss:Vertical=\"Center\" />\r\n");
            OutFileContent.Append("   <Borders/>\r\n");

            OutFileContent.Append("   <Font ss:FontName=\"宋体\" x:CharSet=\"134\" ss:Size=\"12\"/>\r\n");

            OutFileContent.Append("   <Interior/>\r\n");
            OutFileContent.Append("   <NumberFormat/>\r\n");
            OutFileContent.Append("   <Protection/>\r\n");
            OutFileContent.Append("  </Style>\r\n");
            OutFileContent.Append("  <Style ss:ID=\"s62\">\r\n");
            OutFileContent.Append("   <Alignment ss:Vertical=\"Center\" ss:Horizontal=\"Center\" ss:WrapText=\"1\"/>\r\n");

            OutFileContent.Append("   <Font ss:FontName=\"宋体\" x:CharSet=\"134\" ss:Size=\"9\"/>\r\n");
            OutFileContent.Append("  </Style>\r\n");
            OutFileContent.Append("  <Style ss:ID=\"s74\">\r\n");
            OutFileContent.Append("   <Alignment ss:Horizontal=\"Center\" ss:Vertical=\"Center\"/>\r\n");
            OutFileContent.Append("   <Borders>\r\n");
            OutFileContent.Append("  <Border ss:Position=\"Bottom\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\r\n");
            OutFileContent.Append("  <Border ss:Position=\"Left\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\r\n");
            OutFileContent.Append("  <Border ss:Position=\"Right\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\r\n");
            OutFileContent.Append("  <Border ss:Position=\"Top\" ss:LineStyle=\"Continuous\" ss:Weight=\"1\"/>\r\n");
            OutFileContent.Append("  </Borders>\r\n");
            OutFileContent.Append("   <Font ss:FontName=\"宋体\" x:CharSet=\"134\" ss:Size=\"12\" ss:Bold=\"1\"/>\r\n");
            OutFileContent.Append("   <Interior ss:Color=\"#BFBFBF\" ss:Pattern=\"Solid\"/>\r\n");
            OutFileContent.Append("  </Style>\r\n");
            OutFileContent.Append(" </Styles>\r\n");
            OutFileContent.Append(" <Worksheet ss:Name=\"Sheet1\">\r\n");
            OutFileContent.Append("  <Table ss:ExpandedColumnCount=\"255\" x:FullColumns=\"1\" \r\n");
            OutFileContent.Append("x:FullRows=\"1\" ss:StyleID=\"s62\" ss:DefaultColumnWidth=\"75\" ss:DefaultRowHeight=\"20.25\">\r\n");
            OutFileContent.Append("<Column ss:StyleID=\"s62\" ss:AutoFitWidth=\"0\" ss:Width=\"112.5\"/>\r\n");
            return OutFileContent.ToString();
        }
        #endregion

        #region 文件结束
        /// <summary>
        ///  文件结束
        /// </summary>
        /// <param name="OutFileContent"></param>
        /// <returns></returns>
        private static string AddEndFile()
        {
            StringBuilder OutFileContent = new StringBuilder();
            OutFileContent.Append("</Table>\r\n");
            OutFileContent.Append("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">\r\n");
            OutFileContent.Append("<Unsynced/>\r\n");
            OutFileContent.Append("<Print>\r\n");
            OutFileContent.Append("    <ValidPrinterInfo/>\r\n");
            OutFileContent.Append("    <PaperSizeIndex>9</PaperSizeIndex>\r\n");
            OutFileContent.Append("    <HorizontalResolution>600</HorizontalResolution>\r\n");
            OutFileContent.Append("    <VerticalResolution>0</VerticalResolution>\r\n");
            OutFileContent.Append("</Print>\r\n");
            OutFileContent.Append("<Selected/>\r\n");
            OutFileContent.Append("<Panes>\r\n");
            OutFileContent.Append("    <Pane>\r\n");
            OutFileContent.Append("    <Number>3</Number>\r\n");
            OutFileContent.Append("    <RangeSelection>R1:R65536</RangeSelection>\r\n");
            OutFileContent.Append("    </Pane>\r\n");
            OutFileContent.Append("</Panes>\r\n");
            OutFileContent.Append("<ProtectObjects>False</ProtectObjects>\r\n");
            OutFileContent.Append("<ProtectScenarios>False</ProtectScenarios>\r\n");
            OutFileContent.Append("</WorksheetOptions>\r\n");
            OutFileContent.Append("</Worksheet>\r\n");
            OutFileContent.Append("<Worksheet ss:Name=\"Sheet2\">\r\n");
            OutFileContent.Append("<Table ss:ExpandedColumnCount=\"1\" ss:ExpandedRowCount=\"1\" x:FullColumns=\"1\"\r\n");
            OutFileContent.Append("x:FullRows=\"1\" ss:DefaultColumnWidth=\"54\" ss:DefaultRowHeight=\"14.25\">\r\n");
            OutFileContent.Append("<Row ss:AutoFitHeight=\"0\"/>\r\n");
            OutFileContent.Append("</Table>\r\n");
            OutFileContent.Append("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">\r\n");
            OutFileContent.Append("<Unsynced/>\r\n");
            OutFileContent.Append("<ProtectObjects>False</ProtectObjects>\r\n");
            OutFileContent.Append("<ProtectScenarios>False</ProtectScenarios>\r\n");
            OutFileContent.Append("</WorksheetOptions>\r\n");
            OutFileContent.Append("</Worksheet>\r\n");
            OutFileContent.Append("<Worksheet ss:Name=\"Sheet3\">\r\n");
            OutFileContent.Append("<Table ss:ExpandedColumnCount=\"1\" ss:ExpandedRowCount=\"1\" x:FullColumns=\"1\"\r\n");
            OutFileContent.Append("x:FullRows=\"1\" ss:DefaultColumnWidth=\"54\" ss:DefaultRowHeight=\"14.25\">\r\n");
            OutFileContent.Append("<Row ss:AutoFitHeight=\"0\"/>\r\n");
            OutFileContent.Append("</Table>\r\n");
            OutFileContent.Append("<WorksheetOptions xmlns=\"urn:schemas-microsoft-com:office:excel\">\r\n");
            OutFileContent.Append("<Unsynced/>\r\n");
            OutFileContent.Append("<ProtectObjects>False</ProtectObjects>\r\n");
            OutFileContent.Append("<ProtectScenarios>False</ProtectScenarios>\r\n");
            OutFileContent.Append("</WorksheetOptions>\r\n");
            OutFileContent.Append("</Worksheet>\r\n");
            OutFileContent.Append("</Workbook>\r\n");
            return OutFileContent.ToString();
        }
        #endregion

        #region 添加文件内容
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tableHeads"></param>
        /// <param name="tableDatas"></param>
        /// <returns></returns>
        private static string AddContentFile(Dictionary<string, string> tableHeads, List<Dictionary<string, object>> tableDatas)
        {
            StringBuilder OutFileContent = new StringBuilder();
            OutFileContent.Append("<Row ss:AutoFitHeight=\"0\">");
            foreach (string key in tableHeads.Keys)
            {
                OutFileContent.Append("<Cell><Data ss:Type=\"String\">" + tableHeads[key] + "</Data></Cell>");
            }
            OutFileContent.Append("</Row>");

            foreach (Dictionary<string, object> data in tableDatas)
            {
                OutFileContent.Append("<Row ss:AutoFitHeight=\"0\">");
                foreach (string key in tableHeads.Keys)
                {
                    OutFileContent.Append("<Cell><Data ss:Type=\"String\">" + data[key] + "</Data></Cell>");
                }
                OutFileContent.Append("</Row>");
            }

            return OutFileContent.ToString();
        }
        #endregion

        public static string ExportCsv(
            string ReportName,
            Dictionary<string, string> TableHeads,
            List<Dictionary<string, object>> TableDatas)
        {
            DataTable dt = new DataTable();

            string strPath = System.Web.HttpContext.Current.Server.MapPath("~/TempImages/");
            ReportName += strPath + "_" + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv"; ;
            string path = System.Web.HttpContext.Current.Server.MapPath(ReportName);
            System.IO.FileStream fs = new FileStream(path, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, new System.Text.UnicodeEncoding());

            for (int i = 0; i < dt.Columns.Count; i++)
            {
                sw.Write(dt.Columns[i].ColumnName);
                sw.Write("\t");
            }
            sw.WriteLine("");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    sw.Write(DelQuota(dt.Rows[i][j] + string.Empty));
                    sw.Write("\t");
                }
                sw.WriteLine("");
            }
            sw.Flush();
            sw.Close();

            return AppConfig.PortalRoot + "/TempImages/" + ReportName;
        }

        /// <summary>
        /// Delete special symbol
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string DelQuota(string str)
        {
            string result = str;
            string[] strQuota = { "~", "!", "@", "#", "$", "%", "^", "&", "*", "(", ")", "`", ";", "'", ",", ".", "/", ":", "/,", "<", ">", "?" };
            for (int i = 0; i < strQuota.Length; i++)
            {
                if (result.IndexOf(strQuota[i]) > -1)
                    result = result.Replace(strQuota[i], "");
            }
            return result;
        }
    }
}