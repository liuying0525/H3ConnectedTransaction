using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizObjectSchemaMethod : ViewModelBase
    {
        ///<summary>
        ///获取或设置所属主数据ID
        ///</summary>
        public string ParentId { get; set; }

        ///<summary>
        ///获取或设置数据模型编码
        ///</summary>
        public string SchemaCode { get; set; }

        ///<summary>
        ///获取或设置方法编码
        ///</summary>
        public string MethodName { get; set; }

        ///<summary>
        ///获取或设置显示名称
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置方法类型
        ///</summary>
        public string MethodType { get; set; }

        ///<summary>
        ///获取或设置方法类型
        ///</summary>
        public string MethodTypeTip { get; set; }

        ///<summary>
        ///获取或设置是否启用事物
        ///</summary>
        public bool Transaction { get; set; }

        ///<summary>
        ///获取或设置完成后调用更新方法
        ///</summary>
        public bool UpdateAfterInvoking { get; set; }

        /// <summary>
        /// 获取或设置是否可修改名称
        /// </summary>
        public bool IsDefaultMethod { get; set; }

        /// <summary>
        /// 获取或设置业务方法原名称
        /// </summary>
        public string MethodCode { get; set; }


    }
}
