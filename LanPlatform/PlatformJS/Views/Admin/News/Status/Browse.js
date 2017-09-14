LPAngular.controller("RouteAdminNewsStatusBrowse", function ($scope, $location, $routeParams) {
    LPInterface.NavSelect("admin");

    $scope.LoadAttempted = false;

    $scope.CurrentPage = $routeParams.page;
    $scope.LoadStatus = 0;
    $scope.ErrorMessage = "";

    $scope.Statuses = [];

    $scope.Pages = [];
    $scope.PreviousPage = { Text: "", Active: false };
    $scope.NextPage = { Text: "", Active: false };

    $scope.UpdatePermissions = function () {
        if (LPAccounts.CheckLocalPermission("AdminCP")) {
            if ($scope.LoadAttempted == false) {
                LPNews.GetStatusPage($scope.CurrentPage, $scope.LoadData);

                $scope.LoadAttempted = true;
            }
        }
        else {
            $location.path("accessdenied");
        }

        return;
    }

    $scope.LoadData = function (data) {
        if (data != null && data.Data != null) {
            $scope.Statuses = data.Data.Results;

            // Pageation
            var totalPages = Math.ceil(data.Data.TotalResults / 50);
            var nextPageStart = 3;

            if ($scope.CurrentPage == 1) {
                $scope.Pages[0] = $scope.CreatePageObject(1, true);

                nextPageStart = 1;
            } else if ($scope.CurrentPage == 2) {
                $scope.Pages[0] = $scope.CreatePageObject(1, false);
                $scope.Pages[1] = $scope.CreatePageObject(2, true);

                nextPageStart = 2;
            } else {
                $scope.Pages[0] = $scope.CreatePageObject($scope.CurrentPage - 2, false);
                $scope.Pages[1] = $scope.CreatePageObject($scope.CurrentPage - 1, false);
                $scope.Pages[2] = $scope.CreatePageObject($scope.CurrentPage, true);
            }

            for (var x = nextPageStart; x < 5; x++) {
                var page = $scope.CurrentPage - nextPageStart + x + 1;

                if (totalPages < page) {
                    break;
                }

                $scope.Pages[x] = $scope.CreatePageObject(page, false);
            }

            // Previous/Next Page Buttons
            if ($scope.CurrentPage > 1) {
                $scope.PreviousPage.Text = "#!admin/news/status/browse/" + $scope.CurrentPage - 1;
                $scope.PreviousPage.Active = true;
            }

            if ($scope.CurrentPage < totalPages) {
                $scope.NextPage.Text = "#!admin/news/status/browse/" + $scope.CurrentPage + 1;
                $scope.NextPage.Active = true;
            }

            $scope.LoadStatus = 1;
        } else {
            $scope.LoadStatus = 2;
        }

        $scope.$apply();

        return;
    }

    $scope.CreatePageObject = function (num, active) {
        var page = {};

        page.Active = active;
        page.Text = num;

        return page;
    }

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {
            $scope.LoadAttempted = true;

            LPNews.GetStatusPage($scope.CurrentPage, $scope.LoadData);

            $scope.PermissionHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
        }
        else {
            $location.path("accessdenied");
        }
    }
    else {
        $scope.PermissionHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
    }

    $scope.$on("$destroy", function () {
        if ($scope.PermissionHook != null) {
            LPAccounts.OnPermissionsChange.RemoveHook($scope.PermissionHook);
        }
    });

});