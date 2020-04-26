using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.OCR
{
    public class DZDriverLicenseResult
    {
        public string code { get; set; }
        public string message { get; set; }
        public DriverLicenseResult data { get; set; }
    }
    public class DriverLicenseResult
    {
        public string address { get; set; }

        public string birthday { get; set; }

        [JsonProperty("drivetype")]
        public string driveType { get; set; }

        [JsonProperty("driving_license_main_nationality")]
        public string drivingLicenseMainNationality { get; set; }

        [JsonProperty("driving_license_main_number")]
        public string drivingLicenseMainNumber { get; set; }

        [JsonProperty("driving_license_main_valid_for")]
        public string drivingLicenseMainValidFor { get; set; }

        [JsonProperty("driving_license_main_valid_from")]
        public string drivingLicenseMainValidFrom { get; set; }

        [JsonProperty("issue_date")]
        public string issueDate { get; set; }
        
        public string name { get; set; }
        
        public string sex { get; set; }

        [JsonProperty("driving_license_document_number")]
        public string drivingLicenseDocumentNumber { get; set; }
        
        public string type { get; set; }
        
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