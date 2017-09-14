LPAngular.controller("RouteAdminLibrary", function ($scope, $location) {
    LPInterface.NavSelect("admin");

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {

        }
        else {
            $location.path("accessdenied");
        }
    }

});