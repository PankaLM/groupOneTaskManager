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

    $scope.calcRecommendation = function (task) {
      if (!task.internalImportance && !task.externalImportance) task.recomendation = "Trash";
      if (!task.internalImportance && task.externalImportance) task.recomendation = "Transfer";
      if (task.internalImportance && task.clearness && task.simplicity && !task.closeness) task.recomendation = "Schedule";
      if (task.internalImportance && task.clearness && !task.simplicity && !task.closeness) task.recomendation = "Delegate";
      if (task.internalImportance && !task.clearness) task.recomendation = "Clarify";
      if (task.internalImportance && task.clearness && !task.simplicity && task.closeness) task.recomendation = "Simplify";
      if (task.internalImportance && task.externalImportance && task.clearness && task.simplicity && task.closeness) task.recomendation = "Execute";
    };
    $scope.calcRecommendation(task);

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
