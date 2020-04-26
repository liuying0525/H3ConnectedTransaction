using OThinker.H3.Acl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Admin.Handler
{
    /// <summary>
    /// 
    /// </summary>
    public class FunctionHandler : AbstractPortalTreeHandler
    {
        /// <summary>
        /// 是否同步
        /// </summary>
        protected bool IsSyn = false;
        /// <summary>
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

        public FunctionHandler(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
            }
        }
        public override object CreatePortalTree(string functionId, string functionCode)
        {
            return CreateFunctionTree(functionId, functionCode);
        }

        #region 创建功能树
        /// <summary>
        /// 创建功能树
        /// </summary>
        /// <param name="functionCode"></param>
        /// <returns></returns>
        public object CreateFunctionTree(string functionID, string functionCode)
        {
            FunctionNode[] nodes = controller.UserValidator.GetFunctionsByParentCode(functionCode, !this.IsSyn);
            if (nodes == null) return null;
            return ConvertToPortalTree(functionID, nodes);
        }
        #endregion
    }
}
