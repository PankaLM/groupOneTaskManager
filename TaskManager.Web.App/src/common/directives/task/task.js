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
        post: function (scope, element, attrs) {
          if (attrs.readonly) {
            scope.$parent.$watch(attrs.readonly, function (readonly) {
              scope.readonly = readonly;
            });
            scope.selectAction = function () {
              console.log($scope.myOption);
            };

            Noms
              .query({ alias: 'states' })
              .$promise
              .then(function (states) {
                scope.states = states;
              });
          }
        }
      }
    };
  }

  TaskDirective.$inject = ['Noms'];

  angular.module('taskManager').directive('task', TaskDirective);
}(angular));