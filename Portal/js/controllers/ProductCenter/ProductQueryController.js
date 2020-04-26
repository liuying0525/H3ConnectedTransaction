app.controller('ProductQueryController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig", "$interval", "jq.datables",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig, $interval, jqdatables) {

        $scope.SearchKey = {
            ProductName: "",
            StartTime: "",
            EndTime: ""
        };

        //进入视图触发
        $scope.$on('$viewContentLoaded', function (event) {
            //$scope.searchKey = "";
        });
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        $scope.getLanguage = function () {
            $scope.LanJson = {
                search: $translate.instant("uidataTable.search"),
                sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
                sZeroRecords: "未查询到数据",
                sInfo: $translate.instant("uidataTable.sInfo"),
                sProcessing: $translate.instant("uidataTable.sProcessing")
            }
        }
        $scope.getLanguage();


        // 获取列定义
        $scope.getColumns = function () {
            var columns = [];
            columns.push({
                "mData": "RowIndex",
                "sClass": "hide1024"
            });
            columns.push({
                "mData": "ProductName",
                "sClass": "hide1024"
            });
            columns.push({
                "mData": "ProductShortName",
                "sClass": "hide414"
            });
            columns.push({
                "mData": "StartTime",
                "sClass": "hide414"
            });
            columns.push({
                "mData": "EndTime", "sClass": "hide414"
            });
            columns.push({
                "mData": "GroupName",
                "sClass": "hide414"
            });
            columns.push({
                "mData": "CustomerRate", "sClass": "hide1024"
            });
            columns.push({
                "mData": "ActualRate", "sClass": "hide1024"
            });
            columns.push({
                "mData": "SubsidyRate", "sClass": "hide1024"
            });
            return columns;
        }


        $scope.options = function () {
            var options = {
                "lengthMenu": [10, 20, 50, 100],
                "bProcessing": true,
                "bServerSide": true,    // 是否读取服务器分页
                "paging": true,         // 是否启用分页
                "bPaginate": true,      // 分页按钮  
                "bLengthChange": true, // 每页显示多少数据
                "bFilter": false,        // 是否显示搜索栏  
                "searchDelay": 1000,    // 延迟搜索
                "iDisplayLength": 20,   // 每页显示行数  
                "bSort": false,         // 排序
                "singleSelect": true,
                "bInfo": true,          // Showing 1 to 10 of 23 entries 总记录数没也显示多少等信息  
                "pagingType": "full_numbers",  // 设置分页样式，这个是默认的值
                "language": {           // 语言设置
                    "sLengthMenu": $scope.LanJson.sLengthMenu,
                    "sZeroRecords": "<i class=\"icon-emoticon-smile\"></i>" + $scope.LanJson.sZeroRecords,
                    "sInfo": $scope.LanJson.sInfo,
                    "infoEmpty": "",
                    "sProcessing": $scope.LanJson.sProcessing,
                    "paginate": {
                        "first": "<<",
                        "last": ">>",
                        "previous": "<",
                        "next": ">"
                    }
                },
                "sAjaxSource": "ProductCenter/ProductQuery",
                "fnServerData": function (sSource, aDataSet, fnCallback) {
                    console.log(sSource);
                    $.ajax({
                        "dataType": 'json',
                        "type": 'POST',
                        "url": sSource,
                        "data": aDataSet,
                        "success": function (json) {
                            if (json.ExceptionCode == 1 && json.Success == false) {
                                json.Rows = [];
                                json.sEcho = 1;
                                json.Total = 0;
                                json.iTotalDisplayRecords = 0;
                                json.iTotalRecords = 0;
                                $state.go("platform.login");
                            }
                            fnCallback(json);
                        },
                        "error": function (XMLHttpRequest, textStatus, errorThrown) {
                            if (XMLHttpRequest.status == 401) {
                                window.location = "/Portal/index.html#/platform/login";
                            } else showJqErr(XMLHttpRequest);
                        }
                    });
                },
                "sAjaxDataProp": 'Rows',
                "sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
                "sPaginationType": "full_numbers",
                "fnServerParams": function (aoData) {  // 增加自定义查询条件
                    aoData.push({ "name": "ProductName", "value": $scope.SearchKey.ProductName });
                    aoData.push({ "name": "StartTime", "value": $("#StartTime").val() });
                    aoData.push({ "name": "EndTime", "value": $("#EndTime").val() });

                },
                "aoColumns": $scope.getColumns(), // 字段定义
                // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
                "initComplete": function (settings, json) {
                    var filter = $(".searchContainer");
                    filter.find("button").unbind("click.DT").bind("click.DT", function () {
                        $("#tabProductQueryItem").dataTable().fnDraw();
                    });
                },
                //创建行，未绘画到屏幕上时调用
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    //将添加的angular事件添加到作用域中
                    if (aData.ItemSummary != "") {
                        $(nRow).attr("title", aData.ItemSummary);
                        //angular-tooltip暂不可用
                        //$(nRow).attr("tooltips", "");
                        //$(nRow).attr("tooltip-template", aData.ItemSummary);
                        //$(nRow).attr("tooltip-side", "bottom");
                    }
                },
                //datables被draw完后调用
                "fnDrawCallback": function () {
                    
                }
            }
            return options;
        }

        $scope.registerInterval = function () {
            $scope.interval = $interval(function () {
                $("#tabProductQueryItem").dataTable().fnDraw();
            }, 90 * 1000);
        }

        $scope.$on("$destroy", function () {
            $interval.cancel($scope.interval);
        })

        $scope.clearSearch = function () {
            $scope.SearchKey = {
                ProductName: "",
                StartTime: "",
                EndTime: ""
            };
            $("#StartTime").val("");
            $("#EndTime").val("");
        }

        $scope.export = function () {
            var start = $("#StartTime").val();
            var end = $("#EndTime").val();
            $http({
                method: 'get',
                url: '/Portal/ProductCenter/ProductQueryExport?ProductName=' + $scope.SearchKey.ProductName + "&StartTime=" + start + "&EndTime=" + end,
                headers: { 'Content-Type': 'application/vnd.ms-excel' },
                responseType: 'arraybuffer'
            }).success(function (result) {
                if (result == "" || result == null) {
                    $.notify({ message: "未查询到产品数据", status: "danger" });
                }
                else {
                    var b = new Blob([result]);
                    var r = new FileReader();
                    r.readAsText(b, 'utf-8');

                    sleep(2000).then(() => {
                        try {
                            //console.log(r.result);
                            console.log(r.result.indexOf("code"));
                            if (r.result.indexOf("code") >= 0) {
                                //var jsonResult = JSON.parse(r.result);
                                $.notify({ message: "无法导出超过1万条数据", status: "danger" });
                                return;
                            }
                        }
                        catch (ex) {
                            successFlag = true
                        }

                        var blob = new Blob([result], { type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
                        var objectUrl = URL.createObjectURL(blob);
                        var a = document.createElement("a");
                        document.body.appendChild(a);
                        a.style = "display: none";
                        a.href = objectUrl;
                        a.download = '产品';
                        a.click();
                        document.body.removeChild(a);
                    });
                    
                }
            });

            //window.open('/Portal/ProductCenter/ProductQueryExport?ProductName=' + $scope.SearchKey.ProductName + "&StartTime=" + $scope.SearchKey.StartTime + "&EndTime=" + $scope.SearchKey.EndTime);
        }

        function sleep(ms) {
            return new Promise(function (resolve, reject) {
                setTimeout(resolve, ms)
            })
        }

    }]);