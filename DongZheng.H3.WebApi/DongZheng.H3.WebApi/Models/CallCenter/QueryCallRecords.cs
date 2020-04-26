using OThinker.H3.Controllers.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.CallCenter
{
    public class QueryCallRecordsRquest : PagerInfo
    {
        public string ObjectId { get; set; }
        /// <summary>
        /// 0呼出，1呼入
        /// </summary>
        public int CallType { get; set; }

        public string ApplicationNumber { get; set; }

        public string ContractNo { get; set; }

        public string MainApplicantName { get; set; }

        /// <summary>
        /// calltype=0主叫人姓名，calltype=1被叫人姓名
        /// </summary>
        public string CallerName { get; set; }

        /// <summary>
        /// calltype=0主叫人职务，calltype=1被叫人职务
        /// </summary>
        public string CallerPosition { get; set; }

        /// <summary>
        /// calltye=0被呼叫用户姓名(呼出),calltye=1主叫人用户姓名(呼入)
        /// </summary>
        public string CalledName { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户角色类型(呼出),calltye=1主叫人角色类型(呼入)
        /// </summary>
        public string CalledType { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户号码(呼出),calltype=1主叫人号码(呼入)
        /// </summary>
        public string CalledNumber { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户电话类型(呼出),calltype=1主叫人电话类型(呼入）
        /// </summary>
        public string CalledNumberType { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户证件类型(呼出),calltype=1主叫人证件类型(呼入)
        /// </summary>
        public string CalledIdType { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户证件号码(呼出),calltype=1主叫人证件号码(呼入)
        /// </summary>
        public string CalledIdNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 录音文件名
        /// </summary>
        public string RecordFileName { get; set; }

        /// <summary>
        /// 电话系统唯一id
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// 咨询类型
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// 通话时长
        /// </summary>
        public int CallDuration { get; set; }

        /// <summary>
        /// 用户code
        /// </summary>
        public string CallerCode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }
    }

    public class QueryCallRecordsResponse
    {
        public int RowNumber { get; set; }

        public string ObjectId { get; set; }
        /// <summary>
        /// 0呼出，1呼入
        /// </summary>
        public int CallType { get; set; }

        public string ApplicationNumber { get; set; }

        public string ContractNo { get; set; }

        public string MainApplicantName { get; set; }

        /// <summary>
        /// calltype=0主叫人姓名，calltype=1被叫人姓名
        /// </summary>
        public string CallerName { get; set; }

        /// <summary>
        /// calltype=0主叫人职务，calltype=1被叫人职务
        /// </summary>
        public string CallerPosition { get; set; }

        /// <summary>
        /// calltye=0被呼叫用户姓名(呼出),calltye=1主叫人用户姓名(呼入)
        /// </summary>
        public string CalledName { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户角色类型(呼出),calltye=1主叫人角色类型(呼入)
        /// </summary>
        public string CalledType { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户号码(呼出),calltype=1主叫人号码(呼入)
        /// </summary>
        public string CalledNumber { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户电话类型(呼出),calltype=1主叫人电话类型(呼入）
        /// </summary>
        public string CalledNumberType { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户证件类型(呼出),calltype=1主叫人证件类型(呼入)
        /// </summary>
        public string CalledIdType { get; set; }

        /// <summary>
        /// calltype=0被呼叫用户证件号码(呼出),calltype=1主叫人证件号码(呼入)
        /// </summary>
        public string CalledIdNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 录音文件名
        /// </summary>
        public string RecordFileName { get; set; }

        /// <summary>
        /// 电话系统唯一id
        /// </summary>
        public string UniqueId { get; set; }

        /// <summary>
        /// 咨询类型
        /// </summary>
        public string QuestionType { get; set; }

        /// <summary>
        /// 通话时长
        /// </summary>
        public int CallDuration { get; set; }

        public string CreateTime { get; set; }

        public List<string> QTpye1Ids { get; set; }

        public List<string> QTpye2Ids { get; set; }
    }
}