using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models
{
    /// <summary>
    /// 东正api接口返回固定模型
    /// </summary>
    public class DZCommonApiResult<T>
    {
        public string code { get; set; }

        public string message { get; set; }

        public T data { get; set; }
    }
}