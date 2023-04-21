using Pollux.Domain.Data;
using Pollux.Domain.Processing;
using System;

namespace Pollux.Domain.Helper
{
    public static class TlmHelper
    {
        private static readonly TlmHelperSettings _settings = SettingsHandler.GetTlmHelperSettings();


        public static double CalculateBatteryPercentage(this string batteryvoltage)
        {
            try
            {
                var percentage = (double)(double.Parse(batteryvoltage) - _settings.BatteryEmptyMv) / (double)(_settings.BatteryFullMv - _settings.BatteryEmptyMv);
                percentage = percentage < 0 ? 0 : percentage > 100 ? 100 : percentage;
                return percentage * 100;
            }
            catch(Exception) 
            { 
                return 0; 
            }
        }
    }
}