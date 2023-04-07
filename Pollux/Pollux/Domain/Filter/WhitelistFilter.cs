using Pollux.Domain.Data;
using System.Collections.Generic;

namespace Pollux.Domain.Filter
{
    public class WhitelistFilter : IProcessingFilter
    {
        public List<string> Whitelist;
        
        public WhitelistFilter()
        {
            Whitelist = new List<string>();
        }

        public void AddToList(List<string> macs)
        {
            Whitelist.AddRange(macs);
        }

        public void AddToList(string mac)
        {
            Whitelist.Add(mac);
        }

        public bool BeaconPasses(BleBeacon beacon, HistoricalBleBeacon historicalData)
        {
            if (Whitelist.Contains(beacon.MacAdress))
            {
                return true;
            }
            return false;
        }
    }
}