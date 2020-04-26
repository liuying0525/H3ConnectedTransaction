var module = angular.module('starter.controllers', [])
  .controller("mainCtrl", function ($rootScope, $scope, $state) {
      console.log("主程序启动....");
      // 设置信息
      $scope.setting = JSON.parse(window.localStorage.getItem("OThinker.H3.Mobile.Setting")) ||
          {
              autoLogin: config.defaultAutoLogin,            // 是否自动登录
              serviceUrl: config.defaultServiceUrl,          // 服务地址
              httpUrl: "",                            //http请求地址
              workItemUrl: "",                        // 打开待办的URL地址
              startInstanceUrl: "",                   // 发起流程的链接
              instanceSheetUrl: "",                   // 打开在办流程的链接
              uploadImageUrl: "",                     // 图片上传URL
              tempImageUrl: "",                       // 图片存放路径
              language: config.defaultLanguage        // 语言
          };
      //钉钉设置
      $rootScope.dingMobile = {
          isDingMobile: false,                              //是否钉钉移动端，如果是钉钉移动端，需要隐藏当前header，重写钉钉APP Header
          dingHeaderClass: "has-header",                   //隐藏header后 subHeader ion-content需要修改相关样式
          dingSubHeaderClass: "has-header has-subheader",  //隐藏header后 subHeader ion-content需要修改相关样式
          hideHeader: false                                 //是否需要隐藏当前Header
      }
      // 本地存储
      $scope.setLocalStorage = function () {
          // 存储设置信息
          window.localStorage.setItem("OThinker.H3.Mobile.Setting", JSON.stringify($scope.setting));
      };
      // 当前登录的用户信息
      if (window.cordova) {
          $scope.user = JSON.parse(window.localStorage.getItem("OThinker.H3.Mobile.User")) ||
          {
              ObjectID: "",
              Code: "",
              Password: "",
              Image: "",
              Name: "",
              MobileToken: "" // 服务器端返回的Token
          };
      } else {
          $scope.user = {
              ObjectID: "",
              Code: "",
              Password: "",
              Image: "",
              Name: "",
              MobileToken: "" // 服务器端返回的Token
          };
      }
  });
