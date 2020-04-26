//页面加载的事件
$.MvcSheet.Loaded = function (sheetInfo) {
    ddlInit();
    //$.post('/Portal/GetProducts_Select.ashx', {}, function (ret) {
    //$.post('/Portal/GetProducts_Select/Index', {}, function (ret) {//wangxg 19.7
    //    totalAlias = JSON.parse(ret);
    //    console.log(totalAlias);
    //    modProductsAliasOption(true);
    //})

    //wangxg 19.7 added
    $.ajax({
        url: "/Portal/GetProducts_Select/Index",
        data: {  },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (ret) {
            //totalAlias = JSON.parse(ret);
            totalAlias = ret;
            console.log(totalAlias);
            modProductsAliasOption(true);
        },
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });

    if (!$.MvcSheetUI.SheetInfo.IsMobile) {
        address_li_event();
        $("#btnAddressAdd").unbind("click").bind("click", function () {
            if (event)
                event.cancelBubble = true;
            var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
            var ads_code = address_add(identifycode);
            tel_fax_add(identifycode, ads_code);
        });
        $("#btnTelAdd").unbind("click").bind("click", function () {
            if (event)
                event.cancelBubble = true;
            var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
            tel_fax_add(identifycode);
        });
        $("#btnEmployerAdd").unbind("click").bind("click", function () {
            if (event)
                event.cancelBubble = true;
            var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
            employee_add(identifycode);
        });
        $("#btnContactAdd").unbind("click").bind("click", function () {
            if (event)
                event.cancelBubble = true;
            var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
            contact_add(identifycode)
        });

        $("#btnAddressCopy").unbind("click").bind("click", function () {
            if (event)
                event.cancelBubble = true;
            copy_mainApp_address();
        });
        $("#btnEmployerCopy").unbind("click").bind("click", function () {
            if (event)
                event.cancelBubble = true;
            copy_mainApp_workInfo();
        });

        var version = $.MvcSheetUI.SheetInfo.WorkflowVersion;
        if (version < 22) {
            $("#div_Cbankname").removeClass("hidden");
        }

        //modal隐藏事件
        $('#myModal').on('hide.bs.modal', function () {
            $('#myModal').find("tr.rows").remove();
        });
        //modal显示事件
        $('#myModal').on('shown.bs.modal', function () {
            var rows = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows");
            rows.each(function (n, v) {
                if (n == 0)
                    return true;
                var id = $(v).attr("id");
                var rowIndex = $(v).attr("data-row");

                var v1 = "";
                var v2 = "";

                var app_type = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowIndex);
                var guar_type = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", rowIndex);
                if (app_type != "")//共借人
                {
                    v1 = "0";
                    v2 = app_type;
                }
                else if (guar_type != "")//擔保人
                {
                    v1 = "1";
                    v2 = guar_type;
                }

                var v3 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IS_INACTIVE_IND", rowIndex);
                var data = { id: id, v1: v1, v2: v2, v3: v3 };
                addRow(data);
            });
            $("#myModal").find("div.modal-footer").find("a").unbind("click").bind("click", function () { modalConfirm() });
        });
        //还款计划明细模态框显示后的事件；
        $('#pmt_detail').on('shown.bs.modal', function (e) {
            var v_hide_plan_id = $("#hide_plan_id").val();


            var plan_id = $.MvcSheetUI.GetControlValue("PLAN_ID");
            if (plan_id == "") {
                $('#pmt_detail .modal-body div.mycontent').empty();//清空
                $('#pmt_detail .modal-body div.table_body').append("<h1>还款计划ID为空</h1>");
            }
            else {
                if (v_hide_plan_id == plan_id) {//没有修改过Plan id不用重新取值；

                }
                else { //修改过Plan_ID，需要重新取值
                    $("#hide_plan_id").val(plan_id);
                    $('#pmt_detail .modal-body div.mycontent').empty();//清空
                    var rows = get_PMT_Detail(plan_id);
                    var table_rows = "";
                    rows.forEach(function (row, num) {
                        table_rows += "<tr>";
                        table_rows += "<td>" + row.TERM_ID + "</td>";
                        table_rows += "<td>" + row.TOTAL_RECEIVE + "</td>";
                        table_rows += "<td>" + row.PMT + "</td>";
                        table_rows += "<td>" + row.PRINCIPAL + "</td>";
                        table_rows += "<td>" + row.ASSETITC + "</td>";
                        table_rows += "<td>" + row.PMT + "</td>";
                        table_rows += "</tr>";
                    });
                    var sum = get_PMT_Sum(plan_id)[0];
                    var table_body = "<table class=\"table table-hover table-bordered\"><tbody>" + table_rows + "</tbody></table>";
                    var table_foot = "<table class=\"table table-hover table-bordered\"><tfoot><tr><th>" + sum.TERM_ID + "期</th><th></th><th>" + sum.PMT + "</th><th>" +
                        sum.PRINCIPAL + "</th><th>" + sum.ASSETITC + "</th><th>" + sum.PMT + "</th></tr></tfoot></table>";
                    $('#pmt_detail .modal-body div.table_body').append(table_body);
                    $('#pmt_detail .modal-body div.table_foot').append(table_foot);
                }
            }
        });

        //发起模式--非复制
        if ($.MvcSheetUI.QueryString("CopyID") == null && $.MvcSheetUI.SheetInfo.IsOriginateMode) {
            var id = 1;//$.MvcSheetUI.NewGuid();
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", id, 1);
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", "Y", 1);
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", "I", 1);
            $.MvcSheetUI.SetControlValue("IDENTIFICATION_ID", id);
            SetDetailValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", id, 1);
            $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.LIENEE", true, 1);//设置默认抵押人
            SetDetailValue("CONTRACT_DETAIL.IDENTIFICATION_CODE8", id, 1);
            SetDetailValue("VEHICLE_DETAIL.IDENTIFICATION_CODE7", id, 1);
            appType_change();
            //添加默认行
            $("#btnAddressAdd").click();
            $("#btnEmployerAdd").click();
            //地址栏
            $("#btnTelAdd").parent().click();
            //资产栏
            $("#titleAsset").click();
            //金融条款栏
            $("#titleJRTK").click();
            //工作信息栏
            $("#detail_Work_Title").click();
        }
        else if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {//发起模式--复制
            $.MvcSheetUI.SetControlValue("APPLICATION_NUMBER", "");
            var userName = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V;
            $.MvcSheetUI.SetControlValue("USER_NAME", userName);
        }
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
            $("div.rows").find("[id*='ctl524044_control9']").each(function (n, v) {
                asset_Condition_Change(v);
            });
            marital_chg();
            //二手车是不隐藏修改车架号；
            if (getCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, "Condition") == "U") {
                $.MvcSheetUI.GetElement("engine_number").attr("disabled", "disabled");
                $.MvcSheetUI.GetElement("vin_number").attr("disabled", "disabled");
            }
        }
        else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity67") {
            $.MvcSheetUI.GetElement("FKH").find("a.fa-download").css("cssText", "display:inline !important");
        }

        showTab();
        //获取留言
        getmsg();
        //添加留言
        $('#addmsga').on('click', function () { addmsg(); });
        //关于打印加载事件
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8" || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity16") {
            SheetLoaded();
        }
        if ($.MvcSheetUI.SheetInfo.ActivityCode != "Activity2" || $.MvcSheetUI.SheetInfo.SheetMode == $.MvcSheetUI.SheetMode.View ||
            $.MvcSheetUI.SheetInfo.SheetMode == $.MvcSheetUI.SheetMode.Print || $.MvcSheetUI.SheetInfo.WorkItemType == 4) {
            $("#btn_AddBorrower").remove();
            $("#btnAddressAdd").remove();
            $("#btnTelAdd").remove();
            $("#btnEmployerAdd").remove();
            $("#btnContactAdd").remove();

            $("#btnAddressCopy").remove();
            $("#btnEmployerCopy").remove();

            if ($.MvcSheetUI.SheetInfo.WorkItemType == 4) {
                $("#addmsg").addClass("hidden");
                $("#addmsga").addClass("hidden");
            }
        }

        //移动端，不会添加，所以在PC端再增加一行
        if ($.MvcSheetUI.GetElement("VEHICLE_DETAIL").find("div.rows").length == 0) {
            $.MvcSheetUI.GetElement("VEHICLE_DETAIL").SheetUIManager()._AddRow();
            SetDetailValue("VEHICLE_DETAIL.IDENTIFICATION_CODE7", "1", 1);
        }
        //移动端，不会添加，所以在PC端再增加一行
        if ($.MvcSheetUI.GetElement("CONTRACT_DETAIL").find("div.rows").length == 0) {
            $.MvcSheetUI.GetElement("CONTRACT_DETAIL").SheetUIManager()._AddRow();
            SetDetailValue("CONTRACT_DETAIL.IDENTIFICATION_CODE8", "1", 1);
        }
        var v = check_WriteToCAPResultCode();
        if (v != "") {
            layer.alert(v);
        }
    }
    else {
        $(".button-fun").hide();
        //隐藏复制
        $(".copy").hide();
        //隐藏地址表的title；
        $("div[data-datafield=\"ADDRESS\"] .list-title").hide();

        $("div[data-datafield=\"APPLICANT_TYPE\"]").hide();
        $("div[data-datafield=\"APPLICANT_DETAIL\"] .item-index").hide();
        $("div[data-datafield=\"ADDRESS\"] .item-index").hide();

        $("div[data-datafield=\"APPLICANT_DETAIL\"] .item-title").hide();
        $("div[data-datafield=\"ADDRESS\"] .item-title").hide();

        $("#myindex .buttons-right span").unbind("click").bind("click", function () {
            var that = this;
            if ($("div[data-datafield=\"APPLICANT_TYPE\"] .slider-slide").length == 0) {
                add_mobile_rows("A");
                set_index();
            }
            else {
                layer.confirm('角色类型？', {
                    icon: 3,
                    title: '提示',
                    btn: ['共借人', '担保人', '取消'] //按钮
                }, function (index) {
                    add_mobile_rows("A");
                    set_index();
                    layer.msg("添加共借人成功", { icon: 1 });
                }, function (index) {
                    add_mobile_rows("G");
                    set_index();
                    layer.msg("添加担保人成功", { icon: 1 });
                }, function (index) {

                });
            }
        });
        if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {
            //添加一行
            $("#myindex .buttons-right span").click();
        }
        set_index();
    }
}

var alias = [];
var totalAlias = [];
var sysPhone = "";

function modProductsAliasOption(init) {
    alias = [];

    if (init) {
        var interval = setInterval(function () {
            var group = null;
            for (x in $.MvcSheetUI.CacheData) {
                if (x.indexOf('"group_id":"' + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FP_GROUP_ID") + '"') >= 0 && x.indexOf('"fun_M_FinancialType"') >= 0 && x.indexOf('"type"') >= 0) {
                    group = $.MvcSheetUI.CacheData[x];
                }
            }
            if (group != null && group != undefined) {
                $.MvcSheetUI.GetElement("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID").attr('onchange', 'productChange(1, this);');
                var selectValue = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID");
                $.MvcSheetUI.GetElement("CONTRACT_DETAIL.FP_GROUP_ID").attr('onchange', 'modProductsAliasOption(false);');
                var option = '';
                for (x in group) {
                    var token = {
                        ProductID: group[x].key,
                        ProductName: group[x].value,
                        ProductDescription: ''
                    }
                    for (var i = 0; i < totalAlias.length; i++) {
                        if (x == totalAlias[i].ProductID) {
                            token = totalAlias[i];
                            break;
                        }
                    }
                    alias.push(token);
                }
                console.log(alias);
                var description = '暂无描述';
                for (var i = 0; i < alias.length; i++) {
                    option += '<option value="' + alias[i].ProductID + '" ' + (alias[i].ProductID == $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID") ? "selected" : "") + '>' + alias[i].ProductName + '</option>';
                    if (alias[i].ProductID == $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID"))
                        description = alias[i].ProductDescription;
                }
                var html = '<div class="row">' +
                    '<div class="col-md-8">' +
                    '<div class="col-md-4" style="width: 16.67%">' +
                    '<span>产品别名</span>' +
                    '</div>' +
                    '<div class="col-md-8" style="width: 83%">' +
                    '<select id="productAlias" onchange="productChange(0, this);">' +
                    option +
                    '</select>' +
                    '</div>' +
                    '</div>' +
                    '</div>' +
                    '<div class="row">' +
                    '<div class="col-md-12">' +
                    '<div class="col-md-4" style="width: 11.11%">' +
                    '<span>产品描述</span>' +
                    '</div>' +
                    '<div id="productDescription" class="col-md-8" style="width: 88.89%; max-width: 88.89%;" title="' + description + '">' +
                    description +
                    '</div>' +
                    '</div>' +
                    '</div>';
                $.MvcSheetUI.GetElement("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID").parent().parent().parent().after(html);
                clearInterval(interval);
            }
        }, 100)
    } else {
        var interval = setInterval(function () {
            var group = null;
            for (x in $.MvcSheetUI.CacheData) {
                if (x.indexOf('"group_id":"' + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FP_GROUP_ID") + '"') >= 0 && x.indexOf('"fun_M_FinancialType"') >= 0 && x.indexOf('"type"') >= 0) {
                    group = $.MvcSheetUI.CacheData[x];
                }
            }
            if (group != null && group != undefined) {
                var option = '';
                for (x in group) {
                    var token = {
                        ProductID: x,
                        ProductName: group[x],
                        ProductDescription: ''
                    }
                    for (var i = 0; i < totalAlias.length; i++) {
                        if (x == totalAlias[i].ProductID) {
                            token = totalAlias[i];
                            break;
                        }
                    }
                    alias.push(token);
                }
                console.log(alias);
                var description = '暂无描述';
                console.log($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID"))
                for (var i = 0; i < alias.length; i++) {
                    option += '<option value="' + alias[i].ProductID + '" ' + (alias[i].ProductID == $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID") ? "selected" : "") + '>' + alias[i].ProductName + '</option>';
                    if (alias[i].ProductID == $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID"))
                        description = alias[i].ProductDescription;
                }
                $('#productAlias').html(option);
                $('#productAlias').val($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID"));
                $('#productDescription').html(description);
                clearInterval(interval);
            }
        }, 100)
    }
}

function productChange(type, that) {
    var value = $(that).val();
    if ($('#productAlias').val() != value) $('#productAlias').val(value);
    if ($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID") != value) $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", value);
    for (var i = 0; i < alias.length; i++) {
        if (alias[i].ProductID == value) {
            $('#productDescription').html(alias[i].ProductDescription);
            $('#productDescription').attr('title', alias[i].ProductDescription);
            break;
        }
    }
}

// 控件初始化事件
$.MvcSheet.ControlRendered = function () {
    // 如果是 SheetComment，则默认设置所有的 SheetComment 的属性\
    var sheetmode = $.MvcSheetUI.SheetInfo.SheetMode;
    if (this.Type == "SheetTextBox" && this.Editable) {
        if (this.DataField == "APPLICANT_DETAIL.FIRST_THI_NME") {
            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                var i = $("<i class=\"icon ion-camera\" style=\"right:11px;position:absolute;top:0px;padding:12px\"></i>");
                i.unbind("click.camera").bind("click.camera", function () { open_idcard_diag(this) });

                $(this.Element).after(i);//.unbind("click.camera").bind("click.camera", function () { open_idcard_diag(this) });
            }
            else {
                var i = $("<a class=\"glyphicon glyphicon-camera\" data-toggle=\"modal\" data-target=\"#modal_idcard\" style=\"right:11px;position:absolute;top:0px;padding:12px\"></a>");
                i.unbind("click.camera").bind("click.camera", function () { set_idcard_index(this) });
                $(this.Element).after(i);
            }
        }
        else if (this.DataField == "Caccountnum") {
            if ($.MvcSheetUI.SheetInfo.IsMobile) {
                var i = $("<i class=\"icon ion-camera\" style=\"right:11px;position:absolute;top:0px;padding:12px\"></i>");
                i.unbind("click.camera").bind("click.camera", function () { open_bankcard_diag(this) });

                $(this.Element).after(i);//.unbind("click.camera").bind("click.camera", function () { open_bankcard_diag(this) });
            }
            else {
                var i = $("<a class=\"glyphicon glyphicon-camera\" data-toggle=\"modal\" data-target=\"#modal_bankcard\" style=\"right:20px;position:absolute;top:0px;padding:12px\"></a>");
                $(this.Element).after(i);
            }
        }
    }
};

//确认提交后事件
$.MvcSheet.AfterConfirmSubmit = function () {
    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
        setCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, $.MvcSheetUI.SheetInfo.SchemaCode, "ProposalApproval", "");
        //不是二手车，赋值
        if (getCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, "Condition") != "U") {
            setCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, $.MvcSheetUI.SheetInfo.SchemaCode, "Condition", $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.CONDITION", 1));
        }
    }
    return true;
}
var Message;
// 表单验证接口
$.MvcSheet.Validate = function () {    
    //wangxg 19.8 关于公牌贷抵押人的验证
    var ckCompanyLienee = ck_companyLienee();
    if (ckCompanyLienee === 1) {
        shakeMsg('公牌贷抵押人必须是公司');
        return false;
    } else if (ckCompanyLienee === 2) {
        shakeMsg('抵押人不能隐藏');
        return false;
    }

    // 填写环节
    var msg = "";
    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2" || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
        if (this.Action == "Submit" || this.Action == "Reject") {
            var isdayend = checkIsDayend();
            if (!isdayend.Success) {
                shakeMsg(isdayend.Message);
                return false;
            }
        }
    }

    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
        if (this.Action == "Submit") {
            msg = check_HK_Plan();
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            var applicant_detail_rows = $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows");
            var company_detail_rows = $.MvcSheetUI.GetElement("COMPANY_DETAIL").find("div.rows");
            var address_detail_rows = $.MvcSheetUI.GetElement("ADDRESS").find("div.rows");
            //给常用字段赋值
            setCYZD(applicant_detail_rows);

            msg = validate_Ysp(applicant_detail_rows);
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            //二手车Vin号必填
            msg = validate_Vin();
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            //校验Vin号长度；
            if (!vin_change_fi($.MvcSheetUI.GetElement("vin_number")[0])) {
                return false;
            }
            msg = validate_data();
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            msg = validate_marital(applicant_detail_rows);
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            if (!validate_IdentifyCode(applicant_detail_rows, company_detail_rows)) {
                return false;
            }
            msg = validate_lienee(applicant_detail_rows, company_detail_rows);
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            msg = validate_accessory();
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            msg = validate_address(address_detail_rows);
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            msg = validate_address_phone_fax(address_detail_rows);
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            msg = validate_finacial_parameter();
            if (msg != "") {
                shakeMsg(msg);
                return false;
            }
            //msg = validate_tel_number_unique();
            //if (msg != "") {
            //    shakeMsg(msg);
            //    return false;
            //}

            if ($("[data-datafield='PMS_RENTAL_DETAIL'] tr.rows").length == 0) {
                shakeMsg("还款计划未成功生成，请尝试更换产品类型，如无法解决，请联系管理员");
                return false;
            }
            else {
                if (parseFloat($.MvcSheetUI.GetControlValue("PMS_RENTAL_DETAIL.RENTAL_AMT", 1)) != parseFloat($.MvcSheetUI.GetControlValue("PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT", 1))) {
                    shakeMsg("还款额与每期还款总额不一致，请尝试切换产品类型或金额，再调回，如无法解决，请联系管理员");
                    return false;
                }
            }

            //保存表单数据
            var DDL_DATASOURCE = save_ddl_selected_source();
            saveMessageToAttachment("DDL_DATASOURCE", DDL_DATASOURCE);
            //保存融数数据源
            var rsjson = getrsdata();
            saveMessageToAttachment("rsjson", rsjson);

            //给抵押城市赋值
            $("#Control451").val($("#ctl699489").find("option:selected").text() + $("#ctl53508").find("option:selected").text() + $("#ctl291016").find("option:selected").text());
        }
    }
    else if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity8") {
        var v = check_WriteToCAPResultCode();
        if (v != "" && this.Action == "Submit") {
            shakeMsg(v);
            return false;
        }
    }
    return true;
}

// 提交后事件
$.MvcSheet.SubmitAction.OnActionDone = function () {
    debugger;
    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {

    }
}

//人个或者机构切换的事件
function appType_change() {
    //申请类型
    var v = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    //Identify Code
    var identifyid = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", 1);
    if (v == "00001") {//个人00001
        //人员表，设置成个人
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", "I", 1);

        //机构删除
        delete_Company(identifyid);

        //个人增加一行，赋值
        var r_nbr = addRowAndSetVal("APPLICANT_DETAIL", "IDENTIFICATION_CODE2", identifyid);
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
            var haschecked = false;
            $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
                if ($(v).find("[data-datafield=\"APPLICANT_DETAIL.LIENEE\"]").prop("checked")) {
                    haschecked = true;
                    return false;
                }
            });
            if (!haschecked) {
                $.MvcSheetUI.GetElement("COMPANY_DETAIL").find("div.rows").each(function (n, v) {
                    if ($(v).find("[data-datafield=\"COMPANY_DETAIL.LIENEE\"]").prop("checked")) {
                        haschecked = true;
                        return false;
                    }
                });
            }
            if (!haschecked) {
                $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.LIENEE", true, r_nbr);//设置默认抵押人
            }
            $.MvcSheetUI.SetControlValue("APPLICATION_TYPE_NAME", $.MvcSheetUI.GetElement("APPLICATION_TYPE_CODE").find("option:selected").text());
        }
        hideBannerTitle("I");
    }
    else if (v == "00002") {//机构00002
        //人员表，设置成机构
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", "C", 1);

        //个人信息表中删除:包括联系人，工作，个人信息；
        delete_Individual(identifyid);
        delete_Contact(identifyid);
        delete_WorkExp(identifyid);

        //机构信息表中增加一行，赋值
        var r_nbr = addRowAndSetVal("COMPANY_DETAIL", "IDENTIFICATION_CODE3", identifyid);
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
            var haschecked = false;
            $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
                if ($(v).find("[data-datafield=\"APPLICANT_DETAIL.LIENEE\"]").prop("checked")) {
                    haschecked = true;
                    return false;
                }
            });
            if (!haschecked) {
                $.MvcSheetUI.GetElement("COMPANY_DETAIL").find("div.rows").each(function (n, v) {
                    if ($(v).find("[data-datafield=\"COMPANY_DETAIL.LIENEE\"]").prop("checked")) {
                        haschecked = true;
                        return false;
                    }
                });
            }
            if (!haschecked) {
                $.MvcSheetUI.SetControlValue("COMPANY_DETAIL.LIENEE", true, r_nbr);//设置默认抵押人
            }
            $.MvcSheetUI.SetControlValue("APPLICATION_TYPE_NAME", $.MvcSheetUI.GetElement("APPLICATION_TYPE_CODE").find("option:selected").text());
        }
        hideBannerTitle("C");
    }
    addressFieldShow("1", v);
    $.MvcSheetUI.GetElement("IDENTIFICATION_ID").change();
    var lis = $("#borrower ul li.active");
    if (lis.length > 0) {
        $(lis[0]).removeClass("active").find("a").tab("show");
    }
    else {
        $("#borrower ul li:eq(0) a").tab("show");
    }
}
//添加地址
function address_add(identifycode, add_default) {
    var ele = $.MvcSheetUI.GetElement("ADDRESS");
    var maxNum = 0;
    ele.find("div.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index) == identifycode) {
            var add_code = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_CODE", index);
            if (parseInt(add_code) > maxNum)
                maxNum = parseInt(add_code);
        }
    });
    var manager = ele.SheetUIManager();
    manager._AddRow();
    var rowindex = ele.find("div.rows").length;

    SetDetailValue("ADDRESS.IDENTIFICATION_CODE4", identifycode, rowindex);
    SetDetailValue("ADDRESS.ADDRESS_CODE", maxNum + 1, rowindex);

    ele.children("ul").find("li:last").removeClass("active").children("a").tab("show");//显示当前最后一个Tab页签；
    manager._DataFilter();
    return maxNum + 1;
}
//添加电话
function tel_fax_add(identifycode, add_code) {
    var ele = $.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX");
    var maxNum = 0;//同一个IdentifyCode 下最大的序号
    var all_max_num = 0;//整个表中最大的序号

    var address = $.MvcSheetUI.GetElement("ADDRESS");
    if (add_code == undefined) {
        var rows = address.find("div.rows:visible");
        if (rows.length == 0 && !$.MvcSheetUI.SheetInfo.IsOriginateMode) {
            shakeMsg("请先添加一个地址");
            return false;
        }

        add_code = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_CODE", $(rows[0]).attr("data-row"));
    }
    ele.find("tr.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        var seq_id = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_SEQ_ID", index);
        if (parseInt(seq_id) > all_max_num)
            all_max_num = parseInt(seq_id);

        if ($.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", index) == identifycode) {
            if (parseInt(seq_id) > maxNum)
                maxNum = parseInt(seq_id);
        }
    });
    var phone_seq_id;
    if (maxNum == 0)
        phone_seq_id = 1;
    else {
        phone_seq_id = all_max_num + 1;
    }
    ele.SheetUIManager()._AddRow();
    var rowindex = ele.find("tr.rows:last").attr("data-row");
    $.MvcSheetUI.SetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", identifycode, rowindex);
    $.MvcSheetUI.SetControlValue("APPLICANT_PHONE_FAX.ADDRESS_CODE5", add_code, rowindex);
    $.MvcSheetUI.SetControlValue("APPLICANT_PHONE_FAX.PHONE_SEQ_ID", phone_seq_id, rowindex);
    address_li_event();
    var num = 1;
    ele.find("tr.rows").each(function (n, v) {
        var i = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", i) == identifycode &&
            $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.ADDRESS_CODE5", i) == add_code) {
            $(v).find("td").eq(0).html(num);
            num += 1;
        }
    });
    setAddressHiddenField($("#myTab_ctl621351 li:last a")[0]);
    sort_phone_fax(identifycode, add_code);
}
//添加工作信息
function employee_add(identifycode, add_default) {
    var ele = $.MvcSheetUI.GetElement("EMPLOYER");
    var maxNum = 0;
    var rows_count = 0;
    ele.find("div.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("EMPLOYER.IDENTIFICATION_CODE6", index) == identifycode) {
            rows_count++;
            var add_code = $.MvcSheetUI.GetControlValue("EMPLOYER.EMPLOYEE_LINE_ID", index);
            if (parseInt(add_code) > maxNum)
                maxNum = parseInt(add_code);
        }
    });
    if (add_default && add_default == true) {
        if (rows_count > 0)
            return false;
    }
    var manager = ele.SheetUIManager();
    manager._AddRow();
    var rowindex = ele.find("div.rows").length;
    SetDetailValue("EMPLOYER.IDENTIFICATION_CODE6", identifycode, rowindex);
    SetDetailValue("EMPLOYER.EMPLOYEE_LINE_ID", maxNum + 1, rowindex);
    ele.children("ul").find("li:last").removeClass("active").children("a").tab("show");//显示当前最后一个Tab页签；
    manager._DataFilter();
}
//添加联系人
function contact_add(identifycode) {
    var ele = $.MvcSheetUI.GetElement("PERSONNAL_REFERENCE");
    var maxNum = 0;
    ele.find("div.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("PERSONNAL_REFERENCE.IDENTIFICATION_CODE10", index) == identifycode) {
            var add_code = $.MvcSheetUI.GetControlValue("PERSONNAL_REFERENCE.LINE_ID10", index);
            if (parseInt(add_code) > maxNum)
                maxNum = parseInt(add_code);
        }
    });

    var manager = ele.SheetUIManager();
    manager._AddRow();
    var rowindex = ele.find("div.rows").length;
    SetDetailValue("PERSONNAL_REFERENCE.IDENTIFICATION_CODE10", identifycode, rowindex);
    SetDetailValue("PERSONNAL_REFERENCE.LINE_ID10", maxNum + 1, rowindex);
    ele.children("ul").find("li:last").removeClass("active").children("a").tab("show");//显示当前最后一个Tab页签；
    manager._DataFilter();
}

