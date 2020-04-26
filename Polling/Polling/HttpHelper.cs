using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

class HttpHelper
{
    private Encoding HttpEncoding = Encoding.GetEncoding("UTF-8");

    #region Post和Get请求 ------------------------
    /// <summary>
    /// 发送 Post Web 请求
    /// </summary>
    /// <param name="postUrl"></param>
    /// <param name="paramData"></param>
    /// <returns></returns>
    public string PostWebRequest(string postUrl, string paramData)
    {
        string ret = string.Empty;
        try
        {
            byte[] byteArray = HttpEncoding.GetBytes(paramData);  // 转化
            HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(new Uri(postUrl));
            webReq.Method = "POST";
            webReq.ContentType = "application/x-www-form-urlencoded";
            webReq.ContentLength = byteArray.Length;
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
            //AppUtility.Engine.LogWriter.Write(ex.ToString());
        }
        return ret;
    }

    /// <summary>
    /// 发送 Get Web 请求
    /// </summary>
    /// <param name="getUrl"></param>
    /// <param name="dataEncode"></param>
    /// <returns></returns>
    public string GetWebRequest(string getUrl)
    {
        string ret = string.Empty;

        try
        {
            HttpWebRequest webReq = (HttpWebRequest)HttpWebRequest.Create(getUrl);
            using (HttpWebResponse webReponse = (HttpWebResponse)webReq.GetResponse())
            {
                using (Stream stream = webReponse.GetResponseStream())
                {
                    byte[] rsByte = new Byte[webReponse.ContentLength];

                    stream.Read(rsByte, 0, (int)webReponse.ContentLength);
                    ret = System.Text.Encoding.UTF8.GetString(rsByte, 0, rsByte.Length).ToString();
                }
            }
        }
        catch (Exception ex)
        {
            //AppUtility.Engine.LogWriter.Write(ex.ToString());
        }
        return ret;


    }
    /// <summary>
    /// get数据通用方法
    /// </summary>
    /// <param name="strURL">拼接好的链接</param>
    /// <returns></returns>
    public string get(string strURL)//get数据通用方法
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
    /// 判断配置文件，获取accesstoken字符串
    /// </summary>
    /// <returns></returns>

    /// <summary>
    /// post数据通用方法
    /// </summary>
    /// <param name="strURL">要post到的URL</param>
    /// <param name="paraUrlCoded">要post过去的字符串</param>
    /// <returns></returns>
    public string post(string strURL, string paraUrlCoded)//post数据通用方法
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

