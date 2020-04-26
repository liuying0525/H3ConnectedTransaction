using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Models.ZhengYuanModel
{
    /// <summary>
    /// 正源终审--请求模型
    /// </summary>
    public class ZyFinalAuditRequest : ZyRequestBase
    {
        /// <summary>
        /// 交易服务码
        /// </summary>
        public string transCode { get { return "M990001"; } }

        /// <summary>
        /// 异步通知的回调地址
        /// </summary>
        public string returnUrl { get; set; }

        /// <summary>
        /// 申请序列号
        /// </summary>
        public string appSeq { get; set; }

        /// <summary>
        /// 应用提交类型 f:初审 s:终审(只传终审)
        /// </summary>
        public string appType { get { return "s"; } }

        public ZYApplyBaseInfo applyBaseInfo { get; set; }

        public ZYBaseInfo baseInfo { get; set; }

        public ZYReviewResultInfo reviewResultInfo { get; set; }

        public ZYPersonalInfo personalInfo { get; set; }

        public ZYHouseInfo houseInfo { get; set; }

        public ZYComponyInfo componyInfo { get; set; }

        public ZYSpouseInfo spouseInfo { get; set; }

        public ZYCarInfo carInfo { get; set; }

        public ZYMerchant merchant { get; set; }
    }

    /// <summary>
    /// 申请人基本信息域
    /// </summary>
    public class ZYApplyBaseInfo
    {
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string bankCardNo { get; set; }

        /// <summary>
        /// 银行预留手机号
        /// </summary>
        public string bankCardReservePhone { get; set; }
    }

    /// <summary>
    /// 申请信息
    /// </summary>
    public class ZYBaseInfo
    {
        /// <summary>
        /// 是否回退标识 --合作方特有字段，如果为放款确认回退到高审重新发起终审传1，其他时候不传
        /// </summary>
        public string isReject { get; set; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public string applyAmt { get; set; }

        /// <summary>
        /// 申请期数
        /// </summary>
        public string applyTerm { get; set; }

        /// <summary>
        /// 职业身份
        /// </summary>
        public string occupationalIdentity { get; set; }

        /// <summary>
        /// 借款用途 只选自用购车-传汉字
        /// </summary>
        public string loanUsage { get { return "自用购车"; } }
    }

    /// <summary>
    /// 高审结果信息域
    /// </summary>
    public class ZYReviewResultInfo
    {
        /// <summary>
        /// 审批金额
        /// </summary>
        public string approAmt { get; set; }

        /// <summary>
        /// 合同金额
        /// </summary>
        public string contractAmt { get; set; }

        /// <summary>
        /// 新车价格
        /// </summary>
        public string reconsiderAmt { get; set; }

        /// <summary>
        /// 审批期限
        /// </summary>
        public string approTerm { get; set; }

        /// <summary>
        /// 月综合利率
        /// </summary>
        public string monthTotalRate { get; set; }

        /// <summary>
        /// 还款方式
        /// </summary>
        public string paymType { get; set; }

    }

    /// <summary>
    /// 个人信息
    /// </summary>
    public class ZYPersonalInfo
    {
        /// <summary>
        /// 姓名
        /// </summary>
        public string name { get; set; }

        public string email { get; set; }

        /// <summary>
        /// 1. 男 2. 女
        /// </summary>
        public string sex { get; set; }

        public string birthday { get; set; }

        public string age { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string idNo { get; set; }

        /// <summary>
        /// 居住地址省份
        /// </summary>
        public string curProvice { get; set; }

        /// <summary>
        /// 居住地址城市
        /// </summary>
        public string curCity { get; set; }

        /// <summary>
        /// 居住地址区县
        /// </summary>
        public string curCounty { get; set; }

        /// <summary>
        /// 居住详细地址
        /// </summary>
        public string curAddress { get; set; }

        /// <summary>
        /// 户籍地址省份
        /// </summary>
        public string perProvice { get; set; }

        /// <summary>
        /// 户籍地址城市
        /// </summary>
        public string perCity { get; set; }

        /// <summary>
        /// 户籍地址区县
        /// </summary>
        public string perCounty { get; set; }

        /// <summary>
        /// 户籍地址
        /// </summary>
        public string perAddr { get; set; }

        /// <summary>
        /// 本地居住年限(年)
        /// </summary>
        public string ageLimi { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public string mariStatus { get; set; }

        /// <summary>
        /// 居住状况
        /// </summary>
        public string houseType { get; set; }

        /// <summary>
        /// 移动电话
        /// </summary>
        public string mobile { get; set; }

    }

    /// <summary>
    /// 房产信息
    /// </summary>
    public class ZYHouseInfo
    {
        /// <summary>
        /// 本人名下是否有房产 默认1 ,0：是   1：否
        /// </summary>
        public string isExistHome { get; set; }

        /// <summary>
        /// 房产地址省份
        /// </summary>
        public string proProvice { get; set; }

        /// <summary>
        /// 房产地址城市
        /// </summary>
        public string proCity { get; set; }

        /// <summary>
        /// 房产地址区县
        /// </summary>
        public string proCounty { get; set; }

        /// <summary>
        /// 房产详细地址
        /// </summary>
        public string proAddrDetail { get; set; }

        /// <summary>
        /// 房产来源
        /// </summary>
        public string houseSource { get; set; }

        /// <summary>
        /// 当前房产状态
        /// </summary>
        public string propertyStatus { get; set; }

    }

    /// <summary>
    /// 公司信息
    /// </summary>
    public class ZYComponyInfo
    {
        /// <summary>
        /// 单位名称
        /// </summary>
        public string componyName { get; set; }

        /// <summary>
        /// 单位地址省份
        /// </summary>
        public string comProvice { get; set; }

        /// <summary>
        /// 单位地址城市
        /// </summary>
        public string comCity { get; set; }

        /// <summary>
        /// 单位地址区县
        /// </summary>
        public string comCounty { get; set; }

        /// <summary>
        /// 单位详细地址
        /// </summary>
        public string comAddrDetail { get; set; }

        /// <summary>
        /// 单位电话
        /// </summary>
        public string componyPhone { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string position { get; set; }

    }

    /// <summary>
    /// 配偶/直系亲属
    /// </summary>
    public class ZYSpouseInfo
    {
        public string name { get; set; }
        public string phone { get; set; }
        public string relation { get; set; }

    }

    /// <summary>
    /// 车辆信息
    /// </summary>
    public class ZYCarInfo
    {
        /// <summary>
        /// 办单门店
        /// </summary>
        public string shop { get; set; }

        /// <summary>
        /// 办单城市
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// 办单省份
        /// </summary>
        public string provide { get; set; }

        /// <summary>
        /// 车辆识别代码（VIN码）
        /// </summary>
        public string carCode { get; set; }

        /// <summary>
        /// 发动机号码
        /// </summary>
        public string engineCode { get; set; }

        /// <summary>
        /// 车辆品牌
        /// </summary>
        public string carBrand { get; set; }

        /// <summary>
        /// 车系
        /// </summary>
        public string carCategory { get; set; }

        /// <summary>
        /// 车型
        /// </summary>
        public string carType { get; set; }

        /// <summary>
        /// 购买价格
        /// </summary>
        public string purchasePrice { get; set; }

    }


    /// <summary>
    /// 商户信息（放款银行信息）
    /// </summary>
    public class ZYMerchant
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string merchantName { get; set; }

        /// <summary>
        /// 商户证件类型01-身份证（对私账户）      10-组织机构代码（对公账户
        /// </summary>
        public string merchantIdType { get; set; }

        /// <summary>
        /// 商户证件编码
        /// </summary>
        public string merchantIdNo { get; set; }

        /// <summary>
        /// 商户账号类型 01-对私账户   02-对公账户
        /// </summary>
        public string merchantAcctType { get; set; }

        /// <summary>
        /// 商户开户银行名称
        /// </summary>
        public string merchantBankName { get; set; }

        /// <summary>
        /// 商户银行代码
        /// </summary>
        public string merchantBankCode { get; set; }

        /// <summary>
        /// 商户银行卡号
        /// </summary>
        public string merchantBankCardNo { get; set; }

        /// <summary>
        /// 开户银行省
        /// </summary>
        public string bankProvince { get; set; }

        /// <summary>
        /// 开户银行市
        /// </summary>
        public string bankCity { get; set; }

        /// <summary>
        /// 开户银行支行
        /// </summary>
        public string bankChildName { get; set; }
        
    }
}