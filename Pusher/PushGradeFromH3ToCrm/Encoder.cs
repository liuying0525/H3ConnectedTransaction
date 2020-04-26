using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PushGradeFromH3ToCrm
{
    /// <summary>
    /// 加解密算法类
    /// </summary>
    public class EnCoder
    {
        /// <summary>
        /// 秘钥
        /// </summary>
        public static string DES_Key = "B^@s(d)+";

        #region DESEnCode DES加密
        /// <summary>
        /// 加密算法
        /// </summary>
        /// <param name="pToEncrypt">加密前字符串</param>
        /// <param name="sKey">秘钥</param>
        /// <returns>加密后字符串</returns>
        public static string DESEnCode(string pToEncrypt, string sKey = "B^@s(d)+")
        {
            // string pToEncrypt1 = HttpContext.Current.Server.UrlEncode(pToEncrypt);     
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            pToEncrypt = System.Web.HttpUtility.UrlEncode(pToEncrypt);
            byte[] inputByteArray = Encoding.UTF8.GetBytes(pToEncrypt);

            //建立加密对象的密钥和偏移量      
            //原文使用ASCIIEncoding.ASCII方法的GetBytes方法      
            //使得输入密码必须输入英文文本      
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);

            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            StringBuilder ret = new StringBuilder();
            foreach (byte b in ms.ToArray())
            {
                ret.AppendFormat("{0:X2}", b);
            }
            ret.ToString();
            return ret.ToString();
        }
        #endregion

        #region DESDeCode DES解密
        /// <summary>
        /// 解密算法
        /// </summary>
        /// <param name="pToDecrypt">解密前字符串</param>
        /// <param name="sKey">秘钥</param>
        /// <returns>解密后字符串</returns>  
        public static string DESDeCode(string pToDecrypt, string sKey = "B^@s(d)+")
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
        #endregion

    }
}
