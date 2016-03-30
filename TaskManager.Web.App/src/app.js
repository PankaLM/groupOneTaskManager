(function (angular) {
    'use strict';
  angular.module('taskManager', [
        'ng',
        'ui.router',
        'ngResource',
        'localytics.directives',
        'ui.date',
        'ngHandsontable'
    ])
  .config([
    '$stateProvider',
    function (
      $stateProvider) {

      $stateProvider
        .state('tasks', {
          url: '/',
          controller: 'TasksSearchCtrl',
          templateUrl: 'tasks/views/tasksSearch.html',
          resolve: {
            tasks: [
              'Tasks',
              function (Tasks) {
                return Tasks.query().$promise;
              }]
          }
        })
        .state("tasksEdit", {
          url: 'edit/:id',
          controller: 'TasksEditCtrl',
          templateUrl: 'tasks/views/tasksEdit.html',
          resolve: {
            task: [
              '$stateParams', 'Tasks',
              function ($stateParams, Tasks) {
                return Tasks.get({ id: $stateParams.id }).$promise;
              }]
          }
        })
        .state("tasksNew", {
          url: "new",
          controller: 'TasksNewCtrl',
          templateUrl: 'tasks/views/tasksNew.html'
        });
    }]);
}(angular));
