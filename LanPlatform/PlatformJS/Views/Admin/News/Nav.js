LPAngular.controller("AdminNewsNav", function ($scope, $uibModal) {
    $scope.template = "views/admin/news/nav.html";

    if (LPAccounts.LocalAccount != null) {
        // Show commands with permissions
    }

    $scope.UpdateLocalAccount = function (account) {
        if (LPAccounts.LocalAccount != null) {
            // Show commands with permissions
        }
    }

    $scope.ShowStatusCreateModal = function() {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/admin/news/status/create.html',
            controller: 'ModalNewsStatusCreate',
            size: "lg"
        });
    }

});