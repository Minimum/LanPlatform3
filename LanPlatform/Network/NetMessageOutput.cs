using System;
using Newtonsoft.Json;

namespace LanPlatform.Network
{
    public class NetMessageOutput
    {
        public long Id { get; set; }
        public String Type { get; set; }
        public String Data { get; set; }

        public NetMessageOutput()
        {
            Id = 0;
            Type = ""; 
            Data = "";
        }

        public NetMessageOutput(NetMessage message)
        {
            Id = message.Id;
            Type = message.GetMessageType();
            Data = JsonConvert.SerializeObject(message);
        }

    }
}