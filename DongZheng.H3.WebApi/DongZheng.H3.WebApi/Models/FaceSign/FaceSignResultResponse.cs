using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.FaceSign
{
    public class FaceSignResultResponse
    {
        public string Objectid { get; set; }

        public string BizObjectId { get; set; }

        public string ApplicationNumber { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public string ApplicantType { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 面签结果
        /// </summary>
        public string FaceSignResult { get; set; }

        /// <summary>
        /// 原因
        /// </summary>
        public string Resons { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreatedTime { get; set; }

        /// <summary>
        /// 面签单号
        /// </summary>
        public string FaceSignNo { get; set; }

        /// <summary>
        /// 第几次面签
        /// </summary>
        public int RetryNoIndex { get; set; }

        public string Mobile { get; set; }

        public string LiveAddress { get; set; }
    }
}