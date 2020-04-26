var ope_type = "";
var submit_msg = "";
var cancle_msg = "";
var workflowname = "";

function renderExtend() {
    $("[data-extend]").each(function (n, v) {
        var extend_value = $(v).data("extend");
        console.log(extend_value);
        if (extend_value == "wxappfile") {
            console.log($(v).val());
            if (isJSON($(v).val())) {
                var data_jsn = JSON.parse($(v).val());
                var v_html = "";
                $(data_jsn).each(function (n_k, v_k) {
                    v_html += "<li><a target=\"_blank\" href=\"" + urlFileView + v_k + "\">详细</a></li>";
                });
                v_html = "<div class=\"col-md-4\"><ul>" + v_html + "</ul></div>";
                $(v).parent().after(v_html);
            }
            else {
                var a_html = "<div class=\"col-md-4\"><a target=\"_blank\" class=\"SheetHyperLink\" href=\"" + urlFileView + $(v).val() + "\">详细</a></div >";
                $(v).parent().after(a_html);
            }
        }
    });
}

function isJSON(str) {
    if (typeof str == 'string') {
        try {
            var obj = JSON.parse(str);
            if (typeof obj == 'object' && obj) {
                return true;
            } else {
                return false;
            }

        } catch (e) {
            console.log('error：' + str + '!!!' + e);
            return false;
        }
    }
    console.log('It is not a string!')
}

function validateState() {
    var r = queryIsCancled($.MvcSheetUI.SheetInfo.InstanceId);
    if (r.code == 1) {
        if (r.data == "5") {
            alert("申请已取消，请确认");
            return false;
        }
        else if (r.data == "4") {
            alert("申请已结束，请确认");
            return false;
        }
    }
    else {
        alert(r.message);
        return false;
    }
    return true;
}

//确认提交后
function afterconfirm(okOrCancle) {
    debugger;
    var code = "";
    if (okOrCancle == "ok") {
        code = "A00006";
    }
    else if (okOrCancle == "cancle") {
        code = "A00005";
    }
    else {
        code = okOrCancle;
    }
    var result = updatewxappstate($.MvcSheetUI.SheetInfo.InstanceId, code, $("[data-datafield*='SPYJ'] textarea").val());
    if (result.code == 1) {
        return true;
    }
    else {
        alert("回写小程序状态失败");
        console.log(result.message);
        return false;
    }
}
//更新微信小程序的状态
function updatewxappstate(instanceid, code, message) {
    var ret;
    $.ajax({
        url: "/Portal/AfterLoanWorkflow/UpdateState?T=" + new Date().getTime(),
        data: { "InstanceId": instanceid, "Code": code, "Message": message },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },        error: function (msg) {// 19.7             var err = '出现异常';            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {                err = msg.responseText;            }            alert(err + ',异常代码=' + msg.status);            return false;        }
    });
    return ret;
}
//查询流程是否已取消
function queryIsCancled(instanceid) {
    var ret;
    $.ajax({
        url: "/Portal/AfterLoanWorkflow/InstanceState?T=" + new Date().getTime(),
        data: { "InstanceId": instanceid },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },        error: function (msg) {// 19.7             var err = '出现异常';            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {                err = msg.responseText;            }            alert(err + ',异常代码=' + msg.status);            return false;        }
    });
    return ret;
}
//更新微信小程序的状态
function addCancleComment(workitemid, datafield, comment) {
    var ret;
    $.ajax({
        url: "/Portal/AfterLoanWorkflow/AddCancleComment?T=" + new Date().getTime(),
        data: { "WorkitemId": workitemid, "DataField": datafield, "Comment": comment },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },        error: function (msg) {// 19.7             var err = '出现异常';            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {                err = msg.responseText;            }            alert(err + ',异常代码=' + msg.status);            return false;        }
    });
    return ret;
}
//更新流程数据项的值
function updateItemData(instanceid, datafield, value) {
    var ret;
    $.ajax({
        url: "/Portal/AfterLoanWorkflow/SetItemValue?T=" + new Date().getTime(),
        data: { "InstanceId": instanceid, "ItemCode": datafield, "ItemValue": value },
        type: "POST",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },        error: function (msg) {// 19.7             var err = '出现异常';            if (msg.status === 800 || msg.status === 801 || msg.status === 802) {                err = msg.responseText;            }            alert(err + ',异常代码=' + msg.status);            return false;        }
    });
    return ret;
}

function ifcanedit() {
    debugger;
    if ($.MvcSheetUI.SheetInfo.ActivityCode == "Activity3") {
        updateCannotEdit();
    }
}

function updateCannotEdit() {
    var v = $.MvcSheetUI.SheetInfo.BizObject.DataItems.IfCanEdit.V;
    if (!v) {
        //更新状态
        if (updateItemData($.MvcSheetUI.SheetInfo.InstanceId, "IfCanEdit", "true")) {
            updatewxappstate($.MvcSheetUI.SheetInfo.InstanceId, "A00008", "");
        }
    }
}