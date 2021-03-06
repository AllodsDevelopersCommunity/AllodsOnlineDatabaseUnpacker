using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Database.DataType.Implementation
{
    public class AsciiString : DataType
    {
        private string _value;

        public AsciiString(string value)
        {
            _value = value;
        }

        public AsciiString()
        {
        }

        public override XElement Serialize(string name)
        {
            return new XElement(name, _value);
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            var startAddress = Marshal.ReadIntPtr(memoryAddress);
            var endAddress = Marshal.ReadIntPtr(memoryAddress + 4);
            var sb = new StringBuilder();
            for (var localPtr = startAddress; localPtr != endAddress; localPtr += 1)
            {
                var readByte = Marshal.ReadByte(localPtr);
                sb.Append(Convert.ToChar(readByte));
            }

            _value = sb.ToString();
        }
    }
}