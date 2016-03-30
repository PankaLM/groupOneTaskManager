/*global angular*/
(function (angular) {
  'use strict';

  angular.module('taskManager').factory('Tasks', ['$resource', function ($resource) {
    return $resource('api/tasks/:id');
  }]);
}(angular));