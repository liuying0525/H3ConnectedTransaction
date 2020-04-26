/*
    应用中心-查询列表
*/
app.controller('BizQueryViewController', ['$rootScope', '$scope', '$http', '$state', '$stateParams', '$translate', '$modal', '$compile', '$timeout', '$filter', '$interval', 'jq.datables', 'datecalculation', 'ControllerConfig',
function ($rootScope, $scope, $http, $state, $stateParams, $translate, $modal, $compile, $timeout, $filter, $interval, jqdatables, datecalculation, Controller) {
    var PortalRoot = window.localStorage.getItem("H3.PortalRoot");
    $scope.GridData = [];
    $scope.DisplayFormats = [];
    //进入视图触发
    $scope.$on('$viewContentLoaded', function (event) {
        $scope.DisplayFormats = [];
        $scope.init();
    });
    $scope.init = function () {
        $scope.SchemaCode = $stateParams.SchemaCode;
        $scope.QueryCode = $stateParams.QueryCode;
        $scope.FunctionCode = $stateParams.FunctionCode;
        $.ajax({
            async: false,
            type: "POST",
            url: Controller.RunBizQuery.GetBizQueryViewData,
            cache: false,
            dataType: "json",
            data: {
                schemaCode: $scope.SchemaCode,
                queryCode: $scope.QueryCode,
                random: new Date().getTime()
            },
            success: function (data, header, config, status) {
                if (data.Success == false && data.ExceptionCode == 1) {
                    return;
                }
                //显示格式
                $scope.DisplayFormats = data.Extend.DisplayFormats;
                //功能按钮
                $scope.BizQueryActions = data.Extend.ActionFilter.Data.Extend.BizQueryActions;
                //帅选条件
                $scope.FilterData = data.Extend.ActionFilter.Data.Extend.FilterData;
                //默认显示查询结果
                $scope.ListDefault = data.Extend.ActionFilter.Data.Extend.ListDefault;
                //列名
                var GirdColumns = data.Extend.ActionFilter.Data.Extend.GirdColumns;
                //添加选择按钮
                $scope.GirdColumns = [];
                $scope.GirdColumns.push({
                    display: "选择",
                    name: "ObjectID"
                })
                angular.forEach(GirdColumns, function (data, index, full) {
                    $scope.GirdColumns.push(data);
                })
                //功能按钮处理  ng-click
                angular.forEach($scope.BizQueryActions, function (data, index, full) {
                    if (data.ActionType == 0) {
                        var BizMethodName = data.BizMethodName;
                        var DisplayName = data.DisplayName;
                        var ifConfirm = data.Confirm == 1 ? true : false;
                        $scope.BizQueryActions[index].url = "InvokeMethod('" + BizMethodName + "','" + DisplayName + "'," + ifConfirm + ")"
                    }
                    else if (data.ActionType == 1) {
                        var BizSheetCode = data.BizSheetCode;
                        var ifWithID = data.WithID == 1 ? true : false;
                        var AfterSave = data.AfterSave;
                        $scope.BizQueryActions[index].url = "OpenSheet('" + BizSheetCode + "'," + ifWithID + "," + AfterSave + ")"
                    }
                    else if (data.ActionType == 2) {
                        var Url = data.Url;
                        var ifWithID = data.WithID == 1 ? true : false;
                        $scope.BizQueryActions[index].url = "OpenUrl('" + Url + "'," + ifWithID + ")";
                    }
                });
            },
            error: function (data, header, config, status) {
            }
        });
    };
    $scope.FinishedFunc = function () {
        //设置默认值
        SetDefaultSearchValue();
    };
    // 获取语言
    $rootScope.$on('$translateChangeEnd', function () {
        $scope.getLanguage();
        $state.go($state.$current.self.name, {}, { reload: true });
    });
    $scope.getLanguage = function () {
        $scope.LanJson = {
            search: $translate.instant("uidataTable.search"),
            sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
            sZeroRecords: $translate.instant("uidataTable.sZeroRecords_NoRecords"),
            sInfo: $translate.instant("uidataTable.sInfo"),
            sProcessing: $translate.instant("uidataTable.sProcessing")
        }
    };
    $scope.getLanguage();
    $scope.UserOptions = {
        Editable: true, Visiable: true, OrgUnitVisible: true
    };
    //设置查询条件默认值
    var SetDefaultSearchValue = function () {
        angular.forEach($scope.FilterData, function (data, index, full) {
            //文本，时间
            if (data.DisplayType == 0) {
                if (data.FilterType == 2 && (data.LogicType == 'DateTime' || data.LogicType == 'Int' || data.LogicType == 'Double')) {
                    //范围查询不做处理
                } else {
                    $("#" + data.PropertyCode + "").val(data.DefaultValue);
                }
            }
                //下拉框
            else if (data.DisplayType == 1) {
                $("#" + data.PropertyCode + "").val(data.DefaultValue);
            }
                //单选按钮
            else if (data.DisplayType == 2) {
                $("input[name='" + data.PropertyCode + "'][value='" + data.DefaultValue + "']").attr("checked", true);
            }
                //复选按钮
            else if (data.DisplayType == 3) {
                var a = data.DefaultValue.split(";");
                angular.forEach(a, function (value, index, full) {
                    if (value != null && value != "") {
                        $("[name='" + data.PropertyCode + "'][value='" + value + "']").attr("checked", true);
                    }
                })
            }
                //选人控件
            else if (data.DisplayType == 5) {
                var setValue = $interval(function () {
                    if ($("#" + data.PropertyCode + "").SheetUser) {
                        $("#" + data.PropertyCode + "").SheetUser().SetValue(data.DefaultValue);
                        $interval.cancel(setValue);
                    }
                }, 200)
            }
        })
    };
    //获取查询条件值
    var GetSearchConditions = function () {
        var condition = {};
        angular.forEach($scope.FilterData, function (data, index, full) {
            if (data.DisplayType == 0) {
                if (data.FilterType == 2 && (data.LogicType == 'DateTime' || data.LogicType == 'Int' || data.LogicType == 'Long' || data.LogicType == 'Double')) {
                    //范围查询
                    var start = $("#" + data.PropertyCode + "").val();
                    var end = $("#" + data.PropertyCode + "1").val();
                    condition[data.PropertyCode] = { start: start, end: end };
                } else {
                    condition[data.PropertyCode] = $("#" + data.PropertyCode + "").val();
                }
            } else if (data.DisplayType == 1) {
                condition[data.PropertyCode] = $("#" + data.PropertyCode + " option:selected").val();
            }
            else if (data.DisplayType == 2) {
                condition[data.PropertyCode] = $("input[name='" + data.PropertyCode + "']:checked").val();
            } else if (data.DisplayType == 3) {
                condition[data.PropertyCode] = "";
                $("input[name='" + data.PropertyCode + "']:checked").each(function (dex, element) {
                    condition[data.PropertyCode] = $(element).val() + ";" + condition[data.PropertyCode];
                });
            } else if (data.DisplayType == 5) {
                if ($("#" + data.PropertyCode).SheetUser) {
                    if ($scope.initComplete) {
                        condition[data.PropertyCode] = $("#" + data.PropertyCode).SheetUser().GetValue();
                    }
                }
            }
        })
        condition = JSON.stringify(condition);
        return condition;
    };
    $scope.getColumns = function () {
        var columns = [];
        angular.forEach($scope.GirdColumns, function (data, index, full) {
            if (data.name == "ObjectID") {
                columns.push({
                    "mData": "ObjectID",
                    "bSortable": false,
                    "mRender": function (data, type, full) {
                        return '<input type="radio" name="selectRow" ng-model="SelectBizObjectId" value="' + data + '" />';
                    }
                });
            } else {
                columns.push({
                    "mData": data.name,
                    "bSortable": true,
                    "mRender": function (data, type, full) {
                        if (typeof (data) == "string" && data.indexOf("Date") >= 0) {
                            data = datecalculation.changeDateFormat(data);
                        }
                        return data;
                    }
                });
            }
        })
        return columns;
    };
    $scope.BizViewOptions = function () {
        var options = {
            "bProcessing": true,
            "bServerSide": true,    // 是否读取服务器分页
            "paging": true,         // 是否启用分页
            "bPaginate": true,      // 分页按钮  
            "bLengthChange": false, // 每页显示多少数据
            "bFilter": false,        // 是否显示搜索栏  
            "searchDelay": 1000,    // 延迟搜索
            "iDisplayLength": 10,   // 每页显示行数  
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
            "sAjaxSource": "RunBizQuery/GetGridDataForPortal",
            "fnServerData": function (sSource, aDataSet, fnCallback) {
                $.ajax({
                    "dataType": 'json',
                    "type": 'POST',
                    "url": sSource,
                    "data": aDataSet,
                    "success": function (json) {
                        json = $scope.onBeforeShowData(json);
                        fnCallback(json);
                    }
                });
            },
            "fnServerParams": function (aoData) {  // 增加自定义查询条件
                $scope.Conditions = GetSearchConditions();
                aoData.push(
                    { "name": "schemaCode", "value": $scope.SchemaCode },
                    { "name": "queryCode", "value": $scope.QueryCode },
                    { "name": "filterStr", "value": $scope.Conditions },
                    { "name": "random", "value": new Date().getTime() }
                    );
            },
            "sAjaxDataProp": 'Rows',
            "sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
            "sPaginationType": "full_numbers",
            "aoColumns": $scope.getColumns(),
            "initComplete": function (settings, json) {
                $scope.initComplete = true;
                var filter = $(".searchBtn");
                filter.find("a").unbind("click.DT").bind("click.DT", function () {
                    $("#BizViewTable").dataTable().fnDraw();
                });
            },
            "fnRowCallback": function (nRow, aData, iDisplayIndex, iDisplayIndexFull) {
                $compile(nRow)($scope);
                $(nRow).bind("click", function () {
                    $scope.$apply(function () {
                        $scope.SelectBizObjectId = aData.ObjectID;
                    })
                })
            },
            "fnDrawCallback": function () {
                jqdatables.trcss();
            }
        }
        return options;
    };
    $scope.onBeforeShowData = function (json) {
        for (var i = 0; i < json.Rows.length; i++) {
            var row = json.Rows[i];
            for (var p in row) {
                //时间格式
                if (typeof (row[p]) == "string" && row[p].match(/^\/Date\(\-*[0-9]+\)\/$/)) {
                    var d = new Date();
                    var dateValue = parseFloat(row[p].match(/\-*[0-9]+/)[0]);
                    if (parseInt(dateValue) > 0) {
                        d.setTime(dateValue);
                        row[p] = d.getFullYear() + "/" + (d.getMonth() + 1) + "/" + d.getDate() + " " + d.getHours() + ":" + d.getMinutes() + ":" + d.getSeconds();
                    }
                    else {
                        row[p] = "";
                    }
                }
            }
            var newRow = {};
            for (var p in row) {
                //格式转换
                if (typeof ($scope.DisplayFormats) != "undefined" && $scope.DisplayFormats && $scope.DisplayFormats[p]) {
                    var val = $scope.DisplayFormats[p];
                    var matches = $scope.DisplayFormats[p].match(/{[A-z]+[A-z0-9]*}/g);
                    if (matches) {
                        $(matches).each(function () {
                            var _propertyName = this.match(/[A-z]+/)[0];
                            if (row[_propertyName]) {
                                val = val.replace(this, row[_propertyName]);
                            }
                            else {
                                val = val.replace(this, "");
                            }
                        });
                    }
                    newRow[p] = val;
                }
            }
            if (row.ObjectID) {
                CurrentRows[row.ObjectID] = row;
            }
            for (var p in newRow) {
                json.Rows[i][p] = newRow[p];
            }
        }
        return json
    };
    var EditBizObjectUrl = $.Controller.RunBizQuery.EditBizObject.replace("..", "");
    var Param_BizObjectID = "BizObjectID";
    var Param_SchemaCode = "SchemaCode";
    //功能按钮
    $scope.InvokeMethod = function (BizMethodName, DisplayName, IfConfirm) {
        var bizObjectId = GetSelectedBizObjectId();
        if (bizObjectId) {
            //调用方法业务对象(ID为bizObjectId)的方法(methodName)
            if (IfConfirm) {
                // 弹出模态框
                var modalInstance = $modal.open({
                    templateUrl: 'template/ProcessCenter/ConfirmModal.html',
                    size: "sm",
                    controller: function ($scope, $modalInstance) {
                        $scope.Title = $translate.instant("WarnOfNotMetCondition.Tips");
                        $scope.Message = "将执行方法[" + DisplayName + "]?";
                        $scope.Button_OK = true;
                        $scope.Button_OK_Text = $translate.instant("QueryTableColumn.Button_OK");
                        $scope.Button_Cancel = true;
                        $scope.Button_Cancel_Text = $translate.instant("QueryTableColumn.Button_Cancel");
                        $scope.ok = function () {
                            $modalInstance.close();  // 点击确定按钮
                        };
                        $scope.cancel = function () {
                            $modalInstance.dismiss('cancel'); // 退出
                        }
                    }
                });
                //弹窗点击确定的回调事件
                modalInstance.result.then(function () {
                    DoInvokeMethod(BizMethodName, DisplayName, bizObjectId);
                });
            }
            else {
                DoInvokeMethod(methodName, DisplayName, bizObjectId);
            }
        }
        else {
            //请选择
            $.notify({ message: "请选择一项", status: "danger" });
        }
    }
    $scope.OpenSheet = function (BizSheetCode, IfWithID, AfterSave) {
        if (IfWithID) {
            var bizObjectId = GetSelectedBizObjectId();
            if (bizObjectId) {
                //打开表单,参数包含BizObjectID
                window.open(PortalRoot + EditBizObjectUrl + "?" + Param_BizObjectID + "=" + bizObjectId + "&" + Param_SchemaCode + "=" + $scope.SchemaCode + "&SheetCode=" + BizSheetCode + "&Mode=Work&AfterSave=" + (isNaN(AfterSave) ? "" : AfterSave));
            }
            else {
                $.notify({ message: "请选择一项", status: "danger" });
                return;
            }
        }
        else {
            //打开表单,参数不包含BizObjectID,即可新建
            window.open(PortalRoot + EditBizObjectUrl + "?SheetCode=" + BizSheetCode + "&Mode=Originate&" + Param_SchemaCode + "=" + $scope.SchemaCode + "&AfterSave=" + (isNaN(AfterSave) ? "" : AfterSave));
        }
    }
    $scope.OpenUrl = function (url, IfWithID) {
        var bizObjectId = GetSelectedBizObjectId();
        if (bizObjectId) {
            url = url.replace(/{BizObjectID}/g, bizObjectId).replace(/{ObjectID}/g, bizObjectId);
            //参数: 业务对象ID
            if (CurrentRows[bizObjectId] && url.match(/{[_A-z0-9]+}/g)) {
                $(url.match(/{[_A-z0-9]+}/g)).each(function () {
                    var p = this.match(/[_A-z0-9]+/);
                    url = url.replace(this, CurrentRows[bizObjectId][p] || "");
                });
            }
        }
        else if (url.indexOf("{") > -1 && url.indexOf("}") > -1 && url.indexOf("{") < url.indexOf("}")) {
            //请选择
            $.notify({ message: "请选择一项", status: "danger" });
            return;
        }
        //else {
        window.open(url);
        // }
    }
    var GetSelectedBizObjectId = function () {
        if ($scope.SelectBizObjectId) {
            return $scope.SelectBizObjectId;
        } else {
            return;
        }
    }
    var DoInvokeMethod = function (methodName, displayName, bizObjectId) {
        var Param_BizObjectID = "BizObjectID";
        var Param_SchemaCode = "SchemaCode";
        var BizObjectHandlerUrl = $.Controller.RunBizQuery.InvokeMethod.replace("../", "");
        $.ajax({
            type: "post",
            url: BizObjectHandlerUrl,
            cache: false,
            async: false,
            dataType: "json",
            data: {
                bizSchemaCode: $stateParams.SchemaCode,//SchemaCode,
                bizObjectID: bizObjectId,
                methodName: methodName
            },
            success: function (result) {
                if (result == "PortalSessionOut") {
                    //显示或者打开登录对话框
                    $state.go("platform.login", { ParamUrl: "" });
                }
                if (result.Result) {
                    if (result.NeedRefresh) {
                        //方法执行成功后刷新页面
                        $state.go($state.$current.self.name, {}, { reload: true });
                    }
                    else {
                        $.notify({ message: (displayName || methodName) + "执行完成", status: "success" });
                    }
                }
                else {
                    var content = "[" + displayName + "]" + "调用失败:";
                    if (result.Errors) {
                        content += result.Errors[0];
                    }
                    $.notify({ message: content, status: "danger" });
                }
            },
            error: function (msg) {
                var content = "调用失败:";
                if (msg.status == "500") {
                    content += "服务器内部错误";
                }
                else {
                    content += msg.statusText;
                }
                $.notify({ message: content, status: "danger" });
            }
        });
    }
}]);