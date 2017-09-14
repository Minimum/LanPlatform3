LPAngular.controller('ModalLoanerAddApp', function ($uibModalInstance, $scope, account) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;
    $scope.Apps = LPApps.Apps;          // TODO: Change this to a search based input
    $scope.FormApp = -1;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 2;

                $scope.Account.AddApp($scope.App);

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

    $scope.Add = function () {
        if ($scope.Apps[parseInt($scope.FormApp)] != null && $scope.RequestStatus < 1) {
            $scope.RequestStatus = 1;

            $scope.App = $scope.Apps[parseInt($scope.FormApp)];

            $.ajax({
                url: LanPlatform.ApiPath + "loaner/" + $scope.Account.Id + "/app/" + $scope.App.Id,
                type: 'PUT',
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