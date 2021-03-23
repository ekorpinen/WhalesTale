using System;
using System.Linq;
using System.Threading.Tasks;
using WhalesTale.QSFP;

namespace WhalesTale.QSFP100
{
    public interface ITemperatureCalibration
    {
        Task<byte[]> ZeroTemperatureOffsetsAsync();
        Task<byte[]> SetModuleTemperatureOffsetsAsync(double offsetAt0C, double offsetAt75C);
        Task<bool> CalibrateModuleTemperatureAsync(double offsetAt0C, double offsetAt75C);
    }

    public partial class Qsfp100G : Qsfp, IQsfp100G, ITemperatureCalibration
    {
        public async Task<byte[]> ZeroTemperatureOffsetsAsync() =>
            await SetModuleTemperatureOffsetsAsync(0, 0).ConfigureAwait(false);

        /// <summary>
        ///     Calibrate module temperature by setting offset registers.
        ///     Offsets are subtracted from the measured value to create the reported value.
        ///     Example:  module is actually @ 25C.
        ///     The reported temperature is 35C.
        ///     You would call this method CalibrateModuleTemperature(10.0,10.0)
        /// </summary>
        /// <param name="offsetAt0C"></param>
        /// <param name="offsetAt75C"></param>
        public async Task<bool> CalibrateModuleTemperatureAsync(double offsetAt0C, double offsetAt75C)
        {
            return await Task.Run(async () =>
            {
                //scale offset temperatures to counts
                var offset0C = BitConverter
                    .GetBytes((short) (offsetAt0C / Qsfp100GRegister.Page4.TemperatureOffset0C.Register.Scale))
                    .Reverse().ToArray();
                var offset75C = BitConverter
                    .GetBytes((short) (offsetAt75C / Qsfp100GRegister.Page4.TemperatureOffset75C.Register.Scale))
                    .Reverse().ToArray();

                // write values and update cal
                await Device.SetRegAsync(Qsfp100GRegister.Page4.TemperatureOffset0C, offset0C);
                await Device.SetRegAsync(Qsfp100GRegister.Page4.TemperatureOffset75C, offset75C);
                await Update_CalibrationAsync().ConfigureAwait(false);


                const int startConfigAddress = 192;
                const int endConfigAddress = 254;
                var writeData = new byte[Qsfp100GRegister.Page4.CiscoSpecificNvr.EndAddress -
                    Qsfp100GRegister.Page4.CiscoSpecificNvr.StartAddress + 1];
                var readBackData = new byte[Qsfp100GRegister.Page4.CiscoSpecificNvr.EndAddress -
                    Qsfp100GRegister.Page4.CiscoSpecificNvr.StartAddress + 1];

                var page4Data = GetPage(Memory.Pages.NonVolatile.P4Upper);
                Array.Copy(page4Data, startConfigAddress - 128, writeData, 0, endConfigAddress - startConfigAddress);

                var checkSum = UtilityFunctions.ComputeCheckSum(writeData);
                _ = await Device.SetRegAsync(Qsfp100GRegister.Page4.ModuleConfigCheckSum, checkSum).ConfigureAwait(false);
                await Update_CalibrationAsync().ConfigureAwait(false);

                // validate page 4 upper NVR after device reset.
                //  var res = await DutGpio.Reset(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(2500));

                page4Data = GetPage(Memory.Pages.NonVolatile.P4Upper);
                Array.Copy(page4Data, startConfigAddress - 128, readBackData, 0, endConfigAddress - startConfigAddress);

                return readBackData.SequenceEqual(writeData);
            });
        }

        public async Task<byte[]> SetModuleTemperatureOffsetsAsync(double offsetAt0C, double offsetAt75C)
        {
            //scale offset temperatures to counts
            var offset0C = BitConverter
                .GetBytes((short) (offsetAt0C / Qsfp100GRegister.Page4.TemperatureOffset0C.Register.Scale)).Reverse()
                .ToArray();
            var offset75C = BitConverter
                .GetBytes((short) (offsetAt75C / Qsfp100GRegister.Page4.TemperatureOffset75C.Register.Scale)).Reverse()
                .ToArray();

            // write values and update cal
            await Device.SetRegAsync(Qsfp100GRegister.Page4.TemperatureOffset0C, offset0C);
            await Device.SetRegAsync(Qsfp100GRegister.Page4.TemperatureOffset75C, offset75C);
            await Update_CalibrationAsync().ConfigureAwait(false);
            return GetCiscoSpecificConfiguration();
        }
    }
}