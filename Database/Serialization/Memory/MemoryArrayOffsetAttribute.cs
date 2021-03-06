using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NLog;

namespace Database.Serialization.Memory
{
    public class MemoryArrayOffsetAttribute : MemoryOffsetAttribute
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private readonly int _itemSize;

        public MemoryArrayOffsetAttribute(int offset, int itemSize) : base(offset)
        {
            _itemSize = itemSize;
        }

        public override void DeserializeField(FieldInfo field, IntPtr memoryAddress, object obj)
        {
            Logger.Debug($"Deserializing {field.Name} at {memoryAddress.ToString("x8")}");
            var startMemoryAddress = Marshal.ReadIntPtr(memoryAddress + Offset);
            var endMemoryAddress = Marshal.ReadIntPtr(memoryAddress + Offset + 4);
            var count = (endMemoryAddress.ToInt32() - startMemoryAddress.ToInt32()) / _itemSize;
            var elementType = field.FieldType.GetElementType();
            if (!(elementType is null) && typeof(IMemoryDeserializable).IsAssignableFrom(elementType))
            {
                var array = Array.CreateInstance(elementType, count);
                for (var i = 0; i < count; i++)
                {
                    var entry = (IMemoryDeserializable) Activator.CreateInstance(elementType);
                    entry.Deserialize(startMemoryAddress + i * _itemSize + 4);
                    array.SetValue(entry, i);
                }

                field.SetValue(obj, array);
            }
            else
            {
                throw new Exception("Cannot deserialize field");
            }
        }
    }
}