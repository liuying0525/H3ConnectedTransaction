using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using OThinker.H3.Controllers;
using System.Web;
using System.Collections.Specialized;
using Newtonsoft.Json;
using DongZheng.H3.WebApi.Models;

namespace DongZheng.H3.WebApi
{
    public class HttpHelper
    {
        private static Encoding HttpEncoding = Encoding.GetEncoding("UTF-8");

        #region Post和Get请求 ------------------------
        /// <summary>
        /// 发送 Post Web 请求
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string paramData)
        {
            return PostWebRequest(postUrl, "application/x-www-form-urlencoded", paramData);
        }

        /// <summary>
        /// 发送 Post Web 请求
        /// </summary>
        /// <param name="postUrl"></param>
        /// <param name="contentType"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static string PostWebRequest(string postUrl, string contentType, string paramData, int timeoutSecond = 5, double httpVersion =1.1)
        {
            string ret = string.Empty;
            try
            {
                byte[] byteArray = HttpEncoding.GetBytes(paramData);  // 转化
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
                webReq.Method = "POST";
                webReq.ContentType = contentType;
                webReq.ContentLength = byteArray.Length;
                webReq.Timeout = timeoutSecond * 1000;
                if (httpVersion == 1.0)
                {
                    //默认1.1版本
                    webReq.ProtocolVersion = HttpVersion.Version10;
                }
                using (Stream newStream = webReq.GetRequestStream())
                {
                    newStream.Write(byteArray, 0, byteArray.Length);    // 写入参数
                }
                using (HttpWebResponse response = (HttpWebResponse)webReq.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), HttpEncoding))
                    {
                        ret = sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write(string.Format("PostWebRequest Exception url-->{0},msg-->{1}", postUrl, ex.ToString()));
            }
            return ret;
        }

        /// <summary>
        /// post请求返回Data
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="postUrl"></param>
        /// <param name="contentType"></param>
        /// <param name="paramData"></param>
        /// <returns></returns>
        public static T PostWebRequest<T>(string postUrl, string contentType, string paramData, int timeoutSecond = 5)
        {
            string ret = string.Empty;
            DZCommonApiResult<T> apiResult = null;
            try
            {
                byte[] byteArray = HttpEncoding.GetBytes(paramData);  // 转化
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
                    using (StreamReader sr = new StreamReader(response.GetResponseStream(), HttpEncoding))
                    {
                        ret = sr.ReadToEnd();
                    }
                }
                if (!string.IsNullOrEmpty(ret))
                {
                    apiResult = JsonConvert.DeserializeObject<DZCommonApiResult<T>>(ret);

                    return apiResult.data;
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write(string.Format("PostWebRequest T Exception url-->{0},msg-->{1}", postUrl, ex.ToString()));
            }

            return default(T);
        }

        /// <summary>
		/// 上传的方法
		/// </summary>
		/// <param name="uploadfile">单个文件名（上传多个文件的方法自己修改）</param>
		/// <param name="url">post请求的url</param>
		/// <param name="poststring">post的字符串 键值对，相当于表单上的文本框里的字符</param>
		/// <returns></returns>
		public static string UploadFileEx(string Url, NameValueCollection Header_NameValue, NameValueCollection Body_NameValue, string FileField, string FileName, byte[] FileContent, string FileType, 
            int timeoutSecond = 5, double httpVersion = 1.1)
        {
            try
            {
                Uri uri = new Uri(Url);
                string boundary = "----------" + DateTime.Now.Ticks.ToString("x");
                HttpWebRequest webrequest = (HttpWebRequest)WebRequest.Create(uri);
                webrequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webrequest.Method = "POST";
                webrequest.Headers.Add(Header_NameValue);
                if (httpVersion == 1.0)
                {
                    webrequest.ProtocolVersion = HttpVersion.Version10;
                }
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
                if (FileContent != null)
                {
                    //加文件
                    sb.Append("--");
                    sb.Append(boundary);
                    sb.Append("\r\n");
                    sb.Append("Content-Disposition: form-data; name=\"" + FileField + "\";");
                    sb.Append("filename=\"");
                    sb.Append(Path.GetFileName(FileName));
                    sb.Append("\"");
                    sb.Append("\r\n");
                    sb.Append("Content-Type:application/octet-stream");
                    sb.Append("\r\n");
                    sb.Append("\r\n");
                }
                string postHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(postHeader);

                byte[] endBoundary = Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");

                long length = postHeaderBytes.Length + FileContent.Length + endBoundary.Length;
                webrequest.ContentLength = length;

                Stream requestStream = webrequest.GetRequestStream();

                //写入Body内的参数
                requestStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);
                //写入文件内容
                if (FileContent != null)
                {
                    requestStream.Write(FileContent, 0, FileContent.Length);
                }
                requestStream.Write(endBoundary, 0, endBoundary.Length);
                webrequest.Timeout = timeoutSecond * 1000;//10s超时
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
                AppUtility.Engine.LogWriter.Write(string.Format("UploadFileEx Exception url-->{0},msg-->{1}", Url, ex.ToString()));
                return "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Header_NameValue"></param>
        /// <param name="Body_NameValue"></param>
        /// <param name="timeoutSecond">超时时间（秒，默认5秒）</param>
        /// <returns></returns>
        public static string postFormData(string Url, NameValueCollection Header_NameValue, NameValueCollection Body_NameValue, int timeoutSecond = 5)
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
                AppUtility.Engine.LogWriter.Write("\r\n" + sb.ToString() + "--" + boundary + "--\r\n");
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
                AppUtility.Engine.LogWriter.Write(string.Format("postFormData Exception url-->{0},msg-->{1}", Url, ex.ToString()));
                return "";
            }
        }
        /// <summary>
        /// 发送 Get Web 请求
        /// </summary>
        /// <param name="getUrl"></param>
        /// <param name="dataEncode"></param>
        /// <returns></returns>
        public static string GetWebRequest(string getUrl, int timeoutSecond = 5)
        {
            string ret = string.Empty;
            try
            {
                HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create(getUrl);
                webReq.Timeout = timeoutSecond * 1000;
                using (HttpWebResponse webReponse = (HttpWebResponse)webReq.GetResponse())
                {
                    using (Stream stream = webReponse.GetResponseStream())
                    {
                        //byte[] rsByte = new Byte[webReponse.ContentLength];

                        //stream.Read(rsByte, 0, (int)webReponse.ContentLength);
                        //ret = System.Text.Encoding.UTF8.GetString(rsByte, 0, rsByte.Length).ToString();
                        using (StreamReader sr = new StreamReader(stream))
                        {
                            ret = sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppUtility.Engine.LogWriter.Write(string.Format("GetWebRequest Exception url-->{0},msg-->{1}", getUrl, ex.ToString()));
            }
            return ret;
        }
        /// <summary>
        /// get数据通用方法
        /// </summary>
        /// <param name="strURL">拼接好的链接</param>
        /// <returns></returns>
        public static string get(string strURL)//get数据通用方法
        {
            string json = "";
            System.Net.HttpWebRequest request;
            // 创建一个HTTP请求
            request = (HttpWebRequest)WebRequest.Create(strURL);
            System.Net.HttpWebResponse response;
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            json = myreader.ReadToEnd();
            myreader.Close();
            return json;
        }
        
        /// <summary>
        /// post数据通用方法
        /// </summary>
        /// <param name="strURL">要post到的URL</param>
        /// <param name="paraUrlCoded">要post过去的字符串</param>
        /// <returns></returns>
        public static string post(string strURL, string paraUrlCoded)//post数据通用方法
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(strURL);
            System.Net.HttpWebResponse response;
            //Post请求方式
            request.Method = "POST";
            // 内容类型
            request.ContentType = "application/x-www-form-urlencoded";
            // 参数经过URL编码
            byte[] payload;
            //将URL编码后的字符串转化为字节
            payload = System.Text.Encoding.UTF8.GetBytes(paraUrlCoded);
            //设置请求的 ContentLength 
            request.ContentLength = payload.Length;
            //获得请 求流
            System.IO.Stream writer = request.GetRequestStream();
            //将请求参数写入流
            writer.Write(payload, 0, payload.Length);
            // 关闭请求流
            writer.Close();
            // 获得响应流
            response = (System.Net.HttpWebResponse)request.GetResponse();
            System.IO.StreamReader myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            myreader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string responseText = myreader.ReadToEnd();
            myreader.Close();
            return responseText;
        }
        #endregion
    }

}
