using System.IO;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using OThinker.H3.Tracking;
using OThinker.H3.Configs;
using System.Text.RegularExpressions;

namespace OThinker.H3.Controllers
{
    public class AppUtility
    {
        /// <summary>
        /// 获取Portal站点的跟目录路径
        /// </summary>
        public static string PortalRoot
        {
            get
            {
                return AppConfig.PortalRoot;
            }
        }

        /// <summary>
        /// 引擎实例，注：公有云在未登录时，不可访问当前对象
        /// </summary>
        public static OThinker.H3.IEngine Engine
        {
            get
            {
                switch (AppConfig.ConnectionMode)
                {
                    case ConnectionStringParser.ConnectionMode.Mono:
                        return AppConfig.MonoEngine;
                    case ConnectionStringParser.ConnectionMode.Shared:
                        if (UserValidatorFactory.CurrentUser == null)
                        {
                            throw new Exception("请先登录后，再访问引擎对象！");
                        }
                        return UserValidatorFactory.CurrentUser.Engine;
                    default:
                        throw new NotImplementedException();
                }
            }
        }

        /// <summary>
        /// 格式化参数的值
        /// </summary>
        /// <param name="inputString"></param>
        /// <returns></returns>
        public static string RegParam(string inputString)
        {
            return inputString.Replace("'", string.Empty).Replace("--", string.Empty);
        }

        /// <summary>
        /// 启动站点垃圾回收程序
        /// </summary>
        /// <param name="Server"></param>
        public static void StartRecycling(HttpServerUtility Server)
        {
            //TODO:图片垃圾回收
            //string root = SheetUtility.GetTempImageRoot(null);
            //TempImagePath = Server.MapPath(root);
            TempImagePath = Path.Combine(Server.MapPath("."), "TempImages");
            // 启动
            System.Threading.ThreadStart start = new System.Threading.ThreadStart(RecycleImages);
            System.Threading.Thread thread = new System.Threading.Thread(start);
            thread.Start();
        }

