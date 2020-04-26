$(document).ready(function () {
    var type = window.performance.navigation.type;
    //alert("window.performance.navigation.type=" + type);

    if (type != 1) {
        return;
    }

    var userCode = localStorage.getItem("userCode");
    if (userCode != null) {
        loadPop(userCode);
    }
});

function loadcallback() {
    var script = document.createElement("script");
    document.p = script;
    script.type = "text/javascript";
    script.src = "/Portal/Contents/Scripts/call-center.js";
    document.getElementsByTagName('head')[0].appendChild(script);
    script.onload = function () {
    }
}

function mixcallback(str) {
    //alert(str);
    console.log("callback--->" + str);

    var method = QueryString(str, "Method");
    var recordFile = QueryString(str, "Recordfile");
    var uniqueid = QueryString(str, "Uniqueid");

    var department = localStorage.getItem("userDepartment");

    //打开新窗口 700px 620px
    var url = "";

    //呼入：全部弹access.html
    //呼出：信审弹outbound.html,运营弹access.html
    if (method == "Dialin") {
        url = "template/CallCenter/access.html?";//呼入
        var callInNumber = QueryString(str, "CallerIDName");

        ////非手机号码不弹屏
        //if (!checkPhone(callInNumber)) {
        //    return;
        //}
        var dznumber = callInNumber.substr(0, 4).indexOf("2068") >= 0;
        if (dznumber) {
            console.log("callback--->东正内部号码不弹窗" + callInNumber);
            return;
        }

        url = url + "callnumber=" + callInNumber + "&";
    }
    else if (method == "Dialout" && department == "贷后管理") {

        var callOutNumber = QueryString(str, "Calleeid");
        var dznumber = callOutNumber.substr(0, 4).indexOf("2068")>=0;
        if (dznumber) {
            console.log("callback--->东正内部号码不弹窗" + callOutNumber);
            return;
        }
        $("#myModal3").find('button[class="close"][data-dismiss="modal"]').click();
        $("#myModal4").find('button[class="close hidden"][data-dismiss="modal"]').click();
        url = "template/CallCenter/outboundyunying.html?callnumber=" + callOutNumber + "&";//呼出
    }
    else if (method == "Dialout" && department == "信贷审批部" ) {
        var callOutNumber = QueryString(str, "Calleeid");
        var dznumber = callOutNumber.substr(0, 4).indexOf("2068") >= 0;
        if (dznumber) {
            console.log("callback--->东正内部号码不弹窗" + callOutNumber);
            return;
        }
        $("#myModal3").find('button[class="close"][data-dismiss="modal"]').click();
        $("#myModal4").find('button[class="close hidden"][data-dismiss="modal"]').click();
        $("iframe").contents().find("#myModal1").find('button[class="close"][data-dismiss="modal"]').click();
        $("iframe").contents().find("#myModal2").find('button[class="close hidden"][data-dismiss="modal"]').click();
        url = "template/CallCenter/outbound.html?";//呼出
    }
    var iTop = (window.screen.availHeight - 30 - 620) / 2; 
    var iLeft = (window.screen.availWidth - 10 - 700) / 2;

    //打开新窗口，隐藏url地址栏，不可编辑宽度
    window.open(url + "recordFile=" + recordFile + "&uniqueid=" + uniqueid, "_blank", "height=620,width=700,top=" + iTop + ",left=" + iLeft + ",resizable=no,location=no");

}

function loadPop(userCode) {

    $.ajax({
        url: "CallCenter/SelectPhone?code=" + userCode ,
        type: "GET",
        async: false,
        success: function (result) {
            if (result.code == 1 && result.data != null && result.data != "") {

                $("object").remove();
                document.p = undefined;
                //信审只注册呼出
                //运营注册呼入呼出
                localStorage.setItem("phoneExtension", result.data.OfficePhone);
                localStorage.setItem("userPosition", result.data.Position);
                localStorage.setItem("userDepartment", result.data.DepartName);
                
                //if (!document.p) {
                    //localStorage.clear();//清空
                    //localStorage.setItem("temp", result.data); //存入参数:1.调用的值 2.所要存入的数据 
                    //console.log(localStorage.getItem("temp"));//输出
                    var department = localStorage.getItem("userDepartment");

                    var popOut = "ALL";

                    if (department == "信贷审批部") {
                        popOut = "DialOut";
                    }

                    var script = document.createElement("script");
                    document.p = script;
                    script.type = "text/javascript";
                    script.src = "CallCenter/InitPopJS?extension=" + result.data.OfficePhone + "&pop_out=" + popOut;//"http://172.16.2.251/admin/?m=interface&c=api&a=popscreen&extension=" + result.data.OfficePhone + "&pop_type=LINK&pop_out=" + popOut + "&open_type=2&mixcallback=mixcallback";
                    document.getElementsByTagName('head')[0].appendChild(script);
                    script.onload = function () {
                        //loadcallback();
                        $("body").css("overflow-y", "hidden");
                    }
                //}
            }
        }
    });
}

function QueryString(sourceparams, name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)", "i");
    var r = decodeURI(sourceparams.match(reg));
    if (r != null) return decodeURI(r.split(",")[2]); //   decodeURIComponent(encodeURIComponent (unescape(r[2])));
    return null;
}

function checkPhone(phone) {
    if (!(/^1[3456789]\d{9}$/.test(phone))) {
        //alert("手机号码有误，请重填");
        return false;
    }
    return true;
}