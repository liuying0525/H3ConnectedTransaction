using OThinker.H3.Acl;
using OThinker.H3.Controllers.Controllers.Admin.Handler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Admin
{
    [Authorize]
    public class IndexController : ControllerBase
    {
        #region 常量
        private const string FunctionTypeStr = "FunctionType";
        private const string FunctionIDStr = "FunctionID";
        private const string FunctionCodeStr = "FunctionCode";
        #endregion
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get
            {
                return FunctionNode.AdminRootCode;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public IndexController() { }

        //临时方法,用于输出对象到 多语言文件
        //[HttpPost]
        //[ValidateInput(false)]
        //public string OutPutName(string html, string reg, string before, string after, bool isClass, int length = 3)
        //{
        //    System.Text.RegularExpressions.Regex r = new System.Text.RegularExpressions.Regex(reg);
        //    System.Text.RegularExpressions.MatchCollection matches = r.Matches(html);
        //    string output = string.Empty;
        //    foreach (var m in matches)
        //    {

        //        output += before + m.ToString() + after;
        //    };
        //    if (isClass)
        //    {
        //        r = new System.Text.RegularExpressions.Regex("\".*\"");
        //        reg = reg.TrimEnd('\"').TrimEnd('*');
        //        output = output.Replace(reg, "string ");
        //        output = r.Replace(output, "").Replace("\"", "");
        //    }
        //    else
        //    {
        //        var regPre = reg.Substring(0, reg.Length - length);
        //        var regAfter = reg.Substring(reg.Length - length + 1, length - 1);
        //        reg = reg.Replace("\\", "");
        //        output = output.Replace(regPre, "").Replace(regAfter, "").Replace("\":\"\",", "\":\"\",\n");
        //        output = r.Replace(output, "");
        //    }
        //    return output;
        //}

        /// <summary>
        /// 获取系统当前时间
        /// </summary>
        /// <returns>系统当前时间</returns>
        [HttpGet]
        public JsonResult GetSystemDate()
        {

            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result.Extend = new
                {
                    StartTime = DateTime.Now.AddDays(-7).ToString("yyyy-MM-dd"),
                    EndTime = DateTime.Now.ToString("yyyy-MM-dd")
                };
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取许可信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetLicense()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                if (this.UserValidator == null)
                {
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType] + string.Empty == OThinker.H3.Configs.LicenseType.Develop.ToString())
                {
                    result.Extend = new
                    {
                        LicenseType = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType],
                        UserLimit = 0,
                        WorkflowLimit = 0,
                        VesselLimit = 0,
                        EngineLimit = 0,
                        LanguageLimit = 0,
                        BizBus = true,
                        BizRule = true,
                        BPA = true,
                        Apps = true,
                        Portal = true,
                        Mobile = true,
                        WeChat = true,
                        SAP = true,
                        ExpiredDate = DateTime.MaxValue
                    };
                    result.Success = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    result.Success = true;
                    result.Extend = new
                    {
                        LicenseType = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType],
                        UserLimit = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_UserLimit],
                        WorkflowLimit = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_WorkflowLimit],
                        VesselLimit = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_VesselLimit],
                        EngineLimit = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_EngineLimit],
                        LanguageLimit = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LanguageLimit],
                        BizBus = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizBus],
                        BizRule = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizRule],
                        BPA = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BPA],
                        Apps = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Apps],
                        Portal = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Portal],
                        Mobile = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Mobile],
                        WeChat = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_WeChat],
                        SAP = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_SAP],
                        ExpiredDate = this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_ExpiredDate]
                    };
                    return Json(result, JsonRequestBehavior.AllowGet);

                }
            }, FunctionCode);
        }

        /// <summary>
        /// 登出
        /// </summary>
        [HttpGet]
        public void LoginOut()
        {
            UserValidatorFactory.Exit(this);
            Session.Clear();
            Session.Abandon();
        }

        /// <summary>
        /// 获取页面初始化数据
        /// </summary>
        /// <returns>页面初始化数据</returns>
        [HttpGet]
        public JsonResult GetIndexPageData()
        {
            return ExecuteFunctionRun(() =>
            {
                FunctionNode DefaultCode = this.UserValidator.GetFunctionNode(OThinker.H3.Acl.FunctionNode.Category_ServiceMoniter_Code);
                bool isDefaultCode = this.UserValidator.GetFunctionNode(OThinker.H3.Acl.FunctionNode.Category_ServiceMoniter_Code) == null;
                var result = new
                {
                    AdminRootCode = FunctionCode,
                    imgUser_src = !string.IsNullOrEmpty(this.UserValidator.ImagePath) ? this.PortalRoot + "/TempImages/" + this.UserValidator.ImagePath : this.PortalRoot + "/WFRes/assets/images/pixel-admin/user.jpg",
                    lblUserInfo_text = this.UserValidator.UserName,
                    lblMobile_text = this.UserValidator.User.Mobile,
                    lblOfficePhone_text = this.UserValidator.User.OfficePhone,
                    lblEmail_text = this.UserValidator.User.Email,
                    defaultTabs = new List<object>()
                };
                result.defaultTabs.Add(new
                    {
                        tabid = isDefaultCode ? "null" : DefaultCode.ObjectID,
                        title = isDefaultCode ? "null" : "AdminIndex." + DefaultCode.Code,
                        url = isDefaultCode ? "" : DefaultCode.Url
                    });

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            , FunctionCode);
        }

        /// <summary>
        /// 加载菜单树
        /// </summary>
        /// <param name="FunctionType">节点引用的对象类型</param>
        /// <param name="FunctionID">菜单ID</param>
        /// <param name="FunctionCode">节点编码编码</param>
        /// <returns>PortalTreeNode列表对象</returns>
        [HttpPost]
        public JsonResult LoadTreeData(string FunctionType, string FunctionID, string FunctionCode = "", string OwnSchemaCode = "")
        {
            return ExecuteFunctionRun(() =>
            {
                FunctionType = FunctionType ?? FunctionNodeType.Function.ToString();
                AbstractPortalTreeHandler handler = CreateHandler(FunctionType);
                object treeObj = null;
                if (!string.IsNullOrEmpty(OwnSchemaCode))
                    treeObj = handler.CreatePortalTree(FunctionID, FunctionCode, OwnSchemaCode);
                else treeObj = handler.CreatePortalTree(FunctionID, FunctionCode);
                return Json(treeObj, JsonRequestBehavior.AllowGet);
            }, string.Empty);
        }

        [HttpGet]
        [AllowAnonymous]
        public JsonResult GetPortalRoot()
        {
            ActionResult result = new ActionResult();
            result.Extend = this.PortalRoot;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #region 创建对象实例
        private AbstractPortalTreeHandler CreateHandler(string functionType)
        {
            FunctionNodeType nodeType = (FunctionNodeType)Enum.Parse(typeof(FunctionNodeType), functionType);
            switch (nodeType)
            {
                case FunctionNodeType.Organization://加载组织机构子节点
                    return new OrganizationHandler(this);
                case FunctionNodeType.ProcessModel://流程模型
                case FunctionNodeType.Report://报表模型qiancheng
                case FunctionNodeType.ReportFolder://报表目录
                case FunctionNodeType.ReportFolderPage://报表页
                case FunctionNodeType.BizWFFolder:
                case FunctionNodeType.BizFolder:
                    return new ProcessModelHandler(this);//流程目录
                case FunctionNodeType.ServiceFolder://业务服务目录
                    return new BizServiceChildren(this);
                case FunctionNodeType.RuleFolder://业务规则目录
                    return new RuleFolderChildren(this);
                case FunctionNodeType.BizRule:
                    return new BizRuleChildren(this);
                case FunctionNodeType.BizWorkflowPackage://流程包
                    return new WrokflowPackageChildren(this);
                case FunctionNodeType.BizObject://数据模型
                    return new BizOjectChildren(this);
                case FunctionNodeType.Apps://应用程序
                    return new AppsChildren(this);
                case FunctionNodeType.AppNavigation://应用列表
                    return new AppNavigationChildren(this);
                case FunctionNodeType.BPAReportSource://数据源
                case FunctionNodeType.ReportTemplateFolder://报表模板目录
                    return new ReportTreeHander(this);
                default:
                    return new FunctionHandler(this);
            }
        }
        #endregion
    }
}
