<%@ Page Language="C#" AutoEventWireup="true" CodeFile="RiskPoint.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.RiskPoint" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <script type="text/javascript">
        function ScoreAjax(len,cs) {
            $.ajax({
                type: "Post",
                url: "/Portal/GetScore/Index?action=" + cs.action + "&crmDealerId=" + cs.crmid,//wangxg 19.7
                //url: "/Portal/ajax/GetScore.ashx?action=" + cs.action + "&crmDealerId=" + cs.crmid,
                data: {action: cs.action, crmDealerId: cs.crmid},
                success: function(g) {
                    $(cs.id).val(g.split('|')[0]).next().text(g.split('|')[0]);
                    $(cs.id).parent().next().next().find("input").val(g.split('|')[1]).next().text(g.split('|')[1]);
                    count++;
                    if (count == len)
                        GetTime();
                },
                error: function () {
                    count++;
                    if (count == len)
                        GetTime();
                }
            });
        }
        var ___crmids = [];
        var count = 0;
        function GetScore(crmids) {
            count = 0;
            for (var i = 0; i < crmids.length; i++) {
                if (crmids[i].crmid == '') {
                    $(crmids[i].id).val('').next().text('');
                    count++;
                    if (count == crmids.length)
                        GetTime();
                } else {
                    ScoreAjax(crmids.length,crmids[i]);
                }
            }
        }

        function _ScoreAjax(cs) {
            $.ajax({
                type: "Post",
                url: "/Portal/GetScore/Index?action=" + cs.action + "&crmDealerId=" + cs.crmid,//wangxg 19.7
                //url: "/Portal/ajax/GetScore.ashx?action=" + cs.action + "&crmDealerId=" + cs.crmid,
                data: {action: cs.action, crmDealerId: cs.crmid},
                success: function(g) {
                    $(cs.id).val(g).next().text(g);
                },
                //error: function () {
                //},
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            });
        }
        
        function _GetScore(crmids) {
            for (var i = 0; i < crmids.length; i++) {
                if (crmids[i].crmid == '') {
                    $(crmids[i].id).val('').next().text('');
                } else {
                    _ScoreAjax(crmids[i]);
                }
            }
        }

        function GetTime() {
            setTimeout(function () {
                var $systemlist = $("#Control11").find("tr[data-row]");
                var has = false;
                if ($systemlist.length != ___crmids.length) {
                    has = true;
                } else {
                    for (var i = 0; i < $systemlist.length; i++) {
                        if ($systemlist.eq(i).find("td").eq(5).find("input").val() != ___crmids[i].crmid) {
                            has = true;
                            break;
                        }
                    }
                }
                if (has) {
                    ___crmids = [];
                    for (var i = 0; i < $systemlist.length; i++) {
                        ___crmids.push({
                            crmid: $systemlist.eq(i).find("td").eq(5).find("input").val(),
                            action: "RiskPoint1",
                            id: $systemlist.eq(i).find("td").eq(2).find("input")
                        })
                    }
                    
                    GetScore(___crmids);
                } else {
                    GetTime();
                }
            
            },1000*3);
        }
        function trun(that) {
            var strWorkItemID = $.MvcSheetUI.SheetInfo.BizObject.DataItems["RiskPointDecision"].V.R[parseInt($(that).parent().parent().find('td:first').text()) - 1].DataItems["RiskPointDecision.ChildInstanceId"].V
            var workItem = $.MvcSheetUI.SheetInfo.BizObject.DataItems["RiskPointDecision"].V.R[parseInt($(that).parent().parent().find('td:first').text()) - 1].DataItems["RiskPointDecision.RiskDecision"].V;
            //alert(strWorkItemID);
            if ((strWorkItemID == "" || strWorkItemID == null) && (workItem == "关闭账户" || workItem == "调整额度")) {
                $.ajax({
                    async: false,
                    type: "POST",
                    url: "/Portal/GetSubProcessWorkItemId/Index",//wangxg 19.7
                    //url: "/Portal/ajax/GetSubProcessWorkItemId.ashx",
                    data: { instanceId: $.MvcSheetUI.SheetInfo.BizObject.DataItems["InstanceId"].V, workItem: workItem },
                    success: function(g) {
                        var url2 = _PORTALROOT_GLOBAL + "/WorkItemSheets.html?WorkItemID=" + g;
                        window.open(url2);
                    },
                    error: function (msg) {// 19.7 
					    showJqErr(msg);
                    }
                });
            } else {
                var url = _PORTALROOT_GLOBAL + "/WorkItemSheets.html?WorkItemID=" + strWorkItemID;
                window.open(url);
            }
        }
        $.MvcSheet.Loaded = function () {
            GetTime();
            var $child = $("[data-datafield='RiskPointDecision']").find("tr[data-row]");
            for (var i = 0; i < $child.length; i++) {
                var item = $.MvcSheetUI.SheetInfo.BizObject.DataItems["RiskPointDecision"].V.R[i].DataItems["RiskPointDecision.RiskDecision"].V;
                if (item == "无需处理" || item == "其它市场部处理") $child.eq(i).find("#ChildData a").css("display", "none");
            }
            $("[data-subprocess]").click(function () {
                //用户如果没有填写序号，提示该用户 序号没有填写
                var row = $.MvcSheetUI.GetElement("RiskPointDecision").find("tr.rows").length;
                for (var i = 1; i < row; i++) {
                    var strWorkItemID = $.MvcSheetUI.SheetInfo.BizObject.DataItems["RiskPointDecision"].V.R[i].DataItems["RiskPointDecision.ChildInstanceId"].V
                    alert(strWorkItemID);
                    var url = _PORTALROOT_GLOBAL + "/WorkItemSheets.html?WorkItemID=" + strWorkItemID;
                    window.open(url);
                }
            });
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                $(".advice").hide();
                $(".gradenav").hide();
                $(".RiskDecisionnav").hide();
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity52")   //信审意见
            {
                debugger;
                $(".gradenav").hide();
                $(".RiskDecisionnav").hide();
                $("#disRisk").hide();
                var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
                var BizObjectId = $.MvcSheetUI.SheetInfo.BizObjectID;

                $.ajax({
                    //url: "/Portal/ajax/RiskDealer.ashx",
                    url: "/Portal/RiskDealer/Index",//wangxg 19.9
                    data: { 'search': BizObjectId },
                    type: "post",
                    async: true,
                    dataType: "json",
                    success: function (result) {
                        debugger;
                        for (var k = 0; k < result.List.length; k++) {
                            if (result.List[k] != "undefined") {
                                //$.MvcSheetUI.SetControlValue("Control12",result.List[k].经销商名称);
                                $("#Control12").append("<option value='" + result.List[k].经销商名称 + "'>" + result.List[k].经销商名称 + "</option>");
                            }
                        }
                    },
                    error: function (msg) {// 19.7 
					    showJqErr(msg);
                    }
                })
            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity54")    //风控评级
            {
                debugger;
                var str = $.MvcSheetUI.SheetInfo.BizObject.DataItems["RiskDealer"].V;
                str = str.replace("[", "").replace("]", "");
                //str = str.myReplace("&quot;", " ");
                var reg = new RegExp('"', "g");
                var newstr = str.replace(reg, " ");
                $("#disRisk").css("display", "block");
                $("#disRisk").text(newstr);
                $("#modalTable").removeClass("hide");
                $(".RiskDecisionnav").hide();

            }
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity56")     //风控决策
            {
                debugger;
                $("a[name='modal']").hide()
                var str = $.MvcSheetUI.SheetInfo.BizObject.DataItems["RiskDealer"].V;
                str = str.replace("[", "").replace("]", "");
                //str = str.myReplace("&quot;", " ");
                var reg = new RegExp('"', "g");
                var newstr = str.replace(reg, " ");
                $("#disRisk").css("display", "block");
                $("#disRisk").text(newstr);
                $("#modalDecision").removeClass("hide");
                $("#ChildFlow").hide();
                $("#ChildData").hide();
                $("#Control15_Header8").hide();
                //var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
            }
            else {
                $("#ChildFlow").css("display", "normal");
                $("#ChildData").css("display", "normal");
                $("#Control15_Header8").show();
            }


            String.prototype.myReplace = function (f, e) {
                var reg = new RegExp(f, "g");
                return this.replace(reg, e);
            }

        }
    </script>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">风险点干预流程</label>
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
        <div class="nav-icon fa  fa-chevron-right bannerTitle" id="formlist">
            <label id="divSheetInfo" data-en_us="Sheet information">表单信息</label>
        </div>
        <div class="divContent" id="divSheet">
            <div class="row tableContent">
                <div id="control1" class="col-md-12">
                    <table id="Control11" data-datafield="RiskPointForm" data-type="SheetGridView" class="SheetGridView">
                        <tbody>

                            <tr class="header">
                                <td id="Control11_SerialNo" class="rowSerialNo">序号								</td>
                                <td id="Control11_Header3" data-datafield="RiskPointForm.Dealer">
                                    <label id="Control11_Label3" data-datafield="RiskPointForm.Dealer" data-type="SheetLabel" style="">经销商名称</label>
                                </td>
                                <td id="Control11_Header4" data-datafield="RiskPointForm.Grade">
                                    <label id="Control11_Label4" data-datafield="RiskPointForm.Grade" data-type="SheetLabel" style="">系统评分</label>
                                </td>
                                <td id="Control11_Header5" data-datafield="RiskPointForm.Reason">
                                    <label id="Control11_Label5" data-datafield="RiskPointForm.Reason" data-type="SheetLabel" style="">原因/记录</label>
                                </td>
                                <td id="Control11_Header6" data-datafield="RiskPointForm.CollateralRate">
                                    <label id="Control11_Label6" data-datafield="RiskPointForm.CollateralRate" data-type="SheetLabel" style="">抵押率</label>
                                </td>
                                <td id="Control11_Header7" data-datafield="RiskPointForm.dealerid" style="display: none;">
                                    <label id="Control11_Label7" data-datafield="RiskPointForm.dealerid" data-type="SheetLabel" style="">dealerid</label>
                                </td>
                                <td class="rowOption">删除								</td>
                            </tr>
                            <tr class="template">
                                <td id="Control11_Option" class="rowOption"></td>
                                <td data-datafield="RiskPointForm.Dealer">
                                    <input id="Control11_ctl3" type="text" disabled="" data-datafield="RiskPointForm.Dealer" data-type="SheetTextBox" style="width: 80%;" class="" data-popupwindow="PopupWindow" data-schemacode="RiskPointData" data-querycode="GetData" data-outputmappings="RiskPointForm.Dealer:经销商名称,RiskPointForm.dealerid:CrmDealerId" data-inputmappings="RiskPointForm.Dealer:经销商名称">
                                </td>
                                <td data-datafield="RiskPointForm.Grade">
                                    <input id="Control11_ctl4" type="text" data-datafield="RiskPointForm.Grade" data-type="SheetTextBox" style="" disabled="">
                                </td>
                                <td data-datafield="RiskPointForm.Reason">
                                    <textarea id="Control11_ctl5" data-datafield="RiskPointForm.Reason" data-type="SheetTextBox" style="" class=""></textarea>
                                </td>
                                <td data-datafield="RiskPointForm.CollateralRate">
                                    <input id="Control11_ctl6" type="text" data-datafield="RiskPointForm.CollateralRate" data-type="SheetTextBox" style="" disabled="">
                                </td>
                                <td data-datafield="RiskPointForm.dealerid" style="display: none;">
                                    <input id="Control11_ctl7" type="text" data-datafield="RiskPointForm.dealerid" data-type="SheetTextBox" style="" disabled="">
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
                                <td data-datafield="RiskPointForm.Dealer" class=""></td>
                                <td data-datafield="RiskPointForm.Grade"></td>
                                <td data-datafield="RiskPointForm.Reason"></td>
                                <td data-datafield="RiskPointForm.CollateralRate"></td>
                                <td data-datafield="RiskPointForm.dealerid" style="display: none;"></td>
                                <td class="rowOption"></td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        <div class="advice">
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">信审意见</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row">
                    <div id="title3" class="col-md-2">
                        <span id="Label12" data-type="SheetLabel" data-datafield="RiskDealer" style="">风险经销商</span>
                    </div>
                    <div id="control3" class="col-md-8">
                        <select data-datafield="RiskDealer" data-type="SheetDropDownList" id="Control12" class="" style="" multiple="true"></select>
                        <label data-type="SheetLabel" id="disRisk" style="display: none" class=""></label>
                    </div>
                </div>
                <div class="row">
                    <div id="title4" class="col-md-2">
                        <span id="Label13" data-type="SheetLabel" data-datafield="Opinion" style="">信审意见</span>
                    </div>
                    <div id="control4" class="col-md-8">
                        <textarea id="Control13" data-datafield="Opinion" data-type="SheetTextBox" style="width: 86%;" class=""></textarea>
                    </div>
                </div>
            </div>
        </div>
        <div class="gradenav">
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">风控评级</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row tableContent">
                    <div id="control5" class="col-md-12">
                        <table id="Control14" data-datafield="RiskPointRate" data-type="SheetGridView" class="SheetGridView">
                            <tbody>
                                <tr class="header">
                                    <td id="Control14_SerialNo" class="rowSerialNo">序号								</td>
                                    <td id="Control14_Header3" data-datafield="RiskPointRate.Dealer">
                                        <label id="Control14_Label3" data-datafield="RiskPointRate.Dealer" data-type="SheetLabel" style="">经销商名称</label>
                                    </td>
                                    <td id="Control14_Header4" data-datafield="RiskPointRate.Grade">
                                        <label id="Control14_Label4" data-datafield="RiskPointRate.Grade" data-type="SheetLabel" style="">评分</label>
                                    </td>
                                    <td id="Control14_Header5" data-datafield="RiskPointRate.Suggest">
                                        <label id="Control14_Label5" data-datafield="RiskPointRate.Suggest" data-type="SheetLabel" style="">决策建议</label>
                                    </td>
                                    <td class="rowOption">删除								</td>
                                </tr>
                                <tr class="template">
                                    <td id="Control14_Option" class="rowOption"></td>
                                    <td data-datafield="RiskPointRate.Dealer" class="">
                                        <input id="Control14_ctl3" type="text" data-datafield="RiskPointRate.Dealer" data-type="SheetTextBox" style="width: 80%;" class="">
                                        <a href="" name="modal" id="modalTable" data-toggle="modal" class="hide" data-target="#myModal">选择</a>
                                        <!-- <a class="btn" data-toggle="modal" data-target="#myModal" id="btn_Choice" style="margin: 3px; float: right;">选择</a> -->
                                        <!-- <input id="Control14_ctl3" type="text" data-datafield="RiskPointRate.Dealer" data-target="#myModal" data-type="SheetTextBox" style="width: 80%;" class="" data-popupwindow="PopupWindow" data-schemacode="RiskPointQuery" data-querycode="RiskPointQuery" data-outputmappings="RiskPointRate.Dealer:经销商名称" data-inputmappings="RiskPointForm.Dealer:search"> -->
                                    </td>
                                    <td data-datafield="RiskPointRate.Grade">
                                        <input id="Control14_ctl4" type="text" data-datafield="RiskPointRate.Grade" data-type="SheetTextBox" style="" class="">
                                    </td>
                                    <td data-datafield="RiskPointRate.Suggest">
                                        <input id="Control14_ctl5" type="text" data-datafield="RiskPointRate.Suggest" data-type="SheetTextBox" style="" class="">
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
                                    <td data-datafield="RiskPointRate.Dealer"></td>
                                    <td data-datafield="RiskPointRate.Grade"></td>
                                    <td data-datafield="RiskPointRate.Suggest"></td>
                                    <td class="rowOption"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
        <div class="RiskDecisionnav">
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information">风控决策</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row tableContent">
                    <div id="control7" class="col-md-12">
                        <table id="Control15" data-datafield="RiskPointDecision" data-type="SheetGridView" class="SheetGridView">
                            <tbody>

                                <tr class="header">
                                    <td id="Control15_SerialNo" class="rowSerialNo">序号								</td>


                                    <td id="Control15_Header3" data-datafield="RiskPointDecision.Dealer">
                                        <label id="Control15_Label3" data-datafield="RiskPointDecision.Dealer" data-type="SheetLabel" style="">经销商名称</label>
                                    </td>
                                    <td id="Control15_Header4" data-datafield="RiskPointDecision.RiskDecision">
                                        <label id="Control15_Label4" data-datafield="RiskPointDecision.RiskDecision" data-type="SheetLabel" style="">风险决策</label>
                                    </td>
                                    <td id="Target1" data-datafield="RiskPointDecision.TargetLimit">
                                        <label id="Control15_Label5" data-datafield="RiskPointDecision.TargetLimit" data-type="SheetLabel" style="">目标额度</label>
                                    </td>
                                    <td id="Attach1" data-datafield="RiskPointDecision.AttachFile">
                                        <label id="Control15_Label6" data-datafield="RiskPointDecision.AttachFile" data-type="SheetLabel" style="">工联单附件</label>
                                    </td>
                                    <td id="Control15_Header8" data-datafield="RiskPointDecision.ChildFlow">
                                        <label id="Control15_Label99" data-datafield="RiskPointDecision.ChildFlow" data-type="SheetLabel" style="">子流程</label>
                                    </td>
                                    <td class="rowOption">删除</td>
                                </tr>
                                <tr class="template">
                                    <td id="Control15_Option" class="rowOption"></td>
                                    <td data-datafield="RiskPointDecision.Dealer">
                                        <!-- <input id="Control15_ctl3" type="text" data-datafield="RiskPointDecision.Dealer" data-type="SheetTextBox" style="width: 80%;" class="" data-popupwindow="PopupWindow" data-schemacode="RiskPointQuery" data-querycode="RiskPointQuery" data-outputmappings="RiskPointDecision.Dealer:经销商名称"> -->
                                        <input id="Control15_ctl3" type="text" data-datafield="RiskPointDecision.Dealer" data-type="SheetTextBox" style="width: 80%;" class="">
                                        <a href="#" id="modalDecision" data-toggle="modal" class="hide" data-target="#myModalDecision">选择</a>
                                    </td>
                                    <td data-datafield="RiskPointDecision.RiskDecision">
                                        <select data-datafield="RiskPointDecision.RiskDecision" data-type="SheetDropDownList" id="Control15_ctl4" class="" style="" data-masterdatacategory="风控决策" onchange="
if($(this).val() == &quot;关闭账户&quot;)
{
    $(this).parent().parent().find('[data-datafield=\'RiskPointDecision.AttachFile\']').show();
    $(this).parent().parent().find('[data-datafield=\'RiskPointDecision.TargetLimit\']').hide();
}
else if($(this).val() ==  &quot;调整额度&quot;)
{
    $(this).parent().parent().find('[data-datafield=\'RiskPointDecision.AttachFile\']').hide();
    $(this).parent().parent().find('[data-datafield=\'RiskPointDecision.TargetLimit\']').show();
}
else
{
    $(this).parent().parent().find('[data-datafield=\'RiskPointDecision.AttachFile\']').hide();
    $(this).parent().parent().find('[data-datafield=\'RiskPointDecision.TargetLimit\']').hide();
}">
                                        </select>
                                    </td>
                                    <td data-datafield="RiskPointDecision.TargetLimit" class="">
                                        <input id="Control15_ctl5" type="text" data-datafield="RiskPointDecision.TargetLimit" data-type="SheetTextBox" style="display: none;">
                                    </td>
                                    <td data-datafield="RiskPointDecision.AttachFile" class="">
                                        <div id="Control15_ctl6" data-datafield="RiskPointDecision.AttachFile" data-type="SheetAttachment" style="display: none;"></div>
                                    </td>
                                    <td id="ChildData">
                                        <a id="btnChildflow" href="javascript:void(0)" onclick="trun(this)">子流程</a>
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
                                    <td data-datafield="RiskPointDecision.Dealer"></td>
                                    <td data-datafield="RiskPointDecision.RiskDecision"></td>
                                    <td data-datafield="RiskPointDecision.TargetLimit" class="" id="Target3"></td>
                                    <td data-datafield="RiskPointDecision.AttachFile" class="" id="Attach3"></td>
                                    <td class="rowOption"></td>
                                </tr>
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>

        <!-- 风控评级模态框（Modal） -->
        <div class="modal fade" id="myModal" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static">
            <div class="modal-dialog" style="width: 800px">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                            &times;
                        </button>
                        <h4 class="modal-title" id="myModalLabel">选择</h4>
                    </div>
                    <div class="modal-body" id="tblmodal">
                    </div>
                    <div class="modal-footer">
                    </div>
                </div>
            </div>
        </div>

        <!-- 风控决策模态框（Modal） -->
        <div class="modal fade" id="myModalDecision" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static">
            <div class="modal-dialog" style="width: 800px">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">
                            &times;
                        </button>
                        <h4 class="modal-title" id="myModalLabel">选择</h4>
                    </div>
                    <div class="modal-body" id="tblDecisoin">
                    </div>
                    <div class="modal-footer" id="footerDecision">
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        //打开自定义模态框
        $("#modalTable").click(function (e) {
            debugger;
            var parentID = "";
            parentID = e.target.id;
            var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
            var BizObjectId = $.MvcSheetUI.SheetInfo.BizObjectID;

            $.ajax({
                type: "POST",
                //contentType: "application/json",
                //url: "/Portal/Webservices/RiskPoint.asmx/GetRiskPointData",
                //data: "{search:'" + BizObjectId + "'}",
                url: "/Portal/RiskPoint/Index",//wangxg 19.7
                //url: "/Portal/ajax/RiskPoint.ashx",
                data: {search:BizObjectId},
                dataType: "json",
                success: function (result) {
                    debugger;
                    var vartbody = "";
                    //if (json.length > 0) {
                    //    vartbody = vartbody + "<table class=\"table table-bordered table-hover\" style=\"margin: 15px; margin-top: 5px; width: 95%\" id=\"tblRiskdealer\"><tbody><tr class=\"header\" style=\"background-color:#F7F7F7\"><td></td><td>选择</td><td>经销商名称</td><td>渠道分类</td><td>经销商分类</td><td>省份</td><td>城市</td></tr>";
                    //    for (var i = 0; i < json.length; i++) {
                    //        var index = 1;
                    //        index = index + Number(i);
                    //        vartbody = vartbody + "<tr ondblclick=\"ChoiceData1(" + i + "," + parentID + ")\"><td>" + index + "</td><td><input type=\"checkbox\" id=\"chkchoice\" /></td><td id=dealer" + i + ">" + json[i].经销商名称 + "</td><td>" + json[i].渠道分类 + "</td><td>" + json[i].经销商分类 + "</td><td> " + json[i].省份 + "</td><td> " + json[i].城市 + "</td></tr>";
                    //    }
                    //    vartbody = vartbody + "</tbody></table>";
                    //    $("#tblmodal").html("");
                    //    $("#tblmodal").html(vartbody);
                    //} else {
                    //    $("#myModal").hide();
                    //}
                    var $name = "";
                    var $grade = $("#Control14").find(".rows");
                    for (var i = 0; i < $grade.length; i++) {
                        $name += $grade.eq(i).find("[data-datafield='RiskPointRate.Dealer']").val() + "|";
                    }
                    if (result.length > 0) {
                        vartbody = vartbody + "<table class=\"table table-bordered table-hover\" style=\"margin: 15px; margin-top: 5px; width: 95%\" id=\"tblRiskdealer\"><tbody><tr class=\"header\" style=\"background-color:#F7F7F7\"><td>经销商名称</td><td>经销商分类</td><td>省份</td><td>城市</td><td>渠道分类</td></tr>";
                        for (var i = 0; i < result.length; i++) {
                            if($name.indexOf(result[i].经销商名称) < 0)
                                vartbody = vartbody + "<tr ondblclick=\"ChoiceData1(" + i + "," + parentID + ","+result[i].CrmDealerId+")\"><td id=dealer" + i + ">" + result[i].经销商名称 + "</td><td>" + result[i].经销商分类 + "</td><td> " + result[i].省份 + "</td><td> " + result[i].城市 + "</td><td> " + result[i].渠道分类 + "</td></tr>";
                        }
                        vartbody = vartbody + "</tbody></table>";
                        $("#tblmodal").html("");
                        $("#tblmodal").html(vartbody);
                    } else {
                        $("#myModal").hide();
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            })
        })


        //保存模态框中数据到主页面
        function ChoiceData1(index, parentID,crmid) {
            debugger;
            var parindex = parentID[0].id;
            var choice = $("#dealer" + index);
            $("#" + parindex).val(choice.text()).attr("disabled","disabled");
            $("#" + parindex).parent().next().find("input").val(choice.next().text()).attr("disabled", "disabled");
            $(".close").click();
            _GetScore([
                {
                    crmid: crmid,
                    action: "RiskPoint2",
                    id: $("#" + parindex).parent().next().find("input")
                }
            ]);
        }

        //打开自定义模态框
        $("#modalDecision").click(function (e) {
            debugger;
            var parentID = "";
            parentID = e.target.id;
            var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
            var BizObjectId = $.MvcSheetUI.SheetInfo.BizObjectID;

            $.ajax({
                type: "POST",
                //contentType: "application/json",
                //url: "/Portal/Webservices/RiskPoint.asmx/GetRiskPointData",
                url: "/Portal/RiskPoint/Index",//wangxg 19.7
                //url: "/Portal/ajax/RiskPoint.ashx",
                data: {search:BizObjectId},
                dataType: "json",
                success: function (result) {
                    debugger;
                    //var json = eval(result["d"]);
                    var vartbody = "";
                    //if (json.length > 0) {
                    //    vartbody = vartbody + "<table class=\"table table-bordered table-hover\" style=\"margin: 15px; margin-top: 5px; width: 95%\" id=\"tblRiskdealer\"><tbody><tr class=\"header\" style=\"background-color:#F7F7F7\"><td></td><td>选择</td><td>经销商名称</td><td>渠道分类</td><td>经销商分类</td><td>省份</td><td>城市</td></tr>";
                    //    for (var i = 0; i < json.length; i++) {
                    //        var index = 1;
                    //        index = index + Number(i);
                    //        vartbody = vartbody + "<tr ondblclick=\"ChoiceData2(" + i + "," + parentID + ")\"><td>" + index + "</td><td><input type=\"checkbox\" id=\"chkchoice\" /></td><td id=dealer" + i + ">" + json[i].经销商名称 + "</td><td>" + json[i].渠道分类 + "</td><td>" + json[i].经销商分类 + "</td><td> " + json[i].省份 + "</td><td> " + json[i].城市 + "</td></tr>";
                    //    }
                    //    vartbody = vartbody + "</tbody></table>";
                    //    $("#tblDecisoin").html("");
                    //    $("#tblDecisoin").html(vartbody);
                    //    //setDataPage(0, 30, footerDecision, "/Portal/Webservices/RiskPoint.asmx/GetRiskPointData");
                    //} else {
                    //    $("#myModalDecision").hide();
                    //}
                    var $name = "";
                    var $grade = $("#Control15").find(".rows");
                    for (var i = 0; i < $grade.length; i++) {
                        $name += $grade.eq(i).find("[data-datafield='RiskPointDecision.Dealer']").val() + "|";
                    }
                    if (result.length > 0) {
                        vartbody = vartbody + "<table class=\"table table-bordered table-hover\" style=\"margin: 15px; margin-top: 5px; width: 95%\" id=\"tblRiskdealer\"><tbody><tr class=\"header\" style=\"background-color:#F7F7F7\"><td>经销商名称</td><td>渠道分类</td><td>经销商分类</td><td>省份</td><td>城市</td></tr>";
                        for (var i = 0; i < result.length; i++) {
                            if($name.indexOf(result[i].经销商名称) < 0)
                                vartbody = vartbody + "<tr ondblclick=\"ChoiceData2(" + i + "," + parentID + ")\"><td id=dealer" + i + ">" + result[i].经销商名称 + "</td><td>" + result[i].渠道分类 + "</td><td>" + result[i].经销商分类 + "</td><td> " + result[i].省份 + "</td><td> " + result[i].城市 + "</td></tr>";
                        }
                        vartbody = vartbody + "</tbody></table>";
                        $("#tblDecisoin").html("");
                        $("#tblDecisoin").html(vartbody);
                        //setDataPage(0, 30, footerDecision, "/Portal/Webservices/RiskPoint.asmx/GetRiskPointData");
                    } else {
                        $("#myModalDecision").hide();
                    }
                },
                error: function (msg) {// 19.7 
					 showJqErr(msg);
                }
            })
        });

        //保存模态框中数据到主页面
        function ChoiceData2(index, parentID) {
            debugger;
            var parindex = parentID[0].id;
            var choicenValue = $("#dealer" + index).text();
            $("#" + parindex).val(choicenValue).attr("disabled","disabled");
            $(".close").click();
        }
    </script>

</asp:Content>
