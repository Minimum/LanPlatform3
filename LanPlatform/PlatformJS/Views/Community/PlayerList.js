LPAngular.controller("RouteCommunityUsers", function ($scope, $location, $routeParams) {
    LPInterface.NavSelect("community");

    $scope.PageSize = 15;
    $scope.CurrentPage = (parseInt($routeParams.page) > 0) ? parseInt($routeParams.page) : 1;

    $scope.LoadStatus = 0;
    $scope.ErrorMessage = "SAMPLE TEXT";

    $scope.Accounts = [];

    $scope.Paginator = new LPPagination.Paginator();
    $scope.Paginator.CurrentPage = $scope.CurrentPage;
    $scope.Paginator.LinkPrefix = "#!community/users/";
    $scope.PageInfo = new LPPagination.PageInfo();

    $scope.ShowUser = function (id) {
        $location.path('#!/account/' + id);
    }

    $scope.GetUsers = function(data) {
        if (data != null && data.Data != null) {
            $scope.Accounts = [];

            for (var x = 0; x < data.Data.Results.length; x++) {
                $scope.Accounts[x] = LPAccounts.InitializeAccount(data.Data.Results[x]);
            }

            // Pageination
            $scope.Paginator.TotalElements = data.Data.TotalResults;
            $scope.Paginator.PageSize = $scope.PageSize;

            $scope.Paginator.Compute();

            $scope.LoadStatus = 1;
        } else {
            $scope.LoadStatus = 2;
        }

        $scope.$apply();
    }

    $scope.LoadError = function() {
        $scope.LoadStatus = 2;

        $scope.$apply();
    }

    LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, true, LPAccounts.SearchAccountSort.LastActive, $scope.GetUsers);
});