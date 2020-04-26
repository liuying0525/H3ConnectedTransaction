using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 表格数据
    /// </summary>
    public class GridViewModel<T> : ViewModelBase
        // where T : ViewModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="total"></param>
        /// <param name="rows"></param>
        public GridViewModel(int total, List<T> rows)
            : this(total, rows, 0)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="total"></param>
        /// <param name="rows"></param>
        /// <param name="sEcho"></param>
        public GridViewModel(int total, List<T> rows, int sEcho)
            : this(total, rows, sEcho, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="total"></param>
        /// <param name="rows"></param>
        /// <param name="sEcho"></param>
        /// <param name="extendProperty"></param>
        public GridViewModel(int total, List<T> rows, int sEcho, object extendProperty)
        {
            this.Total = total;
            this.Rows = rows;
            this.sEcho = sEcho;
            this.ExtendProperty = extendProperty;
        }

        /// <summary>
        /// 获取或设置JQuery dataTable 组件的sEcho，原值获取原值返回
        /// </summary>
        public int sEcho { get; set; }

        /// <summary>
        /// 获取或设置记录总数
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 获取或设置扩展属性
        /// </summary>
        public object ExtendProperty { get; set; }

        #region Jquery dataTable 输出参数 ----------------
        /// <summary>
        /// 获取或设置dataTable记录总数
        /// </summary>
        public int iTotalRecords
        {
            get { return this.Total; }
        }

        /// <summary>
        /// 获取或设置dataTable显示记录总数
        /// </summary>
        public int iTotalDisplayRecords
        {
            get { return this.Total; }
        }
        #endregion

        /// <summary>
        /// 获取或设置表格数据源
        /// </summary>
        public List<T> Rows { get; set; }
    }
}