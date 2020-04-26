var formModule = angular.module('formApp.controllers', [])
// 全局Controller
.controller("mainCtrl", function ($rootScope, $scope, $state, $compile, $http, $ionicPopup, $timeout, fcommonJS) {
    $rootScope.dingMobile = {
        isDingMobile: false,                             //是否钉钉移动端，如果是钉钉移动端，需要隐藏当前header，重写钉钉APP Header
        dingHeaderClass: "has-header",                   //隐藏header后 subHeader ion-content需要修改相关样式
        dingSubHeaderClass: "has-header has-subheader",  //隐藏header后 subHeader ion-content需要修改相关样式
        hideHeader: false                                //是否需要隐藏当前Header
    }



    //判断是否钉钉移动端
    $scope.GetDingMobile = function () { 
        var loginfrom = "";
        var source = "";
        var reg = new RegExp("(^|&)loginfrom=([^&]*)(&|$)");
        var r = top.window.location.search.substr(1).match(reg);
        if (r != null) loginfrom = unescape(r[2]);

        reg = new RegExp("(^|&)source=([^&]*)(&|$)");
        r = top.window.location.search.substr(1).match(reg);
        if (r != null) source = unescape(r[2]);
        $rootScope.source = source;
        if ($rootScope.source == "message" && loginfrom == "dingtalk" && dd && dd.version) {
            $rootScope.dingMobile.isDingMobile = true;
            $rootScope.dingMobile.dingHeaderClass = "";
            $rootScope.dingMobile.dingSubHeaderClass = "has-subheader";
            $rootScope.dingMobile.hideHeader = true;
            //钉钉打开消息返回 测试只对android有效
            document.addEventListener('backbutton', function () {
                if ($state.current.name == "form.detail" && loginfrom == "dingtalk" && dd) {
                    dd.biz.navigation.close({});
                }
            });
        }
        if (dd && dd.version) {
        //if (loginfrom == "dingtalk" && dd && dd.version) {
            $rootScope.dingMobile.isDingMobile = true;
            $rootScope.dingMobile.dingHeaderClass = "";
            $rootScope.dingMobile.dingSubHeaderClass = "has-subheader";
            $rootScope.dingMobile.hideHeader = true;
            $rootScope.loginfrom = "dingtalk";
        } else if (loginfrom == "app") {
            $rootScope.loginfrom = "app";
        } else if (loginfrom == "wechat") {
            $rootScope.loginfrom = "wechat";
        }
    }
    $scope.GetDingMobile();
    $scope.SetDingDingHeader = function (title) {
        //设置Header标题
        if (dd) {
            dd.biz.navigation.setTitle({
                title: title,//控制标题文本，空字符串表示显示默认文本
                onSuccess: function (result) { },
                onFail: function (err) {
                    alert(err);
                }
            });
        }
    }

    // 移动端重写alert方法
    window.alert = function (msg, callback) {
        if (msg.length >= 256) {
            msg = msg.substr(0, 256) + "...";
        }
        if (!callback) {
            var myPopup = $ionicPopup.show({
                cssClass: 'bpm-sheet-alert',
                template: '<span class="">' + msg + '<span>'
            });

            $timeout(function () {
                myPopup.close();
            }, 1500);
        }
        else {
            $ionicPopup.show({
                cssClass: 'bpm-sheet-confirm',
                template: '<span class="bpm-sheet-confirm">' + msg + '<span>',
                buttons: [{
                    text: '确定',
                    type: 'button-clear',
                    onTap: function (e) {
                        callback();
                    }
                }]
            });
        }
    }
    //微信表单返回逻辑参数
    $rootScope.orgIndex = window.history.length;
})

