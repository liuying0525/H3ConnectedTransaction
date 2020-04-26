//获取留言信息
function getmsg() {
    var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    $.ajax({
        //url: "../../../../ajax/DZBizHandler.ashx",
        url: "/Portal/DZBizHandler/getLYInfo",// 19.6.28 wangxg
        data: { CommandName: "getLYInfo", instanceid: InstanceId },
        type: "POST",
        async: false,
        dataType: "text",
        success: function (result) {
            result = JSON.parse(result.replace(/\n/g, ""));
            $("#liuyan").find("tr:eq(0)").nextAll().remove();
            for (var i = 0; i < result.length; i++) {
                var j = i + 1;
                var tr = "<tr>";
                tr += "<td>" + j + "</td>";
                tr += "<td><label>" + result[i].LYXX + "</label></td>";
                tr += "<td><label>" + result[i].USERNAME + "</label></td>";
                tr += "<td><label>" + result[i].LYSJ.replace(/\//g, "\-") + "</label></td>";
                tr += "</tr>";
                $("#liuyan").append(tr);
            }
        },
        //error: function (msg) {
        //    shakeMsg(msg.responseText + "出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}

//添加留言
function addmsg() {
    if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {
        shakeMsg("请在保存或者提交后添加留言");
        return false;
    }
    var userid = $.MvcSheetUI.SheetInfo.UserID;
    var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    var msgval = $("#addmsg").val();
    if (!$("#addmsg").val() || $("#addmsg").val() == "") {
        shakeMsg("请先填写信息！");
        return false;
    }
    $.ajax({
        //url: "../../../../ajax/DZBizHandler.ashx",
        url: "/Portal/DZBizHandler/insertLYInfo",// 19.6.28 wangxg
        data: { CommandName: "insertLYInfo", userid: userid, instanceid: InstanceId, ly: msgval },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            $("#addmsg").val("");
            getmsg();
        },
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}

//Title点击隐藏事件
function hidediv(id, ctrl1) {
    var ctrl = document.getElementById(id);
    if ($(ctrl).css("display") != 'none') {
        $(ctrl).hide();
        //class ="nav-icon fa fa-chevron-down bannerTitle" 
        if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
            $(ctrl1).removeClass("fa-angle-double-down");
            $(ctrl1).addClass("fa-angle-double-right");
        }
        else {
            $(ctrl1).removeClass("fa-chevron-down");
            $(ctrl1).addClass("fa-chevron-right");
        }
        $(ctrl1).css({ "margin-bottom": "8px", "border-bottom": "1px solid #ccc" });
    }
    else {
        $(ctrl).show();
        if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
            $(ctrl1).removeClass("fa-angle-double-right");
            $(ctrl1).addClass("fa-angle-double-down");
        }
        else {
            $(ctrl1).removeClass("fa-chevron-right");
            $(ctrl1).addClass("fa-chevron-down");
        }
        $(ctrl1).css({ "margin-bottom": "0px", "border-bottom": "0px solid #ccc" });
    }
}
//信审表单使用
function hidediv_xs(id, ctrl1) {
    var ctrl = document.getElementById(id);
    if ($(ctrl).css("display") != 'none') {
        $(ctrl).hide();
        if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
            $(ctrl1).removeClass("fa-angle-double-down");
            $(ctrl1).addClass("fa-angle-double-right");
        }
        else {
            $(ctrl1).removeClass("fa-chevron-down");
            $(ctrl1).addClass("fa-chevron-right");
        }
        $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
    }
    else {
        $(ctrl).show();
        if ($(ctrl1).hasClass("fa-angle-double-down") || $(ctrl1).hasClass("fa-angle-double-right")) {
            $(ctrl1).removeClass("fa-angle-double-right");
            $(ctrl1).addClass("fa-angle-double-down");
        }
        else {
            $(ctrl1).removeClass("fa-chevron-right");
            $(ctrl1).addClass("fa-chevron-down");
        }
        $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
    }
}

function hideInfo(id, ctrl1) {
    if ($(ctrl1).html() == '展开更多 ∨') {
        $(ctrl1).html("收起 &and;");
        $("#" + id).show();
    }
    else {
        $(ctrl1).html("展开更多 &or;");
        $("#" + id).hide();
    }
}

function hideInfo_New(id, ctrl1) {
    if ($(ctrl1).html() == '展开更多 ∨') {
        $(ctrl1).html("收起 &and;");
        $(ctrl1).closest("div.rows").find("#" + id).show();
    }
    else {
        $(ctrl1).html("展开更多 &or;");
        $(ctrl1).closest("div.rows").find("#" +id).hide();
    }
}

//银行卡数字验证
function textnum(ts) {
    if (!/^[0-9]*$/.test($(ts).val())) {
        $(ts).val("");
        shakeMsg("请输入数字!");
    }
}

//SheetDetail 取值及赋值的方法
function SetDetailValue(code, value, rowindex) {
    var table = code.substring(0, code.indexOf('.'));
    var row = $.MvcSheetUI.GetElement(table).find("div.rows[data-row='" + rowindex + "']");
    $(row).find("input[data-datafield='" + code + "']").val(value);
}

function GetDetailValue(code, rowindex) {
    var table = code.substring(0, code.indexOf('.'));
    var row = $.MvcSheetUI.GetElement(table).find("div.rows[data-row='" + rowindex + "']");
    return $(row).find("input[data-datafield='" + code + "']").val();
}



//根据IdentifyCode的值去查找进行某个字段的值
function GetValueByIdentifyCode(detail_code, identifyCode_field, identifyCode_value, search_field, div_Or_table, text_Or_value) {
    var findEle = "";
    if (div_Or_table == "div")
        findEle = "div.rows";
    else if (div_Or_table == "table")
        findEle = "tr.rows";
    else
        return "";
    var returnvalue = "";
    var rows = $.MvcSheetUI.GetElement(detail_code).find(findEle);
    rows.each(function (n, v) {
        var val = $(v).find("input[data-datafield=\"" + detail_code + "." + identifyCode_field + "\"]").val();
        if (val != null && val != undefined && val == identifyCode_value) {
            var con = $(v).find("[data-datafield=\"" + detail_code + "." + search_field + "\"]");
            if (text_Or_value == "Text") {
                if (con.is("select")) {//select 特殊处理一下
                    returnvalue = con.find("option:selected").text();
                }
                else if (con.attr("type") == "checkbox") {//checkbox
                    returnvalue = con.prop("checked");
                }
                else {
                    returnvalue = con.text();
                }
            }
            else {
                if (con.attr("type") == "checkbox") {//checkbox
                    returnvalue = con.prop("checked");
                }
                else {
                    returnvalue = con.val();
                }
            }
            return false;
        }
    });
    return returnvalue;
}
//查询SheetDetail子表中的值
function GetDetailValueByIdentifyCode(detail_code, identifyCode_field, identifyCode_value, search_field, text_Or_value) {
    return GetValueByIdentifyCode(detail_code, identifyCode_field, identifyCode_value, search_field, "div", text_Or_value);
}
//查询SheetGridView子表中的值
function GetTableValueByIdentifyCode(detail_code, identifyCode_field, identifyCode_value, search_field, text_Or_value) {
    return GetValueByIdentifyCode(detail_code, identifyCode_field, identifyCode_value, search_field, "table", text_Or_value);
}
//获取地址的值
function GetAddressValueByIdentifyCode(id, field, text_Or_value) {
    return GetDetailValueByIdentifyCode("ADDRESS", "IDENTIFICATION_CODE4", id, field, text_Or_value);
}
//获取申请人的值
function GetApplicantDetailValueByIdentifyCode(id, field, text_Or_value) {
    return GetDetailValueByIdentifyCode("APPLICANT_DETAIL", "IDENTIFICATION_CODE2", id, field, text_Or_value);
}
//获取公司的值
function GetCompanyDetailValueByIdentifyCode(id, field, text_Or_value) {
    return GetDetailValueByIdentifyCode("COMPANY_DETAIL", "IDENTIFICATION_CODE3", id, field, text_Or_value);
}
//获取电话的值
function GetPhoneFaxValueByIdentifyCode(id, field, text_Or_value) {
    return GetTableValueByIdentifyCode("APPLICANT_PHONE_FAX", "IDENTIFICATION_CODE5", id, field, text_Or_value);
}
//获取工作的值
function GetEmployValueByIdentifyCode(id, field, text_Or_value) {
    return GetDetailValueByIdentifyCode("EMPLOYER", "IDENTIFICATION_CODE6", id, field, text_Or_value);
}


//删除子表行方法
function deleteRow(detailCode, paras, div_Or_table) {
    //循环并删除
    var detail = $.MvcSheetUI.GetElement(detailCode);
    var slt = "";
    var delete_num = 0;//删除的行数
    if (div_Or_table == "div")
        slt = "div.rows";
    else if (div_Or_table == "table")
        slt = "tr.rows";
    else
        return delete_num;

    $(detail).find(slt).each(function (n, v) {
        var isMatch = false;
        for (var key in paras) {
            if ($.MvcSheetUI.GetControlValue(detailCode + "." + key, $(v).attr("data-row")) == paras[key]) {
                isMatch = true;
            }
            else {
                isMatch = false;
                break;
            }
        }
        if (isMatch) {
            if (div_Or_table == "div") {
                var i = detail.find("a[href='#" + $(v).attr("id") + "']").find("i");
                detail.SheetUIManager()._Deleterow(i, false);
                delete_num = delete_num + 1;
                //return false;可能有多条，不能直接返回
            }
            else {
                detail.SheetUIManager()._Deleterow(v);
                delete_num = delete_num + 1;
            }
        }
    });
    if (detail == null || detail == undefined) { }
    else {
        if (div_Or_table == "div") {
            var lia = detail.children("ul").find("li:visible:first a");
            if (lia.length == 1) {
                detail.children("ul").find("li:visible:first a").tab("show");//显示当前第一个Tab页签；
            }
        }
    }
    return delete_num;
}

//删除子表行方法
function deleteRowByIdentifyCode(detailCode, identifyCode, id, div_Or_table) {
    var paras = {};
    paras[identifyCode] = id;
    return deleteRow(detailCode, paras, div_Or_table);
}

//删除个人表中的行
function delete_Individual(id) {
    deleteRowByIdentifyCode("APPLICANT_DETAIL", "IDENTIFICATION_CODE2", id, "div");
}

//删除机构表中的行
function delete_Company(id) {
    deleteRowByIdentifyCode("COMPANY_DETAIL", "IDENTIFICATION_CODE3", id, "div");
}

//删除地址表中的行
function delete_Address(id) {
    deleteRowByIdentifyCode("ADDRESS", "IDENTIFICATION_CODE4", id, "div");
}

//删除电话表中的行
function delete_Phone(id, address_code) {
    if (address_code == null || address_code == undefined) {
        deleteRowByIdentifyCode("APPLICANT_PHONE_FAX", "IDENTIFICATION_CODE5", id, "table");
    }
    else {
        var paras = {};
        paras["IDENTIFICATION_CODE5"] = id;
        paras["ADDRESS_CODE5"] = address_code;
        deleteRow("APPLICANT_PHONE_FAX", paras, "table");
    }
}

//删除联系人中的行
function delete_Contact(id) {
    deleteRowByIdentifyCode("PERSONNAL_REFERENCE", "IDENTIFICATION_CODE10", id, "div");
}

//删除工作中的行
function delete_WorkExp(id) {
    deleteRowByIdentifyCode("EMPLOYER", "IDENTIFICATION_CODE6", id, "div");
}



//保存数据,以txt的形式存放到数据库中
function saveMessageToAttachment(DataField, MessageInfo) {
    if (MessageInfo && MessageInfo != "") {
        $.ajax({
            //url: "../../../../ajax/DZBizHandler.ashx",
            url: "/Portal/DZBizHandler/saveMessageToAttachment",// 19.6.28 wangxg
            data: { CommandName: "saveMessageToAttachment", BizObjectSchemaCode: $.MvcSheetUI.SheetInfo.SchemaCode, BizObjectId: $.MvcSheetUI.SheetInfo.InstanceId, DataField: DataField, MessageInfo: MessageInfo.replace(/ and /ig, '``') },
            type: "POST",
            async: false,
            dataType: "text",
            success: function (result) {

            },
            //error: function (msg) {
            //    shakeMsg("出错了");
            //    return false;
            //},
            error: function (msg) {// 19.7 
                showJqErr(msg);
            }
        });
    }
}

//读取文件内容；
function readMessageFromAttachment(DataField) {
    var tmp = readMessageByDF($.MvcSheetUI.SheetInfo.SchemaCode, $.MvcSheetUI.SheetInfo.InstanceId, DataField);
    if (tmp) tmp = tmp.replace(/``/ig, ' and ');// wangxg 19.8
    return tmp;
}
//读取文件内容；
function readMessageByDF(schemacode, insid, datafield) {
    var ret;
    $.ajax({
        //url: "../../../../ajax/DZBizHandler.ashx",
        url: "/Portal/DZBizHandler/readMessageFromAttachment",// 19.6.28 wangxg
        data: { CommandName: "readMessageFromAttachment", BizObjectSchemaCode: schemacode, BizObjectId: insid, DataField: datafield },
        type: "POST",
        async: false,
        dataType: "text",
        success: function (result) {
            ret = result;
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //    return false;
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
    return ret;
}

//获取字符串的长度，汉字2，字母或者数字为1；
function getCharLenght(str) {
    var bytesCount = 0;
    for (var i = 0; i < str.length; i++) {
        var c = str.charAt(i);
        if (/^[\u0000-\u00ff]$/.test(c)) //匹配双字节
        {
            bytesCount += 1;
        }
        else {
            bytesCount += 2;
        }
    }
    return bytesCount;
}
//设置字符串最大长度
function strMaxLength(str, len) {
    var bytelen = 0;
    var result = "";
    for (var i = 0; i < str.length; i++) {
        bytelen += getCharLenght(str.charAt(i));
        if (bytelen == len) {
            result += str.charAt(i);
            break;
        }
        else if (bytelen < len) {
            result += str.charAt(i);
            continue;
        }
        else {
            break;
        }
    }
    return result;
}

/*加减乘除计算
*说明：javascript的计算有BUG,改用后台计算的方式。
加:add  减:min  乘:mul  除:div
*/
function calculate(type, arg1, arg2, precision) {
    if (precision == null || precision == undefined)
        precision = "0.00";
    var ret = "";
    $.ajax({
        //url: "../../../../Custom/ajax/CommonHandler.ashx",
        url: "/Portal/CommonHandler/Index",//wangxg 19.7
        data: { CommandName: "calculate", type: type, arg1: arg1, arg2: arg2, precision: precision },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //    return false;
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
    return ret;
}
//留言显示与隐藏
function showliuyan() {
    if ($("#showliuyan").is(":visible")) {
        $("#showliuyan").hide();
        $("#lysq").hide();
    } else {
        $("#showliuyan").show();
        $("#lysq").show();
    }
}

//附件详情
function getDownLoadURL() {
    if ($("#divattachment").find("tr").length == 0) {
        shakeMsg("附件为空！");
    } else {
        window.open("../../view/FI.html");
    }
    event.stopPropagation();
}

function getApplicantUserInfo() {
    var d = readMessageByDF("APPLICATION", $.MvcSheetUI.SheetInfo.InstanceId, "rsjson");
    var data = JSON.parse("{" + d + "}");
    $("#sqr_name").find("span").html(data.cust_name);
    $("#sqr_id").find("span").html(data.cert_no);
    $("#sqr_mobile").find("span").html(data.mobile);
    $("#gjr_name").find("span").html(data.partner_name);
    $("#gjr_id").find("span").html(data.partner_cert_no);
    $("#dbr_name").find("span").html(data.guarantee_name);
    $("#dbr_id").find("span").html(data.guarantee_cert_no);
}

function getApplicantUserInfoByBizObject() {
    var d = {};
    debugger;
    $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICANT_TYPE.V.R.forEach(function (v, n) {
        var id_number = v.DataItems["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V;
        
        if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "I" ||
                v.DataItems["APPLICANT_TYPE.GUARANTOR_TYPE"].V == "I") {
            var name_field = "APPLICANT_DETAIL.FIRST_THI_NME";
            var id_field = "APPLICANT_DETAIL.ID_CARD_NBR";
            $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICANT_DETAIL.V.R.forEach(function (app_dtl, app_num) {
                if (app_dtl.DataItems["APPLICANT_DETAIL.IDENTIFICATION_CODE2"].V == id_number) {
                    if (id_number == "1") {
                        d["sqr_name"] = app_dtl.DataItems[name_field].V;
                        d["sqr_id"] = app_dtl.DataItems[id_field].V;
                    }
                    else {
                        if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "I" && d["gjr_name"] == undefined) {
                            d["gjr_name"] = app_dtl.DataItems[name_field].V;
                            d["gjr_id"] = app_dtl.DataItems[id_field].V;
                        }
                        else if (v.DataItems["APPLICANT_TYPE.GUARANTOR_TYPE"].V == "I" && d["dbr_name"] == undefined) {
                            d["dbr_name"] = app_dtl.DataItems[name_field].V;
                            d["dbr_id"] = app_dtl.DataItems[id_field].V;
                        }
                    }
                }
            });
        }
        else if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "C" ||
            v.DataItems["APPLICANT_TYPE.GUARANTOR_TYPE"].V == "C") {
            var name_field = "COMPANY_DETAIL.COMPANY_THI_NME";
            var sts_field = "COMPANY_DETAIL.COMPANY_STS";
            var org_code_field = "COMPANY_DETAIL.ORGANIZATION_CDE";
            $.MvcSheetUI.SheetInfo.BizObject.DataItems.COMPANY_DETAIL.V.R.forEach(function (app_dtl, app_num) {
                if (app_dtl.DataItems["COMPANY_DETAIL.IDENTIFICATION_CODE3"].V == id_number) {
                    if (id_number == "1") {
                        d["sqr_company_name"] = app_dtl.DataItems[name_field].V;
                        d["sqr_company_sts"] = app_dtl.DataItems[sts_field].V;
                        d["sqr_company_org_code"] = app_dtl.DataItems[org_code_field].V;
                    }
                    else {
                        if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "C" && d["gjr_company_name"] == undefined) {
                            d["gjr_company_name"] = app_dtl.DataItems[name_field].V;
                            d["gjr_company_sts"] = app_dtl.DataItems[sts_field].V;
                            d["gjr_company_org_code"] = app_dtl.DataItems[org_code_field].V;
                        }
                        else if (v.DataItems["APPLICANT_TYPE.GUARANTOR_TYPE"].V == "C" && d["dbr_company_name"] == undefined) {
                            d["dbr_company_name"] = app_dtl.DataItems[name_field].V;
                            d["dbr_company_sts"] = app_dtl.DataItems[sts_field].V;
                            d["dbr_company_org_code"] = app_dtl.DataItems[org_code_field].V;
                        }
                    }
                }
            });
        }
    });
    console.log(d);
    if (d.sqr_name) {
        $("#sqr_name").find("span").html(d.sqr_name);
        $("#sqr_id").find("span").html(d.sqr_id);
    }
    else {
        $("[cus_type='sqr']").hide();
    }
    if (d.gjr_name) {
        $("#gjr_name").find("span").html(d.gjr_name);
        $("#gjr_id").find("span").html(d.gjr_id);
    }
    else {
        $("[cus_type='gjr']").hide();
    }
    if (d.dbr_name) {
        $("#dbr_name").find("span").html(d.dbr_name);
        $("#dbr_id").find("span").html(d.dbr_id);
    }
    else {
        $("[cus_type='dbr']").hide();
    }

    if (d.sqr_company_name) {
        $("#sqr_company_name").find("span").html(d.sqr_company_name);
        $("#sqr_company_sts").find("span").html(d.sqr_company_sts);
        $("#sqr_company_org_code").find("span").html(d.sqr_company_org_code);
    }
    else {
        $("[cus_type='sqr_company']").hide();
    }
    if (d.gjr_company_name) {
        $("#gjr_company_name").find("span").html(d.gjr_company_name);
        $("#gjr_company_sts").find("span").html(d.gjr_company_sts);
        $("#gjr_company_org_code").find("span").html(d.gjr_company_org_code);
    }
    else {
        $("[cus_type='gjr_company']").hide();
    }
    if (d.dbr_company_name) {
        $("#dbr_company_name").find("span").html(d.dbr_company_name);
        $("#dbr_company_sts").find("span").html(d.dbr_company_sts);
        $("#dbr_company_org_code").find("span").html(d.dbr_company_org_code);
    }
    else {
        $("[cus_type='dbr_company']").hide();
    }
}
//弹出框（抖动）
function shakeMsg(msg) {
    if ($.MvcSheetUI.SheetInfo.IsMobile) {
        alert(msg);
    }
    else {
        if (typeof (layer) == "undefined") {
            alert(msg);
        }
        else {
            layer.msg(msg, function () { });
        }
    }
}
//弹出框
function showMsg(msg) {
    if ($.MvcSheetUI.SheetInfo.IsMobile) {
        alert(msg);
    }
    else {
        if (typeof (layer) == "undefined") {
            alert(msg);
        }
        else {
            layer.msg(msg);
        }
    }
}
//获取详细还款计划
function get_PMT_Detail(plan_id) {
    var ret;
    $.ajax({
        url: "/Portal/Proposal/GetPMTDetail?plan_id=" + plan_id,
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = JSON.parse(result);
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}

//获取详细还款计划汇总结果
function get_PMT_Sum(plan_id) {
    var ret;
    $.ajax({
        url: "/Portal/Proposal/GetPMTSum?plan_id=" + plan_id,
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = JSON.parse(result);
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}

//详细还款计划明细打印
function plan_detail_print() {
    var id = $.MvcSheetUI.GetControlValue("PLAN_ID");
    window.open("../PlanDetailPrint.html?pid=" + id, "_blank");
}

//核对H3写入到CAP中返回结果的信息：为空或者1不做处理
function check_WriteToCAPResultCode() {
    if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {
        return "";
    }
    else {
        var v_app_num = $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_NUMBER.V;
        var v = $.MvcSheetUI.SheetInfo.BizObject.DataItems.To_CAP_ResultCode.V;
        if (v == null || v == undefined) { //v为空值，错误
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                return "";
            }
            return "写入CAP的错误编码为空，请联系管理员！";
        }
        else if (v_app_num == null || v_app_num == undefined) { //v为空值，错误
            if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity2") {
                return "";
            }
            return "申请单号为空，请联系管理员！";
        }
        else if (v == "1") {
            return "";
        }
        else {
            var ret;
            $.ajax({
                url: "/Portal/Proposal/Get_WriteToCAP_Msg?error_code=" + v,
                data: "",
                type: "GET",
                async: false,
                dataType: "json",
                success: function (result) {
                    ret = result.Msg;
                },
                error: function (msg) {
                    ret = msg;
                }
            });
            return ret;
        }
    }
}

function getCustomSetting(instanceid, itemname) {
    var ret = "";
    $.ajax({
        url: "/Portal/WorkflowSetting/GetCustomSetting?InstanceID=" + instanceid + "&SettingName=" + itemname,
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}

function setCustomSetting(instanceid,schemacode, itemname,itemvalue) {
    var ret = "";
    var para = {
        InstanceID: instanceid,
        SchemaCode: schemacode,
        SettingName: itemname,
        SettingValue: encodeURI(itemvalue)
    };
    $.ajax({
        url: "/Portal/WorkflowSetting/SetCustomSetting",
        data: para,
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}

function get_ysp_result(name, idCardNo, phone) {
    var ret;
    $.ajax({
        url: "/Portal/Api/YSP_Check?name=" + encodeURI(name) + "&idCardNo=" + idCardNo + "&phone=" + phone,
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}

function checkIsDayend() {
    var ret;
    $.ajax({
        url: "/Portal/Proposal/CheckIsDayEnd?T=" + new Date().getTime(),
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}


function getAllMappingCities() {
    var ret = "";
    $.ajax({
        url: "/Portal/Proposal/GetAllMappingCities?T="+new Date().getTime(),
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (msg) {
            ret = msg;
        }
    });
    return ret;
}

function getTrimVal(obj) {
    obj.value = obj.value.trim();
}