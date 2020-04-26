//待办
app.controller('ProposalItemsController', ['$scope', '$rootScope', "$translate", "$http", "$state", "$compile", "$interval", "ControllerConfig", "jq.datables", "datecalculation",
    function ($scope, $rootScope, $translate, $http, $state, $compile, $interval, ControllerConfig, jqdatables, datecalculation) {
        var PortalRoot = window.localStorage.getItem("H3.PortalRoot");

        $scope.StartTime = "";
        $scope.EndTime = "";

        $scope.StartTime = datecalculation.redDays(new Date(), 7);
        $scope.EndTime = datecalculation.addDays(new Date(), 1);


        // 获取列定义
        $scope.getColumns = function () {
            var columns = [];
            columns.push({
                "mData": "INSTANCEID",
                "mRender": function (data, type, full) {
                    return "<td class=\"text-align\"><input type='radio' ng-model=\"instance_id\" value='" + data + "' name='instance_id'/></td>";
                }
            });
            columns.push({
                "mData": "APPLICATION_NUMBER", "sClass": "hide414"
            });
            columns.push({
                "mData": "APPLICATION_NAME", "sClass": "hide414"
            });
            columns.push({
                "mData": "APPLICATION_TYPE_CODE",
                "sClass": "hide414",
                "mRender": function (data, type, full) {
                    var t = "";
                    if (data == "00001")
                        t = "个人Individual";
                    else if (data == "00002")
                        t = "公司Company";
                    return "<td><a ui-toggle-class='show' target='.app-aside-right' targeturl='InstanceSheets.html?InstanceId=" + full.INSTANCEID + "' style=\"text-decoration:underline;text-decoration-color:#ccc\">" + t + "</a></td>";
                }
            });
            columns.push({
                "mData": "CREATEDTIME", "sClass": "hide414",
                "mRender": function (data, type, full) {
                    if (data.length>10)
                        return data.substring(0, 10);
                    else
                        return data;
                }
            });
            columns.push({
                "mData": "STATE",
                "sClass": "hide414",
                "mRender": function (data, type, full) {
                    if (data == 2)
                        return "进行中";
                    else if (data == 4)
                        return "已完成";
                    else if (data == 5)
                        return "已取消";
                    else
                        return data;
                }
            });
            columns.push({
                "mData": "CAP_STATE", "sClass": "hide414",
                "mRender": function (data, type, full) {
                    if (data == "06")
                        return "新的";
                    else if (data == "78")
                        return "覆盖批准";
                    else if (data == "84")
                        return "修改-更新";
                    else if (data == "14")
                        return "草稿";
                    else if (data == "04")
                        return "取消";
                    else if (data == "02")
                        return "拒绝";
                    else if (data == "05")
                        return "待决定";
                    else if (data == "01")
                        return "核准";
                    else if (data == "07")
                        return "进行中";
                    else if (data == "97")
                        return "驳回";
                    else if (data == "55")
                        return "转换";
                    else
                        return data;
                }
            });
            columns.push({
                "mData": "ASSET_MAKE_DSC", "sClass": "hide414"
            });            
            columns.push({
                "mData": "POWER_PARAMETER", "sClass": "hide414"
            });
            columns.push({
                "mData": "FINANCIAL_PRODUCT_NAME", "sClass": "hide414"
            });
            columns.push({
                "mData": "AMOUNT_FINANCED", "sClass": "hide414"
            });
            return columns;
        }

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

        $scope.options = function () {
            var options = {
                "bProcessing": true,
                "bServerSide": true,    // 是否读取服务器分页
                "paging": true,         // 是否启用分页
                "bPaginate": true,      // 分页按钮  
                "bLengthChange": false, // 每页显示多少数据
                "bFilter": false,        // 是否显示搜索栏  
                "searchDelay": 1000,    // 延迟搜索
                "iDisplayLength": 10,   // 每页显示行数  
                //"bSort": false,         // 排序
                "aaSorting": [[4, "desc"]],//默认第几列排序
                "aoColumnDefs": [     // 指定列那些列参与排序
                    { "orderable": false, "aTargets": [0, 1, 2, 3, 5, 6, 7, 8, 9] }
                ],
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
                "sAjaxSource": "/Portal/ProposalAuth/GetProposalDetail",//wangxg 19.7
                //"sAjaxSource": "/Portal/Proposal/GetProposalDetail",
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
                        "error": function (msg,textStatus,errorThrown) {
                            if (msg.status == 401) {
                                window.location = "/Portal/index.html#/platform/login";
                            } else {
                                showJqErr(msg);
                            }
                        }
                    });
                },
                "sAjaxDataProp": 'Rows',
                "sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
                "sPaginationType": "full_numbers",
                "fnServerParams": function (aoData) {  // 增加自定义查询条件
                    aoData.push({ "name": "app_no", "value": $scope.application_no });
                    aoData.push({ "name": "app_name", "value": $scope.application_name });
                    aoData.push({ "name": "from_dte", "value": $("#StartTime").val() });
                    aoData.push({ "name": "to_dte", "value": $("#EndTime").val() });
                },
                "aoColumns": $scope.getColumns(), // 字段定义
                // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
                "initComplete": function (settings, json) {
                    var filter = $(".searchContainer");
                    filter.find("button").unbind("click.DT").bind("click.DT", function () {
                        $("#tabUnfinishWorkitem").dataTable().fnDraw();
                    });
                },
                //创建行，未绘画到屏幕上时调用
                "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                    $(nRow).unbind("click").bind("click", function () {
                        var v = $(this).find("input[type='radio']").val();
                        $("#btn_copy").attr("targeturl", "StartInstance.html?WorkflowCode=APPLICATION&CopyId=" + v);
                        $("#btn_copy").removeClass("hidden");
                        $compile($("#btn_copy"))($scope);
                        $(this).find("input[type='radio']").prop("checked", true);
                    });

                },
                //datables被draw完后调用
                "fnDrawCallback": function () {
                    jqdatables.trcss();
                    $compile($("#tabUnfinishWorkitem"))($scope);
                }
            }
            return options;
        }

        $scope.searchKeyChange = function () {
            if ($scope.searchKey == "")
                $("#tabUnfinishWorkitem").dataTable().fnDraw();
        }
        
        $scope.setTime_cdt = function () {
            if ($scope.week_month == "week") {
                $scope.StartTime = datecalculation.redDays(new Date(), 7);
                $scope.EndTime = datecalculation.addDays(new Date(), 1);
            }
            else {
                $scope.StartTime = datecalculation.redDays(new Date(), 30);
                $scope.EndTime = datecalculation.addDays(new Date(), 1);
            }
            
        }
    }]);
