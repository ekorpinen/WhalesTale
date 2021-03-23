using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Polly;
using Support;
using WhalesTale.Communication;
using WhalesTale.QSFP100;

namespace WhalesTale.Cyclops
{
    public class Cyclops : ICyclops
    {
        public bool VoaTablesAreLoaded;

        public Cyclops(IDeviceIO deviceIO) => Device = deviceIO;

        
        private IDeviceIO Device { get; }


        //public async Task<bool>
        //    SetCiscoCoCoaRegAsync(uint address, ushort len, uint[] data, TimeSpan? timeOut = null) =>
        //    await Device.SetCiscoCoCoaRegAsync(address, len, data);

        //public async Task<dynamic> GetCiscoCoCoaRegAsync(uint address, ushort len, TimeSpan? timeOut = null) =>
        //    await Device.GetCiscoCoCoaRegAsync(address, len, timeOut);


        public async Task<byte[]> GetCocoaStatusPageAsync(CyclopsRegisters.StatusPages whichPage,
            TimeSpan? timeout = null)
        {
            if (!Enum.IsDefined(typeof(CyclopsRegisters.StatusPages), whichPage))
                throw new ArgumentOutOfRangeException($"Page {whichPage} is not a valid COCOA status page.");
            timeout ??= TimeSpan.FromSeconds(3);

            var data = new byte[256];

            Device.SetVendorCommandParameters((byte) whichPage, 0, 256);

            if (!await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaReadCipPage,
                Convert.ToInt32(timeout.Value.TotalMilliseconds)).ConfigureAwait(false))
                throw new Exception("Failed to execute Vendor Command CiscoCocoaReadCipPage");

            Device.Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 128).CopyTo(data, 0);
            Device.Read(VendorCommand.Page3, VendorCommand.Buffer.Address, 128).CopyTo(data, 128);

