'use strict';

app.controller('loginController', ['$scope', '$location', 'authenticationService', 'dataService', function ($scope, $location, authenticationService, dataService) {
    $scope.authenticate = function () {
        var redirectUri = location.protocol + '//' + location.host + '/authenticationComplete.html';
        var externalProviderUrl = dataService.baseUri + "api/account/login?response_type=token&redirect_uri=" + redirectUri;

        window.$windowScope = $scope;

        var oauthWindow = window.open(externalProviderUrl, "Authenticate Account", "location=0, status=0, width=600, height=750");
    };

    $scope.authenticationCompleted = function (externalData) {
        $scope.$apply(function () {
            dataService.getToken(externalData.userName, externalData.externalAccessToken).then(function (results) {
                authenticationService.saveAuthData({
                    token: results.data.accessToken,
                    userName: results.data.userName,
                    isManager: results.data.isManager
                });

                $location.path('/runs');
            },
            function (error) {
                authenticationService.logout();
            });
        });
    }
    
    if(authenticationService.authentication.isAuth) {
        $location.path('/runs');
    }
}]);
