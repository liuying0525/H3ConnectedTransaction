using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.OCR
{
    public class DZVinResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public VinResult data { get; set; }
    }
    public class VinResult
    {
        [JsonProperty("vehicle_license_main_vin")]
        public string vehicleLicenseMainVin { get; set; }
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