LPEvents.EventHandler = function() {
    this.Hooks = [];
}

LPEvents.EventHandler.prototype.AddHook = function (callback) {
    var hook = new LPEvents.EventHook(this, callback);

    this.Hooks.push(hook);
    
    return hook;
}

LPEvents.EventHandler.prototype.RemoveHook = function (hook) {
    this.Hooks.splice(this.Hooks.indexOf(hook), 1);

    return;
}

LPEvents.EventHandler.prototype.Invoke = function(sender, args) {
    this.Hooks.forEach(function(hook) {
        hook.Invoke(sender, args);
    });

    return;
}