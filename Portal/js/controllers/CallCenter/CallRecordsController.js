app.controller('CallRecordsController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig", "$interval", "jq.datables",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig, $interval, jqdatables) {

        var activeCallType = 0;//0呼出，1呼入
        $("#callInSearchKey").hide();
        $(".callInTableHead").css("display", "none");
        //$("#callInTableHead").hide();

        var callerCode = localStorage.getItem("userCode");

        $scope.SearchKeyCallOut = {
            CalledName: "",//被叫人姓名
            CalledType: "",//角色类型
            CalledIdType: "",//证件类型
            CalledIdNumber: "",//证件号码
            ContractNo: "",//合同号
            CalledNumber: "",//被叫人电话
            CalledNumberType: "",//电话类型
            QuestionType: "",//咨询业务类型
            CallerName: "",//主叫人姓名
            CallerPosition: "",//主叫人职务
            CallOutStartTime: "",//开始时间
            CallOutEndTime: "",//结束时间
        };

        $scope.SearchKeyCallIn = {
            CalledName: "",//被叫人姓名
            CalledType: "",//角色类型
            CalledIdType: "",//证件类型
            CalledIdNumber: "",//证件号码
            ContractNo: "",//合同号
            CalledNumber: "",//被叫人电话
            CalledNumberType: "",//电话类型
            QuestionType: "",//咨询业务类型
            CallerName: "",//主叫人姓名
            CallerPosition: "",//主叫人职务
            CallInStartTime: "",//开始时间
            CallInEndTime: "",//结束时间
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
                "mData": "RowNumber", //序号
                "sClass": ""
            });
            columns.push({
                "mData": "CalledName", //被叫人姓名
                "sClass": ""
            });
            columns.push({
                "mData": "CalledType",//被角色类型
                "sClass": ""
            });
            columns.push({
                "mData": "CalledIdType",//证件类型
                "sClass": ""
            });
            columns.push({
                "mData": "CalledIdNumber",//证件号码
                "sClass": ""
            });
            columns.push({
                "mData": "ContractNo",//合同号
                "sClass": ""
            });
            columns.push({
                "mData": "CalledNumber",//被叫人电话
                "sClass": "",
                "mRender": function (data, type, full) {
                    return "<span>" + full.CalledNumber + "<a  ng-click=\"TelephoneBox('" + full.CalledName + "','" + full.CalledNumber + "','" + full.CalledType + "','" + full.CalledNumberType + "','" + full.CalledIdType + "','" + full.CalledIdNumber + "')\"  id=\"btn_AddBorrower\"><img src=\"WFRes/images/3.png\" width=\"20\" height=\"20\" /></a><span>";
                }
            });
            //columns.push({
            //    "mData": "CalledNumberType", //电话类型
            //    "sClass": ""
            //});
            columns.push({
                "mData": "", //外呼结果
                "sClass": "",
                "mRender": function (data, type, full) {
                    return "<span style=\"color:green\">接通</span>";
                }
            });
            columns.push({
                "mData": "QuestionType", //咨询业务
                "sClass": ""
            });
            columns.push({
                "mData": "CallerName",//主叫人姓名
                "sClass": ""
            });
            columns.push({
                "mData": "CallerPosition",//主叫人职务
                "sClass": ""
            });
            //columns.push({
            //    "mData": "Remark",//通话备注
            //    "sClass": ""
            //});
            columns.push({
                //CreatedTime
                "mData": "CreateTime",//录音生成时间
                "sClass": ""
            });
            columns.push({
                "mData": "CallDuration",//录音时长
                "sClass": "",
                "mRender": function (data, type, full) {
                    var time = $scope.formateTime(full.CallDuration);

                    return "<span>" + time.timeStr + "</span>";
                }
            });
            //columns.push({
            //    "mData": "ObjectId",//ID
            //    "sClass": ""
            //});

            columns.push({
                "mData": "ObjectId", //操作
                "sClass": "hide1024",
                "mRender": function (data, type, full) {
                    // ng-click
                    //console.log(full);
                    var qtype1ids = full.QTpye1Ids.join(',');
                    var qtype2ids = full.QTpye2Ids.join(',');
                    return "<span style=\"color:green\" ng-click=\"edit('" + full.ObjectId + "','" + full.UniqueId + "','" + full.CalledName + "','" + full.CalledType + "','" + full.CalledIdType + "','" + full.CalledIdNumber + "','" + full.ContractNo + "','" + full.CalledNumber + "','" + full.CalledNumberType + "','" + full.CallerName + "','" + full.CallerPosition + "','" + full.CreateTime + "','" + full.Remark + "','" + full.CallDuration + "','" + full.QuestionType + "','" + qtype1ids + "','" + qtype2ids + "')\">编辑</span>";
                }
            });
            columns.push({
                "mData": "RowNumber", //录音播放
                "mRender": function (data, type, full) {
                    //console.log("11");
                    //console.log(data);
                    //console.log(full);
                    //console.log("22");
                    return "<a target=\"_blank\" href=\"http://172.16.2.251/interface/api/?action=record_play&recording=/var/spool/asterisk/monitor/" + full.RecordFileName.toString().toUpperCase() + "\"><img src=\"/Portal/WFRes/image/recordplay.png\" width=\"18\" height=\"18\" /></a>";
                }

            });
            columns.push({
                "mData": "RowNumber", //录音下载
                "mRender": function (data, type, full) {
                    //console.log("11");
                    //console.log(data);
                    //console.log(full);
                    //console.log("22");
                    return "<a target=\"_blank\" href=\"http://172.16.2.251/admin/?m=interface&c=api&a=record_download&filename=/var/spool/asterisk/monitor/" + full.RecordFileName.toString().toUpperCase() + "\"><img src=\"/Portal/WFRes/image/download.png\" width=\"18\" height=\"18\" /></a>";
                }
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
                    "sZeroRecords": "<i class=\"icon-emoticon-smile\"></i>" ,//+ $scope.LanJson.sZeroRecords,
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
                "sAjaxSource": "CallCenter/QueryRecords",
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
                            if (json.Total == 0) {
                                alert("未查询到数据");
                            }
                            fnCallback(json);
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
                 
                    var CalledName = activeCallType == 0 ? $scope.SearchKeyCallOut.CalledName : $scope.SearchKeyCallIn.CalledName;
                    var CalledType = activeCallType == 0 ? $scope.SearchKeyCallOut.CalledType : $scope.SearchKeyCallIn.CalledType;
                    var CalledIdType = activeCallType == 0 ? $scope.SearchKeyCallOut.CalledIdType : $scope.SearchKeyCallIn.CalledIdType;
                    var CalledIdNumber = activeCallType == 0 ? $scope.SearchKeyCallOut.CalledIdNumber : $scope.SearchKeyCallIn.CalledIdNumber;
                    var ContractNo = activeCallType == 0 ? $scope.SearchKeyCallOut.ContractNo : $scope.SearchKeyCallIn.ContractNo;
                    var CalledNumber = activeCallType == 0 ? $scope.SearchKeyCallOut.CalledNumber : $scope.SearchKeyCallIn.CalledNumber;
                    var CalledNumberType = activeCallType == 0 ? $scope.SearchKeyCallOut.CalledNumberType : $scope.SearchKeyCallIn.CalledNumberType;
                    var QuestionType = activeCallType == 0 ? $scope.SearchKeyCallOut.QuestionType : $scope.SearchKeyCallIn.QuestionType;
                    var CallerName = activeCallType == 0 ? $scope.SearchKeyCallOut.CallerName : $scope.SearchKeyCallIn.CallerName;
                    var CallerPosition = activeCallType == 0 ? $scope.SearchKeyCallOut.CallerPosition : $scope.SearchKeyCallIn.CallerPosition;
                    var Stime = activeCallType == 0 ? $("#CallOutStartTime").val() : $("#CallInStartTime").val();
                    var Etime = activeCallType == 0 ? $("#CallOutEndTime").val() : $("#CallInEndTime").val();

                    aoData.push({ "name": "CalledName", "value": CalledName });//被叫人姓名
                    aoData.push({ "name": "CalledType", "value": CalledType });//角色类型
                    aoData.push({ "name": "CalledIdType", "value": CalledIdType });//证件类型
                    aoData.push({ "name": "CalledIdNumber", "value": CalledIdNumber });//证件号码
                    aoData.push({ "name": "ContractNo", "value": ContractNo });//合同号
                    aoData.push({ "name": "CalledNumber", "value": CalledNumber });//被叫人电话
                    aoData.push({ "name": "CalledNumberType", "value": CalledNumberType });//电话类型
                    aoData.push({ "name": "QuestionType", "value": QuestionType });//咨询业务类型
                    aoData.push({ "name": "CallerName", "value": CallerName });//主叫人姓名
                    aoData.push({ "name": "CallerPosition", "value": CallerPosition });//主叫人职务
                    aoData.push({ "name": "StartDate", "value": Stime });//开始时间
                    aoData.push({ "name": "EndDate", "value": Etime });//结束时间
                    aoData.push({ "name": "CallerCode", "value": callerCode });//结束时间
                    aoData.push({ "name": "CallType", "value": activeCallType });//结束时间
                    
                },
                "aoColumns": $scope.getColumns(), // 字段定义
                // 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
                "initComplete": function (settings, json) {
                    var filter = $(".searchContainer");
                    filter.find("button").unbind("click.DT").bind("click.DT", function () {
                        $("#tabCallQueryItem").dataTable().fnDraw();
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
                    $compile($("#tabCallQueryItem"))($scope);
                }
            }
            return options;
        }

        $scope.registerInterval = function () {
            $scope.interval = $interval(function () {
                $("#tabCallQueryItem").dataTable().fnDraw();
            }, 90 * 1000);
        }

        $scope.$on("$destroy", function () {
            $interval.cancel($scope.interval);
        })

        $scope.clearSearch = function () {
            $scope.SearchKey = {
                ProductName: "",
                CarType: ""
            };
        }
        $scope.changeTab = function (callType) {

            if (callType == "callOut") {
                if (activeCallType == 0) {
                    return;
                }
                activeCallType = 0;
                $("#callInSearchKey").hide();
                $(".callInTableHead").css("display", "none");
                //$("#callInTableHead").hide();
                $("#callOutSearchKey").show();
                $(".callOutTableHead").css("display", "block");
                //$("#callOutTableHead").show();
                $("#tabCallQueryItem").dataTable().fnDraw();
            }
            else {
                if (activeCallType == 1) {
                    return;
                }
                activeCallType = 1;
                $("#callInSearchKey").show();
                $(".callInTableHead").css("display", "block");
                //$("#callInTableHead").show();
                $("#callOutSearchKey").hide();
                $(".callOutTableHead").css("display", "none");
                //$("#callOutTableHead").hide();
                $("#tabCallQueryItem").dataTable().fnDraw();
            }

        }
        //录音记录 编辑
        $scope.edit = function (ObjectId,UniqueId, CalledName, CalledType, CalledIdType, CalledIdNumber, ContractNo, CalledNumber, CalledNumberType, CallerName, CallerPosition, CreateTime, Remark, CallDuration, QuestionType, QType1Ids, QType2Ids) {
            //判断是呼出0 还是接入1
            if (activeCallType == 0) {
                $("#CallOutQuestionType1").val(QType1Ids.split(",")).trigger("change");
                $("#CallOutQuestionType2").val(QType2Ids.split(",")).trigger("change");

                $('#myModal1').modal('show');
                $("#CallOutObjectId").val(ObjectId);
                $("#CallOutUniqueId").val(UniqueId);
                $("#CallOutCalledName").val(CalledName);//被叫人姓名
                $("#CallOutCalledType").val(CalledType).trigger("change");//角色类型
                $("#CallOutCalledIdType").val(CalledIdType).trigger("change");//证件类型
                $("#CallOutCalledIdNumber").val(CalledIdNumber);//证件号码
                $("#CallOutContractNo").val(ContractNo);//合同号
                $("#CallOutCalledNumber").val(CalledNumber);//被叫人电话
                $("#CallOutCalledNumberType").val(CalledNumberType).trigger("change");//电话类型
                $("#CallOutCallerName").val(CallerName);//主叫人姓名
                $("#CallOutCallerPosition").val(CallerPosition);//主叫人职务
                $("#OutStartTime").text(CreateTime);//录音生成时间
                $("#CallOutRemark").val(Remark);//通话备注
                //$("#CallOutCallDuration").val(CallDuration);//录音时长

                var time = $scope.formateTime(CallDuration);
                $("#CallOutHours").val(time.hour);
                $("#CallOutMinute").val(time.minute);
                $("#CallOutSecond").val(time.second);

            } else if (activeCallType == 1) {
                $("#CallInQuestionType1").val(QType1Ids.split(",")).trigger("change");
                $("#CallInQuestionType2").val(QType2Ids.split(",")).trigger("change");
                $('#myModal2').modal('show');
                $("#CallInObjectId").val(ObjectId);
                $("#CallInUniqueId").val(UniqueId);
                $("#CallInCalledName").val(CalledName);//被叫人姓名
                $("#CallInCalledType").val(CalledType).trigger("change");//角色类型
                $("#CallInCalledIdType").val(CalledIdType).trigger("change");//证件类型
                $("#CallInCalledIdNumber").val(CalledIdNumber);//证件号码
                $("#CallInContractNo").val(ContractNo);//合同号
                $("#CallInCalledNumber").val(CalledNumber);//被叫人电话
                $("#CallInCalledNumberType").val(CalledNumberType).trigger("change");//电话类型
                //$("#CallInQuestionType").val(QuestionType);//业务类型
                $("#CallInCallerName").val(CallerName);//主叫人姓名
                $("#CallInCallerPosition").val(CallerPosition);//主叫人职务
                $("#InStartTime").text(CreateTime);//录音生成时间
                $("#CallInRemark").val(Remark);//通话备注
                //$("#CallInCallDuration").val(CallDuration);//录音时长

                var time = $scope.formateTime(CallDuration);
                $("#CallInCallHours").val(time.hour);
                $("#CallInMinute").val(time.minute);
                $("#CallInSecond").val(time.second);
            }

        }
        //编辑外呼
        $scope.saveCallOut = function () {
            var questionTypeArr1 = $("#CallOutQuestionType1").select2('data');
            var questionTypeArr2 = $("#CallOutQuestionType2").select2('data');

            if (questionTypeArr2.length <= 0) {
                layer.msg("请选择咨询类型", { icon: 2 });
                return;
            }

            var questionTypeData = [];

            questionTypeArr2.forEach(function (item, number) {
                var id = item.id;
                questiontype.type_1.forEach(function (item2, number2) {
                    item2.children.forEach(function (item3, number3) {
                        if (item3.id + "" == id) {
                            questionTypeData.push({ QType1Id: item3.parentid + "", QType1Name: item2.text, QType2Id: id, QType2Name: item3.text });
                        }
                    });
                });
            });


            var ObjectId = $("#CallOutObjectId").val();
            var UniqueId = $("#CallOutUniqueId").val();
            var CallOutCalledName = $("#CallOutCalledName").val();//被叫人姓名
            var CallOutCalledType = $("#CallOutCalledType").val();//角色类型
            var CallOutCalledIdType = $("#CallOutCalledIdType").val();//证件类型
            var CallOutCalledIdNumber = $("#CallOutCalledIdNumber").val();//证件号码
            var CallOutContractNo = $("#CallOutContractNo").val();//合同号
            var CallOutCalledNumber = $("#CallOutCalledNumber").val();//被叫人电话
            var CallOutCalledNumberType = $("#CallOutCalledNumberType").val();//电话类型
            var CallOutCallerName = $("#CallOutCallerName").val();//主叫人姓名
            var CallOutCallerPosition = $("#CallOutCallerPosition").val();//主叫人职务
            var CallOutStartTime = $("#OutStartTime").val();//录音生成时间
            var CallOutRemark = $("#CallOutRemark").val();//通话备注

            //var CallOutCallDuration = $("#CallOutCallDuration").val();//录音时长
            var hours = $("#CallOutHours").val();
            var minutes = $("#CallOutMinute").val();
            var seconds = $("#CallOutSecond").val();
            if (hours == "" || hours == null) {
                hours = 0;
            }
            if (minutes == "" || minutes == null) {
                minutes = 0;
            }
            if (seconds == "" || seconds == null) {
                seconds = 0;
            }
            var CallOutCallDuration = parseInt(hours) * 60 * 60 + parseInt(minutes) * 60 + parseInt(seconds);

            var CallOutrequest = {
                ObjectId: ObjectId,
                UniqueId: UniqueId,
                CalledName: CallOutCalledName,//被叫人姓名
                CalledType: CallOutCalledType,//角色类型
                CalledIdType: CallOutCalledIdType,//证件类型
                CalledIdNumber: CallOutCalledIdNumber,//证件号码
                ContractNo: CallOutContractNo,//合同号
                CalledNumber: CallOutCalledNumber,//被叫人电话
                CalledNumberType: CallOutCalledNumberType,//电话类型
                CallerName: CallOutCallerName, //主叫人姓名
                CallerPosition: CallOutCallerPosition,//主叫人职务
                CreateTime: CallOutStartTime,//录音生成时间
                Remark: CallOutRemark,//备注
                CallDuration: CallOutCallDuration,//录音时长
                QuestionTypeStr: JSON.stringify(questionTypeData), //业务类型

            };
            $.ajax({
                url: "/Portal/CallCenter/UpdateRecord",
                type: "POST",
                async: true,
                data: CallOutrequest,
                dataType: "json",
                success: function (result) {
                    if (result.code > 0) {
                        alert("编辑成功");
                        $("#myModal1").find('button[class="close"][data-dismiss="modal"]').click();
                        $("#tabCallQueryItem").dataTable().fnDraw();
                        //刷新
                    } else {
                        alert("编辑失败");
                    }
                },
                error: function (xhr) {
                    alert("出错");

                },
            });
        }


        //编辑接入
        $scope.saveCallIn =function () {
            var questionTypeArr1 = $("#CallInQuestionType1").select2('data');
            var questionTypeArr2 = $("#CallInQuestionType2").select2('data');

            if (questionTypeArr2.length <= 0) {
                layer.msg("请选择咨询类型", { icon: 2 });
                return;
            }

            var questionTypeData = [];

            questionTypeArr2.forEach(function (item, number) {
                var id = item.id;
                questiontype.type_1.forEach(function (item2, number2) {
                    item2.children.forEach(function (item3, number3) {
                        if (item3.id + "" == id) {
                            questionTypeData.push({ QType1Id: item3.parentid + "", QType1Name: item2.text, QType2Id: id, QType2Name: item3.text });
                        }
                    });
                });
            });

            var ObjectId = $("#CallInObjectId").val();
            var UniqueId = $("#CallInUniqueId").val();
            var CallInCalledName = $("#CallInCalledName").val();//主叫人姓名
            var CallInCalledType = $("#CallInCalledType").val();//角色类型
            var CallInCalledIdType = $("#CallInCalledIdType").val();//证件类型
            var CallInCalledIdNumber = $("#CallInCalledIdNumber").val();//证件号码
            var CallInContractNo = $("#CallInContractNo").val();//合同号
            var CallInCalledNumber = $("#CallInCalledNumber").val();//主叫人电话
            var CallInCalledNumberType = $("#CallInCalledNumberType").val();//电话类型
            //var CallInQuestionType = $("#CallInQuestionType").val()//业务类型
            var CallInCallerName = $("#CallInCallerName").val();//被叫人姓名
            var CallInCallerPosition = $("#CallInCallerPosition").val();//主叫人职务
            var InStartTime = $("#InStartTime").val();//录音生成时间
            var CallInRemark = $("#CallInRemark").val();//通话备注



            //var CallInCallDuration = $("#CallInCallDuration").val();//录音时长
            var hours = $("#CallInCallHours").val();
            var minutes = $("#CallInMinute").val();
            var seconds = $("#CallInSecond").val();
            if (hours == "" || hours == null) {
                hours = 0;
            }
            if (minutes == "" || minutes == null) {
                minutes = 0;
            }
            if (seconds == "" || seconds == null) {
                seconds = 0;
            }
            var CallInCallDuration = parseInt(hours) * 60 * 60 + parseInt(minutes) * 60 + parseInt(seconds);


            var CallInrequest = {
                ObjectId: ObjectId,
                UniqueId: UniqueId,
                CalledName: CallInCalledName,//主叫人姓名
                CalledType: CallInCalledType,//角色类型
                CalledIdType: CallInCalledIdType, //证件类型
                CalledIdNumber: CallInCalledIdNumber,//证件号码
                ContractNo: CallInContractNo, //合同号
                CalledNumber: CallInCalledNumber,//主叫人电话
                CalledNumberType: CallInCalledNumberType, //电话类型
                QuestionTypeStr: JSON.stringify(questionTypeData), //业务类型
                CallerName: CallInCallerName,//被叫人姓名
                CallerPosition: CallInCallerPosition,//主叫人职务
                CreateTime: InStartTime,//录音生成时间
                Remark: CallInRemark,//通话备注
                CallDuration: CallInCallDuration, //录音时长

            };
            $.ajax({
                url: "/Portal/CallCenter/UpdateRecord",
                type: "POST",
                async: true,
                data: CallInrequest,
                dataType: "json",
                success: function (result) {
                    if (result.code > 0) {
                        alert("编辑成功");
                        //$("#myModal2").modal('hide');
                        $("#myModal2").find('button[class="close"][data-dismiss="modal"]').click();
                        $("#tabCallQueryItem").dataTable().fnDraw();
                        //刷新
                    } else {
                        alert("编辑失败");
                    }
                },
                error: function (xhr) {
                    alert("出错");

                },
            });
        }


        $scope.formateTime = function (value) {
            var secondTime = parseInt(value);// 秒
            var minuteTime = 0;// 分
            var hourTime = 0;// 小时
            if (secondTime > 60) {//如果秒数大于60，将秒数转换成整数
                //获取分钟，除以60取整数，得到整数分钟
                minuteTime = parseInt(secondTime / 60);
                //获取秒数，秒数取佘，得到整数秒数
                secondTime = parseInt(secondTime % 60);
                //如果分钟大于60，将分钟转换成小时
                if (minuteTime > 60) {
                    //获取小时，获取分钟除以60，得到整数小时
                    hourTime = parseInt(minuteTime / 60);
                    //获取小时后取佘的分，获取分钟除以60取佘的分
                    minuteTime = parseInt(minuteTime % 60);
                }
            }

            return { hour: hourTime, minute: minuteTime, second: secondTime, timeStr: hourTime + "时" + minuteTime + "分" + secondTime + "秒" };

        }


        $scope.TelephoneBox = function (calledname, phoneNumber, calledtype, phoneNumberType, idtype, idnumber) {
            $("#dialCalledName").val(calledname);
            $("#dialCalledType").val(calledtype);
            $("#dialCalledNumber").val(phoneNumber);
            $("#dialMobileType").val(phoneNumberType);
            $("#dialIdType").val(idtype);
            $("#dialIdNumber").val(idnumber);
            //座机 直接拨打号码
            if (phoneNumberType == "座机") {
                $('#myModal3').modal('hide');//隐藏
                $('#myModal4').modal('show');//显示
                $scope.dialOut();
            } else {
                $('#myModal3').modal('show');//显示
            }
        }

        //呼出
        $scope.dialOut = function () {
            var phoneposition = $('input[name=phoneposition]:checked').val();

            var mobileType = $("#dialMobileType").val();

            if (mobileType == "手机 Mobile Phone") {
                if (phoneposition == undefined || phoneposition == null || phoneposition == "") {
                    alert("请选选择本地/外地号码");
                    return;
                }
            }

            //模态框显示隐藏
            $('#myModal3').modal('hide');
            $('#myModal4').modal('show');
            //extension  主叫号码  extensionDst 被叫号码
            var extension = localStorage.getItem("phoneExtension");
            var extensionDst = $("#dialCalledNumber").val();


            //电话类型
            var mobileType = checkPhone(extensionDst) ? "手机 Mobile Phone" : "座机";

            if (phoneposition == "otherplace") {
                extensionDst = "0" + extensionDst;
            }
            if (extensionDst.startsWith("021-")) {
                extensionDst = extensionDst.substring(4);
                console.log(extensionDst);
            } else if (extensionDst.indexOf("-")) {
                extensionDst = extensionDst.replace("-", "");
                console.log(extensionDst);

            }

            //当前页面获取  主借人姓名  
            var mainPersonName = "";
            //当前页面获取  申请编号
            var applicationNumber = "";

            //呼叫人姓名
            var calledName = $("#dialCalledName").val();
            //呼叫人角色
            var calledType = $("#dialCalledType").val();
            //证件类型
            var idType = $("#dialIdType").val();
            //证件号码
            var idNumber = $("#dialIdNumber").val();
            //合同号
            var contractNo = $("#dialContractNo").val();

            var callerPosition = localStorage.getItem("userPosition");

            $.ajax({
                url: "https://172.16.2.251/admin/?m=interface&c=api&a=dial&extension=" + extension + "&extensionDst=" + extensionDst,
                type: "GET",
                async: false,
                dataType: "jsonp",
                success: function (result) {
                    if (result.result == 1) {
                        //var callPopOutInfo = { CallType: 0, MainApplicantName: mainPersonName, ApplicationNumber: applicationNumber, CalledName: calledName, CalledType: calledType, CalledNumber: extensionDst, CalledIdType: idType, CalledIdNumber: idNumber, CalledNumberType: mobileType, ContractNo: contractNo, CallerPosition: callerPosition, CallerName: $.MvcSheetUI.SheetInfo.UserName, PhonePosition: phoneposition };
                        //localStorage.setItem("callPopOutInfo", JSON.stringify(callPopOutInfo));
                    }
                },
                error: function (xhr) {
                    alert("电话呼出出错");
                },
            });
        }


        //电话挂断
        $scope.handUp = function () {
            var extension = localStorage.getItem("phoneExtension");
            $.ajax({
                url: "https://172.16.2.251/admin/?m=interface&c=api&a=hangup&extension=" + extension,
                type: "GET",
                async: false,
                dataType: "jsonp",
                success: function (result) {
                    $('#myModal3').modal('hide');
                    $('#myModal4').modal('hide');
                    if (result.result == 1) {
                        //挂断电话 隐藏模态框
                    }
                },
                error: function (xhr) {
                    alert("电话挂断出错");
                    $('#myModal4').modal('hide');
                },
            });
        }

        $scope.checkPhone = function (phone) {
            if (!(/^1[3456789]\d{9}$/.test(phone))) {
                //alert("手机号码有误，请重填");
                return false;
            }
            return true;
        }
        
    }]);