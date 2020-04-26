<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SPrewaring.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SPrewaring" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>
<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
<script type="text/javascript">

    var ___crmid = "";
    function GetScore(id) {
        $.ajax({
            type: "Post",
            url: "/Portal/GetScore/Index",//wangxg 19.7
            //url: "/Portal/ajax/GetScore.ashx",
            data: { action: "Prewaring", crmDealerId: id },
            success: function (g) {
                $("#Control12").val(g).next().text(g);
                GetTime();
            },
            //error: function() {
            //    GetTime();
            //},
            error: function (msg) {// 19.7 
                GetTime();
                 showJqErr(msg);
            }
        });
    }
    function GetTime() {
        setTimeout(function() {
            if ($.MvcSheetUI.GetControlValue("CrmDealerId") != ___crmid) {
                ___crmid = $.MvcSheetUI.GetControlValue("CrmDealerId");
                GetScore(___crmid);
            } else {
                GetTime();
            }
            
        },1000*3);
    }
    $.MvcSheet.Loaded = function () {
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
            $(".spyj,.fkjc,.gld,.gxed").hide();
        }
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity13") {
            $(".fkjc,.gld,.gxed").hide();
        }
        GetTime();
        $.MvcSheet.Submit = function (actionControl, text, destActivity, postValue, groupValue) {
            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                var controls = $("#divSheet input[data-type='SheetTextBox']");
                controls.each(function () {
                    $(this).trigger("change");
                });
            }
            if (!$.MvcSheet.ActionValidata(actionControl)) return false;      
            if ($.MvcSheetUI.GetControlValue('Decision') == '关闭账户' && $.MvcSheetUI.GetControlValue('FTUFile').AttachmentIds == "") {
                alert("表单验证不通过!");
                return false;}
            var that = this;
            $.MvcSheet.ConfirmAction(SheetLanguages.Current.ConfirmDo + "【" + text + "】" + SheetLanguages.Current.Operation + "?",function() {
                $.LoadingMask.Show(SheetLanguages.Current.Sumiting);
                //增加一个提交并确认后的事件，此方法在许多需要与其它系统做集成的时候非常有用；Add by chenghs 2018-07-12
                var callResult = true;
                if ($.isFunction(that.AfterConfirmSubmit)) { //javascript函数
                    callResult = that.AfterConfirmSubmit.apply(that);
                } else if (that.AfterConfirmSubmit) { //javascript语句
                    callResult = new Function(that.AfterConfirmSubmit).apply(that);
                }
                if (callResult) {
                    var SheetPostValue = that.GetMvcPostValue(that.Action_Submit, destActivity, postValue);
                    that.PostSheet(
                        {
                            Command: that.Action_Submit,
                            MvcPostValue: JSON.stringify(SheetPostValue)
                        },
                        function(data) {
                            that.ResultHandler.apply(that, [actionControl, data]);
                        });
                } else {
                    $.LoadingMask.Hide();
                }
            });
        }
    }
