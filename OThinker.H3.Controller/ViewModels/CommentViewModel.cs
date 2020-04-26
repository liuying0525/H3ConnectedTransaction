using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class CommentViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置常用意见ID
        /// </summary>
        public string CommentID { get; set; }
        /// <summary>
        /// 获取或设置常用意见序号
        /// </summary>
        public string CommentIndex { get; set; }
        /// <summary>
        /// 获取或设置常用意见文本
        /// </summary>
        public string CommentText { get; set; }
    }
}
