﻿<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UpdateEnterNet.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.EnterNet.UpdateEnterNet" EnableEventValidation="false" MasterPageFile="~/MvcSheetEnterNet2.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <%--<div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">入网修改流程</label>
    </div>--%>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa fa-chevron-right bannerTitle">
            <label id="divBasicInfo" data-en_us="Basic information">基本信息</label>
        </div>
        <div class="divContent">
            <div class="row">
                <div id="divFullNameTitle" class="col-md-2">
                    <label id="lblFullNameTitle" data-type="SheetLabel" data-datafield="Originator.UserName" data-en_us="Originator" data-bindtype="OnlyVisiable" style="">发起人</label>
                </div>
                <div id="divFullName" class="col-md-4">
                    <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style=""></label>
                </div>
                <div id="divOriginateDateTitle" class="col-md-2">
                    <label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                </div>
                <div id="divOriginateDate" class="col-md-4">
                    <label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style=""></label>
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
                    <input id="Control11" type="text" data-datafield="Manager" data-type="SheetTextBox" style="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="Distributor" style="">经销商名称</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="Distributor" data-type="SheetTextBox" style="">
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
                    <span id="Label14" data-type="SheetLabel" data-datafield="DistributorType" style="" class="">经销商分类</span>
                </div>
                <div id="control4" class="col-md-4">
                    <input id="Control14" type="text" data-datafield="DistributorType" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="City" style="" class="">城市</span>
                </div>
                <div id="control5" class="col-md-4">
                    <input id="Control16" type="text" data-datafield="City" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title6" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="Province" style="" class="">省份</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control15" type="text" data-datafield="Province" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title7" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="CompanyAddr" style="" class="">公司地址</span>
                </div>
                <div id="control7" class="col-md-10" colspan="2">
                    <input id="Control17" type="text" data-datafield="CompanyAddr" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title9" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="BelongTo" style="" class="">所属集团或平台</span>
                </div>
                <div id="control9" class="col-md-4">
                    <input id="Control18" type="text" data-datafield="BelongTo" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title10" class="col-md-2">
                    <span id="Label19" data-type="SheetLabel" data-datafield="Brand" style="" class="">经营品牌</span>
                </div>
                <div id="control10" class="col-md-4">
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
                    <textarea id="Control22" data-datafield="Memo" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">营业执照信息</label>
            </div>
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
                    <input id="Control24" type="text" data-datafield="EnterpriseRegistration" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title17" class="col-md-2">
                    <span id="Label25" data-type="SheetLabel" data-datafield="RegistrationDate" style="">登记日期</span>
                </div>
                <div id="control17" class="col-md-4">
                    <input id="Control25" type="text" data-datafield="RegistrationDate" data-type="SheetTextBox" style="">
                </div>
                <div id="title18" class="col-md-2">
                    <span id="Label26" data-type="SheetLabel" data-datafield="CreatDate" style="">创立日期</span>
                </div>
                <div id="control18" class="col-md-4">
                    <input id="Control26" type="text" data-datafield="CreatDate" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title19" class="col-md-2">
                    <span id="Label27" data-type="SheetLabel" data-datafield="LegalRepresentative" style="">法定代表人</span>
                </div>
                <div id="control19" class="col-md-4">
                    <input id="Control27" type="text" data-datafield="LegalRepresentative" data-type="SheetTextBox" style="">
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
                    <input id="Control29" type="text" data-datafield="RegisteredCapital" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">银行开户信息</label>
            </div>
            <div class="row">
                <div id="title22" class="col-md-2">
                    <span id="Label30" data-type="SheetLabel" data-datafield="BankBranch" style="">银行开户分行</span>
                </div>
                <div id="control22" class="col-md-4">
                    <input id="Control30" type="text" data-datafield="BankBranch" data-type="SheetTextBox" style="">
                </div>
                <div id="title23" class="col-md-2">
                    <span id="Label31" data-type="SheetLabel" data-datafield="AccountType" style="">银行账户类型</span>
                </div>
                <div id="control23" class="col-md-4">
                    <input id="Control31" type="text" data-datafield="AccountType" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title24" class="col-md-2">
                    <span id="Label32" data-type="SheetLabel" data-datafield="BankName" style="">银行账户名</span>
                </div>
                <div id="control24" class="col-md-4">
                    <input id="Control32" type="text" data-datafield="BankName" data-type="SheetTextBox" style="">
                </div>
                <div id="title25" class="col-md-2">
                    <span id="Label33" data-type="SheetLabel" data-datafield="BankAccount" style="">银行账号</span>
                </div>
                <div id="control25" class="col-md-4">
                    <input id="Control33" type="text" data-datafield="BankAccount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title26" class="col-md-2">
                    <span id="Label34" data-type="SheetLabel" data-datafield="CoupletNumber" style="">联行号</span>
                </div>
                <div id="control26" class="col-md-4">
                    <input id="Control34" type="text" data-datafield="CoupletNumber" data-type="SheetTextBox" style="">
                </div>
                <div id="div857116" class="col-md-2" style="display: none;">
                    <label data-datafield="CrmDealerId" data-type="SheetLabel" id="ctl917931" class="" style="">Crm经销商Id</label>
                </div>
                <div id="div422732" class="col-md-4" style="display: none;">
                    <input type="text" data-datafield="CrmDealerId" data-type="SheetTextBox" id="ctl476536" class="" style="">
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">服务费银行开户信息</label>
            </div>
            <div class="row">
                <div id="title241" class="col-md-2">
                    <span id="Label321" data-type="SheetLabel" data-datafield="ServiceFeeAccountName" style="">服务费账户名</span>
                </div>
                <div id="control241" class="col-md-4">
                    <input id="Control321" type="text" data-datafield="ServiceFeeAccountName" data-type="SheetTextBox" style="">
                </div>
                <div id="title251" class="col-md-2">
                    <span id="Label331" data-type="SheetLabel" data-datafield="ServiceFeeAccount" style="">服务费账号</span>
                </div>
                <div id="control251" class="col-md-4">
                    <input id="Control331" type="text" data-datafield="ServiceFeeAccount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title242" class="col-md-2">
                    <span id="Label322" data-type="SheetLabel" data-datafield="ServiceFeeBankName" style="">服务费开户行</span>
                </div>
                <div id="control242" class="col-md-4">
                    <input id="Control322" type="text" data-datafield="ServiceFeeBankName" data-type="SheetTextBox" style="">
                </div>
                <div id="title252" class="col-md-2">
                    <span id="Label332" data-type="SheetLabel" data-datafield="ServiceFeeAccountKind" style="">服务费账户类型</span>
                </div>
                <div id="control252" class="col-md-4">
                    <input id="Control3322" type="text" data-datafield="ServiceFeeAccountKind" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title261" class="col-md-2">
                    <span id="Label341" data-type="SheetLabel" data-datafield="ServiceFeeCode" style="">服务费联行号</span>
                </div>
                <div id="control261" class="col-md-4">
                    <input id="Control341" type="text" data-datafield="ServiceFeeCode" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">联系方式</label>
            </div>
            <div class="row tableContent">
                <div id="title27" class="col-md-2">
                    <span id="Label35" data-type="SheetLabel" data-datafield="UpdateContactInformation" style="">联系方式</span>
                </div>
                <div id="control27" class="col-md-10">
                    <table id="Control35" data-datafield="UpdateContactInformation" data-type="SheetGridView" class="SheetGridView" style="width:100%">
                        <tbody>

                            <tr class="header">
                                <td id="Control35_SerialNo" class="rowSerialNo">
                                    序号
                                </td>
                                <td id="Control35_Header3" data-datafield="UpdateContactInformation.Dept">
                                    <label id="Control35_Label3" data-datafield="UpdateContactInformation.Dept" data-type="SheetLabel" style="">部门</label>
                                </td>
                                <td id="Control35_Header4" data-datafield="UpdateContactInformation.Contact">
                                    <label id="Control35_Label4" data-datafield="UpdateContactInformation.Contact" data-type="SheetLabel" style="">联系人</label>
                                </td>
                                <td id="Control35_Header5" data-datafield="UpdateContactInformation.Phone">
                                    <label id="Control35_Label5" data-datafield="UpdateContactInformation.Phone" data-type="SheetLabel" style="">电话</label>
                                </td>
                                <td id="Control35_Header6" data-datafield="UpdateContactInformation.Email">
                                    <label id="Control35_Label6" data-datafield="UpdateContactInformation.Email" data-type="SheetLabel" style="">邮箱</label>
                                </td>
                                <td id="Control35_Header7" data-datafield="UpdateContactInformation.PositionInputName">
                                    <label id="Control35_Label7" data-datafield="UpdateContactInformation.PositionInputName" data-type="SheetLabel" style="">具体职位</label>
                                </td>
                                <td id="Control35_Header8" data-datafield="UpdateContactInformation.IsMainReceiver">
                                    <label id="Control35_Label8" data-datafield="UpdateContactInformation.IsMainReceiver" data-type="SheetLabel" style="">放款短信接收人</label>
                                </td>
                                <td class="rowOption">
                                    删除
                                </td>
                            </tr>
                            <tr class="template">
                                <td id="Control35_Option" class="rowOption"></td>
                                <td data-datafield="UpdateContactInformation.Dept">
                                    <input id="Control35_ctl3" type="text" data-datafield="UpdateContactInformation.Dept" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="UpdateContactInformation.Contact">
                                    <input id="Control35_ctl4" type="text" data-datafield="UpdateContactInformation.Contact" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="UpdateContactInformation.Phone">
                                    <input id="Control35_ctl5" type="text" data-datafield="UpdateContactInformation.Phone" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="UpdateContactInformation.Email">
                                    <input id="Control35_ctl6" type="text" data-datafield="UpdateContactInformation.Email" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="UpdateContactInformation.PositionInputName">
                                    <input id="Control35_ctl7" type="text" data-datafield="UpdateContactInformation.PositionInputName" data-type="SheetTextBox" style="">
                                </td>
                                <td data-datafield="UpdateContactInformation.IsMainReceiver">
                                    <input id="Control35_ctl8" type="text" data-datafield="UpdateContactInformation.IsMainReceiver" data-type="SheetTextBox" style="">
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
                                <td data-datafield="UpdateContactInformation.Dept"></td>
                                <td data-datafield="UpdateContactInformation.Contact"></td>
                                <td data-datafield="UpdateContactInformation.Phone"></td>
                                <td data-datafield="UpdateContactInformation.Email"></td>
                                <td class="rowOption"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">其他信息</label>
            </div>
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
                    <input id="Control37" type="text" data-datafield="SystemScore" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title31" class="col-md-2">
                    <span id="Label38" data-type="SheetLabel" data-datafield="GPSAddr" style="">GPS地址</span>
                </div>
                <div id="control31" class="col-md-10">
                    <input id="Control38" type="text" data-datafield="GPSAddr" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title32" class="col-md-2">
                    <span id="Label39" data-type="SheetLabel" data-datafield="GPSAccount" style="">GPS账号</span>
                </div>
                <div id="control32" class="col-md-4">
                    <input id="Control39" type="text" data-datafield="GPSAccount" data-type="SheetTextBox" style="">
                </div>
                <div id="title33" class="col-md-2">
                    <span id="Label40" data-type="SheetLabel" data-datafield="GPSPassword" style="">GPS密码</span>
                </div>
                <div id="control33" class="col-md-4">
                    <input id="Control40" type="text" data-datafield="GPSPassword" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">附件</label>
            </div>
            <div class="divContent" id="divattachment">
                <div class="row">
                <div id="title34" class="col-md-2">
                    <span id="Label41" data-type="SheetLabel" data-datafield="ApprovalForm" style="">准入审批表</span>
                </div>
                <div id="control34" class="col-md-4">
                    <input id="Control41" type="text" data-hreflink="" data-datafield="ApprovalForm" data-type="SheetTextBox" style="">
                </div>
                <div id="title35" class="col-md-2">
                    <span id="Label42" data-type="SheetLabel" data-datafield="CollectionTable" style="">信息采集表</span>
                </div>
                <div id="control35" class="col-md-4">
                    <input id="Control42" type="text" data-hreflink="" data-datafield="CollectionTable" data-type="SheetTextBox" style="">
                </div>
            </div>
                <div class="row">
                    <div id="title36" class="col-md-2">
                        <span id="Label43" data-type="SheetLabel" data-datafield="BusinessLicense" style="">营业执照复印件</span>
                    </div>
                    <div id="control36" class="col-md-4">
                        <input id="Control43" type="text" data-hreflink="" data-datafield="BusinessLicense" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title37" class="col-md-2">
                        <span id="Label44" data-type="SheetLabel" data-datafield="CreditDocuments" style="">征信/合作文件</span>
                    </div>
                    <div id="control37" class="col-md-4">
                        <input id="Control44" type="text" data-hreflink="" data-datafield="CreditDocuments" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title38" class="col-md-2">
                        <span id="Label45" data-type="SheetLabel" data-datafield="AuthorizationDocument" style="">厂商授权文件</span>
                    </div>
                    <div id="control38" class="col-md-4">
                        <input id="Control45" type="text" data-hreflink="" data-datafield="AuthorizationDocument" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title39" class="col-md-2">
                        <span id="Label46" data-type="SheetLabel" data-datafield="Photo" style="">门头照片</span>
                    </div>
                    <div id="control39" class="col-md-4">
                        <input id="Control46" type="text" data-hreflink="" data-datafield="Photo" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title40" class="col-md-2">
                        <span id="Label47" data-type="SheetLabel" data-datafield="DiscountAgreement" style="">贴息协议</span>
                    </div>
                    <div id="control40" class="col-md-4">
                        <input id="Control47" type="text" data-hreflink="" data-datafield="DiscountAgreement" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title41" class="col-md-2">
                        <span id="Label48" data-type="SheetLabel" data-datafield="CargoAgreement" style="">附加货协议</span>
                    </div>
                    <div id="control41" class="col-md-4">
                        <input id="Control48" type="text" data-hreflink="" data-datafield="CargoAgreement" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title42" class="col-md-2">
                        <span id="Label49" data-type="SheetLabel" data-datafield="HallPhoto" style="">展厅照片</span>
                    </div>
                    <div id="control42" class="col-md-4">
                        <input id="Control49" type="text" data-hreflink="" data-datafield="HallPhoto" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title43" class="col-md-2">
                        <span id="Label50" data-type="SheetLabel" data-datafield="OfficePhoto" style="">办公区照片</span>
                    </div>
                    <div id="control43" class="col-md-4">
                        <input id="Control50" type="text" data-hreflink="" data-datafield="OfficePhoto" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title44" class="col-md-2">
                        <span id="Label51" data-type="SheetLabel" data-datafield="BusinessInformation" style="">企业工商信息</span>
                    </div>
                    <div id="control44" class="col-md-4">
                        <input id="Control51" type="text" data-hreflink="" data-datafield="BusinessInformation" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title45" class="col-md-2">
                        <span id="Label52" data-type="SheetLabel" data-datafield="FinancialInformation" style="">金融专员资料</span>
                    </div>
                    <div id="control45" class="col-md-4">
                        <input id="Control52" type="text" data-hreflink="" data-datafield="FinancialInformation" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title46" class="col-md-2">
                        <span id="Label53" data-type="SheetLabel" data-datafield="FinancialId" style="">金融专员身份证复印件</span>
                    </div>
                    <div id="control46" class="col-md-4">
                        <input id="Control53" type="text" data-hreflink="" data-datafield="FinancialId" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title47" class="col-md-2">
                        <span id="Label54" data-type="SheetLabel" data-datafield="CooperationAgreement" style="">合作协议</span>
                    </div>
                    <div id="control47" class="col-md-4">
                        <input id="Control54" type="text" data-hreflink="" data-datafield="CooperationAgreement" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title48" class="col-md-2">
                        <span id="Label55" data-type="SheetLabel" data-datafield="RepresentativeId" style="">代表/实控人(身份证复印件)</span>
                    </div>
                    <div id="control48" class="col-md-4">
                        <input id="Control55" type="text" data-hreflink="" data-datafield="RepresentativeId" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title49" class="col-md-2">
                        <span id="Label56" data-type="SheetLabel" data-datafield="PersonnelPhoto" style="">人员合照</span>
                    </div>
                    <div id="control49" class="col-md-4">
                        <input id="Control56" type="text" data-hreflink="" data-datafield="PersonnelPhoto" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title50" class="col-md-2">
                        <span id="Label57" data-type="SheetLabel" data-datafield="Report" style="">尽调报告</span>
                    </div>
                    <div id="control50" class="col-md-4">
                        <input id="Control57" type="text" data-hreflink="" data-datafield="Report" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title51" class="col-md-2">
                        <span id="Label58" data-type="SheetLabel" data-datafield="SiteContract" style="">场地租赁合同</span>
                    </div>
                    <div id="control51" class="col-md-4">
                        <input id="Control58" type="text" data-hreflink="" data-datafield="SiteContract" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title52" class="col-md-2">
                        <span id="Label59" data-type="SheetLabel" data-datafield="XFHDSPB" style="">先放后抵审批表</span>
                    </div>
                    <div id="control52" class="col-md-4">
                        <input id="Control59" type="text" data-hreflink="" data-datafield="XFHDSPB" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title53" class="col-md-2">
                        <span id="Label60" data-type="SheetLabel" data-datafield="QuotaForm" style="">额度申请表</span>
                    </div>
                    <div id="control53" class="col-md-4">
                        <input id="Control60" type="text" data-hreflink="" data-datafield="QuotaForm" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title54" class="col-md-2">
                        <span id="Label61" data-type="SheetLabel" data-datafield="SalesData" style="">销量+放款额数据</span>
                    </div>
                    <div id="control54" class="col-md-4">
                        <input id="Control61" type="text" data-hreflink="" data-datafield="SalesData" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title55" class="col-md-2">
                        <span id="Label62" data-type="SheetLabel" data-datafield="FinancialInstitutions" style="">合作金融机构(名单)</span>
                    </div>
                    <div id="control55" class="col-md-4">
                        <input id="Control62" type="text" data-hreflink="" data-datafield="FinancialInstitutions" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title56" class="col-md-2">
                        <span id="Label63" data-type="SheetLabel" data-datafield="BankFlow" style="">公司银行流水(近3个月)</span>
                    </div>
                    <div id="control56" class="col-md-4">
                        <input id="Control63" type="text" data-hreflink="" data-datafield="BankFlow" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title57" class="col-md-2">
                        <span id="Label64" data-type="SheetLabel" data-datafield="PersonalInformation" style="">个人资信情况(法人/实控人)</span>
                    </div>
                    <div id="control57" class="col-md-4">
                        <input id="Control64" type="text" data-hreflink="" data-datafield="PersonalInformation" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title58" class="col-md-2">
                        <span id="Label65" data-type="SheetLabel" data-datafield="SecurityAgreement" style="">担保协议</span>
                    </div>
                    <div id="control58" class="col-md-4">
                        <input id="Control65" type="text" data-hreflink="" data-datafield="SecurityAgreement" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title59" class="col-md-2">
                        <span id="Label66" data-type="SheetLabel" data-datafield="CreditReport" style="">征信报告</span>
                    </div>
                    <div id="control59" class="col-md-4">
                        <input id="Control66" type="text" data-hreflink="" data-datafield="CreditReport" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="title60" class="col-md-2">
                        <span id="Label67" data-type="SheetLabel" data-datafield="QYCreditReport" style="">企业征信报告</span>
                    </div>
                    <div id="control60" class="col-md-4">
                        <input id="Control67" type="text" data-hreflink="" data-datafield="QYCreditReport" data-type="SheetTextBox" style="">
                    </div>
                    <div id="title61" class="col-md-2">
                        <span id="Label68" data-type="SheetLabel" data-datafield="QualificationInformation" style="">开票资质材料</span>
                    </div>
                    <div id="control61" class="col-md-4">
                        <input id="Control68" type="text" data-hreflink="" data-datafield="QualificationInformation" data-type="SheetTextBox" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="div631021" class="col-md-2">
                        <label data-datafield="QueryRecord" data-type="SheetLabel" id="ctl804649" class="" style="">被执行查询记录</label>
                    </div>
                    <div id="div405441" class="col-md-4">
                        <input type="text" data-hreflink="" data-datafield="QueryRecord" data-type="SheetTextBox" id="ctl370183" class="" style="">
                    </div>
                    <div id="div935794" class="col-md-2">
                        <label data-datafield="LegalPerson" data-type="SheetLabel" id="ctl909299" class="" style="">法人自有房产（二网先放后抵）</label>
                    </div>
                    <div id="div793112" class="col-md-4">
                        <input type="text" data-hreflink="" data-datafield="LegalPerson" data-type="SheetTextBox" id="ctl927492" class="" style="">
                    </div>
                </div>
                <div class="row">
                    <div id="div605744" class="col-md-12">
                        <a href="" data-attachment="" target="_blank" class="printHidden btn btn-outline btn-lg" style="width: 100%; border: 1px dashed rgb(221, 221, 221);">批量下载</a>
                    </div>
                </div>
            </div>
            <div class="row" style="display:none;">
                <div id="div126747" class="col-md-2" data-title="" style="padding-bottom: 10000px; margin-bottom: -9995px;">
                    <label data-datafield="ChangeField" data-type="SheetLabel" id="ctl305574" class="" style="display: block;">更改字段</label>
                </div>
                <div id="div194483" class="col-md-4" data-title="更改字段" style="padding-bottom: 10000px; margin-bottom: -9995px;">
                    <textarea data-datafield="ChangeField" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl400972" class="inputMouseOut" placeholder=""></textarea>
                </div>
            </div>
            <div class="row tableContent xssp">
                <div id="title63" class="col-md-2">
                    <span id="Label69" data-type="SheetLabel" data-datafield="SellApproval" style="">销售审批</span>
                </div>
                <div id="control63" class="col-md-10">
                    <textarea id="Control69" data-datafield="SellApproval" data-type="SheetRichTextBox" style="" class="">					</textarea>
                </div>
            </div>
            <div class="row tableContent xssp2">
                <div id="title65" class="col-md-2">
                    <span id="Label70" data-type="SheetLabel" data-datafield="TrustApproval" style="">信审审批</span>
                </div>
                <div id="control65" class="col-md-10">
                    <textarea id="Control70" data-datafield="TrustApproval" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
            <div class="row tableContent fkjc">
                <div id="title67" class="col-md-2">
                    <span id="Label71" data-type="SheetLabel" data-datafield="WindDecision" style="">风控决策</span>
                </div>
                <div id="control67" class="col-md-10">
                    <textarea id="Control71" data-datafield="WindDecision" data-type="SheetRichTextBox" style="">					</textarea>
                </div>
            </div>
            <div class="row xszc">
                <div id="title69" class="col-md-2">
                    <span id="Label72" data-type="SheetLabel" data-datafield="UpdataFiles" style="">上传附件</span>
                </div>
                <div id="control69" class="col-md-10">
                    <div id="Control72" data-datafield="UpdataFiles" data-type="SheetAttachment" style=""></div>
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
        $.MvcSheet.Loaded = function() {

            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                $(".xssp,.xssp2,.fkjc,.xszc").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                $(".xssp2,.fkjc,.xszc").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
                $(".fkjc,.xszc").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity17") {
                $(".xszc").hide();
            }
            $.post("/Portal/GetScore", { action: "UpdateEnterNet", crmDealerId: $.MvcSheetUI.GetControlValue("CrmDealerId") }, function (msg) {//wangxg 19.7
            //$.post("/Portal/ajax/GetScore.ashx", { action: "UpdateEnterNet", crmDealerId: $.MvcSheetUI.GetControlValue("CrmDealerId") }, function (msg) {
                $("#Control37").val(msg).next().text(msg);
            });
            var $files = $("[data-hreflink]");
            for (var i = 0; i < $files.length; i++) {
                var $t = $files.eq(i).next();
                $t.attr("hidden", "");
                var values = $t.find("span").text();
                if (values != "" && values != null) {
                    if (values.indexOf("<>") > -1) {
                        var val = values.split("<>");
                        for (var j = 0; j < val.length; j++) {
                            $t.after("<a href='http://192.168.16.122/" + val[j].split(">")[1] + "' target='_blank'>" + val[j].split(">")[0] + "</a>");
                        }
                    } else {
                        $t.after("<a href='http://192.168.16.122/" + values.split(">")[1] + "' target='_blank'>" + values.split(">")[0] + "</a>");
                    }
                }
            }
            //$("[data-attachment]").attr("href", "/Portal/ajax/EnterNetAttachment.ashx?objectId=" + $.MvcSheetUI.SheetInfo.BizObjectID);
            $("[data-attachment]").attr("href", "/Portal/EnterNetAttachment/Index?objectId=" + $.MvcSheetUI.SheetInfo.BizObjectID);//wangxg 19.7
            if ($("#divattachment").find("a").length == 1) $("[data-attachment]").parent().parent().hide();
            else $("[data-attachment]").parent().parent().show();
            var changeFiled = $.MvcSheetUI.SheetInfo.BizObject.DataItems["ChangeField"].V;
            if (changeFiled != null && changeFiled != "") {
                changeFiled = changeFiled.split(',');
                for (var i = 0; i < changeFiled.length; i++) {
                    try { $.MvcSheetUI.GetElement(changeFiled[i].split(':')[0]).next().css("color", "pink").next().css("color", "pink"); }
                    catch (e) { }
                }
            }
        }
    </script>
</asp:Content>