using System;
using System.Runtime;

namespace System.ComponentModel
{
    /// <summary>
    /// 
    /// </summary>
    [AttributeUsage(AttributeTargets.All)]
    public class PlaceHolderAttribute : Attribute
    {
        public PlaceHolderAttribute(string description)
        {
            this.Description = description;
        }

        public string Description { get; set; }
    }
}