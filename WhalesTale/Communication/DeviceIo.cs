using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CiscoSub20;
using DutGpio;
using Polly;
using Polly.Retry;
using Polly.Timeout;
using Polly.Wrap;
using Support;
using WhalesTale.Cyclops;
using WhalesTale.QSFP;
using WhalesTale.QSFP100;
using static CiscoSub20.Sub20I2C;
using DataType = WhalesTale.RegisterBase.DataType;

namespace WhalesTale.Communication
{
    public class DeviceIO : IDeviceIO
    {
        private const int DefaultTimeOutMS = 3000;
        private const int DefaultRetryDelayMS = 3000;

        private const int NumberOfRetries = 3;
        private const int DefaultStepMS = 100;
        private readonly byte[] _hostMfrPassword = {0x00, 0x00, 0x00, 0x00};
        private readonly byte[] _moduleMfrPassword = {0x20, 0x17, 0x05, 0x17};
        private object ReadWriteLock { get; } = new object();
        private object RegAccessLock { get; } = new object();

        private readonly AsyncPolicyWrap _policyWrap;


        public DeviceIO(II2C i2C, IDutGpio gpio, AsyncPolicyWrap policyWrap)
        {
            I2C = i2C;
            Gpio = gpio;
            _policyWrap = policyWrap;
        }

        public II2C I2C { get; }
        public IDutGpio Gpio { get; }

        public void SetPage(byte page)
        {
            I2C.I2C_SetPage(Sff8636.I2CAddress, Sff8636.PageRegAddr, page);
        }

        public VendorCommand.Status VendorCommandStatus =>
            VendorCommand.Status.Get(ReadByte(VendorCommand.Page1, VendorCommand.StatusAddress));
 
        #region READ WRITE

 

        public void WriteByte(byte page, byte writeAddress, byte data)
        {
            Write(page, writeAddress, new[] {data});
        }
        public void WriteWord(byte page, byte writeAddress, ushort data)
        {
            Write(page, writeAddress,
                BitConverter.GetBytes(UtilityFunctions.SwapBytes(data)));
        }
        public void Write(byte page, byte writeAddress, byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            var dataLength = data.Length;
            var bytesWritten = 0;
            lock (ReadWriteLock)
            {
                SetPage((byte) page);
                while (dataLength != 0)
                {
                    var writeLength = dataLength > VendorCommand.MaxWriteLen
                        ? VendorCommand.MaxWriteLen
                        : (byte) dataLength;
                    var writeData = data.Skip(bytesWritten).Take(writeLength).ToArray();
                    I2C.I2C_RandomWrite(Qsfp100GRegister.I2CAddress, writeAddress, 1, writeData);
                    dataLength -= writeLength;
                    bytesWritten += writeLength;
                    writeAddress += writeLength;
                }
            }
        }


        public byte ReadByte(byte page, byte readAddress)
        {
            lock (ReadWriteLock)
            {
                SetPage((byte)page);
                return I2C.I2C_RandomRead(Qsfp100GRegister.I2CAddress, readAddress);
            }
        }
        public async Task<byte> ReadByteAsync(byte page, byte readAddress)
        {
            var readResult = await ReadAsync(page, readAddress, 1).ConfigureAwait(false);
            return readResult[0];
        }

        public ushort ReadWord(byte page, byte readAddress)
        {
            var readData = new byte[2];
            lock (ReadWriteLock)
            {
                SetPage((byte)page);
                I2C.I2C_RandomRead(Qsfp100GRegister.I2CAddress, readAddress, 2, ref readData);
            }
            return BitConverter.ToUInt16(new byte[2] { readData[1], readData[0] }, 0);
        }
        public async Task<ushort> ReadWordAsync(byte page, byte readAddress)
        {
            var readResult = await ReadAsync(page, readAddress, 2).ConfigureAwait(false);
            return BitConverter.ToUInt16(new byte[2] { readResult[1], readResult[0] }, 0);
        }

        public byte[] Read(byte page, byte readAddress, byte numBytes)
        {
            var readData = new byte[numBytes];
            lock (ReadWriteLock)
            {
                SetPage((byte)page);
                I2C.I2C_RandomRead(Qsfp100GRegister.I2CAddress, readAddress, numBytes, ref readData);
            }
            return readData;
        }
        public async Task<byte[]> ReadAsync(byte page, byte readAddress, byte numBytes)
        {
            SetPage((byte)page);
            var readData = await I2C.I2C_RandomReadAsync(Qsfp100GRegister.I2CAddress, readAddress, numBytes)
                .ConfigureAwait(false);
            return readData;
        }






