using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Support;
using WhalesTale.Communication;
using WhalesTale.QSFP;

namespace WhalesTale.QSFP100
{
    public enum SelectLane : byte
    {
        L1 = 0,
        L2 = 1,
        L3 = 2,
        L4 = 3
    }

    [Flags]
    public enum EnableLane : byte
    {
        None = 0,
        L1 = 0b0001,
        L2 = 0b0010,
        L3 = 0b0100,
        L4 = 0b1000,
        All = 0b1111
    }

    public partial class Qsfp100G : IQsfp100G, ITemperatureCalibration
    {
        public Qsfp100G(IDeviceIO deviceIO) : base(deviceIO)
        {
            Device = deviceIO;
            Host = new HostSide(this);
            Network = new NetworkSide(this);
            Fec = new FecClass(this);
        }

        public IDeviceIO Device { get; }
        public HostSide Host { get; }
        public NetworkSide Network { get; }
        public FecClass Fec { get; }

        public bool WaitForStableReading(Func<double> measureValue, double allowableChange, TimeSpan window,
            TimeSpan updateRate, TimeSpan timeout)
        {
            var numberOfPoint = window.Seconds / updateRate.Seconds;
            var data = new List<double>();
            var timerStopwatch = Stopwatch.StartNew();
            do
            {
                data.Add(measureValue.Invoke());
                if (data.Count() > numberOfPoint)
                {
                    data.RemoveAt(0);
                    Debug.WriteLine($"Max {data.Max()}, Min {data.Min()}, Delta {data.Max() - data.Min()}");
                    if (data.Max() - data.Min() <= allowableChange) return true;
                }

                Thread.Sleep(updateRate);
            } while (timerStopwatch.Elapsed < timeout);

            return false;
        }

        //=======================validated===================

        public async Task<byte[]> GetCiscoSpecificConfigurationAsync()
        {
            var data = new byte[Qsfp100GRegister.Page4.CiscoSpecificNvr.EndAddress -
                Qsfp100GRegister.Page4.CiscoSpecificNvr.StartAddress + 1];

            var page4Data = await GetPageAsync(Memory.Pages.NonVolatile.P4Upper).ConfigureAwait(false);

            Array.Copy(page4Data, Qsfp100GRegister.Page4.CiscoSpecificNvr.StartAddress - 128, data, 0,
                Qsfp100GRegister.Page4.CiscoSpecificNvr.EndAddress -
                Qsfp100GRegister.Page4.CiscoSpecificNvr.StartAddress + 1);
            return data;
        }

        public byte[] GetCiscoSpecificConfiguration() => Task.Run(GetCiscoSpecificConfigurationAsync).GetAwaiter().GetResult();
       

        public async Task<bool> Update_CalibrationAsync(int timeout=200) => await Device
            .ExecuteVendorCmdAsync(VendorCommand.Command.UpdateCalibration, timeout).ConfigureAwait(false);

        public bool Update_Calibration(int timeout) => Task.Run(() =>Update_CalibrationAsync(timeout)).GetAwaiter().GetResult();
        











        public async Task<bool> ResetPrbsValidationAsync() => 0 ==
                                                         await Device.GetRegAsync(Qsfp100GRegister.Page7
                                                             .PrbsClockPatternSelection).ConfigureAwait(false) +
                                                         await Device.GetRegAsync(Qsfp100GRegister.Page7
                                                             .PatternGenerationAdditionalNetwork).ConfigureAwait(false) +
                                                         await Device
                                                             .GetRegAsync(Qsfp100GRegister.Page7.PrbsGeneratorEnable).ConfigureAwait(false);

        public bool ResetPrbsValidation() => Task.Run(ResetPrbsValidationAsync).GetAwaiter().GetResult();


        public async Task<double> CoCoaTemperatureAsync() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.CoCoaTemperature).ConfigureAwait(false);

        public double CoCoaTemperature() => Task.Run(CoCoaTemperatureAsync).GetAwaiter().GetResult();

