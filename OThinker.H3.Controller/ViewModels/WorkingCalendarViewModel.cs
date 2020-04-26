using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 工作日历模型
    /// </summary>
    public class WorkingCalendarViewModel : ViewModelBase
    {
        /*
         * 因为结构与Entity不一致，所以这里重新定义了，让工作时间设置绑定更简单一些
         */
        /// <summary>
        /// 获取或设置工作日历名称
        /// </summary>
        public string DisplayName { get; set; }
        public bool IsDefault { get; set; }
        public bool IsMondayWork { get; set; }
        public bool IsTuesdayWork { get; set; }
        public bool IsWednesdayWork { get; set; }
        public bool IsThursdayWork { get; set; }
        public bool IsFridayWork { get; set; }
        public bool IsSaturdayWork { get; set; }
        public bool IsSundayWork { get; set; }
        public string MinDate { get; set; }
        public string MaxDate { get; set; }

        public int AMStartHour { get; set; }
        public int AMStartMinute { get; set; }
        public int AMEndHour { get; set; }
        public int AMEndMinute { get; set; }

        public int PMStartHour { get; set; }
        public int PMStartMinute { get; set; }
        public int PMEndHour { get; set; }
        public int PMEndMinute { get; set; }


        /// <summary>
        /// 获取或设置描述信息
        /// </summary>
        public string Description { get; set; }

        // End Class
    }

    /// <summary>
    /// 工作日期模型
    /// </summary>
    public class WorkingDayViewModel : ViewModelBase
    {
        /*
         * 因为结构与Entity不一致，所以这里重新定义了，让工作时间设置绑定更简单一些
         */
        public string CalendarId { get; set; }
        public string CurrentDate { get; set; }
        public bool IsWorkingDay { get; set; }
        public bool IsExceptional { get; set; }
        public string Description { get; set; }
        private int _AMStartHour = -1;
        public int AMStartHour
        {
            get { return this._AMStartHour; }
            set { this._AMStartHour = value; }
        }

        private int _AMStartMinute = -1;
        public int AMStartMinute
        {
            get { return this._AMStartMinute; }
            set { this._AMStartMinute = value; }
        }

        private int _AMEndHour = -1;
        public int AMEndHour
        {
            get { return this._AMEndHour; }
            set { this._AMEndHour = value; }
        }

        private int _AMEndMinute = -1;
        public int AMEndMinute
        {
            get { return this._AMEndMinute; }
            set { this._AMEndMinute = value; }
        }

        private int _PMStartHour = -1;
        public int PMStartHour
        {
            get { return this._PMStartHour; }
            set { this._PMStartHour = value; }
        }

        private int _PMStartMinute = -1;
        public int PMStartMinute
        {
            get { return this._PMStartMinute; }
            set { this._PMStartMinute = value; }
        }

        private int _PMEndHour = -1;
        public int PMEndHour
        {
            get { return this._PMEndHour; }
            set { this._PMEndHour = value; }
        }

        private int _PMEndMinute = -1;
        public int PMEndMinute
        {
            get { return this._PMEndMinute; }
            set { this._PMEndMinute = value; }
        }
    }
}