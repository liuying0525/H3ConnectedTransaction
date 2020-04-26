app.controller('ProductManageController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
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
    $scope.SearchKey = {
        ProductName: '',
        ProductAlias: '',
        Description: '',
        State: '',
        Dealer: ''
    };

    $scope.$on('$viewContentLoaded', function (event) {
        var isBase = false;

        for (var i = 0; i < $scope.user.Code.length; i++) {
            if(!('0123456789'.indexOf($scope.user.Code.substr(i,1)) >= 0)) {
                isBase = true;
                break;
            }
        }

        $scope.IsBase = isBase;
        if (!$scope.IsBase) {
            $scope.SearchKey.Dealer = $scope.user.Appellation;
        }
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
            url: '/Portal/GetProducts/Index?IsBase=' + ($scope.IsBase ? "true" : "") + '&Page=' + (page - 1) + '&Size=' + $scope.Size + '&ProductName=' + $scope.SearchKey.ProductName + '&ProductAlias=' + $scope.SearchKey.ProductAlias + '&Description=' + $scope.SearchKey.Description + '&State=' + $scope.SearchKey.State + '&Dealer=' + $scope.SearchKey.Dealer,//wangxg 19.7
            //url: '/Portal/GetProducts.ashx?IsBase=' + ($scope.IsBase ? "true" : "") + '&Page=' + (page - 1) + '&Size=' + $scope.Size + '&ProductName=' + $scope.SearchKey.ProductName + '&ProductAlias=' + $scope.SearchKey.ProductAlias + '&Description=' + $scope.SearchKey.Description + '&State=' + $scope.SearchKey.State + '&Dealer=' + $scope.SearchKey.Dealer,
            data: { page: page, size: $scope.Size }
        }).success(function (req) {
            console.log(req);
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
            for(var i = 0; i < $scope.Datas.length; i++){
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
                        ProductID: $scope.Datas[i].ProductId,
                        Dealer: $scope.Datas[i].Dealer
                    })
                }
            }
            if (deleteArray.length <= 0) {
                alert('您未选择任何产品。');
                return;
            }
            else if (confirm('是否删除选中的' + deleteArray.length + '项产品？执行该操作后无法恢复，请谨慎处理。')) {
            } else {
                return;
            }
        } else {
            if (confirm('是否删除经销商【' + $scope.Datas[index].Dealer + '】的【' + $scope.Datas[index].ProductName + '】产品？执行该操作后无法恢复，请谨慎处理。')) {
                deleteArray.push({
                    ProductID: $scope.Datas[index].ProductId,
                    Dealer: $scope.Datas[index].Dealer
                })
            } else {
                return;
            }
        }

        $http({
            method: 'post',
            //url: '/Portal/DelProducts.ashx',
            url: '/Portal/DelProducts/Index',//wangxg 19.7
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
        if (index >= 0)
            $state.go('app.AddProducts', { ProductName: $scope.Datas[index].ProductName })
        else
            $state.go('app.AddProducts')
    }
}]);