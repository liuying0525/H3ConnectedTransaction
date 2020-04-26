app.controller('LoginController', ['$rootScope', '$scope', '$translate', '$http', '$location', '$state', '$stateParams', '$timeout', '$interval', 'ControllerConfig',
    function ($rootScope, $scope, $translate, $http, $location, $state, $stateParams, $timeout, $interval, Controller) {
        $scope.needResetPwd = false;
        $scope.LoginSuccess = true;
        $scope.ConnectionFailed = true;
        $scope.EnginePasswordValid = true;//引擎密码错误
        $scope.isClick = false;
        $rootScope.$on('$viewContentLoaded', function () {
        });

        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });
        $scope.getLanguage = function () {
            $scope.LanJson = {
                Code: $translate.instant("LoginController.EnterName"),
                Password: $translate.instant("LoginController.EnterPassword"),
                CheckCode: $translate.instant("LoginController.EnterCheckCode"),
                OldPassword: "请输入原密码",
                NewPassword: "请输入新密码",
                NewPasswordConfirm: "请再次输入新密码",
            };
            if ($scope.LanJson.Code == "LoginController.EnterName") {
                $scope.LanJson = {
                    Code: "请输入用户名",
                    Password: "请输入密码",
                    CheckCode: "请输入验证码",
                    OldPassword: "请输入原密码",
                    NewPassword: "请输入新密码",
                    NewPasswordConfirm: "请再次输入新密码",
                };
            }
        };
        $scope.getLanguage();


        //钉钉单点登录开始
        // 处理单点登录
        $scope.getUrlParam = function (name) {
            var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
            var r = window.location.search.substr(1).match(reg);
            if (r != null) return unescape(r[2]); return null;
        };

        var loginFrom = $scope.getUrlParam("loginfrom");
        var state = $scope.getUrlParam("state");
        var code = $scope.getUrlParam("code");
        var workItemID = $scope.getUrlParam("WorkItemID")
        $("object").remove();
        localStorage.removeItem("phoneExtension");
        localStorage.removeItem("userCode");
        localStorage.removeItem("userPosition");
        localStorage.removeItem("userName");
        localStorage.removeItem("userDepartment");

        //根据URL参数判断是否钉钉登录
        if (loginFrom == "dingtalk") { //TODO
            $scope.IsSSO = true;
            debugger;
            //commonJS.loadingHide();
            if (code && state && !workItemID) {
                $scope.LoginSuccess = true;
                $.ajax({
                    url: "Organization/ValidateLoginForDingTalkPC",
                    data: {
                        state: state,
                        code: code
                    },
                    async: false,
                    success: function (result) {
                        $scope.$emit("LoginIn", result);
                        // 设置主界面
                        if (result.Success) {
                            var redirectUrl = window.localStorage.getItem("H3.redirectUrl");
                            if (redirectUrl && redirectUrl != "" && redirectUrl.indexOf("Redirect.html") != -1) {
                                window.localStorage.setItem("H3.PortalRoot", result.PortalRoot);
                                window.localStorage.setItem("H3.redirectUrl", "");
                                $timeout(function () {
                                    window.location.href = redirectUrl;
                                }, 500)
                            } else {
                                //去掉参数,跳转到待办页面
                                var rUrl = window.location.href.replace(window.location.search, "");
                                var index = rUrl.indexOf("#/");
                                rUrl = rUrl.substring(0, index);
                                window.location.href = rUrl + "#/app/Workflow/MyUnfinishedWorkItem";
                            }
                            $scope.LoginSuccess = true;
                        }

                    }
                });
            }
        }
        $scope.getWaitTime = function () {
            var tmp = [];
            try {
                tmp = JSON.parse(localStorage["LoginCheckEroor"]);
            } catch (e) {
                tmp = [];
            }
            var position = tmp.length > $scope.wait.length ? $scope.wait.length - 1 : tmp.length - 1;
            return position;
        };

        // -1无验证码，0有验证码不等待，其他有验证码也等待
        $scope.wait = [-1, -1, 0, 0, 1, 5, 30, 120];
        $scope.hasCheckCode = $scope.wait[$scope.getWaitTime()] >= 0;

        setTimeout(function () {
            // 点击图片刷新
            $('#code_img').unbind('click').bind("click", function () { resetCode(); });
        }, 200);

        $scope.loginIn = function () {



            var flag = false;
            var interval;
            if ($scope.isClick) {
                return;
            }
            $scope.isClick = true;
            $scope.userCode = $("#txtUser").val();
            $scope.userPassword = $("#txtPassword").val();
            $scope.checkCode = $("#txtCheckCode").val();
            if ($scope.userCode == "" || $scope.userCode == undefined) {
                focus("userCode");
                flag = true;
            }
            if ($scope.userPassword == "" || $scope.userPassword == undefined) {
                focus("userPassword");
                flag = true;
            }
            /*var position = $scope.getWaitTime();
            var tmp = [];
            try {
                tmp = JSON.parse(localStorage["LoginCheckEroor"]);
            } catch (e) {
                tmp = [];
            }
            var passtime = parseInt(tmp.length > 0 ? (new Date() - Date.parse(tmp[tmp.length - 1])) / 1000 / 60 : -1);
            if (passtime >= 0 && passtime < $scope.wait[position]) {
                alert('验证过于频繁，请等待[' + ($scope.wait[position] - passtime) + '分]后尝试。');
                flag = true;
            }
            if ($scope.wait[position] >= 0 && $scope.checkCode.toLocaleLowerCase() != verVal.toLocaleLowerCase()) {
                $scope.LoginSuccess = false;
                if (!$scope.LoginSuccess) {
                    $interval(function () {
                        $scope.LoginSuccess = true;
                    }, 3000);
                }
                return;
            }//2019-08-12*/
            if ($scope.hasCheckCode) {
                $scope.isClick = false;
                if ($scope.checkCode.toLocaleLowerCase() != verVal.toLocaleLowerCase()) {
                    $scope.LoginSuccess = false;
                    if (!$scope.LoginSuccess) {
                        $interval(function () {
                            $scope.LoginSuccess = true;
                        }, 3000);
                    }
                    flag = true;
                }
            }
            $.ajax({
                async: false,
                type: "Post",
                url: "/Portal/Ajax/VerificationCode.ashx",
                data: { check: $scope.checkCode, user: $scope.userCode, code: $scope.checkCode, action: "Submit" },
                success: function (g) {
                    var ct = g.split("|");
                    if (ct[0] >= 2 && !$scope.hasCheckCode) {
                        flag = true;
                        $scope.hasCheckCode = true;
                        $scope.isClick = false;//wangxg 19.11
                    }
                    if (ct[1] > 0) {
                        $("#loginIn").attr("disabled", "disabled").find("span").text("验证过于频繁，请等待(" + ct[1] + ")秒...");
                        var timer = ct[1];
                        interval = setInterval(function () {
                            if (timer == 0) { clearInterval(interval); $("#loginIn").removeAttr("disabled").find("span").text("登录"); }
                            else $("#loginIn span").text("验证过于频繁，请等待(" + (timer--) + ")秒...");
                        }, 1000);
                    }
                }
            });
            if (flag) return;
            $http({
                url: "/Portal/Login/SecureLogin",
                //Controller.Organization.LoginIn,
                method: "post",
                data: {
                    userCode: this.userCode,
                    password: this.userPassword,
                    rendom: new Date().getTime()
                }
            })
                .success(function (result, header, config, status) {
                    $scope.$emit("LoginIn", result);
                    // 设置主界面
                    if (result.Success) {
                        clearInterval(interval);
                        $scope.hasCheckCode = false;
                        $.post("/Portal/Ajax/VerificationCode.ashx", { user: $scope.userCode, action: "Success" }, function (g) { });
                        //localStorage["LoginCheckEroor"] = '';//2019-08-12
                        var redirectUrl = window.localStorage.getItem("H3.redirectUrl");
                        if (redirectUrl && redirectUrl != "" && redirectUrl.indexOf("Redirect.html") != -1) {
                            window.localStorage.setItem("H3.PortalRoot", result.PortalRoot);
                            window.localStorage.setItem("H3.redirectUrl", "");
                            $timeout(function () {
                                window.location.href = redirectUrl;
                            }, 500);
                        } else {

                            $state.go("app.MyUnfinishedWorkItem", { TopAppCode: "Workflow" });
                        }
                        $scope.LoginSuccess = true;
                        //$.ajax({
                        //    //url: "ajax/DZBizHandler.ashx",
                        //    url: "/Portal/DZBizHandler/ldaplogin",// 19.6.28 wangxg
                        //    data: {
                        //        CommandName: "ldaplogin",
                        //        UserCode: $scope.userCode
                        //    },
                        //    type: "POST",
                        //    async: true,
                        //    success: function (result) {
                        //    }
                        //});
                        var appellation = result.User.Appellation;
                        //window.localStorage.setItem("userPosition", appellation);
                        //window.localStorage.setItem("userDepartment", result.User.OUDepartName);
                        window.localStorage.setItem("userName", result.User.Name);
                        window.localStorage.setItem("userCode", $scope.userCode);
                        window.localStorage.setItem("appellation", appellation);
                        $("object").remove();
                        loadPop($scope.userCode);
                    }
                    else {
                        $scope.isClick = false;
                        if (result.ErrorCode && result.ErrorCode == 1) {
                            $scope.PasswordComplexityValid = false; //密码复杂度不符合要求
                            if (!$scope.PasswordComplexityValid) {
                                $interval(function () {
                                    $scope.PasswordComplexityValid = true;
                                }, 5000);
                            }
                            alert(result.Message);
                            $scope.needResetPwd = true;

                        } else if (result.Message == "ConnectionFailed") {
                            $scope.ConnectionFailed = false;
                            if (!$scope.ConnectionFailed) {
                                $interval(function () {
                                    $scope.ConnectionFailed = true;
                                }, 5000);
                            }
                        } else if (result.Message == "EnginePasswordInvalid") {
                            $scope.EnginePasswordValid = false; //引擎密码错误
                            if (!$scope.EnginePasswordValid) {
                                $interval(function () {
                                    $scope.EnginePasswordValid = true;
                                }, 5000);
                            }
                        } else {
                            /*tmp.push(new Date());
                            localStorage["LoginCheckEroor"] = JSON.stringify(tmp);
                            $scope.hasCheckCode = $scope.wait[$scope.getWaitTime()] >= 0;//2019-08-12*/
                            //$.post("/Portal/Ajax/VerificationCode.ashx", { user: $scope.userCode, action: "Error", code: $scope.checkCode }, function (g) { });
                            $scope.LoginSuccess = false;
                            if (!$scope.LoginSuccess) {
                                $interval(function () {
                                    $scope.LoginSuccess = true;
                                }, 3000);
                            }
                        }
                    }

                })
                .error(function (data, header, config, status) {
                    $scope.isClick = false;
                });
        }

        $scope.resetPwd = function () {
            $scope.validateInput();
            if ($scope.PwdResetValidateFailed) {
                return;
            }
            $http({
                url: "/Portal/Login/SetPassword",
                method: "post",
                data: {
                    user_code: $scope.userCode,
                    old_pwd: this.oldPassword,
                    new_pwd: this.newPassword,
                    rendom: new Date().getTime()
                }
            }).success(function (result, header, config, status) {
                if (result) {
                    alert("密码更新成功");
                    location.reload();
                }
                else {
                    $scope.PwdResetValidateFailed = true;
                    $scope.PwdValidateErrorMsg = "当前密码不正确";
                }
            });

        }

        $scope.validateInput = function () {

            $scope.oldPassword = $("#oldPassword").val();
            $scope.newPassword = $("#newPassword").val();
            $scope.newPasswordConfirm = $("#newPasswordConfirm").val();

            if ($scope.oldPassword == $scope.newPassword) {
                $scope.PwdResetValidateFailed = true;
                $scope.PwdValidateErrorMsg = "新密码不能与原密码一致";
            } else {
                $scope.PwdResetValidateFailed = false;
                $scope.PwdValidateErrorMsg = "";
            }


            //var reg = new RegExp("^(((?=.*[0-9])(?=.*[a-zA-Z]))|((?=.*[0-9])(?=.*[!@#$%\^&*\(\)]))|((?=.*[a-zA-Z])(?=.*[!@#$%\^&*\(\)]))).{6,16}$");
            if (!$scope.regCheck($scope.newPassword)) {
                $scope.PwdResetValidateFailed = true;
                $scope.PwdValidateErrorMsg = "请输入6-16位的数字、字母、常用符号，不能为单一组合";
                //if (!$scope.PwdResetValidateFailed) {
                //    $interval(function () {
                //        $scope.PwdResetValidateFailed = false;
                //        $scope.PwdValidateErrorMsg = "";
                //    }, 5000);
                //}
                return;
            }
            else {
                $scope.PwdResetValidateFailed = false;
                $scope.PwdValidateErrorMsg = "";
            }

            if ($scope.newPassword != $scope.newPasswordConfirm) {
                $scope.PwdResetValidateFailed = true;
                $scope.PwdValidateErrorMsg = "输入不匹配";
                //if (!$scope.PwdResetValidateFailed) {
                //    $interval(function () {
                //        $scope.PwdResetValidateFailed = false;
                //        $scope.PwdValidateErrorMsg = "";
                //    }, 5000);
                //}
                return;
            }
            else {
                $scope.PwdResetValidateFailed = false;
                $scope.PwdValidateErrorMsg = "";
            }
            
        };

        $scope.regCheck = function (v) {
            //长度
            var len = (v.length >= 6 && v.length <= 16) ? true : false;
            //数字
            var num = v.match(/\d/g) ? true : false;
            //字母
            var en = v.match(/[A-Za-z]/g) ? true : false;
            //特殊符号
            var sign = v.match(/\W/g) ? true : false;
            //true 通过
            if (len) {
                if (num) {
                    if (en || sign) {
                        return true;
                    } else {
                        return false;
                    }
                } else if (en && sign) {
                    return true;
                } else {
                    return false;
                }
            } else {
                return false;
            }
        }

    }]);