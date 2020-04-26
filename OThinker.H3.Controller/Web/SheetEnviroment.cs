using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers
{
    public class SheetEnviroment
    {
        #region URL参数 ---------------------
        /// <summary>
        /// 表单URL上的参数，用于指定打开表单所使用的模式，比如：只读模式、打印模式、工作模式等
        /// </summary>
        public const string Param_Mode = "Mode";
        /// <summary>
        /// 表单上模式编码参数
        /// </summary>
        public const string Param_SchemaCode = "SchemaCode";
        /// <summary>
        /// 表单上业务对象ID参数
        /// </summary>
        public const string Param_BizObjectID = "BizObjectID";
        /// <summary>
        /// 表单URL上的参数，如果是以打开一个工作项的角度来打开这个表单的话，那么需要指定该参数
        /// </summary>
        public const string Param_WorkItemID = "WorkItemID";
        /// <summary>
        /// 表单URL上的参数，如果是以打开一个流程的表单的角度来打开这个表单的话，那么需要指定该参数
        /// </summary>
        public const string Param_InstanceId = "InstanceId";
        /// <summary>
        /// 表单URL上的参数，在查看和打印试图中，用于通过参数指定当前表单对应的是哪个活动，通常并不使用该参数。
        /// </summary>
        public const string Param_Activity = "Activity";
        /// <summary>
        /// 表单URL上的参数，流程发起时有效，用于指定当前表单对应的流程模板。实际上，一个表单可以被N个流程模板共用。
        /// </summary>
        public const string Param_WorkflowCode = "WorkflowCode";
        /// <summary>
        /// 表单编码
        /// </summary>
        public const string Param_SheetCode = "SheetCode";
        /// <summary>
        /// 表单URL上的参数，流程发起时有效，用于指定当前表单对应的流程模板。实际上，一个表单可以被N个流程模板/版本共用。
        /// </summary>
        public const string Param_WorkflowVersion = "WorkflowVersion";
        /// <summary>
        /// 表单URL上的参数，用于指定当前用户名，这样用户不用登陆即可直接打开表单
        /// </summary>
        public const string Param_LoginName = "LoginName";
        /// <summary>
        /// 表单URL上的参数，用于指定当前用户的密码，这样用户不用登陆即可直接打开表单
        /// </summary>
        public const string Param_LoginPassword = "LoginPassword";
        public const string Param_LoginSID = "LoginSID";
        public const string Param_MobileToken = "MobileToken";

        /// <summary>
        /// 表单URL上的参数，用于传递业务对象的 Method Name
        /// </summary>
        public const string Param_Method = "Method";
        /// <summary>
        /// 是否通过手机访问的
        /// </summary>
        public const string Param_IsMobile = "IsMobile";
        /// <summary>
        /// 保存后
        /// </summary>
        public const string Param_AfterSave = "AfterSave";
        #endregion
    }
}
