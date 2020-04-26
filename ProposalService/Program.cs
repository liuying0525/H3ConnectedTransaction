using DongZheng.H3.WebApi;
using ProposalService.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static DongZheng.H3.WebApi.Common.BPM;
using DongZheng.H3.WebApi.Common;
using System.Collections.Specialized;
using System.Net;
using System.IO;

namespace ProposalService
{
    class Program
    {
        #region 配置文件
        //RabbitMQ
        public static string host = ConfigurationManager.AppSettings["RabbitMQHost"] + string.Empty;
        public static string user = ConfigurationManager.AppSettings["RabbitMQUser"] + string.Empty;
        public static string password = ConfigurationManager.AppSettings["RabbitMQPassword"] + string.Empty;
        //queue
        public const string queue_h3_to_wxapp_response = "h3_to_wxapp_response";

        //Restful api url
        public static string url_startworkflow = ConfigurationManager.AppSettings["H3StartWorkflow"] + string.Empty;
        public static string url_cancleinstance = ConfigurationManager.AppSettings["H3CancleInstance"] + string.Empty;

        static log4net.ILog LogWriter = log4net.LogManager.GetLogger("Service");

        #endregion
        static void Main(string[] args)
        {
            WriteInfoLog("开始启动服务...");
            RabbitMQHelper rabbitMQHelper = new RabbitMQHelper(host, user, password);
            WriteInfoLog("开始监听消息队列wxapp_to_h3_startworkflow...");
            rabbitMQHelper.ReceiveMessage("wxapp_to_h3_startworkflow", StartWorkflowMessage);
            WriteInfoLog("开始监听消息队列wxapp_to_h3_cancleinstance...");
            rabbitMQHelper.ReceiveMessage("wxapp_to_h3_cancleinstance", CancleInstance);
            WriteInfoLog("监听中...");
            Console.ReadKey();
            Console.ReadLine();
        }

        #region 消息处理方法
        public static bool StartWorkflowMessage(string message)
        {
            WriteInfoLog("发起流程的消息，message-->" + message);
            StartWorkflowParameter parameter = JsonConvert.DeserializeObject<StartWorkflowParameter>(message);

            Dictionary<string, object> dicPara = JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(parameter.ParamValues));
            dicPara.Add("id", parameter.id);

            NameValueCollection nvBody = new NameValueCollection();

            nvBody.Add("USER_CODE", parameter.UserCode);
            nvBody.Add("WORKFLOW_CODE", parameter.WorkflowCode);
            nvBody.Add("FINISH_START", parameter.FinishStart ? "1" : "0");
            nvBody.Add("INSTANCE_ID", "");
            nvBody.Add("PARAM_VALUES", JsonConvert.SerializeObject(dicPara));

            var ret = postFormData(url_startworkflow, new NameValueCollection(), nvBody);
            try
            {
                RestfulResult startworkflowResult = JsonConvert.DeserializeObject<RestfulResult>(ret);
                var msg = new object();
                if (startworkflowResult.STATUS == "2")
                {
                    msg = new
                    {
                        id = parameter.id,
                        type = "startworkflow",
                        code = "1",
                        data = startworkflowResult.INSTANCE_ID,
                        message = startworkflowResult.MESSAGE
                    };
                }
                else
                {
                    msg = new
                    {
                        id = parameter.id,
                        type = "startworkflow",
                        code = "-1",
                        data = startworkflowResult.INSTANCE_ID,
                        message = startworkflowResult.MESSAGE
                    };
                }
                RabbitMQHelper rabbitMQHelper = new RabbitMQHelper(host, user, password);
                WriteInfoLog("写入h3_to_wxapp_response队列的消息：{0}", JsonConvert.SerializeObject(msg));
                rabbitMQHelper.SendMsg(JsonConvert.SerializeObject(msg), queue_h3_to_wxapp_response);
                WriteInfoLog("【完成】写入h3_to_wxapp_response队列的消息：{0}", JsonConvert.SerializeObject(msg));
            }
            catch (Exception ex)
            {
                return false;
            }
            WriteInfoLog("监听中...");
            return true;
        }

