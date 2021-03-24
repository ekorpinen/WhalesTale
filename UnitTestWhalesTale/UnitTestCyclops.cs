using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CiscoSub20;
using DutGpio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhalesTale.Communication;
using WhalesTale.Cyclops;
using WhalesTale.Module;
using WhalesTale.QSFP;
using WhalesTale.QSFP100;
using  WhalesTale;
using WhalesTale.MaCom;

namespace UnitTestWhalesTale
{
    [TestClass]
    public class UnitTestCyclops
    {

        //readonly string sub20_SerialNumber = "0x4194";
        //
        //        private const string Sub20SerialNumber = "0x2A0A"; // DUT
        private const string Sub20SerialNumber = "0x5858";   // Front DUT @Home
        //readonly string sub20_SerialNumber = "0x3538"; // AWN 
        //readonly string sub20_SerialNumber = "0x340C"; // Golden Unit


        private CSub20 _s20;
        private Module _module;

        [TestInitialize]
        public void TestInit()
        {
            var policies = new Policies();

            var sub20Interfaces = new Sub20Interfaces();
            _s20 = new CSub20(Sub20SerialNumber, sub20Interfaces);

            II2C i2C = new Sub20I2C(_s20);
            var gpioConfiguration = new GpioConfiguration(0x00380000, 0x00380000);
            var gpio = new Sub20Gpio(_s20, gpioConfiguration);
            gpio.GpioInitialize();
            var dutGpio = new DutGpio.DutGpio(null, gpio, new DutGpioBits(16, 17, 18));

            var modPres = dutGpio.ModPresentAsserted;
            Assert.IsTrue(modPres);

            var deviceIO = new DeviceIO(i2C, dutGpio, policies.PolicyWrap);
            var cyclops = new Cyclops(deviceIO);
            var qsfp100GFRS = new Qsfp100G(deviceIO);
            var maCom = new MaCom(deviceIO);
            _module = new Module(deviceIO,  qsfp100GFRS, cyclops, maCom, policies.PolicyWrap);

        }

        [TestCleanup]
        public void TestCleanup() => _s20.Dispose();

        [TestMethod]
        public async Task TestSet_LaserBias_Current()
        {
            const double testCurrent = 75;

            await _module.Cyclops.Set_LaserBias_CurrentAsync(testCurrent);
            var txBias = _module.Qsfp100G.TxBias() * 3.0;
            Assert.AreEqual(testCurrent, txBias, 1);
        }

        

        [TestMethod]
        public void Test_GetIFF2()
        {
            var iff2 = _module.Cyclops.Iff2();
            Assert.IsNotNull(iff2);
        }

        [TestMethod]
        public void Test_GetIFF1()
        {
            var iff1 = _module.Cyclops.Iff1();
            Assert.IsNotNull(iff1);
        }




