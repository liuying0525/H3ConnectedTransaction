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
    /// MVC表单传递到后台数据
    /// </summary>
    public class MvcPostValue
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        public string Command
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置提交/驳回的节点名称
        /// </summary>
        public string DestActivityCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置提交时的角色身份
        /// </summary>
        public string PostValue
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置流程实例名称
        /// </summary>
        public string InstanceName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置流程实例ID
        /// </summary>
        public string InstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置业务对象ID
        /// </summary>
        public string BizObjectId
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前提交人所属的组织ID
        /// 获取或设置发起人所属OU（兼职时可选所在OU发起）
        /// </summary>
        public string ParentUnitID { get; set; }

        /// <summary>
        /// 当前表单的数据项集合值
        /// </summary>
        public MvcBizObject BizObject
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public PriorityType Priority
        {
            get;
            set;
        }

        /// <summary>
        /// HiddenFields
        /// </summary>
        public Dictionary<string, string> HiddenFields
        {
            get;
            set;
        }

        public string[] Participants
        {
            get;
            set;
        }
        /// <summary>
        /// WorkItemType
        /// </summary>
        public int WorkItemType
        {
            get;
            set;
        }
        /// <summary>
        /// WorkItemId
        /// </summary>
        public string WorkItemId
        {
            get;
            set;
        }
    }
}