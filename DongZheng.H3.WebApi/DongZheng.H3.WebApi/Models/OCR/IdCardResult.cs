using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.OCR
{
    public class DZIdCardResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public IdCardResult data { get; set; }
    }
    public class IdCardResult
    {
        public string name { get; set; }

        [JsonProperty("id_number")]
        public string idNumber { get; set; }

        public string birthday { get; set; }
        
        public string sex { get; set; }
        
        public string people { get; set; }
        
        public string address { get; set; }

        [JsonProperty("issue_authority")]
        public string issueAuthority { get; set; }
        
        public string validity { get; set; }

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