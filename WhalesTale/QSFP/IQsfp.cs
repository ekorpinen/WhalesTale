using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using WhalesTale.QSFP100;

namespace WhalesTale.QSFP
{
    public interface IQsfp
    {
        Task<List<NvrRange>> LoadNvrRanges(string filename);
        List<(Memory, int)> CheckPageForErrors(byte[] original, byte[] current, List<NvrRange> nvrRanges);

        Task<byte[]> GetPageAsync(Memory page);
        byte[] GetPage(Memory page);

        Task<double> RxPowerAsync();
        double RxPower();

        Task<string> FirmwareVersionAsync();
        string FirmwareVersion();

        Task<string> SerialNumberAsync();
        string SerialNumber();

        Task<double> TemperatureAsync();
        double Temperature();

        Task<(List<int> TxCdrLol, List<int>RxCdrLol)> RxLolAsync();
        (List<int> TxCdrLol, List<int> RxCdrLol) RxLol();

        Task<List<int>> TxManualCtleAsync();
        List<int> TxManualCtle();

        Task<double> TxPowerAsync();
        double TxPower();

        Task<double> SupplyVoltageAsync();
        double SupplyVoltage();

        Task<double> TxBiasAsync();
        double TxBias();

        Task<bool> RxLosAsync();
        bool RxLos();

        Task<bool> DataNotReadyAsync();
        bool DataNotReady();

        Task<bool> TxDisableAsync(EnableLane enable);
        bool TxDisable(EnableLane enable);

        Task<byte> TxDisableStateAsync();
        byte TxDisableState();

        Task<List<int>> TxAdaptiveCtleAsync();
        List<int> TxAdaptiveCtle();

    }
}