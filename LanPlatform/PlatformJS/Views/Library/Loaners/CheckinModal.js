LPAngular.controller('ModalLoanerCheckin', function ($uibModalInstance, $scope, account) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 1;

                $scope.Account.CheckoutUser = 0;

                $scope.Account.OnUpdate.Invoke($scope, null);
            }
            else {
                $scope.RequestStatus = -1;
                $scope.ErrorMessage = data.StatusCode;
            }
        }
        else {
            $scope.RequestStatus = -1;
            $scope.ErrorMessage = "Server returned invalid response.";
        }
    }

    $scope.Ok = function () {
        $uibModalInstance.dismiss('accept');
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $.post(LanPlatform.ApiPath + "loaner/" + account.Id + "/checkin", {}, $scope.FinishRequest, "json");
});