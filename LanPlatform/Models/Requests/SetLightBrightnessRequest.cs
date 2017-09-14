using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LanPlatform.Models.Requests
{
    public class SetLightBrightnessRequest
    {
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
}