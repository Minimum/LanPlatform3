using System;

namespace LanPlatform.Models.Requests
{
    public class SearchAppRequest
    {
        // Sort
        public SearchAppSort SortBy { get; set; }
        public bool SortDescending { get; set; }

        public String Query { get; set; }
        public long Page { get; set; }
        public int PageSize { get; set; }

        public SearchAppRequest()
        {
            SortBy = SearchAppSort.Id;
            SortDescending = true;

            Query = "";
            Page = 1;
            PageSize = 20;
        }

        public void SanityCheck()
        {
            if (PageSize < 10)
                PageSize = 10;

            if (PageSize > 100)
                PageSize = 100;

            return;
        }
    }

    public enum SearchAppSort
    {
        Id = 0,
        AppType,
        Title,
        Description,
        DownloadType
    }
}