using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 测试用例
    /// </summary>
    public class EditSimulationViewModel:ViewModelBase
    {
        public string WorkflowCode { get; set; }
        public string WorkflowName { get; set; }
        public string UseCaseName { get; set; }
        public string Creator { get; set; }
        public object DataItems { get; set; }
        public object Activitys { get; set; }
        public object Originator { get; set; }
        public string DataItemsString { get; set; }
        public string IgnoresString { get; set; }
        
    }

    /// <summary>
    /// 测试用例集
    /// </summary>
    public class SimulationListViewModel : ViewModelBase
    {
        public string SimulationCode { get; set; }
        public string Simulations { get; set; }
        public string LastRunTime { get; set; }

    }
}
