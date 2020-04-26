using OThinker.Data.Database;
using OThinker.H3.Configs;
using OThinker.H3.Controllers.ViewModels.Cluster;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace OThinker.H3.Controllers.Controllers.Cluster
{
    /// <summary>
    /// 引擎实例管理控制器
    /// </summary>
    public class EngineController : ClusterController
    {
        private string Code
        {
            get
            {
                return Request["Code"] + string.Empty;
            }
        }
        /// <summary>
        /// 获取引擎实例列表
        /// </summary>
        /// <returns>引擎实例列表</returns>
        [HttpPost]
        public JsonResult GetEngineList()
        {
            return ExecuteFunctionRun(() =>
            {
                System.Data.DataTable table = this.Connection.GetLogicUnitTable();
                List<EngineViewModel> list = new List<EngineViewModel>();
                if (table != null)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        EngineViewModel model = new EngineViewModel()
                        {
                            Code = row[OThinker.Clusterware.LogicUnitConfig.PropertyName_Code] + string.Empty,
                            DBType = ((OThinker.Data.Database.DatabaseType)int.Parse(row[OThinker.Clusterware.LogicUnitConfig.PropertyName_DBType] + string.Empty)).ToString(),
                            DBServer = row[OThinker.Clusterware.LogicUnitConfig.PropertyName_DBServer] + string.Empty,
                            DBName = row[OThinker.Clusterware.LogicUnitConfig.PropertyName_DBName] + string.Empty,
                            LogDBType = ((OThinker.Data.Database.DatabaseType)int.Parse(row[OThinker.Clusterware.LogicUnitConfig.PropertyName_LogDBType] + string.Empty)).ToString(),
                            LogDBServer = row[OThinker.Clusterware.LogicUnitConfig.PropertyName_LogDBServer] + string.Empty,
                            LogDBName = row[OThinker.Clusterware.LogicUnitConfig.PropertyName_LogDBName] + string.Empty,
                            UnitState = (row[OThinker.Clusterware.LogicUnitConfig.PropertyName_UnitState] + string.Empty == "1") ? "运行状态" : "挂起状态"
                        };
                        list.Add(model);
                    }
                }
                return Json(CreateLigerUIGridData(list.ToArray()), JsonRequestBehavior.AllowGet);
            });
        }

        /// <summary>
        /// 删除一个引擎实例
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult DelEngine(string codes)
        {
            return ExecuteFunctionRun(() =>
            {
                ActionResult result = new ActionResult(true);
                string engineCodes = codes;
                string[] engineCodeArray = engineCodes.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);
                if (engineCodeArray != null)
                {
                    foreach (string code in engineCodeArray)
                    {
                        this.Connection.RemoveLogicUnit(code);
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }

        /// <summary>
        /// 执行方法
        /// </summary>
        /// <param name="action">方法名称</param>
        /// <param name="codes">编码</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult DoAction(string action, string codes)
        {
            return ExecuteFunctionRun(() =>
            {
                string engineCode = string.Empty;
                engineCode = codes;
                ActionResult result = new ActionResult(true);
                try
                {
                    switch (action)
                    {
                        case "Disabled":
                            this.Connection.Unmount(engineCode);
                            OThinker.Clusterware.LogicUnitConfig config = this.Connection.GetLogicUnitConfigByCode(engineCode);
                            config.UnitState = Clusterware.LogicUnitState.Disabled;
                            this.Connection.UpdateLogicUnit(config);
                            break;
                        case "Enabled":
                            OThinker.Clusterware.LogicUnitConfig config1 = this.Connection.GetLogicUnitConfigByCode(engineCode);
                            config1.UnitState = Clusterware.LogicUnitState.Running;
                            this.Connection.UpdateLogicUnit(config1);
                            this.Connection.Mount(engineCode);
                            break;
                        case "Suspended":
                            this.Connection.Unmount(engineCode);
                            break;
                        case "Restart":
                            this.Connection.Restart(engineCode);
                            break;
                        default:
                            break;
                    }
                }
                catch
                {

                    result.Success = false;
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });

        }


        /// <summary>
        /// 保存引擎实例
        /// </summary>
        /// <param name="model">引擎实例模型</param>
        /// <returns>是否保存成功</returns>
        public JsonResult SaveEngine(EngineViewModel model)
        {
            return ExecuteFunctionRun(() =>
            {

                ActionResult result = new ActionResult(false);
                OThinker.Clusterware.LogicUnitConfig config = new EngineConfig();
                if (Code != string.Empty && false)
                {
                    // ERROR, 更新状态
                    config = this.Connection.GetLogicUnitConfigByCode(Code);

                    OThinker.Data.Database.DatabaseType dbType = (OThinker.Data.Database.DatabaseType)int.Parse(model.DBType);
                    string connString = OThinker.Data.Database.Database.GetConnectionString(dbType, config.DBServer, config.DBName, model.DBUser.Trim(), model.DBPassword.Trim());
                    string logConnString = OThinker.Data.Database.Database.GetConnectionString(dbType, config.LogDBServer, config.LogDBName, model.LogDBName.Trim(), model.LogDBPassword.Trim());
                    CommandFactory factory = new CommandFactory(dbType, connString);
                    CommandFactory Logfactory = new CommandFactory(dbType, logConnString);

                    if (!factory.CheckConnection() || !Logfactory.CheckConnection())
                    {
                        result.Success = false;
                        result.Message = "数据库无法正常连接";
                    }
                    else
                    {
                        config.DBUser = model.DBUser.Trim();
                        config.DBPassword = model.DBPassword.Trim();
                        config.LogDBUser = model.LogDBUser.Trim();
                        config.LogDBPassword = model.LogDBPassword.Trim();
                        config.UnitState = Clusterware.LogicUnitState.Running;

                        this.Connection.UpdateLogicUnit(config);

                        result.Success = true;
                    }
                    return Json(result, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    Regex reg = new Regex("[\\u4e00-\\u9fa5]");
                    if (reg.IsMatch(model.Code.Trim()))
                    {
                        result.Success = false;
                        result.Message = "引擎编码不可包含中文!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    };
                    if (model.Code.Contains(" "))
                    {
                        result.Success = false;
                        result.Message = "引擎编码不可包含空格!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    };
                    if (string.IsNullOrEmpty(model.DBName) && DatabaseType.SqlServer == (OThinker.Data.Database.DatabaseType)int.Parse(model.DBType))
                    {
                        result.Success = false;
                        result.Message = "数据库名称不可为空!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    };
                    if (string.IsNullOrEmpty(model.LogDBName) && DatabaseType.SqlServer == (OThinker.Data.Database.DatabaseType)int.Parse(model.LogDBType))
                    {
                        result.Success = false;
                        result.Message = "日志数据库名称不可为空!";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    };
                    config = new EngineConfig()
                    {
                        Code = model.Code.Trim(),
                        DBType = (OThinker.Data.Database.DatabaseType)int.Parse(model.DBType),
                        DBServer = model.DBServer.Trim(),
                        DBName = string.IsNullOrEmpty(model.DBName) ? "" : model.DBName.Trim(),
                        DBUser = model.DBUser.Trim(),
                        DBPassword = model.DBPassword.Trim(),
                        LogDBType = (OThinker.Data.Database.DatabaseType)int.Parse(model.LogDBType),
                        LogDBServer = model.LogDBServer.Trim(),
                        LogDBName = string.IsNullOrEmpty(model.LogDBName) ? "" : model.LogDBName.Trim(),
                        LogDBUser = model.LogDBUser.Trim(),
                        LogDBPassword = model.LogDBPassword.Trim(),
                        UnitState = Clusterware.LogicUnitState.Running,
                        InitialUserLoginName = "Administrator@" + Code
                    };

                    if (!this.Connection.AddLogicUnit(config))
                    {
                        this.Connection.RemoveLogicUnit(config.Code);
                        result.Success = false;
                        result.Message = "无法成功创建，信息不正确，或者没有足够可用的数据库";
                    }
                    else
                    {
                        result.Success = true;
                    }

                    if (config.DBType == OThinker.Data.Database.DatabaseType.Oracle)
                    {
                        CommandFactory factory = new CommandFactory(DatabaseType.Oracle, config.DBConnString);
                        if (!factory.CheckConnection())
                        {
                            result.Success = false;
                            result.Message = "数据库无法正常连接";
                        }
                    }
                }
                return Json(result, JsonRequestBehavior.AllowGet);
            });
        }
    }
}
