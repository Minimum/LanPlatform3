/*
    News
*/
var LPNews = {};

// Events
LPNews.OnStatusChange = new LPEvents.EventHandler();
LPNews.OnWeatherChange = new LPEvents.EventHandler();

LPNews.GetCurrentStatus = function (callback) {
    $.getJSON(LanPlatform.ApiPath + "news/current", {}, callback);

    return;
}

LPNews.GetStatus = function (id) {

}

LPNews.CreateStatus = function(status, callback, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news",
        method: "PUT",
        data: status,
        success: callback,
        error: error
    });

    return;
}

LPNews.GetStatusPage = function(page, callback) {
    $.getJSON(LanPlatform.ApiPath + "news/browse/status/" + page, {}, callback);

    return;
}

LPNews.GetWeather = function(callback) {
    $.getJSON(LanPlatform.ApiPath + "weather/current", {}, callback);

    return;
}