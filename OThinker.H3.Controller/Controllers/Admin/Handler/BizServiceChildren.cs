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
    #region 业务服务的子菜单
    /// <summary>
    /// 取业务服务的子菜单
    /// </summary>
    public class BizServiceChildren : AbstractPortalTreeHandler
    {
        //是否管理员
        public bool isAdmin { get; set; }

        // <summary>
        /// 控制器
        /// </summary>
        private ControllerBase _controller;
        protected override ControllerBase controller
        {
            get
            {
                return _controller;
            }
        }
        public BizServiceChildren(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
                this.isAdmin = controller.UserValidator.ValidateAdministrator();
            }
        }
        public override object CreatePortalTree(string FunctionID, string FunctionCode)
        {
            return ChildrenForBizService(FunctionID, FunctionCode);
        }

        #region 构造业务服务子菜单
        /// <summary>
        /// 构造业务服务子菜单
        /// </summary>
        /// <param name="serverID"></param>
        /// <param name="serverCode"></param>
        /// <returns></returns>
        private object ChildrenForBizService(string serverID, string serverCode)
        {
            List<PortalTreeNode> TreeNodeList = new List<PortalTreeNode>();
            FunctionNode[] Folders = this.controller.Engine.FunctionAclManager.GetFunctionNodesByParentCode(serverCode);
            if (Folders != null)
            {
                foreach (FunctionNode Folder in Folders)
                {
                    Folder.IconCss = "fa fa-folder-o";
                }
                TreeNodeList = ConvertToPortalTree(serverID, Folders);
            }

            List<FunctionNode> functionNodes = new List<FunctionNode>();
            BizService[] bizServices = this.controller.Engine.BizBus.GetBizServicesByFolderCode(serverCode);
            if (bizServices != null)
            {
                for (int i = 0, j = bizServices.Length; i < j; i++)
                {
                    BizService bizService = bizServices[i];
                    if (!bizService.BizAdapterCode.Equals(Declaration.RuleAdapter_Code))
                    {
                        functionNodes.Add(new FunctionNode
                            (bizService.ObjectID,
                            bizService.Code,
                            bizService.DisplayName,
                            "fa icon-yewufuwu",
                            "",
                            FunctionNodeType.BizService,
                            serverCode,
                            i));
                        foreach (FunctionNode fun in functionNodes)
                        {
                            fun.IconCss = "fa icon-yewufuwu";
                        }
                    }
                }
                TreeNodeList.AddRange(ConvertToPortalTree(serverID, functionNodes.ToArray()));
            }
            return TreeNodeList;
        }
        #endregion
    }
    #endregion
}
