<%@ Page Language="C#" AutoEventWireup="true" CodeFile="EnterNet.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.EnterNet.EnterNet" EnableEventValidation="false" MasterPageFile="~/MvcSheetEnterNet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <script src="/Portal/WFRes/_Scripts/PriintEnterMortgage.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>"></script>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <!--<div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">入网流程</label>
    </div>-->
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-right bannerTitle">
            <label id="divBasicInfo" data-en_us="Basic information" class="">基本信息</label>
        </div>
        <div class="divContent">
            <div class="row">
                <div id="divFullNameTitle" class="col-md-2">
                    <label id="lblFullNameTitle" data-type="SheetLabel" data-datafield="Originator.UserName" data-en_us="Originator" data-bindtype="OnlyVisiable" style="">发起人</label>
                </div>
                <div id="divFullName" class="col-md-4">
                    <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style=""></label>
                </div>
                <div id="divOriginateTimeTitle" class="col-md-2">
                    <label id="lblOriginateTimeTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                </div>
                <div id="divOriginateTime" class="col-md-4">
                    <label id="lblOriginateTime" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style=""></label>
                </div>
            </div>
            <div class="row">
                <div id="divOriginateOUNameTitle" class="col-md-2">
                    <label id="lblOriginateOUNameTitle" data-type="SheetLabel" data-datafield="Originator.OUName" data-en_us="Originate OUName" data-bindtype="OnlyVisiable" style="">所属组织</label>
                </div>
                <div id="divOriginateOUName" class="col-md-4">
                    <!--					<label id="lblOriginateOUName" data-type="SheetLabel" data-datafield="Originator.OUName" data-bindtype="OnlyData">
                    <span class="OnlyDesigner">Originator.OUName</span>
                                        </label>-->
                    <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style=""></select>
                </div>
                <div id="divSequenceNoTitle" class="col-md-2">
                    <label id="lblSequenceNoTitle" data-type="SheetLabel" data-datafield="SequenceNo" data-en_us="SequenceNo" data-bindtype="OnlyVisiable" style="">流水号</label>
                </div>
                <div id="divSequenceNo" class="col-md-4">
                    <label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style=""></label>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
        </div>
        <div class="divContent" id="divSheet">
            <div class="row">
                <div id="title1" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="Manager" style="">区域负责人</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input id="Control11" type="text" data-datafield="Manager" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="Distributor" style="">经销商名称</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="Distributor" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="Type" style="">渠道分类</span>
                </div>
                <div id="control3" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="Type" data-type="SheetTextBox" style="">
                </div>
                <div id="title4" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="DistributorType" style="">经销商分类</span>
                </div>
                <div id="control4" class="col-md-4">
                    <input id="Control14" type="text" data-datafield="DistributorType" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="Province" style="">省份</span>
                </div>
                <div id="control5" class="col-md-4">
                    <input id="Control15" type="text" data-datafield="Province" data-type="SheetTextBox" style="">
                </div>
                <div id="title6" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="City" style="">城市</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control16" type="text" data-datafield="City" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="div789241" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="CompanyAddr" style="" class="">公司地址</span>
                </div>
                <div id="div180859" class="col-md-10">
                    <input id="Control17" type="text" data-datafield="CompanyAddr" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="div251674" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="BelongTo" style="" class="">所属集团或平台</span>
                </div>
                <div id="div654807" class="col-md-4">
                    <input type="text" data-datafield="BelongTo" data-type="SheetTextBox" id="ctl630413" class="" style="">
                </div>
                <div id="div606452" class="col-md-2">
                    <span id="Label19" data-type="SheetLabel" data-datafield="Brand" style="" class="">经营品牌</span>
                </div>
                <div id="div936682" class="col-md-4">
                    <input id="Control19" type="text" data-datafield="Brand" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title11" class="col-md-2">
                    <span id="Label20" data-type="SheetLabel" data-datafield="QYWYKT" style="" class="">企业网银开通</span>
                </div>
                <div id="control11" class="col-md-4">
                    <input id="Control20" type="text" data-datafield="QYWYKT" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="space12" class="col-md-2">
                    <span id="Label21" data-type="SheetLabel" data-datafield="LoanType" style="" class="">贷款模式</span>
                </div>
                <div id="spaceControl12" class="col-md-4">
                    <input id="Control21" type="text" data-datafield="LoanType" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row tableContent">
                <div id="title13" class="col-md-2">
                    <span id="Label22" data-type="SheetLabel" data-datafield="Memo" style="">备注</span>
                </div>
                <div id="control13" class="col-md-10">
                    <textarea id="Control22" data-datafield="Memo" data-type="SheetRichTextBox" style="" class="">					</textarea>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">营业执照信息</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row">
                    <div id="title15" class="col-md-2">
                        <span id="Label23" data-type="SheetLabel" data-datafield="License" style="">营业执照</span>
                    </div>
                    <div id="control15" class="col-md-4">
                        <input id="Control23" type="text" data-datafield="License" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title16" class="col-md-2">
                        <span id="Label24" data-type="SheetLabel" data-datafield="EnterpriseRegistration" style="">企业登记号</span>
                    </div>
                    <div id="control16" class="col-md-4">
                        <input id="Control24" type="text" data-datafield="EnterpriseRegistration" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title17" class="col-md-2">
                        <span id="Label25" data-type="SheetLabel" data-datafield="RegistrationDate" style="" class="">登记日期</span>
                    </div>
                    <div id="control17" class="col-md-4">
                        <input id="Control25" type="text" data-datafield="RegistrationDate" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title18" class="col-md-2">
                        <span id="Label26" data-type="SheetLabel" data-datafield="CreatDate" style="">创立日期</span>
                    </div>
                    <div id="control18" class="col-md-4">
                        <input id="Control26" type="text" data-datafield="CreatDate" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title19" class="col-md-2">
                        <span id="Label27" data-type="SheetLabel" data-datafield="LegalRepresentative" style="">法定代表人</span>
                    </div>
                    <div id="control19" class="col-md-4">
                        <input id="Control27" type="text" data-datafield="LegalRepresentative" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title20" class="col-md-2">
                        <span id="Label28" data-type="SheetLabel" data-datafield="CorporateIdentityCard" style="">法人身份证号</span>
                    </div>
                    <div id="control20" class="col-md-4">
                        <input id="Control28" type="text" data-datafield="CorporateIdentityCard" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title21" class="col-md-2">
                        <span id="Label29" data-type="SheetLabel" data-datafield="RegisteredCapital" style="">注册资金</span>
                    </div>
                    <div id="control21" class="col-md-4">
                        <input id="Control29" type="text" data-datafield="RegisteredCapital" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title22" class="col-md-2">
                    </div>
                    <div id="control22" class="col-md-4">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">银行开户信息</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row">
                    <div id="div669824" class="col-md-2">
                        <span id="Label30" data-type="SheetLabel" data-datafield="BankBranch" style="" class="">银行开户分行</span>
                    </div>
                    <div id="div30695" class="col-md-4">
                        <input id="Control30" type="text" data-datafield="BankBranch" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="div642770" class="col-md-2">
                        <span id="Label31" data-type="SheetLabel" data-datafield="AccountType" style="" class="">银行账户类型</span>
                    </div>
                    <div id="div124917" class="col-md-4">
                        <input id="Control31" type="text" data-datafield="AccountType" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title23" class="col-md-2">
                        <span id="Label32" data-type="SheetLabel" data-datafield="BankName" style="" class="">银行账户名</span>
                    </div>
                    <div id="control23" class="col-md-4">
                        <input id="Control32" type="text" data-datafield="BankName" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title24" class="col-md-2">
                        <span id="Label33" data-type="SheetLabel" data-datafield="BankAccount" style="" class="">银行账号</span>
                    </div>
                    <div id="control24" class="col-md-4">
                        <input id="Control33" type="text" data-datafield="BankAccount" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title25" class="col-md-2">
                        <span id="Label34" data-type="SheetLabel" data-datafield="CoupletNumber" style="" class="">联行号</span>
                    </div>
                    <div id="control25" class="col-md-4">
                        <input id="Control34" type="text" data-datafield="CoupletNumber" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title26" class="col-md-2" style="display: none;">
                    </div>
                    <div id="control26" class="col-md-4" style="display: none;">
                        <input type="text" data-datafield="CrmDealerId" data-type="SheetTextBox" id="crmDealerId" >
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">联系方式</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row tableContent">
                    <div id="title27" class="col-md-2">
                        <span id="Label35" data-type="SheetLabel" data-datafield="ContactInformation" style="">联系方式</span>
                    </div>
                    <div id="control27" class="col-md-10">
                        <table id="Control35" data-datafield="ContactInformation" data-type="SheetGridView" class="SheetGridView">
                            <tbody>

                                <tr class="header">
                                    <td id="Control35_SerialNo" class="rowSerialNo">
                                        序号
                                    </td>
                                    <td id="Control35_Header3" data-datafield="ContactInformation.Department">
                                        <label id="Control35_Label3" data-datafield="ContactInformation.Department" data-type="SheetLabel" style="">部门</label>
                                    </td>
                                    <td id="Control35_Header4" data-datafield="ContactInformation.People">
                                        <label id="Control35_Label4" data-datafield="ContactInformation.People" data-type="SheetLabel" style="">联系人</label>
                                    </td>
                                    <td id="Control35_Header5" data-datafield="ContactInformation.Tel">
                                        <label id="Control35_Label5" data-datafield="ContactInformation.Tel" data-type="SheetLabel" style="">电话</label>
                                    </td>
                                    <td id="Control35_Header6" data-datafield="ContactInformation.Maile">
                                        <label id="Control35_Label6" data-datafield="ContactInformation.Maile" data-type="SheetLabel" style="">邮箱</label>
                                    </td>
                                    <td class="rowOption">
                                        删除
                                    </td>
                                </tr>
                                <tr class="template">
                                    <td id="Control35_Option" class="rowOption"></td>
                                    <td data-datafield="ContactInformation.Department">
                                        <input id="Control35_ctl3" type="text" data-datafield="ContactInformation.Department" data-type="SheetTextBox" style="">
                                    </td>
                                    <td data-datafield="ContactInformation.People">
                                        <input id="Control35_ctl4" type="text" data-datafield="ContactInformation.People" data-type="SheetTextBox" style="">
                                    </td>
                                    <td data-datafield="ContactInformation.Tel">
                                        <input id="Control35_ctl5" type="text" data-datafield="ContactInformation.Tel" data-type="SheetTextBox" style="">
                                    </td>
                                    <td data-datafield="ContactInformation.Maile">
                                        <input id="Control35_ctl6" type="text" data-datafield="ContactInformation.Maile" data-type="SheetTextBox" style="">
                                    </td>
                                    <td class="rowOption">
                                        <a class="delete">
                                            <div class="fa fa-minus">
                                            </div>
                                        </a>
                                        <a class="insert">
                                            <div class="fa fa-arrow-down">
                                            </div>
                                        </a>
                                    </td>
                                </tr>
                                <tr class="footer">
                                    <td class="rowOption"></td>
                                    <td data-datafield="ContactInformation.Department"></td>
                                    <td data-datafield="ContactInformation.People"></td>
                                    <td data-datafield="ContactInformation.Tel"></td>
                                    <td data-datafield="ContactInformation.Maile"></td>
                                    <td class="rowOption"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">其他信息</label>
            </div>
            <div class="divContent" id="divSheetOther">
                <div class="row">
                    <div id="title29" class="col-md-2">
                        <span id="Label36" data-type="SheetLabel" data-datafield="GPSInstallation" style="">GPS安装</span>
                    </div>
                    <div id="control29" class="col-md-4">
                        <input id="Control36" type="text" data-datafield="GPSInstallation" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title30" class="col-md-2">
                        <span id="Label37" data-type="SheetLabel" data-datafield="SystemScore" style="">系统评分</span>
                    </div>
                    <div id="control30" class="col-md-4">
                        <input id="Control37" type="text"  disabled="" data-datafield="SystemScore" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="div786932" class="col-md-2">
                        <span id="Label38" data-type="SheetLabel" data-datafield="GPSAddr" style="" class="">GPS地址</span>
                    </div>
                    <div id="div942718" class="col-md-10">
                        <input id="Control38" type="text" data-datafield="GPSAddr" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title31" class="col-md-2">
                        <span id="Label39" data-type="SheetLabel" data-datafield="GPSAccount" style="" class="">GPS账号</span>
                    </div>
                    <div id="control31" class="col-md-4">
                        <input id="Control39" type="text" data-datafield="GPSAccount" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title32" class="col-md-2">
                        <span id="Label40" data-type="SheetLabel" data-datafield="GPSPassword" style="" class="">GPS密码</span>
                    </div>
                    <div id="control32" class="col-md-4">
                        <input id="Control40" type="text" data-datafield="GPSPassword" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">公司概况</label>
            </div>
            <div class="divContent" id="divSheetCompany">
                <div class="row">
                    <div id="title85" class="col-md-2">
                        <span id="Label81" data-type="SheetLabel" data-datafield="Total" style="" class="">店内员工数</span>
                    </div>
                    <div id="control85" class="col-md-4">
                        <input id="Control81" type="text" data-datafield="Total" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title86" class="col-md-2">
                        <span id="Label82" data-type="SheetLabel" data-datafield="ShareRatio" style="">实际法人股权占比</span>
                    </div>
                    <div id="control86" class="col-md-4">
                        <input id="Control82" type="text" data-datafield="ShareRatio" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title87" class="col-md-2">
                        <span id="Label83" data-type="SheetLabel" data-datafield="Area" style="">经销商营业面积</span>
                    </div>
                    <div id="control87" class="col-md-4">
                        <input id="Control83" type="text" data-datafield="Area" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title88" class="col-md-2">
                        <span id="Label84" data-type="SheetLabel" data-datafield="FinancialDepartment" style="">财务部门</span>
                    </div>
                    <div id="control88" class="col-md-4">
                        <input id="Control84" type="text" data-datafield="FinancialDepartment" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title89" class="col-md-2">
                        <span id="Label85" data-type="SheetLabel" data-datafield="MortgageConfirmation" style="">抵押确认(当地/我司)</span>
                    </div>
                    <div id="control89" class="col-md-4">
                        <input id="Control85" type="text" data-datafield="MortgageConfirmation" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title90" class="col-md-2">
                        <span id="Label86" data-type="SheetLabel" data-datafield="XSQK" style="">经销商近6个月月均车辆销售情况</span>
                    </div>
                    <div id="control90" class="col-md-4">
                        <input id="Control86" type="text" data-datafield="XSQK" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title91" class="col-md-2">
                        <span id="Label87" data-type="SheetLabel" data-datafield="ZXQK" style="">法院被执行情况(企业及个人)</span>
                    </div>
                    <div id="control91" class="col-md-4">
                        <input id="Control87" type="text" data-datafield="ZXQK" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title92" class="col-md-2">
                        <span id="Label88" data-type="SheetLabel" data-datafield="SXQK" style="">法院失信情况(企业及个人)</span>
                    </div>
                    <div id="control92" class="col-md-4">
                        <input id="Control88" type="text" data-datafield="SXQK" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title93" class="col-md-2">
                        <span id="Label89" data-type="SheetLabel" data-datafield="BusinessLife" style="">公司经营年限</span>
                    </div>
                    <div id="control93" class="col-md-4">
                        <input id="Control89" type="text" data-datafield="BusinessLife" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title94" class="col-md-2">
                        <span id="Label90" data-type="SheetLabel" data-datafield="Seniority" style="">实控人行业资历</span>
                    </div>
                    <div id="control94" class="col-md-4">
                        <input id="Control90" type="text" data-datafield="Seniority" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title95" class="col-md-2">
                        <span id="Label91" data-type="SheetLabel" data-datafield="Administrative" style="" class="">办公区</span>
                    </div>
                    <div id="control95" class="col-md-4">

                        <select data-datafield="Administrative" data-type="SheetDropDownList" id="ctl960631" class="" style="" data-defaultitems="自有;租借"></select>
                    </div>
                    <div id="title96" class="col-md-2">
                        <span id="Label92" data-type="SheetLabel" data-datafield="Lease" style="">融资租赁业务</span>
                    </div>
                    <div id="control96" class="col-md-4">
                        <input id="Control92" type="text" data-datafield="Lease" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title97" class="col-md-2">
                        <span id="Label93" data-type="SheetLabel" data-datafield="SixBankFlow" style="">经销商6个月银行流水</span>
                    </div>
                    <div id="control97" class="col-md-4">
                        <input id="Control93" type="text" data-datafield="SixBankFlow" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title98" class="col-md-2">
                        <span id="Label94" data-type="SheetLabel" data-datafield="Profits" style="">平均单台净利润</span>
                    </div>
                    <div id="control98" class="col-md-4">
                        <input id="Control94" type="text" data-datafield="Profits" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title99" class="col-md-2">
                        <span id="Label95" data-type="SheetLabel" data-datafield="IsBank" style="">是否为集团性质或有关联企业</span>
                    </div>
                    <div id="control99" class="col-md-4">
                        <input id="Control95" type="text" data-datafield="IsBank" data-type="SheetTextBox" style="">
                    </div>
                    <div id="space100" class="col-md-2">
                        <label data-datafield="FinancialCount" data-type="SheetLabel" id="ctl987688" class="" style="">金融专员人数</label>
                    </div>
                    <div id="spaceControl100" class="col-md-4">
                        <input type="text" data-datafield="FinancialCount" data-type="SheetTextBox" id="ctl632511" class="" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="div168005" class="col-md-2">
                        <label data-datafield="SalesCount" data-type="SheetLabel" id="ctl280162" class="" style="">销售顾问人数</label>
                    </div>
                    <div id="div415632" class="col-md-4">
                        <input type="text" data-datafield="SalesCount" data-type="SheetTextBox" id="ctl252903" class="" style="">
                    </div>
                    <div id="div746214" class="col-md-2">
                        <label data-datafield="AdvanceLoan" data-type="SheetLabel" id="ctl589099" class="" style="">提前放款额度</label>
                    </div>
                    <div id="div303469" class="col-md-4">
                        <input type="text" data-datafield="AdvanceLoan" data-type="SheetTextBox" id="ctl638452" class="" style="">
                    </div>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">附件</label>
            </div>
            <div class="divContent" id="divattachment">
                <div class="row">
                    <div id="title33" class="col-md-2">
                        <span id="Label41" data-type="SheetLabel" data-datafield="ApprovalForm" style="" class="">准入审批表</span>
                    </div>
                    <div id="control33" class="col-md-4">
                        <input id="Control41" type="text" data-hreflink="" data-datafield="ApprovalForm" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title34" class="col-md-2">
                        <span id="Label42" data-type="SheetLabel" data-datafield="CollectionTable" style="" class="">信息采集表</span>
                    </div>
                    <div id="control34" class="col-md-4">
                        <input id="Control42" type="text" data-hreflink="" data-datafield="CollectionTable" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title35" class="col-md-2">
                        <span id="Label43" data-type="SheetLabel" data-datafield="BusinessLicense" style="" class="">营业执照复印件</span>
                    </div>
                    <div id="control35" class="col-md-4">
                        <input id="Control43" type="text" data-hreflink="" data-datafield="BusinessLicense" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title36" class="col-md-2">
                        <span id="Label44" data-type="SheetLabel" data-datafield="CreditDocuments" style="" class="">征信/合作文件</span>
                    </div>
                    <div id="control36" class="col-md-4">
                        <input id="Control44" type="text" data-hreflink="" data-datafield="CreditDocuments" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title37" class="col-md-2">
                        <span id="Label45" data-type="SheetLabel" data-datafield="AuthorizationDocument" style="" class="">厂商授权文件</span>
                    </div>
                    <div id="control37" class="col-md-4">
                        <input id="Control45" type="text" data-hreflink="" data-datafield="AuthorizationDocument" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title38" class="col-md-2">
                        <span id="Label46" data-type="SheetLabel" data-datafield="Photo" style="" class="">门头照片</span>
                    </div>
                    <div id="control38" class="col-md-4">
                        <input id="Control46" type="text" data-hreflink="" data-datafield="Photo" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title39" class="col-md-2">
                        <span id="Label47" data-type="SheetLabel" data-datafield="DiscountAgreement" style="" class="">贴息协议</span>
                    </div>
                    <div id="control39" class="col-md-4">
                        <input id="Control47" type="text" data-hreflink="" data-datafield="DiscountAgreement" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title40" class="col-md-2">
                        <span id="Label48" data-type="SheetLabel" data-datafield="CargoAgreement" style="" class="">附加货协议</span>
                    </div>
                    <div id="control40" class="col-md-4">
                        <input id="Control48" type="text" data-hreflink="" data-datafield="CargoAgreement" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title41" class="col-md-2">
                        <span id="Label49" data-type="SheetLabel" data-datafield="HallPhoto" style="" class="">展厅照片</span>
                    </div>
                    <div id="control41" class="col-md-4">
                        <input id="Control49" type="text" data-hreflink="" data-datafield="HallPhoto" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title42" class="col-md-2">
                        <span id="Label50" data-type="SheetLabel" data-datafield="OfficePhoto" style="" class="">办公区照片</span>
                    </div>
                    <div id="control42" class="col-md-4">
                        <input id="Control50" type="text" data-hreflink="" data-datafield="OfficePhoto" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title43" class="col-md-2">
                        <span id="Label51" data-type="SheetLabel" data-datafield="BusinessInformation" style="" class="">企业工商信息</span>
                    </div>
                    <div id="control43" class="col-md-4">
                        <input id="Control51" type="text" data-hreflink="" data-datafield="BusinessInformation" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title44" class="col-md-2">
                        <span id="Label52" data-type="SheetLabel" data-datafield="FinancialInformation" style="" class="">金融专员资料</span>
                    </div>
                    <div id="control44" class="col-md-4">
                        <input id="Control52" type="text" data-hreflink="" data-datafield="FinancialInformation" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title45" class="col-md-2">
                        <span id="Label53" data-type="SheetLabel" data-datafield="FinancialId" style="" class="">金融专员身份证复印件</span>
                    </div>
                    <div id="control45" class="col-md-4">
                        <input id="Control53" type="text" data-hreflink="" data-datafield="FinancialId" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title46" class="col-md-2">
                        <span id="Label54" data-type="SheetLabel" data-datafield="CooperationAgreement" style="" class="">合作协议</span>
                    </div>
                    <div id="control46" class="col-md-4">
                        <input id="Control54" type="text" data-hreflink="" data-datafield="CooperationAgreement" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title47" class="col-md-2">
                        <span id="Label55" data-type="SheetLabel" data-datafield="RepresentativeId" style="" class="">代表/实控人(身份证复印件)</span>
                    </div>
                    <div id="control47" class="col-md-4">
                        <input id="Control55" type="text" data-hreflink="" data-datafield="RepresentativeId" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title48" class="col-md-2">
                        <span id="Label56" data-type="SheetLabel" data-datafield="PersonnelPhoto" style="" class="">人员合照</span>
                    </div>
                    <div id="control48" class="col-md-4">
                        <input id="Control56" type="text" data-hreflink="" data-datafield="PersonnelPhoto" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title49" class="col-md-2">
                        <span id="Label57" data-type="SheetLabel" data-datafield="Report" style="" class="">尽调报告</span>
                    </div>
                    <div id="control49" class="col-md-4">
                        <input id="Control57" type="text" data-hreflink="" data-datafield="Report" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title50" class="col-md-2">
                        <span id="Label58" data-type="SheetLabel" data-datafield="SiteContract" style="" class="">场地租赁合同</span>
                    </div>
                    <div id="control50" class="col-md-4">
                        <input id="Control58" type="text" data-hreflink="" data-datafield="SiteContract" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title51" class="col-md-2">
                        <span id="Label59" data-type="SheetLabel" data-datafield="XFHDSPB" style="" class="">先放后抵审批表</span>
                    </div>
                    <div id="control51" class="col-md-4">
                        <input id="Control59" type="text" data-hreflink="" data-datafield="XFHDSPB" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title52" class="col-md-2">
                        <span id="Label60" data-type="SheetLabel" data-datafield="QuotaForm" style="" class="">额度申请表</span>
                    </div>
                    <div id="control52" class="col-md-4">
                        <input id="Control60" type="text" data-hreflink="" data-datafield="QuotaForm" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title53" class="col-md-2">
                        <span id="Label61" data-type="SheetLabel" data-datafield="SalesData" style="" class="">销量+放款额数据</span>
                    </div>
                    <div id="control53" class="col-md-4">
                        <input id="Control61" type="text" data-hreflink="" data-datafield="SalesData" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title54" class="col-md-2">
                        <span id="Label62" data-type="SheetLabel" data-datafield="FinancialInstitutions" style="" class="">合作金融机构(名单)</span>
                    </div>
                    <div id="control54" class="col-md-4">
                        <input id="Control62" type="text" data-hreflink="" data-datafield="FinancialInstitutions" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title55" class="col-md-2">
                        <span id="Label63" data-type="SheetLabel" data-datafield="BankFlow" style="" class="">公司银行流水(近3个月)</span>
                    </div>
                    <div id="control55" class="col-md-4">
                        <input id="Control63" type="text" data-hreflink="" data-datafield="BankFlow" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title56" class="col-md-2">
                        <span id="Label64" data-type="SheetLabel" data-datafield="PersonalInformation" style="" class="">个人资信情况(法人/实控人)</span>
                    </div>
                    <div id="control56" class="col-md-4">
                        <input id="Control64" type="text" data-hreflink="" data-datafield="PersonalInformation" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title57" class="col-md-2">
                        <span id="Label65" data-type="SheetLabel" data-datafield="SecurityAgreement" style="" class="">担保协议</span>
                    </div>
                    <div id="control57" class="col-md-4">
                        <input id="Control65" type="text" data-hreflink="" data-datafield="SecurityAgreement" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title58" class="col-md-2">
                        <span id="Label66" data-type="SheetLabel" data-datafield="CreditReport" style="" class="">征信报告</span>
                    </div>
                    <div id="control58" class="col-md-4">
                        <input id="Control66" type="text" data-hreflink="" data-datafield="CreditReport" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="title59" class="col-md-2">
                        <span id="Label67" data-type="SheetLabel" data-datafield="QYCreditReport" style="" class="">企业征信报告</span>
                    </div>
                    <div id="control59" class="col-md-4">
                        <input id="Control67" type="text" data-hreflink="" data-datafield="QYCreditReport" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title60" class="col-md-2">
                        <span id="Label68" data-type="SheetLabel" data-datafield="QualificationInformation" style="" class="">开票资质材料</span>
                    </div>
                    <div id="control60" class="col-md-4">
                        <input id="Control68" type="text" data-hreflink="" data-datafield="QualificationInformation" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
                <div class="row">
                    <div id="div205744" class="col-md-2">
                        <label data-datafield="QueryRecord" data-type="SheetLabel" id="ctl711287" class="" style="">被执行查询记录</label>
                    </div>
                    <div id="div873318" class="col-md-4">
                        <input type="text" data-hreflink="" data-datafield="QueryRecord" data-type="SheetTextBox" id="ctl666666" class="" style="">
                    </div>
                    <div id="div407720" class="col-md-2">
                        <label data-datafield="LegalPerson" data-type="SheetLabel" id="ctl899797" class="" style="">法人自有房产（二网先放后抵）</label>
                    </div>
                    <div id="div340479" class="col-md-4">
                        <input type="text" data-hreflink="" data-datafield="LegalPerson" data-type="SheetTextBox" id="ctl746922" class="" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="div605744" class="col-md-12">
                        <a href="" data-downloadfile="" target="_blank" class="printHidden btn btn-outline btn-lg" style="width: 100%; border: 1px dashed rgb(221, 221, 221);">批量下载</a>
                    </div>
                </div>
            </div>
            <div class="xscs">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">销售初审</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title61" class="col-md-2">
                            <span id="Label69" data-type="SheetLabel" data-datafield="result0" style="" class="">审批结果</span>
                        </div>
                        <div id="control61" class="col-md-4">

                            <select data-datafield="result0" data-type="SheetDropDownList" id="ctl47662" class="" style="" data-defaultitems="同意;不同意"></select>
                        </div>
                        <div id="title62" class="col-md-2">
                        </div>
                        <div id="control62" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title63" class="col-md-2">
                            <span id="Label70" data-type="SheetLabel" data-datafield="advise0" style="" class="">审批意见</span>
                        </div>
                        <div id="control63" class="col-md-10">

                            <textarea data-datafield="advise0" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl925409" class=""></textarea>
                        </div>
                    </div>
                    <div class="row">
                        <div id="title65" class="col-md-2">
                            <span id="Label71" data-type="SheetLabel" data-datafield="OtherFile" style="" class="">其他附件</span>
                        </div>
                        <div id="control65" class="col-md-10">
                            <div id="Control71" data-datafield="OtherFile" data-type="SheetAttachment" style="" class=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="xszs">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">销售终审</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title67" class="col-md-2">

                            <label data-datafield="result1" data-type="SheetLabel" id="ctl408957" class="" style="">审批结果</label>
                        </div>
                        <div id="control67" class="col-md-4">


                            <select data-datafield="result1" data-type="SheetDropDownList" id="ctl43535" class="" style="" data-defaultitems="同意;不同意"></select>
                        </div>
                        <div id="space68" class="col-md-2">
                        </div>
                        <div id="spaceControl68" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title69" class="col-md-2">

                            <label data-datafield="advise1" data-type="SheetLabel" id="ctl469884" class="" style="">审批意见</label>
                        </div>
                        <div id="control69" class="col-md-10">


                            <textarea data-datafield="advise1" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl774203" class=""></textarea>
                        </div>
                    </div>
                </div>
            </div>
            <div class="xxcs">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">信审初审</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title71" class="col-md-2">

                            <label data-datafield="result2" data-type="SheetLabel" id="ctl364702" class="" style="">审批结果</label>
                        </div>
                        <div id="control71" class="col-md-4">


                            <select data-datafield="result2" data-type="SheetDropDownList" id="ctl181309" class="" style="" data-defaultitems="同意;不同意"></select>
                        </div>
                        <div id="space72" class="col-md-2">
                        </div>
                        <div id="spaceControl72" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title73" class="col-md-2">

                            <label data-datafield="advise2" data-type="SheetLabel" id="ctl399602" class="" style="">审批意见</label>
                        </div>
                        <div id="control73" class="col-md-10">
                            <textarea data-datafield="advise2" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl945487" class=""></textarea>
                        </div>
                    </div>
                    <div class="row">
                        <div id="title65" class="col-md-2">
                            <span id="Label71" data-type="SheetLabel" data-datafield="OtherFile2" style="" class="">信审初审附件</span>
                        </div>
                        <div id="control65" class="col-md-10">
                            <div id="Control71" data-datafield="OtherFile2" data-type="SheetAttachment" style="" class=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="xxfs">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">信审复审</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row">
                        <div id="title84" class="col-md-2">
                            <span id="Label81" data-type="SheetLabel" data-datafield="result4" style="" class="">审批结果</span>
                        </div>
                        <div id="control84" class="col-md-4">
                            <select data-datafield="result4" data-type="SheetDropDownList" id="ctl535398" class="" style="" data-defaultitems="同意;不同意"></select>
                        </div>
                        <div id="space73" class="col-md-2">
                        </div>
                        <div id="spaceControl73" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent">
                        <div id="title73" class="col-md-2">
                            <span id="Label75" data-type="SheetLabel" data-datafield="advise4" style="" class="">审批意见</span>
                        </div>
                        <div id="control73" class="col-md-10">
                            <textarea data-datafield="advise4" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl43451" class=""></textarea>
                        </div>
                    </div>
                    <div class="row">
                        <div id="title65" class="col-md-2">
                            <span id="Label71" data-type="SheetLabel" data-datafield="OtherFile4" style="" class="">信审复审附件</span>
                        </div>
                        <div id="control65" class="col-md-10">
                            <div id="Control71" data-datafield="OtherFile4" data-type="SheetAttachment" style="" class=""></div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="xxzs">
                <div class="nav-icon fa  fa-chevron-right bannerTitle">
                    <label id="divSheetInfo" data-en_us="Sheet information">信审终审</label>
                </div>
                <div class="divContent" id="divSheet">
                    <div class="row xxzsjg">
                        <div id="title75" class="col-md-2">
                            <span id="Label76" data-type="SheetLabel" data-datafield="result3" style="" class="">审批结果</span>
                        </div>
                        <div id="control75" class="col-md-4">
                            <select data-datafield="result3" data-type="SheetDropDownList" id="ctl833517" class="" style="" data-defaultitems="同意;不同意;总经理特批"></select>
                        </div>
                        <div id="space76" class="col-md-2">
                        </div>
                        <div id="spaceControl76" class="col-md-4">
                        </div>
                    </div>
                    <div class="row xxzsjg">
                        <div id="div785742" class="col-md-2">
                            <label data-datafield="LendingMode" data-type="SheetLabel" id="ctl542395" class="" style="">放款模式</label>
                        </div>
                        <div id="div235728" class="col-md-4">
                            <input type="text" data-datafield="LendingMode" data-type="SheetTextBox" id="ctl249930" class="" style="">
                        </div>
                        <div id="div23297" class="col-md-2">
                            <label data-datafield="VerifyLoan" data-type="SheetLabel" id="ctl629429" class="" style="">提前放款额度</label>
                        </div>
                        <div id="div420287" class="col-md-4">
                            <input type="text" data-datafield="VerifyLoan" data-type="SheetTextBox" id="ctl163433" class="" style="">
                        </div>
                    </div>
                    <div class="row tableContent xxzsjg">
                        <div id="title77" class="col-md-2">
                            <span id="Label77" data-type="SheetLabel" data-datafield="advise3" style="" class="">审批意见</span>
                        </div>
                        <div id="control77" class="col-md-10">
                            <textarea data-datafield="advise3" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl498967" class=""></textarea>
                        </div>
                    </div>
                    <div class="row xxzsjg">
                        <div id="title65" class="col-md-2">
                            <span id="Label71" data-type="SheetLabel" data-datafield="OtherFile3" style="" class="">信审终审附件</span>
                        </div>
                        <div id="control65" class="col-md-10">
                            <div id="Control71" data-datafield="OtherFile3" data-type="SheetAttachment" style="" class=""></div>
                        </div>
                    </div>
                    <div class="row zjltp">
                        <div id="title75" class="col-md-2">
                            <label data-datafield="result5" data-type="SheetLabel" id="ctl687740" class="" style="">特批结果</label>
                        </div>
                        <div id="control75" class="col-md-4">
                            <select data-datafield="result5" data-type="SheetDropDownList" id="ctl304298" class="" style="" data-defaultitems="同意;不同意"></select>
                        </div>
                        <div id="space76" class="col-md-2">
                        </div>
                        <div id="spaceControl76" class="col-md-4">
                        </div>
                    </div>
                    <div class="row tableContent zjltp">
                        <div id="title77" class="col-md-2">
                            <label data-datafield="advise5" data-type="SheetLabel" id="ctl812511" class="" style="">特批意见</label>
                        </div>
                        <div id="control77" class="col-md-10">
                            <textarea data-datafield="advise5" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl977596" class=""></textarea>
                        </div>
                    </div>
                    <div class="row xxzsxz">
                        <div id="control9" class="col-md-4">
                            <select name="select2" id="select2" style="width: 200px" onchange="">
                                <option value="" selected="selected">《——请选择——》</option>
                            </select>
                        </div>
                        <div id="control83" class="col-md-8">
                            <a onclick="jumpToEidtDoc('NTKO', false);" class="printHidden btn btn-outline btn-lg" style="width: 100%; border: 1px dashed rgb(221, 221, 221);">下载</a>
                        </div>
                    </div>
                    <div class="row xxzsxy">
                        <div id="title79" class="col-md-2">
                            <span id="Label78" data-type="SheetLabel" data-datafield="Agreement" style="">协议</span>
                        </div>
                        <div id="control79" class="col-md-10">
                            <div id="Control78" data-datafield="Agreement" data-type="SheetAttachment" style=""></div>
                        </div>
                    </div>
                    <div class="row xxzszh">
                        <div id="title81" class="col-md-2">
                            <span id="Label79" data-type="SheetLabel" data-datafield="Account" style="">账户</span>
                        </div>
                        <div id="control81" class="col-md-4">
                            <input type="text" data-datafield="Account" data-type="SheetTextBox" id="ctl453741" class="" style="">
                        </div>
                        <div class="col-md-2" id="space82">
                            <label id="ctl672367" data-type="SheetLabel" data-datafield="FinancialCode">财务编码</label>
                        </div>
                        <div class="col-md-4" id="spaceControl82">
                            <input id="ctl722372" type="text" data-type="SheetTextBox" data-datafield="FinancialCode">
                        </div>
                    </div>
                    <div class="row xxzsgl">
                        <div id="title83" class="col-md-2">
                            <span id="Label80" data-type="SheetLabel" data-datafield="PruductForm" style="">产品关联表</span>
                        </div>
                        <div id="control83" class="col-md-10">
                            <div id="Control80" data-datafield="PruductForm" data-type="SheetAttachment" style=""></div>
                        </div>
                    </div>
                    <div class="row" style="display:none;">
                        <div id="div225249" class="col-md-12">
                            <input type="text" data-datafield="CurrentUser" data-type="SheetTextBox" id="ctl292258" class="" style="">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
    <script type="text/javascript">
        function shakeMsg(msg) {
            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                alert(msg);
            } else {
                if (typeof (layer) == "undefined") {
                    alert(msg);
                } else {
                    layer.msg(msg, function() {});
                }
            }
        }
        function showMsg(msg) {
            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                alert(msg);
            } else {
                layer.msg(msg);
            }
        }
        function getDownLoadURL() {
            if ($("#divattachment").find("a").length < 2) {
                shakeMsg("附件为空！");
            } else {
                window.open("EnterNet.html");
            }
            event.stopPropagation();
        }
        $.MvcSheet.Loaded = function () {
	    $.MvcSheetUI.SetControlValue('CurrentUser', $.MvcSheetUI.SheetInfo.UserName);

            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                $(".xscs,.xszs,.xxcs,.xxfs,.xxzs").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                $(".xszs,.xxcs,.xxfs,.xxzs").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity10") {
                $(".xxcs,.xxfs,.xxzs").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity11") {
                $(".xxfs,.xxzs").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity12") {
                $(".xxzs").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity13") {
                $(".zjltp,.xxzsxy,.xxzszh,.xxzsgl,.xxzsxz").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity16") {
                $(".xxzsxy,.xxzszh,.xxzsgl,.xxzsxz").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity17") {
                $(".xxzszh,.xxzsgl,.xxzsxy").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity108") {
                $(".xxzszh,.xxzsgl").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity19") {
                $(".xxzsgl").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity17") $(".xxzsxz").show();
            else $(".xxzsxz").hide();
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity20") {
                $(".xszs,.xxcs").hide();
                $("#divSheetOther,#divSheetCompany").hide();
                $("#divSheetOther,#divSheetCompany").prev().hide();
                $(".xxzsjg").hide();
            }
            if ($(".xxzsjg").eq(0).find("label").text() == "总经理特批") {
                $(".zjltp").css("display", "block");
            } else {
                $(".zjltp").css("display", "none");
            }
            $.post("/Portal/GetScore/Index", { dealer: $("#Control12").val(), type: $("#Control13").val(), dealerType: $("#Control14").val(), action: "AllowIn", crmDealerId: $("#crmDealerId").val() }, function (msg) {//wangxg 19.7
            //$.post("/Portal/ajax/GetScore.ashx", { dealer: $("#Control12").val(), type: $("#Control13").val(), dealerType: $("#Control14").val(), action: "AllowIn", crmDealerId: $("#crmDealerId").val() }, function (msg) {
                $("#Control37").val(msg).next().text(msg);
            });
            var $files = $("[data-hreflink]");
            for (var i = 0; i < $files.length; i++) {
                var $file = $files.eq(i);
                var $t = $file.next();
                $t.attr("hidden", "");
                var values = $t.find("span").text();
                if (values != "" && values != null) {
                    if (values.indexOf("<>") > -1) {
                        var val = values.split("<>");
                        for (var j = 0; j < val.length; j++) {
                            $t.after("<a href='http://172.16.10.80/" + val[j].split(">")[1] + "' target='_blank'>" + val[j].split(">")[0] + "</a><br/>");
                        }
                    } else {
                        $t.after("<a href='http://172.16.10.80/" + values.split(">")[1] + "' target='_blank'>" + values.split(">")[0] + "</a>");
                    }
                }
            }
            //$("[data-downloadfile]").attr("href", "/Portal/ajax/EnterNetAttachment.ashx?objectId=" + $.MvcSheetUI.SheetInfo.BizObjectID);
            $("[data-downloadfile]").attr("href", "/Portal/EnterNetAttachment/Index?objectId=" + $.MvcSheetUI.SheetInfo.BizObjectID);//wangxg 19.7
            if ($("#divattachment").find("a").length == 1) $("[data-downloadfile]").parent().parent().hide();
            else $("[data-downloadfile]").parent().parent().show();
            SheetLoaded();
        }
    </script>
</asp:Content>