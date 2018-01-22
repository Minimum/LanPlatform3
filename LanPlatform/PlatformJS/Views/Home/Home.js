LPAngular.controller("RouteHomeMain", function ($scope, $interval) {
    LPInterface.NavSelect("home");

    $scope.AccountLoaded = false;
    $scope.WeatherStatus = null;
    $scope.NewsStatusBody = "No status loaded!";
    $scope.QuickLinkLoad = 0;
    
    $scope.LoadNewsStatus = function(data) {
        if (data.Status == LPNet.AppResponseType.ResponseHandled) {
            $scope.NewsStatusBody = data.data.Content;
        }

        $scope.$apply();
    }

    $scope.LoadWeather = function(data) {
        if (data.Status == LPNet.AppResponseType.ResponseHandled) {
            $scope.WeatherStatus = data.data;
        }

        $scope.$apply();
    }

    $scope.LoadQuickLink = function(data) {
        if (data.Status == LPNet.AppResponseType.ResponseHandled) {
            var linkCount = data.Data.length;

            $scope.QuickLinks = [];

            for (var x = 0; x < linkCount; x++) {
                $scope.QuickLinks[x] = new LPNews.QuickLink();

                $scope.QuickLinks[x].LoadModel(data.Data[x]);
            }

            $scope.QuickLinkLoad = 1;
        } else {
            $scope.QuickLinkLoad = -1;
        }

        $scope.$apply();
    }

    $scope.LoadQuickLinkFail = function() {
        $scope.QuickLinkLoad = -1;

        $scope.$apply();
    }

    if (LPAccounts.LocalAccount != null) {
        $scope.AccountFirstName = LPAccounts.LocalAccount.FirstName;

        $scope.AccountLoaded = true;
    }

    // Check news status every 10s
    $interval(function () {
        LPNews.GetCurrentStatus($scope.LoadNewsStatus);
    }, 10000);

    LPNews.GetCurrentStatus($scope.LoadNewsStatus);
    LPNews.GetActiveLinks($scope.LoadQuickLink, $scope.LoadQuickLinkFail);
    //LPNews.GetWeather($scope.LoadWeather);
});