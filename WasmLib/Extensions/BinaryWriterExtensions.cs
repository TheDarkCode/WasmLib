using System;
using System.IO;
using System.Text;

namespace WasmLib.Extensions
{
	public static class BinaryWriterExtensions
	{
		public static void WriteSizedPart(this BinaryWriter writer, Action<BinaryWriter> action, int padding = 0)
		{
			var buffer = new MemoryStream();
			using (var bufferWriter = new BinaryWriter(buffer))
			{
				action(bufferWriter);
				WriteVarUint32(writer, (uint) buffer.Length, padding);
				writer.Write(buffer.ToArray());
			}
		}

		public static void WriteVarUint32(this BinaryWriter writer, uint value, int padding = 0)
		{
			WriteULEB128(writer, value, padding);
		}

		public static void WriteVarString(this BinaryWriter writer, string value)
		{
			WriteVarUint32(writer, (uint) value.Length);
			var bytes = Encoding.UTF8.GetBytes(value);
			writer.Write(bytes);
		}

		public static void WriteValueType(this BinaryWriter writer, ValueType value)
		{
			writer.Write((byte) value);
		}

		private static void WriteULEB128(this BinaryWriter writer, ulong value, int padding)
		{
			var count = 0;
			do
			{
				var bt = (byte) ((byte) value & 0x7f);
				value >>= 7;
				if (value != 0 || padding != 0)
					bt |= 0x80;

				writer.Write(bt);
				count++;
			} while (value != 0);

			padding -= count;
			if (padding <= 0)
				return;

			for (; padding != 1; --padding)
				writer.Write((byte) 0x80);

			writer.Write((byte) 0x0);
		}
	}
}

