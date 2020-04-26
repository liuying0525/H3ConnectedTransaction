using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkflowClauseViewModel
    {

        /// <summary>
        /// 是否有权限编辑: 0锁定，1未锁定
        /// </summary>
        public int IsControlUsable { get; set; }
        /// <summary>
        /// 流程模板编码
        /// </summary>
        public string WorkflowCode { get; set; }
        /// <summary>
        /// 流程模板状态
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否移动端发起
        /// </summary>
        public bool MobileStart { get; set; }
        /// <summary>
        /// 流程模板版本号
        /// </summary>
        public string WorkflowVersion { get; set; }
        /// <summary>
        /// 流水号重置策略
        /// </summary>
        public string SeqNoResetType { get; set; }
        /// <summary>
        /// 流水号编码
        /// </summary>
        public string SequenceCode { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 排序号
        /// </summary>
        public int SortKey { get; set; }
        /// <summary>
        /// 工作日历
        /// </summary>
        public string WorkCalendar { get; set; }
        /// <summary>
        /// 流程异常管理员
        /// </summary>
        public string ExceptionManager { get; set; }
        /// <summary>
        /// 流水号模式
        /// </summary>
        public string SequenceNoType { get; set; }
        /// <summary>
        /// 图片路径
        /// </summary>
        public string ImageUrl { get; set; }
    }
}
