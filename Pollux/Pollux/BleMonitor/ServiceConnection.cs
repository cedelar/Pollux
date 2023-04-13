using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Pollux.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Pollux.BleMonitor
{
    public class BleMonitorServiceConnection : Java.Lang.Object, IServiceConnection
    {
        public bool IsConnected { get; set; }

        public event EventHandler<bool> OnConnectionChanged;

        public BleMonitorServiceBinder Binder { get; set; }

        public void OnServiceConnected(ComponentName name, IBinder serviceBinder)
        {
            Binder = serviceBinder as BleMonitorServiceBinder;
            if(Binder == null)
            {
                throw new InvalidCastException("The serviceBinder needs to be of type " + typeof(BleMonitorServiceBinder));
            }
            IsConnected = true;
            OnConnectionChanged?.Invoke(this, true);
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
            OnConnectionChanged?.Invoke(this, false);
        }

        public Dictionary<string, HistoricalBleBeacon> GetBeaconData()
        {
            if (Binder != null)
            {
                return Binder.GetBeaconData();
            }
            return new Dictionary<string, HistoricalBleBeacon>();
        }
    }
}