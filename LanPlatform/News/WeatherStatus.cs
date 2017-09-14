using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.Database;

namespace LanPlatform.News
{
    public class WeatherStatus : DatabaseObject
    {
        public String Source { get; set; }

        // Current
        public int CurrentTemperature { get; set; }
        public WeatherType CurrentWeatherType { get; set; }
        public int CurrentRainChance { get; set; }
        public long CurrentTime { get; set; }

        // Future
        public int FirstTemperature { get; set; }
        public WeatherType FirstWeatherType { get; set; }
        public int FirstRainChance { get; set; }
        public long FirstTime { get; set; }

        public int SecondTemperature { get; set; }
        public WeatherType SecondWeatherType { get; set; }
        public int SecondRainChance { get; set; }
        public long SecondTime { get; set; }

        public int ThirdTemperature { get; set; }
        public WeatherType ThirdWeatherType { get; set; }
        public int ThirdRainChance { get; set; }
        public long ThirdTime { get; set; }

        // Daily
        public int DailyRainChance { get; set; }
        public int DailyHigh { get; set; }
        public int DailyLow { get; set; }

        public long Sunrise { get; set; }
        public long Sunset { get; set; }

        public WeatherStatus()
        {
            Source = "";

            CurrentTemperature = 0;
            CurrentWeatherType = WeatherType.None;
            CurrentRainChance = 0;
            CurrentTime = 0;

            FirstTemperature = 0;
            FirstWeatherType = WeatherType.None;
            FirstRainChance = 0;
            FirstTime = 0;

            SecondTemperature = 0;
            SecondWeatherType = WeatherType.None;
            SecondRainChance = 0;
            SecondTime = 0;

            ThirdTemperature = 0;
            ThirdWeatherType = WeatherType.None;
            ThirdRainChance = 0;
            ThirdTime = 0;

            DailyRainChance = 0;
            DailyHigh = 0;
            DailyLow = 0;

            Sunrise = 0;
            Sunset = 0;
        }
    }

    public enum WeatherType
    {
        None = 0,
        Sunny,
        Cloudy,
        Rain,
        Lightning,
        Snow
    }
}