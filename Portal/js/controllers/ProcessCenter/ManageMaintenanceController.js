app.controller('ManageMaintenanceController', ['$scope', "$rootScope", "$translate", "$compile", "$http", "$timeout", "$state", "$interval", "$filter", "ControllerConfig", "datecalculation", "jq.datables", "toastr",
    function ($scope, $rootScope, $translate, $compile, $http, $timeout, $state, $interval, $filter, ControllerConfig, datecalculation, jqdatables, toastr) {
        $scope.$on('$viewContentLoaded', function (event) {
            $scope.init();
        });

        $scope.init = function () {
            $scope.StartTime = datecalculation.redDays(new Date(), 30);
            $scope.EndTime = datecalculation.addDays(new Date(), 30);
            $scope.maintenance = initMain();
                   $scope.getIndex();
        }

        $scope.getIndex = function () {
            var dataUrl = "/portal/RelatedTrans/GetIndex";
            $http({
                method: "get",
                url: dataUrl,
                transformRequest: angular.identity
            }).success(function (result, header, config, status) {
                console.log(result);
                if (result) {
                    
                    for(var i in result){
                    	if(result[i]==null){
                    		result[i]=""
                    	}
                    }
                    //					toastr.error(result.errors);
                    $scope.maintenance = result;
                } else {

                }
            })
        }

        //表单保存提交
        $scope.submitted = false;

        function initMain() {
            var omodel = {};
            omodel.asset = "";
            omodel.incomePrevYear = "";
            omodel.ratePrevYear = "";
            omodel.assetPrevReport = "";
            omodel.sharePriceAvg = "";
            omodel.numberOfShares = "";
            createTime="";
            modifyTime="";
            return JSON.parse(JSON.stringify(omodel));
        }

        $scope.OnSubmitForm = function (event) {

            event.preventDefault();
            if ($scope.manageform.$valid) {
                debugger
                if ($scope.maintenance.hasOwnProperty("id")) {
                    var dataUrl = "/portal/RelatedTrans/ModifyIndex";
                    $scope.tips="修改成功"
                } else {
                    var dataUrl = "/portal/RelatedTrans/AddIndex";
                    $scope.tips="保存成功"
                }
            } else {
                $('input.ng-invalid').css("borderColor", "red");
                $scope.manageform.submitted = true;
                return
            }

            var formmain = new FormData();
            for (var prop in $scope.maintenance) {
                formmain.append(prop, $scope.maintenance[prop]);

            }

            $http({
                method: "post",
                url: dataUrl,
                transformRequest: angular.identity,
//              data: JSON.stringify($scope.maintenance),
               data:formmain,
                headers: {
                    'Content-Type': undefined
                },
            }).success(function (result, header, config, status) {
                console.log(result);
                if (!result.success) {
                    toastr.error(result.errors);
                } else {
                    toastr.success($scope.tips);
                }

            })
        }
    }]);

app.directive('ngFocus', [function () {
    var FOCUS_CLASS = "ng-focused";
    return {
        restrict: 'A',
        require: 'ngModel',
        link: function (scope, element, attrs, ctrl) {
            ctrl.$focused = false;
            element.bind('focus', function (evt) {
                element.addClass(FOCUS_CLASS);
                scope.$apply(function () {
                    ctrl.$focused = true;
                })
            }).bind('blur', function (evt) {
                element.removeClass(FOCUS_CLASS);
                scope.$apply(function () {
                    ctrl.$focused = false;
                })
            })
        }
    }
}])
