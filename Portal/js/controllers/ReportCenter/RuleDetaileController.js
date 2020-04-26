app.controller('RuleDetaileController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {

        $scope.Datas = [];
        $scope.Total = 0;
        $scope.TotalPage = 1;
        $scope.Page = 1;
        $scope.Min = true;
        $scope.Max = true;
        $scope.Pid = $stateParams.Objectid;
        $scope.Parentid = $stateParams.Pid;
        $scope.Size = 10;
        $scope.IsBase = false;
        $scope.checkAll = false;
        $scope.Province = [];
        $scope.City = [];
        $scope.SearchKey = {
            DetaileName: '',
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

            $state.go('app.RuleName', { "Objectid": $scope.Parentid})
        }
        $scope.search = function (page) {
            $http({
                method: 'post',
                url: '/Portal/RuleDetaile/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&DetaileName=' + $scope.SearchKey.DetaileName + '&pid=' + $scope.Pid,//wangxg 19.7
                //url: '/Portal/Ajax/RuleDetaile.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&DetaileName=' + $scope.SearchKey.DetaileName + '&pid=' + $scope.Pid,
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
                    alert('您未选择任何规则明细。');
                    return;
                }
                else if (confirm('是否删除选中的' + deleteArray.length + '项产品？执行该操作后无法恢复，请谨慎处理。')) {
                } else {
                    return;
                }
            } else {
                if (confirm('是否删除【' + $scope.Datas[index].RuleName + '】的规则明细？执行该操作后无法恢复，请谨慎处理。')) {
                    deleteArray.push({
                        ObjectID: $scope.Datas[index].ObjectID,

                    })
                } else {
                    return;
                }
            }

            $http({
                method: 'post',
                //url: '/Portal/Ajax/DelRuleDetaile.ashx',
                url: '/Portal/DelRuleDetaile/Index',//wangxg 19.7
                data: deleteArray
            }).success(function () {
                alert('删除成功。');
                $scope.search($scope.Page);
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }
        
        $scope.New = function (index) {
            var Pid = $stateParams.Objectid;
            var cid = "";
            if (index != null && index !== "") {
                cid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.AddRuleDetail', { "Objectid": Pid, "Cid": cid })
        }
        $scope.check = function (index) {
            var Pid = "check";
            var cid = "";
            if (index != null && index !== "") {
                cid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.AddRuleDetail', { "Objectid": Pid, "Cid": cid })
        }
        

    }]);