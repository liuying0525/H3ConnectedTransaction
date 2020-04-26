app.controller('AddYWDataController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {
        $scope.data = [];
        $scope.title = "";
        $scope.dealerName = "";
        $scope.Pid = $stateParams.Objectid;
        $scope.DealerId = $stateParams.DealerId;
        $scope.CrmId = $stateParams.CrmId;
        $scope.init = function () {
            $http({
                method: 'post',
                //url: '/Portal/Ajax/AddYWData.ashx?method=GetDealer&crmId=' + $scope.CrmId + "&dealerId=" + $scope.DealerId
                url: '/Portal/AddYWData/Index?method=GetDealer&crmId=' + $scope.CrmId + "&dealerId=" + $scope.DealerId //wangxg 19.7
            }).success(function (ret) {
                //alert(JSON.stringify(ret))
                $scope.data = ret;
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        };
        $scope.init();
        $scope.cancel = function () {
            $rootScope.back();
        };
        $scope.save = function () {
            //if ($scope.data.RISK == "" || $scope.data.RISK == null || $scope.data.RISK == undefined) {
            //    alert("请选择经销商");
            //    return;
            //}
            if ($scope.data.RISK == "" || $scope.data.RISK == null || $scope.data.RISK == undefined) {
                alert("请填写欺诈风险");
                return;
            }
            if ($scope.data.TOW == "" || $scope.data.TOW == null || $scope.data.TOW == undefined) {
                alert("请填写拖车能力");
                return;
            }
            if ($scope.data.PUNISH == "" || $scope.data.PUNISH == null || $scope.data.PUNISH == undefined) {
                alert("请填写处置能力");
                return;
            }
            if ($scope.data.ARCHIVING == "" || $scope.data.ARCHIVING == null || $scope.data.ARCHIVING == undefined) {
                alert("请填写归档率");
                return;
            }
            if ($scope.data.COMPLAIN == "" || $scope.data.COMPLAIN == null || $scope.data.COMPLAIN == undefined) {
                alert("请填写投诉数");
                return;
            }
            if (isNaN($scope.data.COMPLAIN)) {
                alert("投诉数只能为数字");
                return;
            }
            var data = {
                pId: $scope.Pid,
                DealerId: $scope.DealerId,
                CrmId: $scope.CrmId,
                DealerName: $scope.data.DEALERNAME,
                Risk: $scope.data.RISK,
                Tow: $scope.data.TOW,
                Punish: $scope.data.PUNISH,
                Archiving: $scope.data.ARCHIVING,
                Complain: $scope.data.COMPLAIN
            };
            $http({
                method: 'post',
                //url: '/Portal/Ajax/AddYWData.ashx',
                url: '/Portal/AddYWData/Index',// wangxg 19.7
                data: data,
            }).success(function (req) {
                if (req == "Success") {
                    alert("保存成功");
                    $rootScope.back();
                }
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        };

        //$scope.getText = function () {
        //    var Name = $("#DelalerName option:selected").html();
        //    var Id = $scope.Shop;
        //    $http({
        //        method: 'post',
        //        //url: '/Portal/Ajax/AddYWData.ashx?method=GetGrade' + '&ID=' + Id + '&Name=' + Name
        //        url: '/Portal/AddYWData/Index?method=GetGrade' + '&ID=' + Id + '&Name=' + Name// wangxg 19.7
        //    }).success(function (ret) {
        //        console.log(ret);
        //        $scope.Grade = ret;
        //    });
        //};
        $scope.title = "业务数据修改";
    }]);