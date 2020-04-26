var ocr_token = "";
var ocr_discern_url = "";

//生成调用OCR接口的Token
function get_ocr_toke(regenerate) {
    if (ocr_token == "" || ocr_discern_url == "") {
        var url = "";
        if (regenerate)
            url = "/Portal/OCR/GetNewToken";
        else
            url = "/Portal/OCR/GetToken";
        $.ajax({
            url: url,
            data: "",
            type: "GET",
            async: false,
            dataType: "json",
            success: function (result) {
                var token = result.Token;
                if (token.code == "0") {
                    ocr_token = token.data;
                }
                else {
                    shakeMsg(token.message);
                }
                ocr_discern_url = result.Url;
            },
            //error: function (msg) {
            //    console.log(msg);
            //    shakeMsg("error:" + msg);
            //},
            error: function (msg) {// 19.7 
                showJqErr(msg);
            }
        });
    }
}

//生成调用OCR接口的formdata参数
function getOCRInvokerParameters(fileid, imagetype, iscut, dtf) {
    get_ocr_toke();
    var file_obj = document.getElementById(fileid).files[0];
    var fd = new FormData();
    fd.append("Authorization", ocr_token);
    fd.append('platformType', "1");
    fd.append('multipartFile', file_obj);
    fd.append('imageTyep', imagetype);
    fd.append('isSheetAttachment', false);
    if (iscut) {
        fd.append('isCutImage', iscut);
    }
    fd.append('SchemaCode', $.MvcSheetUI.SheetInfo.SchemaCode);
    //因为还未发起流程，或者保存流程时，Bizobjectid不确定，所以用InstanceID来替代，InstanceID不会变化；
    //然后在活动完成后再把InstanceID  Update  成BizObjectID
    fd.append('BizObjectID', $.MvcSheetUI.SheetInfo.InstanceId);
    fd.append('DataField', dtf);
    fd.append('InstanceId', $.MvcSheetUI.SheetInfo.InstanceId);
    return fd;
}

//生成SheetAttachment控件中的，调用OCR的识别的参数
function Ocr_Parameters(imagetype, iscut, that) {
    get_ocr_toke();
    var fd = new FormData();
    fd.append("Authorization", ocr_token);
    fd.append('platformType', "1");
    fd.append('imageTyep', imagetype);
    fd.append('isCutImage', iscut);
    fd.append('isSheetAttachment', true);
    fd.append('InstanceId', $.MvcSheetUI.SheetInfo.InstanceId);
    //这二个参数之前放在SheetAttachment.js中有BUG；
    fd.append('BizObjectID', $.MvcSheetUI.SheetInfo.BizObjectID);
    fd.append('DataField', that.DataField);
    return fd;
}

//生成SheetAttachment控件中的，调用图片压缩接口的参数
function PictureCompress_Parameters(factor) {
    get_ocr_toke();
    var fd = new FormData();
    fd.append("Authorization", ocr_token);
    fd.append('factor', factor);
    fd.append('InstanceId', $.MvcSheetUI.SheetInfo.InstanceId);
    return fd;
}

