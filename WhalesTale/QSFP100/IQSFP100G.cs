using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WhalesTale.QSFP;

namespace WhalesTale.QSFP100
{
    public interface IQsfp100G : IQsfp, ITemperatureCalibration
    {
        HostSide Host { get; }
        NetworkSide Network { get; }
        FecClass Fec { get; }

        Task<List<(int X, int Y)>> SlicerHistogramAsync(TimeSpan timeout);
        Task<SnrClass> SnrEstimatorAsync(TimeSpan timeout);

        //=======================below is good============================
        Task<byte[]> GetCiscoSpecificConfigurationAsync();
        byte[] GetCiscoSpecificConfiguration();

        Task<bool> Update_CalibrationAsync(int timeoutMs = 200);
        bool Update_Calibration(int timeoutMs = 200);

        Task<bool> ResetPrbsValidationAsync();
        bool ResetPrbsValidation();

        Task<double> CoCoaTemperatureAsync();
        double CoCoaTemperature();

        Task<double> RssiAsync();
        double Rssi();

        Task<double> Ps0P75Async();
        double Ps0P75();

        Task<double> Ps1P00Async();
        double Ps1P00();

        Task<double> Ps1P80Async();
        double Ps1P80();

        Task<double> Ps2P70Async();
        double Ps2P70();

        Task<double> RefV2P50Async();
        double RefV2P50();

        Task<ushort> ErrorCodeAsync();
        ushort ErrorCode();

        Task<ushort> ErrorOptionalDataAsync();
        ushort ErrorOptionalData();

        Task RxSquelchDisableSetAsync(byte whichLanes);
        Task RxSquelchDisableSetAsync(EnableLane whichLanes);
        Task RxSquelchDisableSetAsync(bool l0, bool l1, bool l2, bool l3);
        Task<byte> RxSquelchDisableGetAsync();

        Task TxSquelchDisableSetAsync(byte whichLanes);
        Task TxSquelchDisableSetAsync(EnableLane whichLanes);
        Task TxSquelchDisableSetAsync(bool l0, bool l1, bool l2, bool l3);
        Task<byte> TxSquelchDisableGetAsync();

        Task<bool> FEC_KR4_BypassEnableAsync(bool enable);
        Task<bool> FEC_KR4_BypassStateAsync();

  
        Task TxAdaptiveEqualizationSetAsync(byte whichLanes);
        Task TxAdaptiveEqualizationSetAsync(EnableLane whichLanes);
        Task TxAdaptiveEqualizationSetAsync(bool l0, bool l1, bool l2, bool l3);
        Task<byte> TxAdaptiveEqualizationGetAsync();
        Task<bool> VendorTestModeSetAsync(ushort mode);
        Task<ushort> VendorTestModeGetAsync();
    }
}