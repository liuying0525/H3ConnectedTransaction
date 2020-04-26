'use strict';

/* 
    Controllers 
*/

angular.module('app')
  .controller('AppCtrl', ['$rootScope', '$scope', '$translate', '$localStorage', '$window', '$interval',
      '$http', '$state', '$stateParams', '$location', '$document', '$modal', 'ControllerConfig', '$filter', '$compile', '$timeout', 'menuhandle',
 function ($rootScope, $scope, $translate, $localStorage, $window, $interval, $http, $state, $stateParams, $location, $document, $modal, ControllerConfig, $filter, $compile, $timeout, menuhandle) {
     var isIE = !!navigator.userAgent.match(/MSIE/i);
     isIE && angular.element($window.document.body).addClass('ie');
     $scope.password = "";
     // config
     $scope.app = {
         name: "H3 BPM",
         version: '1.0.0',
         locked: false,
         logoSimple: "img/H1.jpg",
         logoFull: "img/logo.jpg",
         // 颜色定义
         color: {
             primary: '#7266ba',
             info: '#23b7e5',
             success: '#27c24c',
             warning: '#fad733',
             danger: '#f05050',
             light: '#e8eff0',
             dark: '#3a3f51',
             black: '#1c2b36'
         },
         // 页面显示定义
         settings: {
             themeID: 8,
             navbarHeaderColor: 'bg-info dker',
             navbarCollapseColor: 'bg-info dker',
             asideColor: 'bg-light dker b-r',
             headerFixed: true,
             asideFixed: true,
             asideFolded: false,
             asideDock: false,
             container: false,
             allowSetting: false,  // 是否可配置选项
             autoLogin: false      // 是否允许
         }
     }

     // 设置信息本地存储
     if (angular.isDefined($localStorage.settings)) {
         $scope.app.settings = $localStorage.settings;
     }
     else {
         $localStorage.settings = $scope.app.settings;
     }

     $scope.$watch('app.settings', function () {
         if ($scope.app.settings.asideDock && $scope.app.settings.asideFixed) {
             $scope.app.settings.headerFixed = true;
         }
         $localStorage.settings = $scope.app.settings;
     }, true);

     // 设置语言
     $scope.setLang = function (langKey, $event) {
         // set the current lang
         $scope.selectLang = $scope.langs[langKey];
         // You can change the language during runtime
         $translate.use(langKey);
         $scope.lang.isopen = !$scope.lang.isopen;
         setItem(langKey);
         window.sessionStorage.removeItem("LanguageData");
     };

     // 获取是否是手机客户端访问
     $scope.isSmartDevice = function ($window) {
         var ua = $window['navigator']['userAgent'] || $window['navigator']['vendor'] || $window['opera'];
         return (/iPhone|iPod|iPad|Silk|Android|BlackBerry|Opera Mini|IEMobile/).test(ua);
     }

     $scope.getUrlParam = function (name) {
         var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
         var r = window.location.search.substr(1).match(reg);
         if (r != null) return unescape(r[2]); return null;
     };

     //锁定
     $scope.doLock = function () {
         $scope.app.locked = true;
         $scope.LoginSuccess = true;
         //状态记录到服务器
         $http({
             url: "Organization/DoLock",
             params: {
             }
         })
         .success(function (data) {
             $scope.user.DoLock = true;
         })
     }

     // 解锁
     $scope.doUnlock = function () {
         var password = $("#password").val();
         $http({
             url: "Organization/DoUnlock",
             params: {
                 password: password
             }
         }).success(function (data) {
             if (data) {
                 $("#password").val("");
                 $scope.app.locked = false;
                 $scope.user.DoLock = false;
                 $scope.LoginSuccess = true;
             } else {
                 $scope.LoginSuccess = false;
             }
         }).error(function () {
         })
     }
     //切换模式,设计、浏览
     $scope.StartEdit = function () {
         if ($stateParams.Mode == "Design") {
             $state.go($state.$current.self.name, { OT_EditorModel: false, Mode: "" }, { reload: true });
             return;
         }
         if (typeof ($scope.OT_EditorModel) == "undefined" || !$scope.OT_EditorModel) {
             $state.go($state.$current.self.name, { OT_EditorModel: true, Mode: "Design" }, { reload: true });
         } else {
             $state.go($state.$current.self.name, { OT_EditorModel: false, Mode: "" }, { reload: true });
         }
     }

     //刷新
     $scope.refresh = function () {
         var Token = $scope.getUrlParam("Token");
         // 获取当前用户
         $http({
             url: ControllerConfig.Organization.GetCurrentUser,
             params: {
                 random: new Date().getTime(),
                 Token: Token
             }
         })
         .success(function (result, header, config, status) {
             if (!result.Success) {
                 $state.go("platform.login");
             }
             else {
                 $scope.user = result.User;
                 $scope.user.ManagerName = result.ManagerName;
                 $scope.user.OUDepartName = result.OUDepartName;
                 $scope.user.chkEmail = result.chkEmail;
                 $scope.user.chkMobileMessage = result.chkMobileMessage;
                 $scope.user.chkWeChat = result.chkWeChat;
                 $scope.user.chkApp = result.chkApp;
                 $scope.user.chkDingTalk = result.chkDingTalk;
                 $scope.user.FunctionViewModels = result.Functions;
                 $scope.user.ImageUrl = $scope.user.ImageUrl + "?" + $filter("date")(new Date(), "yyyyMMddHHmmss");
                 $scope.ShowMenu();
                 //debugger
                 var rUrl = $scope.getUrlParam("RedirectUrl");
                 if (rUrl) {
                     window.location.href = rUrl;
                 } else if (Token) {
                     var rUrl = window.location.href.replace(window.location.search, "")
                     var index = rUrl.indexOf("#/");
                     rUrl = rUrl.substring(0, index);
                     window.location.href = rUrl + "#/app/Workflow/MyUnfinishedWorkItem";
                 }
             }
         })
         .error(function (data, header, config, status) {
             $state.go("platform.login");
         });
     }
     $scope.refresh();

     //获取待办、待阅、我的流程数量
     $scope.GetItemCount = function () {
         var random = new Date().getTime();
         $http({
             cache: false,
             url: ControllerConfig.WorkItem.GetWorkCount,
             params: {
                 random: random
             }
         })
         .success(function (result, header, config, status) {
             if (result.Success == false && result.ExceptionCode == 1) {
                 //登陆超时
                 $scope.unregisterAutoRefresh();
                 $state.go("platform.login");
             } else {
                 $scope.MyCount = result.Extend;
             }
         })
     }
     //取消自动刷新
     $scope.unregisterAutoRefresh = function () {
         if ($scope.autoRefresh) {
             $interval.cancel($scope.autoRefresh);
         }
     }
     //注册自动刷新
     $scope.registerAutoRefresh = function () {
         $scope.autoRefresh = $interval(function () {
             $scope.GetItemCount();
         }, 60 * 1000);
     }

     // 退出系统
     $scope.loginOut = function () {
         $http({
             url: ControllerConfig.Organization.LoginOut,
             params: {
                 rendom: new Date().getTime()
             }
         })
         .success(function (result, header, config, status) {
             $scope.user = null;
             $state.go("platform.login");
         })
         .error(function (data, header, config, status) {
             $state.go("platform.login");
         });
     }

     // 每次进入View时触发
     $scope.$on('$viewContentLoaded', function (event) {
         $.notify.closeAll();//关闭所有弹窗
         if ($state.current.name != "" && $state.current.name.indexOf("platform.login") == -1) {
             $scope.GetItemCount();
         } else {
             $scope.unregisterAutoRefresh();
         }
         // 关闭表单
         $(".app-aside-right").removeClass("show");

         //锁定
         if ($scope.user && $scope.user.DoLock) {
             $scope.app.locked = true;
         }
     });

     // 登录事件，由LoginController触发
     $scope.$on("LoginIn", function (event, args) {
         if (args.Success) {
             $scope.Name = $translate.instant("HomePage.Workspace_MyUnfinishedWorkItem");
             $scope.user = args.User;
             $scope.user.ManagerName = args.ManagerName;
             $scope.user.OUDepartName = args.OUDepartName;
             $scope.user.chkEmail = args.chkEmail;
             $scope.user.chkMobileMessage = args.chkMobileMessage;
             $scope.user.chkWeChat = args.chkWeChat;
             $scope.user.chkApp = args.chkApp;
             $scope.user.chkDingTalk = args.chkDingTalk;
             $scope.user.ImageUrl = $scope.user.ImageUrl + "?" + $filter("date")(new Date(), "yyyyMMddHHmmss");
             $scope.user.FunctionViewModels = args.Functions;
             // 记录当前PortalRoot的路径
             window.localStorage.setItem("H3.PortalRoot", args.PortalRoot);
             // 隐藏当前窗体
             if (window.parent && window.parent.hideLogin) {
                 window.parent.hideLogin();
             }
             $scope.user.Password = "";
             //注册自动刷新
             $scope.registerAutoRefresh();
         }
     });


     $scope.$on("$stateChangeStart", function (event, toState, toParams, fromState, fromParams) {

     });
     $scope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
         $scope.ShowMenu();
     });
     //显示菜单
     $scope.ShowMenu = function () {
         if ($scope.user) {
             //重新定义菜单显示逻辑
             if ($scope.menuIndex) { }
             else {
                 $scope.menuIndex = menuhandle.getMenuIndex($scope.user.FunctionViewModels);
             }
             $scope.menuData = menuhandle.getAsideMenus($scope.user.FunctionViewModels, $scope.menuIndex);
         }
     }


     // 注册整个文档点击事件，关闭表单
     $document.on("click", function (event) {
         //非待办发起
         if ($scope.$state.current.name != "app.MyUnfinishedWorkItem"
             && $scope.$state.current.name != "app.MyWorkflow"
             && $scope.$state.current.name != "app.MyUnfinishedWorkItemByGroup"
             && $scope.$state.current.name != "app.MyUnfinishedWorkItemByBatch") {
             $scope.ClosePage();
             return;
         }

         //已经保存
         if ($scope.IsSave == true || $(event.target).parents("ul").hasClass("nav")) {
             $scope.ClosePage();
             return;
         }

         //点击收起左侧菜单时，不关闭表单
         if ($(event.target).hasClass("asideFolded")
             || $(event.target).hasClass("fa-dedent")
             || $(event.target).hasClass("fa-dedent-add")) {
             return;
         }

         if ($(event.target).parents("form").length == 1) {
             return;
         }


         if (!$(event.target).attr("target") || $(event.target).attr("target") != ".app-aside-right") {
             var targeturl = $(".app-aside-right").find("iframe").attr("src");
             if (targeturl && targeturl.indexOf("InstanceDetail") == -1) {
                 event.stopPropagation();
                 event.preventDefault();
                 // 弹出模态框
                 var modalInstance = $modal.open({
                     templateUrl: 'template/ProcessCenter/ConfirmModal.html',
                     size: "sm",
                     controller: function ($scope, $modalInstance) {
                         $scope.Title = $translate.instant("WarnOfNotMetCondition.Tips");
                         $scope.Message = $translate.instant("msgGlobalString.ConfirmLeave");
                         $scope.Button_OK = true;
                         $scope.Button_OK_Text = $translate.instant("QueryTableColumn.Button_OK");
                         $scope.Button_Cancel = true;
                         $scope.Button_Cancel_Text = $translate.instant("QueryTableColumn.Button_Cancel");
                         $scope.ok = function () {
                             $modalInstance.close();  // 点击确定按钮
                         };
                         $scope.cancel = function () {
                             $modalInstance.dismiss('cancel'); // 退出
                         }
                     }
                 });
                 //弹窗点击确定的回调事件
                 modalInstance.result.then(function () {
                     $scope.ClosePage();
                 });
                 return;
             }
         }
         $scope.ClosePage();
     });

     $scope.ClosePage = function () {
         $(".app-aside-right").find("iframe").attr("src", "");
         $(".app-aside-right").removeClass("show");
         $scope.IsSave = false;
     }

     $scope.modalDialogs = [];

     // 多语言设置
     $scope.lang = { isopen: false };
     $scope.langs = { zh_CN: '中文', en: 'English' };
     $scope.selectLang = $scope.langs[$translate.proposedLanguage()] || "中文";
     var setItem = function (langKey) {
         if (!langKey) {
             if ($scope.selectLang == "中文") {
                 langKey = "zh_CN";
             }
         }
         if (langKey == "zh_CN") {
             window.localStorage.setItem("H3.Language", "zh_cn");
         } else {
             window.localStorage.setItem("H3.Language", "en_us");
         }
     }
     setItem();
     // 获取当前是否移动端访问
     $scope.isSmartDevice($window) && angular.element($window.document.body).addClass('smart');

     // 打开个人信息
     $scope.showUserInfoModal = function (userId) {
         //关闭流程状态图的div
         angular.element("div[class='ActivitySummary']").hide();
         // 弹出模态框
         var modalInstance = $modal.open({
             templateUrl: "template/ProcessCenter/UserInfo.html",// 指向上面创建的视图
             controller: 'UserInfoModalController',// 初始化模态范围
             size: "lg",
             resolve: {
                 params: function () {
                     return {
                         userId: userId,
                         user: $scope.user
                     };
                 },
                 deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                     return $ocLazyLoad.load([
                         'WFRes/_Scripts/jquery/ajaxfileupload.js',
                         'js/factory/file-reader.js'
                     ]);
                 }]
             }
         });
         // 弹窗点击确定的回调事件
         modalInstance.result.then(function (arg) {
             if (arg.Success && arg.Extend && arg.Extend.ImageUrl) {
                 $scope.user.ImageUrl = arg.Extend.ImageUrl + "?" + $filter("date")(new Date(), "yyyyMMddHHmmss");
             }
         });
     }
     // 编辑密码
     $scope.showUserPasswordModal = function () {
         var modalInstance = $modal.open({
             templateUrl: 'template/ProcessCenter/UserPassword.html',         // 指向上面创建的视图
             controller: 'UserPasswordModalController', // 初始化模态范围             
         });
         // 弹窗点击确定的回调事件
         modalInstance.result.then(function () {
             // 弹出模态框
             var modalInstance = $modal.open({
                 templateUrl: 'template/ProcessCenter/ConfirmModal.html',
                 size: "sm",
                 backdrop: "static",
                 keyboard: false,
                 controller: function ($scope, $modalInstance) {
                     $scope.Title = $translate.instant("WarnOfNotMetCondition.Tips");
                     $scope.Message = $translate.instant("LoginLog.ResetPasswordSuccess");
                     $scope.Button_OK = true;
                     $scope.Button_OK_Text = $translate.instant("QueryTableColumn.Button_OK");
                     $scope.ok = function () {
                         $modalInstance.close();  // 点击确定按钮
                     };
                 }
             });
         });
     }
     //查看任务催办信息
     $scope.showUrgeWorkItemInfoModal = function (WorkItemID) {
         var modalInstance = $modal.open({
             templateUrl: "template/ProcessCenter/UrgeWorkItemInfo.html",
             controller: "UrgeWorkItemInfo",
             size: "lg",
             resolve: {
                 params: function () {
                     return {
                         WorkItemID: WorkItemID
                     };
                 }
             }
         });
     }

     //页面与iframe消息传递
     window.addEventListener('message', function (event) {
         var msg = event.data.toString();
         if (msg.indexOf("TotalCount") > -1) {
             $scope.GetItemCount();
         } else if (msg.indexOf("ClosePage") > -1) {
             $scope.ClosePage();
             $scope.GetItemCount();
             //页面刷新,待办任务列表，不刷新，点击搜索按钮，其它页面还是按前逻辑
             if ($state.$current.self.name == "app.MyUnfinishedWorkItem") {
                 $(".searchContainer").find("button").click();
             }
             else {
                 $state.go($state.$current.self.name, {}, { reload: true });
             }
         } else if (msg.indexOf("IsSave") > -1) {
             $scope.IsSave = true;
         } else if (msg.indexOf("showUserInfoModal") > -1) {
             var id = msg.split(":")[1];
             $scope.showUserInfoModal(id);
         } else if (msg.indexOf("ParentReload") > -1) {
             $timeout(function () {
                 top.window.location.reload();
             }, 1000 * 2);
         }
     })
 }]);

