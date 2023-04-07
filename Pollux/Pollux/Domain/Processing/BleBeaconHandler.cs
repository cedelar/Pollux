using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Plugin.BLE.Abstractions.Contracts;
using Pollux.Domain.Connection;
using Pollux.Domain.Data;
using Pollux.Domain.Filter;
using Pollux.Domain.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Essentials;

namespace Pollux.Domain.Processing
{
    public class BleBeaconHandler
    {
        private const bool _retainMessagesInMemory = true;
        private string[] _whitelist => new string[] { "F2:58:48:27:9C:56", "CA:F4:A4:77:77:26", "7A:89:FE:38:1E:4B" };
        private const string _movementDestination = "Pollux";

        private PolarisConnectionHandler _polarisConnection;
        private LocationHandler _locationHandler;

        private List<BleBeacon> _trackedBeacons;
        private Dictionary<string, HistoricalBleBeacon> _beaconDictionary;
        private List<IProcessingFilter> _commonFilters;
        private List<IProcessingFilter> _movementFilters;
        private List<IProcessingFilter> _tlmFilters;

        public event EventHandler<string> NotificationupdateRequested;

        public BleBeaconHandler()
        {
            _polarisConnection = new PolarisConnectionHandler();
            _locationHandler = new LocationHandler();   
            _trackedBeacons = new List<BleBeacon>();
            _beaconDictionary = new Dictionary<string, HistoricalBleBeacon>();
            SetupFilters();
        }

        public void SubmitDevice(IDevice device)
        {
            var bleBeacon = new BleBeacon(device);
            if(string.IsNullOrEmpty(bleBeacon.MacAdress) || !bleBeacon.Messages.Any())
            {
                return;
            }

            _beaconDictionary.TryGetValue(bleBeacon.MacAdress, out var historicalData);
            if(historicalData == null)
            {
                historicalData = new HistoricalBleBeacon(bleBeacon);
                _beaconDictionary.Add(historicalData.MacAdress, historicalData);
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

            if (_retainMessagesInMemory)
            {
                AddToMemory(device);
            }
        }

        public bool PassesFilters(BleBeacon beacon, List<IProcessingFilter> filters)
        {
            var passes = true;
            foreach(var filter in filters)
            {
                if(!filter.BeaconPasses(beacon, _beaconDictionary[beacon.MacAdress]))
                {
                    passes = false;
                    break;
                }
            }
            return passes;
        }

        public void SetupFilters()
        {
            _commonFilters = new List<IProcessingFilter>();
            _commonFilters.Add(
                new WhitelistFilter()
                {
                    Whitelist = _whitelist.ToList()
                } );

            _movementFilters = new List<IProcessingFilter>();
            _movementFilters.Add(
                new MinIntervalFilter()
                {
                    EventType = EventTypes.MovementSend,
                    IntervalInSec = 30
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
                    IntervalInSec = 60
                });
        }

        public async void SubmitMovement(BleBeacon bleBeacon)
        {
            var movement = new PolarisMovementResult()
            {
                DestinationCode = _movementDestination,
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
                _beaconDictionary[bleBeacon.MacAdress].Event(EventTypes.MovementSend);
            }
        }

        public async void SubmitTlm(BleBeacon bleBeacon)
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
                    _beaconDictionary[bleBeacon.MacAdress].Event(EventTypes.TlmSend);
                }
            }
        }

        private void AddToMemory(IDevice device)
        {
            var beacon = _trackedBeacons.FirstOrDefault(d => d.Id == device.Id);
            if (beacon != null)
            {
                beacon.Update(device);
                //NotificationupdateRequested?.Invoke(this, "Updated " + beacon.MacAdress);
            }
            else
            {
                try
                {
                    var newBeacon = new BleBeacon(device);
                    if (newBeacon.Messages.Count > 0)
                    {
                        _trackedBeacons.Add(newBeacon);
                        //NotificationupdateRequested?.Invoke(this, "Added " + newBeacon.MacAdress);
                    }
                }
                catch (ArgumentException)
                {
                    //Not a bluetoothdevice, ignore
                }
            }
        }
    }
}