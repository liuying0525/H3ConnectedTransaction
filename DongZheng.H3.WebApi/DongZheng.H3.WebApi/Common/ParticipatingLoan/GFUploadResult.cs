using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.ParticipatingLoan
{
    public class GFUploadResultCache
    {
        /// <summary>
        /// Ing Success Fail Clear
        /// </summary>
        public string Result { get; set; }

        public List<string> FailContractNumbers { get; set; }
    }
}