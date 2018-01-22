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