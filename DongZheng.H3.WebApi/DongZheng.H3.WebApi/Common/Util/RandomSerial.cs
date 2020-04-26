using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Util
{
    public class RandomSerial
    {
        /// <summary>
        /// ID基数增长跨度
        /// </summary>
        private const int _idGap = 0x100;

        /// <summary>
        /// 产生随机尾数
        /// </summary>
        private static readonly Random _random = new Random((int)DateTime.Now.Ticks);

        /// <summary>
        /// 循环增长ID
        /// </summary>
        private static int _idBase;

        /// <summary>
        /// 标线时间 (2000)
        /// </summary>
        public static readonly DateTime _markTime = new DateTime(2000, 1, 1);



        /// <summary>
        /// 产生一个16进制的数字字符串(前补零)
        /// </summary>
        /// <param name="length">需要的长度</param>
        /// <returns></returns>
        private static string CreateUniqueString(int length)
        {
            if (length <= 0) return string.Empty;

            int result = CreateUnique();

            string str = result.ToString("X").PadLeft(length, '0');

            return str.Length > length ? str.Substring(str.Length - length, length) : str;
        }

        /// <summary>
        /// 产生一个10进制的数字字符串(前补随机数字串)
        /// </summary>
        /// <param name="length">需要的长度</param>
        /// <returns></returns>
        private static string CreateUniqueNumer(int length)
        {
            if (length <= 0) return string.Empty;

            int result = CreateUnique();

            string str = result.ToString("D");

            return str.Length > length ? str.Substring(str.Length - length, length) : str;
        }

        /// <summary>
        /// 随机序列数字
        /// </summary>
        /// <returns></returns>
        private static int CreateUnique()
        {
            _idBase = (_idBase + _idGap) % 0x0fffff00;

            return _idBase + _random.Next(0xfff);
        }

        /// <summary>
        /// 生成指定长度的随机ID
        /// </summary>
        /// <param name="noLength">指定序列长度</param>
        /// <returns></returns>
        public static string CreateID(int noLength)
        {
            int totalHours = Convert.ToInt32((DateTime.Now - _markTime).TotalHours);

            string date = totalHours.ToString("X");

            //主键长度-临时中值长度-日期长度
            return string.Format("{0}{1}", date,
                CreateUniqueString(noLength - date.Length));
        }

        /// <summary>
        /// 生成指定长度和累加长度的随机ID
        /// </summary>
        /// <returns></returns>
        public static string CreateCdKey(int noLength, bool appendHours)
        {
            var keyTable = "0123456789ABCDEFGHIGKLMNOPQRSTUVWXYZ";

            var cdKeies = keyTable.ToCharArray();

            StringBuilder sb = new StringBuilder();


            for (var i = 0; i < noLength; i++)
            {
                var rand = new Random(RandomSeed());

                var index = rand.Next(0, 36);

                sb.Append(cdKeies[index]);
            }

            string result = sb.ToString();

            if (appendHours)
            {

                int totalHours = Convert.ToInt32((DateTime.Now - _markTime).TotalHours);

                string date = totalHours.ToString("X");

                result = date + result;

            }

            return result;
        }

        /// <summary>
        /// 生成指定长度的随机ID(不带时间戳前缀)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomID(int length)
        {
            string result = string.Empty;

            while (result.Length < length)
            {
                result += CreateUniqueNumer(length);
            }

            return result.Length > length ? result.Substring(result.Length - length, length) : result;
        }

        /// <summary>
        /// 创建随机数(不带时间戳纯数字)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string CreateRandomNumber(int length)
        {
            const int maxNumber = 10;

            string container = string.Empty;

            for (int i = length - 1; i >= 0; i--)
            {
                //Random random = new Random(RandomSeed());

                container += _random.Next(maxNumber).ToString();
            }

            return container;

        }

        /// <summary>
        /// 随机数
        /// </summary>
        /// <returns></returns>
        public static int RandomSeed()
        {
            byte[] bytes = new byte[4];
            System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
            rng.GetBytes(bytes);
            return BitConverter.ToInt32(bytes, 0);

        }
    }
}