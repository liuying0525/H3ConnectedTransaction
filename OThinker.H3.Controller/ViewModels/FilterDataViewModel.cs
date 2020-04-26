using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 查询列表过滤条件显示模型
    /// </summary>
    public class FilterDataViewModel:ViewModelBase
    {
        /// <summary>
        /// 获取或设置属性编码
        /// </summary>
        public string PropertyCode { get; set; }

        /// <summary>
        /// 获取或设置属性名称
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// 获取或设置逻辑类型
        /// </summary>
        public string LogicType { get; set; }


        /// <summary>
        /// 获取或设置是否可见
        /// </summary>
        public int Visible { get; set; }


        /// <summary>
        /// 获取或设置逻辑类型
        /// </summary>
        public int PropertyType { get; set; }

        /// <summary>
        /// 获取或设置属性显示的控件类型
        /// </summary>
        public int DisplayType { get; set; }

        /// <summary>
        /// 获取或设置属性的查询类型
        /// </summary>
        public int FilterType { get; set; }

        /// <summary>
        /// 获取或设置控件的值列表
        /// </summary>
        public List<Item> SelectedValues { get; set; }

        /// <summary>
        /// 获取或设置控件的默认值
        /// </summary>
        public string DefaultValue { get; set; }
        
        /// <summary>
        /// 获取或设置所属的父对象
        /// </summary>
        public int ParentIndex { get; set; }

    }
}
