using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class WorkflowPackageViewModel : ViewModelBase
    {
        ///<summary>
        ///获取或设置所属流程目录
        ///</summary>
        public string Folder { get; set; }

        ///<summary>
        ///获取或设置编码
        ///</summary>
        public string Code { get; set; }

        ///<summary>
        ///获取或设置显示名称
        ///</summary>
        public string DisplayName { get; set; }

        ///<summary>
        ///获取或设置存储类型
        ///</summary>
        public string StorageType { get; set; }

        ///<summary>
        ///获取或设置排序建
        ///</summary>
        public string SortKey { get; set; }

        /// <summary>
        /// 获取或设置锁定人
        /// </summary>
        public string CheckedUser { get; set; }

        /// <summary>
        /// 获取或设置节点类型
        /// </summary>
        public string ObjectType { get; set; }

        /// <summary>
        /// 获取或设置父节点ID
        /// </summary>
        public string ParentId { get; set; } 
        
        /// <summary>
        /// 获取或设置父节点编码
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 获取或设置是否共享
        /// </summary>
        public bool IsShared { get; set; }

        /// <summary>
        /// 获取或设置是否引用共享包
        /// </summary>
        public bool IsQuotePacket { get; set; }

        /// <summary>
        /// 获取或设置绑定的共享包
        /// </summary>
        public string BindPacket { get; set; }
    }
}
