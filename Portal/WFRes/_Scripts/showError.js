

// jQuery 错误提示，可能部分angular写法也可用
var showJqErr = function (msg) {//19.7 wangxg
    if (msg.status === 200 || msg.status === 0) return;
    var err = '出现异常';
    if (msg.status === 800 || msg.status === 801 || msg.status === 802) {
        err = msg.responseText;// 自定义错误模块不能识别此错误。
    }
    // 以下根据异常代码重新定义提示
    if (msg.status === 800) {
        err = '您提交的内容检测到SQL攻击信息';
    } else if (msg.status === 801) {
        err = '检测到敏感符号';
    } else if (msg.status === 802) {
        err = '权限不足';
    }
    alert(err + '，异常代码=' + msg.status);
    return false;
};
// angular 错误提示
var showAgErr = function (data, header) {// 19.7 wangxg
    if (header === 200 || header === 0) return;
    var err = '出现异常';
    if (header === 800 || header === 801 || header === 802) {
        err = data;// 自定义错误模块不能识别此错误。
    }
    // 以下根据异常代码重新定义提示
    if (header === 800) {
        err = '您提交的内容检测到SQL攻击信息';
    } else if (header === 801) {
        err = '检测到敏感符号';
    } else if (header === 802) {
        err = '权限不足';
    }
    alert(err + '，异常代码=' + header);
};

