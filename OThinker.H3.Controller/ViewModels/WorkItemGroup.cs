using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkItemGroup : ViewModelBase
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="workflowCode"></param>
        /// <param name="workflowName"></param>
        public WorkItemGroup(string workflowCode, string workflowName)
        {
            this.WorkflowCode = workflowCode;
            this.WorkflowName = workflowName;
        }

        /// <summary>
        /// 获取或设置流程模板编码
        /// </summary>
        public string WorkflowCode { get; set; }

        /// <summary>
        /// 获取或设置流程模板名称
        /// </summary>
        public string WorkflowName { get; set; }

        private bool _Open = false;
        /// <summary>
        /// 获取或设置流程View视图是否展开
        /// </summary>
        public bool Open
        {
            get
            {
                return this._Open;
            }
            set
            {
                this._Open = value;
            }
        }

        /// <summary>
        /// 获取或设置流程实例数量
        /// </summary>
        public int TotalCount
        {
            get
            {
                return this.WorkItems.Count;
            }
        }

        private List<WorkItemViewModel> _WorkItems = new List<WorkItemViewModel>();

        /// <summary>
        /// 获取或设置任务集合
        /// </summary>
        public List<WorkItemViewModel> WorkItems
        {
            get { return this._WorkItems; }
            set { this._WorkItems = value; }
        }


    }
}
