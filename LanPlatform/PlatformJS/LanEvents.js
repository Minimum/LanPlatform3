/*
    LAN Events
*/
var LPLanEvents = {};

LPLanEvents.Events = [];

LPLanEvents.GetEvent = function (id, callback) {
    var event = LPEvents.Events[id];

    if (event == null) {
        $.getJSON(LanPlatform.ApiPath + "event/" + id, {}, callback);
    }

    return event;
}

LPLanEvents.AddEvent = function(event) {
    if (LPLanEvents.Events[event.Id] != null) {
        // TODO: Check versioning
    }  
    else {
        LPLanEvents.Events[event.Id] = event;
    }

    return;
}