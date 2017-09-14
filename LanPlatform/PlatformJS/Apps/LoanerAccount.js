LPApps.LoanerAccount = function () {
    this.Id = 0;
    this.Version = 0;

    this.Username = "";
    this.Password = "";
    this.CheckoutUser = 0;
    
    this.Apps = [];
    this.AppCount = 0;

    this.CheckoutUserName = "N/A";

    this.OnUpdate = new LPEvents.EventHandler();
}

LPApps.LoanerAccount.prototype.LoadModel = function (model) {
    this.Id = model.Id;
    this.Version = model.Version;

    this.Username = model.Username;
    this.Password = model.Password;
    this.CheckoutUser = model.CheckoutUser;

    this.AppCount = model.Apps.length;

    for (var x = 0; x < this.AppCount; x++) {
        var app = new LPApps.App();

        app.LoadModel(model.Apps[x]);

        app = LPApps.AddApp(app);

        this.Apps[x] = app;
    }

    return;
}

LPApps.LoanerAccount.prototype.Update = function (loaner) {
    this.Version = loaner.Version;

    this.Username = model.Username;
    this.Password = model.Password;
    this.CheckoutUser = model.CheckoutUser;

    for (var x = 0; x < loaner.AppCount; x++) {
        this.AddApp(loaner.Apps[x]);
    }

    return;
}

LPApps.LoanerAccount.prototype.AddApp = function (app) {
    if (this.GetAppIndex(app) == -1) {
        this.Apps[this.AppCount] = app;

        this.AppCount++;
    }
    
    return;
}

LPApps.LoanerAccount.prototype.RemoveApp = function(app) {
    var id = this.Apps.indexOf(app);

    if (id > -1) {
        this.Apps.splice(id, 1);

        this.AppCount--;
    }

    return;
}

LPApps.LoanerAccount.prototype.GetAppIndex = function (app) {
    var appId = -1;

    for (var x = 0; x < this.AppCount; x++) {
        if (this.Apps[x].Id == app.Id) {
            appId = x;

            break;
        }
    }

    return appId;
}

LPApps.LoanerAccount.prototype.ShowPassword = function () {
    return LPAccounts.LocalAccount != null && LPAccounts.LocalAccount.Id == this.CheckoutUser;
}

LPApps.LoanerAccount.prototype.GetCheckoutUserName = function () {
    var self = this;
    var oldName = this.CheckoutUserName;

    if (this.CheckoutUser > 0) {
        var user = LPAccounts.GetAccount(this.CheckoutUser, function (data) {
            if (data.Data != null) {
                var account = LPAccounts.InitializeAccount(data.Data);

                LPAccounts.AddAccount(account);

                self.CheckoutUserName = account.DisplayName;

                if(oldName != self.CheckoutUserName)
                    self.OnUpdate.Invoke(this, null);
            }
        });

        if (user != null) {
            this.CheckoutUserName = user.DisplayName;

            if (oldName != self.CheckoutUserName)
                this.OnUpdate.Invoke(this, null);
        }
    }
    else {
        this.CheckoutUserName = "N/A";
    }

    return;
}
