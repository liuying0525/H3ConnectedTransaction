using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkflowFolderAclViewModel:ViewModelBase
    {
        /// <summary>
        /// 流程文件夹编码
        /// </summary>
        public string WorkflowFolderCode { get; set; }
        /// <summary>
        /// 流程文件夹名称
        /// </summary>
        public string WorkflowFolderName { get; set; }

        /// <summary>
        /// 用户，组，组织
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 权限管理
        /// </summary>
        public bool Run { get; set; }

        /// <summary>
        /// 是否有流程文件夹运行权限
        /// </summary>
        public bool FunctionRun { get; set; }
    }
}
