app.controller('RuleNameController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$modal", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $modal, $state, $stateParams, $filter, ControllerConfig) {

        $scope.Datas = [];
        $scope.Total = 0;
        $scope.TotalPage = 1;
        $scope.Page = 1;
        $scope.Pid = $stateParams.Objectid;
        $scope.Min = true;
        $scope.Max = true;
        $scope.Size = 10;
        $scope.IsBase = false;
        $scope.checkAll = false;
        $scope.SearchKey = {
            ClassName: '',
        };

        $scope.$on('$viewContentLoaded', function (event) {

            $scope.init();
        });

        $scope.init = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);

        }

        $scope.clickSearch = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
        }

        $scope.search = function (page) {
            $http({
                method: 'post',
                url: '/Portal/GetRuleName/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&ClassName=' + $scope.SearchKey.ClassName + '&Pid=' + $scope.Pid,//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleName.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&ClassName=' + $scope.SearchKey.ClassName + '&Pid=' + $scope.Pid,
                data: { page: page, size: $scope.Size }
            }).success(function (req) {

                $scope.Datas = req.Data;
                $scope.Total = req.Total;
                var totalPage = parseInt(parseInt(req.Total) / $scope.Size) + (parseInt(req.Total) % $scope.Size > 0 ? 1 : 0);
                $scope.TotalPage = totalPage <= 0 ? 1 : totalPage;
                $scope.SetPageButton();
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }

        $scope.SetPageButton = function () {
            $scope.Min = $scope.Page == 1;
            $scope.Max = $scope.Page == $scope.TotalPage;
        }

        $scope.CheckMax = function () {
            if ((parseInt($scope.Page) > 0) == false) $scope.Page = 1;
            if (parseInt($scope.Page) > $scope.TotalPage)
                $scope.Page = $scope.TotalPage;
            $scope.search($scope.Page);
        }

        $scope.ModPage = function (Value) {
            $scope.Page = parseInt($scope.Page) + parseInt(Value);
            $scope.search($scope.Page);
        }

        $scope.selectAll = function () {
            if ($scope.checkAll) {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    $scope.Datas[i].checked = true;
                }
            } else {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    $scope.Datas[i].checked = false;
                }
            }
        }

        $scope.select = function (index) {
            console.log(index, $scope.Datas[index].checked)
            if ($scope.Datas[index].checked == true) {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    if ($scope.Datas[i].checked == false) {
                        $scope.checkAll = false;
                        return;
                    }
                }
                $scope.checkAll = true;
            } else {
                $scope.checkAll = false;
            }
        }

        $scope.Delete = function (index) {
            var deleteArray = [];
            if (index == undefined || index == null) {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    if ($scope.Datas[i].checked) {
                        deleteArray.push({
                            ObjectID: $scope.Datas[i].ObjectID,

                        })
                    }
                }
                if (deleteArray.length <= 0) {
                    alert('您未选择任何抵押规则。');
                    return;
                }
                else if (confirm('是否删除选中的' + deleteArray.length + '项？执行该操作后无法恢复，请谨慎处理。')) {
                } else {
                    return;
                }
            } else {
                if (confirm('是否删除性质【' + $scope.Datas[index].BClassName + '】的规则名称？执行该操作后无法恢复，请谨慎处理。')) {
                    deleteArray.push({
                        ObjectID: $scope.Datas[index].ObjectID,

                    })
                } else {
                    return;
                }
            }

            $http({
                method: 'post',
                //url: '/Portal/Ajax/DelRuleName.ashx',
                url: '/Portal/DelRuleName/Index',// wangxg 19.7
                data: deleteArray
            }).success(function () {
                alert('删除成功。');
                $scope.search($scope.Page);
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }

        $scope.clear = function () {
            $scope.SearchKey = {
                ProductName: '',
                ProductAlias: '',
                Description: '',
                State: '',
                Dealer: ''
            };
            $scope.clickSearch();
        }

        $scope.New = function (index) {
            var Objectid = "";
            var pid = "";
            pid = $scope.Pid;
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.RuleDetaile', { "Objectid": Objectid, "Pid": pid })
        }
        $scope.Rank = function (index) {
            var Objectid = "";
            var pid = "";
            pid = $scope.Pid;
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.ScoringRank', { "Objectid": Objectid, "Cid": pid })
        }
        $scope.Back = function (index) {
            $state.go('app.RuleProperty')
        }
        $scope.btn_addAgents = function (index) {
            var Pid = $stateParams.Objectid;
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }

            $http({
                url: '/Portal/GetRuleName/Index?Pid=' + Pid + '&GetState=1',//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleName.ashx?Pid=' + Pid + '&GetState=1',
                data: {}
            }).success(function (result) {

                var BClassName = result;
                var Rate = "";
                var Pid = $scope.Pid;
                var ClassName = "";
                var ObjectID = "";

                // 弹出模态框

                var modalInstance = $modal.open({
                    templateUrl: 'EditRuleName.html',    // 指向上面创建的视图
                    controller: 'EditRuleNameController',// 初始化模态范围
                    size: "md",
                    resolve: {
                        params: function () {
                            return {
                                BClassName: BClassName,
                                ClassName: ClassName,
                                Pid: Pid,
                                Rate: Rate,
                                ObjectID: ObjectID
                            };
                        },
                        deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                            return $ocLazyLoad.load([
                                'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                'WFRes/assets/stylesheets/sheet.css',
                                'WFRes/_Scripts/jquery/jquery.lang.js',
                                'WFRes/_Scripts/ligerUI/ligerui.all.js',
                                'WFRes/_Scripts/MvcSheet/SheetControls.js',
                                'WFRes/_Scripts/MvcSheet/MvcSheetUI.js'
                            ]).then(function () {
                                return $ocLazyLoad.load([
                                    'WFRes/_Scripts/MvcSheet/Controls/SheetWorkflow.js',
                                    'WFRes/_Scripts/MvcSheet/Controls/SheetUser.js'
                                ])
                            });
                        }]
                    }
                });
            })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }
        $scope.btn_updateAgents = function (index) {
            var Objectid = "";
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }

            // 弹出模态框

            var modalInstance = $modal.open({
                templateUrl: 'EditRuleName.html',    // 指向上面创建的视图
                controller: 'EditRuleNameController',// 初始化模态范围
                size: "md",
                resolve: {
                    params: function () {
                        return {
                            BClassName: $scope.Datas[index].BClassName,
                            ClassName: $scope.Datas[index].ClassName,
                            Rate: $scope.Datas[index].Rate,
                            ObjectID: $scope.Datas[index].ObjectID
                        };
                    },
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                            'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                            'WFRes/assets/stylesheets/sheet.css',
                            'WFRes/_Scripts/jquery/jquery.lang.js',
                            'WFRes/_Scripts/ligerUI/ligerui.all.js',
                            'WFRes/_Scripts/MvcSheet/SheetControls.js',
                            'WFRes/_Scripts/MvcSheet/MvcSheetUI.js'
                        ]).then(function () {
                            return $ocLazyLoad.load([
                                'WFRes/_Scripts/MvcSheet/Controls/SheetWorkflow.js',
                                'WFRes/_Scripts/MvcSheet/Controls/SheetUser.js'
                            ])
                        });
                    }]
                }
            });


            $http({
                url: '/Portal/GetRuleName/Index?ObjectID=' + Objectid,//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleName.ashx?ObjectID=' + Objectid,
                data: {}
            })
                .success(function (result) {
                    var BClassName = "";
                    var Rate = "";
                    var ClassName = "";
                    var ObjectID = "";
                    if (result.Data.length > 0 && result.Data[0]["BClassName"] != undefined)
                        BClassName = result.Data[0]["BClassName"];

                    if (result.Data.length > 0 && result.Data[0]["ClassName"] != undefined)
                        ClassName = result.Data[0]["ClassName"];
                    if (result.Data.length > 0 && result.Data[0]["Rate"] != undefined)
                        Rate = result.Data[0]["Rate"];
                    if (result.Data.length > 0 && result.Data[0]["ObjectID"] != undefined)
                        ObjectID = result.Data[0]["ObjectID"];

                })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }

        $scope.btn_updateComputer = function (index) {
            var Objectid = "";
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            var modalInstance = $modal.open({
                templateUrl: 'EditComputer.html',    // 指向上面创建的视图
                controller: 'EditComputerController',// 初始化模态范围
                size: "md",
                resolve: {
                    params: function () {
                        return {
                            ObjectID: Objectid
                        };
                    },
                    deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                        return $ocLazyLoad.load([
                            'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                            'WFRes/assets/stylesheets/sheet.css',
                            'WFRes/_Scripts/jquery/jquery.lang.js',
                            'WFRes/_Scripts/ligerUI/ligerui.all.js',
                            'WFRes/_Scripts/MvcSheet/SheetControls.js',
                            'WFRes/_Scripts/MvcSheet/MvcSheetUI.js'
                        ]).then(function () {
                            return $ocLazyLoad.load([
                                'WFRes/_Scripts/MvcSheet/Controls/SheetWorkflow.js',
                                'WFRes/_Scripts/MvcSheet/Controls/SheetUser.js'
                            ])
                        });
                    }]
                }
            });
        }

    }]);

