using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.FaceSign
{
    /// <summary>
    /// 人员信息
    /// </summary>
    public class PersonsItemInfo
    {
        /// <summary>
        /// 主借人
        /// </summary>
        public string PersonCategory { get; set; }

        /// <summary>
        /// 张三
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 男
        /// </summary>
        public string Sex { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string IdNumber { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Birthday { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 未婚
        /// </summary>
        public string MaritalStatus { get; set; }

        /// <summary>
        /// 共有住宅
        /// </summary>
        public string PropertyType { get; set; }

        /// <summary>
        /// 上海上海市浦东新区龙耀路号
        /// </summary>
        public string LivingAddress { get; set; }

        /// <summary>
        /// 东正金融
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 上海
        /// </summary>
        public string CompanyProvince { get; set; }

        /// <summary>
        /// 上海上海市浦东新区龙耀路号
        /// </summary>
        public string CompanyAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaritalName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string MaritalMobile { get; set; }

        /// <summary>
        /// 共借人
        /// </summary>
        public string CoBorrowerName { get; set; }

        /// <summary>
        /// 抵押人
        /// </summary>
        public string MortgagorName { get; set; }
    }

    /// <summary>
    /// 合同信息
    /// </summary>
    public class ContractInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 百禄贷-车秒贷
        /// </summary>
        public string ProductGroupName { get; set; }

        /// <summary>
        /// 车秒贷外-首20-1298
        /// </summary>
        public string ProductType { get; set; }

        /// <summary>
        /// 经销商名称
        /// </summary>
        public string DealerName { get; set; }

        /// <summary>
        /// 奥迪
        /// </summary>
        public string AssetMake { get; set; }

        /// <summary>
        /// 奥迪A4(进口)
        /// </summary>
        public string AssetBrand { get; set; }

        /// <summary>
        /// 2019款 奥迪A4(进口) 45 TFSI allroad quattro 运动型
        /// </summary>
        public string VehicleComment { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int LeaseTermMonth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal CashDeposit { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string SecurityDepositPCT { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal NewPrice { get; set; }

        /// <summary>
        /// 贷款金额
        /// </summary>
        public decimal AmountFinanced { get; set; }

        /// <summary>
        /// 合同价格
        /// </summary>
        public decimal ContractPrice { get; set; }

        /// <summary>
        /// 车辆销售价格
        /// </summary>
        public decimal CarPrice { get; set; }

        /// <summary>
        /// 附加费金额
        /// </summary>
        public decimal ExtraChargePrice { get; set; }

        /// <summary>
        /// 抵押省市区
        /// </summary>
        public string MortgageCity { get; set; }
    }

    public class FaceSignRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public string AccountNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ApplicationNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<PersonsItemInfo> Persons { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public ContractInfo Contract { get; set; }
    }
}