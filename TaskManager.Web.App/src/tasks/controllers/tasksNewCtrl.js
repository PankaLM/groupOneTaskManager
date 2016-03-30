(function (angular) {
  'use strict';

  function TasksNewCtrl(
      $scope,
      $state,
      Tasks
    ) {
    $scope.newTask = {};
    $scope.editMode = false;

    $scope.back = function () {
      return $state.go('tasks', {});
    };

    $scope.save = function () {
      Tasks.save($scope.newTask)
        .$promise
        .then(function (result) {
          return $state.transitionTo('tasksEdit', {id: result.id});
        });
    };
  }

  TasksNewCtrl.$inject = [
      '$scope',
      '$state',
      'Tasks'
    ];
  angular.module('taskManager').controller('TasksNewCtrl', TasksNewCtrl);
}(angular));
