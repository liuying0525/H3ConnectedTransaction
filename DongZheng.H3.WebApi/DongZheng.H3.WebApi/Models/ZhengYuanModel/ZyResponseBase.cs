using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    public class ZyResponseBase
    {
        /// <summary>
        /// 机构Id
        /// </summary>
        public string orgId { get; set; }

        /// <summary>
        /// 接口版本
        /// </summary>
        public string version { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string traceNo { get; set; }

        /// <summary>
        /// 响应时间
        /// </summary>
        public string resTime { get; set; }

        /// <summary>
        /// 交易服务码
        /// </summary>
        public string transCode { get; set; }

        /// <summary>
        /// 响应码
        /// </summary>
        public string returnCode { get; set; }

        public string returnMsg { get; set; }

        /// <summary>
        /// 产品Id
        /// </summary>
        public string productNum { get; set; }
    }
}