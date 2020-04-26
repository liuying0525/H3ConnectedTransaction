using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OThinker.H3.Controllers;
using Newtonsoft.Json;

namespace DongZheng.H3.WebApi.Common.PreApproval
{
    public class PreApproval
    {
        string url_QueryYSPResult = System.Configuration.ConfigurationManager.AppSettings["QueryYSPResult"] + string.Empty;
        public PreApprovalData QueryPreApprovalResult(string CustomerName, string IdCardNumber)
        {
            if (string.IsNullOrEmpty(CustomerName) || string.IsNullOrEmpty(IdCardNumber))
            {
                AppUtility.Engine.LogWriter.Write(string.Format("QueryPreApprovalResult参数为空CustomerName-->{0},IdCardNumber-->{1}", CustomerName, IdCardNumber));
                return null;
            }
            string para = JsonConvert.SerializeObject(new
            {
                name = CustomerName,
                idCard = IdCardNumber
            });
            var result = HttpHelper.PostWebRequest(url_QueryYSPResult, "application/json", para);
            if (string.IsNullOrEmpty(result))
            {
                AppUtility.Engine.LogWriter.Write(string.Format("QueryPreApprovalResult结果为空CustomerName-->{0},IdCardNumber-->{1}", CustomerName, IdCardNumber));
                return null;
            }
            ResponseData<PreApprovalData> responseData = JsonConvert.DeserializeObject<ResponseData<PreApprovalData>>(result);
            //Code:00参数错误，01有记录，02无记录；
            if (responseData.Code == "01")
            {
                return responseData.Data;
            }
            else
            {
                AppUtility.Engine.LogWriter.Write(string.Format("QueryPreApprovalResult结果为{2},CustomerName-->{0},IdCardNumber-->{1}", CustomerName, IdCardNumber, result));
                return null;
            }
        }
    }

    public class PreApprovalData
    {
        /// <summary>
        /// 预审批结果:01通过  03拒绝   06风险用户  07信息有误
        /// </summary>
        public string preResult { get; set; }
        /// <summary>
        /// 银行卡一
        /// </summary>
        public string firstBankCard { get; set; }
        /// <summary>
        /// 银行卡二
        /// </summary>
        public string secondBankCard { get; set; }
        /// <summary>
        /// 手机号码
        /// </summary>
        public string mobiePhone { get; set; }
        /// <summary>
        /// 提示信息
        /// </summary>
        public string message { get; set; }

    }
}