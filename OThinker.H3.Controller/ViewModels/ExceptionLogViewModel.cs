using OThinker.H3.Instance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 异常日志
    /// </summary>
    public class ExceptionLogViewModel : ViewModelBase
    {
        /// <summary>
        /// 流程模板
        /// </summary>
        public string Workflow { get; set; }
        /// <summary>
        /// 原名称
        /// </summary>
        public string SourceName { get; set; }
        /// <summary>
        /// 发起人
        /// </summary>
        public string Originator { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SequenceNo { get; set; }
        /// <summary>
        /// 异常时间
        /// </summary>
        public string ExceptionTime { get; set; }
        /// <summary>
        /// 阻塞
        /// </summary>
        public int Block { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public int State { get; set; }

    }
}
