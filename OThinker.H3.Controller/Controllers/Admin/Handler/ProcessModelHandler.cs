using System;
using System.Web;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using OThinker.H3;
using OThinker.H3.Acl;
using OThinker.H3.DataModel;
using OThinker.H3.Sheet;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Apps;
using OThinker.Data;
using System.Data;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Controllers.AppCode.Admin;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class ProcessModelHandler : AbstractPortalTreeHandler
    {
        private ControllerBase _controller = null;

        protected override ControllerBase controller
        {
            get { return _controller; }
        }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controller">控制器</param>
        public ProcessModelHandler(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        public override object CreatePortalTree(string functionId, string functionCode)
        {
            return ChildrenForProcessModel(functionId, functionCode);
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
            FunctionNode[] functions = _controller.UserValidator.GetFunctionsByParentCode(functionCode, false);
            List<PortalTreeNode> TreeNodeList = new List<PortalTreeNode>();

            if (functions != null)
            {
                functions = functions.OrderBy(s=>s.DisplayName).OrderBy(s => s.SortKey).ToArray();
                FunctionNode[] Folders = functions.Where(p => p.NodeType == FunctionNodeType.BizFolder).ToArray();
                if (Folders != null)
                {
                    foreach (FunctionNode Folder in Folders)
                    {
                        if (string.IsNullOrEmpty(Folder.IconCss))
                        {
                            Folder.IconCss = "fa fa-folder-o";
                        }
                    }
                    TreeNodeList = ConvertToPortalTree(functionID, Folders);
                }
                FunctionNode[] ShemaNodes = functions.Where(p => p.NodeType == FunctionNodeType.BizWFFolder).ToArray();
                if (ShemaNodes != null)
                {
                    foreach (FunctionNode ShemaNode in ShemaNodes)
                    {
                        //ShemaNode.IconCss = "icon-liuchengshujumoxing";
                        ShemaNode.IconCss = "fa fa-folder-o";
                    }

                    //增加ExtendObject,存储DisplayName
                    List<PortalTreeNode> list = ConvertToPortalTree(functionID, ShemaNodes);
                    foreach (var node in list)
                    {
                        node.ExtendObject = new { Text = ShemaNodes.FirstOrDefault(s => s.ObjectID == node.ObjectID).DisplayName };
                    }
                    TreeNodeList.AddRange(list);
                }

                FunctionNode[] PackageNodes = functions.Where(p => p.NodeType != FunctionNodeType.BizFolder && p.NodeType != FunctionNodeType.BizWFFolder).ToArray();
                if (PackageNodes != null)
                {
                    foreach (FunctionNode PackageNode in PackageNodes)
                    {
                        if (PackageNode.NodeType == OThinker.H3.Acl.FunctionNodeType.BizObject)
                        {
                            PackageNode.IconCss = "fa icon-zhushujushili";
                        }
                        else if (PackageNode.NodeType == OThinker.H3.Acl.FunctionNodeType.BizWorkflowPackage)
                        {
                            PackageNode.IconCss = "fa " + GetWorkflowPackageIcon(PackageNode); // icon-liuchengshujumoxing";
                        }
                        else if (PackageNode.NodeType == OThinker.H3.Acl.FunctionNodeType.ReportFolderPage) {
                            PackageNode.IconCss = "fa icon-charubiaoge";
                        }
                        else
                            PackageNode.IconCss = "fa fa-folder-o";
                    }
                    //增加ExtendObject,存储DisplayName
                    List<PortalTreeNode> list = ConvertToPortalTree(functionID, PackageNodes);
                    foreach (var node in list)
                    {
                        node.ExtendObject = new
                        {
                            Text = PackageNodes.FirstOrDefault(s => s.ObjectID == node.ObjectID).DisplayName
                        };
                    }
                    TreeNodeList.AddRange(list);
                }
            }
            return TreeNodeList;
        }

        /// <summary>
        /// 获取流程模板表
        /// </summary>
        public DataTable DTWorkflowCaluse
        {
            get
            {
                if (_controller.ViewData["DTWorkflowCaluse"] == null)
                {
                    string sql = OThinker.Data.Database.Database.CreateSql(_controller.Engine.Query.CommandFactory.DatabaseType,
                        WorkflowClause.TableName,
                        new string[] { WorkflowClause.PropertyName_BizSchemaCode, WorkflowClause.PropertyName_OwnSchemaCode },
                        WorkflowClause.PropertyName_State + "=" + ((int)WorkflowState.Active).ToString(),
                        string.Empty,
                        OThinker.Data.Database.Database.UnspecificRowNum,
                        OThinker.Data.Database.Database.UnspecificRowNum);
                    _controller.ViewData["DTWorkflowCaluse"] = _controller.Engine.Query.QueryTable(sql);
                }
                return _controller.ViewData["DTWorkflowCaluse"] as DataTable;
            }
        }

        /// <summary>
        /// 获取流程包显示的图标
        /// </summary>
        /// <param name="WorkflowNode"></param>
        /// <returns></returns>
        public string GetWorkflowPackageIcon(FunctionNode WorkflowNode)
        {
            string icon = "icon-liuchengshujumoxing";
            if (!string.IsNullOrEmpty(WorkflowNode.LockedBy))
            {
                if (WorkflowNode.LockedBy == _controller.UserValidator.UserID)
                {
                    icon = "fa-check";
                }
                else
                {
                    icon = "fa-lock";
                }
            }

            DataRow[] workflows = DTWorkflowCaluse.Select(WorkflowClause.PropertyName_BizSchemaCode + "='" + WorkflowNode.Code + "' or " + WorkflowClause.PropertyName_OwnSchemaCode + "='" + WorkflowNode.Code + "'");
            if (workflows == null || workflows.Length == 0)
            {
                icon += " disabled";
            }
            return icon;
        }
        #endregion
    }
}