function getAcls(user_code) {
    var ret;
    $.ajax({
        url: "/Portal/Acl/GetAcls",
        data: { user_code: user_code, T: new Date().getTime() },
        type: "GET",
        async: false,
        dataType: "json",
        success: function (result) {
            ret = result;
        },
        error: function (msg) {
            showJqErr(msg);
        }
    });
    return ret;
}