//弹出框确认按钮
function modalConfirm() {
    try {//弹出确认框，有概率添加不成功，暂不清楚原因，以此来定位异常；
        var rows = new Array();
        $('#myModal').find("tr.rows").each(function (n, v) {
            var tds = $(v).find("td");
            if ($(v).attr("id")) {
                var obj = {
                    id: $(v).attr("id"),
                    v1: $(tds[0]).find("select").val(),
                    v2: $(tds[1]).find("select").val(),
                    v3: $(tds[2]).find("select").val()
                };
                rows.push(obj);
            }
            else {
                var obj = {
                    id: "",
                    v1: $(tds[0]).find("select").val(),
                    v2: $(tds[1]).find("select").val(),
                    v3: $(tds[2]).find("select").val()
                };
                rows.push(obj);
            }
        });
        var third_guarantee = false;

        var app_roles = {};
        rows.forEach(function (v, n) {
            //修改
            var rowIndex;
            if (v.id != "") {
                var row = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows[id='" + v.id + "']");
                rowIndex = row.attr("data-row");
                var displayName = "";
                var showClass = "";
                if (v.v1 == "0")
                    displayName = "共同借款人";
                else if (v.v1 == "1") {
                    displayName = "担保人";
                    third_guarantee = true;
                }
                if (v.v3 == "")
                    showClass = "glyphicon-eye-open";
                else if (v.v3 == "T")
                    showClass = "glyphicon-eye-close";
                $("#tab").find("li[id='li_" + v.id + "']").find("i:first").removeClass("glyphicon-eye-close").removeClass("glyphicon-eye-open").addClass(showClass);
                $("#tab").find("li[id='li_" + v.id + "']").find("span").html(displayName);
                var identifycode = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", rowIndex);
                app_roles["ID" + identifycode] = v.v2;
                if (v.v2 == "I") {//可能是由机构修改成个人的
                    delete_Company(identifycode);
                    addRowAndSetVal("APPLICANT_DETAIL", "IDENTIFICATION_CODE2", identifycode);


                    employee_add(identifycode, true);//添加工作经历
                }
                else if (v.v2 == "C") {//可能是由个人修改成机构的
                    delete_Individual(identifycode);
                    //以下二项,机构是没有的
                    delete_Contact(identifycode);//删除联系人
                    delete_WorkExp(identifycode);//删除工作经历

                    addRowAndSetVal("COMPANY_DETAIL", "IDENTIFICATION_CODE3", identifycode);
                }
            }
            else {//增加
                $.MvcSheetUI.GetElement("APPLICANT_TYPE").SheetUIManager()._AddRow();
                var row = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows:last");
                var id = $.MvcSheetUI.NewGuid();
                row.attr("id", id);
                rowIndex = row.attr("data-row");
                var last_identifycode = parseInt($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", rowIndex - 1));
                $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", last_identifycode + 1, rowIndex);

                var displayName = "";
                var showClass = "";
                if (v.v1 == "0")
                    displayName = "共同借款人";
                else if (v.v1 == "1") {
                    displayName = "担保人";
                    third_guarantee = true;
                }
                if (v.v3 == "")
                    showClass = "glyphicon-eye-open";
                else if (v.v3 == "T")
                    showClass = "glyphicon-eye-close";
                if (v.v2 == "I") {//个人表中增加一行，并赋值
                    addRowAndSetVal("APPLICANT_DETAIL", "IDENTIFICATION_CODE2", last_identifycode + 1);
                    //这二项机构表没有
                    contact_add(last_identifycode + 1);//添加联系人
                    employee_add(last_identifycode + 1);//添加工作经历
                }
                else if (v.v2 == "C") {//机构表中增加一行，并赋值
                    addRowAndSetVal("COMPANY_DETAIL", "IDENTIFICATION_CODE3", last_identifycode + 1);
                }
                //这二项个人和机构都有;
                var ads_code = address_add(last_identifycode + 1);
                tel_fax_add(last_identifycode + 1, ads_code);
                app_roles["ID" + (last_identifycode + 1)] = v.v2;
                var li = $("<li id=\"li_" + id + "\"><a href=\"#\" data-toggle=\"tab\"><span>" + displayName + "</span><i class=\"glyphicon " + showClass + "\"></i><i class=\"glyphicon glyphicon-trash\"></i></a></li>");
                //可见、不可见点击事件
                $(li).find("i:first").unbind("click").bind("click", function () {
                    setActiveOrNot(this);
                });
                //删除事件
                $(li).find("i:last").unbind("click").bind("click", function () {
                    var del = confirm("确认删除？");
                    if (!del)
                        return false;
                    var id = $(this).parent().parent().attr("id").replace("li_", "");
                    var rowindex = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows[id='" + id + "']").attr("data-row");
                    var identifycode = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", rowindex);
                    delete_CoBorrower_Guarantor(identifycode);

                    var isgpd = false;
                    if ($.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE") == "00001") {
                        $.MvcSheetUI.GetControlValue("APPLICANT_TYPE").forEach(function (v, n) {
                            if (v.APPLICANT_TYPE == "C") {
                                isgpd = true;
                                return false;
                            }
                        });
                    }

                    if (isgpd) {
                        //重新修改值
                        $.MvcSheetUI.SetControlValue("isgpd", "1");
                    } else {
                        $.MvcSheetUI.SetControlValue("isgpd", "0");
                    }

                });
                //Tab页签切换事件
                $(li).find("a").on('shown.bs.tab', function (e) {
                    tabChange(e);
                });
                $("#tab").append(li);
            }
            if (v.v1 == "0")//共同借款人
            {
                $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", v.v2, rowIndex);
                $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", "", rowIndex);//担保人设置为空
            }
            else if (v.v1 == "1")//担保人
            {
                $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", v.v2, rowIndex);
                $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", "", rowIndex);//共同借款人设置为空
            }
            //$.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", v.v1, rowIndex);
            //$.MvcSheetUI.SetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", v.v2, rowIndex);

            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IS_INACTIVE_IND", v.v3, rowIndex);
        });
        //删除已删除的角色
        var delIDs = $("#delids").val().split(';');
        delIDs.forEach(function (v, n) {
            if (v != "") {
                var id = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows[id='" + v + "']").find("input[data-datafield='APPLICANT_TYPE.IDENTIFICATION_CODE1']").val();
                delete_CoBorrower_Guarantor(id);
            }
        });
        $("#delids").val("");//清空删除的记录；

        $("div[data-datafield='ADDRESS'] div.rows").each(function (ads_num, ads_row) {
            var index = $(ads_row).attr("data-row");
            var id = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index);
            if (app_roles["ID" + id] == "I") {
                $.MvcSheetUI.SetControlValue("ADDRESS.REGISTERED_ADDRESS", "", index);//注册地址设为空值
                $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_TYPE_CDE", "", index);//地址类型设为空值
            }
            else if (app_roles["ID" + id] == "C") {
                $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_ID", "", index);//户籍地址设为空值
                $.MvcSheetUI.SetControlValue("ADDRESS.NATIVE_DISTRICT", "", index);//籍贯设为空值
                $.MvcSheetUI.SetControlValue("ADDRESS.BIRTHPLEASE_PROVINCE", "", index);//出生地省市县设为空值
                $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_TYPE_CDE", "00003", index);//地址类型设为办公地址

                $.MvcSheetUI.SetControlValue("ADDRESS.CurrentState", "", index);//省份设为空值
                $.MvcSheetUI.SetControlValue("ADDRESS.CurrentCity", "", index);//城市设为空值
            }
        });

        //自动给第三方担保勾选上,并触发Change事件,给GURANTEE_OPTION赋值;
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.GURANTEE_OPTION_H3").find("input:eq(0)").prop("checked", third_guarantee).change();
        $("#myModal").modal("hide");
        $.MvcSheetUI.GetElement("IDENTIFICATION_ID").change();
        var cur_lis = $("#borrower ul li.active");
        if (cur_lis.length > 0)
            cur_lis.removeClass("active").children("a").tab("show");
        else
            $("#borrower ul li:first").removeClass("active").children("a").tab("show");
        if ($("#myTab_ctl621351 li.active").length > 0) {
            setAddressHiddenField($("#myTab_ctl621351 li.active a")[0])
        }

        var isgpd = false;
        if ($.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE") == "00001") {
            $.MvcSheetUI.GetControlValue("APPLICANT_TYPE").forEach(function (v, n) {
                if (v.APPLICANT_TYPE == "C") {
                    isgpd = true;
                    return false;
                }
            });
        }

        if (isgpd) {
            //重新修改值
            $.MvcSheetUI.SetControlValue("isgpd", "1");
        } else {
            $.MvcSheetUI.SetControlValue("isgpd", "0");
        }


    }
    catch (err) {
        showMsg("添加失败，请联系管理员远程排查一下问题原因:" + err);
        console.log(err);
    }
}
//弹出框中添加一行
function addRow(data) {
    var con = $("#btnAddType");
    var template = $(con).next("table").find("tr.template");
    var newRow = template.clone().removeClass("template").addClass("rows");
    newRow.find("a").unbind("click").bind("click", function () { del_Type(this) });

    if (data) {
        newRow.attr("id", data.id);
        var tds = newRow.find("td");
        $(tds[0]).find("select").val(data.v1);
        $(tds[1]).find("select").val(data.v2);
        $(tds[2]).find("select").val(data.v3);
    }
    if ($(con).next("table").find("tr.rows").length == 0) {
        template.after(newRow);
    }
    else {
        $(con).next("table").find("tr.rows:last").after(newRow);
    }

}
//弹出框中删除一行
function del_Type(con) {
    var row = $(con).parent().parent();
    var id = row.attr("id");
    if (id && id != "") {
        $("#delids").val($("#delids").val() + id + ";");
    }
    row.remove();
}

//显示借款人、共同借款人、担保人等
function showTab() {
    var rows = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows");
    $(rows).each(function (n, v) {
        var id = $(v).attr("id");
        var rowIndex = $(v).attr("data-row");

        var mainType = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", rowIndex);

        var v1 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowIndex);
        var v2 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", rowIndex);
        var v3 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IS_INACTIVE_IND", rowIndex);

        var displayName = "";
        var showClass = "";

        if (mainType && mainType != "") {
            displayName = "借款人";
        }
        else {
            if (v1 != "")
                displayName = "共同借款人";
            else if (v2 != "")
                displayName = "担保人";
            if (v3 == "")
                showClass = "glyphicon-eye-open";
            else if (v3 == "T")
                showClass = "glyphicon-eye-close";
        }

        var li = $("<li id=\"li_" + id + "\"><a href=\"#\" data-toggle=\"tab\"><span>" + displayName + "</span><i class=\"glyphicon " + showClass + "\"></i><i class=\"glyphicon glyphicon-trash\"></i></a></li>");
        if (mainType && mainType != "") {
            $(li).find("i").remove();
            $(li).addClass("active");
        }
        else if ($.MvcSheetUI.SheetInfo.ActivityCode != "Activity2" || $.MvcSheetUI.SheetInfo.SheetMode == $.MvcSheetUI.SheetMode.View) {//View模式下不显示删除按钮
            $(li).find("i.glyphicon-trash").remove();
        }
        else {
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2" &&
                ($.MvcSheetUI.SheetInfo.SheetMode == $.MvcSheetUI.SheetMode.Originate || $.MvcSheetUI.SheetInfo.SheetMode == $.MvcSheetUI.SheetMode.Work)) {//View模式下不显示删除按钮

                //可见、不可见点击事件
                $(li).find("i:first").unbind("click").bind("click", function () {
                    var id = $(this).parent().parent().attr("id").replace("li_", "");
                    setActiveOrNot(this);
                });
                //删除事件
                $(li).find("i.glyphicon-trash").unbind("click").bind("click", function () {
                    var del = confirm("确认删除？");
                    if (!del)
                        return false;
                    var id = $(this).parent().parent().attr("id").replace("li_", "");
                    var rowindex = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows[id='" + id + "']").attr("data-row");
                    var identifycode = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", rowindex);
                    delete_CoBorrower_Guarantor(identifycode);
                });
            }
        }
        //Tab页签切换事件
        $(li).find("a").on('shown.bs.tab', function (e) {
            tabChange(e);
        });
        $("#tab").append(li);
    });
    $.MvcSheetUI.SetControlValue("IDENTIFICATION_ID", $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", 1));

    var isgpd = false;
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems["APPLICATION_TYPE_CODE"].V == "00001") {
        $.MvcSheetUI.GetControlValue("APPLICANT_TYPE").forEach(function (v, n) {
            if (v.APPLICANT_TYPE == "C") {
                isgpd = true;
                return false;
            }
        });
    }

    if (isgpd) {
        //重新修改值
        $.MvcSheetUI.SetControlValue("isgpd", "1");
    } else {
        $.MvcSheetUI.SetControlValue("isgpd", "0");
    }
}

//下拉框初始化
function ddlInit() {
    if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {
        //根据当前登录用户获取bp_id
        var bp_id = $.MvcSheetUI.MvcRuntime.executeService("DropdownListDataSource", "get_bp_id", { login_nme: $.MvcSheetUI.SheetInfo.UserCode });
        $.MvcSheetUI.SetControlValue("bp_id", bp_id);
    }
}

