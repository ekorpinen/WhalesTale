using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CiscoSub20;
using DutGpio;
using Polly;
using Polly.Wrap;
using Support;
using WhalesTale.Communication;
using WhalesTale.Cyclops;
using WhalesTale.MaCom;
using WhalesTale.QSFP;
using WhalesTale.QSFP100;

namespace WhalesTale.Module
{
    public class Module : IModule
    {
        public Module(IDeviceIO deviceIo, IQsfp100G qsfp100G, ICyclops cyclops, IMaCom macCom,  AsyncPolicyWrap policyWrap)
        {
            Device = deviceIo;
            DutGpio = Device.Gpio;
            Cyclops = cyclops;
         //   _policyWrap = policyWrap;
            Qsfp100G = qsfp100G;
            MaCom = macCom;
        }

        private IDeviceIO Device { get; }
        public IDutGpio DutGpio { get; }
        public ICyclops Cyclops { get; }
        public IQsfp100G Qsfp100G { get; }
        public IMaCom MaCom { get; }

       // private readonly AsyncPolicyWrap _policyWrap;

        public void Dispose() => Device.I2C.Dispose();

        /// <summary>
        ///     Calibrate module temperature by passing in current heat sink temperature.
        ///     Offsets are subtracted from the measured value to create the reported value.
        ///     Example:  heat sink is @ 25C.
        ///     You would call this method CalibrateModuleTemperature(25.0)
        ///     Note: both offsets (0C and 75C are set the same)
        /// </summary>
        /// <param name="caseHotspotTemperature"></param>
        /// <param name="slope"></param>
        /// <param name="zeroOffsetsFirst"></param>
        public async Task<bool> CalibrateModuleTemperature2(double caseHotspotTemperature, double slope,
            bool zeroOffsetsFirst = false)
        {
            return await Task.Run(async () =>
            {
                if (zeroOffsetsFirst)
                {
                    await Qsfp100G.SetModuleTemperatureOffsetsAsync(0, 0).ConfigureAwait(false);
                    await Task.Delay(3000).ConfigureAwait(false); // allow sometime overcome module internal smoothing
                    var result2 = await WaitDutTemperatureStable(0.5, TimeSpan.FromSeconds(30),
                        TimeSpan.FromSeconds(2), TimeSpan.FromMinutes(5));
                }

                var temperatureOffsetAtCaseHotspotTemp = await Qsfp100G.TemperatureAsync() - caseHotspotTemperature;

                var fixedOffset = temperatureOffsetAtCaseHotspotTemp - caseHotspotTemperature * slope;

                //                    fixed offset temp dependent offset
                var offsetAt0C = fixedOffset + 0.0 * slope;
                var offsetAt70C = fixedOffset + 70.0 * slope;


                var writtenData = await Qsfp100G.SetModuleTemperatureOffsetsAsync(offsetAt0C, offsetAt70C)
                    .ConfigureAwait(false);
                var checkSum = UtilityFunctions.ComputeCheckSum(Qsfp100G.GetCiscoSpecificConfiguration());
                await Device.SetRegAsync(Qsfp100GRegister.Page4.ModuleConfigCheckSum, checkSum);
                await Qsfp100G.Update_CalibrationAsync().ConfigureAwait(false);

                // validate page 4 upper NVR after device reset.
                var res = await DutGpio.Reset(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(2500));
                var (success, _) = await ResetWaitTillIntL(TimeSpan.FromMilliseconds(1000), TimeSpan.FromSeconds(10));
                if (!success) throw new ArgumentException("ResetWaitTillIntL failed");
                var readBackData = Qsfp100G.GetCiscoSpecificConfiguration();
                return readBackData.SequenceEqual(writtenData);
            });
        }

        /// <summary>
        ///     Calibrate module temperature by passing in current heat sink temperature.
        ///     Offsets are subtracted from the measured value to create the reported value.
        ///     Example:  heat sink is @ 25C.
        ///     You would call this method CalibrateModuleTemperature(25.0)
        ///     Note: both offsets (0C and 75C are set the same)
        /// </summary>
        /// <param name="heatSinkTemperature"></param>
        /// <param name="zeroOffsetsFirst"></param>
        public async Task<bool> CalibrateModuleTemperature(double heatSinkTemperature, bool zeroOffsetsFirst = false)
        {
            return await Task.Run(async () =>
            {
                if (zeroOffsetsFirst)
                {
                    await Qsfp100G.SetModuleTemperatureOffsetsAsync(0, 0).ConfigureAwait(false);
                    await Task.Delay(3000).ConfigureAwait(false); // allow sometime overcome module internal smoothing
                }

                var temperatureOffset = await Qsfp100G.TemperatureAsync() - heatSinkTemperature;

                var writtenData = await Qsfp100G.SetModuleTemperatureOffsetsAsync(temperatureOffset, temperatureOffset)
                    .ConfigureAwait(false);
                var checkSum = UtilityFunctions.ComputeCheckSum(Qsfp100G.GetCiscoSpecificConfiguration());
                await Device.SetRegAsync(Qsfp100GRegister.Page4.ModuleConfigCheckSum, checkSum);
                await Qsfp100G.Update_CalibrationAsync().ConfigureAwait(false);

                // validate page 4 upper NVR after device reset.
                var res = await DutGpio.Reset(TimeSpan.FromMilliseconds(50), TimeSpan.FromMilliseconds(2500));
                var (success, bootTime) =
                    await ResetWaitTillIntL(TimeSpan.FromMilliseconds(1000), TimeSpan.FromSeconds(10));

                var readBackData = Qsfp100G.GetCiscoSpecificConfiguration();
                return readBackData.SequenceEqual(writtenData);
            });
        }