//身份证识别
//file:input file
//type:0人像，1国徽；
function idcard_identify(file, type) {
    if (document.getElementById($(file).attr("id")).files[0] == undefined) {
        $("[id*='idcard_" + type + "']").text("");
        return false;
    }
    $.LoadingMask.Show("正在努力识别中...");
    var fd = getOCRInvokerParameters($(file).attr("id"), "3", "1", "SFZ");
    //先从后台进行调用；
    var url = "/Portal/OCR/DiscernImageInfo";//ocr_discern_url
    try {
        $.ajax({
            type: 'post',
            url: url,
            data: fd,
            processData: false,  //tell jQuery not to process the data
            contentType: false,//"application/json;charset=UTF-8",
            dataType: "json",
            success: function (arg) {
                var result = arg;
                console.log(result);
                if (result.code == 0) {
                    var data = result.data;
                    if (type == "0") {
                        $("#idcard_0_name").text(data.name);
                        $("#idcard_0_idNumber").text(data.idNumber);
                        $("#idcard_0_birthday").text(data.birthday);
                        $("#idcard_0_sex").text(data.sex);
                        $("#idcard_0_people").text(data.people);
                        $("#idcard_0_address").text(data.address);
                        $("#idcard_img_0").attr("src", data.fileUrl).show();
                        $("#idcard_file_0").hide();
                    }
                    else if (type == "1") {
                        $("#idcard_1_issueAuthority").text(data.issueAuthority);
                        $("#idcard_1_validity").text(data.validity);
                        $("#idcard_img_1").attr("src", data.fileUrl).show();
                        $("#idcard_file_1").hide();
                    }
                }
                else {
                    if (result.message && result.message != "") {
                        showMsg(result.message);
                    }
                    else {
                        showMsg("身份证识别失败");
                    }
                    if (type == "0") {
                        $("[id*='idcard_0']").text("");
                        $("#idcard_img_0").attr("src", "").hide();
                        $("#idcard_file_0").val("").show();
                    }
                    else {
                        $("[id*='idcard_1']").text("");
                        $("#idcard_img_1").attr("src", "").hide();
                        $("#idcard_file_1").val("").show();
                    }
                }
                $.LoadingMask.Hide();
            }
            ,
            error: function (msg) {// 19.7 
                showJqErr(msg);
            }
        });
    }
    catch (err) {
        showMsg("身份证识别异常!");
        $.LoadingMask.Hide();
    }
}

//银行卡识别
function bankcard_identify(file) {
    if (document.getElementById($(file).attr("id")).files[0] == undefined) {
        $("[id*='bankcard_']").text("");
        return false;
    }
    $.LoadingMask.Show("正在努力识别中...");
    var fd = getOCRInvokerParameters($(file).attr("id"), "1", "0", "YHKMFYJ");//银行卡不切边
    //先从后台进行调用；
    var url = "/Portal/OCR/DiscernImageInfo";//ocr_discern_url
    $.ajax({
        type: 'post',
        url: url,
        data: fd,
        processData: false,  //tell jQuery not to process the data
        contentType: false,//"application/json;charset=UTF-8",
        dataType: "json",
        success: function (arg) {
            var result = arg;
            console.log(result);
            if (result.code == 0) {
                var data = result.data;
                $("#bankcard_type").text(data.type);
                $("#bankcard_cardNumber").text(data.cardNumber);
                $("#bankcard_validate").text(data.validate);
                $("#bankcard_holderName").text(data.holderName);
                $("#bankcard_issuer").text(data.issuer);
                $("#bankcard_img").attr("src", data.fileUrl).show();
                $("#bankcard_file").hide();
            }
            else {
                if (result.message && result.message != "") {
                    showMsg(result.message);
                }
                else {
                    showMsg("银行卡识别失败");
                }
                //失败,数据清空
                $("[id*='bankcard_']").text("");
                $("#bankcard_img").attr("src", "").hide();
                $("#bankcard_file").val("").show();
            }
            $.LoadingMask.Hide();
        }
        ,
        error: function (msg) {// 19.7 
            showJqErr(msg);
        }
    });
}

//身份证重新上传；
function idcard_reupload(type) {
    $("#idcard_img_" + type).hide();
    $("#idcard_file_" + type).val("").show();
}

//银行卡重新上传
function bankcard_reupload() {
    $("#bankcard_file").val("").show();
    $("#bankcard_img").hide();
}

