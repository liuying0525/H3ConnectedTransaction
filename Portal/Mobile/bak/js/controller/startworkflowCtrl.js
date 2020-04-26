module.controller('startworkflowCtrl', function ($scope, categories, $ionicSlideBoxDelegate, $ionicLoading,commonJS) {
      /*切换副标题*/
    $scope.slectIndex = 0;
    $scope.searchKey = '';
     $scope.showmodel = false;//显示筛选model
    // $scope.categories = categories.all();
    // console.log(".................."+$scope.categories);
    // console.log($scope.categories);
    //是否是常用标签页 slectIndex==0 是
    $scope.tabNames = ['常用', '全部'];
    $scope.init = function () {
      categories.all().then(function (res) {
            console.log(res);
           $scope.categories = res;
        });
 
    $scope.activeSlide = function (index) {//点击时候触发
        $scope.slectIndex = index;
        $ionicSlideBoxDelegate.slide(index);
    };
    $scope.slideChanged = function (index) {//滑动时候触发
        $scope.slectIndex = index;
    };
    //侧滑框 筛选 openPopover closePopover
        commonJS.sideSlip($scope,'templates/filter.html');
     }
   $scope.init();

       //改变是否常用流程,todo:待优化
    $scope.changeFavorite = function (workflowCode, category) {
       var stetus="";
        if (category == undefined) {
            angular.forEach($scope.categories, function (data, i, full) {
                angular.forEach(data.Workflows, function (Workflow, j, full) {
                    if (Workflow.WorkflowCode == workflowCode) {
                        Workflow.IsFavorite = false;
                        $scope.categories[i].Workflows[j].IsFavorite = false;
                        stetus="取消常用成功";
                    }
                });
            });
        }
        else {
            angular.forEach($scope.categories, function (data, i, full) {
                if (data.DisplayName == category) {
                    angular.forEach(data.Workflows, function (Workflow, j, full) {
                        if (Workflow.WorkflowCode == workflowCode) {
                            Workflow.IsFavorite = !Workflow.IsFavorite;
                            $scope.categories[i].Workflows[j].IsFavorite = Workflow.IsFavorite;
                            if(Workflow.IsFavorite){
                                   stetus="设置常用成功";
                            }
                            else if (!Workflow.IsFavorite){
                                    stetus="取消常用成功";  
                                }
                        }
                    });
                }
            });
        }
        //提示信息
        $ionicLoading.show({
            template: '<span class="setcommon f15">'+stetus+'</span>',
            duration: 2 * 1000,
            animation: 'fade-in',
            showBackdrop: false,
        });
    };
  $scope.showModel=function(falg){
      console.log(falg);
     var $event=document.getElementById("sidemodel");
     if(falg){
      $event.style.display="block";
     }
    else if(falg=="false"){
         $event.removeAttribute('style');
    }
   

  }
});