            return data;
        }

        public async Task<byte[]> GetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages whichPage,
            TimeSpan? timeout = null)
        {
            if (!Enum.IsDefined(typeof(CyclopsRegisters.ConfigPages), whichPage))
                throw new ArgumentOutOfRangeException($"Page {whichPage} is not a valid COCOA config page.");
            timeout ??= TimeSpan.FromSeconds(3);

            Device.SetVendorCommandParameters((byte) whichPage, 0, 256);

            if (!await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CocoaLoadPageCfg,
                Convert.ToInt32(timeout.Value.TotalMilliseconds)).ConfigureAwait(false))
                throw new Exception("Failed to execute Vendor Command CocoaLoadPageCfg");

            var data = new byte[256];
            Device.Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 128).CopyTo(data, 0);
            Device.Read(VendorCommand.Page3, VendorCommand.Buffer.Address, 128).CopyTo(data, 128);

            var computeCheckSum = UtilityFunctions.ComputeCheckSum(data);
            var storedChecksum = Device.ReadByte(VendorCommand.Page1, 134);
            if (computeCheckSum != storedChecksum)
                throw new DataMisalignedException($"Computed checksum {computeCheckSum} " +
                                                  $"not equal to stored checksum {storedChecksum} " +
                                                  $"for page {whichPage} ");
            return data;
        }

        public async Task<bool> SetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages whichPage, byte[] data,
            TimeSpan? timeout = null)
        {
            timeout ??= TimeSpan.FromSeconds(3);

            Device.SetVendorCommandParameters((byte) whichPage, 0, 256);

            //Device.WriteByte(VendorCommand.Page1, VendorCommand.Params.ChecksumAddress,
            //    UtilityFunctions.ComputeCheckSum(data));

            await Device.SetRegAsync(Qsfp100GRegister.Page4.CoCoaCheckSum, UtilityFunctions.ComputeCheckSum(data))
                .ConfigureAwait(false);


            var array1 = new byte[128];
            var array2 = new byte[128];

            if (data.Length <= 128)
            {
                //Device.Write(VendorCommand.Page2, 128, data);
                await Device.SetRegAsync(Qsfp100GRegister.Page5.CommandBuffer1, data).ConfigureAwait(false);
            }
            else
            {
                Array.Copy(data, 0, array1, 0, 128);
                Array.Copy(data, 128, array2, 0, (byte) (data.Length - 128));
                //Device.Write(VendorCommand.Page2, 128, array1);
                //Device.Write(VendorCommand.Page3, 128, array2);
                await Device.SetRegAsync(Qsfp100GRegister.Page5.CommandBuffer1, array1).ConfigureAwait(false);
                await Device.SetRegAsync(Qsfp100GRegister.Page6.CommandBuffer2, array2).ConfigureAwait(false);
            }

            return await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CocoaSavePageCfg,
                Convert.ToInt32(timeout.Value.TotalMilliseconds)).ConfigureAwait(false);
        }



        // CoCOA Temperature Calibration Methods
        public async Task<byte[]> SetCoCoaTemperatureOffsetToPorValuesAsync()
        {
            return await SetCoCoaTemperatureOffsetAsync(new byte[] {0xFE, 0xD3, 0X00}).ConfigureAwait(false);
        }

        //public void SetCoCOATemperatureOffset(byte[] data)
        //{

        //    var result = new byte[] { 0xFE, 0xD3, 0X00 };
        //    SetCoCOATemperatureOffset(result);
        //}

        public async Task<byte[]> CalibrateCoCoaTemperatureAsync(int moduleTemperatureCount, int coCoaTemperatureCount)
        {
            var offset = 0;

            //this gets the offset as 3 bytes and converts it to a signed int
            var offsetBytes = await GetTemperatureOffsetBytesAsync().ConfigureAwait(false);
            for (var i = 0; i < 3; i++) offset = (offset << 8) | offsetBytes[i];
            if ((offset & 0x800000) != 0) offset |= unchecked((int) 0xFF000000);

            //do the math, and truncate the offset back to 3 bytes
            //offset += (int)(Temperature * 256 - CoCOATemperature * 256);
            offset += moduleTemperatureCount - coCoaTemperatureCount;
            offset &= 0xFFFFFF;

            var result = new byte[3];
            Array.Copy(BitConverter.GetBytes(offset), result, 3);
            if (BitConverter.IsLittleEndian) Array.Reverse(result);

            return await SetCoCoaTemperatureOffsetAsync(result).ConfigureAwait(false);
        }

        public async Task<byte[]> SetCoCoaTemperatureOffsetAsync(byte[] data)
        {
            if (data.Length != 3)
                throw new TargetParameterCountException
                    ($"SetCoCOATemperature offset requires 3 bytes, was give {data.Length}");

            var pageData =
                await GetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages.Page00)
                    .ConfigureAwait(false); // read existing data
            data.CopyTo(pageData, 236); // insert new CoCOA temperature offsets
            await SetCocoaConfigPageAsync((byte) CyclopsRegisters.ConfigPages.Page00, pageData)
                .ConfigureAwait(false); // set data into module
            return await GetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages.Page00)
                .ConfigureAwait(false); // reread page to throw error if checksum is bad
        }

       


        public async Task<int> GetVoaIndexAsync()
        {
            //Your main logging loop should read the VOA setpoints from P1/R126-145, and compare it to the VOA table.
            //All values in the temperature LUT are the same so you only need one.  Units are 1uW/bit.  (Same as attenuation table)
            //The arm is not really needed but could be read from P1/R115.

            var policyResult = await Policy
                .Handle<Exception>()
                .RetryAsync(3, onRetry: async (exception, retryCount) =>
                {
                    Debug.WriteLine($"GetVoaIndexAsync error (retry {retryCount} : {exception.Message})");
                    await Task.Delay(2000);
                })
                .ExecuteAndCaptureAsync(async () =>
                {
                    if (!VoaTablesAreLoaded) await LoadVoaTableAsync();

                    var voa0Data = await Device.GetRegAsync(CyclopsRegisters.Page01.Voa096CA2D);
                    var voa1Data = await Device.GetRegAsync(CyclopsRegisters.Page01.Voa196CA2D);


                    var voa0Index = _voa0Table.IndexOf(voa0Data);
                    var voa1Index = _voa1Table.IndexOf(voa1Data);
                    if (voa0Index == voa1Index)
                        return voa0Index;
                    throw new Exception(
                        $"Failed to find valid VOA index, {voa0Index} not equal to {voa1Index}: GetVoaIndexAsync");
                });
            return policyResult.Result;
        }

        public async Task<bool> SetVpolyAsync(int whichVpoly, byte[] data)
        {
            if (!Functions.IsBetween(0, 4, whichVpoly))
                throw new ArgumentOutOfRangeException($"Vpoly {whichVpoly} is not between 0 and 4 inclusive");
            var
                policyResult = await Policy
                    .Handle<Exception>()
                    .RetryAsync(3, onRetry: async (exception, retryCount) =>
                    {
                        Debug.WriteLine($"SetVpolyAsync error (retry {retryCount} : {exception.Message})");
                        await Task.Delay(2000);
                    })
                    .ExecuteAndCaptureAsync(async () =>
                        {
                            var vpoly0MsbPolyResult =
                                await Device.GetRegAsync(CyclopsRegisters.Page05.VPOLY0_VAL_REG_MSB);
                            var vpoly0LsbPolyResult =
                                await Device.GetRegAsync(CyclopsRegisters.Page05.VPOLY0_VAL_REG_LSB);

                            bool success1 = false, success2 = false;
                            switch (whichVpoly)
                            {
                                case 0:
                                    success1 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY0_VAL_REG_MSB,
                                        data[0]);
                                    success2 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY0_VAL_REG_LSB,
                                        data[1]);
                                    break;
                                case 1:
                                    success1 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY1_VAL_REG_MSB,
                                        data[0]);
                                    success2 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY1_VAL_REG_LSB,
                                        data[1]);
                                    break;
                                case 2:
                                    success1 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY2_VAL_REG_MSB,
                                        data[0]);
                                    success2 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY2_VAL_REG_LSB,
                                        data[1]);
                                    break;
                                case 3:
                                    success1 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY3_VAL_REG_MSB,
                                        data[0]);
                                    success2 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY3_VAL_REG_LSB,
                                        data[1]);
                                    break;
                                case 4:
                                    success1 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY4_VAL_REG_MSB,
                                        data[0]);
                                    success2 = await Device.SetCiscoIpRegAsync(
                                        CyclopsRegisters.Page05.VPOLY4_VAL_REG_LSB,
                                        data[1]);
                                    break;
                            }

                            if (!success1 || !success2)
                                throw new Exception($"Error setting SetVpolyAsync : Vpoly{whichVpoly}.");

                            await ToggleCopyConfigAsync();

                            return true;
                        }
                    );
            return policyResult.Result;
        }


        public async Task<bool> ToggleCopyConfigAsync()
        {
            var policyResult = await Policy
                .Handle<Exception>()
                .RetryAsync(3, onRetry: async (exception, retryCount) =>
                {
                    Debug.WriteLine($"ToggleCopyConfig error (retry {retryCount} : {exception.Message})");
                    await Task.Delay(2000);
                })
                .ExecuteAndCaptureAsync(async () => await Device.ExecuteVendorCmdAsync(
                    VendorCommand.Command.CiscoCocoaToggleCopyConfig,
                    1000, 1000));
            return policyResult.Result;
        }


        public (int VOAO_Arm, int VOA1_Arm) VOA_Arm()
        {
            int arm0 = 0, arm1 = 0;
            //string path = @"c:\TestResults\VOAMIN.csv";
            //using (var file = File.Exists(path) ? File.Open(path, FileMode.Append) : File.Open(path, FileMode.CreateNew))
            //using (var writer = new StreamWriter(file))
            //{

            //    byte result = (byte)GetRegII(CyclopsRegisters.Page01.POWER_SELECT_LEFT_ARM_REGISTER);
            //    arm0 = result.IsBitSet(1) ? 1 : 0;
            //    arm1 = result.IsBitSet(2) ? 1 : 0;
            //}
            return (arm0, arm1);
        }

        public (double VOA0_Power, double VOA1_Power) VOA_Power()
        {
            double voa0Power = 0, voa1Power = 0;
            //string path = @"c:\TestResults\VOAMIN.csv";
            //using (var file = File.Exists(path) ? File.Open(path, FileMode.Append) : File.Open(path, FileMode.CreateNew))
            //using (var writer = new StreamWriter(file))
            //{

            //    VOA0_Power = GetValue(CyclopsRegisters.Page01.VOA0_M32C_A2D);
            //    VOA1_Power = GetValue(CyclopsRegisters.Page01.VOA1_M32C_A2D);
            //    writer.WriteLine($"VOA0_Power {VOA0_Power}");
            //    writer.WriteLine($"VOA1_Power {VOA1_Power}");

            //}
            return (voa0Power, voa1Power);
        }

        #region Public Methods:Voltages

        public async Task<double> VIbiasAsync() => await Device.GetValueAsync(CyclopsRegisters.Page33.VIbias);

        public double VIbias() => Task.Run(VIbiasAsync).GetAwaiter().GetResult();

        public async Task<double> V1P0Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.V1P0);

        public double V1P0() => Task.Run(V1P0Async).GetAwaiter().GetResult();

        public async Task<double> V1P8Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.V1P8);

        public double V1P8() => Task.Run(V1P8Async).GetAwaiter().GetResult();

        public async Task<double> V0P8Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.V0P8);

        public double V0P8() => Task.Run(V0P8Async).GetAwaiter().GetResult();

        public async Task<double> V2P7Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.V2P7);

        public double V2P7() => Task.Run(V2P7Async).GetAwaiter().GetResult();

        public async Task<double> BiasAsync() => await Device.GetValueAsync(CyclopsRegisters.Page32.Bias);

        public double Bias() => Task.Run(BiasAsync).GetAwaiter().GetResult();

        #endregion

        #region Public Methods:IFF

        public async Task<double> Iff1Async() => await Device.GetValueAsync(CyclopsRegisters.Page32.Iff1);

        public double Iff1() => Task.Run(Iff1Async).GetAwaiter().GetResult();

        public async Task<double> Iff2Async() => await Device.GetValueAsync(CyclopsRegisters.Page32.Iff2);

        public double Iff2() => Task.Run(Iff2Async).GetAwaiter().GetResult();

        #endregion

        #region Public Methods:IPMON

        public async Task<double> IpmonAsync(CancellationToken ct = default) =>
            await Device.GetValueAsync(CyclopsRegisters.Page35.Ipmon, ct);

        public double Ipmon(CancellationToken ct = default) => Task.Run(() =>IpmonAsync(ct), ct).GetAwaiter().GetResult();

        public async Task<int> IpmonCount(CancellationToken ct = default) =>
            await Device.GetRegAsync(CyclopsRegisters.Page35.IpmonCntr,  ct);

        #endregion

        #region PRIVATE METHODS

        private List<int> _voa0Table;
        private List<int> _voa1Table;


        private async Task LoadVoaTableAsync(TimeSpan? timeout = null)
        {
            //The vendor command will load the VOA table to P5
            //Each entry in the attenuation table is 3 bytes.  First byte is arm (0 for right, 1 for left)
            //Second byte is power MSB, 3rd byte is power LSB.  Power is 1uW/bit
            //Unused bytes in the attenuation table are 0xFF
            var timeoutValue = timeout ?? TimeSpan.FromSeconds(10);

            _voa0Table = new List<int>();

            _ = await Device.SetRegAsync(Qsfp100GRegister.Page4.CoCoaVoaSelectorOrLoopback, 0);
            _ = await Device.SetRegAsync(Qsfp100GRegister.Page4.CoCoaLength, 128);
            Device.SetVendorCommandParameters(4, 0, 128);
            var executeVendorCmdResult = await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CocoaLoadVoaAttTab,
                Convert.ToInt32(timeoutValue.TotalMilliseconds));

            if (!executeVendorCmdResult) throw new Exception("Failed to execute Vendor Command CocoaLoadVoaAttTab");
            var data = Device.Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 128);

            for (var i = 0; 3 * i < 128 - 2 && data[3 * i] != 0xFF; i++)
                _voa0Table.Add((data[3 * i + 1] << 8) | data[3 * i + 2]);


            _voa1Table = new List<int>();

            await Device.SetRegAsync(Qsfp100GRegister.Page4.CoCoaVoaSelectorOrLoopback, 1);
            await Device.SetRegAsync(Qsfp100GRegister.Page4.CoCoaLength, 128);
            Device.SetVendorCommandParameters(4, 0, 128);


            executeVendorCmdResult = await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CocoaLoadVoaAttTab,
                Convert.ToInt32(timeoutValue.TotalMilliseconds));
            if (!executeVendorCmdResult) throw new Exception("Failed to execute Vendor Command CocoaLoadVoaAttTab");

            data = Device.Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 128);

            for (var i = 0; 3 * i < 128 - 2 && data[3 * i] != 0xFF; i++)
                _voa1Table.Add((data[3 * i + 1] << 8) | data[3 * i + 2]);

            VoaTablesAreLoaded = true;
        }