//根据生日计算年月
function birthdayChange(con) {
    var v = $(con).val();
    var rowIndex = "";
    if ($.MvcSheetUI.SheetInfo.IsMobile) {
        rowIndex = $(con).closest("div.slider-slide").attr("data-row");
    }
    else {
        rowIndex = $(con).closest("div.rows").attr("data-row");
    }
    var idcard_nbr = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_NBR", rowIndex);
    var card_type = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_TYP", rowIndex);
    if (card_type == "00001" && idcard_nbr != "") {
        var date_str;
        if (idcard_nbr.length == 15) {//老的身份证号码
            date_str = idcard_nbr.substr(6, 6);
        }
        else if (idcard_nbr.length == 18) {//新的身份证号码
            date_str = idcard_nbr.substr(6, 8);
        }
        var year = date_str.substr(0, 4);
        var month = date_str.substr(4, 2);
        var day = date_str.substr(6, 2);
        var date = year + "-" + month + "-" + day;
        if (v != date) {
            //shakeMsg("生日与身份证号不匹配");
            $(con).val(date);
            $(con).change();
            return false;
        }
    }

    var result = getDiffDate(v);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.AGE_IN_YEAR", result.DiffYear, rowIndex);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.AGE_IN_MONTH", result.DiffMonth, rowIndex);
    if (result.DiffYear < 18) {
        shakeMsg("未满18周岁不允许贷款");
    }
}

//附加费Change事件
function accessory_chg(con) {
    var val = $(con).val();
    if (val == "")
        return false;
    var rowindex = $(con).closest("tr.rows").attr("data-row");
    $.MvcSheetUI.GetElement("ASSET_ACCESSORY").find("tr.rows").each(function (n, v) {
        if ($(v).attr("data-row") != rowindex) {
            if (val == $.MvcSheetUI.GetControlValue("ASSET_ACCESSORY.ACCESSORY_CDE", $(v).attr("data-row"))) {
                shakeMsg("不可选择重复的附件");
                $.MvcSheetUI.SetControlValue("ASSET_ACCESSORY.ACCESSORY_CDE", "", rowindex);
            }
        }
    });
}

//电话号码Change事件
function phone_chg(con) {
    var v = $(con).val();
    if (v == "")
        return false;
    var index = $(con).closest("tr.rows").attr("data-row");
    var phone_type = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_TYPE_CDE", index);
    if (phone_type == "00003") {//手机号码
        var reg = new RegExp(/^[1][3,4,5,6,7,8,9][0-9]{9}$/);
        if (!reg.test(v)) {
            shakeMsg("请输入有效的手机号码");
            $(con).val("");
        }
    }
}

//性别修改
function sex_chg(con) {
    var v = $(con).val();
    var rowIndex = "";
    if ($.MvcSheetUI.SheetInfo.IsMobile) {
        rowIndex = $(con).closest("div.slider-slide").attr("data-row");
    }
    else {
        rowIndex = $(con).closest("div.rows").attr("data-row");
    }
    var idcard_nbr = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_NBR", rowIndex);
    var card_type = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_TYP", rowIndex);
    if (card_type == "00001" && idcard_nbr != "") {
        var num_sex;//性别的标志号
        if (idcard_nbr.length == 15) {//老的身份证号码
            num_sex = idcard_nbr.substr(13, 1);
        }
        else if (idcard_nbr.length == 18) {//新的身份证号码
            num_sex = idcard_nbr.substr(16, 1);
        }
        var t = num_sex % 2 == 0 ? "F" : "M";
        if (v != t) {
            shakeMsg("性别与身份证号不匹配");
            $(con).val(t);
        }
    }
    var title_code = (v == "M" ? "00001" : "00002");
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.TITLE_CDE", title_code, rowIndex);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.THAI_TITLE_CDE", title_code, rowIndex);
}

//根据开始居住日期计算年月
function livingFromDateChange(con) {
    var v = $(con).val();
    var rowIndex = $(con).parent().parent().parent().parent().attr("data-row");
    var result = getDiffDate(v);
    $.MvcSheetUI.SetControlValue("ADDRESS.TIME_IN_YEAR", result.DiffYear, rowIndex);
    $.MvcSheetUI.SetControlValue("ADDRESS.TIME_IN_MONTH", result.DiffMonth, rowIndex);
}

//根据出厂日期拆分年月
function releaseDateChange(con) {
    var v = $(con).val();
    var rowIndex = $(con).parent().parent().parent().parent().attr("data-row");
    var d = new Date(v);
    $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.RELEASE_YEAR", d.getFullYear(), rowIndex);
    $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.RELEASE_MONTH", d.getMonth() + 1, rowIndex);
}

//当前时间减去date的时间间隔
function getDiffDate(date) {
    var curdate = new Date();
    var birthday = new Date(date);
    var diffYear = curdate.getFullYear() - birthday.getFullYear();
    var diffMonth = curdate.getMonth() - birthday.getMonth();
    if (diffMonth < 0) {
        diffYear -= 1;
        diffMonth += 12;
    }
    return {
        DiffYear: diffYear, DiffMonth: diffMonth
    };
}

//资产状况Change事件
function asset_Condition_Change(con) {
    var v = $(con).val();
    var rowIndex = $(con).parent().parent().parent().parent().attr("data-row");

    $.MvcSheetUI.GetElement("VEHICLE_DETAIL.ENGINE", rowIndex).attr("readonly", "readonly");

    //根据资产状态来判断控制是否可编辑
    if (v == "N") {//新的
        //赋值
        $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.COLOR", "", rowIndex);
        $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.VEHICLE_AGE", "0", rowIndex);
        //设置禁用
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.COLOR", rowIndex).attr("disabled", "disabled");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.REGISTRATION_NUMBER", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.VEHICLE_AGE", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.TRANSMISSION", rowIndex).attr("disabled", "disabled");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.VEHICLE_BODY", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.STYLE", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.CYLINDER", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.ODOMETER_READING", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.WHEEL_WIDTH", rowIndex).attr("readonly", "readonly");
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.SERIES", rowIndex).attr("readonly", "readonly");

        $.MvcSheetUI.GetElement("vin_number").next().remove();
    }
    else {//已使用
        //车身颜色
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.COLOR", rowIndex).removeAttr("disabled");
        //注册号
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.REGISTRATION_NUMBER", rowIndex).removeAttr("readonly");
        //车辆使用年数
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.VEHICLE_AGE", rowIndex).removeAttr("readonly");
        //变速器
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.TRANSMISSION", rowIndex).removeAttr("disabled");
        //车身
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.VEHICLE_BODY", rowIndex).removeAttr("readonly");
        //风格
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.STYLE", rowIndex).removeAttr("readonly");
        //汽缸
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.CYLINDER", rowIndex).removeAttr("readonly");
        //里程表
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.ODOMETER_READING", rowIndex).removeAttr("readonly");
        //轮宽SERIES
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.WHEEL_WIDTH", rowIndex).removeAttr("readonly");
        //轮宽
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.SERIES", rowIndex).removeAttr("readonly");

        $.MvcSheetUI.GetElement("vin_number").after("<label class=\"InvalidText\">*</label>");
    }
}


//删除共借人/担保人
function delete_CoBorrower_Guarantor(id) {
    debugger;
    var row;
    var guarantor_num = 0;
    $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows").each(function (n, v) {
        if ($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", $(v).attr("data-row")) != "") {
            guarantor_num++;
        }
        if ($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", $(v).attr("data-row")) == id) {
            row = v;
        }
    });
    var rowindex = $(row).attr("data-row");
    var identifycode = id;
    var t = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", rowindex);
    if (t != "")
        guarantor_num--;
    if (guarantor_num == 0) {
        //自动给第三方担保勾选上,并触发Change事件,给GURANTEE_OPTION赋值;
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.GURANTEE_OPTION_H3").find("input:eq(0)").prop("checked", false).change();
    }

    if (t == "") {
        t = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowindex);
    }
    if (t == "I") {
        delete_Individual(identifycode);//删除个人表中的数据

        delete_Contact(identifycode);//删除联系人
        delete_WorkExp(identifycode);//删除工作经历
    }
    else if (t == "C") {
        delete_Company(identifycode);//删除机构表中的数据
    }

    delete_Address(identifycode);//删除地址
    delete_Phone(identifycode);//删除电话

    $("#tab").find("li[id='li_" + $(row).attr("id") + "']").remove();//删除li
    $(row).find("a.delete").click();//删除人员表对应行的数据
    $.MvcSheetUI.SetControlValue("IDENTIFICATION_ID", "");
    //$.MvcSheetUI.GetElement("IDENTIFICATION_ID").change();
    $('#borrower a:first').parent().removeClass("active").children("a").tab('show');
}

//判断行是否存在并赋值方法，返回操作行的data-row
function addRowAndSetVal(detailCode, filedCode, value) {
    var detail = $.MvcSheetUI.GetElement(detailCode);
    var index = 0;
    //判断是否存在value的行；
    var isExist = false;
    $(detail).find("div.rows").each(function (n, v) {
        var n = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue(detailCode + "." + filedCode, n) == value) {
            isExist = true;
            index = n;
            return false;
        }
    });
    if (!isExist) {
        $(detail).SheetUIManager()._AddRow();
        index = $(detail).find("div.rows:last").attr("data-row");
        SetDetailValue(detailCode + "." + filedCode, value, index);
    }
    return index;
}

//设置共借人/担保人显示或隐藏
function setActiveOrNot(i) {
    var id = $(i).parent().parent().attr("id").replace("li_", "");
    var rowIndex = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows[id='" + id + "']").attr("data-row");
    if ($(i).hasClass("glyphicon-eye-open")) {
        $(i).removeClass("glyphicon-eye-open").addClass("glyphicon-eye-close");
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IS_INACTIVE_IND", "T", rowIndex);
    }
    else {
        $(i).removeClass("glyphicon-eye-close").addClass("glyphicon-eye-open");
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IS_INACTIVE_IND", "", rowIndex);
    }
}

//隐藏BannerTitle
function hideBannerTitle(type) {
    if (type == "C") {
        $("#detail_Individual").hide();
        $("#detail_Individual_Title").hide();

        $("#detail_Work_Title").hide();
        $("#div_Detail_WorkInfo").hide();

        $("#detail_contact_Title").hide();
        $("#div_Contact_WorkInfo").hide();

        $("#detail_Company").show();
        $("#detail_Company_Title").show();
    }
    else if (type == "I") {
        $("#detail_Individual").show();
        $("#detail_Individual_Title").show();

        $("#detail_Work_Title").show();
        $("#div_Detail_WorkInfo").show();

        $("#detail_contact_Title").show();
        $("#div_Contact_WorkInfo").show();

        $("#detail_Company").hide();
        $("#detail_Company_Title").hide();
    }
}

function addressFieldShow(id, type) {
    var address = $.MvcSheetUI.GetElement("ADDRESS");
    if (address == null || address == undefined || type == undefined) {
        return false;
    }
    address.find("div.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index) == id) {
            if (type == "00001") {
                //显示家庭地址、籍贯、出生省市县
                $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_ID", index).parent().parent().parent().show();
                //隐藏国家
                $.MvcSheetUI.GetElement("ADDRESS.COUNTRY_CDE", index).parent().parent().hide();
                //地址类型
                $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_TYPE_CDE", index).parent().parent().show();
                //现居住地
                $.MvcSheetUI.GetElement("ADDRESS.CurrentState", index).parent().parent().parent().show();
                //开始居住日期
                $.MvcSheetUI.GetElement("ADDRESS.LIVING_FROM_DTE", index).parent().parent().show();
                //住宅类型
                $.MvcSheetUI.GetElement("ADDRESS.RESIDENCE_TYPE_CDE", index).parent().parent().show();
                //籍贯、出生地省市县
                $.MvcSheetUI.GetElement("ADDRESS.NATIVE_DISTRICT", index).closest("div.row").show();
                $("#ctl621351_label6_Row" + index).text("户籍省份");
                $("#ctl621351_label5_Row" + index).text("户籍城市");
                $("#ctl621351_label14_Row" + index).text("现居住地址");
            }
            else if (type == "00002") {
                //隐藏家庭地址、籍贯、出生省市县
                $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_ID", index).parent().parent().parent().hide();
                //显示国家
                $.MvcSheetUI.GetElement("ADDRESS.COUNTRY_CDE", index).parent().parent().show();
                //地址类型
                $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_TYPE_CDE", index).parent().parent().hide();
                //现居住地
                $.MvcSheetUI.GetElement("ADDRESS.CurrentState", index).parent().parent().parent().hide();
                //开始居住日期
                $.MvcSheetUI.GetElement("ADDRESS.LIVING_FROM_DTE", index).parent().parent().hide();
                //住宅类型
                $.MvcSheetUI.GetElement("ADDRESS.RESIDENCE_TYPE_CDE", index).parent().parent().hide();
                //籍贯、出生地省市县
                $.MvcSheetUI.GetElement("ADDRESS.NATIVE_DISTRICT", index).closest("div.row").hide();
                $("#ctl621351_label6_Row" + index).text("公司省份");
                $("#ctl621351_label5_Row" + index).text("公司城市");
                $("#ctl621351_label14_Row" + index).text("公司地址");
                //地址类型设为办公室地址 Office
                $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_TYPE_CDE", "00003", index);
            }
        }
    });
}

//Tab面面切换事件
function tabChange(e) {
    //e.target // 激活的标签页
    //e.relatedTarget // 前一个激活的标签页
    var id = $(e.target).parent().attr("id").replace("li_", "");
    var rowIndex = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows[id='" + id + "']").attr("data-row");
    var v = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", rowIndex);
    $.MvcSheetUI.SetControlValue("IDENTIFICATION_ID", v);
    var mainType = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", rowIndex);
    var type = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", rowIndex);
    var t = "";
    var borrow_type = "";
    if (mainType != "") {
        t = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowIndex);//mainType;
        borrow_type = "borrow";
    }
    else {
        if (type != "") {
            t = type;
            borrow_type = "guarantor";
        }
        else {
            t = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowIndex);//mainType;
            borrow_type = "co-borrow";
        }
    }
    hideBannerTitle(t);
    if (t == "I" && mainType == "") {
        var ele = $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows:visible");
        ele.find("[data-datafield='APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE']").closest("div.row").removeClass("hidden");
        //2019-01-28号：只根据人员关系表中的显示及隐藏来控制，和Spouse无关，Spouse表示是否发送给上海银行；
        //所以Spouse隐藏，不显示；
        //if (borrow_type == "co-borrow") {
        //    ele.find("[data-datafield='APPLICANT_DETAIL.SPOUSE_IND']").closest("div.checkbox").parent().parent().removeClass("hidden");
        //}
    }

    if (borrow_type == "borrow") {//主贷人不显示复制按钮
        $("#btnAddressCopy").addClass("hidden");
        $("#btnEmployerCopy").addClass("hidden");
    }
    else {
        if ($.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE") == "00001") {//个人可以复制工作信息
            $("#btnEmployerCopy").removeClass("hidden");
        }
        else {
            $("#btnEmployerCopy").addClass("hidden");//机构不可复制工作信息
        }
        $("#btnAddressCopy").removeClass("hidden");
    }
}

/*
    子表行添加事件
    参数：grid -> 表示子表对象实例
    参数：args -> 0表示子表对象实例，1表示后台返回的数据对象，2表示当前行号
    args[1] 只会在页面加载的时候有值，添加的行是没有值的
*/
function addressAddRow(grid, args) {
    return false;
}

var addressRowRemoved = function (row) {
    var id = row.find("input[data-datafield='ADDRESS.IDENTIFICATION_CODE4']").val();
    var address_code = row.find("input[data-datafield='ADDRESS.ADDRESS_CODE']").val();
    delete_Phone(id, address_code);
}

//电话删除事件
function telRowDel(row) {
    var ele = $.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX");
    var num = 1;
    ele.find("tr.rows:visible").each(function (n, v) {
        $(v).find("td").eq(0).html(num);
        num += 1;
    });
}

//默认邮寄地址，户籍地址Check变化的事件
function defaultAddressChange(code, con) {
    var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
    var curIndex = $(con).closest("div.rows").attr("data-row");
    var v = $.MvcSheetUI.GetControlValue("ADDRESS." + code, curIndex);
    var index = $.MvcSheetUI.GetElement("ADDRESS").find("div.rows").length;
    for (var n = 1; n <= index; n++) {
        if (identifycode == $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", n) && n != curIndex) {
            if (v)
                $.MvcSheetUI.GetElement("ADDRESS." + code, n).attr("disabled", "disabled");
            else
                $.MvcSheetUI.GetElement("ADDRESS." + code, n).removeAttr("disabled");
        }
    }
}

//关系下拉框Change事件
function relation_chg(con) {
    var val = $(con).val();
    var isOK = true;
    var row = $(con).closest("div.rows");
    var cur_index = row.attr("data-row");
    //给Spouse赋值    
    if (val == "00006") {//Husband-Wife配偶关系
        var state = $.MvcSheetUI.GetElement("APPLICANT_DETAIL.SPOUSE_IND", cur_index).attr("disabled");
        if (state && state == "disabled") {//表示主贷人未婚
            $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.SPOUSE_IND", false, cur_index);
        }
        else {//表示主贷人已婚
            $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.SPOUSE_IND", true, cur_index);
        }


        $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
            var index = $(v).attr("data-row");
            if (cur_index == index)
                return true;
            var r = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE", index);
            if (r == val) {
                shakeMsg(Message.Applicant_Info_18);
                isOK = false;
                return false;
            }
        });
    }
    else {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.SPOUSE_IND", false, cur_index);
    }


    if (!isOK) {
        $.MvcSheetUI.SetControlValue($(con).attr("data-datafield"), "", cur_index);
    }
}

/*
    1.主借人为个人
        a.已婚：所有共借人中只能有一个夫妻关系，Pouse（配偶）只能勾选一次；一个共借人勾选Pouse,一个共借人选择夫妻关系也行（Netsol保存）
        b.未婚：所有共借人者不能有夫妻关系，夫妻关系的前提：婚姻状况为已婚；
    2.主借人为机构
        a.所有共借人者不能有夫妻关系，夫妻关系的前提：婚姻状况为已婚；
    */

//数据校验，是否填写完整
function validate_data() {
    var msg = "";
    //判断VEHICLE_DETAIL表是否数据完整
    var model_dsc = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.POWER_PARAMETER", 1);
    var price = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", "1");

    var miocn_id = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.MIOCN_ID", "1");
    var miocn_nbr = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.MIOCN_NBR", "1");
    var miocn_dsc = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.MIOCN_DSC", "1");

    var type_cde = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.VEHICLE_TYPE_CDE", "1");
    var type_sub_cde = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.VEHICLE_SUBTYPE_CDE", "1");

    if (model_dsc == "" || price == "" || miocn_id == "" || miocn_nbr == "" || miocn_dsc == "" || type_cde == "" || type_sub_cde == "") {
        console.log("model_dsc-->" + model_dsc);
        console.log("price-->" + price);
        console.log("miocn_id-->" + miocn_id);
        console.log("miocn_nbr-->" + miocn_nbr);
        console.log("miocn_dsc-->" + miocn_dsc);
        console.log("type_cde-->" + type_cde);
        console.log("type_sub_cde-->" + type_sub_cde);
        msg = "车型信息不完整，请重新选择，如果选择车型有问题请保存后 右键--重新加载框架，还无法解决请联系管理员";
    }
    return msg;
}

