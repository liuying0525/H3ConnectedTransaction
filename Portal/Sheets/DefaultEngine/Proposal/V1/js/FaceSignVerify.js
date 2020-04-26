
$(window).resize(function () {
    $("#fktop").css({ "width": $("#content-wrapper").outerWidth(), "top": $("#main-navbar").outerHeight(), "left": $("#main-navbar").offset().left });
});

// 页面加载完成事件
$.MvcSheet.Loaded = function (sheetInfo) {

    getApplicantUserInfoByBizObject();
    getmsg();

    getFaceSignInfo();

    ////风控审核结果转换
    //var sysType = $.MvcSheetUI.SheetInfo.BizObject.DataItems["FK_SYS_TYPE"].V;

    //if (sysType == "1") {
    //    var fkresult = $.MvcSheetUI.SheetInfo.BizObject.DataItems["FK_RESULT"].V;

    //    if (fkresult == null || fkresult == "" || fkresult == undefined) {
    //        fkresult = "";
    //    }

    //    $("#Div3").append("<span style='color:red'>" + fkresult + "</span>");
    //}
    //else {
    //    var fkresult = $.MvcSheetUI.SheetInfo.BizObject.DataItems["fkResult"].V;
    //    var arrrsrq = []
    //    arrrsrq['localreject'] = "东正本地规则<span style=\"color:red;\">拒绝</span>";
    //    arrrsrq['cloudaccept'] = "云端规则<span style=\"color:red;\">通过</span>";
    //    arrrsrq['cloudreject'] = "云端规则<span style=\"color:red;\">拒绝</span>";
    //    arrrsrq['cloudmanual'] = "云端规则返回<span style=\"color:red;\">转人工</span>";
    //    arrrsrq['localmanual'] = "本地<span style=\"color:red;\">转人工</span>";
    //    $("#Div3").append(arrrsrq[fkresult]);
    //}

    //添加留言
    $('#addmsga').on('click', function () { addmsg(); });
}

// 表单验证接口
$.MvcSheet.Validate = function () {
    var Activity = $.MvcSheetUI.SheetInfo.ActivityCode;
    
    if (this.IsReject && this.Action == "Activity13") {
        var v = $.MvcSheetUI.MvcRuntime.executeService('CAPServiceNew', 'YYFKSQBack',
            {
                applicationNo: $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER"),
                comment: "面核人工审核驳回信审终审"
            }
        );
        if (!v || v.toUpperCase().indexOf("SUCCESS") == "-1") {
            shakeMsg("CAP错误:" + v);
            return false;
        }
    }
}

$.MvcSheet.AfterConfirmFinish = function () {
    var v = $.MvcSheetUI.MvcRuntime.executeService('CAPServiceNew', 'ProposalApproval',
        {
            InstanceID: $.MvcSheetUI.SheetInfo.InstanceId,
            Application_Number: $.MvcSheetUI.GetControlValue("APPLICATION_NUMBER"),
            StatusCode: "拒绝",//
            Approval_UserCode: $.MvcSheetUI.SheetInfo.BizObject.DataItems["Originator.LoginName"].V,//发起人登录名
            Approval_Comment:"面核人工审核聚聚" 
        }
    );
    if (!v || v.toUpperCase().indexOf("SUCCESS") == "-1") {
        shakeMsg("CAP错误:" + v);
        return false;
    }
    return true;
}


$.MvcSheet.PreInit = function () {
    $.MvcSheetUI.SheetInfo.PermittedActions.Save = false;    $.MvcSheetUI.SheetInfo.PermittedActions.ViewInstance = true;    $.MvcSheetUI.SheetInfo.PermittedActions.Submit = false;    $.MvcSheetUI.SheetInfo.PermittedActions.Print = true;
    $.MvcSheetUI.SheetInfo.PermittedActions.Reject = true;
    $.MvcSheetUI.SheetInfo.PermittedActions.Forward = false;
    $.MvcSheetUI.SheetInfo.PermittedActions.Close = true;
}

