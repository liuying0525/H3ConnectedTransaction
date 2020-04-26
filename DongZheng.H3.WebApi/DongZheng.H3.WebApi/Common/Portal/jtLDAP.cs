using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;


namespace DongZheng.H3.WebApi.Common.Portal
{
    public class jtLDAP
    {
        private string is_url = "LDAP://oms2.zhengtongauto.net:3402/dc=zhengtongauto,dc=com";//测试地址:LDAP://oms2.zhengtongauto.net:3402/dc=zhengtongauto,dc=com
        private string is_userid = "cn=Manager,dc=zhengtongauto,dc=com";//用户代码 cn=Manager,dc=zhengtongauto,dc=com
        private string is_password="secret";//用户密码 secret

        public string LDAP_URL
        {
            set { is_url = value; }
        }

        public string LDAP_USERID
        {
            set { is_userid = value; }
        }

        public string LDAP_PASSWORD
        {
            set { is_password = value; }
        }



        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="dm">用户代码</param>
        /// <param name="ret">返回值</param>
        /// <returns></returns>
        public string f_getPersonMes(string dm, ref string ret)
        {
            try
            {
                DirectoryEntry entry = GetEntry();

                DirectorySearcher searcher = new DirectorySearcher(entry);
                entry.Close();

                searcher.Filter = "(cn=" + dm + ")";

                SearchResult r = searcher.FindOne();

                DirectoryEntry entry2 = null;
                entry2 = r.GetDirectoryEntry();
                string description = entry2.Properties["description"][0].ToString();
                string sn = entry2.Properties["sn"][0].ToString();
                string telephoneNumber = entry2.Properties["telephoneNumber"][0].ToString();

                ret = "{\r\n";
                ret += "\"cn\":\"" + dm + "\",\r\n";
                ret += "\"sn\":\"" + sn + "\",\r\n";
                ret += "\"telephoneNumber\":\"" + telephoneNumber + "\",\r\n";
                ret += "\"description\":\"" + description + "\"\r\n";
                ret += "}";
                entry2.Close();
                return "";
            }
            catch (System.Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        /// <summary>
        /// 获得用户信息
        /// </summary>
        /// <param name="dm">用户代码</param>
        /// <param name="PropertyName"></param>
        /// <returns></returns>
        public string f_getPersonMes(string dm, string PropertyName)
        {
            try
            {
                DirectoryEntry entry = GetEntry();

                DirectorySearcher searcher = new DirectorySearcher(entry);
                entry.Close();

                searcher.Filter = "(cn=" + dm + ")";

                SearchResult r = searcher.FindOne();

                DirectoryEntry entry2 = null;
                entry2 = r.GetDirectoryEntry();


                string ret = "";
                if (entry2.Properties.Contains(PropertyName))
                {
                    ret = entry2.Properties[PropertyName][0].ToString();
                }
                else
                {
                    if (PropertyName == "dn")
                    {
                        ret = entry2.Path.ToString();
                        int i = ret.LastIndexOf("/");
                        ret = ret.Substring(i + 1);
                    }
                }



                entry2.Close();
                return ret;
            }
            catch (System.Exception ex)
            {
                return "";
            }


        }

        /// <summary>
        /// 验证用户密码
        /// </summary>
        /// <param name="dm"></param>
        /// <param name="password"></param>
        /// <returns>空代表密码正确,其他代表密码不正确</returns>
        public string f_VerifyPassword(string dm, string password)
        {
            try
            {
                DirectoryEntry entry = GetEntry();
                return "";
            }
            catch (System.Exception ex)
            {
                return ex.Message.ToString();
            }


        }

        /// <summary>
        /// 连接服务
        /// </summary>
        /// <returns></returns>
        public DirectoryEntry GetEntry()
        {
            DirectoryEntry entry = new DirectoryEntry(is_url, is_userid, is_password, AuthenticationTypes.None);
            try
            {
                object native = entry.NativeObject;
                return entry;
            }
            catch (System.Exception ex)
            {
                throw new Exception("连接失败." + ex.Message);
            }
            return null;
        }

    }
    public class LogManager
    {
        public  static string logPath = string.Empty;
        /// <summary>
        /// 保存日志的文件夹
        /// </summary>
        public static string LogPath
        {
            get
            {
                if (logPath == string.Empty)
                {
                    if (System.Web.HttpContext.Current == null)
                        // Windows Forms 应用
                        logPath = AppDomain.CurrentDomain.BaseDirectory;
                    else
                        // Web 应用
                        logPath = AppDomain.CurrentDomain.BaseDirectory + @"bin\";
                }
                if (!System.IO.Directory.Exists(logPath))
                {
                    System.IO.Directory.CreateDirectory(logPath);//不存在就创建目录
                }
                return logPath;
            }
            set { logPath = value; }
        }

        private static string logFielPrefix = string.Empty;
        /// <summary>
        /// 日志文件前缀
        /// </summary>
        public static string LogFielPrefix
        {
            get { return logFielPrefix; }
            set { logFielPrefix = value; }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(string logFile, string msg)
        {
            try
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                    LogPath + LogFielPrefix + logFile + " " +
                    DateTime.Now.ToString("yyyyMMdd") + ".Log"
                    );
                sw.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss: ") + msg);
                sw.Close();
            }
            catch
            { }
        }

        /// <summary>
        /// 写日志
        /// </summary>
        public static void WriteLog(LogFile logFile, string msg)
        {
            WriteLog(logFile.ToString(), msg);
        }
    }

    /// <summary>
    /// 日志类型
    /// </summary>
    public enum LogFile
    {
        Trace,//追踪
        Warning,
        Error,
        SQL
    }
}
