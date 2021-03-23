using System;
using System.Threading;
using System.Threading.Tasks;
using CiscoSub20;
using DutGpio;
using WhalesTale.Cyclops;
using WhalesTale.QSFP;
using WhalesTale.QSFP100;

namespace WhalesTale.Communication
{
    public interface IDeviceIO
    {
        IDutGpio Gpio { get; }
        II2C I2C { get; }
        void SetPage(byte page);

        byte ReadByte(byte page, byte readAddress);
        ushort ReadWord(byte page, byte readAddress);
        byte[] Read(byte page, byte readAddress, byte numBytes);

        Task<byte[]> ReadAsync(byte page, byte readAddress, byte numBytes); 

        void WriteByte(byte page, byte writeAddress, byte data);
        void WriteWord(byte page, byte writeAddress, ushort data);
        void Write(byte page, byte writeAddress, byte[] data);

        #region Vendor Command Methods
        
        VendorCommand.Status VendorCommandStatus { get; }

        void WriteModuleMfrPassword();

        void ClearModuleMfrPassword();
        
        void SetVendorCommandParameters(RegisterBase.RegisterBase reg);
        
        void SetVendorCommandParameters(byte page, byte address, ushort length);
        
        Task<bool> ExecuteVendorCmdAsync(VendorCommand.Command command, int timeoutMs, int stepMS = 100);

        #endregion Vendor Command Methods


        Task<dynamic> GetRegAsync(Sff8636 reg, CancellationToken ct = default);
        Task<double> GetValueAsync(Sff8636 reg, CancellationToken ct = default);
        Task<bool> SetRegAsync(Sff8636 reg, byte data, CancellationToken ct = default);

        Task<dynamic> GetRegAsync(Qsfp100GRegister reg, CancellationToken ct = default);
        Task<double> GetValueAsync(Qsfp100GRegister reg, CancellationToken ct = default);
        Task<bool> SetRegAsync(Qsfp100GRegister reg, byte[] data, CancellationToken ct = default);
        Task<bool> SetRegAsync(Qsfp100GRegister reg, byte data, CancellationToken ct = default);


        Task<int> GetRegAsync(CyclopsRegisters reg, CancellationToken ct = default);
        Task<bool> SetCiscoIpRegAsync(CyclopsRegisters reg, byte data, CancellationToken ct = default);
        //Task<bool> SetCiscoIpRegAsync(CyclopsRegisters reg, byte[] data, CancellationToken ct = default);
        Task<double> GetValueAsync(CyclopsRegisters reg, CancellationToken ct = default);


        Task<dynamic> GetMaComRegAsync(uint address, ushort len, CancellationToken ct = default);
        Task<bool> SetMaComRegAsync(uint address, ushort len, uint[] data, CancellationToken ct = default);
    }
}