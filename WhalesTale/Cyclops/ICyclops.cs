using System;
using System.Threading;
using System.Threading.Tasks;

namespace WhalesTale.Cyclops
{
    public interface ICyclops
    {
        Task<double> Vpoly0Async();
        Task<double> Vpoly1Async();
        Task<double> Vpoly2Async();
        Task<double> Vpoly3Async();

        Task<double> VIbiasAsync();
        Task<double> V0P8Async();
        Task<double> V1P0Async();
        Task<double> V1P8Async();
        Task<double> V2P7Async();

        Task<double> Iff1Async();
        Task<double> Iff2Async();

        Task<double> BiasAsync();

        double Vpoly0();
        double Vpoly1();
        double Vpoly2();
        double Vpoly3();

        double VIbias();
        double V0P8();
        double V1P0();
        double V1P8();
        double V2P7();

        double Iff1();
        double Iff2();

        double Bias();

        Task<bool> Set_LaserBias_CurrentAsync(double currentMilliAmp, TimeSpan? timeout = null);


        Task<byte[]> GetCocoaStatusPageAsync(CyclopsRegisters.StatusPages whichPage, TimeSpan? timeout = null);
        Task<byte[]> GetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages whichPage, TimeSpan? timeout = null);

        Task<bool> SetCocoaConfigPageAsync(CyclopsRegisters.ConfigPages whichPage, byte[] data,
            TimeSpan? timeout = null);

        Task<byte[]> SetCoCoaTemperatureOffsetToPorValuesAsync();
        Task<byte[]> CalibrateCoCoaTemperatureAsync(int moduleTemperatureCount, int coCoaTemperatureCount);
        Task<byte[]> SetCoCoaTemperatureOffsetAsync(byte[] data);


        Task<double> VOA0_IFFAsync();
        Task<double> VOA1_IFFAsync();

        Task<bool> ToggleCopyConfigAsync();

        Task<bool> SetVpolyAsync(int whichVpoly, byte[] data);

        Task<int> GetVoaIndexAsync();

        //Task<bool> SetCiscoCoCoaRegAsync(uint address, ushort len, uint[] data, TimeSpan? timeOut = null);
        //Task<dynamic> GetCiscoCoCoaRegAsync(uint address, ushort len, TimeSpan? timeOut = null);







        bool Set_TxOpenLoop(CancellationToken ct = default);
        bool Set_IFF1MeasureMode(CancellationToken ct = default);

        double Ipmon(CancellationToken ct = default);
        Task<double> IpmonAsync(CancellationToken ct = default);
        Task<int> IpmonCount(CancellationToken ct = default);



    }
}