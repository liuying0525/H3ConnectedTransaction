using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 定时任务模型
    /// </summary>
    public class ScheduleInvokerViewModel:ViewModelBase
    {
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }
        ///<summary>
        ///获取或设置
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string Description { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string State { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string StartTime { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string EndTime { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string RecurrencyType { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string IntervalSecond { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string FilterMethod { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string Matcher { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string ExeCondition { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string MethodExec { get; set; }

        /// <summary>
        /// 获取或设置过滤条件定义
        /// </summary>
        public string FilterDefinition { get; set; }

        /// <summary>
        /// 获取或设置创建时间
        /// </summary>
        public string CreatedTime { get; set; }

        /// <summary>
        /// 获取或设置修改时间
        /// </summary>
        public string ModifiedTime { get; set; }

    }
}
