LPEvents = {};

LPEvents.Events = [];

LPEvents.AddHook = function(eventName, callback)
{
    if (LPEvents.Events[eventName] == null) {
        LPEvents.Events[eventName] = new LPEvents.EventHandler();
    }

    LPEvents.Events[eventName].AddHook(callback);

    return;
}

LPEvents.RemoveHook = function(eventName, hookId) {
    if (LPEvents.Events[eventName] != null) {
        LPEvents.Events[eventName].RemoveHook(hookId);
    }

    return;
}

LPEvents.Invoke = function (eventName, sender, args)
{
    if (LPEvents.Events[eventName] != null) {
        LPEvents.Events[eventName].Invoke(sender, args);
    }

    return;
}