LPAngular.controller("RouteLibraryLoaners", function ($scope, $uibModal) {
    LPInterface.NavSelect("library");

    $scope.Accounts = [];
    $scope.AccountHooks = [];

    $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);
    $scope.LoggedIn = LPAccounts.LocalAccount != null;

    $scope.LoadAccounts = function (data) {
        if (data.Data != null) {
            var accountCount = data.Data.length;

            for (var x = 0; x < accountCount; x++) {
                $scope.Accounts[x] = new LPApps.LoanerAccount();

                // Load model into object
                $scope.Accounts[x].LoadModel(data.Data[x]);

                // Hook onto accounts
                $scope.AccountHooks[x] = $scope.Accounts[x].OnUpdate.AddHook($scope.UpdateAccountDetails);

                // Update username
                $scope.Accounts[x].GetCheckoutUserName();
            }

            $scope.$apply();
        }
    }

    $scope.UpdateAccountDetails = function(sender, args) {
        $scope.$apply();

        return;
    }

    $scope.$on("$destroy", function () {
        var accountCount = $scope.AccountHooks.length;
        
        for (var x = 0; x < accountCount; x++) {
            $scope.AccountHooks[x].RemoveHook();
        }

        $scope.PermissionUpdateHook.RemoveHook();
    });

    $scope.Checkout = function(account)
    {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkout.html',
            controller: 'ModalLoanerCheckout',
            size: "md",
            resolve: {
                account: function() {
                    return account;
                }
            }
        });

        return;
    }

    $scope.Checkin = function(account) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkin.html',
            controller: 'ModalLoanerCheckin',
            size: "md",
            resolve: {
                account: function () {
                    return account;
                }
            }
        });

        return;
    }

    $scope.ShowCheckin = function(account) {
        var show = false;
        
        if (LPAccounts.LocalAccount != null) {
            show = account.CheckoutUser == LPAccounts.LocalAccount.Id;
        }

        return show;
    }

    $scope.ShowForceCheckin = function (account) {
        var show = false;

        if (LPAccounts.LocalAccount != null) {
            show = account.CheckoutUser != 0 && account.CheckoutUser != LPAccounts.LocalAccount.Id && $scope.AllowEditing;
        }

        return show;
    }

    $scope.ShowCheckout = function (account) {
        return $scope.LoggedIn == true && account.CheckoutUser == 0;
    }

    $scope.UpdatePermissions = function(sender, args) {
        $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);
        $scope.LoggedIn = LPAccounts.LocalAccount != null;

        $scope.$apply();

        return;
    }

    $scope.PermissionUpdateHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);

    LPApps.GetLoanerAccounts($scope.LoadAccounts);
});