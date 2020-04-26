using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class SignatureViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置签章ID
        /// </summary>
        public string SignatureID { get; set; }
        /// <summary>
        /// 获取或设置签章用户ID
        /// </summary>
        public string UnitID { get; set; }

        /// <summary>
        /// 获取或设置签章显示名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 获取或设置是否是默认签章
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        /// 获取或设置签章描述信息
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置签章文件类型
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// 获取或设置签章图片路径
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        /// 获取或设置签章文件排序值
        /// </summary>
        public string SortKey { get; set; }

        /// <summary>
        /// 获取或设置签章文件禁用/启用状态
        /// </summary>
        public string State { get; set; }
    }

    public class SignatureAdminViewModel : ViewModelBase
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 签章对应的用户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 图片
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序值
        /// </summary>
        public int SortKey { get; set; }


        /// <summary>
        /// 默认
        /// </summary>
        public bool IsDefault { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
    }
}
