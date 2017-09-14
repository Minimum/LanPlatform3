LPAccounts.AuthSession = function () {
    this.Id = 0;
    this.Version = 0;
    this.Account = 0;
    this.Active = false;
    this.Key = "";
    this.ExpireDate = "";
}

LPAccounts.AuthSession.prototype.LoadModel = function (model) {
    this.Id = model.Id;
    this.Version = model.Version;
    this.Account = model.Account;
    this.Active = model.Active;
    this.Key = model.Key;
    this.ExpireDate = model.ExpireDate;

    return;
}