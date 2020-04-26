using OThinker.H3.Analytics.Reporting;
using OThinker.H3.BizBus.BizRule;
using OThinker.H3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{


    public class BizRuleViewModel : ViewModelBase
    {
        public string Code { get; set; }

        public string ParentCode { get; set; }

        public string DisplayName { get; set; }
        public string Description { get; set; }
    }

    public class BizRuleHanderViewModel : ViewModelBase
    {
        public string Code { get; set; }

        public string ParentCode { get; set; }

        public string DisplayName { get; set; }

        /// <summary>
        /// 是否覆盖
        /// </summary>
        public bool IsCover { get; set; }

        /// <summary>
        /// 上传的Xml文件内容，导入时使用
        /// </summary>
        public string XMLString { get; set; }

        /// <summary>
        /// 上传的Xml文件名称
        /// </summary>
        public string FileName { get; set; }
    }



    /// <summary>
    /// 业务规则词汇表ViewModel
    /// </summary>
    public class BizRuleGlossaryViewModel:ViewModelBase
    {
        /// <summary>
        /// 数据项名称
        /// </summary>
         public string ElementName{get;set;}
        /// <summary>
        /// 业务规则编码
        /// </summary>
         public string RuleCode { get; set; }
         /// <summary>
         /// 显示名称
         /// </summary>
         public string DisplayName{get;set;}

         /// <summary>
         /// 描述
         /// </summary>
         public string Description { get; set; }
         /// <summary>
         /// 类型(string)
         /// </summary>
         public string LogicType { get; set; }
      
         /// <summary>
         /// 参数类型（IN ,OUT）
         /// </summary>
         public string ParamType { get; set; }

  
         /// <summary>
         /// 默认值
         /// </summary>
         public string DefaultValue{get;set;}

    }

    /// <summary>
    /// 业务规则决策表ViewModel
    /// </summary>
    public class BizRuleDecisionMatrixViewModel : ViewModelBase
    {
        /// <summary>
        /// 决策表Code
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 业务规则编码
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// 上级目录ID
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 决策表名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 决策表类型
        /// </summary>
        public string DecisionMatrixType { get; set; }

        /// <summary>
        /// 作用域
        /// </summary>
        public string Scope { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 返回字段
        /// </summary>
        public string ResultField { get; set; }

        /// <summary>
        /// 行执行方式
        /// </summary>
        public string RowExecutionType { get; set; }

        /// <summary>
        /// 列执行方式
        /// </summary>
        public string ColumnExecutionType { get; set; }
    }

    /// <summary>
    /// 决策表行、列
    /// </summary>
    public class BizRuleDecisionMatrixItemViewModel : ViewModelBase
    {
        /// <summary>
        /// 业务规则编码
        /// </summary>
        public string RuleCode { get; set; }

        /// <summary>
        /// 决策表编码
        /// </summary>
        public string MatrixCode { get; set; }


        public int Index { get; set; }

        public string DisplayName { get; set; }

        public bool IsDefault { get; set; }

        /// <summary>
        /// 生效条件
        /// </summary>
        public string EffectiveCondition { get; set; }

        public string Description { get; set; }

        public int SortKey { get; set; }

        public string ParentStrIndexs { get; set; }

        public List<BizRuleDecisionMatrixItemViewModel> children { get; set; }
    }
}
