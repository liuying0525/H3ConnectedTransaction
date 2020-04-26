angular.module("starter.directives", [])
.directive('bpmSheetUserSelected', function () {
    return {
        restrict: 'E',
        replace: true,
        scope: {
            selectUsers: '=',
            cancelSelected: '='
        },
        templateUrl: '/Portal/Mobile/templates/sheetUserSelected.html',
    }
})