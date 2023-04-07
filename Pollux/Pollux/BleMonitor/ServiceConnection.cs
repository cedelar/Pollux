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

namespace Pollux.BleMonitor
{
    public class BleMonitorServiceConnection : Java.Lang.Object, IServiceConnection
    {
        private static MainActivity _mainActivity;

        public bool IsConnected { get; set; }
        public BleMonitorServiceBinder Binder { get; set; }

        public BleMonitorServiceConnection(MainActivity mainActivity)
        {
            _mainActivity = mainActivity;
        }

        public void OnServiceConnected(ComponentName name, IBinder serviceBinder)
        {
            Binder = serviceBinder as BleMonitorServiceBinder;
            if(Binder == null)
            {
                throw new InvalidCastException("The serviceBinder needs to be of type " + typeof(BleMonitorServiceBinder));
            }
            IsConnected = true;
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            IsConnected = false;
            Binder = null;
        }
    }
}