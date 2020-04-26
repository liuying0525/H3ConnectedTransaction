using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.OCR
{
    public class DZBankCardResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public BankCardResult data { get; set; }
    }
    public class BankCardResult
    {
        public string type { get; set; }

        [JsonProperty("card_number")]
        public string cardNumber { get; set; }
        
        
        public string validate { get; set; }

        [JsonProperty("holder_name")]
        public string holderName { get; set; }
        
        public string issuer { get; set; }
        public string fileUrl { get; set; }

        /// <summary>
        /// 错误信息
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