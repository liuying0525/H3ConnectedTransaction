using Newtonsoft.Json;

namespace DongZheng.H3.WebApi.Models.OCR
{
    /// <summary>
    /// 扣款授权书识别结果
    /// </summary>
    public class AuthorizationResult
    {
        /// <summary>
        /// 申请号
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 客户名
        /// </summary>
        public string Customer { get; set; }

        /// <summary>
        /// 账户
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string Identity { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        [JsonProperty("identity_type")]
        public string IdentityType { get; set; }

        /// <summary>
        /// 错误码
        /// </summary>
        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
    }
}