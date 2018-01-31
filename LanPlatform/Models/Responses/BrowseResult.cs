using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using LanPlatform.DTO;

namespace LanPlatform.Models.Responses
{
    public class BrowseResult<T>
    {
        public long TotalResults { get; set; }
        public List<T> Results { get; protected set; }

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

        public void Add(T result)
        {
            Results.Add(result);

            return;
        }

        public void AddRange(ICollection<T> results)
        {
            Results.AddRange(results);

            return;
        }
    }
}