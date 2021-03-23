using WhalesTale.RegisterBase;

namespace WhalesTale
{
    public static class VendorCommand
    {
        public const byte NumChannels = 1; // Optical channels
        public const byte PageRegAddr = 127;
        public const byte I2CAddress = 0xA0;
        public const byte NumPages = 4;
        public const byte MaxWriteLen = 8;

        public const byte IdentifierRegAddr = 0; // Identifier constants

        public const byte Page1 = 4;
        public const byte Page2 = 5;
        public const byte Page3 = 6;

        public const byte Address = 128;

        public const byte StatusAddress = 129;

        public const byte ModuleSpecificBase = 146;


        public sealed class Status : BaseReg<Status, byte>
        {
            public static readonly Status Idle = new Status(0, "Idle");
            public static readonly Status Complete = new Status(1, "Command Complete");
            public static readonly Status InProgress = new Status(2, "Command In-Progress");
            public static readonly Status Failed = new Status(3, "Command Failed");
            public static readonly Status InvalidCommand = new Status(4, "Invalid Command");
            public static readonly Status InvalidParameter = new Status(5, "Invalid Parameter");
            public static readonly Status InvalidPassword = new Status(6, "Invalid Password");

            private Status(byte value, string name) : base(value, name)
            {
                Library.Add(value, this);
            }
        }

        public sealed class Command : BaseReg<Command, byte>
        {
            public static readonly Command CocoaTuneModeEnable = new Command(3, "Cocoa Tune Mode Enable");
            public static readonly Command CocoaTuneModeDisable = new Command(4, "Cocoa Tune Mode Disable");
            public static readonly Command CocoaLoadPageCfg = new Command(5, "Cocoa Load Page Configuration");
            public static readonly Command CocoaSavePageCfg = new Command(6, "Cocoa Save Page Configuration");
            public static readonly Command CiscoCocoaReadCipPage = new Command(7, "Cisco Cocoa Read CIP Page");
            public static readonly Command CiscoCocoaWriteCipPage = new Command(8, "Cisco Cocoa Write CIP Page");

            public static readonly Command CiscoCocoaSetIbiasByCurrent =
                new Command(9, "Cisco Cocoa Set Ibias By Current");

            public static readonly Command CiscoCocoaSetIbiasByIff1 = new Command(10, "Cisco Cocoa Set Ibias By Iff1");

            public static readonly Command CiscoCocoaSetMziByCounter =
                new Command(11, "Cisco Cocoa Set MZI By Counter");

            public static readonly Command CiscoCocoaSetMziByCtp = new Command(12, "Cisco Cocoa Set MZI By Ctp");

            public static readonly Command CiscoCocoaSetVoaByCounter =
                new Command(13, "Cisco Cocoa Set VAO By Counter");

            public static readonly Command CiscoCocoaSetVoaByCtp = new Command(14, "Cisco Cocoa Set VOA By Ctp");

            public static readonly Command CiscoCocoaToggleCopyConfig =
                new Command(15, "Cisco Cocoa Toggle Copy Configuration");

            public static readonly Command CiscoCocoaMziSweep = new Command(16, "Cisco Cocoa MZI Sweep");
            public static readonly Command CiscoCocoaVoaSweep = new Command(17, "Cisco Cocoa VOA Sweep");
            public static readonly Command CiscoCocoaLaserSweep = new Command(18, "Cisco Cocoa Laser Sweep");
            public static readonly Command CocoaLoadVoaAttTab = new Command(19, "Cocoa Load VOA Attenuation Table");
            public static readonly Command CocoaSaveVoaAttTab = new Command(20, "Cocoa Save VOA Attenuation Table");
            public static readonly Command CiscoCocoaReadRegister = new Command(21, "Cisco Cocoa Read Register");
            public static readonly Command CiscoCocoaWriteRegister = new Command(22, "Cisco Cocoa Write Register");
            public static readonly Command UpdateCalibration = new Command(23, "Update Calibration");
            public static readonly Command LoopbackSelect = new Command(24, "Loopback Select");
            public static readonly Command EyeMonitor = new Command(25, "Eye Monitor");
            public static readonly Command SlicerHistogram = new Command(26, "Slicer Histogram");
            public static readonly Command SnrEstimator = new Command(27, "SNR Estimator");
            public static readonly Command BlocksPowerDown = new Command(28, "Blocks Power Down");
            public static readonly Command PatternTestControl = new Command(29, "Pattern Test Control");
            public static readonly Command GetPllStatus = new Command(30, "Get PLL Status");
            public static readonly Command GeneratePam4CustomPattern = new Command(31, "generate_pam4_custom_pattern");
            public static readonly Command GetCyclopsECID = new Command(32, "Get_Cyclops_ECID");
            public static readonly Command VoltageSupplyIncrement = new Command(33, "Voltage_Supp_Inc (PCBv2 Only)");
            public static readonly Command VoltageSupplyDecrement = new Command(34, "Voltage_Supp_Dec (PCBv2 Only)");
            public static readonly Command TP3EyeMonSliceGet = new Command(35, "TP3_Eye_Mon_Slice_Get");
            public static readonly Command CoCoaLoadVoaTxAttenTable = new Command(36, "cocoa_load_voaTx_att_tab (OASIS GF4 only)");
            public static readonly Command CoCoaSaveVoaTxAttenTable = new Command(37, "cocoa_save_voaTx_att_tab (OASIS GF4 only)");
            public static readonly Command CiscoCoCoaVoaTxSweep = new Command(38, "	cisco_cocoa_voaTx_sweep (OASIS GF4 only)");
            public static readonly Command CiscoCoCoaVoaTxSetByCounter = new Command(39, "cisco_cocoa_set_voaTx_by_counter (OASIS GF4 only)");
            public static readonly Command CiscoCoCoaVoaTxSetByCtp = new Command(40, "cisco_cocoa_set_voaTx_by_ctp (OASIS GF4 only)");

            private Command(byte value, string name) : base(value, name)
            {
                Library.Add(value, this);
            }
        }

        public static class Params
        {
            public const byte Maximum = 16;
            public const byte BaseAddress = 130;
            public const byte PageAddress = BaseAddress + 0;
            public const byte AddressAddress = BaseAddress + 1;
            public const byte LengthAddress = BaseAddress + 2;
            public const byte ChecksumAddress = BaseAddress + 4;
        }

        public static class Buffer
        {
            public const byte Address = 128; // page 05
            public const byte Size = 128;
        }
    }
}