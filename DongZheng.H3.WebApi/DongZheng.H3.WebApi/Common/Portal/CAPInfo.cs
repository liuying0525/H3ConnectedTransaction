using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DongZheng.H3.WebApi.Common.Portal
{
    /// <summary>
    /// Application 的摘要说明
    /// </summary>
    public class CAPInfo
    {
        public CAPInfo()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public List<appAsset> appAsset = new List<appAsset>();
        public List<appFinanTerms> appFinanTerms = new List<appFinanTerms>();
        public List<appInfo> appInfo = new List<appInfo>();
        public List<appRental> appRental = new List<appRental>();
        public List<applicantType> applicantType = new List<applicantType>();
        public List<assetLibilities> assetLibilities = new List<assetLibilities>();
        public List<financialRatios> financialRatios = new List<financialRatios>();
        public List<assetAccessories> assetAccessories = new List<assetAccessories>();
        public List<companyInfo> companyInfo = new List<companyInfo>();
        public List<applicantInfo> applicantInfo = new List<applicantInfo>();
        public List<employment> employment = new List<employment>();
        public List<referenceList> referenceList = new List<referenceList>();
        public List<addressList> addressList = new List<addressList>();
        public List<phoneList> phoneList = new List<phoneList>();

    }

    public class appAsset
    {
        public appAsset()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
     
      public string applicationNo{ get; set; }//Br-A044180000</applicationNo{ get; set; }//
      public string assetConditionCode{ get; set; }//N</assetConditionCode{ get; set; }//
      public string assetConditionName{ get; set; }//新车 New</assetConditionName{ get; set; }//
      public string assetMakeCode{ get; set; }//00026</assetMakeCode{ get; set; }//
      public string assetMakeName{ get; set; }//宝马中国</assetMakeName{ get; set; }//
      public string assetPrice{ get; set; }//454000</assetPrice{ get; set; }//
      public string brandCode{ get; set; }//00852</brandCode{ get; set; }//
      public string brandName{ get; set; }//BMW3系GT</brandName{ get; set; }//
      public string comments{ get; set; }//宝马中国 2017款 320i GT M运动设计套装</comments{ get; set; }//
      public string deliveryDate{ get; set; }//9/22/2017 12:00:00 AM</deliveryDate{ get; set; }//
      public string engineNo { get; set; }//
      public string miocn{ get; set; }//DZ-20170831-V0001</miocn{ get; set; }//
      public string modelCode{ get; set; }//05635</modelCode{ get; set; }//
      public string modelName{ get; set; }//2017款 320i GT M运动设计套装</modelName{ get; set; }//
      public string purposeCode{ get; set; }//1</purposeCode{ get; set; }//
      public string purposeName{ get; set; }//自用 Self-Use</purposeName{ get; set; }//
      public string releaseDate { get; set; }//
      public string releaseMonth{ get; set; }//0</releaseMonth{ get; set; }//
      public string releaseYear{ get; set; }//0</releaseYear{ get; set; }//
      public string series { get; set; }//
      public string transmissionCode{ get; set; }//Y</transmissionCode{ get; set; }//
      public string transmissionName{ get; set; }//自动 Automatic</transmissionName{ get; set; }//
      public string vehicleSubtypeCode{ get; set; }//00164</vehicleSubtypeCode{ get; set; }//
      public string vehicleSubtypeName{ get; set; }//N/A</vehicleSubtypeName{ get; set; }//
      public string vehiclecolorCode { get; set; }//
      public string vehiclecolorName { get; set; }//
      public string vinNo { get; set; }//
      public string manufactureYear { get; set; }//
      public string engineCC { get; set; }//
      public string vehicleAge{ get; set; }//0</vehicleAge{ get; set; }//
      public string registrationNo { get; set; }//
      public string vehicleBody { get; set; }//
      public string style { get; set; }//
      public string cylinder { get; set; }//
      public string odometer{ get; set; }//0</odometer{ get; set; }//
      public string wheelWidth { get; set; }//
      public string guranteeOption{ get; set; }//00001</guranteeOption{ get; set; }//
      public string vineditind{ get; set; }//F</vineditind{ get; set; }//
    }
    public class appFinanTerms
    {
        public appFinanTerms()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
      public string actualRate { get; set; }//11.88</actualRate { get; set; }//
      public string applicationNo { get; set; }//Br-A044180000</applicationNo { get; set; }//
      public string balloonAmount { get; set; }//0</balloonAmount { get; set; }//
      public string balloonRate { get; set; }//0</balloonRate { get; set; }//
      public string deferredTerm { get; set; }//0</deferredTerm { get; set; }//
      public string downpaymentamount { get; set; }//273580.40</downpaymentamount { get; set; }//
      public string downpaymentrate { get; set; }//60.26</downpaymentrate { get; set; }//
      public string financedamount { get; set; }//180419.60</financedamount { get; set; }//
      public string financedamountrate { get; set; }//39.74</financedamountrate { get; set; }//
      public string interestRate { get; set; }//11.88</interestRate { get; set; }//
      public string otherfees { get; set; }//0</otherfees { get; set; }//
      public string paymentFrequencyCode { get; set; }//00001</paymentFrequencyCode { get; set; }//
      public string paymentFrequencyName { get; set; }//Monthly</paymentFrequencyName { get; set; }//
      public string productGroupCode { get; set; }//6</productGroupCode { get; set; }//
      public string productGroupName { get; set; }//等额本息-常规贷</productGroupName { get; set; }//
      public string productTypeCode { get; set; }//92</productTypeCode { get; set; }//
      public string productTypeName { get; set; }//新标贷等额本息</productTypeName { get; set; }//
      public string rentalmodeCode { get; set; }//00003</rentalmodeCode { get; set; }//
      public string rentalmodeName { get; set; }//银行代扣 Direct Debit</rentalmodeName { get; set; }//
      public string salesprice { get; set; }//454000</salesprice { get; set; }//
      public string subsidyAmount { get; set; }//0</subsidyAmount { get; set; }//
      public string subsidyRate { get; set; }//0</subsidyRate { get; set; }//
      public string termMonth { get; set; }//48</termMonth { get; set; }//
      public string totalintamount { get; set; }//47125.36</totalintamount { get; set; }//
      public string vehicleprice { get; set; }//454000</vehicleprice { get; set; }//
      public string accessoriesInd { get; set; }//T</accessoriesInd { get; set; }//
      public string totalaccessoryamount { get; set; } 
    }
    public class appInfo
    {
      public string APPCREATETIME { get; set; }//11/16/2017 12:00:00 AM</APPCREATETIME { get; set; }//
      public string appStatus { get; set; }//14</appStatus { get; set; }//
      public string appTypeCode { get; set; }//00001</appTypeCode { get; set; }//
      public string appTypeName { get; set; }//个人Individual</appTypeName { get; set; }//
      public string applicationNo { get; set; }//Br-A044180000</applicationNo { get; set; }//
      public string comments { get; set; }//
      public string companyCode { get; set; }//1</companyCode { get; set; }//
      public string companyName { get; set; }//上海东正汽车金融有限责任公司</companyName { get; set; }//
      public string dealercityCode { get; set; }//00200</dealercityCode { get; set; }//
      public string dealercityName { get; set; }//珠海</dealercityName { get; set; }//
      public string dealerCode { get; set; }//167</dealerCode { get; set; }//
      public string dealerName { get; set; }//珠海宝泽汽车销售服务有限公司</dealerName { get; set; }//
      public string financialadvisorId { get; set; }//
      public string financialadvisorName { get; set; }//
      public string provinceCode { get; set; }//00018</provinceCode { get; set; }//
      public string provinceName { get; set; }//广东</provinceName { get; set; }//
      public string vehicleTypeCde { get; set; }//00001</vehicleTypeCde { get; set; }//
      public string vehicleTypeName { get; set; }//标准型</vehicleTypeName { get; set; }//
      public string salesPersonCode { get; set; }//337</salesPersonCode { get; set; }//
      public string salesPersonName { get; set; }//珠海宝泽</salesPersonName { get; set; }//
      public string showRoomCode { get; set; }//331</showRoomCode { get; set; }//
      public string showRoomName { get; set; }//珠海宝泽展厅</showRoomName { get; set; }//
      public string distributorCode { get; set; }//
      public string distributorName { get; set; }//
      public string ContactNumber { get; set; }//13726203373</ContactNumber { get; set; }//
      public string refCANumber { get; set; }//121212121212</refCANumber { get; set; }//
      public string userName { get; set; }//9805301</userName { get; set; }//
        
    }
    public class appRental
    {
        public string applicationNo { get; set; }//Br-A043972000</applicationNo{ get; set; }//
        public string rental_seq { get; set; }//1</rental_seq{ get; set; }//
        public string startTerm { get; set; }//1</startTerm{ get; set; }//
        public string endTerm { get; set; }//12</endTerm{ get; set; }//
        public string rentalAmount { get; set; }//14697.98</rentalAmount{ get; set; }//
        public string grossRentalAmount { get; set; }//14697.98</grossRentalAmount{ get; set; }//
    }

    public class applicantType
    {

        public string APPLICATION_NUMBER { get; set; }//Br-A043972000</APPLICATION_NUMBER{ get; set; }//
        public string APPLICANT_TYPE { get; set; }//I</APPLICANT_TYPE{ get; set; }//
        public string GUARANTOR_TYPE { get; set; }//
        public string FIRST_NAME { get; set; }//
        public string LAST_NAME { get; set; }//
        public string COMPANY_NAME { get; set; }//
        public string REGESTRATION_NO { get; set; }//
        public string ID_CARD_NBR { get; set; }//440301198110012725</ID_CARD_NBR{ get; set; }//
        public string IDENTIFICATION_CODE { get; set; }//1</IDENTIFICATION_CODE{ get; set; }//
        public string NAME { get; set; }//陈佳</NAME{ get; set; }//
        public string MAIN_APPLICANT { get; set; }//Y</MAIN_APPLICANT{ get; set; }//
        public string IS_INACTIVE_IND { get; set; }//Y</IS_INACTIVE_IND{ get; set; }//

    }

    public class assetLibilities
    {
      public string Code{ get; set; }//BS120</Code{ get; set; }//
      public string ApplicationNo{ get; set; }//Br-A044180000</ApplicationNo{ get; set; }//
      public string IdentificationCode{ get; set; }//4</IdentificationCode{ get; set; }//
      public string Value{ get; set; }//4740.52</Value{ get; set; }//
      public string FinancialDetailType{ get; set; }//L</FinancialDetailType{ get; set; }//
      public string IdentifierCode{ get; set; }//BS120</IdentifierCode{ get; set; }//
      public string ApplicantRole{ get; set; }//C</ApplicantRole{ get; set; }//
      public string Descritption{ get; set; }//本次车贷月供金额</Descritption{ get; set; }//
      public string Ismandatory{ get; set; }//F</Ismandatory{ get; set; }//
      public string Insertedby { get; set; }//
      public string Insertedon { get; set; }//
      public string Updatedby { get; set; }//
      public string Updatedon { get; set; }//
      public string Pcmind{ get; set; }//F</Pcmind{ get; set; }//
    }
    public class financialRatios
    {
        public string Financialrationcode { get; set; }//HQ001</Financialrationcode{ get; set; }//
        public string Financialratiodes { get; set; }//活期日均Current Interest Average Daily</Financialratiodes{ get; set; }//
        public string Applicationnumber { get; set; }//Br-A043972000</Applicationnumber{ get; set; }//
        public string Evalresult { get; set; }//0</Evalresult{ get; set; }//
        public string Activeind { get; set; }//T</Activeind{ get; set; }//
        public string Visibleind { get; set; }//T</Visibleind{ get; set; }//

    }
    public class assetAccessories
    {
        public string IDENTIFICATION_CODE { get; set; }//1</IDENTIFICATION_CODE{ get; set; }//
        public string additionalCode { get; set; }//00733</additionalCode{ get; set; }//
        public string additionalName { get; set; }//Chestnut trim</additionalName{ get; set; }//
        public string applicationNo { get; set; }//Br-A044180000</applicationNo{ get; set; }//
        public string price { get; set; }
        public string mainType { get; set; }//Accessory</mainType{ get; set; }//

    }
    public class companyInfo
    {
      public string COMPANYROLE{ get; set; }//CB</COMPANYROLE{ get; set; }//
      public string IDENTIFICATION_CODE{ get; set; }//3</IDENTIFICATION_CODE{ get; set; }//
      public string annualRevenue{ get; set; }//0</annualRevenue{ get; set; }//
      public string applicationNo{ get; set; }//Br-A044180000</applicationNo{ get; set; }//
      public string businesstypeCode{ get; set; }//00006</businesstypeCode{ get; set; }//
      public string businesstypeName{ get; set; }//合伙企业 Partnership</businesstypeName{ get; set; }//
      public string capitalReamount{ get; set; }//5000000</capitalReamount{ get; set; }//
      public string companyName{ get; set; }//chines name</companyName{ get; set; }//
      public string emailAddress { get; set; }//
      public string taxid { get; set; }//
      public string compaynameeng { get; set; }//
      public string establishedin { get; set; }//
      public string industrysubtypeCode{ get; set; }//00002</industrysubtypeCode{ get; set; }//
      public string industrysubtypeName{ get; set; }//燃气生产和供应业 Production and Distribution of Gas</industrysubtypeName{ get; set; }//
      public string industrytypeCode{ get; set; }//00004</industrytypeCode{ get; set; }//
      public string industrytypeName{ get; set; }//电力、燃气及水的生产和供应业</industrytypeName{ get; set; }//
      public string loanCard { get; set; }//
      public string loancardPassword { get; set; }//
      public string networthamt{ get; set; }//0</networthamt{ get; set; }//
      public string organizationCode{ get; set; }//124521554545454545</organizationCode{ get; set; }//
      public string parentCompany{ get; set; }//parent company</parentCompany{ get; set; }//
      public string registrationNo{ get; set; }//11111111111111111111</registrationNo{ get; set; }//
      public string representativeName { get; set; }//
      public string representativeidNo { get; set; }//
      public string staffNumber { get; set; }//
      public string subsidaryCompany { get; set; }//
      public string trustName { get; set; }//
      public string years { get; set; }//
      public string representativeDesignation { get; set; }//
      public string comments { get; set; }//
      public string externalCreditRecord{ get; set; }//F</externalCreditRecord{ get; set; }//
      public string noRecord{ get; set; }//T</noRecord{ get; set; }//
      public string lienee{ get; set; }//F</lienee{ get; set; }//
                                   }
    public class applicantInfo
    {
      public string APPLICATION_NUMBER{ get; set; }//Br-A044180000</APPLICATION_NUMBER{ get; set; }//
      public string IDENTIFICATION_CODE{ get; set; }//1</IDENTIFICATION_CODE{ get; set; }//
      public string APPLICANTROLE{ get; set; }//B</APPLICANTROLE{ get; set; }//
      public string idcardtypeCode{ get; set; }//00001</idcardtypeCode{ get; set; }//
      public string idcardtypeName{ get; set; }//居民身份证</idcardtypeName{ get; set; }//
      public string idcardnumber{ get; set; }//440184198411090612</idcardnumber{ get; set; }//
      public string customerid{ get; set; }//0</customerid{ get; set; }//
      public string idcardissuedate { get; set; }//
      public string idcardexpirydate { get; set; }//
      public string titlecode{ get; set; }//00001</titlecode{ get; set; }//
      public string titlename{ get; set; }//Mr.</titlename{ get; set; }//
      public string thaititlecode{ get; set; }//00001</thaititlecode{ get; set; }//
      public string thaititlename{ get; set; }//先生</thaititlename{ get; set; }//
      public string ThaiFirstName { get; set; }//汤子文</customerName{ get; set; }//
      public string thaimiddlename { get; set; }//
      public string thailastname { get; set; }//
      public string engfirstname { get; set; }//
      public string engmiddlename { get; set; }//
      public string englastname { get; set; }//
      public string maritalstatuscode{ get; set; }//00003</maritalstatuscode{ get; set; }//
      public string maritalstatusname{ get; set; }//离异</maritalstatusname{ get; set; }//
      public string childrenflag{ get; set; }//N</childrenflag{ get; set; }//
      public string numberofdependents { get; set; }//
      public string occupationCode{ get; set; }//00003</occupationCode{ get; set; }//
      public string occupationName{ get; set; }//办事人员和有关人员</occupationName{ get; set; }//
      public string occupationType{ get; set; }//O</occupationType{ get; set; }//
      public string citizenshipCode{ get; set; }//00001</citizenshipCode{ get; set; }//
      public string citizenshipName{ get; set; }//中国 Chinese</citizenshipName{ get; set; }//
      public string licenseNumber { get; set; }//
      public string licenseexpirydate { get; set; }//
      public string dateofbirth{ get; set; }//11/9/1984 12:00:00 AM</dateofbirth{ get; set; }//
      public string ageInYear{ get; set; }//32</ageInYear{ get; set; }//
      public string ageInMonth{ get; set; }//10</ageInMonth{ get; set; }//
      public string ageInYYMM{ get; set; }//3210</ageInYYMM{ get; set; }//
      public string employeetypeCode{ get; set; }//00005</employeetypeCode{ get; set; }//
      public string employeetypeName{ get; set; }//受薪人士</employeetypeName{ get; set; }//
      public string timeasCustomer { get; set; }//
      public string genderCode{ get; set; }//M</genderCode{ get; set; }//
      public string genderName{ get; set; }//男</genderName{ get; set; }//
      public string industrytypeCode{ get; set; }//00015</industrytypeCode{ get; set; }//
      public string industrytypeName{ get; set; }//居民服务和其他服务业</industrytypeName{ get; set; }//
      public string IndustrySubTypeCode { get; set; }//
      public string IndustrySubTypeName { get; set; }//
      public string emailaddress{ get; set; }//珠海绫致商贸有限公司</emailaddress{ get; set; }//
      public string tradingas { get; set; }//
      public string educationCode{ get; set; }//00002</educationCode{ get; set; }//
      public string educationName{ get; set; }//大学本科</educationName{ get; set; }//
      public string relationShipCode { get; set; }//
      public string relationShipName { get; set; }//
      public string bankstatementind { get; set; }//
      public string lettercertifyind { get; set; }//
      public string salayslipind { get; set; }//
      public string consent{ get; set; }//N</consent{ get; set; }//
      public string staffind{ get; set; }//F</staffind{ get; set; }//
      public string vipind{ get; set; }//F</vipind{ get; set; }//
      public string blacklistdurationmonthNo { get; set; }//
      public string blacklistind{ get; set; }//F</blacklistind{ get; set; }//
      public string blacklistnorecordind{ get; set; }//T</blacklistnorecordind{ get; set; }//
      public string blacklistcontractind { get; set; }//
      public string hukouCode{ get; set; }//00005</hukouCode{ get; set; }//
      public string hukouName{ get; set; }//非本地户口</hukouName{ get; set; }//
      public string nooffamilymembers { get; set; }//
      public string familyavgincome { get; set; }//
      public string houseownercode { get; set; }//
      public string houseownername { get; set; }//
      public string subOccupationCode{ get; set; }//00044</subOccupationCode{ get; set; }//
      public string subOccupationName{ get; set; }//默认值</subOccupationName{ get; set; }//
      public string designationCode{ get; set; }//00001</designationCode{ get; set; }//
      public string desginationName{ get; set; }//高级领导</desginationName{ get; set; }//
      public string jobgroupCode{ get; set; }//00002</jobgroupCode{ get; set; }//
      public string jobgroupName{ get; set; }//高级</jobgroupName{ get; set; }//
      public string salaryrangeCode{ get; set; }//00001</salaryrangeCode{ get; set; }//
      public string salaryrangeName{ get; set; }//&lt;= 30,000</salaryrangeName{ get; set; }//
      public string actualsalary{ get; set; }//30000</actualsalary{ get; set; }//
      public string drivinglicensestatusCode{ get; set; }//00001</drivinglicensestatusCode{ get; set; }//
      public string drivinglicensestatusName{ get; set; }//本人或直系亲属有驾照</drivinglicensestatusName{ get; set; }//
      public string formername { get; set; }//
      public string nationCode{ get; set; }//0001</nationCode{ get; set; }//
      public string nationName{ get; set; }//汉族</nationName{ get; set; }//
      public string issuingAuthority{ get; set; }//广州市公安局罗岗萝岗分局</issuingAuthority{ get; set; }//
      public string lienee{ get; set; }//T</lienee{ get; set; }//
      public string spouse { get; set; } 
    }
    public class employment
    {
      public string IDENTIFICATION_CODE{ get; set; }//1</IDENTIFICATION_CODE{ get; set; }//
      public string companyAddress{ get; set; }//珠海市香洲区翠微西路12栋2单元</companyAddress{ get; set; }//
      public string companyName{ get; set; }//珠海绫致商贸有限公司</companyName{ get; set; }//
      public string lineId{ get; set; }//1</lineId{ get; set; }//
      public string phonenumber { get; set; }//
      public string Fax { get; set; }//
      public string provinceCode{ get; set; }//00018</provinceCode{ get; set; }//
      public string provinceName{ get; set; }//广东</provinceName{ get; set; }//
      public string cityCode{ get; set; }//00200</cityCode{ get; set; }//
      public string cityName{ get; set; }//珠海</cityName{ get; set; }//
      public string positionCode { get; set; }//
      public string positionName { get; set; }//
      public string postCode { get; set; }//
      public string timeinmonth{ get; set; }//6</timeinmonth{ get; set; }//
      public string timeinyear{ get; set; }//2</timeinyear{ get; set; }//
      public string comments { get; set; }//
      public string jobdescription { get; set; }//
      public string employertype { get; set; }//
      public string noofemployees { get; set; }//
      public string businesstypeCode{ get; set; }//00010</businesstypeCode{ get; set; }//
      public string businesstypeName{ get; set; }//私营有限责任公司 Private limited company</businesstypeName{ get; set; }//
    }
    public class referenceList
    {
      public string IDENTIFICATION_CODE{ get; set; }//2</IDENTIFICATION_CODE{ get; set; }//
      public string address { get; set; }//
      public string cityCode{ get; set; }//00314</cityCode{ get; set; }//
      public string cityName{ get; set; }//海北</cityName{ get; set; }//
      public string lineId{ get; set; }//1</lineId{ get; set; }//
      public string mobile{ get; set; }//</mobile{ get; set; }//
      public string name{ get; set; }//personal</name{ get; set; }//
      public string phoneNo { get; set; }//
      public string postCode{ get; set; }//12547</postCode{ get; set; }//
      public string provinceCode{ get; set; }//00028</provinceCode{ get; set; }//
      public string provinceName{ get; set; }//青海</provinceName{ get; set; }//
      public string relationshipCode{ get; set; }//00109</relationshipCode{ get; set; }//
      public string relationshipName{ get; set; }//Primary BP-Third Party Individual主要业务合伙人-个人第三方</relationshipName{ get; set; }//
      public string hokouCode{ get; set; }//00004</hokouCode{ get; set; }//
      public string hokouName{ get; set; }//本地户口</hokouName{ get; set; }//
}
    public class addressList
    {
      public string IDENTIFICATION_CODE{ get; set; }//1</IDENTIFICATION_CODE{ get; set; }//
      public string addressCode{ get; set; }//1</addressCode{ get; set; }//
      public string AddressId{ get; set; }//广州市黄埔区埔心大秧地街十二巷4号</AddressId{ get; set; }//
      public string addressstatusCode{ get; set; }//C</addressstatusCode{ get; set; }//
      public string addressstatusName{ get; set; }//Current</addressstatusName{ get; set; }//
      public string addresstypeCode{ get; set; }//00002</addresstypeCode{ get; set; }//
      public string addresstypeName{ get; set; }//家庭地址 Home</addresstypeName{ get; set; }//
      public string cityCode{ get; set; }//00200</cityCode{ get; set; }//
      public string cityName{ get; set; }//珠海</cityName{ get; set; }//
      public string currentLivingAddress{ get; set; }//珠海市香洲区翠微东路288号2栋3005房</currentLivingAddress{ get; set; }//
      public string countryCode{ get; set; }//00086</countryCode{ get; set; }//
      public string countryName{ get; set; }//中国</countryName{ get; set; }//
      public string defaultmailingaddress{ get; set; }//Y</defaultmailingaddress{ get; set; }//
      public string hukouaddress{ get; set; }//N</hukouaddress{ get; set; }//
      public string postcode{ get; set; }//519000</postcode{ get; set; }//
      public string propertytypeCode{ get; set; }//00001</propertytypeCode{ get; set; }//
      public string propertytypeName{ get; set; }//自置</propertytypeName{ get; set; }//
      public string residencetypeCode { get; set; }//
      public string residencetypeName { get; set; }//
      public string provinceCode{ get; set; }//00018</provinceCode{ get; set; }//
      public string provinceName{ get; set; }//广东</provinceName{ get; set; }//
      public string homevalue { get; set; }//
      public string livingsince { get; set; }//
      public string stayinyear { get; set; }//
      public string stayinmonth { get; set; }//
      public string nativedistrict{ get; set; }//广州市</nativedistrict{ get; set; }//
      public string birthpalaceprovince{ get; set; }//广州市白云区</birthpalaceprovince{ get; set; }//
      public string registeredAddress { get; set; }//广州市白云区</birthpalaceprovince{ get; set; }//
        
    }
    public class phoneList
    {
      public string IDENTIFICATION_CODE{ get; set; }//1</IDENTIFICATION_CODE{ get; set; }//
      public string addressCode{ get; set; }//1</addressCode{ get; set; }//
      public string areaCode { get; set; }//
      public string countrycodeCode{ get; set; }//86</countrycodeCode{ get; set; }//
      public string countrycodeName { get; set; }//
      public string extension { get; set; }//
      public string phoneNo{ get; set; }//15220500098</phoneNo{ get; set; }//
      public string phoneSeqId{ get; set; }//1</phoneSeqId{ get; set; }//
      public string phonetypeCode{ get; set; }//00003</phonetypeCode{ get; set; }//
      public string phonetypeName{ get; set; }//手机 Mobile Phone</phonetypeName{ get; set; }//
}
}