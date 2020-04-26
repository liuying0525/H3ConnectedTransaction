app.controller('AddMortgageRulesController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {
        $scope.Province = [];
        $scope.City = [];
        $scope.SpName = [];
        $scope.DyName = [];
        $scope.ObjectID = $stateParams.Objectid;
        $scope.Datas = [];
        $scope.title = "";
        $scope.init = function () {
            $scope.ObjectID = $stateParams.Objectid;

            $http({
                method: 'post',
                url: '/Portal/GetMortageRules/Index?method=Data&ObjectID=' + $scope.ObjectID,//wangxg 19.7
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=Data&ObjectID=' + $scope.ObjectID,
                data: {},
                sync: false
            }).success(function (req) {

                $http({
                    method: 'post',
                    url: '/Portal/GetMortageRules/Index?method=GetProvince' + '&id=' + req.Data[0].Province,//wangxg 19.7
                    //url: '/Portal/Ajax/GetMortageRules.ashx?method=GetProvince' + '&id=' + req.Data[0].Province,
                    sync: false
                }).success(function (req2) {
                    $scope.City = req2;
                    $scope.Datas = req.Data;
                    var interval = setInterval(function () {
                        if ($('[ng-model="Datas[0].City"]').find('option').not('[value=""]').length > 0) {
                            $('[ng-model="Datas[0].City"]').val(req.Data[0].City);
                        }
                        if ($('[ng-model="Datas[0].SpName"]').find('option').not('[value=""]').length > 0) {
                            $('[ng-model="Datas[0].SpName"]').val(req.Data[0].SpName);
                        }
                        if ($('[ng-model="Datas[0].DyName"]').find('option').not('[value=""]').length > 0) {
                            $('[ng-model="Datas[0].DyName"]').val(req.Data[0].DyName);
                        }
                        clearInterval(interval);

                    }, 50)
                })
                    .error(function (data, header, config, status) {// 19.7 
                        showAgErr(data, header);
                    });


            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });

        }

        $scope.cancel = function () {
            $rootScope.back();
        }

        $scope.save = function () {

            {
                if ($scope.Datas[0].Province == "" || $scope.Datas[0].Province == null || $scope.Datas[0].Province == undefined) {
                    alert("请填写省份");
                    return;
                }
                if ($scope.Datas[0].City == "" || $scope.Datas[0].City == null || $scope.Datas[0].City == undefined) {
                    alert("请填写城市");
                    return;
                }
                if ($scope.Datas[0].SpName == "" || $scope.Datas[0].SpName == null || $scope.Datas[0].SpName == undefined) {
                    alert("请填写上牌员");
                    return;
                }
                if ($scope.Datas[0].DyName == "" || $scope.Datas[0].DyName == null || $scope.Datas[0].DyName == undefined) {
                    alert("请填写抵押员");
                    return;
                }
                if ($scope.Datas[0].Shop == "" || $scope.Datas[0].Shop == null || $scope.Datas[0].Shop == undefined) {
                    alert("请填写经销商名称");
                    return;
                }
                var products = [];

                var ObjectID = $scope.Datas[0].ObjectID
                products.push({
                    Shop: $scope.Datas[0].Shop,
                    Province: $scope.Datas[0].Province,
                    City: $scope.Datas[0].City,
                    SpName: $scope.Datas[0].SpName,
                    DyName: $scope.Datas[0].DyName

                })
                var data = {
                    ObjectID: ObjectID,
                    Products: products
                }
                $http({
                    method: 'post',
                    //url: '/Portal/Ajax/AddMortageRules.ashx',
                    url: '/Portal/AddMortageRules/Index',// wangxg 19.7
                    data: data,
                }).success(function (req) {
                    if (req == "Success") {
                        alert("保存成功");
                        $rootScope.back();
                    }
                }).error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
            }
        }

        $scope.getProvince = function () {
            $http({
                method: 'post',
                url: '/Portal/GetMortageRules/Index?method=GetProvince',//wangxg 19.7
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=GetProvince',
            }).success(function (req) {
                $scope.Province = req;
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }
        $scope.GetPeople = function () {
            $http({
                method: 'post',
                url: '/Portal/GetMortageRules/Index?method=GetPeople',//wangxg 19.7
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=GetPeople',
            }).success(function (req) {
                $scope.SpName = req[0];
                $scope.DyName = req[1];
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }
        $scope.getCity = function () {
            var id = $scope.Datas[0].Province;
            $http({
                method: 'post',
                url: '/Portal/GetMortageRules/Index?method=GetCity' + '&id=' + id,//wangxg 19.7
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=GetCity' + '&id=' + id,
            }).success(function (req) {
                $scope.City = req;

            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }
        $scope.getProvince();
        $scope.GetPeople();
        $scope.title = "抵押规则新增";
        if ($scope.ObjectID != "") {
            $scope.init();

            $scope.title = "抵押规则修改";
        }
    }]);