function validate_marital(applicant_detail_rows) {
    //申请类型：个人或者机构
    var t = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    var msg = "";
    var app_marital_code;//主借人婚姻状况
    var app_det_index;
    var has_spouse = false;//是否有配偶
    var is_GPD = false;//是否是公片贷
    if (t == "00001") {//个人
        applicant_detail_rows.each(function (n, v_row) {
            var v = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", $(v_row).attr("data-row"));
            if (v == "1") {
                app_marital_code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.MARITAL_STATUS_CDE", $(v_row).attr("data-row"));
                app_det_index = n;
                return false;
            }
        });
        //已婚，只有一个人
        if (app_marital_code == "00001") {
            if (applicant_detail_rows.length == 1) {
                msg += "请提供配偶信息";
                return msg;
            }
        }
        else {//未婚
            has_spouse = true;
        }

        //判断是否是个人公牌贷
        //只要判断第2行的共借类型是否是机构；
        if ($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", 2) == "C") {
            is_GPD = true;
        }
    }
    else {
        has_spouse = true;//机构贷不需要控制是否有配偶；
    }

    applicant_detail_rows.each(function (n, v) {
        var index = $(v).attr("data-row");
        var year = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.AGE_IN_YEAR", index);
        if (parseFloat(year) > 99) {
            msg += "年龄不能超过99岁";
            return false;
        }
        if (parseFloat(year) < 18) {
            msg += "未满18周岁不允许贷款";
            return false;
        }
        if (n == app_det_index) {
            return true;
        }
        var identificion_code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", index);
        var info = get_app_type_info(identificion_code);
        if (info.APPLICANT_TYPE == "I") {//表示是共同借款人
            //关系，00006表示 Husband-Wife配偶关系
            var relation = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE", index);
            if (relation == "") {
                msg += "请选择共同借款人的关系";
                return false;
            }
            //共借人，个人贷，已婚，关系是配偶
            if (t == "00001" && relation == "00006" && app_marital_code == "00001") {
                has_spouse = true;
            }
            //婚姻状态：00001  已婚
            var marital_code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.MARITAL_STATUS_CDE", index);

            //1.先检查共借人关系与婚姻状态
            if (relation == "00006" && marital_code != "00001") {//如果选择了配偶但是没有选择已婚，这是错误的
                msg += Message.Applicant_Info_17;
                return false;
            }

            //2.再检查借款人与共借人关系与婚姻状态
            if (relation == "00006" && marital_code == "00001" && app_marital_code != "00001") {
                msg += Message.Applicant_Info_17;
                return false;
            }

            //3.再检借款人与共借人关系与婚姻状态，查贷款金额
            var financedAmt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1);
            if (relation == "00006" && marital_code == "00001" && app_marital_code == "00001" && parseFloat(financedAmt) > 300000) {
                //检查共借人是否隐藏
                if (validate_glyphicon()) {
                    msg += Message.Applicant_Info_24;
                    return false;
                }
            }

            //个人未婚||机构
            if ((t == "00001" && app_marital_code != "00001") ||
                t != "00001") {
                if (relation == "00006" && marital_code == "00001") {//选择了配偶并且选择已婚，未婚及机构贷不允许
                    msg += Message.Applicant_Info_13;
                    return false;
                }
            }
        }
        else if (info.GUARANTOR_TYPE == "I") {//表示是担保人
            var relation = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE", index);
            if (relation == "") {
                msg += "请选择担保人的关系";
                return false;
            }
            //担保人，个人贷，已婚，关系是配偶，公牌贷
            if (t == "00001" && relation == "00006" && app_marital_code == "00001" && is_GPD) {
                has_spouse = true;
            }
        }
    });

    if (!has_spouse) {
        if (msg == "") {
            msg += "请提供配偶信息，且必须为共借人；公牌贷配偶可以为担保人";
        }
    }
    return msg;
}

//检验电话号码是不是唯一
function validate_tel_number_unique() {
    var all_tels = [];
    var msg = "";
    $("table[data-datafield='APPLICANT_PHONE_FAX'] tr.rows").each(function (n, row) {
        var tel = $(row).find("input[data-datafield='APPLICANT_PHONE_FAX.PHONE_NUMBER']").val();
        if (tel != "") {
            if ($.inArray(tel, all_tels) > -1) {
                msg = "电话都不能重复，重复号码：" + tel;
                return false;
            }
            else {
                all_tels.push(tel);
            }
        }
    });
    return msg;
}

//必须提供一个抵押人
function validate_lienee(applicant_detail_rows, company_detail_rows) {
    var isCheck = false;
    //判断个人中是否有选中抵押人
    applicant_detail_rows.find("input[data-datafield='APPLICANT_DETAIL.LIENEE']").each(function (n, v) {
        if ($(v).prop("checked")) {
            isCheck = true;
        }
    });
    //判断机构中是否有选中抵押人
    company_detail_rows.find("input[data-datafield='COMPANY_DETAIL.LIENEE']").each(function (n, v) {
        if ($(v).prop("checked")) {
            isCheck = true;
        }
    });
    if (!isCheck)
        return Message.Applicant_Info_20;
    return "";
}

//判断附加费用，不能超过资产价格15%
function validate_accessory() {
    //附加费总价
    var acc_price = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1));
    var asset_price = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1));

    //附加费数量
    var assentCount = $.MvcSheetUI.GetElement("ASSET_ACCESSORY").find("tr.rows").length;

    //个人贷、机构贷
    var app_type = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    if (app_type == "00002" && assentCount > 0) {
        return "机构贷不能有附加费";
    }

    //资产状况（新车、二手车）
    var asset_condition = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.CONDITION", 1);

    //购车目的
    var usage = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.USAGE7", 1);
    //全名
    var appelation = window.localStorage.getItem("appellation");
    //判断内外网客户
    var nww = "";
    if ($.MvcSheetUI.SheetInfo.UserCode.indexOf("98") == 0 || $.MvcSheetUI.SheetInfo.UserCode.indexOf("80000") == 0) {
        nww = "内网";
    }
    else {
        nww = "外网";
    }

    if (asset_condition == "N") {
        if (acc_price > parseFloat(calculate("mul", asset_price, 0.15))) {//超过15%
            return Message.Applicant_Info_23;
        }
    }

    if ((nww == "外网" && (appelation.indexOf("02") >= 0 || appelation.indexOf("03") >= 0) ) || asset_condition == "U") {
        if (acc_price > parseFloat(calculate("mul", asset_price, 0.1))) {//超过10%
            return Message.Applicant_Info_25;
        }
    }

    if (assentCount > 0 && usage == "2") {
        return Message.Applicant_Info_26;
    }

    //贷款金额
    var dkAmt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));

    if (assentCount > 0 && dkAmt > 1000000){
        return Message.Applicant_Info_27;
    }

    if (assentCount > 0) {
        var msg = "";
        $.MvcSheetUI.GetElement("ASSET_ACCESSORY").find("tr.rows").each(function (n, v) {
            var code = $.MvcSheetUI.GetControlValue("ASSET_ACCESSORY.ACCESSORY_CDE", $(v).attr("data-row"));
            if (code == "00595") {
                msg = Message.Applicant_Info_28;
                return false;
            }
        });
        if (msg != "") {
            return msg;
        }
    }
    //4s店附加费
    var new4sAssetArray = ["00001", "00354", "00386", "00550", "00004", "00002"];

    //除4S店以外其他经销商附加费
    var otherAssetArray = ["00354", "00386", "00550", "00002", "00004"];

    if (nww == "内网" || (nww == "外网" && appelation.indexOf("01") >= 0)) {
        var msg = "";
        if (asset_condition == "N") {
            $.MvcSheetUI.GetElement("ASSET_ACCESSORY").find("tr.rows").each(function (n, v) {
                var code = $.MvcSheetUI.GetControlValue("ASSET_ACCESSORY.ACCESSORY_CDE", $(v).attr("data-row"));
                if ($.inArray(code, new4sAssetArray) < 0) {
                    msg = Message.Applicant_Info_29;
                    return false;
                }
            });
        }
        if (msg != "") {
            return msg;
        }
    }
    else {
        var msg = "";
        if (asset_condition == "N") {
            $.MvcSheetUI.GetElement("ASSET_ACCESSORY").find("tr.rows").each(function (n, v) {
                var code = $.MvcSheetUI.GetControlValue("ASSET_ACCESSORY.ACCESSORY_CDE", $(v).attr("data-row"));
                if ($.inArray(code, otherAssetArray) < 0) {
                    msg = Message.Applicant_Info_30;
                    return false;
                }
            });
        }
        if (msg != "") {
            return msg;
        }
    }

    //二手车附加费
    var sendHandAssetArray = ["00354", "00002", "00004", "00596"];
    if (asset_condition == "U") {
        var msg = "";
        $.MvcSheetUI.GetElement("ASSET_ACCESSORY").find("tr.rows").each(function (n, v) {
            var code = $.MvcSheetUI.GetControlValue("ASSET_ACCESSORY.ACCESSORY_CDE", $(v).attr("data-row"));
            if ($.inArray(code, sendHandAssetArray) < 0) {
                msg = Message.Applicant_Info_31;
                return false;
            }
            if (code == "00596") {
                var assetPrice = parseFloat($.MvcSheetUI.GetControlValue("ASSET_ACCESSORY.PRICE", $(v).attr("data-row")));
                if (assetPrice > parseFloat(calculate("mul", asset_price, 0.06))) {//超过6%
                    msg = Message.Applicant_Info_32;
                    return false;
                }
            }
        });
        if (msg != "") {
            return msg;
        }
    }

    return "";
}

//判断申请人地址：个人必须要有家庭地址，机构必须有办公地址
function validate_address(address_detail_rows) {
    var app_type = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    var msg = "";
    var ads_type_isOK = false;//地址类型选择是否满足
    address_detail_rows.each(function (n, v) {
        var index = $(v).attr("data-row");
        var code = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index);
        if (code == "1") {
            var address_type = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_TYPE_CDE", index);
            if (app_type == "00001" && address_type == "00002") {//个人且是家庭地址，满足
                ads_type_isOK = true;
            }
            else if (app_type == "00002" && address_type == "00003") {//机构且是办公地址，满足
                ads_type_isOK = true;
            }
        }
        var info = get_app_type_info(code);
        if (info.APPLICANT_TYPE == "I" || info.GUARANTOR_TYPE == "I") {
            var v_v1 = $.MvcSheetUI.GetControlValue("ADDRESS.NATIVE_DISTRICT", index);
            if (v_v1 == "") {
                msg = "请填写籍贯";
                return false;
            }
            var v_v2 = $.MvcSheetUI.GetControlValue("ADDRESS.BIRTHPLEASE_PROVINCE", index);
            if (v_v2 == "") {
                msg = "请填写出生地省市县（区）";
                return false;
            }
            var v_v3 = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_ID", index);
            if (v_v3 == "") {
                msg = "请填写户籍地址";
                return false;
            }
            //可编辑
            if ($.MvcSheetUI.GetElement("ADDRESS.CurrentCity", 1).SheetUIManager().Editable) {
                var v_v4 = $.MvcSheetUI.GetControlValue("ADDRESS.CurrentCity", index);
                if (v_v4 == "") {
                    msg = "请选择现居住地址";
                    return false;
                }
            }
        }
        else {
            var v_v4 = $.MvcSheetUI.GetControlValue("ADDRESS.REGISTERED_ADDRESS", index);
            if (v_v4 == "") {
                //msg = "请填写注册地址";
                //return false;
                $.MvcSheetUI.SetControlValue("ADDRESS.REGISTERED_ADDRESS", $.MvcSheetUI.GetControlValue("ADDRESS.UNIT_NO", index), index);
            }
        }
    });
    if (msg != "")
        return msg;
    if (!ads_type_isOK) {
        if (app_type == "00001") {//个人
            msg = Message.Addresses_3;
        }
        else {//机构
            msg = Message.Addresses_4;
        }
    }
    return msg;
}

//判断：每个借款人至少提供一个地址并附有电话与传真信息
function validate_address_phone_fax(address_detail_rows) {
    var ele_phone_fax = $.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX");
    //地址或者电话表数据有一个为0，直接返回错误信息
    if (address_detail_rows.length == 0 || ele_phone_fax.find("tr.rows").length == 0) {
        return Message.Applicant_Info_22;
    }
    var isOK = false;//是否满足要求
    var msg = "";
    var phoneNumbers = [];
    //遍历所有人员数据
    $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows").each(function (n, v) {
        var identify_code = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", $(v).attr("data-row"));
        var identify_type = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", $(v).attr("data-row"));
        var is_Address_OK = false;//当前这个人是否有地址
        var is_PhoneFax_OK = false;//当前这个人是否有电话
        var is_work_ok = false;//当前这个人是否有工作信息；
        var is_HaveMobile = false;//是否有手机号码
        var workCompanyName = [];//工作的公司名称
        var is_companyNameOk = true;//工作的公司名称是否OK？
        address_detail_rows.each(function (address_n, address_v) {//
            var index = $(address_v).attr("data-row");
            var code = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index);
            if (code == identify_code) {
                is_Address_OK = true;
                var address_code = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.ADDRESS_CODE5", index);
                ele_phone_fax.find("tr.rows").each(function (phone_n, phone_v) {//
                    var index_phone = $(phone_v).attr("data-row");
                    var code_phone = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", index_phone);
                    var phone_address_code = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.ADDRESS_CODE5", index_phone);
                    var phone_type_code = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_TYPE_CDE", index_phone);
                    if (code_phone == identify_code && phone_type_code == "00003") {
                        is_HaveMobile = true;
                        if (identify_type == "Y") {
                            var phoneNumber = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_NUMBER", index_phone);
                            phoneNumbers.push(phoneNumber);
                        }
                    }
                    if (code_phone == identify_code && phone_address_code == address_code) {
                        is_PhoneFax_OK = true;
                        //return false;
                    }
                });
            }
        });
        if (!is_Address_OK || !is_PhoneFax_OK) {
            msg = Message.Applicant_Info_22;
            return false;
        }
        if (!is_HaveMobile) {
            msg = "每个借款人至少提供一个地址并电话类型为手机的信息";
            return false;
        }
        //个人需要判断是否有工作信息；
        if ($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", $(v).attr("data-row")) == "I" ||
            $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", $(v).attr("data-row")) == "I") {
            $.MvcSheetUI.GetElement("EMPLOYER").find("div.rows").each(function (emp_n, emp_v) {//
                var index = $(emp_v).attr("data-row");
                var code = $.MvcSheetUI.GetControlValue("EMPLOYER.IDENTIFICATION_CODE6", index);
                if (code == identify_code) {
                    is_work_ok = true;
                    var empName = $.MvcSheetUI.GetControlValue("EMPLOYER.NAME_2", index);
                    if (workCompanyName == "")
                        workCompanyName.push(empName);
                    else {
                        if ($.inArray(empName, workCompanyName) > -1) {
                            is_companyNameOk = false;
                        }
                        else {
                            workCompanyName.push(empName);
                        }
                    }
                }
            });
            if (!is_work_ok) {
                msg = "每个借款人至少提供一个工作信息";
                return false;
            }
            if (!is_companyNameOk) {
                msg = "每个借款人工作信息中公司名称不允许重复";
                return false;
            }
        }
    });

    if (msg == "" && sysPhone != "" && $.inArray(sysPhone, phoneNumbers) < 0) {
        msg = "主借人地址中的手机号码与预审批填写的手机号码不匹配";
    }

    return msg;
}

//资产信息校验
function validate_asset() {
    var bal_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));//气球比例
    var bal_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1));//气球金额
    var sfk_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));//首付比例
    var sfk_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1));//首付金额
    var rate = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BASE_CUSTOMER_RATE", 1));//客户利率
    if (bal_pct > 100 || bal_pct < 0)
        return Message.Financial_Term_1;
    var sale_price = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1));//销售价格
    if (bal_amt > sale_price || bal_amt < 0) {
        return Message.Financial_Term_4;
    }
    if (sfk_amt < 0 || sfk_amt > sale_price) {
        return Message.Financial_Term_5;
    }
    if (sfk_pct < 0 || sfk_pct > 100) {
        return Message.Financial_Term_6;
    }
    if (rate < 0) {
        return Message.Financial_Term_7;
    }
    if (rate > 100) {
        return Message.Financial_Term_8;
    }
    return "";
}

//身份证号码校验
function validate_IdentifyCode(applicant_detail_rows, company_detail_rows) {
    var result = true;
    applicant_detail_rows.each(function (n, v) {
        var con = $(v).find("input[data-datafield='APPLICANT_DETAIL.ID_CARD_NBR']")[0];
        var v_nme = $(v).find("input[data-datafield='APPLICANT_DETAIL.FIRST_THI_NME']");
        v_nme.change();//触发change();
        if (!IdentifyCodeChange(con)) {
            result = false;
            return false;
        }
        if (v_nme.val().indexOf('、') > -1) {
            shakeMsg("姓名" + v_nme.val() + "包含特殊字符、");
            result = false;
            return false;
        }
    });
    company_detail_rows.each(function (n, v) {
        var con = $(v).find("input[data-datafield='COMPANY_DETAIL.REP_ID_CARD_NO']")[0];
        var v_nme = $(v).find("input[data-datafield='COMPANY_DETAIL.COMPANY_THI_NME']");
        var v_org_code = $(v).find("input[data-datafield='COMPANY_DETAIL.ORGANIZATION_CDE']");
        v_nme.change();//触发change();
        v_org_code.change();//触发change();
        if (!IdentifyCodeChange(con)) {
            result = false;
            return false;
        }
        if (v_nme.val().indexOf('、') > -1) {
            shakeMsg("公司名称" + v_nme.val() + "包含特殊字符、");
            result = false;
            return false;
        }
    });
    return result;
}

//预审批系统结果校验
function validate_Ysp(applicant_detail_rows) {
    var t = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    if (t !== "00001") {//机构贷不校验
        return "";
    }
    var msg = "";
    var name = "";
    var idcardno = "";
    var type = "";
    var v_mobile = "";

    applicant_detail_rows.each(function (n, row) {
        if (msg !== "") return;//如果前面的贷款人验证失败，不再验证提示后面的贷款人

        var code2 = $(row).find("input[data-datafield=\"APPLICANT_DETAIL.IDENTIFICATION_CODE2\"]").val();
        name = $(row).find("input[data-datafield=\"APPLICANT_DETAIL.FIRST_THI_NME\"]").val();
        idcardno = $(row).find("input[data-datafield=\"APPLICANT_DETAIL.ID_CARD_NBR\"]").val();
        type = $(row).find("select[data-datafield=\"APPLICANT_DETAIL.ID_CARD_TYP\"]").val();

        //v_mobile = "";
        //$.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX").find("tr.rows").each(function (num_phone, row_phone) {
        //    var rowindex = $(row_phone).attr("data-row");
        //    var identifycode = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", rowindex);
        //    if (identifycode == code2) {
        //        var phoneType = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_TYPE_CDE", rowindex);
        //        if (phoneType == "00003") {//电话类型为手机
        //            v_mobile = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_NUMBER", rowindex);
        //            return false;
        //        }
        //    }
        //});

        //type=00001表示是身份证，非身份证不做预审批
        if (name == "" || idcardno == "" || type != "00001") {
            return;
        }
        //if (v_mobile == "") {
        //    if (code2 == "1")
        //        msg = "请提供主贷人手机号码";
        //    else
        //        msg = "请提供共贷人或担保人手机号码";
        //    return;
        //}

        var r = get_ysp_result(name, idcardno, v_mobile);
        if (r.YSP_Enable) {//启用预审批
            if (r.ControlType == "strong") {
                if (!r.Queried) {
                    msg = "此用户[" + name + "][" + idcardno + "]未走预审批流程，请先发起预审批流程";
                }
                else if (!r.IsFinish) {
                    msg = "此用户[" + name + "][" + idcardno + "]预审批流程未结束，请等待";
                }
                else if (r.Refused && code2 == "1") {
                    msg = "此用户[" + name + "][" + idcardno + "]被预审批流程拒绝，不能进件，如有疑问请联系管理员";
                }
            }
            else if (r.ControlType == "weak") {
                if (!r.Queried) {
                    msg = "此用户[" + name + "][" + idcardno + "]未走预审批流程，请先发起预审批流程";
                }
                else if (!r.IsFinish) {
                    msg = "此用户[" + name + "][" + idcardno + "]预审批流程未结束，请等待";
                }
            }
            else {//不做控制
                msg = "";
            }
        }

    });
    return msg;
}

//二手车，Vin号必填；
function validate_Vin() {
    var msg = "";
    if ($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.CONDITION", 1) == "U") {
        if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.vin_number.O.indexOf("E") > -1
            && $.MvcSheetUI.GetControlValue("vin_number") == "") {
            msg = "二手车必须输入Vin码，且提交后不能修改";
        }
    }
    return msg;
}

//婚姻状况Change事件
function marital_chg(con) {
    if ($.MvcSheetUI.SheetInfo.SheetMode == $.MvcSheetUI.SheetMode.View) {
        return false;
    }
    if (!con)
        con = $.MvcSheetUI.GetElement("APPLICANT_DETAIL.MARITAL_STATUS_CDE", 1)[0];
    var index = $(con).closest("div.rows").attr("data-row");
    var id = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", index);
    var info = get_app_type_info(id);
    if (info.MAIN_APPLICANT != "") {//主借人的婚姻状况修改时，需要对Pouse控件进行禁用操作
        if ($(con).val() == "00001") {//已婚，Pouse可以点击
            $("input[data-datafield='APPLICANT_DETAIL.SPOUSE_IND']").removeAttr("disabled");
        }
        else {//未婚，Pouse不可点击
            $("input[data-datafield='APPLICANT_DETAIL.SPOUSE_IND']").attr("disabled", "disabled").prop("checked", false);
        }
    }
    $("select[data-datafield='APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE']").change();
}

//是否配偶的标识
function spouse_chg(con) {
    var checked = $(con).prop("checked");
    var index = $(con).closest("div.rows").attr("data-row");
    var hasChecked = false;
    if (checked) {
        $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").find("input[data-datafield='APPLICANT_DETAIL.SPOUSE_IND']").each(function (n, v) {
            if ($(v).closest("div.rows").attr("data-row") == index)
                return true;
            if ($(v).prop("checked")) {
                hasChecked = true;
                shakeMsg(Message.Applicant_Info_19);
                return false;
            }
        });
    }
    if (hasChecked) {
        $(con).prop("checked", false);
    }
}

//贷款卡号change事件
function lending_chg(con) {
    var v = $(con).val();
    if (v != "" && v.length < 16) {
        shakeMsg(Message.Company_Applicant_Info_1);
        $(con).focus();
    }
}

