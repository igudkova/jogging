'use strict';

app.factory('interceptorService', ['$q', '$location', 'authenticationService', function ($q, $location, authenticationService) {
    function request(config) {
        config.headers = config.headers || {};
       
        if (authenticationService.authentication) {
            config.headers.Authorization = 'Bearer ' + authenticationService.authentication.token;
        }

        return config;
    }

    function responseError (rejection) {
        if (rejection.status === 401) {

            authenticationService.logout();

            $location.path('/login');
        }
        return $q.reject(rejection);
    }

    return {
        request: request,
        responseError: responseError
    }
}]);