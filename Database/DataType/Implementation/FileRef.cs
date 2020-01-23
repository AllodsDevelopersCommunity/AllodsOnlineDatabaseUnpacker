using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Xml.Linq;

namespace Database.DataType.Implementation
{
    public class FileRef : DataType
    {
        private IntPtr _href;

        public override XElement Serialize(string name)
        {
            if (_href == IntPtr.Zero) return new XElement(name, new XAttribute("href", ""));
            var cursor = Marshal.ReadIntPtr(_href + 12);
            var readByte = Marshal.ReadByte(cursor);
            var sb = new StringBuilder();
            while (readByte != 0)
            {
                sb.Append(Convert.ToChar(readByte));
                cursor += 1;
                readByte = Marshal.ReadByte(cursor);
            }

            var fileName = sb.ToString();
            var className = Utils.GetClassName(fileName);
            if (!GameDatabase.DoesFileExists(fileName))
            {
                Logger.Warn($"{fileName} is refenced but not extracted");
            }
            return new XElement(name, new XAttribute("href", $"/{fileName}#xpointer(/{className})"));
        }

        public override void Deserialize(IntPtr memoryAddress)
        {
            _href = Marshal.ReadIntPtr(memoryAddress);
        }
    }
}