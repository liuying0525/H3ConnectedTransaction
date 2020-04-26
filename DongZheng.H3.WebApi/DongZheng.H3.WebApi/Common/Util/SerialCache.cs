using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Util
{
    public class SerialCache
    {
        private const string CACHE_KEY_PREFIX = "urn:sid:";

        private static volatile SerialCache _instance;

        private static readonly object Lock = new object();

        /// <summary>
        /// 标线时间 (2000)
        /// </summary>
        private static readonly DateTime _markTime = new DateTime(2000, 1, 1);

        private SerialCache()
        {
        }

        /// <summary>
        /// 序列号缓存
        /// </summary>
        /// <returns></returns>
        public static SerialCache GetInstance()
        {
            if (_instance == null)
            {
                lock (Lock)
                {
                    if (_instance == null)
                    {
                        _instance = new SerialCache();

                        Thread.MemoryBarrier();
                    }
                }
            }

            return _instance;
        }

        /// <summary>
        /// 创建指定长度的编号ID(按小时增长[日期戳(5-6)位]
        /// </summary>
        /// <param name="type">类型</param>
        /// <param name="pkLength">编号总长度</param>
        /// <returns></returns>
        public string CreateID(string type, int pkLength)
        {
            int totalHours = Convert.ToInt32((DateTime.Now - _markTime).TotalHours);

            string dateStr = totalHours.ToString("X");

            var sublen = pkLength - dateStr.Length;

            string key = CACHE_KEY_PREFIX + type + dateStr;

            //using(PooledRedisClientManager pm = RedisPooledManager.Instance.GetPooledClientManager())
            //using (IRedisClient client = pm.GetClient())
            //{
            //    //操作加锁
            //    //using (cl.AcquireLock(key))
            //    //{
            //    long no = client.Increment(key, 1);
            //    //}

            //    //首次执行时设置过期时间
            //    if (no < 2)
            //    {
            //        client.ExpireEntryAt(key, DateTime.Now.AddMinutes(75));
            //    }

            //    string noStr = no.ToString();

            //    noStr = noStr.Length < sublen
            //        ? noStr.PadLeft(sublen, '0')
            //        : noStr.Substring(noStr.Length - sublen, sublen);

            //    // 日期戳 5 + 生成编号N位
            //    return string.Format("{0}{1}", dateStr, noStr);
            //}
            throw new NotImplementedException();
        }

        /// <summary>
        /// 生成扩展编号（按天增长：长度=年月日8位+自增长数字长度）
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public long CreateID(string type)
        {
            string dateStr = string.Format("{0:yyyyMMdd}", DateTime.Now);

            string key = CACHE_KEY_PREFIX + type + dateStr;

            //using (PooledRedisClientManager pm = RedisPooledManager.Instance.GetPooledClientManager())
            //using (IRedisClient cl = pm.GetClient())
            //{
            //    //操作加锁
            //    //using (cl.AcquireLock(key))
            //    //{
            //    long no = cl.Increment(key, 1);
            //    //首次执行时设置过期时间
            //    if (no < 2)
            //    {
            //        cl.ExpireEntryAt(key, DateTime.Now.AddHours(24));
            //    }

            //    //}

            //    return Int64.Parse(dateStr + no);
            //}
            throw new NotImplementedException();
        }
    }
}