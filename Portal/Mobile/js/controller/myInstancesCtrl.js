module.controller('myInstancesCtrl', function ($scope, $rootScope, instances, $ionicSlideBoxDelegate, $ionicModal, commonJS, $ionicScrollDelegate, $http) {
    $scope.filter = {};//当前搜索的字段
    $scope.searchKeyArry = [];//记住三个部门的前一个字段
    $scope.searchItemShow = false;
    $scope.$on("$ionicView.enter", function (scopes, states) {
        commonJS.loadingShow();
        $scope.init();
        //加载我的实例
        $scope.loadAllData();
        //打开筛选页
        commonJS.sideSlip($scope, 'templates/filter.html');
        //设置钉钉头部
        if ($rootScope.dingMobile.isDingMobile) {
            $scope.SetDingDingHeader($rootScope.languages.tabs.myProcess);
            dd.biz.navigation.setRight({
                show: true,//控制按钮显示， true 显示， false 隐藏， 默认true
                control: true,//是否控制点击事件，true 控制，false 不控制， 默认false
                text: $rootScope.languages.filter,//控制显示文本，空字符串表示显示默认文本
                onSuccess: function (result) {
                    $scope.openPopover();
                },
                onFail: function (err) { }
            });
        }
    });
    //入口初始化程序，页面初次加载的事件
    $scope.init = function () {
        /*sousu*/
        $scope.tabNames = $rootScope.languages.tabMyProcess.tab;
        $scope.slectIndex = 0;
        /*处理微信的表单返回*/
        if ($rootScope.loginInfo.loginfrom == "wechat" && $scope.JumpParams.tab) {
            $scope.slectIndex = $scope.JumpParams.tab;
            $scope.renderJumpParams();
        }
        $scope.unfinishNum = 0;
        $scope.searchKey = '';
        $scope.sampleData = false;//是否存在样列数据
        //控制隐藏，解决side-boxs的隐形bug
        $scope.finishedComplete = false;
        $scope.unfinishedComplete = false;
        $scope.cancelComplete = false;
        //搜索用到的变量
        $scope.searchIndex = 2;//区分搜索的页面
        $scope.searchfinishedBefore = [];//当前搜索的字段
        $scope.activeSlide($scope.slectIndex);
    };
    /*点击切换副标题*/
    $scope.activeSlide = function (index) {//点击时候触发
        $scope.slectIndex = index;
        $ionicSlideBoxDelegate.slide(index);
        $scope.slideShow(index);

    };
    $scope.slideShow = function (index) {
        //控制隐藏，解决side-boxs的隐形bug
        switch (index) {
            case 1:
                $scope.searchIndex = 4;
                $scope.finishedComplete = true;
                $scope.unfinishedComplete = false;
                $scope.cancelComplete = false;
                break;
            case 2:
                $scope.searchIndex = 5;
                $scope.cancelComplete = true;
                $scope.finishedComplete = false;
                $scope.unfinishedComplete = false;
                break;
            default:
                $scope.searchIndex = 2;
                $scope.unfinishedComplete = true;
                $scope.finishedComplete = false;
                $scope.cancelComplete = false;
                break;
        }
    }
    /*滑动切换副标题*/
    $scope.slideChanged = function (index) {//滑动时候触发
        if ($scope.searchKey != "") {
            $scope.clearSearch(function () {
                $scope.slectIndex = index;
            });
        } else {
            $scope.slectIndex = index;
        }
        $scope.slideShow(index);
    };


    $scope.$watch("instances", function (newVal, oldVal) {
        if ($scope.instances)
            $scope.currentTab = $scope.getCurrentTab($scope.slectIndex);
    }, true);
    $scope.$watch("slectIndex", function (newVal, oldVal) {
        // 滚动到顶部
        $ionicScrollDelegate.scrollTop(true);
        $scope.filter = $scope.searchKeyArry[$scope.slectIndex] || {};//重置搜索的字段
        if ($scope.instances) {
            $scope.currentTab = $scope.getCurrentTab(newVal);
        }
    });
    //加载面板信息
    $scope.loadAllData = function () {
        var url = "";
        var params = null;
        if (window.cordova) {
            url = $scope.setting.httpUrl + "/LoadAllInstances?callback=JSON_CALLBACK&userId=" + $scope.user.ObjectID;
            url += "&mobileToken=" + $scope.user.MobileToken;
        }
        else {
            url = $scope.setting.httpUrl + "/Mobile/LoadAllInstances";
            params = {
                userId: $scope.user.ObjectID,
                mobileToken: $scope.user.MobileToken
            };
        }
        instances.all(url, params).then(function (result) {
            console.log(url);
            console.log('result.Extend');
            console.log(result.Extend);
            if (result.Success) {
                $scope.instances = result.Extend;
                $scope.unfinishNum = $scope.instances.unfinished.TotalCount;
                //记录为搜索前的数据
                $scope.searchfinishedBefore[0] = result.Extend.unfinished;
                $scope.searchfinishedBefore[1] = result.Extend.finished;
                $scope.searchfinishedBefore[2] = result.Extend.cancel;
                $scope.$broadcast('scroll.infiniteScrollComplete');
            } else {
                commonJS.TimeoutHandler();
            }

        }).finally(function () {
            commonJS.loadingHide();
        });
    };


    //获取当前的Tab
    $scope.getCurrentTab = function (tab) {
        switch (tab) {
            case 0:
                return $scope.instances.unfinished;
                break;
            case 1:
                return $scope.instances.finished;
                break;
            case 2:
                return $scope.instances.cancel;
                break;
        }
    }
    $scope.loadMore = function (callback) {
        if ($scope.currentTab) {
            console.log("当前数据。。。" + $scope.currentTab.WorkItems.length);
            console.log($scope.currentTab);
            commonJS.loadingShow();
            var url = "";
            var params = null;
            if (window.cordova) {
                url = $scope.setting.httpUrl + "/LoadAllInstances?callback=JSON_CALLBACK&userId=" + $scope.user.ObjectID;
                url += "&mobileToken=" + $scope.user.MobileToken;
                url += "&keyWord=" + encodeURI($scope.searchKey);
                url += "&loadStart=" + $scope.currentTab.WorkItems.length;
               // url += "&lastTime=" + commonJS.getDateFromJOSN($scope.currentTab.LastTime).Format("yyyy-MM-dd HH:mm:ss");
                url += "&instanceState=" + $scope.currentTab.InstanceState;
                //搜索
                url += "&keyWord=" +$scope.existsParameter("keyWord", $scope.searchKeyArry[$scope.slectIndex]);
                url += "&IsPriority=" +$scope.existsParameter("IsPriority", $scope.searchKeyArry[$scope.slectIndex]);
                url += "&Status=" + $scope.searchIndex;
                url += "&startDate=" + $scope.existsParameter("startDate", $scope.searchKeyArry[$scope.slectIndex]);
                url += "&endDate=" + $scope.existsParameter("endDate", $scope.searchKeyArry[$scope.slectIndex]);
            }
            else {
                url = $scope.setting.httpUrl + "/Mobile/LoadAllInstances";
                params = {
                    userId: $scope.user.ObjectID,
                    mobileToken: $scope.user.MobileToken,
                    keyWord: encodeURI($scope.searchKey),
                    loadStart: $scope.currentTab.WorkItems.length,
                   // lastTime: commonJS.getDateFromJOSN($scope.currentTab.LastTime).Format("yyyy-MM-dd HH:mm:ss"),
                    instanceState: $scope.currentTab.InstanceState,
                    keyWord: $scope.existsParameter("keyWord", $scope.searchKeyArry[$scope.slectIndex]),//流程名称
                    IsPriority: $scope.existsParameter("IsPriority", $scope.searchKeyArry[$scope.slectIndex]),//2:加急，0：不加。空：不限
                    Status: $scope.searchIndex,//2,代办，4已完成，5取消
                    startDate: $scope.existsParameter("startDate", $scope.searchKeyArry[$scope.slectIndex]),//开始时间
                    endDate: $scope.existsParameter("endDate", $scope.searchKeyArry[$scope.slectIndex]),//开始时间
                };
            }
            instances.refreshWorkItems(url, params).then(function (result) {
                if (result.Success) {
                    //更新对象数据
                    $scope.updateInstances(result.Extend, "load");
                    commonJS.loadingHide();
                }
                $scope.$broadcast('scroll.infiniteScrollComplete');
            })
        };
    };


    //下拉刷新
    $scope.doRefresh = function () {
      //  $scope.LastTime = new Date();
        var url = "";
        var params = null;
        if (window.cordova) {
            url = $scope.setting.httpUrl + "/LoadAllInstances?callback=JSON_CALLBACK&userId=" + $scope.user.ObjectID;
            url += "&mobileToken=" + $scope.user.MobileToken;
            url += "&keyWord=" + encodeURI($scope.searchKey);
            url += "&loadStart=" + $scope.currentTab.WorkItems.length;
           // url += "&lastTime=" + commonJS.getDateFromJOSN($scope.currentTab.LastTime).Format("yyyy-MM-dd HH:mm:ss");
            url += "&instanceState=" + $scope.currentTab.InstanceState;
            //搜索
            url += "&keyWord=" + $scope.existsParameter("keyWord", $scope.searchKeyArry[$scope.slectIndex]);
            url += "&IsPriority=" + $scope.existsParameter("IsPriority", $scope.searchKeyArry[$scope.slectIndex]);
            url += "&Status=" + $scope.searchIndex;
            url += "&startDate=" + $scope.existsParameter("startDate", $scope.searchKeyArry[$scope.slectIndex]);
            url += "&endDate=" + $scope.existsParameter("endDate", $scope.searchKeyArry[$scope.slectIndex]);
        }
        else {
            url = $scope.setting.httpUrl + "/Mobile/LoadAllInstances";
            params = {
                userId: $scope.user.ObjectID,
                mobileToken: $scope.user.MobileToken,
                keyWord: encodeURI($scope.searchKey),
                loadStart: $scope.currentTab.WorkItems.length,
               // lastTime: commonJS.getDateFromJOSN($scope.currentTab.LastTime).Format("yyyy-MM-dd HH:mm:ss"),
                instanceState: $scope.currentTab.InstanceState,
                keyWord: $scope.existsParameter("keyWord", $scope.searchKeyArry[$scope.slectIndex]),//流程名称
                IsPriority: $scope.existsParameter("IsPriority", $scope.searchKeyArry[$scope.slectIndex]),//2:加急，0：不加。空：不限
                Status: $scope.searchIndex,//2,代办，4已完成，5取消
                startDate: $scope.existsParameter("startDate", $scope.searchKeyArry[$scope.slectIndex]),//开始时间
                endDate: $scope.existsParameter("endDate", $scope.searchKeyArry[$scope.slectIndex]),//开始时间
            };
        }
        //刷新请求的数据
        instances.refreshWorkItems(url, params).then(function (result) {
            if (result.Success) {
                $scope.updateInstances(result.Extend, "refresh");
                commonJS.loadingHide();
                $scope.$broadcast('scroll.refreshComplete');
            }
        })
    };

    //更新数据,type:load/refresh
    $scope.updateInstances = function (obj, type) {
        switch ($scope.slectIndex) {
            case 0:
                $scope.unfinishNum = $scope.instances.unfinished.TotalCount;
                $scope.mergeData($scope.currentTab.WorkItems, $scope.instances.unfinished, obj.unfinished, type);
                break;
            case 1:
                $scope.mergeData($scope.currentTab.WorkItems, $scope.instances.finished, obj.finished, type);
                break;
            case 2:
                $scope.mergeData($scope.currentTab.WorkItems, $scope.instances.cancel, obj.cancel, type);
                break;
        }
    };
    /**
     * 合并数组中的数据,保证不重复
     *  @param {array} list   页面用于显示数据的临时数组
     *  @param {object} obj   每一个tab页对应的实例对象
     *  @param {object} ret   服务器返回的实例数据
     *  @param {string} type  加载方式
     *  @returns void
    **/
    $scope.mergeData = function (list, obj, ret, type) {
        if ($scope.searchKey != "") {
            list = [];
            obj.list = [];
        }
        if (type == "load") {
            obj.LoadComplete = ret.LoadComplete;
          //  obj.LastTime = ret.LastTime;
            if (ret.WorkItems && ret.WorkItems.length > 0) {
                for (var i = 0; i < ret.WorkItems.length; i++) {
                    if ($scope.existsInstance(list, ret.WorkItems[i])) continue;
                    obj.WorkItems.push(ret.WorkItems[i]);
                }
            }
        } else if (type == "refresh") {
          //  obj.RefreshTime = ret.RefreshTime;
            if (ret.WorkItems && ret.WorkItems.length > 0) {
                for (var i = ret.WorkItems.length - 1; i > -1; i--) {
                    if ($scope.existsInstance($scope.currentTab.WorkItems, ret.WorkItems[i])) continue;
                    obj.WorkItems.splice(0, 0, ret.WorkItems[i]);
                }
            }
        }
    };
    //判断是否已经包含该对象
    $scope.existsInstance = function (list, obj) {
        if (!list || list.length == 0) return false;
        for (var i = 0; i < list.length; i++) {
            if (list[i].ObjectID == obj.ObjectID) return true;
        }
        return false;
    };
    //(搜索条件传参)判断是否存在该对象
    $scope.existsParameter = function (e, obj) {
        if (angular.equals({}, obj)) {
            return "";
        }
        else {
            if (obj && obj.hasOwnProperty(e)) {
                return obj[e];
            } else {
                return "";
            }
        }
    };

    // 打开我的流程
    $scope.openWorkItem = function (instanceId) {
        if (!instanceId) return;
        var absurl = {
            state: 'tab.myInstances',
            tab: $scope.slectIndex
        }
        window.localStorage.setItem("absurl", JSON.stringify(absurl));
        $scope.worksheetUrl = $scope.setting.instanceSheetUrl + instanceId + "&LoginName=" + encodeURI($scope.user.Code) + "&LoginSID=" + $scope.clientInfo.UUID + "&MobileToken=" + $scope.user.MobileToken;
        commonJS.OpenInstanceSheet($scope, $scope.worksheetUrl, instanceId);
    }
    //搜索赋值
    $scope.setCurrentTab = function (tab, data) {
        switch (tab) {
            case 0:
                $scope.unfinishNum = data.TotalCount;
                return $scope.instances.unfinished = data;
                break;
            case 1:
                return $scope.instances.finished = data;
                break;
            case 2:
                return $scope.instances.cancel = data;
                break;
        }
    }
    //我的流程搜索
    $scope.toSearch = function (filter) {
        if (!angular.equals({}, filter)) {
            //存储搜索数组
            $scope.searchKeyArry[$scope.slectIndex] = filter;
            $scope.overtme = commonJS.timeCheck(filter.startDate, filter.endDate);
            if ($scope.overtme) {
                commonJS.showShortMsg("setcommon f15", "时间区间错误", 2000);
                return false;
            }
           // console.log($scope.searchKeyArry);
            // console.log("那个页面点击的搜索。。。" + $scope.slectIndex);
            var url = "";
            url = $scope.setting.httpUrl + "/Mobile/LoadInstances";
            //  console.log(filter.startDate);
            var options = {
                userId: $scope.user.ObjectID,
                keyWord: filter.keyWord || "",//流程名称
                IsPriority: filter.IsPriority || "",//2:加急，0：不加。空：不限
                Status: $scope.searchIndex,//2,代办，4已完成，5取消
                startDate: filter.startDate || "",//开始时间
                endDate: filter.endDate || "",//开始时间
            };
            console.log(options);
            $http({
                url: url,
                params: options
            }).success(function (data) {
               
                // 滚动到顶部
                $ionicScrollDelegate.scrollTop(true);
                $scope.setCurrentTab($scope.slectIndex, data);
            }).error(function (data) { })

        }
        else {//没有搜索条件点确定=重置$scope.searchfinishedBefore
            $scope.setCurrentTab($scope.slectIndex, $scope.searchfinishedBefore[$scope.slectIndex]);
        }
        $scope.popover.hide();
    };
    $scope.resetSearch = function () {
       // console.log($scope.filter);
        $scope.filter = {};//重置搜索的字段
        $scope.searchKeyArry[$scope.slectIndex] = $scope.filter;
        $scope.setCurrentTab($scope.slectIndex, $scope.searchfinishedBefore[$scope.slectIndex]);
       // $scope.popover.hide();
    };


})
