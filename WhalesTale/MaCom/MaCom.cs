using System;
using System.Threading;
using System.Threading.Tasks;
using WhalesTale.Communication;

namespace WhalesTale.MaCom
{
    public class MaCom : IMaCom
    {
       public  MaCom(IDeviceIO deviceIO)
        {
            Device = deviceIO;
        }

       
       public IDeviceIO Device { get; }

        public async Task<int> FwDebugAsync(CancellationToken ct)
        {
            return (int) await Device.GetMaComRegAsync(0x20000028, 1, ct).ConfigureAwait(false);
        }

        public async Task<int> FwDebug2Async(CancellationToken ct)
        {
            return (int)await Device.GetMaComRegAsync(0x20000088, 1, ct).ConfigureAwait(false);
        }
        
        public async Task<int> TiaCoarseAsync(CancellationToken ct)
        {
            var tiaCoarse = await Device.GetMaComRegAsync(0x500043e0, 1, ct).ConfigureAwait(false);
            return (int)(tiaCoarse % Math.Pow(2, 3));
        }

        public async Task<int> TiaFineAsync(CancellationToken ct)
        {
            var tiaFine = await Device.GetMaComRegAsync(0x500040f0, 1, ct).ConfigureAwait(false);
            return (int)(tiaFine % Math.Pow(2, 4));
        }

        public async Task<int> PvtEfuse(CancellationToken ct)
        {
            await Device.SetMaComRegAsync(0x50080054, 1, new uint[1] { 0xe8000a64 }, ct).ConfigureAwait(false);
            var pvtEfuse = (int)await Device.GetMaComRegAsync(0x50080060, 1, ct).ConfigureAwait(false);
            return (int)Math.Floor((pvtEfuse / Math.Pow(2, 12)) % (int)Math.Pow(2, 6));
        }

        public async Task<int> PvttApplied(CancellationToken ct)
        {
            var pvttApplied = (int)await Device.GetMaComRegAsync(0x50004108, 1, ct).ConfigureAwait(false);
            return (int)pvttApplied % (int)Math.Pow(2, 6);
        }

        public async Task<(int Skew0,int Skew1,int Skew2, int Skew3)> Skews(CancellationToken ct)
        {
            var skew = (int)await Device.GetMaComRegAsync(0x500a800c, 1, ct).ConfigureAwait(false);
            return
            (
                Skew0: (int) (Math.Floor(skew / Math.Pow(2, 0)) % Math.Pow(2, 8)),
                Skew1: (int) (Math.Floor(skew / Math.Pow(2, 8)) % Math.Pow(2, 8)),
                Skew2: (int) (Math.Floor(skew / Math.Pow(2, 16)) % Math.Pow(2, 8)),
                Skew3: (int) (Math.Floor(skew / Math.Pow(2, 24)) % Math.Pow(2, 8))
            );
        }

        public async Task<decimal> AdcBackoff(CancellationToken ct)
        {
            var adcBackoff = (int)await Device.GetMaComRegAsync(0x500a0384, 1, ct).ConfigureAwait(false);
            return (decimal)(10.0 * Math.Log10(adcBackoff / Math.Pow(2, 30)));
        }


        public async Task<decimal> Tap(Taps whichTap, CancellationToken ct)
        {
            await Device.SetMaComRegAsync(0x500a0230, 1, new uint[1] { (uint)whichTap }, ct).ConfigureAwait(false);
            await Device.SetMaComRegAsync(0x500a01f4, 1, new uint[1] { 0x00 }, ct).ConfigureAwait(false);
            await Device.SetMaComRegAsync(0x500a01f4, 1, new uint[1] { 0x02 }, ct).ConfigureAwait(false);
            var tap0 = (int)await Device.GetMaComRegAsync(0x500a024c, 1, ct).ConfigureAwait(false);
            return (tap0 > Math.Pow(2, 29))
                ? (decimal)((tap0 - Math.Pow(2, 30)) / (Math.Pow(2, 28)))
                : (decimal)(tap0 / Math.Pow(2, 28));
        }

