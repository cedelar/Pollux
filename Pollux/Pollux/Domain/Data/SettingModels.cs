using System.Collections.Generic;

namespace Pollux.Domain.Data
{
    public class PolarisConnectionSettings
    {
        public string ApiAdress { get; set; }
        public string LoginEndpoint { get; set; }
        public string MovementEndpoint { get; set; }
        public string TlmEndpoint { get; set; }
        public string PolarisUsername { get; set; }
        public string PolarisPassword { get; set; }
    }

    public class BeaconHandlerSettings
    {
        public bool CommonWhiteListEnabled { get; set; }
        public List<string> CommonWhitelist { get; set; }
        public string MovementDestinationCode { get; set; }
        public int MovementMinSendIntervalSec { get; set; }
        public int TlmMinSendIntervalSec { get; set; }
    }

    public class MonitorServiceSettings
    {
        public int DurationOfBleScansSec { get; set; }
        public int TimeBetweenBleScansSec { get; set; }
        public string NotificationTitleText { get; set; }
        public string NotificationContentText { get; set; }
        public bool AllowNotificationUpdates { get; set; }
    }

    public class LocationHandlerSettings
    {
        public int LocationRefreshTimeSec { get; set; } 
    }

    public class TlmHelperSettings
    {
        public int BatteryEmptyMv { get; set; }
        public int BatteryFullMv { get; set; }

    }
}