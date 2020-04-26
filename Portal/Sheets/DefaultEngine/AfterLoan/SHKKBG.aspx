<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SHKKBG.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.SHKKBG" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <script type="text/javascript" src="js/common.js?v=<%=DateTime.Now.ToString("yyyyMMdd") %>003"></script>
    <script type="text/javascript">
        var urlFileView = "<%=this.wxapp_img_url%>";
        //确认提交后事件
        $.MvcSheet.AfterConfirmSubmit = function () {
            ifcanedit();
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                var v_state_afterLoan = $.MvcSheetUI.GetControlValue("state_afterloan1");
                if (v_state_afterLoan == "取消") {
                    return afterconfirm("cancle");
                }
            }
            else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
                var v_state_afterLoan = $.MvcSheetUI.GetControlValue("state_afterloan2");
                if (v_state_afterLoan == "同意") {
                    return afterconfirm("ok");
                }
                else if (v_state_afterLoan == "取消") {
                    return afterconfirm("cancle");
                }
            }
            return true;
        }

        // 表单验证接口
        $.MvcSheet.Validate = function () {
            //if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
            //    submit_msg = "确定审核通过[" + $.MvcSheetUI.GetControlValue("customerName") + "]的[" + workflowname + "]并提交给贷后吗？";
            //}
            //else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
            //    submit_msg = "确定已操作完成[" + $.MvcSheetUI.GetControlValue("customerName") + "]的[" + workflowname + "]吗？";
            //}

            //var spyj = $("[data-datafield='SPYJ'] textarea").val();
            //if (spyj == "") {
            //    alert("请先填写审批意见");
            //    return false;
            //}

            if (!validateState()) {
                return false;
            }

            //return confirm(submit_msg);
        }

        //页面加载的事件
        $.MvcSheet.Loaded = function (sheetInfo) {
            //if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
            //    $("[data-action=\"Submit\"] span").text("提交贷后");
            //}
            //else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
            //    $("[data-action=\"Submit\"] span").text("操作完成");
            //}
            //workflowname = "还款卡变更申请";
            //cancle_msg = "确定取消[" + $.MvcSheetUI.GetControlValue("customerName") + "]的[" + workflowname + "]吗？";
            renderExtend();
        }
    </script>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">还款卡变更申请</label>
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
                    <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style="">
                    </label>
                </div>
                <div id="divOriginateDateTitle" class="col-md-2">
                    <label id="lblOriginateDateTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                </div>
                <div id="divOriginateDate" class="col-md-4">
                    <label id="lblOriginateDate" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style="">
                    </label>
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
                    <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style="">
                    </select>
                </div>
                <div id="divSequenceNoTitle" class="col-md-2">
                    <label id="lblSequenceNoTitle" data-type="SheetLabel" data-datafield="SequenceNo" data-en_us="SequenceNo" data-bindtype="OnlyVisiable" style="">流水号</label>
                </div>
                <div id="divSequenceNo" class="col-md-4">
                    <label id="lblSequenceNo" data-type="SheetLabel" data-datafield="SequenceNo" data-bindtype="OnlyData" style="">
                    </label>
                </div>
            </div>
        </div>
        <div class="nav-icon fa  fa-chevron-right bannerTitle">
            <label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
        </div>
        <div class="divContent" id="divSheet">
            <div class="row hidden">
                <div id="title1" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="id" style="">id</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input id="Control11" type="text" data-datafield="id" data-type="SheetTextBox" style="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="financialspecialistid" class="" style="">FICode</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control17" type="text" data-datafield="financialspecialistid" data-type="SheetTextBox" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="div891861" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="customerName" class="" style="">姓名</span>
                </div>
                <div id="div125804" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="customerName" data-type="SheetTextBox" class="" style="">
                </div>
                <div id="div526736" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="telnum" class="" style="">手机号</span>
                </div>
                <div id="div484802" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="telnum" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="cardUrl" class="" style="">身份证正面照片</span>
                </div>
                <div id="control3" class="col-md-4 hidden">
                    <input id="Control15" type="text" data-datafield="cardUrl" data-extend="wxappfile" data-type="SheetTextBox" class="" style="">
                </div>
                <div id="title3r" class="col-md-2">
                    <span id="Label15r" data-type="SheetLabel" data-datafield="cardTailUrl" class="" style="">身份证反面照片</span>
                </div>
                <div id="control3r" class="col-md-4 hidden">
                    <input id="Control15r" type="text" data-datafield="cardTailUrl" data-extend="wxappfile" data-type="SheetTextBox" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="bankCardUrl" style="" class="">银行卡照片</span>
                </div>
                <div id="control5" class="col-md-4 hidden">
                    <input id="Control14" type="text" data-datafield="bankCardUrl" data-extend="wxappfile" data-type="SheetTextBox" class="" style="">
                </div>
            </div>
            <div class="row">
                <div id="title6" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="newCardNumber" style="">新卡卡号</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control16" type="text" data-datafield="newCardNumber" data-type="SheetTextBox" style="">
                </div>
                <div id="title8" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="commitTime" style="">提交时间</span>
                </div>
                <div id="control8" class="col-md-4">
                    <input id="Control18" type="text" data-datafield="commitTime" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row hidden">
                <div id="title52" class="col-md-2">
                    <span id="Label142" data-type="SheetLabel" data-datafield="authorizationFile" style="" class="">扣款申请书照片</span>
                </div>
                <div id="control52" class="col-md-4 hidden">
                    <input id="Control142" type="text" data-datafield="authorizationFile" data-extend="wxappfile" data-type="SheetTextBox" class="" style="">
                </div>

                <div id="title62" class="col-md-2">
                    <span id="Label162" data-type="SheetLabel" data-datafield="stopAuthorizationFile" style="">终止划扣授权书</span>
                </div>
                <div id="control62" class="col-md-4 hidden">
                    <input id="Control162" type="text" data-datafield="stopAuthorizationFile" data-extend="wxappfile" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title7" class="col-md-2">
                    <span id="Label19" data-type="SheetLabel" data-datafield="contractId" class="" style="">合同号</span>
                </div>
                <div id="control7" class="col-md-4">
                    <input id="Control19" type="text" data-datafield="contractId" data-type="SheetTextBox" class="" style="">
                </div>
                <div id="title33" class="col-md-2">
                    <span id="Label133" data-type="SheetLabel" data-datafield="applicationnum" style="" class="">申请单号</span>
                </div>
                <div id="control33" class="col-md-4">
                    <input id="Control133" type="text" data-datafield="applicationnum" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title11" class="col-md-2">
                    <span id="Label20" data-type="SheetLabel" data-datafield="Attachment" class="" style="">附件</span>
                </div>
                <div id="control11" class="col-md-10">
                    <div id="Control20" data-datafield="Attachment" data-type="SheetAttachment" style="">
                    </div>
                </div>
            </div>
            <div class="row">
                <div id="div679092" class="col-md-2">
                    <label data-datafield="SPYJ" data-type="SheetLabel" id="ctl1282653" class="" style="">贷后意见</label>
                </div>
                <div id="div652406" class="col-md-10" colspan="3">
                    <div data-datafield="state_afterloan1" data-type="SheetRadioButtonList" id="ctl2714843" class="" style="" data-defaultselected="false" data-defaultitems="同意;取消"></div>
                    <div data-datafield="SPYJ" data-type="SheetComment" id="ctl5409073" class="" style=""></div>
                </div>
            </div>
            <div class="row">
                <div id="div6790923" class="col-md-2">
                    <label data-datafield="SPYJ2" data-type="SheetLabel" id="ctl128265" class="" style="">确认收件</label>
                </div>
                <div id="div6524063" class="col-md-10" colspan="3">
                    <div data-datafield="state_afterloan2" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultselected="false" data-defaultitems="同意;取消"></div>
                    <div data-datafield="SPYJ2" data-type="SheetComment" id="ctl540907" class="" style=""></div>
                </div>
            </div>
        </div>
</asp:Content>
