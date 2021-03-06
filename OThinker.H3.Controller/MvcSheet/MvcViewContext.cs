using System;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Web.UI;
using System.ComponentModel;
using System.Collections;
using System.Threading;
using System.Globalization;
using System.Web;
using System.Data;
using System.Web.UI.HtmlControls;
using OThinker.H3.Acl;
using System.Reflection;
using OThinker.H3.Data;
using OThinker.H3.Instance;
using OThinker.H3.DataModel;
using System.IO;
using System.Text;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;
using OThinker.Organization;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// MVC后台传递到表单数据
    /// </summary>
    public class MvcViewContext
    {
        /// <summary>
        /// 流程的发起环节编码
        /// </summary>
        public string StartActivityCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置提交/驳回的节点名称
        /// </summary>
        public string ActivityCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置表单的显示名称
        /// </summary>
        public string DisplayName
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置数据模型显示名称
        /// </summary>
        public string SchemaCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前用户ID
        /// </summary>
        public string UserID
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前用户编码
        /// </summary>
        public string UserCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前用户的姓名
        /// </summary>
        public string UserName
        {
            get;
            set;
        }
        /// <summary>
        /// 获取或设置当前用户的图像地址
        /// </summary>
        public string UserImage
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置发起人ID
        /// </summary>
        public string Originator
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置发起人Code
        /// </summary>
        public string OriginatorCode
        {
            get;
            set;
        }

        /// <summary>
        /// 公司信息
        /// </summary>
        public Organization.OrganizationUnit Company
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前前端使用的语言
        /// </summary>
        public string Language
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置表单打印的URL
        /// </summary>
        public string PrintUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置流程编码
        /// </summary>
        public string WorkflowCode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置流程版本号
        /// </summary>
        public int WorkflowVersion
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前表单模式
        /// </summary>
        public SheetMode SheetMode
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前开发模式(开发平台/流程)
        /// </summary>
        public SheetDataType SheetDataType
        {
            get;
            set;
        }

        /// <summary>
        /// 是否发起模式
        /// </summary>
        public bool IsOriginateMode
        {
            get;
            set;
        }

        /// <summary>
        /// 是否移动表单
        /// </summary>
        public bool IsMobile
        {
            get;
            set;
        }

        /// <summary>
        /// 移动端返回地址
        /// </summary>
        public string MobileReturnUrl
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置流程实例ID
        /// </summary>
        public string InstanceId
        {
            get;
            set;
        }

        /// <summary>
        /// 业务对象ID
        /// </summary>
        public string BizObjectID
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置工作任务ID
        /// </summary>
        public string WorkItemId
        {
            get;
            set;
        }

        private string _Message = string.Empty;
        /// <summary>
        /// 获取或设置需要通知前端的消息
        /// </summary>
        public string Message
        {
            get
            {
                return this._Message;
            }
            set
            {
                this._Message = value;
            }
        }

        private bool _Close = false;
        /// <summary>
        /// 获取或设置消息通知后是否立即关闭窗口
        /// </summary>
        public bool Close
        {
            get
            {
                return this._Close;
            }
            set
            {
                this._Close = value;
            }
        }

        /// <summary>
        /// 设置是否显示同意/不同意
        /// </summary>
        public bool ApprovalListVisible
        {
            get;
            set;
        }

        /// <summary>
        /// 获取提交的角色信息
        /// </summary>
        public List<MvcListItem> Posts
        {
            get;
            set;
        }

        /// <summary>
        /// 获取可以提交的活动节点集合
        /// </summary>
        public List<MvcListItem> SubmitActivities
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置发起人所属OU
        /// </summary>
        public string OriginatorOU
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前用户以及关联用户所有的OU和名称集合
        /// 获取或设置当前用户直接所属组织<组织ID,组织名称>，如果兼职，那么返回的是多个
        /// </summary>
        public Dictionary<string, string> DirectParentUnits
        {
            // TODO:这里计算 UserValidator.RelationUsers 的数量
            // TODO:批量获取 RelationUsers 和用户主身份所在OU的id和名称集合 Dictionary<OU.ID,OU.Name>
            // TODO
            /* 
             * 前端需要做一个 SheetRelationOU 的下拉框控件，替换发起人所属组织控件
             * 绑定一个默认的系统数据项OriginatorOU，
             * 如果RelationOU只返回了一个或者为空，那SheetRelationOU为只读，并且显示为主单位的OU名称
             * 如果RelationOU为多个，则显示为一个下拉框，并且只允许在流程发起状态时可编辑，
             * 将 SheetRelationOU 的值在保存时传回后台，这个值不存储在 BizObject 中，而是设置流程发起人为当前用户在所选组织中兼职用户的ObjectID
             * 
             * TODO:假设研发部用户A，存在兼职实施部用户B，那么A发起流程，选择发起部门为实施部时
             * 注意验证发起后，OT_InstanceContext.Originator字段为B的ObjectID，第一个OT_WorkItem.Participant的值为A的ID,OT_WorkItem.RelationUserID的值为B的ID
             */
            get;
            set;
        }

        /// <summary>
        /// 获取可以驳回的活动节点集合
        /// </summary>
        public List<MvcListItem> RejectActivities
        {
            get;
            set;
        }

        /// <summary>
        /// 获取可以调整的活动节点集合
        /// </summary>
        public List<MvcListItem> AdjustActivities
        {
            get;
            set;
        }

        /// <summary>
        /// 获取当前工作任务的类型
        /// </summary>
        public WorkItemType WorkItemType
        {
            get;
            set;
        }

        /// <summary>
        /// 当前表单的数据项集合值
        /// </summary>
        public MvcBizObject BizObject
        {
            get;
            set;
        }

        /// <summary>
        /// 获取或设置当前表单允许的操作
        /// </summary>
        public PermittedActions PermittedActions
        {
            get;
            set;
        }

        /// <summary>
        /// 优先级
        /// </summary>
        public Dictionary<string, string> Priorities
        {
            get;
            set;
        }

        /// <summary>
        /// HiddenField
        /// </summary>
        public Dictionary<string, string> HiddenFields
        {
            get;
            set;
        }

        /// <summary>
        /// 如果要征询意见/协办/传阅，那么可选的征询意/协办/传阅的人员由这里获得
        /// </summary>
        public Dictionary<string, UserOptions> OptionalRecipients
        {
            get;
            set;
        }

        /// <summary>
        /// 当前处理人的签章
        /// </summary>
        public List<Signature> MySignatures
        {
            get;
            set;
        }

        public ExtendProperty Activity
        {
            get;
            set;
        }

        public ExtendProperty WorkflowTemplate
        {
            get;
            set;
        }

        public string PageScript
        {
            get;
            set;
        }
        public bool ConsultantFinished
        {
            get;
            set;
        }
        public bool Consulted
        {
            get;
            set;
        }

    }

    public class ExtendProperty
    {
        /// <summary>
        /// 扩展属性1
        /// </summary>
        public string ShortText1 { get; set; }
        /// <summary>
        /// 扩展属性2
        /// </summary>
        public string ShortText2 { get; set; }
        /// <summary>
        /// 扩展属性3
        /// </summary>
        public string ShortText3 { get; set; }
        /// <summary>
        /// 扩展属性4
        /// </summary>
        public string ShortText4 { get; set; }
        /// <summary>
        /// 扩展属性5
        /// </summary>
        public string ShortText5 { get; set; }
        /// <summary>
        /// 扩展属性6
        /// </summary>
        public string LongText6 { get; set; }
        /// <summary>
        /// 扩展属性7
        /// </summary>
        public string LongText7 { get; set; }
    }
}