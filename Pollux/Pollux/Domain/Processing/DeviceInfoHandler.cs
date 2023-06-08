using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.DeviceInfo;
using Pollux.Domain.Connection;
using Pollux.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pollux.Domain.Processing
{
    public class DeviceInfoHandler
    {
        private PolarisConnectionHandler _polarisConnection;
        private LocationHandler _locationHandler;
        private PolarisConnectionSettings _polarisSettings;
        private BeaconHandlerSettings _beaconSettings;

        private DateTime _lastPing;

        private string currentModule = "BleMonitor";
        private string installationId => CrossDeviceInfo.Current.Id;
        private string deviceModel => CrossDeviceInfo.Current.Manufacturer + " " + CrossDeviceInfo.Current.Model;
        private string deviceOS => CrossDeviceInfo.Current.Platform.ToString() + " " + CrossDeviceInfo.Current.VersionNumber;
        private string appVersion => CrossDeviceInfo.Current.AppVersion;

        public DeviceInfoHandler(PolarisConnectionHandler polarisConnection, LocationHandler locationHandler)
        {
            if(polarisConnection == null || locationHandler == null)
            {
                throw new ArgumentNullException("DeviceInfoHandler: (polarisConnection == null || locationHandler == null)");
            }
            _polarisConnection = polarisConnection;
            _locationHandler = locationHandler;
            _polarisSettings = SettingsHandler.GetPolarisConnectionSettings();
            _beaconSettings = SettingsHandler.GetBeaconHandlerSettings();

            SubmitDeviceInfo();

            _lastPing = DateTime.UtcNow;
        }

        public async void Ping()
        {
            if(_lastPing.AddSeconds(_beaconSettings.PingMinSendIntervalSec) < DateTime.UtcNow)
            {
                _lastPing = DateTime.UtcNow;
                await _polarisConnection.SubmitPing(await getPingData());
            }
        }

        private async void SubmitDeviceInfo()
        {
            var deviceInfo = new PolarisDeviceInfoResult()
            {
                DeviceName = _polarisSettings.DeviceName,
                DeviceModel = deviceModel.Length > 15 ? deviceModel.Substring(deviceModel.Length - 15) : deviceModel, //Polaris has MaxLength 15
                DeviceOS = deviceOS,
                ClientType = PolarisClientType.BleMonitor,
                AppVersion = appVersion,
                PingData = new PolarisPingData()
                {
                    InstallationId = installationId,
                    CurrentCoordinates = JsonConvert.SerializeObject(await _locationHandler.GetCurrentCoordinates()),
                    CurrentModule = currentModule
                }
            };

            await _polarisConnection.SubmitDeviceInfo(deviceInfo);
        }

        private async Task<PolarisPingData> getPingData()
        {
            var location = await _locationHandler.GetCurrentCoordinates();
            return new PolarisPingData()
            {
                InstallationId = installationId,
                CurrentCoordinates = JsonConvert.SerializeObject(location),
                CurrentModule = currentModule
            };
        }
    }
}