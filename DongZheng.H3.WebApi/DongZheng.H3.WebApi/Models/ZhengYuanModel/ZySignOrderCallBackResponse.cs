using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 签章回调Model
    /// </summary>
    public class ZySignOrderCallBackResponse : ZyResponseBase
    {
        /// <summary>
        /// 合作方风控单号
        /// </summary>
        public string appSeq { get; set; }

        /// <summary>
        /// 借据号
        /// </summary>
        public string loanId { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string contractNo { get; set; }

        /// <summary>
        /// 签章状态 0:失败，1成功
        /// </summary>
        public string status { get; set; }

        /// <summary>
        /// sftp路径
        /// </summary>
        public string sftpPath { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string remark { get; set; }
    }
}