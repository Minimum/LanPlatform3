/*
    News
*/
var LPNews = {};

LPNews.GetCurrentStatus = function (success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news/current",
        method: "GET",
        success: success,
        error: error
    });

    return;
}

LPNews.GetStatus = function (id) {

}

LPNews.CreateStatus = function(status, success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news",
        method: "PUT",
        data: status,
        success: success,
        error: error
    });

    return;
}

LPNews.GetStatusPage = function(page, success) {
    $.getJSON(LanPlatform.ApiPath + "news/browse/status/" + page, {}, success);

    return;
}

LPNews.GetWeather = function (success) {
    $.getJSON(LanPlatform.ApiPath + "weather/current", {}, success);

    return;
}

LPNews.GetActiveLinks = function (success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news/link",
        method: "GET",
        success: success,
        error: error
    });

    return;
}

LPNews.CreateLink = function(link, success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news/link",
        method: "PUT",
        data: link,
        success: success,
        error: error
    });

    return;
}