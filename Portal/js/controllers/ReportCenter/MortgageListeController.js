app.controller('MortgageListController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
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
            ApplyNo: '',
            CustomerName: '',
            CarType: '',
            Saletor: '',
            FkState: '',
            DyState: '',
            GdTime: '',
            StartTime: '',
            EndTime: '',
        };

        
        $scope.$on('$viewContentLoaded', function (event) {
            $scope.init();
        });

        $scope.init = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
        }

        $scope.onclick = function () {
            alert($scope.SearchKey.StartTime);
        }

        $scope.clickSearch = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
        }

        $scope.search = function (page) {
            console.log($scope.SearchKey.StartTime);
            $http({
                method: 'post',
                url: '/Portal/MortgageList/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&ApplyNo=' + $scope.SearchKey.ApplyNo + '&CustomerName=' + $scope.SearchKey.CustomerName + '&CarType=' + $scope.SearchKey.CarType + '&Saletor=' + $scope.SearchKey.Saletor + '&FkState=' + $scope.SearchKey.FkState + '&DyState=' + $scope.SearchKey.DyState + '&GdTime=' + $scope.SearchKey.GdTime + '&StartTime=' + $scope.SearchKey.StartTime + '&EndTime=' + $scope.SearchKey.EndTime,//wangxg 19.7
                //url: '/Portal/Ajax/MortgageList.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&ApplyNo=' + $scope.SearchKey.ApplyNo + '&CustomerName=' + $scope.SearchKey.CustomerName + '&CarType=' + $scope.SearchKey.CarType + '&Saletor=' + $scope.SearchKey.Saletor + '&FkState=' + $scope.SearchKey.FkState + '&DyState=' + $scope.SearchKey.DyState + '&GdTime=' + $scope.SearchKey.GdTime + '&StartTime=' + $scope.SearchKey.StartTime + '&EndTime=' + $scope.SearchKey.EndTime,
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
            //$("#select_id option[value='未抵押']").prop("selected", true);
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
            debugger;
            var ApplyNo = "";
            var i = $(':radio[name="choseState"]:checked').val();

            ApplyNo = $scope.Datas[i].ApplyNo;
            if (ApplyNo == "") {
                alert('您未选择任何申请号。');
                return;
            }
            else if (confirm('是否发起选中的' + ApplyNo + '号？')) {
            } else {
                return;
            }

            $http({
                method: 'post',
                url: '/Portal/StartWorkFlow/Index',//wangxg 19.7
                //url: '/Portal/Ajax/StartWorkFlow.ashx',
                data: ApplyNo
            }).success(function (req) {
                if (req == 0)
                    alert('已发起抵押流程，不能重复发起。');
                else
                    window.open(req);
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


        $scope.rowClick = function ($index) {
            debugger;
            if ($index.checked == undefined) {
                var num = Number($index + 1);
                //$("#mortgagelist tr:eq(" + num + ")").css("background-color", "gray").siblings().RemoveClass("background-color");
                $($("#mortgagelist tr:eq(" + num + ")")[0]).find("[type=radio]").click();
            } else {
                //$("#mortgagelist tr:eq(" + num + ")").removeClass("background-color");
                $index.checked = !($index.checked);
            }
        };

    }]);