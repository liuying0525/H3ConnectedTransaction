<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SCreditGetOrderRules.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SCreditGetOrderRules" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <script type="text/javascript">
        // 保存前事件
        $.MvcSheet.SaveAction.OnActionPreDo = function () {
            // this.Action  // 获取当前按钮名称
            var csry = $.MvcSheetUI.GetControlValue("FirstCreditPerson");
            var channeltype = $.MvcSheetUI.GetControlValue("ChannelType");
            var amount_from = $.MvcSheetUI.GetControlValue("LoanAmountFrom");
            var amount_to = $.MvcSheetUI.GetControlValue("LoanAmountTo");
            var limit = $.MvcSheetUI.GetControlValue("LimitNumber");

            var sp = $.MvcSheetUI.GetControlValue("SpecialPerson");
            var zsry = $.MvcSheetUI.GetControlValue("FinalCreditPerson");
            var zs_amout_from = $.MvcSheetUI.GetControlValue("FinalLoanAmountFrom");
            var zs_amout_to = $.MvcSheetUI.GetControlValue("FinalLoanAmountTo");
            if (sp) {
                if (zsry == "") {
                    alert("终审人员不能为空");
                    return false;
                }
                if (zs_amout_from == "") {
                    alert("终审审核起始额度不能为空");
                    return false;
                }
                if (zs_amout_to == "") {
                    alert("终审审核终止额度不能为空");
                    return false;
                }
            }
            else {
                if (csry == "") {
                    alert("初审人员不能为空");
                    return false;
                }
                if (channeltype == "") {
                    alert("渠道类型不允许为空");
                    return false;
                }
                if (amount_from == "") {
                    alert("审核起始额度不允许为空");
                    return false;
                }
                if (amount_to == "") {
                    alert("审核终止额度不允许为空");
                    return false;
                }
                if (limit == "") {
                    alert("主动获单上限不允许为空");
                    return false;
                }
                if (zsry == "") {
                    alert("终审人员不能为空");
                    return false;
                }
                if (zs_amout_from == "") {
                    alert("终审审核起始额度不能为空");
                    return false;
                }
                if (zs_amout_to == "") {
                    alert("终审审核终止额度不能为空");
                    return false;
                }
            }
        }
