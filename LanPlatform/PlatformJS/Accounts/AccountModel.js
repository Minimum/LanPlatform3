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