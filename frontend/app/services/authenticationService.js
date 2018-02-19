'use strict';

app.factory('authenticationService', function () {
    var authentication = {
        isAuth: false,
        userName: "",
        isManager: false,
        token: ""
    };
    
    function getFromLocalStorage() {
        var data = localStorage.getItem('authorizationData');
        if(data === "{}") {
            return null;
        }
        
        return JSON.parse(data);
    };

    function saveToLocalStorage(obj) {
        localStorage.setItem('authorizationData', JSON.stringify(obj));
    };

    function logout() {
        authentication.isAuth = false;
        authentication.userName = "";
        authentication.isManager = false;
        authentication.token = "";
        
        saveToLocalStorage({});
    };

    function readAuthData() {
        var authData = getFromLocalStorage();
        if (authData) {
            authentication.isAuth = true;
            authentication.userName = authData.userName;
            authentication.isManager = authData.isManager;
            authentication.token = authData.token;
        }
    };

    function saveAuthData(authData) {
        authentication.isAuth = true;
        authentication.userName = authData.userName;
        authentication.isManager = authData.isManager;
        authentication.token = authData.token;
        
        saveToLocalStorage({
            userName: authentication.userName,
            isManager: authentication.isManager,
            token: authentication.token,
        });
    };

    return {
        authentication: authentication,
        logout: logout,
        readAuthData: readAuthData,
        saveAuthData: saveAuthData
    }
});