app.controller('BatchExpressController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {
        //$scope.data = [];
        //$scope.title = "";
        //$scope.dealerName = "";
        //$scope.Pid = $stateParams.Objectid;
        //$scope.DealerId = $stateParams.DealerId;
        $scope.data = {
            ExpressNumber: ""
        };
        $scope.init = function () {
            $http({
                method: "Post",
                //url: "/Portal/Ajax/BatchExpressNumber.ashx?Action=Load&search="// + $scope.search
                url: "/Portal/BatchExpressNumber/Index?Action=Load&search="// + $scope.search // 19.6.28 wangxg
            }).success(function (ret) {
                //alert(JSON.stringify(ret))
                //$scope.data = ret;
                $("#mortgageList").html(ret);
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        };
        $scope.init();
        $scope.save = function () {
            if ($scope.data.ExpressNumber == "" || $scope.data.ExpressNumber == null || $scope.data.ExpressNumber == undefined) {
                alert("请填写快递单号");
                return;
            }
            var mortgageid = angular.element($("[data-mortgageid]")).attr("data-mortgageid");
            if (mortgageid == "" || mortgageid == null || mortgageid == undefined) {
                alert("请选择抵押流程");
                return;
            }
            $http({
                method: 'post',
                //url: '/Portal/Ajax/BatchExpressNumber.ashx?Action=Save&MortgageId=' + mortgageid + '&ExpressNumber=' + $scope.data.ExpressNumber
                url: '/Portal/BatchExpressNumber/Index?Action=Save&MortgageId=' + mortgageid + '&ExpressNumber=' + $scope.data.ExpressNumber // 19.6.28 wangxg
            }).success(function (req) {
                if (req != "") {
                    alert(req);
                    $rootScope.clear();
                }
            })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        };
        $scope.clear = function () {
            $scope.data = {
                ExpressNumber: ""
            };
        };
        $scope.title = "批量输入快递单号";
    }]);