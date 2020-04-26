app.controller('AddRuleDetaileController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$modal", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $modal, $state, $stateParams, $filter, ControllerConfig) {
        $scope.Province = [];
       
        $scope.Pid = $stateParams.Objectid;      //上级ID
        $scope.Cid = $stateParams.Cid;             
        $scope.Datas = [];
        $scope.title = "";
        $scope.init = function () {
            Cid = "";
            var id = $scope.Pid;
            Cid = $scope.Cid;
            if (Cid == null || Cid == "") {
                $http({
                    method: 'post',
                    url: '/Portal/RuleDetaile/Index?Pid=' + id + '&GetState=1',//wangxg 19.7
                    //url: '/Portal/Ajax/RuleDetaile.ashx?Pid=' + id + '&GetState=1',
                }).success(function (req) {
                    $scope.RuleName = req;

                }).error(function (data, header, config, status) {// 19.7 
                    showAgErr(data, header);
                });
            }
            else {
              
                $http({
                    method: 'post',
                    url: '/Portal/RuleDetaile/Index?Cid=' + Cid + '&Pid=' + id,//wangxg 19.7
                    //url: '/Portal/Ajax/RuleDetaile.ashx?Cid=' + Cid + '&Pid=' + id,
                }).success(function (req) {
                    $scope.RuleName = req.RuleName;
                    $scope.DetaileName = req.DetaileName;
                    $scope.Rate = req.Rate;
                    $scope.Internettype = req.Internettype;
                    $scope.Classname = req.Classname;
                    $scope.State = req.State;
                    $scope.Datas = req.datas;
                    $scope.title = "规则明细";
                    if ($scope.State == 1) {
                        var obj = document.getElementsByClassName('check');
                        $('.check').attr("disabled", "disabled");
                        console.log($('.check'));
                       
                    }
                    })
                    .error(function (data, header, config, status) {// 19.7 
                        showAgErr(data, header);
                    });
            }
        }

        $scope.cancel = function () {
            $rootScope.back();
        }
        $scope.AddRow = function () {
           
            var modalInstance = $modal.open({
                templateUrl: 'AddRuleDetaile.html',    // 指向上面创建的视图
                controller: 'EditRuleNameController1',// 初始化模态范围
                size: "md",
                resolve: {
                    params: function () {
                        return {
                            Type: "",
                            Value: "",
                            Detail: "",
                            GetValue: "",
                            MinValue: "",
                            MaxValue:"",
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
            modalInstance.result.then(function (result) {
                if (result.data.length > 0) {
                    $scope.Datas.push({
                        Score: result.data[0].Score,
                        Memo: result.data[0].memo,
                    })
                }          
            });


        }
        $scope.DelRow = function (index) {
            $scope.Datas.splice(index,1);
           
        }
        $scope.save = function () {


            if ($scope.DetaileName == "" || $scope.DetaileName == null || $scope.DetaileName == undefined) {
                alert("请填写明细名称");
                return;
            }
            if ($scope.Rate == "" || $scope.Rate == null || $scope.Rate == undefined) {
                alert("请填写权重");
                return;
            }
            if (isNaN($scope.Rate)) {
                alert("权重只能为数字");
                return;
            }
                var products = [];
                var cid = "";
                var ObjectID = $scope.Pid
                Cid = $scope.Cid;
                for (var i = 0; i < $scope.Datas.length; i++) {
                    products.push({
                        Score: $scope.Datas[i]["Score"],
                        Memo: $scope.Datas[i]["Memo"],
                    })
                }
                var data = {
                    ObjectID: ObjectID,
                    Cid: Cid,
                    RuleName: $scope.RuleName,
                    DetaileName: $scope.DetaileName,
                    Rate: $scope.Rate,
                    Products: products
                }
                $http({
                    method: 'post',
                    //url: '/Portal/Ajax/AddRuleDetaile.ashx',
                    url: '/Portal/AddRuleDetaile/Index',// wangxg 19.7
                    data: data,
                }).success(function (req) {
                    if (req == "Success") {
                        alert("保存成功");
                        $rootScope.back();
                    }
                })
                    .error(function (data, header, config, status) {// 19.7 
                        showAgErr(data, header);
                    });
          

        }
        
        $scope.title = "规则明细新增";
        if ($scope.ObjectID != "") {
            $scope.init();
            $scope.title = "规则明细修改";
        }

        
    }]);

app.controller("EditRuleNameController1", ["$scope", "$rootScope", "$http", "$translate", "$state", "$filter", "$modal", "$modalInstance", "$interval", "$timeout", "ControllerConfig", "notify", "datecalculation", "params",
    function ($scope, $rootScope, $http, $translate, $state, $filter, $modal, $modalInstance, $interval, $timeout, ControllerConfig, notify, datecalculation, params) {
        $scope.Type ="";
        $scope.Value = "";
        $scope.Detail = "";
        $scope.GetValue = "";
        $scope.MinValue = "";
        $scope.MaxValue = "";
        $scope.getLanguage = function () {
            $scope.LanJson = {
              
               
            }
        }
        $scope.getLanguage();
        // 获取语言
        $rootScope.$on('$translateChangeEnd', function () {
            $scope.getLanguage();
            $state.go($state.$current.self.name, {}, { reload: true });
        });

        $scope.ChooseData = []
        $scope.TotalChooseData = []
       
        $scope.init = function () {

            //state = 1;
            $scope.Type = params.Type;
            $scope.Value = params.Value;
            $scope.Detail = params.Detail;
            $scope.GetValue = params.GetValue;
            $scope.MinValue = params.MinValue;
            $scope.MaxValue = params.MaxValue;
            //$http.get('/Portal/ajax/GetDetailVarList.ashx').then(function (d) {
            $http.get('/Portal/GetDetailVarList/Index').then(function (d) {
                console.log(d.data);
                $scope.TotalChooseData = d.data;
                $scope.searchData();
            });
        }
        $scope.init();

        $scope.searchData = function () {
            var data = [];
            for (var i = 0; i < $scope.TotalChooseData.length; i++) {
                if ($scope.Detail == undefined || $scope.Detail == null || $scope.Detail == '' || $scope.TotalChooseData[i].a.indexOf($scope.Detail) >= 0) {
                    data.push($scope.TotalChooseData[i]);
                }
            }
            $scope.ChooseData = data;
        }

        $scope.setValue = function (a) {
            $scope.Detail = a;
            $scope.searchData()
        }

        $scope.ok = function () {
            var str = "";
            var Detail = "";
            if ($scope.Value == "" || $scope.Value == null || $scope.Value == undefined) {
                alert("请填写分值");
                return;
            }
            if ($scope.Detail == "" || $scope.Detail == null || $scope.Detail == undefined) {
                var value = $('.editVar').val()
                if (value == "" || value == null || value == undefined) {
                    alert("请填写明细变量");
                    return;
                } else {
                    $scope.Detail = value
                }
            }
            if ($scope.Type == '范围') {
                if ($scope.MinValue == "" || $scope.MaxValue == "") {
                    alert("请填写完整匹配区间");
                    return;
                }
                if (!($scope.MinValue == '?' || $scope.MinValue.indexOf('.') > -1)) {
                    alert("匹配区间填写不合法");
                    return;
                }
                if (!($scope.MaxValue == '?' || $scope.MaxValue.indexOf('.') > -1)) {
                    alert("匹配区间填写不合法");
                    return;
                }
                str = "范围" + "[" + $scope.Detail + ":" + $scope.MinValue + "-" + $scope.MaxValue + ")";
            }
            else if ($scope.Type == '文本') {
                if ($scope.GetValue == "" || $scope.GetValue == undefined) {
                    alert("请填写取值");
                    return;
                }
                str = "文本" + "(" + $scope.Detail + ":" + $scope.GetValue + ")";
            }
            else if ($scope.Type == '存在' || $scope.Type == '不存在') {
                str = $scope.Type + "(" + $scope.Detail + ")";
            }
            else {
                alert("请选择类型");
                return;
            }
            if (isNaN($scope.Value)) {
                alert("分值只能为数字");
                return;
            }
          
           
            var data = [];
            data.push({
                Score: $scope.Value,
                memo: str
            });
            $modalInstance.close({ "data": data });
           
          
        };
        $scope.change = function (ret) {
            if (ret == '?' || !isNaN(ret)) {
                return true;
            }
            else {
                alert("匹配区间填写不合法")
                return false;
            }
                
        }
        $scope.format = function(num,type) {
            var s = Number(num);
            if (/[^0-9\.]/.test(s))
                s = 0;
            if (s == null || s == "")
                s = 0;
            if (typeof (type) == 'undefined') {
                type = 4
            }
            if (s == 0)
            {
                if (type == 0)
                { return s; }
                s = s.toString() + '.'; for (var i = 0; i < type; i++) { s += '0'; }
                return s;
            }
            s = s.toFixed(type);
            s = s.toString().replace(/^(\d*)$/, "$1.");
            s = s.replace(/(\d*\.\d*)\d*/, "$1");
            s = s.replace(".", ",");
            var re = /(\d)(\d{3},)/;
            while (re.test(s))
                s = s.replace(re, "$1,$2");
            s = s.replace(/,(\d*)$/, ".$1");
            if (type == 0)
            {// 不带小数位(默认是有小数位)            
                var a = s.split(".");
                s = a[0];
            }
            $scope.MaxValue= s;
        }
        $scope.formatMin = function (num, type) {
            var s = Number(num);
            if (/[^0-9\.]/.test(s))
                s = 0;
            if (s == null || s == "")
                s = 0;
            if (typeof (type) == 'undefined') {
                type = 4
            }
            if (s == 0) {
                if (type == 0) { return s; }
                s = s.toString() + '.'; for (var i = 0; i < type; i++) { s += '0'; }
                return s;
            }
            s = s.toFixed(type);
            s = s.toString().replace(/^(\d*)$/, "$1.");
            s = s.replace(/(\d*\.\d*)\d*/, "$1");
            s = s.replace(".", ",");
            var re = /(\d)(\d{3},)/;
            while (re.test(s))
                s = s.replace(re, "$1,$2");
            s = s.replace(/,(\d*)$/, ".$1");
            if (type == 0) {// 不带小数位(默认是有小数位)            
                var a = s.split(".");
                s = a[0];
            }
            $scope.MinValue = s;
        }
    
        $scope.cancel = function () {
            $modalInstance.dismiss('cancel'); // 退出
        }
    }])