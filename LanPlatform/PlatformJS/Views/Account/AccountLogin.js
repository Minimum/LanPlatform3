LPAngular.controller('ModalAccountLogin', function ($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.Username = "";
    $scope.Password = "";

    $scope.FinishLogin = function(data) {
        if (data != null && data.Data != null) {
            LPAccounts.Auth(data.Data);

            $uibModalInstance.dismiss('finish');
        }
        else {
            $scope.RequestStatus = 2;
        }
    }

    $scope.Ok = function () {
        if ($scope.RequestStatus != 1)
        {
            // Show request working status
            $scope.RequestStatus = 1;

            // Send account edit request
            LPAccounts.StartLogin($scope.Username, $scope.Password, $scope.FinishLogin);
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});