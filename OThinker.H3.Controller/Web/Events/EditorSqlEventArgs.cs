using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// SQL 语句事件
    /// </summary>
    public class EditorSqlConditionEventArgs : EventArgs
    {
        public EditorSqlConditionEventArgs(string condition)
        {
            this.condition = condition;
        }

        private string condition = null;

        /// <summary>
        /// 数据源
        /// </summary>
        public string Condition
        {
            get
            {
                return this.condition;
            }
            set
            {
                this.condition = value;
            }
        }

    }
}
