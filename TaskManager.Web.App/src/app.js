(function (angular) {
    'use strict';
  angular.module('taskManager', [
      'ng',
      'ui.router',
      'ngResource',
      'localytics.directives',
      'ui.date',
      'smart-table'
    ])
    .config([
      '$stateProvider',
      '$urlRouterProvider',
      function (
        $stateProvider,
        $urlRouterProvider) {

        $urlRouterProvider.otherwise('/tasks');

        $stateProvider
          .state('tasks', {
            url: '/tasks',
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
            url: '/edit/:id',
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
            url: "/new",
            controller: 'TasksNewCtrl',
            templateUrl: 'tasks/views/tasksNew.html'
          })
          .state('login', {
            url: '/login',
            controller: 'LoginCtrl',
            templateUrl: 'users/views/login.html'
          })
          .state('register', {
            url: '/register',
            controller: 'RegisterCtrl',
            templateUrl: 'users/views/register.html'
          })
          .state('profile', {
            url: '/profile',
            controller: 'UsersEditCtrl',
            templateUrl: 'users/views/usersEdit.html',
            resolve: {
              user: [
                '$stateParams', 'Users',
                function ($stateParams, Users) {
                  return Users.getUserProfile().$promise;
                }]
            }
          });
    }])
    .run(['$rootScope', '$state',
        function ($rootScope, $state) {
          $rootScope.$on('authenticationRequired', function () {
            $state.go('login');
          });
        }
    ]).controller('AppCtrl', ['$rootScope', '$scope', 'authenticationService', '$state',
      function ($rootScope, $scope, authenticationService, $state) {
        $scope.logout = function () {
          authenticationService.signOut();
          $state.go('login');
        };
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState) {
          $scope.isLoggedIn = authenticationService.isLoggedIn();
        });
        
    }]);
}(angular));
