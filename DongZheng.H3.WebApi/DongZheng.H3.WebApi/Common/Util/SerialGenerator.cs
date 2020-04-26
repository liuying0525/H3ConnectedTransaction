using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Util
{
    public class SerialGenerator
    {
        /// <summary>
        /// 默认长度
        /// </summary>
        private const int DefualtLength = 10;
        /// <summary>
        /// 生产单号
        /// </summary>
        /// <param name="serialType">默认长度10位</param>
        /// <returns></returns>
        public static string Generate(SerialType serialType)
        {
            TypePrefix typePrefix = SerialTypeHelper.GetSerialTypeAndPrefix(serialType);

            if (typePrefix.IsRandom)
            {
                return RandomSerial.CreateID(DefualtLength);
            }

            SerialCache cache = SerialCache.GetInstance();

            //日期5位 编号5位
            return cache.CreateID(typePrefix.Type, DefualtLength);
        }

        /// <summary>
        /// 生产指定长度的单号
        /// </summary>
        /// <param name="serialType"></param>
        /// <param name="pkLength">序列号总长度</param>
        /// <returns></returns>
        public static string Generate(SerialType serialType, int length)
        {
            TypePrefix typePrefix = SerialTypeHelper.GetSerialTypeAndPrefix(serialType);

            if (typePrefix.IsRandom)
            {
                return RandomSerial.CreateID(length);
            }

            SerialCache cache = SerialCache.GetInstance();

            return cache.CreateID(typePrefix.Type, length);
        }

        /// <summary>
        /// 生产指定长度随机编号(不带时间戳前缀)
        /// </summary>
        /// <returns></returns>
        public static string Generate(int length)
        {
            return RandomSerial.CreateRandomID(length);
        }

        /// <summary>
        /// 生产指定长度和指定叠加增量的单号
        /// </summary>
        /// <returns></returns>
        public static string GenerateCdKey(bool appendDate)
        {
            // 生产6位激活码
            return RandomSerial.CreateCdKey(6, appendDate);
        }

        /// <summary>
        /// 生成打印 POS流水单号 等编号等
        /// </summary>
        /// <returns></returns>
        public static long ExtendGenerate(SerialType serialType)
        {
            TypePrefix typePrefix = SerialTypeHelper.GetSerialTypeAndPrefix(serialType);

            SerialCache gen = SerialCache.GetInstance();

            return gen.CreateID(typePrefix.Type);
        }

        /// <summary>
        /// 随机生成手机验证码(纯数字串)
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GenerateNumber(int length)
        {
            return RandomSerial.CreateRandomNumber(length);
        }

    }
}