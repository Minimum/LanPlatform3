LPAngular.controller("FooterController", function ($scope) {
    $scope.FooterMode = 0;

    $scope.Version = LanPlatform.VersionName;
    $scope.Song = "Enter The Yakuza Club";
    $scope.Artist = "Hollywood Burns";

    $scope.Position = 0;

    $scope.PositionText = "0:00";
    $scope.LengthText = "5:17";

    $scope.ShowPlay = true;

    $scope.Volume = 80;

    $scope.PositionSlider = {
        floor: 0,
        ceil: 317,
        hidePointerLabels: true,
        hideLimitLabels: true,
        showSelectionBar: true,
        translate: function (value, sliderId, label) {
            var seconds = (value % 60);

            if (seconds < 10) {
                seconds = "0" + seconds;
            }

            $scope.PositionText = Math.floor(value / 60) + ":" + seconds;

            return (value / 60) + ":" + (value % 60);
        }
    };

    $scope.VolumeSlider = {
        floor: 0,
        ceil: 100,
        hidePointerLabels: true,
        hideLimitLabels: true,
        showSelectionBar: true
    };
});