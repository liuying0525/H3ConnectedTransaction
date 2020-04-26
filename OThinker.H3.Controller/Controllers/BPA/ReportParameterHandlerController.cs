using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.Analytics;
using OThinker.H3.Controllers.Controllers.Admin.Handler;
using OThinker.H3.Controllers.ViewModels;
using OThinker.Organization;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.BPA
{
    /// <summary>
    /// 报表数据源控制器
    /// </summary>
    [Authorize]
    public class ReportParameterHandlerController : ControllerBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ReportParameterHandlerController()
        {
        }

        public override string FunctionCode
        {
            get
            {
                return FunctionNode.BPA_ReportSource_Code;
            }
        }

        /// <summary>
        /// 组织机构数据
        /// </summary>
        /// <param name="ParameterValueStr"></param>
        /// <returns></returns>
        public JsonResult OrganizationData()
        {
            return ExecuteFunctionRun(() =>
            {
                AbstractPortalTreeHandler handler = new OrganizationHandler(this);
                PortalTreeNode companyNode = new PortalTreeNode();
                var myCompany = Engine.Organization.Company;
                companyNode.ObjectID = myCompany.ObjectID;
                companyNode.Code = myCompany.UnitID;
                companyNode.Text = myCompany.Name;
                companyNode.Icon = "fa fa-building-o";
                companyNode.IsLeaf = false;
                object treeObj = handler.CreatePortalTree(companyNode.ObjectID, companyNode.Code);
                companyNode.CreateChildren(treeObj);
                return Json(new object[] { companyNode }, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 获取组织机构类型
        /// </summary>
        /// <param name="ParameterValueStr">参数</param>
        /// <returns>组织机构类型</returns>
        public JsonResult GetOrgText(string ParameterValueStr)
        {
            string orgId = string.IsNullOrWhiteSpace(ParameterValueStr) ? this.UserValidator.User.ParentID : ParameterValueStr;
            OThinker.Organization.Unit Unit = this.Engine.Organization.GetUnit(orgId);
            return Json(new { Name = Unit.Name, OrgID = orgId }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取流程信息
        /// </summary>
        /// <returns>流程信息</returns>
        public JsonResult WorkflowCodeData()
        {
            return this.ExecuteFunctionRun(() =>
            {
                MyWorkflowHandler MyWorkflowHandler = new MyWorkflowHandler(this);
                return Json(MyWorkflowHandler.GetWorkflowNodes(false), JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        /// <summary>
        /// 获取数据字典
        /// </summary>
        /// <param name="ParameterValueStr">参数</param>
        /// <returns>主数据信息</returns>
        public JsonResult MasterDataData(string parameterValue)
        {
            string Category = parameterValue;
            AbstractPortalTreeHandler Handler = new MasterDataTreeHandler(this);
            return Json(Handler.CreatePortalTree(Category), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取系统数据项ID
        /// </summary>
        /// <param name="parameterValue"></param>
        /// <returns></returns>
        public JsonResult GetSystemData(string parameterValue)
        {
            if (parameterValue == "1")//当前组织
            {
                return Json(this.UserValidator.User.ParentID, "text/html", JsonRequestBehavior.AllowGet);
            }
            else//当前用户
            {
                return Json(this.UserValidator.User.ObjectID, "text/html", JsonRequestBehavior.AllowGet);
            }
        }
    }
}
