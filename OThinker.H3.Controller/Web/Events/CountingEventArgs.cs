using System;
using System.Collections.Generic;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 页脚加载事件
    /// </summary>
    public class CountingEventArgs : EventArgs
    {
        public CountingEventArgs(string ColumnName, CountingType CountingType)
        {
            this._ColumnName = ColumnName;
            this._CountingType = CountingType;
        }

        private string _ColumnName = null;
        /// <summary>
        /// 列名称
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this._ColumnName;
            }
        }

        private CountingType _CountingType = CountingType.Sum;
        /// <summary>
        /// 统计方式
        /// </summary>
        public CountingType CountingType
        {
            get
            {
                return this._CountingType;
            }
            set
            {
                this._CountingType = value;
            }
        }

        private string _Value = null;
        /// <summary>
        /// 统计值
        /// </summary>
        public string Value
        {
            get
            {
                return this._Value;
            }
            set
            {
                this._Value = value;
            }
        }
    }
}
