app.controller('CarModelSyncController', ['$scope', "$rootScope", "$translate", "$compile", "$http", "$timeout", "$state", "$interval", "$filter", "ControllerConfig", "datecalculation", "jq.datables", "$modal",
    function ($scope, $rootScope, $translate, $compile, $http, $timeout, $state, $interval, $filter, ControllerConfig, datecalculation, jqdatables, $modal) {
        $scope.$on('$viewContentLoaded', function (event) {

        });
        debugger;
        $scope.enabledClick = true;
        $scope.modelSync = function () {
            $scope.enabledClick = false;
            var url = window.location.origin + "/Portal/CarModel/CarModelSync";//wangxg 19.7
            //var url = window.location.origin + "/WebApi/CarModel/CarModelSync";
            $.ajax({
                type: "post",
                url: url,
                contentType: "application/json; charset=utf-8",
                data: "{usercode:'" + $scope.user.Code + "',username:'" + $scope.user.Name + "'}",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.Success) {
                        alert(data.Msg);
                    }
                    else {
                        alert(data.Msg);
                    }
                    $("#tbSyncLog").dataTable().fnDraw();
                    $("#tabfinishWorkitem").dataTable().fnDraw();
                },
                error: function (msg) {// 19.7 
                    showJqErr(msg);
                }
            })
            $scope.enabledClick = true;
        }
        $scope.modelSyncByVersion = function () {
            var v = $scope.version.split(".");
            if (parseFloat(v[0]) > 3) {

            }
            else if (parseFloat(v[0]) < 3) {
                alert("版本号不能低于3.4.561");
                return false;
            }
            else {//==3
                if (parseFloat(v[1]) > 4) {

                }
                else if (parseFloat(v[1]) == 4) {
                    if (parseFloat(v[2]) > 561) {

                    }
                    else if (parseFloat(v[2]) == 561) {

                    }
                    else {
                        alert("版本号不能低于3.4.561");
                        return false;
                    }
                }
                else {
                    alert("版本号不能低于3.4.561");
                    return false;
                }
            }


            $scope.enabledClick = false;
            var url = window.location.origin + "/Portal/CarModel/CarModelSyncByVersion";//wangxg 19.7
            //var url = window.location.origin + "/WebApi/CarModel/CarModelSyncByVersion";
            $.ajax({
                type: "post",
                url: url,
                contentType: "application/json; charset=utf-8",
                data: "{usercode:'" + $scope.user.Code + "',username:'" + $scope.user.Name + "',version:'" + $scope.version + "'}",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.Success) {
                        alert(data.Msg);
                    }
                    else {
                        alert(data.Msg);
                    }
                    $("#tbSyncLog").dataTable().fnDraw();
                    $("#tabfinishWorkitem").dataTable().fnDraw();
                },
                error: function (msg) {// 19.7 
                    showJqErr(msg);
                }
            })
            $scope.enabledClick = true;
        }
        $scope.getLanguage = function () {
            $scope.LanJson = {
                search: $translate.instant("uidataTable.search"),
                ProcessName: $translate.instant("QueryTableColumn.ProcessName"),
                WorkFlow: $translate.instant("QueryTableColumn.WorkFlow"),
                StartTime: $translate.instant("QueryTableColumn.StartTime"),
                EndTime: $translate.instant("QueryTableColumn.EndTime"),
                sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
                sZeroRecords: "没有找到记录",
                sInfo: $translate.instant("uidataTable.sInfo"),
                sProcessing: $translate.instant("uidataTable.sProcessing")
            }
        }
        $scope.getLanguage();

        $scope.getColumns = function () {
            var columns = [];
            columns.push({
                "mData": "model_name",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">[" + full.series_group_name
                    + "][" + full.series_name + "][" + data + "][" + full.model_id + "]</span></td>";
                }
            });
            columns.push({
                "mData": "upd_flag_dsc",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "upd_comment",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "upd_type",
                "mRender": function (data, type, full) {
                    //upd_type  
                    //0已更新完成
                    //1:调整,2修改,3不处理,4删除/下架

                    //1品牌调整/修改/不处理:11,12,13-->0
                    //2车系调整/不处理:21,23-->0
                    //3车系名称修改/不处理:32,33-->0
                    //4车型名称修改:42-->0
                    //5车型删除,下架:54-->0
                    if (data == "") {
                        if (full.upd_flag == "-1") {
                            return "<td><div style=\"white-space:normal\">" +
                                "<button class='btn btn-default' ng-click=\"model_del('" + full.model_id + "','" + full.upd_flag + "')\">车型下架</button>"
                            "</div></td>";
                        }
                        else if (full.upd_flag == "0") {
                            return "<td><span>历史记录</span></td>";
                        }
                        else {
                            return "<td><div style=\"white-space:normal\">" +
                               "<button class='btn btn-default' ng-click=\"SyncModel('" + full.model_id + "','" + full.upd_flag + "')\">车型同步</button>" +
                               //"<button class='btn btn-default' ng-click=\"SyncIgnore('" + full.model_id + "','" + full.upd_flag + "')\">不做处理</button>" +
                               "</div></td>";
                        }
                    }
                    else {
                        if (data == "0") {
                            return "<td><span>更新完成</span></td>";
                        }
                        else if (data == "2") {
                            return "<td><span style=\"color:red\">直租更新失败</span></td>";
                        }
                        else if (data == "3") {
                            return "<td><span>不处理</span></td>";
                        }
                        //else if (data == "11" || data == "12") {
                        //    if (full.upd_flag == "119" || full.upd_flag == "219"||
                        //        full.upd_flag == "112" || full.upd_flag == "212") {
                        //        return "<td><div style=\"white-space:normal\">" +
                        //       "<button class='btn btn-default' ng-click=\"series_adjust('" + full.model_id + "','" + full.upd_flag + "')\">车系调整</button>" +
                        //       "<button class='btn btn-default' ng-click=\"series_ignore('" + full.model_id + "','" + full.upd_flag + "')\">不做处理</button>" +
                        //       "</div></td>";
                        //    }
                        //    else if (full.upd_flag == "129" || full.upd_flag == "229"||
                        //        full.upd_flag == "122" || full.upd_flag == "222") {
                        //        return "<td><div style=\"white-space:normal\">" +
                        //       "<button class='btn btn-default' ng-click=\"series_nme_chg('" + full.model_id + "','" + full.upd_flag + "')\">车系名称修改</button>" +
                        //       "<button class='btn btn-default' ng-click=\"series_nme_ignore('" + full.model_id + "','" + full.upd_flag + "')\">不做处理</button>" +
                        //       "</div></td>";
                        //    }
                        //    else if (full.upd_flag == "192" || full.upd_flag == "292") {
                        //        return "<td><div style=\"white-space:normal\">" +
                        //       "<button class='btn btn-default' ng-click=\"model_nme_chg('" + full.model_id + "','" + full.upd_flag + "')\">车系调整</button>" +
                        //       "</div></td>";
                        //    }
                        //}
                        //else if (data == "21") {
                        //    if (full.upd_flag == "112" || full.upd_flag == "212" || full.upd_flag == "912") {
                        //        return "<td><div style=\"white-space:normal\">" +
                        //       "<button class='btn btn-default' ng-click=\"model_nme_chg('" + full.model_id + "','" + full.upd_flag + "')\">车系调整</button>" +
                        //       "</div></td>";
                        //    }
                        //}
                        //else if (data == "32") {
                        //    if (full.upd_flag == "122" || full.upd_flag == "222" || full.upd_flag == "922") {
                        //        return "<td><div style=\"white-space:normal\">" +
                        //       "<button class='btn btn-default' ng-click=\"model_nme_chg('" + full.model_id + "','" + full.upd_flag + "')\">车系调整</button>" +
                        //       "</div></td>";
                        //    }
                        //}
                    }
                }
            });
            columns.push({
                "mData": "upd_username",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "upd_time",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "upd_result",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "version_id",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "version_time",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            return columns;
        }
        $scope.getSyncLogColumns = function () {
            var columns = [];
            columns.push({ "mData": "current_version" });
            columns.push({ "mData": "latest_version" });
            columns.push({ "mData": "sync_date" });
            columns.push({
                "mData": "sync_result",
                "mRender": function (data, type, full) {
                    return "<td><span style=\"white-space:normal\">" + data + "</span></td>";
                }
            });
            columns.push({
                "mData": "sync_result_code",
                "mRender": function (data, type, full) {
                    if (data == "") {
                        return "<td><span style=\"white-space:normal\">无记录</span></td>";
                    }
                    else if (data == "-20111") {
                        return "<td><span style=\"white-space:normal\">无需要同步的车型</span></td>";
                    }
                    else {
                        return "<td><button class='btn btn-default' ng-click=\"openSyncDetail('" + data + "')\">详细</button></td>";
                    }
                }
            });
            columns.push({
                "mData": "rent_sync_result_code",
                "mRender": function (data, type, full) {
                    if(data == "") {
                        return "<td><span style=\"white-space:normal\">无记录</span></td>";
                    }
                    else if (data == "-20010") {
                        return "<td><span style=\"white-space:normal\">无需要同步的车型</span></td>";
                    }
                    else {
                        return "<td><button class='btn btn-default' ng-click=\"openSyncDetail_Rent('" + data + "')\">详细</button></td>";
                    }
                }
            });
            columns.push({
                "mData": "che300_data_code",
                "mRender": function (data, type, full) {
                    if (data == "") {
                        return "<td><span style=\"white-space:normal\">无记录</span></td>";
                    }
                    else {
                        return "<td><button class='btn btn-default' ng-click=\"openChe300Data('" + data + "')\">详细</button></td>";
                    }
                }
            });
            columns.push({ "mData": "sync_user_name" });
            return columns;
        }

        $scope.dtOptions_tabfinishWorkitem = {
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
            "sAjaxSource": window.location.origin + "/Portal/CarModel/GetModel_BrandOrSeries_Change",//wangxg 19.7
            //"sAjaxSource": window.location.origin + "/WebApi/CarModel/GetModel_BrandOrSeries_Change",
            "fnServerData": function (sSource, aDataSet, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "type": 'POST',
                    "url": sSource,
                    "data": aDataSet,
                    "success": function (json) {
                        if (json.Success) {
                            fnCallback(json);
                        }
                    },
                    "error": function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status == 401) {
                            window.location = "/Portal/index.html#/platform/login";
                        }
                    }
                });
            },
            "sAjaxDataProp": 'Rows',
            "sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
            "sPaginationType": "full_numbers",
            "fnServerParams": function (aoData) {  // 增加自定义查询条件

                aoData.push(
                    //{ "name": "startTime", "value": $filter("date")($scope.StartTime, "yyyy-MM-dd") },
                    //{ "name": "endTime", "value": $filter("date")($scope.EndTime, "yyyy-MM-dd") },
                    //{ "name": "workflowCode", "value": $scope.WorkflowCode },
                    //{ "name": "instanceName", "value": $scope.InstanceName },
                    //{ "name": "distinct", "value": $scope.ck_Distinct }//增加查询条件，是否去重：add by chenghs 2018-0-28
                    );
            },
            "aoColumns": $scope.getColumns(), // 字段定义
            // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
            "initComplete": function (settings, json) {
                var filter = $("#btn_Query_FinishWorkitem");
                filter.find("button").unbind("click.DT").bind("click.DT", function () {

                    $("#tabfinishWorkitem").dataTable().fnDraw();
                });
            },
            //创建行，未绘画到屏幕上时调用
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //将添加的angular事件添加到作用域中
                $compile(nRow)($scope);
            },
            "fnDrawCallback": function () {
                jqdatables.trcss();
            }
        }

        $scope.dtOptions_tbSyncLog = {
            "bProcessing": true,
            "bServerSide": true,    // 是否读取服务器分页
            "paging": true,         // 是否启用分页
            "bPaginate": true,      // 分页按钮  
            "bLengthChange": false, // 每页显示多少数据
            "bFilter": false,        // 是否显示搜索栏  
            "searchDelay": 1000,    // 延迟搜索
            "iDisplayLength": 5,   // 每页显示行数  
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
            "sAjaxSource": window.location.origin + "/Portal/CarModel/GetChe300SyncResult",//wangxg 19.7
            //"sAjaxSource": window.location.origin + "/WebApi/CarModel/GetChe300SyncResult",
            "fnServerData": function (sSource, aDataSet, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "type": 'POST',
                    "url": sSource,
                    "data": aDataSet,
                    "success": function (json) {
                        if (json.Success) {
                            fnCallback(json);
                        }
                    },
                    "error": function (XMLHttpRequest, textStatus, errorThrown) {
                        if (XMLHttpRequest.status == 401) {
                            window.location = "/Portal/index.html#/platform/login";
                        }
                    }
                });
            },
            "sAjaxDataProp": 'Rows',
            "sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
            "sPaginationType": "full_numbers",
            "fnServerParams": function (aoData) {  // 增加自定义查询条件

                aoData.push(
                    //{ "name": "startTime", "value": $filter("date")($scope.StartTime, "yyyy-MM-dd") },
                    //{ "name": "endTime", "value": $filter("date")($scope.EndTime, "yyyy-MM-dd") },
                    //{ "name": "workflowCode", "value": $scope.WorkflowCode },
                    //{ "name": "instanceName", "value": $scope.InstanceName },
                    //{ "name": "distinct", "value": $scope.ck_Distinct }//增加查询条件，是否去重：add by chenghs 2018-0-28
                    );
            },
            "aoColumns": $scope.getSyncLogColumns(), // 字段定义
            // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
            "initComplete": function (settings, json) {
                var filter = $("#btn_Query_FinishWorkitem");
                filter.find("button").unbind("click.DT").bind("click.DT", function () {

                    $("#tabfinishWorkitem").dataTable().fnDraw();
                });
            },
            //创建行，未绘画到屏幕上时调用
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                //将添加的angular事件添加到作用域中
                $compile(nRow)($scope);
            },
            "fnDrawCallback": function () {
                jqdatables.trcss();
            }
        }

        //打开CMS新增明细的界面
        $scope.openSyncDetail = function (id) {
            var modalInstance = $modal.open({
                templateUrl: 'SyncDetail.html',//script标签中定义的id
                controller: 'SyncDetailController',//modal对应的Controller
                size: "lg",
                resolve: {
                    data: function () {//data作为modal的controller传入的参数
                        return { id: id, type: 0 };//用于传递数据
                    }
                }
            })
        }

        //打开CMS新增明细的界面
        $scope.openSyncDetail_Rent = function (id) {
            if (id < 0) {
                alert("同步失败:" + id);
                return false;
            }
            var modalInstance = $modal.open({
                templateUrl: 'SyncDetail_Rent.html',//script标签中定义的id
                controller: 'SyncDetailController',//modal对应的Controller
                size: "lg",
                resolve: {
                    data: function () {//data作为modal的controller传入的参数
                        return { id: id, type: 2 };//用于传递数据
                    }
                }
            })
        }

        //打开车型批量同步的界面
        $scope.openModelSync = function () {
            var modalInstance = $modal.open({
                templateUrl: 'ModelSync.html',//script标签中定义的id
                controller: 'ModelSyncController',//modal对应的Controller
                size: "lg",
                resolve: {
                    //data: function () {//data作为modal的controller传入的参数
                    //    return { id: id, type: 0 };//用于传递数据
                    //}
                }
            })
        }
        
        //打开Che300数据源的界面
        $scope.openChe300Data = function (id) {
            var modalInstance = $modal.open({
                templateUrl: 'Che300Data.html',//script标签中定义的id
                controller: 'SyncDetailController',//modal对应的Controller
                size: "lg",
                resolve: {
                    data: function () {//data作为modal的controller传入的参数
                        return { id: id, type: 1 };//用于传递数据
                    }
                }
            })
        }

        //人工操作
        $scope.UpdateUpdState = function (type, id, upd_flag) {
            var paras = {
                type: type,
                model_id: id,
                upd_flag: upd_flag,
                usercode: $scope.user.Code,
                username: $scope.user.Name
            };
            $http({
                method: 'POST',
                url: "../Portal/CarModel/UpdateUpdState",//wangxg 19.7
                //url: "../WebApi/CarModel/UpdateUpdState",
                data: JSON.stringify(paras)
            }).then(function successCallback(response) {
                if (response.data.Success) {
                    $("#tabfinishWorkitem").dataTable().fnDraw();
                }
                else {
                    alert(response.data.Msg);
                }

            }, function errorCallback(response) {//wangxg 19.7
                // 请求失败执行代码
                //alert("执行失败，请联系管理员");

                var err = '执行失败，请联系管理员';
                if (response.status === 800 || response.status === 801 || response.status === 802) {
                    err = response.data;
                }
                alert(err + ',异常代码=' + response.status);

            });
        }

        $scope.SyncModel = function (id, upd_flag) {
            $scope.UpdateUpdState("sync", id, upd_flag);
        }

        $scope.SyncIgnore = function (id, upd_flag) {
            $scope.UpdateUpdState("ignore", id, upd_flag);
        }

        $scope.model_del = function (id, upd_flag) {
            $scope.UpdateUpdState("sync", id, upd_flag);
        }
    }]);

