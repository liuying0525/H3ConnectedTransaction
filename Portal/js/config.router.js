'use strict';
/**
 * 路由配置
 */
var versionDZ = new Date().getFullYear() + '' + (new Date().getMonth() + 1) + '' + new Date().getDate();
angular.module('app')
    .run(
        ['$rootScope', '$location', '$compile', '$state', '$stateParams',
            function ($rootScope, $location, $compile, $state, $stateParams) {
                $rootScope.$state = $state;
                $rootScope.$stateParams = $stateParams;

                $rootScope.$on("$stateChangeSuccess", function (event, toState, toParams, fromState, fromParams) {
                    //to be used for back button  
                    ////won`t work when page is reloaded.
                    $rootScope.previousState_name = fromState.name;
                    $rootScope.previousState_params = fromParams;


                    ////判断菜单根目录是否是
                    //if (toParams.TopAppCode != fromParams.TopAppCode && fromParams.TopAppCode && toParams.TopAppCode) {
                    //    var u = navigator.userAgent;
                    //    //如果是火狐浏览器 解决火狐浏览器的兼容性问题
                    //    if (u.indexOf('Gecko') > -1 && u.indexOf('KHTML') == -1) {
                    //      //因为火狐浏览器优先读取缓存，所以先刷新页面在打开指定url
                    //        var url = $location.$$absUrl;
                    //        var parm = parseInt(Math.random() * 101100);
                    //        if ($location.$$absUrl.lastIndexOf('?') > -1) {
                    //            url = url + parm;
                    //        } else {
                    //            url = url + "?" + parm;
                    //        }

                    //        window.location.href = url; 
                    //        location.reload();

                    //    } 
                    //    else {
                    //            window.location.reload();                    
                    //    }

                    // }

                });
                $rootScope.back = function () {
                    $state.go($rootScope.previousState_name, $rootScope.previousState_params);
                }
            }
        ]
    )
    .config(
        ['$stateProvider', '$urlRouterProvider',
            function ($stateProvider, $urlRouterProvider) {
                // 默认页面
                $urlRouterProvider.otherwise('/platform/login');
                $stateProvider
                    // 平台页面基类
                    .state('platform', {
                        url: '/platform',
                        abstract: true,
                        template: '<div ui-view class="fade-in-right-big smooth"></div>'
                    })
                    // 登录界面
                    .state('platform.login', {
                        url: '/login',
                        controller: 'LoginController',
                        templateUrl: 'template/login.html?' + versionDZ + "01",
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([

                                        'js/controllers/LoginController.js?' + versionDZ + "01",
                                        'js/directives/app-directive.js?' + versionDZ,

                                    ]);
                                }
                            ]
                        }
                    })
                    // 主页面基类
                    .state('app', {
                        abstract: true,
                        url: '/app/:TopAppCode',
                        templateUrl: 'template/app.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        //'css/bootstrap.css',
                                        //'css/animate.css',
                                        //'css/font-awesome.min.css',
                                        //'css/simple-line-icons.css',
                                        //'css/font.css',
                                        //'css/app.css',
                                        //'css/appExtend.css',
                                        //'vendor/jquery/file/fileinput.min.css',
                                        'js/directives/app-directive.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    // 主页门户面基类
                    .state('appTemplate', {
                        abstract: true,
                        url: '/appTemplate',
                        templateUrl: 'template/appTemplates.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([

                                        'js/directives/app-directive.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //首页-门户
                    .state('home', {
                        url: '/home/:PageId/:Mode',
                        templateUrl: 'template/default.html?' + versionDZ,
                        controller: "DefaultController",
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                        //'WFRes/assets/stylesheets/sheet.css',
                                        'vendor/jquery/jquery-ui/core.js',
                                        'vendor/jquery/jquery-ui/widget.js',
                                    ]).then(function () {
                                        return $ocLazyLoad.load([
                                            'vendor/jquery/jquery-ui/mouse.js',
                                            'vendor/jquery/jquery-ui/draggable.js',
                                            'vendor/jquery/jquery-ui/droppable.js',
                                            'vendor/jquery/jquery-ui/sortable.js',
                                            'vendor/jquery/jqueryui-touch-punch/jquery.ui.touch-punch.min.js',
                                            'js/controllers/DefaultController.js?' + versionDZ,
                                        ]);
                                    });
                                }
                            ]
                        }
                    })

                    //————门户管理————
                    //门户管理—模板管理
                    .state('appTemplate.PortalTemplates', {
                        url: '/PortalTemplates',
                        controller: "PortalTemplatesController",
                        templateUrl: 'template/Templates/PortalTemplates.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/Templates/PortalTemplatesController.js',
                                    //在线代码编辑
                                    'admin/MvcDesigner/Lib/aspx.css',
                                    'admin/MvcDesigner/Lib/codemirror.js',
                                ]).then(function () {
                                    return $ocLazyLoad.load([
                                        'admin/MvcDesigner/Lib/matchbrackets.js',
                                        'admin/MvcDesigner/Lib/xml.js',
                                        'admin/MvcDesigner/Lib/javascript.js',
                                        'admin/MvcDesigner/Lib/css.js',
                                        'admin/MvcDesigner/Lib/htmlmixed.js',
                                        'admin/MvcDesigner/Lib/htmlembedded.js',
                                        'admin/MvcDesigner/Lib/fullscreen.js',
                                        'admin/MvcDesigner/Lib/clike.js'
                                    ]);
                                });
                            }]
                        }
                    })
                    //门户管理—模板管理
                    .state('appTemplate.PortalPageManeger', {
                        url: '/PortalPageManeger',
                        controller: "PortalPageManegerController",
                        templateUrl: 'template/Templates/PortalPageManeger.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/Templates/PortalPageManegerController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })

                    //————流程中心————
                    //待办
                    .state('app.MyUnfinishedWorkItem', {
                        url: '/MyUnfinishedWorkItem',
                        controller: "MyUnfinishedWorkItemController",
                        templateUrl: 'template/ProcessCenter/MyUnfinishedWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyUnfinishedWorkItemController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //待办-分组模式
                    .state('app.MyUnfinishedWorkItemByGroup', {
                        url: '/MyUnfinishedWorkItemByGroup',
                        cache: 'false',
                        controller: "MyUnfinishedWorkItemByGroupController",
                        templateUrl: 'template/ProcessCenter/MyUnfinishedWorkItemByGroup.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyUnfinishedWorkItemByGroupController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //待办-批量审批模式
                    .state('app.MyUnfinishedWorkItemByBatch', {
                        url: '/MyUnfinishedWorkItemByBatch',
                        controller: "MyUnfinishedWorkItemByBatchController",
                        templateUrl: 'template/ProcessCenter/MyUnfinishedWorkItemByBatch.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyUnfinishedWorkItemByBatchController.js?' + versionDZ
                                    ]);
                                }
                            ]
                        }
                    })
                    //WorkItemDetail
                    .state('app.WorkItemDetail', {
                        url: '/WorkItemDetail',
                        controller: "WorkItemDetailController",
                        templateUrl: 'template/ProcessCenter/WorkItemDetail.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load(['js/controllers/ProcessCenter/WorkItemDetailController.js?' + versionDZ]);
                                }
                            ]
                        }
                    })
                    //待阅
                    .state('app.MyUnReadWorkItem', {
                        url: '/MyUnReadWorkItem',
                        controller: "MyUnReadWorkItemController",
                        templateUrl: 'template/ProcessCenter/MyUnReadWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyUnReadWorkItemController.js?' + versionDZ
                                    ]);
                                }
                            ]
                        }
                    })
                    //放款承诺函
                    .state('app.LoanCommitment', {
                        url: '/LoanCommitment',
                        controller: "LoanCommitmentController",
                        templateUrl: 'template/ProcessCenter/LoanCommitment.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/LoanCommitmentController.js?' + versionDZ
                                    ]);
                                }
                            ]
                        }
                    })
                    //发起
                    .state('app.MyWorkflow', {
                        url: '/MyWorkflow',
                        controller: "MyWorkflowController",
                        templateUrl: 'template/ProcessCenter/MyWorkflow.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyWorkflowController.js',
                                        'vendor/jquery/bootstrapTable/bootstrap-table.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    //已办
                    .state('app.MyFinishedWorkItem', {
                        url: '/MyFinishedWorkItem',
                        controller: "MyFinishedWorkItemController",
                        templateUrl: 'template/ProcessCenter/MyFinishedWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyFinishedWorkItemController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //已阅
                    .state('app.MyReadWorkItem', {
                        url: '/MyReadWorkItem',
                        controller: "MyReadWorkItemController",
                        templateUrl: 'template/ProcessCenter/MyReadWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyReadWorkItemController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //我的流程
                    .state('app.MyInstance', {
                        url: '/MyInstance/:SchemaCode/:State',
                        controller: "MyInstanceController",
                        templateUrl: 'template/ProcessCenter/MyInstance.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyInstanceController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    //查询流程实例
                    .state('app.QueryInstance', {
                        url: '/QueryInstance',
                        controller: "QueryInstanceController",
                        templateUrl: 'template/ProcessCenter/QueryInstance.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'vendor/jquery/bootstrap.js',
                                        'js/controllers/ProcessCenter/QueryInstanceController.js?' + versionDZ
                                    ]);
                                }
                            ]
                        }
                    })
                    //查询任务
                    .state('app.QueryParticipantWorkItem', {
                        url: '/QueryParticipantWorkItem',
                        controller: "QueryParticipantWorkItemController",
                        templateUrl: 'template/ProcessCenter/QueryParticipantWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/QueryParticipantWorkItemController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //导出流程数据
                    .state('app.ExportInstanceData', {
                        url: '/ExportInstanceData',
                        controller: "ExportInstanceDataController",
                        templateUrl: 'template/ProcessCenter/ExportInstanceData.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        //jquery.dataTables.min.js、dataTables.bootstrap.js 不能互换加载顺序
                                        'vendor/jquery/datatables/jquery.dataTables.min.js',
                                        'vendor/jquery/datatables/dataTables.bootstrap.css'
                                    ]).then(function () {
                                        return $ocLazyLoad.load([
                                            'vendor/jquery/datatables/dataTables.bootstrap.js',
                                            'js/controllers/ProcessCenter/ExportInstanceDataController.js?' + versionDZ,
                                        ])
                                    });
                                }
                            ]
                        }
                    })
                    //超时的任务
                    .state('app.QueryElapsedWorkItem', {
                        url: '/QueryElapsedWorkItem',
                        controller: "QueryElapsedWorkItemController",
                        templateUrl: 'template/ProcessCenter/QueryElapsedWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/QueryElapsedWorkItemController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //超时的流程
                    .state('app.QueryElapsedInstance', {
                        url: '/QueryElapsedInstance',
                        controller: "QueryElapsedInstanceController",
                        templateUrl: 'template/ProcessCenter/QueryElapsedInstance.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/QueryElapsedInstanceController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //任务委托
                    .state('app.MyAgents', {
                        url: '/MyAgents',
                        controller: "MyAgentsController",
                        templateUrl: 'template/ProcessCenter/MyAgents.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyAgentsController.js?' + versionDZ,
                                        'js/services/notify.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    //常用意见
                    .state('app.MyComments', {
                        url: '/MyComments',
                        controller: "MyCommentsController",
                        templateUrl: 'template/ProcessCenter/MyComments.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MyCommentsController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js'
                                    ]);
                                }
                            ]
                        }
                    })
                    //签章设置
                    .state('app.MySignature', {
                        url: '/MySignature',
                        controller: "MySignatureController",
                        templateUrl: 'template/ProcessCenter/MySignature.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/MySignatureController.js?' + versionDZ,
                                        'vendor/angular/angular-file/ng-file-upload-shim.js',
                                        'vendor/angular/angular-file/ng-file-upload.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    // 报表展示
                    .state('app.ShowReport', {
                        url: '/ShowReport/:ReportCode/:Params',
                        controller: 'ShowReportControler',
                        templateUrl: 'template/ReportCenter/ShowReport.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([

                                    // 'css/H3Report/bootstrap.min.css',
                                    'css/H3Report/bootstrap-datetimepicker/bootstrap-datetimepicker.css',
                                    'css/H3Report/bootstrap-multiselect/bootstrap-multiselect.css',
                                    'css/H3Report/dataTables-bootstrap/dataTables.bootstrap.min.css',
                                    //'css/H3Report/bootstraptable/bootstrap-table.css',
                                    'css/H3Report/autocomplete/jquery.autocomplete.css',

                                    'css/H3Report/SheetUser.css',

                                    'css/H3Report/PageTurn.css',
                                    'css/H3Report/H3-Icon-Tool/style.css',
                                    'css/H3Report/ChartBase.css',
                                    'css/H3Report/DropDownList.css',
                                    'css/H3Report/Reporting/ReportView.css',
                                    'js/controllers/ReportCenter/ShowReportControler.js',
                                ])
                            }]
                        }
                    })
                    //流程状态-发起模式链接
                    .state('WorkflowInfo', {
                        url: '/WorkflowInfo/:InstanceID/:WorkItemID/:WorkflowCode/:WorkflowVersion',
                        controller: 'WorkflowInfoController',
                        templateUrl: 'template/ProcessCenter/WorkflowInfo.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'WFRes/assets/stylesheets/sheet.css',
                                    'js/controllers/ProcessCenter/WorkflowInfoController.js?' + versionDZ,
                                ]);
                            }]
                        }
                    })
                    //流程状态-任务链接
                    .state('InstanceDetail', {
                        url: '/InstanceDetail/:InstanceID/:WorkItemID/:WorkflowCode/:WorkflowVersion',
                        controller: 'InstanceDetailController',
                        templateUrl: 'template/ProcessCenter/InstanceDetail.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                    'WFRes/assets/stylesheets/sheet.css',
                                    'js/directives/app-directive.js',
                                    'js/controllers/ProcessCenter/InstanceDetailController.js?' + versionDZ,
                                ]);
                            }]
                        }
                    })
                    //流程状态-任务链接
                    .state('app.InstanceDetail', {
                        url: '/InstanceDetail/:InstanceID/:WorkItemID/:WorkflowCode/:WorkflowVersion',
                        controller: 'InstanceDetailController',
                        templateUrl: 'template/ProcessCenter/InstanceDetail.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                    //'WFRes/assets/stylesheets/sheet.css',
                                    'js/controllers/ProcessCenter/InstanceDetailController.js?' + versionDZ,
                                ]);
                            }]
                        }
                    })
                    //用户操作日志
                    .state('InstanceUserLog', {
                        url: '/InstanceUserLog/:InstanceID',
                        controller: 'InstanceUserLogController',
                        templateUrl: 'template/ProcessCenter/InstanceUserLog.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ProcessCenter/InstanceDetailController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })

                    /*
                    自定义页面
                    */

                    //应用中心-任务列表
                    .state('app.MyWorkItem', {
                        url: '/MyWorkItem/:SchemaCode/:State/:FunctionCode',
                        controller: 'MyWorkItemController',
                        templateUrl: 'template/AppCenter/MyWorkItem.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/AppCenter/MyWorkItemController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    //车型同步
                    .state('app.CarModelSync', {
                        url: '/CarModelSync',
                        controller: "CarModelSyncController",
                        templateUrl: 'Custom/template/CarModelSync.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'Custom/js/ajaxfileupload.js?' + versionDZ,
                                        'Custom/js/CarModelSyncController.js?' + versionDZ
                                    ]);
                                }
                            ]
                        }
                    })

                    //应用中心-表单
                    .state('app.EditBizObject', {
                        url: '/EditBizObject/:SchemaCode/:SheetCode/:Mode/:FunctionCode',
                        template: "<div></div>",
                        controller: "EditBizObjectController",
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/AppCenter/EditBizObjectController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    //应用中心-抵押城市维护
                    .state('app.dycs', {
                        url: '/dycs',
                        cache: 'false',
                        controller: "",
                        templateUrl: 'dzbiz/dycs.aspx',
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([

                                    ]);
                                }
                            ]
                        }
                    })
                    .state('app.ProposalList', {
                        url: '/ProposalList',
                        controller: 'ProposalItemsController',
                        templateUrl: 'Custom/template/ProposalItems.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'Custom/js/ProposalItemsController.js?' + versionDZ
                                ])
                            }]
                        }
                    })
                    //应用中心-经销商额度明细
                    .state('app.jxsedmx', {
                        url: '/jxsedmx',
                        cache: 'false',
                        controller: "",
                        templateUrl: 'dzbiz/jxsedmx.aspx',
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([

                                    ]);
                                }
                            ]
                        }
                    })
                    //应用中心-查询列表
                    .state('app.BizQueryView', {
                        url: '/BizQueryView/:SchemaCode/:QueryCode/:FunctionCode',
                        controller: 'BizQueryViewController',
                        templateUrl: 'template/AppCenter/BizQueryView.html?t=' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'WFRes/_Scripts/jquery/jquery.lang.js',
                                    'WFRes/_Scripts/bizquery.js',
                                ]).then(function () {
                                    return $ocLazyLoad.load([
                                        'js/controllers/AppCenter/BizQueryViewController.js?' + versionDZ,
                                    ]);
                                });
                            }]
                        }
                    })
                    // 产品管理
                    .state('app.ProductManage', {
                        url: '/ProductManage',
                        controller: 'ProductManageController',
                        templateUrl: 'template/ReportCenter/ProductManage.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/ProductManageController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    // 产品添加
                    .state('app.AddProducts', {
                        url: '/AddProducts/:ProductName',
                        controller: 'AddProductsController',
                        templateUrl: 'template/ReportCenter/AddProducts.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/AddProductsController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    // 规则管理  FB 20181026
                    .state('app.MortgageRulesManage', {
                        url: '/MortgageRulesManage',
                        controller: 'MortgageRulesManageController',
                        templateUrl: 'template/ReportCenter/MortgageRulesManage.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/MortgageRulesManageController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    // 产品添加
                    .state('app.AddMortgageRules', {
                        url: '/AddMortgageRules/:Objectid',
                        controller: 'AddMortgageRulesController',
                        templateUrl: 'template/ReportCenter/AddMortgageRules.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/AddMortgageRulesController.js?' + versionDZ,
                                    'WFRes/_Scripts/MVCSheet/MvcSheetAll.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    //规则列表
                    .state('app.MortgageList', {
                        url: '/MortgageList',
                        controller: 'MortgageListController',
                        templateUrl: 'template/ReportCenter/MortgageList.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/MortgageListeController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    //规则性质列表
                    .state('app.RuleProperty', {
                        url: '/RuleProperty',
                        controller: 'RulePropertyController',
                        templateUrl: 'template/ReportCenter/RuleProperty.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/RulePropertyController.js?' + versionDZ,
                                    'js/services/notify.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    //规则名称列表
                    .state('app.RuleName', {
                        url: '/RuleName/:Objectid',
                        controller: 'RuleNameController',
                        templateUrl: 'template/ReportCenter/RuleName.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/RuleNameController.js?' + versionDZ,
                                    'js/services/notify.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    //规则明细列表
                    .state('app.RuleDetaile', {
                        url: '/RuleDetaile/:Objectid/:Pid',
                        controller: 'RuleDetaileController',
                        templateUrl: 'template/ReportCenter/RuleDetaile.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/RuleDetaileController.js?' + versionDZ,
                                    'js/services/notify.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    .state('app.AddRuleDetail', {
                        url: '/AddRuleDetail/:Objectid/:Cid', //ObjectID:父ID，cid自己的ID
                        controller: 'AddRuleDetaileController',
                        templateUrl: 'template/ReportCenter/AddRuleDetail.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/AddRuleDetailController.js?' + versionDZ,
                                    'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                    'WFRes/assets/stylesheets/sheet.css',
                                    'js/services/notify.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    .state('app.GetScoring', {
                        url: '/GetScoring', //ObjectID:父ID，cid自己的ID
                        controller: 'ScoringController',
                        templateUrl: 'template/ReportCenter/GetScoring.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/ScoringController.js?' + versionDZ,
                                ])
                            }]
                        }
                    })
                    // END  FB 20181026//评分等级
                    .state('app.ScoringRank', {
                        url: '/ScoringRank/:Objectid/:Cid', //ObjectID:父ID，cid自己的ID
                        controller: 'ScoringRankController',
                        templateUrl: 'template/ReportCenter/ScoringRank.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/ScoringRankController.js?' + versionDZ,
                                    'WFRes/_Content/themes/ligerUI/Aqua/css/ligerui-all.css',
                                    'WFRes/assets/stylesheets/sheet.css',
                                    'js/services/notify.js?' + versionDZ
                                ]);
                            }]
                        }
                    })
                    //业务数据采集
                    .state('app.BusinessDataCollect', {
                        url: '/BusinessDataCollect', //ObjectID:父ID，cid自己的ID
                        controller: 'BusinessDataCollectController',
                        templateUrl: 'template/ReportCenter/BusinessDataCollect.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/BusinessDataCollectController.js?' + versionDZ
                                ]);
                            }]
                        }
                    })
                    .state('app.AddYWData', {
                        url: '/AddYWData/:DealerId/:Objectid', //ObjectID:父ID，cid自己的ID
                        controller: 'AddYWDataController',
                        templateUrl: 'template/ReportCenter/AddYWData.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/AddYWDataController.js?' + versionDZ
                                ]);
                            }]
                        }
                    })
                    //服务器日志
                    .state('app.ServerLog', {
                        url: '/ServerLog',
                        controller: "ServerLogController",
                        templateUrl: 'Custom/template/ServerLog.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'Custom/js/ServerLogController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //批量添加快递单号
                    .state('app.BatchExpress', {
                        url: '/BatchExpress', //ObjectID:父ID，cid自己的ID
                        controller: 'BatchExpressController',
                        templateUrl: 'template/ReportCenter/BatchExpress.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/BatchExpressController.js?' + versionDZ
                                ]);
                            }]
                        }
                    })
                    //解压营业执照关联
                    .state('app.DistributorLicense', {
                        url: '/DistributorLicense', //ObjectID:父ID，cid自己的ID
                        controller: 'DistributorLicenseController',
                        templateUrl: 'template/ReportCenter/DistributorLicense.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/DistributorLicenseController.js?' + versionDZ
                                ]);
                            }]
                        }
                    })
                    //担保额度
                    .state('app.GuaranteeAmount', {
                        url: '/GuaranteeAmount', //ObjectID:父ID，cid自己的ID
                        controller: 'GuaranteeAmountController',
                        templateUrl: 'template/ReportCenter/GuaranteeAmount.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad', function ($ocLazyLoad) {
                                return $ocLazyLoad.load([
                                    'js/controllers/ReportCenter/GuaranteeAmountController.js?' + versionDZ
                                ]);
                            }]
                        }
                    })
                    //产品中心-产品查询
                    .state('app.ProductQuery', {
                        url: '/ProductQuery',
                        controller: "ProductQueryController",
                        templateUrl: 'template/ProductCenter/ProductQuery.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProductCenter/ProductQueryController.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //产品中心-产品/车型查询
                    .state('app.ProductCarTypeQuery', {
                        url: '/ProductCarTypeQuery',
                        controller: "ProductCarTypeQueryController",
                        templateUrl: 'template/ProductCenter/ProductCarTypeQuery.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProductCenter/ProductCarTypeQueryController.js?' + versionDZ,
                                        'js/services/notify.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //产品中心-产品/经销商查询
                    .state('app.ProductDealerQuery', {
                        url: '/ProductDealerQuery',
                        controller: "ProductDealerQueryController",
                        templateUrl: 'template/ProductCenter/ProductDealerQuery.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProductCenter/ProductDealerQueryController.js?' + versionDZ,
                                        'js/services/notify.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //产品中心-添加关联
                    .state('app.ProductDealerAddRelation', {
                        url: '/ProductDealerAddRelation',
                        controller: "ProductDealerAddRelationController",
                        templateUrl: 'template/ProductCenter/ProductDealerAddRelation.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProductCenter/ProductDealerAddRelationController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js',
                                        'vendor/jquery/multiselect/multiselect.min.js',
                                        'vendor/jquery/multiselect/multiselect.css',
                                        //'vendor/jquery/multiselect/prettify.js',
                                        'js/services/notify.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //产品中心-解除关联
                    .state('app.ProductDealerRemoveRelation', {
                        url: '/ProductDealerRemoveRelation',
                        controller: "ProductDealerRemoveRelationController",
                        templateUrl: 'template/ProductCenter/ProductDealerRemoveRelation.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProductCenter/ProductDealerRemoveRelationController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js',
                                        'vendor/jquery/multiselect/multiselect.min.js',
                                        'vendor/jquery/multiselect/multiselect.css',
                                        //'vendor/jquery/multiselect/prettify.js',
                                        'js/services/notify.js?' + versionDZ,
                                    ]);
                                }
                            ]
                        }
                    })
                    //电话录音记录
                    .state('app.CallRecords', {
                        url: '/CallRecords',
                        controller: "CallRecordsController",
                        templateUrl: 'template/CallCenter/CallRecords.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/CallCenter/CallRecordsController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js',
                                        'vendor/jquery/select2/select2.css',
                                        'vendor/jquery/select2/select2-bootstrap.css',
                                        'vendor/jquery/select2/select2.min.js',
                                        'vendor/jquery/jsrender/jsrender.js',
                                        'WFRes/layer/layer.js',
                                        'js/services/questiontype.js?001',
                                        'css/font-awesome.min.css',
                                        'css/build.css'
                                    ]);
                                }
                            ]
                        }
                    })
                    //关联交易管理-关联方管理
                    .state('app.ManageTransaction', {
                        url: '/ManageTransaction',
                        controller: "ManageTransactionController",
                        templateUrl: 'template/ProcessCenter/ManageTransaction.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'vendor/jquery/zTree_v3/css/zTreeStyle/zTreeStyle.css',
                                        'css/H3Report/ManageSheet.css',
                                        'vendor/jquery/zTree_v3/js/jquery.ztree.core.js',
                                        'vendor/jquery/angular-file-upload/es5-sham.min.js',
                                        'vendor/jquery/angular-file-upload/es5-shim.min.js',
                                        'vendor/jquery/angular-file-upload/console-sham.min.js',
                                        'vendor/jquery/angular-file-upload/angular-file-upload.min.js',
                                        'vendor/jquery/chosen/chosen.css',
                                        'vendor/jquery/chosen/chosen.jquery.min.js',
                                        'vendor/jquery/chosen/angular-chosen.min.js',

                                        'js/controllers/ProcessCenter/ManageTransactionController.js?' + versionDZ,
                                        'js/services/notify.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    .state('app.ManageSearch', {
                        url: '/ManageSearch',
                        controller: "ManageSearchController",
                        templateUrl: 'template/ProcessCenter/ManageSearch.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/ManageSearchController.js?' + versionDZ,
                                        'js/services/notify.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    //关联交易管理-关联交易管理
                    .state('app.ManageRelatedparty', {
                        url: '/ManageRelatedparty',
                        controller: "ManageRelatedpartyController",
                        templateUrl: 'template/ProcessCenter/ManageRelatedparty.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'css/H3Report/ManageSheet.css',
                                        'js/controllers/ProcessCenter/ManageRelatedpartyController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js',
                                        'js/services/notify.js',
                                    ]);
                                }
                            ]
                        }
                    })
                    //关联交易管理-公司指数维护
                    .state('app.ManageMaintenance', {
                        url: '/ManageMaintenance',
                        controller: "ManageMaintenanceController",
                        templateUrl: 'template/ProcessCenter/ManageMaintenance.html?' + versionDZ,
                        resolve: {
                            deps: ['$ocLazyLoad',
                                function ($ocLazyLoad) {
                                    return $ocLazyLoad.load([
                                        'js/controllers/ProcessCenter/ManageMaintenanceController.js?' + versionDZ,
                                        'vendor/jquery/bootstrap.js',
                                        'css/H3Report/ManageSheet.css',
                                    ]);
                                }
                            ]
                        }
                    })
                // End
            }
        ]
    );
