var app = angular.module('joggingApp', ['ngRoute', 'angular-loading-bar', 'chart.js']);

app.config(function ($routeProvider) {
    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/app/views/login.html"
    });

    $routeProvider.when("/runs", {
        controller: "runsController",
        templateUrl: "/app/views/runs.html"
    });

    $routeProvider.when("/users", {
        controller: "usersController",
        templateUrl: "/app/views/users.html"
    });

    $routeProvider.otherwise({ redirectTo: "/runs" });
});

app.config(function ($httpProvider) {
    $httpProvider.interceptors.push('interceptorService');
});

app.run(['$rootScope', '$location', 'authenticationService', function ($rootScope, $location, authenticationService) {
    authenticationService.readAuthData();

    $rootScope.$on('$routeChangeStart', function (event) {
        if (!authenticationService.authentication.isAuth) {
            $location.path('/login');
        }
    });
}])

