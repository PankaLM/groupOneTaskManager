/*global angular*/
(function (angular) {
  'use strict';

  angular.module('taskManager').factory('Tasks', ['$resource', function ($resource) {
    return $resource('api/tasks/:id', {}, {
      getNew: {
        method: 'GET',
        url: 'api/tasks/new'
      },
      getMetrics: {
        method: 'GET',
        url: 'api/tasks/metrics',
        isArray: true
      },
      checkForOverloading: {
        method: 'GET',
        url: 'api/tasks/overloading'
      },
      getStatistics: {
        method: 'GET',
        url: 'api/tasks/statistics'
      },
    });
  }]);
}(angular));