function getFaceSignInfo() {

    $.ajax({
        url: "/Portal/Api/QueryFaceSignResult?instanceid=" + $.MvcSheetUI.SheetInfo.InstanceId,
        data: "",
        type: "GET",
        dataType: "json",
        async: true,
        success: function (result) {
            if (result.code>0) {
                var html = "";
                $.each(result.data, function (index, value) {
                    html += "<div class=\"row\">";
                    html += "    <div class=\"col-md-12\">" + value.ApplicantType + "：" + value.Name + "<span style='color:red'>第" + (value.RetryNoIndex + 1) + "次</span>";
                    html += "    </div>";
                    html += "</div>";

                    html += "<div class=\"row\">";
                    html += "    <div class=\"col-md-4\">";
                    html += "        <div class=\"col-md-4\">姓名";
                    html += "        </div>";
                    html += "        <div class=\"col-md-8\">"+ value.Name;
                    html += "        </div>";
                    html += "     </div>";
                    html += "   <div class=\"col-md-8\">";
                    html += "       <div class=\"col-md-4\">证件号";
                    html += "       </div>";
                    html += "       <div class=\"col-md-8\">" + value.IdNumber;
                    html += "       </div>";
                    html += "   </div>";
                    html += "</div>";

                    html += "<div class=\"row\">";
                    html += "    <div class=\"col-md-4\">";
                    html += "        <div class=\"col-md-4\">手机号";
                    html += "        </div>";
                    html += "        <div class=\"col-md-8\">" + value.Mobile;
                    html += "        </div>";
                    html += "     </div>";
                    html += "   <div class=\"col-md-8\">";
                    html += "       <div class=\"col-md-4\">地址";
                    html += "       </div>";
                    html += "       <div class=\"col-md-8\">" + value.LiveAddress;
                    html += "       </div>";
                    html += "   </div>";
                    html += "</div>";

                    html += "<div class=\"row\" style=\"border-left: 1px solid #ccc;\">";
                    html += "    <div class=\"col-md-4\" style=\"border-left: 0px\">";
                    html += "        <div class=\"col-md-4\">面签状态";
                    html += "        </div>";
                    html += "        <div class=\"col-md-8\" style='color:red'>" + value.FaceSignResult;
                    html += "        </div>";
                    html += "     </div>";
                    html += "   <div class=\"col-md-8\">";
                    html += "       <div class=\"col-md-4\"><span>面签视频</span>";
                    html += "       </div>";
                    html += "       <div class=\"col-md-8\">";
                    html += "          <a class=\"btn btn-primary btn-md\" data-toggle=\"modal\" data-target=\"#myModal\" onclick=\"initVideoUrl('" + value.FaceSignNo + "','" + value.ApplicantType+"')\" style=\"margin: 3px; float: right; \">点击查看视频</a>";
                    html += "       </div>";
                    html += "   </div>";
                    html += "</div>";

                    html += "<div class=\"row bottom\">";
                    html += "    <div class=\"col-md-12\">";
                    html += "        <div class=\"col-md-4\">面签备注";
                    html += "        </div>";
                    html += "        <div class=\"col-md-8\">" + value.Resons;
                    html += "        </div>";
                    html += "     </div>";
                    html += "</div>";


                });
                $("#div_FaceSignInfo").append(html);
            }
            else {
                shakeMsg("获取人员面签信息出错！错误原因:" + result.Message);
            }
        },
        error: function (msg) {
            shakeMsg("获取人员面签信息出错！错误原因：" + msg);
        }
    });
}

function initVideoUrl(appNo, relType) {
    $("#facevideo").attr("src", videourl + "?appNo=" + appNo + "&relTyp=" + relType);
}

function videoStop() {
    var myVideo = document.getElementById("facevideo");
    myVideo.pause();
}
