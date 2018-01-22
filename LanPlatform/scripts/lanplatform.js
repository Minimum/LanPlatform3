// JSCombiner: Platform.js
var LanPlatform = {};

LanPlatform.AppName = "LanPlatform";
LanPlatform.AppBuild = 3000;
LanPlatform.VersionName = "LanPlatform v3." + LanPlatform.AppBuild;
LanPlatform.AppPath = "http://localhost:45100/";
LanPlatform.ApiPath = LanPlatform.AppPath + "api/";

LanPlatform.BeginInitialize = function () {
    $.getJSON(LanPlatform.ApiPath + "site/init", {}, LanPlatform.Initialize);

    return;
}

LanPlatform.Initialize = function(data) {
    if (data != null) {
        // Initialize systems
        LPNet.Initialize(data);
        LPAccounts.Initialize(data);
        LPApps.Initialize(data);
        LPInterface.Initialize(data);
    }
    else {
        // TODO: If invalid data or error, show warning modal

        // TODO: Consider putting a timer on this
        $.getJSON(LanPlatform.ApiPath + "site/init", {}, LanPlatform.Initialize);
    }

    return;
}

$(document).ready(LanPlatform.BeginInitialize);

// JSCombiner: Networking.js
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

// JSCombiner: AjaxCall.js
LPNet.AjaxCall = function() {
    this.Status = LPNet.AppResponseType.ResponseHandled;
    this.StatusCode = "";

    this.DataType = "";
    this.Data = null;
}

LPNet.AjaxCall.prototype.Initialize = function (success, error) {
    this.OnSuccess = success;
    this.OnError = error;

    return;
}

LPNet.AjaxCall.prototype.Success = function(data) {
    if (data != null) {
        if (data.AppName != LanPlatform.AppName) {
            this.Status = LPNet.AppResponseType.AppTypeMismatch;

            if (this.OnError != null)
                this.OnError(this);
        }
        else if (data.AppBuild != LanPlatform.AppBuild) {
            this.Status = LPNet.AppResponseType.AppVersionMismatch;

            if (this.OnError != null)
                this.OnError(this);
        }
        else {
            this.Status = data.Status;
            this.StatusCode = data.StatusCode;

            this.DataType = data.DataType;
            this.Data = data.Data;

            if (this.Status == LPNet.AppResponseType.ResponseHandled) {
                if (this.OnSuccess != null)
                    this.OnSuccess(this);
            }
            else {
                if (this.OnError != null)
                    this.OnError(this);
            }
        }
    }
    else {
        this.Status = LPNet.AppResponseType.ResponseInvalid;

        if (this.OnError != null)
            this.OnError(this);
    }

    return;
}

LPNet.AjaxCall.prototype.Error = function () {
    this.Status = LPNet.AppResponseType.ResponseInvalid;

    if (this.OnError != null)
        this.OnError(this);

    return;
}

// JSCombiner: Events.js
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

// JSCombiner: EventHandler.js
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

// JSCombiner: EventHook.js
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

// JSCombiner: LanEvents.js
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

// JSCombiner: Accounts.js
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
    //var userCount = data.Data.ActiveUsers.length;

    //for (var x = 0; x < userCount; x++) {
        //LPAccounts.AddAccount(data.Data.ActiveUsers[x]);
    //}

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

LPAccounts.CheckAdmin = function (account, flag, scope, callback, error)
{
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "account/" + account.Id + "/access/" + scope + "/" + flag,
        method: "GET",
        success: callback,
        error: error
    });

    return;
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

// JSCombiner: AccountAdminAccess.js
LPAccounts.AccountAdminAccess = function ()
{
    this.Id = 0;
    this.UserId = 0;
    this.Scope = "";
    this.Flags = "";
}

// JSCombiner: AccountModel.js
LPAccounts.AccountModel = function (account) {
    this.Id = account.Id;
    this.Version = account.Version;

    this.Root = account.Root;
    this.AccountType = account.AccountType;

    this.Gender = account.Gender;
    this.FirstName = account.FirstName;
    this.LastName = account.LastName;
    this.Birthday = account.Birthday;
    this.ContactEmail = account.ContactEmail;
    this.ContactPhone = account.ContactPhone;
    this.ContactFacebook = account.ContactFacebook;
    this.ContactSteam = account.ContactSteam;

    this.TotalEvents = account.TotalEvents;
    this.EventOffset = account.EventOffset;
    this.RemoteEvents = account.RemoteEvents;
    this.LastEvent = account.LastEvent;
    this.DisplayName = account.DisplayName;
    this.CustomUrl = account.CustomUrl;
    this.LastActive = account.LastActive;
    this.Avatar = account.Avatar;
    this.Visibility = account.Visibility;

    //this.TotalAwards = 0;

    this.AwardsEnabled = account.AwardsEnabled;
    this.AwardsXpEnabled = account.AwardsXpEnabled;
    this.AwardsLevel = account.AwardsLevel;
    this.AwardsXp = account.AwardsXp;
}

// JSCombiner: AccountRoleAccess.js
LPAccounts.AccountGroupAccess = function ()
{
    this.Id = 0;
    this.UserId = 0;
    this.GroupId = 0;
}

// JSCombiner: AuthSession.js
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

// JSCombiner: AuthUsername.js
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

// JSCombiner: UserAccount.js
LPAccounts.UserAccount = function ()
{
    this.Id = 0;
    this.Version = 0;

    this.Root = false;
    this.AccountType = LPAccounts.UserAccountType.Player;

    this.Gender = 0;
    this.FirstName = "";
    this.LastName = "";
    this.Birthday = 0;
    this.ContactEmail = "";
    this.ContactPhone = "";
    this.ContactFacebook = "";
    this.ContactSteam = 0;

    this.TotalEvents = 0;
    this.EventOffset = 0;
    this.RemoteEvents = 0;
    this.LastEvent = 0;
    this.DisplayName = "";
    this.CustomUrl = "";
    this.LastActive = 0;
    this.Avatar = 0;
    this.Visibility = 0;

    this.TotalAwards = 0;

    this.AwardsEnabled = true;
    this.AwardsXpEnabled = true;
    this.AwardsLevel = 0;
    this.AwardsXp = 0;

    this.AvatarUrl = "";
    this.IsOnline = false;

    this.OnUpdate = new LPEvents.EventHandler();
}

LPAccounts.UserAccount.prototype.LoadModel = function (model)
{
    if (this.Id != 0)
    {
        return;
    }

    this.Id = model.Id;
    this.Version = model.Version;

    this.Root = model.Root;
    this.AccountType = model.AccountType;

    this.Gender = model.Gender;
    this.FirstName = model.FirstName;
    this.LastName = model.LastName;
    this.Birthday = model.Birthday;
    this.ContactEmail = model.ContactEmail;
    this.ContactPhone = model.ContactPhone;
    this.ContactFacebook = model.ContactFacebook;
    this.ContactSteam = model.ContactSteam;

    this.TotalEvents = model.TotalEvents;
    this.EventOffset = model.EventOffset;
    this.RemoteEvents = model.RemoteEvents;
    this.LastEvent = model.LastEvent;
    this.DisplayName = model.DisplayName;
    this.CustomUrl = model.CustomUrl;
    this.LastActive = model.LastActive;
    this.Avatar = model.Avatar;
    this.Visibility = model.Visibility;

    this.AwardsEnabled = model.AwardsEnabled;
    this.AwardsXpEnabled = model.AwardsXpEnabled;
    this.AwardsLevel = model.AwardsLevel;
    this.AwardsXp = model.AwardsXp;

    this.AvatarUrl = this.GetAvatarURL();
    this.IsOnline = this.LastActive + 900 >= Math.floor(Date.now() / 1000);

    return;
}

LPAccounts.UserAccount.prototype.GetAvatarURL = function ()
{
    return (this.Avatar > 0) ? LanPlatform.ApiPath + "content/data/id/" + this.Avatar : LanPlatform.AppPath + "content_fixed/accounts/default_avatar.png";
}

LPAccounts.UserAccount.prototype.CheckFlag = function (flag)
{

}

LPAccounts.UserAccount.prototype.GetLastActiveTime = function ()
{
    var a = new Date(this.LastActive * 1000);

    var months = ['January','February','March','April','May','June','July','August','September','October','November','December'];
    var year = a.getFullYear();
    var month = months[a.getMonth()];
    var date = a.getDate();
    var hour = a.getHours();
    var min = a.getMinutes();
    var sec = a.getSeconds();
    var suffix = (hour >= 12) ? " PM" : " AM";

    hour = hour % 12;

    if (hour == 0) {
        hour = 12;
    }

    var time = date + ' ' + month + ' ' + year + ((hour < 10) ? " 0" : " ") + hour + ((min < 10) ? ":0" : ":") + min + ((sec < 10) ? ":0" : ":") + sec + suffix;

    return time;
}

LPAccounts.UserAccount.prototype.IsActive = function () {
    return Math.round((new Date()).getTime() / 1000) <= this.LastActive + 1800;
}

LPAccounts.UserAccount.prototype.ToModel = function () {
    return new LPAccounts.AccountModel(this);
}

// Accounts related enums
LPAccounts.AccountVisibility = {
    Visible: 0,
    HiddenFromGuests: 1,
    HiddenFromUsers: 2
};

LPAccounts.UserAccountType = {
    Player: 0,
    Bot: 1
};

LPAccounts.Gender = {
    None: 0,
    Male: 1,
    Female: 2
}

// JSCombiner: Content.js
var LPContent = {};

LPContent.GetContent = function(id, callback) {
    $.getJSON(LanPlatform.ApiPath + "content/" + id, {}, callback);

    return;
}

LPContent.InitializeContent = function(model) {
    var content = new LPContent.ContentItem();

    content.LoadModel(model);

    return content;
}

