using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi
{
    public class Risk
    {
        public class Parameters_Result
        {
            public string recordId { get; set; }
            public string reqId { get; set; }
            public string requestId { get; set; }
            public string finalResult { get; set; }
            public string refuseReason { get; set; }

            //页面展示结果
            public string showResult { get; set; }

            public bool IsEmpty()
            {
                if (string.IsNullOrEmpty(recordId) || string.IsNullOrEmpty(reqId) ||
                    string.IsNullOrEmpty(requestId) || string.IsNullOrEmpty(finalResult))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public class Parameters_Evaluation
        {
            public string reqId { get; set; }
            public string cbUrl { get; set; }
            public string time { get; set; }
            public string requestId { get; set; }
            public string requestFlags { get; set; }
            public List<Person> personnelList { get; set; }
            public Order order { get; set; }


            public class Person
            {
                /// <summary>
                /// 人员类别：00主贷人；01共同借款人；02担保人
                /// </summary>
                public string personnelCategory { get; set; }
                /// <summary>
                /// 人员关系
                /// </summary>
                public string personnelRelationship { get; set; }
                /// <summary>
                /// 曾用名
                /// </summary>
                public string formerName { get; set; }
                /// <summary>
                /// 姓名
                /// </summary>
                public string custName { get; set; }
                /// <summary>
                /// 证件类型
                /// </summary>
                public string certType { get; set; }
                /// <summary>
                /// 证件号码
                /// </summary>
                public string certNo { get; set; }
                /// <summary>
                /// 证件到期日
                /// </summary>
                public string certEndDate { get; set; }
                /// <summary>
                /// 手机号
                /// </summary>
                public string mobile { get; set; }
                /// <summary>
                /// 性别
                /// </summary>
                public string gender { get; set; }
                /// <summary>
                /// 年龄（年）
                /// </summary>
                public string birthdayYear { get; set; }
                /// <summary>
                /// 年龄（月）
                /// </summary>
                public string birthdayMonth { get; set; }
                /// <summary>
                /// 出生日期
                /// </summary>
                public string custBirthday { get; set; }
                /// <summary>
                /// 国籍
                /// </summary>
                public string nationality { get; set; }
                /// <summary>
                /// 民族
                /// </summary>
                public string nation { get; set; }
                /// <summary>
                /// 教育程度
                /// </summary>
                public string educationLevel { get; set; }
                /// <summary>
                /// 婚姻状态
                /// </summary>
                public string maritalStatus { get; set; }
                /// <summary>
                /// 住房情况
                /// </summary>
                public string houseStatus { get; set; }
                /// <summary>
                /// 单位名称
                /// </summary>
                public string employer { get; set; }
                /// <summary>
                /// 单位电话
                /// </summary>
                public string employerTelphone { get; set; }
                /// <summary>
                /// 单位地址（省）
                /// </summary>
                public string employerAddressProvince { get; set; }
                /// <summary>
                /// 单位地址（市）
                /// </summary>
                public string employerAddressCity { get; set; }
                /// <summary>
                /// 单位地址
                /// </summary>
                public string employerAddress { get; set; }
                /// <summary>
                /// 岗位
                /// </summary>
                public string occupation { get; set; }
                /// <summary>
                /// 工作年限（年份）
                /// </summary>
                public string workingYears { get; set; }
                /// <summary>
                /// 月收入
                /// </summary>
                public string incomeMonth { get; set; }
                /// <summary>
                /// 出生地省市县（区）
                /// </summary>
                public string birthPalaceProvince { get; set; }
                /// <summary>
                /// 籍贯省市县（区）
                /// </summary>
                public string nativeDistrict { get; set; }
                /// <summary>
                /// 所属省市县（区）
                /// </summary>
                public string domicilePlace { get; set; }
                /// <summary>
                /// 现居住地省份
                /// </summary>
                public string currentAddressProvince { get; set; }
                /// <summary>
                /// 现居住地城市
                /// </summary>
                public string currentAddressCity { get; set; }

                /// <summary>
                /// 现居住地城市
                /// </summary>
                public string currentAddressCounty { get; set; }

                /// <summary>
                /// 现居住住址
                /// </summary>
                public string currentLivingAddress { get; set; }
                /// <summary>
                /// 是否调用征信
                /// </summary>
                public string isInvokePboc { get; set; }

                /// <summary>
                /// 是否抵押人 0否  1是
                /// </summary>
                public string lienee { get; set; }
            }

            public class Order
            {
                public string businessType { get; set; }
                public string foursNumber { get; set; }
                public string dealerNam { get; set; }
                public string vehicleModel { get; set; }
                public string vehicleBrand { get; set; }
                public string vehiclePurpose { get; set; }
                public string vehiclePrice { get; set; }
                public string applicationAmount { get; set; }
                public string applicationTerm { get; set; }
                public string productGroup { get; set; }
                public string productType { get; set; }
                public string firstPaymentsRatio { get; set; }
                public string firstPaymentsAmount { get; set; }
                public string vehicleIdentificationValue { get; set; }
                public string extraChargeLoan { get; set; }
                public string ifRereqComplete { get; set; }
                public string ifMortgage { get; set; }
                public string vehicleManufactureDate { get; set; }
                public string vehicleKilometers { get; set; }
                public string ifDataComplete { get; set; }
                public string ifStockSimplifiedLoan { get; set; }
                public string exposureValue { get; set; }
                public string bankNo { get; set; }
                public string ifQualityDealerCode { get; set; }
                public string plegeAddressCity { get; set; }
                public string resideAddressProvince { get; set; }
                public string dealerAddressCity { get; set; }
                public string customerSource { get; set; }
                public decimal interestRates { get; set; }
                public string dealerType { get; set; }
                public int maleCardLoans { get; set; }

                /// <summary>
                /// 贷款类型
                /// </summary>
                public string loanType { get; set; }
            }

            public class PersonsInfo
            {
                /// <summary>
                /// 人员信息
                /// </summary>
                public List<Person> Persons { get; set; }
                /// <summary>
                /// 借款人是否是主贷人
                /// </summary>
                public string Ifmortgage { get; set; }
            }
        }

        public class Result_NCIIC
        {
            public string reqID { get; set; }
            public string requestId { get; set; }
            public List<NciicResultDto> nciicResultList { get; set; }

            public class NciicResultDto
            {
                public string personnelCategory { get; set; }
                public string gmsfhm { get; set; }
                public string xm { get; set; }
                public string cym { get; set; }
                public string xb { get; set; }
                public string mz { get; set; }
                public string csrq { get; set; }
                public string whcd { get; set; }
                public string hyzk { get; set; }
                public string fwcs { get; set; }
                public string csdssx { get; set; }
                public string jgssx { get; set; }
                public string ssssxq { get; set; }
                public string zz { get; set; }
                public string resultGmsfhm { get; set; }
                public string resultXm { get; set; }
                public string resultCym { get; set; }
                public string resultXb { get; set; }
                public string resultMz { get; set; }
                public string resultCsrq { get; set; }
                public string resultWhcd { get; set; }
                public string resultHyzk { get; set; }
                public string resultFwcs { get; set; }
                public string resultCsdssx { get; set; }
                public string resultJgssx { get; set; }
                public string resultSsssxq { get; set; }
                public string resultZz { get; set; }
            }
        }
    }
}