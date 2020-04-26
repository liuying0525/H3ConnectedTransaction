using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkflowTemplateViewModel:ViewModelBase
    {
        public string Command { get; set; }//没有用，做占位接收数据
        public string WorkflowTemplate { get; set; }

        public string StartActivities { get; set; }

        public string EndActivities { get; set; }

        public string FillSheetActivities { get; set; }

        public string ApproveActivities { get; set; }

        public string CirculateActivities { get; set; }

        public string NotifyActivities { get; set; }

        public string WaitActivities { get; set; }

        public string ConnectionActivities { get; set; }

        public string BizActionActivities { get; set; }

        public string SubInstanceActivities { get; set; }

    }
}
