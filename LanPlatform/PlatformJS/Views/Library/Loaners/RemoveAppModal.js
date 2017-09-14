LPAngular.controller('ModalLoanerRemoveApp', function ($uibModalInstance, $scope, account, app) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;
    $scope.App = app;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 2;

                $scope.Account.RemoveApp($scope.App);

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

    $scope.Remove = function () {
        if ($scope.RequestStatus < 1) {
            $scope.RequestStatus = 1;

            $.ajax({
                url: LanPlatform.ApiPath + "loaner/" + $scope.Account.Id + "/app/" + $scope.App.Id,
                type: 'DELETE',
                success: $scope.FinishRequest
            });
        }
    }

    $scope.Ok = function () {
        $uibModalInstance.dismiss('accept');
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});