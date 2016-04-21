/*global angular, _*/
(function (angular, _) {
  'use strict';

  angular.module('taskManager').factory('Users', ['$resource', function ($resource) {
    return $resource('api/users/:userId', {}, {
        changePassword: {
          method: 'POST',
          url: 'api/user/changePassword'
        },
        checkDuplicateUsers: {
          method: 'GET',
          url: 'api/user/duplicateUsers',
          isArray: true
        },
        isCorrectPassword: {
          method: 'POST',
          url: 'api/user/isCorrectPassword',
          transformRequest: function (data, headers) {
            _.extend(headers(), {
              'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8'
            });
            return encodeURIComponent('password') + '=' + encodeURIComponent(data);
          }
        },
        getUserProfile: {
          method: 'GET',
          url: 'api/users/profile'
        }
      });
  }]);
}(angular, _));
