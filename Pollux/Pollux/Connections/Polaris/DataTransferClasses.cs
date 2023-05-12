using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace Pollux.Domain.Data
{
    //Mirrors MovementResult on Polaris for datatransfer
    public class PolarisMovementResult
    {
        public string DestinationCode { get; set; }

        public List<PolarisScannedTag> ScannedTags { get; set; }

        public PolarisMovementType MovementType { get; set; }

        public string Coordinates { get; set; }
    }

    public class PolarisScannedTag
    {
        public string TagText { get; set; }

        public PolarisScanType ScanType { get; set; }
    }

    public enum PolarisMovementType
    {
        Movement,
        MovementFromSource,
        Returns
    }

    public enum PolarisScanType
    {
        /// <summary>
        /// 1D or 2D barcode
        /// </summary>
        Barcode = 0,

        /// <summary>
        /// Radio-frequency identification
        /// </summary>
        RFID = 1,

        /// <summary>
        /// A human readable code
        /// </summary>
        Manual = 2,

        /// <summary>
        /// RTLS technology used mostly in health-care
        /// </summary>
        CenTrak = 3,

        /// <summary>
        /// Bluetooth Low Energy tags
        /// </summary>
        BLE = 4,

        /// <summary>
        /// gRPC messages from Herz
        /// </summary>
        Hertz = 5
    }

    //Mirrors Authenticationclasses on Polaris for datatransfer
    public class PolarisAuthentication
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class PolarisAuthenticationResult
    {
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationDate { get; set; }
        public long ExpirationIntervalMs { get; set; }
    }

    public class PolarisRefreshTokenRequest
    {
        public string RefreshToken { get; set; }
    }

    public class PolarisTLMMessage
    {
        public string Id { get; set; }
        //in mV
        public string BatteryVoltage { get; set; }
        //in °C
        public string BatteryTemperature { get; set; }
        public string AdvPDUCount { get; set; }
        //in 100ms
        public string TimeSincePoweron { get; set; }
        public string BatteryPercentage { get; set; }
    }

    public class PolarisDeviceInfoResult
    {
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceOS { get; set; }
        public string AppVersion { get; set; }
        public bool ConfigUpdated { get; set; }
        public PolarisClientType ClientType { get; set; }
        public PolarisPingData PingData { get; set; }
    }

    public class PolarisPingData
    {
        public string InstallationId { get; set; }
        public string CurrentCoordinates { get; set; }
        public string CurrentModule { get; set; }
    }

    public enum PolarisClientType
    {
        Unknown,
        Handheld,
        Gate,
        Hertz,
        BleMonitor,
        Other
    }
}