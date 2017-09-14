using System;

namespace LanPlatform.Engine
{
    public static class EngineUtil
    {
        public static long CurrentTime
        {
            get
            {
                return (long) DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
            }
        }
    }
}