LPContent.UploadContent = function(data) {
    
}

// JSCombiner: ContentAccess.js
LPContent.ContentAccessType = {
    None: 0,
    User: 1,
    Group: 2
}

LPContent.ContentAccess = function ()
{
    this.Id = 0;
    this.Version = 0;
    this.Content = 0;
    this.Type = LPContent.ContentAccessType.None;
    this.User = 0;
}



// JSCombiner: ContentItem.js
LPContent.ContentType = {
    None: 0,
    Binary: 1,
    ImageBmp: 2,
    ImageGif: 3,
    ImageJpg: 4,
    ImagePng: 5,
    ImageSvg: 6
}

LPContent.ContentItem = function ()
{
    this.Id = 0;
    this.Version = 0;
    this.Author = 0;
    this.Hash = "";
    this.Filename = "";
    this.Size = 0;
    this.Type = LPContent.ContentType.None;
    this.TimeAdded = 0;
}

LPContent.ContentItem.prototype.LoadModel = function (model) {
    if (this.Id != 0) {
        return;
    }

    this.Id = model.Id;
    this.Version = model.Version;
    this.Author = model.Author;
    this.Hash = model.Hash;
    this.Filename = model.Filename;
    this.Size = model.Size;
    this.Type = model.Type;
    this.TimeAdded = model.TimeAdded;

    return;
}

// JSCombiner: ContentUpload.js
LPContent.ContentUpload = function (file) {
    this.File = file;

    this.Status = LPContent.UploadStatus.Waiting;
    this.StatusCode = "";

    this.ProgressPercent = 0;
    this.ProgressSize = 0;
    this.TotalSize = 0;

    this.OnProgressUpdate = new LPEvents.EventHandler();
    this.OnSuccess = new LPEvents.EventHandler();
    this.OnFailure = new LPEvents.EventHandler();
};

LPContent.ContentUpload.prototype.GetType = function () {
    return this.File.type;
};

LPContent.ContentUpload.prototype.GetSize = function () {
    return this.File.size;
};

LPContent.ContentUpload.prototype.GetName = function () {
    return this.File.name;
};

LPContent.ContentUpload.prototype.Start = function () {
    var formData = new FormData();

    // add assoc key values, this will be posts values
    formData.append("file", this.File, this.getName());
    formData.append("upload_file", true);

    this.Status = LPContent.UploadStatus.Running;

    $.ajax({
        type: "POST",
        url: LanPlatform.ApiPath + "content/upload",
        xhr: this.XhrHandling,
        success: this.SuccessHandling,
        error: this.FailureHandling,
        async: true,
        data: formData,
        cache: false,
        contentType: false,
        processData: false,
        timeout: 60000
    });

    return;
};

LPContent.ContentUpload.prototype.XhrHandling = function() {
    var myXhr = $.ajaxSettings.xhr();

    if (myXhr.upload) {
        myXhr.upload.addEventListener('progress', this.ProgressHandling, false);
    }

    return myXhr;
}

LPContent.ContentUpload.prototype.SuccessHandling = function (data) {
    this.Status = LPContent.UploadStatus.Complete;

    this.OnSuccess.Invoke(this, data);

    return;
}

LPContent.ContentUpload.prototype.FailureHandling = function(jqXhr, exception) {
    this.Status = LPContent.UploadStatus.Failed;

    if (jqXhr.status === 0) {
        this.StatusCode = 'Could not connect.';
    } else if (jqXhr.status == 409) {
        this.StatusCode = 'Invalid upload.';
    } else if (jqXhr.status == 401) {
        this.StatusCode = 'Access denied.';
    } else if (jqXhr.status == 404) {
        this.StatusCode = 'Requested page not found. [404]';
    } else if (jqXhr.status == 500) {
        this.StatusCode = 'Internal Server Error [500].';
    } else if (exception === 'timeout') {
        this.StatusCode = 'Time out error.';
    } else if (exception === 'abort') {
        this.StatusCode = 'Ajax request aborted.';
    } else {
        this.StatusCode = 'Uncaught Error.\n' + jqXhr.responseText;
    }

    this.OnFailure.Invoke(this, error);

    return;
}

LPContent.ContentUpload.prototype.ProgressHandling = function (event) {
    this.ProgressSize = event.loaded || event.position;
    this.TotalSize = event.total;

    if (event.lengthComputable) {
        this.ProgressPercent = Math.ceil(this.ProgressSize / this.TotalSize * 100);
    }

    this.OnProgressUpdate.Invoke(this, this.ProgressPercent);

    return;
};

LPContent.UploadStatus = {
    Waiting: 0,
    Running: 1,
    Complete: 2,
    Failed: 3
}

// JSCombiner: Apps.js
LPApps = {};

LPApps.SearchAppSort =
{
    Id : 0,
    AppType : 1,
    Title : 2,
    Description : 3,
    DownloadType : 4
}

LPApps.Loaners = [];
LPApps.Apps = [];

LPApps.FlagAppEdit = "AppEdit";
LPApps.FlagLoanerEdit = "AppLoanerEdit";
LPApps.FlagLoanerCheckout = "AppLoanerCheckout";

LPApps.Initialize = function (data) {
    if (data.Data.LoanerAccounts != null) {
        var loanerCount = data.Data.LoanerAccounts.length;

        for (var x = 0; x < loanerCount; x++) {
            var loaner = new LPApps.LoanerAccount();

            loaner.LoadModel(data.Data.LoanerAccounts[x]);

            LPApps.AddLoanerAccount(loaner);
        }
    }
    
    if (data.Data.Apps != null) {
        var appCount = data.Data.Apps.length;

        for (var x = 0; x < appCount; x++) {
            var app = new LPApps.App();

            app.LoadModel(data.Data.Apps[x]);

            LPApps.AddApp(app);
        }
    }

    LPApps.OnSteamCodeReceived = new LPEvents.EventHandler();

    return;
}

LPApps.GetApp = function (id, callback, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "app/" + id,
        method: "GET",
        success: callback,
        error: error,
        timeout: 5000
    });

    return;
}

LPApps.GetAppLoaners = function(id, callback, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "app/" + id + "/loaner",
        method: "GET",
        success: callback,
        error: error,
        timeout: 5000
    });

    return;
}

LPApps.GetAppPage = function(page, pageSize, sortDescending, sortProperty, callback) {
    $.getJSON(LanPlatform.ApiPath + "app/search?Page=" + page + "&PageSize=" + pageSize + "&SortBy=" + sortProperty + "&SortDescending=" + sortDescending, {}, callback);

    return;
}

LPApps.AddApp = function (app) {
    var result = LPApps.Apps[app.Id];

    if (result == null) {
        LPApps.Apps[app.Id] = app;

        result = app;
    }
    else if (LPApps.Apps[app.Id].VERSION < app.Version) {
        LPApps.Apps[app.Id].Update(app);
    }

    return result;
}

LPApps.GetLoanerAccounts = function (callback) {
    $.getJSON(LanPlatform.ApiPath + "loaner/all", {}, callback);

    return;
}

LPApps.GetLoanerAccount = function (id, callback) {
    var account = LPApps.Loaners[id];

    if (account == null) {
        $.getJSON(LanPlatform.ApiPath + "loaner/" + id, {}, callback);
    }

    return account;
}

LPApps.AddLoanerAccount = function (loaner) {
    var result = LPApps.Loaners[loaner.Id];

    if (result == null) {
        LPApps.Loaners[loaner.Id] = loaner;

        result = loaner;
    }
    else if (LPApps.Loaners[loaner.Id].Version < loaner.Version) {
        LPApps.Loaners[loaner.Id].Update(loaner);
    }

    return result;
}

// JSCombiner: App.js
LPApps.App = function () {
    this.Id = 0;
    this.Version = 0;

    this.Type = LPApps.AppType.None;
    this.Title = "";
    this.Description = "";
    this.DownloadType = LPApps.AppDownloadType.None;
    this.DownloadInfo = "";

    this.OnUpdate = new LPEvents.EventHandler();
}

LPApps.App.prototype.LoadModel = function(model) {
    this.Id = model.Id;
    this.Version = model.Version;

    this.Type = model.Type;
    this.Title = model.Title;
    this.Description = model.Description;
    this.DownloadType = model.DownloadType;
    this.DownloadInfo = model.DownloadInfo;

    return;
}

LPApps.App.prototype.Update = function(app) {
    this.Version = app.Version;

    this.Type = app.Type;
    this.Title = app.Title;
    this.Description = app.Description;
    this.DownloadType = app.DownloadType;
    this.DownloadInfo = app.DownloadInfo;

    return;
}

LPApps.App.prototype.GetTypeName = function () {
    var name = "None";

    switch(this.Type) {
        case LPApps.AppType.App:
        {
            name = "Application";

            break;
        }

        case LPApps.AppType.Game:
        {
            name = "Game";

            break;
        }

        case LPApps.AppType.Mod:
        {
            name = "Modification";

            break;
        }
    }

    return name;
}

LPApps.App.prototype.GetDownloadTitle = function() {
    var title = "None";

    switch (this.DownloadType) {
        case LPApps.AppDownloadType.Url:
            {
                title = "External URL";

                break;
            }

        case LPApps.AppDownloadType.Steam:
            {
                title = "Steam";

                break;
            }

        case LPApps.AppDownloadType.Content:
            {
                title = "Download";

                break;
            }
    }

    return title;
}

LPApps.App.prototype.GetDownloadLink = function() {
    var link = "None";

    switch (this.DownloadType) {
        case LPApps.AppDownloadType.Url:
            {
                link = this.DownloadInfo;

                break;
            }

        case LPApps.AppDownloadType.Steam:
            {
                link = "steam://install/" + this.DownloadInfo;

                break;
            }

        case LPApps.AppDownloadType.Content:
            {
                // TODO: this
                link = "SAMPLE TEXT";

                break;
            }
    }

    return link;
}

