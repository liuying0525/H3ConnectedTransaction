app.controller('MortgageRulesManageController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig) {

        $scope.Datas = [];
        $scope.Total = 0;
        $scope.TotalPage = 1;
        $scope.Page = 1;
        $scope.Min = true;
        $scope.Max = true;
        $scope.Size = 10;
        $scope.IsBase = false;
        $scope.checkAll = false;
        $scope.Province = [];
        $scope.City = [];
        $scope.SearchKey = {
            Shop: '',
            Province: '',
            City: '',
            SpName: '',
            DyName: '',
        };

        $scope.$on('$viewContentLoaded', function (event) {
          
            $scope.init();
        });

        $scope.init = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
            $scope.getProvince();
        }

        $scope.clickSearch = function () {
            $scope.Page = 1;
            $scope.search($scope.Page);
        }

        $scope.search = function (page) {
            $http({
                method: 'post',
                url: '/Portal/GetMortageRules/Index?method=Data&Page=' + (page - 1) + '&Size=' + $scope.Size + '&Shop=' + $scope.SearchKey.Shop + '&Province=' + $scope.SearchKey.Province + '&City=' + $scope.SearchKey.City + '&SpName=' + $scope.SearchKey.SpName + '&DyName=' + $scope.SearchKey.DyName,//wangxg 19.7
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=Data&Page=' + (page - 1) + '&Size=' + $scope.Size + '&Shop=' + $scope.SearchKey.Shop + '&Province=' + $scope.SearchKey.Province + '&City=' + $scope.SearchKey.City + '&SpName=' + $scope.SearchKey.SpName + '&DyName=' + $scope.SearchKey.DyName,
                data: { page: page, size: $scope.Size }
            }).success(function (req) {

                $scope.Datas = req.Data;
                $scope.Total = req.Total;
                var totalPage = parseInt(parseInt(req.Total) / $scope.Size) + (parseInt(req.Total) % $scope.Size > 0 ? 1 : 0);
                $scope.TotalPage = totalPage <= 0 ? 1 : totalPage;
                $scope.SetPageButton();
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }

        $scope.SetPageButton = function () {
            $scope.Min = $scope.Page == 1;
            $scope.Max = $scope.Page == $scope.TotalPage;
        }

        $scope.CheckMax = function () {
            if ((parseInt($scope.Page) > 0) == false) $scope.Page = 1;
            if (parseInt($scope.Page) > $scope.TotalPage)
                $scope.Page = $scope.TotalPage;
            $scope.search($scope.Page);
        }

        $scope.ModPage = function (Value) {
            $scope.Page = parseInt($scope.Page) + parseInt(Value);
            $scope.search($scope.Page);
        }

        $scope.selectAll = function () {
            if ($scope.checkAll) {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    $scope.Datas[i].checked = true;
                }
            } else {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    $scope.Datas[i].checked = false;
                }
            }
        }

        $scope.select = function (index) {
            console.log(index, $scope.Datas[index].checked)
            if ($scope.Datas[index].checked == true) {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    if ($scope.Datas[i].checked == false) {
                        $scope.checkAll = false;
                        return;
                    }
                }
                $scope.checkAll = true;
            } else {
                $scope.checkAll = false;
            }
        }

        $scope.Delete = function (index) {
            var deleteArray = [];
            if (index == undefined || index == null) {
                for (var i = 0; i < $scope.Datas.length; i++) {
                    if ($scope.Datas[i].checked) {
                        deleteArray.push({
                            ObjectID: $scope.Datas[i].ObjectID,
                         
                        })
                    }
                }
                if (deleteArray.length <= 0) {
                    alert('您未选择任何抵押规则。');
                    return;
                }
                else if (confirm('是否删除选中的' + deleteArray.length + '项产品？执行该操作后无法恢复，请谨慎处理。')) {
                } else {
                    return;
                }
            } else {
                if (confirm('是否删除办理店【' + $scope.Datas[index].Shop + '】的抵押规则？执行该操作后无法恢复，请谨慎处理。')) {
                    deleteArray.push({
                        ObjectID: $scope.Datas[index].ObjectID,
                      
                    })
                } else {
                    return;
                }
            }

            $http({
                method: 'post',
                url: '/Portal/DelMortageRules/Index',//wangxg 19.7
                //url: '/Portal/Ajax/DelMortageRules.ashx',
                data: deleteArray
            }).success(function () {
                alert('删除成功。');
                $scope.search($scope.Page);
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }

        $scope.clear = function () {
            $scope.SearchKey = {
                ProductName: '',
                ProductAlias: '',
                Description: '',
                State: '',
                Dealer: ''
            };
            $scope.clickSearch();
        }

        $scope.New = function (index) {
            var Objectid = "";
            if (index != undefined && index != null && index + "" != "") {
                Objectid = $scope.Datas[index].ObjectID;
            }
            $state.go('app.AddMortgageRules', { "Objectid" : Objectid})
        }

        $scope.getProvince = function () {
            $http({
                method: 'post',
                url: '/Portal/GetMortageRules/Index?method=GetProvince',//wangxg 19.7
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=GetProvince',
            }).success(function (req) {
                $scope.Province = req;
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        }
        $scope.getCity = function (id) {
            $http({
                method: 'post',
                //url: '/Portal/Ajax/GetMortageRules.ashx?method=GetCity' + '&id=' + id
                url: '/Portal/GetMortageRules/Index?method=GetCity' + '&id=' + id
            }).success(function (req) {
                $scope.City = req;
            }).error(function (data, header, config, status) {// 19.7 
                showAgErr(data, header);
            });
        };


        var _PORTALROOT_GLOBAL = window.localStorage.getItem("H3.PortalRoot");
        $scope.DownloadTemp = function () {
           
            if (!_PORTALROOT_GLOBAL) {
                $.ajax({
                    url: "../Index/GetPortalRoot",
                    type: "GET",
                    cache: false,
                    async: false,//同步执行
                    dataType: "JSON",
                    success: function (data) {
                        if (typeof (data.Extend) != "undefined") {
                            _PORTALROOT_GLOBAL = data.Extend;
                        }
                    },
                    error: function (msg) {// 19.7 
                        showJqErr(msg);
                    }
                });
            }
            var url = _PORTALROOT_GLOBAL + "/WFRes/_ExclTemplate/MortRuleTemp.xls";
            window.open(url);
        }

        //打开自定义模态框
        $("#uploadfile").click(function (e) {

        })


        $scope.uploadMortfile = function () {
            var formData = new FormData();
            formData.append("file", document.getElementById("uploadrulefile").files[0]);

            $.ajax({
                type: "POST",
                url: "/Portal/SetMortgageRules/Index",//wangxg 19.7
                //url: "/Portal/ajax/SetMortgageRules.ashx",
                data: formData,
                processData: false,
                contentType: false,
                success: function (result) {
                    var str = JSON.parse(result);
                    if (str.Result == "-1") {
                        alert(str.Message);
                    }
                    $(".close").click();
                    document.getElementById("uploadrulefile").outerHTML = document.getElementById("uploadrulefile").outerHTML;
                    //document.getElementById('uploadrulefile') && document.getElementById('uploadrulefile').reset();
                    console.log(str.Message);
                    location.reload();
                },
                error: function (msg) {// 19.7 
                    showJqErr(msg);
                }
            })
        }
        
    }]);