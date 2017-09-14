LPAngular.controller("RouteHomeMain", function ($scope) {
    LPInterface.NavSelect("home");

    $scope.AccountLoaded = false;

    $scope.WeatherStatus = null;

    $scope.NewsStatus = {};
    $scope.NewsStatus.Id = 0;
    $scope.NewsStatus.Title = "No Title";
    $scope.NewsStatus.Content = "No status loaded!";
    
    $scope.UpdateNewsStatus = function(data) {
        if (data.Status == LPNet.RESPONSE_HANDLED) {
            $scope.NewsStatus = data.data;
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

    LPNews.GetCurrentStatus($scope.UpdateNewsStatus);
    //LPNews.GetWeather($scope.UpdateWeather);
});