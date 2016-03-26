(function (angular) {
    'use strict';

    angular.module('taskManager')
    .controller('PageController', ['$scope', '$stateParams', function($scope, $stateParams) {
        $scope.page = $stateParams.pageName;
    }]);
}(angular));