.controller("formSheetCtrl", function ($rootScope, $scope, $state, $timeout, $compile, $http, $ionicPopup, $ionicPlatform, $ionicTabsDelegate, $cordovaDevice, $cordovaAppVersion, $cordovaNetwork, $ionicScrollDelegate, $ionicActionSheet, $ionicHistory, $ionicModal, fcommonJS, $ionicPopover, httpService, $stateParams) {
    //PC端html内容
    var _ChildNodes = document.getElementById("content-wrapper").childNodes;
    //内容放置滚动div内，否则不能上下滚动
    $("#mobile-content").find("div.scroll").prepend(_ChildNodes);
    //支持移动端滚动
    $(_ChildNodes).css("overflow", "hidden");

    //移动端流程状态
    console.log(_PORTALROOT_GLOBAL);
    $scope.portalroot = _PORTALROOT_GLOBAL;
    $scope.instanceInfo = {};
    $scope.showMoreInfo = true;
    $scope.showHide = function () {
        $scope.showMoreInfo = !$scope.showMoreInfo;
        $ionicScrollDelegate.resize();
        if ($scope.showMoreInfo) {
            $ionicScrollDelegate.scrollBottom();
        }
    }

    //随机头像颜色
    $scope.getRandom = function () {
        var num = Math.floor(Math.random() * 4);
        return num;
    };
    $scope.randomNumbers = [];
    for (var i = 0; i < 5; i++) {
        $scope.randomNumbers.push($scope.getRandom());
    }
    console.log($scope.randomNumbers[0]);

    //在移动端删除PC框架
    $("div[id*=sheetContent]:first").remove();
    //
    $("div.row").removeClass("row").addClass("list");
    //传入ionic 服务
    $.MvcSheetUI.IonicFramework = {
        $rootScope: $rootScope,
        $scope: $scope,
        $state: $state,
        $timeout: $timeout,
        $compile: $compile,
        $ionicActionSheet: $ionicActionSheet,
        $ionicScrollDelegate: $ionicScrollDelegate,
        $compile: $compile,
        $ionicPopup: $ionicPopup,
        $ionicModal: $ionicModal,
        fcommonJS: fcommonJS,
        $ionicPopover: $ionicPopover//单选多选效果改为侧滑
    }
    //执行入口
    $.MvcSheet.Init(function () {
        try {
            //Header 标题
            var _InstanceTitle = $.MvcSheetUI.SheetInfo.BizObject.DataItems["Sheet__DisplayName"].V;
            $rootScope.InstanceId = $.MvcSheetUI.SheetInfo.InstanceId;
            $rootScope.WorkItemId = $.MvcSheetUI.SheetInfo.WorkItemId;
            $rootScope.SheetMode = $.MvcSheetUI.SheetInfo.SheetMode;
            $rootScope.WorkflowCode = $.MvcSheetUI.SheetInfo.WorkflowCode;
            $rootScope.WorkflowVersion = $.MvcSheetUI.SheetInfo.WorkflowVersion;
            $rootScope.InstanceTitle = _InstanceTitle;


            //标签名中英文切换
            $rootScope.names = {
                Back: SheetLanguages.Current.Back,
                Spread: SheetLanguages.Current.Spread,
                Retract: SheetLanguages.Current.Retract,
                WorkflowCharts: SheetLanguages.Current.WorkflowCharts,
                Close: SheetLanguages.Current.Close,
                OK: SheetLanguages.Current.OK,
                SelectAll: SheetLanguages.Current.SelectAll,
                UnselectAll: SheetLanguages.Current.UnselectAll,
                Search: SheetLanguages.Current.Search,
                Day: SheetLanguages.Current.Day,
                Hour: SheetLanguages.Current.Hour,
                Minute: SheetLanguages.Current.Minute,
                Second: SheetLanguages.Current.Second,
                State: SheetLanguages.Current.State,
                SheetUser: SheetLanguages.Current.SheetUser,
                Approve: SheetLanguages.Current.Approver,
                Waiting: SheetLanguages.Current.Waiting,
                Confirm: SheetLanguages.Current.Confirm,
                PleaseSelect: SheetLanguages.Current.PleaseSelect,
                Query: SheetLanguages.Current.Query,
                AssociatedSheet: SheetLanguages.Current.AssociatedSheet

            };
            console.log($rootScope.names);


            //修改样式为每个item加上下划线
            $("#masterContent_divContent .list>div.item").addClass('hasBorderBottom');
            $(".attachment.item").removeClass("hasBorderBottom");
            $(".slider-slides .item").removeClass("hasBorderBottom");
            $(".SheetGridView").addClass('hasBorderBottom');
            $(".SignaturePanel").addClass("hasBorderBottom");
            $(".InputPanel").addClass("hasBorderBottom");




            //流程状态初始化
            console.log($rootScope.InstanceId);
            httpService.get(_PORTALROOT_GLOBAL + '/wechat/LoadInstanceState', { instanceID: $rootScope.InstanceId, WorkItemId: $rootScope.WorkItemId }).then(function (res) {
                console.log(res);
                if (res.SUCCESS == true) {
                    $scope.instanceInfo.BaseInfo = res.BaseInfo;
                    if (res.BaseInfo.Approvers.length > 3) {
                        $scope.instanceInfo.BaseInfo.Approvers = res.BaseInfo.Approvers.slice(0, 3).join(',') + '...';
                    }
                    else {
                        $scope.instanceInfo.BaseInfo.Approvers = res.BaseInfo.Approvers.join(',');
                    }
                    $scope.instanceInfo.InstanceLogInfo = res.InstanceLogInfo;
                } else {
                    $scope.instanceInfo = false;
                }

            }, function (reason) {
                fcommonJS.showShortTop(reason);
            });
        } catch (e) { }
    });
    //打开流程图
    $scope.openFlowChart = function (e) {
        $state.go("form.instancestate", { Mode: $rootScope.SheetMode, InstanceID: $rootScope.InstanceId, WorkflowCode: $rootScope.WorkflowCode, WorkflowVersion: $rootScope.WorkflowVersion });
    }

    // 每次进入View时触发
    $scope.$on("$ionicView.enter", function (scopes, states) {
        if ($rootScope.fetchUserSelect) {
            $rootScope.fetchUserSelect.ClearChoices();
            $rootScope.fetchUserSelect.SetValue("");
        }
        if ($rootScope.dingMobile.isDingMobile && dd) {
            //设置header 右边按钮
            dd.biz.navigation.setRight({
                show: false
            });
            //ios的消息返回
            if ($rootScope.source == "message") {
                dd.biz.navigation.setLeft({
                    control: true,
                    onSuccess: function (result) {
                        if ($state.current.name == "form.detail") {
                            dd.biz.navigation.close({});
                        } else {
                            $ionicHistory.goBack();
                        }
                    },
                    onFail: function (err) { }
                });
            }
        }
    });

    //SheetUser完成事件
    $rootScope.$on('sheetUserFinished', function (event, data) {
        var ngmodel = data.dataField + data.rowNum;
        var the = $scope[ngmodel];
        the.ClearChoices();
        the.SetValue(data.obj);
    });
    //表单上的返回按钮
    $scope.ClosePage = function () {
        if ($rootScope.dingMobile.isDingMobile) {
            //钉钉的已隐藏
        } else if (typeof (WeixinJSBridge) != "undefined") {
            //微信关闭
            if ($rootScope.source == "message") {
                WeixinJSBridge.call("closeWindow");
            } else {
                var goIndex = $rootScope.orgIndex - window.history.length - 1;
                window.history.go(goIndex);
            }
        } else {
            //app关闭
            //window.open($.MvcSheetUI.PortalRoot + "/Mobile/index.html/tab.home", '_self');
            window.history.back();
        }
    }
})
    //选人控件
 .controller('sheetUserCtrl', function ($rootScope, $scope, $ionicActionSheet, $state, $stateParams, $ionicBackdrop, $timeout, $ionicModal, $ionicSideMenuDelegate, $ionicHistory, sheetUserService, $ionicScrollDelegate) {
     $scope.sheetUserHandler = $.MvcSheetUI.PortalRoot + "/SheetUser/LoadOrgTreeNodes";
     $scope.SelectFormStructure = !($.MvcSheetUI.sheetUserParams.loadOptions.indexOf("RootUnitID") < 0 && $.MvcSheetUI.sheetUserParams.options.UserVisible && $.MvcSheetUI.sheetUserParams.loadOptions.indexOf("VisibleUnits") < 0);
     $scope.init = function () {
         $scope.UserOUMembers = [];//所在部门人员    
         $scope.Organizations = []; //部门成员
         $scope.deptNav = [];  //导航数据项  { id;"",name:"",index:""}
         $scope.showDeptID = "";//当前部门id        
         $scope.CacheData = {};//缓存数据   
         $scope.SelectItems = [];//已选
         $scope.checkedstetus = true;//全选
         //搜索相关
         $scope.searchKey = "";
         $scope.SearchMode = false;
         $scope.searchedKeys = [];
         $scope.searchItems = [];//搜索结果
         $scope.searchedItems = [];//搜索缓存数据  
         //搜索展示人员还是部门
         $scope.SearchEmp = false;
         $scope.SearchDep = false;


     };
     $scope.$on("$ionicView.enter", function (scopes, states) {
         $scope.sheetUserParams = $.MvcSheetUI.sheetUserParams;
         $scope.ShowCurrentDept = $scope.sheetUserParams.loadOptions.indexOf("RootUnitID") < 0 && $scope.sheetUserParams.options.UserVisible && $scope.sheetUserParams.loadOptions.indexOf("VisibleUnits") < 0;
         $scope.init();
         $scope.SelectItems = sheetUserService.initItems($scope.sheetUserParams.initUsers);
         //设置已选-从组织架构中选择
         $scope.selectedName = sheetUserService.getSelectedName($scope.SelectItems);
         $scope.InitOUMembers();
         if (!$scope.ShowCurrentDept) {
             $scope.SelectStructure(true);
         } else {
             $scope.SelectFormStructure = false;
             $scope.checkedpagestaue();//检测组织页面是否选中
         }
     });

     //初始化本部门人员
     $scope.InitOUMembers = function () {
         if (!$.MvcSheetUI.SheetInfo.UserOUMembers && $scope.sheetUserParams.options.UserVisible) {
             $.MvcSheetUI.SheetInfo.UserOUMembers = [];
             var parentUnits = $.MvcSheetUI.SheetInfo.DirectParentUnits;
             for (var key in parentUnits) {
                 sheetUserService.loadData($scope.sheetUserHandler + "?IsMobile=true&ParentID=" + key + "&o=U", null).then(function (res) {
                     var filterdata = $.grep(res, function (value) {
                         if (value.ExtendObject.UnitType == "U") {
                             return value;
                         }
                     });
                     $.MvcSheetUI.SheetInfo.UserOUMembers = $.MvcSheetUI.SheetInfo.UserOUMembers.concat(filterdata);
                     //获取所在部门人员
                     $scope.UserOUMembers = sheetUserService.sheetUserAdapter($.MvcSheetUI.SheetInfo.UserOUMembers, $scope.sheetUserParams.selecFlag);
                     //所在部门设置已选-所在部门
                     $scope.UserOUMembers = sheetUserService.checkItems($scope.UserOUMembers, $scope.SelectItems);
                 });
             };
         } else {
             $scope.UserOUMembers = sheetUserService.sheetUserAdapter($.MvcSheetUI.SheetInfo.UserOUMembers, $scope.sheetUserParams.selecFlag);
             //所在部门设置已选-所在部门
             $scope.UserOUMembers = sheetUserService.checkItems($scope.UserOUMembers, $scope.SelectItems);
         }
     };

     //从组织架构中选择
     $scope.SelectStructure = function (SelectFormStructure) {
         $scope.SelectFormStructure = SelectFormStructure;
         //获取根节点信息
         var querystr = $scope.sheetUserParams.loadOptions.replace("&VisibleUnits=", "&V=");
         sheetUserService.loadData($scope.sheetUserHandler + "?LoadTree=true&Recursive=true&isMobile=true&" + querystr, null).then(function (res) {
             $scope.showDeptID = res[0].ObjectID;
             $scope.deptNav.push({
                 id: res[0].ObjectID,
                 name: res[0].Text,
                 pid: "",
                 index: "0"
             });
         });
         //加载数据
         if ($scope.CacheData[""]) {
             $scope.getCacheData("");
         } else {
             $scope.getData("");
         }
     };
     //
     $scope.itemClick = function (e, org) {
         //if ($scope.deptNav.length && $scope.deptNav[$scope.deptNav.length - 1].id == org.id) {
         //    return;
         //}
         var Percent = e.clientX / screen.availWidth * 100;
         if (org.type != "U" && org.root != true && !$scope.SearchMode && (!org.canSelect || Percent > 15)) {
             //展开部门
             $ionicScrollDelegate.scrollTop();
             org.checked = !org.checked;
             $scope.deptNav.push({
                 id: org.id,
                 name: org.name,
                 pid: $scope.showDeptID,
                 index: "1"
             });
             $scope.showDeptID = org.id;
             //加载数据
             if ($scope.CacheData[org.id]) {
                 $scope.getCacheData(org.id);
             } else {
                 $scope.getData(org.id)
             }
         } else {
             //选中
             $scope.addItem(e, org)
         }
     };
     //添加
     $scope.addItem = function (e, item) {
         var i = item;
         if (e.target.tagName.toLowerCase() != "input") {
             item.checked = !item.checked;
         }
         if (item.checked) {
             if (!$scope.sheetUserParams.isMutiple) {
                 $scope.SelectItems = new Array();
                 $scope.SelectItems.push(i);
                 $scope.sheetUserFinished();
             } else {
                 $scope.SelectItems.push(i);
             }
         } else {
             //删除已选
             $scope.SelectItems = sheetUserService.removeItem($scope.SelectItems, item);
         }
         $scope.initcheckedStetus($scope.Organizations, $scope.SelectItems.length);//改变全选按钮状态
         $scope.selectedName = sheetUserService.getSelectedName($scope.SelectItems);
         if (!$scope.SelectFormStructure) {
             $scope.checkedpagestaue();
         }
     };
     //删除已选
     $scope.delItem = function (index) {
         var deleteItem = $scope.SelectItems[index];
         console.log(deleteItem)
         $scope.SelectItems.splice(index, 1);
         //更新当前页面数据
         $scope.Organizations = sheetUserService.deleteSelectItem($scope.Organizations, $scope.SelectItems);
         $scope.UserOUMembers = sheetUserService.deleteSelectItem($scope.UserOUMembers, $scope.SelectItems);
         $scope.SelectItems = sheetUserService.removeItem($scope.SelectItems, deleteItem);
         $scope.selectedName = sheetUserService.getSelectedName($scope.SelectItems);
         $scope.initcheckedStetus($scope.Organizations, $scope.SelectItems.length);//改变全选按钮状态
     };
     //部门导航点击事件
     $scope.navClick = function (deptId, index) {
         $scope.deptNav = $scope.deptNav.slice(0, index + 1);
         //加载数据
         if ($scope.CacheData[deptId]) {
             $scope.getCacheData(deptId);
         } else {
             $scope.getData(deptId);
         }
         $scope.showDeptID = deptId;
     };
     //加载数据
     $scope.getData = function (parentid) {
         var querystr = $scope.sheetUserParams.loadOptions.replace("&VisibleUnits=", "&V=");
         sheetUserService.loadData($scope.sheetUserHandler + "?ParentID=" + parentid + "&isMobile=true&" + querystr, null).then(function (res) {
             $scope.Organizations = sheetUserService.sheetUserAdapter(res, $scope.sheetUserParams.selecFlag);
             //是否加根节点
             if (parentid == "" && $scope.sheetUserParams.options.RootSelectable && $scope.sheetUserParams.options.OrgUnitVisible == "true") {
                 //debugger
                 var root = {
                     Icon: "icon-zuzhitubiao",
                     canSelect: true,
                     checked: false,
                     code: $scope.deptNav[0].id,
                     id: $scope.deptNav[0].id,
                     name: $scope.deptNav[0].name,
                     type: "O",
                     root: true
                 };
                 $scope.Organizations.unshift(root);
             }

             $scope.checkedstetus = true;//是否全选
             $scope.Organizations = sheetUserService.checkItems($scope.Organizations, $scope.SelectItems);
             $scope.initcheckedStetus($scope.Organizations, $scope.SelectItems.length);//改变全选按钮状态
             if (parentid == "" && $scope.sheetUserParams.options.RootSelectable && $scope.sheetUserParams.options.OrgUnitVisible == "true") {
                 $scope.CacheData[$scope.deptNav[0].id] = $scope.Organizations;
             } else {
                 $scope.CacheData[parentid] = $scope.Organizations;
             }
         })
     };
     //加载缓存数据
     $scope.getCacheData = function (deptId) {
         $scope.Organizations = $scope.CacheData[deptId];
         $scope.Organizations = sheetUserService.checkItems($scope.Organizations, $scope.SelectItems);
         $scope.Organizations = sheetUserService.deleteSelectItem($scope.Organizations, $scope.SelectItems);
         $scope.initcheckedStetus($scope.Organizations, $scope.SelectItems.length);//改变全选按钮状态
     }

     //选择完成，回到表单页面
     $scope.sheetUserFinished = function () {
         var objs = sheetUserService.convertItems($scope.SelectItems);
         var rowNum = $scope.sheetUserParams.options.RowNum;
         $rootScope.$broadcast("sheetUserFinished", { dataField: $scope.sheetUserParams.dataField, obj: objs, rowNum: rowNum });
         $scope.init();
         $ionicHistory.goBack();
     };
     //搜索
     $scope.goToSeach = function () {
         $scope.SearchMode = true;
     };
     //清理缓存数据
     $scope.$watch('SearchMode', function (n, o) {
         if (n != true) {
             $scope.searchItems = [];
         }
     })
     //添加
     $scope.addSearchItem = function (e, item) {
         $scope.addItem(e, item);
         if (item.checked) {
             if (!$scope.sheetUserParams.isMutiple) {
                 $scope.closeSearchModal();
             }
         }
     };
     //清除
     $scope.resetSearchKey = function (e) {
         var Percent = e.clientX / screen.availWidth * 100;
         if ($scope.searchKey != "" && Percent > 90) {
             //清除搜索关键词
             $scope.searchKey = "";
         }
     };
     $scope.doSearch = function (key) {
         //搜索展示人员还是部门
         $scope.SearchEmp = false;
         $scope.SearchDep = false;
         if (!key) return;
         var cacheKey = key + ($scope.showDeptID || "");
         $scope.searchItems = [];
         //查询是否已经缓存
         var isSearched = $scope.searchedKeys.some(function (n) {
             return n === cacheKey;
         });
         //已经缓存，从缓存中获取
         if (isSearched) {
             $scope.searchedItems.forEach(function (currentItem) {
                 if (currentItem.key === cacheKey) {
                     if (currentItem.type == "U") {
                         $scope.SearchEmp = true;
                     }
                     if (currentItem.type == "O" || currentItem.type == "G") {
                         $scope.SearchDep = true;
                     }
                     var text = currentItem.name;
                     var regExp = new RegExp(cacheKey, 'g');
                     var result = text.replace(regExp, '<b class="userSearched">' + cacheKey + '</b>');
                     currentItem.names = result;
                     $scope.searchItems.push(currentItem);
                 }
             });
             $scope.searchItems = sheetUserService.checkItems($scope.searchItems, $scope.SelectItems);
         }
         else { //从服务器端获取数据
             $scope.searchedKeys.push(cacheKey);
             var querystr = $scope.sheetUserParams.loadOptions.replace("&VisibleUnits=", "&V=");
             var loadUrl = $scope.sheetUserHandler + "?" + querystr;
             var params = {
                 ParentID: $scope.showDeptID || "",
                 SearchKey: encodeURI(key),
                 IsMobile: true,
             };
             sheetUserService.loadData(loadUrl, params).then(function (res) {
                 var users = sheetUserService.sheetUserAdapter(res);
                 users.forEach(function (currentItem) {
                     if (currentItem.type == "U") {
                         $scope.SearchEmp = true;
                     }
                     else if (currentItem.type == "O" || currentItem.type == "G") {
                         $scope.SearchDep = true;
                     }
                     currentItem.key = cacheKey;
                     var text = currentItem.name;
                     var regExp = new RegExp(cacheKey, 'g');
                     var result = text.replace(regExp, '<b class="userSearched">' + cacheKey + '</b>');
                     currentItem.names = result;
                     $scope.searchedItems.push(currentItem)
                     $scope.searchItems.push(currentItem);
                 });
                 $scope.searchItems = sheetUserService.checkItems($scope.searchItems, $scope.SelectItems);

             });
         }

     };
     //关闭
     $scope.cancel = function () {
         $scope.sheetUserFinished();
     };
     //全选按钮状态
     /*$scope.checkedstetus = true;标识全选按钮不选中
     *objs：当前能选的数据
     *stetus：已经选中的数组的长度
     */
     $scope.initcheckedStetus = function (objs, len) {//$scope.SelectItems.length
         if ($scope.Organizations.length == 0 || !$scope.SelectFormStructure) {
             objs = $scope.UserOUMembers;
         }
         if (len == 0) {
             $scope.checkedstetus = true;
             return false;
         };
         var going = true;
         angular.forEach(objs, function (obj) {
             if (going) {
                 if (obj.canSelect && obj.checked) {//已经选中则跳过
                     $scope.checkedstetus = false;
                 }
                 else if (obj.canSelect && !obj.checked) {//能选但是未选中则直接返回
                     $scope.checkedstetus = true;
                     going = false;
                 }
             }

         });
     };
     //用于检测组织结构页面的数据是否选中
     $scope.checkedpagestaue = function () {
         if (!$scope.SelectFormStructure && $scope.SelectItems.length != 0) {//组织结构的页面
             var i = 0;
             angular.forEach($scope.UserOUMembers, function (obj) {
                 var going = true;
                 angular.forEach($scope.SelectItems, function (SelectItem) {
                     if (going) {
                         if (obj.id == SelectItem.id) {//选中
                             i = i + 1;
                             obj.checked = true;
                             going = false;
                         } else {
                             obj.checked = false;
                         }
                     }
                 });
             });
             if (i == $scope.UserOUMembers.length) {
                 $scope.checkedstetus = false;//全选按钮选中
             }
             else {
                 $scope.checkedstetus = true;//全选按钮B部选中
             }
         }
         else if (!$scope.SelectFormStructure && $scope.SelectItems.length == 0) {//当没有选中的情况下，相当于要把当前部门取消全选
             $scope.checkedObj($scope.UserOUMembers, false);//取消选中
         }
     }
     //全选
     $scope.checkedObj = function (objs, stetus) {
         angular.forEach(objs, function (obj) {
             if (obj.canSelect && stetus) {//选中
                 var i = obj;
                 if (!obj.checked) {//没有选中的要选中
                     obj.checked = true;
                     if (!$scope.sheetUserParams.isMutiple) {//单选
                         $scope.SelectItems = new Array();
                         $scope.SelectItems.push(i);
                         $scope.sheetUserFinished();
                     } else {
                         $scope.SelectItems.push(i);
                     }
                 }
             }
             else if (obj.canSelect && !stetus) {
                 obj.checked = false;
                 $scope.SelectItems = sheetUserService.removeItem($scope.SelectItems, obj);
             }
         });

     }
     $scope.checkedAll = function ($event) {
         // console.log($scope.Organizations);
         // console.log($scope.UserOUMembers);
         if ($scope.checkedstetus) {//全选
             if ($scope.Organizations.length == 0 || !$scope.SelectFormStructure) {
                 $scope.checkedObj($scope.UserOUMembers, true);
             }
             else {
                 $scope.checkedObj($scope.Organizations, true);
             }
             $scope.checkedstetus = false;
         } else {
             if ($scope.Organizations.length == 0 || !$scope.SelectFormStructure) {
                 $scope.checkedObj($scope.UserOUMembers, false);
             }
             else {
                 $scope.checkedObj($scope.Organizations, false);
             }
             $scope.checkedstetus = true;
         }
     };
     //返回上级组织
     $scope.goBack = function () {
         $ionicScrollDelegate.scrollTop();
         if ($scope.SearchMode) {
             $("input[type='search']").blur();
             $scope.SearchMode = false;
             $scope.searchItems = [];
             $scope.searchKey = "";
             $scope.Organizations = sheetUserService.checkItems($scope.Organizations, $scope.SelectItems);
             $scope.UserOUMembers = sheetUserService.checkItems($scope.UserOUMembers, $scope.SelectItems);

             $scope.Organizations = sheetUserService.deleteSelectItem($scope.Organizations, $scope.SelectItems);
             $scope.UserOUMembers = sheetUserService.deleteSelectItem($scope.UserOUMembers, $scope.SelectItems);
             return;
         }
         if ($scope.deptNav.length == 0 || ($scope.deptNav.length == 1 && !$scope.ShowCurrentDept)) {
             $scope.sheetUserFinished();
             $ionicHistory.goBack();
         } else {
             $scope.deptNav = $scope.deptNav.slice(0, $scope.deptNav.length - 1);
             if ($scope.deptNav.length == 0) {
                 $scope.SelectFormStructure = false;
                 $scope.checkedpagestaue();//组织结构页面
             } else {
                 var id = $scope.deptNav[$scope.deptNav.length - 1].id;
                 if ($scope.CacheData[id]) {
                     //$scope.Organizations = $scope.CacheData[id];
                     $scope.getCacheData(id);
                 } else {
                     $scope.getData(id);
                 }
                 $scope.showDeptID = id;
             }
         }
     };
 })
   //查询列表
