using Pollux.Domain.Data;
using System.Linq;

namespace Pollux.Domain.Filter
{
    public class FrameTypeFilter : IProcessingFilter
    {
        public FrameType FrameType;

        public bool BeaconPasses(BleBeacon beacon, HistoricalBleBeacon historicalData)
        {
            return beacon.Messages.Any(m => m.Type == FrameType);
        }
    }
}