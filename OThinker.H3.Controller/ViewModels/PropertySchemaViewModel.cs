using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 业务对象模式的属性模式
    /// </summary>
    public class PropertySchemaViewModel :ViewModelBase
    {
        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ParentProperty { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ParentItemName { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ItemName { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string LogicType { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string LogicTypeName { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string ChildSchemaCode { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public string DefaultValue { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool Trackable { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool Indexed { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool Searchable { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool Abstract { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool IsReserved { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// 获取或设置
        /// </summary>
        public List<PropertySchemaViewModel> children { get; set; }


    }
}
