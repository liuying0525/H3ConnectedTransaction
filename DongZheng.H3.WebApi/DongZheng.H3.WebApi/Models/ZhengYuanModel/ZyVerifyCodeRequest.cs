using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源获取初审短信验证码--请求模型
    /// </summary>
    public class ZyVerifyCodeRequest : ZyRequestBase
    {
        /// <summary>
        /// 交易服务码
        /// </summary>
        public string transCode { get { return "M990730"; } }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string idNo { get; set; }

        /// <summary>
        /// 证件提交类型 01:身份证(默认并唯一)
        /// </summary>
        public string idType { get { return "01"; } }

        /// <summary>
        ///客户名称 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobile { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCardNo { get; set; }

    }
}