app.controller('SyncDetailController', ['$scope', "$modal", "$modalInstance", "$http", "data",
function ($scope, $modal, $modalInstance, $http, data) {
    $scope.data = data;
    //在这里处理要进行的操作   
    $scope.ok = function () {
        $modalInstance.close();
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    }

    //SyncDetail
    if ($scope.data.type == 0) {
        $http({
            method: 'GET',
            url: '../Portal/CarModel/GetSyncDetail?seq_id=' + $scope.data.id//wangxg 19.7
            //url: '../WebApi/CarModel/GetSyncDetail?seq_id=' + $scope.data.id
        }).then(function successCallback(response) {
            debugger;
            $scope.SyncDetailData = response.data;
            }, function errorCallback(response) {//wangxg 19.7
                showJqErr(response);
        });
    }//Che300Data
    else if ($scope.data.type == 1) {
        $http({
            method: 'GET',
            url: '../Portal/CarModel/GetChe300SyncData?id=' + $scope.data.id//wangxg 19.7
            //url: '../WebApi/CarModel/GetChe300SyncData?id=' + $scope.data.id
        }).then(function successCallback(response) {
            debugger;
            $scope.Che300Data = JSON.parse(response.data);
        }, function errorCallback(response) {//wangxg 19.7
            showJqErr(response);
        });
    }
    else if ($scope.data.type == 2) {
        $http({
            method: 'GET',
            url: '../Portal/CarModel/GetSyncDetail_Rent?seq_id=' + $scope.data.id//wangxg 19.7
            //url: '../WebApi/CarModel/GetSyncDetail_Rent?seq_id=' + $scope.data.id
        }).then(function successCallback(response) {
            debugger;
            $scope.RentData = response.data;
        }, function errorCallback(response) {//wangxg 19.7
            showJqErr(response);
        });
    }
}]);

