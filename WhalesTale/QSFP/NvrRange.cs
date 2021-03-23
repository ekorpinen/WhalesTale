namespace WhalesTale.QSFP
{
    public class NvrRange
    {
        private NvrRange(Memory page, int beginAddress, int endAddress)
        {
            Page = page;
            BeginAddress = beginAddress;
            EndAddress = endAddress;
        }

        public Memory Page { get; }
        public int BeginAddress { get; }
        public int EndAddress { get; }

        public static NvrRange CreateInstance(Memory page, int beginAddress, int endAddress) =>
            new NvrRange(page, beginAddress, endAddress);
    }
}