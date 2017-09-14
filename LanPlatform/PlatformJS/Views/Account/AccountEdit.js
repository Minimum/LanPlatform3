LPAngular.controller('ModalAccountEditInfo', function($uibModalInstance, $scope, account) {
    $scope.Account = account;
    $scope.FormModel = new LPAccounts.AccountModel($scope.Account);

    $scope.RequestStatus = 0;

    $scope.AdvancedEditAccess = LPAccounts.CheckLocalAdmin("AccountEditAdvanced", "platform");

    $scope.EditResult = function () {
        // TODO: Add fail condition
        $uibModalInstance.close(true);
    }

    $scope.Ok = function() {
        // Show request working status
        $scope.RequestStatus = 1;

        // Send account edit request
        LPAccounts.EditAccount($scope.FormModel, $scope.EditResult);

        return;
    };

    $scope.Cancel = function() {
        $uibModalInstance.dismiss('cancel');
    };
});