using System;

namespace LanPlatform.Models.Requests
{
    public class SearchAccountRequest
    {
        // Sort
        public SearchAccountSort SortBy { get; set; }
        public bool SortDescending { get; set; }

        public String Query { get; set; }
        public long Page { get; set; }
        public int PageSize { get; set; }

        public SearchAccountRequest()
        {
            SortBy = SearchAccountSort.Id;
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

    public enum SearchAccountSort
    {
        Id = 0,
        LastActive,
        DisplayName,
        FirstName,
        LastName,
        TotalLans
    }
}