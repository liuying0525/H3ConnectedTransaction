using System;

namespace OThinker.H3.Controllers
{
    /// <summary>
    /// Portal�ж���Session������
    /// </summary>
    public class Sessions
    {
        /// <summary>
        /// ҳ������
        /// </summary>
        /// <returns></returns>
        public static string GetSheetSettingsName()
        {
            return "SheetSettings";
        }

        /// <summary>
        /// ����ģ��Session������
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
        /// ��õ�½�û�����֤��
        /// </summary>
        /// <returns></returns>
        public static string GetUserValidator()
        {
            return "UserValidator";
        }

        /// <summary>
        /// ��ȡ�û��Ƿ���΢�ŵ�¼
        /// </summary>
        /// <returns></returns>
        public static string GetWeChatLogin()
        {
            return "WeChatLogin";
        }

        /// <summary>
        /// ��ȡ�û��Ƿ���΢�ŵ�¼
        /// </summary>
        /// <returns></returns>
        public static string GetDingTalkLogin()
        {
            return "DingTalkLogin";
        }

        /// <summary>
        /// ��õ�¼��
        /// </summary>
        /// <returns></returns>
        public static string GetUserAlias()
        {
            return "H3_UserAlias";
        }

        /// <summary>
        /// ����������ʹ�õ�
        /// </summary>
        /// <returns></returns>
        public static string GetLang()
        {
            return "H3_Language";
        }

        /// <summary>
        /// ��ȡLicense������
        /// </summary>
        /// <returns></returns>
        public static string GetLicenseType()
        {
            return "LicenseType";
        }
    }
}
