app.controller('ProductDealerAddRelationController', ['$scope', "$rootScope", "$translate", "$timeout", "$compile",
    "$http", "$state", "$stateParams",
    "$filter", "ControllerConfig", "$interval", "jq.datables",
    function ($scope, $rootScope, $translate, $timeout, $compile, $http, $state, $stateParams, $filter, ControllerConfig, $interval, jqdatables) {

        $scope.products = [];

        $scope.dealerName = "";

        //$scope.dealers = [
        //    { BusinessPartnerId: "151", CompanyName: "湖北鼎杰汽车销售服务有限公司" },
        //    { BusinessPartnerId: "152", CompanyName: "湖北博诚汽车销售服务有限公司" },
        //    { BusinessPartnerId: "153", CompanyName: "武汉开泰汽车销售服务有限公司" },
        //    { BusinessPartnerId: "154", CompanyName: "内蒙古鼎杰汽车贸易有限公司" },
        //    { BusinessPartnerId: "155", CompanyName: "湖北欣瑞汽车销售服务有限公司" },
        //    { BusinessPartnerId: "156", CompanyName: "武汉宝泽汽车销售服务有限公司" }
        //];

        $scope.dealers = [];



        //进入视图触发
        $scope.$on('$viewContentLoaded', function (event) {
            $('#multi_dealer').multiselect({
                right: '#multi_dealer_to',
                rightSelected: '#multi_d_rightSelected',
                leftSelected: '#multi_d_leftSelected',
                rightAll: '#multi_d_rightAll',
                leftAll: '#multi_d_leftAll',

                //search: {
                //    left: '<input type="text" name="q" class="form-control" placeholder="请输入经销商名称" />'
                //},

                moveToRight: function (Multiselect, $options, event, silent, skipStack) {
                    var button = $(event.currentTarget).attr('id');

                    if (button == 'multi_d_rightSelected') {
                        var $left_options = Multiselect.$left.find('> option:selected');
                        Multiselect.$right.eq(0).append($left_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$right.eq(0).find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$right.eq(0));
                        }
                    } else if (button == 'multi_d_rightAll') {
                        var $left_options = Multiselect.$left.children(':visible');
                        Multiselect.$right.eq(0).append($left_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$right.eq(0).find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$right.eq(0));
                        }
                    }
                },

                moveToLeft: function (Multiselect, $options, event, silent, skipStack) {
                    var button = $(event.currentTarget).attr('id');

                    if (button == 'multi_d_leftSelected') {
                        var $right_options = Multiselect.$right.eq(0).find('> option:selected');
                        Multiselect.$left.append($right_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$left.find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$left);
                        }
                    } else if (button == 'multi_d_leftAll') {
                        var $right_options = Multiselect.$right.eq(0).children(':visible');
                        Multiselect.$left.append($right_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$left.find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$left);
                        }
                    }
                }
            });

            $('#multi_product').multiselect({
                right: '#multi_product_to',
                rightSelected: '#multi_d_rightSelected_product',
                leftSelected: '#multi_d_leftSelected_product',
                rightAll: '#multi_d_rightAll_product',
                leftAll: '#multi_d_leftAll_product',

                search: {
                    left: '<input type="text" name="search" class="form-control" placeholder="请输入产品名称" />'
                },

                moveToRight: function (Multiselect, $options, event, silent, skipStack) {
                    var button = $(event.currentTarget).attr('id');

                    if (button == 'multi_d_rightSelected_product') {
                        var $left_options = Multiselect.$left.find('> option:selected');
                        Multiselect.$right.eq(0).append($left_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$right.eq(0).find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$right.eq(0));
                        }
                    } else if (button == 'multi_d_rightAll_product') {
                        var $left_options = Multiselect.$left.children(':visible');
                        Multiselect.$right.eq(0).append($left_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$right.eq(0).find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$right.eq(0));
                        }
                    }
                },

                moveToLeft: function (Multiselect, $options, event, silent, skipStack) {
                    var button = $(event.currentTarget).attr('id');

                    if (button == 'multi_d_leftSelected_product') {
                        var $right_options = Multiselect.$right.eq(0).find('> option:selected');
                        Multiselect.$left.append($right_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$left.find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$left);
                        }
                    } else if (button == 'multi_d_leftAll_product') {
                        var $right_options = Multiselect.$right.eq(0).children(':visible');
                        Multiselect.$left.append($right_options);

                        if (typeof Multiselect.callbacks.sort == 'function' && !silent) {
                            Multiselect.$left.find('> option').sort(Multiselect.callbacks.sort).appendTo(Multiselect.$left);
                        }
                    }
                }
            });

            $scope.init();
        });

        $scope.init = function () {

            $http({
                method: 'get',
                url: '/Portal/ProductCenter/AllProductsQuery',
            }).success(function (result) {
                if (result == "" || result == null) {
                    $.notify({ message: "未查询到产品数据", status: "danger" });
                } else {
                    $scope.products = JSON.parse(result);
                    console.log($scope.products.length);
                }
            });
        }

        $scope.searchDealers = function(){
            $http({
                method: 'get',
                url: '/Portal/ProductCenter/AllDealersQuery?dealerName=' + $scope.dealerName,
            }).success(function (result) {
                if (result == "" || result == null) {
                    $.notify({ message: "未查询到经销商", status: "danger" });
                } else {
                    $scope.dealers = JSON.parse(result);
                    console.log($scope.dealers.length);
                }
            })
        }

        $scope.addRelation = function () {

            var products = [];
            var dealers = [];
            $("#multi_product_to").find("option").each(function (index, value) {
                products.push({ ProductId: $(value).val(), ProductName: $(value).text() });
            });
            $("#multi_dealer_to").find("option").each(function (index, value) {
                var subIndex = $(value).text().indexOf('(');

                var dealerName = subIndex < 0 ? $(value).text() : $(value).text().substring(0, subIndex);

                dealers.push({ DealerCode: $(value).val(), DealerName: dealerName });
            });

            console.log(products);
            console.log(dealers);

            $http({
                method: 'post',
                url: '/Portal/ProductCenter/AddRelation',
                data: JSON.stringify({ Products: products, Dealers: dealers}),
            }).success(function (respon) {
                if (respon.Code != 1) {
                    $.notify({ message: "添加关联成功", status: "success" });
                }
                else {
                    $.notify({ message: "添加关联失败", status: "danger" });
                }

            });

            
        }

    }]);