using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源终审结果回调
    /// </summary>
    public class ZyFinalAuditCallBackResponse : ZyResponseBase
    {
        /// <summary>
        /// 借款编号
        /// </summary>
        public string appSeq { get; set; }

        /// <summary>
        /// 审批状态
        /// </summary>
        public string apsStatus { get; set; }

        /// <summary>
        /// 审批拒绝原因
        /// </summary>
        public string apsReason { get; set; }

        /// <summary>
        /// 触碰征信规则
        /// </summary>
        public string bankCreditRules { get; set; }

        /// <summary>
        /// 待审核退回原因
        /// </summary>
        public string returnReason { get; set; }

        /// <summary>
        /// loanLimit
        /// </summary>
        public string loanLimit { get; set; }

        /// <summary>
        /// PICC审批额度
        /// </summary>
        public string piccAuditAmount { get; set; }

    }
}