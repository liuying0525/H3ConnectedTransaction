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
    /// 编辑器加载事件
    /// </summary>
    public class EditorLoadingEventArgs : EventArgs
    {
        public EditorLoadingEventArgs(string ColumnName, Type ColumnType, object Value, Control Control, string ObjectID, GridViewRow Row)
        {
            this._ColumnName = ColumnName;
            this._ColumnType = ColumnType;
            this._Value = Value;
            this._Control = Control;
            this._ObjectID = ObjectID;
            this._CurrentRow = Row;
        }

        private GridViewRow _CurrentRow = null;
        /// <summary>
        /// 当前编辑的行
        /// </summary>
        public GridViewRow CurrentRow
        {
            get
            {
                return this._CurrentRow;
            }
        }

        private string _ObjectID = null;
        /// <summary>
        /// 主键字段的值
        /// </summary>
        public string ObjectID
        {
            get
            {
                return this._ObjectID;
            }
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
        /// 列类型
        /// </summary>
        public Type ColumnType
        {
            get
            {
                return this._ColumnType;
            }
        }

        private object _Value = null;
        /// <summary>
        /// 值
        /// </summary>
        public object Value
        {
            get
            {
                return this._Value;
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
        }

        private bool _Set = false;
        /// <summary>
        /// 如果设置了值，则把这个属性设置为true
        /// </summary>
        public bool Set
        {
            get
            {
                return this._Set;
            }
            set
            {
                this._Set = value;
            }
        }
    }
}
