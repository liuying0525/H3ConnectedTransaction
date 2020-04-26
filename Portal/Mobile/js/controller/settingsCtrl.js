var settings = module.controller('settingsCtrl', function ($scope, $rootScope, $state, $ionicHistory, logout, $ionicLoading) {
    $scope.$on("$ionicView.enter", function (scopes, states) {
        $rootScope.hideTabs = true;
        $ionicHistory.clearCache();
        //设置钉钉头部
        if ($rootScope.dingMobile.isDingMobile) {
            $scope.SetDingDingHeader($rootScope.languages.login.systemSetting);
            dd.biz.navigation.setRight({
                show: true,//控制按钮显示， true 显示， false 隐藏， 默认true
                control: true,//是否控制点击事件，true 控制，false 不控制， 默认false
                text: $rootScope.languages.confirm,//控制显示文本，空字符串表示显示默认文本
                onSuccess: function (result) {
                    $scope.setIP();
                },
                onFail: function (err) { }
            });
        }
        /*********设置ip***************/
        $scope.settingIP = window.localStorage.getItem('OThinker.H3.Mobile.setIP') || '127.0.0.1';
        window.localStorage.setItem('OThinker.H3.Mobile.setIP', $scope.settingIP);
    });
    $scope.init = function () {
        $scope.initSettings();//系统设置
        $scope.initIP();//设置IP
        $scope.hideOther = $rootScope.dingMobile.hideOther;//区别钉钉微信隐藏退出按钮
    };
    /*********系统设置***************/
    $scope.initSettings = function () {
        //设置页面 返回
        $scope.goback = function () {
          $ionicHistory.goBack();
        };
        $scope.logout = function () {
            $scope.$parent.setting.autoLogin = false;
            window.localStorage.setItem('OThinker.H3.Mobile.Setting', JSON.stringify($scope.$parent.setting));
            logout.browser().then(function (result) {
                $state.go("login");
            }, function (reanson) {
                commonJS.showShortTop("<span class=\"setcommon f15\">" + reason + "</span>");
                $state.go("login");
            });
        };
        //设置页面 进入语言选项
        $scope.goLanguage = function () {
            $state.go("language");
        };
    }
    /*********设置ip***************/
    $scope.initIP = function () {
        $scope.settingIP = window.localStorage.getItem('OThinker.H3.Mobile.setIP') || '127.0.0.1';
        window.localStorage.setItem('OThinker.H3.Mobile.setIP', $scope.settingIP);
        $scope.validateIp = function () {
            var ip = $scope.settingIP;
            var tmp = ip.split('.');
            var flag = true;
            if (tmp.length != 4) { flag = false; }
            tmp.forEach(function (v, i) {
                if (v=='') {
                    flag = false;
                }
                if(isNaN(Number(v))==true){
                    flag = false;
                }
                if (Number(v) > 255 || Number(v) < 0) {
                    flag = false;
                }
            });
            return flag;
        };
        $scope.setIP = function () {
            if (!$scope.validateIp()) {
                $ionicLoading.show({
                    template: '<span class="setcommon f15">' + $rootScope.languages.ipError   + '</span>',
                    duration: 2 * 1000,
                    animation: 'fade-in',
                    showBackdrop: false,
                });
                return;
            }
            window.localStorage.setItem('OThinker.H3.Mobile.setIP', $scope.settingIP);
            $ionicLoading.show({
                template: '<span class="setcommon f15">' + $rootScope.languages.setSuccess + '</span>',
                duration: 2 * 1000,
                animation: 'fade-in',
                showBackdrop: false,
            });
        };
    }
    $scope.init();
});
