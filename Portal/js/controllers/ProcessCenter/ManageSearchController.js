app.controller('ManageSearchController', ['$scope', "$rootScope", "$translate", "$compile", "$http", "$timeout", "$state", "$interval", "$filter", "$modal", "ControllerConfig", "datecalculation", "jq.datables",
    function ($scope, $rootScope, $translate, $compile, $http, $timeout, $state, $interval, $filter, $modal, ControllerConfig, datecalculation, jqdatables) {
        $scope.$on('$viewContentLoaded', function (event) {
            $scope.init();
        });

        $scope.ck_Distinct = false;

        $scope.init = function () {
            $scope.name = $translate.instant("WorkItemController.FinishedWorkitem");
            $scope.StartTime = datecalculation.redDays(new Date(), 30);
            $scope.EndTime = datecalculation.addDays(new Date(), 30);
        }

        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });
        $scope.getLanguage = function () {
            $scope.LanJson = {
                search: $translate.instant("uidataTable.search"),
                ProcessName: $translate.instant("QueryTableColumn.ProcessName"),
                WorkFlow: $translate.instant("QueryTableColumn.WorkFlow"),
                StartTime: $translate.instant("QueryTableColumn.StartTime"),
                EndTime: $translate.instant("QueryTableColumn.EndTime"),
                sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
                sZeroRecords: $translate.instant("uidataTable.sZeroRecords"),
                sInfo: $translate.instant("uidataTable.sInfo"),
                sProcessing: $translate.instant("uidataTable.sProcessing")
            }
        }
        $scope.getLanguage();

        //初始化流程模板
        $scope.WorkflowOptions = {
            Editable: true,
            Visiable: true,
            Mode: "WorkflowTemplate",
            //AllowSearch: false,
            IsMultiple: false,
            OnChange: "",
            PlaceHolder: $scope.LanJson.WorkFlow
        }

        $scope.getColumns = function () {
            var columns = [];
            //          columns.push({
            //              "mData": "InstanceName",
            //              "mRender": function (data, type, full) {
            //                  return "<a ui-toggle-class='show' target='.app-aside-right' targeturl='WorkItemSheets.html?WorkItemID=" + full.ObjectID + "'>" + data + "</a>";
            //              }
            //          });
            columns.push({
                "mData": "AgencyID",
                "mRender": function (data, type, full) {
                    return "<input type=\"checkbox\" class=\"AgencyItem\" ng-checked=\"checkAll\" data-id=\"" + data + "\"/>";
                }
            });
            columns.push({
                "mData": "WorkflowName",
                "mRender": function (data, type, full) {
                    if (data == "") data = "所有的";
                    return "<span>" + data + "</span>";
                }
            });
            columns.push({ "mData": "WasAgentName", "sClass": "center" });
            columns.push({ "mData": "StartTime" });
            columns.push({ "mData": "EndTime" });
            //        columns.push({
            //          "mData": "AgencyID",
            //          "mRender": function (data, type, full) {
            //              return "<a ng-click=\"btn_addAgents('" + data + "')\"><span data-id=\"" + data + "\" translate=\"QueryTableColumn.Edit\"></span></a>";
            //          }
            //      });
            columns.push({ "mData": "OriginatorOUName", "sClass": "center hide1024" });
            return columns;
        }

        $scope.options_ManageRelatedparty = {
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
            //          "sAjaxSource": "/Portal/WorkItemHandler/GetFinishWorkItemsDistinct",// wangxg 调用 api的接口，为了xss过滤
            //"sAjaxSource": ControllerConfig.WorkItem.GetFinishWorkItems+"Distinct",//新的api方法，去重，add by chenghs 2018-02-28
            "sAjaxSource": ControllerConfig.Agents.GeyAgencyTable,
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
                    { "name": "startTime", "value": $filter("date")($scope.StartTime, "yyyy-MM-dd") },
                    { "name": "endTime", "value": $filter("date")($scope.EndTime, "yyyy-MM-dd") },
                    { "name": "workflowCode", "value": $scope.WorkflowCode },
                    { "name": "instanceName", "value": $scope.InstanceName },
                    { "name": "distinct", "value": $scope.ck_Distinct }//增加查询条件，是否去重：add by chenghs 2018-0-28
                );
            },
            "aoColumns": $scope.getColumns(), // 字段定义
            // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
            "initComplete": function (settings, json) {
                var filter = $(".searchContainer");
                filter.find("button").unbind("click.DT").bind("click.DT", function () {
                    $scope.WorkflowCode = $("#sheetWorkflow").SheetUIManager().GetValue();
                    $("#tabManageRelatedparty").dataTable().fnDraw();
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

        $scope.btn_addRelated = function (data) {
            if (data == undefined) AgencyID = "";
            else AgencyID = data;

            $http({
                url: ControllerConfig.Agents.GetAgency,
                params: {
                    agentID: AgencyID,
                    random: new Date().getTime()
                }
            })
                .success(function (result, header, config, status) {
                    var Agency = result.Rows[0];
                    // 弹出模态框
                    var modalInstance = $modal.open({
                        templateUrl: 'EditRelated.html',    // 指向上面创建的视图
                        controller: 'EditRelatedController',// 初始化模态范围
                        size: "md",
                        resolve: {
                            params: function () {
                                return {
                                    user: $scope.user,
                                    Agency: Agency,
                                    AgencyID: AgencyID
                                };
                            },
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                    'WFRes/assets/stylesheets/sheet.css',
                                    'WFRes/_Scripts/jquery/jquery.lang.js',
                                    'WFRes/_Scripts/ligerUI/ligerui.all.js',
                                    'WFRes/_Scripts/MvcSheet/SheetControls.js',
                                    'WFRes/_Scripts/MvcSheet/MvcSheetUI.js'
                                ]).then(function () {
                                    return $ocLazyLoad.load([
                                        'WFRes/_Scripts/MvcSheet/Controls/SheetWorkflow.js',
                                        'WFRes/_Scripts/MvcSheet/Controls/SheetUser.js'
                                    ])
                                });
                            }]
                        }
                    });
                })
                .error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
        }

    }]);

