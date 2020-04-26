using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 数据项对象集合
    /// </summary>
    public class MvcBizObject
    {
        public MvcBizObject()
        {

        }

        private MvcDataItemTable _DataItems = new MvcDataItemTable();
        /// <summary>
        /// 数据项的集合
        /// </summary>
        public MvcDataItemTable DataItems
        {
            get
            {
                return _DataItems;
            }
            set
            {
                this._DataItems = value;
            }
        }
    }
}
