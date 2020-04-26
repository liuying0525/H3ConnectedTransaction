using OThinker.H3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{

    /// <summary>
    /// 测试服务运行ViewModel
    /// </summary>
    public class TestServiceViewModel : ViewModelBase
    {
        /// <summary>
        /// 服务编码
        /// </summary>
        public string ServiceCode { get; set; }

        /// <summary>
        /// 运行方法名称
        /// </summary>
        public string RunMethod { get; set; }

        public string Text { get; set; }

        public string Value { get; set; }

        /// <summary>
        /// 方法参数
        /// </summary>
        public List<DataItem> InParams { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class DataItem
    {
        public string ItemName { get; set; }
        public DataLogicType LogicType { get; set; }

        public string LogicTypeString { get { return LogicType.ToString(); } }
        public object ItemValue { get; set; }
    }
}
