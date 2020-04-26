using System;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using OThinker.H3.Acl;
using OThinker.H3.DataModel;
using OThinker.H3.Sheet;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    #region 流程模型的取数
    /// <summary>
    /// 流程模型的取数
    /// </summary>
    public class WorkflowHandler : AbstractPortalTreeHandler
    {
        //是否包含设计中的流程模板
        private const string ContainDraftStr = "ContainDraft";
        //是否业务对象选择模式
        private const string IsBizObjectModeStr = "IsBizObjectMode";
        /// <summary>
        /// 是否包含设计中的流程模板
        /// </summary>
        protected bool ContainDraft=false;
       
        /// <summary>
        /// 是否业务对象选择模式
        /// </summary>
        protected bool IsBizObjectMode = false;

         /// <summary>
        /// 是否业务对象选择模式
        /// </summary>
        protected bool IsSharedPacket = false;

        private ControllerBase _controller = null;
        protected override ControllerBase controller
        {
            get { return _controller; }
        }

        public WorkflowHandler(string ContainDraft,string IsBizObjectMode,string IsSharedPacket ,ControllerBase controller)
        {
            this.ContainDraft = string.Compare(ContainDraft, true.ToString(), true) == 0;
            this.IsBizObjectMode = string.Compare(IsBizObjectMode, true.ToString(), true) == 0;
            this.IsSharedPacket = string.Compare(IsSharedPacket, true.ToString(), true) == 0;
            this._controller = controller;
        }
        public override object CreatePortalTree(string FunctionID, string FunctionCode)
        {
            return ChildrenForProcessModel(FunctionID, FunctionCode);
        }

        #region 构造流程模型子数据
        /// <summary>
        /// 构造流程模型子数据
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        protected object ChildrenForProcessModel(string functionID, string functionCode)
        {
            FunctionNode[] functions = this.controller.Engine.FunctionAclManager.GetFunctionNodesByParentCode(functionCode);
            if (functions != null)
                return FunctionNodeToPortalTree(functionID, functions);
            else
                return GetWrokflowPackageChildren(functionID, functionCode);
        }

        private object GetWrokflowPackageChildren(string PackageID, string PackageCode)
        {//流程包的编码，等于数据模型编码
            List<FunctionNode> functionNodes = new List<FunctionNode>();

            //流程模板
            WorkflowClause[] workflowClauses = this.controller.Engine.WorkflowManager.GetClausesBySchemaCode(PackageCode);
            if (workflowClauses != null)
            {
                foreach (WorkflowClause workflowClause in workflowClauses)
                {
                    functionNodes.Add(new FunctionNode
                        (workflowClause.ObjectID,
                        workflowClause.WorkflowCode,
                        workflowClause.WorkflowName,
                        "fa icon-liuchengshujumoxing",
                        "",
                        FunctionNodeType.BizWorkflow,
                        PackageCode,
                        0));
                }
            }
            return FunctionNodeToPortalTree(PackageID, functionNodes.ToArray());
        }

        /// <summary>
        /// 流程模板类型选择时将FunctionNode转为PortalTree展示给客户端用
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        protected object FunctionNodeToPortalTree(string functionID, FunctionNode[] nodes)
        {
            List<PortalTreeNode> treeNodeList = new List<PortalTreeNode>();
            if (nodes == null) return treeNodeList;
            //过滤后排序
            if (IsBizObjectMode)
            {
                nodes = nodes.Where(e =>
                    e.NodeType == FunctionNodeType.BizWFFolder
                    || e.NodeType == FunctionNodeType.BizWorkflowPackage
                    || e.NodeType == FunctionNodeType.BizWorkflow
                    || e.NodeType == FunctionNodeType.BizFolder
                    || e.NodeType == FunctionNodeType.BizObject
                    ).OrderBy(p => p.SortKey).ToArray();
            }
            else
            {
                nodes = nodes.Where(e =>
                    e.NodeType == FunctionNodeType.BizWFFolder
                    || e.NodeType == FunctionNodeType.BizWorkflowPackage
                    || e.NodeType == FunctionNodeType.BizWorkflow
                    ).OrderBy(p => p.SortKey).ToArray();
            }
            //获取流程包节点集合
            #region 批量获取流程模板集合
            Dictionary<string, WorkflowTemplate.WorkflowClause[]> clauses = null;
            List<string> schemaCodes = new List<string>();
            WorkflowTemplate.WorkflowClause[] tmp = null;
            foreach (FunctionNode node in nodes)
            {
                if (node.NodeType == FunctionNodeType.BizWorkflowPackage || node.NodeType == FunctionNodeType.BizFolder)
                {
                    schemaCodes.Add(node.Code);
                }
            }
            if (schemaCodes.Count > 0)
            {
                clauses = this.controller.Engine.WorkflowManager.GetClausesBySchemaCodes(schemaCodes.ToArray());
            }
            #endregion

            foreach (FunctionNode node in nodes)
            {
                string showUrl = GetShowPageUrl(functionID, node);
                string toolBarCode = GetToolbarCode(node);
                if (node.NodeType == FunctionNodeType.BizWFFolder)
                    node.IconCss = "fa fa-folder-o";
                if (node.NodeType == FunctionNodeType.BizWorkflowPackage)
                    node.IconCss = "fa icon-liuchengshujumoxing";
                if (node.NodeType == FunctionNodeType.BizWorkflow)
                    node.IconCss = "fa icon-liuchengmoxing";
                if (node.NodeType == FunctionNodeType.BizFolder)
                    node.IconCss = "fa fa-folder-o";
                if (node.NodeType == FunctionNodeType.BizObject)
                    node.IconCss = "fa icon-zhushujushili";
                string IconUrl = node.IconCss;
                bool isLeaf = false;
                List<PortalTreeNode> children = null;
                //如果节点类型是BizWorkflowPackage,则判断其下流程模板个数，如果只有一个，则只显示流程包名称，反之，都显示
                if (node.NodeType == FunctionNodeType.BizWorkflowPackage)
                {
                    if (this.IsBizObjectMode)
                    {
                        isLeaf = true;
                    }
                    else
                    {
                        isLeaf = false;
                    }
                }
                else if (node.NodeType != FunctionNodeType.BizWorkflow && node.NodeType != FunctionNodeType.BizObject)
                {
                    isLeaf = false;
                }
                else
                {
                    isLeaf = true;
                }

                //如果不包括设计中的, 并且数据模型未发布
                if (!ContainDraft && node.NodeType == FunctionNodeType.BizWorkflowPackage && this.controller.Engine.BizObjectManager.GetPublishedSchema(node.Code) == null)
                {
                    continue;
                }

                if (IsSharedPacket)
                {
                    BizObjectSchema schema = this.controller.Engine.BizObjectManager.GetDraftSchema(node.Code);
                    if (schema != null)
                    {
                        if (!schema.IsShared)
                        {
                            continue;
                        }

                    }
                }

                //if (!IsBizObjectMode && isLeaf && tmp != null && tmp.Length == 1)
                //{
                //    treeNodeList.Add(new PortalTreeNode()
                //    {
                //        ObjectID = tmp[0].ObjectID,
                //        Code = tmp[0].WorkflowCode,
                //        Text = string.IsNullOrWhiteSpace(tmp[0].DisplayName) ? tmp[0].WorkflowCode : tmp[0].DisplayName,
                //        Icon = this.PortalRoot + "/" + IconUrl,
                //        ShowPageUrl = showUrl,
                //        LoadDataUrl = "",
                //        IsLeaf = true,
                //        ParentID = functionID,
                //        ToolBarCode = FunctionNodeType.BizWorkflow.ToString(),
                //        children = children
                //    });
                //}
                //else
                {
                    if (!ContainDraft && isLeaf && node.NodeType == FunctionNodeType.BizWorkflow)
                    {
                        //如果是未发布的流程，不会添加到列表中
                        int[] publishedVersions = this.controller.Engine.WorkflowManager.GetWorkflowVersions(node.Code);
                        if (publishedVersions == null || publishedVersions.Length == 0)
                        {
                            continue;
                        }
                    }

                    treeNodeList.Add(new PortalTreeNode()
                    {
                        ObjectID = node.ObjectID,
                        Code = node.Code,
                        Text = string.IsNullOrWhiteSpace(node.DisplayName) ? node.Code : node.DisplayName,
                        //Icon = this.PortalRoot + "/" + IconUrl,
                        Icon = IconUrl,
                        ShowPageUrl = showUrl,
                        LoadDataUrl = isLeaf ? "" : GetLoadWorkflowDataUrl(node.ObjectID, node.Code, node.NodeType, this.ContainDraft, this.IsBizObjectMode),
                        IsLeaf = isLeaf,
                        ParentID = functionID,
                        ToolBarCode = toolBarCode,
                        children = children
                    });
                }
            }
            return treeNodeList;
        }
        #endregion
    }
    #endregion
}