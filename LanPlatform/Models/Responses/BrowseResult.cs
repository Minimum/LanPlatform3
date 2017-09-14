using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Models.Responses
{
    public class BrowseResult<T>
    {
        public long TotalResults { get; set; }
        public List<T> Results { get; set; }

        public BrowseResult()
        {
            TotalResults = 0;
            Results = new List<T>();
        }

        public BrowseResult(List<T> results, long totalResults)
        {
            TotalResults = totalResults;
            Results = results;
        }
    }
}