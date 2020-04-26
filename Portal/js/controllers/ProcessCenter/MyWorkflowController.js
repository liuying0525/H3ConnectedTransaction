app.controller('MyWorkflowController', ['$scope', '$rootScope', '$translate', '$http', '$state', '$compile', '$interval', '$timeout', 'ControllerConfig', 'jq.datables',
    function ($scope, $rootScope, $translate, $http, $state, $compile, $interval, $timeout, ControllerConfig, jqdatables) {
        window.FrequentEvents = {
            'click .like': function (e, value, row, index) {
                SetFrequent(row, index);
            }
        };

        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });
        $scope.getLanguage = function () {
            $scope.LanJson = {
                bootstrap_table_Loading: $translate.instant("uidataTable.sProcessing"),
                bootstrap_table_NoResult: $translate.instant("uidataTable.sNoResult"),
                search: $translate.instant("uidataTable.search"),

                ProcessName: $translate.instant("QueryTableColumn.ProcessName"),
                Publisheddate: $translate.instant("QueryTableColumn.Publisheddate"),
                Version: $translate.instant("QueryTableColumn.Version"),
                Saveusual: $translate.instant("QueryTableColumn.Saveusual"),
                Initiate: $translate.instant("QueryTableColumn.Initiate"),
                FrequentFlow: $translate.instant("QueryTableColumn.FrequentFlow"),
            }
        }
        $scope.getLanguage();

        //是否是移动端
        var IsMobile = false;
        $scope.bootstrapTableOptions = {
            cache: false,
            url: ControllerConfig.Workflow.QueryWorkflowNodes + "?IsMobile=" + IsMobile + "&random=" + new Date().getTime(),
            columns: [
                { field: "DisplayName", title: $scope.LanJson.ProcessName, formatter: DisplayFormatter },
                { field: "PublishedTime", title: $scope.LanJson.Publisheddate, formatter: DateFormatter, align: "center" },
                { field: "Version", title: $scope.LanJson.Version, formatter: VersionFormatter, width: 80, align: "center" },
                {
                    field: "Frequent", title: $scope.LanJson.Saveusual, formatter: FrequentFormatter, events: FrequentEvents,
                    width: 100, align: "center"
                },
                { field: "DisplayName", title: $scope.LanJson.Initiate, formatter: OperateFormatter, width: 100, align: "center" }
            ],
            treegrid: true,
            striped: false,
            //loadingText: "加载中...",
            loadingText: $scope.LanJson.bootstrap_table_Loading,
            search: $scope.LanJson.search,
            bootstrap_table_NoResult: $scope.LanJson.bootstrap_table_NoResult,
            rowStyle: rowStyle,
            onAll: function () {
                $compile($("#MyWorkflowTable"))($scope);
                $compile($("input"))($scope);
            },
            searchChanged: function () {
                $compile($("#MyWorkflowTable"))($scope);
            }
        }

        function rowStyle(row, index) {
            if (row.IsLeaf == false) {
                return {
                    classes: "whitesmoke"
                }
            } else {
                return {}
            }
        }

        function DisplayFormatter(value, row, index) {
            if (row.IsLeaf == 0) {
                if (value == "FrequentFlow")
                    value = $scope.LanJson.FrequentFlow;
                return '<a class="like"  href="javascript:void(0)" >' + value + '</a>';
            } else {
                return '<a class="like" ui-toggle-class="show" target=".app-aside-right" targeturl="StartInstance.html?WorkflowCode=' + encodeURI(row.Code) + '">' + value + '</a>';;
            }
        }

        function DateFormatter(value, row, index) {
            if (row.IsLeaf == 0) return "";
            if (value == null) return "";
            return value;
        }

        function VersionFormatter(value, row, index) {
            if (row.IsLeaf == 0) return "";
            return value;
        }

        function FrequentFormatter(value, row, index) {
            if (row.IsLeaf == 0) return "";
            if (row.Frequent == 1) {
                return '<a class="like" href="javascript:void(0)" ><i class="fa fa-heart"></i></a>';
            }
            else {
                return '<a class="like" href="javascript:void(0)" ><i class="fa fa-heart-o"></i></a>';
            }
        }

        var SetFrequent = function (row, index) {
            row.Frequent = row.Frequent == 1 ? false : true;
            $http({
                url: ControllerConfig.Workflow.ChangeFrequence + "?WorkflowCode=" + row.Code + "&IsFrequence=" + row.Frequent + "&random=" + new Date().getTime(),
            })
                .success(function (result) {
                    $("#MyWorkflowTable").bootstrapTable('refresh', { silent: true });
                }).error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }

        function OperateFormatter(value, row, index) {
            if (row.IsLeaf == 0) return "";
            return '<a class="like" ui-toggle-class="show" target=".app-aside-right" targeturl="StartInstance.html?WorkflowCode=' + encodeURI(row.Code) + '&PageAction=Close"><span translate="QueryTableColumn.Initiate"></span></a>';
        }
    }]);