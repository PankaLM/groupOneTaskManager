(function (angular) {
    'use strict';
  angular.module('taskManager', [
      'ng',
      'ui.router',
      'ngResource',
      'localytics.directives',
      'ui.date',
      'ui.bootstrap',
      'smart-table',
      'jkuri.timepicker'
    ])
    .config([
      '$stateProvider',
      '$urlRouterProvider',
      function (
        $stateProvider,
        $urlRouterProvider) {

        $urlRouterProvider.otherwise('/login');
        $urlRouterProvider.otherwise(function($injector, $location){
          var isLoggedIn = $injector.get('authenticationService').isLoggedIn();
          if (isLoggedIn) {
            $location.url('/tasks');
          } else {
            $location.url('/login');
          }
        });
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
                }],
              flyCutoff: [
                'Tasks',
                function (Tasks) {
                  return Tasks.getFlyCutoff().$promise
                    .then(function (res) {
                      return res.flyCutoff;
                    });
                }]
            }
          })
          .state('recurringTaskGroups', {
            url: '/groups',
            controller: 'RecurringTaskGroupsSearchCtrl',
            templateUrl: 'tasks/views/recurringTaskGroupsSearch.html',
            resolve: {
              groups: [
                'Tasks',
                function (Tasks) {
                  return Tasks.getRecurringTaskGroups().$promise;
                }]
            }
          })
          .state('metrics', {
            url: '/metrics',
            controller: 'TasksMetricsCtrl',
            templateUrl: 'tasks/views/tasksMetrics.html',
            resolve: {
              tasks: [
                'Tasks',
                function (Tasks) {
                  return Tasks.getMetrics().$promise;
                }]
            }
          })
          .state('statistics', {
            url: '/statistics',
            controller: 'TasksStatisticsCtrl',
            templateUrl: 'tasks/views/tasksStatistics.html',
            resolve: {
              statistics: [
                'Tasks',
                function (Tasks) {
                  return Tasks.getStatistics().$promise;
                }]
            }
          })
          .state('inviteFriend', {
            url: '/inviteFriend',
            controller: 'InviteFriendCtrl',
            templateUrl: 'users/views/inviteFriend.html'
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
            templateUrl: 'tasks/views/tasksNew.html',
            resolve: {
              newTask: [
                'Tasks',
                '$stateParams',
                function (Tasks, $stateParams) {
                  return Tasks.getNew().$promise;
                }]
            }
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
          })
          .state('settings', {
            url: '/settings',
            controller: 'SettingsEditCtrl',
            templateUrl: 'settings/views/settingsEdit.html',
            resolve: {
              flyCutoff: [
                'Tasks',
                function (Tasks) {
                  return Tasks.getFlyCutoff().$promise
                    .then(function (res) {
                      return res.flyCutoff;
                    });
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
    ]).controller('AppCtrl', [
      '$rootScope',
      '$scope',
      'authenticationService',
      '$state', 
      'Tasks',
      function ($rootScope, $scope, authenticationService, $state, Tasks) {
        $scope.logout = function () {
          authenticationService.signOut();
        };
        $scope.removeAlert = function () {
          $scope.showAlert = false;
        };
        $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState) {
          $scope.isLoggedIn = authenticationService.isLoggedIn();
          if ($scope.isLoggedIn && toState && toState.name === 'tasks') {
              Tasks.checkForOverloading().$promise.then(function (res) {
                if (res.hasOverloadedDay) {
                  $scope.showAlert = true;
                  $scope.overloadedDate = res.overloadedDay;
                } else {
                  $scope.showAlert = false;
                }
              });
          }
          $scope.alertForOverloading = false;
        });
        
    }]);
}(angular));