</script>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">预警流程</label>
    </div>
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
                    <span id="Label11" data-type="SheetLabel" data-datafield="Dealer" style="">经销商名称</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input type="text" data-datafield="Dealer" data-type="SheetTextBox" id="ctl240303" class="" style="" data-schemacode="WarningData" data-querycode="GetList" data-outputmappings="Dealer:经销商名称,Channels:渠道分类,DealerClass:经销商分类,Province:省份,City:城市,Address:Address,Company:Company,Brand:Brand,NetSilver:NetSilver,Loan:Loan,OpenDate:OpenDate,Memo:Memo,License:License,Register:Register,RegisterDate:RegisterDate,OriginateDate:OriginateDate,Representative:Representative,Card:Card,Capital:Capital,OpenBank:OpenBank,AccountType:AccountType,AccountName:AccountName,Account:Account,Couplet:Couplet,CrmDealerId:CrmDealerId" data-popupwindow="PopupWindow">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="Grade" style="">评分</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control12" type="text" disabled="" data-datafield="Grade" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="Channels" style="">渠道分类</span>
                </div>
                <div id="control3" class="col-md-4">

                    <input type="text" data-datafield="Channels" disabled="" data-type="SheetTextBox" id="ctl334018" class="" style="">
                </div>
                <div id="title4" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="DealerClass" style="">经销商分类</span>
                </div>
                <div id="control4" class="col-md-4">

                    <input type="text" data-datafield="DealerClass" disabled="" data-type="SheetTextBox" id="ctl726012" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    
                <label data-datafield="Province" data-type="SheetLabel" id="ctl265503" class="" style="">省份</label></div>
                <div id="control5" class="col-md-4">
                    
                <input type="text" data-datafield="Province" disabled="" data-type="SheetTextBox" id="ctl597182" class="" style=""></div>
                <div id="title6" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="City" style="" class="">城市</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control16" type="text" disabled="" data-datafield="City" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title7" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="Address" style="">公司地址</span>
                </div>
                <div id="control7" class="col-md-10" colspan="2">
                    <input id="Control17" type="text" disabled="" data-datafield="Address" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title9" class="col-md-2">

                    <span id="Label18" data-type="SheetLabel" data-datafield="Company" style="" class="">所属集团或平台</span>
                </div>
                <div id="control9" class="col-md-4">

                    <input id="Control18" type="text" disabled="" data-datafield="Company" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title10" class="col-md-2">

                    <span id="Label19" data-type="SheetLabel" data-datafield="Brand" style="" class="">经营品牌</span>
                </div>
                <div id="control10" class="col-md-4">

                    <input id="Control19" type="text" disabled="" data-datafield="Brand" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title11" class="col-md-2">

                    <span id="Label20" data-type="SheetLabel" data-datafield="NetSilver" style="" class="">企业网银开通</span>
                </div>
                <div id="control11" class="col-md-4">

                    <input id="Control20" type="text" disabled="" data-datafield="NetSilver" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title12" class="col-md-2">

                    <span id="Label21" data-type="SheetLabel" data-datafield="Loan" style="" class="">贷款模式</span>
                </div>
                <div id="control12" class="col-md-4">

                    <input id="Control21" type="text" disabled="" data-datafield="Loan" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title13" class="col-md-2">

                    <span id="Label22" data-type="SheetLabel" data-datafield="OpenDate" style="" class="">账户开通时间</span>
                </div>
                <div id="control13" class="col-md-4">

                    <input id="Control22" type="text" disabled="" data-datafield="OpenDate" data-type="SheetTime" style="" class="">
                </div>
                <div id="space14" class="col-md-2">
                </div>
                <div id="spaceControl14" class="col-md-4">
                </div>
            </div>
            <div class="row tableContent">
                <div id="title15" class="col-md-2">
                    <span id="Label24" data-type="SheetLabel" data-datafield="Memo" style="">备注</span>
                </div>
                <div id="control15" class="col-md-10">
                    <textarea id="Control24" data-datafield="Memo" disabled="" data-type="SheetRichTextBox" style="" class="">					</textarea>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">营业执照信息</label>
            </div>
            <div class="row">
                <div id="title17" class="col-md-2">
                    <span id="Label25" data-type="SheetLabel" data-datafield="License" style="">营业执照号</span>
                </div>
                <div id="control17" class="col-md-4">
                    <input id="Control25" type="text" disabled="" data-datafield="License" data-type="SheetTextBox" style="">
                </div>
                <div id="title18" class="col-md-2">
                    <span id="Label26" data-type="SheetLabel" data-datafield="Register" style="">企业登记号</span>
                </div>
                <div id="control18" class="col-md-4">
                    <input id="Control26" type="text" disabled="" data-datafield="Register" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title19" class="col-md-2">
                    <span id="Label27" data-type="SheetLabel" data-datafield="RegisterDate" style="">登记日期</span>
                </div>
                <div id="control19" class="col-md-4">
                    <input id="Control27" type="text" disabled="" data-datafield="RegisterDate" data-type="SheetTime" style="">
                </div>
                <div id="title20" class="col-md-2">
                    <span id="Label28" data-type="SheetLabel" data-datafield="OriginateDate" style="">创立日期</span>
                </div>
                <div id="control20" class="col-md-4">
                    <input id="Control28" type="text" disabled="" data-datafield="OriginateDate" data-type="SheetTime" style="">
                </div>
            </div>
            <div class="row">
                <div id="title21" class="col-md-2">
                    <span id="Label29" data-type="SheetLabel" data-datafield="Representative" style="">法定代表人</span>
                </div>
                <div id="control21" class="col-md-4">
                    <input id="Control29" type="text" disabled="" data-datafield="Representative" data-type="SheetTextBox" style="">
                </div>
                <div id="title22" class="col-md-2">
                    <span id="Label30" data-type="SheetLabel" data-datafield="Card" style="">法人身份号</span>
                </div>
                <div id="control22" class="col-md-4">
                    <input id="Control30" type="text" disabled="" data-datafield="Card" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title23" class="col-md-2">
                    <span id="Label31" data-type="SheetLabel" data-datafield="Capital" style="">注册资金</span>
                </div>
                <div id="control23" class="col-md-4">
                    <input id="Control31" type="text" disabled="" data-datafield="Capital" data-type="SheetTextBox" style="">
                </div>
                <div id="title23" class="col-md-2" style="display: none;">
                    
                <label data-datafield="CrmDealerId" data-type="SheetLabel" id="ctl78010" class="" style="">Crm经销商Id</label></div>
                <div id="control23" class="col-md-4" style="display: none;">
                    
                <input type="text" data-datafield="CrmDealerId" data-type="SheetTextBox" id="ctl595144" class="" style=""></div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information" class="">银行开户信息</label>
            </div>
            <div class="row">
                <div id="title24" class="col-md-2">
                    <span id="Label32" data-type="SheetLabel" data-datafield="OpenBank" style="">银行开户分行</span>
                </div>
                <div id="control24" class="col-md-4">
                    <input id="Control32" type="text" disabled="" data-datafield="OpenBank" data-type="SheetTextBox" style="">
                </div>
                <div id="title25" class="col-md-2">
                    <span id="Label33" data-type="SheetLabel" data-datafield="AccountType" style="">银行账户类型</span>
                </div>
                <div id="control25" class="col-md-4">
                    <input id="Control33" type="text" disabled="" data-datafield="AccountType" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title26" class="col-md-2">
                    <span id="Label34" data-type="SheetLabel" data-datafield="AccountName" style="">银行账户名</span>
                </div>
                <div id="control26" class="col-md-4">
                    <input id="Control34" type="text" disabled="" data-datafield="AccountName" data-type="SheetTextBox" style="">
                </div>
                <div id="title27" class="col-md-2">
                    <span id="Label35" data-type="SheetLabel" data-datafield="Account" style="" class="">银行账号</span>
                </div>
                <div id="control27" class="col-md-4">
                    <input id="Control35" type="text" disabled="" data-datafield="Account" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title28" class="col-md-2">
                    <span id="Label36" data-type="SheetLabel" data-datafield="Couplet" style="" class="">联行号</span>
                </div>
                <div id="control28" class="col-md-4">
                    <input id="Control36" type="text" disabled="" data-datafield="Couplet" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information" class="">其他</label>
            </div>
            <div class="row">
                <div id="title29" class="col-md-2">
                    <span id="Label37" data-type="SheetLabel" data-datafield="WarningInfo" style="">预警信息</span>
                </div>
                <div id="control29" class="col-md-4">

                    <select data-datafield="WarningInfo" data-type="SheetDropDownList" id="ctl346223" class="" style="" data-masterdatacategory="预警信息"></select>
                </div>
            </div>
            <div class="row tableContent">
                <div id="title31" class="col-md-2">

                    <span id="Label38" data-type="SheetLabel" data-datafield="Reason" style="" class="">原因/记录</span>
                </div>
                <div id="control31" class="col-md-10">
                    <textarea id="Control38" data-datafield="Reason" data-type="SheetRichTextBox" style="" class="">					</textarea>
                </div>
            </div>
            <div class="row tableContent spyj">
                <div id="title31" class="col-md-2">
                    <span id="Label38" data-type="SheetLabel" data-datafield="Notion" style="" class="">审批意见</span>
                </div>
                <div id="control31" class="col-md-10">
                    <textarea id="Control38" data-datafield="Notion" data-type="SheetRichTextBox" style="" class="">					</textarea>
                </div>
            </div>
            <div class="row fkjc">
                <div id="title29" class="col-md-2">
                    <span id="Label37" data-type="SheetLabel" data-datafield="Decision" style="" class="">风控决策</span>
                </div>
                <div id="control29" class="col-md-4">
                    <select data-datafield="Decision" data-type="SheetDropDownList" id="ctl566805" class="" style="" data-masterdatacategory="风控决策" data-onchange="if($(this).val()==&quot;关闭账户&quot;){
