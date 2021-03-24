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
    public class UnitTestQsfp100G_Network
    {
        //        private const string Sub20SerialNumber = "0x2A0A"; // DUT
        private const string Sub20SerialNumber = "0x5858";   // Front DUT @Home

        private Module _module;

        [TestInitialize]
        public void TestInit()
        {
            var policies = new Policies();

            _module = Module.ModuleFactory(Sub20SerialNumber, policies.PolicyWrap );
        }

        [TestCleanup]
        public void TestCleanup() => _module.Dispose();


        //**************************  BELOW HERE ARE VERIFIED  *********************************************


        [TestMethod]
        public async Task TestBerAsync()
        {
            var readResult = await _module.Qsfp100G.Network.BerAsync(System.TimeSpan.FromSeconds(5)).ConfigureAwait(false); ;
            Trace.WriteLine($"BerAsync: {readResult}");
            Assert.IsNotNull(readResult);
        }

        [TestMethod]
        public void TestUpdate_Calibration()
        {
            var readResult = _module.Qsfp100G.Update_CalibrationAsync();
            Trace.WriteLine($"Update_Calibration: {readResult}");
            Assert.IsNotNull(readResult);
        }

    }
}