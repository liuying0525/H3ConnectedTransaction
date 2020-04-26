using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.ParticipatingLoan
{
    /// <summary>
    /// 上传图片通知
    /// </summary>
    public class GFUploadNotifyRequest
    {
        public List<string> ContractNumbers { get; set; }
    }

    public class AppNumberUnionIdRelation
    {
        public string UnionId { get; set; }

        public string ApplicationNumber { get; set; }

        public string ContractNumber { get; set; }

        public string InstanceId { get; set; }
    }

    public class GFAppNumberContractRelation
    {
        public string ContractNumber { get; set; }

        public string ApplicationNumber { get; set; }
    }
}