using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 还款计划查询
    /// </summary>
    public class ZyRepaymentPlanResponse : ZyResponseBase
    {
        /// <summary>
        /// 总条数
        /// </summary>
        public int totalCount { get; set; }

        /// <summary>
        /// 循环体条数
        /// </summary>
        public int rowsCount { get; set; }

        /// <summary>
        /// 应还金额和
        /// </summary>
        public decimal sumRepayingPricipalInterest { get; set; }

        /// <summary>
        /// 应还利息和
        /// </summary>
        public decimal sumRepayingInterest { get; set; }

        /// <summary>
        /// 应还本金和
        /// </summary>
        public decimal sumRepayingPricipal { get; set; }

        /// <summary>
        /// 应还费用和
        /// </summary>
        public decimal sumCurrentFee { get; set; }

        /// <summary>
        /// 逾期利息和
        /// </summary>
        public decimal sumNotyetImposeInterest { get; set; }

        /// <summary>
        /// 已还金额和
        /// </summary>
        public decimal sumRepayedPricipalInterest { get; set; }

        /// <summary>
        /// 已还本金和
        /// </summary>
        public decimal sumRepayedPricipal { get; set; }

        /// <summary>
        /// 已还利息和
        /// </summary>
        public decimal sumRepayedInterest { get; set; }

        /// <summary>
        /// 已还逾期利息和
        /// </summary>
        public decimal sumRepayedNotyetImposeInterest { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public List<RowsDeatil> rows { get; set; }
    }

    public class RowsDeatil
    {
        /// <summary>
        /// 还款计划明细Id
        /// </summary>
        public string repayingPlanDetailId { get; set; }

        /// <summary>
        /// 还款计划Id
        /// </summary>
        public string repayingPlanId { get; set; }

        /// <summary>
        /// 期数
        /// </summary>
        public int currentPeriod { get; set; }

        /// <summary>
        /// 计划还款日
        /// </summary>
        public string currentEndDate { get; set; }

        /// <summary>
        /// 应还本金
        /// </summary>
        public decimal currentPrincipal { get; set; }

        /// <summary>
        /// 应还利息
        /// </summary>
        public decimal currentInterest { get; set; }

        /// <summary>
        /// 应还总额本息
        /// </summary>
        public decimal currentPrincipalInterest { get; set; }

        /// <summary>
        /// 截止当期累计应还本
        /// </summary>
        public decimal endCurrentPrincipal { get; set; }

        /// <summary>
        /// 截至当期本金余额
        /// </summary>
        public decimal endCurrentPrincipalBalance { get; set; }

        /// <summary>
        /// 截止当期累计应还息
        /// </summary>
        public decimal endCurrentInterest { get; set; }

        /// <summary>
        /// 逾期手续费
        /// </summary>
        public decimal overFee { get; set; }

        /// <summary>
        /// 逾期利息
        /// </summary>
        public decimal overInt { get; set; }

        /// <summary>
        /// 状态 0未还，1正常部分还款，2已还，3逾期，4停止计息，5已冲正，7期部分还款，8还款中，18冲正处理中
        //  当期本金、利息、罚息、复利、费用如果有未结清的，则该标志为'Y'
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// 实际还款日期
        /// </summary>
        public string repayingDate { get; set; }


        /// <summary>
        /// 已还本金
        /// </summary>
        public decimal repayedPrincipal { get; set; }

        /// <summary>
        /// 已还利息
        /// </summary>
        public decimal repayedInterest { get; set; }

        /// <summary>
        /// 已还逾期利息
        /// </summary>
        public decimal repayedImposeInterest { get; set; }

        /// <summary>
        /// 已还总金额
        /// </summary>
        public decimal repayedTotalamount { get; set; }

        /// <summary>
        /// 应还费用
        /// </summary>
        public decimal currentFee { get; set; }

        /// <summary>
        /// 已还费用
        /// </summary>
        public decimal repayedFee { get; set; }

        /// <summary>
        /// 减免费用
        /// </summary>
        public decimal derateFee { get; set; }

    }
}