        public async Task<(bool success, TimeSpan bootTime)> BootTime(Action<bool> enable, TimeSpan timeOut)
        {
            var policyResult = await Policy
                .Handle<Exception>()
                .RetryAsync(3, (exception, retryCount) =>
                {
                    Debug.WriteLine($"BootTime error: retry {retryCount} : {exception.Message})");
                    Task.Delay(2000).Wait();
                })
                .ExecuteAndCaptureAsync(async () =>
                {
                    enable(false);
                    await Task.Delay(250);
                    var stopwatch = Stopwatch.StartNew();
                    enable(true);
                    bool interruptStatus;
                    do
                    {
                        await Task.Delay(10);
                        if (stopwatch.Elapsed > timeOut)
                            throw new TimeoutException($"Module failed to boot in {timeOut}");
                        interruptStatus = DutGpio.InterruptAsserted;
                        // Debug.WriteLine($" BootTime : {stopwatch.Elapsed.TotalMilliseconds} mS, interruptStatus : {interruptStatus}");
                    } while (interruptStatus & (stopwatch.Elapsed < timeOut));

                    stopwatch.Stop();

                    return stopwatch.Elapsed > timeOut
                        ? (false, stopwatch.Elapsed)
                        : (!interruptStatus, stopwatch.Elapsed);
                });

            return policyResult.Result;
        }

        public async Task<(bool success, TimeSpan bootTime, int error, int exError)> ResetWaitTillReadyWithErrorCheck(
            TimeSpan assertTime, TimeSpan timeOut, IProgress<TmgProgressReporting> progress = null)
        {
            progress?.Report(new TmgProgressReporting(DateTime.Now, $"Module Reset : Assert {assertTime}"));

            var bootTimeTask = Task.Run(() => BootTime(Reset, timeOut));
            await Task.WhenAll(bootTimeTask).ConfigureAwait(false);

            var dataReadyTask = Task.Run(() => WaitForDataReady(timeOut));
            await Task.WhenAll(dataReadyTask).ConfigureAwait(false);

            var errorCode = await Qsfp100G.ErrorCodeAsync().ConfigureAwait(false);
            var errorCodeOptional = await Qsfp100G.ErrorOptionalDataAsync().ConfigureAwait(false);

            var success2 = bootTimeTask.Result.success & (errorCode == 0);

            var progressMessage =
                $"Completed Reset : status {bootTimeTask.Result.success}, DataReady {dataReadyTask.Result.success}, " +
                $"Error Code {errorCode}, " +
                $"ErrorCodeOptional Code {errorCodeOptional}, " +
                $"in boot time: {bootTimeTask.Result.bootTime.TotalMilliseconds} mS, " +
                $"in data ready time: {dataReadyTask.Result.dataReadyTime.TotalMilliseconds} mS ";

            Debug.WriteLine(progressMessage);

            progress?.Report(new TmgProgressReporting(DateTime.Now, progressMessage));

            return (success2, bootTimeTask.Result.bootTime, errorCode, errorCodeOptional);
        }

        public async Task<(bool success, TimeSpan bootTime)> ResetWaitTillIntL(TimeSpan assertTime, TimeSpan timeOut,
            IProgress<TmgProgressReporting> progress = null)
        {
            progress?.Report(new TmgProgressReporting(DateTime.Now, $"Module Reset : Assert {assertTime}"));

            var results = await BootTime(Reset, timeOut);

            progress?.Report(new TmgProgressReporting(DateTime.Now,
                $"Completed Reset : status {results.success} in {results.bootTime.TotalMilliseconds} mS"));
            return results;
        }

