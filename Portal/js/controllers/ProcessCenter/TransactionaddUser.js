//任务委托
app.controller('TransactionaddUserController', ['$scope', "$rootScope", "$window", "$translate", "$http", "$state", "$filter", "$modal", "$compile", "ControllerConfig", "jq.datables", "FileUploader", "datecalculation",
    function ($scope, $rootScope, $window, $translate, $http, $state, $filter, $modal, $compile, ControllerConfig, jqdatables, FileUploader, datecalculation) {
        //进入视图触发
        $scope.$on('$viewContentLoaded', function (event) {

        });

        //表单的高度
        $('.contentBox').height($(window).height() - ($('#divTopBars').offset().top + 50));
        $('.collapse.in').prev('.panel-heading').addClass('active');
        $('#accordion, #bs-collapse')
            .on('show.bs.collapse', function (a) {
                $(a.target).prev('.panel-heading').addClass('active');
            })
            .on('hide.bs.collapse', function (a) {
                $(a.target).prev('.panel-heading').removeClass('active');
            });

        $scope.StartTime = "";
        $scope.EndTime = "";
        $scope.StartTime = datecalculation.redDays(new Date(), 7);
        $scope.EndTime = datecalculation.addDays(new Date(), 1);

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
        var uploader = $scope.uploader = new FileUploader({
            url: '/vendor/jquery/angular-file-upload/upload.php',
        });
        $scope.single = function (ev) {
            uploader.filters = [];
            if (ev.target.dataset.type == 'image') {

                uploader.filters.push({
                    name: 'imageFilter',
                    fn: function (item /*{File|FileLikeObject}*/, options) {
                        var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                        return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
                    }
                });
            } else {
                // a sync filter              
                uploader.filters.push({
                    name: 'syncFilter',
                    fn: function (item /*{File|FileLikeObject}*/, options) {
                        console.log('syncFilter');
                        return this.queue.length < 10;
                    }
                });
                // an async filter
                uploader.filters.push({
                    name: 'asyncFilter',
                    fn: function (item /*{File|FileLikeObject}*/, options, deferred) {
                        console.log('asyncFilter');
                        setTimeout(deferred.resolve, 1e3);
                    }
                });

            }

        }

        // CALLBACKS

        uploader.onWhenAddingFileFailed = function (item /*{File|FileLikeObject}*/, filter, options) {
            console.info('onWhenAddingFileFailed', item, filter, options);
        };
        uploader.onAfterAddingFile = function (fileItem) {
            console.info('onAfterAddingFile', fileItem);
            debugger
            var thetype = fileItem._file.type.split("/")[fileItem._file.type.split("/").length - 1];
            if (uploader.filters[uploader.filters.length - 1].name == "imageFilter") {
                if (thetype != "jpg" || thetype != "png" || thetype != "jpeg" || thetype != "bmp" || thetype != "gif") {
                    $scope.thetypefile = false;
                }
            } else {
                $scope.thetypefile = true;
            }

        };
        uploader.onAfterAddingAll = function (addedFileItems) {
            console.info('onAfterAddingAll', addedFileItems);
        };
        uploader.onBeforeUploadItem = function (item) {
            console.info('onBeforeUploadItem', item);
        };
        uploader.onProgressItem = function (fileItem, progress) {
            console.info('onProgressItem', fileItem, progress);
        };
        uploader.onProgressAll = function (progress) {
            console.info('onProgressAll', progress);
        };
        uploader.onSuccessItem = function (fileItem, response, status, headers) {
            console.info('onSuccessItem', fileItem, response, status, headers);
        };
        uploader.onErrorItem = function (fileItem, response, status, headers) {
            console.info('onErrorItem', fileItem, response, status, headers);
        };
        uploader.onCancelItem = function (fileItem, response, status, headers) {
            console.info('onCancelItem', fileItem, response, status, headers);
        };
        uploader.onCompleteItem = function (fileItem, response, status, headers) {
            console.info('onCompleteItem', fileItem, response, status, headers);
        };
        uploader.onCompleteAll = function () {
            console.info('onCompleteAll');
        };

        console.info('uploader', uploader);

        $scope.dongshow = true;

        $scope.addUser = function () {
            $scope.addusershow = true;
            $scope.addManageshow = false;
            $scope.dongshow = false;

        }
        $scope.addManage = function () {
            $scope.addusershow = false;
            $scope.addManageshow = true;
            $scope.dongshow = false;

        }
        $scope.addClose = function () {
            $scope.dongshow = true;
            $scope.addusershow = false;
            $scope.addManageshow = false;
        }
        $scope.getLanguage = function () {
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
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });


        $scope.btn_addBank = function (banks) {
            $scope.banks.push({ depositBank: '', bankAccount: '' });

        }
        $scope.btn_addFamily = function (family) {
            $scope.family.push({ name: '', idnumber: '', relation: '', birth: '' });

        }
        $scope.btn_addorganization = function (family) {
            $scope.orlist.push({ organization: '', registrationNo: '', nature: '' });

        }

        $scope.btn_addAgents = function (data) {
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
                        templateUrl: 'EditAgency.html',    // 指向上面创建的视图
                        controller: 'EditAgencyController',// 初始化模态范围
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
        $scope.hidediv = function (id, ctrl1) {
            var ctrl = document.getElementById(id);
            var ctrl1 = document.getElementsByClassName(ctrl1)
            if ($(ctrl).css("display") != 'none') {
                $(ctrl).slideUp(500);

                //class ="nav-icon fa fa-chevron-down bannerTitle" 
                if ($(ctrl1).hasClass("fa-angle-down") || $(ctrl1).hasClass("fa-angle-right")) {
                    $(ctrl1).removeClass("fa-angle-down");
                    $(ctrl1).addClass("fa-angle-right");
                }
                else {
                    $(ctrl1).removeClass("fa-chevron-down");
                    $(ctrl1).addClass("fa-chevron-right");
                }

                //      $(ctrl1).css({ "margin-bottom": "8px", "border-bottom": "1px solid #ccc" });
            }
            else {
                $(ctrl).slideDown(500);
                if ($(ctrl1).hasClass("fa-angle-down") || $(ctrl1).hasClass("fa-angle-right")) {
                    $(ctrl1).removeClass("fa-angle-right");
                    $(ctrl1).addClass("fa-angle-down");
                }
                else {
                    $(ctrl1).removeClass("fa-chevron-right");
                    $(ctrl1).addClass("fa-chevron-down");
                }

                //      $(ctrl1).css({ "margin-bottom": "0px", "border-bottom": "0px solid #ccc" });
            }
        }
        $scope.treeDemo = {
            'setting': {},
            'zNodes': [
                {
                    name: "父节点1 - 展开", open: true,
                    children: [
                        {
                            name: "父节点11 - 折叠",
                            children: [
                                { name: "叶子节点111" },
                                { name: "叶子节点112" },
                                { name: "叶子节点113" },
                                { name: "叶子节点114" }
                            ]
                        },
                        {
                            name: "父节点12 - 折叠",
                            children: [
                                { name: "叶子节点121" },
                                { name: "叶子节点122" },
                                { name: "叶子节点123" },
                                { name: "叶子节点124" }
                            ]
                        },
                        { name: "父节点13 - 没有子节点", isParent: true }
                    ]
                },
                {
                    name: "父节点2 - 折叠",
                    children: [
                        {
                            name: "父节点21 - 展开", open: true,
                            children: [
                                { name: "叶子节点211" },
                                { name: "叶子节点212" },
                                { name: "叶子节点213" },
                                { name: "叶子节点214" }
                            ]
                        },
                        {
                            name: "父节点22 - 折叠",
                            children: [
                                { name: "叶子节点221" },
                                { name: "叶子节点222" },
                                { name: "叶子节点223" },
                                { name: "叶子节点224" }
                            ]
                        },
                        {
                            name: "父节点23 - 折叠",
                            children: [
                                { name: "叶子节点231" },
                                { name: "叶子节点232" },
                                { name: "叶子节点233" },
                                { name: "叶子节点234" }
                            ]
                        }
                    ]
                },
                { name: "父节点3 - 没有子节点", isParent: true }

            ],
            'init': function () {
                $http({
                    url: "http://localhost:8020/portal/RelatedTrans/GetTree"
                }).success(function (result, header, config, status) {
                    console.log(result)
                    console.log(result.subUsers);

                    $.fn.zTree.init($("#treeDemo"), $scope.treeDemo.setting, $scope.treeDemo.zNodes);
                })
            }
        }
        $scope.treeDemo.init();

    }]);

app.directive('ngThumb', ['$window', function ($window) {
    var helper = {
        support: !!($window.FileReader && $window.CanvasRenderingContext2D),
        isFile: function (item) {
            return angular.isObject(item) && item instanceof $window.File;
        },
        isImage: function (file) {
            var type = '|' + file.type.slice(file.type.lastIndexOf('/') + 1) + '|';
            return '|jpg|png|jpeg|bmp|gif|'.indexOf(type) !== -1;
        }
    };

    return {
        restrict: 'A',
        template: '<canvas/>',
        link: function (scope, element, attributes) {
            if (!helper.support) return;

            var params = scope.$eval(attributes.ngThumb);

            if (!helper.isFile(params.file)) return;
            if (!helper.isImage(params.file)) return;

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
                canvas.attr({ width: width, height: height });
                canvas[0].getContext('2d').drawImage(this, 0, 0, width, height);
            }
        }
    };


}])