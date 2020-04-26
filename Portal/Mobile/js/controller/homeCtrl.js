module.controller('HomeCtrl', function ($scope, $rootScope, $state, $timeout, $ionicLoading, UnfinishedWorkItems, finishedworkitems, Unreadedworkitems, Readedworkitems, $http, $ionicScrollDelegate, $ionicSlideBoxDelegate, httpService, commonJS, GetWorkItemCount, $ionicHistory) {
    $scope.filter = {};//当前搜索的字段
    $scope.searchItemShow = true;//是否显示选人控件选项
    $scope.filterFlag = false;//点击选人控件之后重新要打开筛选页
    $scope.searchflag = true;//是否展示筛选框
    $scope.searchKeyArry = [];//记住三个部门的前一个字段
    $scope.$on("$ionicView.enter", function (scopes, states) {

        $scope.$broadcast('scroll.infiniteScrollComplete');//关闭初始状态触发上啦拉刷新
        if ($scope.filterFlag) {//已经点击选人控件，重新打开
            $scope.openPopover();
        }
        else if (!$scope.filterFlag) {
            //区分第一次和之后点击选人控件是否关闭
            $scope.user = JSON.parse(window.localStorage.getItem('OThinker.H3.Mobile.User'));
            console.log($scope.user);
            console.log($scope.user.ObjectID);
            //if ($scope.user.ObjectID == "") {//存在缓存id就应该初始
            //    var loginfrom = commonJS.getUrlParam('loginfrom');
            //    if (window.cordova || !loginfrom) {
            //        $state.go("login");
            //    } else {
            //        commonJS.loadingShow();
            //    }

            //}
            var reloadData = commonJS.getUrlParam('reloadData');
            if ($rootScope.loginInfo.success || reloadData) {
                $scope.init();
            } else {
                if (window.cordova || !$rootScope.loginInfo.loginfrom) {
                    $state.go("login");
                } else {
                    commonJS.loadingShow();
                }
            }
            commonJS.sideSlip($scope, 'templates/filter.html');
        }
        //设置钉钉头部
        if ($rootScope.dingMobile.isDingMobile) {
            $scope.SetDingDingHeader($rootScope.languages.tabs.home);
            dd.biz.navigation.setMenu({
                backgroundColor: "#ADD8E6",
                textColor: "#A7A7A7",
                items: [
                  {
                      "id": "1",
                      "text": $rootScope.languages.tabHome.setting
                  },
                  {
                      "id": "2",
                      "text": $rootScope.languages.filter
                  }
                ],
                onSuccess: function (data) {
                    if (data && Number(data.id) == 1) { $scope.gosetting(); }
                    if (data && Number(data.id) == 2) { //侧滑框 筛选
                        $scope.openPopover();
                    }
                },
                onFail: function (err) {
                }
            });
        }
    });

    //钉钉登录接受成功登录再加载(可能不需要了)
    $rootScope.$on("LoginIn", function () {
        //获取缓存的用户信息
        // $scope.user = JSON.parse(window.localStorage.getItem('OThinker.H3.Mobile.User'));
        $scope.init();
    });
    //入口初始化程序，页面初次加载的事件
    $scope.init = function () {
        //数据已全部加载完毕 第一次自动上拉刷新的bug
        $scope.allDataLoaded = false;

        // $ionicHistory.clearCache();
        // $ionicHistory.clearHistory();
        $scope.sampleData = false;//是否存在样列数据
        $ionicSlideBoxDelegate.enableSlide(false);
        $scope.tabNames = $rootScope.languages.tabHome.tab;
        $scope.pullingText = "松开刷新";
        $scope.refreshingText = "努力加载中...";
        //搜索字段
        $scope.searchIndex = false;//索引
        $scope.searchfinishedBefore = [];//当前搜索的字段
        /*切换副标题*/
        $scope.slectIndex = 0;
        $scope.activeSlide = function (index) {//点击时候触发
            switch (index) {
                case 0:
                    if (!$scope.GetUndo) {
                        $scope.GetUndo = true;
                    }
                    break;
                case 1:
                    if (!$scope.GetUnreade) {
                        commonJS.loadingShow();
                        $scope.GetUnReadedWorkItems().then(function (res) {
                            $scope.unreadedworkitems = res.CirculateItems;
                            $scope.unReadNum = res.TotalCount;
                            $scope.searchfinishedBefore[1] = res;//存储搜索前的数据
                            $scope.unreadedworkLoadComplete = res.LoadComplete;
                        }, function (reason) {

                        }).finally(function () {
                            $scope.GetUnreade = true;
                            // 滚动到顶部(避免会直接多一次上拉刷新的操作)
                            $ionicScrollDelegate.scrollTop(true);
                            commonJS.loadingHide();
                        })

                    }
                    break;
                case 2:
                    if (!$scope.Getdo) {
                        commonJS.loadingShow();
                        $scope.GetfinishedWorkItems().then(function (res) {
                            $scope.finishedworkitems = res.WorkItems;
                            $scope.searchfinishedBefore[2] = res;//存储搜索前的数据
                            $scope.finishedWorkLoadComplete = res.LoadComplete;
                        }, function (reason) {

                        }).finally(function () {
                            $scope.Getdo = true;
                            // 滚动到顶部(避免会直接多一次上拉刷新的操作)
                            $ionicScrollDelegate.scrollTop(true);
                            commonJS.loadingHide();
                        })

                    }
                    break;
                case 3:
                    if (!$scope.Getreade) {
                        commonJS.loadingShow();
                        $scope.GetReadedWorkItems().then(function (res) {
                            $scope.readedworkitems = res.CirculateItems;
                            $scope.searchfinishedBefore[3] = res;//存储搜索前的数据
                            $scope.readedworkLoadComplete = res.LoadComplete;
                        }, function (reason) {

                        }).finally(function () {
                            $scope.Getreade = true;
                            // 滚动到顶部(避免会直接多一次上拉刷新的操作)
                            $ionicScrollDelegate.scrollTop(true);
                            commonJS.loadingHide();
                        })
                    }
                    break;
            }
            $scope.slectIndex = index;
            $ionicSlideBoxDelegate.slide(index);

        };
        $initAllData();


    };


    //获取当前的Tab的数据长度用于搜索
    $scope.getCurrentTabLength = function (tab) {
        switch (tab) {
            case 0:
                return $scope.unfinishedWorkItems.length;
                break;
            case 1:
                return $scope.unreadedworkitems.length;
                break;
            case 2:
                return $scope.finishedworkitems.length;
                break;
            case 3:
                return $scope.readedworkitems.length;
                break;
        }
    }


    //待办和待阅数
    $scope.unReadNum = 0;
    $scope.unDoNum = 0;
    $scope.GetWorkItemCount = function (extend) {
        var options = {};
        if (extend) {
            $scope.extend(options, extend);
        }
        return GetWorkItemCount.all(options);
    };

    //待办
    $scope.unfinishedWorkItems = [];//存放数据
    $scope.unfinishedWorkLoadComplete = true;//是否还有数据
    // $scope.unfinishedWorkLastTime = '';//上次加载时间
    $scope.GetUnfinishedWorkItems = function (extend) {
        var options = {
            finishedWorkItem: false,
            keyWord: '',
            //  lastTime: new Date().Format("yyyy-MM-dd HH:mm:ss"),
            sortDirection: 'Desc',
            sortKey: 'ReceiveTime',
            userId: $scope.user.ObjectID
        }
        if (extend) {
            $scope.extend(options, extend);
        }
        return UnfinishedWorkItems.all(options);
    };
    //待阅
    $scope.unreadedworkitems = [];
    $scope.unreadedworkLoadComplete = true;
    $scope.GetUnReadedWorkItems = function (extend) {
        var options = {
            keyWord: '',
            readWorkItem: false,
            sortDirection: 'Desc',
            sortKey: 'ReceiveTime',
            userCode: $scope.user.Code,
            userId: $scope.user.ObjectID
        }
        if (extend) {
            $scope.extend(options, extend);
        }
        return Unreadedworkitems.all(options);
    };
    //已办
    $scope.finishedworkitems = [];
    $scope.finishedWorkLoadComplete = true;
    $scope.GetfinishedWorkItems = function (extend) {
        var options = {
            finishedWorkItem: true,
            keyWord: '',
            sortDirection: 'Desc',
            sortKey: 'OT_WorkItemFinished.FinishTime',
            userId: $scope.user.ObjectID
        }
        if (extend) {
            $scope.extend(options, extend);
        }
        return finishedworkitems.all(options);
    };
    //已阅
    $scope.readedworkitems = [];
    $scope.readedworkLoadComplete = true;
    $scope.GetReadedWorkItems = function (extend) {
        var options = {
            keyWord: '',
            readWorkItem: true,
            sortDirection: 'Desc',
            sortKey: 'ReceiveTime',
            userId: $scope.user.ObjectID
        }
        if (extend) {
            $scope.extend(options, extend);
        }
        return Readedworkitems.all(options);
    };



    //初始化所有数据
    $initAllData = function () {
        commonJS.loadingShow();
        //点击加载(只控制第一次)
        $scope.GetUndo = true;
        $scope.GetUnreade = false;
        $scope.Getdo = false;
        $scope.Getreade = false;
      
        //待办
        $scope.GetUnfinishedWorkItems().then(function (res) {
            if (res.hasOwnProperty('Success')) {
                commonJS.TimeoutHandler();
            }
            $scope.unfinishedWorkItems = res.WorkItems;
            $scope.unDoNum = res.TotalCount;
            $scope.searchfinishedBefore[0] = res;//存储搜索前的数据
            $scope.unfinishedWorkLoadComplete = res.LoadComplete;
        }, function (reason) {

        }).finally(function () {
            // 滚动到顶部
            $ionicScrollDelegate.scrollTop(true);
            commonJS.loadingHide();
        })
        //代办待阅数
        $scope.GetWorkItemCount().then(function (res) {
            if (!res.Success && res.ExceptionCode == 1) {

                $scope.$broadcast('scroll.infiniteScrollComplete');
                $ionicLoading.show({
                    template: '<span class="lodingspan f15">' + res.Message + '</span>',
                    duration: 4 * 1000,
                    animation: 'fade-in',
                    showBackdrop: false,
                });

            } else {
                $scope.unReadNum = res.Extend.UnreadWorkItemCount;
                $scope.unDoNum = res.Extend.UnfinishedWorkItemCount;
            }

        }, function (reason) {

        }).finally(function () {
        })
        /*处理微信的表单返回*/
        if ($rootScope.loginInfo.loginfrom == "wechat" && $scope.JumpParams.tab) {
            $scope.slectIndex = $scope.JumpParams.tab;
            $scope.renderJumpParams();
            $scope.activeSlide($scope.slectIndex);
        }
    }

    //跳转设置页
    $scope.gosetting = function () {
        $state.go("settings");
    }

    //切换tab回到顶部
    $scope.$watch("slectIndex", function (newVal, oldVal) {
        // 滚动到顶部
        $scope.filter = $scope.searchKeyArry[$scope.slectIndex] || {};//重置搜索的字段
        $rootScope.filterUsers = $scope.filter.filterUsers || "";
        $ionicScrollDelegate.scrollTop(true);
        //区别搜索传值
        switch (newVal) {
            case 1:
            case 0:
                $scope.searchIndex = false;
                break;
            case 2:
            case 3:
                $scope.searchIndex = true;
                break;
        }
    });


    //标记为已阅
    $scope.setReaded = function (id) {
        var options = {
            CirculateItemIDs: id,
            ReadAll: false,
            userCode: $scope.user.Code,
            userId: $scope.user.ObjectID
        };
        Unreadedworkitems.remove(options).then(function (res) {
            //  console.log(res);
            //删除该元素
            var len = $scope.unreadedworkitems.length;
            for (var i = 0; i < len; i++) {
                if ($scope.unreadedworkitems[i].ObjectID == id) {
                    $scope.unreadedworkitems.splice(i, 1);
                }
            }
            //代办待阅数
            $scope.GetWorkItemCount().then(function (result) {
                $scope.unReadNum = result.Extend.UnreadWorkItemCount;
                $scope.unDoNum = result.Extend.UnfinishedWorkItemCount;
            }, function (reason) {

            }).finally(function () {

            })
        }, function (reason) {

        })
    }

    //下拉刷新
    $scope.refresh = function (e) {
        var index = $scope.slectIndex;
        var extend = {};
        var complete = 0;
        function completeAll() {
            complete++;
            if (complete == 1) {
                $scope.$broadcast('scroll.refreshComplete');
            }
        }
        //刷新代办和待阅数目

        $scope.$broadcast('scroll.refreshComplete');
        if (index == 0) {
            extend = $scope.addsearchKey(index);
            extend.existsLength = $scope.unfinishedWorkItems.length;
            //console.log(" 下拉。。。。console.log(extend);");
            // console.log(extend);
            $scope.GetUnfinishedWorkItems(extend).then(function (res) {
                if ($scope.unfinishedWorkItems.length < 10) {
                    $scope.unfinishedWorkItems = res.WorkItems;
                } else {
                    $scope.unfinishedWorkItems.splice(0, 10);
                    $scope.unfinishedWorkItems = res.WorkItems.concat($scope.unfinishedWorkItems);
                }
                $scope.unDoNum = res.TotalCount;
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        } else if (index == 1) {
            extend = $scope.addsearchKey(index);
            extend.existsLength = $scope.unreadedworkitems.length;
            $scope.GetUnReadedWorkItems(extend).then(function (res) {
                if ($scope.unreadedworkitems.length < 10) {
                    $scope.unreadedworkitems = res.CirculateItems;
                } else {
                    $scope.unreadedworkitems.splice(0, 10);
                    $scope.unreadedworkitems = res.CirculateItems.concat($scope.unreadedworkitems);
                }
                $scope.unReadNum = res.TotalCount;
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        } else if (index == 2) {
            extend = $scope.addsearchKey(index);
            extend.existsLength = $scope.finishedworkitems.length;
            $scope.GetfinishedWorkItems(extend).then(function (res) {
                if ($scope.finishedworkitems.length < 10) {
                    $scope.finishedworkitems = res.WorkItems;
                } else {
                    $scope.finishedworkitems.splice(0, 10);
                    $scope.finishedworkitems = res.WorkItems.concat($scope.finishedworkitems);
                }
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        } else if (index == 3) {
            extend = $scope.addsearchKey(index);
            extend.existsLength = $scope.readedworkitems.length;
            $scope.GetReadedWorkItems(extend).then(function (res) {
                if ($scope.readedworkitems.length < 10) {
                    $scope.readedworkitems = res.CirculateItems;
                } else {
                    $scope.readedworkitems.splice(0, 10);
                    $scope.readedworkitems = res.CirculateItems.concat($scope.readedworkitems);
                }
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        }
    }


    //加载更多
    $scope.loadMore = function () {
        function completeAll() {
            $scope.$broadcast('scroll.infiniteScrollComplete');
            //  console.log('complete scroll!');
        }

        var extend = {}
        var index = $scope.slectIndex;
        if (index == 0) {
            extend = $scope.addsearchKey(index);
            extend.loadStart = $scope.getCurrentTabLength(index);
            $scope.GetUnfinishedWorkItems(extend).then(function (res) {
                $scope.$broadcast('scroll.refreshComplete');
                $scope.unfinishedWorkItems = $scope.unfinishedWorkItems.concat(res.WorkItems);
                $scope.unfinishedWorkLoadComplete = res.LoadComplete;
                //  $scope.unfinishedWorkLastTime = new Date(Number(res.LastTime.match(/\((\d+)\)/)[1])).Format("yyyy-MM-dd HH:mm:ss");

            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        } else if (index == 1) {
            extend = $scope.addsearchKey(index);
            extend.loadStart = $scope.getCurrentTabLength(index);
            $scope.GetUnReadedWorkItems(extend).then(function (res) {
                $scope.unreadedworkitems = $scope.unreadedworkitems.concat(res.CirculateItems);
                $scope.unreadedworkLoadComplete = res.LoadComplete;
                //  $scope.unreadedworkLastTime = new Date(Number(res.LastTime.match(/\((\d+)\)/)[1])).Format("yyyy-MM-dd HH:mm:ss");
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        } else if (index == 2) {
            extend = $scope.addsearchKey(index);
            extend.loadStart = $scope.getCurrentTabLength(index);
            $scope.GetfinishedWorkItems(extend).then(function (res) {
                $scope.finishedworkitems = $scope.finishedworkitems.concat(res.WorkItems);
                $scope.finishedWorkLoadComplete = res.LoadComplete;
                //   $scope.finishedWorkLastTime = new Date(Number(res.LastTime.match(/\((\d+)\)/)[1])).Format("yyyy-MM-dd HH:mm:ss");
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        } else if (index == 3) {
            extend = $scope.addsearchKey(index);
            extend.loadStart = $scope.getCurrentTabLength(index);
            $scope.GetReadedWorkItems(extend).then(function (res) {
                $scope.readedworkitems = $scope.readedworkitems.concat(res.CirculateItems);
                $scope.readedworkLoadComplete = res.LoadComplete;
                // $scope.readedworkLastTime = new Date(Number(res.LastTime.match(/\((\d+)\)/)[1])).Format("yyyy-MM-dd HH:mm:ss");
            }, function (reason) {

            }).finally(function () {
                completeAll();
            })
        }
    }

    ////////////////////////////////////////////////////////////////////////////////
    //扩充对象
    $scope.extend = function (o, n) {
        for (var p in n)
            if (n.hasOwnProperty(p)) o[p] = n[p];
    };

    // 打开待办待阅已办已阅
    $scope.openWorkItem = function (workItemId) {
        if (!workItemId) return;
        var absurl = {
            state: 'tab.home',
            tab: $scope.slectIndex
        }
        window.localStorage.setItem("absurl", JSON.stringify(absurl));
        $scope.worksheetUrl = $scope.setting.workItemUrl + workItemId + "&LoginName=" + encodeURI($scope.user.Code) + "&MobileToken=" + $scope.user.MobileToken;

        //$scope.worksheetUrl = $scope.setting.workItemUrl + workItemId + "&LoginName=" + encodeURI($scope.user.Code) + "&LoginSID=" + $scope.clientInfo.UUID + "&MobileToken=" + $scope.user.MobileToken;
        commonJS.GetWorkItemSheetUrl($scope, $scope.worksheetUrl, workItemId);

    }

    //搜索赋值
    $scope.setCurrentTab = function (tab, data) {
        switch (tab) {
            case 0:
                $scope.unfinishedWorkLoadComplete = data.LoadComplete;
                $scope.unDoNum = data.TotalCount;//更新微数据q
                return $scope.unfinishedWorkItems = data.WorkItems;
                break;
            case 1:
                $scope.unreadedworkLoadComplete = data.LoadComplete;
                $scope.unReadNum = data.TotalCount;//更新微数据
                return $scope.unreadedworkitems = data.CirculateItems;
                break;
            case 2:
                $scope.finishedWorkLoadComplete = data.LoadComplete;
                return $scope.finishedworkitems = data.WorkItems;
                break;
            case 3:
                $scope.readedworkLoadComplete = data.LoadComplete;
                return $scope.readedworkitems = data.CirculateItems;

                break;
        }
    }
    //我的流程搜索
    $scope.toSearch = function (filter) {
        $scope.filter.filterUsers = $rootScope.filterUsers;
        var array = $scope.initItemsCode($rootScope.filterUsers);//选人控件传数据的特殊格式
        if (!angular.equals({}, filter)) {
            //存储搜索数组
            $scope.searchKeyArry[$scope.slectIndex] = filter;  //时间区间不对校验
            $scope.overtme = commonJS.timeCheck(filter.startDate, filter.endDate);
            if ($scope.overtme) {
                commonJS.showShortMsg("setcommon f15", "时间区间错误", 2000);
                return false;
            }
            var options = {
                sortDirection: "Desc",
                sortKey: "ReceiveTime",
                userId: $scope.user.ObjectID,
                keyWord: filter.keyWord || "",//流程名称
                IsPriority: filter.IsPriority || "",//2:加急，0：不加。空：不限
                startDate: filter.startDate || "",//开始时间
                endDate: filter.endDate || "",//开始时间
                Originators: array,
            };
            if ($scope.slectIndex == 0 || $scope.slectIndex == 2) {
                var url = "";//待办
                url = $scope.setting.httpUrl + "/Mobile/GetWorkItems";
                options.finishedworkItem = $scope.searchIndex;
            }
            else {
                var url = "";;//待阅
                url = $scope.setting.httpUrl + "/Mobile/LoadCirculateItems";
                options.readWorkItem = $scope.searchIndex;
            }
            console.log("筛选。。。。。。" + url);
            console.log(options);
            $http({
                url: url,
                params: options
            }).success(function (data) {

                $scope.filterFlag = false;
                // $scope.popover.hide();
                $scope.setCurrentTab($scope.slectIndex, data);
            }).error(function (data) {

            })

        }
        else {//没有搜索条件点确定=重置
            $scope.setCurrentTab($scope.slectIndex, $scope.searchfinishedBefore[$scope.slectIndex]);
        }
        // 滚动到顶部
        $ionicScrollDelegate.scrollTop(true);
        $scope.popover.hide();
    };
    $scope.resetSearch = function () {
        $scope.filter = {};//重置搜索的字段
        $scope.searchKeyArry[$scope.slectIndex] = $scope.filter;
        $rootScope.filterUsers = "";
        $scope.setCurrentTab($scope.slectIndex, $scope.searchfinishedBefore[$scope.slectIndex]);
        //  $scope.popover.hide();
    };
    $scope.initItemsCode = function (users) {
        var objs = [];
        if (users && !angular.equals({}, users)) {
            if (users.constructor == Object) {
                var tempUser = users.Code;
                objs.push(tempUser);
            } else {
                users.forEach(function (n, i) {
                    var tempUser = n.Code;
                    objs.push(tempUser);
                });
            }
        }
        return objs;
    };
    //上下拉刷新所带searchkey
    $scope.addsearchKey = function (index) {
        var indexfilter = $scope.searchKeyArry[index] || {};
        var array = [];
        if (!angular.equals({}, indexfilter)) {
            var array = $scope.initItemsCode(indexfilter.filterUsers);//选人控件传数据的特殊格式
        }
        //搜索条件
        var options = {
            sortDirection: "Desc",
            sortKey: "ReceiveTime",
            userId: $scope.user.ObjectID,
            keyWord: indexfilter.keyWord || "",//流程名称
            IsPriority: indexfilter.IsPriority || "",//2:加急，0：不加。空：不限
            startDate: indexfilter.startDate || "",//开始时间
            endDate: indexfilter.endDate || "",//开始时间
            Originators: array
        };
        if ($scope.slectIndex == 0 || $scope.slectIndex == 2) {
            options.finishedworkItem = $scope.searchIndex;
        }
        else {
            options.readWorkItem = $scope.searchIndex;
        }
        return options;
    };
});
