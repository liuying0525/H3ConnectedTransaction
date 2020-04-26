using OThinker.H3.Acl;
using OThinker.H3.Configs;
using OThinker.H3.Controllers.AppCode.Admin;
using OThinker.H3.Controllers.Controllers.Admin.Handler;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Deployment;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysDeployment
{
    /// <summary>
    /// 一键部署控制器
    /// </summary>
   [Authorize]
    public class SysDeploymentController : ControllerBase
    {
        /// <summary>
        /// 获取功能节点编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.SysDeployment_OneButtonDeployment_Code; }
        }
        #region
        /// <summary>
        /// 获取当前注册授权类型
        /// </summary>
        public OThinker.H3.Configs.LicenseType LicenseType
        {
            get
            {
                return (OThinker.H3.Configs.LicenseType)this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType];
            }
        }
        /// <summary>
        /// 获取应用中心是否允许使用
        /// </summary>
        public bool AppsAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Apps] + string.Empty);
            }
        }

        /// <summary>
        /// 获取BPA是否允许使用
        /// </summary>
        public bool BPAAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BPA] + string.Empty);
            }
        }

        /// <summary>
        /// 获取业务集成是否允许使用
        /// </summary>
        public bool BizBusAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizBus] + string.Empty);
            }
        }
        /// <summary>
        /// 获取业务规则是否允许使用
        /// </summary>
        public bool BizRuleAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizRule] + string.Empty);
            }
        }

        /// <summary>
        /// 不需要部署的节点
        /// </summary>
        List<string> filterCode = new List<string> { 
            FunctionNode.BizBus_ListBizAdapter_Code,
            FunctionNode.BizBus_EditSAPConnectionConfig_Code,
            FunctionNode.BizRule_ListBizAdapter_Code, 
            FunctionNode.Category_SysLog_Code, 
            FunctionNode.Category_SysDeployment_Code,
            FunctionNode.SysParam_FileServer_Code,
            FunctionNode.SysParam_SharePoint_Code
        };

        /// <summary>
        /// 最新部署时间
        /// </summary>
        private DateTime lastDeploymentTime
        {
            get
            {
                DateTime time;
                if (!DateTime.TryParse(Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_LastDeploymentTime), out time))
                {
                    time = DateTime.MinValue;
                }
                return time;
            }
        }

        #endregion
        /// <summary>
        /// 获取引擎实例基本信息
        /// </summary>
        /// <returns>引擎实例基本信息</returns>
        [HttpGet]
        public JsonResult GetSysDeploymentData()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                SysDeploymentViewModel model = new SysDeploymentViewModel();
                model.IPAddr = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentServerHost);
                model.Port = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentServerPort);
                model.EngineName = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_DeploymentEngineCode);
                model.UserName = string.Empty;
                model.Password = string.Empty;
                result.Extend = model;
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取引擎实例树信息
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetDepolyTree()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                AbstractPortalTreeHandler handler = CreateHandler(this, FunctionNodeType.Function);
                List<PortalTreeNode> root = handler.CreatePortalTree("", FunctionNode.AdminRootCode) as List<PortalTreeNode>;
                if (root != null)
                {
                    foreach (PortalTreeNode node in root)
                    {
                        node.NeedDeploy = true;
                        SetChildTreeNode(this, node);
                    }
                }
                result.Extend = new { Rows = root };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 检查部署信息是否合法
        /// </summary>
        /// <param name="model">部署信息</param>
        /// <returns>检查结果</returns>
        [HttpPost]
        public JsonResult CheckConnection(SysDeploymentViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result;
                result = CheckSysDeployment(model);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 保存部署信息
        /// </summary>
        /// <param name="model">部署信息模型</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        public JsonResult SaveDeployment(SysDeploymentViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result;
                result = CheckSysDeployment(model);
                if (result.Success)
                {
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DeploymentServerHost, model.IPAddr);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DeploymentServerPort, model.Port);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DeploymentEngineCode, model.EngineName);
                    this.Session["LoginName"] = model.UserName;
                    this.Session["Password"] = model.Password;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 更新部署时间
        /// </summary>
        [HttpGet]
        public void RecordDeloyTime()
        {
            Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_LastDeploymentTime, DateTime.Now.ToString());
        }
        /// <summary>
        /// 执行部署方法
        /// </summary>
        /// <param name="code">节点编码</param>
        /// <param name="node">节点类型</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult ExecuteDeploy(string code, string node)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                FunctionNodeType nodeType = FunctionNodeType.Function;
                if (string.IsNullOrEmpty(code) ||
                    !Enum.TryParse<FunctionNodeType>(node, out nodeType))
                {
                    result.Extend = new
                    {
                        State = false,
                        ErroMsg = "SysDeploymen.Msg0"
                    };
                    return Json(result.Extend, JsonRequestBehavior.AllowGet);
                }

                string loginName = this.Session["LoginName"].ToString();
                string password = this.Session["Password"].ToString();
                DeploymentProcess deploymentProcess = new DeploymentProcess(this, loginName, password);
                deploymentProcess.Connection();
                object obj = deploymentProcess.OneButtonDeployment(nodeType, code);
                deploymentProcess.Dispose();
                return Json(obj, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 检查输入信息是否合法
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private ActionResult CheckSysDeployment(SysDeploymentViewModel model)
        {
            ActionResult result = new ActionResult();
            result = new ActionResult();
            result.Success = true;
            string ServerHost = model.IPAddr;
            string Port = model.Port;
            string EngineName = model.EngineName;
            string LoginName = model.UserName;
            string Password = model.Password;

            string errorMsg = CheckIP(ServerHost);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                result.Message = errorMsg;
                result.Success = false;
                return result;


            }
            errorMsg = CheckPort(Port);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                result.Message = errorMsg;
                result.Success = false;
                return result;
            }
            string extendMsg;
            errorMsg = CheckEnginConnet(ServerHost, Port, EngineName, LoginName, Password, out extendMsg);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                result.Message = errorMsg;
                result.Extend = extendMsg;
                result.Success = false;
                return result;
            }
            return result;
        }

        private string CheckIP(string ServerHost)
        {
            if (string.IsNullOrWhiteSpace(ServerHost))
            {
                return "SysDeployment.Msg1";
            }

            string[] IPPart = ServerHost.Split('.');
            if ((IPPart.Length != 0 && ServerHost == ".") || IPPart.Length != 4)
            {
                return "SysDeployment.Msg2";
            }

            foreach (string ip in IPPart)
            {
                if (ip.Length > 3)
                {
                    return "SysDeployment.Msg2";
                }

                if (Int32.Parse(ip) > 255)
                {
                    if (ip.Length > 3)
                    {
                        return "SysDeployment.Msg2";
                    }
                }
            }
            return string.Empty;
        }

        private string CheckPort(string Port)
        {
            int portNum;
            if (!Int32.TryParse(Port, out portNum))
            {
                return "SysDeployment.Msg3";
            }

            if (portNum >= 0 && portNum <= 65535)
            {
                return string.Empty;
            }
            else
            {
                return "SysDeployment.Msg3";
            }
        }

        private string CheckEnginConnet(string ServerHost, string Port, string EngineName, string LoginName, string Password, out string extendMsg)
        {
            string ConnString = string.Format("Servers={0}:{1};User={2};Password={3};Engine={4}",
                ServerHost, Port, LoginName, Password, EngineName);
            extendMsg = "";
            try
            {
                OThinker.H3.Connection conn = new Connection();
                Clusterware.ConnectionResult connResult = conn.Open(string.Format(ConnString, ServerHost, Port, LoginName, Password, EngineName));
                conn.Dispose();
                if (connResult == Clusterware.ConnectionResult.Success)
                {
                    return string.Empty;
                }
                else
                {
                    extendMsg = "LoginMsg." + connResult;
                    return "SysDeployment.Msg4";
                }
            }
            catch (Exception ex)
            {
                extendMsg = ex.Message;
                return "SysDeployment.Msg4";
            }
        }
       [NonAction]
        public List<PortalTreeNode> GetReportTreeNodes(ControllerBase controller)
        {
            var hander = new ReportTreeHander(controller);
            List<PortalTreeNode> root = hander.CreatePortalTree("", FunctionNode.Category_Report_Code) as List<PortalTreeNode>;
            if (root != null)
            {
                foreach (PortalTreeNode node in root)
                {
                    node.NeedDeploy = true;
                    SetChildTreeNode(controller, node);
                }
            }
            return root;
        } 
        private AbstractPortalTreeHandler CreateHandler(ControllerBase context, FunctionNodeType nodeType)
        {
            switch (nodeType)
            {
                case FunctionNodeType.Organization://加载组织机构子节点
                    return new OrganizationHandler(context);
                case FunctionNodeType.ProcessModel://流程模型
                case FunctionNodeType.BizWFFolder:
                case FunctionNodeType.BizFolder:
                    return new ProcessModelHandler(context);//流程目录
                case FunctionNodeType.ServiceFolder://业务服务目录
                    return new BizServiceChildren(context);
                case FunctionNodeType.RuleFolder://业务规则目录
                    return new RuleFolderChildren(context);
                case FunctionNodeType.BizRule:
                    return new BizRuleChildren(context);
                case FunctionNodeType.BizWorkflowPackage://流程包
                    return new WrokflowPackageChildren(context);
                case FunctionNodeType.BizObject://数据模型
                    return new BizOjectChildren(context);
                case FunctionNodeType.Apps://应用程序
                    return new AppsChildren(context);
                case FunctionNodeType.AppNavigation://应用列表
                    return new AppNavigationChildren(context);
                case FunctionNodeType.BPAReportSource://数据源
                case FunctionNodeType.ReportTemplateFolder://报表模板目录
                    return new ReportTreeHander(context);
                default:
                    return new FunctionHandler(context);
            }
        }


        private void SetChildTreeNode(ControllerBase context, PortalTreeNode node)
        {
            //不显示组织模型、数据模型、流程包的子节点
            if (node.Code == FunctionNode.Category_Organization_Code
                || node.NodeType == FunctionNodeType.BizObject
                || node.NodeType == FunctionNodeType.BizWorkflowPackage
                || node.NodeType == FunctionNodeType.BizRule)
            {
                return;
            }

            List<PortalTreeNode> children = CreateHandler(context, node.NodeType).CreatePortalTree(node.ObjectID, node.Code) as List<PortalTreeNode>;
            if (children != null && node.Code == FunctionNode.Category_ServiceMoniter_Code)
            {
                // 根据模块授权过滤部署节点
                List<PortalTreeNode> childrenAuthorized = new List<PortalTreeNode>();
                foreach (PortalTreeNode child in children)
                {
                    if (child.Code == FunctionNode.Category_Apps_Code && !AppsAuthorized ||
                        child.Code == FunctionNode.Category_BizBus_Code && !BizBusAuthorized ||
                        child.Code == FunctionNode.Category_BizRule_Code && !BizRuleAuthorized ||
                        child.Code == FunctionNode.Category_BPA_Code && !BPAAuthorized)
                    {
                        continue;
                    }
                    childrenAuthorized.Add(child);
                }
                children = childrenAuthorized;
            }

            List<PortalTreeNode> childrenFilter = new List<PortalTreeNode>();
            if (children != null)
            {
                // 节点的最新更新时间
                Dictionary<ConfigurationChange, DateTime> modifiedTimeDic = this.Engine.Interactor.GetConfigurationChanges();
                foreach (PortalTreeNode child in children)
                {
                    if (!filterCode.Contains(child.Code))
                    {
                        ConfigurationChange key = new ConfigurationChange(child.NodeType, child.Code);
                        //主数据和流程包只呈现发布后的
                        if (child.NodeType == FunctionNodeType.BizObject || child.NodeType == FunctionNodeType.BizWorkflowPackage)
                        {
                            if (modifiedTimeDic.Keys.Contains(key))
                            {
                                child.NeedDeploy = modifiedTimeDic[key] > lastDeploymentTime;
                                childrenFilter.Add(child);
                            }
                        }
                        else
                        {
                            child.NeedDeploy = true;
                            if (modifiedTimeDic.Keys.Contains(key))
                            {
                                child.NeedDeploy = modifiedTimeDic[key] > lastDeploymentTime;
                            }
                            childrenFilter.Add(child);
                        }
                    }
                }
            }
            node.children = childrenFilter;

            if (children != null)
            {
                foreach (PortalTreeNode child in childrenFilter)
                {
                    SetChildTreeNode(context, child);
                }
            }
        }
    }
}
