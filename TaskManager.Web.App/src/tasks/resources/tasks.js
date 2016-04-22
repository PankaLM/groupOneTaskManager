﻿/*global angular*/
(function (angular) {
  'use strict';

  angular.module('taskManager').factory('Tasks', ['$resource', function ($resource) {
    return $resource('api/tasks/:id', {}, {
      getNew: {
        method: 'GET',
        url: 'api/tasks/new'
      },
    });
  }]);
}(angular));