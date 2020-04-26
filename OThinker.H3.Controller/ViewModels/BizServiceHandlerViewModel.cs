using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizServiceHandlerViewModel:ViewModelBase
    {
        public BizServiceHandlerViewModel() { }

        /// <summary>
        /// 上级目录ID
        /// </summary>
        public string ParentID { get; set; }

        /// <summary>
        /// 上级目录文件夹
        /// </summary>
        public string ParentCode { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 存储文件的XML内容
        /// </summary>
        public string XMLString { get; set; }


        /// <summary>
        /// 是否覆盖
        /// </summary>
        public string Cover { get; set; }

        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServiceName { get; set; }
    }
}
