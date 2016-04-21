(function (angular) {
  'use strict';

  function SettingsEditCtrl(
      $scope,
      $state
    ) {
    $scope.editMode = false;

    //TODO: Get the settings from the server
    $scope.settings = {};

    $scope.save = function () {
      //TODO: Actually save the settings
      $scope.editMode = false;
    };

    $scope.back = function () {
      console.log('dafuq man?');
      return $state.go('tasks', {});
    };
  }

  SettingsEditCtrl.$inject = [
      '$scope',
      '$state'
    ];
  angular.module('taskManager').controller('SettingsEditCtrl', SettingsEditCtrl);
}(angular));
