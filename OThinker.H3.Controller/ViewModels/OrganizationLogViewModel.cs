using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 组织机构日志显示模型
    /// </summary>
    public class OrganizationLogViewModel : ViewModelBase
    {
        /// <summary>
        /// 设置和获取操作用户
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// 设置或获取操作时间
        /// </summary>
        public string Time { get; set; }
        /// <summary>
        ///  设置或获取操作类型
        /// </summary>
        public string Operation { get; set; }
        /// <summary>
        ///  设置或获取组织机构
        /// </summary>
        public string UnitID { get; set; }
        /// <summary>
        ///  设置或获取操作内容
        /// </summary>
        public string Content { get; set; }
    }
}
