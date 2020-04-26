using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 集成日志
    /// </summary>
    public class InvokingLogViewModel:ViewModelBase
    {
        public string UserID { get; set; }

        public string BizServiceCode { get; set; }

        public string MethodName { get; set; }

        public string StartTime { get; set; }

        public string EndTime { get; set; }

        public string UsedTime { get; set; }
    }
}
