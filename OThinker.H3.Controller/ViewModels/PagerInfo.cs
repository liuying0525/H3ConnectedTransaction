using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 页码信息类
    /// </summary>
    public class PagerInfo
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string sortname { get; set; }
        /// <summary>
        /// 排序方向
        /// </summary>
        public string sortorder { get; set; }
        /// <summary>
        /// 获取或设置当页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 获取或设置每页显示记录数
        /// </summary>
        public int PageSize { get; set; }

        #region JQuery dataTable 传递的参数 ---------------
        private int _iDisplayStart = -1;
        /// <summary>
        /// 获取或设置dataTable开始显示的记录行号
        /// </summary>
        public int iDisplayStart
        {
            get { return this._iDisplayStart; }
            set
            {
                this._iDisplayStart = value;
                this.PageIndex = this._iDisplayStart / this.iDisplayLength + 1;
            }
        }

        private int _iDisplayLength = -1;
        /// <summary>
        /// 获取或设置dataTable每页显示记录数
        /// </summary>
        public int iDisplayLength
        {
            get { return this._iDisplayLength; }
            set
            {
                this._iDisplayLength = value;
                this.PageSize = value;
                this.PageIndex = this.iDisplayStart / value + 1;
            }
        }

        /// <summary>
        /// 获取或设置dataTable.sEcho
        /// </summary>
        public int sEcho { get; set; }


        /// <summary>
        /// 获取或设置排序列索引,来自datatables参数
        /// </summary>
        public int iSortCol_0 { get; set; }
        /// <summary>
        /// 获取或设置排序模式,来自datatables参数
        /// </summary>
        public string sSortDir_0 { get; set; }
        #endregion

        /// <summary>
        /// 获取查询数据开始索引
        /// </summary>
        public int StartIndex
        {
            get
            {
                return (this.PageIndex - 1) * this.PageSize + 1;
            }
        }

        /// <summary>
        /// 获取查询数据结束索引
        /// </summary>
        public int EndIndex
        {
            get
            {
                return this.PageIndex * this.PageSize;
            }
        }

        // End Class
    }
}