using System.Threading.Tasks;

namespace WhalesTale.QSFP100
{
    public interface IFec
    {
        Task<bool> HighSer();
        Task<uint> UnCorrectableFrameCount();
        Task<uint> CorrectedFrameCount();
        Task<uint> ErrorSymbolCount();
        Task<uint> BipCount();
        Task<uint> TotalWordCount();
        Task<byte> Status();
        Task<bool> IdleEnable(bool enable);
        Task<bool> ErrorInjectEnable(bool enable);
        Task<double> PreFecBer();
        Task<double> PreFecSer();
        Task<double> UnCorrectableWer();
        Task<double> CorrectableWer();

        Task<bool> ClearCounters();
        Task<bool> CountersEnable(bool enable);
    }
}