LPApps.AppType = {
    None : 0,
    App : 1,
    Game : 2,
    Mod : 3
}

LPApps.AppDownloadType = {
    None : 0,
    Url : 1,
    Steam : 2,
    Content : 3
}

// JSCombiner: LoanerAccount.js
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


// JSCombiner: Interface.js
/*
    UI
*/
var LPInterface = {};

LPInterface.PAGE_INACTIVE = "";
LPInterface.PAGE_UPDATE = "update";
LPInterface.PAGE_ALERT = "alert";
LPInterface.PAGE_URGENT = "urgent";
LPInterface.PAGE_ACTIVE = "active";

LPInterface.StatusNames = ["", "update", "alert", "urgent", "active"];

LPInterface.Initialize = function () {

}

LPInterface.NavSelect = function (section) {
    LPInterface.NavButton.NavClick(section);

    return;
}

LPInterface.SetSectionStatus = function (sectionName, status) {
    LPInterface.NavButton.SetNavStatus(sectionName, status);

    return;
}

// JSCombiner: News.js
/*
    News
*/
var LPNews = {};

LPNews.GetCurrentStatus = function (success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news/current",
        method: "GET",
        success: success,
        error: error
    });

    return;
}

LPNews.GetStatus = function (id) {

}

LPNews.CreateStatus = function(status, success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news",
        method: "PUT",
        data: status,
        success: success,
        error: error
    });

    return;
}

LPNews.GetStatusPage = function(page, success) {
    $.getJSON(LanPlatform.ApiPath + "news/browse/status/" + page, {}, success);

    return;
}

LPNews.GetWeather = function (success) {
    $.getJSON(LanPlatform.ApiPath + "weather/current", {}, success);

    return;
}

LPNews.GetActiveLinks = function (success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news/link",
        method: "GET",
        success: success,
        error: error
    });

    return;
}

LPNews.CreateLink = function(link, success, error) {
    $.ajax({
        dataType: "json",
        url: LanPlatform.ApiPath + "news/link",
        method: "PUT",
        data: link,
        success: success,
        error: error
    });

    return;
}

// JSCombiner: QuickLink.js
LPNews.QuickLink = function () {
    this.Id = 0;
    this.Version = 0;

    this.Title = "";
    this.Link = "";
    this.LinkType = 0;
    this.Local = false;
}

LPNews.QuickLink.prototype.LoadModel = function(model) {
    this.Id = model.Id;
    this.Version = model.Version;

    this.Title = model.Title;
    this.Link = model.Link;
    this.LinkType = model.LinkType;
    this.Local = model.Local;

    return;
}

LPNews.QuickLink.prototype.GetLinkTarget = function() {
    var target = "_self";

    // New Window
    if (this.LinkType == 2) {
        target = "_blank";
    }
    
    return target;
}

// JSCombiner: WeatherStatus.js
LPNews.WeatherStatus = function() {
    // Current
    this.CurrentTemperature = 0;
    this.CurrentWeatherType = 0;
    this.CurrentRainChance = 0;
    this.CurrentTime = 0;

    // Future
    this.FirstTemperature = 0;
    this.FirstWeatherType = 0;
    this.FirstRainChance = 0;
    this.FirstTime = 0;

    this.SecondTemperature = 0;
    this.SecondWeatherType = 0;
    this.SecondRainChance = 0;
    this.SecondTime = 0;

    this.ThirdTemperature = 0;
    this.ThirdWeatherType = 0;
    this.ThirdRainChance = 0;
    this.ThirdTime = 0;

    // Daily
    this.DailyRainChance = 0;
    this.DailyHigh = 0;
    this.DailyLow = 0;

    this.Sunrise = 0;
    this.Sunset = 0;
}

LPNews.WeatherStatus.prototype.LoadModel = function(model) {
    // Current
    this.CurrentTemperature = model.CurrentTemperature;
    this.CurrentWeatherType = model.CurrentWeatherType;
    this.CurrentRainChance = model.CurrentRainChance;
    this.CurrentTime = model.CurrentTime;

    // Future
    this.FirstTemperature = model.FirstTemperature;
    this.FirstWeatherType = model.FirstWeatherType;
    this.FirstRainChance = model.FirstRainChance;
    this.FirstTime = model.FirstTime;

    this.SecondTemperature = model.SecondTemperature;
    this.SecondWeatherType = model.SecondWeatherType;
    this.SecondRainChance = model.SecondRainChance;
    this.SecondTime = model.SecondTime;

    this.ThirdTemperature = model.ThirdTemperature;
    this.ThirdWeatherType = model.ThirdWeatherType;
    this.ThirdRainChance = model.ThirdRainChance;
    this.ThirdTime = model.ThirdTime;

    // Daily
    this.DailyRainChance = model.DailyRainChance;
    this.DailyHigh = model.DailyHigh;
    this.DailyLow = model.DailyLow;

    this.Sunrise = model.Sunrise;
    this.Sunset = model.Sunset;

    return;
}

LPNews.WeatherStatus.prototype.GetFirstTime = function() {
    var a = new Date(this.FirstTime * 1000);

    var hour = a.getHours();
    var min = a.getMinutes();
    var suffix = (hour >= 12) ? " PM" : " AM";

    hour = hour % 12;

    if (hour == 0) {
        hour = 12;
    }

    var time = hour + ((min < 10) ? ":0" : ":") + min + suffix;

    return time;
}

// JSCombiner: Chat.js
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

// JSCombiner: Channel.js


// JSCombiner: Message.js


// JSCombiner: Angular.js
var LPAngular = angular.module("LanPlatform", [
    "ngRoute", "ngSanitize", "ui.bootstrap", "rzModule"
]);

/*
    Angular
*/
LPAngular.controller("RouteLibraryMods", function ($scope) {
    LPInterface.NavSelect("library");
    LPInterface.SetupRoute($scope, "RouteLibraryMods");
});

LPAngular.controller("RouteLibraryApps", function ($scope) {
    LPInterface.NavSelect("library");
    LPInterface.SetupRoute($scope, "RouteLibraryApps");
});

LPAngular.controller("RouteLibraryDownloads", function ($scope) {
    LPInterface.NavSelect("library");
    LPInterface.SetupRoute($scope, "RouteLibraryDownloads");
});

LPAngular.controller("RouteCommunityMain", function ($scope) {
    LPInterface.NavSelect("community");
    LPInterface.SetupRoute($scope, "RouteCommunityMain");
});

LPAngular.controller("RouteCommunityLeaderboards", function ($scope) {
    LPInterface.NavSelect("community");
    LPInterface.SetupRoute($scope, "RouteCommunityLeaderboards");
});

LPAngular.controller("RouteAdminMain", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminMain");

    if (LPAccounts.LocalAccount != null && LPAccounts.LocalAccount.IsModerator()) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.controller("RouteAdminCommunityGuestsEdit", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminCommunityGuestsEdit");

    if (LPAccounts.LocalAccount != null && LPAccounts.IsModerator(LPAccounts.LocalAccount)) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.controller("RouteAdminCommunityGuestsCreate", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminCommunityGuestsCreate");

    if (LPAccounts.LocalAccount != null && LPAccounts.IsModerator(LPAccounts.LocalAccount)) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.controller("RouteAdminCommunityGuestsSearch", function ($scope, $location) {
    LPInterface.NavSelect("admin");
    LPInterface.SetupRoute($scope, "RouteAdminCommunityGuestsSearch");

    if (LPAccounts.LocalAccount != null && LPAccounts.IsModerator(LPAccounts.LocalAccount)) {

    }
    else {
        $location.path("accessdenied");
    }
});

LPAngular.directive('suchHref', ['$location', function ($location) {
    return {
        restrict: 'A',
        link: function (scope, element, attr) {
            element.attr('style', 'cursor:pointer');
            element.on('click', function () {
                $location.url(attr.suchHref)
                scope.$apply();
            });
        }
    }
}]);



// JSCombiner: Footer.js
LPAngular.controller("FooterController", function ($scope) {
    $scope.FooterMode = 0;

    $scope.Version = LanPlatform.VersionName;
});

// JSCombiner: Header.js
LPAngular.controller("NavProfileController", function ($scope, $location, $uibModal) {
    $scope.LoggedIn = false;
    $scope.FullName = "Hover to login";
    $scope.DisplayName = "Guest";
    $scope.Avatar = LanPlatform.AppPath + "content_fixed/accounts/default_avatar.png";

    $scope.Logout = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/account/logout.html',
            controller: 'ModalAccountLogout',
            size: "md"
        });
    }

    $scope.ViewAccount = function() {
        if (LPAccounts.LocalAccount != null)
        {
            $location.path("account/" + LPAccounts.LocalAccount.Id);
        }
    }

    $scope.Login = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/account/login.html',
            controller: 'ModalAccountLogin',
            size: "md"
        });
    }

    $scope.UpdateLocalAccount = function (sender, account) {
        $scope.LoggedIn = account != null;

        if ($scope.LoggedIn) {
            $scope.FullName = account.FirstName + " " + account.LastName;
            $scope.DisplayName = account.DisplayName;
            $scope.Avatar = account.AvatarUrl;
        } else {
            $scope.FullName = "Hover to login";
            $scope.DisplayName = "Guest";
            $scope.Avatar = LanPlatform.AppPath + "content_fixed/accounts/default_avatar.png";
        }

        $scope.$apply();

        return;
    }

    if (LPAccounts.LocalAccount != null) {
        $scope.UpdateLocalAccount($scope, LPAccounts.LocalAccount);
    }

    LPAccounts.OnLocalAccountChange.AddHook($scope.UpdateLocalAccount);

    LPInterface.NavProfile = $scope;
});

