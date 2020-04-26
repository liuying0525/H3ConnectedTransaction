using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 添加用户ViewModel
    /// </summary>
    public class OrgUserViewModel : ViewModelBase
    {

        /// <summary>
        /// 编辑权限
        /// </summary>
        public bool Edit { get; set; }

        /// <summary>
        /// 查看权限
        /// </summary>
        public bool View { get; set; }

        /// <summary>
        /// 登录账户
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户头像
        /// </summary>
        public string UserFace { get; set; }
        /// <summary>
        /// 上级主管
        /// </summary>
        public string Manager { get; set; }

        /// <summary>
        /// 所属机构
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 在职状态
        /// </summary>
        public Organization.UserServiceState ServiceState { get; set; }

        /// <summary>
        /// 启用状态
        /// </summary>
        public Organization.State State { get; set; }

        /// <summary>
        /// 排序值 
        /// </summary>
        public int SortKey { get; set; }
        /// <summary>
        /// 所属角色
        /// </summary>
        public object OrgPost { get; set; }

        public string OrgPostIDS { get; set; }

        /// <summary>
        /// 员工编号
        /// </summary>
        public string EmployeeNumber { get; set; }

        /// <summary>
        /// 员工职级
        /// </summary>
        public int EmployeeRank { get; set; }
        /// <summary>
        /// 称谓
        /// </summary>
        public string Appellation { get; set; }
        /// <summary>
        /// 秘书
        /// </summary>
        public string Secretary { get; set; }
        /// <summary>
        /// 入职日期
        /// </summary>
        public string EntryDate { get; set; }
        /// <summary>
        /// 离职日期
        /// </summary>
        public string DepartureDate { get; set; }
        /// <summary>
        /// 隐私级别
        /// </summary>
        public Organization.PrivacyLevel Privacy { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        public string IDNumber { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public UserGender Gender { get; set; }

        /// <summary>
        /// 出生日期
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 办公电话
        /// </summary>
        public string OfficePhone { get; set; }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string Fax { get; set; }

        /// <summary>
        /// SID 
        /// </summary>
        public string SID { get; set; }

        /// <summary>
        /// 系统管理员
        /// </summary>
        public bool SystemAdmin { get; set; }

        /// <summary>
        /// 允许登录后台
        /// </summary>
        public bool PortalAdmin { get; set; }
        /// <summary>
        /// 系统内置用户
        /// </summary>
        public bool SystemUser { get; set; }
        /// <summary>
        /// 虚拟用户
        /// </summary>
        public bool VirtualUser { get; set; }
        /// <summary>
        /// 关联用户
        /// </summary>
        public string RelationUser { get; set; }

        /// <summary>
        /// 接收邮件提醒
        /// </summary>
        public bool EmailNotify { get; set; }

        /// <summary>
        /// 接收App提醒
        /// </summary>
        public bool AppNotify { get; set; }

        /// <summary>
        /// 接收微信提醒
        /// </summary>
        public bool WechatNotify { get; set; }

        /// <summary>
        /// 接收短信提醒
        /// </summary>
        public bool MessageNotify { get; set; }
        /// <summary>
        /// 接收钉钉提醒
        /// </summary>
        public bool DingTalkNotify { get; set; }
        /// <summary>
        /// 钉钉账户
        /// </summary>
        public string DingTalkAccount { get; set; }
    }
}