.controller("sheetQueryCtrl", function ($rootScope, $scope, $state, $stateParams, $http, $ionicActionSheet, $ionicHistory, fcommonJS, $SheetQuery) {
    console.log($stateParams);
    $scope.choosedObjectId = $stateParams.objectid;
    //查询需要参数
    var sheetQuery = {
        controlManager: {}, //父控件实例
        dataField: "",
        rowNum: "",
        schemaCode: "",
        queryCode: "",
        filter: [],
        inputMappings: "",
        outputMappings: "",
    }
    $scope.OutputMapJson = {};
    $scope.InputMapJson = {};
    $scope.viewModel = [];
    $scope.displayColumns = [];
    $scope.columnNames = [];
    $scope.PageSize = 10; //分页数据
    $scope.NextPageIndex = 0; //当前页数，
    $scope.LoadFinished = false; //是否加载完成
    $scope.IsBindInputVlues = false;
    $scope.HasBindFilters = false;

    //初始化参数
    $scope.initParams = function () {
        sheetQuery.dataField = $stateParams.datafield;
        sheetQuery.rowNum = $stateParams.rownum;
        sheetQuery.controlManager = $.MvcSheetUI.GetElement(sheetQuery.dataField, sheetQuery.rowNum).SheetUIManager();
        if (sheetQuery.controlManager) {
            sheetQuery.schemaCode = sheetQuery.controlManager.SchemaCode;
            sheetQuery.queryCode = sheetQuery.controlManager.QueryCode;
            if (sheetQuery.controlManager.InputMappings) {
                sheetQuery.inputMappings = sheetQuery.controlManager.InputMappings;
            }
            if (sheetQuery.controlManager.OutputMappings) {
                sheetQuery.outputMappings = sheetQuery.controlManager.OutputMappings;
            }
        }
    }

    $scope.SetQueryValue = function (item) {
        console.log(item);
        //选中效果
        $scope.choosedObjectId = item.oldItem.ObjectID;
        console.log($scope.OutputMapJson);
        for (var key in $scope.OutputMapJson) {
            if (key == sheetQuery.dataField) {
                var objValue = $scope.OutputMapJson[key];
                var objArry = objValue.split(';');
                if (objArry && objArry.length > 0) {
                    var dataArry = [];
                    for (var j = 0; j < objArry.length; j++) {
                        dataArry[j] = item.oldItem[objArry[j]];
                    }
                    console.log(dataArry);
                    sheetQuery.controlManager.SetValue(dataArry);
                } else {
                    console.log(item.oldItem[$scope.OutputMapJson[key]]);
                    sheetQuery.controlManager.SetValue(item.oldItem[$scope.OutputMapJson[key]])
                }
                //当前控件，直接赋值
                //赋值后自动验证
                try {
                    sheetQuery.controlManager.Validate()
                } catch (e) { }
            } else {
                var e = $.MvcSheetUI.GetElement(key, sheetQuery.rowNum);
                if (e != null && e.data($.MvcSheetUI.SheetIDKey)) {
                    e.SheetUIManager().SetValue(item.oldItem[$scope.OutputMapJson[key]]);
                    if (e.SheetUIManager().Validate) {
                        e.SheetUIManager().Validate();
                    }
                }
            }
        }
        //$ionicHistory.goBack();
    }

    //读取inputmapping映射值
    $scope.GetInputMappings = function () {
        var inputJson = {};
        if ($scope.InputMapJson) {
            for (var key in $scope.InputMapJson) {
                if ($scope.InputMapJson[key])
                    if ($scope.InputMapJson[key].GetValue()!=="") {
                        inputJson[key] = $scope.InputMapJson[key].GetValue();
                    }
            }
        }
        return JSON.stringify(inputJson);
    }
    //处理传入参数映射配置，对应的值是控件的实例
    $scope.InputMappingSetting = function () {
        var mapping = sheetQuery.inputMappings.split(',');
        if (!mapping) { $scope.InputMapJson = null; }
        for (var i = 0; i < mapping.length; i++) {
            var map = mapping[i].split(':');
            var targetDataField = map[0];
            var e = $.MvcSheetUI.GetElement(targetDataField, sheetQuery.rowNum);
            if (e != null) {
                $scope.InputMapJson[map[1]] = e.SheetUIManager();
            }
        }
    }
    //处理映射配置
    $scope.MappingSetting = function () {
        var mapping = sheetQuery.outputMappings.split(',');

        for (var i = 0; i < mapping.length; i++) {
            var map = mapping[i].split(':');
            $scope.OutputMapJson[map[0]] = map[1];
        }
        $scope.InputMappingSetting();
    }

    //初始化参数
    $scope.initParams();
    $scope.MappingSetting();

    $scope.GetDisplayName = function (key) {
        if (!$scope.columnNames) return key;
        return $scope.columnNames[key] || key;
    }

    /// <summary>
    /// 控件类型
    /// </summary>
    $scope.ControlType = {
        /// <summary>
        /// 文本框类型
        /// </summary>
        TextBox: 0,
        /// <summary>
        /// 下拉框类型
        /// </summary>
        DropdownList: 1,
        /// <summary>
        /// 单选框类型
        /// </summary>
        RadioButtonList: 2,
        /// <summary>
        /// 复选框类型
        /// </summary>
        CheckBoxList: 3,
        /// <summary>
        /// 长文本框类型
        /// </summary>
        RichTextBox: 4
    }

    //绑定过滤条件控件
    $scope.BindFilter = function () {
        $scope.HasBindFilters = true;
        if (!sheetQuery.filter || sheetQuery.filter.length == 0) return;
        $scope.FilterPanelID = $.MvcSheetUI.NewGuid();
        this.FilterPanel = $("#divFilter");
        //添加过滤项
        var ulElement = $("<ul>").addClass("list").appendTo(this.FilterPanel);
        for (var i = 0; i < sheetQuery.filter.length; i++) {
            var filterItem = sheetQuery.filter[i];
            if (!filterItem.Visible) continue; //不可见
            if (filterItem.FilterType == 3) continue; //系统参数

            var defaultVal = filterItem.DefaultValue;
            if (this.InputMapJson[filterItem.PropertyName]) {
                //传入参数
            }
            var liElement = $("<li>").appendTo(ulElement).addClass("item item-input");
            var label = $("<label for='" + $scope.FilterPanelID + filterItem.PropertyName + "'>" + $scope.GetDisplayName(filterItem.PropertyName) + "</label>").addClass("input-label");
            liElement.append(label);
            switch (filterItem.DisplayType) {
                case $scope.ControlType.DropdownList:
                    var input = $("<select id='" + $scope.FilterPanelID + filterItem.PropertyName + "' data-property='" + filterItem.PropertyName + "'>");
                    input.append("<option value=''>" + SheetLanguages.Current.All + "</option>");
                    var vals = filterItem.SelectedValues.split(";");
                    for (var j = 0; j < vals.length; j++) {
                        if (vals[j] != "") {
                            input.append("<option value='" + vals[j] + "'>" + vals[j] + "</option>");
                        }
                    }
                    input.val(filterItem.DefaultValue);
                    liElement.append(input);
                    break;

                case $scope.ControlType.RadioButtonList:
                    var vals = filterItem.SelectedValues.split(";");
                    for (var j = 0; j < vals.length; j++) {
                        if (vals[j] != "") {
                            var newid = $.MvcSheetUI.NewGuid();
                            liElement.append("<input id='" + newid + "' type='radio' name='" + filterItem.PropertyName + "' value='" + vals[j] + "'></input>");
                            liElement.append("<label for='" + newid + "'>" + vals[j] + "</label>");
                        }
                    }
                    $("input[name='" + filterItem.PropertyName + "'][value='" + filterItem.DefaultValue + "']").attr("checked", "checked")
                    liElement.append("<br style='clear: both;'></br>");
                    break;

                case $scope.ControlType.CheckBoxList:
                    var vals = filterItem.SelectedValues.split(";");
                    for (var j = 0; j < vals.length; j++) {
                        if (vals[j] != "") {
                            var newid = $.MvcSheetUI.NewGuid();
                            liElement.append("<input id='" + newid + "' type='checkbox' name='" + filterItem.PropertyName + "' value='" + vals[j] + "'></input>");
                            liElement.append("<label for='" + newid + "'>" + vals[j] + "</label>");
                        }
                    }
                    $("input[name='" + filterItem.PropertyName + "'][value='" + filterItem.DefaultValue + "']").attr("checked", "checked")
                    liElement.append("<br style='clear: both;'></br>");
                    break;

                default:
                    //Error:文本类型，需要判断FilterType 和 LogicType,日期、数字 范围
                    liElement.append("<input type='text' id='" + $scope.FilterPanelID + filterItem.PropertyName + "' data-property='" + filterItem.PropertyName + "' placeholder='" + $scope.GetDisplayName(filterItem.PropertyName) + "' autocomplete='off'></input>");
                    $("#" + filterItem.PropertyName).val(filterItem.DefaultValue);
                    break;
            }
        }
    }
    //绑定过滤条件的传入数据,显示时
    $scope.BindFilterInputValues = function () {
        $scope.IsBindInputVlues = true;
        for (var i = 0; i < sheetQuery.filter.length; i++) {
            var filterItem = sheetQuery.filter[i];
            if (!filterItem.Visible) continue; //不可见
            if (filterItem.FilterType == 3) continue; //系统参数
            if (!$scope.InputMapJson[filterItem.PropertyName]) continue;
            switch (filterItem.DisplayType) {
                case $scope.ControlType.RadioButtonList:
                case $scope.ControlType.CheckBoxList:
                    $scope.FilterPanel.find("input[name='" + filterItem.PropertyName + "'][value='" + $scope.InputMapJson[filterItem.PropertyName].GetValue() + "']").attr("checked", "checked");
                    break;
                default:
                    $("#" + $scope.FilterPanelID + filterItem.PropertyName).val($scope.InputMapJson[filterItem.PropertyName].GetValue());
                    break;
            }
        }
    }

    //读取过滤数据，查询时
    $scope.GetFilters = function () {
        var filters = {};
        for (var i = 0; i < sheetQuery.filter.length; i++) {
            var filterItem = sheetQuery.filter[i];
            if (!filterItem.Visible) continue; //不可见
            if (filterItem.FilterType == 3) continue; //系统参数
            switch (filterItem.DisplayType) {
                case $scope.ControlType.RadioButtonList:
                case $scope.ControlType.CheckBoxList:
                    if ($scope.FilterPanel.find("input[name='" + filterItem.PropertyName + "']:checked").val()) {
                        filters[filterItem.PropertyName] = $("input[name='" + filterItem.PropertyName + "']:checked").val();
                    }
                    break;
                default:
                    if ($("#" + $scope.FilterPanelID + filterItem.PropertyName).val()) {
                        filters[filterItem.PropertyName] = $("#" + this.FilterPanelID + filterItem.PropertyName).val();
                    }
                    break;
            }
        }
        return JSON.stringify(filters);
    }

    $scope.getPropertyNameFromData = function (bizObject, propertyName) {
        for (var k in bizObject) {
            if (k.toLocaleLowerCase() == propertyName.toLocaleLowerCase()) {
                return k;
            }
        }
    }

    //从后台读取数据后，绑定到前端
    $scope.BindData = function (data) {
        console.log(data);
        //列显示
        if (data) {
            //需要显示的列
            if (data.QuerySetting) {
                for (var index in data.QuerySetting.Columns) {
                    if (data.QuerySetting.Columns[index].Visible == 1) {
                        $scope.displayColumns.push(data.QuerySetting.Columns[index].PropertyName);
                    }
                }
            }
            //列编码和显示名称
            $scope.columnNames = data.Columns;
            //当前数据项需要显示的字段
            var NameKey = $scope.OutputMapJson[sheetQuery.dataField];
            var tmpArray = [];
            //显示视图
            for (var index in data.QueryData) {
                var itemName;
                var i = 0;
                var d = data.QueryData[index];
                var summary = "";
                for (var key in d) {
                    if ($scope.displayColumns.indexOf(key) > -1) {
                        //if (!NameKey && i == 0) {
                        //    itemName = d[key];
                        //    continue;
                        //} else if (key == NameKey) {
                        //    itemName = d[key];
                        //    continue;
                        //}
                        var val = d[key] == null ? "" : d[key];
                        summary += $scope.columnNames[key] + ":" + val + "<br/>";
                        i++;

                    }
                }

                tmpArray.push({ itemName: itemName, summary: summary, oldItem: d });
            }
            $scope.LoadFinished = data.LoadFinished;
            //合并数组
            $scope.viewModel.splice($scope.NextPageIndex * 10, tmpArray.length);
            for (var i = $scope.NextPageIndex * 10,j = 0; i < $scope.NextPageIndex * 10 + tmpArray.length; i++,j++) {
                $scope.viewModel.splice(i, 0, tmpArray[j]);
            }
            //页数加1,改变NetWorkFlag的状态##
            if (!data.LoadFinished) {
                $scope.NextPageIndex += 1;
                $scope.NetWorkFlag.index = $scope.NextPageIndex;
                $scope.NetWorkFlag.status = false;
            } 


            sheetQuery.filter = data.QuerySetting.QueryItems;
            if (!$scope.HasBindFilters) {
                $scope.BindFilter();
            }

            if (!$scope.IsBindInputVlues) {
                $scope.BindFilterInputValues();
            }
        }
    };
    //从后台读取数据
    $scope.NextPageIndex = 0;
    //防止发送重复的请求##
    $scope.NetWorkFlag = {
        index: 0,
        status:false
    };
    $scope.viewModel = [];
    $scope.LoadQueryData = function (isSearch, InputMapping) {
        var localInpuptMapping
        if (InputMapping) {
            localInpuptMapping = InputMapping;
        } else {
            localInpuptMapping = $scope.GetInputMappings();
        }
        if (!isSearch) { localInpuptMapping = {}; }
        var params = {
            Action: "GetQuerySettingAndData",
            SchemaCode: sheetQuery.schemaCode,
            QueryCode: sheetQuery.queryCode,
            InputMapping: localInpuptMapping,
            PageSize: $scope.PageSize,
            NextPageIndex: $scope.NextPageIndex
        };
        //筛选数据
        if (sheetQuery.filter.length > 0) {
            //params["Action"] = "GetQueryData";
            //如何没绑定inputmapping的值，得绑定 
            params["Filters"] = $scope.GetFilters();
        }

        if ($scope.NextPageIndex == $scope.NetWorkFlag.index && !$scope.NetWorkFlag.status) {
            //已经发送了请求不再重复发送请求##
            $scope.NetWorkFlag.status = true;
            var promise = $SheetQuery.QueryData(params);
            promise.then(function (data) {
                $scope.$broadcast('scroll.infiniteScrollComplete');
                $scope.BindData(data)
            })
        } else {
            console.log("repeat network!");
        }
    };
    $scope.conditionLoadQueryData = function (isSearch) {
        $scope.NextPageIndex = 0;
        $scope.viewModel = [];
        $scope.NetWorkFlag = {
            index: 0,
            status: false
        };
        var InputMapping = {};
        $("[data-property]").each(function (i, v) {
            if ($(v).val()!=="") {
                InputMapping[$(v).data("property")] = $(v).val();
            }
        })
        console.log(InputMapping);
        $scope.LoadQueryData(isSearch, InputMapping);
    }
    //$scope.RefreshData = function (isSearch) {
    //    var localInpuptMapping = $scope.GetInputMappings();
    //    if (isSearch) { localInpuptMapping = {}; }
    //    var params = {
    //        Action: "GetQuerySettingAndData",
    //        SchemaCode: sheetQuery.schemaCode,
    //        QueryCode: sheetQuery.queryCode,
    //        InputMapping: localInpuptMapping,
    //        PageSize: $scope.PageSize,
    //        NextPageIndex: $scope.NextPageIndex
    //    };
    //    //筛选数据
    //    if (sheetQuery.filter.length > 0) {
    //        //params["Action"] = "GetQueryData";
    //        //如何没绑定inputmapping的值，得绑定
    //        params["Filters"] = $scope.GetFilters();
    //    }
    //    var promise = $SheetQuery.QueryData(params);
    //    promise.then(function (data) {
    //        $scope.BindData(data);
    //        $scope.$broadcast('scroll.infiniteScrollComplete');
    //    })
    //}
    $scope.goBack = function () {
        window.history.back();
    }

    // 每次进入View时触发
    $scope.$on("$ionicView.enter", function (scopes, states) {
        if ($rootScope.dingMobile.isDingMobile) {
            //设置header 右边按钮
            dd.biz.navigation.setMenu({
                items: [
                    {
                        "id": "1",//字符串
                        "text": "查询"
                    }
                ],
                onSuccess: function (data) {
                    $scope.conditionLoadQueryData(true);
                }
            });
        }
        $scope.LoadQueryData(true);
    });

})
    //传阅、转发。。。