LPAngular.controller("NavButtonController", function ($scope) {
    $scope.Buttons = [];

    $scope.Buttons["home"] = {};
    $scope.Buttons["home"].Enabled = true;
    $scope.Buttons["home"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["library"] = {};
    $scope.Buttons["library"].Enabled = true;
    $scope.Buttons["library"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["ambience"] = {};
    $scope.Buttons["ambience"].Enabled = false;
    $scope.Buttons["ambience"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["community"] = {};
    $scope.Buttons["community"].Enabled = true;
    $scope.Buttons["community"].Status = LPInterface.PAGE_INACTIVE;

    $scope.Buttons["admin"] = {};
    $scope.Buttons["admin"].Enabled = LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP");
    $scope.Buttons["admin"].Status = LPInterface.PAGE_INACTIVE;

    $scope.CurrentActiveButton = "";

    $scope.NavClick = function (button) {
        // Don't do anything if the current button is the same
        if ($scope.CurrentActiveButton == button) {
            return;
        }

        // Set previous status to inactive
        if ($scope.Buttons[$scope.CurrentActiveButton] != null) {
            $scope.Buttons[$scope.CurrentActiveButton].Status = LPInterface.PAGE_INACTIVE;
        }

        // Set current status to active
        if ($scope.Buttons[button] != null) {
            $scope.Buttons[button].Status = LPInterface.PAGE_ACTIVE;
            $scope.CurrentActiveButton = button;
        }

        return;
    }

    $scope.SetNavStatus = function(button, status) {
        if ($scope.Buttons[button] != null && $scope.Buttons[button].Status != LPInterface.PAGE_ACTIVE) {
            $scope.Buttons[button].Status = status;
        }

        return;
    }

    $scope.UpdateLocalAccount = function(sender, account) {
        $scope.Buttons["admin"].Enabled = account != null && LPAccounts.CheckLocalPermission("AdminCP");

        $scope.$apply();
    }

    LPAccounts.OnLocalAccountChange.AddHook($scope.UpdateLocalAccount);

    LPInterface.NavButton = $scope;
});

// JSCombiner: AccountEdit.js
LPAngular.controller('ModalAccountEditInfo', function($uibModalInstance, $scope, account) {
    $scope.Account = account;
    $scope.FormModel = new LPAccounts.AccountModel($scope.Account);

    $scope.RequestStatus = 0;

    $scope.AdvancedEditAccess = LPAccounts.CheckLocalAdmin("AccountEditAdvanced", "platform");

    $scope.EditResult = function () {
        // TODO: Add fail condition
        $uibModalInstance.close(true);
    }

    $scope.Ok = function() {
        // Show request working status
        $scope.RequestStatus = 1;

        // Send account edit request
        LPAccounts.EditAccount($scope.FormModel, $scope.EditResult);

        return;
    };

    $scope.Cancel = function() {
        $uibModalInstance.dismiss('cancel');
    };
});

// JSCombiner: AccountLogin.js
LPAngular.controller('ModalAccountLogin', function ($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.Username = "";
    $scope.Password = "";

    $scope.FinishLogin = function(data) {
        if (data != null && data.Data != null) {
            LPAccounts.Auth(data.Data);

            $uibModalInstance.dismiss('finish');
        }
        else {
            $scope.RequestStatus = 2;
        }
    }

    $scope.Ok = function () {
        if ($scope.RequestStatus != 1)
        {
            // Show request working status
            $scope.RequestStatus = 1;

            // Send account edit request
            LPAccounts.StartLogin($scope.Username, $scope.Password, $scope.FinishLogin);
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

// JSCombiner: AccountLogout.js
LPAngular.controller('ModalAccountLogout', function ($uibModalInstance, $scope) {
    $scope.Ok = function () {
        LPAccounts.Logout();

        $uibModalInstance.dismiss('accept');

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

// JSCombiner: AccountView.js
LPAngular.controller("RouteAccountView", function ($scope, $location, $routeParams, $sce, $uibModal) {
    LPInterface.NavSelect("community");

    $scope.AccountLoadStatus = 0;

    $scope.AllowEditing = false;

    // Awards
    $scope.AwardSections = [true, false, false];

    $scope.AwardTestHtml = $sce.trustAsHtml("<div style=\"text-align:left\">Pulling The Plug<br/>XP: 50<br/><br/>Successfully kill the UPS Truck.</div>");

    $scope.SelectAwardSection = function (section, sections) {
        sections[0] = false;
        sections[1] = false;
        sections[2] = false;

        sections[section] = true;
    }

    $scope.LastEventField = function () {
        if ($scope.LastEvent != null) {
            return $scope.LastEvent.Name;
        }
        else {
            return "None";
        }
    }

    $scope.OnlineStatusColor = { color: "#e74c3c" };

    $scope.ShowEditInfo = function()
    {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/account/editinfo.html',
            controller: 'ModalAccountEditInfo',
            size: "lg",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });
    }

    $scope.GetAccountDetails = function(data) {
        if (data.Data != null) {
            var account = LPAccounts.InitializeAccount(data.Data);

            LPAccounts.AddAccount(account);

            $scope.UpdateAccountDetails(account);

            $scope.AccountLoaded = 1;
            $scope.AccountChangeHook = account.OnUpdate.AddHook($scope.TargetAccountChanged);
        }
        else {
            $scope.AccountLoaded = 2;
        }

        $scope.$apply();
    }

    $scope.GetEventDetails = function(data) {
        if (data.Data != null) {
            LPLanEvents.AddEvent(data.Data);

            $scope.LastEvent = data.Data;
            $scope.$apply();
        }

        return;
    }

    $scope.UpdateAccountDetails = function(account) {
        $scope.Account = account;
        $scope.OnlineStatusColor = { color: account.IsOnline ? "#00bc8c" : "#605e5e" };
        $scope.AllowEditing = LPAccounts.IsLocalAccount(account) || LPAccounts.CheckLocalPermission("AccountEditBasic", "platform");
        $scope.LastEvent = LPLanEvents.GetEvent(account.LastEvent, $scope.GetEventDetails);
        $scope.Avatar = account.AvatarUrl;

        return;
    }

    $scope.TargetAccountChanged = function (sender, account) {

    }

    $scope.ChangeAvatar = function() {
        
    }

    $scope.$on("$destroy", function () {
        if ($scope.AccountChangeHook != null) {
            $scope.AccountChangeHook.RemoveHook();
        }
    });

    var account = LPAccounts.GetAccount($routeParams.accountId, $scope.GetAccountDetails);

    if (account != null) {
        $scope.UpdateAccountDetails(account);

        $scope.AccountLoaded = 1;
        $scope.AccountChangeHook = account.OnUpdate.AddHook($scope.TargetAccountChanged);
    }
});

// JSCombiner: AwardGroup.js


// JSCombiner: Community.js
LPAngular.controller("RouteAdminCommunity", function ($scope, $location) {
    LPInterface.NavSelect("admin");

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {

        }
        else {
            $location.path("accessdenied");
        }
    }
});

// JSCombiner: Library.js
LPAngular.controller("RouteAdminLibrary", function ($scope, $location) {
    LPInterface.NavSelect("admin");

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {

        }
        else {
            $location.path("accessdenied");
        }
    }

});

// JSCombiner: News.js
LPAngular.controller("RouteAdminNews", function ($scope, $location) {
    LPInterface.NavSelect("admin");

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {

        }
        else {
            $location.path("accessdenied");
        }
    }

});

// JSCombiner: Nav.js
LPAngular.controller("AdminCommunityNav", function ($scope) {
    $scope.template = "views/admin/community/nav.html";

    if (LPAccounts.LocalAccount != null) {
        // Show commands with permissions
    }

    $scope.UpdateLocalAccount = function(account)
    {
        if (LPAccounts.LocalAccount != null) {
            // Show commands with permissions
        }
    }

});

// JSCombiner: AddUsername.js


// JSCombiner: Browse.js
LPAngular.controller("RouteAdminCommunityAccountsBrowse", function ($scope, $location, $routeParams, $uibModal) {
    LPInterface.NavSelect("admin");

    $scope.PageSize = 15;

    $scope.LoadAttempted = false;

    $scope.CurrentPage = (parseInt($routeParams.page) > 0) ? parseInt($routeParams.page) : 1;
    $scope.LoadStatus = 0;
    $scope.ErrorMessage = "";

    $scope.Accounts = [];

    $scope.Paginator = new LPPagination.Paginator();
    $scope.Paginator.CurrentPage = $scope.CurrentPage;
    $scope.Paginator.LinkPrefix = "#!admin/community/accounts/browse/";
    $scope.PageInfo = new LPPagination.PageInfo();

    $scope.LoadData = function (data)
    {
        if (data != null && data.Data != null) {
            $scope.Accounts = [];

            for (var x = 0; x < data.Data.Results.length; x++) {
                $scope.Accounts[x] = LPAccounts.InitializeAccount(data.Data.Results[x]);
            }

            // Pageination
            $scope.Paginator.TotalElements = data.Data.TotalResults;
            $scope.Paginator.PageSize = $scope.PageSize;

            $scope.Paginator.Compute();

            $scope.LoadStatus = 1;
        }
        else {
            if (data != null) {
                if (data.StatusCode == "DAO_ERROR") {
                    $scope.ErrorMessage = "The database has encountered an error.";
                }
            }

            $scope.LoadStatus = 2;
        }

        $scope.$apply();

        return;
    }

    $scope.UpdatePermissions = function () {
        if (LPAccounts.CheckLocalPermission("AdminCP")) {
            if ($scope.LoadAttempted == false) {
                LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, false, LPAccounts.SearchAccountSort.Id, $scope.LoadData);

                $scope.LoadAttempted = true;
            }
        }
        else {
            $location.path("accessdenied");
        }

        return;
    }

    $scope.ShowCreateAccountModal = function()
    {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/admin/community/accounts/create.html',
            controller: 'ModalAccountCreate',
            size: "lg"
        });

        modalInstance.result.then(function(result) {
            if (result == 0) {
                LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, false, LPAccounts.SearchAccountSort.Id, $scope.LoadData);
            }
        });

        return;
    }

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {
            $scope.LoadAttempted = true;

            LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, false, LPAccounts.SearchAccountSort.Id, $scope.LoadData);

            $scope.PermissionHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
        }
        else {
            $location.path("accessdenied");
        }
    }
    else {
        $scope.PermissionHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
    }

    $scope.$on("$destroy", function () {
        if ($scope.PermissionHook != null) {
            LPAccounts.OnPermissionsChange.RemoveHook($scope.PermissionHook);
        }
    });
});

