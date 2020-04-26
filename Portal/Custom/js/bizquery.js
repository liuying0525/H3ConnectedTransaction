function initCustomFilter() {
    //资产
    if (SchemaCode == "M_VEHICLE" && QueryCode == "fun_M_VEHICLE") {
        //增加查询条件
        debugger;
        var bp_id = inputMappings.split("|")[3].split(",")[1];
        var asset_condition = inputMappings.split("|")[4].split(",")[1];
        $("select[name='asset_make_cde']").select2({ placeholder: "" });
        $("select[name='asset_brand_cde']").select2({ placeholder: "" });
        $("select[name='asset_model_cde']").select2({ placeholder: "" });
        var data_mak = executeService("Get_Asset_Make", "Get_Asset_Make", { bp_id: bp_id, asset_condition: asset_condition });
        if (data_mak) {
            $("select[name='asset_make_cde']").find("option").remove();
            $("select[name='asset_make_cde']").append("<option value=\"\">--请选择--</option>");
            $(data_mak.data).each(function (n, v) {
                $("select[name='asset_make_cde']").append("<option value=\"" + v.asset_make_cde + "\">" + v.asset_make_dsc + "</option>");
            });
        }
        //绑定change事件
        $("select[name='asset_make_cde']").unbind("change.SheetDropDownList").bind("change.SheetDropDownList", function () {
            $("select[name='asset_brand_cde']").find("option").remove();
            $("select[name='asset_brand_cde']").prev().find("span.select2-chosen").html("--请选择--");
            $("select[name='asset_model_cde']").find("option").remove();
            $("select[name='asset_model_cde']").prev().find("span.select2-chosen").html("--请选择--");
            $("select[name='asset_brand_cde']").append("<option value=\"\">--请选择--</option>");
            $("select[name='asset_model_cde']").append("<option value=\"\">--请选择--</option>");
            var data = executeService("DropdownListDataSource", "get_asset_brand", { asset_make_cde: $("select[name='asset_make_cde']").val() });
            if (data == null || data == undefined)
                return false;
            $(data.List).each(function (n, v) {
                $("select[name='asset_brand_cde']").append("<option value=\"" + v.ASSET_BRAND_CDE + "\">" + v.ASSET_BRAND_DSC + "</option>");
            });
        });

        //绑定change事件
        $("select[name='asset_brand_cde']").unbind("change.SheetDropDownList").bind("change.SheetDropDownList", function () {
            $("select[name='asset_model_cde']").find("option").remove();
            $("select[name='asset_model_cde']").prev().find("span.select2-chosen").html("--请选择--");
            $("select[name='asset_model_cde']").append("<option value=\"\">--请选择--</option>");
            var data = executeService("DropdownListDataSource", "get_asset_model_code", { asset_make_cde: $("select[name='asset_make_cde']").val(), asset_brand_cde: $("select[name='asset_brand_cde']").val() });
            if (data == null || data == undefined)
                return false;
            $(data.List).each(function (n, v) {
                $("select[name='asset_model_cde']").append("<option value=\"" + v.ASSET_MODEL_CDE + "\">" + v.ASSET_MODEL_DSC + "</option>");
            });
        });
        //增加查询条件
        if (inputMappings) {
            var inputs = inputMappings.split("|");
            if (inputs && inputs.length > 0) {
                for (var i = 0; i < inputs.length; i++) {
                    var key = inputs[i].split(",")[0];
                    var value = inputs[i].split(",")[1];
                    $("[name='" + key + "']").val(value);
                    $("[name='" + key + "']").change();
                }
            }
        }

        
    }

    var divConfirm = "<div id='panelConfirm' style='margin-bottom:5px'><div class='container'><div class='row'><div class='col-md-2'><a class='btn btn-default btn-sm' onclick='btnConfirm()'>确 定</a></div></div></div></div>"
    $("#panelSearch").after(divConfirm);
}

function btnConfirm() {
    var radio = $("input[type='radio']:checked");
    if (radio.length == 0) {
        alert("未选中任何项");
    }
    else {
        $("input[type='radio']:checked").closest("tr").dblclick();
        //$("input[type='radio']:checked").trigger("dblclick");
    }
}

function executeService(serviceCode, methodName,options)
{
    var param = { cmd: "ExecuteServiceMethod", "ServiceCode": serviceCode, "MethodName": methodName };
    if (options) {
        for (var item in options) {
            if (!param[item.toLowerCase()])
                param[item.toLowerCase()] = options[item];
        }
    }
    debugger;
    var returnValue;
    $.ajax({
        type: "POST",
        async: false,
        url: "/Portal/AjaxServices.aspx",
        data: param,
        dataType: "json",
        success: function (data) {
            returnValue = data;
        },
        //error: function (e) {
        //    console.log(e);
        //},
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
    return returnValue;
}