        public static bool CancleInstance(string message)
        {
            WriteInfoLog("取消流程的消息，message-->" + message);

            string instanceid = "";
            string id = "";
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(message);
            if (dic.Keys.Contains("id"))
                id = dic["id"];
            if (dic.Keys.Contains("InstanceId"))
                instanceid = dic["InstanceId"];

            var ret = PostWebRequest(url_cancleinstance, JsonConvert.SerializeObject(new { INSTANCEID = instanceid }));
            RestfulResultCancleInstance result = JsonConvert.DeserializeObject<RestfulResultCancleInstance>(ret);
            var msg = new object();
            if (result.STATUS == "1")
            {
                msg = new
                {
                    id = id,
                    type = "cancleinstance",
                    code = "1",
                    data = instanceid,
                    message = result.MESSAGE
                };
            }
            else
            {
                msg = new
                {
                    id = id,
                    type = "cancleinstance",
                    code = "-1",
                    data = instanceid,
                    message = result.MESSAGE
                };
            }
            RabbitMQHelper rabbitMQHelper = new RabbitMQHelper(host, user, password);
            WriteInfoLog("写入h3_to_wxapp_response队列的消息：{0}", JsonConvert.SerializeObject(msg));
            rabbitMQHelper.SendMsg(JsonConvert.SerializeObject(msg), queue_h3_to_wxapp_response);
            WriteInfoLog("【完成】写入h3_to_wxapp_response队列的消息：{0}", JsonConvert.SerializeObject(msg));
            WriteInfoLog("监听中...");
            return true;
        }

        public static string postFormData(string Url, NameValueCollection Header_NameValue, NameValueCollection Body_NameValue, int timeoutSecond = 10)
        {
            try
            {
                Uri uri = new Uri(Url);
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
                webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webrequest.Method = "POST";
                webrequest.Headers.Add(Header_NameValue);

                StringBuilder sb = new StringBuilder();

                //加文本
                foreach (string key in Body_NameValue.Keys)
                {
                    sb.Append("--");
                    sb.Append(boundary);
                    sb.Append("\r\n");
                    sb.Append("Content-Disposition: form-data; name=\"");
                    sb.Append(key);
                    sb.Append("\"");
                    sb.Append("\r\n");
                    sb.Append("\r\n");
                    sb.Append(Body_NameValue.Get(key));
                    sb.Append("\r\n");
                }
                string postHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                byte[] endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");
                Console.WriteLine("\r\n" + sb.ToString() + "--" + boundary + "--\r\n");
                long length = postHeaderBytes.Length + endBoundary.Length;
                webrequest.ContentLength = length;

                Stream requestStream = webrequest.GetRequestStream();

                //写入Body内的参数
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                requestStream.Write(endBoundary, 0, endBoundary.Length);
                webrequest.Timeout = timeoutSecond * 1000;
                WebResponse responce = webrequest.GetResponse();
                Stream s = responce.GetResponseStream();
                StreamReader sr = new StreamReader(s);
                string str = sr.ReadToEnd();
                requestStream.Close();
                sr.Close();
                s.Close();
                responce.Close();
                return str;
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("postFormData Exception url-->{0},msg-->{1}", Url, ex.ToString()));
                return "";
            }
        }

        public static string PostWebRequest(string postUrl, string paramData, string contentType = "application/json", int timeoutSecond = 10)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = Encoding.GetEncoding("UTF-8").GetBytes(paramData);  // 转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = contentType;
                webReq.ContentLength = byteArray.Length;
                webReq.Timeout = timeoutSecond * 1000;
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);    // 写入参数
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.GetEncoding("UTF-8")))
                    {
                        ret = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(string.Format("PostWebRequest Exception url-->{0},msg-->{1}", postUrl, ex.ToString()));
            }
            return ret;
        }
        #endregion

        #region 写日志
        static void WriteInfoLog(string format, params object[] args)
        {
            LogWriter.InfoFormat(format, args);
            Console.WriteLine(DateTime.Now.ToString() + format, args);
        }

        static void WriteInfoLog(string format)
        {
            LogWriter.Info(format);
            Console.WriteLine(DateTime.Now.ToString() + format);
        }

        static void WriteErrorLog(string format, params object[] args)
        {
            LogWriter.ErrorFormat(format, args);
            Console.WriteLine(DateTime.Now.ToString() + format, args);
        }

        static void WriteErrorLog(string format)
        {
            LogWriter.Error(format);
            Console.WriteLine(DateTime.Now.ToString() + format);
        }
        #endregion
    }
}