//邮编编码Change事件
function postcode_chg(con) {
    var v = $(con).val();
    if (v != "" && v.length < 6) {
        shakeMsg("邮编应为6位");
        $(con).focus();
    }
}

//根据id获取applicant info 信息
function get_app_type_info(id) {
    var IDENTIFICATION_CODE;
    var APPLICANT_TYPE;
    var GUARANTOR_TYPE;
    var MAIN_APPLICANT;
    var NAME;
    var IS_INACTIVE_IND;
    $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows").each(function (n, v) {
        var code = $(v).find("input[data-datafield='APPLICANT_TYPE.IDENTIFICATION_CODE1']").val();
        if (code == id) {
            APPLICANT_TYPE = $(v).find("input[data-datafield='APPLICANT_TYPE.APPLICANT_TYPE']").val();
            GUARANTOR_TYPE = $(v).find("input[data-datafield='APPLICANT_TYPE.GUARANTOR_TYPE']").val();
            MAIN_APPLICANT = $(v).find("input[data-datafield='APPLICANT_TYPE.MAIN_APPLICANT']").val();
            NAME = $(v).find("input[data-datafield='APPLICANT_TYPE.NAME1']").val();
            IS_INACTIVE_IND = $(v).find("input[data-datafield='APPLICANT_TYPE.IS_INACTIVE_IND']").val();
            return false;
        }
    });

    return {
        IDENTIFICATION_CODE: id, APPLICANT_TYPE: APPLICANT_TYPE, GUARANTOR_TYPE: GUARANTOR_TYPE, MAIN_APPLICANT: MAIN_APPLICANT, NAME: NAME, IS_INACTIVE_IND: IS_INACTIVE_IND
    };
}

//身份证号码change事件
function IdentifyCodeChange(con) {
    var index = "";
    if ($.MvcSheetUI.SheetInfo.IsMobile) {
        index = $(con).closest("div.slider-slide").attr("data-row");
    }
    else {
        index = $(con).closest("div.rows").attr("data-row");
    }
    var datafield = $(con).attr("data-datafield");
    datafield = datafield.substring(0, datafield.indexOf("."));
    if (datafield == "APPLICANT_DETAIL") {
        var type = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_TYP", index);
        var idnumber = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_NBR", index);
        var name = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.FIRST_THI_NME", index);
        if (type == "00001") {
            if (idnumber == "") {
                shakeMsg("身份证号码不允许为空");
                return false;
            }
            var success = IdentityCodeValid(idnumber);
            if (!success) {
                return false;
            }
            else {
                var date_str;//出生日期
                var num_sex;//性别的标志号
                if (idnumber.length == 15) {//老的身份证号码
                    date_str = idnumber.substr(6, 6);
                    num_sex = idnumber.substr(13, 1);
                }
                else if (idnumber.length == 18) {//新的身份证号码
                    date_str = idnumber.substr(6, 8);
                    num_sex = idnumber.substr(16, 1);
                }
                var year = date_str.substr(0, 4);
                var month = date_str.substr(4, 2);
                var day = date_str.substr(6, 2);
                var date = year + "-" + month + "-" + day;
                $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.DATE_OF_BIRTH", date, index);
                $.MvcSheetUI.GetElement("APPLICANT_DETAIL.DATE_OF_BIRTH", index).change();
                //偶数表示为女性，奇数为男性
                if (num_sex % 2 == 0) {
                    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.SEX", "F", index);
                }
                else {
                    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.SEX", "M", index);
                }
            }

            
            var identifycode = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", index);

            if (name != "" && identifycode == "1") {
                var yspInfo = getYSPInfo(name, idnumber);
                //TODO ------------------------- wangxg 19.8 note 上线前要取消注释，只在测试时不验证
                if (yspInfo.phone == "" || yspInfo.phone == null) {
                    shakeMsg("未查询到预审批填写的手机号");
                    return false;
                }
                sysPhone = yspInfo.phone;
            }

        }
    }
    else if (datafield == "COMPANY_DETAIL") {
        var v = $(con).val();
        if (v && v != "") {
            var success = IdentityCodeValid(v);
            if (!success) {
                return false;
            }
        }
    }
    return true;
}

//地址信息Tab点击事件
function address_li_event() {
    $("#myTab_ctl621351").find("a").on('shown.bs.tab', function (e) {
        var row = $($(e.currentTarget).attr("href"));
        var index = row.attr("data-row");
        var identifycode = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index);
        var ads_code = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_CODE", index);
        sort_phone_fax(identifycode, ads_code);
        setAddressHiddenField(e.currentTarget);
    });
}

function setAddressHiddenField(tab_a) {
    var row = $($(tab_a).attr("href"));
    var index = row.attr("data-row");
    var id = row.attr("id");
    var identifycode = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index);
    var info = get_app_type_info(identifycode);
    if (info.APPLICANT_TYPE == "I" || info.GUARANTOR_TYPE == "I") {//个人
        $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_ID", index).parent().parent().parent().show();//显示家庭地址
        //隐藏国家
        $.MvcSheetUI.GetElement("ADDRESS.COUNTRY_CDE", index).parent().parent().hide();
        //地址类型
        $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_TYPE_CDE", index).parent().parent().show();

        //现居住地
        $.MvcSheetUI.GetElement("ADDRESS.CurrentState", index).parent().parent().parent().show();

        $.MvcSheetUI.GetElement("ADDRESS.RESIDENCE_TYPE_CDE", index).parent().parent().hide();

        //开始居住日期
        $.MvcSheetUI.GetElement("ADDRESS.LIVING_FROM_DTE", index).parent().parent().show();
        //住宅类型
        $.MvcSheetUI.GetElement("ADDRESS.RESIDENCE_TYPE_CDE", index).parent().parent().show();
        //籍贯、出生地省市县
        $.MvcSheetUI.GetElement("ADDRESS.NATIVE_DISTRICT", index).closest("div.row").show();
        $("#ctl621351_label6_Row" + index).text("户籍省份");
        $("#ctl621351_label5_Row" + index).text("户籍城市");
        $("#ctl621351_label14_Row" + index).text("现居住地址");
    }
    else if (info.APPLICANT_TYPE == "C" || info.GUARANTOR_TYPE == "C") {//机构
        $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_ID", index).parent().parent().parent().hide();//隐藏家庭地址
        //显示国家
        $.MvcSheetUI.GetElement("ADDRESS.COUNTRY_CDE", index).parent().parent().show();
        //地址类型
        $.MvcSheetUI.GetElement("ADDRESS.ADDRESS_TYPE_CDE", index).parent().parent().hide();

        //现居住地
        $.MvcSheetUI.GetElement("ADDRESS.CurrentState", index).parent().parent().parent().hide();
        //开始居住日期
        $.MvcSheetUI.GetElement("ADDRESS.LIVING_FROM_DTE", index).parent().parent().hide();
        //住宅类型
        $.MvcSheetUI.GetElement("ADDRESS.RESIDENCE_TYPE_CDE", index).parent().parent().hide();
        //籍贯、出生地省市县
        $.MvcSheetUI.GetElement("ADDRESS.NATIVE_DISTRICT", index).closest("div.row").hide();
        $("#ctl621351_label6_Row" + index).text("公司省份");
        $("#ctl621351_label5_Row" + index).text("公司城市");
        $("#ctl621351_label14_Row" + index).text("公司地址");
    }
    var default_ads = false;
    var default_hk_ads = false;
    $("div[data-datafield='ADDRESS'] div.rows").each(function (n, address_row) {
        var ads_index = $(address_row).attr("data-row");
        if ($(address_row).attr("id") != id && $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", ads_index) == identifycode) {
            if ($.MvcSheetUI.GetControlValue("ADDRESS.DEFAULT_ADDRESS", ads_index)) {
                default_ads = true;
            }
            if ($.MvcSheetUI.GetControlValue("ADDRESS.HUKOU_ADDRESS", ads_index)) {
                default_hk_ads = true;
            }
        }
    });
    if (default_ads) {
        $.MvcSheetUI.GetElement("ADDRESS.DEFAULT_ADDRESS", index).attr("disabled", "disabled");
    }
    if (default_hk_ads) {
        $.MvcSheetUI.GetElement("ADDRESS.HUKOU_ADDRESS", index).attr("disabled", "disabled");
    }
}

function sort_phone_fax(identificationid, add_code) {
    var num = 1;
    var ele = $.MvcSheetUI.GetElement("ADDRESS");
    if (!ele)
        return false;

    $.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX").find("tr.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", index) == identificationid &&
            $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.ADDRESS_CODE5", index) == add_code) {
            $(this).show();
            $(v).find("td").eq(0).html(num);
            num += 1;
        }
        else {
            $(this).hide();
        }
    });
}

//设置application type 的名称
function setApplicationTypeName(con, type) {
    if ($(con).val().trim() != $(con).val()) {
        $(con).val($(con).val().trim());
        return false;
    }
    var fieldcode = type == "I" ? "APPLICANT_DETAIL.IDENTIFICATION_CODE2" : "COMPANY_DETAIL.IDENTIFICATION_CODE3";
    var index = $(con).closest("div.rows").attr("data-row");
    var identifycode = $.MvcSheetUI.GetControlValue(fieldcode, index);
    $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows").each(function (n, v) {
        var n = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", n) == identifycode) {
            var con_val = $(con).val();
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.NAME1", strMaxLength(con_val, 100), n);//最高只能100byte长度
        }
    });
    if (identifycode == "1") {//表示申请人，
        $.MvcSheetUI.SetControlValue("APPLICATION_NAME", $(con).val());
    }

    //如果是主借人
    if (identifycode == "1" && name != "" && name != null) {
        var idnumber = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARD_NBR", index);
        var name = $.MvcSheetUI.GetControlValue("APPLICATION_NAME", 1);
        var yspInfo = getYSPInfo(name, idnumber);
        //TODO ------------------------- wangxg 19.8 note 上线前要取消注释，只在测试时不验证
        if (yspInfo.phone == "" || yspInfo.phone == null) {
            shakeMsg("未查询到预审批填写的手机号");
            return false;
        }
        sysPhone = yspInfo.phone;
    }

}

//设置application type 的名称
function setApplicationTypeName_Mobile(con, type) {
    if ($(con).val().trim() != $(con).val()) {
        $(con).val($(con).val().trim());
        return false;
    }
    var fieldcode = type == "I" ? "APPLICANT_DETAIL.IDENTIFICATION_CODE2" : "COMPANY_DETAIL.IDENTIFICATION_CODE3";
    var index = $(con).closest("div.slider-slide").attr("data-row");
    var identifycode = $.MvcSheetUI.GetControlValue(fieldcode, index);
    $("div[data-datafield=\"APPLICANT_TYPE\"] .slider-slide").each(function (n, v) {
        var n = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", n) == identifycode) {
            var con_val = $(con).val();
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.NAME1", strMaxLength(con_val, 100), n);//最高只能100byte长度
        }
    });
    if (identifycode == "1") {//表示申请人，
        $.MvcSheetUI.SetControlValue("APPLICATION_NAME", $(con).val());
    }
}

//获取金融产品参数
function get_Product_Parameter(auto_get_qs) {
    var p_id = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1);

    if (p_id == "")
        return null;
    var qs = "";
    if (auto_get_qs)
        qs = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);
    var result;
    // 执行后台事件
    $.MvcSheet.Action(
        {
            Action: "get_product_parameter",    // 后台方法名称
            Datas: [p_id, qs],     // 输入参数，格式 ["{数据项名称}","String值","控件ID"]，当包含数据项名称时 LoadControlValue必须为true
            LoadControlValue: true,  // 是否获取表单数据
            PostSheetInfo: false,    // 是否获取已经改变的表单数据
            Async: false,
            OnActionDone: function (e) {
                console.log(e);
                // 执行完成后回调事件
                result = e;
            }
        }
    )
    return result;
}

//获取还款计划(总价，贷款金额，尾款金额，贷款期数，产品ID)
function get_hk_plan(total_price, dk_amt, wk_amt, dkqs, product_id) {
    var result;
    // 执行后台事件
    $.MvcSheet.Action(
        {
            Action: "get_HK_plan",    // 后台方法名称
            Datas: [total_price, dk_amt, wk_amt, dkqs, product_id],     // 输入参数，格式 ["{数据项名称}","String值","控件ID"]，当包含数据项名称时 LoadControlValue必须为true
            LoadControlValue: true,  // 是否获取表单数据
            PostSheetInfo: false,    // 是否获取已经改变的表单数据
            Async: false,
            OnActionDone: function (e) {
                console.log(e);
                // 执行完成后回调事件
                result = e;
            }
        }
    )
    return result;
}

//抵押人先中change事件，所有人中，只能选择一个抵押人
function ck_Lienee_change(con) {
    var ischecked = $(con).prop("checked");
    var row_id = $(con).closest("div.rows").attr("id");
    if (ischecked) {
        //把其它的checkbox设置成false
        //个人
        $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
            if ($(v).attr("id") != row_id) {
                var index = $(v).attr("data-row");
                $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.LIENEE", false, index);
            }
        });
        //机构
        $.MvcSheetUI.GetElement("COMPANY_DETAIL").find("div.rows").each(function (n, v) {
            if ($(v).attr("id") != row_id) {
                var index = $(v).attr("data-row");
                $.MvcSheetUI.SetControlValue("COMPANY_DETAIL.LIENEE", false, index);
            }
        });
    }
}

// 公牌贷，抵押人必须是共贷人(公司），且不能隐藏 wangxg 19.8
// return 0验证通过。 1公牌贷[复选框]必须是共借人(公司)。2若满足1则共借人不能隐藏。
function ck_companyLienee() {
    var result = 0;
    var orgs = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE");  
    var gpd = $.MvcSheetUI.GetControlValue("isgpd");
    // 以下只对类型是公司的部分遍历
    $.MvcSheetUI.GetElement("COMPANY_DETAIL").find("div.rows").each(function (n, v) {
        if (result !== 0) return;// 跳出内部函数
        var index = $(v).attr("data-row");
        var applicantType = '';// 共借人类型 APPLICANT_TYPE = 申请人类型（C是公司，I个人）;GUARANTOR_TYPE = 担保人类型（C是公司，I个人）
        var IDENTIFICATION_CODE3 = $.MvcSheetUI.GetControlValue("COMPANY_DETAIL.IDENTIFICATION_CODE3", index);
        var objectId = '';
        for (var t in orgs) {
            if (orgs[t].IDENTIFICATION_CODE1 === IDENTIFICATION_CODE3) {
                applicantType = orgs[t].APPLICANT_TYPE;
                objectId = orgs[t].ObjectID;
            }
        }
        var companyLienee = $.MvcSheetUI.GetControlValue("COMPANY_DETAIL.LIENEE", index);//抵押人是否勾选
        if (gpd === '1' && applicantType === 'C' && !companyLienee) {
            result = 1;
            return;
        }
        if (companyLienee) {
            var hide = $("#li_" + objectId).find("i.glyphicon-eye-close");
            if (hide.length > 0) {// 说明隐藏了
                result = 2;
            }
        }
    });

    // 对个人部分遍历， 验证抵押人[勾选]不能隐藏
    $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
        if (result !== 0) return;// 跳出内部函数
        var index = $(v).attr("data-row");
        var IDENTIFICATION_CODE2 = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", index);
        var objectId = '';
        for (var t in orgs) {
            if (orgs[t].IDENTIFICATION_CODE1 === IDENTIFICATION_CODE2) {
                objectId = orgs[t].ObjectID;
            }
        }
        var companyLienee = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.LIENEE", index);//抵押人是否勾选
        if (companyLienee) {
            var hide = $("#li_" + objectId).find("i.glyphicon-eye-close");
            if (hide.length > 0) {// 说明隐藏了
                result = 2;
            }
        }
    });
    return result;
}

//制造商名称Change事件
function power_parameter_chg(con) {
    var v = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ASSET_MAKE_DSC", 1) + " " + $(con).val();//制造商+运力参数
    $.MvcSheetUI.GetElement("VEHICLE_DETAIL.COMMENTS7", 1).unbind("change").bind("change", function () { asset_comment_chg(this) });
    $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.COMMENTS7", v, 1);
    $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.VEHICLE_COMMENT", v, 1);
}

//备注修改后，给VEHICLE_COMMENT赋同样的值；
function asset_comment_chg(con) {
    $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.VEHICLE_COMMENT", $(con).val(), 1);
}

