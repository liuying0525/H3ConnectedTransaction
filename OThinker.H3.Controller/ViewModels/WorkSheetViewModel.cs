using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkSheetViewModel : ViewModelBase
    {
        ///<summary>
        ///获取或设置父目录名称
        ///</summary>
        public string Root { get; set; }

        ///<summary>
        ///获取或设置父ID
        ///</summary>
        public string ParentId { get; set; }

        ///<summary>
        ///获取或设置父编码
        ///</summary>
        public string ParentCode { get; set; }
        ///<summary>
        ///获取或设置编码
        ///</summary>
        public string Code { get; set; }

        ///<summary>
        ///获取或设置名称
        ///</summary>
        public string Name { get; set; }

        ///<summary>
        ///获取或设置类型
        ///</summary>
        public string Type { get; set; }

        ///<summary>
        ///获取或设置PC访问地址
        ///</summary>
        public string PCAdd { get; set; }

        ///<summary>
        ///获取或设置手机访问地址是否启用
        ///</summary>
        public bool isMobileAdd { get; set; }

        ///<summary>
        ///获取或设置手机访问地址
        ///</summary>
        public string MobileAdd { get; set; }

        ///<summary>
        ///获取或设置打印地址是否启用
        ///</summary>
        public bool isPrintAdd { get; set; }

        ///<summary>
        ///获取或设置打印地址
        ///</summary>
        public string PrintAdd { get; set; }

        ///<summary>
        ///获取或设置排序码
        ///</summary>
        public string SortKey { get; set; }

    }
}