app.controller("EditRuleNameController", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params",
    function ($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params) {

        $scope.getLanguage = function () {
            $scope.LanJson = {

            }
        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });



        $scope.BClassName = {
            Editable: false, Visiable: true,
        }
        $scope.init = function () {

            //state = 1;
            $scope.ObjectID = params.ObjectID;
            //$scope.Code.Editable = false;
            $scope.BClassName = params.BClassName;
            $scope.Rate = params.Rate;
            $scope.Pid = params.Pid;
            //$scope.StartTime = params.fTime;
            $scope.ClassName = params.ClassName;
            //$scope.WasAgentOptions.V = params.teacheID;
        }
        $scope.init();

        $scope.ok = function () {
            if ($scope.ClassName == "" || $scope.ClassName == null || $scope.ClassName == undefined) {
                alert("请填写名称");
                return;
            }
            if ($scope.Rate == "" || $scope.Rate == null || $scope.Rate == undefined) {
                alert("请填写权重");
                return;
            }
            if (isNaN($scope.Rate)) {
                alert("权重只能为数字");
                return;
            }
            $http({
                url: '/Portal/GetRuleName/Index?method=AddRuleName',//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleName.ashx?method=AddRuleName',
                params: {
                    BClassName: $scope.BClassName,
                    ClassName: $scope.ClassName,
                    Rate: $scope.Rate,
                    Pid: $scope.Pid,
                    ObjectID: $scope.ObjectID,
                }
            })
                .success(function (result, header, config, status) {
                    if (result == 1) {
                        $modalInstance.close();
                        $state.go($state.$current.self.name, {}, { reload: true });
                    }
                    else {
                        $scope.FailMessage = $scope.LanJson.InvalidAgency;
                        var ctrl = angular.element(document.querySelector("#EditError"));
                        notify.showMessage(ctrl);
                        return;
                    }
                })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel'); // 退出
        }
    }])


