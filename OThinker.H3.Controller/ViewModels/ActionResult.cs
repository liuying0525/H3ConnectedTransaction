using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// 操作返回类
    /// </summary>
    public class ActionResult
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public ActionResult() {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="success"></param>
        public ActionResult(bool success) : this(success, string.Empty) { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="message">返回消息</param>
        public ActionResult(bool success, string message)
            : this(success, message, null)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="message">返回消息</param>
        /// <param name="extend">扩展信息</param>
        public ActionResult(bool success, string message, object extend)
            : this(success, message, extend, ExceptionCode.Unspecified)
        {
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="success">是否成功</param>
        /// <param name="message">返回消息</param>
        /// <param name="extend">扩展信息</param>
        /// <param name="ExceptionCode">异常代码信息</param>
        public ActionResult(bool success, string message, object extend, ExceptionCode ExceptionCode)
        {
            this.Success = success;
            this.Message = message;
            this.Extend = extend;
            this.ExceptionCode = ExceptionCode;
        }

        /// <summary>
        /// 获取或设置返回错误编码
        /// </summary>
        public ExceptionCode ExceptionCode { get; set; }

        /// <summary>
        /// 获取或设置后台业务操作是否成功
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 获取或设置后台业务操作返回消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 获取或设置消息扩展信息
        /// </summary>
        public object Extend { get; set; }
    }
}