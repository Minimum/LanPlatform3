LPAngular.controller("RouteHomeMain", function ($scope, $interval) {
    LPInterface.NavSelect("home");

    $scope.AccountLoaded = false;

    $scope.WeatherStatus = null;

    $scope.NewsStatusBody = "No status loaded!";
    
    $scope.UpdateNewsStatus = function(data) {
        if (data.Status == LPNet.RESPONSE_HANDLED) {
            $scope.NewsStatusBody = data.data.Content;
        }
    }

    $scope.UpdateWeather = function(data) {
        if (data.Status == LPNet.RESPONSE_HANDLED) {
            $scope.WeatherStatus = data.data;
        }
    }

    if (LPAccounts.LocalAccount != null) {
        $scope.AccountFirstName = LPAccounts.LocalAccount.FirstName;

        $scope.AccountLoaded = true;
    }

    // Check news status every 10s
    $interval(function () {
        LPNews.GetCurrentStatus($scope.UpdateNewsStatus);
    }, 10000);

    LPNews.GetCurrentStatus($scope.UpdateNewsStatus);
    //LPNews.GetWeather($scope.UpdateWeather);
});