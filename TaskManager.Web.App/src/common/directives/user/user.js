(function (angular) {
  'use strict';

  function UserDirective() {
    return {
      priority: 110,
      replace: true,
      templateUrl: 'common/directives/user/user.html',
      scope: {
        model: '=ngModel',
        readonly: '=',
        isNew: '='
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

  angular.module('taskManager').directive('user', UserDirective);
}(angular));