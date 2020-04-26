using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models
{
    /// <summary>
    /// 数据项参数
    /// </summary>
    [Serializable]
    public class DataItemParam
    {
        private string itemName = string.Empty;
        /// <summary>
        /// 获取或设置数据项名称
        /// </summary>
        public string ItemName
        {
            get { return itemName; }
            set { this.itemName = value; }
        }

        private object itemValue = string.Empty;
        /// <summary>
        /// 获取或设置数据项的值
        /// </summary>
        public object ItemValue
        {
            get { return itemValue; }
            set { this.itemValue = value; }
        }
    }
}