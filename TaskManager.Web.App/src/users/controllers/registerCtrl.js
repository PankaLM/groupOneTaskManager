(function (angular) {
  'use strict';

  function RegisterCtrl(
      $scope,
      $state,
      Users
    ) {
    $scope.register = function () {
      Users.save({}, $scope.newUser)
        .$promise
        .then(function (result) {
          return $state.transitionTo('login');
        });
    };
  }

  RegisterCtrl.$inject = [
      '$scope',
      '$state',
      'Users'
  ];

  angular.module('taskManager').controller('RegisterCtrl', RegisterCtrl);
}(angular));
