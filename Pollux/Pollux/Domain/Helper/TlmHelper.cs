using System;

namespace Pollux.Domain.Helper
{
    public static class TlmHelper
    {
        private const int _batteryFullInMv = 3000;
        private const int _batteryEmptyInMv = 1000;

        public static double CalculateBatteryPercentage(this string batteryvoltage)
        {
            try
            {
                var percentage = (double)(double.Parse(batteryvoltage) - _batteryEmptyInMv) / (double)(_batteryFullInMv - _batteryEmptyInMv);
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