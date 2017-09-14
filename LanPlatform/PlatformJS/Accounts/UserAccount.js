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