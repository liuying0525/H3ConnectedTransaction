using DongZheng.H3.WebApi.Common.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    public class ZyRequestBase
    {
        /// <summary>
        /// 机构Id
        /// </summary>
        public string orgId { get { return "Par_dz"; } }

        /// <summary>
        /// 版本号
        /// </summary>
        public string version { get { return "1.0.0"; } }

        /// <summary>
        /// 流水号
        /// </summary>
        private string TraceNo { get; set; } //; { get { return string.Format("{0}{1}{2}", orgId, _reqTime, SerialGenerator.Generate(SerialType.Normal, 11)); } }

        public string traceNo
        {
            get
            {
                if (string.IsNullOrEmpty(TraceNo))
                {
                    TraceNo =  string.Format("{0}{1}{2}", orgId, _reqTime, SerialGenerator.Generate(SerialType.Normal, 11));

                    return TraceNo;
                }
                return TraceNo;
            }
        }

        /// <summary>
        /// 请求时间
        /// </summary>
        private string _reqTime { get { return DateTime.Now.ToString("yyyyMMddHHmmss"); } }

        public string reqTime { get { return _reqTime; } }

        /// <summary>
        /// 产品Id
        /// </summary>
        public string productNum { get { return "CP1905130002"; } }

        /// <summary>
        /// 渠道
        /// </summary>
        public string channelNo { get { return "05"; } }
        
    }
}