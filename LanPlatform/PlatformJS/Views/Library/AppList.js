LPAngular.controller("RouteLibraryApps", function ($scope, $location, $routeParams) {
    LPInterface.NavSelect("library");

    $scope.LoadStatus = 0;

    $scope.Apps = [];
    $scope.AppHooks = [];

    $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagAppEdit);

    // Paging
    $scope.PageSize = 15;
    $scope.CurrentPage = (parseInt($routeParams.page) > 0) ? parseInt($routeParams.page) : 1;

    $scope.Paginator = new LPPagination.Paginator();
    $scope.Paginator.CurrentPage = $scope.CurrentPage;
    $scope.Paginator.LinkPrefix = "#!library/apps/";
    $scope.PageInfo = new LPPagination.PageInfo();

    $scope.LoadApps = function (data) {
        if (data.Data != null) {
            var appCount = data.Data.Results.length;

            for (var x = 0; x < appCount; x++) {
                $scope.Apps[x] = new LPApps.App();

                // Load model into object
                $scope.Apps[x].LoadModel(data.Data.Results[x]);

                // Hook onto apps
                $scope.AppHooks[x] = $scope.Apps[x].OnUpdate.AddHook($scope.UpdateAppDetails);
            }

            // Pageination
            $scope.Paginator.TotalElements = data.Data.TotalResults;
            $scope.Paginator.PageSize = $scope.PageSize;

            $scope.Paginator.Compute();

            $scope.LoadStatus = 1;

            $scope.$apply();
        } else {
            $scope.LoadStatus = 2;
        }
    }

    LPApps.GetAppPage($scope.CurrentPage, $scope.PageSize, true, LPApps.SearchAppSort.Id, $scope.LoadApps);
});