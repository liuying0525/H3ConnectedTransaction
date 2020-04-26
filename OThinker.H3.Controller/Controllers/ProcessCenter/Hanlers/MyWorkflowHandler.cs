using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.Acl;
using System.Web;

namespace OThinker.H3.Controllers
{
    public class MyWorkflowHandler
    {
        private bool ShowFavorite = true;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controller">控制器</param>
        public MyWorkflowHandler(ControllerBase controller)
        {
            this._Controller = controller;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="UserValidator"></param>
        public MyWorkflowHandler(ControllerBase controller, UserValidator UserValidator)
        {
            this._Controller = controller;
            this._UserValidator = UserValidator;
        }

        private UserValidator _UserValidator = null;
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        private UserValidator UserValidator
        {
            get
            {
                if (this._UserValidator == null)
                {
                    this._UserValidator = Controller.UserValidator;
                }
                return this._UserValidator;
            }
        }

        private ControllerBase _Controller = null;

        /// <summary>
        /// 获取控制器
        /// </summary>
        private ControllerBase Controller
        {
            get
            {
                return this._Controller;
            }
        }


        public List<OThinker.H3.Controllers.ViewModels.WorkflowNode> GetWorkflowNodes()
        {
            return GetWorkflowNodes(true, false);
        }

        public List<OThinker.H3.Controllers.ViewModels.WorkflowNode> GetWorkflowNodes(bool ShowFavorite)
        {
            return GetWorkflowNodes(ShowFavorite, false);
        }

        public List<OThinker.H3.Controllers.ViewModels.WorkflowNode> GetWorkflowNodes(bool ShowFavorite, bool IsMobile)
        {
            this.ShowFavorite = ShowFavorite;
            //新逻辑，第一次就获取所有的节点，并记录其层级，然后递归展现；
            //1、或有所有有权限发起的流程模板
            System.Data.DataTable aclTable = Controller.Engine.Query.QueryWorkflow(this.UserValidator.RecursiveMemberOfs, this.UserValidator.ValidateAdministrator());
            //2、根据可以发起的流程模板编码，倒推获取所有的节点集合
            List<string> aclWorkflowCodes = new List<string>();
            foreach (System.Data.DataRow row in aclTable.Rows)
            {
                if (aclWorkflowCodes.Contains(row[DraftWorkflowTemplateHeader.PropertyName_WorkflowCode] + string.Empty)) continue;
                aclWorkflowCodes.Add(row[DraftWorkflowTemplateHeader.PropertyName_WorkflowCode] + string.Empty);
            }
            //FunctionAclManager
            List<FunctionNode> nodes = Controller.Engine.WorkflowManager.GetParentNodesByWorkflowCodes(aclWorkflowCodes);
            //获取所有的流程模板头，用于获取发布时间
            //WorflowTemplateManager
            Dictionary<string, PublishedWorkflowTemplateHeader> dicHeaders = Controller.Engine.WorkflowManager.GetDefaultWorkflowHeaders(aclWorkflowCodes.ToArray());
            List<PublishedWorkflowTemplateHeader> headers = new List<PublishedWorkflowTemplateHeader>();
            //移动端可发起的流程编码
            List<string> MobileVisibleWorkflowCodes = new List<string>();
            //图标集合 <WorkflowCode,ImagePath>
            Dictionary<string, string> WorkflowIcons = new Dictionary<string, string>();
            Dictionary<string, string> WorkflowIconFileNames = new Dictionary<string, string>();
            if (IsMobile)
            {
                List<string> lstSchemaCodes = new List<string>();
                foreach (PublishedWorkflowTemplateHeader header in dicHeaders.Values)
                {
                    if (!lstSchemaCodes.Contains(header.BizObjectSchemaCode.ToLower()))
                    {
                        lstSchemaCodes.Add(header.BizObjectSchemaCode.ToLower());
                    }
                }
                Dictionary<string, WorkflowClause[]> dicClauses = Controller.Engine.WorkflowManager.GetClausesBySchemaCodes(lstSchemaCodes.ToArray());

                foreach (string key in dicClauses.Keys)
                {
                    foreach (WorkflowClause c in dicClauses[key])
                    {
                        if (c.MobileStart && !MobileVisibleWorkflowCodes.Contains(c.WorkflowCode.ToLower()))
                        {
                            MobileVisibleWorkflowCodes.Add(c.WorkflowCode.ToLower());
                            if (!string.IsNullOrEmpty(c.IconFileName))
                            {
                                WorkflowIconFileNames.Add(c.WorkflowCode.ToLower(), c.IconFileName);
                            }
                            else if (!string.IsNullOrEmpty(c.Icon))
                            {
                                WorkflowIcons.Add(c.WorkflowCode.ToLower(), c.Icon);
                            }
                        }
                    }
                }
            }
            foreach (PublishedWorkflowTemplateHeader header in dicHeaders.Values)
            {
                if (!IsMobile || MobileVisibleWorkflowCodes.Contains(header.WorkflowCode.ToLower()))
                {
                    headers.Add(header);
                }
            }
            //剔除掉null对象

            //4.根据流程编码批量获取所有的常用流程
            List<string> favoriteWorkflowCodes = Controller.Engine.WorkflowConfigManager.GetFavoriteWorkflowCodes(this.UserValidator.UserID);
            return GetWorkflowNodeByParentCode(FunctionNode.Category_ProcessModel_Code,
                nodes,
                favoriteWorkflowCodes,
                aclWorkflowCodes,
                headers,
                WorkflowIcons,
                WorkflowIconFileNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parentCode"></param>
        /// <param name="nodes"></param>
        /// <param name="favoriteWorkflows"></param>
        /// <param name="headers"></param>
        /// <param name="WorkflowIcons"></param>
        /// <param name="WorkflowIconFileNames"></param>
        /// <returns></returns>
        private List<OThinker.H3.Controllers.ViewModels.WorkflowNode> GetWorkflowNodeByParentCode(
           string parentCode,
           List<FunctionNode> nodes,
           List<string> favoriteWorkflowCodes,
           List<string> aclWorkflowCodes,
           List<PublishedWorkflowTemplateHeader> headers,
           Dictionary<string, string> WorkflowIcons,
            Dictionary<string, string> WorkflowIconFileNames)
        {

            List<OThinker.H3.Controllers.ViewModels.WorkflowNode> WorkflowNodes = new List<OThinker.H3.Controllers.ViewModels.WorkflowNode>();
            List<FunctionNode> childrenNodes = nodes.Where(p => p.ParentCode.ToLower() == parentCode.ToLower()).ToList();
            if (childrenNodes == null || childrenNodes.Count == 0)
                return null;

            System.Collections.ArrayList sortedChildrenNodes = new System.Collections.ArrayList();
            System.Collections.ArrayList keyList = new System.Collections.ArrayList();
            foreach (FunctionNode c in childrenNodes)
            {
                sortedChildrenNodes.Add(c);
                keyList.Add(c.SortKey);
            }
            sortedChildrenNodes = OThinker.Data.Sorter.Sort(keyList, sortedChildrenNodes);

            List<PublishedWorkflowTemplateHeader> tmpHeaders;

            //读取常用流程
            if (parentCode == FunctionNode.Category_ProcessModel_Code && this.ShowFavorite)
            {
                OThinker.H3.Controllers.ViewModels.WorkflowNode FavoriteNode = new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                {
                    IsLeaf = false,
                    ObjectID = Guid.NewGuid().ToString(),
                    DisplayName = "FrequentFlow",
                    children = new List<OThinker.H3.Controllers.ViewModels.WorkflowNode>()
                };
                foreach (string workflowcode in aclWorkflowCodes)
                {
                    tmpHeaders = FindTemplateHeaders(headers, workflowcode);
                    if (tmpHeaders != null)
                    {
                        foreach (PublishedWorkflowTemplateHeader tmpHeader in tmpHeaders)
                        {
                            if (favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0)
                            {
                                FavoriteNode.children.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                                {
                                    IsLeaf = true,
                                    ObjectID = tmpHeader.ObjectID,
                                    Code = tmpHeader.WorkflowCode,
                                    DisplayName = tmpHeader.DisplayName,
                                    PublishedTime = tmpHeader.PublishTime.ToShortDateString(),
                                    Version = tmpHeader.WorkflowVersion,
                                    Frequent = 1
                                });
                            }
                        }
                    }
                }

                if (FavoriteNode.children.Count > 0)
                {
                    WorkflowNodes.Add(FavoriteNode);

                }
            }

            foreach (FunctionNode node in sortedChildrenNodes)
            {
                switch (node.NodeType)
                {
                    case FunctionNodeType.BizWorkflowPackage:
                        tmpHeaders = FindTemplateHeaders(headers, node.Code);
                        foreach (PublishedWorkflowTemplateHeader tmpHeader in tmpHeaders)
                        {
                            WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                            {
                                IsLeaf = true,
                                ObjectID = tmpHeader.ObjectID,
                                Code = tmpHeader.WorkflowCode,
                                DisplayName = tmpHeader.DisplayName,
                                PublishedTime = tmpHeader.PublishTime.ToShortDateString(),
                                Version = tmpHeader.WorkflowVersion,
                                Icon = "icon-liuchengshujumoxing",
                                Frequent = (favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0 ? 1 : 0)
                            });
                            //headers.Remove(tmpHeader);
                        }
                        break;
                    case FunctionNodeType.BizWorkflow:
                        tmpHeaders = FindTemplateHeaders(headers, node.Code);
                        foreach (PublishedWorkflowTemplateHeader tmpHeader in tmpHeaders)
                        {
                            WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                            {
                                IsLeaf = true,
                                ObjectID = tmpHeader.ObjectID,
                                Code = tmpHeader.WorkflowCode,
                                DisplayName = tmpHeader.DisplayName,
                                PublishedTime = tmpHeader.PublishTime.ToShortDateString(),
                                Version = tmpHeader.WorkflowVersion,
                                Icon = "icon-liuchengmoxing",
                                Frequent = (favoriteWorkflowCodes.Where(c => string.Compare(c, tmpHeader.WorkflowCode, true) == 0).Count() != 0 ? 1 : 0)
                            });
                            headers.Remove(tmpHeader);
                        }
                        break;
                    default:
                        List<OThinker.H3.Controllers.ViewModels.WorkflowNode> nodeChildren = GetWorkflowNodeByParentCode(node.Code, nodes, favoriteWorkflowCodes, aclWorkflowCodes, headers, WorkflowIcons, WorkflowIconFileNames);
                        if (nodeChildren.Count > 0)
                        {
                            WorkflowNodes.Add(new OThinker.H3.Controllers.ViewModels.WorkflowNode()
                            {
                                IsLeaf = false,
                                ObjectID = node.ObjectID,
                                Code = node.Code,
                                DisplayName = node.DisplayName,
                                children = nodeChildren,
                                Icon = "icon-liuchengmoxing"
                            });
                        }
                        break;
                }
            }
            //设置图标
            foreach (OThinker.H3.Controllers.ViewModels.WorkflowNode n in WorkflowNodes)
            {
                if (n.Code != null)
                {
                    if (WorkflowIcons.ContainsKey(n.Code.ToLower()))
                    {
                        n.Icon = WorkflowIcons[n.Code.ToLower()];
                    }
                    if (WorkflowIconFileNames.ContainsKey(n.Code.ToLower()))
                    {
                        n.IconFileName = WorkflowIconFileNames[n.Code.ToLower()];
                    }
                }
                if (n.children != null)
                {
                    foreach (OThinker.H3.Controllers.ViewModels.WorkflowNode child in n.children)
                    {
                        if (child.Code != null)
                        {
                            if (WorkflowIcons.ContainsKey(child.Code.ToLower()))
                            {
                                child.Icon = WorkflowIcons[child.Code.ToLower()];
                            }
                            if (WorkflowIconFileNames.ContainsKey(child.Code.ToLower()))
                            {
                                child.IconFileName = WorkflowIconFileNames[child.Code.ToLower()];
                            }
                        }
                    }
                }
            }
            return WorkflowNodes;
        }

        public int GetChildrenNodesOfPackage(string nodeCode, List<FunctionNode> nodes)
        {
            int count = 0;
            foreach (FunctionNode node in nodes)
            {
                if (node.ParentCode == nodeCode)
                {
                    count++;
                }
            }
            return count;
        }

        /// <summary>
        /// 获取流程模板头信息
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="objectCode"></param>
        /// <returns></returns>
        public List<PublishedWorkflowTemplateHeader> FindTemplateHeaders(List<PublishedWorkflowTemplateHeader> headers, string objectCode)
        {
            List<PublishedWorkflowTemplateHeader> lstHeaders = new List<PublishedWorkflowTemplateHeader>();
            foreach (PublishedWorkflowTemplateHeader header in headers)
            {
                if (header.BizObjectSchemaCode == objectCode)
                {
                    lstHeaders.Add(header);
                }
            }
            return lstHeaders;
        }

        // End Class
    }
}
