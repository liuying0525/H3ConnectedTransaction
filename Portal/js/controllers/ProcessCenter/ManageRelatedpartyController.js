app.controller('ManageRelatedpartyController', ['$scope', "$rootScope", "$translate", "$compile", "$http", "$timeout", "$state", "$interval", "$filter", "$modal", "ControllerConfig", "datecalculation", "jq.datables",
	function($scope, $rootScope, $translate, $compile, $http, $timeout, $state, $interval, $filter, $modal, ControllerConfig, datecalculation, jqdatables) {
		$scope.$on('$viewContentLoaded', function(event) {
			$scope.init();
		});

		$scope.ck_Distinct = false;

		$scope.init = function() {
			$scope.name = $translate.instant("WorkItemController.FinishedWorkitem");
			$scope.StartTime = datecalculation.redDays(new Date(), 30);
			$scope.EndTime = datecalculation.addDays(new Date(), 30);
		}

		// 获取语言
		$rootScope.$on('$translateChangeEnd', function() {
			$scope.getLanguage();
			$state.go($state.$current.self.name, {}, {
				reload: true
			});
		});
		$scope.getLanguage = function() {
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

		$scope.getColumns = function() {
			var columns = [];
			columns.push({
				"mData": "id",
				"mRender": function(data, type, full) {
					return "<input type=\"checkbox\" class=\"AgencyItem\" ng-checked=\"checkAll\" data-id=\"" + data + "\"/>";
				},
				"sClass": "text-center"
			});
			columns.push({
				"mData": "identityNo",
				"mRender": function(data, type, full) {
					if(data == "") data = "所有的";
					return "<span>" + data + "</span>";
				},
				"sClass": "text-center"
			});
			columns.push({
				"mData": "name",
				"sClass": "text-center"
			});
			//          columns.push({ "mData": "status" });
			columns.push({
				"mData": "transType",
				"mRender": function(data, type, full) {
					if(data == 0) data = "授信";
					if(data == 1) data = "交易";
					if(data == 2) data = "服务";
					if(data == 3) data = "股权";
					if(data == 4) data = "其它";
					return "<span>" + data + "类</span>";
				},
				"sClass": "text-center"
			});
			columns.push({
				"mData": "transMoney",
				"sClass": "text-center"
			});
			columns.push({
				"mData": "grade",
				"mRender": function(data, type, full) {
					if(data == 0) data = "非关联交易";
					if(data == 1) data = "一般关联交易";
					if(data == 2) data = "重大关联交易";
					return "<span>" + data + "</span>";
				},
				"sClass": "text-center"
			});
			return columns;
		}
		$scope.datamodel = {
			"pageIndex": 0,
			"pageSize": 10,
			"orderColumn": "CREATETIME",
			"orderType": 0,
			"searchs": []
		}
		$scope.options_ManageRelatedparty = {
			"bProcessing": true,
			"bServerSide": true, // 是否读取服务器分页
			"paging": true, // 是否启用分页
			"bPaginate": true, // 分页按钮  
			"bLengthChange": false, // 每页显示多少数据
			"bFilter": false, // 是否显示搜索栏  
			"searchDelay": 1000, // 延迟搜索
			"aoColumnDefs": [{
				"bSortable": false,
				"aTargets": [0]
			}], //对第0列的不进行排序
			"aaSorting": [
				[1, "desc"]
			],
			"iDisplayLength": 10, // 每页显示行数  
			"bSort": true, // 排序  
			"singleSelect": true,
			"bInfo": true, // Showing 1 to 10 of 23 entries 总记录数没也显示多少等信息  

			"pagingType": "full_numbers", // 设置分页样式，这个是默认的值
			"language": { // 语言设置
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
			"sAjaxSource": "/portal/RelatedTrans/GetTransData",
			"fnServerData": function(sSource, aDataSet, fnCallback) {
				localStorage.setItem('sEcho', parseInt(aDataSet.sEcho));
				$.ajax({
					"dataType": 'json',
					"type": 'POST',
					"url": sSource,
					"data": aDataSet,
					//					"data": JSON.stringify($scope.datamodel),
					"success": function(json) {
						if(json.ExceptionCode == 1 && json.Success == false) {
							json.Rows = [];
							json.sEcho = 1;
							json.Total = 0;
							json.iTotalDisplayRecords = 0;
							json.iTotalRecords = 0;
							$state.go("platform.login");
						}
						var datajson = {}
						datajson.Rows = angular.copy(json.data);
						datajson.Total = parseInt(json.page.totalRecord);
						datajson.iTotalRecords = parseInt(json.page.totalRecord);
						datajson.sEcho = parseInt(localStorage.getItem('sEcho'));
						datajson.iTotalDisplayRecords = parseInt(json.page.totalRecord);
						datajson.ExtendProperty = null;
						datajson.ObjectID = null;

						fnCallback(datajson);
					},
					"error": function(msg, textStatus, errorThrown) {
						if(msg.status == 401) {
							window.location = "/Portal/index.html#/platform/login";
						} else { //wangxg 19.7
							showJqErr(msg);
						}
					}
				});
			},
			"sAjaxDataProp": 'Rows',
			"sDom": "<'row'<'col-sm-6'l><'col-sm-6'f>r>t<'row'<'col-sm-6'i><'col-sm-6'p>>",
			"sPaginationType": "full_numbers",
			"fnServerParams": function(aoData) { // 增加自定义查询条件
				$scope.StartTime = $("#StartTime").val();
				$scope.EndTime = $("#EndTime").val();
				var ordercolumnindex = aoData.filter(item => item.name == 'iSortCol_0')[0].value;

				var orderMatchindex = aoData.filter(item => item.name == 'mDataProp_' + ordercolumnindex)[0].value

				var shortType = 0;

				aoData.filter(item => item.name == 'sSortDir_0')[0].value == "asc" ? shortType = 1 : shortType = 0;

				var pagesize = aoData.filter(item => item.name == 'iDisplayLength')[0].value
				var pageindex = Math.floor((aoData.filter(item => item.name == 'iDisplayStart')[0].value + 1) / pagesize)
				var searchkeys = [];

				if(!!$scope.searchName) {
					var searchoption = {
						"key": "name",
						"value": $scope.searchName,
						"compareType": 6
					}
					searchkeys.push(searchoption)
				}
				if(!!$scope.searchIdentityNo) {
					var searchoption = {
						"key": "identityNo",
						"value": $scope.searchIdentityNo,
						"compareType": 6
					}
					searchkeys.push(searchoption)
				}
				aoData.push({
						"name": "pageIndex",
						"value": pageindex
					}, {
						"name": "pageSize",
						"value": pagesize
					}, 
					{
						"name": "orderColumn",//"value": orderMatchindex
						"value": "CREATETIME"
					}, 
					{
						"name": "orderType",
						"value": shortType
					}, {
						"name": "searchs",
						"value": JSON.stringify(searchkeys)
					}

				);
			},
			"aoColumns": $scope.getColumns(), // 字段定义
			// 初始化完成事件,这里需要用到 JQuery ，因为当前表格是 JQuery 的插件
			"initComplete": function(settings, json) {
				var filter = $(".searchContainer");
				filter.find("button").unbind("click.DT").bind("click.DT", function() {
					$("#tabManageRelatedparty").dataTable().fnDraw();
				});

				$scope.search = function(event, searchName) {

					if(searchName == 'serchName') {
						$scope.searchName = $("#serchN").val();
						$("#tabManageRelatedparty").dataTable().fnDraw();
					}
				}
				$scope.search();

			},
			//创建行，未绘画到屏幕上时调用
			"fnRowCallback": function(nRow, aData, iDisplayIndex, iDisplayIndexFull) {
				//将添加的angular事件添加到作用域中
				$compile(nRow)($scope);
			},
			"fnDrawCallback": function() {
				jqdatables.trcss();
			}
		}

		$scope.btn_addRelated = function(data) {
			var modalInstance = $modal.open({
				templateUrl: 'EditRelated.html', // 指向上面创建的视图
				controller: 'EditRelatedController', // 初始化模态范围
				size: "md",
				resolve: {
					params: function() {
						//                            return {
						////                                name: $scope.user,

						//                            };

					},

				}
			});
			modalInstance.opened.then(function() {
				console.log('modal is opened');

			})

			modalInstance.result.then(function(ret) {
				$state.go($state.$current.self.name, {}, {
					reload: true
				});
				console.log(ret);
			}, function(reason) {
				console.log(reason);
			});
		}
	}
]);

app.controller("EditRelatedController", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params", "toastr",
	function($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params, toastr) {
		$timeout.cancel($scope.timerM);
		$scope.getLanguage = function() {
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
		$rootScope.$on('$translateChangeEnd', function() {
			$scope.getLanguage();
			$state.go($state.$current.self.name, {}, {
				reload: true
			});

		});

		$scope.relatedType = [{
				id: "请选择",
				name: "请选择"
			}, {
				id: "User",
				name: "自然人"
			},
			{
				id: "Org",
				name: "机构"
			}
		]
		$scope.transType = [{
				id: "请选择",
				name: "请选择"
			}, {
				id: "授信",
				name: "授信"
			},
			{
				id: "交易",
				name: "交易"
			},
			{
				id: "服务",
				name: "服务"
			},
			{
				id: "股权",
				name: "股权"
			},
			{
				id: "其它",
				name: "其它"
			},
		]

		function initMatch() {
			var omodel = {};
			omodel.name = "";
			omodel.relatedType = "请选择";
			omodel.identityNo = "";
			omodel.transType = "请选择";
			omodel.transMoney = "";
			return JSON.parse(JSON.stringify(omodel));
			//			debugger
		}

		$scope.match = initMatch()
		//控件初始化参数
		$scope.EtartTimeOption = {
			dateFmt: 'yyyy-MM-dd',
			realDateFmt: "yyyy-MM-dd",
			minDate: '2012-1-1',
			maxDate: '2099-12-31',
			onpicked: function(e) {
				$scope.StartTime = e.el.value;
			}
		}
		$scope.EndTimeOption = {
			dateFmt: 'yyyy-MM-dd',
			realDateFmt: "yyyy-MM-dd",
			minDate: '2012-1-1',
			maxDate: '2099-12-31',
			onpicked: function(e) {
				$scope.EndTime = e.el.value;
			}
		}

		$scope.typeChange = function() {
			if($scope.match.transType == '请选择') {
				$scope.ManageForm.transType.$invalid = true;
				$scope.ManageForm.transType.$error.required = true;
				$scope.ManageForm.transType.$valid = false;
				$("select[name='transType']").attr("style", "border-color:red!important");
			} else {
				$("#tester").css("cssText", "height: 300px !important;");
				$("select[name='transType']").attr('style', 'border-color:#cfdadd!important');
			}
		}
		
		$scope.relatedChange = function() {
			if($scope.match.relatedType == '请选择') {
				$scope.ManageForm.relatedType.$invalid = true;
				$scope.ManageForm.relatedType.$error.required = true;
				$scope.ManageForm.relatedType.$valid = false;
				$("select[name='relatedType']").attr("style", "border-color:red!important");
			} else {
				$("#tester").css("cssText", "height: 300px !important;");
				$("select[name='relatedType']").attr('style', 'border-color:#cfdadd!important');
			}
		}
		
		

		$scope.divShow = function(type) {
			$(type).addClass("ng-hide");
		}
		
		
		

		$scope.ok = function(event, type) {  
			$(".relatedwarn").addClass("ng-hide");
			event.preventDefault();
			if($scope.ManageForm.$valid) {

			} else {
				if($scope.match.transType == "请选择") {
					$scope.ManageForm.transType.$invalid = true;
					$scope.ManageForm.transType.$error.required = true;
					$("select[name='transType']")[0].className = "form-control ng-pristine ng-untouched ng-invalid ng-invalid-required"
				}
					if($scope.match.relatedType == "请选择") {
					$scope.ManageForm.relatedType.$invalid = true;
					$scope.ManageForm.relatedType.$error.required = true;
					$("select[name='relatedType']")[0].className = "form-control ng-pristine ng-untouched ng-invalid ng-invalid-required"
				}
				$('input.ng-invalid,select.ng-invalid').addClass('redBorder');
				$scope.ManageForm.submitted = true;
				
				return;
			}
var dataUrl = "/portal/RelatedTrans/MatchTransData"
//			if(type == 'match') {
//				var dataUrl = "/portal/RelatedTrans/MatchTransData"
//			} else {
//				var dataUrl = "/portal/RelatedTrans/AddTransData"
//			}

			var dataModle = angular.copy($scope.match);
			$scope.match.transType == "请选择" ? dataModle.transType = "" : dataModle.transType = $scope.match.transType;

			$scope.match.relatedType == "请选择" ? dataModle.relatedType = "" : dataModle.relatedType = $scope.match.relatedType;
			$http({
				method: "post",
				url: dataUrl,
				transformRequest: angular.identity,
				data: JSON.stringify(dataModle),
			}).success(function(result, header, config, status) {
				if(!!result.success) {
					if(type == "match") {
						$scope.matchResult = result.data.grade;
						$("#matchResult" + result.data.grade).removeClass("ng-hide");
//						if(result.data.grade!=0){
//							$scope.AddBtn = true;
//						}
						
						$scope.matchText = "重新匹配";
						
					} else {
//						toastr.success('添加成功');
//						$scope.timerM = $timeout(function() {
//							$modalInstance.close($scope.selected);
//						}, 2000);


					}

				}else{
					toastr.error(result.errors);
				}

			})
		};
		
		
		$scope.cancel = function () {
            $modalInstance.dismiss('cancel'); // 退出
        }

	}
	   
])