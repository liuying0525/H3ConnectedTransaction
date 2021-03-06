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
    /// 节点集合
    /// </summary>
    public class MvcListItem
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Code"></param>
        /// <param name="Name"></param>
        public MvcListItem(string Code, string Name)
        {
            this.Code = Code;
            this.Name = Name;
        }

        public MvcListItem(string Code, string Name, string Url, int Size, string ContentType)
        {
            this.Code = Code;
            this.Name = Name;
            this.Url = Url;
            this.Size = Size;
            this.ContentType = ContentType;
        }

        /// <summary>
        /// 获取节点Code
        /// </summary>
        public string Code
        {
            get;
            set;
        }

        /// <summary>
        /// 获取节点显示名称
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// 地址
        /// </summary>
        public string Url
        {
            set;
            get;
        }

        /// <summary>
        /// 大小
        /// </summary>
        public int Size
        {
            set;
            get;
        }

        /// <summary>
        /// 附件类型
        /// </summary>
        public string ContentType
        {
            set;
            get;
        }
    }
}