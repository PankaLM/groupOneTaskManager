(function (angular, _) {
  'use strict';

  function TaskDirective($q, Noms) {
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
          var statesPromise = Noms
            .query({ alias: 'states' })
            .$promise;
          var actionsPromise = Noms
            .query({ alias: 'actions' })
            .$promise;
          var tasksPromise = Noms
            .query({ alias: 'tasks', taskId: scope.model.taskId })
            .$promise;
          $q.all({
            states: statesPromise,
            tasks: tasksPromise,
            actions: actionsPromise
          })
            .then(function (results) {
              scope.states = results.states;
              scope.tasks = results.tasks;
              scope.actions = results.actions;
              if (scope.model.dependantTaskId) {
                scope.dependantTask = _.filter(scope.tasks, function (task) {
                  return task.id === scope.model.dependantTaskId;
                })[0];
              }
            });
        },
        post: function (scope, element, attrs) {
          if (attrs.readonly) {
            scope.$parent.$watch(attrs.readonly, function (readonly) {
              scope.readonly = readonly;
            });
          }
     
          scope.$watch('model.dependantTaskId', function (value) {
            scope.dependantTask = _.filter(scope.tasks, function (task) {
              return task.id === scope.model.dependantTaskId;
            })[0];
          });
        }
      }
    };
  }

  TaskDirective.$inject = ['$q', 'Noms'];

  angular.module('taskManager').directive('task', TaskDirective);
}(angular, _));