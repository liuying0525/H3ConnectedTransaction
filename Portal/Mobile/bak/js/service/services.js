var rootpath = '';

var services = angular.module('starter.services', [])
  //获取代办和待阅的数目
  .factory('GetWorkItemCount', function (httpService) {
      return httpService.get(rootpath + '/portal/Mobile/GetWorkItemCount', {});
  })
  //登录
  .factory('login', function (httpService) {
      var path = '';
      return {
          browser: function (options) {
              //浏览器
              return httpService.get(rootpath + '/portal/Organization/LoginIn', options);
          },
          app: function (options) {
              //跨域登录
          }
      };
  })

    //待办数据请求
    .factory('UnfinishedWorkItems', function (httpService) {
        var path = rootpath + "/Mobile/GetWorkItems";
        return {
            all: function (params) {
                return httpService.get('js/datas/unfinishedworkitems.json', {});
                //return httpService.get(path, params);
            },
            refreshWorkItems: function (params) {
                return httpService.get('js/datas/unreadworkitem.json', params);
            },
            loadWorkItems: function (params) {
                return httpService.get('js/datas/unfinishedworkitems.json', params);
            }
        };
    })
     //已办数据请求
    .factory('finishedworkitems', function (httpService) {
        var path = '';
        return {
            all: function () {
                return httpService.get('js/datas/finishedworkitem.json', {});
            },
            refreshWorkItems: function (params) {
                return httpService.get('js/datas/finishedworkitem.json', params);
            },
            loadWorkItems: function (params) {
                return httpService.get('js/datas/finishedworkitem.json', params);
            }
        };
    })
 //待阅数据请求
 .factory('Unreadworkitems', function (httpService) {
     var path = '';
     return {
         all: function () {
             return httpService.get('js/datas/unreadworkitem.json', {});
         },
         refreshWorkItems: function (params) {
             return httpService.get('js/datas/unfinishedworkitems.json', params);
         },
         loadWorkItems: function (params) {
             return httpService.get('js/datas/unreadworkitem.json', params);
         },
         remove: function (params) {
             return httpService.post('js/datas/unreadworkitem.json', params);
         }
     };
 })
//发起流程全部数据
 .factory('categories', function (httpService) {
     var path = '';
     return {
         all: function () {
             return httpService.get('js/datas/startworkflow.json', {});
         },
         refreshWorkItems: function (params) {
             return httpService.get('js/datas/startworkflow.json', params);
         },
         loadWorkItems: function (params) {
             return httpService.get('js/datas/startworkflow.json', params);
         },

     };
 })
//我的流程
 .factory('instances', function (httpService) {
     var path = '';
     return {
         all: function () {
             return httpService.get('js/datas/myinstance.json', {});
         },
         refreshWorkItems: function (params) {
             return httpService.get('js/datas/myinstancefresh.json', params);
         },
         loadWorkItems: function (params) {
             return httpService.get('js/datas/myinstancefresh.json', params);
         },

     };
 })

