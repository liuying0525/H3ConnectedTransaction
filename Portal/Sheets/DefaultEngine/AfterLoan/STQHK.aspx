<%@ Page Language="C#" AutoEventWireup="true" CodeFile="STQHK.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.STQHK" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

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
            //客服节点
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                //客服取消
                var v_state = $.MvcSheetUI.GetControlValue("state_service");
                if (v_state == "取消") {//扣款成功
                    return afterconfirm("cancle");
                }
            }
            //贷后处理
            else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
                //贷后状态
                var v_state_afterLoan = $.MvcSheetUI.GetControlValue("state_afterloan");                
                if (v_state_afterLoan == "取消") {
                    return afterconfirm("cancle");
                }
                else {
                    //当前状态
                    var v_state = $.MvcSheetUI.GetControlValue("currentState");
                    if (v_state == "") {//扣款成功
                        return afterconfirm("ok");
                    }
                }                
            }
            //二次处理
            else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
                var v_state_afterLoan = $.MvcSheetUI.GetControlValue("state_afterloan");
                if (v_state_afterLoan == "取消") {
                    return afterconfirm("cancle");
                }
                else {
                    return afterconfirm("ok");
                }
            }
            return true;
        }

        // 表单验证接口
        $.MvcSheet.Validate = function () {
            //if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
            //    submit_msg = "确定审核通过[" + $.MvcSheetUI.GetControlValue("customerName") + "]的[" + workflowname + "]并提交给贷后吗？";
            //}
            //else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8" || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
            //    submit_msg = "确定已操作完成[" + $.MvcSheetUI.GetControlValue("customerName") + "]的[" + workflowname + "]吗？";
            //}

            //var spyj = $("[data-datafield*='SPYJ'] textarea").val();
            //if (spyj == "") {
            //    alert("请先填写审批意见");
            //    return false;
            //}

            if (!validateState()) {
                return false;
            }

            //return confirm(submit_msg);
            return true;
        }

        //页面加载的事件
        $.MvcSheet.Loaded = function (sheetInfo) {            
            //workflowname = "提前还款申请";
            //cancle_msg = "确定取消[" + $.MvcSheetUI.GetControlValue("customerName") + "]的[" + workflowname + "]吗？";
            
        }

    </script>
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">提前还款</label>
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
                    <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style="">
                    </select>
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
            <div class="row hidden">
                <div id="div39290" class="col-md-2">
                    <span id="Label11" data-type="SheetLabel" data-datafield="id" style="" class="">id</span>
                </div>
                <div id="div166436" class="col-md-4">
                    <input id="Control11" type="text" data-datafield="id" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="div891847" class="col-md-2">
                </div>
                <div id="div363746" class="col-md-4">
                </div>
            </div>
            <div class="row">
                <div id="title1" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" data-datafield="customerName" style="" class="">姓名</span>
                </div>
                <div id="control1" class="col-md-4">
                    <input id="Control12" type="text" data-datafield="customerName" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title2" class="col-md-2">
                    <span id="Label20" data-type="SheetLabel" data-datafield="telnum" style="" class="">手机号</span>
                </div>
                <div id="control2" class="col-md-4">
                    <input id="Control20" type="text" data-datafield="telnum" data-type="SheetTextBox" style="" class="">
                </div>
            </div>
            <div class="row">
                <div id="div573105" class="col-md-2">
                    <span id="Label19" data-type="SheetLabel" data-datafield="contractId" style="" class="">合同号</span>
                </div>
                <div id="div177860" class="col-md-4">
                    <input id="Control19" type="text" data-datafield="contractId" data-type="SheetTextBox" style="" class="">
                </div>
                <div id="title33" class="col-md-2">
                    <span id="Label133" data-type="SheetLabel" data-datafield="applicationnum" style="" class="">申请单号</span>
                </div>
                <div id="control33" class="col-md-4">
                    <input id="Control133" type="text" data-datafield="applicationnum" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title3" class="col-md-2">
                    <span id="Label13" data-type="SheetLabel" data-datafield="advanceTime" style="" class="">提前还款日期</span>
                </div>
                <div id="control3" class="col-md-4">
                    <input id="Control13" type="text" data-datafield="advanceTime" data-type="SheetTextBox" style="">
                </div>
                <div id="title31" class="col-md-2">
                    <span id="Label131" data-type="SheetLabel" data-datafield="ActualPaymentTime" style="" class="">实际提前还款日期</span>
                </div>
                <div id="control31" class="col-md-4">
                    <input id="Control131" type="text" data-datafield="ActualPaymentTime" data-type="SheetTime" style="">
                </div>
            </div>
            <div class="row">
                <div id="title4" class="col-md-2">
                    <span id="Label14" data-type="SheetLabel" data-datafield="etosprinamt" style="">未还本金</span>
                </div>
                <div id="control4" class="col-md-4">
                    <input id="Control14" type="text" data-datafield="etosprinamt" data-type="SheetTextBox" style="">
                </div>
                <div id="title41" class="col-md-2">
                    <span id="Label141" data-type="SheetLabel" data-datafield="ActualPaymentPrincipal" style="">实际未还本金</span>
                </div>
                <div id="control41" class="col-md-4">
                    <input id="Control141" type="text" data-datafield="ActualPaymentPrincipal" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title5" class="col-md-2">
                    <span id="Label15" data-type="SheetLabel" data-datafield="intrcblamt" style="">当期利息</span>
                </div>
                <div id="control5" class="col-md-4">
                    <input id="Control15" type="text" data-datafield="intrcblamt" data-type="SheetTextBox" style="">
                </div>
                <div id="title51" class="col-md-2">
                    <span id="Label151" data-type="SheetLabel" data-datafield="ActualPaymentInterest" style="">实际当期利息</span>
                </div>
                <div id="control51" class="col-md-4">
                    <input id="Control151" type="text" data-datafield="ActualPaymentInterest" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title63" class="col-md-2">
                    <span id="Label163" data-type="SheetLabel" data-datafield="interestPenalty" style="">罚息</span>
                </div>
                <div id="control63" class="col-md-4">
                    <input id="Control163" type="text" data-datafield="interestPenalty" data-type="SheetTextBox" style="">
                </div>
                <div id="title631" class="col-md-2">
                    <span id="Label1631" data-type="SheetLabel" data-datafield="ActualPenaltyInterest" style="">实际罚息</span>
                </div>
                <div id="control631" class="col-md-4">
                    <input id="Control1631" type="text" data-datafield="ActualPenaltyInterest" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title6" class="col-md-2">
                    <span id="Label16" data-type="SheetLabel" data-datafield="defaultAmount" style="">违约金</span>
                </div>
                <div id="control6" class="col-md-4">
                    <input id="Control16" type="text" data-datafield="defaultAmount" data-type="SheetTextBox" style="">
                </div>
                <div id="title61" class="col-md-2">
                    <span id="Label161" data-type="SheetLabel" data-datafield="ActualPenaltyAmount" style="">实际违约金</span>
                </div>
                <div id="control61" class="col-md-4">
                    <input id="Control161" type="text" data-datafield="ActualPenaltyAmount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title7r" class="col-md-2">
                    <span id="Label17r" data-type="SheetLabel" data-datafield="odRentalAmt" style="">逾期租金</span>
                </div>
                <div id="control7r" class="col-md-4">
                    <input id="Control17r" type="text" data-datafield="odRentalAmt" data-type="SheetTextBox" style="">
                </div>
                <div id="title71r" class="col-md-2">
                    <span id="Label171r" data-type="SheetLabel" data-datafield="ActualOdRentalAmt" style="">实际逾期租金</span>
                </div>
                <div id="control71r" class="col-md-4">
                    <input id="Control171r" type="text" data-datafield="ActualOdRentalAmt" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title7" class="col-md-2">
                    <span id="Label17" data-type="SheetLabel" data-datafield="ettotalamt" style="">提前还款金额</span>
                </div>
                <div id="control7" class="col-md-4">
                    <input id="Control17" type="text" data-datafield="ettotalamt" data-type="SheetTextBox" style="">
                </div>
                <div id="title71" class="col-md-2">
                    <span id="Label171" data-type="SheetLabel" data-datafield="ActualPaymentAmount" style="">实际提前还款金额</span>
                </div>
                <div id="control71" class="col-md-4">
                    <input id="Control171" type="text" data-datafield="ActualPaymentAmount" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div id="title8" class="col-md-2">
                    <span id="Label18" data-type="SheetLabel" data-datafield="commitTime" style="">提交时间</span>
                </div>
                <div id="control8" class="col-md-4">
                    <input id="Control18" type="text" data-datafield="commitTime" data-type="SheetTextBox" style="">
                </div>
            </div>
            <div class="row">
                <div class="col-md-2">
                    <span data-type="SheetLabel" data-datafield="currentState" style="">扣款状态</span>
                </div>
                <div class="col-md-4">
                    <select data-type="SheetDropDownList" data-datafield="currentState">
                        <option value=""></option>
                        <option value="A0009">扣款失败</option>
                        <option value="A0010">余额不足</option>
                    </select>
                </div>
            </div>
            <div class="row">
                <div id="div679092" class="col-md-2">
                    <label data-datafield="SPYJ_service" data-type="SheetLabel" id="ctl128265" class="" style="">客服意见</label>
                </div>
                <div id="div652406" class="col-md-10" colspan="3">
                    <div data-datafield="state_service" data-type="SheetRadioButtonList" id="ctl271484" class="" style="" data-defaultselected="false" data-defaultitems="同意;取消"></div>
                    <div data-datafield="SPYJ_service" data-type="SheetComment" id="ctl540907" class="" style=""></div>
                </div>
            </div>
            <div class="row">
                <div id="div6790923" class="col-md-2">
                    <label data-datafield="SPYJ_afterloan" data-type="SheetLabel" id="ctl1282653" class="" style="">贷后意见</label>
                </div>
                <div id="div6524063" class="col-md-10" colspan="3">
                    <div data-datafield="state_afterloan" data-type="SheetRadioButtonList" id="ctl2714899" class="" style="" data-defaultselected="false" data-defaultitems="同意;取消"></div>
                    <div data-datafield="SPYJ_afterloan" data-type="SheetComment" id="ctl5409073" class="" style=""></div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
