using OThinker.H3.Acl;
using OThinker.H3.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    public class AppsChildren : AbstractPortalTreeHandler
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
        public AppsChildren(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }

        public override object CreatePortalTree(string functionId, string functionCode)
        {
            return ChildrenForApps(functionId, functionCode);
        }

        #region 构造应用程序子菜单
        /// <summary>
        /// 构造应用程序子菜单
        /// </summary>
        /// <param name="treeNode"></param>
        /// <returns></returns>
        private object ChildrenForApps(string FunctionID, string FunctionCode)
        {
            List<FunctionNode> lstFunctionNodes = new List<FunctionNode>();
            AppNavigation[] Apps = _controller.Engine.AppNavigationManager.GetAllApps();
            if (Apps != null)
            {
                foreach (AppNavigation app in Apps)
                {
                    lstFunctionNodes.Add(
                        new FunctionNode(
                            app.AppCode,
                            app.DisplayName,
                            app.IconUrl,
                            "Apps/EditApp.html&AppCode=" + app.AppCode,
                            FunctionNodeType.AppNavigation,
                            FunctionNode.Category_Apps_Code,
                            app.SortKey,
                            app.Description));
                }
                for (int i = 0; i < lstFunctionNodes.Count; i++)
                {
                    if (lstFunctionNodes[i].DisplayName.IndexOf("流程中心") > -1)
                    {
                        lstFunctionNodes[i].IconCss = "fa icon-liuchengzhongxin";
                    }
                    else if (lstFunctionNodes[i].DisplayName.IndexOf("报表中心") > -1)
                    {
                        lstFunctionNodes[i].IconCss = "fa fa-bar-chart-o";
                    }
                    else
                        lstFunctionNodes[i].IconCss = "fa fa-folder-o";
                }
            }

            return ConvertToPortalTree(FunctionID, lstFunctionNodes.ToArray());
        }


        #endregion
    }
}
