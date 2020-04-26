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
    #region 业务规则子菜单
    public class BizRuleChildren : AbstractPortalTreeHandler
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
        public BizRuleChildren(ControllerBase controller)
        {
            if (_controller == null)
            {
                this._controller = controller;
                this.isAdmin = controller.UserValidator.ValidateAdministrator();
            }
        }

        public override object CreatePortalTree(string ruleID, string ruleCode)
        {
            List<PortalTreeNode> decisionList = new List<PortalTreeNode>();
            OThinker.H3.BizBus.BizRule.BizRuleTable rule = this.controller.Engine.BizBus.GetBizRule(ruleCode);
            if (rule != null && rule.DecisionMatrixes != null)
            {
                foreach (OThinker.H3.BizBus.BizRule.BizRuleDecisionMatrix matrix in rule.DecisionMatrixes)
                {
                    decisionList.Add(new PortalTreeNode()
                    {
                        ObjectID = Guid.NewGuid().ToString(),
                        Code = matrix.Code,
                        Text = "_" + (string.IsNullOrWhiteSpace(matrix.DisplayName) ? matrix.Code : matrix.DisplayName),
                        IsLeaf = true,
                        ParentID = ruleID,
                        ShowPageUrl = this.controller.PortalRoot + ConstantString.PagePath_EditBizRuleDecisionMatrix + "&" + ConstantString.Param_RuleCode + "=" + ruleCode + "&" + ConstantString.Param_DecisionMatrixCode + "=" + matrix.Code + "&ParentID=" + ruleID,
                        Icon = "fa icon-menu"
                    });
                }
            }
            return decisionList;
        }
    }
    #endregion

}
