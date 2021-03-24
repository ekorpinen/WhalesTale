using System;
using System.Threading.Tasks;
using CiscoSub20;
using DutGpio;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WhalesTale;
using WhalesTale.Communication;
using WhalesTale.Cyclops;
using WhalesTale.MaCom;
using WhalesTale.Module;
using WhalesTale.QSFP;

namespace UnitTestWhalesTale
{
    [TestClass]
    public class UnitTestModule
    {
        //readonly string sub20_SerialNumber = "0x4194";
        //readonly string sub20_SerialNumber = "0x37B7"; // DUT
        //        private const string Sub20SerialNumber = "0x2A0A"; // DUT
        private const string Sub20SerialNumber = "0x5858";   // Front DUT @Home
        //readonly string sub20_SerialNumber = "0x340C"; // Golden Unit

        private CSub20 _s20;
        private Module _module;

        [TestInitialize]
        public void TestInit()
        {
            var policies = new Policies();

            var sub20Interfaces = new Sub20Interfaces();
            _s20 = new CSub20(Sub20SerialNumber, sub20Interfaces);

            var i2C = new Sub20I2C(_s20);
            var gpioConfiguration = new GpioConfiguration(0x00380000, 0x00380000);
            var gpio = new Sub20Gpio(_s20, gpioConfiguration);
            gpio.GpioInitialize();
            var dutGpio = new DutGpio.DutGpio(null, gpio, new DutGpioBits(16, 17, 18));

            var deviceIO = new DeviceIO(i2C, dutGpio, policies.PolicyWrap);
            var cyclops = new Cyclops(deviceIO);
            var qsfp100GFRS = new WhalesTale.QSFP100.Qsfp100G(deviceIO);
            var maCom   = new MaCom(deviceIO);
            _module = new Module(deviceIO,  qsfp100GFRS, cyclops, maCom, policies.PolicyWrap);
        }

        [TestCleanup]
        public void TestCleanup() => _s20.Dispose();

        //[TestMethod]
        //public async Task Test_CalibrateModuleTemperature2()
        //{
        //    var result = await _module.CalibrateModuleTemperature2(0.0, -0.0277586, true);
        //    Assert.IsNotNull(result);
        //}

        //[TestMethod]
        //public async Task Test_CalibrateModuleTemperature()
        //{
        //    await _module.CalibrateModuleTemperature(35, true);
        //}

        [TestMethod]
        public async Task Test_ResetAndDelay()
        {
            await _module.ResetAndDelay(TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(5), null);
        }
    }
}