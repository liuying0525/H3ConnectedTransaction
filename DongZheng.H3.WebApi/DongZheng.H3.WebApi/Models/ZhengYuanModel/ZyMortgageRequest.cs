using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    public class ZyMortgageRequest : ZyVerifyCodeRequest
    {
        /// <summary>
        /// 交易服务码
        /// </summary>
        public string transCode { get { return "M990008"; } }

        /// <summary>
        /// 申请序列号
        /// </summary>
        public string appSeq { get; set; }

        
        /// <summary>
        /// 抵押合同号
        /// </summary>
        public string pledgeContractNo { get; set; }

        /// <summary>
        /// 放款通知回调地址
        /// </summary>
        public string returnUrl { get; set; }

        /// <summary>
        /// 抵押日期
        /// </summary>
        public string pledgeTime { get; set; }

        /// <summary>
        /// GPS设备是否安装  0:否，1:是
        /// </summary>
        public string gpsInstallStatus { get; set; }

        /// <summary>
        /// 合同信息
        /// </summary>
        public List<ContractInfo> contractInfo { get; set; }

    }

    public class ContractInfo
    {
        /// <summary>
        /// 合同编码
        /// </summary>
        public string contractNo { get; set; }

        /// <summary>
        /// 资料在sftp路径
        /// </summary>
        public string sftpPath { get; set; }
    }
}