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

namespace Pollux.Domain.Filter
{
    public class MinIntervalFilter : IProcessingFilter
    {
        public EventTypes EventType;
        public double IntervalInSec;

        private void setEventType(EventTypes type)
        {
            EventType = type;
        }

        /// <summary>
        /// Set the minimum interval between events in sec
        /// </summary>
        /// <param name="interval"></param>
        private void setInterval(double interval)
        {
            IntervalInSec = interval;
        }

        public bool BeaconPasses(BleBeacon beacon, HistoricalBleBeacon historicalData)
        {
            return historicalData.TimeSinceEvent(EventType) > IntervalInSec;
        }
    }
}