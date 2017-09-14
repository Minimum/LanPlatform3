/*
*   Accounts
*/
var LPAccounts = {};

LPAccounts.SearchAccountSort =
{
    Id : 0,
    LastActive : 1,
    DisplayName : 2,
    FirstName : 3,
    LastName : 4,
    TotalLans : 5
}

// Account cache
LPAccounts.Accounts = [];
LPAccounts.LocalAccount = null;
LPAccounts.LocalPermissions = [];

LPAccounts.Initialized = false;

// Events
LPAccounts.OnLocalAccountChange = new LPEvents.EventHandler();
LPAccounts.OnPermissionsChange = new LPEvents.EventHandler();

LPAccounts.Initialize = function (data) {
    if (data.Data.LocalUserAccount != null) {
        // Authenticate
        LPAccounts.Auth(data.Data.LocalUserAccount);

        // Set permissions
        LPAccounts.SetLocalPermissions(data.Data.LocalPermissions);
    }

    // Get active users
    var userCount = data.Data.ActiveUsers.length;

    for (var x = 0; x < userCount; x++) {
        //LPAccounts.AddAccount(data.Data.ActiveUsers[x]);
    }

    LPAccounts.Initialized = true;

    return;
}

LPAccounts.IsLocalAccount = function (account) {
    return LPAccounts.LocalAccount != null && LPAccounts.LocalAccount.Id == account.Id;
}

LPAccounts.StartLogin = function (username, password, callback) {
    $.post(LanPlatform.ApiPath + "account/login/user", { Username: username, Password: password }, callback, "json");
    
    return;
}

LPAccounts.Logout = function () {
    $.post(LanPlatform.ApiPath + "account/logout", {}, LPAccounts.FinishLogout, "json");

    return;
}

LPAccounts.FinishLogout = function (data) {
    var status = LPNet.AppResponse(data);

    if (data.Status == LPNet.RESPONSE_HANDLED) {
        LPAccounts.LocalAccount = null;
    }

    LPAccounts.OnLocalAccountChange.Invoke(this, this.LocalAccount);

    return;
}

LPAccounts.BeginAuth = function(data) {
    var status = LPNet.AppResponse(data);
    var success = false;

    if (data.Status == LPNet.RESPONSE_HANDLED) {
        LPAccounts.Auth(data.Data);

        success = true;
    }

    return success;
}

LPAccounts.Auth = function (model) {
    var account = LPAccounts.InitializeAccount(model);

    LPAccounts.LocalAccount = account;
    LPAccounts.AddAccount(account);

    LPAccounts.OnLocalAccountChange.Invoke(LPAccounts, LPAccounts.LocalAccount);

    return;
}

LPAccounts.InitializeAccount = function (model)
{
    var account = new LPAccounts.UserAccount();

    account.LoadModel(model);

    return account;
}

LPAccounts.SetLocalPermissions = function(permissions) {
    LPAccounts.OnPermissionsChange.Invoke(this, permissions);

    LPAccounts.LocalPermissions = permissions;

    return;
}

LPAccounts.GetAccount = function(id, callback) {
    var account = LPAccounts.Accounts[id];

    if (account == null) {
        $.getJSON(LanPlatform.ApiPath + "account/" + id, {}, callback);
    }

    return account;
}

LPAccounts.GetAccountPage = function (page, pageSize, sortDescending, sortProperty, callback) {
    $.getJSON(LanPlatform.ApiPath + "account/search?Page=" + page + "&PageSize=" + pageSize + "&SortBy=" + sortProperty + "&SortDescending=" + sortDescending, {}, callback);

    return;
}

LPAccounts.AddAccount = function (account)
{
    LPAccounts.Accounts[account.Id] = account;

    return;
}

LPAccounts.CreateAccount = function(account, callback, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "account",
        method: "PUT",
        data: account.ToModel(),
        success: callback,
        error: error
    });

    return;
}

LPAccounts.EditAccount = function (model, callback) {
    $.post(LanPlatform.ApiPath + "account/" + model.Id, model, callback, "json");

    return;
}

LPAccounts.CreateAuthUsername = function(account, username, callback, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "account/" + account.Id + "/auth/user",
        method: "PUT",
        data: username.ToCreateRequest(),
        success: callback,
        error: error
    });

    return;
}

LPAccounts.GetAvatarURL = function(account)
{
    var url = "content_fixed/accounts/default_avatar.png";

    if(account.Avatar != 0)
    {
        url = LPContent.GetContent(account.Avatar).GetURL();
    }

    return url;
}

LPAccounts.CheckAdmin = function (account, flag, scope, callback)
{
    // TODO: Add proper flag checking
    var access = account.Root;

    if (!access) {
        
    }

    return access;
}

LPAccounts.CheckLocalPermission = function(flag) {
    return LPAccounts.CheckLocalPermission(flag, "platform");
}

LPAccounts.CheckLocalPermission = function (flag, scope) {
    var success = false;

    if (LPAccounts.LocalAccount != null) {
        if (LPAccounts.LocalAccount.Root) {
            success = true;
        }
        else {
            var len = LPAccounts.LocalPermissions.length;

            for (var x = 0; x < len; x++) {
                if (LPAccounts.LocalPermissions[x].Flag == flag &&
                    LPAccounts.LocalPermissions[x].Scope == scope) {
                    success = true;

                    break;
                }
            }
        }
    }

    return success;
}