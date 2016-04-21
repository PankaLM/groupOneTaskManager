(function (angular) {
  'use strict';

  function UsersEditCtrl(
      $scope,
      $state,
      Users,
      user
    ) {
    $scope.user = user;
    $scope.isInvalidPassword = false;
    $scope.update = function () {
      if ($scope.user.oldPassword) {
        Users.isCorrectPassword($scope.user.oldPassword).$promise
        .then(function (result) {
          if (result.isCorrect) {
            return save();
          } else {
            $scope.isInvalidPassword = true;
          }
        });
      } else {
        return save();
      }
    };

    function save () {
      return Users.save({ userId: $scope.user.userId }, $scope.user)
        .$promise
        .then(function (result) {
          return $state.transitionTo('tasks');
        });
    }

    function checkPassword (password) {
      if (!password) {
        return true;
      }
      return Users.isCorrectPassword(password).$promise
      .then(function (result) {
        $scope.passIsCorrect = result.isCorrect;
        return result.isCorrect;
      });
    }
  }

  UsersEditCtrl.$inject = [
      '$scope',
      '$state',
      'Users',
      'user'
  ];

  angular.module('taskManager').controller('UsersEditCtrl', UsersEditCtrl);
}(angular));
