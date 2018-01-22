var LPAngular = angular.module("LanPlatform", [
    "ngRoute", "ngSanitize", "ui.bootstrap", "rzModule"
]);

/*
    Angular
*/
LPAngular.controller("RouteLibraryMods", function ($scope) {
    LPInterface.NavSelect("library");
    LPInterface.SetupRoute($scope, "RouteLibraryMods");
});

LPAngular.controller("RouteLibraryApps", function ($scope) {
    LPInterface.NavSelect("library");
    LPInterface.SetupRoute($scope, "RouteLibraryApps");
});

LPAngular.controller("RouteLibraryDownloads", function ($scope) {
    LPInterface.NavSelect("library");
    LPInterface.SetupRoute($scope, "RouteLibraryDownloads");
});

LPAngular.controller("RouteCommunityMain", function ($scope) {
    LPInterface.NavSelect("community");
    LPInterface.SetupRoute($scope, "RouteCommunityMain");
});

LPAngular.controller("RouteCommunityLeaderboards", function ($scope) {
    LPInterface.NavSelect("community");
    LPInterface.SetupRoute($scope, "RouteCommunityLeaderboards");
});

LPAngular.controller("RouteAdminMain", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminMain");

    if (LPAccounts.LocalAccount != null && LPAccounts.LocalAccount.IsModerator()) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.controller("RouteAdminCommunityGuestsEdit", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminCommunityGuestsEdit");

    if (LPAccounts.LocalAccount != null && LPAccounts.IsModerator(LPAccounts.LocalAccount)) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.controller("RouteAdminCommunityGuestsCreate", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminCommunityGuestsCreate");

    if (LPAccounts.LocalAccount != null && LPAccounts.IsModerator(LPAccounts.LocalAccount)) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.controller("RouteAdminCommunityGuestsSearch", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminCommunityGuestsSearch");

    if (LPAccounts.LocalAccount != null && LPAccounts.IsModerator(LPAccounts.LocalAccount)) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.directive('suchHref', ['$location', function ($location) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            element.attr('style', 'cursor:pointer');
            element.on('click', function () {
                $location.url(attr.suchHref)
                scope.$apply();
            });
        }
    }
}]);

