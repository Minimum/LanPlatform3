LPAngular.controller("RouteLibraryLoanerView", function ($scope, $location, $routeParams, $uibModal) {
    LPInterface.NavSelect("library");

    $scope.LoadStatus = 0;
    $scope.CheckoutStatus = 0;

    $scope.AllowCheckout = LPAccounts.LocalAccount != null;
    $scope.ForceCheckin = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerCheckout);
    $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);

    $scope.LoadAccount = function(data) {
        if (data != null) {
            if (data.Data != null) {
                var loaner = new LPApps.LoanerAccount();

                loaner.LoadModel(data.Data);

                $scope.Account = LPApps.AddLoanerAccount(loaner);

                $scope.AccountUpdateHook = $scope.Account.OnUpdate.AddHook($scope.UpdateAccount);

                $scope.UpdateCheckoutStatus();

                $scope.LoadStatus = 1;
            }
            else {
                $scope.LoadStatus = -1;
            }
        }
        else {
            $scope.LoadStatus = -1;
        }

        $scope.$apply();

        return;
    }

    $scope.UpdateAccount = function (sender, args) {
        $scope.UpdateCheckoutStatus();

        $scope.$apply();

        return;
    }

    $scope.UpdatePermissions = function(sender, args) {
        $scope.ForceCheckin = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerCheckout);
        $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);

        $scope.$apply();
    }

    $scope.UpdateCheckoutStatus = function() {
        if ($scope.Account.CheckoutUser == 0) {
            $scope.CheckoutStatus = 0;
        }
        else if (LPAccounts.LocalAccount != null && $scope.Account.CheckoutUser == LPAccounts.LocalAccount.Id) {
            $scope.CheckoutStatus = 1;
        }
        else {
            $scope.CheckoutStatus = 2;
        }

        $scope.Account.GetCheckoutUserName();

        return;
    }

    $scope.AddApp = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/addapp.html',
            controller: 'ModalLoanerAddApp',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });

        return;
    }

    $scope.RemoveApp = function (app) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/removeapp.html',
            controller: 'ModalLoanerRemoveApp',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                },
                app: function () {
                    return app;
                }
            }
        });

        return;
    }

    $scope.Checkout = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkout.html',
            controller: 'ModalLoanerCheckout',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });

        return;
    }

    $scope.Checkin = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkin.html',
            controller: 'ModalLoanerCheckin',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });

        return;
    }

    $scope.$on("$destroy", function () {
        if ($scope.AccountUpdateHook != null) {
            $scope.AccountUpdateHook.RemoveHook();
        }

        $scope.PermissionUpdateHook.RemoveHook();
    });

    $scope.Account = LPApps.GetLoanerAccount($routeParams.accountId, $scope.LoadAccount);

    if ($scope.Account != null) {
        $scope.UpdateCheckoutStatus();

        $scope.LoadStatus = 1;

        $scope.AccountUpdateHook = $scope.Account.OnUpdate.AddHook($scope.UpdateAccount);
    }

    $scope.PermissionUpdateHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
});