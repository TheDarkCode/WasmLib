using System.IO;
using WasmLib.Extensions;

namespace WasmLib.Metadata
{
	public class ExportMetadata
	{
		public string FunctionName { get; set; }
		public uint FunctionIndex { get; set; }

		public static ExportMetadata Read(BinaryReader reader)
		{
			var export = new ExportMetadata
			{
				FunctionIndex = reader.ReadVarUint32(),
				FunctionName = reader.ReadVarString()
			};

			return export;
		}

		public void Write(BinaryWriter writer)
		{
			writer.WriteVarUint32(FunctionIndex);
			writer.WriteVarString(FunctionName);
		}
	}
}
