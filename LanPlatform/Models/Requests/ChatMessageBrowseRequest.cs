using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Models.Requests
{
    public class ChatMessageBrowseRequest
    {
        public long Start { get; set; }
        public int Limit { get; set; }

        public ChatMessageBrowseRequest()
        {
            Start = 0;
            Limit = 100;
        }

        public void SanityCheck()
        {
            if (Start < 0)
                Start = 0;

            if (Limit < 1)
            {
                Limit = 1;
            }
            else if (Limit > 500)
            {
                Limit = 500;
            }

            return;
        }
    }
}