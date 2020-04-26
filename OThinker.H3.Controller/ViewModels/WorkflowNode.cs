using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// WorkflowNode 的摘要说明
    /// </summary>
    public class WorkflowNode : ViewModelBase
    {
        public WorkflowNode()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 是否流程节点（0：否，1 是）
        /// </summary>
        public bool IsLeaf;

        /// <summary>
        /// 节点Id
        /// </summary>
        public string ObjectID;

        /// <summary>
        /// 节点编码
        /// </summary>
        public string Code;

        /// <summary>
        /// 节点显示名称
        /// </summary>
        public string DisplayName;

        /// <summary>
        /// 流程发布时间
        /// </summary>
        public string PublishedTime;
        //public DateTime PublishedTime;

        /// <summary>
        /// 流程版本号
        /// </summary>
        public int Version;

        /// <summary>
        /// 是否常用流程（0 否，1 是）
        /// </summary>
        public int Frequent;

        /// <summary>
        /// 图标文件名称
        /// </summary>
        public string IconFileName;

        /// <summary>
        /// 图标字体
        /// </summary>
        public string Icon;

        /// <summary>
        /// 子节点
        /// </summary>
        public List<WorkflowNode> children;
    }
}
