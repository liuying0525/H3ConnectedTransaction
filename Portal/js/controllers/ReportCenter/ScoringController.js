app.controller('ScoringController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
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
            JXSMC: '',
            NWWW: '',
            KIND: '',
            JXS_LEVEL:''
        };

        $scope.$on('$viewContentLoaded', function (event) {
            $scope.init();
        });
        $scope.init = function() {
            $scope.Page = 1;
            $scope.search($scope.Page);
        };

        $scope.clickSearch = function() {
            $scope.Page = 1;
            $scope.search($scope.Page);
        };

        $scope.search = function (page) {
            $http({
                method: 'post',
                url: '/Portal/GetScoring/Index?Page=' + (page - 1) + '&Size=' + $scope.Size + '&JXSMC=' + $scope.SearchKey.JXSMC + '&NWWW=' + $scope.SearchKey.NWWW + '&KIND=' + $scope.SearchKey.KIND + '&JXS_LEVEL=' + $scope.SearchKey.JXS_LEVEL,//wangxg 19.7
                //url: '/Portal/Ajax/GetScoring.ashx?Page=' + (page - 1) + '&Size=' + $scope.Size + '&JXSMC=' + $scope.SearchKey.JXSMC + '&NWWW=' + $scope.SearchKey.NWWW + '&KIND=' + $scope.SearchKey.KIND + '&JXS_LEVEL=' + $scope.SearchKey.JXS_LEVEL,
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

        $scope.DumpTo = function (link) {
            eval(link);
        }

        $scope.clickScore = function() {
            $.post("/Portal/ajax/TestScore.ashx", "", function(msg) { alert(msg) });
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
        };

        $scope.Delete = function (index) {
            var ApplyNo = "";
            var i = $(':radio[name="choseState"]:checked').val();

            ApplyNo = $scope.Datas[i].ApplyNo;
            if (ApplyNo == "") {
                alert('您未选择任何申请号。');
                return;
            } else if (confirm('是否发起选中的' + ApplyNo + '号？')) {

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
                    alert('删除成功。');
                else
                    window.location.href = req;
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        };

        $scope.clear = function () {
            $scope.SearchKey = {
                JXSMC: '',
                NWWW: '',
                KIND: '',
                JXS_LEVEL:''
            };
            $scope.clickSearch();
        };
    }]);