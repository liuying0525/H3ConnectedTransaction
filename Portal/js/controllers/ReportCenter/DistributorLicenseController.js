app.controller('DistributorLicenseController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {
        //$scope.data = [];
        //$scope.title = "";
        //$scope.dealerName = "";
        //$scope.Pid = $stateParams.Objectid;
        //$scope.DealerId = $stateParams.DealerId;
        $scope.data = {
            Distributor: "",
            License: "",
            Objectid:""
        };
        $scope.init = function () {
            $http({
                method: "Post",
                //url: "/Portal/Ajax/DistributorLicense.ashx?Action=Load&search=" + $scope.data.Distributor
                url: "/Portal/DistributorLicense/Index?Action=Load&search=" + $scope.data.Distributor //wangxg 19.7
            }).success(function (ret) {
                //alert(JSON.stringify(ret))
                //$scope.data = ret;
                $("#divSense>ul").html(ret);
                })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        };
        $scope.init();
        $scope.save = function () {
            if ($scope.data.License == "" || $scope.data.License == null || $scope.data.License == undefined) {
                alert("请输入营业执照名称");
                return;
            }
            if ($scope.data.Distributor == "" || $scope.data.Distributor == null || $scope.data.Distributor == undefined) {
                alert("请选经销商");
                return;
            }
            debugger
            $http({
                method: 'Post',
                //url: '/Portal/Ajax/DistributorLicense.ashx?Action=Save&License=' + $scope.data.License + '&Distributor=' + angular.element($("#divSense").prev()).val() + '&Objectid=' + angular.element($("[data-yyzz]")).attr("data-objectid") + '&crmDealerID=' + angular.element($("[data-yyzz]")).attr("data-crmdealerid")
                url: '/Portal/DistributorLicense/Index?Action=Save&License=' + $scope.data.License + '&Distributor=' + angular.element($("#divSense").prev()).val() + '&Objectid=' + angular.element($("[data-yyzz]")).attr("data-objectid") + '&crmDealerID=' + angular.element($("[data-yyzz]")).attr("data-crmdealerid") // wangxg 19.7
            }).success(function (req) {
                if (req != "") {
                    alert(req);
                    $scope.clear();
                }
            })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        };
        $scope.search = function () {
            if ($scope.data.Distributor == "" || $scope.data.Distributor == null || $scope.data.Distributor == undefined) $("#divSense").hide();
            else {
                $("#divSense").show();
                $scope.init();
            }
        };
        $scope.clear = function () {
            $scope.data = {
                Distributor: "",
                License: "",
                Objectid: ""
            };
        };
        $scope.title = "解压打印经销商营业执照关联";
    }]);