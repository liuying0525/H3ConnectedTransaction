module.controller('myInstancesCtrl', function ($scope,instances,$ionicSlideBoxDelegate,commonJS,$ionicScrollDelegate) {
    //入口初始化程序，页面初次加载的事件
    $scope.init = function () {
        /*sousu*/
        $scope.tabNames = ['进行中', '已完成', '已取消'];
        $scope.pullingText = "松开刷新";
        $scope.refreshingText = "努力加载中...";
        $scope.slectIndex = 0;
        $scope.searchKey = '';
        instances.all().then(function (res) {
            $scope.instances = res.Extend;
         });
       //侧滑框 筛选 openPopover closePopover
        commonJS.sideSlip($scope,'templates/filter.html');
        /*点击切换副标题*/
        $scope.activeSlide = function (index) {//点击时候触发
            $scope.slectIndex = index;
            $ionicSlideBoxDelegate.slide(index);
        };
        /*滑动切换副标题*/
        $scope.slideChanged = function (index) {//滑动时候触发
            if ($scope.searchKey != "") {
                $scope.clearSearch(function () {
                    $scope.slectIndex = index;
                });
            } else {
                $scope.slectIndex = index;
            }
        };
    };
       $scope.$watch("instances", function (newVal, oldVal) {
        if ($scope.instances)
            $scope.currentTab = $scope.getCurrentTab($scope.slectIndex);
    }, true);
    $scope.$watch("slectIndex", function (newVal, oldVal) {
        // 滚动到顶部
        $ionicScrollDelegate.scrollTop(true);
        if ($scope.instances) {
            $scope.currentTab = $scope.getCurrentTab(newVal);
            $scope.currentTab.moredata=false;//初始每个tab第一次会有刷新动作false代表不会移除
        }
    });
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
            commonJS.loadingShow();
         instances.loadWorkItems().then(function (result) {
             console.log("'上拉,,,");
            if (result.Success) {
                $scope.updateInstances(result.Extend, "load");
                commonJS.loadingHide();
            }

        })
        // 滚动到顶部
        $ionicScrollDelegate.scrollTop(true);

    };
      $scope.$broadcast('scroll.infiniteScrollComplete');
    };   
 //上拉刷新
    $scope.doRefresh = function () {
        $scope.lastRefshTime = new Date();
        var url = "";
        var params = null;
        if (window.cordova) {
            //刷新带参数
          }
        else {
          //刷新带参数
        }
        //刷新请求的数据
         instances.refreshWorkItems(params).then(function (result) {
             console.log("下拉.........");
             console.log(result);
            if (result.Success) {

                $scope.updateInstances(result.Extend, "refresh");
                commonJS.loadingHide();
                $scope.$broadcast('scroll.refreshComplete');
                 
            }
        })

    }; //更新数据,type:load/refresh
    $scope.updateInstances = function (obj, type) {
        switch ($scope.slectIndex) {
            case 0:
                $scope.mergeData($scope.currentTab.list, $scope.instances.unfinished, obj.unfinished, type);
                break;
            case 1:
                $scope.mergeData($scope.currentTab.list, $scope.instances.finished, obj.finished, type);
                break;
            case 2:
                $scope.mergeData($scope.currentTab.list, $scope.instances.cancel, obj.cancel, type);
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
            if (ret.list && ret.list.length > 0) {
                for (var i = ret.list.length - 1; i > -1; i--) {
                    if ($scope.existsInstance($scope.currentTab.list, ret.list[i])) continue;
                    obj.list.splice(0, 0, ret.list[i]);
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
 
    $scope.init();
})
