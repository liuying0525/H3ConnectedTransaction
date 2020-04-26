
using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ProductCenter
{
    /// <summary>
    /// 产品查询
    /// </summary>
    public class ProductQueryRequest: PagerInfo
    {
        public string ProductName { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }

        public bool All { get; set; }
    }


    public class ProductQueryViewModel : ViewModelBase
    {
        public int RowIndex { get; set; }

        public string ProductId { get; set; }

        public string ProductName { get; set; }

        /// <summary>
        /// 产品组
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string GroupId { get; set; }

        /// <summary>
        /// 产品有效期始
        /// </summary>
        public string StartTime { get; set; }

        /// <summary>
        /// 产品有效期至
        /// </summary>
        public string EndTime { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        public string ModifyTime { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string FLEET_COMP_INDIV_IND { get; set; }

        /// <summary>
        /// 最小融资额
        /// </summary>
        public string MinFinancingAmount { get; set; }

        /// <summary>
        /// 最大融资额
        /// </summary>
        public string MaxFinancingAmount { get; set; }

        /// <summary>
        /// 最小租期
        /// </summary>
        public string MinLeaseTrm { get; set; }

        /// <summary>
        /// 最大租期
        /// </summary>
        public string MaxLeaseTrm { get; set; }

        /// <summary>
        /// 最大融资比例
        /// </summary>
        public string MaxFinancingPCT { get; set; }

        /// <summary>
        /// 尾款 R 表示支持尾款
        /// </summary>
        public string FutureValueType { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string MinimumRvPct { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string MaximumRvPct { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string MinimumTermMm { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string MaximumTermMm { get; set; }

        /// <summary>
        /// 残值百分比
        /// </summary>
        public string RvPCT { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string RvGuaranteeAmt { get; set; }

        /// <summary>
        /// 客户利率
        /// </summary>
        public string CustomerRate { get; set; }

        /// <summary>
        /// 实际利率
        /// </summary>
        public string ActualRate { get; set; }

        /// <summary>
        /// 补贴率
        /// </summary>
        public string SubsidyRate { get; set; }

        /// <summary>
        /// TODO：
        /// </summary>
        public string RvEditableInd { get; set; }

        /// <summary>
        /// 提前终止罚金比例
        /// </summary>
        public string EndNonactrebatePct { get; set; }

        /// <summary>
        /// 提前结清  A  C  配置表T C 固定金额F A
        /// </summary>
        public string EndPenaltyInd { get; set; }

    }

}