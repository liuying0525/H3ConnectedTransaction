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
    /// MVC返回前端数据
    /// </summary>
    public class MvcResult
    {
        /// <summary>
        /// 获取或设置返回的流程实例ID
        /// </summary>
        public string InstanceId { get; set; }

        /// <summary>
        /// 获取或设置工作任务的ID
        /// </summary>
        public string WorkItemId { get; set; }

        /// <summary>
        /// 获取打开工作任务的URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 获取或设置处理后是否关闭页面
        /// </summary>
        public bool ClosePage { get; set; }

        /// <summary>
        /// 获取或设置操作是否成功
        /// </summary>
        public bool Successful { get; set; }

        /// <summary>
        /// 获取或设置是否是保存操作
        /// </summary>
        public bool IsSaveAction { get; set; }

        /// <summary>
        /// 获取或设置是否是驳回操作
        /// </summary>
        public bool IsRejectAction { get; set; }

        /// <summary>
        /// 获取或设置是否需要刷新页面
        /// </summary>
        public bool Refresh { get; set; }

        /// <summary>
        /// 获取或设置返回到页面的消息
        /// </summary>
        public string Message { get; set; }

        private List<string> _Errors = new List<string>();
        /// <summary>
        /// 获取或设置错误消息的集合
        /// </summary>
        public List<string> Errors
        {
            get
            {
                return _Errors;
            }
        }

        /// <summary>
        /// 移动端返回的URL
        /// </summary>
        public string MobileReturnUrl
        {
            get;
            set;
        }
    }
}