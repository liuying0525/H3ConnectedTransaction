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
    /// 监听实例控制器
    /// </summary>
    [Authorize]
    public class ListenerPolicyController : ControllerBase
    {
        /// <summary>
        /// 获取当前模块权限编码
        /// </summary>
        public override string FunctionCode
        {
            get { return FunctionNode.Category_ProcessModel_Code; }
        }
        #region 参数
        protected string SchemaCode = null;
        private H3.DataModel.BizObjectSchema Schema = null;
        private BizListenerPolicy _ListenerPolicy;
        protected BizListenerPolicy ListenerPolicy
        {
            get
            {
                if (_ListenerPolicy == null)
                {
                    _ListenerPolicy = this.Engine.BizObjectManager.GetListenerPolicy(this.SchemaCode);
                }
                return _ListenerPolicy;
            }
        }
        #endregion
        private ActionResult ParseParam(string schemaCode)
        {
            ActionResult result = new ActionResult();
            this.SchemaCode = schemaCode;

            if (string.IsNullOrEmpty(this.SchemaCode))
            {
                //指定的参数不正确
                result.Message = "ListenerPolicy.Msg2";
                result.Success = false;
                return result;
            }

            this.Schema = this.Engine.BizObjectManager.GetDraftSchema(this.SchemaCode);
            if (this.Schema == null)
            {
                //业务对象模式不存在
                result.Message = "ListenerPolicy.Msg1";
                result.Success = false;
                return result;
            }
            result.Success = true;
            return result;
        }

        /// <summary>
        /// 获取监听实例页面数据
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <returns>监听实例页面数据</returns>
        [HttpGet]
        public JsonResult GetListenerPolicy(string schemaCode, string ownSchemaCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(schemaCode);
                string matcher = string.Empty;
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                List<Item> policyTypes = InitPolicyTypes();
                List<Item> filterMethods = InitFilterMethods();
                List<Item> filters = InitFilters();
                List<Item> fields = InitFields();
                List<Item> operators = InitOperators();
                List<Item> conditions = GetFilter(ListenerPolicy.Filter,out matcher);
                ListenerPolicyViewModel model = new ListenerPolicyViewModel()
                {
                    IntervalSecond = ListenerPolicy.IntervalSecond.ToString(),
                    Filter = matcher,
                    Field = fields.FirstOrDefault().Value.ToString(),
                    Operator = operators.FirstOrDefault().Value.ToString(),
                    SchemaCode = schemaCode,
                    FilterMethod = ListenerPolicy.FilterMethod ?? filterMethods.FirstOrDefault().Value,
                    PolicyType = ListenerPolicy.PolicyType.ToString(),
                    Conditions = conditions.Select(s => s.Value).ToList()
                };
                result.Extend = new
                {
                    PolicyTypes = policyTypes,
                    FilterMethods = filterMethods,
                    Filters = filters,
                    Fields = fields,
                    Conditions = conditions,
                    Operators = operators,
                    ListenerPolicy = model,
                    IsLocked = BizWorkflowPackageLockByID(this.SchemaCode, ownSchemaCode)
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取流程监听器实例列表
        /// </summary>
        /// <param name="schemaCode">数据模型编码</param>
        /// <param name="pageInfo">分页信息</param>
        /// <returns>获取流程监听器实例列表</returns>
        [HttpPost]
        public JsonResult GetListenerPolicyList(string schemaCode, PagerInfo pageInfo)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = ParseParam(schemaCode);
                if (!result.Success) return null;
                return Json(GetPolicyGridData(pageInfo), JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 解析过滤条件匹配单元的集合
        /// </summary>
        /// <param name="filter">过滤条件匹配单元的集合</param>
        /// <param name="matcher">过滤条件的类型:or或and</param>
        /// <returns>解析后的集合</returns>
        private List<Item> GetFilter(OThinker.H3.BizBus.Filter.Filter filter,out string macher)
        {
            List<Item> list = new List<Item>();
            if (filter == null || filter.Matcher == null) {
                macher = "And";
                return list;

            }
            OThinker.H3.BizBus.Filter.MatcherCollection matchers = null;
            Item item;
            if (filter.Matcher is OThinker.H3.BizBus.Filter.And)
            {
                macher = "And";
                matchers = ((OThinker.H3.BizBus.Filter.And)filter.Matcher);
                foreach (OThinker.H3.BizBus.Filter.ItemMatcher matcher in matchers.Matchers)
                {
                    item = new Item();
                    string text = "[{0}] [{1}] [{2}] [{3}]";
                    string[] values = { "ListenerPolicy.And", matcher.ItemName, matcher.ComparisonOperator.ToString(), matcher.Value.ToString() };
                    string value = string.Format("[And][Item][Name]{0}[/Name][Operator]{1}[/Operator][Value]{2}[/Value][/Item][/And]"
                        , matcher.ItemName, matcher.ComparisonOperator, matcher.Value);
                    item.Text = text;
                    item.Value = value;
                    item.Extend = values;
                    list.Add(item);
                }
            }
            else
            {
                macher = "Or";
                matchers = ((OThinker.H3.BizBus.Filter.Or)filter.Matcher);
                foreach (OThinker.H3.BizBus.Filter.ItemMatcher matcher in matchers.Matchers)
                {
                    item = new Item();
                    string text = "[{0}] [{1}] [{2}] [{3}]";
                    string[] values = { "ListenerPolicy.Or", matcher.ItemName, matcher.ComparisonOperator.ToString(), matcher.Value.ToString() };
                    string value = string.Format("[Or][Item][Name]{0}[/Name][Operator]{1}[/Operator][Value]{2}[/Value][/Item][/Or]"
                        , matcher.ItemName, matcher.ComparisonOperator, matcher.Value);
                    item.Text = text;
                    item.Value = value;
                    item.Extend = values;
                    list.Add(item);
                }
            }
            return list;
        }
        /// <summary>
        /// 获取流程监听器实例列表
        /// </summary>
        /// <param name="pageInfo">分页信息</param>
        /// <returns>流程监听器实例</returns>
        private object GetPolicyGridData(PagerInfo pageInfo)
        {
            BizListener[] bizListenerList = this.Engine.BizObjectManager.GetListenersBySchemaCode(this.SchemaCode);
            int total = 0;
            List<BizListenerViewModel> listenerLists = new List<BizListenerViewModel>();
            if (bizListenerList != null)
            {
                total = bizListenerList.Length;
                bizListenerList = bizListenerList.Skip((pageInfo.PageIndex - 1) * pageInfo.PageSize).Take(pageInfo.PageSize).ToArray();
                Messages.AsyncEndMessage message = null;
                foreach (BizListener listener in bizListenerList)
                {
                    message = listener.Message as Messages.AsyncEndMessage;
                    listenerLists.Add(new BizListenerViewModel()
                    {
                        ObjectID = listener.BizObjectId,
                        InstanceID = message.InstanceId,
                        ActivityCode = message.ActivityCode,
                        Condition = listener.Condition,
                        CreatedTime = listener.CreatedTime.ToString("yyyy-MM-d HH:mm:ss")
                    });
                }
            }
            return new { Rows = listenerLists, Total = total };
        }


        private List<Item> InitPolicyTypes()
        {

            List<Item> items = new List<Item>();
            string[] policyTypes = Enum.GetNames(typeof(BizListenerPolicyType));
            if (policyTypes.Length > 0) policyTypes = policyTypes.Where(s => s != BizListenerPolicyType.None.ToString()).Reverse().ToArray();
            items = policyTypes.Select(s => new Item
            {
                Value = s,
                Text = "ListenerPolicy." + s
            }).ToList();
            return items;
        }

        private List<Item> InitFilterMethods()
        {
            List<Item> items = new List<Item>();
            // 过滤方法
            foreach (OThinker.H3.DataModel.MethodGroupSchema method in this.Schema.Methods)
            {
                if (method.MethodType == H3.DataModel.MethodType.Filter)
                {
                    items.Add(new Item(method.FullName, method.MethodName));
                }
            }
            return items;
        }

        private List<Item> InitFilters()
        {
            List<Item> list = new List<Item>() { 
                new Item("ListenerPolicy.And","And"),
                new Item("ListenerPolicy.Or","Or"),
            };
            return list;
        }

        private List<Item> InitFields()
        {
            List<Item> list = new List<Item>();
            foreach (PropertySchema propertity in this.Schema.Properties)
            {
                list.Add(new Item(propertity.DisplayName, propertity.Name));
            }
            return list;
        }

        private List<Item> InitOperators()
        {
            List<Item> list = new List<Item>() { 
                new Item("ListenerPolicy.Equal","Equal"),
                new Item("ListenerPolicy.Below","Below"),
                new Item("ListenerPolicy.NotBelow","NotBelow"),
            };
            return list;
        }

        /// <summary>
        /// 过滤条件
        /// </summary>
        protected string Filter(List<string> conditions)
        {
            string filter = "<Filter>";
            string andXml = "<And>";
            string orXml = "<Or>";
            bool addAnd = false;
            bool addOr = false;
            foreach (string item in conditions)
            {
                if (item.IndexOf("[And]") != -1)
                {
                    addAnd = true;
                    andXml += "[" + item.TrimStart("[And]".ToCharArray()).TrimEnd("[/And]".ToCharArray()) + "]";
                }
                else
                {
                    addOr = true;
                    orXml += "[" + item.TrimStart("[Or]".ToCharArray()).TrimEnd("[/Or]".ToCharArray()) + "]";
                }
            }
            andXml += "</And>";
            orXml += "</Or>";
            filter += (!addAnd ? "" : andXml) + (!addOr ? "" : orXml) + "</Filter>";
            filter = filter.Replace("[", "<").Replace("]", ">");
            return filter;
        }

        [HttpPost]
        public JsonResult SaveListenerPolicy(ListenerPolicyViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);
                BizListenerPolicyType policyType = (BizListenerPolicyType)Enum.Parse(typeof(BizListenerPolicyType), model.PolicyType);
                // 检查输入
                if ((policyType == BizListenerPolicyType.Batch || policyType == BizListenerPolicyType.EventDrivenAndBatch) &&
                    string.IsNullOrEmpty(model.Filter))
                {
                    //请输入过滤条件
                    result.Success = false;
                    result.Message = "ListenerPolicy.Msg4";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                string conditions = model.Conditions == null ? "" : this.Filter(model.Conditions);
                //result.Extend = conditions;
                //return Json(result, JsonRequestBehavior.AllowGet);
                try
                {
                    if (model.Conditions != null && model.Conditions.Count > 0)
                    {
                        OThinker.Data.Convertor.XmlToObject(typeof(H3.BizBus.Filter.Filter), conditions);
                    }
                }
                catch
                {
                    //输入的过滤器格式错误
                    result.Success = false;
                    result.Message = "ListenerPolicy.Msg5";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                // 验证输入是否正确
                int interval = 0;
                if (!int.TryParse(model.IntervalSecond, out interval))
                {
                    //时间间隔必须是正整数
                    result.Success = false;
                    result.Message = "ListenerPolicy.Msg6";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                if (interval < H3.DataModel.Declaration.Filter_MinIntervalSecond)
                {
                    //时间间隔设置过于密集，会造成服务器资源消耗太大
                    result.Success = false;
                    result.Message = "ListenerPolicy.Msg7";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                BizListenerPolicy ListenerPolicy = new BizListenerPolicy();

                ListenerPolicy.PolicyType = policyType;
                ListenerPolicy.IntervalSecond = int.Parse(model.IntervalSecond);
                ListenerPolicy.FilterMethod = model.FilterMethod;
                if (!string.IsNullOrEmpty(conditions))
                {
                    ListenerPolicy.Filter = (H3.BizBus.Filter.Filter)OThinker.Data.Convertor.XmlToObject(typeof(H3.BizBus.Filter.Filter), conditions);
                }

                if (!this.Engine.BizObjectManager.SetListenerPolicy(this.SchemaCode, ListenerPolicy))
                {
                    result.Success = false;
                    //保存失败(可能原因：1数据模型未发布；2数据模型被删除)
                    result.Message = "ListenerPolicy.Msg8";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    this.Engine.BizObjectManager.UpdateDraftSchema(this.Schema);
                    result.Success = true;
                    result.Message = "";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
            });
        }

        /// <summary>
        /// 添加过滤条件
        /// </summary>
        /// <param name="model">监听模型</param>
        /// <returns>过滤条件xml格式</returns>
        [HttpPost]
        public JsonResult AddCondition(ListenerPolicyViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                result = ParseParam(model.SchemaCode);
                if (!result.Success) return Json(result, JsonRequestBehavior.AllowGet);

                if (string.IsNullOrWhiteSpace(model.ConditionValue))
                {
                    result.Success = false;
                    //值必填
                    result.Message = "ListenerPolicy.Msg3";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string text = "[{0}] [{1}] [{2}] [{3}]";
                string[] values = { model.Filter, model.Field, model.Operator, model.ConditionValue };
                string val = string.Format("[{0}][Item][Name]{1}[/Name][Operator]{2}[/Operator][Value]{3}[/Value][/Item][/{0}]", model.Filter, model.Field, model.Operator, model.ConditionValue);
                result.Success = true;
                result.Extend = new
                {
                    Text = text,
                    Values = values,
                    Val = val
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
