LPAngular.controller("AdminCommunityNav", function ($scope) {
    $scope.template = "views/admin/community/nav.html";

    if (LPAccounts.LocalAccount != null) {
        // Show commands with permissions
    }

    $scope.UpdateLocalAccount = function(account)
    {
        if (LPAccounts.LocalAccount != null) {
            // Show commands with permissions
        }
    }

});