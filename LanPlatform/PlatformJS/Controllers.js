LPAngular.config(['$locationProvider', '$routeProvider',
    function config($locationProvider, $routeProvider) {
        $locationProvider.hashPrefix('!');

        $routeProvider.
            when('/', {
                templateUrl: 'views/home/main.html',
                controller: "RouteHomeMain"
            }).

            when('/library', {
                templateUrl: 'views/library/main.html',
                controller: "RouteLibraryMain"
            }).
                when('/library/games', {
                    templateUrl: 'views/library/games.html',
                    controller: "RouteLibraryGames"
                }).
                when('/library/mods', {
                    templateUrl: 'views/library/mods.html',
                    controller: "RouteLibraryMods"
                }).
                when('/library/apps', {
                    templateUrl: 'views/library/apps.html',
                    controller: "RouteLibraryApps"
                }).
                when('/library/loaners', {
                    templateUrl: 'views/library/loaners.html',
                    controller: "RouteLibraryLoaners"
                }).
                    when('/library/loaner/:accountId', {
                        templateUrl: 'views/library/loaners/view.html',
                        controller: "RouteLibraryLoanerView"
                    }).
                when('/library/downloads', {
                    templateUrl: 'views/library/downloads.html',
                    controller: "RouteLibraryDownloads"
                }).

            when('/ambience', {
                templateUrl: 'views/ambience/main.html',
                controller: "RouteAmbienceMain"
            }).
                when('/ambience/lighting', {
                    templateUrl: 'views/ambience/lighting/main.html',
                    controller: "RouteAmbienceLighting"
                }).
                when('/ambience/music', {
                    templateUrl: 'views/ambience/music/main.html',
                    controller: "RouteAmbienceMusic"
                }).

            when('/community', {
                templateUrl: 'views/community/main.html',
                controller: "RouteCommunityMain"
            }).
                when('/community/users', {
                    redirectTo: '/community/users/1'
                }).
                when('/community/users/:page', {
                    templateUrl: 'views/community/users.html',
                    controller: "RouteCommunityUsers"
                }).
                when('/community/chat', {
                    templateUrl: 'views/community/chat.html',
                    controller: "RouteCommunityChat"
                }).
                when('/community/leaderboards', {
                    templateUrl: 'views/community/leaderboards.html',
                    controller: "RouteCommunityLeaderboards"
                }).

            when('/admin', {
                templateUrl: 'views/admin/main.html',
                controller: "RouteAdminMain"
            }).
                when('/admin/news', {
                    templateUrl: 'views/admin/news.html',
                    controller: "RouteAdminNews"
                }).
                    when('/admin/news/status/browse/:page', {
                        templateUrl: 'views/admin/news/status/browse.html',
                        controller: "RouteAdminNewsStatusBrowse"
                    }).
                    when('/admin/news/status/edit/:statusId', {
                        templateUrl: 'views/admin/news/status/edit.html',
                        controller: "RouteAdminNewsStatusEdit"
                    }).
                when('/admin/library', {
                    templateUrl: 'views/admin/library.html',
                    controller: "RouteAdminLibrary"
                }).
                when('/admin/community', {
                    templateUrl: 'views/admin/community.html',
                    controller: "RouteAdminCommunity"
                }).
                    when('/admin/community/accounts/', {
                        templateUrl: 'views/admin/community.html',
                        controller: "RouteAdminCommunity"
                    }).
                    when('/admin/community/accounts/edit/:guestId', {
                        templateUrl: 'views/admin/community/guests/edit.html',
                        controller: "RouteAdminCommunityGuestsEdit"
                    }).
                    when('/admin/community/accounts/create', {
                        templateUrl: 'views/admin/community/guests/create.html',
                        controller: "RouteAdminCommunityGuestsCreate"
                    }).
                    when('/admin/community/accounts/search?phrase&page', {
                        templateUrl: 'views/admin/community/guests/search.html',
                        controller: "RouteAdminCommunityGuestsSearch"
                    }).
                    when('/admin/community/accounts/browse/:page', {
                        templateUrl: 'views/admin/community/accounts/browse.html',
                        controller: "RouteAdminCommunityAccountsBrowse"
                    }).

            when('/account/:accountId', {
                templateUrl: 'views/account/main.html',
                controller: "RouteAccountView"
            }).

            when('/accessdenied', {
                templateUrl: 'views/global/accessdenied.html'
            }).

            otherwise({ redirectTo: '/' });
    }
]);