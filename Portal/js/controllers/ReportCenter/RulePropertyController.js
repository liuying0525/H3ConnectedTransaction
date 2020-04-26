app.controller('RulePropertyController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http","$modal", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $modal, $state,  $stateParams, $filter, ControllerConfig) {

        $scope.Datas = [];
        $scope.Total = 0;
        $scope.TotalPage = 1;
        $scope.Page = 1;
        $scope.Min = true;
        $scope.Max = true;
        $scope.Size = 10;
        $scope.IsBase = false;
        $scope.checkAll = false;
        $scope.Province = [];
        $scope.City = [];
        $scope.SearchKey = {
            InternetType: '',
            ClassName: '',
        };

        $scope.$on('$viewContentLoaded', function (event) {

            $scope.init();
        });

        $scope.Rank = function (index) {
            var Objectid = "";
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.ScoringRank', { "Objectid": Objectid })
        }

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
                url: '/Portal/GetRuleProperty/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&InternetType=' + $scope.SearchKey.InternetType + '&ClassName=' + $scope.SearchKey.ClassName ,//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleProperty.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&InternetType=' + $scope.SearchKey.InternetType + '&ClassName=' + $scope.SearchKey.ClassName ,
                data: { page: page, size: $scope.Size }
            }).success(function (req) {

                $scope.Datas = req.Data;
                $scope.Total = req.Total;
                var totalPage = parseInt(parseInt(req.Total) / $scope.Size) + (parseInt(req.Total) % $scope.Size > 0 ? 1 : 0);
                $scope.TotalPage = totalPage <= 0 ? 1 : totalPage;
                $scope.SetPageButton();
                })
                .error(function (data, header, config, status) {// 19.7 
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
                else if (confirm('是否删除选中的' + deleteArray.length + '项产品？执行该操作后无法恢复，请谨慎处理。')) {
                } else {
                    return;
                }
            } else {
                if (confirm('是否删除办理店【' + $scope.Datas[index].InternetType + '】的规则性质？执行该操作后无法恢复，请谨慎处理。')) {
                    deleteArray.push({
                        ObjectID: $scope.Datas[index].ObjectID,

                    })
                } else {
                    return;
                }
            }

            $http({
                method: 'post',
                url: '/Portal/DelRuleProperty/Index',//wangxg 19.7
                //url: '/Portal/Ajax/DelRuleProperty.ashx',
                data: deleteArray
            }).success(function () {
                alert('删除成功。');
                $scope.search($scope.Page);
                })
                .error(function (data, header, config, status) {// 19.7 
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
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.RuleName', { "Objectid": Objectid })
        }

        $scope.btn_addAgents = function (data) {
            var modalInstance = $modal.open({
                templateUrl: 'EditAgency.html',    // 指向上面创建的视图
                controller: 'EditController',// 初始化模态范围
                size: "md",
                resolve: {
                    params: function () {
                        return {

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
        $scope.btn_updateAgents = function (index) {
            var Objectid = "";
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            
            $http({
                url: '/Portal/GetRuleProperty/Index?ObjectID=' + Objectid,//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleProperty.ashx?ObjectID=' + Objectid,
                data: {}
            })
                .success(function (result) {
                    var InternetType = "";
                    var ClassName = "";
                    var ObjectID = "";
                    if (result.Data.length > 0 && result.Data[0]["InternetType"] != undefined)
                        InternetType = result.Data[0]["InternetType"];

                    if (result.Data.length > 0 && result.Data[0]["ClassName"] != undefined)
                        ClassName = result.Data[0]["ClassName"];
                    if (result.Data.length > 0 && result.Data[0]["ObjectID"] != undefined)
                        ObjectID = result.Data[0]["ObjectID"];
                   
                    // 弹出模态框

                    var modalInstance = $modal.open({
                        templateUrl: 'EditAgency.html',    // 指向上面创建的视图
                        controller: 'EditController',// 初始化模态范围
                        size: "md",
                        resolve: {
                            params: function () {
                                return {
                                    InternetType: InternetType,
                                    ClassName: ClassName,
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
       
    }]);

app.controller("EditController", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params",
    function ($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params) {
        $scope.Datas = [];
        $scope.Datas0 = [];
        $scope.getLanguage = function () {
          
        }

        $scope.InternetType = "";
        $scope.ClassName = "";
        $scope.ObjectID = "";

        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        //控件初始化参数
      
        $scope.init = function () {
            if (params.ObjectID != "") {
                //state = 1;
                $scope.ObjectID = params.ObjectID;
                //$scope.Code.Editable = false;
                $scope.InternetType = params.InternetType;
                //$scope.Age = params.Age;
                //$scope.StartTime = params.fTime;
                $scope.ClassName = params.ClassName;
                //$scope.WasAgentOptions.V = params.teacheID;
            }

            //编辑初始化
            //$http({
            //    url: '/Portal/GetRuleProperty/Index?method=GetPerproty',//wangxg 19.7
            //    //url: '/Portal/Ajax/GetRuleProperty.ashx?method=GetPerproty',
            //    data: {}
            //}).success(function (result) {
            //    $scope.Datas = result.property;
            //    $scope.Datas0 = result.type;
            //    })
        }
        $scope.init();

        $scope.ok = function () {
            if ($scope.InternetType == "" || $scope.InternetType == null || $scope.InternetType == undefined) {
                alert("请填写内网/外网");
                return;
            }
            if ($scope.ClassName == "" || $scope.ClassName == null || $scope.ClassName == undefined) {
                alert("请填写性质");
                return;
            }
            $http({
                url: '/Portal/GetRuleProperty/Index?method=AddRuleProperty',//wangxg 19.7
                //url: '/Portal/Ajax/GetRuleProperty.ashx?method=AddRuleProperty',
                params: {
                    InternetType: $scope.InternetType,
                    ClassName: $scope.ClassName,
                    ObjectID: $scope.ObjectID,
                }
            })
                .success(function (result, header, config, status) {
                    if (result==1) {
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