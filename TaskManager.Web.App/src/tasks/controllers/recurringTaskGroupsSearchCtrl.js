(function (angular, _) {
  'use strict';

  function RecurringTaskGroupsSearchCtrl(
      $scope,
      $state,
      Tasks,
      groups
    ) {
    $scope.groups = groups;

    $scope.createNew = function () {
      return $state.transitionTo('tasksNew');
    };
    $scope.edit = function (id) {
      return $state.go('tasksEdit', { id: id });
    };

    $scope.remove = function (id) {
      Tasks.remove({ id: id })
        .$promise
        .then(function () {
          _.remove($scope.groups,
             function (task) {
               return task.taskId === id;
             });
        });
    };
  }

  RecurringTaskGroupsSearchCtrl.$inject = [
      '$scope',
      '$state',
      'Tasks',
      'groups'
  ];
  angular.module('taskManager')
    .controller('RecurringTaskGroupsSearchCtrl', RecurringTaskGroupsSearchCtrl);
}(angular, _));
