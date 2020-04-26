app.controller('QueryParticipantWorkItemController', ['$scope', "$rootScope", "$translate", "$http", "$timeout", "$state", "$filter", "$compile", "ControllerConfig", "datecalculation", "jq.datables",
    function ($scope, $rootScope, $translate, $http, $timeout, $state, $filter, $compile, ControllerConfig, datecalculation, jqdatables) {
        $scope.State = 0;
        $scope.$on("$viewContentLoaded", function (event) {
            $scope.Participant = $scope.user.ObjectID;
            //$scope.name = $translate.instant("WorkItemController.QueryParticipantWorkItem");
        })

        $scope.getLanguage = function () {
            $scope.LanJson = {
                search: $translate.instant("uidataTable.search"),
                ProcessName: $translate.instant("QueryTableColumn.ProcessName"),
                WorkFlow: $translate.instant("QueryTableColumn.WorkFlow"),
                Originator: $translate.instant("QueryTableColumn.Originator"),
                Approver: $translate.instant("QueryTableColumn.Approver"),
                StartTime: $translate.instant("QueryTableColumn.StartTime"),
                EndTime: $translate.instant("QueryTableColumn.EndTime"),
                sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
                sZeroRecords: $translate.instant("uidataTable.sZeroRecords"),
                sInfo: $translate.instant("uidataTable.sInfo"),
                sProcessing: $translate.instant("uidataTable.sProcessing"),
                UnfinishedText: $translate.instant("InstanceState.Unfinished"),
                FinishedText: $translate.instant("InstanceState.Finished"),
                CanceledText: $translate.instant("InstanceState.Canceled"),
                UnspecifiedText: $translate.instant("InstanceState.Unspecified"),

                //权限
                QueryInstanceByProperty_NotEnoughAuth1: $translate.instant("NotEnoughAuth.QueryInstanceByProperty_NotEnoughAuth1"),
                QueryInstanceByProperty_NotEnoughAuth2: $translate.instant("NotEnoughAuth.QueryInstanceByProperty_NotEnoughAuth2"),
                QueryInstanceByProperty_NotEnoughAuth3: $translate.instant("NotEnoughAuth.QueryInstanceByProperty_NotEnoughAuth3"),
                QueryParticipantWorkItem_NoParticipant: $translate.instant("NotEnoughAuth.QueryParticipantWorkItem_NoParticipant"),
                DataFilter_NotEnoughAuth: $translate.instant("NotEnoughAuth.DataFilter_NotEnoughAuth"),
            }
        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        $scope.WorkflowOptions = {
            Editable: true, Visiable: true, Mode: "WorkflowTemplate", IsMultiple: false, PlaceHolder: $scope.LanJson.WorkFlow
        }
        $scope.UserOptions = {
            Editable: true, Visiable: true, OrgUnitVisible: false, V: $scope.user.ObjectID, PlaceHolder: $scope.LanJson.Approver
        }
        // 获取列定义
        $scope.getColumns = function () {
            var columns = [];
            columns.push({
                "mData": "Priority",
                "sClass": "center hide1024",
                "mRender": function (data, type, full) {
                    var rtnstring = "";
                    //紧急程度
                    if (full.Priority == "0") {
                        rtnstring = "<i class=\"glyphicon glyphicon-bell\" style=\"margin-left:5px;\"></i>";
                    } else if (full.Priority == "1") {
                        rtnstring = "<i class=\"glyphicon glyphicon-bell\" style=\"color:green;margin-left:5px;\"></i>";
                    } else {
                        rtnstring = "<i class=\"glyphicon glyphicon-bell\" style=\"color:red;margin-left:5px;\"></i>";
                    }
                    return rtnstring;
                }
            });
            columns.push({
                "mData": "InstanceName",
                "mRender": function (data, type, full) {
                    return "<a ui-toggle-class='show' target='.app-aside-right' targeturl='WorkItemSheets.html?WorkItemID=" + full.ObjectID + "'>" + data + "</a>";

                }
            });
            columns.push({ "mData": "WorkflowName" });
            columns.push({
                "mData": "DisplayName",
                "sClass": "center hide414",
                "mRender": function (data, type, full) {
                    data = data != "" ? data : full.ActivityCode;
                    return "<a ui-toggle-class='show' target='.app-aside-right' targeturl='WorkItemSheets.html?WorkItemID=" + full.ObjectID + "'>" + data + "</a>";
                }
            });
            columns.push({
                "mData": "ParticipantName",
                "sClass": "center hide1024",
                "mRender": function (data, type, full) {
                    return "<a ng-click=\"showUserInfoModal('" + full.Participant + "');\" new-Bindcompiledhtml>" + data + "</a>";
                }
            });
            columns.push({ "mData": "OriginatorName", "sClass": "center hide1024", });

            if ($scope.State == 0) {
                columns.push({ "mData": "ReceiveTime", "sClass": "center hide414" });
            } else {
                columns.push({ "mData": "FinishTime", "sClass": "center hide414" });
            }
            return columns;
        }

        $scope.dtOptions_tabQueryParticipantWorkItem = {
            "bProcessing": true,
            "bServerSide": true,    // 是否读取服务器分页
            "paging": true,         // 是否启用分页
            "bPaginate": true,      // 分页按钮  
            "bLengthChange": false, // 每页显示多少数据
            "bFilter": false,        // 是否显示搜索栏  
            "searchDelay": 1000,    // 延迟搜索
            "iDisplayLength": 10,   // 每页显示行数  
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
            "sAjaxSource": ControllerConfig.WorkItem.QueryParticipantWorkItems,
            "fnServerData": function (sSource, aDataSet, fnCallback) {
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
                        if (json.ExtendProperty != null && json.ExtendProperty.Success == false) {
                            // 没有权限，弹出提示
                            if (json.ExtendProperty.Message == "QueryInstanceByProperty_NotEnoughAuth1") {
                                $.notify({ message: $scope.LanJson.QueryInstanceByProperty_NotEnoughAuth1, status: "danger" });
                            } else if (json.ExtendProperty.Message == "QueryInstanceByProperty_NotEnoughAuth2") {
                                $.notify({ message: $scope.LanJson.QueryInstanceByProperty_NotEnoughAuth2, status: "danger" });
                            } else if (json.ExtendProperty.Message == "QueryInstanceByProperty_NotEnoughAuth3") {
                                $.notify({ message: $scope.LanJson.QueryInstanceByProperty_NotEnoughAuth3, status: "danger" });
                            }
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
                $scope.StartTime = $("#StartTime").val();
                $scope.EndTime = $("#EndTime").val();
                aoData.push(
                    { "name": "instanceName", "value": $scope.InstanceName },
                    { "name": "workflowCode", "value": $scope.WorkflowCode },
                    { "name": "participant", "value": $scope.Participant },
                    { "name": "startTime", "value": $filter("date")($scope.StartTime, "yyyy-MM-dd") },
                    { "name": "endTime", "value": $filter("date")($scope.EndTime, "yyyy-MM-dd") },
                    { "name": "state", "value": $scope.State }
                    );
            },
            "aoColumns": $scope.getColumns(),  // 字段定义           
            // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
            "initComplete": function (settings, json) {
                var filter = $(".searchContainer");
                filter.find("button").unbind("click.DT").bind("click.DT", function () {
                    $scope.WorkflowCode = $("#sheetWorkflow").SheetUIManager().GetValue();
                    $scope.Participant = $("#sheetUser").SheetUIManager().GetValue();
                    $("#tabQueryParticipantWorkItem").dataTable().fnDraw();
                });
                filter.find("select").unbind("change.Load").bind("change.Load", function () {
                    $scope.WorkflowCode = $("#sheetWorkflow").SheetUIManager().GetValue();
                    $scope.Participant = $("#sheetUser").SheetUIManager().GetValue();
                    $("#tabQueryParticipantWorkItem").dataTable().fnDraw();
                });
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $compile(nRow)($scope);
            },
            //datables被draw完后调用
            "fnDrawCallback": function () {
                jqdatables.trcss();
            }
        }
    }]);