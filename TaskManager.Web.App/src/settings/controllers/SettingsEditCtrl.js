(function (angular) {
  'use strict';

  function SettingsEditCtrl(
      $scope,
      $state,
      Tasks,
      flyCutoff
    ) {
    $scope.editMode = false;

    $scope.model = { flyCutoff: flyCutoff };

    $scope.save = function () {
      Tasks.saveFlyCutoff({ flyCutoff: $scope.model.flyCutoff }, {})
        .$promise
        .then(function (result) {
          return $state.transitionTo('tasks');
        });
      $scope.editMode = false;
    };

    $scope.back = function () {
      return $state.go('tasks');
    };
  }

  SettingsEditCtrl.$inject = [
      '$scope',
      '$state',
      'Tasks',
      'flyCutoff'
    ];
  angular.module('taskManager').controller('SettingsEditCtrl', SettingsEditCtrl);
}(angular));
