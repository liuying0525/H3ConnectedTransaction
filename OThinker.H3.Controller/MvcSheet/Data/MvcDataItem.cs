using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Data;
using System.Web.UI.HtmlControls;
using OThinker.H3.Acl;
using System.Reflection;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using OThinker.H3.DataModel;
using System.IO;
using System.Text;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 传递到前端的数据项
    /// </summary>
    public class MvcDataItem
    {
        /// <summary>
        /// MVC数据项构造函数
        /// </summary>
        public MvcDataItem()
        {
        }

        /// <summary>
        /// MVC数据项构造函数
        /// </summary>
        /// <param name="DataItem">业务数据项</param>
        /// <param name="IsSheetLocked">表单是否被锁定，这个锁定通常是由其他用户签出表单造成的</param>
        public MvcDataItem(IInstanceDataItem DataItem, bool IsSheetLocked)
        {
            this.L = DataItem.LogicType;

            if (DataItem.Visible)
            {
                this.O += "V";
            }
            if (DataItem.Editable && !IsSheetLocked)
            {
                this.O += "E";
            }
            if (DataItem.Required && !IsSheetLocked)
            {
                this.O += "R";
            }
            if (DataItem.TrackVisible)
            {
                this.O += "T";
            }
            if (DataItem.MobileVisible)
            {
                this.O += "M";
            }
            if (DataItem.LogicType == DataLogicType.Long)
            {
                this.V = DataItem.Value + string.Empty;
            }
            else if (DataItem.LogicType != DataLogicType.BizObject
                && DataItem.LogicType != DataLogicType.BizObjectArray)
            {
                this.V = DataItem.Value;
            }
            this.N = DataItem.DisplayName;
        }

        /// <summary>
        /// 获取或设置数据项的逻辑类型
        /// </summary>
        public DataLogicType L
        {
            get;
            set;
        }

        private string _O = string.Empty;
        /// <summary>
        /// 获取或设置数据项的操作权限
        /// V:是否可见
        /// E:是否可编辑
        /// R:是否必填
        /// T:是否可见痕迹
        /// M:是否移动办公可见
        /// </summary>
        public string O
        {
            get
            {
                return this._O;
            }
            set
            {
                this._O = value;
            }
        }

        private object _v = null;
        /// <summary>
        /// 获取或设置数据项的值
        /// </summary>
        public object V
        {
            get
            {
                if (this.L == DataLogicType.DateTime)
                {
                    DateTime dt;
                    if (DateTime.TryParse(this._v + string.Empty, out  dt))
                    {
                        return dt.ToString("yyyy-MM-dd HH:mm:ss");
                    }
                    else
                    {
                        return this._v;
                    }
                }
                else
                {
                    return this._v;
                }
            }
            set
            {
                this._v = value;
            }
        }

        /// <summary>
        /// 获取或设置数据项的显示名称
        /// </summary>
        public string N
        {
            get;
            set;
        }
    }
}