</script>
    <div class="panel-body sheetContainer">
        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label id="divSheetInfo" data-en_us="Sheet information" class="">初审人员信息</label>
        </div>
        <div class="divContent" id="divSheet">
            <div class="row showUser">
                <div id="title1" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="FirstCreditPerson" style="" class="">初审人员</span>
                </div>
                <div id="control1" class="col-md-4">
                    <div id="Control11" data-datafield="FirstCreditPerson" data-type="SheetUser" class="" style="" data-rootunitid="40e3f156-040f-4fb7-bd84-39ce490a8b29" data-mappingcontrols="UserName:Name,UserCode:Code,Department:DepartmentName"></div>
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="UserName" class="" style="">姓名</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="UserName" disabled="disabled" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="UserCode" style="">账号</span>
                </div>
                <div id="control3" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="UserCode" disabled="disabled" data-type="SheetTextBox" class="" style="">
                </div>
                <div id="title4" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="Department" style="">部门</span>
                </div>
                <div id="control4" class="col-md-4">
                    <input id="Control14" type="text" data-datafield="Department" disabled="disabled" data-type="SheetTextBox" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="ChannelType" style="">渠道类型</span>
                </div>
                <div id="control5" class="col-md-4">
                    <select data-datafield="ChannelType" data-type="SheetDropDownList" id="ctl324793" class="" style="" data-defaultitems="内网;外网;内外网" data-displayemptyitem="true">
                    </select>
                </div>
                <div id="title6" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="AssetCondition" style="">资产类型</span>
                </div>
                <div id="control6" class="col-md-4">
                    <select data-datafield="AssetCondition" data-type="SheetDropDownList" id="ctl434391" class="" style="" data-masterdatacategory="资产状况" data-displayemptyitem="true" data-emptyitemtext="全部">
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="title7" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="LoanAmountFrom" style="">审核起始额度</span>
                </div>
                <div id="control7" class="col-md-4">
                    <input id="Control17" type="text" data-datafield="LoanAmountFrom" data-type="SheetTextBox" style="" class="" data-formatrule="{0:C2}">
                </div>
                <div id="title8" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="LoanAmountTo" style="">审核终止额度</span>
                </div>
                <div id="control8" class="col-md-4">
                    <input id="Control18" type="text" data-datafield="LoanAmountTo" data-type="SheetTextBox" style="" class="" data-formatrule="{0:C2}">
                </div>
            </div>
            <div class="row">
                <div id="div991384" class="col-md-2">
                    <label data-datafield="LimitNumber" data-type="SheetLabel" id="ctl539470" class="" style="">主动获单上限</label>
                </div>
                <div id="div800940" class="col-md-4">
                    <input type="text" data-datafield="LimitNumber" data-type="SheetTextBox" id="ctl594621" class="" style="" data-defaultvalue="3">
                </div>
                <div id="div276056" class="col-md-2">
                </div>
                <div id="div174634" class="col-md-4">
                </div>
            </div>
        </div>

        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label data-en_us="Sheet information" class="">终审人员信息</label>
        </div>
        <div class="divContent">
            <div class="row showUser">
                <div id="title9" class="col-md-2">
                    <span id="Label19" data-type="SheetLabel" data-datafield="FinalCreditPerson" style="">终审人员</span>
                </div>
                <div id="control9" class="col-md-4">
                    <div id="Control19" data-datafield="FinalCreditPerson" data-type="SheetUser" style="" class="" data-mappingcontrols="FinalUserName:Name,FinalUserCode:Code,FinalDepartment:DepartmentName"></div>
                </div>
                <div id="title10" class="col-md-2">
                    <span id="Label20" data-type="SheetLabel" data-datafield="FinalUserName" style="">终审姓名</span>
                </div>
                <div id="control10" class="col-md-4">
                    <input id="Control20" type="text" data-datafield="FinalUserName" disabled="disabled" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title11" class="col-md-2">
                    <span id="Label21" data-type="SheetLabel" data-datafield="FinalUserCode" style="">终审账号</span>
                </div>
                <div id="control11" class="col-md-4">
                    <input id="Control21" type="text" data-datafield="FinalUserCode" disabled="disabled" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title12" class="col-md-2">
                    <span id="Label22" data-type="SheetLabel" data-datafield="FinalDepartment" style="">终审部门</span>
                </div>
                <div id="control12" class="col-md-4">
                    <input id="Control22" type="text" data-datafield="FinalDepartment" disabled="disabled" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="div324750" class="col-md-2">
                    <label data-datafield="SpecialPerson" data-type="SheetLabel" id="ctl499808" class="" style="">特殊终审</label>
                </div>
                <div id="div103790" class="col-md-4">
                    <input type="checkbox" data-datafield="SpecialPerson" data-type="SheetCheckBox" id="ctl391661" class="" style="">
                </div>
                <div id="div464403" class="col-md-2"></div>
                <div id="div307643" class="col-md-4"></div>
            </div>
            <div class="row">
                <div id="div167892" class="col-md-2">
                    <label data-datafield="FinalLoanAmountFrom" data-type="SheetLabel" id="ctl727952" class="" style="">终审审核起始额度</label>
                </div>
                <div id="div94290" class="col-md-4">
                    <input type="text" data-datafield="FinalLoanAmountFrom" data-type="SheetTextBox" id="ctl230781" class="" style="" data-formatrule="{0:C2}" data-defaultvalue="0">
                </div>
                <div id="div792361" class="col-md-2">
                    <label data-datafield="FinalLoanAmountTo" data-type="SheetLabel" id="ctl341157" class="" style="">终审审核终止额度</label>
                </div>
                <div id="div102938" class="col-md-4">
                    <input type="text" data-datafield="FinalLoanAmountTo" data-type="SheetTextBox" id="ctl747102" class="" style="" data-formatrule="{0:C2}" data-defaultvalue="499999">
                </div>
            </div>
            <div class="row">
                <div id="div366069" class="col-md-12">
                    <ul>
                        <li>1.维护初审及终审人员信息</li>
                        <li>2.默认终审的审核额度：0-499999</li>
                        <li>3.特殊终审不用维护初审信息</li>
                    </ul>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
