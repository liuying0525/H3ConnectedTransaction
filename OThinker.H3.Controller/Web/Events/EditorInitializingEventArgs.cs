using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Generic;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 编辑器初始化事件
    /// </summary>
    public class EditorInitializingEventArgs : EventArgs
    {
        public EditorInitializingEventArgs(string ColumnName, Type ColumnType)
        {
            this._ColumnName = ColumnName;
            this._ColumnType = ColumnType;
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

        private Type _ColumnType;
        /// <summary>
        /// 列的类型
        /// </summary>
        public Type ColumnType
        {
            get
            {
                return this._ColumnType;
            }
        }

        private string _DisplayName = null;
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName
        {
            get
            {
                return this._DisplayName;
            }
            set
            {
                this._DisplayName = value;
            }
        }

        private bool _Visible = true;
        /// <summary>
        /// 该控件是否可见
        /// </summary>
        public bool Visible
        {
            get
            {
                return this._Visible;
            }
            set
            {
                this._Visible = value;
            }
        }

        private Control _Control = null;
        /// <summary>
        /// 编辑控件
        /// </summary>
        public Control Control
        {
            get
            {
                return this._Control;
            }
            set
            {
                this._Control = value;
            }
        }
    }        
}