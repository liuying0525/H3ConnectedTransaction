using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 数据项值的集合
    /// </summary>
    public class MvcDataItemTable : Dictionary<string, MvcDataItem>
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public MvcDataItemTable()
        {
        }

        /// <summary>
        /// 设置数据项的值
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        public void SetValue(string Key, object Value)
        {
            if (this.ContainsKey(Key))
            {
                this[Key].V = Value;
            }
            else
            {
                this.Add(Key, new MvcDataItem() { V = Value });
            }
        }
    }
}
