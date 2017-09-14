LPAccounts.AuthUsername = function() {
    this.Id = 0;
    this.Version = 0;
    this.Account = 0;
    this.Active = false;
    this.Username = "";
    this.Password = "";
}

LPAccounts.AuthUsername.prototype.LoadModel = function(model) {
    this.Id = model.Id;
    this.Version = model.Version;
    this.Account = model.Account;
    this.Active = model.Active;
    this.Username = model.Username;
    this.Password = model.Password;

    return;
}

LPAccounts.AuthUsername.prototype.ToCreateRequest = function() {
    return { Username: this.Username, Password: this.Password };
}