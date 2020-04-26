using DongZheng.H3.WebApi.Common;
using OThinker.H3.Controllers;
using OThinker.Organization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi
{
    public static class Proposal
    {
        #region Basic Methods
        /// <summary>
        /// 根据Cap的状态Code获取对应的中文名称
        /// </summary>
        /// <param name="code">CAP状态Code</param>
        /// <returns></returns>
        public static string GetCAPStateName(string code)
        {
            switch (code)
            {
                case "06": return "新的";
                case "78": return "覆盖批准";
                case "84": return "修改-更新";
                case "14": return "草稿";
                case "04": return "取消";
                case "02": return "拒绝";
                case "05": return "待决定";
                case "01": return "核准";
                case "07": return "进行中";
                case "97": return "驳回";
                case "55": return "转换";

                default: return code;
            }
        }
        /// <summary>
        /// 根据RoleCode获取对应的中文名称
        /// </summary>
        /// <param name="role_code">Role Code（B、C、G）</param>
        /// <returns></returns>
        public static string GetCAPRoleName(string role_code)
        {
            switch (role_code)
            {
                case "B": return "借款人";
                case "C": return "共借人";
                case "G": return "担保人";

                default: return role_code;
            }
        }

        public static rtn_data GenerateProposalTask(string UserCode, string CustomerName, string IdCardNumber, string Mobile, string AppOrderId)
        {
            rtn_data rtn = new rtn_data();
            var para_get_bp_id = new Dictionary<string, object>();
            para_get_bp_id.Add("login_nme", UserCode);

            var bp_id_result = BizService.ExecuteBizNonQuery("DropdownListDataSource", "get_bp_id", para_get_bp_id);
            if (bp_id_result == null || bp_id_result.Count == 0)
            {
                AppUtility.Engine.LogWriter.Write("后台获取bp_id失败，login_nme-->" + UserCode);
                rtn.code = -1;
                rtn.message = "获取bp_id失败，login_nme-->" + UserCode;
            }
            else
            {
                #region 自动发起流程的参数
                #region 人员关系表参数
                var app_type = new List<object>();
                app_type.Add(new
                {
                    IDENTIFICATION_CODE1 = 1,
                    APPLICANT_TYPE = "I",
                    MAIN_APPLICANT = "Y",
                    NAME1 = CustomerName
                });
                #endregion

                #region 申请人信息表参数
                var app_detail = new List<object>();
                app_detail.Add(new
                {
                    IDENTIFICATION_CODE2 = 1,
                    FIRST_THI_NME = CustomerName,
                    ID_CARD_NBR = IdCardNumber
                });
                #endregion

                #region 地址信息表参数
                var app_ads = new List<object>();
                app_ads.Add(new
                {
                    ADDRESS_CODE = 1,
                    IDENTIFICATION_CODE4 = 1
                });
                #endregion

                #region 电话信息表参数
                var app_tel = new List<object>();
                app_tel.Add(new
                {
                    PHONE_SEQ_ID = 1,
                    IDENTIFICATION_CODE5 = 1,
                    ADDRESS_CODE5 = 1,
                    PHONE_NUMBER = Mobile
                });
                #endregion

                #region 工作信息表参数
                var app_work = new List<object>();
                app_work.Add(new
                {
                    EMPLOYEE_LINE_ID = 1,
                    IDENTIFICATION_CODE6 = 1
                });
                #endregion

                var para = Newtonsoft.Json.JsonConvert.SerializeObject(new
                {
                    USER_NAME = UserCode,//FI账号
                    APPLICATION_NAME = CustomerName,   //主贷人的名称
                    APPLICANT_TYPE = app_type,
                    APPLICATION_TYPE_NAME = "个人Individual",
                    BP_ID = bp_id_result["BP_PRIMARY_ID"] + string.Empty,
                    AppUnionId = AppOrderId,
                    APPLICANT_DETAIL = app_detail,
                    ADDRESS = app_ads,
                    APPLICANT_PHONE_FAX = app_tel,
                    EMPLOYER = app_work
                });
                #endregion

                var r = new BPM().StartWorkflow_Base(UserCode, "APPLICATION", false, "", para);
                AppUtility.Engine.LogWriter.Write(Newtonsoft.Json.JsonConvert.SerializeObject(r));
                if (r.STATUS == "2")
                {
                    rtn.code = 1;
                    rtn.message = r.MESSAGE;
                    rtn.data = r.INSTANCE_ID;
                }
                else
                {
                    rtn.code = -1;
                    rtn.message = r.MESSAGE;
                }
            }
            return rtn;
        }
        #endregion
    }
}
