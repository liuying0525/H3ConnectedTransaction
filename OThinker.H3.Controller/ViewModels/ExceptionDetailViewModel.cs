using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// 异常日志详情
    /// </summary>
    public class ExceptionDetailViewModel :ViewModelBase
    {

        ///<summary>
        ///获取或设置异常实例的连接
        ///</summary>
        public string InstanceUrl { get; set; }

        ///<summary>
        ///获取或设置实例名称
        ///</summary>
        public string InstanceName { get; set; }

        ///<summary>
        ///获取或设置发生时间
        ///</summary>
        public string OccurrenceTime { get; set; }

        ///<summary>
        ///获取或设置修复状态
        ///</summary>
        public string Fix { get; set; }

        ///<summary>
        ///获取或设置修复时间
        ///</summary>
        public string FixTime { get; set; }

        ///<summary>
        ///获取或设置是否阻塞
        ///</summary>
        public string Block { get; set; }

        ///<summary>
        ///获取或设置模板类别
        ///</summary>
        public string WorkflowPackage { get; set; }

        ///<summary>
        ///获取或设置模板版本
        ///</summary>
        public string WorkflowVersion { get; set; }

        ///<summary>
        ///获取或设置异常源的类型
        ///</summary>
        public string ExceptionSource { get; set; }

        ///<summary>
        ///获取或设置异常源的名称
        ///</summary>
        public string SourceName { get; set; }

        ///<summary>
        ///获取或设置异常源的动作
        ///</summary>
        public string Action { get; set; }

        ///<summary>
        ///获取或设置异常详细信息
        ///</summary>
        public string Message { get; set; } 

    }
}
