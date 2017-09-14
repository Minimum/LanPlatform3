LPAngular.controller("AdminLibraryNav", function ($scope) {
    $scope.template = "views/admin/library/nav.html";

    if (LPAccounts.LocalAccount != null) {
        // Show commands with permissions
    }

    $scope.UpdateLocalAccount = function (account) {
        if (LPAccounts.LocalAccount != null) {
            // Show commands with permissions
        }
    }

});