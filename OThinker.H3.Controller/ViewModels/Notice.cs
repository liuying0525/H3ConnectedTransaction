using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 通知类
    /// </summary>
    public class Notice
    {
        /// <summary>
        /// 获取或设置是否发送
        /// </summary>
        public bool WetherSend { get; set; }

        /// <summary>
        /// 获取或设置发送方式
        /// </summary>
        public string NewTaskArrivedNotifyType { get; set; }
        /// <summary>
        /// 获取或设置提醒标题
        /// </summary>
        public string NotificationTitle { get; set; }
        /// <summary>
        /// 获取或设置提醒内容
        /// </summary>
        public string Details { get; set; }

        /// <summary>
        /// 获取或设置提醒时间
        /// </summary>
        public string RemindTime { get; set; }
        /// <summary>
        /// 获取或设置发送频率
        /// </summary>
        public string Fequency { get; set; }

        public string WorkCalendar { get; set; }
        /// <summary>
        /// 获取或设置周期日
        /// </summary>
        public List<string> CycleType { get; set; }

        /// <summary>
        /// 获取或设置提醒之后的处理方式
        /// </summary>
        public string RemindPostAction { get; set; }

        /// <summary>
        /// 获取或设置提醒之后的间隔
        /// </summary>
        public string TimeInterval { get; set; }

    }
}