//打开身份证识别模态框
function open_idcard_diag(con) {
    if ($.MvcSheetUI.IonicFramework.$scope.modal) {
        if ($.MvcSheetUI.IonicFramework.$scope.i_rowindex == $(con).closest("div.slider-slide").attr("data-row")) {
            $.MvcSheetUI.IonicFramework.$scope.modal.show();
            return false;
        }
        else {
            $.MvcSheetUI.IonicFramework.$scope.modal.remove();
        }
    }
    $.MvcSheetUI.IonicFramework.$scope.i_rowindex = $(con).closest("div.slider-slide").attr("data-row");
    $.MvcSheetUI.IonicFramework.$ionicModal.fromTemplateUrl('/Portal/Custom/template/OCR_idcard.html?T=' + new Date().getTime(), {
        scope: $.MvcSheetUI.IonicFramework.$scope
    }).then(function (model) {
        model.show();
        $.MvcSheetUI.IonicFramework.$scope.modal = model;
        $.MvcSheetUI.IonicFramework.$scope.confirm = function () {
            var index = $.MvcSheetUI.IonicFramework.$scope.i_rowindex;
            var d = get_idcard_data();
            if (d.idcard_name == "" || d.idcard_idNumber == "" || d.idcard_birthday == "" || d.idcard_sex == "" || d.idcard_people == "" ||
                d.idcard_address == "" || d.idcard_issueAuthority == "" || d.idcard_validity == "") {
                var n = layer.confirm("身份证识别错误或者识别不完整，确定返回并关闭?", function (n) {
                    var index = $.MvcSheetUI.IonicFramework.$scope.i_rowindex;
                    var d = get_idcard_data();
                    layer.close(n);
                    set_idcard_info(d, index, index);
                    $.MvcSheetUI.IonicFramework.$scope.modal.hide();
                });
            }
            else {
                set_idcard_info(d, index, index);
                $.MvcSheetUI.IonicFramework.$scope.modal.hide();
            }
        }
    });
}

//打开身份证识别模态框  (预审批表单)
function open_idcard_diag_ysp(con) {
    if ($.MvcSheetUI.IonicFramework.$scope.modal) {
        $.MvcSheetUI.IonicFramework.$scope.modal.show();
    }
    $.MvcSheetUI.IonicFramework.$ionicModal.fromTemplateUrl('/Portal/Custom/template/OCR_idcard.html?T=' + new Date().getTime(), {
        scope: $.MvcSheetUI.IonicFramework.$scope
    }).then(function (model) {
        model.show();
        $.MvcSheetUI.IonicFramework.$scope.modal = model;

        //隐藏背面上传，有效期，签发机关
        $("div.card:eq(1)").hide();
        $("#idcard_1_issueAuthority").closest("div.item").hide();
        $("#idcard_1_validity").closest("div.item").hide();

        $.MvcSheetUI.IonicFramework.$scope.confirm = function () {
            var d = get_idcard_data();
            if (d.idcard_name == "" || d.idcard_idNumber == "" ) {
                var n = layer.confirm("姓名或身份号码未识别，确定返回并关闭?", function (n) {
                    var d = get_idcard_data();
                    layer.close(n);
                    $.MvcSheetUI.SetControlValue("SYS_XM", d.idcard_name);
                    $.MvcSheetUI.SetControlValue("SYS_IDCARD", d.idcard_idNumber);
                    $.MvcSheetUI.IonicFramework.$scope.modal.hide();
                });
            }
            else {
                $.MvcSheetUI.SetControlValue("SYS_XM", d.idcard_name);
                $.MvcSheetUI.SetControlValue("SYS_IDCARD", d.idcard_idNumber);
                $.MvcSheetUI.IonicFramework.$scope.modal.hide();
            }
        }
    });
}

