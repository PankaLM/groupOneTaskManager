(function (angular) {
  'use strict';

  function TasksEditCtrl(
      $scope,
      $stateParams,
      $state,
      task,
      Tasks
    ) {
    $scope.editMode = false;
    $scope.task = task;

    $scope.back = function () {
      return $state.go('tasks', {});
    };

    $scope.save = function () {
      Tasks.save({ id: $stateParams.id }, $scope.task)
        .$promise
        .then(function (result) {
          return $state.transitionTo('tasks');
        });
    };

    $scope.remove = function () {
      Tasks.remove({ id: $stateParams.id })
        .$promise
        .then(function () {
          return $state.go('tasks');
        });
    };

  }

  TasksEditCtrl.$inject = [
      '$scope',
      '$stateParams',
      '$state',
      'task',
      'Tasks'
    ];
    angular.module('taskManager').controller('TasksEditCtrl', TasksEditCtrl);
}(angular));
