/*
    factory
*/
var app =
angular.module('app')
    //注册http响应超时拦截器
        .factory('httpInterceptor', ['$rootScope', '$q', '$injector', '$location', function ($rootScope, $q, $injector, $location) {
       var httpInterceptor = {
           //request: function (config, $rootScope,$location, $routeParams) {// 19.7.1 wangxg 在系统自带的http请求之前，在URL参数中添加FunctionCode参数
           //    var functionCode = "";
           //    var seg = document.URL.split("#");
           //    if (seg.length > 1) {
           //        var relUrl = seg[1];
           //        var tem = relUrl.split("/");
           //        if (tem.length > 3) {
           //            functionCode = tem[3];
           //        }
           //    }
           //    //在请求前给修改url（增加一个功能代码参数，用于后台权限确认）
           //    config.url += config.url.match(/\?/) ? "&" : "?";
           //    config.url += "FunctionCode=" + functionCode;
           //    return config || $q.when(config);
           //},
           'response': function (response) {
               //if (response.data && response.data.ExceptionCode == 1) {
               //    debugger;
               //    var url = window.location.href;
               //    var rootScope = $injector.get('$rootScope');
               //    window.localStorage.setItem("H3.redirectUrl", url);
               //    rootScope.$state.go("platform.login");
               //    return $q.reject(response);
               //}
               return response;
           }
       }
       return httpInterceptor;
   }]);