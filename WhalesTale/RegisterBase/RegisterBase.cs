using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace WhalesTale.RegisterBase
{
    public sealed class Access
    {
        public static readonly Access Unknown = new Access(-1, "Unknown");
        public static readonly Access ReadWrite = new Access(0, "Read and or write.");
        public static readonly Access ReadOnly = new Access(1, "Read only.");

        public readonly string Name;
        public readonly int Value;

        private Access(int value, string name)
        {
            Name = name;
            Value = value;
        }
    }

    public sealed class DataType
    {
        public static readonly DataType Unknown = new DataType(-1, "Unknown");
        public static readonly DataType Hex = new DataType(0, "Hex");
        public static readonly DataType HexWord = new DataType(1, "Hex word (2 bytes)");
        public static readonly DataType Dec = new DataType(2, "Decimal");
        public static readonly DataType DecWord = new DataType(3, "Decimal word (2 bytes)");
        public static readonly DataType String = new DataType(4, "String");
        public static readonly DataType Binary = new DataType(5, "Binary");
        public static readonly DataType Int32 = new DataType(6, "4 bytes");

        public readonly string Name;
        public readonly int Value;

        private DataType(int value, string name)
        {
            Name = name;
            Value = value;
        }
    }

    public class PageBase
    {
        public PageBase(byte page, byte startAddress, byte length)
        {
            Page = page;
            StartAddress = startAddress;
            Length = length;
        }

        public byte Page { get; }
        public byte StartAddress { get; }
        public byte Length { get; }
    }


    /// <summary>
    ///     Class RegisterBase.
    /// </summary>
    public class RegisterBase
    {
        public Access Access;
        public byte Address;
        public bool Dynamic;
        public string Name;
        public byte Page;
        public double Scale;
        public bool Signed;
        public byte Size;
        public DataType Type;

        public RegisterBase(byte page, byte address, byte size, Access access, DataType type, string name,
            bool signed = false, double scale = 0.0, bool dynamic = false)
        {
            Page = page;
            Address = address;
            Size = size;
            Access = access;
            Type = type;
            Name = name;
            Signed = signed;
            Scale = scale;
            Dynamic = dynamic;
        }
    }

    public class BaseReg<T, TU> where T : class
    {
        protected static Dictionary<TU, T> Library = new Dictionary<TU, T>();

        protected BaseReg(TU value, string name)
        {
            Name = name;
            Value = value;
        }

        public string Name { get; }
        public TU Value { get; }

        public static Dictionary<TU, T> Dic
        {
            get
            {
                RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
                return Library;
            }
            private set => Library = value;
        }

        public override string ToString() => Name;

        public static T Get(TU key)
        {
            RuntimeHelpers.RunClassConstructor(typeof(T).TypeHandle);
            return Library[key];
        }
    }

    /// <summary>
    ///     Class BaseRegLanes.
    /// </summary>
    public class BaseRegLanes
    {
        public BaseRegLanes(byte lane1, byte lane2, byte lane3, byte lane4)
        {
            Lane1 = lane1;
            Lane2 = lane2;
            Lane3 = lane3;
            Lane4 = lane4;

            List.Add(Lane1);
            List.Add(Lane2);
            List.Add(Lane3);
            List.Add(Lane4);
        }

        public byte Lane1 { get; }
        public byte Lane2 { get; }
        public byte Lane3 { get; }
        public byte Lane4 { get; }
        public List<byte> List { get; }

        public override string ToString() => $"{Lane1}, {Lane2}, {Lane3}, {Lane4}";
    }

    /// <summary>
    ///     Class BaseRegAlarmsWarnings.
    /// </summary>
    public class BaseRegAlarmsWarnings
    {
        public BaseRegAlarmsWarnings(byte highAlarm, byte lowAlarm, byte highWarning, byte lowWarning)
        {
            HighAlarm = highAlarm;
            LowAlarm = lowAlarm;
            HighWarning = highWarning;
            LowWarning = lowWarning;
        }

        public byte HighAlarm { get; }
        public byte LowAlarm { get; }
        public byte HighWarning { get; }
        public byte LowWarning { get; }

        public override string ToString() => $"{HighAlarm}, {LowAlarm}, {HighWarning}, {LowWarning}";
    }
}