var settings=module.controller('settingsCtrl', function ($scope, $rootScope, $state,$ionicHistory) {

    $scope.init = function () {
   $scope.isLanguages=false;
        //显示控制器
        $scope.$on('$ionicView.enter', function () {
            $rootScope.hideTabs = false;
        });
        $scope.show = false;//控制标题栏是否显示
        $scope.transrate = 0;//控制标题栏透明度

        $scope.initSettings();//系统设置
        $scope.initLanguages();//语言设置

    };
    /*********系统设置***************/
    $scope.initSettings = function () {
        //设置页面 返回
        $scope.goback = function () {
          $ionicHistory.goBack();
        };
        $scope.logout = function () {
            $state.go("login");
        };
        //设置页面 进入语言选项
        $scope.goLanguage = function () {
            $state.go("language");
        };

    }
      /*********语言切换***************/
    $scope.initLanguages = function () {

         $scope.languages = [
                { text: "中文", value: "中文" },
                { text: "English", value: "English" }
            ];

        //语言页面 完成  返回
        $scope.gobackSetting = function () {
           $scope.isLanguages= false;
            $state.go("settings");
        };
         $scope.changelanguages = function () {
            $scope.isLanguages= !$scope.isLanguages;
            console.log($scope.isLanguages);
        };
    }


    $scope.init();
});
