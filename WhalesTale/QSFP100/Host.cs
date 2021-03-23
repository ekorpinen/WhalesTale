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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Support;

namespace WhalesTale.QSFP100

{
    public class HostSide : IHost
    {
        private readonly Qsfp100G _parent;

        public HostSide(Qsfp100G parent) => _parent = parent;

        public async Task<bool> ClockSource(Qsfp100GRegister.Page7.ClockSourceMaskReg129 whichOne)
        {
            byte currentByte = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false);
            currentByte &= 0x7F;
            currentByte |= (byte) ((byte) whichOne << 3);
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection, currentByte).ConfigureAwait(false);

            //validate
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false) == currentByte;
        }

        public async Task<bool> PatternSelect(Qsfp100GRegister.Page7.PatternMaskReg129 whichOne)
        {
            byte currentByte = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false);
            currentByte &= 0x8F;
            currentByte |= (byte) ((byte) whichOne << 0);
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection, currentByte).ConfigureAwait(false);

            //validate
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsClockPatternSelection).ConfigureAwait(false) == currentByte;
        }

        public async Task<bool> PatternSelect(Qsfp100GRegister.Page7.PatternMaskReg166 whichOne)
        {
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PatternGenerationAdditionalNetwork, (byte) whichOne).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PatternGenerationAdditionalNetwork).ConfigureAwait(false) ==
                   (byte) whichOne;
        }

        public async Task<bool> PatternGeneratorEnable(EnableLane enable)
        {
            const byte mask = 0xF0;
            byte generatorEnabled = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable).ConfigureAwait(false);

            generatorEnabled = (byte) (generatorEnabled & mask);
            generatorEnabled = (byte) (generatorEnabled | (byte) enable);

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable, generatorEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable).ConfigureAwait(false) == generatorEnabled;
        }

        public async Task<bool> ErrorCheckerEnable(EnableLane enable)
        {
            const byte mask = 0xF0;
            byte checkersEnabled = await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsCheckerEnable).ConfigureAwait(false);

            checkersEnabled = (byte) (checkersEnabled & mask);
            checkersEnabled = (byte) (checkersEnabled | (byte) enable);

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsCheckerEnable, checkersEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsCheckerEnable).ConfigureAwait(false) == checkersEnabled;
        }

        public async Task<bool> ErrorCounterFreeze(EnableLane freeze) // note false means UPDATE error counter
        {
            byte counterUpdateEnabled =
                await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze).ConfigureAwait(false);
            counterUpdateEnabled |= (byte) freeze;

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze, counterUpdateEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze).ConfigureAwait(false) ==
                   counterUpdateEnabled;
        }


        public async Task<bool> ErrorCheckerLocked(SelectLane lane) =>
            Bit.IsBitSet(await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsHostCheckerLock).ConfigureAwait(false), (byte) lane);

        public async Task<ulong> BitCount(SelectLane lane)
        {
            int data = (int) lane switch
            {
                0 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitCountHostL1).ConfigureAwait(false),
                1 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitCountHostL2).ConfigureAwait(false),
                2 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitCountHostL3).ConfigureAwait(false),
                3 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitCountHostL4).ConfigureAwait(false),
                _ => throw new InvalidDataException("Lane must be between 0 and 3 inclusive.")
            };

            var bitCountExponent = (ushort) ((data >> 10) & 0x3f);
            var bitCountMantissa = (ushort) (data & 0x3ff);
            return bitCountMantissa * (ulong) Math.Pow(2, bitCountExponent);
        }

        public async Task<ulong> ErrorCount(SelectLane lane)
        {
            int data = (int) lane switch
            {
                0 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitErrorHostL1).ConfigureAwait(false),
                1 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitErrorHostL2).ConfigureAwait(false),
                2 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitErrorHostL3).ConfigureAwait(false),
                3 => await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitErrorHostL4).ConfigureAwait(false),
                _ => throw new InvalidDataException("Lane must be between 0 and 3 inclusive.")
            };

            var bitCountExponent = (ushort) ((data >> 10) & 0x3f);
            var bitCountMantissa = (ushort) (data & 0x3ff);
            return bitCountMantissa * (ulong) Math.Pow(2, bitCountExponent);
        }

        public async Task<List<(SelectLane Lane, double BER, ulong BitCount, ulong ErrorCount, bool Locked)>> BerAsync(
            EnableLane lanes, TimeSpan timeSpan)
        {
            //ErrorCheckerEnable(EnableLane.NONE);
            //await Task.Delay(250);
            //ErrorCheckerEnable(EnableLane.All);
            await ErrorCounterFreeze(EnableLane.All);
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            await ErrorCounterClear(EnableLane.All);
            //var test = BitCount(SelectLane.L1);
            await Task.Delay(timeSpan);
            await ErrorCounterFreeze(EnableLane.All);
            await Task.Delay(TimeSpan.FromMilliseconds(500));
            var selectedLane = SelectLane.L1;
            var result = new List<(SelectLane, double, ulong, ulong, bool)>();
            foreach (var @enum in GetUniqueFlags(lanes))
            {
                var lane = (EnableLane) @enum;
                selectedLane = lane switch
                {
                    EnableLane.L1 => SelectLane.L1,
                    EnableLane.L2 => SelectLane.L2,
                    EnableLane.L3 => SelectLane.L3,
                    EnableLane.L4 => SelectLane.L4,
                    _ => selectedLane
                };
                {
                    var bitCount = BitCount(selectedLane);
                    var errorCount = ErrorCount(selectedLane);
                    var ber = await BitCount(selectedLane) == 0
                        ? 1
                        : await ErrorCount(selectedLane) / (double) await BitCount(selectedLane);
                    result.Add((selectedLane, ber, await bitCount, await errorCount,
                        await ErrorCheckerLocked(selectedLane)));
                }
            }

            return result;
        }

        public async Task<double> Ber() => await ErrorCount(0) / (double) await BitCount(0);

        public async Task<bool> ErrorCounterClear(EnableLane clear) // note false means UPDATE error counter
        {
            byte counterUpdateEnabled =
                await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze).ConfigureAwait(false);

            counterUpdateEnabled = (byte) (counterUpdateEnabled & ~(byte) clear);

            // validate
            await _parent.Device.SetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze, counterUpdateEnabled).ConfigureAwait(false);
            return await _parent.Device.GetRegAsync(Qsfp100GRegister.Page7.PrbsBitAndErrorCountUpdateFreeze).ConfigureAwait(false) ==
                   counterUpdateEnabled;
        }

        public static IEnumerable<Enum> GetUniqueFlags(Enum flags)
        {
            ulong flag = 1;
            foreach (var value in Enum.GetValues(flags.GetType()).Cast<Enum>())
            {
                var bits = Convert.ToUInt64(value);
                while (flag < bits) flag <<= 1;

                if (flag == bits && flags.HasFlag(value)) yield return value;
            }
        }
    }
}