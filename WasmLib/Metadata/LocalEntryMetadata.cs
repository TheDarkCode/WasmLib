using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata
{
	public class LocalEntryMetadata
	{
		public Local Local { get; set; }

		internal static LocalEntryMetadata Read(BinaryReader reader)
		{
			var local = new Local
			{
				Count = reader.ReadVarUint32(),
				Type = reader.ReadValueType()
			};

			return new LocalEntryMetadata {Local = local};
		}

		public void Write(BinaryWriter writer)
		{
			writer.WriteVarUint32(Local.Count);
			writer.WriteValueType(Local.Type);
		}
	}
}