.controller('fetchUserCtrl', ['$rootScope', '$scope', '$ionicHistory', '$ionicPopup', function ($rootScope, $scope, $ionicHistory, $ionicPopup) {
    // 每次进入View时触发
    $scope.$on("$ionicView.enter", function (scopes, states) {
        $scope.Params = $.MvcSheetUI.actionSheetParam;
        $("#fetchUserSelect").html();
        $scope.SheetUser = $("#fetchUserSelect").SheetUser($scope.Params.ueroptions);
        if (!$scope.SheetUser && $rootScope.fetchUserSelect) {
            $scope.SheetUser = $rootScope.fetchUserSelect;
        }
        $($(".fetchUserContainer")[0].children).show();
        $($(".fetchUserContainer")[0]).find("span").attr("style", "width: auto;word-break: normal;word-wrap: break-word;white-space: initial;")
    });

    //SelectUser完成事件
    $rootScope.$on('sheetUserFinished', function (event, data) {
        $scope.fetchUserSelect = data.obj;
        $rootScope.fetchUserSelect = $scope.SheetUser;
    });

    $scope.doAction = function () {
        var Datas = [];
        if (angular.isUndefined($scope.fetchUserSelect) || (angular.isArray($scope.fetchUserSelect) && $scope.fetchUserSelect.length == 0)) {
            alert($scope.Params.title);
            return;
        }
        else if (angular.isArray($scope.fetchUserSelect)) {
            var datas = "";
            angular.forEach($scope.fetchUserSelect, function (data, index, full) {
                if (datas == "") {
                    datas = data.Code;
                } else {
                    datas = datas + "," + data.Code;
                }
            });
            Datas.push(datas);
        } else {
            var data = $scope.fetchUserSelect.Code;
            if (data != "")
                Datas.push(data);
        }
        Datas.push(false)
        var action = {
            Action: $scope.Params.Action,
            Datas: Datas
        };
        $.MvcSheet.Action(action);
        $ionicHistory.goBack();
    }
}])
    //流程状态
