using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源初审结果回调
    /// </summary>
    public class ZyFirstAuditCallBackResponse : ZyResponseBase
    {
        /// <summary>
        /// 绑定流水号
        /// </summary>
        public string transactionNo { get; set; }

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
    }
}