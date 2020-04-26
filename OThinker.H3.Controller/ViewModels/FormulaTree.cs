using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class FormulaTreeNode:ViewModelBase
    {
        /// <summary>
        /// 显示文本
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 图片路径
        /// </summary>
        public string Icon { get; set; }
        public bool IsLeaf { get; set; }

        /// <summary>
        /// 加载URL
        /// </summary>
        public string LoadDataUrl { get; set; }

        /// <summary>
        /// 公式类型，前台根据这个判断使用哪个公式
        /// </summary>
        public string FormulaType { get; set; }

        /// <summary>
        /// 存储公式需要使用的值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ParentID { get; set; }
    }

    public class FormulaTipViewModel : ViewModelBase
    {
        /// <summary>
        /// 常量 
        /// </summary>
        public List<object> Consts { get; set; }

        /// <summary>
        /// 一般函数
        /// </summary>
        public object MathFunctions { get; set; }

        /// <summary>
        /// 参与者函数
        /// </summary>
        public object ParticipantFunctions { get; set; }

        /// <summary>
        /// 参数类型
        /// </summary>
        public object LogicTypes { get; set; }

        /// <summary>
        /// 系统数据项
        /// </summary>
        public object InstanceVariables { get; set; }

        /// <summary>
        /// 流程数据项
        /// </summary>
        public object DataItems { get; set; }

        /// <summary>
        /// 规则词汇表
        /// </summary>
        public object RuleElement { get; set; }
    }
}
