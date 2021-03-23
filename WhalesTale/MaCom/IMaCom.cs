using System;
using System.Threading;
using System.Threading.Tasks;

namespace WhalesTale.MaCom
{
    public interface IMaCom
    {
        public Task<int> FwDebugAsync(CancellationToken ct = default);
        public  Task<int> FwDebug2Async(CancellationToken ct = default);
        public Task<int> TiaCoarseAsync(CancellationToken ct = default);
        public Task<int> TiaFineAsync(CancellationToken ct = default);
        public Task<int> PvtEfuse(CancellationToken ct = default);
        public  Task<int> PvttApplied(CancellationToken ct = default);
        public  Task<(int Skew0, int Skew1, int Skew2, int Skew3)> Skews(CancellationToken ct = default);
        public  Task<decimal> AdcBackoff(CancellationToken ct = default);
        public  Task<decimal> Tap(MaCom.Taps whichTap, CancellationToken ct = default);
        public Task<(decimal Level0, decimal Level1, decimal Level2, decimal Level3)> Levels(CancellationToken ct = default);
        public Task<(decimal Decision01, decimal Decision12, decimal Decision23)> Decisions(CancellationToken ct = default);
        public Task<decimal> PpmOffset(CancellationToken ct = default);
    }
}