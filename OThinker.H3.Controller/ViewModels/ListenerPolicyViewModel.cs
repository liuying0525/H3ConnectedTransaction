using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OThinker.H3.Controllers.ViewModels
{
    public class ListenerPolicyViewModel : ViewModelBase
    {

        ///<summary>
        ///获取或设置监听类型
        ///</summary>
        public string PolicyType { get; set; }

        ///<summary>
        ///获取或设置监听间隔
        ///</summary>
        public string IntervalSecond { get; set; }

        ///<summary>
        ///获取或设置过滤方法
        ///</summary>
        public string FilterMethod { get; set; }

        ///<summary>
        ///获取或设置最近更新的对象
        ///</summary>
        public List<string> Conditions { get; set; }

        ///<summary>
        ///获取或设置过滤方式
        ///</summary>
        public string Filter { get; set; }

        ///<summary>
        ///获取或设置字段名称
        ///</summary>
        public string Field { get; set; }

        ///<summary>
        ///获取或设置运算符
        ///</summary>
        public string Operator { get; set; }

        ///<summary>
        ///获取或设置条件值
        ///</summary>
        public string ConditionValue { get; set; }

        /// <summary>
        /// 获取或设置数据模型编码
        /// </summary>
        public string SchemaCode { get; set; }
    }
}
