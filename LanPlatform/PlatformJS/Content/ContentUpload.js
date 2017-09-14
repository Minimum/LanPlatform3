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