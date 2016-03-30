(function (angular, _) {
  'use strict';

  function TasksSearchCtrl(
      $scope,
      $state,
      Tasks,
      tasks
    ) {
    $scope.tasks = tasks;

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
