app.controller('GuaranteeAmountController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {

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
            DISTRIBUTOR: ''
        };

        $scope.$on('$viewContentLoaded', function (event) {
            $scope.init();
        });
        $scope.init = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
        };

        $scope.clickSearch = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
        };

        $scope.search = function (page) {
            $http({
                method: 'post',
                url: '/Portal/GuaranteeAmount/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&DISTRIBUTOR=' + $scope.SearchKey.DISTRIBUTOR,//wangxg 19.7
                //url: '/Portal/Ajax/GuaranteeAmount.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&DISTRIBUTOR=' + $scope.SearchKey.DISTRIBUTOR,
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
        };

        $scope.SetPageButton = function () {
            $scope.Min = $scope.Page == 1;
            $scope.Max = $scope.Page == $scope.TotalPage;
        };

        $scope.CheckMax = function () {
            if ((parseInt($scope.Page) > 0) == false) $scope.Page = 1;
            if (parseInt($scope.Page) > $scope.TotalPage)
                $scope.Page = $scope.TotalPage;
            $scope.search($scope.Page);
        };

        $scope.ModPage = function (Value) {
            $scope.Page = parseInt($scope.Page) + parseInt(Value);
            $scope.search($scope.Page);
        };

        $scope.clear = function () {
            $scope.SearchKey = {
                DISTRIBUTOR: ''
            };
            $scope.clickSearch();
        };
    }]);