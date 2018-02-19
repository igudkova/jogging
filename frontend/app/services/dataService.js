'use strict';

app.factory('dataService', ['$http', '$q', function ($http, $q) {
    var baseUri = 'http://jogging.azurewebsites.net/'; //"http://localhost:54100/"
    var runs, users;

    function getToken(userName, externalAccessToken) {
        return $http.get(baseUri + 'api/account/token?userName=' + userName + "&externalAccessToken=" + externalAccessToken);
    }

    function getRuns() {
        var deferred = $q.defer();
        
        if(runs) {  
            deferred.resolve(runs);
        }
        else {
            $http.get(baseUri + 'api/runs').then(function(results) {
                runs = results.data;
                deferred.resolve(runs);
            },
            function (error) {
                deferred.reject(error);
            });
        }
        
        return deferred.promise;
    }

    function postRun(run) {
        return $http.post(baseUri + 'api/runs', run);
    }

    function deleteRun(id) {
        return $http.delete(baseUri + 'api/runs?id=' + id);
    }

    function putRun(id, run) {
        return $http.put(baseUri + 'api/runs?id=' + id, run);
    }

    function getUsers() {
        var deferred = $q.defer();
        
        if(users) {  
            deferred.resolve(users);
        }
        else {
            $http.get(baseUri + 'api/users').then(function(results) {
                users = results.data;
                deferred.resolve(users);
            },
            function (error) {
                deferred.reject(error);
            });
        }
        
        return deferred.promise;
    }

    function deleteUser(name) {
        return $http.delete(baseUri + 'api/users?name=' + name);
    }

    function putUser(name, param) {
        return $http.put(baseUri + 'api/users?name=' + name, param);
    }
    
    function cleanup() {
        runs = null;
        users = null;
    }

    return {
        baseUri: baseUri,
        getToken: getToken,
        getRuns: getRuns,
        postRun: postRun,
        deleteRun: deleteRun,
        putRun: putRun,
        getUsers: getUsers,
        deleteUser: deleteUser,
        putUser: putUser,
        cleanup: cleanup
    };
}]);