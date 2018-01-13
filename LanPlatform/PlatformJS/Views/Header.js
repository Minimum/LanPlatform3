LPAngular.controller("NavProfileController", function ($scope, $location, $uibModal) {
    $scope.LoggedIn = false;
    $scope.FullName = "Hover to login";
    $scope.DisplayName = "Guest";
    $scope.Avatar = LanPlatform.AppPath + "content_fixed/accounts/default_avatar.png";

    $scope.Logout = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/account/logout.html',
            controller: 'ModalAccountLogout',
            size: "md"
        });
    }

    $scope.ViewAccount = function() {
        if (LPAccounts.LocalAccount != null)
        {
            $location.path("account/" + LPAccounts.LocalAccount.Id);
        }
    }

    $scope.Login = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/account/login.html',
            controller: 'ModalAccountLogin',
            size: "md"
        });
    }

    $scope.UpdateLocalAccount = function (sender, account) {
        $scope.LoggedIn = account != null;

        if ($scope.LoggedIn) {
            $scope.FullName = account.FirstName + " " + account.LastName;
            $scope.DisplayName = account.DisplayName;
            $scope.Avatar = account.AvatarUrl;
        } else {
            $scope.FullName = "Hover to login";
            $scope.DisplayName = "Guest";
            $scope.Avatar = LanPlatform.AppPath + "content_fixed/accounts/default_avatar.png";
        }

        $scope.$apply();

        return;
    }

    if (LPAccounts.LocalAccount != null) {
        $scope.UpdateLocalAccount($scope, LPAccounts.LocalAccount);
    }

    LPAccounts.OnLocalAccountChange.AddHook($scope.UpdateLocalAccount);

    LPInterface.NavProfile = $scope;
});

LPAngular.controller("NavButtonController", function ($scope) {
    $scope.Buttons = [];

    $scope.Buttons["home"] = {};
    $scope.Buttons["home"].Enabled = true;
    $scope.Buttons["home"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["library"] = {};
    $scope.Buttons["library"].Enabled = true;
    $scope.Buttons["library"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["ambience"] = {};
    $scope.Buttons["ambience"].Enabled = false;
    $scope.Buttons["ambience"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["community"] = {};
    $scope.Buttons["community"].Enabled = true;
    $scope.Buttons["community"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["admin"] = {};
    $scope.Buttons["admin"].Enabled = LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP");
    $scope.Buttons["admin"].Status = LPInterface.PAGE_INACTIVE;

    $scope.CurrentActiveButton = "";

    $scope.NavClick = function (button) {
        // Don't do anything if the current button is the same
        if ($scope.CurrentActiveButton == button) {
            return;
        }

        // Set previous status to inactive
        if ($scope.Buttons[$scope.CurrentActiveButton] != null) {
            $scope.Buttons[$scope.CurrentActiveButton].Status = LPInterface.PAGE_INACTIVE;
        }

        // Set current status to active
        if ($scope.Buttons[button] != null) {
            $scope.Buttons[button].Status = LPInterface.PAGE_ACTIVE;
            $scope.CurrentActiveButton = button;
        }

        return;
    }

    $scope.SetNavStatus = function(button, status) {
        if ($scope.Buttons[button] != null && $scope.Buttons[button].Status != LPInterface.PAGE_ACTIVE) {
            $scope.Buttons[button].Status = status;
        }

        return;
    }

    $scope.UpdateLocalAccount = function(sender, account) {
        $scope.Buttons["admin"].Enabled = account != null && LPAccounts.CheckLocalPermission("AdminCP");

        $scope.$apply();
    }

    LPAccounts.OnLocalAccountChange.AddHook($scope.UpdateLocalAccount);

    LPInterface.NavButton = $scope;
});