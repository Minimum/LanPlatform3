LPAngular.controller('ModalLoanerCheckout', function ($uibModalInstance, $scope, account) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 1;

                $scope.Account.CheckoutUser = LPAccounts.LocalAccount.Id;

                $scope.Account.OnUpdate.Invoke($scope, null);
            }
            else {
                if (data.StatusCode == "CHECKOUT_LIMIT_HIT") {
                    $scope.RequestStatus = -2;
                }
                else {
                    $scope.RequestStatus = -1;

                    $scope.ErrorMessage = data.StatusCode;
                }
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

    $.post(LanPlatform.ApiPath + "loaner/" + account.Id + "/checkout", {}, $scope.FinishRequest, "json");
});