using Newtonsoft.Json;
using Pollux.Domain.Data;
using System.Collections.Generic;
using Xamarin.Essentials;

namespace Pollux.Domain.Processing
{
    public static class SettingsHandler
    {
        public static PolarisConnectionSettings GetPolarisConnectionSettings()
        {
            if (Preferences.ContainsKey(SettingObjects.PolarisConnectionSettings))
            {
                return JsonConvert.DeserializeObject<PolarisConnectionSettings>(Preferences.Get(SettingObjects.PolarisConnectionSettings,  null));
            }
            return new PolarisConnectionSettings()
            {
                ApiAdress = "https://aucxis-polaris-test-web.azurewebsites.net/",
                LoginEndpoint = "api/Authentication/Login",
                MovementEndpoint = "apiinternal/ActionConfirmation/Movement",
                TlmEndpoint = "apiinternal/BleTag/PostBleTLM",
                PolarisUsername = "Pollux",
                PolarisPassword = "Pollux01"
            };       
        }

        public static BeaconHandlerSettings GetBeaconHandlerSettings()
        {
            if (Preferences.ContainsKey(SettingObjects.BeaconHandlerSettings))
            {
                return JsonConvert.DeserializeObject<BeaconHandlerSettings>(Preferences.Get(SettingObjects.BeaconHandlerSettings, null));
            }
            return new BeaconHandlerSettings()
            {
                CommonWhiteListEnabled = false,
                CommonWhitelist = new List<string>() {"F2:58:48:27:9C:56", "CA:F4:A4:77:77:26"},
                MovementDestinationCode = "Pollux",
                MovementMinSendIntervalSec = 60,
                TlmMinSendIntervalSec = 30
            };
        }
        
        public static MonitorServiceSettings GetMonitorServiceSettings()
        {         
            if (Preferences.ContainsKey(SettingObjects.MonitorServiceSettings))
            {
                return JsonConvert.DeserializeObject<MonitorServiceSettings>(Preferences.Get(SettingObjects.MonitorServiceSettings, null));
            }
            return new MonitorServiceSettings()
            {
                DurationOfBleScansSec = 10,
                TimeBetweenBleScansSec = 1,
                NotificationTitleText = "Pollux Ble Monitor",
                NotificationContentText = "Activated",
                AllowNotificationUpdates = true
            };           
        }

        public static LocationHandlerSettings GetLocationHandlerSettings()
        {
            if (Preferences.ContainsKey(SettingObjects.LocationHandlerSettings))
            {
                return JsonConvert.DeserializeObject<LocationHandlerSettings>(Preferences.Get(SettingObjects.LocationHandlerSettings, null));
            }
            return new LocationHandlerSettings()
            {
                LocationRefreshTimeSec = 10
            };
        }

        public static void Save(this PolarisConnectionSettings settings)
        {
            Preferences.Set(SettingObjects.PolarisConnectionSettings, JsonConvert.SerializeObject(settings));
        }
        public static void Save(this BeaconHandlerSettings settings)
        {
            Preferences.Set(SettingObjects.BeaconHandlerSettings, JsonConvert.SerializeObject(settings));
        }
        public static void Save(this MonitorServiceSettings settings)
        {
            Preferences.Set(SettingObjects.MonitorServiceSettings, JsonConvert.SerializeObject(settings));
        }

        public static void Save(this LocationHandlerSettings settings)
        {
            Preferences.Set(SettingObjects.LocationHandlerSettings, JsonConvert.SerializeObject(settings));
        }
    }

    public static class SettingObjects
    {
        public static readonly string PolarisConnectionSettings = "PolarisConnectionSettings";
        public static readonly string BeaconHandlerSettings = "BeaconHandlerSettings";
        public static readonly string MonitorServiceSettings = "MonitorServiceSettings";
        public static readonly string LocationHandlerSettings = "LocationHandlerSettings";
    }
}