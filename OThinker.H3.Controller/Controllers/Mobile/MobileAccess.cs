using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using OThinker.Organization;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.Instance;
using System.Web.Mvc;
using OThinker.H3.Site;
using System.Collections;
using OThinker.H3.DataModel;
using OThinker.H3.BizBus.Filter;
using System.Text;
using OThinker.Data.Database;
using OThinker.H3.WorkflowTemplate;

namespace OThinker.H3.Controllers
{
    public class MobileAccess : ControllerBase
    {

        public override string FunctionCode
        {
            get { return ""; }
        }
        public MobileAccess()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }

        public string Test()
        {
            return "SUCCESS";
        }

        private UserValidator _CurrentUser;
        /// <summary>
        /// 当前用户
        /// </summary>
        public UserValidator CurrentUser
        {
            get
            {
                if (System.Web.HttpContext.Current.Session[Sessions.GetUserValidator()] != null)
                {
                    _CurrentUser = System.Web.HttpContext.Current.Session[Sessions.GetUserValidator()] as UserValidator;
                }
                return _CurrentUser;
            }
            set
            {
                _CurrentUser = value;
                Session[Sessions.GetUserValidator()] = _CurrentUser;
            }
        }
        protected void OnLoad(EventArgs e)
        {
            string wechatCode = Request.QueryString["code"] + string.Empty;
            string engineCode = Request.QueryString["state"] + string.Empty;

            if (!string.IsNullOrEmpty(wechatCode)
                && !string.IsNullOrEmpty(engineCode)
                && Session[Sessions.GetUserValidator()] == null)
            {
                UserValidatorFactory.LoginAsWeChat(engineCode, wechatCode);
            }

            if (CurrentUser == null && !Request.RawUrl.ToLower().Contains("login"))
            {
                //Response.Redirect("Login.aspx");
            }
        }

        public Object GetBadgeNum(string userId, string mobileToken)
        {
            int WorkItemBadge = 0;
            int CirculateItemBadge = 0;
            WorkItemBadge = GetAllWorkItems(userId);
            CirculateItemBadge = GetAllCirculateItems(userId).Length;
            var result = new
            {
                WorkItemBadge = WorkItemBadge,
                CirculateItemBadge = CirculateItemBadge,
            };
            return result;
        }

        #region 待办任务
        /// <summary>
        /// 加载用户的待办任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="sortKey"></param>
        /// <param name="sortDirection"></param>
        /// <param name="finishedWorkItem"></param>
        /// <returns></returns>
        public Object LoadWorkItems(string userId, string mobileToken, string keyWord,
                DateTime lastTime,
                string sortKey,
                string sortDirection,
                bool finishedWorkItem)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            DateTime refreshTime = lastTime;
            List<string> Conditions = new List<string>();
            string wrokItemTableName = Engine.Query.GetItemTableName(finishedWorkItem ? WorkItem.WorkItemState.Finished : WorkItem.WorkItemState.Unfinished);

