//附件详情
function getDownLoadURL() {
    if ($("#divattachment").find("tr").length == 0) {
        shakeMsg("附件为空！");
    } else {
        window.open("../../view/FI.html");
    }
    event.stopPropagation();
}

//FI数据日志
function getDataTrackLog() {
    
    window.open("../../view/FIDataTrack.html?id=" + $.MvcSheetUI.SheetInfo.InstanceId);
    
    event.stopPropagation();
}

var viewer;
//敞口
function get_exposure(application_no) {
    var ret;
    $.ajax({
        url: "/Portal/Proposal/Get_EXPOSURE?application_number=" + application_no,
        data: "",
        type: "GET",
        dataType: "json",
        async: false,
        success: function (result) {
            ret = result;
        },
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
    return ret;
}

function exposure_render() {
    var app_no = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    var template = $("#example div.template");

    var capckje = 0;
    var gjrcapck = 0;
    var gjrcmsck = 0;
    var fullckje = 0;
    var cmsckje = 0;

    var hasckje = false;//是否有敞口

    var exposure_data = get_exposure(app_no);
    exposure_data.Data.forEach(function (data, n) {
        var type_name = "";
        if (data.Type == "B")
            type_name = "借款人";
        else if (data.Type == "C")
            type_name = "共同借款人";
        else if (data.Type == "G")
            type_name = "担保人";
        var li = $("<li><a href=\"#exposure_" + data.No + "\" data-toggle=\"tab\">" + type_name + ":" + data.Name + "</a></li>");
        if (data.CAP.length == 0 && data.CMS.length == 0) {
            $(li).hide();
        }
        $("#example #exposure_Tab").append($(li));
        var cap_ck_sub = 0;
        var cms_ck_sub = 0;
        var row = template.clone().removeClass("template").addClass("tab-pane fade").attr("id", "exposure_" + data.No);

        //主贷人有CAP敞口
        if (data.Type == "B") {
            //有CAP敞口或者有CMS敞口
            if (data.CAP.length > 1 || data.CMS.length > 0) {
                hasckje = true;
            }
        }

        //cap 敞口
        data.CAP.forEach(function (v_cap, n_cap) {
            var tr = "<tr>";
            if (v_cap.SYS_IDENTIFIED_IND == "T") {//表示不可编辑
                if (v_cap.IS_EXPOSED_IND == "T") {//默认勾选中
                    tr += "<td><input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/></td>";
                    capckje = calculate("add", capckje, v_cap.NET_FINANCED_AMT, "N2");
                    cap_ck_sub = calculate("add", cap_ck_sub, v_cap.NET_FINANCED_AMT, "N2");
                }
                else {//默认没有勾选中
                    tr += "<td><input type=\"checkbox\" disabled=\"disabled\"/></td>";
                }
            } else {//可以编辑
                if (v_cap.IS_EXPOSED_IND == "T") {//默认勾选中
                    tr += "<td><input onchange=\"ck_check(this,'cap','capckje')\" data-number=\"" + v_cap.NET_FINANCED_AMT + "\" type=\"checkbox\" checked=\"checked\" /></td>";
                    capckje = calculate("add", capckje, v_cap.NET_FINANCED_AMT, "N2");
                    cap_ck_sub = calculate("add", cap_ck_sub, v_cap.NET_FINANCED_AMT, "N2");
                }
                else {//默认没有勾选中
                    tr += "<td><input onchange=\"ck_check(this,'cap','capckje')\" data-number=\"" + v_cap.NET_FINANCED_AMT + "\" type=\"checkbox\"/></td>";
                }
            }
            //本单申请，不显示
            if (v_cap.EXP_APPLICATION_NUMBER == app_no) {
                return false;
            }

            tr += "<td>" + v_cap.NET_FINANCED_AMT + "</td>";
            tr += "<td><a onclick=\"getinstence('" + v_cap.EXP_APPLICATION_NUMBER + "')\" >" + v_cap.EXP_APPLICATION_NUMBER + "</a></td>";
            tr += "<td>" + v_cap.APPLICATION_STATUS_DSC + "</td>";
            tr += "<td>" + v_cap.EXP_APPLICANT_ROLE_NAME + "</td>";
            tr += "<td>" + v_cap.EXP_APPLICANT_CARD_ID + "</td>";
            tr += "<td>" + v_cap.EXP_APPLICANT_NAME + "</td>";
            tr += "<td>" + v_cap.FP_GROUP_NME + "</td>";
            tr += "<td>" + v_cap.TYPE + "</td>";
            tr += "<td>" + v_cap.APPLICATION_STATUS_DTE + "</td>";
            tr += "<td>" + v_cap.NO_OF_TERMS + "</td>";
            tr += "<td>" + v_cap.ASSET_MAKE_DSC + "</td>";
            tr += "<td>" + v_cap.ASSET_MODEL_DSC + "</td>";
            tr += "<td>" + v_cap.ASSET_BRAND_DSC + "</td>";
            tr += "</tr>";
            $(row).find("#Application").append(tr);
        });

        //cms 敞口
        data.CMS.forEach(function (cms_d, cms_n) {
            var tr = "<tr>";
            if (cms_d.SYS_IDENTIFIED_IND == "T") {//表示不可编辑
                if (cms_d.IS_EXPOSED_IND == "T") {//默认勾选中
                    tr += "<td><input type=\"checkbox\" checked=\"checked\" disabled=\"disabled\"/></td>";
                    cmsckje = calculate("add", cmsckje, cms_d.PRINCIPLE_OUTSTANDING_AMT, "N2");
                    cms_ck_sub = calculate("add", cms_ck_sub, cms_d.PRINCIPLE_OUTSTANDING_AMT, "N2");
                }
                else {//默认没有勾选中
                    tr += "<td><input type=\"checkbox\" disabled=\"disabled\"/></td>";
                }
            } else {//可以编辑
                if (cms_d.IS_EXPOSED_IND == "T") {//默认勾选中
                    tr += "<td><input onchange=\"ck_check(this,'cms','cmsckje')\" data-number=\"" + cms_d.PRINCIPLE_OUTSTANDING_AMT + "\" type=\"checkbox\" checked=\"checked\" /></td>";
                    cmsckje = calculate("add", cmsckje, cms_d.PRINCIPLE_OUTSTANDING_AMT);
                    cms_ck_sub = calculate("add", cms_ck_sub, cms_d.PRINCIPLE_OUTSTANDING_AMT);
                }
                else {//默认没有勾选中
                    tr += "<td><input onchange=\"ck_check(this,'cms','cmsckje')\" data-number=\"" + cms_d.PRINCIPLE_OUTSTANDING_AMT + "\" type=\"checkbox\"/></td>";
                }
            }
            tr += "<td>" + cms_d.PRINCIPLE_OUTSTANDING_AMT + "</td>";
            tr += "<td><a onclick=\"getinstence('" + cms_d.EXP_APPLICATION_NUMBER + "')\" >" + cms_d.EXP_APPLICATION_NUMBER + "</a></td>";
            tr += "<td>" + cms_d.CONTRACT_NUMBER + "</td>";
            tr += "<td>" + cms_d.REQUEST_STATUS_DSC + "</td>";
            tr += "<td>" + cms_d.EXP_INACTIVE_DSC + "</td>";
            tr += "<td>" + cms_d.EXP_APPLICANT_ROLE_NAME + "</td>";

            tr += "<td>" + cms_d.EXP_APPLICANT_CARD_ID + "</td>";
            tr += "<td>" + cms_d.EXP_APPLICANT_NAME + "</td>";
            tr += "<td>" + cms_d.BUSINESS_PARTNER_NME + "</td>";
            tr += "<td>" + cms_d.FP_GROUP_NME + "</td>";
            tr += "<td>" + cms_d.CONTRACT_DTE + "</td>";
            tr += "<td>" + cms_d.INTEREST_RATE + "</td>";
            tr += "<td>" + cms_d.NET_FINANCED_AMT + "</td>";
            tr += "<td>" + cms_d.ASSET_MAKE_DSC + "</td>";
            tr += "<td>" + cms_d.ASSET_MODEL_DSC + "</td>";
            tr += "<td>" + cms_d.ASSET_BRAND_DSC + "</td>";
            tr += "<td>" + cms_d.OVERDUE_30_DAYS + "</td>";
            tr += "<td>" + cms_d.OVERDUE_60_DAYS + "</td>";
            tr += "<td>" + cms_d.OVERDUE_90_DAYS + "</td>";
            tr += "<td>" + cms_d.OVERDUE_ABOVE_90_DAYS + "</td>";
            tr += "<td>" + cms_d.OVERDUE_ABOVE_120_DAYS + "</td>";
            tr += "<td>" + cms_d.NO_OF_TERMS + "</td>";
            tr += "<td>" + cms_d.NO_OF_TERMS_PAID + "</td>";
            tr += "</tr>";
            $(row).find("#cmsApplication").append(tr);
        });

        $(row).find("#sub_total_cap").html(cap_ck_sub);
        $(row).find("#sub_total_cms").html(cms_ck_sub);

        $("#example #exposure_TabContent").append(row);
    });

    //没有敞口
    if (!hasckje) {
        $("#control3571_Row1").find("span").css({ "color": "#000" });
        $("#control3571_Row1").find("a").hide();
    }

    $('#example #exposure_Tab a:first').tab('show');

    $("#capckje").html(capckje);

    $("#cmsckje").html(cmsckje);
    var total_ck_amt = calculate("add", $("#cmsckje").html(), $("#capckje").html(), "N2");
    $("#fullckje").html(total_ck_amt);
    $("#ctrlCkje_Row1").html(total_ck_amt);
    $("#fz_amt").html(exposure_data.Total_FZ);
}

function getinstence(applicationno) {
    $.ajax({
        url: "/Portal/Proposal/Get_H3_Detail",
        data: { application_number: applicationno },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            if (result.URL == "") {
                shakeMsg("在H3系统中没有查找到相关的单据");
            }
            else {
                $("#aclick").attr("href", result.URL);
                document.getElementById("aclick").click();
            }
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}
function ck_check(checkbox, type, ck_total_field) {
    //span当前角色的风险总额
    var span = $(checkbox).closest("div.tab-pane").find("#sub_total_" + type);
    if (checkbox.checked) {
        $("#" + ck_total_field).html(calculate("add", $("#" + ck_total_field).html(), checkbox.dataset.number, "N2"));
        span.html(calculate("add", span.html(), checkbox.dataset.number, "N2"));
    } else {
        $("#" + ck_total_field).html(calculate("min", $("#" + ck_total_field).html(), checkbox.dataset.number, "N2"));
        span.html(calculate("min", span.html(), checkbox.dataset.number, "N2"));
    }
    $("#fullckje").html(calculate("add", $("#capckje").html(), $("#cmsckje").html(), "N2"));
    $("#ctrlCkje").html(calculate("add", $("#cmsckje").html(), $("#capckje").html(), "N2"));
}
function rsfkClick() {
    var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    var url = "";
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE && $.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE.V == "1") {

        var idCard = $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.ID_CARD_NBR']").text();
        console.log(idCard);
        url = "../../view/fkresult.html?id=" + InstanceId + "&repid=" + $.MvcSheetUI.GetControlValue("FK_RECORDID") + "&idCard=" + idCard;
    }
    else {
        url = "../../view/rsfkresultTEST.html?&rsfkurl=" + rsfkurl + "&InstanceId=" + InstanceId;
    }
    $("#aclick").attr("href", url);
    document.getElementById("aclick").click();
    return false;
}
//获取申请人PBOC人行报告
function pbocClick(name, idtype, id) {
    var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    var operator = $.MvcSheetUI.SheetInfo.UserName;
    var datajson = "{\"reportRequest\":{\"repID\":\"" + InstanceId + "\",\"name\":\"" + name + "\",\"idtype\":\"" + idtype + "\",\"id\":\"" + id + "\",\"time\":\"\",\"operator\":\"" + operator + "\"}}";
    $.ajax({
        //url: "../../../../ajax/RSHandler.ashx",
        url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
        data:
        {
            CommandName: "getRSResult",
            param: datajson,
            address: pbocurl
        },
        type: "post",
        async: false,
        dataType: "json",
        success: function (result) {
            if (result.report && result.report != "") {
                $("#aclick").attr("href", result.report);
                document.getElementById("aclick").click();
            }
            else {
                shakeMsg("融数反馈结果：" + result.msg);
            }
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
    return false;
}

function nciicClick() {
    var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
    var ncurl = "";
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE && $.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE.V == "1") {
        ncurl = "NCIIC_FK.html?instanceid=" + instanceid + "&app_no=" + $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    }
    else {
        ncurl = "NCIIC.html?&nciicurl=" + nciicurl + "&instanceid=" + instanceid;
    }
    $("#aclick").attr("href", ncurl);
    document.getElementById("aclick").click();
    return false;
}
//隐藏附加上传空白行
function hideFIrow() {
    $("#divattachment .row").each(function () {
        if ($(this).find(">.col-md-2>span").css("display") == "none") {
            $(this).hide();
        }
    })
}
//隐藏二级菜单
function hidemsg() {
    //审核
    var IsWork = $.MvcSheetUI.QueryString("Mode");
    if (IsWork == "work") {
        $(".bannerTitle")[0].click();
    }
    //初始化状态
    showliuyan();
    $("div[data-isretract='true']").each(function () {
        $(this).find('a').click();
    })
    //隐藏融数
    if ($("#Control380").val() == "overtime") {
        $("#rsmanchk").show();
        getRS();
        getNciic();
    } else {
        getRS();
        getNciic();
    }
}

//人工查询
function rsmanchk() {
    var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
    $.ajax({
        //url: "../../../../ajax/RSHandler.ashx?Math=" + Math.random(),
        url: "/Portal/RSHandler/Index?Math=" + Math.random(),// 19.6.28 wangxg
        data:
        {
            CommandName: "postRongshuNew",
            instanceid: instanceid,
            SchemaCode: $.MvcSheetUI.SheetInfo.SchemaCode,
            manual: "2"
        },
        type: "post",
        async: true,
        dataType: "json",
        success: function (result) {
            $("#rsmanchk").attr("disabled", "disabled");
            $("#searching").html("查询中...");
            timegets();
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}
function timegets() {
    var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
    $.ajax({
        //url: "../../../../ajax/RSHandler.ashx",
        url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
        data:
        {
            CommandName: "getRSResult",
            param: "{\"reqID\": \"" + instanceid + "\" }",
            address: rsfkurl
        },
        type: "post",
        async: true,
        dataType: "json",
        success: function (result) {
            debugger;
            if (result.code == "00") {
                $("#searching").html("");
                printtext(result);
                getNciic();
            } else {
                setTimeout("timegets()", 10000);
            }
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}
var arrrsrq = [];
arrrsrq['localreject'] = "东正本地规则<span style='color:red;'>拒绝</span>";
arrrsrq['cloudaccept'] = "云端规则<span style='color:red;'>通过</span>";
arrrsrq['cloudreject'] = "云端规则<span style='color:red;'>拒绝</span>";
arrrsrq['cloudmanual'] = "云端规则返回<span style='color:red;'>转人工</span>";
arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
function getRS() {
    //var result = { "reqID": "5168553c-145d-49cf-af3c-583355ea5f1b", "data": { "action": "cloudreject", "limit": "", "scorecard": "446.71386199999995" }, "code": "00", "msg": "同盾数据：建议拒绝,", "sqr_reject_code": "RF010", "gjr_reject_code": "", "dbr_reject_code": "", "ds": { "sqr": { "6594111": { "score": 5, "conditions": [{ "hits": [{ "evidenceTime": 1511845280000, "fraudTypeDisplayName": "信用异常", "riskLevelDisplayName": "中", "value": "441322197910265235" }, { "evidenceTime": 1515480401000, "fraudTypeDisplayName": "异常借款", "riskLevelDisplayName": "中", "value": "441322197910265235" }], "type": "grey_list" }], "ruleId": "6594111" }, "6594291": { "score": 5, "conditions": [{ "hits": [{ "evidenceTime": 1511845280000, "fraudTypeDisplayName": "信用异常", "riskLevelDisplayName": "中", "value": "13428055390" }, { "evidenceTime": 1515480401000, "fraudTypeDisplayName": "异常借款", "riskLevelDisplayName": "中", "value": "13428055390" }], "type": "grey_list" }], "ruleId": "6594291" }, "6594771": { "score": 12, "conditions": [{ "dimType": "id_number", "list": ["13428055390", "134※※※※5266", "134※※※※0359", "0"], "result": 4, "subDimType": "account_mobile", "type": "frequency_distinct" }, { "dimType": "id_number", "list": ["115※※※※796@qq.com"], "result": 1, "subDimType": "account_email", "type": "frequency_distinct" }, { "dimType": "id_number", "list": ["广东省 ※※※※※※※※※※※※※※※※※路78号", "石湾镇※※※※※※※※※55号"], "result": 2, "subDimType": "home_address", "type": "frequency_distinct" }], "ruleId": "6594771" }, "6595011": { "score": 10, "conditions": [{ "dimType": "id_number", "list": ["广东省 ※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※※费站附近", "东平路※※※※※10号"], "result": 2, "subDimType": "organization_address", "type": "frequency_distinct" }], "ruleId": "6595011" }, "6595071": { "score": 10, "conditions": [{ "hits": [{ "count": 1, "industryDisplayName": "小额贷款公司" }, { "count": 1, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 1, "dimType": "account_mobile", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 2, "resultsForDim": [{ "count": 2, "dimType": "account_mobile" }, { "count": 2, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595071" }, "6595081": { "score": 9, "conditions": [{ "hits": [{ "count": 1, "industryDisplayName": "一般消费分期平台" }, { "count": 1, "industryDisplayName": "融资租赁" }, { "count": 1, "industryDisplayName": "信用卡中心" }, { "count": 1, "industryDisplayName": "小额贷款公司" }, { "count": 1, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 1, "dimType": "account_mobile", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "信用卡中心" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "信用卡中心" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "小额贷款公司" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 5, "resultsForDim": [{ "count": 5, "dimType": "account_mobile" }, { "count": 5, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595081" }, "6595091": { "score": 30, "conditions": [{ "hits": [{ "count": 3, "industryDisplayName": "一般消费分期平台" }, { "count": 1, "industryDisplayName": "互联网金融门户" }, { "count": 1, "industryDisplayName": "第三方服务商" }, { "count": 1, "industryDisplayName": "大型消费金融公司" }, { "count": 1, "industryDisplayName": "融资租赁" }, { "count": 1, "industryDisplayName": "信用卡中心" }, { "count": 2, "industryDisplayName": "小额贷款公司" }, { "count": 10, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 3, "dimType": "account_mobile", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "互联网金融门户" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "第三方服务商" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "大型消费金融公司" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "account_mobile", "industryDisplayName": "信用卡中心" }, { "count": 2, "dimType": "account_mobile", "industryDisplayName": "小额贷款公司" }, { "count": 10, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 3, "dimType": "id_number", "industryDisplayName": "一般消费分期平台" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "互联网金融门户" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "第三方服务商" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "大型消费金融公司" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "融资租赁" }, { "count": 1, "dimType": "id_number", "industryDisplayName": "信用卡中心" }, { "count": 2, "dimType": "id_number", "industryDisplayName": "小额贷款公司" }, { "count": 10, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 20, "resultsForDim": [{ "count": 20, "dimType": "account_mobile" }, { "count": 20, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595091" }, "6595101": { "score": 4, "conditions": [{ "hits": [{ "count": 2, "industryDisplayName": "P2P网贷" }], "hitsForDim": [{ "count": 2, "dimType": "account_mobile", "industryDisplayName": "P2P网贷" }, { "count": 2, "dimType": "id_number", "industryDisplayName": "P2P网贷" }], "result": 2, "resultsForDim": [{ "count": 2, "dimType": "account_mobile" }, { "count": 2, "dimType": "id_number" }], "type": "association_partner" }], "ruleId": "6595101" }, "ds6__policy_set_name": "借款事件_网站_20170720", "ds6__risk_type": "suspiciousLoan_reject", "spend_time": 47, "ds6__policy_name": "借款事件_网站_20170720", "ds6__final_score": 85, "ds6__final_decision": "Reject", "ds6__policy_set": [{ "ds6__policy_score": 85, "ds6__policy_mode": "Weighted", "ds6__risk_type": "suspiciousLoan", "ds6__policy_decision": "Reject", "ds6__policy_name": "STARK借款_网站", "ds6__policy_uuid": "1a2c7963355e4b13895576231fb9cdfc", "ds6__hit_rules": [{ "score": 5, "decision": "Accept", "name": "身份证命中中风险关注名单", "id": "6594111", "uuid": "0aa7ff6fbb684c3f80891dbb4f89b248", "parentUuid": "873cc0e156b04f6ead012b46768f3688" }, { "score": 5, "decision": "Accept", "name": "手机号命中中风险关注名单", "id": "6594291", "uuid": "6fb19669dfd54b58978007e6600964ee", "parentUuid": "63dccc42db734b1ebe1773fc635d95af" }, { "score": 12, "decision": "Accept", "name": "3个月内身份证关联多个申请信息", "id": "6594771", "uuid": "792d6b5060904ac683285fd2df39e2b2", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "3个月内申请人身份证关联工作单位地址个数大于等于2", "id": "6595011", "uuid": "aa62cadc312f4a85957d6c160c7466c0", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "7天内申请人在多个平台申请借款", "id": "6595071", "uuid": "1f157ed863c84c2481e92129ee0adc27", "parentUuid": "" }, { "score": 9, "decision": "Accept", "name": "1个月内申请人在多个平台申请借款", "id": "6595081", "uuid": "56777b55175a4b4caac02b932c35ce72", "parentUuid": "" }, { "score": 30, "decision": "Accept", "name": "3个月内申请人在多个平台申请借款", "id": "6595091", "uuid": "e1552335f5e84d10babb4929a287b3c8", "parentUuid": "" }, { "score": 4, "decision": "Accept", "name": "3个月内申请人在多个平台被放款_不包含本合作方", "id": "6595101", "uuid": "8ee79e36dd8248b59592c0aa46af2324", "parentUuid": "" }] }, { "ds6__policy_score": 0, "ds6__policy_mode": "Weighted", "ds6__risk_type": "applySuspicious", "ds6__policy_decision": "Accept", "ds6__policy_name": "欺诈行为_网站", "ds6__policy_uuid": "3d27dfa14e04440e94d171975bae2a00" }], "success": true, "seq_id": "1517549293866986F47C82BCA7833569", "ds6__hit_rules": [{ "score": 5, "decision": "Accept", "name": "身份证命中中风险关注名单", "id": "6594111", "uuid": "0aa7ff6fbb684c3f80891dbb4f89b248", "parentUuid": "873cc0e156b04f6ead012b46768f3688" }, { "score": 5, "decision": "Accept", "name": "手机号命中中风险关注名单", "id": "6594291", "uuid": "6fb19669dfd54b58978007e6600964ee", "parentUuid": "63dccc42db734b1ebe1773fc635d95af" }, { "score": 12, "decision": "Accept", "name": "3个月内身份证关联多个申请信息", "id": "6594771", "uuid": "792d6b5060904ac683285fd2df39e2b2", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "3个月内申请人身份证关联工作单位地址个数大于等于2", "id": "6595011", "uuid": "aa62cadc312f4a85957d6c160c7466c0", "parentUuid": "" }, { "score": 10, "decision": "Accept", "name": "7天内申请人在多个平台申请借款", "id": "6595071", "uuid": "1f157ed863c84c2481e92129ee0adc27", "parentUuid": "" }, { "score": 9, "decision": "Accept", "name": "1个月内申请人在多个平台申请借款", "id": "6595081", "uuid": "56777b55175a4b4caac02b932c35ce72", "parentUuid": "" }, { "score": 30, "decision": "Accept", "name": "3个月内申请人在多个平台申请借款", "id": "6595091", "uuid": "e1552335f5e84d10babb4929a287b3c8", "parentUuid": "" }, { "score": 4, "decision": "Accept", "name": "3个月内申请人在多个平台被放款_不包含本合作方", "id": "6595101", "uuid": "8ee79e36dd8248b59592c0aa46af2324", "parentUuid": "" }], "ds1__product__operation": "null", "ds1__product__result": "null", "ds2__als_d7_id_bank_allnum": "null", "ds2__als_d7_id_nbank_p2p_allnum": "null", "ds2__als_d7_id_nbank_mc_allnum": "null", "ds2__als_d7_id_nbank_cf_allnum": "null", "ds2__als_d7_id_nbank_ca_allnum": "null", "ds2__als_d7_id_nbank_com_allnum": "null", "ds2__als_d7_cell_bank_allnum": "null", "ds2__als_d7_cell_nbank_p2p_allnum": "null", "ds2__als_d7_cell_nbank_mc_allnum": "null", "ds2__als_d7_cell_nbank_cf_allnum": "null", "ds2__als_d7_cell_nbank_ca_allnum": "null", "ds2__als_d7_cell_nbank_com_allnum": "null", "ds2__als_m6_id_avg_monnum": "null", "ds2__als_m6_id_tot_mons": "null", "ds2__als_m6_id_max_monnum": "null", "ds3__busiData__last3MonMuliAppCnt": "null", "ds3__busiData__last1MonMuliAppCnt": "null", "ds3__busiData__records__credooScore__0": "null", "ds4__user_gray__phone_gray_score": "null", "ds4__near1mselectcount": "null", "ds4__near3mselectcount": "null", "ds5__zm_score": "null", "ds5__score": "null", "ds6_final_score": "null", "ds7__fxtj__zhixing": "null", "ds7__fxtj__caipan": "null", "ds7__fxtj__shenpan": "null", "ds7__fxtj__weifa": "null", "ds7__fxtj__qianshui": "null", "ds7__fxtj__feizheng": "null", "ds7__fxtj__qiankuan": "null", "ds8__CDCA002__0": "null", "ds8__CDCA003__0": "null", "ds8__CDCA004__0": "null", "ds8__CDCA005__0": "null", "ds8__CDTB001__0": "null", "ds8__CDTB004__0": "null", "ds8__CDTB016__0": "null", "ds8__CDTB019__0": "null", "ds8__CDTB031__0": "null", "ds8__CDTB033__0": "null", "ds8__CDTB045__0": "null", "ds8__CDTB047__0": "null", "ds8__CDTP033__0": "null", "ds8__CDTP046__0": "null", "ds8__CDTP047__0": "null", "ds8__CDTP060__0": "null", "ds8__CDTC001__0": "null", "ds8__CDTC003__0": "null", "ds8__CDTC004__0": "null", "ds8__CDTC006__0": "null", "ds8__CDCT001__0": "null", "ds8__CDCT003__0": "null", "ds8__CSRL002__0": "null", "ds8__CSRL004__0": "null", "ds8__CSWC001__0": "null" }, "gjr": null, "dbr": null } };
    // wangxg 19.8 自动审批就不调用RS
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE && $.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE.V == "1") {
        return;
    }
    var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
    $.ajax({
        //url: "../../../../ajax/RSHandler.ashx",
        url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
        data:
        {
            CommandName: "getRSResult",
            param: "{\"reqID\": \"" + instanceid + "\" }",
            address: rsfkurl
        },
        type: "post",
        dataType: "json",
        success: function (result) {
            debugger;
            printtext(result);
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}
function getNciic() {
    // wangxg 19.8 自动审批就不调用RS
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE && $.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE.V == "1") {
        return;
    }
    var instanceid = $.MvcSheetUI.SheetInfo.InstanceId;
    //var rquet = { "reqID": "3ad7e98d-f580-4a95-99ad-749338da60db", "code": "00", "msg": "成功", "sqr": { "result_gmsfhm": "一致", "result_xm": "一致", "result_cym": "", "result_xb": "一致", "result_mz": "一致", "result_csrq": "一致", "result_whcd": "不一致（高于库中级别）", "result_hyzk": "不一致", "result_fwcs": "此项无记录", "result_csdssx": "市不一致", "result_jgssx": "省一致", "result_ssssxq": "省一致/市一致", "result_zz": "不一致", "xp": "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" }, "gjr": { "result_gmsfhm": "一致", "result_xm": "一致", "result_cym": "", "result_xb": "一致", "result_mz": "一致", "result_csrq": "一致", "result_whcd": "不一致（高于库中级别）", "result_hyzk": "一致", "result_fwcs": "不一致", "result_csdssx": "市一致", "result_jgssx": "省一致", "result_ssssxq": "县（区）不一致", "result_zz": "不一致", "xp": "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" } };
    
    $.ajax({
        //url: "../../../../ajax/RSHandler.ashx",
        url: "/Portal/RSHandler/Index",// 19.6.28 wangxg
        data:
        {
            CommandName: "getRSResult",
            param: "{\"reqID\": \"" + instanceid + "\" }",
            address: nciicurl
        },
        type: "post",
        async: true,
        dataType: "json",
        success: function (result) {
            //var result = rquet;
            if (result.code == "00") {
                //申请人
                if (result.sqr) {
                    var span_xm_sqr = "";//姓名
                    var span_sfzh_sqr = "";//身份证号
                    var span_hyzk_sqr = "";//婚姻状况
                    //姓名
                    if (result.sqr.result_xm == "不一致") {
                        span_xm_sqr = "<span class=\"red\">不一致</span>";
                    }
                    else {
                        span_xm_sqr = "<span class=\"black\">" + result.sqr.result_xm + "</span>";
                    }
                    //身份证号
                    if (result.sqr.result_gmsfhm == "不一致") {
                        span_sfzh_sqr = "<span class=\"red\">不一致</span>";
                    }
                    else {
                        span_sfzh_sqr = "<span class=\"black\">" + result.sqr.result_gmsfhm + "</span>";
                    }
                    //婚姻状况
                    if (result.sqr.result_hyzk == "不一致") {
                        span_hyzk_sqr = "<span class=\"red\">不一致</span>";
                    }
                    else {
                        span_hyzk_sqr = "<span class=\"black\">" + result.sqr.result_hyzk + "</span>";
                    }
                    //赋值
                    $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.FIRST_THI_NME']").after(span_xm_sqr);
                    $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.ID_CARD_NBR']").after(span_sfzh_sqr);
                    $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.MARITAL_STATUS_CDE']").after(span_hyzk_sqr);
                }
                //共借人
                if (result.gjr) {
                    var span_xm_gjr = "";//姓名
                    var span_sfzh_gjr = "";//身份证号
                    var span_hyzk_gjr = "";//婚姻状况
                    if (result.gjr.result_xm == "不一致") {
                        span_xm_gjr = "<span class=\"red\">不一致</span>";
                    } else {
                        span_xm_gjr = "<span class=\"black\">" + result.gjr.result_xm + "</span>";
                    }

                    if (result.gjr.result_gmsfhm == "不一致") {
                        span_sfzh_gjr = "<span class=\"red\">不一致</span>";
                    } else {
                        span_sfzh_gjr = "<span class=\"black\">" + result.gjr.result_gmsfhm + "</span>";
                    }

                    if (result.gjr.result_hyzk == "不一致") {
                        span_hyzk_gjr = "<span class=\"red\">不一致</span>";
                    } else {
                        span_hyzk_gjr = "<span class=\"black\">" + result.gjr.result_hyzk + "</span>";
                    }
                    //赋值
                    $("div[t='gjr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.FIRST_THI_NME']").after(span_xm_gjr);
                    $("div[t='gjr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.ID_CARD_NBR']").after(span_sfzh_gjr);
                    $("div[t='gjr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.MARITAL_STATUS_CDE']").after(span_hyzk_gjr);
                }
                //担保人
                if (result.dbr) {
                    var span_xm_dbr = "";//姓名
                    var span_sfzh_dbr = "";//身份证号
                    var span_hyzk_dbr = "";//婚姻状况
                    if (result.dbr.result_xm == "不一致") {
                        span_xm_dbr = "<span class=\"red\">不一致</span>";
                    }
                    else {
                        span_xm_dbr = "<span class=\"black\">" + result.dbr.result_xm + "</span>";
                    }

                    if (result.dbr.result_gmsfhm == "不一致") {
                        span_sfzh_dbr = "<span class=\"red\">不一致</span>";
                    }
                    else {
                        span_xm_dbr = "<span class=\"black\">" + result.dbr.result_gmsfhm + "</span>";
                    }

                    if (result.dbr.result_hyzk == "不一致") {
                        span_hyzk_dbr = "<span class=\"red\">不一致</span>";
                    }
                    else {
                        span_hyzk_dbr = "<span class=\"black\">" + result.dbr.result_hyzk + "</span>";
                    }
                    //赋值
                    $("div[t='dbr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.FIRST_THI_NME']").after(span_xm_gjr);
                    $("div[t='dbr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.ID_CARD_NBR']").after(span_sfzh_gjr);
                    $("div[t='dbr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.MARITAL_STATUS_CDE']").after(span_hyzk_gjr);
                }
            }
        },
        //error: function (msg) {
        //    shakeMsg("出错了");
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}
function printtext(results) {
    var result = results;
    if (result.code == "00") {
        $("#searching").html("");
        if (result.data) {
            $("#fkpgjg").html(arrrsrq[result.data.action]);
            $("#grxypf").html(Math.round(result.data.scorecard));
            var span;
            if (Math.round(result.data.scorecard) < 400) {
                span = "<span style=\"color:red;padding-left:15px;\">建议拒绝</span>";
            } else if (Math.round(result.data.scorecard) >= 470) {
                span = "<span style=\"color:red;padding-left:15px;\">建议通过</span>";
            } else {
                span = "<span style=\"color:red;padding-left:15px;\">建议人工审批</span>";
            }
            $("#grxypf").after(span);
        }

        var span_sqr = "";
        var span_gjr = "";
        var span_dbr = "";
        //申请人电话
        if (result.ds.sqr) {
            if (result.ds.sqr.ds1__product__result == "不一致") {
                span_sqr = "<span class=\"red\">不一致</span>";
            }
            else {
                span_sqr = "<span class=\"black\">" + result.ds.sqr.ds1__product__result + "</span>";
            }
        }
        //共借人电话
        if (result.ds.gjr) {
            if (result.ds.gjr.ds1__product__result == "不一致") {
                span_gjr = "<span class=\"red\">不一致</span>";
            }
            else {
                span_gjr = "<span class=\"black\">" + result.ds.gjr.ds1__product__result + "</span>";
            }
        }
        //担保人电话
        if (result.ds.dbr) {
            if (result.ds.dbr.ds1__product__result == "不一致") {
                span_dbr = "<span class=\"red\">不一致</span>";
            }
            else {
                span_dbr = "<span class=\"black\">" + result.ds.dbr.ds1__product__result + "</span>";
            }
        }
        //赋值
        $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_PHONE_FAX.PHONE_NUMBER']").after(span_sqr);
        $("div[t='gjr']").eq(0).find("span[data-dtf='APPLICANT_PHONE_FAX.PHONE_NUMBER']").after(span_gjr);
        $("div[t='dbr']").eq(0).find("span[data-dtf='APPLICANT_PHONE_FAX.PHONE_NUMBER']").after(span_dbr);
    }
    //申请人
    //黑名单
    if (result.sqr_reject_code.indexOf("RB021") != -1 || result.sqr_reject_code.indexOf("RB022") != -1 || result.sqr_reject_code.indexOf("RB024") != -1 || result.sqr_reject_code.indexOf("RB025") != -1 || result.sqr_reject_code.indexOf("RB009") != -1 || result.sqr_reject_code.indexOf("RB010") != -1) {
        $("div[t='sqr']").eq(0).find("#hmdjl").append("<span style='color:red;'>是</span>");
    } else {
        $("div[t='sqr']").eq(0).find("#hmdjl").append("<span>否</span>");
    }
    //外部信用
    if (result.sqr_reject_code.indexOf("RB004") != -1) {
        $("div[t='sqr']").eq(0).find("#wbxyjl").append("<span style='color:red;'>是</span>");
    } else {
        $("div[t='sqr']").eq(0).find("#wbxyjl").append("<span>否</span>");
    }
    //共借人
    //黑名单
    if (result.gjr_reject_code.indexOf("RB021") != -1 || result.gjr_reject_code.indexOf("RB022") != -1 || result.gjr_reject_code.indexOf("RB024") != -1 || result.gjr_reject_code.indexOf("RB025") != -1 || result.gjr_reject_code.indexOf("RB009") != -1 || result.gjr_reject_code.indexOf("RB010") != -1) {
        $("div[t='gjr']").eq(0).find("#hmdjl").append("<span style='color:red;'>是</span>");
    } else {
        $("div[t='gjr']").eq(0).find("#hmdjl").append("<span>否</span>");
    }
    //外部信用
    if (result.gjr_reject_code.indexOf("RB004") != -1) {
        $("div[t='gjr']").eq(0).find("#wbxyjl").append("<span style='color:red;'>是</span>");
    } else {
        $("div[t='gjr']").eq(0).find("#wbxyjl").append("<span>否</span>");
    }
    //担保人
    //黑名单
    if (result.dbr_reject_code.indexOf("RB021") != -1 || result.dbr_reject_code.indexOf("RB022") != -1 || result.dbr_reject_code.indexOf("RB024") != -1 || result.dbr_reject_code.indexOf("RB025") != -1 || result.dbr_reject_code.indexOf("RB009") != -1 || result.dbr_reject_code.indexOf("RB010") != -1) {
        $("div[t='dbr']").eq(0).find("#hmdjl").append("<span style='color:red;'>是</span>");
    } else {
        $("div[t='dbr']").eq(0).find("#hmdjl").append("<span>否</span>");
    }
    //外部信用
    if (result.dbr_reject_code.indexOf("RB004") != -1) {
        $("div[t='dbr']").eq(0).find("#wbxyjl").append("<span style='color:red;'>是</span>");
    } else {
        $("div[t='dbr']").eq(0).find("#wbxyjl").append("<span>否</span>");
    }
}
//审批意见
function hidexscsyj() {
    $("#xscsyj").find(".widget-comments").removeClass('bordered');
    $("#Control362").find(".widget-comments").removeClass('bordered');
    var Activity = '<%=this.ActionContext.ActivityCode%>';
    var IsWork = $.MvcSheetUI.QueryString("Mode");
    var len = $("#xscsyj").find(".widget-comments").find(".comment").length;
    if (len > 1 && Activity == "Activity13") {
        var num = len - 1;
        $("#xscsyj").find(".widget-comments").find(".comment:eq(" + num + ")").prevAll().hide();
        $("#xscsyj").find(".widget-comments").find(".comment:last-child").after("<a onclick=\"isshowcsyj(this)\" data-isshow=\'false\' style=\'float:right;margin-right:30px;margin-bottom:5px;\'>展开更多(" + num + ") &or;</a>");
    }
    if (Activity == "Activity14" && IsWork == "work" && $("#Control362").find(".widget-comments").length == 0) {
        $("#xszsyj").hide();
        
    }

    var userName = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V;

    var isInner = userName.indexOf("98") == 0 || userName.indexOf("80000") == 0;

    if (($.MvcSheetUI.SheetInfo.ActivityCode == "Activity14" && IsWork == "work") || $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_TYPE_CODE.V != "00001" || isInner) {
        $("#row-facesign").hide();
    }

    //if (Activity == "Activity13" && IsWork == "work") {
    //    $("#Control362").find("select").parent().hide();
    //    $("#Control362").next("label").hide();
    //}
}
//是否显示初审意见
function isshowcsyj(ts) {
    if (ts.dataset.isshow == 'false') {
        $("#xscsyj").find(".widget-comments").find(".comment").show();
        ts.dataset.isshow = 'true';
        $(ts).html("收起 &and;");
    } else {
        var num = $("#xscsyj").find(".widget-comments").find(".comment").length * 1 - 1;
        $("#xscsyj").find(".widget-comments").find(".comment:eq(" + num + ")").prevAll().hide();
        ts.dataset.isshow = 'false';
        $(ts).html("展开更多(" + num + ") &or;");
    }
}

//附加费详细按钮
function showAccessoryLink() {
    var accessory_amt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ACCESSORY_AMT", 1);
    if (accessory_amt == "" || parseFloat(accessory_amt) == 0) {
        //附加费为空，或者为0，隐藏详细链接；
        //$("a[href=\"#accessoryDetail\"]:visible").hide();//不做控制
    }
}

//审核信息顶部固定
function setfixtop() {
    $("#fktop").css({ "position": "fixed", "top": $("#main-navbar").outerHeight(), "left": $("#main-navbar").offset().left, "width": $("#content-wrapper").outerWidth() });
    $(".panel-body").css({ "padding-top": $("#fktop").outerHeight() });
}
$(window).resize(function () {
    $("#fktop").css({ "width": $("#content-wrapper").outerWidth(), "top": $("#main-navbar").outerHeight(), "left": $("#main-navbar").offset().left });
});
//标红
function isred() {
    if ($("#divOriginateOUName").find('label').html() == "外网经销商") {
        $("#divOriginateOUName").find('label').css({ "color": "red" });
    }
    //第三方担保????
    //if ($("#Control315").val() == "否") {
    //    $("#Control315").parent().find("label").css({ "color": "red" });
    //}
    //合同价格
    if ($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.ASSET_COST", 1) * 1 >= $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", 1) * 1) {
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.ASSET_COST", 1).find("label").find("span").css({ "color": "red" });
    }
    //销售价格
    if ($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.SALE_PRICE", 1) * 1 >= $("#control309").find("label").find("span").html() * 1) {
        $.MvcSheetUI.GetElement("VEHICLE_DETAIL.SALE_PRICE", 1).find("label").find("span").css({ "color": "red" });
    }
}
//初审状态下拉框
function bindradio() {
    $("#ctl271484").find("input[type='radio']").click(function () {
        if ($(this).val() == "拒绝" || $(this).val() == "有条件核准" || $(this).val() == "取消") {
            $("#csjjxz").show();
        } else {
            $("#csjjxz").hide();
        }
    })
}

function cstjChange(con) {
    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
        var ope = $.MvcSheetUI.GetControlValue("csshzt");
        if (ope == "有条件核准") {
            var csyj = $.MvcSheetUI.GetControlValue("CSYJ");
            var tj = $.MvcSheetUI.GetControlValue("xsjjlyzx");
            if (csyj == null || csyj == "") {
                csyj = tj;
            } else {
                csyj = csyj + "，" + tj;
            }
            $("#xscsyj").find('textarea').val(csyj);
        }
    }
}

// 页面加载完成事件
$.MvcSheet.Loaded = function (sheetInfo) {
    show_borrow_tab();
    isHighRisk();
    exposure_render();
    getmsg();
    hideFIrow();
    isred();
    hidemsg();
    setfixtop();
    hidexscsyj();
    //bindradio();
    showAccessoryLink();

    callInit();

    hideFiDataTrackLog();

    //显示和隐藏
    $("#addPhoneExtension").click(function () {
        $("#CALLEDPARTYNAME").val("");
        $("#CALLEDPARTYTYPE").val("");
        $("#CALLEDPARTYNUMBER").val("");
        $("#tabAddPhone").show();
    })


    var ext_sys_code = $.MvcSheetUI.SheetInfo.BizObject.DataItems.EXTERNAL_SYSTEM_CODE.V;
    var lb_navbar = "个人贷款申请";
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_TYPE_CODE.V == "00002") {
        lb_navbar = "机构贷款申请";
    }
    else {
        var isgpd = false;//是否是公牌贷
        $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICANT_TYPE.V.R.forEach(function (v, n) {
            if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "C") {
                isgpd = true;
                return false;
            }
        });
        if (isgpd) {
            lb_navbar = "个人汽车贷款申请<span style=\"color:red;\">(公牌)</span>";
        }
    }
    if (ext_sys_code) {
        if (ext_sys_code == "autohome") {
            lb_navbar = lb_navbar + "<span style=\"color:red;\">(汽车之家)</span>";
        }
        else {
            lb_navbar = lb_navbar + "<span style=\"color:red;\">(" + ext_sys_code + ")</span>";
        }
    }
    $("#main-navbar").find("h3").html(lb_navbar);

    //添加留言
    $('#addmsga').on('click', function () { addmsg(); });
    document.oncontextmenu = function () {
        return false;
    }

    //车辆销售价格等于新车指导价格，车辆销售价格标红
    if (parseFloat($.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.NEW_PRICE", 1)) == parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1))) {
        $.MvcSheetUI.GetElement("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1).next().css("color", "red");
    }

    //显示隐藏dom
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE && $.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE.V == "1") {
        $("#xypf").addClass("hidden");//信用评分
        $("#rgcx").addClass("hidden");//人工查询
        $("#aclickh").addClass("hidden");//数融pboc
        $("#dzdatamodel").addClass("hidden");//隐藏东正大数据模型
        $("#lb-fkpgjg").addClass("hidden");//模型评估结果label
        $("#lb-yusuan").addClass("hidden");//客户还款测算label
        $("#lb-srfzc").addClass("hidden");//客户还款测算值label
        $("#fk_aclickh").removeClass("hidden");//东正风控pboc
        $("#lb-fkshowresult").removeClass("hidden");//东正风控展示审核结果
        //加载pboc
        queryFkPBOC();

    }
    else {
        $("#lb-grade").addClass("hidden");//客户评级
    }    
    showModelData();

    loadChe300QueryResult();
}

//确认提交后事件
$.MvcSheet.AfterConfirmSubmit = function () {
    //var v_approval = getCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, "ProposalApproval");getDownLoadURL
    //if (v_approval.toUpperCase().indexOf("SUCCESS") >= 0) {
    //    return true;
    //}
    var flag_needxdh = $.MvcSheetUI.SheetInfo.BizObject.DataItems.flag_NeedXDH.V;
    if (flag_needxdh) {//需要信货会审批，在信货会节点调用接口；
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity15") {
            var comments = $.MvcSheetUI.SheetInfo.BizObject.DataItems.ZSYJj.V.Comments;

            var v = $.MvcSheetUI.MvcRuntime.executeService('CAPServiceNew', 'ProposalApproval',
                {
                    InstanceID: $.MvcSheetUI.SheetInfo.InstanceId,
                    Application_Number: $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER"),
                    StatusCode: $.MvcSheetUI.GetControlValue("zsshzt"),//终审审批意见，使用之前的方式
                    Approval_UserCode: $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V,//发起人登录名
                    Approval_Comment: comments[comments.length - 1].Text//终审最后一条审批意见
                }
            );
            if (!v || v.toUpperCase().indexOf("SUCCESS") == "-1") {
                shakeMsg("CAP错误:" + v);
                return false;
            }
            else {
                setCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, $.MvcSheetUI.SheetInfo.SchemaCode, "ProposalApproval", v);
            }
        }
    }
    else {//不需要信货会审批，在终审节点调用接口；
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity13") {
            var v = $.MvcSheetUI.MvcRuntime.executeService('CAPServiceNew', 'ProposalApproval',
                {
                    InstanceID: $.MvcSheetUI.SheetInfo.InstanceId,
                    Application_Number: $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER"),
                    StatusCode: $.MvcSheetUI.GetControlValue("zsshzt"),//终审状态，使用之前的方式
                    Approval_UserCode: $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V,//发起人登录名
                    Approval_Comment: $.MvcSheetUI.GetControlValue("ZSYJj") //终审最后一条审批意见
                }
            );
            if (!v || v.toUpperCase().indexOf("SUCCESS") == "-1") {
                shakeMsg("CAP错误:" + v);
                return false;
            }
            else {
                setCustomSetting($.MvcSheetUI.SheetInfo.InstanceId, $.MvcSheetUI.SheetInfo.SchemaCode, "ProposalApproval", v);
            }
        }
    }
    return true;
}

// 表单验证接口
$.MvcSheet.Validate = function () {
    //信审初审
    if (this.IsReject && ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity14" || $.MvcSheetUI.SheetInfo.ActivityCode == "Activity13")) {
        var ope = $.MvcSheetUI.GetControlValue("csshzt");
        var shyj = "初审意见";
        if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity13") {
            shyj = "终审意见";
            ope = $.MvcSheetUI.GetControlValue("zsshzt");
        }
        if (ope != "驳回") {
            shakeMsg(shyj+"请选择驳回");
            return false;
        }
    }

    if (this.Action == "Submit" &&  $.MvcSheetUI.SheetInfo.ActivityCode == "Activity13") {
        var ope = $.MvcSheetUI.GetControlValue("zsshzt");
        if (ope == "驳回") {
            shakeMsg("请选择驳回操作");
            return false;
        }
    }

    if (this.Action == "Submit" && $.MvcSheetUI.SheetInfo.ActivityCode == "Activity14") {
        var ope = $.MvcSheetUI.GetControlValue("csshzt");
        if (ope == "驳回") {
            shakeMsg("请选择驳回操作");
            return false;
        }
        if (!$.MvcSheetUI.GetElement("xsjjlyzx").is(":hidden")) {
            if (ope != "核准") {
                var category1 = $.MvcSheetUI.GetControlValue("xsjjlyzx");
                var category2 = $.MvcSheetUI.GetControlValue("xsjjlyzx1");
                var category2OptionNum = $.MvcSheetUI.GetElement("xsjjlyzx1").find("option").length;
                if (!category1 || category1 == "") {
                    shakeMsg("请选择" + ope + "的原因");
                    return false;
                }
                else if (category2OptionNum > 1 && (!category2 || category2 == "")) {
                    shakeMsg("请选择" + ope + "的原因");
                    return false;
                }
            }
        }
    }
    return true;
}

//获取人员信息:申请人,共借人,担保人
function get_application_data() {
    var data = $.MvcSheetUI.SheetInfo.BizObject.DataItems;
    var result = [];
    var ddl_Data = JSON.parse(readMessageFromAttachment("DDL_DATASOURCE"));//下拉框数据源
    data.APPLICANT_TYPE.V.R.forEach(function (v, n) {
        var v_main_app = v.DataItems["APPLICANT_TYPE.MAIN_APPLICANT"].V;
        var v_app_type = v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V;
        var v_app_guart = v.DataItems["APPLICANT_TYPE.GUARANTOR_TYPE"].V;
        var v_identify_code = v.DataItems["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V;
        var d = {};
        var type = "";
        if (!v_app_type || v_app_type == "")
            type = v_app_guart;
        else {
            type = v_app_type;
        }
        addobj(d, v.DataItems);
        //d = v.DataItems;
        if (type == "I") {
            //个人明细表
            data.APPLICANT_DETAIL.V.R.forEach(function (v_app_dtl, n_app_dtl) {
                if (v_app_dtl.DataItems["APPLICANT_DETAIL.IDENTIFICATION_CODE2"].V == v_identify_code) {
                    addobj(d, v_app_dtl.DataItems);
                    return false;
                }
            });
            //工作信息表(只取一条)
            data.EMPLOYER.V.R.forEach(function (v_employee_dtl, n_employee_dtl) {
                if (v_employee_dtl.DataItems["EMPLOYER.IDENTIFICATION_CODE6"].V == v_identify_code) {
                    addobj(d, v_employee_dtl.DataItems);
                    return false;
                }
            });
            //联系人明细表(只取一条)
            data.PERSONNAL_REFERENCE.V.R.forEach(function (v_CONTACT, n_CONTACT) {
                if (v_CONTACT.DataItems["PERSONNAL_REFERENCE.IDENTIFICATION_CODE10"].V == v_identify_code) {
                    addobj(d, v_CONTACT.DataItems);
                    return false;
                }
            });
        }
        else if (type == "C") {
            //机构明细表
            data.COMPANY_DETAIL.V.R.forEach(function (v_company_dtl, n_company_dtl) {
                if (v_company_dtl.DataItems["COMPANY_DETAIL.IDENTIFICATION_CODE3"].V == v_identify_code) {
                    addobj(d, v_company_dtl.DataItems);
                    return false;
                }
            });
        }

        //地址明细表(只取一条)
        data.ADDRESS.V.R.forEach(function (v_ADDRESS, n_ADDRESS) {
            if (v_ADDRESS.DataItems["ADDRESS.IDENTIFICATION_CODE4"].V == v_identify_code) {
                addobj(d, v_ADDRESS.DataItems);
                //电话号码,最取一条
                data.APPLICANT_PHONE_FAX.V.R.forEach(function (v_APPLICANT_PHONE_FAX, n_APPLICANT_PHONE_FAX) {
                    if (v_APPLICANT_PHONE_FAX.DataItems["APPLICANT_PHONE_FAX.ADDRESS_CODE5"].V == v_ADDRESS.DataItems["ADDRESS.ADDRESS_CODE"].V
                        && v_APPLICANT_PHONE_FAX.DataItems["APPLICANT_PHONE_FAX.IDENTIFICATION_CODE5"].V == v_identify_code) {
                        addobj(d, v_APPLICANT_PHONE_FAX.DataItems);
                        return false;
                    }
                });
                return false;
            }
        });

        result.push(d);
    });
    console.log(result);
    result.forEach(function (v, n) {
        //个人
        ddl_Data.APPLICANT_DETAIL.forEach(function (v_app, n_app) {
            if (v_app.IDENTIFICATION_CODE == v["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V) {
                for (var k in v_app) {
                    if (k != "IDENTIFICATION_CODE") {
                        if (v[k].V == v_app[k].Code) {
                            v[k].V = v_app[k].Text;
                        }
                    }
                }
                return false;
            }
        });
        //机构
        ddl_Data.COMPANY_DETAIL.forEach(function (v_app, n_app) {
            if (v_app.IDENTIFICATION_CODE == v["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V) {
                for (var k in v_app) {
                    if (k != "IDENTIFICATION_CODE") {
                        if (v[k].V == v_app[k].Code) {
                            v[k].V = v_app[k].Text;
                        }
                    }
                }
                return false;
            }
        });
        //地址
        ddl_Data.ADDRESS.forEach(function (v_app, n_app) {
            if (v_app.IDENTIFICATION_CODE == v["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V &&
                v_app.ADDRESS_CODE == v["ADDRESS.ADDRESS_CODE"].V) {
                for (var k in v_app) {
                    if (k != "IDENTIFICATION_CODE" && k != "ADDRESS_CODE") {
                        if (v[k].V == v_app[k].Code) {
                            v[k].V = v_app[k].Text;
                        }
                    }
                }
                return false;
            }
        });
        //电话
        ddl_Data.APPLICANT_PHONE_FAX.forEach(function (v_app, n_app) {
            if (v_app.IDENTIFICATION_CODE == v["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V &&
                v_app.ADDRESS_CODE == v["ADDRESS.ADDRESS_CODE"].V &&
                v_app.PHONE_SEQ_ID == v["APPLICANT_PHONE_FAX.PHONE_SEQ_ID"].V) {
                for (var k in v_app) {
                    if (k != "IDENTIFICATION_CODE" && k != "ADDRESS_CODE" && k != "PHONE_SEQ_ID") {
                        if (v[k].V == v_app[k].Code) {
                            v[k].V = v_app[k].Text;
                        }
                    }
                }
                return false;
            }
        });
        //工作
        ddl_Data.EMPLOYER.forEach(function (v_app, n_app) {
            if (v_app.IDENTIFICATION_CODE == v["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V &&
                v_app.EMPLOYEE_LINE_ID == v["EMPLOYER.EMPLOYEE_LINE_ID"].V) {
                for (var k in v_app) {
                    if (k != "IDENTIFICATION_CODE" && k != "EMPLOYEE_LINE_ID") {
                        if (v[k].V == v_app[k].Code) {
                            v[k].V = v_app[k].Text;
                        }
                    }
                }
                return false;
            }
        });
        //联系人
        ddl_Data.PERSONNAL_REFERENCE.forEach(function (v_app, n_app) {
            if (v_app.IDENTIFICATION_CODE == v["APPLICANT_TYPE.IDENTIFICATION_CODE1"].V &&
                v_app.LINE_ID == v["PERSONNAL_REFERENCE.LINE_ID10"].V) {
                for (var k in v_app) {
                    if (k != "IDENTIFICATION_CODE" && k != "LINE_ID") {
                        if (v[k].V == v_app[k].Code) {
                            v[k].V = v_app[k].Text;
                        }
                    }
                }
                return false;
            }
        });
    });
    console.log(result);
    return result;
}

var data;
//添加对象的值
function addobj(obj1, obj2) {
    for (var k in obj2) {
        obj1[k] = obj2[k];
    }
}
//显示申请人,共借人,担保人
function show_borrow_tab() {
    //增加PCM客户测算
    var loanAmt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1);   //放款金额
    var periods = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);           //期数
    var dzcarloan = "";                             //东正车贷月应还款额

    //判断是否有共借人
    var hasGjr = false;
    $($.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICANT_TYPE.V.R).each(function (n, v) {
        if (v.DataItems["APPLICANT_TYPE.MAIN_APPLICANT"].V == null) {//非主借人
            if (v.DataItems["APPLICANT_TYPE.APPLICANT_TYPE"].V == "I") {//共借人
                hasGjr = true;
                return false;
            }
        }
    });
    //申请类型。个人，机构
    var app_type = $.MvcSheetUI.SheetInfo.BizObject.DataItems.APPLICATION_TYPE_CODE.V;

    //计算 东正车贷月应还款额
    if (hasGjr) { dzcarloan = parseFloat(loanAmt) / parseInt(periods); }
    else { dzcarloan = 0.7 * parseFloat(loanAmt) / parseInt(periods); }
    var acls = getAcls($.MvcSheetUI.SheetInfo.UserCode);
    //无人工查询权限
    if ($.inArray("003", acls) == -1) {
        $("#rsmanchk").parent().empty();
    }
    data = get_application_data();
    data.forEach(function (v, n) {
        var id = v["APPLICANT_TYPE.ObjectID"].V.trim();
        var type = v["APPLICANT_TYPE.APPLICANT_TYPE"].V;
        var g_type = v["APPLICANT_TYPE.GUARANTOR_TYPE"].V;
        var main_type = v["APPLICANT_TYPE.MAIN_APPLICANT"].V;
        var name = "";
        var type_name = "";
        var t = "";
        if (main_type == "Y") {
            type_name = "借款人";
            t = "sqr";
        }
        else {
            if (type && type != "") {
                type_name = "共借人";
                t = "gjr";
            }
            else if (g_type && g_type != "") {
                type_name = "担保人";
                t = "dbr";
            }
        }

        //PCM 客户测算
        if (app_type == "00001" && v["APPLICANT_TYPE.APPLICANT_TYPE"].V == "I") {
            getCustomerInfo(type, v["APPLICANT_TYPE.NAME1"].V, type_name, $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER"), v["APPLICANT_DETAIL.ID_CARD_NBR"].V, dzcarloan);
        }

        name = type_name + ":" + v["APPLICANT_TYPE.NAME1"].V;
        var is_view = v["APPLICANT_TYPE.IS_INACTIVE_IND"].V == "T" ? "隐藏" : "显示";
        if (is_view == "显示") {
            //2019-01-28号：只根据人员关系表中的显示及隐藏来控制，和Spouse无关，Spouse表示是否发送给上海银行；
            //if (v["APPLICANT_TYPE.APPLICANT_TYPE"].V == "I" && v["APPLICANT_DETAIL.SPOUSE_IND"] && v["APPLICANT_DETAIL.SPOUSE_IND"].V) {
            //    is_view = "隐藏";
            //}
        }
        //1.生成Tab标签
        $("#tab_borrower").append("<li><a href=\"#" + id + "\" data-toggle=\"tab\"><span>" + name + "</span><span style=\"margin-left:10px\">（" + is_view + "）</span></a></li>");
        //2.生成Tab标签对应的内容
        //个人
        var template;
        if (type == "I" || g_type == "I") {
            template = $("#div_borrower").find(".template.I");
            if ($.inArray("001", acls) > -1) {
                $("#aclickh").append("<a href=\"javascript:void(0);\" class=\"role\" onclick=\"pbocClick('" + v["APPLICANT_DETAIL.FIRST_THI_NME"].V + "','" + v["APPLICANT_DETAIL.ID_CARD_TYP"].V + "','" + v["APPLICANT_DETAIL.ID_CARD_NBR"].V + "');\">" + type_name + "</a>");
            }
        }
        //机构
        else if (type == "C" || g_type == "C") {
            template = $("#div_borrower").find(".template.C");
        }
        var row = $(template).clone(true).removeClass("template").addClass("rows").addClass("tab-pane").addClass("fade").attr("id", id).attr("t", t);
        if (main_type == "Y") {  //申请人:个人
            if (type == "I") {
                //不显示关系
                $(row).find("span[data-dtf='APPLICANT_DETAIL.GUARANTOR_RELATIONSHIP_CDE']").parent().parent().hide();
            }
        }
        else if (type == "I") {//共借人-个人
            //显示是否是配偶
            $(row).find("span[data-dtf='APPLICANT_DETAIL.SPOUSE_IND']").parent().parent().removeClass("hidden");
        }
        else if (g_type == "I") {//担保人-个人

        }

        if (main_type == "Y") {
            $(row).find("#bankcard_report_link").removeClass("hidden").find("a").unbind("click").bind("click", function () {
                queryBankCardReport(v["APPLICANT_DETAIL.FIRST_THI_NME"].V, v["APPLICANT_DETAIL.ID_CARD_NBR"].V);
            });
        }

        //先注释
        if ((g_type == "I" || type == "I") && v["APPLICANT_DETAIL.ID_CARD_TYP"].V == "居民身份证") {
            $(row).find("#td_report_link").removeClass("hidden").find("a").unbind("click").bind("click", function () {
                queryTdReport(v["APPLICANT_DETAIL.ID_CARD_NBR"].V);
            });
        }

        $(row).find("[data-dtf]").each(function (n_span, v_span) {
            var dtf = $(v_span).attr("data-dtf");
            if (dtf != "" && v[dtf]) {
                var v_val;
                //format属性
                if ($(v_span).attr("data-fmt") && $(v_span).attr("data-fmt") != "") {
                    //目前只支持日期Format
                    v_val = dateFmt(v[dtf].V, $(v_span).attr("data-fmt"));
                }
                //options属性
                else if ($(v_span).attr("data-options") && $(v_span).attr("data-options") != "") {
                    v_val = fmtOptions(v[dtf].V, $(v_span).attr("data-options"));
                }
                else {
                    v_val = v[dtf].V;
                    if (dtf == "EMPLOYER.NAME_2") {
                        var app_no = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
                        $.ajax({
                            url: "/Portal/Proposal/GetCompanyOrderList?t=" + new Date().getTime(),
                            data: { "companyName": v_val.trim(), "application_number": app_no },
                            type: "POST",
                            dataType: "json",
                            async: true,
                            success: function (result) {
                                if (result.Success) {
                                    if (result.Data.length > 0) {
                                        $(v_span).parent().append("<a href = \"#dealersDetail\" data-toggle=\"modal\" onclick = \"getCompanyOrderList(this)\" target = \"_blank\" >单位敞口</a>");
                                        $(v_span).attr("style", "color:red");
                                    }
                                }
                            },
                            error: function (msg) {// 19.7 
                                showJqErr(msg);
                            }
                        });
                    }
                    if (dtf == "APPLICANT_PHONE_FAX.PHONE_NUMBER") {
                        var calledName = "";
                        var phoneNumber = "";
                        var calledtype = "";
                        var phoneNumberType = "";
                        var idtype = "";
                        var idnumber = "";

                        if (type == "I" || g_type == "I") {
                            calledName = v["APPLICANT_DETAIL.FIRST_THI_NME"].V;
                            phoneNumber = v[dtf].V;
                            calledtype = main_type == "Y" ? "主借人" : type_name;
                            phoneNumberType = v["APPLICANT_PHONE_FAX.PHONE_TYPE_CDE"].V;
                            idtype = v["APPLICANT_DETAIL.ID_CARD_TYP"].V;
                            idnumber = v["APPLICANT_DETAIL.ID_CARD_NBR"].V;
                        }
                        else if (type == "C" || g_type == "C") {
                            calledName = v["COMPANY_DETAIL.COMPANY_THI_NME"].V;
                            phoneNumber = v[dtf].V;
                            calledtype = "单位";
                            phoneNumberType = v["APPLICANT_PHONE_FAX.PHONE_TYPE_CDE"].V;
                            idtype = "组织机构代码";
                            idnumber = v["COMPANY_DETAIL.ORGANIZATION_CDE"].V;
                        }

                        $(v_span).parent().append("<a  data-toggle=\"modal\" onclick=\"TelephoneBox('" + calledName + "','" + phoneNumber + "','" + calledtype + "','" + phoneNumberType + "','" + idtype + "','" + idnumber + "')\" data-target=\"#myModal1\" id=\"btn_AddBorrower\"><img src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a>");
                    }
                    if (dtf == "EMPLOYER.PHONE") {
                        var phoneNumber = v[dtf].V;
                        if (phoneNumber != "" && phoneNumber != null && phoneNumber != undefined) {
                            var calledName = v["EMPLOYER.NAME_2"].V;
                            var calledtype = "单位";
                            var phoneNumberType = checkPhone(phoneNumber) ? "手机 Mobile Phone" : "座机";
                            var idtype = "";
                            var idnumber = "";
                            $(v_span).parent().append("<a  data-toggle=\"modal\" onclick=\"TelephoneBox('" + calledName + "','" + phoneNumber + "','" + calledtype + "','" + phoneNumberType + "','" + idtype + "','" + idnumber + "')\" data-target=\"#myModal1\" id=\"btn_AddBorrower\"><img src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a>");
                        }
                    }

                    if (dtf == "PERSONNAL_REFERENCE.PHONE" || dtf == "PERSONNAL_REFERENCE.MOBILE") {
                        var phoneNumber = v[dtf].V;
                        if (phoneNumber != "" && phoneNumber != null && phoneNumber != undefined) {
                            var calledName = v["PERSONNAL_REFERENCE.NAME10"].V;
                            var calledtype = "其他";
                            var phoneNumberType = checkPhone(phoneNumber) ? "" : "座机";
                            var idtype = "";
                            var idnumber = "";
                            $(v_span).parent().append("<a  data-toggle=\"modal\" onclick=\"TelephoneBox('" + calledName + "','" + phoneNumber + "','" + calledtype + "','" + phoneNumberType + "','" + idtype + "','" + idnumber + "')\" data-target=\"#myModal1\" id=\"btn_AddBorrower\"><img src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a>");
                        }
                    }
                }

                //style样式
                if ($(v_span).attr("data-style") && $(v_span).attr("data-style") != "") {
                    var style_arr = $(v_span).attr("data-style").split("?");
                    var v_v1 = style_arr[1];
                    var v_v2 = style_arr[2];
                    //this  value直接用当前值替换
                    var v_v = style_arr[0].replace("this", "'" + v_val + "'").replace("value", "'" + v_val + "'");
                    //if (style_arr[0].indexOf("{") > 0) {
                    //    var df = style_arr[0].substr(style_arr[0].indexOf("{"), style_arr[0].indexOf("}"));
                    //    if (dt == dtf) {
                    //        v_v = v_v.replace("{" + df + "}", v_val);
                    //    }
                    //    else {

                    //    }
                    //}
                    var v_style;
                    if (eval(v_v)) {
                        v_style = v_v1.replace(/'/g, "");
                    }
                    else {
                        v_style = v_v2.replace(/'/g, "");
                    }
                    if ($(v_span).attr("style") && $(v_span).attr("style") != "") {
                        $(v_span).attr("style", v_style + $(v_span).attr("style"));
                    }
                    else {
                        $(v_span).attr("style", v_style);
                    }
                }
                if (v_val == null || v_val == undefined)
                    v_val = "";
                $(v_span).html(v_val.toString());
            }
        });
        $(template).after(row);
    });
    //3.显示第一个标签；
    $('#tab_borrower a:first').tab('show')
}

/*
时间格式如下:yyyy-MM-dd hh:mm:ss
*/
function dateFmt(d, fmt) {
    if (!d || d == "" || !fmt || fmt == "")
        return d;
    var date = new Date(d);
    var o = {
        "M+": date.getMonth() + 1,                 //月份   
        "d+": date.getDate(),                    //日   
        "h+": date.getHours(),                   //小时   
        "m+": date.getMinutes(),                 //分   
        "s+": date.getSeconds(),                 //秒   
        "q+": Math.floor((date.getMonth() + 3) / 3), //季度   
        "S": date.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (date.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}
//options
function fmtOptions(v, opts) {
    if (!opts || opts == "") {
        return v;
    }
    if (v == null)
        v = "";
    var ret = v;//默认等于Format之前的值
    opts.split(";").forEach(function (val, n) {
        if (val.split(":")[0] == v + "") {
            ret = val.split(":")[1];
            return false;
        }
    });
    return ret;
}

//PCM
function getCustomerInfo(type, name, roleName, appno, certNo, pmt) {
    if (type != "I")
        return false;
    $.ajax({
        url: "/Portal/PCMHandler/Index",//wangxg 19.7
        //url: "/Portal/ajax/PCMHandler.ashx",
        data: { CommandName: "GetConnect", certno: certNo, dzcarloan: pmt, appno: appno },
        type: "POST",
        async: true,
        dataType: "json",
        success: function (result) {
            //result = JSON.parse(result.replace(/\n/g, ""));
            if (result.Result == "Success") {
                var newRow = $("#divCustoms .template").clone().removeClass("template");
                var title = newRow.find("#title_pcm");
                $(title.find("span")[0]).html(name);
                $(title.find("span")[1]).html(roleName);
                newRow.find("#table_pcm .c_row label").each(function (n, v) {
                    $(v).html(result[$(v).data("data")]);
                });
                $("#divCustoms").append(newRow);
                if (roleName == "借款人") {
                    var loanAmt = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.AMOUNT_FINANCED", 1);   //放款金额
                    var periods = $.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.LEASE_TERM_IN_MONTH", 1);           //期数
                    var hked = parseFloat(loanAmt) / parseInt(periods);
                    var cz = (parseFloat(result.C_RepayLoan)).toFixed(2);
                    $("#srfzc").html(cz);
                    if (cz < 0) {
                        $("#srfzc").css("color", "red");
                    }
                }
            }
        },
        //error: function (msg) {
        //    shakeMsg("客户测算结果出错！");
        //},
        error: function (msg) {// 19.7 
            var err = '客户测算结果出错';
            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {
                err = msg.responseText;
            }
            shakeMsg(err + ',异常代码=' + msg.status);
            return false;
        }
    });
}

//查询同盾报表
function queryTdReport(idcard_no) {
    console.log(idcard_no);
    if (idcard_no && idcard_no != "") {
        localStorage.setItem("id", idcard_no);
        window.open("TDReport.html", "_blank");
    }
    else {
        shakeMsg("身份证号码为空");
    }
}

//查询银行卡信息报表
function queryBankCardReport(name, idCard) {

    var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    var url = "../../view/bankcardinforesult.html?id=" + InstanceId + "&name=" + name + "&idCard=" + idCard;
    //$("#bankcardreportclick").attr("href", url);
    //document.getElementById("bankcardreportclick").click();
    window.open(url, "_blank");
}



function isHighRisk() {
    var fi_code = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V;
    $.ajax({
        url: "/Portal/Proposal/IsHighRisk?FI_Code=" + fi_code + "&t=" + new Date().getTime(),
        data: "",
        type: "GET",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result.HighRisk != "") {
                $("#lblFullName").append("<span style='color:red'>(" + result.HighRisk + ")</span>");
            }
        },
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}

//单位敞口
function getCompanyOrderList(target) {
    var companyName = $(target).parent().children("span").html();
    var app_no = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    $("#dealersInfo").empty();
    $.ajax({
        url: "/Portal/Proposal/GetCompanyOrderList?t=" + new Date().getTime(),
        data: { "companyName": companyName.trim(), "application_number": app_no },
        type: "POST",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result.Success) {
                var html;
                $.each(result.Data, function (index, value) {
                    html += "<tr class=\"header\">\
                        <td class=\"rowSerialNo hidden\">"+ value.OBJECTID + "</td>";
                    html += "<td>";
                    if (value.WORKFLOWCODE == "APPLICATION") {
                        html += "<a target=\"_blank\" href=\"/Portal/Sheets/DefaultEngine/Proposal/V1/XSSH.aspx?Mode=View&InstanceId=" + value.OBJECTID + "&SheetCode=APPLICATION_003&T=" + new Date().getTime() + "\">" + value.APPLICATION_NUMBER + "</a>";
                    }
                    else if (value.WORKFLOWCODE == "RetailApp") {
                        html += "<a target=\"_blank\" href=\"/Portal/Sheets/DefaultEngine/RetailApp/SRetailAppTEST.aspx?Mode=View&InstanceId=" + value.OBJECTID + "&SheetCode=XSDKSQBD&T=" + new Date().getTime() + "\">" + value.APPLICATION_NUMBER + "</a>";
                    }
                    else if (value.WORKFLOWCODE == "CompanyApp") {
                        html += "<a target=\"_blank\" href=\"/Portal/Sheets/DefaultEngine/CompanyApp/SCompanyAppTEST.aspx?Mode=View&InstanceId=" + value.OBJECTID + "&SheetCode=XSCompanyApp&T=" + new Date().getTime() + "\">" + value.APPLICATION_NUMBER + "</a>";
                    }
                    else if (value.WORKFLOWCODE == "HRetailApp") {
                        html += "<a target=\"_blank\" href=\"/Portal/Sheets/DefaultEngine/RetailApp/HSRetailApp.aspx?Mode=View&InstanceId=" + value.OBJECTID + "&SheetCode=grqcdksq&T=" + new Date().getTime() + "\">" + value.APPLICATION_NUMBER + "</a>";
                    }
                    else if (value.WORKFLOWCODE == "HCompanyApp") {
                        html += "<a target=\"_blank\" href=\"/Portal/Sheets/DefaultEngine/CompanyApp/HSCompanyApp.aspx?Mode=View&InstanceId=" + value.OBJECTID + "&SheetCode=HSCompanyApp1&T=" + new Date().getTime() + "\">" + value.APPLICATION_NUMBER + "</a>";
                    }
                    html += "</td>";
                    html += "<td>" + value.BUSINESS_PARTNER_NME + "</td>\
                        <td>"+ value.APPLICATION_DATE + "</td>\
                        <td>"+ value.Trial_STATUS + "</td>\
                        <td>"+ value.FINALLY_STATUS + "</td>\
                        <td>"+ value.STATUS + "</td>\
                        </tr > ";
                });
                $("#dealersInfo").html(html);
            }
            else {
                shakeMsg("获取订单结果出错！错误原因:" + result.Message);
            }
        },
        //error: function (msg) {
        //    shakeMsg("获取订单结果出错！错误原因：" + msg);
        //},
        error: function (msg) {// 19.7 
            var err = '获取订单结果出错！';
            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {
                err = msg.responseText;
            }
            alert(err + ',异常代码=' + msg.status);
            return false;
        }
    });
}

//查询风控PBOC
function queryFkPBOC() {
    var acls = getAcls($.MvcSheetUI.SheetInfo.UserCode);
    if ($.inArray("001", acls) == -1) {
        $("#fk_aclickh").addClass("hidden");
        return;
    }

    var instanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    var applicationNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    var recordId = $.MvcSheetUI.GetControlValue("FK_RECORDID");

    $.ajax({
        url: "/Portal/Api/QueryPBOCReport?InstanceId=" + instanceId + "&ApplicationNumber=" + applicationNumber + "&recordId=" + recordId,
        data: "",
        type: "GET",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result == null || result == "") {
                shakeMsg("PBOC查询失败！");
                return;
            }

            if (result.code != "00") {
                shakeMsg("PBOC查询错误！");
                return;
            }

            var pbocReports = result.data.pbocReportInfoList;

            if (pbocReports == null || pbocReports == null || pbocReports == undefined) {
                shakeMsg("无PBOC报告");
                return;
            }

            var pbocReportHtml = "";

            pbocReportHtml += "";
            pbocReportHtml += "<select style='margin-left:10px' id='pbocReport_select' onchange='changePbocReport()'>";
            pbocReportHtml += "<option value=''>--请选择--</option>";
            pbocReports.forEach(item => {
                pbocReportHtml += "<option value=\"" + item.pbocReportUrl + "\">" + item.name + "(" + item.PersonnelCategoryName + ")" + "</option>";
            });

            pbocReportHtml += "</select >";
            pbocReportHtml += "";

            $("#fk_pboc_label").after(pbocReportHtml);
        },
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}


//选择pboc人员
function changePbocReport() {

    var pbocReportValue = $("#pbocReport_select").val();

    if (pbocReportValue == null || pbocReportValue == "" || pbocReportValue == undefined) {

        return;
    }
    else {
        $("#pbocReport_select").val("");
    }
    window.localStorage.setItem("pbocurl", pbocReportValue);
    //打卡新窗口
    //window.open(pbocReportValue, "_blank");
    window.open("../../view/pbocview.html", "_blank", "height=1000,width=1000,top=70,left=70,resizable=no,location=no,menubar=no,titlebar=no,titlebar=no");

}

function fkResult(id, idCard) {
    var rtnData;
    $.ajax({
        url: "/Portal/Api/QueryRiskEvaluationResult?InstanceId=" + id + "&idCard=" + idCard + "&T=" + new Date().getTime(),
        data: "",
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            if (result == "" || result == null) {
                alert("未查询到数据");
                return false;
            }
            var r = JSON.parse(result);
            if (r.code == "00") {
                rtnData = r.data;
            }
            else {
                alert(r.message);
                return false;
            }
        }
    });
    return rtnData;
}

function showModelData() {
    if ($.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE && $.MvcSheetUI.SheetInfo.BizObject.DataItems.FK_SYS_TYPE.V == "1") {
        var id = $.MvcSheetUI.SheetInfo.InstanceId;
        var idCard = $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.ID_CARD_NBR']").text();
        var fkresult = fkResult(id, idCard);
        if (fkresult) {
            //客户评级
            if (fkresult.modelResult.grade) {
                $("#sp_grade").text(fkresult.modelResult.grade);
            }
        }
    }
}

/*********** 电话相关 *************/

//通过贷款申请号查询信息   添加电话
function InquiryApplicationnumber() {
    var applicationnumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    $.ajax({
        url: "/Portal/CallCenter/SelectApplicationnumber?Applicationnumber=" + applicationnumber,
        type: "GET",
        async: true,
        success: function (result) {
            if (result.code > 0) {
                //result.data.each(function () {
                //});
                //< img src = "../ V1 / img / 3.png" width = "20" height = "20" /> 
                $("#tabPhones").html("")
                $.each(result.data, function (index, obj) {
                    //var html = "";
                    //html += "<tr>";
                    //html += "<td>" + (index + 1) + "</td>";
                    //html += "<td>" + obj.Calledpartyname+ "</td>"
                    $("#tabPhones").append("<tr><td>" + (index + 1) + "</td><td>" + obj.Calledpartyname + "</td><td>" + obj.Calledpartytype + "</td><td>" + obj.Calledpartynumber + "<a onclick=\"TelephoneBox('" + obj.Calledpartyname + "','" + obj.Calledpartynumber + "','" + obj.Calledpartytype + "','" + obj.Calledpartynumbertype + "','','')\" data-toggle=\"modal\" data-target=\"#myModal1\" id=\"btn_AddBorrower\"><img src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a></td><td>" + obj.Calledpartynumbertype + "</td><td onclick=\"UpdateStatus('" + obj.ObjectId + "')\"><a>删除</a></td></tr>");
                })
            }
        },
        error: function (xhr) {
            alert("出错");
        },

    });
}

function TelephoneBox(calledname, phoneNumber, calledtype, phoneNumberType, idtype, idnumber) {

    $("#dialCalledName").val(calledname);
    $("#dialCalledType").val(calledtype);
    $("#dialCalledNumber").val(phoneNumber);
    $("#dialMobileType").val(phoneNumberType);
    $("#dialIdType").val(idtype);
    $("#dialIdNumber").val(idnumber);
    //座机 直接拨打号码
    if (phoneNumberType == "座机") {
        $('#myModal1').modal('hide');//隐藏
        $('#myModal2').modal('show');//显示
        dialOut();
    }

}







//删除功能：通过ObjectId Status修改状态为0:无效  添加电话
function UpdateStatus(ObjectId) {

    layer.confirm('确认删除吗？', {
        btn: ['取消', '确认'] //按钮
    }, function (index, layero) {
        layer.close(index);

    }, function () {
        $.ajax({
            url: "/Portal/CallCenter/CallCenterUpdate?ObjectId=" + ObjectId,
            type: "GET",
            async: true,
            success: function (result) {
                if (result.code > 0) {
                    alert("删除成功");
                    InquiryApplicationnumber();

                } else {
                    alert("删除失败");
                }
            },
            error: function (xhr) {
                alert("出错");
            },
        });
    });
}


//通过贷款申请号查询信息 录音列表
function Recording() {
    var applicationnumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    $.ajax({
        url: "/Portal/CallCenter/XSSHQueryRecords?Applicationnumber=" + applicationnumber,
        type: "GET",
        async: true,
        success: function (result) {
            if (result.code > 0) {
                $("#RecordTab").html("");
                $.each(result.data, function (index, obj) {
                    $("#RecordTab").append("<tr><td>" + (index + 1) + "</td><td>" + obj.Calledpartyname + "</td><td>" + obj.Calledpartytype + "</td><td>" + obj.Calledpartynumber + "<a onclick=\"TelephoneBox('" + obj.Calledpartyname + "','" + obj.Calledpartynumber + "','" + obj.Calledpartytype + "','" + obj.Calledpartynumbertype + "','','')\" data-toggle=\"modal\" data-target=\"#myModal1\" id=\"btn_AddBorrower\"><img src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a></td><td>" + obj.Calledpartynumbertype + "</td><td>" + obj.Createdtime + "</td><td><a target=\"_blank\" href=\"http://172.16.2.251/interface/api/?action=record_play&recording=/var/spool/asterisk/monitor/" + obj.Recordfilename + "\"><img src=\"../V1/img/2.png\" width=\"18\" height=\"18\" /></a></td></tr>");
                })
            }
        },
        error: function (xhr) {
            alert("出错");
        },

    });
}




//初始化相关操作
function callInit() {

    InquiryApplicationnumber();//通过贷款申请号查询信息 电话信息
    Recording();//通过贷款申请号查询信息 录音列表信息

    $("#myModal1").hide();

    //取消 清空
    $("#EmptyPhone").click(function () {
        $("#CALLEDPARTYNAME").val("");
        $("#CALLEDPARTYTYPE").val("");
        $("#CALLEDPARTYNUMBER").val("");
    })

    $("#tabAddPhone").hide();



    //添加电话 新增功能
    $("#savePhone").click(function () {
        var CALLEDPARTYNAME = $("#CALLEDPARTYNAME").val();
        var CALLEDPARTYTYPE = $("#CALLEDPARTYTYPE").val();
        var CALLEDPARTYNUMBER = $("#CALLEDPARTYNUMBER").val();
        var CALLEDPARTYNUMBERTYPE = $("#CALLEDPARTYNUMBERTYPE").val();
        var myreg = /^[0-9]*$/;
        //if (!myreg.test(CALLEDPARTYNUMBER)) {
        //    shakeMsg("被叫人电话输入错误");
        //    return;
        //}

        //手机 验证
        var phone = /^[1][3,4,5,7,8][0-9]{9}$/;
        //座机验证
        ///0\d{2,3}-\d{7,8}/  /^((\d{3,4})-?)?\d{7,8}$/;
        var Landline = /^\d{3,4}-\d{7,8}$/;
        if (!phone.test(CALLEDPARTYNUMBER) && CALLEDPARTYNUMBERTYPE == "手机 Mobile Phone") {
            shakeMsg("手机号格式不对，请重新输入！");
            return;
        } else if (!Landline.test(CALLEDPARTYNUMBER) && CALLEDPARTYNUMBERTYPE == "座机") {
            shakeMsg("座机格式不对，请重新输入！");
            return;
        }


        //当前页面获取  主借人姓名  MIANAPPLICANTNAME
        var MIANAPPLICANTNAME = $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.FIRST_THI_NME']").text();
        //当前页面获取  贷款号  APPLICATIONNUMBER
        var APPLICATIONNUMBER = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
        var model = "{'APPLICATIONNUMBER':'" + APPLICATIONNUMBER + "', 'MIANAPPLICANTNAME': '" + MIANAPPLICANTNAME + "','CALLEDPARTYNAME': '" + CALLEDPARTYNAME + "', 'CALLEDPARTYTYPE':'" + CALLEDPARTYTYPE + "','CALLEDPARTYNUMBER': '" + CALLEDPARTYNUMBER + "','CALLEDPARTYNUMBERTYPE': '" + CALLEDPARTYNUMBERTYPE + "' }";
        var request1 = {
            Applicationnumber: APPLICATIONNUMBER,
            Mianapplicantname: MIANAPPLICANTNAME,
            Calledpartyname: CALLEDPARTYNAME,
            Calledpartytype: CALLEDPARTYTYPE,
            Calledpartynumber: CALLEDPARTYNUMBER,
            Calledpartynumbertype: CALLEDPARTYNUMBERTYPE
        };
        if (CALLEDPARTYNAME != "" && CALLEDPARTYTYPE != "" && CALLEDPARTYNUMBER != "") {
            $.ajax({
                url: "/Portal/CallCenter/CallCenterInsert",
                type: "POST",
                async: true,
                data: request1,
                dataType: "json",
                success: function (result) {
                    // CALLEDPARTYNUMBER

                    if (result.code > 0) {
                        alert("新增成功");
                        $("#tabAddPhone").hide();//把新增文本框隐藏
                        InquiryApplicationnumber();
                    } else {
                        alert("新增失败");
                    }


                },
                error: function (xhr) {
                    alert("出错");

                },
            });

        } else {
            alert("请填写完整信息");
            return;

        }



    });

    //录音条件查询
    $("#RecordingConditions").click(function () {
        var applicationnumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
        var calledName = $("#calledName").val();
        var calledType = $("#calledType").val();
        var calledNumber = $("#calledNumber").val();
        $.ajax({
            url: "/Portal/CallCenter/XSSHQueryRecords?Applicationnumber=" + applicationnumber + "&Calledpartyname=" + calledName + "&Calledpartytype=" + calledType + "&Calledpartynumber=" + calledNumber,
            type: "GET",
            async: true,
            success: function (result) {
                if (result.code > 0) {
                    $("#RecordTab").html("");
                    $.each(result.data, function (index, obj) {
                        $("#RecordTab").append("<tr><td>" + (index + 1) + "</td><td>" + obj.Calledpartyname + "</td><td>" + obj.Calledpartytype + "</td><td>" + obj.Calledpartynumber + "<a onclick=\"TelephoneBox('" + obj.Calledpartyname + "','" + obj.Calledpartynumber + "','" + obj.Calledpartytype + "','" + obj.Calledpartynumbertype + "','','')\" data-toggle=\"modal\" data-target=\"#myModal1\" id=\"btn_AddBorrower\" ><img  src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a></td><td>" + obj.Calledpartynumbertype + "</td><td>" + obj.Createdtime + "</td><td><a target=\"_blank\" href=\"http://172.16.2.251/interface/api/?action=record_play&recording=/var/spool/asterisk/monitor/" + obj.Recordfilename + "\"><img src=\"../V1/img/2.png\" width=\"18\" height=\"18\" /></a></td></tr>");
                        //$("#RecordTab").append("<tr><td>" + (index + 1) + "</td><td>" + obj.Calledpartyname + "</td><td>" + obj.Calledpartytype + "</td><td onclick=\"CALLEDPARTYNUMBER('" + obj.Calledpartyname + "','" + obj.Calledpartynumber + "','" + obj.Calledpartytype + "','" + obj.Calledpartynumbertype + "','','')\">" + obj.Calledpartynumber + "<a  data-toggle=\"modal\" data-target=\"#myModal1\" id=\"btn_AddBorrower\"><img src=\"../V1/img/3.png\" width=\"20\" height=\"20\" /></a></td><td>" + obj.Calledpartynumbertype + "</td><td>" + obj.Createdtime + "</td><td><a target=\"_blank\" href=\"http://172.16.2.251/interface/api/?action=record_play&recording=/var/spool/asterisk/monitor/" + obj.Recordfilename + "\"><img src=\"../V1/img/2.png\" width=\"18\" height=\"18\" /></a></td></tr>");
                    })
                }
            },
            error: function (xhr) {
                alert("出错");
            },

        });
    })

}



//呼出
function dialOut() {
    var phoneposition = $('input[name=phoneposition]:checked').val();

    if (CALLEDPARTYNUMBERTYPE == "手机 Mobile Phone") {
        if (phoneposition == undefined || phoneposition == null || phoneposition == "") {
            shakeMsg("请选选择本地/外地号码");
            return;
        }
    }

    //模态框显示隐藏
    $('#myModal1').modal('hide');
    $('#myModal2').modal('show');
    //extension  主叫号码  extensionDst 被叫号码
    var extension = localStorage.getItem("phoneExtension");
    var extensionDst = $("#dialCalledNumber").val();


    //电话类型
    var mobileType = checkPhone(extensionDst) ? "手机 Mobile Phone" : "座机";

    if (phoneposition == "otherplace") {
        extensionDst = "0" + extensionDst;
    }
    if (extensionDst.startsWith("021-")) {
        extensionDst = extensionDst.substring(4);
        console.log(extensionDst);
    } else if (extensionDst.indexOf("-")) {
        extensionDst = extensionDst.replace("-", "");
        console.log(extensionDst);

    }

    //当前页面获取  主借人姓名  
    var mainPersonName = $("div[t='sqr']").eq(0).find("span[data-dtf='APPLICANT_DETAIL.FIRST_THI_NME']").text();
    //当前页面获取  申请编号
    var applicationNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    
    //呼叫人姓名
    var calledName = $("#dialCalledName").val();
    //呼叫人角色
    var calledType = $("#dialCalledType").val();
    //证件类型
    var idType = $("#dialIdType").val();
    //证件号码
    var idNumber = $("#dialIdNumber").val();
    //合同号
    var contractNo = $("#dialContractNo").val();

    var callerPosition = localStorage.getItem("userPosition");

    $.ajax({
        url: "https://172.16.2.251/admin/?m=interface&c=api&a=dial&extension=" + extension + "&extensionDst=" + extensionDst,
        type: "GET",
        async: false,
        dataType: "jsonp",
        success: function (result) {
            if (result.result == 1) {
                var callPopOutInfo = { CallType: 0, MainApplicantName: mainPersonName, ApplicationNumber: applicationNumber, CalledName: calledName, CalledType: calledType, CalledNumber: extensionDst, CalledIdType: idType, CalledIdNumber: idNumber, CalledNumberType: mobileType, ContractNo: contractNo, CallerPosition: callerPosition, CallerName: $.MvcSheetUI.SheetInfo.UserName, PhonePosition: phoneposition };
                localStorage.setItem("callPopOutInfo", JSON.stringify(callPopOutInfo));
            }
        },
        error: function (xhr) {
            alert("电话呼出出错");
        },
    });
}


//电话挂断
function handUp() {
    var extension = localStorage.getItem("phoneExtension");
    $.ajax({
        url: "https://172.16.2.251/admin/?m=interface&c=api&a=hangup&extension=" + extension,
        type: "GET",
        async: false,
        dataType: "jsonp",
        success: function (result) {
            $('#myModal1').modal('hide');
            $('#myModal2').modal('hide');
            if (result.result == 1) {
                //挂断电话 隐藏模态框
            }
        },
        error: function (xhr) {
            alert("电话挂断出错");
            $('#myModal2').modal('hide');
        },
    });
}

function checkPhone(phone) {
    if (!(/^1[3456789]\d{9}$/.test(phone))) {
        //alert("手机号码有误，请重填");
        return false;
    }
    return true;
}

//加载车300查询结果
function loadChe300QueryResult() {
    var url = "/Portal/ModelQuery/QueryModelInfo";
    $.ajax({
        url: url + "?t=" + new Date().getTime(),
        data: { "InstanceId": $.MvcSheetUI.SheetInfo.InstanceId },
        type: "POST",
        dataType: "json",
        async: true,
        success: function (result) {
            console.log(result);
            if (result.Code == "1") {
                if (result.Data && result.Data.length > 0) {
                    var html_val = "";
                    var che300_data_num = result.Data.length;
                    result.Data.forEach(function (che300_data, che300_num) {
                        var che300_model = "";
                        var che300_price = "";
                        var isok = false;
                        if (che300_data.model_name == $.MvcSheetUI.GetControlValue("VEHICLE_DETAIL.POWER_PARAMETER", 1)) {
                            che300_model = che300_data.model_name + "<span style=\"color:green;font-weight:700\">(一致)</span>";
                            isok = true;
                        }
                        else {
                            che300_model = che300_data.model_name + "<span style=\"color:red;font-weight:700\">(不一致)</span>";
                        }

                        if (parseFloat($.MvcSheetUI.GetControlValue("CONTRACT_DETAIL.TOTAL_ASSET_COST", 1)) <= che300_data.model_price) {
                            che300_price = che300_data.model_price + "<span style=\"color:green;font-weight:700\">(符合)</span>";
                            //isok = true;
                        }
                        else {
                            che300_price = che300_data.model_price + "<span style=\"color:red;font-weight:700\">(不符合)</span>";
                        }
                        var html_row = "<div class=\"row che300result\"><div class=\"col-md-8\"><div class=\"leftbox\" style=\"width: 17.5%\"><label data-type=\"SheetLabel\">VIN码查询车型</label></div><div class=\"centerline\" style=\"left:17.5%;\"></div><div class=\"rightbox\"><label><span>" + che300_model + "</span></label></div></div><div class=\"col-md-4\"><div class=\"leftbox\"><label data-type=\"SheetLabel\">VIN码查询价格</label></div><div class=\"centerline\"></div><div class=\"rightbox\"><label><span>" + che300_price + "</span></label></div></div></div>";
                        if (isok) {
                            html_val = html_row + html_val;
                        }
                        else {
                            html_val = html_val + html_row;
                        }
                    });
                    $("#powerPara_Row1").parent().after(html_val);
                    if (che300_data_num > 1) {
                        $(".che300result:eq(0) div:eq(0)").append("<i class=\"glyphicon glyphicon-chevron-right\" style=\"float:left;font-size:20px;margin-top:5px\"></i>");
                        $(".che300result:eq(0)").unbind("click").bind("click", function () {
                            che300ResultDetail();
                        }).nextAll(".che300result").hide();
                    }
                }
            }
            else {
                var html_row = "<div class=\"row che300result\"><div class=\"col-md-8\"><div class=\"leftbox\" style=\"width: 17.7%\"><label data-type=\"SheetLabel\">VIN码查询车型</label></div><div class=\"centerline\" style=\"left:17.5%;\"></div><div class=\"rightbox\"><label><span style=\"color:red;font-weight:700\">" + result.Message + "</span></label></div></div><div class=\"col-md-4\"><div class=\"leftbox\"><label data-type=\"SheetLabel\">VIN码查询价格</label></div><div class=\"centerline\"></div><div class=\"rightbox\"><label><span style=\"color:red;font-weight:700\">" + result.Message + "</span></label></div></div></div>";
                //var html_row = "<div class=\"row che300result\"><div class=\"col-md-2\"><label data-type=\"SheetLabel\">VIN码查询车型</label></div><div class=\"col-md-4\"><label><span style=\"color:red;font-weight:700\">" + result.Message + "</span></label></div><div class=\"col-md-2\"><label data-type=\"SheetLabel\">VIN码查询价格</label></div><div class=\"col-md-4\"><label><span style=\"color:red;font-weight:700\">" + result.Message + "</span></label></div></div>";
                $("#powerPara_Row1").parent().after(html_row);
            }
        }
    });
}

function che300ResultDetail() {
    if ($(".che300result:eq(0)").nextAll(".che300result").is(":hidden")) {
        $(".che300result:eq(0)").nextAll(".che300result").show();
        $(".che300result:eq(0) i").removeClass("glyphicon-chevron-right").addClass("glyphicon-chevron-down");
        $(".che300result").css({ "border-left": "2px solid #ccc", "border-right": "2px solid #ccc" });
        $(".che300result:first").css({ "border-top": "2px solid #ccc" });
        $(".che300result:last").css({ "border-bottom": "2px solid #ccc" });
    }
    else {
        $(".che300result:eq(0)").nextAll(".che300result").hide();
        $(".che300result:eq(0) i").removeClass("glyphicon-chevron-down").addClass("glyphicon-chevron-right");
        $(".che300result").css({ "border-left": "0px solid #ccc", "border-right": "1px solid #ccc" });
        $(".che300result:first").css({ "border-top": "1px solid #ccc" });
        $(".che300result:last").css({ "border-bottom": "1px solid #ccc" });
    }
}

function hideFiDataTrackLog() {
    $.ajax({
        url: "/Portal/Api/FIDataTrackLogCount?instanceId=" + $.MvcSheetUI.SheetInfo.InstanceId ,
        data: "",
        type: "GET",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result.code == 1 && result.data > 1) {
                $("#lb_datatracklog").removeClass("hidden");
            }
        },
        error: function (msg) {
            showJqErr(msg);
        }
    });
}