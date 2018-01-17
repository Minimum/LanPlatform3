LPAngular.controller("RouteLibraryAppView", function($scope, $location, $routeParams, $uibModal) {
    LPInterface.NavSelect("library");

    $scope.LoadStatus = 0;
    $scope.LoadLoaners = 0;
    $scope.ErrorMessage = "";

    $scope.LoadApp = function (data)
    {
        if (data != null)
        {
            if (data.Data != null)
            {
                $scope.App = new LPApps.App();
                $scope.App.LoadModel(data.Data);

                $scope.AppName = $scope.App.Title;
                $scope.AppType = $scope.App.GetTypeName();

                $scope.LoadStatus = 1;

                LPApps.GetAppLoaners($routeParams.AppId, $scope.LoadLoaners, $scope.LoadLoadersFail);
            }
            else
            {
                if (data.StatusCode == "INVALID_APP") {
                    $scope.LoadStatus = -1;
                    $scope.ErrorMessage = "The app does not exist.";
                }
                else
                {
                    $scope.LoadStatus = -1;
                    $scope.ErrorMessage = "Object returned was null.";
                }
            }
        }
        else
        {
            $scope.LoadStatus = -1;
            $scope.ErrorMessage = "Invalid server response.";
        }

        $scope.$apply();
    }

    $scope.LoadAppFail = function ()
    {
        $scope.LoadStatus = -1;
        $scope.ErrorMessage = "Communication to the server failed.";

        $scope.$apply();
    }

    $scope.LoadLoaners = function (data)
    {
        if (data != null)
        {
            if (data.Data != null)
            {
                $scope.Loaners = data.Data;

                $scope.LoadLoaners = 1;
            }
            else
            {
                $scope.LoadLoaners = -1;
            }
        }
        else
        {
            $scope.LoadLoaners = -1;
        }

        $scope.$apply();
    }

    $scope.LoadLoanersFail = function ()
    {
        $scope.LoadLoaners = -1;

        $scope.$apply();
    }

    LPApps.GetApp($routeParams.appId, $scope.LoadApp, $scope.LoadAppFail);
});