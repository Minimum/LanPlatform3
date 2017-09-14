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

