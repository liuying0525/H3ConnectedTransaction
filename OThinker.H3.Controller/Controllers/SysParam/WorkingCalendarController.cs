using OThinker.H3.Acl;
using OThinker.H3.Calendar;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.WorkflowTemplate;
using OThinker.H3.WorkItem;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 工作日历控制器
    /// </summary>
    public class WorkingCalendarController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.SysParam_WorkingCalendar_Code; }
        }

        /// <summary>
        /// 获取所有工作日历
        /// </summary>
        /// <returns>返回工作日历集合</returns>
        [HttpPost]
        public JsonResult GetWorkingCalendarList()
        {
            return ExecuteFunctionRun(() =>
            {
                Calendar.WorkingCalendar[] calendarList = this.Engine.WorkingCalendarManager.GetCalendarList();
                return Json(calendarList, JsonRequestBehavior.DenyGet);
            });
        }

        /// <summary>
        /// 获取指定月份的工作日历集合
        /// </summary>
        /// <param name="calendarId">工作日历ID</param>
        /// <param name="year">年份</param>
        /// <param name="month">月份</param>
        /// <returns>返回指定年月的工作日期</returns>
        [HttpPost]
        public JsonResult GetWorkingDays(string calendarId, int year, int month)
        {
            return ExecuteFunctionRun(() =>
            {
                Calendar.WorkingDay[] days = this.Engine.WorkingCalendarManager.GetWorkingDays(calendarId);

                List<Calendar.WorkingDay> workingDays = new List<Calendar.WorkingDay>();

                if (days != null)
                {
                    foreach (Calendar.WorkingDay workingDay in days)
                    {
                        if (workingDay.CurrentDate.Year == year && workingDay.CurrentDate.Month == month)
                        {
                            workingDays.Add(workingDay);
                        }
                    }
                }

                return Json(workingDays, JsonRequestBehavior.DenyGet);
            });
        }

        /// <summary>
        /// 获取工作日历实体对象
        /// </summary>
        /// <param name="calendarID">工作日历ID</param>
        /// <returns>返回工作日历</returns>
        [HttpGet]
        public JsonResult GetWorkingCalendar(string calendarID)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = null;

                WorkingCalendar calendar = this.Engine.WorkingCalendarManager.GetCalendar(calendarID);
                if (calendar == null)
                {
                    result = new ActionResult(false, "WorkingCalendar.CalendarNotExists");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                WorkingCalendarViewModel workingCalendar = new WorkingCalendarViewModel()
                {
                    DisplayName = calendar.DisplayName,
                    IsDefault = calendar.IsDefault,
                    IsMondayWork = calendar.IsMondayWork,
                    IsTuesdayWork = calendar.IsTuesdayWork,
                    IsThursdayWork = calendar.IsThursdayWork,
                    IsWednesdayWork = calendar.IsWednesdayWork,
                    IsFridayWork = calendar.IsFridayWork,
                    MinDate = calendar.MinDate.ToString("yyyy-MM-dd"),
                    MaxDate = calendar.MaxDate.ToString("yyyy-MM-dd"),
                    IsSaturdayWork = calendar.IsSaturdayWork,
                    IsSundayWork = calendar.IsSundayWork,
                    Description = calendar.Description,
                    ObjectID = calendar.ObjectID
                };
                if (calendar.WorkingTimeSpans != null && calendar.WorkingTimeSpans.Length > 0)
                {
                    workingCalendar.AMStartHour = calendar.WorkingTimeSpans[0].StartTimeHour;
                    workingCalendar.AMStartMinute = calendar.WorkingTimeSpans[0].StartTimeMinute;
                    workingCalendar.AMEndHour = calendar.WorkingTimeSpans[0].EndTimeHour;
                    workingCalendar.AMEndMinute = calendar.WorkingTimeSpans[0].EndTimeMinute;
                }
                if (calendar.WorkingTimeSpans != null && calendar.WorkingTimeSpans.Length > 1)
                {
                    workingCalendar.PMStartHour = calendar.WorkingTimeSpans[1].StartTimeHour;
                    workingCalendar.PMStartMinute = calendar.WorkingTimeSpans[1].StartTimeMinute;
                    workingCalendar.PMEndHour = calendar.WorkingTimeSpans[1].EndTimeHour;
                    workingCalendar.PMEndMinute = calendar.WorkingTimeSpans[1].EndTimeMinute;
                }
                result = new ActionResult(true, string.Empty, workingCalendar);
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存工作日历
        /// </summary>
        /// <param name="workingCalendar">工作日历模型对象</param>
        /// <returns>返回保存是否成功</returns>
        [HttpPost]
        public JsonResult SaveWorkingCalendar(WorkingCalendarViewModel workingCalendar)
        {
            ActionResult result = null;
            WorkingCalendar calendar = null;
            if (!string.IsNullOrEmpty(workingCalendar.ObjectID))
            {
                calendar = this.Engine.WorkingCalendarManager.GetCalendar(workingCalendar.ObjectID);
                if (calendar == null)
                {
                    result = new ActionResult(false, "WorkingCalendar.CalendarNotExists");
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                calendar = new WorkingCalendar();
            }

            calendar.DisplayName = workingCalendar.DisplayName;
            calendar.IsDefault = workingCalendar.IsDefault;
            calendar.IsMondayWork = workingCalendar.IsMondayWork;
            calendar.IsTuesdayWork = workingCalendar.IsTuesdayWork;
            calendar.IsThursdayWork = workingCalendar.IsThursdayWork;
            calendar.IsWednesdayWork = workingCalendar.IsWednesdayWork;
            calendar.IsFridayWork = workingCalendar.IsFridayWork;
            calendar.MinDate = Convert.ToDateTime(workingCalendar.MinDate);
            calendar.MaxDate = Convert.ToDateTime(workingCalendar.MaxDate);
            calendar.IsSaturdayWork = workingCalendar.IsSaturdayWork;
            calendar.IsSundayWork = workingCalendar.IsSundayWork;
            calendar.Description = workingCalendar.Description;

            List<WorkingTimeSpan> times = new List<WorkingTimeSpan>();
            times.Add(new WorkingTimeSpan()
            {
                StartTimeHour = workingCalendar.AMStartHour,
                StartTimeMinute = workingCalendar.AMStartMinute,
                EndTimeHour = workingCalendar.AMEndHour,
                EndTimeMinute = workingCalendar.AMEndMinute
            });
            times.Add(new WorkingTimeSpan()
            {
                StartTimeHour = workingCalendar.PMStartHour,
                StartTimeMinute = workingCalendar.PMStartMinute,
                EndTimeHour = workingCalendar.PMEndHour,
                EndTimeMinute = workingCalendar.PMEndMinute
            });
            calendar.WorkingTimeSpans = times.ToArray();

            //if (workingCalendar.WorkingTimeSpans == null || workingCalendar.WorkingTimeSpans.Length == 0)
            //{// 没有设置工作时间
            //    result = new ActionResult(false, "WorkingCalendar.NoWorkingTimeSpans");
            //    return Json(result, JsonRequestBehavior.AllowGet);
            //}
            //else
            if (!workingCalendar.IsMondayWork
            && !workingCalendar.IsTuesdayWork
            && !workingCalendar.IsWednesdayWork
            && !workingCalendar.IsThursdayWork
            && !workingCalendar.IsFridayWork
            && !workingCalendar.IsSaturdayWork
            && !workingCalendar.IsSundayWork)
            {// 没有设置工作时间
                result = new ActionResult(false, "WorkingCalendar.NoWorkingDays");
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            if (calendar.MaxDate.Subtract(calendar.MinDate).TotalDays > 366 * 10)
            {
                result = new ActionResult(false, "WorkingCalendar.DaysToLong");
                return Json(result, JsonRequestBehavior.DenyGet);
            }

            return ExecuteFunctionRun(() =>
            {
                if (calendar.Serialized)
                {
                    this.Engine.WorkingCalendarManager.UpdateCalendar(calendar);
                }
                else
                {
                    this.Engine.WorkingCalendarManager.AddCalendar(calendar);
                }
                result = new ActionResult(true, "msgGlobalString.SaveSucced");

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取工作日期
        /// </summary>
        /// <param name="CalendarID"></param>
        /// <param name="WorkingDate"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetWorkingDay(string CalendarID, DateTime WorkingDate)
        {
            WorkingDayViewModel workingDayViewModel = null;
            return ExecuteFunctionRun(() =>
            {
                WorkingDay workingDay = this.Engine.WorkingCalendarManager.GetWorkingDay(CalendarID, WorkingDate);
                workingDayViewModel = new WorkingDayViewModel()
                {
                    CalendarId = workingDay.CalendarId,
                    CurrentDate = workingDay.CurrentDate.ToString("yyyy-MM-dd"),
                    Description = workingDay.Description,
                    IsExceptional = workingDay.IsExceptional,
                    IsWorkingDay = workingDay.IsWorkingDay,
                    ObjectID = workingDay.ObjectID
                };
                if (workingDay.WorkingTimeSpans != null && workingDay.WorkingTimeSpans.Length > 0)
                {
                    workingDayViewModel.AMStartHour = workingDay.WorkingTimeSpans[0].StartTimeHour;
                    workingDayViewModel.AMStartMinute = workingDay.WorkingTimeSpans[0].StartTimeMinute;
                    workingDayViewModel.AMEndHour = workingDay.WorkingTimeSpans[0].EndTimeHour;
                    workingDayViewModel.AMEndMinute = workingDay.WorkingTimeSpans[0].EndTimeMinute;
                }
                if (workingDay.WorkingTimeSpans != null && workingDay.WorkingTimeSpans.Length > 1)
                {
                    workingDayViewModel.PMStartHour = workingDay.WorkingTimeSpans[1].StartTimeHour;
                    workingDayViewModel.PMStartMinute = workingDay.WorkingTimeSpans[1].StartTimeMinute;
                    workingDayViewModel.PMEndHour = workingDay.WorkingTimeSpans[1].EndTimeHour;
                    workingDayViewModel.PMEndMinute = workingDay.WorkingTimeSpans[1].EndTimeMinute;
                }
                return Json(workingDayViewModel, JsonRequestBehavior.DenyGet);
            });

        }

        /// <summary>
        /// 删除工作日历
        /// </summary>
        /// <param name="CalendarID"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult RemoveWorkingCalendar(string CalendarID)
        {
            return ExecuteFunctionRun(() =>
            {
                this.Engine.WorkingCalendarManager.RemoveCalendar(CalendarID);
                ActionResult result = new ActionResult(true);
                return Json(result, JsonRequestBehavior.DenyGet);
            });
        }

        /// <summary>
        /// 更新工作日
        /// </summary>
        /// <param name="workingDay">工作日期</param>
        /// <returns>返回更新是否成功</returns>
        [HttpPost]
        public JsonResult UpdateWorkingDay(WorkingDayViewModel workingDayViewModel)
        {
            ActionResult result = null;

            WorkingDay workingDay = this.Engine.WorkingCalendarManager.GetWorkingDay(workingDayViewModel.CalendarId,
                Convert.ToDateTime(workingDayViewModel.CurrentDate));
            if (workingDay == null)
            {
                result = new ActionResult(false, "WorkingCalendar.WorkingDayNotExists");
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            workingDay.Description = workingDayViewModel.Description;
            workingDay.IsWorkingDay = workingDayViewModel.IsWorkingDay;
            workingDay.IsExceptional = workingDayViewModel.IsExceptional;
            List<WorkingTimeSpan> times = new List<WorkingTimeSpan>();
            if (workingDayViewModel.AMStartHour > -1 && workingDayViewModel.AMEndHour >= workingDayViewModel.AMStartHour
                && workingDayViewModel.AMStartMinute > -1)
            {
                times.Add(new WorkingTimeSpan()
                {
                    StartTimeHour = workingDayViewModel.AMStartHour,
                    StartTimeMinute = workingDayViewModel.AMStartMinute,
                    EndTimeHour = workingDayViewModel.AMEndHour,
                    EndTimeMinute = workingDayViewModel.AMEndMinute
                });
            }

            if (workingDayViewModel.PMStartHour > -1 && workingDayViewModel.PMEndHour >= workingDayViewModel.PMStartHour
                && workingDayViewModel.PMStartMinute > -1)
            {
                times.Add(new WorkingTimeSpan()
                {
                    StartTimeHour = workingDayViewModel.PMStartHour,
                    StartTimeMinute = workingDayViewModel.PMStartMinute,
                    EndTimeHour = workingDayViewModel.PMEndHour,
                    EndTimeMinute = workingDayViewModel.PMEndMinute
                });
            }
            workingDay.WorkingTimeSpans = times.ToArray();

            if (workingDay.IsWorkingDay &&
               (workingDay.WorkingTimeSpans == null || workingDay.WorkingTimeSpans.Length == 0))
            {// 没有设置工作时间
                result = new ActionResult(false, "WorkingCalendar.NoWorkingTimeSpans");
                return Json(result, JsonRequestBehavior.AllowGet);
            }

            return ExecuteFunctionRun(() =>
            {
                this.Engine.WorkingCalendarManager.UpdateWorkingDay(workingDay);
                result = new ActionResult(true, "msgGlobalString.SaveSucced");

                return Json(result, JsonRequestBehavior.DenyGet);
            });
        }

        // End Class
    }
}