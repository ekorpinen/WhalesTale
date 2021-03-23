using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Support;
using WhalesTale.Communication;
using WhalesTale.QSFP100;

namespace WhalesTale.QSFP
{
    public class Qsfp : IQsfp
    {
        private const byte RxLosMask = 0x01;
        public Qsfp(IDeviceIO deviceIo) => Device = deviceIo;
        private IDeviceIO Device { get; }

        public async Task<List<NvrRange>> LoadNvrRanges(string filename)
        {
            var results = new List<NvrRange>();
            using var nvr = new StreamReader(filename);
            await nvr.ReadLineAsync(); // read header line
            await nvr.ReadLineAsync(); // column header line
            while (!nvr.EndOfStream)
            {
                var tokens = (await nvr.ReadLineAsync())?.Split(',');
                var rec = tokens?[0] switch
                {
                    "0L" => NvrRange.CreateInstance(Memory.Pages.NonVolatile.P0Lower, int.Parse(tokens[1]),
                        int.Parse(tokens[2])),
                    "0U" => NvrRange.CreateInstance(Memory.Pages.NonVolatile.P0Upper, int.Parse(tokens[1]),
                        int.Parse(tokens[2])),
                    "1" => NvrRange.CreateInstance(Memory.Pages.NonVolatile.P1Upper, int.Parse(tokens[1]),
                        int.Parse(tokens[2])),
                    "2" => NvrRange.CreateInstance(Memory.Pages.NonVolatile.P2Upper, int.Parse(tokens[1]),
                        int.Parse(tokens[2])),
                    "3" => NvrRange.CreateInstance(Memory.Pages.NonVolatile.P3Upper, int.Parse(tokens[1]),
                        int.Parse(tokens[2])),
                    "4" => NvrRange.CreateInstance(Memory.Pages.NonVolatile.P4Upper, int.Parse(tokens[1]),
                        int.Parse(tokens[2])),
                    _ => null
                };
                results.Add(rec);
            }

            return results;
        }

        public List<(Memory, int)> CheckPageForErrors(byte[] original, byte[] current, List<NvrRange> aPageOfNvrRanges)
        {
            if (aPageOfNvrRanges.Count == 0)
                throw new ArgumentException("Must have a nvr range defined to check for error; 0 ranges is invalid.");
            var badAddress = new List<(Memory, int)>();
            aPageOfNvrRanges.ForEach(range =>
            {
                for (var address = range.BeginAddress - aPageOfNvrRanges[0].Page.StartAddress;
                    address <= range.EndAddress - aPageOfNvrRanges[0].Page.StartAddress;
                    address++)
                    if (original[address] != current[address])
                        badAddress.Add((range.Page, address + aPageOfNvrRanges[0].Page.StartAddress));
            });
            return badAddress;
        }

        public async Task<byte[]> GetPageAsync(Memory page) =>
            await Device.ReadAsync(page.Page, page.StartAddress, page.Length).ConfigureAwait(false);

        public byte[] GetPage(Memory page)
        {
            return Task.Run(() => GetPageAsync(page)).GetAwaiter().GetResult();
        }

        public async Task<double> TxPowerAsync() => await Device.GetValueAsync(Sff8636.Page0.Lower.QsfpTxPowerL1);
        public double TxPower() => Task.Run(TxPowerAsync).GetAwaiter().GetResult();
        public async Task<double> TemperatureAsync() => await Device.GetValueAsync(Sff8636.Page0.Lower.QsfpTemperature);
        public double Temperature() => Task.Run(TemperatureAsync).GetAwaiter().GetResult();

        public async Task<string> SerialNumberAsync() =>
            await Device.GetRegAsync(Sff8636.Page0.Upper.QsfpVendorSn, new CancellationToken()).ConfigureAwait(false);

        public string SerialNumber() => Task.Run(SerialNumberAsync).GetAwaiter().GetResult();

        public async Task<string> FirmwareVersionAsync()
        {
            var readSnRead = await Device.GetRegAsync(Sff8636.Page2.QsfpFirmwareVersionIdentifier, new CancellationToken());
            return readSnRead.Insert(2, ".");
        }

        public string FirmwareVersion() => Task.Run(FirmwareVersionAsync).GetAwaiter().GetResult();