        public async Task<(bool success, TimeSpan bootTime)> ResetWaitTillReady(TimeSpan assertTime, TimeSpan timeOut,
            IProgress<TmgProgressReporting> progress = null)
        {
            progress?.Report(new TmgProgressReporting(DateTime.Now, $"Module Reset : Assert {assertTime}"));

            var bootTimeResults = await BootTime(Reset, timeOut);
            var (success, dataReadyTime) = await WaitForDataReady(timeOut);

            progress?.Report(new TmgProgressReporting(DateTime.Now,
                $"Completed Reset : status {bootTimeResults.success & success} in boot time: " +
                $"{bootTimeResults.bootTime.TotalMilliseconds} mS ; in data ready time: {dataReadyTime.TotalMilliseconds} mS "));
            return bootTimeResults;
        }

        public async Task<bool> ResetAndDelay(TimeSpan assertTime, TimeSpan delayTime, TimeSpan timeOut,
            IProgress<TmgProgressReporting> progress = null)
        {
            progress?.Report(new TmgProgressReporting(DateTime.Now, $"Module Reset : Assert {assertTime}"));

            Reset(false);
            await Task.Delay(assertTime);
            Reset(true);
            await Task.Delay(delayTime);

            return true;

            void Reset(bool s)
            {
                DutGpio.Reset(!s);
            }
        }

        public async Task<bool> WaitDutTemperatureStable(double allowableChange, TimeSpan window, TimeSpan updateRate,
            TimeSpan timeout)
        {
            var numberOfPoint = window.Seconds / updateRate.Seconds;
            var data = new List<double>();
            var timerStopwatch = Stopwatch.StartNew();
            do
            {
                data.Add(await Qsfp100G.TemperatureAsync());
                if (data.Count() > numberOfPoint)
                {
                    data.RemoveAt(0);
                    Debug.WriteLine($"Max {data.Max()}, Min {data.Min()}, Delta {data.Max() - data.Min()}");
                    if (data.Max() - data.Min() <= allowableChange) return true;
                }

                await Task.Delay(updateRate);
            } while (timerStopwatch.Elapsed < timeout);

            return false;
        }

        public static Module ModuleFactory(string s20SerialNumber, AsyncPolicyWrap policyWrap)
        {
            var sub20Interfaces = new Sub20Interfaces();
            if (!string.IsNullOrEmpty(s20SerialNumber))
                return ModuleFactory(new CSub20(s20SerialNumber, sub20Interfaces), policyWrap);
            throw new Exception($"Null or Empty Sub20 requested : {s20SerialNumber}");
        }

        public static Module ModuleFactory(ISub20 sub20, AsyncPolicyWrap policyWrap)
        {
            var i2C = new Sub20I2C(sub20);

            var gpioConfiguration = new GpioConfiguration(0x00380000, 0x00380000);
            IGpio gpio = new Sub20Gpio(sub20, gpioConfiguration);
            gpio.GpioInitialize();
            var dutGpio = new DutGpio.DutGpio(null, gpio, new DutGpioBits(16, 17, 18));

            var deviceIO = new DeviceIO(i2C, dutGpio, policyWrap);
            var cyclops = new Cyclops.Cyclops(deviceIO);
            var qsfp100GFRS = new Qsfp100G(deviceIO);
            var macom = new MaCom.MaCom(deviceIO);
            var module = new Module(deviceIO,  qsfp100GFRS, cyclops, macom, policyWrap);
            return module;
        }

        ~Module()
        {
            Device.I2C.Dispose();
        }


        public void Reset(bool s)
        {
            DutGpio.Reset(!s);
        }


        public Task<(bool success, TimeSpan dataReadyTime)> WaitForDataReady(TimeSpan timeOut)
        {
            Task<(bool interruptStatus, TimeSpan sbttopwatch)> policyResult = Policy
                .Handle<Exception>()
                .Retry(3, (exception, retryCount) =>
                {
                    Debug.WriteLine($"BootTime error: retry {retryCount} : {exception.Message})");
                    Task.Delay(2000).Wait();
                })
                .Execute(async () =>
                {
                    var stopwatch = Stopwatch.StartNew();
                    bool dataNotReadyStatus;
                    do
                    {
                        dataNotReadyStatus = await Qsfp100G.DataNotReadyAsync();
                        await Task.Delay(10);
                        Debug.WriteLine(
                            $" DataReadyTime : {stopwatch.Elapsed.TotalMilliseconds} mS, dataNotReadyStatus: {dataNotReadyStatus}");
                        if (stopwatch.Elapsed > timeOut)
                            throw new TimeoutException(
                                $"Module failed to get to Valid DataNotReady state  in {timeOut}");
                    } while (dataNotReadyStatus && stopwatch.Elapsed < timeOut);

                    stopwatch.Stop();

                    return stopwatch.Elapsed > timeOut
                        ? (false, stopwatch.Elapsed)
                        : (!dataNotReadyStatus, stopwatch.Elapsed);
                });
            return policyResult;
        }
    }
}