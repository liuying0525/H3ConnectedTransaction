app.controller('ServerLogController', ['$scope', "$rootScope", "$translate", "$compile", "$http", "$timeout", "$state", "$interval", "$filter", "ControllerConfig", "datecalculation", "jq.datables", "$modal",
    function ($scope, $rootScope, $translate, $compile, $http, $timeout, $state, $interval, $filter, ControllerConfig, datecalculation, jqdatables, $modal) {
        $scope.$on('$viewContentLoaded', function (event) {

        });
        debugger;
        $scope.SeachTime = datecalculation.redDays(new Date(), 0);
        $scope.btQuery = function () {
            var logFolder = $("#logFolder").find("option:selected").text();
            var logNameRule = $("#logNameRule").find("option:selected").val();
            var seachTime = $("#SeachTime").val();
            var pageSize = $("#pageSize").val();
            var pageIndex = $("#pageIndex").val();
            if (seachTime == "") {
                alert("请输入查询时间");
                return;
            }
            if (parseInt(pageIndex) > parseInt(pageSize)) {
                alert("当前页码不能大于最大页码");
                return;
            }
            $.ajax({
                type: "POST",
                url: "/Portal/ServerLog/ReadServerLog?logFolder=" + logFolder + "&logNameRule=" + logNameRule + "&seachTime=" + seachTime + "&pageSize=" + pageSize + "&pageIndex=" + pageIndex,
                contentType: "application/json; charset=utf-8",
                async: false,
                dataType: "json",
                success: function (data) {
                    $("#content").empty();
                    if (data.Success) {
                        var arrjson = data.Data.split('~');
                        arrjson.forEach((item, index) => {
                            $("#content").append("<p>" + item + "</p>");
                        })
                    }
                    else {
                        $("#content").html(data.Data);
                    }
                },
                error: function (msg) {
                    showJqErr(msg);//wangxg 19.8
                    //$("#content").html(msg.responseText + ",异常代码=" + msg.status);
                }
            });
        };
    }]);