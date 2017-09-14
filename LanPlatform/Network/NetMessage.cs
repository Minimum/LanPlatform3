using System;
using System.Collections.Generic;
using LanPlatform.Database;
using Newtonsoft.Json;

namespace LanPlatform.Network
{
    public abstract class NetMessage : DatabaseObject
    {
        [JsonIgnore]
        public List<long> Targets { get; set; }

        public NetMessage()
        {
            Targets = new List<long>();
        }

        public abstract String GetMessageType();
    }
}