        private static string TempImagePath = null;
        private static System.DateTime LastRecycleImagesTime = System.DateTime.MinValue;
        /// <summary>
        /// 回收临时文件
        /// </summary>
        private static void _RecycleImages()
        {
            if (LastRecycleImagesTime.AddDays(1) > System.DateTime.Now
                || AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Shared)
            {
                return;
            }
            LastRecycleImagesTime = System.DateTime.Now;

            try
            {
                Engine.LogWriter.Write("Deleting temporary images...");
                System.IO.DirectoryInfo dir = new DirectoryInfo(TempImagePath);
                System.IO.FileInfo[] files = dir.GetFiles();

                if (files != null)
                {
                    foreach (System.IO.FileInfo file in files)
                    {
                        if (file.CreationTime.AddDays(1) < System.DateTime.Now)
                        {
                            // 删除掉
                            try
                            {
                                System.IO.File.Delete(file.FullName);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Engine.LogWriter.Write("Deleting temporary images error: " + ex);
            }
        }

        /// <summary>
        /// 回收所有图片
        /// </summary>
        private static void RecycleImages()
        {
            while (true)
            {
                _RecycleImages();
                WriteOnlineUsersToServer();
                System.Threading.Thread.Sleep(1000 * 3);
            }
        }

        /// <summary>
        /// 当前在线用户数
        /// </summary>
        public static int OnlineUserCount
        {
            get
            {
                return _OnlineUserIdAliasTable.Count;
            }
        }

        /// <summary>
        /// (用户ID, 操作日志)
        /// </summary>
        private static Dictionary<string, OThinker.H3.Tracking.UserLog> _UserLogTable = new Dictionary<string, OThinker.H3.Tracking.UserLog>();
        private static void WriteOnlineUsersToServer()
        {
            try
            {
                if (_UserLogTable.Count != 0)
                {
                    List<OThinker.H3.Tracking.UserLog> logs = new List<Tracking.UserLog>();
                    lock (_UserLogTable)
                    {
                        foreach (OThinker.H3.Tracking.UserLog log in _UserLogTable.Values)
                        {
                            logs.Add(log);
                        }
                        _UserLogTable.Clear();
                    }
                    Engine.UserLogWriter.OnUserLogInOut(logs.ToArray(), System.Environment.MachineName, OnlineUserCount);
                }
            }
            catch
            {
            }
        }

        /// <summary>
        /// 增加当前在线用户数
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Request"></param>
        /// <param name="PortalType"></param>
        public static void OnUserLogin(UserValidator User,
            HttpRequest Request,
            Site.PortalType PortalType = Site.PortalType.Portal)
        {
            lock (_OnlineUserIdAliasTable)
            {
                OThinker.H3.Tracking.UserLog log = new Tracking.UserLog(
                   Tracking.UserLogType.Login,
                   User.UserID,
                   null,
                   null,
                   null,
                   null,
                   null,
                   null,
                   SheetUtility.GetClientIP(Request),
                   SheetUtility.GetClientPlatform(Request),
                   SheetUtility.GetClientBrowser(Request));

                if (_OnlineUserIdAliasTable.ContainsKey(User.UserID.ToLower()))
                {
                    //_OnlineUserIdAliasTable.Remove(User.UserID);
                    //_OnlineUserIdLoginTimeTable.Remove(User.UserID);
                    return;
                }
                _OnlineUserIdAliasTable.Add(User.UserID, User.UserCode);
                _OnlineUserIdLoginTimeTable.Add(User.UserID, DateTime.Now);

                log.UserCode = User.UserCode;
                log.SiteType = PortalType;

                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == OThinker.H3.ConnectionStringParser.ConnectionMode.Mono)
                {
                    // 私有云模式批量写入
                    if (!_UserLogTable.ContainsKey(User.UserID.ToLower()))
                    {
                        _UserLogTable.Add(User.UserID.ToLower(), log);
                    }
                }
                else
                {
                    // 公有云模式登录即写入
                    User.Engine.UserLogWriter.Write(log);
                }
            }
        }

        /// <summary>
        /// 用户登出事件
        /// </summary>
        /// <param name="User"></param>
        /// <param name="Request"></param>
        public static void OnUserLogout(UserValidator User, HttpRequest Request)
        {
            if (User == null)
            {
                return;
            }

            // 减少当前用户数
            //lock (_UserLogTable)
            lock (_OnlineUserIdAliasTable)
            {
                //if (!_UserLogTable.ContainsKey(User.UserID.ToLower()))
                //{
                //    return;
                //}
                if (!_OnlineUserIdAliasTable.ContainsKey(User.UserID.ToLower()))
                {
                    return;
                }
                _OnlineUserIdAliasTable.Remove(User.UserID);
                _OnlineUserIdLoginTimeTable.Remove(User.UserID);

                OThinker.H3.Tracking.UserLog log = new Tracking.UserLog(
                    Tracking.UserLogType.Logout,
                    User.UserID,
                    null,
                    null,
                    null,
                    null,
                    null,
                    null,
                    SheetUtility.GetClientIP(Request),
                    SheetUtility.GetClientPlatform(Request),
                    SheetUtility.GetClientBrowser(Request));
                if (!_UserLogTable.ContainsKey(User.UserID.ToLower()))
                {
                    _UserLogTable.Add(User.UserID.ToLower(), log);
                }
            }
        }

        //考虑共享Portal的情况，用户列表用EngineCode进行区分
        private static Dictionary<string, Dictionary<string, string>> _OnlineUserIdAliasTables = new Dictionary<string, Dictionary<string, string>>();
        private static Dictionary<string, Dictionary<string, DateTime>> _OnlineUserIdLoginTimeTables = new Dictionary<string, Dictionary<string, DateTime>>();

        private static Dictionary<string, string> _OnlineUserIdAliasTable
        {
            get
            {
                if (!_OnlineUserIdAliasTables.ContainsKey(Engine.EngineConfig.Code))
                {
                    _OnlineUserIdAliasTables.Add(Engine.EngineConfig.Code, new Dictionary<string, string>());
                }
                return _OnlineUserIdAliasTables[Engine.EngineConfig.Code];
            }
        }
        private static Dictionary<string, DateTime> _OnlineUserIdLoginTimeTable
        {
            get
            {
                if (!_OnlineUserIdLoginTimeTables.ContainsKey(Engine.EngineConfig.Code))
                {
                    _OnlineUserIdLoginTimeTables.Add(Engine.EngineConfig.Code, new Dictionary<string, DateTime>());
                }
                return _OnlineUserIdLoginTimeTables[Engine.EngineConfig.Code];
            }
        }
        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        public static System.Data.DataTable OnlineUserTable
        {
            get
            {
                lock (_OnlineUserIdAliasTable)
                {
                    System.Data.DataTable table = new System.Data.DataTable();
                    table.Columns.Add("UserID");
                    table.Columns.Add("UserAlias");
                    table.Columns.Add("LoginTime");
                    foreach (string id in _OnlineUserIdAliasTable.Keys)
                    {
                        table.Rows.Add(new object[]{
                            id,
                            _OnlineUserIdAliasTable[id],
                            _OnlineUserIdLoginTimeTable[id]                             
                        });
                    }
                    return table;
                }
            }
        }

        /// <summary>
        /// 查看worksheet文件版本号
        /// </summary>
        /// <returns></returns>
        public static string GetAppVersion()
        {
            AssemblyFileVersionAttribute fileVersion = (AssemblyFileVersionAttribute)AssemblyFileVersionAttribute.GetCustomAttribute(Assembly.GetExecutingAssembly(),
                typeof(AssemblyFileVersionAttribute));
            return fileVersion.Version;
        }
    }
}