// 用户信息Controller
app.controller('UserInfoModalController', ['$scope', '$http', '$state', '$modalInstance', '$filter', '$timeout', 'datecalculation', 'ControllerConfig', 'fileReader', 'params',
    function ($scope, $http, $state, $modalInstance, $filter, $timeout, datecalculation, ControllerConfig, FileReader, params) {
        // TODO:通过 userId 获取指定的用户信息，如果是当前用户，则直接获取 $scope.user 对象
        if (params.userId == params.user.ObjectID) {
            $scope.editAble = true; // 获取当前用户是否有权限编辑和保存 
            $scope.user = params.user;
            $scope.user.BirthdayFormat = datecalculation.changeDateFormat($scope.user.Birthday);
            $scope.imageSrc = $scope.user.ImageUrl + "?" + $filter("date")(new Date(), "yyyyMMddHHmmss");
        } else {
            $scope.editAble = false;
            $http({
                url: ControllerConfig.PersonalInfo.GetUserInfo,
                params: {
                    UserID: params.userId
                }
            })
            .success(function (result, header, config, status) {
                $scope.user = result.User;
                $scope.user.ManagerName = result.ManagerName;
                $scope.user.OUDepartName = result.OUDepartName;
                $scope.user.chkEmail = result.chkEmail;
                $scope.user.chkMobileMessage = result.chkMobileMessage;
                $scope.user.chkWeChat = result.chkWeChat;
                $scope.user.chkApp = result.chkApp;
                $scope.user.chkDingTalk = result.chkDingTalk;
                $scope.user.BirthdayFormat = datecalculation.changeDateFormat($scope.user.Birthday);
                $scope.imageSrc = $scope.user.ImageUrl + "?" + $filter("date")(new Date(), "yyyyMMddHHmmss");
            })
            .error(function (data, header, config, status) {
                return;
            });
        }

        //用户图片预览
        $scope.getFile = function () {
            FileReader.readAsDataUrl($scope.file, $scope)
            .then(function (result) {
                $scope.imageSrc = result;
            });
        }
        $scope.ok = function () {
            // TODO：如果是当前用户有权限编辑，那么保存至数据库
            var params = {
                UserID: $scope.user.ObjectID,
                Mobile: $scope.user.Mobile,
                OfficePhone: $scope.user.OfficePhone,
                Email: $scope.user.Email,
                FacsimileTelephoneNumber: $scope.user.FacsimileTelephoneNumber,
                chkEmail: $scope.user.chkEmail,
                chkApp: $scope.user.chkApp,
                chkWeChat: $scope.user.chkWeChat,
                chkMobileMessage: $scope.user.chkMobileMessage,
                chkDingTalk: $scope.user.chkDingTalk,
            }
            $scope.saveData(params);
        };

        $scope.saveData = function (params) {
            $.ajaxFileUpload({
                url: ControllerConfig.PersonalInfo.UpdateUserInfo,
                fileElementId: "file",
                secureuri: false,
                type: "post",
                data: params,
                dataType: 'json',
                async: false,
                success: function (result) {
                    //modify by chenghs 2018-02-24
                    if (result.Success)
                        $modalInstance.close(result);
                    else {
                        $scope.user.Mobile = "";
                        alert(result.Message);
                    }
                }
            });
        }
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel'); // 退出
        };
    }]);