//获取身份证相关信息
function get_idcard_data() {
    var idcard_name = $("#idcard_0_name").text().trim();
    var idcard_idNumber = $("#idcard_0_idNumber").text().trim();
    var idcard_birthday = $("#idcard_0_birthday").text().trim();
    if (idcard_birthday != "") {
        idcard_birthday = idcard_birthday.replace("年", "-").replace("月", "-").replace("日", "");
    }
    var idcard_sex = $("#idcard_0_sex").text().trim();
    if (idcard_sex != "") {
        idcard_sex = (idcard_sex == "男" ? "M" : "F");
    }
    var idcard_people = $("#idcard_0_people").text().trim();
    if (idcard_people != "") {
        idcard_people = (idcard_people == "汉" ? "0001" : "0002");
    }
    var idcard_address = $("#idcard_0_address").text().trim();
    var idcard_issueAuthority = $("#idcard_1_issueAuthority").text().trim();
    var idcard_validity = $("#idcard_1_validity").text().trim();
    var idcard_validity_from = "";
    var idcard_validity_to = "";
    if (idcard_validity != "") {
        if (idcard_validity.indexOf('-') > -1) {
            var dates = idcard_validity.split('-');
            var date1 = dates[0];
            var date2 = dates[1];
            date1 = date1.replace(/\./g, "-").replace(/:/g, "-");
            date2 = (date2 == "长期" ? "9999-01-01" : date2.replace(/\./g, "-").replace(/:/g, "-"));

            if (date1.length != 10) {
                date1 = "";
            }
            if (date2.length != 10) {
                date2 = "";
            }

            idcard_validity_from = date1;
            idcard_validity_to = date2;
        }
    }
    var data = {};
    data["idcard_name"] = idcard_name;
    data["idcard_idNumber"] = idcard_idNumber;
    data["idcard_birthday"] = idcard_birthday;
    data["idcard_sex"] = idcard_sex;
    data["idcard_people"] = idcard_people;
    data["idcard_address"] = idcard_address;
    data["idcard_issueAuthority"] = idcard_issueAuthority;
    data["idcard_validity"] = idcard_validity;
    data["idcard_validity_from"] = idcard_validity_from;
    data["idcard_validity_to"] = idcard_validity_to;
    return data;
}

//给身份证相关字段赋值
function set_idcard_info(data,index,ads_index) {
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.FIRST_THI_NME", data.idcard_name, index);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.ID_CARD_NBR", data.idcard_idNumber, index);
    if (data.idcard_birthday != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.DATE_OF_BIRTH", data.idcard_birthday, index);
    }
    if (data.idcard_sex != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.SEX", data.idcard_sex, index);
    }
    if (data.idcard_people != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.NATION_CDE", data.idcard_people, index);
    }
    $.MvcSheetUI.SetControlValue("ADDRESS.ADDRESS_ID", data.idcard_address, ads_index);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.ISSUING_AUTHORITY", data.idcard_issueAuthority, index);
    if (data.idcard_validity != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.ID_CARDISSUE_DTE", data.idcard_validity_from, index);
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL.ID_CARDEXPIRY_DTE", data.idcard_validity_to, index);
    }
}

//给身份证相关字段赋值-正源零售申请
function set_idcard_info_zy(data, index, ads_index) {
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.FIRST_THI_NME", data.idcard_name, index);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.ID_CARD_NBR", data.idcard_idNumber, index);
    if (data.idcard_birthday != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.DATE_OF_BIRTH", data.idcard_birthday, index);
    }
    if (data.idcard_sex != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.SEX", data.idcard_sex, index);
    }
    if (data.idcard_people != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.NATION_CDE", data.idcard_people, index);
    }
    $.MvcSheetUI.SetControlValue("ADDRESS_ZY.ADDRESS_ID", data.idcard_address, ads_index);
    $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.ISSUING_AUTHORITY", data.idcard_issueAuthority, index);
    if (data.idcard_validity != "") {
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.ID_CARDISSUE_DTE", data.idcard_validity_from, index);
        $.MvcSheetUI.SetControlValue("APPLICANT_DETAIL_ZY.ID_CARDEXPIRY_DTE", data.idcard_validity_to, index);
    }
}

