using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class MasterPackageViewModel:WorkflowPackageViewModel
    {
        /// <summary>
        /// 获取或设置业务服务
        /// </summary>
        public string BizService { get; set; }

        /// <summary>
        /// 获取或设置是否是导入模式
        /// </summary>
        public bool IsImport { get; set; }
    }
}