var chg_num = 0;
//检查金额产品参数是否合格,并赋值
function fpp_chg(con, type) {
    var isOK = true;
    var para;

    //控件对象
    var control_qs = $.MvcSheetUI.GetElement("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);//期数

    var control_pct = $.MvcSheetUI.GetElement("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1);//首付款比例
    var control_amt = $.MvcSheetUI.GetElement("CONTRACT_DETAIL.CASH_DEPOSIT", 1);//首付款金额

    var control_wq_pct = $.MvcSheetUI.GetElement("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1);//尾期比例
    var control_wq_amt = $.MvcSheetUI.GetElement("CONTRACT_DETAIL.BALLOON_AMOUNT", 1);//尾期金额

    control_qs.unbind("change.SheetTextBox");
    control_pct.unbind("change.SheetTextBox");
    control_amt.unbind("change.SheetTextBox");
    control_wq_pct.unbind("change.SheetTextBox");
    control_wq_amt.unbind("change.SheetTextBox");

    //产品下拉框的change事件
    //下拉框的值等于BizObject对象中的值;
    console.log("CONTRACT_DETAIL.V.R-->" + $.MvcSheetUI.SheetInfo.BizObject.DataItems.CONTRACT_DETAIL.V.R);
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.CONTRACT_DETAIL.V.R &&
        $.MvcSheetUI.SheetInfo.BizObject.DataItems.CONTRACT_DETAIL.V.R.length > 0) {
        if ($(con).data("datafield") == "CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID" && chg_num == 0
            && $.MvcSheetUI.SheetInfo.BizObject.DataItems.CONTRACT_DETAIL.V.R[0].DataItems["CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID"].V == $(con).val()) {
            chg_num = chg_num + 1;
            //绑定事件
            control_qs.bind("change.SheetTextBox", function () {
                fpp_chg(this, "qs");
            });
            control_pct.bind("change.SheetTextBox", function () {
                fpp_chg(this, "pct");
            });
            control_amt.bind("change.SheetTextBox", function () {
                fpp_chg(this, "amt");
            });
            control_wq_pct.bind("change.SheetTextBox", function () {
                fpp_chg(this, "wq_pct");
            });
            control_wq_amt.bind("change.SheetTextBox", function () {
                fpp_chg(this, "wq_amt");
            });
            return false;
        }
    }

    if (type == "fp")
        para = get_Product_Parameter(false);
    else
        para = get_Product_Parameter(true);
    if (!para)
        return false;
    if (!para.Success) {
        shakeMsg(para.Message);
        return false;
    }
    var para_Data = para.Data;

    var asset_price = parseFloat($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", 1));
    var sale_price = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1) == "" ? 0 : $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1));
    var accessory_price = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1) == "" ? 0 : $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1));
    var total_Price = sale_price + accessory_price;//销售价格+附加费

    var pct = 0;//首付款比例
    var amt = 0;//首付款金额
    var dk_pct = 0;//贷款比例
    var dk_amt = 0;//贷款金额
    var wq_pct = 0;//尾期比例
    var wq_amt = 0;//尾期金额
    var qs = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);//期数
    if (type == "fp") {//产品ID Change
        dk_pct = para_Data.MAXIMUM_FINANCING_PCT;//最大贷款比例
        dk_amt = parseFloat(calculate("div", calculate("mul", total_Price, dk_pct), 100));//最大贷款金额
        qs = para_Data.MINIMUM_LEASE_TRM;//最小期数
        pct = parseFloat(calculate("min", 100, dk_pct));
        amt = parseFloat(calculate("min", total_Price, dk_amt));
        wq_pct = para_Data.RV_PCT;
        wq_amt = parseFloat(calculate("div", calculate("mul", wq_pct, total_Price), 100));
    }
    else if (type == "sp") {//车辆销售价格
        pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));
        amt = parseFloat(calculate("div", calculate("mul", pct, total_Price), 100));
        dk_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", 1));
        dk_amt = parseFloat(calculate("min", total_Price, amt));
        //wq_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));
        //wq_amt = parseFloat(calculate("div", calculate("mul", wq_pct, total_Price), 100));
        wq_pct = para_Data.RV_PCT;
        wq_amt = parseFloat(calculate("div", calculate("mul", wq_pct, total_Price), 100));
    }
    else if (type == "qs") {//期数change
        if (para.Data.AVAILABLE_TRM != qs) {//修改的期数无效
            shakeMsg(Message.Financial_Term_12);
            //isOK = false;此情况就认识OK，会使用默认值；
            qs = para.Data.AVAILABLE_TRM;
        }
        amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1));
        pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));
        dk_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", 1));
        dk_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));
        //wq_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));
        //wq_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1));
        wq_pct = para_Data.RV_PCT;
        wq_amt = parseFloat(calculate("div", calculate("mul", wq_pct, total_Price), 100));
    }
    else if (type == "pct") {//首付款比例
        pct = parseFloat($(con).val());
        amt = parseFloat(calculate("div", calculate("mul", total_Price, pct), 100));
        dk_pct = parseFloat(calculate("min", 100, pct));
        dk_amt = parseFloat(calculate("min", total_Price, amt));
        //wq_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));
        //wq_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1));
        wq_pct = para_Data.RV_PCT;
        wq_amt = parseFloat(calculate("div", calculate("mul", wq_pct, total_Price), 100));
    }
    else if (type == "amt") {//首付款金额
        amt = parseFloat($(con).val());
        pct = parseFloat(calculate("mul", calculate("div", amt, total_Price, "0.0000"), 100));
        dk_pct = parseFloat(calculate("min", 100, pct));
        dk_amt = parseFloat(calculate("min", total_Price, amt));
        //wq_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));
        //wq_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1));
        wq_pct = para_Data.RV_PCT;
        wq_amt = parseFloat(calculate("div", calculate("mul", wq_pct, total_Price), 100));
    }
    else if (type == "wq_pct") {//尾款比例
        amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1));
        pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));
        dk_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", 1));
        dk_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));
        wq_pct = parseFloat($(con).val());
        wq_amt = parseFloat(calculate("div", calculate("mul", total_Price, wq_pct), 100));
    }
    else if (type == "wq_amt") {//尾款金额
        amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1));
        pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1));
        dk_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", 1));
        dk_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));
        wq_pct = parseFloat(calculate("mul", calculate("div", wq_amt, total_Price, "0.0000"), 100));
        wq_amt = parseFloat($(con).val());
    }

    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", qs, 1);//期数.

    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", pct.toFixed(2), 1);//首付款比例
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", amt.toFixed(2), 1);//首付款金额

    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", dk_pct.toFixed(2), 1);//贷款比例
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", dk_amt.toFixed(2), 1);//贷款金额
    $.MvcSheetUI.SetControlValue("financedamount", dk_amt);//贷款金额

    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", wq_pct.toFixed(2), 1);//尾款比例
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", wq_amt.toFixed(2), 1);//尾款金额

    //绑定事件
    control_qs.bind("change.SheetTextBox", function () {
        fpp_chg(this, "qs");
    });
    control_pct.bind("change.SheetTextBox", function () {
        fpp_chg(this, "pct");
    });
    control_amt.bind("change.SheetTextBox", function () {
        fpp_chg(this, "amt");
    });
    control_wq_pct.bind("change.SheetTextBox", function () {
        fpp_chg(this, "wq_pct");
    });
    control_wq_amt.bind("change.SheetTextBox", function () {
        fpp_chg(this, "wq_amt");
    });

    if (para_Data.FUTURE_VALUE_TYP == "R") {
        //尾款百分例控件，尾款金额控件
        if (para_Data.RV_EDITABLE_IND == "T") {//可以编辑
            control_wq_pct.removeAttr("disabled");
            control_wq_amt.removeAttr("disabled");
        }
        else {// if (para_Data.RV_EDITABLE_IND == "F") {//不可以编辑
            control_wq_pct.attr("disabled", "disabled");
            control_wq_amt.attr("disabled", "disabled");
        }
    }
    else {
        control_wq_pct.attr("disabled", "disabled");
        control_wq_amt.attr("disabled", "disabled");
    }


    var min_sfk_pct = parseFloat(calculate("min", 100, para_Data.MAXIMUM_FINANCING_PCT));//最小首付款比例；
    //判断车辆销售价格不能大于资产价格
    if (sale_price > asset_price) {
        shakeMsg(Message.Financial_Term_16);
        isOK = false;
    }
    else if (pct < min_sfk_pct) {//小于最小首付款比例,赋最小值；
        shakeMsg(Message.Financial_Term_11);
        isOK = false;
    }
    else if (pct > 100) {
        shakeMsg(Message.Financial_Term_11);
        isOK = false;
    }
    else if (dk_pct > para_Data.MAXIMUM_FINANCING_PCT || dk_pct < 0) {
        shakeMsg(Message.Financial_Term_11);
        isOK = false;
    }
    else if (parseFloat(calculate("add", pct, dk_pct)) != 100) {
        shakeMsg("首付款比例与贷款比例之和应为100");
        isOK = false;
    }
    else if (dk_amt < para_Data.MINIMUM_FINANCING_AMT || dk_amt > para_Data.MAXIMUM_FINANCING_AMT) {
        shakeMsg(Message.Financial_Term_13);
        isOK = false;
    }
    else if (parseFloat(calculate("add", amt, dk_amt)) != total_Price) {
        shakeMsg("首付款价格与贷款价格之和应等于销售价格");
        isOK = false;
    }
    //尾款可以编辑的时候需要去判断尾款的数值是否在产品设置的范围内;
    else if (para_Data.RV_EDITABLE_IND == "T" && (wq_pct < para_Data.MINIMUM_RV_PCT || wq_pct > para_Data.MAXIMUM_RV_PCT)) {
        shakeMsg(Message.Financial_Term_15);
        isOK = false;
    }
    else if (wq_amt > dk_amt) {
        shakeMsg(Message.Financial_Term_17);
        isOK = false;
    }

    if (!isOK) {
        $.MvcSheetUI.GetElement("PMS_RENTAL_DETAIL").SheetUIManager()._Clear();
        return false;
    }
    else {
        //设置产品参数
        Set_Product_Para();
        return true;
    }
}

//校验金额条款
function validate_finacial_parameter() {
    var para = get_Product_Parameter(false);
    if (!para)
        return "金融条件校验失败";
    if (!para.Success) {
        return para.Message;
    }
    var para_Data = para.Data;

    var qs = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);//期数
    //判断期数是否正确    
    if (para_Data.MINIMUM_LEASE_TRM > parseInt(qs) || para_Data.MAXIMUN_LEASE_TRM < parseInt(qs)) {
        return Message.Financial_Term_12;
    }

    para = get_Product_Parameter(true);
    para_Data = para.Data;

    //表单数据
    var asset_price = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", 1);//资产价格
    var sale_price = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1);//车辆销售价格

    var total_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1));//总价
    var sfk_pct = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1);//首付款比例
    var sfk_amt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1);//首付款金额
    var dk_pct = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCED_AMT_PCT", 1);//贷款比例
    var dk_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1));//贷款金额
    var wk_pct = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_PERCENTAGE", 1));//尾款比例
    var wk_amt = parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1));//尾款金额
    //尾款比例0-100之间
    if (wk_pct < 0 || wk_pct > 100) {
        return Message.Financial_Term_1;
    }
    //尾款金额在0-资产价格之间
    if (wk_amt < 0 || wk_amt > asset_price) {
        return Message.Financial_Term_4;
    }
    //判断销售价格
    if (parseFloat(sale_price) > parseFloat(asset_price)) {
        return Message.Financial_Term_16;
    }

    //判断贷款比例与付款比例之和应为100；    
    if (parseFloat(calculate("add", sfk_pct, dk_pct)) != 100) {
        return "贷款与付款比例之和应为100";
    }
    //判断贷款比例是否小于最大比例；
    if (parseFloat(dk_pct) > para_Data.MAXIMUM_FINANCING_PCT) {
        return Message.Financial_Term_11;
    }
    //判断贷款金额是否在设置的区间    
    if (dk_amt < para_Data.MINIMUM_FINANCING_AMT || dk_amt > para_Data.MAXIMUM_FINANCING_AMT) {
        return Message.Financial_Term_13;
    }
    //判断贷款金额与付款金额是否等于总价
    if (parseFloat(calculate("add", sfk_amt, dk_amt)) != total_amt) {
        return "贷款金额与付款金额之和不等于销售价格";
    }
    //判断尾款
    if (para_Data.FUTURE_VALUE_TYP == "R") {//尾款可以编辑的时候需要去判断尾款的数值是否在产品设置的范围内;
        if (para_Data.RV_EDITABLE_IND == "T" && (wk_pct < para_Data.MINIMUM_RV_PCT || wk_pct > para_Data.MAXIMUM_RV_PCT)) {
            return Message.Financial_Term_15;
        }
    }
    return "";
}

//根据金融条款参数设置相关信息
function Set_Product_Para() {
    var p_id = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1);
    //获取还款计划表，参数：总价，贷款金额，尾期金额，期数，产品ID

    var total_price = calculate("add", $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1), $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1));
    //以下写法有BUG，因为总价是通过计算规则计算出来的。如果金额发生修改会触发Change事件，但是此时还未计算，显示的是上一次的结果；
    //var total_price = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1);//总价
    var dk_amt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1);//贷款金额
    var wq_amt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1);//尾期金额
    var qs = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);//期数
    var product_id = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1);//产品ID

    var v = get_hk_plan(total_price, dk_amt, wq_amt, qs, product_id);
    if (!v.Success) {
        shakeMsg(v.Message);
        return false;
    }
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.BASE_CUSTOMER_RATE", v.RTE, 1);//客户利率；
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.ACTUAL_RTE", v.ACTUAL_RTE, 1);//实际利率；
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.CALC_SUBSIDY_RTE", v.TX_RTE, 1);//贴息利率；
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.ASSETITC", v.TOTAL_ASSETITC, 1);//利息总额；
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.YFJE", v.TOTAL_PAY_AMT, 1);//应付总额
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.CALC_SUBSIDY_AMT", v.SUBSIDY_AMT, 1);//贴息金额
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.WCYE", v.TOTAL_NO_PAY_AMT, 1);//未偿余额
    $.MvcSheetUI.SetControlValue("CONTRACT_DETAIL.DEFERRED_TRM", 0, 1);//展期期数
    $.MvcSheetUI.SetControlValue("PLAN_ID", v.Plan_ID);//保存还款计划ID

    var ele_plan_simple = $.MvcSheetUI.GetElement("PMS_RENTAL_DETAIL").SheetUIManager();
    //ele_plan_simple._Clear();
    var len = $.MvcSheetUI.GetElement("PMS_RENTAL_DETAIL").find("tr.rows").length;
    if (v.Plan_Simple.length > len) {//新的简版还款计划数量大于原先的数量
        for (var i = 0; i < v.Plan_Simple.length - len; i++) {
            ele_plan_simple._AddRow();//添加新行
        }
    }
    else if (v.Plan_Simple.length < len) {//新的简版还款计划数量少于原先的数量
        var plan_simple_trs = $.MvcSheetUI.GetElement("PMS_RENTAL_DETAIL").find("tr.rows")
        for (var i = v.Plan_Simple.length; i < len; i++) {
            ele_plan_simple._Deleterow($(plan_simple_trs[i]));//删除新行
        }
    }
    else {
        //数量相等的不做处理；
    }
    //设置定时执行：0.5s
    setTimeout(function () {
        $(v.Plan_Simple).each(function (n, v) {
            $.MvcSheetUI.SetControlValue("PMS_RENTAL_DETAIL.RENTAL_DETAIL_SEQ", v.RENTAL_DETAIL_SEQ, n + 1);
            $.MvcSheetUI.SetControlValue("PMS_RENTAL_DETAIL.START_TRM", v.START_TRM, n + 1);
            $.MvcSheetUI.SetControlValue("PMS_RENTAL_DETAIL.END_TRM", v.END_TRM, n + 1);
            $.MvcSheetUI.SetControlValue("PMS_RENTAL_DETAIL.RENTAL_AMT", v.RENTAL_AMT, n + 1);
            $.MvcSheetUI.SetControlValue("PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT", v.EQUAL_RENTAL_AMT, n + 1);
            $.MvcSheetUI.SetControlValue("PMS_RENTAL_DETAIL.INTEREST_RTE", v.INTEREST_RTE, n + 1);
        });
    }, 500);
}

//身份证卡的发行日期与到期日期
function id_card_date_chg(con) {
    var index = "";
    if ($.MvcSheetUI.SheetInfo.IsMobile) {
        index = $(con).closest("div.slider-slide").attr("data-row");
    }
    else {
        index = $(con).closest("div.rows").attr("data-row");
    }
    var issue_date = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARDISSUE_DTE", index);
    var expire_date = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.ID_CARDEXPIRY_DTE", index);
    if (issue_date == "" || expire_date == "")
        return false;
    if (new Date(issue_date) >= new Date(expire_date)) {
        $(con).val("");
        shakeMsg(Message.Applicant_Info_1);
    }
}

//保存下拉框选中Item的值
function save_ddl_selected_source() {
    var data = {
    };
    var data_APPLICATION = {
    };//基本信息
    var data_APPLICANT_DETAIL = [];//个人明细
    var data_COMPANY_DETAIL = [];//机构明细
    var data_ADDRESS = [];//地址
    var data_APPLICANT_PHONE_FAX = [];//电话
    var data_EMPLOYER = [];//工作
    var data_CONTACT = [];//联系人
    var data_VEHICLE_DETAIL = {
    };//资产    
    var data_CONTRACT_DETAIL = {
    };//金融条款
    //基本信息
    $("#divSheet").find("select").each(function (n, v) {
        data_APPLICATION[$(v).attr("data-datafield")] = {
            Code: $(v).find("option:selected").val(),
            Text: $(v).find("option:selected").text()
        };
    });
    //个人明细表
    $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
        var obj = {
        };
        obj["IDENTIFICATION_CODE"] = $(v).find("input[data-datafield='APPLICANT_DETAIL.IDENTIFICATION_CODE2']").val();
        $(v).find("select").each(function (num, val) {
            obj[$(val).attr("data-datafield")] = {
                Code: $(val).find("option:selected").val(),
                Text: $(val).find("option:selected").text()
            };
        });
        data_APPLICANT_DETAIL.push(obj);
    });
    //机构明细表
    $.MvcSheetUI.GetElement("COMPANY_DETAIL").find("div.rows").each(function (n, v) {
        var obj = {
        };
        obj["IDENTIFICATION_CODE"] = $(v).find("input[data-datafield='COMPANY_DETAIL.IDENTIFICATION_CODE3']").val();
        $(v).find("select").each(function (num, val) {
            obj[$(val).attr("data-datafield")] = {
                Code: $(val).find("option:selected").val(),
                Text: $(val).find("option:selected").text()
            };
        });
        data_COMPANY_DETAIL.push(obj);
    });
    //地址明细表
    $.MvcSheetUI.GetElement("ADDRESS").find("div.rows").each(function (n, v) {
        var obj = {
        };
        obj["IDENTIFICATION_CODE"] = $(v).find("input[data-datafield='ADDRESS.IDENTIFICATION_CODE4']").val();
        obj["ADDRESS_CODE"] = $(v).find("input[data-datafield='ADDRESS.ADDRESS_CODE']").val();
        $(v).find("select").each(function (num, val) {
            obj[$(val).attr("data-datafield")] = {
                Code: $(val).find("option:selected").val(),
                Text: $(val).find("option:selected").text()
            };
        });
        data_ADDRESS.push(obj);
    });
    //电话明细表
    $.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX").find("tr.rows").each(function (n, v) {
        var obj = {
        };
        obj["IDENTIFICATION_CODE"] = $(v).find("input[data-datafield='APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5']").val();
        obj["ADDRESS_CODE"] = $(v).find("input[data-datafield='APPLICANT_PHONE_FAX.ADDRESS_CODE5']").val();
        obj["PHONE_SEQ_ID"] = $(v).find("input[data-datafield='APPLICANT_PHONE_FAX.PHONE_SEQ_ID']").val();
        $(v).find("select").each(function (num, val) {
            obj[$(val).attr("data-datafield")] = {
                Code: $(val).find("option:selected").val(),
                Text: $(val).find("option:selected").text()
            };
        });
        data_APPLICANT_PHONE_FAX.push(obj);
    });
    //工作明细表
    $.MvcSheetUI.GetElement("EMPLOYER").find("div.rows").each(function (n, v) {
        var obj = {
        };
        obj["IDENTIFICATION_CODE"] = $(v).find("input[data-datafield='EMPLOYER.IDENTIFICATION_CODE6']").val();
        obj["EMPLOYEE_LINE_ID"] = $(v).find("input[data-datafield='EMPLOYER.EMPLOYEE_LINE_ID']").val();
        $(v).find("select").each(function (num, val) {
            obj[$(val).attr("data-datafield")] = {
                Code: $(val).find("option:selected").val(),
                Text: $(val).find("option:selected").text()
            };
        });
        data_EMPLOYER.push(obj);
    });
    //联系人
    $.MvcSheetUI.GetElement("PERSONNAL_REFERENCE").find("div.rows").each(function (n, v) {
        var obj = {
        };
        obj["IDENTIFICATION_CODE"] = $(v).find("input[data-datafield='PERSONNAL_REFERENCE.IDENTIFICATION_CODE10']").val();
        obj["LINE_ID"] = $(v).find("input[data-datafield='PERSONNAL_REFERENCE.LINE_ID10']").val();
        $(v).find("select").each(function (num, val) {
            obj[$(val).attr("data-datafield")] = {
                Code: $(val).find("option:selected").val(),
                Text: $(val).find("option:selected").text()
            };
        });
        data_CONTACT.push(obj);
    });
    //资产信息
    $.MvcSheetUI.GetElement("VEHICLE_DETAIL").find("div.rows[data-row='1']").find("select").each(function (n, v) {
        data_VEHICLE_DETAIL[$(v).attr("data-datafield")] = {
            Code: $(v).find("option:selected").val(),
            Text: $(v).find("option:selected").text()
        };
    });
    //金融条款
    $.MvcSheetUI.GetElement("CONTRACT_DETAIL").find("div.rows[data-row='1']").find("select").each(function (n, v) {
        data_CONTRACT_DETAIL[$(v).attr("data-datafield")] = {
            Code: $(v).find("option:selected").val(),
            Text: $(v).find("option:selected").text()
        };
    });

    data["APPLICATION"] = data_APPLICATION;
    data["APPLICANT_DETAIL"] = data_APPLICANT_DETAIL;
    data["COMPANY_DETAIL"] = data_COMPANY_DETAIL;
    data["ADDRESS"] = data_ADDRESS;
    data["APPLICANT_PHONE_FAX"] = data_APPLICANT_PHONE_FAX;
    data["EMPLOYER"] = data_EMPLOYER;
    data["PERSONNAL_REFERENCE"] = data_CONTACT;
    data["VEHICLE_DETAIL"] = data_VEHICLE_DETAIL;
    data["CONTRACT_DETAIL"] = data_CONTRACT_DETAIL;
    console.log(data);
    //$.MvcSheetUI.SetControlValue("DDL_DATASOURCE", JSON.stringify(data));
    return JSON.stringify(data);
}
// 保存前事件
$.MvcSheet.SaveAction.OnActionPreDo = function () {

}

function setCYZD(applicant_detail_rows) {
    var lx = "";//金融产品类型
    var cyzd = "";//常用字段
    var ID_CARD_NBR = "";//主贷人身份证号
    var app_type = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    //------------------------------常用字段开始-----------------------------
    var ft_group = $.MvcSheetUI.GetElement("CONTRACT_DETAIL.FP_GROUP_ID", 1).find("option:checked").text();
    if (ft_group.indexOf("简化") > -1 || ft_group.indexOf("车秒") > -1 || ft_group.indexOf("高首付") > -1) {
        lx = "1";
    }
    else if (lx.indexOf("机构贷") > -1) {
        lx = "3";
    }
    else {
        lx = "2";
    }
    if (app_type == "00001") {
        applicant_detail_rows.each(function (n, row) {
            if ($(row).find("input[data-datafield=\"APPLICANT_DETAIL.IDENTIFICATION_CODE2\"]").val() == "1") {
                ID_CARD_NBR = $(row).find("input[data-datafield=\"APPLICANT_DETAIL.ID_CARD_NBR\"]").val();
                return false;
            }
        });
    }


    cyzd += $.MvcSheetUI.SheetInfo.UserCode + ",";//金融专员账号
    cyzd += lx + ",";//产品类型
    cyzd += ID_CARD_NBR + ",";//申请人证件号码
    $.MvcSheetUI.SetControlValue("cyzd", cyzd);
    //------------------------------常用字段结束-----------------------------
}