app.controller("EditRelatedController", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params",
    function ($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params) {

        $scope.getLanguage = function () {
            $scope.LanJson = {
                StartTime: $translate.instant("QueryTableColumn.StartTime"),
                EndTime: $translate.instant("QueryTableColumn.EndTime"),

                InvalidAgency: $translate.instant("WarnOfNotMetCondition.InvalidAgency"),
                NoSelectWorkItem: $translate.instant("WarnOfNotMetCondition.NoSelectWorkItem"),
                NoSelectWasAgent: $translate.instant("WarnOfNotMetCondition.NoSelectWasAgent"),
                NoSelectWorkflows: $translate.instant("WarnOfNotMetCondition.NoSelectWorkflows"),
                InvalidOfTime: $translate.instant("WarnOfNotMetCondition.InvalidOfTime"),
                NoSelectOriginatorRange: $translate.instant("WarnOfNotMetCondition.NoSelectOriginatorRange"),
            }
        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        //控件初始化参数
        $scope.EtartTimeOption = {
            dateFmt: 'yyyy-MM-dd', realDateFmt: "yyyy-MM-dd", minDate: '2012-1-1', maxDate: '2099-12-31',
            onpicked: function (e) {
                $scope.StartTime = e.el.value;
            }
        }
        $scope.EndTimeOption = {
            dateFmt: 'yyyy-MM-dd',
            realDateFmt: "yyyy-MM-dd", minDate: '2012-1-1', maxDate: '2099-12-31',
            onpicked: function (e) {
                $scope.EndTime = e.el.value;
            }
        }
        $scope.WorkflowOptions = {
            Editable: true, Visiable: true, Mode: "WorkflowTemplate", IsMultiple: true, PlaceHolder: $scope.LanJson.WorkFlow
        }
        $scope.WasAgentOptions = {
            Editable: true, Visiable: true, PlaceHolder: $scope.LanJson.Originator,
        }
        $scope.OriginatorRangeOptions = {
            Editable: true, Visiable: true, OrgUnitVisible: true, PlaceHolder: $scope.LanJson.Originator,
        }

        $scope.init = function () {
            $scope.user = params.user;
            $scope.IsAllWorkflow = "true";
            //编辑初始化
            if (params.AgencyID != "") {
                $scope.IsEdit = true;
                var Agency = params.Agency;
                if (Agency.WorkflowCode == "") {
                    $scope.IsAllWorkflow = "true";
                }
                else {
                    $scope.IsAllWorkflow = "false";
                    $scope.WorkflowOptions.V = Agency.WorkflowCode;
                    $scope.WorkflowCodes = Agency.WorkflowCode;
                }
                $scope.StartTime = Agency.StartTime;
                $scope.EndTime = Agency.EndTime;

                $scope.WorkflowOptions.Editable = false;
                $scope.WasAgentOptions.V = Agency.WasAgentID;
                $scope.OriginatorRangeOptions.V = Agency.OriginatorRange

            }
        }
        $scope.init();

        $scope.ok = function () {
            // TODO：如果是当前用户有权限编辑，那么保存至数据库
            //          $scope.WasAgent = $("#WasAgent").SheetUIManager().GetValue();
            //          $scope.WorkflowCodes = $("#WorkflowCodes").SheetUIManager().GetValue();
            //          $scope.OriginatorRange = $("#OriginatorRange").SheetUIManager().GetValue();
            //          if ($scope.WasAgent == "") {
            //              $scope.FailMessage = $scope.LanJson.NoSelectWasAgent;
            //              var ctrl = angular.element(document.querySelector("#EditError"));
            //              notify.showMessage(ctrl);
            //              return;
            //          }
            //
            //          if ($scope.StartTime == null || $scope.EndTime == null) {
            //              $scope.FailMessage = $scope.LanJson.InvalidOfTime;
            //              var ctrl = angular.element(document.querySelector("#EditError"));
            //              notify.showMessage(ctrl);
            //              return;
            //          }
            //          if (!datecalculation.isOrderBy($scope.StartTime, $scope.EndTime)) {
            //              $scope.FailMessage = $scope.LanJson.InvalidOfTime;
            //              var ctrl = angular.element(document.querySelector("#EditError"));
            //              notify.showMessage(ctrl);
            //              return;
            //          }
            //          if ($scope.IsAllWorkflow == "false" && $scope.WorkflowCodes.length <= 0) {
            //              $scope.FailMessage = $scope.LanJson.NoSelectWorkflows;
            //              var ctrl = angular.element(document.querySelector("#EditError"));
            //              notify.showMessage(ctrl);
            //              return;
            //          }
            //          if ($scope.OriginatorRange.length <= 0) {
            //              $scope.FailMessage = $scope.LanJson.NoSelectOriginatorRange;
            //              var ctrl = angular.element(document.querySelector("#EditError"));
            //              notify.showMessage(ctrl);
            //              return;
            //          }
            //          var WorkflowCodes = $scope.WorkflowCodes;
            //          $http({
            //              url: ControllerConfig.Agents.AddAgency,
            //              params: {
            //                  AgencyID: params.AgencyID,
            //                  IsAllWorkflow: $scope.IsAllWorkflow,
            //                  WorkflowCodes: $scope.WorkflowCodes,
            //                  StartTime: $scope.StartTime,
            //                  EndTime: $scope.EndTime,
            //                  OriginatorRange: $scope.OriginatorRange,
            //                  WasAgent: $scope.WasAgent,
            //              }
            //          })
            //              .success(function (result, header, config, status) {
            //                  if (result.Success == true) {
            //                      $modalInstance.close();
            //                      $state.go($state.$current.self.name, {}, { reload: true });
            //                  }
            //                  else {
            //                      $scope.FailMessage = $scope.LanJson.InvalidAgency;
            //                      var ctrl = angular.element(document.querySelector("#EditError"));
            //                      notify.showMessage(ctrl);
            //                      return;
            //                  }
            //              })
            //              .error(function (data, header, config, status) {// 19.7 
            //                  showAgErr(data, header);
            //              });

            $scope.relatedTips = true


        };

        $scope.cancel = function () {
            $modalInstance.dismiss('cancel'); // 退出
        }
    }])