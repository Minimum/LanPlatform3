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