//打开银行卡识别模态框
function open_bankcard_diag(con) {
    if ($.MvcSheetUI.IonicFramework.$scope.modal_bankcard) {
        $.MvcSheetUI.IonicFramework.$scope.modal_bankcard.show();
        return false;
    }
    $.MvcSheetUI.IonicFramework.$ionicModal.fromTemplateUrl('/Portal/Custom/template/OCR_bankcard.html?T=' + new Date().getTime(), {
        scope: $.MvcSheetUI.IonicFramework.$scope
    }).then(function (model) {
        model.show();
        $.MvcSheetUI.IonicFramework.$scope.modal_bankcard = model;
        $.MvcSheetUI.IonicFramework.$scope.confirm_bankcard = function () {
            var idcard_number = $("#bankcard_cardNumber").text();//银行卡号
            var idcard_name = $("#bankcard_issuer").text();//发卡行
            idcard_number = idcard_number.replace(/ /g, "");
            if (idcard_number == "" || idcard_name=="") {
                var n = layer.confirm("银行或银行卡号未识别，确定返回并关闭?", function (n) {
                    layer.close(n);
                    var idcard_number = $("#bankcard_cardNumber").text();
                    var idcard_name = $("#bankcard_issuer").text();
                    idcard_number = idcard_number.replace(/ /g, "");
                    $.MvcSheetUI.SetControlValue("Cbankname", idcard_name);
                    $.MvcSheetUI.SetControlValue("Caccountnum", idcard_number);
                    $.MvcSheetUI.IonicFramework.$scope.modal_bankcard.hide();
                });
            }
            else {
                $.MvcSheetUI.SetControlValue("Cbankname", idcard_name);
                $.MvcSheetUI.SetControlValue("Caccountnum", idcard_number);
                $.MvcSheetUI.IonicFramework.$scope.modal_bankcard.hide();
            }
        }
    })
}

//身份证信息回写到表单上（PC表单）
function idcard_confirm() {
    var d = get_idcard_data();// 
    if (d.idcard_name == "" || d.idcard_idNumber == "" || d.idcard_birthday == "" || d.idcard_sex == "" || d.idcard_people == "" ||
                d.idcard_address == "" || d.idcard_issueAuthority == "" || d.idcard_validity == "") {
        var n = layer.confirm("身份证识别错误或者识别不完整，确定返回并关闭?", function (n) {
            var d = get_idcard_data();//
            layer.close(n);
            var n = $("#hide_rowindex_field").val();
            $("#modal_idcard").modal("hide");
            var ads_n;
            var code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", n);
            $("div[data-datafield=\"ADDRESS\"] div.rows").each(function (n, v) {
                var id = $(v).find("input[data-datafield=\"ADDRESS.IDENTIFICATION_CODE4\"]").val();
                if (id == code) {
                    ads_n = $(v).attr("data-row");
                    return false;
                }
            });

            set_idcard_info(d, n, ads_n);
        });
    }
    else {
        var n = $("#hide_rowindex_field").val();
        $("#modal_idcard").modal("hide");
        var ads_n;
        var code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL.IDENTIFICATION_CODE2", n);
        $("div[data-datafield=\"ADDRESS\"] div.rows").each(function (n, v) {
            var id = $(v).find("input[data-datafield=\"ADDRESS.IDENTIFICATION_CODE4\"]").val();
            if (id == code) {
                ads_n = $(v).attr("data-row");
                return false;
            }
        });

        set_idcard_info(d, n, ads_n);
    }

}


//身份证信息回写到表单上（PC表单--正源零售神奇）
function idcard_confirm_zy() {
    var d = get_idcard_data();// 
    if (d.idcard_name == "" || d.idcard_idNumber == "" || d.idcard_birthday == "" || d.idcard_sex == "" || d.idcard_people == "" ||
        d.idcard_address == "" || d.idcard_issueAuthority == "" || d.idcard_validity == "") {
        var n = layer.confirm("身份证识别错误或者识别不完整，确定返回并关闭?", function (n) {
            var d = get_idcard_data();//
            layer.close(n);
            var n = $("#hide_rowindex_field").val();
            $("#modal_idcard").modal("hide");
            var ads_n;
            var code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL_ZY.IDENTIFICATION_CODE2", n);
            $("div[data-datafield=\"ADDRESS_ZY\"] div.rows").each(function (n, v) {
                var id = $(v).find("input[data-datafield=\"ADDRESS_ZY.IDENTIFICATION_CODE4\"]").val();
                if (id == code) {
                    ads_n = $(v).attr("data-row");
                    return false;
                }
            });

            set_idcard_info_zy(d, n, ads_n);
        });
    }
    else {
        var n = $("#hide_rowindex_field").val();
        $("#modal_idcard").modal("hide");
        var ads_n;
        var code = $.MvcSheetUI.GetControlValue("APPLICANT_DETAIL_ZY.IDENTIFICATION_CODE2", n);
        $("div[data-datafield=\"ADDRESS_ZY\"] div.rows").each(function (n, v) {
            var id = $(v).find("input[data-datafield=\"ADDRESS_ZY.IDENTIFICATION_CODE4\"]").val();
            if (id == code) {
                ads_n = $(v).attr("data-row");
                return false;
            }
        });

        set_idcard_info_zy(d, n, ads_n);
    }

}


