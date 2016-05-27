/*global angular*/
(function (angular) {
  'use strict';

  angular.module('taskManager').factory('Tasks', ['$resource', function ($resource) {
    return $resource('api/tasks/:id', {}, {
      getNew: {
        method: 'GET',
        url: 'api/tasks/new'
      },
      getRecurringTaskGroups: {
        method: 'GET',
        url: 'api/tasks/recurringTaskGroups',
        isArray: true
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
      saveFlyCutoff: {
        method: 'POST',
        url: 'api/tasks/flyCutoff'
      },
      getFlyCutoff: {
        method: 'GET',
        url: 'api/tasks/flyCutoff'
      }
    });
  }]);
}(angular));