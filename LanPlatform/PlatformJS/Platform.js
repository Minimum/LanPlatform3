var LanPlatform = {};

LanPlatform.AppBuild = 3000;
LanPlatform.VersionName = "GabionPlatform v3." + LanPlatform.AppBuild;
LanPlatform.AppPath = "http://localhost:45100/";
LanPlatform.ApiPath = LanPlatform.AppPath + "api/";

LanPlatform.BeginInitialize = function () {
    $.getJSON(LanPlatform.ApiPath + "site/init", {}, LanPlatform.Initialize);

    return;
}

LanPlatform.Initialize = function(data) {
    if (data != null) {
        // Initialize systems
        LPNet.Initialize(data);
        LPAccounts.Initialize(data);
        LPApps.Initialize(data);
        LPInterface.Initialize(data);
    }
    else {
        // TODO: If invalid data or error, show warning modal

        // TODO: Consider putting a timer on this
        $.getJSON(LanPlatform.ApiPath + "site/init", {}, LanPlatform.Initialize);
    }

    return;
}

$(document).ready(LanPlatform.BeginInitialize);