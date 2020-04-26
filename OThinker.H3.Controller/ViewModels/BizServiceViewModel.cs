using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 业务服务viewmodel
    /// </summary>
    public class BizServiceViewModel:ViewModelBase
    {
        /// <summary>
        /// 适配器
        /// </summary>
        public string Class { get; set; }

        /// <summary>
        /// 业务服务编码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 业务服务名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 版本
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// 数据库编码
        /// </summary>
        public string DbCode { get; set; }

        /// <summary>
        /// 数据表名
        /// </summary>
        public string TableName { get; set; }


        /// <summary>
        /// 设置的属性及值
        /// </summary>
        public List<BizServiceSettingViewModel> Settings { get; set; }
    }

    /// <summary>
    /// 业务服务属性ViewModel
    /// </summary>
    public class BizServiceSettingViewModel : ViewModelBase
    {
        /// <summary>
        /// 属性名称
        /// </summary>
        public string SettingName { get; set; }

        /// <summary>
        /// 属性值
        /// </summary>
        public string SettingValue { get; set; }
    }
}
