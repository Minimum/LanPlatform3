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