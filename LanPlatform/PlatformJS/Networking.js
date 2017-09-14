/*
*   Networking
*/
var LPNet = {};

LPNet.MessageTypes = [];

LPNet.AppResponseType =
{
    AppTypeMismatch: -3,
    AppVersionMismatch: -2,
    ResponseInvalid: -1,
    ResponseHandled: 0,
    ResponseError: 1,
    AppNotInstalled: 2,
    AppDisabled: 3,
    AppError: 4,
    AccessDenied: 5
}

LPNet.Initialize = function () {

}

LPNet.RegisterMessage = function (name, callback) {
    if (LPNet.MessageTypes[name] == null) {
        LPNet.MessageTypes[name] = new LPEvents.EventHandler();
    }

    return LPNet.MessageTypes[name].AddHook(callback);
}

LPNet.AppResponse = function (data) {
    var response;

    if (data != null && data.AppName != null && data.AppName == "LanPlatform") {
        if (data.AppBuild != null && data.AppBuild == LanPlatform.AppBuild) {
            response = data.AppResponse;
        }
        else {
            response = LPNet.AppResponseType.AppVersionMismatch;
        }
    }
    else {
        response = LPNet.RESPONSE_INVALID;
    }

    return response;
}

LPNet.ReadMessages = function () {

}