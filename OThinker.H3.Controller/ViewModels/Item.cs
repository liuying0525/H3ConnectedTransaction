using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// Option选项
    /// </summary>
    public class Item
    {
        public Item() { }
        public Item(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
        /// <summary>
        /// 获取或设置Text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 获取或设置Value
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 附属信息
        /// </summary>
        public object Extend { get; set; }


    }
}
