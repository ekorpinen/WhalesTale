using WhalesTale.RegisterBase;

namespace WhalesTale.QSFP
{
    public sealed class Memory : PageBase
    {
        public readonly PageBase Register;
        public string Name;

        private Memory(string name, byte page, byte startAddress, byte length) : base(page, startAddress, length)
        {
            Name = name;
            Register = new PageBase(page, startAddress, length);
        }

        public static class Pages
        {
            public static class NonVolatile
            {
                public static readonly Memory P0Lower = new Memory("Lower page0", 0, 0, 128);
                public static readonly Memory P0Upper = new Memory("Upper page0", 0, 128, 128);
                public static readonly Memory P1Upper = new Memory("Upper page1", 1, 128, 128);
                public static readonly Memory P2Upper = new Memory("Upper page2", 2, 128, 128);
                public static readonly Memory P3Upper = new Memory("Upper page3", 3, 128, 128);
                public static readonly Memory P4Upper = new Memory("Upper page4", 4, 128, 128);
            }

            public static class Volatile
            {
                public static readonly Memory P5Upper = new Memory("Upper page5", 5, 128, 128);
                public static readonly Memory P6Upper = new Memory("Upper page6", 6, 128, 128);
                public static readonly Memory P7Upper = new Memory("Upper page7", 7, 128, 128);
                public static readonly Memory P8Upper = new Memory("Upper page8", 8, 128, 128);
            }
        }
    }
}