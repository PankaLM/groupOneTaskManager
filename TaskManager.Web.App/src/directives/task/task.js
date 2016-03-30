(function (angular) {
  'use strict';

  function TaskDirective() {
    return {
      priority: 110,
      replace: true,
      templateUrl: 'directives/task/task.html',
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
          }
        }
      }
    };
  }

  angular.module('taskManager').directive('task', TaskDirective);
}(angular));