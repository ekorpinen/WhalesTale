using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WhalesTale.QSFP100
{
    public interface IHost
    {
        Task<bool> PatternSelect(Qsfp100GRegister.Page7.PatternMaskReg166 whichOne);
        Task<bool> PatternSelect(Qsfp100GRegister.Page7.PatternMaskReg129 whichOne);
        Task<bool> PatternGeneratorEnable(EnableLane enable);
        Task<bool> ClockSource(Qsfp100GRegister.Page7.ClockSourceMaskReg129 whichOne);
        Task<bool> ErrorCheckerEnable(EnableLane enable);
        Task<bool> ErrorCheckerLocked(SelectLane lane);
        Task<bool> ErrorCounterFreeze(EnableLane freeze);
        Task<ulong> BitCount(SelectLane lane);
        Task<ulong> ErrorCount(SelectLane lane);
        Task<double> Ber();

        Task<List<(SelectLane Lane, double BER, ulong BitCount, ulong ErrorCount, bool Locked)>> BerAsync(
            EnableLane lanes, TimeSpan timeSpan);
    }
}