        public async Task<double> RssiAsync() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.RssiVoltage).ConfigureAwait(false);

        public double Rssi() => Task.Run(RssiAsync).GetAwaiter().GetResult();

        public async Task<double> Ps0P75Async() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.PowerSupply0P75).ConfigureAwait(false);

        public double Ps0P75() => Task.Run(Ps0P75Async).GetAwaiter().GetResult();

        public async Task<double> Ps1P00Async() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.PowerSupply1P0).ConfigureAwait(false);

        public double Ps1P00() => Task.Run(Ps1P00Async).GetAwaiter().GetResult();

        public async Task<double> Ps1P80Async() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.PowerSupply1P8).ConfigureAwait(false);

        public double Ps1P80() => Task.Run(Ps1P80Async).GetAwaiter().GetResult();

        public async Task<double> Ps2P70Async() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.PowerSupply2P7).ConfigureAwait(false);

        public double Ps2P70() => Task.Run(Ps2P70Async).GetAwaiter().GetResult();

        public async Task<double> RefV2P50Async() =>
            await Device.GetValueAsync(Qsfp100GRegister.Page4.ReferenceVoltage2P5).ConfigureAwait(false);

        public double RefV2P50() => Task.Run(RefV2P50Async).GetAwaiter().GetResult();

        public async Task<ushort> ErrorCodeAsync() =>
            await Device.GetRegAsync(Qsfp100GRegister.Page4.ErrorCode).ConfigureAwait(false);

        public ushort ErrorCode() => Task.Run(ErrorCodeAsync).GetAwaiter().GetResult();

        public async Task<ushort> ErrorOptionalDataAsync() =>
            await Device.GetRegAsync(Qsfp100GRegister.Page4.ErrorOptionalData).ConfigureAwait(false);

        public ushort ErrorOptionalData() => Task.Run(ErrorOptionalDataAsync).GetAwaiter().GetResult();

        public async Task RxSquelchDisableSetAsync(byte whichLanes)
        {
            if (!Functions.IsBetween(0, 15, whichLanes))
                throw new ArgumentException(
                    $"RxSquelchDisable(byte whichLanes) : whichLanes={whichLanes} must be 0 <= value <= 15.");
            var initial = await RxSquelchDisableGetAsync().ConfigureAwait(false);
            var data = whichLanes;
            initial = (byte) ((initial & 0x0F) | (data << 4));
            await Device.SetRegAsync(Qsfp100GRegister.Page3.TxRxSquelchDisable, initial).ConfigureAwait(false);
        }

        public async Task RxSquelchDisableSetAsync(EnableLane whichLanes) =>
            await RxSquelchDisableSetAsync((byte) whichLanes).ConfigureAwait(false);

        public async Task RxSquelchDisableSetAsync(bool l0, bool l1, bool l2, bool l3)
        {
            var whichLanes = (byte) (((l3 ? 1 : 0) << 3) + ((l2 ? 1 : 0) << 2) + ((l1 ? 1 : 0) << 1) + (l0 ? 1 : 0));
            await RxSquelchDisableSetAsync(whichLanes).ConfigureAwait(false);
        }

        public async Task<byte> RxSquelchDisableGetAsync()
        {
            var readResult =
                (byte) await Device.GetRegAsync(Qsfp100GRegister.Page3.TxRxSquelchDisable).ConfigureAwait(false);
            return (byte) (readResult >> 4);
        }

        public async Task TxSquelchDisableSetAsync(byte whichLanes)
        {
            if (!Functions.IsBetween(0, 15, whichLanes))
                throw new ArgumentException(
                    $"TxSquelchDisable(byte whichLanes) : whichLanes={whichLanes} must be 0 <= value <= 15.");
            var initial = await RxSquelchDisableGetAsync().ConfigureAwait(false);
            var data = whichLanes;
            initial = (byte) ((initial & 0xF0) | data);
            await Device.SetRegAsync(Qsfp100GRegister.Page3.TxRxSquelchDisable, initial).ConfigureAwait(false);
        }

        public async Task TxSquelchDisableSetAsync(EnableLane whichLanes) =>
            await TxSquelchDisableSetAsync((byte) whichLanes).ConfigureAwait(false);

        public async Task TxSquelchDisableSetAsync(bool l0, bool l1, bool l2, bool l3)
        {
            var whichLanes = (byte) (((l3 ? 1 : 0) << 3) + ((l2 ? 1 : 0) << 2) + ((l1 ? 1 : 0) << 1) + (l0 ? 1 : 0));
            await TxSquelchDisableSetAsync(whichLanes).ConfigureAwait(false);
        }

        public async Task<byte> TxSquelchDisableGetAsync()
        {
            var readResult =
                (byte) await Device.GetRegAsync(Qsfp100GRegister.Page3.TxRxSquelchDisable).ConfigureAwait(false);
            return (byte) (readResult & 0x0F);
        }

        public async Task<bool> FEC_KR4_BypassEnableAsync(bool enable) =>
            await Device.SetRegAsync(Qsfp100GRegister.Page3.HostModeSelect, 0x08).ConfigureAwait(false);

        public async Task<bool> FEC_KR4_BypassStateAsync()
        {
            var readResult = await Device.GetRegAsync(Qsfp100GRegister.Page3.HostModeSelect).ConfigureAwait(false);
            return Support.Bit.IsBitSet(readResult, 3);
        }


        public async Task TxAdaptiveEqualizationSetAsync(byte whichLanes)
        {
            byte initial = await Device.GetRegAsync(Qsfp100GRegister.Page3.RxOutputDisableTxAdpEqualization)
                .ConfigureAwait(false);
            initial = (byte) ((initial & 0xF0) | whichLanes);
            await Device.SetRegAsync(Qsfp100GRegister.Page3.RxOutputDisableTxAdpEqualization, initial);
        }

        public async Task TxAdaptiveEqualizationSetAsync(EnableLane whichLanes) =>
            await TxAdaptiveEqualizationSetAsync((byte) whichLanes).ConfigureAwait(false);

        public async Task TxAdaptiveEqualizationSetAsync(bool l0, bool l1, bool l2, bool l3)
        {
            var whichLanes = (byte) (((l3 ? 1 : 0) << 3) + ((l2 ? 1 : 0) << 2) + ((l1 ? 1 : 0) << 1) + (l0 ? 1 : 0));
            await TxAdaptiveEqualizationSetAsync(whichLanes).ConfigureAwait(false);
        }

        public async Task<byte> TxAdaptiveEqualizationGetAsync()
        {
            var readResult = (byte) await Device.GetRegAsync(Qsfp100GRegister.Page3.RxOutputDisableTxAdpEqualization)
                .ConfigureAwait(false);
            return (byte) (readResult & 0x0F);
        }

        public async Task<bool> VendorTestModeSetAsync(ushort mode) => await Device
            .SetRegAsync(Qsfp100GRegister.Page4.VendorTestMode, (byte) mode).ConfigureAwait(false);

        public async Task<ushort> VendorTestModeGetAsync() =>
            await Device.GetRegAsync(Qsfp100GRegister.Page4.VendorTestMode).ConfigureAwait(false);

        #region Private Methods

        #endregion
    }
}