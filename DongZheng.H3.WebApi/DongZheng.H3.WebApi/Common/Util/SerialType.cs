using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Util
{
    public enum SerialType
    {
        /// <summary>
        /// 通常业务[随机]
        /// </summary>
        Normal = 10,

    }

    public class SerialTypeHelper
    {
        /// <summary>
        /// 根据业务类型枚举，获取实际的业务类型和相应前缀
        /// </summary>
        /// <param name="type">业务枚举</param>
        /// <returns></returns>
        public static TypePrefix GetSerialTypeAndPrefix(SerialType type)
        {
            var tp = new TypePrefix { Type = type.ToString().ToLower() };

            switch (type)
            {
                case SerialType.Normal:
                    tp.IsRandom = true;
                    return tp;
                default:
                    tp.IsRandom = true;
                    return tp;
            }
        }
    }

    public class TypePrefix
    {
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 是否由随机生成器生成
        /// </summary>
        public bool IsRandom { get; set; }
    }
}