using System;
using System.Collections.Generic;
using OThinker.H3.Data;
using System.Reflection;
using OThinker.H3.DataModel;
using System.Data;
using OThinker.H3.BizBus.Filter;
using System.Collections;
using NPOI.SS.UserModel;
using System.IO;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Linq;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// Mvc表单的基类
    /// </summary>
    public class MvcPage : System.Web.UI.Page
    {
        #region 变量 ------------------------------------

        private ActionContext _ActionContext;
        /// <summary>
        /// 环境
        /// </summary>
        public ActionContext ActionContext
        {
            get
            {
                if (this._ActionContext == null)
                {
                    // ERROR, 在页面加载的时候，会执行两次OnLoad方法，如果不能改变为一次，那么这里需要判断一下，是否需要执行new的方法
                    this._ActionContext = new ActionContext(this.Request);
                }
                return this._ActionContext;
            }
        }
        private Instance.InstanceContext _InstanceContext;
        /// <summary>
        /// 流程实例上下文
        /// </summary>
        public Instance.InstanceContext InstanceContext
        {
            get
            {
                return this._InstanceContext;
            }
        }
        private string _assist = "Assist";
        private string _forward = "Forward";

        private MvcController _MvcController = null;
        /// <summary>
        /// 获取 MVC Controller
        /// </summary>
        public MvcController MvcController
        {
            get
            {
                if (this._MvcController == null)
                {
                    this._MvcController = new MvcController(this, this.ActionContext);
                }
                return this._MvcController;
            }
        }

        //mvc前端表单返回的数据
        public MvcPostValue _MvcPost = null;
        /// <summary>
        /// 表单回发后台数据
        /// </summary>
        public MvcPostValue MvcPost
        {
            get
            {
                if (_MvcPost == null)
                {
                    string _MvcPostValue = Request[MvcController.Param_MvcPostValue] + string.Empty;
                    if (!string.IsNullOrEmpty(_MvcPostValue))
                    {
                        _MvcPost = Newtonsoft.Json.JsonConvert.DeserializeObject<MvcPostValue>(_MvcPostValue);
                    }
                }
                return this._MvcPost;
            }
        }
        private string _MobileReturnUrl = string.Empty;
        /// <summary>
        /// 获取移动办公返回地址
        /// </summary>
        public string MobileReturnUrl
        {
            get
            {
                if (this._MobileReturnUrl == string.Empty)
                {
                    if (IsOriginateMode)
                    {
                        this._MobileReturnUrl = AppConfig.GetMobileOriginateInstancesUrl();
                    }
                    else if (IsWorkMode)
                    {
                        this._MobileReturnUrl = AppConfig.GetMobileUnfinishedWorkItemUrl();
                    }
                    else
                    {
                        this._MobileReturnUrl = AppConfig.GetMobileFinishedWorkItemUrl();
                    }
                }
                return this._MobileReturnUrl;
            }
        }

        /// <summary>
        /// 当前表单模式是否是工作模式
        /// </summary>
        public bool IsWorkMode
        {// 注：放在这里是为了避免访问 ActionContext ，构造较大的数据
            get
            {
                return this.SheetMode == SheetMode.Work;
            }
        }

        /// <summary>
        /// 当前表单模式是否是发起模式
        /// </summary>
        public bool IsOriginateMode
        {// 注：放在这里是为了避免访问 ActionContext ，构造较大的数据
            get
            {
                return this.SheetMode == SheetMode.Originate;
            }
        }

        private SheetMode _SheetMode = SheetMode.Unspecified;
        /// <summary>
        /// 获取表单模式
        /// </summary>
        public SheetMode SheetMode
        {// 注：放在这里是为了避免访问 ActionContext ，构造较大的数据
            get
            {
                if (_SheetMode == SheetMode.Unspecified)
                {
                    string strMode = this.Request.QueryString[ActionContext.Param_Mode];
                    if (string.IsNullOrEmpty(strMode))
                    {
                        this._SheetMode = SheetMode.View;
                    }
                    else
                    {
                        this._SheetMode = (SheetMode)Enum.Parse(typeof(SheetMode), strMode);
                    }
                }

                return this._SheetMode;
            }
        }
        #endregion

        #region 构造函数 --------------------------------

        /// <summary>
        /// SheetPage 构造函数
        /// </summary>
        public MvcPage()
        {
        }

        #endregion

        #region 页面加载事件 ----------------------------
        /// <summary>
        /// 页面加载事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnLoad(EventArgs e)
        {
            // TODO:加载表单所有数据，以及表单语言，输出到前端
            string json = string.Empty;
            //try
            //{
            string Command = Request["Command"] + string.Empty;
            if (string.IsNullOrEmpty(Command))
            {
                //Response.End();
                return;
            }

            //返回结果
            MvcResult result = null;
            InitLang();

            switch (Command.ToLower())
            {
                case MvcController.Button_Load:
                    MvcViewContext MvcSheet = this.LoadDataFields();
                    json = Newtonsoft.Json.JsonConvert.SerializeObject(MvcSheet);
                    break;
                case MvcController.Button_Save:
                    // 保存数据
                    if (!string.IsNullOrEmpty(MvcPost.InstanceId))
                    {
                        MvcController.ActionContext.InstanceId = MvcPost.InstanceId;
                    }
                    result = new MvcResult()
                    {
                        Successful = true,
                        IsSaveAction = true,
                        InstanceId = MvcController.ActionContext.InstanceId
                    };
                    this.SaveDataFields(MvcPost, result);
                    break;
                case MvcController.Button_Submit:
                    if (!string.IsNullOrEmpty(MvcPost.InstanceId))
                    {
                        MvcController.ActionContext.InstanceId = MvcPost.InstanceId;
                    }
                    result = new MvcResult()
                    {
                        Successful = true,
                        IsSaveAction = false,
                        InstanceId = MvcController.ActionContext.InstanceId
                    };
                    this.Submit(MvcPost, result);
                    break;
                case MvcController.Button_Reject:
                    result = new MvcResult()
                    {
                        Successful = true,
                        IsSaveAction = false,
                        IsRejectAction = true,
                        InstanceId = MvcController.ActionContext.InstanceId
                    };
                    this.Reject(MvcPost, result);
                    break;
                case MvcController.Button_CancelInstance:
                    // 取消流程
                    result = this.CancelInstance();
                    break;
                case MvcController.Button_FinishInstance:
                    // 结束流程
                    result = new MvcResult()
                    {
                        Successful = true,
                        IsSaveAction = false,
                        InstanceId = MvcController.ActionContext.InstanceId
                    };
                    this.FinishInstance(MvcPost, result);
                    break;
                case MvcController.Button_RetrieveInstance:
                    // 取回流程
                    result = MvcController.DoRetrieveInstance();
                    break;
                case MvcController.Button_LockInstance:
                    // 锁定
                    result = MvcController.LockActivity();
                    break;
                case MvcController.Button_UnLockInstance:
                    // 解锁
                    result = MvcController.UnLockActivity();
                    break;
                default:
                    MethodInfo method = this.GetType().GetMethod(Command);
                    object[] parameters = Newtonsoft.Json.JsonConvert.DeserializeObject<object[]>(Request["Param"]);
                    if (method != null)
                    {
                        try
                        {
                            List<object> paramList = new List<object>();
                            if (parameters != null)
                            {
                                ParameterInfo[] parames = method.GetParameters();
                                int index = 0;
                                foreach (object parameter in parameters)
                                {
                                    if (index == parames.Length) break;
                                    if (parames[index].ParameterType.IsArray)
                                    {
                                        List<string> strs = new List<string>();
                                        if (parameter is Newtonsoft.Json.Linq.JArray)
                                        {
                                            Newtonsoft.Json.Linq.JArray arr = (Newtonsoft.Json.Linq.JArray)parameter;
                                            for (int i = 0; i < arr.Count(); i++)
                                            {
                                                strs.Add(arr[i].ToString());
                                            }
                                            paramList.Add(strs.ToArray());
                                        }
                                        else
                                        {
                                            paramList.Add(null);
                                        }
                                    }
                                    else
                                    {
                                        paramList.Add(parameter);

                                    }
                                    index++;
                                }
                            }
                            //解决SheetTextBox中属性FormatRult展示0.00%的bug
                            //解决方案： 获取数据库中参数，如参数是构造后直接输出，不进行二次构造
                            object res = string.Empty;
                            if (paramList.Count > 0 && paramList.Count == 2 && paramList[1].ToString().IndexOf("%") > -1)
                                res = paramList[1];
                            else
                                res = method.Invoke(this, paramList.ToArray());
                            if (res is MvcResult)
                            {
                                result = (MvcResult)res;
                            }
                            else
                            {
                                //处理json 序列化时时间格式中带T的问题
                                IsoDateTimeConverter timeConverter=new IsoDateTimeConverter{DateTimeFormat="yyyy'-'MM'-'dd HH':'mm':'ss"};
                                json = Newtonsoft.Json.JsonConvert.SerializeObject(res, timeConverter);
                            }
                        }
                        catch (Exception ex)
                        {
                            result = new MvcResult()
                            {
                                Successful = false,
                                Message = string.Format(Configs.Global.ResourceManager.GetString("MvcPage_MehodError"), Command, ex.Message)
                            };
                        }
                    }
                    else
                    {
                        result = new MvcResult()
                        {
                            Successful = false,
                            Message = Configs.Global.ResourceManager.GetString("MvcPage_MehodNotExist") + ":" + Command
                        };
                    }
                    break;
            }

            if (result != null)
            {
                result.MobileReturnUrl = MobileReturnUrl;
            }

            //这个不能为 string.empty 不然，ajax的回调函数不执行
            if (string.IsNullOrWhiteSpace(json))
            {
                json = Newtonsoft.Json.JsonConvert.SerializeObject(result);
            }
            //}
            //catch (Exception ex)
            //{
            //    json = ex.Message;
            //}
            Response.Clear();
            Response.Write(json);
            Response.End();
        }

        /// <summary>
        /// 加载表单数据
        /// </summary>
        public virtual MvcViewContext LoadDataFields()
        {
            // 验证权限
            OThinker.Data.BoolMatchValue customAuthorized = this.ValidateAuthorization();
            if (customAuthorized == OThinker.Data.BoolMatchValue.False ||
                (customAuthorized == OThinker.Data.BoolMatchValue.Unspecified && this.ActionContext.Authorized == OThinker.Data.BoolMatchValue.False))
            {
                MvcViewContext sheet = new MvcViewContext()
                {
                    Message = Configs.Global.ResourceManager.GetString("MvcController_Perission"),
                    Close = true
                };
                return sheet;
            }

            MvcViewContext MvcSheet = this.MvcController.DoPageLoad();

            Dictionary<string, UserOptions> OptionalRecipients = this.GetOptionalRecipients();
            if (OptionalRecipients != null)
            {
                MvcSheet.OptionalRecipients = OptionalRecipients;
            }


            return MvcSheet;
        }

        public void InitLang()
        {
            string Lang = Request["Lang"] + string.Empty;
            if (Lang == "") return;
            string ChineseString = "zh-CN";
            string EnglishString = "en-US";
            string defaultLanguage = ChineseString;
            if (Lang.ToLower() == "en_us")
            {
                defaultLanguage = EnglishString;
            }
            // 设置当前语言环境
            try
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(defaultLanguage);
            }
            catch
            {
            }
        }
        #endregion

        #region 按钮操作 --------------------------------
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="MvcPostValue"></param>
        /// <param name="MvcResult"></param>
        public virtual void SaveDataFields(MvcPostValue MvcPostValue, MvcResult MvcResult)
        {
            this.MvcController.DoSaveAtion(MvcPostValue, MvcResult);
        }

        /// <summary>
        /// 驳回
        /// </summary>
        /// <param name="MvcPostValue"></param>
        /// <param name="MvcResult"></param>
        public virtual void Reject(MvcPostValue MvcPostValue, MvcResult MvcResult)
        {
            // 先保存
            this.SaveDataFields(MvcPostValue, MvcResult);
            MvcResult.Url = string.Empty;
            MvcResult.MobileReturnUrl = string.Empty;
            // 再提交
            this.MvcController.DoReject(MvcPostValue, MvcResult);
        }

        /// <summary>
        /// 结束流程
        /// </summary>
        /// <param name="MvcPostValue"></param>
        /// <param name="MvcResult"></param>
        public virtual void FinishInstance(MvcPostValue MvcPostValue, MvcResult MvcResult)
        {
            this.MvcController.DoFinishInstance(MvcPostValue, MvcResult);
        }

        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="MvcPostValue"></param>
        /// <param name="MvcResult"></param>
        public virtual void Submit(MvcPostValue MvcPostValue, MvcResult MvcResult)
        {
            this.SaveDataFields(MvcPostValue, MvcResult);
            MvcResult.Url = string.Empty;
            MvcResult.MobileReturnUrl = string.Empty;
            // 再提交
            this.MvcController.DoSubmitAction(MvcPostValue, MvcResult);
        }

        #endregion

        #region 取消流程事件 ----------------------------
        /// <summary>
        /// 取消流程事件
        /// </summary>
        public virtual MvcResult CancelInstance()
        {
            return this.MvcController.DoCancelInstance();
        }

        #endregion

        #region 反射接口实现 ----------------------------

        #region 获取主数据的值，返回JSON数组

        /// <summary>
        /// 获取主数据的值，返回JSON数组
        /// </summary>
        /// <param name="Category">数据字典编码</param>
        /// <returns></returns>
        public object GetMetadataByCategory(string Category)
        {
            EnumerableMetadata[] datas = this.ActionContext.Engine.MetadataRepository.GetByCategory(Category);
            if (datas != null)
            {
                var results = from data in datas
                              select new { Code = data.Code, EnumValue = data.EnumValue, IsDefault = data.IsDefault, Sort = data.SortKey };
                return results;
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region 执行业务服务返回前端需要绑定的数据项
        /// <summary>
        /// 执行业务服务返回前端需要绑定的数据项
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="QueryCode"></param>
        /// <param name="Filter">{"查询字段1":"1","查询字段2":"2"}</param>
        /// <param name="DataTextField"></param>
        /// <param name="DataValueField"></param>
        /// <returns></returns>
        public List<object> GetBizServiceData(
            string SchemaCode,
            string QueryCode,
            object Filter,
            string DataTextField,
            string DataValueField)
        {
            // 获取查询对象
            BizQuery query = AppUtility.Engine.BizObjectManager.GetBizQuery(QueryCode);
            // 构造查询条件
            Filter filter = new Filter();
            And and = new And();
            filter.Matcher = and;
            ItemMatcher propertyMatcher = null;
            if (query != null && query.QueryItems != null)
            {
                int i = 0;
                foreach (DataModel.BizQueryItem queryItem in query.QueryItems)
                {
                    // 增加系统参数条件
                    if (queryItem.FilterType == DataModel.FilterType.SystemParam)
                    {
                        propertyMatcher = new OThinker.H3.BizBus.Filter.ItemMatcher(queryItem.PropertyName,
                            OThinker.Data.ComparisonOperatorType.Equal,
                            SheetUtility.GetSystemParamValue(this.ActionContext.User, queryItem.SelectedValues));
                        and.Add(propertyMatcher);
                        continue;
                    }
                    bool hasValue = false;
                    if (Filter != null)
                    {
                        Dictionary<string, string> filterItems = null;
                        filterItems = JsonConvert.DeserializeObject<Dictionary<string, string>>(JsonConvert.SerializeObject(Filter));

                        if (filterItems != null)
                        {
                            foreach (KeyValuePair<string, string> item in filterItems)
                            {
                                if (queryItem.PropertyName == item.Key)
                                {
                                    propertyMatcher = new OThinker.H3.BizBus.Filter.ItemMatcher(item.Key
                                        , queryItem.FilterType == DataModel.FilterType.Contains
                                        ? OThinker.Data.ComparisonOperatorType.Contain
                                        : OThinker.Data.ComparisonOperatorType.Equal, item.Value);
                                    and.Add(propertyMatcher);
                                    hasValue = true;
                                }
                            }
                        }
                    }
                    if (!hasValue && !string.IsNullOrEmpty(queryItem.DefaultValue))
                    {
                        propertyMatcher = new OThinker.H3.BizBus.Filter.ItemMatcher(queryItem.PropertyName,
                            OThinker.Data.ComparisonOperatorType.Equal,
                            queryItem.DefaultValue);
                        and.Add(propertyMatcher);
                    }
                    i++;
                }
            }
            List<object> r = new List<object>();
            DataModel.BizObjectSchema schema = OThinker.H3.Controllers.AppUtility.Engine.BizObjectManager.GetPublishedSchema(SchemaCode);
            Dictionary<string, string> SelectItems = new Dictionary<string, string>();
            if (schema != null)
            {
                DataModel.BizObject[] objs = schema.GetList(
                    this.ActionContext.Engine.Organization,
                    this.ActionContext.Engine.MetadataRepository,
                    this.ActionContext.Engine.BizObjectManager,
                    this.ActionContext.User.UserID,
                    BizObjectSchema.MethodName_GetList,
                    filter);

                DataTable dtSource = DataModel.BizObjectUtility.ToTable(schema, objs);
                foreach (DataRow row in dtSource.Rows)
                {
                    string key = row[DataValueField].ToString();
                    string text = row[DataTextField].ToString();
                    //if (!SelectItems.ContainsKey(key))
                    //{
                    //    SelectItems.Add(key, text);
                    //}
                    r.Add(new { key = key, value = text });
                }
            }
            return r;//SelectItems;
        }
        #endregion

        #region 获取用户属性

        /// <summary>
        /// 获取用户属性
        /// </summary>
        /// <param name="UserID"></param>
        /// <param name="Propertystr">用;隔开</param>
        /// <returns></returns>
        public virtual object GetUserProperty(string UserID, string Propertystr)
        {
            if (string.IsNullOrWhiteSpace(UserID)) { return null; }
            string[] Propertys = Propertystr.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (Propertys == null) { return null; }

            Dictionary<string, object> PropertyJson = new Dictionary<string, object>();


            Organization.Unit unit = this.ActionContext.Engine.Organization.GetUnit(UserID);
            if (unit == null)
            {
                unit = this.ActionContext.Engine.Organization.GetUserByCode(UserID);
                if (unit == null)
                    return null;
            }

            foreach (string p in Propertys)
            {
                if (PropertyJson.ContainsKey(p))
                {
                    continue;
                }
                switch (p)
                {
                    case Organization.User.PropertyName_DepartmentName:
                        if (unit.UnitType == Organization.UnitType.User)
                        {
                            string departmentName = this.ActionContext.Engine.Organization.GetUnit(unit.ParentID).Name;
                            PropertyJson.Add(p, departmentName);
                            //PropertyJson.Add(p, ((OThinker.Organization.User)unit).DepartmentName);
                        }
                        break;
                    default:
                        PropertyInfo PropertyInfo = unit.GetType().GetProperty(p);
                        if (PropertyInfo == null)
                        {
                            PropertyJson.Add(p, null);
                        }
                        else
                        {
                            PropertyJson.Add(p, PropertyInfo.GetValue(unit, null));
                        }
                        break;
                }
            }
            return PropertyJson;
        }

        #endregion

        #region 导入导出Excel的方法

        /// <summary>
        /// 导出表格到 Excel
        /// </summary>
        /// <param name="datatab"></param>
        public string Exportexcel(object datatab, object columnnames)
        {
            List<Dictionary<string, object>> result = Newtonsoft.Json.JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(datatab + string.Empty);
            if (result == null || result.Count == 0)
            {
                //不做操作
            }
            Dictionary<string, string> heads = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(columnnames + string.Empty);

            List<Dictionary<string, object>> datas = new List<Dictionary<string, object>>();
            Dictionary<string, object> data = new Dictionary<string, object>();
            Dictionary<string, string> newheads = new Dictionary<string, string>();

            //此处对参与者和多人参与者导出时候做处理 添加一列对应导出名字，列名的 "名称$" 用于在导入的时候再新添加的多余列移除避免导入异常
            string schemecode = Request.QueryString["SheetCode"].TrimStart('S');
            BizObjectSchema Bos = this.ActionContext.Engine.BizObjectManager.GetPublishedSchema(schemecode);
            var bizObjectArrays = Bos.Properties.Where(w => w.LogicType == OThinker.H3.Data.DataLogicType.BizObjectArray);
            for (int i = 0; i < result.Count; i++)
            {
                data = new Dictionary<string, object>();
                foreach (string key in heads.Keys)
                {
                    if (!newheads.ContainsKey(key))
                    {
                        newheads.Add(key, heads[key]);
                    }
                    data.Add(key, result[i][key]);
                    if (bizObjectArrays != null)
                    {
                        foreach (var item in bizObjectArrays)
                        {
                            //当数据源存在多个子表的时候，判断子表是否存在这个列
                            if (item.ChildSchema.GetProperty(key) == null)
                            {
                                continue;
                            }
                            var type = item.ChildSchema.GetProperty(key).LogicType;
                            //处理时间段类型
                            if (type == OThinker.H3.Data.DataLogicType.TimeSpan)
                            {
                                if (result[i][key] != null)
                                {
                                    data.Remove(key); 
                                    string str = result[i][key].ToString();
                                    string[] temostrs = null;
                                    if (str.Contains("$"))
                                    {
                                        temostrs = str.Split('$');
                                        string[] EndStr = temostrs[1].Split(':');
                                        str = temostrs[0] + "天" + EndStr[0]+"小时"+ EndStr[1]+"分钟"+ EndStr[2]+ "秒";
                                    } 
                                    data.Add(key, str);

                                }
                            }
                            if (type == OThinker.H3.Data.DataLogicType.SingleParticipant)
                            {
                                if (!newheads.ContainsKey(key + "$"))
                                {
                                    newheads.Add(key + "$", heads[key] + "名称$");
                                }
                                string TempResult = string.Empty;
                                if (result[i][key] != null)
                                {
                                    TempResult = this.ActionContext.Engine.Organization.GetName(result[i][key].ToString());
                                    data.Add(key + "$", TempResult);
                                }
                                break;
                            }
                            if (type == OThinker.H3.Data.DataLogicType.MultiParticipant)
                            {
                                if (!newheads.ContainsKey(key + "$"))
                                {
                                    newheads.Add(key + "$", heads[key] + "名称$");
                                }
                                string TempResult = string.Empty;
                                if (result[i][key] != null)
                                {
                                    System.Web.Script.Serialization.JavaScriptSerializer JavaScriptSerializer = new System.Web.Script.Serialization.JavaScriptSerializer();
                                    string[] Ids = JavaScriptSerializer.Deserialize<string[]>(result[i][key].ToString());
                                    if (Ids != null)
                                    {
                                        Dictionary<string, string> UnitNames = this.ActionContext.Engine.Organization.GetNames(Ids);
                                        TempResult = string.Join(",", UnitNames.Values);
                                    }
                                    data.Add(key + "$", TempResult);
                                }
                                break;
                            }
                        }
                    }
                }
                datas.Add(data);
            }

            string FileUrl = ExportExcel.ExportExecl("ExcelTemplete" + DateTime.Now.Ticks.ToString(), newheads, datas);

            return FileUrl;
        }

        #endregion

        #region 转发 转办

        /// <summary>
        /// 转发 转办
        /// </summary>
        /// <param name="UserID"></param>
        /// <returns></returns>
        //public virtual MvcResult Forward(string UserID, string UserComment)
        //{
        //    MvcResult MvcResult = new MvcResult();
        //    if (string.IsNullOrEmpty(UserID))
        //        return MvcResult;
        //    //
        //    bool IsDelegant = false;//判断是否为委托人转发
        //    if (this.ActionContext.WorkItem.Delegant != null && this.ActionContext.WorkItem.Delegant == this.ActionContext.User.UserID)
        //    {
        //        IsDelegant = true;
        //    }
        //    if (this.ActionContext.Engine.WorkItemManager.HandOverWorkItem(this.ActionContext.WorkItemID, UserID) == OThinker.H3.ErrorCode.SUCCESS)
        //    {
        //        Comment comment = new Comment()
        //        {
        //            ParentObjectID = this.ActionContext.WorkItem.ObjectID,
        //            ObjectID = Guid.NewGuid().ToString(),
        //            BizObjectSchemaCode = this.ActionContext.BizObject.Schema.SchemaCode,
        //            BizObjectId = this.ActionContext.BizObjectID,
        //            InstanceId = this.ActionContext.InstanceId,
        //            Activity = this.ActionContext.ActivityCode,
        //            UserID = this.ActionContext.User.UserID,
        //            UserName = this.ActionContext.User.UserName,
        //            OUName = this.ActionContext.User.DepartmentName,
        //            TokenId = this.ActionContext.WorkItem.TokenId,
        //            DataField = GetActivityCommentDataField(MvcController.SheetForwardComment),// "Sheet__Forward",
        //            Text = string.IsNullOrEmpty(UserComment) ? "" : UserComment,
        //            Approval = OThinker.Data.BoolMatchValue.True,
        //            CreatedTime = DateTime.Now,
        //            ModifiedBy = this.ActionContext.User.UserID,
        //            WorkItemId = this.ActionContext.WorkItemID,
        //            ParentPropertyText = Configs.Global.ResourceManager.GetString("SheetActionPane_Forward") + " " + this.ActionContext.Engine.Organization.GetName(UserID),
        //            ItemType = (int)WorkItem.WorkItemType.ActivityForward
        //        };
        //        this.ActionContext.Engine.BizObjectManager.AddComment(comment);
        //        // 记录操作日志
        //        this.MvcController.AddUserLog(Tracking.UserLogType.Forward);
        //    }
        //    //UpdateFrequentUser(this.ActionContext.User.UserID, new string[] { UserID });
        //    MvcResult.Successful = true;
        //    MvcResult.ClosePage = true;
        //    return MvcResult;
        //}

        public virtual MvcResult Forward(string UserID)
        {

            MvcResult MvcResult = new MvcResult();
            if (string.IsNullOrEmpty(UserID))
                return MvcResult;
            //
            bool IsDelegant = false;//判断是否为委托人转发
            if (this.ActionContext.WorkItem.Delegant != null && this.ActionContext.WorkItem.Delegant == this.ActionContext.User.UserID)
            {
                IsDelegant = true;
            }
            if (this.ActionContext.Engine.WorkItemManager.ForwardWorkItem(this.ActionContext.WorkItemID, UserID) == OThinker.H3.ErrorCode.SUCCESS)
            { 
                // 记录操作日志
                this.MvcController.AddUserLog(Tracking.UserLogType.Forward);
            }
            //UpdateFrequentUser(this.ActionContext.User.UserID, new string[] { UserID });
            MvcResult.Successful = true;
            MvcResult.ClosePage = true;
            return MvcResult;
        }

        /// <summary>
        /// 获取节点的审批意见
        /// </summary>
        /// <param name="defaultDataField"></param>
        /// <returns></returns>
        private string GetActivityCommentDataField(string defaultDataField)
        {
            string datafield = this.ActionContext.Workflow.GetDefaultCommentDataItem(this.ActionContext.Schema, this.ActionContext.ActivityCode);
            if (string.IsNullOrEmpty(datafield)) return defaultDataField;
            return datafield;
        }
        #endregion

        #region 协办
        /// <summary>
        /// 协办
        /// </summary>
        /// <param name="UserIDs"></param>
        /// <param name="MonitorConsultant"></param>
        /// <returns></returns>
        public virtual MvcResult Assist(string UserIDs, bool MonitorConsultant, string UserComment)
        {
            MvcResult MvcResult = new MvcResult();

            string[] circulatedItems = null;
            if ((circulatedItems = this.ActionContext.Engine.WorkItemManager.AssistWorkItem(
               this.ActionContext.User.UserID,
               this.ActionContext.WorkItem.WorkItemID,
               UserIDs.Split(','),
               this.ActionContext.User.UserName + Configs.Global.ResourceManager.GetString("SheetActionPane_Assist") + this.ActionContext.WorkItem.DisplayName,
               MonitorConsultant,
               OThinker.H3.WorkItem.ReturnToType.ReturnBack)) != null && circulatedItems.Length != 0)
            {
                if (!string.IsNullOrEmpty(UserComment))
                {
                    string[] users = UserIDs.Split(',');
                    List<OThinker.Organization.Unit> units = this.ActionContext.Engine.Organization.GetUnits(users);
                    string userstring = string.Empty;

                    for (int i = 0; i < circulatedItems.Length; i++)
                    {
                        WorkItem.WorkItem _workitem = this.ActionContext.Engine.WorkItemManager.GetWorkItem(circulatedItems[i]);
                        if (_workitem != null) userstring += _workitem.ParticipantName + ";";
                    }
                    if (!string.IsNullOrEmpty(userstring))
                    {
                        Comment comment = new Comment()
                        {
                            ParentObjectID = this.ActionContext.WorkItem.ObjectID,
                            ObjectID = Guid.NewGuid().ToString(),
                            BizObjectSchemaCode = this.ActionContext.BizObject.Schema.SchemaCode,
                            BizObjectId = this.ActionContext.BizObjectID,
                            InstanceId = this.ActionContext.InstanceId,
                            Activity = this.ActionContext.ActivityCode,
                            UserID = this.ActionContext.User.UserID,
                            OUName = this.ActionContext.User.DepartmentName,
                            TokenId = this.ActionContext.WorkItem.TokenId,
                            DataField = GetActivityCommentDataField(MvcController.SheetAssistComment),// "Sheet__Assist",
                            Text = string.IsNullOrEmpty(UserComment) ? "" : UserComment,
                            Approval = OThinker.Data.BoolMatchValue.True,
                            CreatedTime = DateTime.Now,
                            UserName = this.ActionContext.User.UserName,
                            ModifiedBy = this.ActionContext.User.UserID,
                            WorkItemId = this.ActionContext.WorkItemID,
                            ParentPropertyText = Configs.Global.ResourceManager.GetString("SheetActionPane_Assist") + " " + userstring,
                            ItemType = (int)WorkItem.WorkItemType.WorkItemAssist
                        };
                        this.ActionContext.Engine.BizObjectManager.AddComment(comment);
                        MvcResult.Successful = true;
                    }
                    else
                    {
                        MvcResult.Successful = false;
                    }
                }
                // 记录操作日志
                this.MvcController.AddUserLog(Tracking.UserLogType.Assist);
            }
            return MvcResult;
        }
        #endregion

        #region 征询
        /// <summary>
        /// 征询意见
        /// </summary>
        /// <param name="UserIDs">用户ID</param>
        /// <param name="MonitorConsultant">是否监控征询意见结束事件</param>
        /// <returns></returns>
        public virtual MvcResult Consult(string UserIDs, bool MonitorConsultant, string UserComment)
        {
            MvcResult MvcResult = new MvcResult();
            MvcResult.Successful = false;
            // 获得选中的用户
            string[] selectedUsers = UserIDs.Split(',');
            List<string> consultants = this.ActionContext.Engine.Organization.GetMembers(selectedUsers, Organization.State.Active);

            // 获得已经征询过意见的人员
            string[] consultedConsultants = this.ActionContext.Engine.Query.QueryConsultants(this.ActionContext.WorkItem.WorkItemID);
            System.Collections.ArrayList consultantList = new ArrayList();
            if (consultants != null)
            {
                foreach (string consultant in consultants)
                {
                    bool exist = false;
                    // 检查是否在已被征询过人里面
                    if (consultedConsultants != null)
                    {
                        foreach (string consulted in consultedConsultants)
                        {
                            if (consultant == consulted)
                            {
                                exist = true;
                                break;
                            }
                        }
                    }
                    if (exist)
                    {
                        continue;
                    }
                    else
                    {
                        consultantList.Add(consultant);
                    }
                }
            }

            string[] newConsultants = OThinker.Data.ArrayConvertor<string>.ToArray(consultantList);
            string[] consultWorkItems = null;
            // 征询意见任务的显示名称
            string displayName =
                this.ActionContext.User.UserName +
                Configs.Global.ResourceManager.GetString("SheetActionPane_Consult") + ": " +
                this.ActionContext.WorkItem.DisplayName;
            // 添加征询任务
            if ((consultWorkItems = this.ActionContext.Engine.WorkItemManager.ConsultWorkItem(
                this.ActionContext.User.UserID,
                this.ActionContext.WorkItem.WorkItemID,
                newConsultants,
                displayName,
                string.IsNullOrEmpty(UserComment) ? "" : UserComment,
                MonitorConsultant)) != null && consultWorkItems.Length != 0)
            {
                MvcResult.Successful = true;
                // 记录操作日志
                this.MvcController.AddUserLog(Tracking.UserLogType.Consult);
            }

            return MvcResult;
        }

        #endregion

        #region 传阅
        /// <summary>
        /// 传阅
        /// </summary>
        /// <param name="UserIDs"></param>
        /// <param name="Recirculate"></param>
        /// <returns></returns>
        public virtual MvcResult Circulate(string UserIDs, bool Recirculate)
        {
            MvcResult MvcResult = new MvcResult();
            List<string> circulateUsers = null;
            string[] selectedUsers = UserIDs.Split(',');

            Organization.Unit unit = null;
            circulateUsers = new List<string>();
            bool overMax = false;
            foreach (string unitId in selectedUsers)
            {
                unit = this.ActionContext.Engine.Organization.GetUnit(unitId);
                if (unit == null)
                {
                    continue;
                }
                else if (unit.UnitType == Organization.UnitType.User)
                {
                    circulateUsers.Add(unit.ObjectID);
                    if (circulateUsers.Count > 500) overMax = true;
                }
                else
                {
                    // 递归获取该 OU 下所有人员 
                    if (!GetUnitUsers(unit, circulateUsers))
                    {
                        overMax = true;
                    }
                }
            }
            if (overMax)
            {
                MvcResult.Successful = false;
                MvcResult.Errors.Add("超过最大限制数：500，请联系管理员操作！");
                return MvcResult;
            }

            string[] circulatedItems = null;
            if (this.ActionContext.WorkItem != null)
            {
                if ((circulatedItems = this.ActionContext.Engine.WorkItemManager.Circulate(
                this.ActionContext.User.UserID,
                this.ActionContext.WorkItem.WorkItemID,
                circulateUsers.ToArray(),//selectedUsers,
                Recirculate,
                this.ActionContext.User.UserName + Configs.Global.ResourceManager.GetString("SheetActionPane_Circulate") + ":"
                + (this.ActionContext.WorkItem.DisplayName == "" ? this.ActionContext.WorkItem.ActivityCode : this.ActionContext.WorkItem.DisplayName))) != null
                && circulatedItems.Length != 0)
                {
                    MvcResult.Successful = true;
                    // 记录操作日志
                    this.MvcController.AddUserLog(Tracking.UserLogType.Circulate);
                }
            }
            else
            {

                if ((circulatedItems = this.ActionContext.Engine.WorkItemManager.Circulate(
                this.ActionContext.User.UserID,
                this.ActionContext.CirculateItem.ObjectID,
                circulateUsers.ToArray(),//selectedUsers,
                Recirculate,
                this.ActionContext.User.UserName + Configs.Global.ResourceManager.GetString("SheetActionPane_Circulate") + ":" + this.ActionContext.CirculateItem.DisplayName)) != null
                && circulatedItems.Length != 0)
                {
                    MvcResult.Successful = true;
                    // 记录操作日志
                    this.MvcController.AddUserLog(Tracking.UserLogType.Circulate);
                }
            }


            return MvcResult;
        }

        /// <summary>
        /// 获取选择OU下的所有用户
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="circulateUsers"></param>
        private bool GetUnitUsers(Organization.Unit unit, List<string> circulateUsers)
        {
            List<string> users = this.ActionContext.Engine.Organization.GetChildren(unit.ObjectID, Organization.UnitType.User, false, Organization.State.Active);
            foreach (string user in users)
            {
                circulateUsers.Add(user);
                if (circulateUsers.Count > 500) return false;
            }

            List<Organization.Unit> units = this.ActionContext.Engine.Organization.GetChildUnits(unit.ObjectID, Organization.UnitType.OrganizationUnit, false, Organization.State.Active);
            foreach (Organization.Unit u in units)
            {
                if (!GetUnitUsers(u, circulateUsers))
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region 调整活动参与者
        /// <summary>
        /// 调整活动参与者
        /// </summary>
        /// <param name="UserIDs"></param>
        /// <returns></returns>
        public virtual MvcResult AdjustParticipant(string UserIDs)
        {
            MvcResult MvcResult = new MvcResult();
            string ActivityCode = this.ActionContext.ActivityCode;
            // 新用户
            System.Collections.Generic.List<string> newUserList = new System.Collections.Generic.List<string>();
            // 原有用户
            string[] oldUsers = null;
            Instance.IToken[] tokens = this.ActionContext.InstanceContext.GetTokens(ActivityCode, Instance.TokenState.Running);
            if (tokens != null && tokens.Length > 0)
            {
                oldUsers = this.ActionContext.Engine.Query.QueryParticipants(
                    this.ActionContext.InstanceId,
                    tokens[0].TokenId,
                    OThinker.H3.WorkItem.WorkItemType.Participative,
                    OThinker.H3.WorkItem.WorkItemState.Unfinished);
            }
            System.Collections.Generic.List<string> oldUserList = new System.Collections.Generic.List<string>();
            if (oldUsers != null)
            {
                oldUserList = new System.Collections.Generic.List<string>(oldUsers);
            }

            // 当前选中的用户
            string[] currentUsers = UserIDs.Split(',');
            // 获得新添加的用户
            if (currentUsers == null || currentUsers.Length == 0) return MvcResult;
            foreach (string user in currentUsers)
            {
                if (!newUserList.Contains(user))
                {
                    newUserList.Add(user);
                }
            }
            // 添加旧的用户
            foreach (string user in oldUserList)
            {
                if (!newUserList.Contains(user))
                {
                    newUserList.Add(user);
                }
            }

            string[] finishedUsers = null;
            Instance.IToken[] finishedTokens = this.ActionContext.InstanceContext.GetTokens(ActivityCode, Instance.TokenState.Finished);
            if (finishedTokens != null && finishedTokens.Length > 0)
            {
                finishedUsers = this.ActionContext.Engine.Query.QueryParticipants(
                    this.ActionContext.InstanceId,
                    this.ActionContext.WorkItem.TokenId,
                    OThinker.H3.WorkItem.WorkItemType.Participative,
                    OThinker.H3.WorkItem.WorkItemState.Finished);
            }
            System.Collections.Generic.List<string> finishedList = new System.Collections.Generic.List<string>();
            if (finishedUsers != null)
            {
                finishedList = new System.Collections.Generic.List<string>(finishedUsers);
            }

            System.Collections.Generic.List<string> authUsers = new System.Collections.Generic.List<string>(newUserList);
            foreach (string user in authUsers)
            {
                if (finishedList.Contains(user))
                {
                    newUserList.Remove(user);
                }
            }
            //Error:SpsSite SpsWebName SpsList SpsListItemId 没有这些属性
            // 为被征询意见的用户添加SPS权限
            //this.SpsAuth.Authrize(authUsers.ToArray(), this.context.WorkflowCode, this.context.WorkflowVersion, this.context.InstanceId, this.context.SpsSite, this.context.SpsWebName, this.context.SpsList, this.context.SpsListItemId);

            int tokenId = this.ActionContext.InstanceContext.GetLastToken(ActivityCode).TokenId;
            OThinker.H3.Messages.AdjustParticipantMessage message = new OThinker.H3.Messages.AdjustParticipantMessage(
                OThinker.H3.Messages.MessageEmergencyType.Normal,
                this.ActionContext.InstanceId,
                ActivityCode,
                newUserList.ToArray(),
                tokenId);
            this.ActionContext.Engine.InstanceManager.SendMessage(message);

            // 记录操作日志
            this.MvcController.AddUserLog(Tracking.UserLogType.AdjustParticipant);

            MvcResult.ClosePage = true;
            return MvcResult;
        }
        #endregion

        #region 获取格式化信息
        /// <summary>
        /// 获取格式化信息
        /// </summary>
        /// <param name="Format"></param>
        /// <param name="InputValue"></param>
        /// <returns></returns>
        public string GetFormatValue(string Format, string InputValue)
        {
            decimal inputValue = 0m;
            decimal.TryParse(InputValue, out inputValue);

            return string.Format(Format, inputValue);
        }
        #endregion

        /// <summary>
        /// 获取附件信息
        /// </summary>
        /// <param name="SchemaCode"></param>
        /// <param name="BizObjectID"></param>
        /// <param name="AttachmentIDs"></param>
        /// <returns></returns>
        public AttachmentHeader[] GetAttachmentHeader(string SchemaCode, string BizObjectID, string AttachmentIDs)
        {
            string[] idArray = AttachmentIDs.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
            if (idArray == null || idArray.Length == 0) return null;
            AttachmentHeader[] headers = this.ActionContext.Engine.BizObjectManager.GetAttachmentHeaders(SchemaCode, BizObjectID, idArray);
            return headers;

        }
        #endregion

        #region 检查用户是否具有操作权限 ----------------
        /// <summary>
        /// 检查用户是否具有操作权限
        /// </summary>
        /// <returns></returns>
        public virtual OThinker.Data.BoolMatchValue ValidateAuthorization()
        {
            return this.MvcController.ValidateAuthorization();
        }
        #endregion

        #region 如果要征询意见/协办/传阅，那么可选的征询意/协办/传阅的人员由这里获得
        /// <summary>
        /// 如果要征询意见/协办/传阅，那么可选的征询意/协办/传阅的人员由这里获得
        /// </summary>
        /// <returns></returns>
        public virtual Dictionary<string, UserOptions> GetOptionalRecipients()
        {
            return null;
        }
        #endregion
    }
}