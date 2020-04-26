using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using OThinker.H3.Controllers;
using OThinker.H3.BizBus;
using OThinker.H3.BizBus.BizService;

public static class BizService
{
    /// <summary>
    /// 后台调用业务服务
    /// </summary>
    /// <param name="serviceCode">业务服务编码</param>
    /// <param name="serviceMethod">方法名称</param>
    /// <param name="dicParams">参数字典</param>
    public static Dictionary<string, object> ExecuteBizNonQuery(string serviceCode, string serviceMethod, Dictionary<string, object> dicParams)
    {
        try
        {
            // 获得业务方法
            MethodSchema method = AppUtility.Engine.BizBus.GetMethod(serviceCode, serviceMethod);
            // 获得参数列表
            BizStructure param = null;
            if (dicParams != null)
            {
                // 填充业务方法需要的参数
                param = BizStructureUtility.Create(method.ParamSchema);
                foreach (var item in dicParams)
                {
                    param[item.Key] = item.Value;
                }
            }
            // 调用方法返回结果
            BizStructure ret = null;

            // 调用方法，获得返回结果
            ret = AppUtility.Engine.BizBus.Invoke(
                 new BizServiceInvokingContext(
                     OThinker.Organization.User.AdministratorID,
                     serviceCode,
                     method.ServiceVersion,
                     method.MethodName,
                     param));
            Dictionary<string, object> result = new Dictionary<string, object>();
            if (ret != null && ret.Schema != null)
            {
                foreach (ItemSchema item in ret.Schema.Items)
                {
                    result.Add(item.Name, ret[item.Name]);
                }
            }
            return result;
        }
        catch (Exception ex)
        {
            // 调用错误日志记录
            AppUtility.Engine.LogWriter.Write("业务服务调用错误：" + ex);
            return null;
        }
    }
}
