/*global angular, _*/
(function (angular, _) {
  'use strict';

  angular.module('taskManager').factory('Users', ['$resource', function ($resource) {
    return $resource('api/users/:userId', {}, {
        changePassword: {
          method: 'POST',
          url: 'api/user/changePassword'
        }
      });
  }]);
}(angular, _));
