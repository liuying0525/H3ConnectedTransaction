using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace DongZheng.H3.WebApi.Controllers.Ajax.PusherHelper
{
    [ValidateInput(false)]
    [Xss]
    public class TestController : OThinker.H3.Controllers.ControllerBase
    {
        public override string FunctionCode => "";


        private string DESDeCode(string pToDecrypt, string sKey = "B^@s(d)+")
        {
            //    HttpContext.Current.Response.Write(pToDecrypt + "<br>" + sKey);     
            //    HttpContext.Current.Response.End();     
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

            StringBuilder ret = new StringBuilder();

            // return HttpContext.Current.Server.UrlDecode(System.Text.Encoding.Default.GetString(ms.ToArray()));  
            return System.Web.HttpUtility.UrlDecode(System.Text.Encoding.UTF8.GetString(ms.ToArray()));
        }

        public void Index()
        {
            var context = HttpContext;
            using (StreamReader sr = new StreamReader(context.Request.InputStream))
            {
                string t = sr.ReadToEnd();
                JObject d = JObject.Parse(t);
                t = DESDeCode(d["data"].ToString());
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write("Hello World");
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