//保存调用融数接口的数据,有许多下拉框,没有对应的文本,后台取值很麻烦;
function getrsdata() {
    var param1 = "";//申请人
    var rowIndex1 = -1;//申请人标识
    var param2 = "";//共借人
    var rowIndex2 = -1;//共借人标识
    var param3 = "";//担保人
    var rowIndex3 = -1;//担保人标识
    var sfpo = "否";//共借人是否配偶
    var sfycgjr = "否";//是否隐藏共借人
    var sfycdbr = "否";//是否隐藏担保人
    var lx = "";//金融产品类型
    var cyzd = "";//常用字段
    var ID_CARD_NBR = "";
    var APPLICATION_TYPE_CODE = $.MvcSheetUI.GetControlValue("APPLICATION_TYPE_CODE");
    var rows = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows");

    var allMappingCities;
    //获取所有的映射地址
    if ($.MvcSheetUI.CacheData["AllMappingCities"]) {
        allMappingCities = $.MvcSheetUI.CacheData["AllMappingCities"];
    }
    else {
        var r_cities = getAllMappingCities();
        if (r_cities.Success) {
            allMappingCities = r_cities.Data;
            $.MvcSheetUI.CacheData["AllMappingCities"] = r_cities.Data;
        }
    }
    allMappingCities[""] = "";

    //共借人有多个的情况下，取第2个，担保人有多个情况下，取第1个；
    var co_bo_num = 0;//共借人的个数
    $(rows).each(function (n, v) {
        var id = $(v).attr("id");
        var rowIndex = $(v).attr("data-row");
        var mainType = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", rowIndex);//申请人
        var v1 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowIndex);//角色类型 I-个人，C-机构
        var v2 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", rowIndex);//角色是否为担保人
        var v3 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IS_INACTIVE_IND", rowIndex);//角色是否隐藏
        var id_code = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", rowIndex);
        if (v1 == "I") {
            if (mainType == "Y" && rowIndex1 == -1) {//申请人
                rowIndex1 = id_code;
                //return true;
            }
            else if (mainType == "" && v2 == "") {
                co_bo_num++;
                if (co_bo_num <= 2) {
                    rowIndex2 = id_code;
                    if ($.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.SPOUSE_IND", getRowIndexByIdCode(id_code))) {//是否配偶
                        sfpo = "是";
                        //2019-01-28号：只根据人员关系表中的显示及隐藏来控制，和Spouse无关，Spouse表示是否发送给上海银行；
                        //sfycgjr = "是";
                    }
                    else {
                        sfpo = "否";
                        //2019-01-28号：只根据人员关系表中的显示及隐藏来控制，和Spouse无关，Spouse表示是否发送给上海银行；
                        //sfycgjr = "否";
                    }
                    if (v3 != "") {
                        sfycgjr = "是";
                    }
                    else {
                        sfycgjr = "否";
                    }
                }
            }
        }
        else if (v1 == "") {
            if (v2 == "I" && rowIndex3 == -1) {//担保人
                rowIndex3 = id_code;
                if (v3 != "") {
                    sfycdbr = "是";
                }
            }
        }
    })
    //------------------------------申请人开始-------------------------------
    param1 += "\"cust_name\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "FIRST_THI_NME") + "\",";
    param1 += "\"cert_tpye\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "ID_CARD_TYP", "Text") + "\",";
    ID_CARD_NBR = GetApplicantDetailValueByIdentifyCode(rowIndex1, "ID_CARD_NBR");
    var province1 = GetAddressValueByIdentifyCode(rowIndex1, "STATE_CDE4", "Text");
    param1 += "\"cert_no\":\"" + ID_CARD_NBR + "\",";
    param1 += "\"cert_end_date\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "ID_CARDEXPIRY_DTE").replace(/-/g, "/") + "\",";//dt.Rows[0]["IdCardExpiryDate"].ToString().Trim() + "\",";
    param1 += "\"mobile\":\"" + GetPhoneFaxValueByIdentifyCode(rowIndex1, "PHONE_NUMBER") + "\",";// + dt.Rows[0]["phoneNo"].ToString().Trim() + "\",";
    param1 += "\"gender\":\"" + (GetApplicantDetailValueByIdentifyCode(rowIndex1, "SEX") == "F" ? "女" : "男") + "\",";// + dt.Rows[0]["genderName"].ToString().Trim() + "\",";
    param1 += "\"birthday_year\":\"\",";
    param1 += "\"birthday_month\":\"\",";
    param1 += "\"cust_birthday\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "DATE_OF_BIRTH").replace(/-/g, "/") + "\",";// + dt.Rows[0]["DateOfBirth"].ToString().Trim() + "\",";
    param1 += "\"registered_address_province\":\"" + province1 + "\",";// + dt.Rows[0]["hukouprovinceName"].ToString().Trim() + "\",";
    param1 += "\"registered_address_city\":\"" + allMappingCities[GetAddressValueByIdentifyCode(rowIndex1, "CITY_CDE4", "Text")] + "\",";//户籍城市
    //新增居住地
    param1 += "\"residence_address_city\":\"" + allMappingCities[GetAddressValueByIdentifyCode(rowIndex1, "CurrentCity")] + "\",";
    //新增抵押城市
    param1 += "\"plege_address_city\":\"" + ($("#ctl53508").find("option:selected").text() == "直辖县级" ? $("#ctl291016").find("option:selected").text() : $("#ctl53508").find("option:selected").text()) + "\",";
    //新增经销商城市
    param1 += "\"dealer_address_city\":\"" + allMappingCities[$.MvcSheetUI.GetElement("CITY_CDE").find("option:selected").text()] + "\",";
    param1 += "\"reside_address_province\":\"" + $.MvcSheetUI.GetElement("STATE_CDE").find("option:selected").text() + "\",";// dt.Rows[0]["provinceName"].ToString().Trim() + "\",";
    param1 += "\"nationality\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "RACE_CDE", "Text") + "\",";// dt.Rows[0]["CitizenshipName"].ToString().Trim() + "\",";
    param1 += "\"nation\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "NATION_CDE", "Text") + "\",";// dt.Rows[0]["NationName"].ToString().Trim() + "\",";
    param1 += "\"education_level\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "EDUCATION_CDE", "Text") + "\",";// dt.Rows[0]["EducationName"].ToString().Trim() + "\",";
    param1 += "\"marital_status\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "MARITAL_STATUS_CDE", "Text") + "\",";// dt.Rows[0]["MaritalStatusName"].ToString().Trim() + "\",";
    param1 += "\"marital_status_check\":\"\",";
    param1 += "\"house_status\":\"" + GetAddressValueByIdentifyCode(rowIndex1, "PROPERTY_TYPE_CDE", "Text") + "\",";// dt.Rows[0]["propertytypeName"].ToString().Trim() + "\",";
    param1 += "\"employer\":\"" + GetEmployValueByIdentifyCode(rowIndex1, "NAME_2") + "\",";// dt.Rows[0]["ZjrCompanyName"].ToString().Trim() + "\",";
    param1 += "\"employer_telphone\":\"" + GetPhoneFaxValueByIdentifyCode(rowIndex1, "PHONE_NUMBER") + "\",";// dt.Rows[0]["phoneNo"].ToString().Trim() + "\",";
    param1 += "\"employer_address_province\":\"" + GetEmployValueByIdentifyCode(rowIndex1, "STATE_CDE2", "Text") + "\",";// dt.Rows[0]["comapnyProvinceName"].ToString().Trim() + "\",";
    param1 += "\"employer_address_city\":\"" + allMappingCities[GetEmployValueByIdentifyCode(rowIndex1, "CITY_CDE6", "Text")] + "\",";// dt.Rows[0]["companyCityName"].ToString().Trim() + "\",";
    param1 += "\"occupation\":\"" + GetEmployValueByIdentifyCode(rowIndex1, "DESIGNATION_CDE", "Text") + "\",";// dt.Rows[0]["positionName"].ToString().Trim() + "\",";
    param1 += "\"working_years\":\"" + GetEmployValueByIdentifyCode(rowIndex1, "TIME_IN_YEAR_2") + "\",";// dt.Rows[0]["timeinyear"].ToString().Trim() + "\",";
    param1 += "\"income_month\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "ACTUAL_SALARY") + "\",";// dt.Rows[0]["ActualSalary"].ToString().Trim() + "\",";
    //判断内外网客户
    var nww = "";
    if ($.MvcSheetUI.SheetInfo.UserCode.indexOf("98") == 0 || $.MvcSheetUI.SheetInfo.UserCode.indexOf("80000") == 0) {
        nww = "内网";
    }
    else {
        nww = "外网";
    }
    param1 += "\"customer_source\":\"" + nww + "\",";
    param1 += "\"cym\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex1, "FORMER_NAME") + "\",";// dt.Rows[0]["FormerName"].ToString().Trim() + "\",";
    param1 += "\"csdssx\":\"" + GetAddressValueByIdentifyCode(rowIndex1, "BIRTHPLEASE_PROVINCE") + "\",";// dt.Rows[0]["birthpalaceprovince"].ToString().Trim() + "\",";
    //申请人籍贯所属省市县
    param1 += "\"jgssx\":\"" + GetAddressValueByIdentifyCode(rowIndex1, "NATIVE_DISTRICT") + "\",";// dt.Rows[0]["nativedistrict"].ToString().Trim() + "\",";
    //申请人所属省市县province+city
    var ssssxq = province1 + allMappingCities[GetAddressValueByIdentifyCode(rowIndex1, "CITY_CDE4", "Text")];
    param1 += "\"ssssxq\":\"" + ssssxq + "\",";// dt.Rows[0]["hukouprovinceName"].ToString().Trim() + dt.Rows[0]["hukoucityName"].ToString().Trim() + "\",";
    param1 += "\"zz\":\"" + GetAddressValueByIdentifyCode(rowIndex1, "UNIT_NO") + "\",";// dt.Rows[0]["currentLivingAddress"].ToString().Trim() + "\",";
    var asset_condition = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.CONDITION", 1);
    param1 += "\"business_type\":\"" + (asset_condition == "N" ? "新车 New" : "二手车 Uesd") + "\",";// dt.Rows[0]["assetConditionName"].ToString().Trim() + "\",";
    param1 += "\"fours_number\":\"" + $.MvcSheetUI.SheetInfo.UserCode + "\",";// dt.Rows[0]["userName"].ToString().Trim() + "\",";
    param1 += "\"dealer_name\":\"" + $.MvcSheetUI.SheetInfo.UserName + "\",";// dt.Rows[0]["dealerName"].ToString().Trim() + "\",";
    param1 += "\"vehicle_model\":\"" + $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ASSET_BRAND_DSC", 1) + "\",";// dt.Rows[0]["brandName"].ToString().Trim() + "\",";
    param1 += "\"vehicle_brand\":\"" + $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ASSET_MAKE_DSC", 1) + "\",";// dt.Rows[0]["assetMakeName"].ToString().Trim() + "\",";
    param1 += "\"vehicle_purpose\":\"" + $.MvcSheetUI.GetElement("VEHICLE_DETAIL.USAGE7", 1).find("option:selected").text() + "\",";// dt.Rows[0]["purposeName"].ToString().Trim() + "\",";
    param1 += "\"vehicle_price\":\"" + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1) + "\",";// dt.Rows[0]["vehicleprice"].ToString().Trim() + "\",";
    param1 += "\"application_amount\":\"" + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1) + "\",";// dt.Rows[0]["financedamount"].ToString().Trim() + "\",";
    param1 += "\"application_term\":\"" + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1) + "\",";// dt.Rows[0]["termMonth"].ToString().Trim() + "\",";
    param1 += "\"product_group\":\"" + $.MvcSheetUI.GetElement("CONTRACT_DETAIL.FP_GROUP_ID", 1).find("option:selected").text() + "\",";// dt.Rows[0]["productGroupName"].ToString().Trim() + "\",";
    param1 += "\"product_type\":\"" + $.MvcSheetUI.GetElement("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1).find("option:selected").text() + "\",";// dt.Rows[0]["productTypeName"].ToString().Trim() + "\",";
    param1 += "\"first_payments_ratio\":\"" + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SECURITY_DEPOSIT_PCT", 1) + "\",";// dt.Rows[0]["downpaymentrate"].ToString().Trim() + "\",";
    param1 += "\"first_payments_amount\":\"" + $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.CASH_DEPOSIT", 1) + "\",";// dt.Rows[0]["downpaymentamount"].ToString().Trim() + "\",";
    param1 += "\"vehicle_identification_value\":\"" + $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", 1) + "\",";// dt.Rows[0]["assetPrice"].ToString().Trim() + "\",";
    param1 += "\"if_partner\":\"\",";
    //判断是否包括附加费贷款（是/否）---待确认
    var fjf = parseFloat(($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1) == "" ? "0" : $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1)));
    param1 += "\"if_extra_charge_loan\":\"" + (fjf > 0 ? "是" : "否") + "\",";// (dt.Rows[0]["accessoriesInd"].ToString().Trim()) + "\",";
    param1 += "\"if_rereq_complete\":\"是\",";
    param1 += "\"if_mortgage\":\"" + (GetApplicantDetailValueByIdentifyCode(rowIndex1, "LIENEE") == true ? "是" : "否") + "\",";// dt.Rows[0]["lienee"].ToString().Trim() + "\",";
    param1 += "\"vehicle_manufacture_date\":\"" + $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.RELEASE_DTE", 1) + "\",";// dt.Rows[0]["releaseDate"].ToString().Trim() + "\",";
    param1 += "\"vehicle_kilometers\":\"" + $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ODOMETER_READING", 1) + "\",";// dt.Rows[0]["odometer"].ToString().Trim() + "\",";
    param1 += "\"if_data_complete\":\"是\"";
    //------------------------------申请人结束---------------------

    //------------------------------共借人开始-------------------------------
    //共借人和申请人的关系
    param2 += "\"if_partner_relatives\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "GUARANTOR_RELATIONSHIP_CDE", "Text") + "\",";// dt.Rows[0]["GjrelationShipName"].ToString().Trim() + "\",";
    param2 += "\"if_partner_mate\":\"" + sfpo + "\",";// (dt.Rows[0]["GjrelationShipName"].ToString().Contains("配偶") ? "是" : "否") + "\",";
    param2 += "\"if_hidden_partner\":\"" + sfycgjr + "\",";// dt.Rows[0]["IS_INACTIVE_IND"].ToString().Trim() + "\",";
    param2 += "\"if_hidden_partner2\":\"" + sfycdbr + "\",";// dt.Rows[0]["Gjspouse"].ToString().Trim() + "\",";
    param2 += "\"mate_name\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "FIRST_THI_NME") + "\",";// dt.Rows[0]["GjThaiFirstName"].ToString().Trim() + "\",";
    param2 += "\"mate_cert_type\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "ID_CARD_TYP", "Text") + "\",";// dt.Rows[0]["GjIdCarTypeName"].ToString().Trim() + "\",";
    param2 += "\"mate_cert_no\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "ID_CARD_NBR") + "\",";// dt.Rows[0]["GjIdCardNo"].ToString().Trim() + "\",";
    param2 += "\"mate_mobile\":\"" + GetPhoneFaxValueByIdentifyCode(rowIndex2, "PHONE_NUMBER") + "\",";// dt.Rows[0]["GjphoneNo"].ToString().Trim() + "\",";
    param2 += "\"partner_name\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "FIRST_THI_NME") + "\",";// dt.Rows[0]["GjThaiFirstName"].ToString().Trim() + "\",";
    param2 += "\"partner_cert_type\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "ID_CARD_TYP", "Text") + "\",";// dt.Rows[0]["GjIdCarTypeName"].ToString().Trim() + "\",";
    param2 += "\"partner_cert_no\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "ID_CARD_NBR") + "\",";// dt.Rows[0]["GjIdCardNo"].ToString().Trim() + "\",";
    param2 += "\"partner_moblie\":\"" + GetPhoneFaxValueByIdentifyCode(rowIndex2, "PHONE_NUMBER") + "\",";// dt.Rows[0]["GjphoneNo"].ToString().Trim() + "\",";
    param2 += "\"partner_cym\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "FORMER_NAME") + "\",";// dt.Rows[0]["GjFormerName"].ToString().Trim() + "\",";
    var partner_xb = GetApplicantDetailValueByIdentifyCode(rowIndex2, "SEX");
    partner_xb = (partner_xb == "" ? "" : (partner_xb == "F" ? "女" : "男"));
    param2 += "\"partner_xb\":\"" + partner_xb + "\",";// dt.Rows[0]["GjgenderName"].ToString().Trim() + "\",";
    param2 += "\"partner_mz\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "NATION_CDE", "Text") + "\",";// dt.Rows[0]["GjNationName"].ToString().Trim() + "\",";
    param2 += "\"partner_csrq\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "DATE_OF_BIRTH").replace(/-/g, "/") + "\",";// dt.Rows[0]["GjDateOfBirth"].ToString().Trim() + "\",";
    param2 += "\"partner_whcd\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "EDUCATION_CDE", "Text") + "\",";// dt.Rows[0]["GjEducationName"].ToString().Trim() + "\",";
    param2 += "\"partner_hyzk\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex2, "MARITAL_STATUS_CDE", "Text") + "\",";// dt.Rows[0]["GjMaritalStatusName"].ToString().Trim() + "\",";
    param2 += "\"partner_fwcs\":\"" + GetEmployValueByIdentifyCode(rowIndex2, "NAME_2") + "\",";// dt.Rows[0]["GjcompanyName"].ToString().Trim() + "\",";
    param2 += "\"partner_csdssx\":\"" + GetAddressValueByIdentifyCode(rowIndex2, "BIRTHPLEASE_PROVINCE") + "\",";// dt.Rows[0]["Gjbirthpalaceprovince"].ToString().Trim() + "\",";
    param2 += "\"partner_jgssx\":\"" + GetAddressValueByIdentifyCode(rowIndex2, "NATIVE_DISTRICT") + "\",";// dt.Rows[0]["Gjnativedistrict"].ToString().Trim() + "\",";
    var ssssxq = GetAddressValueByIdentifyCode(rowIndex2, "STATE_CDE4", "Text") + allMappingCities[GetAddressValueByIdentifyCode(rowIndex2, "CITY_CDE4", "Text")];
    param2 += "\"partner_ssssxq\":\"" + ssssxq + "\",";// dt.Rows[0]["GjhukouprovinceName"].ToString().Trim() + dt.Rows[0]["GjhukoucityName"].ToString().Trim() + "\",";
    param2 += "\"partner_zz\":\"" + GetAddressValueByIdentifyCode(rowIndex2, "UNIT_NO") + "\",";// dt.Rows[0]["GjcurrentLivingAddress"].ToString().Trim() + "\",";
    //------------------------------共借人结束-------------------------------

    //------------------------------担保人开始-------------------------------
    param3 += "\"if_guarantee\":\"\",";
    param3 += "\"guarantee_name\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "FIRST_THI_NME") + "\",";// dt.Rows[0]["DbThaiFirstName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_cert_type\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "ID_CARD_TYP", "Text") + "\",";// dt.Rows[0]["DbIdCarTypeName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_cert_no\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "ID_CARD_NBR") + "\",";// dt.Rows[0]["DbIdCardNo"].ToString().Trim() + "\",";
    param3 += "\"guarantee_mobile\":\"" + GetPhoneFaxValueByIdentifyCode(rowIndex3, "PHONE_NUMBER") + "\",";// dt.Rows[0]["DbphoneNo"].ToString().Trim() + "\",";
    param3 += "\"guarantee_cym\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "FORMER_NAME") + "\",";// dt.Rows[0]["DbFormerName"].ToString().Trim() + "\",";
    var guarantee_xb = GetApplicantDetailValueByIdentifyCode(rowIndex3, "SEX");
    guarantee_xb == ("" ? "" : (guarantee_xb == "F" ? "女" : "男"));
    param3 += "\"guarantee_xb\":\"" + guarantee_xb + "\",";// dt.Rows[0]["DbgenderName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_mz\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "NATION_CDE", "Text") + "\",";// dt.Rows[0]["DbNationName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_csrq\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "DATE_OF_BIRTH") + "\",";// dt.Rows[0]["DbDateOfBirth"].ToString().Trim() + "\",";
    param3 += "\"guarantee_whcd\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "EDUCATION_CDE", "Text") + "\",";// dt.Rows[0]["DbEducationName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_hyzk\":\"" + GetApplicantDetailValueByIdentifyCode(rowIndex3, "MARITAL_STATUS_CDE", "Text") + "\",";// dt.Rows[0]["DbMaritalStatusName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_fwcs\":\"" + GetEmployValueByIdentifyCode(rowIndex3, "NAME_2") + "\",";// dt.Rows[0]["DbcompanyName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_csdssx\":\"" + GetAddressValueByIdentifyCode(rowIndex3, "BIRTHPLEASE_PROVINCE") + "\",";// dt.Rows[0]["Dbbirthpalaceprovince"].ToString().Trim() + "\",";
    param3 += "\"guarantee_jgssx\":\"" + GetAddressValueByIdentifyCode(rowIndex3, "NATIVE_DISTRICT") + "\",";// dt.Rows[0]["Dbnativedistrict"].ToString().Trim() + "\",";
    var ssssxq3 = GetAddressValueByIdentifyCode(rowIndex3, "STATE_CDE4", "Text") + allMappingCities[GetAddressValueByIdentifyCode(rowIndex3, "CITY_CDE4", "Text")];
    param3 += "\"guarantee_ssssxq\":\"" + ssssxq3 + "\",";// dt.Rows[0]["DbhukouprovinceName"].ToString().Trim() + dt.Rows[0]["DbhukoucityName"].ToString().Trim() + "\",";
    param3 += "\"guarantee_zz\":\"" + GetAddressValueByIdentifyCode(rowIndex3, "UNIT_NO") + "\",";// dt.Rows[0]["DbcurrentLivingAddress"].ToString().Trim() + "\"";
    //------------------------------担保人结束-------------------------------
    //常用字段已移动到方法setCYZD()中
    //------------------------------NCIIC页面使用-----------------------------
    var nciic_result = {};//需要增加公司地址
    nciic_result["company_address"] = GetEmployValueByIdentifyCode(rowIndex1, "ADDRESS_ONE_2");
    nciic_result["partner_company_address"] = GetEmployValueByIdentifyCode(rowIndex2, "ADDRESS_ONE_2");
    nciic_result["guarantee_company_address"] = GetEmployValueByIdentifyCode(rowIndex3, "ADDRESS_ONE_2");
    saveMessageToAttachment("nciic", JSON.stringify(nciic_result));//保存
    //------------------------------NCIIC页面使用-----------------------------

    return (param3 + param2 + param1).replace(new RegExp('\"null\"', "gm"), '\"\"');
}

