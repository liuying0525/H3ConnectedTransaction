using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// Portal中对于Session的引用
    /// </summary>
    public class Sessions
    {
        /// <summary>
        /// 页面设置
        /// </summary>
        /// <returns></returns>
        public static string GetSheetSettingsName()
        {
            return "SheetSettings";
        }

        /// <summary>
        /// 流程模板Session的名称
        /// </summary>
        /// <param name="WorkflowPackage"></param>
        /// <param name="WorkflowName"></param>
        /// <param name="WorkflowVersion"></param>
        /// <returns></returns>
        public static string GetWorkflow(
            string WorkflowCode,
            int WorkflowVersion)
        {
            return "Workflow_" + WorkflowCode + "." + WorkflowVersion;
        }

        /// <summary>
        /// 获得登陆用户的验证器
        /// </summary>
        /// <returns></returns>
        public static string GetUserValidator()
        {
            return "UserValidator";
        }

        /// <summary>
        /// 获取用户是否是微信登录
        /// </summary>
        /// <returns></returns>
        public static string GetWeChatLogin()
        {
            return "WeChatLogin";
        }

        /// <summary>
        /// 获取用户是否是微信登录
        /// </summary>
        /// <returns></returns>
        public static string GetDingTalkLogin()
        {
            return "DingTalkLogin";
        }

        /// <summary>
        /// 获得登录名
        /// </summary>
        /// <returns></returns>
        public static string GetUserAlias()
        {
            return "H3_UserAlias";
        }

        /// <summary>
        /// 保存语言所使用的
        /// </summary>
        /// <returns></returns>
        public static string GetLang()
        {
            return "H3_Language";
        }

        /// <summary>
        /// 获取License的类型
        /// </summary>
        /// <returns></returns>
        public static string GetLicenseType()
        {
            return "LicenseType";
        }
    }
}
