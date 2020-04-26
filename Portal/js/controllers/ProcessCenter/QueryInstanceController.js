//查询流程实例
app.controller('QueryInstanceController', ['$scope', "$rootScope", "$translate", "$http", "$timeout", "$state", "$filter", "$compile", "ControllerConfig", "datecalculation", "jq.datables",
    function ($scope, $rootScope, $translate, $http, $timeout, $state, $filter, $compile, ControllerConfig, datecalculation, jqdatables) {
        $scope.$on('$viewContentLoaded', function (event) {
            $scope.init();
        });

        $scope.allFlows = [];//wangxg 19.10 当前用户可以查看的全部流程

        $scope.init = function () {
            $scope.UnfinishedText = $translate.instant("InstanceController.Unfinished");
            $scope.FinishedText = $translate.instant("InstanceController.Finished");
            $scope.CanceledText = $translate.instant("InstanceController.Canceled");
            $scope.UnspecifiedText = $translate.instant("InstanceController.Unspecified");

            $scope.Originator = $scope.user.ObjectID;
            $scope.InstanceState = 0;

            // wangxg 19.10 页面加载完先拿到当前用户可以发起 + 查看的全部流程
            //$http.get('Workflow/QueryWorkflowNodes?Mode=WorkflowTemplate&ShowFavorite=false&IsShared=&BizSchema=1')
            //    .then(function (resp) {
            //        if (resp.data && resp.data.length > 0) {
            //            for (var i = 0; i < resp.data.length; i++) {
            //                var children = resp.data[i].children;
            //                for (var j = 0; j < children.length; j++) {
            //                    $scope.allFlows.push(children[j].Code);
            //                }
            //            }
            //        }
            //    });
            
        }

        $scope.getLanguage = function () {
            $scope.LanJson = {
                search: $translate.instant("uidataTable.search"),
                ProcessName: $translate.instant("QueryTableColumn.ProcessName"),
                WorkFlow: $translate.instant("QueryTableColumn.WorkFlow"),
                Originator: $translate.instant("QueryTableColumn.Originator"),
                StartTime: $translate.instant("QueryTableColumn.StartTime"),
                EndTime: $translate.instant("QueryTableColumn.EndTime"),
                sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
                sZeroRecords: $translate.instant("uidataTable.sZeroRecords_NoRecords"),
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
            };

            // wangxg 19.10 为日期范围添加默认值
            var now = new Date();
            now.setMonth(now.getMonth() - 1)
            $scope.StartTime = now.Format('yyyy-MM-dd');
            $scope.EndTime = new Date().Format('yyyy-MM-dd');
        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });
        $scope.WorkflowOptions = {
            Editable: true, Visiable: true, Mode: "WorkflowTemplate", IsMultiple: false, PlaceHolder: $scope.LanJson.WorkFlow, BizSchema:1// wangxg 19.10
        }
        $scope.UserOptions = {
            Editable: true, Visiable: true, OrgUnitVisible: true, V: $scope.user.ObjectID, PlaceHolder: $scope.LanJson.Originator
        }
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
                    return '<a class="like" ui-toggle-class="show" target=".app-aside-right" targeturl="InstanceSheets.html?InstanceId=' + full.InstanceID + '">' + data + '</a>';
                }
            });
            columns.push({ "mData": "WorkflowName", "sClass": "center", });
            columns.push({
                "mData": "OriginatorName",
                "sClass": "center hide1024",
                "mRender": function (data, type, full) {
                    return "<a ng-click=\"showUserInfoModal('" + full.Originator + "');\" new-Bindcompiledhtml>" + data + "</a>";
                }
            });
            columns.push({ "mData": "CreatedTime", "sClass": "center hide414", });
            //审批环节
            columns.push({
                "mData": "ApproverLink",
                "sClass": "center hide1024",
                "mRender": function (data, type, full) {
                    var link = data.split(",");
                    var text = "";
                    if (link.length == 1) {
                        text = data;
                    } else {
                        text = link[0] + "...";
                    }
                    return "<span id=\"tooltip\" data-toggle=\"tooltip\" data-placement=\"left\" class='ApproverLink' title=\"" + data + "\">" + text + "</span>";
                }
            });
            //审批人
            columns.push({
                "mData": "Approver",
                "sClass": "center hide1024",
                "mRender": function (data, type, full) {
                    var link = data.split(",");
                    var text = "";
                    if (full.Exceptional == true) {
                        text = "出现异常，请与管理员联系！";
                    }
                    else if (link.length == 1) {
                        text = data;
                    }
                    else {
                        text = link[0] + "...";
                    }
                    return "<span id=\"tooltip\" data-toggle=\"tooltip\" data-placement=\"left\" class='Approver InstanceExceptionInfo' title=\"" + data + "\">" + text + "</span>";
                }
            });
            //结束时间
            columns.push({
                "mData": "FinishedTime",
                "sClass": "center hide414",
                "mRender": function (data, type, full) {
                    return "<span id=\"FinishedTime\">" + data + "</span>";
                }
            });
            //取消时间
            columns.push({
                "mData": "FinishedTime",
                "sClass": "center hide414",
                "mRender": function (data, type, full) {
                    return "<span id=\"CancelTime\">" + data + "</span>";
                }
            });
            return columns;
        }

        $scope.tabQueryInstance = {
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
            //"sAjaxSource": ControllerConfig.Instance.QueryInstance,
            "sAjaxSource": '/Portal/InstanceEx/QueryInstance',
            "fnServerData": function (sSource, aDataSet, fnCallback) {
                if (!$scope.WorkflowCode) {// wangxg 19.10 
                    // 方案1 = 流程模板必须选择，否则不请求后台
                    //$.notify({ message: '流程模板必须选择', status: "danger" });
                    //$('#tabQueryInstance_processing').hide();
                    //var tem = { Rows: [], sEcho: 1, Total: 0, iTotalDisplayRecords: 0, iTotalRecords: 0 };              
                    //fnCallback(tem);
                    //return;

                    // 方案2 = 流程模板没选择时，自动请求当前用户可以看到的全部流程
                    for (var i = 0; i < aDataSet.length; i++) {
                        var item = aDataSet[i];
                        if (item.name === 'workflowCode') {
                            item.value = $scope.allFlows.join(',');
                        }
                    }
                }
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
                    { "name": "searchKey", "value": $scope.SearchKey },
                    { "name": "workflowCode", "value": $scope.WorkflowCode },
                    { "name": "unitID", "value": $scope.Originator },
                    { "name": "startTime", "value": $filter("date")($scope.StartTime, "yyyy-MM-dd") },
                    { "name": "endTime", "value": $filter("date")($scope.EndTime, "yyyy-MM-dd") },
                    { "name": "instaceState", "value": $scope.InstanceState }
                    );
            },
            "aoColumns": $scope.getColumns(), // 字段定义
            // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
            "initComplete": function (settings, json) {
                var filter = $(".searchContainer");
                filter.find("button").unbind("click.DT").bind("click.DT", function () {
                    $scope.WorkflowCode = $("#sheetWorkflow").SheetUIManager().GetValue();
                    $scope.Originator = $("#sheetUser").SheetUIManager().GetValue();
                    $("#tabQueryInstance").dataTable().fnDraw();
                });
                filter.find("select").unbind("change.Load").bind("change.Load", function () {
                    $scope.WorkflowCode = $("#sheetWorkflow").SheetUIManager().GetValue();
                    $scope.Originator = $("#sheetUser").SheetUIManager().GetValue();
                    $("#tabQueryInstance").dataTable().fnDraw();
                });
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                if (aData.InstanceState == "2" && aData.Approver == "" && aData.ApproverLink == "") {
                    $(nRow).addClass("InstanceException");
                }
                $compile(nRow)($scope);
            },
            //datables被draw完后调用
            "fnDrawCallback": function () {
                $("[data-toggle='tooltip']").tooltip();
                jqdatables.trcss();

                if ($scope.InstanceState == 0) {
                    //进行中
                    this.find("td #FinishedTime").parent().hide();
                    this.find("td #CancelTime").parent().hide();
                }
                if ($scope.InstanceState == 1) {
                    //已完成
                    this.find("td #CancelTime").parent().hide();
                    this.find("td .ApproverLink").parent().hide();
                    this.find("td .Approver").parent().hide();
                    this.find("th[id=FinishedTime]").css("width", "110px");
                }
                if ($scope.InstanceState == 2) {
                    this.find("td #FinishedTime").parent().hide();
                    this.find("td .ApproverLink").parent().hide();
                    this.find("td .Approver").parent().hide();
                    this.find("th[id=CancelTime]").css("width", "110px");
                }
            }
        }
    }]);

// 对Date的扩展，将 Date 转化为指定格式的String   
// 月(M)、日(d)、小时(h)、分(m)、秒(s)、季度(q) 可以用 1-2 个占位符，   
// 年(y)可以用 1-4 个占位符，毫秒(S)只能用 1 个占位符(是 1-3 位的数字)   
// 例子：   
// (new Date()).Format("yyyy-MM-dd hh:mm:ss.S") ==> 2006-07-02 08:09:04.423   
// (new Date()).Format("yyyy-M-d h:m:s.S")      ==> 2006-7-2 8:9:4.18   
Date.prototype.Format = function (fmt) { //author: meizz   
    var o = {
        "M+": this.getMonth() + 1,                 //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时   
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()             //毫秒   
    };
    if (/(y+)/.test(fmt))
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    for (var k in o)
        if (new RegExp("(" + k + ")").test(fmt))
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length == 1) ? (o[k]) : (("00" + o[k]).substr(("" + o[k]).length)));
    return fmt;
}