module.controller('HomeCtrl', function ($scope, $state, UnfinishedWorkItems, finishedworkitems, Unreadworkitems, $ionicScrollDelegate, $ionicSlideBoxDelegate, httpService, commonJS, GetWorkItemCount) {

    //入口初始化程序，页面初次加载的事件
    $scope.init = function () {
        $scope.slectIndex = 0;
        $ionicSlideBoxDelegate.enableSlide(false);
        $scope.tabNames = ['待办', '待阅', '已办', '已阅'];
        $scope.pullingText = "松开刷新";
        $scope.refreshingText = "努力加载中...";
        $scope.lengthFlag = false;//是否还有更多数据
        $scope.begeFlag = true;//是否有微阅读的数目
        /*切换副标题*/

        $scope.searchKey = '';
        $scope.activeSlide = function (index) {//点击时候触发

            if ($scope.searchKey != '') {
                $scope.clearSearch(function () {
                    $scope.slectIndex = index;
                    $ionicSlideBoxDelegate.slide(index);
                });
            } else {
                $scope.slectIndex = index;
                $ionicSlideBoxDelegate.slide(index);
            }
        };
        //初始化
        $scope.initUnfinishedWorkItems();
        $scope.initfinishedWorkItems();
        $scope.initfinishedReadWorkItems();
        $scope.initUnReadWorkItems();

        //侧滑框 筛选 openPopover closePopover
        commonJS.sideSlip($scope, 'templates/filter.html');


        //test
        GetWorkItemCount.then(function (result) {
            console.log("GetWorkItemCount");
            console.log(result);
        }, function (reason) {
            console.log("GetWorkItemCount");
            console.log(reason);
        })

    };
    //待办和待阅数
    $scope.GetWorkItemCount = function () {
        GetWorkItemCount.then(function (res) {
            // console.log(res);
        }, function (reason) {
            // commonJS.showShortTop(reason);
        })
    };
    $scope.GetWorkItemCount();



    //待办
    $scope.initUnfinishedWorkItems = function () {
        var params = {
            //userId: $scope.user.ObjectID,
            //mobileToken: $scope.user.MobileToken,
            //keyWord: $scope.searchKey,
            //lastTime: $scope.lastLoadTime.Format("yyyy-MM-dd HH:mm:ss"),
            //sortKey: $scope.sortKey,
            //sortDirection: $scope.sortDirection,
            //finishedWorkItem: false
        }
        UnfinishedWorkItems.all(params).then(function (res) {
            if (res.status == 1) {
                $scope.unfinishedWorkItems = res.result.workItems;
                $scope.unfinishedWorkItemNum = $scope.unfinishedWorkItems.length;
                $scope.lengthFlag = res.result.LoadComplete;
            };
        });
    };
    //已办
    $scope.initfinishedWorkItems = function () {
        finishedworkitems.all().then(function (res) {
            $scope.finishedworkitems = res.WorkItems;
            console.log($scope.finishedworkitems);
        });
    };
    //已阅
    $scope.initfinishedReadWorkItems = function () { };
    //未阅
    $scope.initUnReadWorkItems = function () {
        Unreadworkitems.all().then(function (res) {
            if (res.status == 1) {
                $scope.unreadworkitems = res.result.workItems;
                $scope.unreadworkitemNum = $scope.unreadworkitems.length;
            };
        });
        $scope.remove = function (ObjectId) {
            Unreadworkitems.remove(ObjectId);
        };
    };

    // 打开待办
    $scope.openWorkItem = function (workItemId) {
        if (!workItemId) return;
        //$scope.worksheetUrl = $scope.setting.workItemUrl + workItemId + "&LoginName=" + encodeURI($scope.user.Code) + "&LoginSID=" + $scope.clientInfo.UUID + "&MobileToken=" + $scope.user.MobileToken;
        $scope.worksheetUrl = "http://localhost:23690/portal/WorkItemSheets.html?IsMobile=true&WorkItemID=" + workItemId + "&LoginName=Administrator&LoginSID=&MobileToken=null";
        commonJS.GetWorkItemSheetUrl($scope, $scope.worksheetUrl, workItemId);
    }
    $scope.gosetting = function () {
        $state.go('settings');
    }
    //监听  待办
    $scope.$watch("unfinishedWorkItems", function (newVal, oldVal) {
        if ($scope.unfinishedWorkItems)

            $scope.currentTab = $scope.getCurrentTab($scope.slectIndex);
    }, true);
    //监听  待阅
    $scope.$watch("unreadworkitems", function (newVal, oldVal) {
        if ($scope.unreadworkitems)
            $scope.currentTab = $scope.getCurrentTab($scope.slectIndex);
    }, true);
    $scope.$watch("slectIndex", function (newVal, oldVal) {
        // 滚动到顶部
        $ionicScrollDelegate.scrollTop(true);
        if ($scope.instances) {
            $scope.currentTab = $scope.getCurrentTab(newVal);
        }
    });
    //获取当前的Tab
    $scope.getCurrentTab = function (tab) {
        switch (tab) {
            case 0:
                return $scope.unfinishedWorkItems;
                break;
            case 1:
                return $scope.unreadworkitems;
                break;
            case 2:
                return $scope.finishedworkitems;
                break;
            case 3:
                return $scope.readworkitems;
                break;
        }
    }
    //上拉刷新
    $scope.doRefresh = function (slectIndex) {
        $scope.lastRefshTime = new Date();
        var url = "";
        var params = null;
        //刷新待办数据
        // UnfinishedWorkItems.refreshWorkItems().then(function (result) {
        //      console.log("result");
        //     console.log(result);
        //      // commonJS.loadingHide();
        //        if (result.status == 1) {
        //        //$scope.unfinishedWorkItems.push(result.result.workItems);

        //           $scope.updateInstances(result.result.workItems, "refresh");

        //          setTimeout(function(){
        //             $scope.$broadcast('scroll.infiniteScrollComplete');
        //        },1000*2);
        //     };
        // })
        //刷新待阅数据
        Unreadworkitems.refreshWorkItems().then(function (result) {
            // commonJS.loadingHide();
            if (result.status == 1) {
                //$scope.unfinishedWorkItems.push(result.result.workItems);

                $scope.updateInstances(result.result.workItems, "refresh");

                setTimeout(function () {
                    $scope.$broadcast('scroll.infiniteScrollComplete');
                }, 1000 * 2);
            };
        })
    };
    //更新数据,type:load/refresh
    $scope.updateInstances = function (obj, type) {
        switch ($scope.slectIndex) {
            //待办
            case 0:
                $scope.mergeData($scope.currentTab, $scope.unfinishedWorkItems, obj, type);
                break;
                //待阅
            case 1:
                $scope.mergeData($scope.currentTab, $scope.unreadworkitems, obj, type);
                break;
                //已办
            case 2:
                $scope.mergeData($scope.currentTab, $scope.finishedworkitems, obj, type);
                break;
                //已阅
            case 3:
                $scope.mergeData($scope.currentTab, $scope.readworkitems, obj, type);
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
            obj.moredata = ret.moredata;
            obj.lastReloadTime = ret.lastReloadTime;
            if (ret.list && ret.list.length > 0) {
                for (var i = 0; i < ret.list.length; i++) {
                    if ($scope.existsInstance(list, ret.list[i])) continue;
                    obj.list.push(ret.list[i]);
                }
            }
        } else if (type == "refresh") {
            obj.lastRefreshTime = ret.lastRefreshTime;
            if (ret && ret.length > 0) {
                for (var i = ret.length - 1; i > -1; i--) {
                    if ($scope.existsInstance(obj, ret[i])) continue;
                    obj.splice(0, 0, ret[i]);
                }
            }
        }
    };
    //判断是否已经包含该对象
    $scope.existsInstance = function (list, obj) {
        if (!list || list.length == 0) return false;
        for (var i = 0; i < list.length; i++) {
            if (list[i].ObjectId == obj.ObjectId) return true;
        }
        return false;
    };
    $scope.init();
});
