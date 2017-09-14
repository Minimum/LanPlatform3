LPEvents.EventHook = function(handler, callback) {
    this.Handler = handler;
    this.Callback = callback;
}

LPEvents.EventHook.prototype.RemoveHook = function() {
    this.Handler.RemoveHook(this);

    return;
}

LPEvents.EventHook.prototype.Invoke = function(sender, args) {
    this.Callback(sender, args);

    return;
}