        public async Task<(List<int> TxCdrLol, List<int> RxCdrLol)> RxLolAsync()
        {
            var readResult = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpLatchedTxRxCdrLolFlag,new CancellationToken())
                .ConfigureAwait(false);
            var rxCdrLol = new List<int>
            {
                (readResult >> 0) & 0x01,
                (readResult >> 1) & 0x01,
                (readResult >> 2) & 0x01,
                (readResult >> 3) & 0x01
            };
            var txCdrLol = new List<int>
            {
                (readResult >> 4) & 0x01,
                (readResult >> 5) & 0x01,
                (readResult >> 6) & 0x01,
                (readResult >> 7) & 0x01
            };
            return (txCdrLol, rxCdrLol);
        }

        public (List<int> TxCdrLol, List<int> RxCdrLol) RxLol() => Task.Run(RxLolAsync).GetAwaiter().GetResult();

        public async Task<List<int>> TxManualCtleAsync()
        {
            var readResult = await Device.GetRegAsync(Sff8636.Page3.QsfpInputEqualization, new CancellationToken()).ConfigureAwait(false);
            var result = new List<int>
            {
                (readResult >> 0) & 0x0F,
                (readResult >> 4) & 0x0F,
                (readResult >> 8) & 0x0F,
                (readResult >> 12) & 0x0F
            };
            return result;
        }

        public List<int> TxManualCtle() => Task.Run(TxManualCtleAsync).GetAwaiter().GetResult();

        public async Task<double> RxPowerAsync() => await Device.GetValueAsync(Sff8636.Page0.Lower.QsfpRxPowerL1);
        public double RxPower() => Task.Run(RxPowerAsync).GetAwaiter().GetResult();

        public async Task<double> SupplyVoltageAsync() =>
            await Device.GetValueAsync(Sff8636.Page0.Lower.QsfpSupplyVoltage).ConfigureAwait(false);

        public double SupplyVoltage() => Task.Run(SupplyVoltageAsync).GetAwaiter().GetResult();

        public async Task<double> TxBiasAsync() => await Device.GetValueAsync(Sff8636.Page0.Lower.QsfpTxBiasL1);

        public double TxBias() => Task.Run(TxBiasAsync).GetAwaiter().GetResult();

        public async Task<bool> RxLosAsync()
        {
            var readResult = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpLatchedTxRxLosIndicators, new CancellationToken())
                .ConfigureAwait(false);
            return ((byte) readResult & RxLosMask) <= 0;
        }

        public bool RxLos() => Task.Run(RxLosAsync).GetAwaiter().GetResult();

        public async Task<bool> DataNotReadyAsync()
        {
            var readResult = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpStatus, new CancellationToken()).ConfigureAwait(false);
            return ((int) readResult).IsBitSet(0);
        }

        public bool DataNotReady() => Task.Run(DataNotReadyAsync).GetAwaiter().GetResult();

        public async Task<bool> TxDisableAsync(EnableLane enable)
        {
            const byte mask = 0xF0;
            // read original register value
            byte txEnable = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpTxDisable, new CancellationToken()).ConfigureAwait(false);

            // set appropriate bit
            txEnable = (byte) (txEnable & mask);
            txEnable = (byte) (txEnable | (byte) enable);

            CancellationToken ct;
            //  set and validate
            await Device.SetRegAsync(Sff8636.Page0.Lower.QsfpTxDisable, txEnable,ct ).ConfigureAwait(false);
            return await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpTxDisable, new CancellationToken()).ConfigureAwait(false) == txEnable;
        }

        public bool TxDisable(EnableLane enable) => Task.Run(() => TxDisableAsync(enable)).GetAwaiter().GetResult();

        public async Task<byte> TxDisableStateAsync()
        {
            byte readResults = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpTxDisable, new CancellationToken()).ConfigureAwait(false);
            return readResults;
        }

        public byte TxDisableState() => Task.Run(TxDisableStateAsync).GetAwaiter().GetResult();

        public async Task<List<int>> TxAdaptiveCtleAsync()
        {
            var readResult = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpTxL1L2InputEqualization, new CancellationToken())
                .ConfigureAwait(false);
            var result = new List<int>
            {
                (readResult >> 4) & 0x0F,
                (readResult >> 0) & 0x0F
            };
            readResult = await Device.GetRegAsync(Sff8636.Page0.Lower.QsfpTxL3L4InputEqualization, new CancellationToken())
                .ConfigureAwait(false);
            result.Add((readResult >> 4) & 0x0F);
            result.Add((readResult >> 0) & 0x0F);
            return result;
        }

        public List<int> TxAdaptiveCtle() => Task.Run(TxAdaptiveCtleAsync).GetAwaiter().GetResult();


        #region PRIVATE METHODS

        #endregion PRIVATE METHODS
    }
}