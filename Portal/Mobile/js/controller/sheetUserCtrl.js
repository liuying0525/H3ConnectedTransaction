module.controller('sheetUserCtrl', function ($rootScope, $scope, $ionicActionSheet, $state, $stateParams, $ionicBackdrop, $timeout, $ionicModal, $ionicSideMenuDelegate, $ionicHistory, sheetUserService) {
    $scope.sheetUserHandler = "/Portal/SheetUser/LoadOrgTreeNodes";
    $scope.SelectFormStructure = true;
    $scope.init = function () {
        $scope.UserOUMembers = [];//所在部门人员    
        $scope.Organizations = []; //部门成员    
        $scope.deptNav = [];  //导航数据项  { id;"",name:"",index:""}
        $scope.showDeptID = "";//当前部门id        
        $scope.CacheData = {};//缓存数据   
        $scope.SelectItems = [];//已选
        $scope.checkedstetus = true;//是否全选
        //搜索相关
        $scope.searchKey = "";
        $scope.SearchMode = false;
        $scope.searchedKeys = [];
        $scope.searchItems = [];//搜索结果
        $scope.searchedItems = [];//搜索缓存数据  
        //搜索展示人员还是部门
        $scope.SearchEmp = false;
        $scope.SearchDep = false;
        $scope.sheetUserParams = {
            selecFlag: "U",
            initUsers:$rootScope.filterUsers,
            isMutiple: true,
            loadOptions: "o=U",
            loadUrl: "/Portal/SheetUser/LoadOrgTreeNodes?IsMobile=true&Recursive=true",
            options: {
                RootSelectable: true,
                OrgUnitVisible: "",
                UserVisible: true
            }
        };
        //$scope.getData("");
    };
    $scope.$on("$ionicView.enter", function (scopes, states) {
        //设置钉钉头部
        if ($rootScope.dingMobile.isDingMobile) {
            $scope.SetDingDingHeader("发起人");
            dd.biz.navigation.setRight({
                show: true,//控制按钮显示， true 显示， false 隐藏， 默认true
                control: true,//是否控制点击事件，true 控制，false 不控制， 默认false
                text: "关闭",//控制显示文本，空字符串表示显示默认文本
                onSuccess: function (result) {
                    $scope.cancel();
                },
                onFail: function (err) { }
            });
            dd.biz.navigation.setLeft({
                show: true,//控制按钮显示， true 显示， false 隐藏， 默认true
                control: true,//是否控制点击事件，true 控制，false 不控制， 默认false
                text: "关闭",//控制显示文本，空字符串表示显示默认文本
                onSuccess: function (result) {
                    $scope.cancel();
                },
                onFail: function (err) { }
            });
        }
        $scope.ShowCurrentDept = true;
        $scope.init();
        $scope.InitOUMembers();
        
        $scope.SelectItems = sheetUserService.initItems($scope.sheetUserParams.initUsers);
        //设置已选-从组织架构中选择
        $scope.selectedName = sheetUserService.getSelectedName($scope.SelectItems);
       
        if (!$scope.ShowCurrentDept) {
            $scope.SelectStructure(true);
        } else {
            $scope.SelectFormStructure = false;
            $scope.checkedpagestaue();
        }
    });

    //初始化本部门人员
    $scope.InitOUMembers = function () {
        if (!$scope.InitUserOUMembers) {
            $scope.InitUserOUMembers = [];
            var parentUnits = $scope.user.DirectParentUnits;
            for (var key in parentUnits) {
                sheetUserService.loadData($scope.sheetUserHandler + "?IsMobile=true&ParentID=" + key + "&o=U", null).then(function (res) {
                    var filterdata = $.grep(res, function (value) {
                        if (value.ExtendObject.UnitType == "U") {
                            return value;
                        }
                    });
                    $scope.InitUserOUMembers = $scope.InitUserOUMembers.concat(filterdata);
                    //获取所在部门人员
                    $scope.UserOUMembers = sheetUserService.sheetUserAdapter($scope.InitUserOUMembers, $scope.sheetUserParams.selecFlag);
                    //所在部门设置已选-所在部门
                    $scope.UserOUMembers = sheetUserService.checkItems($scope.UserOUMembers, $scope.SelectItems);
                });
            };
        } else {
            $scope.UserOUMembers = sheetUserService.sheetUserAdapter($scope.InitUserOUMembers, $scope.sheetUserParams.selecFlag);
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
        if ($scope.deptNav.length && $scope.deptNav[$scope.deptNav.length - 1].id == org.id) {
            return;
        }
        var Percent = e.clientX / screen.availWidth * 100;
        if (org.type != "U" && org.root != true && !$scope.SearchMode && (!org.canSelect || Percent > 15)) {
            //展开部门  
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
                $scope.checkedstetus = true;//是否全选
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
       // console.log(deleteItem)
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
            $scope.Organizations = sheetUserService.checkItems($scope.Organizations, $scope.SelectItems);
            $scope.initcheckedStetus($scope.Organizations, $scope.SelectItems.length);//改变全选按钮状态
            $scope.CacheData[parentid] = $scope.Organizations;
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
        //重写方法
        var objs = sheetUserService.convertItems($scope.SelectItems);
        $rootScope.filterUsers = objs;
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
        if ($scope.searchKey != "" && Percent >20) {
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
        else if (!$scope.SelectFormStructure&& $scope.SelectItems.length == 0) {//当没有选中的情况下，相当于要把当前部门取消全选
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
        if ($scope.checkedstetus) {//全选
            if ($scope.Organizations.length == 0 ||!$scope.SelectFormStructure) {
                $scope.checkedObj($scope.UserOUMembers, true);
            }
            else {
                $scope.checkedObj($scope.Organizations, true);
            }
            $scope.checkedstetus = false;
        } else {
            if ($scope.Organizations.length == 0 ||!$scope.SelectFormStructure) {
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
                $scope.checkedpagestaue();
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
});
