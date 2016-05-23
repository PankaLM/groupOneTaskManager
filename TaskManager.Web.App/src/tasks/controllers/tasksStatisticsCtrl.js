(function (angular) {
  'use strict';

  function TasksStatisticsCtrl(
      $scope,
      statistics
    ) {
    $scope.statistics = statistics;
  }

  TasksStatisticsCtrl.$inject = [
      '$scope',
      'statistics'
  ];
  angular.module('taskManager')
    .controller('TasksStatisticsCtrl', TasksStatisticsCtrl);
}(angular));
