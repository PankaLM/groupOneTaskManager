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
    $scope.motionLevelDictionary = {
      'Trash': 14,
      'Transfer': 28,
      'Schedule': 42,
      'Defer': 42,
      'Delegate': 56,
      'Follow up': 56,
      'Clarify': 70,
      'Simplify': 84,
      'Execute': 100
    };

    $scope.calcRecommendation = function (task) {
      if (!task.internalImportance && !task.externalImportance) task.recomendation = "Trash";
      if (!task.internalImportance && task.externalImportance) task.recomendation = "Transfer";
      if (task.internalImportance && task.clearness && task.simplicity && !task.closeness) task.recomendation = "Schedule";
      if (task.internalImportance && task.clearness && !task.simplicity && !task.closeness) task.recomendation = "Delegate";
      if (task.internalImportance && !task.clearness) task.recomendation = "Clarify";
      if (task.internalImportance && task.clearness && !task.simplicity && task.closeness) task.recomendation = "Simplify";
      if (task.internalImportance && task.externalImportance && task.clearness && task.simplicity && task.closeness) task.recomendation = "Execute";
      updateScale($scope.motionLevelDictionary[task.recomendation]);
    };

    $scope.calcRecommendation(task);

    function updateScale (motionLevel) {
      var type = null;
      if (motionLevel < 33) {
        type = 'success';
      } else if (motionLevel < 66) {
        type = 'warning';
      } else {
        type = 'danger';
      }
      $scope.motionLevel = motionLevel;
      $scope.scaleStyle = type;
    }

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
