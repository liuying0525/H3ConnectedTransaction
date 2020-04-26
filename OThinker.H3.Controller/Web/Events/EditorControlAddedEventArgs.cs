using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Collections;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.OleDb;

namespace OThinker.H3.Controllers
{
    public class EditorControlAddedEventArgs : EventArgs
    {
        public EditorControlAddedEventArgs(string ColumnName, WebControl Control)
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

        private WebControl _Control = null;
        /// <summary>
        /// 编辑控件
        /// </summary>
        public WebControl Control
        {
            get
            {
                return this._Control;
            }
        }
    }
}
