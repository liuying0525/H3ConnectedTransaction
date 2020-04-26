using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json.Linq;

namespace PushGradeFromH3ToCrm
{
    /// <summary>
    /// 推送类
    /// </summary>
    public class Pusher
    {
        /// <summary>
        /// 单例
        /// </summary>
        private static Pusher _self = new Pusher();
        /// <summary>
        /// 推送县城
        /// </summary>
        private Thread _pusher;
        /// <summary>
        /// 私有构造
        /// </summary>
        private Pusher() { }
        /// <summary>
        /// 获取对象
        /// </summary>
        /// <returns></returns>
        public static Pusher GetInstance()
        {
            return _self;
        }
        /// <summary>
        /// 开始推送
        /// </summary>
        /// <returns>结果</returns>
        public bool StartPusher()
        {
            try
            {
                _pusher = new Thread(new ThreadStart(PushTimer));
                _pusher.Start();
                Log.WriteLog(true, "推送服务启动成功", 0);
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(false, "推送服务启动失败：" + ex.ToString(), 0);
                return false;
            }
        }
        /// <summary>
        /// 推送实体
        /// </summary>
        private void PushTimer()
        {
            while (true)
            {
                if (Directory.Exists("./WaitPush"))
                {
                    DirectoryInfo dir = new DirectoryInfo("./WaitPush");
                    FileInfo[] fis = dir.GetFiles();
                    int filesCount = fis.Length;
                    for (int i = filesCount - 1; i >= 0; i--)
                    {
                        FileInfo file = fis[i];
                        DateTime start = DateTime.Now;
                        try
                        {
                            string sendData = "";
                            using (StreamReader sr = new StreamReader(file.FullName))
                            {
                                sendData = sr.ReadToEnd();
                            }
                            sendData = "{\"list\": " + sendData + ",\"from_type\":\"8\",\"link_no\":\"8\"}";
                            sendData = sendData.Replace("\r\n", "");
                            sendData = sendData.Replace(" ", "");
                            string encodeSendData = PushGradeFromH3ToCrm.EnCoder.DESEnCode(sendData);
                            JObject sendJson = new JObject();
                            sendJson.Add("data", encodeSendData);
                            sendJson.Add("sign", "");
                            string postData = sendJson.ToString();
                            postData = postData.Replace("\r\n", "");
                            postData = postData.Replace(" ", "");
                            byte[] data = Encoding.UTF8.GetBytes(postData);

                            HttpWebRequest req = WebRequest.Create(Config.Settings["PushUrl"]) as HttpWebRequest;
                            req.Method = "POST";
                            req.ContentType = "application/json";
                            req.ContentLength = data.Length;
                            req.KeepAlive = true;

                            using (Stream reqStream = req.GetRequestStream())
                            {
                                reqStream.Write(data, 0, data.Length);
                            }

                            HttpWebResponse rsp = req.GetResponse() as HttpWebResponse;
                            using (StreamReader sr = new StreamReader(rsp.GetResponseStream()))
                            {
                                string ret = sr.ReadToEnd();
                                JObject retJson = JObject.Parse(ret);
                                if (retJson["result_code"] != null && retJson["result_code"].ToString() == "0")//result_code为CRM返回结果
                                {
                                    Log.WriteLog(true, "推送成功。\r\n推送数据：\r\n" + sendData + "\r\n加密后：\r\n" + postData, (DateTime.Now - start).TotalSeconds, start);
                                    File.Delete(file.FullName);
                                }
                                else
                                {
                                    Log.WriteLog(true, "推送失败。返回信息：\r\n" + ret + "\r\n推送数据：\r\n" + sendData + "\r\n加密后：\r\n" + postData, (DateTime.Now - start).TotalSeconds, start);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.WriteLog(true, "推送异常。异常信息：\r\n" + ex.ToString(), (DateTime.Now - start).TotalSeconds, start);
                        }
                    }
                }
                Thread.Sleep(2 * 60 * 60 * 1000);//推送CRM的时间间隔
            }
        }
    }
}
