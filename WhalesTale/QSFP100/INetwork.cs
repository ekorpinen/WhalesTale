using System;
using System.Threading.Tasks;

namespace WhalesTale.QSFP100
{
    public interface INetwork
    {
        Task<bool> PatternSelectAsync(Qsfp100GRegister.Page7.PatternMaskReg166 whichOne);
        Task<bool> PatternSelectAsync(Qsfp100GRegister.Page7.PatternMaskReg129 whichOne);
        Task<bool> PatternGeneratorEnableAsync(EnableLane enable);
        Task<bool> ClockSourceAsync(Qsfp100GRegister.Page7.ClockSourceMaskReg129 whichOne);
        Task<bool> ErrorCheckerEnableAsync(EnableLane enable);
        Task<bool> ErrorCheckerLockedAsync();
        Task<bool> ErrorCounterFreezeAsync(EnableLane freeze);
        Task<ulong> BitCountAsync();
        Task<ulong> ErrorCountAsync();
        Task<double> BerAsync();
        Task<(double BER, ulong BitCount, ulong ErrorCount, bool Locked)> BerAsync(TimeSpan timeSpan);
    }
}