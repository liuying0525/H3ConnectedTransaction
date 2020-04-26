using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.SS.Util;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.BizRule
{
    public class BizRuleTableController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return Acl.FunctionNode.Category_BizRule_Code; }
        }

        /// <summary>
        /// 获取业务规则表内容
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GeBizRuleTalbe(string ruleCode, string matrixCode,bool isView)
        {
            return ExecuteFunctionRun(() => {

                if (string.IsNullOrEmpty(ruleCode) || string.IsNullOrEmpty(matrixCode)) {
                    return null;
                }


                #region 获取对应的业务规则和决策表
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = null;
                OThinker.H3.BizBus.BizRule.BizRuleDecisionMatrix Matrix = null;

                Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                if (Rule == null) { return null; }

                Matrix = Rule.GetDecisionMatrix(matrixCode);
                if (Matrix == null) { return null; }
                #endregion


                BizRuleTableViewModel model =new BizRuleTableViewModel();

                #region 基本信息
                model.RuleCode = ruleCode;
                model.MatrixCode = matrixCode;
                model.IsView = isView;
                model.MatrixType = Matrix.MatrixType.ToString();
                model.HorizontalCellCount= Matrix.HorizontalCellCount;
                model.VerticalCellCount = Matrix.VerticalCellCount;
                model.RowDepth = Matrix.RowDepth;
                model.ColumnDepth = Matrix.ColumnDepth;
                #endregion

                #region  列集合

                model.Columns = GetMatrixData(Matrix.Columns);

                #endregion

                #region  行集合

                model.Rows = GetMatrixData(Matrix.Rows);

                #endregion

                #region  单元格集合

                int rowLength = Matrix.Cells.Length;

                List<List<MatrixCellViewModel>> listCellcells = new List<List<MatrixCellViewModel>>();

                for (int i = 0; i < rowLength; i++)
                {
                    List<MatrixCellViewModel> listCells = new List<MatrixCellViewModel>();
                    int colLength = Matrix.Cells[i].Length;
                    for (int j = 0; j < colLength; j++)
                    {
                        MatrixCellViewModel cell = new MatrixCellViewModel();
                        cell.RowIndex = i;
                        cell.ColumnIndex = j;
                        cell.Value = Matrix.Cells[i][j].CellValue;

                        listCells.Add(cell);
                    }
                    listCellcells.Add(listCells);
                }
                model.Cells = listCellcells;

                #endregion

                return Json(model,JsonRequestBehavior.AllowGet);
            });
        }

          /// <summary>
        /// 保存业务规则表内容
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SaveBizRuleTable(string ruleCode, string matrixCode, string cellString)
        {
            return ExecuteFunctionRun(() => {

                List<List<MatrixCellViewModel>> cells = JsonConvert.DeserializeObject<List<List<MatrixCellViewModel>>>(cellString);

                ActionResult result = new ActionResult(false, "BizRule.DeletedOrNotExists");

                #region 获取对应的业务规则和决策表
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = null;
                OThinker.H3.BizBus.BizRule.BizRuleDecisionMatrix Matrix = null;

                Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                if (Rule == null) { return Json(result); }

                Matrix = Rule.GetDecisionMatrix(matrixCode);
                if (Matrix == null) { return Json(result); }
                #endregion

                if (Matrix.Cells != null)
                {
                    try {
                        int rowLength = Matrix.Cells.Length;
                        for (int i = 0; i < rowLength; i++)
                        {
                            int colLength = Matrix.Cells[i].Length;
                            for (int j = 0; j < colLength; j++)
                            {
                                Matrix.Cells[i][j].CellValue = cells[i][j].Value;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        result.Message = "msgGlobalString.SaveFailed";
                        result.Extend = ex.Message;
                    }
                }
                this.Engine.BizBus.UpdateBizRule(Rule);

                result.Success = true;
                result.Message = "msgGlobalString.SaveSucced";
                return Json(result);
            });

        }

        /// <summary>
        /// 导出Excel
        /// </summary>
        /// <param name="ruleCode"></param>
        /// <param name="matrixCode"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult ExportToExcel(string ruleCode,string matrixCode,string tableDataString)
        {
            return ExecuteFunctionRun(() =>
            {
                tableDataString = Server.HtmlDecode(tableDataString.Replace('\"', '"'));

                if (string.IsNullOrEmpty(ruleCode) || string.IsNullOrEmpty(matrixCode))
                {
                    return null;
                }

                #region 获取对应的业务规则和决策表
                OThinker.H3.BizBus.BizRule.BizRuleTable Rule = null;
                OThinker.H3.BizBus.BizRule.BizRuleDecisionMatrix Matrix = null;

                Rule = this.Engine.BizBus.GetBizRule(ruleCode);
                if (Rule == null) { return null; }

                Matrix = Rule.GetDecisionMatrix(matrixCode);
                if (Matrix == null) { return null; }
                #endregion

                List<List<MatrixCellExcelViewModel>> TableData = JsonConvert.DeserializeObject<List<List<MatrixCellExcelViewModel>>>(tableDataString);

                IWorkbook workbook = new HSSFWorkbook();
                ISheet sheet = workbook.CreateSheet();

                for (int rowIndex = 0; rowIndex < TableData.Count; rowIndex++)
                {
                    List<MatrixCellExcelViewModel> tableRow = TableData[rowIndex];
                    IRow row = sheet.GetRow(rowIndex);
                    if (row == null)
                    {
                        row = sheet.CreateRow(rowIndex);
                    }
                    for (int colIndex = 0; colIndex < tableRow.Count; colIndex++)
                    {
                        MatrixCellExcelViewModel tableCell = tableRow[colIndex];

                        int colIndex2 = 0;
                        while (row.GetCell(colIndex2) != null)
                        {
                            colIndex2++;
                        }

                        if (tableCell.ColSpan > 1)
                        {
                            for (int i = colIndex2; i < colIndex2 + tableCell.ColSpan; i++)
                            {
                                row.CreateCell(i);
                            }
                            setCellRangeAddress(sheet, rowIndex, rowIndex, colIndex2, colIndex2 + tableCell.ColSpan - 1);
                        }
                        else
                        {
                            row.CreateCell(colIndex2);
                        }

                        if (tableCell.RowSpan > 1)
                        {
                            for (int i = rowIndex + 1; i < rowIndex + tableCell.RowSpan; i++)
                            {
                                IRow row2 = sheet.GetRow(i);
                                if (row2 == null)
                                {
                                    row2 = sheet.CreateRow(i);
                                }
                                if (tableCell.ColSpan > 1)
                                {
                                    for (int j = colIndex2; j < colIndex2 + tableCell.ColSpan; j++)
                                    {
                                        row2.CreateCell(j);
                                    }
                                }
                                else
                                {
                                    row2.CreateCell(colIndex2);
                                }
                            }
                            setCellRangeAddress(sheet, rowIndex, rowIndex + tableCell.RowSpan - 1, colIndex2, colIndex2);
                        }

                        //给单元格赋值
                        row.GetCell(colIndex2).SetCellValue(tableCell.Value);
                        //给单元格添加样式
                        ICellStyle cellStyle = workbook.CreateCellStyle();
                        cellStyle.Alignment = HorizontalAlignment.Center;
                        cellStyle.VerticalAlignment = VerticalAlignment.Center;
                        row.GetCell(colIndex2).CellStyle = cellStyle;
                    }
                }

                string virtualPath = this.PortalRoot+"/TempImages/" + Matrix.DisplayName + DateTime.Now.ToString("yyyy-MM-dd") + ".xls";
                string FilePath = Server.MapPath(this.PortalRoot + "/TempImages/" + Matrix.DisplayName + DateTime.Now.ToString("yyyy-MM-dd") + ".xls");
                
                using ( FileStream  ms = System.IO.File.Create(FilePath))
                {
                    workbook.Write(ms);
                }
                string FileUrl =  virtualPath;

                ActionResult result = new ActionResult();
                result.Success = true;
                result.Message = FileUrl;

                return Json(result); 
            });
        }

        private void setCellRangeAddress(ISheet sheet, int rowStart, int rowEnd, int colStart, int colEnd)
        {
            CellRangeAddress cellRangeAddress = new CellRangeAddress(rowStart, rowEnd, colStart, colEnd);
            sheet.AddMergedRegion(cellRangeAddress);
        }

        /// <summary>
        /// 获取行或列信息
        /// </summary>
        /// <param name="headers"></param>
        /// <returns></returns>
        private List<MatrixColumnOrRowViewModel> GetMatrixData(OThinker.H3.BizBus.BizRule.Header[] headers)
        {
            List<MatrixColumnOrRowViewModel> dataList = new List<MatrixColumnOrRowViewModel>();

            for (int i = 0, j = headers.Length; i < j; i++)
            {
                OThinker.H3.BizBus.BizRule.Header header = headers[i];

                MatrixColumnOrRowViewModel model = new MatrixColumnOrRowViewModel();
                model.DisplayName = header.DisplayName;
                model.Value = header.Value;
                model.CellCount = header.CellCount;

                if (header.Children != null && header.Children.Length > 0)
                {
                    List<MatrixColumnOrRowViewModel> listChildren = GetMatrixData(header.Children);
                    if (model.Children != null)
                    {
                        model.Children.AddRange(listChildren);
                    }
                    else
                    {
                        model.Children = new List<MatrixColumnOrRowViewModel>();
                        model.Children.AddRange(listChildren);
                    }
                }
                dataList.Add(model);
            }
            return dataList;
        }
    }
}
