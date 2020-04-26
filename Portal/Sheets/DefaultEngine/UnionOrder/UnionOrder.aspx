<%@ Page Language="C#" AutoEventWireup="true" CodeFile="UnionOrder.aspx.cs" Inherits="OThinker.H3.Portal.Sheets.DefaultEngine.UnionOrder" EnableEventValidation="false" MasterPageFile="~/MvcSheet.master" %>

<%@ OutputCache Duration="999999" VaryByParam="T" VaryByCustom="browser" %>
<asp:Content ID="head" ContentPlaceHolderID="headContent" runat="Server">
    <script type="text/javascript">
        var firstInput = true;
        var isLeader = false;
        var keyboardHeight = 0;
        var lastPermierFlag = "";
        $.MvcSheet.Loaded = function (sheetInfo) {
            var userName = $.MvcSheetUI.SheetInfo.UserName;
            var userId = $.MvcSheetUI.SheetInfo.Originator;
            var schemaCode = $.MvcSheetUI.SheetInfo.SchemaCode;
            var rootUnitId = $.MvcSheetUI.GetElement("Relevant_DepartmentPerson").attr("data-rootunitid");
            var apiurl = "/Portal/SheetUser/LoadOrgTreeNodes?ParentID=" + rootUnitId + "&o=GU&RootUnitID=" + rootUnitId;
            console.log(apiurl);
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2" || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                $.ajax({
                    type: "GET",
                    url: apiurl,
                    dataType: "json",
                    async: false,
                    success: function (data) {
                        for (var i = 0; i < data.length; i++) {
                            if (userName == data[i].Text || userName.indexOf("刘军") >= 0) {
                                isLeader = true;
                                break;
                            }
                        }
                    }
                });
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                    if (!isLeader) {
                        $(".showUser").addClass("hidden");
                        $("#transefer").removeClass("hidden");
                    } else {
                        $.MvcSheetUI.SetControlValue("flag_departLeader", "1", 1);
                    }

                    $.ajax({
                        type: "GET",
                        url: "/Portal/Api/UnionOrderOperator?userId=" + userId + "&schemaCode=" + schemaCode,
                        dataType: "json",
                        async: false,
                        success: function (response) {
                            if (response.code == 1) {
                                var manager = $.MvcSheetUI.GetElement("Deparment_Leaders", 1).SheetUIManager();
                                manager.SetValue(response.data);
                            }
                        }
                    });
                    
                }
                if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                    if (!isLeader) {
                        $(".showUser").addClass("hidden");
                        $("#Control80").children("div:last-child").css("display", "none");
                    }
                }
            }
            var buttons = [];
            $("#dvmButtons").find("button").each(function (index, item) { buttons.push($(item).text()); console.log($(item).text()); });
            if ($.inArray("已阅", buttons) > 0 && $.inArray("取消流程", buttons) > 0) {
                $("#dvmButtons").find("button").each(function (index, item) {
                    console.log($(item).text());
                    if ($(item).text() == "取消流程") {
                        $(item).addClass("hidden");
                    }
                });
            }

            var activityCode = $.MvcSheetUI.SheetInfo.ActivityCode;

            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                var auditResultHtml = "";
                switch (activityCode) {
                    case "Activity3":
                        $.MvcSheetUI.GetElement("Deparment_Result").next().hide();
                        auditResultHtml += "<div class=\"content-wrapper\">";
                        auditResultHtml += "<div class=\"list\">";
                        auditResultHtml += "<div class=\"item item-input item-select\" style=\"padding-left:0px\">";
                        auditResultHtml += "<div>";
                        auditResultHtml += "部门审批结果";
                        auditResultHtml += "</div>";
                        auditResultHtml += "<select id='auditResult'><option value=\"\">请选择</option>";
                        auditResultHtml += "<option value=\"同意\">同意</option>";
                        auditResultHtml += "<option value=\"驳回\">驳回</option>";
                        auditResultHtml += "<option value=\"拒绝\">拒绝</option>";
                        if (isLeader) {
                            auditResultHtml += "<option value = \"办结\" > 办结</option>";
                        }
                        auditResultHtml += "</select>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        break;
                    case "Activity8":
                        $.MvcSheetUI.GetElement("Relevant_Depar_Audit").next().hide();
                        auditResultHtml += "<div class=\"content-wrapper\">";
                        auditResultHtml += "<div class=\"list\">";
                        auditResultHtml += "<div class=\"item item-input item-select\" style=\"padding-left:0px\">";
                        auditResultHtml += "<div>";
                        auditResultHtml += "相关部门审批结果";
                        auditResultHtml += "</div>";
                        auditResultHtml += "<select id='auditResult'>";
                        auditResultHtml += "<option value=\"\"></option>";
                        auditResultHtml += "<option value=\"拒绝\">拒绝</option>";
                        auditResultHtml += "<option value = \"办结\" > 办结</option>";
                        auditResultHtml += "</select>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        break;
                    case "Activity10":
                        $.MvcSheetUI.GetElement("Vice_Premier_Audit").next().hide();
                        auditResultHtml += "<div id='viceAudit' class=\"content-wrapper\">";
                        auditResultHtml += "<div class=\"list\">";
                        auditResultHtml += "<div class=\"item item-input item-select\" style=\"padding-left:0px\">";
                        auditResultHtml += "<div>";
                        auditResultHtml += "副总审批结果";
                        auditResultHtml += "</div>";
                        auditResultHtml += "<select id='auditResult'>";
                        auditResultHtml += "<option value=\"\">请选择</option>";
                        auditResultHtml += "<option value=\"同意\">同意</option>";
                        auditResultHtml += "<option value=\"驳回\">驳回</option>";
                        auditResultHtml += "<option value=\"拒绝\">拒绝</option>";
                        auditResultHtml += "<option value = \"办结\" > 办结</option>";
                        auditResultHtml += "</select>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        break;
                    case "Activity12":
                        $.MvcSheetUI.GetElement("Premier_Audit").next().hide();
                        auditResultHtml += "<div class=\"content-wrapper\">";
                        auditResultHtml += "<div class=\"list\">";
                        auditResultHtml += "<div class=\"item item-input item-select\" style=\"padding-left:0px\">";
                        auditResultHtml += "<div>";
                        auditResultHtml += "总经理审批结果";
                        auditResultHtml += "</div>";
                        auditResultHtml += "<select id='auditResult'>";
                        auditResultHtml += "<option value=\"\">请选择</option>";
                        auditResultHtml += "<option value=\"同意\">同意</option>";
                        auditResultHtml += "<option value=\"驳回\">驳回</option>";
                        auditResultHtml += "<option value=\"拒绝\">拒绝</option>";
                        auditResultHtml += "<option value = \"办结\" > 办结</option>";
                        auditResultHtml += "</select>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        break;
                    case "Activity14":
                        $.MvcSheetUI.GetElement("Persident_Audit").next().hide();
                        auditResultHtml += "<div class=\"content-wrapper\">";
                        auditResultHtml += "<div class=\"list\">";
                        auditResultHtml += "<div class=\"item item-input item-select\" style=\"padding-left:0px\">";
                        auditResultHtml += "<div>";
                        auditResultHtml += "董事长审批结果";
                        auditResultHtml += "</div>";
                        auditResultHtml += "<select id='auditResult'>";
                        auditResultHtml += "<option value=\"\">请选择</option>";
                        auditResultHtml += "<option value=\"同意\">同意</option>";
                        auditResultHtml += "<option value=\"驳回\">驳回</option>";
                        auditResultHtml += "<option value=\"拒绝\">拒绝</option>";
                        auditResultHtml += "</select>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        auditResultHtml += "</div>";
                        break;
                    default:
                        break;
                }
                if (auditResultHtml != "") {
                    $(".IPtitle").addClass("hidden");
                    $(".IPtitle").before(auditResultHtml);
                    $("#auditResult").unbind('click').bind('click', function (e) {
                        e.stopPropagation();
                    });

                    var deparAudit = "";
                    switch (activityCode) {
                        case "Activity3":
                            deparAudit = $.MvcSheetUI.GetControlValue("Deparment_Result", 1);
                            break;
                        case "Activity8":
                            deparAudit = $.MvcSheetUI.GetControlValue("Relevant_Depar_Audit", 1);;
                            break;
                        case "Activity10":
                            deparAudit = $.MvcSheetUI.GetControlValue("Vice_Premier_Audit", 1);
                            break;
                        case "Activity12":
                            deparAudit = $.MvcSheetUI.GetControlValue("Premier_Audit", 1);
                            break;
                        case "Activity14":
                            deparAudit = $.MvcSheetUI.GetControlValue("Persident_Audit", 1);
                            break;
                        default:
                            break;
                    }
                    $("#auditResult").val(deparAudit);
                }

            }
            //副总最后一位有按钮可见
            if ( activityCode == "Activity10") {
                var valuePremiers = $.MvcSheetUI.GetControlValue("Premiers_Persons", 1);
                var lastPermier = valuePremiers[valuePremiers.length - 1];
                if (lastPermier != $.MvcSheetUI.SheetInfo.UserID) {
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        //$.MvcSheetUI.GetElement("Vice_Premier_Audit").next().hide();
                        $("#viceAudit").addClass("hidden");
                    }
                    else {
                        $.MvcSheetUI.GetElement("Vice_Premier_Audit").hide();
                    }
                }
                else {
                    lastPermierFlag = "1";
                }
            }
            //alert("android=" + ionic.Platform.isIOS());
            if ($.MvcSheetUI.SheetInfo.IsMobile && ionic.Platform.isIOS()) {

                //$("textarea").unbind('click').bind('click', function (e) {
                //    $("textarea").focus();
                //});

                //$("textarea").unbind('focus').bind('focus',function (e) {
                //    var value = $(".scroll").css("transform");
                //    alert("transform1=" + value);
                //    var values = value.split(",");
                //    var height = parseInt(values[values.length - 1]);
                //    keyboardHeight = firstInput ? height : keyboardHeight;
                //    var transformcss = "translate3d(0px, " + (keyboardHeight - 300) + "px, 0px) scale(1)";
                //    $(".scroll").css("transform", transformcss);
                //    //alert("transform2=" + $(".scroll").css("transform"));
                //    if (firstInput) {
                //        firstInput = false;
                //    }
                //});

            }

            //var cloneFlow = $(".flowDetails").eq(0).clone(true);
            //$(".flowDetails").remove();
            //$("#divBasicInfo").parent().parent().after(cloneFlow);

            if ($.MvcSheetUI.SheetInfo.WorkflowVersion < 12) {
                if ($.MvcSheetUI.SheetInfo.IsMobile) {
                    $("option[value='驳回']").each(function (index, item) { $(item).remove() })
                } else {
                    $("input[value='驳回']").each(function (index, item) { $(item).parent().remove() })
                }
            }

        }


        $.MvcSheet.Validate = function () {

            var auditResult = $("#auditResult").val();
            var activeCode = $.MvcSheetUI.SheetInfo.ActivityCode;
            switch (activeCode) {
                case "Activity3":
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        $.MvcSheetUI.SetControlValue("Deparment_Result",auditResult, 1);
                    }
                    else {
                        auditResult = $.MvcSheetUI.GetControlValue("Deparment_Result", 1);
                    }
                    break;
                case "Activity8":
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        $.MvcSheetUI.SetControlValue("Relevant_Depar_Audit",auditResult, 1);
                    }
                    else {
                        auditResult = $.MvcSheetUI.GetControlValue("Relevant_Depar_Audit", 1);
                    }
                    break;
                case "Activity10":
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        $.MvcSheetUI.SetControlValue("Vice_Premier_Audit",auditResult, 1);
                    }
                    else {
                        auditResult = $.MvcSheetUI.GetControlValue("Vice_Premier_Audit", 1);
                    }
                    
                    break;
                case "Activity12":
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        $.MvcSheetUI.SetControlValue("Premier_Audit",auditResult, 1);
                    }
                    else {
                        auditResult = $.MvcSheetUI.GetControlValue("Premier_Audit", 1);
                    }
                    
                    break;
                case "Activity14":
                    if ($.MvcSheetUI.SheetInfo.IsMobile) {
                        $.MvcSheetUI.SetControlValue("Persident_Audit",auditResult, 1);
                    }
                    else {
                        auditResult = $.MvcSheetUI.GetControlValue("Persident_Audit", 1);
                    }
                    
                    break;
                default:
                    break;
            }

            

            //副总最后一位需要选择按钮
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity10") {
                var valuePremiers = $.MvcSheetUI.GetControlValue("Premiers_Persons", 1);
                var lastPermier = valuePremiers[valuePremiers.length - 1];
                if (lastPermier == $.MvcSheetUI.SheetInfo.UserID) {
                    if (auditResult == "") {
                        var message = $.MvcSheetUI.SheetInfo.IsMobile ? "请选择审批结果" : "请点选审批按钮";
                        alert(message);
                        return false;
                    }
                }
            }
            if ($.MvcSheetUI.SheetInfo.WorkflowVersion < 12) {
                if (this.IsReject != undefined && this.IsReject) {

                    if (auditResult == "拒绝" || auditResult == "办结") {
                        alert("驳回操作不能选择拒绝或办结");
                        return false;
                    }
                }
                else if ($.MvcSheetUI.SheetInfo.ActivityCode != "Activity10" && $.MvcSheetUI.SheetInfo.ActivityCode != "Activity8") {
                    if (auditResult == "") {
                        var message = $.MvcSheetUI.SheetInfo.IsMobile ? "请选择审批结果" : "请点选审批按钮";
                        alert(message);
                        return false;
                    }
                }
            }
            else {
                if (this.IsReject != undefined && this.IsReject) {

                    //只有Activity10 且 不是最后一位领导  或者 Activity8 驳回时 ，才可不选驳回结果
                    if (($.MvcSheetUI.SheetInfo.ActivityCode == "Activity10" && lastPermierFlag != "1") || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {

                    } else {
                        if (auditResult == "拒绝" || auditResult == "办结" || auditResult == "同意") {
                            alert("驳回操作只能选择驳回");
                            return false;
                        }
                    }

                }
                else if ($.MvcSheetUI.SheetInfo.ActivityCode != "Activity8") {
                    if (($.MvcSheetUI.SheetInfo.ActivityCode == "Activity10" && lastPermierFlag == "1") || $.MvcSheetUI.SheetInfo.ActivityCode != "Activity10") {
                        if (auditResult == "") {
                            var message = $.MvcSheetUI.SheetInfo.IsMobile ? "请选择审批结果" : "请点选审批按钮";
                            alert(message);
                            return false;
                        }
                        if (auditResult == "驳回") {
                            alert("请选择驳回操作");
                            return false;
                        }
                    }
                }
            }
            
            var orderType = $.MvcSheetUI.GetControlValue("Order_Type", 1);

            if (isLeader) {
                if ((activeCode == "Activity2" || activeCode == "Activity3" )&& (orderType == "0025" || orderType == "0031") ) {
                    var departPersons = $.MvcSheetUI.GetControlValue("Relevant_DepartmentPerson", 1);
                    if ($.inArray("753f9db5-4eaf-490e-8d5a-309472614caf", departPersons) < 0) {
                        alert("请选择相关部门领导：王成");
                        return false;
                    }
                }
                var department = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.OUName"].V;
                if ( department !="财务部" && (activeCode == "Activity2" || activeCode == "Activity3" )&& (orderType == "0012" || orderType == "0009"|| orderType == "0011"|| orderType == "0034") ) {
                    var departPersons = $.MvcSheetUI.GetControlValue("Relevant_DepartmentPerson", 1);
                    if ($.inArray("f837a96b-1130-4afa-9d56-32f59585ab8a", departPersons) < 0) {
                        alert("请选择相关部门领导：南迪");
                        return false;
                    }
                }
                //if ((activeCode == "Activity2" || activeCode == "Activity3" )&& (orderType == "0009" || orderType == "0011"|| orderType == "0034") ) {
                //    var permiers = $.MvcSheetUI.GetControlValue("Premiers_Persons", 1);
                //    if ($.inArray("85247a44-b85a-403b-8949-3266e126a377", permiers) < 0) {
                //        alert("请选择副总经理：王平");
                //        return false;
                //    }
                //}
            }

            //if ((isLeader || activeCode == "Activity3") && (orderType == "0025" || orderType == "0031") && (isLeader)) {
            //    var departPersons = $.MvcSheetUI.GetControlValue("Relevant_DepartmentPerson", 1);
            //    if ($.inArray("753f9db5-4eaf-490e-8d5a-309472614caf", departPersons) < 0) {
            //        alert("请选择相关部门领导：王成");
            //        return false;
            //    }
            //}

            var value = $.MvcSheetUI.GetControlValue("Relevant_DepartmentPerson", 1);
            var valuePremiers = $.MvcSheetUI.GetControlValue("Premiers_Persons", 1);
            if (($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2" && isLeader) || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
                if (value != "") {
                    $.MvcSheetUI.SetControlValue("flag_selectRDP", "1", 1);
                } else {
                    $.MvcSheetUI.SetControlValue("flag_selectRDP", "0", 1);
                }
                if (valuePremiers != "") {
                    $.MvcSheetUI.SetControlValue("flag_selectPP", "1", 1);
                } else {
                    $.MvcSheetUI.SetControlValue("flag_selectPP", "0", 1);
                }
            }


            return true;
        }
        $.MvcSheet.AfterConfirmSubmit = function () {
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
                var auditResult = $.MvcSheetUI.GetControlValue("Relevant_Depar_Audit", 1);
                if (auditResult == "拒绝" || auditResult == "办结") {
                    var v = $.MvcSheetUI.MvcRuntime.executeService('DZBIZServiceNew', 'FinishInstance',
                        {
                            InsID: $.MvcSheetUI.SheetInfo.InstanceId
                        }
                    );
                    console.log(v);
                }
            }
            return true;
        }
        $.MvcSheet.ControlRendered = function () {

            var activityCode = $.MvcSheetUI.SheetInfo.ActivityCode;

            //副总不是最后一位的话，取消点击事件
            if (activityCode == "Activity10" && $.MvcSheetUI.SheetInfo.IsMobile) {
                if (this.DataField == "Vice_Premier_Audit") {
                    var valuePremiers = $.MvcSheetUI.GetControlValue("Premiers_Persons", 1);
                    var lastPermier = valuePremiers[valuePremiers.length - 1];
                    if (lastPermier != $.MvcSheetUI.SheetInfo.UserID) {
                        $(this.Element.parentElement).unbind('click.showChoice');
                    }
                }
            }

            //if ($.MvcSheetUI.SheetInfo.IsMobile && this.Type == "SheetComment") {
                //var that = this;
                //var ionic = $.MvcSheetUI.IonicFramework;
                //$(".LatestCommentPanel").unbind('click.chooseComments').bind('click.chooseComments', function (e) {
                //    e.stopPropagation();
                //    ionic.$ionicModal.fromTemplateUrl('/Portal/Mobile/form/templates/radio_popover2.html', {
                //        scope: ionic.$scope
                //    }).then(function (popover) {

                //        //console.log(popover.scope.$parent == ionic.$scope);
                //        //console.log(popover.scope);
                //        //console.log(ionic.$scope);

                //        popover.scope.header = SheetLanguages.Current.FreComments;
                //        popover.scope.data = {};
                //        var findex = ionic.$scope.frequentCommentIndex;
                //        popover.scope.data.RadioListValue = findex;
                //        popover.scope.RadioListDisplay = that.V.FrequentlyUsedComments;
                //        popover.show();
                //        popover.scope.hide = function () {
                //            popover.hide();
                //        }
                //        if (this.IsMobile) {
                //            $(popover.el).find(".popover").height("90%");
                //        }
                //        popover.scope.clickRadio = function (value, index) {
                //            that.CommentInput.val(value);
                //            that.Validate();
                //            ionic.$scope.frequentCommentIndex = index;
                //        };
                //        popover.scope.searchFocus = false;
                //        popover.scope.searchAnimate = function () {
                //            popover.scope.searchFocus = !popover.scope.searchFocus;
                //        };
                //        popover.scope.searChange = function () {
                //            popover.scope.searchNum = $(".active .popover .list").children('label').length;

                //        };
                //    });
                //    return;
                //});
            //}
        }

    </script>
