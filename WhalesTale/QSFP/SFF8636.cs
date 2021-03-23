using WhalesTale.RegisterBase;

namespace WhalesTale.QSFP
{
    public sealed class Sff8636 : RegisterBase.RegisterBase
    {
        public static readonly byte I2CAddress = 0xA0;
        public static readonly byte PageRegAddr = 127;
        public readonly RegisterBase.RegisterBase Register;

        private Sff8636(string name, byte page, byte address, byte size, Access access, DataType type,
            bool signed = false, double scale = 0.0, bool dynamic = false) : base(page, address, size, access, type,
            name, signed, scale, dynamic)
        {
            Name = name;
            Register = new RegisterBase.RegisterBase(page, address, size, access, type, name, signed, scale, dynamic);
        }

        public static class Password // passwords
        {
            public const byte EntryAddress = 123;
            public const byte Size = 4;
        }

        public sealed class Page0
        {
            public sealed class Lower
            {
                public static readonly Sff8636 QsfpIdentifier =
                    new Sff8636("QSFP Identifier", 0, 0, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpRevisionCompliance =
                    new Sff8636("Revision Compliance", 0, 1, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpStatus =
                    new Sff8636("Status", 0, 2, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpLatchedTxRxLosIndicators =
                    new Sff8636("Latched Tx/Rx LOS indicators", 0, 3, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpLatchedTxFaultIndicators = new Sff8636("Latched Tx fault indicators",
                    0, 4, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpLatchedTxRxCdrLolFlag = new Sff8636("Latched Tx/Rx CDR LOL flag", 0,
                    5, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpLatchedTemperatureAlarms = new Sff8636("Latched temperature alarms",
                    0, 6, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpLatchedSupplyVoltageAlarms =
                    new Sff8636("Latched supply voltage alarms", 0, 7, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTecReadinessFlag = new Sff8636("TEC readiness flag - not used", 0, 8,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpRxL1L2PowerAlarms = new Sff8636("Latched Rx 1/2 power alarms", 0, 9,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpRxL3L4PowerAlarms = new Sff8636("Latched Rx 3/4 power alarms", 0, 10,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTxL1L2BiasAlarms = new Sff8636("Latched Tx 1/2 bias alarms", 0, 11,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTxL3L4BiasAlarms = new Sff8636("Latched Tx 3/4 bias alarms", 0, 12,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTxL1L2PowerAlarms = new Sff8636("Latched Tx 1/2 power alarms", 0, 13,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTxL3L4PowerAlarms = new Sff8636("Latched Tx 3/4 power alarms", 0, 14,
                    1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpReserved1521 =
                    new Sff8636("Reserved (15-17) - Cisco specific (18-21)", 0, 15, 7, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpTemperature = new Sff8636("Temperature", 0, 22, 2, Access.ReadOnly,
                    DataType.DecWord, true, 0.00390625, true);

                public static readonly Sff8636 QsfpReserved2425 =
                    new Sff8636("Reserved (24-25)", 0, 24, 2, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpSupplyVoltage = new Sff8636("Supply Voltage", 0, 26, 2,
                    Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpReserved2833 =
                    new Sff8636("Reserved (28-29) - Cisco specific (30-33)", 0, 28, 6, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpRxPowerL1 = new Sff8636("Rx1 Power", 0, 34, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpRxPowerL2 = new Sff8636("Rx2 Power", 0, 36, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpRxPowerL3 = new Sff8636("Rx3 Power", 0, 38, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpRxPowerL4 = new Sff8636("Rx4 Power", 0, 40, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpTxBiasL1 = new Sff8636("Tx1 Bias", 0, 42, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.002, true);

                public static readonly Sff8636 QsfpTxBiasL2 = new Sff8636("Tx2 Bias", 0, 44, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.002, true);

                public static readonly Sff8636 QsfpTxBiasL3 = new Sff8636("Tx3 Bias", 0, 46, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.002, true);

                public static readonly Sff8636 QsfpTxBiasL4 = new Sff8636("Tx4 Bias", 0, 48, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.002, true);

                public static readonly Sff8636 QsfpTxPowerL1 = new Sff8636("Tx1 Power", 0, 50, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpTxPowerL2 = new Sff8636("Tx2 Power", 0, 52, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpTxPowerL3 = new Sff8636("Tx3 Power", 0, 54, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpTxPowerL4 = new Sff8636("Tx4 Power", 0, 56, 2, Access.ReadOnly,
                    DataType.DecWord, false, 0.0001, true);

                public static readonly Sff8636 QsfpReserved5877 =
                    new Sff8636("Reserved (58-65) - Cisco specific (66-77)", 0, 58, 20, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpTxL1L2InputEqualization =
                    new Sff8636("TX1/TX2 input equalization values", 0, 78, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTxL3L4InputEqualization =
                    new Sff8636("TX3/TX4 input equalization values", 0, 79, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpReserved8085 =
                    new Sff8636("Cisco specific (80-81) - Reserved (82-85)", 0, 80, 6, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpTxDisable =
                    new Sff8636("Tx Disable", 0, 86, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpNotUsed8792 =
                    new Sff8636("Not used (87-92)", 0, 87, 6, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpPowerControl =
                    new Sff8636("Power Control", 0, 93, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpNotUsed9499 =
                    new Sff8636("Not used (94-99)", 0, 94, 6, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpTxRxLosMaskingBits = new Sff8636("Tx/Rx LOS masking bits", 0, 100, 1,
                    Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpTxFaultMaskingBits = new Sff8636("Tx fault masking bits", 0, 101, 1,
                    Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpTxRxCdrLolMaskingBits =
                    new Sff8636("Tx/Rx CDR Loss of Lock masking bits", 0, 102, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpTemperatureAlarmMaskingBits =
                    new Sff8636("Temperature alarms masking bits", 0, 103, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpSupplyVoltageAlarmMaskingBits =
                    new Sff8636("Supply voltage alarms masking bits", 0, 104, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpEnterpriseClassIndication =
                    new Sff8636("Enterprise Class Indication", 0, 105, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpMonitorSupport =
                    new Sff8636("Monitoring Support", 0, 106, 1, Access.ReadWrite, DataType.Binary);

                public static readonly Sff8636 QsfpNotUsed107 =
                    new Sff8636("Not used (107)", 0, 107, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpPropagationDelay =
                    new Sff8636("Propagation Delay", 0, 108, 2, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpAdvancedLPmodeFarSideManagedMinVoltage =
                    new Sff8636("Advanced Low Power Mode/Far Side Managed/Min Operating Voltage", 0, 110, 1,
                        Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpAssignedForPciExpress =
                    new Sff8636("Assigned for use by PCI Express", 0, 111, 2, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpFarAndNearEndImplementation =
                    new Sff8636("Far End and Near End Implementation", 0, 113, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpP0LowReserved =
                    new Sff8636("P0_Low reserved", 0, 114, 5, Access.ReadOnly, DataType.Binary);
            }

            public sealed class Upper
            {
                public static readonly Sff8636 QsfpIdentifier =
                    new Sff8636("Identifier", 0, 128, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpExtIdentifier =
                    new Sff8636("Ext. Identifier", 0, 129, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpConnector =
                    new Sff8636("Connector", 0, 130, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpEthernetComplianceCode =
                    new Sff8636("10/40G/100G Ethernet Compliance Code", 0, 131, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpUnusedComplianceCode =
                    new Sff8636("Unused Compliance Code", 0, 132, 7, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpEncoding =
                    new Sff8636("Encoding", 0, 139, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpBrNominal =
                    new Sff8636("BR, nominal", 0, 140, 1, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpExtendedRateCompliance =
                    new Sff8636("Extended RateSelect Compliance", 0, 141, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpLengthSmf =
                    new Sff8636("Length(SMF)", 0, 142, 1, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpLengthOm350 =
                    new Sff8636("Length(OM3 50um)", 0, 143, 1, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpLengthOm250 =
                    new Sff8636("Length(OM2 50um)", 0, 144, 1, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpLengthOm162P5 =
                    new Sff8636("Length(OM1 62.5um)", 0, 145, 1, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpLength =
                    new Sff8636("Length", 0, 146, 1, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpDeviceTech =
                    new Sff8636("Device Tech", 0, 147, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpVendorName =
                    new Sff8636("Vendor name", 0, 148, 16, Access.ReadOnly, DataType.String);

                public static readonly Sff8636 QsfpExtendedTransceiver =
                    new Sff8636("Extended Transceiver", 0, 164, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpVendorOui =
                    new Sff8636("Vendor OUI", 0, 165, 3, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpVendorPn =
                    new Sff8636("Vendor PN", 0, 168, 16, Access.ReadOnly, DataType.String);

                public static readonly Sff8636 QsfpVendorRev =
                    new Sff8636("Vendor Rev", 0, 184, 2, Access.ReadOnly, DataType.String);

                public static readonly Sff8636 QsfpWavelength =
                    new Sff8636("Wavelength", 0, 186, 2, Access.ReadOnly, DataType.DecWord);

                public static readonly Sff8636 QsfpWavelengthTolerance =
                    new Sff8636("Wavelength Tolerance", 0, 188, 2, Access.ReadOnly, DataType.DecWord);

                public static readonly Sff8636 QsfpMaxCaseTemperature =
                    new Sff8636("Max Case Temp", 0, 190, 2, Access.ReadOnly, DataType.Dec);

                public static readonly Sff8636 QsfpCcBase =
                    new Sff8636("CC_BASE", 0, 192, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpTxEqualizationRxAmplEmph =
                    new Sff8636("TX Equ, RX ampli/Emph Options", 0, 193, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpTxRxCdrOptionsAndOutputSquelch =
                    new Sff8636("Options for Tx/Rx CDR and output squelch", 0, 194, 1, Access.ReadOnly,
                        DataType.Binary);

                public static readonly Sff8636 QsfpModuleOptions =
                    new Sff8636("Module options", 0, 195, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpVendorSn =
                    new Sff8636("Vendor SN", 0, 196, 16, Access.ReadOnly, DataType.String);

                public static readonly Sff8636 QsfpDateCode =
                    new Sff8636("Date code", 0, 212, 8, Access.ReadOnly, DataType.String);

                public static readonly Sff8636 QsfpDiagnosticMonitoringType = new Sff8636("Diagnostic Monitoring Type",
                    0, 220, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpEnhancedOptions =
                    new Sff8636("Enhanced Options", 0, 221, 1, Access.ReadOnly, DataType.Binary);

                public static readonly Sff8636 QsfpExtendedBitRate =
                    new Sff8636("Extended Bit Rate", 0, 222, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpCcExt =
                    new Sff8636("CC_EXT", 0, 223, 1, Access.ReadOnly, DataType.Hex);

                public static readonly Sff8636 QsfpVendorSpecificSecurityCode =
                    new Sff8636("Vendor Specific Security Code", 0, 224, 32, Access.ReadOnly, DataType.Hex);
            }
        }

        public sealed class Page2
        {
            public static readonly Sff8636 QspfCleiCode =
                new Sff8636("CLEI Code", 2, 128, 10, Access.ReadOnly, DataType.String);

            public static readonly Sff8636 QspfCiscoPartNumber =
                new Sff8636("Cisco Part Number", 2, 138, 10, Access.ReadOnly, DataType.String);

            public static readonly Sff8636 QspfCiscoVid =
                new Sff8636("Cisco VID", 2, 148, 4, Access.ReadOnly, DataType.String);

            public static readonly Sff8636 QspfTempRange =
                new Sff8636("Temp range", 2, 152, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpReserved153154 =
                new Sff8636("Reserved (153-154)", 2, 153, 2, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpFirmwareVersionIdentifier =
                new Sff8636("FW version/identifier", 2, 155, 6, Access.ReadOnly, DataType.String);

            public static readonly Sff8636 QsfpCcUser128160 =
                new Sff8636("CC_USER [128-160]", 2, 161, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpReserved162167 =
                new Sff8636("Reserved (162-167)", 2, 162, 6, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpLbcScaleNeg40C =
                new Sff8636("LBC_SCALE_NEG40C", 2, 168, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Sff8636 QsfpLbcScaleNeg5C =
                new Sff8636("LBC_SCALE_NEG5C", 2, 170, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Sff8636 QsfpLbcScale20C =
                new Sff8636("LBC_SCALE_20C", 2, 172, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Sff8636 QsfpLbcScale45C =
                new Sff8636("LBC_SCALE_450C", 2, 174, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Sff8636 QsfpLbcScale70C =
                new Sff8636("LBC_SCALE_70C", 2, 176, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Sff8636 QsfpLbcScale85C =
                new Sff8636("LBC_SCALE_85C", 2, 178, 2, Access.ReadOnly, DataType.DecWord);

            public static readonly Sff8636 QsfpTemp =
                new Sff8636("Temp", 2, 180, 2, Access.ReadOnly, DataType.DecWord, true, 0.00390625);

            public static readonly Sff8636 QsfpLbc1 =
                new Sff8636("LBC1", 2, 182, 2, Access.ReadOnly, DataType.DecWord, false, 0.0005);

            public static readonly Sff8636 QsfpOpt1 =
                new Sff8636("OPT1", 2, 184, 2, Access.ReadOnly, DataType.DecWord, false, 0.0001);

            public static readonly Sff8636 QsfpReserved186 =
                new Sff8636("Reserved 186", 2, 186, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpCcVendor162186 =
                new Sff8636("CC_Vendor [162-186]", 2, 187, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpReserved1881897 =
                new Sff8636("Reserved (188-189)", 2, 188, 2, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpFixedValue0XAa7 =
                new Sff8636("Fixed value = 0xAA", 2, 190, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpExtendedIdChecksum188190 = new Sff8636("Extended ID Checksum [188-190]",
                2, 191, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpPid = new Sff8636("PID", 2, 192, 20, Access.ReadOnly, DataType.String);

            public static readonly Sff8636 QsfpReserved212222 =
                new Sff8636("Reserved (212-222)", 2, 212, 11, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpChecksum192222 =
                new Sff8636("Checksum [192-222]", 2, 223, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpCiscoMemoryMapSpecEdcs = new Sff8636(
                "Cisco QSFP100G Memory Map Specification EDCS Number (First 6 characters)", 2, 224, 6, Access.ReadOnly,
                DataType.String);

            public static readonly Sff8636 QsfpCiscoSpecRev =
                new Sff8636("Cisco QSFP100G Specification Revision Number", 2, 230, 2, Access.ReadOnly,
                    DataType.String);

            public static readonly Sff8636 QsfpCiscoMemoryMapSpecEdcsRevLetter =
                new Sff8636("Cisco QSFP100G Memory Map Specification EDCS Number (Last character)", 2, 232, 1,
                    Access.ReadOnly, DataType.String);

            public static readonly Sff8636 QsfpCcVendor224232 =
                new Sff8636("CC_VENDOR [224-232]", 2, 233, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpLbc2 =
                new Sff8636("LBC2", 2, 234, 2, Access.ReadOnly, DataType.DecWord, false, 0.0005);

            public static readonly Sff8636 QsfpLbc3 =
                new Sff8636("LBC3", 2, 236, 2, Access.ReadOnly, DataType.DecWord, false, 0.0005);

            public static readonly Sff8636 QsfpLbc4 =
                new Sff8636("LBC4", 2, 238, 2, Access.ReadOnly, DataType.DecWord, false, 0.0005);

            public static readonly Sff8636 QsfpOpt2 =
                new Sff8636("OPT2", 2, 240, 2, Access.ReadOnly, DataType.DecWord, false, 0.0001);

            public static readonly Sff8636 QsfpOpt3 =
                new Sff8636("OPT3", 2, 242, 2, Access.ReadOnly, DataType.DecWord, false, 0.0001);

            public static readonly Sff8636 QsfpOpt4 =
                new Sff8636("OPT4", 2, 244, 2, Access.ReadOnly, DataType.DecWord, false, 0.0001);

            public static readonly Sff8636 QsfpCcVendor234245 =
                new Sff8636("CC_VENDOR [234-245]", 2, 246, 1, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpReserved247254 =
                new Sff8636("CC_VENDOR [247-254]", 2, 247, 8, Access.ReadOnly, DataType.Hex);

            public static readonly Sff8636 QsfpCcVendor247254 =
                new Sff8636("CC_VENDOR [247-254]", 2, 255, 1, Access.ReadOnly, DataType.Hex);
        }

        public sealed class Page3
        {
            public static readonly Sff8636 QsfpInputEqualization = new Sff8636("INPUT EQUALIZATION", 3, 234, 2, Access.ReadWrite, DataType.HexWord);
          
            public static class SerialId // Serial ID page constants
            {
                public const byte DPage = 0;
                public const byte ChecksumStartAddr = 128;
                public const byte ChecksumEndAddr = 190;
                public const byte ChecksumAddr = 191;
                public const byte ExtChecksumStartAddr = 192;
                public const byte IdExtChecksumEndAddr = 222;
                public const byte IdExtChecksumAddr = 223;
            }

            public static class UserEeprom // User EEPROM page constants
            {
                public const byte Page = 2;
                public const byte CcUserStartAddr = 128;
                public const byte CcUserEndAddr = 160;
                public const byte CcUserAddr = 161;
                public const byte CcVendorStartAddr = 162;
                public const byte CcVendorEndAddr = 186;
                public const byte CcVendorAddr = 187;
                public const byte ExtIdCsStartAddr = 188;
                public const byte ExtIdCsEndAddr = 190;
                public const byte ExtIdCsAddr = 191;
                public const byte CcVendor2CsStartAddr = 192;
                public const byte CcVendor2CsEndAddr = 222;
                public const byte CcVendor2CsAddr = 223;
                public const byte CcVendor3CsStartAddr = 224;
                public const byte CcVendor3CsEndAddr = 232;
                public const byte CcVendor3CsAddr = 233;
                public const byte CcVendor4CsStartAddr = 234;
                public const byte CcVendor4CsEndAddr = 245;
                public const byte CcVendor4CsAddr = 246;
                public const byte CcVendor5CsStartAddr = 247;
                public const byte CcVendor5CsEndAddr = 254;
                public const byte CcVendor5CsAddr = 255;
            }

            public static class Status // Status constants
            {
                public const byte RegisterAddress = 2;
                public const byte FlatMemoryBit = 0x04;
                public const byte IntLBit = 0x02;
                public const byte DataNotReadyBit = 0x01;
            }

            public static class Control // Control constants
            {
                public const byte TxDisableAddr = 86;
                public const byte TxDisableBitShift = 0;
                public const byte RxRateSelectAddr = 87;
                public const byte TxRateSelectAddr = 88;

                public const byte PowerControlAddr = 93;
                public const byte PowerSetBit = 0x02;
                public const byte PowerOverrideBit = 0x01;
                public const byte OptionalChannelControlsPage = 3;
                public const byte RxAmpMask = 0x0f;

                public const byte RxSqDisableAddr = 240;
                public const byte RxSqDisableBitShift = 4;
                public const byte TxSqDisableAddr = 240;
                public const byte TxSqDisableBitShift = 0;
                public const byte RxOutputDisableAddr = 241;
                public const byte RxOutputDisableBitShift = 4;

                public sealed class ApplicationSelectAddress
                {
                    public static BaseRegLanes Rx = new BaseRegLanes(92, 91, 90, 89);
                    public static BaseRegLanes Tx = new BaseRegLanes(97, 96, 95, 94);
                }

                public static class OutputAmplitude
                {
                    public static BaseRegLanes RxAddress = new BaseRegLanes(238, 238, 239, 239);
                    public static BaseRegLanes RxBitShift = new BaseRegLanes(4, 0, 4, 0);
                }

                public static class Monitors // Monitor address constants
                {
                    public const byte TemperatureMonitorAddr = 22; // 22 = MSB, 23 = LSB
                    public const byte SupplyVoltageMonitorAddr = 26; // 26 = MSB, 27 = LSB

                    public static class PowerMonitor
                    {
                        public static BaseRegLanes Rx = new BaseRegLanes(34, 36, 38, 40);
                        public static BaseRegLanes Tx = new BaseRegLanes(50, 52, 54, 56);
                    }

                    public static class Bias
                    {
                        public static BaseRegLanes Tx = new BaseRegLanes(42, 44, 46, 48);
                    }
                }

                public static class Thresholds // Threshold constants
                {
                    public const byte ThresholdsPage = 3;

                    public const byte PostEmphasisCase1 = 160;
                    public const byte PostEmphasisCase2 = 161;
                    public const byte PostEmphasisCase3 = 162;
                    public const byte PostEmphasisCase4 = 163;
                    public const byte PostEmphasisCase5 = 164;
                    public const byte PostEmphasisCase6 = 165;
                    public const byte PostEmphasisCase7 = 166;
                    public const byte PostEmphasisCase8 = 167;

                    public static class Temperature
                    {
                        public static BaseRegAlarmsWarnings Tx = new BaseRegAlarmsWarnings(128, 130, 132, 134);
                    }

                    public static class SupplyVoltage
                    {
                        public static BaseRegAlarmsWarnings Tx = new BaseRegAlarmsWarnings(144, 146, 148, 150);
                    }

                    public static class RxPower
                    {
                        public static BaseRegAlarmsWarnings Tx = new BaseRegAlarmsWarnings(176, 178, 180, 182);
                    }

                    public static class TxBias
                    {
                        public static BaseRegAlarmsWarnings Tx = new BaseRegAlarmsWarnings(184, 186, 188, 190);
                    }

                    public static class TxPower
                    {
                        public static BaseRegAlarmsWarnings Tx = new BaseRegAlarmsWarnings(192, 194, 196, 198);
                    }
                }

                public static class LatchedAddresses // Latch address constants
                {
                    public static class TxLos
                    {
                        public const byte Address = 3;
                        public const byte BitShift = 4;
                        public const byte Mask = 100;
                    }

                    public static class RxLos
                    {
                        public const byte Address = 3;
                        public const byte BitShift = 0;
                        public const byte Mask = 100;
                    }

                    public static class TxFault
                    {
                        public const byte Address = 4;
                        public const byte BitShift = 0;
                        public const byte Mask = 101;
                    }

                    public static class Temperature
                    {
                        public const byte Address = 6;
                        public const byte BitShift = 4;
                        public const byte Mask = 103;
                    }

                    public static class SupplyVoltage
                    {
                        public const byte Address = 7;
                        public const byte BitShift = 0;
                        public const byte Mask = 104;
                    }
                }

                public static class RxPower
                {
                    public static BaseRegLanes Address = new BaseRegLanes(9, 9, 10, 10);
                    public static BaseRegLanes BitShift = new BaseRegLanes(4, 0, 4, 0);
                    public static BaseRegLanes Mask = new BaseRegLanes(242, 242, 243, 243);
                }

                public static class TxBias
                {
                    public static BaseRegLanes Address = new BaseRegLanes(11, 11, 12, 12);
                    public static BaseRegLanes BitShift = new BaseRegLanes(4, 0, 4, 0);
                    public static BaseRegLanes Mask = new BaseRegLanes(244, 244, 245, 245);
                }

                public static class TxPower
                {
                    public static BaseRegLanes Address = new BaseRegLanes(13, 13, 14, 14);
                    public static BaseRegLanes BitShift = new BaseRegLanes(4, 0, 4, 0);
                    public static BaseRegLanes Mask = new BaseRegLanes(246, 246, 247, 247);
                }

                public static class UnCategorized
                {
                    public const byte InitializationCompleteLatchAddress = 6;
                    public const byte InitializationCompleteBitShift = 0;
                    public const byte HighAlarmBit = 0x08;
                    public const byte LowAlarmBit = 0x04;
                    public const byte HighWarningBit = 0x02;
                    public const byte LowWarningBit = 0x01;
                    public const byte MaskUpperPage = 3;

                    // Common constants
                    public const byte Channel1Bit = 0x01;
                    public const byte Channel2Bit = 0x02;
                    public const byte Channel3Bit = 0x04;
                    public const byte Channel4Bit = 0x08;
                }
            }
        }
    }
}