using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using OThinker.H3.Acl;
using OThinker.H3.Controllers.Controllers.Admin.Handler;

namespace OThinker.H3.Controllers.Controllers.Admin.TreeDataController
{
    /// <summary>
    /// 树 控制器
    /// </summary>
    public class PortalTreeController:ControllerBase
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public PortalTreeController() { }
        public override  string FunctionCode {
            get
            {
                return "";
            }
        }

        #region 常量
        private const string FunctionTypeStr = "FunctionType";
        private const string FunctionIDStr = "FunctionID";
        private const string FunctionCodeStr = "FunctionCode";
        #endregion
        public JsonResult LoadTreeData(string FunctionType, string FunctionID, string FunctionCode="")
        {
            FunctionType = FunctionType??FunctionNodeType.Function.ToString();
            AbstractPortalTreeHandler handler = CreateHandler(FunctionType);
            object treeObj = handler.CreatePortalTree(FunctionID,FunctionCode);
            return Json(treeObj, JsonRequestBehavior.AllowGet);
        }


        #region 创建对象实例
        private AbstractPortalTreeHandler CreateHandler(string functionType)
        {
            FunctionNodeType nodeType = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), functionType);
            switch (nodeType)
            {
                case FunctionNodeType.Organization://加载组织机构子节点
                    return new OrganizationHandler(this);
                //case FunctionNodeType.ProcessModel://流程模型
                //case FunctionNodeType.BizWFFolder:
                //case FunctionNodeType.BizFolder:
                //    return new ProcessModelHandler(context);//流程目录
                case FunctionNodeType.ServiceFolder://业务服务目录
                    return new BizServiceChildren(this);
                case FunctionNodeType.RuleFolder://业务规则目录
                    return new RuleFolderChildren(this);
                case FunctionNodeType.BizRule:
                    return new BizRuleChildren(this);
                //case FunctionNodeType.BizWorkflowPackage://流程包
                //    return new WrokflowPackageChildren(context);
                //case FunctionNodeType.BizObject://数据模型
                //    return new BizOjectChildren(context);
                //case FunctionNodeType.Apps://应用程序
                //    return new AppsChildren(context);
                //case FunctionNodeType.AppNavigation://应用列表
                //    return new AppNavigationChildren(context);
                //case FunctionNodeType.BPAReportSource://数据源
                //case FunctionNodeType.ReportTemplateFolder://报表模板目录
                //    return new ReportTreeHander(context);
                default:
                    var obj = UserValidator;
                    var child = this.UserValidator;
                    return new FunctionHandler(this);
            }
        }
        #endregion
    }
}
