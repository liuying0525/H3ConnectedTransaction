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
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    /// <summary>
    /// 树处理类 基抽象类
    /// </summary>
    public abstract class AbstractPortalTreeHandler
    {
        #region 常量
        private const string FunctionTypeStr = "FunctionType";
        private const string FunctionIDStr = "FunctionID";
        private const string FunctionCodeStr = "FunctionCode";
        private const string OwnSchemaCodeStr = "OwnSchemaCode";
        private const string ContainDraftStr = "ContainDraft";
        private const string IsBizObjectModeStr = "IsBizObjectMode";

        public const string Param_ID = "ID";
        public const string Param_Parent = "Parent";
        public const string Param_Mode = "Mode";
        protected abstract ControllerBase controller { get; }
        #endregion

        public virtual object CreatePortalTree(string functionId, string functionCode) { return null; }

        public virtual object CreatePortalTree(string functionId, string functionCode,string OwnSchemaCode) { return null; }

        public virtual object CreatePortalTree(string Code)
        {
            return null;
        }

        #region 将FunctionNode转为PortalTree展示给客户端用
        /// <summary>
        /// 将FunctionNode转为PortalTree展示给客户端用
        /// </summary>
        /// <param name="functionID"></param>
        /// <param name="nodes"></param>
        /// <returns></returns>
        protected List<PortalTreeNode> ConvertToPortalTree(string functionID, FunctionNode[] nodes)
        {
            List<PortalTreeNode> treeNodeList = new List<PortalTreeNode>();
            if (nodes == null) return treeNodeList;
            //排序
            //nodes = nodes.OrderBy(p => p.NodeType).ThenBy(p => p.SortKey).ToArray();
            nodes = nodes.OrderBy(p => p.SortKey).ToArray();
            foreach (FunctionNode node in nodes)
            {
                string showUrl = GetShowPageUrl(functionID, node);
                string toolBarCode = GetToolbarCode(node);
                //string IconUrl = GetIconUrl(node);
                string IconUrl = node.IconCss;
                List<PortalTreeNode> children = GetChildren(node);
                switch (node.Code)
                {
                    case FunctionNode.Category_ServiceMoniter_Code://后台根节点，默认展开第一级
                        PortalTreeNode adminRooNode = new PortalTreeNode()
                        {
                            ObjectID = node.ObjectID,
                            Code = node.Code,
                            Text = getGlobalDisplayName(node),
                            //Icon = this.PortalRoot + "/" + node.IconUrl,
                            Icon = IconUrl,
                            ShowPageUrl = node.Url,
                            IsLeaf = true,
                            ParentID = functionID,
                            NodeType = node.NodeType
                        };
                        adminRooNode.CreateChildren(this.CreatePortalTree(node.ObjectID, node.Code));
                        treeNodeList.Add(adminRooNode);
                        break;
                    case FunctionNode.Organization_Data_Code://组织机构
                        treeNodeList.Add(BuildCompanyNode(node, toolBarCode));
                        break;
                    default:
                        #region 纠正叶子节点
                        //有子菜单、业务规则、流程模型、业务服务，都不是叶子节点
                        FunctionNode[] ChildrenNodes = controller.UserValidator.GetFunctionsByParentCode(node.Code);
                        bool isLeaf = ChildrenNodes == null || ChildrenNodes.Length == 0;
                        if (isLeaf)
                        {
                            isLeaf = children == null;
                            if (isLeaf)
                            {
                                isLeaf = string.IsNullOrWhiteSpace(toolBarCode);//如果有工具栏，基本上都是有子菜单的
                                if (isLeaf)
                                {
                                    isLeaf = !(node.Code == FunctionNode.BizRule_ListRuleTable_Code
                                         || node.Code == FunctionNode.Category_ProcessModel_Code
                                         || node.Code == FunctionNode.BizBus_BizService_Code
                                         || node.Code == FunctionNode.ProcessModel_BizMasterData_Code);
                                }
                            }
                        }
                        #endregion

                        treeNodeList.Add(new PortalTreeNode()
                        {
                            ObjectID = node.ObjectID,
                            Code = node.Code,
                            Text = getGlobalDisplayName(node),
                            //Icon = ((IconUrl != null && IconUrl.StartsWith("data:image")) ? IconUrl : (this.PortalRoot + "/" + IconUrl)),
                            Icon = node.IconCss,
                            ShowPageUrl = showUrl,
                            LoadDataUrl = isLeaf ? string.Empty : GetLoadDataUrl(node.ObjectID, node.Code, node.NodeType, node.OwnSchemaCode),
                            IsLeaf = isLeaf,
                            ParentID = functionID,
                            ToolBarCode = toolBarCode,
                            children = children,
                            IsLock = (node.IsLocked == true ? true : false),
                            IsLockedByCurrentUser = (node.IsLocked == true && node.LockedBy == controller.UserValidator.UserID),
                            NodeType = node.NodeType,
                            OwnSchemaCode=node.OwnSchemaCode
                        });
                        break;
                }
            }
            return treeNodeList;
        }

        private string getGlobalDisplayName(FunctionNode node)
        {
            string displayName = "MenuTreeCategory." + node.Code;
            //判断如果是用户建立的文件夹,业务服务，文件名称直接返回
            if (node.Code == FunctionNode.ProcessModel_BizMasterData_Code ||
                node.Code == FunctionNode.App_Workflow || node.Code == FunctionNode.App_Report) { return displayName; }
            if (node.NodeType == FunctionNodeType.BizWFFolder || node.NodeType == FunctionNodeType.BizFolder
                || node.NodeType == FunctionNodeType.ServiceFolder || node.NodeType == FunctionNodeType.RuleFolder
                || node.NodeType == FunctionNodeType.ReportTemplateFolder || node.NodeType == FunctionNodeType.BizService || node.NodeType == FunctionNodeType.BizRule
                || node.NodeType == FunctionNodeType.AppNavigation||node.NodeType==FunctionNodeType.ReportFolderPage)
            {
                if (node.Code != FunctionNode.BizBus_BizService_Code
                           && node.Code != FunctionNode.BizRule_ListRuleTable_Code
                           && node.Code != FunctionNode.BPA_ReportTemplate_Code)
                {
                    displayName = "_" + node.DisplayName;
                }
            }


            return displayName;
        }
        #endregion

        #region 获取显示数据地址
        /// <summary>
        /// 获取显示数据地址
        /// </summary>
        /// <param name="Node"></param>
        protected string GetShowPageUrl(string functionID, FunctionNode Node)
        {

            string url = "";
            switch (Node.NodeType)
            {
                case FunctionNodeType.BizObject:
                    url = this.controller.PortalRoot + ConstantString.PagePath_EditBizObjectSchema + "&ParentID=" + functionID + "&" + ConstantString.Param_SchemaCode + "=" + HttpUtility.UrlEncode(Node.Code);
                    break;
                case FunctionNodeType.BizService:
                    url = this.controller.PortalRoot + ConstantString.PagePath_EditBizService + "&ParentID=" + functionID + "&" + ConstantString.Param_ServiceCode + "=" + HttpUtility.UrlEncode(Node.Code);
                    break;
                case FunctionNodeType.BizSheet:
                    //这里得读取表单信息，根据表单判断是默认表单还是自定义表单
                    if (Node.IsSystem)
                    {
                        //if (WorkSheet.AppConfig.SheetType == WorkSheet.SheetModeType.ASPX)
                        //{
                        //    url = ConstantString.PagePath_SheetDesigner + "?" + ConstantString.Param_SheetCode + "=" + HttpUtility.UrlEncode(Node.Code) + "&ParentID=" + functionID;
                        //}
                        //else
                        {
                            url = this.controller.PortalRoot + ConstantString.PagePath_MvcDesigner + "&" + ConstantString.Param_SheetCode + "=" + HttpUtility.UrlEncode(Node.Code) + "&ParentID=" + functionID;
                        }
                    }
                    else
                        url = this.controller.PortalRoot + ConstantString.PagePath_WorkSheetEdit + "&" + ConstantString.Param_ID + "=" + HttpUtility.UrlEncode(Node.ObjectID) + "&ParentID=" + functionID;
                    break;
                case FunctionNodeType.BizWorkflow:
                    url = this.controller.PortalRoot + ConstantString.PagePath_WorkflowDesigner + "&" + ConstantString.Param_WorkflowCode + "=" + HttpUtility.UrlEncode(Node.Code) + "&ParentID=" + functionID;
                    break;
                case FunctionNodeType.BizWFFolder:
                    //qiancheng
                case FunctionNodeType.ReportFolder:
                case FunctionNodeType.BizFolder:
                case FunctionNodeType.ServiceFolder:
                case FunctionNodeType.RuleFolder:
                case FunctionNodeType.ReportTemplateFolder:
                    if (Node.Code != FunctionNode.BizBus_BizService_Code
                        && Node.Code != FunctionNode.BizRule_ListRuleTable_Code
                        && Node.Code != FunctionNode.BPA_ReportTemplate_Code)
                    {
                        url = this.controller.PortalRoot + ConstantString.PagePath_AddProcessFolder + "&" + ConstantString.Param_ID + "=" + Node.ObjectID + "&ParentID=" + functionID;
                    }
                    break;
                case FunctionNodeType.BizWorkflowPackage:
                    url = this.controller.PortalRoot + ConstantString.PagePath_EditBizWorkflowPackage + "&" + ConstantString.Param_ID + "=" + Node.ObjectID + "&ParentID=" + functionID;
                    break;
                case FunctionNodeType.BizRule:
                    url = this.controller.PortalRoot + ConstantString.PagePath_EditBizRuleTable + "&" + ConstantString.Param_RuleCode + "=" + HttpUtility.UrlEncode(Node.Code) + "&ParentID=" + functionID;
                    break;
                case FunctionNodeType.BPAReportTemplate:
                    ReportTemplate ReportTemplate = controller.Engine.Analyzer.GetReportTemplateByCode(Node.Code);
                    if (ReportTemplate != null)
                    {
                        if (ReportTemplate.ReportType == ReportType.Cross)
                            url = this.controller.PortalRoot + ConstantString.PagePath_ReportTemplateCrossEditUrl;
                        else
                            url = this.controller.PortalRoot + ConstantString.PagePath_ReportTemplateSummaryEditUrl;
                    }
                    url += "&" + ConstantString.Param_Code + "=" + HttpUtility.UrlEncode(Node.Code) + "&ParentID=" + functionID;
                    break;
                default:
                    return Node.Url;
            }
            //return string.IsNullOrWhiteSpace(url) ? url : this.PortalRoot + url;
            return url;
        }
        #endregion

        #region  获取悬浮工具栏Code ------------------------
        /// <summary>
        /// 获取悬浮工具栏Code
        /// </summary>
        /// <param name="Node"></param>
        /// <returns></returns>
        protected string GetToolbarCode(FunctionNode Node)
        {
            switch (Node.Code)
            {
                case FunctionNode.Organization_Data_Code:
                    return ConstantString.TB_Company;
                case FunctionNode.Category_ProcessModel_Code:
                    return ConstantString.TB_ProcessModel;
                case FunctionNode.BizBus_BizService_Code:
                    return ConstantString.TB_BizService;
                case FunctionNode.ProcessModel_BizMasterData_Code:
                    return ConstantString.TB_BizMasterData;
                case FunctionNode.BizRule_ListRuleTable_Code:
                    return ConstantString.TB_BizRule;
                case FunctionNode.BPA_ReportSource_Code:
                    return ConstantString.TB_ReportSource;
                    //qiancheng
                case FunctionNode.Category_Report_Code:
                    return ConstantString.TB_ReportPage;

                //case FunctionNode.BPA_ReportTemplate_Code:
                //    return ConstantString.TB_ReportTemplate;
                case FunctionNode.Category_Apps_Code:
                    return "Apps";
                default:
                    if (Node.NodeType == FunctionNodeType.Function
                        || Node.NodeType == FunctionNodeType.BPAReportSource
                        || Node.NodeType == FunctionNodeType.BPAReportTemplate)
                    {
                        return string.Empty;
                    }
                    return Node.NodeType.ToString();
            }
        }
        #endregion

        #region 获取孩子 ----------------------------------
        private List<PortalTreeNode> GetChildren(FunctionNode node)
        {
            List<PortalTreeNode> treeNodes = new List<PortalTreeNode>();
            switch (node.NodeType)
            {
                case FunctionNodeType.BizService:
                    OThinker.H3.BizBus.BizService.BizService service = controller.Engine.BizBus.GetBizService(node.Code);
                    if (service.AllowCustomMethods)
                    {
                        treeNodes.Add(new PortalTreeNode()
                        {
                            ObjectID = Guid.NewGuid().ToString(),
                            Code = node.Code + "_MethodList",
                            Text = "MenuTreeCategory.AbstractPortalTreeHandler.MethodList",
                            IsLeaf = true,
                            Icon = "fa icon-map",
                            ShowPageUrl = ConstantString.PagePath_ListBizServiceMethod + "&" + ConstantString.Param_ServiceCode + "=" + node.Code,
                            ParentID = node.ObjectID,
                            NodeType = node.NodeType
                        });
                    }
                    break;
                case FunctionNodeType.BizWorkflow:
                    treeNodes.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = node.Code + "_WFHistory",
                        Text = "MenuTreeCategory.AbstractPortalTreeHandler.History",
                        IsLeaf = true,
                        Icon = "fa icon-spinner5",
                        ShowPageUrl = ConstantString.PagePath_BizWorkflowHistory + "&" + ConstantString.Param_WorkflowCode + "=" + HttpUtility.UrlEncode(node.Code),
                        ParentID = node.ObjectID,
                        NodeType = node.NodeType
                    });

                    treeNodes.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = node.Code + "_WFSimulation",
                        Text = "MenuTreeCategory.AbstractPortalTreeHandler.Simulation",
                        IsLeaf = true,
                        Icon = "fa icon-point-right",
                        ShowPageUrl = ConstantString.PagePath_ListSimulation + "&" + ConstantString.Param_WorkflowCode + "=" + HttpUtility.UrlEncode(node.Code),
                        ParentID = node.ObjectID,
                        NodeType = node.NodeType
                    });

                    treeNodes.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = node.Code + "_WFSetting",
                        Text = "MenuTreeCategory.AbstractPortalTreeHandler.Setting",
                        Icon = "fa icon-cogs",
                        IsLeaf = true,
                        ShowPageUrl = ConstantString.PagePath_WorkflowTemplateSetting + "&" + ConstantString.Param_WorkflowCode + "=" + HttpUtility.UrlEncode(node.Code),
                        ParentID = node.ObjectID,
                        NodeType = node.NodeType
                    });
                    break;
            }
            return treeNodes.Count > 0 ? treeNodes : null;
        }
        #endregion

        #region 获取加载数据地址 --------------------------
        /// <summary>
        /// 获取加载数据地址
        /// </summary>
        /// <param name="typeCode">加载数据类型，如加载组织，加载公司，加载部门</param>
        /// <param name="funCode">加载数据的编码，如公司ID，部门ID</param>
        /// <returns></returns>
        protected string GetLoadDataUrl(string FunctionId, string FunctionCode, FunctionNodeType FunctionType, string OwnSchemaCode = "")
        {
            string loadDataUrl = string.Format(controller.PortalRoot + "/Index/LoadTreeData/?{0}={1}&{2}={3}&{4}={5}"
                    , FunctionIDStr
                    , HttpUtility.UrlEncode(FunctionId)
                    , FunctionCodeStr
                    , HttpUtility.UrlEncode(FunctionCode)
                    , FunctionTypeStr
                    , HttpUtility.UrlEncode(FunctionType.ToString()));
            if (!string.IsNullOrEmpty(OwnSchemaCode))
                loadDataUrl = string.Format(loadDataUrl + "&{0}={1}", OwnSchemaCodeStr, HttpUtility.UrlEncode(OwnSchemaCode));
            return loadDataUrl;
        }

        /// <summary>
        /// 获取流程模板类型加载数据地址
        /// </summary>
        /// <param name="typeCode">加载数据类型，如加载组织，加载公司，加载部门</param>
        /// <param name="funCode">加载数据的编码，如公司ID，部门ID</param>
        /// <returns></returns>
        protected string GetLoadWorkflowDataUrl(string FunctionId, string FunctionCode, FunctionNodeType FunctionType)
        {
            return GetLoadWorkflowDataUrl(FunctionId, FunctionCode, FunctionType, false, false);
        }

        /// <summary>
        /// 获取流程模板类型加载数据地址
        /// </summary>
        /// <param name="typeCode">加载数据类型，如加载组织，加载公司，加载部门</param>
        /// <param name="funCode">加载数据的编码，如公司ID，部门ID</param>
        /// <param name="containDraft">是否包含设计中的流程模板</param>
        /// <returns></returns>
        protected string GetLoadWorkflowDataUrl(string FunctionId, string FunctionCode, FunctionNodeType FunctionType, bool containDraft, bool IsBizObjectMode)
        {
            return string.Format(controller.PortalRoot + "/WorkflowTree/CreateWorkflowTree?{0}={1}&{2}={3}&{4}={5}&{6}={7}&{8}={9}"
                , FunctionIDStr
                , HttpUtility.UrlEncode(FunctionId)
                , FunctionCodeStr
                , HttpUtility.UrlEncode(FunctionCode)
                , FunctionTypeStr
                , HttpUtility.UrlEncode(FunctionType.ToString())
                , ContainDraftStr
                , HttpUtility.UrlEncode((containDraft ? true : false).ToString().ToLower())
                , IsBizObjectModeStr
                , HttpUtility.UrlEncode((IsBizObjectMode ? true : false).ToString().ToLower()));
        }

        #endregion

        #region 构建公司节点 ------------------------------
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FunctionNode"></param>
        /// <param name="toolBarCode"></param>
        /// <returns></returns>
        private PortalTreeNode BuildCompanyNode(FunctionNode FunctionNode, string toolBarCode)
        {
            PortalTreeNode companyNode = new PortalTreeNode();

            //
            FunctionNode parentNode = controller.Engine.FunctionAclManager.GetFunctionNodeByCode(FunctionNode.ParentCode);

            var myCompany = controller.Engine.Organization.GetUnit(controller.Engine.Organization.RootUnit.ObjectID);
            companyNode.ObjectID = myCompany.ObjectID;
            companyNode.Code = myCompany.UnitID;
            companyNode.Text = "_" + myCompany.Name;//组织结构名称需要加上"_",否则前端显示时候会以资源文件中的字符串替换，找不到会显示ID
            companyNode.IsLeaf = false;
            //companyNode.ShowPageUrl = FunctionNode.Url;
            companyNode.ParentID = parentNode.ObjectID;
            companyNode.LoadDataUrl = GetLoadDataUrl(myCompany.ObjectID, FunctionNode.Code, FunctionNode.NodeType);
            companyNode.ShowPageUrl = ConstantString.PagePath_EditCompany + "&ID=" + myCompany.UnitID;
            companyNode.ToolBarCode = toolBarCode;
            companyNode.NodeType = FunctionNodeType.Organization;
            companyNode.Icon = "fa fa-building-o";
            return companyNode;
        }
        #endregion

    }
}