app.controller("EditComputerController", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params", "toastr",
    function ($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params, toastr) {

        $scope.getLanguage = function () {
            $scope.LanJson = {

            }
        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        $scope.data = [];
        $scope.items = [];

        $scope.BClassName = {
            Editable: false, Visiable: true,
        }
        $scope.init = function () {
            $scope.ObjectID = params.ObjectID;
            $http({
                url: '/Portal/SetComputer/Index?id=' + $scope.ObjectID,//wangxg 19.7
                //url: '/Portal/Ajax/SetComputer.ashx?id=' + $scope.ObjectID,

            }).success(function (ret) {

                $scope.data = ret["data"];

                for (var i = 0; i < $scope.data.length; i++) {
                    $scope.data[i].count = 0;
                }

                $scope.items = [];
                var str = ret["expression"].split("");
                var tmpStr = "";
                for (var i = 0; i < str.length; i++) {
                    if (tmpStr != "") {
                        if (str[i] == "}") {
                            var index = -1;
                            for (var j = 0; j < $scope.data.length; j++) {
                                if ($scope.data[j].id == tmpStr.substr(1)) {
                                    index = j;
                                    $scope.data[j].count++;
                                    break;
                                }
                            }
                            $scope.items.push({
                                class: 'VarExp',
                                value: index.toString()
                            })
                            tmpStr = "";

                        } else {
                            tmpStr += str[i];
                        }
                        continue;
                    }
                    if (str[i] == "{") {
                        tmpStr += str[i];
                    } else {
                        $scope.items.push({
                            class: 'TextExp',
                            value: str[i]
                        })
                    }
                }
            })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }
        $scope.init();

        $scope.ok = function () {
            var s = "";
            var result = "";
            var s = $scope.check();
            if (!s)
                return;
            for (var i = 0; i < $scope.items.length; i++) {
                if ($scope.items[i].class == 'TextExp') {
                    result += $scope.items[i].value;
                } else {
                    if ($scope.items[i].value == -1) {
                        result += '0';
                    } else {
                        result += '{' + $scope.data[$scope.items[i].value].id + '}';
                    }
                }
            }
            var real= result.replace(/×/, "*");
             real = real.replace(/÷/, "/");
            var data = {
                id: $scope.ObjectID,
                exp: real,
            }
            $http({
                method: 'post',
                url: '/Portal/SetComputer/Index',//wangxg 19.7
                //url: '/Portal/Ajax/SetComputer.ashx',
                data: data,
            }).success(function (ret) {
                if (ret == "success") {
                    alert("保存成功！");
                }
                else {
                    alert("保存失败请稍后再试！");
                }
                $modalInstance.dismiss('cancel'); // 退出
            }
            ).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        };

        $scope.SetValue = function (s) {
            $scope.items.push({
                class: 'TextExp',
                value: s.toString()
            })
        }

        $scope.GetValue = function (s) {
            $scope.data[s].count++;
            $scope.items.push({
                class: 'VarExp',
                value: s.toString()
            })
        }

        $scope.RemoveLast = function () {
            if ($scope.items[$scope.items.length - 1].class == 'VarExp') {
                $scope.data[$scope.items[$scope.items.length - 1].value].count--;
            }
            $scope.items.splice($scope.items.length - 1, 1);
        }

        $scope.RemoveAll = function () {
            $scope.items = [];
            for (var i = 0; i < $scope.data.length; i++) {
                $scope.data[i].count = 0;
            }
        }
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel'); // 退出
        }
        
        $scope.openModal = function(text){
        	$modal.open({  
                    template : text,  
                    controller : function(){},  
                });  
        }

        $scope.default = function () {
            var items = [];
            var data = [];
            for (var i = 0; i < $scope.data.length; i++) {
                items.push({
                    class: 'VarExp',
                    value: i
                });
                items.push({
                    class: 'TextExp',
                    value: '+'
                });
                var tmp = $scope.data[i];
                tmp.count = 1;
                data.push(tmp);
            }
            items.splice(items.length - 1, 1);
            $scope.items = items;
            $scope.data = data;
        }

        $scope.showErrorMessage = function(prePreItem, preItem, item, nextItem, nextNextItem){
            var message = "";
            if (prePreItem == null || prePreItem == undefined) { }
            else if(prePreItem.class == 'VarExp'){
                message += '['+$scope.data[prePreItem.value].name+']';
            }else{
                message += prePreItem.value;
            }

            if (preItem == null || preItem == undefined) { }
            else if (preItem.class == 'VarExp') {
                message += '[' + $scope.data[preItem.value].name + ']';
            }else{
                message += preItem.value;
            }

            message += '  →' + item + '←  ';

            if (nextItem == null || nextItem == undefined) { }
            else if (nextItem.class == 'VarExp') {
                message += '[' + $scope.data[nextItem.value].name + ']';
            }else{
                message += nextItem.value;
            }

            if (nextNextItem == null || nextNextItem == undefined) { }
            else if (nextNextItem.class == 'VarExp') {
                message += '[' + $scope.data[nextNextItem.value].name + ']';
            }else{
                message += nextNextItem.value;
            }
            toastr.error('参考位置:\r\n' + message, '公式不正确，请仔细确认。');
        }

        $scope.check = function () {
            console.log('原items:', $scope.items);

            var trueItems = [];

            var pre = '';
            var num = '';
            for (var i = 0; i < $scope.items.length; i++) {
                if (num != '') {
                    if ($scope.items[i].class == 'TextExp' && '0123456789.'.indexOf($scope.items[i].value) >= 0) {
                        num += $scope.items[i].value;
                        continue;
                    } else {
                        if (num.indexOf('.') == 0 || (num.indexOf('.') >= 0 && num.lastIndexOf('.') != num.indexOf('.'))) {
                            $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, num, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                            return false;
                        }
                        var tmp = parseFloat(num).toString().split('.');
                        if (tmp[0].length > 8) {
                            $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, num, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                            return false;
                        }
                        else if (tmp.length >= 2 && tmp[1].length > 5) {
                            $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, num, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                            return false;
                        }

                        // 在这加判断
                        if (tmp.length > 1 && tmp[0] == '0' && trueItems[trueItems.length - 1].value == '÷') {
                            $scope.showErrorMessage(trueItems.length >= 3 ? trueItems[trueItems.length - 3] : null, trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems[trueItems.length - 1].value + '' + num, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                            return false;
                        }

                        var numArray = tmp[0].split('').concat(tmp.length >= 2 ? ['.'] : []).concat(tmp.length >= 2 ? tmp[1].split('') : []);
                        for (var j = 0; j < numArray.length; j++) {
                            trueItems.push({
                                class: 'TextExp',
                                value: numArray[j]
                            })
                        }
                        pre = 'NumExp';
                        num = '';
                        i--;
                        continue;
                    }
                } else {
                    switch ($scope.items[i].class) {
                        //如果是变量
                        case 'VarExp':
                            switch (pre) {
                                case 'VarExp':
                                case 'NumExp':
                                    $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, '[' + $scope.data[$scope.items[i].value].name + ']', $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                                    return false;
                                default:
                                    trueItems.push($scope.items[i])//给pre加标识，trueItem加加值
                                    pre = 'VarExp';
                                    continue;
                            }
                            break;
                        case 'TextExp':      //如果是常量
                            if ('0123456789.'.indexOf($scope.items[i].value) >= 0) {
                                switch (pre) {
                                    case 'VarExp':
                                    case 'NumExp':
                                        $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, $scope.items[i].value, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                                        return false;
                                    default:
                                        num = $scope.items[i].value;
                                        continue;
                                }
                            } else {
                                if (trueItems.length == 0) {
                                    $scope.showErrorMessage(null, null, $scope.items[i].value, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                                    return false;
                                }
                                switch (pre) {
                                    case 'OpExp':
                                        $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, $scope.items[i].value, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                                        return false;
                                    default:
                                        trueItems.push($scope.items[i])
                                        pre = 'OpExp';         //如果是运算符
                                        continue;
                                }
                                pre = 'OpExp';
                                num = '';
                                trueItems.push($scope.items[i])
                            }
                    }
                }
            }
            if (num != '') {
                if (num.indexOf('.') == 0 || (num.indexOf('.') >= 0 && num.lastIndexOf('.') != num.indexOf('.'))) {
                    $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, num, null, null);
                    return false;
                }
                if (pre == 'NumExp' || pre == 'VarExp') return false;
                var tmp = parseFloat(num).toString().split('.');
                if (tmp[0].length > 8)
                {
                    $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, num, null, null);
                    return false;
                }
                else if (tmp.length >= 2 && tmp[1].length > 5) {
                    $scope.showErrorMessage(trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1] : null, num, null, null);
                    return false;
                }
                if (tmp.length == 1 && tmp[0] == '0' && trueItems[trueItems.length - 1].value == '÷') {
                    $scope.showErrorMessage(trueItems.length >= 3 ? trueItems[trueItems.length - 3] : null, trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems[trueItems.length - 1].value + '' + num, $scope.items.length > i + 1 ? $scope.items[i + 1] : null, $scope.items.length > i + 2 ? $scope.items[i + 2] : null);
                    return false;
                }
                var numArray = tmp[0].split('').concat(tmp.length >= 2 ? ['.'] : []).concat(tmp.length >= 2 ? tmp[1].split('') : []);
                for (var i = 0; i < numArray.length; i++) {
                    trueItems.push({
                        class: 'TextExp',
                        value: numArray[i]
                    })
                }
                pre = 'NumExp';
            }

            if (pre == 'OpExp') {
                $scope.showErrorMessage(trueItems.length >= 3 ? trueItems[trueItems.length - 3] : null, trueItems.length >= 2 ? trueItems[trueItems.length - 2] : null, trueItems.length >= 1 ? trueItems[trueItems.length - 1].value : '', null, null);
                return false;
            };
            $scope.items = trueItems;
            toastr.success('校验成功。');
            return true;
           
        }
    }])