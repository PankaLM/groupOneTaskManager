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
    $scope.flyLimit = null;

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
      if ($scope.flyLimit) {
        $scope.tasks = _.filter($scope.originalTasks, function (task) {
          return task.flyScore >= $scope.flyLimit;
        });
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
