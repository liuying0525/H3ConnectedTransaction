using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// SQL 语句事件
    /// </summary>
    public class EditorSourceLoadingEventArgs : EventArgs
    {
        public EditorSourceLoadingEventArgs(DataTable dt)
        {
            this.dataTableSource = dt;
        }

        private DataTable dataTableSource = null;
        /// <summary>
        /// 数据源
        /// </summary>
        public DataTable DataTableSource
        {
            get
            {
                return this.dataTableSource;
            }
            set
            {
                this.dataTableSource = value;
            }
        }

    }
}
