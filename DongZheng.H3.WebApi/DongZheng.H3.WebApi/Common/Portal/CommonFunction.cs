using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using OThinker.H3.Controllers;
using System.Net;
using System.Configuration;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Converters;
using OThinker.H3.BizBus;
using OThinker.H3.BizBus.BizService;
using OThinker.H3;
/// <summary>
/// CommonFunction 的摘要说明
/// </summary>

namespace DongZheng.H3.WebApi.Common.Portal
{
    public class CommonFunction
    {
        public CommonFunction()
        {
            //
            //  TODO: 在此处添加构造函数逻辑
            //
        }
        private static IEngine _Engine = null;
        /// <summary>
        /// 流程引擎的接口，该接口会比this.Engine的方式更快，因为其中使用了缓存
        /// </summary>
        public static IEngine Engine
        {
            get
            {
                if (OThinker.H3.Controllers.AppConfig.ConnectionMode == ConnectionStringParser.ConnectionMode.Mono)
                {
                    return OThinker.H3.Controllers.AppUtility.Engine;
                }
                return _Engine;
            }
            set
            {
                _Engine = value;
            }
        }
        public static bool hasData(object obj)
        {
            bool rtn = true;
            if (obj == null)
            {
                return false;
            }
            if (obj.GetType() == typeof(string))
            {
                if ((string)obj == "")
                {
                    rtn = false;
                }
            }
            if (obj.GetType() == typeof(DataTable))
            {
                if (((DataTable)obj).Rows.Count < 1)
                {
                    rtn = false;
                }
            }
            if (obj.GetType() == typeof(DataSet))
            {
                DataSet ds = (DataSet)obj;
                if (ds.Tables.Count < 1 || ds.Tables[0].Rows.Count < 1)
                {
                    rtn = false;
                }
            }
            return rtn;
        }
        /// <summary>  
        /// dataTable转换成Json格式  
        /// </summary>  
        /// <param name="dt"></param>  
        /// <returns></returns>  
        public static string DataTable2Json(System.Data.DataTable dt)
        {
            StringBuilder jsonBuilder = new StringBuilder();
            jsonBuilder.Append("{\"Name\":\"" + dt.TableName + "\",\"Rows");
            jsonBuilder.Append("\":[");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                jsonBuilder.Append("{");
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    jsonBuilder.Append("\"");
                    jsonBuilder.Append(dt.Columns[j].ColumnName);
                    jsonBuilder.Append("\":\"");
                    jsonBuilder.Append(dt.Rows[i][j].ToString().Replace("\"", "\\\"")); //对于特殊字符，还应该进行特别的处理。
                    jsonBuilder.Append("\",");
                }
                jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                jsonBuilder.Append("},");
            }
            jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            jsonBuilder.Append("]");
            jsonBuilder.Append("}");
            return jsonBuilder.ToString();
        }
        /// <summary>  
        /// 利用反射将DataTable转换为List<T>对象
        /// </summary>  
        /// <param name="dt">DataTable 对象</param>  
        /// <returns>List<T>集合</returns>  
        public static List<T> DataTableToList<T>(DataTable dt) where T : class, new()
        {
            // 定义集合  
            List<T> ts = new List<T>();
            //定义一个临时变量  
            string tempName = string.Empty;
            //遍历DataTable中所有的数据行  
            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();
                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();
                //遍历该对象的所有属性  
                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;//将属性名称赋值给临时变量  
                    //检查DataTable是否包含此列（列名==对象的属性名）    
                    if (dt.Columns.Contains(tempName))
                    {
                        //取值  
                        object value = dr[tempName];
                        //如果非空，则赋给对象的属性  
                        if (value != DBNull.Value)
                        {
                            pi.SetValue(t, value, null);
                        }
                    }
                }
                //对象添加到泛型集合中  
                ts.Add(t);
            }
            return ts;
        }
        public static string Obj2Json<T>(T data)
        {
            try
            {
                System.Runtime.Serialization.Json.DataContractJsonSerializer serializer = new System.Runtime.Serialization.Json.DataContractJsonSerializer(data.GetType());
                using (MemoryStream ms = new MemoryStream())
                {
                    serializer.WriteObject(ms, data);
                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
            catch
            {
                return null;
            }
        }
        /// <summary> 
        /// xml文件转化为实体类列表
        /// </summary> 
        /// <typeparam name="T">实体名称</typeparam>         
        /// <param name="xml">您的xml文件</param>         
        /// <param name="headtag">xml头文件</param>         
        /// <returns>实体列表</returns> 
        public static List<T> XmlToObjList<T>(string xml, string headtag) where T : new()
        {
            List<T> list = new List<T>();
            XmlDocument doc = new XmlDocument();
            PropertyInfo[] propinfos = null;
            doc.LoadXml(xml);
            //XmlNodeList nodelist = doc.SelectNodes(headtag); 
            XmlNodeList nodelist = doc.GetElementsByTagName(headtag);
            foreach (XmlNode node in nodelist)
            {
                T entity = new T();
                //初始化propertyinfo                 
                if (propinfos == null)
                {
                    Type objtype = entity.GetType();
                    propinfos = objtype.GetProperties();
                }
                //填充entity类的属性 
                foreach (PropertyInfo propinfo in propinfos)
                {
                    //实体类字段首字母变成小写的 
                    string name = propinfo.Name.Substring(0, 1) + propinfo.Name.Substring(1, propinfo.Name.Length - 1);
                    XmlNode cnode = node.SelectSingleNode(name);
                    string v = cnode.InnerText;
                    if (v != null)
                        propinfo.SetValue(entity, Convert.ChangeType(v, propinfo.PropertyType), null);
                }
                list.Add(entity);
            }
            return list;
        }
        // <summary> 
        ///  实体类序列化成xml
        /// </summary>
        /// <param name="enitities">实体.</param>         
        /// <param name="headtag">节点名称</param>         
        /// <returns></returns> 
        public static string ObjListToXml<T>(List<T> enitities, string headtag)
        {
            StringBuilder sb = new StringBuilder(); PropertyInfo[] propinfos = null;
            sb.AppendLine("<?xml version=\"1.0 \" encoding=\"utf - 8 \"?>");
            sb.AppendLine("<" + headtag + ">");
            foreach (T obj in enitities)
            {
                //初始化propertyinfo                 
                if (propinfos == null)
                {
                    Type objtype = obj.GetType();
                    propinfos = objtype.GetProperties();
                }
                sb.AppendLine("<item>");
                foreach (PropertyInfo propinfo in propinfos)
                {
                    sb.Append("<");
                    sb.Append(propinfo.Name); sb.Append(">");
                    sb.Append(propinfo.GetValue(obj, null)); sb.Append("</");
                    sb.Append(propinfo.Name); sb.AppendLine(">");
                }
                sb.AppendLine("</item>");

            }
            sb.AppendLine("</" + headtag + ">");
            return sb.ToString();
        }
        /// <summary>
        /// 添加日志
        /// </summary>
        /// <param name="LogDire"></param>
        /// <param name="logName"></param>
        /// <param name="logInfo"></param>
        public static string AddLog(string folder, string logName, string logInfo)
        {
            string rtn = "true";
            try
            {
                string LogDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Log\\" + folder + "\\";
                if (!Directory.Exists(LogDire))
                {
                    // 不存在，创建
                    Directory.CreateDirectory(LogDire);
                }
                string path = LogDire + logName + ".txt";
                if (!File.Exists(path))
                {
                    // 不存在，创建
                    FileStream fs = File.Create(path);
                    fs.Close();

                    File.WriteAllText(path, logInfo, Encoding.Default);
                }
                else
                {
                    File.AppendAllText(path, logInfo, Encoding.Default);
                }
            }
            catch
            {
                rtn = "fase";
            }
            return rtn;
        }
        public static string ReadLog(string LogDire, string logName)
        {
            string rtn = "";
            try
            {
                if (LogDire == "")
                {
                    LogDire = AppDomain.CurrentDomain.BaseDirectory.ToString() + "rsLog\\";
                }
                string strFileName = LogDire + "\\" + logName;
                if (File.Exists(strFileName))
                {
                    Stream stream1 = new FileStream(strFileName, FileMode.Open);
                    StreamReader sr = new StreamReader(stream1, Encoding.Default);
                    rtn = sr.ReadToEnd().ToString();
                    sr.Close();
                    stream1.Close();
                }

            }
            catch
            {
                rtn = "";
            }
            return rtn;
        }
        public static DataTable ExecuteDataTableSql(string sql)
        {
            return Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteDataTable(sql);
        }
        public static DataTable ExecuteDataTableSql(string connectionCode, string sql)
        {
            var dbObject = Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteDataTable(sql);
            }
            return null;
        }
        public static object ExecuteScalar(string connectionCode, string sql)
        {
            var dbObject = OThinker.H3.Controllers.AppUtility.Engine.SettingManager.GetBizDbConnectionConfig(connectionCode);
            if (dbObject != null)
            {
                OThinker.Data.Database.CommandFactory factory = new OThinker.Data.Database.CommandFactory(dbObject.DbType, dbObject.DbConnectionString);
                var command = factory.CreateCommand();
                return command.ExecuteScalar(sql);
            }
            return null;
        }
        public static void WriteH3Log(string message)
        {
            Engine.LogWriter.Write(message);
        }
        public static string getConfigurationAppSettingsKeyValue(string AppSettingsKey)
        {
            return ConfigurationManager.AppSettings[AppSettingsKey] + string.Empty;
        }
        /// <summary>
        /// 后台发送post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="param">传送参数</param>
        /// <returns></returns>
        public static string PostHttp(string url, string param)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            //request.Timeout = 5000;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            //if (response.StatusCode == HttpStatusCode.OK)
            //{

            //}
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
        /// <summary>
        /// 后台发送post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <param name="Timeout">毫秒</param>
        /// <returns></returns>
        public static string PostHttp(string url, string param, int Timeout)
        {
            string strURL = url;
            System.Net.HttpWebRequest request;
            request = (HttpWebRequest)WebRequest.Create(strURL);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            string paraUrlCoded = param;
            byte[] payload;
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            request.ContentLength = payload.Length;
            Stream writer = request.GetRequestStream();
            writer.Write(payload, 0, payload.Length);
            writer.Close();
            System.Net.HttpWebResponse response;
            request.Timeout = Timeout;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            //if (response.StatusCode == HttpStatusCode.OK)
            //{

            //}
            System.IO.Stream s;
            s = response.GetResponseStream();
            string StrDate = "";
            string strValue = "";
            StreamReader Reader = new StreamReader(s, Encoding.UTF8);
            while ((StrDate = Reader.ReadLine()) != null)
            {
                strValue += StrDate + "\r\n";
            }
            return strValue;
        }
        /// <summary>
        /// 32位MD5加密
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static string MD5Encrypt32(string targetValue)
        {
            string cl = targetValue;
            string pwd = "";
            MD5 md5 = MD5.Create(); //实例化一个md5对像
            // 加密后是一个字节类型的数组，这里要注意编码UTF8/Unicode等的选择　
            byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(cl));
            // 通过使用循环，将字节类型的数组转换为字符串，此字符串是常规字符格式化所得
            for (int i = 0; i < s.Length; i++)
            {
                // 将得到的字符串使用十六进制类型格式。格式后的字符是小写的字母，如果使用大写（X）则格式后的字符是大写字符 
                pwd = pwd + s[i].ToString("X2");
            }
            return pwd;
        }
        /// <summary>
        /// Table转json
        /// </summary>
        /// <param name="dt">DataTable</param>
        /// <returns></returns>
        public static string SerializeDataTableToJson(DataTable dt)
        {
            string rtn = "";
            IsoDateTimeConverter timeConverter = new IsoDateTimeConverter { DateTimeFormat = "yyyy'-'MM'-'dd HH':'mm':'ss" };
            rtn = Newtonsoft.Json.JsonConvert.SerializeObject(dt, Newtonsoft.Json.Formatting.Indented, timeConverter);
            return rtn;

        }
        public static string get_uft8(string unicodeString)
        {
            UTF8Encoding utf8 = new UTF8Encoding();
            Byte[] encodedBytes = utf8.GetBytes(unicodeString);
            String decodedString = utf8.GetString(encodedBytes);
            return decodedString;
        }
        /// <summary>
        /// 后台调用业务服务
        /// </summary>
        /// <param name="serviceCode">业务服务编码</param>
        /// <param name="serviceMethod">方法名称</param>
        /// <param name="dicParams">参数字典</param>
        public static Dictionary<string, object> ExecuteBizBus(string serviceCode, string serviceMethod, Dictionary<string, object> dicParams)
        {
            try
            {
                // 获得业务方法
                MethodSchema method = AppUtility.Engine.BizBus.GetMethod(serviceCode, serviceMethod);
                // 获得参数列表
                BizStructure param = null;
                if (dicParams != null)
                {
                    // 填充业务方法需要的参数
                    param = BizStructureUtility.Create(method.ParamSchema);
                    foreach (var item in dicParams)
                    {
                        param[item.Key] = item.Value;
                    }
                }
                // 调用方法返回结果
                BizStructure ret = null;

                // 调用方法，获得返回结果
                ret = AppUtility.Engine.BizBus.Invoke(
                     new BizServiceInvokingContext(
                         OThinker.Organization.User.AdministratorID,
                         serviceCode,
                         method.ServiceVersion,
                         method.MethodName,
                         param));
                Dictionary<string, object> result = new Dictionary<string, object>();
                if (ret != null && ret.Schema != null)
                {
                    foreach (ItemSchema item in ret.Schema.Items)
                    {
                        result.Add(item.Name, ret[item.Name]);
                    }
                }
                return result;
            }
            catch (Exception ex)
            {
                // 调用错误日志记录
                AppUtility.Engine.LogWriter.Write("业务服务调用错误：" + ex);
                return null;
            }
        }
    }
    /// <summary>
    /// Xml序列化与反序列化
    /// </summary>
    public class XmlUtil
    {
        #region 反序列化
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="xml">XML字符串</param>
        /// <returns></returns>
        public static object Deserialize(Type type, string xml)
        {
            try
            {
                using (StringReader sr = new StringReader(xml))
                {
                    XmlSerializer xmldes = new XmlSerializer(type);
                    return xmldes.Deserialize(sr);
                }
            }
            catch (Exception e)
            {

                return null;
            }
        }
        public static object Deserialize1(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="type"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static object Deserialize(Type type, Stream stream)
        {
            XmlSerializer xmldes = new XmlSerializer(type);
            return xmldes.Deserialize(stream);
        }
        #endregion

        #region 序列化
        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="obj">对象</param>
        /// <returns></returns>
        public static string Serializer(Type type, object obj)
        {
            MemoryStream Stream = new MemoryStream();
            XmlSerializer xml = new XmlSerializer(type);
            try
            {
                //序列化对象
                xml.Serialize(Stream, obj);
            }
            catch (InvalidOperationException)
            {
                throw;
            }
            Stream.Position = 0;
            StreamReader sr = new StreamReader(Stream);
            string str = sr.ReadToEnd();

            sr.Dispose();
            Stream.Dispose();

            return str;
        }

        #endregion
    }
}