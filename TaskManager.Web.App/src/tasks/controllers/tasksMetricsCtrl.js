(function (angular, _) {
  'use strict';

  function TasksMetricsCtrl(
      $scope,
      $state,
      Tasks,
      tasks
    ) {
    $scope.originalTasks = tasks;
    $scope.tasks = tasks;
    $scope.flyLimit = null;

    $scope.edit = function (id) {
      return $state.go('tasksEdit', { id: id });
    };

    $scope.limitTasks = function () {
      if ($scope.flyLimit) {
        $scope.tasks = _.filter($scope.originalTasks, function (task) {
          return task.flyScore >= $scope.flyLimit;
        });
      } else {
        $scope.tasks = $scope.originalTasks;
      }
    };
  }

  TasksMetricsCtrl.$inject = [
      '$scope',
      '$state',
      'Tasks',
      'tasks'
  ];
  angular.module('taskManager')
    .controller('TasksMetricsCtrl', TasksMetricsCtrl);
}(angular, _));
