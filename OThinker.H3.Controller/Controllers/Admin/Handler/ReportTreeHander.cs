using OThinker.H3.Acl;
using OThinker.H3.Analytics.Reporting;
using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class ReportTreeHander : AbstractPortalTreeHandler
    {
        /// <summary>
        /// 控制器
        /// </summary>
        private ControllerBase _controller;
        protected override ControllerBase controller
        {
            get { return _controller; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controller">控制器</param>
        public ReportTreeHander(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        /// <summary>
        /// 获取树
        /// </summary>
        /// <param name="functionId">功能ID</param>
        /// <param name="functionCode">功能编码</param>
        /// <returns>树结构信息</returns>
        public override object CreatePortalTree(string functionId, string functionCode)
        {
            if (functionCode == FunctionNode.BPA_ReportSource_Code)
            {
                return CreateReportSourceNodes(functionId, functionCode);
            }
            else
            {
                return CreateReportNodes(functionId, functionCode);
            }
        }

        /// <summary>
        /// 获取报表模板目录树信息
        /// </summary>
        /// <param name="functionId">功能ID</param>
        /// <param name="functionCode">功能编码</param>
        /// <returns>报表模板目录树</returns>
        private List<PortalTreeNode> CreateReportNodes(string functionId, string functionCode)
        {
            List<PortalTreeNode> TreeNodes = new List<PortalTreeNode>();
            FunctionNode[] Folders = _controller.Engine.FunctionAclManager.GetFunctionNodesByParentCode(functionCode);
            if (Folders != null)
            {
                foreach (FunctionNode Folder in Folders)
                {
                    Folder.IconCss = "fa fa-folder-o";
                }
                TreeNodes = ConvertToPortalTree(functionId, Folders);
            }

            ReportTemplate[] ReportTemplates = _controller.Engine.Analyzer.GetReportTemplateByFolderCode(functionCode);
            if (ReportTemplates != null)
            {
                foreach (ReportTemplate ReportTemplate in ReportTemplates)
                {
                    string displayName = "MenuTreeCategory." + ReportTemplate.Code;
                    TreeNodes.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = ReportTemplate.Code,
                        Text = displayName,
                        //Icon = GetIconUrl(ReportTemplate),
                        Icon = GetIconCss(ReportTemplate),
                        IsLeaf = true,
                        ParentID = functionId,
                        NodeType = FunctionNodeType.BPAReportTemplate,
                        ShowPageUrl = GetReportUrl(functionId, ReportTemplate),
                        ExtendObject = new { Text = ReportTemplate.DisplayName }
                    });
                }
            }
            return TreeNodes;
        }

        /// <summary>
        /// 获取报表数据源树信息
        /// </summary>
        /// <param name="functionId">功能ID</param>
        /// <param name="functionCode">功能编码</param>
        /// <returns>报表数据源树</returns>
        private List<PortalTreeNode> CreateReportSourceNodes(string functionId, string functionCode)
        {
            ReportSource[] ReportSources = _controller.Engine.Analyzer.GetReportSources();
            List<PortalTreeNode> TreeNodes = new List<PortalTreeNode>();
            if (ReportSources != null)
            {
                foreach (ReportSource ReportSource in ReportSources)
                {
                    string displayName = "MenuTreeCategory." + ReportSource.Code;
                    TreeNodes.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = ReportSource.Code,
                        Text = displayName,
                        ShowPageUrl = "../Admin/BPA/EditReportSource.html&ReportSourceCode=" + ReportSource.Code,
                        //Icon = this.PortalRoot + "/WFRes/_Content/themes/ligerUI/icons/H3_icons/" + FunctionNodeType.BPAReportSource.ToString() + ".png",
                        Icon = "fa icon-baobiaoshujuyuan",
                        IsLeaf = true,
                        NodeType = FunctionNodeType.BPAReportSource,
                        ParentID = functionId,
                        ExtendObject =new { Text = ReportSource.DisplayName }
                    });
                }
            }
            return TreeNodes;
        }

        /// <summary>
        /// 获取报表数据源连接地址
        /// </summary>
        /// <param name="FunctionID">功能ID</param>
        /// <param name="ReportTemplate">报表数据模板</param>
        /// <returns>链接地址</returns>
        private string GetReportUrl(string FunctionID, ReportTemplate ReportTemplate)
        {
            string url = "..";
            if (ReportTemplate.ReportType == ReportType.Cross)
                url += ConstantString.PagePath_ReportTemplateCrossEditUrl;
            else
                url += ConstantString.PagePath_ReportTemplateSummaryEditUrl;
            return url += "&" + ConstantString.Param_ReportSourceCode + "=" + HttpUtility.UrlEncode(ReportTemplate.Code) + "&ParentID=" + FunctionID;
        }

        /// <summary>
        /// 获取报表模板样式
        /// </summary>
        /// <param name="ReportTemplate">报表模板</param>
        /// <returns>css样式</returns>
        private string GetIconCss(ReportTemplate ReportTemplate)
        {
            if (ReportTemplate.ReportType == ReportType.Cross)
            {
                return "fa icon-bangding";
            }
            else
            {
                return "fa icon-charubiaoge";
            }
        }
    }
}
