
using OThinker.Reporting;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace OThinker.H3.Controllers.Reporting
{
    public class ReportBase : ControllerBase
    {
        public override string FunctionCode
        {
            get { return ""; }
        }

        #region 汇总表和明细表数据导出
        /// <summary>
        /// 汇总表导出
        /// </summary>
        /// <param name="ValueTable"></param>
        /// <param name="MyColumnTread"></param>
        /// <param name="RowSimpleHeaderNodes"></param>
        /// <param name="MyCombineReport"></param>
        /// <param name="ExcelName"></param>
        /// <param name="allReportWidgetColumns"></param>
        /// <param name="UnitTableAndAssociationTable"></param>
        protected void CombineExportToExcel(
            List<List<string>> ValueTable,
            Dictionary<string, List<MyTd>> MyColumnTread,
            List<SimpleHeaderNode> RowSimpleHeaderNodes,
            CombineReport MyCombineReport, string ExcelName,
            Dictionary<string, ReportWidgetColumn> allReportWidgetColumns,
            Dictionary<string, string> UnitTableAndAssociationTable,
            ReportWidgetColumn[] Columns
            )
        {
            XSSFWorkbook book = new XSSFWorkbook();
            #region 格式
            ICellStyle style = book.CreateCellStyle();
            style.WrapText = true;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

            #endregion
            ISheet sheet = book.CreateSheet("Sheet1");
            List<string>[] ArrayValueTable = ValueTable.ToArray();
            Dictionary<int, int> RowBeginNum = new Dictionary<int, int>();
            //--------------------------------------------导出逻辑--------------------------------------
            int i = 0;
            {
                //列表头error没有处理汇总列
                foreach (string ColumnCode in MyColumnTread.Keys)
                {
                    sheet.CreateRow(i);
                    IRow row = sheet.GetRow(i);
                    int j = 0;
                    if (RowBeginNum.ContainsKey(i))
                        j = RowBeginNum[i];
                    foreach (MyTd Td in MyColumnTread[ColumnCode])
                    {
                        ICell cell = row.CreateCell(j);
                        cell.SetCellValue(Td.Value);
                        cell.CellStyle = style;
                        if (Td.ColSpan > 1 || Td.RowSpan > 1)
                        {
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(i, i + Td.RowSpan - 1, j, j + Td.ColSpan - 1));
                            if (j == 0 && Td.RowSpan > 1)
                            {
                                for (var k = i; k < i + Td.RowSpan; k++)
                                {
                                    if (!RowBeginNum.ContainsKey(k))
                                        RowBeginNum.Add(k, j + Td.ColSpan);
                                }
                            }
                        }
                        j = j + Td.ColSpan;
                    }
                    i++;
                }
            }
            {
                int ColumnHeaderNum = i;
                //行表头和数据
                foreach (SimpleHeaderNode Node in RowSimpleHeaderNodes)
                {
                    int j = 0;
                    sheet.CreateRow(i);
                    IRow myrow = sheet.GetRow(i);
                    BuidRowHeaderAndValues(sheet, Node, i, j, myrow, ArrayValueTable, ColumnHeaderNum, allReportWidgetColumns, UnitTableAndAssociationTable, style, Columns);
                    i = i + Node.RowSpan;
                }
            }
            //----------------------------------------end导出逻辑---------------------------------------

            ExcelName = ExcelName + "_" + DateTime.Now.ToString("yyyy-MM-dd");
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                book.Write(ms);
                Response.Clear();
                //Response.ContentEncoding = encoding;
                Response.Charset = "UTF-8";
                Response.ContentType = "application/ms-excel;charset=UFT-8";
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", ExcelName));
                //byte[] bytes = Encoding.UTF8.GetBytes(ms.ToString());
                //Response.AddHeader("Content-Length", bytes.Length.ToString());
                Response.BinaryWrite(ms.ToArray());
                Response.Flush();
                Response.Close();
                book = null;
                ms.Close();
                ms.Dispose();
            }
        }

        /// <summary>
        /// 明细表导出
        /// </summary>
        /// <param name="SourceData"></param>
        /// <param name="ResultSourceColumns"></param>
        /// <param name="ExcelName"></param>
        /// <param name="ChildCodes"></param>
        protected void DetailExportToExcel(List<Dictionary<string, object>> SourceData, List<OThinker.Reporting.ReportWidgetColumn> ResultSourceColumns, string ExcelName, Dictionary<string, ColumnSummary> ChildCodes)
        {
            XSSFWorkbook book = new XSSFWorkbook();
            ISheet sheet = book.CreateSheet("Sheet1");
            int firstcolumnnum = 0;
            int secondcolumnnum = 0;
            #region 格式
            ICellStyle style = book.CreateCellStyle();
            style.WrapText = true;
            style.VerticalAlignment = VerticalAlignment.Center;
            style.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Left;

            #endregion
            HashSet<string> childecodes = new HashSet<string>();
            //--------------------------------------------导出逻辑--------------------------------------
            if (ChildCodes != null && ChildCodes.Count > 0)
            {
                #region 有子表
                //表头，
                sheet.CreateRow(0);
                IRow firstrow = sheet.GetRow(0);
                sheet.CreateRow(1);
                IRow secondrow = sheet.GetRow(1);
                int i = 0;
                foreach (string key in ChildCodes.Keys)
                {
                    ColumnSummary node = ChildCodes[key];
                    if (node.ChildeColumnSummary != null && node.ChildeColumnSummary.Count > 0)
                    {
                        ICell cell = firstrow.CreateCell(firstcolumnnum);
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 0, firstcolumnnum, firstcolumnnum + node.ChildeColumnSummary.Count - 1));
                        cell.SetCellValue(node.DisplayName);
                        firstcolumnnum = firstcolumnnum + node.ChildeColumnSummary.Count;
                        foreach (string childkey in node.ChildeColumnSummary.Keys)
                        {
                            childecodes.Add(childkey);
                            ColumnSummary childnode = node.ChildeColumnSummary[childkey];
                            ICell childcell = secondrow.CreateCell(secondcolumnnum);
                            childcell.SetCellValue(childnode.DisplayName);
                            childcell.CellStyle = style;
                            secondcolumnnum++;
                        }
                    }
                    else
                    {
                        ICell cell = firstrow.CreateCell(firstcolumnnum);
                        cell.SetCellValue(node.DisplayName);
                        cell.CellStyle = style;
                        sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(0, 1, firstcolumnnum, firstcolumnnum));
                        firstcolumnnum++;
                        secondcolumnnum++;
                    }
                    i++;
                }
                //end表头
                //表体
                int RowNum = 2;
                int ColumnNum = 0;
                foreach (Dictionary<string, object> bo in SourceData)
                {
                    ColumnNum = 0;
                    sheet.CreateRow(RowNum);
                    IRow row = sheet.GetRow(RowNum);
                    int MaxHeight = 0;
                    //取子表最大行数
                    foreach (string q in childecodes)
                    {
                        if (bo.ContainsKey(q))
                        {
                            List<string> values = (List<string>)bo[q];
                            if (values != null)
                            {
                                if (values.Count > MaxHeight)
                                {
                                    MaxHeight = values.Count;
                                }
                            }
                        }
                    }
                    //准备行
                    if (MaxHeight > 1)
                    {
                        for (var tempnum = RowNum + 1; tempnum < RowNum + MaxHeight; tempnum++)
                        {
                            sheet.CreateRow(tempnum);
                        }
                    }
                    foreach (OThinker.Reporting.ReportWidgetColumn Column in ResultSourceColumns)
                    {

                        if (childecodes.Contains(Column.ColumnCode))
                        {

                            //子表从上到下填数据，从rownum到rownum+maxheight-1行
                            List<string> values = (List<string>)bo[Column.ColumnCode];
                            if (values != null && values.Count > 0)
                            {
                                int childRownum = RowNum;
                                foreach (string v in values)
                                {
                                    IRow childrow = sheet.GetRow(childRownum);
                                    ICell childcell = childrow.CreateCell(ColumnNum);
                                    switch (Column.ColumnType)
                                    {
                                        case ColumnType.Numeric:
                                            double tempresult = 0;
                                            if (double.TryParse(v, out tempresult))
                                            {
                                                childcell.SetCellValue(tempresult);
                                            }
                                            else
                                            {
                                                childcell.SetCellValue(v);
                                            }
                                            break;
                                        default:
                                            childcell.SetCellValue(v);
                                            break;
                                    }
                                    childcell.CellStyle = style;
                                    childRownum++;
                                }
                            }

                        }
                        else
                        {
                            ICell cell = row.CreateCell(ColumnNum);
                            switch (Column.ColumnType)
                            {
                                case ColumnType.Numeric:
                                    double tempresult = 0;
                                    if (double.TryParse(bo[Column.ColumnCode].ToString(), out tempresult))
                                    {
                                        cell.SetCellValue(tempresult);
                                    }
                                    else
                                    {
                                        cell.SetCellValue(bo[Column.ColumnCode].ToString());
                                    }
                                    break;
                                default:
                                    cell.SetCellValue(bo[Column.ColumnCode].ToString());
                                    break;
                            }
                            cell.CellStyle = style;
                            sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(RowNum, RowNum + MaxHeight - 1, ColumnNum, ColumnNum));
                        }
                        ColumnNum++;
                    }
                    RowNum++;
                }
                //end表体
                #endregion
            }
            else
            {
                #region 无子表
                //表头，如果有主表子表，需要改，暂时不改
                sheet.CreateRow(0);
                IRow firstrow = sheet.GetRow(0);
                int i = 0;
                foreach (OThinker.Reporting.ReportWidgetColumn Column in ResultSourceColumns)
                {
                    ICell cell = firstrow.CreateCell(i);
                    cell.SetCellValue(Column.DisplayName);
                    cell.CellStyle = style;
                    i++;
                }
                //end表头
                //表体
                int RowNum = 1;
                int ColumnNum = 0;
                foreach (Dictionary<string, object> bo in SourceData)
                {
                    ColumnNum = 0;

                    sheet.CreateRow(RowNum);
                    IRow row = sheet.GetRow(RowNum);
                    foreach (OThinker.Reporting.ReportWidgetColumn Column in ResultSourceColumns)
                    {
                        ICell cell = row.CreateCell(ColumnNum);
                        switch (Column.ColumnType)
                        {
                            case ColumnType.Numeric:
                                double tempresult = 0;
                                if (double.TryParse(bo[Column.ColumnCode].ToString(), out tempresult))
                                {
                                    cell.SetCellValue(tempresult);
                                }
                                else
                                {
                                    cell.SetCellValue(bo[Column.ColumnCode].ToString());
                                }
                                break;
                            default:
                                cell.SetCellValue(bo[Column.ColumnCode].ToString());
                                break;
                        }
                        cell.CellStyle = style;
                        ColumnNum++;
                    }
                    RowNum++;
                }
                //end表体
                #endregion
            }

            //----------------------------------------end导出逻辑---------------------------------------
            ExcelName = ExcelName + "_" + DateTime.Now.ToString("yyyy-MM-dd");
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                book.Write(ms);

                Response.Clear();
                Response.Buffer = true;
                Response.Charset = "UTF-8";
                Response.ContentType = "application/ms-excel;charset=UFT-8";
                Response.AddHeader("Content-Disposition", string.Format("attachment; filename={0}.xlsx", ExcelName));

                Response.BinaryWrite(ms.ToArray());
                // Response.OutputStream.ToString();
                Response.Flush();
                // Response.Close();
                Response.End();
                book = null;
                ms.Close();
                ms.Dispose();

            }
        }

        #endregion 

        #region 数据操作的基础方法 包含数据表头等数据的构建
        /// <summary>
        /// 构建行表头和数据
        /// </summary>
        /// <param name="sheet"></param>
        /// <param name="Node"></param>
        /// <param name="RowNumber"></param>
        /// <param name="ColumnNumber"></param>
        /// <param name="CurrentRow"></param>
        /// <param name="ArrayValueTable"></param>
        /// <param name="ColumnHeaderNumber"></param>
        /// <param name="allReportWidgetColumns"></param>
        /// <param name="UnitTableAndAssociationTable"></param>
        /// <param name="style"></param>
        /// <param name="Columns"></param>
        private void BuidRowHeaderAndValues(
            ISheet sheet,
            SimpleHeaderNode Node,
            int RowNumber,
            int ColumnNumber,
            IRow CurrentRow,
            List<string>[] ArrayValueTable,
            int ColumnHeaderNumber,
            Dictionary<string, ReportWidgetColumn> allReportWidgetColumns,
            Dictionary<string, string> UnitTableAndAssociationTable,
            ICellStyle style,
            ReportWidgetColumn[] Columns)
        {
            int Counter = 0;
            ICell cell = CurrentRow.CreateCell(ColumnNumber);
            string nodevalue = Node.Value;
            if (Node.ColumnCode != null && allReportWidgetColumns.ContainsKey(Node.ColumnCode) && (allReportWidgetColumns[Node.ColumnCode].ColumnType == ColumnType.Association || allReportWidgetColumns[Node.ColumnCode].ColumnType == ColumnType.Unit) && UnitTableAndAssociationTable.ContainsKey(Node.Value))
            {
                nodevalue = UnitTableAndAssociationTable[Node.Value];
            }
            if (Node.ColumnCode != null && allReportWidgetColumns.ContainsKey(Node.ColumnCode))
            {
                switch (allReportWidgetColumns[Node.ColumnCode].ColumnType)
                {
                    case ColumnType.Numeric:
                        double tempresult = 0;
                        if (double.TryParse(nodevalue, out tempresult))
                        {
                            cell.SetCellValue(tempresult);
                        }
                        else
                        {
                            cell.SetCellValue(nodevalue);
                        }
                        break;
                    default:
                        cell.SetCellValue(nodevalue);
                        break;
                }
            }
            else
            {
                cell.SetCellValue(nodevalue);
            }
            cell.CellStyle = style;
            if (Node.ColSpan > 1 || Node.RowSpan > 1)
            {
                sheet.AddMergedRegion(new NPOI.SS.Util.CellRangeAddress(RowNumber, RowNumber + Node.RowSpan - 1, ColumnNumber, ColumnNumber + Node.ColSpan - 1));
            }
            ColumnNumber = ColumnNumber + Node.ColSpan;
            if (Node.ChildNode == null || Node.ChildNode.Count == 0)
            {
                //这里加数据表内的数据
                int tempcounter = 0;
                foreach (string value in ArrayValueTable[RowNumber - ColumnHeaderNumber])
                {
                    ReportWidgetColumn itemColumn = null;
                    if (Columns != null & Columns.Length > 0)
                    {
                        itemColumn = Columns[tempcounter % Columns.Length];
                    }
                    ICell mycell = CurrentRow.CreateCell(ColumnNumber);
                    //类型转换
                    if (itemColumn != null)
                    {
                        switch (itemColumn.ColumnType)
                        {
                            case ColumnType.Numeric:
                                double tempresult = 0;
                                if (double.TryParse(value, out tempresult))
                                {
                                    mycell.SetCellValue(tempresult);
                                }
                                else
                                {
                                    mycell.SetCellValue(value);
                                }
                                break;
                            default:
                                mycell.SetCellValue(value);
                                break;
                        }
                    }
                    else
                    {
                        mycell.SetCellValue(value);
                    }

                    mycell.CellStyle = style;
                    ColumnNumber++;
                    tempcounter++;
                }
                return;
            }
            else
            {
                foreach (SimpleHeaderNode ChildNode in Node.ChildNode)
                {
                    if (Counter == 0)
                    {
                        BuidRowHeaderAndValues(sheet, ChildNode, RowNumber, ColumnNumber, CurrentRow, ArrayValueTable, ColumnHeaderNumber, allReportWidgetColumns, UnitTableAndAssociationTable, style, Columns);

                    }
                    else
                    {
                        IRow myrow = sheet.CreateRow(RowNumber);
                        BuidRowHeaderAndValues(sheet, ChildNode, RowNumber, ColumnNumber, myrow, ArrayValueTable, ColumnHeaderNumber, allReportWidgetColumns, UnitTableAndAssociationTable, style, Columns);
                    }
                    RowNumber = RowNumber + ChildNode.RowSpan;
                    Counter++;
                }
            }
        }
       
        /// <summary>
        /// 报表，删除defautcode
        /// </summary>
        /// <param name="Setting"></param>
        protected void RemoveDefautcodeFromSC(ReportWidget Setting)
        {
            List<ReportWidgetColumn> columns = new List<ReportWidgetColumn>();
            if (Setting.Series != null && Setting.Series.Length > 0)
            {
                foreach (ReportWidgetColumn q in Setting.Series)
                {

                    if (q.ColumnCode == ReportWidgetColumn.DefaultCountCode)
                    {
                        continue;
                    }
                    columns.Add(q);
                }
                Setting.Series = columns.ToArray();
            }
            columns = new List<ReportWidgetColumn>();
            if (Setting.Categories != null && Setting.Categories.Length > 0)
            {
                foreach (ReportWidgetColumn q in Setting.Categories)
                {

                    if (q.ColumnCode == ReportWidgetColumn.DefaultCountCode)
                    {
                        continue;
                    }
                    columns.Add(q);
                }
                Setting.Categories = columns.ToArray();
            }
        }
        /// <summary>
        /// 根据数据库类型获取简易看板数据源的列
        /// </summary>
        /// <param name="Filters"></param>
        /// <param name="SimpleBoard"></param>
        /// <param name="ReportSource"></param>
        /// <param name="dbType"></param>
        /// <returns></returns>
        protected List<ReportWidgetColumn> GetSourceColumns(ReportFilter[] Filters, ReportWidgetSimpleBoard SimpleBoard, ReportSource ReportSource, OThinker.Data.Database.DatabaseType dbType)
        {
            List<ReportWidgetColumn> SourceColumns = this.Engine.ReportQuery.GetSourceColumns(this.Engine, SimpleBoard, ReportSource);
            List<ReportWidgetColumn> results = new List<ReportWidgetColumn>();
            foreach (ReportWidgetColumn c in SourceColumns)
            {
                if (SimpleBoard.ContainColumn(Filters, c.ColumnCode, ReportSource, this.Engine.BizObjectManager, this.Engine.Organization, dbType))
                {
                    results.Add(c);
                }
            }

            return results;
        }

        /// <summary>
        /// 获取报表数据源的列集合
        /// </summary>
        /// <param name="Filters"></param>
        /// <param name="reportWidget"></param>
        /// <param name="ReportSource"></param>
        /// <param name="ChildCodes"></param>
        /// <returns></returns>
        protected List<ReportWidgetColumn> GetSourceColumns(ReportFilter[] Filters, ReportWidget reportWidget, ReportSource ReportSource, out Dictionary<string, ColumnSummary> ChildCodes)
        {

            ChildCodes = new Dictionary<string, ColumnSummary>();
            List<ReportWidgetColumn> SourceColumns = this.Engine.ReportQuery.GetSourceColumns(this.Engine, reportWidget, ReportSource);
            List<ReportWidgetColumn> results = new List<ReportWidgetColumn>();
            foreach (ReportWidgetColumn c in SourceColumns)
            {
                if (reportWidget.ContainColumn(Filters, c.ColumnCode, ReportSource, this.Engine.BizObjectManager, this.Engine.Organization, this.Engine.EngineConfig.DBType, c.DisplayName))
                {
                    results.Add(c);
                }
            }
            //设置主表id为，I_ccc.objectid
            string MainObjectId = this.GetMainObjectId(ReportSource.SchemaCode);
            string MainObjectId_Name = this.GetMainObjectId_Name(ReportSource.SchemaCode);
            //当有主表和子表字段时，加column 主表objectid，用于明细表关联
            this.DetailNeedMainObjectId(reportWidget, ReportSource, out ChildCodes);
            ReportWidgetColumn MainObjectIdColumn = new ReportWidgetColumn();
            MainObjectIdColumn.ColumnCode = MainObjectId_Name;
            MainObjectIdColumn.ColumnName = MainObjectId;
            MainObjectIdColumn.DisplayName = MainObjectId;
            

            if (!ReportSource.IsUseSql)
            {
               
                results.Add(MainObjectIdColumn);
            }
                


            return results;
        }
       
        /// <summary>
        /// 是否是明细数据所需要的主键ObjectID
        /// </summary>
        /// <param name="reportWidget"></param>
        /// <param name="ReportSource"></param>
        /// <param name="ChildCodes"></param>
        /// <returns></returns>
        private bool DetailNeedMainObjectId(ReportWidget reportWidget, ReportSource ReportSource, out Dictionary<string, ColumnSummary> ChildCodes)
        {
            ChildCodes = new Dictionary<string, ColumnSummary>();
            Dictionary<string, ColumnSummary> Codes = new Dictionary<string, ColumnSummary>();
            if (ReportSource.ReportSourceAssociations == null || ReportSource.ReportSourceAssociations.Length == 0)
            {
                return false;
            }
            List<string> schemaCodes = new List<string>();
            List<string> propertyCodes = new List<string>();
            Dictionary<string, string> propertyNames = new Dictionary<string, string>();
            Dictionary<string, string> displayNameTable = new Dictionary<string, string>();
            //数据模型是否存在,子对象显示名称
            {
                List<string> tempSchemaCodes = new List<string>();
                List<string> childSchemaCodes = new List<string>();
                foreach (ReportSourceAssociation q in ReportSource.ReportSourceAssociations)
                {
                    if (q.IsSubSheet && q.MasterField.ToLowerInvariant() == ("I" + q.RootSchemaCode + "_ObjectID").ToLowerInvariant() && q.SubField.ToLowerInvariant() == ("I" + q.SchemaCode + "_ParentObjectID").ToLowerInvariant())
                    {
                        tempSchemaCodes.Add(q.SchemaCode);
                        tempSchemaCodes.Add(q.RootSchemaCode);
                        childSchemaCodes.Add(q.SchemaCode);
                    }
                }
                //  if (tempSchemaCodes.Count == 0 || (tempSchemaCodes.Count > 0 && !this.Engine.BizObjectManager.ExistSchemas(tempSchemaCodes.ToArray())))
                if (tempSchemaCodes.Count == 0)
                    return false;
                if (childSchemaCodes.Count > 0)
                {
                    displayNameTable = this.GetSchemasDisplayName(OThinker.Data.Utility.RemoveRedundant<string>(childSchemaCodes.ToArray()));
                }
            }
            foreach (ReportSourceAssociation q in ReportSource.ReportSourceAssociations)
            {
                if (q.IsSubSheet && q.MasterField.ToLowerInvariant() == ("I" + q.RootSchemaCode + "_ObjectID").ToLowerInvariant() && q.SubField.ToLowerInvariant() == ("I" + q.SchemaCode + "_ParentObjectID").ToLowerInvariant())
                {
                    schemaCodes.Add(q.RootSchemaCode);
                    propertyCodes.Add(q.SchemaCode);
                    //前面有schema的存在性判断，这里不需要再判断
                    string displayname = displayNameTable[q.SchemaCode];
                    Codes.Add(q.SchemaCode, new ColumnSummary() { DisplayName = displayname });
                }
            }
            if (schemaCodes.Count > 0)
            {
                propertyNames = this.GetPropertiesName(schemaCodes.ToArray(), propertyCodes.ToArray());
            }
            foreach (ReportSourceAssociation q in ReportSource.ReportSourceAssociations)
            {
                if (q.IsSubSheet && q.MasterField.ToLowerInvariant() == ("I" + q.RootSchemaCode + "_ObjectID").ToLowerInvariant() && q.SubField.ToLowerInvariant() == ("I" + q.SchemaCode + "_ParentObjectID").ToLowerInvariant())
                {
                    if (propertyNames.ContainsKey(q.RootSchemaCode + q.SchemaCode))
                    {
                        Codes.Add(q.SchemaCode, new ColumnSummary() { DisplayName = propertyNames[q.RootSchemaCode + q.SchemaCode] });
                    }
                }
                else
                {
                    return false;
                }
            }
            //树；
            foreach (ReportWidgetColumn q in reportWidget.Columns)
            {
                string originalcode = this.GetOriginalCode(q.ColumnCode);
                ColumnSummary t = new ColumnSummary();
                t.DisplayName = q.DisplayName;
                t.Code = q.ColumnCode;
                if (Codes.ContainsKey(originalcode))
                {
                    if (ChildCodes.ContainsKey(originalcode))
                    {
                        ChildCodes[originalcode].ChildeColumnSummary.Add(q.ColumnCode, t);
                    }
                    else
                    {
                        ChildCodes.Add(originalcode, Codes[originalcode]);
                        ChildCodes[originalcode].ChildeColumnSummary.Add(q.ColumnCode, t);
                    }
                }
                else
                {
                    ChildCodes.Add(q.ColumnCode, new ColumnSummary() { DisplayName = q.DisplayName, Code = q.ColumnCode });
                }
            }
            return true;
        }

        #endregion

        #region 根据数据模型设置相应的数据原型字段 （用于构建数据查询所需的条件或者列字段）
        /// <summary>
        /// 根据流程数据模型设置成为I_sdsd.objectid
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <returns></returns>
        protected string GetMainObjectId(string SchemaCode)
        {
            return "I_" + SchemaCode + ".ObjectId";
        }
        /// <summary>
        /// 根据流程数据模型设置成为I_sdsd_objectid
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <returns></returns>
        protected string GetMainObjectId_Name(string SchemaCode)
        {
            return "I_" + SchemaCode + "_ObjectId";
        }
        /// <summary>
        /// 根据流程数据模型设置成为I_sdsd_objectid
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <returns></returns>
        protected string GetSonObjectID_Name(string SonSchemaCode)
        {
            return "I_" + SonSchemaCode + "_ObjectID";
        }
        /// <summary>
        /// 根据流程数据模型设置成为I_sdsd_objectid
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <returns></returns>
        protected string GetSonObjectID_Objectid(string SonSchemaCode)
        {
            return "I_" + SonSchemaCode + ".ObjectID";
        }
        protected string GetOriginalCode(string Code)
        {
            Code = Code.Substring(1, Code.Length - 1);
            return Code.Split('_')[0];

        }
        #endregion
    }
    #region 报表基础数据类
    /// <summary>
    /// 汇总表数据对象
    /// </summary>
    public class CombineReport
    {
        public CombineReport()
        {
            this.ColumnHeaderNodes = new Dictionary<string, HeaderNode>();
            this.RowHeaderNodes = new Dictionary<string, HeaderNode>();
            this.OnlyHeader = new OnlyHeader();
        }
        /// <summary>
        /// 从上到下,最后一行是行表头
        /// </summary>
        public OnlyHeader OnlyHeader { get; set; }
        /// <summary>
        /// 列表头
        /// </summary>
        public Dictionary<string, HeaderNode> ColumnHeaderNodes { get; set; }
        /// <summary>
        /// 行表头，包含列数据
        /// </summary>
        public Dictionary<string, HeaderNode> RowHeaderNodes { get; set; }

    }
    /// <summary>
    /// 表头
    /// </summary>
    public class OnlyHeader
    {
        public OnlyHeader()
        {
            this.ColumnHeader = new Dictionary<string, OnlyHeaderNode>();
            this.RowHeader = new Dictionary<string, OnlyHeaderNode>();
        }
        public Dictionary<string, OnlyHeaderNode> ColumnHeader { get; set; }
        public Dictionary<string, OnlyHeaderNode> RowHeader { get; set; }
    }
    /// <summary>
    /// 只生成头节点
    /// </summary>
    public class OnlyHeaderNode
    {
        public OnlyHeaderNode()
        {
            this.ChildNodes = new List<OnlyHeaderNode>();
        }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public Function FunctionType { get; set; }
        public List<OnlyHeaderNode> ChildNodes { get; set; }
        public bool IsLeafNode { get { return ChildNodes == null || ChildNodes.Count == 0; } }
    }
    /// <summary>
    /// 用于生成表头树的头结点
    /// </summary>
    public class HeaderNode
    {
        public HeaderNode()
        {
            this.BoObjectIDs = new HashSet<string>();
            this.IsLeaf = false;
            this.ChildrenValueAndObjectID = new Dictionary<string, string>();
            this.Road = new Dictionary<string, string>();
        }
        /// <summary>
        /// 路径，用于表体的联动
        /// </summary>
        public Dictionary<string, string> Road { get; set; }
        /// <summary>
        /// ObjectId
        /// </summary>
        public string ObjectID { get; set; }
        /// <summary>
        /// 父结点Id
        /// </summary>
        public string ParentObjectID { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string ColumnCode { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public ColumnType ColumnType { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string HeaderValue { get; set; }
        /// <summary>
        /// 包含的Bo的Id
        /// </summary>
        public HashSet<string> BoObjectIDs { get; set; }
        /// <summary>
        /// 是叶子结点
        /// </summary>
        public bool IsLeaf { get; set; }
        /// <summary>
        /// 子结点的值及Id
        /// </summary>
        public Dictionary<string, string> ChildrenValueAndObjectID { get; set; }
    }
    /// <summary>
    ///简易看板的头节点
    /// </summary>
    public class SimpleHeaderNode
    {
        public SimpleHeaderNode()
        {
            ChildNode = new List<SimpleHeaderNode>();
        }
        public string Value { get; set; }
        public int RowSpan { get; set; }
        public int ColSpan { get; set; }
        public string ColumnCode { get; set; }
        public List<SimpleHeaderNode> ChildNode { get; set; }
    }
    /// <summary>
    /// 汇总
    /// </summary>
    public class myCollect
    {
        public myCollect()
        {
            this.Values = new List<double>();
        }
        public OThinker.Reporting.Function FunctionType { get; set; }
        public ColumnType ColumnType { get; set; }
        public string Format { get; set; }
        public List<double> Values { get; set; }
    }
    /// <summary>
    /// 用于构造汇总表行列的数据对象
    /// </summary>
    public class MyTd
    {
        public string Value { get; set; }
        public string Code { get; set; }
        public int ColSpan { get; set; }
        public int RowSpan { get; set; }
        public string ColumnCode { get; set; }
    }
    /// <summary>
    /// 图表返回给前段的数据
    /// </summary>
    public class ChartsDataResult
    {
        public ChartsDataResult()
        {
            this.Categories = new List<CategoriesForCharts>();
            this.Series = new List<SeriesForCharts>();
        }
        public List<CategoriesForCharts> Categories { get; set; }
        public List<SeriesForCharts> Series { get; set; }
        public string CategoryCode { get; set; }
        public string SerieCode { get; set; }
        public OThinker.Reporting.ColumnType CategoryType { get; set; }
        public OThinker.Reporting.ColumnType SerieType { get; set; }
        public string CategoryDisplayName { get; set; }
        public string SerieDisplayName { get; set; }
    }
    public class CategoriesForCharts
    {
        public string key { get; set; }
        public string value { get; set; }
    }
    /// <summary>
    /// 列Summary
    /// </summary>
    public class ColumnSummary
    {
        public ColumnSummary()
        {
            this.ChildeColumnSummary = new Dictionary<string, ColumnSummary>();
        }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, ColumnSummary> ChildeColumnSummary { get; set; }
    }
    /// <summary>
    /// 系列
    /// </summary>
    public class SeriesForCharts
    {
        public SeriesForCharts()
        {
            this.Data = new List<double>();
        }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Precision { get; set; }
        public List<double> Data { get; set; }
    }
    /// <summary>
    /// 排序
    /// </summary>
    public class SorkeyForCombine
    {
        public double CollectValue { get; set; }
        public int RowNumber { get; set; }
    }

    #endregion

    #region 设计器中的类
    public class FieldObject
    {
        public string Name { get; set; }

        public string DisplayName { get; set; }

        public ColumnType DataType { get; set; }
    }
    public class BizProperty
    {
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public string ParentCode { get; set; }
        public OThinker.Reporting.ColumnType DataType { get; set; }
    }

    public class Association
    {
        public string RootSchemaCode { get; set; }
        public string MasterField { get; set; }
        public string SchemaCode { get; set; }
        public string SubField { get; set; }
    }


    #endregion
}