        public async Task<(decimal Level0, decimal Level1, decimal Level2, decimal Level3)> Levels(CancellationToken ct)
        {
            var levelRead = (int)await Device.GetMaComRegAsync(0x500a06c0, 1, ct).ConfigureAwait(false);
            var level0 = (Math.Floor(levelRead / Math.Pow(2, 0)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal) (((Math.Floor(levelRead / Math.Pow(2, 0)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                             Math.Pow(2, 3))
                : (decimal) ((Math.Floor(levelRead / Math.Pow(2, 0)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

            var  level1 = (Math.Floor(levelRead / Math.Pow(2, 16)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal) (((Math.Floor(levelRead / Math.Pow(2, 16)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                             Math.Pow(2, 3))
                : (decimal) ((Math.Floor(levelRead / Math.Pow(2, 16)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

             levelRead = (int)await Device.GetMaComRegAsync(0x500a06c4, 1, ct).ConfigureAwait(false);
            var level2 = (Math.Floor(levelRead / Math.Pow(2, 0)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal)(((Math.Floor(levelRead / Math.Pow(2, 0)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                            Math.Pow(2, 3))
                : (decimal)((Math.Floor(levelRead / Math.Pow(2, 0)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

            var level3 = (Math.Floor(levelRead / Math.Pow(2, 16)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal)(((Math.Floor(levelRead / Math.Pow(2, 16)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                            Math.Pow(2, 3))
                : (decimal)((Math.Floor(levelRead / Math.Pow(2, 16)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

            return
            (
                Level0: level0,
                Level1: level1,
                Level2: level2,
                Level3: level3
            );
        }

        public async Task<(decimal Decision01 , decimal Decision12, decimal Decision23)> Decisions(CancellationToken ct)
        {
            var decision = (int)await Device.GetMaComRegAsync(0x500a06c8, 1, ct).ConfigureAwait(false);

            var decision01 = (Math.Floor(decision / Math.Pow(2, 0)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal)(((Math.Floor(decision / Math.Pow(2, 0)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                            Math.Pow(2, 3))
                : (decimal)((Math.Floor(decision / Math.Pow(2, 0)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

            var decision12 = (Math.Floor(decision / Math.Pow(2, 16)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal)(((Math.Floor(decision / Math.Pow(2, 16)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                            Math.Pow(2, 3))
                : (decimal)((Math.Floor(decision / Math.Pow(2, 16)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

            decision = (int)await Device.GetMaComRegAsync(0x500a06cc, 1, ct).ConfigureAwait(false);
            var decision23 = (Math.Floor((double)decision / Math.Pow(2, 0)) % Math.Pow(2, 12) > Math.Pow(2, 11))
                ? (decimal)(((Math.Floor((double)decision / Math.Pow(2, 0)) % Math.Pow(2, 12)) - Math.Pow(2, 12)) /
                            Math.Pow(2, 3))
                : (decimal)((Math.Floor((double)decision / Math.Pow(2, 0)) % Math.Pow(2, 12)) / Math.Pow(2, 3));

            
            return
            (
                Decision01: decision01,
                Decision12: decision12,
                Decision23: decision23
            );
        }
        public async Task<decimal> PpmOffset(CancellationToken ct)
        {
            var ppmOffset = (int)await Device.GetMaComRegAsync(0x500A02B8, 1, ct).ConfigureAwait(false);
            return (ppmOffset > Math.Pow(2, 29))
                ? (decimal)((ppmOffset - Math.Pow(2, 30)) / 1717.0)
                : (decimal)(ppmOffset / 1717.0); ;
        }

        public enum Taps
       {
           Tap0= 0x00 ,Tap1= 0x400, Tap2 = 0x800, Tap3 = 0xC00,
           Tap4 = 0x1000, Tap5 = 0x1400, Tap6 = 0x1800, Tap7 = 0x1C00,
           Tap8 = 0x2000, Tap9 = 0x2400, Tap10 = 0x2800, Tap11 = 0x2C00,
           Tap12 = 0x3000, Tap13 = 0x3400, Tap14 = 0x3800, Tap15 = 0x3C00,
       }
    }
}
