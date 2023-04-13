using Newtonsoft.Json;
using Plugin.BLE.Abstractions.Contracts;
using Pollux.Domain.Connection;
using Pollux.Domain.Data;
using Pollux.Domain.Filter;
using Pollux.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pollux.Domain.Processing
{
    public class BleBeaconHandler
    {
        private readonly BeaconHandlerSettings _settings;
        private PolarisConnectionHandler _polarisConnection;
        private LocationHandler _locationHandler;

        private List<IProcessingFilter> _commonFilters;
        private List<IProcessingFilter> _movementFilters;
        private List<IProcessingFilter> _tlmFilters;

        public event EventHandler<string> NotificationupdateRequested;

        public Dictionary<string, HistoricalBleBeacon> BeaconDictionary;

        public BleBeaconHandler()
        {
            _settings = SettingsHandler.GetBeaconHandlerSettings();
            _polarisConnection = new PolarisConnectionHandler();
            _locationHandler = new LocationHandler();   
            BeaconDictionary = new Dictionary<string, HistoricalBleBeacon>();
            SetupFilters();
        }

        public void SubmitDevice(IDevice device)
        {
            var bleBeacon = new BleBeacon(device);
            if(string.IsNullOrEmpty(bleBeacon.MacAdress) || !bleBeacon.Messages.Any())
            {
                return;
            }

            BeaconDictionary.TryGetValue(bleBeacon.MacAdress, out var historicalData);
            if(historicalData == null)
            {
                historicalData = new HistoricalBleBeacon(bleBeacon);
                BeaconDictionary.Add(historicalData.MacAdress, historicalData);
            }
            else
            {
                historicalData.Seen(bleBeacon);
            }

            //Check if something needs to happen
            var sendingPermitted = PassesFilters(bleBeacon, _commonFilters);
            if (!sendingPermitted) return;

            //Check if movement needs to be send
            var movementPermitted = PassesFilters(bleBeacon, _movementFilters);
            if (movementPermitted) SubmitMovement(bleBeacon);
            
            //Check if Tlm needs to be send
            var tlmPermitted = PassesFilters(bleBeacon, _tlmFilters);
            if (tlmPermitted) SubmitTlm(bleBeacon);
        }

        private bool PassesFilters(BleBeacon beacon, List<IProcessingFilter> filters)
        {
            var passes = true;
            foreach(var filter in filters)
            {
                if(!filter.BeaconPasses(beacon, BeaconDictionary[beacon.MacAdress]))
                {
                    passes = false;
                    break;
                }
            }
            return passes;
        }

        private void SetupFilters()
        {
            _commonFilters = new List<IProcessingFilter>();
            if (_settings.CommonWhiteListEnabled)
            {
                _commonFilters.Add(
                    new WhitelistFilter()
                    {
                        Whitelist = _settings.CommonWhitelist
                    });
            }

            _movementFilters = new List<IProcessingFilter>();
            _movementFilters.Add(
                new MinIntervalFilter()
                {
                    EventType = EventTypes.MovementSend,
                    IntervalInSec = _settings.MovementMinSendIntervalSec
                });

            _tlmFilters = new List<IProcessingFilter>();
            _tlmFilters.Add(
                new FrameTypeFilter()
                {
                    FrameType = FrameType.Tlm
                }); 
            _tlmFilters.Add(
                new MinIntervalFilter()
                {
                    EventType = EventTypes.TlmSend,
                    IntervalInSec = _settings.TlmMinSendIntervalSec
                });
        }

        private async void SubmitMovement(BleBeacon bleBeacon)
        {
            var movement = new PolarisMovementResult()
            {
                DestinationCode = _settings.MovementDestinationCode,
                ScannedTags = new List<PolarisScannedTag>()
                {
                    new PolarisScannedTag()
                    {
                        TagText = bleBeacon.MacAdress,
                        ScanType = PolarisScanType.BLE
                    }
                },
                MovementType = PolarisMovementType.Movement,
                Coordinates = JsonConvert.SerializeObject(await _locationHandler.GetCurrentCoordinates())
            };
            var success = await _polarisConnection.SubmitMovement(movement);
            if (success)
            {
                NotificationupdateRequested?.Invoke(this, "Movement: " + bleBeacon.MacAdress);
                BeaconDictionary[bleBeacon.MacAdress].Event(EventTypes.MovementSend);
            }
        }

        private async void SubmitTlm(BleBeacon bleBeacon)
        {
            var tlmFrame = bleBeacon.Messages.FirstOrDefault(f => f.Type == FrameType.Tlm);
            if(tlmFrame != null)
            {
                var tlmMessage = new PolarisTLMMessage()
                {
                    Id = bleBeacon.MacAdress,
                    BatteryVoltage = tlmFrame.getDataField(TlmDatafields.BatteryVoltage),
                    BatteryTemperature = ((int)Convert.ToDouble(tlmFrame.getDataField(TlmDatafields.Temperature))).ToString(), //Polaris requires int
                    AdvPDUCount = tlmFrame.getDataField(TlmDatafields.AdvCount),
                    TimeSincePoweron = tlmFrame.getDataField(TlmDatafields.TimeSincePoweron),
                    BatteryPercentage = ((int)tlmFrame.getDataField(TlmDatafields.BatteryVoltage).CalculateBatteryPercentage()).ToString(),  //Polaris requires int
                };
                var success = await _polarisConnection.SubmitTlm(new List<PolarisTLMMessage>() {tlmMessage});
                if (success)
                {
                    NotificationupdateRequested?.Invoke(this, "Tlm: " + bleBeacon.MacAdress);
                    BeaconDictionary[bleBeacon.MacAdress].Event(EventTypes.TlmSend);
                }
            }
        }
    }
}