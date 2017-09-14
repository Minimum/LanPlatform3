LPNews.WeatherStatus = function() {
    // Current
    this.CurrentTemperature = 0;
    this.CurrentWeatherType = 0;
    this.CurrentRainChance = 0;
    this.CurrentTime = 0;

    // Future
    this.FirstTemperature = 0;
    this.FirstWeatherType = 0;
    this.FirstRainChance = 0;
    this.FirstTime = 0;

    this.SecondTemperature = 0;
    this.SecondWeatherType = 0;
    this.SecondRainChance = 0;
    this.SecondTime = 0;

    this.ThirdTemperature = 0;
    this.ThirdWeatherType = 0;
    this.ThirdRainChance = 0;
    this.ThirdTime = 0;

    // Daily
    this.DailyRainChance = 0;
    this.DailyHigh = 0;
    this.DailyLow = 0;

    this.Sunrise = 0;
    this.Sunset = 0;
}

LPNews.WeatherStatus.prototype.LoadModel = function(model) {
    // Current
    this.CurrentTemperature = model.CurrentTemperature;
    this.CurrentWeatherType = model.CurrentWeatherType;
    this.CurrentRainChance = model.CurrentRainChance;
    this.CurrentTime = model.CurrentTime;

    // Future
    this.FirstTemperature = model.FirstTemperature;
    this.FirstWeatherType = model.FirstWeatherType;
    this.FirstRainChance = model.FirstRainChance;
    this.FirstTime = model.FirstTime;

    this.SecondTemperature = model.SecondTemperature;
    this.SecondWeatherType = model.SecondWeatherType;
    this.SecondRainChance = model.SecondRainChance;
    this.SecondTime = model.SecondTime;

    this.ThirdTemperature = model.ThirdTemperature;
    this.ThirdWeatherType = model.ThirdWeatherType;
    this.ThirdRainChance = model.ThirdRainChance;
    this.ThirdTime = model.ThirdTime;

    // Daily
    this.DailyRainChance = model.DailyRainChance;
    this.DailyHigh = model.DailyHigh;
    this.DailyLow = model.DailyLow;

    this.Sunrise = model.Sunrise;
    this.Sunset = model.Sunset;

    return;
}

LPNews.WeatherStatus.prototype.GetFirstTime = function() {
    var a = new Date(this.FirstTime * 1000);

    var hour = a.getHours();
    var min = a.getMinutes();
    var suffix = (hour >= 12) ? " PM" : " AM";

    hour = hour % 12;

    if (hour == 0) {
        hour = 12;
    }

    var time = hour + ((min < 10) ? ":0" : ":") + min + suffix;

    return time;
}