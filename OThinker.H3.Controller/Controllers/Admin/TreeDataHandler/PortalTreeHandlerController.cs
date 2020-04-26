using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using OThinker.H3.Acl;
using OThinker.H3.Analytics;
using OThinker.H3.Controllers.Controllers.Admin.Handler;

namespace OThinker.H3.Controllers
{
    //没有使用 
    public class PortalTreeHandlerController : ControllerBase
    {
        #region 常量
        private const string FunctionTypeStr = "FunctionType";
        private const string FunctionIDStr = "FunctionID";
        private const string FunctionCodeStr = "FunctionCode";
        #endregion

        #region 变量
        /// <summary>
        /// 父级功能类型
        /// </summary>
        protected string FunctionType
        {
            get
            {
                return HttpUtility.UrlDecode(Request[FunctionTypeStr]) ?? FunctionNodeType.Function.ToString();
            }
        }
        /// <summary>
        /// 功能内码
        /// </summary>
        protected string FunctionID
        {
            get
            {
                return HttpUtility.UrlDecode(Request[FunctionIDStr]) ?? "";
            }
        }
        /// <summary>
        /// 获取当前Controller的权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return "";
            }
        }
        #endregion


        public JsonResult CreateHandler()
        {
            return this.ExecuteFunctionRun(() =>
            {
                AbstractPortalTreeHandler handler = this.CreateHandler(this);
                //object treeObj = handler.CreatePortalTree(this.FunctionID, this.FunctionCode);
                object treeObj = handler.CreatePortalTree(this.FunctionID, FunctionNode.AdminRootCode);
                return Json(treeObj, JsonRequestBehavior.AllowGet);
            });

        }

        private AbstractPortalTreeHandler CreateHandler(ControllerBase controller)
        {
            FunctionNodeType nodeType = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), FunctionType);
            switch (nodeType)
            {
                case FunctionNodeType.Organization://加载组织机构子节点
                    return new OrganizationHandler(controller);
                case FunctionNodeType.ProcessModel://流程模型
                case FunctionNodeType.BizWFFolder:
                //case FunctionNodeType.BizFolder:
                //return new ProcessModelHandler(context);//流程目录
                //case FunctionNodeType.ServiceFolder://业务服务目录
                //return new BizServiceChildren(context);
                //case FunctionNodeType.RuleFolder://业务规则目录
                //    return new RuleFolderChildren(context);
                //case FunctionNodeType.BizRule:
                //    return new BizRuleChildren(context);
                //case FunctionNodeType.BizWorkflowPackage://流程包
                //    return new WrokflowPackageChildren(context);
                //case FunctionNodeType.BizObject://数据模型
                //    return new BizOjectChildren(context);
                case FunctionNodeType.Apps://应用程序
                    return new AppsChildren(controller);
                //case FunctionNodeType.AppNavigation://应用列表
                //    return new AppNavigationChildren(context);
                case FunctionNodeType.BPAReportSource://数据源
                case FunctionNodeType.ReportTemplateFolder://报表模板目录
                    return new ReportTreeHander(controller);
                default:
                    return new FunctionHandler(controller);
            }
        }
    }
}
