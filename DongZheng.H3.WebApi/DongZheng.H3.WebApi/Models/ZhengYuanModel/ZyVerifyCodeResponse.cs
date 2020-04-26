using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源获取初审短信验证码--响应模型
    /// </summary>
    public class ZyVerifyCodeResponse : ZyResponseBase
    {
        /// <summary>
        /// 绑定流水号
        /// </summary>
        public string transactionNo { get; set; }

        /// <summary>
        /// 签约流水号
        /// </summary>
        public string signNo { get; set; }
    }
}