app.controller('ModelSyncController', ['$scope', "$modal", "$modalInstance", "$http",
function ($scope, $modal, $modalInstance, $http) {
    //在这里处理要进行的操作   
    $scope.ok = function () {
        $modalInstance.close();
    };
    $scope.cancel = function () {
        $modalInstance.dismiss('cancel');
    }
    $scope.BrandList;
    $scope.SeriesList;
    $scope.ModelList;
    $scope.ModelListAll = [];
    $http({
        method: 'GET',
        url: '../Portal/CarModel/GetBrandList'//wangxg 19.7
        //url: '../WebApi/CarModel/GetBrandList'
    }).then(function successCallback(response) {
        if (response.data.Success) {
            $scope.BrandList = response.data.Data;
        }
    }, function errorCallback(response) {//wangxg 19.7
        showJqErr(response);
    });

    $scope.brand_chg = function () {
        if ($scope.Brand == "") {
            $scope.SeriesList = [];
            $scope.ModelList = [];
            $("#tb_model_info tbody tr").empty();
            return false;
        }
        $http({
            method: 'GET',
            url: '../Portal/CarModel/GetSeriesList?brand_id=' + $scope.Brand//wangxg 19.7
            //url: '../WebApi/CarModel/GetSeriesList?brand_id=' + $scope.Brand
        }).then(function successCallback(response) {
            if (response.data.Success) {
                $scope.SeriesList = response.data.Data;
            }
        }, function errorCallback(response) {//wangxg 19.7
            showJqErr(response);
        });
    }

    $scope.series_chg = function () {
        if ($scope.Series == "") {
            $scope.ModelList = [];
            $("#tb_model_info tbody tr").empty();
            return false;
        }
        $http({
            method: 'GET',
            url: '../Portal/CarModel/GetModelList?series_id=' + $scope.Series//wangxg 19.7
            //url: '../WebApi/CarModel/GetModelList?series_id=' + $scope.Series
        }).then(function successCallback(response) {
            if (response.data.Success) {
                $scope.ModelList = response.data.Data;
                $scope.ModelListAll = response.data.Data;
            }
        }, function errorCallback(response) {//wangxg 19.7
            showJqErr(response);
        });
    }
    //车型名称修改
    $scope.modelname_chg = function () {
        if ($scope.Series == "" || $scope.Brand == "") {
            $("#tb_model_info tbody tr").empty();
            return false;
        }
        $scope.ModelListNew = $scope.ModelListAll;
        $scope.ModelList = [];
        $($scope.ModelListNew).each(function (n, v) {
            if (v.model_name.indexOf($scope.modelname) > -1) {
                $scope.ModelList.push(v);
            }
        });
        //$http({
        //    method: 'GET',
        //    url: '../WebApi/CarModel/GetModelList?series_id=' + $scope.Series
        //}).then(function successCallback(response) {
        //    if (response.data.Success) {
        //        $scope.ModelList = [];
        //        $(response.data.Data).each(function (n, v) {
        //            if (v.model_name.indexOf($scope.modelname) > -1) {
        //                $scope.ModelList.push(v);
        //            }
        //        });
        //    }
        //}, function errorCallback(response) {
        //    // 请求失败执行代码
        //});
    }
    //同步（批量）
    $scope.batchSync = function () {
        var checked_models = [];
        $("#tb_model_info tbody tr input[type='checkbox']").each(function (n, v) {
            if (v.checked) {
                checked_models.push(v.value);
            }
        });
        if (checked_models.length == 0) {
            alert("请选择至少一个车型");
            return false;
        }
        if (confirm("确认同步？")) {
            $http({
                method: 'POST',
                //url: '../WebApi/CarModel/SyncModelBatch',
                url: '../Portal/CarModel/SyncModelBatch',//wangxg 19.7
                data: {
                    model_ids: checked_models
                }
            }).then(function successCallback(response) {
                var result = response.data;
                if (result.Success) {
                    $(result.Data).each(function (n, v) {
                        if (v.Ope_State == "2") {
                            $("#sp_cms_" + v.Model_Id).addClass("glyphicon glyphicon-chevron-right").css("color", "yellow").css("cursor", "pointer").attr("title", "需要手工处理");
                            $("#sp_rent_" + v.Model_Id).addClass("glyphicon glyphicon-chevron-right").css("color", "yellow").css("cursor", "pointer").attr("title", "需要手工处理");
                        }
                        else if (v.Ope_State == "1") {
                            if (v.CMS_State) {
                                $("#sp_cms_" + v.Model_Id).addClass("glyphicon glyphicon-ok").css("color", "green").css("cursor", "pointer").attr("title", v.CMS_Msg);
                            }
                            else {
                                $("#sp_cms_" + v.Model_Id).addClass("glyphicon glyphicon-remove").css("color", "red").css("cursor", "pointer").attr("title", v.CMS_Msg);
                            }

                            if (v.Rent_State) {
                                $("#sp_rent_" + v.Model_Id).addClass("glyphicon glyphicon-ok").css("color", "green").css("cursor", "pointer").attr("title", v.Rent_Msg);
                            }
                            else {
                                $("#sp_rent_" + v.Model_Id).addClass("glyphicon glyphicon-remove").css("color", "red").css("cursor", "pointer").attr("title", v.Rent_Msg);
                            }
                        }
                    });
                }
                else {
                    alert(result.Msg);
                }
            }, function errorCallback(response) {//wangxg 19.7
                showJqErr(response);
            });
        }
    }
    //选择所有
    $scope.selectAll = function () {
        if ($("#ckAll")[0].checked) {
            $("#tb_model_info tbody tr input[type='checkbox']").each(function (n, v) {
                v.checked = true;
            });
        }
        else {
            $("#tb_model_info tbody tr input[type='checkbox']").each(function (n, v) {
                v.checked = false;
            });
        }
    }

    $scope.selectOne = function (con, $event) {
        var d = con.data;
        var ck = $("#ck_" + d.model_id)[0];
        if (ck.checked) {
            ck.checked = false;
        }
        else {
            ck.checked =true;
        }
    }
}]);