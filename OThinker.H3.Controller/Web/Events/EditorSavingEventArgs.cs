using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Generic;

namespace OThinker.H3.Controllers
{
    public class EditorSavingEventArgs : EventArgs
    {
        public EditorSavingEventArgs(string ColumnName, Control Control)
        {
            this._ColumnName = ColumnName;
            this._Control = Control;
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

        private bool _Get = false;
        /// <summary>
        /// 是否获得
        /// </summary>
        public bool Get
        {
            get
            {
                return this._Get;
            }
            set
            {
                this._Get = value;
            }
        }

        private string _Value;
        /// <summary>
        /// 值的字符串形式
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

        private bool _Valid = true;
        /// <summary>
        /// 值是否合法
        /// </summary>
        public bool Valid
        {
            get
            {
                return this._Valid;
            }
            set
            {
                this._Valid = value;
            }
        }

        private string _ErrorMessage = null;
        /// <summary>
        /// 如果不合法则设置错误消息
        /// </summary>
        public string ErrorMessage
        {
            get
            {
                return this._ErrorMessage;
            }
            set
            {
                this._ErrorMessage = value;
            }
        }
    }
}