// 编辑密码Controller
app.controller('UserPasswordModalController', ['$scope', '$http', '$interval', '$modalInstance', '$modal', 'ControllerConfig', function ($scope, $http, $interval, $modalInstance, $modal, ControllerConfig) {
    $scope.editResult = true;
    $scope.ok = function () {
        // TODO：更新密码
        if ($scope.NewPassword != $scope.ConfirmPassword) {
            $scope.ShowMsg2 = true;
            return;
        }
        $http({
            url: ControllerConfig.PersonalInfo.SetPassword,
            params: {
                OldPassword: $scope.OldPassword,
                NewPassword: $scope.NewPassword,
            }
        })
        .success(function (result, header, config, status) {
            if (result) {
                $modalInstance.close();  // 点击保存按钮
            } else {
                $scope.editResult = false;
                if (!$scope.editResult) {
                    $interval(function () {
                        $scope.editResult = true;
                    }, 1000);
                }
            }
        })
        .error(function (data, header, config, status) { })
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel'); // 退出
    }
}]);

//任务催办信息Controller
app.controller('UrgeWorkItemInfo', ['$scope', '$http', '$modalInstance', 'ControllerConfig', 'params',
function ($scope, $http, $modalInstance, ControllerConfig, params) {
    $scope.WorkItemID = params.WorkItemID;
    $http({
        url: ControllerConfig.InstanceDetail.GetUrgeWorkItemInfo,
        params: {
            WorkItemID: $scope.WorkItemID
        }
    })
    .success(function (result, header, config, status) {
        $scope.UrgeWorkItemInfo = result;
    })
    .error(function (data, header, config, status) {
        return;
    });
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel'); // 退出
    }
}]);

