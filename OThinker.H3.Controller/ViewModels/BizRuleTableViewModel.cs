using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizRuleTableViewModel:ViewModelBase
    {
        public BizRuleTableViewModel() 
        {
            HorizontalCellCount = 1;
            VerticalCellCount = 1;
            RowDepth = 0;
            ColumnDepth = 0;
            IsView = false;
            Rows = new List<MatrixColumnOrRowViewModel>();
            Columns = new List<MatrixColumnOrRowViewModel>();
            Cells = new List<List<MatrixCellViewModel>>();
        }
        /// <summary>
        /// 业务规则编码
        /// </summary>
        public string RuleCode { get; set; }
        /// <summary>
        /// 决策表编码
        /// </summary>
        public string MatrixCode { get; set; }

        /// <summary>
        /// 决策表列占用的单元格数量（决策表列的叶子节点的个数）
        /// </summary>
        public int HorizontalCellCount { get; set; }

        /// <summary>
        /// 决策表行占用的单元格数量（决策表行的叶子节点的个数）
        /// </summary>
        public int VerticalCellCount { get; set; }

        /// <summary>
        /// 决策表行的深度（行的层级,如果行都没有子节点 是1级 ，如果行都只有一个子节点且所有子节点都没有子节点，是2级，以此类推
        /// </summary>
        public int RowDepth { get; set; }
        /// <summary>
        /// 决策表列的深度
        /// </summary>
        public int ColumnDepth { get; set; }

        /// <summary>
        /// 总行数
        /// </summary>
        public int ToltalRows { get { return VerticalCellCount + ColumnDepth; } }

        /// <summary>
        /// 总列数
        /// </summary>
        public int ToltalColumns { get { return HorizontalCellCount + RowDepth; } }

        /// <summary>
        /// 决策表类型（Script | SelectiveArray | SortedArray）
        /// </summary>
        public string MatrixType { get;set; }

        /// <summary>
        /// 是否为查看模式，默认为False
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// 决策表行信息
        /// </summary>
        public List<MatrixColumnOrRowViewModel> Rows { get; set; }

        /// <summary>
        /// 决策表列信息
        /// </summary>
        public List<MatrixColumnOrRowViewModel> Columns { get; set; }

        /// <summary>
        /// 决策表单元格信息
        /// </summary>
        public List<List<MatrixCellViewModel>> Cells { get; set; }

    }

    /// <summary>
    /// 决策表行或列
    /// </summary>
    public class MatrixColumnOrRowViewModel
    {
        public MatrixColumnOrRowViewModel()
        {
            CellCount = 0;
            HorizontalAlign = "center";
            VerticalAlign = "middle";
            Children = new List<MatrixColumnOrRowViewModel>();
        }

        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 生效条件/值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 占用单元格的数量，rowspan columnspan
        /// </summary>
        public int CellCount { get; set; }
        public string HorizontalAlign { get; set; }
        public string VerticalAlign { get; set; }

        public List<MatrixColumnOrRowViewModel> Children { get; set; }

    }

    /// <summary>
    /// 决策表单元格
    /// </summary>
    public class MatrixCellViewModel
    {
        /// <summary>
        /// 单元格值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 行索引
        /// </summary>
        public int RowIndex { get; set; }

        /// <summary>
        /// 列索引
        /// </summary>
        public int ColumnIndex { get; set; }

        /// <summary>
        /// 单元格宽度
        /// </summary>
        public int CellWidth { get; set; }
    }

    /// <summary>
    /// 导出Excel使用
    /// </summary>
    public class MatrixCellExcelViewModel
    {
        /// <summary>
        /// 单元格值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 行跨度
        /// </summary>
        public int RowSpan { get; set; }

        /// <summary>
        /// 列跨度
        /// </summary>
        public int ColSpan { get; set; }
    }
}
