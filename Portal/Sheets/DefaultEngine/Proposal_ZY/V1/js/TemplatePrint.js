//NTKO跳转 打开类型、是否重新打印
function jumpToEidtDoc(Type, isNew) {
    var ContractNumber
    var schemacode = $.MvcSheetUI.SheetInfo.SchemaCode;
    if (schemacode == "APPLICATION_ZY") {
        ContractNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    }

    if (ContractNumber == "" || ContractNumber == null) {
        alert("请填写合同编号！")
        return;
    }
    if ($("#select2").val() == "" || $("#select2").val() == null) {
        alert("请选择模板！")
        return;
    }
    if ($.MvcSheetUI.SheetInfo.IsOriginateMode) {
        $.MvcSheet.Action(
           {
               Action: "Save",//后台方法名称
               Datas: [], //输入参数，格式["{数据项名称}","string值","控件ID"]
               LoadControlValue: true,//是否获取表单数据
               PostSheetInfo: true, //是否获取已经改变的表单数据
               OnActionDone: function (e) {
                   if (e) {
                       var BizObjectID = $.MvcSheetUI.SheetInfo.BizObjectID;
                       var url = GetNTKOUrl(BizObjectID, ContractNumber, Type, isNew);
                       if (url == "" || url == undefined) return;
                       window.open(url);
                   }
               }
           });
    }
    else {
        var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
        var BizObjectID = $.MvcSheetUI.SheetInfo.BizObjectID;
        var url = GetNTKOUrl(BizObjectID, ContractNumber, Type, isNew);
        if (url == "" || url == undefined) return;
        window.open(url);
    }
}

//NTKO显示公共页面
function GetNTKOUrl(BizObjectID, ContractNumber, Type, isNew) {
    //文件对应编码
    //var file = $.MvcSheetUI.GetControlValue("Template");
    var file = $("#select2").val();

    //模版编码+流程BizObjectID做json key值
    var key = file + BizObjectID;
    var fileGuid = GetfileGuid(key);
    var CheckKeyJson = $.MvcSheetUI.GetControlValue("CheckKeyJson");
    var JSONstr = null;
    if (CheckKeyJson != "") {
        JSONstr = eval('(' + CheckKeyJson + ')');
    }
    if (JSONstr == null || fileGuid == "" || isNew) {
        //调用方法生成文件方法并获取fileGuid
        fileGuid = GetFileName(file, ContractNumber, fileGuid);
        //存入json字符串
        SetJson(JSONstr, key, fileGuid);
    }
    if (fileGuid == "") return;
    //var url = "/Portal/Sheets/NTKO/NTKO_TESTEdit.aspx?Mode=Originate&WorkflowCode=NTKO_TESTEdit&WorkflowVersion=1";
    var url = "";
    if (Type == "NTKO") {
        url = "/Portal/Sheets/NTKO/NTKOEdit.html";
        url = url + "?Template=" + encodeURI(fileGuid);
    } else {
        url = "/Portal/Sheets/pdfjs/web/viewer.html?file=" + encodeURI(fileGuid) + ".pdf";
    }
    return url + "&T=" + new Date().getTime();

}

//获取之前生成id
function GetfileGuid(key) {
    //获取表单内json字符串
    var CheckKeyJson = $.MvcSheetUI.GetControlValue("CheckKeyJson");
    var JSONstr = null;
    var fileGuid = "";
    //检查之前文件是否生成过
    if (CheckKeyJson != "") {
        JSONstr = eval('(' + CheckKeyJson + ')');
        for (var i = 0; i < JSONstr.length; i++) {
            //判断文件是否生成过
            if (JSONstr[i][key]) {
                //如果之前生成过则fileGuid取之前的
                fileGuid = JSONstr[i][key];
                break;
            }
        }
    }
    return fileGuid;
}

//生成文件并返回FileGUID（文件名称）
// file 文件对应编码
// ContractNumber 合同编码
function GetFileName(file, ContractNumber, fileGuid) {
    var FileGUID = "";
    $.MvcSheet.Action(
        {
            Action: "SetWordFile",//后台方法名称
            Datas: [file, ContractNumber, fileGuid], //输入参数，格式["{数据项名称}","string值","控件ID"]
            Async: false,
            LoadControlValue: false,//是否获取表单数据
            PostSheetInfo: false, //是否获取已经改变的表单数据
            OnActionDone: function (e) {
                if (e) {
                    if (e.Flag) {
                        FileGUID = e.FileName;
                        $.MvcSheetUI.SetControlValue("DKHT_FileName", e.FileName, 1);
                    } else {
                        alert("文件生成出错请联系管理员！");
                        return;
                    }
                }
            }
        });
    return FileGUID;
}

//生成json
function SetJson(JSONstr, key, value) {
    if (value == "") return;
    var json;
    var isSet = true;
    if (JSONstr == "" || JSONstr == null) {
        json = [];
    } else {
        json = JSONstr;
        for (var i = 0; i < JSONstr.length; i++) {
            //判断文件是否生成过
            if (JSONstr[i][key]) {
                isSet = false;
                break;
            }
        }
    }
    var j = {};
    //包含这个key则不插入
    if (isSet) {
        j[key] = value;
        json.push(j);
        var a = JSON.stringify(json);
        //写入表单 以备下次check
        $.MvcSheetUI.SetControlValue("CheckKeyJson", a);
    }

}


