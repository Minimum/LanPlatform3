LPAngular.controller("RouteCommunityChat", function ($scope) {
    LPInterface.NavSelect("community");

    $scope.LoadStatus = 1;

    // Channels view
    $scope.ChannelLoad = 0;
    $scope.ChatChannels = [];

    // Chat view
    $scope.ChatHeader = "Chat";
    $scope.ChatMessages = [];

    $scope.LoadMessages = function(data) {
        
    }

    $scope.LoadNewMessages = function(data) {
        
    }

    $scope.LoadChannels = function(data) {
        
    }

    $scope.LoadChannelsFail = function(data) {
        
    }

    $scope.SendMessage = function() {
        
    }

    LPChat.GetChannels($scope.LoadChannels, $scope.LoadChannelsFail);
});