        [TestMethod]
        public async Task MaCom()
        {
            await using var writer = File.CreateText(Support.Functions.ChangeFileName($@"C:\TestResults\macom.csv"));

            var fwDebug = await _module.MaCom.FwDebugAsync();
            await writer.WriteLineAsync($"{fwDebug}, 0x{fwDebug:X8}").ConfigureAwait(false);

            var tiaCoarse = await _module.MaCom.TiaCoarseAsync();
            await writer.WriteLineAsync($"{tiaCoarse}, 0x{tiaCoarse:X8}").ConfigureAwait(false);

            var tiaFine = await _module.MaCom.TiaFineAsync();
            await writer.WriteLineAsync($"{tiaFine}, 0x{tiaFine:X8}").ConfigureAwait(false);

            var pvttEfuse = await _module.MaCom.PvtEfuse();
            await writer.WriteLineAsync($"{pvttEfuse}, 0x{pvttEfuse:X8}").ConfigureAwait(false);
            
            var pvttApplied = await _module.MaCom.PvttApplied();
            await writer.WriteLineAsync($"{pvttApplied}, 0x{pvttApplied:X8}").ConfigureAwait(false);

            var skew = await _module.MaCom.Skews();
            await writer.WriteLineAsync($"{skew.Skew0}, 0x{skew.Skew0:X8}");
            await writer.WriteLineAsync($"{skew.Skew1}, 0x{skew.Skew1:X8}");
            await writer.WriteLineAsync($"{skew.Skew2}, 0x{skew.Skew2:X8}");
            await writer.WriteLineAsync($"{skew.Skew3}, 0x{skew.Skew3:X8}");

            for (var i = 0; i < 5; i++)
            {
                var adcBackoff = await _module.MaCom.AdcBackoff();
                await writer.WriteLineAsync($"{adcBackoff:F6}").ConfigureAwait(false);
            }

            foreach (MaCom.Taps tap in (MaCom.Taps[]) Enum.GetValues(typeof(MaCom.Taps)))
            {
                var tapValue = await _module.MaCom.Tap(tap);
                await writer.WriteLineAsync($"Tap{Enum.GetName(typeof(MaCom.Taps), tap)} {tapValue}, {tapValue:F9}")
                    .ConfigureAwait(false);
            }

            var level = await _module.MaCom.Levels();
            await writer.WriteLineAsync($"{level.Level0:F9}");
            await writer.WriteLineAsync($"{level.Level1:F9}");
            await writer.WriteLineAsync($"{level.Level2:F9}");
            await writer.WriteLineAsync($"{level.Level3:F9}");


            var decision = await _module.MaCom.Decisions();
            await writer.WriteLineAsync($"{decision.Decision01:F3}");
            await writer.WriteLineAsync($"{decision.Decision12:F3}");
            await writer.WriteLineAsync($"{decision.Decision23:F3}");

            var ppmOffset = await _module.MaCom.PpmOffset();
            await writer.WriteLineAsync($"{ppmOffset:F9}");


            var fwDebug2 = await _module.MaCom.FwDebug2Async();
            await writer.WriteLineAsync($"{fwDebug2}, 0x{fwDebug2:X8}");
        }


        [TestMethod]
        public async Task Test_DumpCyclopsVoltages()
        {

            var resultDutReset = _module.ResetWaitTillIntL(assertTime: TimeSpan.FromMilliseconds(1000), timeOut: TimeSpan.FromSeconds(10), progress: null);
            Task.WaitAll(resultDutReset);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(l0: true, l1: true, l2: true, l3: true);
            await _module.Qsfp100G.RxSquelchDisableSetAsync(l0: true, l1: true, l2: true, l3: true);
            await _module.Qsfp100G.Fec.CountersEnable(true);

            await _module.Qsfp100G.Host.PatternSelect(Qsfp100GRegister.Page7.PatternMaskReg129.Prbs31);
            await _module.Qsfp100G.Host.ErrorCheckerEnable(EnableLane.All);
            await _module.Qsfp100G.Host.PatternGeneratorEnable(EnableLane.All);


            //var path = @"c:\TestResults\CyclopsVoltageDump.csv";

            await using var writer = File.CreateText(Support.Functions.ChangeFileName($@"C:\TestResults\CyclopsVoltageDump.csv"));
            await writer.WriteLineAsync("Time (mS), Cyclops.V0P8, Cyclops.V1P0, Cyclops.V1P8, Cyclops.V2P7");
            var timer = Stopwatch.StartNew();
            do
            {
                var v0P8 = _module.Cyclops.V0P8();
                var v1P0 = _module.Cyclops.V1P0();
                var v1P8 = _module.Cyclops.V1P8();
                var v2P7 = _module.Cyclops.V2P7();
                await writer.WriteLineAsync($"{timer.ElapsedMilliseconds},{v0P8},{v1P0},{v1P8},{v2P7}");
            } while (timer.Elapsed < TimeSpan.FromSeconds(30));
        }

        [TestMethod]
        public void Test_GetV0P8()
        {
            var v0P8 = _module.Cyclops.V0P8();
            Assert.IsNotNull(v0P8);
            Assert.AreEqual(0.8, v0P8, 0.8 * .05);// within 5%
        }

        [TestMethod]
        public void Test_GetV1P0()
        {
            var v1P0 = _module.Cyclops.V1P0();
            Assert.IsNotNull(v1P0);
            Assert.AreEqual(1.0, v1P0, 1.0 * .05);// within 5%
        }

        [TestMethod]
        public void Test_GetV1P8()
        {
            var v1P8 = _module.Cyclops.V1P8();
            Assert.IsNotNull(v1P8);
            Assert.AreEqual(1.8, v1P8, 1.8 * .05);// within 5%
        }

