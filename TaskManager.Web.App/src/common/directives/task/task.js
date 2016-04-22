(function (angular) {
  'use strict';

  function TaskDirective(Noms) {
    return {
      priority: 110,
      replace: true,
      templateUrl: 'common/directives/task/task.html',
      scope: {
        model: '=ngModel',
        readonly: '='
      },
      link: {
        pre: function (scope) {
          Noms
             .query({ alias: 'states' })
             .$promise
             .then(function (states) {
               scope.states = states;
             });
        },
        post: function (scope, element, attrs) {
          if (attrs.readonly) {
            scope.$parent.$watch(attrs.readonly, function (readonly) {
              scope.readonly = readonly;
            });
          }
        }
      }
    };
  }

  TaskDirective.$inject = ['Noms'];

  angular.module('taskManager').directive('task', TaskDirective);
}(angular));