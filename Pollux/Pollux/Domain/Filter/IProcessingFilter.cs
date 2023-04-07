using Pollux.Domain.Data;

namespace Pollux.Domain.Filter
{
    public interface IProcessingFilter
    {
        //return true to continue processing, return false to discard.
        public bool BeaconPasses(BleBeacon beacon, HistoricalBleBeacon historicalData);
    }
}