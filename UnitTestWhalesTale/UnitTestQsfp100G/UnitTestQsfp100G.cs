using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhalesTale;
using WhalesTale.Module;
using WhalesTale.QSFP;
using WhalesTale.QSFP100;

namespace UnitTestWhalesTale.UnitTestQsfp100G
{
    [TestClass]
    public class UnitTestQsfp100G
    {
        //        private const string Sub20SerialNumber = "0x2A0A"; // DUT
        private const string Sub20SerialNumber = "0x5858";   // Front DUT @Home

        private Module _module;
        private IQsfp _qsfp;

        [TestInitialize]
        public void TestInit()
        {
            var policies = new Policies();

            _module = Module.ModuleFactory(Sub20SerialNumber,policies.PolicyWrap);
            _qsfp = (IQsfp)_module.Qsfp100G;
        }

        [TestCleanup]
        public void TestCleanup() => _module.Dispose();


        //**************************  BELOW HERE ARE VERIFIED  *********************************************

        [TestMethod]
        // Confirming that inherited QSFP class is implemented
        public void TestSerialNumber()
        {
            var result = _qsfp.SerialNumber();
            Trace.WriteLine($"SerialNumber: {result}");
            Assert.IsNotNull(result); //  QSFP
        }

        [TestMethod]
        public async Task TestUpdate_CalibrationAsync()
        {
            var readResult = await _module.Qsfp100G.Update_CalibrationAsync().ConfigureAwait(false); ;
            Trace.WriteLine($"Update_CalibrationAsync: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestUpdate_Calibration()
        {
            var readResult = _module.Qsfp100G.Update_Calibration();
            Trace.WriteLine($"Update_Calibration: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestResetPrbsValidationAsync()
        {
            var readResult = await _module.Qsfp100G.ResetPrbsValidationAsync().ConfigureAwait(false); ;
            Trace.WriteLine($"CoCoaResetPrbsValidationAsync: {readResult}");
            Assert.IsNotNull(readResult);
        }
        
        [TestMethod]
        public void TestResetPrbsValidation()
        {
            var readResult =  _module.Qsfp100G.ResetPrbsValidation();
            Trace.WriteLine($"CoCoaResetPrbsValidation: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestGetCiscoSpecificConfigurationAsync()
        {
            var readResult = await _module.Qsfp100G.GetCiscoSpecificConfigurationAsync().ConfigureAwait(false); Trace.WriteLine($"GetCiscoSpecificConfiguration: {readResult}");
            Assert.IsNotNull(readResult);
            // ReSharper disable once MethodHasAsyncOverload
            var syncVersion = _module.Qsfp100G.GetCiscoSpecificConfiguration();
            CollectionAssert.AreEquivalent(readResult, syncVersion);
        }

        [TestMethod]
        public void TestGetCiscoSpecificConfiguration()
        {
            var readResult = _module.Qsfp100G.GetCiscoSpecificConfiguration();
            Trace.WriteLine($"GetCiscoSpecificConfiguration: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestCoCoaTemperatureAsync()
        {
            var readResult = await _module.Qsfp100G.CoCoaTemperatureAsync().ConfigureAwait(false); ;
            Trace.WriteLine($"CoCoaTemperatureAsync: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestCoCoaTemperature()
        {
            var readResult =  _module.Qsfp100G.CoCoaTemperature();
            Trace.WriteLine($"CoCoaTemperature: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestRssiAsync()
        {
            var readResult = await _module.Qsfp100G.RssiAsync().ConfigureAwait(false); ;
            Trace.WriteLine($"RssiAsync: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestRssi()
        {
            var readResult = _module.Qsfp100G.Rssi();
            Trace.WriteLine($"Rssi: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestPs0P75Async()
        {
            var readResult = await _module.Qsfp100G.Ps0P75Async().ConfigureAwait(false); ;
            Trace.WriteLine($"Ps0P75Async: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestPs0P75()
        {
            var readResult = _module.Qsfp100G.Ps0P75();
            Trace.WriteLine($"Ps0P75: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestPs1P00Async()
        {
            var readResult = await _module.Qsfp100G.Ps1P00Async().ConfigureAwait(false); ;
            Trace.WriteLine($"Ps1P00Async: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestPs1P00()
        {
            var readResult = _module.Qsfp100G.Ps1P00();
            Trace.WriteLine($"Ps1P00: {readResult}");
            Assert.IsNotNull(readResult);
        }
        [TestMethod]
        public async Task TestPs1P80Async()
        {
            var readResult = await _module.Qsfp100G.Ps1P80Async().ConfigureAwait(false); ;
            Trace.WriteLine($"Ps1P80Async: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestPs1P80()
        {
            var readResult = _module.Qsfp100G.Ps1P80();
            Trace.WriteLine($"Ps1P80: {readResult}");
            Assert.IsNotNull(readResult);
        }
        [TestMethod]
        public async Task TestPs2P70Async()
        {
            var readResult = await _module.Qsfp100G.Ps2P70Async().ConfigureAwait(false); ;
            Trace.WriteLine($"Ps2P70Async: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestPs2P70()
        {
            var readResult = _module.Qsfp100G.Ps2P70();
            Trace.WriteLine($"Ps2P70: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestRefV2P50Async()
        {
            var readResult = await _module.Qsfp100G.RefV2P50Async().ConfigureAwait(false);
            Trace.WriteLine($"RefV2P50Async: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestRefV2P50()
        {
            var readResult = _module.Qsfp100G.RefV2P50();
            Trace.WriteLine($"RefV2P50: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public async Task TestRxSquelchDisableSetAndGet()
        {
            await _module.Qsfp100G.RxSquelchDisableSetAsync(EnableLane.None).ConfigureAwait(false);
            var readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(0, readResult);
            
            await _module.Qsfp100G.RxSquelchDisableSetAsync(EnableLane.L1).ConfigureAwait(false);
             readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1,readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(EnableLane.L2).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(2, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(EnableLane.L3).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(4, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(EnableLane.L4).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(8, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(EnableLane.All).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(true,false,false,false).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(true, true, true, true).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(1).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.RxSquelchDisableSetAsync(15).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.RxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            Trace.WriteLine($"RxSquelchDisableGetAsync: {readResult}");
        }

        [TestMethod]
        public async Task TestTxSquelchDisableSetAndGet()
        {
            byte readResult;

            await _module.Qsfp100G.TxSquelchDisableSetAsync(EnableLane.None).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(0, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(EnableLane.L1).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(EnableLane.L2).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(2, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(EnableLane.L3).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(4, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(EnableLane.L4).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(8, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(EnableLane.All).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(true, false, false, false).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(true, true, true, true).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(1).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.TxSquelchDisableSetAsync(15).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxSquelchDisableGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            Trace.WriteLine($"TxSquelchDisableGetAsync: {readResult}");
        }

       
        [TestMethod]
        public async Task TestErrorCodeAsync()
        {
            var readResult = await _module.Qsfp100G.ErrorCodeAsync().ConfigureAwait(false);
            Trace.WriteLine($"ErrorCodeAsync: {readResult}");
            Assert.IsNotNull(readResult);
            Assert.AreEqual(0,readResult);
        }

        [TestMethod]
        public void TestErrorCode()
        {
            var readResult = _module.Qsfp100G.ErrorCode();
            Trace.WriteLine($"ErrorCode: {readResult}");
            Assert.IsNotNull(readResult);
            Assert.AreEqual(0, readResult);
        }

        [TestMethod]
        public async Task TestErrorOptionalDataAsync()
        {
            var readResult = await _module.Qsfp100G.ErrorOptionalDataAsync().ConfigureAwait(false);
            Trace.WriteLine($"ErrorOptionalDataAsync: {readResult}");
            Assert.IsNotNull(readResult);
            Assert.AreEqual(0, readResult);
        }

        [TestMethod]
        public void TestErrorOptionalData()
        {
            var readResult = _module.Qsfp100G.ErrorOptionalData();
            Trace.WriteLine($"ErrorOptionalData: {readResult}");
            Assert.IsNotNull(readResult);
            Assert.AreEqual(0, readResult);
        }

        [TestMethod]
        public async Task TestFEC_KR4_BypassEnableAsync()
        {
            var readResult = await _module.Qsfp100G.FEC_KR4_BypassEnableAsync(true).ConfigureAwait(false);
            Trace.WriteLine($"FEC_KR4_BypassEnableAsync: {readResult}");
            Assert.IsNotNull(readResult);
            var state = await _module.Qsfp100G.FEC_KR4_BypassStateAsync().ConfigureAwait(false);
            Assert.IsTrue(state);
        }

        [TestMethod]
        public async Task TestTxAdaptiveEqualizationSetAndGet()
        {
            byte readResult;

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(EnableLane.None).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(0, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(EnableLane.L1).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(EnableLane.L2).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(2, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(EnableLane.L3).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(4, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(EnableLane.L4).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(8, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(EnableLane.All).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(true, false, false, false).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(true, true, true, true).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(1).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1, readResult);

            await _module.Qsfp100G.TxAdaptiveEqualizationSetAsync(15).ConfigureAwait(false);
            readResult = await _module.Qsfp100G.TxAdaptiveEqualizationGetAsync().ConfigureAwait(false);
            Assert.AreEqual(15, readResult);

            Trace.WriteLine($"TxAdaptiveEqualizationGetAsync: {readResult}");
        }


        [TestMethod]
        public async Task TestVendorTestModeSetAndGetAsync()
        {
            var setResult = await _module.Qsfp100G.VendorTestModeSetAsync(1).ConfigureAwait(false);
            Assert.IsTrue(setResult);
            Trace.WriteLine($"VendorTestModeSetAsync: {setResult}");
            var state = await _module.Qsfp100G.VendorTestModeGetAsync().ConfigureAwait(false);
            Assert.AreEqual(1,state);
            setResult = await _module.Qsfp100G.VendorTestModeSetAsync(0).ConfigureAwait(false);
            Assert.IsTrue(setResult);
            state = await _module.Qsfp100G.VendorTestModeGetAsync().ConfigureAwait(false);
            Assert.AreEqual(0, state);
        }


    }
}