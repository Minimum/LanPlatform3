LPAngular.controller("RouteAdminCommunityAccountsBrowse", function ($scope, $location, $routeParams, $uibModal) {
    LPInterface.NavSelect("admin");

    $scope.PageSize = 15;

    $scope.LoadAttempted = false;

    $scope.CurrentPage = (parseInt($routeParams.page) > 0) ? parseInt($routeParams.page) : 1;
    $scope.LoadStatus = 0;
    $scope.ErrorMessage = "";

    $scope.Accounts = [];

    $scope.Paginator = new LPPagination.Paginator();
    $scope.Paginator.CurrentPage = $scope.CurrentPage;
    $scope.Paginator.LinkPrefix = "#!admin/community/accounts/browse/";
    $scope.PageInfo = new LPPagination.PageInfo();

    $scope.LoadData = function (data)
    {
        if (data != null && data.Data != null) {
            $scope.Accounts = [];

            for (var x = 0; x < data.Data.Results.length; x++) {
                $scope.Accounts[x] = LPAccounts.InitializeAccount(data.Data.Results[x]);
            }

            // Pageination
            $scope.Paginator.TotalElements = data.Data.TotalResults;
            $scope.Paginator.PageSize = $scope.PageSize;

            $scope.Paginator.Compute();

            $scope.LoadStatus = 1;
        }
        else {
            if (data != null) {
                if (data.StatusCode == "DAO_ERROR") {
                    $scope.ErrorMessage = "The database has encountered an error.";
                }
            }

            $scope.LoadStatus = 2;
        }

        $scope.$apply();

        return;
    }

    $scope.UpdatePermissions = function () {
        if (LPAccounts.CheckLocalPermission("AdminCP")) {
            if ($scope.LoadAttempted == false) {
                LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, false, LPAccounts.SearchAccountSort.Id, $scope.LoadData);

                $scope.LoadAttempted = true;
            }
        }
        else {
            $location.path("accessdenied");
        }

        return;
    }

    $scope.ShowCreateAccountModal = function()
    {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/admin/community/accounts/create.html',
            controller: 'ModalAccountCreate',
            size: "lg"
        });

        modalInstance.result.then(function(result) {
            if (result == 0) {
                LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, false, LPAccounts.SearchAccountSort.Id, $scope.LoadData);
            }
        });

        return;
    }

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {
            $scope.LoadAttempted = true;

            LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, false, LPAccounts.SearchAccountSort.Id, $scope.LoadData);

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