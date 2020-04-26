//待办
app.controller('MyUnfinishedWorkItemController', ['$scope', '$rootScope', "$translate", "$http", "$state", "$compile", "$interval", "ControllerConfig", "jq.datables", "$timeout",
    function ($scope, $rootScope, $translate, $http, $state, $compile, $interval, ControllerConfig, jqdatables, $timeout) {
        var PortalRoot = window.localStorage.getItem("H3.PortalRoot");
        $scope.searchKey = "";

        $scope.StartTime = "";
        $scope.EndTime = "";
        //增加内外网，任务优先级
        $scope.UserType = "";
        $scope.ItemPriority = "";

        //是否显示分单按钮；
        $scope.ShowAllocationButton = false;
        //是否可以点击
        $scope.clickEnabled = true;
        //任务池信息
        $scope.TaskPoolInfo = "";

        //进入视图触发
        $scope.$on('$viewContentLoaded', function (event) {
            $scope.searchKey = "";
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
                sZeroRecords: $translate.instant("uidataTable.sZeroRecords_UnfinishedWorkItem"),
                sInfo: $translate.instant("uidataTable.sInfo"),
                sProcessing: $translate.instant("uidataTable.sProcessing")
            }
        }
        $scope.getLanguage();

        // 获取列定义
        $scope.getColumns = function () {
            var columns = [];
            columns.push({
                "mData": "Priority",
                "sClass": "hide1024",
                "mRender": function (data, type, full, a) {
                    var rtnstring = "";
                    //紧急程度
                    if (full.Priority == "0") {
                        rtnstring = "<i class=\"glyphicon glyphicon-bell\" ></i>";
                    } else if (full.Priority == "1") {
                        rtnstring = "<i class=\"glyphicon glyphicon-bell\" style=\"color:green;\"></i>";
                    } else {
                        rtnstring = "<i class=\"glyphicon glyphicon-bell\" style=\"color:red;\"></i>";
                    }
                    //是否催办
                    if (full.Urged == false) {
                        rtnstring = rtnstring + "<a> <i class=\"glyphicon glyphicon-bullhorn\"></i></a>";
                    } else {
                        rtnstring = rtnstring + "<a ng-click=\"showUrgeWorkItemInfoModal('" + full.ObjectID + "')\"> <i class=\"glyphicon glyphicon-bullhorn\" style=\"color:orangered;\"></i></a>";
                    }
                    return rtnstring;
                }
            });
            columns.push({
                "mData": "InstanceName",
                "mRender": function (data, type, full) {
                    return "<td><a ui-toggle-class='show' target='.app-aside-right' targeturl='WorkItemSheets.html?WorkItemID=" + full.ObjectID + "'>" + data + "</a></td>";
                }
            });
            columns.push({
                "mData": "DisplayName",
                "mRender": function (data, type, full) {
                    //打开流程状态
                    data = data != "" ? data : full.ActivityCode;
                    return "<td><a href='index.html#/InstanceDetail/" + full.InstanceId + "/" + full.ObjectID + "/" + "/' target='_blank'>" + data + "</a></td>";
                }
            });
            columns.push({
                "mData": "ReceiveTime", "sClass": "center hide414"
            });
            columns.push({
                "mData": "OriginatorName",
                "sClass": "hide414",
                "mRender": function (data, type, full) {
                    return "<a ng-click=\"showUserInfoModal('" + full.Originator + "');\" new-Bindcompiledhtml>" + data + "</a>";
                }
            });
            columns.push({
                "mData": "OriginatorOUName", "sClass": "hide1024"
            });
            return columns;
        }

        $http({
            method: "POST",
            //url: "/Portal/AutoAllocationOrder/GetAllocationAcl"
            url: "/Portal/AutoAllocationOrderAuth/GetAllocationAcl"//wangxg 19.7
        })
            .success(function (result, header, config, status) {
                $scope.ShowAllocationButton = result;
                if (result) {
                    $scope.getTaskPoolInfo();
                    $interval($scope.getTaskPoolInfo, 5 * 1000);
                }
            })
            .error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });

        $scope.allocationOrder = function () {
            $scope.clickEnabled = false;
            $http({
                method: "POST",
                url: "/Portal/AutoAllocationOrderAuth/GetOrder"//wangxg 19.7
                //url: "/Portal/AutoAllocationOrder/GetOrder"
            })
                .success(function (result, header, config, status) {
                    if (result.code == 1) {
                        //获单成功,再去查询任务池信息;
                        $scope.getTaskPoolInfo();
                        setTimeout(function () {
                            $("#tabUnfinishWorkitem").dataTable().fnDraw();
                        }, 500);
                    }
                    else {
                        alert(result.message);
                    }
                })
                //.error(function (data, header, config, status) {
                //    console.log(data);
                //    alert("获取订单失败");
                //})
                .error(function (data, header, config, status) {// 19.7 
                    var err = '获取订单失败';
                    if (header === 800 || header === 801 || header === 802) {
                        err = data;
                    }
                    alert(err + ',异常代码=' + header);
                });

            //1s之后才可以点击提交
            $timeout(function () {
                $scope.clickEnabled = true;
            }, 1000);
        }

        $scope.getTaskPoolInfo = function () {
            $http({
                method: "POST",
                url: "/Portal/AutoAllocationOrderAuth/GetOrderNumber"//wangxg 19.7
                //url: "/Portal/AutoAllocationOrder/GetOrderNumber"
            })
                .success(function (result, header, config, status) {
                    if (result.code == 1) {
                        $scope.TaskPoolInfo = result.data;
                    }
                })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }

        $scope.options = function () {
            var options = {
                "bProcessing": true,
                "bServerSide": true,    // 是否读取服务器分页
                "paging": true,         // 是否启用分页
                "bPaginate": true,      // 分页按钮  
                "bLengthChange": false, // 每页显示多少数据
                "bFilter": false,        // 是否显示搜索栏  
                "searchDelay": 1000,    // 延迟搜索
                "iDisplayLength": 50,   // 每页显示行数  
                "bSort": true,         // 排序
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
                //"sAjaxSource": ControllerConfig.WorkItem.GetUnfinishWorkItems + "New",
                "sAjaxSource": "WorkItemHandlerAuth/GetUnfinishWorkItemsNew",
                "fnServerData": function (sSource, aDataSet, fnCallback) {
                    console.log(sSource);
                    //debugger;
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
                        "error": function (msg, textStatus, errorThrown) {
                            if (msg.status == 401) {
                                window.location = "/Portal/index.html#/platform/login";
                            } else {//wangxg 19.7
                                showJqErr(msg);
                            }
                        }
                    });
                },
                "sAjaxDataProp": 'Rows',
                "sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
                "sPaginationType": "full_numbers",
                "fnServerParams": function (aoData) {  // 增加自定义查询条件
                    aoData.push({ "name": "keyWord", "value": $scope.searchKey });
                    aoData.push({ "name": "startDate", "value": $("#StartTime").val() });
                    aoData.push({ "name": "endDate", "value": $("#EndTime").val() });
                    aoData.push({ "name": "userType", "value": $scope.UserType });
                    aoData.push({ "name": "itemPriority", "value": $scope.ItemPriority });

                },
                "aoColumns": $scope.getColumns(), // 字段定义
                // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
                "initComplete": function (settings, json) {
                    var filter = $(".searchContainer");
                    filter.find("button.search").unbind("click.DT").bind("click.DT", function () {
                        $("#tabUnfinishWorkitem").dataTable().fnDraw();
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
                    if ($("#tabUnfinishWorkitem").find("tbody>tr:eq(0)").find("td:nth-child(2)>a")[0] && $("#tabUnfinishWorkitem").find("tbody>tr:eq(0)").find("td:nth-child(2)>a")[0].attributes[2].value) {
                        var WorkItemID = "";
                        $("#tabUnfinishWorkitem").find("tbody>tr").each(function () {
                            WorkItemID += "" + $(this).find("td:nth-child(2)>a")[0].attributes[2].value.split("=")[1] + ",";
                            //console.log($(this).find("td:nth-child(2)>a")[0].attributes[2].value.split("=")[1]);
                        })
                        WorkItemID = WorkItemID.substring(0, WorkItemID.length - 1);
                        $.ajax({
                            //url:"ajax/DZBizHandler.ashx",
                            url: "/Portal/DZBizHandlerAuth/getRejectWorkItem",// 19.6.28 wangxg
                            data: { CommandName: "getRejectWorkItem", objectids:WorkItemID} ,
                            type:"POST",
                            async:false,
                            dataType:"json",
                            success: function (result) {
                                if (result && result.length > 0) {
                                    //console.log(result);
                                    $("#tabUnfinishWorkitem").find("tbody>tr").each(function () {
                                        for (var i = 0; i < result.length; i++) {
                                            if ($(this).find("td:nth-child(2)>a")[0].attributes[2].value.split("=")[1] == result[i].OBJECTID) {
                                                $(this).find("td").css("cssText", "background-color: #ccc!important");
                                            }
                                        }
                                    })
                                }
                            },
                            //error: function (msg) {
                            //    alert(msg.responseText + "出错了");
                            //},
                            error: function (msg) {// 19.7 
                                showJqErr(msg);
                            }
                        });
                    }
                    jqdatables.trcss();
                    $compile($("#tabUnfinishWorkitem"))($scope);
                    //重新注册
                    $interval.cancel($scope.interval);
                    $scope.registerInterval();
                }
            }
            return options;
        }

        $scope.searchKeyChange = function () {
            if ($scope.searchKey == "")
                $("#tabUnfinishWorkitem").dataTable().fnDraw();
        }

        $scope.registerInterval = function () {
            $scope.interval = $interval(function () {
                $("#tabUnfinishWorkitem").dataTable().fnDraw();
            }, 90 * 1000);
        }

        $scope.$on("$destroy", function () {
            $interval.cancel($scope.interval);
        })
    }]);
