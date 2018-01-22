LPChat = {};

LPChat.GetMessages = function (channel, success, error) {
    var call = new LPNet.AjaxCall();

    call.Initialize(success, error);

    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "chat/" + channel + "/recent",
        method: "GET",
        success: call.Success,
        error: call.Error
    });

    return;
}

LPChat.GetChannel = function (channel, success, error) {
    var call = new LPNet.AjaxCall();

    call.Initialize(success, error);

    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "chat/" + channel,
        method: "GET",
        success: call.Success,
        error: call.Error
    });

    return;
}

LPChat.GetChannels = function (success, error) {
    var call = new LPNet.AjaxCall();

    call.Initialize(success, error);

    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "chat",
        method: "GET",
        success: call.Success,
        error: call.Error
    });

    return;
}