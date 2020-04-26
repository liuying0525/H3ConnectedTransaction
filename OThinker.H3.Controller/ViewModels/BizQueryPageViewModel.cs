using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class BizQueryPageViewModel
    {
        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set;}

        /// <summary>
        /// 获取或设置列表文件类名
        /// </summary>
        public string ListClassName { get; set; } 

        /// <summary>
        /// 获取或设置生成页面
        /// </summary>
        public string Query { get; set; } 
       
        /// <summary>
        /// 获取或设置Aspx代码
        /// </summary>
        public string ListASPXPage { get; set; } 
       
        /// <summary>
        /// 获取或设置CS代码
        /// </summary>
        public string ListCSPage { get; set; }
    }
}