//判断是否打印过  打印过则显示重新打印按钮
function TemplateChange() {
    var btnshow = false;
    var CheckKeyJson = $.MvcSheetUI.GetControlValue("CheckKeyJson");
    var file = $("#select2").val();
    if (CheckKeyJson != "") {
        //var file = $.MvcSheetUI.GetControlValue("Template");

        var BizObjectID = $.MvcSheetUI.SheetInfo.BizObjectID;
        var key = file + BizObjectID;
        var JSONstr = eval('(' + CheckKeyJson + ')');
        for (var i = 0; i < JSONstr.length; i++) {
            //判断文件是否生成过
            if (JSONstr[i][key]) {
                btnshow = true;
                break;
            }
        }
    }
    var IsWork = $.MvcSheetUI.QueryString("Mode");

    if (file == "" || file == null) {
        $("#space10").hide();
    } else {
        $("#space10").show();
        if (IsWork == "work") {
            if (btnshow) {
                $("#NewPrint").show();
            } else {
                $("#NewPrint").hide();
            }
        } else {
            if (isAdmin() && btnshow) {
                $("#NewPrint").show();
            } else {
                $("#NewPrint").hide();
            }
        }
    }
}


function PrintHYX() {
    var i = 0;
    var file = "79548df0-cc9f-4570-a8cd-6ac443313a16"; //欢迎信模版objectid
    //$("#ctl371782").find("option").each(function () {
    //    if ($(this).text() == "欢迎信、还款计划表") {
    //        file = $(this).val();
    //    }
    //});
    //if (file == "") {
    //    alert("欢迎信、还款计划表  模版未添加！");
    //    return;
    //}
    var BizObjectID = $.MvcSheetUI.SheetInfo.BizObjectID;
    var ContractNumber = $.MvcSheetUI.GetControlValue("applicationNo");
    //模版编码+流程BizObjectID做json key值
    var key = file + BizObjectID;
    var fileGuid = GetFileName(file, ContractNumber, "");
    if (fileGuid == "") return;
    var url;
    url = "/Portal/Sheets/NTKO/NTKOEdit.html";
    url = url + "?Template=" + encodeURI(fileGuid);
    window.open(url);

}


function IsFinished() {
    var isEnd = false;
    var InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
    $.MvcSheet.Action(
        {
            Action: "IsFinished",//后台方法名称
            Datas: [InstanceId], //输入参数，格式["{数据项名称}","string值","控件ID"]
            Async: false,
            LoadControlValue: false,//是否获取表单数据
            PostSheetInfo: false, //是否获取已经改变的表单数据
            OnActionDone: function (e) {
                if (e) {
                    isEnd = e;
                }
            }
        });

    return isEnd;
}

function isAdmin() {
    var isAdmin = false;
    var UserID = $.MvcSheetUI.SheetInfo.UserID;
    $.MvcSheet.Action(
        {
            Action: "isAdmin",//后台方法名称
            Datas: [UserID], //输入参数，格式["{数据项名称}","string值","控件ID"]
            Async: false,
            LoadControlValue: false,//是否获取表单数据
            PostSheetInfo: false, //是否获取已经改变的表单数据
            OnActionDone: function (e) {
                if (e) {
                    isAdmin = e;
                }
            }
        });

    return isAdmin;
}


function SheetLoaded() {

    TemplateChange();
    var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
    var SchemaCode = $.MvcSheetUI.SheetInfo.SchemaCode;
    var IsWork = $.MvcSheetUI.QueryString("Mode");
    var type;

    if (SchemaCode == "APPLICATION_ZY") {
        if (Activity == 'Activity119') type = "正源";
    }
    
    var IsView = false;
    if (IsWork == "view") {
        IsView = true;
    }
    if (SchemaCode == "APPLICATION_ZY" && Activity == 'Activity119'  && !IsMobile) {
        $("#TemplateDiv").show();

        $.MvcSheet.Action({
            Action: "Getfiledata",    // 后台方法名称
            Datas: [type, IsView],// 输入参数，
            LoadControlValue: false,  // 是否获取表单数据
            PostSheetInfo: false,    // 是否获取已经改变的表单数据
            Async: true,
            OnActionDone: function (e) {
                if (e.length <= 0) {
                    $("#TemplateDiv").hide();

                } else {
                    for (var i = 0; i < e.length; i++) {
                        $("#select2").append("<option value='" + e[i].OBJECTID + "'>" + e[i].TEMPLATENAME + "</option>");
                    }
                }


            }
        })


    }



    $.MvcSheetUI.SetControlValue("WorkflowType", type);
    if (Activity == 'Activity16' && IsWork == "view" && !IsMobile && IsFinished()) {
        $("#HYXprint").show();
    }
}