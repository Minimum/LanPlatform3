LPAngular.controller("RouteAccountView", function ($scope, $location, $routeParams, $sce, $uibModal) {
    LPInterface.NavSelect("community");

    $scope.AccountLoadStatus = 0;

    $scope.AllowEditing = false;

    // Awards
    $scope.AwardSections = [true, false, false];

    $scope.AwardTestHtml = $sce.trustAsHtml("<div style=\"text-align:left\">Pulling The Plug<br/>XP: 50<br/><br/>Successfully kill the UPS Truck.</div>");

    $scope.SelectAwardSection = function (section, sections) {
        sections[0] = false;
        sections[1] = false;
        sections[2] = false;

        sections[section] = true;
    }

    $scope.LastEventField = function () {
        if ($scope.LastEvent != null) {
            return $scope.LastEvent.Name;
        }
        else {
            return "None";
        }
    }

    $scope.OnlineStatusColor = { color: "#e74c3c" };

    $scope.ShowEditInfo = function()
    {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/account/editinfo.html',
            controller: 'ModalAccountEditInfo',
            size: "lg",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });
    }

    $scope.GetAccountDetails = function(data) {
        if (data.Data != null) {
            var account = LPAccounts.InitializeAccount(data.Data);

            LPAccounts.AddAccount(account);

            $scope.UpdateAccountDetails(account);

            $scope.AccountLoaded = 1;
            $scope.AccountChangeHook = account.OnUpdate.AddHook($scope.TargetAccountChanged);
        }
        else {
            $scope.AccountLoaded = 2;
        }

        $scope.$apply();
    }

    $scope.GetEventDetails = function(data) {
        if (data.Data != null) {
            LPLanEvents.AddEvent(data.Data);

            $scope.LastEvent = data.Data;
            $scope.$apply();
        }

        return;
    }

    $scope.UpdateAccountDetails = function(account) {
        $scope.Account = account;
        $scope.OnlineStatusColor = { color: account.IsOnline ? "#00bc8c" : "#605e5e" };
        $scope.AllowEditing = LPAccounts.IsLocalAccount(account) || LPAccounts.CheckLocalPermission("AccountEditBasic", "platform");
        $scope.LastEvent = LPLanEvents.GetEvent(account.LastEvent, $scope.GetEventDetails);
        $scope.Avatar = account.AvatarUrl;

        return;
    }

    $scope.TargetAccountChanged = function (sender, account) {

    }

    $scope.ChangeAvatar = function() {
        
    }

    $scope.$on("$destroy", function () {
        if ($scope.AccountChangeHook != null) {
            $scope.AccountChangeHook.RemoveHook();
        }
    });

    var account = LPAccounts.GetAccount($routeParams.accountId, $scope.GetAccountDetails);

    if (account != null) {
        $scope.UpdateAccountDetails(account);

        $scope.AccountLoaded = 1;
        $scope.AccountChangeHook = account.OnUpdate.AddHook($scope.TargetAccountChanged);
    }
});