/*global angular*/
(function (angular) {
  'use strict';

  angular.module('taskManager').factory('Noms', ['$resource', function ($resource) {
    return $resource('api/noms/:alias/:id');
  }]);
}(angular));