'use strict';

app.controller('indexController', ['$scope', '$location', 'dataService', 'authenticationService', function ($scope, $location, dataService, authenticationService) {
    $('button.close').click(function () {
        $('.alert').hide();
    });
    
    $scope.isActive = function (viewLocation) { 
        return viewLocation === $location.path();
    };    
    
    $scope.logout = function () {
        dataService.cleanup();
        authenticationService.logout();
        $location.path('/#/');
    }

    $scope.authentication = authenticationService.authentication;
}]);