        //=========================== QSFP

        /// <summary>
        ///  Reads an SFF8636 register.
        ///
        /// Retries and Timeout set by policy.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// 
        public async Task<dynamic> GetRegAsync(Sff8636 reg, CancellationToken cancellationToken = default)
        {
            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetPage(reg.Register.Page);
                var readData = await I2C.I2C_RandomReadAsync(Sff8636.I2CAddress, reg.Register.Address, reg.Register.Size).ConfigureAwait(false);
                var res = ConvertData(reg, readData);
                return res;
            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Result;
        }

        /// <summary>
        /// Writes and verifies setting of a SFF8636 register.
        /// 
        /// Retries and Timeout set by policy.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetRegAsync(Sff8636 reg, byte data, CancellationToken cancellationToken = default)
        {
            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetPage(reg.Register.Page);
                I2C.I2C_RandomWrite(Sff8636.I2CAddress, reg.Register.Address, data);

                var readBack = await I2C.I2C_RandomReadAsync(Sff8636.I2CAddress, reg.Register.Address).ConfigureAwait(false);
                if (data != readBack) throw new ValidationException($"Validation failed, {data} not equal {readBack}.");

            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address}:{data})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Outcome == OutcomeType.Successful;
        }

        /// <summary>
        /// Reads and scales a SFF8636 register.
        /// Must be of type Dec or DecWord.
        /// 
        /// Exception thrown for all other types.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<double> GetValueAsync(Sff8636 reg, CancellationToken cancellationToken = default)
        {
            var result = await GetRegAsync(reg, cancellationToken).ConfigureAwait(false);
            if (reg.Register.Type.Equals(DataType.Dec) | reg.Register.Type.Equals(DataType.DecWord))
                return result * reg.Register.Scale;
            throw new ArgumentException($"Register {reg.Name} with data type : {reg.Type} does not support scaling.");
        }


        //=========================== QSFP100G

        /// <summary>
        ///  Reads a Qsfp100GRegister register.
        /// 
        /// Retries and Timeout set by policy.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<dynamic> GetRegAsync(Qsfp100GRegister reg, CancellationToken cancellationToken = default)
        {
            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetPage(reg.Register.Page);
                var readData = await I2C.I2C_RandomReadAsync(Qsfp100GRegister.I2CAddress, reg.Register.Address, reg.Register.Size).ConfigureAwait(false);
                var res = ConvertData(reg, readData);
                return res;
            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Result;
        }

        /// <summary>
        /// Writes and verifies setting a Qsfp100GRegister register which is 1 byte.
        /// 
        /// Retries and Timeout set by policy.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetRegAsync(Qsfp100GRegister reg, byte data, CancellationToken cancellationToken = default)
        {
            if (reg.Register.Size != 1)
                throw new ArgumentOutOfRangeException(reg.Name,
                    $"requires {reg.Register.Size} bytes while data is 1 byte.");

            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetPage(reg.Register.Page);
                I2C.I2C_RandomWrite(Qsfp100GRegister.I2CAddress, reg.Register.Address, data);

                var readBack = await I2C.I2C_RandomReadAsync(Qsfp100GRegister.I2CAddress, reg.Register.Address).ConfigureAwait(false);
                if (data!=readBack) throw new ValidationException($"Validation failed, {data} not equal {readBack}.");

            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address}:{data})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Outcome == OutcomeType.Successful;
        }

        /// <summary>
        /// Writes and verifies setting a Qsfp100GRegister register which > 1 byte.
        /// data is a byte array.
        /// 
        /// Retries and Timeout set by policy.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="data"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<bool> SetRegAsync(Qsfp100GRegister reg, byte[] data, CancellationToken cancellationToken = default)
        {
            if (reg.Register.Size != data.Length)
                throw new ArgumentOutOfRangeException(reg.Name,
                    $"requires {reg.Register.Size} bytes while data only has {data.Length} bytes.");

            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetPage(reg.Register.Page);
                I2C.I2C_RandomWrite(Qsfp100GRegister.I2CAddress, reg.Register.Address, reg.Register.Size, data);

                var readBack = await I2C.I2C_RandomReadAsync(Qsfp100GRegister.I2CAddress, reg.Register.Address).ConfigureAwait(false);
                if (!StructuralComparisons.StructuralEqualityComparer.Equals(data, readBack)) throw new ValidationException($"Validation failed, {data} not equal {readBack}.");

            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address}:{data})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Outcome == OutcomeType.Successful;
        }

        /// <summary>
        /// Reads and scales a SFF8636 register.
        /// Must be of type Dec or DecWord.
        /// 
        /// Exception thrown for all other types.
        /// </summary>
        /// <param name="reg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<double> GetValueAsync(Qsfp100GRegister reg, CancellationToken cancellationToken = default)
        {
            var result = await GetRegAsync(reg, cancellationToken).ConfigureAwait(false);
            if (reg.Register.Type.Equals(DataType.Dec) | reg.Register.Type.Equals(DataType.DecWord))
                return result * reg.Register.Scale;
            throw new ArgumentException($"Register {reg.Name} with data type : {reg.Type} does not support scaling.");

        }



        //=========================== CYCLOPS

        public async Task<int> GetRegAsync(CyclopsRegisters reg, CancellationToken cancellationToken = default)
        {
            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetVendorCommandParameters(reg.Register);
                var executeVendorCmdResult = await ExecuteVendorCmdAsync(
                    VendorCommand.Command.CiscoCocoaReadCipPage, DefaultTimeOutMS, DefaultStepMS).ConfigureAwait(false);
                if (!executeVendorCmdResult)
                    throw new Exception("Failed to execute Vendor Command CiscoCocoaReadCipPage");
                var data = await ReadAsync(VendorCommand.Page2, VendorCommand.Buffer.Address, reg.Register.Size)
                    .ConfigureAwait(false);
                var res = ConvertData(reg, data);
                return (int)res;
            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Result;
        }

        public async Task<bool> SetCiscoIpRegAsync(CyclopsRegisters reg, byte data, CancellationToken cancellationToken = default)
        {
//            return await SetCiscoIpRegAsync(reg, new[] { data }, cancellationToken);

            if (reg.Register.Size != 1)
                throw new ArgumentOutOfRangeException(reg.Name,
                    $"requires {reg.Register.Size} bytes while data is 1 byte.");

            var policyWrapResults = await _policyWrap.ExecuteAndCaptureAsync(async (context, ct) =>
            {
                ct.ThrowIfCancellationRequested();

                SetVendorCommandParameters(reg.Register.Page, reg.Register.Address, reg.Register.Size);
                WriteByte(VendorCommand.Page2, VendorCommand.Buffer.Address, data);
                var success = await ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaWriteCipPage, 1000, 1000);
                if (!success) throw new Exception("Failed to execute Vendor Command CiscoCocoaWriteCipPage");
                return true;
            }, new Dictionary<string, object>() { { $"{GetCaller()}", $"{reg.Name}({reg.Page}.{reg.Address}:{data})" } }, cancellationToken: cancellationToken);
            if (policyWrapResults.Outcome == OutcomeType.Failure) throw policyWrapResults.FinalException;

            return policyWrapResults.Outcome == OutcomeType.Successful;

        }

        //public async Task<bool> SetCiscoIpRegAsync(CyclopsRegisters reg, byte[] data, CancellationToken cancellationToken = default)
        //{
        //    var policyResult = await Policy.Handle<Exception>().RetryAsync(3, onRetry: async (exception, retryCount) =>
        //    {
        //        Debug.WriteLine($"ReadReg error ({reg.Name} : retry {retryCount} : {exception.Message})");
        //        await Task.Delay(2000);
        //    }).ExecuteAndCaptureAsync(async () =>
        //    {
        //        SetVendorCommandParameters(reg.Register.Page, reg.Register.Address, reg.Register.Size);
        //        WriteByte(VendorCommand.Page2, VendorCommand.Buffer.Address, data[0]);
        //        var success = await ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaWriteCipPage, 1000, 1000);
        //        if (!success) throw new Exception("Failed to execute Vendor Command CiscoCocoaWriteCipPage");
        //        return true;
        //    });
        //    return policyResult.Result;
        //}

        public async Task<double> GetValueAsync(CyclopsRegisters reg, CancellationToken cancellationToken = default)
        {
            var result = await GetRegAsync(reg, cancellationToken).ConfigureAwait(false);
            if (reg.Register.Type.Equals(DataType.Dec) | reg.Register.Type.Equals(DataType.DecWord))
                return result * reg.Register.Scale;
            throw new ArgumentException($"Register {reg.Name} with data type : {reg.Type} does not support scaling.");


            //if ((Math.Abs(reg.Register.Scale) < 1.0E-10) | reg.Register.Type.Equals(DataType.HexWord) |
            //    reg.Register.Type.Equals(DataType.Int32) | reg.Register.Type.Equals(DataType.Binary))
            //{
            //    //                var r = await Task.Run(() => readReg(reg, timeOut));
            //    var r = await GetRegAsync(reg, cancellationToken);
            //    return r;
            //}

            ////              var s = await Task.Run(() => readReg(reg, timeOut));
            //var s = await GetRegAsync(reg, cancellationToken);
            //var ss = s * reg.Register.Scale;
            //return ss;
        }


        public async Task<dynamic> GetMaComRegAsync(uint address, ushort len, CancellationToken ct)
        {
            //timeOut ??= TimeSpan.FromSeconds(3);
            var policyResult = await Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, retryCount) =>
                {
                    Debug.WriteLine(
                        $"ReadCiscoCoCoaReg error address ({address} : retry {retryCount} : {exception.Message})");
                    Task.Delay(2000).Wait();
                }).ExecuteAndCaptureAsync(async () =>
                {
                    WriteWord(VendorCommand.Page1, 144, (ushort)(address >> 16));
                    WriteWord(VendorCommand.Page1, 146, (ushort)(address & 0x0000FFFF));
                    WriteWord(VendorCommand.Page1, 132, len);
                    if (!await ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaReadRegister, 2000, 1000)
                        .ConfigureAwait(false))
                        throw new Exception("Failed to execute Vendor Command CiscoCocoaReadRegister");
                    var data = Read(VendorCommand.Page2, VendorCommand.Buffer.Address, 4);
                    return ConvertData(data);
                });
            if (policyResult.Outcome == OutcomeType.Successful) return policyResult.Result;
            else throw new Exception($"failed ReadCiscoCoCoaReg : {address} {policyResult.FinalException.Message}");
        }

        public async Task<bool> SetMaComRegAsync(uint address, ushort len, uint[] data, CancellationToken ct)
        {
            WriteWord(VendorCommand.Page1, 144, (ushort)(address >> 16));
            WriteWord(VendorCommand.Page1, 146, (ushort)(address & 0x0000FFFF));
            WriteWord(VendorCommand.Page1, 132, len);
            for (ushort i = 0; i < len; i++)
            {
                WriteWord(VendorCommand.Page2, (byte)(VendorCommand.Buffer.Address + 4 * i), (ushort)(data[i] >> 16));
                WriteWord(VendorCommand.Page2, (byte)(VendorCommand.Buffer.Address + 4 * i + 2),
                    (ushort)(data[i] & 0x0000FFFF));
            }

            var res = await ExecuteVendorCmdAsync(VendorCommand.Command.CiscoCocoaWriteRegister, 2000, 1000)
                .ConfigureAwait(false);
            if (res) return res;
            throw new Exception("Failed to execute Vendor Command CiscoCocoaReadCipPage");
        }




        #endregion READ WRITE


        #region Vendor Command Methods

        public async Task<bool> ExecuteVendorCmdAsync(VendorCommand.Command command) =>
            await ExecuteVendorCmdAsync(command, DefaultTimeOutMS, DefaultStepMS);

        public async Task<bool> ExecuteVendorCmdAsync(VendorCommand.Command command, TimeSpan timeout, TimeSpan step) =>
            await ExecuteVendorCmdAsync(command, (int) timeout.TotalMilliseconds, (int) step.TotalMilliseconds);

        public async Task<bool> ExecuteVendorCmdAsync(VendorCommand.Command command, int timeoutMs, int stepMS)
        {
            var policyResult = await Policy.Handle<Exception>().RetryAsync(3, onRetry: async (exception, retryCount) =>
            {
                Debug.WriteLine($"ExecuteVendorCmdAsync error (retry {retryCount} : {exception.Message})");
                await Task.Delay(2000);
            }).ExecuteAndCaptureAsync(async () =>
            {
                WriteModuleMfrPassword();
                if (VendorCommandStatus != VendorCommand.Status.Idle)
                    throw new Exception($"Cannot execute command 0x{command.Value:0X}, vendor status is not idle");
                WriteByte(VendorCommand.Page1, VendorCommand.Address, command.Value); // write the command
                var status = VendorCommandStatus; // read initial status
                Debug.WriteLine(status.Name);
                while (status == VendorCommand.Status.Idle || status == VendorCommand.Status.InProgress)
                {
                    if (timeoutMs <= 0)
                        throw new Exception($"Timeout waiting for vendor command to complete: 0x{command.Value:0X}.");
                    timeoutMs -= stepMS;
                    await Task.Delay(stepMS);
                    status = VendorCommandStatus;
                    Debug.WriteLine($"status {status.Value}:  {status.Name}");
                }

                WriteByte(VendorCommand.Page1, VendorCommand.StatusAddress,
                    VendorCommand.Status.Idle.Value); // set status to idle
                if (status != VendorCommand.Status.Complete)
                    throw new Exception(
                        $"Vendor command : 0x{command.Value:0X} failed with status 0x{status.Value:0X}.");
                ClearModuleMfrPassword();
                return true;
            });
            return policyResult.Result;
        }

        public void SetVendorCommandParameters(RegisterBase.RegisterBase reg)
        {
            SetVendorCommandParameters(reg.Page, reg.Address, reg.Size);
        }

        public void SetVendorCommandParameters(byte page, byte address, ushort length)
        {
            WriteByte(VendorCommand.Page1, VendorCommand.Params.PageAddress, page);
            WriteByte(VendorCommand.Page1, VendorCommand.Params.AddressAddress, address);
            WriteWord(VendorCommand.Page1, VendorCommand.Params.LengthAddress, length);
        }

        public void WriteModuleMfrPassword()
        {
            I2C.I2C_RandomWrite(0, Sff8636.Password.EntryAddress,
                Sff8636.Password.Size, _moduleMfrPassword);
        }

        public void ClearModuleMfrPassword()
        {
            I2C.I2C_RandomWrite(0, Sff8636.Password.EntryAddress, Sff8636.Password.Size, _hostMfrPassword);
        }

        #endregion Vendor Command Methods

        #region Private Methods

        private static void ManageRetryException(Exception exception, TimeSpan timeSpan, int retryCount, Context context)
        {
            var action = context != null ? context.First().Key : "unknown method";
            var actionDescription = context != null ? context.First().Value : "unknown description";
            var msg = $"Retry n°{retryCount} of {action} ({actionDescription}) : {exception.Message}";
            Console.WriteLine(msg);
        }

        private static Task ManageTimeoutException(Context context, TimeSpan timeSpan, Task task)
        {
            var action = context != null ? context.First().Key : "unknown method";
            var actionDescription = context != null ? context.First().Value : "unknown description";

            task.ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    var msg = $"Running {action} ({actionDescription}) but the execution timed out after {timeSpan.TotalSeconds} seconds, eventually terminated with: {t.Exception}.";
                    Console.WriteLine(msg);
                }
                else if (t.IsCanceled)
                {
                    var msg = $"Running {action} ({actionDescription}) but the execution timed out after {timeSpan.TotalSeconds} seconds, task cancelled.";
                    Console.WriteLine(msg);
                }
            });
            return task;
        }
        private static dynamic ConvertData(byte[] data) => BitConverter.ToUInt32(data.Reverse().ToArray(), 0);

        //private static string ConvertData(Sff8636 reg, byte[] data)
        //{
        //    return reg.Register.Type != DataType.String ? string.Empty : Encoding.UTF8.GetString(data).Trim();
        //}
        private static dynamic ConvertData(byte data) => BitConverter.ToUInt32(new []{data}.Reverse().ToArray(), 0);

       private static dynamic ConvertData(dynamic reg, byte[] data)
        {
            if (reg.Register.Type == DataType.String) return Encoding.UTF8.GetString(data).Trim();
            if ((reg.Register.Type == DataType.Hex) | (reg.Register.Type == DataType.Binary)) return data[0];
            if (reg.Register.Type == DataType.Dec || reg.Register.Type == DataType.DecWord ||
                reg.Register.Type == DataType.HexWord)
            {
                if (reg.Register.Signed) return BitConverter.ToInt16(data.Reverse().ToArray(), 0);
                return BitConverter.ToUInt16(data.Reverse().ToArray(), 0);
            } // end Type.Dec

            if (reg.Register.Type != DataType.Int32) return null;
            if (reg.Register.Signed) return BitConverter.ToInt32(data.Reverse().ToArray(), 0);
            return BitConverter.ToUInt32(data.Reverse().ToArray(), 0);
        }
        
        public static string GetCaller([CallerMemberName] string caller = null) => caller;

        #endregion Private Methods
    }

}