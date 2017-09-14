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