// JSCombiner: Create.js
LPAngular.controller("ModalAccountCreate", function($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.ProgressMessage = "";
    $scope.ErrorMessage = "";

    $scope.FormDisplayName = "";
    $scope.FormFirstName = "";
    $scope.FormLastName = "";
    $scope.FormUsername = "";
    $scope.FormPassword = "";

    $scope.AccountCallback = function (data) {
        if (data != null) {
            if (data.Status == 0) {
                $scope.SubmitLogin(data.Data);
            }
            else if (data.Status == 1) {
                $scope.RequestStatus = 2;

                if (data.StatusCode == "ACCESS_DENIED") {
                    $scope.ErrorMessage = "Access denied on account creation.";
                } else {
                    $scope.ErrorMessage = "Unhandled request error on account creation.";
                }
            }
            else {
                $scope.RequestStatus = 2;

                $scope.ErrorMessage = "Unhandled app error on account creation.";
            }
        }
        else {
            $scope.RequestStatus = 2;

            $scope.ErrorMessage = "Invalid server response on account creation.";
        }
    }

    $scope.AccountFail = function () {
        $scope.RequestStatus = 2;

        $scope.ErrorMessage = "Unhandled error on account creation.";
    }

    $scope.LoginCallback = function (data) {
        if (data != null) {
            if (data.Status == 0) {
                $uibModalInstance.close(0);
            }
            else if (data.Status == 1) {
                $scope.RequestStatus = 2;

                if (data.StatusCode == "ACCESS_DENIED") {
                    $scope.ErrorMessage = "Access denied on login creation.";
                }
                else if (data.StatusCode == "USERNAME_EXISTS") {
                    $scope.ErrorMessage = "Login already exists.  Account created without login.";
                } else {
                    $scope.ErrorMessage = "Unhandled request error on login creation.";
                }
            }
            else {
                $scope.RequestStatus = 2;

                $scope.ErrorMessage = "Unhandled app error on login creation.";
            }
        }
        else {
            $scope.RequestStatus = 2;

            $scope.ErrorMessage = "Invalid server response on login creation.";
        }
    }

    $scope.LoginFail = function () {
        $scope.RequestStatus = 2;

        $scope.ErrorMessage = "Unhandled error on login creation.";
    }

    $scope.SubmitAccount = function () {
        $scope.ProgressMessage = "Creating account...";

        // Create model
        var account = new LPAccounts.UserAccount();

        account.DisplayName = $scope.FormDisplayName;
        account.FirstName = $scope.FormFirstName;
        account.LastName = $scope.FormLastName;

        // Submit model
        LPAccounts.CreateAccount(account, $scope.AccountCallback, $scope.AccountFail);

        return;
    }

    $scope.SubmitLogin = function(account) {
        $scope.ProgressMessage = "Creating credentials...";

        // Create model
        var login = new LPAccounts.AuthUsername();

        login.Account = account.Id;
        login.Username = $scope.FormUsername;
        login.Password = $scope.FormPassword;

        // Submit model
        LPAccounts.CreateAuthUsername(login, $scope.LoginCallback, $scope.LoginFail);
        
        return;
    }

    $scope.Ok = function () {
        if ($scope.RequestStatus != 1 && $scope.RequestStatus != 2) {
            $scope.RequestStatus = 1;

            $scope.SubmitAccount();
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

// JSCombiner: Nav.js
LPAngular.controller("AdminLibraryNav", function ($scope) {
    $scope.template = "views/admin/library/nav.html";

    if (LPAccounts.LocalAccount != null) {
        // Show commands with permissions
    }

    $scope.UpdateLocalAccount = function (account) {
        if (LPAccounts.LocalAccount != null) {
            // Show commands with permissions
        }
    }

});

// JSCombiner: Nav.js
LPAngular.controller("AdminNewsNav", function ($scope, $uibModal) {
    $scope.template = "views/admin/news/nav.html";

    if (LPAccounts.LocalAccount != null) {
        // Show commands with permissions
    }

    $scope.UpdateLocalAccount = function (account) {
        if (LPAccounts.LocalAccount != null) {
            // Show commands with permissions
        }
    }

    $scope.ShowStatusCreateModal = function() {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/admin/news/status/create.html',
            controller: 'ModalNewsStatusCreate',
            size: "lg"
        });
    }

});

// JSCombiner: Browse.js
LPAngular.controller("RouteAdminNewsStatusBrowse", function ($scope, $location, $routeParams) {
    LPInterface.NavSelect("admin");

    $scope.LoadAttempted = false;

    $scope.CurrentPage = $routeParams.page;
    $scope.LoadStatus = 0;
    $scope.ErrorMessage = "";

    $scope.Statuses = [];

    $scope.Pages = [];
    $scope.PreviousPage = { Text: "", Active: false };
    $scope.NextPage = { Text: "", Active: false };

    $scope.UpdatePermissions = function () {
        if (LPAccounts.CheckLocalPermission("AdminCP")) {
            if ($scope.LoadAttempted == false) {
                LPNews.GetStatusPage($scope.CurrentPage, $scope.LoadData);

                $scope.LoadAttempted = true;
            }
        }
        else {
            $location.path("accessdenied");
        }

        return;
    }

    $scope.LoadData = function (data) {
        if (data != null && data.Data != null) {
            $scope.Statuses = data.Data.Results;

            // Pageation
            var totalPages = Math.ceil(data.Data.TotalResults / 50);
            var nextPageStart = 3;

            if ($scope.CurrentPage == 1) {
                $scope.Pages[0] = $scope.CreatePageObject(1, true);

                nextPageStart = 1;
            } else if ($scope.CurrentPage == 2) {
                $scope.Pages[0] = $scope.CreatePageObject(1, false);
                $scope.Pages[1] = $scope.CreatePageObject(2, true);

                nextPageStart = 2;
            } else {
                $scope.Pages[0] = $scope.CreatePageObject($scope.CurrentPage - 2, false);
                $scope.Pages[1] = $scope.CreatePageObject($scope.CurrentPage - 1, false);
                $scope.Pages[2] = $scope.CreatePageObject($scope.CurrentPage, true);
            }

            for (var x = nextPageStart; x < 5; x++) {
                var page = $scope.CurrentPage - nextPageStart + x + 1;

                if (totalPages < page) {
                    break;
                }

                $scope.Pages[x] = $scope.CreatePageObject(page, false);
            }

            // Previous/Next Page Buttons
            if ($scope.CurrentPage > 1) {
                $scope.PreviousPage.Text = "#!admin/news/status/browse/" + $scope.CurrentPage - 1;
                $scope.PreviousPage.Active = true;
            }

            if ($scope.CurrentPage < totalPages) {
                $scope.NextPage.Text = "#!admin/news/status/browse/" + $scope.CurrentPage + 1;
                $scope.NextPage.Active = true;
            }

            $scope.LoadStatus = 1;
        } else {
            $scope.LoadStatus = 2;
        }

        $scope.$apply();

        return;
    }

    $scope.CreatePageObject = function (num, active) {
        var page = {};

        page.Active = active;
        page.Text = num;

        return page;
    }

    if (LPAccounts.Initialized) {
        if (LPAccounts.LocalAccount != null && LPAccounts.CheckLocalPermission("AdminCP")) {
            $scope.LoadAttempted = true;

            LPNews.GetStatusPage($scope.CurrentPage, $scope.LoadData);

            $scope.PermissionHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
        }
        else {
            $location.path("accessdenied");
        }
    }
    else {
        $scope.PermissionHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
    }

    $scope.$on("$destroy", function () {
        if ($scope.PermissionHook != null) {
            LPAccounts.OnPermissionsChange.RemoveHook($scope.PermissionHook);
        }
    });

});

// JSCombiner: Create.js
LPAngular.controller("ModalNewsStatusCreate", function ($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";

    $scope.FormTitle = "";
    $scope.FormContentType = 0;

    $scope.FormAppId = 0;

    $scope.SubmissionCallback = function(data) {
        if (data != null) {
            if (data.Status == 0) {
                $uibModalInstance.dismiss('accept');
            }
            else if (data.Status == 1) {
                if (data.StatusCode == "ACCESS_DENIED") {
                    $scope.RequestStatus = 2;

                    $scope.ErrorMessage = "Access denied.";
                }
            }
            else {
                $scope.RequestStatus = 2;

                $scope.ErrorMessage = "Unhandled app error.";
            }
        }
        else {
            $scope.RequestStatus = 2;

            $scope.ErrorMessage = "Invalid server response.";
        }
    }

    $scope.SubmissionFailure = function() {
        $scope.RequestStatus = 2;

        $scope.ErrorMessage = "Unhandled error.";
    }

    $scope.UploadSuccess = function() {
        
    }

    $scope.UploadFailure = function() {
        
    }

    $scope.UploadProgress = function() {
        
    }

    $scope.SubmitStatus = function() {
        // Create status model
        var statusModel = {};

        statusModel.Id = 0;
        statusModel.Title = $scope.FormTitle;
        statusModel.ContentType = $scope.FormContentType;

        var contentModel = {};

        switch ($scope.RequestStatus) {
            case 1:
            {
                contentModel.AppId = $scope.FormAppId;
            }
        }

        statusModel.Content = JSON.stringify(contentModel);

        // Submit model
        LPNews.CreateStatus(statusModel, $scope.SubmissionCallback, $scope.SubmissionFailure);

        return;
    }

    $scope.Ok = function () {
        if ($scope.RequestStatus != 1 && $scope.RequestStatus != 2) {
            $scope.RequestStatus = 1;

            
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

});

// JSCombiner: Main.js
LPAngular.controller("RouteAmbienceMain", function ($scope) {
    LPInterface.NavSelect("ambience");
    LPInterface.SetupRoute($scope, "RouteAmbienceMain");
});

// JSCombiner: Main.js
LPAngular.controller("RouteAmbienceLighting", function ($scope) {
    LPInterface.NavSelect("ambience");
    LPInterface.SetupRoute($scope, "RouteAmbienceLighting");
});

// JSCombiner: Main.js
LPAngular.controller("RouteAmbienceMusic", function ($scope) {
    LPInterface.NavSelect("ambience");
    LPInterface.SetupRoute($scope, "RouteAmbienceMusic");
});

// JSCombiner: Chat.js
LPAngular.controller("RouteCommunityChat", function ($scope) {
    LPInterface.NavSelect("community");

    $scope.LoadStatus = 1;

    // Channels view
    $scope.ChannelLoad = 0;
    $scope.ChatChannels = [];

    // Chat view
    $scope.ChatHeader = "Chat";
    $scope.ChatMessages = [];

    $scope.LoadMessages = function(data) {
        
    }

    $scope.LoadNewMessages = function(data) {
        
    }

    $scope.LoadChannels = function(data) {
        
    }

    $scope.LoadChannelsFail = function(data) {
        
    }

    $scope.SendMessage = function() {
        
    }

    LPChat.GetChannels($scope.LoadChannels, $scope.LoadChannelsFail);
});

// JSCombiner: PlayerList.js
LPAngular.controller("RouteCommunityUsers", function ($scope, $location, $routeParams) {
    LPInterface.NavSelect("community");

    $scope.PageSize = 15;
    $scope.CurrentPage = (parseInt($routeParams.page) > 0) ? parseInt($routeParams.page) : 1;

    $scope.LoadStatus = 0;
    $scope.ErrorMessage = "SAMPLE TEXT";

    $scope.Accounts = [];

    $scope.Paginator = new LPPagination.Paginator();
    $scope.Paginator.CurrentPage = $scope.CurrentPage;
    $scope.Paginator.LinkPrefix = "#!community/users/";
    $scope.PageInfo = new LPPagination.PageInfo();

    $scope.ShowUser = function (id) {
        $location.path('#!/account/' + id);
    }

    $scope.GetUsers = function(data) {
        if (data != null && data.Data != null) {
            $scope.Accounts = [];

            for (var x = 0; x < data.Data.Results.length; x++) {
                $scope.Accounts[x] = LPAccounts.InitializeAccount(data.Data.Results[x]);
            }

            // Pageination
            $scope.Paginator.TotalElements = data.Data.TotalResults;
            $scope.Paginator.PageSize = $scope.PageSize;

            $scope.Paginator.Compute();

            $scope.LoadStatus = 1;
        } else {
            $scope.LoadStatus = 2;
        }

        $scope.$apply();
    }

    $scope.LoadError = function() {
        $scope.LoadStatus = 2;

        $scope.$apply();
    }

    LPAccounts.GetAccountPage($scope.CurrentPage, $scope.PageSize, true, LPAccounts.SearchAccountSort.LastActive, $scope.GetUsers);
});

// JSCombiner: UploadFile.js
LPAngular.controller('ModalUploadFile', function ($uibModalInstance, $scope) {
    $scope.RequestStatus = 0;
    $scope.PromptTitle = "Upload File";
    $scope.FilePath = "";

    $scope.OnUpload = new LPEvents.EventHandler();

    $scope.FinishUpload = function (data) {
        if (data != null) {
            $scope.OnUpload.Invoke(this, data);

            $uibModalInstance.dismiss('finish');
        }
        else {
            $scope.RequestStatus = 2;
        }
    }

    $scope.Ok = function () {
        // Check if file exists

        if ($scope.RequestStatus != 1) {
            // Show request working status
            $scope.RequestStatus = 1;

            // Upload file
            
        }

        return;
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

// JSCombiner: Home.js
LPAngular.controller("RouteHomeMain", function ($scope, $interval) {
    LPInterface.NavSelect("home");

    $scope.AccountLoaded = false;
    $scope.WeatherStatus = null;
    $scope.NewsStatusBody = "No status loaded!";
    $scope.QuickLinkLoad = 0;
    
    $scope.LoadNewsStatus = function(data) {
        if (data.Status == LPNet.AppResponseType.ResponseHandled) {
            $scope.NewsStatusBody = data.data.Content;
        }

        $scope.$apply();
    }

    $scope.LoadWeather = function(data) {
        if (data.Status == LPNet.AppResponseType.ResponseHandled) {
            $scope.WeatherStatus = data.data;
        }

        $scope.$apply();
    }

    $scope.LoadQuickLink = function(data) {
        if (data.Status == LPNet.AppResponseType.ResponseHandled) {
            var linkCount = data.Data.length;

            $scope.QuickLinks = [];

            for (var x = 0; x < linkCount; x++) {
                $scope.QuickLinks[x] = new LPNews.QuickLink();

                $scope.QuickLinks[x].LoadModel(data.Data[x]);
            }

            $scope.QuickLinkLoad = 1;
        } else {
            $scope.QuickLinkLoad = -1;
        }

        $scope.$apply();
    }

    $scope.LoadQuickLinkFail = function() {
        $scope.QuickLinkLoad = -1;

        $scope.$apply();
    }

    $scope.RetryLink = function() {
        $scope.QuickLinkLoad = 0;

        LPNews.GetActiveLinks($scope.LoadQuickLink, $scope.LoadQuickLinkFail);
    }

    if (LPAccounts.LocalAccount != null) {
        $scope.AccountFirstName = LPAccounts.LocalAccount.FirstName;

        $scope.AccountLoaded = true;
    }

    // Check news status every 10s
    $interval(function () {
        LPNews.GetCurrentStatus($scope.LoadNewsStatus);
    }, 10000);

    LPNews.GetCurrentStatus($scope.LoadNewsStatus);
    LPNews.GetActiveLinks($scope.LoadQuickLink, $scope.LoadQuickLinkFail);
    //LPNews.GetWeather($scope.LoadWeather);
});

// JSCombiner: AppList.js
LPAngular.controller("RouteLibraryApps", function ($scope, $location, $routeParams) {
    LPInterface.NavSelect("library");

    $scope.LoadStatus = 0;

    $scope.Apps = [];
    $scope.AppHooks = [];

    $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagAppEdit);

    // Paging
    $scope.PageSize = 15;
    $scope.CurrentPage = (parseInt($routeParams.page) > 0) ? parseInt($routeParams.page) : 1;

    $scope.Paginator = new LPPagination.Paginator();
    $scope.Paginator.CurrentPage = $scope.CurrentPage;
    $scope.Paginator.LinkPrefix = "#!library/apps/";
    $scope.PageInfo = new LPPagination.PageInfo();

    $scope.LoadApps = function (data) {
        if (data.Data != null) {
            var appCount = data.Data.Results.length;

            for (var x = 0; x < appCount; x++) {
                $scope.Apps[x] = new LPApps.App();

                // Load model into object
                $scope.Apps[x].LoadModel(data.Data.Results[x]);

                // Hook onto apps
                $scope.AppHooks[x] = $scope.Apps[x].OnUpdate.AddHook($scope.UpdateAppDetails);
            }

            // Pageination
            $scope.Paginator.TotalElements = data.Data.TotalResults;
            $scope.Paginator.PageSize = $scope.PageSize;

            $scope.Paginator.Compute();

            $scope.LoadStatus = 1;

            $scope.$apply();
        } else {
            $scope.LoadStatus = 2;
        }
    }

    LPApps.GetAppPage($scope.CurrentPage, $scope.PageSize, true, LPApps.SearchAppSort.Id, $scope.LoadApps);
});

// JSCombiner: LoanerList.js
LPAngular.controller("RouteLibraryLoaners", function ($scope, $uibModal) {
    LPInterface.NavSelect("library");

    $scope.Accounts = [];
    $scope.AccountHooks = [];

    $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);
    $scope.LoggedIn = LPAccounts.LocalAccount != null;

    $scope.LoadAccounts = function (data) {
        if (data.Data != null) {
            var accountCount = data.Data.length;

            for (var x = 0; x < accountCount; x++) {
                $scope.Accounts[x] = new LPApps.LoanerAccount();

                // Load model into object
                $scope.Accounts[x].LoadModel(data.Data[x]);

                // Hook onto accounts
                $scope.AccountHooks[x] = $scope.Accounts[x].OnUpdate.AddHook($scope.UpdateAccountDetails);

                // Update username
                $scope.Accounts[x].GetCheckoutUserName();
            }

            $scope.$apply();
        }
    }

    $scope.UpdateAccountDetails = function(sender, args) {
        $scope.$apply();

        return;
    }

    $scope.$on("$destroy", function () {
        var accountCount = $scope.AccountHooks.length;
        
        for (var x = 0; x < accountCount; x++) {
            $scope.AccountHooks[x].RemoveHook();
        }

        $scope.PermissionUpdateHook.RemoveHook();
    });

    $scope.Checkout = function(account)
    {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkout.html',
            controller: 'ModalLoanerCheckout',
            size: "md",
            resolve: {
                account: function() {
                    return account;
                }
            }
        });

        return;
    }

    $scope.Checkin = function(account) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkin.html',
            controller: 'ModalLoanerCheckin',
            size: "md",
            resolve: {
                account: function () {
                    return account;
                }
            }
        });

        return;
    }

    $scope.ShowCheckin = function(account) {
        var show = false;
        
        if (LPAccounts.LocalAccount != null) {
            show = account.CheckoutUser == LPAccounts.LocalAccount.Id;
        }

        return show;
    }

    $scope.ShowForceCheckin = function (account) {
        var show = false;

        if (LPAccounts.LocalAccount != null) {
            show = account.CheckoutUser != 0 && account.CheckoutUser != LPAccounts.LocalAccount.Id && $scope.AllowEditing;
        }

        return show;
    }

    $scope.ShowCheckout = function (account) {
        return $scope.LoggedIn == true && account.CheckoutUser == 0;
    }

    $scope.UpdatePermissions = function(sender, args) {
        $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);
        $scope.LoggedIn = LPAccounts.LocalAccount != null;

        $scope.$apply();

        return;
    }

    $scope.PermissionUpdateHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);

    LPApps.GetLoanerAccounts($scope.LoadAccounts);
});

