using System;

namespace LuckyDice.custom.monobehaviour.impl.persistent.weathercredits
{
    public class WeatherCreditsEvent3 : WeatherCreditsEvent
    {
        private void Awake()
        {
            CreditsPerTrigger = 3;
        }
    }
}