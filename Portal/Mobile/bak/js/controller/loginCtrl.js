module.controller('loginCtrl', function ($scope,$rootScope,$ionicHistory,$state,commonJS,login,focus) {

    $scope.init = function () {
        //显示控制器
        $scope.$on('$ionicView.enter',function(){
            $rootScope.hideTabs=false;
            console.log("ionicView.enter");
        });
        $scope.show=false;//控制标题栏是否显示
        $scope.transrate=0;//控制标题栏透明度
    };
    //登录页面查看密码
    $scope.showPassword=false;
    $scope.togglePassword = function ($event) {
      $scope.showPassword=!$scope.showPassword;
    }
    //跳转settings
    $scope.goSettings=function(){
      $state.go('settings');
    }
    //登录逻辑
    $scope.user={};
    $scope.validateUser = function (submitLogin) {
      if (!$scope.user.Code) {
        focus("userCode");
        commonJS.showShortTop("<span class=\"setcommon f15\">请输入账号！</span>");
        return;
      }
      if (!$scope.user.Password) {
        focus("userPassword");
        commonJS.showShortTop("<span class=\"setcommon f15\">请输入密码！</span>");
        return;
      }
      if (window.cordova) {
        if ($scope.user.MobileToken || $scope.user.Password) {
          //某些手机在启动时获取不到JPushID
          if ($scope.clientInfo.JPushID == "") {
            if (window.plugins && window.plugins.jPushPlugin) {
              window.plugins.jPushPlugin.getRegistrationID(function (id) {
                $scope.clientInfo.JPushID = id;
              });
            }
          }
          var pwd = $scope.user.Password.replace(/&/g, "_38;_");
          url = $scope.setting.httpUrl + "/ValidateLogin?callback=JSON_CALLBACK";
          url += "&userCode=" + $scope.user.Code;
          url += "&password=" + encodeURIComponent(pwd);
          url += "&uuid=" + $scope.clientInfo.UUID;
          url += "&jpushId=" + $scope.clientInfo.JPushID;
          url += "&mobileToken=" + $scope.user.MobileToken;
          url += "&mobileType=" + $scope.clientInfo.Platform;
          url += "&isAppLogin=true";
        }
      }
      else {//微信，钉钉登录，浏览器
        if ($scope.user.Code && $scope.user.Password) {
          commonJS.loadingShow();
          options = {
            userCode: $scope.user.Code,
            password: $scope.user.Password,
            rendom: new Date().getTime()
          };
          login.browser(options).then(function(result){
            commonJS.loadingHide();
            console.log(result);
            if(result.Success==true){
              //登录成功

              $ionicHistory.clearCache();
              $ionicHistory.clearHistory();
              config.portalroot = result.PortalRoot.toLocaleLowerCase();
              $scope.user.ObjectID = result.User.ObjectID;
              $scope.user.Code = result.User.Code;
              $scope.user.Name = result.User.Name;
              $scope.user.MobileToken = result.User.MobileToken;
              if (result.User.ImageUrl.indexOf("/user") > -1) {
                result.User.ImageUrl = "";
              }
              $scope.user.ImageUrl = result.User.ImageUrl == "" ? "" : $scope.setting.tempImageUrl + result.User.ImageUrl;
              $scope.user.Email = result.User.Email;
              $scope.user.DepartmentName = result.User.DepartmentName;
              $scope.user.OfficePhone = result.User.OfficePhone;
              $scope.user.Mobile = result.User.Mobile;
              $scope.user.WeChat = result.User.WeChat;
              $scope.user.Appellation = result.User.Appellation;
              // 存储最近一次登录的用户信息
              window.localStorage.setItem("OThinker.H3.Mobile.User", JSON.stringify($scope.user));
              $scope.user.Password = "";
              // 登录成功，转向主页面
              $rootScope.$broadcast("LoginIn", "");
              $state.go("tab.home");


            }else {
              //登录失败
              commonJS.showShortTop("<span class=\"setcommon f15\">账号或密码不对！</span>");
            }
          },function(reason){
            commonJS.loadingHide();
            commonJS.showShortTop("<span class=\"setcommon f15\">"+reason+"</span>");
          })
        };
      }
    };
    $scope.init();
});
