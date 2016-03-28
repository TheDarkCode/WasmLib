using System;
using System.IO;
using System.Text;

namespace WasmLib.Extensions
{
	public static class BinaryReaderExtensions
	{
		public static uint ReadVarUint32(this BinaryReader reader)
		{
			return (uint) ReadULEB128(reader);
		}

		public static string ReadVarString(this BinaryReader reader)
		{
			var length = reader.ReadVarUint32();
			var bytes = reader.ReadBytes((int) length);
			return Encoding.UTF8.GetString(bytes);
		}

		public static ValueType ReadValueType(this BinaryReader reader)
		{
			var value = reader.ReadByte();

			if (!Enum.IsDefined(typeof (ValueType), (int) value))
				throw new NotSupportedException($"unexpected value type: {value}");

			return (ValueType) value;
		}

		private static ulong ReadULEB128(this BinaryReader reader)
		{
			ulong value = 0;
			var shift = 0;
			while (true)
			{
				var bt = reader.ReadByte();
				value += (ulong) (bt & 0x7f) << shift;

				if (bt < 128)
					break;

				shift += 7;
			}
			return value;
		}
	}
}

