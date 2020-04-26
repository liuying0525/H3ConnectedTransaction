angular.module('starter', ['ionic', 'starter.controllers', 'starter.services', 'ngCordova'])

    .config(function($stateProvider, $urlRouterProvider,$ionicConfigProvider) {

      $ionicConfigProvider.platform.ios.tabs.style('standard');
      $ionicConfigProvider.platform.ios.tabs.position('bottom');
      $ionicConfigProvider.platform.android.tabs.style('standard');
      $ionicConfigProvider.platform.android.tabs.position('standard');



      $ionicConfigProvider.platform.ios.navBar.alignTitle('center');
   /*   $ionicConfigProvider.platform.android.navBar.alignTitle('left');*/

      $ionicConfigProvider.platform.ios.backButton.previousTitleText('').icon('ion-ios-arrow-thin-left');
      $ionicConfigProvider.platform.android.backButton.previousTitleText('').icon('ion-android-arrow-back');

      $ionicConfigProvider.platform.ios.views.transition('ios');
      $ionicConfigProvider.platform.android.views.transition('android');

     var defaultState = "/tab/home";
        if (window.cordova) {
            defaultState = "/login";
        }
      // Ionic uses AngularUI Router which uses the concept of states
      // Learn more here: https://github.com/angular-ui/ui-router
      // Set up the various states which the app can be in.
      // Each state's controller can be found in controllers.js
      $stateProvider

        // setup an abstract state for the tabs directive
        //登陆
          .state('login',{
            url:'/login',
            templateUrl:'templates/login.html',
            controller:'loginCtrl'
          })

        //系统设置
        .state('settings',{
          url:'/settings',
          templateUrl:'templates/settings.html',
          controller:'settingsCtrl'
        })
        .state('language',{
          url:'/tab/settings/language',
          templateUrl:'templates/language.html',
          controller:'settingsCtrl'
        })


        //表单页-编辑-展示
        .state('inputGroup',{
          url:'/inputGroup',
          templateUrl:'templates/input-group.html',
          controller:'inputGroupCtrl'
        })

        //流程详情
        .state('details',{
          url:'/details',
          templateUrl:'templates/details.html',
          controller:'detailsCtrl'
        })
        //普通流程详情
        .state('listDetails',{
          url:'/listDetails',
          templateUrl:'templates/listDetails.html',
          controller:'listDetailsCtrl'
        })
        //底部
          .state('tab', {
            url: '/tab',
            abstract: true,
            templateUrl: 'templates/tabs.html'
          })
          .state('tab.home', {
            url: '/home',
            views: {
              'tab-home': {
                templateUrl: 'templates/tab-home.html',
                controller: 'HomeCtrl'
              }
            }
          })
          .state('tab.startworkflow', {
            url: '/startworkflow',
            views: {
              'tab-startworkflow': {
                templateUrl:'templates/tab-startworkflow.html',
                controller: 'startworkflowCtrl'
              }
            }
          })
          .state('tab.myInstances', {
            url: '/myInstances',
            views: {
              'tab-myInstances': {
                templateUrl: 'templates/tab-myInstances.html',
                controller: 'myInstancesCtrl'
              }
            }
          });
      // $urlRouterProvider.otherwise(defaultState);
      $urlRouterProvider.otherwise('/login');


    });
