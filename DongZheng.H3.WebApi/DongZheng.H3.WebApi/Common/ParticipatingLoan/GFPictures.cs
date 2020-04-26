using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.ParticipatingLoan
{
    /// <summary>
    /// 广发需要图片Model
    /// </summary>
    public class GFPictureModel
    {
        public string orderId { get; set; }

        /// <summary>
        /// 身份证正面照
        /// </summary>
        public string idCardFace { get; set; }

        /// <summary>
        /// 身份证背面照
        /// </summary>
        public string idCardBack { get; set; }

        /// <summary>
        /// 征信授权书
        /// </summary>
        public string proxyBook { get; set; }

        /// <summary>
        /// 活体照片
        /// </summary>
        public string facePicture { get; set; }
    }
}