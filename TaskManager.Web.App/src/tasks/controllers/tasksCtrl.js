(function (angular) {
    'use strict';

    angular.module('taskManager').controller('TasksCtrl',
      ['$scope', 'Tasks', function ($scope, Tasks) {
        $scope.tasks = Tasks.query();
    }]);
}(angular));
