using System;
using WhalesTale.RegisterBase;

namespace WhalesTale.Cyclops
{
    public sealed class CyclopsRegisters : RegisterBase.RegisterBase
    {
        public static readonly byte I2CAddress = 0xA0;
        public static readonly byte PageRegAddr = 127;
        public readonly RegisterBase.RegisterBase Register;

        public enum ConfigPages
        {
            Page00 = 0,
            Page01 = 1,
            Page02 = 2,
            Page03 = 3,
            Page04 = 4,
            Page05 = 5,
            Page06 = 6,
            Page07 = 7,
            Page08 = 8,
            Page09 = 9,
            Page10 = 10,
            Page16 = 16
        }

        public enum StatusPages
        {
            Page32 = 32,
            Page33 = 33,
            Page34 = 34,
            Page35 = 35
        }


//        public readonly string Name;

       // private CyclopsRegisters(string name, byte page, byte address, byte size, Access access, DataType dataType,
       //     bool signed = false, double scale = 0.0, bool dynamic = false)


       private CyclopsRegisters(string name, byte page, byte address, byte size, Access access, DataType dataType,
           bool signed = false, double scale = 1.0, bool dynamic = false) : base(page, address, size, access, dataType,
           name, signed, scale, dynamic)

       {
           Name = name ?? throw new ArgumentNullException(nameof(name));
           Register = new RegisterBase.RegisterBase(page, address, size, access, dataType, name, signed, scale,
               dynamic);
       }


       public sealed class Page01
        {
            public static readonly CyclopsRegisters MzdcControlRegister1 =
                new CyclopsRegisters("MZDC_CONTROL_REGISTER_1", 1, 1, 1, Access.ReadOnly, DataType.Binary);

            public static readonly CyclopsRegisters MzdcControlRegister2 =
                new CyclopsRegisters("MZDC_CONTROL_REGISTER_2", 1, 2, 1, Access.ReadOnly, DataType.Binary);

            public static readonly CyclopsRegisters PowerSelectLeftArmRegister =
                new CyclopsRegisters("POWER_SELECT_LEFT_ARM_REGISTER", 1, 115, 1, Access.ReadOnly, DataType.Binary);

