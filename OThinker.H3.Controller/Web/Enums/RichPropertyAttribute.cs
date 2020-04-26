using System;
using System.Runtime;

namespace System.ComponentModel
{
    /// <summary>
    /// 记录当前属性是否需要富文本框编辑
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class RichPropertyAttribute : Attribute
    {
        public RichPropertyAttribute(bool richProperty)
        {
            this._RichProperty = richProperty;
        }

        private bool _RichProperty = false;
        public bool RichProperty
        {
            get { return this._RichProperty; }
            set { this._RichProperty = value; }
        }
    }
}