        [TestMethod]
        public void Test_GetV2P7()
        {
            var v2P7 = _module.Cyclops.V2P7();
            Assert.IsNotNull(v2P7);
            Assert.AreEqual(2.5, v2P7, 2.5 * .05);// within 5%
        }

        [TestMethod]
        public void Test_GetIPMON()
        {
            var ipmon = _module.Cyclops.Ipmon();
            Assert.IsNotNull(ipmon);
        }


        [TestMethod]
        public void Test_GetVOA0_IFF()
        {
            var voa0Iff = _module.Cyclops.VOA0_IFFAsync();
            Assert.IsNotNull(voa0Iff);
        }

        [TestMethod]
        public void Test_GetVOA1IFF()
        {
            var voa1Iff = _module.Cyclops.VOA1_IFFAsync();
            Assert.IsNotNull(voa1Iff);
        }

        [TestMethod]
        public void Test_GetCOCOAConfigPage()
        {
            byte[] pages = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 16 };
            var sw = Stopwatch.StartNew();
            var dump = pages.Select(async page => await _module.Cyclops.GetCocoaConfigPageAsync((CyclopsRegisters.ConfigPages)page)).ToList();
            sw.Stop();
            Assert.IsNotNull(dump);
        }

        [TestMethod]
        public void Test_GetCOCOAStatusPage()
        {
            byte[] pages = { 32, 33, 34, 35 };
            var dump = pages.Select(async page =>await _module.Cyclops.GetCocoaStatusPageAsync((CyclopsRegisters.StatusPages)page)).ToList();
            Assert.IsTrue(dump.Any());
        }

        [TestMethod]
        public async Task Test_SetCoCOATempOffsetToPORValues()
        {
            await _module.Cyclops.SetCoCoaTemperatureOffsetToPorValuesAsync();
            var pageData = await _module.Cyclops.GetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages.Page00);
            Assert.IsNotNull(pageData);
        }

        [TestMethod]
        public async Task Test_CalibrateCoCOATemperature()
        {
            await  _module.Cyclops.SetCoCoaTemperatureOffsetToPorValuesAsync();
            var calBytes = await _module.Cyclops.CalibrateCoCoaTemperatureAsync(40 * 256, (int)(18.5 * 256));
            Assert.IsNotNull(calBytes);
            var r = await _module.DutGpio.Reset(TimeSpan.FromMilliseconds(200), TimeSpan.FromSeconds(5));
            Assert.IsTrue(r);
            var pageData = await _module.Cyclops.GetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages.Page00);
            Assert.IsNotNull(pageData);
        }

        //validate below here
        
        [TestMethod]
        public async Task TestVpoly0Async()
        {
            var resultAsync = await _module.Cyclops.Vpoly0Async();
            Trace.WriteLine($"Vpoly0Async: {resultAsync}");
            Assert.IsNotNull(resultAsync);
        }

        [TestMethod]
        public void TestVpoly0()
        {
            var result =  _module.Cyclops.Vpoly0();
            Trace.WriteLine($"Vpoly0: {result}");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestVpoly1Async()
        {
            var resultAsync = await _module.Cyclops.Vpoly1Async();
            Trace.WriteLine($"Vpoly1Async: {resultAsync}");
            Assert.IsNotNull(resultAsync);
        }

        [TestMethod]
        public void TestVpoly1()
        {
            var result = _module.Cyclops.Vpoly1();
            Trace.WriteLine($"Vpoly1: {result}");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestVpoly2Async()
        {
            var resultAsync = await _module.Cyclops.Vpoly2Async();
            Trace.WriteLine($"Vpoly2Async: {resultAsync}");
            Assert.IsNotNull(resultAsync);
        }

        [TestMethod]
        public void TestVpoly2()
        {
            var result = _module.Cyclops.Vpoly2();
            Trace.WriteLine($"Vpoly2: {result}");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task TestVpoly3Async()
        {
            var resultAsync = await _module.Cyclops.Vpoly3Async();
            Trace.WriteLine($"Vpoly3Async: {resultAsync}");
            Assert.IsNotNull(resultAsync);
        }

        [TestMethod]
        public void TestVpoly3()
        {
            var result = _module.Cyclops.Vpoly3();
            Trace.WriteLine($"Vpoly3: {result}");
            Assert.IsNotNull(result);
        }



    }
}