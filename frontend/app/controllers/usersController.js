app.controller('usersController', ['$scope', 'dataService', function ($scope, dataService) {
    $scope.users = [];
    $scope.selectedIndex = -1;    

    $('button.close').click(function () {
        $('.alert').hide();
    });

    dataService.getUsers().then(function (results) {
        $scope.users = results;
    },
    function (error) {
        var message = "Unspecified error.";
        if(error.data) {
            message = error.data.Message;
        }
        $scope.message = message;
    });

    $scope.select = function (user) {
        $scope.user = angular.copy(user);
        
        var index = 0;
        while(index < $scope.users.length && 
              $scope.users[index++].name !== user.name);
        $scope.selectedIndex = index - 1;
        
        form.isManager.focus();
    }
    
    $scope.reset = function () {
        $scope.selectedIndex = -1;
        $scope.user = null;
        
        form.reset();

        $scope.form.$setPristine()
        $scope.form.$setUntouched();
    }

    $scope.update = function (user) {
        var param = "'isManager=" + user.isManager + "'";
        
        dataService.putUser(user.name, param).then(function (results) {
            $scope.users[$scope.selectedIndex] = user;
            $scope.reset();
        },
        function (error) {
            var message = "Unspecified error.";
            if(error.data) {
                message = error.data.Message;
            }
            $scope.message = message;
        });        
    };

    $scope.delete = function (user) {
        dataService.deleteUser(user.name).then(function () {
            $scope.users.splice($scope.selectedIndex, 1);
            $scope.reset();
        },
        function (error) {
            var message = "Unspecified error.";
            if(error.data) {
                message = error.data.Message;
            }
            $scope.message = message;
        });        
    };
}]);
