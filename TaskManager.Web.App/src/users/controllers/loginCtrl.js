(function (angular) {
  'use strict';

  function LoginCtrl(
      $scope,
      $state,
      authenticationService
    ) {
        $scope.invalidUsernameAndPassword = false;
        $scope.submitting = false;

    $scope.login = function (username, password) {
      return authenticationService
              .authenticate(username, password)
              .then(function (success) {
                if (success) {
                  $state.go('tasks');
                } else {
                  $scope.submitting = false;
                  $scope.invalidUsernameAndPassword = true;
                }
              });
    };
  }

  LoginCtrl.$inject = [
      '$scope',
      '$state',
      'authenticationService'
  ];

  angular.module('taskManager').controller('LoginCtrl', LoginCtrl);
}(angular));
