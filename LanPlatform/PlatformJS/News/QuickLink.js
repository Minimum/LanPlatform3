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