            string orderBy = string.Empty;
            if (finishedWorkItem)
            {// 兼容旧版本考虑
                sortKey = sortKey.ToLower().Replace(WorkItem.WorkItem.TableName.ToLower() + ".", WorkItem.WorkItemFinished.TableName.ToLower() + ".");
            }
            Conditions.Add(sortKey + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));

            if (finishedWorkItem)
            {// 已办按照完成时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC ";
                Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=2 OR " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=3)");
            }
            else
            {// 待办按照接收时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC ";
                Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=0 OR " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=1)");
            }
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.WorkItem.PropertyName_Participant + "='" + userId + "' OR " + WorkItem.WorkItem.PropertyName_Delegant + "='" + userId + "')");
            // 允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            if (keyWord.Trim() != string.Empty)
            {
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'" + ")");
            }
            // 每次只返回10条
            dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1, 10, orderBy, wrokItemTableName);
            if (dtWorkItems == null) return null;

            int workItemCount = 0;
            if (!finishedWorkItem)
            {// 待办才显示数量
                Conditions.Remove(Conditions[0]); // 汇总计算移除时间条件
                workItemCount = Engine.Query.CountWorkItem(Conditions.ToArray(), keyWord.Trim() != string.Empty);
            }

            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();
            }

            List<MobileWorkItem> workItems = new List<MobileWorkItem>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                workItems.Add(GetMobileWorkItemFromRow(units, row));
            }

            if (dtWorkItems.Rows.Count > 0)
            {
                if (finishedWorkItem)
                {
                    lastTime = DateTime.Parse(dtWorkItems.Rows[workItems.Count - 1][WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty);
                }
                else
                {
                    lastTime = DateTime.Parse(dtWorkItems.Rows[workItems.Count - 1][WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty);
                }
                refreshTime = DateTime.Parse(dtWorkItems.Rows[0][WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty);
            }

            var result = new
            {
                LoadComplete = dtWorkItems.Rows.Count < 10, // 是否加载完成
                LastTime = lastTime,                        // 最后加载时间
                RefreshTime =   refreshTime,                  // 刷新时需要使用的时间
                WorkItems = workItems,                      // 工作任务内容
                TotalCount = workItemCount                  // 记录总数
            };
            return result;
        }

        /// <summary>
        /// 获取所有的任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="sortKey"></param>
        /// <param name="sortDirection"></param>
        /// <param name="finishedWorkItem"></param>
        /// <param name="priortyType">流程优先级</param>
        /// <returns></returns>
        public LoadWorkItemsClass GetLoadWorkItems(WorkItemQueryParams Params)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            List<string> Conditions = new List<string>();
            string wrokItemTableName = Engine.Query.GetItemTableName(Params.finishedWorkItem ? WorkItem.WorkItemState.Finished : WorkItem.WorkItemState.Unfinished);
            string orderBy = string.Empty;
            if (!string.IsNullOrEmpty(Params.sortKey))
            {
                if (Params.finishedWorkItem)
                {// 兼容旧版本考虑
                    Params.sortKey = Params.sortKey.ToLower().Replace(WorkItem.WorkItem.TableName.ToLower() + ".", WorkItem.WorkItemFinished.TableName.ToLower() + ".");
                }
                //Conditions.Add(Params.sortKey + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, (DateTime)Params.lastTime));
            }
            if (Params.finishedWorkItem)
            {// 已办按照完成时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC ";
                //Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=2 OR " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=3)");
                //不允许已取消的状态出现
                //Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "!='" + (int)InstanceState.Canceled + "'");
                //在已办中只展示允许状态数据
                Conditions.Add(WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "='" + (int)InstanceState.Running + "'");
            }
            else
            {// 待办按照优先级,接收时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC ";
                Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=0 OR " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=1)");
            }
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.WorkItem.PropertyName_Participant + "='" + Params.userId + "' OR " + WorkItem.WorkItem.PropertyName_Delegant + "='" + Params.userId + "')");
            // 允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");

            //关键字搜索(流程实例名称，流水号，发起人姓名)
            if (Params.keyWord != null && Params.keyWord.Trim() != string.Empty)
            {
                //Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'" + " OR " + Instance.InstanceContext.PropertyName_OriginatorName + " like '%" + keyWord + "%'" + ")");
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + Params.keyWord + "%')");
            }

            //时间条件
            if (Params.startDate.HasValue)
            {
                Conditions.Add(WorkItem.CirculateItem.PropertyName_ReceiveTime + ">= '" + string.Format("{0} 00:00:00", Params.startDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            if (Params.endDate.HasValue)
            {
                Conditions.Add(WorkItem.CirculateItem.PropertyName_ReceiveTime + "<= '" + string.Format("{0} 23:59:59", Params.endDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            //状态条件
            if (Params.instanceState.HasValue)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "=" + Params.instanceState);
            }
            if (Params.IsPriority.HasValue)
            {
                if (Params.IsPriority == (int)PriorityType.High)
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "=" + (int)PriorityType.High);
                }
                else
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "<" + (int)PriorityType.High);
                }
            }
            if (Params.Originators != null && Params.Originators.Length > 0)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Originator + " in ('" + string.Join("','", Params.Originators) + "')");
            }
            // 每次只返回10条
            dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), Params.loadStart + 1, Params.returnCount, orderBy, wrokItemTableName);
            if (dtWorkItems == null) return null;

            int workItemCount = 0;
            if (!Params.finishedWorkItem)
            {// 待办才显示数量
                //Conditions.Remove(Conditions[0]); // 汇总计算移除时间条件
                workItemCount = Engine.Query.CountWorkItem(Conditions.ToArray(), string.IsNullOrWhiteSpace(Params.keyWord));
            }

            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();
                List<string> OriginatorOUs = new List<string>();
                foreach (Unit unit in units)
                {
                    OriginatorOUs.Add(unit.ParentID + string.Empty);
                }
                Unit[] OriginatorOUUnits = this.Engine.Organization.GetUnits(OriginatorOUs.ToArray()).ToArray();
                units = units.Union(OriginatorOUUnits).ToArray();
            }

            List<MobileWorkItem> workItems = new List<MobileWorkItem>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                workItems.Add(GetMobileWorkItemFromRow(units, row));
            }
            //SetSummary(workItems, true);
            var result = new LoadWorkItemsClass()
            {
                LoadComplete = dtWorkItems.Rows.Count < 10, // 是否加载完成
                //LastTime = (DateTime)Params.lastTime,       // 最后加载时间
                //RefreshTime = refreshTime,                  // 刷新时需要使用的时间
                WorkItems = workItems,                      // 工作任务内容
                TotalCount = workItemCount                  // 记录总数
            };
            return result;
        }

        /// <summary>
        /// 设置摘要
        /// </summary>
        /// <param name="workItemId"></param>
        //public void SetSummary(List<MobileWorkItem> workItemId, bool isWorkItem)
        //{
        //    String DisplayName, Value, WorkflowCode;
        //    int WorkflowVersion;
        //    //List<Dictionary<String, String>> Sheetfields = new List<Dictionary<string, string>>();
        //    //string workItemId = "";
        //    for (int i = 0; i < workItemId.Count; i++)
        //    {
        //        List<WorkItemSummary> Sheetfield = new List<WorkItemSummary>();
        //        var item = workItemId[i];
        //        if (isWorkItem)
        //        {
        //            var workItem = this.Engine.WorkItemManager.GetWorkItem(item.ObjectID);
        //            WorkflowCode = workItem.WorkflowCode;
        //            WorkflowVersion = workItem.WorkflowVersion;
        //        }
        //        else
        //        {
        //            var CirculateItem = this.Engine.WorkItemManager.GetCirculateItem(item.ObjectID);
        //            WorkflowCode = CirculateItem.WorkflowCode;
        //            WorkflowVersion = CirculateItem.WorkflowVersion;
        //        }
        //        var template = this.Engine.WorkflowManager.GetPublishedTemplate(WorkflowCode, WorkflowVersion);
        //        var scheme = this.Engine.BizObjectManager.GetPublishedSchema(template.BizObjectSchemaCode);
        //        BizObject Bo = new BizObject(this.Engine, scheme, item.Originator);
        //        Bo.ObjectID = item.BizObjectID;
        //        Bo.Load();
        //        //Bo.Invoke(BizObjectSchema.MethodName_GetList);
        //        foreach (var field in (template.FlowSummary ?? String.Empty).Replace("{", "").Split('}'))
        //        {
        //            if (!String.IsNullOrWhiteSpace(field))
        //            {
        //                try
        //                {
        //                    DisplayName = scheme.GetField(field) != null ? scheme.GetField(field).DisplayName : string.Empty;
        //                    Value = Bo.GetValue(field) == null ? string.Empty : Bo.GetValue(field).ToString();
        //                    Sheetfield.Add(new WorkItemSummary()
        //                    {
        //                        DisplayName = DisplayName,
        //                        Value = Value
        //                    });
        //                }
        //                catch (Exception ex)
        //                {
        //                    this.Engine.LogWriter.Write(ex.Message);
        //                }
        //            }
        //        }
        //        workItemId[i].Summary = Sheetfield;
        //    }
        //}

        private List<WorkItemSummary> ConvertSummary(string summary)
        {
            if (string.IsNullOrEmpty(summary))
                return null;
            List<WorkItemSummary> itemSummary = new List<WorkItemSummary>();
            try
            {
                var dicSummary = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(summary);
                foreach (var item in dicSummary)
                {
                    itemSummary.Add(new WorkItemSummary { DisplayName = item.Key, Value = ConvertTimeFormat(item.Value, "yyyy-MM-dd hh:mm:ss") });
                }
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write("WorkItemSummary JSON deserizlize failed:" + ex.Message);
            }
            return itemSummary;
        }

        /// <summary>
        /// 刷新用户的待办任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        /// <param name="finishedWorkItem">是否加载已办任务</param>
        /// <param name="existsLength">返回待办任务ID的数量</param>
        public Object RefreshWorkItem(string userId,
            string mobileToken,
            string keyWord,
            DateTime lastTime,
            bool finishedWorkItem,
            int existsLength)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差
            string orderBy = string.Empty;
            List<string> Conditions = new List<string>();

            string wrokItemTableName = Engine.Query.GetItemTableName(finishedWorkItem ? WorkItem.WorkItemState.Finished : WorkItem.WorkItemState.Unfinished);

            if (finishedWorkItem)
            {// 已办按照完成时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC ";
                Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=2 OR " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=3)");
            }
            else
            {// 待办按照接收时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC ";
                Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=0 OR " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=1)");
            }
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.WorkItem.PropertyName_Participant + "='" + userId + "' OR " + WorkItem.WorkItem.PropertyName_Delegant + "='" + userId + "')");
            //允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            //关键字搜索(流程实例名称，流水号，发起人姓名)
            if (keyWord.Trim() != string.Empty)
            {
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'" + ") OR " + Instance.InstanceContext.PropertyName_OriginatorName + " like '%" + keyWord + "%'" + ")");
            }
            // 每次只返回10条
            dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1
                , 10, orderBy, wrokItemTableName);
            if (dtWorkItems == null) return null;
            int workItemCount = -1;
            Conditions.Remove(Conditions[0]);  // 汇总计算移除时间条件
            if (dtWorkItems.Rows.Count > 0 && !finishedWorkItem)
            {
                workItemCount = Engine.Query.CountWorkItem(Conditions.ToArray(), keyWord.Trim() != string.Empty);
            }

            List<MobileWorkItem> workItems = new List<MobileWorkItem>();
            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();
            }

            foreach (DataRow row in dtWorkItems.Rows)
            {
                workItems.Add(GetMobileWorkItemFromRow(units, row));
            }
            if (dtWorkItems.Rows.Count > 0)
            {
                lastTime = DateTime.Parse(dtWorkItems.Rows[0][WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty);
            }

            // 加载未完成的待办，因为PC端完成了，移动端可能没有刷新，这里返回所有待办中的任务ID，这里要改成查询 ObjectID
            List<string> existsIds = new List<string>();
            if (existsLength > 0)
            {
                Conditions.Add(WorkItem.WorkItem.PropertyName_ReceiveTime + "<=" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));

                dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1, existsLength + 10, orderBy, wrokItemTableName);
                foreach (DataRow row in dtWorkItems.Rows)
                {
                    existsIds.Add(row["ObjectID"] + string.Empty);
                }
            }

            var result = new
            {
                LastTime = lastTime,                        // 最后加载时间
                WorkItems = workItems,                      // 工作任务内容
                TotalCount = workItemCount,                 // 记录总数
                ExistsIds = existsIds.ToArray()             // 未办完的待办任务ID集合
            };
            return result;
        }

        /// <summary>
        /// 刷新用户的待办任务
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        /// <param name="finishedWorkItem">是否加载已办任务</param>
        /// <param name="existsLength">返回待办任务ID的数量</param>
        /// <param name="priorityType">流程优先级</param>
        public RefreshWorkItemsClass GetRefreshWorkItemsFn(string userId,
            string mobileToken,
            string keyWord,
            DateTime lastTime,
            bool finishedWorkItem,
            int existsLength, OThinker.H3.Instance.PriorityType priorityType, DateTime? startDate, DateTime? endDate, int returnCount)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差
            string orderBy = string.Empty;
            List<string> Conditions = new List<string>();

            string wrokItemTableName = Engine.Query.GetItemTableName(finishedWorkItem ? WorkItem.WorkItemState.Finished : WorkItem.WorkItemState.Unfinished);

            if (finishedWorkItem)
            {// 已办按照完成时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_FinishTime + " DESC ";
                //Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=2 OR " + wrokItemTableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "=3)");

                //不允许已取消的状态出现
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "!='" + (int)InstanceState.Canceled + "'");
                //在已办中只展示允许状态数据
                Conditions.Add(WorkItem.WorkItemFinished.TableName + "." + WorkItem.WorkItemFinished.PropertyName_State + "='" + (int)InstanceState.Running + "'");
            }
            else
            {// 待办按照接收时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC ";
                Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=0 OR " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_State + "=1)");
            }
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.WorkItem.PropertyName_Participant + "='" + userId + "' OR " + WorkItem.WorkItem.PropertyName_Delegant + "='" + userId + "')");
            //允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            //优先级
            if (priorityType != Instance.PriorityType.Unspecified && priorityType == Instance.PriorityType.High)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "=" + (int)Instance.PriorityType.High);
            }
            else if (priorityType != Instance.PriorityType.Unspecified && priorityType != Instance.PriorityType.High)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "!=" + (int)Instance.PriorityType.High);
            }
            //关键字搜索(流程实例名称，流水号，发起人姓名)
            if (keyWord != null && keyWord.Trim() != string.Empty)
            {
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'" + ") OR " + Instance.InstanceContext.PropertyName_OriginatorName + " like '%" + keyWord + "%'" + ")");
            }
            // 每次只返回10条
            dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1, returnCount, orderBy, wrokItemTableName);
            int totalCount = 0;
            if (dtWorkItems == null)
            {
                return null;
            }
            //int workItemCount = -1;
            //Conditions.Remove(Conditions[0]);  // 汇总计算移除时间条件
            if (dtWorkItems.Rows.Count > 0 && !finishedWorkItem)
            {
                totalCount = Engine.Query.CountWorkItem(Conditions.ToArray(), keyWord.Trim() != string.Empty);
            }

            List<MobileWorkItem> workItems = new List<MobileWorkItem>();
            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();

                List<string> OriginatorOUs = new List<string>();
                foreach (Unit unit in units)
                {
                    OriginatorOUs.Add(unit.ParentID + string.Empty);
                }
                Unit[] OriginatorOUUnits = this.Engine.Organization.GetUnits(OriginatorOUs.ToArray()).ToArray();
                units = units.Union(OriginatorOUUnits).ToArray();
            }

            foreach (DataRow row in dtWorkItems.Rows)
            {
                workItems.Add(GetMobileWorkItemFromRow(units, row));
            }
            if (dtWorkItems.Rows.Count > 0)
            {
                //lastTime = DateTime.Parse(dtWorkItems.Rows[0][WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty);
                //最后一次活动时间
                lastTime = DateTime.Parse(dtWorkItems.Rows[0][Instance.InstanceContext.PropertyName_LastActiveTime] + string.Empty);
            }

            // 加载未完成的待办，因为PC端完成了，移动端可能没有刷新，这里返回所有待办中的任务ID，这里要改成查询 ObjectID
            List<string> existsIds = new List<string>();
            if (existsLength > 0)
            {
                Conditions.Add(WorkItem.WorkItem.PropertyName_ReceiveTime + "<=" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));

                dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1, existsLength + 10, orderBy, wrokItemTableName);
                foreach (DataRow row in dtWorkItems.Rows)
                {
                    existsIds.Add(row["ObjectID"] + string.Empty);
                }
            }

            var result = new RefreshWorkItemsClass
            {
                LastTime = lastTime,                        // 最后加载时间
                WorkItems = workItems,                      // 工作任务内容
                TotalCount = totalCount,                 // 记录总数
                ExistsIds = existsIds.ToArray()             // 未办完的待办任务ID集合
            };

            return result;
        }

        #endregion

        #region 流程实例
        /// <summary>
        /// 获取用户的在办流程实例
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">搜索关键字</param>
        /// <param name="lastTime">最后时间</param>
        /// <param name="sortKey">排序字段</param>
        /// <param name="sortDirection">排序方式</param>
        public Object LoadInstances(string userId,
            string mobileToken,
            string keyWord,
            DateTime lastTime,
            string sortKey,
            string sortDirection)
        {
            DataTable dtInstances = null;
            DateTime refreshTime = lastTime;
            List<string> Conditions = new List<string>();

            Conditions.Add(sortKey + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));

            // 流程状态小于4
            Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "<4");
            // 发起人条件
            Conditions.Add(Instance.InstanceContext.PropertyName_Originator + "='" + userId + "'");
            // 过滤条件
            if (keyWord.Trim() != string.Empty)
            {
                Conditions.Add(Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'");
            }
            // 每次只返回10条
            dtInstances = Engine.Query.QueryInstance(Conditions.ToArray(), 1, 10,
                sortKey,
                sortDirection.ToLower().IndexOf("desc") == -1);
            if (dtInstances == null) return null;

            // 加载数据
            List<MobileInstance> instances = new List<MobileInstance>();
            foreach (DataRow row in dtInstances.Rows)
            {
                instances.Add(new MobileInstance()
                {
                    CreatedTime = Convert.ToDateTime(row[Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty).ToString("yyyy-MM-dd"),
                    InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty,
                    ObjectID = row[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty
                });
            }

            if (dtInstances.Rows.Count > 0)
            {
                lastTime = DateTime.Parse(dtInstances.Rows[instances.Count - 1][Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty);
                refreshTime = DateTime.Parse(dtInstances.Rows[0][Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty);
            }

            var result = new
            {
                LoadComplete = dtInstances.Rows.Count < 10, // 是否加载完成
                LastTime = lastTime,                        // 最后加载时间
                RefreshTime = refreshTime,                  // 刷新时需要使用的时间
                Instances = instances                       // 工作任务内容
            };
            return result;
        }

        /// <summary>
        /// 刷新用户的在办
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        public Object RefreshInstances(string userId, string mobileToken, string keyWord, DateTime lastTime)
        {
            DataTable dtInstances = null;
            lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差

            List<string> Conditions = new List<string>();
            // 时间条件
            Conditions.Add(Instance.InstanceContext.PropertyName_CreatedTime + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));
            // 流程状态小于4
            Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "<4");
            // 发起人条件
            Conditions.Add(Instance.InstanceContext.PropertyName_Originator + "='" + userId + "'");
            // 过滤条件
            if (keyWord.Trim() != string.Empty)
            {
                Conditions.Add(Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'");
            }

            // 每次只返回10条
            dtInstances = Engine.Query.QueryInstance(Conditions.ToArray(),
                1,
                10,
                Instance.InstanceContext.PropertyName_CreatedTime,
                false);

            // 加载数据
            List<MobileInstance> instances = new List<MobileInstance>();
            foreach (DataRow row in dtInstances.Rows)
            {
                instances.Add(new MobileInstance()
                {
                    CreatedTime = Convert.ToDateTime(row[Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty).ToString("yyyy-MM-dd"),
                    InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty,
                    ObjectID = row[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty
                });
            }

            if (dtInstances.Rows.Count > 0)
            {
                lastTime = DateTime.Parse(dtInstances.Rows[instances.Count - 1][Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty);
            }

            var result = new
            {
                LastTime = lastTime,                        // 最后加载时间
                Instances = instances                       // 工作任务内容
            };
            return result;
        }

        #endregion

        #region 流程状态
        /// <summary>
        /// 获取流程状态信息
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="instanceId"></param>
        /// <param name="UserValidator"></param>
        /// <returns></returns>
        public Object LoadInstanceState(string userId, string mobileToken, string instanceId, UserValidator UserValidator,string workItemId)
        {
            Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(instanceId);
            if (InstanceContext != null)
            {
                List<string> Approvers = new List<string>();
                //List<string> ActiveStatu = new List<string>();
                if (InstanceContext.State == Instance.InstanceState.Running)
                {
                    WorkflowTemplate.PublishedWorkflowTemplate WorkflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplate(InstanceContext.WorkflowCode, InstanceContext.WorkflowVersion);
                    DataTable participantRows = this.Engine.PortalQuery.GetActiveNamebyInstanceId(new List<string>() { instanceId });

                    foreach (DataRow row in participantRows.Rows)
                    {
                        var Activity = WorkflowTemplate.GetActivityByCode(row[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty);
                        if (Activity != null && (Activity.ActivityType == OThinker.H3.WorkflowTemplate.ActivityType.Approve || Activity.ActivityType == OThinker.H3.WorkflowTemplate.ActivityType.Circulate))
                        {
                            //Approvers += row[WorkItem.WorkItem.PropertyName_ParticipantName] + ",";
                            Approvers.Add(row[WorkItem.WorkItem.PropertyName_ParticipantName] + string.Empty);
                        }
                    }
                }
                //else
                //{
                //    State = OThinker.H3.Settings.CustomSetting.GetStateName(UserValidator.PortalSettings,
                //                InstanceContext.State, InstanceContext.Approval);
                //}
                var BaseInfo = new
                {
                    Name = InstanceContext.InstanceName,
                    OriginatorName = InstanceContext.OriginatorName,
                    SequenceNo = InstanceContext.SequenceNo,
                    State = InstanceContext.State,
                    WorkflowFullName = this.Engine.WorkflowManager.GetTemplateDisplayName(
                        InstanceContext.WorkflowCode,
                        InstanceContext.WorkflowVersion),
                    StartDate = InstanceContext.StartTime.ToShortDateString(),
                    FinishDate = (InstanceContext.State == Instance.InstanceState.Finished) ? InstanceContext.FinishTime.ToShortDateString() : string.Empty,
                    ParticipantImageURL = GetUserImageURL(InstanceContext.Originator),
                    Approvers = Approvers
                };
                // TODO:这里代码重复调用远程接口问题再检查下
                // TODO:如果是子流程，在主流程的流程状态就只显示进行中，省去一次查询数据库的请求
                var InstanceLogInfo = this.GetInstanceLogInfo(userId, InstanceContext, workItemId);
                var result = new
                {
                    SUCCESS = true,
                    BaseInfo = BaseInfo,
                    InstanceLogInfo = InstanceLogInfo
                };
                return result;
            }
            else
            {
                var result = new
                {
                    SUCCESS = false
                };
                return result;
            }
        }

        /// <summary>
        /// 获取流程的日志信息
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        private List<InstanceLogInfo> GetInstanceLogInfo(string userid, InstanceContext instance,string workItemId)
        {
            Instance.InstanceContext InstanceContext = instance;
            DataTable TokenTable = InstanceContext.GetTokenTable();
            DataTable ItemTable = this.GetItemTable(InstanceContext.InstanceId);
            // 查询子流程
            //DataTable ChildInstanceTable = this.Engine.Query.QueryChildInstance(InstanceID,
            //    Instance.Token.UnspecifiedID,
            //    Instance.InstanceState.Unspecified,
            //    OThinker.Data.BoolMatchValue.Unspecified);
            //获取所有的日志信息
            IEnumerable<LogInfo> Items = this.GetItemTable(TokenTable, ItemTable);

            //数据处理返回到前端
            List<InstanceLogInfo> result = new List<InstanceLogInfo>();

            WorkflowTemplate.PublishedWorkflowTemplate WorkflowTemplate = this.Engine.WorkflowManager.GetPublishedTemplate(InstanceContext.WorkflowCode, InstanceContext.WorkflowVersion);

            List<DataItemPermission> viewDisablePermissions = new List<DataItemPermission>();
            if (!string.IsNullOrEmpty(workItemId))
            {
                //获取前端流程的数据权限
                var fromWorkItem = Items.Where(p => p.WorkItemID == workItemId).FirstOrDefault();
                WorkflowTemplate.Activity fromActivity = WorkflowTemplate.GetActivityByCode(fromWorkItem.ActivityCode);
                if (fromActivity is ParticipativeActivity)
                {
                    viewDisablePermissions = ((ParticipativeActivity)fromActivity).DataPermissions.Where(p => !p.MobileVisible).ToList();
                }
            }
            foreach (var item in Items)
            {
                //活动显示名称
                WorkflowTemplate.Activity Activity = WorkflowTemplate.GetActivityByCode(item.ActivityCode);
                string ActivityName = Activity.DisplayName + string.Empty;
                string Comment = string.Empty;
                string SignatureId = string.Empty;

                // 设置完成时间是否显示
                //string finishedTime = item.FinishedTime + string.Empty;
                //if (finishedTime.StartsWith("1753") || finishedTime.StartsWith("9999") || finishedTime.StartsWith("000"))
                //    finishedTime = string.Empty;

                //获取组织对象的显示名称集合
                string[] columns = new string[] { WorkItem.WorkItem.PropertyName_Delegant, WorkItem.WorkItem.PropertyName_Creator };
                Dictionary<string, string> unitNames = this.GetUnitNamesFromTable(ItemTable, columns);
                if (Activity.ActivityType == OThinker.H3.WorkflowTemplate.ActivityType.Approve)
                {
                    //获取审批意见
                    String CommentName = String.Empty;
                    if (item.ItemType == ((int)WorkItem.WorkItemType.WorkItemConsult).ToString() ||
                        item.ItemType == ((int)WorkItem.WorkItemType.ActivityConsult).ToString() ||
                        item.ItemType == ((int)WorkItem.WorkItemType.Consult).ToString())
                    {
                        CommentName = MvcController.SheetConsultComment;
                        if (item.Participant != userid && item.Creator != userid)
                            CommentName = string.Empty;
                    }
                    else
                    {
                        CommentName = WorkflowTemplate.GetDefaultCommentDataItem(this.Engine.BizObjectManager.GetPublishedSchema(InstanceContext.BizObjectSchemaCode), item.ActivityCode);
                    }

                    //是否可见
                    var viewVisable = true;

                    //检查是否有手机不显示字段
                    if (viewDisablePermissions.Any())
                    {
                        var permission = viewDisablePermissions.Where(p => p.ItemName == CommentName).FirstOrDefault();
                        viewVisable = permission == null ? true : false;
                    }

                    if (viewVisable)
                    {
                        var CommentValues = Engine.BizObjectManager.GetCommentsByBizObject(InstanceContext.BizObjectSchemaCode, InstanceContext.BizObjectId, CommentName);
                        if (CommentValues != null && CommentValues.Length > 0 && CommentValues.FirstOrDefault(o => o.UserID == item.Participant && o.TokenId == item.TokenID) != null)
                        {
                            Comment = CommentValues.FirstOrDefault(o => o.UserID == item.Participant && o.TokenId == item.TokenID).Text;
                            SignatureId = CommentValues.FirstOrDefault(o => o.UserID == item.Participant && o.TokenId == item.TokenID).SignatureId;
                        }
                    }
                }
                string approvalName = "";

                //其他（征询，协办。。。）
                string creator = string.Empty;
                string creatorName = string.Empty;
                string userImg = string.Empty;
                try
                {
                    approvalName = GetStatusName(int.Parse(item.State), item.Approval);
                }
                catch
                {
                    approvalName = GetApprovalState(item.Approval);
                }
                if (item.State != "")
                {
                    userImg = GetUserImageURL(item.Participant);
                }
                else
                {
                    approvalName = "";
                }

                if (item.ItemType != string.Empty)
                {
                    // 类型
                    WorkItem.WorkItemType itemType = (WorkItem.WorkItemType)OThinker.Data.Convertor.Convert<int>(item.ItemType);
                    string action = this.GetItemAction(itemType);
                    if (!(string.IsNullOrEmpty(item.Creator) || item.Creator == item.Delegant || item.Creator == item.Participant))
                    {
                        creator = item.Creator + string.Empty;
                        creatorName = "(" + this.GetValueFromDictionary(unitNames, creator) + "的" + this.GetItemAction(itemType) + ")";
                    }
                }
                result.Add(new InstanceLogInfo
                {
                    ActivityCode = item.ActivityCode,
                    ActivityName = ActivityName,
                    CreatedTime = ConvertTimeFormat(item.CreatedTime.ToString(), "MM-dd HH:mm"),
                    FinishedTime = item.State == "0" ? "" : ConvertTimeFormat(item.FinishedTime.ToString(), "MM-dd HH:mm"),
                    ParticipantName = string.IsNullOrEmpty(item.ParticipantName.Trim()) ? ActivityName : item.ParticipantName.Trim(),
                    ParticipantOuName = Engine.Organization.GetName(item.OrgUnit),
                    ParticipantImageURL = userImg,
                    ApprovalName = approvalName,
                    Approval = item.Approval,
                    Comments = Comment,
                    SignatureId = SignatureId,
                    Status = item.State,
                    CreatorName = creatorName
                });
            }
            return result;
        }

        /// <summary>
        /// 获取操作描述名称
        /// </summary>
        /// <param name="itemType"></param>
        /// <returns></returns>
        private string GetItemAction(WorkItem.WorkItemType itemType)
        {
            string itemTypeAction = null;
            switch (itemType)
            {
                case WorkItem.WorkItemType.Circulate:
                    itemTypeAction = "传阅";//InstanceToken_Circulated;
                    break;
                case WorkItem.WorkItemType.WorkItemAssist:
                case WorkItem.WorkItemType.ActivityAssist:
                    itemTypeAction = "协办";//InstanceToken_Assist;
                    break;
                case WorkItem.WorkItemType.WorkItemConsult:
                case WorkItem.WorkItemType.ActivityConsult:
                    itemTypeAction = "征询";//InstanceToken_Consult;
                    break;
                case WorkItem.WorkItemType.ActivityForward:
                    itemTypeAction = "转办";//InstanceToken_Forward;
                    break;
                case WorkItem.WorkItemType.Approve:
                case WorkItem.WorkItemType.Assistive:
                case WorkItem.WorkItemType.Consult:
                case WorkItem.WorkItemType.Fill:
                case WorkItem.WorkItemType.Read:
                    break;
                default:
                    throw new NotImplementedException();
            }
            return itemTypeAction;
        }

        /// <summary>
        /// 获取流程日志的Item表
        /// </summary>
        /// <param name="InstanceID"></param>
        /// <returns></returns>
        private DataTable GetItemTable(string InstanceID)
        {
            //获取流程实例所有ItemTable
            DataTable WorkItemTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.WorkItem.TableName);
            DataTable WorkItemFinishedTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.WorkItemFinished.TableName);
            DataTable CirculateItemTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.CirculateItem.TableName);
            DataTable CirculateItemFinishedTable = this.Engine.PortalQuery.QueryWorkItem(WorkflowTemplate.WorkflowDocument.NullWorkflowVersion,
                new string[] { InstanceID }, WorkItem.WorkItemState.Unspecified, WorkItem.CirculateItemFinished.TableName);

            //合并Table   
            string[] columns = new string[] { Instance.Token.PropertyName_TokenId, WorkItem.WorkItem.PropertyName_ObjectID, WorkItem.WorkItem.PropertyName_Participant, WorkItem.WorkItem.PropertyName_ParticipantName, WorkItem.WorkItem.PropertyName_ActivityCode, WorkItem.WorkItem.PropertyName_DisplayName, WorkItem.WorkItem.PropertyName_ReceiveTime, WorkItem.WorkItem.PropertyName_FinishTime, WorkItem.WorkItem.PropertyName_UsedTime, WorkItem.WorkItem.PropertyName_OrgUnit, WorkItem.WorkItem.PropertyName_Approval, WorkItem.WorkItem.PropertyName_Delegant, WorkItem.WorkItem.PropertyName_Finisher, WorkItem.WorkItem.PropertyName_FinisherName, WorkItem.WorkItem.PropertyName_ItemType, WorkItem.WorkItem.PropertyName_Creator, WorkItem.WorkItem.PropertyName_State, WorkItem.WorkItem.PropertyName_WorkflowCode };

            DataTable dt = new DataTable();
            foreach (string col in columns)
            {
                dt.Columns.Add(col);
            }
            foreach (DataRow row in WorkItemTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    dr[col] = row[col];
                }
                dt.Rows.Add(dr);
            }
            foreach (DataRow row in WorkItemFinishedTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    dr[col] = row[col];
                }
                dt.Rows.Add(dr);
            }
            foreach (DataRow row in CirculateItemTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    if (col == WorkItem.WorkItem.PropertyName_Approval)
                    {
                        dr[col] = 1;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_UsedTime)
                    {
                        dr[col] = 0;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_ItemType)
                    {
                        dr[col] = 4;
                    }
                    else
                    {
                        dr[col] = row[col];
                    }
                }
                dt.Rows.Add(dr);
            }
            foreach (DataRow row in CirculateItemFinishedTable.Rows)
            {
                DataRow dr = dt.NewRow();
                foreach (var col in columns)
                {
                    if (col == WorkItem.WorkItem.PropertyName_Approval)
                    {
                        dr[col] = 1;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_UsedTime)
                    {
                        dr[col] = 0;
                    }
                    else if (col == WorkItem.WorkItem.PropertyName_ItemType)
                    {
                        dr[col] = 4;
                    }
                    else
                    {
                        dr[col] = row[col];
                    }
                }
                dt.Rows.Add(dr);
            }
            //排序
            dt.DefaultView.Sort = Instance.Token.PropertyName_TokenId + " asc";
            dt = dt.DefaultView.ToTable();
            return dt;
        }

        private IEnumerable<LogInfo> GetItemTable(
            DataTable TokenTable,
            DataTable ItemTable)
        {
            var query = from t1 in TokenTable.AsEnumerable()
                        join t2 in ItemTable.AsEnumerable()
                        on t1[Instance.Token.PropertyName_TokenId].ToString()
                            equals t2[WorkItem.WorkItem.PropertyName_TokenId].ToString()
                            into tmp
                        from workItem in tmp.DefaultIfEmpty()
                        // orderby workItem[WorkItem.WorkItem.PropertyName_TokenId], workItem[WorkItem.WorkItem.PropertyName_FinishTime]
                        select new
                        {
                            TokenID = int.Parse(t1[Instance.Token.PropertyName_TokenId] + string.Empty),
                            WorkItemID = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty,
                            Participant = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Participant] + string.Empty,
                            ParticipantName = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_ParticipantName] + string.Empty,
                            ActivityCode = workItem == null ? t1[Instance.Token.PropertyName_Activity] + string.Empty : workItem[WorkItem.WorkItem.PropertyName_ActivityCode] + string.Empty,
                            ActivityName = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                            CreatedTime = workItem == null ? DateTime.Parse(t1[Instance.Token.PropertyName_CreatedTime] + string.Empty)
                                    : DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty),
                            FinishedTime = workItem == null ? DateTime.Parse(t1[Instance.Token.PropertyName_FinishedTime] + string.Empty)
                                    : DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty),
                            SortTime = workItem == null ? DateTime.Parse(t1[Instance.Token.PropertyName_FinishedTime] + string.Empty)
                                : (DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty).Year < 2000 ? DateTime.Now : DateTime.Parse(workItem[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty)),
                            Approval = workItem == null ? string.Empty : (workItem[WorkItem.WorkItem.PropertyName_Approval] + string.Empty),
                            Retrievable = bool.Parse(t1[Instance.Token.PropertyName_Retrievable] + string.Empty),
                            Exceptional = bool.Parse(t1[Instance.Token.PropertyName_Exceptional] + string.Empty),
                            UsedTime = workItem == null ? t1[Instance.Token.PropertyName_UsedTime] + string.Empty : workItem[WorkItem.WorkItem.PropertyName_UsedTime] + string.Empty,
                            OrgUnit = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_OrgUnit] + string.Empty,
                            Delegant = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Delegant] + string.Empty,
                            Finisher = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Finisher] + string.Empty,
                            FinisherName = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_FinisherName] + string.Empty,
                            ItemType = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_ItemType] + string.Empty,
                            Creator = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_Creator] + string.Empty,
                            State = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_State] + string.Empty,
                            WorkflowCode = workItem == null ? string.Empty : workItem[WorkItem.WorkItem.PropertyName_WorkflowCode] + string.Empty,
                            InstanceState = t1 != null ? t1[Instance.InstanceContext.PropertyName_State] + string.Empty : string.Empty,
                        };
            var result = from q in query
                         orderby q.TokenID, q.SortTime
                         select new LogInfo
                         {
                             TokenID = q.TokenID,
                             WorkItemID = q.WorkItemID.Trim(),
                             Participant = q.Participant,
                             ParticipantName = q.ParticipantName,
                             ActivityCode = q.ActivityCode,
                             ActivityName = q.ActivityName,
                             CreatedTime = q.CreatedTime,
                             FinishedTime = q.FinishedTime,
                             Approval = q.Approval,
                             Retrievable = q.Retrievable,
                             Exceptional = q.Exceptional,
                             UsedTime = q.UsedTime,
                             OrgUnit = q.OrgUnit,
                             Delegant = q.Delegant.Trim(),
                             Finisher = q.Finisher.Trim(),
                             FinisherName = q.FinisherName,
                             ItemType = q.ItemType,
                             Creator = q.Creator.Trim(),
                             State = q.State,
                             WorkflowCode = q.WorkflowCode,
                             InstanceState = q.InstanceState
                         };
            return result;
        }
        /// <summary>
        /// 任务审批状态
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetApprovalState(string state)
        {

            string ApprovalName = string.Empty;
            switch (state)
            {
                case "-1":
                    ApprovalName = "处理中";
                    break;
                case "0":
                    ApprovalName = "拒绝";
                    break;
                case "1":
                    ApprovalName = "通过";
                    break;
                default:
                    ApprovalName = "已完成";
                    break;

            }
            return ApprovalName;
        }

        /// <summary>
        /// 获取状态名称
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        protected string GetStatusName(int State, string Approval)
        {
            WorkItem.WorkItemState s = (WorkItem.WorkItemState)State;
            if (s == WorkItem.WorkItemState.Waiting)
            {
                return "未启动";// this.PortalResource.GetString("MobileLogin_UnStart");
            }
            else if (s == WorkItem.WorkItemState.Working)
            {
                return "进行中";// this.PortalResource.GetString("MobileLogin_Working");
            }
            else if (s == WorkItem.WorkItemState.Finished)
            {
                string ApprovalName = string.Empty;
                switch (Approval)
                {
                    case "0":
                        ApprovalName = "拒绝";
                        break;
                    case "1":
                        ApprovalName = "通过";
                        break;
                    default:
                        ApprovalName = string.Empty;
                        break;
                }
                return ApprovalName;
                //return string.IsNullOrEmpty(ApprovalName) ? "已完成" : ("已完成 | " + ApprovalName);
                //return "已完成";// this.PortalResource.GetString("MobileLogin_WFinished");
            }
            else if (s == WorkItem.WorkItemState.Canceled)
            {
                return "已取消";// this.PortalResource.GetString("MobileLogin_Cancled");
            }
            return string.Empty;
        }

        #endregion

        #region 待阅、已阅
        /// <summary>
        /// 加载待阅任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="sortKey"></param>
        /// <param name="sortDirection"></param>
        /// <param name="readWorkItem"></param>
        /// <returns></returns>
        public Object LoadCirculateItemsFn(CirculateItemQueryParams Params)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            List<string> Conditions = new List<string>();
            string CirculateItemTableName = !Params.readWorkItem ? WorkItem.CirculateItem.TableName : WorkItem.CirculateItemFinished.TableName;

            string orderBy = string.Empty;
            if (Params.readWorkItem)
            {
                orderBy = " ORDER BY " + CirculateItemTableName + "." + WorkItem.CirculateItemFinished.PropertyName_FinishTime + " DESC ";
            }
            else
            {
                orderBy = " ORDER BY " + CirculateItemTableName + "." + WorkItem.CirculateItem.PropertyName_ReceiveTime + " DESC ";
            }
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.CirculateItem.PropertyName_Participant + "='" + Params.userId + "' OR " + WorkItem.CirculateItem.PropertyName_Delegant + "='" + Params.userId + "')");
            // 允许手机处理
            Conditions.Add(WorkItem.CirculateItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            if (!string.IsNullOrEmpty(Params.keyWord))
            {
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + Params.keyWord + "%'" + ")");
            }
            //时间条件
            if (Params.startDate.HasValue)
            {
                Conditions.Add(WorkItem.CirculateItem.PropertyName_ReceiveTime + ">= '" + string.Format("{0} 00:00:00", Params.startDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            if (Params.endDate.HasValue)
            {
                Conditions.Add(WorkItem.CirculateItem.PropertyName_ReceiveTime + "<= '" + string.Format("{0} 23:59:59", Params.endDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            if (Params.IsPriority.HasValue)
            {
                if (Params.IsPriority == (int)PriorityType.High)
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "=" + (int)PriorityType.High);
                }
                else
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "<" + (int)PriorityType.High);
                }
            }
            if (Params.Originators != null && Params.Originators.Length > 0)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Originator + " in ('" + string.Join("','", Params.Originators) + "')");
            }
            // 每次只返回10条
            dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), Params.loadStart + 1, Params.returnCount, orderBy, CirculateItemTableName);
            if (dtWorkItems == null) return null;

            int workItemCount = 0;
            // TODO 待阅数量待获取
            if (!Params.readWorkItem)
            {// 待阅才显示数量
                //workItemCount = GetAllCirculateItems(Params.userId).Length;
                workItemCount = Engine.Query.CountCirculateItem(Conditions.ToArray());
            }

            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.CirculateItem.PropertyName_Originator] + string.Empty);
                Originators.Add(row[WorkItem.CirculateItem.PropertyName_Creator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();
            }

            List<MobileWorkItem> workItems = new List<MobileWorkItem>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                workItems.Add(GetMobileCirculateItemFromRow(units, row));
            }
            //SetSummary(workItems, false);
            var result = new
            {
                LoadComplete = dtWorkItems.Rows.Count < 10, // 是否加载完成
                //LastTime = lastTime,                        // 最后加载时间
                //RefreshTime = refreshTime,                  // 刷新时需要使用的时间
                CirculateItems = workItems,                      // 工作任务内容
                TotalCount = workItemCount                  // 记录总数
            };
            return result;
        }

        /// <summary>
        /// 刷新待阅任务
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="keyWord"></param>
        /// <param name="lastTime"></param>
        /// <param name="readWorkItem"></param>
        /// <param name="existsLength"></param>
        /// <returns></returns>
        public Object RefreshCirculateItemFn(string userId,
            string mobileToken,
            string keyWord,
            DateTime lastTime,
            bool readWorkItem,
            int existsLength)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差
            string orderBy = string.Empty;
            List<string> Conditions = new List<string>();
            string CirculateItemTableName = readWorkItem ? WorkItem.CirculateItemFinished.TableName : WorkItem.CirculateItem.TableName;
            if (readWorkItem)
            {
                orderBy = " ORDER BY " + CirculateItemTableName + "." + WorkItem.CirculateItemFinished.PropertyName_FinishTime + " DESC ";
            }
            else
            {
                orderBy = " ORDER BY " + CirculateItemTableName + "." + WorkItem.CirculateItem.PropertyName_ReceiveTime + " DESC ";
            }
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.CirculateItem.PropertyName_Participant + "='" + userId + "' OR " + WorkItem.CirculateItem.PropertyName_Delegant + "='" + userId + "')");
            //允许手机处理
            Conditions.Add(WorkItem.CirculateItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            if (keyWord.Trim() != string.Empty)
            {
                Conditions.Add(Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%'");
            }
            // 每次只返回10条
            dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1, 10, orderBy, CirculateItemTableName);
            if (dtWorkItems == null) return null;
            int workItemCount = -1;

            if (dtWorkItems.Rows.Count > 0 && !readWorkItem)
            {
                //TODO  待处理
                workItemCount = GetAllCirculateItems(userId).Length;
            }

            List<MobileWorkItem> circulateItems = new List<MobileWorkItem>();
            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.CirculateItem.PropertyName_Originator] + string.Empty);
                Originators.Add(row[WorkItem.CirculateItem.PropertyName_Creator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();
            }

            foreach (DataRow row in dtWorkItems.Rows)
            {
                circulateItems.Add(GetMobileCirculateItemFromRow(units, row));
            }
            if (dtWorkItems.Rows.Count > 0)
            {
                lastTime = DateTime.Parse(dtWorkItems.Rows[0][WorkItem.CirculateItem.PropertyName_ReceiveTime] + string.Empty);
            }

            // 加载未完成的待办，因为PC端完成了，移动端可能没有刷新，这里返回所有待办中的任务ID，这里要改成查询 ObjectID
            List<string> existsIds = new List<string>();
            if (existsLength > 0)
            {
                Conditions.Add(WorkItem.WorkItem.PropertyName_ReceiveTime + "<=" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));

                dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 1, existsLength + 10, orderBy, CirculateItemTableName);
                foreach (DataRow row in dtWorkItems.Rows)
                {
                    existsIds.Add(row["ObjectID"] + string.Empty);
                }
            }

            var result = new
            {
                LastTime = lastTime,                        // 最后加载时间
                CirculateItems = circulateItems,                      // 工作任务内容
                TotalCount = workItemCount,                 // 记录总数
                ExistsIds = existsIds.ToArray()             // 未办完的待办任务ID集合
            };
            return result;
        }


        /// <summary>
        /// 执行已阅
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="mobileToken"></param>
        /// <param name="CirculateItemIDs"></param>
        /// <param name="ReadAll"></param>
        /// <returns></returns>
        public Object ReadCirculateItemsByBatchFn(string userId,
           string mobileToken,
           string CirculateItemIDs,
            bool ReadAll)
        {
            string[] ids;
            ids = CirculateItemIDs.Split(';');
            if (ReadAll)
            {
                //获取所有待阅任务
                ids = GetAllCirculateItems(userId);
            }
            foreach (string id in ids)
            {
                long result = this.Engine.WorkItemManager.FinishCirculateItem(id, userId, WorkItem.AccessPoint.Batch);
            }
            return ids;
        }


        #region 我的实例
        /// <summary>
        /// 加载我的流程
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="lastTime">上一次加载时间</param>
        /// <returns>我的流程实例</returns>
        [NonAction]
        public object LoadAllInstanceList(InstanceQueryParams Params)
        {
            //lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差
            //InstanceQueryParams Params = new InstanceQueryParams
            //{
            //    userId=userId,
            //    lastTime=lastTime,
            //    mobileToken=mobileToken
            //};
            var unfinishedInstance = LoadUnFinishedInstance(Params);//userId, lastTime, sortKey, sortDirection, (int)InstanceState.Running, WorkItem.WorkItemState.Unfinished, mobileToken,null,null,null);
            var finishedInstance = LoadFinishedInstance(Params);//userId, lastTime, sortKey, sortDirection, (int)InstanceState.Finished, WorkItem.WorkItemState.Finished, mobileToken,null,null,null);
            var canceledInstance = LoadCanceledInstance(Params);//userId, lastTime, sortKey, sortDirection, (int)InstanceState.Canceled, WorkItem.WorkItemState.Canceled, mobileToken,null,null,null);
            ActionResult result = new ActionResult(true);
            result.Extend = new
            {
                unfinished = unfinishedInstance,
                finished = finishedInstance,
                cancel = canceledInstance
            };
            return result;
        }
        [NonAction]
        public LoadWorkItemsClass LoadFinishedInstance(InstanceQueryParams Params)
        {
            Params.workState = WorkItem.WorkItemState.Finished;
            Params.status = (int)InstanceState.Finished;
            Params.sortKey = "FinishTime";
            Params.sortDirection = "Desc";
            var result = GetFinishInstance(Params);
            result.RefreshTime = System.DateTime.Now;
            return result;
        }
        [NonAction]
        public LoadWorkItemsClass LoadUnFinishedInstance(InstanceQueryParams Params)
        {
            Params.workState = WorkItem.WorkItemState.Unfinished;
            Params.status = (int)InstanceState.Running;
            Params.sortKey = "ReceiveTime";
            Params.sortDirection = "desc";
            var result = GetUnfinishInstance(Params);
            result.RefreshTime = System.DateTime.Now;
            return result;
        }
        [NonAction]
        public LoadWorkItemsClass LoadCanceledInstance(InstanceQueryParams Params)
        {
            Params.workState = WorkItem.WorkItemState.Canceled;
            Params.status = (int)InstanceState.Canceled;
            Params.sortKey = "FinishTime";
            Params.sortDirection = "desc";
            var result = GetFinishInstance(Params);
            result.RefreshTime = System.DateTime.Now;
            return result;
        }
        /// <summary>
        /// 加载更多流程实例
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="keyWord">关键字</param>
        /// <param name="lastTime">上一次加载时间</param>
        /// <param name="instanceState">流程状态</param>
        /// <returns>流程实例</returns>
        [NonAction]
        public object LoadInstances(string userId, string keyWord, int instanceState, int loadStart)
        {
            var Params = new InstanceQueryParams
            {
                userId = userId,
                keyWord = keyWord,
                loadStart = loadStart,
                sortKey = "CreatedTime",
                sortDirection = "Desc"
            };
            var instances = new LoadWorkItemsClass();
            if (instanceState == (int)InstanceState.Finished)
            {
                Params.workState = WorkItem.WorkItemState.Finished;
                instances = GetFinishInstance(Params);
            }
            else
            {
                Params.workState = WorkItem.WorkItemState.Unfinished;
                instances = LoadUnFinishedInstance(Params);
            }

            ActionResult result = new ActionResult(true);
            result.Extend = instances;
            return result;
        }
        /// <summary>
        /// 获取流程实例
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="lastTime">上一次加载时间</param>
        /// <param name="sortKey">排序码</param>
        /// <param name="sortDirection">排序顺序</param>
        /// <param name="state">流程状态</param>
        /// <param name="keyWord">关键词</param>
        /// <returns>流程实例列表</returns>
        private LoadWorkItemsClass GetUnfinishInstance(InstanceQueryParams Params)
        {
            DataTable dtWorkItems = null;
            Unit[] units = null;
            List<string> Conditions = new List<string>();
            string wrokItemTableName = Engine.Query.GetItemTableName(Params.workState);
            string orderBy = string.Empty;
            //Conditions.Add(Params.sortKey + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, Params.lastTime));
            if (!string.IsNullOrEmpty(Params.sortKey))
            {// 待办按照优先级,接收时间降序排
                orderBy = " ORDER BY " + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_Priority + " DESC," + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC ";
            }
            // 任务处理者条件
            Conditions.Add("(" + wrokItemTableName + "." + WorkItem.WorkItem.PropertyName_Originator + "='" + Params.userId + "')");
            // 允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");

            //关键字搜索(流程实例名称，流水号，发起人姓名)
            if (!string.IsNullOrWhiteSpace(Params.keyWord))
            {
                //Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'" + " OR " + Instance.InstanceContext.PropertyName_OriginatorName + " like '%" + keyWord + "%'" + ")");
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + Params.keyWord + "%')");
            }
            if (!string.IsNullOrEmpty(Params.Code))
            {
                Conditions.Add("(" + Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_WorkflowCode + " like '" + Params.Code + "')");
            }
            //时间条件
            //时间条件
            if (Params.startDate.HasValue)
            {
                Conditions.Add(WorkItem.CirculateItem.PropertyName_ReceiveTime + ">= '" + string.Format("{0} 00:00:00", Params.startDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            if (Params.endDate.HasValue)
            {
                Conditions.Add(WorkItem.CirculateItem.PropertyName_ReceiveTime + "<= '" + string.Format("{0} 23:59:59", Params.endDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            //是否加急
            if (Params.IsPriority.HasValue)
            {
                if (Params.IsPriority == (int)PriorityType.High)
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "=" + (int)PriorityType.High);
                }
                else
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "<" + (int)PriorityType.High);
                }
            }
            //状态条件
            Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "=" + Params.status);

            if (Params.refreshTime.HasValue)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_CreatedTime + ">='" + Params.refreshTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), 0, 999, orderBy, wrokItemTableName);
                Conditions.RemoveAt(Conditions.Count - 1);
            }
            else
            {
                //每次只返回10条
                dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), Params.loadStart + 1, Params.returnCount, orderBy, wrokItemTableName);
            }
            //Conditions.Add(wrokItemTableName+"."+WorkItem.WorkItem.PropertyName_State + "=" + (int)Params.workState);
            if (dtWorkItems == null) return null;

            int workItemCount = 0;

            workItemCount = Engine.Query.CountWorkItem(Conditions.ToArray(), string.IsNullOrWhiteSpace(Params.keyWord));
            // 加载数据
            List<string> Originators = new List<string>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                Originators.Add(row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty);
            }
            if (Originators.Count > 0)
            {
                units = this.Engine.Organization.GetUnits(Originators.ToArray()).ToArray();
                List<string> OriginatorOUs = new List<string>();
                foreach (Unit unit in units)
                {
                    OriginatorOUs.Add(unit.ParentID + string.Empty);
                }
                Unit[] OriginatorOUUnits = this.Engine.Organization.GetUnits(OriginatorOUs.ToArray()).ToArray();
                units = units.Union(OriginatorOUUnits).ToArray();
            }
            List<MobileWorkItem> workItems = new List<MobileWorkItem>();
            foreach (DataRow row in dtWorkItems.Rows)
            {
                workItems.Add(GetMobileWorkItemFromRow(units, row));
            }
            //SetSummary(workItems, true);
            var result = new LoadWorkItemsClass()
            {
                LoadComplete = dtWorkItems.Rows.Count < 10, // 是否加载完成
                //LastTime = lastTime,                        // 最后加载时间
                //RefreshTime = refreshTime,                  // 刷新时需要使用的时间
                WorkItems = workItems,                      // 工作任务内容
                TotalCount = workItemCount                  // 记录总数
            };
            return result;
            //return this.GetLoadWorkItems(userId, mobileToken, keyWord, lastTime, sortKey, sortDirection, false, Instance.PriorityType.Unspecified, null, null, 10, state);
            // DataTable dtInstances = null;
            // DateTime refreshTime = lastTime;
            // List<string> Conditions = new List<string>();
            // Conditions.Add(sortKey + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));

            // // 流程状态
            // Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "=" + state);
            // // 发起人条件
            // Conditions.Add(Instance.InstanceContext.PropertyName_Originator + "='" + userId + "'");
            // // 过滤条件
            // if (keyWord.Trim() != string.Empty)
            // {
            //     Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'" + ")");
            // }
            // //每次只返回10条
            //dtInstances = Engine.Query.QueryInstance(Conditions.ToArray(), 1, 10,
            //    sortKey,
            //    sortDirection.ToLower().IndexOf("desc") == -1);
            // if (dtInstances == null) return null;

            // //所属组织ID
            // List<string> unitsIds = new List<string>();
            // foreach (DataRow row in dtInstances.Rows)
            // {
            //     if (!unitsIds.Contains(row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty))
            //         unitsIds.Add(row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty);
            // };
            // List<Unit> units = this.Engine.Organization.GetUnits(unitsIds.ToArray());
            // // 加载数据
            // List<MobileInstance> instances = new List<MobileInstance>();
            // foreach (DataRow row in dtInstances.Rows)
            // {
            //     string unitId = (row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty).Trim();
            //     if (!string.IsNullOrEmpty(unitId))
            //     {
            //         instances.Add(new MobileInstance()
            //         {
            //             CreatedTime = Convert.ToDateTime(row[Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty).ToString("yyyy-MM-dd HH:mm"),
            //             FinishedTime = Convert.ToDateTime(row[Instance.InstanceContext.PropertyName_FinishTime] + string.Empty).ToString("yyyy-MM-dd HH:mm"),
            //             InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty,
            //             ObjectID = row[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty,
            //             Orginator = row[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty,
            //             OrgUnit = units.FirstOrDefault(s => s.UnitID == unitId).Name
            //         });
            //     }
            // }

            // if (dtInstances.Rows.Count > 0)
            // {
            //     lastTime = DateTime.Parse(dtInstances.Rows[instances.Count - 1][Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty);
            //     refreshTime = DateTime.Parse(dtInstances.Rows[0][Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty);
            // }

            // var result = new
            // {
            //     moredata = dtInstances.Rows.Count < 10,         // 是否还有数据
            //     lastReloadTime = lastTime,                      // 最后加载时间
            //     lastRefreshTime = refreshTime,                  // 刷新时需要使用的时间
            //     list = instances,                               // 工作任务内容
            //     InstanceState = state                           //流程状态
            // };

            // return result;
        }

        private LoadWorkItemsClass GetFinishInstance(InstanceQueryParams Params)
        {
            DataTable dtInstances = null;
            //DateTime refreshTime = Params.lastTime;
            //DateTime _lastTime = Params.lastTime;
            List<string> Conditions = new List<string>();
            //Conditions.Add(Params.sortKey + "<" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, refreshTime));

            // 流程状态
            Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "=" + Params.status);
            // 发起人条件
            Conditions.Add(Instance.InstanceContext.PropertyName_Originator + "='" + Params.userId + "'");
            // 过滤条件
            if (!string.IsNullOrEmpty(Params.keyWord))
            {
                Conditions.Add("(" + Instance.InstanceContext.PropertyName_InstanceName + " like '%" + Params.keyWord + "%')");
            }
            if (!string.IsNullOrEmpty(Params.Code))
            {
                Conditions.Add("(" + Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_WorkflowCode + " like '" + Params.Code + "')");
            }
            //是否加急
            if (Params.IsPriority.HasValue)
            {
                if (Params.IsPriority == (int)PriorityType.High)
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "=" + (int)PriorityType.High);
                }
                else
                {
                    Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_Priority + "<" + (int)PriorityType.High);
                }
            }
            if (Params.startDate.HasValue)
            {
                Conditions.Add(Instance.InstanceContext.PropertyName_CreatedTime + ">= '" + string.Format("{0} 00:00:00", Params.startDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            if (Params.endDate.HasValue)
            {
                Conditions.Add(Instance.InstanceContext.PropertyName_CreatedTime + "<= '" + string.Format("{0} 23:59:59", Params.endDate.Value.ToString("yyyy-MM-dd")) + "'");
            }
            if (Params.refreshTime.HasValue)
            {
                Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_FinishTime + ">='" + Params.refreshTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "'");
                //每次只返回10条
                dtInstances = Engine.Query.QueryInstance(Conditions.ToArray(), 0, 999,
                    Params.sortKey,
                    Params.sortDirection.ToLower().IndexOf("desc") == -1);
            }
            else
            {
                //每次只返回10条
                dtInstances = Engine.Query.QueryInstance(Conditions.ToArray(), Params.loadStart + 1, Params.returnCount,
                    Params.sortKey,
                    Params.sortDirection.ToLower().IndexOf("desc") == -1);
            }

            if (dtInstances == null) return null;

            //所属组织ID
            List<string> unitsIds = new List<string>();
            foreach (DataRow row in dtInstances.Rows)
            {
                if (!unitsIds.Contains(row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty))
                    unitsIds.Add(row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty);
            };
            List<Unit> units = this.Engine.Organization.GetUnits(unitsIds.ToArray());
            // 加载数据
            List<MobileWorkItem> instances = new List<MobileWorkItem>();
            foreach (DataRow row in dtInstances.Rows)
            {
                string unitId = (row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty).Trim();
                string instanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty;
                if (string.IsNullOrEmpty(instanceName))
                {
                    instanceName = this.Engine.WorkflowManager.GetClauseDisplayName(row[Instance.InstanceContext.PropertyName_WorkflowCode] + string.Empty);
                }
                if (!string.IsNullOrEmpty(unitId))
                {
                    instances.Add(new MobileWorkItem()
                    {
                        ObjectID = row[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty,
                        InstanceName = instanceName,
                        InsatanceId = row[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty,
                        Priority = this.GetColumnsValue(row, InstanceContext.PropertyName_Priority),
                        OrigiantorName = row[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty,
                        OrigiantorOUName = units.FirstOrDefault(s => s.UnitID == unitId).Name,
                        ReceiveTime = ConvertTimeFormat(row[Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty, "yyyy-MM-dd HH:mm"),
                        FinishTime = ConvertTimeFormat(row[Instance.InstanceContext.PropertyName_FinishTime] + string.Empty, "yyyy-MM-dd HH:mm"),
                        //ItemType = int.Parse(row[WorkItem.WorkItem.PropertyName_ItemType] + string.Empty),
                        Originator = row[Instance.InstanceContext.PropertyName_Originator] + string.Empty,
                        //RemindStatus = ((Instance.PriorityType)row[WorkItem.WorkItem.PropertyName_Priority]) == Instance.PriorityType.High ? Status.Urgent
                        //: (int.Parse(row[WorkItem.WorkItem.PropertyName_Urged] + string.Empty) == 0 ? Status.Normal : Status.Hasten),
                        OriginatorImageURL = GetUserImageURL(row[Instance.InstanceContext.PropertyName_Originator] + string.Empty),
                        BizObjectID = row[Instance.InstanceContext.PropertyName_BizObjectId] + string.Empty,
                        //ActivityName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                        //ActivityStatus = GetStatusName(int.Parse(row[WorkItem.WorkItem.PropertyName_State] + string.Empty), row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty),
                        //ApprovelStatueName = GetApprovalState(row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty),
                        //ApprovelStatus = row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty,
                        Summary = ConvertSummary(row[Instance.InstanceContext.PropertyName_WorkflowSummary] + string.Empty)
                    });
                }
            }
            var result = new LoadWorkItemsClass()
            {
                LoadComplete = dtInstances.Rows.Count < 10, // 是否加载完成
                //LastTime = _lastTime,                        // 最后加载时间
                //RefreshTime = refreshTime,                  // 刷新时需要使用的时间
                WorkItems = instances                      // 工作任务内容
                //TotalCount = workItemCount                  // 记录总数
            };

            return result;
        }

        /// <summary>
        /// 刷新用户的在办
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="mobileToken">用户身份</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="lastTime">最后刷新时间</param>
        /// <param name="instanceState">流程状态</param>
        [NonAction]
        public Object RefreshInstancesByState(string userId, string keyWord, DateTime lastTime, int instanceState)
        {
            ActionResult result = new ActionResult(true);

            DataTable dtInstances = null;
            lastTime = lastTime.AddSeconds(1); // 增加1秒，排除毫秒上的误差

            List<string> Conditions = new List<string>();
            // 时间条件
            Conditions.Add(Instance.InstanceContext.PropertyName_CreatedTime + ">" + OThinker.Data.Database.Database.GetDateString(this.Engine.EngineConfig.DBType, lastTime));
            // 流程状态
            Conditions.Add(Instance.InstanceContext.TableName + "." + Instance.InstanceContext.PropertyName_State + "=" + instanceState);
            // 发起人条件
            Conditions.Add(Instance.InstanceContext.PropertyName_Originator + "='" + userId + "'");
            // 过滤条件
            if (keyWord.Trim() != string.Empty)
            {
                Conditions.Add(Instance.InstanceContext.PropertyName_InstanceName + " like '%" + keyWord + "%' OR " + Instance.InstanceContext.PropertyName_SequenceNo + " like '%" + keyWord + "%'");
            }

            // 每次只返回10条
            dtInstances = Engine.Query.QueryInstance(Conditions.ToArray(),
                1,
                10,
                Instance.InstanceContext.PropertyName_CreatedTime,
                false);

            //所属组织ID
            List<string> unitsIds = new List<string>();
            foreach (DataRow row in dtInstances.Rows)
            {
                if (!unitsIds.Contains(row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty))
                    unitsIds.Add(row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty);
            };
            List<Unit> units = this.Engine.Organization.GetUnits(unitsIds.ToArray());
            // 加载数据
            List<MobileInstance> instances = new List<MobileInstance>();
            foreach (DataRow row in dtInstances.Rows)
            {
                string unitId = row[Instance.InstanceContext.PropertyName_OrgUnit] + string.Empty;
                instances.Add(new MobileInstance()
                {
                    CreatedTime = Convert.ToDateTime(row[Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty).ToString("MM-dd"),
                    InstanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty,
                    ObjectID = row[Instance.InstanceContext.PropertyName_ObjectID] + string.Empty,
                    Orginator = row[Instance.InstanceContext.PropertyName_OriginatorName] + string.Empty,
                    OrgUnit = units.FirstOrDefault(s => s.UnitID == unitId).Name
                });
            }

            if (dtInstances.Rows.Count > 0)
            {
                lastTime = DateTime.Parse(dtInstances.Rows[0][Instance.InstanceContext.PropertyName_CreatedTime] + string.Empty);
            }
            else
            {
                lastTime = lastTime.AddSeconds(-1);        //还原最后的刷新时间
            }

            result.Extend = new
            {
                moredata = dtInstances.Rows.Count < 10,         // 是否还有数据
                lastRefreshTime = lastTime,                  // 刷新时需要使用的时间
                list = instances,                               // 工作任务内容
                InstanceState = instanceState                   //流程状态
            };
            return result;
        }
        #endregion

        #region 返回用户任务数
        /// <summary>
        /// 返回用户所有待办任务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>ObjectId[]</returns>
        public int GetAllWorkItems(string userId)
        {
            if (string.IsNullOrEmpty(userId)) return 0;

            List<string> Conditions = new List<string>();
            string WorkItemTableName = WorkItem.WorkItem.TableName;
            string orderBy = " ORDER BY " + WorkItemTableName + "." + WorkItem.WorkItem.PropertyName_ReceiveTime + " DESC ";
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.WorkItem.PropertyName_Participant + "='" + userId + "' OR " + WorkItem.WorkItem.PropertyName_Delegant + "='" + userId + "')");
            //允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            // 每次只返回10条
            DataTable dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), -1, -1, orderBy, WorkItemTableName);
            if (dtWorkItems != null)
            {
                return dtWorkItems.Rows.Count;
            }
            return 0;
        }

        /// <summary>
        /// 返回用户所有待阅任务
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>ObjectId[]</returns>
        public string[] GetAllCirculateItems(string userId)
        {
            List<string> ids = new List<string>();
            if (string.IsNullOrEmpty(userId)) return ids.ToArray();

            List<string> Conditions = new List<string>();
            string CirculateItemTableName = WorkItem.CirculateItem.TableName;
            string orderBy = " ORDER BY " + CirculateItemTableName + "." + WorkItem.CirculateItem.PropertyName_ReceiveTime + " DESC ";
            // 任务处理者条件
            Conditions.Add("(" + WorkItem.CirculateItem.PropertyName_Participant + "='" + userId + "' OR " + WorkItem.CirculateItem.PropertyName_Delegant + "='" + userId + "')");
            //允许手机处理
            Conditions.Add(WorkItem.CirculateItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
            // 每次只返回10条
            DataTable dtWorkItems = Engine.PortalQuery.QueryWorkItem(Conditions.ToArray(), -1, -1, orderBy, CirculateItemTableName);
            if (dtWorkItems != null)
            {
                foreach (DataRow row in dtWorkItems.Rows)
                {
                    string id = row[WorkItem.CirculateItem.PropertyName_ObjectID] + string.Empty;
                    ids.Add(id);
                }
            }
            return ids.ToArray();
        }

        #endregion

        #endregion

        #region 组织、用户
        /// <summary>
        /// 获取指定父ID的直属子组织和用户
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <param name="parentId"></param>
        public Object GetOrganizationByParentFn(UserValidator userValidator, string userCode, string mobileToken, string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
            {
                parentId = this.Engine.Organization.Company.ObjectID;
            }

            OThinker.Organization.Unit parentUnit = this.Engine.Organization.GetUnit(parentId);
            if (parentUnit == null) return null;

            // 查组织
            Unit[] units = this.Engine.Organization.GetChildUnits(parentId, UnitType.OrganizationUnit, false, State.Active).ToArray();
            List<MobileOrganizationUnit> orgUnits = new List<MobileOrganizationUnit>();
            if (units != null)
            {
                List<int> sortKeys = new List<int>();
                foreach (Unit unit in units)
                {
                    sortKeys.Add(unit.SortKey);
                }
                units = OThinker.Data.Sorter<int, Unit>.Sort(sortKeys.ToArray(), units.ToArray());

                foreach (Unit unit in units)
                {
                    orgUnits.Add(new MobileOrganizationUnit()
                    {
                        ObjectID = unit.ObjectID,
                        Name = unit.Name
                    });
                }
            }

            // 查用户
            units = this.Engine.Organization.GetChildUnits(parentId, UnitType.User, false, State.Active).ToArray();
            List<MobileUser> orgUsers = new List<MobileUser>();

            if (units != null)
            {
                List<int> sortKeys = new List<int>();
                foreach (Unit unit in units)
                {
                    sortKeys.Add(unit.SortKey);
                }
                units = OThinker.Data.Sorter<int, Unit>.Sort(sortKeys.ToArray(), units);

                foreach (Unit unit in units)
                {
                    OThinker.Organization.User user = unit as OThinker.Organization.User;
                    orgUsers.Add(GetMobileUser(userValidator, user, user.ImageUrl, parentUnit.Name, string.Empty));
                }
            }

            string pId = string.Empty;
            if (parentUnit is Organization.OrganizationUnit)
            {
                pId = ((Organization.OrganizationUnit)parentUnit).ParentID;
            }

            //获取当前用户的ou
            OThinker.Organization.Unit userOU = this.Engine.Organization.GetParentUnit(userValidator.UserID);

            units = null;
            if (parentId != this.Engine.Organization.Company.ObjectID)
            {
                //获取所有的父OU
                units = this.Engine.Organization.GetParentUnits(parentId, UnitType.OrganizationUnit, true, State.Active).ToArray();
            }

            var userOuObject = new
            {
                name = userOU.Name,
                objectId = userOU.ObjectID
            };

            var result = new
            {
                orgUnits = orgUnits,
                orgUsers = orgUsers,
                parentName = parentUnit.Name,
                parentId = parentUnit.ObjectID,
                parentParentId = pId,
                userOU = userOuObject,
                parentUnits = units
            };
            return result;
        }

        /// <summary>
        /// 搜索用户
        /// </summary>
        /// <param name="userCode"></param>
        /// <param name="mobileToken"></param>
        /// <param name="parentId"></param>
        /// <param name="searchKey"></param>
        public Object SearchUserFn(string userCode, string mobileToken, string parentId, string searchKey)
        {
            searchKey = searchKey.Replace("'", string.Empty).Replace("-", string.Empty);
            if (string.IsNullOrEmpty(parentId))
            {
                parentId = this.Engine.Organization.Company.ObjectID;
            }

            string condition = "(" + OThinker.Organization.User.PropertyName_Name + " like '%" + searchKey + "%' OR " + OThinker.Organization.User.PropertyName_Code + " like '%" + searchKey + "%')";
            condition += " AND " + OThinker.Organization.User.PropertyName_State + "=0";

            string querySql = @"SELECT TOP 20 a.ObjectID,a.Code,a.Name,a.OfficePhone,a.Mobile,a.ImageUrl FROM OT_User a 
                             INNER JOIN OT_OrganizationUnit b ON a.ParentID=b.ObjectID
                             LEFT JOIN OT_OrganizationUnit c ON b.ParentID=c.ObjectID
                             LEFT JOIN OT_OrganizationUnit d ON c.ParentID=d.ObjectID
                             LEFT JOIN OT_OrganizationUnit e ON d.ParentID=e.ObjectID
                              where 
                               (a.Name like '%{0}%' Or a.Code like '%{0}%')
                               And (
                                a.ParentID='{1}'
	                            OR b.ParentID='{1}'
	                            OR c.ParentID='{1}'
	                            OR d.ParentID='{1}'
	                            OR e.ParentID='{1}'
                               )
                               And a.State=1
                             ORDER BY a.Name";
            this.Engine.LogWriter.Write("Mobile Query User->" + string.Format(querySql, searchKey, parentId));
            DataTable dtUser = Engine.Query.QueryTable(string.Format(querySql, searchKey, parentId));
            List<MobileUser> orgUsers = new List<MobileUser>();

            if (dtUser != null)
            {
                foreach (DataRow row in dtUser.Rows)
                {
                    string imageUrl = row[OThinker.Organization.User.PropertyName_ImageUrl] + string.Empty;
                    if (!string.IsNullOrEmpty(imageUrl))
                    {
                        try
                        {
                            int start = imageUrl.IndexOf("face");
                            imageUrl = imageUrl.Substring(start);
                        }
                        catch (Exception e) { }
                    }
                    orgUsers.Add(new MobileUser()
                    {
                        ObjectID = row[OThinker.Organization.User.PropertyName_ObjectID] + string.Empty,
                        Name = row[OThinker.Organization.User.PropertyName_Name] + string.Empty,
                        Code = row[OThinker.Organization.User.PropertyName_Code] + string.Empty,
                        OfficePhone = row[OThinker.Organization.User.PropertyName_OfficePhone] + string.Empty,
                        ImageUrl = imageUrl
                    });
                }
            }
            var result = new
            {
                orgUsers = orgUsers
            };
            return result;
        }

        /// <summary>
        /// 获取移动办公用户对象
        /// </summary>
        /// <param name="sourceUser">获取信息的用户</param>
        /// <param name="user">被获取的用户</param>
        /// <param name="imageUrl"></param>
        /// <param name="departmentName"></param>
        /// <param name="mobileToken"></param>
        /// <returns></returns>
        public MobileUser GetMobileUser(
            UserValidator sourceUser,
            OThinker.Organization.User user,
            string imageUrl,
            string departmentName,
            string mobileToken)
        {
            string mobile, officePhone, email, code, wechat;
            mobile = officePhone = email = code = wechat = string.Empty;

            if (user.PrivacyLevel == PrivacyLevel.PublicToAll
                || (sourceUser != null && user.PrivacyLevel == PrivacyLevel.PublicToDept && user.ParentID == sourceUser.User.ParentID)
                || (sourceUser != null && user.PrivacyLevel == PrivacyLevel.PublicToDepts && this.Engine.Organization.IsAncestor(user.ParentID, sourceUser.User.ParentID))
                || (sourceUser != null && user.PrivacyLevel == PrivacyLevel.Confidential && sourceUser.ValidateOrgView(user.ObjectID))
                )
            {
                mobile = user.Mobile;
                officePhone = user.OfficePhone;
                email = user.Email;
                code = user.Code;
                wechat = user.WeChatAccount;
            }

            imageUrl = OThinker.H3.Controllers.UserValidator.GetUserImagePath(this.Engine,
                user, this.PortalRoot + @"/TempImages/").Replace("//", "/");
            if (imageUrl != "")
            {
                imageUrl = "/" + imageUrl + "?T=" + DateTime.Now.ToString("yyyyMMddhhmmss");
            }

            MobileUser mobileUser = new MobileUser()
            {
                ObjectID = user.ObjectID,
                Name = user.Name,
                Code = user.Code,
                Appellation = user.Appellation,
                DepartmentName = departmentName,
                Email = email,
                Mobile = mobile,
                ImageUrl = imageUrl,
                OfficePhone = officePhone,
                WeChat = wechat,
                MobileToken = mobileToken
            };
            return mobileUser;
        }

        /// <summary>
        /// 获取组织的名称
        /// </summary>
        /// <param name="units"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private string GetUnitName(Unit[] units, string userId)
        {
            if (units == null) return string.Empty;
            foreach (Unit unit in units)
            {
                if (userId == unit.ObjectID)
                {
                    return unit.Name;
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取流程发起人OU名称
        /// </summary>
        /// <returns></returns>
        private string GetOriginatorOUName(Unit[] units, string originatorId)
        {
            if (units == null) return string.Empty;
            foreach (Unit unit in units)
            {
                if (originatorId == unit.ObjectID)
                {
                    var OUunitId = unit.ParentID;
                    foreach (Unit ou in units)
                    {
                        if (OUunitId == ou.ObjectID)
                        {
                            return ou.Name;
                        }
                    }
                }
            }
            return string.Empty;
        }
        /// <summary>
        /// 获取流程发起人头像URL
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        private String GetUserImageURL(string userId)
        {
            var userInfo = this.GetUserInfo(userId);
            return this.GetUserImageUrlAllowEmpty(userInfo);
        }
        private Organization.User GetUserInfo(string userId)
        {
            //if(this.Engine.EngineConfig.DBType)
            string querySql = GetUserInfoSql(this.Engine.EngineConfig.DBType);
            DataTable dtUser = Engine.Query.QueryTable(string.Format(querySql, userId));
            Organization.User user = new User();
            if (dtUser != null)
            {
                DataRow row = dtUser.Rows[0];
                var userCode = dtUser.Rows[0][OThinker.Organization.User.PropertyName_Code] == null ? string.Empty : dtUser.Rows[0][OThinker.Organization.User.PropertyName_Code].ToString();
                return this.Engine.Organization.GetUserByCode(userCode);
            }
            return new Organization.User();
        }

        /// <summary>
        /// 获取用户信息的查询语句
        /// </summary>
        /// <param name="dbType">数据库类型</param>
        /// <returns>查询语句</returns>
        private string GetUserInfoSql(DatabaseType dbType)
        {
            string sql = string.Empty;
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    sql = @"SELECT TOP 1 a.ObjectID,a.Code,a.Name,a.OfficePhone,a.Mobile,a.ImageUrl FROM OT_User a where a.ObjectID='{0}'";
                    break;
                case DatabaseType.Oracle:
                    sql = @"SELECT * FROM (SELECT  a.ObjectID,a.Code,a.Name,a.OfficePhone,a.Mobile,a.ImageUrl FROM OT_User a where a.ObjectID='{0}') WHERE ROWNUM=1";
                    break;
                default:
                    throw new NotImplementedException();
            };
            return sql;
        }
        #endregion

        #region 获取移动办公显示的任务数据
        public string GetColumnsValue(DataRow row, string cloumn)
        {
            return row.Table.Columns.Contains(cloumn) ? row[cloumn] + string.Empty : "";
        }

        public MobileWorkItem GetMobileWorkItemFromRow(Unit[] units, DataRow row)
        {
            string instanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty;
            if (string.IsNullOrWhiteSpace(instanceName))
            {
                var workitem = this.Engine.WorkItemManager.GetWorkItem(row[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty);
                //var template = this.Engine.WorkflowManager.GetPublishedTemplate(workitem.WorkflowCode, workitem.WorkflowVersion);
                instanceName = this.Engine.WorkflowManager.GetClauseDisplayName(workitem.WorkflowCode);
                //instanceName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty;
            }


                 Status remindStatus = row[WorkItem.WorkItem.PropertyName_Priority] +string.Empty == ((int)Instance.PriorityType.High).ToString() ? Status.Urgent
            : (int.Parse(row[WorkItem.WorkItem.PropertyName_Urged] + string.Empty) == 0 ? Status.Normal : Status.Hasten);
                 string activityName =!string.IsNullOrEmpty( row[WorkItem.WorkItem.PropertyName_ItemType]+string.Empty ) && int.Parse(row[WorkItem.WorkItem.PropertyName_ItemType]+string.Empty) == (int)WorkItem.WorkItemType.ActivityForward ? "转办" :
                row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty;
                



             

            return new MobileWorkItem()
            {
                ObjectID = row[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty,
                InstanceName = instanceName,
                InsatanceId = (row[WorkItem.WorkItem.PropertyName_InstanceId] + string.Empty),
                Priority = this.GetColumnsValue(row, InstanceContext.PropertyName_Priority),
                OrigiantorName = GetUnitName(units, row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty),
                OrigiantorOUName = GetOriginatorOUName(units, row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty),
                ReceiveTime = ConvertTimeFormat(row[WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty, "yyyy-MM-dd HH:mm"),
                FinishTime = ConvertTimeFormat(row[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty, "yyyy-MM-dd HH:mm"),
                ItemType = int.Parse(row[WorkItem.WorkItem.PropertyName_ItemType] + string.Empty),
                Originator = row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty,
                RemindStatus = remindStatus,
                OriginatorImageURL = GetUserImageURL(row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty),
                BizObjectID = row[Instance.InstanceContext.PropertyName_BizObjectId] + string.Empty,
                ActivityName = activityName,
                ActivityStatus = GetStatusName(int.Parse(row[WorkItem.WorkItem.PropertyName_State] + string.Empty), row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty),
                ApprovelStatueName = GetApprovalState(row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty),
                ApprovelStatus = row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty,
                Summary = ConvertSummary(row[Instance.InstanceContext.PropertyName_WorkflowSummary] + string.Empty)
            };
            
        }

        public MobileWorkItem GetMobileCirculateItemFromRow(Unit[] units, DataRow row)
        {
            string instanceName = row[Instance.InstanceContext.PropertyName_InstanceName] + string.Empty;
            if (string.IsNullOrWhiteSpace(instanceName))
            {
                var workitem = this.Engine.WorkItemManager.GetCirculateItem(row[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty);
                instanceName = this.Engine.WorkflowManager.GetClauseDisplayName(workitem.WorkflowCode);
                //instanceName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty;
            }
            return new MobileWorkItem()
            {
                ObjectID = row[WorkItem.WorkItem.PropertyName_ObjectID] + string.Empty,
                InstanceName = instanceName,
                CreatorName = GetUnitName(units, row[WorkItem.WorkItem.PropertyName_Creator] + string.Empty),
                OrigiantorName = GetUnitName(units, row[WorkItem.WorkItem.PropertyName_Originator] + string.Empty),
                ReceiveTime = ConvertTimeFormat(row[WorkItem.WorkItem.PropertyName_ReceiveTime] + string.Empty, "yyyy-MM-dd HH:mm"),
                FinishTime = ConvertTimeFormat(row[WorkItem.WorkItem.PropertyName_FinishTime] + string.Empty, "yyyy-MM-dd HH:mm"),
                BizObjectID = row[Instance.InstanceContext.PropertyName_BizObjectId] + string.Empty,
                ActivityName = row[WorkItem.WorkItem.PropertyName_DisplayName] + string.Empty,
                Summary = ConvertSummary(row[Instance.InstanceContext.PropertyName_WorkflowSummary] + string.Empty)
                //ActivityStatus = GetStatusName(int.Parse(row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty), row[WorkItem.WorkItem.PropertyName_Approval] + string.Empty)
            };
        }

        private string ConvertTimeFormat(string Time, string format = "yyyy-MM-dd")
        {
            var time = ParseTime(Time);
            if (time == null)
                return Time;
            return ((DateTime)time).ToString(format);
            //DateTime _Time;
            //if (DateTime.TryParse(Time, out _Time))
            //{
            //    string _TimeStr = _Time.ToString(format);
            //    if (_TimeStr.StartsWith("1753") || _TimeStr.StartsWith("9999") || _TimeStr.StartsWith("000"))
            //        _TimeStr = string.Empty;
            //    return _TimeStr;
            //}
            //return string.Empty;
        }

        private DateTime? ParseTime(string Time)
        {
            DateTime _Time;
            if (DateTime.TryParse(Time, out _Time))
            {
                string _timeStr = _Time.ToString();
                if (_timeStr.StartsWith("1753") || _timeStr.StartsWith("9999") || _timeStr.StartsWith("000"))
                    return null;
                return _Time;
            }
            return null;
        }

        #endregion

        #region 发起流程
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userValidate"></param>
        /// <returns></returns>
        public Object LoadWorkflowsFn(UserValidator userValidate)
        {
            MyWorkflowHandler MyWorkflowHandler = new MyWorkflowHandler(this, userValidate);
            List<WorkflowNode> Allnodes = new List<WorkflowNode>();
            List<WorkflowNode> nodes = MyWorkflowHandler.GetWorkflowNodes(true, true);
            List<MobileWorkflowCollection> workflows = new List<MobileWorkflowCollection>();

            Dictionary<string, string> allnodename = new Dictionary<string, string>();
            //先要获取所有级别的node（流程目录）
            if (nodes != null)
            {
                foreach (WorkflowNode node in nodes)
                {
                    allnodename = GetChildrenNodesOfPackage(Allnodes, "", node, allnodename);
                }
            }
            if (Allnodes != null)
            {
                foreach (WorkflowNode node in Allnodes)
                {
                    if (node.children != null)
                    {
                        List<MobileWorkflow> mobileWorkflows = new List<MobileWorkflow>();
                        LoadWorkflowNode(mobileWorkflows, node.DisplayName, node);
                        if (mobileWorkflows.Count > 0)
                        {
                            workflows.Add(new MobileWorkflowCollection()
                            {
                                DisplayName = allnodename[node.ObjectID],
                                Workflows = mobileWorkflows
                            });
                        }
                    }
                }
            }
            var result = new
            {
                Workflows = workflows
            };
            return result;
        }

        /// <summary>
        /// 递归获取所有流程魔板
        /// </summary>
        /// <param name="workflows"></param>
        /// <param name="parentName"></param>
        /// <param name="node"></param>
        private void LoadWorkflowNode(List<MobileWorkflow> workflows, string parentName, WorkflowNode node)
        {
            if (node.children != null && node.children.Count > 0)
            {
                foreach (WorkflowNode workflow in node.children)
                {
                    if (workflow.IsLeaf)
                    {
                        var WorkflowImage = string.IsNullOrEmpty(workflow.IconFileName) ? (this.PortalRoot + "/WFRes/image/Workflow_DefaultIcon.png") : (this.PortalRoot + "/TempImages/Workflow/" + workflow.IconFileName);
                        workflows.Add(new MobileWorkflow()
                        {
                            DisplayName = workflow.DisplayName,
                            WorkflowCode = workflow.Code,
                            IsFavorite = (workflow.Frequent == 1),
                            WorkflowImage = WorkflowImage
                        });
                    }
                }
            }
        }
        private Dictionary<string, string> GetChildrenNodesOfPackage(List<WorkflowNode> Allnodes, string parentName, WorkflowNode node, Dictionary<string, string> allnodename)
        {
            Dictionary<string, string> names = new Dictionary<string, string>();
            var name = string.Empty;
            if (parentName == "")
            {
                name = node.DisplayName;
            }
            else
            {
                name = parentName + " > " + node.DisplayName;
            }
            allnodename.Add(node.ObjectID, name);
            if (node.IsLeaf)
            { }
            else if (node.children != null && node.children.Count > 0)
            {
                Allnodes.Add(node);
                foreach (WorkflowNode workflownode in node.children)
                {
                    if (!workflownode.IsLeaf)
                    {
                        GetChildrenNodesOfPackage(Allnodes, name, workflownode, allnodename);
                    }
                }
            }
            return allnodename;
        }


        // public Object LoadWorkflows(UserValidator userValidate)
        // {
        //MyWorkflowHandler MyWorkflowHandler = new MyWorkflowHandler(this, userValidate);
        //List<WorkflowNode> nodes = MyWorkflowHandler.GetWorkflowNodes(true, true);

        //List<MobileWorkflowCollection> workflows = new List<MobileWorkflowCollection>();

        //if (nodes != null)
        //{
        //    foreach (WorkflowNode node in nodes)
        //    {
        //        if (node.children != null)
        //        {
        //            List<MobileWorkflow> mobileWorkflows = new List<MobileWorkflow>();

        //            LoadWorkflowNode(mobileWorkflows, node.DisplayName, node);
        //            if (mobileWorkflows.Count > 0)
        //            {
        //                workflows.Add(new MobileWorkflowCollection()
        //                {
        //                    DisplayName = node.DisplayName,
        //                    Workflows = mobileWorkflows
        //                });
        //            }
        //        }
        //    }
        //}
        //var result = new
        //{
        //    Workflows = workflows
        //};
        //return result;
        //}

        //private void LoadWorkflowNode(List<MobileWorkflow> workflows, string parentName, WorkflowNode node)
        //{
        //    if (node.IsLeaf)
        //    {
        //        workflows.Add(new MobileWorkflow()
        //        {
        //            DisplayName = node.DisplayName,
        //            WorkflowCode = node.Code,
        //            IsFavorite = (node.Frequent == 1)
        //        });
        //    }
        //    else if (node.children != null && node.children.Count > 0)
        //    {
        //        foreach (WorkflowNode workflow in node.children)
        //        {
        //            LoadWorkflowNode(workflows, parentName, workflow);
        //        }
        //    }
        //}

        #endregion

        #region 密码重置
        public ActionResult SetPasswordFn(string userCode, string oldPassword, string newPassword)
        {
            ActionResult result = new ActionResult(true);
            //验证
            UserValidator userValidator = UserValidatorFactory.Validate(userCode, oldPassword);

            if (userValidator != null)
            {
                OThinker.Organization.User EditUser = this.Engine.Organization.GetUserByCode(userCode);
                EditUser.Password = newPassword;
                Engine.Organization.UpdateUnit(EditUser.Code, EditUser);

                result.Message = "密码重置成功";
            }
            else
            {
                result.Success = false;
                result.Message = "密码验证不通过";
            }
            return result;
        }

        #endregion

        #region 移动主页
        /// <summary>
        /// 底部菜单 待办 待阅数量
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public object GetWorkItemCountCommon(string UserID)
        {

            // 构造查询用户帐号的条件
            string[] conditions = this.Engine.Query.GetWorkItemConditions(
                UserID,
                H3.WorkItem.WorkItem.PropertyName_ReceiveTime,
                DateTime.MinValue,
                DateTime.MaxValue,
                H3.WorkItem.WorkItemState.Unfinished,
                "",
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                H3.Instance.InstanceState.Unspecified,
                "",
                true,
                false);

            List<string> Conditions = new List<string>(conditions); 
            // 允许手机处理
            Conditions.Add(WorkItem.WorkItem.PropertyName_MobileProcessing + "='" + ((int)OThinker.Data.BoolMatchValue.True) + "'");
             
            // 获取总记录数      
            int UnfinishedWorkItemCount = this.Engine.Query.CountWorkItem(Conditions.ToArray());



            conditions = this.Engine.Query.GetWorkItemConditions(
                UserID,
                H3.WorkItem.WorkItem.PropertyName_ReceiveTime,
                DateTime.MinValue,
                DateTime.MaxValue,
                H3.WorkItem.WorkItemState.Unfinished,
                "",
                OThinker.Data.BoolMatchValue.Unspecified,
                OThinker.Data.BoolMatchValue.Unspecified,
                H3.Instance.InstanceState.Unspecified,
                "",
                true,
                true);
            List<string> UnreadConditions = new List<string>(conditions);
            // 获取总记录数
            int UnreadWorkItemCount = this.Engine.Query.Count(H3.WorkItem.CirculateItem.TableName, UnreadConditions.ToArray());

            return new { UnfinishedWorkItemCount = UnfinishedWorkItemCount, UnreadWorkItemCount = UnreadWorkItemCount };
        }



        #endregion


        #region 数据模型
        public List<object> GetDataModelDataFn(string userId, string DataModelCode, string QueryCode, string SortBy, int FromRowNum, int ShowCount)
        {
            List<object> result = new List<object>();
            BizObjectSchema schema = Engine.BizObjectManager.GetPublishedSchema(DataModelCode);
            DataModel.BizQuery query = Engine.BizObjectManager.GetBizQuery(QueryCode);
            if (schema != null && query != null)
            {
                OThinker.H3.BizBus.Filter.Filter filter = GetFilter(schema, "GetList", query);
                if (ShowCount <= 0) ShowCount = 5;
                filter.FromRowNum = FromRowNum;
                filter.ToRowNum = FromRowNum + ShowCount;
                //SORTBY是正常的语法 Column1,Column2 DESC
                if (string.IsNullOrEmpty(SortBy))
                {
                    SortBy = " CreatedTime DESC ";
                }
                string[] arrSortKey = SortBy.Split(',');
                List<OThinker.H3.BizBus.Filter.SortBy> list = new List<OThinker.H3.BizBus.Filter.SortBy>();
                foreach (string str in arrSortKey)
                {
                    if (string.IsNullOrEmpty(str))
                        continue;
                    string str2 = str.Trim();
                    string[] ssarr = str2.Split(' ');
                    if (ssarr.Length == 0 || string.IsNullOrEmpty(ssarr[0]))
                        continue;
                    string sortitem = ssarr[0];
                    OThinker.H3.BizBus.Filter.SortDirection sd = OThinker.H3.BizBus.Filter.SortDirection.Ascending;
                    for (var k = 1; k < ssarr.Length; k++)
                    {
                        if (string.IsNullOrEmpty(ssarr[k]))
                            continue;
                        if (ssarr[k].ToLower() == "asc")
                        {
                            break;
                        }
                        if (ssarr[k].ToLower() == "desc")
                        {
                            sd = OThinker.H3.BizBus.Filter.SortDirection.Descending;
                            break;
                        }
                    }
                    filter.AddSortBy(sortitem, sd);
                }
                DataModel.BizObject[] objs = schema.GetList(
                    this.Engine.Organization,
                    this.Engine.MetadataRepository,
                    this.Engine.BizObjectManager,
                    userId,
                    "GetList",
                    filter);
                // 开始绑定数据源
                DataTable tablesource = DataModel.BizObjectUtility.ToTable(schema, objs);
                List<object> tr = new List<object>();
                foreach (DataRow dr in tablesource.Rows)
                {
                    string ObjectID = dr["ObjectID"].ToString();
                    string Name = dr["Name"].ToString();
                    string CreatedTime = this.GetValueFromDate(dr["CreatedTime"], WorkItemTimeFormat);
                    InstanceContext[] InstanceContext = this.Engine.InstanceManager.GetInstanceContextsByBizObject(DataModelCode, ObjectID);
                    if (InstanceContext != null && InstanceContext.Length > 0)
                    {
                        var newRow = new
                        {
                            ObjectID = ObjectID,
                            Name = Name,
                            CreatedTime = CreatedTime,
                            InstanceId = InstanceContext[0].InstanceId
                        };
                        result.Add(newRow);
                    }
                }
            }
            return result;
        }

        private OThinker.H3.BizBus.Filter.Filter GetFilter(DataModel.BizObjectSchema Schema, string ListMethod, DataModel.BizQuery Query)
        {
            // 构造查询条件
            OThinker.H3.BizBus.Filter.Filter filter = new OThinker.H3.BizBus.Filter.Filter();

            OThinker.H3.BizBus.Filter.And and = new OThinker.H3.BizBus.Filter.And();
            filter.Matcher = and;
            ItemMatcher propertyMatcher = null;
            if (Query.QueryItems != null)
            {
                foreach (DataModel.BizQueryItem queryItem in Query.QueryItems)
                { // 增加系统参数条件
                    if (queryItem.FilterType == DataModel.FilterType.SystemParam)
                    {
                        propertyMatcher = new OThinker.H3.BizBus.Filter.ItemMatcher(queryItem.PropertyName,
                             OThinker.Data.ComparisonOperatorType.Equal,
                             SheetUtility.GetSystemParamValue(this.UserValidator, queryItem.SelectedValues));
                        and.Add(propertyMatcher);
                    }
                    else if (queryItem.Visible == OThinker.Data.BoolMatchValue.False)
                    {
                        and.Add(new ItemMatcher(queryItem.PropertyName,
                                queryItem.FilterType == DataModel.FilterType.Contains ? OThinker.Data.ComparisonOperatorType.Contain
                                : OThinker.Data.ComparisonOperatorType.Equal,
                                queryItem.DefaultValue));
                    }
                }
            }
            return filter;
        }
        #endregion

        #region 催办
        [NonAction]
        protected bool AddUrgeInstance(string InstanceID, string UrgeContent)
        {
            try
            {
                System.Collections.Generic.List<string> instances = new System.Collections.Generic.List<string>();
                instances.Add(InstanceID);
                instances = this.AddChildren(InstanceID, instances);
                // 催办
                foreach (string instance in instances)
                {
                    this.Engine.UrgencyManager.Urge(this.UserValidator.UserID, instance, UrgeContent);
                }
                return true;
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write(ex.Message);
                return false;
            }
        }
        private System.Collections.Generic.List<string> AddChildren(string InstanceID, System.Collections.Generic.List<string> Instances)
        {
            if (InstanceID != null && InstanceID != "" && InstanceID != Instance.InstanceContext.UnspecifiedID)
            {
                string[] children = this.Engine.PortalQuery.QueryChildInstances(
                        InstanceID,
                        H3.Instance.Token.UnspecifiedID,
                        OThinker.H3.Instance.InstanceState.Unfinished,
                        OThinker.Data.BoolMatchValue.Unspecified);

                if (children != null && children.Length != 0)
                {
                    foreach (string child in children)
                    {
                        if (child != null && child != "" && child != Instance.InstanceContext.UnspecifiedID)
                        {
                            Instances.Add(child);
                            this.AddChildren(child, Instances);
                        }
                    }
                }
            }
            return Instances;
        }
        #endregion
        #region 取消流程
        [NonAction]
        protected bool CancelInstance(string InstanceID)
        {
            try
            {
                Instance.InstanceContext InstanceContext = this.Engine.InstanceManager.GetInstanceContext(InstanceID);
                // 取消该流程
                Messages.CancelInstanceMessage cancelMessage =
                        new Messages.CancelInstanceMessage(
                            Messages.MessageEmergencyType.Normal,
                            InstanceID,
                            true);
                this.Engine.InstanceManager.SendMessage(cancelMessage);
                // 记录日志
                Tracking.UserLog log = new Tracking.UserLog(
                                    Tracking.UserLogType.CancelInstance,
                                    this.UserValidator.UserID,
                                    InstanceContext.BizObjectSchemaCode,
                                    InstanceContext.BizObjectId,
                                    InstanceID,
                                    InstanceID,
                                    null,
                                    null,
                                    SheetUtility.GetClientIP(Request),
                                    SheetUtility.GetClientPlatform(Request),
                                    SheetUtility.GetClientBrowser(Request));
                this.Engine.UserLogWriter.Write(log);
                return true;
            }
            catch (Exception ex)
            {
                this.Engine.LogWriter.Write(ex.Message);
                return false;
            }
        }
        #endregion

        /// <summary>
        /// 验证是否有权限访问当前编码
        /// </summary>
        /// <param name="FunctionCode"></param>
        /// <returns></returns>
        private bool ValidateFunction(string FunctionCode)
        {
            if (FunctionCode == string.Empty) return true;

            return this.UserValidator.ValidateFunctionRun(FunctionCode);
        }

        #region class
        /// <summary>
        /// 流程模板集合
        /// </summary>
        public class MobileWorkflowCollection
        {
            public string DisplayName { get; set; }
            public List<MobileWorkflow> Workflows { get; set; }
        }

        /// <summary>
        /// 流程模板
        /// </summary>
        public class MobileWorkflow
        {
            /// <summary>
            /// 获取或设置流程显示名称
            /// </summary>
            public string DisplayName { get; set; }
            /// <summary>
            /// 获取或设置流程编码
            /// </summary>
            public string WorkflowCode { get; set; }
            /// <summary>
            /// 获取或设置流程图片路径
            /// </summary>
            public string WorkflowImage { get; set; }
            /// <summary>
            /// 获取或设置是否已收藏
            /// </summary>
            public bool IsFavorite { get; set; }
        }

        /// <summary>
        /// 工作任务表表
        /// </summary>
        public class MobileWorkItem
        {
            /// <summary>
            /// 获取或设置工作任务ID
            /// </summary>
            public string ObjectID { get; set; }
            /// <summary>
            /// 获取或设置流程实例名称
            /// </summary>
            public string InstanceName { get; set; }
            /// <summary>
            ///设置或获取流程实例ID
            /// </summary>
            public string InsatanceId { get; set; }
            /// <summary>
            /// 获取或设置流程实例优先级
            /// </summary>
            public string Priority { get; set; }
            /// <summary>
            /// 获取或设置发起人名称
            /// </summary>
            public string OrigiantorName { get; set; }
            /// <summary>
            /// 获取或设置发起人OU名称
            /// </summary>
            public string OrigiantorOUName { get; set; }
            /// <summary>
            /// 获取或设置任务来源人名称
            /// </summary>
            public string CreatorName { get; set; }
            /// <summary>
            /// 获取或设置接收时间
            /// </summary>
            public string ReceiveTime { get; set; }
            /// <summary>
            /// 获取或设置完成时间
            /// </summary>
            public string FinishTime { get; set; }
            /// <summary>
            /// 获取或设置工作项类型
            /// </summary>
            public int ItemType { get; set; }
            /// <summary>
            /// 获取或设置操作人
            /// </summary>
            public string Originator { get; set; }
            /// <summary>
            /// 获取或设置发起人头像URL
            /// </summary>
            public string OriginatorImageURL { get; set; }
            /// <summary>
            /// 获取或设置备注
            /// </summary>
            public List<WorkItemSummary> Summary { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public Status RemindStatus { get; set; }
            /// <summary>
            /// Biz实例ID
            /// </summary>
            public string BizObjectID { get; set; }
            /// <summary>
            /// 当前流程状态名称
            /// </summary>
            public string ActivityName { get; set; }
            /// <summary>
            /// 当前流程状态名称{状态|审批状态}
            /// </summary>
            public string ActivityStatus { get; set; }
            /// <summary>
            /// 处理状态
            /// </summary>
            public string ApprovelStatus { get; set; }
            /// <summary>
            /// 审批名称
            /// </summary>
            public string ApprovelStatueName { get; set; }
        }

        public class WorkItemSummary
        {
            public string DisplayName { get; set; }
            public string Value { get; set; }
        }
        public enum Status
        {
            /// <summary>
            /// 普通
            /// </summary>
            Normal = 0,
            /// <summary>
            /// 加急
            /// </summary>
            Urgent = 1,
            /// <summary>
            /// 催办
            /// </summary>
            Hasten = 2
        }
        /// <summary>
        /// 工作任务表表
        /// </summary>
        public class MobileInstance
        {
            /// <summary>
            /// 获取或设置工作任务ID
            /// </summary>
            public string ObjectID { get; set; }

            /// <summary>
            /// 获取或设置流程实例名称
            /// </summary>
            public string InstanceName { get; set; }

            /// <summary>
            /// 获取或设置发起时间
            /// </summary>
            public string CreatedTime { get; set; }

            /// <summary>
            /// 获取或设置完成、取消时间
            /// </summary>
            public string FinishedTime { get; set; }

            /// <summary>
            /// 获取或者设置发起人
            /// </summary>
            public string Orginator { get; set; }

            /// <summary>
            /// 获取或设置发起人组织
            /// </summary>
            public string OrgUnit { get; set; }
        }

        /// <summary>
        /// 工作任务表表
        /// </summary>
        public class MobileUser
        {
            /// <summary>
            /// 获取或设置用户ID
            /// </summary>
            public string ObjectID { get; set; }
            /// <summary>
            /// 获取或设置用户名称
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 获取或设置用户编码
            /// </summary>
            public string Code { get; set; }
            /// <summary>
            /// 获取或设置用户的称谓
            /// </summary>
            public string Appellation { get; set; }
            /// <summary>
            /// 获取或设置用户的部门名称
            /// </summary>
            public string DepartmentName { get; set; }
            /// <summary>
            /// 获取或设置用户的邮箱
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// 获取或设置用户的电话
            /// </summary>
            public string Mobile { get; set; }
            /// <summary>
            /// 获取或设置用户的办公电话
            /// </summary>
            public string OfficePhone { get; set; }
            /// <summary>
            /// 获取或设置用户的微信号
            /// </summary>
            public string WeChat { get; set; }
            /// <summary>
            /// 获取或设置用户的头像URL
            /// </summary>
            public string ImageUrl { get; set; }
            /// <summary>
            /// 最新一次的MobileToken
            /// </summary>
            public string MobileToken { get; set; }
        }

        /// <summary>
        /// 工作任务表表
        /// </summary>
        public class MobileOrganizationUnit
        {
            /// <summary>
            /// 获取或设置组织机构ID
            /// </summary>
            public string ObjectID { get; set; }
            /// <summary>
            /// 获取或设置组织机构名称
            /// </summary>
            public string Name { get; set; }
        }

        /// <summary>
        /// 刷新任务
        /// </summary>
        public class RefreshWorkItemsClass
        {
            public DateTime LastTime { get; set; }                   // 最后加载时间
            public List<MobileWorkItem> WorkItems { get; set; }      // 工作任务内容
            public int TotalCount { get; set; }                      // 记录总数
            public string[] ExistsIds { get; set; }
        }

        /// <summary>
        /// 待办任务
        /// </summary>
        public class LoadWorkItemsClass
        {
            public bool LoadComplete { get; set; }                    // 是否加载完成
            public DateTime LastTime { get; set; }                    // 最后加载时间
            public DateTime RefreshTime { get; set; }                 // 刷新时需要使用的时间
            public List<MobileWorkItem> WorkItems { get; set; }          // 工作任务内容
            public int TotalCount { get; set; }                       // 记录总数
        }
        #endregion

        #region

        #endregion

        #region 查询条件实体
        //参数太多，改成实体传参
        [Serializable]
        public class QueryParams
        {
            public string userId { get; set; }
            public string mobileToken { get; set; }
            public string keyWord { get; set; }
            public string sortKey { get; set; }
            public string sortDirection { get; set; }

            //public bool finishedWorkItem { get; set; }
            public DateTime? startDate { get; set; }
            public DateTime? endDate { get; set; }
            public int? IsPriority { get; set; }
            public int loadStart { get; set; }

            private int _pageVolume = 10;
            public int returnCount { get { return loadStart + _pageVolume; } }

            public string Code { get; set; }
        }
        public class WorkItemQueryParams : QueryParams
        {
            //public DateTime? lastTime { get; set; }
            public bool finishedWorkItem { get; set; }
            public string[] Originators { get; set; }
            public int? instanceState { get; set; }
        }
        [Serializable]
        public class CirculateItemQueryParams : QueryParams
        {
            public bool readWorkItem { get; set; }
            //public DateTime? lastTime { get; set; }
            public string[] Originators { get; set; }
        }

        public class InstanceQueryParams : QueryParams
        {
            public int status { get; set; }
            //public DateTime lastTime { get; set; }
            public WorkItem.WorkItemState workState { get; set; }

            public DateTime? refreshTime { get; set; }
        }
        #endregion

        #region 获取应用
        protected Apps FetchAllApps(string userId)
        {
            List<OThinker.H3.Apps.AppNavigation> Apps = new List<OThinker.H3.Apps.AppNavigation>();
            List<OThinker.H3.Apps.AppNavigation> FavApps = new List<OThinker.H3.Apps.AppNavigation>();
            OThinker.H3.Apps.AppNavigation[] AllApps = this.Engine.AppNavigationManager.GetAllApps();
            var favoriteAppCodes = this.Engine.AppNavigationManager.GetFavoriteApps(userId);
            //Dictionary<string, OThinker.H3.Apps.AppNavigation> appNavigation = new Dictionary<string, Apps.AppNavigation>();
            foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
            {
                foreach (OThinker.H3.Apps.AppNavigation app in AllApps)
                {
                    if (functionNode.Code == app.AppCode && app.VisibleOnMobile == OThinker.Data.BoolMatchValue.True)
                    {
                        Apps.Add(app);
                        if (favoriteAppCodes.Contains(app.AppCode))
                            FavApps.Add(app);
                    }
                }
            }
            return new Apps() { AllApps = Apps, FavoriteApps = FavApps };
        }

        protected Apps GetApps(string AppCode)
        {
            OThinker.H3.Apps.AppNavigation App = this.Engine.AppNavigationManager.GetApp(AppCode);
            List<OThinker.H3.Apps.AppNavigation> Apps = new List<OThinker.H3.Apps.AppNavigation>();
            foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
            {
                if (functionNode.Code == App.AppCode && App.VisibleOnMobile == OThinker.Data.BoolMatchValue.True)
                {
                    Apps.Add(App);
                }
            }
            return new Apps() { AllApps = Apps, FavoriteApps = null };
        }
        protected List<FunctionViewModel> FetchFunction(string appCode)
        {
            var functions = FindChildren(appCode);
            foreach (var function in functions)
            {
                function.Children = FindChildren(function.Code);
            }
            return functions;
        }

        private List<FunctionViewModel> FindChildren(string partnerCode)
        {
            List<FunctionViewModel> children = new List<FunctionViewModel>();
            foreach (OThinker.H3.Acl.FunctionNode functionNode in this.UserValidator.RunnableFunctions)
            {
                if (functionNode.ParentCode == partnerCode)
                {
                    FunctionViewModel viewModel = new FunctionViewModel()
                    {
                        Code = functionNode.Code,
                        DisplayName = functionNode.DisplayName,
                        //TopAppCode = this.TopAppCode,
                        IconCss = functionNode.IconCss,
                        SortKey = functionNode.SortKey,
                        Url = functionNode.Url,
                        Children = new List<FunctionViewModel>()
                    };
                    children.Add(viewModel);
                }
            }
            return children;
        }

        protected bool SetFavoriteApp(string userID, string appCode, bool isFavorite)
        {
            if (isFavorite)
            {
                return this.Engine.AppNavigationManager.AddFavoriteApp(userID, appCode);
            }
            else
            {
                return this.Engine.AppNavigationManager.DeleteFavoriteApp(userID, appCode);
            }
        }

        public class Apps
        {
            public List<OThinker.H3.Apps.AppNavigation> AllApps { get; set; }

            public List<OThinker.H3.Apps.AppNavigation> FavoriteApps { get; set; }
        }
        #endregion
    }
}
