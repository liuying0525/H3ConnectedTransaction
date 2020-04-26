using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace DZ.External.WebApi.Common
{
    /// <summary>
    /// 加密服务
    /// </summary>
    public class MD5Encryptor
    {
        /// <summary>
        /// 获取MD5加密后数据
        /// </summary>
        /// <param name="inputString">输入字符串</param>
        /// <returns>输出MD5加密字符串</returns>
        public static string GetMD5(string inputString)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] fromData = System.Text.Encoding.Unicode.GetBytes(inputString);
            byte[] targetData = md5.ComputeHash(fromData);
            string byte2String = null;

            for (int i = 0; i < targetData.Length; i++)
            {
                byte2String += targetData[i].ToString("x");
            }

            return byte2String;
        }

        ///// <summary>
        ///// 获取md5的32位加密后小写字符串
        ///// </summary>
        ///// <param name="inputValue"></param>
        ///// <returns></returns>
        //public static string GetUTF8MD5(string inputString)
        //{
        //    string pwd = null;
        //    MD5 md5 = MD5.Create();// 实例化一个md5对像
        //    byte[] s = md5.ComputeHash(Encoding.UTF8.GetBytes(inputString));
        //    for (int i = 0; i < s.Length; i++)
        //    {
        //        pwd = pwd + s[i].ToString("x");
        //    }
        //    return pwd;
        //}

        private const string EnglishChar = "abcdefghijklmnopqrstuvwxyz";
        private const string SpecialChar = "~!@#$%^&*()";


        /// <summary>
        /// 生成一个10位数的密码
        /// </summary>
        /// <returns></returns>
        public static string GeneratePassword()
        {
            // 因为密码不允许存在连续的数字或字母
            string password = string.Empty;
            Random r = new Random();
            // 2个字母
            password += EnglishChar[r.Next(0, 25)].ToString();
            password += EnglishChar[r.Next(0, 25)].ToString();
            // 2个数字
            password += r.Next(10, 99).ToString();
            // 2个字母
            password += EnglishChar[r.Next(0, 25)].ToString();
            password += EnglishChar[r.Next(0, 25)].ToString();
            // 2个数字
            password += r.Next(10, 99).ToString();
            // 2个特殊字符
            password += SpecialChar[r.Next(0, 9)].ToString();
            password += SpecialChar[r.Next(0, 9)].ToString();
            return password;
        }

    }
}