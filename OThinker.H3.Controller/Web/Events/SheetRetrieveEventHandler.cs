using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 表单取回事件。
    /// </summary>
    public class SheetRetrieveEventArgs
    {
        /// <summary>
        /// 取回事件的构造函数
        /// </summary>
        /// <param name="WorkItemID"></param>
        public SheetRetrieveEventArgs(string WorkItemID)
        {
            this._WorkItemID = WorkItemID;
        }

        private string _WorkItemID;
        /// <summary>
        /// 工作项ID
        /// </summary>
        public string WorkItemID
        {
            get
            {
                return this._WorkItemID;
            }
        }

        private bool _Continue = true;
        /// <summary>
        /// 是否自动跳转到被取回的工作项的页面
        /// </summary>
        public bool Continue
        {
            get
            {
                return this._Continue;
            }
            set
            {
                this._Continue = value;
            }
        }
    }

    /// <summary>
    /// 取回事件
    /// </summary>
    /// <param name="Sender"></param>
    /// <param name="e"></param>
    public delegate void SheetRetrieveEventHandler(object Sender, SheetRetrieveEventArgs e);
}
