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

    $scope.motionLevel = null;
    $scope.scaleStyle = null;

    //TODO: Link to backend.
    $scope.updateScale = function (motionLevel) {
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
    };
    $scope.updateScale(15);

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
