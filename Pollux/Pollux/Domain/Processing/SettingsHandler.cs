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
                ApiAdress = "https://polaris-ble-test.azurewebsites.net/",
                LoginEndpoint = "api/Authentication/Login",
                MovementEndpoint = "apiinternal/ActionConfirmation/Movement",
                TlmEndpoint = "apiinternal/BleTag/PostBleTLM",
                DeviceInfoEndpoint = "apiinternal/DeviceInfo/Post",
                PingEndpoint = "apiinternal/Configuration/PingWithData",
                PolarisUsername = "Pollux",
                PolarisPassword = "Pollux01",
                DeviceName = "Unnamed Device"
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
                CommonWhitelist = new List<string>() { 
                    //Testbeacons CD
                    "F2:58:48:27:9C:56",
                    "CA:F4:A4:77:77:26",
                    //Beacons POC1
                    "C3:7F:40:2B:BB:0F",
                    "F5:74:01:91:37:EE",
                    "ED:D3:E3:B4:03:32",
                    //Beacons POC2
                    "EC:96:FB:22:B6:AF",
                    "E2:4F:27:0C:27:04",
                    "EC:1A:EB:33:66:06",
                    "E6:AC:69:E1:51:AD",
                    "DC:9B:D3:FE:10:E6",
                    "F5:C2:B8:02:6D:B0",
                    "CA:F4:12:B5:D6:5B",
                    "CF:C0:27:E0:DA:F6",
                    "FB:5C:30:8E:94:E6",
                    "F4:90:27:92:35:F1",
                    "E6:5B:14:97:3F:39",
                    "C1:72:14:29:CB:F6",
                    "ED:E5:F1:1D:5A:6B",
                    "C8:B1:CA:6A:80:AA",
                    "ED:9A:6F:E0:07:2D",
                    "CC:57:F9:52:A3:30",
                    "D4:47:E3:45:C6:E9",
                    "FC:8C:04:12:DE:59",
                    "D2:14:23:3D:AE:45",
                    "CD:6E:ED:8F:52:5D",
                    "E7:D9:DA:15:7B:B8",
                    "F7:38:46:5A:38:55",
                    "C4:F4:70:7E:DB:13",
                    "DF:2D:2B:4E:2B:31",
                    "D8:EA:60:A4:3C:6F",
                    "FF:14:DD:F7:5D:F6",
                    "CD:08:56:79:2E:81",
                    "D7:44:77:6D:4A:AE",
                    "E7:C5:9B:94:8D:86",
                    "C3:C0:B2:E0:10:D6",
                    "EC:C0:1D:6B:8B:D5",
                    "CD:D9:AB:2F:0A:25",
                    "CC:55:FE:93:CF:AC",
                    "E8:D9:55:68:B8:E0",
                    "F6:2E:4B:C3:2F:C6",
                    "D2:A2:80:49:51:48",
                    "CB:D4:3C:FD:F4:CA",
                    "DE:EF:7E:76:14:C0",
                    "C5:E9:7F:55:2F:03",
                    "F4:DB:1B:83:9D:57",
                    "E4:85:B9:5A:29:6E",
                    "FB:DB:CF:79:B8:D7",
                    "D1:2D:20:56:F9:19",
                    "F7:71:8A:C2:AC:E6",
                    "F6:44:07:D5:31:05",
                    "CF:BD:D3:E6:7C:93",
                    "F2:3F:2F:4F:78:B0",
                    "F6:77:21:3D:9F:38",
                    "C9:FC:22:F8:A9:C8",
                    "D5:90:2E:7F:52:AD",
                    "F6:1E:3E:FE:92:0A",
                    "E4:A9:4B:13:59:89",
                    "F2:40:83:46:1F:FC",
                    "DA:44:4C:FD:6C:A4",
                    "D5:E1:52:F7:E0:4A",
                    "FC:74:2C:1A:EA:07",
                    "EF:E5:A9:CD:4E:F3",
                    "DB:41:05:38:5D:87",
                    "FC:63:D3:79:99:8F",
                    "C8:BC:AC:C2:6F:30",
                    "DA:97:64:BC:CF:BC",
                    "D3:1A:E2:DF:58:ED",
                    "D4:97:02:2F:4B:27",
                    "FD:D7:F2:25:5F:64",
                    "DE:20:C2:9D:B5:53",
                    "CD:85:FB:CC:E1:20",
                    "C6:63:85:1F:DA:59",
                    "FA:34:7F:C0:6C:10",
                    "ED:45:29:1B:34:67",
                    "C8:58:C1:57:09:7D",
                    "E1:AF:63:AD:F7:EE",
                    "E3:94:2E:DD:5D:22",
                    "DA:CC:23:48:89:AA",
                    "C4:8E:EE:53:6F:B8",
                    "F7:8E:BE:FE:9C:20",
                    "CF:42:AD:A5:49:2F",
                    "C9:43:AB:A3:45:2A",
                    "FC:03:4A:BD:11:32",
                    "DE:95:76:D3:7E:05",
                    "D4:17:6F:10:9B:94",
                    "E8:A4:92:3A:DA:A2",
                    "CA:06:F3:F7:7F:45",
                    "EE:6A:30:93:42:4A",
                    "FA:AA:35:E9:B1:CC",
                    "CA:6C:43:A0:A2:E7",
                    "C8:D7:D8:07:8E:DE",
                    "F7:A1:94:F8:BC:C6",
                    "F8:A6:70:AA:1A:B6",
                    "DE:78:7D:9B:B3:70",
                    "F2:A5:77:DB:2D:63",
                    "D7:2B:0C:AB:EE:DB",
                    "EF:1B:AB:C4:53:93",
                    "CC:DA:BE:99:8F:09",
                    "D0:6D:EF:14:A2:51",
                    "CA:A5:E7:E1:03:0F",
                    "D7:A0:38:1B:45:60",
                    "CF:66:68:6A:BA:B6",
                    "F1:18:08:1D:74:79",
                    "F3:13:6A:8B:88:24",
                    "D2:26:E4:EB:32:A8"
                },
                MovementDestinationCode = "Pollux",
                MovementMinSendIntervalSec = 30,
                TlmMinSendIntervalSec = 120,
                PingMinSendIntervalSec = 60,
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
                AllowNotificationUpdates = false
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

        public static TlmHelperSettings GetTlmHelperSettings()
        {
            if (Preferences.ContainsKey(SettingObjects.TlmHelperSettings))
            {
                return JsonConvert.DeserializeObject<TlmHelperSettings>(Preferences.Get(SettingObjects.TlmHelperSettings, null));
            }
            return new TlmHelperSettings()
            {
                BatteryEmptyMv = 1000,
                BatteryFullMv = 3000
            };
        }

        public static GuiSettings GetGuiSettings()
        {
            if (Preferences.ContainsKey(SettingObjects.GuiSettings))
            {
                return JsonConvert.DeserializeObject<GuiSettings>(Preferences.Get(SettingObjects.GuiSettings, null));
            }
            return new GuiSettings()
            {
                HideNonWhitelistedBeaconsInMonitor = true,
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
        public static void Save(this GuiSettings settings)
        {
            Preferences.Set(SettingObjects.GuiSettings, JsonConvert.SerializeObject(settings));
        }
    }

    public static class SettingObjects
    {
        public static readonly string PolarisConnectionSettings = "PolarisConnectionSettings";
        public static readonly string BeaconHandlerSettings = "BeaconHandlerSettings";
        public static readonly string MonitorServiceSettings = "MonitorServiceSettings";
        public static readonly string LocationHandlerSettings = "LocationHandlerSettings";
        public static readonly string TlmHelperSettings = "TlmHelperSettings";
        public static readonly string GuiSettings = "GuiSettings";
    }
}