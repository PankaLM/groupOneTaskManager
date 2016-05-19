(function (angular) {
  'use strict';

  function InviteFriendCtrl(
      $scope,
      $state,
      Users
    ) {
    $scope.model = {};
    $scope.sendInvitation = function () {
      return Users.sendInvitation({ email: $scope.model.email }, {})
        .$promise
        .then(function () {
          return $state.transitionTo('tasks');
        });
    };
  }

  InviteFriendCtrl.$inject = [
      '$scope',
      '$state',
      'Users'
  ];

  angular.module('taskManager').controller('InviteFriendCtrl', InviteFriendCtrl);
}(angular));
