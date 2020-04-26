using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 系统管理控制器
    /// </summary>
    public class ServerMonitorController : ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.Category_SysParam_Settings; }
        }
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
        /// 获取SAP是否允许使用
        /// </summary>
        public bool SAPAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_SAP] + string.Empty);
            }
        }

        /// <summary>
        /// 获取Mobile是否允许使用
        /// </summary>
        public bool MobileAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Mobile] + string.Empty);
            }
        }

        /// <summary>
        /// 获取WeChat是否允许使用
        /// </summary>
        public bool WeChatAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_WeChat] + string.Empty);
            }
        }

        /// <summary>
        /// 获取Portal是否允许使用
        /// </summary>
        public bool PortalAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_Portal] + string.Empty);
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
        /// 获取授权到期时间
        /// </summary>
        public DateTime ExpriedDate
        {
            get
            {
                return DateTime.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_ExpiredDate] + string.Empty);
            }
        }

        /// <summary>
        /// 获取在线用户数
        /// </summary>
        /// <returns>在线用户数</returns>
        [HttpGet]
        public JsonResult GetServerMonitor()
        {
            return ExecuteFunctionRun(() =>
            {
                ServerMonitorViewModel model = new ServerMonitorViewModel();

                model.UserCount = AppUtility.Engine.Query.Count(OThinker.Organization.User.TableName, new string[] { OThinker.Organization.User.PropertyName_State + "=" + (int)OThinker.Organization.State.Active });
                model.OnlineCount = AppUtility.OnlineUserCount;
                model.TemplateNum = AppUtility.Engine.Query.Count(WorkflowClause.TableName, new string[] { });
                model.InstanceNum = AppUtility.Engine.Query.Count(Instance.InstanceContext.TableName, new string[] { });
                model.RunningInstanceNum = AppUtility.Engine.Query.Count(Instance.InstanceContext.TableName, new string[] { "State=2" });
                model.DatabaseSize = AppUtility.Engine.EngineConfig.GetDbSize().ToString() + "MB";

                int vesselLimit = int.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_VesselLimit] + string.Empty);
                int userLimit = int.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_UserLimit] + string.Empty);
                int workflowLimit = int.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_WorkflowLimit] + string.Empty);
                int languageLimit = int.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LanguageLimit] + string.Empty);
                model.VesselLimit = vesselLimit;
                model.UserLimit = userLimit;
                model.WorkflowLimit = workflowLimit;
                model.LanguageLimit = languageLimit;

                model.LanguageCount = 2;
                model.Expired = this.ExpriedDate.ToString("yyyy-MM-dd");


                if (LicenseType == OThinker.H3.Configs.LicenseType.Develop)
                    model.Register = "ServerMonitor.Develop";
                else if (LicenseType == OThinker.H3.Configs.LicenseType.Test)
                    model.Register = "ServerMonitor.Test";
                else if (LicenseType == OThinker.H3.Configs.LicenseType.Product)
                    model.Register = "ServerMonitor.Official";
                //版本号
                model.LicenseType = AppUtility.GetAppVersion();
                model.CheckedImg = "../WFRes/images/Checked.gif"; ;
                model.UnCheckedImg = "../WFRes/images/UnChecked.gif";
                model.imgBizBus = this.BizBusAuthorized;
                model.imgBizRule = this.BizRuleAuthorized;
                model.imgApps = this.AppsAuthorized;
                model.imgBPA = this.BPAAuthorized;
                model.imgMobile = this.MobileAuthorized;
                model.imgPortal = this.PortalAuthorized;
                model.imgSAP = this.SAPAuthorized;
                model.imgWeChat = this.WeChatAuthorized;

                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="pageInfo">分页信息</param>
        /// <returns>在线用户列表</returns>
        [HttpPost]
        public JsonResult GetOnlineUserList(PagerInfo pageInfo)
        {
            return ExecuteFunctionRun(() =>
            {

                DataTable dt = AppUtility.OnlineUserTable;
                int total = dt.Rows.Count;
                int startIndex = pageInfo.StartIndex - 1;
                int endIndex = pageInfo.EndIndex >= total ? total : pageInfo.EndIndex;
                List<OnlineUserViewModel> list = new List<OnlineUserViewModel>();
                for (int i = startIndex; i < endIndex; i++)
                {
                    OnlineUserViewModel model = new OnlineUserViewModel() {
                        UserID = dt.Rows[i]["UserID"].ToString(),
                        UserName = dt.Rows[i]["UserAlias"].ToString(),
                        LoginTime = dt.Rows[i]["LoginTime"].ToString()
                       
                    };
                    list.Add(model);
                }
                var gridData = new { Rows = list, Total = total };
                return Json(gridData,JsonRequestBehavior.AllowGet) ;
            });
        }
    }
}
