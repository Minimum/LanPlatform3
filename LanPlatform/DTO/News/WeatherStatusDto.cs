using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LanPlatform.News;

namespace LanPlatform.DTO.News
{
    public class WeatherStatusDto : GabionDto
    {
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

        public WeatherStatusDto()
        {
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

        public WeatherStatusDto(WeatherStatus status)
            : base(status)
        {
            CurrentTemperature = status.CurrentTemperature;
            CurrentWeatherType = status.CurrentWeatherType;
            CurrentRainChance = status.CurrentRainChance;
            CurrentTime = status.CurrentTime;

            FirstTemperature = status.FirstTemperature;
            FirstWeatherType = status.FirstWeatherType;
            FirstRainChance = status.FirstRainChance;
            FirstTime = status.FirstTime;

            SecondTemperature = status.SecondTemperature;
            SecondWeatherType = status.SecondWeatherType;
            SecondRainChance = status.SecondRainChance;
            SecondTime = status.SecondTime;

            ThirdTemperature = status.ThirdTemperature;
            ThirdWeatherType = status.ThirdWeatherType;
            ThirdRainChance = status.ThirdRainChance;
            ThirdTime = status.ThirdTime;

            DailyRainChance = status.DailyRainChance;
            DailyHigh = status.DailyHigh;
            DailyLow = status.DailyLow;

            Sunrise = status.Sunrise;
            Sunset = status.Sunset;
        }

        public override string GetClassname()
        {
            return "WeatherStatus";
        }

        public static List<GabionDto> ConvertList(ICollection<WeatherStatus> objects)
        {
            var models = new List<GabionDto>();

            foreach (WeatherStatus target in objects)
            {
                models.Add(new WeatherStatusDto(target));
            }

            return models;
        }
    }
}