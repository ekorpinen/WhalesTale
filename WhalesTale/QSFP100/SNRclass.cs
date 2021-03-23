namespace WhalesTale.QSFP100
{
    public class SnrClass
    {
        public double Snr0 { get; set; }
        public double Snr1 { get; set; }
        public double Snr2 { get; set; }
        public double Snr3 { get; set; }

        public double SnrEyeU { get; set; }
        public double SnrEyeM { get; set; }
        public double SnrEyeL { get; set; }

        public int Status { get; set; }

        public byte Skew0 { get; set; }
        public byte Skew1 { get; set; }
        public byte Skew2 { get; set; }
        public byte Skew3 { get; set; }
    }
}