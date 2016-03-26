(function (angular) {
    'use strict';

    angular.module('taskManager').controller('HomeController', ['$scope', function($scope) {
        $scope.page = 'Homepage';
    }]);
}(angular));