document.write("<script src='/Portal/WFRes/_Scripts/showError.js'></script>"); 

// 19.7.1 wangxg 测试 ：在jQuery ajax的http请求之前，在URL参数中添加FunctionCode参数
$.ajaxSetup({
    //complete: function (xhr, status) {
    //    if (xhr.status === 800) {
    //        xhr.responseText = '您提交的内容检测到SQL攻击信息';
    //    } else if (xhr.status === 801) {
    //        xhr.responseText = '检测到敏感符号';
    //    } else if (xhr.status === 802) {
    //        xhr.responseText = '权限不足';
    //    }
    //}
    //beforeSend: function (jqXHR, settings) {
    //    var functionCode = "";
    //    var seg = document.URL.split("#");
    //    if (seg.length > 1) {
    //        var relUrl = seg[1];
    //        var tem = relUrl.split("/");
    //        if (tem.length > 3) {
    //            functionCode = tem[3];
    //        }
    //    }
    //    //在请求前给修改url（增加一个功能代码参数，用于后台权限确认）
    //    settings.url += settings.url.match(/\?/) ? "&" : "?";
    //    settings.url += "FunctionCode=" + functionCode;
    //},
    //error: function (jqXHR, textStatus, errorThrown) {
    //    var status = '';
    //    switch (jqXHR.status) {
    //        case (800):
    //            status = '您提交的内容检测到SQL攻击信息';
    //            break;
    //        case (801):
    //            status = '检测到敏感符号';
    //            break;
    //        case (802):
    //            status = '权限不足';
    //            break;
    //        default:
    //            status = '';
    //        //TODO 未知错误
    //    }
    //    if (status !== '') {
    //        alert(status);
    //    }
            
    //}
});	

