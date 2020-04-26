using OThinker.H3.Acl;
using OThinker.H3.Controllers.ViewModels;
using OThinker.H3.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.ProcessModel
{
    /// <summary>
    /// 定时作业控制器
    /// </summary>
   [Authorize]
    public class ScheduleInvokerController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }
        #region 参数
        private string InvokerId = null;
        private string SchemaCode = null;
        private bool Edit = true;
        private ScheduleInvoker Invoker = null;
        private H3.DataModel.BizObjectSchema Schema = null;

        /// <summary>
        /// 解析参数
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="id">定时任务ID</param>
        /// <returns>解析结果</returns>
        private ActionResult ParseParam(string schemaCode = "", string id = "")
        {
            ActionResult result = new ActionResult();
            this.InvokerId = id;
            if (string.IsNullOrEmpty(this.InvokerId))
            {
                // 新建模式
                this.Edit = false;
                this.SchemaCode = schemaCode;

                if (string.IsNullOrEmpty(this.SchemaCode))
                {
                    //指定的参数不正确
                    result.Message = "ScheduleInvoker.Msg3";
                    result.Success = false;
                    return result;
                }

                this.Schema = this.Engine.BizObjectManager.GetPublishedSchema(this.SchemaCode);
                if (this.Schema == null)
                {
                    //业务对象模式不存在
                    result.Message = "ScheduleInvoker.Msg4";
                    result.Success = false;
                    return result;
                }
            }
            else
            {
                // 编辑模式
                this.Edit = true;

                this.Invoker = this.Engine.BizObjectManager.GetScheduleInvoker(this.InvokerId);
                if (this.Invoker == null)
                {
                    //该定期调用设置不存在
                    result.Message = "ScheduleInvoker.Msg5";
                    result.Success = false;
                    return result;
                }

                this.Schema = this.Engine.BizObjectManager.GetDraftSchema(this.Invoker.SchemaCode);
                this.SchemaCode = this.Invoker.SchemaCode;
                if (this.Schema == null)
                {
                    //业务对象模式不存在

                    result.Message = "ScheduleInvoker.Msg1";
                    result.Success = false;
                    return result;
                }
            }
            result.Success = true;
            return result;
        }
        #endregion
        /// <summary>
        /// 获取定时作业列表
        /// </summary>
        /// <param name="pageInfo">分页信息</param>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>定时作业列表</returns>
        [HttpPost]
        public JsonResult GetScheduleInvokerList(PagerInfo pageInfo, string schemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                ScheduleInvoker[] invokers = this.Engine.BizObjectManager.GetScheduleInvokerList(schemaCode);
                List<ScheduleInvokerViewModel> invokerList = new List<ScheduleInvokerViewModel>();
                ScheduleInvokerViewModel model = new ScheduleInvokerViewModel();

                if (invokers != null && invokers.Length != 0)
                {

                    invokerList = invokers.Skip(pageInfo.PageSize * (pageInfo.PageIndex - 1)).Take(pageInfo.PageSize).Select(s => new ScheduleInvokerViewModel()
                    {
                        ObjectID = s.ObjectID,
                        DisplayName = s.DisplayName,
                        CreatedTime = s.CreatedTime.ToShortDateString(),
                        ModifiedTime =s.ModifiedTime.ToShortDateString(),
                        State = s.State.ToString()

                    }).ToList();
                }
                result.Extend = new { Rows = invokerList, Total = invokerList.Count };
                return Json(result.Extend, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 保存定时任务
        /// </summary>
        /// <param name="model">定时任务模型</param>
        /// <returns>是否保存成功</returns>
        [HttpPost]
        [ValidateInput(false)]

        public JsonResult SaveScheduleInvoker(ScheduleInvokerViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode, model.ObjectID);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                // 验证输入是否正确
                int interval = 0;
                if (!int.TryParse(model.IntervalSecond, out interval))
                {

                    //时间间隔必须是正整数
                    result.Message = "ScheduleInvoker.Msg7";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (interval < H3.DataModel.Declaration.Filter_MinIntervalSecond)
                {
                    //时间间隔设置过于密集，会造成服务器资源消耗太大
                    result.Message = "ScheduleInvoker.Msg8";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 验证过滤器的设置
                try
                {
                    H3.BizBus.Filter.Filter filter = (H3.BizBus.Filter.Filter)OThinker.Data.Convertor.XmlToObject(typeof(H3.BizBus.Filter.Filter), model.FilterDefinition);
                }
                catch
                {
                    //过滤器设置不正确，无法解析
                    result.Message = "ScheduleInvoker.Msg9";
                    result.Success = false;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 验证条件表达式
                if (!string.IsNullOrEmpty(model.ExeCondition))
                {
                    H3.DataModel.BizObject bo = new H3.DataModel.BizObject(this.Engine.Organization, this.Engine.MetadataRepository, this.Engine.BizObjectManager, this.Engine.BizBus, this.Schema, this.UserValidator.UserID, this.UserValidator.User.ParentID);
                    try
                    {
                        object v = bo.CalcExpression<bool>(model.ExeCondition);
                    }
                    catch (Exception ex)
                    {
                        //条件表达式不正确
                        result.Message = "ScheduleInvoker.Msg10";
                        result.Success = false;
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                }

                H3.DataModel.ScheduleInvoker si = null;
                if (this.Edit)
                {
                    si = this.Invoker;
                }
                else
                {
                    si = new ScheduleInvoker();
                    si.SchemaCode = this.SchemaCode;
                }

                si.DisplayName = model.DisplayName;
                si.Description = model.Description;
                si.State = (ScheduleInvokerState)Enum.Parse(typeof(ScheduleInvokerState), model.State, false);
                si.StartTime = DateTime.Parse(model.StartTime);
                si.EndTime = DateTime.Parse(model.EndTime);
                si.Recurrency = (RecurrencyType)Enum.Parse(typeof(RecurrencyType), model.RecurrencyType, false);
                si.IntervalSecond = interval;
                si.FilterMethod = model.FilterMethod;
                si.FilterDefinition = model.FilterDefinition;
                si.Condition = string.IsNullOrEmpty(model.ExeCondition) ? null : model.ExeCondition;
                si.MethodName = model.MethodExec;
                si.ModifiedTime = DateTime.Now;
                if (this.Edit)
                {
                    result.Success = this.Engine.BizObjectManager.UpdateScheduleInvoker(si);
                }
                else
                {
                    result.Success = this.Engine.BizObjectManager.AddScheduleInvoker(si);
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 删除定时任务
        /// </summary>
        /// <param name="ids">定时任务ID</param>
        /// <returns>是否成功</returns>
        [HttpPost]
        public JsonResult DelScheduleInvoker(string ids)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                List<string> errorName = new List<string>();
                if (!string.IsNullOrWhiteSpace(ids))
                {
                    string[] idArray = ids.Split(';');
                    foreach (string id in idArray)
                    {
                        this.Engine.BizObjectManager.RemoveScheduleInvoker(id);
                    }
                }
                result.Success = true;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
        /// <summary>
        /// 获取定时作业信息
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="id">定时作业Id</param>
        /// <returns>定时作业信息</returns>
        [HttpGet]
        public JsonResult GetScheduleInvoker(string schemaCode, string id)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode, id);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                ScheduleInvokerViewModel model;
                List<Item> states = GetStates();
                List<Item> recurrencyTypes = GetRecurrencyTypes();
                List<Item> filterMethods = GetFilterMethods();
                List<Item> matchers = GetMatchers();
                List<Item> columns = GetColumns();
                List<Item> operators = GetOperators();
                List<Item> orders = GetOrders();
                List<Item> methodExecs = GetMethodExecs();
                if (this.Edit)
                {
                    model = GetScheduleInvoker();
                }
                else
                {
                    model = new ScheduleInvokerViewModel();
                    model.SchemaCode = schemaCode;
                    model.DisplayName = "";
                    model.Description = "";
                    model.State = ScheduleInvokerState.Active.ToString();
                    model.StartTime = DateTime.Now.ToShortDateString();
                    model.EndTime = DateTime.Now.ToShortDateString();
                    model.RecurrencyType = RecurrencyType.AllTheTime.ToString();
                    model.IntervalSecond = "1000";
                    model.FilterMethod = filterMethods.FirstOrDefault() == null ? "" : filterMethods.FirstOrDefault().Value;
                    model.ExeCondition = "";
                    model.MethodExec = methodExecs.FirstOrDefault() == null ? "" : methodExecs.FirstOrDefault().Value;
                    model.FilterDefinition = OThinker.Data.Convertor.ObjectToXml(H3.DataModel.BizObjectUtility.CreateDefaultFilter(this.Schema));
                }
                result.Extend = new
                {
                    ScheduleInvoker = model,
                    States = states,
                    RecurrencyTypes = recurrencyTypes,
                    FilterMethods = filterMethods,
                    Matchers = matchers,
                    Columns = columns,
                    Operators = operators,
                    Orders = orders,
                    MethodExecs = methodExecs,
                    OrdersList = new { },
                    ConditionsList = new { },
                    ParentId = "",
                    IsLocked =  BizWorkflowPackageLockByID(this.SchemaCode)
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取定时任务信息
        /// </summary>
        /// <returns>定时任务信息</returns>
        private ScheduleInvokerViewModel GetScheduleInvoker()
        {
            ScheduleInvokerViewModel model = new ScheduleInvokerViewModel();
            model.ObjectID = this.InvokerId;
            model.SchemaCode = this.SchemaCode;
            model.DisplayName = this.Invoker.DisplayName;
            model.Description = this.Invoker.Description;
            model.State = ((ScheduleInvokerState)this.Invoker.State).ToString();
            model.StartTime = this.Invoker.StartTime.ToShortDateString();
            model.EndTime = this.Invoker.EndTime.ToShortDateString();
            model.RecurrencyType = ((RecurrencyType)this.Invoker.Recurrency).ToString();
            model.IntervalSecond = this.Invoker.IntervalSecond.ToString();
            model.FilterMethod = this.Invoker.FilterMethod;
            model.ExeCondition = this.Invoker.Condition;
            model.MethodExec = this.Invoker.MethodName;
            model.FilterDefinition = this.Invoker.FilterDefinition;
            return model;
        }

        /// <summary>
        /// 加载可执行方法列表
        /// </summary>
        /// <returns>可执行方法列表</returns>
        private List<Item> GetMethodExecs()
        {
            List<Item> list = new List<Item>();
            // 加载方法列表
            foreach (H3.DataModel.MethodGroupSchema method in this.Schema.Methods)
            {
                if (method.MethodType != H3.DataModel.MethodType.Filter && !H3.DataModel.BizObjectSchema.IsDefaultMethod(method.MethodName))
                {
                    list.Add(new Item(method.FullName, method.MethodName));
                }
            }
            return list;
        }
        /// <summary>
        /// 获取排序标示
        /// </summary>
        /// <returns>排序标示</returns>
        private List<Item> GetOrders()
        {
            List<Item> list = new List<Item>();
            foreach (var m in Enum.GetNames(typeof(OThinker.H3.BizBus.Filter.SortDirection)))
            {
                if (m != OThinker.H3.BizBus.Filter.SortDirection.Unspecified.ToString())
                    list.Add(new Item(m, m));
            }
            return list;
        }

        /// <summary>
        /// 获取运算符
        /// </summary>
        /// <returns>运算符</returns>
        private List<Item> GetOperators()
        {
            List<Item> list = new List<Item>();
            foreach (var m in Enum.GetNames(typeof(OThinker.Data.ComparisonOperatorType)))
            {
                list.Add(new Item(m, m));
            }
            return list;
        }

        /// <summary>
        /// 获取所包含的的属性
        /// </summary>
        /// <returns>包含的的属性</returns>
        private List<Item> GetColumns()
        {
            List<Item> list = new List<Item>();
            foreach (PropertySchema p in this.Schema.Properties)
            {
                list.Add(new Item(p.Name, p.DisplayName));
            }
            return list;
        }

        /// <summary>
        /// 获取匹配符列表
        /// </summary>
        /// <returns>匹配符列表</returns>
        private List<Item> GetMatchers()
        {
            List<Item> list = new List<Item>();
            list.Add(new Item(OThinker.H3.BizBus.Filter.And.XmlPrefix_And, OThinker.H3.BizBus.Filter.And.XmlPrefix_And));
            list.Add(new Item(OThinker.H3.BizBus.Filter.Or.XmlPrefix_Or, OThinker.H3.BizBus.Filter.Or.XmlPrefix_Or));
            return list;
        }

        /// <summary>
        /// 获取过滤方法列表
        /// </summary>
        /// <returns>过滤方法列表</returns>
        private List<Item> GetFilterMethods()
        {
            List<Item> list = new List<Item>();

            // 加载方法列表
            foreach (H3.DataModel.MethodGroupSchema method in this.Schema.Methods)
            {
                if (method.MethodType == H3.DataModel.MethodType.Filter)
                {
                    list.Add(new Item(method.FullName, method.MethodName));
                }
            }
            return list;
        }
        /// <summary>
        /// 获取轮训次数列表
        /// <returns>轮训次数列表</returns>
        private List<Item> GetRecurrencyTypes()
        {
            List<Item> list = new List<Item>();
            // 初始化所有影射方式
            foreach (string name in Enum.GetNames(typeof(OThinker.H3.DataModel.RecurrencyType)))
            {
                list.Add(new Item("ScheduleInvoker." + name, name));
            }
            return list;
        }
        /// <summary>
        /// 获取定期调用器的状态
        /// </summary>
        /// <returns>定期调用器的状态</returns>
        private List<Item> GetStates()
        {
            List<Item> list = new List<Item>();
            // 初始化所有影射方式
            foreach (string name in Enum.GetNames(typeof(OThinker.H3.DataModel.ScheduleInvokerState)))
            {
                list.Add(new Item("ScheduleInvoker." + name, name));
            }
            return list;
        }

    }
}
