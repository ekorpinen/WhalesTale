// ***********************************************************************
// Assembly         : Registers
// Author           : Errol Korpinen
// Created          : 12-09-2018
//
// Last Modified By : labuser
// Last Modified On : 12-09-2018
// ***********************************************************************
// <copyright file="TP2.cs" company="Cisco TMG">
//     Copyright ©  2018
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Threading;
using System.Threading.Tasks;
using Support;

namespace WhalesTale.QSFP100

{
    public class FecClass : IFec
    {
        [Flags]
        public enum PmdStatus : byte
        {
            None = 0,
            PmdFault = 1 << 0,
            PmdTxFault = 1 << 1,
            PmdRxFault = 1 << 2,
            PmdLosFault = 1 << 3,
            FecSerError = 1 << 4
        }

        private readonly Qsfp100G _parent;

        public FecClass(Qsfp100G parent) => _parent = parent;

        public async Task<bool> HighSer() =>
            Bit.IsBitSet(await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecHighSer).ConfigureAwait(false), 0);

        public async Task<uint> UnCorrectableFrameCount() =>
            await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecUncorrectableFrameCounter).ConfigureAwait(false);

        public async Task<uint> CorrectedFrameCount() =>
            (uint) await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecCorrectableFrameCounter).ConfigureAwait(false);

        public async Task<uint> ErrorSymbolCount() =>
            await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecErrorSymbolCounter).ConfigureAwait(false);

        public async Task<uint> BipCount() => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecBipCounter).ConfigureAwait(false);

        public async Task<uint> TotalWordCount() =>
            await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecTotalWordCounter).ConfigureAwait(false);

        public async Task<byte> Status() => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecPmdStatus).ConfigureAwait(false);

        public async Task<bool> ClearCounters()
        {
            // 0->1 transition
            const byte mask = 0xFD; // 1111 1101
            byte clearCounter =
                await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecCountersControl).ConfigureAwait(false); // read current reg contents
            clearCounter = (byte) (clearCounter & mask); // set control bit to 0
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page8.FecCountersControl, clearCounter).ConfigureAwait(false); // write to register
            Thread.Sleep(250); // holder register with control bit set to 0 for 250 mS

            clearCounter |= 1 << 1; // set control bit to 1
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page8.FecCountersControl, clearCounter).ConfigureAwait(false); // write register with control bit 1 
            Thread.Sleep(250); // wait 250 mS

            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecCountersControl).ConfigureAwait(false) ==
                   clearCounter; // confirm register setting
        }

        public async Task<bool> CountersEnable(bool enable)
        {
            const byte mask = 0xFE;
            byte countersEnable = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecCountersControl).ConfigureAwait(false);
            countersEnable = (byte) (countersEnable & mask);
            countersEnable |= (byte) ((enable ? 0 : 1) << 0); // note that 0=enable per EDCS-11544458
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page8.FecCountersControl, countersEnable).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecCountersControl).ConfigureAwait(false) == countersEnable;
        }

        public async Task<bool> IdleEnable(bool enable)
        {
            const byte mask = 0xFD;
            byte idleEnabled = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecTxErrorInjectionIdleEnable).ConfigureAwait(false);

            idleEnabled = (byte) (idleEnabled & mask);
            idleEnabled = (byte) (idleEnabled | (enable ? 1 << 1 : 0 << 1));

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page8.FecTxErrorInjectionIdleEnable, idleEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecTxErrorInjectionIdleEnable).ConfigureAwait(false) == idleEnabled;
        }

        public async Task<bool> ErrorInjectEnable(bool enable)
        {
            const byte mask = 0xFE;
            byte errorInjectEnabled = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecTxErrorInjectionIdleEnable).ConfigureAwait(false);

            errorInjectEnabled = (byte) (errorInjectEnabled & mask);
            errorInjectEnabled = (byte) (errorInjectEnabled | (enable ? 1 : 0));

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page8.FecTxErrorInjectionIdleEnable, errorInjectEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page8.FecTxErrorInjectionIdleEnable).ConfigureAwait(false) ==
                   errorInjectEnabled;
        }

        public async Task<double> PreFecBer() => await BipCount() / (double) await TotalWordCount() / 5440.0;

        public async Task<double> PreFecSer() => await ErrorSymbolCount() / (double) await TotalWordCount() / 544;

        public async Task<double> UnCorrectableWer() =>
            await UnCorrectableFrameCount() / (double) await TotalWordCount();

        public async Task<double> CorrectableWer() => await CorrectedFrameCount() / (double) await TotalWordCount();
    }
}