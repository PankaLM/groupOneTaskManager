(function (angular) {
  'use strict';

  angular.module('taskManager')
  .factory('authenticationService', [
      '$q',
      '$injector',
      '$rootScope',
      'sessionTokenStore',
      function ($q, $injector, $rootScope, sessionTokenStore) {
        var $http;
        function AuthenticationService() {
        }

        function getHttp() {
          //service initialized later because of circular dependency problem.
          $http = $http || $injector.get('$http');
        }

        function createFormUrlEncodedString(data) {
          var res = [];
          _.forOwn(data, function (value, key) {
            res.push(encodeURIComponent(key) + '=' + encodeURIComponent(value));
          });
          return res.join('&');
        }
        AuthenticationService.prototype.authenticate = function (username, password) {
          var self = this;
          getHttp();

          return $http({
            method: 'POST',
            url: 'api/token',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' },
            data: createFormUrlEncodedString({
              username: username,
              password: password,
              grant_type: 'password'
            })
          }).then(function (response) {
            if (response.data.token_type !== 'bearer' &&
                !response.data.access_token) {
              return $q.reject('Unsupported token type');
            }

            sessionTokenStore.setToken(response.data.access_token);

            return true;
          }, function (response) {
            if (response.data.error === 'invalid_grant') {
              return false;
            }
            return $q.reject(response.data);
          });
        };

        AuthenticationService.prototype.authConfig = function (config) {
          var accessToken = sessionTokenStore.getToken();
          if (accessToken) {
            config.headers.Authorization = 'Bearer ' + accessToken;
          }

          return config;
        };

        AuthenticationService.prototype.signOut = function () {
          sessionTokenStore.deleteToken();
          $rootScope.$broadcast('authenticationRequired', this);

          return $q.when();
        };

        AuthenticationService.prototype.isLoggedIn = function () {
          var accessToken = sessionTokenStore.getToken();
          if (accessToken) {
            return true;
          } else {
            return false;
          }
        };

        return new AuthenticationService();
      }
  ])
  .factory('authHttpInterceptor', ['$q', 'authenticationService', function ($q, authenticationService) {
    return {
      request: function (config) {
        return authenticationService.authConfig(config);
      },
      responseError: function (rejection) {
        return $q.reject(rejection);
      }
    };
  }])

  .config(['$httpProvider', function ($httpProvider) {
    $httpProvider.interceptors.push('authHttpInterceptor');
  }]);
}(angular));