$(&quot;[data-datafield='FTUFile']&quot;).parent().parent().css(&quot;display&quot;,&quot;block&quot;);
$(&quot;[data-datafield='UpdateLines']&quot;).parent().parent().css(&quot;display&quot;,&quot;none&quot;);
}
else if($(this).val()==&quot;调整额度&quot;){
$(&quot;[data-datafield='FTUFile']&quot;).parent().parent().css(&quot;display&quot;,&quot;none&quot;);
$(&quot;[data-datafield='UpdateLines']&quot;).parent().parent().css(&quot;display&quot;,&quot;block&quot;);
}
else{
$(&quot;[data-datafield='FTUFile']&quot;).parent().parent().css(&quot;display&quot;,&quot;none&quot;);
$(&quot;[data-datafield='UpdateLines']&quot;).parent().parent().css(&quot;display&quot;,&quot;none&quot;);
}"></select>
                </div>
            </div>
            <div class="row gld">
                <div id="title32" class="col-md-2">
                    <span id="Label39" data-type="SheetLabel" data-datafield="FTUFile" style="">工联单</span>
                </div>
                <div id="control32" class="col-md-10">

                    <div data-datafield="FTUFile" data-type="SheetAttachment" id="ctl335735" class="" style=""></div>
                </div>
            </div>
            <div class="row gxed">
                <div id="title33" class="col-md-2">
                    <span id="Label40" data-type="SheetLabel" data-datafield="UpdateLines" style="">更新额度</span>
                </div>
                <div id="control33" class="col-md-4">

                    <input type="text" data-datafield="UpdateLines" data-type="SheetTextBox" id="ctl206902" class="" style="" data-vaildationrule="{Decision} == &quot;调整额度&quot;">
                </div>
            </div>
        </div>
    </div>
</asp:Content>