using Newtonsoft.Json;
using System.Collections.Generic;

namespace DongZheng.H3.WebApi.Models.OCR
{
    public class RecognizeData
    {
        [JsonProperty("single_data_list")]
        public IList<RecognizeDataItem> SingleDataList { get; set; }

        [JsonProperty("object_data_list")]
        public IList<RecognizeDataItem> ObjectDataList { get; set; }
    }
}