app.controller('runsController', ['$scope', '$filter', 'dataService', function ($scope, $filter, dataService) {
    $scope.runs = [];
    $scope.selectedIndex = -1;

    $('button.close').click(function () {
        $('.alert').hide();
    });

    $("#fromDatetimepicker").datepicker({
        noDefault: true,
        format: 'dd mmm yyyy, hh:ii',
        weekStart: 1
    });
    $("#toDatetimepicker").datepicker({
        noDefault: true,
        format: 'dd mmm yyyy, hh:ii',
        weekStart: 1
    });

    $("#datetimepicker").datepicker({
        noDefault: true,
        format: 'dd mmm yyyy, hh:ii',
        weekStart: 1
    })
    .on("changeDate", function (e) {
        var value = this.value;
        var date = Date.parse(value);
        if (date) {
            $scope.form.date.value = new Date(date);
            $scope.form.date.$setValidity('dateinvalid', true);
        }
        else {
            $scope.form.date.value = "";
            $scope.form.date.$setValidity('dateinvalid', false);
        }
    });

    dataService.getRuns().then(function (results) {
        $scope.runs = results;  
        
        $scope.runs.forEach(function (el) {
            el.date = new Date(el.date);
        });
    },
    function (error) {
        var message = "Unspecified error.";
        if(error.data) {
            message = error.data.Message;
        }
        $scope.message = message;
    });
    
    $scope.filter = function() {
        $scope.from = Date.parse(fromDatetimepicker.value);
        $scope.to = Date.parse(toDatetimepicker.value);
    }
    
    $scope.dateFilter = function (run) {
        var from = $scope.from ? new Date($scope.from) : new Date("1970/01/01 00:00");
        var to = $scope.to ? new Date($scope.to) : new Date("2100/01/01 00:00");
        
        var date = run.date.getTime();
        return (date >= from) && (date < to);
    };

    $scope.generateReport = function () {
        dataService.getRuns().then(function (results) {
            $scope.isReport = true;
            
            var runs = results;  
            var weeks = [];
            
            if(runs.length > 0) {
                runs = $filter('orderBy')(runs, 'date');
                
                function getWeek(d) {
                    var day = d.getDay();
                    var monday = d.getDate() - day + (day == 0 ? -6 : 1);
                    var sunday = d.getDate() + 7 - day;
                    
                    monday = new Date(d).setDate(monday);
                    sunday = new Date(d).setDate(sunday);

                    return {
                        from: new Date(monday).setHours(0, 0, 0, 0),
                        to: new Date(sunday).setHours(0, 0, 0, 0)
                    }
                };
                
                var week = {
                    to: new Date('1970/01/01')
                };

                for(var i=0; i<runs.length; i++) {
                    if(runs[i].date.setHours(0, 0, 0, 0) <= week.to) {
                        week.distance += runs[i].distance;
                        week.duration += runs[i].duration;
                        week.active++;
                    }
                    else {
                        if(week.active) {
                            week.speed = week.distance / week.duration * 60 / 1000 / week.active;
                            week.distance = week.distance / week.active;
                            weeks.push(week);
                        }
                        
                        week = getWeek(runs[i].date);
                        week.distance = runs[i].distance;
                        week.duration = runs[i].duration;
                        week.active = 1;
                    }
                }
                
                if(week.active) {
                    week.speed = week.distance / week.duration * 60 / 1000 / week.active;
                    week.distance = week.distance / week.active;
                    weeks.push(week);
                }
            }

            // set up report charts
            var labels = [];
            var distance = [];
            var speed = [];
            weeks.forEach(function(week) {
                labels.push($filter('date')(week.from, "dd MMM yyyy"));
                distance.push(Math.round(week.distance));
                speed.push(Math.round(week.speed));
            });
            
            $scope.distanceSeries = ["Average distance"];
            $scope.speedSeries = ["Average speed"];
            $scope.labels = labels;
            $scope.distance = [distance];
            $scope.speed = [speed];

            // set up report grid
            $scope.weeks = weeks;
        },
        function (error) {
            var message = "Unspecified error.";
            if(error.data) {
                message = error.data.Message;
            }
            $scope.message = message;
        });
    };

    $scope.add = function (run) {
        if (!$scope.form.$valid) {
            return;
        }

        run.date = $scope.form.date.value;

        dataService.postRun(run).then(function (results) {
            var newRun = results.data;
            newRun.date = new Date(newRun.date);

            $scope.runs.push(newRun);
            
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
    
    $scope.select = function (run) {
        $scope.run = angular.copy(run);
        
        var index = 0;
        while(index < $scope.runs.length && 
              $scope.runs[index++].id !== run.id);
        $scope.selectedIndex = index - 1;
        
        $scope.form.date.value = new Date(run.date);
        $scope.form.date.$setValidity('dateinvalid', true);
        
        form.datetimepicker.value = $filter('date')(run.date, "dd MMM yyyy, hh:mm");
        form.datetimepicker.focus();
    }
    
    $scope.reset = function () {
        $scope.selectedIndex = -1;
        $scope.run = null;
        
        form.reset();
        $scope.form.date.value = "";
        $scope.form.date.$setValidity('dateinvalid', false);

        $scope.form.$setPristine()
        $scope.form.$setUntouched();
    }

    $scope.update = function (run) {
        if (!$scope.form.$valid) {
            return;
        }
        
        run.date = $scope.form.date.value;
        
        dataService.putRun(run.id, run).then(function (results) {
            $scope.runs[$scope.selectedIndex] = run;
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

    $scope.delete = function (run) {
        dataService.deleteRun(run.id).then(function () {
            $scope.runs.splice($scope.selectedIndex, 1);
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
