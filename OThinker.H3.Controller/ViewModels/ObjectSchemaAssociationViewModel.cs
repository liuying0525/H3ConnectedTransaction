using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class ObjectSchemaAssociationViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置映射列表
        /// </summary>
        public string PropertyMap { get; set; }
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }

        ///<summary>
        ///获取或设置关联关系就名称
        ///</summary>
        public string RelationName { get; set; }

        ///<summary>
        ///获取或设置关联关系显示名称
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置关联关系类型
        ///</summary>
        public string Type { get; set; }

        ///<summary>
        ///获取或设置过滤方法
        ///</summary>
        public string FilterMethod { get; set; }

        ///<summary>
        ///获取或设置数据模型
        ///</summary>
        public string WorkflowID { get; set; }

        ///<summary>
        ///获取或设置被关联对象名称
        ///</summary>
        public string ItemName { get; set; }

        ///<summary>
        ///获取或设置关联方式
        ///</summary>
        public string MapType { get; set; }

        ///<summary>
        ///获取或设置关联对象名称
        ///</summary>
        public string MapTo { get; set; }
    }
}