// JSCombiner: Main.js
LPAngular.controller("RouteLibraryMain", function ($scope) {
    LPInterface.NavSelect("library");

});

// JSCombiner: AppView.js
LPAngular.controller("RouteLibraryAppView", function($scope, $location, $routeParams, $uibModal) {
    LPInterface.NavSelect("library");

    $scope.LoadStatus = 0;
    $scope.LoadLoaners = 0;
    $scope.ErrorMessage = "";

    $scope.LoadApp = function (data)
    {
        if (data != null)
        {
            if (data.Data != null)
            {
                $scope.App = new LPApps.App();
                $scope.App.LoadModel(data.Data);

                $scope.AppName = $scope.App.Title;
                $scope.AppType = $scope.App.GetTypeName();

                $scope.LoadStatus = 1;

                LPApps.GetAppLoaners($routeParams.AppId, $scope.LoadLoaners, $scope.LoadLoadersFail);
            }
            else
            {
                if (data.StatusCode == "INVALID_APP") {
                    $scope.LoadStatus = -1;
                    $scope.ErrorMessage = "The app does not exist.";
                }
                else
                {
                    $scope.LoadStatus = -1;
                    $scope.ErrorMessage = "Object returned was null.";
                }
            }
        }
        else
        {
            $scope.LoadStatus = -1;
            $scope.ErrorMessage = "Invalid server response.";
        }

        $scope.$apply();
    }

    $scope.LoadAppFail = function ()
    {
        $scope.LoadStatus = -1;
        $scope.ErrorMessage = "Communication to the server failed.";

        $scope.$apply();
    }

    $scope.LoadLoaners = function (data)
    {
        if (data != null)
        {
            if (data.Data != null)
            {
                $scope.Loaners = data.Data;

                $scope.LoadLoaners = 1;
            }
            else
            {
                $scope.LoadLoaners = -1;
            }
        }
        else
        {
            $scope.LoadLoaners = -1;
        }

        $scope.$apply();
    }

    $scope.LoadLoanersFail = function ()
    {
        $scope.LoadLoaners = -1;

        $scope.$apply();
    }

    LPApps.GetApp($routeParams.appId, $scope.LoadApp, $scope.LoadAppFail);
});

