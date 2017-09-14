LPAngular.controller('ModalUploadFile', function ($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.PromptTitle = "Upload File";
    $scope.FilePath = "";

    $scope.OnUpload = new LPEvents.EventHandler();

    $scope.FinishUpload = function (data) {
        if (data != null) {
            $scope.OnUpload.Invoke(this, data);

            $uibModalInstance.dismiss('finish');
        }
        else {
            $scope.RequestStatus = 2;
        }
    }

    $scope.Ok = function () {
        // Check if file exists

        if ($scope.RequestStatus != 1) {
            // Show request working status
            $scope.RequestStatus = 1;

            // Upload file
            
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});