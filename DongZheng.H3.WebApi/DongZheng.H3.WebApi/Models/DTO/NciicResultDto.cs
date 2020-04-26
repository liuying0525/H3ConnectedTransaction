using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.DTO
{
    /// <summary>
    /// NCICC返回Data模型
    /// </summary>
    public class NciicResultDto
    {
        /// <summary>
        /// 人员角色	
        /// </summary>
        public string personnelCategory { get; set; } = string.Empty;

        public string PersonnelCategoryName
        {
            get
            {
                var category = "";
                switch (personnelCategory)
                {
                    case "A":
                        category = "申请人";
                        break;
                    case "B":
                        category = "共同申请人";
                        break;
                    case "C":
                        category = "担保人";
                        break;
                    default:
                        break;

                }

                return category;
            }
        }

        /// <summary>
        /// 公民身份号码	
        /// </summary>
        public string gmsfhm { get; set; } = string.Empty;

        /// <summary>
        /// 姓名
        /// </summary>
        public string xm { get; set; } = string.Empty;

        /// <summary>
        /// 曾用名	
        /// </summary>
        public string cym { get; set; } = string.Empty;

        /// <summary>
        /// 性别
        /// </summary>
        public string xb { get; set; } = string.Empty;

        /// <summary>
        /// 民族
        /// </summary>
        public string mz { get; set; } = string.Empty;

        /// <summary>
        /// 出生日期	
        /// </summary>
        public string csrq { get; set; } = string.Empty;

        /// <summary>
        /// 文化程度	
        /// </summary>
        public string whcd { get; set; } = string.Empty;

        /// <summary>
        /// 婚姻状况	
        /// </summary>
        public string hyzk { get; set; } = string.Empty;

        /// <summary>
        /// 服务处所	
        /// </summary>
        public string fwcs { get; set; } = string.Empty;

        /// <summary>
        /// 出生地省市县（区）
        /// </summary>
        public string csdssx { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯省市县（区）
        /// </summary>
        public string jgssx { get; set; } = string.Empty;

        /// <summary>
        /// 所属省市县（区）
        /// </summary>
        public string ssssxq { get; set; } = string.Empty;

        /// <summary>
        /// 住址
        /// </summary>
        public string zz { get; set; } = string.Empty;

        /// <summary>
        /// 公民身份号码比对结果
        /// </summary>
        public string resultGmsfhm { get; set; } = string.Empty;

        /// <summary>
        /// 姓名比对结果
        /// </summary>
        public string resultXm { get; set; } = string.Empty;

        /// <summary>
        /// 曾用名比对结果
        /// </summary>
        public string resultCym { get; set; } = string.Empty;

        /// <summary>
        /// 性别比对结果
        /// </summary>
        public string resultXb { get; set; } = string.Empty;

        /// <summary>
        /// 民族比对结果
        /// </summary>
        public string resultMz { get; set; } = string.Empty;


        /// <summary>
        /// 出生日期比对结果	
        /// </summary>
        public string resultCsrq { get; set; } = string.Empty;

        /// <summary>
        /// 文化程度比对结果		
        /// </summary>
        public string resultWhcd { get; set; } = string.Empty;

        /// <summary>
        /// 婚姻状况比对结果			
        /// </summary>
        public string resultHyzk { get; set; } = string.Empty;

        /// <summary>
        /// 服务处所比对结果			
        /// </summary>
        public string resultFwcs { get; set; } = string.Empty;

        /// <summary>
        /// 出生地省市县（区）比对结果				
        /// </summary>
        public string resultCsdssx { get; set; } = string.Empty;

        /// <summary>
        /// 籍贯省市县（区）比对结果				
        /// </summary>
        public string resultJgssx { get; set; } = string.Empty;

        /// <summary>
        /// 所属省市县（区）比对结果				
        /// </summary>
        public string resultSsssxq { get; set; } = string.Empty;

        /// <summary>
        /// 住址比对结果				
        /// </summary>
        public string resultZz { get; set; } = string.Empty;
    }
}