.controller('instanceStateCtrl', ['$rootScope', '$scope', '$stateParams', '$http', '$ionicScrollDelegate', '$ionicHistory', 'fcommonJS', function ($rootScope, $scope, $stateParams, $http, $ionicScrollDelegate, $ionicHistory, fcommonJS) {
    $scope.$on("$ionicView.enter", function (scopes, states) {
    });

    if ($rootScope.dingMobile.isDingMobile) {
        //$scope.SetDingDingHeader("流程状态");
    }
    //保证上下滑动
    $scope.scrollHeight = { 'height': document.body.scrollHeight + "px" };

    console.log($stateParams);
    $scope.Mode = $stateParams.Mode;
    $scope.InstanceID = $stateParams.InstanceID;
    $scope.WorkflowCode = $stateParams.WorkflowCode;
    $scope.WorkflowVersion = $stateParams.WorkflowVersion;
    $scope.closePage = function () {
        // window.history.back();
        $ionicHistory.goBack();
        console.log("haha");
    }
    if ($scope.Mode == 3) {
        $scope.IsOriginate = true;
    } else {
        $scope.IsOriginate = false;
    }

    $scope.init = function () {
        $scope.type = "base";
        if ($scope.IsOriginate) {
            MobileLoader.ShowWorkflow($scope.InstanceID, $scope.WorkflowCode, $scope.WorkflowVersion, _PORTALROOT_GLOBAL);
        } else {
            MobileLoader.ShowWorkflow($scope.InstanceID, "", -1, _PORTALROOT_GLOBAL);
        }
        fcommonJS.loadingHide();
    }
    $scope.init();
}])
    //
.controller('downLoadFileCtrl', ['$rootScope', '$scope', '$location', '$http', '$stateParams', '$ionicHistory', '$window', function ($rootScope, $scope, $location, $http, $stateParams, $ionicHistory, $window) {
    $scope.$on("$ionicView.enter", function (scopes, states) {
        $scope.url = $rootScope.AttachmentUrl;
        $scope.extension = $stateParams.extension;
        var pic_extension = ".jpg,.gif,.jpeg,.png";
        var txt_extension = ".txt";
        var mp3_extension = ".mp3";
        $scope.isImg = pic_extension.indexOf($scope.extension) > -1 ? true : false;
        $scope.isTxt = txt_extension.indexOf($scope.extension) > -1 ? true : false;
        $scope.isMp3 = mp3_extension.indexOf($scope.extension) > -1 ? true : false;
    });
}])


