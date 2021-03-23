// ***********************************************************************
// Assembly         : Registers
// Author           : Errol Korpinen
// Created          : 12-09-2018
//
// Last Modified By : labuser
// Last Modified On : 12-09-2018
// ***********************************************************************
// <copyright file="TP3.cs" company="Cisco TMG">
//     Copyright ©  2018
// </copyright>
// <summary></summary>
// ***********************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Polly;

namespace WhalesTale.QSFP100
{
    public partial class Qsfp100G : IQsfp100G
    {
        public async Task<List<(int X, int Y)>> SlicerHistogramAsync(TimeSpan timeout)
        {
            var histogramData = new List<(int X, int Y)>();
            if (!await Device.ExecuteVendorCmdAsync(VendorCommand.Command.SlicerHistogram,
                    Convert.ToInt32(timeout.TotalMilliseconds), 1000)
                .ConfigureAwait(false)) throw new Exception("Failed to execute Vendor Command CiscoCocoaSetVoaByCtp");
            var data = await Device.ReadAsync(VendorCommand.Page2, VendorCommand.Buffer.Address, 128)
                .ConfigureAwait(false);
            for (ushort i = 0; i < 64; i++) histogramData.Add((i - 32, (data[i * 2] << 8) | data[i * 2 + 1]));
            return histogramData;
        }

        public async Task<SnrClass> SnrEstimatorAsync(TimeSpan timeout)
        {
            var policyResult = await Policy
                .Handle<Exception>()
                .RetryAsync(3, onRetry: async (exception, retryCount) =>
                {
                    Debug.WriteLine($"SnrEstimator error ( retry {retryCount} : {exception.Message})");
                    await Task.Delay(2000);
                })
                .ExecuteAndCaptureAsync(async () =>
                {
                    var results = new SnrClass();

                    var executeVendorCmdResult = await Device.ExecuteVendorCmdAsync(VendorCommand.Command.SnrEstimator,
                        Convert.ToInt32(timeout.TotalMilliseconds));
                    if (!executeVendorCmdResult) throw new Exception("Failed to execute Vendor Command SnrEstimator");

                    // Collect SNR per level (4 dw), Eye SNR (3 dw) and FW status (1 dw) + 4 bytes Skew
                    var data = await Device.ReadAsync(VendorCommand.Page2, VendorCommand.Buffer.Address, 36);

                    results.Snr0 = ((data[0] << 24) | (data[1] << 16) | (data[2] << 8) | data[3]) / 256.0;
                    results.Snr1 = ((data[4] << 24) | (data[5] << 16) | (data[6] << 8) | data[7]) / 256.0;
                    results.Snr2 = ((data[8] << 24) | (data[9] << 16) | (data[10] << 8) | data[11]) / 256.0;
                    results.Snr3 = ((data[12] << 24) | (data[13] << 16) | (data[14] << 8) | data[15]) / 256.0;

                    results.SnrEyeL = ((data[16] << 24) | (data[17] << 16) | (data[18] << 8) | data[19]) / 256.0;
                    results.SnrEyeM = ((data[20] << 24) | (data[21] << 16) | (data[22] << 8) | data[23]) / 256.0;
                    results.SnrEyeU = ((data[24] << 24) | (data[25] << 16) | (data[26] << 8) | data[27]) / 256.0;

                    results.Status = (data[28] << 24) | (data[29] << 16) | (data[30] << 8) | data[31];

                    results.Skew0 = data[32];
                    results.Skew1 = data[33];
                    results.Skew2 = data[34];
                    results.Skew3 = data[35];

                    return results;
                });
            if (policyResult.Outcome == OutcomeType.Successful) return policyResult.Result;
            throw new Exception($"Failed SnrEstimatorAsync : {policyResult.FinalException}");
        }
    }
}