function getRowIndexByIdCode(idcode) {
    var index = -1;
    $.MvcSheetUI.GetElement("APPLICANT_DETAIL").find("div.rows").each(function (n, v) {
        if ($.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", $(v).attr("data-row")) == idcode) {
            index = $(v).attr("data-row");
            return true;
        }
    });
    return index;
}

//资产信息中担保方式的change事件,
function gurantee_option_chg() {
    //H3格式:  a;b;   cap:  a,b
    var v_g = $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.GURANTEE_OPTION_H3", 1);
    v_g = v_g.replace(/;/g, ",");
    if (v_g != "")
        v_g = v_g.substring(0, v_g.length - 1);
    $.MvcSheetUI.SetControlValue("VEHICLE_DETAIL.GURANTEE_OPTION", v_g, 1);
}
//设置公司年份
function setCompanyYear(con) {
    var v = $(con).val();
    var c = new Date();
    var d = new Date(v);
    var y = c.getFullYear() - d.getFullYear();
    var index = $(con).closest("div.rows").attr("data-row");
    $.MvcSheetUI.SetControlValue("COMPANY_DETAIL.ESTABLISHED_SINCE_YY", y, index);
}

function setText(con, field) {
    var v = $(con).find("option:selected").text();
    $.MvcSheetUI.SetControlValue(field, v, 1);
}

function app_type_remove(row, args) {
    debugger;
    var index = row.attr("data-row");
    $("div[data-datafield=\"APPLICANT_TYPE\"] div.slider-slide[data-row='" + index + "'] .delete").click();
    $("div[data-datafield=\"ADDRESS\"] div.slider-slide[data-row='" + index + "'] .delete").click();
    setTimeout("set_index()", 300);
}

function add_mobile_rows(type) {
    //人员关系表添加一行
    $("div[data-datafield=\"APPLICANT_TYPE\"] .buttons-right").find("span").click();
    //个人信息表增加一行
    $("div[data-datafield=\"APPLICANT_DETAIL\"] .buttons-right").find("span").click();
    //地址信息表增加一行
    $("div[data-datafield=\"ADDRESS\"] .buttons-right").find("span").click();
    //隐藏复制
    $(".copy").hide();
    //隐藏地址表的title；
    $("div[data-datafield=\"ADDRESS\"] .list-title").hide();

    var rows = $("div[data-datafield=\"APPLICANT_TYPE\"] .slider-slide");
    if (rows.length == 1) {
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", "1", 1);
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", "Y", 1);
        if (type == "A") {
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", "I", 1);
        }
        else if (type == "G") {
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", "I", 1);
        }

        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", "1", 1);
        $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_CODE", "1", 1);
        $.MvcSheetUI.SetControlValue("ADDRESS.IDENTIFICATION_CODE4", "1", 1);
    }
    else {
        var max_identify_code = 0;
        $(rows).each(function (n, v) {
            if (n == rows.length - 1) {
                return false;
            }
            var index = $(v).attr("data-row");
            var id = parseInt($.MvcSheetUI.GetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", index));
            if (id > max_identify_code)
                max_identify_code = id;
        });
        max_identify_code = max_identify_code + 1;
        var newRow_Index = $(rows).last().attr("data-row");
        $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.IDENTIFICATION_CODE1", max_identify_code, newRow_Index);

        if (type == "A") {
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", "I", newRow_Index);
        }
        else if (type == "G") {
            $.MvcSheetUI.SetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", "I", newRow_Index);
        }

        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", max_identify_code, newRow_Index);
        $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_CODE", "1", newRow_Index);
        $.MvcSheetUI.SetControlValue("ADDRESS.IDENTIFICATION_CODE4", max_identify_code, newRow_Index);
    }
}

function set_index() {
    var buttons = $("div[data-datafield=\"APPLICANT_TYPE\"] .buttons-left ion-scroll:eq(1) button").clone();
    $(buttons).each(function (n, v) {
        $(v).unbind("click").bind("click", function () {
            $(this).parent().children("button").removeClass("button-blue");
            $(this).addClass("button-blue");
            var index = $(this).attr("index");
            $("div[data-datafield=\"APPLICANT_TYPE\"] .buttons-left ion-scroll:eq(1) button[index='" + index + "']").click();
            $("div[data-datafield=\"APPLICANT_DETAIL\"] .buttons-left ion-scroll:eq(1) button[index='" + index + "']").click();
            $("div[data-datafield=\"ADDRESS\"] .buttons-left ion-scroll:eq(1) button[index='" + index + "']").click();
        });
    });
    $("#myindex div.scroll").children().remove();
    $("#myindex div.scroll").append(buttons);
}

//复制主借人的地址信息及电话信息
function copy_mainApp_address() {
    var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
    //清空当前的所有地址
    var ads_manager = $.MvcSheetUI.GetElement("ADDRESS").SheetUIManager();
    $("div[data-datafield='ADDRESS'] div.rows").each(function (num_r, row) {
        var index = $(row).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index) == identifycode) {
            ads_manager._Deleterow($("div[data-datafield='ADDRESS'] a[href='#" + $(row).attr("id") + "'] i"), false);
        }
    });

    var mainApp_data = [];
    $.MvcSheetUI.GetElement("ADDRESS").find("div.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", index) == "1") {
            var address = {};
            address["ADDRESS.UNIT_NO"] = $.MvcSheetUI.GetControlValue("ADDRESS.UNIT_NO", index);
            address["ADDRESS.DEFAULT_ADDRESS"] = $.MvcSheetUI.GetControlValue("ADDRESS.DEFAULT_ADDRESS", index);
            address["ADDRESS.ADDRESS_ID"] = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_ID", index);
            address["ADDRESS.HUKOU_ADDRESS"] = $.MvcSheetUI.GetControlValue("ADDRESS.HUKOU_ADDRESS", index);
            address["ADDRESS.REGISTERED_ADDRESS"] = $.MvcSheetUI.GetControlValue("ADDRESS.REGISTERED_ADDRESS", index);
            address["ADDRESS.COUNTRY_CDE"] = $.MvcSheetUI.GetControlValue("ADDRESS.COUNTRY_CDE", index);
            address["ADDRESS.STATE_CDE4"] = $.MvcSheetUI.GetControlValue("ADDRESS.STATE_CDE4", index);
            address["ADDRESS.CITY_CDE4"] = $.MvcSheetUI.GetControlValue("ADDRESS.CITY_CDE4", index);
            address["ADDRESS.NATIVE_DISTRICT"] = $.MvcSheetUI.GetControlValue("ADDRESS.NATIVE_DISTRICT", index);
            address["ADDRESS.BIRTHPLEASE_PROVINCE"] = $.MvcSheetUI.GetControlValue("ADDRESS.BIRTHPLEASE_PROVINCE", index);
            address["ADDRESS.POST_CODE_2"] = $.MvcSheetUI.GetControlValue("ADDRESS.POST_CODE_2", index);
            address["ADDRESS.ADDRESS_TYPE_CDE"] = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_TYPE_CDE", index);
            address["ADDRESS.ADDRESS_STATUS"] = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_STATUS", index);
            address["ADDRESS.PROPERTY_TYPE_CDE"] = $.MvcSheetUI.GetControlValue("ADDRESS.PROPERTY_TYPE_CDE", index);
            address["ADDRESS.RESIDENCE_TYPE_CDE"] = $.MvcSheetUI.GetControlValue("ADDRESS.RESIDENCE_TYPE_CDE", index);

            address["ADDRESS.LIVING_FROM_DTE"] = $.MvcSheetUI.GetControlValue("ADDRESS.LIVING_FROM_DTE", index);
            address["ADDRESS.HOME_VALUE"] = $.MvcSheetUI.GetControlValue("ADDRESS.HOME_VALUE", index);
            address["ADDRESS.TIME_IN_YEAR"] = $.MvcSheetUI.GetControlValue("ADDRESS.TIME_IN_YEAR", index);
            address["ADDRESS.TIME_IN_MONTH"] = $.MvcSheetUI.GetControlValue("ADDRESS.TIME_IN_MONTH", index);
            var address_code = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_CODE", index);
            var tel_data = [];
            $.MvcSheetUI.GetElement("APPLICANT_PHONE_FAX").find("tr.rows").each(function (number, tel) {
                var tel_index = $(tel).attr("data-row");
                if ($.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5", tel_index) == "1" &&
                    $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.ADDRESS_CODE5", tel_index) == address_code) {
                    tel_d = {};
                    //有顺序问题，须先给电话类型赋值，再给电话赋值
                    tel_d["APPLICANT_PHONE_FAX.PHONE_TYPE_CDE"] = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_TYPE_CDE", tel_index);
                    tel_d["APPLICANT_PHONE_FAX.COUNTRY_CODE"] = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.COUNTRY_CODE", tel_index);
                    tel_d["APPLICANT_PHONE_FAX.AREA_CODE"] = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.AREA_CODE", tel_index);
                    tel_d["APPLICANT_PHONE_FAX.PHONE_NUMBER"] = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.PHONE_NUMBER", tel_index);
                    tel_d["APPLICANT_PHONE_FAX.EXTENTION_NBR"] = $.MvcSheetUI.GetControlValue("APPLICANT_PHONE_FAX.EXTENTION_NBR", tel_index);
                    tel_data.push(tel_d);
                }
            });
            address["APPLICANT_PHONE_FAX"] = tel_data;
            mainApp_data.push(address);
        }
    });
    console.log("copied");

    $(mainApp_data).each(function (num, data) {
        address_add(identifycode);
        var cur_add_index = $("div[data-datafield='ADDRESS'] div.rows:last").attr("data-row");
        var add_code = $.MvcSheetUI.GetControlValue("ADDRESS.ADDRESS_CODE", cur_add_index);
        for (var p in data) {
            if (p == "APPLICANT_PHONE_FAX") {
                $(data[p]).each(function (num, tel) {
                    tel_fax_add(identifycode, add_code);
                    var cur_tel_index = $("table[data-datafield='APPLICANT_PHONE_FAX'] tr.rows:last").attr("data-row");
                    for (var t in tel) {
                        $.MvcSheetUI.SetControlValue(t, tel[t], cur_tel_index);
                    }
                });
            } else {
                $.MvcSheetUI.SetControlValue(p, data[p], cur_add_index);
            }
        }
    });
    $.MvcSheetUI.GetElement("IDENTIFICATION_ID").change();
}

//复制主借人的工作信息
function copy_mainApp_workInfo() {
    //清空当前的所有地址
    var identifycode = $.MvcSheetUI.GetControlValue("IDENTIFICATION_ID");
    var emp_manager = $.MvcSheetUI.GetElement("EMPLOYER").SheetUIManager();
    $("div[data-datafield='EMPLOYER'] div.rows").each(function (num_r, row) {
        var index = $(row).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("EMPLOYER.IDENTIFICATION_CODE6", index) == identifycode) {
            emp_manager._Deleterow($("div[data-datafield='EMPLOYER'] a[href='#" + $(row).attr("id") + "'] i"), false);
        }
    });


    var mainApp_data = [];
    $.MvcSheetUI.GetElement("EMPLOYER").find("div.rows").each(function (n, v) {
        var index = $(v).attr("data-row");
        if ($.MvcSheetUI.GetControlValue("EMPLOYER.IDENTIFICATION_CODE6", index) == "1") {
            var employer = {};
            employer["EMPLOYER.NAME_2"] = $.MvcSheetUI.GetControlValue("EMPLOYER.NAME_2", index);
            employer["EMPLOYER.BUSINESS_NATURE_CDE"] = $.MvcSheetUI.GetControlValue("EMPLOYER.BUSINESS_NATURE_CDE", index);
            employer["EMPLOYER.STATE_CDE2"] = $.MvcSheetUI.GetControlValue("EMPLOYER.STATE_CDE2", index);
            employer["EMPLOYER.CITY_CDE6"] = $.MvcSheetUI.GetControlValue("EMPLOYER.CITY_CDE6", index);
            employer["EMPLOYER.TIME_IN_YEAR_2"] = $.MvcSheetUI.GetControlValue("EMPLOYER.TIME_IN_YEAR_2", index);
            employer["EMPLOYER.TIME_IN_MONTH_2"] = $.MvcSheetUI.GetControlValue("EMPLOYER.TIME_IN_MONTH_2", index);
            employer["EMPLOYER.ADDRESS_ONE_2"] = $.MvcSheetUI.GetControlValue("EMPLOYER.ADDRESS_ONE_2", index);
            employer["EMPLOYER.DESIGNATION_CDE"] = $.MvcSheetUI.GetControlValue("EMPLOYER.DESIGNATION_CDE", index);
            employer["EMPLOYER.PHONE"] = $.MvcSheetUI.GetControlValue("EMPLOYER.PHONE", index);
            employer["EMPLOYER.FAX"] = $.MvcSheetUI.GetControlValue("EMPLOYER.FAX", index);
            employer["EMPLOYER.POST_CODE_3"] = $.MvcSheetUI.GetControlValue("EMPLOYER.POST_CODE_3", index);
            employer["EMPLOYER.EMPLOYER_TYPE"] = $.MvcSheetUI.GetControlValue("EMPLOYER.EMPLOYER_TYPE", index);
            employer["EMPLOYER.NUMBER_OF_EMPLOYEES"] = $.MvcSheetUI.GetControlValue("EMPLOYER.NUMBER_OF_EMPLOYEES", index);
            employer["EMPLOYER.JOB_DESCRIPTION"] = $.MvcSheetUI.GetControlValue("EMPLOYER.JOB_DESCRIPTION", index);
            employer["EMPLOYER.EMPLOYER_COMMENT"] = $.MvcSheetUI.GetControlValue("EMPLOYER.EMPLOYER_COMMENT", index);

            mainApp_data.push(employer);
        }
    });
    console.log("copied");

    $(mainApp_data).each(function (num, data) {
        employee_add(identifycode);
        var cur_add_index = $("div[data-datafield='EMPLOYER'] div.rows:last").attr("data-row");
        for (var p in data) {
            $.MvcSheetUI.SetControlValue(p, data[p], cur_add_index);
        }
    });
    $.MvcSheetUI.GetElement("IDENTIFICATION_ID").change();
}

//页面初始化之前
$.MvcSheet.PreInit = function () {
    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
        var app_num = $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_NUMBER.V;
        if (app_num && app_num.indexOf("Br-") == 0) {//去掉取消流程按钮
            $.MvcSheetUI.SheetInfo.PermittedActions.CancelInstance = false;
        }
    }
}

//校验还款计划明细是否正常
function check_HK_Plan() {
    var p_id = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1);
    //获取还款计划表，参数：总价，贷款金额，尾期金额，期数，产品ID

    var total_price = calculate("add", $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1), $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.ACCESSORY_AMT", 1));
    //以下写法有BUG，因为总价是通过计算规则计算出来的。如果金额发生修改会触发Change事件，但是此时还未计算，显示的是上一次的结果；
    //var total_price = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.SALE_PRICE", 1);//总价
    var dk_amt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1);//贷款金额
    var wq_amt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.BALLOON_AMOUNT", 1);//尾期金额
    var qs = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);//期数
    var product_id = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.FINANCIAL_PRODUCT_ID", 1);//产品ID

    var v = get_hk_plan(total_price, dk_amt, wq_amt, qs, product_id);
    var success = true;

    var ele_plan_simple = $.MvcSheetUI.GetElement("PMS_RENTAL_DETAIL");

    if (!v.Plan_Simple || v.Plan_Simple == undefined) {
        return "";
    }

    if (ele_plan_simple.find("tr.rows").length != v.Plan_Simple.length) {
        return "简版还款计划未生成或者生成有误";
    }

    $(v.Plan_Simple).each(function (n, v_plan) {
        if ($.MvcSheetUI.GetControlValue("PMS_RENTAL_DETAIL.RENTAL_AMT", n + 1) != v_plan.RENTAL_AMT ||
            $.MvcSheetUI.GetControlValue("PMS_RENTAL_DETAIL.EQUAL_RENTAL_AMT", n + 1) != v_plan.EQUAL_RENTAL_AMT) {
            success = false;
            return false;
        }
    });
    if (!success)
        return "简版还款计划生成有误";
    return "";
}

//检查共借人是否隐藏状态
function validate_glyphicon() {
    var rows = $.MvcSheetUI.GetElement("APPLICANT_TYPE").find("tr.rows");

    var glyphicon = false;

    $(rows).each(function (n, v) {
        var id = $(v).attr("id");
        var rowIndex = $(v).attr("data-row");

        var mainType = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.MAIN_APPLICANT", rowIndex);

        var v1 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.APPLICANT_TYPE", rowIndex);
        var v2 = $.MvcSheetUI.GetControlValue("APPLICANT_TYPE.GUARANTOR_TYPE", rowIndex);

        var displayName = "";

        if (mainType && mainType != "") {
            displayName = "借款人";
        }
        else {
            if (v1 != "")
                displayName = "共同借款人";
            else if (v2 != "")
                displayName = "担保人";
        }

        if (displayName == "共同借款人") {

            var i = $("#li_" + id).find("i.glyphicon-eye-close");

            if (i.length > 0) {
                glyphicon = true;
            }
        }

    });

    return glyphicon;
}

//给籍贯，出生地省市赋值
function set_Race_Province(con) {
    var rowindex = $(con).closest("div.rows").attr("data-row");
    var identify_code = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", rowindex);
    var app_type = get_app_type_info(identify_code);
    if (app_type.APPLICANT_TYPE == "I" || app_type.GUARANTOR_TYPE == "I") {
        var t = $.MvcSheetUI.GetElement("ADDRESS.STATE_CDE4", rowindex).find("option:selected").text() + $.MvcSheetUI.GetElement("ADDRESS.CITY_CDE4", rowindex).find("option:selected").text();
        $.MvcSheetUI.SetControlValue("ADDRESS.NATIVE_DISTRICT", t, rowindex);
        $.MvcSheetUI.SetControlValue("ADDRESS.BIRTHPLEASE_PROVINCE", t, rowindex);
    }
}

//给注册号赋值；
function setSTSValue(con) {
    var rowindex = $(con).closest("div.rows").attr("data-row");
    $.MvcSheetUI.SetControlValue("COMPANY_DETAIL.COMPANY_STS", $(con).val(), rowindex);
}

//给注册号赋值；
function setRegisterAddress(con) {
    var rowindex = $(con).closest("div.rows").attr("data-row");
    var identifycode = $.MvcSheetUI.GetControlValue("ADDRESS.IDENTIFICATION_CODE4", rowindex);
    var info = get_app_type_info(identifycode);
    if (info.APPLICANT_TYPE == "C" || info.GUARANTOR_TYPE == "C") {
        $.MvcSheetUI.SetControlValue("ADDRESS.REGISTERED_ADDRESS", $(con).val(), rowindex);
    }
}

//获取预审批的信息
function getYSPInfo(name, idCard) {

    var yspInfo = {};

    $.ajax({
        url: "/Portal/Api/YSP_BankCards",
        data: { Name: name, IdCardNo: idCard },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {

            if (result != "" && result != null) {

                yspInfo.bankcard1 = result.bankcard1;

                yspInfo.bankcard2 = result.bankcard2;

                yspInfo.phone = result.phone;
            }

        },
        //error: function (msg) {
        //    shakeMsg("获取预审批信息出错！");
        //},
        error: function (msg) {// 19.7 
            var err = '获取预审批信息出错';
            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {
                err = msg.responseText;
            }
            shakeMsg(err + ',异常代码=' + msg.status);
            return false;
        }
    });

    return yspInfo;
}