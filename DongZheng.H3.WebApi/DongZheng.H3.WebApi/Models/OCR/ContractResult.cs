using Newtonsoft.Json;

namespace DongZheng.H3.WebApi.Models.OCR
{
    /// <summary>
    /// 抵押贷款合同识别结果
    /// </summary>
    public class ContractResult
    {
        public RecognizeData Data { get; set; }

        [JsonProperty("error_code")]
        public int ErrorCode { get; set; }

        [JsonProperty("error_msg")]
        public string ErrorMsg { get; set; }
    }
}