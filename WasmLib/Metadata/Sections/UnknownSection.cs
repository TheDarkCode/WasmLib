using System.IO;

namespace WasmLib.Metadata.Sections
{
	public class UnknownSection : Section
	{
		public byte[] Data { get; set; } = new byte[0];

		protected override void ReadSpecific(BinaryReader reader)
		{
			Data = reader.ReadBytes((int) (Size - (ulong)reader.BaseStream.Position + Marker));
		}

		protected override void WriteSpecific(BinaryWriter writer)
		{
			writer.Write(Data);
		}
	}
}
