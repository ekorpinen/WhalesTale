using System;
using System.Threading.Tasks;
using DutGpio;
using Support;
using WhalesTale.Cyclops;
using WhalesTale.QSFP100;

namespace WhalesTale.Module
{
    public interface IModule
    {
        IDutGpio DutGpio { get; }
        ICyclops Cyclops { get; }
        IQsfp100G Qsfp100G { get; }


        Task<bool> CalibrateModuleTemperature(double heatSinkTemperature, bool zeroOffsetsFirst = false);

        Task<bool> CalibrateModuleTemperature2(double caseHotspotTemperature, double slope,
            bool zeroOffsetsFirst = false);

        void Dispose();

        Task<(bool success, TimeSpan bootTime)> BootTime(Action<bool> enable, TimeSpan timeOut);

        Task<(bool success, TimeSpan bootTime)> ResetWaitTillIntL(TimeSpan assertTime, TimeSpan timeOut,
            IProgress<TmgProgressReporting> progress = null);

        Task<bool> ResetAndDelay(TimeSpan assertTime, TimeSpan delayTime, TimeSpan timeOut,
            IProgress<TmgProgressReporting> progress = null);

        Task<bool> WaitDutTemperatureStable(double allowableChange, TimeSpan window, TimeSpan updateRate,
            TimeSpan timeout);

        Task<(bool success, TimeSpan bootTime)> ResetWaitTillReady(TimeSpan assertTime, TimeSpan timeOut,
            IProgress<TmgProgressReporting> progress = null);


        Task<(bool success, TimeSpan bootTime, int error, int exError)> ResetWaitTillReadyWithErrorCheck(
            TimeSpan assertTime, TimeSpan timeOut, IProgress<TmgProgressReporting> progress = null);
    }
}