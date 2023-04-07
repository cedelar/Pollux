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

namespace Pollux.Domain.Data
{
    public class HistoricalBleBeacon
    {
        private Dictionary<EventTypes, DateTime> _lastEventTimes;

        public Guid Id { get; set; }
        public string MacAdress { get; set; }
        public int LastRssi { get; set; }
        public DateTime LastSeenTime { get; set; }

        public HistoricalBleBeacon(BleBeacon beacon)
        {
            Id = beacon.Id;
            MacAdress = beacon.MacAdress;
            LastRssi = beacon.LastRssi;
            LastSeenTime = DateTime.UtcNow;
            _lastEventTimes = new Dictionary<EventTypes, DateTime>();
        }

        public void Seen(BleBeacon beacon)
        {
            LastRssi = beacon.LastRssi;
            LastSeenTime = DateTime.UtcNow;
        }

        public void Event(EventTypes type)
        {
            _lastEventTimes[type] = DateTime.UtcNow;
        }

        /// <summary>
        /// Returns time since the event in seconds
        /// If event hasn't happend yet, returns double.MaxValue
        /// </summary>
        public double TimeSinceEvent(EventTypes type)
        {
            if(!_lastEventTimes.TryGetValue(type, out var time))
            {
                return double.MaxValue;
            }
            return (DateTime.UtcNow - time).TotalSeconds;
        }

        /// <summary>
        /// Returns null if event hasn't happend yet
        /// </summary>
        public DateTime TimeOfEvent(EventTypes type)
        {
            _lastEventTimes.TryGetValue(type, out var time);
            return time;
        }
    }

    public enum EventTypes{
        None,
        MovementSend,
        TlmSend
    }
}