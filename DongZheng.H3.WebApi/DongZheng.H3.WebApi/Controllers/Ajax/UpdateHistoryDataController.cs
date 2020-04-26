using Newtonsoft.Json.Linq;
using OThinker.H3.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax
{
    [ValidateInput(false)]
    [Xss]
    public class UpdateHistoryDataController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => throw new NotImplementedException();

        private static readonly WebClient Web = new WebClient();
        public void Index()
        {
            var context = HttpContext;
            context.Response.ContentType = "text/plain";
            string data = "", content = "";
            AppUtility.Engine.LogWriter.Write("--------------------------------------------------------------------------------CRM_To_H3--更--新--历--史--数--据--------------------------------------------------------------------------------");
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                content = data = sr.ReadToEnd();
                JObject jtoken = JObject.Parse(data);
                data = jtoken["data"].ToString();
            }
            data = DESDecrypt(data, "B^@s(d)+");
            JObject jsonData = JObject.Parse(data);
            int index = 0;
            try { index = Convert.ToInt32(context.Application["CRMDATAINDEX"]) + 1; }
            catch (Exception e) { }
            AppUtility.Engine.LogWriter.Write("CRM_To_H3更新历史------第" + index + "条------数据-------------------CRMID：" + jsonData["third_system_no"]);
            string sql = "SELECT OBJECTID FROM I_ALLOWIN WHERE CRMDEALERID = '" + jsonData["third_system_no"] + "'";
            string objectid = AppUtility.Engine.EngineConfig.CommandFactory.CreateCommand().ExecuteScalar(sql) + string.Empty;
            //string result = HttpPost("http://localhost:8011/Portal/Risk_Management/" + (string.IsNullOrEmpty(objectid) ? "CRM_To_H3_AccedeCompany.ashx" : "CRM_To_H3_ModifyInfomationOfAccedeCompany.ashx"), content);
            string result = HttpPost("http://localhost:8011/Portal/" + (string.IsNullOrEmpty(objectid) ? "CRM_To_H3_AccedeCompany/Index" : "CRM_To_H3_ModifyInfomationOfAccedeCompany/Index"), content);
            context.Application["CRMDATAINDEX"] = index;
            context.Response.Write(result);
        }
        public static string HttpPost(string postUrl, string postDataStr)
        {
            string returnValue;
            try
            {
                byte[] byteData = Encoding.UTF8.GetBytes(postDataStr);
                Uri uri = new Uri(postUrl);
                HttpWebRequest webReq = (HttpWebRequest)WebRequest.Create(uri);
                webReq.Method = "POST";
                webReq.ContentType = "application/x-www-form-urlencoded";
                webReq.ContentLength = byteData.Length;
                //定义Stream信息
                using (Stream stream = webReq.GetRequestStream())
                {
                    stream.Write(byteData, 0, byteData.Length);
                    //stream.Close();
                }
                //获取返回信息
                HttpWebResponse response = (HttpWebResponse)webReq.GetResponse();
                using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    returnValue = streamReader.ReadToEnd();
                    //关闭信息
                    //streamReader.Close();
                }
                response.Close();
                //stream.Close();
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
            return returnValue;
        }
        public static string DESDecrypt(string pToDecrypt, string sKey = "test")
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = new byte[pToDecrypt.Length / 2];
            for (int x = 0; x < pToDecrypt.Length / 2; x++)
            {
                int i = (Convert.ToInt32(pToDecrypt.Substring(x * 2, 2), 16));
                inputByteArray[x] = (byte)i;
            }
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //StringBuilder ret = new StringBuilder();
            return System.Web.HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}