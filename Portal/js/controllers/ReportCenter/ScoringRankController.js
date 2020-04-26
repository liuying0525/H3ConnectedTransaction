app.controller('ScoringRankController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
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
            PropertyName: '',
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
        $scope.Back = function () {

            $state.go('app.RuleProperty')
        }
        $scope.search = function (page) {
            $http({
                method: 'post',
                url: '/Portal/ScoringRank/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&PropertyName=' + $scope.SearchKey.PropertyName + '&Pid=' + $scope.Pid,//wangxg 19.7
                //url: '/Portal/Ajax/ScoringRank.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&PropertyName=' + $scope.SearchKey.PropertyName + '&Pid=' + $scope.Pid,
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
                    alert('您未选择任何评分等级。');
                    return;
                }
                else if (confirm('是否删除选中的' + deleteArray.length + '项评分？执行该操作后无法恢复，请谨慎处理。')) {
                } else {
                    return;
                }
            } else {
                if (confirm('是否删除【' + $scope.Datas[index].PropertyName + '】的评分？执行该操作后无法恢复，请谨慎处理。')) {
                    deleteArray.push({
                        ObjectID: $scope.Datas[index].ObjectID,

                    })
                } else {
                    return;
                }
            }

            $http({
                method: 'post',
                //url: '/Portal/Ajax/DelScoringRank.ashx',
                url: '/Portal/DelScoringRank/Index',//wangxg 19.7
                data: deleteArray
            }).success(function () {
                alert('删除成功。');
                $scope.search($scope.Page);
                }).error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }
        
        $scope.New = function (index) {
            var Objectid = "";
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.RuleName', { "Objectid": Objectid })
        }

        $scope.btn_addAgents = function (index) {
            var Pid = $stateParams.Objectid;
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }

            $http({
                url: '/Portal/ScoringRank/Index?Pid=' + Pid + '&GetState=1',//wangxg 19.7
                //url: '/Portal/Ajax/ScoringRank.ashx?Pid=' + Pid + '&GetState=1',
                data: {}
            }).success(function (result) {

                var BClassName = result;
                var Grade = "";
                var Pid = $scope.Pid;
                var RangeFrom = "";
                var RangeTo = "";
                var ObjectID = "";
                // 弹出模态框

                var modalInstance = $modal.open({
                    templateUrl: 'EditScoringRank.html',    // 指向上面创建的视图
                    controller: 'EditScoringRankController',// 初始化模态范围
                    size: "md",
                    resolve: {
                        params: function () {
                            return {
                                PropertyName: BClassName,
                                RangeFrom: RangeFrom,
                                RangeTo: RangeTo,
                                Pid: Pid,
                                Grade: Grade,
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

            $http({
                url: '/Portal/ScoringRank/Index?ObjectID=' + Objectid,//wangxg 19.7
                //url: '/Portal/Ajax/ScoringRank.ashx?ObjectID=' + Objectid,
                data: {}
            })
                .success(function (result) {
                    var PropertyName = "";
                    var Grade = "";
                    var ObjectID = "";
                    if (result.Data.length > 0 && result.Data[0]["PropertyName"] != undefined)
                        PropertyName = result.Data[0]["PropertyName"];

                    if (result.Data.length > 0 && result.Data[0]["Grade"] != undefined)
                        Grade = result.Data[0]["Grade"];
                    if (result.Data.length > 0 && result.Data[0]["RangeFrom"] != undefined)
                        RangeFrom = result.Data[0]["RangeFrom"];
                    if (result.Data.length > 0 && result.Data[0]["RangeTo"] != undefined)
                        RangeTo = result.Data[0]["RangeTo"];
                    if (result.Data.length > 0 && result.Data[0]["ObjectID"] != undefined)
                        ObjectID = result.Data[0]["ObjectID"];

                    // 弹出模态框

                    var modalInstance = $modal.open({
                        templateUrl: 'EditScoringRank.html',    // 指向上面创建的视图
                        controller: 'EditScoringRankController',// 初始化模态范围
                        size: "md",
                        resolve: {
                            params: function () {
                                return {
                                    PropertyName: PropertyName,
                                    RangeFrom: RangeFrom,
                                    RangeTo: RangeTo,
                                    Pid:"",
                                    Grade: Grade,
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

app.controller("EditScoringRankController", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params",
    function ($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params) {
        $scope.Datas = [];
        $scope.Datas0 = [];
        $scope.getLanguage = function () {

        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        //控件初始化参数

        $scope.init = function () {

            //编辑初始化
           
                $scope.ObjectID = params.ObjectID;
                $scope.PropertyName = params.PropertyName;
                //$scope.Code.Editable = false;
                $scope.Grade = params.Grade;
                $scope.RangeFrom = params.RangeFrom;
                $scope.RangeTo = params.RangeTo;
                $scope.Pid = params.Pid;
                $scope.ObjectID = params.ObjectID;

        }
        $scope.init();

        $scope.ok = function () {
            if ($scope.Grade == "" || $scope.Grade == null || $scope.Grade == undefined) {
                alert("请填写等级");
                return;
            }
            if ($scope.RangeTo == "" || $scope.RangeTo == null || $scope.RangeTo == undefined) {
                alert("请填写最大分值");
                return;
            }
            if (isNaN($scope.RangeTo) && $scope.RangeTo!='?') {
                alert("最大分值只能为数字");
                return;
            }
            if (isNaN($scope.RangeFrom) && $scope.RangeFrom != '?')  {
                alert("最小分值只能为数字");
                return;
            }
            if (parseInt($scope.RangeTo) < parseInt($scope.RangeFrom)) {
                alert("最大分值必须大于最小分值");
                return;
            }
            if ($scope.RangeFrom == "" || $scope.RangeFrom == null || $scope.RangeFrom == undefined) {
                alert("请填写最小分值");
                return;
            }
            $http({
                url: '/Portal/ScoringRank/Index?method=AddScoringRank',//wangxg 19.7
                //url: '/Portal/Ajax/ScoringRank.ashx?method=AddScoringRank',
                params: {
                    Grade: $scope.Grade,
                    RangeTo: $scope.RangeTo,
                    RangeFrom: $scope.RangeFrom,
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