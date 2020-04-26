using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using System.Web.UI.WebControls;
using OThinker.Data.Database;
using OThinker.H3.Data;
using OThinker.H3.Settings;
using Newtonsoft.Json;
using System.Web;
using OThinker.H3.DataModel;
using OThinker.H3.Acl;
using OThinker.H3.BizBus.BizService;

namespace OThinker.H3.Controllers.Controllers.BizBus
{
    public class BizServiceController : ControllerBase
    {
        public BizServiceController() { }

        public override string FunctionCode
        {
            get { return FunctionNode.BizBus_BizService_Code; }
        }

        /// <summary>
        /// 获取当前注册授权类型
        /// </summary>
        public OThinker.H3.Configs.LicenseType LicenseType
        {
            get
            {
                return (OThinker.H3.Configs.LicenseType)this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_LicenseType];
            }
        }

        /// <summary>
        /// 获取业务集成是否允许使用
        /// </summary>
        public bool BizBusAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizBus] + string.Empty);
            }
        }
        /// <summary>
        /// 获取业务规则是否允许使用
        /// </summary>
        public bool BizRuleAuthorized
        {
            get
            {
                if (LicenseType == Configs.LicenseType.Develop) return true;
                return bool.Parse(this.UserValidator.PortalSettings[OThinker.H3.Configs.License.PropertyName_BizRule] + string.Empty);
            }
        }


        #region 创建业务服务
        /// <summary>
        /// 初始化服务，并返回对应的属性集合
        /// </summary>
        /// <param name="bizAdapterCode"></param>
        /// <param name="serviceCode"></param>
        /// <param name="dbCode"></param>
        /// <param name="tableName"></param>
        ///  /// <param name="createNew">是否创建新的属性值，不是加载旧的</param>
        /// <returns></returns>
        public JsonResult InitServiceSetting(string bizAdapterCode, string serviceCode, string dbCode, string tableName, bool createNew)
        {
            return ExecuteFunctionRun(() =>
            {
                H3.BizBus.BizService.BizService service = CreateService(bizAdapterCode, serviceCode);

                if (string.IsNullOrEmpty(serviceCode) || createNew)
                {
                    Dictionary<string, DataColumn> columns = null;
                    string paramfix = null;
                    try
                    {
                        Dictionary<string, DataColumn> _columns = this.Engine.SettingManager.GetBizDbTableColumns(dbCode, tableName);
                        columns = DataLogicTypeConvertor.FilterBizObjectSupportType(_columns);
                        BizDbConnectionConfig conn = this.Engine.SettingManager.GetBizDbConnectionConfig(dbCode);
                        paramfix = OThinker.Data.Database.Database.GetParameterFlag(conn.DbType);
                    }
                    catch
                    {
                        // 可能数据库无法连接
                        //this.ShowWarningMessage(this.PortalResource.GetString("CreateDbTableBizService_MsgUnable"));
                        return null;
                    }

                    // 主键的条件
                    string condition = null;
                    foreach (string columnName in columns.Keys)
                    {
                        DataColumn column = columns[columnName];
                        if (column.IsPrimaryKey)
                        {
                            if (!string.IsNullOrEmpty(condition))
                            {
                                condition += " AND ";
                            }
                            condition += column.Name + "=" + paramfix + column.Name;
                        }
                    }

                    // 创建对应的SQL
                    string select = "SELECT * FROM " + tableName + " WHERE " + condition;
                    System.Text.StringBuilder nameBuilder = new System.Text.StringBuilder();
                    System.Text.StringBuilder valueBuilder = new System.Text.StringBuilder();
                    System.Text.StringBuilder setBuilder = new System.Text.StringBuilder();
                    System.Text.StringBuilder orderByBuilder = new System.Text.StringBuilder();
                    List<string> colKeys = new List<string>();
                    foreach (string name in columns.Keys)
                    {
                        OThinker.Data.Database.DataColumn column = columns[name];
                        if (nameBuilder.Length != 0)
                        {
                            nameBuilder.Append(", ");
                            valueBuilder.Append(", ");
                            setBuilder.Append(", ");

                            //orderByBuilder.Append(",");
                        }
                        setBuilder.Append(column.Name + "=" + paramfix + column.Name);
                        nameBuilder.Append(column.Name);
                        valueBuilder.Append(paramfix + column.Name);
                        colKeys.Add(column.Name);
                    }
                    if (colKeys != null && colKeys.Count > 0)
                        orderByBuilder.Append(colKeys.ToArray()[0] + " ASC");

                    string update = "UPDATE " + tableName + " SET " + setBuilder.ToString() + " WHERE " + condition;
                    string insert = "INSERT INTO " + tableName + " (" + nameBuilder.ToString() + ") VALUES (" + valueBuilder.ToString() + ")";
                    string delete = "DELETE FROM " + tableName + " WHERE " + condition;
                    string upsert = "IF EXISTS (SELECT 1 FROM " + tableName + " WHERE " + condition + ") " + update + " ELSE " + insert;
                    string orderBy = "ORDER BY " + orderByBuilder.ToString();

                    // 初始化其中的属性
                    OThinker.H3.BizBus.BizService.BizServiceSetting[] settings = service.Settings;
                    if (settings != null)
                    {
                        foreach (OThinker.H3.BizBus.BizService.BizServiceSetting setting in settings)
                        {
                            switch (setting.SettingName)
                            {
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_DbCode:
                                    service.SetSettingValue(setting.SettingName, dbCode);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_TableName:
                                    service.SetSettingValue(setting.SettingName, tableName);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_InsertSql:
                                    service.SetSettingValue(setting.SettingName, insert);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_SelectSql:
                                    service.SetSettingValue(setting.SettingName, select);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_UpdateSql:
                                    service.SetSettingValue(setting.SettingName, update);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_UpsertSql:
                                    service.SetSettingValue(setting.SettingName, upsert);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_DeleteSql:
                                    service.SetSettingValue(setting.SettingName, delete);
                                    break;
                                case OThinker.H3.BizBus.Declaration.DbTableAdapter_OrderBySql:
                                    service.SetSettingValue(setting.SettingName, orderBy);
                                    break;
                            }
                        }
                    }
                }
                List<BizServiceSettingViewModel> serviceSettings = GetSettingFromService(service);
                //获取属性
                return Json(serviceSettings, JsonRequestBehavior.AllowGet);

            });
        }


        /// <summary>
        /// 获取Table列表
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <returns></returns>
        public JsonResult GetTableList(string dbCode)
        {
            return ExecuteFunctionRun(() =>
            {
                //创建服务
                string[] tableNames = this.Engine.SettingManager.GetBizDbTableNames(dbCode);
                string[] views = this.Engine.SettingManager.GetBizDbViewNames(dbCode);

                List<ListItem> lists = new System.Collections.Generic.List<ListItem>();
                if (tableNames != null)
                {
                    Array.Sort(tableNames);
                    foreach (string tableName in tableNames)
                    {
                        lists.Add(new ListItem(tableName, tableName));
                    }
                }
                if (views != null)
                {
                    Array.Sort(views);
                    foreach (string view in views)
                    {
                        lists.Add(new ListItem(view, view));
                    }
                }

                return Json(lists, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取服务属性
        /// </summary>
        /// <param name="bizAdapterCode">适配器</param>
        /// <param name="serviceCode">服务编码，可以为空</param>
        /// <returns></returns>
        public JsonResult GetBizServiceSetting(string bizAdapterCode, string serviceCode)
        {
            return ExecuteFunctionRun(() =>
            {

                H3.BizBus.BizService.BizService service = CreateService(bizAdapterCode, serviceCode);

                List<BizServiceSettingViewModel> setting = GetSettingFromService(service);
                //获取属性
                return Json(setting, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存，更新
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public JsonResult SaveBizService(string formData)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                bool Creating = false;
                if (!this.BizBusAuthorized)
                {
                    result.Message = "BizService.NoAuthorized";
                    result.Success = false;
                    return Json(result);
                }

                BizServiceViewModel model = JsonConvert.DeserializeObject<BizServiceViewModel>(formData);

                //判断是更新还是新增
                if (string.IsNullOrEmpty(model.ObjectID)) { Creating = true; }

                H3.BizBus.BizService.BizService service = CreateService(model.Class, Creating ? "" : model.Code);

                // 更新属性
                if (Creating)
                {
                    if (string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.ParentCode))
                    {

                        result.Message = "BizService.Msg4";
                        result.Success = false;
                        return Json(result);
                    }

                    //业务服务编码必须以字母开始，不让创建到数据库表字段时报错
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                    if (!regex.Match(model.Code).Success)
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchemaProperty.Msg4";
                        return Json(result);
                    }

                    service.Code = model.Code;
                    service.FolderCode = model.ParentCode;
                }

                if (string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    result.Message = "BizService.InvalidName";
                    result.Success = false;
                    return Json(result);
                }

                service.DisplayName = model.DisplayName;
                service.Description = model.Description;
                service.VersionNo = model.Version;


                // 属性
                foreach (BizServiceSettingViewModel item in model.Settings)
                {
                    string name = item.SettingName;
                    string value = item.SettingValue;
                    service.SetSettingValue(name, value);
                }

                string r = "";
                bool isValidate = true;

                if (this.GetNotVerifiedAdapterCodes().Any(s => s == service.BizAdapterCode)) { isValidate = false; }
                //保存/更新
                if (Creating)
                {
                    r = this.Engine.BizBus.AddBizService(service, isValidate).ToString();
                }
                else
                {
                    r = this.Engine.BizBus.UpdateBizService(service, isValidate).ToString();
                }

                if (!string.IsNullOrEmpty(r) && !r.StartsWith("警告"))
                {
                    result.Message = "BizService.Msg2";
                    result.Extend = r;
                    result.Success = false;
                }
                else
                {
                    result.Message = r.StartsWith("警告") ? r : "msgGlobalString.SaveSuccess";
                    result.Success = true;
                    result.Extend = service;
                }

                return Json(result);
            });
        }

        /// <summary>
        /// 获取服务信息
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public JsonResult GetBizServiceInfo(string serviceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                H3.BizBus.BizService.BizService service = CreateService("", serviceCode);
                BizServiceViewModel model = new BizServiceViewModel();
                model.ObjectID = service.ObjectID;
                model.Class = service.BizAdapterCode;
                model.Code = service.Code;
                model.Description = service.Description;
                model.DisplayName = service.DisplayName;
                model.Version = service.VersionNo;
                model.Settings = GetSettingFromService(service);
                if (model.Settings != null)
                {
                    foreach (BizServiceSettingViewModel m in model.Settings)
                    {
                        if (m.SettingName == "DbCode") { model.DbCode = m.SettingValue; continue; }
                        if (m.SettingName == "TableName") { model.TableName = m.SettingValue; }
                    }
                }
                return Json(model, JsonRequestBehavior.AllowGet);
            });
        }

        //检验 
        public JsonResult ValidateBizService(string formData)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                bool Creating = false;
                if (!this.BizBusAuthorized)
                {
                    result.Message = "BizService.NoAuthorized";
                    result.Success = false;
                    return Json(result);
                }

                BizServiceViewModel model = JsonConvert.DeserializeObject<BizServiceViewModel>(formData);

                //判断是更新还是新增
                if (string.IsNullOrEmpty(model.ObjectID)) { Creating = true; }

                H3.BizBus.BizService.BizService service = CreateService(model.Class, Creating ? "" : model.Code);

                // 更新属性
                if (Creating)
                {
                    if (string.IsNullOrEmpty(model.Code) || string.IsNullOrEmpty(model.ParentCode))
                    {

                        result.Message = "BizService.Msg4";
                        result.Success = false;
                        return Json(result);
                    }

                    //业务服务编码必须以字母开始，不让创建到数据库表字段时报错
                    System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("^[a-zA-Z\\u4e00-\\u9fa5][0-9a-zA-Z\\u4e00-\\u9fa5_]*$");
                    if (!regex.Match(model.Code).Success)
                    {
                        result.Success = false;
                        result.Message = "EditBizObjectSchemaProperty.Msg4";
                        return Json(result);
                    }

                    service.Code = model.Code;
                    service.FolderCode = model.ParentCode;
                }

                if (string.IsNullOrWhiteSpace(model.DisplayName))
                {
                    result.Message = "BizService.InvalidName";
                    result.Success = false;
                    return Json(result);
                }




                H3.BizBus.BizService.BizService services = CreateService("", model.Code);
                H3.ValidationResult r = this.Engine.BizBus.UpdateBizService(services, true);

                if (!string.IsNullOrEmpty(r.ToString()))
                {
                    result.Message = "BizService.ValidateField";
                    result.Success = false;
                    result.Extend = r.ToString();
                    return Json(result);
                }

                result.Message = "BizService.ValidateSucess";
                result.Success = true;
                return Json(result);
            });
        }

        /// <summary>
        /// 重新加载
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public JsonResult Reload(string serviceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                H3.BizBus.BizService.BizService service = CreateService("", serviceCode);
                H3.ValidationResult r = this.Engine.BizBus.UpdateBizService(service, true);

                //ReLoadCurrentTabPage 前端刷新页面
                result.Message = "BizService.ReloadSucess";
                result.Success = true;
                return Json(result);
            });
        }

        /// <summary>
        /// 删除服务
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public JsonResult Remove(string serviceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                H3.BizBus.BizService.BizService service = CreateService("", serviceCode);
                if (service.UsedCount > 0)
                {
                    result.Message = "BizService.NotDeleted";
                    result.Success = false;
                    return Json(result);
                }

                if (this.Engine.BizBus.RemoveBizService(service.Code))
                {
                    result.Message = "msgGlobalString.DeleteSucced";
                    return Json(result);
                }
                else
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.DeleteFailed";
                    return Json(result);
                }
            });
        }

        #region 私有方法
        //创建服务,如果服务编码存在，则直接获取
        private H3.BizBus.BizService.BizService CreateService(string bizAdapterCode, string serviceCode)
        {
            //创建服务
            H3.BizBus.BizService.BizService service = null;
            if (string.IsNullOrEmpty(serviceCode))
            {
                if (string.IsNullOrEmpty(bizAdapterCode))
                {
                    return null;
                }
                H3.BizBus.BizAdapters.BizAdapterPropertyAttribute adapter = this.Engine.BizBus.GetBizAdapterAttribute(bizAdapterCode);
                service = adapter.CreateService();
            }
            else
            {
                // 编辑模式
                service = this.Engine.BizBus.GetBizService(serviceCode);
                if (service == null)
                {
                    //指定的业务处理服务不存在
                    return null;
                }
            }
            return service;
        }

        private List<BizServiceSettingViewModel> GetSettingFromService(H3.BizBus.BizService.BizService service)
        {
            // 获得所有的属性
            List<BizServiceSettingViewModel> setting = new System.Collections.Generic.List<BizServiceSettingViewModel>();
            if (service.Settings != null)
            {
                foreach (OThinker.H3.BizBus.BizService.BizServiceSetting s in service.Settings)
                {
                    setting.Add(new BizServiceSettingViewModel() { SettingName = s.SettingName, SettingValue = s.SettingValue });
                }
            }
            return setting;
        }

        /// <summary>
        /// 获取不需要验证的适配器编码
        /// </summary>
        /// <returns>不需要验证的编码列表</returns>
        private List<string> GetNotVerifiedAdapterCodes()
        {
            List<string> list = new List<string>() { 
            OThinker.H3.BizBus.Declaration.RESTfulAdapter_Code
            };

            return list;


        }
        #endregion

        #endregion

        #region  测试运行业务服务
        /// <summary>
        /// 获取服务方法列表
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <returns></returns>
        public JsonResult GetMethodList(string serviceCode)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                // 验证权限
                if (!this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.BizBus_BizService_Code) &&
                    !this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.Category_BizRule_Code) &&
                    !this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.BizRule))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.PermissionDenied";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                OThinker.H3.BizBus.BizService.BizService service = this.Engine.BizBus.GetBizService(serviceCode);

                // 创建业务对象
                OThinker.H3.BizBus.BizService.MethodSchema[] methods = this.Engine.BizBus.GetMethods(serviceCode);
                List<ListItem> lists = new System.Collections.Generic.List<ListItem>();
                if (methods != null)
                {
                    foreach (OThinker.H3.BizBus.BizService.MethodSchema m in methods)
                    {
                        lists.Add(new ListItem(m.FullName, m.MethodName));
                    }
                }
                result.Message = "";
                result.Success = true;
                result.Extend = lists;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取服务方法参数列表
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <returns></returns>
        public JsonResult GetMethodParamList(string serviceCode, string methodName)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                // 验证权限
                if (!this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.BizBus_BizService_Code) &&
                    !this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.Category_BizRule_Code) &&
                    !this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.BizRule))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.PermissionDenied";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 获得方法的定义
                OThinker.H3.BizBus.BizService.MethodSchema method = this.Engine.BizBus.GetMethod(serviceCode, methodName);
                if (method == null)
                {
                    result.Message = "";
                    result.Success = true;
                    return Json(result, JsonRequestBehavior.AllowGet);
                }


                // 属性名称列表
                System.Data.DataTable table = new System.Data.DataTable();
                table.Columns.Add("ItemName");
                table.Columns.Add("LogicType");

                List<DataItem> lists = new System.Collections.Generic.List<DataItem>();
                if (method.ParamSchema != null)
                {
                    if (method.ParamSchema.Items != null)
                    {
                        foreach (OThinker.H3.BizBus.BizService.ItemSchema item in method.ParamSchema.Items)
                        {
                            string itemValue = item.DefaultValue == null ? "" : item.DefaultValue.ToString();
                            if (item.LogicType == DataLogicType.DateTime)
                            {
                                if (!string.IsNullOrEmpty(itemValue))
                                {
                                    DateTime dt = DateTime.Now;
                                    DateTime.TryParse(itemValue, out dt);
                                    itemValue = dt.ToString("yyyy-MM-dd");
                                }
                            }
                            lists.Add(new DataItem() { ItemName = item.Name.Replace(".", "--"), LogicType = item.LogicType, ItemValue = itemValue });
                        }
                    }
                }
                result.Message = "";
                result.Success = true;
                result.Extend = lists;
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取服务方法参数列表
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <returns></returns>
        public JsonResult GetMethodReturnList(string formData)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                TestServiceViewModel model = null;
                try
                {
                    model = JsonConvert.DeserializeObject<TestServiceViewModel>(formData);
                }
                catch
                {
                    result.Success = false;
                    result.Message = "";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                if (model == null) { return null; }

                // 验证权限
                if (!this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.BizBus_BizService_Code) &&
                    !this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.Category_BizRule_Code) &&
                    !this.UserValidator.ValidateFunctionRun(Acl.FunctionNode.BizRule))
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.PermissionDenied";
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                // 获得方法的定义
                OThinker.H3.BizBus.BizService.MethodSchema method = this.Engine.BizBus.GetMethod(model.ServiceCode, model.RunMethod);
                if (method == null) { return null; }

                //查找方法的返回类型，前端根据返回类型判断是否弹出提示框
                OThinker.H3.BizBus.BizService.BizService Service = Engine.BizBus.GetBizService(model.ServiceCode);
                OThinker.H3.BizBus.BizService.BizServiceMethod bMethod = Service.GetMethod(model.RunMethod);


                // 删除与组织结构对应的系统权限
                OThinker.H3.BizBus.BizService.BizStructure param = null;
                if (method.ParamSchema != null)
                {
                    param = H3.BizBus.BizService.BizStructureUtility.Create(method.ParamSchema);
                }

                //读取参数
                foreach (DataItem item in model.InParams)
                {
                    string itemName = item.ItemName.Replace("--", ".");
                    string str = item.ItemValue.ToString();

                    OThinker.H3.BizBus.BizService.ItemSchema itemSchema = method.ParamSchema.GetItem(itemName);
                    OThinker.H3.Data.DataLogicType logicType = itemSchema.LogicType;
                    // 转换成相应的类型
                    object obj = null;
                    if (logicType == Data.DataLogicType.SingleParticipant)
                    {

                        obj = item.ItemValue.ToString();
                    }
                    else if (logicType == Data.DataLogicType.MultiParticipant)
                    {

                        obj = (string[])item.ItemValue;
                    }
                    else if (logicType == Data.DataLogicType.BizStructure ||
                        logicType == Data.DataLogicType.BizStructureArray ||
                        itemSchema.RealType.IsSubclassOf(typeof(System.Array)))
                    {
                        try
                        {
                            str = Server.UrlDecode(str);
                            //XML格式参数
                            obj = OThinker.H3.BizBus.BizService.BizStructure.ParseValueXml(method.ParamSchema, itemName, str);
                        }
                        catch
                        {
                            obj = str.Split(new string[] { ",", ";" }, StringSplitOptions.RemoveEmptyEntries);
                        }
                    }
                    else if (!OThinker.Data.Convertor.Convert(
                         str,
                         itemSchema.RealType,
                         ref obj))
                    {
                        result.Message = "BizService.TestBizService_MsgUnabled";
                        result.Success = false;
                        return Json(result);
                    }

                    param[itemName] = obj;
                }

                // 调用方法
                OThinker.H3.BizBus.BizService.BizStructure ret = null;
                try
                {
                    ret = this.Engine.BizBus.Invoke(
                         new OThinker.H3.BizBus.BizService.BizServiceInvokingContext(
                             this.UserValidator.UserID,
                             model.ServiceCode,
                             method.ServiceVersion,
                             method.MethodName,
                             param));
                }
                catch (Exception ex)
                {
                    result.Message = "BizService.TestBizService_Msg0";
                    result.Success = false;
                    Json(result);
                }

                //返回结果
                List<DataItem> listReturn = new System.Collections.Generic.List<DataItem>();
                if (ret != null && ret.Schema != null)
                {
                    foreach (OThinker.H3.BizBus.BizService.ItemSchema item in ret.Schema.Items)
                    {
                        OThinker.H3.Data.DataLogicType logicType = item.LogicType;
                        // 转换成相应的类型
                        object obj = ret[item.Name];
                        object objValue = ParseValue(item.LogicType, obj);
                        listReturn.Add(new DataItem() { ItemName = item.Name, LogicType = logicType, ItemValue = objValue });
                    }
                }

                result.Message = "";
                if (listReturn.Count == 0 && bMethod != null && bMethod.ReturnType != OThinker.H3.BizBus.BizService.MethodReturnType.None) { result.Message = "BizService.TestBizService_NoReturn"; }
                if (listReturn.Count == 0 && method.ReturnSchema != null) { result.Message = "BizService.TestBizService_NoReturn"; }
                result.Success = true;
                result.Extend = listReturn;

                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 将流程数据的值转换为相应格式的值，前台获取
        /// </summary>
        /// <param name="LogicType"></param>
        /// <param name="ItemValue"></param>
        /// <returns></returns>
        public object ParseValue(OThinker.H3.Data.DataLogicType LogicType, object ItemValue)
        {
            object obj = null;
            if (ItemValue == null)
            {
                obj = string.Empty;
            }
            else if (LogicType == OThinker.H3.Data.DataLogicType.Bool)
            {
                if (ItemValue is bool)
                {
                    obj = (bool)ItemValue == true;
                }
                else if (ItemValue is string)
                {
                    obj = (string)ItemValue == "1";
                }
            }
            else if (LogicType == OThinker.H3.Data.DataLogicType.SingleParticipant)
            {
                obj = ItemValue.ToString();
                OThinker.Organization.Unit u = this.Engine.Organization.GetUnit(ItemValue.ToString());
                if (u != null) { obj = u.Name; }
            }
            else if (LogicType == OThinker.H3.Data.DataLogicType.MultiParticipant)
            {

                obj = (string[])ItemValue;
                string Names = "";
                Dictionary<string, string> dicNames = this.Engine.Organization.GetNames((string[])ItemValue);
                if (dicNames != null)
                {
                    foreach (string key in dicNames.Keys)
                    {
                        Names += dicNames[key] + ";";
                    }

                    obj = Names;
                }
            }
            else if (
                LogicType == Data.DataLogicType.BizStructure ||
                LogicType == Data.DataLogicType.BizStructureArray ||
                LogicType == Data.DataLogicType.BizObject ||
                LogicType == Data.DataLogicType.BizObjectArray)
            {
                System.Data.DataTable table = null;
                switch (LogicType)
                {
                    case Data.DataLogicType.BizStructure:
                        OThinker.H3.BizBus.BizService.BizStructure bizStruct = (OThinker.H3.BizBus.BizService.BizStructure)ItemValue;
                        table = OThinker.H3.BizBus.BizService.BizStructureUtility.ToTable(
                            bizStruct.Schema,
                            new OThinker.H3.BizBus.BizService.BizStructure[] { bizStruct });
                        break;
                    case Data.DataLogicType.BizStructureArray:
                        OThinker.H3.BizBus.BizService.BizStructure[] bizStructs = (OThinker.H3.BizBus.BizService.BizStructure[])ItemValue;
                        foreach (OThinker.H3.BizBus.BizService.BizStructure st in bizStructs)
                        {
                            if (st != null)
                            {
                                table = OThinker.H3.BizBus.BizService.BizStructureUtility.ToTable(
                                    st.Schema,
                                    bizStructs);
                                break;
                            }
                        }
                        break;
                    case Data.DataLogicType.BizObject:
                        BizObject bizObject = (BizObject)ItemValue;
                        table = BizObjectUtility.ToTable(
                            bizObject.Schema,
                            new BizObject[] { bizObject });
                        break;
                    case Data.DataLogicType.BizObjectArray:
                        BizObject[] bizObjects = (BizObject[])ItemValue;
                        foreach (BizObject bo in bizObjects)
                        {
                            if (bo != null)
                            {
                                table = BizObjectUtility.ToTable(
                                    bo.Schema,
                                    bizObjects);
                                break;
                            }
                        }
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (table != null)
                {
                    table.TableName = "BizObjects";
                }
                obj = ParseTableToObj(table);
            }
            else if (ItemValue is System.Array)
            {
                obj = ItemValue;
            }
            else if (ItemValue is string)
            {
                string s = string.Empty;
                DateTime time = DateTime.Now;
                if (DateTime.TryParse(ItemValue + string.Empty, out time))
                {
                    if (time.Year == 1753 || time.Year == 9999 || time.Year == 1)
                        s = "-";
                    else
                        s = time.ToString();
                }
                else
                {
                    s = (string)ItemValue;
                }
                obj = s;
            }
            else
            {

                obj = ItemValue.ToString().Replace("\n", "<BR>");
            }

            return obj;
        }

        /// <summary>
        /// 将Table转换成数据
        /// </summary>
        /// <param name="dtSource"></param>
        /// <returns></returns>
        public List<object> ParseTableToObj(System.Data.DataTable dtSource)
        {
            if (dtSource == null) return null;
            //存放Table数据
            List<object> lstObj = new System.Collections.Generic.List<object>();

            //表头
            List<string> listHeader = new System.Collections.Generic.List<string>();
            foreach (System.Data.DataColumn dc in dtSource.Columns)
            {
                listHeader.Add(dc.ColumnName);
            }

            lstObj.Add(listHeader);
            //表体

            foreach (System.Data.DataRow dr in dtSource.Rows)
            {
                lstObj.Add(dr.ItemArray);
            }

            return lstObj;
        }
        #endregion

        #region RESTful 适配器
        /// <summary>
        /// 获取RESTful适配器输入参数信息
        /// </summary>
        /// <param name="bizServiceCode"业务服务编码</param>
        /// <returns>构造LigerUI所需要的数据</returns>
        [HttpPost]
        public JsonResult GetRESTfulParams(string bizServiceCode, string paramType)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult();
                RESTfulParamViewModel model = new RESTfulParamViewModel()
                {
                    BizServiceCode = bizServiceCode,
                    ParamType = (ParamType)Enum.Parse(typeof(ParamType), paramType)
                };
                List<RESTfulParamViewModel> list = GetRESTfulParams(model, out result);
                object gridData = CreateLigerUIGridData(list.ToArray());
                return Json(gridData, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存RESTful适配器参数
        /// </summary>
        /// <param name="model">适配器参数模型</param>
        /// <returns>保存结果</returns>
        [HttpPost]
        public JsonResult SaveRESTfulParam(RESTfulParamViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                if (model.children != null && model.children[0] == null)
                {
                    model.children = null;
                }
                ActionResult result;
                List<RESTfulParamViewModel> list = GetRESTfulParams(model, out result);
                if (!result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                string oldParamName = this.Request["OldParamName"] + string.Empty;
                model.ParamDataTypeName = Data.DataLogicTypeConvertor.ToLogicTypeName((DataLogicType)Enum.Parse(typeof(Data.DataLogicType), model.ParamDataType));
                result = new ActionResult(true);
                //子参数
                List<RESTfulParamViewModel> childParams = new List<RESTfulParamViewModel>();
                int index = list.Count();
                int childIndex = -1;
                if (!string.IsNullOrEmpty(oldParamName))
                {
                    //移除旧参数信息
                    RESTfulParamViewModel item = null;
                    //移除指定的自定义配置参数
                    if (string.IsNullOrEmpty(model.ParentParamName))
                    {
                        item = list.FirstOrDefault(s => s.ParamName == oldParamName);
                        index = list.IndexOf(item);
                        childParams = item.children;
                        if (item != null)
                        {
                            list.Remove(item);
                        }
                    }
                    else
                    {
                        RESTfulParamViewModel parentParam = list.FirstOrDefault(s => s.ParamName == model.ParentParamName);
                        if (parentParam != null)
                        {
                            item = parentParam.children.FirstOrDefault(s => s.ParamName == oldParamName);
                            childIndex = parentParam.children.IndexOf(item);
                            parentParam.children.Remove(item);
                        }

                    }
                }
                //添加
                if (string.IsNullOrEmpty(model.ParentParamName))
                {
                    model.children = new List<RESTfulParamViewModel>();
                    foreach (var child in childParams)
                    {
                        child.ParentParamName = model.ParamName;
                        model.children.Add(child);
                    }
                    list.Insert(index, model);
                    //list.Add(model);
                }
                else
                {
                    RESTfulParamViewModel childParam = list.FirstOrDefault(s => s.ParamName == model.ParentParamName);
                    if (childParam != null)
                    {
                        if (childParam.children == null)
                        {
                            childParam.children = new List<RESTfulParamViewModel>();
                        }
                        if (childIndex >= 0)
                        {
                            childParam.children.Insert(childIndex, model);
                        }
                        else
                        {
                            childParam.children.Add(model);
                        }
                    }
                }

                H3.BizBus.BizService.BizService service = CreateService("", model.BizServiceCode);
                string settingName = GetParamSettingName(model.ParamType);
                string settingValue = JsonConvert.SerializeObject(list);
                service.SetSettingValue(settingName, settingValue);
                //TODO,暂时不做验证
                ValidationResult r = this.Engine.BizBus.UpdateBizService(service, false);
                result.Success = r.Valid;
                if (!result.Success) result.Message = "更新失败,请重试.";
                result.Extend = new
                {
                    Param = new RESTfulParamViewModel()
                    {
                        BizServiceCode = model.BizServiceCode,
                        ParentParamName = model.ParentParamName,
                        IsNew = true,
                        ParamType = model.ParamType,
                        ParamDataType = DataLogicType.ShortString.ToString()
                    },
                    ParamName = "",
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取RESTful适配器参数
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <returns>RESTful适配器</returns>
        public JsonResult GetRESTfulParam(RESTfulParamViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result;
                List<RESTfulParamViewModel> list = GetRESTfulParams(model, out result);
                if (!result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                RESTfulParamViewModel paramModel = new RESTfulParamViewModel();
                if (string.IsNullOrEmpty(model.ParentParamName))
                {
                    paramModel = list.FirstOrDefault(s => s.ParamName == model.ParamName);
                }
                else
                {
                    RESTfulParamViewModel parentParam = list.FirstOrDefault(s => s.ParamName == model.ParentParamName);
                    if (parentParam != null && parentParam.children != null)
                        paramModel = parentParam.children.FirstOrDefault(s => s.ParamName == model.ParamName);
                }
                List<Item> logicTypes = GetLogicTypes(!string.IsNullOrEmpty(model.ParentParamName));
                if (paramModel == null)
                {
                    paramModel = new RESTfulParamViewModel()
                    {
                        IsNew = true,
                        BizServiceCode = model.BizServiceCode,
                        ParentParamName = model.ParentParamName,
                        ParamDataType = DataLogicType.ShortString.ToString(),
                        ParamType = model.ParamType
                    };
                }
                else
                {
                    paramModel.IsNew = false;
                }
                result.Extend = new
                {
                    Param = paramModel,
                    LogicTypes = logicTypes,
                    ParamName = paramModel.ParamName,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 移除一个RESTful适配器参数
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult RemoveRESTfulParam(RESTfulParamViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(false);
                List<RESTfulParamViewModel> list = GetRESTfulParams(model, out result);
                if (!result.Success)
                {
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                RESTfulParamViewModel item = null;
                //移除指定的自定义配置参数
                if (string.IsNullOrEmpty(model.ParentParamName))
                {
                    item = list.FirstOrDefault(s => s.ParamName == model.ParamName);
                    if (item != null)
                    {
                        list.Remove(item);
                    }
                }
                else
                {
                    RESTfulParamViewModel parentParam = list.FirstOrDefault(s => s.ParamName == model.ParentParamName);
                    if (parentParam != null)
                    {
                        item = parentParam.children.FirstOrDefault(s => s.ParamName == model.ParamName);
                        //list.FirstOrDefault(s => s.ParamName == model.ParentParamName).children.Remove(item);
                        parentParam.children.Remove(item);
                    }

                }
                H3.BizBus.BizService.BizService service = CreateService("", model.BizServiceCode);
                string settingName = GetParamSettingName(model.ParamType);
                string settingValue = JsonConvert.SerializeObject(list);
                service.SetSettingValue(settingName, settingValue);
                ValidationResult r = this.Engine.BizBus.UpdateBizService(service, false);
                result.Success = r.Valid;
                result.Extend = new
                {
                    ParamType = model.ParamType
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 获取RESTful参数配置
        /// </summary>
        /// <param name="model">参数模型</param>
        /// <param name="result">输出获取结果</param>
        /// <returns>RESTful所有参数配置</returns>
        private List<RESTfulParamViewModel> GetRESTfulParams(RESTfulParamViewModel model, out ActionResult result)
        {
            List<RESTfulParamViewModel> list = new List<RESTfulParamViewModel>();
            result = new ActionResult(false);
            H3.BizBus.BizService.BizService service = CreateService("", model.BizServiceCode);
            if (service == null)
            {
                result.Message = "BizService.BizServiceIsNotExist";
                return list;
            }
            string settingName = GetParamSettingName(model.ParamType);
            //查找业务服务自定义属性
            BizServiceSetting bizServiceSetting = service.Settings.FirstOrDefault(s => s.SettingName == settingName);
            if (bizServiceSetting == null)
            {
                result.Message = "BizService.BizServiceSettingIsNotExist";
                return list;
            }
            if (!string.IsNullOrEmpty(bizServiceSetting.SettingValue))
            {
                list = JsonConvert.DeserializeObject<List<RESTfulParamViewModel>>(bizServiceSetting.SettingValue);
            }
            result.Success = true;
            return list;
        }

        /// <summary>
        /// 获取RESTful自定义设置名称
        /// </summary>
        /// <param name="paramType">自定义设置类型</param>
        /// <returns>自定义设置名称</returns>
        private string GetParamSettingName(ParamType paramType)
        {
            string settingName = string.Empty;
            if (paramType == ParamType.InputParam)
            {
                settingName = OThinker.H3.BizBus.Declaration.RESTfulAdapter_InputParams;
            }
            else
            {
                settingName = OThinker.H3.BizBus.Declaration.RESTfulAdapter_OutputParams;
            }
            return settingName;
        }

        /// <summary>
        /// 属性类型
        /// </summary>
        /// <returns></returns>
        private List<Item> GetLogicTypes(bool isChild)
        {
            List<Item> list = new List<Item>();
            OThinker.H3.Data.DataLogicType[] types = OThinker.H3.Data.DataLogicTypeConvertor.GetBizObjectSupportLogicTypes();
            string[] normal = { "ShortString", "String", "Bool", "Int", "Long", "Double", "DateTime", "BizObjectArray" };
            foreach (OThinker.H3.Data.DataLogicType logicType in types)
            {
                if (isChild)
                {
                    if (logicType == OThinker.H3.Data.DataLogicType.BizObject
                        || logicType == OThinker.H3.Data.DataLogicType.BizObjectArray
                        || logicType == OThinker.H3.Data.DataLogicType.Comment)
                    {
                        continue;
                    }
                }
                string typeName = Data.DataLogicTypeConvertor.ToLogicTypeName(logicType);
                if (normal.Contains(logicType + string.Empty))
                    list.Add(new Item(typeName, logicType.ToString()));
                //if (this.CurrentSchema.GetProperty(this.SelectedProperty) != null && this.CurrentSchema.GetProperty(this.SelectedProperty).LogicType != logicType)
                //    list.Add(new Item(typeName, logicType.ToString()));
            }
            return list;
        }
        #endregion

    }
}
