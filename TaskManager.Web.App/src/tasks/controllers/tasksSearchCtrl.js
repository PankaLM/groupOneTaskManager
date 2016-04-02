(function (angular, _) {
  'use strict';

  function TasksSearchCtrl(
      $scope,
      $state,
      Tasks,
      tasks
    ) {
      $scope.originalTasks = tasks;
      $scope.tasks = tasks;
      $scope.taskLimit = null;

    $scope.createNew = function () {
      return $state.transitionTo('tasksNew', {});
    };
    $scope.edit = function (id) {
      return $state.go('tasksEdit', { id: id });
    };

    $scope.remove = function (id) {
      Tasks.remove({ id: id })
        .$promise
        .then(function () {
          _.remove($scope.tasks,
             function (task) {
               return task.taskId === id;
             });
        });
    };

    $scope.limitTasks = function () {
      if ($scope.taskLimit) {
          $scope.tasks = $scope.originalTasks.slice(0, $scope.taskLimit);
      } else {
          $scope.tasks = $scope.originalTasks;
      }
    };
  }

  TasksSearchCtrl.$inject = [
      '$scope',
      '$state',
      'Tasks',
      'tasks'
  ];
  angular.module('taskManager')
    .controller('TasksSearchCtrl', TasksSearchCtrl);
}(angular, _));
