using DongZheng.H3.WebApi.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ApiResponse
{
    public class QueryPbocResultResponse
    {
        public string reqId { get; set; }

        public List<PbocResultDto> pbocReportInfoList { get; set; }
        
    }
}