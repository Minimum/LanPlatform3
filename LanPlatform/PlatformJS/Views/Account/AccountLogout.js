LPAngular.controller('ModalAccountLogout', function ($uibModalInstance, $scope) {
    $scope.Ok = function () {
        LPAccounts.Logout();

        $uibModalInstance.dismiss('accept');

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});