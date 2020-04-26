using System;
using System.Runtime;

namespace System.ComponentModel
{
    /// <summary>
    /// 记录属性的设置示例
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class ExampleAttribute : Attribute
    {
        public ExampleAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }
}