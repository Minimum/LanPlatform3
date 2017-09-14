LPAngular.controller("ModalAccountCreate", function($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.ProgressMessage = "";
    $scope.ErrorMessage = "";

    $scope.FormDisplayName = "";
    $scope.FormFirstName = "";
    $scope.FormLastName = "";
    $scope.FormUsername = "";
    $scope.FormPassword = "";

    $scope.AccountCallback = function (data) {
        if (data != null) {
            if (data.Status == 0) {
                $scope.SubmitLogin(data.Data);
            }
            else if (data.Status == 1) {
                $scope.RequestStatus = 2;

                if (data.StatusCode == "ACCESS_DENIED") {
                    $scope.ErrorMessage = "Access denied on account creation.";
                } else {
                    $scope.ErrorMessage = "Unhandled request error on account creation.";
                }
            }
            else {
                $scope.RequestStatus = 2;

                $scope.ErrorMessage = "Unhandled app error on account creation.";
            }
        }
        else {
            $scope.RequestStatus = 2;

            $scope.ErrorMessage = "Invalid server response on account creation.";
        }
    }

    $scope.AccountFail = function () {
        $scope.RequestStatus = 2;

        $scope.ErrorMessage = "Unhandled error on account creation.";
    }

    $scope.LoginCallback = function (data) {
        if (data != null) {
            if (data.Status == 0) {
                $uibModalInstance.close(0);
            }
            else if (data.Status == 1) {
                $scope.RequestStatus = 2;

                if (data.StatusCode == "ACCESS_DENIED") {
                    $scope.ErrorMessage = "Access denied on login creation.";
                }
                else if (data.StatusCode == "USERNAME_EXISTS") {
                    $scope.ErrorMessage = "Login already exists.  Account created without login.";
                } else {
                    $scope.ErrorMessage = "Unhandled request error on login creation.";
                }
            }
            else {
                $scope.RequestStatus = 2;

                $scope.ErrorMessage = "Unhandled app error on login creation.";
            }
        }
        else {
            $scope.RequestStatus = 2;

            $scope.ErrorMessage = "Invalid server response on login creation.";
        }
    }

    $scope.LoginFail = function () {
        $scope.RequestStatus = 2;

        $scope.ErrorMessage = "Unhandled error on login creation.";
    }

    $scope.SubmitAccount = function () {
        $scope.ProgressMessage = "Creating account...";

        // Create model
        var account = new LPAccounts.UserAccount();

        account.DisplayName = $scope.FormDisplayName;
        account.FirstName = $scope.FormFirstName;
        account.LastName = $scope.FormLastName;

        // Submit model
        LPAccounts.CreateAccount(account, $scope.AccountCallback, $scope.AccountFail);

        return;
    }

    $scope.SubmitLogin = function(account) {
        $scope.ProgressMessage = "Creating credentials...";

        // Create model
        var login = new LPAccounts.AuthUsername();

        login.Account = account.Id;
        login.Username = $scope.FormUsername;
        login.Password = $scope.FormPassword;

        // Submit model
        LPAccounts.CreateAuthUsername(login, $scope.LoginCallback, $scope.LoginFail);
        
        return;
    }

    $scope.Ok = function () {
        if ($scope.RequestStatus != 1 && $scope.RequestStatus != 2) {
            $scope.RequestStatus = 1;

            $scope.SubmitAccount();
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});