// JSCombiner: CreateAppModal.js


// JSCombiner: DeleteAppModal.js


// JSCombiner: EditAppModal.js


// JSCombiner: EditContentModal.js


// JSCombiner: AddAppModal.js
LPAngular.controller('ModalLoanerAddApp', function ($uibModalInstance, $scope, account) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;
    $scope.Apps = LPApps.Apps;          // TODO: Change this to a search based input
    $scope.FormApp = -1;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 2;

                $scope.Account.AddApp($scope.App);

                $scope.Account.OnUpdate.Invoke($scope, null);
            }
            else {
                $scope.RequestStatus = -1;
                $scope.ErrorMessage = data.StatusCode;
            }
        }
        else {
            $scope.RequestStatus = -1;
            $scope.ErrorMessage = "Server returned invalid response.";
        }
    }

    $scope.Add = function () {
        if ($scope.Apps[parseInt($scope.FormApp)] != null && $scope.RequestStatus < 1) {
            $scope.RequestStatus = 1;

            $scope.App = $scope.Apps[parseInt($scope.FormApp)];

            $.ajax({
                url: LanPlatform.ApiPath + "loaner/" + $scope.Account.Id + "/app/" + $scope.App.Id,
                type: 'PUT',
                success: $scope.FinishRequest
            });
        }
    }

    $scope.Ok = function () {
        $uibModalInstance.dismiss('accept');
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

// JSCombiner: CheckinModal.js
LPAngular.controller('ModalLoanerCheckin', function ($uibModalInstance, $scope, account) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 1;

                $scope.Account.CheckoutUser = 0;

                $scope.Account.OnUpdate.Invoke($scope, null);
            }
            else {
                $scope.RequestStatus = -1;
                $scope.ErrorMessage = data.StatusCode;
            }
        }
        else {
            $scope.RequestStatus = -1;
            $scope.ErrorMessage = "Server returned invalid response.";
        }
    }

    $scope.Ok = function () {
        $uibModalInstance.dismiss('accept');
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $.post(LanPlatform.ApiPath + "loaner/" + account.Id + "/checkin", {}, $scope.FinishRequest, "json");
});

// JSCombiner: CheckoutModal.js
LPAngular.controller('ModalLoanerCheckout', function ($uibModalInstance, $scope, account) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 1;

                $scope.Account.CheckoutUser = LPAccounts.LocalAccount.Id;

                $scope.Account.OnUpdate.Invoke($scope, null);
            }
            else {
                if (data.StatusCode == "CHECKOUT_LIMIT_HIT") {
                    $scope.RequestStatus = -2;
                }
                else {
                    $scope.RequestStatus = -1;

                    $scope.ErrorMessage = data.StatusCode;
                }
            }
        }
        else {
            $scope.RequestStatus = -1;
            $scope.ErrorMessage = "Server returned invalid response.";
        }
    }

    $scope.Ok = function () {
        $uibModalInstance.dismiss('accept');
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };

    $.post(LanPlatform.ApiPath + "loaner/" + account.Id + "/checkout", {}, $scope.FinishRequest, "json");
});

// JSCombiner: LoanerView.js
LPAngular.controller("RouteLibraryLoanerView", function ($scope, $location, $routeParams, $uibModal) {
    LPInterface.NavSelect("library");

    $scope.LoadStatus = 0;
    $scope.CheckoutStatus = 0;

    $scope.AllowCheckout = LPAccounts.LocalAccount != null;
    $scope.ForceCheckin = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerCheckout);
    $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);

    $scope.LoadAccount = function(data) {
        if (data != null) {
            if (data.Data != null) {
                var loaner = new LPApps.LoanerAccount();

                loaner.LoadModel(data.Data);

                $scope.Account = LPApps.AddLoanerAccount(loaner);

                $scope.AccountUpdateHook = $scope.Account.OnUpdate.AddHook($scope.UpdateAccount);

                $scope.UpdateCheckoutStatus();

                $scope.LoadStatus = 1;
            }
            else {
                $scope.LoadStatus = -1;
            }
        }
        else {
            $scope.LoadStatus = -1;
        }

        $scope.$apply();

        return;
    }

    $scope.UpdateAccount = function (sender, args) {
        $scope.UpdateCheckoutStatus();

        $scope.$apply();

        return;
    }

    $scope.UpdatePermissions = function(sender, args) {
        $scope.ForceCheckin = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerCheckout);
        $scope.AllowEditing = LPAccounts.CheckLocalPermission(LPApps.FlagLoanerEdit);

        $scope.$apply();
    }

    $scope.UpdateCheckoutStatus = function() {
        if ($scope.Account.CheckoutUser == 0) {
            $scope.CheckoutStatus = 0;
        }
        else if (LPAccounts.LocalAccount != null && $scope.Account.CheckoutUser == LPAccounts.LocalAccount.Id) {
            $scope.CheckoutStatus = 1;
        }
        else {
            $scope.CheckoutStatus = 2;
        }

        $scope.Account.GetCheckoutUserName();

        return;
    }

    $scope.AddApp = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/addapp.html',
            controller: 'ModalLoanerAddApp',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });

        return;
    }

    $scope.RemoveApp = function (app) {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/removeapp.html',
            controller: 'ModalLoanerRemoveApp',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                },
                app: function () {
                    return app;
                }
            }
        });

        return;
    }

    $scope.Checkout = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkout.html',
            controller: 'ModalLoanerCheckout',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });

        return;
    }

    $scope.Checkin = function () {
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'views/library/loaners/checkin.html',
            controller: 'ModalLoanerCheckin',
            size: "md",
            resolve: {
                account: function () {
                    return $scope.Account;
                }
            }
        });

        return;
    }

    $scope.$on("$destroy", function () {
        if ($scope.AccountUpdateHook != null) {
            $scope.AccountUpdateHook.RemoveHook();
        }

        $scope.PermissionUpdateHook.RemoveHook();
    });

    $scope.Account = LPApps.GetLoanerAccount($routeParams.accountId, $scope.LoadAccount);

    if ($scope.Account != null) {
        $scope.UpdateCheckoutStatus();

        $scope.LoadStatus = 1;

        $scope.AccountUpdateHook = $scope.Account.OnUpdate.AddHook($scope.UpdateAccount);
    }

    $scope.PermissionUpdateHook = LPAccounts.OnPermissionsChange.AddHook($scope.UpdatePermissions);
});

