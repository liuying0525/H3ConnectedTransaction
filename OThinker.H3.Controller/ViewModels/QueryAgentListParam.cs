using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
   public class QueryAgentListParam
    {

        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string[] WorkFlowCode { get; set; }

        /// <summary>
        /// 委托人
        /// </summary>
        public string[] Agent { get; set; }

        /// <summary>
        /// 代理人
        /// </summary>
        public string[] Mandatary { get; set; }
    }
}
