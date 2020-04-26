using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// TreeNode 的摘要说明
    /// </summary>
    public class PortalTreeNode
    {
        public PortalTreeNode()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        /// <summary>
        /// 内码，可以为空
        /// </summary>
        public string ObjectID { set; get; }

        /// <summary>
        /// 节点编码（唯一）
        /// </summary>
        public string Code { set; get; }

        /// <summary>
        /// 节点名称
        /// </summary>
        public string Text { set; get; }

        /// <summary>
        /// tab页显示地址
        /// </summary>
        public string ShowPageUrl { set; get; }

        /// <summary>
        /// 异步取数时，加载数据地址
        /// </summary>
        public string LoadDataUrl { set; get; }

        /// <summary>
        /// 是否子节点，如果为false的话，是没有展开操作的
        /// </summary>
        public bool IsLeaf { set; get; }

        /// <summary>
        /// 工具栏标示
        /// </summary>
        public string ToolBarCode { set; get; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { set; get; }

        /// <summary>
        /// 父节点Code
        /// </summary>
        public string ParentID { set; get; }

        /// <summary>
        /// 孩子节点
        /// </summary>
        public object children { set; get; }

        /// <summary>
        /// 节点是否锁定
        /// </summary>
        public bool IsLock { get; set; }
        /// <summary>
        /// 节点锁定用户
        /// </summary>
        public bool IsLockedByCurrentUser { get; set; }

        public FunctionNodeType NodeType { get; set; }

        /// <summary>
        /// 是否需要部署
        /// </summary>
        public bool NeedDeploy { get; set; }

        /// <summary>
        /// 是否可以拖拽
        /// </summary>
        public bool Draggable
        {
            get
            {
                switch (this.NodeType)
                {
                    case FunctionNodeType.Organization:
                    case FunctionNodeType.BizWorkflowPackage:
                    case FunctionNodeType.ReportTemplateFolder:
                    case FunctionNodeType.BPAReportTemplate:
                    case FunctionNodeType.BizWFFolder:
                        return true;
                    default:
                        return false;
                }
            }
        }

        /// <summary>
        /// 获取或设置原有数据模型编码
        /// </summary>
        public string OwnSchemaCode { get; set; }

        /// <summary>
        /// 扩展对象，用于前端需要的额外数据，json格式；如组织机构需要获取 DepartmentName，那么就可以 {DepartmentName:"*****"}
        /// </summary>
        public object ExtendObject { get; set; }

        /// <summary>
        /// 创建子树数据，该方法放在节点的最后一个执行
        /// </summary>
        /// <param name="childrenData"></param>
        public void CreateChildren(object childrenData = null)
        {
            switch (this.Code)
            {
                case "":
                    break;
                default:
                    //如果没有子树数据，就是连接或取子数据的连接
                    //如果有子数据，该节点就不能为连接或异步取子数据
                    //this.Url = childrenData == null ? this.Url : "";
                    this.IsLeaf = childrenData == null;
                    this.children = childrenData;
                    break;
            }
        }

    }
}
