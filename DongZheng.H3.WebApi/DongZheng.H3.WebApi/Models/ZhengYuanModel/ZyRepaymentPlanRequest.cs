using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 还款计划查询
    /// </summary>
    public class ZyQueryRepaymentPlanRequest : ZyRequestBase
    {
        /// <summary>
        /// 交易服务码
        /// </summary>
        public string transCode { get { return "M900205"; } }

        /// <summary>
        /// 借据号
        /// </summary>
        public string loanId { get; set; }
    }
}