</asp:Content>
<asp:Content ID="menu" ContentPlaceHolderID="cphMenu" runat="Server">
</asp:Content>
<asp:Content ID="master" ContentPlaceHolderID="masterContent" runat="Server">
    <div style="text-align: center;" class="DragContainer">
        <label id="lblTitle" class="panel-title">工联单</label>
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
                    <label id="lblFullName" data-type="SheetLabel" data-datafield="Originator.UserName" data-bindtype="OnlyData" style="" class=""></label>
                </div>
                <div id="divOriginateTimeTitle" class="col-md-2">
                    <label id="lblOriginateTimeTitle" data-type="SheetLabel" data-datafield="OriginateTime" data-en_us="Originate Date" data-bindtype="OnlyVisiable" style="">发起时间</label>
                </div>
                <div id="divOriginateTime" class="col-md-4">
                    <label id="lblOriginateTime" data-type="SheetLabel" data-datafield="OriginateTime" data-bindtype="OnlyData" style="" class=""></label>
                </div>
            </div>
            <div class="row">
                <div id="divOriginateOUNameTitle" class="col-md-2">
                    <label id="lblOriginateOUNameTitle" data-type="SheetLabel" data-datafield="Originator.OUName" data-en_us="Originate OUName" data-bindtype="OnlyVisiable" style="">部门</label>
                </div>
                <div id="divOriginateOUName" class="col-md-4">
                    <!--					<label id="lblOriginateOUName" data-type="SheetLabel" data-datafield="Originator.OUName" data-bindtype="OnlyData">
                    <span class="OnlyDesigner">Originator.OUName</span>
                                        </label>-->
                    <select data-datafield="Originator.OUName" data-type="SheetOriginatorUnit" id="ctlOriginaotrOUName" class="" style=""></select>
                </div>

                <div id="title2" class="col-md-2">
                    <span id="Label12" data-type="SheetLabel" style="" class="">职位</span>
                </div>
                <div id="control2" class="col-md-4">
                    <label id="lblOriginateAppellation" data-type="SheetLabel" data-datafield="Originator.Appellation" data-bindtype="OnlyData" style="" class=""></label>
                </div>
            </div>
            <div class="nav-icon fa  fa-chevron-right bannerTitle">
                <label id="divSheetInfo" data-en_us="Sheet information" class="">表单信息</label>
            </div>
            <div class="divContent" id="divSheet">
                <div class="row">
                    <div id="title5" class="col-md-2">
                        <span id="Label14" data-type="SheetLabel" data-datafield="UnionOrderName" style="" class="">事项标题</span>
                    </div>
                    <div id="control5" class="col-md-4">
                        <input id="Control14" type="text" data-datafield="UnionOrderName" maxlength="15" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title6" class="col-md-2">
                        <label data-datafield="Order_Type" data-type="SheetLabel" id="ctl129920" class="" style="">表单种类</label>
                    </div>
                    <div id="control6" class="col-md-4">
                        <select data-datafield="Order_Type" data-type="SheetDropDownList" id="ctl999502" data-masterdatacategory="工联单种类" data-queryable="true" class="" style="" data-displayemptyitem="true"></select>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title21" class="col-md-2">
                        <span id="Label24" data-type="SheetLabel" data-datafield="Application_Description" style="" class="">申请描述</span>
                    </div>
                    <div id="control21" class="col-md-10">

                        <textarea data-datafield="Application_Description" style="height: 60px; width: 100%;" data-type="SheetRichTextBox" id="ctl199238" class=""></textarea>
                    </div>
                </div>
                <div class="row showUser">
                    <div id="title4" class="col-md-2">
                        <span id="Label20" data-type="SheetLabel" data-datafield="Relevant_DepartmentPerson" style="" class="">相关部门领导</span>
                    </div>
                    <div id="control4" class="col-md-4">
                        <div id="Control20" data-datafield="Relevant_DepartmentPerson" data-type="SheetUser" style="" class="" data-groupvisible="true" data-rootselectable="false" data-rootunitid="669f91c6-5256-431e-9e09-3d2a93610ede"></div>
                    </div>
                    <div id="title22" class="col-md-6" colspan="2">
                    </div>

                </div>
                <div class="row  showUser">
                    <div id="div691896" class="col-md-2">
                        <label data-datafield="Premiers_Persons" data-type="SheetLabel" id="ctl150856" class="" style="">副总经理</label>
                    </div>
                    <div id="div670524" class="col-md-4">
                        <div data-datafield="Premiers_Persons" data-type="SheetUser" id="ctl311496" class="" style="" data-groupvisible="true" data-rootselectable="false" data-rootunitid="2a740d2a-86d1-4cf2-91d8-1b28e6515dfa"></div>
                    </div>
                    <div id="div763279" class="col-md-6" colspan="2">
                    </div>

                </div>
                <div id="transefer" class="row showUser">
                    <div id="div723709" class="col-md-2">
                        <label data-datafield="Transfers" data-type="SheetLabel" id="ctl60795" class="" style="">传阅</label>
                    </div>
                    <div id="div344685" class="col-md-4">
                        <div data-datafield="Transfers" data-type="SheetUser" id="ctl914120" class="" style="" data-rootselectable="false" data-orgunitvisible="true"></div>
                    </div>
                    <div id="div189861" class="col-md-6" colspan="2">
                        <div data-datafield="Deparment_Leaders" data-type="SheetUser" id="ctl914121" class="hidden" style="" data-rootselectable="false" data-orgunitvisible="true"></div>
                    </div>
                </div>
                <div class="row">
                    <div id="title3" class="col-md-2">
                        <span id="Label13" data-type="SheetLabel" data-datafield="Files" style="">附件</span>
                    </div>
                    <div id="control3" class="col-md-10">
                        <div id="Control13" data-datafield="Files" data-type="SheetAttachment" style="" class=""></div>
                    </div>
                </div>
                <div class="row">
                    <div id="div388690" class="col-md-2">
                        <span id="Label19" style="" class="">部门审批结果</span>
                    </div>
                    <div id="div852308" class="col-md-10">
                        <div data-datafield="Deparment_Result" data-type="SheetRadioButtonList" id="Control80" class="" style="" data-defaultitems="同意;驳回;拒绝;办结" data-defaultselected="false"></div>
                        <div id="Control19" data-datafield="Deparment_Audit" data-type="SheetComment" style="" class=""></div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title7" class="col-md-2">
                        <span id="Label15" data-type="SheetLabel" data-datafield="Relevant_Depar_AuditResult" style="" class="">相关部门审批结果</span>
                    </div>
                    <div id="control7" class="col-md-10">
                        <div data-datafield="Relevant_Depar_Audit" data-type="SheetRadioButtonList" id="Control79" class="" style="" data-defaultitems="拒绝;办结" data-defaultselected="false"></div>
                        <div id="Control15" data-datafield="Relevant_Depar_AuditResult" data-type="SheetComment" style="" class=""></div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title13" class="col-md-2">
                        <span id="Label18" data-type="SheetLabel" data-datafield="Vice_Premier_Result" style="">副总审批结果</span>
                    </div>
                    <div id="control13" class="col-md-10">
                        <div data-datafield="Vice_Premier_Audit" data-type="SheetRadioButtonList" id="Control81" class="" style="" data-defaultitems="同意;驳回;拒绝;办结" data-defaultselected="false"></div>
                        <div id="Control18" data-datafield="Vice_Premier_Result" data-type="SheetComment" style="" class=""></div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title11" class="col-md-2">
                        <span id="Label17" data-type="SheetLabel" data-datafield="Premier_Result" style="">总经理审批结果</span>
                    </div>
                    <div id="control11" class="col-md-10">
                        <div data-datafield="Premier_Audit" data-type="SheetRadioButtonList" id="Control82" class="" style="" data-defaultitems="同意;驳回;拒绝;办结" data-defaultselected="false"></div>
                        <div id="Control17" data-datafield="Premier_Result" data-type="SheetComment" style="" class=""></div>
                    </div>
                </div>
                <div class="row tableContent">
                    <div id="title9" class="col-md-2">
                        <span id="Label16" data-type="SheetLabel" data-datafield="President_Result" style="">董事长审核结果</span>
                    </div>
                    <div id="control9" class="col-md-10">
                        <div data-datafield="Persident_Audit" data-type="SheetRadioButtonList" id="Control83" class="" style="" data-defaultitems="同意;驳回;拒绝" data-defaultselected="false"></div>
                        <div id="Control16" data-datafield="President_Result" data-type="SheetComment" style=""></div>
                    </div>
                </div>
                <div class="row hidden">
                    <div id="title99" class="col-md-2">
                        <span id="Label99" data-type="SheetLabel" data-datafield="flag_departLeader" style="" class="">是否领导</span>
                    </div>
                    <div id="control99" class="col-md-4">
                        <input id="Control99" type="text" data-datafield="flag_departLeader" maxlength="15" data-type="SheetTextBox" style="" class="">
                    </div>
                    <div id="title98" class="col-md-2">
                        <label data-type="SheetLabel" id="ctl12992098" class="" style="">是否选择相关部门/多副总</label>
                    </div>
                    <div id="control98" class="col-md-4">
                        <input id="Control98" type="text" data-datafield="flag_selectRDP" maxlength="15" data-type="SheetTextBox" style="" class="">
                        <input id="Control97" type="text" data-datafield="flag_selectPP" maxlength="15" data-type="SheetTextBox" style="" class="">
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
