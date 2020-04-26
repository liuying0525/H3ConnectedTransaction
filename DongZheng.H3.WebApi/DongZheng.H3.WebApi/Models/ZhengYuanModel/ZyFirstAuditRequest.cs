using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源初审--请求模型
    /// </summary>
    public class ZyFirstAuditRequest : ZyRequestBase
    {
        /// <summary>
        /// 交易服务码
        /// </summary>
        public string transCode { get { return "M990000"; } }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string idNo { get; set; }

        /// <summary>
        /// 证件提交类型 01:身份证(默认并唯一)
        /// </summary>
        public string idType { get { return "01"; } }

        /// <summary>
        ///名称 
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 银行预留手机号
        /// </summary>
        public string bankCardReservePhone { get; set; }

        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCardNo { get; set; }

        /// <summary>
        /// 手机验证码
        /// </summary>
        public string identityCode { get; set; }

        /// <summary>
        /// 绑定流水号
        /// </summary>
        public string transactionNo { get; set; }
    }
}