using System;
using WhalesTale.RegisterBase;

namespace WhalesTale.QSFP100
{
    public sealed class Qsfp100GRegister : RegisterBase.RegisterBase
    {
        public static readonly byte I2CAddress = 0xA0;
        public static readonly byte PageRegAddr = 127;
        public readonly RegisterBase.RegisterBase Register;

        private Qsfp100GRegister(string name, byte page, byte address, byte size, Access access, DataType dataType,
            bool signed = false, double scale = 1.0, bool dynamic = false) : base(page, address, size, access, dataType,
            name, signed, scale, dynamic)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Register = new RegisterBase.RegisterBase(page, address, size, access, dataType, name, signed, scale,
                dynamic);
        }

        public sealed class Page3
        {
            public static readonly Qsfp100GRegister HostModeAdvertising = new Qsfp100GRegister(
                "Supported C2M electrical interfaces / FEC termination inside XCVR", 3, 226, 1, Access.ReadWrite,
                DataType.Hex);

            public static readonly Qsfp100GRegister HostModeSelect = new Qsfp100GRegister(
                "C2M electrical interfaces / line side FEC termination inside XCVR selection", 3, 227, 1,
                Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister TxRxSquelchDisable =
                new Qsfp100GRegister("Disable squelch Rx[7-4], Tx[3-0](L3-L0)", 3, 240, 1, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister RxOutputDisableTxAdpEqualization =
                new Qsfp100GRegister("Disable RxOutput[7-4], TxAdaptive[3-0](L4-L1)", 3, 241, 1, Access.ReadWrite,
                    DataType.Hex);
        }

        public sealed class Page4
        {
            public static readonly Qsfp100GRegister CoCoaPage =
                new Qsfp100GRegister("CoCOA Page Parameter", 4, 130, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaAddress =
                new Qsfp100GRegister("CoCOA Address parameter", 4, 131, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaLength =
                new Qsfp100GRegister("CoCOA Length parameter", 4, 132, 2, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaCheckSum =
                new Qsfp100GRegister("CoCOA Checksum parameter", 4, 134, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister Reserved135 =
                new Qsfp100GRegister("Reserved (135)", 4, 135, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaBypassOrStart =
                new Qsfp100GRegister("CoCOA bypass parameter or Start value", 4, 136, 2, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaVoaSelectorOrLoopback =
                new Qsfp100GRegister("CoCOA VOA selector parameter or Loopback mode", 4, 138, 1, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaArmSelector =
                new Qsfp100GRegister("CoCOA ARM selector parameter", 4, 139, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaDelay = new Qsfp100GRegister("CoCOA Delay value parameter", 4,
                140, 2, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoaStep = new Qsfp100GRegister("CoCOA Step value parameter", 4,
                142, 2, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister CoCoa32BitAddress =
                new Qsfp100GRegister("CoCOA 32bit Address parameter", 4, 144, 4, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister Reserved148160 =
                new Qsfp100GRegister("Reserved for future Vendor Cmd use(148-160)", 4, 148, 13, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister VendorTestMode = new Qsfp100GRegister("Vendor test mode", 4, 161, 1,
                Access.ReadWrite, DataType.Hex, false, 1, true);

            public static readonly Qsfp100GRegister CoCoaTemperature = new Qsfp100GRegister("CoCOA Temperature", 4, 162,
                2, Access.ReadOnly, DataType.DecWord, true, 0.00390625, true);

            public static readonly Qsfp100GRegister PowerSupply0P75 = new Qsfp100GRegister("0.75V Power Supply", 4, 164,
                2, Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister PowerSupply1P0 = new Qsfp100GRegister("1.0V Power Supply", 4, 166,
                2, Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister PowerSupply1P8 = new Qsfp100GRegister("1.8V Power Supply", 4, 168,
                2, Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister PowerSupply2P7 = new Qsfp100GRegister("2.7V Power Supply", 4, 170,
                2, Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister CoCoaVssMonitor = new Qsfp100GRegister("CoCOA Vss monitoring", 4,
                172, 2, Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister ErrorCode = new Qsfp100GRegister("Error code", 4, 174, 2,
                Access.ReadOnly, DataType.HexWord, dynamic: true);

            public static readonly Qsfp100GRegister ErrorOptionalData = new Qsfp100GRegister("Error optional data", 4,
                176, 2, Access.ReadOnly, DataType.HexWord, dynamic: true);

            public static readonly Qsfp100GRegister ReferenceVoltage2P5 = new Qsfp100GRegister("2.5V Reference", 4, 178,
                2, Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister CyclopsEcid = new Qsfp100GRegister("CyclopsRegisters ECID", 4, 180,
                4, Access.ReadOnly, DataType.Hex, false, 1, true);

            public static readonly Qsfp100GRegister RssiVoltage = new Qsfp100GRegister("RSSI Voltage", 4, 184, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly Qsfp100GRegister Reserved186191 =
                new Qsfp100GRegister("Reserved for future Vendor registers (186-191)", 4, 186, 6, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister ConfigFlags =
                new Qsfp100GRegister("Config Flags", 4, 192, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister QualFlags =
                new Qsfp100GRegister("Qual Flags", 4, 193, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister ModuleFlags =
                new Qsfp100GRegister("Qsfp Flags", 4, 194, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister RxSwapPnConfigFlags =
                new Qsfp100GRegister("Rx Swap p/n Config Flags", 4, 195, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister TxSwapPnConfigFlags =
                new Qsfp100GRegister("Tx Swap p/n Config Flags", 4, 196, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister RxLosAssert = new Qsfp100GRegister("Rx LOS assert", 4, 197, 2,
                Access.ReadWrite, DataType.DecWord, scale: 0.0001);

            public static readonly Qsfp100GRegister RxLosDeAssert = new Qsfp100GRegister("Rx LOS de-assert", 4, 199, 2,
                Access.ReadWrite, DataType.DecWord, scale: 0.0001);

            public static readonly Qsfp100GRegister TemperatureOffset0C = new Qsfp100GRegister("Temperature offset 0C",
                4, 201, 2, Access.ReadWrite, DataType.DecWord, true, 0.00390625);

            public static readonly Qsfp100GRegister TemperatureOffset75C =
                new Qsfp100GRegister("Temperature offset 75C", 4, 203, 2, Access.ReadWrite, DataType.DecWord, true,
                    0.00390625);

            public static readonly Qsfp100GRegister RxAmplitudeDefault =
                new Qsfp100GRegister("Default Rx Amplitude", 4, 205, 1, Access.ReadWrite, DataType.Hex);

            public static readonly Qsfp100GRegister SkewL0 = new Qsfp100GRegister("L0 Skew", 4, 206, 2,
                Access.ReadWrite, DataType.DecWord, true, 0);

            public static readonly Qsfp100GRegister SkewL1 = new Qsfp100GRegister("L1 Skew", 4, 208, 2,
                Access.ReadWrite, DataType.DecWord, true, 0);

            public static readonly Qsfp100GRegister SkewL2 = new Qsfp100GRegister("L2 Skew", 4, 210, 2,
                Access.ReadWrite, DataType.DecWord, true, 0);

            public static readonly Qsfp100GRegister Voa0SetPoint =
                new Qsfp100GRegister("VOA_0 set-point", 4, 212, 2, Access.ReadWrite, DataType.DecWord);

            public static readonly Qsfp100GRegister Voa1SetPoint =
                new Qsfp100GRegister("VOA_1 set-point", 4, 214, 2, Access.ReadWrite, DataType.DecWord);

            public static readonly Qsfp100GRegister ReservedModuleConfigFuture =
                new Qsfp100GRegister("Reserved for future module cfg registers", 4, 216, 5, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister ReservedManufacturingData =
                new Qsfp100GRegister("Reserved for Extra Manufacturing data (221-254)", 4, 221, 34, Access.ReadWrite,
                    DataType.Hex);

            public static readonly Qsfp100GRegister ModuleConfigCheckSum =
                new Qsfp100GRegister("Qsfp Config Checksum (192-254)", 4, 255, 1, Access.ReadWrite, DataType.Hex);

            public static class CiscoSpecificNvr
            {
                public const byte StartAddress = 192;
                public const byte EndAddress = 254;
            }
        }

        public sealed class Page5
        {
            public static readonly Qsfp100GRegister CommandBuffer1 =
                new Qsfp100GRegister("Command Buffer 1", 5, 128, 128, Access.ReadWrite, DataType.Hex);
        }

        public sealed class Page6
        {
            public static readonly Qsfp100GRegister CommandBuffer2 =
                new Qsfp100GRegister("Command Buffer 2", 6, 128, 128, Access.ReadWrite, DataType.Hex);
        }

        public sealed class Page7
        {
            public enum ClockSourceMaskReg129
            {
                External = 0x00,
                Internal = 0x01
            }

            public enum PatternMaskReg129
            {
                Prbs31 = 0x00,
                Prbs23 = 0x01,
                Prbs15 = 0x02,
                Prbs11 = 0x03,
                Prbs9 = 0x04,
                Prbs7 = 0x05
            }

            public enum PatternMaskReg166
            {
                NoPattern = 0x00,
                QPrbs2E13 = 0x01,
                Jp03A = 0x02,
                Jp03B = 0x03,
                Sq16 = 0x04,
                Linear = 0x05,
                Ssprq = 0x06,
                Div128 = 0x07,
                Div640 = 0x08,
                Div500 = 0x09,
                Div100 = 0x0A,
                Nrz9 = 0x0B,
                QPrbs2E31Half = 0x0C,
                QPrbs2E15Half = 0x0D,
                QPrbs2E9Half = 0x0D,
                QPrbs2E13Half = 0x0F,
                QPrbs2E31Qtr = 0x10,
                QPrbs2E15Qtr = 0x11,
                QPrbs2E9Qtr = 0x12,
                QPrbs2E13Qtr = 0x13,
                Pam418Rate = 0x14,
                Pam4116Rate = 0x15,
                Pam4132Rate = 0x16
            }

            public static readonly int Page = 7;

            public static readonly Qsfp100GRegister PrbsPatternsSupported =
                new Qsfp100GRegister("PRBS Pattern Support", 7, 128, 1, Access.ReadWrite, DataType.Binary);

            public static readonly Qsfp100GRegister PrbsClockPatternSelection = new Qsfp100GRegister(
                "PRBS Generator Clock Source and Pattern Selection", 7, 129, 1, Access.ReadWrite, DataType.Binary);

            public static readonly Qsfp100GRegister PrbsGeneratorEnable =
                new Qsfp100GRegister("PRBS Generator Enable", 7, 130, 1, Access.ReadWrite, DataType.Binary);

            public static readonly Qsfp100GRegister PrbsCheckerEnable =
                new Qsfp100GRegister("PRBS Checker Enable", 7, 131, 1, Access.ReadWrite, DataType.Binary);

            public static readonly Qsfp100GRegister PrbsBitAndErrorCountUpdateFreeze =
                new Qsfp100GRegister("PRBS Bit and Error Count Update/Freeze", 7, 132, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister PrbsGeneratorClockSource = new Qsfp100GRegister(
                "Recovered Host/NetworkSide Lane PRBS Generator Clock Source", 7, 133, 1, Access.ReadWrite,
                DataType.Binary);

            public static readonly Qsfp100GRegister PrbsBitCountHostL1 =
                new Qsfp100GRegister("Host Lane 1 PRBS Bit Count", 7, 134, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitCountHostL2 =
                new Qsfp100GRegister("Host Lane 2 PRBS Bit Count", 7, 136, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitCountHostL3 =
                new Qsfp100GRegister("Host Lane 3 PRBS Bit Count", 7, 138, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitCountHostL4 =
                new Qsfp100GRegister("Host Lane 4 PRBS Bit Count", 7, 140, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitErrorHostL1 =
                new Qsfp100GRegister("Host Lane 1 PRBS Error Count", 7, 142, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitErrorHostL2 =
                new Qsfp100GRegister("Host Lane 2 PRBS Error Count", 7, 144, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitErrorHostL3 =
                new Qsfp100GRegister("Host Lane 3 PRBS Error Count", 7, 146, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitErrorHostL4 =
                new Qsfp100GRegister("Host Lane 4 PRBS Error Count", 7, 148, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsBitCountNetwork =
                new Qsfp100GRegister("NetworkSide Lane PRBS Bit Count", 7, 150, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister Reserved152157 =
                new Qsfp100GRegister("Reserved (152-157)", 7, 152, 6, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PrbsErrorCountNetwork =
                new Qsfp100GRegister("NetworkSide Lane PRBS Error Count", 7, 158, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister Reserved160165 =
                new Qsfp100GRegister("Reserved (160-165)", 7, 160, 6, Access.ReadOnly, DataType.DecWord);

            public static readonly Qsfp100GRegister PatternGenerationAdditionalNetwork =
                new Qsfp100GRegister("NetworkSide additional generator pattern selection", 7, 166, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister PatternCheckerAdditionalNetwork =
                new Qsfp100GRegister("NetworkSide additional checker pattern selection", 7, 167, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister PatternGenerationAdditionalHost =
                new Qsfp100GRegister("Host additional generator pattern selection", 7, 168, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister PrbsHostCheckerLock =
                new Qsfp100GRegister("Host PRBS Checker lock register", 7, 169, 1, Access.ReadWrite, DataType.Binary);

            public static readonly Qsfp100GRegister PrbsNetworkCheckerLock =
                new Qsfp100GRegister("NetworkSide PRBS Checker lock register", 7, 170, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister Reserved171255 =
                new Qsfp100GRegister("Reserved (171-255)", 7, 171, 85, Access.ReadOnly, DataType.Hex);
        }

        public sealed class Page8
        {
            public enum FecPmdStatusMask
            {
                Fault = 0x01,
                TxFault = 0x02,
                RxFault = 0x04,
                SignalDetect = 0x08,
                FecSerError = 0x10
            }

            public static readonly Qsfp100GRegister FecCapabilitiesStatus =
                new Qsfp100GRegister("FEC capabilities status register", 8, 128, 1, Access.ReadOnly, DataType.Binary);

            public static readonly Qsfp100GRegister FecBypassCorrectionIndication =
                new Qsfp100GRegister("FEC bypass correction/indication control register", 8, 129, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister FecHighSer =
                new Qsfp100GRegister("FEC High SER register", 8, 130, 1, Access.ReadOnly, DataType.Binary);

            public static readonly Qsfp100GRegister FecCountersControl =
                new Qsfp100GRegister("FEC counters control register", 8, 131, 1, Access.ReadWrite, DataType.Binary);

            public static readonly Qsfp100GRegister FecTxErrorInjectionIdleEnable =
                new Qsfp100GRegister("FEC TX error injection/Idle enable control register", 8, 132, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly Qsfp100GRegister Reserved133 =
                new Qsfp100GRegister("Reserved (133)", 8, 133, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Qsfp100GRegister FecUncorrectableFrameCounter =
                new Qsfp100GRegister("FEC uncorrectable frame counter", 8, 134, 4, Access.ReadOnly, DataType.Int32);

            public static readonly Qsfp100GRegister FecCorrectableFrameCounter =
                new Qsfp100GRegister("FEC correctable frame counter", 8, 138, 4, Access.ReadOnly, DataType.Int32);

            public static readonly Qsfp100GRegister FecErrorSymbolCounter =
                new Qsfp100GRegister("FEC Error symbol counter", 8, 142, 4, Access.ReadOnly, DataType.Int32);

            public static readonly Qsfp100GRegister FecBipCounter =
                new Qsfp100GRegister("FEC BIP counter", 8, 146, 4, Access.ReadOnly, DataType.Int32);

            public static readonly Qsfp100GRegister FecTotalWordCounter =
                new Qsfp100GRegister("FEC Total Word Counter", 8, 150, 4, Access.ReadOnly, DataType.Int32);

            public static readonly Qsfp100GRegister FecPmdStatus =
                new Qsfp100GRegister("FEC PMD status register", 8, 154, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Qsfp100GRegister Reserved155255 =
                new Qsfp100GRegister("Reserved (151-255)", 8, 155, 101, Access.ReadOnly, DataType.Hex);
        }
    }
}