using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.FaceSign
{
    /// <summary>
    /// 回调结果接收
    /// </summary>
    public class FaceSignCallBackResponse
    {
        /// <summary>
        /// 面签单号
        /// </summary>
        public string appNo { get; set; }

        /// <summary>
        /// 东正申请号
        /// </summary>
        public string applicationNumber { get; set; }

        public FaceSignBizInfo bizInfoData { get; set; }

        public List<FaceSignBizRelDetails> bizRelDetails { get; set; }

    }

    /// <summary>
    /// 业务表数据
    /// </summary>
    public class FaceSignBizInfo
    {
        /// <summary>
        /// 合同编号
        /// </summary>
        public string contNo { get; set; }

        /// <summary>
        /// 综合面签状态结果
        /// </summary>
        public string bizSts { get; set; }

        /// <summary>
        /// 退回重新面签次数
        /// </summary>
        public int retryNo { get; set; }

        /// <summary>
        /// 经销商账户
        /// </summary>
        public string accountNo { get; set; }
    }

    /// <summary>
    /// 业务人信息数据
    /// </summary>
    public class FaceSignBizRelDetails
    {
        /// <summary>
        /// 主体人类型(主借人;...;...)
        /// </summary>
        public string relTyp { get; set;}

        public string name { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string idNo { get; set; }

        public string mobile { get; set; }

        /// <summary>
        /// 家庭地址
        /// </summary>
        public string liveAddr { get; set; }

        /// <summary>
        /// 面签状态结果
        /// </summary>
        public string bizSts { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string reasons { get; set; }
    }
}