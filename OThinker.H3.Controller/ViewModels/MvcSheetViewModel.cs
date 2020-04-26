using OThinker.H3.Sheet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class MvcSheetViewModel:ViewModelBase
    {
        public string SheetCode { get; set; }

        public string SchemaCode { get; set; }
        
        /// <summary>
        /// 表单名称
        /// </summary>
        public string SheetName { get; set; }
        /// <summary>
        /// 设计器的显示名称
        /// </summary>
        public string DesignerName { get; set; }

        /// <summary>
        /// 数据项树数据
        /// </summary>
        public List<MvcTreeData> DataItemTreeData { get; set; }

        /// <summary>
        /// HTML是否从数据库加载
        /// </summary>
        public bool IsHtmlFromDB { get; set; }
        /// <summary>
        /// 前台设计区域内容
        /// </summary>
        public string DesignModeContent{get;set;}
        public string CSharpCode{get;set;}
        public string RuntimeContent{get;set;}
        
        //启用自定义代码
        public bool EnabledCode { get; set; }

        public string Editor{get;set;}

        public bool ASPXChanged{get;set;}

        public string PrintModel{get;set;}

        public string Javascript{get;set;}

        public string SheetHtml { get; set; }

        public string SheetJson { get; set; }

        /// <summary>
        /// 当前表单对象
        /// </summary>
        public BizSheet Sheet { get; set; }

        /// <summary>
        /// Organization.User的公共属性拼接成以","号分隔的字符串
        /// </summary>
        public string PropertyNames { get; set; }

        /// <summary>
        /// 数据模型的自定义数据项,返回前台放在js变量中，供配置向导使用
        /// </summary>
        public object BizObjectFields { get; set; }

        /// <summary>
        /// 给用于ComputationRule配置的数据项下拉框绑定数据
        /// </summary>
        public object ComputationRuleDataItem{get;set;}

        /// <summary>
        /// 流程是否锁定，1-未锁定，0-锁定，页面不可编辑
        /// </summary>
        public int IsControlUsable { get; set; }
    }

    public  class MvcTreeData
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string ParentCode { get; set; }

        public bool _isexpand = false;
        public bool isexpand
        {
            get { return this._isexpand; }
            set { this._isexpand = value; }
        }
    }

    /// <summary>
    /// 表单设计器使用
    /// </summary>
   public struct MyBizObjectField
    {
        public string Code { get; set; }
        public string Name { get; set; }
        /// <summary>
        /// 是否为子表数据项
        /// </summary>
        public string IsChildField { get; set; }
    }

}
