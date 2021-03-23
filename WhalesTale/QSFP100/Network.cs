using System;
using System.Threading.Tasks;
using Support;
using WhalesTale.Communication;

namespace WhalesTale.QSFP100
{
    public class NetworkSide : INetwork
    {
        private readonly Qsfp100G _parent;

        public NetworkSide(Qsfp100G parent) => _parent = parent;

        public async Task<bool> ClockSourceAsync(Qsfp100GRegister.Page7.ClockSourceMaskReg129 whichOne)
        {
            byte currentByte = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false);
            currentByte &= 0x7F;
            currentByte |= (byte) ((byte) whichOne << 7);
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection, currentByte).ConfigureAwait(false);

            //validate
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false) == currentByte;
        }

        public async Task<bool> PatternSelectAsync(Qsfp100GRegister.Page7.PatternMaskReg129 whichOne)
        {
            byte currentByte = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false);
            currentByte &= 0x8F;
            currentByte |= (byte) ((byte) whichOne << 4);
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection, currentByte).ConfigureAwait(false);

            //validate
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false) == currentByte;
        }

        public async Task<bool> PatternSelectAsync(Qsfp100GRegister.Page7.PatternMaskReg166 whichOne)
        {
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PatternGenerationAdditionalNetwork, (byte) whichOne).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PatternGenerationAdditionalNetwork).ConfigureAwait(false) ==
                   (byte) whichOne;
        }

        public async Task<bool> PatternGeneratorEnableAsync(EnableLane enable)
        {
            const byte mask = 0x0F;
            byte generatorEnabled = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable).ConfigureAwait(false);

            generatorEnabled = (byte) (generatorEnabled & mask);
            generatorEnabled = (byte) (generatorEnabled | ((byte) enable << 4));

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable, generatorEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable).ConfigureAwait(false) == generatorEnabled;
        }

        public async Task<bool> ErrorCheckerEnableAsync(EnableLane enable)
        {
            const byte mask = 0x0F;
            byte checkersEnabled = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsCheckerEnable).ConfigureAwait(false);

            checkersEnabled = (byte) (checkersEnabled & mask);
            checkersEnabled = (byte) (checkersEnabled | ((byte) enable << 4));

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsCheckerEnable, checkersEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsCheckerEnable).ConfigureAwait(false) == checkersEnabled;
        }

        public async Task<bool> ErrorCheckerLockedAsync() =>
            Bit.IsBitSet(await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsNetworkCheckerLock).ConfigureAwait(false), 0);

        public async Task<bool> ErrorCounterFreezeAsync(EnableLane freeze) // note false means UPDATE error counter
        {
            const byte mask = 0x0F;
            byte counterUpdateEnabled =
                await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze).ConfigureAwait(false);

            counterUpdateEnabled = (byte) (counterUpdateEnabled & mask);
            counterUpdateEnabled = (byte) (counterUpdateEnabled | ((byte) freeze << 4));

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze, counterUpdateEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze).ConfigureAwait(false) ==
                   counterUpdateEnabled;
        }


        public async Task<ulong> BitCountAsync()
        {
            var data = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitCountNetwork).ConfigureAwait(false);
            var bitCountExponent = (ushort) ((data >> 10) & 0x3f);
            var bitCountMantissa = (ushort) (data & 0x3ff);
            return bitCountMantissa * (ulong) Math.Pow(2, bitCountExponent);
        }

        public async Task<ulong> ErrorCountAsync()
        {
            var data = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsErrorCountNetwork).ConfigureAwait(false);
            var bitCountExponent = (ushort) ((data >> 10) & 0x3f);
            var bitCountMantissa = (ushort) (data & 0x3ff);
            return bitCountMantissa * (ulong) Math.Pow(2, bitCountExponent);
        }

        public async Task<double> BerAsync() => await BitCountAsync() == 0
            ? 1
            : await ErrorCountAsync().ConfigureAwait(false) / (double) await BitCountAsync().ConfigureAwait(false);

        public async Task<(double BER, ulong BitCount, ulong ErrorCount, bool Locked)> BerAsync(TimeSpan timeSpan)
        {
            await ErrorCheckerEnableAsync(EnableLane.None).ConfigureAwait(false);
            await Task.Delay(250);
            await ErrorCheckerEnableAsync(EnableLane.L1).ConfigureAwait(false);
            await Task.Delay(timeSpan);
            //      await ErrorCheckerEnableAsync(EnableLane.L1).ConfigureAwait(false);
            var ber = await BitCountAsync() == 0
                ? 1
                : await ErrorCountAsync().ConfigureAwait(false) / (double) await BitCountAsync().ConfigureAwait(false);
            var bitCount = await BitCountAsync().ConfigureAwait(false);
            var errorCount = await ErrorCountAsync().ConfigureAwait(false);
            return (ber, bitCount, errorCount, await ErrorCheckerLockedAsync().ConfigureAwait(false));
        }

    }
}