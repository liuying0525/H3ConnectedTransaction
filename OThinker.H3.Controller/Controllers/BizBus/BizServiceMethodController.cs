using Newtonsoft.Json;
using OThinker.H3.Acl;
using OThinker.H3.BizBus.BizService;
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace OThinker.H3.Controllers.Controllers.BizBus
{
    public class BizServiceMethodController:ControllerBase
    {
        public override string FunctionCode
        {
            get { return FunctionNode.BizBus_BizService_Code; }
        }

        public BizServiceMethodController() { }

        protected OThinker.H3.BizBus.BizService.BizService GetService(string serviceCode)
        {
            return Engine.BizBus.GetBizService(serviceCode);
        }

        /// <summary>
        /// 获取服务方法列表
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <returns></returns>
        public JsonResult GetDataList(string serviceCode)
        {
            return ExecuteFunctionRun(() => {

                List<BizServiceMethodViewModel> lists = new List<BizServiceMethodViewModel>();
                OThinker.H3.BizBus.BizService.BizService Service = GetService(serviceCode);
                if (Service.Methods != null)
                {
                    BizServiceMethodViewModel data;
                    foreach (BizServiceMethod method in Service.Methods)
                    {
                        data = new BizServiceMethodViewModel();
                        data.ObjectID = method.ObjectID;
                        data.DisplayName = method.DisplayName;
                        data.MethodName = method.MethodName;
                        data.ReturnType = method.ReturnType.ToString();
                        data.MethodSetting = method.MethodSetting;
                      
                        lists.Add(data);
                    }
                }
                var gridData = CreateLigerUIGridData(lists.ToArray());
                return Json(gridData,JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 删除服务方法
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="methodNames"></param>
        /// <returns></returns>
        public JsonResult DeleteData(string serviceCode,string methodNames)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                string[] methods = methodNames.Split(';');
                OThinker.H3.BizBus.BizService.BizService Service = GetService(serviceCode);
                foreach (string methodName in methods)
                {
                    BizServiceMethod BizServiceMethod = Service.GetMethod(methodName);
                    if (BizServiceMethod != null && BizServiceMethod.UsedCount > 0)
                    {
                        result.Success = false;
                        result.Message = "BizServiceMethod.Mss1";
                       
                        return Json(result);
                    }
                    if (!string.IsNullOrWhiteSpace(methodName))
                       Service.RemoveMethod(methodName);
                }

                OThinker.H3.ValidationResult result2 = Engine.BizBus.UpdateBizService(Service, true);
                if (result2.Valid)
                {
                    // 删除成功
                    result.Message = "msgGlobalString.DeleteSucced";
                }
                else
                {
                    // 删除失败
                    result.Message = "msgGlobalString.DeleteFailed";
                    result.Extend = result2.ToString();//记录具体错误信息
                    result.Success = false;
                }
                return Json(result);
            });
        }


        string _ParameterPrefix = "@";


        /// <summary>
        /// 获取参数前缀符号
        /// </summary>
        /// <param name="Service"></param>
        /// <returns></returns>
        private Settings.BizDbConnectionConfig GetDbConnectionConfig( OThinker.H3.BizBus.BizService.BizService Service)
        {
             Settings.BizDbConnectionConfig DbConnectionConfig = null;
                    if (Service.Settings != null)
                    {
                        foreach (OThinker.H3.BizBus.BizService.BizServiceSetting s in Service.Settings)
                        {
                            if (s.SettingName == OThinker.H3.BizBus.Declaration.DbTableAdapter_DbCode)
                            {
                                string dbCode = s.SettingValue;
                                DbConnectionConfig = Engine.SettingManager.GetBizDbConnectionConfig(dbCode);
                                if (DbConnectionConfig != null)
                                {
                                    _ParameterPrefix = OThinker.Data.Database.Database.GetParameterFlag(DbConnectionConfig.DbType);
                                }
                            }
                        }
                    }
            return DbConnectionConfig;
        }
        
       /// <summary>
       /// 获取方法信息
       /// </summary>
       /// <param name="serviceCode"></param>
       /// <param name="methodName"></param>
       /// <returns></returns>
        public JsonResult GetMethod(string serviceCode,string methodName)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                // 编辑模式
                OThinker.H3.BizBus.BizService.BizService Service = GetService(serviceCode);
                Settings.BizDbConnectionConfig DbConnectionConfig = null;
                OThinker.H3.BizBus.BizService.BizServiceMethod Method = null;

                if (Service == null)
                {
                    result.Success = false;
                    result.Message = "BizServiceMethod.ServiceNotExists";
                    //ShowErrorMessage(PortalResource.GetString("EditBizServiceMethod_Msg1"));
                    //CloseCurrentDialog(); 客户端执行
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                //如果是DbSqlAdapter
                else if (OThinker.H3.BizBus.Declaration.DbSqlAdapter_Code == Service.BizAdapterCode)
                {
                    //获取参数前缀符号
                    DbConnectionConfig =GetDbConnectionConfig(Service);
                }
                // 方法名称
                if (!string.IsNullOrEmpty(methodName))
                {
                    Method = Service.GetMethod(methodName);
                    if (Method == null)
                    {
                        //ShowErrorMessage(PortalResource.GetString("EditBizServiceMethod_Msg2"));
                        //CloseCurrentDialog();
                        result.Message = "BizServiceMethod.MethodNotExist";
                        result.Success = false;
                    }
                    else
                    {
                        BizServiceMethodViewModel model = new BizServiceMethodViewModel()
                        {
                            ObjectID = Method.ObjectID,
                            MethodName = Method.MethodName,
                            BizAdapterCode = Service.BizAdapterCode,
                            DisplayName = Method.DisplayName,
                            Description = Method.Description,
                            ReturnType = Method.ReturnType.ToString(),
                            MethodSetting = Method.MethodSetting,
                            ServiceCode = serviceCode,
                            ParameterPrefix = _ParameterPrefix
                        };

                        result.Extend = model;
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    BizServiceMethodViewModel model=new BizServiceMethodViewModel(){
                        ParameterPrefix =_ParameterPrefix,
                        BizAdapterCode=Service.BizAdapterCode,
                        ServiceCode=serviceCode
                    };
                    result.Extend = model;
                }

                result.Success=true;result.Message="";
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }


        /// <summary>
        /// 获取方法列
        /// </summary>
        /// <param name="serviceCode"></param>
        /// <param name="methodName"></param>
        /// <param name="sql"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public JsonResult GetMethodColumns(string serviceCode, string methodName,string sql,string parameters)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true, "");
                OThinker.H3.BizBus.BizService.BizService Service = GetService(serviceCode);
                Settings.BizDbConnectionConfig DbConnectionConfig = GetDbConnectionConfig(Service);
                if (DbConnectionConfig != null)
                {
                    try
                    {
                        string[] ParameterNames = (string[])JsonConvert.DeserializeObject<string[]>(parameters);//前端组合的XML中的Parameters
                        List<OThinker.Data.Database.Parameter> lstParameters = new List<OThinker.Data.Database.Parameter>();
                        if (ParameterNames != null)
                        {
                            foreach (string p in ParameterNames)
                            {
                                lstParameters.Add(new OThinker.Data.Database.Parameter(p, System.Data.DbType.String, null));
                            }
                        }

                        OThinker.Data.Database.ICommand Command = new OThinker.Data.Database.CommandFactory(DbConnectionConfig.DbType, DbConnectionConfig.DbConnectionString).CreateCommand();
                        System.Data.DataTable dt = Command.ExecuteDataTable(sql, lstParameters.ToArray());
                        if (dt != null)
                        {
                            Dictionary<string, string> ColumnDictionary = new Dictionary<string, string>();
                            string StringType = typeof(string).FullName + string.Empty;

                            foreach (System.Data.DataColumn c in dt.Columns)
                            {
                                if (!ColumnDictionary.ContainsKey(c.ColumnName))
                                {
                                    Data.DataLogicType LogicType = Data.DataLogicType.ShortString;
                                    switch (c.DataType.ToString().ToLower())
                                    {
                                        case "system.boolean":
                                            LogicType = Data.DataLogicType.Bool;
                                            break;
                                        case "system.char":
                                        case "system.byte":
                                        case "system.sbyte":
                                            LogicType = Data.DataLogicType.ShortString;
                                            break;
                                        case "system.int":
                                        case "system.int16":
                                        case "system.int32":
                                        case "system.int64":
                                        case "system.uint16":
                                        case "system.uint32":
                                        case "system.uint64":
                                            LogicType = Data.DataLogicType.Int;
                                            break;
                                        case "system.decimal":
                                        case "system.double":
                                        case "system.single":
                                            LogicType = Data.DataLogicType.Decimal;
                                            break;
                                        case "system.object":
                                            break;
                                        case "system.string":
                                            LogicType = Data.DataLogicType.String;
                                            break;
                                        default:
                                            break;
                                    }
                                    ColumnDictionary.Add(c.ColumnName, LogicType.ToString());
                                }
                            }
                            result.Extend=ColumnDictionary;
                        }
                    }
                    catch(Exception ex)
                    {
                        result.Success = false;
                        
                        result.Extend = ex.Message;
                    }

                   return Json(result, JsonRequestBehavior.AllowGet);
                }
                return null;
            });
       
        }


        /// <summary>
        /// 获取返回类型
        /// </summary>
        /// <returns></returns>
        public JsonResult GetReturnType()
        {
            return ExecuteFunctionRun(() =>
            {
                // 列出所有的返回值类型
                string[] names = Enum.GetNames(typeof(OThinker.H3.BizBus.BizService.MethodReturnType));
                List<object> lists = new List<object>();
                foreach (string name in names)
                {
                    if (name != OThinker.H3.BizBus.BizService.MethodReturnType.Auto.ToString())
                    {
                        lists.Add(new { Text=name,Value=name});
                    }
                }

                return Json(lists,JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 保存方法
        /// </summary>
        /// <returns></returns>
        public JsonResult SaveMethod(BizServiceMethodViewModel model)
        {
            return ExecuteFunctionRun(() => {
                ActionResult result = new ActionResult(true, "");

                OThinker.H3.BizBus.BizService.BizService Service = GetService(model.ServiceCode);
                OThinker.H3.BizBus.BizService.BizServiceMethod Method = null;

                bool Creating = string.IsNullOrEmpty(model.ObjectID); 
                // 更新属性
                if (Creating)
                {
                    Method = new OThinker.H3.BizBus.BizService.BizServiceMethod();
                }
                else
                {
                    Method = Service.GetMethod(model.MethodName);
                }
                Method.MethodName = model.MethodName;
                Method.DisplayName = model.DisplayName;
                Method.Description = model.Description;
                Method.ReturnType = (H3.BizBus.BizService.MethodReturnType)Enum.Parse(typeof(H3.BizBus.BizService.MethodReturnType), model.ReturnType);
                Method.MethodSetting = Server.UrlDecode(model.MethodSetting);//传递之前有编码，这里需要解码

                if (Creating)
                {
                    if (!Service.AddMethod(Method))
                    {
                        result.Success = false;
                        result.Message = "BizServiceMethod.SaveFailed1";//添加方法失败，可能是方法重名
                        return Json(result);
                    }
                }

                H3.ValidationResult r = this.Engine.BizBus.UpdateBizService(Service, true);
                string result2 = r.ToString();
                if (string.IsNullOrEmpty(result2))
                {
                    result.Success = true;
                    result.Message = "msgGlobalString.SaveSucced";
                }
                else
                {
                    result.Success = false;
                    result.Message = "msgGlobalString.SaveFailed";
                    result.Extend = result2;
                }
                return Json(result);
            });
        }
    }
}
