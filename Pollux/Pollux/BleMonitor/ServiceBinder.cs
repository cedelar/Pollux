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
using System.Threading.Tasks;

namespace Pollux.BleMonitor
{
    public class BleMonitorServiceBinder : Binder
    {
        public BleMonitorService Service { get; set; }

        public BleMonitorServiceBinder(BleMonitorService service)
        {
            Service = service;
        }

        public Dictionary<string, HistoricalBleBeacon> GetBeaconData()
        {
            if (Service != null)
            {
                return Service.GetBeaconData();
            }
            return new Dictionary<string, HistoricalBleBeacon>();
        }

        public void ResetBeaconData()
        {
            if (Service != null)
            {
                Service.ResetBeaconData();
            }
        }
    }
}