// JSCombiner: RemoveAppModal.js
LPAngular.controller('ModalLoanerRemoveApp', function ($uibModalInstance, $scope, account, app) {
    $scope.RequestStatus = 0;
    $scope.ErrorMessage = "";
    $scope.Account = account;
    $scope.App = app;

    $scope.FinishRequest = function (data) {
        if (data != null) {
            if (data.Status == LPNet.AppResponseType.ResponseHandled) {
                $scope.RequestStatus = 2;

                $scope.Account.RemoveApp($scope.App);

                $scope.Account.OnUpdate.Invoke($scope, null);
            }
            else {
                $scope.RequestStatus = -1;
                $scope.ErrorMessage = data.StatusCode;
            }
        }
        else {
            $scope.RequestStatus = -1;
            $scope.ErrorMessage = "Server returned invalid response.";
        }
    }

    $scope.Remove = function () {
        if ($scope.RequestStatus < 1) {
            $scope.RequestStatus = 1;

            $.ajax({
                url: LanPlatform.ApiPath + "loaner/" + $scope.Account.Id + "/app/" + $scope.App.Id,
                type: 'DELETE',
                success: $scope.FinishRequest
            });
        }
    }

    $scope.Ok = function () {
        $uibModalInstance.dismiss('accept');
    };

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
});

// JSCombiner: Routes.js
LPAngular.config(['$locationProvider', '$routeProvider',
    function config($locationProvider, $routeProvider) {
        $locationProvider.hashPrefix('!');

        $routeProvider.
            when('/', {
                templateUrl: 'views/home/main.html',
                controller: "RouteHomeMain"
            }).

            when('/library', {
                templateUrl: 'views/library/main.html',
                controller: "RouteLibraryMain"
            }).
                when('/library/games', {
                    templateUrl: 'views/library/games.html',
                    controller: "RouteLibraryGames"
                }).
                when('/library/mods', {
                    templateUrl: 'views/library/mods.html',
                    controller: "RouteLibraryMods"
                }).
                when('/library/apps', {
                    templateUrl: 'views/library/apps.html',
                    controller: "RouteLibraryApps"
                }).
                    when('/library/app/:appId', {
                        templateUrl: 'views/library/apps/view.html',
                        controller: "RouteLibraryAppView"
                    }).
                when('/library/loaners', {
                    templateUrl: 'views/library/loaners.html',
                    controller: "RouteLibraryLoaners"
                }).
                    when('/library/loaner/:accountId', {
                        templateUrl: 'views/library/loaners/view.html',
                        controller: "RouteLibraryLoanerView"
                    }).
                when('/library/downloads', {
                    templateUrl: 'views/library/downloads.html',
                    controller: "RouteLibraryDownloads"
                }).

            when('/ambience', {
                templateUrl: 'views/ambience/main.html',
                controller: "RouteAmbienceMain"
            }).
                when('/ambience/lighting', {
                    templateUrl: 'views/ambience/lighting/main.html',
                    controller: "RouteAmbienceLighting"
                }).
                when('/ambience/music', {
                    templateUrl: 'views/ambience/music/main.html',
                    controller: "RouteAmbienceMusic"
                }).

            when('/community', {
                templateUrl: 'views/community/main.html',
                controller: "RouteCommunityMain"
            }).
                when('/community/users', {
                    redirectTo: '/community/users/1'
                }).
                when('/community/users/:page', {
                    templateUrl: 'views/community/users.html',
                    controller: "RouteCommunityUsers"
                }).
                when('/community/chat', {
                    templateUrl: 'views/community/chat.html',
                    controller: "RouteCommunityChat"
                }).
                when('/community/leaderboards', {
                    templateUrl: 'views/community/leaderboards.html',
                    controller: "RouteCommunityLeaderboards"
                }).

            when('/admin', {
                templateUrl: 'views/admin/main.html',
                controller: "RouteAdminMain"
            }).
                when('/admin/news', {
                    templateUrl: 'views/admin/news.html',
                    controller: "RouteAdminNews"
                }).
                    when('/admin/news/status/browse/:page', {
                        templateUrl: 'views/admin/news/status/browse.html',
                        controller: "RouteAdminNewsStatusBrowse"
                    }).
                    when('/admin/news/status/edit/:statusId', {
                        templateUrl: 'views/admin/news/status/edit.html',
                        controller: "RouteAdminNewsStatusEdit"
                    }).
                when('/admin/library', {
                    templateUrl: 'views/admin/library.html',
                    controller: "RouteAdminLibrary"
                }).
                when('/admin/community', {
                    templateUrl: 'views/admin/community.html',
                    controller: "RouteAdminCommunity"
                }).
                    when('/admin/community/accounts/', {
                        templateUrl: 'views/admin/community.html',
                        controller: "RouteAdminCommunity"
                    }).
                    when('/admin/community/accounts/edit/:guestId', {
                        templateUrl: 'views/admin/community/guests/edit.html',
                        controller: "RouteAdminCommunityGuestsEdit"
                    }).
                    when('/admin/community/accounts/create', {
                        templateUrl: 'views/admin/community/guests/create.html',
                        controller: "RouteAdminCommunityGuestsCreate"
                    }).
                    when('/admin/community/accounts/search?phrase&page', {
                        templateUrl: 'views/admin/community/guests/search.html',
                        controller: "RouteAdminCommunityGuestsSearch"
                    }).
                    when('/admin/community/accounts/browse/:page', {
                        templateUrl: 'views/admin/community/accounts/browse.html',
                        controller: "RouteAdminCommunityAccountsBrowse"
                    }).

            when('/account/:accountId', {
                templateUrl: 'views/account/main.html',
                controller: "RouteAccountView"
            }).

            when('/accessdenied', {
                templateUrl: 'views/global/accessdenied.html'
            }).

            otherwise({ redirectTo: '/' });
    }
]);

// JSCombiner: Pagination.js
LPPagination = {};

// JSCombiner: PageInfo.js
LPPagination.PageInfo = function() {
    this.Number = 0;
    this.Active = false;
    this.CurrentPage = false;
    this.Link = "";
}

// JSCombiner: Paginator.js
LPPagination.Paginator = function() {
    this.CurrentPage = 0;
    this.TotalElements = 0;
    this.PageSize = 0;

    this.LinkPrefix = "";
    this.LinkSuffix = "";

    this.OtherPageButtonCount = 2;

    this.Pages = [];
    this.PreviousPage = new  LPPagination.PageInfo();
    this.NextPage = new LPPagination.PageInfo();
}

LPPagination.Paginator.prototype.Compute = function() {
    // Pageation
    var totalPages = Math.ceil(this.TotalElements / this.PageSize);
    var totalNextPages = totalPages - this.CurrentPage;
    var totalPrevPages = totalPages - totalNextPages - 1;
    var allowedNextPages = this.OtherPageButtonCount;
    var allowedPrevPages = this.OtherPageButtonCount;
    
    if (totalNextPages < this.OtherPageButtonCount) {
        allowedNextPages = totalNextPages;

        allowedPrevPages = (totalPrevPages < allowedPrevPages + this.OtherPageButtonCount - totalNextPages)
            ? totalPrevPages
            : allowedPrevPages + this.OtherPageButtonCount - totalNextPages;
    }

    if (totalPrevPages < this.OtherPageButtonCount) {
        allowedPrevPages = totalPrevPages;

        allowedNextPages = (totalNextPages < allowedNextPages + this.OtherPageButtonCount - totalPrevPages)
            ? totalNextPages
            : allowedNextPages + this.OtherPageButtonCount - totalPrevPages;
    }

    var prevPageEnd = allowedPrevPages;
    var nextPageEnd = allowedPrevPages + 1 + allowedNextPages;
        
    this.Pages = [];

    // Previous pages
    for (var x = 0; x < prevPageEnd; x++) {
        var pageNum = this.CurrentPage - prevPageEnd + x;

        this.Pages[x] = this.CreatePageInfo(pageNum, true, this.LinkPrefix + pageNum + this.LinkSuffix);
    }

    // Current page
    this.Pages[prevPageEnd] = this.CreatePageInfo(this.CurrentPage, false, "");
    this.Pages[prevPageEnd].CurrentPage = true;

    // Next pages
    for (var x = prevPageEnd + 1; x < nextPageEnd; x++) {
        var pageNum = this.CurrentPage + x - prevPageEnd;

        this.Pages[x] = this.CreatePageInfo(pageNum, true, this.LinkPrefix + pageNum + this.LinkSuffix);
    }

    // Previous/Next Page Buttons
    if (this.CurrentPage > 1) {
        this.PreviousPage = this.CreatePageInfo("<", true, this.LinkPrefix + (this.CurrentPage - 1) + this.LinkSuffix);
    } else {
        this.PreviousPage = this.CreatePageInfo("<", false, "");
    }
    
    if (this.CurrentPage < totalPages) {
        this.NextPage = this.CreatePageInfo(">", true, this.LinkPrefix + (this.CurrentPage + 1) + this.LinkSuffix);
    } else {
        this.NextPage = this.CreatePageInfo(">", false, "");
    }
    
}

LPPagination.Paginator.prototype.CreatePageInfo = function(pageNum, active, link) {
    var page = new LPPagination.PageInfo();

    page.Number = pageNum;
    page.Active = active;
    page.Link = link;

    return page;
}

