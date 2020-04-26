using OThinker.H3.Acl;
using OThinker.H3.Calendar;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Newtonsoft.Json;
using OThinker.H3.Notification;
namespace OThinker.H3.Controllers.Controllers.SysParam
{
    /// <summary>
    /// 消息设置控制器
    /// </summary>
    [Authorize]
    public class MessageSettingsController : ControllerBase
    {
        /// <summary>
        /// 当前模块编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.SysParam_MessageSetting_Code; }
        }
        /// <summary>
        /// 获取消息设置的数据
        /// </summary>
        /// <returns>消息设置数据</returns>
        [HttpGet]
        public JsonResult GetMessageSettingsPageData()
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                try
                {
                    //构造消息设置模型对象
                    MessageSettingsViewModel model = new MessageSettingsViewModel()
                    {
                        NewWorkItemNotice = GetNewWorkItemNotice(),
                        UrgencyNotice = GetUrgencyNotice(),
                        UnfinishedWorkItemNotice = GetUnfinishedWorkItemNotice(),
                        ExceptionNotice = GetExceptionNotice(),
                        OvertimePolicy1 = GetOvertimePolicy1(),
                        OvertimePolicy2 = GetOvertimePolicy2()
                    };

                    result.Extend = new
                    {
                        MessageSettings = model,
                        DeliveryMethods = InitDeliveryMethods(),
                        WorkCalendar = InitWorkCalendar(),
                        RemindPostAction = InitRemindPostAction()
                    };
                    result.Success = true;
                }
                catch (Exception e)
                {
                    result.Success = false;
                    result.Message = e.Message;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存消息设置
        /// </summary>
        /// <param name="formData">消息设置数据</param>
        /// <returns>保存结果ActionResult</returns>
        [ValidateInput(false)]
        [HttpPost]
        public JsonResult SaveMessageSettings(string formData)
        {
            return ExecuteFunctionRun(() =>
            {
                MessageSettingsViewModel model = JsonConvert.DeserializeObject<MessageSettingsViewModel>(formData);
                ActionResult result = new ActionResult();
                try
                {
                    #region 新任务到达时提醒
                    foreach (Notice notice in model.NewWorkItemNotice)
                    {
                        if (!string.IsNullOrEmpty(notice.Details))
                        {
                            notice.Details = System.Text.RegularExpressions.Regex.Replace(notice.Details, "\\s*|\t|\r|\n", "");
                            notice.Details = System.Text.RegularExpressions.Regex.Replace(notice.Details, "<ahref", "<a href");
                        }

                        int notifyType = (int)Enum.Parse(typeof(NotifyType), notice.NewTaskArrivedNotifyType);
                        switch (notifyType)
                        {
                            case (int)NotifyType.Email:
                                {
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemNotificationTitle, notice.NotificationTitle);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailNotificationContent, notice.Details);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailPush, notice.WetherSend.ToString().ToLower());
                                    break;

                                }
                            case (int)NotifyType.App:
                                {
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemAppNotification, notice.NotificationTitle);//不确定名字是否正确
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemAppContent, notice.Details);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_AppPush, notice.WetherSend.ToString().ToLower());

                                    break;
                                }
                            case (int)NotifyType.WeChat:
                                {
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemWeChatTitle, notice.NotificationTitle);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemWeChatContent, notice.Details);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_WeChatPush, notice.WetherSend.ToString().ToLower());
                                    break;
                                }
                            case (int)NotifyType.DingTalk:
                                {
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemDDTitle, notice.NotificationTitle);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NewWorkItemDDContent, notice.Details);
                                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_DDPush, notice.WetherSend.ToString().ToLower());
                                    break;
                                }

                        }
                    }
                    #endregion

                    #region 催办任务到达提醒
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_UrgencyNotifyType, model.UrgencyNotice.NewTaskArrivedNotifyType);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_UrgencyNotificationTitle, model.UrgencyNotice.NotificationTitle);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailUrgencyContent, model.UrgencyNotice.Details);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NotifyUrgency, model.UrgencyNotice.WetherSend.ToString().ToLower());
                    #endregion

                    #region 定期发送未完成任务提醒
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_ReccurentReminderMethod, model.UnfinishedWorkItemNotice.NewTaskArrivedNotifyType);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindTime, model.UnfinishedWorkItemNotice.RemindTime);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_ReccurentReminderPattern, model.UnfinishedWorkItemNotice.Fequency);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailRemindTitle, model.UnfinishedWorkItemNotice.NotificationTitle);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailRemindContent, model.UnfinishedWorkItemNotice.Details);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_ReccurentReminderWorkingCalendar, model.UnfinishedWorkItemNotice.WorkCalendar);
                    string cycleType = string.Empty;
                    if (model.UnfinishedWorkItemNotice.CycleType != null)
                    {
                        foreach (string s in model.UnfinishedWorkItemNotice.CycleType)
                        {
                            cycleType += s + ";";
                        }
                        cycleType = cycleType.TrimEnd(';');
                        this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RecurrentReminderDayOfWeek, cycleType);
                    }

                    #endregion

                    #region 异常任务任务到达提醒
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_ExceptionNotifyType, model.ExceptionNotice.NewTaskArrivedNotifyType);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_ExceptionNotificationTitle, model.ExceptionNotice.NotificationTitle);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_EmailExceptionContent, model.ExceptionNotice.Details);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_NotifyException, model.ExceptionNotice.WetherSend.ToString().ToLower());
                    #endregion

                    #region 超时任务异常提醒
                    // 超时提醒策略1
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1NotifyType, model.OvertimePolicy1.NewTaskArrivedNotifyType);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1Title, model.OvertimePolicy1.NotificationTitle);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1Content, model.OvertimePolicy1.Details);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1PostInterval, model.OvertimePolicy1.TimeInterval);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy1PostAction, model.OvertimePolicy1.RemindPostAction);

                    // 超时提醒策略2
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2NotifyType, model.OvertimePolicy2.NewTaskArrivedNotifyType);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2Title, model.OvertimePolicy2.NotificationTitle);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2Content, model.OvertimePolicy2.Details);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2PostInterval, model.OvertimePolicy2.TimeInterval);
                    this.Engine.SettingManager.SetCustomSetting(CustomSetting.Setting_RemindPolicy2PostAction, model.OvertimePolicy2.RemindPostAction);
                    #endregion
                    result.Success = true;
                    result.Message = "msgGlobalString.SaveSucced";
                }
                catch (Exception e)
                {

                    result.Success = false;
                    result.Message = e.Message;
                }


                return Json(result, JsonRequestBehavior.AllowGet); ;
            });
        }

        /// <summary>
        /// 初始化发送方式
        /// </summary>
        /// <returns></returns>
        private List<Item> InitDeliveryMethods()
        {
            List<Item> items = new List<Item>();
            string[] notifyTypes = Enum.GetNames(typeof(NotifyType));
            items = notifyTypes.Select(s => new Item
            {
                Value = s,
                Text = s
            }).ToList();
            return items;
        }

        /// <summary>
        /// 初始化工作日历
        /// </summary>
        /// <returns></returns>
        private List<Item> InitWorkCalendar()
        {
            List<Item> list = new List<Item>();
            OThinker.H3.Calendar.WorkingCalendar[] workingCalendar = Engine.WorkingCalendarManager.GetCalendarList();
            list = workingCalendar.Select(s => new Item
            {
                Text = s.DisplayName,
                Value = s.ObjectID
            }).ToList();
            return list;
        }

        /// <summary>
        /// 初始化超时设置
        /// </summary>
        /// <returns></returns>
        private List<Item> InitRemindPostAction()
        {
            List<Item> items = new List<Item>();
            string[] names = Enum.GetNames(typeof(RemindPostAction));
            items = names.Select(s => new Item
            {
                Text = s,
                Value = s
            }).ToList();
            return items;
        }

        /// <summary>
        /// 获取超时提醒策略2
        /// </summary>
        /// <returns>超时提醒策略2</returns>
        private Notice GetOvertimePolicy2()
        {
            Notice notice = new Notice();
            notice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2Title);
            notice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2Content);
            notice.NewTaskArrivedNotifyType = CustomSetting.GetNotifyType(Engine.SettingManager, CustomSetting.Setting_RemindPolicy2NotifyType).ToString();
            notice.TimeInterval = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2PostInterval);
            notice.RemindPostAction = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy2PostAction) ?? InitRemindPostAction().FirstOrDefault().Value; ;
            return notice;
        }

        /// <summary>
        /// 获取超时提醒策略1
        /// </summary>
        /// <returns>超时提醒策略1</returns>
        private Notice GetOvertimePolicy1()
        {
            Notice notice = new Notice();
            notice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1Title);
            notice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1Content);
            notice.NewTaskArrivedNotifyType = CustomSetting.GetNotifyType(Engine.SettingManager, CustomSetting.Setting_RemindPolicy1NotifyType).ToString();
            notice.TimeInterval = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1PostInterval);
            notice.RemindPostAction = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindPolicy1PostAction) ?? InitRemindPostAction().FirstOrDefault().Value;
            return notice;
        }

        /// <summary>
        /// 获取异常提醒设置
        /// </summary>
        /// <returns>异常提醒设置</returns>
        private Notice GetExceptionNotice()
        {
            Notice notice = new Notice();
            notice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ExceptionNotificationTitle);
            notice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailExceptionContent);
            notice.WetherSend = GetBoolSetting(CustomSetting.Setting_NotifyException);
            notice.NewTaskArrivedNotifyType = CustomSetting.GetNotifyType(Engine.SettingManager, CustomSetting.Setting_ExceptionNotifyType).ToString();
            return notice;
        }

        /// <summary>
        /// 获取定期发送未完成任务提醒设置
        /// </summary>
        /// <returns>未完成任务提醒设置</returns>
        private Notice GetUnfinishedWorkItemNotice()
        {
            Notice notice = new Notice();
            notice.NewTaskArrivedNotifyType = Enum.GetName(typeof(NotifyType), 2);
            notice.RemindTime = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RemindTime);
            notice.Fequency = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ReccurentReminderPattern) + string.Empty;
            string clycleTypeStr = this.Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_RecurrentReminderDayOfWeek) + string.Empty;
            if (!string.IsNullOrEmpty(clycleTypeStr))
            {
                string[] clycleArr = clycleTypeStr.Split(';');
                notice.CycleType = clycleArr.ToList();
            }
            else
            {
                notice.CycleType = new List<string>();
            }
            notice.WorkCalendar = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_ReccurentReminderWorkingCalendar) + string.Empty;
            notice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailRemindTitle) + string.Empty;
            notice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailRemindContent) + string.Empty;
            return notice;
        }

        /// <summary>
        /// 获取催办任务提醒设置
        /// </summary>
        /// <returns>催办任务提醒设置</returns>
        private Notice GetUrgencyNotice()
        {
            Notice notice = new Notice();
            notice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_UrgencyNotificationTitle);
            notice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailUrgencyContent);
            notice.WetherSend = GetBoolSetting(CustomSetting.Setting_NotifyUrgency);
            notice.NewTaskArrivedNotifyType = CustomSetting.GetNotifyType(Engine.SettingManager, CustomSetting.Setting_UrgencyNotifyType).ToString();
            return notice;
        }

        /// <summary>
        /// 获取新任务到达提醒设置
        /// </summary>
        /// <returns>新任务到达提醒设置</returns>
        private List<Notice> GetNewWorkItemNotice()
        {
            List<Notice> list = new List<Notice>();

            //邮件提醒
            Notice emailNotice = new Notice();
            emailNotice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemNotificationTitle);
            emailNotice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_EmailNotificationContent);
            emailNotice.WetherSend = GetBoolSetting(CustomSetting.Setting_EmailPush);
            emailNotice.NewTaskArrivedNotifyType = Enum.GetName(typeof(NotifyType), NotifyType.Email);
            list.Add(emailNotice);
            //app提醒
            Notice appNotice = new Notice();
            appNotice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemAppNotification);
            appNotice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemAppContent);
            appNotice.WetherSend = GetBoolSetting(CustomSetting.Setting_AppPush);
            appNotice.NewTaskArrivedNotifyType = Enum.GetName(typeof(NotifyType), NotifyType.App); ;
            list.Add(appNotice);
            //微信提醒
            Notice weChatNotice = new Notice();
            weChatNotice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemWeChatTitle);
            weChatNotice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemWeChatContent);
            weChatNotice.WetherSend = GetBoolSetting(CustomSetting.Setting_WeChatPush);
            weChatNotice.NewTaskArrivedNotifyType = Enum.GetName(typeof(NotifyType), NotifyType.WeChat);
            list.Add(weChatNotice);

            //钉钉提醒
            Notice ddNotice = new Notice();
            ddNotice.NotificationTitle = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemDDTitle);
            ddNotice.Details = Engine.SettingManager.GetCustomSetting(CustomSetting.Setting_NewWorkItemDDContent);
            ddNotice.WetherSend = GetBoolSetting(CustomSetting.Setting_DDPush);
            ddNotice.NewTaskArrivedNotifyType = Enum.GetName(typeof(NotifyType), NotifyType.DingTalk);
            list.Add(ddNotice);
            return list;
        }

        /// <summary>
        /// 获取设置的值
        /// </summary>
        /// <param name="SettingName">设置名称</param>
        /// <returns>设置的值</returns>
        private bool GetBoolSetting(string SettingName)
        {
            return CustomSetting.GetValue<bool>(Engine.SettingManager, SettingName, false);
        }

    }
}
