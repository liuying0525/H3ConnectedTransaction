using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class AppNavigationChildren : AbstractPortalTreeHandler
    {
        private ControllerBase _controller = null;
        protected override ControllerBase controller
        {
            get { return this._controller; }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="controller">控制器</param>
        public AppNavigationChildren(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        public override object CreatePortalTree(string functionId, string functionCode)
        {
            return ChildrenForAppNavigation(functionId, functionCode);
        }

        #region 构造数据模型(业务对象)的子菜单，代码写死的
        /// <summary>
        /// 构造数据模型(业务对象)的子菜单，代码写死的
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        private object ChildrenForAppNavigation(string functionID, string functionCode)
        {
            List<PortalTreeNode> lstTreeNodes = new List<PortalTreeNode>();
            //菜单列表
            lstTreeNodes.Add(new PortalTreeNode()
            {
                ObjectID = Guid.NewGuid().ToString(),
                Code = functionCode + Guid.NewGuid().ToString(),
                Text = "MenuTreeCategory.AppNavigation",
                IsLeaf = true,
                //Icon = "../WFRes/_Content/themes/ligerUI/icons/H3_icons/BizSchemaView.png",
                Icon = "fa fa-th",
                ShowPageUrl = "Apps/AppMenuList.html&AppCode=" + functionCode,
                ParentID = functionID
            });
            return lstTreeNodes.ToArray();
        }
        #endregion
    }
}
