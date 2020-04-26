using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProposalService.Model
{
    class StartWorkflowParameter
    {
        public string id { get; set; }
        public string UserCode { get; set; }
        public string WorkflowCode { get; set; }
        public bool FinishStart { get; set; }
        public string InstanceId { get; set; }
        public object ParamValues { get; set; }
    }
}