//身份证信息回写到表单上（PC端预审批表单）
function idcard_confirm_ysp() {
    var d = get_idcard_data();// 
    if (d.idcard_name == "" || d.idcard_idNumber == "" ) {
        var n = layer.confirm("姓名或身份号码未识别，确定返回并关闭?", function (n) {
            var d = get_idcard_data();//
            layer.close(n);
            $("#modal_idcard").modal("hide");
            $.MvcSheetUI.SetControlValue("SYS_XM", d.idcard_name);
            $.MvcSheetUI.SetControlValue("SYS_IDCARD", d.idcard_idNumber);
        });
    }
    else {
        $.MvcSheetUI.SetControlValue("SYS_XM", d.idcard_name);
        $.MvcSheetUI.SetControlValue("SYS_IDCARD", d.idcard_idNumber);
    }
    $("#modal_idcard").modal("hide");
}

//正源身份证信息回写到表单上（PC端预审批表单）
function idcard_confirm_ysp_zy() {
    var d = get_idcard_data();// 
    if (d.idcard_name == "" || d.idcard_idNumber == "") {
        var n = layer.confirm("姓名或身份号码未识别，确定返回并关闭?", function (n) {
            var d = get_idcard_data();//
            layer.close(n);
            $("#modal_idcard").modal("hide");
            $.MvcSheetUI.SetControlValue("zy_SYS_XM", d.idcard_name);
            $.MvcSheetUI.SetControlValue("zy_SYS_IDCARD", d.idcard_idNumber);
        });
    }
    else {
        $.MvcSheetUI.SetControlValue("zy_SYS_XM", d.idcard_name);
        $.MvcSheetUI.SetControlValue("zy_SYS_IDCARD", d.idcard_idNumber);
    }
    $("#modal_idcard").modal("hide");
}


//银行卡信息回写到表单上（PC表单）
function bankcard_confirm() {
    var idcard_number = $("#bankcard_cardNumber").text();//银行卡号
    var idcard_name = $("#bankcard_issuer").text();//发卡行
    idcard_number = idcard_number.replace(/ /g, "");
    if (idcard_number == "" || idcard_name == "") {
        var n = layer.confirm("银行或银行卡号未识别，确定返回并关闭?", function (n, idcard) {
            layer.close(n);
            var idcard_number = $("#bankcard_cardNumber").text();
            var idcard_name = $("#bankcard_issuer").text();
            idcard_number = idcard_number.replace(/ /g, "");
            $.MvcSheetUI.SetControlValue("Cbankname", idcard_name);
            $.MvcSheetUI.SetControlValue("Caccountnum", idcard_number);
            $("#modal_bankcard").modal("hide");
        });
    }
    else {
        $.MvcSheetUI.SetControlValue("Cbankname", idcard_name);
        $.MvcSheetUI.SetControlValue("Caccountnum", idcard_number);
        $("#modal_bankcard").modal("hide");
    }
}

//设置点击ORC按钮给Index赋值的功能
function set_idcard_index(con) {
    var v = $("#hide_rowindex_field").val();
    var d_row = $(con).closest("div.rows").attr("data-row")
    if (v != d_row) {
        //1.清空
        //2.保存id
        $("[id*='idcard_0']").text("");
        $("[id*='idcard_1']").text("");
        //身份证正面
        $("#idcard_img_0").attr("src", "").hide();
        $("#idcard_file_0").val("").show();
        //身份证反面
        $("#idcard_img_1").attr("src", "").hide();
        $("#idcard_file_1").val("").show();
        //保存当前index
        $("#hide_rowindex_field").val(d_row);
    }
}
