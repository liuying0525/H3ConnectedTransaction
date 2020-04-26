using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class MessageSettingsViewModel
    {
        /// <summary>
        /// 获取或设置新任务到达提醒
        /// </summary>
        public List<Notice> NewWorkItemNotice { get; set; }

        /// <summary>
        /// 获取或设置催办时提醒
        /// </summary>
        public Notice UrgencyNotice { get; set; }

        /// <summary>
        /// 获取或设置定期发送未完成任务提醒
        /// </summary>
        public Notice UnfinishedWorkItemNotice { get; set; }

        /// <summary>
        /// 获取或设置异常异性
        /// </summary>
        public Notice ExceptionNotice { get; set; }

        /// <summary>
        /// 获取或设置超时提醒策略1
        /// </summary>
        public Notice OvertimePolicy1 { get; set; }

        /// <summary>超时提醒策略2
        /// 获取或设置
        /// </summary>
        public Notice OvertimePolicy2 { get; set; }
    }
}
