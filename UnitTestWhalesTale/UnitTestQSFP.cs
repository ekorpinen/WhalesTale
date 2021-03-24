using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhalesTale;
using WhalesTale.Module;
using WhalesTale.QSFP;
using WhalesTale.QSFP100;

namespace UnitTestWhalesTale
{
    [TestClass]
    public class UnitTestQSFP
    {

//        private const string Sub20SerialNumber = "0x2A0A"; // DUT
        private const string Sub20SerialNumber = "0x5858";   // Front DUT @Home

        private Module _module;

        [TestInitialize]
        public void TestInit()
        {
            var policies = new Policies();
            _module = Module.ModuleFactory(Sub20SerialNumber, policies.PolicyWrap);
        }

        [TestCleanup]
        public void TestCleanup() => _module.Dispose();
        
        [TestMethod]
        public async Task TestTemperatureAsync()
        {
            var resultAsync = await _module.Qsfp100G.TemperatureAsync().ConfigureAwait(false);
            Trace.WriteLine($"TemperatureAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public async Task TestLoadNvrRanges()
        {
            var result = await _module.Qsfp100G.LoadNvrRanges(@"..\..\..\WT_NVR_Ranges.csv");

            //  Trace.WriteLine($"TemperatureAsync: {resultAsync}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestCheckPageForErrors()
        {
            _ = await _module.Qsfp100G.LoadNvrRanges(@"..\..\..\WT_NVR_Ranges.csv");
            var ranges = new List<NvrRange> {NvrRange.CreateInstance(Memory.Pages.NonVolatile.P0Lower, 0, 0)};

            var original = new byte[] {0};
            var current = new byte[] {0};
            var result = _module.Qsfp100G.CheckPageForErrors(original, current, ranges);

            Assert.AreEqual(0, result.Count);
            Trace.WriteLine($"CheckPageForErrors (no errors): {result.Count}");

            original = new byte[] {0};
            current = new byte[] {1};
            result = _module.Qsfp100G.CheckPageForErrors(original, current, ranges);
            Assert.AreEqual(1, result.Count);
            Trace.WriteLine($"CheckPageForErrors (with errors):  # errors ={result.Count}");
        }

        [TestMethod]
        public void TestTemperature()
        {
            var result = _module.Qsfp100G.Temperature();
            Trace.WriteLine($"Temperature: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestSerialNumberAsync()
        {
            var resultAsync = await _module.Qsfp100G.SerialNumberAsync().ConfigureAwait(false);
            Trace.WriteLine($"SerialNumberAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestSerialNumber()
        {
            var result = _module.Qsfp100G.SerialNumber();
            Trace.WriteLine($"SerialNumber: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestFirmwareVersionAsync()
        {
            var resultAsync = await _module.Qsfp100G.FirmwareVersionAsync().ConfigureAwait(false);
            Trace.WriteLine($"FirmwareVersionAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestFirmwareVersion()
        {
            var result = _module.Qsfp100G.FirmwareVersion();
            Trace.WriteLine($"FirmwareVersion: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TxManualCtleAsync()
        {
            var resultAsync = await _module.Qsfp100G.TxManualCtleAsync().ConfigureAwait(false);
            Trace.WriteLine($"TxManualCTLEAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }
        [TestMethod]
        public void TxManualCtle()
        {
            var result = _module.Qsfp100G.TxManualCtle();
            Trace.WriteLine($"TxManualCTLE: {result}");
            Assert.IsNotNull(result); //  QSFP
        }




        [TestMethod]
        public async Task TestRxLolAsync()
        {
            var resultAsync = await _module.Qsfp100G.RxLolAsync().ConfigureAwait(false);
            Trace.WriteLine($"RxLolAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestRxLol()
        {
            var result = _module.Qsfp100G.RxLol();
            Trace.WriteLine($"RxLol: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestRxPowerAsync()
        {
            var resultAsync = await _module.Qsfp100G.RxPowerAsync().ConfigureAwait(false);
            Trace.WriteLine($"RxPowerAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestRxPower()
        {
            var result = _module.Qsfp100G.RxPower();
            Trace.WriteLine($"RxPower: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestTxPowerAsync()
        {
            var resultAsync = await _module.Qsfp100G.TxPowerAsync().ConfigureAwait(false);
            Trace.WriteLine($"TxPowerAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestTxPower()
        {
            var result = _module.Qsfp100G.TxPower();
            Trace.WriteLine($"TxPower: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestSupplyVoltageAsync()
        {
            var resultAsync = await _module.Qsfp100G.SupplyVoltageAsync().ConfigureAwait(false);
            Trace.WriteLine($"SupplyVoltageAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestSupplyVoltage()
        {
            var result = _module.Qsfp100G.SupplyVoltage();
            Trace.WriteLine($"SupplyVoltage: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestTxBiasAsync()
        {
            var resultAsync = await _module.Qsfp100G.TxBiasAsync().ConfigureAwait(false);
            Trace.WriteLine($"TxBiasAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestTxBias()
        {
            var result = _module.Qsfp100G.TxBias();
            Trace.WriteLine($"TxBias: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestRxLosAsync()
        {
            var resultAsync = await _module.Qsfp100G.RxLosAsync().ConfigureAwait(false);
            Trace.WriteLine($"RxLosAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestRxLos()
        {
            var result = _module.Qsfp100G.RxLos();
            Trace.WriteLine($"RxLos: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public void TestTxAdaptiveCtle()
        {
            var result = _module.Qsfp100G.TxAdaptiveCtle();
            Trace.WriteLine($"TxManualCTLE: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestTxAdaptiveCtleAsync()
        {
            var resultAsync = await _module.Qsfp100G.TxAdaptiveCtleAsync().ConfigureAwait(false);
            Trace.WriteLine($"TxManualCTLEAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }



        [TestMethod]
        public async Task TestDataNotReadyAsync()
        {
            var resultAsync = await _module.Qsfp100G.DataNotReadyAsync().ConfigureAwait(false);
            Trace.WriteLine($"DataNotReadyAsync: {resultAsync}");
            Assert.IsNotNull(resultAsync); //  QSFP
        }

        [TestMethod]
        public void TestDataNotReady()
        {
            var result = _module.Qsfp100G.DataNotReady();
            Trace.WriteLine($"DataNotReady: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        private readonly SemaphoreSlim _testLock = new SemaphoreSlim(1);

        [TestMethod]
        public async Task TestTxDisableAsync()
        {
            try
            {

                await _testLock.WaitAsync();

                var resultAsync =
                    await _module.Qsfp100G.TxDisableAsync(enable: EnableLane.L1).ConfigureAwait(false);
                Trace.WriteLine($"TxDisableAsync: {resultAsync}");
                var resultTxDisableState = await _module.Qsfp100G.TxDisableStateAsync().ConfigureAwait(false);
                Trace.WriteLine($"TxDisableStateAsync after L1: {resultTxDisableState}");
                Assert.IsNotNull(resultAsync); //  QSFP
                var resultAsync2 =
                    await _module.Qsfp100G.TxDisableAsync(enable: EnableLane.None).ConfigureAwait(false);
                Trace.WriteLine($"TxDisableAsync: {resultAsync2}");
                var resultTxDisableState2 = await _module.Qsfp100G.TxDisableStateAsync().ConfigureAwait(false);
                Trace.WriteLine($"TxDisableStateAsync after L1: {resultTxDisableState2}");
            }
            catch (Exception e)
            {
                Trace.WriteLine($"ERROR: {e}");
                Console.WriteLine(e);
                throw;
            }
        }

        [TestMethod]
        public void TestTxDisable()
        {
            _testLock.Wait();
            var result = _module.Qsfp100G.TxDisable(enable: EnableLane.L1);
            Trace.WriteLine($"TxDisable: {result}");
            var resultTxDisableState = _module.Qsfp100G.TxDisableState();
            Trace.WriteLine($"TxDisableState after L1: {resultTxDisableState}");
            Assert.IsNotNull(result); //  QSFP
            var result2 = _module.Qsfp100G.TxDisable(enable: EnableLane.None);
            Trace.WriteLine($"TxDisable: {result2}");
            var resultTxDisableState2 = _module.Qsfp100G.TxDisableState();
            Trace.WriteLine($"TxDisableState after L1: {resultTxDisableState2}");
        }

        [TestMethod]
        public async Task TestGetPageAsync()
        {
            var page = await _module.Qsfp100G.GetPageAsync(Memory.Pages.NonVolatile.P0Lower).ConfigureAwait(false);
            Assert.AreEqual(17, page[0]); //  QSFP
        }

        [TestMethod]
        public void TestGetPage()
        {
            var page = _module.Qsfp100G.GetPage(Memory.Pages.NonVolatile.P0Lower);
            Assert.AreEqual(17, page[0]); //  QSFP
        }

    }
}

