app.controller('FinancialProductCorrelationController', ['$scope', "$rootScope", "$translate", "$compile", "$http", "$timeout", "$state", "$interval", "$filter", "ControllerConfig", "datecalculation", "jq.datables",
    function ($scope, $rootScope, $translate, $compile, $http, $timeout, $state, $interval, $filter, ControllerConfig, datecalculation, jqdatables) {
        $scope.$on('$viewContentLoaded', function (event) {
            
        });

        $scope.Upload = function () {
            debugger
            if ($scope.CheckFile()) {
                var uploadUrl = window.location.origin + "/WebApi/Upload/ImportFinancialProductCorrelation";
                $.ajaxFileUpload({
                    url: uploadUrl,
                    fileElementId: ['fileFI', 'fileProduct'],
                    type: "post",
                    dataType: 'json',
                    success: function (data) {
                        alert("Success");
                        console.log(data);
                    },
                    error: function (msg) {// 19.7 
                        showJqErr(msg);
                    }
                });
            }
        }

        $scope.CheckFile = function () {
            var fileVal1 = $("#fileFI").val();
            var fileVal2 = $("#fileProduct").val();
            if (fileVal1 == "" || fileVal2 == "") {
                alert("请选择导入文件");
                return false;
            }
            if (fileVal1.toLowerCase().indexOf(".xls") < 0 || fileVal2.toLowerCase().indexOf(".xls") < 0) {
                alert("请选择Excel文件(.xls)");
                return false;
            }
            return true
        }

    }]);