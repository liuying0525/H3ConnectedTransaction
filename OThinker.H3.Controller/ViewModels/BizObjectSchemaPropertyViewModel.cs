using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 数据模型数据项
    /// </summary>
    public class BizObjectSchemaPropertyViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }
        ///<summary>
        ///获取或设置数据项名称
        ///</summary>
        public string PropertyName { get; set; }

        ///<summary>
        ///获取或设置树节点显示名称
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置数据项类型
        ///</summary>
        public string LogicType { get; set; }

        ///<summary>
        ///获取或设置是否是虚拟字段
        ///</summary>
        public bool VirtualField { get; set; }

        ///<summary>
        ///获取或设置记录痕迹
        ///</summary>
        public bool RecordTrail { get; set; }

        ///<summary>
        ///获取或设置索引
        ///</summary>
        public bool Indexed { get; set; }

        ///<summary>
        ///获取或设置是否用作搜索
        ///</summary>
        public bool Searchable { get; set; }

        ///<summary>
        ///获取或设置默认值
        ///</summary>
        public string DefaultValue { get; set; }

        ///<summary>
        ///获取或设置公式的值
        ///</summary>
        public string Formula { get; set; }

        ///<summary>
        ///获取或设置全局变量
        ///</summary>
        public string Global { get; set; }

        /// <summary>
        /// 获取或设置是否已经发布
        /// </summary>
        public bool IsPublished { get; set; }

        /// <summary>
        /// 获取或设置属性编码
        /// </summary>
        public string Property { get; set; }

        /// <summary>
        /// 获取或设置父编码
        /// </summary>
        public string ParentProperty { get; set; }
        
    }
}
