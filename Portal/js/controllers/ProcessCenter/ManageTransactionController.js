﻿ //任务委托
app.controller('ManageTransactionController', ['$scope', "$rootScope", "$window", "$translate", "$http", "$state", "$filter", "$modal", "$compile", "ControllerConfig", "jq.datables", "FileUploader", "datecalculation", "toastr", "$timeout",
	function($scope, $rootScope, $window, $translate, $http, $state, $filter, $modal, $compile, ControllerConfig, jqdatables, FileUploader, datecalculation, toastr, $timeout) {
		//进入视图触发
		$scope.$on('$viewContentLoaded', function(event) {
//			$scope.init();
		});
//		$scope.init = function() {
//			$http({
//				method: 'get', //请求方式
//				url: "/portal/RelatedTrans/GetDetail/" + '67a93df3-a902-4a4f-a803-77ec137858c2',
//			}).success(function(result, header, config, status) {
//				if(!result) return
//				$scope.relatedInfo = result;
//				$scope.addBtn = true;
//
//			})
//		}

		//语言初始化
		$scope.getLanguage = function() {
			$scope.LanJson = {
				sLengthMenu: $translate.instant("uidataTable.sLengthMenu"),
				sZeroRecords: $translate.instant("uidataTable.sZeroRecords_NoAgnets"),
				sInfo: $translate.instant("uidataTable.sInfo"),
				sProcessing: $translate.instant("uidataTable.sProcessing"),

				Confirm_Delete: $translate.instant("WarnOfNotMetCondition.Confirm_Delete"),
				NoSelectItem: $translate.instant("WarnOfNotMetCondition.NoSelectAgent"),
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

		//初始化表单下拉框选项
		$scope.countrys = [{
				en: "请选择",
				cn: "请选择"
			}, 
			{
				en: "中国",
				cn: "中国"
			}, {
				en: "其它",
				cn: "其它"
			}
		]
		$scope.dutys = [{
				id: "请选择",
				name: "请选择"
			}, {
				id: "董事长",
				name: "董事长"
			},
			{
				id: "董事",
				name: "董事"
			},
			{
				id: "独立董事",
				name: "独立董事"
			},
			{
				id: "监事",
				name: "监事"
			},
			{
				id: "高管",
				name: "高管"
			},
			{
				id: "其它",
				name: "其它"
			}
		];

		$scope.highestEducations = [{
				id: "请选择",
				name: "请选择"
			}, {
				id: "小学及以下",
				name: "小学及以下"
			},
			{
				id: "初中",
				name: "初中"
			},
			{
				id: "职高",
				name: "职高"
			}, {
				id: "高中",
				name: "高中"
			}, {
				id: "大专",
				name: "大专"
			},
			{
				id: "本科",
				name: "本科"
			},
			{
				id: "硕士",
				name: "硕士"
			},
			{
				id: "博士及以上",
				name: "博士及以上"
			}
		]

		$scope.relations = [{
			id: "请选择",
			name: "请选择"
		}, {
			id: "股东",
			name: "股东"
		}, {
			id: "高管",
			name: "高管"
		}, {
			id: "重要岗位",
			name: "重要岗位"
		}, {
			id: "其它",
			name: "其它"
		}]
		
		$scope.categorys = [{
			id:"请选择",
			name:"请选择"
		},{
			id:"上市公司",
			name:"上市公司"
		},
		{
			id:"民企",
			name:"民企"
		},
		{
			id:"国营企业",
			name:"国营企业"
		},
		{
			id:"事业单位",
			name:"事业单位"
		},
		{
			id:"其它",
			name:"其它"
		}]

		//初始机构信息
		$scope.relatedInfo = initOrganization();

		//初始化人员信息
		$scope.personInfo = initPerson();

        $scope.allInfo = {}; //保存接口开始所有的信息

        $scope.addBtn = true;

		function initOrganization() {
			var omodel = {};
			omodel.id = "";
			omodel.name = "";
			omodel.category = "请选择";
			omodel.licenseNo = "";
			omodel.legalName = "";
			omodel.legalMobile = "";
			omodel.operatingPeriod = "";
			omodel.registerAsset = "";
			omodel.registerAddress = "";
			omodel.businessAddress = "";
			omodel.BGSZGBL = "";
			omodel.ZBGSGQBL = "";
			omodel.businessScope = "";
			omodel.shareRatio = 0;
			omodel.banks = [{
				depositBank: "",
				bankNo: ""
			}];
			return JSON.parse(JSON.stringify(omodel));
		}

		function initPerson() {
			var omodel = {};
			omodel.name = "";
			omodel.id = "";
			omodel.passport = "";
			omodel.idcard = "";
			omodel.country = "请选择";
			omodel.duty = "请选择";
			omodel.relation = "请选择";
			omodel.highestEducation = "请选择";
			omodel.age = '';
			omodel.sex = 1;
			omodel.graduatedSchool = "";
			omodel.huJiAddress = "";
			omodel.juZhuAddress = "";
			omodel.mobile = "";
			omodel.email = "";
			omodel.shareRation = 0;
			omodel.banks = [{
				depositBank: "",
				bankNo: ""
			}];
			return JSON.parse(JSON.stringify(omodel));
		}

		function treeClick(event, treeId, treeNode) {
			$scope.reEvent = event;
			$scope.reTreenode = treeNode;
			$scope.reSelectnode=treeNode;
			$scope.deleteCredentials = [];
			$scope.deleteAttachments = [];
			$('input,select').removeClass("redBorder")
			$("canvas").remove();
			var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
var treeNodes=treeObj.getNodes();

			if(!treeNode || !treeNode.id) {
				//$scope.parentId = '67a93df3-a902-4a4f-a803-77ec137858c2'

			} else {
				$scope.parentId = treeNode.id;
				if(treeNode.level == 2) {
					$scope.addBtn = false;
				} else {
					if(treeNode.level == 0) {
						$scope.clickAdd = false;
					}
					$scope.addBtn = true;
				}
				getDetail(treeNode);
			}
		
		}

	
		function getDetail(treeNode){
			if(!!treeNode){
				$scope.parentId = treeNode.id;
			}
				$http({
				method: 'get', //请求方式
				url: "/portal/RelatedTrans/GetDetail/" + $scope.parentId,
			}).success(function(result, header, config, status) {
//				debugger
				$scope.baseName = result.name;
				if(!!result.credentials&&result.credentials.length > 0) {
					var imgList = []
					for(var i = 0; i < result.credentials.length; i++) {
						imgList[i] = result.credentials[i];
						imgList[i].url = location.origin + "/portal/RelatedTrans/GetCredential/" + result.credentials[i].fileId;
						imgList[i].id = result.credentials[i].fileId;
						imgList[i].type = "image/jpeg";
						imgList[i].size = 389417;
					}
					if(treeNode.type == "subOrgs") {
						$scope.feedbackimg(imgList, uploader, "#dcredentialfiles"); //#dcredentialfiles
					} else {
						$scope.feedbackimg(imgList, uploader, "#psersonfiles");
						
					}
				} else {
					$scope.uploader.queue = [];
				}
				if(!!result.attachments&&result.attachments.length > 0) {

					var fileList = []
					for(var i = 0; i < result.attachments.length; i++) {
						fileList[i] = result.attachments[i];
						fileList[i].url = "/portal/RelatedTrans/GetAttachment/" + result.attachments[i].fileId;
						fileList[i].id = result.attachments[i].fileId;
					}
					if(treeNode.type == "subOrgs") {
						$scope.feedbackimg(fileList, uploader1, "#dcredentialfiles");
					} else {
						$scope.feedbackimg(fileList, uploader1, "#psersonfiles");
					}

				} else {
					$scope.uploader1.queue = [];
				}

				if(treeNode.type == "subOrgs" || treeNode.type == "nodeParent") {
					$scope.addusershow = false;
					$scope.addManageshow = true;
					if(result.banks.length == 0) {
						result.banks = [{
							depositBank: "",
							bankNo: ""
						}];
					}
					$scope.relatedInfo = result;
					
				} else if(treeNode.type == "subUsers") {

					$scope.addusershow = true;
					$scope.addManageshow = false;
					if(!!result.banks&&result.banks.length == 0) {
						result.banks = [{
							depositBank: "",
							bankNo: ""
						}];
					}
					
					$scope.personInfo = result;
					if(!!result.mobile){
						$scope.personInfo.mobile=Number(result.mobile)
					}
					
				}

			});			
			$scope.deleteCredentials = [];
			$scope.deleteAttachments = [];						
		}

		function addNode(parentId, node) {
			var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
			//追加机构
			if(node.hasOwnProperty("subOrgs") && node["subOrgs"].length > 0) {
				var subOrgs = node["subOrgs"];

				for(var k = 0; k < subOrgs.length; k++) {
					var nodeInfo = {};
					nodeInfo.name = subOrgs[k].name;
					nodeInfo.icon = '../../Portal/WFRes/image/related.png';
					nodeInfo.id = subOrgs[k].id;
					nodeInfo.parentId = subOrgs[k].parentId;
					nodeInfo.type = "subOrgs";
					nodeInfo.open = true;
					
					if(subOrgs[k].level == 2) {
						nodeInfo.isParent = false;
					} else {
						nodeInfo.children = [];
					}
						if(nodeInfo.id == $scope.aacurId) {
						parentId.open = true;
					} else {
						parentId.open = false
					}
					parentId.children.push(nodeInfo);
					addNode(nodeInfo, subOrgs[k]);
				}
			}
			// 追加人
			if(node.hasOwnProperty("subUsers") && node["subUsers"].length > 0) {
				var subUsers = node["subUsers"];

				for(var k = 0; k < subUsers.length; k++) {
					var nodeInfo = {};
					nodeInfo.name = subUsers[k].name;
					nodeInfo.icon = "../../Portal/WFRes/image/person.png";
					nodeInfo.id = subUsers[k].id;
					nodeInfo.parentId = subUsers[k].parentId;
					nodeInfo.type = "subUsers";
					nodeInfo.tId = $scope.trnodeId
					nodeInfo.open = true;
					if(subUsers[k].level == 2) {
						nodeInfo.isParent = false;
					} else {
						nodeInfo.children = []
					}
					if(nodeInfo.id == $scope.aacurId) {
						parentId.open = true;
					} else {
						parentId.open = false
					}
					parentId.children.push(nodeInfo);
					addNode(nodeInfo, subUsers[k]);
				}
			}
		}

		//表单保存提交
		$scope.submitted = false;
		$scope.MaOnSubmitForm = function(event, type) {			
					if($scope.reSelectnode){
					$scope.parentId=$scope.reSelectnode.id;
					}						
			event.preventDefault();
			var datamodel = {}
			if(type == 'related') {
				$scope.typeform = $scope.relatedform;
				$scope.typeInfo = $scope.relatedInfo;
//				if($scope.relatedInfo.level == 0) {
//					toastr.error('主机构信息不允许修改');
//					return;
//				}
				datamodel = $scope.relatedInfo;
				if(!!datamodel.id) {
					dataUrl = '/portal/RelatedTrans/ModifyOrg';
					$scope.tips = "修改成功"
				} else {
					dataUrl = '/portal/RelatedTrans/AddOrg';
					$scope.tips = "新增成功"
				}
			} else {
				$scope.typeform = $scope.personform;
				$scope.typeInfo = $scope.personInfo;
				datamodel = $scope.personInfo;
				if(!!datamodel.id) {
					dataUrl = '/portal/RelatedTrans/ModifyUser';
					$scope.tips = "修改成功"
				} else {
					dataUrl = '/portal/RelatedTrans/AddUser';
					$scope.tips = "新增成功"
				}
			}
             
			if($scope.typeInfo.relation == "请选择") {
				$scope.typeform.relation.$invalid = true;
				$scope.typeform.relation.$error.required = true;
				$("select[name='relation']")[0].className = "form-control ng-pristine ng-untouched ng-invalid ng-invalid-required"
				$scope.typeform.$valid = false
			}
			if($scope.typeInfo.category == "请选择") {
				$scope.typeform.category.$invalid = true;
				$scope.typeform.category.$error.required = true;
				$("select[name='category']")[0].className = "form-control ng-pristine ng-untouched ng-invalid ng-invalid-required"
				$scope.typeform.$valid = false
			}
			
			if($scope.typeform.$valid) {

			} else {
				$('input.ng-invalid,select.ng-invalid').addClass('redBorder');
				$scope.typeform.submitted = true;
				return;
			}

			datamodel.parentId = $scope.parentId;
			var form1 = new FormData();

			if(!!$scope.deleteCredentials && $scope.deleteCredentials.length > 0) {
				for(let i = 0; i < $scope.deleteCredentials.length; i++) {
					if(!!$scope.deleteCredentials[i].id) {
						form1.append("deleteCredentials[" + i.toString() + "]", $scope.deleteCredentials[i].id);
					}
				}
			}
			if(!!$scope.deleteAttachments && $scope.deleteAttachments.length > 0) {
				for(let i = 0; i < $scope.deleteAttachments.length; i++) {
					if(!!$scope.deleteAttachments[i].id) {
						form1.append("deleteAttachments[" + i.toString() + "]", $scope.deleteAttachments[i].id);
					}
				}
			}

			if($scope.uploader.queue.length > 0) {
				for(let i = 0; i < $scope.uploader.queue.length; i++) {

					if(!$scope.uploader.queue[i].hasOwnProperty('id')) {
						form1.append("credentials.index", i.toString());
						form1.append("credentials[" + i.toString() + "]", $scope.uploader.queue[i]._file);

					}
				}
			}

			if($scope.uploader1.queue.length > 0) {
				for(let i = 0; i < $scope.uploader1.queue.length; i++) {
					if(!$scope.uploader1.queue[i].hasOwnProperty('id')) {
						form1.append("attachments.index", i.toString());
						form1.append("attachments[" + i.toString() + "]", $scope.uploader1.queue[i]._file);
					}
				}
			}

			for(var prop in datamodel) {
				if(prop == "banks" && datamodel["banks"].length > 0) {
					for(var i = 0; i < datamodel["banks"].length; i++) {
//						debugger
						if(!!datamodel["banks"][i].id) {
//							debugger
							form1.append("banks[" + i + "].id", datamodel["banks"][i].id);
						}
						if(!!datamodel["banks"][i].depositBank.trim()) {
							form1.append("banks[" + i + "].depositBank", datamodel["banks"][i].depositBank);
						}
						if(!!datamodel["banks"][i].bankNo.trim()) {
							form1.append("banks[" + i + "].bankNo", datamodel["banks"][i].bankNo);
						}

					
					}
				} else if(prop == "banks" && datamodel["banks"].length == 0) {
					delete datamodel["banks"]
				} else if(prop == 'credentials') {
					datamodel['credentials'] = [];
				} else if(prop == 'attachments') {
					datamodel['attachments'] = [];
				} else {
					if(datamodel[prop] != null) {
						form1.append(prop, datamodel[prop]);
					} else {
						form1.append(prop, '');
					}

				}
			}
			console.log(form1.get("parentId"));

			$("select").change(function() {
				var checkText = $(this).find("option:selected").text();
				if(checkText != '请选择') {
					$(this)[0].className = "form-control ng-pristine ng-untouched ng-valid ng-valid-required"
				}
			});

			$http({
				method: 'post', //请求方式
				url: dataUrl,
				transformRequest: angular.identity,
				data: form1,
				headers: {
					'Content-Type': undefined
				}, //请求头信息
				transformRequest: angular.identity
			}).success(function(result, header, config, status) {
				if(!result.success) {
					toastr.error(result.errors);
				} else {
					if(!!result.data) {
						datamodel.id = result.data.id;
						$scope.aacurId = result.data.id;
						datamodel.level=result.data.Level;
						if(result.data.Level==2){
							
							$scope.addBtn=false;
						}
						dataUrl = '/portal/RelatedTrans/ModifyUser';
						
						
						if(result.credentials.length > 0) {
							for(let i = 0; i < result.credentials.length; i++) {
								$scope.uploader.queue[i].id = result.credentials[i].fileId
							}
						}
						if(result.attachments.length > 0) {
							for(let i = 0; i < result.attachments.length; i++) {
								$scope.uploader1.queue[i].id = result.attachments[i].fileId
									$scope.uploader1.queue[i].progress = 100;
									$scope.uploader1.queue[i].isUploaded = true;
									$scope.uploader1.queue[i].isSuccess = true;

							}
						}						
						if(!!result.data.Banks&&result.data.Banks.length > 0) {
							for(var i=0;i<result.data.Banks.length;i++){
								datamodel.banks[i].id=result.data.Banks[i].id;
							}
							
						}
						if(!!$scope.reTreenode){
								if($scope.reTreenode.children > 0) {
													for(var i = 0; i < $scope.reTreenode.children.lenght; i++) {
														if($scope.reTreenode.children[i].id == result.data.id) {
															$scope.trnodeId = $scope.reTreenode.children[i].tId;
														}
													}
											}							
						}else{
							var nodes=$scope.treeObj.getNodes();
							$scope.reTreenode=nodes[0]
						}					
						$scope.treeDemo.init();
					}else{
						getDetail($scope.reSelectnode)						
					}
						toastr.success($scope.tips);		
                }
//              $scope.deleteCredentials = [];
//              $scope.deleteAttachments = [];
			})

			$("select").change(function() {
				var checkText = $(this).find("option:selected").text();
				if(checkText != '请选择') {
					$(this)[0].className = "form-control ng-pristine ng-untouched ng-valid ng-valid-required"
				}
			});
		}

		//时间选择器
		$scope.setTime_cdt = function() {
			if($scope.week_month == "week") {
				$scope.StartTime = datecalculation.redDays(new Date(), 7);
				$scope.EndTime = datecalculation.addDays(new Date(), 1);
			} else {
				$scope.StartTime = datecalculation.redDays(new Date(), 30);
				$scope.EndTime = datecalculation.addDays(new Date(), 1);
			}
		}

		$scope.feedbackimg = function(imgList, upload, dcredentialfiles) {
			upload.queue = [];
			for(var i = 0, len = imgList.length; i < len; i++) {
				var img1 = new FileUploader.FileItem(upload, {
					lastModifiedDate: new Date(),
					size: imgList[i].size,
					type: "image/jpeg",
					name: imgList[i].fileName,
				});
				img1.isReady = false;
				img1.progress = 100;
				img1.isUploaded = true;
				img1.isSuccess = true;
				img1.fileurl = imgList[i].url;
				img1.id = imgList[i].id;
				img1.name = imgList[i].fileName;
				upload.queue.push(img1);

				if(imgList[i].type == "image/jpeg") {
					render(imgList[i].url, i, dcredentialfiles);
				}

			}
		};

		// 参数，最大高度
		var MAX_HEIGHT = 100;
		// 渲染
		function render(src, i, dcredentialfiles) {
			// 创建一个 Image 对象

			var image = new Image();
			image.src = src;

			image.onload = function() {

				var cq = dcredentialfiles + " canvas";
				var canvas = document.querySelectorAll(cq);
				// 如果高度超标
				if(image.height > MAX_HEIGHT) {
					image.width *= MAX_HEIGHT / image.height;
					image.height = MAX_HEIGHT;
				}
				if(canvas.length > 0) {
					if(!!canvas[i].getContext("2d")){
						var ctx = canvas[i].getContext("2d");
						
					}
					
					// canvas清屏
					ctx.clearRect(0, 0, canvas[i].width, canvas[i].height);
					var width = canvas[i].width || image.width / image.height * canvas[i].height;
					var height = canvas[i].height || image.height / image.width * canvas[i].width;
					// 重置canvas宽高
					$(canvas[i]).attr({
						width: width,
						height: height
					});
					// 将图像绘制到canvas上
					ctx.drawImage(image, 0, 0, width, height);
				}
			}
			image.onload();

		};

		$scope.addManageshow = true;

		$scope.addRole = function(formname) {
			$('input,select').removeClass('redBorder')
			//			$scope.clickAdd = false;
			$scope.uploader.queue = []
			$scope.uploader1.queue = []
			$("canvas").remove();
			if(formname == "Manageform") {
				$scope.relatedInfo = initOrganization();
				$scope.addusershow = false;
				$scope.addManageshow = true;
				$scope.baseName = "新增关联方（机构）";
				$scope.$watch("relatedInfo.name", function(newValue, oldValue) {
					oldValue = ""
					if(oldValue != newValue) {
						$scope.baseName = newValue;
					}
				})

				for(var t in $scope.relatedform) {
					if(JSON.stringify(t).indexOf("$") == -1 && JSON.stringify(t).indexOf("$$") == -1) {
						$scope.relatedform[t].$dirty = false;
						$scope.relatedform.submitted = false;

					}

				}

			} else if(formname == "userform") {
				$scope.personInfo = initPerson();
				$scope.addManageshow = false;
				$scope.addusershow = true;
				$scope.baseName = "新增关联方（自然人）";
				$scope.$watch("personInfo.name", function(newValue, oldValue) {
					oldValue = ""
					if(oldValue != newValue) {
						$scope.baseName = newValue;
					}
				})

				for(var t in $scope.personform) {
					if(JSON.stringify(t).indexOf("$") == -1 && JSON.stringify(t).indexOf("$$") == -1) {
						$scope.personform[t].$dirty = false;
						$scope.personform.submitted = false;
					}
				}
			}
		}

		//		$scope.addManage = function(formname) {
		//			$('input,select').removeClass('redBorder')
		//			$scope.clickAdd = false;
		//			$scope.uploader.queue = []
		//			$scope.uploader1.queue = []
		//			$("canvas").remove();
		//			$scope.relatedInfo = initOrganization();
		//
		//			if(formname == "Manageform") {
		//				$scope.relatedInfo = initOrganization();
		//			} else if(formname == "userform") {
		//				$scope.personInfo = initPerson();
		//			}
		//			$scope.addusershow = false;
		//			$scope.addManageshow = true;
		//
		//		}
		$scope.addClose = function() {
			$scope.dongshow = true;
			$scope.addusershow = false;
			$scope.addManageshow = false;
		}
		$scope.btn_addBank = function(banktype) {
			if(!$scope.personInfo.hasOwnProperty('banks')) {
				$scope.personInfo.banks = [];
			}
			if(banktype == "personInfo") {
				$scope.personInfo.banks.push({
					depositBank: '',
					bankNo: ''
				});
			} else {
				$scope.relatedInfo.banks.push({
					depositBank: '',
					bankNo: ''
				});
			}

		}
		$scope.btn_addFamily = function(family) {
			$scope.family.push({
				name: '',
				idnumber: '',
				relation: '',
				birth: ''
			});
		}
		$scope.btn_addorganization = function(family) {
			$scope.orlist.push({
				organization: '',
				registrationNo: '',
				nature: ''
			});
		}

		app.directive('ngFocus', [function() {
			var FOCUS_CLASS = "ng-focused";
			return {
				restrict: 'A',
				require: 'ngModel',
				link: function(scope, element, attrs, ctrl) {
					ctrl.$focused = false;
					element.bind('focus', function(evt) {
						element.addClass(FOCUS_CLASS);
						scope.$apply(function() {
							ctrl.$focused = true;
						})
					}).bind('blur', function(evt) {
						element.removeClass(FOCUS_CLASS);
						scope.$apply(function() {
							ctrl.$focused = false;
						})
					})
				}
			}
		}])

		//初始化树
		$scope.treeDemo = {
			'setting': {
				callback: {
					onClick: treeClick,
				},
				view: {},
				check: {
					enable: true
				}
			},
			'zNodes': [{
					name: "父节点1 - 展开",
					open: true,
					children: [{
							name: "父节点11 - 折叠",
							children: [{
									name: "叶子节点111"
								},
								{
									name: "叶子节点112"
								},
								{
									name: "叶子节点113"
								},
								{
									name: "叶子节点114"
								}
							]
						},
						{
							name: "父节点12 - 折叠",
							children: [{
									name: "叶子节点121"
								},
								{
									name: "叶子节点122"
								},
								{
									name: "叶子节点123"
								},
								{
									name: "叶子节点124"
								}
							]
						},
						{
							name: "父节点13 - 没有子节点",
							isParent: true
						}
					]
				},
				{
					name: "父节点2 - 折叠",
					children: [{
							name: "父节点21 - 展开",
							open: true,
							children: [{
									name: "叶子节点211"
								},
								{
									name: "叶子节点212"
								},
								{
									name: "叶子节点213"
								},
								{
									name: "叶子节点214"
								}
							]
						},
						{
							name: "父节点22 - 折叠",
							children: [{
									name: "叶子节点221"
								},
								{
									name: "叶子节点222"
								},
								{
									name: "叶子节点223"
								},
								{
									name: "叶子节点224"
								}
							]
						},
						{
							name: "父节点23 - 折叠",
							children: [{
									name: "叶子节点231"
								},
								{
									name: "叶子节点232"
								},
								{
									name: "叶子节点233"
								},
								{
									name: "叶子节点234"
								}
							]
						}
					]
				},
				{
					name: "父节点3 - 没有子节点",
					isParent: true
				}
			],
			'init': function() {
				//$.fn.zTree.init($("#treeDemo"), $scope.treeDemo.setting, $scope.treeDemo.zNodes);
				$http({
					url: "/portal/RelatedTrans/GetTree"
				}).success(function(result, header, config, status) {
					$scope.allInfo = result;
					var nodeParent = {};
					nodeParent.name = result.name;
					nodeParent.id = result.id;
					$scope.parentId = result.id;
					nodeParent.parentId = "0";
					nodeParent.type = "nodeParent";
					nodeParent.children = [];
					nodeParent.icon = '../../Portal/WFRes/image/related.png';
					addNode(nodeParent, result);
					$.fn.zTree.init($("#treeDemo"), $scope.treeDemo.setting, nodeParent);
					var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
					$scope.treeObj = treeObj
					var nodes = [];
					var nodes = treeObj.getNodes();
                    treeObj.expandNode(nodes[0], true, false, true);
                    getDetail(nodes[0]);
					//$scope.tips
					for(var i = 0; i < nodes[0].children.length; i++) { //设置节点展开
						(function(i) {
							$scope.classOpen = [];
							if(nodes[0].children[i].children.length <= 0) {
								nodes[0].children[i].open = true;
								nodes[0].children[i].isParent = false;
								$scope.classOpen.push(nodes[0].children[i].tId)
								if($scope.tips == "新增成功" && ($scope.reTreenode.id == nodes[0].id || !$scope.reTreenode.id) ) {
								
									if($scope.aacurId == nodes[0].children[i].id){
										$scope.reSelectnode =nodes[0].children[i]
								
									treeObj.selectNode($scope.reSelectnode, false, false);
									}
									
									
								}								
							} else {
									for(var j = 0; j < nodes[0].children[i].children.length; j++) {
									nodes[0].children[i].children[j].open = true;
									if(nodes[0].children[i].children[j].id==$scope.aacurId){
										$scope.reSelectnode =nodes[0].children[i].children[j]
										treeObj.selectNode($scope.reSelectnode, false, false);
										
									}
									
								}
								if($scope.tips == "新增成功" && $scope.reTreenode.id == nodes[0].children[i].id) {
									nodes[0].children[i].open = true;
								}
							}
						})(i)
					};
				})
			}
		}

		$scope.treeDemo.init();

		//图片上传处理
		var uploader = $scope.uploader = new FileUploader({
			url: '/portal/RelatedTrans/AddUser',
			method: 'POST',
			queueLimit: 100,
			autoUpload: false
		});

		uploader.filters.push({
			name: 'imageFilter',
			fn: function(item /*{File|FileLikeObject}*/ , options) {
				var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
				if('|jpg|png|jpeg|bmp|gif|'.indexOf(type) == -1) {
					toastr.warning('只能选择图片格式的文件进行上传', '文件格式错误');
					return false;
				} else {
					return true;
				}
			}
		});
		$scope.deleteCredentials = [];
		uploader.removeFromQueue = function(value) {
			for(var i = 0; i < value.uploader.queue.length; i++) {
				if(value.$$hashKey == value.uploader.queue[i].$$hashKey) {
					value.uploader.queue.splice(i, 1);
					if(value.hasOwnProperty('id')) {
						$scope.deleteCredentials.push(value)
					}
				}
			}
		}

		// CALLBACKS
		uploader.onWhenAddingFileFailed = function(item /*{File|FileLikeObject}*/ , filter, options) {
			console.info('onWhenAddingFileFailed', item, filter, options);
		};
		uploader.onAfterAddingFile = function(fileItem) {
			console.info('onAfterAddingFile', fileItem);
			$scope.fileItem = fileItem._file;
			var thetype = fileItem._file.type.split("/")[fileItem._file.type.split("/").length - 1];
			if(uploader.filters[uploader.filters.length - 1].name == "imageFilter") {
				if(thetype != "jpg" || thetype != "png" || thetype != "jpeg" || thetype != "bmp" || thetype != "gif") {
					$scope.thetypefile = false;
				}
			} else {
				$scope.thetypefile = true;
			}

		};
		uploader.onAfterAddingAll = function(addedFileItems) {
			console.info('onAfterAddingAll', addedFileItems);
		};
		uploader.onBeforeUploadItem = function(item) {
			console.info('onBeforeUploadItem', item);
		};
		uploader.onProgressItem = function(fileItem, progress) {
			console.info('onProgressItem', fileItem, progress);
		};
		uploader.onProgressAll = function(progress) {
			console.info('onProgressAll', progress);
		};
		uploader.onSuccessItem = function(fileItem, response, status, headers) {
			console.info('onSuccessItem', fileItem, response, status, headers);
		};
		uploader.onErrorItem = function(fileItem, response, status, headers) {
			console.info('onErrorItem', fileItem, response, status, headers);
		};
		uploader.onCancelItem = function(fileItem, response, status, headers) {
			console.info('onCancelItem', fileItem, response, status, headers);

		};
		uploader.onCompleteItem = function(fileItem, response, status, headers) {
			console.info('onCompleteItem', fileItem, response, status, headers);
		};
		uploader.onCompleteAll = function() {
			console.info('onCompleteAll');
		};
		console.info('uploader', uploader);

		var uploader1 = $scope.uploader1 = new FileUploader({
			url: '/portal/RelatedTrans/AddUser',
			method: 'POST',
			queueLimit: 100,
			autoUpload: false
		});
		$scope.deleteAttachments = [];
		uploader1.removeFromQueue = function(value, index) {
			for(var i = 0; i < value.uploader.queue.length; i++) {
				if(value.$$hashKey == value.uploader.queue[i].$$hashKey) {
					value.uploader.queue.splice(i, 1);
					if(value.hasOwnProperty('id')) {
						$scope.deleteAttachments.push(value)
					}
				}
			}
		}

		uploader1.onWhenAddingFileFailed = function(item /*{File|FileLikeObject}*/ , filter, options) {
			console.info('onWhenAddingFileFailed', item, filter, options);
		};
		uploader1.onAfterAddingFile = function(fileItem) {
			console.info('onAfterAddingFile', fileItem);
			var thetype = fileItem._file.type.split("/")[fileItem._file.type.split("/").length - 1];
			if(uploader1.filters[uploader1.filters.length - 1].name == "imageFilter") {
				if(thetype != "jpg" || thetype != "png" || thetype != "jpeg" || thetype != "bmp" || thetype != "gif") {
					$scope.thetypefile = false;
				}
			} else {
				$scope.thetypefile = true;
			}

		};
		uploader1.onAfterAddingAll = function(addedFileItems) {
			console.info('onAfterAddingAll', addedFileItems);
		};
		uploader1.onBeforeUploadItem = function(item) {
			console.info('onBeforeUploadItem', item);
		};
		uploader1.onProgressItem = function(fileItem, progress) {

			console.info('onProgressItem', fileItem, progress);
		};
		uploader1.onProgressAll = function(progress) {
			console.info('onProgressAll', progress);
		};
		uploader1.onSuccessItem = function(fileItem, response, status, headers) {
			console.info('onSuccessItem', fileItem, response, status, headers);
		};
		uploader1.onErrorItem = function(fileItem, response, status, headers) {
			console.info('onErrorItem', fileItem, response, status, headers);
		};
		uploader1.onCancelItem = function(fileItem, response, status, headers) {
			console.info('onCancelItem', fileItem, response, status, headers);

		};
		uploader1.onCompleteItem = function(fileItem, response, status, headers) {
			console.info('onCompleteItem', fileItem, response, status, headers);
		};
		uploader1.onCompleteAll = function() {
			console.info('onCompleteAll');
		};
		console.info('uploader', uploader1);

		//点击图片跳转
		$scope.imgUrl = function(src) {
			window.open(src, '_blank');
		}





	}
]);

app.directive('ngThumb', ['$window', function($window) {
	var helper = {
		support: !!($window.FileReader && $window.CanvasRenderingContext2D),
		isFile: function(item) {
			return angular.isObject(item) && item instanceof $window.File;
		},
		isImage: function(file) {
			var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
			return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
		}
	};
	return {
		restrict: 'A',
		template: '<canvas/>',
		link: function(scope, element, attributes) {
			if(!helper.support) return;

			var params = scope.$eval(attributes.ngThumb);

			if(!helper.isFile(params.file)) return;
			if(!helper.isImage(params.file)) return;

			var canvas = element.find('canvas');
			var reader = new FileReader();

			reader.onload = onLoadFile;
			reader.readAsDataURL(params.file);

			function onLoadFile(event) {
				var img = new Image();
				img.onload = onLoadImage;
				img.src = event.target.result;
			}

			function onLoadImage() {
				var width = params.width || this.width / this.height * params.height;
				var height = params.height || this.height / this.width * params.width;
				canvas.attr({
					width: width,
					height: height
				});
				canvas[0].getContext('2d').drawImage(this, 0, 0, width, height);
			}
		}
	};

}])