//NTKO跳转 打开类型、是否重新打印
function jumpToEidtDoc(Type, isNew) {
    var ContractNumber
    var schemacode = $.MvcSheetUI.SheetInfo.SchemaCode;
    if (schemacode == "RetailApp" || schemacode == "CompanyApp") {
        ContractNumber = $.MvcSheetUI.GetControlValue("applicationNo");
    }
    else if (schemacode == "APPLICATION") {
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
        TemplateChange();//生成过，就显示重新打印的按钮；
        window.open(url);
    }
}

//NTKO显示公共页面
function GetNTKOUrl(BizObjectID, ContractNumber, Type, isNew) {
    //文件对应编码
    //var file = $.MvcSheetUI.GetControlValue("Template");
    var file = $("#select2").val();

    var fileGuid = GetExistAttachmentID(file, ContractNumber);
    if (fileGuid == "" || isNew) {
        //调用方法生成文件方法并获取fileGuid
        fileGuid = GetFileName(file, ContractNumber);
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


//生成文件并返回FileGUID（文件名称）
// file 文件对应编码
// ContractNumber 合同编码
function GetFileName(file, ContractNumber) {
    var FileGUID = "";
    $.MvcSheet.Action(
        {
            Action: "SetWordFile",//后台方法名称
            Datas: [file, ContractNumber], //输入参数，格式["{数据项名称}","string值","控件ID"]
            Async: false,
            LoadControlValue: false,//是否获取表单数据
            PostSheetInfo: false, //是否获取已经改变的表单数据
            OnActionDone: function (e) {
                if (e) {
                    if (e.Flag) {
                        //FileGUID = e.FileName;
                        FileGUID = e.Id;
                    } else {
                        console.log(e.Message);
                        alert("文件生成出错请联系管理员！");
                        return;
                    }
                }
            }
        });
    return FileGUID;
}

function GetExistAttachmentID(templateId, contractNumber) {
    if (templateId == "")
        return "";
    var id = "";
    $.MvcSheet.Action(
        {
            Action: "GetExistAttachmentID",//后台方法名称
            Datas: [templateId, contractNumber], //输入参数，格式["{数据项名称}","string值","控件ID"]
            Async: false,
            LoadControlValue: false,//是否获取表单数据
            PostSheetInfo: false, //是否获取已经改变的表单数据
            OnActionDone: function (e) {
                id = e.AttachmentID;
            }
        });
    return id;
}

//判断是否打印过  打印过则显示重新打印按钮
function TemplateChange() {
    var btnshow = false;
    var file = $("#select2").val();
    var ContractNumber
    var schemacode = $.MvcSheetUI.SheetInfo.SchemaCode;
    if (schemacode == "RetailApp" || schemacode == "CompanyApp") {
        ContractNumber = $.MvcSheetUI.GetControlValue("applicationNo");
    }
    else if (schemacode == "APPLICATION") {
        ContractNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
    }

    var fileGuid = GetExistAttachmentID(file, ContractNumber);
    var statusCode = getStatusCode(ContractNumber);
    var admin = isAdmin();

    if (fileGuid != "") {
        btnshow = true;
    }

    // 管理员账号 && 已放款
    if (admin && statusCode === '55') {
        btnshow = false;
    }

    var IsWork = $.MvcSheetUI.QueryString("Mode");

    if (file == "" || file == null) {
        $("#space10").hide();
        $("#NewPrint").hide();
    } else {
        $("#space10").show();
        if (IsWork == "work") {
            if (btnshow) {
                $("#NewPrint").show();
            } else {
                $("#NewPrint").hide();
            }
        } else {
            if (admin && btnshow) {
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
    var ContractNumber = $.MvcSheetUI.GetControlValue("applicationNo");

    var fileGuid = GetFileName(file, ContractNumber);
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

// 获取放款状态
function getStatusCode(applicationNumber) {
    var statusCode;
    $.MvcSheet.Action(
        {
            Action: "GetStatusCode",//后台方法名称
            Datas: [applicationNumber], //输入参数，格式["{数据项名称}","string值","控件ID"]
            Async: false,
            LoadControlValue: false,//是否获取表单数据
            PostSheetInfo: false, //是否获取已经改变的表单数据
            OnActionDone: function (e) {
                if (e) {
                    statusCode = e;
                }
            }
        });

    return statusCode;
}

function SheetLoaded() {

    TemplateChange();
    var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
    var SchemaCode = $.MvcSheetUI.SheetInfo.SchemaCode;
    var IsWork = $.MvcSheetUI.QueryString("Mode");
    var type;
    var applicationNumber="";
    if (SchemaCode == "RetailApp") {
        if (Activity == 'Activity16') type = "个人";
        if (Activity == 'Activity8') type = "个人_附件上传";
    }
    else if (SchemaCode == "CompanyApp") {
        if (Activity == 'Activity16') type = "机构";
        if (Activity == 'Activity8') type = "机构_附件上传";
    }
    else if (SchemaCode == "APPLICATION") {
        var t = $.MvcSheetUI.SheetInfo.BizObject.DataItems["APPLICATION_TYPE_CODE"].V;
        applicationNumber = $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER");
        if (t == "00001") {//个人
            if (Activity == 'Activity48') type = "个人";
            if (Activity == 'Activity8') type = "个人_附件上传";
        }
        else {//机构
            if (Activity == 'Activity48') type = "机构";
            if (Activity == 'Activity8') type = "机构_附件上传";
        }
    }
    var IsView = false;
    if (IsWork == "view") {
        IsView = true;
    }
    if ((
        (SchemaCode == "RetailApp" && (Activity == 'Activity16' || Activity == 'Activity8')) ||
        (SchemaCode == "CompanyApp" && (Activity == 'Activity16' || Activity == 'Activity8')) ||
        (SchemaCode == "APPLICATION" && (Activity == 'Activity48' || Activity == 'Activity8'))
        ) && !IsMobile) {
        $("#TemplateDiv").show();

        $.MvcSheet.Action({
            Action: "Getfiledata",    // 后台方法名称
            Datas: [type, IsView, applicationNumber],// 输入参数，
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