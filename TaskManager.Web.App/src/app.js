(function (angular) {
    'use strict';

    angular.module('taskManager', [
        'ui.router',
        'ngResource',
    ])
    .config([
        '$stateProvider',
        '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider
            .otherwise('/');

            $stateProvider
            .state("tasks", {
              url: "/",
              controller: 'TasksCtrl',
              templateUrl: 'tasks/views/tasksSearch.html'
            });
        }
    ]);
}(angular));
