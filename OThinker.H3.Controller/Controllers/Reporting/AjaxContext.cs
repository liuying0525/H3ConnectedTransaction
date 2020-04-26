using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.Controllers.Reporting
{
    public class AjaxContext
    {
        /// <summary>
        ///是否是返回成功
        /// </summary>
        public bool Successful
        {
            get
            {
                return string.IsNullOrWhiteSpace(ErrorMessage);
            }
        }
        public bool IsLoggedIn = true;
        /// <summary>
        /// 异常信息
        /// </summary>
        public string ErrorMessage
        {
            get;
            set;
        }
        /// <summary>
        /// 是否是异常信息
        /// </summary>
        private bool _SolutionError = false;
        public bool SolutionError
        {
            get
            {
                return _SolutionError;
            }
            set
            {
                _SolutionError = value;
            }
        }
        /// <summary>
        /// 正确信息
        /// </summary>
        public string SuccessMessage
        {
            get;
            set;
        }

        public Dictionary<string, object> Result = new Dictionary<string, object>();
        /// <summary>
        /// debugger日志信息
        /// </summary>
        public string[] debugLogs
        {
            get;
            set;
        }
    }
}
