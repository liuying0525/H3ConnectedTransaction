using OThinker.H3.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    /// <summary>
    /// RESTful适配器参数模型
    /// </summary>
    public class RESTfulParamViewModel : ViewModelBase
    {
        /// <summary>
        /// 获取或设置所属业务服务编码
        /// </summary>
        public string BizServiceCode { get; set; }

        /// <summary>
        /// 获取或设置参数名称
        /// </summary>
        public string ParamName { get; set; }

        /// <summary>
        /// 获取或设置是否必填
        /// </summary>
        public bool IsRequired { get; set; }

        /// <summary>
        /// 获取或设置参数类型
        /// </summary>
        public string ParamDataType { get; set; }

        /// <summary>
        /// 获取或设置参数类型名称
        /// </summary>
        public string ParamDataTypeName { get; set; }

        /// <summary>
        /// 获取或设置参数描述
        /// </summary>
        public string ParamDesc { get; set; }

        /// <summary>
        /// 获取或设置RESTful验证值
        /// </summary>
        public string ValidateValue { get; set; }

        /// <summary>
        /// 获取或设置父属性名称
        /// </summary>
        public string ParentParamName { get; set; }

        /// <summary>
        /// 获取或设置子参数
        /// </summary>
        public List<RESTfulParamViewModel> children { get; set; }

        /// <summary>
        /// 获取或设置参数类型
        /// </summary>
        public ParamType ParamType { get; set; }

        /// <summary>
        /// 获取或设置是否新建
        /// </summary>
        public bool IsNew { get; set; }
    }

    /// <summary>
    /// 参数类型
    /// </summary>
    public enum ParamType { 

        /// <summary>
        /// 输入参数
        /// </summary>
        InputParam=0,

        /// <summary>
        /// 输出参数
        /// </summary>
        OutputParam=1
    }
}

