app.controller('AddProductsController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {

    $scope.ProductName = '';
    $scope.Dealer = $scope.user.Appellation;
    $scope.Datas = [];

    $scope.init = function () {
        $scope.ProductName = $stateParams.ProductName;
        $http({
            method: 'post',
            url: '/Portal/GetProducts/Index?Dealer=' + $scope.user.Appellation,//wangxg 19.7
            //url: '/Portal/GetProducts.ashx?Dealer=' + $scope.user.Appellation,
            data: {},
            sync: true
        }).success(function (req) {
            $scope.Datas = req.Data;

        }).error(function (data, header, config, status) {// 19.7 
            showAgErr(data, header);
        }).then(function () {
            if ($stateParams.ProductName != '' && $stateParams.ProductName != undefined && $stateParams.ProductName != null) {
                var interval = setInterval(function () {
                    var anchors = $('[anchor]');
                    if (anchors != undefined && anchors.length > 0) {
                        for (var i = 0; i < anchors.length; i++) {
                            if ($(anchors[i]).attr('anchor').indexOf($stateParams.ProductName) >= 0) {
                                $('[anchorDiv]').parent().parent().parent().parent().animate({ scrollTop: $(anchors[i]).offset().top - 50 }, 500)
                                break;
                            }
                        }
                        clearInterval(interval);
                    }
                }, 200)
            }
        });

        
    }

    $scope.search = function () {
        var anchors = $('[anchor]');
        if (anchors != undefined && anchors.length > 0) {
            for (var i = 0; i < anchors.length; i++) {
                if ($(anchors[i]).attr('anchor').indexOf($scope.ProductName) >= 0) {
                    $('[anchorDiv]').parent().parent().parent().parent().animate({ scrollTop: $(anchors[i]).offset().top - 50 }, 500)
                    break;
                }
            }
        }
    }

    $scope.cancel = function () {
        $rootScope.back();
    }

    $scope.save = function () {
        for (var i = 0; i < $scope.Datas.length; i++) {
            if ($scope.Datas[i].ProductAlias == null || $scope.Datas[i].ProductAlias == undefined || $scope.Datas[i].ProductAlias.length <= 0) {
                hasEmptyAlias = true;
                break;
            }
        }

        if (!hasEmptyAlias || confirm('存在未设置【产品别名】的产品，点击确定后将设置【产品名称】为【产品别名】，是否确定提交？')) {
            for (var i = 0; i < $scope.Datas.length; i++) {
                if ($scope.Datas[i].ProductAlias == null || $scope.Datas[i].ProductAlias == undefined || $scope.Datas[i].ProductAlias.length <= 0) {
                    $scope.Datas[i].ProductAlias = $scope.Datas[i].ProductName
                }
            }

            var products = [];
            var hasEmptyAlias = false;
            for (var i = 0; i < $scope.Datas.length; i++) {
                //if ($scope.Datas[i].Description != '' && $scope.Datas[i].Description.length < 200) {
                //    alert('产品【' + $scope.Datas[i].ProductName + '】的描述未达到要求长度。')
                //    return;
                //}
                products.push({
                    ProductId: $scope.Datas[i].ProductId,
                    Name: $scope.Datas[i].ProductName,
                    Alias: $scope.Datas[i].ProductAlias,
                    Description: $scope.Datas[i].Description
                })
            }

            var data = {
                Dealer: $scope.Dealer,
                Products: products
            }
            $http({
                method: 'post',
                //url: '/Portal/AddProducts.ashx',
                url: '/Portal/AddProducts/Index',// wangxg 19.7
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

    $scope.init();
}]);