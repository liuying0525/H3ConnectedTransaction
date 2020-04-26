using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 系统管理显示模型
    /// </summary>
    public class ServerMonitorViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置当前版本
        /// </summary>
        public string LicenseType { get; set; }
        /// <summary>
        /// 获取或设置是否注册
        /// </summary>
        public string Register { get; set; }
        /// <summary>
        /// 获取或设置有效期
        /// </summary>
        public string Expired { get; set; }
        /// <summary>
        /// 获取或设置流程实例数
        /// </summary>
        public int InstanceNum { get; set; }
        /// <summary>
        /// 获取或设置运行中的流程实例数
        /// </summary>
        public int RunningInstanceNum { get; set; }
        /// <summary>
        /// 获取或设置数据库大小
        /// </summary>
        public string DatabaseSize { get; set; }
        /// <summary>
        /// 获取或设置当前在线用户数
        /// </summary>
        public int OnlineCount { get; set; }
        /// <summary>
        /// 获取或设置注册集群服务器数
        /// </summary>
        public int VesselLimit { get; set; }
        /// <summary>
        /// 获取或设置使用注册集群服务器数
        /// </summary>
        public string VesselCount { get; set; }
        /// <summary>
        /// 获取或设置注册流程模板数量
        /// </summary>
        public int WorkflowLimit { get; set; }
        /// <summary>
        /// 获取或设置使用的注册流程模板数量
        /// </summary>
        public int TemplateNum { get; set; }
        /// <summary>
        /// 获取或设置用户总数
        /// </summary>
        public int UserLimit { get; set; }
        /// <summary>
        /// 获取或设置实际使用的用户总数
        /// </summary>
        public int UserCount { get; set; }
        /// <summary>
        /// 获取或设置语言包上限
        /// </summary>
        public int LanguageLimit { get; set; }
        /// <summary>
        /// 获取或设置当前语言包数量
        /// </summary>
        public int LanguageCount { get; set; }

        /// <summary>
        /// 获取或设置是否集成引擎
        /// </summary>
        public bool imgBizBus { get; set; }
        /// <summary>
        /// 获取或设置规则引擎
        /// </summary>
        public bool imgBizRule { get; set; }
        /// <summary>
        /// 获取或设置报表引擎
        /// </summary>
        public bool imgBPA { get; set; }
        /// <summary>
        /// 获取或设置应用引擎
        /// </summary>
        public bool imgApps { get; set; }
        /// <summary>
        /// 获取或设置企业门户
        /// </summary>
        public bool imgPortal { get; set; }
        /// <summary>
        /// 获取或设置移动APP
        /// </summary>
        public bool imgMobile { get; set; }
        /// <summary>
        /// 获取或设置微信企业号
        /// </summary>
        public bool imgWeChat { get; set; }
        /// <summary>
        /// 获取或设置SAPConnector
        /// </summary>
        public bool imgSAP { get; set; }

        public string CheckedImg { get; set; }
        public string UnCheckedImg { get; set; }
    }
}
