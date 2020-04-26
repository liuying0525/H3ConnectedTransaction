using DongZheng.H3.WebApi.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ApiResponse
{
    /// <summary>
    /// 查询风控NCIIC返回Response
    /// </summary>
    public class QueryNciicResultResponse
    {
        public string reqID { get; set; }

        public string requestId { get; set; }
         
        public List<NciicResultDto> nciicResultList { get; set; }
    }
}