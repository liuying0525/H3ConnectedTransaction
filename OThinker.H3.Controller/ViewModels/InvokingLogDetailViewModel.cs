using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 集成日志明细
    /// </summary>
    public class InvokingLogDetailViewModel:ViewModelBase
    {

        ///<summary>
        ///获取或设置
        ///</summary>
        public string User { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string CreatedTime { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string EndTime { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string UsedTime { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string SlaveCode { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string TranId { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string ServiceCode { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string MethodName { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string ParamXml { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string ReturnXml { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string ExceptionMessage { get; set; }

        ///<summary>
        ///获取或设置
        ///</summary>
        public string ExceptionStack { get; set; } 

    }
}
