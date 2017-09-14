LPAngular.controller("ModalNewsStatusCreate", function ($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";

    $scope.FormTitle = "";
    $scope.FormContentType = 0;

    $scope.FormAppId = 0;

    $scope.SubmissionCallback = function(data) {
        if (data != null) {
            if (data.Status == 0) {
                $uibModalInstance.dismiss('accept');
            }
            else if (data.Status == 1) {
                if (data.StatusCode == "ACCESS_DENIED") {
                    $scope.RequestStatus = 2;

                    $scope.ErrorMessage = "Access denied.";
                }
            }
            else {
                $scope.RequestStatus = 2;

                $scope.ErrorMessage = "Unhandled app error.";
            }
        }
        else {
            $scope.RequestStatus = 2;

            $scope.ErrorMessage = "Invalid server response.";
        }
    }

    $scope.SubmissionFailure = function() {
        $scope.RequestStatus = 2;

        $scope.ErrorMessage = "Unhandled error.";
    }

    $scope.UploadSuccess = function() {
        
    }

    $scope.UploadFailure = function() {
        
    }

    $scope.UploadProgress = function() {
        
    }

    $scope.SubmitStatus = function() {
        // Create status model
        var statusModel = {};

        statusModel.Id = 0;
        statusModel.Title = $scope.FormTitle;
        statusModel.ContentType = $scope.FormContentType;

        var contentModel = {};

        switch ($scope.RequestStatus) {
            case 1:
            {
                contentModel.AppId = $scope.FormAppId;
            }
        }

        statusModel.Content = JSON.stringify(contentModel);

        // Submit model
        LPNews.CreateStatus(statusModel, $scope.SubmissionCallback, $scope.SubmissionFailure);

        return;
    }

    $scope.Ok = function () {
        if ($scope.RequestStatus != 1 && $scope.RequestStatus != 2) {
            $scope.RequestStatus = 1;

            
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});