            public static readonly CyclopsRegisters Voa0M32CA2D = new CyclopsRegisters("VOA0_M32C_A2D", 1, 126, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa00CA2D = new CyclopsRegisters("VOA0_0C_A2D", 1, 128, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa032CA2D = new CyclopsRegisters("VOA0_32C_A2D", 1, 130, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa064CA2D = new CyclopsRegisters("VOA0_64C_A2D", 1, 132, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa096CA2D = new CyclopsRegisters("VOA0_96C_A2D", 1, 134, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa1M32CA2D = new CyclopsRegisters("VOA1_M32C_A2D", 1, 136, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa10CA2D = new CyclopsRegisters("VOA1_0C_A2D", 1, 138, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa132CA2D = new CyclopsRegisters("VOA1_32C_A2D", 1, 140, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa164CA2D = new CyclopsRegisters("VOA1_64C_A2D", 1, 142, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);

            public static readonly CyclopsRegisters Voa196CA2D = new CyclopsRegisters("VOA1_96C_A2D", 1, 144, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.020, true);
        }

        public sealed class Page05
        {
            public static readonly CyclopsRegisters VPOLY0_VAL_REG_MSB =
                new CyclopsRegisters("VPOLY0_VAL_REG_MSB", 5, 0, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY0_VAL_REG_LSB =
                new CyclopsRegisters("VPOLY0_VAL_REG_LSB", 5, 1, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY1_VAL_REG_MSB =
                new CyclopsRegisters("VPOLY1_VAL_REG_MSB", 5, 2, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY1_VAL_REG_LSB =
                new CyclopsRegisters("VPOLY1_VAL_REG_LSB", 5, 3, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY2_VAL_REG_MSB =
                new CyclopsRegisters("VPOLY2_VAL_REG_MSB", 5, 4, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY2_VAL_REG_LSB =
                new CyclopsRegisters("VPOLY2_VAL_REG_LSB", 5, 5, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY3_VAL_REG_MSB =
                new CyclopsRegisters("VPOLY3_VAL_REG_MSB", 5, 6, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY3_VAL_REG_LSB =
                new CyclopsRegisters("VPOLY3_VAL_REG_LSB", 5, 7, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY4_VAL_REG_MSB =
                new CyclopsRegisters("VPOLY4_VAL_REG_MSB", 5, 8, 1, Access.ReadWrite, DataType.Hex);

            public static readonly CyclopsRegisters VPOLY4_VAL_REG_LSB =
                new CyclopsRegisters("VPOLY4_VAL_REG_LSB", 5, 9, 1, Access.ReadWrite, DataType.Hex);
        }

        public sealed class Page06
        {
            public static readonly CyclopsRegisters BiasCurrentGlobalControlRegister =
                new CyclopsRegisters("BIAS_CURRENT_GLOBAL_CONTROL_REGISTER", 6, 1, 1, Access.ReadWrite,
                    DataType.Binary);

            public static readonly CyclopsRegisters BiasCurrentControlRegister1 =
                new CyclopsRegisters("BIAS_CURRENT_CONTROL_REGISTER_1", 6, 2, 1, Access.ReadWrite, DataType.Binary);
        }

        public sealed class Page07
        {
            public static readonly CyclopsRegisters IffGroupAMultRegister1 =
                new CyclopsRegisters("IFF_GROUPA_MULT_REGISTER_1", 7, 52, 1, Access.ReadWrite, DataType.Binary);

            public static readonly CyclopsRegisters IffGroupAControlRegister1 =
                new CyclopsRegisters("IFF_GROUPA_CONTROL_REGISTER_1", 7, 56, 1, Access.ReadWrite, DataType.Binary);

            public static readonly CyclopsRegisters IffGroupAControlRegister5 =
                new CyclopsRegisters("IFF_GROUPA_CONTROL_REGISTER_5", 7, 60, 1, Access.ReadWrite, DataType.Binary);
        }

        public sealed class Page32
        {
            public static readonly CyclopsRegisters Bias = new CyclopsRegisters("A2D_TX_BIAS", 32, 8, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.008, true);

            public static readonly CyclopsRegisters Iff1 = new CyclopsRegisters("A2D_IFF1", 32, 24, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Iff1Bar = new CyclopsRegisters("A2D_IFF1_BAR", 32, 26, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Iff0APoltrk = new CyclopsRegisters("A2D_IFF0A_POLTRK", 32, 28, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Iff0BPoltrk = new CyclopsRegisters("A2D_IFF0B_POLTRK", 32, 30, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Iff2 = new CyclopsRegisters("A2D_IFF2", 32, 32, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Voa0Iff = new CyclopsRegisters("A2D_IFF_OLPBK_VOA0", 32, 34, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Voa1Iff = new CyclopsRegisters("A2D_IFF_OLPBK_VOA1", 32, 36, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.02, true);

            public static readonly CyclopsRegisters Voa0 = new CyclopsRegisters("COUNTER_VAL_MZDC_OLPBK_VOA0", 32, 70,
                2, Access.ReadOnly, DataType.HexWord, false, 0.0, true);

            public static readonly CyclopsRegisters Voa1 = new CyclopsRegisters("COUNTER_VAL_MZDC_OLPBK_VOA1", 32, 72,
                2, Access.ReadOnly, DataType.HexWord, false, 0.0, true);
        }

        public sealed class Page33
        {
            public static readonly CyclopsRegisters Vpoly0 = new CyclopsRegisters("V Poly 0", 33, 49, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters Vpoly1 = new CyclopsRegisters("V Poly 1", 33, 51, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters Vpoly2 = new CyclopsRegisters("V Poly 2", 33, 53, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters Vpoly3 = new CyclopsRegisters("V Poly 3", 33, 55, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters VIbias = new CyclopsRegisters("V IBias", 33, 57, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters V1P0 = new CyclopsRegisters("Vdd 1.0", 33, 59, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters V1P8 = new CyclopsRegisters("Vdd 1.8", 33, 61, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters V0P8 = new CyclopsRegisters("Vdd 0.8", 33, 63, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.0001, true);

            public static readonly CyclopsRegisters V2P7 = new CyclopsRegisters("Vdd 2.7", 33, 65, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.0001, true);
        }

        public sealed class Page35
        {
            public static readonly CyclopsRegisters Ipmon = new CyclopsRegisters("IPMON", 35, 0, 2, Access.ReadOnly,
                DataType.DecWord, false, 0.10, true);

            public static readonly CyclopsRegisters IpmonCntr = new CyclopsRegisters("IPMON_CNTR", 35, 16, 2,
                Access.ReadOnly, DataType.DecWord, false, 0.0, true);
        }

        
    }
}