//        private async Task<byte[]> GetTemperatureOffsetBytes(TimeSpan timeout) => await GetTemperatureOffsetBytesAsync(Convert.ToInt32(timeout.TotalMilliseconds)).ConfigureAwait(false);

        private async Task<byte[]> GetTemperatureOffsetBytesAsync(int timeoutMs = 1000)
        {
            Device.SetVendorCommandParameters(0, 236, 3);
            if (!await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaReadCipPage, timeoutMs)
                .ConfigureAwait(false))
                throw new Exception("Failed to execute Vendor Command CiscoCocoaReadCipPage");
            var bufferContent = Device.Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 3);
            return bufferContent;
        }


        private static byte[] BiasWord(double biasMa) =>
            BitConverter.GetBytes((ushort) ((ushort) (Convert.ToUInt16(biasMa / 0.008) >> 8) |
                                            (ushort) (Convert.ToUInt16(biasMa / 0.008) << 8)));

        private static byte[] CreateBiasArray(double current)
        {
            // used to pack the Ibias look up table with ALL the same currents.
            var word = BiasWord(current);
            return new[] {word[0], word[1], word[0], word[1], word[0], word[1], word[0], word[1], word[0], word[1]};
        }

        #endregion PRIVATE METHODS


        //======================================== validated below here ============================================
        public async Task<double> Vpoly0Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.Vpoly0).ConfigureAwait(false);
        
        public double Vpoly0() => Task.Run(Vpoly0Async).GetAwaiter().GetResult();

        public async Task<double> Vpoly1Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.Vpoly1).ConfigureAwait(false);

        public double Vpoly1() => Task.Run(Vpoly1Async).GetAwaiter().GetResult();

        public async Task<double> Vpoly2Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.Vpoly2).ConfigureAwait(false);

        public double Vpoly2() => Task.Run(Vpoly2Async).GetAwaiter().GetResult();

        public async Task<double> Vpoly3Async() => await Device.GetValueAsync(CyclopsRegisters.Page33.Vpoly3).ConfigureAwait(false);

        public double Vpoly3() => Task.Run(Vpoly3Async).GetAwaiter().GetResult();


        public async Task<double> VOA0_IFFAsync() =>
            await Device.GetValueAsync(CyclopsRegisters.Page32.Voa0Iff).ConfigureAwait(false);

        public async Task<double> VOA1_IFFAsync() =>
            await Device.GetValueAsync(CyclopsRegisters.Page32.Voa1Iff).ConfigureAwait(false);

        public async Task<bool> Set_LaserBias_CurrentAsync(double currentMillAmp, TimeSpan? timeout = null)
        {
            var minimumCurrent = 0.0;
            Guard.ThrowIfLessThan(currentMillAmp, minimumCurrent, nameof(currentMillAmp),
                $"{currentMillAmp} is less than {minimumCurrent}");

            if (currentMillAmp <= 0) throw new ArgumentOutOfRangeException(nameof(currentMillAmp));
            timeout ??= TimeSpan.FromMilliseconds(100);
            var data = CreateBiasArray(currentMillAmp);

            Device.SetPage(VendorCommand.Page2);
            Device.Write(VendorCommand.Page2, VendorCommand.Buffer.Address, data);
            var dataReadBack = Device.Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 10);

            if (!data.SequenceEqual(dataReadBack))
                throw new DataMisalignedException(
                    $"Vendor command buffer Error {data} != {dataReadBack}");
            return await Device.ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaSetIbiasByCurrent,
                Convert.ToInt32(timeout.Value.TotalMilliseconds)).ConfigureAwait(false);
        }





        public bool Set_TxOpenLoop(CancellationToken ct = default)
        {
            Device.SetCiscoIpRegAsync(CyclopsRegisters.Page06.BiasCurrentGlobalControlRegister, 0x13, ct);
            Device.SetCiscoIpRegAsync(CyclopsRegisters.Page06.BiasCurrentControlRegister1, 0x00, ct);
            return true;
        }

        public bool Set_IFF1MeasureMode(CancellationToken ct = default)
        {
            Device.SetCiscoIpRegAsync(CyclopsRegisters.Page07.IffGroupAMultRegister1, 0x80, ct);
            Device.SetCiscoIpRegAsync(CyclopsRegisters.Page07.IffGroupAControlRegister1, 0x66, ct);
            Device.SetCiscoIpRegAsync(CyclopsRegisters.Page07.IffGroupAControlRegister5, 0x55, ct);
            return true;
        }
    }
}