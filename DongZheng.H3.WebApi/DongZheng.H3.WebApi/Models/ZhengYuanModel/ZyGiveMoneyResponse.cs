using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源放款通知
    /// </summary>
    public class ZyGiveMoneyResponse : ZyResponseBase
    {
        /// <summary>
        /// 批次号
        /// </summary>
        public string batchNo { get; set; }

        public List<lpbApplDnList> lpbApplDnList { get; set; }
    }

    public class lpbApplDnList
    {
        /// <summary>
        /// 担保系统借款编号
        /// </summary>
        public string appSeq { get; set; }


        /// <summary>
        /// 借据号
        /// </summary>
        public string loanNo { get; set; }

        /// <summary>
        /// 放款金额
        /// </summary>
        public decimal dnAmt { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string custName { get; set; }

        /// <summary>
        /// 是否成功 0：否   1：是  2：转人工
        /// </summary>
        public string fkStatus { get; set; }

        /// <summary>
        /// 记账日期
        /// </summary>
        public string transferDate { get; set; }

        /// <summary>
        /// 错误信息表述
        /// </summary>
        public string errorMsg { get; set; }

    }
}