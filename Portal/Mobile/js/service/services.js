var rootpath='';

var services = angular.module('starter.services', [])
  //获取代办和待阅的数目
  .factory('GetWorkItemCount',function(httpService){
    return {
      all:function(options){
        return httpService.get(rootpath+'/portal/Mobile/GetWorkItemCount',options);
      }
    };
  })
  //登录
  .factory('login', function (httpService) {
    return {
      browser: function (options) {
        //浏览器
        return httpService.post(rootpath+'/portal/Organization/LoginIn',options);
      },
      app:function(options){
        //跨域登录
      }
    };
  })


  .factory('logout', function (httpService) {
      return {
          browser: function (options) {
              //浏览器
              return httpService.get(rootpath + '/portal/Organization/LoginOut', options);
          },
          app: function (options) {
              //跨域登录
          }
      };
  })

  //待办数据请求
  .factory('UnfinishedWorkItems', function (httpService) {
    return {
        all: function (options) {
            return httpService.get(rootpath+'/portal/Mobile/GetWorkItems',options);
        }
    };
  })
  //已办数据请求
  .factory('finishedworkitems', function (httpService) {
    return {
        all: function (options) {
            return httpService.get(rootpath+'/portal/Mobile/GetWorkItems',options);
        }
    };
  })
  //待阅数据请求
  .factory('Unreadedworkitems', function (httpService) {
    return {
        all: function (options) {
            return httpService.get(rootpath+'/portal/Mobile/LoadCirculateItems',options);
        },
        remove:function(options){
               return httpService.get(rootpath+'/portal/Mobile/ReadCirculateItems',options);
        }
    };
  })
  //已阅数据请求
  .factory('Readedworkitems', function (httpService) {
    return {
      all: function (options) {
        return httpService.get(rootpath+'/portal/Mobile/LoadCirculateItems',options);
      }
    };
  })
//发起流程全部数据
 .factory('categories', function (httpService) {
        var path = '';
        return {
            all: function (params) {
                return httpService.get(rootpath+'/portal/Mobile/LoadWorkflows',params);
            },
            refreshWorkItems:function(params){
                 return httpService.get('js/datas/startworkflow.json',params);
            },
            setFavorite:function(url,params){
                 return httpService.post(rootpath+'/portal'+url,params);
            },

        };
    })
//我的流程
 .factory('instances', function (httpService) {
        var path = '';
        return {
            all: function (url,params) {
                return httpService.get(url,params);
            },
            refreshWorkItems:function(url,params){
                 return httpService.get(url,params);
            },
            loadWorkItems:function(params){
                 return httpService.get('js/datas/myinstancefresh.json',params);
            },

        };
    });
