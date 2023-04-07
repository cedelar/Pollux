using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pollux.Domain.Data
{
    public class BleBeacon
    { 
        public Guid Id { get; set; }
        public string MacAdress { get; set; }
        public int LastRssi { get; set; }
        public List<EddystoneMessage> Messages { get; private set; }

        public BleBeacon(IDevice device)
        {
            Id = device.Id;
            LastRssi = device.Rssi;
            Messages = new List<EddystoneMessage>();

            var bluetoothDevice = device.NativeDevice as Android.Bluetooth.BluetoothDevice;
            if(bluetoothDevice == null)
            {
                return;
            }
            MacAdress = bluetoothDevice.Address;
            AddMessages(device.Rssi, device.AdvertisementRecords.ToList());
        }

        public void Update(IDevice device = null)
        {
            if (device == null)
            {
                return;
            }
            LastRssi = device.Rssi;
            AddMessages(device.Rssi, device.AdvertisementRecords.ToList());
        }

        public void AddMessages(int rssi, List<AdvertisementRecord> records)
        {
            records.ForEach(r =>
            {
                try
                {
                    Messages.Add(new EddystoneMessage(rssi, r));
                }
                catch(ArgumentException)
                {
                    //Happens if message is not a valid Eddystonemessage, ignore and move on.
                }
            });
        }
    }
}
